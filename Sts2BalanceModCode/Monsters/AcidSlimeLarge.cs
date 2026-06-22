using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Animations;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// 分裂后产生的酸液大型史莱姆。参考 ActsFromThePast 的 AcidSlimeLarge。
/// </summary>
public sealed class AcidSlimeLarge : Sts2MonsterModel
{
  private const string CorrosiveSpit = "CORROSIVE_SPIT";
  private const string Tackle = "TACKLE";
  private const string Lick = "LICK";

  protected override string VisualsPath =>
    "res://Sts2BalanceMod/monsters/slime_boss/slime_boss.tscn";

  public override int MinInitialHp => 70;
  public override int MaxInitialHp => 70;

  private const int CorrosiveSpitDamage = 11;
  private const int TackleDamage = 16;
  private const int SlimedCount = 2;
  private const int WeakTurns = 2;

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

    corrosiveSpit.FollowUpState = lick;
    lick.FollowUpState = tackle;
    tackle.FollowUpState = corrosiveSpit;

    return new MonsterMoveStateMachine([corrosiveSpit, tackle, lick], corrosiveSpit);
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

  public override CreatureAnimator GenerateAnimator(MegaSprite controller)
  {
    var idle = new AnimState("idle", true);
    var hit = new AnimState("hit");
    hit.NextState = idle;
    var animator = new CreatureAnimator(idle, controller);
    animator.AddAnyState("Hit", hit);
    return animator;
  }
}
