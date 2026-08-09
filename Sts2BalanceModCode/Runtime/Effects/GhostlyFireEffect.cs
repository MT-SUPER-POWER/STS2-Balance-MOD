using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Runtime.Effects;

public sealed class GhostlyFireEffect : NSts1Effect
{
  private static readonly string AtlasPath = ModAssetPaths.Resource("vfx", "vfx.atlas");

  private static readonly string[] FireRegions = ["env/fire1", "env/fire2", "env/fire3"];

  private Sprite2D? _sprite;
  private float _x;
  private float _y;
  private float _velocityX;
  private float _velocityY;
  private float _scale;
  private Color _color;

  public static GhostlyFireEffect Create(float x, float y)
  {
    var effect = new GhostlyFireEffect
    {
      _x = x + (float)GD.RandRange(-2.0, 2.0),
      _y = y + (float)GD.RandRange(-2.0, 2.0),
      _velocityX = (float)GD.RandRange(-10.0, 10.0),
      _velocityY = (float)GD.RandRange(-150.0, -20.0),
    };
    effect.Setup();
    return effect;
  }

  protected override void Initialize()
  {
    Duration = 1f;
    StartingDuration = 1f;

    var regionName = FireRegions[Random.Shared.Next(FireRegions.Length)];
    var textureRegion = LibGdxAtlas.GetRegion(AtlasPath, regionName);
    if (textureRegion == null)
    {
      IsDone = true;
      return;
    }

    _sprite = new Sprite2D
    {
      Texture = textureRegion.Value.Texture,
      RegionEnabled = true,
      RegionRect = textureRegion.Value.Region,
      Centered = true,
      Material = CreateAdditiveMaterial(),
    };
    AddChild(_sprite);

    _scale = (float)GD.RandRange(5.0, 6.0);
    _color = new Color(0.5f, 1f, 0f, 0f);
    Position = new Vector2(_x, _y);
    UpdateSprite();
  }

  protected override void Update(float delta)
  {
    _x += _velocityX * delta;
    _y += _velocityY * delta;

    Duration -= delta;
    if (Duration < 0f)
    {
      IsDone = true;
      return;
    }

    if (_scale > 0.1f)
      _scale -= delta / 4f;

    _color.A = Duration / 2f;
    Position = new Vector2(_x, _y);
    UpdateSprite();
  }

  private void UpdateSprite()
  {
    if (_sprite == null)
      return;

    var wobbleX = (float)GD.RandRange(0.95, 1.05);
    var wobbleY = (float)GD.RandRange(0.95, 1.05);
    _sprite.Scale = new Vector2(_scale * wobbleX, _scale * wobbleY);
    _sprite.Modulate = _color;
  }

  private static CanvasItemMaterial CreateAdditiveMaterial()
  {
    return new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add };
  }
}
