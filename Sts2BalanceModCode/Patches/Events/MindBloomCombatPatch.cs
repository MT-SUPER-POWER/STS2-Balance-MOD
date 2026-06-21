using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;
using Sts2BalanceMod.Sts2BalanceModCode.Events;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// 心灵绽放 Boss 战斗补丁。
/// 输入：战斗房间奖励与自定义遭遇房间类型。
/// 输出：心灵绽放战斗按 Boss 房处理，并只保留事件指定的金币与遗物奖励。
/// </summary>
public static class MindBloomCombatPatch
{
  private static readonly HashSet<Type> MindBloomEncounters =
  [
    typeof(MindBloomGuardian),
    typeof(MindBloomHexaghost),
    typeof(MindBloomSlimeBoss),
  ];

  [HarmonyPatch(typeof(RewardsSet), nameof(RewardsSet.WithRewardsFromRoom))]
  public static class RewardsPatch
  {
    [HarmonyPostfix]
    private static void Postfix(RewardsSet __result, AbstractRoom room)
    {
      if (room is not CombatRoom combatRoom)
        return;
      if (!MindBloomEncounters.Contains(combatRoom.Encounter.GetType()))
        return;

      var extraRewards = combatRoom.ExtraRewards.Values
        .SelectMany(list => list)
        .ToHashSet();

      __result.Rewards.RemoveAll(reward =>
        !extraRewards.Contains(reward) &&
        reward is GoldReward or RelicReward);
    }
  }

  [HarmonyPatch(typeof(EncounterModel), nameof(EncounterModel.RoomType), MethodType.Getter)]
  public static class RoomTypePatch
  {
    [HarmonyPostfix]
    private static void Postfix(EncounterModel __instance, ref RoomType __result)
    {
      if (MindBloom.CombatActive && MindBloomEncounters.Contains(__instance.GetType()))
      {
        __result = RoomType.Boss;
      }
    }
  }
}
