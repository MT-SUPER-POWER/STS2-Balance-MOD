using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using FluentAssertions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;
using Xunit;

namespace Sts2BalanceMod.Tests;

public class WellLaidPlansPatchTests
{
    [Fact]
    public void WellLaidPlans_RarityPrefix_ShouldReturnUncommon()
    {
        var card = new WellLaidPlans();
        CardRarity result = CardRarity.Rare;
        bool shouldContinue = WellLaidPlansRollbackPatch.RarityPrefix(card, ref result);

        shouldContinue.Should().BeFalse();
        result.Should().Be(CardRarity.Uncommon);
    }

    [Fact]
    public void WellLaidPlans_CanonicalEnergyCostPrefix_ShouldReturnOne()
    {
        var card = new WellLaidPlans();
        int result = 2;
        bool shouldContinue = WellLaidPlansRollbackPatch.CanonicalEnergyCostPrefix(card, ref result);

        shouldContinue.Should().BeFalse();
        result.Should().Be(1);
    }

    [Fact]
    public void WellLaidPlans_CanonicalVarsPrefix_ShouldReturnCardsVarOne()
    {
        var card = new WellLaidPlans();
        IEnumerable<DynamicVar>? result = null;
        bool shouldContinue = WellLaidPlansRollbackPatch.CanonicalVarsPrefix(card, ref result!);

        shouldContinue.Should().BeFalse();
        result.Should().NotBeNull();
        var vars = result!.ToList();
        vars.Should().HaveCount(1);
        vars[0].Name.Should().Be("Cards");
        vars[0].BaseValue.Should().Be(1m);
    }

    [Fact]
    public void WellLaidPlans_OnUpgradePrefix_ShouldUpgradeCardsVarWithoutChangingCost()
    {
        var card = new WellLaidPlans();
        IEnumerable<DynamicVar>? canonicalVars = null;
        WellLaidPlansRollbackPatch.CanonicalVarsPrefix(card, ref canonicalVars!);
        var varSet = new DynamicVarSet(canonicalVars!);
        varSet.InitializeWithOwner(card);

        typeof(CardModel).GetField("_dynamicVars", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(card, varSet);

        card.DynamicVars["Cards"].BaseValue.Should().Be(1m);

        bool shouldContinue = WellLaidPlansRollbackPatch.OnUpgradePrefix(card);
        shouldContinue.Should().BeFalse();

        card.DynamicVars["Cards"].BaseValue.Should().Be(2m);
    }

    [Theory]
    [InlineData("zhs")]
    [InlineData("eng")]
    [InlineData("ita")]
    [InlineData("rus")]
    public void Localization_ShouldContainWellLaidPlansKeys(string lang)
    {
        string locBasePath = TestPathHelper.GetPath("Sts2BalanceMod", "localization", lang);
        string cardsJsonPath = Path.Combine(locBasePath, "cards.json");
        File.Exists(cardsJsonPath).Should().BeTrue();

        using var cardsDoc = JsonDocument.Parse(File.ReadAllText(cardsJsonPath));
        cardsDoc.RootElement.TryGetProperty("WELL_LAID_PLANS.description", out _).Should().BeTrue($"cards.json in '{lang}' should contain WELL_LAID_PLANS.description");
        cardsDoc.RootElement.TryGetProperty("WELL_LAID_PLANS.selectionScreenPrompt", out _).Should().BeTrue($"cards.json in '{lang}' should contain WELL_LAID_PLANS.selectionScreenPrompt");
    }
}
