using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Powers;
using Sts2BalanceMod.Sts2BalanceModCode.Utility;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// STS1-BOSS-01 - 时间吞噬者怪物模型。
/// 输入：玩家出牌、怪物回合与自身当前生命值。
/// 输出：执行攻击、减益、格挡与半血 Haste 转阶段，并通过 TimeWarpPower 限制每回合出牌数。
/// </summary>
public sealed class TimeEater : Sts2MonsterModel
{
  private const string Reverberate = "REVERBERATE";
  private const string Ripple = "RIPPLE";
  private const string HeadSlam = "HEAD_SLAM";
  private const string Haste = "HASTE";

  private const int ReverberateHits = 3;
  private const int RippleBlock = 20;
  private const int DebuffTurns = 1;
  private const int DrawReductionAmount = 2;
  private const int SlimedCount = 2;
  private const string TimeWarpSfx = "res://Sts2BalanceMod/sfx/time_eater/time_warp.ogg";

  private static readonly LocString HasteDialog =
    L10NMonsterLookup("STS2BALANCEMOD-TIME_EATER.banter.haste");

  private static readonly LocString IntroDialog =
    L10NMonsterLookup("STS2BALANCEMOD-TIME_EATER.banter.intro");

  private bool _usedHaste;
  private bool _firstTurn = true;

  protected override string VisualsPath => "res://Sts2BalanceMod/monsters/time_eater/time_eater.tscn";

  protected override string AttackSfx => "event:/sfx/enemy/enemy_attacks/punch_construct/punch_construct_attack_single";

