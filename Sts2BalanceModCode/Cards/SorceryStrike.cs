using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// 巫术打击 - 1 费（升级 0 费）造成 9 点伤害，抽 1 张牌，施加 1 层巫术易伤。消耗
/// </summary>
[RegisterCard(typeof(EventCardPool), FullPublicEntry = "STS2_BALANCEMOD_SORCERY_STRIKE")]
public sealed class SorceryStrike : BalanceCardTemplate
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Strike];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9M, ValueProp.Move),
        new PowerVar<SorceryVulnerable>(1M),
    ];

    public SorceryStrike() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await CardPileCmd.Draw(choiceContext, 1, Owner);

        if (cardPlay.Target != null)
        {
            await PowerCmd.Apply<SorceryVulnerable>(choiceContext, cardPlay.Target, DynamicVars[nameof(SorceryVulnerable)].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
