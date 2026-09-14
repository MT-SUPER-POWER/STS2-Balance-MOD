using Godot;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Random;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events.UI;

/// <summary>
/// STS1-EVENT-07 — 大转盘小游戏逻辑。
/// 管理转盘结果、角度计算，并通过 NWheelSpinScreen 展示 UI。
/// 移植自 ActsFromThePast.Minigames.WheelSpinMinigame。
/// </summary>
public class WheelSpinMinigame(Player owner, int result, int actIndex)
{
  private readonly TaskCompletionSource _completionSource = new();
  private readonly Player _owner = owner;

  /// <summary>
  /// 结果段索引（0-5）
  /// </summary>
  public int Result { get; } = result;

  /// <summary>
  /// 视觉着陆角度（度），包含小幅抖动偏移
  /// </summary>
  public float ResultAngle { get; } = result * 60f + Rng.Chaotic.NextInt(-10, 11);

  /// <summary>
  /// 当前幕索引，用于选择背景
  /// </summary>
  public int ActIndex { get; } = actIndex;

  public event Action? Finished;

  public void Complete()
  {
    if (_completionSource.Task.IsCompleted)
      return;
    _completionSource.SetResult();
    Finished?.Invoke();
  }

  public void ForceEnd() => _completionSource.TrySetCanceled();

  public async Task PlayMinigame()
  {
    if (!LocalContext.IsMe(_owner))
      return;

    NWheelSpinScreen.ShowScreen(this);
    await _completionSource.Task;
  }
}
