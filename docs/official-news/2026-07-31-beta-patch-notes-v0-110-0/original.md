---
date: "2026-07-31"
version: "v0.110.0"
title: "Beta Patch Notes - v0.110.0"
source: "https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1839676055887004"
author: "demileaf"
---

Another beta patch just dropped! This one contains a host of balance changes and reworks. Notably, Mirage and Pillar of Creation have been reverted back to their pre-0.109.0 versions and have also been slightly buffed and nerfed, respectively. We've also implemented a brand new feature in this patch--a keyboard-only control option! So be sure to let us know what you think if you try it out. In terms of art, many cards have new, polished portrait art and, excitingly, characters now have VFX for their unique forms!

![](https://clan.fastly.steamstatic.com/images/44971832/5241bdca8e33580f7b3ba5361221225407ec551c.gif)

### CONTENT & BALANCE:

**Ancients:**

- Buffed **Neow's Abundance** card:

- Now always creates Upgraded powers
- Upgrade changed to lower Energy cost by 1

- Reworked **Tezcatara's Toasty Mittens** relic: "At the start of your turn, Exhaust the top card of your Draw Pile and gain 1 Strength." -> "At the start of your turn, Exhaust 1 card from your Hand and gain 1 Strength."
- Buffed **Tezcatara's Seal of Gold** relic: Gold loss decreased from 5 -> 3
- Changed **Nonupeipe's Beautiful Bracelet** relic:

- Cards enchanted increased from 3 -> 4
- Swift amount decreased from 3 -> 2

- Buffed **Nonupeipe's Fur Coat** relic: marked combats increased from 7 -> 8
- Nerfed **Nonupeipe's Signet Ring** relic: Gold decreased from 999 -> 888
- Buffed **Pael's Relax** card: Block increased from 15(17) → 16(18)
- Buffed **Tanx's Whistle** card: cost decreased from 3 -> 2
- Buffed **Tanx's Maul** card: Maul damage scaling increased from 1(2) -> 2(3)
- Reverted **Vakuu's Fiddle** relic: card draw decreased again from 3 -> 2

**Ironclad:**

- Buffed **Mangle** card: damage increased from 15(20) -> 20(26)
- Buffed **Pact's End** card: damage increased from 17(23) -> 18(24)

**Silent:**

- Reworked **Haze** card: Uncommon - Skill - Cost 3 - "Sly. Apply 4(6) Poison to ALL enemies." -> Uncommon - Skill - Cost 2 - "Apply 4(6) Poison and 1(2) Weak to ALL enemies."
- Reworked **Outbreak** card into a Rare Skill: Uncommon - Power - Cost 1 - "Whenever you apply Poison, deal 4(5) damage to ALL enemies." -> Rare - Skill - Cost 3 - "Apply 9(12) Poison to ALL enemies. Poison triggers immediately."
- Reworked and renamed **Scare** card -> **Sidestep**: Uncommon - Skill - Cost 0 - "Apply 1 Weak to ALL enemies. Exhaust. (Lose Exhaust.)" -> Uncommon - Skill - Cost 0 - "Next turn, gain 1(2) Energy."
- Changed **Mirage** card:

- Reverted back to its previous version (i.e. gains Block based on enemy Poison again)
- No longer Exhausts

- Nerfed **Well Laid Plans** card: cost increased from 1(0) -> 2(1)
- Changed **Echoing Slash** card: rarity decreased from Rare -> Uncommon

**Regent:**

- Reworked **Regalite** relic: "Whenever you create a card, gain 2 Block." -> "The first time you create a card each turn, gain 6 Block."
- Buffed **Terraforming** card: Vigor increased from 6(8) → 7(10)
- Changed **Pillar Of Creation** card:

- Reverted back to its previous version (i.e. effect is no longer triggered just once per turn)
- Block decreased from 3(4) -> 2(3)

- Buffed **Crush Under** card: damage increased from 7(8) -> 8(9)

**Necrobinder:**

- Changed **Eidolon**: can no longer be generated mid-combat
- Buffed **Sacrifice** card: Block gain increased from double -> triple Osty's Max HP
- Buffed **The Scythe** card: damage scaling increased from 4(5) -> 5(7)

**Defect:**

- Buffed **Biased Cognition** card: Focus increased from 4(5) → 5(6)
- Buffed **Refract** card: damage increased from 9(12) -> 10(13)
- Nerfed **Rocket Punch** card: "Whenever you create a Status, reduce this card's cost to 0 Energy until played." -> "Whenever you create a Status, reduce this card's cost by 1 Energy until played."
- Buffed **Synchronize** card:

- Upgrade changed to increase temp focus from 2 -> 3
- No longer Exhausts

### USER INTERFACE & EXPERIENCE:

**General:**

- Added support for keyboard-only mode
- Added a "Share" button on the map screen which takes a screenshot of the entire map
- Added a "Listening For Input" popup when a player is remapping inputs or bindings
- Updated padding, font sizes, and color for input remap screen
- Updated input glyph positioning on various buttons
- Updated change profile button to render a larger input glyph
- Updated keyboard-mode glyph asset from placeholder -> style match
- Adjusted hotkey icon size and position in top bar
- Updated generic selection reticle asset to improve contrast and resolution. It's now rendered very slightly smaller as well.
- If a binding is missing, a gray hyphen is now rendered rather than nothing in input settings
- Users are now informed if Steam Input is activated or not in a more explicit way
- Scrollbar in settings screen is now placed better in non-standard aspect ratios
- When using a controller, the game now always tries to initially hover a map node connected to your current one when you have **Winged Boots** relics
- Disabled left/right paging for Stats screen as Achievements screen does not exist yet

**Multiplayer:**

- Made the UI for joining a friend's run nicer looking
- **Tutor** card now un-ends the target's turn if they have ended their turn
- Removed "Waiting for Players" screen when dying to Architect

### ART:

- Added form VFX
- Added portrait art for the following cards:

- Constellation
- Blade Symphony
- Concoct
- Tutor
- Imitation Learning
- Soulbound
- Fade

- Added beta portrait art for **Sidestep** card
- Added fire to **Test Subject** burn animation

### WRITING:

**General:**

- When the word "potions" shows up in option descriptions or relic descriptions it is now gold and capitalized
- Fixed **Slow** debuff power description not displaying its active multiplier in the hovertip
- Fixed typos in **Abundance** and **Blade Symphony** cards
- Updated **Diamond Diadem** relic's wording to be clearer about how long its effect lasts
- Renamed Full Keyboard -> Keyboard-Only

**Localization:**

- Updated translations for various languages
- Fixed translation errors in the following languages:

- French
- Italian
- Russian
- Thai
- Turkish
- Traditional Chinese

### BUG FIXES:

**General:**

- Uncommon cards show up slightly more often (as intended)
- Players can no longer discern the shuffle order of cards in certain scenarios
- The run history is now centered on ultrawide screens
- Fixed an issue where an Epoch unlock earned at the end of a run could be lost, permanently blocking Timeline progress (affected saves are repaired on load)
- Fixed an issue where leaving the game over screen mid-animation could award an Epoch unlock without spending the score that paid for it
- Fixed being able to controller navigate to card grid while inspecting a card in the Compendium
- Fixed opening the settings screen when a player would hit "escape" to close the potions popup

**Enemies:**

- **Tough Egg** can now have 1 more max HP
- **Decimillipede** now correctly displays stats in the Bestiary
- Fixed error that occurred when **Tough Egg** would hatch in the Bestiary

**Events:**

- Fixed typo in **Lantern Key** event

**Potions & Relics:**

- **Bellows** relic now properly upgrades the extra card when **Pendulum** relic triggers on turn 1
- Fixed **Dowsing** missing card preview for Abundance

**Ironclad:**

- Fixed **Juggling** card so that when your third attack also auto-plays an attack (for example, Uproar card), Juggling now properly copies the original attack instead of the auto-played attack

**Multiplayer:**

- Fixed chosen card getting stuck if another player gives you the **Touch of Insanity** potion
- **Feral** card now puts **The Ball** card into your hand instead of another player's
- **Imitation Learning** card no longer softlocks with Replay, now ignores duplicate plays
- **Tutor** card no longer softlocks with Replay
- Players can Abandon Run outside of combat again

### MODDING:

- The "Report Bug" button is no longer shown in error dialogs when any player in a session has mods
