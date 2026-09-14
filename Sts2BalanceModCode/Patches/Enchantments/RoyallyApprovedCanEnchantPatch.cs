using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Enchantments;

/// <summary>
/// RELIC-ROYAL-STAMP-01 — 放宽王室认证（Royally Approved）附魔类型约束，允许能力牌附魔。
/// Target: MegaCrit.Sts2.Core.Models.Enchantments.RoyallyApproved.CanEnchantCardType(CardType cardType).
/// Reason: 原版王室认证仅允许攻击牌（Attack）和技能牌（Skill），放宽约束使能力牌（Power）也可以被附魔并获得固有和保留。
/// WARNING: Verified against D:\Game\Sts2Code\src\MegaCrit.Sts2.Core.Models.Enchantments\RoyallyApproved.cs; game updates may change this decompiled implementation.
/// </summary>
[HarmonyPatch(typeof(RoyallyApproved), nameof(RoyallyApproved.CanEnchantCardType))]
public static class RoyallyApprovedCanEnchantPatch
{
  [HarmonyPostfix]
  private static void Postfix(CardType cardType, ref bool __result)
  {
    if (cardType == CardType.Power)
    {
      __result = true;
    }
  }
}
