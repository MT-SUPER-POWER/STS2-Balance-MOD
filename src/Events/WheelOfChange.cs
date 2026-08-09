using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.src.Events.UI;

using Sts2BalanceMod.src.Abstract;

namespace Sts2BalanceMod.src.Events;

/// <summary>
/// STS1-EVENT-07 — 大转盘：移植结果逻辑，暂不移植参考模组的自定义转盘 UI。
/// 来源参考 ActsFromThePast.SharedEvents.WheelOfChange。
/// </summary>
[RegisterSharedEvent]
public sealed class WheelOfChange : BalanceEventTemplate
{
  private const decimal HpLossPercent = 0.15M;

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new IntVar("HpLoss", 0),
    new IntVar("GoldAmount", 0),
  ];

  public override void CalculateVars()
  {
    var owner = Owner;
    if (owner?.Creature == null)
      return;

    DynamicVars["HpLoss"].BaseValue = (int)(owner.Creature.MaxHp * HpLossPercent);
    DynamicVars["GoldAmount"].BaseValue = GetGoldAmount();
  }

  protected override IReadOnlyList<EventOption> GenerateInitialOptions()
  {
    return [Option(Play)];
  }

  private async Task Play()
  {
    var owner = Owner;
    if (owner == null)
      return;

    for (var i = 0; i < owner.RunState.CurrentActIndex; i++)
      Rng.NextInt(1);

    var result = Rng.NextInt(6);
    var minigame = new WheelSpinMinigame(owner, result, owner.RunState.CurrentActIndex);
    await minigame.PlayMinigame();
    ShowResult(result);
  }

  private int GetGoldAmount()
  {
    return Owner?.RunState.CurrentActIndex switch
    {
      0 => 100,
      1 => 200,
      _ => 300,
    };
  }

  private void ShowResult(int result)
  {
    var (pageKey, optionKey) = result switch
    {
      0 => ("GOLD", "PRIZE_GOLD"),
      1 => ("RELIC", "PRIZE_RELIC"),
      2 => ("HEAL", "PRIZE_HEAL"),
      3 => ("CURSE", "PRIZE_CURSE"),
      4 => ("REMOVE", "PRIZE_REMOVE"),
      _ => ("DAMAGE", "PRIZE_DAMAGE"),
    };

    SetEventState(PageDescription(pageKey),
    [
      new EventOption(this, () => ApplyResult(result),
        $"{Id.Entry}.pages.RESULT.options.{optionKey}",
        Array.Empty<IHoverTip>()),
    ]);
  }

  private async Task ApplyResult(int result)
  {
    var owner = Owner;
    if (owner?.Creature == null)
      return;

    switch (result)
    {
      case 0:
        await PlayerCmd.GainGold(DynamicVars["GoldAmount"].IntValue, owner);
        break;
      case 1:
        await RewardsCmd.OfferCustom(owner,
        [
          new RelicReward(owner),
        ]);
        break;
      case 2:
        await CreatureCmd.Heal(owner.Creature, owner.Creature.MaxHp);
        break;
      case 3:
        await CardPileCmd.AddCurseToDeck<Decay>(owner);
        break;
      case 4:
        var prefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 1);
        var selectedCards = await CardSelectCmd.FromDeckForRemoval(owner, prefs);
        await CardPileCmd.RemoveFromDeck(selectedCards.ToList());
        break;
      default:
        await CreatureCmd.Damage(
          new ThrowingPlayerChoiceContext(),
          owner.Creature,
          DynamicVars["HpLoss"].BaseValue,
          ValueProp.Unblockable | ValueProp.Unpowered,
          null,
          null);
        SetEventFinished(PageDescription("DAMAGE_RESULT"));
        return;
    }

    SetEventFinished(PageDescription("LEAVE"));
  }
}
