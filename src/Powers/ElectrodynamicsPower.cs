using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Entities.Powers;
using Sts2BalanceMod.src.Abstract;

namespace Sts2BalanceMod.src.Powers;

/// <summary>
/// LEGACY-04 — 电动力学能力
/// 闪电球从“随机攻击一个敌人”改为“攻击所有敌人”（见 LightningOrbElectrodynamicsPatch）
/// </summary>
[RegisterPower]
public sealed class ElectrodynamicsPower : BalancePowerTemplate
{
  public ElectrodynamicsPower() : base(PowerType.Buff, PowerStackType.Single) { }
}
