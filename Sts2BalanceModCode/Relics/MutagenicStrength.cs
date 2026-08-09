using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

/// <summary>
/// STS1-EVENT-03 依赖遗物 — 突变之力：每场战斗开始时获得临时力量。
/// 来源参考 ActsFromThePast.Relics.MutagenicStrength。
/// </summary>
[RegisterRelic(typeof(EventRelicPool), FullPublicEntry = "STS2_BALANCEMOD_MUTAGENIC_STRENGTH")]
public sealed class MutagenicStrength : BalanceRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Event;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
      new PowerVar<StrengthPower>(3M),
  ];

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
      HoverTipFactory.FromPower<StrengthPower>(),
  ];

    public override async Task AfterRoomEntered(AbstractRoom room)
    {
        if (room is not CombatRoom || Owner?.Creature == null)
            return;

        Flash();
        await PowerCmd.Apply<MutagenicStrengthPower>(
          new ThrowingPlayerChoiceContext(),
          Owner.Creature,
          DynamicVars.Strength.BaseValue,
          Owner.Creature,
          null);
    }
}
