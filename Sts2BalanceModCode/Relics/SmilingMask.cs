using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Merchant;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

// ======================== RELIC-06: 微笑面具 ========================

/// <summary>
/// RELIC-06 — 微笑面具：删牌价格固定 50
/// </summary>
[RegisterRelic(typeof(SharedRelicPool), FullPublicEntry = "STS2_BALANCEMOD_SMILING_MASK")]
public sealed class SmilingMask : BalanceRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Common;
    public static readonly int FIXED_DELETE_PRICE = 50;

    private const string _deletePriceKey = "delete_price";

    protected override IEnumerable<DynamicVar> CanonicalVars => new[]
    {
    new DynamicVar(_deletePriceKey, FIXED_DELETE_PRICE)
  };

    public override decimal ModifyMerchantPrice(Player player, MerchantEntry entry, decimal originalPrice)
    {
        if (player != base.Owner)
            return originalPrice;
        if (entry is not MerchantCardRemovalEntry)
            return originalPrice;
        return base.DynamicVars[_deletePriceKey].BaseValue;
    }
}
