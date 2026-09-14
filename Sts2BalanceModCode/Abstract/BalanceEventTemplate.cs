using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using STS2RitsuLib.Content;
using STS2RitsuLib.Scaffolding.Content;

namespace Sts2BalanceMod.Sts2BalanceModCode.Abstract;

/// <summary>
/// Shared RitsuLib event convention. Individual events override the portrait only when their artwork deliberately
/// differs from their published entry.
/// </summary>
public abstract class BalanceEventTemplate : ModEventTemplate
{
  public override bool IsShared => false;

  protected virtual string PortraitFileName => ModAssetPaths.TypeFileName(GetType());

  public override EventAssetProfile AssetProfile => new(
    InitialPortraitPath: ModAssetPaths.EventImage(PortraitFileName));

  protected EventOption Option(Func<Task> action) => Option(action, "INITIAL", []);

  protected EventOption Option(Func<Task> action, string pageKey) => Option(action, pageKey, []);

  protected EventOption Option(Func<Task> action, string pageKey, IHoverTip hoverTip) =>
    Option(action, pageKey, [hoverTip]);

  protected EventOption Option(
    Func<Task> action,
    string pageKey,
    IEnumerable<IHoverTip> hoverTips)
  {
    string optionKey = ToSnakeCase(action.Method.Name).ToUpperInvariant();
    string localizationKey = pageKey == "INITIAL"
      ? InitialOptionKey(optionKey)
      : ModOptionKey(pageKey, optionKey);

    return new EventOption(this, action, localizationKey, hoverTips);
  }

  private static string ToSnakeCase(string str)
  {
    if (string.IsNullOrEmpty(str)) return str;
    var sb = new System.Text.StringBuilder(str.Length + 4);
    for (int i = 0; i < str.Length; i++)
    {
      char c = str[i];
      if (char.IsUpper(c) && i > 0)
      {
        sb.Append('_');
      }
      sb.Append(c);
    }
    return sb.ToString();
  }
}
