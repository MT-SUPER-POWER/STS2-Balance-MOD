using System.Threading;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace Sts2BalanceMod.src.RestSite;

// ======================== REST SITE OPTION: PEACE PIPE ========================

/// <summary>
/// 宁静烟斗火堆选项：在火堆删除一张可移除卡牌。
/// 输入：构造时传入火堆所属玩家。
/// 输出：打开删牌界面，确认后从牌组移除一张牌。
/// 返回值：玩家成功删除卡牌时返回 true；取消选择或无可删牌时返回 false。
/// </summary>
public sealed class PeacePipeRestSiteOption : BalanceRestSiteOption
{
  private const int CardsToRemove = 1;

  public override string OptionId => "SMOKE";

  public override bool IsEnabled => GetRemovableCardCount(Owner) >= CardsToRemove;

  public override LocString Description
  {
    get
    {
      LocString description = new("rest_site_ui", $"OPTION_{OptionId}.description");
      description.Add("Cards", CardsToRemove);
      return IsEnabled
        ? description
        : new LocString("rest_site_ui", $"OPTION_{OptionId}.descriptionDisabled");
    }
  }

  public override IEnumerable<string> AssetPaths => base.AssetPaths.Concat(NRestSmokeVfx.AssetPaths);

  public PeacePipeRestSiteOption(Player owner) : base(owner)
  {
  }

  public override async Task<bool> OnSelect()
  {
    IReadOnlyList<CardModel> selectedCards = await SelectCardsForRemoval(CardsToRemove);
    if (selectedCards.Count == 0)
    {
      return false;
    }

    await RemoveCardsFromDeck(selectedCards);
    return true;
  }

  public override Task DoLocalPostSelectVfx(CancellationToken ct = default)
  {
    NDebugAudioManager.Instance?.Play("SOTE_SFX_SleepBlanket_v1.mp3", 0.45f, PitchVariance.Small);
    NRestSiteRoom.Instance?.AddChildSafely(NRestSmokeVfx.Create());

    return Task.CompletedTask;
  }

  public override Task DoRemotePostSelectVfx()
  {
    return DoLocalPostSelectVfx();
  }
}
