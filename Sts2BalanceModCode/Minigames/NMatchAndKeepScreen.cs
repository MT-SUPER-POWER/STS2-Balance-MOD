using Godot;
using HarmonyLib;
using MegaCrit.Sts2.addons.mega_text;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Multiplayer;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.UI;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using MegaCrit.Sts2.Core.Nodes.HoverTips;
using MegaCrit.Sts2.Core.Nodes.Screens.Overlays;
using MegaCrit.Sts2.Core.Nodes.Screens.ScreenContext;
using Sts2BalanceMod.Sts2BalanceModCode.Utils;

namespace Sts2BalanceMod.Sts2BalanceModCode.Minigames;

/// <summary>
/// 对对碰全屏小游戏界面。
/// 展示 4x3 共 12 张暗牌，支持鼠标与手柄十字键导航、翻牌动画、配对吸附与结算。
/// </summary>
public partial class NMatchAndKeepScreen : Control, IOverlayScreen, IScreenContext
{
  private const float GridScale = 0.5f;
  private const float SelectedScale = 0.6f;
  private const float MismatchScale = 0.75f;
  private const float ScaleTweenTime = 0.2f;

  private static readonly float[] ColOffsets = [-320f, -110f, 100f, 310f];
  private static readonly float[] RowOffsets = [-210f, 20f, 250f];

  private const float MatchWait = 1.0f;
  private const float MismatchWait = 1.25f;
  private const float GameDoneWait = 1.0f;
  private const float CleanupWait = 1.0f;
  private const float CardSlideTime = 0.4f;
  private const float FadeInDuration = 0.5f;
  private const float FadeOutDuration = 0.5f;

  private const string CardBackAtlasPath = "res://Sts2BalanceMod/images/event_extras/cardui.atlas";
  private const string CardBackRegionName = "512/card_back";
  private const string BackgroundImagePath = "res://Sts2BalanceMod/images/events/MatchAndKeep.png";

  private static NMatchAndKeepScreen? _instance;
  private MatchAndKeepMinigame _minigame = null!;

  private Control _gridContainer = null!;
  private MegaRichTextLabel _attemptsLabel = null!;
  private readonly List<CardSlot> _slots = [];

  private int _firstSelection = -1;
  private int _attemptsLeft;
  private int _matchCount;
  private bool _isProcessing;

  private Tween? _waitTween;
  private AtlasTexture? _cardBackTexture;

  public static NMatchAndKeepScreen ShowScreen(MatchAndKeepMinigame minigame)
  {
    if (_instance != null && GodotObject.IsInstanceValid(_instance))
    {
      _instance.QueueFree();
    }

    NMatchAndKeepScreen screen = new()
    {
      _minigame = minigame,
      _attemptsLeft = minigame.MaxAttempts
    };
    screen.LoadCardBack();
    screen.BuildUI();
    _instance = screen;
    NOverlayStack.Instance.Push((IOverlayScreen)screen);
    screen.SetupFocusNeighbors();
    screen.DealCards();

    return screen;
  }

  public override void _ExitTree()
  {
    KillAllTweens();
    _minigame.ForceEnd();
    _instance = null;
  }

  private void KillAllTweens()
  {
    _waitTween?.Kill();
  }

  public NetScreenType ScreenType => NetScreenType.None;
  public bool UseSharedBackstop => false;

  public Control DefaultFocusedControl
  {
    get
    {
      foreach (CardSlot slot in _slots)
      {
        if (!slot.IsMatched)
          return slot.Holder;
      }
      return this;
    }
  }

  public void AfterOverlayOpened() { }

  public void AfterOverlayClosed()
  {
    KillAllTweens();
    this.QueueFreeSafely();
  }

  public void AfterOverlayShown() { }
  public void AfterOverlayHidden() { }

  private void LoadCardBack()
  {
    LibGdxAtlas.TextureRegion? region = LibGdxAtlas.GetRegion(CardBackAtlasPath, CardBackRegionName);
    if (region != null)
    {
      _cardBackTexture = new AtlasTexture
      {
        Atlas = region.Value.Texture,
        Region = region.Value.Region
      };
    }
  }

  private void BuildUI()
  {
    SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);

    Texture2D? bgTex = GD.Load<Texture2D>(BackgroundImagePath);
    if (bgTex != null)
    {
      TextureRect bgRect = new()
      {
        Texture = bgTex,
        ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
        StretchMode = TextureRect.StretchModeEnum.KeepAspectCovered,
        MouseFilter = MouseFilterEnum.Ignore
      };
      bgRect.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
      AddChild(bgRect);
    }

    _gridContainer = new Control { MouseFilter = MouseFilterEnum.Ignore };
    _gridContainer.SetAnchorsAndOffsetsPreset(LayoutPreset.FullRect);
    AddChild(_gridContainer);

