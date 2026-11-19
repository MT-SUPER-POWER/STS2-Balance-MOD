using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Connection;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using Sts2BalanceMod.Sts2BalanceModCode.Settings;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Multiplayer;

internal sealed record BalanceConfigMismatchInfo(
  string[] DifferenceIds,
  IReadOnlyDictionary<string, bool> HostValues,
  IReadOnlyDictionary<string, bool> ClientValues) : ConnectionFailureExtraInfo;

internal sealed record BalanceConfigReadFailureInfo(BalanceConfigReadError Error) : ConnectionFailureExtraInfo;

/// <summary>
/// Append our active gameplay choices at the host/client send boundary.
/// WARNING: the configuration trailer follows the original HandshakeManager payload.
/// </summary>
internal static class BalanceWriteHandshakePatch
{
  internal static Dictionary<string, bool> Snapshot()
  {
    var dict = new Dictionary<string, bool>(StringComparer.Ordinal);
    foreach (var c in BalanceCatalog.All)
    {
      if (c.AffectsGameplay)
      {
        dict[c.Id] = BalanceModSettings.IsEnabled(c.Id);
      }
    }
    return dict;
  }

  internal static void Append(ulong peerId, PacketWriter writer)
  {
    try
    {
      byte[] payload = BalanceConfigWire.Encode(Snapshot());
      writer.WriteUInt(BalanceConfigWire.Magic);
      writer.WriteUShort((ushort)payload.Length);
      writer.WriteBytes(payload, payload.Length);
      Log.Info($"[Sts2BalanceMod] Appended balance configuration handshake ({payload.Length} bytes, {payload.Length / 4} options) to peer {peerId}.");
    }
    catch (Exception ex)
    {
      Log.Error($"[Sts2BalanceMod] Failed to append balance handshake to peer {peerId}: {ex}");
    }
  }
}

[HarmonyPatch(typeof(NetHostGameService), nameof(NetHostGameService.SendHandshakeMessage))]
internal static class BalanceHostSendHandshakePatch
{
  private static void Prefix(ulong peerId, PacketWriter writer) =>
    BalanceWriteHandshakePatch.Append(peerId, writer);
}

[HarmonyPatch(typeof(NetClientGameService), nameof(NetClientGameService.SendHandshakeMessage))]
internal static class BalanceClientSendHandshakePatch
{
  private static void Prefix(ulong peerId, PacketWriter writer) =>
    BalanceWriteHandshakePatch.Append(peerId, writer);
}

/// <summary>
/// Compare active settings after the vanilla checks and attach details to the native error.
/// WARNING: the native success result carries peer version info and the reader is at the trailer.
/// </summary>
[HarmonyPatch(typeof(HandshakeManager), "TryReadHandshakeMessage")]
internal static class BalanceReadHandshakePatch
{
  private static void Postfix(HandshakeManager __instance, ulong senderId, PacketReader reader,
    IHandshakeHandler ____handler, ref HandshakeResult __result)
  {
    if (__result.status != HandshakeStatus.Success) return;

    var original = __result.extraInfo;
    bool isHost = original?.localIsHost ?? false;
    var readError = ReadConfiguration(reader, out var remote);
    if (remote == null)
    {
      Log.Error($"[Sts2BalanceMod] Could not read balance configuration from peer {senderId}: {readError}.");
      var failure = new BalanceConfigReadFailureInfo(readError)
      {
        localInfo = original?.localInfo ?? PeerVersionInfo.LocalDefault(),
        remoteInfo = original?.remoteInfo ?? PeerVersionInfo.LocalDefault(),
        localIsHost = isHost
      };
      // The Steam disconnect event can arrive before ConnectionFailed. Clear the pending
      // handshake first so the game dispatches this error with its details.
      __instance.AbortHandshake(senderId);
      __result = new HandshakeResult(HandshakeStatus.InvalidHandshake);
      ____handler.HandshakeFailed(senderId, new NetErrorInfo(NetError.InvalidHandshake, selfInitiated: false)
      {
        ConnectionExtraInfo = failure
      });
      return;
    }

    Log.Info($"[Sts2BalanceMod] Read balance configuration ({remote.Count} options) from peer {senderId}.");
    var local = BalanceWriteHandshakePatch.Snapshot();
    string[] differences = BalanceConfigWire.Differences(local, remote);
    if (differences.Length == 0) return;

    var host = isHost ? local : remote;
    var client = isHost ? remote : local;
    __result.extraInfo = new BalanceConfigMismatchInfo(differences, host, client)
    {
      localInfo = original?.localInfo ?? PeerVersionInfo.LocalDefault(),
      remoteInfo = original?.remoteInfo ?? PeerVersionInfo.LocalDefault(),
      localIsHost = isHost
    };
    __result.status = HandshakeStatus.ModMismatch;
    Log.Warn($"[Sts2BalanceMod] Balance settings differ with peer {senderId}: {string.Join(", ", differences)}.");
  }

