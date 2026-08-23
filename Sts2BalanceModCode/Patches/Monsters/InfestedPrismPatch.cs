using System.Collections.Generic;
using System.Threading.Tasks;
using HarmonyLib;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;
using Sts2BalanceMod.Sts2BalanceModCode.Settings;

namespace Sts2BalanceMod.Sts2BalanceModCode.Patches.Monsters;

/// <summary>
/// 目标类型：MegaCrit.Sts2.Core.Models.Monsters.InfestedPrism
/// 目标方法：AfterAddedToRoom, GenerateMoveStateMachine
/// 修改原因：BOSS-01 感染棱柱重构。移除原有的活力火花（VitalSparkPower）开场赋予，并将其行动循环重构为固定4回合的【轻击】->【重击】->【连击】->【强化】。
/// 警告：依赖反编译源码 MegaCrit.Sts2.Core.Models.Monsters.InfestedPrism
/// </summary>
[HarmonyPatch(typeof(InfestedPrism))]
public static class InfestedPrismPatch
{
    [HarmonyPatch(nameof(InfestedPrism.AfterAddedToRoom))]
    [HarmonyPrefix]
    public static bool AfterAddedToRoomPrefix(InfestedPrism __instance, ref Task __result)
    {
        // CONFIG-02: 关闭重做时执行原版方法，恢复 VitalSparkPower 开场机制。
        if (!BalanceModSettings.InfestedPrismReworkEnabled)
        {
            return true;
        }

        // 阻止原版在 AfterAddedToRoom 中对玩家赋予 VitalSparkPower
        __result = Task.CompletedTask;
        return false;
    }

    [HarmonyPatch("GenerateMoveStateMachine")]
    [HarmonyPrefix]
    public static bool GenerateMoveStateMachinePrefix(InfestedPrism __instance, ref MonsterMoveStateMachine __result)
    {
        // CONFIG-02: 与开场能力使用同一个开关，确保关闭时完整恢复原版行动状态机。
        if (!BalanceModSettings.InfestedPrismReworkEnabled)
        {
            return true;
        }

        int lightDamage = AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 14, 12);
        int heavyDamage = AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 22, 20);
        int multiDamage = AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);

        MoveState move1 = new MoveState("LIGHT_ATTACK_MOVE", targets => LightAttackMove(__instance, targets), new SingleAttackIntent(lightDamage), new DebuffIntent());
        MoveState move2 = new MoveState("HEAVY_ATTACK_MOVE", targets => HeavyAttackMove(__instance, targets), new SingleAttackIntent(heavyDamage), new DefendIntent());
        MoveState move3 = new MoveState("MULTI_ATTACK_MOVE", targets => MultiAttackMove(__instance, targets), new MultiAttackIntent(multiDamage, 3));
        MoveState move4 = new MoveState("FORTIFY_MOVE", targets => FortifyMove(__instance, targets), new DefendIntent(), new BuffIntent());

        move1.FollowUpState = move2;
        move2.FollowUpState = move3;
        move3.FollowUpState = move4;
        move4.FollowUpState = move1;

        List<MonsterState> states = new List<MonsterState> { move1, move2, move3, move4 };
        __result = new MonsterMoveStateMachine(states, move1);
        return false;
    }

    private static async Task LightAttackMove(InfestedPrism prism, IReadOnlyList<Creature> targets)
    {
        int lightDamage = AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 14, 12);
        AttackCommand attack = await DamageCmd.Attack(lightDamage).FromMonster(prism)
          .WithAttackerAnim("Attack", 0.1f)
          .WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/infested_prisms/infested_prisms_attack")
          .WithHitFx("vfx/vfx_attack_slash")
          .Execute(null);

        await ApplyInfectionFromAttack(attack, prism.Creature);

        foreach (Creature target in targets)
        {
            await PowerCmd.Apply<FrailPower>(new ThrowingPlayerChoiceContext(), target, 1m, prism.Creature, null);
        }
    }

    private static async Task HeavyAttackMove(InfestedPrism prism, IReadOnlyList<Creature> targets)
    {
        int heavyDamage = AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 22, 20);
        int heavyBlock = AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 12, 10);

        AttackCommand attack = await DamageCmd.Attack(heavyDamage).FromMonster(prism)
          .WithAttackerAnim("AttackBlock", 0.25f)
          .WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/infested_prisms/infested_prisms_attack_defend")
          .WithHitFx("vfx/vfx_attack_slash")
          .Execute(null);

        await ApplyInfectionFromAttack(attack, prism.Creature);

        await CreatureCmd.GainBlock(prism.Creature, heavyBlock, ValueProp.Move, null);
    }

    private static async Task MultiAttackMove(InfestedPrism prism, IReadOnlyList<Creature> targets)
    {
        int multiDamage = AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 7, 6);
        AttackCommand attack = await DamageCmd.Attack(multiDamage).WithHitCount(3).FromMonster(prism)
          .WithAttackerAnim("AttackDouble", 0.2f)
          .OnlyPlayAnimOnce()
          .WithAttackerFx(null, "event:/sfx/enemy/enemy_attacks/infested_prisms/infested_prisms_attack_spin")
          .WithHitFx("vfx/vfx_attack_slash")
          .Execute(null);

        await ApplyInfectionFromAttack(attack, prism.Creature);
    }

    private static async Task FortifyMove(InfestedPrism prism, IReadOnlyList<Creature> targets)
    {
        int fortifyBlock = AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 18, 16);
        int fortifyStrength = AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

        await CreatureCmd.GainBlock(prism.Creature, fortifyBlock, ValueProp.Move, null);
        await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), prism.Creature, fortifyStrength, prism.Creature, null);
    }

    private static async Task ApplyInfectionFromAttack(AttackCommand attack, Creature dealer)
    {
        if (attack?.Results == null)
        {
            return;
        }

        foreach (List<DamageResult> hitResults in attack.Results)
        {
            // Each outer list represents one actual attack hit. A hit that crosses a
            // creature's Block can produce multiple DamageResults for that creature,
            // but it must still apply Infection only once.
            var infectedThisHit = new HashSet<Creature>();

            foreach (DamageResult result in hitResults)
            {
                if (result.Receiver != null
                    && result.Receiver.IsPlayer
                    && result.UnblockedDamage > 0
                    && infectedThisHit.Add(result.Receiver))
                {
                    await PowerCmd.Apply<InfectedPower>(
                      new ThrowingPlayerChoiceContext(),
                      result.Receiver,
                      InfectedPower.InfectionPerHit,
                      dealer,
                      null);
                }
            }
        }
    }
}
