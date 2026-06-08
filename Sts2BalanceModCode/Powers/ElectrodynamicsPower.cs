using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// LEGACY-04 — 电动力学能力
/// 闪电球被动和激发伤害 +N（类似 Focus，但只作用于 LightningOrb）
/// </summary>
public sealed class ElectrodynamicsPower : Sts2BalanceModPower
{
  public override PowerType Type => PowerType.Buff;
  public override PowerStackType StackType => PowerStackType.Counter;

  public override decimal ModifyOrbValue(OrbModel orb, decimal value)
  {
    if (orb is LightningOrb && Owner.Player == orb.Owner)
      return value + (decimal)Amount;

    return value;
  }
}
