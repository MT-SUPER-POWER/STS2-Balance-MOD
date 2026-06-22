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
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Animations;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// STS1-BOSS - 史莱姆老大的心灵绽放事件用轻量模型。
/// 输入：心灵绽放战斗中的玩家目标。
/// 输出：执行黏液喷吐、蓄力和重击循环，生命≤50%时分裂为尖刺史莱姆+酸液史莱姆。
/// </summary>
public sealed class SlimeBoss : Sts2MonsterModel
{
  private const string GoopSpray = "GOOP_SPRAY";
  private const string PrepSlam = "PREP_SLAM";
  private const string Slam = "SLAM";
  private const string Split = "SPLIT";

  protected override string VisualsPath =>
    "res://Sts2BalanceMod/monsters/slime_boss/slime_boss.tscn";

  public override bool HasDeathSfx => false;

  public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 150, 140);

  public override int MaxInitialHp => MinInitialHp;

  private int SlamDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 38, 35);

  private int SlimedCount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 3);

  private bool _splitTriggered;
  public bool SplitTriggered
  {
    get => _splitTriggered;
    set
    {
      AssertMutable();
      _splitTriggered = value;
    }
  }

  private MoveState _splitState = null!;
  public MoveState SplitState => _splitState;

  public override async Task AfterAddedToRoom()
  {
    await base.AfterAddedToRoom();
    await PowerCmd.Apply<SplitPower>(new ThrowingPlayerChoiceContext(), Creature, 1m, Creature, null);
  }

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    var goopSpray = new MoveState(GoopSpray, GoopSprayMove, new AbstractIntent[] { new StatusIntent(SlimedCount) });
    var prepSlam = new MoveState(PrepSlam, PrepSlamMove, new AbstractIntent[] { new UnknownIntent() });
    var slam = new MoveState(Slam, SlamMove, new AbstractIntent[] { new SingleAttackIntent(SlamDamage) });

    MonsterMoveStateMachine? moveMachine = null;
    _splitState = new MoveState(Split, SplitMove, new AbstractIntent[] { new UnknownIntent() });

    var moveBranch = new SelectorBranchState("MOVE_BRANCH", () =>
    {
      if (SplitTriggered)
        return Split;
      return GoopSpray;
    });

    goopSpray.FollowUpState = prepSlam;
    prepSlam.FollowUpState = slam;
    slam.FollowUpState = moveBranch;
    _splitState.FollowUpState = _splitState;

    moveMachine = new MonsterMoveStateMachine(
      [goopSpray, prepSlam, slam, _splitState, moveBranch], goopSpray);
    return moveMachine;
  }

  /// <summary>
  /// 分裂动作：杀死当前史莱姆老大，生成尖刺大型+酸液大型史莱姆。
  /// </summary>
  private async Task SplitMove(IReadOnlyList<Creature> targets)
  {
    var currentHp = Creature.CurrentHp;
    var combatState = Creature.CombatState;
    var originalPosition = NCombatRoom.Instance?.GetCreatureNode(Creature)?.Position ?? Vector2.Zero;

    // 分裂动画
    await FastAttackAnimation.Play(Creature);
    await Cmd.Wait(0.8f);

    await CreatureCmd.Kill(Creature);

    // NOTE: 等待 BOSS 完全消失后再生成分裂个体，避免三者同时出现
    await Cmd.Wait(1.0f);

    // 查找可用槽位
    var occupiedSlots = combatState.GetTeammatesOf(Creature)
        .Where(t => t.IsAlive)
        .Select(t => t.SlotName)
        .ToHashSet();

    var spikeSlot = combatState.Encounter.Slots?
        .FirstOrDefault(s => s.StartsWith("spike_large") && !occupiedSlots.Contains(s));
    var acidSlot = combatState.Encounter.Slots?
        .FirstOrDefault(s => s.StartsWith("acid_large") && !occupiedSlots.Contains(s));

    var useSlots = spikeSlot != null && acidSlot != null;

    Queue<Vector2>? positionQueue = null;
    var enemyContainer = NCombatRoom.Instance?.GetNode<Godot.Control>("%EnemyContainer");
    Callable? callable = null;

    if (!useSlots)
    {
      positionQueue = new Queue<Vector2>();

      void OnChildEntered(Node child)
      {
        if (child is NCreature nc && positionQueue.Count > 0)
          nc.Position = positionQueue.Dequeue();
      }

      callable = Callable.From<Node>(OnChildEntered);
      enemyContainer?.Connect(Node.SignalName.ChildEnteredTree, callable.Value);
      positionQueue.Enqueue(originalPosition + new Vector2(-385f, 20f));
    }

    // 生成尖刺史莱姆
    var spikeSlime = ModelDb.Monster<SpikeSlimeLarge>().ToMutable();
    var spikeCreature = await CreatureCmd.Add(spikeSlime, combatState, CombatSide.Enemy, spikeSlot);
    await CreatureCmd.SetMaxHp(spikeCreature, currentHp);
    await CreatureCmd.Heal(spikeCreature, currentHp);

    if (!useSlots)
      positionQueue!.Enqueue(originalPosition + new Vector2(120f, 20f));

    // 生成酸液史莱姆
    var acidSlime = ModelDb.Monster<AcidSlimeLarge>().ToMutable();
    var acidCreature = await CreatureCmd.Add(acidSlime, combatState, CombatSide.Enemy, acidSlot);
    await CreatureCmd.SetMaxHp(acidCreature, currentHp);
    await CreatureCmd.Heal(acidCreature, currentHp);

    if (!useSlots && callable.HasValue)
      enemyContainer?.Disconnect(Node.SignalName.ChildEnteredTree, callable.Value);
  }

  private async Task GoopSprayMove(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    await CardPileCmd.AddToCombatAndPreview<Slimed>(targets, PileType.Discard, SlimedCount, (Player?)null);
  }

  private Task PrepSlamMove(IReadOnlyList<Creature> targets) => Cmd.Wait(0.3f);

  private async Task SlamMove(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    await DamageCmd.Attack(SlamDamage)
      .FromMonster(this)
      .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
      .Execute(null);
  }

  public override CreatureAnimator GenerateAnimator(MegaSprite controller)
  {
    var idle = new AnimState("idle", true);
    return new CreatureAnimator(idle, controller);
  }
}
