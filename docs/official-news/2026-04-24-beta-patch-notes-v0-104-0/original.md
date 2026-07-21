---
date: "2026-04-24"
version: "v0.104.0"
title: "Beta Patch Notes - v0.104.0"
source: "https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1830797770234584"
author: "demileaf"
---

Hey Slayers, we're back to updating the beta branch this week! Here's an overview of our goals with this patch.

**On Ascensions:**

When it comes to general balance, the base difficulty is quite hard right now. We're continuously adjusting this baseline difficulty to make it more accessible for a wide range of players, while also continuing to deliver a challenging game at the highest Ascensions.

Similarly to the first game, completing runs of Slay the Spire 2 at the highest difficulty should feel like a monumental achievement for a fraction of players devoted to understanding the ins and outs of the game's mechanics. Upon beating your first run, we added a message to communicate this intention for the Ascension system.

**On Doormaker:**

We are currently monitoring the Doormaker and looking at a combination in-game feedback, social media posts, and metrics. We want to give players time to adjust a bit, otherwise we'll be balancing around kneejerk reactions. Currently, from looking at millions of runs, Doormaker's overall difficulty/winrate is in a good place (slightly weaker than the other Act 3 bosses both in kill rate and damage dealt). However, we do want to ensure that its mechanics aren't too abrasive against certain playstyles.

**Miscellaneous:**

In other news, we've also got a few card reworks, new Badges, and a balance pass (notably, a Neow's Fury buff is here). One oft-requested feature we've added is an error message when in-game feedback is not received (for instance, if a player is offline), so that should make it clearer when your feedback was actually able to be properly submitted. We've also got a ton of fixes for various crashes and bugs as well as cool new art for some cards (and a certain Ancient)!

