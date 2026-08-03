using BaseLib.Extensions;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Events;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// 自定义事件 portrait 路径补丁与防拉伸处理。
/// 1. 自动定位 Mod 资源目录下的自定义事件背景图（消除硬编码白名单）。
/// 2. Postfix 设置 TextureRect.StretchMode 为 KeepAspectCovered，防止事件图像发生非等比拉伸扭曲。
/// </summary>
[HarmonyPatch]
internal static class CustomEventPortraitPatch
{
    [HarmonyPatch(typeof(EventModel), nameof(EventModel.GetAssetPaths))]
    [HarmonyPostfix]
    private static void GetAssetPathsPostfix(EventModel __instance, IRunState runState, ref IEnumerable<string> __result)
    {
        if (__instance is OldBeggar)
        {
            __result = __result.Append("events/cleric.png".ImagePath());
        }

        if (!TryGetPortraitPath(__instance, out var portraitPath))
        {
            return;
        }

        var defaultPath = $"res://images/events/{__instance.Id.Entry.ToLowerInvariant()}.png";
        __result = __result.Select(path => path == defaultPath ? portraitPath : path);
    }

    [HarmonyPatch(typeof(EventModel), nameof(EventModel.CreateInitialPortrait))]
    [HarmonyPrefix]
    private static bool CreateInitialPortraitPrefix(EventModel __instance, ref Texture2D __result)
    {
        if (!TryGetPortraitPath(__instance, out var portraitPath))
        {
            return true;
        }

        __result = PreloadManager.Cache.GetTexture2D(portraitPath);
        return false;
    }

    [HarmonyPatch(typeof(NEventLayout), nameof(NEventLayout.SetPortrait))]
    [HarmonyPostfix]
    private static void SetPortraitAspectFixPostfix(NEventLayout __instance)
    {
        var portraitNode = __instance.GetNodeOrNull<TextureRect>("%Portrait");
        if (portraitNode != null)
        {
            portraitNode.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
            portraitNode.StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered;
        }
    }

    private static bool TryGetPortraitPath(EventModel eventModel, out string portraitPath)
    {
        var candidatePath = $"events/{eventModel.Id.Entry.RemovePrefix().ToLowerInvariant()}.png".ImagePath();
        if (ResourceLoader.Exists(candidatePath))
        {
            portraitPath = candidatePath;
            return true;
        }

        portraitPath = string.Empty;
        return false;
    }
}
