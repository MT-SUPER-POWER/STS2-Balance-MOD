using MegaCrit.Sts2.Core.Entities.Encounters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Encounters;

// ======================== 红面具三人帮遭遇 ========================

/// <summary>
/// STS1-EVENT — 红面具强盗遭遇战。
/// 包含 Pointy（尖头）、Romeo（罗密欧）、Bear（熊）三个怪物。
/// 仅通过 MaskedBandits 事件触发，不会在普通战斗中出现。
/// </summary>
public sealed class RedMaskBandits : Sts2EncounterModel
{
  public override RoomType RoomType => RoomType.Monster;

  public override bool IsValidForAct(ActModel act) => false;

  public override bool IsWeak => false;

  public override bool HasScene => true;

  public override IReadOnlyList<string> Slots => ["pointy", "romeo", "bear"];

  public override IEnumerable<MonsterModel> AllPossibleMonsters =>
  [
    ModelDb.Monster<Pointy>(),
    ModelDb.Monster<Romeo>(),
    ModelDb.Monster<Bear>(),
  ];

  protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
  [
    (ModelDb.Monster<Pointy>().ToMutable(), "pointy"),
    (ModelDb.Monster<Romeo>().ToMutable(), "romeo"),
    (ModelDb.Monster<Bear>().ToMutable(), "bear"),
  ];
}
