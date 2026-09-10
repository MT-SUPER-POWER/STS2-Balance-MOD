using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Random;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Extensions;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Animations;
using Sts2BalanceMod.Sts2BalanceModCode.Runtime.Combat;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// EVENT-COLOSSEUM-01 — 红奴隶贩子 (SlaverRed)。
/// 首回合刺击，后续概率施展不可打出攻击牌的缠绕捕网，并伴有易伤刮击。
/// </summary>
[RegisterMonster]
public sealed class SlaverRed : BalanceMonsterTemplate
{
    public override MonsterAssetProfile AssetProfile => new(
      ModAssetPaths.Resource("monsters", "slaver_red", "slaver_red.tscn"));

    public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 48, 46);
    public override int MaxInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 52, 50);

    private static int StabDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 14, 13);
    private static int ScrapeDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 9, 8);
    private static int VulnerableAmount => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 2, 1);

    private const string STAB = "STAB";
    private const string ENTANGLE = "ENTANGLE";
    private const string SCRAPE = "SCRAPE";

    private bool _usedEntangle;
    protected override string AttackSfx => "event:/sfx/enemy/enemy_attacks/gremlin_merc/sneaky_gremlin_attack";

    protected override MonsterMoveStateMachine GenerateMoveStateMachine()
    {
        List<MonsterState> states = [];

        MoveState stabState = new(
          STAB,
          Stab,
          [new SingleAttackIntent(StabDamage)]
        );

        MoveState entangleState = new(
          ENTANGLE,
          Entangle,
          [new CardDebuffIntent()]
        );

        MoveState scrapeState = new(
          SCRAPE,
          Scrape,
          [new SingleAttackIntent(ScrapeDamage), new DebuffIntent()]
        );

        RngConditionalBranchState moveBranch = new("MOVE_BRANCH", SelectNextMove);

        stabState.FollowUpState = moveBranch;
        entangleState.FollowUpState = moveBranch;
        scrapeState.FollowUpState = moveBranch;

        states.Add(stabState);
        states.Add(entangleState);
        states.Add(scrapeState);
        states.Add(moveBranch);

        // 首回合固定使用刺击
        return new MonsterMoveStateMachine(states, stabState);
    }

    private string SelectNextMove(Creature owner, Rng rng, MonsterMoveStateMachine stateMachine)
    {
        int num = rng.NextInt(100);

        // 若尚未施放过缠绕，有 25% 概率施放缠绕
        if (num >= 75 && !_usedEntangle)
        {
            return ENTANGLE;
        }

        // 已施放缠绕或未命中缠绕判定，若未连续刺击两次，有 20% 概率刺击
        if (num >= 55 && _usedEntangle && !LastTwoMoves(stateMachine, STAB))
        {
            return STAB;
        }

        // 否则若上一回合未刮击，使用刮击
        if (!LastMove(stateMachine, SCRAPE))
        {
            return SCRAPE;
        }

        return STAB;
    }

    private async Task Stab(IReadOnlyList<Creature> targets)
    {
        await FastAttackAnimation.Play(Creature, async () =>
        {
            await DamageCmd.Attack(StabDamage)
              .FromMonster(this)
              .WithNoAttackerAnim()
              .WithHitFx("vfx/vfx_attack_slash", tmpSfx: TmpSfx.slashAttack)
              .Execute(null);
        });
    }

    private async Task Entangle(IReadOnlyList<Creature> targets)
    {
        await CreatureCmd.TriggerAnim(Creature, "UseNet", 0.0f);
        await Cmd.Wait(0.2f);

        foreach (Creature target in targets.Where(t => t.IsAlive))
        {
            await PowerCmd.Apply<TangledPower>(new ThrowingPlayerChoiceContext(), target, 1, Creature, null);
        }

        _usedEntangle = true;
    }

    private async Task Scrape(IReadOnlyList<Creature> targets)
    {
        await FastAttackAnimation.Play(Creature, async () =>
        {
            await DamageCmd.Attack(ScrapeDamage)
              .FromMonster(this)
              .WithNoAttackerAnim()
              .WithHitFx("vfx/vfx_attack_slash", tmpSfx: TmpSfx.slashAttack)
              .Execute(null);
        });

        foreach (Creature target in targets.Where(t => t.IsAlive))
        {
            await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), target, VulnerableAmount, Creature, null);
        }
    }

    public override CreatureAnimator GenerateAnimator(MegaSprite controller)
    {
        AnimState idle = new("idle", true);
        AnimState idleNoNet = new("idleNoNet", true);

        CreatureAnimator animator = new(idle, controller);
        animator.AddAnyState("UseNet", idleNoNet);

        return animator;
    }
}
