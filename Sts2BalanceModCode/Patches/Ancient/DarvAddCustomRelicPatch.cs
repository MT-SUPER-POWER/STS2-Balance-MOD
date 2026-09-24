using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Ancient;

/// <summary>
/// RELIC-07: 把自定义遗物注入达夫的静态 _validRelicSets
/// Patch AllPossibleOptions getter — 图鉴和实际事件都会调，且不会提前触发 Darv..cctor()
/// </summary>
[HarmonyPatch(typeof(Darv))]
[HarmonyPatch(nameof(Darv.AllPossibleOptions), MethodType.Getter)]
internal static class DarvAddCustomRelicPatch
{
  private static bool _registered;

  [HarmonyPrefix]
  private static bool Prefix()
  {
    if (_registered)
      return true;

    try
    {
      // 反射获取静态字段 _validRelicSets
      FieldInfo? field = typeof(Darv).GetField("_validRelicSets", BindingFlags.Static | BindingFlags.NonPublic);
      if (field == null)
      {
        BalanceModEntry.Logger.Info("[DarvAddCustomRelicPatch] 找不到 _validRelicSets 字段");
        return true;
      }

      var list = (System.Collections.IList?)field.GetValue(null);
      if (list == null)
      {
        BalanceModEntry.Logger.Info("[DarvAddCustomRelicPatch] _validRelicSets 为 null");
        return true;
      }

      BalanceModEntry.Logger.Info($"[DarvAddCustomRelicPatch] 原始 _validRelicSets 数量={list.Count}");

      // 获取嵌套 struct ValidRelicSet
      Type? structType = typeof(Darv).GetNestedType("ValidRelicSet", BindingFlags.NonPublic);
      if (structType == null)
      {
        BalanceModEntry.Logger.Info("[DarvAddCustomRelicPatch] 找不到 ValidRelicSet 类型");
        return true;
      }

      // WARNING: 不能 new，用 ModelDb.Relic<T>() 获取已注册的 canonical 实例
      var relicsToAdd = new (string Name, RelicModel Relic)[]
      {
        ("CoffieCup", ModelDb.Relic<CoffieCup>()),
        ("FusionHammer", ModelDb.Relic<FusionHammer>()),
        ("CurseKey", ModelDb.Relic<CurseKey>()),
      };

      foreach ((string? name, RelicModel? relic) in relicsToAdd)
      {
        if (!Settings.BalanceContent.IsEnabled(relic.GetType()))
          continue;
        // WARNING: Darv filters ValidRelicSet before drawing; it does not call relic.IsAllowed.
        Func<Player, bool> allowed = player => relic.IsAllowed(player.RunState) &&
          (relic is not CurseKey || player.RunState.Players.Count == 1);
        object? set = Activator.CreateInstance(structType, [allowed, new RelicModel[] { relic }]);
        list.Add(set);
        BalanceModEntry.Logger.Info($"[DarvAddCustomRelicPatch] 成功注册 {name}");
      }

      _registered = true;
      BalanceModEntry.Logger.Info($"[DarvAddCustomRelicPatch] 注册完成, _validRelicSets 数量={list.Count}");
    }
    catch (System.Exception ex)
    {
      BalanceModEntry.Logger.Info($"[DarvAddCustomRelicPatch] 异常: {ex.GetType().Name} — {ex.Message}\n{ex.StackTrace}");
    }

    return true;
  }
}
