using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// Targets: CardModel.get_Rarity, CardModel.get_CanonicalVars,
/// WellLaidPlans.OnUpgrade, WellLaidPlans.OnPlay, and
/// WellLaidPlansPower.BeforeSideTurnEnd and WellLaidPlansPower.ShouldFlush.
/// Reason: CARD-03 restores the 1-cost Uncommon card and its 1/2-card Retain effect,
/// while preserving the game's current multiplayer availability.
/// WARNING: Hook order and signatures are verified against decompiled game source; do not modify that source directly.
/// </summary>
[HarmonyPatch]
public static class WellLaidPlansRollbackPatch
{
    // The original card inherits both properties from CardModel, so both
    // prefixes must target CardModel rather than a nonexistent derived getter.
    [HarmonyPatch(typeof(CardModel), "get_Rarity")]
    [HarmonyPrefix]
    public static bool RarityPrefix(CardModel __instance, ref CardRarity __result)
    {
        if (__instance is not WellLaidPlans)
            return true;

        __result = CardRarity.Uncommon;
        return false;
    }

    [HarmonyPatch(typeof(CardModel), "get_CanonicalVars")]
    [HarmonyPrefix]
    public static bool CanonicalVarsPrefix(CardModel __instance, ref IEnumerable<DynamicVar> __result)
    {
        if (__instance is not WellLaidPlans)
            return true;

        __result = [new CardsVar(1)];
        return false;
    }

    [HarmonyPatch(typeof(WellLaidPlans), "OnUpgrade")]
    [HarmonyPrefix]
    public static bool OnUpgradePrefix(WellLaidPlans __instance)
    {
        __instance.DynamicVars["Cards"].UpgradeValueBy(1m);
        __instance.EnergyCost.UpgradeBy(-1);
        return false;
    }

    [HarmonyPatch(typeof(WellLaidPlans), "OnPlay")]
    [HarmonyPrefix]
    public static bool OnPlayPrefix(
        WellLaidPlans __instance,
        PlayerChoiceContext choiceContext,
        CardPlay cardPlay,
        ref Task __result)
    {
        __result = ApplyRetainPower(__instance, choiceContext);
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), "BeforeSideTurnEnd")]
    [HarmonyPrefix]
    public static bool BeforeSideTurnEndPrefix(
        AbstractModel __instance,
        PlayerChoiceContext choiceContext,
        CombatSide side,
        IEnumerable<Creature> participants,
        ref Task __result)
    {
        if (__instance is not WellLaidPlansPower power)
            return true;

        __result = participants.Contains(power.Owner)
            ? RetainCards(power, choiceContext)
            : Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(WellLaidPlansPower), nameof(WellLaidPlansPower.ShouldFlush))]
    [HarmonyPrefix]
    public static bool ShouldFlushPrefix(
        WellLaidPlansPower __instance,
        Player player,
        ref bool __result)
    {
        if (player != __instance.Owner.Player)
            return true;

        // The current game power returns false here and retains the entire hand.
        // Restored behavior must let the normal flush retain only selected cards.
        __result = true;
        return false;
    }

    private static async Task ApplyRetainPower(WellLaidPlans card, PlayerChoiceContext choiceContext)
    {
        await CreatureCmd.TriggerAnim(
            card.Owner.Creature,
            "PowerUp",
            card.Owner.Character.PowerUpAnimDelay);
        await PowerCmd.Apply<WellLaidPlansPower>(
            choiceContext,
            card.Owner.Creature,
            card.DynamicVars["Cards"].BaseValue,
            card.Owner.Creature,
            card);
    }

    private static async Task RetainCards(WellLaidPlansPower power, PlayerChoiceContext choiceContext)
    {
        Player? player = power.Owner.Player;
        if (player == null)
            return;

        var prefs = new CardSelectorPrefs(
            new LocString("cards", "WELL_LAID_PLANS.selectionScreenPrompt"),
            0,
            power.Amount);
        IEnumerable<CardModel> selectedCards = await CardSelectCmd.FromHand(
            choiceContext,
            player,
            prefs,
            c => !c.ShouldRetainThisTurn,
            power);

        foreach (CardModel card in selectedCards)
            CardCmd.ApplySingleTurnRetain(card);
    }
}
