using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

/// <summary>
/// STS1-EVENT-08 红面具大人之墓关联事件遗物 — 巨熊的皮甲：
/// 拾取时获得 8 点最大生命值。第 1 回合获得 10 点格挡，并对所有敌人施加 1 层虚弱。
/// </summary>
[RegisterRelic(typeof(EventRelicPool), FullPublicEntry = "STS2_BALANCEMOD_BEARS_PELT")]
public sealed class BearsPelt : BalanceRelicTemplate
{
  public override RelicRarity Rarity => RelicRarity.Event;
  public override bool HasUponPickupEffect => true;

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new MaxHpVar(8m),
    new BlockVar(10m, ValueProp.Unpowered),
    new PowerVar<WeakPower>(1m),
  ];

  protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
  [
    HoverTipFactory.Static(StaticHoverTip.Block),
    HoverTipFactory.FromPower<WeakPower>(),
  ];

  public override async Task AfterObtained()
  {
    if (Owner?.Creature != null)
    {
      await CreatureCmd.GainMaxHp(Owner.Creature, DynamicVars.MaxHp.BaseValue);
    }
  }

  public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, IReadOnlyList<Creature> participants, ICombatState combatState)
  {
    Player? owner = Owner;
    if (owner?.Creature == null || !participants.Contains(owner.Creature))
      return;

    if (owner.PlayerCombatState?.TurnNumber <= 1)
    {
      Flash();
      await CreatureCmd.GainBlock(owner.Creature, DynamicVars.Block.BaseValue, ValueProp.Unpowered, null);
      await PowerCmd.Apply<WeakPower>(choiceContext, combatState.HittableEnemies, DynamicVars["WeakPower"].BaseValue, owner.Creature, null);
    }
  }
}
