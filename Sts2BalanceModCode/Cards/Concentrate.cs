using BaseLib.Utils;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// LEGACY-03 — 全神贯注（猎人）
/// 0费 | 技能 | 丢弃 3 张牌，获得 2 点能量
/// 升级：丢弃 2 张牌（仍获得 2 能）
/// </summary>
[Pool(typeof(SilentCardPool))]
public sealed class Concentrate : Sts2CardModel
{
  public Concentrate() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
  {
    WithCards(3, -1); // 基础弃 3，升级弃 2
    WithEnergy(2, 0); // 获得 2 能
  }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    int discardCount = DynamicVars.Cards.IntValue;

    var cards = (await CardSelectCmd.FromHandForDiscard(
        choiceContext,
        Owner,
        new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, 0, discardCount),
        null,
        this
    )).ToList();

    foreach (var card in cards)
      await CardCmd.Discard(choiceContext, card);

    await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
  }
}
