using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-08 — 将巨镰的初始伤害调整为 16 点（配合官方增至 5(7) 的成长数值）。
/// </summary>
[HarmonyPatch]
public static class TheScytheDamagePatch
{
    private const int BaseDamage = 16;


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
