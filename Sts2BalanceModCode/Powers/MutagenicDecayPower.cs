using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Powers;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// 突变衰退（Mutagenic Decay）Debuff。
/// 机制：拥有者在己方回合结束时，失去 1 点力量，并将本 Debuff 层数递减 1；归零时自动移除。
/// 关联：由【突变之力】（MutagenicStrength）遗物在战斗开始时赋予。
/// 作为 Debuff，可正常被人工制品（Artifact）抵挡。
/// </summary>
[RegisterPower]
public sealed class MutagenicDecayPower() : BalancePowerTemplate(PowerType.Debuff, PowerStackType.Counter)
{
  private const decimal _strengthLossPerTurn = 1M;

  public override async Task AfterSideTurnEnd(
      PlayerChoiceContext choiceContext,
      CombatSide side,
      IEnumerable<Creature> participants)
  {
    if (side != Owner.Side || !participants.Contains(Owner) || Amount <= 0)
    {
      return;
    }

    Flash();
    await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, -_strengthLossPerTurn, Owner, null);
    await PowerCmd.Decrement(this);
  }
}
