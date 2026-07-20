using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

[Pool(typeof(IroncladCardPool))]
public sealed class Evolve : Sts2CardModel
{
    public Evolve() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self)
    {
        WithPower<EvolvePower>(1, 1);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<EvolvePower>(choiceContext, Owner.Creature, IsUpgraded ? 2 : 1, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
    }
}
