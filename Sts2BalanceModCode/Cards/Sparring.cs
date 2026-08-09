using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Monsters;
using MegaCrit.Sts2.Core.ValueProps;
using Sts2BalanceMod.Sts2BalanceModCode.Abstract;
using STS2RitsuLib.Interop.AutoRegistration;

namespace Sts2BalanceMod.Sts2BalanceModCode.Cards;

/// <summary>
/// CARD-NEW — 比试
/// 2费 | 攻击 | 罕见 | 消耗
/// 奥斯提造成7/9点伤害。造成8点伤害。造成伤害较少的一方回复4/6点生命。
/// </summary>
[RegisterCard(typeof(NecrobinderCardPool), FullPublicEntry = "STS2_BALANCEMOD_SPARRING")]
public sealed class Sparring : BalanceCardTemplate
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    public Sparring()
        : base(2, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
    {
    }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
      new OstyDamageVar(7M, ValueProp.Move),
    new DamageVar(8M, ValueProp.Move),
    new HealVar(4M),
  ];

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        int ostyDmg = 0;
        int playerDmg = 0;

        // 1. Osty deals 7 (or 9) damage. If Osty is missing, deals 0 damage and triggers missing animation.
        if (Owner.IsOstyAlive && Owner.Osty != null)
        {
            await CreatureCmd.TriggerAnim(Owner.Osty, "attack_poke", Osty.attackerAnimDelay);
            var ostyResult = await DamageCmd.Attack(DynamicVars.OstyDamage.BaseValue)
                .FromOsty(Owner.Osty, this, cardPlay)
                .Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_blunt", null, Osty.ostyAttackSfx)
                .Execute(choiceContext);
            ostyDmg = ostyResult.Results.SelectMany(r => r).Sum(r => r.UnblockedDamage);
        }
        else
        {
            Osty.CheckMissingWithAnim(Owner);
        }

        // 2. Player deals 8 damage.
        await CreatureCmd.TriggerAnim(Owner.Creature, "Cast", Owner.Character.CastAnimDelay);
        var playerResult = await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this, cardPlay)
            .Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", null, "event:/sfx/characters/attack_fire")
            .Execute(choiceContext);
        playerDmg = playerResult.Results.SelectMany(r => r).Sum(r => r.UnblockedDamage);

        // 3. Side dealing less damage heals 4 (or 6).
        decimal healVal = DynamicVars.Heal.BaseValue;
        if (ostyDmg < playerDmg)
        {
            if (Owner.IsOstyAlive && Owner.Osty != null)
            {
                await CreatureCmd.Heal(Owner.Osty, healVal);
            }
        }
        else if (playerDmg < ostyDmg)
        {
            await CreatureCmd.Heal(Owner.Creature, healVal);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.OstyDamage.UpgradeValueBy(2M);
        DynamicVars.Heal.UpgradeValueBy(2M);
    }
}
