using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Encounters;

/// <summary>
/// AFP-BOSS-01 — 心灵绽放专用守护者遭遇。
/// 目标类型：Guardian；仅由事件或开发调试显式调用，不注册到普通地图池。
/// RoomType 保持 Monster，避免胜利后触发 Boss 换幕流程。
/// </summary>
public sealed class MindBloomGuardian : Sts2EncounterModel
{
  private const string ModeShiftIcon =
    "res://Sts2BalanceMod/images/powers/actsfromthepast-mode_shift_power.png";
  private const string SharpHideIcon =
    "res://Sts2BalanceMod/images/powers/actsfromthepast-sharp_hide_power.png";

  public override RoomType RoomType => RoomType.Monster;

  public override IEnumerable<string> ExtraAssetPaths => [ModeShiftIcon, SharpHideIcon];

  public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Guardian>()];

  protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
  [
    (ModelDb.Monster<Guardian>().ToMutable(), null),
  ];
}
