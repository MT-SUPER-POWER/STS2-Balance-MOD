using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Multiplayer;
using MegaCrit.Sts2.Core.Multiplayer.Connection;
using MegaCrit.Sts2.Core.Multiplayer.Serialization;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using Sts2BalanceMod.Sts2BalanceModCode.Settings;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Multiplayer;

internal sealed record BalanceConfigMismatchInfo : ConnectionFailureExtraInfo
{
  internal required string[] DifferenceIds { get; init; }
  internal required IReadOnlyDictionary<string, bool> HostValues { get; init; }
  internal required IReadOnlyDictionary<string, bool> ClientValues { get; init; }
  internal string? CustomTitle { get; init; }
  internal string? CustomDetails { get; init; }

  internal string Details => GetDetails(LocManager.Instance?.Language);

  internal string GetTitle(string? lang)
  {
    if (CustomTitle != null) return CustomTitle;
    return lang is "zhs" or "zht" ? "Mod 参数配置不一致" : "Mod Configuration Mismatch";
  }

  internal string GetDetails(string? lang)
  {
    if (CustomDetails != null) return CustomDetails;
    return FormatDetails(lang ?? "eng", DifferenceIds, HostValues, ClientValues, localIsHost);
  }

  private static string FormatDetails(
    string lang,
    string[] differenceIds,
    IReadOnlyDictionary<string, bool> hostValues,
    IReadOnlyDictionary<string, bool> clientValues,
    bool isHost)
  {
    bool isZh = lang is "zhs" or "zht";
    string header = isZh
      ? "平衡调整 Mod (Sts2BalanceMod) 的生效配置与房主不一致，无法加入房间。\n请将配置调整为一致并重启游戏后重试。\n\n不一致的选项："
      : "STS2 Balance Mod (Sts2BalanceMod) configuration does not match the host.\nPlease match the settings and restart the game before retrying.\n\nMismatched options:";

    string hostRole = isHost
      ? (isZh ? "本人(房主)" : "You (Host)")
      : (isZh ? "房主" : "Host");
    string clientRole = isHost
      ? (isZh ? "加入者" : "Peer")
      : (isZh ? "本人" : "You");

    string FormatValue(IReadOnlyDictionary<string, bool> dict, string id)
    {
      if (!dict.TryGetValue(id, out bool val))
      {
        return isZh ? "无此项" : "Missing";
      }
      return val ? (isZh ? "开启" : "Enabled") : (isZh ? "关闭" : "Disabled");
    }

    string FormatLine(string id)
    {
      var change = BalanceCatalog.All.FirstOrDefault(c => c.Id == id);
      string name;
      if (change != null)
      {
        string g = isZh ? change.Group : LocalizeGroup(change.Group);
        string s = isZh ? change.Subgroup : LocalizeSubgroup(change.Subgroup);
        string cat = string.IsNullOrEmpty(s) ? g : $"{g} / {s}";
        name = $"{change.Label} ({cat})";
      }
      else
      {
        name = id;
      }
      string hVal = FormatValue(hostValues, id);
      string cVal = FormatValue(clientValues, id);
      return isZh
        ? $"・[{id}] {name}：{hostRole}=[{hVal}] / {clientRole}=[{cVal}]"
        : $"・[{id}] {name}: {hostRole}=[{hVal}] / {clientRole}=[{cVal}]";
    }

    const int maxLines = 8;
    var lines = differenceIds.Take(maxLines).Select(FormatLine).ToList();
    if (differenceIds.Length > maxLines)
    {
      int remaining = differenceIds.Length - maxLines;
      string more = isZh
        ? $"...以及其他 {remaining} 项不一致"
        : $"...and {remaining} more mismatches";
      lines.Add(more);
    }

    return $"{header}\n" + string.Join("\n", lines);
  }

  private static string LocalizeGroup(string group) => group switch
  {
    "新增卡牌" => "New Cards",
    "新增普通遗物" => "New Relics",
    "回归事件" => "Returning Events",
    "原版卡牌调整" => "Vanilla Cards",
    "原版普通遗物调整" => "Vanilla Relics",
    "原版事件调整" => "Vanilla Events",
    "怪物调整" => "Monster Balance",
    "先古之民调整" => "Ancient Balance",
    "通用规则" => "General Rules",
    _ => group
  };

  private static string LocalizeSubgroup(string subgroup) => subgroup switch
  {
    "铁甲战士" => "Ironclad",
    "静默猎手" => "Silent",
    "故障机器人" => "Defect",
    "死灵缚者" => "Necrobinder",
    "储君" => "Regent",
    "涅奥" => "Neow",
    "达尔夫" => "Darf",
    "探克斯" => "Tanx",
    "瓦库" => "Vaku",
    "奥罗波斯" => "Ouroboros",
    "提兹卡塔拉" => "Tizcatala",
    "诺努佩佩" => "Nonupepe",
    "其他" => "Other",
    _ => subgroup
  };
}

/// <summary>
/// Clear any stale mismatch info when initiating a new handshake.
/// </summary>
[HarmonyPatch(typeof(HandshakeManager), nameof(HandshakeManager.BeginHandshakeFor), [typeof(ulong)])]
internal static class BalanceBeginHandshakePatch
{
  private static void Prefix()
  {
    BalanceReadHandshakePatch.LastMismatch = null;
  }
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

