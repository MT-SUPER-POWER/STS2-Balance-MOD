using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Sts2BalanceMod.Sts2BalanceModCode.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

/// <summary>
/// 先古遗物：破旧的玩偶 (Shabby Doll)
/// 代价: 获得遗物时扣除 50% 最大生命值上限。
/// 效果: 将牌组中所有的基础【打击】与【防御】全部替换为升级后的【巫术打击+】与【巫术防御+】。
/// </summary>
[Pool(typeof(SharedRelicPool))]
public sealed class ShabbyDoll : Sts2RelicModel
{
    public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    public override bool HasUponPickupEffect => true;

    public override async Task AfterObtained()
    {
        if (Owner?.Creature == null)
            return;

        // 1. 扣除 50% 最大生命值上限
        int currentMaxHp = Owner.Creature.MaxHp;
        int newMaxHp = Math.Max(1, currentMaxHp / 2);
        Owner.Creature.SetMaxHpInternal(newMaxHp);

        // 2. 将牌组中所有的基础【打击】与【防御】卡牌替换为升级后的【巫术打击+】与【巫术防御+】
        var deckCards = Owner.Deck.Cards.ToList();
        var cardsToReplace = deckCards.Where(c => 
            c.Tags.Contains(CardTag.Strike) || 
            c.Tags.Contains(CardTag.Defend) || 
            c.Id.Entry.Contains("Strike") || 
            c.Id.Entry.Contains("Defend")).ToList();

        foreach (var card in cardsToReplace)
        {
            await CardPileCmd.RemoveFromDeck(card, showPreview: false);

            bool isStrike = card.Tags.Contains(CardTag.Strike) || card.Id.Entry.Contains("Strike");
            CardModel newCard = isStrike
                ? ModelDb.Card<SorceryStrike>().ToMutable()
                : ModelDb.Card<SorceryDefend>().ToMutable();

            CardCmd.Upgrade(newCard);
            await CardPileCmd.Add(newCard, PileType.Deck);
        }
    }
}
