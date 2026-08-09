using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// 抽牌减少能力。
/// 输入：拥有者下一次抽牌数量与所在阵营回合结束事件。
/// 输出：减少 1 张抽牌，并在拥有者阵营回合结束时递减持续时间。
/// </summary>
[RegisterPower]
public sealed class DrawReductionPower() : BalancePowerTemplate(PowerType.Debuff, PowerStackType.Counter)
{
    private const decimal DrawPenalty = 1M;


    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != Owner.Player)
        {
            return count;
        }

        Flash();
        return count - DrawPenalty;
    }

    public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
    {
        if (side != Owner.Side)
        {
            return;
        }

        await PowerCmd.TickDownDuration(this);
    }
}
