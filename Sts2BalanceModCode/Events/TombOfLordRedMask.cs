using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events;

/// <summary>
/// STS1-EVENT-08 — 红面具大人之墓：已有红面具时获得金币，否则献上金币获得红面具。
/// 来源参考 ActsFromThePast.Acts.TheBeyond.Events.TombOfLordRedMask。
/// </summary>
public sealed class TombOfLordRedMask : CustomEventModel
{
  private const int GoldAmount = 222;

  public override ActModel[] Acts => [];

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new GoldVar(GoldAmount),
    new IntVar("PlayerGold", 0),
  ];

  public override void CalculateVars()
  {
    DynamicVars["PlayerGold"].BaseValue = Owner?.Gold ?? 0;
  }

  protected override IReadOnlyList<EventOption> GenerateInitialOptions()
  {
    var owner = Owner;
    if (owner == null)
      return [];

    var options = new List<EventOption>();
    if (owner.Relics.Any(r => r is RedMask))
    {
      options.Add(Option(WearMask));
    }
    else
    {
      options.Add(new EventOption(this, null,
        $"{Id.Entry}.pages.INITIAL.options.WEAR_MASK_LOCKED",
        Array.Empty<IHoverTip>()));
      // NOTE: 二代原生 RedMask 的 HoverTip 在 Mod 事件内会触发能量图标池解析异常，先只展示选项文本。
      options.Add(Option(PayRespects));
    }

    options.Add(Option(Leave));
    return options;
  }

  private async Task WearMask()
  {
    var owner = Owner;
    if (owner == null)
      return;

    await PlayerCmd.GainGold(GoldAmount, owner);
    SetEventFinished(PageDescription("WEAR_MASK"));
  }

  private async Task PayRespects()
  {
    var owner = Owner;
    if (owner == null)
      return;

    if (owner.Gold > 0)
      await PlayerCmd.LoseGold(owner.Gold, owner, GoldLossType.Spent);

    await RelicCmd.Obtain(ModelDb.Relic<RedMask>().ToMutable(), owner);
    SetEventFinished(PageDescription("PAY_RESPECTS"));
  }

  private Task Leave()
  {
    SetEventFinished(PageDescription("LEAVE"));
    return Task.CompletedTask;
  }
}
