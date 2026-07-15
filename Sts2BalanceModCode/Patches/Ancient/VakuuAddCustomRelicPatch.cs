using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Ancient;

/// <summary>
/// RELIC-02: 把自定义遗物注入瓦库的静态 _validRelicSets
/// Patch AllPossibleOptions getter
/// </summary>
[HarmonyPatch(typeof(Vakuu))]
[HarmonyPatch(nameof(Vakuu.AllPossibleOptions), MethodType.Getter)]
internal static class VakuuAddCustomRelicPatch
{
    private static bool _registered = false;

    [HarmonyPrefix]
    private static bool Prefix()
    {
        if (_registered)
            return true;

        try
        {
            // 反射获取静态字段 _validRelicSets
            var field = typeof(Vakuu).GetField("_validRelicSets", BindingFlags.Static | BindingFlags.NonPublic);
            if (field == null)
            {
                MainFile.Logger.Info("[VakuuAddCustomRelicPatch] 找不到 _validRelicSets 字段");
                return true;
            }

            var list = (System.Collections.IList?)field.GetValue(null);
            if (list == null)
            {
                MainFile.Logger.Info("[VakuuAddCustomRelicPatch] _validRelicSets 为 null");
                return true;
            }

            MainFile.Logger.Info($"[VakuuAddCustomRelicPatch] 原始 _validRelicSets 数量={list.Count}");

            // 获取嵌套 struct ValidRelicSet
            Type? structType = typeof(Vakuu).GetNestedType("ValidRelicSet", BindingFlags.NonPublic);
            if (structType == null)
            {
                MainFile.Logger.Info("[VakuuAddCustomRelicPatch] 找不到 ValidRelicSet 类型");
                return true;
            }

            // WARNING: 不能 new，用 ModelDb.Relic<T>() 获取已注册的 canonical 实例
            var relicsToAdd = new (string Name, RelicModel Relic)[]
            {
        ("SoulContract", ModelDb.Relic<SoulContract>()),
            };

            foreach (var (name, relic) in relicsToAdd)
            {
                var set = Activator.CreateInstance(structType, new object[] { new RelicModel[] { relic } });
                list.Add(set);
                MainFile.Logger.Info($"[VakuuAddCustomRelicPatch] 成功注册 {name}");
            }

            _registered = true;
            MainFile.Logger.Info($"[VakuuAddCustomRelicPatch] 注册完成, _validRelicSets 数量={list.Count}");
        }
        catch (System.Exception ex)
        {
            MainFile.Logger.Info($"[VakuuAddCustomRelicPatch] 异常: {ex.GetType().Name} — {ex.Message}\n{ex.StackTrace}");
        }

        return true;
    }
}
