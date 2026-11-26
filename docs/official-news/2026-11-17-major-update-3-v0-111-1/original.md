---
date: "2026-11-17"
version: "v0.111.1"
title: "Major Update #3 - v0.111.1"
source: "https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1849458702691833"
author: "demileaf"
---

It's time, Slayers! The third major main branch update is finally here, bringing all of the content, art, balance changes, and bug fixes from v0.108.0-v0.111.0! So if you're a beta branch player, this won't be relevant to you. Go back to Fooling around!

Speaking of which, while the new character and game modes on the current beta branch are the talk of the town right now, all that stuff will need to be tested and polished up quite a bit more before it reaches the main branch. We're not sure when that will be, so if you want to try it out now then switch over to our more experimental beta branch by right-clicking on *Slay the Spire 2* in your library and navigating to Properties -> Game Versions & Betas and selecting `public-beta` from the dropdown.

But for those of you seeking a more stable experience, we've got lots of juicy content for you today (4 whole beta patches worth), so let's check it all out!

![](https://clan.fastly.steamstatic.com/images/44971832/5241bdca8e33580f7b3ba5361221225407ec551c.gif)

One of the first things to note is that this update is an extra pretty one! We've introduced some unique VFX for characters' forms, special low-health idle animations, passive Orb VFX, lots of epoch art, and more!

Also...

![](https://clan.fastly.steamstatic.com/images/44971832/37f96ab06c915ff9f4bda409ea56a48d6203ea5c.gif)

Aeonglass is alive!!! The cool animations that our animator broke down back in the July Neowsletter are finally available for all players, that way everyone can experience the iconic boss in all its glory.

Other major things to note are that Traditional Chinese and Indonesian are now available, keyboard-only mode is an option, there are some new Neow relics, a big batch of character-specific multiplayer cards dropped, and tons of balance changes are here. On with the rest of the notes!

## CONTENT & BALANCE:

**General:**

- End-of-turn Orb passives now trigger before Doom.

- This means that, if a player would die to self-Doom (ex. from Neurosurge card) but also has an Orb with an end-of-turn passive that would kill the last remaining enemy (ex. Lightning Orb while the enemy is at 3 HP), they will now win the combat instead of losing it after ending their turn.

- Added a Randomize button to the Custom Run screen. This button randomly selects 2 good modifiers and 1 bad one, and randomizes the selected character.

**Ancients:**

- Added new Neow relic, **Neow's Sacrifice**: "Upon pickup, procure 1 Ambergris and add 1 Guilty to your Deck."

- **Ambergris** potion: "Heal for 50% of your max HP. If used in combat, take an extra turn."

- Added new Neow relic, **Dowsing Rod**: "Upon pickup, add 1 Dowsing to your Deck."

- **Dowsing** card: Quest | "Unplayable. After entering 5 more ? rooms, transforms into Abundance."
- **Abundance** card: Skill | Cost 1 | "Choose 1 of 3 Upgraded Powers to add into your Hand. It's free to play this turn. Exhaust." Upgrade lowers its cost by 1.

- Changed **Darv** blessing pools for more variety (you will see Energy relics more often).
- Moved **Orobas' Sand Castle** relic from Pool 1 → Pool 2 (second option instead of first option).
- Reworked **Nonupeipe's Diamond Diadem** relic: "Whenever you play 2 or fewer cards in a turn, take half damage from enemies." → "Start combat with 20 Block. Block is not removed at the start of your 2nd turn."
- Changed **Nonupeipe's Beautiful Bracelet** relic: now Enchants 4 random cards instead of letting you select 3 cards to Enchant. Swift amount decreased from 3 → 2.
- Buffed **Nonupeipe's Fur Coat** relic: marked combats increased from 7 → 8.
- Nerfed **Nonupeipe's Signet Ring** relic: Gold decreased from 999 → 888.
- Buffed **Pael's Relax** card: Block increased from 15(17) → 16(18).
- Nerfed **Tanx's Meat Cleaver** relic: HP gain decreased from 9 → 5.
- Buffed **Tanx's Whistle** card: cost decreased from 3 → 2.
- Buffed **Tanx's Maul** card: Maul damage scaling increased from 1(2) → 2(3).
- Buffed **Tezcatara's Toybox** relic: relics gained increased from 4 → 5.
- Buffed **Tezcatara's Seal of Gold** relic: Gold loss decreased from 5 → 3.
- Reworked **Tezcatara's Toasty Mittens** relic: "At the start of your turn, Exhaust the top card of your Draw Pile and gain 1 Strength." → "At the start of your turn, Exhaust 1 card from your Hand and gain 1 Strength."
- Nerfed **Tezcatara's Brightest Flame** card: max HP loss increased from 1 → 2.
- Changed **Vakuu's Sere Talon** and **Distinguished Cape** relics: swapped downsides.
- Changed **Vakuu's Jeweled Mask** relic: now chooses a non-Innate Power unless all Powers in your deck are Innate.

**Enemies:**

- Nerfed **Aeonglass**: Ebb move damage decreased from 26(32) → 22(26) to lower its front-loaded damage.
- Buffed **Torch Head Amalgam**:

- Now starts with a new Strong Tackle move on turn 1 to increase front-loaded damage. Deals 26(32) damage.
- Weak Tackle damage at A9 increased from 14(15) → 14(16).
- Tackle damage at A9 increased from 18(19) → 18(22).

- Buffed **Axebot**:

- Hammer Uppercut move damage increased from 12(14) → 14(18).
- Now gains +10 Max HP each time it respawns.
- The One-Two damage increased from 9(10)x2 → 10(11)x2.

- Buffed **Mechaknight**: now also deals 8(12) damage on its Flamethrower turn.
- Buffed **Entomancer**: HP at A8 increased from 145(155) → 145(165).
- Buffed **Exoskeleton**: HP at A8 increased from 24-28(25-29) → 24-28(26-30).
- Buffed **Globe Head**: Galvanic power at A9 increased from 6(6) → 6(8).
- Buffed **Louse Progenitor**: Strength gain at A9 increased from 5(5) → 5(7).
- Buffed **Soul Fysh**: De-Gas move damage at A9 increased from 16(17) → 16(18).

**Events:**

- Nerfed **History Course** relic: now only repeats Attacks, does not affect Skills.

**Colorless Cards:**

- Changed **Rend** card: cost decreased from 2 → 1. Base damage decreased from 15(18) → 10(12).
- Changed **Salvo** and **Splash** cards: swapped rarities. Splash is now Rare and Salvo is now Uncommon.

**Ironclad:**

- Reworked **Expect a Fight** card: Uncommon - Skill - Cost 2(1) - "Gain 1 Energy for each Attack in your Hand. You cannot gain additional Energy this turn." → Uncommon - Skill - Cost 3 - "Gain 15(16) Block. Gains 5(8) additional Block for each Strength you have."
- Nerfed **Colossus** card: Block gain decreased from 5(8) → 4(7).
- Nerfed **Crimson Mantle** card: Block gain decreased from 8(10) → 7(10).
- Changed **Taunt** card: Block gain decreased from 7(8) → 6(7). Rarity decreased from Uncommon → Common.
- Buffed **Demon Form** card: Strength gain increased from 2(3) → 3(4).
- Buffed **Forgotten Ritual** card: no longer requires a card to be Exhausted to gain Energy.
- Buffed **Howl From Beyond** card: damage increased from 16(21) → 18(24).
- Buffed **Mangle** card: damage increased from 15(20) → 20(26).
- Buffed **Pact's End** card: damage increased from 17(23) → 18(24).
- Buffed **Primal Force** card: **Giant Rock** token damage increased from 16(20) → 20(24).
- Buffed **Rampage** card: base damage increased from 9 → 10. Scaling damage increased from 5(9) → 5(10).
- Buffed **Setup Strike** card: Strength gain increased from 2(3) → 3(4).
- Changed **Bloodletting** card: rarity increased from Common → Uncommon.
- Changed **Cruelty** card: rarity decreased from Rare → Uncommon.
- Changed **Dominate** card: rarity increased from Uncommon → Rare.

**Silent:**

- Reworked **Expertise** card: "Draw cards until you have 6(7) in your hand." → "Draw 2(3) cards. They gain Retain this turn."
- Reworked **Outbreak** card: Uncommon - Power - Cost 1 - "Every 3 times you apply Poison, deal 15 damage to ALL enemies." → Rare - Skill - Cost 3 - "Apply 9(12) Poison to ALL enemies. Poison triggers immediately."
- Reworked **Well-Laid Plans** card:

- Uncommon - Power - Cost 1 - "At the end of your turn, Retain up to 1(2) card(s)." → Rare - Power - Cost 2(1) - "At the end of your turn, you no longer discard your Hand."
- Now also available in multiplayer, was previously singleplayer-only.

- Reworked **Haze** card: Uncommon - Skill - Cost 3 - "Sly. Apply 4(6) Poison to ALL enemies." → Uncommon - Skill - Cost 2 - "Apply 4(6) Poison and 1(2) Weak to ALL enemies."
- Reworked and renamed **Scare** card → **Sidestep**: Uncommon - Skill - Cost 0 - "Apply 1 Weak to ALL enemies. Exhaust. (Lose Exhaust.)" → Uncommon - Skill - Cost 0 - "Next turn, gain 1(2) Energy."
- Changed **Mirage** card: Upgrade changed from lowering cost 1 → 0 to removing Exhaust.
- Nerfed **Tracking** card: "Weak enemies take double damage from Attacks." → "Weak enemies take 50% more damage from Attacks."
- Nerfed **Blade of Ink** card: Inky enchantment no longer gives additional damage (still applies Weak).
- Buffed **Anticipate** card: Dexterity increased from 2(3) → 2(4).
- Buffed **Flick-Flack** card: damage increased from 6(8) → 7(9).
- Changed **Accelerant** card: rarity decreased from Rare → Uncommon.
- Changed **Echoing Slash** card: rarity decreased from Rare → Uncommon.

**Regent:**

- Reworked **Regalite** relic: "Whenever you create a card, gain 2 Block." → "The first time you create a card each turn, gain 4 Block."
- Changed **Guiding Star** card: Star cost decreased from 2 → 1. Now draws cards next turn instead of this turn.
- Nerfed **Pillar of Creation** card: Block decreased from 3(4) → 2(3).
- Nerfed **Collision Course** card: damage decreased from 11(15) → 10(14).
- Nerfed **GUARDS!!!** card's **Minion Sacrifice** token: Block gain decreased from 8(11) → 7(10).
- Nerfed **Refine Blade** card: Forge decreased from 9(13) → 8(12).
- Buffed **Alignment** card: Star cost decreased from 3 → 2.
- Buffed **Crush Under** card: damage increased from 7(8) → 8(9).
- Buffed **Devastate** card: damage increased from 30(40) → 35(45).
- Buffed **Divine Destiny** relic: Star gain increased from 6 → 7.
- Buffed **Resonance** card: Star cost decreased from 3 → 2.
- Buffed **Spoils of Battle** card: Forge increased from 5(8) → 6(9).
- Buffed **Terraforming** card: Vigor increased from 6(8) → 7(10).

**Necrobinder:**

- Reworked **Eidolon** card: "Exhaust your Hand. If 9 cards were Exhausted this way, gain 1 Intangible." → "Play ALL Ethereal cards in your Exhaust Pile. Exhaust."

- Upgrade lowers its cost by 1.
- Does not play Unplayable cards.
- Can no longer be generated mid-combat.

- Buffed **Haunt** card: HP loss increased from 6(8) → 7(9).
- Buffed **Reave** card: damage increased from 9(11) → 10(13).
- Buffed **Sacrifice** card: Block gain increased from double → triple Osty's Max HP.
- Buffed **Shroud** card: Block gain increased from 2(3) → 3(4).
- Buffed **Soulstorm** card: additional damage increased from 2(3) → 4(6).
- Buffed **The Scythe** card: damage scaling increased from 4(5) → 5(7).
- Buffed **Time's Up** card: no longer Exhausts.

**Defect:**

- Reworked **Hyperbeam** card: "Deal 28(36) damage to ALL enemies. Lose 3 Focus." → "Deal 24(30) damage to ALL enemies. Lose 3 Focus this turn."
- Changed **Synchronize** card: no longer Exhausts. Focus changed from 2 → 1(2).
- Nerfed **Compact** card's **Fuel** token:

- No longer draws cards.
- Upgrade changed from increasing card draw → granting 1 additional Energy.

- Nerfed **Null** card: Weak decreased from 2(3) → 1(2).
- Nerfed **Rocket Punch** card: "Whenever you create a Status, reduce this card's cost to 0 Energy until played." → "Whenever you create a Status, reduce this card's cost by 1 Energy until played."
- Buffed **Biased Cognition** card: Focus increased from 4(5) → 5(6).
- Buffed **Momentum Strike** card: damage increased from 10(13) → 11(15).
- Buffed **Refract** card: damage increased from 9(12) → 10(13).
- Buffed **Sunder** card: damage increased from 24(32) → 26(34).
- Buffed **Thunder** card: damage increased from 6(8) → 8(11).
- Changed **Scrape** card: no longer discards cards whose energy costs are temporarily set to 0 by global temporary effects (ex. Pounce card's "The next Skill you play costs 0 Energy" effect).
- Changed **Shatter** card: now Exhausts.
- Changed **Trash to Treasure** card: upgrade changed from gaining Innate → lowering cost by 1.

**Potions & Relics:**

- Changed **Regen Potion**: its healing effect now triggers before Doom, preventing the player from dying to self-Doom in certain situations.

**Multiplayer:**

- Added new multiplayer **Ironclad** cards:

- **Midnight**: Rare - Attack - Cost 12 - "Deal 60(72) damage. Costs 1 less for each card Exhausted this combat by ANYONE."
- **Blaze**: Uncommon - Skill - Cost 2 - "Give another player 5(7) Strength."
- **Outrage**: Uncommon - Attack - Cost 0 - "Deal 9(13) damage. Add a copy of this card into EVERYONE'S Discard Pile."

- Buffed and slightly reworked **Tank** card: "Take double damage from enemies. Allies take half damage from enemies." → "Take 50% additional damage from enemies. Allies take 50% less damage from enemies."
- Added new multiplayer **Silent** cards:

- **Blade Symphony**: Uncommon - Skill - Cost 2(1) - "Add 2 Shivs to ALL players' Hands."
- **Concoct**: Uncommon - Skill - Cost 0 - "Another player's Attacks apply 3(4) Poison this turn."
- **Fade**: Uncommon - Skill - Cost 0 - "Retain. Another player gains 6(9) Dexterity this turn."

- Changed **Flanking** card: rarity increased from Uncommon → Rare.
- Added new multiplayer **Regent** cards:

- **Plot**: Uncommon - Skill - Cost 1 - "Next turn, ALL players draw 2(3) cards."
- **Constellation**: Uncommon - Skill - Cost 0 - "Another player draws 1 card, gains 1 Energy, and gains 9(12) Block."
- **Tutor**: Rare - Skill - Cost 1(0) - "Another player chooses a card in their Draw Pile to add into their Hand."

- Added new multiplayer **Necrobinder** cards:

- **Underworld**: Uncommon - Skill - Cost 2 - "Whenever other players deal attack damage this turn, apply that much Doom. Exhaust. (Lose Exhaust.)"
- **Soulbound**: Uncommon - Power - Cost 1 - "(Innate.) Choose an ally. Whenever you create a Soul, add a Soul to their Draw Pile."
- **Cacophony**: Rare - Power - Cost 2 - "Every 33 cards drawn by ALL players, deal 66(99) damage to a random enemy."

- Changed **Legion of Bone** card: no longer Exhausts.
- Added new multiplayer **Defect** cards:

- **Hibernate**: Uncommon - Skill - Cost 2 - "Channel 2(3) Frost. This turn, your Frost grants ALL allies Block."
- **One For All**: Rare - Power - Cost 1 - "EVERYONE'S 0 energy Attacks deal 3(4) additional damage."
- **Imitation Learning**: Rare - Skill - Cost 1 - "Choose another player. The next 2(3) times they play a Power, you play a copy of it. Exhaust."

- Changed **Ignition** card: rarity decreased from Rare → Uncommon.
- Added new multiplayer Colorless card:

- **The Ball**: Uncommon - Attack - Cost 1 - "Deal 10 damage. Increase this card's damage by 10(15) this combat and give it to a random ally."

- Nerfed **Beacon of Hope** card: cost increased from 1 → 2.
- Nerfed enemy Block scaling in 2-player multiplayer.

## USER INTERFACE & EXPERIENCE:

**General:**

- Added support for Keyboard-Only mode.
- Seeds are now 12 characters. Internal seed is larger.
- Added a "Share" button on the map screen which takes a screenshot of the entire map.
- Added a "Listening For Input" popup when remapping inputs or bindings.
- Added a text toast informing the player whenever they cannot remap a specific key/button to an input for the current control scheme.
- If a binding is missing, a gray hyphen is now rendered rather than nothing in input settings.
- Updated padding, font sizes, and color for the input remap screen.
- Users are now informed if Steam Input is activated or not in a more explicit way.
- Set all controller input deadzones to 0.5. Joysticks should feel less sensitive and trigger buttons require them to be pressed in slightly more before the input is received now.
- When using a controller, the game now always tries to initially hover a map node connected to your current one when you have **Winged Boots** relic.
- Added a selection reticle for potion holders.
- Updated generic selection reticle asset to improve contrast and resolution. Rendered very slightly smaller as well.
- Adjusted hotkey icon size and position in top bar.
- Updated input glyph positions and sizes on various buttons and on the map, merchant, combat, and character select screens.
- Updated "Change Profile" button to render a larger input glyph.
- Updated keyboard-mode glyph asset.
- Removed faint white outline from game cursor.
- Scrollbar in settings screen is now placed better in non-standard aspect ratios.
- Made it so players can confirm card selection via E again.
- Cards with on-end-turn effects are played faster when there are many.
- Run timer is now paused while the Pause Menu is open during a single player match.
- Sped up Epoch unlock VFX if playing in fast mode.
- Added event combat encounters to the Bestiary.
- Created a stats section in the Bestiary.
- Hid some duplicate moves of some monsters in the Bestiary.
- Made minor adjustments to the selection arrow in the Bestiary monster list.
- Disabled left/right paging for Stats screen as Achievements screen doesn't exist yet.
- Relics skipped in the **Merchant??** event are now recorded in the Run History.
- Players can now see rooms affected by **Fur Coat** relic in the Run History.
- **Room Full of Cheese** event now shows the cards that were skipped in the Run History.

**Regent:**

- **Pale Blue Dot** power now counts down the number of cards that need to be played before it procs next to its icon.

**Multiplayer:**

- You can now play nearly any potion on another player.
- Map ink circles are now the same for all players.
- Made the UI for joining a friend's run nicer looking.
- Added "Failed to Join" text VFX.
- The host now sees an error when a client fails to join the lobby.
- Invite button is now shown when the overlay is disabled and displays an error.
- Version mismatch error now contains more information.
- Removed "Waiting for Players" screen when dying to The Architect.

## ART:

- Added low HP idle animations for all playable characters.
- Added character form VFX.
- Added Defect Orb passive activation VFX.
- Added card removal VFX.
- Added new card Exhaust VFX.
- Updated Ironclad cast animation.
- Updated Necrobinder slash animation and VFX.
- Added **Aeonglass** animations.
- Updated **Gremlin Merc** animations.
- If you kill **Gremlin Merc** before it steals any of your gold, Fat Gremlin now spawns without holding a bag of gold.
- Added fire to **Test Subject** burn animation.
- Changed **Dexterity Potion** back to being green.
- Added assets for **Dowsing Rod** and **Neow's Sacrifice** relics and **Ambergris** potion.
- Added portrait art for **Wither** status card.
- Added Epoch art for:

- Ablaze
- Annihilation
- Arise
- Darv
- Futility
- Preon
- Slumber
- Spireborn
- The Architect
- The Pact
- To Kill

## WRITING:

**General:**

- Big Game Hunter modifier's hovertip now specifies that it makes elites drop rare card rewards instead of just "better rewards".
- Clarified that **Silken Tress** relic applies only to the player who chose it in multiplayer.
- **Diamond Diadem** relic's wording has been updated to be clearer about how long its effect lasts.
- **Helix Drill** card's wording has been updated to be clearer that it only counts energy spent on other cards.
- Fixed **Slow** power description not displaying its active multiplier in the hovertip.
- When the word "potions" shows up in option descriptions or relic descriptions it's now gold and capitalized.
- Updated wording for map share UX to feel less stiff.
- Renamed "Full Keyboard" → "Keyboard-Only".

**Localization:**

- Added Traditional Chinese (zht) and Indonesian (ind) as selectable languages with English fallback.
- Added Traditional Chinese fonts (Source Han Serif TC, Noto Sans Mono CJK TC).
- Updated translations for various languages.
- Fixed translation errors in various languages.

## BUG FIXES:

**General:**

- Uncommon cards show up slightly more often (as intended).
- When shuffling, you can no longer discern the shuffle order by looking at your Discard Pile.
- Fixed many instances of weird RNG behavior (shoutout to tckmn!).
- Fixed several ways you could obtain duplicate relics on the first run.
- Fixed crash when returning to the main menu while a card-selection prompt is open.
- Fixed a crash that could occasionally happen after closing and then re-opening the single-card inspection screen.
- Fixed an error when a card finished moving to a pile after combat ended.
- Fixed an error when a combat ended while cards were still shuffling.
- Failing to load a saved run now shows an error instead of silently disabling Continue.
- Saving is now retried when antivirus or cloud sync briefly blocks the save file.
- Details are now included in the Steam initialization error popup.
- Fixed an issue where an Epoch unlock earned at the end of a run could be lost, permanently blocking Timeline progress. Affected saves are repaired on load.
- Fixed an issue where leaving the game over screen mid-animation could award an Epoch unlock without spending the score that paid for it.
- Fixed an issue where badges earned at the end of a run could be lost if you left the game over screen early or had already earned every unlock.
- Fixed Epoch paginator arrows becoming unclickable.
- "Power wears off" notification no longer blocks mouse input.
- Placeholder Orb numbers no longer briefly show up at the start of combat.
- VSync setting in Graphics options now shows the correct On/Off state after restarting.
- Correctly snapped scrolling menus at low FPS.
- Fixed combat music sometimes continuing on the map and other non-combat screens.
- Fixed input being blocked after clearing map drawings while in map drawing mode.
- Fixed map drawings breaking when lines are cleared while a stroke is in progress.
- Fixed opening the settings screen when you hit Escape to close the potions popup.
- Run timer no longer starts a run hidden if you have "always show run timer" toggled on.
- The Run History is now centered on ultrawide screens.
- Fixed Run History death quote getting cut off if it is too long.
- Run History now shows the boss icon for old runs that fought **The Doormaker**.
- Fixed stats screen showing that you have discovered more monsters than exist in the game.
- Fixed softlock when opening the Bestiary in the treasure room from a fresh save/reload.

**Controller:**

- Fixed broken controller navigation to the "mod settings" button in the main menu.
- Fixed controller navigation breaking if you opened up the deck view while holding a non-targeted card.
- Fixed being able to controller navigate to the card grid while inspecting a card in the Compendium.
- Fixed focus not returning to cards after rerolling rewards on controller.
- Disabled controller navigation to Orbs while in another card selection screen.
- Fixed being able to controller navigate to **Byrdpip**.

**Daily Runs:**

- Fixed the Daily Run leaderboard failing to load for some dates.
- Fixed not being able to see past scores in the Daily leaderboard if you did not submit a score on that day.
- Fixed score warning not showing up correctly if you have already uploaded a Daily score for today.

**Ancients:**

- Fixed Elite music playing in **Neow** room if you died to an Elite your previous run.
- **Tezcatara's Toybox** relic no longer flashes once it is used up.
- Fixed **Tanx's Whistle** card preview and tooltip snapping to the top-left corner when hovered.
- Fixed **Dowsing** card missing its card preview for Abundance.

**Enemies:**

- It is no longer possible to get two encounters in a row that contain **Slime** monsters.
- It is no longer possible to get two encounters in a row that contain **Snapping Jaxfruit** monster.
- If an attack with Replay kills the last enemy on its first play, the subsequent plays are now skipped.
- Fixed a visual issue where, if the player uses Thorns to kill a monster that spawns more monsters on death (i.e. Axebot, Phrog Parasite), the dead monster's attack wouldn't visually occur until after the new monsters had spawned.
- Fixed an issue where **Skulking Colony** could end up healing itself after a player with **The Boot** relic hits it when its HP loss maximum has already been exceeded for the turn.
- **Tough Egg** can now have 1 more max HP.
- Fixed awkward delay when Osty and **Kaiser Crab** spawn at start of combat.
- Fixed Form VFX not flipping when the player flips direction during the **Kaiser Crab** combat.
- Fixed **Fabricator** animations that weren't playing right.
- Fixed **Fat Gremlin** sometimes spawning without a bag of money when it should.
- Fixed **Cubex Construct**'s intents overlapping with its body if it takes damage while transitioning to its charging animation.
- Fixed **Inklet**'s head getting cut off when the Doom animation plays.
- Fixed **Slithering Strangler** playing its hurt animation for its Thwack move.
- Fixed **Torch Head Amalgam**'s laser attack VFX not reaching the player when it was shrunk.
- Fixed **Wrigglers** spawning offscreen in 4:3 aspect ratios.
- Added missing **Tunneler** animations to the Bestiary.
- **Tunneler** animations can now be played regardless of Burrowed state in the Bestiary.
- **Decimillipede** now correctly displays stats in the Bestiary.
- **Kaiser Crab** no longer covers the monster title in the Bestiary.
- Fixed error when **Tough Egg** hatches in the Bestiary.
- Fixed impact SFX playing for **Kin Priest** Soul Beam move in the Bestiary.
- Fixed the **Lost and Forgotten** monster's death particles continuing to play if you interrupt the animation in the Bestiary.
- Repeatedly clicking **Knowledge Demon**'s Curse of Knowledge move in the Bestiary no longer causes its animation to stop playing.

**Events:**

- Fixed an issue where Vigor was only applying to the first of **Mad Science** card's 3 hits when it was created with the Violence option.
- Fixed **Mad Science** generating a card in your Hand not scaling **Supermassive**.
- Fixed death quotes for dying to **Punch Off** and **Mysterious Knight**.
- Fixed **Planisphere** relic proccing twice in the **Battleworn Dummy** event.
- **Forbidden Grimoire** card's card removal reward will now show up after the **Battleworn Dummy** fight if you are successful.
- Fixed typo in **Lantern Key** event.

**Potions & Relics:**

- **Bellows** relic now properly upgrades the extra card when **Pendulum** relic triggers on turn 1.
- Fixed an issue where **Bone Tea** relic could fail to upgrade your hand if you also have **Toolbox** relic.
- Fixed an issue where, if a player auto-plays their entire start-of-turn hand draw (via an effect like Hellraiser), **Unceasing Top** relic may not trigger despite their hand being empty.
- Fixed **Pollinous Core** relic counting up past 4 if you end combat after your turn but before you draw cards.
- **Lasting Candy** relic no longer procs if the reward set has no card rewards.
- **Unsettling Lamp** relic no longer gets used up if the target's Artifact blocks the debuff.
- **White Beast Statue** relic no longer decreases your normal potion odds when it guarantees potion rewards, to prevent you never rolling potions again if it gets removed mid-run.
- Fixed **Flex Potion** description appearing twice on the character when applied.
- Fixed a softlock that could occur when throwing multiple **Foul Potions** in quick succession.

**Colorless Cards:**

- Fixed **Entropy** card being able to transform Curses into Ascender's Bane.

**Ironclad:**

- After playing **Juggling** card, when your third attack also auto-plays an attack (for example, Uproar card), Juggling now properly copies the original attack instead of the auto-played attack.
- **Hellraiser** card no longer infinitely plays **Shining Strike** card when it's enchanted with Swift.

**Silent:**

- **Expose** card now properly triggers **Hand Drill** relic.
- **Nightmare** card can no longer be generated during combat.

**Regent:**

- Fixed **Minion Strike** card becoming stuck on screen if played too quickly.
- Fixed **History Course** copy of **Shining Strike** card going back into your Draw Pile.

**Necrobinder:**

- Fixed a crash that could happen when an enemy dies to Doom.
- **Transfigure** card can no longer be generated during combat.
- **Cruelty** card's extra damage now properly applies to Osty attacks.
- Fixed **Enfeebling Touch** + **Misery** causing enemies with Artifact to gain Strength next turn.
- Fixed Necrobinder's power-up cast animation not playing.

**Defect:**

- Fixed Defect's power-up animation freezing when interrupted while playing **Echo Form** card.
- Fixed the **Subroutine** power hovertip typo if you have more than 4 Subroutines.
- **Feral** card no longer puts the **History Course** copy of **Helix Drill** card back into your Hand.
- Fixed **Gold Plated Cables** relic flashing if the Orb's passive is not firing.
- **Gold Plated Cables** relic now procs if **Emotion Chip** relic triggers Orbs at the start of your turn.

**Multiplayer:**

- Fixed more instances of Daily Runs diverging based on player net ID.
- Event RNG seeds are now consistent across all multiplayer Daily Runs.
- Fixed state divergence that occurred if one player took a long time to enter combat.
- Fixed divergence caused by the host changing the Ascension as the game is starting.
- Fixed divergence caused by discarding a potion while moving to a new map point.
- Fixed state divergence caused by taking a reward as you are transitioning to another map point.
- Fixed divergence occurring when a player chose a rest site option while transitioning to another map point.
- Fixed divergence when a player has **Music Box** relic and combat ends while making a choice.
- Fixed divergence when playing cards very fast while killing yourself.
- Fixed card effects breaking when a player dies while their own card is still resolving (e.g. to Thorns).
- Fixed a crash that could happen when selecting a relic at a treasure room.
- Fixed bug where you could set yourself ready for the next act two acts in advance.
- Unreadying in a loaded Daily Run lobby no longer leaves you readied up.
- The host's screen is no longer blank when hosting a run from a save.
- The real reason is now shown when a join fails before connecting.
- Resolved issue where, if Epochs were deprecated, that player could no longer join multiplayer games.
- Players can Abandon Run outside of combat again.
- Fixed mouse remaining hidden forever if the game ended while in potion targeting.
- Player icons are now rendered above the ink circle on the map.
- If you open a screen during a treasure room relic fight, the hands now behave correctly.
- **Jungle Maze Adventure** event no longer appears if a player doesn't have the HP for the solo option.
- Fixed **Ghost Seed** relic adding Ethereal to Strikes and Defends generated by other players during combat.
- Fixed chosen card getting stuck if another player gives you the **Touch of Insanity** potion.
- If **Huddle Up** triggers a choice for another player, the playing player is no longer blocked from playing cards.
- Fixed **Unmovable** card causing other players to receive double Block from **Demonic Shield** card when it was not the first Block card played.

## PERFORMANCE:

- Compressed Crystal Sphere event and Vantom background assets.
- Preloaded only the glyphs the bold and italic faces can render.
- Reduced hitch on Punch Off and The Lantern Key events.
- Reduced stuttering when text appears for the first time in Japanese, Chinese, Korean, Thai and Russian.

## MODDING:

- Unmodded saves are now copied to the modded save directory if you are launching modded for the first time, with a popup explaining the process.
- Fixed an issue where unmodded saves were not copied for players who had previously declined mod loading.
- Saves are no longer copied to the modded save directory if a player declined mod loading but left their mods installed.
- Fixed issues in multiplayer where players with the same mods would get "version mismatch" (thanks to OLC of RitsuLib for helping out with this!)

- Build the serialization cache from ModelDb instead of via reflection
- Sort content by ModelId instead of typename
- Use more tiebreakers when sorting
- Sort epoch IDs in `ModelIdSerializationCache`
- Exclude non-gameplay-affecting models from the `ModelIdSerializationCache` hash

- Merged SavedPropertySerializationCache into ModelIdSerializationCache. SavedProperties are now included in its hash.
- SavedProperty IDs from mods are now properly sorted for multiplayer.
- Content from mods with affectsGameplay set to false is now sorted to the end of ID maps.
- The correct mod lists are now logged in the non-gameplay mod mismatch warning.
- Added controller support for the Modding menu.
- Fixed Steam mods causing ReflectionHelper.ModTypes to return nothing.
- Mods can now register multiple assemblies.
- SerializableSave.NumReloads is now consistent across peers.
- Steam mod now takes precedence over local if the version is greater.
- Batched the Steam Workshop call for checking branch support, meaning mods should load faster.
- Began publishing STS2.dll XML documentation.
- The "Report Bug" button is no longer shown in error dialogs when any player in a session has mods.
