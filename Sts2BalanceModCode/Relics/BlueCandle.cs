using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

/// <summary>
/// RELIC-BLUE-CANDLE-01 — 蓝蜡烛 (Blue Candle)
/// 效果：可以打出原本不能被打出的诅咒牌，打出诅咒牌会让你失去 1 点生命并将其消耗。
/// </summary>
[RegisterRelic(typeof(SharedRelicPool), FullPublicEntry = "STS2_BALANCEMOD_BLUE_CANDLE")]
public sealed class BlueCandle : BalanceRelicTemplate
{
  public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
  public override RelicRarity Rarity => RelicRarity.Uncommon;

  protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
  [
    HoverTipFactory.FromKeyword(CardKeyword.Exhaust)
  ];

  /// <summary>
  /// 原生战斗 Hook：动态修改卡牌在战斗中的关键词。
  /// 将诅咒牌的【不可打出】词条移除，并追加【消耗】词条。
  /// </summary>
  public override bool TryModifyKeywordsInCombat(CardModel card, ISet<CardKeyword> keywords)
  {
    if (card.Type == CardType.Curse && card.Owner == Owner)
    {
      keywords.Remove(CardKeyword.Unplayable);
      keywords.Add(CardKeyword.Exhaust);
      return true;
    }

    return base.TryModifyKeywordsInCombat(card, keywords);
  }

  /// <summary>
  /// 原生战斗 Hook：动态修正卡牌耗能。
  /// 仅对原本无法打出（原耗能为负）的诅咒牌修正为 0 费打出；若诅咒原本具备正数能量费用则保持原费不变。
  /// </summary>
  public override bool TryModifyEnergyCostInCombat(CardModel card, decimal originalCost, ref decimal modifiedCost)
  {
    if (card.Type == CardType.Curse && card.Owner == Owner)
    {
      if (originalCost < 0m || modifiedCost < 0m)
      {
        modifiedCost = 0m;
        return true;
      }
    }

    return base.TryModifyEnergyCostInCombat(card, originalCost, ref modifiedCost);
  }

  /// <summary>
  /// 原生战斗 Hook：结算去向修正。
  /// 诅咒牌打出后默认进入消耗堆（若卡牌带有永恒词条则受保护保留）。
  /// </summary>
  public override CardLocation ModifyCardPlayResultLocation(
    CardModel card,
    bool isAutoPlay,
    ResourceInfo resources,
    CardLocation currentLocation)
  {
    if (card.Type == CardType.Curse && card.Owner == Owner)
    {
      if (!card.Keywords.Contains(CardKeyword.Eternal))
      {
        return new CardLocation(currentLocation.player, PileType.Exhaust, currentLocation.position);
      }
    }

    return base.ModifyCardPlayResultLocation(card, isAutoPlay, resources, currentLocation);
  }

  /// <summary>
  /// 原生战斗 Hook：打出卡牌后触发。
  /// 每当打出诅咒牌，角色失去 1 点生命（无视格挡、不吃攻击/受击数值增益的生命流失）。
  /// </summary>
  public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    if (cardPlay.Card.Type == CardType.Curse && cardPlay.Player == Owner && Owner?.Creature != null)
    {
      Flash();
      await CreatureCmd.Damage(
        choiceContext,
        Owner.Creature,
        1m,
        ValueProp.Unblockable | ValueProp.Unpowered,
        null,
        cardPlay
      );
    }
  }
}
