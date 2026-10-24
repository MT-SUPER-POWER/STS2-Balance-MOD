using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Orbs;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Orbs;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// CARD-RECURSION-01 — 递归（Recursion）
/// 1费（升级0费） | 普通 | 技能 | 目标：自身
/// 激发你最右侧的充能球。然后生成一个刚才被激发的充能球。
/// 特殊处理：暗黑球（DarkOrb）保留蓄力数值（_evokeVal），玻璃球（GlassOrb）保留剩余点数（_passiveVal）。
/// </summary>
[RegisterCard(typeof(DefectCardPool), FullPublicEntry = "STS2_BALANCEMOD_RECURSION")]
public sealed class Recursion : BalanceCardTemplate
{
  public override OrbEvokeType OrbEvokeType => OrbEvokeType.Front;

  protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
  [
    HoverTipFactory.Static(StaticHoverTip.Evoke),
    HoverTipFactory.Static(StaticHoverTip.Channeling)
  ];

  public Recursion() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    if (Owner?.PlayerCombatState == null)
    {
      return;
    }

    OrbQueue orbQueue = Owner.PlayerCombatState.OrbQueue;
    if (orbQueue.Orbs.Count <= 0)
    {
      return;
    }

    await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);

    OrbModel firstOrb = orbQueue.Orbs[0];
    ModelId orbId = firstOrb.Id;
    decimal? darkEvokeVal = null;
    decimal? glassPassiveVal = null;

    if (firstOrb is DarkOrb darkOrb)
    {
      darkEvokeVal = darkOrb.EvokeVal;
    }
    else if (firstOrb is GlassOrb glassOrb)
    {
      if (AccessTools.Field(typeof(GlassOrb), "_passiveVal")?.GetValue(glassOrb) is decimal pVal)
      {
        glassPassiveVal = pVal;
      }
    }

    await OrbCmd.EvokeNext(choiceContext, Owner);
    await Cmd.CustomScaledWait(0.1f, 0.2f);

    OrbModel newOrb = ModelDb.GetById<OrbModel>(orbId).ToMutable();
    if (newOrb is DarkOrb newDark && darkEvokeVal.HasValue)
    {
      AccessTools.Field(typeof(DarkOrb), "_evokeVal")?.SetValue(newDark, darkEvokeVal.Value);
    }
    else if (newOrb is GlassOrb newGlass && glassPassiveVal.HasValue)
    {
      AccessTools.Field(typeof(GlassOrb), "_passiveVal")?.SetValue(newGlass, glassPassiveVal.Value);
    }

    await OrbCmd.Channel(choiceContext, newOrb, Owner);
  }

  protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
