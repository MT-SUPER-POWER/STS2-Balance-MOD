using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Sts2BalanceMod.Sts2BalanceModCode.Runtime.Effects;

public static class BorderFlashEffect
{
  private static NSmokyVignetteVfx? _currentVfx;

  public static void Play(Color tint, Color? highlight = null)
  {
    var highlightColor = highlight ?? new Color(tint.R, tint.G, tint.B, 0.15f);

    if (_currentVfx != null && GodotObject.IsInstanceValid(_currentVfx))
    {
      _currentVfx.Reset(tint, highlightColor);
      return;
    }

    _currentVfx = NSmokyVignetteVfx.Create(tint, highlightColor);
    NRun.Instance?.GlobalUi.AddChildSafely(_currentVfx);
  }

  public static void PlayChartreuse()
  {
    Play(
      new Color(0.5f, 1f, 0f, 0.3f),
      new Color(0.7f, 1f, 0.2f, 0.15f));
  }

  public static void PlayFire()
  {
    Play(
      new Color(1f, 0.3f, 0f, 0.4f),
      new Color(1f, 0.5f, 0f, 0.2f));
  }
}
