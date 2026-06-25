using Godot;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using Sts2BalanceMod.Sts2BalanceModCode.Utility;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events.UI;

/// <summary>
/// STS1-EVENT-07 — 大转盘自定义 UI 覆盖层。
/// 简化版移植自 ActsFromThePast.Minigames.NWheelSpinScreen。
/// 移除：粒子效果、控制器支持、atlas 背景（使用纯色背景代替）。
/// 保留：bounce-in 动画、旋转动画（linear spin + elastic deceleration）、bounce-out 动画。
/// </summary>
public partial class NWheelSpinScreen : Control, IOverlayScreen
{
  // ─── IOverlayScreen / IScreenContext ───
  public NetScreenType ScreenType => NetScreenType.None;
  public bool UseSharedBackstop => false;
  public Control? DefaultFocusedControl => this;
  // ─── 布局 ───
  private const float WheelDisplaySize = 1024f;
  private const float ArrowDisplaySize = 512f;
  private const float ButtonDisplaySize = 512f;
  private const float ArrowOffsetX = 480f;
  private const float WheelAngleOffset = 0f;
  private const float ButtonCenterX = -460f;
  private const float ButtonFinalY = 330f;
  private const float ButtonStartY = 900f;

  // ─── 动画参数 ───
  private const float BounceInDuration = 1.5f;
  private const float SpinDuration = 2f;
  private const float SpinVelocity = 1500f;
  private const float DecelerateDuration = 3f;
  private const float PauseDuration = 1f;
  private const float BounceOutDuration = 0.8f;
  private const float WheelStartOffset = -600f;
  private const float WheelBaseY = 50f;

  // ─── 资源路径 ───
  private const string WheelTexPath = "res://Sts2BalanceMod/images/event_extras/wheel.png";
  private const string ArrowTexPath = "res://Sts2BalanceMod/images/event_extras/wheelArrow.png";
  private const string ButtonTexPath = "res://Sts2BalanceMod/images/event_extras/spinButton.png";
  private const string SpinSfxPath = "res://Sts2BalanceMod/sfx/events/wheel.ogg";

  // ─── 实例 ───
  private static NWheelSpinScreen? _instance;
  private WheelSpinMinigame _minigame = null!;

  // ─── UI 元素 ───
  private TextureRect _wheelRect = null!;
  private TextureRect _arrowRect = null!;
  private TextureRect _buttonRect = null!;
  private TextureRect _buttonGlowRect = null!;

  // ─── Tween 状态 ───
  private Tween? _mainTween;
  private Tween? _glowTween;
  private bool _spinning;

  public static NWheelSpinScreen ShowScreen(WheelSpinMinigame minigame)
  {
    if (_instance != null && IsInstanceValid(_instance))
      _instance.QueueFree();

    var screen = new NWheelSpinScreen
    {
      _minigame = minigame,
    };
    screen.BuildUI();
    screen.BindEvents();
    _instance = screen;
    NOverlayStack.Instance.Push((IOverlayScreen)screen);
    screen.StartBounceIn();
    return screen;
  }

  private void BindEvents()
  {
    _minigame.Finished += OnMinigameFinished;
  }

  private void UnbindEvents()
  {
    _minigame.Finished -= OnMinigameFinished;
  }

  public override void _ExitTree()
  {
    UnbindEvents();
    KillAllTweens();
    _minigame.ForceEnd();
    _instance = null;
  }

  private void KillAllTweens()
  {
    _mainTween?.Kill();
    _glowTween?.Kill();
  }

  // ─── IOverlayScreen ───

  public void AfterOverlayOpened() { }
  public void AfterOverlayClosed()
  {
    KillAllTweens();
    this.QueueFreeSafely();
  }
  public void AfterOverlayShown() { }
  public void AfterOverlayHidden() { }

  // ─── UI 构建 ───

