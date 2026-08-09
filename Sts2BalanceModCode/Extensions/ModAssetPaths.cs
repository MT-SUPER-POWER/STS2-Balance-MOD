using Godot;

namespace Sts2BalanceMod.Sts2BalanceModCode.Extensions;

/// <summary>
/// Owns every convention-based path inside this mod's PCK.
/// Content models should request an asset here instead of assembling a <c>res://</c> path themselves.
/// </summary>
public static class ModAssetPaths
{
  public const string Root = "res://" + BalanceModEntry.ModId;

  public static string Resource(params string[] segments)
  {
    var path = Root;
    foreach (var segment in segments)
    {
      path = Path.Join(path, segment);
    }

    return path;
  }

  public static string Image(string fileName) => Resource("images", fileName);

  public static string CardPortrait(string fileName) =>
    ExistingOrFallback(Resource("images", "card_portraits", fileName),
      Resource("images", "card_portraits", "Card.png"), "card image");

  public static string LargeCardPortrait(string fileName) =>
    ExistingOrFallback(Resource("images", "card_portraits", "big", fileName),
      Resource("images", "card_portraits", "big", "Card.png"), "large card image");

  public static string PowerIcon(string fileName) =>
    ExistingOrFallback(Resource("images", "powers", fileName),
      Resource("images", "powers", "Power.png"), "power image");

  public static string LargePowerIcon(string fileName) =>
    ExistingOrFallback(Resource("images", "powers", "big", fileName),
      Resource("images", "powers", "big", "Power.png"), "large power image");

  public static string EnchantmentIcon(string fileName) =>
    Resource("images", "enchantments", fileName);

  public static string RelicIcon(string fileName) =>
    ExistingOrFallback(Resource("images", "relics", fileName),
      Resource("images", "relics", "Relic.png"), "relic image");

  public static string LargeRelicIcon(string fileName) =>
    ExistingOrFallback(Resource("images", "relics", "big", fileName),
      Resource("images", "relics", "big", "Relic.png"), "large relic image");

  public static string RelicOutlineIcon(string fileName) =>
    ExistingOrFallback(Resource("images", "relics", "outlines", fileName),
      ExistingOrFallback(Resource("images", "relics", $"{Path.GetFileNameWithoutExtension(fileName)}Outline.png"),
        Resource("images", "relics", "outlines", "Relic.png"), "relic outline image"), "relic outline image");

  public static string RestSiteOptionIcon(string fileName) =>
    ExistingOrFallback(Resource("images", "ui", "rest_site", fileName),
      Resource("images", "ui", "rest_site", "Option.png"), "rest-site option image");

  public static string CharacterUi(string fileName) => Resource("images", "charui", fileName);

  public static string EventImage(string fileName) =>
    ExistingOrFallback(Resource("images", "events", fileName),
      Resource("images", "events", "Event.png"), "event image");

  public static string EncounterImage(string fileName) => Resource("images", "encounters", fileName);

  public static string ContentFileName(string publicEntry)
  {
    const string legacyPrefix = "STS2_BALANCEMOD_";
    var stem = publicEntry.StartsWith(legacyPrefix, StringComparison.OrdinalIgnoreCase)
      ? publicEntry[legacyPrefix.Length..]
      : publicEntry;

    return $"{ToPascalCase(stem)}.png";
  }

  public static string TypeFileName(Type type)
  {
    ArgumentNullException.ThrowIfNull(type);
    return $"{type.Name}.png";
  }

  private static string ExistingOrFallback(string path, string fallbackPath, string assetKind)
  {
    if (ResourceLoader.Exists(path))
    {
      return path;
    }

    BalanceModEntry.Logger.Info($"Could not find {assetKind} path: {path}");
    return fallbackPath;
  }

  private static string ToPascalCase(string text)
  {
    if (string.IsNullOrEmpty(text)) return text;
    if (text.Contains('_') || text.Contains('-') || text.Contains(' '))
    {
      var parts = text.Split(new[] { '_', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
      return string.Concat(parts.Select(p => p.Length == 0 ? "" : char.ToUpperInvariant(p[0]) + p[1..].ToLowerInvariant()));
    }
    if (text.All(char.IsUpper))
    {
      return char.ToUpperInvariant(text[0]) + text[1..].ToLowerInvariant();
    }
    return char.ToUpperInvariant(text[0]) + text[1..];
  }
}
