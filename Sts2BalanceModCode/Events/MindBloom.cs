using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events;

/// <summary>
/// STS1-EVENT-06 / MIND-BLOOM-02 — 心灵绽放。
/// 第一战使用本局第一幕原版 Boss；胜利结算后恢复事件，第二战通过 MindBloomSecondFight 模块接入。
/// </summary>
public sealed class MindBloom : CustomEventModel
{
  private const int FightGold = 50;
  private const int GoldRewardAmount = 999;
  private bool _isBeforeTreasure;
  public override ActModel[] Acts => [];

  public override bool IsShared => true;

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new GoldVar(GoldRewardAmount),
  ];

  public override bool IsAllowed(IRunState runState)
  {
    return runState.CurrentActIndex == 2;
  }

  public override void CalculateVars()
  {
    var owner = Owner;
    if (owner == null)
      return;

    var threshold = owner.RunState.Players.Count > 1 ? 38 : 41;
    _isBeforeTreasure = owner.RunState.TotalFloor < threshold;
  }

  protected override IReadOnlyList<EventOption> GenerateInitialOptions()
  {
    var options = new List<EventOption>
    {
      Option(Fight),
      Option(Upgrade, "INITIAL", HoverTipFactory.FromRelic(ModelDb.Relic<MarkOfTheBloom>()).ToArray()),
    };

    options.Add(_isBeforeTreasure
      ? Option(Gold, "INITIAL", HoverTipFactory.FromCardWithCardHoverTips<Normality>().ToArray())
      : Option(Heal, "INITIAL", HoverTipFactory.FromCardWithCardHoverTips<Doubt>().ToArray()));

    return options;
  }

  private Task Fight()
  {
    var owner = Owner;
    if (owner == null || Rng == null)
      return Task.CompletedTask;

    var bosses = GetFightBosses(owner);
    if (bosses.Count == 0)
      return Task.CompletedTask;

    var bossEncounter = Rng.NextItem(bosses);
    if (bossEncounter == null)
      return Task.CompletedTask;
    bossEncounter = bossEncounter.ToMutable();
    bossEncounter.GenerateMonstersWithSlots(owner.RunState);

    var mindBloomEncounter = ModelDb.Encounter<MindBloomBossEncounter>();
    mindBloomEncounter.SetBoss(bossEncounter);

    var rareRelic = RelicFactory.PullNextRelicFromFront(owner, RelicRarity.Rare)?.ToMutable();
    if (rareRelic == null)
      return Task.CompletedTask;

    var rewards = new List<Reward>
    {
      new GoldReward(FightGold, owner),
      new RelicReward(rareRelic, owner),
    };

    // 先写入战后页面；第一战奖励结算后 EventRoom.Resume 会重建该页面。
    SetEventState(PageDescription("POST_FIRST"), GeneratePostFirstOptions());
    EnterCombatWithoutExitingEvent(mindBloomEncounter, rewards, true);
    return Task.CompletedTask;
  }

  private IReadOnlyList<EventOption> GeneratePostFirstOptions()
  {
    var options = new List<EventOption>();
    if (MindBloomSecondFight.IsReady)
      options.Add(Option(ContinueFight, "POST_FIRST"));

    options.Add(Option(LeaveAfterFirstFight, "POST_FIRST"));
    return options;
  }

  private Task ContinueFight()
  {
    var owner = Owner;
    if (owner == null || Rng == null ||
        !MindBloomSecondFight.TryCreatePlan(owner, Rng, out var plan) || plan == null)
    {
      return Task.CompletedTask;
    }

    EnterCombatWithoutExitingEvent(plan.Encounter, plan.Rewards, false);
    return Task.CompletedTask;
  }

  private Task LeaveAfterFirstFight()
  {
    SetEventFinished(PageDescription("LEAVE_AFTER_FIRST"));
    return Task.CompletedTask;
  }

  /// <summary>
  /// 从本局第一层（密林 Overgrowth 或暗港 Underdocks）全量 Boss 池中真随机选取。
  /// </summary>
  private static IReadOnlyList<EncounterModel> GetFightBosses(Player owner)
  {
    var firstAct = owner.RunState.Acts.FirstOrDefault();
    if (firstAct is not (Overgrowth or Underdocks))
      return [];

    return firstAct.AllBossEncounters.ToList();
  }

  private async Task Upgrade()
  {
    var owner = Owner;
    if (owner == null)
      return;

    foreach (var card in PileType.Deck.GetPile(owner).Cards)
    {
      if (card.IsUpgradable)
        CardCmd.Upgrade(card);
    }

    await RelicCmd.Obtain(ModelDb.Relic<MarkOfTheBloom>().ToMutable(), owner);
    SetEventFinished(PageDescription("UPGRADE"));
  }

  private async Task Gold()
  {
    var owner = Owner;
    if (owner == null)
      return;

    await PlayerCmd.GainGold(GoldRewardAmount, owner);
    for (var i = 0; i < 2; i++)
    {
      var card = owner.RunState.CreateCard(ModelDb.Card<Normality>(), owner);
      var result = await CardPileCmd.Add(card, PileType.Deck);
      CardCmd.PreviewCardPileAdd([result], 2f);
    }

    await Cmd.Wait(0.75f);
    SetEventFinished(PageDescription("GOLD"));
  }

  private async Task Heal()
  {
    var owner = Owner;
    if (owner?.Creature == null)
      return;

    await CreatureCmd.Heal(owner.Creature, owner.Creature.MaxHp);
    var card = owner.RunState.CreateCard(ModelDb.Card<Doubt>(), owner);
    var result = await CardPileCmd.Add(card, PileType.Deck);
    CardCmd.PreviewCardPileAdd([result], 2f);
    await Cmd.Wait(0.75f);
    SetEventFinished(PageDescription("HEAL"));
  }

}
