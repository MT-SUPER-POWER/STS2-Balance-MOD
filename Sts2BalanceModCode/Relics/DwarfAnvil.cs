using BaseLib.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens.CardSelection;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Enchantments;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

// ======================== RELIC-10: 矮人铁砧 ========================

/// <summary>
/// RELIC-10 — 矮人铁砧：商店遗物。
/// 拾起时选择一张攻击/技能牌附魔，附魔后该牌可在火堆 Smith 反复升级，
/// 每次按 ceil(n(n+7)/2) 公式提升伤害/格挡。
/// </summary>
[Pool(typeof(SharedRelicPool))]
public sealed class DwarfAnvil : Sts2RelicModel
{
  public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
  public override RelicRarity Rarity => RelicRarity.Rare;

  public override bool HasUponPickupEffect => true;

  protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    HoverTipFactory.FromEnchantment<ForgeEnchantment>();

  public override async Task AfterObtained()
  {
    var forge = ModelDb.Enchantment<ForgeEnchantment>();
    var prefs = new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1);

    foreach (var card in await CardSelectCmd.FromDeckForEnchantment(Owner, forge, 1, prefs))
    {
      CardCmd.Enchant(forge.ToMutable(), card, 1m);
      CardCmd.Preview(card);
    }
  }
}

// ===== Patch: 持有 DwarfAnvil 时 Smith 可用 + 可选附魔牌 =====

/// <summary>
/// 持有 DwarfAnvil 且牌组有附魔牌时，即使无普通可升级牌也启用 Smith。
/// </summary>
[HarmonyPatch(typeof(SmithRestSiteOption), "IsEnabled", MethodType.Getter)]
internal static class DwarfAnvilSmithEnabledPatch
{
  [HarmonyPostfix]
  private static void Postfix(SmithRestSiteOption __instance, ref bool __result)
  {
    if (__result) return;

    var ownerProp = typeof(RestSiteOption).GetProperty("Owner", BindingFlags.Instance | BindingFlags.NonPublic);
    var owner = (Player?)ownerProp?.GetValue(__instance);
    if (owner?.GetRelic<DwarfAnvil>() == null) return;

    if (PileType.Deck.GetPile(owner).Cards.Any(c => c.Enchantment is ForgeEnchantment))
      __result = true;
  }
}

/// <summary>
/// 持有 DwarfAnvil 时，Smith 的选牌列表中包含 forge-enchanted 牌（即使已升级）。
/// 选择附魔牌进行 Smith 会消耗本次锻造机会，并递增其 forge count。
/// </summary>
[HarmonyPatch(typeof(SmithRestSiteOption), nameof(SmithRestSiteOption.OnSelect))]
internal static class DwarfAnvilSmithPrefixPatch
{
  private static readonly FieldInfo? SmithCountField =
    typeof(SmithRestSiteOption).GetField("SmithCount", BindingFlags.Instance | BindingFlags.Public);

  [HarmonyPrefix]
  private static bool Prefix(SmithRestSiteOption __instance, ref Task<bool> __result)
  {
    var ownerProp = typeof(RestSiteOption).GetProperty("Owner", BindingFlags.Instance | BindingFlags.NonPublic);
    var owner = (Player?)ownerProp?.GetValue(__instance);
    if (owner?.GetRelic<DwarfAnvil>() == null)
      return true; // 无遗物→走原逻辑

    var smithCount = (int)(SmithCountField?.GetValue(__instance) ?? 1);
    var forge = ModelDb.Enchantment<ForgeEnchantment>();

    var deck = PileType.Deck.GetPile(owner);

    // 正常可升级牌
    var upgradeCards = deck.Cards.Where(c => c.IsUpgradable).ToList();
    // 附魔牌
    var forgeCards = deck.Cards.Where(c => c.Enchantment is ForgeEnchantment).ToList();

    var combined = upgradeCards.Concat(forgeCards).Distinct().ToList();
    if (combined.Count == 0)
    {
      __result = Task.FromResult(false);
      return false;
    }

    // 直接使用游戏内置选牌界面
    var prefs = new CardSelectorPrefs(CardSelectorPrefs.UpgradeSelectionPrompt, smithCount)
    {
      Cancelable = true,
      RequireManualConfirmation = true,
    };

    // 需要有 RunState 来调用 ShowScreen；从玩家的 RunState 拿
    var screen = NDeckUpgradeSelectScreen.ShowScreen(combined, prefs, owner.RunState);
    __result = HandleSmithSelection(screen, owner, forge);

    return false; // 跳过原方法
  }

  private static async Task<bool> HandleSmithSelection(
    NDeckUpgradeSelectScreen screen, Player owner, EnchantmentModel forge)
  {
    var selected = await screen.CardsSelected();
    if (!selected.Any())
      return false;

    foreach (var card in selected)
    {
      if (card.Enchantment is ForgeEnchantment)
      {
        // 附魔牌：递增 forge 次数
        var existing = card.Enchantment?.Amount ?? 0;
        CardCmd.Enchant(forge.ToMutable(), card, existing + 1);
      }
      else
      {
        // 正常牌：升级
        CardCmd.Upgrade(card, CardPreviewStyle.None);
      }
    }

    await Hook.AfterRestSiteSmith(owner.RunState, owner);
    return true;
  }
}
