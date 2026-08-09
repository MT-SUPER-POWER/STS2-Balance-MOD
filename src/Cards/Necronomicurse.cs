using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using Sts2BalanceMod.src.Abstract;
using Sts2BalanceMod.src.Relics;

namespace Sts2BalanceMod.src.Cards;

/// <summary>
/// STS1-CARD-01 — 死灵诅咒：不可打出、永恒，消耗后回到手牌。
/// 来源参考 ActsFromThePast.Cards.Necronomicurse。
/// </summary>
[RegisterCard(typeof(CurseCardPool), FullPublicEntry = "STS2_BALANCEMOD_NECRONOMICURSE")]
public sealed class Necronomicurse : BalanceCardTemplate
{
  public Necronomicurse() : base(-1, CardType.Curse, CardRarity.Curse, TargetType.None)
  {
  }

  public override bool CanBeGeneratedByModifiers => false;

  public override int MaxUpgradeLevel => 0;

  public override IEnumerable<CardKeyword> CanonicalKeywords =>
  [
    CardKeyword.Unplayable,
    CardKeyword.Eternal,
  ];

  protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    return Task.CompletedTask;
  }

  public override async Task AfterCardExhausted(
    PlayerChoiceContext choiceContext,
    CardModel card,
    bool causedByEthereal)
  {
    if (card != this)
      return;

    Owner.Relics.FirstOrDefault(r => r is Necronomicon)?.Flash();
    await CardPileCmd.Add(this, PileType.Hand);
  }
}
