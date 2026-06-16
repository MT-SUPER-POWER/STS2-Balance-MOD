using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Cards;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events;

/// <summary>
/// STS1-EVENT-03 — 增益研究者：获得 J.A.X.、变化两张牌或获得突变之力。
/// 来源参考 ActsFromThePast.Acts.TheCity.Events.Augmenter。
/// </summary>
public sealed class Augmenter : CustomEventModel
{
  public override ActModel[] Acts => [];

  public override bool IsAllowed(IRunState runState)
  {
    return runState.Players.All(p => p.Deck.Cards.Count(c => c.IsRemovable) >= 2);
  }

  protected override IReadOnlyList<EventOption> GenerateInitialOptions()
  {
    var owner = Owner;
    if (owner == null)
      return [];

    var options = new List<EventOption>
    {
      Option(Jax, "INITIAL", HoverTipFactory.FromCard(ModelDb.Card<Jax>())),
    };

    if (owner.Deck.Cards.Count(c => c.IsRemovable) >= 2)
      options.Add(Option(Transform));
    else
      options.Add(new EventOption(this, null,
        $"{Id.Entry}.pages.INITIAL.options.TRANSFORM_LOCKED",
        Array.Empty<IHoverTip>()));

    options.Add(Option(Mutagens, "INITIAL",
      HoverTipFactory.FromRelic(ModelDb.Relic<MutagenicStrength>()).ToArray()));

    return options;
  }

  private async Task Jax()
  {
    var owner = Owner;
    if (owner == null)
      return;

    var jax = owner.RunState.CreateCard(ModelDb.Card<Jax>(), owner);
    var result = await CardPileCmd.Add(jax, PileType.Deck);
    CardCmd.PreviewCardPileAdd(result, 2f);
    SetEventFinished(PageDescription("JAX"));
  }

  private async Task Transform()
  {
    var owner = Owner;
    if (owner == null)
      return;

    var prefs = new CardSelectorPrefs(
      L10NLookup($"{Id.Entry}.pages.TRANSFORM.selectionScreenPrompt"), 2);
    var selectedCards = await CardSelectCmd.FromDeckForTransformation(owner, prefs);
    foreach (var card in selectedCards.ToList())
      await CardCmd.TransformToRandom(card, owner.RunState.Rng.Niche, CardPreviewStyle.HorizontalLayout);

    SetEventFinished(PageDescription("TRANSFORM"));
  }

  private async Task Mutagens()
  {
    var owner = Owner;
    if (owner == null)
      return;

    await RelicCmd.Obtain(ModelDb.Relic<MutagenicStrength>().ToMutable(), owner);
    SetEventFinished(PageDescription("MUTAGENS"));
  }
}
