using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Enchantments;

/// <summary>
/// 铁砧附魔：被附魔的牌费用永久 -1（最低 0 费）。
/// </summary>
[RegisterEnchantment]
public sealed class ForgeEnchantment : BalanceEnchantmentTemplate
{
    public override bool CanEnchantCardType(CardType cardType) => true;

    protected override void OnEnchant()
    {
        Card.EnergyCost.UpgradeBy(-1);
    }
}
