namespace Sts2BalanceMod.Sts2BalanceModCode.Extensions;

/// <summary>
/// Compatibility facade for existing content. New code should use <see cref="ModAssetPaths"/> directly.
/// </summary>
public static class StringExtensions
{
  public static string ImagePath(this string path) => ModAssetPaths.Image(path);
  public static string CardImagePath(this string path) => ModAssetPaths.CardPortrait(path);
  public static string BigCardImagePath(this string path) => ModAssetPaths.LargeCardPortrait(path);
  public static string PowerImagePath(this string path) => ModAssetPaths.PowerIcon(path);
  public static string BigPowerImagePath(this string path) => ModAssetPaths.LargePowerIcon(path);
  public static string RelicImagePath(this string path) => ModAssetPaths.RelicIcon(path);
  public static string BigRelicImagePath(this string path) => ModAssetPaths.LargeRelicIcon(path);
  public static string RestSiteOptionImagePath(this string path) => ModAssetPaths.RestSiteOptionIcon(path);
  public static string CharacterUiPath(this string path) => ModAssetPaths.CharacterUi(path);
  public static string EventImagePath(this string path) => ModAssetPaths.EventImage(path);
  public static string EncounterImagePath(this string path) => ModAssetPaths.EncounterImage(path);
}
