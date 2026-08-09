/*
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using Sts2BalanceMod.src.Relics;

namespace Sts2BalanceMod.src.Patches.Events;

/// <summary>
/// 先古之民 Tanx 事件 Patch：
/// 目标类型：MegaCrit.Sts2.Core.Models.Events.Tanx
/// 修改原因：将 Tanx 的先古遗物【爪子】（Claws）替换为【破旧的玩偶】（ShabbyDoll）
/// </summary>
[HarmonyPatch(typeof(Tanx))]
public static class TanxPatch
{
    private static readonly MethodInfo? RelicOptionMethod = AccessTools.Method(
        typeof(AncientEventModel), 
        "RelicOption", 
        [typeof(RelicModel), typeof(string), typeof(string)]
    );

    [HarmonyPatch("AllPossibleOptions", MethodType.Getter)]
    [HarmonyPostfix]
    public static void AllPossibleOptionsPostfix(Tanx __instance, ref IEnumerable<EventOption> __result)
    {
        var shabbyDollModel = ModelDb.Relic<ShabbyDoll>()?.ToMutable();
        if (shabbyDollModel == null || RelicOptionMethod == null)
            return;

        var shabbyOption = RelicOptionMethod.Invoke(__instance, [shabbyDollModel, "INITIAL", null]) as EventOption;
        if (shabbyOption == null)
            return;

        var list = __result.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Relic is Claws)
            {
                list[i] = shabbyOption;
            }
        }
        __result = list;
    }

    [HarmonyPatch("GenerateInitialOptions")]
    [HarmonyPostfix]
    public static void GenerateInitialOptionsPostfix(Tanx __instance, ref IReadOnlyList<EventOption> __result)
    {
        var shabbyDollModel = ModelDb.Relic<ShabbyDoll>()?.ToMutable();
        if (shabbyDollModel == null || RelicOptionMethod == null)
            return;

        var shabbyOption = RelicOptionMethod.Invoke(__instance, [shabbyDollModel, "INITIAL", null]) as EventOption;
        if (shabbyOption == null)
            return;

        var list = __result.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].Relic is Claws)
            {
                list[i] = shabbyOption;
            }
        }
        __result = list;
    }
}
*/
