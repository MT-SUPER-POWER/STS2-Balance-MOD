using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-01 — 骨妹升级的挽歌改为不消耗
/// 升级前保留原版消耗词条，升级后仅移除消耗词条。
/// </summary>
[HarmonyPatch(typeof(Dirge), "get_CanonicalKeywords")]
public static class DirgeExhaustPatch
{
  [HarmonyPrefix]
  public static bool Prefix(Dirge __instance, ref IEnumerable<CardKeyword> __result)
  {
    if (!__instance.IsUpgraded)
    {
      return true;
    }

    __result = System.Array.Empty<CardKeyword>();
    return false; // 跳过原方法
  }
}

/// <summary>
/// CARD-01 — 骨妹升级的挽歌不提高召唤次数
/// Prefix 跳过原版 OnUpgrade，避免 Summon 动态值从 3 提升到 4。
/// </summary>
[HarmonyPatch(typeof(Dirge), "OnUpgrade")]
public static class DirgeUpgradePatch
{
  [HarmonyPrefix]
  public static bool Prefix()
  {
    return false; // 跳过原方法
  }
}
