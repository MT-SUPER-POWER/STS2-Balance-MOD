using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Interop.AutoRegistration;
using Sts2BalanceMod.src.Abstract;

namespace Sts2BalanceMod.src.Encounters;

/// <summary>
/// 心灵绽放专属遭遇 — RoomType 为 Monster，避免被系统当作 Boss 战触发换幕。
/// 怪物/槽位从实际抽选的一层 Boss 遭遇委托生成。
/// </summary>
[RegisterGlobalEncounter]
public sealed class MindBloomBossEncounter : BalanceEncounterTemplate
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

  public override IEnumerable<MonsterModel> AllPossibleMonsters =>
    _bossEncounter?.AllPossibleMonsters ?? [];

  protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters()
  {
    // MonstersWithSlots 里的怪物已经是 mutable 的（Boss 遭遇 GenerateMonsters 已调用 ToMutable）
    return _bossEncounter?.MonstersWithSlots
      .Select(m => (m.Item1, m.Item2))
      .ToList() ?? [];
  }
}
