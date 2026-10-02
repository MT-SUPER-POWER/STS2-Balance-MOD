using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

// ======================== RELIC-INSERTER-01: 机械臂 ========================

/// <summary>
/// RELIC-INSERTER-01 — 机械臂（Inserter）：每 2 回合，获得 1 个充能球栏位。
/// 故障机器人（Defect）专属遗物，稀有度：稀有（Rare）。
/// Target: RelicModel.AfterSideTurnStart
/// Reason: 为集中流提供长线球位再生，与耗尽（Consume）和暴涨（Bulk Up）形成资源闭环。
/// WARNING: Verified against D:\Game\Sts2Code\src\MegaCrit.Sts2.Core.Models.Relics\HappyFlower.cs;
/// follows the same TurnsSeen counter pattern.
/// </summary>
[RegisterRelic(typeof(DefectRelicPool), FullPublicEntry = "STS2_BALANCEMOD_INSERTER")]
public sealed class Inserter : BalanceRelicTemplate
{
  private const int _turnsRequired = 2;

  public override RelicRarity Rarity => RelicRarity.Rare;

  public override bool ShowCounter => true;

  public override int DisplayAmount => TurnsSeen + 1;

  [SavedProperty]
  public int TurnsSeen
  {
    get => _turnsSeen;
    set
    {
      AssertMutable();
      _turnsSeen = value;
      InvokeDisplayAmountChanged();
    }
  }

  private int _turnsSeen;

  public override async Task AfterSideTurnStart(CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
  {
    if (participants.Contains(Owner.Creature))
    {
      TurnsSeen++;
      if (TurnsSeen >= _turnsRequired)
      {
        TurnsSeen = 0;
        Flash();
        await OrbCmd.AddSlots(Owner, 1);
      }
      else
      {
        Status = RelicStatus.Active; // Light up on the turn before triggering
      }
    }
  }

  public override Task AfterCombatEnd(CombatRoom _)
  {
    TurnsSeen = 0;
    Status = RelicStatus.Normal;
    return Task.CompletedTask;
  }
}
