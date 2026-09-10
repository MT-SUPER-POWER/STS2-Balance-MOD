using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// 先古卡：女巫形态 (Witch Form)
/// 3 费 | 能力 | 稀有度：先古 | 目标：自身
/// 获得 1 层女巫形态。结束你的回合。
/// 升级：获得固有。
/// </summary>
[RegisterCard(typeof(EventCardPool), FullPublicEntry = "STS2_BALANCEMOD_WITCH_FORM")]
public sealed class WitchForm : BalanceCardTemplate
{
  protected override IEnumerable<DynamicVar> CanonicalVars => [new PowerVar<WitchFormPower>(1m)];

  protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
  [
      HoverTipFactory.FromCard<SorceryStrike>(upgrade: true),
      HoverTipFactory.FromCard<SorceryDefend>(upgrade: true),
  ];

  public WitchForm()
      : base(3, CardType.Power, CardRarity.Ancient, TargetType.Self)
  {
  }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    await PowerCmd.Apply<WitchFormPower>(
        choiceContext,
        Owner.Creature,
        DynamicVars[nameof(WitchFormPower)].BaseValue,
        Owner.Creature,
        this);
    PlayerCmd.EndTurn(Owner, canBackOut: false);
  }

  protected override void OnUpgrade() => AddKeyword(CardKeyword.Innate);
}
