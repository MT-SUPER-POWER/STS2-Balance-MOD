using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

[RegisterPower]
public sealed class EvolvePower() : BalancePowerTemplate(PowerType.Buff, PowerStackType.Counter)
{

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
