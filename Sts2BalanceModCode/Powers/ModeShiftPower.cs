using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// AFP-BOSS-01 - 守护者的形态转换伤害计数器。
/// 达到阈值时立刻切换为防御形态；若守护者正在执行行动，则延迟到行动结束后切换。
/// </summary>
[RegisterPower]
public sealed class ModeShiftPower() : BalancePowerTemplate(PowerType.Buff, PowerStackType.Counter)
{
  public override bool ShouldScaleInMultiplayer => true;


  public override async Task AfterDamageReceived(
    PlayerChoiceContext choiceContext,
    Creature target,
    DamageResult result,
    ValueProp props,
    Creature? dealer,
    CardModel? cardSource)
  {
    if (target != Owner || result.UnblockedDamage <= 0 || Owner.IsDead)
      return;

    if (Owner.Monster is not Guardian { IsOpen: true, CloseUpTriggered: false } guardian)
      return;

    var newAmount = Math.Max(0, Amount - result.UnblockedDamage);
    SetAmount(newAmount);

    if (newAmount > 0)
      return;

    Flash();
    guardian.CloseUpTriggered = true;
    if (guardian.IsExecutingMove)
      guardian.PendingModeShift = true;
    else
      await guardian.TransitionToDefensiveMode();
  }
}
