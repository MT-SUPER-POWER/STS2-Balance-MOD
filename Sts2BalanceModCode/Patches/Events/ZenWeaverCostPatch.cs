using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Events;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// 禅意织者（Zen Weaver）删牌事件价格调整：
/// - 删 1 张牌 (EmotionalAwarenessCost): 125金 -> 75金（触发门槛同步降至 75金）
/// - 删 2 张牌 (ArachnidAcupunctureCost): 250金 -> 150金
/// - 顿悟 (BreathingTechniquesCost): 50金 (保持不变)
/// </summary>
[HarmonyPatch(typeof(ZenWeaver), "get_CanonicalVars")]
public static class ZenWeaverCostPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[]
        {
            new DynamicVar("BreathingTechniquesCost", 50m),
            new DynamicVar("EmotionalAwarenessCost", 75m),
            new DynamicVar("ArachnidAcupunctureCost", 150m)
        };
        return false;
    }
}
