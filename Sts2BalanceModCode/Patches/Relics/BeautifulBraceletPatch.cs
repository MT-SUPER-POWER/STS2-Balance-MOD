using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// RELIC-05 — 华美手镯 (Beautiful Bracelet).
/// Target: MegaCrit.Sts2.Core.Models.Relics.BeautifulBracelet.AfterObtained.
/// Reason: 将原版随机 4 张牌附魔迅捷 2 改为由玩家自选 4 张牌附魔迅捷 2。
/// WARNING: Verified against D:\Game\Sts2Code\src\MegaCrit.Sts2.Core.Models.Relics\BeautifulBracelet.cs.
/// </summary>
[HarmonyPatch(typeof(BeautifulBracelet), nameof(BeautifulBracelet.AfterObtained))]
public static class BeautifulBraceletPatch
{
    [HarmonyPrefix]
    public static bool Prefix(BeautifulBracelet __instance, ref Task __result)
    {
        __result = ProcessAfterObtained(__instance);
        return false;
    }

    private static async Task ProcessAfterObtained(BeautifulBracelet relic)
    {
        Swift swift = ModelDb.Enchantment<Swift>();
        int swiftAmount = relic.DynamicVars["Swift"].IntValue;
        int cardsCount = relic.DynamicVars.Cards.IntValue;

        IEnumerable<CardModel> selectedCards = await CardSelectCmd.FromDeckForEnchantment(
            prefs: new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, cardsCount),
            player: relic.Owner,
            enchantment: swift,
            amount: swiftAmount);

        foreach (CardModel item in selectedCards)
        {
            CardCmd.Enchant<Swift>(item, swiftAmount);
            NCardEnchantVfx? nCardEnchantVfx = NCardEnchantVfx.Create(item);
            if (nCardEnchantVfx != null)
            {
                NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(nCardEnchantVfx);
            }
        }
    }
}
