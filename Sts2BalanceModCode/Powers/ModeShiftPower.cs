using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

public sealed class ModeShiftPower() : Sts2PowerModel(PowerType.Buff, PowerStackType.Counter)
{
  public override bool ShouldScaleInMultiplayer => true;

  public override string CustomPackedIconPath =>
    "res://Sts2BalanceMod/images/powers/actsfromthepast-mode_shift_power.png";

  public override string CustomBigIconPath => CustomPackedIconPath;

  public override async Task AfterDamageReceived(
    PlayerChoiceContext choiceContext,
    Creature target,
    DamageResult result,
    ValueProp props,
    Creature? dealer,
    CardModel? cardSource)
  {
    if (target != Owner || result.UnblockedDamage <= 0M)
      return;
    if (Owner.Monster is not Guardian guardian)
      return;
    if (!guardian.IsOpen || guardian.CloseUpTriggered || Owner.IsDead)
      return;

    var newAmount = Math.Max(0, Amount - (int)result.UnblockedDamage);
    SetAmount(newAmount);

    if (newAmount > 0)
      return;

    Flash();
    guardian.CloseUpTriggered = true;
    if (guardian.IsExecutingMove)
    {
      guardian.PendingModeShift = true;
      return;
    }

    await guardian.TransitionToDefensiveMode();
  }
}
