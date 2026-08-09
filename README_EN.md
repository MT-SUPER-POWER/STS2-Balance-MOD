<div align="center">
  <img alt="logo" height="100" width="100" src="docs/img/icon.ico" />
  <h2> Sts2BalanceMod </h2>
  <p> Sts2BalanceMod — Slay the Spire 2 Balance & Content Mod </p>
  <p>
    <a href="README.md"><img src="https://img.shields.io/badge/Language-%E7%AE%80%E4%BD%93%E4%B8%AD%E6%96%87-lightgrey?style=flat-square" alt="简体中文" /></a>
    <img src="https://img.shields.io/badge/Language-English-blue?style=flat-square" alt="English" />
  </p>
  <p>
    <img src="Assets/profile/ironclad.png" width="28" height="28" title="Ironclad" />
    <img src="Assets/profile/silent.png" width="28" height="28" title="Silent" />
    <img src="Assets/profile/regent.png" width="28" height="28" title="Regent" />
    <img src="Assets/profile/necrobinder.png" width="28" height="28" title="Necrobinder" />
    <img src="Assets/profile/defect.png" width="28" height="28" title="Defect" />
  </p>
  <a href="https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/stargazers">
    <img src="https://img.shields.io/github/stars/MT-SUPER-POWER/STS2-Balance-MOD?style=flat" alt="Stars" />
  </a>
  <a href="https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/releases">
    <img src="https://img.shields.io/github/v/release/MT-SUPER-POWER/STS2-Balance-MOD" alt="Version" />
  </a>
  <a href="https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/issues">
    <img src="https://img.shields.io/github/issues/MT-SUPER-POWER/STS2-Balance-MOD" alt="Issues" />
  </a>
  <a href="LICENSE">
    <img src="https://img.shields.io/github/license/MT-SUPER-POWER/STS2-Balance-MOD" alt="License" />
  </a>
</div>


## Installation

### Prerequisites

