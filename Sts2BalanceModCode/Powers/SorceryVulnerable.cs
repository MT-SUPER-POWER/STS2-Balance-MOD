using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// 巫术易伤 - 受到的攻击伤害增加 75%。受到攻击后减少 1 层。
/// </summary>
public sealed class SorceryVulnerable() : Sts2PowerModel(PowerType.Debuff, PowerStackType.Counter)
{
    public override decimal ModifyDamageMultiplicative(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
    {
        if (target != Owner || !props.IsPoweredAttack())
        {
            return 1m;
        }
        return 1.75m;
    }

    public override async Task AfterAttack(PlayerChoiceContext choiceContext, AttackCommand command)
    {
        if (!command.DamageProps.IsPoweredAttack())
            return;

        bool hitOwner = command.Results
            .SelectMany(r => r)
            .Any(r => r.Receiver == Owner);

        if (hitOwner)
        {
            Flash();
            await PowerCmd.TickDownDuration(this);
        }
    }
}
