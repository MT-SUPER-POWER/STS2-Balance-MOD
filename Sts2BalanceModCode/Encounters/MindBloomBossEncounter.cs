using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Encounters;

/// <summary>
/// 心灵绽放专属遭遇 — RoomType 为 Monster，避免被系统当作 Boss 战触发换幕。
/// 怪物/槽位从实际抽选的一层 Boss 遭遇委托生成。
/// </summary>
[RegisterGlobalEncounter]
public sealed class MindBloomBossEncounter : BalanceEncounterTemplate
{
    private EncounterModel? _bossEncounter;

    /// <summary>
    /// 实际要打的 Boss 遭遇引用。
    /// </summary>
    public EncounterModel? BossEncounter => _bossEncounter;

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

    public override bool HasScene => _bossEncounter?.HasScene ?? false;

    public override IReadOnlyList<string> Slots => _bossEncounter?.Slots ?? [];

    public override float GetCameraScaling() => _bossEncounter?.GetCameraScaling() ?? 1f;

    public override Vector2 GetCameraOffset() => _bossEncounter?.GetCameraOffset() ?? Vector2.Zero;

    public override string CustomBgm => _bossEncounter?.CustomBgm ?? "";

    public override IEnumerable<string> ExtraAssetPaths => _bossEncounter?.ExtraAssetPaths ?? [];

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
