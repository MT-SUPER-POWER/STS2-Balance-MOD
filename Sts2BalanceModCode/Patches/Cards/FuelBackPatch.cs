using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// 改回抽2加1费
/// </summary>
[HarmonyPatch(typeof(Fuel), "OnPlay")]
public static class FuelDrawPatch
{

    [HarmonyPrefix]
    public static bool Prefix(Fuel __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay, ref Task __result)
    {
        __result = OnPlay(__instance, choiceContext, cardPlay);

        return false;
    }

    private static async Task OnPlay(Fuel __instance, PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(__instance.DynamicVars.Energy.BaseValue, __instance.Owner);
        await CardPileCmd.Draw(choiceContext, __instance.DynamicVars.Cards.BaseValue, __instance.Owner);
    }
}


[HarmonyPatch(typeof(Fuel), "get_CanonicalVars")]
public static class FuelGetVarPatch
{
    [HarmonyPrefix]
    public static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result = new DynamicVar[]
        {
      new EnergyVar(1),
      new CardsVar(1)
        };
        return false;
    }
}

[HarmonyPatch(typeof(Fuel), "OnUpgrade")]
public static class FuelUpgradePatch
{
    [HarmonyPrefix]
    public static bool Prefix(Fuel __instance)
    {
        __instance.DynamicVars.Cards.UpgradeValueBy(1m);
        return false;
    }
}
