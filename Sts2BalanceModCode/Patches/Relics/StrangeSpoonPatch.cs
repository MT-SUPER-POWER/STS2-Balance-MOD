using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// RELIC-STRANGE-SPOON-01 — 商店遗物「奇怪的汤勺」：打出应消耗的牌有 50% 几率进入弃牌堆，【凋萎】必定消耗。
/// Target: MegaCrit.Sts2.Core.Models.CardModel.GetResultLocationForCardPlay
/// Reason: 当卡牌在打出后原定进入消耗堆（PileType.Exhaust）时，检查玩家是否持有 StrangeSpoon。若是 Wither（凋萎）则必定消耗；否则 50% 几率改为进入弃牌堆（PileType.Discard）。
/// WARNING: Verified against D:\Game\Sts2Code\src\MegaCrit.Sts2.Core.Models\CardModel.cs; game updates may change this decompiled implementation.
/// </summary>
[HarmonyPatch(typeof(CardModel), "GetResultLocationForCardPlay")]
public static class StrangeSpoonPatch
{
    [HarmonyPostfix]
    public static void Postfix(CardModel __instance, ref CardLocation __result)
    {
        // 仅当卡牌原本要进入消耗堆时触发
        if (__result.pileType != PileType.Exhaust)
            return;

        // 【凋萎】(Wither) 100% 必定消耗，不受汤勺效果保护
        if (__instance is Wither)
            return;

        var owner = __instance.Owner;
        if (owner == null)
            return;

        var spoon = owner.Relics.OfType<StrangeSpoon>().FirstOrDefault();
        if (spoon != null)
        {
            // 50% 几率进入弃牌堆
            if (owner.RunState.Rng.CombatCardSelection.NextBool())
            {
                __result = new CardLocation(__result.player, PileType.Discard, __result.position);
                spoon.Flash();
            }
        }
    }
}
