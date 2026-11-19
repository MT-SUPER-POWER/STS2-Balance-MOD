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
    return lang switch
    {
      "jpn" => "MOD設定の不一致",
      "zhs" or "zht" => "Mod 参数配置不一致",
      _ => "Mod Configuration Mismatch"
    };
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
    string header = lang switch
    {
      "jpn" => "STS2 バランス調整 MOD (Sts2BalanceMod) の設定がホストと一致しないため、部屋に参加できません。\n設定を一致させてゲームを再起動してから再試行してください。\n\n不一致の項目:",
      "zhs" or "zht" => "平衡调整 Mod (Sts2BalanceMod) 的生效配置与房主不一致，无法加入房间。\n请将配置调整为一致并重启游戏后重试。\n\n不一致的选项：",
      _ => "STS2 Balance Mod (Sts2BalanceMod) configuration does not match the host.\nPlease match the settings and restart the game before retrying.\n\nMismatched options:"
    };

    string hostRole;
    string clientRole;
    if (isHost)
    {
      hostRole = lang switch
      {
        "jpn" => "あなた(ホスト)",
        "zhs" or "zht" => "本人(房主)",
        _ => "You (Host)"
      };
      clientRole = lang switch
      {
        "jpn" => "参加者",
        "zhs" or "zht" => "加入者",
        _ => "Peer"
      };
    }
    else
    {
      hostRole = lang switch
      {
        "jpn" => "ホスト",
        "zhs" or "zht" => "房主",
        _ => "Host"
      };
      clientRole = lang switch
      {
        "jpn" => "あなた",
        "zhs" or "zht" => "本人",
        _ => "You"
      };
    }

    string FormatValue(IReadOnlyDictionary<string, bool> dict, string id)
    {
      if (!dict.TryGetValue(id, out bool val))
      {
        return lang switch
        {
          "jpn" => "未設定",
          "zhs" or "zht" => "无此项",
          _ => "Missing"
        };
      }
      if (val)
      {
        return lang switch
        {
          "jpn" => "有効",
          "zhs" or "zht" => "开启",
          _ => "Enabled"
        };
      }
      return lang switch
      {
        "jpn" => "無効",
        "zhs" or "zht" => "关闭",
        _ => "Disabled"
      };
    }

    string FormatLine(string id)
    {
      var change = BalanceCatalog.All.FirstOrDefault(c => c.Id == id);
      string name;
      if (change != null)
      {
        string g = LocalizeGroup(change.Group, lang);
        string s = LocalizeSubgroup(change.Subgroup, lang);
        string cat = string.IsNullOrEmpty(s) ? g : $"{g} / {s}";
        name = $"{change.Label} ({cat})";
      }
      else
      {
        name = id;
      }
      string hVal = FormatValue(hostValues, id);
      string cVal = FormatValue(clientValues, id);
      if (lang is "zhs" or "zht")
      {
        return $"・[{id}] {name}：{hostRole}=[{hVal}] / {clientRole}=[{cVal}]";
      }
      return $"・[{id}] {name}: {hostRole}=[{hVal}] / {clientRole}=[{cVal}]";
    }

    const int maxLines = 8;
    var lines = differenceIds.Take(maxLines).Select(FormatLine).ToList();
    if (differenceIds.Length > maxLines)
    {
      int remaining = differenceIds.Length - maxLines;
      string more = lang switch
      {
        "jpn" => $"...他 {remaining} 件の不一致",
        "zhs" or "zht" => $"...以及其他 {remaining} 项不一致",
        _ => $"...and {remaining} more mismatches"
      };
      lines.Add(more);
    }

    return $"{header}\n" + string.Join("\n", lines);
  }

  private static string LocalizeGroup(string group, string lang)
  {
    if (lang == "jpn")
    {
      return group switch
      {
        "新增卡牌" => "新規カード",
        "新增普通遗物" => "新規レリック",
        "回归事件" => "復刻イベント",
        "原版卡牌调整" => "カード調整",
        "原版普通遗物调整" => "レリック調整",
        "原版事件调整" => "イベント調整",
        "怪物调整" => "モンスター調整",
        "先古之民调整" => "エンシェント調整",
        "通用规则" => "共通ルール",
        _ => group
      };
    }
    if (lang is not ("zhs" or "zht"))
    {
      return group switch
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
    }
    return group;
  }

  private static string LocalizeSubgroup(string subgroup, string lang)
  {
    if (string.IsNullOrEmpty(subgroup)) return string.Empty;
    if (lang == "jpn")
    {
      return subgroup switch
      {
        "铁甲战士" => "アイアンクラッド",
        "静默猎手" => "サイレント",
        "故障机器人" => "ディフェクト",
        "死灵缚者" => "ネクロバインダー",
        "储君" => "リージェント",
        "涅奥" => "ネオー",
        "达尔夫" => "ダルフ",
        "探克斯" => "タンクス",
        "瓦库" => "ヴァク",
        "奥罗波斯" => "ウロボロス",
        "提兹卡塔拉" => "ティズカタラ",
        "诺努佩佩" => "ノヌペペ",
        "其他" => "その他",
        _ => subgroup
      };
    }
    if (lang is not ("zhs" or "zht"))
    {
      return subgroup switch
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
    return subgroup;
  }
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
      string lang = LocManager.Instance?.Language ?? "eng";
      string title = lang switch
      {
        "jpn" => "MODバージョンの不一致",
        "zhs" or "zht" => "Mod 版本不兼容",
        _ => "Mod Version Incompatible"
      };
      string details = lang switch
      {
        "jpn" => "相手の STS2 バランス調整 MOD (Sts2BalanceMod) と通信形式が一致しません。\nお互いに Mod を最新バージョンに更新してゲームを再起動してください。",
        "zhs" or "zht" => "对方的平衡调整 Mod (Sts2BalanceMod) 通信协议不兼容。\n请双方均更新至最新版本 Mod 并重启游戏后重试。",
        _ => "Incompatible STS2 Balance Mod communication protocol.\nPlease both update to the latest mod version and restart the game."
      };
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
