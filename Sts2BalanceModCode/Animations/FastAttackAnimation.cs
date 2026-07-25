using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Sts2BalanceMod.Sts2BalanceModCode.Animations;

/// <summary>
/// 快速前冲攻击动画，用于还原 STS1 怪物的简单突进动作。
/// </summary>
public static class FastAttackAnimation
{
    private const float _animationDuration = 0.4f;
    private const float _actionDuration = 0.25f;
    private const float _targetDistance = 90f;

    public static async Task Play(Creature creature)
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(creature);
        if (creatureNode == null)
        {
            return;
        }

        var originalPosition = creatureNode.Position;
        var direction = creature.IsPlayer ? 1f : -1f;
        var tween = creatureNode.CreateTween();

        tween.TweenMethod(
          Callable.From<float>(timer =>
          {
              var t = timer < 0f ? 0f : timer * 2f;
              var easedT = t * t * (3f - 2f * t);
              var xOffset = Mathf.Lerp(0f, _targetDistance, easedT);

              creatureNode.Position = new Vector2(originalPosition.X + xOffset * direction, originalPosition.Y);
          }),
          _animationDuration,
          0f,
          _animationDuration).SetTrans(Tween.TransitionType.Linear);

        await Cmd.Wait(_actionDuration);
    }
}
