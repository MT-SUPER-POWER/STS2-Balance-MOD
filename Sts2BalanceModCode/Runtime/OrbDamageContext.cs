using System.Threading;

namespace Sts2BalanceMod.Sts2BalanceModCode.Runtime;

/// <summary>
/// 跟踪当前调用栈是否处于充能球（Orb）触发与伤害流程中。
/// 基于 AsyncLocal 实现跨 async/await 异步调用的上下文传递。
/// </summary>
public static class OrbDamageContext
{
  private static readonly AsyncLocal<bool> _isOrbDealingDamage = new();

  public static bool IsOrbDealingDamage
  {
    get => _isOrbDealingDamage.Value;
    set => _isOrbDealingDamage.Value = value;
  }
}
