using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// 先古之民 Tanx 事件 Patch：
/// 目标类型：MegaCrit.Sts2.Core.Models.Events.Tanx
/// 目标方法：GenerateInitialOptions
/// 修改原因：在 Tanx 的先古遗物选择项中包含【破旧的玩偶】（ShabbyDoll）
/// </summary>
[HarmonyPatch(typeof(Tanx), "GenerateInitialOptions")]
public static class TanxPatch
{
    private static readonly MethodInfo? RelicOptionMethod = AccessTools.Method(
        typeof(AncientEventModel), 
        "RelicOption", 
        [typeof(RelicModel), typeof(string), typeof(string)]
    );

    [HarmonyPostfix]
    public static void Postfix(Tanx __instance, ref IReadOnlyList<EventOption> __result)
    {
        var shabbyDollModel = ModelDb.Relic<ShabbyDoll>()?.ToMutable();
        if (shabbyDollModel == null || RelicOptionMethod == null)
            return;

        var option = RelicOptionMethod.Invoke(__instance, [shabbyDollModel, "INITIAL", null]) as EventOption;
        if (option == null)
            return;

        var list = __result.ToList();
        if (list.Count > 0)
        {
            list[0] = option;
        }
        else
        {
            list.Add(option);
        }

        __result = list;
    }
}
