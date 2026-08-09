using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using Sts2BalanceMod.src.Abstract;

namespace Sts2BalanceMod.src.Cards;

/// <summary>
/// LEGACY-03 — 全神贯注（猎人）
/// 0费 | 技能 | 丢弃 3 张牌，获得 2 点能量
/// 升级：丢弃 2 张牌（仍获得 2 能）
/// </summary>
[RegisterCard(typeof(SilentCardPool), FullPublicEntry = "STS2_BALANCEMOD_CONCENTRATE")]
public sealed class Concentrate : BalanceCardTemplate
{
  protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(3), new EnergyVar(2)];

  public Concentrate() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    int discardCount = DynamicVars.Cards.IntValue;

    var cards = (await CardSelectCmd.FromHandForDiscard(
        choiceContext,
        Owner,
        new CardSelectorPrefs(CardSelectorPrefs.DiscardSelectionPrompt, discardCount),
        null,
        this
    )).ToList();

    foreach (var card in cards)
      await CardCmd.Discard(choiceContext, card);

    await PlayerCmd.GainEnergy(DynamicVars.Energy.IntValue, Owner);
  }

  protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(-1);
}
