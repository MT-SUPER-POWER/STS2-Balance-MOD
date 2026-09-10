using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Animations;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// EVENT-COLOSSEUM-01 — 奴隶贩子头目 (Taskmaster)。
/// 永续释放痛击之鞭，往玩家弃牌堆塞入伤口牌并在高进阶下不断积累力量。
/// </summary>
[RegisterMonster]
public sealed class Taskmaster : BalanceMonsterTemplate
{
    public override MonsterAssetProfile AssetProfile => new(
      ModAssetPaths.Resource("monsters", "taskmaster", "taskmaster.tscn"));

    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 57, 54);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 64, 60);

    private const int ScouringWhipDamage = 7;
    private static int WoundCount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 1);
    private static bool GainsStrength => AscensionHelper.HasAscension(AscensionLevel.DeadlyEnemies);

    private const string SCOURING_WHIP = "SCOURING_WHIP";
    protected override string AttackSfx => "event:/sfx/enemy/enemy_attacks/gremlin_merc/sneaky_gremlin_attack";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        MoveState scouringWhipState = new(
          SCOURING_WHIP,
          ScouringWhip,
          [new SingleAttackIntent(ScouringWhipDamage), new StatusIntent(WoundCount)]
        );

        scouringWhipState.FollowUpState = scouringWhipState;

        return new MonsterMoveStateMachine(
          [scouringWhipState],
          scouringWhipState
        );
    }

    private async Task ScouringWhip(IReadOnlyList<Creature> targets)
    {
        await FastAttackAnimation.Play(Creature, async () =>
        {
            await DamageCmd.Attack(ScouringWhipDamage)
              .FromMonster(this)
              .WithNoAttackerAnim()
              .WithHitFx("vfx/vfx_attack_slash", tmpSfx: TmpSfx.slashAttack)
              .Execute(null);
        });

        await CardPileCmd.AddToCombatAndPreview<Wound>(targets, PileType.Discard, WoundCount, (Player?)null);

        if (GainsStrength)
        {
            await PowerCmd.Apply<StrengthPower>(new ThrowingPlayerChoiceContext(), Creature, 1, Creature, null);
        }
    }

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        AnimState idle = new("idle", true);
        return new CreatureAnimator(idle, controller);
    }
}
