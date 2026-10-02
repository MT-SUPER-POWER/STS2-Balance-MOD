using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Random;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// 哨兵 (Sentry)。
/// 经典一代第一幕精英，携带 1 层人工制品。左右与中间哨兵交替释放高额激光伤害与向弃牌堆塞入晕眩牌 (Dazed)。
/// </summary>
[RegisterMonster]
public sealed class Sentry : BalanceMonsterTemplate
{
  public const string BOLT = "BOLT";
  public const string BEAM = "BEAM";

  public override MonsterAssetProfile AssetProfile => new(
    ModAssetPaths.Resource("monsters", "sentry", "sentry.tscn"));

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 39, 38);
  public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 45, 42);

  private static int BeamDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 9);
  private static int DazedAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

  private bool _boltFirst;

  public bool BoltFirst
  {
    get => _boltFirst;
    set
    {
      AssertMutable();
      _boltFirst = value;
    }
  }

  public override async Task AfterAddedToRoom()
  {
    await base.AfterAddedToRoom();
    await PowerCmd.Apply<ArtifactPower>(new ThrowingPlayerChoiceContext(), Creature, 1m, Creature, null);
  }

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    List<MonsterState> states = [];

    MoveState boltState = new(
      BOLT,
      BoltMove,
      [new StatusIntent(DazedAmount)]
    );

    MoveState beamState = new(
      BEAM,
      BeamMove,
      [new SingleAttackIntent(BeamDamage)]
    );

    boltState.FollowUpState = beamState;
    beamState.FollowUpState = boltState;

    states.Add(boltState);
    states.Add(beamState);

    MoveState initialState = BoltFirst ? boltState : beamState;
    return new MonsterMoveStateMachine(states, initialState);
  }

  private async Task BoltMove(IReadOnlyList<Creature> targets)
  {
    string spazAnim = Rng.Chaotic.NextInt(3) switch
    {
      0 => "spaz1",
      1 => "spaz2",
      _ => "spaz3"
    };

    await CreatureCmd.TriggerAnim(Creature, spazAnim, 0.0f);
    NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short);
    await Cmd.Wait(0.4f);

    await CardPileCmd.AddToCombatAndPreview<Dazed>(targets, PileType.Discard, DazedAmount, (Player?)null);
  }

  private async Task BeamMove(IReadOnlyList<Creature> targets)
  {
    await CreatureCmd.TriggerAnim(Creature, "attack", 0.0f);
    NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short);
    await Cmd.Wait(0.25f);

    await DamageCmd.Attack(BeamDamage)
      .FromMonster(this)
      .WithHitFx("vfx/vfx_attack_pierce")
      .Execute(null);
  }

  public override CreatureAnimator GenerateAnimator(MegaSprite controller)
  {
    AnimState idle = new("idle", true);
    AnimState attack = new("attack");
    AnimState spaz1 = new("spaz1");
    AnimState spaz2 = new("spaz2");
    AnimState spaz3 = new("spaz3");
    AnimState hit = new("hit");

    attack.NextState = idle;
    spaz1.NextState = idle;
    spaz2.NextState = idle;
    spaz3.NextState = idle;
    hit.NextState = idle;

    CreatureAnimator animator = new(idle, controller);
    animator.AddAnyState("Attack", attack);
    animator.AddAnyState("spaz1", spaz1);
    animator.AddAnyState("spaz2", spaz2);
    animator.AddAnyState("spaz3", spaz3);
    animator.AddAnyState("Hit", hit);

    MegaAnimationState animState = controller.GetAnimationState();
    animState.SetTimeScale(2.0f);
    MegaTrackEntry? current = animState.GetCurrent(0);
    current?.SetTrackTime(Rng.Chaotic.NextFloat(current.GetAnimationEnd()));
    animState.Update(0.0f);
    animState.Apply(controller.GetSkeleton()!);

    return animator;
  }
}
