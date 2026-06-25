using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Encounters;

/// <summary>
/// STS1-BOSS-01 — 时间吞噬者 Boss 遭遇。
/// 输入：作为 Boss 房遭遇被 Act 抽取。
/// 输出：生成一个时间吞噬者怪物并使用一代回归资源。
/// BGM 由 ModBgmPatch 负责在战斗开始/结束时管理（FadeIn/FadeOut）。
/// </summary>
public sealed class TimeEaterBoss : Sts2EncounterModel
{
  private const string TimeWarpPowerIcon = "res://Sts2BalanceMod/images/powers/actsfromthepast-time_warp_power.png";
  private const string DrawReductionPowerIcon = "res://Sts2BalanceMod/images/powers/actsfromthepast-draw_reduction_power.png";

  public override RoomType RoomType => RoomType.Boss;

  public override MegaSkeletonDataResource? BossNodeSpineResource => null;

  public override string BossNodePath => "res://Sts2BalanceMod/map_boss_icons/time_eater";

  public override IEnumerable<string> ExtraAssetPaths =>
  [
    TimeWarpPowerIcon,
    DrawReductionPowerIcon,
  ];

  public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<TimeEater>()];

  protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
  [
    (ModelDb.Monster<TimeEater>().ToMutable(), null),
  ];
}
