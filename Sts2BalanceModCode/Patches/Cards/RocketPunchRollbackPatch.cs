using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-14 — 回调火箭飞拳 (Rocket Punch) 耗能逻辑修正
/// 每当生成 Status 牌时，获得 1 次下一张打出的火箭飞拳 0 费的额度（单次生效、不累加）。
/// 打出任意 1 张火箭飞拳后立即消耗该额度，其余火箭飞拳恢复基础 2 费。
/// </summary>
public static class RocketPunchRollbackPatch
{
    private static readonly HashSet<Player> PlayersWithFreePunch = new();

    public static bool HasFreePunch(Player player)
    {
        return PlayersWithFreePunch.Contains(player);
    }

    public static void SetPlayerHasFreePunch(Player player, bool hasFreePunch)
    {
        if (hasFreePunch)
        {
            PlayersWithFreePunch.Add(player);
        }
        else
        {
            PlayersWithFreePunch.Remove(player);
        }
    }

    [HarmonyPatch(typeof(RocketPunch), nameof(RocketPunch.AfterCardGeneratedForCombat))]
    public static class AfterCardGeneratedPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(RocketPunch __instance, CardModel card, Player creator, ref Task __result)
        {
            if (creator != __instance.Owner || card.Owner != __instance.Owner || card.Type != CardType.Status)
            {
                __result = Task.CompletedTask;
                return false;
            }

            SetPlayerHasFreePunch(__instance.Owner, true);
            __result = Task.CompletedTask;
            return false;
        }
    }

    [HarmonyPatch(typeof(CardEnergyCost), nameof(CardEnergyCost.GetWithModifiers))]
    public static class EnergyCostPatch
    {
        private static readonly FieldInfo CardField = AccessTools.Field(typeof(CardEnergyCost), "_card");

        [HarmonyPostfix]
        public static void Postfix(CardEnergyCost __instance, CostModifiers modifiers, ref int __result)
        {
            if (CardField?.GetValue(__instance) is RocketPunch rocketPunch && rocketPunch.Owner != null)
            {
                if (HasFreePunch(rocketPunch.Owner))
                {
                    __result = 0;
                }
            }
        }
    }

    [HarmonyPatch(typeof(RocketPunch), "OnPlay")]
    public static class OnPlayPatch
    {
        [HarmonyPrefix]
        public static void Prefix(RocketPunch __instance)
        {
            if (__instance.Owner != null)
            {
                SetPlayerHasFreePunch(__instance.Owner, false);
            }
        }
    }
}
