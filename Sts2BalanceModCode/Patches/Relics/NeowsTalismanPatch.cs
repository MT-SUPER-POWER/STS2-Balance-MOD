using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// RELIC-NEOWS-TALISMAN-01 — 涅奥遗物「涅奥的护符」（Neow's Talisman）重做为快速开局遗物（涅奥的悲哀）
/// Targets:
/// - NeowsTalisman.AfterObtained
/// - RelicModel.get_HasUponPickupEffect, get_ShowCounter, get_DisplayAmount, get_IsUsedUp, get_Status
/// - Creature.SetUniqueMonsterHpValue, Creature.ScaleMonsterHpForMultiplayer
/// - CombatRoom.StartCombat
/// Reason: 取消原版升级1攻1防效果；改为使接下来3场战斗中的所有敌人初始生命值为1点，耗尽后置灰失效。
/// WARNING: Verified against D:\Game\Sts2Code\src\MegaCrit.Sts2.Core.Models.Relics\NeowsTalisman.cs.
/// </summary>
[HarmonyPatch]
public static class NeowsTalismanPatch
{
    private const int TotalLamentCombats = 3;

    [HarmonyPatch(typeof(NeowsTalisman), nameof(NeowsTalisman.AfterObtained))]
    [HarmonyPrefix]
    public static bool AfterObtainedPrefix(ref Task __result)
    {
        // 取消原版升级打击和防御的效果
        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch(typeof(RelicModel), "get_HasUponPickupEffect")]
    [HarmonyPrefix]
    public static bool HasUponPickupEffectPrefix(RelicModel __instance, ref bool __result)
    {
        if (__instance is NeowsTalisman)
        {
            __result = false;
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(RelicModel), "get_ShowCounter")]
    [HarmonyPrefix]
    public static bool ShowCounterPrefix(RelicModel __instance, ref bool __result)
    {
        if (__instance is NeowsTalisman talisman)
        {
            __result = !GetIsUsedUp(talisman);
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(RelicModel), "get_DisplayAmount")]
    [HarmonyPrefix]
    public static bool DisplayAmountPrefix(RelicModel __instance, ref int __result)
    {
        if (__instance is NeowsTalisman talisman)
        {
            __result = Math.Max(0, GetRemainingCharges(talisman));
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(RelicModel), "get_IsUsedUp")]
    [HarmonyPrefix]
    public static bool IsUsedUpPrefix(RelicModel __instance, ref bool __result)
    {
        if (__instance is NeowsTalisman talisman)
        {
            __result = GetIsUsedUp(talisman);
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(RelicModel), "get_Status")]
    [HarmonyPrefix]
    public static bool StatusPrefix(RelicModel __instance, ref RelicStatus __result)
    {
        if (__instance is NeowsTalisman talisman && GetIsUsedUp(talisman))
        {
            __result = RelicStatus.Disabled;
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(Creature), nameof(Creature.SetUniqueMonsterHpValue))]
    [HarmonyPostfix]
    public static void SetUniqueMonsterHpValuePostfix(Creature __instance)
    {
        if (__instance.Side == CombatSide.Enemy && IsLamentActiveForCreature(__instance))
        {
            __instance.SetMaxHpInternal(1m);
            __instance.SetCurrentHpInternal(1m);
        }
    }

    [HarmonyPatch(typeof(Creature), nameof(Creature.ScaleMonsterHpForMultiplayer))]
    [HarmonyPrefix]
    public static bool ScaleMonsterHpForMultiplayerPrefix(Creature __instance)
    {
        if (__instance.Side == CombatSide.Enemy && IsLamentActiveForCreature(__instance))
        {
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(CombatRoom), "StartCombat")]
    [HarmonyPostfix]
    public static void StartCombatPostfix(CombatRoom __instance)
    {
        if (__instance.CombatState == null)
            return;

        bool hasLament = false;
        foreach (var player in __instance.CombatState.Players)
        {
            var talisman = player.Relics.OfType<NeowsTalisman>().FirstOrDefault();
            if (talisman != null && IsLamentActive(talisman))
            {
                talisman.Flash();
                hasLament = true;
            }
        }

        if (hasLament)
        {
            foreach (var enemy in __instance.CombatState.Enemies)
            {
                enemy.SetMaxHpInternal(1m);
                enemy.SetCurrentHpInternal(1m);
            }
        }
    }

    public static int GetFinishedCombats(NeowsTalisman talisman)
    {
        if (!talisman.IsMutable)
            return 0;

        if (talisman.Owner?.RunState is not RunState runState || runState.MapPointHistory == null)
            return 0;

        int floor = 0;
        int combats = 0;
        foreach (var act in runState.MapPointHistory)
        {
            foreach (var entry in act)
            {
                floor++;
                if (floor >= talisman.FloorAddedToDeck)
                {
                    if (entry.Rooms.Any(r => r.RoomType == RoomType.Monster || r.RoomType == RoomType.Elite || r.RoomType == RoomType.Boss || r.MonsterIds.Count > 0))
                    {
                        combats++;
                    }
                }
            }
        }

        if (CombatManager.Instance.IsInProgress && combats > 0)
        {
            combats--;
        }

        return combats;
    }

    public static int GetRemainingCharges(NeowsTalisman talisman)
    {
        int finished = GetFinishedCombats(talisman);
        return Math.Max(0, TotalLamentCombats - finished);
    }

    public static bool GetIsUsedUp(NeowsTalisman talisman)
    {
        return GetRemainingCharges(talisman) <= 0;
    }

    public static bool IsLamentActive(NeowsTalisman talisman)
    {
        return !GetIsUsedUp(talisman);
    }

    private static bool IsLamentActiveForCreature(Creature creature)
    {
        var combatState = creature.CombatState;
        if (combatState == null)
            return false;

        foreach (var player in combatState.Players)
        {
            var talisman = player.Relics.OfType<NeowsTalisman>().FirstOrDefault();
            if (talisman != null && IsLamentActive(talisman))
            {
                return true;
            }
        }
        return false;
    }
}
