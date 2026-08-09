using STS2RitsuLib.Content;
using STS2RitsuLib.Scaffolding.Content;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;

namespace Sts2BalanceMod.Sts2BalanceModCode.Abstract;

/// <summary>
/// This is the base class for your mod's relics, which is set up to load the relic's images from your mod's resources.
/// When creating a relic, right click the Relics folder and create a new file with the Custom Relic template.
/// This will generate a class that extends this one.
/// You can also just create the class manually; just make sure to inherit from this class.
/// </summary>
public abstract class BalanceRelicTemplate : ModRelicTemplate
{
  public override RelicAssetProfile AssetProfile => new(
    IconPath: ModAssetPaths.RelicIcon(ModAssetPaths.ContentFileName(Id.Entry)),
    IconOutlinePath: ModAssetPaths.RelicOutlineIcon(ModAssetPaths.ContentFileName(Id.Entry)),
    BigIconPath: ModAssetPaths.LargeRelicIcon(ModAssetPaths.ContentFileName(Id.Entry)));
}
