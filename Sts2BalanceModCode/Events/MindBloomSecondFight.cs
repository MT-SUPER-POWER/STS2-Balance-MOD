using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rewards;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events;

/// <summary>
/// 心灵绽放第二战的完整执行计划。
/// 事件只依赖这份结果，不需要知道 Boss 候选池、奖励或强化的生成细节。
/// </summary>
internal sealed record MindBloomSecondFightPlan(
  EncounterModel Encounter,
  IReadOnlyList<Reward> Rewards);

/// <summary>
/// MIND-BLOOM-02 的第二战规则模块。
/// 接缝保持为 IsReady + TryCreatePlan；后续奖励和随机强化只在本模块内部实现。
/// </summary>
internal static class MindBloomSecondFight
{
  /// <summary>
  /// NOTE: 奖励与随机强化尚待产品讨论。在两者落地前，不向玩家暴露未完成的第二战选项。
  /// </summary>
  internal static bool IsReady => false;

  internal static bool TryCreatePlan(
    Player owner,
    Rng rng,
    out MindBloomSecondFightPlan? plan)
  {
    if (!IsReady)
    {
      plan = null;
      return false;
    }

    var encounter = rng.NextItem<EncounterModel>(
    [
      ModelDb.Encounter<MindBloomGuardian>(),
      ModelDb.Encounter<MindBloomHexaghost>(),
      ModelDb.Encounter<MindBloomSlimeBoss>(),
    ]);
    if (encounter == null)
    {
      plan = null;
      return false;
    }

    // TODO(MIND-BLOOM-02): 在这里生成第二战追加奖励，并为所选 Boss 生成随机强化计划。
    // 在奖励与强化规则确认前 IsReady 保持 false，本空列表不会进入玩家流程。
    plan = new MindBloomSecondFightPlan(encounter, Array.Empty<Reward>());
    return true;
  }
}
