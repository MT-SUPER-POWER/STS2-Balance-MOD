using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

public sealed class EvolvePower() : Sts2PowerModel(PowerType.Buff, PowerStackType.Counter)
{
    public override string CustomPackedIconPath => "res://Sts2BalanceMod/images/powers/evolve_power.png";
    public override string CustomBigIconPath => "res://Sts2BalanceMod/images/powers/big/evolve_power.png";

    public override async Task AfterCardDrawn(PlayerChoiceContext choiceContext, CardModel drawnCard, bool fromHandDraw)
    {
        if (drawnCard == null || Owner?.Player == null)
            return;

        if (drawnCard.Type == CardType.Status)
        {
            Flash();
            await CardPileCmd.Draw(choiceContext, (int)Amount, Owner.Player);
        }
    }
}
