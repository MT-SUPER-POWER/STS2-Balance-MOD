using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

// ======================== 红面具三人帮 — Pointy ========================

/// <summary>
/// STS1-EVENT — 红面具强盗 Pointy（尖头），快速二连刺。
/// </summary>
public sealed class Pointy : Sts2MonsterModel
{
  protected override string VisualsPath => "res://Assets/ActsFromThePast/ActsFromThePast/monsters/pointy/pointy.tscn";

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 34, 30);
  public override int MaxInitialHp => MinInitialHp;

  private int AttackDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);
  private const int AttackHits = 2;

  private const string STAB = "STAB";

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
    animator.AddAnyState("Slash", attack);
    animator.AddAnyState("Hit", hit);

    return animator;
  }
}
