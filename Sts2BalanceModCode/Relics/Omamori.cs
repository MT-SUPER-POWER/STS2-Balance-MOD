using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Runs;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Rooms;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

// ======================== RELIC-04: 御守 ========================

/// <summary>
/// EVENT-04 — 旧日垃圾堆奖励加入**御守**
/// 御守 — 抵消你接下来获得的 2 张诅咒牌。
///
/// 核心逻辑：
///   1. AfterCardChangedPiles() 每次卡牌改变牌堆时被游戏调用
///   2. 检测到诅咒牌进入持有者牌堆时，消耗诅咒并扣减计数器
///   3. 计数器归零后遗物耗尽
/// </summary>
[Pool(typeof(SharedRelicPool))]
public sealed class Omamori : Sts2RelicModel
{
  // NOTE: CursesKey 与 relics.json 中的 {Curses} 占位符对应，用于本地化文本的动态数值替换
  private const string CursesKey = "Curses";

  private bool _isActivating;

  public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
  public override RelicRarity Rarity => RelicRarity.Common;

  // 显示计数器（遗物图标右下角显示剩余可抵消次数）
  public override bool ShowCounter => true;

  /// <summary>
  /// 遗物图标上显示的数字：
  ///   - 动画播放期间显示满值（2），表示"正在抵消"的状态
  ///   - 平时显示剩余可抵消次数（2、1、0）
  /// </summary>
  public override int DisplayAmount
  {
    get
    {
      if (IsActivating)
        return base.DynamicVars[CursesKey].IntValue;

      return CursesRemaining;
    }
  }

  // NOTE: 注册变量到本地化文本，key 名与 relics.json 中的 {Curses} 对应
  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new DynamicVar(CursesKey, 2m),  // 初始可抵消 2 张诅咒
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
  /// 剩余可抵消诅咒次数 — 带 [SavedProperty] 特性，会自动存盘并在读档时恢复。
  /// </summary>
  [SavedProperty]
  public int CursesRemaining
  {
    get => _cursesRemaining;
    set
    {
      AssertMutable();
      _cursesRemaining = value;
      InvokeDisplayAmountChanged();
    }
  }
  private int _cursesRemaining = 2;

  /// <summary>
  /// 每次卡牌改变牌堆时调用。
  /// 当诅咒牌进入持有者牌堆（手牌/抽牌堆/弃牌堆）时触发抵消。
  /// </summary>
  public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
  {
    // 只处理诅咒牌
    if (card.Type != CardType.Curse)
      return;

    // 只响应持有者本人的牌，不响应敌人或队友的
    if (card.Owner != Owner)
      return;

    // 次数已用完
    if (CursesRemaining <= 0)
      return;

    // 扣减剩余次数
    CursesRemaining--;

    // 将诅咒牌送入消耗堆
    await CardPileCmd.RemoveFromDeck(card, showPreview: false);

    // 播放激活动画
    await TaskHelper.RunSafely(DoActivateVisuals());

    // 更新遗物状态：次数归零则标记为耗尽
    base.Status = CursesRemaining > 0 ? RelicStatus.Normal : RelicStatus.Disabled;
  }

  /// <summary>
  /// 播放激活视觉反馈：
  ///   1. 设置 IsActivating = true → 显示"满"状态数字
  ///   2. 遗物闪光 Flash()
  ///   3. 等 1 秒让玩家看到
  ///   4. 恢复 IsActivating = false，根据剩余次数确定最终状态
  /// </summary>
  private async Task DoActivateVisuals()
  {
    IsActivating = true;
    Flash();

    await Cmd.Wait(1f);

    IsActivating = false;

    base.Status = CursesRemaining > 0 ? RelicStatus.Normal : RelicStatus.Disabled;
  }

}
