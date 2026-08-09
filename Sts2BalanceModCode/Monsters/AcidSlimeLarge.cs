using Godot;
using MegaCrit.Sts2.Core.Animation;
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
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Random;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Animations;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Audio;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Combat;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// AFP-BOSS-03 — 酸液史莱姆（大）。半血时强制分裂为两只继承当前生命的中型酸液史莱姆。
/// </summary>
[RegisterMonster]
public sealed class AcidSlimeLarge : BalanceMonsterTemplate
{
  private const string CorrosiveSpitMove = "CORROSIVE_SPIT";
  private const string TackleMove = "TACKLE";
  private const string LickMove = "LICK";
  private const string SplitMove = "SPLIT";
  private const int WeakTurns = 2;
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
    OverrideHp ?? AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 68, 65);

  public override int MaxInitialHp =>
    OverrideHp ?? AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 72, 69);

  private int CorrosiveSpitDamage =>
    AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 12, 11);

  private int TackleDamage =>
    AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 18, 16);

  public override MonsterAssetProfile AssetProfile => new(
    ModAssetPaths.Resource("monsters", "acid_slime_large", "acid_slime_large.tscn"));

  public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

  public override async Task AfterAddedToRoom()
  {
    await base.AfterAddedToRoom();
    await PowerCmd.Apply<SplitPower>(new ThrowingPlayerChoiceContext(), Creature, 1M, Creature, null);
  }

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    var corrosiveSpitState = new MoveState(
      CorrosiveSpitMove,
      CorrosiveSpit,
      [new SingleAttackIntent(CorrosiveSpitDamage), new StatusIntent(SlimedCount)]);
    var tackleState = new MoveState(
      TackleMove,
      Tackle,
      [new SingleAttackIntent(TackleDamage)]);
    var lickState = new MoveState(
      LickMove,
      Lick,
      [new DebuffIntent()]);
    _splitState = new MoveState(
      SplitMove,
      Split,
      [new UnknownIntent()]);
    var moveBranch = new RngConditionalBranchState("MOVE_BRANCH", SelectNextMove);

    corrosiveSpitState.FollowUpState = moveBranch;
    tackleState.FollowUpState = moveBranch;
    lickState.FollowUpState = moveBranch;
    _splitState.FollowUpState = _splitState;

    return new MonsterMoveStateMachine(
      [corrosiveSpitState, tackleState, lickState, _splitState, moveBranch],
      moveBranch);
  }

  private string SelectNextMove(Creature owner, Rng rng, MonsterMoveStateMachine stateMachine)
  {
    if (SplitTriggered)
      return SplitMove;

    var roll = rng.NextInt(100);
    if (roll < 40)
    {
      return LastTwoMoves(stateMachine, CorrosiveSpitMove)
        ? rng.NextFloat() < 0.6f ? TackleMove : LickMove
        : CorrosiveSpitMove;
    }

    if (roll < 70)
    {
      return LastTwoMoves(stateMachine, TackleMove)
        ? rng.NextFloat() < 0.6f ? CorrosiveSpitMove : LickMove
        : TackleMove;
    }

    return LastMove(stateMachine, LickMove)
      ? rng.NextFloat() < 0.4f ? CorrosiveSpitMove : TackleMove
      : LickMove;
  }

  private static bool LastMove(MonsterMoveStateMachine stateMachine, string moveId)
  {
    return stateMachine.StateLog.Count > 0 && stateMachine.StateLog[^1].Id == moveId;
  }

  private static bool LastTwoMoves(MonsterMoveStateMachine stateMachine, string moveId)
  {
    var log = stateMachine.StateLog;
    return log.Count >= 2 && log[^1].Id == moveId && log[^2].Id == moveId;
  }

  private async Task CorrosiveSpit(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    await DamageCmd.Attack(CorrosiveSpitDamage)
      .FromMonster(this)
      .WithAttackerFx(sfx: "event:/sfx/enemy/enemy_attacks/twig_slime_s/twig_slime_s_attack")
      .WithHitFx("vfx/vfx_slime_impact")
      .Execute(null);

    // NOTE: AFP 默认关闭“一代黏液牌”兼容项，因此直接生成 STS2 当前 Slimed。
    await CardPileCmd.AddToCombatAndPreview<Slimed>(targets, PileType.Discard, SlimedCount, (Player?)null);
  }

  private async Task Tackle(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    await DamageCmd.Attack(TackleDamage)
      .FromMonster(this)
      .WithAttackerFx(sfx: "event:/sfx/enemy/enemy_attacks/twig_slime_s/twig_slime_s_attack")
      .WithHitFx("vfx/vfx_slime_impact")
      .Execute(null);
  }

  private async Task Lick(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    foreach (var target in targets.Where(target => target.IsAlive))
    {
      await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, WeakTurns, Creature, null);
    }
  }

  private async Task Split(IReadOnlyList<Creature> targets)
  {
    var currentHp = Creature.CurrentHp;
    var combatState = Creature.CombatState
      ?? throw new InvalidOperationException("Acid Slime split requires an active combat state.");
    var originalPosition = NCombatRoom.Instance?.GetCreatureNode(Creature)?.Position ?? Vector2.Zero;

    _ = ShakeAnimation.Play(Creature, 1f, 3f);
    await Cmd.Wait(1f);
    AFTPModAudio.Play("general", "slime_split");
    await CreatureCmd.Kill(Creature);

    var occupiedSlots = combatState.GetTeammatesOf(Creature)
      .Where(teammate => teammate.IsAlive)
      .Select(teammate => teammate.SlotName)
      .ToHashSet();
    var firstSlot = combatState.Encounter?.Slots?
      .FirstOrDefault(slot => slot.StartsWith("acid_med") && !occupiedSlots.Contains(slot));
    string? secondSlot = null;
    if (firstSlot != null)
    {
      occupiedSlots.Add(firstSlot);
      secondSlot = combatState.Encounter?.Slots?
        .FirstOrDefault(slot => slot.StartsWith("acid_med") && !occupiedSlots.Contains(slot));
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
      var firstSlime = (AcidSlimeMedium)ModelDb.Monster<AcidSlimeMedium>().ToMutable();
      var firstCreature = await CreatureCmd.Add(firstSlime, combatState, CombatSide.Enemy, firstSlot);
      await CreatureCmd.SetMaxHp(firstCreature, currentHp);
      await CreatureCmd.Heal(firstCreature, currentHp);

      if (!useEncounterSlots)
        positionQueue.Enqueue(originalPosition + new Vector2(134f, Rng.Chaotic.NextFloat() * 8f - 4f));

      var secondSlime = (AcidSlimeMedium)ModelDb.Monster<AcidSlimeMedium>().ToMutable();
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
    var idle = new AnimState("Idle", true);
    var damage = new AnimState("damage") { NextState = idle };
    var animator = new CreatureAnimator(idle, controller);
    animator.AddAnyState("Hit", damage);
    return animator;
  }
}
