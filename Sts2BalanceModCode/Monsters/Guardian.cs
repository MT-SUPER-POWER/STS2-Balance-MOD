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
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Animations;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// STS1-BOSS - 守护者的心灵绽放事件用轻量模型。
/// 输入：心灵绽放战斗中的玩家目标。
/// 输出：按一层 Boss 的节奏执行格挡、攻击与易伤/虚弱减益。
/// </summary>
public sealed class Guardian : Sts2MonsterModel
{
  private const string ChargeUp = "CHARGE_UP";
  private const string FierceBash = "FIERCE_BASH";
  private const string VentSteam = "VENT_STEAM";
  private const string Whirlwind = "WHIRLWIND";

  protected override string VisualsPath =>
    "res://Assets/ActsFromPast/ActsFromThePast/monsters/guardian/guardian.tscn";

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 250, 240);

  public override int MaxInitialHp => MinInitialHp;

  private int FierceBashDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 36, 32);

  private const int ChargeBlock = 9;
  private const int WhirlwindDamage = 5;
  private const int WhirlwindHits = 4;
  private const int DebuffTurns = 2;

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    var chargeUp = new MoveState(ChargeUp, ChargeUpMove, new AbstractIntent[] { new DefendIntent() });
    var fierceBash = new MoveState(FierceBash, FierceBashMove, new AbstractIntent[] { new SingleAttackIntent(FierceBashDamage) });
    var ventSteam = new MoveState(VentSteam, VentSteamMove, new AbstractIntent[] { new DebuffIntent() });
    var whirlwind = new MoveState(Whirlwind, WhirlwindMove, new AbstractIntent[] { new MultiAttackIntent(WhirlwindDamage, WhirlwindHits) });

    chargeUp.FollowUpState = fierceBash;
    fierceBash.FollowUpState = ventSteam;
    ventSteam.FollowUpState = whirlwind;
    whirlwind.FollowUpState = chargeUp;

    return new MonsterMoveStateMachine([chargeUp, fierceBash, ventSteam, whirlwind], chargeUp);
  }

  private Task ChargeUpMove(IReadOnlyList<Creature> targets) =>
    CreatureCmd.GainBlock(Creature, ChargeBlock, ValueProp.Move, null);

  private async Task FierceBashMove(IReadOnlyList<Creature> targets)
  {
    await DamageCmd.Attack(FierceBashDamage)
      .FromMonster(this)
      .WithAttackerAnim("Attack", 0.4f)
      .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
      .Execute(null);
  }

  private async Task VentSteamMove(IReadOnlyList<Creature> targets)
  {
    foreach (var target in targets.Where(t => t.IsAlive))
    {
      await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, DebuffTurns, Creature, null);
      await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), target, DebuffTurns, Creature, null);
    }
  }

  private async Task WhirlwindMove(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    await DamageCmd.Attack(WhirlwindDamage)
      .WithHitCount(WhirlwindHits)
      .FromMonster(this)
      .WithHitFx("vfx/vfx_attack_slash")
      .Execute(null);
  }

  public override CreatureAnimator GenerateAnimator(MegaSprite controller)
  {
    var idle = new AnimState("idle", true);
    var attack = new AnimState("attack");
    attack.NextState = idle;

    var animator = new CreatureAnimator(idle, controller);
    animator.AddAnyState("Attack", attack);
    return animator;
  }

  protected override string? GetBestiaryMoveAnimationId(string moveStateId)
  {
    return moveStateId switch
    {
      FierceBash or Whirlwind => "attack",
      _ => base.GetBestiaryMoveAnimationId(moveStateId),
    };
  }
}
