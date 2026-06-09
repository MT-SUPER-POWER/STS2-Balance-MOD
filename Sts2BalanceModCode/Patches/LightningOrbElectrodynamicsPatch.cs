using System.Reflection;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches;

/// <summary>
/// 电动力学：闪电球被动/激发攻击所有敌人（原版为随机单体，现覆盖 TeslaCoil 等指定目标场景）
/// </summary>
[HarmonyPatch(typeof(LightningOrb), "ApplyLightningDamage")]
public static class LightningOrbElectrodynamicsPatch
{
  private static readonly MethodInfo PlayEvokeSfx =
      AccessTools.Method(typeof(OrbModel), "PlayEvokeSfx")!;

  [HarmonyPrefix]
  public static bool Prefix(
      LightningOrb __instance,
      decimal value,
      Creature? target,
      PlayerChoiceContext choiceContext,
      ref Task<IEnumerable<Creature>> __result)
  {
    // NOTE: 不检查 target != null，因为 TeslaCoil 等卡牌会传入指定目标，
    // 有电动力学时无论 target 是否为 null 都应攻击全体敌人
    if (!__instance.Owner.Creature.HasPower<ElectrodynamicsPower>()) return true;

    __result = HitAllEnemies(__instance, value, choiceContext);
    return false;
  }

  // FIXME: 原 BUG 已修复——根因是 TeslaCoil 传入指定 target 导致 patch 短路跳过群伤
  private static async Task<IEnumerable<Creature>> HitAllEnemies(
      LightningOrb orb,
      decimal value,
      PlayerChoiceContext choiceContext)
  {
    List<Creature> enemies = orb.CombatState.GetOpponentsOf(orb.Owner.Creature)
        .Where(e => e.IsHittable)
        .ToList();

    if (enemies.Count == 0)
      return [];

    foreach (Creature enemy in enemies)
      VfxCmd.PlayOnCreature(enemy, "vfx/vfx_attack_lightning");

    PlayEvokeSfx.Invoke(orb, null);

    await CreatureCmd.Damage(
        choiceContext,
        enemies,
        value,
        ValueProp.Unpowered,
        orb.Owner.Creature);

    return enemies;
  }
}
