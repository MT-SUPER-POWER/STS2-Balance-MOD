using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

[HarmonyPatch(typeof(PreservedFog), "get_CanonicalVars")]
public static class PreservedFogMoreDELPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[1]
        {
      new CardsVar(4)
        };
        return false;
    }
}
