using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-02 — 吸引仇恨 (PULL_AGGRO)
/// - 升级后数值调整为，召唤6，防御9
/// </summary>
[HarmonyPatch(typeof(PullAggro), "OnUpgrade")]
public static class PullAggroPatch
{
    [HarmonyPrefix]
    public static bool Prefix(PullAggro __instance)
    {
        __instance.DynamicVars.Summon.UpgradeValueBy(2m); // 基础 4 + 2 = 6
        __instance.DynamicVars.Block.UpgradeValueBy(2m);  // 基础 7 + 2 = 9
        return false;
    }
}
