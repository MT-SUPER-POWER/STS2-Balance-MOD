using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// 巫术防御 - 1 费（升级 0 费）获得 8 点格挡，抽 1 张牌，施加 1 层巫术虚弱。消耗
/// </summary>
[RegisterCard(typeof(EventCardPool), FullPublicEntry = "STS2_BALANCEMOD_SORCERY_DEFEND")]
public sealed class SorceryDefend : BalanceCardTemplate
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override HashSet<CardTag> CanonicalTags => [CardTag.Defend];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(8M, ValueProp.Move),
        new PowerVar<SorceryWeak>(1M),
    ];

    public SorceryDefend() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.AnyEnemy) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);

        await CardPileCmd.Draw(choiceContext, 1, Owner);

        if (cardPlay.Target != null)
        {
            await PowerCmd.Apply<SorceryWeak>(choiceContext, cardPlay.Target, DynamicVars[nameof(SorceryWeak)].BaseValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
