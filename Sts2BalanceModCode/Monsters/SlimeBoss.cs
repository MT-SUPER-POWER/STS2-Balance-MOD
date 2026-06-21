using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Audio;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Animations;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// STS1-BOSS - 史莱姆老大的心灵绽放事件用轻量模型。
/// 输入：心灵绽放战斗中的玩家目标。
/// 输出：执行黏液喷吐、蓄力和重击循环。
/// </summary>
public sealed class SlimeBoss : Sts2MonsterModel
{
  private const string GoopSpray = "GOOP_SPRAY";
  private const string PrepSlam = "PREP_SLAM";
  private const string Slam = "SLAM";

  protected override string VisualsPath =>
    "res://Sts2BalanceMod/monsters/slime_boss/slime_boss.tscn";

  public override bool HasDeathSfx => false;

  public override DamageSfxType TakeDamageSfxType => DamageSfxType.Slime;

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 150, 140);

  public override int MaxInitialHp => MinInitialHp;

  private int SlamDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 38, 35);

  private int SlimedCount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 5, 3);

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    var goopSpray = new MoveState(GoopSpray, GoopSprayMove, new AbstractIntent[] { new StatusIntent(SlimedCount) });
    var prepSlam = new MoveState(PrepSlam, PrepSlamMove, new AbstractIntent[] { new UnknownIntent() });
    var slam = new MoveState(Slam, SlamMove, new AbstractIntent[] { new SingleAttackIntent(SlamDamage) });

    goopSpray.FollowUpState = prepSlam;
    prepSlam.FollowUpState = slam;
    slam.FollowUpState = goopSpray;

    return new MonsterMoveStateMachine([goopSpray, prepSlam, slam], goopSpray);
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
