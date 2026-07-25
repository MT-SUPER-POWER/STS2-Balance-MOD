using System.Linq;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Combat.History.Entries;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// CARD-01 — 内脏切除 (Eviscerate)
/// 3费 | 攻击 | 罕见 | 静默猎手卡池
/// 你在这个回合内每丢弃一张牌，耗能就减少 1 点能量。造成 7 点伤害 3 次。
/// 升级：伤害 7 -> 9
/// </summary>
[Pool(typeof(SilentCardPool))]
public sealed class Eviscerate : Sts2CardModel
{
  public Eviscerate()
      : base(3, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
  {
    WithDamage(7, 2); // 基础 7，升级 +2 = 9
  }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

    await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
        .WithHitCount(3)
        .FromCard(this, cardPlay)
        .Targeting(cardPlay.Target)
        .WithHitFx("vfx/vfx_attack_slash")
        .Execute(choiceContext);
  }

  public override Task AfterCardEnteredCombat(CardModel card)
  {
    if (card != this || IsClone)
    {
      return Task.CompletedTask;
    }
    int count = CombatManager.Instance.History.Entries
        .OfType<CardDiscardedEntry>()
        .Count(e => e.Card.Owner == Owner && e.HappenedThisTurn(CombatState));
    ReduceCostBy(count);
    return Task.CompletedTask;
  }

  public override Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
  {
    if (card.Owner != Owner)
    {
      return Task.CompletedTask;
    }
    ReduceCostBy(1);
    return Task.CompletedTask;
  }

  private void ReduceCostBy(int amount)
  {
    if (amount > 0)
    {
      EnergyCost.AddThisTurn(-amount);
    }
  }
}
