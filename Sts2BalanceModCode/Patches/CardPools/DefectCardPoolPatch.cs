using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using Sts2BalanceMod.Sts2BalanceModCode.Settings;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.CardPools;

/// <summary>
/// LEGACY-04 — 从 Defect 卡池中移除 ConsumingShadow
/// CARD-SYNTHESIS-01 — 从 Defect 卡池中移除 Synthesis (人工合成)
/// CARD-SYNCHRONIZE-01 — 从 Defect 卡池中移除 Synchronize (同步)
/// Target: MegaCrit.Sts2.Core.Models.CardPools.DefectCardPool.GenerateAllCards
/// Reason: 根据 C23 (ConsumingShadow), C35 (Synthesis), C36 (Synchronize) 配置项从卡池中过滤对应原版卡牌。
/// </summary>
/// <remarks>
/// NOTE: 只移除不替换 — Electrodynamics, Fission 等自定义卡牌由 RitsuLib 自动注册到对应卡池；此补丁仅保留原版卡池移除逻辑。
/// </remarks>
[HarmonyPatch(typeof(DefectCardPool), "GenerateAllCards")]
public static class DefectCardPoolPatch
{
  [HarmonyPostfix]
  public static CardModel[] Postfix(CardModel[] __result) =>
  [
    .. __result.Where(c =>
      (c is not ConsumingShadow || !BalanceModSettings.IsEnabled("C23")) &&
      (c is not Synthesis || !BalanceModSettings.IsEnabled("C35")) &&
      (c is not Synchronize || !BalanceModSettings.IsEnabled("C36"))
    )
  ];
}
