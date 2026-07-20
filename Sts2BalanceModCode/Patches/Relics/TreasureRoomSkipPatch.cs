using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Runs;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// RELIC-01 — 宝箱房添加跳过选项（仅在单人模式下）。
/// 1. NTreasureRoom._Ready Postfix: 单人模式下强行启用继续按钮。
/// 2. NProceedButton.Disable Prefix: 单人模式下拦截宝箱房继续按钮的禁用逻辑。
/// </summary>
[HarmonyPatch]
public static class TreasureRoomSkipPatch
{
    [HarmonyPatch(typeof(NTreasureRoom), "_Ready")]
    [HarmonyPostfix]
    public static void NTreasureRoomReadyPostfix(NTreasureRoom __instance)
    {
        if (typeof(RunManager)
            .GetProperty("State", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(RunManager.Instance) is IRunState runState && runState.Players.Count == 1)
        {
            NProceedButton proceedButton = __instance.GetNodeOrNull<NProceedButton>("%ProceedButton");
            if (proceedButton != null)
            {
                proceedButton.Enable();
                proceedButton.Visible = true;
            }
        }
    }

    [HarmonyPatch(typeof(NProceedButton), nameof(NProceedButton.Disable))]
    [HarmonyPrefix]
    public static bool NProceedButtonDisablePrefix(NProceedButton __instance)
    {
        if (typeof(RunManager)
            .GetProperty("State", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(RunManager.Instance) is IRunState runState && runState.Players.Count == 1)
        {
            Godot.Node parent = __instance.GetParent();
            if (parent is NTreasureRoom)
            {
                __instance.Enable();
                __instance.Visible = true;
                return false; // 拦截原版 Disable 方法，使其保持启用状态
            }
        }
        return true; // 允许正常 Disable
    }
}
