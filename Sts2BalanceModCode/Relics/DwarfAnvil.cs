using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Enchantments;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

// ======================== RELIC-10: 矮人铁砧 ========================

/// <summary>
/// RELIC-10 — 矮人铁砧：商店遗物。
/// 拾起时选择 3 张牌附魔，被附魔的牌费用永久 -1（最低 0 费）。
/// </summary>
[Pool(typeof(SharedRelicPool))]
public sealed class DwarfAnvil : Sts2RelicModel
{
  private const string EnergyKey = "Energy";

  public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
  public override RelicRarity Rarity => RelicRarity.Shop;

  public override bool HasUponPickupEffect => true;

  protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    HoverTipFactory.FromEnchantment<ForgeEnchantment>();

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new EnergyVar(1),
  ];

  public override async Task AfterObtained()
  {
    var forge = ModelDb.Enchantment<ForgeEnchantment>();
    var prefs = new CardSelectorPrefs(
      new LocString("card_selection", "TO_ENCHANT"), 3)
    {
      Cancelable = false,
      RequireManualConfirmation = true,
    };

    foreach (var card in await CardSelectCmd.FromDeckForEnchantment(Owner, forge, 1, prefs))
    {
      CardCmd.Enchant(forge.ToMutable(), card, base.DynamicVars[EnergyKey].IntValue);
      CardCmd.Preview(card);
    }
  }
}
