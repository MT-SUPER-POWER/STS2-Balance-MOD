using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// 巫术防御 - 1 费（升级 0 费）获得 8 点格挡，抽 1 张牌，施加 1 层巫术虚弱。消耗
/// </summary>
[Pool(typeof(EventCardPool))]
public sealed class SorceryDefend : Sts2CardModel
{
    public SorceryDefend() : base(1, CardType.Skill, CardRarity.Ancient, TargetType.AnyEnemy)
    {
        WithBlock(8);
        WithVar(new DynamicVar("SorceryWeak", 1));
        WithTags(CardTag.Defend);
        WithKeywords(CardKeyword.Exhaust);
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CommonActions.CardBlock(this, cardPlay);

        await CardPileCmd.Draw(choiceContext, 1, Owner);

        if (cardPlay.Target != null)
        {
            await PowerCmd.Apply<SorceryWeak>(choiceContext, cardPlay.Target, DynamicVars["SorceryWeak"].IntValue, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        EnergyCost.UpgradeBy(-1);
    }
}
