using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;

using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events;

/// <summary>
/// STS1-EVENT-01 — 诅咒书本：连续阅读并承受伤害后，获得一本事件书。
/// 来源参考 ActsFromThePast.Acts.TheCity.Events.CursedTome。
/// </summary>
[RegisterActEvent(typeof(Hive))]
public sealed class CursedTome : BalanceEventTemplate
{
  private const int DmgPage1 = 1;
  private const int DmgPage2 = 2;
  private const int DmgPage3 = 3;
  private const int DmgStop = 3;
  private const int DmgObtain = 15;

  /// <summary>仅限 Act 2（Hive）出现，太早遇到死灵书过于强大。</summary>

  /// <summary>仅在 Act 2 出现。</summary>
  public override bool IsAllowed(IRunState runState)
  {
    return runState.CurrentActIndex == 1;
  }

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new IntVar("DmgPage1", DmgPage1),
    new IntVar("DmgPage2", DmgPage2),
    new IntVar("DmgPage3", DmgPage3),
    new IntVar("DmgStop", DmgStop),
    new IntVar("DmgObtain", DmgObtain),
  ];

  protected override IReadOnlyList<EventOption> GenerateInitialOptions()
  {
    return
    [
      Option(Read),
      Option(Leave),
    ];
  }

  private Task Read()
  {
    SetEventState(PageDescription("PAGE_1"),
    [
      new EventOption(this, Page1Continue, $"{Id.Entry}.pages.PAGE_1.options.CONTINUE", []),
    ]);
    return Task.CompletedTask;
  }

  private async Task Page1Continue()
  {
    await DamageOwner(DmgPage1);
    SetEventState(PageDescription("PAGE_2"),
    [
      new EventOption(this, Page2Continue, $"{Id.Entry}.pages.PAGE_2.options.CONTINUE", []),
    ]);
  }

  private async Task Page2Continue()
  {
    await DamageOwner(DmgPage2);
    SetEventState(PageDescription("PAGE_3"),
    [
      new EventOption(this, Page3Continue, $"{Id.Entry}.pages.PAGE_3.options.CONTINUE", []),
    ]);
  }

  private async Task Page3Continue()
  {
    await DamageOwner(DmgPage3);
    SetEventState(PageDescription("LAST_PAGE"),
    [
      new EventOption(this, Obtain, $"{Id.Entry}.pages.LAST_PAGE.options.OBTAIN", []),
      new EventOption(this, Stop, $"{Id.Entry}.pages.LAST_PAGE.options.STOP", []),
    ]);
  }

  private async Task Obtain()
  {
    var owner = Owner;
    if (owner == null)
      return;

    await DamageOwner(DmgObtain);
    await RewardsCmd.OfferCustom(owner,
    [
      new RelicReward(GetRandomBook().ToMutable(), owner),
    ]);
    SetEventFinished(PageDescription("OBTAIN"));
  }

  private async Task Stop()
  {
    await DamageOwner(DmgStop);
    SetEventFinished(PageDescription("STOP"));
  }

  private Task Leave()
  {
    SetEventFinished(PageDescription("LEAVE"));
    return Task.CompletedTask;
  }

  private async Task DamageOwner(int amount)
  {
    var owner = Owner;
    if (owner?.Creature == null)
      return;

    await CreatureCmd.Damage(
      new ThrowingPlayerChoiceContext(),
      owner.Creature,
      amount,
      ValueProp.Unblockable | ValueProp.Unpowered,
      null,
      null);
  }

  private RelicModel GetRandomBook()
  {
    var owner = Owner;
    if (owner == null)
      return ModelDb.Relic<Necronomicon>();

    var possibleBooks = new List<RelicModel>();
    if (!owner.Relics.Any(r => r is Necronomicon))
      possibleBooks.Add(ModelDb.Relic<Necronomicon>());
    if (!owner.Relics.Any(r => r is Enchiridion))
      possibleBooks.Add(ModelDb.Relic<Enchiridion>());
    if (!owner.Relics.Any(r => r is NilrysCodex))
      possibleBooks.Add(ModelDb.Relic<NilrysCodex>());

    return possibleBooks.Count == 0
      ? ModelDb.Relic<Necronomicon>()
      : Rng.NextItem(possibleBooks)!;
  }
}
