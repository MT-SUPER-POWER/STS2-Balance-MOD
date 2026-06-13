using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-03 — 玉石凋萎：凋零卡改为 1费 消耗 保留，其余逻辑不变
/// </summary>
[HarmonyPatch(typeof(Wither), "get_CanonicalKeywords")]
public static class AgeonglassWitherKeywordsPatch
{
  [HarmonyPrefix]
  public static bool Prefix(ref IEnumerable<CardKeyword> __result)
  {
    __result = new CardKeyword[] { CardKeyword.Exhaust, CardKeyword.Retain };
    return false;
  }
}

// ======================== 费用：从 -1（不可打出）改为 1 费 ========================
// NOTE: CanonicalEnergyCost 定义在基类 CardModel 且 Wither 未覆写，所以必须 patch
// CardModel 的 getter，再通过 __instance 类型判断只对 Wither 生效
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
