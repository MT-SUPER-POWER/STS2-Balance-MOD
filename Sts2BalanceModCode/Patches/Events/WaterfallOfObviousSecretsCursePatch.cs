using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Events;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// EVENT-NEW-EVENTS-CURSE-REDUCE — 显秘瀑布 (Waterfall of Obvious Secrets) 诅咒数量调整。
/// Target: MegaCrit.Sts2.Core.Models.Events.WaterfallOfObviousSecrets.get_CanonicalVars.
/// Reason: 暴打选项获得的「内疚」诅咒数量由 2 张减少至 1 张。
/// WARNING: Verified against decompiled WaterfallOfObviousSecrets.cs in sts2.dll v0.113.0.
/// </summary>
[HarmonyPatch(typeof(WaterfallOfObviousSecrets), "get_CanonicalVars")]
[Sts2BalanceMod.Sts2BalanceModCode.Settings.BalancePatch("E18")]
public static class WaterfallOfObviousSecretsCursePatch
{
  [HarmonyPrefix]
  public static bool Prefix(ref IEnumerable<DynamicVar> __result)
  {
    __result = new DynamicVar[]
    {
      new DynamicVar("NegotiateCostKey", 70m),
      new DynamicVar("BeatUpCurseCount", 1m)
    };
    return false;
  }
}
