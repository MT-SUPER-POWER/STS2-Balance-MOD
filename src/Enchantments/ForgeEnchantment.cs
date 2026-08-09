using System.Collections.Generic;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Sts2BalanceMod.src.Enchantments;

/// <summary>
/// 铁砧附魔：被附魔的牌费用永久 -1（最低 0 费）。
/// </summary>
public sealed class ForgeEnchantment : EnchantmentModel
{
  public override bool ShowAmount => false;
  public override bool IsStackable => false;

  public override bool CanEnchantCardType(CardType cardType) => true;

  protected override void OnEnchant()
  {
    Card.EnergyCost.UpgradeBy(-1);
  }
}
