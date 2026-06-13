using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;


[HarmonyPatch(typeof(SturdyClamp), nameof(SturdyClamp.AfterPreventingBlockClear))]
public static class SturdyClampEnhancedPatch
{
  private const int RetainedBlock = 20;

  [HarmonyPrefix]
  public static bool Prefix(SturdyClamp __instance, AbstractModel preventer, Creature creature, ref Task __result)
  {
    // 是不是当前遗物触发的保留护甲效果
    if (__instance != preventer || creature != __instance.Owner.Creature) { return true; }

    int block = creature.Block;
    if (block != 0 && block > RetainedBlock)
    {
      // 替换原本逻辑的返回对象，执行我们的 Task 逻辑
      __result = CreatureCmd.LoseBlock(creature, block - RetainedBlock);
    }

    return false;
  }
}
