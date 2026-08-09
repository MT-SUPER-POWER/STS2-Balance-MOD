using STS2RitsuLib.Interop.AutoRegistration;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// 步步为营能力：每回合开始获得 1 点额外能量，抽 1 张额外卡牌。
/// 持续 Amount 回合后在回合结束时自动移除。
///
/// 设计参考：
/// - ClarityPower（多回合 +1 抽，AfterSideTurnStart 递减）
/// - EnergyNextTurnPower（AfterEnergyReset 加能量）
///
/// 生命周期（Amount=3）：
///   Turn 0: 挂上 power（SkipNextDurationTick=true，当回合不消耗）
///   Turn 1: +1 能量 / +1 抽 → 回合末 Amount=2
///   Turn 2: +1 能量 / +1 抽 → 回合末 Amount=1
///   Turn 3: +1 能量 / +1 抽 → 回合末 Amount=0 → 自动移除
/// </summary>
[RegisterPower]
public sealed class StepByStepPower() : BalancePowerTemplate(PowerType.Buff, PowerStackType.Counter)
{
  public override async Task AfterEnergyReset(Player player)
  {
    if (player != Owner.Player)
      return;

    await PlayerCmd.GainEnergy(1, player);
  }

  public override decimal ModifyHandDraw(Player player, decimal count)
  {
    if (player != Owner.Player)
      return count;

    return count + 1m;
  }

  public override async Task AfterSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
  {
    if (side != Owner.Side)
      return;

    await PowerCmd.TickDownDuration(this);
  }
}
