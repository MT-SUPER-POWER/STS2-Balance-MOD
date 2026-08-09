using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// RELIC-02 — 烘焙手套 (Toasty Mittens)
/// 官方最新版 Hook 为 AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)。
/// Mod 效果：允许在抽牌后从手牌选择 1 张消耗（提供 Skip 跳过选项）；
/// 只有成功消耗卡牌时才获得 1 点力量，若 Skip 跳过则不加力量。
/// </summary>
[HarmonyPatch(typeof(ToastyMittens), nameof(ToastyMittens.AfterPlayerTurnStart))]
public static class ToastyMittensPatch
{
    [HarmonyPrefix]
    public static bool Prefix(
        ToastyMittens __instance,
        PlayerChoiceContext choiceContext,
        Player player,
        ref Task __result)
    {
        if (player != __instance.Owner.Creature.Player)
        {
            __result = Task.CompletedTask;
            return false;
        }

        __result = ProcessToastyMittens(__instance, player, choiceContext);
        return false;
    }

    private static async Task ProcessToastyMittens(ToastyMittens relic, Player player, PlayerChoiceContext choiceContext)
    {
        IReadOnlyList<CardModel> handCards = PileType.Hand.GetPile(player).Cards;
        if (handCards.Count == 0)
        {
            return;
        }

        var prefs = new CardSelectorPrefs(
            CardSelectorPrefs.ExhaustSelectionPrompt,
            0,
            1);

        IEnumerable<CardModel> selectedCards = await CardSelectCmd.FromHand(
            choiceContext,
            player,
            prefs,
            null,
            relic);

        CardModel? cardToExhaust = selectedCards.FirstOrDefault();
        if (cardToExhaust != null)
        {
            relic.Flash();
            await CardCmd.Exhaust(choiceContext, cardToExhaust);
            await PowerCmd.Apply<StrengthPower>(
                choiceContext,
                player.Creature,
                relic.DynamicVars.Strength.BaseValue,
                player.Creature,
                null);
        }
    }
}