1. **[Slay the Spire 2](https://store.steampowered.com/app/2868840/Slay_the_Spire_2/)** Version ≥ 0.110.0
2. **[BaseLib](https://github.com/Alchyr/BaseLib-StS2)** — Mod loader dependency, must be installed first, Version ≥ 3.4.0+

### Installation Steps

1. Download the latest release package (`.zip`) from the [Releases](https://github.com/MT-SUPER-POWER/STS2-Balance-MOD/releases) page.
2. Extract the **entire folder** into the STS2 mods directory:
   - **Windows**: `%AppData%/SlayTheSpire2/mods/`
   - **macOS**: `~/Library/Application Support/SlayTheSpire2/mods/`
   - **Linux**: `~/.local/share/SlayTheSpire2/mods/`
3. Ensure `BaseLib` is also installed in the same `mods` directory.
4. Launch the game and confirm `Sts2BalanceMod` is checked in the Mod Manager menu.

---

## Balance & Content Adjustments

> [!note]
> The following list contains all implemented balance tweaks, reworks, and classic content returns in this Mod.
>
> For version release history, see **[CHANGELOG.md](CHANGELOG.md)**. For powers & status details, see **[docs/powers.md](docs/powers.md)**. For event details, see **[docs/events.md](docs/events.md)**.

### Mod Settings

This Mod is integrated into the game's native Mod Settings UI. Settings are auto-saved by BaseLib and restored across game sessions.

> [!warning]
> Multiplayer games do not auto-sync Mod settings. All players must use identical settings to avoid desynchronized event choices or battle states.

| Setting | Default | Effect |
| :--- | :---: | :--- |
| **Add "Leave" option to events** | Enabled | Controls whether an explicit "Leave" option is added to "Bug Eater", "Frankenstein", and "Future of Potions?". Changes take effect on next event entrance. |
| **Enable Infested Prism Rework** | Enabled | When enabled, uses fixed 4-turn rotation and [Infected] mechanic. When disabled, reverts to vanilla Spark of Vitality and state machine. |

### Shop

#### High Ascension Card Removal Cost (SHOP-01)

- **Vanilla**: A6+ Card removal always costs 50 Gold base (+25 per removal).
- **MOD Rework**: Ascension < 6 base 50 Gold (+25 per removal). **A6+ base 75 Gold** (+25 per removal).

### Card Adjustments

| Card | Character | Type | Vanilla | MOD Rework |
|------|:---:|:----:|------|------|
| **Dirge** | <img src="Assets/profile/necrobinder.png" width="22" height="22" title="Necrobinder"> | Power | Upgraded: Summon +1, Exhaust, Soul+ | Upgraded: **Gains Retain keyword** |
| **Blade Dance** | <img src="Assets/profile/silent.png" width="22" height="22" title="Silent"> | Skill | Common card, Exhausts on play | Removes Exhaust, rarity changed to **Uncommon**, reusable |
| **Acrobatics** | <img src="Assets/profile/silent.png" width="22" height="22" title="Silent"> | Skill | Uncommon card | Rarity lowered to **Common**, increasing offer chance |
| **Biased Cognition** | <img src="Assets/profile/defect.png" width="22" height="22" title="Defect"> | Power | -1 Focus per turn permanently | Focus loss auto-expires after specified turns without over-deducting Focus |
| **Multicast** | <img src="Assets/profile/defect.png" width="22" height="22" title="Defect"> | Skill | Upgraded: Evoke X+1 times | Upgrade changed to gain **Retain** keyword (removed vanilla X+1) |
| **Wraith Form** | <img src="Assets/profile/silent.png" width="22" height="22" title="Silent"> | Skill | Lose 1 Dexterity every turn while Intangible | Applies Intangible; **completely removes Dexterity loss drawback** |
| **Up My Sleeve** | <img src="Assets/profile/silent.png" width="22" height="22" title="Silent"> | Skill | Reduces cost on play, overlapping with Blade Dance | **Completely removed** from Silent card pool |
| **Fuel** | <img src="Assets/profile/defect.png" width="22" height="22" title="Defect"> | Skill | Transform Status cards into 2 Energy | Gain 1 Energy and **draw 1 card (Upgraded: 2)** |
| **Drain Power** | <img src="Assets/profile/necrobinder.png" width="22" height="22" title="Necrobinder"> | Attack | Deal 10/12 damage, upgraded randomly upgrades 3 discarded cards | Damage reduced to **6/8**; base upgrades **2** discard cards, upgraded upgrades **all** discard cards |
| **Pull Aggro** | <img src="Assets/profile/necrobinder.png" width="22" height="22" title="Necrobinder"> | Skill | Vanilla upgraded stats | Upgraded stats adjusted to: **Summon 6, Block 9** |
| **Wither** | <img src="Assets/map/aeonglass_boss.png" width="22" height="22" title="Aeonglass"> | Status | Unplayable, goes directly to Exhaust pile | Changed to **1-cost Playable**, Exhausts on play |
| **Glow** | <img src="Assets/profile/regent.png" width="22" height="22" title="Regent"> | Skill | Gain Stardust, draw extra card next turn | Changed to **immediately draw 2 cards** this turn; upgrade grants +1 Stardust |
| **Bloodletting** | <img src="Assets/profile/ironclad.png" width="22" height="22" title="Ironclad"> | Skill | Uncommon (v1.0.9 change) | Rarity reverted back to **Common** |
| **Pillar of Creation** | <img src="Assets/profile/regent.png" width="22" height="22" title="Regent"> | Power | Gain 5/8 Block on first card created each turn | Reverted: Gain **3/4 Block** every time a card is created |
| **Cruelty** | <img src="Assets/profile/ironclad.png" width="22" height="22" title="Ironclad"> | Power | Shifted to Ironclad Uncommon | **Completely removed** from Ironclad card pool (replaced by Evolve) |
| **Dowsing** | <img src="Assets/profile/neow.png" width="22" height="22" title="Neow"> | Quest | Transform into Abundance after 5 ? rooms | Adjusted to transform after **4 ? rooms** |
| **The Ball** | <img src="Assets/profile/colorless.png" width="22" height="22" title="Colorless"> | Attack | Scaling damage +10/15 per play | Damage scaling increased to **+15/20** |
| **The Scythe** | <img src="Assets/profile/necrobinder.png" width="22" height="22" title="Necrobinder"> | Attack | 2-cost, Exhaust; deal 13 damage, scaling +4/5 | Base damage increased to **16**, pairing with official +5/7 scaling |
| **Rocket Punch** | <img src="Assets/profile/defect.png" width="22" height="22" title="Defect"> | Attack | 2-cost; cost -1 when Status card generated | Reverted to legacy effect: Cost **drops directly to 0** when Status card is generated |
| **Accelerant** | <img src="Assets/profile/silent.png" width="22" height="22" title="Silent"> | Power | Uncommon card | Rarity reverted to **Rare** |
| **Well-Laid Plans** | <img src="Assets/profile/silent.png" width="22" height="22" title="Silent"> | Power | Rare, 1/0 cost; non-discard turn end | Uncommon, 2/1 cost; Retain up to **1/2** cards at turn end (stackable on multiple plays, filters existing Retain cards) |
| **Grand Finale** | <img src="Assets/profile/silent.png" width="22" height="22" title="Silent"> | Attack | 0-cost, requires exactly 0 cards in draw pile | **X-Cost** card, playable when **draw pile count ≤ X**; **Upgraded reduces cost by 2** (consumes $\max(0, X - 2)$ energy) |
| **Pinpoint** | <img src="Assets/profile/silent.png" width="22" height="22" title="Silent"> | Attack | Cost reduces based on skill count | **Completely removed** from Silent card pool (replaced by Eviscerate) |


### STS1 Classic Returned Cards

| Card | Character | Type | Rarity | Cost | Effect (Base / Upgraded) |
|------|:---:|:----:|:------:|:----:|------|
| **Death Reap** | <img src="Assets/profile/ironclad.png" width="22" height="22" title="Ironclad"> | Attack | Rare | 2 | Exhaust. Deal **4 / 6** damage to ALL enemies. Heal HP equal to unblocked damage dealt. |
| **Power Through** | <img src="Assets/profile/ironclad.png" width="22" height="22" title="Ironclad"> | Skill | Uncommon | 1 | Gain **15 / 20** Block. Add 2 Wounds into your hand. |
| **Evolve** | <img src="Assets/profile/ironclad.png" width="22" height="22" title="Ironclad"> | Power | Uncommon | 1 | Whenever you draw a Status card, draw **1 / 2** card(s). |
| **Concentrate** | <img src="Assets/profile/silent.png" width="22" height="22" title="Silent"> | Skill | Uncommon | 0 | Discard **3 / 2** cards. Gain 2 Energy. |
| **Eviscerate** | <img src="Assets/profile/silent.png" width="22" height="22" title="Silent"> | Attack | Uncommon | 3 | Costs 1 less for each card discarded this turn. Deal **7 / 9** damage 3 times. |
| **Electrodynamics** | <img src="Assets/profile/defect.png" width="22" height="22" title="Defect"> | Power | Rare | 2 | Channel **2 / 3** Lightning. Lightning Orbs now hit ALL enemies. |

### New Mod Cards

| Card | Character | Type | Rarity | Cost | Effect (Base / Upgraded) |
|------|:---:|:----:|:------:|:----:|------|
| **Sparring** | <img src="Assets/profile/necrobinder.png" width="22" height="22" title="Necrobinder"> | Attack | Uncommon | 2 | Exhaust. Deal **8** damage, Osty deals **7 / 9** damage. Whichever side deals less unblocked damage heals **4 / 6** HP. |
| **Ram** | <img src="Assets/profile/necrobinder.png" width="22" height="22" title="Necrobinder"> | Attack | Common | 2 | Osty loses **6 / 5** HP, deal **20 / 26** damage to ALL enemies; cannot trigger if Osty HP is insufficient. |
| **Step by Step** | <img src="Assets/profile/silent.png" width="22" height="22" title="Silent"> | Skill | Rare | X | Exhaust. For next X (Upgraded: X+1) turns, draw +1 card and gain +1 Energy each turn. Upgraded gains Retain. |
| **Sorcery Strike** | <img src="Assets/profile/tanx.png" width="22" height="22" title="Tanx"> | Attack | Ancient | 1 (0+) | Exhaust. Deal **9** damage, draw **1** card, apply **1** Sorcery Vulnerable. |
| **Sorcery Defend** | <img src="Assets/profile/tanx.png" width="22" height="22" title="Tanx"> | Skill | Ancient | 1 (0+) | Exhaust. Gain **8** Block, draw **1** card, apply **1** Sorcery Weak. |

### Powers & Status Adjustments

> [!tip]
> For complete powers, debuffs, and Boss mechanics reference, see **[docs/powers.md](docs/powers.md)**.

| Power / Effect | Category | Source | Description |
| :--- | :---: | :--- | :--- |
| <img src="Assets/powers/electrodynamics_power.png" width="22" height="22" valign="middle"> **Electrodynamics** | Player Buff | Defect Card "Electrodynamics" | Lightning Orbs hit ALL enemies. |
| <img src="Assets/powers/evolve_power.png" width="22" height="22" valign="middle"> **Evolve** | Player Buff | Ironclad Card "Evolve" | Whenever you draw a Status card, draw **1 / 2** card(s). |
| <img src="Assets/powers/step_by_step_power.png" width="22" height="22" valign="middle"> **Step by Step** | Player Buff | Silent Card "Step by Step" | Draw +1 card and gain +1 Energy each turn for **X / X+1** turns. |
| <img src="Assets/powers/sorcery_vulnerable.png" width="22" height="22" valign="middle"> **Sorcery Vulnerable** | Debuff | Ancient Card "Sorcery Strike" | Takes **75%** more damage from attacks. Decreases by 1 at turn end if attacked this turn. |
| <img src="Assets/powers/sorcery_weak.png" width="22" height="22" valign="middle"> **Sorcery Weak** | Debuff | Ancient Card "Sorcery Defend" | Deals **50%** less attack damage. Decreases by 1 at turn end if attacked this turn. |
| <img src="Assets/powers/infected_power.png" width="22" height="22" valign="middle"> **Infected** | Debuff | Infested Prism Boss | At turn end, lose **{Amount}** HP (applied by unblocked attacks). |
| <img src="Assets/powers/mode_shift_power.png" width="22" height="22" valign="middle"> **Mode Shift** | Boss Mechanic | Guardian Boss | Shift into Defensive Mode after taking **{Amount}** unblocked damage. |
| <img src="Assets/powers/sharp_hide_power.png" width="22" height="22" valign="middle"> **Sharp Hide** | Boss Mechanic | Guardian Boss | Whenever attacked, deal **{Amount}** damage back to the attacker. |
| <img src="Assets/powers/split_power.png" width="22" height="22" valign="middle"> **Split** | Boss Mechanic | Slime Boss | Splits into 2 smaller Slimes when HP ≤ **50%**. |
| <img src="Assets/powers/time_warp_power.png" width="22" height="22" valign="middle"> **Time Warp** | Boss Mechanic | Time Eater Boss (Code reserved) | Ends player's turn and gains 2 Strength after **{Amount}** cards are played. |


### Monsters & Bosses

| Name | Code | Vanilla Behavior | MOD Reworked Behavior |
| :--- | :--- | :--- | :--- |
| <img src="Assets/map/aeonglass_boss.png" width="22" height="22" valign="middle"> **Aeonglass** | MON-01 | Wither cards generated by Boss are unplayable, sent directly to Exhaust. | Wither cards are **1-cost playable and Exhaust**, retaining growth. |
| <img src="Assets/map/elite.png" width="22" height="22" valign="middle"> **Infested Prism** | BOSS-01 | Contaminates skill cards (Spark of Vitality) on turn 1, gaining Strength when played; 4-turn attack rotation. | Removes Spark of Vitality; applies **Infected** on unblocked attacks and uses fixed 4-turn rotation (configurable via Mod Settings). |
| <img src="Assets/map/monster.png" width="22" height="22" valign="middle"> **Bandit Bear** | MONSTER-01 | Turn 1 Bear Hug applies 1 Vulnerable. | Bear Hug debuff changed to reduce 2 **Dexterity** (`DexterityPower` -2). |
| <img src="Assets/map/guardian_boss.png" width="22" height="22" valign="middle"> **Guardian** | AFP-BOSS-01 | - | Ported from Acts From the Past v1.0.5 with full rotation, Mode Shift, Sharp Hide, animations and sound; Mind Bloom encounter exclusive. |
| <img src="Assets/map/hexaghost_boss.png" width="22" height="22" valign="middle"> **Hexaghost** | AFP-BOSS-02 | - | Ported from Acts From the Past v1.0.5 with 6 flame states, Divider/Sear/Inferno, Burn upgrades, FX and sound; Mind Bloom encounter exclusive. |
| <img src="Assets/map/slime_boss.png" width="22" height="22" valign="middle"> **Slime Boss** | AFP-BOSS-03 | - | Ported from Acts From the Past v1.0.5 with Boss, Acid/Spike Slime L/M units, and 2-stage split chain; 7-slot Mind Bloom encounter exclusive. |

### Events & Encounters

> [!tip]
> For complete event branch options and trigger conditions, see **[docs/events.md](docs/events.md)**.

#### Vanilla Event Adjustments
- **Zen Weaver**: Card removal prices reduced to **75 / 150 Gold**.
- **Trash Heap**: Relic reward pool now includes <img src="Assets/relics/omamori.png" width="18" height="18" valign="middle"> **Omamori**.
- **Bug Eater / Frankenstein / Future of Potions?**: Added configurable **"Leave"** choices in initial options.

#### STS1 Returned Events
| Event Name | Requirements | Summary |
| :--- | :--- | :--- |
| **Old Beggar** | All players Gold ≥ 75 | Pay 75 Gold to reveal Cleric card removal service. |
| **Cursed Tome** | Act 2 (No Tome) | Multi-stage reading test to receive <img src="Assets/relics/necronomicon.png" width="18" height="18" valign="middle"> **Necronomicon** / <img src="Assets/relics/nilrys_codex.png" width="18" height="18" valign="middle"> **Nilry's Codex** / <img src="Assets/relics/enchiridion.png" width="18" height="18" valign="middle"> **Enchiridion**. |
| **Masked Bandits** | Act 2, Floor ≥ 23 | Hand over all Gold or fight to earn <img src="Assets/relics/red_mask.png" width="18" height="18" valign="middle"> **Red Mask**. |
| **Augmenter (J.A.X.)** | Act 2 (Removable cards ≥ 2) | Obtain J.A.X. / Transform 2 cards / Obtain <img src="Assets/relics/mutagenic_strength.png" width="18" height="18" valign="middle"> **Mutagenic Strength**. |
| **The Divine Fountain** | Deck has Curse cards | Completely purge all Curse cards from your deck. |
| **Cleric** | All players Gold ≥ 35 | Offers 25% HP heal for Gold / 75 Gold card removal. |
| **Mind Bloom** | Act 3 | Choice of Act 1 Boss fight, upgrade all cards (gains <img src="Assets/relics/mark_of_the_bloom.png" width="18" height="18" valign="middle"> **Mark of the Bloom**), or 999 Gold. |
| **Wheel of Change** | - | Spin wheel for random Gold / Relic / Full Heal / Card Removal / Curse / Damage. |
| **Tomb of Lord Red Mask** | Act 3 | Offer all Gold for <img src="Assets/relics/red_mask.png" width="18" height="18" valign="middle"> **Red Mask** (or gain 222 Gold if wearing Red Mask). |
| **The Library** | Act 3 | Offers **[Read]** (choose 1 cross-class card) or **[Sleep]** (heal 33% HP). |


### Relics

#### New Relics

| Relic | Type | Description |
| :--- | :--- | :--- |
| <img src="Assets/relics/sundial.png" width="22" height="22" valign="middle"> **Sundial** | <img src="Assets/profile/merchant.png" width="22" height="22" title="Merchant"> | Every 3 times you shuffle your draw pile (persists across combats), gain 3 Energy. |
| <img src="Assets/relics/orange_pill.png" width="22" height="22" valign="middle"> **Orange Pills** | <img src="Assets/profile/merchant.png" width="22" height="22" title="Merchant"> | Playing an Attack, Skill, and Power card in a single turn removes all debuffs (except Queen's Soul Shackles). |
| <img src="Assets/relics/dead_branch.png" width="22" height="22" valign="middle"> **Dead Branch** | Rare | Whenever a card is Exhausted, add a random card to your hand (Ethereal trigger gives turn-retain). |
| <img src="Assets/relics/omamori.png" width="22" height="22" valign="middle"> **Omamori** | <img src="Assets/map/event.png" width="22" height="22" title="Event"> | Negates the next 2 Curses obtained (with counter). |
| <img src="Assets/relics/peace_pipe.png" width="22" height="22" valign="middle"> **Peace Pipe** | Rare | Adds a "Toke" option at Rest Sites, allowing you to remove a card. |
| <img src="Assets/relics/smiling_mask.png" width="22" height="22" valign="middle"> **Smiling Mask** | <img src="Assets/profile/merchant.png" width="22" height="22" title="Merchant"> | Card removal cost is fixed at 50 Gold. |
| <img src="Assets/relics/coffie_cup.png" width="22" height="22" valign="middle"> **Coffee Drip** | <img src="Assets/profile/darv.png" width="22" height="22" title="Darv"> | Cannot Rest at Rest Sites; gain +1 Energy each turn. |
| <img src="Assets/relics/fusion_hammer.png" width="22" height="22" valign="middle"> **Fusion Hammer** | <img src="Assets/profile/darv.png" width="22" height="22" title="Darv"> | Cannot Forge at Rest Sites; gain +1 Energy each turn. |
| <img src="Assets/relics/curse_key.png" width="22" height="22" valign="middle"> **Cursed Key** | <img src="Assets/profile/darv.png" width="22" height="22" title="Darv"> | Gain +1 Energy each turn; obtain a random Curse upon opening a chest. **Singleplayer only. Allows skipping chest via bottom-right ProceedButton to avoid Curses.** |
| <img src="Assets/relics/dwarf_anvil.png" width="22" height="22" valign="middle"> **Dwarf Anvil** | <img src="Assets/profile/merchant.png" width="22" height="22" title="Merchant"> | Upon pickup, applies "Forge" enchantment to 3 cards, permanently reducing cost by 1 (min 0). |
| <img src="Assets/relics/wrist_blade.png" width="22" height="22" valign="middle"> **Wrist Blade** | Uncommon | Silent exclusive. 0-cost Attack cards deal 4 additional damage. |
| <img src="Assets/relics/hovering_kite.png" width="22" height="22" valign="middle"> **Hovering Kite** | Common | Silent exclusive. The first time you discard a card each turn, gain 1 Energy. |
| <img src="Assets/relics/soul_contract.png" width="22" height="22" valign="middle"> **Soul Contract** | <img src="Assets/profile/merchant.png" width="22" height="22" title="Merchant"> | Select 1 card in your deck with Exhaust and permanently remove its Exhaust property. |
| <img src="Assets/relics/nilrys_codex.png" width="22" height="22" valign="middle"> **Nilry's Codex** | <img src="Assets/map/event.png" width="22" height="22" title="Event"> | At turn end, choose 1 of 3 random **Upgraded** cards to shuffle into your draw pile. |
<!-- | <img src="Assets/relics/shabbydoll.png" width="22" height="22" valign="middle"> **Shabby Doll** | <img src="Assets/profile/tanx.png" width="22" height="22" title="Tanx"> | Upon pickup, lose 50% max HP, and replace all starter Strikes and Defends with upgraded **Sorcery Strike+** and **Sorcery Defend+**. | -->

#### Vanilla Relic Adjustments

| Relic | Type | Vanilla | MOD Rework |
| :--- | :--- | :--- | :--- |
| <img src="Assets/relics/sturdy_clamp.png" width="22" height="22" valign="middle"> **Sturdy Clamp** | Rare | Retain 10 Armor | Retain **15 Armor** |
| <img src="Assets/relics/preserved_fog.png" width="22" height="22" valign="middle"> **Preserved Fog** | <img src="Assets/profile/vakuu.png" width="22" height="22" title="Vakuu"> | Remove 3 cards | Remove **4 cards** |
| <img src="Assets/relics/red_mask.png" width="22" height="22" valign="middle"> **Red Mask** | <img src="Assets/map/event.png" width="22" height="22" title="Event"> | In general relic pool | **Removed** from general relic pool, obtainable only via Red Mask events |
| <img src="Assets/relics/history_course.png" width="22" height="22" valign="middle"> **History Course** | <img src="Assets/map/event.png" width="22" height="22" title="Event"> | Replays only last Attack played last turn | Reverted to replay last **Attack or Skill** played last turn |
| <img src="Assets/relics/diamond_diadem.png" width="22" height="22" valign="middle"> **Nonupeipe's Diamond Diadem** | <img src="Assets/profile/nonupeipe.png" width="22" height="22" title="Nonupeipe"> | Combat start gain 20 Block, retained next turn | Reverted: Halves incoming damage when playing ≤ 2 cards in a turn |
| <img src="Assets/relics/toasty_mittens.png" width="22" height="22" valign="middle"> **Toasty Mittens** | <img src="Assets/profile/tezcatara.png" width="22" height="22" title="Tezcatara"> | Auto-exhausts 1 draw card at turn start for +1 Str | Allows selecting 1 hand card to exhaust (**provides Skip option**), granting +1 Str upon success |
| <img src="Assets/relics/signet_ring.png" width="22" height="22" valign="middle"> **Signet Ring** | <img src="Assets/profile/nonupeipe.png" width="22" height="22" title="Nonupeipe"> | Gain 888 Gold | Reverted to gain **999 Gold** |

---

## Project Knowledge Base

👉 **[Knowledge Base Hub Index (docs/README.md)](docs/README.md)**

- **[Powers & Status Manual](docs/powers.md)**: Full guide on Buffs, Debuffs, Boss mechanics, and vanilla power reworks.
- **[Events & Encounters Manual](docs/events.md)**: Guide on STS1 returned events, vanilla adjustments, choices, and triggers.
- **[STS2 Modding Guide](docs/sts2-modding-guide.md)**: Tutorial on building STS2 Mods from scratch.
- **[Technical Reports](docs/README.md#系统设计与技术分析报告)**: Layered technical analysis of Red Mask event, Treasure Room Skip system report, etc.
- **[Pending Tasks List](docs/balance-changes.md)** and **[Changelog](CHANGELOG.md)**.

## About the Spire

1. [Slay the Spire 2 Wiki](https://sts2.huijiwiki.com/wiki/%E9%A6%96%E9%A1%B5)
2. [Slay the Spire 1 Wiki](https://sts.huijiwiki.com/wiki/%E9%A6%96%E9%A1%B5)
3. [Steam Official Announcements](https://steamcommunity.com/games/2868840/announcements/)
4. [Spire Codex Statistics](https://spire-codex.com/)
5. [STS2 Modding Tutorials](https://tutorials.sts2modding.com/)

## License

This project is open-source under the **[GNU General Public License v3.0 (GPL-3.0)](LICENSE)**.

- Free usage, modification, and commercial redistribution permitted.
- **Copyleft Constraint**: Any derivative work or re-distribution using code from this repository **must remain open-source under GPL-3.0 with full source code made available**.
