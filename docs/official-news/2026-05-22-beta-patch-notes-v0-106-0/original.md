---
date: "2026-05-22"
version: "v0.106.0"
title: "Beta Patch Notes - v0.106.0"
source: "https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1833334318570400"
author: "demileaf"
---

It's time for another beta patch! A main focus of this one is enemy balance, including a rework of Infested Prism, more adjustments to Skulking Colony, and some changes to the new boss, Aeonglass. We've also got quite a few more Epoch art additions and more cool combat animations for some of the characters! Let's dive in...

![](https://clan.fastly.steamstatic.com/images/44971832/d356154903784c0317d29d7925f22908a1cccfb1.gif)

### CONTENT & BALANCE

**General:**

- If an enemy has Vulnerable and a player takes an extra turn via **Pael's Eye** relic, the enemy's Vulnerable amount no longer decrements before the player's extra turn

- This change also applies to other powers that decrement at the end of the enemy's turn (Frail, Intangible, Weak, etc.)

**Ancients:**

- Changed **Neow's Scroll Boxes** **Silken Tress** relics: swapped downsides and pools

**Ironclad:**

- Buffed **Drum of Battle** card: Energy gain on Exhaust effect now respects Replay and card play duplication effects like Duplicator potion
- Buffed **Howl From Beyond** card: trigger moved from start of turn -> end of turn
- Buffed **Entrench** card: can now gain extra Block from enchantments like Nimble
- Buffed **Unrelenting** card: damage increased from 12(18) -> 14(20)

**Silent:**

- Reworked and renamed **Follow Through** card:

- Rarity increased from Common -> Uncommon
- Renamed -> **Scare**

- Changed **Predator** card: rarity decreased from Uncommon -> Common
- Buffed **Pounce** card: damage increased from 12(18) -> 14(20)

**Regent:**

- Buffed **Furnace** card: Forge increased from 4(6) -> 5(7)
- Nerfed **Minion Sacrifice** card: Block decreased from 9(12) -> 8(11)
- Changed **Sealed Throne** card: upgrade changed from Innate -> lowering Energy cost by 1
- Changed **Astral Pulse** card: damage changed from 14(18) -> 6x2(8x2)

**Necrobinder:**

- Buffed **Death March** card: additional damage increased from 3(4) -> 4(6)
- Nerfed **Debilitate** card: debuff decreased from 3(4) turns -> 2(3) turns

**Defect:**

- Changed **Fusion** card:

- Energy cost decreased from 2(1) -> 1
- Now Exhausts, and loses Exhaust on upgrade

- Buffed **Synthesis** card: damage increased from 12(18) -> 14(20)
- Nerfed **Shatter** card: damage decreased from 11(15) -> 7(11)

**Colorless Cards:**

- Nerfed **Fasten** card: additional Block decreased from 5(7) -> 4(6)

**Potions & Relics:**

- Buffed **Mummified Hand** relic: now randomly chooses cards in a way that's more favorable to the player

- Priority 1 is cards that currently cost 1+ AND with a base cost of 1+ before any current cost modifications
- Priority 2 is cards that currently cost 1+ regardless of base cost
- Priority 3 is cards with a base cost of 1+ that have been temporarily modified to cost 0
- If none of these priorities match, a completely random card in the player's hand is chosen

**Enemies:**

- Reworked **Infested Prism**
- Changed **Aeonglass**:

- Adjusted moveset
- Reworked Wither status and Withering Presence power

- Changed **Haunted Ship**: Ramming Speed move removed, now alternates between Swipe and Stomp instead (always starting with Swipe after Haunt)
- Changed **Punch Construct**:

- Fast Punch move now leads into Strong Punch instead of Strong Punch leading into Fast Punch
- Ready move now leads into Fast Punch
- Now applies Frail instead of Weak

- Changed **Skulking Colony**:

- HP increased from 70(75) -> 75(80)
- Hardened Shell power increased from 15 -> 20
- Smash move removed, now uses Zoom move turns 1 and 2 instead
- No longer gains Block

### ART:

- Added new **Ironclad** heavy attack animation
- Added new **Necrobinder** cast animation
- Added new **Regent** Sovereign Blade animation
- Added Epoch art for:

- Booting Up
- Soup
- Poisoner
- Scribbles
- The Calm

- Updated Epoch art for:

- Hello Hello!
- Friendship

- Added alternate phobia mode art for:

- **Fossil Stalker**
- **Haunted Ship**
- **Skulking Colony**
- **The Obscura**
- **Twig Slime M**

### USER INTERFACE & EXPERIENCE:

- Improved **Byrdonis**'s UX

- Territorial power's hovertip now mentions Byrdonis by name
- Hovering over Byrdonis now also shows the Strength hovertip

- Updated the **Aeonglass** boss node icon
- Added the following enemies to the Bestiary:

- **Kaiser Crab** arms
- **Decimillipede**
- **Parafright**

- Added global rank paginator and adjust arrow positions to leaderboards
- Leaderboards in game over screen have been moved a bit higher to allow room for toggle
- Ability to see other player scores near your score in leaderboards
- Added controller navigation for the modifer icons on the top bar

### AUDIO:

- Added SFX for **Skulking Colony**
- Added SFX for when a card travels to a pile

### WRITING:

- Fixed some inconsistencies in **Jeweled Mask** relic's wording
- Fixed **Exterminate** card's wording to be more consistent with **Conflagration** card
- The word "Power" is no longer keyworded in **Lost Wisp** relic's description
- Added more translated text for various languages
- Fixed formatting, numeric values, and punctuation in various languages

### BUG FIXES:

**General:**

- Fixed vertical separators showing up for Daily Run screen even if no scores have been submitted
- Fixed an issue where, after switching between two languages and continuing a saved run, the hovertips for some items in the top bar could display in the wrong language
- Fixed an issue where, after winning an Elite combat and receiving a potion reward, your odds of receiving another potion reward in the next combat could sometimes go up instead of down
- Fixed being able to open the card inspection screen and toggle the View Upgrades button with the same button press
- Fixed broken controller navigation from relics to the reward screen if you clear out the top reward option
- Fixed cards in hand getting stuck if you try to play it while it is being transformed
- Fixed num keys for card selection not working if you mouse is too low on the screen
- If you load a save file with a deprecated boss, it can no longer roll the same double boss
- Controller input is now ignored when game window is unfocused
- Improved reliability of settings saves on startup
- Replayed cards show the appropriate amount of damage if powers wore off between replays
- Stopped cloud-syncing settings.save to prevent Steam Cloud conflicts
- Taking a card removal reward as the last reward no longer keeps the background faded out

**Ancients:**

- Fixed crash when selecting Ancient relic options
- Fixed **Neow's Kaleidescope** relic using up all of **Silver Crucible** relic's charges
- Fixed **Neow's Silken Tress** relic in multiplayer only affecting card rewards for the host

**Necrobinder:**

- **Funerary Mask** relic now shuffles Souls into Draw Pile before your hand is drawn

**Silent:**

- You can no longer play cards before **Tools of the Trade** triggers

**Defect:**

- Fixed spacing issue in the **Consuming Shadow** power hovertip
- **Scrape** no longer discards cards that cost zero Energy but have a Star cost

**Colorless Cards:**

- Fixed a softlock when playing **Anointed** card when your Draw Pile contains more cards than can fit in your hand
- Fixed being able to highlight creatures while the **Purity** card selection screen is open

**Potions & Relics:**

- Fixed broken controller nav from relics to card rewards after you reroll via the **Driftwood** relic
- Fixed completing **Orrery** relic's card selection in shop also buying/clicking on an item in the shop on controller

**Enemies:**

- Fixed **Aeonglass** first move intent showing a buff instead of a debuff
- Fixed **Aeonglass** background music
- Fixed **Ceremonial Beast** background music transition not lining up with when it gets stunned
- Fixed **Living Fog** missing its smoke
- Fixed **Kaiser Crab's Rocket Claw**'s charge up SFX looping forever
- Fixed **Test Subject** starting phase 2 with extra strength if it is killed by **Sleight of Flesh** via **Enfeebling Touch**
- Fixed **Thieving Hopper** softlock if you have no cards for him to steal
- Fixed VFX glitch when **Snapping Jaxfruit** dies to the **Reflect** power
- Fixed **Waterfall Giant** not respawning in the Bestiary when you kill him
- **Inklet** and **Vantom**'s Slippery power no longer loses a stack when fully blocking all damage from a hit
- **Skulking Colony**'s per-turn damage cap now properly resets at the start of both the player's and the enemies' turns, fixing an issue where Poison damage counted towards the damage cap on the players' next turn
- Fixed softlock after defeating **Infested Prism**
- **Zapbot**'s High Voltage power's hovertip now properly mentions that it gains Strength at the end of its turn instead of the start
- Added missing move names to the Bestiary
- Summoned monsters are now revealed in the Bestiary (you will have to defeat them once to reveal them)
- Hid **Lagvulin Matriarch**'s zzz particle when it is no longer sleeping in the Bestiary
- **Waterfall Giant** steam spouts no longer misbehave after death animation in Bestiary
- Fixed a few issues with **Test Subject** in the Bestiary

**Events:**

- Fixed **Crystal Sphere** event potion rewards giving potions of the wrong rarity
- You no longer get rewards from the **Battleworn Dummy** event after saving and quitting

**Multiplayer:**

- Potions are now canceled if they're targeted on a creature that was killed by another player
- Corrected **Test Subject** HP scaling in multiplayer
- Fixed state divergences related to event combats
- Fixed state divergence that would occur after **Crystal Sphere** event
- Fixed possible state divergence when event option resolution took a long time
- Relics with turn-number-specific triggers like **Candelabra** no longer skip their trigger when another player takes an extra turn with **Pael's Eye** relic on the triggering turn
- Exiting the Merchant room no longer skips history recording for remaining players
- **Orrery** relic card rewards no longer report the wrong cards to other players
