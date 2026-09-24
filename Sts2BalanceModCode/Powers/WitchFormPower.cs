using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Cards;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// 女巫形态能力：在回合开始时将手牌中的两张牌转化为升级后的巫术打击与巫术防御。
/// </summary>
[RegisterPower]
public sealed class WitchFormPower() : BalancePowerTemplate(PowerType.Buff, PowerStackType.Single)
{
  public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
  {
    if (player != Owner.Player)
    {
      return;
    }

    Flash();
    var transformableCards = PileType.Hand.GetPile(player).Cards
        .Where(card => card.IsTransformable)
        .ToList();
    if (transformableCards.Count == 0)
    {
      return;
    }

    if (transformableCards.Count == 1)
    {
      await TransformSingleCard(choiceContext, player, transformableCards[0]);
      return;
    }

    // 提示玩家选择 2 张手牌进行蜕变
    var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 2);
    var sources = (await CardSelectCmd.FromHand(
        choiceContext,
        player,
        prefs,
        card => card.IsTransformable,
        this)).ToList();

    if (sources.Count < 2)
    {
      return;
    }

    CardModel attackSource = sources[0];
    CardModel defendSource = sources[1];

    CardModel attack = CreateReplacement<SorceryStrike>(attackSource, forceUpgrade: true);
    CardModel defend = CreateReplacement<SorceryDefend>(defendSource, forceUpgrade: true);
    await CardCmd.Transform(
        [new CardTransformation(attackSource, attack), new CardTransformation(defendSource, defend)],
        rng: null,
        CardPreviewStyle.None);
  }

  private static async Task TransformSingleCard(
      PlayerChoiceContext choiceContext,
      Player player,
      CardModel source)
  {
    CardModel attackPreview = source.CardScope!.CreateCard<SorceryStrike>(player);
    CardModel defendPreview = source.CardScope!.CreateCard<SorceryDefend>(player);
    CardCmd.Upgrade([attackPreview, defendPreview], CardPreviewStyle.None);
    CardModel choice = await CardSelectCmd.FromChooseACardScreen(
        choiceContext,
        [attackPreview, defendPreview],
        player) ?? throw new InvalidOperationException("Witch Form result selection cannot be skipped.");

    CardModel replacement = choice is SorceryStrike
        ? CreateReplacement<SorceryStrike>(source, forceUpgrade: true)
        : CreateReplacement<SorceryDefend>(source, forceUpgrade: true);
    await CardCmd.Transform(source, replacement, CardPreviewStyle.None);
  }

  private static CardModel CreateReplacement<TCard>(CardModel source, bool forceUpgrade)
      where TCard : CardModel
  {
    CardModel replacement = source.CardScope!.CreateCard<TCard>(source.Owner);
    if (forceUpgrade || source.IsUpgraded)
    {
      CardCmd.Upgrade(replacement, CardPreviewStyle.None);
    }

    if (source.Enchantment is not null)
    {
      var enchantment = (EnchantmentModel)source.Enchantment.MutableClone();
      CardCmd.Enchant(enchantment, replacement, enchantment.Amount);
    }

    return replacement;
  }
}
