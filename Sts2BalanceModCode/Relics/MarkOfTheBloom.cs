using STS2RitsuLib.Interop.AutoRegistration;
using STS2RitsuLib.Combat.Healing;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

/// <summary>
/// STS1-RELIC-01 — 绽放印记：持有者无法回复生命。
/// 来源参考 ActsFromThePast.Relics.MarkOfTheBloom。
/// </summary>
[RegisterRelic(typeof(EventRelicPool), FullPublicEntry = "STS2_BALANCEMOD_MARK_OF_THE_BLOOM")]
public sealed class MarkOfTheBloom : BalanceRelicTemplate, IHealHookListener
{
  public override RelicRarity Rarity => RelicRarity.Event;

  public decimal ModifyHealMultiplicative(HealContext context, decimal amount)
  {
    if (context.Creature.Player != Owner)
      return 1M;

    if (amount > 0)
      Flash();

    return 0M;
  }
}
