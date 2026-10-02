using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// 冒险者尸体事件补丁：
/// 1. 配合独立 Encounter 站位场景 (.tscn) 与 Exordium 背景；
/// 2. 隐藏二代默认背景，控制怪物潜伏与地面尸体显隐；
/// 3. 战斗胜利后结算未搜刮完的全部剩余战利品。
/// </summary>
public static class DeadAdventurerCombatPatch
{
  private static readonly HashSet<Type> _encounters =
  [
    typeof(DeadAdventurerNobEncounter),
    typeof(DeadAdventurerLagavulinEncounter),
    typeof(DeadAdventurerSentriesEncounter)
  ];

  [HarmonyPatch(typeof(NCombatRoom), nameof(NCombatRoom._Ready))]
  public static class VisualsPatch
  {
    [HarmonyPostfix]
    public static void Postfix(NCombatRoom __instance)
    {
      FieldInfo? visualsField = typeof(NCombatRoom).GetField(
        "_visuals",
        BindingFlags.NonPublic | BindingFlags.Instance);
      ICombatRoomVisuals? visuals = visualsField?.GetValue(__instance) as ICombatRoomVisuals;

      if (visuals?.Encounter == null)
        return;

      if (!_encounters.Contains(visuals.Encounter.GetType()))
        return;

      // 1. 隐藏二代原版背景与容器，使 Encounter 场景中的 Exordium 一代背景完全呈现
      Control? bgContainer = __instance.GetNodeOrNull<Control>("%BgContainer");
      if (bgContainer != null)
      {
        bgContainer.Visible = false;
      }
      if (__instance.Background != null)
      {
        __instance.Background.Visible = false;
      }

      // 2. 如果当前处于事件搜查阶段（VisualOnly）
      if (__instance.Mode == CombatRoomMode.VisualOnly)
      {
        // 初始潜伏状态：隐藏怪物容器，展示地上的尸体
        Control? enemyContainer = __instance.GetNodeOrNull<Control>("%EnemyContainer");
        if (enemyContainer != null)
        {
          enemyContainer.Visible = false;
        }

        Control? body = __instance.GetNodeOrNull<Control>("%DeadAdventurerBody");
        if (body != null)
        {
          body.Visible = true;
        }
      }
    }
  }

  /// <summary>
  /// 搜寻惊醒怪物时调用：怪物显形、地上尸体隐藏。
  /// </summary>
  public static void RevealEnemies()
  {
    if (NEventRoom.Instance?.Layout is not NCombatEventLayout layout)
      return;

    NCombatRoom? combatRoom = layout.EmbeddedCombatRoom;
    if (combatRoom == null)
      return;

    Control? enemyContainer = combatRoom.GetNodeOrNull<Control>("%EnemyContainer");
    if (enemyContainer != null)
      enemyContainer.Visible = true;

    Control? body = combatRoom.GetNodeOrNull<Control>("%DeadAdventurerBody");
    if (body != null)
      body.Visible = false;
  }

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
