using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Encounters;

/// <summary>
/// EVENT-COLOSSEUM-01 — 竞技场第二战遭遇：奴隶贩子头目 + 地精大块头。
/// 精英遭遇难度，战胜后斩获稀有遗物、罕见遗物、100 金币与卡牌奖励。
/// </summary>
[RegisterGlobalEncounter]
public sealed class ColosseumSecondEncounter : BalanceEncounterTemplate
{
  public override RoomType RoomType => RoomType.Elite;

  public override bool IsWeak => false;

  public override EncounterAssetProfile AssetProfile => new(
      EncounterScenePath: ModAssetPaths.Resource("scenes", "colosseum_second_encounter.tscn"));

  public override IReadOnlyList<string> Slots => ["taskmaster", "gremlin_nob"];

  public override IEnumerable<MonsterModel> AllPossibleMonsters =>
  [
    ModelDb.Monster<Taskmaster>(),
      ModelDb.Monster<GremlinNob>(),
    ];

  protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
  [
    (ModelDb.Monster<Taskmaster>().ToMutable(), "taskmaster"),
      (ModelDb.Monster<GremlinNob>().ToMutable(), "gremlin_nob"),
    ];
}
