using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Events;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using Sts2BalanceMod.Sts2BalanceModCode.Minigames;

namespace Sts2BalanceMod.Sts2BalanceModCode.Events;

/// <summary>
/// 对对碰 (Match and Keep)。
/// 经典一代神龛事件：4x3 翻牌配对小游戏，5 次尝试机会，翻到相同两张卡牌直接获得。
/// </summary>
[RegisterSharedEvent]
public sealed class MatchAndKeep : BalanceEventTemplate
{
  private const int Attempts = 5;

  public override bool IsShared => true;

  protected override IEnumerable<DynamicVar> CanonicalVars =>
  [
    new IntVar("Attempts", Attempts)
  ];

  protected override IReadOnlyList<EventOption> GenerateInitialOptions() =>
  [
    Option(Continue)
  ];

  private Task Continue()
  {
    SetEventState(PageDescription("RULES"),
    [
      Option(Play, "RULES")
    ]);
    return Task.CompletedTask;
  }

  private async Task Play()
  {
    Player? owner = Owner;
    if (owner == null || Rng == null)
      return;

    MatchAndKeepMinigame minigame = new(owner, Rng, Attempts, owner.RunState.CurrentActIndex);
    await minigame.PlayMinigame();
    SetEventFinished(PageDescription("COMPLETE"));
  }
}
