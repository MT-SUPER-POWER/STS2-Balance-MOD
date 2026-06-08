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
/// 电动力学：闪电球被动/激发在无指定目标时攻击所有敌人（原版为随机单体）
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
    if (target != null) return true;
    if (!__instance.Owner.Creature.HasPower<ElectrodynamicsPower>()) return true;

    __result = HitAllEnemies(__instance, value, choiceContext);
    return false;
  }

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
