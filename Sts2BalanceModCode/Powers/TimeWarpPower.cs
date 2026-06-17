using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Utility;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// STS1-BOSS-01 — 时间扭曲能力。
/// 输入：玩家每次打出牌后触发。
/// 输出：第 12 张牌后结束当前玩家回合，并让时间吞噬者获得力量。
/// </summary>
public sealed class TimeWarpPower() : Sts2PowerModel(PowerType.Buff, PowerStackType.Counter)
{
  private const decimal CardsPerWarp = 12M;
  private const decimal StrengthGain = 2M;
  private const string TimeWarpSfx = "res://Sts2BalanceMod/sfx/time_eater/time_warp.ogg";

  public override string CustomPackedIconPath =>
    "res://Sts2BalanceMod/images/powers/actsfromthepast-time_warp_power.png";

  public override string CustomBigIconPath => CustomPackedIconPath;

  public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    if (Owner?.CombatState == null || cardPlay.Card.Owner?.Creature == null)
      return;

    if (cardPlay.Card.Owner.Creature.Side == Owner.Side)
      return;

    if (Amount > 1M)
    {
      await PowerCmd.ModifyAmount(choiceContext, this, -1M, Owner, null, silent: false);
      return;
    }

    Flash();
    Sts2ModAudio.PlayOneShot(TimeWarpSfx);
    await Cmd.Wait(0.35f);
    Sts2ModAudio.PlayOneShot(TimeWarpSfx, 0.65f);

    await PowerCmd.ModifyAmount(choiceContext, this, CardsPerWarp - Amount, Owner, null, silent: false);
    await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, StrengthGain, Owner, null);
    PlayerCmd.EndTurn(cardPlay.Card.Owner, canBackOut: false);
  }
}
