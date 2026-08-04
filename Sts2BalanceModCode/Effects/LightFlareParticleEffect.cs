using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Effects;

public sealed class LightFlareParticleEffect : NSts1Effect
{
  private const string AtlasPath = "res://Sts2BalanceMod/vfx/vfx.atlas";
  private const string BlurRegion = "combat/blurDot";

  private Sprite2D? _sprite;
  private Sprite2D? _glowSprite;
  private float _x;
  private float _y;
  private float _speed;
  private float _speedStart;
  private float _speedTarget;
  private float _waveIntensity;
  private float _waveSpeed;
  private float _rotation;
  private float _scale;
  private Color _color;

  public static LightFlareParticleEffect Create(float x, float y, Color color)
  {
    color.A = 0f;
    var effect = new LightFlareParticleEffect
    {
      _x = x,
      _y = y,
      _color = color,
    };
    effect.Setup();
    return effect;
  }

  protected override void Initialize()
  {
    Duration = (float)GD.RandRange(0.5, 1.1);
    StartingDuration = Duration;

    var textureRegion = LibGdxAtlas.GetRegion(AtlasPath, BlurRegion);
    if (textureRegion == null)
    {
      IsDone = true;
      return;
    }

    var material = new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add };
    _glowSprite = new Sprite2D
    {
      Texture = textureRegion.Value.Texture,
      RegionEnabled = true,
      RegionRect = textureRegion.Value.Region,
      Centered = true,
      Material = material,
      ZIndex = -1,
    };
    AddChild(_glowSprite);

    _sprite = new Sprite2D
    {
      Texture = textureRegion.Value.Texture,
      RegionEnabled = true,
      RegionRect = textureRegion.Value.Region,
      Centered = true,
      Material = material,
    };
    AddChild(_sprite);

    _speed = (float)GD.RandRange(200.0, 300.0);
    _speedStart = _speed;
    _speedTarget = (float)GD.RandRange(0.1, 0.5);
    _rotation = (float)GD.RandRange(0.0, 360.0);
    _waveIntensity = (float)GD.RandRange(5.0, 10.0);
    _waveSpeed = (float)GD.RandRange(-20.0, 20.0);
    _scale = (float)GD.RandRange(0.2, 1.0);

    Position = new Vector2(_x, _y);
    UpdateSprite();
  }

  protected override void Update(float delta)
  {
    var radians = Mathf.DegToRad(_rotation);
    _x += Mathf.Cos(radians) * _speed * delta;
    _y += Mathf.Sin(radians) * _speed * delta;

    var progress = 1f - Duration / StartingDuration;
    _speed = Lerp(_speedStart, _speedTarget, Mathf.Sqrt(progress));
    _rotation += Mathf.Cos(Duration * _waveSpeed) * _waveIntensity;
    _color.A = Duration < 0.5f ? EaseOut(Duration * 2f) : 1f;

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
    if (_sprite == null || _glowSprite == null)
      return;

    _sprite.RotationDegrees = _rotation;
    _sprite.Scale = Vector2.One * _scale;
    _sprite.Modulate = _color;

    _glowSprite.RotationDegrees = _rotation;
    _glowSprite.Scale = Vector2.One * (_scale * 4f);
    _glowSprite.Modulate = new Color(_color.R, _color.G, _color.B, _color.A / 4f);
  }
}
