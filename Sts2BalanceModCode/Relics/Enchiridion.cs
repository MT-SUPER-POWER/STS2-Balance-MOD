using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

/// <summary>
/// STS1-RELIC-01 — 英雄宝典：每场战斗第一回合将一张随机能力牌加入手牌，本回合可免费打出。
/// 来源参考 ActsFromThePast.Relics.Enchiridion。
/// </summary>
[Pool(typeof(EventRelicPool))]
public sealed class Enchiridion : Sts2RelicModel
{
  public override RelicRarity Rarity => RelicRarity.Event;

  public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
  {
    if (player != Owner || Owner.Creature?.CombatState?.RoundNumber != 1)
      return;

    Flash();

    var powerCards = Owner.Character.CardPool
      .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
      .Where(c => c.Type == CardType.Power)
      .ToList();

    var card = CardFactory.GetDistinctForCombat(
      Owner,
      powerCards,
      1,
      Owner.RunState.Rng.CombatCardGeneration).First();

    card.SetToFreeThisTurn();
    await CardPileCmd.AddGeneratedCardToCombat(card, PileType.Hand, Owner);
  }
}