  private void BuildUI()
  {
    SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

    // 半透明黑色背景
    var bg = new ColorRect
    {
      Color = new Color(0f, 0f, 0f, 0.7f),
      MouseFilter = MouseFilterEnum.Ignore,
    };
    bg.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
    AddChild(bg);

    // 转盘
    var wheelTex = GD.Load<Texture2D>(WheelTexPath);
    float halfWheel = WheelDisplaySize / 2f;
    _wheelRect = new TextureRect
    {
      Texture = wheelTex,
      CustomMinimumSize = new Vector2(WheelDisplaySize, WheelDisplaySize),
      ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
      StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
      PivotOffset = new Vector2(halfWheel, halfWheel),
      AnchorLeft = 0.5f, AnchorTop = 0.5f,
      AnchorRight = 0.5f, AnchorBottom = 0.5f,
      OffsetLeft = -halfWheel, OffsetTop = -halfWheel + WheelStartOffset,
      OffsetRight = halfWheel, OffsetBottom = halfWheel + WheelStartOffset,
      GrowHorizontal = GrowDirection.Both,
      GrowVertical = GrowDirection.Both,
      MouseFilter = MouseFilterEnum.Ignore,
    };
    AddChild(_wheelRect);

    // 箭头（固定在转盘右侧）
    var arrowTex = GD.Load<Texture2D>(ArrowTexPath);
    float halfArrow = ArrowDisplaySize / 2f;
    _arrowRect = new TextureRect
    {
      Texture = arrowTex,
      CustomMinimumSize = new Vector2(ArrowDisplaySize, ArrowDisplaySize),
      ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
      StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
      AnchorLeft = 0.5f, AnchorTop = 0.5f,
      AnchorRight = 0.5f, AnchorBottom = 0.5f,
      OffsetLeft = ArrowOffsetX - halfArrow, OffsetTop = -halfArrow + WheelStartOffset,
      OffsetRight = ArrowOffsetX + halfArrow, OffsetBottom = halfArrow + WheelStartOffset,
      GrowHorizontal = GrowDirection.Both,
      GrowVertical = GrowDirection.Both,
      MouseFilter = MouseFilterEnum.Ignore,
    };
    AddChild(_arrowRect);

    // 旋转按钮
    var buttonTex = GD.Load<Texture2D>(ButtonTexPath);
    float halfButton = ButtonDisplaySize / 2f;
    _buttonRect = new TextureRect
    {
      Texture = buttonTex,
      CustomMinimumSize = new Vector2(ButtonDisplaySize, ButtonDisplaySize),
      ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
      StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
      PivotOffset = new Vector2(halfButton, halfButton),
      AnchorLeft = 0.5f, AnchorTop = 0.5f,
      AnchorRight = 0.5f, AnchorBottom = 0.5f,
      GrowHorizontal = GrowDirection.Both,
      GrowVertical = GrowDirection.Both,
      MouseFilter = MouseFilterEnum.Stop,
      Visible = false,
    };
    _buttonRect.Connect(Control.SignalName.GuiInput,
      Callable.From<InputEvent>(ev =>
      {
        if (_spinning) return;
        if (ev is InputEventMouseButton mb && mb.Pressed && mb.ButtonIndex == MouseButton.Left)
        {
          _buttonRect.AcceptEvent();
          StartSpinning();
        }
      }));
    _buttonRect.Connect(Control.SignalName.MouseEntered,
      Callable.From(() => SetButtonHovered(true)));
    _buttonRect.Connect(Control.SignalName.MouseExited,
      Callable.From(() => SetButtonHovered(false)));
    AddChild(_buttonRect);

    // 按钮发光叠加层
    _buttonGlowRect = new TextureRect
    {
      Texture = buttonTex,
      CustomMinimumSize = new Vector2(ButtonDisplaySize, ButtonDisplaySize),
      ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
      StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
      PivotOffset = new Vector2(halfButton, halfButton),
      AnchorLeft = 0.5f, AnchorTop = 0.5f,
      AnchorRight = 0.5f, AnchorBottom = 0.5f,
      GrowHorizontal = GrowDirection.Both,
      GrowVertical = GrowDirection.Both,
      MouseFilter = MouseFilterEnum.Ignore,
      Material = new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add },
      Visible = false,
    };
    AddChild(_buttonGlowRect);

    // 初始透明
    Modulate = new Color(1f, 1f, 1f, 0f);

