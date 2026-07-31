using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-04 & CARD-10 — 华丽收场 (Grand Finale) X 费与打出条件调整。
/// 
/// 目标类型: MegaCrit.Sts2.Core.Models.Cards.GrandFinale
/// 目标方法: get_IsPlayable
/// 修改原因: 基础版修改为“抽牌堆卡牌数 <= 当前能量（X）”，升级版修改为“抽牌堆卡牌数 <= 当前能量 + 2（X + 2）”。
/// 反编译警告: 原始方法在 GrandFinale.cs 中实现为 IsPlayable，编译后对应 get_IsPlayable 属性。
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
    int maxAllowed = __instance.IsUpgraded ? energy + 2 : energy;
    __result = drawPileCount <= maxAllowed;

    return false;
  }
}

/// <summary>
/// 目标类型: MegaCrit.Sts2.Core.Models.CardModel
/// 目标方法: get_HasEnergyCostX
/// 修改原因: 华丽收场原本是 0 费牌，我们需要让游戏认为它是 X 费牌。
/// 反编译警告: CardModel.HasEnergyCostX 是 virtual 属性，由 CardModel 的派生类覆写。
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
/// CARD-10 — 华丽收场 (Grand Finale) 升级逻辑重写。
/// 
/// 目标类型: MegaCrit.Sts2.Core.Models.Cards.GrandFinale
/// 目标方法: OnUpgrade
/// 修改原因: 升级后扩大打出条件至 X+2，不再增加伤害数值。
/// </summary>
[HarmonyPatch(typeof(GrandFinale), "OnUpgrade")]
public static class GrandFinaleCanonicalUpgradePatch
{
  [HarmonyPrefix]
  public static bool Prefix(GrandFinale __instance)
  {
    // 不调用原版 OnUpgrade(UpgradeValueBy 15m)，保持基础伤害不变
    return false;
  }
}


