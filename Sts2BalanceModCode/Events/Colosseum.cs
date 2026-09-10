using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Runs.History;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events;

/// <summary>
/// EVENT-COLOSSEUM-01 — 一代经典回归事件：竞技场 (Colosseum)。
/// 注册至第三幕（Glory 荣耀之都）。
/// 包含两阶段角斗战斗：第一战对阵双奴隶贩子（无战利品），战胜后可选择趁乱逃跑或迎战头目与地精大块头夺取稀有遗物、罕见遗物与金币。
/// </summary>
[RegisterActEvent(typeof(Glory))]
public sealed class Colosseum : BalanceEventTemplate
{
    public override bool IsShared => true;

    internal static bool NeedsReplayInitialization { get; set; }

    public override void OnRoomEnter()
    {
        NeedsReplayInitialization = false;
    }

    public override bool IsAllowed(IRunState runState) => true;

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
        return [Option(Fight)];
    }

    private Task Fight()
    {
        ColosseumFirstEncounter encounter = ModelDb.Encounter<ColosseumFirstEncounter>();
        // 先设定好战后页面，第一战胜利结算后恢复事件将呈现中场抉择
        SetEventState(PageDescription("POST_FIRST"), GeneratePostFirstOptions());
        EnterCombatWithoutExitingEvent(encounter, [], true);
        return Task.CompletedTask;
    }

    public override Task Resume(AbstractRoom room)
    {
        SetEventState(PageDescription("POST_FIRST"), GeneratePostFirstOptions());
        return Task.CompletedTask;
    }

    private List<EventOption> GeneratePostFirstOptions()
    {
        return
        [
          Option(FightAgain, "POST_FIRST"),
          Option(Flee, "POST_FIRST"),
        ];
    }

    private Task FightAgain()
    {
        Player? owner = Owner;
        if (owner == null)
            return Task.CompletedTask;

        NeedsReplayInitialization = true;

        RelicModel? rareRelic = RelicFactory.PullNextRelicFromFront(owner, RelicRarity.Rare)?.ToMutable();
        RelicModel? uncommonRelic = RelicFactory.PullNextRelicFromFront(owner, RelicRarity.Uncommon)?.ToMutable();

        List<Reward> rewards = [];
        if (rareRelic != null)
            rewards.Add(new RelicReward(rareRelic, owner));
        if (uncommonRelic != null)
            rewards.Add(new RelicReward(uncommonRelic, owner));
        rewards.Add(new GoldReward(100, owner));

        ColosseumSecondEncounter secondEncounter = ModelDb.Encounter<ColosseumSecondEncounter>();
        EnterCombatWithoutExitingEvent(secondEncounter, rewards, false);
        return Task.CompletedTask;
    }

    private Task Flee()
    {
        SetEventFinished(PageDescription("FLEE"));
        return Task.CompletedTask;
    }

    private bool HasCompletedFirstFight()
    {
        IReadOnlyList<MapPointRoomHistoryEntry>? rooms = Owner?.RunState.CurrentMapPointHistoryEntry?.Rooms;
        if (rooms == null)
            return false;

        ModelId firstFightEncounterId = ModelDb.Encounter<ColosseumFirstEncounter>().Id;
        return rooms.Any(room =>
          room.ModelId == firstFightEncounterId && room.TurnsTaken > 0);
    }
}
