using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

/// <summary>
/// Every 3 times you shuffle your draw pile, gain 2 energy.
/// The shuffle count persists across combats.
/// </summary>
[Pool(typeof(SharedRelicPool))]
public sealed class Sundial : Sts2RelicModel
{
  private const string ShufflesKey = "Shuffles";
  private const string EnergyKey = "Energy";

  private bool _isActivating;
  private int _shufflesSeen;

  public override string FlashSfx => "event:/sfx/ui/gain_energy";

  public override RelicRarity Rarity => RelicRarity.Shop;

  public override bool ShowCounter => true;

  public override int DisplayAmount
  {
    get
    {
      if (IsActivating)
        return base.DynamicVars[ShufflesKey].IntValue;

      return ShufflesSeen;
    }
  }

  // NOTE: 注册变量到本地化文本，key 名与 relics.json 中的 {Shuffles}、{Energy} 对应
  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new DynamicVar(ShufflesKey, 3m),
    new EnergyVar(2),
  ];

  private bool IsActivating
  {
    get => _isActivating;
    set
    {
      AssertMutable();
      _isActivating = value;
      InvokeDisplayAmountChanged();
    }
  }

  [SavedProperty]
  public int ShufflesSeen
  {
    get => _shufflesSeen;
    set
    {
      AssertMutable();
      _shufflesSeen = value;
      InvokeDisplayAmountChanged();
    }
  }

  public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
  {
    if (shuffler != Owner)
      return;

    var requiredShuffles = base.DynamicVars[ShufflesKey].IntValue;

    ShufflesSeen = (ShufflesSeen + 1) % requiredShuffles;

    base.Status = ShufflesSeen == requiredShuffles - 1
      ? RelicStatus.Active
      : RelicStatus.Normal;

    if (ShufflesSeen == 0)
    {
      await TaskHelper.RunSafely(DoActivateVisuals());

      await PlayerCmd.GainEnergy(
        base.DynamicVars[EnergyKey].BaseValue,
        shuffler
      );
    }
  }

  private async Task DoActivateVisuals()
  {
    IsActivating = true;
    Flash();

    await Cmd.Wait(1f);

    IsActivating = false;

    // 触发动画结束后，根据当前计数恢复状态
    base.Status = ShufflesSeen == base.DynamicVars[ShufflesKey].IntValue - 1
      ? RelicStatus.Active
      : RelicStatus.Normal;
  }

  public override Task AfterCombatEnd(CombatRoom _)
  {
    // NOTE:这里不要重置 ShufflesSeen。
    // 只清掉激活状态，否则下一场战斗可能还显示 Active。
    base.Status = RelicStatus.Normal;
    return Task.CompletedTask;
  }
}
