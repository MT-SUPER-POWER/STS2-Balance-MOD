using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-01 — 能量汲取 (DRAIN_POWER)
/// - 伤害调整到 6/8
/// - 改为升级后，升级所有弃牌堆的所有手牌（所有卡牌）
/// </summary>
[HarmonyPatch(typeof(DrainPower))]
public static class DrainPowerPatch
{
    [HarmonyPatch("get_CanonicalVars")]
    [HarmonyPrefix]
    public static bool GetCanonicalVarsPrefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[2]
        {
            new DamageVar(6m, ValueProp.Move),
            new CardsVar(2)
        };
        return false;
    }

    [HarmonyPatch("OnUpgrade")]
    [HarmonyPrefix]
    public static bool OnUpgradePrefix(DrainPower __instance)
    {
        __instance.DynamicVars.Damage.UpgradeValueBy(2m);
        // 不升级 Cards 变量，保持基础值 2
        return false;
    }

    [HarmonyPatch("OnPlay")]
    [HarmonyPrefix]
    public static bool OnPlayPrefix(DrainPower __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        __result = OnPlayAsync(__instance, choiceContext, cardPlay);
        return false;
    }

    private static async Task OnPlayAsync(DrainPower __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        System.ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        // 1. 造成伤害
        await DamageCmd.Attack(__instance.DynamicVars.Damage.BaseValue)
            .FromCard(__instance, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // 2. 升级弃牌堆中的卡牌
        var discardPile = PileType.Discard.GetPile(__instance.Owner);
        List<CardModel> upgradableCards;

        if (__instance.IsUpgraded)
        {
            // 升级弃牌堆中的所有可升级卡牌
            upgradableCards = discardPile.Cards.Where(c => c.IsUpgradable).ToList();
        }
        else
        {
            // 随机升级弃牌堆中指定数量（2张）的可升级卡牌
            upgradableCards = discardPile.Cards
                .Where(c => c.IsUpgradable)
                .TakeRandom(__instance.DynamicVars.Cards.IntValue, __instance.Owner.RunState.Rng.CombatCardSelection)
                .ToList();
        }

        foreach (CardModel card in upgradableCards)
        {
            CardCmd.Upgrade(card);
            CardCmd.Preview(card);
        }
    }
}
