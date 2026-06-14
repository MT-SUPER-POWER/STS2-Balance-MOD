using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;
using System.Linq;
using System.Threading.Tasks;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

// ======================== RELIC-07 诅咒钥匙 Harmony 补丁 ========================

/// <summary>
/// 在 RewardsCmd.GenerateForRoomEnd 被调用时（即宝箱打开/奖励生成的瞬间），
/// 若玩家持有 CurseKey 且当前为宝箱房或 Boss 房，则获得一张随机诅咒牌。
/// </summary>
[HarmonyPatch(typeof(RewardsCmd), nameof(RewardsCmd.GenerateForRoomEnd))]
public static class CurseKeyPatch
{
  [HarmonyPostfix]
  static void Postfix(Player player, AbstractRoom room)
  {
    // NOTE: 仅宝箱房（TreasureRoom）和 Boss 房触发，普通战斗/事件/商店等不触发
    bool isChestRoom = room is TreasureRoom || room.RoomType == RoomType.Boss;
    if (!isChestRoom) return;

    // 异步逻辑通过 RunSafely 执行，避免阻塞原调用链
    TaskHelper.RunSafely(AddRandomCurse(player));
  }

  /// <summary>
  /// 从诅咒池中随机选择一张诅咒牌加入牌组
  /// </summary>
  private static async Task AddRandomCurse(Player player)
  {
    var curseKey = player.GetRelic<CurseKey>();
    if (curseKey == null) return;

    // NOTE: 参考 SereTalon / CursedRun 的诅咒生成逻辑
    var availableCurses = ModelDb.CardPool<CurseCardPool>()
        .GetUnlockedCards(player.UnlockState, player.RunState.CardMultiplayerConstraint)
        .Where(c => c.CanBeGeneratedByModifiers)
        .ToList();

    if (availableCurses.Count == 0) return;

    var canonicalCurse = player.RunState.Rng.Niche.NextItem(availableCurses);
    if (canonicalCurse == null) return;

    var curseCard = player.RunState.CreateCard(canonicalCurse, player);

    curseKey.Flash();
    await Cmd.Wait(0.5f);

    var result = await CardPileCmd.Add(curseCard, PileType.Deck);
    CardCmd.PreviewCardPileAdd(result, 2f);
  }
}
