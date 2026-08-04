using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Effects;

public sealed class GhostlyWeakFireEffect : NSts1Effect
{
  private const string AtlasPath = "res://Sts2BalanceMod/vfx/vfx.atlas";

  private static readonly string[] FireRegions = ["env/fire1", "env/fire2", "env/fire3"];

  private Sprite2D? _sprite;
  private float _x;
  private float _y;
  private float _velocityX;
  private float _velocityY;
  private float _scale;
  private Color _color;

  public static GhostlyWeakFireEffect Create(float x, float y)
  {
    var effect = new GhostlyWeakFireEffect
    {
      _x = x + (float)GD.RandRange(-2.0, 2.0),
      _y = y + (float)GD.RandRange(-2.0, 2.0),
      _velocityX = (float)GD.RandRange(-2.0, 2.0),
      _velocityY = (float)GD.RandRange(-80.0, 0.0),
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

    _scale = (float)GD.RandRange(2.0, 3.0);
    _color = new Color(0.53f, 0.81f, 0.92f, 0f);
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

    _color.A = Duration / 2f;
    Position = new Vector2(_x, _y);
    UpdateSprite();
  }

  private void UpdateSprite()
  {
    if (_sprite == null)
      return;

    _sprite.Scale = Vector2.One * _scale;
    _sprite.Modulate = _color;
  }

  private static CanvasItemMaterial CreateAdditiveMaterial()
  {
    return new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add };
  }
}
