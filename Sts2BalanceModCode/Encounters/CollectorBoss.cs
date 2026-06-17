// NOTE: 暂注释 — 后续版本开放 Boss 战斗
#if false
using Godot;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Encounters;

/// <summary>
/// STS1-BOSS-02 — 收藏家 Boss 遭遇。
/// 输入：作为 Boss 房遭遇被 Act 抽取。
/// 输出：生成收藏家与预留召唤物槽位。
/// </summary>
public sealed class CollectorBoss : Sts2EncounterModel
{
  public override RoomType RoomType => RoomType.Boss;

  public override MegaSkeletonDataResource? BossNodeSpineResource => null;

  public override string BossNodePath => "res://Sts2BalanceMod/map_boss_icons/collector";

  public override string CustomBgm => "event:/music/act2_boss_kaiser_crab";

  public override IReadOnlyList<string> Slots => ["collector", "left", "right"];

  public override IEnumerable<MonsterModel> AllPossibleMonsters =>
  [
    ModelDb.Monster<Collector>(),
    ModelDb.Monster<CollectorTorchHead>(),
  ];

  protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
  [
    (ModelDb.Monster<Collector>().ToMutable(), "collector"),
  ];
}
#endif
