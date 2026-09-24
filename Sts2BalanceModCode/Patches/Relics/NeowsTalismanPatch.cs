using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.RelicPools;
using CustomNeowsTalisman = Sts2BalanceMod.Sts2BalanceModCode.Relics.NeowsTalisman;
using VanillaNeowsTalisman = MegaCrit.Sts2.Core.Models.Relics.NeowsTalisman;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// RELIC-NEOWS-TALISMAN-01 & RELIC-NEOWS-TALISMAN-02
/// 将原版涅奥的护符（Neow's Talisman）重做项替换为自定义遗物模型 CustomNeowsTalisman：
/// 1. 涅奥开局选项中提供 CustomNeowsTalisman
/// 2. RelicCmd.Obtain 拦截原版遗物并替换为自定义遗物
/// 3. EventRelicPool 中过滤移除原版遗物
/// </summary>
[HarmonyPatch]
[Sts2BalanceMod.Sts2BalanceModCode.Settings.BalancePatch("R10")]
public static class NeowsTalismanPatch
{
  private static readonly MethodInfo _relicOptionMethod = typeof(AncientEventModel).GetMethod(
    "RelicOption",
    BindingFlags.Instance | BindingFlags.NonPublic,
    [typeof(RelicModel), typeof(string), typeof(string)]
  ) ?? throw new InvalidOperationException("Could not find AncientEventModel.RelicOption method via reflection.");

  [HarmonyPatch(typeof(Neow), "get_NeowsTalismanOption")]
  [HarmonyPrefix]
  public static bool NeowsTalismanOptionPrefix(Neow __instance, ref EventOption __result)
  {
    var customRelic = (CustomNeowsTalisman)ModelDb.Relic<CustomNeowsTalisman>().ToMutable();
    __result = (EventOption)_relicOptionMethod.Invoke(__instance, [customRelic, "INITIAL", "NEOW.pages.DONE.POSITIVE.description"])!;
    return false;
  }

  [HarmonyPatch(typeof(RelicCmd), nameof(RelicCmd.Obtain), typeof(RelicModel), typeof(Player), typeof(int))]
  [HarmonyPrefix]
  public static void RelicCmdObtainPrefix(ref RelicModel relic)
  {
    if (relic is VanillaNeowsTalisman)
    {
      relic = (CustomNeowsTalisman)ModelDb.Relic<CustomNeowsTalisman>().ToMutable();
    }
  }

  [HarmonyPatch(typeof(EventRelicPool), "GenerateAllRelics")]
  [HarmonyPostfix]
  public static IEnumerable<RelicModel> EventPoolFilterPostfix(IEnumerable<RelicModel> __result) => __result.Where(r => r is not VanillaNeowsTalisman);
}
