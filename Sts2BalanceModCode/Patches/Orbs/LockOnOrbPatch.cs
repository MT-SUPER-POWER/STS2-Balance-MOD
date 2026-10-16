using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
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
/// 充能球全局伤害上下文补丁：
/// 在 CreatureCmd.Damage 结算伤害前，根据 PlayerChoiceContext 中的模型栈精确识别是否为充能球发起的伤害。
/// 这一步在同步主线程执行，确保后续 Hook.ModifyDamage 中的 LockOnPower 能够立即识别充能球伤害来源。
/// </summary>
[HarmonyPatch(typeof(CreatureCmd), nameof(CreatureCmd.Damage),
  [typeof(PlayerChoiceContext), typeof(IEnumerable<Creature>), typeof(decimal), typeof(ValueProp), typeof(Creature), typeof(CardModel), typeof(CardPlay)])]
public static class CreatureCmdDamageOrbContextPatch
{
  [HarmonyPrefix]
  public static void Prefix(PlayerChoiceContext choiceContext, CardModel? cardSource, out bool __state)
  {
    __state = OrbDamageContext.IsOrbDealingDamage;
    if (cardSource == null && (choiceContext?.LastInvolvedModel is OrbModel || choiceContext?.ModelStack?.Any(m => m is OrbModel) == true))
    {
      OrbDamageContext.IsOrbDealingDamage = true;
    }
  }

  [HarmonyPostfix]
  public static void Postfix(bool __state)
  {
    OrbDamageContext.IsOrbDealingDamage = __state;
  }
}

/// <summary>
/// 闪电球锁定补丁：
/// 1. 若场上有带有跟踪锁定（LockOnPower）的敌人且未指定目标，闪电球优先命中被锁定的敌人。
/// 2. 在伤害执行生命周期内设置 OrbDamageContext.IsOrbDealingDamage = true，确保被动与激发伤害均能享受 50% 加伤。
/// </summary>
[HarmonyPatch(typeof(LightningOrb), "ApplyLightningDamage")]
public static class LightningOrbLockOnPatch
{
  [HarmonyPrefix]
  public static void Prefix(LightningOrb __instance, ref Creature? target, out bool __state)
  {
    __state = OrbDamageContext.IsOrbDealingDamage;
    OrbDamageContext.IsOrbDealingDamage = true;

    // 如果已有指定目标（如卡牌直接指定），则不改变
    if (target != null)
      return;

    var lockedOnEnemies = __instance.CombatState?.GetOpponentsOf(__instance.Owner.Creature)
      .Where(e => e.IsHittable && e.HasPower<LockOnPower>())
      .ToList();

    if (lockedOnEnemies != null && lockedOnEnemies.Count > 0)
    {
      target = __instance.Owner.RunState.Rng.CombatTargets.NextItem(lockedOnEnemies);
    }
  }

  [HarmonyPostfix]
  public static void Postfix(bool __state)
  {
    OrbDamageContext.IsOrbDealingDamage = __state;
  }
}

/// <summary>
/// 暗黑球锁定补丁：
/// 若场上有带有跟踪锁定（LockOnPower）的敌人，暗黑球激发时优先命中该敌人（多个时取当前生命最低者）。
/// 同时在伤害执行期间显式标记充能球伤害上下文。
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
    orb.ActivateEvoke([target]);
    OrbDamageContext.IsOrbDealingDamage = true;
    try
    {
      await CreatureCmd.Damage(playerChoiceContext, target, orb.EvokeVal, ValueProp.Unpowered, orb.Owner.Creature);
    }
    finally
    {
      OrbDamageContext.IsOrbDealingDamage = false;
    }
    return [target];
  }
}

/// <summary>
/// 玻璃球伤害上下文补丁：
/// 拦截玻璃球的 Passive 与 Evoke，在其同步伤害结算前标记 OrbDamageContext.IsOrbDealingDamage = true。
/// </summary>
[HarmonyPatch]
public static class GlassOrbDamageContextPatch
{
  [HarmonyPatch(typeof(GlassOrb), nameof(GlassOrb.Passive))]
  [HarmonyPrefix]
  public static void PrefixPassive(out bool __state)
  {
    __state = OrbDamageContext.IsOrbDealingDamage;
    OrbDamageContext.IsOrbDealingDamage = true;
  }

  [HarmonyPatch(typeof(GlassOrb), nameof(GlassOrb.Passive))]
  [HarmonyPostfix]
  public static void PostfixPassive(bool __state)
  {
    OrbDamageContext.IsOrbDealingDamage = __state;
  }

  [HarmonyPatch(typeof(GlassOrb), nameof(GlassOrb.Evoke))]
  [HarmonyPrefix]
  public static void PrefixEvoke(out bool __state)
  {
    __state = OrbDamageContext.IsOrbDealingDamage;
    OrbDamageContext.IsOrbDealingDamage = true;
  }

  [HarmonyPatch(typeof(GlassOrb), nameof(GlassOrb.Evoke))]
  [HarmonyPostfix]
  public static void PostfixEvoke(bool __state)
  {
    OrbDamageContext.IsOrbDealingDamage = __state;
  }
}
