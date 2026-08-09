using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.src.Cards;
using Sts2BalanceMod.src.Relics;

using Sts2BalanceMod.src.Abstract;

namespace Sts2BalanceMod.src.Events;

/// <summary>
/// STS1-EVENT-03 — 增益研究者：获得 J.A.X.、变化两张牌或获得突变之力。
/// 来源参考 ActsFromThePast.Acts.TheCity.Events.Augmenter。
/// </summary>
[RegisterActEvent(typeof(Hive))]
public sealed class Augmenter : BalanceEventTemplate
{
  /// <summary>仅限 Act 2（Hive）出现，太早遇到 J.A.X. 过于强大。</summary>

  public override bool IsAllowed(IRunState runState)
  {
    if (runState.CurrentActIndex != 1) return false;
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
