using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Saves.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

/// <summary>
/// RELIC-NEOWS-TALISMAN-01 & RELIC-NEOWS-TALISMAN-02
/// 涅奥遗物「涅奥的护符」重做为快速开局遗物（类似 1 代涅奥的悲哀）：
/// 你遇到的接下来的 3 场战斗中，所有敌人的生命值只有 1 点。耗尽后置灰失效，且仅限单人模式出现与选用。
/// </summary>
[RegisterRelic(typeof(EventRelicPool), FullPublicEntry = "STS2_BALANCEMOD_NEOWS_TALISMAN")]
public sealed class NeowsTalisman : BalanceRelicTemplate
{
  private const string _combatsKey = "Combats";
  private int _combatsLeft = 3;

  public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
  public override RelicRarity Rarity => RelicRarity.Ancient;

  public override bool IsUsedUp => CombatsLeft <= 0;
  public override bool ShowCounter => !IsUsedUp;
  public override int DisplayAmount => Math.Max(0, CombatsLeft);

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new DynamicVar(_combatsKey, 3m),
  ];

  [SavedProperty]
  public int CombatsLeft
  {
    get => _combatsLeft;
    set
    {
      AssertMutable();
      _combatsLeft = value;
      DynamicVars[_combatsKey].BaseValue = _combatsLeft;
      InvokeDisplayAmountChanged();
      if (IsUsedUp)
      {
        Status = RelicStatus.Disabled;
      }
    }
  }

  public override bool IsAllowed(IRunState runState) => runState.Players.Count == 1;

  public override Task AfterRoomEntered(AbstractRoom room)
  {
    if (!IsUsedUp && room is CombatRoom combatRoom)
    {
      Flash();
      CombatsLeft--;
      foreach (Creature enemy in combatRoom.CombatState.Enemies)
      {
        enemy.SetMaxHpInternal(1m);
        enemy.SetCurrentHpInternal(1m);
      }
    }

    return Task.CompletedTask;
  }
}
