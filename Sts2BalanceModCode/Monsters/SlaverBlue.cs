using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Random;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Animations;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Combat;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// EVENT-COLOSSEUM-01 — 蓝奴隶贩子 (SlaverBlue)。
/// 刺击与虚弱搜刮轮替，提供持续施压与防御削弱。
/// </summary>
[RegisterMonster]
public sealed class SlaverBlue : BalanceMonsterTemplate
{
  public override MonsterAssetProfile AssetProfile => new(
    ModAssetPaths.Resource("monsters", "slaver_blue", "slaver_blue.tscn"));

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 48, 46);
  public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 52, 50);

  private static int StabDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 13, 12);
  private static int RakeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 8, 7);
  private static int WeakAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 2, 1);

  private const string STAB = "STAB";
  private const string RAKE = "RAKE";
  protected override string AttackSfx => "event:/sfx/enemy/enemy_attacks/gremlin_merc/sneaky_gremlin_attack";

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    List<MonsterState> states = [];

    MoveState stabState = new(
      STAB,
      Stab,
      [new SingleAttackIntent(StabDamage)]
    );

    MoveState rakeState = new(
      RAKE,
      Rake,
      [new SingleAttackIntent(RakeDamage), new DebuffIntent()]
    );

    RngConditionalBranchState moveBranch = new("MOVE_BRANCH", SelectNextMove);

    stabState.FollowUpState = moveBranch;
    rakeState.FollowUpState = moveBranch;

    states.Add(stabState);
    states.Add(rakeState);
    states.Add(moveBranch);

    return new MonsterMoveStateMachine(states, moveBranch);
  }

  private static string SelectNextMove(Creature owner, Rng rng, MonsterMoveStateMachine stateMachine)
  {
    int num = rng.NextInt(100);

    // 60% 概率使用刺击（不可连续两次以上）
    if (num >= 40 && !LastTwoMoves(stateMachine, STAB))
    {
      return STAB;
    }

    // 否则使用搜刮（不可连续搜刮）
    if (!LastMove(stateMachine, RAKE))
    {
      return RAKE;
    }

    return STAB;
  }

  private async Task Stab(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature, async () =>
    {
      await DamageCmd.Attack(StabDamage)
            .FromMonster(this)
            .WithNoAttackerAnim()
            .WithHitFx("vfx/vfx_attack_slash", tmpSfx: TmpSfx.slashAttack)
            .Execute(null);
    });
  }

  private async Task Rake(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature, async () =>
    {
      await DamageCmd.Attack(RakeDamage)
            .FromMonster(this)
            .WithNoAttackerAnim()
            .WithHitFx("vfx/vfx_attack_slash", tmpSfx: TmpSfx.slashAttack)
            .Execute(null);
    });

    foreach (Creature target in targets.Where(t => t.IsAlive))
    {
      await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, WeakAmount, Creature, null);
    }
  }

  public override CreatureAnimator GenerateAnimator(MegaSprite controller)
  {
    AnimState idle = new("idle", true);
    return new CreatureAnimator(idle, controller);
  }
}
