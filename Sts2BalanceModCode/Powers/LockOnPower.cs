using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// 跟踪锁定（Lock-On）Debuff。
/// 机制：拥有者受到的充能球伤害增加 50%。闪电球与暗黑球优先以该目标为目标。
/// 拥有者回合结束时层数递减 1。
/// </summary>
[RegisterPower]
public sealed class LockOnPower() : BalancePowerTemplate(PowerType.Debuff, PowerStackType.Counter)
{
  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new DynamicVar("DamageMultiplier", 1.5m)
  ];

  public override decimal ModifyDamageMultiplicative(
      Creature? target,
      decimal amount,
      ValueProp props,
      Creature? dealer,
      CardModel? cardSource,
      CardPlay? cardPlay)
  {
    if (target == Owner && OrbDamageContext.IsOrbDealingDamage)
    {
      return 1.5m;
    }
    return 1m;
  }

  public override async Task AfterSideTurnEnd(
      PlayerChoiceContext choiceContext,
      CombatSide side,
      IEnumerable<Creature> participants)
  {
    if (side == Owner.Side && participants.Contains(Owner) && Amount > 0)
    {
      await PowerCmd.Decrement(this);
    }
  }
}
