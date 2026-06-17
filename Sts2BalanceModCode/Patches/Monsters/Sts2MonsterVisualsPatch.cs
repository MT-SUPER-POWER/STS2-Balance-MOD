using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Monsters;

/// <summary>
/// 修复 Godot 导出无法解析游戏本体 NCreatureVisuals 脚本时，MOD 怪物场景根节点退化成 Node2D 的问题。
/// 输入：本 MOD 的怪物模型。
/// 输出：始终返回可被游戏强转的 NCreatureVisuals 根节点。
/// </summary>
[HarmonyPatch(typeof(MonsterModel), nameof(MonsterModel.CreateVisuals))]
internal static class Sts2MonsterVisualsPatch
{
  [HarmonyPrefix]
  private static bool Prefix(MonsterModel __instance, ref NCreatureVisuals __result)
  {
    if (__instance is not Sts2MonsterModel monster)
    {
      return true;
    }

    var scene = PreloadManager.Cache.GetScene(monster.ModVisualsPath);
    var node = scene.Instantiate<Node2D>(PackedScene.GenEditState.Disabled);

    if (node is NCreatureVisuals visuals)
    {
      __result = visuals;
      return false;
    }

    __result = WrapNode2D(node);
    return false;
  }

  private static NCreatureVisuals WrapNode2D(Node2D source)
  {
    var visuals = new NCreatureVisuals
    {
      Name = source.Name,
      Position = source.Position,
      Rotation = source.Rotation,
      Scale = source.Scale,
      Skew = source.Skew,
      Visible = source.Visible,
      ZIndex = source.ZIndex,
      YSortEnabled = source.YSortEnabled,
    };

    foreach (var child in source.GetChildren())
    {
      source.RemoveChild(child);
      visuals.AddChild(child);
      SetOwnerRecursive(child, visuals);
    }

    source.QueueFree();
    return visuals;
  }

  private static void SetOwnerRecursive(Node node, Node owner)
  {
    node.Owner = owner;

    foreach (var child in node.GetChildren())
    {
      SetOwnerRecursive(child, owner);
    }
  }
}