    for (int i = 0; i < 12; i++)
    {
      CreateCardSlot(i);
    }

    FontVariation? font = GD.Load<FontVariation>("res://themes/kreon_bold_glyph_space_one.tres");
    _attemptsLabel = new MegaRichTextLabel
    {
      BbcodeEnabled = true,
      FitContent = true,
      ScrollActive = false,
      MouseFilter = MouseFilterEnum.Ignore,
      AutowrapMode = TextServer.AutowrapMode.Off,
      AnchorLeft = 0.5f,
      AnchorTop = 1f,
      AnchorRight = 0.5f,
      AnchorBottom = 1f,
      OffsetLeft = -250,
      OffsetTop = -80,
      OffsetRight = 250,
      OffsetBottom = -30,
      GrowHorizontal = GrowDirection.Both,
      GrowVertical = GrowDirection.Begin
    };
    _attemptsLabel.AddThemeColorOverride("default_color", Colors.White);
    _attemptsLabel.AddThemeFontSizeOverride("normal_font_size", 30);
    if (font != null)
    {
      _attemptsLabel.AddThemeFontOverride("normal_font", font);
    }
    _attemptsLabel.AddThemeConstantOverride("outline_size", 10);
    _attemptsLabel.AddThemeColorOverride("font_outline_color", new Color(0.15f, 0.1f, 0.23f, 1f));
    AddChild(_attemptsLabel);
    RefreshAttemptsLabel();

