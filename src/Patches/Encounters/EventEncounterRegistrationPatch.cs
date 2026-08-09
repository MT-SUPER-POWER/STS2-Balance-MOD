using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using Sts2BalanceMod.src.Encounters;

namespace Sts2BalanceMod.src.Patches.Encounters;

/// <summary>
/// 目标：<see cref="ModelDb.EventEncounters"/> getter。
/// 原因：仅由 MOD 事件触发的遭遇应出现在怪物图鉴的「事件」分组，而不是任何幕的普通遭遇池。
/// WARNING：依赖反编译源码中 NBestiary.AddEvents 直接枚举 ModelDb.EventEncounters 的实现细节；游戏更新后需复核。
/// </summary>
[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.EventEncounters), MethodType.Getter)]
internal static class EventEncounterRegistrationPatch
{
  [HarmonyPostfix]
  private static void Postfix(ref IEnumerable<EncounterModel> __result)
  {
    var eventEncounters = __result.ToList();
    var registeredIds = eventEncounters.Select(encounter => encounter.Id).ToHashSet();

    EncounterModel[] modEventEncounters =
    [
      ModelDb.Encounter<RedMaskBandits>(),
      ModelDb.Encounter<MindBloomGuardian>(),
      ModelDb.Encounter<MindBloomHexaghost>(),
      ModelDb.Encounter<MindBloomSlimeBoss>(),
    ];

    foreach (var encounter in modEventEncounters)
    {
      if (registeredIds.Add(encounter.Id))
      {
        eventEncounters.Add(encounter);
      }
    }

    __result = eventEncounters;
  }
}
