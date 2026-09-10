using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Bestiary;

namespace Sts2BalanceMod.Sts2BalanceModCode.Runtime.Animations;

/// <summary>
/// 1 代移植怪物使用的短促前冲攻击动画。
/// 前冲到达打击顶点瞬间触发 onHit 回调（伤害结算、受击音效与受击特效），随后平滑回退回原位。
/// </summary>
public static class FastAttackAnimation
{
    private const float ForwardDuration = 0.12f;
    private const float ReturnDuration = 0.22f;
    private const float TargetDistance = 90f;

    public static async Task Play(Creature creature, Func<Task>? onHit = null)
    {
        NCreature? creatureNode = NCombatRoom.Instance?.GetCreatureNode(creature) ?? NBestiary.Instance?.GetCreatureNode(creature);
        if (creatureNode == null)
        {
            if (onHit != null)
            {
                await onHit();
            }
            return;
        }

        Vector2 originalPosition = creatureNode.Position;
        float direction = creature.IsPlayer ? 1f : -1f;
        Vector2 targetPosition = new(originalPosition.X + TargetDistance * direction, originalPosition.Y);

        // 阶段 1：快速前冲刺向目标 (0.12s)
        Tween tweenForward = creatureNode.CreateTween();
        tweenForward.TweenProperty(creatureNode, "position", targetPosition, ForwardDuration)
            .SetTrans(Tween.TransitionType.Cubic)
            .SetEase(Tween.EaseType.Out);

        await Cmd.Wait(ForwardDuration);

        // 阶段 2：在达到最前点的命中瞬间，触发伤害结算与受击反馈！
        Task? hitTask = onHit?.Invoke();

        // 阶段 3：怪物平滑回弹回原位 (0.22s)
        Tween tweenBack = creatureNode.CreateTween();
        tweenBack.TweenProperty(creatureNode, "position", originalPosition, ReturnDuration)
            .SetTrans(Tween.TransitionType.Quad)
            .SetEase(Tween.EaseType.InOut);

        await Cmd.Wait(ReturnDuration);
        creatureNode.Position = originalPosition;

        if (hitTask != null)
        {
            await hitTask;
        }
    }
}
