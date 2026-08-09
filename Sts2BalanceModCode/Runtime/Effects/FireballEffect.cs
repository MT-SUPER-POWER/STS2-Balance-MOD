using Godot;

namespace Sts2BalanceMod.Sts2BalanceModCode.Runtime.Effects;

public sealed class FireballEffect : NSts1Effect
{
    private const float EffectDuration = 0.5f;
    private const float FireballInterval = 0.016f;

    private Vector2 _startPosition;
    private Vector2 _targetPosition;
    private float _vfxTimer;

    public static FireballEffect Create(Vector2 startPosition, Vector2 targetPosition)
    {
        var effect = new FireballEffect
        {
            _startPosition = startPosition,
            _targetPosition = targetPosition + new Vector2(
            (float)(Random.Shared.NextDouble() * 40.0 - 20.0),
            (float)(Random.Shared.NextDouble() * 40.0 - 20.0)),
            Position = startPosition,
        };
        effect.Setup();
        return effect;
    }

    protected override void Initialize()
    {
        Duration = EffectDuration;
        StartingDuration = EffectDuration;
        _vfxTimer = 0f;
    }

    protected override void Update(float delta)
    {
        var progress = Duration / StartingDuration;
        Position = new Vector2(
          Lerp(_targetPosition.X, _startPosition.X, Fade(progress)),
          Lerp(_targetPosition.Y, _startPosition.Y, Fade(progress)));

        _vfxTimer -= delta;
        if (_vfxTimer < 0f)
        {
            _vfxTimer = FireballInterval;
            SpawnTrailParticles();
        }

        Duration -= delta;
        if (Duration >= 0f)
            return;

        IsDone = true;
        SpawnImpactEffects();
    }

    private void SpawnTrailParticles()
    {
        var parent = GetParent();
        if (parent == null)
            return;

        parent.AddChild(LightFlareParticleEffect.Create(
          Position.X,
          Position.Y,
          new Color(0.5f, 1f, 0f, 1f)).Root);
        parent.AddChild(FireBurstParticleEffect.Create(Position.X, Position.Y).Root);
    }

    private void SpawnImpactEffects()
    {
        var parent = GetParent();
        if (parent == null)
            return;

        parent.AddChild(GhostIgniteEffect.Create(Position.X, Position.Y).Root);
        parent.AddChild(GhostlyWeakFireEffect.Create(Position.X, Position.Y).Root);
    }

    private static float Fade(float value)
    {
        return Mathf.Clamp(value * value * value * (value * (value * 6f - 15f) + 10f), 0f, 1f);
    }
}
