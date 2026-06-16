using BaseLib.Abstracts;
using BaseLib.Extensions;
using MegaCrit.Sts2.Core.Entities.Cards;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;

namespace Sts2BalanceMod.Sts2BalanceModCode.Abstract;

/// <summary>
/// 一代回归事件卡抽象基类。
/// NOTE: 这些卡牌从 ActsFromThePast 移植，保留 CustomCardModel 的 override 能力，同时复用本项目图片 fallback。
/// </summary>
public abstract class Sts2LegacyCardModel(int cost, CardType type, CardRarity rarity, TargetType target) :
  CustomCardModel(cost, type, rarity, target)
{
  public override string CustomPortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".BigCardImagePath();
  public override string PortraitPath => $"{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
  public override string BetaPortraitPath => $"beta/{Id.Entry.RemovePrefix().ToLowerInvariant()}.png".CardImagePath();
}
