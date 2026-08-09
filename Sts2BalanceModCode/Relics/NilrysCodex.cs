using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models.RelicPools;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Relics;

/// <summary>
/// STS1-RELIC-01 — 尼利的宝典：回合结束时从 3 张随机牌中选择 1 张洗入抽牌堆。
/// 来源参考 ActsFromThePast.Relics.NilrysCodex。
/// RELIC-04: 所有展示的卡牌均为升级版。
/// </summary>
[RegisterRelic(typeof(EventRelicPool), FullPublicEntry = "STS2_BALANCEMOD_NILRYS_CODEX")]
public sealed class NilrysCodex : BalanceRelicTemplate
{
    public override RelicRarity Rarity => RelicRarity.Event;

    public override async Task BeforeFlushLate(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != Owner)
            return;

        if (CombatManager.Instance == null || !CombatManager.Instance.IsInProgress)
            return;

        Flash();

        var cardChoices = CardFactory.GetDistinctForCombat(
          Owner,
          Owner.Character.CardPool.GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint),
          3,
          Owner.RunState.Rng.CombatCardGeneration).ToList();

        // RELIC-04: 升级所有候选卡牌，使玩家获得升级版本
        // WARNING: 依赖 CardCmd.Upgrade 内部调用 UpgradeInternal + FinalizeUpgradeInternal，
        //          不直接调用 card.Upgrade() 以保持与游戏正式升级流程一致。
        foreach (var card in cardChoices)
            CardCmd.Upgrade(card);

        var selectedCard = await CardSelectCmd.FromChooseACardScreen(
          choiceContext,
          cardChoices,
          Owner,
          true);

        if (selectedCard == null)
            return;

        var result = await CardPileCmd.AddGeneratedCardToCombat(
          selectedCard,
          PileType.Draw,
          Owner,
          CardPilePosition.Random);
        CardCmd.PreviewCardPileAdd(result);
    }
}
