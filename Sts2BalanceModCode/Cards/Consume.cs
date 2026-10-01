using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// CARD-CONSUME-01 — 耗尽（Consume）
/// 2费 | 稀有 | 技能 | 获得 2(+) 点集中，失去 1 个充能球栏位
/// 故障机器人专属金卡，高风险高回报的集中成长核心牌。
/// </summary>
[RegisterCard(typeof(DefectCardPool), FullPublicEntry = "STS2_BALANCEMOD_CONSUME")]
public sealed class Consume : BalanceCardTemplate
{
  private const string _orbSlotsKey = "OrbSlots";

  protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
  [
    HoverTipFactory.FromPower<FocusPower>()
  ];

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new PowerVar<FocusPower>(2m),
    new DynamicVar(_orbSlotsKey, 1m)
  ];

  public Consume() : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    OrbCmd.RemoveSlots(Owner, DynamicVars[_orbSlotsKey].IntValue);
    await PowerCmd.Apply<FocusPower>(choiceContext, Owner.Creature, DynamicVars["FocusPower"].BaseValue, Owner.Creature, this);
  }

  protected override void OnUpgrade() => DynamicVars["FocusPower"].UpgradeValueBy(1m);
}
