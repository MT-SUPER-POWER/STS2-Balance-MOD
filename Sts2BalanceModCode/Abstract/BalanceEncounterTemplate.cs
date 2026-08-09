using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Rooms;
using STS2RitsuLib.Scaffolding.Content;

namespace Sts2BalanceMod.Sts2BalanceModCode.Abstract;

/// <summary>
/// MOD 遭遇模型抽象基类，提供通用默认值。
/// 所有 MOD 的自定义遭遇都应继承此类。
/// Boss 遭遇如果有特殊相机缩放/偏移，在各遭遇类中自行重写 GetCameraScaling / GetCameraOffset。
/// </summary>
public abstract class BalanceEncounterTemplate : ModEncounterTemplate
{
    public override MegaSkeletonDataResource? BossNodeSpineResource => null;

    // These encounters are addressable by events and patches, not candidates for a normal map room.
    // RegisterGlobalEncounter still makes them available through ModelDb without leaking them into an Act pool.
    public override bool IsValidForAct(ActModel act) => false;
}
