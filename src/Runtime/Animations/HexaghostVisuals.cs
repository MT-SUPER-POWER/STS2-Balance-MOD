using Godot;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Combat;

namespace Sts2BalanceMod.src.Runtime.Animations;

/// <summary>
/// Hexaghost 核心、三层等离子、阴影与六枚火球的组合视觉控制器。
/// </summary>
public sealed class HexaghostVisuals : IDisposable
{
  private const float BobSpeed = 0.75f;
  private const float BobAmount = 5f;
  private const float BodyOffsetY = -225f;
  private static readonly string ResourceRoot = ModAssetPaths.Resource("monsters", "hexaghost");

  private static readonly Vector2[] OrbPositions =
  [
    new(-90f, -370f),
    new(90f, -370f),
    new(160f, -240f),
    new(90f, -110f),
    new(-90f, -110f),
    new(-160f, -240f),
  ];

  private readonly Creature _creature;
  private readonly NCreature _creatureNode;
  private readonly HexaghostOrbVisual[] _orbs = new HexaghostOrbVisual[6];

  private Sprite2D? _plasma1;
  private Sprite2D? _plasma2;
  private Sprite2D? _plasma3;
  private Sprite2D? _shadow;
  private float _rotationSpeed = 1f;
  private float _targetRotationSpeed = 30f;
  private float _plasma1Angle;
  private float _plasma2Angle;
  private float _plasma3Angle;
  private float _bobTimer;
  private bool _disposed;

  public HexaghostVisuals(Creature creature, NCreature creatureNode)
  {
    _creature = creature;
    _creatureNode = creatureNode;

    CreatePlasmaLayers();
    CreateOrbs();
    TaskHelper.RunSafely(UpdateLoop());
  }

  private void CreatePlasmaLayers()
  {
    var visualsNode = _creatureNode.Visuals;

    _plasma3 = CreateLayer($"{ResourceRoot}/plasma3.png", -3);
    visualsNode.AddChild(_plasma3);

    _plasma2 = CreateLayer($"{ResourceRoot}/plasma2.png", -2);
    visualsNode.AddChild(_plasma2);

    _plasma1 = CreateLayer($"{ResourceRoot}/plasma1.png", -1);
    visualsNode.AddChild(_plasma1);

    _shadow = CreateLayer($"{ResourceRoot}/shadow.png", -4);
    visualsNode.AddChild(_shadow);
  }

  private static Sprite2D CreateLayer(string path, int zIndex)
  {
    return new Sprite2D
    {
      Texture = PreloadManager.Cache.GetTexture2D(path),
      ZIndex = zIndex,
    };
  }

  private void CreateOrbs()
  {
    for (var i = 0; i < _orbs.Length; i++)
    {
      var orb = new HexaghostOrbVisual(i, OrbPositions[i]);
      orb.SetParentNode(_creatureNode.Visuals);
      _orbs[i] = orb;
    }
  }

  private async Task UpdateLoop()
  {
    while (!_disposed && GodotObject.IsInstanceValid(_creatureNode) && _creature.IsAlive)
    {
      Update((float)_creatureNode.GetProcessDeltaTime());
      await _creatureNode.ToSignal(_creatureNode.GetTree(), SceneTree.SignalName.ProcessFrame);
    }
  }

  private void Update(float delta)
  {
    if (_plasma1 == null || _plasma2 == null || _plasma3 == null || _shadow == null)
      return;

    _rotationSpeed = Mathf.Lerp(_rotationSpeed, _targetRotationSpeed, delta * 5f);
    _plasma1Angle -= _rotationSpeed * delta;
    _plasma2Angle -= _rotationSpeed / 2f * delta;
    _plasma3Angle -= _rotationSpeed / 3f * delta;

    _bobTimer += BobSpeed * delta;
    var bobOffset = Mathf.Sin(_bobTimer) * BobAmount;

    _plasma1.Rotation = Mathf.DegToRad(_plasma1Angle);
    _plasma1.Position = new Vector2(0f, -bobOffset * 0.5f + BodyOffsetY);

    _plasma2.Rotation = Mathf.DegToRad(_plasma2Angle);
    _plasma2.Position = new Vector2(6f, -bobOffset + BodyOffsetY);

    _plasma3.Rotation = Mathf.DegToRad(_plasma3Angle);
    _plasma3.Scale = Vector2.One * 0.95f;
    _plasma3.Position = new Vector2(12f, -bobOffset * 2f + BodyOffsetY);

    _shadow.Position = new Vector2(12f, -bobOffset / 4f - 15f + BodyOffsetY);

    var parentGlobalPosition = _creatureNode.Visuals.GlobalPosition;
    foreach (var orb in _orbs)
      orb.Update(delta, parentGlobalPosition);
  }

  public void SetTargetRotationSpeed(float speed)
  {
    _targetRotationSpeed = speed;
  }

  public void ActivateAllOrbs()
  {
    foreach (var orb in _orbs)
      orb.Activate();
  }

  public void ActivateNextOrb()
  {
    foreach (var orb in _orbs)
    {
      if (orb.IsActivated)
        continue;

      orb.Activate(immediate: true);
      return;
    }
  }

  public void DeactivateAllOrbs()
  {
    foreach (var orb in _orbs)
      orb.Deactivate();
  }

  public void HideAllOrbs()
  {
    foreach (var orb in _orbs)
      orb.Hide();
  }

  public void Dispose()
  {
    if (_disposed)
      return;

    _disposed = true;
    SafeFree(_plasma1);
    SafeFree(_plasma2);
    SafeFree(_plasma3);
    SafeFree(_shadow);

    foreach (var orb in _orbs)
      orb?.Dispose();
  }

  private static void SafeFree(Node? node)
  {
    if (node != null && GodotObject.IsInstanceValid(node))
      node.QueueFree();
  }
}
