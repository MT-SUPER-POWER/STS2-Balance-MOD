using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Powers;

/// <summary>
/// CARD-07 — 创世之柱平衡调整
/// 官方 v0.110.0 更新了效果机制。Mod 仅将其基础格挡调整为 3 点（升级后 4 点）。
/// </summary>
[HarmonyPatch(typeof(PillarOfCreation), MethodType.Constructor)]
public static class PillarOfCreationPatch
{
    [HarmonyPostfix]
    public static void CardConstructorPostfix(PillarOfCreation __instance)
    {
        if (__instance.DynamicVars != null && __instance.DynamicVars.ContainsKey("Block"))
        {
            __instance.DynamicVars["Block"].BaseValue = 3M;
        }
    }

    [HarmonyPatch(typeof(PillarOfCreation), "OnUpgrade")]
    [HarmonyPrefix]
    public static bool OnUpgradePrefix(PillarOfCreation __instance)
    {
        // 原版 UpgradeValueBy(2m) 会使 Block 从 3 变 5。
        // 此处覆盖改为 UpgradeValueBy(1m)，使升级后 Block 为 4 点。
        __instance.DynamicVars["Block"].UpgradeValueBy(1M);
        return false;
    }
}

