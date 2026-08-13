using System;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using MegaCrit.Sts2.Core.Events;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using Xunit;

namespace Sts2BalanceMod.Tests;

public class AssetExistenceTests
{
    private static Assembly GetModAssembly() => typeof(BalanceCardTemplate).Assembly;

    [Fact]
    public void Cards_ShouldHaveMatchingPortraitImages()
    {
        string cardPortraitsPath = TestPathHelper.GetPath("Sts2BalanceMod", "images", "card_portraits");
        Directory.Exists(cardPortraitsPath).Should().BeTrue("images/card_portraits directory must exist");

        var cardTypes = GetModAssembly().GetTypes()
            .Where(t => !t.IsAbstract && typeof(BalanceCardTemplate).IsAssignableFrom(t))
            .ToList();

        cardTypes.Should().NotBeEmpty("mod assembly should contain concrete card classes");

        foreach (var cardType in cardTypes)
        {
            string expectedImageName = $"{cardType.Name}.png";
            string expectedImagePath = Path.Combine(cardPortraitsPath, expectedImageName);

            File.Exists(expectedImagePath).Should().BeTrue(
                $"card class '{cardType.Name}' must have a matching portrait image at 'images/card_portraits/{expectedImageName}'"
            );
        }
    }

    [Fact]
    public void Relics_ShouldHaveMatchingIconAndOutlineImages()
    {
        string relicsPath = TestPathHelper.GetPath("Sts2BalanceMod", "images", "relics");
        string outlinesPath = Path.Combine(relicsPath, "outlines");

        Directory.Exists(relicsPath).Should().BeTrue("images/relics directory must exist");
        Directory.Exists(outlinesPath).Should().BeTrue("images/relics/outlines directory must exist");

        var relicTypes = GetModAssembly().GetTypes()
            .Where(t => !t.IsAbstract && typeof(BalanceRelicTemplate).IsAssignableFrom(t))
            .ToList();

        relicTypes.Should().NotBeEmpty("mod assembly should contain concrete relic classes");

        foreach (var relicType in relicTypes)
        {
            string expectedImageName = $"{relicType.Name}.png";
            string expectedIconPath = Path.Combine(relicsPath, expectedImageName);
            string expectedOutlinePath = Path.Combine(outlinesPath, expectedImageName);

            File.Exists(expectedIconPath).Should().BeTrue(
                $"relic class '{relicType.Name}' must have a matching icon image at 'images/relics/{expectedImageName}'"
            );
            File.Exists(expectedOutlinePath).Should().BeTrue(
                $"relic class '{relicType.Name}' must have a matching outline image at 'images/relics/outlines/{expectedImageName}'"
            );
        }
    }

    [Fact]
    public void Events_ShouldHaveMatchingEventImages()
    {
        string eventsPath = TestPathHelper.GetPath("Sts2BalanceMod", "images", "events");
        Directory.Exists(eventsPath).Should().BeTrue("images/events directory must exist");

        var eventTypes = GetModAssembly().GetTypes()
            .Where(t => !t.IsAbstract && typeof(BalanceEventTemplate).IsAssignableFrom(t))
            .ToList();

        eventTypes.Should().NotBeEmpty("mod assembly should contain concrete event classes");

        foreach (var eventType in eventTypes)
        {
            // 战斗型事件没有单独背景图，跳过背景图校验
            var instance = (BalanceEventTemplate)Activator.CreateInstance(eventType)!;
            if (instance.LayoutType == EventLayoutType.Combat)
                continue;

            string expectedImageName = $"{eventType.Name}.png";
            string expectedImagePath = Path.Combine(eventsPath, expectedImageName);

            File.Exists(expectedImagePath).Should().BeTrue(
                $"event class '{eventType.Name}' must have a matching background image at 'images/events/{expectedImageName}'"
            );
        }
    }

    [Fact]
    public void Powers_ShouldHaveMatchingPowerIcons()
    {
        string powersPath = TestPathHelper.GetPath("Sts2BalanceMod", "images", "powers");
        Directory.Exists(powersPath).Should().BeTrue("images/powers directory must exist");

        var powerTypes = GetModAssembly().GetTypes()
            .Where(t => !t.IsAbstract && typeof(BalancePowerTemplate).IsAssignableFrom(t))
            .ToList();

        foreach (var powerType in powerTypes)
        {
            string expectedImageName = $"{powerType.Name}.png";
            string expectedImagePath = Path.Combine(powersPath, expectedImageName);

            File.Exists(expectedImagePath).Should().BeTrue(
                $"power class '{powerType.Name}' must have a matching icon image at 'images/powers/{expectedImageName}'"
            );
        }
    }

    [Fact]
    public void RestSiteOptions_ShouldHaveMatchingOptionImages()
    {
        string restSitePath = TestPathHelper.GetPath("Sts2BalanceMod", "images", "ui", "rest_site");
        Directory.Exists(restSitePath).Should().BeTrue("images/ui/rest_site directory must exist");

        var optionTypes = GetModAssembly().GetTypes()
            .Where(t => !t.IsAbstract && typeof(BalanceRestSiteOption).IsAssignableFrom(t))
            .ToList();

        optionTypes.Should().NotBeEmpty("mod assembly should contain concrete rest site option classes");

        foreach (var optionType in optionTypes)
        {
            var instance = (BalanceRestSiteOption)System.Runtime.CompilerServices.RuntimeHelpers.GetUninitializedObject(optionType);
            string expectedImageName = ModAssetPaths.ContentFileName($"Option_{instance.OptionId}");
            string expectedImagePath = Path.Combine(restSitePath, expectedImageName);

            File.Exists(expectedImagePath).Should().BeTrue(
                $"rest site option class '{optionType.Name}' must have a matching icon image at 'images/ui/rest_site/{expectedImageName}'"
            );
        }
    }
}
