using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;


namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// LEGACY-02 — 硬撑（战士）
/// 1费 | 罕见 | 技能 | 获得 15 点格挡, 往手中塞入两张伤口
/// 升级：格挡 15→20
/// </summary>
[RegisterCard(typeof(IroncladCardPool), FullPublicEntry = "STS2_BALANCEMOD_POWER_THOUGHT")]
public sealed class PowerThought : BalanceCardTemplate
{
    protected override IEnumerable<DynamicVar> CanonicalVars => [new BlockVar(15M, ValueProp.Move)];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromCard<Wound>()];

    public PowerThought() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
        await CardPileCmd.AddToCombatAndPreview<Wound>(Owner.Creature, PileType.Hand, 2, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(5M);
}
