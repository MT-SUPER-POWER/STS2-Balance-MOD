using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

/// <summary>
/// STS1-EVENT-08 红面具大人之墓关联事件遗物 — 尖刺的双刺：
/// 第 1 回合将 2 张小刀加入手牌；攻击对虚弱或易伤敌人造成额外伤害。
/// </summary>
[RegisterRelic(typeof(EventRelicPool), FullPublicEntry = "STS2_BALANCEMOD_POINTYS_TWIN_BLADES")]
public sealed class PointysTwinBlades : BalanceRelicTemplate
{
  public override RelicRarity Rarity => RelicRarity.Event;

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new DynamicVar("Shivs", 2m),
    new DynamicVar("ExtraDamage", 3m),
  ];

  protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    HoverTipFactory.FromCardWithCardHoverTips<Shiv>()
      .Concat([
        HoverTipFactory.FromPower<WeakPower>(),
        HoverTipFactory.FromPower<VulnerablePower>(),
      ]);

  public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
  {
    Player? owner = Owner;
    if (owner == null || player != owner)
      return;

    if (owner.PlayerCombatState?.TurnNumber <= 1)
    {
      Flash();
      await Shiv.CreateInHand(owner, DynamicVars["Shivs"].IntValue, combatState);
    }
  }

  public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props, Creature? dealer, CardModel? cardSource, CardPlay? cardPlay)
  {
    if (!props.IsPoweredAttack() || target == null || dealer != Owner?.Creature)
      return 0m;

    if (target.Powers.Any(p => p is WeakPower || p is VulnerablePower))
      return DynamicVars["ExtraDamage"].BaseValue;

    return 0m;
  }
}
