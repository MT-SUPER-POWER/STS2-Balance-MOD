using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.CardPools;
using Sts2BalanceMod.src.Abstract;
using Sts2BalanceMod.src.Powers;

namespace Sts2BalanceMod.src.Cards;

/// <summary>
/// 猎人：步步为营
/// 金卡，技能，消耗 X 费用，再接来 X 回合，多抽1卡，多加1费用
/// 升级：X+1 + 保留
///
/// 设计参考：ActsFromThePast 的 Doppelganger 思路（但该卡尚未实现），机制对标 ClarityPower + EnergyNextTurnPower。
/// </summary>
[RegisterCard(typeof(SilentCardPool), FullPublicEntry = "STS2_BALANCEMOD_STEP_BY_STEP")]
public sealed class StepByStep : BalanceCardTemplate
{
  protected override bool HasEnergyCostX => true;

  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.FromPower<StepByStepPower>()];

  public StepByStep() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
    int xValue = ResolveEnergyXValue();
    if (IsUpgraded)
      xValue++;

    // 应用 Power
    var power = await PowerCmd.Apply<StepByStepPower>(choiceContext, Owner.Creature, xValue, Owner.Creature, this);
    // 跳过当回合结束时的递减（打牌当回合不计入消耗）
    if (power != null)
      power.SkipNextDurationTick = true;
  }

  protected override void OnUpgrade()
  {
    AddKeyword(CardKeyword.Retain);
  }
}
