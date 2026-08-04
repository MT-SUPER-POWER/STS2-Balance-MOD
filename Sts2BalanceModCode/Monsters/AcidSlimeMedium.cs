using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Audio;
using MegaCrit.Sts2.Core.Random;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Animations;
using Sts2BalanceMod.Sts2BalanceModCode.Utility;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// AFP-BOSS-03 — 酸液史莱姆（中），由大型酸液史莱姆分裂产生。
/// </summary>
public sealed class AcidSlimeMedium : Sts2MonsterModel
{
  private const string CorrosiveSpitMove = "CORROSIVE_SPIT";
  private const string TackleMove = "TACKLE";
  private const string LickMove = "LICK";
  private const int WeakTurns = 1;
  private const int SlimedCount = 1;

  private int? _overrideHp;

  public int? OverrideHp
  {
    get => _overrideHp;
    set
    {
      AssertMutable();
      _overrideHp = value;
    }
  }

  public override int MinInitialHp =>
    OverrideHp ?? AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 29, 28);

  public override int MaxInitialHp =>
    OverrideHp ?? AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 34, 32);

  private int CorrosiveSpitDamage =>
    AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 8, 7);

  private int TackleDamage =>
    AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 10);

  protected override string VisualsPath =>
    "res://Sts2BalanceMod/monsters/acid_slime_medium/acid_slime_medium.tscn";

  public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

  public override async Task AfterAddedToRoom()
  {
    await base.AfterAddedToRoom();
    Creature.Died += OnDeath;
  }

  private void OnDeath(Creature _)
  {
    Creature.Died -= OnDeath;
    NAudioManager.Instance?.PlayOneShot("event:/sfx/enemy/enemy_attacks/twig_slime_m/twig_slime_m_die");
  }

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    var corrosiveSpitState = new MoveState(
      CorrosiveSpitMove,
      CorrosiveSpit,
      [new SingleAttackIntent(CorrosiveSpitDamage), new StatusIntent(SlimedCount)]);
    var tackleState = new MoveState(
      TackleMove,
      Tackle,
      [new SingleAttackIntent(TackleDamage)]);
    var lickState = new MoveState(
      LickMove,
      Lick,
      [new DebuffIntent()]);
    var moveBranch = new RngConditionalBranchState("MOVE_BRANCH", SelectNextMove);

    corrosiveSpitState.FollowUpState = moveBranch;
    tackleState.FollowUpState = moveBranch;
    lickState.FollowUpState = moveBranch;

    return new MonsterMoveStateMachine(
      [corrosiveSpitState, tackleState, lickState, moveBranch],
      moveBranch);
  }

  private static string SelectNextMove(Creature owner, Rng rng, MonsterMoveStateMachine stateMachine)
  {
    var roll = rng.NextInt(100);
    if (roll < 40)
    {
      return LastTwoMoves(stateMachine, CorrosiveSpitMove)
        ? rng.NextFloat() < 0.5f ? TackleMove : LickMove
        : CorrosiveSpitMove;
    }

    if (roll < 80)
    {
      return LastTwoMoves(stateMachine, TackleMove)
        ? rng.NextFloat() < 0.5f ? CorrosiveSpitMove : LickMove
        : TackleMove;
    }

    return LastMove(stateMachine, LickMove)
      ? rng.NextFloat() < 0.4f ? CorrosiveSpitMove : TackleMove
      : LickMove;
  }

  private static bool LastMove(MonsterMoveStateMachine stateMachine, string moveId)
  {
    return stateMachine.StateLog.Count > 0 && stateMachine.StateLog[^1].Id == moveId;
  }

  private static bool LastTwoMoves(MonsterMoveStateMachine stateMachine, string moveId)
  {
    var log = stateMachine.StateLog;
    return log.Count >= 2 && log[^1].Id == moveId && log[^2].Id == moveId;
  }

  private async Task CorrosiveSpit(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    await DamageCmd.Attack(CorrosiveSpitDamage)
      .FromMonster(this)
      .WithAttackerFx(sfx: "event:/sfx/enemy/enemy_attacks/twig_slime_m/twig_slime_m_attack")
      .WithHitFx("vfx/vfx_slime_impact")
      .Execute(null);

    // NOTE: AFP 默认关闭“一代黏液牌”兼容项，因此直接生成 STS2 当前 Slimed。
    await CardPileCmd.AddToCombatAndPreview<Slimed>(targets, PileType.Discard, SlimedCount, (Player?)null);
  }

  private async Task Tackle(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    await DamageCmd.Attack(TackleDamage)
      .FromMonster(this)
      .WithAttackerFx(sfx: "event:/sfx/enemy/enemy_attacks/twig_slime_m/twig_slime_m_attack")
      .WithHitFx("vfx/vfx_slime_impact")
      .Execute(null);
  }

  private async Task Lick(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    foreach (var target in targets.Where(target => target.IsAlive))
    {
      await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, WeakTurns, Creature, null);
    }
  }

  public override CreatureAnimator GenerateAnimator(MegaSprite controller)
  {
    var idle = new AnimState("idle", true);
    var damage = new AnimState("damage") { NextState = idle };
    var animator = new CreatureAnimator(idle, controller);
    animator.AddAnyState("Hit", damage);
    return animator;
  }
}
