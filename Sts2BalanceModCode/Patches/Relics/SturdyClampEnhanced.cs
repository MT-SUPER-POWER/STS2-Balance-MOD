using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.ValueProps;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Relics;

[HarmonyPatch(typeof(SturdyClamp), nameof(SturdyClamp.AfterPreventingBlockClear))]
public static class SturdyClampEnhancedPatch
{
    private const int _retainedBlock = 15;

    [HarmonyPrefix]
    public static bool Prefix(SturdyClamp __instance, AbstractModel preventer, Creature creature, ref Task __result)
    {
        // 1. 如果不是当前遗物触发的，返回 true 交给原版处理（原版方法会自行返回 Task，不会报错）
        if (__instance != preventer || creature != __instance.Owner?.Creature)
        {
            return true;
        }

        int block = creature.Block;

        // 2. 如果护甲 > 15，扣除多余护甲，将 Task 赋给 __result
        if (block != 0 && block > _retainedBlock)
        {
            __result = CreatureCmd.LoseBlock(new ThrowingPlayerChoiceContext(), creature, block - _retainedBlock, null);
        }
        else
        {
            // 3. 【关键修复】如果护甲 <= 15，不需要扣除，但必须赋一个空的 Task 防止上层 await 时报空引用异常！
            __result = Task.CompletedTask;
        }

        // 跳过原版逻辑
        return false;
    }
}

// 修改其 Var 变量，保持 json 文件翻译说明一直
[HarmonyPatch(typeof(SturdyClamp), "get_CanonicalVars")]
public static class SturdyClampGetVarPatch
{
    // NOTE: 一个修改 `CanonicalVars` 的案例
    [HarmonyPrefix]
    public static bool Prefix(ref IEnumerable<DynamicVar> __result)
    {
        __result =
        [
        new BlockVar(15m, ValueProp.Unpowered)
        ];
        return false;
    }
}
