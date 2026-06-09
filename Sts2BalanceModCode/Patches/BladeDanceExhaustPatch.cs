using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches;

/// <summary>
/// CARD-02 — 刀舞删除消耗词条
/// Prefix 直接替换 CanonicalKeywords 为空，彻底移除消耗词条
/// </summary>
[HarmonyPatch(typeof(BladeDance), "get_CanonicalKeywords")]
public static class BladeDanceExhaustPatch
{
  [HarmonyPrefix]
  public static bool Prefix(ref IEnumerable<CardKeyword> __result)
  {
    __result = System.Array.Empty<CardKeyword>();
    return false; // 跳过原方法
  }
}

/// <summary>
/// CARD-03 — 刀舞白卡→蓝卡（稀有度降级）
/// </summary>
[HarmonyPatch(typeof(CardModel), "get_Rarity")]
public static class BladeDanceRarityPatch
{
  [HarmonyPrefix]
  public static bool Prefix(CardModel __instance, ref CardRarity __result)
  {
    if (__instance is BladeDance)
    {
      __result = CardRarity.Uncommon;
      return false; // 跳过原 getter
    }
    return true;
  }
}
