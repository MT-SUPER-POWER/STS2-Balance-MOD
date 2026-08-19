using System;
using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-COOLANT-01 — 故障机器人金卡能力牌「冷却剂」重做并降级为蓝卡
/// Target: CardModel.get_Rarity, CardModel.get_CanonicalVars, CardModel.get_ExtraHoverTips, Coolant.OnUpgrade
/// Reason: 稀有度从 Rare 降为 Uncommon，重做为能力抽牌引擎（打出能力牌时抽 1/2 张牌）。
/// WARNING: Verified against D:\Game\Sts2Code\src\MegaCrit.Sts2.Core.Models.Cards\Coolant.cs.
/// </summary>
[HarmonyPatch]
public static class CoolantPatch
{
    [HarmonyPatch(typeof(CardModel), "get_Rarity")]
    [HarmonyPrefix]
    public static bool RarityPrefix(CardModel __instance, ref CardRarity __result)
    {
        if (__instance is not Coolant)
            return true;

        __result = CardRarity.Uncommon;
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "get_CanonicalVars")]
    [HarmonyPrefix]
    public static bool CanonicalVarsPrefix(CardModel __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (__instance is not Coolant)
            return true;

        __result = new DynamicVar[] { new PowerVar<CoolantPower>(1m) };
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "get_ExtraHoverTips")]
    [HarmonyPrefix]
    public static bool ExtraHoverTipsPrefix(CardModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (__instance is not Coolant)
            return true;

        __result = Array.Empty<IHoverTip>();
        return false;
    }

    [HarmonyPatch(typeof(Coolant), "OnUpgrade")]
    [HarmonyPrefix]
    public static bool OnUpgradePrefix(Coolant __instance)
    {
        __instance.DynamicVars["CoolantPower"].UpgradeValueBy(1m);
        return false;
    }
}
