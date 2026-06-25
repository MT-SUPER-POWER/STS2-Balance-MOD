using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Gold;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Rewards;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events;

// ======================== 红面具强盗事件 ========================

/// <summary>
/// STS1-EVENT — 红面具三人帮：面对强盗团伙，交钱或开战。
/// 参考 ActsFromThePast.Acts.TheCity.Events.MaskedBandits 移植。
/// </summary>
public sealed class MaskedBandits : CustomEventModel
{
  private static readonly LocString EmptyDescription =
    new("events", "STS2BALANCEMOD-MASKED_BANDITS.pages.EMPTY.description");

  private NSpeechBubbleVfx? _speechBubble;

  public override ActModel[] Acts => [ModelDb.Act<Hive>()];

  public override bool IsShared => true;

  public override EventLayoutType LayoutType => EventLayoutType.Combat;

  public override EncounterModel CanonicalEncounter => ModelDb.Encounter<RedMaskBandits>();

  /// <summary>
  /// 第 3 层（城市）且未持有红面具时触发
  /// </summary>
  public override bool IsAllowed(IRunState runState)
  {
    if (runState.CurrentActIndex != 1) return false;         // NOTE: 第 2 幕（索引 1）
    if (runState.TotalFloor < 23) return false;              // NOTE: 至少第 23 层
    return !runState.Players.Any(p => p.Relics.Any(r => r is RedMask));
  }

  protected override IReadOnlyList<EventOption> GenerateInitialOptions()
  {
    return
    [
      Option(Pay),
      Option(Fight),
    ];
  }

  private async Task Pay()
  {
    var owner = Owner;
    if (owner == null) return;

    var goldToLose = owner.Gold;
    if (goldToLose > 0)
      await PlayerCmd.LoseGold(goldToLose, owner, GoldLossType.Stolen);

    SetEventState(EmptyDescription,
    [
      new EventOption(this, Paid2, $"{Id.Entry}.pages.PAID_1.options.CONTINUE", []),
    ]);
    PlayPaidLine<Pointy>("PAID_1");
  }

  private Task Paid2()
  {
    SetEventState(EmptyDescription,
    [
      new EventOption(this, Paid3, $"{Id.Entry}.pages.PAID_2.options.CONTINUE", []),
    ]);
    PlayPaidLine<Romeo>("PAID_2");
    return Task.CompletedTask;
  }

  private Task Paid3()
  {
    PlayPaidLine<Romeo>("PAID_3");
    SetEventFinished(EmptyDescription);
    return Task.CompletedTask;
  }

  private void PlayPaidLine<TMonster>(string pageKey) where TMonster : MonsterModel
  {
    if (!LocalContext.IsMe(Owner)) return;

    if (_speechBubble != null)
    {
      _ = _speechBubble.AnimOut();
      _speechBubble = null;
    }

    var speaker = FindCreature<TMonster>();
    if (speaker == null) return;

    _speechBubble = TalkCmd.Play(PageDescription(pageKey), speaker, VfxColor.Red, VfxDuration.Forever);
  }

  private static Creature? FindCreature<TMonster>() where TMonster : MonsterModel
  {
    return NCombatRoom.Instance?.CreatureNodes
      .FirstOrDefault(n => n.Entity.Monster is TMonster)
      ?.Entity;
  }

  private Task Fight()
  {
    var owner = Owner;
    if (owner == null) return Task.CompletedTask;

    var redMaskRelic = ModelDb.Relic<RedMask>().ToMutable();
    var rewards = new List<Reward>
    {
      new GoldReward(25, 35, owner),
      new RelicReward(redMaskRelic, owner),
    };
    EnterCombatWithoutExitingEvent<RedMaskBandits>(rewards, false);
    return Task.CompletedTask;
  }
}
