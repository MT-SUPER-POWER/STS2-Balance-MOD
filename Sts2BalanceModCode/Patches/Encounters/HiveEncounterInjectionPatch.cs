/* using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Encounters;

/// <summary>
/// 将红面具三人帮遭遇注入 Hive（第 2 幕），使其怪物在 Compendium 图鉴中可见。
/// 该遭遇也通过 MaskedBandits 事件触发，但注入到遭遇池是为了图鉴展示。
/// </summary>
public static class HiveEncounterInjectionPatch
{
  [HarmonyPatch(typeof(Hive), nameof(Hive.GenerateAllEncounters))]
  public static class HivePatch
  {
    [HarmonyPostfix]
    private static void Postfix(ref IEnumerable<EncounterModel> __result)
    {
      __result = __result.Append(ModelDb.Encounter<RedMaskBandits>());
    }
  }
}
 */
