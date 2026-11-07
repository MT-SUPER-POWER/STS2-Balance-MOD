using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Enchantments;

/// <summary>
/// ENCHANT-ECHO-WHIRLPOOL — 回响附魔：被附魔的卡牌打出时额外打出 1 次（Replay 1）。
/// </summary>
[RegisterEnchantment]
public sealed class EchoEnchantment : BalanceEnchantmentTemplate
{
  private const string _timesKey = "Times";

  protected override IEnumerable<DynamicVar> CanonicalVars => [new IntVar(_timesKey, 1m)];

  public override EnchantmentAssetProfile AssetProfile => new(
    IconPath: ModAssetPaths.EnchantmentIcon("EchoEnchantment.png"));

  public override bool CanEnchantCardType(CardType cardType) =>
    cardType is CardType.Attack or CardType.Skill or CardType.Power;

  public override int EnchantPlayCount(int originalPlayCount) =>
    originalPlayCount + DynamicVars[_timesKey].IntValue;

  protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [HoverTipFactory.Static(StaticHoverTip.ReplayDynamic, DynamicVars[_timesKey])];
}
