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
using MegaCrit.Sts2.Core.Runs.History;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events;

/// <summary>
/// STS1-EVENT-06 / MIND-BLOOM-02 / MIND-BLOOM-04 — 心灵绽放。
/// 第一战使用本局第一幕原版 Boss；胜利结算后恢复事件，第二战通过 MindBloomSecondFight 模块接入。
/// </summary>
[RegisterSharedEvent]
public sealed class MindBloom : BalanceEventTemplate
{
    public override bool IsShared => true;
    private const int _fightGold = 50;
    private const int _goldRewardAmount = 999;
    private bool _isBeforeTreasure;

    internal static bool NeedsReplayInitialization { get; set; }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
      new GoldVar(_goldRewardAmount),
    ];

    public override void OnRoomEnter()
    {
        NeedsReplayInitialization = false;
    }

    public override bool IsAllowed(IRunState runState)
    {
        return runState.CurrentActIndex == 2;
    }

    public override void CalculateVars()
    {
        Player? owner = Owner;
        if (owner == null)
            return;

        int threshold = owner.RunState.Players.Count > 1 ? 38 : 41;
        _isBeforeTreasure = owner.RunState.TotalFloor < threshold;
    }

    protected override void SetInitialEventState(bool isPreFinished)
    {
        if (HasCompletedFirstFight())
        {
            SetEventState(PageDescription("POST_FIRST"), GeneratePostFirstOptions());
            return;
        }

        base.SetInitialEventState(isPreFinished);
    }

    protected override IReadOnlyList<EventOption> GenerateInitialOptions()
    {
        List<EventOption> options =
        [
          Option(Fight),
          Option(Upgrade, "INITIAL", [.. HoverTipFactory.FromRelic(ModelDb.Relic<MarkOfTheBloom>())]),
        ];

        options.Add(_isBeforeTreasure
          ? Option(Gold, "INITIAL", [.. HoverTipFactory.FromCardWithCardHoverTips<Normality>()])
          : Option(Heal, "INITIAL", [.. HoverTipFactory.FromCardWithCardHoverTips<Doubt>()]));

        return options;
    }

    private Task Fight()
    {
        Player? owner = Owner;
        if (owner == null || Rng == null)
            return Task.CompletedTask;

        List<EncounterModel> bosses = GetFightBosses(owner);
        if (bosses.Count == 0)
            return Task.CompletedTask;

        EncounterModel? bossEncounter = Rng.NextItem(bosses);
        if (bossEncounter == null)
            return Task.CompletedTask;
        bossEncounter = bossEncounter.ToMutable();
        bossEncounter.GenerateMonstersWithSlots(owner.RunState);

        MindBloomBossEncounter mindBloomEncounter = ModelDb.Encounter<MindBloomBossEncounter>();
        mindBloomEncounter.SetBoss(bossEncounter);

        RelicModel? rareRelic = RelicFactory.PullNextRelicFromFront(owner, RelicRarity.Rare)?.ToMutable();
        if (rareRelic == null)
            return Task.CompletedTask;

        List<Reward> rewards =
        [
          new GoldReward(_fightGold, owner),
          new RelicReward(rareRelic, owner),
        ];

        // 先写入战后页面；第一战奖励结算后 EventRoom.Resume 会重建该页面。
        SetEventState(PageDescription("POST_FIRST"), GeneratePostFirstOptions());
        EnterCombatWithoutExitingEvent(mindBloomEncounter, rewards, true);
        return Task.CompletedTask;
    }

    private List<EventOption> GeneratePostFirstOptions()
    {
        List<EventOption> options = [];
        if (MindBloomSecondFight.IsReady)
            options.Add(Option(ContinueFight, "POST_FIRST"));

        options.Add(Option(LeaveAfterFirstFight, "POST_FIRST"));
        return options;
    }

    private bool HasCompletedFirstFight()
    {
        IReadOnlyList<MapPointRoomHistoryEntry>? rooms = Owner?.RunState.CurrentMapPointHistoryEntry?.Rooms;
        if (rooms == null)
            return false;

        ModelId firstFightEncounterId = ModelDb.Encounter<MindBloomBossEncounter>().Id;
        return rooms.Any(room =>
          room.ModelId == firstFightEncounterId && room.TurnsTaken > 0);
    }

    private Task ContinueFight()
    {
        Player? owner = Owner;
        if (owner == null || Rng == null ||
            !MindBloomSecondFight.TryCreatePlan(owner, Rng, out MindBloomSecondFightPlan? plan) || plan == null)
        {
            return Task.CompletedTask;
        }

        NeedsReplayInitialization = true;
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
    private static List<EncounterModel> GetFightBosses(Player owner)
    {
        ActModel? firstAct = owner.RunState.Acts.Count > 0 ? owner.RunState.Acts[0] : null;
        if (firstAct is not (Overgrowth or Underdocks))
            return [];

        return [.. firstAct.AllBossEncounters];
    }

    private async Task Upgrade()
    {
        Player? owner = Owner;
        if (owner == null)
            return;

        foreach (CardModel card in PileType.Deck.GetPile(owner).Cards)
        {
            if (card.IsUpgradable)
                CardCmd.Upgrade(card);
        }

        await RelicCmd.Obtain(ModelDb.Relic<MarkOfTheBloom>().ToMutable(), owner);
        SetEventFinished(PageDescription("UPGRADE"));
    }

    private async Task Gold()
    {
        Player? owner = Owner;
        if (owner == null)
            return;

        await PlayerCmd.GainGold(_goldRewardAmount, owner);
        for (int i = 0; i < 2; i++)
        {
            CardModel card = owner.RunState.CreateCard(ModelDb.Card<Normality>(), owner);
            CardPileAddResult result = await CardPileCmd.Add(card, PileType.Deck);
            CardCmd.PreviewCardPileAdd([result], 2f);
        }

        await Cmd.Wait(0.75f);
        SetEventFinished(PageDescription("GOLD"));
    }

    private async Task Heal()
    {
        Player? owner = Owner;
        if (owner?.Creature == null)
            return;

        await CreatureCmd.Heal(owner.Creature, owner.Creature.MaxHp);
        CardModel card = owner.RunState.CreateCard(ModelDb.Card<Doubt>(), owner);
        CardPileAddResult result = await CardPileCmd.Add(card, PileType.Deck);
        CardCmd.PreviewCardPileAdd([result], 2f);
        await Cmd.Wait(0.75f);
        SetEventFinished(PageDescription("HEAL"));
    }

}
