using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Powers;

/// <summary>
/// CARD-04: BiasedCognition（认知偏差）
/// 原本：永久每回合 -1 聚焦
/// 改为：聚焦归零时自动移除该能力（修复结束回合按钮消失的 BUG）
/// </summary>
[HarmonyPatch(typeof(BiasedCognitionPower), nameof(BiasedCognitionPower.AfterSideTurnStart))]
public static class BiasedCognitionPowerPatch
{
    public static bool Prefix(BiasedCognitionPower __instance, CombatSide side, IReadOnlyList<Creature> participants)
    {
        // 不是这个生物的主场回合就不管
        if (!participants.Contains(__instance.Owner))
            return true;

        // 聚焦已归零 → 移除能力（修复结束回合按钮消失的 BUG）
        if (!HasFocus(__instance.Owner))
        {
            __instance.RemoveInternal();
            return false;
        }

        return true; // 继续原方法（扣 1 聚焦）
    }

    /// <summary>
    /// 聚焦数值变化时检查：如果归零 + 身上有 BiasedCognition，移除
    /// 处理回合中卡牌消耗聚焦导致归零的情况
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PowerModel), nameof(PowerModel.SetAmount))]
    public static void OnPowerSetAmount(PowerModel __instance)
    {
        if (__instance is not FocusPower focus || focus.Amount > 0)
            return;

        var biased = focus.Owner?.GetPower<BiasedCognitionPower>();
        if (biased != null)
            biased.RemoveInternal();
    }

    // ======================== HELPERS ========================

    private static bool HasFocus(Creature owner)
    {
        var focus = owner.GetPower<FocusPower>();
        return focus != null && focus.Amount > 0;
    }
}
