using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Random;

namespace Sts2BalanceMod.Sts2BalanceModCode.Utility;

/// <summary>
/// 根据怪物、战斗随机数与完整行动历史动态选择后继状态。
/// 用于无法由原版固定条件分支精确表达的 AFP 怪物行动概率与防连续规则。
/// </summary>
public sealed class RngConditionalBranchState(
  string stateId,
  Func<Creature, Rng, MonsterMoveStateMachine, string> selectNextState) : MonsterState
{
  public override string Id => stateId;

  public override bool ShouldAppearInLogs => false;

  public override string GetNextState(Creature owner, Rng rng)
  {
    var stateMachine = owner.Monster?.MoveStateMachine
      ?? throw new InvalidOperationException("RNG branch state requires an initialized monster state machine.");
    return selectNextState(owner, rng, stateMachine);
  }

  public override void RegisterStates(Dictionary<string, MonsterState> monsterStates)
  {
    monsterStates.Add(Id, this);
  }
}
