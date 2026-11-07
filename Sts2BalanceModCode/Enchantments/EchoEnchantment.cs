using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Enchantments;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Enchantments;

/// <summary>
/// ENCHANT-ECHO-WHIRLPOOL — 回响附魔：被附魔的卡牌每回合首次打出时额外打出 1 次（Replay 1）。
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

  private bool HasBeenPlayedThisTurn =>
    HasCard &&
    Card.CombatState != null &&
    CombatManager.Instance?.History?.CardPlaysStarted?.Any(e =>
      e.CardPlay.Card == Card &&
      e.CardPlay.IsFirstInSeries &&
      e.HappenedThisTurn(Card.CombatState)) == true;

  public override int EnchantPlayCount(int originalPlayCount)
  {
    if (HasBeenPlayedThisTurn)
    {
      return originalPlayCount;
    }

    return originalPlayCount + DynamicVars[_timesKey].IntValue;
  }

  public override Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    if (cardPlay.Card == Card)
    {
      Status = EnchantmentStatus.Disabled;
    }

    return Task.CompletedTask;
  }

  public override Task BeforeSideTurnStart(
    PlayerChoiceContext choiceContext,
    CombatSide side,
    IReadOnlyList<Creature> participants,
    ICombatState combatState)
  {
    if (HasCard && Card.Owner?.Creature != null && participants.Contains(Card.Owner.Creature))
    {
      Status = EnchantmentStatus.Normal;
    }

    return Task.CompletedTask;
  }

  public override Task BeforeCombatStart()
  {
    Status = EnchantmentStatus.Normal;
    return Task.CompletedTask;
  }

  public override Task AfterCombatEnd(CombatRoom room)
  {
    Status = EnchantmentStatus.Normal;
    return Task.CompletedTask;
  }

  protected override IEnumerable<IHoverTip> ExtraHoverTips =>
    [HoverTipFactory.Static(StaticHoverTip.ReplayDynamic, DynamicVars[_timesKey])];
}
