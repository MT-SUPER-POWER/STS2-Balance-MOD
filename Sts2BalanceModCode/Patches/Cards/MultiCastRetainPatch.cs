using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// CARD-05 — 多重释放升级后获得保留词条
/// 升级后的 +1 释放次数由 OnPlay 中的 IsUpgraded 判断处理，不由 OnUpgrade 控制
/// 所以只需 Postfix 追加 Retain 即可
/// </summary>
[HarmonyPatch(typeof(CardModel), nameof(CardModel.UpgradeInternal))]
[HarmonyPatch(typeof(CardModel), "OnUpgrade")]
public static class MultiCastRetainPatch
{
    [HarmonyPrefix]
    public static bool Prefix(CardModel __instance)
    {
        if (__instance is not MultiCast)
            return true; // 其他卡正常升级

        __instance.AddKeyword(CardKeyword.Retain);

        return false; // 也就是取消原本的 X+1
    }
}
