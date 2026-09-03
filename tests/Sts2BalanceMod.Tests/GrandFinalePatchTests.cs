using System.Reflection;
using FluentAssertions;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;
using Xunit;

namespace Sts2BalanceMod.Tests;

public class GrandFinalePatchTests
{
    [Fact]
    public void GrandFinaleHasEnergyCostXPatch_Prefix_ShouldReturnTrueForGrandFinale()
    {
        var card = new GrandFinale();
        bool result = false;
        bool shouldContinue = GrandFinaleHasEnergyCostXPatch.Prefix(card, ref result);

        shouldContinue.Should().BeFalse();
        result.Should().BeTrue();
    }

    [Fact]
    public void GrandFinaleEnergyToSpendPatch_ShouldHaveHarmonyPatchAttribute()
    {
        var patchAttr = typeof(GrandFinaleEnergyToSpendPatch).GetCustomAttribute<HarmonyPatch>();
        patchAttr.Should().NotBeNull();
        patchAttr!.info.declaringType.Should().Be(typeof(CardEnergyCost));
        patchAttr.info.methodName.Should().Be(nameof(CardEnergyCost.GetAmountToSpend));
    }

    [Fact]
    public void GrandFinaleIsPlayablePatch_ShouldHaveHarmonyPatchAttribute()
    {
        var patchAttr = typeof(GrandFinaleIsPlayablePatch).GetCustomAttribute<HarmonyPatch>();
        patchAttr.Should().NotBeNull();
        patchAttr!.info.declaringType.Should().Be(typeof(GrandFinale));
        patchAttr.info.methodName.Should().Be("get_IsPlayable");
    }

    [Fact]
    public void GrandFinaleCanonicalUpgradePatch_Prefix_ShouldReturnFalseToNotAddDamage()
    {
        var card = new GrandFinale();
        bool shouldContinue = GrandFinaleCanonicalUpgradePatch.Prefix(card);

        shouldContinue.Should().BeFalse("upgrade should not call vanilla to add damage");
    }
}
