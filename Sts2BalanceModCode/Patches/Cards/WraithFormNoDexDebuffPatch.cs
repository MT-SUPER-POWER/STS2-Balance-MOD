using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Cards;

/// <summary>
/// 幽魂形态：
/// 1. 移除 WraithFormPower 负面效果
/// 2. 移除 DexterityPower 的 HoverTip
/// </summary>
public static class WraithFormReworkPatch
{
    /// <summary>
    /// 拦截打出效果：只获得无实体，不再获得 WraithFormPower
    /// </summary>
    [HarmonyPatch(typeof(WraithForm), "OnPlay")]
    public static class OnPlayPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(
          WraithForm __instance,
          PlayerChoiceContext choiceContext,
          CardPlay cardPlay,
          ref Task __result
        )
        {
            __result = Run(__instance, choiceContext, cardPlay);
            return false;
        }

        private static async Task Run(
          WraithForm card,
          PlayerChoiceContext choiceContext,
          CardPlay cardPlay
        )
        {
            await CreatureCmd.TriggerAnim(
              card.Owner.Creature,
              "PowerUp",
              card.Owner.Character.PowerUpAnimDelay
            );

            await PowerCmd.Apply<IntangiblePower>(
              choiceContext,
              card.Owner.Creature,
              card.DynamicVars["IntangiblePower"].BaseValue,
              card.Owner.Creature,
              card
            );

            // 原版这里还有：
            // await PowerCmd.Apply<WraithFormPower>(...);
            // 我们直接不要。
        }
    }

    /// <summary>
    /// 拦截 HoverTips：只保留无实体说明，移除敏捷说明
    /// </summary>
    [HarmonyPatch]
    public static class ExtraHoverTipsPatch
    {
        public static MethodBase TargetMethod()
        {
            return AccessTools.PropertyGetter(
              typeof(WraithForm),
              "ExtraHoverTips"
            );
        }

        [HarmonyPrefix]
        public static bool Prefix(ref IEnumerable<IHoverTip> __result)
        {
            __result = new IHoverTip[]
            {
        HoverTipFactory.FromPower<IntangiblePower>()
            };

            return false;
        }
    }
}
