using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// 女巫形态能力：在你的回合开始时额外抽牌。
/// </summary>
[RegisterPower]
public sealed class WitchFormPower() : BalancePowerTemplate(PowerType.Buff, PowerStackType.Counter)
{
  public override decimal ModifyHandDraw(Player player, decimal count)
  {
    if (player != Owner.Player)
    {
      return count;
    }

    return count + (decimal)Amount;
  }
}
