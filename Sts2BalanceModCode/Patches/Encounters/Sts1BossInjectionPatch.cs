using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Encounters;

/// <summary>
/// STS1-BOSS-01/02 — 将一代 Boss 注入现有 Act 的 Boss 候选池。
/// </summary>
public static class Sts1BossInjectionPatch
{
  [HarmonyPatch(typeof(Hive), nameof(Hive.GenerateAllEncounters))]
  public static class HivePatch
  {
    [HarmonyPostfix]
    private static void Postfix(ref IEnumerable<EncounterModel> __result)
    {
      __result = AppendIfMissing(__result, ModelDb.Encounter<CollectorBoss>());
    }
  }

  [HarmonyPatch(typeof(Glory), nameof(Glory.GenerateAllEncounters))]
  public static class GloryPatch
  {
    [HarmonyPostfix]
    private static void Postfix(ref IEnumerable<EncounterModel> __result)
    {
      __result = AppendIfMissing(__result, ModelDb.Encounter<TimeEaterBoss>());
    }
  }

  private static IEnumerable<EncounterModel> AppendIfMissing(IEnumerable<EncounterModel> encounters, EncounterModel encounter)
  {
    var list = encounters.ToList();
    if (list.All(e => e.Id != encounter.Id))
      list.Add(encounter);

    return list;
  }
}
