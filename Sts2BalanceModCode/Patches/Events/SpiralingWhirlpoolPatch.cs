using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Enchantments;
using Sts2BalanceMod.Sts2BalanceModCode.Settings;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Events;

/// <summary>
/// ENCHANT-ECHO-WHIRLPOOL — 改造第 1 幕事件「旋转漩涡」（SpiralingWhirlpool）：
/// 目标类型：MegaCrit.Sts2.Core.Models.Events.SpiralingWhirlpool
/// 修改原因：将仅限基础打击/防御的「涡旋」附魔替换为自选牌的「回响」附魔（EchoEnchantment）。
/// WARNING: Verified against D:\Game\Sts2Code\src\MegaCrit.Sts2.Core.Models\Events\SpiralingWhirlpool.cs; game updates may change this decompiled implementation.
/// </summary>
[HarmonyPatch(typeof(SpiralingWhirlpool))]
[BalancePatch("E17")]
public static class SpiralingWhirlpoolPatch
{
  private static readonly MethodInfo? SetEventFinishedMethod =
    AccessTools.Method(typeof(EventModel), "SetEventFinished", [typeof(LocString)]);

  [HarmonyPatch("IsAllowed")]
  [HarmonyPrefix]
  private static bool IsAllowedPrefix(IRunState runState, ref bool __result)
  {
    if (!BalanceModSettings.IsEnabled("E17"))
    {
      return true;
    }

    EchoEnchantment echo = ModelDb.Enchantment<EchoEnchantment>();
    __result = runState.Players.All(p => p.Deck.Cards.Any(echo.CanEnchant));
    return false;
  }

  [HarmonyPatch("GenerateInitialOptions")]
  [HarmonyPostfix]
  private static void GenerateInitialOptionsPostfix(SpiralingWhirlpool __instance, ref IReadOnlyList<EventOption> __result)
  {
    if (!BalanceModSettings.IsEnabled("E17") || __result == null || __result.Count < 2)
    {
      return;
    }

    EchoEnchantment echo = ModelDb.Enchantment<EchoEnchantment>();
    EventOption observeEchoOption = new EventOption(
      __instance,
      () => ObserveTheEcho(__instance),
      new LocString("events", "STS2_BALANCE_MOD_EVENT_SPIRALING_WHIRLPOOL_OBSERVE.title"),
      new LocString("events", "STS2_BALANCE_MOD_EVENT_SPIRALING_WHIRLPOOL_OBSERVE.description"),
      "STS2_BALANCE_MOD_EVENT_SPIRALING_WHIRLPOOL_OBSERVE",
      HoverTipFactory.FromEnchantment<EchoEnchantment>());

    __result = [observeEchoOption, __result[1]];
  }

  private static async Task ObserveTheEcho(SpiralingWhirlpool instance)
  {
    if (instance.Owner == null)
    {
      return;
    }

    EchoEnchantment echo = ModelDb.Enchantment<EchoEnchantment>();
    CardModel? cardModel = (await CardSelectCmd.FromDeckForEnchantment(
      instance.Owner,
      echo,
      1,
      c => c != null && echo.CanEnchant(c),
      new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1))).FirstOrDefault();

    if (cardModel != null)
    {
      CardCmd.Enchant<EchoEnchantment>(cardModel, 1m);
      NCardEnchantVfx? nCardEnchantVfx = NCardEnchantVfx.Create(cardModel);
      if (nCardEnchantVfx != null)
      {
        NRun.Instance?.GlobalUi.CardPreviewContainer.AddChildSafely(nCardEnchantVfx);
      }
    }

    SetEventFinishedMethod?.Invoke(instance, [new LocString("events", "SPIRALING_WHIRLPOOL.pages.OBSERVE.description")]);
  }
}
