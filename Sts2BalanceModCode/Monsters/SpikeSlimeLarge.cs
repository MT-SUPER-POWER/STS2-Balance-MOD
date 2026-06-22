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
/// 分裂后产生的尖刺大型史莱姆。参考 ActsFromThePast 的 SpikeSlimeLarge。
/// </summary>
public sealed class SpikeSlimeLarge : Sts2MonsterModel
{
  private const string FlameTackle = "FLAME_TACKLE";
  private const string Lick = "LICK";

  protected override string VisualsPath =>
    "res://Sts2BalanceMod/monsters/slime_boss/slime_boss.tscn";

  public override int MinInitialHp => 70;
  public override int MaxInitialHp => 70;

  private const int FlameTackleDamage = 16;
  private const int SlimedCount = 2;
  private const int FrailTurns = 2;

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    var flameTackle = new MoveState(
      FlameTackle, FlameTackleMove,
      new AbstractIntent[] { new SingleAttackIntent(FlameTackleDamage), new StatusIntent(SlimedCount) });

    var lick = new MoveState(
      Lick, LickMove,
      new AbstractIntent[] { new DebuffIntent() });

    flameTackle.FollowUpState = lick;
    lick.FollowUpState = flameTackle;

    return new MonsterMoveStateMachine([flameTackle, lick], flameTackle);
  }

  private async Task FlameTackleMove(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    await DamageCmd.Attack(FlameTackleDamage)
      .FromMonster(this)
      .WithHitFx("vfx/vfx_slime_impact")
      .Execute(null);
    await CardPileCmd.AddToCombatAndPreview<Slimed>(targets, PileType.Discard, SlimedCount, (Player?)null);
  }

  private async Task LickMove(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    foreach (var target in targets.Where(t => t.IsAlive))
    {
      await PowerCmd.Apply<FrailPower>(new ThrowingPlayerChoiceContext(), target, FrailTurns, Creature, null);
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
