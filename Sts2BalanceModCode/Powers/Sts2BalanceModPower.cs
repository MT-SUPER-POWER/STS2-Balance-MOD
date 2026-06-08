using BaseLib.Abstracts;
using BaseLib.Extensions;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using MegaCrit.Sts2.Core.Entities.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// DEPRECATED: 请使用 Sts2PowerModel（Abstract/Sts2PowerModel.cs）作为能力基类。
/// 保留此文件以兼容旧模板生成代码。
/// </summary>
public abstract class Sts2BalanceModPower(PowerType type, PowerStackType stackType) : Sts2PowerModel(type, stackType)
{
}
