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

// ======================== RELIC-01: 日晷 ========================

/// <summary>
/// 日晷（Sundial）— 每将抽牌堆洗牌 3 次，获得 3 点能量。
/// 洗牌次数跨战斗保留（不重置）。
///
/// 核心逻辑：
///   1. AfterShuffle() 每次洗牌时被游戏调用
///   2. 内部维护计数器 ShufflesSeen，0→1→2→0 循环
///   3. 到达第 3 次（ShufflesSeen 归零）时触发回能动画
///   4. 跨战斗不清零计数器，只清零 UI 激活状态
/// </summary>
[Pool(typeof(SharedRelicPool))]
public sealed class Sundial : Sts2RelicModel
{
  // NOTE: 这两个 key 与 relics.json 中的 {Shuffles} 和 {Energy} 占位符一一对应，
  //       用于本地化文本的动态数值替换
  private const string ShufflesKey = "Shuffles";
  private const string EnergyKey = "Energy";

  // 是否正在播放激活动画（闪光 + 等待）
  private bool _isActivating;
  // 当前已计数多少次洗牌（模 requiredShuffles）
  private int _shufflesSeen;

  public override string FlashSfx => "event:/sfx/ui/gain_energy";

  // 日晷是商店遗物
  public override RelicRarity Rarity => RelicRarity.Shop;

  // 显示计数器（在遗物图标右下角显示数字）
  public override bool ShowCounter => true;

  /// <summary>
  /// 遗物图标上显示的数字：
  ///   - 动画播放期间显示所需次数（如 3），表示"满"的状态
  ///   - 平时显示当前计数（如 0、1、2）
  /// </summary>
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
  //       DynamicVar 的 decimal 值 = 变量的"基准值"
  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new DynamicVar(ShufflesKey, 3m),   // 需要 3 次洗牌触发回能
    new EnergyVar(3),                  // 触发后获得 3 点能量
  ];

  /// <summary>
  /// 动画激活状态属性。
  /// 修改时需要调用 AssertMutable() 确保遗物当前可修改，
  /// 然后通过 InvokeDisplayAmountChanged() 通知 UI 刷新显示数值。
  /// </summary>
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

  /// <summary>
  /// 洗牌计数 — 带 [SavedProperty] 特性，会自动存盘并在读档时恢复。
  /// 所以洗牌次数可以跨战斗跨楼层保留。
  /// </summary>
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

  /// <summary>
  /// 每次抽牌堆洗牌时调用。
  /// 洗牌来源包括：回合结束自动洗牌、策略/御守/压缩等卡牌效果。
  ///
  /// 计数器逻辑：
  ///   - (count + 1) % 3，实现 0→1→2→0→1→2→... 循环
  ///   - 当计数从 2 变回 0 时触发回能
  ///   - 从 0→1 和 1→2 时仅更新 UI 状态（显示黄色发光边框）
  /// </summary>
  public override async Task AfterShuffle(PlayerChoiceContext choiceContext, Player shuffler)
  {
    // 只响应持有者本人的洗牌，不响应敌人或队友的
    if (shuffler != Owner)
      return;

    var requiredShuffles = base.DynamicVars[ShufflesKey].IntValue;

    // 步进计数器（模运算实现循环计数）
    ShufflesSeen = (ShufflesSeen + 1) % requiredShuffles;

    // 当 ShufflesSeen == 2 时（= 下一次归零）显示 Active 状态（发光边框）
    // 否则显示普通状态
    base.Status = ShufflesSeen == requiredShuffles - 1
      ? RelicStatus.Active
      : RelicStatus.Normal;

    // 计数归零 = 第 3 次洗牌，触发回能
    if (ShufflesSeen == 0)
    {
      // TaskHelper.RunSafely 确保动画即使抛出异常也不会中断回能
      await TaskHelper.RunSafely(DoActivateVisuals());

      // 给持有者回复能量
      await PlayerCmd.GainEnergy(
        base.DynamicVars[EnergyKey].BaseValue,
        shuffler
      );
    }
  }

  /// <summary>
  /// 播放激活视觉反馈：
  ///   1. 设置 IsActivating = true → 显示"满"状态数字
  ///   2. 遗物闪光 Flash()
  ///   3. 等 1 秒让玩家看到
  ///   4. 恢复 IsActivating = false，根据当前计数确定最终状态
  /// </summary>
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

  /// <summary>
  /// 战斗结束时调用。
  ///
  /// NOTE: 这里不要重置 ShufflesSeen（洗牌计数跨战斗保留），
  ///       否则会成为"每次战斗重置"的版，跟原版日晷不一致。
  ///       只清掉激活状态，防止下一场战斗还显示发光边框。
  /// </summary>
  public override Task AfterCombatEnd(CombatRoom _)
  {
    // NOTE:这里不要重置 ShufflesSeen。
    // 只清掉激活状态，否则下一场战斗可能还显示 Active。
    base.Status = RelicStatus.Normal;
    return Task.CompletedTask;
  }
}
