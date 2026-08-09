using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.src.Abstract;
using Sts2BalanceMod.src.Monsters;

namespace Sts2BalanceMod.src.Powers;

/// <summary>
/// AFP-BOSS-03 — 史莱姆分裂能力。
/// 在本次伤害令宿主降至半血或以下时立即把下一行动改为分裂，并阻止分裂链中途结束战斗。
/// </summary>
[RegisterPower]
public sealed class SplitPower() : BalancePowerTemplate(PowerType.Buff, PowerStackType.Single)
{
  public override PowerAssetProfile AssetProfile => new(
    IconPath: ModAssetPaths.PowerIcon("actsfromthepast-split_power.png"),
    BigIconPath: ModAssetPaths.PowerIcon("actsfromthepast-split_power.png"));

  public override bool ShouldStopCombatFromEnding() => true;

  public override Task AfterDamageReceived(
    PlayerChoiceContext choiceContext,
    Creature target,
    DamageResult result,
    ValueProp props,
    Creature? dealer,
    CardModel? cardSource)
  {
    if (target != Owner || result.UnblockedDamage <= 0 || target.CurrentHp > target.MaxHp / 2)
      return Task.CompletedTask;

    switch (Owner.Monster)
    {
      case AcidSlimeLarge { SplitTriggered: false } acidSlime:
        Flash();
        acidSlime.SplitTriggered = true;
        acidSlime.SetMoveImmediate(acidSlime.SplitState, true);
        break;
      case SpikeSlimeLarge { SplitTriggered: false } spikeSlime:
        Flash();
        spikeSlime.SplitTriggered = true;
        spikeSlime.SetMoveImmediate(spikeSlime.SplitState, true);
        break;
      case SlimeBoss { SplitTriggered: false } slimeBoss:
        Flash();
        slimeBoss.SplitTriggered = true;
        slimeBoss.SetMoveImmediate(slimeBoss.SplitState, true);
        break;
    }

    return Task.CompletedTask;
  }
}
