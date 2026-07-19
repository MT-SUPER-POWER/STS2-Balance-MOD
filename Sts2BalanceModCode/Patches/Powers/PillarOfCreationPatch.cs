using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Powers;

/// <summary>
/// CARD-02 — 创世之柱平衡调整
/// 1. 构造函数 Patch：修改 BaseBlock 5->3，UpgradeBlock 3->1 (即升级后 4)
/// 2. Power Patch：每生成一张卡牌都触发 3/4 护甲生成，而不是每回合仅限第一次
/// </summary>
[HarmonyPatch]
public static class PillarOfCreationPatch
{
  [HarmonyPatch(typeof(PillarOfCreation), MethodType.Constructor)]
  [HarmonyPostfix]
  public static void CardConstructorPostfix(PillarOfCreation __instance)
  {
    var traverse = Traverse.Create(__instance);
    traverse.Field("BaseBlock").SetValue(3M);
    traverse.Field("UpgradeBlock").SetValue(1M);

    if (__instance.DynamicVars != null && __instance.DynamicVars.ContainsKey("Block"))
    {
      __instance.DynamicVars["Block"].BaseValue = 3M;
    }
  }

  [HarmonyPatch(typeof(PillarOfCreationPower), nameof(PillarOfCreationPower.AfterCardGeneratedForCombat))]
  [HarmonyPrefix]
  public static bool PowerAfterCardGeneratedForCombatPrefix(
    PillarOfCreationPower __instance,
    CardModel card,
    Player creator,
    ref Task __result
  )
  {
    if (__instance.Owner == null || creator == null)
    {
      __result = Task.CompletedTask;
      return false;
    }

    if (creator != __instance.Owner.Player)
    {
      __result = Task.CompletedTask;
      return false;
    }

    Traverse.Create(__instance).Method("Flash").GetValue();
    __result = CreatureCmd.GainBlock(__instance.Owner, __instance.Amount, ValueProp.Unpowered, null);
    return false;
  }
}
