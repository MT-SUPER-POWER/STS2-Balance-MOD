using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Rewards;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events;

/// <summary>
/// STS1-EVENT-06 — 心灵绽放：本批先移植非战斗分支。
/// 来源参考 ActsFromThePast.Acts.TheBeyond.Events.MindBloom。
/// </summary>
public sealed class MindBloom : CustomEventModel
{
  private const int FightGold = 50;
  private const int GoldRewardAmount = 999;
  private bool _isBeforeTreasure;
  internal static bool CombatActive { get; set; }

  public override ActModel[] Acts => [];

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new GoldVar(GoldRewardAmount),
  ];

  public override void CalculateVars()
  {
    var owner = Owner;
    if (owner == null)
      return;

    var threshold = owner.RunState.Players.Count > 1 ? 38 : 41;
    _isBeforeTreasure = owner.RunState.TotalFloor < threshold;
  }

  protected override IReadOnlyList<EventOption> GenerateInitialOptions()
  {
    var options = new List<EventOption>
    {
      Option(Fight),
      Option(Upgrade, "INITIAL", HoverTipFactory.FromRelic(ModelDb.Relic<MarkOfTheBloom>()).ToArray()),
    };

    options.Add(_isBeforeTreasure
      ? Option(Gold, "INITIAL", HoverTipFactory.FromCardWithCardHoverTips<Normality>().ToArray())
      : Option(Heal, "INITIAL", HoverTipFactory.FromCardWithCardHoverTips<Doubt>().ToArray()));

    return options;
  }

  private Task Fight()
  {
    var owner = Owner;
    if (owner == null)
      return Task.CompletedTask;

    CombatActive = true;
    var bosses = new List<EncounterModel>
    {
      ModelDb.Encounter<MindBloomGuardian>(),
      ModelDb.Encounter<MindBloomHexaghost>(),
      ModelDb.Encounter<MindBloomSlimeBoss>(),
    };
    var encounter = Rng.NextItem(bosses).ToMutable();
    var rareRelic = RelicFactory.PullNextRelicFromFront(owner, RelicRarity.Rare)?.ToMutable();
    if (rareRelic == null)
      return Task.CompletedTask;

    var rewards = new List<Reward>
    {
      new GoldReward(FightGold, owner),
      new RelicReward(rareRelic, owner),
    };
    EnterCombatWithoutExitingEvent(encounter, rewards, false);
    return Task.CompletedTask;
  }

  private async Task Upgrade()
  {
    var owner = Owner;
    if (owner == null)
      return;

    foreach (var card in PileType.Deck.GetPile(owner).Cards)
    {
      if (card.IsUpgradable)
        CardCmd.Upgrade(card);
    }

    await RelicCmd.Obtain(ModelDb.Relic<MarkOfTheBloom>().ToMutable(), owner);
    SetEventFinished(PageDescription("UPGRADE"));
  }

  private async Task Gold()
  {
    var owner = Owner;
    if (owner == null)
      return;

    await PlayerCmd.GainGold(GoldRewardAmount, owner);
    for (var i = 0; i < 2; i++)
    {
      var card = owner.RunState.CreateCard(ModelDb.Card<Normality>(), owner);
      var result = await CardPileCmd.Add(card, PileType.Deck);
      CardCmd.PreviewCardPileAdd([result], 2f);
    }

    await Cmd.Wait(0.75f);
    SetEventFinished(PageDescription("GOLD"));
  }

  private async Task Heal()
  {
    var owner = Owner;
    if (owner?.Creature == null)
      return;

    await CreatureCmd.Heal(owner.Creature, owner.Creature.MaxHp);
    var card = owner.RunState.CreateCard(ModelDb.Card<Doubt>(), owner);
    var result = await CardPileCmd.Add(card, PileType.Deck);
    CardCmd.PreviewCardPileAdd([result], 2f);
    await Cmd.Wait(0.75f);
    SetEventFinished(PageDescription("HEAL"));
  }

  protected override void OnEventFinished()
  {
    CombatActive = false;
  }
}
