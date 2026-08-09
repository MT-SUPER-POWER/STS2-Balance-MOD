using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// RELIC-08 — 红面具改为 Event 稀有度，从 SharedRelicPool 移除，注入 EventRelicPool。
/// </summary>

// 把 RedMask 的稀有度从 Common 改为 Event
[HarmonyPatch(typeof(RedMask), "get_Rarity")]
public static class RedMaskRarityPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ref RelicRarity __result)
    {
        __result = RelicRarity.Event;
        return false; // 跳过原方法
    }
}

// 从 SharedRelicPool 移除 RedMask（池子是硬编码列表，改 Rarity 不够）
[HarmonyPatch(typeof(SharedRelicPool), "GenerateAllRelics")]
public static class RedMaskRemoveFromSharedPatch
{
    [HarmonyPostfix]
    public static RelicModel[] Postfix(RelicModel[] __result)
    {
        return __result.Where(r => r is not RedMask).ToArray();
    }
}

// 注入 RedMask 到 EventRelicPool（使其在遗物图鉴中可见）
[HarmonyPatch(typeof(EventRelicPool), "GenerateAllRelics")]
public static class RedMaskAddToEventPoolPatch
{
    [HarmonyPostfix]
    public static IEnumerable<RelicModel> Postfix(IEnumerable<RelicModel> __result)
    {
        return __result.Append(ModelDb.Relic<RedMask>());
    }
}
