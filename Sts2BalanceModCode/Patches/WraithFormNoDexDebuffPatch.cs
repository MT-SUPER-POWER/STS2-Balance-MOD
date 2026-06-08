using HarmonyLib;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches;

/// <summary>
/// CARD-07 — 幽魂形态不再每回合扣敏捷
/// 跳过 WraithFormPower.AfterSideTurnStart 原文逻辑
/// </summary>
[HarmonyPatch(typeof(WraithFormPower), "AfterSideTurnStart")]
public static class WraithFormNoDexDebuffPatch
{
  [HarmonyPrefix]
  public static bool Prefix() => false;
}
