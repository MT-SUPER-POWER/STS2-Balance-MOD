using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Models;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Encounters;

/// <summary>
/// 修复 MOD 遭遇使用自定义场景路径时，原版 EncounterModel 仍按 ID 拼接 res://scenes/encounters 的问题。
/// </summary>
[HarmonyPatch(typeof(EncounterModel), nameof(EncounterModel.CreateScene))]
internal static class CustomEncounterScenePatch
{
  [HarmonyPrefix]
  private static bool Prefix(EncounterModel __instance, ref Control __result)
  {
    if (__instance is not Sts2EncounterModel { CustomScenePath: { } scenePath })
    {
      return true;
    }

    __result = PreloadManager.Cache.GetScene(scenePath).Instantiate<Control>(PackedScene.GenEditState.Disabled);
    return false;
  }
}
