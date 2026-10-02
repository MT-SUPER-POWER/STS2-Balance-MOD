using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// 乐嘉维林休眠能力 (AsleepLagavulinPower)。
/// 保持休眠状态；受到未格挡伤害立即苏醒，回合结束倒数，3 回合后自然苏醒。
/// </summary>
[RegisterPower]
public sealed class AsleepLagavulinPower() : BalancePowerTemplate(PowerType.Buff, PowerStackType.Counter)
{
  public override async Task AfterDamageReceived(
    PlayerChoiceContext choiceContext,
    Creature target,
    DamageResult result,
    ValueProp props,
    Creature? dealer,
    CardModel? cardSource)
  {
    if (Owner == null || target != Owner || result.UnblockedDamage <= 0)
      return;

    if (Owner.Monster is not Lagavulin lagavulin)
      return;

    if (Owner.HasPower<MetallicizePower>())
      await PowerCmd.Remove(Owner.GetPower<MetallicizePower>()!);

    await lagavulin.WakeUpFromDamage();
    await PowerCmd.Remove(this);
  }

  public override Task BeforeSideTurnStart(
    PlayerChoiceContext choiceContext,
    CombatSide side,
    IReadOnlyList<Creature> participants,
    ICombatState combatState)
  {
    if (Owner == null || side != CombatSide.Player || combatState.RoundNumber != 1)
      return Task.CompletedTask;

    MetallicizePower? metalPower = Owner.GetPower<MetallicizePower>();
    if (metalPower == null)
      return Task.CompletedTask;

    return CreatureCmd.GainBlock(Owner, metalPower.Amount, ValueProp.Unpowered, null);
  }

  public override async Task BeforeSideTurnEndVeryEarly(
    PlayerChoiceContext choiceContext,
    CombatSide side,
    IEnumerable<Creature> participants)
  {
    if (Owner == null || side != Owner.Side || Amount > 1 || !Owner.HasPower<MetallicizePower>())
      return;

    await PowerCmd.Remove(Owner.GetPower<MetallicizePower>()!);
  }

  public override async Task AfterSideTurnEnd(
    PlayerChoiceContext choiceContext,
    CombatSide side,
    IEnumerable<Creature> participants)
  {
    if (Owner == null || side != Owner.Side)
      return;

    await PowerCmd.Decrement(this);
    if (Amount > 0)
      return;

    if (Owner.Monster is Lagavulin lagavulin)
      await lagavulin.WakeUpNaturally();
  }
}
