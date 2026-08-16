using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

// ======================== 红面具三人帮 — Romeo ========================

/// <summary>
/// STS1-EVENT — 红面具强盗 Romeo，嘲讽后切换 Cross Slash（高伤）和 Agonizing Slash（上易伤）。
/// </summary>
[RegisterMonster]
public sealed class Romeo : BalanceMonsterTemplate
{
    public override MonsterAssetProfile AssetProfile => new(
      ModAssetPaths.Resource("monsters", "romeo", "romeo.tscn"));

    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 37, 35);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 41, 39);

    private static int CrossSlashDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 17, 15);
    private static int AgonizeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 10);
    private const int WeakAmount = 3;
    private static readonly LocString MockBearAliveLine =
      L10NMonsterLookup("STS2_BALANCE_MOD_MONSTER_ROMEO.moves.MOCK.bearAlive");
    private static readonly LocString MockBearDeadLine =
      L10NMonsterLookup("STS2_BALANCE_MOD_MONSTER_ROMEO.moves.MOCK.bearDead");
    private static readonly LocString DeathReactLine =
      L10NMonsterLookup("STS2_BALANCE_MOD_MONSTER_ROMEO.deathReactLine");

    private const string CROSS_SLASH = "CROSS_SLASH";
    private const string MOCK = "MOCK";
    private const string AGONIZING_SLASH = "AGONIZING_SLASH";
    protected override string AttackSfx => "event:/sfx/enemy/enemy_attacks/gremlin_merc/sneaky_gremlin_attack";

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();

        var combatState = Creature.CombatState;
        if (combatState == null)
        {
            return;
        }

        var bear = combatState.GetTeammatesOf(Creature)
          .FirstOrDefault(t => t.Monster is Bear);
        if (bear != null)
        {
            bear.Died += BearDeathResponse;
        }
    }

    private void BearDeathResponse(Creature deadCreature)
    {
        deadCreature.Died -= BearDeathResponse;
        if (Creature.IsDead)
        {
            return;
        }

        TalkCmd.Play(DeathReactLine, Creature, VfxColor.Red, VfxDuration.Long);
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var mockState = new MoveState(MOCK, Mock, [new UnknownIntent()]);
        var crossSlashState = new MoveState(CROSS_SLASH, CrossSlash, [new SingleAttackIntent(CrossSlashDamage)]);
        var agonizingSlashState = new MoveState(AGONIZING_SLASH, AgonizingSlash, [new SingleAttackIntent(AgonizeDamage), new DebuffIntent()]);
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

    private Task Mock(IReadOnlyList<Creature> targets)
    {
        var bearAlive = Creature.CombatState?.GetTeammatesOf(Creature)
          .Any(t => t != Creature && t.IsAlive && t.Monster is Bear) == true;
        var line = bearAlive ? MockBearAliveLine : MockBearDeadLine;
        TalkCmd.Play(line, Creature, VfxColor.Red, VfxDuration.Long);
        return Task.CompletedTask;
    }

    private async Task CrossSlash(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(CrossSlashDamage)
            .FromMonster(this)
            .WithAttackerAnim("Attack", 0.4f)
            .WithAttackerFx(null, AttackSfx)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(null);
    }

    private async Task AgonizingSlash(IReadOnlyList<Creature> targets)
    {
        await DamageCmd.Attack(AgonizeDamage)
            .FromMonster(this)
            .WithAttackerAnim("Attack", 0.4f)
            .WithAttackerFx(null, AttackSfx)
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
        animator.AddAnyState("Attack", attack);
        animator.AddAnyState("Stab", attack);
        animator.AddAnyState(MOCK, attack);
        animator.AddAnyState(CROSS_SLASH, attack);
        animator.AddAnyState(AGONIZING_SLASH, attack);
        animator.AddAnyState("Hit", hit);
        controller.GetAnimationState().SetTimeScale(0.8f);

        return animator;
    }
}
