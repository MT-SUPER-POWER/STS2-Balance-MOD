using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// 遗物「尖刺的双刺」生成的特制小刀：
/// 0 耗能攻击牌，造成 6 点伤害，保留、消耗，给予 1 层虚弱。
/// </summary>
[RegisterCard(typeof(TokenCardPool), FullPublicEntry = "STS2_BALANCEMOD_POINTY_SHIV")]
public sealed class PointyShiv : BalanceCardTemplate
{
  public override TargetType TargetType => HasFanOfKnives ? TargetType.AllEnemies : TargetType.AnyEnemy;

  public override bool CanBeGeneratedByModifiers => false;

  public override int MaxUpgradeLevel => 0;

  protected override HashSet<CardTag> CanonicalTags => [CardTag.Shiv];

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new DamageVar(6m, ValueProp.Move),
    new PowerVar<WeakPower>(1m),
  ];

  public override IEnumerable<CardKeyword> CanonicalKeywords =>
  [
    CardKeyword.Retain,
    CardKeyword.Exhaust,
  ];

  protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
  [
    HoverTipFactory.FromPower<WeakPower>(),
  ];

  private bool HasFanOfKnives => IsMutable && Owner != null && Owner.Creature.HasPower<FanOfKnivesPower>();

  public PointyShiv()
    : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy)
  {
  }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    Player? owner = Owner;
    ICombatState? combatState = CombatState;
    if (owner?.Creature == null || combatState == null)
      return;

    Creature ownerCreature = owner.Creature;
    AttackCommand attackCommand = DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay);
    if (HasFanOfKnives)
    {
      Creature? lastEnemy = combatState.HittableEnemies.LastOrDefault();
      attackCommand = attackCommand.TargetingAllOpponents(combatState)
        .WithHitVfxNode(_ => NShivThrowVfx.Create(ownerCreature, lastEnemy, Colors.Green));
    }
    else
    {
      ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
      attackCommand = attackCommand.Targeting(cardPlay.Target)
        .WithHitVfxNode(t => NShivThrowVfx.Create(ownerCreature, t, Colors.Green));
    }

    if (owner.Character is Silent)
    {
      attackCommand.WithAttackerAnim("Shiv", 0.2f);
    }

    await attackCommand.Execute(choiceContext);

    decimal weakAmount = DynamicVars["WeakPower"].BaseValue;
    if (HasFanOfKnives)
    {
      await PowerCmd.Apply<WeakPower>(choiceContext, combatState.HittableEnemies, weakAmount, ownerCreature, this);
    }
    else if (cardPlay.Target != null && cardPlay.Target.IsAlive)
    {
      await PowerCmd.Apply<WeakPower>(choiceContext, cardPlay.Target, weakAmount, ownerCreature, this);
    }
  }
}
