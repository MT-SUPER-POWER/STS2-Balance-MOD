using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Saves.Runs;
using STS2RitsuLib.Interop.AutoRegistration;

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
/// TODO: 在 option 旁边加遗物图片，提示用户，可以御守阻挡，但是 <= 0 就不再提示了
/// </summary>
[RegisterRelic(typeof(SharedRelicPool), FullPublicEntry = "STS2_BALANCEMOD_OMAMORI")]
public sealed class Omamori : BalanceRelicTemplate
{
  // NOTE: _cursesKey 与 relics.json 中的 {Curses} 占位符对应，用于本地化文本的动态数值替换
  private const string _cursesKey = "Curses";
  private bool _isActivating;
  private int _cursesRemaining = 2;

  public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
  public override RelicRarity Rarity => RelicRarity.Event;

  // 显示计数器（遗物图标右下角显示剩余可抵消次数）
  public override bool ShowCounter => true;     // NOTE: 制作一个带有计数的遗物

  /// <summary>
  /// 遗物图标上显示的数字
  /// </summary>
  public override int DisplayAmount => CursesRemaining;       // NOTE: 告知外部 遗物还有 多少 计数的接口

  // NOTE: 给外部看的是具体数量
  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new DynamicVar(_cursesKey, 2m),  // 初始可抵消 2 张诅咒
  ];

  /// <summary>
  /// 动画激活状态属性。
  /// 激活动画的时候，通知外部读取新的遗物计数
  /// </summary>
  private bool IsActivating
  {
    get => _isActivating;
    set
    {
      AssertMutable();    // 相当于保护锁
      _isActivating = value;
      InvokeDisplayAmountChanged();    // NOTE: 通知外部变更遗物计数地主动 notify
    }
  }

  /// <summary>
  /// 剩余可抵消诅咒次数 — 带 [SavedProperty] 特性，SL 也不会重置
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


  /// <summary>
  /// 每次卡牌改变牌堆时调用。
  /// 只拦截诅咒牌「首次获得」时（oldPileType == PileType.None），
  /// 即牌是全新创建加入牌组，而非战斗中在各堆之间移动。
  /// WARNING: 游戏在 CardPileCmd.Add 中以 oldPile?.Type ?? PileType.None 传入 oldPile，
  ///          当牌从未属于任何堆时 oldPileType 为 PileType.None，此即「获得」时机。
  ///          若不加此守卫，战斗内诅咒牌入手时也会触发，导致 RemoveFromDeck 在
  ///          战斗上下文中抛出异常，破坏 ActionQueue 造成牌悬停无法打出。
  /// </summary>
  public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
  {
    // 只处理诅咒牌
    if (card.Type != CardType.Curse)
      return;

    // 只响应持有者本人的牌，不响应敌人或队友的
    if (card.Owner != Owner)
      return;

    // 只拦截「首次获得」（oldPileType == PileType.None 表示牌是新创建的，从未属于任何堆）
    // 遗物描述「获得的诅咒」指路途中获得的新诅咒，与战斗内抽牌/弃牌无关
    if (oldPileType != PileType.None)
      return;

    // 次数已用完
    if (CursesRemaining <= 0)
      return;

    // 扣减剩余次数, 注意这里扣的是 SL 的那个值
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
  }

}
