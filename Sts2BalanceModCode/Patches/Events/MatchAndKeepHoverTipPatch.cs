using HarmonyLib;
using MegaCrit.Sts2.Core.Nodes.Cards.Holders;
using Sts2BalanceMod.Sts2BalanceModCode.Minigames;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// 目标类型：MegaCrit.Sts2.Core.Nodes.Cards.Holders.NCardHolder
/// 目标方法：CreateHoverTips
/// 修改原因：在对对碰（Match and Keep）事件小游戏中，暗牌（未翻开的背面朝上卡牌）在鼠标悬停或手柄聚焦时不应显示关键词/卡片说明，
///           避免玩家通过悬停提示提前得知暗牌内容破坏翻牌记忆玩法。
/// 依赖反编译细节警告：依赖 NCardHolder.CreateHoverTips() 作为创建卡牌 HoverTips 的入口；若底层版本调整或由其他组件接管悬浮提示，需同步跟进。
/// </summary>
[HarmonyPatch(typeof(NCardHolder), "CreateHoverTips")]
public static class MatchAndKeepHoverTipPatch
{
  [HarmonyPrefix]
  public static bool Prefix(NCardHolder __instance)
  {
    if (NMatchAndKeepScreen.ShouldSuppressHoverTips(__instance))
    {
      return false;
    }
    return true;
  }
}
