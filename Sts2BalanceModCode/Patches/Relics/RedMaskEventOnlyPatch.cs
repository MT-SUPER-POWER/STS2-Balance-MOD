using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// RELIC-08 — 红面具从一般共享遗物池移除，只通过红面具相关事件获得。
/// 同时加到 EventRelicPool 中确保能够在遗物图鉴中正常显示。
/// </summary>

[HarmonyPatch(typeof(SharedRelicPool), "GenerateAllRelics")]
public static class RedMaskRemoveFromSharedPatch
{
  [HarmonyPostfix]
  public static RelicModel[] Postfix(RelicModel[] __result)
  {
    return __result.Where(r => r is not RedMask).ToArray();
  }
}

[HarmonyPatch(typeof(EventRelicPool), "GenerateAllRelics")]
public static class RedMaskAddToEventPoolPatch
{
  [HarmonyPostfix]
  public static IEnumerable<RelicModel> Postfix(IEnumerable<RelicModel> __result)
  {
    // NOTE: 将 RedMask 加入到 EventRelicPool，使其在遗物图鉴中可见
    return __result.Append(ModelDb.Relic<RedMask>());
  }
}
