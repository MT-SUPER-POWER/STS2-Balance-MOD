using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// 辉光：改为当回合抽 2 张，不再下回合额外抽牌。
/// </summary>
[HarmonyPatch(typeof(Glow), "OnPlay")]
public static class GlowDrawCardPatch
{
  private const int DrawCount = 2;

  [HarmonyPrefix]
  public static bool Prefix(
      Glow __instance,
      PlayerChoiceContext choiceContext,
      CardPlay cardPlay,
      ref Task __result
  )
  {
    __result = OnPlay(__instance, choiceContext, cardPlay);

    return false;
  }

  private static async Task OnPlay(
    Glow __instance,
    PlayerChoiceContext choiceContext,
    CardPlay cardPlay
  )
  {
    await CreatureCmd.TriggerAnim(__instance.Owner.Creature, "Cast", __instance.Owner.Character.CastAnimDelay);
    await PlayerCmd.GainStars(__instance.DynamicVars.Stars.BaseValue, __instance.Owner);
    await CardPileCmd.Draw(choiceContext, DrawCount, __instance.Owner);
  }
}


[HarmonyPatch(typeof(Glow), "OnUpgrade")]
public static class GlowUpgradePatch
{
  [HarmonyPrefix]
  public static bool Prefix(Glow __instance)
  {
    __instance.DynamicVars.Stars.UpgradeValueBy(1m);
    return false;
  }
}
