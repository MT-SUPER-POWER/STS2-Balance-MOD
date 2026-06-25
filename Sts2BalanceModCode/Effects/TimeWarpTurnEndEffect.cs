using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Sts2BalanceMod.Sts2BalanceModCode.Effects;

/// <summary>
/// STS1-BOSS-01 — Time Warp 触发时的时钟弹出视觉效果。
/// 从屏幕底部弹入一个旋转时钟图标，2 秒后淡出消失。
///
/// 注意：不继承 Node2D，避免 Godot 源码生成器创建绑定方法导致 MonoMod JIT 崩溃。
/// 改用内建 Node2D 节点，通过 Root 属性暴露给调用方添加到场景树。
/// </summary>
public sealed class TimeWarpTurnEndEffect
{
  private const string AtlasPath = "res://Sts2BalanceMod/images/powers/time_warp.png";
  private static readonly Vector2 AtlasRegionPos = new(92f, 390f);
  private static readonly Vector2 AtlasRegionSize = new(86f, 87f);

  private readonly Node2D _root;
  private Sprite2D? _sprite;
  private Tween? _tween;
  private bool _disposed;

  /// <summary>
  /// 将此节点添加到场景树后，效果自动播放。
  /// </summary>
  public Node2D Root => _root;

  private TimeWarpTurnEndEffect()
  {
    _root = new Node2D();
    _root.TreeExited += () => Dispose();
  }

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
      _root.QueueFree();
      _disposed = true;
      return;
    }

    _sprite = new Sprite2D
    {
      Texture = texture,
      RegionEnabled = true,
      RegionRect = new Rect2(AtlasRegionPos, AtlasRegionSize),
      Centered = true,
    };
    _root.AddChild(_sprite);

    // 注意：GetViewport 需要在 _root 进入场景树后才能正常工作。
    // 这里设一个默认值，在场景树中会自动适配。
    float x = 960f;    // 默认 1920/2
    float targetY = 540f;   // 默认 1080/2
    float startY = 1200f;   // 默认屏幕下方

    float scale = 3f;
    _sprite.Scale = new Vector2(scale, scale);
    _sprite.SelfModulate = new Color(1f, 1f, 1f, 1f);

    _root.Position = new Vector2(x, startY);

    // 动画：弹入 + 旋转 + 淡出（用 _root 创建和管理 Tween）
    _tween = _root.CreateTween();
    _tween.SetParallel(true);

    // 弹入
    var bounceTween = _root.CreateTween();
    bounceTween.TweenProperty(_root, "position", new Vector2(x, targetY), 1.0f)
      .SetTrans(Tween.TransitionType.Back)
      .SetEase(Tween.EaseType.Out);

    // 旋转
    _tween.TweenCallback(Callable.From(() =>
    {
      var rotateTween = _root.CreateTween();
      rotateTween.TweenMethod(
        Callable.From<float>(angle =>
        {
          if (_sprite != null)
            _sprite.Rotation = angle;
        }),
        0f, Mathf.Pi * 4f, 2.0f
      ).SetTrans(Tween.TransitionType.Linear);
    }));

    // 1 秒后淡出
    _tween.TweenInterval(1.0f);
    _tween.TweenProperty(_root, "modulate", new Color(1f, 1f, 1f, 0f), 1.0f)
      .SetTrans(Tween.TransitionType.Cubic)
      .SetEase(Tween.EaseType.In);

    // 完成后自毁
    _tween.TweenCallback(Callable.From(() =>
    {
      if (!_disposed)
      {
        _disposed = true;
        _tween?.Kill();
        _root.QueueFree();
      }
    }));
  }

  /// <summary>
  /// 外部调用，在 _root 被移出场景树时通知。
  /// </summary>
  public void Dispose()
  {
    if (_disposed) return;
    _disposed = true;
    _tween?.Kill();
    _root.QueueFree();
  }
}
