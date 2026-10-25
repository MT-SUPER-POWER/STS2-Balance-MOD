using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events;

/// <summary>
/// STS1-EVENT-08 — 红面具大人之墓：
/// 仅在第 3 幕且持有「红面具」时出现。在红面具强盗团的墓前缅怀昔日的三位强盗伙伴：
/// 1. 罗密欧（Romeo）：获得 222 金币。
/// 2. 尖刺（Pointy）：获得事件遗物「尖刺的双刺」。
/// 3. 巨熊（Bear）：获得事件遗物「巨熊的皮甲」。
/// 4. 离开。
/// </summary>
[RegisterSharedEvent]
public sealed class TombOfLordRedMask : BalanceEventTemplate
{
  public override bool IsShared => false;
  private const int GoldAmount = 222;

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new GoldVar(GoldAmount),
  ];

  public override bool IsAllowed(IRunState runState)
  {
    if (runState.CurrentActIndex != 2)
      return false; // NOTE: 第 3 幕（索引 2）
    return runState.Players.Any(p => p.Relics.Any(r => r is RedMask));
  }

  protected override IReadOnlyList<EventOption> GenerateInitialOptions()
  {
    Player? owner = Owner;
    if (owner == null)
      return [];

    var options = new List<EventOption>();

    if (owner.Relics.Any(r => r is RedMask))
    {
      options.Add(Option(TributeRomeo));
      options.Add(Option(TributePointy, "INITIAL", ModelDb.Relic<PointysTwinBlades>().HoverTips));
      options.Add(Option(TributeBear, "INITIAL", ModelDb.Relic<BearsPelt>().HoverTips));
    }
    else
    {
      options.Add(new EventOption(this, null,
        $"{Id.Entry}.pages.INITIAL.options.TRIBUTE_ROMEO_LOCKED",
        []));
    }

    options.Add(Option(Leave));
    return options;
  }

  private async Task TributeRomeo()
  {
    Player? owner = Owner;
    if (owner == null)
      return;

    await PlayerCmd.GainGold(GoldAmount, owner);
    SetEventFinished(PageDescription("TRIBUTE_ROMEO"));
  }

  private async Task TributePointy()
  {
    Player? owner = Owner;
    if (owner == null)
      return;

    await RelicCmd.Obtain(ModelDb.Relic<PointysTwinBlades>().ToMutable(), owner);
    SetEventFinished(PageDescription("TRIBUTE_POINTY"));
  }

  private async Task TributeBear()
  {
    Player? owner = Owner;
    if (owner == null)
      return;

    await RelicCmd.Obtain(ModelDb.Relic<BearsPelt>().ToMutable(), owner);
    SetEventFinished(PageDescription("TRIBUTE_BEAR"));
  }

  private Task Leave()
  {
    SetEventFinished(PageDescription("LEAVE"));
    return Task.CompletedTask;
  }
}
