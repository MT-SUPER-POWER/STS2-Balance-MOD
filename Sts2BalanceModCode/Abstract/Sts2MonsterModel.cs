using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Sts2BalanceMod.Sts2BalanceModCode.Abstract;

/// <summary>
/// MOD 怪物模型抽象基类，提供通用默认值。
/// 所有 MOD 自定义怪物都应该继承此类。
/// </summary>
public abstract class Sts2MonsterModel : MonsterModel
{
  public string ModVisualsPath => VisualsPath;

  private string ModLocalizationEntry =>
    Id.Entry.StartsWith("STS2BALANCEMOD-") ? Id.Entry : "STS2BALANCEMOD-" + Id.Entry;

  // NOTE: 原版 MonsterModel 使用 base.Id.Entry 读取怪物本地化，
  // 但 BaseLib 不会给原版 LocString 自动补 MOD 前缀，这里显式使用 MOD key。
  public override LocString Title => new("monsters", ModLocalizationEntry + ".name");

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
      var locMoveId = moveId.EndsWith("_MOVE") ? moveId[..^5] : moveId;
      var moveName = new LocString("monsters", $"{ModLocalizationEntry}.moves.{locMoveId}.title");
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
