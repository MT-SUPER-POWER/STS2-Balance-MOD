using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Entities.RestSite;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.RestSite;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

// ======================== RELIC-05: 宁静烟斗 ========================

/// <summary>
/// RELIC-05 — 宁静烟斗：在火堆新增“烟斗”选项，允许删除一张牌。
/// </summary>
[RegisterRelic(typeof(SharedRelicPool), FullPublicEntry = "STS2_BALANCEMOD_PEACE_PIPE")]
public sealed class PeacePipe : BalanceRestSiteRelicTemplate
{
    public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
    public override RelicRarity Rarity => RelicRarity.Rare;

    protected override bool CanAddRestSiteOption(Player player, ICollection<RestSiteOption> options)
    {
        // NOTE: 避免多个来源重复追加同一个按钮，后续扩展其他烟斗类选项时也能复用该保护。
        return options.All(static option => option is not PeacePipeRestSiteOption);
    }

    protected override RestSiteOption CreateRestSiteOption(Player player)
    {
        return new PeacePipeRestSiteOption(player);
    }
}
