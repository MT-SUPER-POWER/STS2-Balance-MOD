namespace Sts2BalanceMod.Sts2BalanceModCode.Settings;

internal enum BalanceConfigReadError
{
  None,
  MissingHeader,
  UnrecognizedHeader,
  TruncatedLength,
  InvalidLength,
  TruncatedPayload,
  InvalidEntries
}

internal static class BalanceConfigDiagnostics
{
  internal static bool IsChinese(string? language) => language is "zhs" or "zht";

  internal static string MismatchTitle(string? language) =>
    IsChinese(language) ? "平衡调整 Mod 的设置不一致" : "STS2 Balance Mod settings differ";

  internal static string MismatchSummary(string? language, int count) => IsChinese(language)
    ? $"双方有 {count} 项生效设置不同。请选择「详情」查看项目和双方的开关状态。调整为一致后，请重启游戏再试。"
    : $"{count} active settings differ. Open Details to see each setting and both values. Match the settings and restart the game before retrying.";

  internal static string[] MismatchLines(
    string? language, IEnumerable<string> ids,
    IReadOnlyDictionary<string, bool> host, IReadOnlyDictionary<string, bool> client, bool localIsHost)
  {
    bool isChinese = IsChinese(language);
    string hostRole = isChinese ? (localIsHost ? "你（房主）" : "房主") : (localIsHost ? "You (Host)" : "Host");
    string clientRole = isChinese ? (localIsHost ? "加入者" : "你") : (localIsHost ? "Joining player" : "You");

    string Value(IReadOnlyDictionary<string, bool> values, string id)
    {
      if (!values.TryGetValue(id, out bool enabled))
        return isChinese ? "无此设置" : "Setting unavailable";
      return enabled ? (isChinese ? "开启" : "Enabled") : (isChinese ? "关闭" : "Disabled");
    }

    return ids.Select(id =>
    {
      BalanceChange? change = BalanceCatalog.All.FirstOrDefault(c => c.Id == id);
      string name = change == null
        ? id
        : $"{id} {change.LocalizedLabel} ({Category(change, isChinese)})";
      return $"• {name}\n  {hostRole} = {Value(host, id)} / {clientRole} = {Value(client, id)}";
    }).ToArray();
  }

  internal static (string Title, string Body) ReadFailure(string? language, BalanceConfigReadError error, bool localIsHost)
  {
    if (IsChinese(language))
    {
      string peer = localIsHost ? "加入者" : "房主";
      string reason = error switch
      {
        BalanceConfigReadError.MissingHeader => $"未收到{peer}的平衡调整 Mod 配置。",
        BalanceConfigReadError.UnrecognizedHeader => $"{peer}发来的平衡调整 Mod 配置头无法识别。",
        BalanceConfigReadError.TruncatedLength => $"{peer}发来的配置长度字段不完整。",
        BalanceConfigReadError.InvalidLength => $"{peer}发来的配置长度无效。",
        BalanceConfigReadError.TruncatedPayload => $"{peer}发来的配置数据不完整。",
        _ => $"{peer}发来的配置包含无法识别或重复的条目。"
      };
      return ("无法核对平衡调整 Mod 设置", $"{reason}\n\n请确认双方使用同一构建的平衡调整 Mod，重启游戏后再试。");
    }

    string remote = localIsHost ? "The joining player" : "The host";
    string detail = error switch
    {
      BalanceConfigReadError.MissingHeader => $"{remote} did not send readable STS2 Balance Mod settings.",
      BalanceConfigReadError.UnrecognizedHeader => $"{remote} sent an unrecognized STS2 Balance Mod configuration header.",
      BalanceConfigReadError.TruncatedLength => $"{remote} sent an incomplete configuration length field.",
      BalanceConfigReadError.InvalidLength => $"{remote} sent an invalid configuration length.",
      BalanceConfigReadError.TruncatedPayload => $"{remote} sent incomplete configuration data.",
      _ => $"{remote} sent invalid or duplicate configuration entries."
    };
    return ("Could not verify STS2 Balance Mod settings",
      $"{detail}\n\nMake sure both players use the same build of STS2 Balance Mod and restart the game before retrying.");
  }

  private static string Category(BalanceChange change, bool isChinese)
  {
    string fallbackGroup = isChinese ? change.Group : change.Group switch
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
      _ => change.Group
    };
    string fallbackSubgroup = isChinese ? change.Subgroup : change.Subgroup switch
    {
      "铁甲战士" => "Ironclad",
      "静默猎手" => "Silent",
      "故障机器人" => "Defect",
      "死灵缚者" => "Necrobinder",
      "储君" => "Regent",
      "涅奥" => "Neow",
      "达尔夫" => "Darv",
      "探克斯" => "Tanx",
      "瓦库" => "Vakuu",
      "奥罗波斯" => "Orobas",
      "提兹卡塔拉" => "Tezcatara",
      "诺努佩佩" => "Nonupeipe",
      "其他" => "Other",
      _ => change.Subgroup
    };
    string group = BalanceCatalog.Loc(BalanceCatalog.GroupKey(change.Group), fallbackGroup);
    string subgroup = string.IsNullOrEmpty(change.Subgroup)
      ? string.Empty
      : BalanceCatalog.Loc(BalanceCatalog.SubgroupKey(change.Subgroup), fallbackSubgroup);
    return subgroup.Length == 0 ? group : $"{group} / {subgroup}";
  }
}
