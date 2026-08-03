using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// 巫术打击 - 1 费（升级 0 费）造成 9 点伤害，抽 1 张牌，施加 1 层巫术易伤。消耗
/// </summary>
[Pool(typeof(EventCardPool))]
public sealed class SorceryStrike : Sts2CardModel
{
    public SorceryStrike() : base(1, CardType.Attack, CardRarity.Ancient, TargetType.AnyEnemy)
    {
        WithDamage(9);
        WithPower<SorceryVulnerable>(1);
        WithTags(CardTag.Strike);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardAttack(this, cardPlay)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        await CardPileCmd.Draw(choiceContext, 1, Owner);

        if (cardPlay.Target != null)
        {
            await PowerCmd.Apply<SorceryVulnerable>(choiceContext, cardPlay.Target, DynamicVars["SorceryVulnerable"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
