using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models;

namespace Sts2BalanceMod.Sts2BalanceModCode.Abstract;

/// <summary>
/// MOD 怪物模型抽象基类，提供通用默认值。
/// 所有 MOD 的自定义怪物都应继承此类。
/// </summary>
public abstract class Sts2MonsterModel : MonsterModel
{
  // NOTE: 原版 MonsterModel 的 Title 使用 base.Id.Entry 生成 LocString，
  // 但 BaseLib 不会对原版 MonsterModel 的 LocString 自动添加 MOD 前缀。
  // 因此这里显式包含 STS2BALANCEMOD 前缀，确保能匹配 localization/xxx/monsters.json。
  public override LocString Title =>
    new LocString("monsters", "STS2BALANCEMOD-" + GetType().Name.ToUpper() + ".name");
}
