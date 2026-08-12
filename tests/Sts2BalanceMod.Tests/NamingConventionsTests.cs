using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using FluentAssertions;
using Xunit;

namespace Sts2BalanceMod.Tests;

public class NamingConventionsTests
{
    private static readonly Regex PascalCaseRegex = new(@"^[A-Z][a-zA-Z0-9]*$", RegexOptions.Compiled);

    [Fact]
    public void ImagesInModFolder_ShouldBePascalCase()
    {
        string imagesPath = TestPathHelper.GetPath("Sts2BalanceMod", "images");
        Directory.Exists(imagesPath).Should().BeTrue("images directory should exist in Sts2BalanceMod");

        var imageFiles = Directory.GetFiles(imagesPath, "*.png", SearchOption.AllDirectories)
            .Where(f => !f.Contains(".godot"))
            .ToList();

        imageFiles.Should().NotBeEmpty("image files should be present");

        var nonPascalFiles = imageFiles
            .Select(f => new
            {
                FullPath = f,
                RelativePath = Path.GetRelativePath(imagesPath, f),
                FileNameWithoutExt = Path.GetFileNameWithoutExtension(f)
            })
            .Where(x => !PascalCaseRegex.IsMatch(x.FileNameWithoutExt))
            .ToList();

        nonPascalFiles.Should().BeEmpty(
            $"all mod images should use PascalCase naming format. Violating files: {string.Join(", ", nonPascalFiles.Select(x => x.RelativePath))}"
        );
    }

    [Fact]
    public void RelicOutlines_ShouldBeInOutlinesSubdirectory()
    {
        string relicsPath = TestPathHelper.GetPath("Sts2BalanceMod", "images", "relics");
        string outlinesPath = Path.Combine(relicsPath, "outlines");

        Directory.Exists(relicsPath).Should().BeTrue("images/relics directory should exist");
        Directory.Exists(outlinesPath).Should().BeTrue("images/relics/outlines directory should exist");

        var relicMainFiles = Directory.GetFiles(relicsPath, "*.png", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileNameWithoutExtension)
            .ToList();

        var outlineFiles = Directory.GetFiles(outlinesPath, "*.png", SearchOption.TopDirectoryOnly)
            .Select(Path.GetFileNameWithoutExtension)
            .ToList();

        foreach (var relicName in relicMainFiles)
        {
            outlineFiles.Should().Contain(
                relicName,
                $"relic icon '{relicName}.png' must have a matching outline image at 'images/relics/outlines/{relicName}.png'"
            );
        }
    }

    [Fact]
    public void ImageGenSourceFiles_ShouldBePascalCase()
    {
        string imageGenSourcesPath = TestPathHelper.GetPath("image_gen");
        if (!Directory.Exists(imageGenSourcesPath))
            return;

        var sourceImages = Directory.GetFiles(imageGenSourcesPath, "*.png", SearchOption.AllDirectories)
            .Where(f => !f.Contains(".venv") && !f.Contains("node_modules"))
            .ToList();

        var nonPascalFiles = sourceImages
            .Select(f => new
            {
                RelativePath = Path.GetRelativePath(imageGenSourcesPath, f),
                FileNameWithoutExt = Path.GetFileNameWithoutExtension(f)
            })
            .Where(x => !PascalCaseRegex.IsMatch(x.FileNameWithoutExt))
            .ToList();

        nonPascalFiles.Should().BeEmpty(
            $"all source images in image_gen should use PascalCase naming. Violating files: {string.Join(", ", nonPascalFiles.Select(x => x.RelativePath))}"
        );
    }
}
