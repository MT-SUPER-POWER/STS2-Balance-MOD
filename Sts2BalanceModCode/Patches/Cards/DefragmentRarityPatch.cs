using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-DEFRAGMENT-01 — 碎片整理金卡→蓝卡（稀有度降级）
/// 原版 Defragment 为稀有（Rare），降为罕见（Uncommon）使集中流更容易启动。
/// Target: CardModel.get_Rarity
/// Reason: 让集中流的基础建构牌更易获取，解决中后期集中匮乏问题。
/// WARNING: Verified against D:\Game\Sts2Code\src\MegaCrit.Sts2.Core.Models.Cards\Defragment.cs;
/// game updates may change this decompiled implementation.
/// </summary>
[HarmonyPatch(typeof(CardModel), "get_Rarity")]
[Sts2BalanceMod.Sts2BalanceModCode.Settings.BalancePatch("C33")]
public static class DefragmentRarityPatch
{
  [HarmonyPrefix]
  public static bool Prefix(CardModel __instance, ref CardRarity __result)
  {
    if (__instance is Defragment)
    {
      __result = CardRarity.Uncommon;
      return false; // 跳过原 getter
    }
    return true;
  }
}
