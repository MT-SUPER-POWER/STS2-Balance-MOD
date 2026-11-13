using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Runs;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// 先古卡：女巫形态 (Witch Form)
/// 3 费（升级 2 费）| 能力 | 稀有度：先古 | 目标：自身
/// 升级后获得固有。
/// 打出时获得女巫形态（每回合多抽 1 张牌），并将手牌中所有的基础打击与防御蜕变为升级后的巫术打击+与巫术防御+。
/// </summary>
[RegisterCard(typeof(EventCardPool), FullPublicEntry = "STS2_BALANCEMOD_WITCH_FORM")]
public sealed class WitchForm : BalanceCardTemplate
{
  public override IEnumerable<CardKeyword> CanonicalKeywords => [];

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

    var handCards = PileType.Hand.GetPile(Owner).Cards;
    var targets = handCards.Where(c => c.IsTransformable && c.IsBasicStrikeOrDefend).ToList();
    if (targets.Count > 0)
    {
      var transformations = new List<CardTransformation>();
      foreach (CardModel target in targets)
      {
        CardModel replacement = target.Tags.Contains(CardTag.Strike)
            ? CreateReplacement<SorceryStrike>(target, forceUpgrade: true)
            : CreateReplacement<SorceryDefend>(target, forceUpgrade: true);
        transformations.Add(new CardTransformation(target, replacement));
      }

      await CardCmd.Transform(transformations, rng: null, CardPreviewStyle.None);
    }
  }

  protected override void OnUpgrade()
  {
    EnergyCost.UpgradeBy(-1);
    AddKeyword(CardKeyword.Innate);
  }

  public static CardModel CreateReplacement<TCard>(CardModel source, bool forceUpgrade = true)
      where TCard : CardModel
  {
    ICardScope scope = source.CardScope ?? (ICardScope?)source.Owner?.RunState ?? throw new InvalidOperationException("No card scope found for card transformation.");
    CardModel replacement = scope.CreateCard<TCard>(source.Owner);
    if (forceUpgrade || source.IsUpgraded)
    {
      CardCmd.Upgrade(replacement, CardPreviewStyle.None);
    }

    if (source.Enchantment is not null)
    {
      var enchantment = (EnchantmentModel)source.Enchantment.MutableClone();
      CardCmd.Enchant(enchantment, replacement, enchantment.Amount);
    }

    return replacement;
  }
}
