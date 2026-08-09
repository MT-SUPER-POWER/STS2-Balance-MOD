using STS2RitsuLib.Interop.AutoRegistration;
using HarmonyLib;
using System.Reflection;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Entities.RestSite;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;


/// <summary>
/// RELIC-07 — 融合之锤：你不再能够锻造，但是每回合多加一点费用
/// </summary>
[RegisterRelic(typeof(EventRelicPool), FullPublicEntry = "STS2_BALANCEMOD_FUSION_HAMMER")]
public sealed class FusionHammer : BalanceRelicTemplate
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

// ===== 代价: 篝火不能敲牌 =====
[HarmonyPatch(typeof(SmithRestSiteOption), nameof(SmithRestSiteOption.IsEnabled), MethodType.Getter)]
internal static class FusionHammerRestPatch
{
  [HarmonyPostfix]
  private static void Postfix(ref bool __result, SmithRestSiteOption __instance)
  {
    // 只对 Smith（锻造）选项生效，不影响其他选项
    if (!__result || __instance is not SmithRestSiteOption)
      return;

    // Owner 是 protected 属性，走反射拿
    var ownerProp = typeof(RestSiteOption).GetProperty("Owner", BindingFlags.Instance | BindingFlags.NonPublic);
    var player = (Player?)ownerProp?.GetValue(__instance);

    if (player?.GetRelic<FusionHammer>() != null)
      __result = false;  // 灰色不可点击
  }
}
