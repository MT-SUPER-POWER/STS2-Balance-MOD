using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Multiplayer.Game;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// 心灵绽放 Boss 战斗补丁。
/// 输入：战斗房间奖励与自定义遭遇房间类型。
/// 输出：心灵绽放战斗只保留事件指定的金币与遗物奖励。
/// </summary>
public static class MindBloomCombatPatch
{
  private static bool IsMindBloomEncounter(EncounterModel encounter) =>
    encounter is MindBloomBossEncounter or
      MindBloomGuardian or
      MindBloomHexaghost or
      MindBloomSlimeBoss;

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
  /// 目标：RewardsSet.WithRewardsFromRoom(AbstractRoom)。
  /// 原因：心灵绽放两场战斗使用事件明确传入的金币/遗物，移除房间类型自动生成的同类奖励。
  /// WARNING：依赖反编译确认的 ExtraRewards 与 RewardsSet 合并顺序；游戏更新后需复核。
  /// </summary>
  [HarmonyPatch(typeof(RewardsSet), nameof(RewardsSet.WithRewardsFromRoom))]
  public static class RewardsPatch
  {
    [HarmonyPostfix]
    private static void Postfix(RewardsSet __result, AbstractRoom room)
    {
      if (room is not CombatRoom combatRoom)
        return;
      if (!IsMindBloomEncounter(combatRoom.Encounter))
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