    Modulate = new Color(1f, 1f, 1f, 0f);
  }

  private void CreateCardSlot(int index)
  {
    int col = index % 4;
    int row = index % 3;

    Control wrapper = new()
    {
      MouseFilter = MouseFilterEnum.Ignore,
      AnchorLeft = 0.5f,
      AnchorTop = 0.5f,
      AnchorRight = 0.5f,
      AnchorBottom = 0.5f,
      OffsetLeft = 0f,
      OffsetTop = 0f,
      OffsetRight = 0f,
      OffsetBottom = 0f,
      GrowHorizontal = GrowDirection.Both,
      GrowVertical = GrowDirection.Both,
      Scale = new Vector2(GridScale, GridScale)
    };
    _gridContainer.AddChild(wrapper);

    CardModel cardModel = _minigame.Cards[index];
    NCard ncard = NCard.Create(cardModel, ModelVisibility.Visible);
    NGridCardHolder holder = NGridCardHolder.Create(ncard);
    holder.Position = Vector2.Zero;
    wrapper.AddChild(holder);

    ncard.Visible = false;
    Callable.From(() =>
    {
      ncard.UpdateVisuals(PileType.None, CardPreviewMode.Normal);
    }).CallDeferred();

    TextureRect? overlay = null;
    if (_cardBackTexture != null)
    {
      overlay = new TextureRect
      {
        Texture = _cardBackTexture,
        ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
        StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
        CustomMinimumSize = new Vector2(300, 422),
        Position = new Vector2(-150, -211),
        Size = new Vector2(300, 422),
        MouseFilter = MouseFilterEnum.Ignore,
        Visible = true
      };
      holder.AddChild(overlay);
    }

    int idx = index;
    holder.Pressed += _ => OnCardClicked(idx);

    holder.Connect(Control.SignalName.MouseEntered, Callable.From(() =>
    {
      if (idx < _slots.Count && !_slots[idx].IsFaceUp)
        Callable.From(() => NHoverTipSet.Remove(holder)).CallDeferred();
    }));
    holder.Connect(Control.SignalName.FocusEntered, Callable.From(() =>
    {
      if (idx < _slots.Count && !_slots[idx].IsFaceUp)
        Callable.From(() => NHoverTipSet.Remove(holder)).CallDeferred();
    }));

    wrapper.OffsetTop = 800f;
    wrapper.OffsetBottom = 800f;

    _slots.Add(new CardSlot
    {
      Wrapper = wrapper,
      Holder = holder,
      CardNode = ncard,
      Overlay = overlay,
      PairIndex = _minigame.PairIndices[index],
      IsFaceUp = false,
      IsMatched = false,
      Col = col,
      Row = row
    });
  }

  private void SetupFocusNeighbors()
  {
    int[,] gridMap = new int[4, 3];
    for (int c = 0; c < 4; c++)
    {
      for (int r = 0; r < 3; r++)
      {
        gridMap[c, r] = -1;
      }
    }

    for (int i = 0; i < _slots.Count; i++)
    {
      if (_slots[i].IsMatched)
        continue;
      gridMap[_slots[i].Col, _slots[i].Row] = i;
    }

    for (int i = 0; i < _slots.Count; i++)
    {
      CardSlot slot = _slots[i];
      if (slot.IsMatched)
        continue;

      NGridCardHolder holder = slot.Holder;
      int col = slot.Col;
      int row = slot.Row;

      holder.FocusNeighborLeft = FindNeighbor(gridMap, col, row, -1, 0)?.GetPath() ?? holder.GetPath();
      holder.FocusNeighborRight = FindNeighbor(gridMap, col, row, 1, 0)?.GetPath() ?? holder.GetPath();
      holder.FocusNeighborTop = FindNeighbor(gridMap, col, row, 0, -1)?.GetPath() ?? holder.GetPath();
      holder.FocusNeighborBottom = FindNeighbor(gridMap, col, row, 0, 1)?.GetPath() ?? holder.GetPath();
    }
  }

  private NGridCardHolder? FindNeighbor(int[,] gridMap, int col, int row, int dCol, int dRow)
  {
    int c = col + dCol;
    int r = row + dRow;

    for (int attempt = 0; attempt < 4; attempt++)
    {
      if (c < 0) c = 3;
      if (c > 3) c = 0;
      if (r < 0) r = 2;
      if (r > 2) r = 0;

      int idx = gridMap[c, r];
      if (idx >= 0)
        return _slots[idx].Holder;

      c += dCol;
      r += dRow;
    }
    return null;
  }

  private void DealCards()
  {
    Tween tween = CreateTween();
    tween.SetParallel(true);

    tween.TweenProperty(
      (GodotObject)this, (NodePath)"modulate:a",
      (Variant)1.0f, FadeInDuration
    ).From((Variant)0.0f)
     .SetTrans(Tween.TransitionType.Cubic)
     .SetEase(Tween.EaseType.Out);

    for (int i = 0; i < _slots.Count; i++)
    {
      CardSlot slot = _slots[i];
      float targetX = ColOffsets[slot.Col];
      float targetY = RowOffsets[slot.Row];

      tween.TweenProperty((GodotObject)slot.Wrapper, (NodePath)"offset_left", (Variant)targetX, CardSlideTime)
        .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
      tween.TweenProperty((GodotObject)slot.Wrapper, (NodePath)"offset_right", (Variant)targetX, CardSlideTime)
        .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
      tween.TweenProperty((GodotObject)slot.Wrapper, (NodePath)"offset_top", (Variant)targetY, CardSlideTime)
        .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
      tween.TweenProperty((GodotObject)slot.Wrapper, (NodePath)"offset_bottom", (Variant)targetY, CardSlideTime)
        .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
    }
  }

  private void OnCardClicked(int index)
  {
    if (_isProcessing)
      return;

    CardSlot slot = _slots[index];
    if (slot.IsFaceUp || slot.IsMatched)
      return;

    slot.IsFaceUp = true;
    slot.CardNode.Visible = true;
    if (slot.Overlay != null)
    {
      slot.Overlay.Visible = false;
    }

    Tween scaleTween = slot.Wrapper.CreateTween();
    scaleTween.TweenProperty(
      (GodotObject)slot.Wrapper, (NodePath)"scale",
      (Variant)new Vector2(SelectedScale, SelectedScale), ScaleTweenTime
    ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);

    scaleTween.TweenCallback(Callable.From(() =>
    {
      Traverse traverse = Traverse.Create(slot.Holder);
      traverse.Field("_isFocused").SetValue(false);
      traverse.Method("RefreshFocusState").GetValue();
    }));

    if (_firstSelection < 0)
    {
      _firstSelection = index;
    }
    else
    {
      int first = _firstSelection;
      _firstSelection = -1;
      _isProcessing = true;

      bool isMatch = _minigame.Cards[first].Id == _minigame.Cards[index].Id;
      if (isMatch)
      {
        HandleMatch(first, index);
      }
      else
      {
        HandleMismatch(first, index);
      }
    }
  }

  private void HandleMatch(int a, int b)
  {
    Tween tween = CreateTween();
    tween.SetParallel(true);

    foreach (int idx in new[] { a, b })
    {
      tween.TweenProperty((GodotObject)_slots[idx].Wrapper, (NodePath)"offset_left", (Variant)0f, CardSlideTime)
        .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
      tween.TweenProperty((GodotObject)_slots[idx].Wrapper, (NodePath)"offset_right", (Variant)0f, CardSlideTime)
        .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
      tween.TweenProperty((GodotObject)_slots[idx].Wrapper, (NodePath)"offset_top", (Variant)0f, CardSlideTime)
        .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
      tween.TweenProperty((GodotObject)_slots[idx].Wrapper, (NodePath)"offset_bottom", (Variant)0f, CardSlideTime)
        .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.InOut);
    }

    tween.SetParallel(false);
    tween.TweenInterval(MatchWait);

    tween.TweenCallback(Callable.From(() =>
    {
      _slots[a].IsMatched = true;
      _slots[b].IsMatched = true;
      _slots[a].Wrapper.Visible = false;
      _slots[b].Wrapper.Visible = false;

      CardModel canonical = _minigame.Canonicals[_slots[a].PairIndex];
      Player player = _minigame.Owner;
      async Task AddMatchedCard()
      {
        CardModel cardInstance = player.RunState.CreateCard(canonical, player);
        CardPileAddResult result = await CardPileCmd.Add(cardInstance, PileType.Deck);
        CardCmd.PreviewCardPileAdd([result]);
      }
      TaskHelper.RunSafely(AddMatchedCard());

      _matchCount++;
      _attemptsLeft--;
      RefreshAttemptsLabel();
      SetupFocusNeighbors();
      _isProcessing = false;

      CheckGameEnd();
    }));
  }

  private void HandleMismatch(int a, int b)
  {
    SetSlotScale(a, MismatchScale);
    SetSlotScale(b, MismatchScale);

    _waitTween?.Kill();
    _waitTween = CreateTween();
    _waitTween.TweenInterval(MismatchWait);
    _waitTween.TweenCallback(Callable.From(() =>
    {
      _slots[a].IsFaceUp = false;
      _slots[b].IsFaceUp = false;
      _slots[a].CardNode.Visible = false;
      _slots[b].CardNode.Visible = false;
      if (_slots[a].Overlay != null) _slots[a].Overlay.Visible = true;
      if (_slots[b].Overlay != null) _slots[b].Overlay.Visible = true;

      SetSlotScale(a, GridScale);
      SetSlotScale(b, GridScale);

      _attemptsLeft--;
      RefreshAttemptsLabel();
      _isProcessing = false;

      CheckGameEnd();
    }));
  }

  private void SetSlotScale(int index, float scale)
  {
    Control wrapper = _slots[index].Wrapper;
    Tween tween = wrapper.CreateTween();
    tween.TweenProperty(
      (GodotObject)wrapper, (NodePath)"scale",
      (Variant)new Vector2(scale, scale), ScaleTweenTime
    ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.Out);
  }

  private void CheckGameEnd()
  {
    bool allMatched = _matchCount >= 6;
    bool outOfAttempts = _attemptsLeft <= 0;

    if (!allMatched && !outOfAttempts)
      return;

    _isProcessing = true;

    _waitTween?.Kill();
    _waitTween = CreateTween();

    if (outOfAttempts && !allMatched)
    {
      _waitTween.TweenInterval(GameDoneWait);
    }

    _waitTween.TweenCallback(Callable.From(() =>
    {
      Tween cleanupTween = CreateTween();
      cleanupTween.SetParallel(true);

      for (int i = 0; i < _slots.Count; i++)
      {
        CardSlot slot = _slots[i];
        if (slot.IsMatched)
          continue;

        SetSlotScale(i, GridScale);

        cleanupTween.TweenProperty((GodotObject)slot.Wrapper, (NodePath)"offset_left", (Variant)0f, CardSlideTime)
          .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);
        cleanupTween.TweenProperty((GodotObject)slot.Wrapper, (NodePath)"offset_right", (Variant)0f, CardSlideTime)
          .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);
        cleanupTween.TweenProperty((GodotObject)slot.Wrapper, (NodePath)"offset_top", (Variant)800f, CardSlideTime)
          .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);
        cleanupTween.TweenProperty((GodotObject)slot.Wrapper, (NodePath)"offset_bottom", (Variant)800f, CardSlideTime)
          .SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);
      }

      cleanupTween.SetParallel(false);
      cleanupTween.TweenInterval(CleanupWait);

      cleanupTween.TweenProperty(
        (GodotObject)this, (NodePath)"modulate:a",
        (Variant)0.0f, FadeOutDuration
      ).SetTrans(Tween.TransitionType.Cubic).SetEase(Tween.EaseType.In);

      cleanupTween.TweenCallback(Callable.From(() =>
      {
        _minigame.Complete();
        NOverlayStack.Instance.Remove((IOverlayScreen)this);
      }));
    }));
  }

  private void RefreshAttemptsLabel()
  {
    LocString loc = new("events", "STS2_BALANCE_MOD_EVENT_MATCH_AND_KEEP.minigame.attempts");
    loc.Add("Count", (decimal)_attemptsLeft);
    _attemptsLabel.Text = $"[center]{loc.GetFormattedText()}[/center]";
  }

  private sealed class CardSlot
  {
    public Control Wrapper { get; init; } = null!;
    public NGridCardHolder Holder { get; init; } = null!;
    public NCard CardNode { get; init; } = null!;
    public TextureRect? Overlay { get; init; }
    public int PairIndex { get; init; }
    public int Col { get; init; }
    public int Row { get; init; }
    public bool IsFaceUp { get; set; }
    public bool IsMatched { get; set; }
  }
}
