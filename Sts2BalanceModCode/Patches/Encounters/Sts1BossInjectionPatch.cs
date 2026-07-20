/* using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Acts;
using MegaCrit.Sts2.Core.Models.Encounters;
using Sts2BalanceMod.Sts2BalanceModCode.Encounters;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Encounters;

/// <summary>
/// STS1-BOSS-01 - 将三层永世沙漏 Boss 替换为时间吞噬者。
/// 输入：Glory 生成的遭遇池与 Boss 发现顺序。
/// 输出：保持三层 Boss 数量不变，仅把 AeonglassBoss 替换为 TimeEaterBoss。
/// </summary>
public static class Sts1BossInjectionPatch
{
  [HarmonyPatch(typeof(Glory), nameof(Glory.GenerateAllEncounters))]
  public static class GloryEncountersPatch
  {
    [HarmonyPostfix]
    private static void Postfix(ref IEnumerable<EncounterModel> __result)
    {
      __result = ReplaceAeonglassWithTimeEater(__result);
    }
  }

  [HarmonyPatch(typeof(Glory), nameof(Glory.BossDiscoveryOrder), MethodType.Getter)]
  public static class GloryBossDiscoveryOrderPatch
  {
    [HarmonyPostfix]
    private static void Postfix(ref IEnumerable<EncounterModel> __result)
    {
      __result = ReplaceAeonglassWithTimeEater(__result);
    }
  }

  private static IEnumerable<EncounterModel> ReplaceAeonglassWithTimeEater(IEnumerable<EncounterModel> encounters)
  {
    var timeEater = ModelDb.Encounter<TimeEaterBoss>();
    return encounters.Select(encounter => encounter is AeonglassBoss ? timeEater : encounter).ToList();
  }
}
*/
