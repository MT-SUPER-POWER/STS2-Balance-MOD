using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches;

/// <summary>
/// 修复 BaseLib 3.4.0 对 ModelDb.InitIds 进行 Patch 时，
/// 未提前为自定义 Mod 模型设置 AbstractModel.Id 导致在 AbstractModel.InitId 中
/// 访问 Id.Category 引发 NullReferenceException 的底层 bug。
/// </summary>
[HarmonyPatch(typeof(AbstractModel), nameof(AbstractModel.InitId))]
public static class AbstractModelInitIdFixPatch
{
    [HarmonyPrefix]
    public static void Prefix(AbstractModel __instance, ModelId id)
    {
        if (__instance.Id == null)
        {
            PropertyInfo? prop = typeof(AbstractModel).GetProperty(nameof(AbstractModel.Id));
            prop?.SetValue(__instance, id);
        }
    }
}
