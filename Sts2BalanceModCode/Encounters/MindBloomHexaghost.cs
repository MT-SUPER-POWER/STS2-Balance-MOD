using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Encounters;

/// <summary>
/// AFP-BOSS-02 — 心灵绽放专用六火亡魂遭遇。
/// 目标类型：Hexaghost；仅由事件或开发调试显式调用，不注册到普通地图池。
/// RoomType 保持 Monster，避免胜利后触发 Boss 换幕流程。
/// </summary>
public sealed class MindBloomHexaghost : Sts2EncounterModel
{
  private const string VisualRoot = "res://Sts2BalanceMod/monsters/hexaghost";
  private const string VfxTexture = "res://Sts2BalanceMod/vfx/vfx.png";
  private MindBloomBossEnhancementPlan? _enhancementPlan;

  public override RoomType RoomType => RoomType.Monster;

  public override IEnumerable<string> ExtraAssetPaths =>
  [
    $"{VisualRoot}/plasma1.png",
    $"{VisualRoot}/plasma2.png",
    $"{VisualRoot}/plasma3.png",
    $"{VisualRoot}/shadow.png",
    VfxTexture,
  ];

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
