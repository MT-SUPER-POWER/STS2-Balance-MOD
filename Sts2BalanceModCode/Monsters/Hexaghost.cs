using Godot;
using MegaCrit.Sts2.Core.Animation;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Audio.Debug;
using MegaCrit.Sts2.Core.Bindings.MegaSpine;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Ascension;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.MonsterMoves.Intents;
using MegaCrit.Sts2.Core.MonsterMoves.MonsterMoveStateMachine;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.Nodes.Vfx.Utilities;
using MegaCrit.Sts2.Core.Random;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using Sts2BalanceMod.Sts2BalanceModCode.Animations;
using Sts2BalanceMod.Sts2BalanceModCode.Effects;
using Sts2BalanceMod.Sts2BalanceModCode.Patches.Monsters;
using Sts2BalanceMod.Sts2BalanceModCode.Utility;

namespace Sts2BalanceMod.Sts2BalanceModCode.Monsters;

/// <summary>
/// AFP-BOSS-02 — STS1 Hexaghost Boss。
/// 先根据队伍存活角色的平均当前生命动态计算 Divider，之后按六枚火球数量执行固定循环。
/// </summary>
public sealed class Hexaghost : Sts2MonsterModel
{
  private const string ActivateMove = "ACTIVATE";
  private const string DividerMove = "DIVIDER";
  private const string TackleMove = "TACKLE";
  private const string InflameMove = "INFLAME";
  private const string SearMove = "SEAR";
  private const string InfernoMove = "INFERNO";

  private const int SearDamage = 6;
  private const int FireTackleCount = 2;
  private const int InfernoHits = 6;
  private const int StrengthenBlock = 12;
  private const int InfernoBurnCount = 3;

  private bool _burnUpgraded;
  private int _orbActiveCount;
  private int _dividerDamage;
  private HexaghostVisuals? _visuals;

  protected override string VisualsPath =>
    "res://Sts2BalanceMod/monsters/hexaghost/hexaghost.tscn";

  public override int MinInitialHp =>
    AscensionHelper.GetValueIfAscension(AscensionLevel.ToughEnemies, 264, 250);

  public override int MaxInitialHp => MinInitialHp;

  private int InfernoDamage =>
    AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