  private static void Postfix(PacketWriter writer)
  {
    try
    {
      BalanceReadHandshakePatch.LastMismatch = null;
      byte[] payload = BalanceConfigWire.Encode(Snapshot());
      writer.WriteUInt(BalanceConfigWire.Magic);
      writer.WriteUShort((ushort)payload.Length);
      writer.WriteBytes(payload, payload.Length);
    }
    catch (Exception ex)
    {
      Log.Error($"[Sts2BalanceMod] Failed to append balance handshake: {ex}");
    }
  }
}

/// <summary>
/// Only augment a successful vanilla handshake. Other error categories retain vanilla behavior.
/// ModMismatch is the existing transport carrier; the extra-info subtype identifies our own error.
/// </summary>
[HarmonyPatch(typeof(HandshakeManager), "TryReadHandshakeMessage")]
internal static class BalanceReadHandshakePatch
{
  internal static BalanceConfigMismatchInfo? LastMismatch;

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
    catch (Exception ex)
    {
      Log.Error($"[Sts2BalanceMod] Failed to read balance configuration handshake: {ex}");
      bool isZh = LocManager.Instance?.Language is "zhs" or "zht";
      string title = isZh ? "Mod 版本不兼容" : "Mod Version Incompatible";
      string details = isZh
        ? "对方的平衡调整 Mod (Sts2BalanceMod) 通信协议不兼容。\n请双方均更新至最新版本 Mod 并重启游戏后重试。"
        : "Incompatible STS2 Balance Mod communication protocol.\nPlease both update to the latest mod version and restart the game.";
      LastMismatch = new BalanceConfigMismatchInfo
      {
        localInfo = __result.extraInfo?.localInfo ?? PeerVersionInfo.LocalDefault(),
        remoteInfo = __result.extraInfo?.remoteInfo ?? PeerVersionInfo.LocalDefault(),
        localIsHost = __result.extraInfo?.localIsHost ?? false,
        DifferenceIds = [],
        HostValues = new Dictionary<string, bool>(),
        ClientValues = new Dictionary<string, bool>(),
        CustomTitle = title,
        CustomDetails = details
      };
      __instance.AbortHandshake(senderId);
      __result = new HandshakeResult(HandshakeStatus.InvalidHandshake);
      ____handler.HandshakeFailed(senderId, new NetErrorInfo(NetError.InvalidHandshake, selfInitiated: false));
      return;
    }

    var local = BalanceWriteHandshakePatch.Snapshot();
    string[] differences = BalanceConfigWire.Differences(local, remote);
    if (differences.Length == 0) return;

    var original = __result.extraInfo;
    bool isHost = original?.localIsHost ?? false;
    var host = isHost ? local : remote;
    var client = isHost ? remote : local;

    var mismatch = new BalanceConfigMismatchInfo
    {
      localInfo = original?.localInfo ?? PeerVersionInfo.LocalDefault(),
      remoteInfo = original?.remoteInfo ?? PeerVersionInfo.LocalDefault(),
      localIsHost = isHost,
      DifferenceIds = differences,
      HostValues = host,
      ClientValues = client
    };

    LastMismatch = mismatch;
    Log.Warn($"[Sts2BalanceMod] Balance configuration mismatch detected with peer {senderId}:\n{mismatch.GetDetails(LocManager.Instance?.Language)}");

    __result.extraInfo = mismatch;
    __result.status = HandshakeStatus.ModMismatch;
  }
}

/// <summary>
/// Intercept network error popups to display detailed balance configuration differences.
/// Falls back to LastMismatch if Steam transport disconnection cleared ConnectionExtraInfo.
/// </summary>
[HarmonyPatch(typeof(NErrorPopup), nameof(NErrorPopup.Create), [typeof(NetErrorInfo)])]
internal static class BalanceConfigErrorPopupPatch
{
  private static bool Prefix(NetErrorInfo info, ref NErrorPopup? __result)
  {
    if (info.SelfInitiated && (info.GetReason() == NetError.Quit || info.GetReason() == NetError.CancelledJoin))
    {
      BalanceReadHandshakePatch.LastMismatch = null;
      return true;
    }
    var mismatch = info.ConnectionExtraInfo as BalanceConfigMismatchInfo ?? BalanceReadHandshakePatch.LastMismatch;
    if (mismatch == null) return true;
    BalanceReadHandshakePatch.LastMismatch = null;

    string lang = LocManager.Instance?.Language ?? "eng";
    __result = NErrorPopup.Create(mismatch.GetTitle(lang), mismatch.GetDetails(lang), false);
    return false;
  }
}

/// <summary>
/// Ensure GetErrorString returns the detailed parameter differences when available.
/// </summary>
[HarmonyPatch(typeof(NetErrorInfo), nameof(NetErrorInfo.GetErrorString))]
internal static class BalanceConfigErrorTextPatch
{
  private static void Postfix(NetErrorInfo __instance, ref string __result)
  {
    var mismatch = __instance.ConnectionExtraInfo as BalanceConfigMismatchInfo ?? BalanceReadHandshakePatch.LastMismatch;
    if (mismatch != null)
    {
      string lang = LocManager.Instance?.Language ?? "eng";
      __result = mismatch.GetDetails(lang);
    }
  }
}
