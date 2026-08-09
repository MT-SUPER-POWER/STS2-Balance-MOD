using STS2RitsuLib.Interop.AutoRegistration;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Effects;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Audio;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Combat;

namespace Sts2BalanceMod.Sts2BalanceModCode.Powers;

/// <summary>
/// STS1-BOSS-01 — 时间扭曲能力。
/// 输入：玩家每次打出牌后触发。
/// 输出：第 12 张牌后结束当前玩家回合，并让时间吞噬者获得力量。
/// </summary>
[RegisterPower]
public sealed class TimeWarpPower() : BalancePowerTemplate(PowerType.Buff, PowerStackType.Counter)
{
  private const decimal BaseCardsPerWarp = 12M;
  private const decimal CardsPerExtraPlayer = 3M;
  private const decimal StrengthGain = 2M;
  private static readonly string TimeWarpSfx = ModAssetPaths.Resource("sfx", "time_eater", "time_warp.ogg");

  public override PowerAssetProfile AssetProfile => new(
    IconPath: ModAssetPaths.PowerIcon("actsfromthepast-time_warp_power.png"),
    BigIconPath: ModAssetPaths.PowerIcon("actsfromthepast-time_warp_power.png"));

  private int PlayerCount => Math.Max(1, Owner?.CombatState?.Players.Count ?? 1);

  private decimal CardsPerWarp => BaseCardsPerWarp + CardsPerExtraPlayer * (PlayerCount - 1);

  public override async Task AfterCardPlayed(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    if (Owner?.CombatState == null || cardPlay.Card.Owner?.Creature == null)
      return;

    if (cardPlay.Card.Owner.Creature.Side == Owner.Side)
      return;

    // NOTE: Sly牌不计入时间扭曲计数
    if (cardPlay.Card.IsSlyThisTurn)
      return;

    if (Amount > 1M)
    {
      await PowerCmd.ModifyAmount(choiceContext, this, -1M, Owner, null, silent: false);
      return;
    }

    Flash();
    BalanceModAudio.PlayOneShot(TimeWarpSfx);
    // 视觉效果：时钟弹出动画
    var effect = TimeWarpTurnEndEffect.Create();
    if (NCombatRoom.Instance?.CombatVfxContainer is Node vfxContainer)
      vfxContainer.AddChildSafely(effect.Root);
    await Cmd.Wait(0.35f);
    BalanceModAudio.PlayOneShot(TimeWarpSfx, 0.65f);

    await PowerCmd.ModifyAmount(choiceContext, this, CardsPerWarp - Amount, Owner, null, silent: false);
    await PowerCmd.Apply<StrengthPower>(choiceContext, Owner, StrengthGain, Owner, null);
    PlayerCmd.EndTurn(cardPlay.Card.Owner, canBackOut: false);
  }
}
