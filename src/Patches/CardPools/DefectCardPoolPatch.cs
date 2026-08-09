using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.src.Patches.CardPools;

/// <summary>
/// LEGACY-04 — 从 Defect 卡池中移除 ConsumingShadow
/// </summary>
/// <remarks>
/// NOTE: 只移除不替换 — Electrodynamics 通过 [Pool(typeof(DefectCardPool))] 由
/// 卡牌由 RitsuLib 自动注册到对应卡池；此补丁仅保留原版卡池替换逻辑。
/// 如果这里再做原位替换，ConcatModelsFromMods 会再追加一份，造成重复。
/// </remarks>
[HarmonyPatch(typeof(DefectCardPool), "GenerateAllCards")]
public static class DefectCardPoolPatch
{
  [HarmonyPostfix]
  public static CardModel[] Postfix(CardModel[] __result)
  {
    return __result.Where(c => c is not ConsumingShadow).ToArray();
  }
}
