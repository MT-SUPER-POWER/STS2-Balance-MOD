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
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Random;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Animations;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Combat;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// EVENT-COLOSSEUM-01 — 地精大块头 (GremlinNob)。
/// 首回合怒吼获得狂怒（玩家打技能牌其获得力量），后续交替释放易伤重击与致命猛冲。
/// </summary>
[RegisterMonster]
public sealed class GremlinNob : BalanceMonsterTemplate
{
  public override MonsterAssetProfile AssetProfile => new(
    ModAssetPaths.Resource("monsters", "gremlin_nob", "gremlin_nob.tscn"));

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 85, 82);
  public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 90, 86);

  private static int RushDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 16, 14);
  private static int BashDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 8, 6);
  private const int VulnerableAmount = 2;
  private static int EnrageAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

  private const string RUSH = "RUSH";
  private const string SKULL_BASH = "SKULL_BASH";
  private const string BELLOW = "BELLOW";
  protected override string AttackSfx => "event:/sfx/enemy/enemy_attacks/gremlin_merc/sneaky_gremlin_attack";

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    List<MonsterState> states = [];

    MoveState bellowState = new(
      BELLOW,
      Bellow,
      [new BuffIntent()]
    );

    MoveState rushState = new(
      RUSH,
      Rush,
      [new SingleAttackIntent(RushDamage)]
    );

    MoveState skullBashState = new(
      SKULL_BASH,
      SkullBash,
      [new SingleAttackIntent(BashDamage), new DebuffIntent()]
    );

    RngConditionalBranchState moveBranch = new("MOVE_BRANCH", SelectNextMove);

    bellowState.FollowUpState = moveBranch;
    rushState.FollowUpState = moveBranch;
    skullBashState.FollowUpState = moveBranch;

    states.Add(bellowState);
    states.Add(rushState);
    states.Add(skullBashState);
    states.Add(moveBranch);

    // 首回合固定使用怒吼
    return new MonsterMoveStateMachine(states, bellowState);
  }

  private static string SelectNextMove(Creature owner, Rng rng, MonsterMoveStateMachine stateMachine)
  {
    // 若过去两回合未使用过重击，优先使用重击
    if (!LastMove(stateMachine, SKULL_BASH) && !LastMoveBefore(stateMachine, SKULL_BASH))
    {
      return SKULL_BASH;
    }

    // 若已连续两次猛冲，转为重击
    if (LastTwoMoves(stateMachine, RUSH))
    {
      return SKULL_BASH;
    }

    return RUSH;
  }

  private async Task Bellow(IReadOnlyList<Creature> targets)
  {
    VfxCmd.PlayOnCreatureCenter(Creature, "vfx/vfx_scream");
    await Cmd.Wait(0.5f);

    // 玩家每打出一张技能牌，地精大块头获得力量（多人保持至少 2 点）
    await PowerCmd.Apply<EnragePower>(new ThrowingPlayerChoiceContext(), Creature, EnrageAmount, Creature, null);
  }

  private async Task Rush(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature, async () =>
    {
      await DamageCmd.Attack(RushDamage)
            .FromMonster(this)
            .WithNoAttackerAnim()
            .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: TmpSfx.bluntAttack)
            .Execute(null);

      NGame.Instance?.ScreenShake(ShakeStrength.Medium, ShakeDuration.Short);
    });
  }

  private async Task SkullBash(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature, async () =>
    {
      await DamageCmd.Attack(BashDamage)
            .FromMonster(this)
            .WithNoAttackerAnim()
            .WithHitFx("vfx/vfx_heavy_blunt", tmpSfx: TmpSfx.heavyAttack)
            .Execute(null);

      NGame.Instance?.ScreenShake(ShakeStrength.Strong, ShakeDuration.Short);
    });

    foreach (Creature target in targets.Where(t => t.IsAlive))
    {
      await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), target, VulnerableAmount, Creature, null);
    }
  }

  public override CreatureAnimator GenerateAnimator(MegaSprite controller)
  {
    AnimState idle = new("animation", true);
    return new CreatureAnimator(idle, controller);
  }
}
