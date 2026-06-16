using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// STS1-BOSS-02 — 收藏家怪物模型。
/// 输入：Boss 回合与场上召唤物数量。
/// 输出：攻击、强化或召唤 Torch Head。
/// </summary>
public sealed class Collector : MonsterModel
{
  protected override string VisualsPath => "res://Assets/ActsFromThePast/ActsFromThePast/monsters/collector/collector.tscn";

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 300, 282);

  public override int MaxInitialHp => MinInitialHp;

  private int FireballDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 21, 18);

  private int MegaDebuffDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 18, 15);

  private int BuffStrength => 3;

  private int BuffBlock => 15;

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    var summon = new MoveState("SUMMON_MOVE", SummonMove, new UnknownIntent());
    var fireball = new MoveState("FIREBALL_MOVE", FireballMove, new SingleAttackIntent(FireballDamage));
    var buff = new MoveState("BUFF_MOVE", BuffMove, new BuffIntent(), new DefendIntent());
    var megaDebuff = new MoveState("MEGA_DEBUFF_MOVE", MegaDebuffMove,
      new SingleAttackIntent(MegaDebuffDamage), new DebuffIntent());
    var branch = new ConditionalBranchState("CollectorBranch");

    summon.FollowUpState = fireball;
    fireball.FollowUpState = buff;
    buff.FollowUpState = megaDebuff;
    megaDebuff.FollowUpState = branch;
    branch.AddState(summon, ShouldSummon);
    branch.AddState(fireball, () => !ShouldSummon());

    return new MonsterMoveStateMachine([summon, fireball, buff, megaDebuff, branch], summon);
  }

  private bool ShouldSummon()
  {
    return CombatState.Enemies.Count(c => c.Monster is CollectorTorchHead && !c.IsDead) < 2;
  }

  private async Task SummonMove(IReadOnlyList<Creature> targets)
  {
    if (!ShouldSummon())
      return;

    var slot = CombatState.Encounter?.GetNextSlot(CombatState);
    await CreatureCmd.Add<CollectorTorchHead>(CombatState, string.IsNullOrEmpty(slot) ? null : slot);
  }

  private async Task FireballMove(IReadOnlyList<Creature> targets)
  {
    await DamageCmd.Attack(FireballDamage).FromMonster(this)
      .WithAttackerAnim("Attack", 0.4f)
      .WithHitFx("vfx/vfx_attack_blunt")
      .Execute(null);
  }

  private async Task BuffMove(IReadOnlyList<Creature> targets)
  {
    foreach (var enemy in CombatState.Enemies.Where(c => !c.IsDead))
    {
      await CreatureCmd.GainBlock(enemy, BuffBlock, ValueProp.Move, null);
      await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), enemy, BuffStrength, Creature, null);
    }
  }

  private async Task MegaDebuffMove(IReadOnlyList<Creature> targets)
  {
    await DamageCmd.Attack(MegaDebuffDamage).FromMonster(this)
      .WithAttackerAnim("Attack", 0.4f)
      .WithHitFx("vfx/vfx_attack_blunt")
      .Execute(null);
    await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), targets, 3M, Creature, null);
    await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), targets, 3M, Creature, null);
    await PowerCmd.Apply<FrailPower>(new ThrowingPlayerChoiceContext(), targets, 3M, Creature, null);
  }
}
