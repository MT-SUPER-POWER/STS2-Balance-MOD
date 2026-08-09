using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// Target: CardModel.get_Rarity for Accelerant.
/// Reason: CARD-02 restores Accelerant from Uncommon to Rare.
/// WARNING: Target behavior is verified against decompiled game source; do not modify that source directly.
/// </summary>
[HarmonyPatch(typeof(CardModel), "get_Rarity")]
public static class AccelerantRarityRollbackPatch
{
    [HarmonyPrefix]
    public static bool Prefix(CardModel __instance, ref CardRarity __result)
    {
        if (__instance is not Accelerant)
            return true;

        __result = CardRarity.Rare;
        return false;
    }
}
