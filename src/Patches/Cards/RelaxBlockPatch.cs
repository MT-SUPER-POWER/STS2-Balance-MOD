using HarmonyLib;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sts2BalanceMod.src.Patches.Cards;

/// <summary>
/// Relax（放松）—— 撤销增强补丁（保持官方原版格挡 16→18）
/// </summary>
// [HarmonyPatch(typeof(Relax), "get_CanonicalVars")]
public static class RelaxBlockPatch
{
    // 已根据官方最新版本撤销 Mod 增强，保留原版效果。
    /*
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
    */
}

