using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-01 — 放血稀有度改回白卡
/// </summary>
[HarmonyPatch(typeof(CardModel), "get_Rarity")]
public static class BloodlettingRarityPatch
{
    [HarmonyPrefix]
    public static bool Prefix(CardModel __instance, ref CardRarity __result)
    {
        if (__instance is Bloodletting)
        {
            __result = CardRarity.Common;
            return false;
        }
        return true;
    }
}
