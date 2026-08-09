using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Saves.Runs;

namespace Sts2BalanceMod.src.Relics;

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
[RegisterRelic(typeof(SharedRelicPool), FullPublicEntry = "STS2_BALANCEMOD_OMAMORI")]
public sealed class Omamori : BalanceRelicTemplate
{
  // NOTE: CursesKey 与 relics.json 中的 {Curses} 占位符对应，用于本地化文本的动态数值替换
  private const string CursesKey = "Curses";
  private bool _isActivating;
  private int _cursesRemaining = 2;

  // The source artwork predates the normalized filename convention and keeps an uppercase large-icon filename.
  public override RelicAssetProfile AssetProfile => new(
    IconPath: ModAssetPaths.RelicIcon("omamori.png"),
    IconOutlinePath: ModAssetPaths.RelicIcon("omamori_outline.png"),
    BigIconPath: ModAssetPaths.Resource("images", "relics", "big", "Omamori.png"));

  public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
  public override RelicRarity Rarity => RelicRarity.Event;

  // 显示计数器（遗物图标右下角显示剩余可抵消次数）
  public override bool ShowCounter => true;     // NOTE: 制作一个带有计数的遗物

  /// <summary>
  /// 遗物图标上显示的数字
  /// </summary>
  public override int DisplayAmount       // NOTE: 告知外部 遗物还有 多少 计数的接口
  {
    get { return CursesRemaining; }
  }

  // NOTE: 给外部看的是具体数量
  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new DynamicVar(CursesKey, 2m),  // 初始可抵消 2 张诅咒
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
  /// 当诅咒牌进入持有者牌堆（手牌/抽牌堆/弃牌堆）时触发抵消。
  /// </summary>
  public override async Task AfterCardChangedPiles(CardModel card, PileType oldPileType, AbstractModel? clonedBy)
  {
    // 只处理诅咒牌
    if (card.Type != CardType.Curse) return;

    // 只响应持有者本人的牌，不响应敌人或队友的
    if (card.Owner != Owner) return;

    // 次数已用完
    if (CursesRemaining <= 0) return;

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
    this.IsActivating = true;
    Flash();
    await Cmd.Wait(1f);
    this.IsActivating = false;
  }

}
