using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-01 — 挽歌：在原版升级效果的基础上，额外获得保留词条。
/// </summary>
[HarmonyPatch(typeof(Dirge), "OnUpgrade")]
public static class DirgeOnUpgradePatch
{
  [HarmonyPostfix]
  public static void Postfix(Dirge __instance)
  {
    __instance.AddKeyword(CardKeyword.Retain);
  }
}
