using System.Collections.Generic;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;


namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

// ======================== RELIC-01: 袖箭 ========================

/// <summary>
/// RELIC-01 — 袖箭：费用为0的 攻击牌 额外造成 4 点伤害。
/// 猎人（silent）专属遗物，罕见度：罕见。
/// </summary>
[Pool(typeof(SilentRelicPool))]
public sealed class WristBlade : Sts2RelicModel
{
  private const string ExtraDamageKey = "Damage";

  public override RelicRarity Rarity => RelicRarity.Uncommon;

  protected override IEnumerable<DynamicVar> CanonicalVars => new[]
  {
    new DynamicVar(ExtraDamageKey, 4m)
  };

  public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
  {
    if (!props.IsPoweredAttack())
    {
      return 0m;
    }
    if (cardSource == null)
    {
      return 0m;
    }
    if (cardSource.Type != CardType.Attack)
    {
      return 0m;
    }
    if (cardSource.Owner != base.Owner)
    {
      return 0m;
    }
    // Only apply to cards that currently cost 0 energy (excluding X-cost cards, which cost X and consume all energy)
    if (cardSource.EnergyCost.CostsX || cardSource.EnergyCost.GetWithModifiers(CostModifiers.All) != 0)
    {
      return 0m;
    }

    return base.DynamicVars[ExtraDamageKey].BaseValue;
  }
}
