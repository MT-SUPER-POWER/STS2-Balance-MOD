using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.src.Cards;

namespace Sts2BalanceMod.src.Relics;

/// <summary>
/// STS1-RELIC-01 — 死灵之书：拾取时获得死灵诅咒，每回合首次打出 2 费以上攻击牌时额外打出一次。
/// 来源参考 ActsFromThePast.Relics.Necronomicon。
/// </summary>
[RegisterRelic(typeof(EventRelicPool), FullPublicEntry = "STS2_BALANCEMOD_NECRONOMICON")]
public sealed class Necronomicon : BalanceRelicTemplate
{
  private bool _activated = true;

  public override RelicRarity Rarity => RelicRarity.Event;

  protected override IEnumerable<IHoverTip> AdditionalHoverTips => HoverTipFactory.FromCardWithCardHoverTips<Necronomicurse>();

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new EnergyVar(2),
  ];

  public override async Task AfterObtained()
  {
    var curse = Owner.RunState.CreateCard(ModelDb.Card<Necronomicurse>(), Owner);
    var result = await CardPileCmd.Add(curse, PileType.Deck);
    CardCmd.PreviewCardPileAdd(result, 2f);
  }

  public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
  {
    if (player != Owner)
      return Task.CompletedTask;

    _activated = true;
    Status = RelicStatus.Normal;
    return Task.CompletedTask;
  }

  public override int ModifyCardPlayCount(CardModel card, Creature? target, int playCount)
  {
    if (card.Owner != Owner)
      return playCount;

    if (!_activated)
      return playCount;

    if (card.Type != CardType.Attack)
      return playCount;

    if (card.EnergyCost.GetResolved() < DynamicVars.Energy.IntValue)
      return playCount;

    return playCount + 1;
  }

  public override Task AfterModifyingCardPlayCount(CardModel card)
  {
    if (card.Owner != Owner)
      return Task.CompletedTask;

    if (card.Type != CardType.Attack)
      return Task.CompletedTask;

    if (card.EnergyCost.GetResolved() < DynamicVars.Energy.IntValue)
      return Task.CompletedTask;

    if (!_activated)
      return Task.CompletedTask;

    _activated = false;
    Flash();
    Status = RelicStatus.Disabled;

    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(CombatRoom room)
  {
    _activated = true;
    Status = RelicStatus.Normal;
    return Task.CompletedTask;
  }
}
