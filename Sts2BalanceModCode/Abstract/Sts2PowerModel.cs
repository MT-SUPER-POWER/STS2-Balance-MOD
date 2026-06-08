using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Powers;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;

namespace Sts2BalanceMod.Sts2BalanceModCode.Abstract;

/// <summary>
/// 自定义 Power 抽象基类 — 自动加载 mod 资源路径下的能力图标
/// 对标 Sts2CardModel 的设计模式
/// </summary>
public abstract class Sts2PowerModel(PowerType type, PowerStackType stackType) : CustomPowerModel
{
  // ======================== IMAGE PATHS ========================
  public override string CustomPackedIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".PowerImagePath();
  public override string CustomBigIconPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigPowerImagePath();

  // ======================== POWER PROPERTIES ========================
  public override PowerType Type => type;
  public override PowerStackType StackType => stackType;
}
