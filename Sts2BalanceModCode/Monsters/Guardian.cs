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
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Animations;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// STS1-BOSS - 守护者的心灵绽放事件用轻量模型。
/// 输入：心灵绽放战斗中的玩家目标。
/// 输出：按一层 Boss 的节奏执行格挡、攻击与易伤/虚弱减益。
/// </summary>
public sealed class Guardian : Sts2MonsterModel
{
  private const string CloseUp = "CLOSE_UP";
  private const string ChargeUp = "CHARGE_UP";
  private const string FierceBash = "FIERCE_BASH";
  private const string RollAttack = "ROLL_ATTACK";
  private const string TwinSlam = "TWIN_SLAM";
  private const string VentSteam = "VENT_STEAM";
  private const string Whirlwind = "WHIRLWIND";

  protected override string VisualsPath =>
    "res://Sts2BalanceMod/monsters/guardian/guardian.tscn";

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 250, 240);

  public override int MaxInitialHp => MinInitialHp;

  private int FierceBashDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 36, 32);

  private int RollDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 10, 9);

  private int SharpHideThorns => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);

  private int DamageThresholdBase => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 40, 30);

  private const int ChargeBlock = 9;
  private const int DefensiveBlock = 20;
  private const int DamageThresholdIncrease = 10;
  private const int TwinSlamDamage = 8;
  private const int TwinSlamHits = 2;
  private const int WhirlwindDamage = 5;
  private const int WhirlwindHits = 4;
  private const int DebuffTurns = 2;
  private int _nextThreshold;
  private bool _isOpen = true;
  private bool _closeUpTriggered;
  private bool _pendingModeShift;
  private bool _isExecutingMove;
  private MoveState? _closeUpState;

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
    await PowerCmd.Apply<ModeShiftPower>(new ThrowingPlayerChoiceContext(), Creature, _nextThreshold, Creature, null);
  }

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    var chargeUp = new MoveState(ChargeUp, ChargeUpMove, new AbstractIntent[] { new DefendIntent() });
    var fierceBash = new MoveState(FierceBash, FierceBashMove, new AbstractIntent[] { new SingleAttackIntent(FierceBashDamage) });
    var ventSteam = new MoveState(VentSteam, VentSteamMove, new AbstractIntent[] { new DebuffIntent() });
    var whirlwind = new MoveState(Whirlwind, WhirlwindMove, new AbstractIntent[] { new MultiAttackIntent(WhirlwindDamage, WhirlwindHits) });
    _closeUpState = new MoveState(CloseUp, CloseUpMove, new AbstractIntent[] { new BuffIntent() });
    var rollAttack = new MoveState(RollAttack, RollAttackMove, new AbstractIntent[] { new SingleAttackIntent(RollDamage) });
    var twinSlam = new MoveState(TwinSlam, TwinSlamMove, new AbstractIntent[] { new MultiAttackIntent(TwinSlamDamage, TwinSlamHits), new BuffIntent() });

    MonsterMoveStateMachine? moveMachine = null;
    var offensiveBranch = new SelectorBranchState("OFFENSIVE_BRANCH", () =>
    {
      if (!IsOpen)
        return CloseUp;
      var last = moveMachine!.StateLog.LastOrDefault(state => state is MoveState)?.Id;
      return last switch
      {
        ChargeUp => FierceBash,
        FierceBash => VentSteam,
        VentSteam => Whirlwind,
        Whirlwind => ChargeUp,
        TwinSlam => Whirlwind,
        _ => ChargeUp,
      };
    });

    chargeUp.FollowUpState = offensiveBranch;
    fierceBash.FollowUpState = offensiveBranch;
    ventSteam.FollowUpState = offensiveBranch;
    whirlwind.FollowUpState = offensiveBranch;
    twinSlam.FollowUpState = offensiveBranch;
    _closeUpState.FollowUpState = rollAttack;
    rollAttack.FollowUpState = twinSlam;

    moveMachine = new MonsterMoveStateMachine(
      [chargeUp, fierceBash, ventSteam, whirlwind, _closeUpState, rollAttack, twinSlam, offensiveBranch],
      chargeUp);
    return moveMachine;
  }


  private async Task CheckPendingModeShift()
  {
    if (!PendingModeShift)
      return;

    PendingModeShift = false;
    CloseUpTriggered = true;
    await TransitionToDefensiveMode(setMove: false);
  }

  private async Task ChargeUpMove(IReadOnlyList<Creature> targets)
  {
    await CreatureCmd.GainBlock(Creature, ChargeBlock, ValueProp.Move, null);
    await CheckPendingModeShift();
  }

  private async Task FierceBashMove(IReadOnlyList<Creature> targets)
  {
    _isExecutingMove = true;
    await DamageCmd.Attack(FierceBashDamage)
      .FromMonster(this)
      .WithAttackerAnim("Attack", 0.4f)
      .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
      .Execute(null);
    _isExecutingMove = false;
    await CheckPendingModeShift();
  }

  private async Task VentSteamMove(IReadOnlyList<Creature> targets)
  {
    foreach (var target in targets.Where(t => t.IsAlive))
    {
      await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, DebuffTurns, Creature, null);
      await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), target, DebuffTurns, Creature, null);
    }

    await CheckPendingModeShift();
  }

  private async Task WhirlwindMove(IReadOnlyList<Creature> targets)
  {
    _isExecutingMove = true;
    await FastAttackAnimation.Play(Creature);
    await DamageCmd.Attack(WhirlwindDamage)
      .WithHitCount(WhirlwindHits)
      .FromMonster(this)
      .WithHitFx("vfx/vfx_attack_slash")
      .Execute(null);
    _isExecutingMove = false;
    await CheckPendingModeShift();
  }

  private Task CloseUpMove(IReadOnlyList<Creature> targets) =>
    PowerCmd.Apply<SharpHidePower>(new ThrowingPlayerChoiceContext(), Creature, SharpHideThorns, Creature, null);

  private async Task RollAttackMove(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    await DamageCmd.Attack(RollDamage)
      .FromMonster(this)
      .WithAttackerFx(sfx: AttackSfx)
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
      .WithAttackerFx(sfx: "event:/sfx/enemy/enemy_attacks/punch_construct/punch_construct_attack_double")
      .WithHitFx("vfx/vfx_attack_blunt")
      .Execute(null);
    await PowerCmd.Remove<SharpHidePower>(Creature);
    _isExecutingMove = false;
    await CheckPendingModeShift();
  }

  public async Task TransitionToDefensiveMode(bool setMove = true)
  {
    await PowerCmd.Remove<ModeShiftPower>(Creature);
    _nextThreshold += DamageThresholdIncrease;
    await CreatureCmd.GainBlock(Creature, DefensiveBlock, ValueProp.Move, null);
    await CreatureCmd.TriggerAnim(Creature, "transition", 0.0f);

    _isOpen = false;
    if (setMove && _closeUpState != null)
    {
      SetMoveImmediate(_closeUpState, true);
    }
  }

  private async Task TransitionToOffensiveMode()
  {
    await PowerCmd.Apply<ModeShiftPower>(new ThrowingPlayerChoiceContext(), Creature, _nextThreshold, Creature, null);

    if (Creature.Block > 0)
    {
      await CreatureCmd.LoseBlock(Creature, Creature.Block);
    }

    await CreatureCmd.TriggerAnim(Creature, "idle", 0.0f);
    _isOpen = true;
    CloseUpTriggered = false;
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
    var transition = new AnimState("transition");
    var attack = new AnimState("attack");

    transition.NextState = defensive;
    attack.NextState = idle;
    idle.AddBranch("transition", transition);
    idle.AddBranch("defensive", defensive);
    defensive.AddBranch("idle", idle);

    var animator = new CreatureAnimator(idle, controller);
    animator.AddAnyState("Attack", attack);
    return animator;
  }

  protected override string? GetBestiaryMoveAnimationId(string moveStateId)
  {
    return moveStateId switch
    {
      FierceBash or Whirlwind or RollAttack or TwinSlam => "attack",
      CloseUp => "transition",
      _ => base.GetBestiaryMoveAnimationId(moveStateId),
    };
  }
}
