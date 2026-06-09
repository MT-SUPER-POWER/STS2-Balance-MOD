using MegaCrit.Sts2.Core.Entities.Powers;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// LEGACY-04 — 电动力学能力
/// 闪电球从“随机攻击一个敌人”改为“攻击所有敌人”（见 LightningOrbElectrodynamicsPatch）
/// </summary>
public sealed class ElectrodynamicsPower : Sts2PowerModel
{
  public ElectrodynamicsPower() : base(PowerType.Buff, PowerStackType.Single) { }
}
