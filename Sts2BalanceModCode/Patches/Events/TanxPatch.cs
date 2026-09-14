using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Extensions;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Relics;
using Sts2BalanceMod.Sts2BalanceModCode.Relics;
using Sts2BalanceMod.Sts2BalanceModCode.Settings;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// 先古之民 Tanx 事件 Patch：
/// 目标类型：MegaCrit.Sts2.Core.Models.Events.Tanx
/// 修改原因：当启用联动开关时，将先古遗物【精致的玩偶】（DelicateDoll）加入 Tanx 的可选遗物候选池，参与随机抽取
/// 依赖细节警告：依赖 Tanx.BaseOptionPool 与 Tanx.TriBoomerangOption 私有属性，以及 AncientEventModel.RelicOption 方法签名
/// </summary>
[HarmonyPatch(typeof(Tanx))]
public static class TanxPatch
{
  private static readonly MethodInfo? RelicOptionMethod = AccessTools.Method(
      typeof(AncientEventModel),
      "RelicOption",
      [typeof(RelicModel), typeof(string), typeof(string)]
  );

  private static readonly PropertyInfo? BasePoolProp = AccessTools.Property(typeof(Tanx), "BaseOptionPool");
  private static readonly PropertyInfo? TriOptionProp = AccessTools.Property(typeof(Tanx), "TriBoomerangOption");

  [HarmonyPatch("AllPossibleOptions", MethodType.Getter)]
  [HarmonyPostfix]
  public static void AllPossibleOptionsPostfix(Tanx __instance, ref IEnumerable<EventOption> __result)
  {
    if (!BalanceModSettings.DelicateDollEnabled)
      return;

    RelicModel? dollModel = ModelDb.Relic<DelicateDoll>()?.ToMutable();
    if (dollModel == null || RelicOptionMethod == null)
      return;

    if (RelicOptionMethod.Invoke(__instance, [dollModel, "INITIAL", null]) is EventOption dollOption)
    {
      __result = __result.Append(dollOption);
    }
  }

  [HarmonyPatch("GenerateInitialOptions")]
  [HarmonyPrefix]
  public static bool GenerateInitialOptionsPrefix(Tanx __instance, ref IReadOnlyList<EventOption> __result)
  {
    if (!BalanceModSettings.DelicateDollEnabled)
      return true;

    RelicModel? dollModel = ModelDb.Relic<DelicateDoll>()?.ToMutable();
    if (dollModel == null || RelicOptionMethod == null || BasePoolProp == null)
      return true;

    if (RelicOptionMethod.Invoke(__instance, [dollModel, "INITIAL", null]) is not EventOption dollOption)
      return true;

    if (BasePoolProp.GetValue(__instance) is not IEnumerable<EventOption> basePool)
      return true;

    var list = basePool.ToList();
    list.Add(dollOption);

    if (__instance.Owner != null && __instance.Owner.Deck.Cards.Count(c => ModelDb.Enchantment<Instinct>().CanEnchant(c)) >= 3)
    {
      if (TriOptionProp?.GetValue(__instance) is EventOption triOption)
      {
        list.Add(triOption);
      }
    }

    __result = list.UnstableShuffle(__instance.Rng).Take(3).ToList();
    return false;
  }
}
