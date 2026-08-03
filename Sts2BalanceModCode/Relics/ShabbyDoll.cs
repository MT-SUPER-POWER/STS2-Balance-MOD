using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
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

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromCard<SorceryStrike>(upgrade: true),
        HoverTipFactory.FromCard<SorceryDefend>(upgrade: true)
    };

    public override async Task AfterObtained()
    {
        if (Owner?.Creature == null)
            return;

        Flash();

        // 1. 扣除 50% 最大生命值上限并调整当前生命
        int currentMaxHp = Owner.Creature.MaxHp;
        int newMaxHp = Math.Max(1, currentMaxHp / 2);
        Owner.Creature.SetMaxHpInternal(newMaxHp);
        if (Owner.Creature.CurrentHp > newMaxHp)
        {
            Owner.Creature.SetCurrentHpInternal(newMaxHp);
        }

        // 2. 将牌组中所有的基础【打击】与【防御】卡牌替换为升级后的【巫术打击+】与【巫术防御+】
        var deckCards = Owner.Deck.Cards.ToList();
        var cardsToReplace = deckCards.Where(c => 
            c.IsBasicStrikeOrDefend ||
            c.Tags.Contains(CardTag.Strike) || 
            c.Tags.Contains(CardTag.Defend) || 
            c.Id.Entry.Contains("STRIKE", StringComparison.OrdinalIgnoreCase) || 
            c.Id.Entry.Contains("DEFEND", StringComparison.OrdinalIgnoreCase)).ToList();

        foreach (var card in cardsToReplace)
        {
            await CardPileCmd.RemoveFromDeck(card, showPreview: true);

            bool isStrike = card.Tags.Contains(CardTag.Strike) || card.Id.Entry.Contains("STRIKE", StringComparison.OrdinalIgnoreCase);
            CardModel newCard = isStrike
                ? ModelDb.Card<SorceryStrike>().ToMutable()
                : ModelDb.Card<SorceryDefend>().ToMutable();

            CardCmd.Upgrade(newCard);
            var result = await CardPileCmd.Add(newCard, PileType.Deck);
            CardCmd.PreviewCardPileAdd(result);
        }
    }
}
