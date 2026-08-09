using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using Sts2BalanceMod.src.Abstract;
using Sts2BalanceMod.src.Powers;

namespace Sts2BalanceMod.src.Cards;

[RegisterCard(typeof(IroncladCardPool), FullPublicEntry = "STS2_BALANCEMOD_EVOLVE")]
public sealed class Evolve : BalanceCardTemplate
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<EvolvePower>(1M)];

    public Evolve() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<EvolvePower>(choiceContext, Owner.Creature, DynamicVars[nameof(EvolvePower)].BaseValue, Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars[nameof(EvolvePower)].UpgradeValueBy(1M);
    }
}
