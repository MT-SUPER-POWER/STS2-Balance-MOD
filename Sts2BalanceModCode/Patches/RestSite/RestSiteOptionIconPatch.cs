using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.RestSite;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.RestSite;

// ======================== REST SITE OPTION ICON PATCH ========================

/// <summary>
/// 火堆选项图标 Getter 补丁。
/// 输入：访问 RestSiteOption.Icon。
/// 输出：若为 BalanceRestSiteOption，返回其 CustomIconPath 纹理。
/// FIXME: 目的：修复在火堆界面悬停/选中 Mod 选项时（如 Smoke 宁静烟斗），因为遗物命名规则和官方不一样导致的问题
///   官方的命名规则都是 xxx_xxx_xx ，而我们命名规则是 XxxxxXxxx，ResetSiteOption 的 iconPath 就用到了这个规则
///   但是变量所以他们没有写 virtual 我们就算继承了之后，内部写了同样的变量，多态也不会找到继承类里的变量
///   只能通过这个 PATCH 先解决我们命名都是大写的引起的问题
/// </summary>
[HarmonyPatch(typeof(RestSiteOption), nameof(RestSiteOption.Icon), MethodType.Getter)]
internal static class RestSiteOptionIconPatch
{
    [HarmonyPrefix]
    private static bool Prefix(RestSiteOption __instance, ref Texture2D __result)
    {
        if (__instance is BalanceRestSiteOption balanceOption)
        {
            __result = PreloadManager.Cache.GetTexture2D(balanceOption.CustomIconPath);
            return false;
        }

        return true;
    }
}
