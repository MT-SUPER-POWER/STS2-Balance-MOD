using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// Relax（放松）—— 格挡值 15→18（升级后 17→20）
/// </summary>
[HarmonyPatch(typeof(Relax), "get_CanonicalVars")]
public static class RelaxBlockPatch
{
  [HarmonyPrefix]
  public static bool Prefix(ref IEnumerable<DynamicVar> __result)
  {
    __result = new DynamicVar[3]
    {
        new BlockVar(18m, ValueProp.Move),
        new CardsVar(2),
        new EnergyVar(2)
    };
    return false;
  }
}
