using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

using Sts2BalanceMod.src.Abstract;

namespace Sts2BalanceMod.src.Events;

/// <summary>
/// STS1-EVENT-04 — 神圣泉水：移除牌组中的所有可移除诅咒。
/// 来源参考 ActsFromThePast.SharedEvents.TheDivineFountain。
/// </summary>
[RegisterSharedEvent]
public sealed class TheDivineFountain : BalanceEventTemplate
{

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new IntVar("MaxHpGain", 0),
  ];

  public override bool IsAllowed(IRunState runState)
  {
    return runState.Players.All(p =>
      PileType.Deck.GetPile(p).Cards.Any(c => c.Type == CardType.Curse && c.IsRemovable));
  }

  protected override IReadOnlyList<EventOption> GenerateInitialOptions()
  {
    return
    [
      Option(Drink),
      Option(Leave),
    ];
  }

  private async Task Drink()
  {
    var owner = Owner;
    if (owner == null)
    {
      SetEventFinished(PageDescription("LEAVE"));
      return;
    }

    var curses = owner.Deck.Cards
      .Where(c => c.Type == CardType.Curse && c.IsRemovable)
      .ToList();

    await CardPileCmd.RemoveFromDeck(curses);
    SetEventFinished(PageDescription("DRINK"));
  }

  private Task Leave()
  {
    SetEventFinished(PageDescription("LEAVE"));
    return Task.CompletedTask;
  }
}
