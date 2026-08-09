using Godot;

namespace Sts2BalanceMod.src.Runtime.Effects;

/// <summary>
/// IntenseZoomEffect 使用的放射状光锥粒子。
/// </summary>
public sealed class IntenseZoomParticle : NSts1Effect
{
  private static readonly string AtlasPath = ModAssetPaths.Resource("vfx", "vfx.atlas");
  private const float EffectDuration = 1.5f;

  private Sprite2D _sprite = null!;
  private Vector2 _basePosition;
  private bool _isBlack;
  private float _flickerTimer;
  private float _offsetX;
  private float _lengthX;
  private float _lengthY;
  private float _alpha;

  public static IntenseZoomParticle Create(Vector2 position, bool isBlack)
  {
    var effect = new IntenseZoomParticle
    {
      _basePosition = position,
      _isBlack = isBlack,
    };
    effect.Setup();
    return effect;
  }

  protected override void Initialize()
  {
    Duration = EffectDuration;
    StartingDuration = EffectDuration;
    var coneName = GD.RandRange(0, 2) switch
    {
      0 => "cone8",
      1 => "cone5",
      _ => "cone6",
    };

    var textureRegion = LibGdxAtlas.GetRegion(AtlasPath, coneName);
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
      Centered = false,
      Offset = new Vector2(0f, -textureRegion.Value.Region.Size.Y / 2f),
    };
    if (!_isBlack)
    {
      _sprite.Material = new CanvasItemMaterial
      {
        BlendMode = CanvasItemMaterial.BlendModeEnum.Add,
      };
    }

    AddChild(_sprite);
    Position = _basePosition;
    RandomizeParticle();
  }

  protected override void Update(float delta)
  {
    Duration -= delta;
    _flickerTimer -= delta;
    if (_flickerTimer < 0f)
    {
      RandomizeParticle();
      _flickerTimer = (float)GD.RandRange(0f, 0.05f);
    }

    if (Duration < 0f)
    {
      IsDone = true;
      return;
    }

    _sprite.Scale = new Vector2(_lengthX, _lengthY);
    _sprite.Position = new Vector2(_offsetX, 0f);
    var color = _isBlack ? Colors.Black : new Color(0.937f, 0.808f, 0.373f);
    _sprite.Modulate = new Color(color.R, color.G, color.B, _alpha);
  }

  private void RandomizeParticle()
  {
    Root.RotationDegrees = (float)GD.RandRange(0f, 360f);
    var durationFactor = 2f - Duration;
    _offsetX = (float)GD.RandRange(200f, 600f) * durationFactor;
    _lengthX = (float)GD.RandRange(1f, 1.3f);
    _lengthY = (float)GD.RandRange(0.9f, 1.2f);
    var fade = 1f - (1f - Duration / EffectDuration) * (1f - Duration / EffectDuration);
    _alpha = (float)GD.RandRange(_isBlack ? 0.5f : 0.2f, _isBlack ? 1f : 0.7f) * fade;
  }
}
