using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events;

/// <summary>
/// STS1-EVENT-06 — 大图书馆：从 20 张跨职业卡牌中选择 1 张，或回复 33% 最大生命值。
/// 仅在 Act 3（Glory）出现。
/// </summary>
[RegisterSharedEvent]
public sealed class TheLibrary : BalanceEventTemplate
{
    private const int CardChoiceCount = 20;
    private const decimal HealPercent = 0.33M;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
      new HealVar(0M),
    new IntVar("CardChoiceCount", CardChoiceCount),
  ];

    public override void CalculateVars()
    {
        var owner = Owner;
        if (owner?.Creature == null)
            return;

        DynamicVars.Heal.BaseValue = Math.Floor(owner.Creature.MaxHp * HealPercent);
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        return
        [
          Option(Read),
      Option(Sleep),
    ];
    }

    private async Task Read()
    {
        var owner = Owner;
        if (owner == null)
        {
            SetEventFinished(PageDescription("SLEEP"));
            return;
        }

        var charPools = ModelDb.AllCardPools
          .Where(p => p is not ColorlessCardPool and not CurseCardPool);

        var cardResults = CardFactory.CreateForReward(
            owner,
            CardChoiceCount,
            CardCreationOptions.ForNonCombatWithDefaultOdds(charPools))
          .ToList();

        var prefs = new CardSelectorPrefs(
          L10NLookup($"{Id.Entry}.pages.READ.selectionScreenPrompt"), 1)
        {
            Cancelable = false,
        };

        var selectedCard = (await CardSelectCmd.FromSimpleGridForRewards(
          new BlockingPlayerChoiceContext(),
          cardResults,
          owner,
          prefs)).FirstOrDefault();

        if (selectedCard != null)
        {
            CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(selectedCard, PileType.Deck));
        }

        var bookIndex = Rng.NextInt(3);
        var bookText = bookIndex switch
        {
            0 => L10NLookup($"{Id.Entry}.pages.READ.description_1"),
            1 => L10NLookup($"{Id.Entry}.pages.READ.description_2"),
            _ => L10NLookup($"{Id.Entry}.pages.READ.description_3"),
        };

        SetEventFinished(bookText);
    }

    private async Task Sleep()
    {
        var owner = Owner;
        if (owner?.Creature == null)
        {
            SetEventFinished(PageDescription("SLEEP"));
            return;
        }

        await CreatureCmd.Heal(owner.Creature, DynamicVars.Heal.BaseValue);
        SetEventFinished(PageDescription("SLEEP"));
    }
}
