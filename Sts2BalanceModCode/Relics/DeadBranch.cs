using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Factories;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

// ======================== RELIC-03: 枯木树枝 ========================

/// <summary>
/// RELIC-03 — 枯木树枝：每当你消耗一张牌增加一张随机手牌到你的手中
/// </summary>
[RegisterRelic(typeof(SharedRelicPool), FullPublicEntry = "STS2_BALANCEMOD_DEAD_BRANCH")]
public sealed class DeadBranch : BalanceRelicTemplate
{
  public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
  public override RelicRarity Rarity => RelicRarity.Rare;

  private bool HasPrismaticGem => Owner?.GetRelic<PrismaticGem>() != null;

  private IEnumerable<CardPoolModel> GetCardPools()
  {
    if (HasPrismaticGem)
    {
      return Owner.UnlockState.CharacterCardPools
          .Append(ModelDb.CardPool<ColorlessCardPool>())
          .Distinct();
    }

    return [Owner.Character.CardPool];
  }

  private CardModel GenerateRandomCard()
  {
    var pools = GetCardPools();
    var allCards = pools.SelectMany(p =>
        p.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint));

    return CardFactory.GetForCombat(Owner, allCards, 1, Owner.RunState.Rng.CombatCardGeneration).First();
  }

  public override async Task AfterCardExhausted(PlayerChoiceContext choiceContext, CardModel card, bool causedByEthereal)
  {
    if (card.Owner != Owner) return;
    var newGeneratedCard = GenerateRandomCard();

    // NOTE: 如何让卡片有短时效的保留效果
    if (causedByEthereal) newGeneratedCard.GiveSingleTurnRetain();

    Flash();    // NOTE: 让遗物闪烁一下

    await CardPileCmd.AddGeneratedCardToCombat(newGeneratedCard, PileType.Hand, Owner);
  }
}
