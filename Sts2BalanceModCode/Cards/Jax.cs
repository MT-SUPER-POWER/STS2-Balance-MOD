using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// STS1-CARD-01 — J.A.X.：失去生命并获得力量。
/// 来源参考 ActsFromThePast.Cards.Jax。
/// </summary>
[RegisterCard(typeof(EventCardPool), FullPublicEntry = "STS2_BALANCEMOD_JAX")]
public sealed class Jax : BalanceCardTemplate
{
    public Jax() : base(0, CardType.Skill, CardRarity.Event, TargetType.Self)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
      new HpLossVar(3M),
    new PowerVar<StrengthPower>(2M),
  ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
      HoverTipFactory.FromPower<StrengthPower>(),
  ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        VfxCmd.PlayOnCreatureCenter(Owner.Creature, "vfx/vfx_bloody_impact");

        await CreatureCmd.Damage(
          choiceContext,
          Owner.Creature,
          base.DynamicVars.HpLoss.BaseValue,
          ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move,
          this, cardPlay);

        await PowerCmd.Apply<StrengthPower>(
          new ThrowingPlayerChoiceContext(),
          Owner.Creature,
          DynamicVars.Strength.BaseValue,
          Owner.Creature,
          this);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Strength.UpgradeValueBy(1M);
    }
}
