using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.src.Patches.Cards;

/// <summary>
/// CARD-04 & CARD-10 — 华丽收场 (Grand Finale) X 费机制调整。
///
/// 基础版与升级版打出条件统一为：“抽牌堆卡牌数 <= 当前能量（X）”。
/// 能量扣除计算：
/// - 升级提供 2 点减费；
/// - 化学 X (Chemical X) 等 X 额外增益通过 Hook.ModifyXValue 动态再减 2 点能量（可叠加）；
/// - 实际扣能量 = Max(0, 当前能量 - 升级减费 - 化学X等X增益)。
/// </summary>
[HarmonyPatch(typeof(GrandFinale), "get_IsPlayable")]
public static class GrandFinaleIsPlayablePatch
{
    [HarmonyPrefix]
    public static bool Prefix(GrandFinale __instance, ref bool __result)
    {
        if (__instance.Owner == null || __instance.Owner.PlayerCombatState == null)
        {
            return true;
        }

        int drawPileCount = PileType.Draw.GetPile(__instance.Owner).Cards.Count;
        int energy = __instance.Owner.PlayerCombatState.Energy;
        __result = drawPileCount <= energy;

        return false;
    }
}

/// <summary>
/// 目标类型: MegaCrit.Sts2.Core.Models.CardModel
/// 目标方法: get_HasEnergyCostX
/// 修改原因: 华丽收场原本是 0 费牌，我们需要让游戏认为它是 X 费牌。
/// </summary>
[HarmonyPatch(typeof(CardModel), "get_HasEnergyCostX")]
public static class GrandFinaleHasEnergyCostXPatch
{
    [HarmonyPrefix]
    public static bool Prefix(CardModel __instance, ref bool __result)
    {
        if (__instance is GrandFinale)
        {
            __result = true;
            return false;
        }
        return true;
    }
}

/// <summary>
/// GrandFinale 打出时扣除能量计算：结合升级减费与 Hook.ModifyXValue (如化学 X)。
/// </summary>
[HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.GetAmountToSpend))]
public static class GrandFinaleEnergyToSpendPatch
{
    private static readonly FieldInfo _cardField = AccessTools.Field(typeof(CardEnergyCost), "_card");

    [HarmonyPrefix]
    public static bool Prefix(CardEnergyCost __instance, ref int __result)
    {
        if (_cardField?.GetValue(__instance) is GrandFinale grandFinale && grandFinale.Owner?.PlayerCombatState != null)
        {
            int currentEnergy = grandFinale.Owner.PlayerCombatState.Energy;
            int upgradeSavings = grandFinale.IsUpgraded ? 2 : 0;
            int xModifierBonus = grandFinale.CombatState != null ? Hook.ModifyXValue(grandFinale.CombatState, grandFinale, 0) : 0;
            int totalSavings = upgradeSavings + xModifierBonus;

            __result = Math.Max(0, currentEnergy - totalSavings);
            return false;
        }
        return true;
    }
}

/// <summary>
/// 动态注册 DynamicVar（CalculationBase, CalculationExtra, EnergySaved 与 CalculatedSpend）：
/// - CalculationBase/CalculationExtra: 防止 CalculatedVar 在 SetOwner/UpdateValues 初始化时抛出 KeyNotFoundException('CalculationBase') 报错；
/// - EnergySaved: 用于非战斗/图鉴界面展示 {EnergySaved:diff()} 动态高亮数值；
/// - CalculatedSpend: 用于战斗中手牌实时计算并渲染具体的扣除能量。
/// </summary>
[HarmonyPatch(typeof(GrandFinale), "get_CanonicalVars")]
public static class GrandFinaleCanonicalVarsPatch
{
    [HarmonyPostfix]
    public static void Postfix(GrandFinale __instance, ref IEnumerable<DynamicVar> __result)
    {
        var list = __result.ToList();
        list.Add(new CalculationBaseVar(0m));
        list.Add(new CalculationExtraVar(1m));
        list.Add(new EnergyVar("EnergySaved", 0));
        list.Add(new CalculatedVar("CalculatedSpend").WithMultiplier((CardModel card, Creature? _) =>
        {
            if (card.Owner?.PlayerCombatState == null) return 0;
            int currentEnergy = card.Owner.PlayerCombatState.Energy;
            int upgradeSavings = card.IsUpgraded ? 2 : 0;
            int xModifierBonus = card.CombatState != null ? Hook.ModifyXValue(card.CombatState, card, 0) : 0;
            int totalSavings = upgradeSavings + xModifierBonus;
            return Math.Max(0, currentEnergy - totalSavings);
        }));
        __result = list;
    }
}

/// <summary>
/// CARD-10 — 华丽收场 (Grand Finale) 升级逻辑重写：对 EnergySaved 变量执行 UpgradeValueBy(2m)。
/// </summary>
[HarmonyPatch(typeof(GrandFinale), "OnUpgrade")]
public static class GrandFinaleCanonicalUpgradePatch
{
    [HarmonyPrefix]
    public static bool Prefix(GrandFinale __instance)
    {
        if (__instance.DynamicVars.ContainsKey("EnergySaved"))
        {
            __instance.DynamicVars["EnergySaved"].UpgradeValueBy(2m);
        }
        return false;
    }
}
