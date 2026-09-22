using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Sts2BalanceMod.Sts2BalanceModCode.Cards;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;
using Sts2BalanceMod.Sts2BalanceModCode.Settings;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

/// <summary>
/// 当禁用方舟联动设置时，从 SharedRelicPool 过滤【精致的玩偶】，并从 EventCardPool 过滤【女巫形态】及巫术衍生卡。
/// </summary>
[HarmonyPatch(typeof(SharedRelicPool), "GenerateAllRelics")]
public static class SharedPoolDelicateDollFilterPatch
{
  [HarmonyPostfix]
  public static IEnumerable<RelicModel> Postfix(IEnumerable<RelicModel> __result)
  {
    if (!BalanceModSettings.DelicateDollEnabled)
    {
      return __result.Where(r => r is not DelicateDoll);
    }
    return __result;
  }
}

[HarmonyPatch(typeof(EventCardPool), "GenerateAllCards")]
public static class EventCardPoolWitchFormFilterPatch
{
  [HarmonyPostfix]
  public static CardModel[] Postfix(CardModel[] __result)
  {
    if (!BalanceModSettings.DelicateDollEnabled)
    {
      return [.. __result.Where(c => c is not WitchForm && c is not SorceryStrike && c is not SorceryDefend)];
    }
    return __result;
  }
}
