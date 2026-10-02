using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Random;

namespace Sts2BalanceMod.Sts2BalanceModCode.Minigames;

/// <summary>
/// 对对碰小游戏核心控制器。
/// 负责根据玩家卡池与诅咒池生成 6 对配对卡牌、洗牌打乱，并协调全屏 UI 进行交互。
/// </summary>
public sealed class MatchAndKeepMinigame
{
  private readonly TaskCompletionSource _completionSource = new();

  public Player Owner { get; }

  /// <summary>12 张打乱展示的卡牌实例。</summary>
  public CardModel[] Cards { get; }

  /// <summary>12 张卡牌对应的配对组索引 (0–5)。两张卡具有相同索引即视为匹配成功。</summary>
  public int[] PairIndices { get; }

  /// <summary>6 种基础卡牌模型。</summary>
  public CardModel[] Canonicals { get; }

  public int MaxAttempts { get; }
  public int ActIndex { get; }

  public MatchAndKeepMinigame(Player owner, Rng rng, int attempts, int actIndex)
  {
    Owner = owner;
    MaxAttempts = attempts;
    ActIndex = actIndex;

    Cards = new CardModel[12];
    PairIndices = new int[12];
    Canonicals = new CardModel[6];

    GenerateCards(rng);
    ShuffleCards(rng);
  }

  private void GenerateCards(Rng rng)
  {
    for (int i = 0; i < ActIndex; i++)
    {
      rng.NextInt(1);
    }

    var characterPool = Owner.Character.CardPool
      .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
      .ToList();

    var cursePool = ModelDb.CardPool<CurseCardPool>()
      .GetUnlockedCards(Owner.UnlockState, Owner.RunState.CardMultiplayerConstraint)
      .Where(c => c.CanBeGeneratedByModifiers)
      .ToList();

    Canonicals[0] = rng.NextItem(characterPool.Where(c => c.Rarity == CardRarity.Rare))
      ?? rng.NextItem(characterPool)!;
    Canonicals[1] = rng.NextItem(characterPool.Where(c => c.Rarity == CardRarity.Uncommon))
      ?? rng.NextItem(characterPool)!;
    Canonicals[2] = rng.NextItem(characterPool.Where(c => c.Rarity == CardRarity.Common))
      ?? rng.NextItem(characterPool)!;

    Canonicals[3] = cursePool.Count > 0 ? rng.NextItem(cursePool)! : ModelDb.Card<Guilty>();
    Canonicals[4] = cursePool.Count > 0 ? rng.NextItem(cursePool)! : ModelDb.Card<Guilty>();

    var basics = characterPool.Where(c =>
      c.Rarity == CardRarity.Basic &&
      !c.Tags.Contains(CardTag.Strike) &&
      !c.Tags.Contains(CardTag.Defend)).ToList();

    if (basics.Count == 0)
    {
      basics = Owner.Character.StartingDeck
        .Where(c => c.Rarity == CardRarity.Basic &&
                    !c.Tags.Contains(CardTag.Strike) &&
                    !c.Tags.Contains(CardTag.Defend))
        .ToList();
    }

    Canonicals[5] = basics.Count > 0
      ? rng.NextItem(basics)!
      : (rng.NextItem(characterPool.Where(c => c.Rarity == CardRarity.Common)) ?? rng.NextItem(characterPool)!);

    // 每种模型生成 2 张卡牌实例
    for (int i = 0; i < 6; i++)
    {
      Cards[i * 2] = Owner.RunState.CreateCard(Canonicals[i], Owner);
      Cards[i * 2 + 1] = Owner.RunState.CreateCard(Canonicals[i], Owner);
      PairIndices[i * 2] = i;
      PairIndices[i * 2 + 1] = i;
    }
  }

  private void ShuffleCards(Rng rng)
  {
    for (int i = 11; i > 0; i--)
    {
      int j = rng.NextInt(i + 1);
      (Cards[i], Cards[j]) = (Cards[j], Cards[i]);
      (PairIndices[i], PairIndices[j]) = (PairIndices[j], PairIndices[i]);
    }
  }

  public void Complete()
  {
    if (_completionSource.Task.IsCompleted)
      return;
    _completionSource.SetResult();
  }

  public void ForceEnd()
  {
    if (_completionSource.Task.IsCompleted)
      return;
    _completionSource.TrySetCanceled();
  }

  public async Task PlayMinigame()
  {
    if (!LocalContext.IsMe(Owner))
      return;

    NMatchAndKeepScreen.ShowScreen(this);
    await _completionSource.Task;
  }
}
