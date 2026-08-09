using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Helpers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Runtime.Effects;

public sealed class GiantFireEffect : NSts1Effect
{
    private static readonly string AtlasPath = ModAssetPaths.Resource("vfx", "vfx.atlas");
    private const float EffectDuration = 1.5f;
    private const float ScreenWidth = 1920f;
    private const float ScreenHeight = 1080f;

    private Sprite2D? _sprite;
    private float _brightness;
    private float _velocityX;
    private float _velocityY;
    private float _delayTimer;
    private float _scale;
    private Color _color;

    public static GiantFireEffect Create()
    {
        var effect = new GiantFireEffect();
        effect.Setup();
        return effect;
    }

    protected override void Initialize()
    {
        Duration = EffectDuration;
        StartingDuration = EffectDuration;

        var flameName = Random.Shared.Next(3) switch
        {
            0 => "combat/flame4",
            1 => "combat/flame5",
            _ => "combat/flame6",
        };

        var textureRegion = LibGdxAtlas.GetRegion(AtlasPath, flameName);
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

        var imageWidth = textureRegion.Value.Region.Size.X;
        var imageHeight = textureRegion.Value.Region.Size.Y;
        var x = (float)(Random.Shared.NextDouble() * ScreenWidth) - imageWidth / 2f;
        var y = ScreenHeight + (float)(Random.Shared.NextDouble() * 200.0 + 200.0) + imageHeight / 2f;
        Position = new Vector2(x, y);

        _velocityX = (float)(Random.Shared.NextDouble() * 140.0 - 70.0);
        _velocityY = -(float)(Random.Shared.NextDouble() * 1200.0 + 500.0);

        _color = Colors.White;
        _color.A = 0f;
        var greenReduction = (float)(Random.Shared.NextDouble() * 0.5);
        _color.G -= greenReduction;
        _color.B -= greenReduction - (float)(Random.Shared.NextDouble() * 0.2);

        var rotation = (float)(Random.Shared.NextDouble() * 20.0 - 10.0);
        _scale = (float)(Random.Shared.NextDouble() * 6.5 + 0.5);
        _brightness = (float)(Random.Shared.NextDouble() * 0.4 + 0.2);
        _delayTimer = (float)(Random.Shared.NextDouble() * 0.1);

        if (Random.Shared.Next(2) == 0)
            _sprite.FlipH = true;

        _sprite.Rotation = Mathf.DegToRad(rotation);
        _sprite.Scale = Vector2.One * _scale;
        _sprite.Modulate = _color;
    }

    protected override void Update(float delta)
    {
        if (_sprite == null)
        {
            IsDone = true;
            return;
        }

        if (_delayTimer > 0f)
        {
            _delayTimer -= delta;
            return;
        }

        Position += new Vector2(_velocityX * delta, _velocityY * delta);
        _scale *= (float)(Random.Shared.NextDouble() * 0.1 + 0.95);
        _sprite.Scale = Vector2.One * _scale;

        Duration -= delta;
        if (Duration < 0f)
        {
            IsDone = true;
        }
        else if (StartingDuration - Duration < 0.75f)
        {
            _color.A = Lerp(0f, _brightness, Fade((StartingDuration - Duration) / 0.75f));
        }
        else if (Duration < 1f)
        {
            _color.A = Lerp(0f, _brightness, Fade(Duration));
        }

        _sprite.Modulate = _color;
    }

    private static float Fade(float value)
    {
        return Mathf.Clamp(value * value * value * (value * (value * 6f - 15f) + 10f), 0f, 1f);
    }
}
