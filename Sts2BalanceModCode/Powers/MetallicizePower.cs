using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// 金属化能力 (Metallicize)：回合结束时获得指定数值的格挡。
/// </summary>
[RegisterPower]
public sealed class MetallicizePower() : BalancePowerTemplate(PowerType.Buff, PowerStackType.Counter)
{
  public override bool ShouldScaleInMultiplayer => true;

  public override async Task BeforeSideTurnEndEarly(
    PlayerChoiceContext choiceContext,
    CombatSide side,
    IEnumerable<Creature> participants)
  {
    if (Owner == null || side != Owner.Side)
      return;

    Flash();
    await CreatureCmd.GainBlock(Owner, (decimal)Amount, ValueProp.Unpowered, null);
  }
}
