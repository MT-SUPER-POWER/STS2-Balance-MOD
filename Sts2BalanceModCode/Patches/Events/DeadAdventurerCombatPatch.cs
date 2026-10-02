using System.Reflection;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Events;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// 冒险者尸体事件补丁：
/// 1. 独立 Exordium 背景 (.tscn) 实例化与置换；
/// 2. 遭遇战站位场景插槽配合与怪物潜伏控制；
/// 3. 地面尸体独立图层显隐（ZIndex 0，避免被遮挡）；
/// 4. 战利品正确结算。
/// </summary>
public static class DeadAdventurerCombatPatch
{
  private static readonly HashSet<Type> _encounters =
  [
    typeof(DeadAdventurerNobEncounter),
    typeof(DeadAdventurerLagavulinEncounter),
    typeof(DeadAdventurerSentriesEncounter)
  ];

  private const string ExordiumBgScenePath = "res://Sts2BalanceMod/scenes/backgrounds/exordium_background.tscn";

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

      // 1. 替换二代默认背景：隐藏原生 Background，实例化 Exordium 背景 .tscn
      if (__instance.Background != null)
      {
        __instance.Background.Visible = false;
      }

      if (__instance.GetNodeOrNull<Control>("ExordiumBackgroundNode") == null)
      {
        PackedScene? bgScene = GD.Load<PackedScene>(ExordiumBgScenePath);
        if (bgScene != null)
        {
          Control bgInstance = bgScene.Instantiate<Control>();
          bgInstance.Name = "ExordiumBackgroundNode";
          bgInstance.ZIndex = -50;

          Control? bgContainer = __instance.GetNodeOrNull<Control>("%BgContainer");
          if (bgContainer != null)
          {
            bgContainer.AddChild(bgInstance);
          }
          else
          {
            __instance.AddChild(bgInstance);
            __instance.MoveChild(bgInstance, 0);
          }
        }
      }

      // 2. 如果当前处于事件搜查阶段（VisualOnly）
      if (__instance.Mode == CombatRoomMode.VisualOnly)
      {
        // 初始潜伏状态：隐藏怪物容器
        Control? enemyContainer = __instance.GetNodeOrNull<Control>("%EnemyContainer");
        if (enemyContainer != null)
        {
          enemyContainer.Visible = false;
        }

        // 挂载独立的冒险者尸体覆盖层（ZIndex 0，确保在 Exordium 地面之上）
        if (__instance.GetNodeOrNull<Control>("DeadAdventurerBodyOverlay") == null)
        {
          Texture2D? texture = GD.Load<Texture2D>(ModAssetPaths.Resource("images", "events", "DeadAdventurer.png"));
          if (texture != null)
          {
            TextureRect overlay = new()
            {
              Name = "DeadAdventurerBodyOverlay",
              Texture = texture,
              StretchMode = TextureRect.StretchModeEnum.Scale,
              MouseFilter = Control.MouseFilterEnum.Ignore,
              OffsetTop = 40f,
              ZIndex = 0
            };
            overlay.SetAnchorsPreset(Control.LayoutPreset.FullRect);
            __instance.AddChild(overlay);

            // 同步场景震动
            Control? sceneContainer = __instance.GetNodeOrNull<Control>("%CombatSceneContainer");
            if (sceneContainer != null)
            {
              Vector2 basePos = sceneContainer.Position;
              sceneContainer.Connect(
                "item_rect_changed",
                Callable.From(() =>
                {
                  if (GodotObject.IsInstanceValid(overlay) && GodotObject.IsInstanceValid(sceneContainer))
                  {
                    Vector2 delta = sceneContainer.Position - basePos;
                    overlay.OffsetTop = 40f + delta.Y;
                    overlay.OffsetLeft = delta.X;
                  }
                }));
            }
          }
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

    Control? overlay = combatRoom.GetNodeOrNull<Control>("DeadAdventurerBodyOverlay");
    if (overlay != null)
      overlay.Visible = false;
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
