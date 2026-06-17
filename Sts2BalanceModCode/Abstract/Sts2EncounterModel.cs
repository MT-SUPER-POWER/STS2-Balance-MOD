using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;

namespace Sts2BalanceMod.Sts2BalanceModCode.Abstract;

/// <summary>
/// MOD 遭遇模型抽象基类，提供通用默认值。
/// 所有 MOD 的自定义遭遇都应继承此类。
/// Boss 遭遇如果有特殊相机缩放/偏移，在各遭遇类中自行重写 GetCameraScaling / GetCameraOffset。
/// </summary>
public abstract class Sts2EncounterModel : EncounterModel
{
  public override MegaSkeletonDataResource? BossNodeSpineResource => null;

  public virtual string? CustomScenePath => null;
}
