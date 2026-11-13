using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Cards;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

/// <summary>
/// 先古遗物：精致的玩偶 (Delicate Doll)
/// 效果: 拾起时失去 25% 最大生命值上限，将一张未升级的【女巫形态】加入牌组，并将 2 张牌蜕变为升级后的【巫术打击+】与【巫术防御+】。
/// 战斗开始时，对所有敌人施加 2 层易伤与 2 层虚弱。
/// </summary>
[RegisterRelic(typeof(SharedRelicPool), FullPublicEntry = "STS2_BALANCEMOD_DELICATE_DOLL")]
public sealed class DelicateDoll : BalanceRelicTemplate
{
  public override string FlashSfx => "event:/sfx/ui/relic_activate_general";
  public override RelicRarity Rarity => RelicRarity.Ancient;

  public override bool HasUponPickupEffect => true;

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
      new PowerVar<VulnerablePower>(2m),
      new PowerVar<WeakPower>(2m),
  ];

  protected override IEnumerable<IHoverTip> AdditionalHoverTips =>
  [
      HoverTipFactory.FromCard<WitchForm>(upgrade: false),
      HoverTipFactory.FromCard<SorceryStrike>(upgrade: true),
      HoverTipFactory.FromCard<SorceryDefend>(upgrade: true),
  ];

  public override async Task AfterObtained()
  {
    if (Owner?.RunState == null || Owner.Creature == null)
      return;

    Flash();
    int hpLoss = Owner.Creature.MaxHp / 4;
    if (hpLoss > 0)
    {
      await CreatureCmd.LoseMaxHp(new ThrowingPlayerChoiceContext(), Owner.Creature, hpLoss, null);
    }

    CardModel card = Owner.RunState.CreateCard<WitchForm>(Owner);
    CardCmd.PreviewCardPileAdd(await CardPileCmd.Add(card, PileType.Deck), 2f);

    var prefs = new CardSelectorPrefs(CardSelectorPrefs.TransformSelectionPrompt, 2)
    {
      Cancelable = false,
    };
    List<CardModel> selectedCards = (await CardSelectCmd.FromDeckForTransformation(Owner, prefs)).ToList();
    if (selectedCards.Count > 0)
    {
      var transformations = new List<CardTransformation>();
      CardModel strike = WitchForm.CreateReplacement<SorceryStrike>(selectedCards[0], forceUpgrade: true);
      transformations.Add(new CardTransformation(selectedCards[0], strike));

      if (selectedCards.Count > 1)
      {
        CardModel defend = WitchForm.CreateReplacement<SorceryDefend>(selectedCards[1], forceUpgrade: true);
        transformations.Add(new CardTransformation(selectedCards[1], defend));
      }

      await CardCmd.Transform(transformations, rng: null, CardPreviewStyle.HorizontalLayout);
    }
  }

  public override async Task BeforeSideTurnStart(
      PlayerChoiceContext choiceContext,
      CombatSide side,
      IReadOnlyList<Creature> participants,
      ICombatState combatState)
  {
    if (Owner.PlayerCombatState is null || !participants.Contains(Owner.Creature) || Owner.PlayerCombatState.TurnNumber > 1)
    {
      return;
    }

    Flash();
    await PowerCmd.Apply<VulnerablePower>(
        choiceContext,
        combatState.HittableEnemies,
        DynamicVars[nameof(VulnerablePower)].BaseValue,
        Owner.Creature,
        null);
    await PowerCmd.Apply<WeakPower>(
        choiceContext,
        combatState.HittableEnemies,
        DynamicVars[nameof(WeakPower)].BaseValue,
        Owner.Creature,
        null);
  }
}
