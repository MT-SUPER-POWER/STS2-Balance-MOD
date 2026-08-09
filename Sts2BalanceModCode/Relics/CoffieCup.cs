using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;


/// <summary>
/// RELIC-07 — 咖啡杯：在火堆无法休息，但是每回合多加一点费用
/// </summary>
[RegisterRelic(typeof(EventRelicPool), FullPublicEntry = "STS2_BALANCEMOD_COFFIE_CUP")]
public sealed class CoffieCup : BalanceRelicTemplate
{
    public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
    public override RelicRarity Rarity => RelicRarity.Ancient;

    protected override IEnumerable<DynamicVar> CanonicalVars => [
      new EnergyVar(1),
  ];

    public override decimal ModifyMaxEnergy(Player player, decimal amount)
    {
        if (player != base.Owner)
        {
            return amount;
        }
        return amount + base.DynamicVars.Energy.IntValue;
    }
}

// ===== 代价: 篝火不能休息 =====
[HarmonyPatch(typeof(RestSiteOption), nameof(RestSiteOption.IsEnabled), MethodType.Getter)]
internal static class CoffeeCupRestPatch
{
    [HarmonyPostfix]
    private static void Postfix(ref bool __result, RestSiteOption __instance)
    {
        // 只对 Heal（回血）选项生效，不影响 Smith 等
        if (!__result || __instance is not HealRestSiteOption)
            return;

        // Owner 是 protected 属性，走反射拿
        var ownerProp = typeof(RestSiteOption).GetProperty("Owner", BindingFlags.Instance | BindingFlags.NonPublic);
        var player = (Player?)ownerProp?.GetValue(__instance);

        if (player?.GetRelic<CoffieCup>() != null)
            __result = false;  // 灰色不可点击
    }
}
