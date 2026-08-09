using MegaCrit.Sts2.Core.Helpers;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Audio;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Combat;

namespace Sts2BalanceMod.Sts2BalanceModCode.Runtime.Effects;

public sealed class ScreenOnFireEffect : NSts1Effect
{
    private const float EffectDuration = 3f;
    private const float SpawnInterval = 0.05f;

    private float _spawnTimer;
    private bool _playedInitialEffects;

    public static ScreenOnFireEffect Create()
    {
        var effect = new ScreenOnFireEffect();
        effect.Setup();
        return effect;
    }

    protected override void Initialize()
    {
        Duration = EffectDuration;
        StartingDuration = EffectDuration;
        _spawnTimer = 0f;
        _playedInitialEffects = false;
    }

    protected override void Update(float delta)
    {
        if (!_playedInitialEffects)
        {
            _playedInitialEffects = true;
            AFTPModAudio.Play("hexaghost", "ghost_flames");
            BorderFlashEffect.PlayFire();
        }

        Duration -= delta;
        _spawnTimer -= delta;

        if (_spawnTimer < 0f)
        {
            _spawnTimer = SpawnInterval;
            var parent = GetParent();
            if (parent != null)
            {
                for (var i = 0; i < 8; i++)
                    parent.AddChildSafely(GiantFireEffect.Create().Root);
            }
        }

        if (Duration < 0f)
            IsDone = true;
    }
}
