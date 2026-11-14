using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-FORGOTTEN-RITUAL — 被遗忘的仪式：升级后取消消耗（移除 Exhaust 词条，保持基础 3 点能量获得）。
/// Target: ForgottenRitual.OnUpgrade, ForgottenRitual.get_ExtraHoverTips
/// Reason: 原版升级增加 1 点能量（获得 4 点能量，且仍保留消耗）；修改为升级后不再增加能量，而是移除消耗词条。
/// WARNING: Verified against D:\Game\Sts2Code\src\Core\Models\Cards\ForgottenRitual.cs.
/// </summary>
[HarmonyPatch]
[Sts2BalanceMod.Sts2BalanceModCode.Settings.BalancePatch("C40")]
public static class ForgottenRitualUpgradePatch
{
  [HarmonyPatch(typeof(ForgottenRitual), "OnUpgrade")]
  [HarmonyPrefix]
  public static bool OnUpgradePrefix(ForgottenRitual __instance)
  {
    __instance.RemoveKeyword(CardKeyword.Exhaust);
    return false; // 取消原版的 base.DynamicVars.Energy.UpgradeValueBy(1m)
  }

  [HarmonyPatch(typeof(ForgottenRitual), "get_ExtraHoverTips")]
  [HarmonyPrefix]
  public static bool ExtraHoverTipsPrefix(ForgottenRitual __instance, ref IEnumerable<IHoverTip> __result)
  {
    if (__instance.IsUpgraded)
    {
      __result = [HoverTipFactory.ForEnergy(__instance)];
      return false;
    }
    return true;
  }
}
