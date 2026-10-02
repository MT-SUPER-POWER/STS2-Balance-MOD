using HarmonyLib;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// 冒险者尸体精英战斗结算补丁。
/// 确保战后奖励中，系统自动生成的金币与遗物被剔除，保留由事件显式注入的未摸出奖励与卡牌。
/// </summary>
public static class DeadAdventurerCombatPatch
{
  private static readonly HashSet<Type> _encounters =
  [
    typeof(DeadAdventurerNobEncounter),
    typeof(DeadAdventurerLagavulinEncounter),
    typeof(DeadAdventurerSentriesEncounter)
  ];

  [HarmonyPatch(typeof(RewardsSet), nameof(RewardsSet.WithRewardsFromRoom))]
  public static class RewardsPatch
  {
    [HarmonyPostfix]
    private static void Postfix(RewardsSet __result, AbstractRoom room)
    {
      if (room is not CombatRoom combatRoom || combatRoom.Encounter == null)
        return;

      if (!_encounters.Contains(combatRoom.Encounter.GetType()))
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
