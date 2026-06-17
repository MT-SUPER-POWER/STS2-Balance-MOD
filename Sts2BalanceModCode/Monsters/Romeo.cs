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
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

// ======================== 红面具三人帮 — Romeo ========================

/// <summary>
/// STS1-EVENT — 红面具强盗 Romeo，嘲讽后切换 Cross Slash（高伤）和 Agonizing Slash（上易伤）。
/// </summary>
public sealed class Romeo : Sts2MonsterModel
{
  protected override string VisualsPath => "res://Assets/ActsFromThePast/ActsFromThePast/monsters/romeo/romeo.tscn";

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 37, 35);
  public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 41, 39);

  private int CrossSlashDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 17, 15);
  private int AgonizeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 10);
  private const int WeakAmount = 3;

  private const string CROSS_SLASH = "CROSS_SLASH";
  private const string MOCK = "MOCK";
  private const string AGONIZING_SLASH = "AGONIZING_SLASH";

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    var mockState = new MoveState(MOCK, Mock, new AbstractIntent[] { new UnknownIntent() });
    var crossSlashState = new MoveState(CROSS_SLASH, CrossSlash, new AbstractIntent[] { new SingleAttackIntent(CrossSlashDamage) });
    var agonizingSlashState = new MoveState(AGONIZING_SLASH, AgonizingSlash, new AbstractIntent[] { new SingleAttackIntent(AgonizeDamage), new DebuffIntent() });
    var moveBranch = new ConditionalBranchState("MOVE_BRANCH");

    mockState.FollowUpState = agonizingSlashState;
    agonizingSlashState.FollowUpState = moveBranch;
    crossSlashState.FollowUpState = moveBranch;

    // NOTE: 使用闭包捕获 machine 引用，构造完成后才赋值，运行时条件才被求值
    MonsterMoveStateMachine? machine = null;
    moveBranch.AddState(crossSlashState, () => !LastTwoMoves(machine, CROSS_SLASH));
    moveBranch.AddState(agonizingSlashState, () => true); // 回退

    machine = new MonsterMoveStateMachine([mockState, crossSlashState, agonizingSlashState, moveBranch], mockState);
    return machine;
  }

  private static bool LastTwoMoves(MonsterMoveStateMachine? machine, string moveId)
  {
    if (machine == null) return false;
    var log = machine.StateLog;
    if (log.Count < 2) return false;
    return log[log.Count - 1].Id == moveId && log[log.Count - 2].Id == moveId;
  }

  private async Task Mock(IReadOnlyList<Creature> targets)
  {
    await CreatureCmd.TriggerAnim(Creature, "Attack", 0.0f);
    await Cmd.Wait(0.5f);
  }

  private async Task CrossSlash(IReadOnlyList<Creature> targets)
  {
    await CreatureCmd.TriggerAnim(Creature, "Stab", 0.0f);
    await Cmd.Wait(0.4f);

    await DamageCmd.Attack(CrossSlashDamage)
        .FromMonster(this)
        .WithHitFx("vfx/vfx_attack_slash")
        .Execute(null);
  }

  private async Task AgonizingSlash(IReadOnlyList<Creature> targets)
  {
    await CreatureCmd.TriggerAnim(Creature, "Stab", 0.0f);
    await Cmd.Wait(0.4f);

    await DamageCmd.Attack(AgonizeDamage)
        .FromMonster(this)
        .WithHitFx("vfx/vfx_attack_slash")
        .Execute(null);

    foreach (var target in targets.Where(t => t.IsAlive))
    {
      await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, WeakAmount, Creature, null);
    }
  }

  public override CreatureAnimator GenerateAnimator(MegaSprite controller)
  {
    var idle = new AnimState("Idle", true);
    var attack = new AnimState("Attack");
    var hit = new AnimState("Hit");

    attack.NextState = idle;
    hit.NextState = idle;

    var animator = new CreatureAnimator(idle, controller);
    animator.AddAnyState("Stab", attack);
    animator.AddAnyState("Hit", hit);

    return animator;
  }
}
