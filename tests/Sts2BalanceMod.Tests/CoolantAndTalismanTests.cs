using System.IO;
using System.Text.Json;
using FluentAssertions;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Relics;
using Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;
using Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;
using Xunit;

namespace Sts2BalanceMod.Tests;

public class CoolantAndTalismanTests
{
    [Fact]
    public void CoolantRarity_Prefix_ShouldReturnUncommon()
    {
        var coolant = new Coolant();
        CardRarity result = CardRarity.Rare;
        bool shouldContinue = CoolantPatch.RarityPrefix(coolant, ref result);

        shouldContinue.Should().BeFalse();
        result.Should().Be(CardRarity.Uncommon);
    }

    [Fact]
    public void NeowsTalisman_RemainingCharges_WithoutHistory_ShouldDefaultToThree()
    {
        var talisman = new NeowsTalisman();
        int remaining = NeowsTalismanPatch.GetRemainingCharges(talisman);
        bool isUsedUp = NeowsTalismanPatch.GetIsUsedUp(talisman);
        bool isActive = NeowsTalismanPatch.IsLamentActive(talisman);

        remaining.Should().Be(3);
        isUsedUp.Should().BeFalse();
        isActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("zhs")]
    [InlineData("eng")]
    [InlineData("ita")]
    [InlineData("rus")]
    public void Localization_ShouldContainCoolantAndNeowsTalismanKeys(string lang)
    {
        string locBasePath = TestPathHelper.GetPath("Sts2BalanceMod", "localization", lang);

        // Check cards.json
        string cardsJsonPath = Path.Combine(locBasePath, "cards.json");
        File.Exists(cardsJsonPath).Should().BeTrue();
        using var cardsDoc = JsonDocument.Parse(File.ReadAllText(cardsJsonPath));
        cardsDoc.RootElement.TryGetProperty("COOLANT.description", out _).Should().BeTrue($"cards.json in '{lang}' should contain COOLANT.description");

        // Check powers.json
        string powersJsonPath = Path.Combine(locBasePath, "powers.json");
        File.Exists(powersJsonPath).Should().BeTrue();
        using var powersDoc = JsonDocument.Parse(File.ReadAllText(powersJsonPath));
        powersDoc.RootElement.TryGetProperty("COOLANT_POWER.description", out _).Should().BeTrue($"powers.json in '{lang}' should contain COOLANT_POWER.description");

        // Check relics.json
        string relicsJsonPath = Path.Combine(locBasePath, "relics.json");
        File.Exists(relicsJsonPath).Should().BeTrue();
        using var relicsDoc = JsonDocument.Parse(File.ReadAllText(relicsJsonPath));
        relicsDoc.RootElement.TryGetProperty("NEOWS_TALISMAN.description", out _).Should().BeTrue($"relics.json in '{lang}' should contain NEOWS_TALISMAN.description");
    }
}
