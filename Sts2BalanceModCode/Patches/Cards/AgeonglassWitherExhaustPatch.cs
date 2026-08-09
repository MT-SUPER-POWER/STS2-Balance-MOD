using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// 改为 消耗 保留
/// </summary>
[HarmonyPatch(typeof(Wither), "get_CanonicalKeywords")]
public static class AgeonglassWitherKeywordsPatch
{
  [HarmonyPrefix]
  public static bool Prefix(ref IEnumerable<CardKeyword> __result)
  {
    __result = [CardKeyword.Exhaust];
    return false;
  }
}

/// <summary>
/// 从不可打出改为可用一费消耗
/// </summary>
[HarmonyPatch(typeof(CardModel), "get_CanonicalEnergyCost")]
public static class AgeonglassWitherCostPatch
{
  [HarmonyPrefix]
  public static bool Prefix(CardModel __instance, ref int __result)
  {
    if (__instance is not Wither) return true;
    __result = 1;
    return false;
  }
}
