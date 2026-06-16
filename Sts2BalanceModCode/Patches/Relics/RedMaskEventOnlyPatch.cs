using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// RELIC-08 — 红面具从一般共享遗物池移除，只通过红面具相关事件获得。
/// </summary>
[HarmonyPatch(typeof(SharedRelicPool), "GenerateAllRelics")]
public static class RedMaskEventOnlyPatch
{
  [HarmonyPostfix]
  public static RelicModel[] Postfix(RelicModel[] __result)
  {
    return __result.Where(r => r is not RedMask).ToArray();
  }
}
