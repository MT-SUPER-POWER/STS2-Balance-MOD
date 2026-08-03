using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Sts2BalanceMod.Sts2BalanceModCode.Animations;

/// <summary>
/// AFP 史莱姆老大的抛物线跳跃动画。
/// </summary>
public static class JumpAnimation
{
  private const float AnimationDuration = 0.7f;
  private const float ActionDuration = 0.25f;
  private const float JumpHeight = 150f;

  public static async Task Play(Creature creature)
  {
    var creatureNode = NCombatRoom.Instance?.GetCreatureNode(creature);
    var visuals = creatureNode?.Visuals;
    if (creatureNode == null || visuals == null)
      return;

    var originalPosition = visuals.Position;
    var tween = creatureNode.CreateTween();
    tween.TweenMethod(
        Callable.From<float>(progress =>
        {
          var verticalOffset = 4f * JumpHeight * progress * (1f - progress);
          visuals.Position = new Vector2(originalPosition.X, originalPosition.Y - verticalOffset);
        }),
        0f,
        1f,
        AnimationDuration)
      .SetTrans(Tween.TransitionType.Linear);

    await Cmd.Wait(ActionDuration);
  }
}
