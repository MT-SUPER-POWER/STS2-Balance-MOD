using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Models.Events;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;
using MegaCrit.Sts2.Core.Entities.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Ancient;

/// <summary>
/// RELIC-02: 瓦库事件 Pool1 选项定制补丁
/// 仅在图鉴模式下强制注入“灵魂契约”，在正常对局中保持原有的随机池行为。
/// </summary>
[HarmonyPatch(typeof(Vakuu), "Pool1", MethodType.Getter)]
public static class VakuuPoolPatch
{
    [HarmonyPostfix]
    private static void Postfix(Vakuu __instance, ref IEnumerable<EventOption> __result)
    {
        // 通过反射获取 Vakuu 类中继承或定义的泛型方法 RelicOption<TRelic>()
        // 注意：新版游戏 API 中该方法含有默认参数，参数长度不再是 0，因此我们只需根据泛型标志寻找
        var method = typeof(Vakuu).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            .FirstOrDefault(m => m.Name == "RelicOption" && m.IsGenericMethod);

        if (method == null)
        {
            MainFile.Logger.Info("[VakuuPoolPatch] 找不到 RelicOption 泛型方法");
            return;
        }

        var genericMethod = method.MakeGenericMethod(typeof(SoulContract));
        // 必须传入对应的参数对象，反射调用不支持省略默认参数
        if (genericMethod.Invoke(__instance, new object[] { "INITIAL", null }) is not EventOption soulContractOption)
        {
            MainFile.Logger.Info("[VakuuPoolPatch] 调用 RelicOption<SoulContract> 失败");
            return;
        }

        // 直接将灵魂契约追加到选项池中，参与随机概率
        var list = __result.ToList();
        list.Add(soulContractOption);

        __result = list;
    }
}
