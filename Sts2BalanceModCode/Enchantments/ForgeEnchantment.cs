using System;
using System.Collections.Generic;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sts2BalanceMod.Sts2BalanceModCode.Enchantments;

public sealed class ForgeEnchantment : EnchantmentModel
{
  public override bool ShowAmount => true;
  public override bool HasExtraCardText => true;
  public override bool IsStackable => true;

  public override bool CanEnchantCardType(CardType cardType)
  {
    return cardType is CardType.Attack or CardType.Skill;
  }

  protected override IEnumerable<IHoverTip> ExtraHoverTips
  {
    get
    {
      yield return HoverTipFactory.FromKeyword(CardKeyword.Exhaust);
    }
  }

  private static int GetBoostAmount(int n)
  {
    return (int)Math.Ceiling((decimal)n * (n + 7) / 2m);
  }

  public override decimal EnchantDamageAdditive(decimal originalDamage, ValueProp props)
  {
    if (!props.IsPoweredAttack()) return 0m;
    return GetBoostAmount(Amount);
  }

  public override decimal EnchantBlockAdditive(decimal originalBlock)
  {
    return GetBoostAmount(Amount);
  }
}

/// <summary>
/// Patch EnchantmentModel.IconPath getter so ForgeEnchantment uses the MOD icon path.
/// (IconPath is not virtual, so a Harmony postfix is the cleanest fix.)
/// </summary>
[HarmonyPatch(typeof(EnchantmentModel))]
internal static class ForgeEnchantmentIconPatch
{
  [HarmonyPatch(nameof(EnchantmentModel.IconPath), MethodType.Getter)]
  [HarmonyPostfix]
  private static void IconPathPostfix(EnchantmentModel __instance, ref string __result)
  {
    if (__instance is ForgeEnchantment)
      __result = "res://Sts2BalanceMod/images/enchantments/forge_enchantment.png";
  }
}
