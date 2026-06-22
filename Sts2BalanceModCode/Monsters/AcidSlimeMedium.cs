using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
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

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// 分裂后产生的酸液中号史莱姆。参考 ActsFromThePast 的 AcidSlimeMedium。
/// </summary>
public sealed class AcidSlimeMedium : Sts2MonsterModel
{
  private const string CorrosiveSpit = "CORROSIVE_SPIT";
  private const string Tackle = "TACKLE";
  private const string Lick = "LICK";

  protected override string VisualsPath =>
    "res://Sts2BalanceMod/monsters/acid_slime_medium/acid_slime_medium.tscn";

  public override int MinInitialHp => 30;
  public override int MaxInitialHp => 34;

  private const int CorrosiveSpitDamage = 8;
  private const int TackleDamage = 12;
  private const int SlimedCount = 1;
  private const int WeakTurns = 1;

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

    corrosiveSpit.FollowUpState = tackle;
    tackle.FollowUpState = lick;
    lick.FollowUpState = corrosiveSpit;

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
    var damage = new AnimState("damage");
    damage.NextState = idle;
    var animator = new CreatureAnimator(idle, controller);
    animator.AddAnyState("Hit", damage);
    return animator;
  }
}
