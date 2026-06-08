using BaseLib.Utils;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;


namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// LEGACY-02 — 硬撑（战士）
/// 1费 | 罕见 | 技能 | 获得 15 点格挡, 往手中塞入两张伤口
/// 升级：格挡 15→20
/// </summary>
[Pool(typeof(IroncladCardPool))]
public sealed class PowerThought : Sts2CardModel
{
  public PowerThought() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
  {
    WithBlock(15, 5); // 基础 15，升级 +5 = 20
    WithTip(typeof(Wound));
  }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    await CardPileCmd.AddToCombatAndPreview<Wound>(Owner.Creature, PileType.Hand, 2, Owner);
  }
}
