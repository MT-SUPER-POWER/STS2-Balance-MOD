using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Encounters;

/// <summary>
/// EVENT-COLOSSEUM-01 — 竞技场第一战遭遇：蓝奴隶贩子 + 红奴隶贩子。
/// 战胜后不生成战利品奖励（ShouldGiveRewards => false），直接返回事件界面进行中场抉择。
/// </summary>
[RegisterGlobalEncounter]
public sealed class ColosseumFirstEncounter : BalanceEncounterTemplate
{
    public override RoomType RoomType => RoomType.Monster;

    public override bool ShouldGiveRewards => false;

    public override bool IsWeak => false;

    public override EncounterAssetProfile AssetProfile => new(
        EncounterScenePath: ModAssetPaths.Resource("scenes", "colosseum_first_encounter.tscn"));

    public override IReadOnlyList<string> Slots => ["blue", "red"];

    public override IEnumerable<MonsterModel> AllPossibleMonsters =>
    [
      ModelDb.Monster<SlaverBlue>(),
      ModelDb.Monster<SlaverRed>(),
    ];

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
    [
      (ModelDb.Monster<SlaverBlue>().ToMutable(), "blue"),
      (ModelDb.Monster<SlaverRed>().ToMutable(), "red"),
    ];
}
