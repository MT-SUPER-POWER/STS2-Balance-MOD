using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// 增益研究者事件的突变之力：本场战斗临时获得力量。
/// 来源参考 ActsFromThePast.Powers.MutagenicStrengthPower。
/// </summary>
[RegisterPower]
public sealed class MutagenicStrengthPower : TemporaryStrengthPower
{
    public override AbstractModel OriginModel => ModelDb.Relic<MutagenicStrength>();
}
