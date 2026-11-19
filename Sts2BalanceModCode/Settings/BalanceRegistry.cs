using System.Reflection;
using MegaCrit.Sts2.Core.Logging;

namespace Sts2BalanceMod.Sts2BalanceModCode.Settings;

internal static class BalanceRegistry
{
  internal static (Type Type, string? Id)[] Validate(Assembly assembly)
  {
    var catalog = BalanceCatalog.ValidateIds(BalanceCatalog.All);
    BalanceContent.ValidateBindings(catalog);
    BalanceLocalizationPatch.ValidateBindings(catalog);

    return assembly.GetTypes().Select(type =>
    {
      string? id = BalancePatch.FindSettingId(type);
      if (id != null)
        BalanceCatalog.RequireKnownId(catalog, id, $"Patch type '{type.FullName}'");
      return (type, id);
    }).ToArray();
  }

  internal static void LogStartupSummary(Logger logger)
  {
    var gameplay = BalanceCatalog.All.Where(change => change.AffectsGameplay).ToArray();
    var disabled = gameplay.Where(change => !BalanceModSettings.IsEnabled(change.Id))
      .Select(change => change.Id).ToArray();
    string disabledSummary = disabled.Length == 0 ? "" : $" Disabled: {string.Join(", ", disabled)}.";
    logger.Info($"Validated {BalanceCatalog.All.Count} balance settings. Gameplay settings enabled: {gameplay.Length - disabled.Length}/{gameplay.Length}.{disabledSummary}");
  }
}
