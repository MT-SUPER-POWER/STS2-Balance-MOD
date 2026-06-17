using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// STS1-BOSS-02 — 收藏家召唤物 Torch Head。
/// 输入：怪物回合。
/// 输出：进行单体火焰攻击，死亡后从战斗移除。
/// </summary>
public sealed class CollectorTorchHead : Sts2MonsterModel
{
  protected override string VisualsPath => "res://Assets/ActsFromThePast/ActsFromThePast/monsters/collector/collector.tscn";

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 40, 38);

  public override int MaxInitialHp => MinInitialHp;

  private int TackleDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 9, 7);

  public override async Task AfterAddedToRoom()
  {
    await base.AfterAddedToRoom();
    await PowerCmd.Apply<MinionPower>(new ThrowingPlayerChoiceContext(), Creature, 1M, null, null);
  }

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    var tackle = new MoveState("TACKLE_MOVE", TackleMove, new SingleAttackIntent(TackleDamage));
    tackle.FollowUpState = tackle;
    return new MonsterMoveStateMachine([tackle], tackle);
  }

  private async Task TackleMove(IReadOnlyList<Creature> targets)
  {
    await DamageCmd.Attack(TackleDamage).FromMonster(this)
      .WithAttackerAnim("Attack", 0.3f)
      .WithHitFx("vfx/vfx_attack_blunt")
      .Execute(null);
  }
}
