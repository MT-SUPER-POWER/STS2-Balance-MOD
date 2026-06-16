using BaseLib.Hooks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

/// <summary>
/// STS1-RELIC-01 — 绽放印记：持有者无法回复生命。
/// 来源参考 ActsFromThePast.Relics.MarkOfTheBloom。
/// </summary>
[Pool(typeof(EventRelicPool))]
public sealed class MarkOfTheBloom : Sts2RelicModel, IHealAmountModifier
{
  public override RelicRarity Rarity => RelicRarity.Event;

  public decimal ModifyHealMultiplicative(Creature creature, decimal amount)
  {
    if (creature.Player != Owner)
      return 1M;

    if (amount > 0)
      Flash();

    return 0M;
  }
}
