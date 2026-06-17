using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// STS1-BOSS-01 — 时间吞噬者怪物模型。
/// 输入：玩家出牌与怪物回合。
/// 输出：执行攻击/减益/格挡强化，并通过 TimeWarpPower 限制每回合出牌数。
/// </summary>
public sealed class TimeEater : Sts2MonsterModel
{
  private const decimal HealThreshold = 0.5M;
  private bool _hasHealed;

  protected override string VisualsPath => "res://Assets/ActsFromThePast/ActsFromThePast/monsters/time_eater/time_eater.tscn";

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 480, 456);

  public override int MaxInitialHp => MinInitialHp;

  private int ReverberateDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 8, 7);

  private int ReverberateHits => 3;

  private int HeadSlamDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 32, 26);

  private int RippleBlock => 20;

  private int RippleStrength => 2;

  private int HasteStrength => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 4, 3);

  public override async Task AfterAddedToRoom()
  {
    await base.AfterAddedToRoom();
    await PowerCmd.Apply<TimeWarpPower>(new ThrowingPlayerChoiceContext(), Creature, 12M, Creature, null);
  }

  public override async Task AfterCurrentHpChanged(Creature creature, decimal delta)
  {
    if (creature != Creature || delta >= 0M || _hasHealed)
      return;

    if (Creature.CurrentHp > Creature.MaxHp * HealThreshold)
      return;

    _hasHealed = true;
    await CreatureCmd.Heal(Creature, Creature.MaxHp / 2M);
    await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Creature, HasteStrength, Creature, null);
  }

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    var reverberate = new MoveState("REVERBERATE_MOVE", ReverberateMove,
      new MultiAttackIntent(ReverberateDamage, ReverberateHits));
    var ripple = new MoveState("RIPPLE_MOVE", RippleMove, new DefendIntent(), new BuffIntent());
    var headSlam = new MoveState("HEAD_SLAM_MOVE", HeadSlamMove,
      new SingleAttackIntent(HeadSlamDamage), new DebuffIntent());

    reverberate.FollowUpState = ripple;
    ripple.FollowUpState = headSlam;
    headSlam.FollowUpState = reverberate;

    return new MonsterMoveStateMachine([reverberate, ripple, headSlam], reverberate);
  }

  private async Task ReverberateMove(IReadOnlyList<Creature> targets)
  {
    await DamageCmd.Attack(ReverberateDamage).WithHitCount(ReverberateHits).FromMonster(this)
      .WithAttackerAnim("Attack", 0.4f)
      .WithHitFx("vfx/vfx_attack_blunt")
      .Execute(null);
  }

  private async Task RippleMove(IReadOnlyList<Creature> targets)
  {
    await CreatureCmd.GainBlock(Creature, RippleBlock, ValueProp.Move, null);
    await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Creature, RippleStrength, Creature, null);
  }

  private async Task HeadSlamMove(IReadOnlyList<Creature> targets)
  {
    await DamageCmd.Attack(HeadSlamDamage).FromMonster(this)
      .WithAttackerAnim("Attack", 0.5f)
      .WithHitFx("vfx/vfx_heavy_blunt")
      .Execute(null);
    await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), targets, 1M, Creature, null);
  }
}
