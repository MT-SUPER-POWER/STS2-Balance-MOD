using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

// ======================== RELIC-07 诅咒钥匙 ========================

/// <summary>
/// 在原版流程中，玩家在宝箱房选择了遗物之后（<c>NProceedButton.UpdateText(ProceedLoc)</c> 被调用时），
/// 若持有诅咒钥匙，则获得一张随机诅咒牌。
///
/// 不再拦截 <c>RewardsCmd.GenerateForRoomEnd</c>（开箱即生诅咒），
/// 改为选完遗物后生成，让诅咒成为"取遗物的代价"，体验更合理。
/// </summary>
[HarmonyPatch(typeof(NProceedButton), "UpdateText")]
public static class CurseKeyPatch
{
  [HarmonyPostfix]
  static void Postfix(NProceedButton __instance, LocString loc)
  {
    // 仅当文字被设为 ProceedLoc（= OpenChest 结尾，遗物已选）
    if (loc.LocEntryKey != NProceedButton.ProceedLoc.LocEntryKey)
      return;

    // 不是在开箱流程中 → 过滤掉 _Ready 中的 UpdateText(ProceedLoc)
    if (!TreasureRoomSkipPatch.IsAfterChestOpen())
      return;

    // 如果是 mod 跳过宝箱 → 不生成诅咒
    if (TreasureRoomSkipPatch.SkipChestForCurseKey)
      return;

    var runState = typeof(RunManager)
        .GetProperty("State", BindingFlags.NonPublic | BindingFlags.Instance)
        ?.GetValue(RunManager.Instance) as IRunState;

    if (runState == null || runState.Players.Count != 1)
      return;

    var player = runState.Players[0];
    if (player.GetRelic<CurseKey>() == null)
      return;

    TaskHelper.RunSafely(AddRandomCurse(player));
  }

  private static async Task AddRandomCurse(Player player)
  {
    var curseKey = player.GetRelic<CurseKey>();
    if (curseKey == null) return;

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
