using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// CARD-BULLSEYE-01 — 瞄准靶心（Bullseye）
/// 1费 | 罕见 | 攻击 | 造成 8（升级 11）点伤害，给予 2（升级 3）层锁定
/// </summary>
[RegisterCard(typeof(DefectCardPool), FullPublicEntry = "STS2_BALANCEMOD_BULLSEYE")]
public sealed class Bullseye : BalanceCardTemplate
{
  protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
  [
    HoverTipFactory.FromPower<LockOnPower>()
  ];

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new DamageVar(8m, ValueProp.Move),
    new PowerVar<LockOnPower>(2m)
  ];

  public Bullseye() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

    await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
      .WithHitFx("vfx/vfx_attack_slash")
      .Execute(choiceContext);

    await PowerCmd.Apply<LockOnPower>(choiceContext, cardPlay.Target, DynamicVars["LockOnPower"].BaseValue, Owner.Creature, this);
  }

  protected override void OnUpgrade()
  {
    DynamicVars.Damage.UpgradeValueBy(3m);
    DynamicVars["LockOnPower"].UpgradeValueBy(1m);
  }
}
