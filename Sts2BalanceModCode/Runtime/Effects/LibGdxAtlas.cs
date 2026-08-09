using Godot;

namespace Sts2BalanceMod.Sts2BalanceModCode.Runtime.Effects;

/// <summary>
/// 读取 AFP 资源所使用的 libGDX 文本图集，并缓存图集元数据与纹理。
/// </summary>
public static class LibGdxAtlas
{
    private static readonly Dictionary<string, Texture2D> TextureCache = new();
    private static readonly Dictionary<string, AtlasData> AtlasCache = new();

    public readonly record struct TextureRegion(Texture2D Texture, Rect2 Region);

    public static TextureRegion? GetRegion(string atlasPath, string regionName)
    {
        var atlasData = LoadAtlasData(atlasPath);
        if (!atlasData.Regions.TryGetValue(regionName, out var region) || region.TexturePath == null)
            return null;

        var texture = LoadTexture(region.TexturePath);
        return texture == null
          ? null
          : new TextureRegion(texture, new Rect2(region.X, region.Y, region.Width, region.Height));
    }

    private static Texture2D? LoadTexture(string path)
    {
        if (TextureCache.TryGetValue(path, out var cached))
            return cached;

        var texture = ResourceLoader.Load<Texture2D>(path);
        if (texture != null)
            TextureCache[path] = texture;

        return texture;
    }

    private static AtlasData LoadAtlasData(string atlasPath)
    {
        if (AtlasCache.TryGetValue(atlasPath, out var cached))
            return cached;

        var atlasData = ParseAtlasFile(atlasPath);
        AtlasCache[atlasPath] = atlasData;
        return atlasData;
    }

    private static AtlasData ParseAtlasFile(string atlasPath)
    {
        var data = new AtlasData();
        using var file = Godot.FileAccess.Open(atlasPath, Godot.FileAccess.ModeFlags.Read);
        if (file == null)
            return data;

        var lines = file.GetAsText(skipCr: true).Split('\n');
        var directory = atlasPath.GetBaseDir();
        string? currentTexturePath = null;
        string? currentRegion = null;
        var currentRegionData = new RegionData();

        foreach (var rawLine in lines)
        {
            var line = rawLine.TrimEnd('\r');
            if (string.IsNullOrWhiteSpace(line))
            {
                StoreCurrentRegion();
                currentRegion = null;
                continue;
            }

            if (line.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                line.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                line.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
            {
                StoreCurrentRegion();
                currentRegion = null;
                currentTexturePath = $"{directory}/{line}";
                continue;
            }

            var trimmedLine = line.Trim();
            if (trimmedLine.StartsWith("size:") ||
                trimmedLine.StartsWith("format:") ||
                trimmedLine.StartsWith("filter:") ||
                trimmedLine.StartsWith("repeat:"))
            {
                continue;
            }

            if (line.StartsWith("  ") || line.StartsWith('\t'))
            {
                if (currentRegion == null)
                    continue;

                ParseRegionProperty(trimmedLine, currentRegionData);
                continue;
            }

            StoreCurrentRegion();
            currentRegion = trimmedLine;
            currentRegionData = new RegionData();
        }

        StoreCurrentRegion();
        return data;

        void StoreCurrentRegion()
        {
            if (currentRegion == null || currentTexturePath == null)
                return;

            currentRegionData.TexturePath = currentTexturePath;
            data.Regions[currentRegion] = currentRegionData;
        }
    }

    private static void ParseRegionProperty(string line, RegionData region)
    {
        var colonIndex = line.IndexOf(':');
        if (colonIndex < 0)
            return;

        var key = line[..colonIndex].Trim();
        var values = line[(colonIndex + 1)..]
          .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        switch (key)
        {
            case "xy" when values.Length >= 2:
                region.X = int.Parse(values[0]);
                region.Y = int.Parse(values[1]);
                break;
            case "size" when values.Length >= 2:
                region.Width = int.Parse(values[0]);
                region.Height = int.Parse(values[1]);
                break;
        }
    }

    private sealed class AtlasData
    {
        public Dictionary<string, RegionData> Regions { get; } = new();
    }

    private sealed class RegionData
    {
        public string? TexturePath { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
