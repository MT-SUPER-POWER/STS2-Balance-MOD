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
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Animations;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Audio;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Combat;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Effects;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// AFP-BOSS-01 - 守护者怪物模型。
/// 输入：受到的未格挡伤害、当前形态与行动序列。
/// 输出：在进攻形态执行固定循环，达到 Mode Shift 阈值后切换为防御形态，再通过双重猛击返回进攻形态。
/// </summary>
[RegisterMonster]
public sealed class Guardian : MindBloomBossMonsterModel
{
    private const string CloseUp = "CLOSE_UP";
    private const string FierceBash = "FIERCE_BASH";
    private const string RollAttack = "ROLL_ATTACK";
    private const string TwinSlam = "TWIN_SLAM";
    private const string Whirlwind = "WHIRLWIND";
    private const string ChargeUp = "CHARGE_UP";
    private const string VentSteam = "VENT_STEAM";

    private const int WhirlwindDamage = 5;
    private const int WhirlwindCount = 4;
    private const int TwinSlamDamage = 8;
    private const int TwinSlamHits = 2;
    private const int DefensiveBlock = 20;
    private const int ChargeUpBlock = 9;
    private const int VentDebuffAmount = 2;
    private const int DamageThresholdIncrease = 10;

    private static readonly LocString DestroyDialog =
      L10NMonsterLookup("STS2_BALANCE_MOD_MONSTER_GUARDIAN.moves.CHARGE_UP.dialog");

    private int _nextThreshold;
    private bool _isOpen = true;
    private bool _closeUpTriggered;
    private bool _pendingModeShift;
    private bool _isExecutingMove;
    private MoveState _closeUpState = null!;

    public override MonsterAssetProfile AssetProfile => new(
      ModAssetPaths.Resource("monsters", "guardian", "guardian.tscn"));

