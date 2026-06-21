using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Encounters;

/// <summary>
/// 心灵绽放专用的一层 Boss 战斗：史莱姆老大。
/// 输入：仅由心灵绽放事件显式创建，不进入随机遭遇池。
/// 输出：使用一代事件场景槽位生成史莱姆老大战斗。
/// </summary>
public sealed class MindBloomSlimeBoss : Sts2EncounterModel
{
  public override RoomType RoomType => RoomType.Monster;

  public override bool HasScene => true;

  public override string CustomScenePath =>
    "res://Sts2BalanceMod/scenes/actsfromthepast-mind_bloom_slime_boss.tscn";

  public override IEnumerable<string> ExtraAssetPaths => [CustomScenePath];

  public override IReadOnlyList<string> Slots =>
  [
    "spike_med_1", "spike_large", "spike_med_2",
    "acid_med_1", "slime_boss", "acid_large", "acid_med_2",
  ];

  public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<SlimeBoss>()];

  protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
  [
    (ModelDb.Monster<SlimeBoss>().ToMutable(), "slime_boss"),
  ];
}
