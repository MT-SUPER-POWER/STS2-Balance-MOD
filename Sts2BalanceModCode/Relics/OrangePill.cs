using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Rooms;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

// ======================== RELIC-02: 药丸 ========================

/// <summary>
/// 橙色药丸 — 你每在同一回合内打出攻击牌、技能牌、能力牌各一张，移除你身上的所有负面效果。
/// </summary>
[Pool(typeof(SharedRelicPool))]
public sealed class OrangePill : Sts2RelicModel
{
  private bool _isActivating;
  private bool _isAttackPlayed;
  private bool _isSkillPlayed;
  private bool _isPowerPlayed;

  public override string FlashSfx => "event:/sfx/ui/relic_activate_general";

  public override RelicRarity Rarity => RelicRarity.Shop;
  private bool IsActivating
  {
    get => _isActivating;
    set
    {
      AssertMutable();
      _isActivating = value;
    }
  }

  private bool IsAttackPlayed
  {
    get => _isAttackPlayed;
    set
    {
      AssertMutable();
      _isAttackPlayed = value;
    }
  }

  private bool IsSkillPlayed
  {
    get => _isSkillPlayed;
    set
    {
      AssertMutable();
      _isSkillPlayed = value;
    }
  }

  private bool IsPowerPlayed
  {
    get => _isPowerPlayed;
    set
    {
      AssertMutable();
      _isPowerPlayed = value;
    }
  }

  /// <summary>
  /// 玩家打出一张牌后触发。
  /// </summary>
  public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    // 只响应持有者本人打出的牌
    if (cardPlay.Card.Owner != Owner)
      return;

    switch (cardPlay.Card.Type)
    {
      case CardType.Attack:
        IsAttackPlayed = true;
        break;

      case CardType.Skill:
        IsSkillPlayed = true;
        break;

      case CardType.Power:
        IsPowerPlayed = true;
        break;

      default:
        return;
    }

    UpdateRelicStatus();

    if (!HasAllThreeTypesPlayed())
      return;

    await TaskHelper.RunSafely(ActivateAndCleanDebuffs());
  }

  /// <summary>
  /// 回合结束时清空本回合记录。
  /// </summary>
  public override Task BeforeSideTurnEnd(PlayerChoiceContext choiceContext, CombatSide side, IEnumerable<Creature> participants)
  {
    // 玩家回合结束
    if (side == CombatSide.Player) ResetPlayedFlags();

    return Task.CompletedTask;
  }

  private bool HasAllThreeTypesPlayed()
  {
    return IsAttackPlayed && IsSkillPlayed && IsPowerPlayed;
  }

  private bool HasAnyTypePlayed()
  {
    return IsAttackPlayed || IsSkillPlayed || IsPowerPlayed;
  }

  private void UpdateRelicStatus()
  {
    // 这里用 Active 表示本回合已经开始凑三种牌了
    Status = HasAnyTypePlayed()
      ? RelicStatus.Active
      : RelicStatus.Normal;
  }

  private void ResetPlayedFlags()
  {
    IsAttackPlayed = false;
    IsSkillPlayed = false;
    IsPowerPlayed = false;

    Status = RelicStatus.Normal;
  }

  private async Task ActivateAndCleanDebuffs()
  {
    if (IsActivating)
      return;

    IsActivating = true;

    try
    {
      Flash();

      await RemoveAllDebuffs();

      // 给玩家一点点时间看到遗物触发效果
      await Cmd.Wait(0.25f);
    }
    finally
    {
      // 触发一次后重置，允许同一回合内再次凑齐三种牌再触发一次
      ResetPlayedFlags();
      IsActivating = false;
    }
  }

  private async Task RemoveAllDebuffs()
  {
    // 不准清除女王的 魂缚锁链 负面效果
    var debuffs = Owner.Creature.Powers
      .Where(power => power.Type == PowerType.Debuff && power.Id != ModelDb.GetId<ChainsOfBindingPower>())
      .ToList();

    foreach (var debuff in debuffs)
    {
      await PowerCmd.Remove(debuff);
    }
  }

  public override Task AfterCombatEnd(CombatRoom _)
  {
    // 只清掉激活状态，否则下一场战斗可能还显示 Active。
    base.Status = RelicStatus.Normal;
    ResetPlayedFlags();

    return Task.CompletedTask;
  }
}
