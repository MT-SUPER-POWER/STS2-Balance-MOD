using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

// ======================== 红面具三人帮 — Pointy ========================

/// <summary>
/// STS1-EVENT — 红面具强盗 Pointy（尖头），快速二连刺。
/// </summary>
public sealed class Pointy : Sts2MonsterModel
{
  protected override string VisualsPath => "res://Sts2BalanceMod/monsters/pointy/pointy.tscn";

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 34, 30);
  public override int MaxInitialHp => MinInitialHp;

  private int AttackDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
  private const int AttackHits = 2;
  private static readonly LocString DeathReactLine =
    L10NMonsterLookup("STS2BALANCEMOD-POINTY.deathReactLine");

  private const string STAB = "STAB";

  public override async Task AfterAddedToRoom()
  {
    await base.AfterAddedToRoom();

    var combatState = Creature.CombatState;
    if (combatState == null)
    {
      return;
    }

    var bear = combatState.GetTeammatesOf(Creature)
      .FirstOrDefault(t => t.Monster is Bear);
    if (bear != null)
    {
      bear.Died += BearDeathResponse;
    }
  }

  private void BearDeathResponse(Creature deadCreature)
  {
    deadCreature.Died -= BearDeathResponse;
    if (Creature.IsDead)
    {
      return;
    }

    TalkCmd.Play(DeathReactLine, Creature, VfxColor.Red, VfxDuration.Long);
  }

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    var stabState = new MoveState(STAB, Stab, new AbstractIntent[] { new MultiAttackIntent(AttackDamage, AttackHits) });
    stabState.FollowUpState = stabState;
    return new MonsterMoveStateMachine([stabState], stabState);
  }

  private async Task Stab(IReadOnlyList<Creature> targets)
  {
    await CreatureCmd.TriggerAnim(Creature, "Slash", 0.0f);
    await Cmd.Wait(0.3f);

    for (int i = 0; i < AttackHits; i++)
    {
      await DamageCmd.Attack(AttackDamage)
          .FromMonster(this)
          .WithHitFx("vfx/vfx_attack_slash")
          .Execute(null);
    }
  }

  public override CreatureAnimator GenerateAnimator(MegaSprite controller)
  {
    var idle = new AnimState("Idle", true);
    var attack = new AnimState("Attack");
    var hit = new AnimState("Hit");

    attack.NextState = idle;
    hit.NextState = idle;

    var animator = new CreatureAnimator(idle, controller);
    animator.AddAnyState("Attack", attack);
    animator.AddAnyState("Slash", attack);
    animator.AddAnyState(STAB, attack);
    animator.AddAnyState("Hit", hit);

    return animator;
  }

  protected override string? GetBestiaryMoveAnimationId(string moveStateId)
  {
    return moveStateId == STAB ? "Attack" : base.GetBestiaryMoveAnimationId(moveStateId);
  }
}
