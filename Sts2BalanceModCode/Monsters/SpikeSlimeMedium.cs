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
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Animations;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Audio;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Combat;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// AFP-BOSS-03 — 尖刺史莱姆（中），由大型尖刺史莱姆分裂产生。
/// </summary>
[RegisterMonster]
public sealed class SpikeSlimeMedium : BalanceMonsterTemplate
{
    private const string FlameTackleMove = "FLAME_TACKLE";
    private const string LickMove = "LICK";
    private const int FrailTurns = 1;
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

    private int FlameTackleDamage =>
      AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 8);

    public override MonsterAssetProfile AssetProfile => new(
      ModAssetPaths.Resource("monsters", "spike_slime_medium", "spike_slime_medium.tscn"));

    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        Creature.Died += OnDeath;
    }

    private void OnDeath(Creature _)
    {
        Creature.Died -= OnDeath;
        NAudioManager.Instance?.PlayOneShot("event:/sfx/enemy/enemy_attacks/leaf_slime_m/leaf_slime_m_die");
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var flameTackleState = new MoveState(
          FlameTackleMove,
          FlameTackle,
          [new SingleAttackIntent(FlameTackleDamage), new StatusIntent(SlimedCount)]);
        var lickState = new MoveState(
          LickMove,
          Lick,
          [new DebuffIntent()]);
        var moveBranch = new RngConditionalBranchState("MOVE_BRANCH", SelectNextMove);

        flameTackleState.FollowUpState = moveBranch;
        lickState.FollowUpState = moveBranch;

        return new MonsterMoveStateMachine([flameTackleState, lickState, moveBranch], moveBranch);
    }

    private static string SelectNextMove(Creature owner, Rng rng, MonsterMoveStateMachine stateMachine)
    {
        var roll = rng.NextInt(100);
        if (roll < 30)
        {
            return LastTwoMoves(stateMachine, FlameTackleMove) ? LickMove : FlameTackleMove;
        }

        return LastMove(stateMachine, LickMove) ? FlameTackleMove : LickMove;
    }

    private async Task FlameTackle(IReadOnlyList<Creature> targets)
    {
        await FastAttackAnimation.Play(Creature);
        await DamageCmd.Attack(FlameTackleDamage)
          .FromMonster(this)
          .WithAttackerFx(sfx: "event:/sfx/enemy/enemy_attacks/leaf_slime_m/leaf_slime_m_attack")
          .WithHitFx("vfx/vfx_slime_impact")
          .Execute(null);

        // NOTE: AFP 默认关闭“一代黏液牌”兼容项，因此直接生成 STS2 当前 Slimed。
        await CardPileCmd.AddToCombatAndPreview<Slimed>(targets, PileType.Discard, SlimedCount, (Player?)null);
    }

    private async Task Lick(IReadOnlyList<Creature> targets)
    {
        await FastAttackAnimation.Play(Creature);
        foreach (var target in targets.Where(target => target.IsAlive))
        {
            await PowerCmd.Apply<FrailPower>(new ThrowingPlayerChoiceContext(), target, FrailTurns, Creature, null);
        }
    }

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        var idle = new AnimState("idle", true);
        var hit = new AnimState("hit") { NextState = idle };
        var animator = new CreatureAnimator(idle, controller);
        animator.AddAnyState("Hit", hit);
        return animator;
    }
}
