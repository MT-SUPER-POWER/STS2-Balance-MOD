using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Runs;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events;

/// <summary>
/// STS1-EVENT-09 - 老乞丐：支付金币后显露牧师身份，并允许移除一张牌。
/// 来源参考 ActsFromThePast.Acts.TheCity.Events.OldBeggar。
/// </summary>
public sealed class OldBeggar : CustomEventModel
{
  private const int GoldCost = 75;
  private const string ClericPortraitPath = "res://Sts2BalanceMod/images/events/cleric.png";

  public override ActModel[] Acts => [];

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new IntVar("GoldCost", GoldCost),
  ];

  public override bool IsAllowed(IRunState runState)
  {
    return runState.Players.All(p => p.Gold >= GoldCost);
  }

  protected override IReadOnlyList<EventOption> GenerateInitialOptions()
  {
    var owner = Owner;
    if (owner == null)
      return [Option(Leave)];

    var options = new List<EventOption>();
    if (owner.Gold >= GoldCost)
    {
      options.Add(Option(GiveGold));
    }
    else
    {
      options.Add(new EventOption(this, null,
        $"{Id.Entry}.pages.INITIAL.options.GIVE_GOLD_LOCKED",
        Array.Empty<IHoverTip>()));
    }

    options.Add(Option(Leave));
    return options;
  }

  private async Task GiveGold()
  {
    var owner = Owner;
    if (owner == null)
    {
      SetEventFinished(PageDescription("LEAVE"));
      return;
    }

    await PlayerCmd.LoseGold(GoldCost, owner, GoldLossType.Spent);
    SetEventState(PageDescription("GAVE_GOLD"),
    [
      Option(RemoveCard, "GAVE_GOLD"),
    ]);
    SwitchToClericPortrait();
  }

  private async Task RemoveCard()
  {
    var owner = Owner;
    if (owner == null)
    {
      SetEventFinished(PageDescription("LEAVE"));
      return;
    }

    var prefs = new CardSelectorPrefs(CardSelectorPrefs.RemoveSelectionPrompt, 1);
    var selectedCards = await CardSelectCmd.FromDeckForRemoval(owner, prefs);
    await CardPileCmd.RemoveFromDeck(selectedCards.ToList());
    SetEventFinished(PageDescription("REMOVE_CARD"));
  }

  private Task Leave()
  {
    SetEventFinished(PageDescription("LEAVE"));
    return Task.CompletedTask;
  }

  private void SwitchToClericPortrait()
  {
    if (Node?.FindChild("Portrait", true, false) is TextureRect portrait)
    {
      portrait.Texture = PreloadManager.Cache.GetTexture2D(ClericPortraitPath);
    }
  }
}
