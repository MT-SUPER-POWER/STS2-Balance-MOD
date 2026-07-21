using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// Target: HistoryCourse.AfterAutoPrePlayPhaseEntered.
/// Reason: RELIC-01 restores replaying the last Attack or Skill from the owner's previous turn.
/// WARNING: The copied base-game flow is verified from decompiled source; do not modify that source directly.
/// </summary>
[HarmonyPatch(typeof(HistoryCourse), nameof(HistoryCourse.AfterAutoPrePlayPhaseEntered))]
public static class HistoryCourseRollbackPatch
{
    [HarmonyPrefix]
    public static bool Prefix(
        HistoryCourse __instance,
        PlayerChoiceContext choiceContext,
        Player player,
        ref Task __result)
    {
        __result = ReplayLastAttackOrSkill(__instance, choiceContext, player);
        return false;
    }

    private static async Task ReplayLastAttackOrSkill(
        HistoryCourse relic,
        PlayerChoiceContext choiceContext,
        Player player)
    {
        var owner = relic.Owner;
        if (player != owner || owner.PlayerCombatState?.TurnNumber <= 1)
            return;

        CardModel? card = CombatManager.Instance.History.CardPlaysFinished
            .LastOrDefault(entry =>
                entry.CardPlay.Player == owner &&
                entry.HappenedLastPlayerTurn(owner) &&
                entry.CardPlay.Card.Type is CardType.Attack or CardType.Skill &&
                !entry.CardPlay.Card.IsDupe)
            ?.CardPlay.Card;

        if (card == null)
            return;

        relic.Flash();
        await CardCmd.AutoPlay(choiceContext, card.CreateDupe(player), null);
    }
}
