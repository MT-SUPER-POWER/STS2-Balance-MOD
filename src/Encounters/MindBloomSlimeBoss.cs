using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Content;
using STS2RitsuLib.Interop.AutoRegistration;
using Sts2BalanceMod.src.Abstract;
using Sts2BalanceMod.src.Extensions;
using Sts2BalanceMod.src.Monsters;

namespace Sts2BalanceMod.src.Encounters;

/// <summary>
/// AFP-BOSS-03 — 心灵绽放专用史莱姆 Boss 遭遇。
/// 目标类型：SlimeBoss 及完整分裂怪物链；仅由事件或开发调试显式调用。
/// 自定义场景提供大型与中型史莱姆的固定槽位；RoomType 保持 Monster，避免触发换幕。
/// </summary>
[RegisterGlobalEncounter]
public sealed class MindBloomSlimeBoss : BalanceEncounterTemplate
{
  private MindBloomBossEnhancementPlan? _enhancementPlan;

  public override RoomType RoomType => RoomType.Monster;

  public override EncounterAssetProfile AssetProfile => new(
    EncounterScenePath: ModAssetPaths.Resource("scenes", "actsfromthepast-mind_bloom_slime_boss.tscn"),
    ExtraAssetPaths: [ModAssetPaths.PowerIcon("actsfromthepast-split_power.png")]);

  public override IReadOnlyList<string> Slots =>
  [
    "spike_med_1", "spike_large", "spike_med_2",
    "acid_med_1", "slime_boss", "acid_large", "acid_med_2",
  ];

  public override IEnumerable<MonsterModel> AllPossibleMonsters =>
  [
    ModelDb.Monster<SlimeBoss>(),
    ModelDb.Monster<SpikeSlimeLarge>(),
    ModelDb.Monster<SpikeSlimeMedium>(),
    ModelDb.Monster<AcidSlimeLarge>(),
    ModelDb.Monster<AcidSlimeMedium>(),
  ];

  internal void SetEnhancementPlan(MindBloomBossEnhancementPlan plan)
  {
    // NOTE: EventModel 要求传入 canonical 遭遇，配置字段会在进入战斗时随 ToMutable 一起复制。
    _enhancementPlan = plan;
  }

  protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
  {
    var boss = (SlimeBoss)ModelDb.Monster<SlimeBoss>().ToMutable();
    boss.MindBloomEnhancementPlan = _enhancementPlan;
    return [(boss, "slime_boss")];
  }
}
