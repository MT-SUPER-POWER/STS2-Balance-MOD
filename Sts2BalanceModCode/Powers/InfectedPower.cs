using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// 【感染】能力模型。
/// 输入：玩家回合结束事件。
/// 输出：在玩家回合结束时，使拥有者受到等同于感染层数的伤害（可被格挡）。
/// 注：感染的施加统一由 InfestedPrismPatch 的 ApplyInfectionFromAttack 负责（只要有未格挡伤害，每段固定+2层），
///      本类中不重写 AfterDamageReceived，避免双重触发或误将 UnblockedDamage 值当成层数施加。
/// </summary>
public sealed class InfectedPower() : Sts2PowerModel(PowerType.Debuff, PowerStackType.Counter)
{
  public const int InfectionPerHit = 2;

  public override string CustomPackedIconPath => "res://Sts2BalanceMod/images/powers/infected_power.png";
  public override string CustomBigIconPath => "res://Sts2BalanceMod/images/powers/big/infected_power.png";

  public override async Task BeforeSideTurnEnd(
    PlayerChoiceContext choiceContext,
    CombatSide side,
    IEnumerable<Creature> participants)
  {
    if (side != Owner.Side || !participants.Contains(Owner) || Amount <= 0)
    {
      return;
    }

    Flash();
    await CreatureCmd.Damage(choiceContext, Owner, Amount, ValueProp.Unpowered, null, null);
  }
}
