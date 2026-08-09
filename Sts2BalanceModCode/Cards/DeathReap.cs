using STS2RitsuLib.Interop.AutoRegistration;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// LEGACY-03 — 死亡收割
/// 2费 | 攻击 | 消耗 | 造成 4 点伤害，回复等量于非格挡伤害的生命
/// 升级：伤害 4→6
/// </summary>
[RegisterCard(typeof(IroncladCardPool), FullPublicEntry = "STS2_BALANCEMOD_DEATH_REAP")]
public sealed class DeathReap : BalanceCardTemplate
{
  public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

  protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4M, ValueProp.Move)];

  public DeathReap() : base(2, CardType.Attack, CardRarity.Rare, TargetType.AllEnemies) { }

  protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
  {
    var result = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
        .FromCard(this, cardPlay)
        .TargetingAllOpponents(CombatState!)
        .WithHitFx("vfx/vfx_giant_horizontal_slash")
        .Execute(choiceContext);

    int healed = result.Results
        .SelectMany(r => r)
        .Sum(r => r.UnblockedDamage);

    if (healed > 0)
      await CreatureCmd.Heal(Owner.Creature, healed);
  }

  protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(2M);
}
