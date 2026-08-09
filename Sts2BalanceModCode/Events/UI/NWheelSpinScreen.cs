using Godot;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Audio;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Combat;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events.UI;

/// <summary>
/// STS1-EVENT-07 — 大转盘自定义 UI。
///
/// 注意：此类不继承 Godot.Control，也不实现 IOverlayScreen，
/// 避免 Godot 源码生成器创建 InvokeGodotClassMethod / GetGodotClassPropertyValue
/// （MonoMod JIT hook 编译这些方法时抛 ArgumentException）。
///
/// 改用内建 Godot 节点（Control / TextureRect / Tween 等），
/// 直接作为 NOverlayStack 的子节点加入场景树，手动管理生命周期。
/// </summary>
public sealed class NWheelSpinScreen
{
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
    private static readonly string WheelTexPath = ModAssetPaths.Resource("images", "event_extras", "wheel.png");
    private static readonly string ArrowTexPath = ModAssetPaths.Resource("images", "event_extras", "wheelArrow.png");
    private static readonly string ButtonTexPath = ModAssetPaths.Resource("images", "event_extras", "spinButton.png");
    private static readonly string SpinSfxPath = ModAssetPaths.Resource("sfx", "events", "wheel.ogg");

    // ─── 实例 ───
    private static NWheelSpinScreen? _instance;
    private readonly WheelSpinMinigame _minigame;

    // ─── Godot 节点 ───
    private readonly Control _root;
    private readonly ColorRect _backdrop;
    private TextureRect _wheelRect = null!;
    private TextureRect _arrowRect = null!;
    private TextureRect _buttonRect = null!;
    private TextureRect _buttonGlowRect = null!;

    // ─── Tween 状态 ───
    private Tween? _mainTween;
    private Tween? _glowTween;
    private bool _spinning;

    private NWheelSpinScreen(WheelSpinMinigame minigame)
    {
        _minigame = minigame;

        _root = new Control();
        _root.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _root.MouseFilter = Control.MouseFilterEnum.Ignore;

        // 遮罩层：阻止点击穿透
        _backdrop = new ColorRect
        {
            Color = new Color(0f, 0f, 0f, 0.7f),
            MouseFilter = Control.MouseFilterEnum.Stop,
        };
        _backdrop.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        _root.AddChild(_backdrop);

        BuildWheel();
        BindEvents();
    }

