using BaseLib.Extensions;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// CARD-NEW — 猛撞 (Ram)
/// 2费 | 攻击 | 普通
/// 奥斯提失去6点生命，对所有敌人造成20点伤害。只有在奥斯提拥有至少 5 点生命时才会触发效果。
/// 升级：伤害 20->26，奥斯提失去生命 6->5。
/// </summary>
[Pool(typeof(NecrobinderCardPool))]
public sealed class Ram : Sts2CardModel
{
  protected override bool ShouldGlowRedInternal => base.Owner.IsOstyMissing;

  protected override bool IsPlayable => !base.Owner.IsOstyMissing;

  public Ram()
      : base(2, CardType.Attack, CardRarity.Common, TargetType.AllEnemies)
  {
    WithDamage(20, 6); // 基础 20，升级 +6 = 26
    WithVar(new HpLossVar(6m).WithUpgrade(-1m)); // 基础 6，升级 -1 = 5
  }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    // 约束：奥斯提必须拥有至少与扣除生命一致的生命值才会触发效果
    if (Owner.IsOstyAlive && Owner.Osty != null && Owner.Osty.CurrentHp >= DynamicVars.HpLoss.BaseValue)
    {
      // 1. 奥斯提失去相应生命值 (HpLoss)
      await CreatureCmd.Damage(
        choiceContext,
        Owner.Osty,
        DynamicVars.HpLoss.BaseValue,
        ValueProp.Unblockable | ValueProp.Unpowered,
        null,
        this,
        cardPlay
      );

      // 2. 人物播放施法动画并对所有敌人造成伤害
      await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
      await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
          .FromCard(this, cardPlay)
          .TargetingAllOpponents(CombatState!)
          .WithHitFx("vfx/vfx_giant_horizontal_slash")
          .Execute(choiceContext);
    }
  }
}