  private int FireTackleDamage =>
    AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 6, 5);

  private int StrengthAmount =>
    AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 3, 2);

  private int SearBurnCount =>
    AscensionHelper.GetValueIfAscension(AscensionLevel.DeadlyEnemies, 2, 1);

  public override async Task AfterAddedToRoom()
  {
    await base.AfterAddedToRoom();
    _burnUpgraded = false;
    _orbActiveCount = 0;
    _dividerDamage = 0;

    var creatureNode = NCombatRoom.Instance?.GetCreatureNode(Creature);
    if (creatureNode != null)
      _visuals = new HexaghostVisuals(Creature, creatureNode);
  }

  protected override MonsterMoveStateMachine GenerateMoveStateMachine()
  {
    var activateState = new MoveState(
      ActivateMove,
      Activate,
      [new UnknownIntent()]);
    var dividerState = new MoveState(
      DividerMove,
      Divider,
      [new HexaghostDynamicMultiAttackIntent(() => _dividerDamage, InfernoHits)]);
    var tackleState = new MoveState(
      TackleMove,
      Tackle,
      [new MultiAttackIntent(FireTackleDamage, FireTackleCount)]);
    var inflameState = new MoveState(
      InflameMove,
      Inflame,
      [new DefendIntent(), new BuffIntent()]);
    var searState = new MoveState(
      SearMove,
      Sear,
      [new SingleAttackIntent(SearDamage), new StatusIntent(SearBurnCount)]);
    var infernoState = new MoveState(
      InfernoMove,
      Inferno,
      [new MultiAttackIntent(InfernoDamage, InfernoHits), new DebuffIntent()]);
    var moveBranch = new RngConditionalBranchState("MOVE_BRANCH", SelectNextMove);

    activateState.FollowUpState = dividerState;
    dividerState.FollowUpState = moveBranch;
    tackleState.FollowUpState = moveBranch;
    inflameState.FollowUpState = moveBranch;
    searState.FollowUpState = moveBranch;
    infernoState.FollowUpState = moveBranch;

    return new MonsterMoveStateMachine(
      [activateState, dividerState, tackleState, inflameState, searState, infernoState, moveBranch],
      activateState);
  }

  private string SelectNextMove(Creature owner, Rng rng, MonsterMoveStateMachine stateMachine)
  {
    return _orbActiveCount switch
    {
      0 => SearMove,
      1 => TackleMove,
      2 => SearMove,
      3 => InflameMove,
      4 => TackleMove,
      5 => SearMove,
      6 => InfernoMove,
      _ => SearMove,
    };
  }

  private Task Activate(IReadOnlyList<Creature> targets)
  {
    _orbActiveCount = 6;
    _visuals?.ActivateAllOrbs();
    _visuals?.SetTargetRotationSpeed(120f);

    var livingTargets = targets.Where(target => target.IsAlive).ToList();
    var averageHp = livingTargets.Count > 0 ? livingTargets.Average(target => target.CurrentHp) : 1d;
    _dividerDamage = (int)(averageHp / 12d) + 1;
    return Task.CompletedTask;
  }

  private async Task Divider(IReadOnlyList<Creature> targets)
  {
    for (var i = 0; i < InfernoHits; i++)
    {
      await Cmd.Wait(0.05f);
      await DamageCmd.Attack(_dividerDamage)
        .FromMonster(this)
        .WithAttackerFx(sfx: "event:/sfx/characters/attack_fire")
        .WithHitVfxNode(CreateGhostFireBurst)
        .Execute(null);
    }

    DeactivateAllOrbs();
  }

  public static Node2D? CreateGhostFireBurst(Creature target)
  {
    var creatureNode = NCombatRoom.Instance?.GetCreatureNode(target);
    if (creatureNode == null || !creatureNode.IsInteractable)
      return null;

    var vfx = PreloadManager.Cache.GetScene("scenes/vfx/vfx_fire_burst.tscn").Instantiate<Node2D>();
    vfx.GlobalPosition = creatureNode.VfxSpawnPosition;
    vfx.Modulate = new Color(0.455f, 0.918f, 0.027f, 1f);
    return vfx;
  }

  private async Task Tackle(IReadOnlyList<Creature> targets)
  {
    BorderFlashEffect.PlayChartreuse();
    await FastAttackAnimation.Play(Creature);
    await DamageCmd.Attack(FireTackleDamage)
      .WithHitCount(FireTackleCount)
      .FromMonster(this)
      .WithAttackerFx(sfx: "event:/sfx/characters/attack_fire")
      .WithHitVfxNode(CreateGhostFireBurst)
      .Execute(null);
    ActivateNextOrb();
  }

  private async Task Inflame(IReadOnlyList<Creature> targets)
  {
    NPowerUpVfx.CreateGhostly(Creature);
    await CreatureCmd.GainBlock(Creature, StrengthenBlock, ValueProp.Move, null);
    await PowerCmd.Apply<StrengthPower>(
      new ThrowingPlayerChoiceContext(),
      Creature,
      StrengthAmount,
      Creature,
      null);
    ActivateNextOrb();
  }

  private async Task Sear(IReadOnlyList<Creature> targets)
  {
    var playerCreature = targets.FirstOrDefault(target => target.Player != null);
    if (playerCreature != null)
    {
      var fireball = FireballEffect.Create(
        Sts1VfxHelper.GetCreatureCenter(Creature),
        Sts1VfxHelper.GetCreatureCenter(playerCreature));
      Sts1VfxHelper.Play(fireball);
      await Cmd.Wait(0.5f);
    }

    await DamageCmd.Attack(SearDamage)
      .FromMonster(this)
      .WithAttackerFx(sfx: "event:/sfx/characters/attack_fire")
      .WithHitVfxNode(CreateGhostFireBurst)
      .Execute(null);

    await AddBurnsToDiscard(targets, SearBurnCount);
    ActivateNextOrb();
  }

  private async Task Inferno(IReadOnlyList<Creature> targets)
  {
    Sts1VfxHelper.Play(ScreenOnFireEffect.Create());
    await Cmd.Wait(1f);

    await DamageCmd.Attack(InfernoDamage)
      .WithHitCount(InfernoHits)
      .FromMonster(this)
      .WithAttackerFx(sfx: "event:/sfx/characters/attack_fire")
      .WithHitVfxNode(CreateGhostFireBurst)
      .Execute(null);

    await UpgradeAllBurnsAndAddMore(targets);
    _burnUpgraded = true;
    DeactivateAllOrbs();
  }

  private void ActivateNextOrb()
  {
    _orbActiveCount++;
    _visuals?.ActivateNextOrb();
  }

  private void DeactivateAllOrbs()
  {
    _orbActiveCount = 0;
    _visuals?.DeactivateAllOrbs();
    NDebugAudioManager.Instance?.Play("card_exhaust.mp3");
    NDebugAudioManager.Instance?.Play("card_exhaust.mp3");
  }

  private async Task UpgradeAllBurnsAndAddMore(IReadOnlyList<Creature> targets)
  {
    HexaghostBurnUpgradePatch.AllowBurnUpgrade = true;
    try
    {
      foreach (var playerCreature in targets.Where(target => target.Player != null))
      {
        var player = playerCreature.Player!;
        var burnsToUpgrade = player.Piles
          .Where(pile => pile.Type is PileType.Draw or PileType.Discard or PileType.Hand)
          .SelectMany(pile => pile.Cards)
          .OfType<Burn>()
          .Where(burn => burn.IsUpgradable)
          .ToList();

        foreach (var burn in burnsToUpgrade)
        {
          burn.UpgradeInternal();
          burn.FinalizeUpgradeInternal();
        }

        await AddUpgradedBurns(playerCreature, InfernoBurnCount);
      }

      await Cmd.Wait(1f);
    }
    finally
    {
      HexaghostBurnUpgradePatch.AllowBurnUpgrade = false;
    }
  }

  private async Task AddBurnsToDiscard(IReadOnlyList<Creature> targets, int count)
  {
    if (!_burnUpgraded)
    {
      await CardPileCmd.AddToCombatAndPreview<Burn>(targets, PileType.Discard, count, (Player?)null);
      return;
    }

    HexaghostBurnUpgradePatch.AllowBurnUpgrade = true;
    try
    {
      foreach (var playerCreature in targets.Where(target => target.Player != null))
        await AddUpgradedBurns(playerCreature, count);

      await Cmd.Wait(1f);
    }
    finally
    {
      HexaghostBurnUpgradePatch.AllowBurnUpgrade = false;
    }
  }

  private static async Task AddUpgradedBurns(Creature playerCreature, int count)
  {
    var player = playerCreature.Player!;
    var statusCards = new CardPileAddResult[count];
    for (var i = 0; i < count; i++)
    {
      var burn = playerCreature.CombatState!.CreateCard<Burn>(player);
      burn.UpgradeInternal();
      burn.FinalizeUpgradeInternal();
      statusCards[i] = await CardPileCmd.AddGeneratedCardToCombat(burn, PileType.Discard, (Player?)null);
    }

    CardCmd.PreviewCardPileAdd(
      statusCards,
      style: count > 5 ? CardPreviewStyle.MessyLayout : CardPreviewStyle.HorizontalLayout);
  }

  public override async Task AfterDeath(
    PlayerChoiceContext choiceContext,
    Creature creature,
    bool wasRemovalPrevented,
    float deathAnimLength)
  {
    await base.AfterDeath(choiceContext, creature, wasRemovalPrevented, deathAnimLength);
    if (creature != Creature)
      return;

    _visuals?.HideAllOrbs();
    _visuals?.Dispose();
    _visuals = null;
    NGame.Instance?.ScreenShake(ShakeStrength.Strong, ShakeDuration.Long);
  }
}
