using System;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sts2BalanceMod.Sts2BalanceModCode.Enchantments;

/// <summary>
/// RELIC-10 — 矮人铁砧附魔效果。
/// 每次火堆锻造时递增 Amount，按公式 ceil(n(n+7)/2) 增加伤害/格挡。
/// 可堆叠：同一张卡牌上可以多次应用此附魔（每次 Amount 递增）。
/// </summary>
public sealed class ForgeEnchantment : EnchantmentModel
{
  public override bool ShowAmount => true;

  /// <summary>
  /// 可堆叠：允许对已有此附魔的卡牌再次附魔（Amount 递增）
  /// </summary>
  public override bool IsStackable => true;

  public override bool CanEnchantCardType(CardType cardType)
  {
    return cardType is CardType.Attack or CardType.Skill;
  }

  /// <summary>
  /// 强化公式：ceil(n(n+7)/2)
  /// n = 锻造次数（附魔 Amount）
  /// </summary>
  private static int GetBoostAmount(int n)
  {
    return (int)Math.Ceiling((decimal)n * (n + 7) / 2m);
  }

  public override decimal EnchantDamageAdditive(decimal originalDamage, ValueProp props)
  {
    // 仅对 powered attack 生效（非状态/非力量造成的攻击）
    if (!props.IsPoweredAttack())
      return 0m;
    return GetBoostAmount(Amount);
  }

  public override decimal EnchantBlockAdditive(decimal originalBlock)
  {
    return GetBoostAmount(Amount);
  }
}
