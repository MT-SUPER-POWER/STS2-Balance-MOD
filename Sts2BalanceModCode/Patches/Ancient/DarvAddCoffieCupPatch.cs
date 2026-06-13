using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches;

/// <summary>
/// DARV-01: 把 CoffieCup（咖啡杯）加入达夫的选项池
/// </summary>
[HarmonyPatch(typeof(Darv), nameof(Darv.AllPossibleOptions), MethodType.Getter)]
internal static class DarvAddCoffieCupPatch
{
  [HarmonyPostfix]
  private static void Postfix(ref IEnumerable<EventOption> __result, Darv __instance)
  {
    var list = __result.ToList();

    // 1) 创建遗物实例
    var coffieCup = new CoffieCup();

    // 2) 反射调用 protected 方法 RelicOption(RelicModel, string, string?)
    // 反射调用 AncientEventModel.RelicOption(RelicModel relic, string pageName, string? customDonePage)
    var relicOptionMethod = typeof(AncientEventModel)
        .GetMethod("RelicOption", BindingFlags.Instance | BindingFlags.NonPublic,
            new[] { typeof(RelicModel), typeof(string), typeof(string) });

    if (relicOptionMethod != null)
    {
      var option = (EventOption)relicOptionMethod.Invoke(__instance,
        new object[] { coffieCup, "INITIAL", null });

      if (option != null)
        list.Add(option);
    }

    __result = list;
  }
}
