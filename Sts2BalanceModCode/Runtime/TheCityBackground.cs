using Godot;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Sts2BalanceMod.Sts2BalanceModCode.Runtime;

/// <summary>
/// 移植自 ActsFromThePast 的 1 代城市地下战斗场景（The City）。
/// 用于竞技场（Colosseum）等战斗遭遇，提供完整的地面、背景柱子、阴暗氛围与对齐偏移。
/// </summary>
public partial class TheCityBackground : NCombatBackground
{
    private const string AtlasPath = "res://Sts2BalanceMod/backgrounds/city/scene.atlas";

    // Background layers
    private TextureRect _bg = null!;
    private TextureRect _bgGlow = null!;
    private TextureRect _bgGlow2 = null!;
    private TextureRect _bg2 = null!;
    private TextureRect _bg2Glow = null!;
    private TextureRect _bgGlow2Double = null!;
    private TextureRect _floor = null!;
    private TextureRect _ceiling = null!;
    private TextureRect _wall = null!;
    private TextureRect _chains = null!;
    private TextureRect _chainsGlow = null!;
    private TextureRect _chainsGlow2 = null!;
    private TextureRect _mg = null!;
    private TextureRect _mgGlow = null!;
    private TextureRect _mgGlow2 = null!;
    private TextureRect _mgAlt = null!;
    private TextureRect _fg = null!;
    private TextureRect _fgGlow = null!;
    private TextureRect _fg2 = null!;
    private TextureRect _throne = null!;
    private TextureRect _throneGlow = null!;

    // Pillars
    private TextureRect _pillar1 = null!;
    private TextureRect _pillar2 = null!;
    private TextureRect _pillar3 = null!;
    private TextureRect _pillar4 = null!;
    private TextureRect _pillar5 = null!;

    // Render flags
    private bool _renderAltBg;
    private bool _renderMg;
    private bool _renderMgGlow;
    private bool _renderMgAlt;
    private bool _renderWall;
    private bool _renderChains;
    private bool _renderThrone;
    private bool _renderFg2;
    private bool _darkDay;
    private PillarConfig _pillarConfig = PillarConfig.Open;

    private readonly Color _overlayColor = Colors.White;
    private bool _initialized;

    private enum PillarConfig
    {
        Open,
        SidesOnly,
        Full,
        Left1,
        Left2
    }

    public override void _Ready()
    {
        base._Ready();
        Initialize();
    }

    public void Initialize()
    {
        if (_initialized) return;
        _initialized = true;

        MouseFilter = MouseFilterEnum.Ignore;
        ZIndex = -100;

        // Create layers in order (back to front)
        _bg = CreateTextureRect("mod/bg1", -50);
        _bgGlow = CreateTextureRect("mod/bgGlowv2", -49);
        _bgGlow2 = CreateTextureRect("mod/bgGlowBlur", -48);
        _bg2 = CreateTextureRect("mod/bg2", -46);
        _bg2Glow = CreateTextureRect("mod/bg2Glow", -45);
        _bgGlow2Double = CreateTextureRect("mod/bgGlowBlur", -47);
        _floor = CreateTextureRect("mod/floor", -45);
        _ceiling = CreateTextureRect("mod/ceiling", -44);
        _wall = CreateTextureRect("mod/wall", -43);
        _chains = CreateTextureRect("mod/chains", -42);
        _chainsGlow = CreateTextureRect("mod/chainsGlow", -41);
        _chainsGlow2 = CreateTextureRect("mod/chainsGlow", -40);
        _mg = CreateTextureRect("mod/mg1", -39);
        _mgGlow = CreateTextureRect("mod/mg1Glow", -38);
        _mgGlow2 = CreateTextureRect("mod/mg1Glow", -37);
        _mgAlt = CreateTextureRect("mod/mg2", -36);
        _pillar1 = CreateTextureRect("mod/p1", -35);
        _pillar2 = CreateTextureRect("mod/p2", -34);
        _pillar3 = CreateTextureRect("mod/p3", -33);
        _pillar4 = CreateTextureRect("mod/p4", -32);
        _pillar5 = CreateTextureRect("mod/p5", -31);
        _throne = CreateTextureRect("mod/throne", -30);
        _throneGlow = CreateTextureRect("mod/throneGlow", -29);
        _fg = CreateTextureRect("mod/fg", -20);
        _fgGlow = CreateTextureRect("mod/fgGlow", -19);
        _fg2 = CreateTextureRect("mod/fgHideWindow", -18);

        // Set up additive blend for glow layers
        SetAdditiveBlend(_bgGlow);
        SetAdditiveBlend(_bgGlow2);
        SetAdditiveBlend(_bgGlow2Double);
        SetAdditiveBlend(_bg2Glow);
        SetAdditiveBlend(_chainsGlow);
        SetAdditiveBlend(_chainsGlow2);
        SetAdditiveBlend(_mgGlow);
        SetAdditiveBlend(_mgGlow2);
        SetAdditiveBlend(_throneGlow);
        SetAdditiveBlend(_fgGlow);

        RandomizeScene();
    }

