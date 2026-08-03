using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Encounters;

/// <summary>
/// 心灵绽放专属遭遇 — RoomType 为 Monster，避免被系统当作 Boss 战触发换幕。
/// 怪物/槽位从实际抽选的一层 Boss 遭遇委托生成。
/// </summary>
public sealed class MindBloomBossEncounter : Sts2EncounterModel
{
  private EncounterModel? _bossEncounter;

  /// <summary>
  /// 关键：RoomType 不为 Boss，让奖励界面不走换幕逻辑。
  /// </summary>
  public override RoomType RoomType => RoomType.Monster;

  /// <summary>
  /// 设置实际要打的 Boss 遭遇引用。
  /// </summary>
  public void SetBoss(EncounterModel boss)
  {
    _bossEncounter = boss;
  }

  /// <summary>
  /// 图鉴和资源预加载需要稳定的候选集合，不能依赖某一局临时抽中的 Boss。
  /// 实际战斗怪物仍由 <see cref="SetBoss"/> 选中的遭遇生成。
  /// </summary>
  public override IEnumerable<MonsterModel> AllPossibleMonsters =>
    ModelDb.Act<Overgrowth>().AllBossEncounters
      .Concat(ModelDb.Act<Underdocks>().AllBossEncounters)
      .SelectMany(encounter => encounter.AllPossibleMonsters)
      .DistinctBy(monster => monster.Id);

  protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
  {
    // MonstersWithSlots 里的怪物已经是 mutable 的（Boss 遭遇 GenerateMonsters 已调用 ToMutable）
    return _bossEncounter?.MonstersWithSlots
      .Select(m => (m.Item1, m.Item2))
      .ToList() ?? [];
  }
}