  public override int MinInitialHp => AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 480, 456);

  public override int MaxInitialHp => MinInitialHp;

  private int ReverberateDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 8, 7);

  private int HeadSlamDamage => AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 32, 26);

  private bool HasDoubleBossAscension => AscensionHelper.HasAscension(AscensionLevel.DoubleBoss);

  private bool UsedHaste
  {
    get => _usedHaste;
    set
    {
      AssertMutable();
      _usedHaste = value;
    }
  }

  private bool FirstTurn
  {
    get => _firstTurn;
    set
    {
      AssertMutable();
      _firstTurn = value;
    }
  }

  public override async Task AfterAddedToRoom()
  {
    await base.AfterAddedToRoom();
    await PowerCmd.Apply<TimeWarpPower>(new ThrowingPlayerChoiceContext(), Creature, 12M, Creature, null);
  }

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    var reverberateState = new MoveState(
      Reverberate,
      ReverberateMove,
      new AbstractIntent[] { new MultiAttackIntent(ReverberateDamage, ReverberateHits) });
    var rippleState = new MoveState(
      Ripple,
      RippleMove,
      new AbstractIntent[] { new DefendIntent(), new DebuffIntent() });
    var headSlamIntents = HasDoubleBossAscension
      ? new AbstractIntent[] { new SingleAttackIntent(HeadSlamDamage), new DebuffIntent(), new StatusIntent(SlimedCount) }
      : [new SingleAttackIntent(HeadSlamDamage), new DebuffIntent()];
    var headSlamState = new MoveState(
      HeadSlam,
      HeadSlamMove,
      headSlamIntents);
    var hasteState = new MoveState(
      Haste,
      HasteMove,
      new AbstractIntent[] { new BuffIntent() });
    var randomBranch = new RandomBranchState("RANDOM_BRANCH");
    randomBranch.AddBranch(reverberateState, 2, 45f);
    randomBranch.AddBranch(headSlamState, MoveRepeatType.CannotRepeat, 35f);
    randomBranch.AddBranch(rippleState, MoveRepeatType.CannotRepeat, 20f);

    var moveBranch = new ConditionalBranchState("MOVE_BRANCH");
    moveBranch.AddState(hasteState, ShouldUseHaste);
    moveBranch.AddState(randomBranch, () => true);

    reverberateState.FollowUpState = moveBranch;
    rippleState.FollowUpState = moveBranch;
    headSlamState.FollowUpState = moveBranch;
    hasteState.FollowUpState = moveBranch;

    return new MonsterMoveStateMachine(
      [reverberateState, rippleState, headSlamState, hasteState, randomBranch, moveBranch],
      moveBranch);
  }

  private bool ShouldUseHaste()
  {
    // NOTE: 转阶段发生在第一次低于半血后的下一次选招，强制执行 HASTE。
    if (Creature.CurrentHp >= Creature.MaxHp / 2M || UsedHaste)
    {
      return false;
    }

    UsedHaste = true;
    return true;
  }

  private async Task PlayIntroIfFirstTurn()
  {
    // NOTE: 在 Bestiary 中时，招式预览不存在"第一回合"概念，每次播放都应该显示开场白。
    if (!FirstTurn)
      return;

    var creature = Creature;
    if (creature == null)
      return;

    FirstTurn = false;

    // NOTE: 检测是否为 Bestiary 模式（使用 NullCombatState），是则不消耗 _firstTurn，
    //       让每次招式预览都能显示开场台词。
    if (creature.CombatState is NullCombatState)
    {
      TalkCmd.Play(IntroDialog, creature, VfxColor.Purple, VfxDuration.VeryLong);
      FirstTurn = true; // 确保下次招式预览还能触发
      return;
    }

    TalkCmd.Play(IntroDialog, creature, VfxColor.Purple, VfxDuration.VeryLong);
    await Cmd.Wait(0.5f);
  }

  private async Task ReverberateMove(IReadOnlyList<Creature> targets)
  {
    await PlayIntroIfFirstTurn();
    await DamageCmd.Attack(ReverberateDamage).WithHitCount(ReverberateHits).OnlyPlayAnimOnce()
      .FromMonster(this)
      .WithAttackerAnim("Attack", 0.4f)
      .WithAttackerFx(null, AttackSfx)
      .WithHitFx("vfx/vfx_attack_blunt")
      .Execute(null);
  }

  private async Task RippleMove(IReadOnlyList<Creature> targets)
  {
    await PlayIntroIfFirstTurn();
    await CreatureCmd.GainBlock(Creature, RippleBlock, ValueProp.Move, null);

    foreach (var target in targets.Where(t => t.IsAlive))
    {
      await PowerCmd.Apply<VulnerablePower>(new ThrowingPlayerChoiceContext(), target, DebuffTurns, Creature, null);
      await PowerCmd.Apply<WeakPower>(new ThrowingPlayerChoiceContext(), target, DebuffTurns, Creature, null);
    }
  }

  private async Task HeadSlamMove(IReadOnlyList<Creature> targets)
  {
    await PlayIntroIfFirstTurn();
    await CreatureCmd.TriggerAnim(Creature, "Slam", 0.4f);
    Sts2ModAudio.PlayOneShot(TimeWarpSfx, 0.8f);
    VfxCmd.PlayOnCreatureCenters(targets.Where(t => t.IsAlive), VfxCmd.gazePath);
    await DamageCmd.Attack(HeadSlamDamage).FromMonster(this)
      .WithHitFx("vfx/vfx_slime_impact", null, "heavy_attack.mp3")
      .Execute(null);

    foreach (var target in targets.Where(t => t.IsAlive))
    {
      await PowerCmd.Apply<DrawReductionPower>(new ThrowingPlayerChoiceContext(), target, DrawReductionAmount, Creature, null);
    }

    if (HasDoubleBossAscension)
    {
      NDebugAudioManager.Instance?.Play("card_deal.mp3", 0.45f, PitchVariance.Small);
      await CardPileCmd.AddToCombatAndPreview<Slimed>(targets, PileType.Discard, SlimedCount, (Player?)null);
    }
  }

  private async Task HasteMove(IReadOnlyList<Creature> targets)
  {
    await PlayIntroIfFirstTurn();
    TalkCmd.Play(HasteDialog, Creature, VfxColor.Purple, VfxDuration.VeryLong);

    var debuffs = Creature.Powers.Where(power => power.Type == PowerType.Debuff).ToList();
    foreach (var debuff in debuffs)
    {
      await PowerCmd.Remove(debuff);
    }

    var healAmount = Creature.MaxHp / 2M - Creature.CurrentHp;
    if (healAmount > 0M)
    {
      await CreatureCmd.Heal(Creature, healAmount);
    }

    if (HasDoubleBossAscension)
    {
      await CreatureCmd.GainBlock(Creature, HeadSlamDamage, ValueProp.Move, null);
    }
  }

  public override CreatureAnimator GenerateAnimator(MegaSprite controller)
  {
    var idle = new AnimState("Idle", true);
    var attack = new AnimState("Attack");
    var hit = new AnimState("Hit");

    attack.NextState = idle;
    hit.NextState = idle;

    var animator = new CreatureAnimator(idle, controller);
    animator.AddAnyState("Attack", attack);
    animator.AddAnyState("Slam", attack);
    animator.AddAnyState("Hit", hit);
    controller.GetAnimationState().SetTimeScale(0.8f);

    return animator;
  }

  protected override string? GetBestiaryMoveAnimationId(string moveStateId)
  {
    return moveStateId switch
    {
      Reverberate or HeadSlam or Haste => "Attack",
      _ => base.GetBestiaryMoveAnimationId(moveStateId),
    };
  }
}
