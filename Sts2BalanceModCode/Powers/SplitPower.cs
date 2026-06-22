using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// 史莱姆分裂能力 — 当持有者生命 ≤ 50% 时触发强制分裂动作。
/// 参考 ActsFromThePast.Powers.SplitPower。
/// </summary>
public sealed class SplitPower : Sts2PowerModel
{
  public SplitPower() : base(PowerType.Buff, PowerStackType.Single)
  {
  }

  public override string CustomPackedIconPath =>
    "res://Sts2BalanceMod/images/powers/actsfromthepast-split_power.png";

  public override string CustomBigIconPath => CustomPackedIconPath;

  public override bool ShouldStopCombatFromEnding() => true;

  public override async Task AfterDamageReceived(
    PlayerChoiceContext choiceContext,
    Creature target,
    DamageResult result,
    ValueProp props,
    Creature? dealer,
    CardModel? cardSource)
  {
    if (target != Owner)
      return;
    if (result.UnblockedDamage <= 0)
      return;
    if (target.CurrentHp > target.MaxHp / 2)
      return;

    if (Owner.Monster is SlimeBoss slimeBoss)
    {
      if (slimeBoss.SplitTriggered)
        return;
      Flash();
      slimeBoss.SplitTriggered = true;
      slimeBoss.SetMoveImmediate(slimeBoss.SplitState, true);
    }
    else if (Owner.Monster is AcidSlimeLarge acidSlime)
    {
      if (acidSlime.SplitTriggered)
        return;
      Flash();
      acidSlime.SplitTriggered = true;
      acidSlime.SetMoveImmediate(acidSlime.SplitState, true);
    }
    else if (Owner.Monster is SpikeSlimeLarge spikeSlime)
    {
      if (spikeSlime.SplitTriggered)
        return;
      Flash();
      spikeSlime.SplitTriggered = true;
      spikeSlime.SetMoveImmediate(spikeSlime.SplitState, true);
    }
  }
}
