using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-09 / CARD-14 — 回调火箭飞拳 (Rocket Punch) 耗能降为 0 费
/// 每当生成 Status 牌时，该张火箭飞拳的费用直接降为 0 费（在下一次打出前生效）。
/// 当该张火箭飞拳打出后，其单张牌的 0 费效果随打出清空，下次抽到恢复 2 费。
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