  private static BalanceConfigReadError ReadConfiguration(PacketReader reader, out Dictionary<string, bool>? remote)
  {
    remote = null;
    long availableBits = (long)reader.Buffer.Length * 8 - reader.BitPosition;
    if (availableBits < 32) return BalanceConfigReadError.MissingHeader;
    if (reader.ReadUInt() != BalanceConfigWire.Magic) return BalanceConfigReadError.UnrecognizedHeader;
    if (availableBits < 48) return BalanceConfigReadError.TruncatedLength;

    int length = reader.ReadUShort();
    if (!BalanceConfigWire.ValidPayloadLength(length)) return BalanceConfigReadError.InvalidLength;
    if (availableBits - 48 < length * 8) return BalanceConfigReadError.TruncatedPayload;

    byte[] payload = new byte[length];
    reader.ReadBytes(payload, length);
    try
    {
      remote = BalanceConfigWire.Decode(payload);
      return BalanceConfigReadError.None;
    }
    catch (InvalidDataException)
    {
      return BalanceConfigReadError.InvalidEntries;
    }
  }
}

/// <summary>
/// Use the vanilla network error popup for this mod's typed errors.
/// WARNING: the original error_popup scene owns the VerticalPopup and Description nodes.
/// </summary>
[HarmonyPatch(typeof(NErrorPopup), nameof(NErrorPopup.Create), [typeof(NetErrorInfo)])]
internal static class BalanceConfigErrorPopupPatch
{
  private static bool Prefix(NetErrorInfo info, ref NErrorPopup? __result)
  {
    string? language = LocManager.Instance?.Language;
    switch (info.ConnectionExtraInfo)
    {
      case BalanceConfigReadFailureInfo failure:
        var (title, body) = BalanceConfigDiagnostics.ReadFailure(language, failure.Error, failure.localIsHost);
        __result = NErrorPopup.Create(title, body, false);
        return false;
      case BalanceConfigMismatchInfo mismatch:
        __result = CreateMismatchPopup(language, mismatch);
        return false;
      default:
        return true;
    }
  }

  private static NErrorPopup? CreateMismatchPopup(string? language, BalanceConfigMismatchInfo mismatch)
  {
    string title = BalanceConfigDiagnostics.MismatchTitle(language);
    string[] lines = BalanceConfigDiagnostics.MismatchLines(language,
      mismatch.DifferenceIds, mismatch.HostValues, mismatch.ClientValues, mismatch.localIsHost);
    string summary = $"{BalanceConfigDiagnostics.MismatchSummary(language, lines.Length)}\n\n{lines[0]}";
    NErrorPopup? popup = NErrorPopup.Create(title, summary, false);
    if (popup == null) return null;

    string details = string.Join("\n\n", lines);
    string detailsLabel = BalanceConfigDiagnostics.IsChinese(language) ? "详情" : "Details";
    string backLabel = BalanceConfigDiagnostics.IsChinese(language) ? "返回" : "Back";
    popup.Ready += () =>
    {
      var vertical = popup.GetNode<NVerticalPopup>("VerticalPopup");
      var description = vertical.GetNode<MegaRichTextLabel>("Description");
      var button = vertical.NoButton;
      button.IsYes = false;
      button.SetText(detailsLabel);
      button.Show();
      bool showingDetails = false;

      // InitNoButton closes the modal. Bind the native button directly to switch views.
      button.Released += _ =>
      {
        showingDetails = !showingDetails;
        description.AutoSizeEnabled = !showingDetails;
        description.ScrollActive = showingDetails;
        description.HorizontalAlignment = showingDetails ? HorizontalAlignment.Left : HorizontalAlignment.Center;
        description.VerticalAlignment = showingDetails ? VerticalAlignment.Top : VerticalAlignment.Center;
        vertical.SetText(title, showingDetails ? details : summary);
        if (showingDetails) description.ScrollToLine(0);
        button.SetText(showingDetails ? backLabel : detailsLabel);
      };
    };
    return popup;
  }
}

/// <summary>
/// Include a short explanation in native surfaces that only render the error body.
/// </summary>
[HarmonyPatch(typeof(NetErrorInfo), nameof(NetErrorInfo.GetErrorString))]
internal static class BalanceConfigErrorTextPatch
{
  private static void Postfix(NetErrorInfo __instance, ref string __result)
  {
    string? language = LocManager.Instance?.Language;
    switch (__instance.ConnectionExtraInfo)
    {
      case BalanceConfigReadFailureInfo failure:
        __result = BalanceConfigDiagnostics.ReadFailure(language, failure.Error, failure.localIsHost).Body;
        break;
      case BalanceConfigMismatchInfo mismatch:
        __result = string.Join("\n", BalanceConfigDiagnostics.MismatchLines(language,
          mismatch.DifferenceIds.Take(3), mismatch.HostValues, mismatch.ClientValues, mismatch.localIsHost));
        break;
    }
  }
}
