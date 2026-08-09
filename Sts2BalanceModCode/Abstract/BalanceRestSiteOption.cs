using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;

namespace Sts2BalanceMod.Sts2BalanceModCode.Abstract;

// ======================== REST SITE OPTION BASE ========================

/// <summary>
/// Mod 火堆选项基类，封装常见的牌组选牌与删牌能力。
/// 输入：构造时传入持有该火堆选项的玩家。
/// 输出：派生类实现 OptionId 与 OnSelect，并通过受保护方法复用火堆选项逻辑。
/// 返回值：OnSelect 返回 true 表示选项已成功执行，返回 false 表示玩家取消或未产生效果。
/// </summary>
public abstract class BalanceRestSiteOption : RestSiteOption
{
    protected BalanceRestSiteOption(Player owner) : base(owner)
    {
    }

    /// <summary>
    /// 自定义火堆选项按钮标题。
    /// 输入：无。
    /// 输出：用于 UI 按钮文字的本地化字符串。
    /// </summary>
    public virtual LocString CustomTitle => new("rest_site_ui", $"OPTION_{OptionId}.name");

    /// <summary>
    /// 自定义火堆选项图标路径。
    /// 输入：无。
    /// 输出：位于 Mod 资源目录内的 PNG 路径。
    /// </summary>
    public virtual string CustomIconPath => $"Option{char.ToUpperInvariant(OptionId[0])}{OptionId[1..].ToLowerInvariant()}.png".RestSiteOptionImagePath();

    public override IEnumerable<string> AssetPaths => [CustomIconPath];

    /// <summary>
    /// 统计玩家牌组中可被移除的卡牌数量。
    /// 输入：目标玩家。
    /// 输出：可删除卡牌数量。
    /// </summary>
    protected static int GetRemovableCardCount(Player player)
    {
        return PileType.Deck.GetPile(player).Cards.Count(static c => c.IsRemovable);
    }

    /// <summary>
    /// 打开牌组删牌选择界面。
    /// 输入：需要选择的卡牌数量。
    /// 输出：玩家确认选择的卡牌列表；如果取消则为空列表。
    /// </summary>
    protected async Task<IReadOnlyList<CardModel>> SelectCardsForRemoval(int count)
    {
        CardSelectorPrefs prefs = new(CardSelectorPrefs.RemoveSelectionPrompt, count)
        {
            Cancelable = true,
            RequireManualConfirmation = true
        };

        return (await CardSelectCmd.FromDeckForRemoval(Owner, prefs)).ToList();
    }

    /// <summary>
    /// 从牌组中移除指定卡牌并播放预览。
    /// 输入：待移除卡牌集合。
    /// 输出：异步完成删牌流程。
    /// </summary>
    protected static async Task RemoveCardsFromDeck(IReadOnlyList<CardModel> cards)
    {
        foreach (CardModel card in cards)
        {
            await CardPileCmd.RemoveFromDeck(card);
        }
    }
}
