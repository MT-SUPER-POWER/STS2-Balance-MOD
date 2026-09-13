using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// 改为 消耗 保留
/// </summary>
[HarmonyPatch(typeof(Wither), "get_CanonicalKeywords")]
public static class AgeonglassWitherKeywordsPatch
{
  [HarmonyPrefix]
  public static bool Prefix(ref IEnumerable<CardKeyword> __result)
  {
    __result = [CardKeyword.Exhaust];
    return false;
  }
}

/// <summary>
/// 从不可打出改为可用一费消耗
/// NOTE: 如何给构造函数没有费用的卡打补丁
/// </summary>
[HarmonyPatch(typeof(CardModel), "get_CanonicalEnergyCost")]
public static class AgeonglassWitherCostPatch
{
  [HarmonyPrefix]
  public static bool Prefix(CardModel __instance, ref int __result)
  {
    if (__instance is not Wither)
      return true;
    __result = 1;
    return false;
  }
}

/// <summary>
/// CARD-WITHER-01 — 凋萎（Wither）每两次升级打出消耗费用+1。
/// Target: MegaCrit.Sts2.Core.Models.Cards.Wither.FakeUpgrade.
/// Reason: 永世沙漏（Aeonglass）战斗中多次强化凋萎时，使玩家打出凋萎的代价随着强化次数动态增长。
/// WARNING: Verified against D:\Game\Sts2Code\src\MegaCrit.Sts2.Core.Models.Cards\Wither.cs; game updates may change this decompiled implementation.
/// </summary>
[HarmonyPatch(typeof(Wither), nameof(Wither.FakeUpgrade))]
public static class AgeonglassWitherFakeUpgradePatch
{
  // NOTE: 如何访问一个类内的私有变量
  private static readonly AccessTools.FieldRef<Wither, int> FakeUpgradeLevelRef =
      AccessTools.FieldRefAccess<Wither, int>("_fakeUpgradeLevel");

  [HarmonyPostfix]
  public static void Postfix(Wither __instance)
  {
    if (!__instance.IsMutable)
      return;

    int level = FakeUpgradeLevelRef(__instance);
    int newCost = 1 + (level / 2);
    __instance.EnergyCost.SetCustomBaseCost(newCost);
  }
}
