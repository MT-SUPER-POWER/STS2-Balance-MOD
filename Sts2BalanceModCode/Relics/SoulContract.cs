using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Models.Enchantments;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

// ======================== RELIC-02: 灵魂契约 ========================

/// <summary>
/// RELIC-02 — 灵魂契约：扣除最大生命上限 10% 的代价，给一张有消耗的牌去除消耗。
/// </summary>
[Pool(typeof(EventRelicPool))]
public sealed class SoulContract : Sts2RelicModel
{
    public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
      HoverTipFactory.FromEnchantment<SoulsPower>();

    public override async Task AfterObtained()
    {
        if (Owner?.Creature == null)
            return;

        // 扣除最大生命上限 10% 的代价
        int maxHp = Owner.Creature.MaxHp;
        int hpLoss = (int)Math.Round(maxHp * 0.1);
        int newMaxHp = maxHp - hpLoss;
        if (newMaxHp < 1)
            newMaxHp = 1; // 至少保留 1 点最大生命值
        await CreatureCmd.SetMaxHp(Owner.Creature, newMaxHp);

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
            CardCmd.Enchant(soulsPowerEnch.ToMutable(), card, 0);
            CardCmd.Preview(card);
        }
    }
}
