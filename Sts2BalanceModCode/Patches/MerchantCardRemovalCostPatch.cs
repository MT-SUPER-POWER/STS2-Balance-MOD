using System.Reflection;
using HarmonyLib;

using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches;

/// <summary>
/// SHOP-01: 调整删牌价格
///   低进阶 (V1-5):   BaseCost=50, PriceIncrease=25
///   高进阶 (A6+):    BaseCost=75, PriceIncrease=25
/// </summary>
[HarmonyPatch(typeof(MerchantCardRemovalEntry), nameof(MerchantCardRemovalEntry.CalcCost))]
internal static class MerchantCardRemovalPricePatch
{
  // MerchantEntry 里面有两个成员变量就是 _cost 和 _player
  private static readonly FieldInfo? CostField =
      AccessTools.Field(typeof(MerchantEntry), "_cost");

  private static readonly FieldInfo? PlayerField =
      AccessTools.Field(typeof(MerchantEntry), "_player");

  [HarmonyPrefix]
  private static bool Prefix(MerchantCardRemovalEntry __instance)
  {
    // 如果游戏更新导致字段名变了，就放弃补丁，走原版逻辑，避免直接炸游戏
    if (CostField is null || PlayerField is null)
    {
      return true;
    }

    var player = (Player?)PlayerField.GetValue(__instance);
    if (player is null)
    {
      return true;
    }

    int removalsUsed = player.ExtraFields.CardShopRemovalsUsed;   // 删了多少次牌

    // 这里改成你自己的价格逻辑
    int baseCost = AscensionHelper.GetValueIfAscension(
        AscensionLevel.Inflation,
        75,  // 高进阶价格
        50   // 普通价格
    );

    int priceIncrease = 25;

    int newCost = baseCost + priceIncrease * removalsUsed;

    // 防止出现负数价格
    newCost = Math.Max(0, newCost);

    CostField.SetValue(__instance, newCost);

    // false = 不执行原版 CalcCost()
    return false;
  }
}
