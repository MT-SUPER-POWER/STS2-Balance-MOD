using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Encounters;

/// <summary>
/// AFP-BOSS-02 — 心灵绽放专用六火亡魂遭遇。
/// 目标类型：Hexaghost；仅由事件或开发调试显式调用，不注册到普通地图池。
/// RoomType 保持 Monster，避免胜利后触发 Boss 换幕流程。
/// </summary>
[RegisterGlobalEncounter]
public sealed class MindBloomHexaghost : BalanceEncounterTemplate
{
    private MindBloomBossEnhancementPlan? _enhancementPlan;

    public override RoomType RoomType => RoomType.Monster;

    public override float GetCameraScaling() => 0.9f;

    public override Vector2 GetCameraOffset() => Vector2.Down * 50f;

    public override EncounterAssetProfile AssetProfile => new(
      ExtraAssetPaths:
      [
        ModAssetPaths.Resource("monsters", "hexaghost", "plasma1.png"),
      ModAssetPaths.Resource("monsters", "hexaghost", "plasma2.png"),
      ModAssetPaths.Resource("monsters", "hexaghost", "plasma3.png"),
      ModAssetPaths.Resource("monsters", "hexaghost", "shadow.png"),
      ModAssetPaths.Resource("vfx", "vfx.atlas"),
      ModAssetPaths.Resource("vfx", "vfx.png"),
      ModAssetPaths.Resource("vfx", "vfx2.png"),
      ]);

    public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Hexaghost>()];

    internal void SetEnhancementPlan(MindBloomBossEnhancementPlan plan)
    {
        // NOTE: EventModel 要求传入 canonical 遭遇，配置字段会在进入战斗时随 ToMutable 一起复制。
        _enhancementPlan = plan;
    }

    protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
    {
        var boss = (Hexaghost)ModelDb.Monster<Hexaghost>().ToMutable();
        boss.MindBloomEnhancementPlan = _enhancementPlan;
        return [(boss, null)];
    }
}
