using Godot;
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
/// </summary>
public sealed class TimeEaterBoss : Sts2EncounterModel
{
  public override RoomType RoomType => RoomType.Boss;

  public override MegaSkeletonDataResource? BossNodeSpineResource => null;

  public override string BossNodePath => "res://Assets/ActsFromThePast/ActsFromThePast/map_boss_icons/time_eater";

  public override string CustomBgm => "event:/music/act3_boss_queen";

  public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<TimeEater>()];

  protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
  [
    (ModelDb.Monster<TimeEater>().ToMutable(), null),
  ];
}
