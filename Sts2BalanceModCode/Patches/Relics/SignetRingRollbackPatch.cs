using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// RELIC-03 — 图章戒指 (Signet Ring) 获得金币回调为 999
/// </summary>
[HarmonyPatch(typeof(SignetRing), nameof(SignetRing.AfterObtained))]
public static class SignetRingRollbackPatch
{
    [HarmonyPrefix]
    public static bool Prefix(SignetRing __instance, ref Task __result)
    {
        __result = PlayerCmd.GainGold(999, __instance.Owner);
        return false;
    }
}
