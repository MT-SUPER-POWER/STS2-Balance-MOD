---
date: "2026-03-27"
version: "v0.101.0"
title: "Beta Patch Notes - v0.101.0"
source: "https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1828441623103381"
author: "demileaf"
---

Hey everyone, Anthony here. It's time for another Beta Patch where we test out experimental changes. Today we are going to do something a little different for the patch notes as I will be giving some insights into our design intent and thoughts behind various changes. Reminder that nothing in the beta branch is set in stone, expect lots of back and forth, and above all please let me know what you think by using the in-game feedback tool by hitting F2! I read countless pieces of player feedback and it has been instrumental in helping guide our process.

### CONTENT:

**Reworks and Reverts:**

**Silent:**

- Deprecated **Prepare** and **Prepared** returns: "Common - Skill - Cost 1 - "Discard 2 cards. Next turn, gain 2(3) Energy." -> "Common - Skill - Cost 0 - Draw 1(2) card(s). Discard 1(2) card(s)."

**Necrobinder:**

- Reverted (Buffed) **Capture Spirit** to its previous version: "Uncommon - Skill - Cost 1 - Enemy loses 2(3) HP. Add 2(3) Souls into your Draw Pile." -> "Uncommon - Skill - 1 - Enemy loses 3(4) HP. Add 3(4) Souls into your Draw Pile."
- Reverted (Buffed) **Borrowed Time** to its previous version: "Uncommon - Skill - Cost 0 - Apply 6(3) Doom to yourself. Gain 1 Energy." -> "Uncommon - Skill - Cost 0 - Apply 3 Doom to yourself. Gain 1(2) Energy."

Starting off, I am reverting the changes made last week to **Prepared**, **Borrowed Time**, and **Capture Spirit**. For Prepared, while I still think that the Sly synergies are overall too dominant of a strategy, Prepared is so integral to the Silent's core identity that I will be looking for a different approach to bring Sly in line in the future. Borrowed Time is likely to receive a complete rework in the future, but for now I prefer that the card is fun to play with in all decks.

**Regent:**

- Reworked **Arsenal** card: "Power - Rare - Cost 1 Whenever you play a Colorless Card, gain 1(2) Strength." -> "Power - Rare - Cost 1 (Innate.) Whenever you create a card, gain 1 Strength."

**Potions & Relics:**

- Reworked **Pendulum** relic: "Common - Whenever you shuffle your Draw Pile, draw a card." -> "Common - Every 3 turns, draw 1 card."

**Enemies:**

- Reworked and nerfed the **Doormaker**. Fight is now less random.

**Doormaker** is receiving another rework! Now each visual phase he is in has its own unique power to hamper the player. All of them are disruptive and will hopefully encourage interesting play, but should now be less random and reward player agency more.

### BALANCE:

**General:**

- Elites can no longer spawn on floor 6 to better match StS1
- Improved map generation consistency
- Fixed maps sometimes generating with fewer shops than intended
- Gloom Ascension modifier no longer affects ? rooms in addition to Rest Sites

Since Neow now counts as the first floor of the map, Elites were actually spawning one floor earlier than they should have been. This change should have a big impact on how tough fighting early Elites is. Additionally we found some improvements could be made to the map generation algorithm to help limit the number of maps overflowing with early monster rooms and not much else. For the Gloom change, although it did make the game more difficult it was not in a satisfying or fun way as it just made maps less interesting. Instead I will be looking to rework Gloom entirely in the future to a modifier that makes the game harder but in a more interesting way.

**Ironclad:**

- Buffed **Cinder** card: now Exhausts a random card in your Hand instead of the top card of the Draw Pile
- Changed **Tremble** card: now Exhausts

Tremble gaining Exhaust is a buff to Ironclad's Exhaust synergies, and coupled with the Cinder change should have a large impact on the Ironclad's cardpool.

**Silent**

- Buffed **Serpent Form**: Damage increased from 4(5) -> 4(6)

**Regent:**

- Buffed **Falling Star** card: Damage increased from 7(11) -> 8(12)
- Buffed **Glitterstream** card: Next Turn Block increased from 4(6) -> 5(7)
- Buffed **Parry** card: Block gain increased from 8(11) -> 10(14)
- Buffed **Refine Blade** card: Forge increased from 6(10) -> 9(13)
- Buffed **Celestial Might** card: Upgrade changed from +2 damage -> 1 additional hit
- Buffed **Guiding Star** card: now draws cards immediately instead of next turn
- Buffed **Sword Sage** card: no longer increases the cost of Sovereign Blade
- Changed **Spoils of War** card: now draws 2 cards, but Forge decreased from 12(17) -> 5(8)

**Defect:**

- Nerfed **Voltaic** card: Energy cost increased from 2 -> 3

**Potions & Relics:**

- Buffed **Tiny Mailbox** relic:

- Rarity moved from Common -> Uncommon
- Potions procured increased from 1 -> 2

