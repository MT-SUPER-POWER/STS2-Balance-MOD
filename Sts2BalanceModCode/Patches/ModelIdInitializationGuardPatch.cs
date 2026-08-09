using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches;

/// <summary>
/// Target: <see cref="AbstractModel.InitId"/>.
/// Reason: other installed framework mods can invoke the game's ID initialization patch while a freshly discovered
/// mod model has not received its backing ID yet. Assigning the incoming ID first preserves the game's invariant and
/// lets RitsuLib's public-entry override run normally.
/// WARNING: relies on the game's auto-property backing field; re-check after a game model serialization update.
/// </summary>
[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.InitId))]
internal static class ModelIdInitializationGuardPatch
{
    private static readonly FieldInfo? IdBackingField = typeof(AbstractModel)
      .GetField("<Id>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);

    [HarmonyPrefix]
    private static bool Prefix(AbstractModel __instance, ModelId id)
    {
        if (__instance.Id == null)
        {
            IdBackingField?.SetValue(__instance, id);
        }

        return __instance.Id != null;
    }
}
