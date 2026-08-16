using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
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
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Random;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Animations;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Audio;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Combat;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// AFP-BOSS-03 — 尖刺史莱姆（大）。半血时强制分裂为两只继承当前生命的中型尖刺史莱姆。
/// </summary>
[RegisterMonster]
public sealed class SpikeSlimeLarge : BalanceMonsterTemplate
{
    private const string FlameTackleMove = "FLAME_TACKLE";
    private const string LickMove = "LICK";
    private const string SplitMove = "SPLIT";
    private const int SlimedCount = 2;

    private int? _overrideHp;
    private bool _splitTriggered;
    private MoveState _splitState = null!;

    public int? OverrideHp
    {
        get => _overrideHp;
        set
        {
            AssertMutable();
            _overrideHp = value;
        }
    }

    public bool SplitTriggered
    {
        get => _splitTriggered;
        set
        {
            AssertMutable();
            _splitTriggered = value;
        }
    }

    public MoveState SplitState => _splitState;

    public override int MinInitialHp =>
      OverrideHp ?? AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 67, 64);

    public override int MaxInitialHp =>
      OverrideHp ?? AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 73, 70);

    private int FlameTackleDamage =>
      AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 18, 16);

    private int FrailTurns =>
      AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

    public override MonsterAssetProfile AssetProfile => new(
      ModAssetPaths.Resource("monsters", "spike_slime_large", "spike_slime_large.tscn"));

    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<SplitPower>(new ThrowingPlayerChoiceContext(), Creature, 1M, Creature, null);
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
        _splitState = new MoveState(
          SplitMove,
          Split,
          [new UnknownIntent()]);
        var moveBranch = new RngConditionalBranchState("MOVE_BRANCH", SelectNextMove);

        flameTackleState.FollowUpState = moveBranch;
        lickState.FollowUpState = moveBranch;
        _splitState.FollowUpState = _splitState;

        return new MonsterMoveStateMachine(
          [flameTackleState, lickState, _splitState, moveBranch],
          moveBranch);
    }

    private string SelectNextMove(Creature owner, Rng rng, MonsterMoveStateMachine stateMachine)
    {
        if (SplitTriggered)
            return SplitMove;

        var roll = rng.NextInt(100);
        if (roll < 30)
            return LastTwoMoves(stateMachine, FlameTackleMove) ? LickMove : FlameTackleMove;

        return LastMove(stateMachine, LickMove) ? FlameTackleMove : LickMove;
    }

    private async Task FlameTackle(IReadOnlyList<Creature> targets)
    {
        await FastAttackAnimation.Play(Creature);
        await DamageCmd.Attack(FlameTackleDamage)
          .FromMonster(this)
          .WithAttackerFx(sfx: "event:/sfx/enemy/enemy_attacks/twig_slime_s/twig_slime_s_attack")
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

    private async Task Split(IReadOnlyList<Creature> targets)
    {
        _ = ShakeAnimation.Play(Creature, 1f, 3f);
        await Cmd.Wait(1f);
        AFTPModAudio.Play("general", "slime_split");
        NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Short);

        // NOTE: 在图鉴环境中仅播放分裂音效与蓄力动画，不执行实际生成与杀死
        if (Creature.CombatState is not CombatState combatState)
            return;

        var currentHp = Creature.CurrentHp;
        var originalCreatureNode = NCombatRoom.Instance?.GetCreatureNode(Creature);
        var originalPosition = originalCreatureNode?.Position ?? Vector2.Zero;

        // 立即生成粘液爆裂特效并隐藏大史莱姆的视觉节点，避免死亡消散残留与分裂生成的两只史莱姆重叠显示
        if (originalCreatureNode != null)
        {
            var vfxScenePath = SceneHelper.GetScenePath("vfx/vfx_slime_impact");
            var vfx = PreloadManager.Cache.GetScene(vfxScenePath).Instantiate<Node2D>();
            vfx.GlobalPosition = originalCreatureNode.VfxSpawnPosition;
            vfx.Scale = new Vector2(1.4f, 1.4f);
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx);

            originalCreatureNode.Visuals.Visible = false;
            originalCreatureNode.Visible = false;
        }

        await CreatureCmd.Kill(Creature);

        var occupiedSlots = combatState.GetTeammatesOf(Creature)
          .Where(teammate => teammate.IsAlive)
          .Select(teammate => teammate.SlotName)
          .ToHashSet();
        var firstSlot = combatState.Encounter?.Slots?
          .FirstOrDefault(slot => slot.StartsWith("spike_med") && !occupiedSlots.Contains(slot));
        string? secondSlot = null;
        if (firstSlot != null)
        {
            occupiedSlots.Add(firstSlot);
            secondSlot = combatState.Encounter?.Slots?
              .FirstOrDefault(slot => slot.StartsWith("spike_med") && !occupiedSlots.Contains(slot));
        }

        var useEncounterSlots = firstSlot != null && secondSlot != null;
        var positionQueue = new Queue<Vector2>();
        var enemyContainer = NCombatRoom.Instance?.GetNode<Control>("%EnemyContainer");
        Callable? childEnteredCallable = null;

        if (!useEncounterSlots)
        {
            void OnChildEntered(Node child)
            {
                if (child is NCreature creatureNode && positionQueue.Count > 0)
                    creatureNode.Position = positionQueue.Dequeue();
            }

            childEnteredCallable = Callable.From<Node>(OnChildEntered);
            enemyContainer?.Connect(Node.SignalName.ChildEnteredTree, childEnteredCallable.Value);
            positionQueue.Enqueue(originalPosition + new Vector2(-134f, Rng.Chaotic.NextFloat() * 8f - 4f));
        }

        try
        {
            var firstSlime = (SpikeSlimeMedium)ModelDb.Monster<SpikeSlimeMedium>().ToMutable();
            var firstCreature = await CreatureCmd.Add(firstSlime, combatState, CombatSide.Enemy, firstSlot);
            await CreatureCmd.SetMaxHp(firstCreature, currentHp);
            await CreatureCmd.Heal(firstCreature, currentHp);

            if (!useEncounterSlots)
                positionQueue.Enqueue(originalPosition + new Vector2(134f, Rng.Chaotic.NextFloat() * 8f - 4f));

            var secondSlime = (SpikeSlimeMedium)ModelDb.Monster<SpikeSlimeMedium>().ToMutable();
            var secondCreature = await CreatureCmd.Add(secondSlime, combatState, CombatSide.Enemy, secondSlot);
            await CreatureCmd.SetMaxHp(secondCreature, currentHp);
            await CreatureCmd.Heal(secondCreature, currentHp);
        }
        finally
        {
            if (childEnteredCallable.HasValue)
                enemyContainer?.Disconnect(Node.SignalName.ChildEnteredTree, childEnteredCallable.Value);
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