    public override int MinInitialHp =>
      AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 250, 240);

    public override int MaxInitialHp => MinInitialHp;

    private int FierceBashDamage =>
      AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 36, 32);

    private int RollDamage =>
      AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 9);

    private int SharpHideThorns =>
      AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);

    private int DamageThresholdBase =>
      AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 40, 30);

    public bool IsOpen => _isOpen;

    public bool IsExecutingMove => _isExecutingMove;

    public bool CloseUpTriggered
    {
        get => _closeUpTriggered;
        set
        {
            AssertMutable();
            _closeUpTriggered = value;
        }
    }

    public bool PendingModeShift
    {
        get => _pendingModeShift;
        set
        {
            AssertMutable();
            _pendingModeShift = value;
        }
    }

    public override async Task AfterAddedToRoom()
    {
        await base.AfterAddedToRoom();
        _nextThreshold = DamageThresholdBase;
        await PowerCmd.Apply<ModeShiftPower>(
          new ThrowingPlayerChoiceContext(), Creature, _nextThreshold, Creature, null);
        await ApplyMindBloomEnhancements();
    }

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        var chargeUpState = new MoveState(
          ChargeUp,
          ChargeUpMove,
          [new DefendIntent()]);
        var fierceBashState = new MoveState(
          FierceBash,
          FierceBashMove,
          [new SingleAttackIntent(FierceBashDamage)]);
        var ventSteamState = new MoveState(
          VentSteam,
          VentSteamMove,
          [new DebuffIntent()]);
        var whirlwindState = new MoveState(
          Whirlwind,
          WhirlwindMove,
          [new MultiAttackIntent(WhirlwindDamage, WhirlwindCount)]);
        _closeUpState = new MoveState(
          CloseUp,
          CloseUpMove,
          [new BuffIntent()]);
        var rollAttackState = new MoveState(
          RollAttack,
          RollAttackMove,
          [new SingleAttackIntent(RollDamage)]);
        var twinSlamState = new MoveState(
          TwinSlam,
          TwinSlamMove,
          [new MultiAttackIntent(TwinSlamDamage, TwinSlamHits), new BuffIntent()]);

        var offensiveBranch = new RngConditionalBranchState("OFFENSIVE_BRANCH", SelectNextOffensiveMove);

        chargeUpState.FollowUpState = offensiveBranch;
        fierceBashState.FollowUpState = offensiveBranch;
        ventSteamState.FollowUpState = offensiveBranch;
        whirlwindState.FollowUpState = offensiveBranch;
        twinSlamState.FollowUpState = offensiveBranch;

        _closeUpState.FollowUpState = rollAttackState;
        rollAttackState.FollowUpState = twinSlamState;

        return new MonsterMoveStateMachine(
          [
            chargeUpState,
        fierceBashState,
        ventSteamState,
        whirlwindState,
        _closeUpState,
        rollAttackState,
        twinSlamState,
        offensiveBranch,
      ],
          chargeUpState);
    }

    private string SelectNextOffensiveMove(Creature owner, Rng rng, MonsterMoveStateMachine stateMachine)
    {
        if (!_isOpen)
            return CloseUp;

        var lastMove = stateMachine.StateLog.LastOrDefault(state => state is MoveState)?.Id;
        return lastMove switch
        {
            ChargeUp => FierceBash,
            FierceBash => VentSteam,
            VentSteam => Whirlwind,
            TwinSlam => Whirlwind,
            Whirlwind => ChargeUp,
            _ => ChargeUp,
        };
    }

    private async Task CheckPendingModeShift()
    {
        if (!_pendingModeShift)
            return;

        _pendingModeShift = false;
        CloseUpTriggered = true;
        await TransitionToDefensiveMode(setMove: false);
    }

    private async Task ChargeUpMove(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.GainBlock(Creature, ChargeUpBlock, ValueProp.Move, null);
        AFTPModAudio.Play("guardian", "guardian_destroy");
        TalkCmd.Play(DestroyDialog, Creature, VfxColor.Gold, VfxDuration.VeryLong);
        await CheckPendingModeShift();
    }

    private async Task FierceBashMove(IReadOnlyList<Creature> targets)
    {
        _isExecutingMove = true;
        await FastAttackAnimation.Play(Creature);
        await DamageCmd.Attack(FierceBashDamage)
          .FromMonster(this)
          .WithHitFx("vfx/vfx_attack_blunt", tmpSfx: "blunt_attack.mp3")
          .Execute(null);
        _isExecutingMove = false;
        await CheckPendingModeShift();
    }

    private async Task VentSteamMove(IReadOnlyList<Creature> targets)
    {
        foreach (var target in targets.Where(target => target.IsAlive))
        {
            await PowerCmd.Apply<WeakPower>(
              new ThrowingPlayerChoiceContext(), target, VentDebuffAmount, Creature, null);
            await PowerCmd.Apply<VulnerablePower>(
              new ThrowingPlayerChoiceContext(), target, VentDebuffAmount, Creature, null);
        }

        await CheckPendingModeShift();
    }

    private async Task WhirlwindMove(IReadOnlyList<Creature> targets)
    {
        _isExecutingMove = true;
        await FastAttackAnimation.Play(Creature);
        AFTPModAudio.Play("general", "whirlwind");

        for (var i = 0; i < WhirlwindCount; i++)
        {
            AFTPModAudio.Play("general", "attack_heavy");

            var target = targets.FirstOrDefault(candidate => candidate.IsAlive);
            var targetNode = target == null ? null : NCombatRoom.Instance?.GetCreatureNode(target);
            if (targetNode != null)
            {
                var cleaveVfx = CleaveEffect.Create(targetNode.VfxSpawnPosition);
                NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(cleaveVfx.Root);
            }

            await Cmd.Wait(0.15f);
            await DamageCmd.Attack(WhirlwindDamage)
              .FromMonster(this)
              .Execute(null);
        }

        _isExecutingMove = false;
        await CheckPendingModeShift();
    }

    private async Task CloseUpMove(IReadOnlyList<Creature> targets)
    {
        await PowerCmd.Apply<SharpHidePower>(
          new ThrowingPlayerChoiceContext(), Creature, SharpHideThorns, Creature, null);
    }

    private async Task RollAttackMove(IReadOnlyList<Creature> targets)
    {
        await FastAttackAnimation.Play(Creature);
        await DamageCmd.Attack(RollDamage)
          .FromMonster(this)
          .WithAttackerFx(
            sfx: "event:/sfx/enemy/enemy_attacks/punch_construct/punch_construct_attack_single")
          .WithHitFx("vfx/vfx_attack_blunt")
          .Execute(null);
    }

    private async Task TwinSlamMove(IReadOnlyList<Creature> targets)
    {
        _isExecutingMove = true;
        await TransitionToOffensiveMode();
        await DamageCmd.Attack(TwinSlamDamage)
          .WithHitCount(TwinSlamHits)
          .FromMonster(this)
          .WithAttackerFx(
            sfx: "event:/sfx/enemy/enemy_attacks/punch_construct/punch_construct_attack_double")
          .WithHitFx("vfx/vfx_attack_blunt")
          .Execute(null);
        await PowerCmd.Remove<SharpHidePower>(Creature);
        _isExecutingMove = false;
        await CheckPendingModeShift();
    }

    public async Task TransitionToDefensiveMode(bool setMove = true)
    {
        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(Creature);
        if (creatureNode != null)
        {
            var vfx = IntenseZoomEffect.Create(creatureNode.VfxSpawnPosition, false);
            NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(vfx.Root);
        }

        await PowerCmd.Remove<ModeShiftPower>(Creature);
        _nextThreshold += DamageThresholdIncrease;
        AFTPModAudio.Play("guardian", "guardian_boss_transform");
        await CreatureCmd.GainBlock(Creature, DefensiveBlock, ValueProp.Move, null);
        await CreatureCmd.TriggerAnim(Creature, "transition", 0f);

        var spineBody = creatureNode?.Visuals.SpineBody;
        if (spineBody != null)
        {
            var animationState = spineBody.GetAnimationState();
            var trackEntry = animationState.GetCurrent(0);
            if (trackEntry != null)
            {
                trackEntry.SetTimeScale(2f);
                await Cmd.Wait(trackEntry.GetAnimationEnd() / 2f);
            }

            animationState.SetAnimation("defensive", true, 0);
        }

        _isOpen = false;
        if (setMove)
            SetMoveImmediate(_closeUpState, true);
    }

    private async Task TransitionToOffensiveMode()
    {
        await PowerCmd.Apply<ModeShiftPower>(
          new ThrowingPlayerChoiceContext(), Creature, _nextThreshold, Creature, null);

        if (Creature.Block > 0)
        {
            await CreatureCmd.LoseBlock(
              new ThrowingPlayerChoiceContext(), Creature, Creature.Block, null);
        }

        await CreatureCmd.TriggerAnim(Creature, "idle", 0f);

        var creatureNode = NCombatRoom.Instance?.GetCreatureNode(Creature);
        var trackEntry = creatureNode?.Visuals.SpineBody?.GetAnimationState().GetCurrent(0);
        trackEntry?.SetMixDuration(0.2f);

        _isOpen = true;
        _closeUpTriggered = false;
    }

    public override async Task BeforeDeath(Creature creature)
    {
        if (creature == Creature)
        {
            var sharpHide = Creature.GetPower<SharpHidePower>();
            if (sharpHide is { AttackInProgress: true, AttackSource.IsAlive: true })
            {
                await CreatureCmd.Damage(
                  new ThrowingPlayerChoiceContext(),
                  sharpHide.AttackSource,
                  sharpHide.Amount,
                  ValueProp.Unpowered,
                  null,
                  null);
            }
        }

        await base.BeforeDeath(creature);
    }

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        var idle = new AnimState("idle", true)
        {
            BoundsContainer = "IdleBounds",
        };
        var defensive = new AnimState("defensive", true)
        {
            BoundsContainer = "DefensiveBounds",
        };
        var transition = new AnimState("transition")
        {
            NextState = defensive,
        };

        idle.AddBranch("transition", transition);
        idle.AddBranch("defensive", defensive);
        defensive.AddBranch("idle", idle);

        return new CreatureAnimator(idle, controller);
    }
}
