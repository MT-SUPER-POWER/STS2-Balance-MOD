using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;
using Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events;

/// <summary>
/// 冒险者尸体 (Dead Adventurer)。
/// 经典一代第一幕专属事件，仅在第 1 幕（Overgrowth / Underdocks）第 7 层以上出现。
/// 初始遇敌概率受进阶影响（A0~A14 为 25%，A15+ 为 35%），每次搜查 +25%。
/// 搜查可获得 30G、随机遗物或空。触发战斗后击败一代精英，全额补发未搜出的全部剩余战利品。
/// </summary>
[RegisterActEvent(typeof(Overgrowth))]
[RegisterActEvent(typeof(Underdocks))]
public sealed class DeadAdventurer : BalanceEventTemplate
{
  public override bool IsShared => false;

  public override EventLayoutType LayoutType => EventLayoutType.Combat;

  public override EncounterModel CanonicalEncounter => _enemyType switch
  {
    0 => ModelDb.Encounter<DeadAdventurerSentriesEncounter>(),
    1 => ModelDb.Encounter<DeadAdventurerNobEncounter>(),
    _ => ModelDb.Encounter<DeadAdventurerLagavulinEncounter>()
  };

  public override EventAssetProfile AssetProfile => new();

  private const int GoldRewardAmount = 30;
  private const int EncounterChanceRamp = 25;

  private static int EncounterChanceStart => AscensionHelper.HasAscension(AscensionLevel.DeadlyEnemies) ? 35 : 25;

  public override bool IsAllowed(IRunState runState) =>
    runState.CurrentActIndex == 0 && runState.TotalFloor >= 7;

  private int _encounterChance;
  private int _numSearches;
  private int _enemyType;
  private List<RewardType> _rewards = [];

  private enum RewardType { Gold, Relic, Nothing }

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new IntVar("EncounterChance", EncounterChanceStart)
  ];

  public override void CalculateVars()
  {
    _encounterChance = EncounterChanceStart;
    _numSearches = 0;
    _enemyType = Rng?.NextInt(3) ?? 0;

    _rewards = [RewardType.Gold, RewardType.Relic, RewardType.Nothing];
    if (Rng != null)
    {
      for (int i = _rewards.Count - 1; i > 0; i--)
      {
        int j = Rng.NextInt(i + 1);
        (_rewards[i], _rewards[j]) = (_rewards[j], _rewards[i]);
      }
    }

    DynamicVars["EncounterChance"].BaseValue = _encounterChance;
  }

  public override LocString InitialDescription
  {
    get
    {
      string key = _enemyType switch
      {
        0 => "STS2_BALANCE_MOD_EVENT_DEAD_ADVENTURER.pages.INITIAL.description.SENTRIES",
        1 => "STS2_BALANCE_MOD_EVENT_DEAD_ADVENTURER.pages.INITIAL.description.NOB",
        _ => "STS2_BALANCE_MOD_EVENT_DEAD_ADVENTURER.pages.INITIAL.description.LAGAVULIN"
      };
      return L10NLookup(key);
    }
  }

  protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
  [
    Option(Search),
    Option(Leave)
  ];

  private async Task Search()
  {
    if (Rng != null && Rng.NextInt(100) < _encounterChance)
    {
      await TriggerCombat();
    }
    else
    {
      await GrantReward();
    }
  }

  private Task TriggerCombat()
  {
    DeadAdventurerCombatPatch.RevealEnemies();
    SetEventState(
      PageDescription("FIGHT"),
      [Option(EnterCombat, "FIGHT")]);
    return Task.CompletedTask;
  }

  private Task EnterCombat()
  {
    Player? owner = Owner;
    if (owner == null)
      return Task.CompletedTask;

    EncounterModel encounter = CanonicalEncounter;

    List<Reward> rewards =
    [
      new CardReward(CardCreationOptions.ForRoom(owner, RoomType.Elite), 3, owner),
      new GoldReward(25, 35, owner)
    ];

    foreach (RewardType rewardType in _rewards)
    {
      switch (rewardType)
      {
        case RewardType.Gold:
          rewards.Add(new GoldReward(GoldRewardAmount, owner));
          break;
        case RewardType.Relic:
          RelicModel? relic = RelicFactory.PullNextRelicFromFront(owner)?.ToMutable();
          if (relic != null)
            rewards.Add(new RelicReward(relic, owner));
          break;
      }
    }

    EnterCombatWithoutExitingEvent(encounter, rewards, false);
    return Task.CompletedTask;
  }

  private async Task GrantReward()
  {
    _numSearches++;
    _encounterChance += EncounterChanceRamp;
    DynamicVars["EncounterChance"].BaseValue = _encounterChance;

    RewardType reward = _rewards[0];
    _rewards.RemoveAt(0);
    bool wasLastReward = _numSearches >= 3;

    Player? owner = Owner;
    if (owner != null)
    {
      switch (reward)
      {
        case RewardType.Gold:
          await PlayerCmd.GainGold(GoldRewardAmount, owner);
          break;
        case RewardType.Nothing:
          break;
        case RewardType.Relic:
          RelicModel? relic = RelicFactory.PullNextRelicFromFront(owner)?.ToMutable();
          if (relic != null)
            await RelicCmd.Obtain(relic, owner);
          break;
      }
    }

    if (wasLastReward)
    {
      SetEventFinished(PageDescription("SUCCESS"));
    }
    else
    {
      string pageKey = reward switch
      {
        RewardType.Gold => "GOLD",
        RewardType.Relic => "RELIC",
        _ => "NOTHING"
      };
      SetEventState(PageDescription(pageKey), GetPostRewardOptions());
    }
  }

  private IReadOnlyList<EventOption> GetPostRewardOptions()
  {
    if (_numSearches >= 3)
    {
      return [Option(Leave, "SUCCESS")];
    }

    return
    [
      Option(Search),
      Option(Leave)
    ];
  }

  private Task Leave()
  {
    SetEventFinished(PageDescription("LEAVE"));
    return Task.CompletedTask;
  }
}
