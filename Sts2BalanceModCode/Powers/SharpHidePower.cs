using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// AFP-BOSS-01 - 守护者防御形态的尖锐外壳。
/// 玩家打出攻击牌后受到等同于能力层数的无强化伤害。
/// </summary>
[RegisterPower]
public sealed class SharpHidePower() : BalancePowerTemplate(PowerType.Buff, PowerStackType.Counter)
{

    public bool AttackInProgress { get; private set; }

    public Creature? AttackSource { get; private set; }

    public override Task BeforeCardPlayed(CardPlay cardPlay)
    {
        if (cardPlay.Card.Type == CardType.Attack)
        {
            AttackInProgress = true;
            AttackSource = cardPlay.Card.Owner?.Creature;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        AttackInProgress = false;
        AttackSource = null;

        if (cardPlay.Card.Type != CardType.Attack)
            return;

        Flash();
        var player = cardPlay.Card.Owner?.Creature;
        if (player is { IsAlive: true })
        {
            await CreatureCmd.Damage(
              choiceContext,
              player,
              Amount,
              ValueProp.Unpowered,
              null,
              null);
        }
    }
}
