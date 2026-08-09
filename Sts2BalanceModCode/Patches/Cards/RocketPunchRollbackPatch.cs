using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;


namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-09 — 回调火箭飞拳 (Rocket Punch) 耗能直接降至 0 费
/// 确保每当生成 Status 牌时，火箭飞拳的费用直接降为 0 费（而不是仅仅 -1 费）。
/// </summary>
[HarmonyPatch(typeof(RocketPunch), nameof(RocketPunch.AfterCardGeneratedForCombat))]
public static class RocketPunchRollbackPatch
{
    [HarmonyPrefix]
    public static bool Prefix(RocketPunch __instance, CardModel card, Player creator, ref Task __result)
    {
        if (creator != __instance.Owner || card.Owner != __instance.Owner || card.Type != CardType.Status)
        {
            __result = Task.CompletedTask;
            return false;
        }

        __instance.EnergyCost.SetUntilPlayed(0);
        __result = Task.CompletedTask;
        return false;
    }
}
