using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.CardPools;

/// <summary>
/// CARD-03 & CARD-04 — 用进化替换残酷
/// 从战士卡池中移除残酷。进化会自动通过 [Pool(typeof(IroncladCardPool))] 注入。
/// </summary>
[HarmonyPatch(typeof(IroncladCardPool), "GenerateAllCards")]
public static class IroncladCardPoolPatch
{
    [HarmonyPostfix]
    public static CardModel[] Postfix(CardModel[] __result)
    {
        return [.. __result.Where(c => c is not Cruelty)];
    }
}
