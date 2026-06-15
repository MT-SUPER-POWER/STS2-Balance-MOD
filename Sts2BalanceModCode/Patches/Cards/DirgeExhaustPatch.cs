using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-01 — 骨妹升级的挽歌不提高召唤次数
/// Prefix 删除消耗词条并跳过原版 OnUpgrade，避免 Summon 动态值从 3 提升到 4。
/// </summary>
[HarmonyPatch(typeof(Dirge), "OnUpgrade")]
public static class DirgeExhaustPatch
{
  [HarmonyPrefix]
  public static bool Prefix(Dirge __instance)
  {
    __instance.RemoveKeyword(CardKeyword.Exhaust);
    return false; // 跳过原方法
  }
}