- Changed **Permafrost** relic: Rarity moved from Common -> Uncommon
- Changed **Bag of Marbles** relic: Rarity moved from Uncommon -> Common
- Changed **Red Mask rarity** relic: Rarity moved from Uncommon -> Common
- Changed **Bellows** relic: Rarity moved from Uncommon -> Rare
- Changed **Lasting Candy** relic: Rarity moved from Rare -> Uncommon
- Changed **Crossbow** Tanx relic: Now uses Free to Play instead of Costs 0 Energy
- Changed **Vexing Puzzlebox** relic: Now uses Free to Play instead of Costs 0 Energy (Now also only lasts until end of turn)
- Changed **Liquid Memories** potion: Now uses Free to Play instead of Costs 0 Energy

For relic rarity, the design goal is that commons are not necessarily weaker, but that they are often simple effects that could add value to most decks. As rarity increases we allow higher complexity and more impact on drafting/play decisions.

**Enemies:**

- Nerfed **Skulking Colony**: deals less damage, damage scales slower, and no longer applies Dazed status cards
- Changed **Haunted Ship**: now applies Dazed status cards to the player
- Nerfed **The Lost and Forgotten**: stat theft no longer scales with Ascension
- Nerfed **Waterfall Giant**: HP decreased from 250(260) -> 240(250)

Skulking Colony went from too weak to too strong. This nerf should put his encounter more in line with how difficult the fight was intended to be. Additionally I found that the Dazed statuses actually worked against the design goals of his power, the new version is a tighter design overall that presents a more unified question for the player to answer. If the encounter is now too weak, simple numbers can be buffed in the future.

**Events:**

- Disallowed Gold generating relics from showing up as the featured relic in the Welcome to Wongo's event
- Mad Science Skill Chaos option (In the Tinker Time event) now uses Free to Play instead of Costs 0 Energy


### ART:


- Added Test Subject burn animation
- Axe and Crossbow Ruby Raiders now have improved textures
- Added Portrait art for the following cards:

- Compact
- Shatter
- Fuel
- Consuming Shadow
- Subroutine
- Trash to Treasure


### USER INTERFACE & EXPERIENCE:

- Date at top-right now reflects build date instead of current date
- Increased feedback form character limit from 500 to 8000

Receiving in-game feedback from YOU the player is the best way we have of improving the game. Many of you have requested the ability to write more without hitting the character cap too early. We've increased the character count 16 fold, so I am eagerly looking forward to reading your effort posts!

### BUG FIXES:

**General:**

- Fixed exploit where you could controller navigate to card grid when the upgrade/remove/enchant/transform preview screen is open
- Fixed a crash that could occur at the start of the player's turn when abandoning a run
- Fixed a softlock that could occur when abandoning a run during a combat-style event
- Fixed cards upgraded to cost 0 no sorting correctly when ordering by cost in the deck view
- Fixed the Y and X glyphs being incorrect on Switch controllers
- Format large numbers on victory screen using the player's language
- Localize feedback screen Category label
- A proper error is displayed if you try to join an in-progress game
- Fixed softlock when a monster's visuals would fail to load
- Numbers now are formatted appropriately to the player's language

**Necrobinder:**

- Fixed Grave Warden+ showing Soul+ in its hovertip preview
- Fixed Seance+ showing Soul+ in its hovertip preview
- Foul potion no longer damages Osty

**Potions & Relics:**

- Fixed a state divergence related to Joss Paper relic
- Fixed a state divergence related to Dualcast card and Snecko Eye relic

**Enemies:**

- Fixed a softlock when playing Scrape card against The Doormaker boss
- Fixed blackscreen when loading a game saved at the rewards from the Mysterious Knight combat

**Ancients:**

- Fixed being able to save and load at Neow to avoid the damage taken from Precarious Shears
- Fixed softlock when you play an Imbued card that prompts a card selection and Vakuu's Whispering Earring relic
- Fix issues with Pael's Eye relic when the player dies at the start of the extra turn

**Events:**

- Fixed Doll Room SFX continuing to play if you save and quit out from the event
- Fixed state divergence when you use Driftwood relic at any event
- Fixed timing-related state divergence at the Doll Room event
- Fixed timing-related state divergence when entering Battleworn Dummy event
- Waterlogged Scriptorium event's Tentacle Quill and Prickly Sponge options now properly show a hovertip for Retain

**Multiplayer:**

- Fixed back button not reappearing if you unready in an Multiplayer load game lobby
- Fixed Beacon of Hope power hovertip description so it no longer says it stacks
- Bottled Potential now shuffles the target player's cards, not yours
- Fixed Radiate card counting Stars from other players
- If both players have Pael's Eye and only one player triggers the extra turn, the second player no longer takes their extra turn immediately afterward
- Fixed rare timing issue when any player selects Random
- Fixed Haunt proccing when another player plays Soul
