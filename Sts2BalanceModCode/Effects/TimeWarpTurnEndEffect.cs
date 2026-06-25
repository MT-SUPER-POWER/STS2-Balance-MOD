using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Sts2BalanceMod.Sts2BalanceModCode.Effects;

/// <summary>
/// STS1-BOSS-01 — Time Warp 触发时的时钟弹出视觉效果。
/// 从屏幕底部弹入一个旋转时钟图标，2 秒后淡出消失。
/// 参考 ActsFromThePast.TimeWarpTurnEndEffect 移植。
/// （不使用 NSts1Effect 基类，该基类在游戏中不可用；改用 Node2D + Tween）
/// </summary>
public partial class TimeWarpTurnEndEffect : Node2D
{
  private const string AtlasPath = "res://Sts2BalanceMod/images/powers/time_warp.png";
  // 128/time region in powers.atlas: position (92,390), size (86,87), original 128x128
  private static readonly Vector2 AtlasRegionPos = new(92f, 390f);
  private static readonly Vector2 AtlasRegionSize = new(86f, 87f);

  private Sprite2D? _sprite;
  private Tween? _tween;
  private float _duration = 2f;

  public static TimeWarpTurnEndEffect Create()
  {
    var effect = new TimeWarpTurnEndEffect();
    effect.Initialize();
    return effect;
  }

  private void Initialize()
  {
    var texture = ResourceLoader.Load<Texture2D>(AtlasPath);
    if (texture == null)
    {
      QueueFree();
      return;
    }

    _sprite = new Sprite2D
    {
      Texture = texture,
      RegionEnabled = true,
      RegionRect = new Rect2(AtlasRegionPos, AtlasRegionSize),
      Centered = true,
    };
    AddChild(_sprite);

    var viewport = GetViewport();
    var viewportSize = viewport?.GetVisibleRect().Size ?? new Vector2(1920, 1080);

    float scale = 3f;
    _sprite.Scale = new Vector2(scale, scale);
    _sprite.SelfModulate = new Color(1f, 1f, 1f, 1f);

    float x = viewportSize.X * 0.5f;
    float startY = viewportSize.Y + AtlasRegionSize.Y / 2f * scale;
    float targetY = viewportSize.Y * 0.5f;

    Position = new Vector2(x, startY);

    // 动画：弹入 + 旋转 + 淡出
    _tween = CreateTween();
    _tween.SetParallel(true);

    // 弹入：从底部到中央，使用 Back.Out easing
    var bounceTween = CreateTween();
    bounceTween.TweenProperty(this, "position", new Vector2(x, targetY), 1.0f)
      .SetTrans(Tween.TransitionType.Back)
      .SetEase(Tween.EaseType.Out);

    // 旋转：整段匀速旋转
    _tween.TweenCallback(Callable.From(() =>
    {
      var rotateTween = CreateTween();
      rotateTween.TweenMethod(
        Callable.From<float>(angle =>
        {
          if (_sprite != null)
            _sprite.Rotation = angle;
        }),
        0f, Mathf.Pi * 4f, 2.0f  // 两整圈
      ).SetTrans(Tween.TransitionType.Linear);
    }));

    // 1秒后开始淡出
    _tween.TweenInterval(1.0f);
    _tween.TweenProperty(this, "modulate", new Color(1f, 1f, 1f, 0f), 1.0f)
      .SetTrans(Tween.TransitionType.Cubic)
      .SetEase(Tween.EaseType.In);

    // 完成后自毁
    _tween.TweenCallback(Callable.From(QueueFree));
  }

  public override void _ExitTree()
  {
    _tween?.Kill();
    _tween = null;
    base._ExitTree();
  }
}
