using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.CardPools;

/// <summary>
/// CARD-08 — 袖里乾坤定位和刀舞冲突，暂时删除
/// CARD-01 — 移除原版 Precision (Pinpoint/精密)，由新卡 Eviscerate (内脏切除) 替代
/// </summary>
/// <remarks>
/// NOTE: 只移除不替换 — Custom cards 通过 [Pool(typeof(SilentCardPool))] 由
/// BaseLib 的 AddCustomPools → ConcatModelsFromMods 自动注入，无需手动追加。
/// 如果这里再做原位替换，ConcatModelsFromMods 会再追加一份，造成重复。
/// </remarks>
[HarmonyPatch(typeof(SilentCardPool), "GenerateAllCards")]
public static class SilentCardPoolPatch
{
  [HarmonyPostfix]
  public static CardModel[] Postfix(CardModel[] __result)
  {
    return __result.Where(c => c is not UpMySleeve && c is not Pinpoint).ToArray();
  }
}
