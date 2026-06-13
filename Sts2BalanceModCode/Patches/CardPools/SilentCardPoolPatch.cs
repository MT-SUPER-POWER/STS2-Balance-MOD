using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.CardPools;

/// <summary>
/// CARD-08 — 袖里乾坤定位和刀舞冲突，暂时删除
/// </summary>
/// <remarks>
/// NOTE: 只移除不替换 — Electrodynamics 通过 [Pool(typeof(DefectCardPool))] 由
/// BaseLib 的 AddCustomPools → ConcatModelsFromMods 自动注入，无需手动追加。
/// 如果这里再做原位替换，ConcatModelsFromMods 会再追加一份，造成重复。
/// </remarks>
[HarmonyPatch(typeof(SilentCardPool), "GenerateAllCards")]
public static class SilentCardPoolPatch
{
  [HarmonyPostfix]
  public static CardModel[] Postfix(CardModel[] __result)
  {
    return __result.Where(c => c is not UpMySleeve).ToArray();
  }
}
