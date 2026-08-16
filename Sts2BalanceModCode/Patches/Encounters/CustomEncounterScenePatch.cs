using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Models;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Encounters;

/// <summary>
/// 修复 MOD 遭遇使用自定义场景路径时，原版 EncounterModel 仍按 ID 拼接 res://scenes/encounters 的问题。
/// 支持 BalanceEncounterTemplate 的自定义场景以及 MindBloomBossEncounter 委托的一层 Boss 场景。
/// </summary>
[HarmonyPatch(typeof(EncounterModel), nameof(EncounterModel.CreateScene))]
public static class CustomEncounterScenePatch
{
    [HarmonyPrefix]
    public static bool Prefix(EncounterModel __instance, ref Control __result)
    {
        // 1. 心灵绽放第一战：如果委托的原版 Boss 有独立场景（如同族小队 the_kin_boss），直接实例化其实际场景
        if (__instance is MindBloomBossEncounter { BossEncounter: { HasScene: true } bossEncounter })
        {
            __result = bossEncounter.CreateScene();
            return false;
        }

        // 2. MOD 自定义遭遇：从 AssetProfile 的 EncounterScenePath 实例化场景
        if (__instance is BalanceEncounterTemplate { AssetProfile.EncounterScenePath: { } scenePath } &&
            !string.IsNullOrEmpty(scenePath))
        {
            __result = PreloadManager.Cache.GetScene(scenePath).Instantiate<Control>(PackedScene.GenEditState.Disabled);
            return false;
        }

        return true;
    }
}
