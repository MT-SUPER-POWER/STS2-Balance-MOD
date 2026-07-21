---
date: "2026-06-04"
version: "v0.107.0"
title: "Beta Patch Notes - v0.107.0"
source: "https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1834602721189013"
author: "demileaf"
---

Another bi-weekly beta patch is here! This one is a bit smaller as it is intended to lay the groundwork for moving the beta branch to the main branch. Therefore, these updates are mostly focused on stability: a lot of bug fixes, UI/UX/QOL additions, and performance improvements along with just a few balance changes and art additions....

![](https://clan.fastly.steamstatic.com/images/44971832/96250518646cf2f0fecc834de558fd06e1f23d56.gif)

...Like this new Defect animation!

Without further ado, let's dive in...

### CONTENT & BALANCE:

**Enemies:**

- Buffed **Skulking Colony**: now gets +1 Strength on its Strength gain turn on high Ascension
- Changed **Aeonglass**:

- No longer applies the Ebb debuff
- Now gains Block on its Ebb move (single hit) instead of its Increasing Intensity move

**Potions & Relics:**

- Buffed **The Boot** relic: damage increase now also applies to Osty's attacks

**Ironclad:**

- Buffed **Juggernaut** card: damage increased from 5(7) -> 6(8)

**Regent:**

- Buffed **Monarch's Gaze** card: Energy cost decreased from 3(2) -> 2(1)
- Nerfed **Reflect** card: Block gain decreased from 16(20) -> 15(20)

**Necrobinder:**

- Buffed **Sic 'Em** card: Summon increased from 2(3) -> 3(4)
- Buffed **The Scythe** card: damage increased from 3(4) -> 4(5)


### QUALITY OF LIFE:

- During the Gremlin Merc fight, if Gremlin Merc didn't steal any gold but Fat Gremlin escaped, the player now receives 50% of the normal gold reward instead of 0%


### USER INTERFACE & EXPERIENCE:

**General:**

- If errors occur on run start, a popup is now displayed instead of a black screen
- Implemented preferred Ascension for random character option


**Ancients:**

- When **Vakuu** tells you how many times you've visited him, the number he says is now accurate
- **Pael's Flesh** relic now visually increases the maximum energy amount shown in your Energy counter

- For example, if you have 3 max energy before Pael's Flesh triggers, your energy counter will show 4/4 instead of 4/3 after Pael's Flesh triggers

**Enemies:**

- The **Wither** status card preview upgrade level now matches the upgrade level that **Aeonglass** is actually on

**Necrobinder:**

- **Sacrifice** card now previews how much Block it will gain, so that it's easier to plan with effects like Dexterity and Frail

### ART:

- Added Epoch art for:

- Devastation
- The 12
- The Giant

- Added **Defect** power up animation
- Added new **Soul Nexus** VFX

### WRITING:

- Updated translations for various languages

### BUG FIXES:

**General:**

- If you have Steam Cloud turned off, cloud files no longer overwrite local ones
- First floor of Act 2 can no longer be a ? room instead of a fight when carrying **Spoils Map** quest card
- Fixed card Energy cost sometimes showing the wrong value when its cost is randomized
- Fixed cards returning to the wrong spot in your hand after deselecting them during a card selection
- Card draw is now stopped after combat ends during the draw phase
- Overgrowth act now plays both of its music tracks instead of always the same one
- Music no longer restarts twice at the start of a run

**Ancient:**

- Fixed an issue where, if the player has **Vakuu's Whispering Earring** relic and it auto-plays **Void Form** card, it could sometimes continue auto-playing cards on the player's next turn
- Fixed **Nonupeipe's Diamond Diadem** relic resetting its count after **Hellraiser** power plays cards at the start of the turn

**Enemies:**

- **Aeonglass's Wither** status card no longer briefly returns to your hand before you discard your hand at the end of your turn
- Can no longer upgrade **Wither** status card with **Armaments** or **Drain Life** cards
- Fixed an issue where, after killing Spectral Knight, cards that would normally be Ethereal could sometimes become non-Ethereal
- Fixed creature and boss VFX occasionally failing to appear
- Fixed **Flyconid** spore effect lingering when its attack is interrupted
- Fixed **Fossil Stalker** VFX continuously playing in the Bestiary
- Fixed the **Living Fog** encounter name in the death quote
- If you kill **Gremlin Merc** before he steals any gold from you, the rewards screen no longer says "Mugged..." even if Fat Gremlin escapes

**Potions & Relics:**

- Fixed an issue where, if the player hatched **Byrdonis Egg** at a rest site while the rest of their deck was upgraded and they had **Miniature Tent** relic, they were unable to upgrade the newly-added **Byrd Swoop** card until the next rest site
- Fixed an issue where, if **Pantograph** relic healed the player above 50% health, **Red Skull** relic's Strength increase would still apply until an attack was played
- **Snecko Skull** relic now always triggers before **Unsettling Lamp** relic
- Fixed visual errors with the outline of some potion assets

**Events:**

- **Gnarled Hammer** relic can no longer be traded at the **Relic Trader** event
- Fixed **Perfect Fit** enchantment not working correctly if you play **Reboot** card while the enchanted card is in your hand or draw pile
- Fixed **War Historian Repy** event not removing all duplicates of **Lantern Key** card if you have more than 2

**Silent:**

- Fixed an issue where, if the player had **Unceasing Top** relic, played **Hidden Daggers** card, and discarded a Sly card, Unceasing Top could cause the player to incorrectly draw a card before the Shivs were added to their hand

**Necrobinder:**

- Fixed an issue where, if the player played **Lethality** card and the first attack of the turn had Replay, the replayed attack would incorrectly display Lethality's buffed damage amount before correctly dealing its base damage amount

**Multiplayer:**

- Fixed crash when using potions in multiplayer due to state divergence
- Fixed rare issue which caused incorrect cards to be shown when obtaining cards at certain events
- **Nightmare** card VFX no longer plays for all players

### PERFORMANCE:

- Reduced stuttering when combat effects appear for the first time in a run

### MODDING:

- Base game no longer deletes progress from mods that are removed or errored
