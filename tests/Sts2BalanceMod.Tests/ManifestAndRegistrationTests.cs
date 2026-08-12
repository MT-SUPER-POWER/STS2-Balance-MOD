using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace Sts2BalanceMod.Tests;

public class ManifestAndRegistrationTests
{
    private static readonly Regex VersionHeaderRegex = new(@"^##\s+\[?(v\d+\.\d+\.\d+)\]?", RegexOptions.Multiline);

    [Fact]
    public void ModManifest_ShouldExistAndBeValidJson()
    {
        string manifestPath = TestPathHelper.GetPath("Sts2BalanceMod.json");
        File.Exists(manifestPath).Should().BeTrue("Sts2BalanceMod.json manifest must exist at repo root");

        string content = File.ReadAllText(manifestPath);
        using var doc = JsonDocument.Parse(content);
        var root = doc.RootElement;

        root.TryGetProperty("id", out var idProp).Should().BeTrue("manifest must have 'id' property");
        idProp.GetString().Should().Be("Sts2BalanceMod");

        root.TryGetProperty("version", out var versionProp).Should().BeTrue("manifest must have 'version' property");
        versionProp.GetString().Should().MatchRegex(@"^v\d+\.\d+\.\d+$");
    }

    [Fact]
    public void ManifestVersion_ShouldMatchChangelogLatestVersion()
    {
        string manifestPath = TestPathHelper.GetPath("Sts2BalanceMod.json");
        string changelogPath = TestPathHelper.GetPath("CHANGELOG.md");

        File.Exists(changelogPath).Should().BeTrue("CHANGELOG.md must exist at repo root");

        string manifestContent = File.ReadAllText(manifestPath);
        using var doc = JsonDocument.Parse(manifestContent);
        string manifestVersion = doc.RootElement.GetProperty("version").GetString()!;

        string changelogContent = File.ReadAllText(changelogPath);
        var matches = VersionHeaderRegex.Matches(changelogContent);

        matches.Should().NotBeEmpty("CHANGELOG.md should contain at least one version header like '## vX.X.X'");
        string latestChangelogVersion = matches[0].Groups[1].Value;

        manifestVersion.Should().Be(
            latestChangelogVersion,
            $"Sts2BalanceMod.json version '{manifestVersion}' must match latest CHANGELOG.md version '{latestChangelogVersion}'"
        );
    }
}
