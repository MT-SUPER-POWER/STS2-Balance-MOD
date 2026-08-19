using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Powers;

/// <summary>
/// CARD-COOLANT-01 — 冷却剂能力（CoolantPower）效果重做
/// Target: CoolantPower.AfterSideTurnStart, AbstractModel.AfterCardPlayed, PowerModel.get_ExtraHoverTips
/// Reason: 移除每回合按不同充能球数量加格挡的旧效果；改为打出能力牌时抽 Amount 张牌（未升级 1 张，升级后 2 张）。
/// WARNING: Verified against D:\Game\Sts2Code\src\MegaCrit.Sts2.Core.Models.Powers\CoolantPower.cs.
/// </summary>
[HarmonyPatch]
public static class CoolantPowerPatch
{
    private static readonly Action<PowerModel>? FlashPower = 
        AccessTools.MethodDelegate<Action<PowerModel>>(AccessTools.Method(typeof(PowerModel), "Flash"));

    [HarmonyPatch(typeof(CoolantPower), nameof(CoolantPower.AfterSideTurnStart))]
    [HarmonyPrefix]
    public static bool AfterSideTurnStartPrefix(ref Task __result)
    {
        // 屏蔽原版充能球格挡逻辑
        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.AfterCardPlayed))]
    [HarmonyPostfix]
    public static async Task AfterCardPlayedPostfix(AbstractModel __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        if (__instance is not CoolantPower power)
            return;

        if (power.Owner?.Player == null)
            return;

        if (cardPlay.Player == power.Owner.Player && cardPlay.Card.Type == CardType.Power)
        {
            FlashPower?.Invoke(power);
            await CardPileCmd.Draw(choiceContext, power.Amount, power.Owner.Player);
        }
    }

    [HarmonyPatch(typeof(PowerModel), "get_ExtraHoverTips")]
    [HarmonyPrefix]
    public static bool ExtraHoverTipsPrefix(PowerModel __instance, ref IEnumerable<IHoverTip> __result)
    {
        if (__instance is not CoolantPower)
            return true;

        __result = Array.Empty<IHoverTip>();
        return false;
    }
}
