using MegaCrit.Sts2.Core.Entities.Cards;
using STS2RitsuLib.Content;
using STS2RitsuLib.Scaffolding.Content;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;

namespace Sts2BalanceMod.Sts2BalanceModCode.Abstract;

/// <summary>
/// Shared RitsuLib card convention: the published entry determines the conventional artwork file name.
/// </summary>
public abstract class BalanceCardTemplate(int cost, CardType type, CardRarity rarity, TargetType target) :
    ModCardTemplate(cost, type, rarity, target)
{
  public override CardAssetProfile AssetProfile => new(
    PortraitPath: ModAssetPaths.LargeCardPortrait(ModAssetPaths.ContentFileName(Id.Entry)));
}
