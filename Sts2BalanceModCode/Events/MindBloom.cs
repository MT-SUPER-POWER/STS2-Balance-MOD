using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events;

/// <summary>
/// STS1-EVENT-06 — 心灵绽放：本批先移植非战斗分支。
/// 来源参考 ActsFromThePast.Acts.TheBeyond.Events.MindBloom。
/// </summary>
public sealed class MindBloom : CustomEventModel
{
  private const int GoldRewardAmount = 999;
  private bool _isBeforeTreasure;

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
      new(this, null,
        $"{Id.Entry}.pages.INITIAL.options.FIGHT_LOCKED",
        Array.Empty<IHoverTip>()),
      Option(Upgrade, "INITIAL", HoverTipFactory.FromRelic(ModelDb.Relic<MarkOfTheBloom>()).ToArray()),
    };

    options.Add(_isBeforeTreasure
      ? Option(Gold, "INITIAL", HoverTipFactory.FromCardWithCardHoverTips<Normality>().ToArray())
      : Option(Heal, "INITIAL", HoverTipFactory.FromCardWithCardHoverTips<Doubt>().ToArray()));

    return options;
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
}
