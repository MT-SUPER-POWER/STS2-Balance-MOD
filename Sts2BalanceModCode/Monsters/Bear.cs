using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Animations;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

// ======================== 红面具三人帮 — Bear ========================

/// <summary>
/// STS1-EVENT — 红面具强盗 Bear（熊），三人帮最肉的一个。
/// 使用 Bear Hug（减敏）、Maul（重击）、Lunge（攻击+格挡）。
/// </summary>
public sealed class Bear : Sts2MonsterModel
{
  protected override string VisualsPath => "res://Sts2BalanceMod/monsters/bear/bear.tscn";

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 40, 38);
  public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 44, 42);

  private int MaulDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 20, 18);
  private int LungeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 9);
  private const int LungeBlock = 9;
  private const int BearHugVulnerable = 1;

  private const string MAUL = "MAUL";
  private const string BEAR_HUG = "BEAR_HUG";
  private const string LUNGE = "LUNGE";
  private const string AttackHitSfx = "blunt_attack.mp3";
  protected override string AttackSfx => "event:/sfx/enemy/enemy_attacks/gremlin_merc/sneaky_gremlin_attack";

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    var bearHugState = new MoveState(BEAR_HUG, BearHug, new AbstractIntent[] { new DebuffIntent() });
    var maulState = new MoveState(MAUL, Maul, new AbstractIntent[] { new SingleAttackIntent(MaulDamage) });
    var lungeState = new MoveState(LUNGE, Lunge, new AbstractIntent[] { new SingleAttackIntent(LungeDamage), new DefendIntent() });

    bearHugState.FollowUpState = lungeState;
    lungeState.FollowUpState = maulState;
    maulState.FollowUpState = lungeState;

    return new MonsterMoveStateMachine([bearHugState, maulState, lungeState], bearHugState);
  }

  private async Task BearHug(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    SfxCmd.Play(AttackSfx);

    foreach (var target in targets.Where(t => t.IsAlive))
    {
      await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), target, BearHugVulnerable, Creature, null);
    }
  }

  private async Task Maul(IReadOnlyList<Creature> targets)
  {
    await DamageCmd.Attack(MaulDamage)
        .FromMonster(this)
        .WithAttackerAnim("Attack", 0.4f)
        .WithHitFx("vfx/vfx_attack_blunt", null, AttackHitSfx)
        .Execute(null);
  }

  private async Task Lunge(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);

    await DamageCmd.Attack(LungeDamage)
        .FromMonster(this)
        .WithHitFx("vfx/vfx_attack_slash", null, AttackHitSfx)
        .Execute(null);

    await CreatureCmd.GainBlock(Creature, LungeBlock, ValueProp.Move, null);
  }

  public override CreatureAnimator GenerateAnimator(MegaSprite controller)
  {
    var idle = new AnimState("Idle", true);
    var attack = new AnimState("Attack");
    var hit = new AnimState("Hit");

    attack.NextState = idle;
    hit.NextState = idle;

    var animator = new CreatureAnimator(idle, controller);
    animator.AddAnyState("Attack", attack);
    animator.AddAnyState(BEAR_HUG, attack);
    animator.AddAnyState("Maul", attack);
    animator.AddAnyState(MAUL, attack);
    animator.AddAnyState(LUNGE, attack);
    animator.AddAnyState("Hit", hit);

    return animator;
  }

  protected override string? GetBestiaryMoveAnimationId(string moveStateId)
  {
    return moveStateId switch
    {
      BEAR_HUG or MAUL or LUNGE => "Attack",
      _ => base.GetBestiaryMoveAnimationId(moveStateId),
    };
  }
}
