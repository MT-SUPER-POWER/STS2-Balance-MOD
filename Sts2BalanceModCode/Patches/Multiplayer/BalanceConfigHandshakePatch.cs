using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Connection;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using Sts2BalanceMod.Sts2BalanceModCode.Settings;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Multiplayer;

internal sealed record BalanceConfigMismatchInfo : ConnectionFailureExtraInfo
{
  internal required string Details { get; init; }
}

/// <summary>
/// Append our active gameplay choices after the vanilla version handshake.
/// WARNING: verified against HandshakeManager.WriteHandshakeMessage and TryReadHandshakeMessage;
/// vanilla fields/order must remain untouched when updating to a new game version.
/// </summary>
[HarmonyPatch(typeof(HandshakeManager), nameof(HandshakeManager.WriteHandshakeMessage),
  [typeof(ulong), typeof(PeerVersionInfo), typeof(PacketWriter)])]
internal static class BalanceWriteHandshakePatch
{
  internal static Dictionary<string, bool> Snapshot() => BalanceCatalog.All.Where(c => c.AffectsGameplay)
    .ToDictionary(c => c.Id, c => BalanceModSettings.IsEnabled(c.Id), StringComparer.Ordinal);

  private static void Postfix(PacketWriter writer)
  {
    byte[] payload = BalanceConfigWire.Encode(Snapshot());
    writer.WriteUInt(BalanceConfigWire.Magic);
    writer.WriteUShort((ushort)payload.Length);
    writer.WriteBytes(payload, payload.Length);
  }
}

/// <summary>
/// Only augment a successful vanilla handshake. Other error categories retain vanilla behavior.
/// ModMismatch is the existing transport carrier; the extra-info subtype identifies our own error.
/// </summary>
[HarmonyPatch(typeof(HandshakeManager), "TryReadHandshakeMessage")]
internal static class BalanceReadHandshakePatch
{
  private static void Postfix(HandshakeManager __instance, ulong senderId, PacketReader reader,
    IHandshakeHandler ____handler, ref HandshakeResult __result)
  {
    if (__result.status != HandshakeStatus.Success) return;
    Dictionary<string, bool> remote;
    try
    {
      if ((long)reader.BitPosition + 48 > (long)reader.Buffer.Length * 8 || reader.ReadUInt() != BalanceConfigWire.Magic)
        throw new InvalidDataException("Missing balance configuration handshake.");
      int length = reader.ReadUShort();
      if (length == 0 || length > BalanceConfigWire.MaxEntries * 4 || length % 4 != 0 ||
          (long)reader.BitPosition + length * 8 > (long)reader.Buffer.Length * 8)
        throw new InvalidDataException("Truncated balance configuration handshake.");
      byte[] payload = new byte[length];
      reader.ReadBytes(payload, length);
      remote = BalanceConfigWire.Decode(payload);
    }
    catch (InvalidDataException)
    {
      // Vanilla has already cancelled the handshake timer. Explicitly use its existing
      // abort/failure cleanup instead of leaving the join waiting forever or reporting a mismatch.
      __instance.AbortHandshake(senderId);
      __result = new HandshakeResult(HandshakeStatus.InvalidHandshake);
      ____handler.HandshakeFailed(senderId, new NetErrorInfo(NetError.InvalidHandshake, selfInitiated: false));
      return;
    }
    var local = BalanceWriteHandshakePatch.Snapshot();
    string[] differences = BalanceConfigWire.Differences(local, remote);
    if (differences.Length == 0) return;
    var original = __result.extraInfo!;
    var host = original.localIsHost ? local : remote;
    var client = original.localIsHost ? remote : local;
    string Value(IReadOnlyDictionary<string, bool> values, string id) =>
      values.TryGetValue(id, out bool enabled) ? (enabled ? "开启" : "关闭") : "无此选项";
    string Line(string id)
    {
      var change = BalanceCatalog.All.FirstOrDefault(c => c.Id == id);
      string name = change == null ? id : $"{change.Group} / {change.Label}";
      return $"{name}：房主 {Value(host, id)}，{(original.localIsHost ? "加入者" : "本人")} {Value(client, id)}";
    }
    __result.extraInfo = new BalanceConfigMismatchInfo
    {
      localInfo = original.localInfo, remoteInfo = original.remoteInfo, localIsHost = original.localIsHost,
      Details = "平衡调整 Mod 的生效配置不同，无法加入房间。\n调整为一致并重启游戏后重试。\n\n" + string.Join("\n", differences.Select(Line))
    };
    __result.status = HandshakeStatus.ModMismatch;
  }
}

// Target only our subtype; preserve all other NErrorPopup and NetErrorInfo behavior.
[HarmonyPatch(typeof(NErrorPopup), nameof(NErrorPopup.Create), [typeof(NetErrorInfo)])]
internal static class BalanceConfigErrorPopupPatch
{
  private static bool Prefix(NetErrorInfo info, ref NErrorPopup? __result)
  {
    if (info.ConnectionExtraInfo is not BalanceConfigMismatchInfo mismatch) return true;
    __result = NErrorPopup.Create("Mod 配置不一致", mismatch.Details, false);
    return false;
  }
}

[HarmonyPatch(typeof(NetErrorInfo), nameof(NetErrorInfo.GetErrorString))]
internal static class BalanceConfigErrorTextPatch
{
  private static void Postfix(NetErrorInfo __instance, ref string __result)
  {
    if (__instance.ConnectionExtraInfo is BalanceConfigMismatchInfo mismatch) __result = mismatch.Details;
  }
}
