using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

// ======================== RELIC-03: 灵魂契约 ========================

/// <summary>
/// RELIC-03 — 灵魂契约：给一张有消耗的牌去除消耗。
/// </summary>
[RegisterRelic(typeof(SharedRelicPool), FullPublicEntry = "STS2_BALANCEMOD_SOUL_CONTRACT")]
public sealed class SoulContract : BalanceRelicTemplate
{
    public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
    public override RelicRarity Rarity => RelicRarity.Shop;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
      HoverTipFactory.FromEnchantment<SoulsPower>();

    public override async Task AfterObtained()
    {
        if (Owner?.Creature == null)
            return;

        // 给一张有消耗的牌去除消耗
        SoulsPower soulsPowerEnch = ModelDb.Enchantment<SoulsPower>();
        var prefs = new CardSelectorPrefs(
          new LocString("card_selection", "TO_ENCHANT"), 1)
        {
            Cancelable = false,
            RequireManualConfirmation = true,
        };

        IEnumerable<CardModel> selectedCards = await CardSelectCmd.FromDeckForEnchantment(Owner, soulsPowerEnch, 1, prefs);
        foreach (CardModel card in selectedCards)
        {
            card.ApplyEnchantmentAndPreview<SoulsPower>(0m);
        }
    }
}
