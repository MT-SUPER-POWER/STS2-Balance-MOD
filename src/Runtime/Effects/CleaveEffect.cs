using Godot;

namespace Sts2BalanceMod.src.Runtime.Effects;

/// <summary>
/// 守护者旋风攻击使用的一代劈砍特效。
/// </summary>
public sealed class CleaveEffect : NSts1Effect
{
  private static readonly string AtlasPath = ModAssetPaths.Resource("vfx", "vfx.atlas");
  private const float FadeInTime = 0.05f;
  private const float FadeOutTime = 0.4f;

  private Sprite2D _sprite = null!;
  private Sprite2D _additiveSprite = null!;
  private float _velocityX;
  private float _fadeInTimer;
  private float _fadeOutTimer;
  private float _stallTimer;
  private float _scale;
  private float _rotation;
  private float _alpha;

  public static CleaveEffect Create(Vector2 position)
  {
    var effect = new CleaveEffect { Position = position };
    effect.Setup();
    return effect;
  }

  protected override void Initialize()
  {
    _fadeInTimer = FadeInTime;
    _fadeOutTimer = FadeOutTime;
    _stallTimer = (float)GD.RandRange(0f, 0.2f);
    _scale = 1.2f;
    _rotation = (float)GD.RandRange(-5f, 1f);
    _velocityX = 100f;

    var textureRegion = LibGdxAtlas.GetRegion(AtlasPath, "combat/cleave");
    if (textureRegion == null)
    {
      IsDone = true;
      return;
    }

    _sprite = CreateSprite(textureRegion.Value, additive: false);
    _additiveSprite = CreateSprite(textureRegion.Value, additive: true);
    AddChild(_sprite);
    AddChild(_additiveSprite);
    UpdateSprites();
  }

  protected override void Update(float delta)
  {
    if (_stallTimer > 0f)
    {
      _stallTimer -= delta;
      return;
    }

    Position += new Vector2(_velocityX * delta, 0f);
    _rotation += (float)GD.RandRange(-0.5f, 0.5f);
    _scale += 0.005f;

    if (_fadeInTimer > 0f)
    {
      _fadeInTimer = Math.Max(0f, _fadeInTimer - delta);
      var t = _fadeInTimer / FadeInTime;
      _alpha = Fade(1f - t);
    }
    else if (_fadeOutTimer > 0f)
    {
      _fadeOutTimer = Math.Max(0f, _fadeOutTimer - delta);
      var t = _fadeOutTimer / FadeOutTime;
      _alpha = t * t;
    }
    else
    {
      IsDone = true;
      return;
    }

    UpdateSprites();
  }

  private static Sprite2D CreateSprite(LibGdxAtlas.TextureRegion region, bool additive)
  {
    var sprite = new Sprite2D
    {
      Texture = region.Texture,
      RegionEnabled = true,
      RegionRect = region.Region,
      Centered = true,
    };
    if (additive)
    {
      sprite.Material = new CanvasItemMaterial
      {
        BlendMode = CanvasItemMaterial.BlendModeEnum.Add,
      };
    }

    return sprite;
  }

  private void UpdateSprites()
  {
    foreach (var sprite in new[] { _sprite, _additiveSprite })
    {
      sprite.Scale = Vector2.One * _scale;
      sprite.RotationDegrees = _rotation;
      sprite.Modulate = new Color(1f, 1f, 1f, _alpha);
    }
  }

  private static float Fade(float t)
  {
    return t * t * t * (t * (t * 6f - 15f) + 10f);
  }
}
