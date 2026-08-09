using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using Sts2BalanceMod.src.Abstract;
using Sts2BalanceMod.src.Extensions;
using Sts2BalanceMod.src.Monsters;

namespace Sts2BalanceMod.src.Encounters;

/// <summary>
/// STS1-BOSS-01 — 时间吞噬者 Boss 遭遇。
/// 输入：作为 Boss 房遭遇被 Act 抽取。
/// 输出：生成一个时间吞噬者怪物并使用一代回归资源。
/// BGM 由 ModBgmPatch 负责在战斗开始/结束时管理（FadeIn/FadeOut）。
/// </summary>
[RegisterGlobalEncounter]
public sealed class TimeEaterBoss : BalanceEncounterTemplate
{
  public override RoomType RoomType => RoomType.Boss;

  public override MegaSkeletonDataResource? BossNodeSpineResource => null;

  public override EncounterAssetProfile AssetProfile => new(
    BossNodeSpinePath: ModAssetPaths.Resource("map_boss_icons", "time_eater"),
    ExtraAssetPaths:
    [
      ModAssetPaths.PowerIcon("actsfromthepast-time_warp_power.png"),
      ModAssetPaths.PowerIcon("actsfromthepast-draw_reduction_power.png"),
    ]);

  public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<TimeEater>()];

  protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
  [
    (ModelDb.Monster<TimeEater>().ToMutable(), null),
  ];
}
