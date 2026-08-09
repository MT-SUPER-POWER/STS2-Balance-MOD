using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

[HarmonyPatch(typeof(TrashHeap), "get_Relics")]
public static class TrashHeapRelicsPatch
{
    [HarmonyPostfix]
    private static void Postfix(ref RelicModel[] __result)
    {
        __result = __result
            .Concat(new RelicModel[]
            {
          ModelDb.Relic<Omamori>(),
            })
            .ToArray();
    }
}
