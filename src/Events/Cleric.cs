using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

using Sts2BalanceMod.src.Abstract;

namespace Sts2BalanceMod.src.Events;

/// <summary>
/// STS1-EVENT-05 — 牧师：提供付费治疗或付费删牌。
/// 来源参考 ActsFromThePast.Acts.Exordium.Events.Cleric。
/// </summary>
[RegisterSharedEvent]
public sealed class Cleric : BalanceEventTemplate
{

  private const int HealCost = 35;
  private const int PurifyCost = 75;
  private const decimal HealPercent = 0.25M;

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new HealVar(0M),
    new IntVar("HealCost", HealCost),
    new IntVar("PurifyCost", PurifyCost),
  ];

  public override void CalculateVars()
  {
    var owner = Owner;
    if (owner?.Creature == null)
      return;

    DynamicVars.Heal.BaseValue = Math.Floor(owner.Creature.MaxHp * HealPercent);
  }

  protected override IReadOnlyList<EventOption> GenerateInitialOptions()
  {
    var owner = Owner;
    if (owner == null)
      return [Option(Leave)];

    var options = new List<EventOption>();

    if (owner.Gold >= HealCost)
      options.Add(Option(Heal));
    else
      options.Add(new EventOption(this, null,
        $"{Id.Entry}.pages.INITIAL.options.HEAL_LOCKED",
        Array.Empty<IHoverTip>()));

    var canPurify = owner.Gold >= PurifyCost && owner.Deck.Cards.Any(c => c.IsRemovable);
    if (canPurify)
      options.Add(Option(Purify));
    else
      options.Add(new EventOption(this, null,
        $"{Id.Entry}.pages.INITIAL.options.PURIFY_LOCKED",
        Array.Empty<IHoverTip>()));

    options.Add(Option(Leave));
    return options;
  }

  private async Task Heal()
  {
    var owner = Owner;
    if (owner?.Creature == null)
    {
      SetEventFinished(PageDescription("LEAVE"));
      return;
    }

    await PlayerCmd.LoseGold(HealCost, owner, GoldLossType.Spent);
    await CreatureCmd.Heal(owner.Creature, DynamicVars.Heal.BaseValue);
    SetEventFinished(PageDescription("HEAL"));
  }

  private async Task Purify()
  {
    var owner = Owner;
    if (owner == null)
    {
      SetEventFinished(PageDescription("LEAVE"));
      return;
    }

    await PlayerCmd.LoseGold(PurifyCost, owner, GoldLossType.Spent);
    var prefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 1);
    var selectedCards = await CardSelectCmd.FromDeckForRemoval(owner, prefs);
    await CardPileCmd.RemoveFromDeck(selectedCards.ToList());
    SetEventFinished(PageDescription("PURIFY"));
  }

  private Task Leave()
  {
    SetEventFinished(PageDescription("LEAVE"));
    return Task.CompletedTask;
  }

  public override bool IsAllowed(IRunState runState)
  {
    return runState.Players.All<Player>(p => p.Gold >= HealCost);
  }
}
