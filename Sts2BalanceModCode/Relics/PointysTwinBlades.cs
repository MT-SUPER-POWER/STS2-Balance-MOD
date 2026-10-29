using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Cards;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

/// <summary>
/// STS1-EVENT-08 红面具大人之墓关联事件遗物 — 尖刺的双刺：
/// 第 1 回合将 2 张具有保留且能造成虚弱的小刀+加入手牌。
/// </summary>
[RegisterRelic(typeof(EventRelicPool), FullPublicEntry = "STS2_BALANCEMOD_POINTYS_TWIN_BLADES")]
public sealed class PointysTwinBlades : BalanceRelicTemplate
{
  public override RelicRarity Rarity => RelicRarity.Event;

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new DynamicVar("Shivs", 2m),
  ];

  protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
    HoverTipFactory.FromCardWithCardHoverTips<PointyShiv>();

  public override async Task BeforeHandDraw(Player player, PlayerChoiceContext choiceContext, ICombatState combatState)
  {
    Player? owner = Owner;
    if (owner == null || player != owner)
      return;

    if (owner.PlayerCombatState?.TurnNumber <= 1)
    {
      Flash();
      List<CardModel> shivs = [];
      for (int i = 0; i < DynamicVars["Shivs"].IntValue; i++)
      {
        shivs.Add(combatState.CreateCard<PointyShiv>(owner));
      }
      await CardPileCmd.AddGeneratedCardsToCombat(shivs, PileType.Hand, owner);
    }
  }
}