    private static void SetAdditiveBlend(TextureRect rect)
    {
        var material = new CanvasItemMaterial();
        material.BlendMode = CanvasItemMaterial.BlendModeEnum.Add;
        rect.Material = material;
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        GetTree().ProcessFrame += OnProcessFrame;
    }

    public override void _ExitTree()
    {
        GetTree().ProcessFrame -= OnProcessFrame;
        base._ExitTree();
    }

    private void OnProcessFrame()
    {
        if (!_initialized || !IsInsideTree()) return;
        UpdateGlowAnimations();
    }

    private TextureRect CreateTextureRect(string regionName, int zIndex)
    {
        var rect = new TextureRect();
        rect.MouseFilter = MouseFilterEnum.Ignore;
        rect.ZIndex = zIndex;

        var regionInfo = LibGdxAtlas.GetRegionData(AtlasPath, regionName);
        var region = LibGdxAtlas.GetRegion(AtlasPath, regionName);

        if (region != null && regionInfo != null)
        {
            var atlasTexture = new AtlasTexture();
            atlasTexture.Atlas = region.Value.Texture;
            atlasTexture.Region = region.Value.Region;

            rect.Texture = atlasTexture;
            rect.StretchMode = TextureRect.StretchModeEnum.Keep;

            float offsetX = regionInfo.Value.OffsetX - (regionInfo.Value.OrigWidth / 2f) - 23f;
            float offsetY = regionInfo.Value.OrigHeight - regionInfo.Value.OffsetY - regionInfo.Value.Height -
                            (regionInfo.Value.OrigHeight / 2f);

            rect.Position = new Vector2(offsetX, offsetY);
            rect.Size = new Vector2(regionInfo.Value.Width, regionInfo.Value.Height);
        }

        AddChild(rect);
        return rect;
    }

    private void RandomizeScene()
    {
        _darkDay = GD.Randf() > 0.6f;
        _renderAltBg = GD.Randf() > 0.5f;
        _renderMg = true;

        if (_renderMg)
        {
            _renderMgAlt = GD.Randf() > 0.5f;
            if (!_renderMgAlt)
                _renderMgGlow = GD.Randf() > 0.5f;
        }

        _renderWall = GD.Randi() % 5 == 4;
        _renderChains = _renderWall && GD.Randf() > 0.5f;
        _renderFg2 = GD.Randf() > 0.5f;

        if (_renderWall)
        {
            int roll = (int)(GD.Randi() % 3);
            _pillarConfig = roll switch
            {
                0 => PillarConfig.Open,
                1 => PillarConfig.Left1,
                _ => PillarConfig.Left2
            };
        }
        else
        {
            int roll = (int)(GD.Randi() % 3);
            _pillarConfig = roll switch
            {
                0 => PillarConfig.Open,
                1 => PillarConfig.SidesOnly,
                _ => PillarConfig.Full
            };
        }

        _renderThrone = false;
        UpdateVisibility();
    }

