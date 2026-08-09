using Godot;
using MegaCrit.Sts2.Core.Nodes.Rooms;

namespace Sts2BalanceMod.src.Runtime.Effects;

public sealed class GhostIgniteEffect : NSts1Effect
{
  private const int ParticleCount = 25;

  private float _x;
  private float _y;

  public static GhostIgniteEffect Create(float x, float y)
  {
    var effect = new GhostIgniteEffect
    {
      _x = x,
      _y = y,
    };
    effect.Setup();
    return effect;
  }

  protected override void Initialize()
  {
    Duration = 0.1f;
    StartingDuration = Duration;
  }

  protected override void Update(float delta)
  {
    var vfxContainer = NCombatRoom.Instance?.CombatVfxContainer;
    if (vfxContainer != null)
    {
      for (var i = 0; i < ParticleCount; i++)
      {
        vfxContainer.AddChild(FireBurstParticleEffect.Create(_x, _y).Root);
        vfxContainer.AddChild(LightFlareParticleEffect.Create(
          _x,
          _y,
          new Color(0.5f, 1f, 0f, 1f)).Root);
      }
    }

    IsDone = true;
  }
}
