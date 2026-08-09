using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.Rewards;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;

using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

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
  private const int ExtraGold = 100;

  internal static bool IsReady => true;

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

    EncounterModel[] encounterPool =
    [
      ModelDb.Encounter<MindBloomGuardian>(),
      ModelDb.Encounter<MindBloomHexaghost>(),
      ModelDb.Encounter<MindBloomSlimeBoss>(),
    ];
    MindBloomDurabilityEnhancement[] durabilityPool =
    [
      MindBloomDurabilityEnhancement.Giant,
      MindBloomDurabilityEnhancement.Plating,
      MindBloomDurabilityEnhancement.Regeneration,
    ];
    MindBloomThreatEnhancement[] threatPool =
    [
      MindBloomThreatEnhancement.Strength,
      MindBloomThreatEnhancement.Ritual,
    ];

    var encounter = encounterPool[rng.NextInt(0, encounterPool.Length)];
    var enhancementPlan = new MindBloomBossEnhancementPlan(
      durabilityPool[rng.NextInt(0, durabilityPool.Length)],
      threatPool[rng.NextInt(0, threatPool.Length)]);

    switch (encounter)
    {
      case MindBloomGuardian guardian:
        guardian.SetEnhancementPlan(enhancementPlan);
        break;
      case MindBloomHexaghost hexaghost:
        hexaghost.SetEnhancementPlan(enhancementPlan);
        break;
      case MindBloomSlimeBoss slimeBoss:
        slimeBoss.SetEnhancementPlan(enhancementPlan);
        break;
      default:
        plan = null;
        return false;
    }

    var rareRelic = RelicFactory.PullNextRelicFromFront(owner, RelicRarity.Rare).ToMutable();
    var uncommonRelic = RelicFactory.PullNextRelicFromFront(owner, RelicRarity.Uncommon).ToMutable();
    var rewards = new List<Reward>
    {
      new GoldReward(ExtraGold, owner),
      new RelicReward(rareRelic, owner),
      new RelicReward(uncommonRelic, owner),
    };

    plan = new MindBloomSecondFightPlan(encounter, rewards);
    return true;
  }
}
