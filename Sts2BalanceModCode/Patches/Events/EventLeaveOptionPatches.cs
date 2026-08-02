using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using Sts2BalanceMod.Sts2BalanceModCode.Config;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events
{
  /// <summary>
  /// 为 Bugslayer、TinkerTime 和 TheFutureOfPotions 事件新增“离开”选项的补丁。
  /// </summary>
  public static class EventLeaveOptionPatches
  {
    private static readonly MethodInfo SetEventFinishedMethod = AccessTools.Method(typeof(EventModel), "SetEventFinished", new[] { typeof(LocString) });

    [HarmonyPatch(typeof(Bugslayer), "GenerateInitialOptions")]
    public static class BugslayerPatch
    {
      [HarmonyPostfix]
      private static void Postfix(Bugslayer __instance, ref IReadOnlyList<EventOption> __result)
      {
        if (!BalanceModConfig.EnableEventLeaveOptions)
        {
          return;
        }

        var list = new List<EventOption>(__result);
        
        Func<Task> leaveAction = async () =>
        {
          SetEventFinishedMethod.Invoke(__instance, new object[] { new LocString("events", "BUGSLAYER.pages.LEAVE.description") });
          await Task.CompletedTask;
        };

        list.Add(new EventOption(__instance, leaveAction, "BUGSLAYER.pages.INITIAL.options.LEAVE"));
        __result = list.AsReadOnly();
      }
    }

    [HarmonyPatch(typeof(TinkerTime), "GenerateInitialOptions")]
    public static class TinkerTimePatch
    {
      [HarmonyPostfix]
      private static void Postfix(TinkerTime __instance, ref IReadOnlyList<EventOption> __result)
      {
        if (!BalanceModConfig.EnableEventLeaveOptions)
        {
          return;
        }

        var list = new List<EventOption>(__result);
        
        Func<Task> leaveAction = async () =>
        {
          SetEventFinishedMethod.Invoke(__instance, new object[] { new LocString("events", "TINKER_TIME.pages.LEAVE.description") });
          await Task.CompletedTask;
        };

        list.Add(new EventOption(__instance, leaveAction, "TINKER_TIME.pages.INITIAL.options.LEAVE"));
        __result = list.AsReadOnly();
      }
    }

    [HarmonyPatch(typeof(TheFutureOfPotions), "GenerateInitialOptions")]
    public static class TheFutureOfPotionsPatch
    {
      [HarmonyPostfix]
      private static void Postfix(TheFutureOfPotions __instance, ref IReadOnlyList<EventOption> __result)
      {
        if (!BalanceModConfig.EnableEventLeaveOptions)
        {
          return;
        }

        var list = new List<EventOption>(__result);
        
        Func<Task> leaveAction = async () =>
        {
          SetEventFinishedMethod.Invoke(__instance, new object[] { new LocString("events", "THE_FUTURE_OF_POTIONS.pages.LEAVE.description") });
          await Task.CompletedTask;
        };

        list.Add(new EventOption(__instance, leaveAction, "THE_FUTURE_OF_POTIONS.pages.INITIAL.options.LEAVE"));
        __result = list.AsReadOnly();
      }
    }
  }
}