![](https://clan.fastly.steamstatic.com/images/44971832/52d0afb58122687b0a9242f5b216994973b336af.png)

### CONTENT:

**General:**

- Added three more Badges

- Damage Leader - Dealt the most damage to enemies in multiplayer.
- Debuffer - Applied the most Debuffs in multiplayer.
- C-c-c-Combo - Played 20 cards in a single turn.

**Ironclad:**

- Reworked **Conflagration** card:

- Old: Conflagration - Attack - Cost 1 - Rare - Deal 8(9) damage to ALL enemies. Deals 2(3) additional damage for each other Attack you've played this turn.
- New: Conflagration - Attack - Cost 1 - Rare - Deal 2 damage to ALL enemies 4(5) times.

- Reworked **Drum of Battle** card:

- Old: Drum of Battle - Power - Cost 0 - Uncommon - Draw 2(3) cards. At the start of your turn, Exhaust the top card of your Draw Pile.
- New: Drum of Battle - Skill - Cost 1 - Uncommon - Draw 2 cards. When this card is Exhausted, gain 2(3) Energy.

Anthony's note: **Drum of Battle** was previously the lowest picked Uncommon Ironclad card, and generally unpopular. This rework attempts to put it into a more interesting spot and adds some synergy to Exhaust decks. **Conflagration** had low pick rates and middling win rates for a Rare, while also feeling more like an effect that belonged on the Silent (caring about number of card plays). It has been reworked into a simple AOE card that can act as a Strength payout. The numbers will likely need tuning to get into a desired spot, but this should feel like a more appropriate part of the Ironclad's kit.

**Regent:**

- Reworked **Parry** card:

- Old: Parry - Uncommon - Power - Cost 1 - Whenever you play Sovereign Blade, gain 10(14) Block.
- New: Parry - Uncommon - Power - Cost 1 - Sovereign Blade now gains 10(14) Block.
- This means Parry's Block is displayed on the Sovereign Blade card and is now also affected by powers like Dexterity and Frail.

Anthony's note: Parry's numbers are looking correct now, but we think it will be more fun showing the effect on the Blade itself. Relics and powers now being able to interact with the Block should open up new fun interactions. As a result of this change, its numbers may need further tweaking in the future, but we will start with it as is.

**Enemies:**

- Reworked Axebots combat to have just a single, more challenging Axebot

### BALANCE:

**General:**

- Terminal run modifier can no longer bring a player below 1 HP

**Silent:**

- Changed **Nightmare** card: if you play it and select a card with an Affliction on it, that Affliction is now removed from the copies added to your hand the following turn
- Buffed **Untouchable** card: upgraded Block gain increased From +2 -> +3

**Regent:**

- Changed **Tyranny** card: now triggers before **Bombardment** card

- This means that if, for example, Tyranny exhausts Bombardment, Bombardment will be played immediately after

- Nerfed **Reflect** card: Block decreased from 17(21) -> 16(20)

**Ancients:**

- Buffed **Neow's Booming Conch** relic: in addition to drawing extra cards, now gains 1 Energy at the start of Elite combats
- Buffed **Neow's Fury** card: instead of selecting randomly now allows you to choose which 2 cards will be put into your Hand from your Discard Pile (you may also now choose fewer than 2 cards if you want)
- Buffed **Tezcatara's Nutritious Soup** relic: in addition to making your Strikes cost 0 and giving them Eternal, now makes all your Strikes deal 3 additional damage

**Enemies:**

- Nerfed **Ruby Raider Assassin**: damage decreased from 11(12) -> 10(11)
- Changed **Scrolls of Biting**: have slightly less HP at low Ascension but slightly higher HP at high Ascension, changed from 31(32) - 38(39) -> 30(33)-37(39)
- Changed **Owl Magistrate**: now has slightly less HP at low Ascension, but slightly higher HP at high Ascension, changed from 234(243) -> 231(247)
- Changed **Slimed Berserker**: now has slightly less HP at low Ascension, but slightly higher HP at high Ascension, changed from 266(276) -> 261(281)
- Nerfed **Vantom** at lower Ascensions:

- Dismember move damage decreased from 27(30) -> 26(30)
- Slippery power goes from 9 -> 8(9)

- Nerfed **Waterfall Giant**: now heals less at lower Ascension, HP healed changed from 15 -> 10(15)

Anthony's note: We are slowly starting the process of making lower Ascensions a little easier, and high Ascensions slightly harder. We want to do this gradually instead of all at once.

**Events:**

- Changed **Slippery Bridge** event: will no longer offer to remove the same card multiple times unless it has already offered to remove all other cards in your deck at least once

**Multiplayer:**

- Nerfed multiplayer scaling for enemy Artifact, Slippery, and Skittish powers
- **Stratagem** card is no longer banned from the colorless card pool during multiplayer runs (this banning was originally due to a bug with Stratagem in multiplayer, which has now been fixed)
- Buffed **Tag Team** card: Replay effect now triggers on "deal damage to ALL enemies" attacks too

### ART:

- Added portrait art for the following cards:

- **Feral**
- **Not Yet**
- **Spite**
- **Fight Through**
- **Hailstorm**
- **Synchronize**
- **Iteration**
- **Voltaic**
- **Apparition**
- **Abrasive**

- Revised **Neow's Fury** artwork (is now centered)
- Added **Vakuu** background art and VFX
- Added **Skulking Colony** animations
- Updated art for the following enemies:

- **Tunneler**
- **Entomancer**
- **Mawler**

- Updated animation for the following enemies:

- **Vantom**
- **Ceremonial Beast**
- **Entomancer**

- Repositioned **Sludge Spinner** death landing spot

### USER INTERFACE & EXPERIENCE:

- Added Ascension first-time tutorial popup for single player and multiplayer
- An error is now displayed when Steam crashes
- Feedback tool now displays an error if we didn't receive the feedback
- Adjusted hitbox, speech, and intent position of Underdocks Cultists
- Fixed intents being positioned too high or too low in multi-enemy combats
- Bestiary button now looks more locked (similar to how other buttons look locked)
- The player's hand now animates out of the way while it is the Enemy turn
- Updated Ancient card text backgrounds to fit more text and to be very slightly darker and embossed
- Set up controller navigation for Badges in the game over screen
- Added basic controller navigation to Badges in the run history screen
- Added controller navigation for the player info icons in the run history
- Improved controller navigation in the Fake Merchant shop
- Pause menu button controller navigation now wraps

### WRITING:

- **Brilliant Scarf** relic description now specifies that it only counts cards played "from your hand" so it's clear that it doesn't count auto-played cards
- **Reaper Form** card wording has been changed to make it clear that the Doom is applied by Reaper Form's power rather than by the played card:

- Old: "Whenever Attacks deal damage, they also apply that much Doom."
- New: "Whenever Attacks deal damage, apply that much Doom."

- Added translations for various languages
- Fixed various translation errors

### BUG FIXES:

**General:**

- Score separator lines are no longer shown while fetching score or if it fails
- Effects that make the next card free to play (Unrelenting card, Void Form card, etc.) now properly take precedence over effects that increase the cost of all cards (Entangled affliction, Spiked Gauntlets relic, Borrowed Time card, etc.)
- Fixed combat softlock that could occur after returning to main menu and resuming a run
- Fixed crash when applying powers to dying creatures
- Fixed crash when closing the game during startup before saves are initialized
- Fixed crash when upgrading a card at rest site
- Fixed error caused by offscreen Epoch indicator on Timeline screen
- Fixed error when un-focusing Potion slot during Potion removal
- Fixed Linux crash during combat when UI elements are removed mid-frame
- Fixed crash when entering rooms
- Fixed an issue with the Game Over screen after discovering multiple enemies while playing in Polish
- Fixed being able to target dead enemies with controller
- Fixed crash when opening card pile screen while targeting an enemy with controller
- Fixed rare combat errors that could occur when re-entering combat
- Fixed rare softlock related to writing debug files
- Fixed softlock on run histories from older versions of the game
- Made save operations fault-tolerant to prevent softlocks
- Score is correct when at ascension 10
- Fixed wrong floor count when submitting Daily score

**Ironclad:**

- **Howl From Beyond** card now plays from the Exhaust pile after you've drawn your hand instead of before
- Fixed **Anger** card played by **Havoc** also Exhausting on play
- Fixed being able to select an upgraded card with **Armaments** card if it is added to your hand by **Largesse+** card
- **Not Yet** card can no longer be generated via a **Skill Potion**

**Silent:**

- Fixed being able to skip the **Survivor** card discard selection via the peek button

**Regent:**

- **Bombardment** card now plays from the Exhaust pile after you've drawn your hand instead of before
- **I Am Invincible** card now properly triggers before you've discarded your hand instead of after
- **Regalite** relic no longer gains Block from enemies applying statuses
- **Summon Forth** card now properly puts existing **Sovereign Blade**s into your Hand before Forging instead of after

**Necrobinder:**

- Fixed crash when Osty dies during scene transition
- Fixed **Reave** card always bottom-decking the **Soul** card it creates

**Defect:**

- Fixed Beckon still damaging the player if the combat is ended by Lightning orbs

**Potions & Relics:**

- Fixed **The Boot** relic amplifying **Bad Luck** curse's damage from 1 to 5 when the player has Intangible
- **Self-Forming Clay** relic is no longer triggered by Terminal modifier's max HP loss

**Ancients:**

- Players can no longer roll **Tezcatara's Nutritious Soup** blessing if there are no Strikes in their deck
- Fixed **Vakuu's Whispering Earring** not correctly playing **Stardust** card
- **Leafy Poutice** relic no longer transforms the upgraded Strikes/Defends from **Neow's Talisman** relic if you get both from **Neow's Bones** relic
- **Unceasing Top** relic now triggers properly when **Vakuu's Whispering Earring** relic plays the last card in your hand

**Enemies:**

- Fixed being unable to discard potions in the **Punch Off** event
- Fixed **Parafright** illusion keeping its negative Strength from **Dark Shackles** when you kill it for the turn

**Events:**

- Fixed being unable to give the **Foul Potion** to the **Fake Merchant**
- Fixed cards from other classes appearing in the **Infested Automaton** event
- Fixed softlock if you somehow die in the **Battleworn Dummy** event fight
- All **Spoils Map** cards are now removed if you complete the quest and you somehow have multiple
- You now get 600 gold for each copy of the **Spoils Map** quest card you have

**Multiplayer:**

- Fixed a rare issue in multiplayer when playing **Armaments** card during the Knight Gang encounter
- Fixed **Pael's Legion** and **Vambrace** relics not proccing from the owner applying Block to other players
- **Dense Vegetation** can no longer appear if any player has less than 8 HP
- Fixed softlock related to using **Driftwood** relic on your first run
- Fixed rare softlock loading into a treasure room when a teammate is using a controller
- Fixed state divergence related to using a potion just before combat began
- Improved error reporting for a specific rare connection error
- You can no longer buy items at the shop while transitioning rooms
- **Pillar of Creation**, **Supermassive**, and **Arsenal** cards now proc for the player that played **Largesse** card rather than the player who gets the generated card
- The correct error message is now shown for Steam multiplayer connection timeout (error 5008)
- "Show Multiplayer Drawings" tickbox is now correctly localized
- You can no longer start a loaded multiplayer game with only one person
