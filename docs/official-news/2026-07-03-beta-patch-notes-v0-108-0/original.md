---
date: "2026-07-03"
version: "v0.108.0"
title: "Beta Patch Notes - v0.108.0"
source: "https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1836506165569491"
author: "demileaf"
---

For all you beta testers who have been chomping at the bit for a new patch to chew on... here it is! Today's beta patch brings a ton of fixes, balance changes, some UX additions, a BIG batch of new multiplayer cards, and new art additions--including some animations for the latest boss, Aeonglass!

![](https://clan.fastly.steamstatic.com/images/44971832/469e5b0ea6846f4009611dc50e8af071021b67ec.gif)

We've also got some big updates in the modding realm to call out...

### Modding Updates

Thank you to our incredible community of modders (and mod players) for all of your feedback since we released Steam Workshop support! We've got quite a few changes that will be relevant to modders at the bottom of the notes, but first we wanted to specifically call out some major fixes and changes that affect players.

Previously, when launching the game with mods enabled for the first time, it appeared as though your progress was wiped. That is because unmodded saves are saved separately from your modded ones as a protective measure, and we defaulted to having the modded saves start entirely from scratch. Now, your unmodded saves are automatically copied over to the modded saves and a popup appears to explain the process to new mod users. They are still saved separately, though, which means any progress you make when playing with mods enabled will not be reflected after switching to unmodded and vice versa.

Additionally, players have been frequently encountering version conflict errors when doing modded multiplayer runs. We're introducing some fixes which should reduce those instances and more detailed error popups when they do occur. Note that we still recommend having the same exact mods enabled as your teammates, particularly if they actually affect gameplay and are not merely cosmetic.

We plan for these changes to come to the main branch soon, so non-beta-testers can keep an eye out for that! For more info on modding and playing with mods, check the patch notes from Major Update 2. Now it's time for the rest of today's patch notes...

### CONTENT & BALANCE:

**General:**

- End-of-turn Orb passives now trigger before Doom

- This means that, if a player would die to self-Doom (ex. from Neurosurge card) but also has an Orb with an end-of-turn passive that would kill the last remaining enemy (ex. Lightning Orb while the enemy is at 3 HP), they will now win the combat instead of losing it after ending their turn

**Enemies:**

- Buffed **Torch Head Amalgam**: increased damage at Ascension 9

- Weak Tackle damage increased from 14(15) → 14(16)
- Tackle damage increased from 18(19) → 18(22)

**Potions & Relics:**

- Changed **Regen Potion**: its healing effect now triggers before Doom, preventing the player from dying to self-Doom in certain situations

**Ironclad:**

- Nerfed **Colossus** card: Block gain increased from 5(8) → 4(7)
- Nerfed **Crimson Mantle** card: Block gain decreased from 8(10) → 7(10)
- Buffed **Howl From Beyond** card: Damage increased from 16(21) → 18(24)
- Buffed **Setup Strike**: Strength gain increased from 2(3) → 3(4)

**Silent:**

- Reworked **Outbreak** card: "Every 3 times you apply Poison, deal 15 damage to ALL enemies." → "Whenever you apply Poison, deal 3(4) damage to ALL enemies."
- Buffed **Anticipate** card: Dexterity increased from 2(3) → 2(4)
- Buffed **Flick-Flack** card: damage increased from 6(8) → 7(9)
- Nerfed **Tracking** card: "Weak enemies take double damage from Attacks." → "Weak enemies take 50% more damage from Attacks."

**Regent:**

- Buffed **Devastate** card: damage increased from 30(40) → 35(45)
- Buffed **Divine Destiny** relic: Star gain increased from 6 → 7
- Nerfed **GUARDS!!!** card's **Minion Sacrifice** token: Block gain decreased from 8(11) → 7(10)
- Buffed **Resonance** card: Star cost decreased from 3 → 2

**Necrobinder:**

- Buffed **Haunt** card: HP loss increased from 6(8) → 7(9)
- Buffed **Reave** card: damage increased from 9(11) → 10(13)
- Buffed **Soulstorm** card: additional damage increased from 2(3) → 4(6)

**Defect:**

- Nerfed **Compact** card's **Fuel** token:

- No longer draws cards
- Upgrade changed from increasing card draw → granting 1 additional energy

- Buffed **Momentum Strike** card: damage from 10(13) → 11(15)
- Changed **Scrape** card: no longer discards cards whose energy costs are temporarily set to 0 by global temporary effects (ex. Pounce card's "The next Skill you play is costs 0 Energy" effect)
- Changed **Shatter** card: now Exhausts

**Multiplayer:**

- Added new multiplayer **Ironclad** cards:

- **Midnight**: Rare - Attack - Cost 12 - "Deal 99(120) damage. Costs 1 less for each card Exhausted this combat by ANYONE."
- **Blaze**: Uncommon Skill - Cost 2 - "Give another player 5(7) Strength."
- **Outrage:** Uncommon - Attack - Cost 0 - "Deal 9(13) damage. Add a copy of this card into EVERYONE'S Discard Pile."

- Buffed and slightly reworked **Tank** card: "Take double damage from enemies. Allies take half damage from enemies." → "Take 50% additional damage from enemies. Allies take 50% less damage from enemies."
- Added new multiplayer **Silent** cards:

- **Blade Symphony**: Uncommon - Skill - Cost 1 - "Add 2(3) Shivs to ALL players Hands."
- **Concoct**: Uncommon - Skill - Cost 0 - "Another player's Attacks apply 3(4) Poison this turn."
- **Fade**: Uncommon - Skill - Cost 0 - "Retain. Another player gains 6(9) Dexterity this turn."

- Changed **Flanking** card: rarity increased from Uncommon → Rare
- Added new multiplayer **Regent** card:

- **Plot**: Uncommon - Skill - Cost 1 - "Next turn, ALL players draw 2(3) cards."
- **Constellation**: Uncommon - Skill - Cost 0 - "Another player draws 1 card, gains , and gains 9(12) Block."

- Added new multiplayer **Necrobinder** card:

- **Underworld**: Uncommon - Skill - Cost 2 - "Whenever other players deal attack damage this turn, apply that much Doom. Exhaust. (Lose Exhaust)"
- **Soulbound**: Uncommon - Power - Cost 1 - "(Innate.) Choose an ally. Whenever you create a Soul, add a Soul to their Draw Pile."
- **Cacophony**: Rare - Power - Cost 2 - "Every 33 cards drawn by ALL players, deal 66(99) damage to a random enemy."

- Changed **Legion of Bone** card: no longer Exhausts
- Added new multiplayer **Defect** cards:

- **Hibernate**: Uncommon - Skill - Cost 2 - "Channel 2(3) Frost. This turn, your Frost grants ALL allies Block."
- **One For All**: Rare - Power - Cost 1 - "EVERYONE'S 0 energy Attacks deal 3(4) additional damage."
- **Imitation Learning**: Rare - Skill - Cost 1 - "Choose another player. The next 2(3) times they play a Power, you play a copy of it. Exhaust."

- Changed **Ignition** card: rarity decreased from Rare → Uncommon
- Added new multiplayer Colorless card:

- **The Ball**: Uncommon - Attack - Cost 1 - "Deal 10 damage. Increase this card's damage by 15(25) this combat and give it to a random ally."

- Nerfed enemy block scaling in 2-player multiplayer
- Nerfed **Beacon of Hope** card: cost increased from 1 → 2

### USER INTERFACE & EXPERIENCE:

**General:**

- Added a Randomize button to the Custom Run screen which randomly selects 2 good modifiers and 1 bad one, and randomizes the selected character
- The run timer is now paused while the Pause Menu is open during a single player match
- Added a selection reticle for potion holders
- Created a stats section in the Bestiary
- Added event combat encounters to the Bestiary
- Hid moves with duplicate animations for some monsters in the Bestiary
- Slightly adjusted selection arrow in Bestiary monster list
- Players can now see which rooms were affected by **Fur Coat** relic in the Run History
- Relics skipped in the **Merchant??** event are now recorded in the Run History
- **Room Full of Cheese** event now shows the cards that were skipped in the Run History
- All controller input deadzones have been set to 0.5, meaning joysticks should feel less sensitive and trigger buttons must be pressed in slightly more before the input is received now

**Regent:**

- **Pale Blue Dot** power now counts down the number of cards that need to be played before it procs next to its icon

**Multiplayer:**

- Version mismatch error now contains more information

### ART:

- Added portrait art for **Wither** status card
- Added **Aeonglass** animations
- Updated **Gremlin Merc** animations
- If you kill **Gremlin Merc** before it steals any of your gold, Fat Gremlin now spawns without holding a bag of gold
- Added Epoch art for:

- Ablaze
- Arise
- Darv
- Preon
- Slumber
- Spireborn
- The Architect
- The Pact

- Added Orb passive activation VFX
- Added card removal VFX
- Added new card Exhaust VFX

### WRITING:

- Big Game Hunter modifier's hovertip now specifies that it makes elites drop rare card rewards instead of just "better rewards"
- **Helix Drill** card's wording has been updated to be clearer that it only counts energy spent on other cards
- Clarified that **Silken Tress** relic applies only to the player who chose it in multiplayer
- Updated translations for various languages

### BUG FIXES:

**General:**

- Details are now included in the Steam initialization error popup
- Saving is now retried when antivirus or cloud sync briefly blocks the save file
- Run timer no longer starts a run hidden if you have "always show run timer" toggled on
- Fixed crash when returning to the main menu while a card-selection prompt is open
- Fixed broken controller navigation to the "mod settings" button in the main menu
- Fixed controller navigation breaking if you opened up the deck view while holding a non-targeted card
- Fixed combat music sometimes continuing on the map and other non-combat screens
- Fixed Epoch paginator arrows becoming unclickable
- Fixed softlock when opening the Bestiary in the treasure room from a fresh save/reload
- Fixed Run History death quote getting cut off if it was too long
- Run History now shows the boss icon for old runs that fought **The Doormaker**
- Fixed stats screen showing that you have discovered more monsters than there exists in game
- Fixed the Daily Run leaderboard failing to load for some dates
- Fixed many instances of weird RNG behavior (shoutout again to tckmn!)
- Fixed being able to controller navigate to **Byrdpip**

**Ancients:**

- Fixed Elite music playing in **Neow** room if you died to an Elite your previous run
- **Tezcatara's Toy Box** relic no longer flashes once it is used up

**Enemies:**

- It is no longer possible to get two encounters in a row that contain **Slime** monsters
- It is no longer possible to get two encounters in a row that contain **Snapping Jaxfruit** monster
- If an attack enchanted with Replay kills the last enemy on its first play, the subsequent plays are now skipped

- This fixes an issue where, if a card with Replay is enchanted with Corrupted, the player would lose extra HP even after the first play killed the last enemy

- Fixed a visual issue where, if the player uses Thorns to kill a monster that spawns more monsters on death (i.e. Axebot, Phrog Parasite), the dead monster's attack wouldn't visually occur until after the new monsters had spawned
- Fixed **Cubex Construct**'s intents overlapping with its body if it takes damage while transitioning to its charging animation
- Fixed **Inklet**'s head getting cut off during Doom animation
- Fixed **Slithering Strangler** playing its hurt animation for its Thwack move
- Fixed **Wrigglers** spawning offscreen in 4:3 aspect ratios
- Fixed **Torch Head Amalgam**'s laser attack VFX not reaching the player when it was shrunk
- Fixed impact SFX playing for **Kin Priest** Soul Beam move in the Bestiary
- **Kaiser Crab** no longer covers the monster title in the Bestiary
- Repeatedly clicking **Knowledge Demon**'s Curse of Knowledge move in the Bestiary no longer causes its animation to stop playing
- Added missing Tunneler animations to the Bestiary
- **Tunneler** animations can now be played regardless of Burrowed state in the Bestiary

**Potions & Relics:**

- Fixed an issue where **Bone Tea** relic could fail to upgrade your hand if you also have **Toolbox** relic
- Fixed an issue where, if a player auto-plays their entire start-of-turn hand draw (via an effect like Hellraiser), **Unceasing Top** relic may not trigger despite their hand being empty
- Fixed **Planisphere** relic proccing twice in the **Battleworn Dummy** event
- Fixed **Pollinous Core** relic counting up past 4 if you end combat after your turn but before you draw cards
- **Lasting Candy** no longer procs if the reward set has no card rewards
- **Unsettling Lamp** relic no longer get used up if the target's Artifact blocks the debuff
- **White Beast Statue** relic no longer decreases your normal potion odds when it guarantees getting potion rewards to prevent you never rolling potions again if it gets removed mid-run
- Fixed several ways you could obtain duplicate relics on the first run
- Fixed **Flex Potion** description appearing twice on the character when applied
- Fixed a softlock that could occur when throwing multiple **Foul Potions** in quick succession

**Events:**

- Fixed an issue where Vigor was only applying to the first of **Mad Science** card's 3 hits when it was created with the Violence option
- Fixed death quotes for dying to **Punch Off** and **Mysterious Knight** encounters

**Colorless Cards:**

- Fixed **Entropy** card being able to transform curses into Ascender's Bane

**Ironclad:**

- **Hellraiser** card no longer infinitely plays **Shining Strike** card when it's enchanted with Swift

**Silent:**

- **Nightmare** card can no longer be generated during combat

**Regent:**

- Fixed **Minion Strike** card becoming stuck on screen if played too quickly
- Fixed **History Course** copy of **Shining Strike** card going back into your Draw Pile
- Fixed **Supermassive** card not scaling from cards generated by **Mad Science** card

**Necrobinder:**

- Fixed a crash that could happen when an enemy dies to Doom
- **Transfigure** card can no longer be generated during combat
- Fixed **Enfeebling Touch** + **Misery** combo causing enemies with Artifact to gain Strength next turn
- **Forbidden Grimoire** card's card removal reward will now show up after **Battleworn Dummy** fight if you are successful

**Defect:**

- Disabled controller navigation to Orbs while in another card selection screen
- Fixed the **Subroutine** power hovertip typo if you have more than 4 Subroutines
- **Feral** card no longer puts **History Course** copy of **Helix Drill** card back into your hand
- Fixed **Gold Plated Cables** relic flashing if the Orb's passive is not firing
- **Gold Plated Cables** relic now procs if **Emotion Chip** relic triggers Orbs at the start of your turn

**Multiplayer:**

- Fixed even more instances of Daily runs diverging based on player net ID
- Fixed divergence caused by discarding a potion while moving to a new map point
- Fixed bug where you could set yourself ready for the next act two acts in advance
- Fixed state divergence caused by taking a reward as you are transitioning to another map point
- Fixed divergence caused by host changing the Ascension as the game is starting
- Fixed divergence when player has **Music Box** relic and combat ends while making a choice
- Fixed divergence when playing cards very fast while killing yourself
- Fixed **Ghost Seed** relic adding Ethereal to Strikes and Defends generated by other players during combat
- If you open a screen during a treasure room relic fight, the hands now behave correctly
- Fixed mouse remaining hidden forever if the game ended while in potion targeting
- Event RNG seeds are now consistent across all multiplayer Daily Runs
- **Jungle Maze Adventure** event no longer appears if a player doesn't have enough HP for the solo option

### PERFORMANCE:

- Compressed various combat background assets

### MODDING:

- Unmodded saves are now copied to the modded save directory if you are launching modded for the first time
- Added controller support for Modding menu
- Fixed Steam mods causing ReflectionHelper.ModTypes to return nothing
- SavedProperty IDs from mods are now properly sorted for multiplayer
- Content from mods with affectsGameplay set to false are now sorted to end of ID maps
- Mods can now register multiple assemblies
- SerializableSave.NumReloads is now consistent across peers
- Steam mod now takes precedence over local if the version is greater
- Batched the Steam Workshop call for checking branch support, meaning mods should load faster
- Began publishing STS2.dll XML documentation
- Fixed issues in multiplayer where players with the same mods would get "version mismatch" (thanks to OLC of RitsuLib for helping out with this!)

- Build the serialization cache from ModelDb instead of via reflection
- Sort content by ModelId instead of typename
- Use more tiebreakers when sorting
- Sort epoch IDs in `ModelIdSerializationCache`
- Exclude non-gameplay-affecting models from the `ModelIdSerializationCache` hash

- The correct mod lists are now logged in the non-gameplay mod mismatch warning
