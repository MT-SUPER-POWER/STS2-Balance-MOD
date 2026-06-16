using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// 增益研究者事件的突变之力：本场战斗临时获得力量。
/// 来源参考 ActsFromThePast.Powers.MutagenicStrengthPower。
/// </summary>
public sealed class MutagenicStrengthPower : TemporaryStrengthPower, ICustomModel
{
  public override AbstractModel OriginModel => ModelDb.Relic<MutagenicStrength>();
}
