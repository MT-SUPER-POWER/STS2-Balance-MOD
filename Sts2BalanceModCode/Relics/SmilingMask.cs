using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Entities.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

// ======================== RELIC-06: 微笑面具 ========================

/// <summary>
/// RELIC-06 — 微笑面具：删牌价格固定 50
/// </summary>
[Pool(typeof(SharedRelicPool))]
public sealed class SmilingMask : Sts2RelicModel
{
  public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
  public override RelicRarity Rarity => RelicRarity.Common;
  public static readonly int FIXED_DELETE_PRICE = 50;
}
