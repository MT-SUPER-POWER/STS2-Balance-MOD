using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;

namespace Sts2BalanceMod.src.Runtime.Effects;

public sealed class FireBurstParticleEffect : NSts1Effect
{
  private static readonly string AtlasPath = ModAssetPaths.Resource("vfx", "vfx.atlas");
  private const float Gravity = 180f;

  private static readonly string[] FireRegions = ["env/fire1", "env/fire2", "env/fire3"];

  private Sprite2D? _sprite;
  private float _x;
  private float _y;
  private float _velocityX;
  private float _velocityY;
  private float _floor;
  private float _scale;
  private float _rotation;
  private Color _color;

  public static FireBurstParticleEffect Create(float x, float y)
  {
    var effect = new FireBurstParticleEffect { _x = x, _y = y };
    effect.Setup();
    return effect;
  }

  protected override void Initialize()
  {
    Duration = (float)GD.RandRange(0.5, 1.0);
    StartingDuration = Duration;

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
      Material = new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add },
    };
    AddChild(_sprite);

    _color = new Color(
      (float)GD.RandRange(0.1, 0.3),
      (float)GD.RandRange(0.8, 1.0),
      (float)GD.RandRange(0.1, 0.3),
      0f);
    _rotation = (float)GD.RandRange(-10.0, 10.0);
    _scale = (float)GD.RandRange(2.0, 4.0);
    _velocityX = (float)GD.RandRange(-900.0, 900.0);
    _velocityY = (float)GD.RandRange(-500.0, 0.0);
    _floor = _y + (float)GD.RandRange(100.0, 250.0);

    Position = new Vector2(_x, _y);
    UpdateSprite();
  }

  protected override void Update(float delta)
  {
    _velocityY += Gravity / _scale * delta;
    _x += _velocityX * delta * Mathf.Sin(delta);
    _y += _velocityY * delta;

    if (_scale > 0.3f)
      _scale -= delta * 2f;

    if (_y > _floor)
    {
      _velocityY = -_velocityY * 0.75f;
      _y = _floor - 0.1f;
      _velocityX *= 1.1f;
    }

    var progress = 1f - Duration / StartingDuration;
    _color.A = progress < 0.1f ? EaseOut(progress * 10f) : MathF.Pow(Duration / StartingDuration, 2f);

    Duration -= delta;
    if (Duration < 0f)
    {
      IsDone = true;
      return;
    }

    Position = new Vector2(_x, _y);
    UpdateSprite();
  }

  private void UpdateSprite()
  {
    if (_sprite == null)
      return;

    _sprite.RotationDegrees = _rotation;
    _sprite.Scale = Vector2.One * _scale;
    _sprite.Modulate = _color;
  }
}
