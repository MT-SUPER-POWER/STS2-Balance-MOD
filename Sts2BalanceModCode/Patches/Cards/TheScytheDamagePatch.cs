using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-05 — 将巨镰的初始伤害从 13 提高至 20。
/// </summary>
/// <remarks>
/// 目标：<see cref="TheScythe"/> 构造函数及其私有 <c>UpdateDamage</c> 方法。
/// 原因：原版在构造时和伤害重算时均硬编码 13；只修改构造结果会使第一次打出后伤害回落。
/// WARNING: 此补丁依赖游戏反编译源码中的私有字段 <c>_currentDamage</c> 和方法
/// <c>UpdateDamage</c>。游戏更新后必须重新核对其实现。
/// </remarks>
[HarmonyPatch]
public static class TheScytheDamagePatch
{
    private const int BaseDamage = 20;

    [HarmonyPatch(typeof(TheScythe), MethodType.Constructor)]
    [HarmonyPostfix]
    private static void ConstructorPostfix(TheScythe __instance)
    {
        SetCurrentDamage(__instance);
    }

    [HarmonyPatch(typeof(TheScythe), "UpdateDamage")]
    [HarmonyPostfix]
    private static void UpdateDamagePostfix(TheScythe __instance)
    {
        SetCurrentDamage(__instance);
    }

    private static void SetCurrentDamage(TheScythe scythe)
    {
        int currentDamage = BaseDamage + scythe.IncreasedDamage;
        Traverse.Create(scythe).Field("_currentDamage").SetValue(currentDamage);
        scythe.DynamicVars["Damage"].BaseValue = currentDamage;
    }
}
