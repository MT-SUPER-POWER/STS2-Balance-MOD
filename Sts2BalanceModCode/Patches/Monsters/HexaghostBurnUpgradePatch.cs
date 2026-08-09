using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Monsters;

/// <summary>
/// 仅在 Hexaghost 的 Inferno/后续 Sear 生成强化灼伤时，临时开放 Burn 的一次升级。
/// WARNING: 目标签名来自当前游戏 API 与 AFP 1.0.5 反编译依赖；游戏升级后必须重新核对。
/// </summary>
[HarmonyPatch(typeof(Burn))]
internal static class HexaghostBurnUpgradePatch
{
    internal static bool AllowBurnUpgrade { get; set; }

    /// <summary>
    /// 目标：Burn.MaxUpgradeLevel getter。
    /// 原因：原版 Burn 不可升级，而 Hexaghost 的 Inferno 必须把场内灼伤升级一次。
    /// </summary>
    [HarmonyPatch(nameof(Burn.MaxUpgradeLevel), MethodType.Getter)]
    [HarmonyPostfix]
    private static void MaxUpgradeLevelPostfix(ref int __result)
    {
        if (AllowBurnUpgrade)
            __result = 1;
    }

    /// <summary>
    /// 目标：CardModel.UpgradeInternal。
    /// 原因：为 Hexaghost 强化的 Burn 增加 AFP 原版规定的 2 点伤害。
    /// </summary>
    [HarmonyPatch(typeof(CardModel), nameof(CardModel.UpgradeInternal))]
    [HarmonyPostfix]
    private static void UpgradeInternalPostfix(CardModel __instance)
    {
        if (!AllowBurnUpgrade || __instance is not Burn { IsUpgraded: true } burn)
            return;

        burn.DynamicVars.Damage.UpgradeValueBy(2M);
    }
}
