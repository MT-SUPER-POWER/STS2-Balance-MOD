using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

// ======================== RELIC-02: 悬浮风筝 ========================

/// <summary>
/// RELIC-02 — 悬浮风筝：你在每回合第一次弃牌时，获得1点能量。
/// 猎人（silent）专属遗物，罕见度：罕见。
/// </summary>
[RegisterRelic(typeof(SilentRelicPool), FullPublicEntry = "STS2_BALANCEMOD_HOVERING_KITE")]
public sealed class HoveringKite : BalanceRelicTemplate
{
    private const string EnergyKey = "Energy";

    public override RelicRarity Rarity => RelicRarity.Uncommon;

    protected override IEnumerable<DynamicVar> CanonicalVars => new[]
    {
    new EnergyVar(1)
  };

    protected override IEnumerable<IHoverTip> AdditionalHoverTips => new[]
    {
    HoverTipFactory.ForEnergy(this)
  };

    [SavedProperty]
    public bool DiscardedThisTurn { get; set; }

    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player == Owner)
        {
            DiscardedThisTurn = false;
            base.Status = RelicStatus.Active; // Glowing frame indicating it is ready to trigger
        }
        return Task.CompletedTask;
    }

    public override async Task AfterCardDiscarded(PlayerChoiceContext choiceContext, CardModel card)
    {
        if (Owner != null && card.Owner == Owner && Owner.Creature?.Side == Owner.Creature?.CombatState?.CurrentSide)
        {
            // Set to true synchronously to avoid race conditions with multiple discards on the same frame
            if (!DiscardedThisTurn)
            {
                DiscardedThisTurn = true;
                base.Status = RelicStatus.Normal; // Turn off glow
                Flash();
                await PlayerCmd.GainEnergy(base.DynamicVars[EnergyKey].BaseValue, Owner);
            }
        }
    }

    public override Task AfterCombatEnd(CombatRoom _)
    {
        base.Status = RelicStatus.Normal;
        DiscardedThisTurn = false;
        return Task.CompletedTask;
    }
}
