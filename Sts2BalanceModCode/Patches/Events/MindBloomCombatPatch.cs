using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Multiplayer.Replay;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;
using Sts2BalanceMod.Sts2BalanceModCode.Events;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// 心灵绽放 Boss 战斗补丁。
/// 输入：连续事件战斗的同步、回放与奖励状态。
/// 输出：第二战正常初始化回放；第一战只保留事件指定的金币与遗物奖励。
/// </summary>
public static class MindBloomCombatPatch
{
    /// <summary>
    /// 目标：EventSynchronizer.ResumeEvents(AbstractRoom)。
    /// 原因：原版 EventCombatSynchronizer 的单场事件战斗状态在恢复事件后仍保持 ready，
    /// 若同一事件再次开战会抛出 already ready。第一战返回时清空并重新绑定当前事件。
    /// WARNING：依赖反编译确认的私有字段 _combatSynchronizer；游戏更新后需复核字段名与调用时序。
    /// </summary>
    [HarmonyPatch(typeof(EventSynchronizer), nameof(EventSynchronizer.ResumeEvents))]
    public static class ResumeEventsPatch
    {
        [HarmonyPostfix]
        private static void Postfix(
          EventSynchronizer __instance,
          AbstractRoom exitedRoom,
          EventCombatSynchronizer ____combatSynchronizer)
        {
            if (exitedRoom is not CombatRoom { Encounter: MindBloomBossEncounter })
                return;

            ____combatSynchronizer.ResetState();
            ____combatSynchronizer.InitializeForEvent(__instance.GetLocalEvent());
        }
    }

    /// <summary>
    /// 目标：CombatManager.StartCombatInternal(CombatTurnState, Func&lt;Task&gt;)。
    /// 原因：第一战结束会停止并清空 CombatReplayWriter；事件房间内直接进入第二战时，
    /// RunManager.EnterRoomWithoutExitingCurrentRoom 不会像进入新地图点那样重新记录初始状态。
    /// WARNING：依赖反编译确认的私有方法名与第二战启动时序；游戏更新后需复核。
    /// </summary>
    [HarmonyPatch(typeof(CombatManager), "StartCombatInternal")]
    public static class ReplayWriterPatch
    {
        [HarmonyPrefix]
        private static void Prefix()
        {
            if (!MindBloom.NeedsReplayInitialization)
                return;

            MindBloom.NeedsReplayInitialization = false;
            RunManager runManager = RunManager.Instance;
            CombatReplayWriter replayWriter = runManager.CombatReplayWriter;
            if (!replayWriter.IsEnabled || replayWriter.IsRecordingReplay)
                return;

            replayWriter.RecordInitialState(runManager.ToSave(null));
        }
    }

    /// <summary>
    /// 目标：RewardsSet.WithRewardsFromRoom(AbstractRoom)。
    /// 原因：第一战使用事件明确传入的 50 金币与稀有遗物，移除房间类型自动生成的同类奖励；
    /// 第二战保留普通怪物房金币、卡牌与药水概率，因此不进入该过滤逻辑。
    /// WARNING：依赖反编译确认的 ExtraRewards 与 RewardsSet 合并顺序；游戏更新后需复核。
    /// </summary>
    [HarmonyPatch(typeof(RewardsSet), nameof(RewardsSet.WithRewardsFromRoom))]
    public static class RewardsPatch
    {
        [HarmonyPostfix]
        private static void Postfix(RewardsSet __result, AbstractRoom room)
        {
            if (room is not CombatRoom { Encounter: MindBloomBossEncounter } combatRoom)
                return;

            var extraRewards = combatRoom.ExtraRewards.Values
              .SelectMany(list => list)
              .ToHashSet();

            __result.Rewards.RemoveAll(reward =>
              !extraRewards.Contains(reward) &&
              reward is GoldReward or RelicReward);
        }
    }
}
