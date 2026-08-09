using MegaCrit.Sts2.Core.Entities.Enchantments;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using STS2RitsuLib.Scaffolding.Content;

namespace Sts2BalanceMod.Sts2BalanceModCode.Abstract;

/// <summary>
/// Shared RitsuLib enchantment convention: hidden amounts, no stacking, and a type-named icon in the mod PCK.
/// Concrete enchantments only need to declare their eligibility and gameplay effect.
/// </summary>
public abstract class BalanceEnchantmentTemplate : ModEnchantmentTemplate
{
    public override bool ShowAmount => false;
    public override bool IsStackable => false;

    public override EnchantmentAssetProfile AssetProfile => new(
      IconPath: ModAssetPaths.EnchantmentIcon(ModAssetPaths.TypeFileName(GetType())));
}
