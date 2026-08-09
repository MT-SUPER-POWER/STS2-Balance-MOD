using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

// ======================== RELIC-07: 诅咒钥匙 ========================

/// <summary>
/// RELIC-07 — 诅咒钥匙：每回合 +1 费用，打开宝箱时获得一张随机诅咒牌
/// （诅咒逻辑见 CurseKeyPatch）
/// </summary>
[RegisterRelic(typeof(EventRelicPool), FullPublicEntry = "STS2_BALANCEMOD_CURSE_KEY")]
public sealed class CurseKey : BalanceRelicTemplate
{
  public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
  public override RelicRarity Rarity => RelicRarity.Ancient;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new EnergyVar(1),
  ];

  // ======================== 能量加成 ========================

  public override decimal ModifyMaxEnergy(Player player, decimal amount)
  {
    if (player != base.Owner)
    {
      return amount;
    }
    return amount + base.DynamicVars.Energy.IntValue;
  }
}
