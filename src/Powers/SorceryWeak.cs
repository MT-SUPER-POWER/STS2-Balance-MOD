using STS2RitsuLib.Interop.AutoRegistration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.src.Abstract;

namespace Sts2BalanceMod.src.Powers;

/// <summary>
/// 巫术虚弱 - 造成的攻击伤害减少 50%。若本回合进行过攻击，回合结束时减少 1 层。
/// </summary>
[RegisterPower]
public sealed class SorceryWeak() : BalancePowerTemplate(PowerType.Debuff, PowerStackType.Counter)
{
    private bool _didAttackThisTurn;

    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (dealer != Owner || !props.IsPoweredAttack())
        {
            return 1m;
        }
        return 0.5m;
    }

    public override Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (command.Attacker == Owner && command.DamageProps.IsPoweredAttack())
        {
            _didAttackThisTurn = true;
        }

        return Task.CompletedTask;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side == Owner.Side)
        {
            if (_didAttackThisTurn)
            {
                _didAttackThisTurn = false;
                Flash();
                await PowerCmd.TickDownDuration(this);
            }
        }
    }
}
