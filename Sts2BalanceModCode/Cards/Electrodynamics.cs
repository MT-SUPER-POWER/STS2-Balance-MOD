using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Orbs;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// LEGACY-04 — 电动力学（机器人，替换吞噬暗影）
/// 2费 | 稀有 | 能力 | 召唤 2 个闪电球，闪电球改为攻击所有敌人
/// </summary>
[RegisterCard(typeof(DefectCardPool), FullPublicEntry = "STS2_BALANCEMOD_ELECTRODYNAMICS")]
public sealed class Electrodynamics : BalanceCardTemplate
{
  protected override IEnumerable<DynamicVar> CanonicalVars => [new CardsVar(2)];

  public Electrodynamics() : base(2, CardType.Power, CardRarity.Rare, TargetType.Self) { }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    await CreatureCmd.TriggerAnim(Owner.Creature, "PowerUp", Owner.Character.PowerUpAnimDelay);

    for (int i = 0; i < DynamicVars.Cards.IntValue; i++)
      await OrbCmd.Channel<LightningOrb>(choiceContext, Owner);

    await PowerCmd.Apply<ElectrodynamicsPower>(choiceContext, Owner.Creature, 1, Owner.Creature, this);
  }

  protected override void OnUpgrade() => DynamicVars.Cards.UpgradeValueBy(1);
}
