using BaseLib.Config;

namespace Sts2BalanceMod.Sts2BalanceModCode.Config;

/// <summary>
/// 玩家可配置的平衡 Mod 功能。
/// BaseLib 负责生成设置 UI，并持久化这些静态属性。
/// </summary>
public sealed class BalanceModConfig : SimpleModConfig
{
  [ConfigHoverTip]
  public static bool EnableEventLeaveOptions { get; set; } = true;

  [ConfigHoverTip]
  public static bool EnableInfestedPrismRework { get; set; } = true;
}
