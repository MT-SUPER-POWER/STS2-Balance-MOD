using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Screens.Bestiary;
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
  private const string _closeUp = "CLOSE_UP";
  private const string _fierceBash = "FIERCE_BASH";
  private const string _rollAttack = "ROLL_ATTACK";
  private const string _twinSlam = "TWIN_SLAM";
  private const string _whirlwind = "WHIRLWIND";
  private const string _chargeUp = "CHARGE_UP";
  private const string _ventSteam = "VENT_STEAM";

  private const int _whirlwindDamage = 5;
  private const int _whirlwindCount = 4;
  private const int _twinSlamDamage = 8;
  private const int _twinSlamHits = 2;
  private const int _defensiveBlock = 20;
  private const int _chargeUpBlock = 9;
  private const int _ventDebuffAmount = 2;
  private const int _damageThresholdIncrease = 10;

  private static readonly LocString _destroyDialog =
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

  private static int FierceBashDamage =>
    AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 36, 32);

  private static int RollDamage =>
    AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 9);

  private static int SharpHideThorns =>
    AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);

  private static int DamageThresholdBase =>
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
    MoveState chargeUpState = new(
      _chargeUp,
      ChargeUpMove,
      [new DefendIntent()]);
    MoveState fierceBashState = new(
      _fierceBash,
      FierceBashMove,
      [new SingleAttackIntent(FierceBashDamage)]);
    MoveState ventSteamState = new(
      _ventSteam,
      VentSteamMove,
      [new DebuffIntent()]);
    MoveState whirlwindState = new(
      _whirlwind,
      WhirlwindMove,
      [new MultiAttackIntent(_whirlwindDamage, _whirlwindCount)]);
    _closeUpState = new MoveState(
      _closeUp,
      CloseUpMove,
      [new BuffIntent()]);
    MoveState rollAttackState = new(
      _rollAttack,
      RollAttackMove,
      [new SingleAttackIntent(RollDamage)]);
    MoveState twinSlamState = new(
      _twinSlam,
      TwinSlamMove,
      [new MultiAttackIntent(_twinSlamDamage, _twinSlamHits), new BuffIntent()]);

    RngConditionalBranchState offensiveBranch = new("OFFENSIVE_BRANCH", SelectNextOffensiveMove);

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
      return _closeUp;

    string? lastMove = stateMachine.StateLog.LastOrDefault(state => state is MoveState)?.Id;
    return lastMove switch
    {
      _chargeUp => _fierceBash,
      _fierceBash => _ventSteam,
      _ventSteam => _whirlwind,
      _twinSlam => _whirlwind,
      _whirlwind => _chargeUp,
      _ => _chargeUp,
    };
  }

  private static NCreature? GetCreatureNode(Creature creature)
  {
    return NCombatRoom.Instance?.GetCreatureNode(creature)
        ?? NBestiary.Instance?.GetCreatureNode(creature);
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
    if (!_isOpen)
      await TransitionToOffensiveMode();

    await CreatureCmd.GainBlock(Creature, _chargeUpBlock, ValueProp.Move, null);
    AFTPModAudio.Play("guardian", "guardian_destroy");
    TalkCmd.Play(_destroyDialog, Creature, VfxColor.Gold, VfxDuration.VeryLong);
    await CheckPendingModeShift();
  }

  private async Task FierceBashMove(IReadOnlyList<Creature> targets)
  {
    if (!_isOpen)
      await TransitionToOffensiveMode();

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
    if (!_isOpen)
      await TransitionToOffensiveMode();

    foreach (Creature target in targets.Where(target => target.IsAlive))
    {
      await PowerCmd.Apply<WeakPower>(
        new ThrowingPlayerChoiceContext(), target, _ventDebuffAmount, Creature, null);
      await PowerCmd.Apply<VulnerablePower>(
        new ThrowingPlayerChoiceContext(), target, _ventDebuffAmount, Creature, null);
    }

    await CheckPendingModeShift();
  }

  private async Task WhirlwindMove(IReadOnlyList<Creature> targets)
  {
    if (!_isOpen)
      await TransitionToOffensiveMode();

    _isExecutingMove = true;
    await FastAttackAnimation.Play(Creature);
    AFTPModAudio.Play("general", "whirlwind");

    for (int i = 0; i < _whirlwindCount; i++)
    {
      AFTPModAudio.Play("general", "attack_heavy");

      Creature? target = targets.FirstOrDefault(candidate => candidate.IsAlive);
      NCreature? targetNode = target == null ? null : GetCreatureNode(target);
      if (targetNode != null)
      {
        var cleaveVfx = CleaveEffect.Create(targetNode.VfxSpawnPosition);
        Node? container = NCombatRoom.Instance?.CombatVfxContainer
            ?? NBestiary.Instance?.GetNodeOrNull<Control>("%MonsterVisualsContainer")
            ?? targetNode.GetParent();
        container?.AddChildSafely(cleaveVfx.Root);
      }

      await Cmd.Wait(0.15f);
      await DamageCmd.Attack(_whirlwindDamage)
        .FromMonster(this)
        .Execute(null);
    }

    _isExecutingMove = false;
    await CheckPendingModeShift();
  }

  private async Task CloseUpMove(IReadOnlyList<Creature> targets)
  {
    if (_isOpen)
      await TransitionToDefensiveMode(setMove: false);

    await PowerCmd.Apply<SharpHidePower>(
      new ThrowingPlayerChoiceContext(), Creature, SharpHideThorns, Creature, null);
  }

  private async Task RollAttackMove(IReadOnlyList<Creature> targets)
  {
    if (_isOpen)
      await TransitionToDefensiveMode(setMove: false);

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
    if (!_isOpen)
      await TransitionToOffensiveMode();

    await FastAttackAnimation.Play(Creature);
    await DamageCmd.Attack(_twinSlamDamage)
      .WithHitCount(_twinSlamHits)
      .FromMonster(this)
      .WithAttackerFx(
        sfx: "event:/sfx/enemy/enemy_attacks/punch_construct/punch_construct_attack_double")
      .WithHitFx("vfx/vfx_attack_blunt")
      .Execute(null);

    if (Creature.CombatState is CombatState)
      await PowerCmd.Remove<SharpHidePower>(Creature);

    _isExecutingMove = false;
    await CheckPendingModeShift();
  }

  public async Task TransitionToDefensiveMode(bool setMove = true)
  {
    NCreature? creatureNode = GetCreatureNode(Creature);
    if (creatureNode != null)
    {
      var vfx = IntenseZoomEffect.Create(creatureNode.VfxSpawnPosition, false);
      Node? container = NCombatRoom.Instance?.CombatVfxContainer
          ?? NBestiary.Instance?.GetNodeOrNull<Control>("%MonsterVisualsContainer")
          ?? creatureNode.GetParent();
      container?.AddChildSafely(vfx.Root);
    }

    if (Creature.CombatState is CombatState)
    {
      await PowerCmd.Remove<ModeShiftPower>(Creature);
      _nextThreshold += _damageThresholdIncrease;
      await CreatureCmd.GainBlock(Creature, _defensiveBlock, ValueProp.Move, null);
    }

    AFTPModAudio.Play("guardian", "guardian_boss_transform");
    await CreatureCmd.TriggerAnim(Creature, "transition", 0f);

    MegaSprite? spineBody = creatureNode?.Visuals.SpineBody;
    if (spineBody != null)
    {
      MegaAnimationState animationState = spineBody.GetAnimationState();
      MegaTrackEntry? trackEntry = animationState.GetCurrent(0);
      if (trackEntry != null)
      {
        trackEntry.SetTimeScale(2f);
        await Cmd.Wait(trackEntry.GetAnimationEnd() / 2f);
      }

      animationState.SetAnimation("defensive", true, 0);
    }

    _isOpen = false;
    if (setMove && Creature.CombatState is CombatState)
      SetMoveImmediate(_closeUpState, true);
  }

  private async Task TransitionToOffensiveMode()
  {
    if (Creature.CombatState is CombatState)
    {
      await PowerCmd.Apply<ModeShiftPower>(
        new ThrowingPlayerChoiceContext(), Creature, _nextThreshold, Creature, null);

      if (Creature.Block > 0)
      {
        await CreatureCmd.LoseBlock(
          new ThrowingPlayerChoiceContext(), Creature, Creature.Block, null);
      }
    }

    await CreatureCmd.TriggerAnim(Creature, "idle", 0f);

    NCreature? creatureNode = GetCreatureNode(Creature);
    MegaSprite? spineBody = creatureNode?.Visuals.SpineBody;
    if (spineBody != null)
    {
      MegaAnimationState animState = spineBody.GetAnimationState();
      animState.SetAnimation("idle", true, 0);
      MegaTrackEntry? trackEntry = animState.GetCurrent(0);
      trackEntry?.SetMixDuration(0.2f);
    }

    _isOpen = true;
    _closeUpTriggered = false;
  }

  public override async Task BeforeDeath(Creature creature)
  {
    if (creature == Creature)
    {
      SharpHidePower? sharpHide = Creature.GetPower<SharpHidePower>();
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
