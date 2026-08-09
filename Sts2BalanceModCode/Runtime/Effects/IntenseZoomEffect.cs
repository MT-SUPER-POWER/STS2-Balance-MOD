using Godot;

namespace Sts2BalanceMod.Sts2BalanceModCode.Runtime.Effects;

/// <summary>
/// 守护者形态转换时的聚焦闪光。
/// </summary>
public sealed class IntenseZoomEffect : NSts1Effect
{
  private const int ParticleCount = 10;

  private Vector2 _targetPosition;
  private bool _isBlack;

  public static IntenseZoomEffect Create(Vector2 position, bool isBlack = false)
  {
    var effect = new IntenseZoomEffect
    {
      _targetPosition = position,
      _isBlack = isBlack,
      Position = position,
    };
    effect.Setup();
    return effect;
  }

  protected override void Initialize()
  {
    BorderFlashEffect.Play(_isBlack ? Colors.Black : new Color(0.937f, 0.808f, 0.373f));
    var parent = GetParent();
    if (parent != null)
    {
      for (var i = 0; i < ParticleCount; i++)
        parent.AddChild(IntenseZoomParticle.Create(_targetPosition, _isBlack).Root);
    }

    IsDone = true;
  }

  protected override void Update(float delta)
  {
  }
}
