using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// RELIC-SANDCASTLE-01 — 先古遗物「沙堡」调整：选择升级3张牌，随机升级3张牌。
/// Target: MegaCrit.Sts2.Core.Models.Relics.SandCastle.AfterObtained
/// Reason: 原版拾起时直接随机升级 6 张牌；重做为先由玩家自选 3 张牌升级，随后在剩余可升级牌中随机升级 3 张。
/// WARNING: Verified against D:\Game\Sts2Code\src\MegaCrit.Sts2.Core.Models.Relics\SandCastle.cs; game updates may change this decompiled implementation.
/// </summary>
[HarmonyPatch(typeof(SandCastle), "AfterObtained")]
public static class SandCastlePatch
{
    [HarmonyPrefix]
    public static bool Prefix(SandCastle __instance, ref Task __result)
    {
        __result = AfterObtainedImpl(__instance);
        return false;
    }

    private static async Task AfterObtainedImpl(SandCastle instance)
    {
        var player = instance.Owner;
        if (player == null)
            return;

        // 1. 玩家自选 3 张可升级牌进行升级
        var chosenCards = (await CardSelectCmd.FromDeckForUpgrade(
            player: player,
            prefs: new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, 3)
        )).ToList();

        foreach (var card in chosenCards)
        {
            CardCmd.Upgrade(card);
        }

        // 2. 从牌组中剩余的可升级牌中随机挑选 3 张升级
        var remainingUpgradable = PileType.Deck.GetPile(player).Cards
            .Where(c => c?.IsUpgradable ?? false)
            .ToList()
            .StableShuffle(player.RunState.Rng.Niche)
            .Take(3)
            .ToList();

        NRun.Instance?.GlobalUi.GridCardPreviewContainer.ForceMaxColumnsUntilEmpty(3);
        foreach (var card in remainingUpgradable)
        {
            CardCmd.Upgrade(card, CardPreviewStyle.GridLayout);
        }
    }
}
