using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;
using Sts2BalanceMod.Sts2BalanceModCode.Settings;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// RELIC-BLUE-CANDLE-01 — 蓝蜡烛：确保原本不可打出的诅咒牌在手牌中被正确识别为可打出。
/// Target: MegaCrit.Sts2.Core.Models.CardModel.CanPlay(ref UnplayableReason, ref AbstractModel)
/// Reason: 当持有蓝蜡烛时，解除诅咒牌因 HasUnplayableKeyword 导致的阻断，允许玩家拖拽并打出。
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.CanPlay), [typeof(UnplayableReason).MakeByRefType(), typeof(AbstractModel).MakeByRefType()])]
[BalancePatch("R27")]
public static class BlueCandleCanPlayPatch
{
  [HarmonyPostfix]
  public static void Postfix(CardModel __instance, ref UnplayableReason reason, ref AbstractModel blocker, ref bool __result)
  {
    if (__result)
      return;

    if (__instance.Type != CardType.Curse)
      return;

    Player? owner = __instance.Owner;
    if (owner == null)
      return;

    if (owner.Relics.OfType<BlueCandle>().Any())
    {
      if (reason == UnplayableReason.HasUnplayableKeyword)
      {
        reason = UnplayableReason.None;
        blocker = null;
        __result = true;
      }
    }
  }
}

/// <summary>
/// RELIC-BLUE-CANDLE-01 — 蓝蜡烛：手牌可打出状态 UI 属性同步。
/// Target: MegaCrit.Sts2.Core.Models.CardModel.IsPlayable (getter)
/// Reason: 确保持有蓝蜡烛时，手牌中的诅咒牌正常显示可打出的视觉高亮。
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.IsPlayable), MethodType.Getter)]
[BalancePatch("R27")]
public static class BlueCandleIsPlayablePatch
{
  [HarmonyPostfix]
  public static void Postfix(CardModel __instance, ref bool __result)
  {
    if (__result)
      return;

    if (__instance.Type != CardType.Curse)
      return;

    Player? owner = __instance.Owner;
    if (owner != null && owner.Relics.OfType<BlueCandle>().Any())
    {
      __result = true;
    }
  }
}
