using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Audio;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Combat;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Effects;

namespace Sts2BalanceMod.Sts2BalanceModCode.Runtime.Animations;

/// <summary>
/// Hexaghost 单枚火球的延时点燃、漂浮与持续粒子表现。
/// </summary>
public sealed class HexaghostOrbVisual : IDisposable
{
    private const float BobSpeed = 2f;
    private const float BobAmount = 3f;
    private const float ParticleInterval = 0.06f;

    private readonly int _index;
    private readonly Vector2 _basePosition;

    private Node? _parentNode;
    private Vector2 _currentPosition;
    private float _activateTimer;
    private float _bobTimer;
    private float _particleTimer;
    private bool _playedSfx;

    public bool IsActivated { get; private set; }
    public bool IsHidden { get; private set; } = true;

    public HexaghostOrbVisual(int index, Vector2 position)
    {
        _index = index;
        _basePosition = position + new Vector2(
          (float)GD.RandRange(-10f, 10f),
          (float)GD.RandRange(-10f, 10f));
        _currentPosition = _basePosition;
        _activateTimer = index * 0.3f;
    }

    public void SetParentNode(Node parent)
    {
        _parentNode = parent;
    }

    public void Activate(bool immediate = false)
    {
        _playedSfx = false;
        IsActivated = true;
        IsHidden = false;
        _activateTimer = immediate ? 0f : _index * 0.3f;
    }

    public void Deactivate()
    {
        IsActivated = false;
    }

    public void Hide()
    {
        IsHidden = true;
    }

    public void Update(float delta, Vector2 parentGlobalPosition)
    {
        if (IsHidden || _parentNode == null || !GodotObject.IsInstanceValid(_parentNode))
            return;

        _bobTimer += BobSpeed * delta;
        var bobOffset = Mathf.Sin(_bobTimer) * BobAmount;
        _currentPosition = _basePosition + new Vector2(bobOffset * 2f, bobOffset * 2f);
        var globalPosition = parentGlobalPosition + _currentPosition;

        if (IsActivated)
        {
            _activateTimer -= delta;
            if (_activateTimer >= 0f)
                return;

            if (!_playedSfx)
            {
                _playedSfx = true;
                SpawnIgniteEffect(globalPosition);
                PlayIgniteSound();
            }

            _particleTimer -= delta;
            if (_particleTimer < 0f)
            {
                SpawnFireEffect(globalPosition);
                _particleTimer = ParticleInterval;
            }

            return;
        }

        _particleTimer -= delta;
        if (_particleTimer < 0f)
        {
            SpawnWeakFireEffect(globalPosition);
            _particleTimer = ParticleInterval;
        }
    }

    private static void SpawnIgniteEffect(Vector2 position)
    {
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(
          GhostIgniteEffect.Create(position.X, position.Y).Root);
    }

    private static void SpawnFireEffect(Vector2 position)
    {
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(
          GhostlyFireEffect.Create(position.X, position.Y).Root);
    }

    private static void SpawnWeakFireEffect(Vector2 position)
    {
        NCombatRoom.Instance?.CombatVfxContainer.AddChildSafely(
          GhostlyWeakFireEffect.Create(position.X, position.Y).Root);
    }

    private static void PlayIgniteSound()
    {
        var soundName = GD.Randf() < 0.5f ? "ghost_orb_ignite_1" : "ghost_orb_ignite_2";
        AFTPModAudio.Play("hexaghost", soundName);
    }

    public void Dispose()
    {
        _parentNode = null;
    }
}
