using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.src.Relics;

namespace Sts2BalanceMod.src.Patches.Relics;

/// <summary>
/// RELIC-01 — 诅咒钥匙只在单人模式下出现。
/// 在多人模式下，从 EventRelicPool 与 SharedRelicPool 过滤移除 CurseKey。
/// </summary>
[HarmonyPatch(typeof(EventRelicPool), "GenerateAllRelics")]
public static class EventPoolCurseKeyFilterPatch
{
  [HarmonyPostfix]
  public static IEnumerable<RelicModel> Postfix(IEnumerable<RelicModel> __result)
  {
    var runState = typeof(RunManager)
        .GetProperty("State", BindingFlags.NonPublic | BindingFlags.Instance)
        ?.GetValue(RunManager.Instance) as IRunState;

    if (runState != null && runState.Players.Count > 1)
    {
      return __result.Where(r => r is not CurseKey);
    }
    return __result;
  }
}

[HarmonyPatch(typeof(SharedRelicPool), "GenerateAllRelics")]
public static class SharedPoolCurseKeyFilterPatch
{
  [HarmonyPostfix]
  public static IEnumerable<RelicModel> Postfix(IEnumerable<RelicModel> __result)
  {
    var runState = typeof(RunManager)
        .GetProperty("State", BindingFlags.NonPublic | BindingFlags.Instance)
        ?.GetValue(RunManager.Instance) as IRunState;

    if (runState != null && runState.Players.Count > 1)
    {
      return __result.Where(r => r is not CurseKey);
    }
    return __result;
  }
}
