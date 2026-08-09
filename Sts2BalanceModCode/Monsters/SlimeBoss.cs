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
/// AFP-BOSS-03 — 史莱姆老大。
/// 循环执行黏液喷射、蓄力、重击，并在半血时强制分裂为一只大型尖刺史莱姆和一只大型酸液史莱姆。
/// </summary>
[RegisterMonster]
public sealed class SlimeBoss : MindBloomBossMonsterModel
{
    private const string _slamMove = "SLAM";
    private const string _prepSlamMove = "PREP_SLAM";
    private const string _splitMove = "SPLIT";
    private const string _goopSprayMove = "GOOP_SPRAY";

    private bool _splitTriggered;
    private MoveState _splitState = null!;

    public override int MinInitialHp =>
      AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 150, 140);

    public override int MaxInitialHp => MinInitialHp;

    private static int SlamDamage =>
      AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 38, 35);

    private static int SlimedCount =>
      AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 3);

    public override MonsterAssetProfile AssetProfile => new(
      ModAssetPaths.Resource("monsters", "slime_boss", "slime_boss.tscn"));

    public override bool HasDeathSfx => false;

    public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

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

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        await PowerCmd.Apply<SplitPower>(new ThrowingPlayerChoiceContext(), Creature, 1M, Creature, null);
        await ApplyMindBloomEnhancements();
        Creature.Died += OnDeath;
    }

    private void OnDeath(Creature _)
    {
        Creature.Died -= OnDeath;
        AFTPModAudio.Play("slime_boss", "slime_boss_death_1");
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var goopSprayState = new MoveState(
          _goopSprayMove,
          GoopSpray,
          [new StatusIntent(SlimedCount)]);
        var prepSlamState = new MoveState(
          _prepSlamMove,
          PrepSlam,
          [new UnknownIntent()]);
        var slamState = new MoveState(
          _slamMove,
          Slam,
          [new SingleAttackIntent(SlamDamage)]);
        _splitState = new MoveState(
          _splitMove,
          Split,
          [new UnknownIntent()]);
        var moveBranch = new RngConditionalBranchState("MOVE_BRANCH", SelectNextMove);

        goopSprayState.FollowUpState = prepSlamState;
        prepSlamState.FollowUpState = slamState;
        slamState.FollowUpState = moveBranch;
        _splitState.FollowUpState = _splitState;

        return new MonsterMoveStateMachine(
          [goopSprayState, prepSlamState, slamState, _splitState, moveBranch],
          goopSprayState);
    }

    private string SelectNextMove(Creature owner, Rng rng, MonsterMoveStateMachine stateMachine)
    {
        return SplitTriggered ? _splitMove : _goopSprayMove;
    }

    private async Task GoopSpray(IReadOnlyList<Creature> targets)
    {
        await FastAttackAnimation.Play(Creature);
        AFTPModAudio.Play("general", "slime_attack");

        // NOTE: AFP 默认关闭“一代黏液牌”兼容项，因此直接生成 STS2 当前 Slimed。
        await CardPileCmd.AddToCombatAndPreview<Slimed>(targets, PileType.Discard, SlimedCount, (Player?)null);
    }

    private async Task PrepSlam(IReadOnlyList<Creature> targets)
    {
        PlayPrepSfx();
        TalkCmd.Play(
          L10NMonsterLookup("STS2_BALANCE_MOD_MONSTER_SLIME_BOSS.moves.PREP_SLAM.banter"),
          Creature,
          VfxColor.Green,
          VfxDuration.Long);
        NGame.Instance?.ScreenShake(ShakeStrength.Weak, ShakeDuration.Long);
        await Cmd.Wait(0.3f);
    }

    private async Task Slam(IReadOnlyList<Creature> targets)
    {
        await JumpAnimation.Play(Creature);

        foreach (Creature target in targets.Where(target => target.IsAlive))
        {
            NCreature? creatureNode = target.GetCreatureNode();
            if (creatureNode == null)
                continue;

            Node2D vfx = PreloadManager.Cache.GetScene(SceneHelper.GetScenePath("vfx/vfx_heavy_blunt"))
              .Instantiate<Node2D>();
            vfx.Modulate = Colors.Green;
            target.GetVfxContainer()?.AddChildSafely(vfx);
            vfx.GlobalPosition = creatureNode.GlobalPosition;
        }

        await Cmd.Wait(0.4f);
        await DamageCmd.Attack(SlamDamage)
          .FromMonster(this)
          .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
          .Execute(null);
    }

    private async Task Split(IReadOnlyList<Creature> targets)
    {
        int currentHp = Creature.CurrentHp;
        ICombatState combatState = Creature.CombatState
          ?? throw new InvalidOperationException("Slime Boss split requires an active combat state.");
        Vector2 originalPosition = NCombatRoom.Instance?.GetCreatureNode(Creature)?.Position ?? Vector2.Zero;

        _ = ShakeAnimation.Play(Creature, 1f, 3f);
        await Cmd.Wait(1f);
        AFTPModAudio.Play("general", "slime_split");
        await CreatureCmd.Kill(Creature);

        var occupiedSlots = combatState.GetTeammatesOf(Creature)
          .Where(teammate => teammate.IsAlive)
          .Select(teammate => teammate.SlotName)
          .ToHashSet();
        string? spikeSlot = combatState.Encounter?.Slots?
          .FirstOrDefault(slot => slot.StartsWith("spike_large") && !occupiedSlots.Contains(slot));
        string? acidSlot = combatState.Encounter?.Slots?
          .FirstOrDefault(slot => slot.StartsWith("acid_large") && !occupiedSlots.Contains(slot));

        bool useEncounterSlots = spikeSlot != null && acidSlot != null;
        Queue<Vector2> positionQueue = new();
        Control? enemyContainer = NCombatRoom.Instance?.GetNode<Control>("%EnemyContainer");
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
            positionQueue.Enqueue(originalPosition + new Vector2(-385f, 20f));
        }

        try
        {
            var spikeSlime = (SpikeSlimeLarge)ModelDb.Monster<SpikeSlimeLarge>().ToMutable();
            spikeSlime.OverrideHp = currentHp;
            Creature spikeCreature = await CreatureCmd.Add(spikeSlime, combatState, CombatSide.Enemy, spikeSlot);
            await CreatureCmd.SetMaxHp(spikeCreature, currentHp);
            await CreatureCmd.Heal(spikeCreature, currentHp);

            if (!useEncounterSlots)
                positionQueue.Enqueue(originalPosition + new Vector2(120f, 20f));

            var acidSlime = (AcidSlimeLarge)ModelDb.Monster<AcidSlimeLarge>().ToMutable();
            acidSlime.OverrideHp = currentHp;
            Creature acidCreature = await CreatureCmd.Add(acidSlime, combatState, CombatSide.Enemy, acidSlot);
            await CreatureCmd.SetMaxHp(acidCreature, currentHp);
            await CreatureCmd.Heal(acidCreature, currentHp);
        }
        finally
        {
            if (childEnteredCallable.HasValue)
                enemyContainer?.Disconnect(Node.SignalName.ChildEnteredTree, childEnteredCallable.Value);
        }
    }

    private static void PlayPrepSfx()
    {
        string sfxName = Rng.Chaotic.NextInt(2) == 0
          ? "slime_boss_talk_1"
          : "slime_boss_talk_2";
        AFTPModAudio.Play("slime_boss", sfxName);
    }

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        return new CreatureAnimator(new AnimState("idle", true), controller);
    }
}
