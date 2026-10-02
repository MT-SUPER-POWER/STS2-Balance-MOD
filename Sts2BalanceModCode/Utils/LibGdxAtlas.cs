using Godot;

namespace Sts2BalanceMod.Sts2BalanceModCode.Utils;

/// <summary>
/// LibGDX Atlas 纹理图集解析工具类，供事件与 UI 提取切片纹理。
/// </summary>
public static class LibGdxAtlas
{
  private static readonly Dictionary<string, Texture2D> _textureCache = new();
  private static readonly Dictionary<string, AtlasData> _atlasCache = new();

  public struct TextureRegion
  {
    public Texture2D Texture;
    public Rect2 Region;
  }

  public struct RegionInfo
  {
    public int X;
    public int Y;
    public int Width;
    public int Height;
    public int OrigWidth;
    public int OrigHeight;
    public int OffsetX;
    public int OffsetY;
    public bool Rotate;
  }

  public static RegionInfo? GetRegionData(string atlasPath, string regionName)
  {
    AtlasData atlasData = LoadAtlasData(atlasPath);
    if (!atlasData.Regions.TryGetValue(regionName, out RegionData? region) || region == null)
    {
      return null;
    }

    return new RegionInfo
    {
      X = region.X,
      Y = region.Y,
      Width = region.Width,
      Height = region.Height,
      OrigWidth = region.OrigWidth,
      OrigHeight = region.OrigHeight,
      OffsetX = region.OffsetX,
      OffsetY = region.OffsetY,
      Rotate = region.Rotate
    };
  }

  public static TextureRegion? GetRegion(string atlasPath, string regionName)
  {
    AtlasData atlasData = LoadAtlasData(atlasPath);
    if (!atlasData.Regions.TryGetValue(regionName, out RegionData? region) || region == null)
    {
      return null;
    }

    Texture2D? baseTexture = LoadTexture(region.TexturePath);
    if (baseTexture == null)
    {
      return null;
    }

    return new TextureRegion
    {
      Texture = baseTexture,
      Region = new Rect2(region.X, region.Y, region.Width, region.Height)
    };
  }

  private static Texture2D? LoadTexture(string path)
  {
    if (_textureCache.TryGetValue(path, out Texture2D? cached))
    {
      return cached;
    }

    Texture2D? texture = GD.Load<Texture2D>(path);
    if (texture == null)
    {
      return null;
    }

    _textureCache[path] = texture;
    return texture;
  }

  private static AtlasData LoadAtlasData(string atlasPath)
  {
    if (_atlasCache.TryGetValue(atlasPath, out AtlasData? cached))
    {
      return cached;
    }

    AtlasData atlasData = ParseAtlasFile(atlasPath);
    _atlasCache[atlasPath] = atlasData;
    return atlasData;
  }

  private static AtlasData ParseAtlasFile(string atlasPath)
  {
    AtlasData data = new();
    using Godot.FileAccess? fileContent = Godot.FileAccess.Open(atlasPath, Godot.FileAccess.ModeFlags.Read);
    if (fileContent == null)
    {
      return data;
    }

    string text = fileContent.GetAsText(true);
    string[] lines = text.Split('\n');
    string directory = atlasPath.GetBaseDir();
    string? currentTexturePath = null;
    string? currentRegion = null;
    RegionData currentRegionData = new();

    for (int i = 0; i < lines.Length; i++)
    {
      string line = lines[i].Trim('\r');
      if (string.IsNullOrWhiteSpace(line))
      {
        if (currentRegion != null && currentTexturePath != null)
        {
          currentRegionData.TexturePath = currentTexturePath;
          data.Regions[currentRegion] = currentRegionData;
        }
        currentRegion = null;
        continue;
      }

      if (line.EndsWith(".png") || line.EndsWith(".jpg") || line.EndsWith(".jpeg"))
      {
        if (currentRegion != null && currentTexturePath != null)
        {
          currentRegionData.TexturePath = currentTexturePath;
          data.Regions[currentRegion] = currentRegionData;
          currentRegion = null;
        }
        currentTexturePath = directory + "/" + line;
        continue;
      }

      if (line.StartsWith("size:") || line.StartsWith("format:") ||
          line.StartsWith("filter:") || line.StartsWith("repeat:"))
      {
        continue;
      }

      if (lines[i].StartsWith("  ") || lines[i].StartsWith("\t"))
      {
        if (currentRegion == null)
        {
          continue;
        }

        int colonIndex = line.IndexOf(':');
        if (colonIndex == -1)
        {
          continue;
        }

        string key = line[..colonIndex].Trim();
        string value = line[(colonIndex + 1)..].Trim();
        switch (key)
        {
          case "xy":
            string[] xy = value.Split(',');
            currentRegionData.X = int.Parse(xy[0].Trim());
            currentRegionData.Y = int.Parse(xy[1].Trim());
            break;
          case "size":
            string[] size = value.Split(',');
            currentRegionData.Width = int.Parse(size[0].Trim());
            currentRegionData.Height = int.Parse(size[1].Trim());
            break;
          case "orig":
            string[] orig = value.Split(',');
            currentRegionData.OrigWidth = int.Parse(orig[0].Trim());
            currentRegionData.OrigHeight = int.Parse(orig[1].Trim());
            break;
          case "offset":
            string[] offset = value.Split(',');
            currentRegionData.OffsetX = int.Parse(offset[0].Trim());
            currentRegionData.OffsetY = int.Parse(offset[1].Trim());
            break;
          case "rotate":
            currentRegionData.Rotate = value == "true";
            break;
        }
      }
      else
      {
        if (currentRegion != null && currentTexturePath != null)
        {
          currentRegionData.TexturePath = currentTexturePath;
          data.Regions[currentRegion] = currentRegionData;
        }
        currentRegion = line;
        currentRegionData = new RegionData();
      }
    }

    if (currentRegion != null && currentTexturePath != null)
    {
      currentRegionData.TexturePath = currentTexturePath;
      data.Regions[currentRegion] = currentRegionData;
    }

    return data;
  }

  private sealed class AtlasData
  {
    public Dictionary<string, RegionData> Regions { get; } = [];
  }

  private sealed class RegionData
  {
    public string TexturePath { get; set; } = "";
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int OrigWidth { get; set; }
    public int OrigHeight { get; set; }
    public int OffsetX { get; set; }
    public int OffsetY { get; set; }
    public bool Rotate { get; set; }
  }
}
