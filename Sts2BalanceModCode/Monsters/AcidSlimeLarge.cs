using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Animations;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// 酸液大型史莱姆 — 由史莱姆老大分裂产生，半血后再分裂为两个酸液中号史莱姆。
/// 参考 ActsFromThePast 的 AcidSlimeLarge。
/// </summary>
public sealed class AcidSlimeLarge : Sts2MonsterModel
{
  private const string CorrosiveSpit = "CORROSIVE_SPIT";
  private const string Tackle = "TACKLE";
  private const string Lick = "LICK";
  private const string Split = "SPLIT";

  protected override string VisualsPath =>
    "res://Sts2BalanceMod/monsters/acid_slime_large/acid_slime_large.tscn";

  public override int MinInitialHp => 70;
  public override int MaxInitialHp => 70;

  private const int CorrosiveSpitDamage = 11;
  private const int TackleDamage = 16;
  private const int SlimedCount = 2;
  private const int WeakTurns = 2;

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
    var corrosiveSpit = new MoveState(
      CorrosiveSpit, CorrosiveSpitMove,
      new AbstractIntent[] { new SingleAttackIntent(CorrosiveSpitDamage), new StatusIntent(SlimedCount) });

    var tackle = new MoveState(
      Tackle, TackleMove,
      new AbstractIntent[] { new SingleAttackIntent(TackleDamage) });

    var lick = new MoveState(
      Lick, LickMove,
      new AbstractIntent[] { new DebuffIntent() });

    _splitState = new MoveState(Split, SplitMove, new AbstractIntent[] { new UnknownIntent() });

    corrosiveSpit.FollowUpState = lick;
    lick.FollowUpState = tackle;
    tackle.FollowUpState = corrosiveSpit;
    _splitState.FollowUpState = _splitState;

    return new MonsterMoveStateMachine([corrosiveSpit, tackle, lick, _splitState], corrosiveSpit);
  }

  /// <summary>
  /// 分裂动作：杀死当前酸液大史莱姆，生成两个酸液中号史莱姆。
  /// </summary>
  private async Task SplitMove(IReadOnlyList<Creature> targets)
  {
    var currentHp = Creature.CurrentHp;
    var combatState = Creature.CombatState;

    // 分裂动画
    await FastAttackAnimation.Play(Creature);
    await Cmd.Wait(0.6f);

    await CreatureCmd.Kill(Creature);

    // NOTE: 等待本体消失后再生成分裂个体
    await Cmd.Wait(0.8f);

    // 查找可用槽位
    var occupiedSlots = combatState.GetTeammatesOf(Creature)
        .Where(t => t.IsAlive)
        .Select(t => t.SlotName)
        .ToHashSet();

    var slot1 = combatState.Encounter.Slots?
        .FirstOrDefault(s => s.StartsWith("acid_med") && !occupiedSlots.Contains(s));

    string? slot2 = null;
    if (slot1 != null)
    {
      occupiedSlots.Add(slot1);
      slot2 = combatState.Encounter.Slots?
          .FirstOrDefault(s => s.StartsWith("acid_med") && !occupiedSlots.Contains(s));
    }

    var useSlots = slot1 != null && slot2 != null;

    // 生成第一个酸液中号史莱姆
    var slime1 = ModelDb.Monster<AcidSlimeMedium>().ToMutable();
    var creature1 = await CreatureCmd.Add(slime1, combatState, CombatSide.Enemy, slot1);
    await CreatureCmd.SetMaxHp(creature1, currentHp);
    await CreatureCmd.Heal(creature1, currentHp);

    // 生成第二个酸液中号史莱姆
    var slime2 = ModelDb.Monster<AcidSlimeMedium>().ToMutable();
    var creature2 = await CreatureCmd.Add(slime2, combatState, CombatSide.Enemy, slot2);
    await CreatureCmd.SetMaxHp(creature2, currentHp);
    await CreatureCmd.Heal(creature2, currentHp);
  }

  private async Task CorrosiveSpitMove(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    await DamageCmd.Attack(CorrosiveSpitDamage)
      .FromMonster(this)
      .WithHitFx("vfx/vfx_slime_impact")
      .Execute(null);
    await CardPileCmd.AddToCombatAndPreview<Slimed>(targets, PileType.Discard, SlimedCount, (Player?)null);
  }

  private async Task TackleMove(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    await DamageCmd.Attack(TackleDamage)
      .FromMonster(this)
      .WithHitFx("vfx/vfx_slime_impact")
      .Execute(null);
  }

  private async Task LickMove(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    foreach (var target in targets.Where(t => t.IsAlive))
    {
      await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, WeakTurns, Creature, null);
    }
  }

  // NOTE: 动画名参考 ActsFromThePast AcidSlimeLarge，Spine 骨骼使用 "Idle"（大写 I）和 "damage"
  public override CreatureAnimator GenerateAnimator(MegaSprite controller)
  {
    var idle = new AnimState("Idle", true);
    var damage = new AnimState("damage");
    damage.NextState = idle;
    var animator = new CreatureAnimator(idle, controller);
    animator.AddAnyState("Hit", damage);
    return animator;
  }
}
