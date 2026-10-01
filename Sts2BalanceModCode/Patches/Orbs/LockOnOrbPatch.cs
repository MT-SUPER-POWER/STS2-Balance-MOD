using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Orbs;

/// <summary>
/// 充能球伤害标记补丁：
/// 拦截 OrbCmd.Evoke 与 OrbCmd.Passive，在其异步生命周期内设置 OrbDamageContext.IsOrbDealingDamage = true，
/// 供 LockOnPower 等机制精确识别充能球来源伤害并施加 50% 增伤。
/// </summary>
[HarmonyPatch]
public static class OrbDamageTrackingPatch
{
  [HarmonyPatch(typeof(OrbCmd), "Evoke")]
  [HarmonyPostfix]
  public static void PostfixEvoke(ref Task __result)
  {
    __result = WrapWithContext(__result);
  }

  [HarmonyPatch(typeof(OrbCmd), "Passive")]
  [HarmonyPostfix]
  public static void PostfixPassive(ref Task __result)
  {
    __result = WrapWithContext(__result);
  }

  private static async Task WrapWithContext(Task originalTask)
  {
    OrbDamageContext.IsOrbDealingDamage = true;
    try
    {
      await originalTask;
    }
    finally
    {
      OrbDamageContext.IsOrbDealingDamage = false;
    }
  }
}

/// <summary>
/// 暗黑球锁定补丁：
/// 若场上有带有跟踪锁定（LockOnPower）的敌人，暗黑球激发时优先命中该敌人（多个时取当前生命最低者）。
/// </summary>
[HarmonyPatch(typeof(DarkOrb), "Evoke")]
public static class DarkOrbLockOnPatch
{
  [HarmonyPrefix]
  public static bool Prefix(DarkOrb __instance, PlayerChoiceContext playerChoiceContext, ref Task<IEnumerable<Creature>> __result)
  {
    var lockedOnEnemies = __instance.CombatState?.HittableEnemies
      .Where(e => e.HasPower<LockOnPower>())
      .ToList();

    if (lockedOnEnemies != null && lockedOnEnemies.Count > 0)
    {
      __result = EvokeLockedOn(__instance, playerChoiceContext, lockedOnEnemies);
      return false;
    }

    return true;
  }

  private static async Task<IEnumerable<Creature>> EvokeLockedOn(DarkOrb orb, PlayerChoiceContext playerChoiceContext, List<Creature> lockedOnEnemies)
  {
    AccessTools.Method(typeof(OrbModel), "PlayEvokeSfx")?.Invoke(orb, null);
    Creature? target = lockedOnEnemies.MinBy(c => c.CurrentHp);
    if (target == null)
    {
      return [];
    }
    AccessTools.Method(typeof(OrbModel), "ActivateEvoke", [typeof(Creature[])])?.Invoke(orb, [new Creature[] { target }]);
    await CreatureCmd.Damage(playerChoiceContext, target, orb.EvokeVal, ValueProp.Unpowered, orb.Owner.Creature);
    return [target];
  }
}
