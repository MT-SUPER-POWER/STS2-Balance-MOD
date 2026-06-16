using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Events;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// 自定义事件 portrait 路径补丁。
/// 输入：一代回归事件模型。
/// 输出：预加载与实际创建 portrait 时使用 Mod 资源目录下的事件图。
/// </summary>
[HarmonyPatch]
internal static class CustomEventPortraitPatch
{
  [HarmonyPatch(typeof(EventModel), nameof(EventModel.GetAssetPaths))]
  [HarmonyPostfix]
  private static void GetAssetPathsPostfix(EventModel __instance, IRunState runState, ref IEnumerable<string> __result)
  {
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

  private static bool TryGetPortraitPath(EventModel eventModel, out string portraitPath)
  {
    if (eventModel is not (Augmenter or Cleric or CursedTome or MindBloom or TheDivineFountain or TombOfLordRedMask or WheelOfChange))
    {
      portraitPath = string.Empty;
      return false;
    }

    portraitPath = $"events/{eventModel.Id.Entry.ToLowerInvariant()}.png".ImagePath();
    return true;
  }
}
