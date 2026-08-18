using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

/// <summary>
/// 商店遗物：奇怪的汤勺 (Strange Spoon)
/// 获得效果：打出消耗的牌，有 50% 的几率进入弃牌堆而不是消耗。（凋萎必定会被消耗）
/// </summary>
[RegisterRelic(typeof(SharedRelicPool), FullPublicEntry = "STS2_BALANCEMOD_STRANGE_SPOON")]
public sealed class StrangeSpoon : BalanceRelicTemplate
{
    public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
    public override RelicRarity Rarity => RelicRarity.Shop;

    public override bool HasUponPickupEffect => true;

    protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    [
        HoverTipFactory.FromKeyword(CardKeyword.Exhaust),
        new HoverTip(
            new LocString("relics", "STS2_BALANCEMOD_STRANGE_SPOON_WITHER.title"),
            new LocString("relics", "STS2_BALANCEMOD_STRANGE_SPOON_WITHER.description")
        )
    ];

    public override async Task AfterObtained()
    {
        if (Owner?.Creature == null)
            return;
        Flash();
    }
}
