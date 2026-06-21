using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Monsters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Encounters;

/// <summary>
/// 心灵绽放专用的一层 Boss 战斗：六火亡魂。
/// 输入：仅由心灵绽放事件显式创建，不进入随机遭遇池。
/// 输出：生成六火亡魂战斗，并在事件补丁中按 Boss 房间处理。
/// </summary>
public sealed class MindBloomHexaghost : Sts2EncounterModel
{
  public override RoomType RoomType => RoomType.Monster;

  public override IEnumerable<MonsterModel> AllPossibleMonsters => [ModelDb.Monster<Hexaghost>()];

  protected override IReadOnlyList<(MonsterModel, string?)> GenerateMonsters() =>
  [
    (ModelDb.Monster<Hexaghost>().ToMutable(), null),
  ];
}
