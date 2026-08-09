using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// Targets: DiamondDiadem.AfterSideTurnStart, get_CanonicalVars, get_ExtraHoverTips,
/// BeforeSideTurnEnd, and ModifyDamageMultiplicative.
/// Reason: RELIC-02 replaces the new Block/Blur effect with half damage from enemies
/// after the owner played two or fewer cards on their previous turn.
/// WARNING: Target hooks and combat-history semantics are verified against decompiled source; do not modify that source directly.
/// </summary>
[HarmonyPatch]
public static class DiamondDiademRollbackPatch
{
    private sealed class TurnState
    {
        public bool HalveEnemyDamage;
    }

    private static readonly ConditionalWeakTable<DiamondDiadem, TurnState> _turnStates = new();

    [HarmonyPatch(typeof(DiamondDiadem), nameof(DiamondDiadem.AfterSideTurnStart))]
    [HarmonyPrefix]
    public static bool AfterSideTurnStartPrefix(ref Task __result)
    {
        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(DiamondDiadem), "get_CanonicalVars")]
    [HarmonyPrefix]
    public static bool CanonicalVarsPrefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = Array.Empty<DynamicVar>();
        return false;
    }

    [HarmonyPatch(typeof(DiamondDiadem), "get_ExtraHoverTips")]
    [HarmonyPrefix]
    public static bool ExtraHoverTipsPrefix(ref IEnumerable<IHoverTip> __result)
    {
        __result = Array.Empty<IHoverTip>();
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
        if (__instance is not DiamondDiadem relic)
            return true;

        if (side == CombatSide.Player && participants.Contains(relic.Owner.Creature))
        {
            _turnStates.GetOrCreateValue(relic).HalveEnemyDamage =
                CombatManager.Instance.History.CardPlaysFinished.Count(entry =>
                    entry.CardPlay.Player == relic.Owner &&
                    entry.HappenedThisTurn(relic.Owner.Creature.CombatState)) <= 2;
        }

        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(AbstractModel), "ModifyDamageMultiplicative")]
    [HarmonyPrefix]
    public static bool ModifyDamageMultiplicativePrefix(
        AbstractModel __instance,
        Creature target,
        decimal amount,
        ValueProp props,
        Creature dealer,
        CardModel cardSource,
        CardPlay cardPlay,
        ref decimal __result)
    {
        if (__instance is not DiamondDiadem relic)
            return true;

        __result = ShouldHalveEnemyDamage(relic, target, dealer) ? 0.5m : 1m;
        return false;
    }

    private static bool ShouldHalveEnemyDamage(DiamondDiadem relic, Creature target, Creature dealer)
    {
        if (target != relic.Owner.Creature ||
            dealer?.Side != CombatSide.Enemy ||
            target.CombatState?.CurrentSide != CombatSide.Enemy)
            return false;

        return _turnStates.TryGetValue(relic, out var state) && state.HalveEnemyDamage;
    }
}