    private void UpdateVisibility()
    {
        _bg.Modulate = _overlayColor;
        _floor.Modulate = _overlayColor;
        _ceiling.Modulate = _overlayColor;
        _wall.Modulate = _overlayColor;
        _chains.Modulate = _overlayColor;
        _mg.Modulate = _overlayColor;

        _bg2.Visible = _renderAltBg;
        _bg2Glow.Visible = _renderAltBg;
        _bgGlow2.Visible = _darkDay;
        _bgGlow2Double.Visible = _darkDay;
        _bgGlow2Double.Modulate = new Color(1, 1, 1, 0.7f);
        _wall.Visible = _renderWall;
        _chains.Visible = _renderChains;
        _chainsGlow.Visible = _renderChains;
        _chainsGlow2.Visible = _renderChains;
        _mg.Visible = _renderMg;
        _mgGlow.Visible = _renderMg;
        _mgGlow2.Visible = _renderMg && _renderMgGlow;
        _mgAlt.Visible = _renderMgAlt;
        _mgAlt.Modulate = _renderMgGlow ? new Color(1f, 1f, 0.9f, 1f) : Colors.White;
        _fg2.Visible = _renderFg2;
        _throne.Visible = _renderThrone;
        _throneGlow.Visible = _renderThrone;

        // Pillars
        _pillar1.Visible = _pillarConfig is PillarConfig.SidesOnly or PillarConfig.Full or PillarConfig.Left1 or PillarConfig.Left2;
        _pillar2.Visible = _pillarConfig is PillarConfig.Full or PillarConfig.Left2;
        _pillar3.Visible = _pillarConfig == PillarConfig.Full;
        _pillar4.Visible = _pillarConfig == PillarConfig.Full;
        _pillar5.Visible = _pillarConfig is PillarConfig.SidesOnly or PillarConfig.Full;
    }

    private void UpdateGlowAnimations()
    {
        if (_renderChains)
        {
            float chainsDegrees = (float)(Time.GetTicksMsec() % 360);
            float chainsAlpha = Mathf.Cos(Mathf.DegToRad(chainsDegrees)) / 10f + 0.9f;
            var chainsColor = new Color(1, 1, 1, chainsAlpha);
            _chainsGlow.Modulate = chainsColor;
            _chainsGlow2.Modulate = chainsColor;
        }

        if (_renderMg)
        {
            if (_renderMgGlow)
            {
                float mgDegrees = (float)(Time.GetTicksMsec() / 10 % 360);
                float mgAlpha = Mathf.Cos(Mathf.DegToRad(mgDegrees)) / 2f + 0.5f;
                var mgColor = new Color(1, 1, 0.9f, mgAlpha);
                _mgGlow.Modulate = mgColor;
                _mgGlow2.Modulate = mgColor;
            }
            else
            {
                _mgGlow.Modulate = Colors.White;
            }
        }
    }

    public void OnTreeEntered()
    {
        TreeEntered -= OnTreeEntered;
        Initialize();

        // 向上查找到 NCombatRoom 节点并调整前景与角色容器位置
        Node? parent = GetParent();
        while (parent != null && parent is not NCombatRoom)
            parent = parent.GetParent();

        if (parent is NCombatRoom combatRoom)
        {
            var sceneContainer = combatRoom.GetNodeOrNull<Control>("%CombatSceneContainer")
                              ?? combatRoom.GetNodeOrNull<Control>("%SceneContainer");
            if (sceneContainer != null)
            {
                ReparentToContainer(_fg, sceneContainer, -3);
                ReparentToContainer(_fgGlow, sceneContainer, -3);
                ReparentToContainer(_fg2, sceneContainer, -3);
            }

            var allyContainer = combatRoom.GetNodeOrNull<Control>("%AllyContainer");
            var enemyContainer = combatRoom.GetNodeOrNull<Control>("%EnemyContainer");
            if (allyContainer != null)
                allyContainer.Position += Vector2.Down * 30f;
            if (enemyContainer != null)
                enemyContainer.Position += Vector2.Down * 30f;
        }
    }

    private static void ReparentToContainer(TextureRect layer, Control container, int zIndex)
    {
        if (!GodotObject.IsInstanceValid(layer) || layer.GetParent() == null) return;
        var globalPos = layer.GlobalPosition;
        layer.GetParent().RemoveChild(layer);
        container.AddChild(layer);
        layer.GlobalPosition = globalPos;
        layer.ZIndex = zIndex;
    }
}
