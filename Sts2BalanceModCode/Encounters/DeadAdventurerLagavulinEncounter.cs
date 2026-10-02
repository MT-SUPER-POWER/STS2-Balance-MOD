using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Encounters;

/// <summary>
/// 冒险者尸体遭遇战：乐嘉维林 (Lagavulin)。
/// </summary>
[RegisterGlobalEncounter]
public sealed class DeadAdventurerLagavulinEncounter : BalanceEncounterTemplate
{
  public override RoomType RoomType => RoomType.Monster;
  public override bool IsWeak => false;

  public override EncounterAssetProfile AssetProfile => new(
    EncounterScenePath: ModAssetPaths.Resource("scenes", "encounters", "dead_adventurer_single.tscn"));

  public override IReadOnlyList<string> Slots => ["single"];

  public override IEnumerable<MonsterModel> AllPossibleMonsters =>
  [
    ModelDb.Monster<Lagavulin>()
  ];

  protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
  {
    var lagavulin = (Lagavulin)ModelDb.Monster<Lagavulin>().ToMutable();
    lagavulin.StartsAwake = true;
    return [(lagavulin, "single")];
  }
}
