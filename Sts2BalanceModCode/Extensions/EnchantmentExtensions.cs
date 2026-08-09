using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Sts2BalanceMod.Sts2BalanceModCode.Extensions;

/// <summary>
/// Standardizes the visible completion of an enchantment grant.
/// </summary>
public static class EnchantmentExtensions
{
    /// <summary>
    /// Applies a registered enchantment and immediately queues the card preview that shows the resulting change.
    /// Call this only after the caller has selected a card accepted by the enchantment's eligibility rules.
    /// </summary>
    public static void ApplyEnchantmentAndPreview<TEnchantment>(this CardModel card, decimal amount)
      where TEnchantment : EnchantmentModel
    {
        ArgumentNullException.ThrowIfNull(card);

        CardCmd.Enchant<TEnchantment>(card, amount);
        CardCmd.Preview(card);
    }
}
