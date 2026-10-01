using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// CARD-FISSION-01 — 裂变（Fission）
/// 0费 | 稀有 | 技能 | 消耗
/// 基础：移除所有充能球。每移除一个充能球，获得 1 点能量并抽 1 张牌。
/// 升级：激发所有充能球。每激发一个充能球，获得 1 点能量并抽 1 张牌。
/// </summary>
[RegisterCard(typeof(DefectCardPool), FullPublicEntry = "STS2_BALANCEMOD_FISSION")]
public sealed class Fission : BalanceCardTemplate
{
  public override OrbEvokeType OrbEvokeType => IsUpgraded ? OrbEvokeType.All : OrbEvokeType.None;

  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

  protected override IEnumerable<IHoverTip> AdditionalHoverTips => [HoverTipFactory.Static(StaticHoverTip.Evoke)];

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new EnergyVar(1),
    new CardsVar(1)
  ];

  public Fission() : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self) { }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    if (Owner?.PlayerCombatState == null)
    {
      return;
    }

    int orbCount = Owner.PlayerCombatState.OrbQueue.Orbs.Count;
    if (orbCount <= 0)
    {
      return;
    }

    await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

    if (IsUpgraded)
    {
      for (int i = 0; i < orbCount; i++)
      {
        await OrbCmd.EvokeNext(choiceContext, Owner);
        await Cmd.CustomScaledWait(0.1f, 0.2f);
      }
    }
    else
    {
      while (Owner.PlayerCombatState.OrbQueue.Orbs.Count > 0)
      {
        OrbModel orb = Owner.PlayerCombatState.OrbQueue.Orbs[0];
        Owner.PlayerCombatState.OrbQueue.Remove(orb);
        NCombatRoom.Instance?.GetCreatureNode(Owner.Creature)?.OrbManager?.EvokeOrbAnim(orb);
        orb.RemoveInternal();
        await Cmd.CustomScaledWait(0.05f, 0.1f);
      }
    }

    await PlayerCmd.GainEnergy(orbCount * DynamicVars.Energy.IntValue, Owner);
    await CardPileCmd.Draw(choiceContext, orbCount * DynamicVars.Cards.IntValue, Owner);
  }
}
