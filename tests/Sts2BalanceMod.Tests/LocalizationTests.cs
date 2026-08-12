using System.IO;
using System.Linq;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace Sts2BalanceMod.Tests;

public class LocalizationTests
{
    private static readonly string[] LocLanguages = ["zhs", "eng"];
    private static readonly string[] RequiredJsonFiles =
    [
        "cards.json",
        "relics.json",
        "powers.json",
        "events.json",
        "monsters.json",
        "encounters.json",
        "enchantments.json"
    ];

    [Theory]
    [InlineData("zhs")]
    [InlineData("eng")]
    public void LocalizationJsonFiles_ShouldExistAndBeValidJson(string lang)
    {
        string langDir = TestPathHelper.GetPath("Sts2BalanceMod", "localization", lang);
        Directory.Exists(langDir).Should().BeTrue($"localization directory for '{lang}' must exist");

        foreach (string file in RequiredJsonFiles)
        {
            string filePath = Path.Combine(langDir, file);
            File.Exists(filePath).Should().BeTrue($"localization file '{lang}/{file}' must exist");

            string content = File.ReadAllText(filePath);
            var act = () => JsonDocument.Parse(content);
            act.Should().NotThrow($"file '{lang}/{file}' should be valid JSON");
        }
    }

    [Fact]
    public void PlaceholdersInJsonFiles_ShouldHaveMatchingBraces()
    {
        string locBasePath = TestPathHelper.GetPath("Sts2BalanceMod", "localization");

        foreach (string lang in LocLanguages)
        {
            string langDir = Path.Combine(locBasePath, lang);
            if (!Directory.Exists(langDir))
                continue;

            var jsonFiles = Directory.GetFiles(langDir, "*.json", SearchOption.AllDirectories);
            foreach (string jsonFile in jsonFiles)
            {
                string jsonContent = File.ReadAllText(jsonFile);
                using var doc = JsonDocument.Parse(jsonContent);

                ValidateBraceMatchingInJsonElement(doc.RootElement, Path.GetRelativePath(locBasePath, jsonFile));
            }
        }
    }

    private static void ValidateBraceMatchingInJsonElement(JsonElement element, string contextPath)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in element.EnumerateObject())
            {
                ValidateBraceMatchingInJsonElement(prop.Value, $"{contextPath} -> {prop.Name}");
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            int index = 0;
            foreach (var item in element.EnumerateArray())
            {
                ValidateBraceMatchingInJsonElement(item, $"{contextPath}[{index++}]");
            }
        }
        else if (element.ValueKind == JsonValueKind.String)
        {
            string text = element.GetString() ?? string.Empty;
            int openBraces = text.Count(c => c == '{');
            int closeBraces = text.Count(c => c == '}');

            openBraces.Should().Be(
                closeBraces,
                $"unmatched braces found in localization text at '{contextPath}': \"{text}\""
            );
        }
    }
}