    public static NWheelSpinScreen ShowScreen(WheelSpinMinigame minigame)
    {
        if (_instance != null)
            DisposeInstance();

        var screen = new NWheelSpinScreen(minigame);
        _instance = screen;

        // 直接添加到 OverlayStack，不经过 Push（避免 IOverlayScreen 要求）
        var stack = NOverlayStack.Instance;
        stack?.AddChild(screen._root);
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

    private static void DisposeInstance()
    {
        if (_instance == null)
            return;
        _instance.UnbindEvents();
        _instance.KillAllTweens();
        _instance._minigame.ForceEnd();
        _instance._root.QueueFree();
        _instance = null;
    }

    private void KillAllTweens()
    {
        _mainTween?.Kill();
        _glowTween?.Kill();
    }

    private void BuildWheel()
    {
        var wheelTex = GD.Load<Texture2D>(WheelTexPath);
        float halfWheel = WheelDisplaySize / 2f;
        _wheelRect = new TextureRect
        {
            Texture = wheelTex,
            CustomMinimumSize = new Vector2(WheelDisplaySize, WheelDisplaySize),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            PivotOffset = new Vector2(halfWheel, halfWheel),
            AnchorLeft = 0.5f,
            AnchorTop = 0.5f,
            AnchorRight = 0.5f,
            AnchorBottom = 0.5f,
            OffsetLeft = -halfWheel,
            OffsetTop = -halfWheel + WheelStartOffset,
            OffsetRight = halfWheel,
            OffsetBottom = halfWheel + WheelStartOffset,
            GrowHorizontal = Control.GrowDirection.Both,
            GrowVertical = Control.GrowDirection.Both,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        _root.AddChild(_wheelRect);

        var arrowTex = GD.Load<Texture2D>(ArrowTexPath);
        float halfArrow = ArrowDisplaySize / 2f;
        _arrowRect = new TextureRect
        {
            Texture = arrowTex,
            CustomMinimumSize = new Vector2(ArrowDisplaySize, ArrowDisplaySize),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            AnchorLeft = 0.5f,
            AnchorTop = 0.5f,
            AnchorRight = 0.5f,
            AnchorBottom = 0.5f,
            OffsetLeft = ArrowOffsetX - halfArrow,
            OffsetTop = -halfArrow + WheelStartOffset,
            OffsetRight = ArrowOffsetX + halfArrow,
            OffsetBottom = halfArrow + WheelStartOffset,
            GrowHorizontal = Control.GrowDirection.Both,
            GrowVertical = Control.GrowDirection.Both,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };
        _root.AddChild(_arrowRect);

        var buttonTex = GD.Load<Texture2D>(ButtonTexPath);
        float halfButton = ButtonDisplaySize / 2f;
        _buttonRect = new TextureRect
        {
            Texture = buttonTex,
            CustomMinimumSize = new Vector2(ButtonDisplaySize, ButtonDisplaySize),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            PivotOffset = new Vector2(halfButton, halfButton),
            AnchorLeft = 0.5f,
            AnchorTop = 0.5f,
            AnchorRight = 0.5f,
            AnchorBottom = 0.5f,
            GrowHorizontal = Control.GrowDirection.Both,
            GrowVertical = Control.GrowDirection.Both,
            MouseFilter = Control.MouseFilterEnum.Stop,
            Visible = false,
        };
        _buttonRect.GuiInput += ev =>
        {
            if (_spinning)
                return;
            if (ev is InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left })
            {
                _buttonRect.AcceptEvent();
                StartSpinning();
            }
        };
        _buttonRect.MouseEntered += () => SetButtonHovered(true);
        _buttonRect.MouseExited += () => SetButtonHovered(false);
        _root.AddChild(_buttonRect);

        _buttonGlowRect = new TextureRect
        {
            Texture = buttonTex,
            CustomMinimumSize = new Vector2(ButtonDisplaySize, ButtonDisplaySize),
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
            PivotOffset = new Vector2(halfButton, halfButton),
            AnchorLeft = 0.5f,
            AnchorTop = 0.5f,
            AnchorRight = 0.5f,
            AnchorBottom = 0.5f,
            GrowHorizontal = Control.GrowDirection.Both,
            GrowVertical = Control.GrowDirection.Both,
            MouseFilter = Control.MouseFilterEnum.Ignore,
            Material = new CanvasItemMaterial { BlendMode = CanvasItemMaterial.BlendModeEnum.Add },
            Visible = false,
        };
        _root.AddChild(_buttonGlowRect);

        // 初始透明
        _root.Modulate = new Color(1f, 1f, 1f, 0f);

        // 初始按钮位置
        SetButtonRect(ButtonStartY + WheelBaseY);
    }

    private void SetButtonRect(float y)
    {
        float halfButton = ButtonDisplaySize / 2f;
        _buttonRect.OffsetLeft = ButtonCenterX - halfButton;
        _buttonRect.OffsetRight = ButtonCenterX + halfButton;
        _buttonRect.OffsetTop = y - halfButton;
        _buttonRect.OffsetBottom = y + halfButton;
        _buttonGlowRect.OffsetLeft = _buttonRect.OffsetLeft;
        _buttonGlowRect.OffsetRight = _buttonRect.OffsetRight;
        _buttonGlowRect.OffsetTop = _buttonRect.OffsetTop;
        _buttonGlowRect.OffsetBottom = _buttonRect.OffsetBottom;
    }

    // ─── 弹入动画 ───

    private void StartBounceIn()
    {
        _mainTween?.Kill();
        _mainTween = _root.CreateTween();
        _mainTween.SetParallel(true);

        _mainTween.TweenProperty(_root, "modulate",
          new Color(1f, 1f, 1f, 1f), BounceInDuration)
          .From(new Color(1f, 1f, 1f, 0f))
          .SetTrans(Tween.TransitionType.Cubic)
          .SetEase(Tween.EaseType.Out);

        _mainTween.TweenMethod(
          Callable.From<float>(offset =>
          {
              float halfWheel = WheelDisplaySize / 2f;
              _wheelRect.OffsetTop = -halfWheel + WheelBaseY + offset;
              _wheelRect.OffsetBottom = halfWheel + WheelBaseY + offset;
              _arrowRect.OffsetTop = -ArrowDisplaySize / 2f + WheelBaseY + offset;
              _arrowRect.OffsetBottom = ArrowDisplaySize / 2f + WheelBaseY + offset;
          }),
          WheelStartOffset, 0f, BounceInDuration
        ).SetTrans(Tween.TransitionType.Bounce)
         .SetEase(Tween.EaseType.Out);

        _mainTween.SetParallel(false);
        _mainTween.TweenCallback(Callable.From(() =>
        {
            _buttonRect.Visible = true;
            _buttonGlowRect.Visible = true;
            SlideButtonIn();
            StartGlowPulse();
        }));
    }

    // ─── 按钮动画 ───

    private void SlideButtonIn()
    {
        var tween = _root.CreateTween();
        tween.TweenMethod(
          Callable.From<float>(y => SetButtonRect(y)),
          ButtonStartY + WheelBaseY, ButtonFinalY + WheelBaseY, 0.6f
        ).SetTrans(Tween.TransitionType.Back)
         .SetEase(Tween.EaseType.Out);
    }

    private void SlideButtonOut()
    {
        var tween = _root.CreateTween();
        tween.TweenMethod(
          Callable.From<float>(y => SetButtonRect(y)),
          ButtonFinalY + WheelBaseY, ButtonStartY + WheelBaseY, 0.4f
        ).SetTrans(Tween.TransitionType.Back)
         .SetEase(Tween.EaseType.In);

        tween.TweenCallback(Callable.From(() =>
        {
            _buttonRect.Visible = false;
            _buttonGlowRect.Visible = false;
        }));
    }

    // ─── 按钮发光 ───

    private void StartGlowPulse()
    {
        _glowTween?.Kill();
        _glowTween = _root.CreateTween();
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
        if (_buttonRect == null || _spinning)
            return;
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

        BalanceModAudio.PlayOneShot(SpinSfxPath);

        float resultAngle = _minigame.ResultAngle;
        float spinEnd = SpinVelocity * SpinDuration;

        _mainTween?.Kill();
        _mainTween = _root.CreateTween();

        _mainTween.TweenMethod(
          Callable.From<float>(angle => _wheelRect.RotationDegrees = -angle + WheelAngleOffset),
          0f, spinEnd, SpinDuration
        ).SetTrans(Tween.TransitionType.Linear);

        _mainTween.TweenMethod(
          Callable.From<float>(t =>
            _wheelRect.RotationDegrees = -ElasticLerp(resultAngle, -180f, t) + WheelAngleOffset),
          1.0f, 0.0f, DecelerateDuration
        ).SetTrans(Tween.TransitionType.Linear);

        _mainTween.TweenCallback(Callable.From(() =>
        {
            _wheelRect.RotationDegrees = -resultAngle + WheelAngleOffset;
        }));

        _mainTween.TweenInterval(PauseDuration);
        _mainTween.TweenCallback(Callable.From(StartBounceOut));
    }

    // ─── 弹出动画 → 清理 ───

    private void StartBounceOut()
    {
        _mainTween?.Kill();
        _mainTween = _root.CreateTween();
        _mainTween.SetParallel(true);

        _mainTween.TweenProperty(_root, "modulate",
          new Color(1f, 1f, 1f, 0f), BounceOutDuration)
          .SetTrans(Tween.TransitionType.Cubic)
          .SetEase(Tween.EaseType.In);

        _mainTween.TweenMethod(
          Callable.From<float>(offset =>
          {
              float halfWheel = WheelDisplaySize / 2f;
              float y = WheelBaseY + offset;
              _wheelRect.OffsetTop = -halfWheel + y;
              _wheelRect.OffsetBottom = halfWheel + y;
              _arrowRect.OffsetTop = -ArrowDisplaySize / 2f + y;
              _arrowRect.OffsetBottom = ArrowDisplaySize / 2f + y;
          }),
          0f, WheelStartOffset, BounceOutDuration
        ).SetTrans(Tween.TransitionType.Back)
         .SetEase(Tween.EaseType.In);

        _mainTween.SetParallel(false);
        _mainTween.TweenCallback(Callable.From(() =>
        {
            // 动画结束后清理
            _minigame.Complete();
        }));
    }

    private void OnMinigameFinished()
    {
        DisposeInstance();
    }

    // ─── Elastic Interpolation ───

    private static float ElasticIn(float a)
    {
        if (a >= 0.99f)
            return 1f;
        if (a <= 0f)
            return 0f;
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
}
