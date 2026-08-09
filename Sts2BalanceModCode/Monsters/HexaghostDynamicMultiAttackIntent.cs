using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// Hexaghost Divider 专用的动态多段攻击意图，在 ACTIVATE 结算后才读取伤害。
/// </summary>
internal sealed class HexaghostDynamicMultiAttackIntent : AttackIntent
{
    private readonly int _repeats;

    protected override LocString IntentLabelFormat => new("intents", "FORMAT_DAMAGE_MULTI");

    public override int Repeats => _repeats;

    public HexaghostDynamicMultiAttackIntent(Func<int> damage, int repeats)
    {
        _repeats = repeats;
        DamageCalc = () => damage();
    }

    public override int GetTotalDamage(IEnumerable<Creature> targets, Creature owner)
    {
        return GetSingleDamage(targets, owner) * Repeats;
    }

    public override LocString GetIntentLabel(IEnumerable<Creature> targets, Creature owner)
    {
        var label = IntentLabelFormat;
        label.Add("Damage", (decimal)GetSingleDamage(targets, owner));
        label.Add("Repeat", (decimal)Repeats);
        return label;
    }
}
