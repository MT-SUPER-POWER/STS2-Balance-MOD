using System;
using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// 使用自定义委托选择下一个状态的分支节点。
/// ConditionalBranchState 仅支持 AddState + Func&lt;bool&gt; 条件，
/// 而守护者的进攻分支需要根据状态日志动态选择下一动作，
/// 因此使用此委托式分支实现。
/// </summary>
public sealed class SelectorBranchState : MonsterState
{
    private readonly string _branchId;
    private readonly Func<string> _selector;

    public override string Id => _branchId;
    public override bool ShouldAppearInLogs => false;

    public SelectorBranchState(string branchId, Func<string> selector)
    {
        _branchId = branchId;
        _selector = selector;
    }

    public override string GetNextState(Creature owner, Rng rng) => _selector();

    public override void RegisterStates(Dictionary<string, MonsterState> states)
    {
        states.Add(Id, this);
    }
}
