using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using Sts2BalanceMod.Sts2BalanceModCode.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches;

/// <summary>
/// LEGACY-04 — 将 Defect 卡池中的 ConsumingShadow 替换为 Electrodynamics
/// </summary>
[HarmonyPatch(typeof(DefectCardPool), "GenerateAllCards")]
public static class DefectCardPoolPatch
{
  [HarmonyPostfix]
  public static CardModel[] Postfix(CardModel[] __result)
  {
    for (int i = 0; i < __result.Length; i++)
    {
      if (__result[i] is ConsumingShadow)
      {
        __result[i] = ModelDb.Card<Electrodynamics>();
        break;
      }
    }
    return __result;
  }
}
