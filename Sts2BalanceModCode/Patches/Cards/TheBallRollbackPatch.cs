using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// Target: TheBall.get_CanonicalVars.
/// Reason: CARD-01 restores The Ball's per-play growth from 10/15 to 15/20.
/// WARNING: Target and values are based on decompiled game source; do not modify that source directly.
/// </summary>
[HarmonyPatch(typeof(TheBall), "get_CanonicalVars")]
public static class TheBallRollbackPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result =
        [
            new DamageVar(10m, ValueProp.Move),
            new DynamicVar("Increase", 15m)
        ];
        return false;
    }
}
