using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Combat;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// 乐嘉维林 (Lagavulin)。
/// 经典一代第一幕精英，初始处于金属化沉睡状态。惊醒后交替进行重击与灵魂吸取（-1/-2 力量与敏捷）。
/// </summary>
[RegisterMonster]
public sealed class Lagavulin : BalanceMonsterTemplate
{
  public const string SLEEP = "SLEEP";
  public const string ATTACK = "ATTACK";
  public const string DEBUFF = "DEBUFF";

  public override MonsterAssetProfile AssetProfile => new(
    ModAssetPaths.Resource("monsters", "lagavulin", "lagavulin.tscn"));

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 112, 109);
  public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 115, 111);

  private static int AttackDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 20, 18);
  private static int DebuffAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, -2, -1);
  private const int MetalAmount = 8;
  private const int AsleepTurns = 3;

  private bool _isAwake;
  private int _debuffTurnCount;
  private int _sleepTurnCount;
  private bool _startsAwake;

  public bool IsAwake
  {
    get => _isAwake;
    set
    {
      AssertMutable();
      _isAwake = value;
    }
  }

  public int DebuffTurnCount
  {
    get => _debuffTurnCount;
    set
    {
      AssertMutable();
      _debuffTurnCount = value;
    }
  }

  public int SleepTurnCount
  {
    get => _sleepTurnCount;
    set
    {
      AssertMutable();
      _sleepTurnCount = value;
    }
  }

  public bool StartsAwake
  {
    get => _startsAwake;
    set
    {
      AssertMutable();
      _startsAwake = value;
    }
  }

  public override async Task AfterAddedToRoom()
  {
    await base.AfterAddedToRoom();

    if (StartsAwake)
    {
      IsAwake = true;
      ApplyAwakeBounds();
    }
    else
    {
      await PowerCmd.Apply<MetallicizePower>(new ThrowingPlayerChoiceContext(), Creature, MetalAmount, Creature, null);
      await PowerCmd.Apply<AsleepLagavulinPower>(new ThrowingPlayerChoiceContext(), Creature, AsleepTurns, Creature, null);
    }
  }

  public async Task WakeUpFromDamage()
  {
    TalkCmd.Play(L10NMonsterLookup("STS2_BALANCE_MOD_MONSTER_LAGAVULIN.dialog.WAKE"), Creature, VfxColor.White, VfxDuration.Long);
    await CreatureCmd.TriggerAnim(Creature, "Wake", 0.6f);
    IsAwake = true;
    ApplyAwakeBounds();
    await CreatureCmd.Stun(Creature, StunnedMove, ATTACK);
  }

  public async Task WakeUpNaturally()
  {
    TalkCmd.Play(L10NMonsterLookup("STS2_BALANCE_MOD_MONSTER_LAGAVULIN.dialog.WAKE"), Creature, VfxColor.White, VfxDuration.Long);
    await CreatureCmd.TriggerAnim(Creature, "Wake", 0.6f);
    IsAwake = true;
    ApplyAwakeBounds();
  }

  private void ApplyAwakeBounds()
  {
    var creatureNode = NCombatRoom.Instance?.GetCreatureNode(Creature);
    if (creatureNode == null)
      return;

    var visuals = creatureNode.GetNodeOrNull<Node>("Lagavulin");
    if (visuals == null)
      return;

    var rootBounds = visuals.GetNodeOrNull<Control>("Bounds");
    var awakeBounds = visuals.GetNodeOrNull<Control>("AwakeBounds/Bounds");
    if (rootBounds != null && awakeBounds != null)
    {
      rootBounds.Position = awakeBounds.Position;
      rootBounds.Size = awakeBounds.Size;
    }

    var rootCenter = visuals.GetNodeOrNull<Marker2D>("CenterPos");
    var awakeCenter = visuals.GetNodeOrNull<Marker2D>("AwakeBounds/CenterPos");
    if (rootCenter != null && awakeCenter != null)
    {
      rootCenter.Position = awakeCenter.Position;
    }

    var rootIntent = visuals.GetNodeOrNull<Marker2D>("IntentPos");
    var awakeIntent = visuals.GetNodeOrNull<Marker2D>("AwakeBounds/IntentPos");
    if (rootIntent != null && awakeIntent != null)
    {
      rootIntent.Position = awakeIntent.Position;
    }

    var hitbox = creatureNode.GetNodeOrNull<Control>("Hitbox");
    if (hitbox != null && awakeBounds != null)
    {
      hitbox.Position = awakeBounds.Position;
      hitbox.Size = awakeBounds.Size;
    }
  }

  public Task StunnedMove(IReadOnlyList<Creature> targets) => Task.CompletedTask;

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    List<MonsterState> states = [];

    MoveState sleepState = new(
      SLEEP,
      SleepMove,
      [new SleepIntent()]
    );

    MoveState attackState = new(
      ATTACK,
      AttackMove,
      [new SingleAttackIntent(AttackDamage)]
    );

    MoveState debuffState = new(
      DEBUFF,
      DebuffMove,
      [new DebuffIntent()]
    );

    RngConditionalBranchState mainBranch = new("MAIN_BRANCH", SelectNextMove);

    sleepState.FollowUpState = mainBranch;
    attackState.FollowUpState = mainBranch;
    debuffState.FollowUpState = mainBranch;

    states.Add(sleepState);
    states.Add(attackState);
    states.Add(debuffState);
    states.Add(mainBranch);

    return new MonsterMoveStateMachine(states, mainBranch);
  }

  private string SelectNextMove(Creature owner, Rng rng, MonsterMoveStateMachine stateMachine)
  {
    if (!IsAwake && Creature.HasPower<AsleepLagavulinPower>())
    {
      return SLEEP;
    }

    if (StartsAwake && stateMachine.StateLog.Count == 0)
    {
      return DEBUFF;
    }

    if (DebuffTurnCount >= 2)
    {
      return DEBUFF;
    }

    if (LastTwoMoves(stateMachine, ATTACK))
    {
      return DEBUFF;
    }

    return ATTACK;
  }

  private Task SleepMove(IReadOnlyList<Creature> targets)
  {
    SleepTurnCount++;
    string dialogKey = SleepTurnCount switch
    {
      1 => "STS2_BALANCE_MOD_MONSTER_LAGAVULIN.dialog.SLEEP_1",
      2 => "STS2_BALANCE_MOD_MONSTER_LAGAVULIN.dialog.SLEEP_2",
      _ => ""
    };

    if (!string.IsNullOrEmpty(dialogKey))
    {
      TalkCmd.Play(L10NMonsterLookup(dialogKey), Creature, VfxColor.White, VfxDuration.Long);
    }

    return Task.CompletedTask;
  }

  private async Task AttackMove(IReadOnlyList<Creature> targets)
  {
    DebuffTurnCount++;

    await DamageCmd.Attack(AttackDamage)
      .FromMonster(this)
      .WithAttackerAnim("Attack", 0.3f)
      .WithHitFx("vfx/vfx_attack_blunt")
      .Execute(null);
  }

  private async Task DebuffMove(IReadOnlyList<Creature> targets)
  {
    DebuffTurnCount = 0;

    await CreatureCmd.TriggerAnim(Creature, "Debuff", 0.0f);
    await Cmd.Wait(0.3f);

    foreach (Creature target in targets.Where(t => t.IsAlive))
    {
      await PowerCmd.Apply<DexterityPower>(new ThrowingPlayerChoiceContext(), target, DebuffAmount, Creature, null);
      await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), target, DebuffAmount, Creature, null);
    }
  }

  public override CreatureAnimator GenerateAnimator(MegaSprite controller)
  {
    AnimState sleepLoop = new("Idle_1", true)
    {
      BoundsContainer = "SleepingBounds"
    };

    AnimState hurtSleeping = new("Idle_1", true);
    AnimState wakeUp = new("Coming_out");

    AnimState idleLoop = new("Idle_2", true)
    {
      BoundsContainer = "AwakeBounds"
    };

    AnimState attack = new("Attack");
    AnimState debuff = new("Debuff");
    AnimState hurt = new("Hit");

    hurtSleeping.NextState = sleepLoop;
    wakeUp.NextState = idleLoop;
    attack.NextState = idleLoop;
    debuff.NextState = idleLoop;
    hurt.NextState = idleLoop;

    AnimState initialState = StartsAwake ? idleLoop : sleepLoop;
    CreatureAnimator animator = new(initialState, controller);

    animator.AddAnyState("Sleep", sleepLoop);
    animator.AddAnyState("Wake", wakeUp, () => !IsAwake);
    animator.AddAnyState("Idle_2", idleLoop);
    animator.AddAnyState("Attack", attack);
    animator.AddAnyState("Debuff", debuff);
    animator.AddAnyState("Hit", hurt, () => IsAwake);
    animator.AddAnyState("Hit", hurtSleeping, () => !IsAwake);

    return animator;
  }
}
