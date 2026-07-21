---
date: "2026-04-03"
version: "v0.102.0"
title: "Beta Patch Notes - v0.102.0"
source: "https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1828894815558978"
author: "demileaf"
---

Hey Slayers, we've got another Beta patch for you all this week!

Notably, this patch brings in some structural work for a system called Badges.

Badges may appear at the end of a run, noting various things that happened during a run like defeating a boss without losing HP, beating the game quickly, specific easter eggs, or pointing out that you forgot to spend your gold. They're meant as little reminders to let you know what was unique about each adventure.

While they only show up at the end of a run for now, they'll appear in your run history and stats screen later on. This is part of a series of upcoming changes to make runs feel distinct and improve scoring (more on that later!).

In terms of visual changes, there are some new art additions for various cards and a few enemies, as well as some cool new VFX for afflictions!

![](https://clan.fastly.steamstatic.com/images/44971832/32ecfd6863876dd8fb6e5d742bce2119c6d3c975.gif)

Up next, we've got some more commentary from Anthony on this patch's content reworks, so let's dive right into it.

### CONTENT:

**General:**

- Reworked Ascension 6 from Gloom - "Less rest sites." -> Inflation - "Removing cards from your deck at the Merchant is more expensive."

As mentioned last week, affecting map generation through Ascension has increasingly seemed to be a miss. It's often too hard to tell what change if any is a result of Ascension, or if it was just an unlucky map generation. Instead Ascension 6 will now make card removal at the Merchant start at 100 Gold, and will increase in price by 50 gold per removal instead of 25. I expect this to be felt significantly more than the old Gloom modifier.

**Silent:**

- Reworked **Blade of Ink** card: "Rare - Cost 1 - Skill | This turn, whenever you play an Attack, gain 2(3) Strength this turn." -> "Rare - Cost 1 - Skill - Add 2(3) Inky Shivs into your Hand."

- Added new Inky enchantment: "This card deals 2 additional damage and applies 1 Weak."


The Blade of Ink rework is an experiment with generating Enchanted cards mid combat. Definitely let us know how this feels or if Enchantments are best left to things like Events and Relics. I suspect this card will be high impact, and gives Silent another strong Weak applier.

**Necrobinder:**

- Reworked **Borrowed Time** card: "Uncommon - Cost 0 - Skill | Apply 3 Doom to yourself. Gain 1(2) Energy." -> "Uncommon - Cost 1 - Skill | Gain 4(6) energy. Cards cost an additional energy this turn."

This new Borrowed Time should go particularly well with high cost cards like Reap and Bury and help to flesh out other ways to play Necrobinder.

**Enemies:**

- Reworked **Doormaker** Grasp turn and power: now attacks for 20(23) and the power is reworked to "Whenever you play a card, lose 1 energy."

Doormaker's Grasp turn was too underwhelming, and overall lowered the difficulty of the fight too much. This new version should make the entire fight high stakes and actually give the player a difficult problem to puzzle over turn to turn instead of making the fight about just living until turn 4.

### BALANCE:

**Silent:**

- Buffed **Leading Strike**:

- Shivs increased from 1 -> 2
- Damage decreased from 7 -> 3

- Nerfed **Speedster**: damage decreased from 2(3) -> 1(2)
- Nerfed **Untouchable**: Block decreased from 7(9) -> 6(8)

**Potions & Relics:**

- Changed **History Course** relic: when it plays a copy of an X-cost card, it now uses your current energy for its X-value instead of the energy spent the last time the card was played. Note: It still plays the copy for free

**Enemies:**

- Buffed **Skulking Colony**:

- Now attacks on all turns
- Deals slightly more damage


**Ancients:**

- Buffed Nonupeipe's **Fur Coat** relic: now affects restocked Axebots and other enemies that enter combat during a fight

**Events:**

- Nerfed **Hungry For Mushrooms** event: Fragrant Mushroom option upgraded cards decreased from 3 -> 2
- Changed **Waterlogged Scriptorium** event:

- Prickly Sponge option gold cost lowered from 155 -> 99
- Tentacle Quill gold cost lowered from 65 -> 55

- Buffed **Whispering Hollow** event: Exchange option gold cost decreased from 50 -> 35+-9

### ART:

- Added portrait art for the following cards:

- Hotfix
- Modded
- Synthesis
- Sweeping Gaze

- Added new art for Ruby Raiders
- Added background art and VFX for Vantom boss fight
- Added new visuals for the following afflictions:

- Bound
- Entangled
- Galvanized
- Hexed
- Ringing
- Smog

- Updated empty chest VFX, smoke puffs

### USER INTERFACE & EXPERIENCE:

**General:**

- Adjusted and added placeholder assets for score lines in run summary and game over screen
- Adjusted timings and pauses for dialogue for Gremlin Merc
- Adjusted Cultist "Cacaw" dialogue timings
- Adjusted speech bubble positions for Damp and Calcified Cultists
- All monsters now have distinct speech bubble colors
- Players can now navigate between cards/potions/relics in the Timeline unlock screen on controller
- Players can now navigate to the top bar character portrait to see the Ascension hovertip on controller
- Pressing the top bar shortcut while already focusing on a top bar element now moves your controller focus back into your hand

**Multiplayer:**

- Better handling of intermittent Steam errors
- Disabled skipping chests in multiplayer. Too many people thought it was the Proceed button.

### WRITING:

**General:**

- **Foul Potion** description now specifies that it deals damage to "ALL players and enemies" instead of "EVERYONE", making it clearer that it does not hit Osty
- Fixed wording in the Statistics Total Playtime/Fastest win hovertip

**Localization:**

- Updated translations for various languages
- Fixed translation errors in the following languages:

- German
- Japanese
- Italian
- Spanish
- Thai
- Polish


### BUG FIXES:

**General:**

- Fixed broken Steam rich presence translations
- Fixed a crash that could occur on startup after a Steam Cloud sync conflict
- Fixed a crash that could occur when the game failed to start and tried to show an error dialog
- Fixed an issue where numbers were improperly formatted on the victory screen
- If there is an error while spawning a creature into combat, a placeholder is shown instead of softlocking the combat
- Fixed softlock caused by being able to use your controller to confirm a hand card selection while you are peeking
- Fixed being able to press down arrow to controller navigate the card grid while an upgrade/remove/transform preview is up
- Vulkan is now defaulted to on Intel iGPUs with known D3D12 stability issues

**Ironclad:**

- Fixed softlock when selecting a card to upgrade via **Armaments** card while **Lethality** power still applies

**Silent:**

- Fixed **Bullet Time** card getting played by **Mayhem** power not setting the cost of the cards in your hand to 0

**Potions & Relics:**

- Fixed **Unsettling Lamp** relic doubling the Strength you get from **Dominate** card
- If a potion is discarded just before combat ends, it's no longer forever unusable
- When clicking a potion that can be thrown at all enemies simultaneously, the potion popup now properly says "Throw" instead of "Drink"

**Enemies:**

- Fixed an issue where a few enemies' damage intents were occasionally showing the wrong number
- Fixed not being able to navigate from the relic inventory back to the combat in the **Mysterious Knight** fight on controller
- Removed broken text from **Thieving Hopper** enemy's Escape Artist power hovertip when playing in Polish
- Fixed **Soul Nexus** animation not playing correctly

**Ancients:**

- Removed the peek button from the card selection via Neow's **Hefty Tablet** relic
- Effects that occur before your turn now interact correctly with Vakuu's **Whispering Earring** relic
- Fixed Vakuu's **Whispering Earring** relic continuing to play cards after **Void Form** power ended the turn
- Fixed an issue where Vakuu's **Lord's Parasol** relic would occasionally only obtain some of the items that the Merchant sells
- Fixed being able to end the **Lord's Parasol** purchase animation on controller

**Events:**

- Fixed an issue where, if the player visited a combat-style event without actually starting a combat (i.e. choosing "Return the Key" at The Lantern Key event) and then visited the Merchant immediately after, throwing a Foul Potion at the Merchant would not grant the player any gold

**Multiplayer:**

- **Trash to Treasure** power no longer procs from other player's generated statuses
- Fixed softlock in **Thieving Hopper** fight if one person is dead when it tries to steal cards
- Fixed state divergence related to **Driftwood** and **Dream Catcher** relics
- Player name is now correctly displayed when hovering over your character
- When re-entering the multiplayer host screen, the Ascension is now correctly synced
- You can now reload after saving a game at a rewards screen with non-standard rewards
