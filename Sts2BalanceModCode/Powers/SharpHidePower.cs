using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

public sealed class SharpHidePower() : Sts2PowerModel(PowerType.Buff, PowerStackType.Counter)
{
  public bool AttackInProgress { get; private set; }

  public Creature? AttackSource { get; private set; }

  public override string CustomPackedIconPath =>
    "res://Sts2BalanceMod/images/powers/actsfromthepast-sharp_hide_power.png";

  public override string CustomBigIconPath => CustomPackedIconPath;

  public override Task BeforeCardPlayed(CardPlay cardPlay)
  {
    if (cardPlay.Card.Type == CardType.Attack)
    {
      AttackInProgress = true;
      AttackSource = cardPlay.Card.Owner?.Creature;
    }

    return Task.CompletedTask;
  }

  public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    AttackInProgress = false;
    AttackSource = null;

    if (cardPlay.Card.Type != CardType.Attack)
      return;

    Flash();
    var player = cardPlay.Card.Owner?.Creature;
    if (player is { IsAlive: true })
    {
      await CreatureCmd.Damage(choiceContext, player, Amount, ValueProp.Unpowered, Owner, null);
    }
  }
}
