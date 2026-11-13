using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Events;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// EVENT-NEW-EVENTS-CURSE-REDUCE — 诺奴佩普温泉 (Thermae Nonupeipe) 诅咒数量调整。
/// Target: MegaCrit.Sts2.Core.Models.Events.ThermaeNonupeipe.get_CanonicalVars.
/// Reason: 痛饮泉水选项获取先古遗物时获得的「内疚」诅咒数量由 3 张减少至 2 张。
/// WARNING: Verified against decompiled ThermaeNonupeipe.cs in sts2.dll v0.113.0.
/// </summary>
[HarmonyPatch(typeof(ThermaeNonupeipe), "get_CanonicalVars")]
[Sts2BalanceMod.Sts2BalanceModCode.Settings.BalancePatch("E19")]
public static class ThermaeNonupeipeCursePatch
{
  [HarmonyPrefix]
  public static bool Prefix(ref IEnumerable<DynamicVar> __result)
  {
    __result = new DynamicVar[]
    {
      new GoldVar("ThermalBathsCost", 88),
      new HpLossVar("DrinkHpLoss", 31m),
      new CardsVar("DrinkGuiltyCount", 2),
      new StringVar("BathTowelDescription"),
      new StringVar("BathRobesDescription"),
      new StringVar("DrinkRelicName")
    };
    return false;
  }
}
