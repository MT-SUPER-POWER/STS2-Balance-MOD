using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using STS2RitsuLib.Scaffolding.Content;

namespace Sts2BalanceMod.Sts2BalanceModCode.Abstract;

/// <summary>
/// MOD 怪物模型抽象基类，提供通用默认值。
/// 所有 MOD 自定义怪物都应该继承此类。
/// </summary>
public abstract class BalanceMonsterTemplate : ModMonsterTemplate
{
    protected virtual string LocalizationEntry => Id.Entry;

    public string ModVisualsPath => CustomVisualsPath ?? VisualsPath;

    public override LocString Title => MonsterLoc("name");

    /// <summary>
    /// 获取当前怪物的本地化文本（key 格式为 {LocalizationEntry}.{subKey}）。
    /// </summary>
    protected LocString MonsterLoc(string subKey) => new("monsters", $"{LocalizationEntry}.{subKey}");

    /// <summary>
    /// 获取怪物指定动作的标题本地化文本（自动去除 _MOVE 尾缀）。
    /// </summary>
    protected LocString MoveTitle(string moveStateId)
    {
        var locMoveId = moveStateId.EndsWith("_MOVE") ? moveStateId[..^5] : moveStateId;
        return new LocString("monsters", $"{LocalizationEntry}.moves.{locMoveId}.title");
    }

    /// <summary>
    /// 判断怪物上一次行动是否为指定动作。
    /// </summary>
    protected static bool LastMove(MonsterMoveStateMachine? stateMachine, string moveId)
    {
        if (stateMachine == null) return false;
        var log = stateMachine.StateLog;
        return log.Count > 0 && log[^1].Id == moveId;
    }

    /// <summary>
    /// 判断怪物最近两次行动是否均为指定动作。
    /// </summary>
    protected static bool LastTwoMoves(MonsterMoveStateMachine? stateMachine, string moveId)
    {
        if (stateMachine == null) return false;
        var log = stateMachine.StateLog;
        return log.Count >= 2 && log[^1].Id == moveId && log[^2].Id == moveId;
    }

    /// <summary>
    /// 判断怪物倒数第 N 次（1-indexed，1 表示最近一次，2 表示上上次）行动是否为指定动作。
    /// </summary>
    protected static bool LastMoveBefore(MonsterMoveStateMachine? stateMachine, string moveId, int turnsAgo = 1)
    {
        if (stateMachine == null || turnsAgo <= 0) return false;
        var log = stateMachine.StateLog;
        return log.Count >= turnsAgo && log[^turnsAgo].Id == moveId;
    }

    public override List<BestiaryMonsterMove> GenerateBestiaryMoveList(NCreatureVisuals? creatureVisuals)
    {
        var moves = new List<BestiaryMonsterMove>();
        var states = MoveStateMachine?.States;

        if (states == null)
        {
            return moves;
        }

        foreach (var state in states)
        {
            var stateId = state.Key;
            var monsterState = state.Value;

            if (string.IsNullOrEmpty(stateId) || monsterState is not MoveState ||
              !ShouldShowMoveInBestiary(stateId))
            {
                continue;
            }

            var moveId = stateId;
            var moveName = MoveTitle(moveId);
            var animationId = GetBestiaryMoveAnimationId(moveId);

            if (moveName.Exists() && animationId != null)
            {
                moves.Add(BestiaryMonsterMove.FromAction(moveName,
                  () => PlayBestiaryAnimation(creatureVisuals, animationId)));
            }
            else
            {
                moves.Add(moveName.Exists()
                  ? BestiaryMonsterMove.FromState(moveName, moveId)
                  : BestiaryMonsterMove.FromState(moveId));
            }
        }

        MegaSkeletonDataResource? skeletonData = creatureVisuals?.SpineBody?.GetSkeleton()?.GetData();
        if (skeletonData != null && skeletonData.HasAnimation("revive"))
        {
            moves.Add(BestiaryMonsterMove.FromAnim("revive", null));
        }

        if (skeletonData != null && skeletonData.HasAnimation("hurt"))
        {
            moves.Add(BestiaryMonsterMove.FromAnim("hurt", TakeDamageSfx).StopOtherSfx());
        }

        if (skeletonData != null && skeletonData.HasAnimation("die"))
        {
            moves.Add(BestiaryMonsterMove.FromAnim("die", DeathSfx).StopOtherSfx());
        }

        return moves;
    }

    protected virtual string? GetBestiaryMoveAnimationId(string moveStateId)
    {
        return null;
    }

    private static Task PlayBestiaryAnimation(NCreatureVisuals? creatureVisuals, string animationId)
    {
        var spine = creatureVisuals?.SpineBody;
        var skeletonData = spine?.GetSkeleton()?.GetData();
        if (spine == null || skeletonData == null || !skeletonData.HasAnimation(animationId))
        {
            return Task.CompletedTask;
        }

        var animationState = spine.GetAnimationState();
        animationState.SetAnimation(animationId, loop: false);

        if (skeletonData.HasAnimation("Idle"))
        {
            animationState.AddAnimation("Idle", 0f, loop: true);
        }
        else if (skeletonData.HasAnimation("idle_loop"))
        {
            animationState.AddAnimation("idle_loop", 0f, loop: true);
        }

        return Task.CompletedTask;
    }
}
