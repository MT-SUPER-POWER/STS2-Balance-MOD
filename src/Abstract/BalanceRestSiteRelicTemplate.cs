using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
namespace Sts2BalanceMod.src.Abstract;

// ======================== REST SITE RELIC BASE ========================

/// <summary>
/// 能向火堆追加自定义选项的遗物基类。
/// 输入：游戏在生成火堆选项时传入玩家与当前选项集合。
/// 输出：当遗物归属于该玩家且满足条件时，向集合追加一个自定义 RestSiteOption。
/// 返回值：true 表示已追加选项，false 表示未改动选项集合。
/// </summary>
public abstract class BalanceRestSiteRelicTemplate : BalanceRelicTemplate
{
  public sealed override bool TryModifyRestSiteOptions(Player player, ICollection<RestSiteOption> options)
  {
    if (player != Owner)
    {
      return false;
    }

    if (!CanAddRestSiteOption(player, options))
    {
      return false;
    }

    options.Add(CreateRestSiteOption(player));
    return true;
  }

  /// <summary>
  /// 判断当前火堆是否应该展示该遗物提供的选项。
  /// 输入：持有者玩家与当前火堆选项集合。
  /// 输出：true 表示允许追加选项。
  /// </summary>
  protected virtual bool CanAddRestSiteOption(Player player, ICollection<RestSiteOption> options)
  {
    return true;
  }

  /// <summary>
  /// 创建该遗物提供的火堆选项实例。
  /// 输入：持有者玩家。
  /// 输出：用于显示和执行的 RestSiteOption。
  /// </summary>
  protected abstract RestSiteOption CreateRestSiteOption(Player player);
}
