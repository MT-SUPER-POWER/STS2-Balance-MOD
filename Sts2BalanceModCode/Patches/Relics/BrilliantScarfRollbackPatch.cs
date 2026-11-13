using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// RELIC-BRILLIANT-SCARF-REVERT — 艳丽围巾 (Brilliant Scarf) 削弱回退。
/// Target: MegaCrit.Sts2.Core.Models.Relics.BrilliantScarf.get_CanonicalVars.
/// Reason: 官方在 v0.113.0 将触发门槛由第 5 张牌削弱至第 6 张牌，本 Patch 将其回退至第 5 张牌触发。
/// WARNING: Verified against decompiled BrilliantScarf.cs in sts2.dll v0.113.0.
/// </summary>
[HarmonyPatch(typeof(BrilliantScarf), "get_CanonicalVars")]
[Sts2BalanceMod.Sts2BalanceModCode.Settings.BalancePatch("R13")]
public static class BrilliantScarfRollbackPatch
{
  private const int RollbackTriggerThreshold = 5;

  [HarmonyPostfix]
  public static void Postfix(ref IEnumerable<DynamicVar> __result)
  {
    __result = new DynamicVar[]
    {
      new CardsVar(RollbackTriggerThreshold)
    };
  }
}
