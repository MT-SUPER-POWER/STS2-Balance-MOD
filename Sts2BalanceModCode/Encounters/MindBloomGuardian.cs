using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Encounters;

/// <summary>
/// AFP-BOSS-01 — 心灵绽放专用守护者遭遇。
/// 目标类型：Guardian；仅由事件或开发调试显式调用，不注册到普通地图池。
/// RoomType 保持 Monster，避免胜利后触发 Boss 换幕流程。
/// </summary>
[RegisterGlobalEncounter]
public sealed class MindBloomGuardian : BalanceEncounterTemplate
{
    private MindBloomBossEnhancementPlan? _enhancementPlan;

    public override RoomType RoomType => RoomType.Monster;

    public override EncounterAssetProfile AssetProfile => new(
      ExtraAssetPaths:
      [
        ModAssetPaths.PowerIcon("ModeShiftPower.png"),
      ModAssetPaths.PowerIcon("SharpHidePower.png"),
      ]);

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Guardian>()];

    internal void SetEnhancementPlan(MindBloomBossEnhancementPlan plan)
    {
        // NOTE: EventModel 要求传入 canonical 遭遇，配置字段会在进入战斗时随 ToMutable 一起复制。
        _enhancementPlan = plan;
    }

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
    {
        var boss = (Guardian)ModelDb.Monster<Guardian>().ToMutable();
        boss.MindBloomEnhancementPlan = _enhancementPlan;
        return [(boss, null)];
    }
}
