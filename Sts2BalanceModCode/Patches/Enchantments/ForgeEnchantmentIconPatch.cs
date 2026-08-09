using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using Sts2BalanceMod.Sts2BalanceModCode.Enchantments;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Enchantments;

/// <summary>
/// 把 ForgeEnchantment 的图标路径重定向到 MOD 自带资源。
/// 用 Harmony 在 getter 上做前缀替换。
/// </summary>
[HarmonyPatch(typeof(EnchantmentModel), "get_" + nameof(EnchantmentModel.IconPath))]
public static class ForgeEnchantmentIconPatch
{
  private static readonly string ModIconPath = ModAssetPaths.Resource("images", "enchantments", "ForgeEnchantment.png");

  [HarmonyPrefix]
  public static bool Prefix(EnchantmentModel __instance, ref string __result)
  {
    if (__instance is not ForgeEnchantment)
    {
      return true;
    }

    __result = ModIconPath;
    return false;
  }
}
