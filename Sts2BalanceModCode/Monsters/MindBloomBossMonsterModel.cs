using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Powers;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

internal enum MindBloomDurabilityEnhancement
{
    Giant,
    Plating,
    Regeneration,
}

internal enum MindBloomThreatEnhancement
{
    Strength,
    Ritual,
}

/// <summary>
/// 心灵绽放第二战在玩家确认继续后生成的 Boss 强化组合。
/// 两个枚举分别来自耐久池和威胁池，计划只写入遭遇开场生成的 Boss 本体。
/// </summary>
internal sealed record MindBloomBossEnhancementPlan(
  MindBloomDurabilityEnhancement Durability,
  MindBloomThreatEnhancement Threat);

/// <summary>
/// 仅供心灵绽放第二战使用的 Boss 基类，集中承载随机强化计划与应用规则。
/// 派生 Boss 仍负责自己的行动循环、形态转换和分裂机制。
/// </summary>
public abstract class MindBloomBossMonsterModel : BalanceMonsterTemplate
{
    private MindBloomBossEnhancementPlan? _mindBloomEnhancementPlan;

    internal MindBloomBossEnhancementPlan? MindBloomEnhancementPlan
    {
        get => _mindBloomEnhancementPlan;
        set
        {
            AssertMutable();
            _mindBloomEnhancementPlan = value;
        }
    }

    /// <summary>
    /// 在原有入场逻辑完成后应用强化。此时怪物生命已经完成多人缩放。
    /// </summary>
    protected async Task ApplyMindBloomEnhancements()
    {
        MindBloomBossEnhancementPlan? plan = _mindBloomEnhancementPlan;
        if (plan == null)
            return;

        switch (plan.Durability)
        {
            case MindBloomDurabilityEnhancement.Giant:
                await CreatureCmd.GainMaxHp(Creature, Creature.MaxHp * 0.25M);
                break;
            case MindBloomDurabilityEnhancement.Plating:
                await PowerCmd.Apply<PlatingPower>(
                  new ThrowingPlayerChoiceContext(), Creature, 10M, Creature, null);
                break;
            case MindBloomDurabilityEnhancement.Regeneration:
                await PowerCmd.Apply<RegenPower>(
                  new ThrowingPlayerChoiceContext(), Creature, 10M, Creature, null);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(plan.Durability));
        }

        switch (plan.Threat)
        {
            case MindBloomThreatEnhancement.Strength:
                await PowerCmd.Apply<StrengthPower>(
                  new ThrowingPlayerChoiceContext(), Creature, GetStrengthAmount(), Creature, null);
                break;
            case MindBloomThreatEnhancement.Ritual:
                await PowerCmd.Apply<RitualPower>(
                  new ThrowingPlayerChoiceContext(), Creature, 1M, Creature, null);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(plan.Threat));
        }
    }

    private int GetStrengthAmount() => this switch
    {
        Guardian => 2,
        Hexaghost => 1,
        SlimeBoss => 3,
        _ => throw new InvalidOperationException(
          $"Unsupported Mind Bloom boss enhancement target: {GetType().Name}"),
    };
}
