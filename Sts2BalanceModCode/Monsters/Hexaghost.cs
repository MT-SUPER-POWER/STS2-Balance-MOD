using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Animations;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// STS1-BOSS - 六火亡魂的心灵绽放事件用轻量模型。
/// 输入：玩家当前生命值与战斗目标。
/// 输出：执行 Divider、灼烧、强化与多段攻击循环。
/// </summary>
public sealed class Hexaghost : Sts2MonsterModel
{
  private const string Activate = "ACTIVATE";
  private const string Divider = "DIVIDER";
  private const string Sear = "SEAR";
  private const string Tackle = "TACKLE";
  private const string Inflame = "INFLAME";
  private const string Inferno = "INFERNO";

  private int _dividerDamage;

  protected override string VisualsPath =>
    "res://Sts2BalanceMod/monsters/hexaghost/hexaghost.tscn";

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 264, 250);

  public override int MaxInitialHp => MinInitialHp;

  private int InfernoDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

  private int TackleDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);

  private int StrengthAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

  private int BurnCount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 2, 1);

  private const int SearDamage = 6;
  private const int TackleHits = 2;
  private const int InfernoHits = 6;
  private const int InflameBlock = 12;

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    var activate = new MoveState(Activate, ActivateMove, new AbstractIntent[] { new UnknownIntent() });
    var divider = new MoveState(Divider, DividerMove, new AbstractIntent[] { new MultiAttackIntent(1, 6) });
    var sear1 = new MoveState($"{Sear}_1", SearMove, new AbstractIntent[] { new SingleAttackIntent(SearDamage), new StatusIntent(BurnCount) });
    var tackle1 = new MoveState($"{Tackle}_1", TackleMove, new AbstractIntent[] { new MultiAttackIntent(TackleDamage, TackleHits) });
    var sear2 = new MoveState($"{Sear}_2", SearMove, new AbstractIntent[] { new SingleAttackIntent(SearDamage), new StatusIntent(BurnCount) });
    var inflame = new MoveState(Inflame, InflameMove, new AbstractIntent[] { new DefendIntent(), new BuffIntent() });
    var tackle2 = new MoveState($"{Tackle}_2", TackleMove, new AbstractIntent[] { new MultiAttackIntent(TackleDamage, TackleHits) });
    var sear3 = new MoveState($"{Sear}_3", SearMove, new AbstractIntent[] { new SingleAttackIntent(SearDamage), new StatusIntent(BurnCount) });
    var inferno = new MoveState(Inferno, InfernoMove, new AbstractIntent[] { new MultiAttackIntent(InfernoDamage, InfernoHits), new StatusIntent(3) });

    activate.FollowUpState = divider;
    divider.FollowUpState = sear1;
    sear1.FollowUpState = tackle1;
    tackle1.FollowUpState = sear2;
    sear2.FollowUpState = inflame;
    inflame.FollowUpState = tackle2;
    tackle2.FollowUpState = sear3;
    sear3.FollowUpState = inferno;
    inferno.FollowUpState = sear1;

    return new MonsterMoveStateMachine([activate, divider, sear1, tackle1, sear2, inflame, tackle2, sear3, inferno], activate);
  }

  private Task ActivateMove(IReadOnlyList<Creature> targets)
  {
    var livingTargets = targets.Where(t => t.IsAlive).ToList();
    var averageHp = livingTargets.Count > 0 ? livingTargets.Average(t => t.CurrentHp) : 1.0;
    _dividerDamage = (int)(averageHp / 12.0) + 1;
    return Task.CompletedTask;
  }

  private async Task DividerMove(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    await DamageCmd.Attack(_dividerDamage)
      .WithHitCount(6)
      .FromMonster(this)
      .WithAttackerFx(sfx: "event:/sfx/characters/attack_fire")
      .WithHitFx("scenes/vfx/vfx_fire_burst.tscn")
      .Execute(null);
  }

  private async Task SearMove(IReadOnlyList<Creature> targets)
  {
    await DamageCmd.Attack(SearDamage)
      .FromMonster(this)
      .WithAttackerFx(sfx: "event:/sfx/characters/attack_fire")
      .WithHitFx("scenes/vfx/vfx_fire_burst.tscn")
      .Execute(null);
    await CardPileCmd.AddToCombatAndPreview<Burn>(targets, PileType.Discard, BurnCount, (Player?)null);
  }

  private async Task TackleMove(IReadOnlyList<Creature> targets)
  {
    await FastAttackAnimation.Play(Creature);
    await DamageCmd.Attack(TackleDamage)
      .WithHitCount(TackleHits)
      .FromMonster(this)
      .WithAttackerFx(sfx: "event:/sfx/characters/attack_fire")
      .WithHitFx("scenes/vfx/vfx_fire_burst.tscn")
      .Execute(null);
  }

  private async Task InflameMove(IReadOnlyList<Creature> targets)
  {
    await CreatureCmd.GainBlock(Creature, InflameBlock, ValueProp.Move, null);
    await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Creature, StrengthAmount, Creature, null);
  }

  private async Task InfernoMove(IReadOnlyList<Creature> targets)
  {
    await DamageCmd.Attack(InfernoDamage)
      .WithHitCount(InfernoHits)
      .FromMonster(this)
      .WithAttackerFx(sfx: "event:/sfx/characters/attack_fire")
      .WithHitFx("scenes/vfx/vfx_fire_burst.tscn")
      .Execute(null);
    await CardPileCmd.AddToCombatAndPreview<Burn>(targets, PileType.Discard, 3, (Player?)null);
  }

  public override CreatureAnimator GenerateAnimator(MegaSprite controller)
  {
    var idle = new AnimState("idle_loop", true);
    return new CreatureAnimator(idle, controller);
  }
}
