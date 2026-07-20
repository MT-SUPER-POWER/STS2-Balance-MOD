using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// RELIC-01 — 宝箱房添加跳过选项（仅在单人模式下）。
/// 1. NTreasureRoom._Ready Postfix: 单人模式下强行启用继续按钮。
/// 2. NClickableControl.Disable Prefix: 单人模式下拦截宝箱房继续按钮的禁用逻辑。
/// </summary>
[HarmonyPatch]
public static class TreasureRoomSkipPatch
{
  [HarmonyPatch(typeof(NTreasureRoom), "_Ready")]
  [HarmonyPostfix]
  public static void NTreasureRoomReadyPostfix(NTreasureRoom __instance)
  {
    var runState = typeof(RunManager)
        .GetProperty("State", BindingFlags.NonPublic | BindingFlags.Instance)
        ?.GetValue(RunManager.Instance) as IRunState;

    if (runState != null && runState.Players.Count == 1)
    {
      var proceedButton = __instance.GetNodeOrNull<NProceedButton>("%ProceedButton");
      if (proceedButton != null)
      {
        proceedButton.Enable();
        proceedButton.Visible = true;
      }
    }
  }

  [HarmonyPatch(typeof(NClickableControl), "Disable")]
  [HarmonyPrefix]
  public static bool NClickableControlDisablePrefix(NClickableControl __instance)
  {
    if (__instance is NProceedButton proceedButton)
    {
      var runState = typeof(RunManager)
          .GetProperty("State", BindingFlags.NonPublic | BindingFlags.Instance)
          ?.GetValue(RunManager.Instance) as IRunState;

      if (runState != null && runState.Players.Count == 1)
      {
        var parent = proceedButton.GetParent();
        if (parent is NTreasureRoom)
        {
          proceedButton.Enable();
          proceedButton.Visible = true;
          return false; // 拦截原版 Disable 方法，使其保持启用状态
        }
      }
    }
    return true; // 允许正常 Disable
  }
}
