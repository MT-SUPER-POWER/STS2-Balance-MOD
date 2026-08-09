using MegaCrit.Sts2.Core.Entities.Powers;
using STS2RitsuLib.Content;
using STS2RitsuLib.Scaffolding.Content;
using Sts2BalanceMod.src.Extensions;

namespace Sts2BalanceMod.src.Abstract;

/// <summary>
/// 自定义 Power 抽象基类 — 自动加载 mod 资源路径下的能力图标
/// 对标 BalanceCardTemplate 的设计模式
/// </summary>
public abstract class BalancePowerTemplate(PowerType type, PowerStackType stackType) : ModPowerTemplate
{
  public override PowerAssetProfile AssetProfile => new(
    IconPath: ModAssetPaths.PowerIcon(ModAssetPaths.TypeFileName(GetType())),
    BigIconPath: ModAssetPaths.LargePowerIcon(ModAssetPaths.TypeFileName(GetType())));

  // ======================== POWER PROPERTIES ========================
  public override PowerType Type => type;
  public override PowerStackType StackType => stackType;
}
