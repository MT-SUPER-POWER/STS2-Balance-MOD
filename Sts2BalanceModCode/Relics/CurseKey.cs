using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rewards;
using System.Linq;
using System.Threading.Tasks;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

// ======================== RELIC-07: 诅咒钥匙 ========================

/// <summary>
/// RELIC-07 — 诅咒钥匙：每回合多加一点费用，但每次获取奖励时获得一张随机诅咒牌
/// </summary>
[Pool(typeof(EventRelicPool))]
public sealed class CurseKey : Sts2RelicModel
{
  public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
  public override RelicRarity Rarity => RelicRarity.Ancient;

  protected override IEnumerable<DynamicVar> CanonicalVars => [
    new EnergyVar(1),
  ];

  // ======================== 能量加成 ========================

  public override decimal ModifyMaxEnergy(Player player, decimal amount)
  {
    if (player != base.Owner)
    {
      return amount;
    }
    return amount + base.DynamicVars.Energy.IntValue;
  }

  // ======================== 奖励诅咒 ========================

  /// <summary>
  /// 每次打开宝箱获取遗物后，从诅咒池中随机获得一张诅咒牌加入牌组
  /// </summary>
  public override async Task AfterRewardTaken(Player player, Reward reward)
  {
    if (player != Owner) return;

    // NOTE: 仅宝箱遗物（RelicReward）触发，战斗奖励、商店等不触发
    if (reward is not RelicReward) return;

    // NOTE: 参考 SereTalon / CursedRun 的诅咒生成逻辑，从 CurseCardPool 中获取可用诅咒
    var availableCurses = ModelDb.CardPool<CurseCardPool>()
        .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
        .Where(c => c.CanBeGeneratedByModifiers)
        .ToList();

    if (availableCurses.Count == 0) return;

    // NOTE: 使用 Niche RNG（与 SereTalon 一致），从可用诅咒中随机选取一张
    var canonicalCurse = Owner.RunState.Rng.Niche.NextItem(availableCurses);
    var curseCard = Owner.RunState.CreateCard(canonicalCurse, Owner);

    Flash(); // 遗物闪烁特效

    await CardPileCmd.Add(curseCard, PileType.Deck);
  }
}
