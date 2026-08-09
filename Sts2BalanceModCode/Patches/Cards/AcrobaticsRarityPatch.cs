using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-03 — 杂技蓝卡→白卡（稀有度降级）
/// Patch CardModel 基类的 get_Rarity，用 Prefix 直接覆盖返回值
/// </summary>
[HarmonyPatch(typeof(CardModel), "get_Rarity")]
public static class AcrobaticsRarityPatch
{
    [HarmonyPrefix]
    public static bool Prefix(CardModel __instance, ref CardRarity __result)
    {
        if (__instance is Acrobatics)
        {
            __result = CardRarity.Common;
            return false; // 跳过原 getter
        }
        return true;
    }
}
