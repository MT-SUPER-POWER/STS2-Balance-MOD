using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// LEGACY-03 — 死亡收割
/// 2费 | 攻击 | 消耗 | 造成 4 点伤害，回复等量于非格挡伤害的生命
/// 升级：伤害 4→6
/// </summary>
[Pool(typeof(IroncladCardPool))]
public sealed class DeathReap : Sts2CardModel
{
  public DeathReap()
      : base(2, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy)
  {
    WithDamage(4, 2); // 基础 4，升级 +2 = 6
    WithKeywords(CardKeyword.Exhaust);
  }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    var target = cardPlay.Target;
    if (target == null) return;

    var result = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
        .FromCard(this)
        .Targeting(target)
        .Execute(choiceContext);

    int healed = result.Results
        .SelectMany(r => r)
        .Sum(r => r.UnblockedDamage);

    if (healed > 0)
      await CreatureCmd.Heal(Owner.Creature, healed);
  }
}
