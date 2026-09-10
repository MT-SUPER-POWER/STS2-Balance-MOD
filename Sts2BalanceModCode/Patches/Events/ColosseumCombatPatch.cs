using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Replay;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;
using Sts2BalanceMod.Sts2BalanceModCode.Events;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// EVENT-COLOSSEUM-01 — 竞技场双阶段战斗补丁。
/// 保证第一战结束后重置 EventCombatSynchronizer；
/// 第二战开始时初始化 CombatReplayWriter；
/// 并在第二战结算时移除默认精英掉落的多余遗物和金币，只保留事件传入的稀有遗物、罕见遗物、100 金币与卡牌奖励。
/// </summary>
public static class ColosseumCombatPatch
{
    [HarmonyPatch(typeof(EventSynchronizer), nameof(EventSynchronizer.ResumeEvents))]
    public static class ResumeEventsPatch
    {
        [HarmonyPostfix]
        private static void Postfix(
          EventSynchronizer __instance,
          AbstractRoom exitedRoom,
          EventCombatSynchronizer ____combatSynchronizer)
        {
            if (exitedRoom is not CombatRoom { Encounter: ColosseumFirstEncounter })
                return;

            ____combatSynchronizer.ResetState();
            ____combatSynchronizer.InitializeForEvent(__instance.GetLocalEvent());
        }
    }

    [HarmonyPatch(typeof(CombatManager), "StartCombatInternal")]
    public static class ReplayWriterPatch
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            if (!Colosseum.NeedsReplayInitialization)
                return;

            Colosseum.NeedsReplayInitialization = false;
            RunManager runManager = RunManager.Instance;
            CombatReplayWriter replayWriter = runManager.CombatReplayWriter;
            if (!replayWriter.IsEnabled || replayWriter.IsRecordingReplay)
                return;

            replayWriter.RecordInitialState(runManager.ToSave(null));
        }
    }

    [HarmonyPatch(typeof(RewardsSet), nameof(RewardsSet.WithRewardsFromRoom))]
    public static class RewardsPatch
    {
        [HarmonyPostfix]
        private static void Postfix(RewardsSet __result, AbstractRoom room)
        {
            if (room is not CombatRoom { Encounter: ColosseumSecondEncounter } combatRoom)
                return;

            HashSet<Reward> extraRewards = combatRoom.ExtraRewards.Values
              .SelectMany(list => list)
              .ToHashSet();

            __result.Rewards.RemoveAll(r =>
              !extraRewards.Contains(r) &&
              r is GoldReward or RelicReward);
        }
    }

    [HarmonyPatch(typeof(EncounterModel), nameof(EncounterModel.CreateBackground))]
    public static class CombatBackgroundPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(EncounterModel __instance, ref NCombatBackground __result)
        {
            if (__instance is ColosseumFirstEncounter or ColosseumSecondEncounter)
            {
                var background = new TheCityBackground();
                background.Name = "TheCityActBackground";

                for (int i = 0; i < 4; i++)
                {
                    var layer = new Godot.Control();
                    layer.Name = $"Layer_{i:D2}";
                    background.AddChild(layer);
                }

                var foreground = new Godot.Control();
                foreground.Name = "Foreground";
                background.AddChild(foreground);

                background.TreeEntered += background.OnTreeEntered;
                __result = background;
                return false;
            }

            return true;
        }
    }
}

