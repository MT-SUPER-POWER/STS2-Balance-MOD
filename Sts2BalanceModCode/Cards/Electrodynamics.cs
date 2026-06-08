using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Orbs;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// LEGACY-04 — 电动力学（机器人，替换吞噬暗影）
/// 2费 | 稀有 | 能力 | 召唤 2 个闪电球，闪电球改为攻击所有敌人
/// </summary>
[Pool(typeof(DefectCardPool))]
public sealed class Electrodynamics : Sts2CardModel
{
  public Electrodynamics() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self)
  {
    WithCards(2, 1); // 召唤 2 个闪电球
  }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);

    for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
      await OrbCmd.Channel<LightningOrb>(choiceContext, Owner);

    await PowerCmd.Apply<ElectrodynamicsPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
  }
}
