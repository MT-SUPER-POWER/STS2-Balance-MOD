using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Encounters;

/// <summary>
/// 冒险者尸体遭遇战：三哨兵 (Sentries)。
/// </summary>
[RegisterGlobalEncounter]
public sealed class DeadAdventurerSentriesEncounter : BalanceEncounterTemplate
{
  public override RoomType RoomType => RoomType.Monster;
  public override bool IsWeak => false;

  public override IEnumerable<MonsterModel> AllPossibleMonsters =>
  [
    ModelDb.Monster<Sentry>()
  ];

  protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
  {
    var sentry0 = (Sentry)ModelDb.Monster<Sentry>().ToMutable();
    var sentry1 = (Sentry)ModelDb.Monster<Sentry>().ToMutable();
    var sentry2 = (Sentry)ModelDb.Monster<Sentry>().ToMutable();

    sentry0.BoltFirst = true;
    sentry1.BoltFirst = false;
    sentry2.BoltFirst = true;

    return
    [
      (sentry0, null),
      (sentry1, null),
      (sentry2, null)
    ];
  }
}