    // 设置初始按钮位置（独立于转盘动画）
    _buttonRect.OffsetLeft = ButtonCenterX - halfButton;
    _buttonRect.OffsetRight = ButtonCenterX + halfButton;
    _buttonRect.OffsetTop = ButtonStartY + WheelBaseY - halfButton;
    _buttonRect.OffsetBottom = ButtonStartY + WheelBaseY + halfButton;
    _buttonGlowRect.OffsetLeft = _buttonRect.OffsetLeft;
    _buttonGlowRect.OffsetRight = _buttonRect.OffsetRight;
    _buttonGlowRect.OffsetTop = _buttonRect.OffsetTop;
    _buttonGlowRect.OffsetBottom = _buttonRect.OffsetBottom;
  }

  // ─── 弹入动画 ───

  private void StartBounceIn()
  {
    _mainTween?.Kill();
    _mainTween = CreateTween();
    _mainTween.SetParallel(true);

    // 淡入
    _mainTween.TweenProperty(this, "modulate", new Color(1f, 1f, 1f, 1f), BounceInDuration)
      .From(new Color(1f, 1f, 1f, 0f))
      .SetTrans(Tween.TransitionType.Cubic)
      .SetEase(Tween.EaseType.Out);

    // 转盘从上方弹入
    _mainTween.TweenMethod(
      Callable.From<float>(offset => SetWheelVerticalOffset(offset)),
      WheelStartOffset, 0f, BounceInDuration
    ).SetTrans(Tween.TransitionType.Bounce)
     .SetEase(Tween.EaseType.Out);

    // 箭头的垂直位置也跟随
    _mainTween.TweenMethod(
      Callable.From<float>(offset => _arrowRect.OffsetTop = -ArrowDisplaySize / 2f + WheelBaseY + offset),
      WheelStartOffset, 0f, BounceInDuration
    ).SetTrans(Tween.TransitionType.Bounce)
     .SetEase(Tween.EaseType.Out);

    _mainTween.TweenMethod(
      Callable.From<float>(offset => _arrowRect.OffsetBottom = ArrowDisplaySize / 2f + WheelBaseY + offset),
      WheelStartOffset, 0f, BounceInDuration
    ).SetTrans(Tween.TransitionType.Bounce)
     .SetEase(Tween.EaseType.Out);

    _mainTween.SetParallel(false);
    _mainTween.TweenCallback(Callable.From(() =>
    {
      // 转盘到位后，显示并滑入按钮
      _buttonRect.Visible = true;
      _buttonGlowRect.Visible = true;
      SlideButtonIn();
      StartGlowPulse();
    }));
  }

  private void SetWheelVerticalOffset(float offset)
  {
    float halfWheel = WheelDisplaySize / 2f;
    _wheelRect.OffsetTop = -halfWheel + WheelBaseY + offset;
    _wheelRect.OffsetBottom = halfWheel + WheelBaseY + offset;
  }

  // ─── 按钮动画 ───

  private void SlideButtonIn()
  {
    var tween = CreateTween();
    tween.TweenMethod(
      Callable.From<float>(y =>
      {
        float halfButton = ButtonDisplaySize / 2f;
        _buttonRect.OffsetTop = y - halfButton;
        _buttonRect.OffsetBottom = y + halfButton;
        _buttonGlowRect.OffsetTop = _buttonRect.OffsetTop;
        _buttonGlowRect.OffsetBottom = _buttonRect.OffsetBottom;
      }),
      ButtonStartY + WheelBaseY, ButtonFinalY + WheelBaseY, 0.6f
    ).SetTrans(Tween.TransitionType.Back)
     .SetEase(Tween.EaseType.Out);
  }

  private void SlideButtonOut()
  {
    var tween = CreateTween();
    tween.TweenMethod(
      Callable.From<float>(y =>
      {
        float halfButton = ButtonDisplaySize / 2f;
        _buttonRect.OffsetTop = y - halfButton;
        _buttonRect.OffsetBottom = y + halfButton;
        _buttonGlowRect.OffsetTop = _buttonRect.OffsetTop;
        _buttonGlowRect.OffsetBottom = _buttonRect.OffsetBottom;
      }),
      ButtonFinalY + WheelBaseY, ButtonStartY + WheelBaseY, 0.4f
    ).SetTrans(Tween.TransitionType.Back)
     .SetEase(Tween.EaseType.In);

    tween.TweenCallback(Callable.From(() =>
    {
      _buttonRect.Visible = false;
      _buttonGlowRect.Visible = false;
    }));
  }

  // ─── 按钮发光脉冲 ───

  private void StartGlowPulse()
  {
    _glowTween?.Kill();
    _glowTween = CreateTween();
    _glowTween.SetLoops();

    _glowTween.TweenMethod(
      Callable.From<float>(a => _buttonGlowRect.Modulate = new Color(1f, 1f, 1f, a)),
      0.07f, 0.35f, 0.8f
    ).SetTrans(Tween.TransitionType.Sine)
     .SetEase(Tween.EaseType.InOut);

    _glowTween.TweenMethod(
      Callable.From<float>(a => _buttonGlowRect.Modulate = new Color(1f, 1f, 1f, a)),
      0.35f, 0.07f, 0.8f
    ).SetTrans(Tween.TransitionType.Sine)
     .SetEase(Tween.EaseType.InOut);
  }

  private void SetButtonHovered(bool hovered)
  {
    if (_buttonRect == null || _spinning) return;
    _buttonRect.Scale = hovered ? Vector2.One * 1.05f : Vector2.One;
    _buttonGlowRect.Scale = _buttonRect.Scale;

    if (hovered)
    {
      _glowTween?.Kill();
      _buttonGlowRect.Modulate = new Color(1f, 1f, 1f, 0.25f);
    }
    else
    {
      StartGlowPulse();
    }
  }

  // ─── 旋转动画 ───

  private void StartSpinning()
  {
    _spinning = true;
    _glowTween?.Kill();
    SlideButtonOut();

    Sts2ModAudio.PlayOneShot(SpinSfxPath);

    float resultAngle = _minigame.ResultAngle;
    float spinEnd = SpinVelocity * SpinDuration; // ~3000°

    _mainTween?.Kill();
    _mainTween = CreateTween();

    // 阶段1：匀速高速旋转
    _mainTween.TweenMethod(
      Callable.From<float>(angle => _wheelRect.RotationDegrees = -angle + WheelAngleOffset),
      0f, spinEnd, SpinDuration
    ).SetTrans(Tween.TransitionType.Linear);

    // 阶段2：弹性减速，模拟 STS1 的 ElasticIn
    _mainTween.TweenMethod(
      Callable.From<float>(t => _wheelRect.RotationDegrees = -ElasticLerp(resultAngle, -180f, t) + WheelAngleOffset),
      1.0f, 0.0f, DecelerateDuration
    ).SetTrans(Tween.TransitionType.Linear);

    _mainTween.TweenCallback(Callable.From(() =>
    {
      // 微调最终角度
      _wheelRect.RotationDegrees = -resultAngle + WheelAngleOffset;
    }));

    // 停顿后弹出
    _mainTween.TweenInterval(PauseDuration);
    _mainTween.TweenCallback(Callable.From(StartBounceOut));
  }

  // ─── 弹出动画 ───

  private void StartBounceOut()
  {
    _mainTween?.Kill();
    _mainTween = CreateTween();
    _mainTween.SetParallel(true);

    // 淡出
    _mainTween.TweenProperty(this, "modulate", new Color(1f, 1f, 1f, 0f), BounceOutDuration)
      .SetTrans(Tween.TransitionType.Cubic)
      .SetEase(Tween.EaseType.In);

    // 转盘向上滑出
    _mainTween.TweenMethod(
      Callable.From<float>(offset => SetWheelVerticalOffset(offset)),
      0f, WheelStartOffset, BounceOutDuration
    ).SetTrans(Tween.TransitionType.Back)
     .SetEase(Tween.EaseType.In);

    _mainTween.SetParallel(false);
    _mainTween.TweenCallback(Callable.From(() => _minigame.Complete()));
  }

  // ─── Elastic In 插值 ───

  private static float ElasticIn(float a)
  {
    if (a >= 0.99f) return 1f;
    if (a <= 0f) return 0f;
    float raw = -(float)(Math.Pow(2, 10 * (a - 1))
      * Math.Sin((a - 1.1f) * 900f * Mathf.Pi / 180f));
    if (a < 0.5f)
    {
      float damp = a / 0.5f;
      raw *= damp * damp;
    }
    return raw;
  }

  private static float ElasticLerp(float from, float to, float t)
  {
    return from + (to - from) * ElasticIn(t);
  }

  // ─── 清理 ───

  private void OnMinigameFinished()
  {
    NOverlayStack.Instance.Remove((IOverlayScreen)this);
  }
}
