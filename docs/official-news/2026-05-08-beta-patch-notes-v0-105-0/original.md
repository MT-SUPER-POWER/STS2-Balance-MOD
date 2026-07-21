---
date: "2026-05-08"
version: "v0.105.0"
title: "Beta Patch Notes - v0.105.0"
source: "https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1832065502813730"
author: "demileaf"
---

It's beta patch time!

For those new to *Slay the Spire 2*, keep in mind that these changes are pushed to our Beta branch to ensure stability, then eventually merged into the main branch. For those who play using controller, we highly recommend using the [Beta branch]("https://store.steampowered.com/news/app/2868840/view/502852451136700758") as we're trying to resolve crashes related to selection screens when playing via controller/Steam Deck.

While our focus is still on fixing crashes and ensuring the game runs on as many hardware configurations as possible, let's look at some of this patch's highlights.

## Bi-Weekly Patching

As you've probably noticed, we're moving to patch every 2 weeks rather than weekly as outlined in our Early Access Disclaimer. This may be a bit surprising as the first game patched weekly, but it was a lot of work so it really sucked.

Anyways, the 2-week cadence should make it less hectic for us, allowing bigger changes and more time working and polishing stuff before it goes out. It also allows more time for beta players to absorb and give feedback on changes.

## The Bestiary

This week brings... the Bestiary! While it's still an outline of what it will end up being, we hope that seeing these monsters and their animations and data (not yet implemented) all listed in a separate screen will be fun.

![](https://clan.fastly.steamstatic.com/images/44971832/bcf1a1b14356dcaed73042d288bd62bc84bcde5a.jpg)
*Pro Tip: Press the 1 - 8 keys when you're viewing a monster.*

## Doormaker Out, Aeonglass In

Doormaker has been replaced with a brand new boss, Aeonglass! While Doormaker had interesting micro decisions in the fight, he was over the complexity threshold of what we want and had lingering issues. We decided that starting over fresh will let us hit what we actually want for an Act 3 boss.

## CONTENT & BALANCE:

**General:**

-  The Bestiary is now available in the Compendium

**Silent:**

-  Nerfed **Blade of Ink** card: Inky enchantment damage decreased from +2 -> +1

**Regent:**

- Slightly reworked **Sword Sage** card:

- Old: Sword Sage - Power - Cost 2(1) - Rare - "Sovereign Blade now hits an additional time."
- New: Sword Sage - Power - Cost 2(1) - Rare - "Sovereign Blade gains Replay 1."
- Among other things, this means that Sword Sage card now has synergy with Parry card

- Nerfed **Bulwark** card: Block decreased from 13(16) -> 12(15)
- Buffed **Crescent Spear** card: base damage increased from 6 -> 8
- Nerfed **Patter** card: Block gain decreased from 9(11) -> 8(10)
- Buffed **Royalties** card: Gold Gain from 30(35) -> 30(40)

**Defect:**

-  Buffed **Infused Core** relic: now also gains "Lighting Orbs deal 1 additional damage."
-  Buffed **Hyperbeam**: damage increased from 26(34) -> 28(36)
-  Buffed **Shatter** card: now Evokes all of your Orbs twice
-  Buffed **Tesla Coil+** card: damage decreased from 6 -> 4, but Lightning Triggers increased from once -> twice
-  Buffed **Uproar** card: damage increased from 5(7) -> 6(8)

**Colorless Cards:**

-  Buffed **Gold Axe** card: in multiplayer, now deals damage equal to the number of cards played by ALL players, not just cards played by its owner

**Enemies:**

-  Reworked **Doormaker** to a new boss, **Aeonglass**
-  Changed **Fossil Stalker**: in multiplayer, now gains a set amount of Strength if it hits any player, instead of gaining Strength for each player hit
-  Changed **Haunted Ship**: now applies 3 Weak on Turn 1 instead of applying Weak when it attacks
-  Buffed **Soul Fysh**: Scream move damage increased from 11(12) -> 13(15)

**Ancients:**

-  Reworked **Tezcatara's Pumpkin Candle** relic

- Old: "Gain 1 Energy at the start of each turn. Extinguishes at the start of Act 3."
- New: "Gain 1 Energy at the start of each turn. Extinguishes after 5 combats. Can be Kindled at rest sites."

- Added new Neow relics:

- **Kaleidoscope** (temp name) - "Upon pickup, gain 2 card rewards with cards from other characters."
- **Fishing Rod** - "Every 3 normal combats, Upgrade a random card in your Deck."
- **Silken Tress** - "Enchant all cards in the first card reward with Glam."

- Moved **Tezcatara's Seal of Gold** relic to option 3 so that it is in the same pool as **Pumpkin Candle**

**Events:**

-  Something *special* now happens if you have multiple **Lantern Key** cards (doesn't apply to multiplayer)

## ART:

-  Added art for Friendship Epoch
-  Added new power art for Smoggy
-  Added multi-hit animation for **Corpse Slug**
-  Added unstun animation for **Rock Bowlbug**
-  Tweaked Vantom explosion to look right on shrunken Vantom
-  Added a phobia mode alternate portrait for the **Zen Weaver** event

## USER INTERFACE & EXPERIENCE:

-  Limited the total number of cards rendered when playing the Clone animation at a rest site to 15
-  Added translation issue dropdown option to the in-game feedback form
-  Outline darkness of continue button now matches the next dialogue arrow
-  Adjusted **Gremlin Mercenary** combat enemy hitboxes, speech bubble, and intent positions
-  **Skulking Colony**'s health bar now changes color to indicate that it cannot take any more damage this turn
-  When hovering over stale dialogues at Ancients, all of the dialogues will become opaque instead of just the one currently being hovered over
-  Added a more informative error popup for Steam BadCert error in multiplayer
-  If you move a current_run.save with a modded character in it back to unmodded, you get an error instead of a black screen

## WRITING:

-  Reworded Axebot death message to reflect that there is only 1 of them now
-  Added more translated text for various languages
-  Fixed translation errors in:

-  German
-  Latin American Spanish
-  Castilian Spanish
-  French
-  Italian
-  Japanese
-  Polish
-  Brazilian Portuguese
-  Thai
-  Turkish

## AUDIO:

-  Added **Soul Fysh** background music
-  Added **Kaiser Crab** background music
-  Added **Kaiser Crab** SFX
-  Added hyper beam SFX

## BUG FIXES:

**General:**

-  Fixed save files being lost or corrupted after a PC crash, blue screen, or power outage
-  Fixed black screen softlock when Steam would disconnect during room transitions
-  Fixed the Sealed Deck modifier not being affected by the multi-character card modifiers
-  Night Terrors modifier can no longer kill you
-  The Perfect badge is no longer awarded if you abandon run at a boss
-  Fixed being unable to navigate off of top bar in shop room
-  Fixed being unable to controller navigate through rest site options when player has 2 rows of relics
-  Fixed controller navigation in shop if you buy out all relics and colorless cards
-  The game no longer rapidly switches between "Controller Detected" and "Mouse Detected" when playing with a controller
-  Fixed being able to start a single player run via controller while the Ascension tutorial was open
-  Fixed potion hovertip not appearing if you press "b" in the potion popup screen
-  Fixed errors in input settings for new keybindings added after saving
-  Fixed rare problem where feedback screen is stuck open on game launch
-  Fixed rare softlock when playing Compact with status cards in hand
-  Fixed softlock after the popup about shuffling in the tutorial
-  Fixed Steam cloud sync errors when syncing many run history files
-  Fixed duplicate files appearing in run history after Steam cloud sync
-  Restored the default rendering backend to D3D12 for Intel 620 GPUs
-  Fixed audio staying muted after pressing volume keys or alt-tabbing on Linux
-  Fixed crash on Linux when ICU libraries are unavailable

**Ironclad:**

-  To avoid softlocking after setting up an infinite loop (i.e. Hellraiser + 2 Pommel Strike cards), **Hellraiser** will only play 9 cards per turn if all enemies have infinite HP

-  **Hellraiser** card now properly plays cards containing "Strike" against enemies with infinite HP

**Silent:**

- Fixed **Fan of Knives** card VFX not playing

**Regent:**

-  Fixed not being able to select cards with controller when you choose all the cards in your hand via **GUARDS!!!** card

**Necrobinder:**

-  Fixed **Fetch** card not glowing gold if it is drawn by **Gambling Chip** relic
-  Fixed **Enfeebling Touch** power causing **Ceremonial Beast** to gain Strength after you stun it

**Colorless Cards:**

-  Fixed VFX sometimes failing to appear during **Rolling Boulder** power effect

**Potions & Relics:**

-  Players are now disallowed from throwing the **Foul Potion** if the Merchant inventory is open
-  Using **Foul Potion** on the Merchant no longer freezes the game on controller
-  **Bookmark** relic is now able to properly apply its cost-reduction effect to a card that has been retained by a "Retain your Hand" effect like Equilibrium card
-  Fixed **Beetle Juice** potion causing **Vantom**'s tail to become visibly cut off
-  If the player has **The Boot** relic and deals **Skulking Colony** 1-4 damage, they now properly take 5 damage

**Ancients:**

-  Fixed issue where the top of the next dialogue bubble was slightly visible before it would animate in at Ancients
-  Fixed being able to navigate back to **Neow**'s blessing options while looking at the Card Library
-  Fixed card inspection button blocking "accept" hotkey when looking at the **Pandora's Box** relic card view screen
-  Fixed issue where playing **Wish** card via controller would immediately select a card in the grid on the same button press


**Enemies:**

-  Fixed **Kaiser Crab** appearing if you load into an already completed boss room
-  Recentered **Axebot** in fight so it looks like there is supposed to be only one of them
-  Fixed a softlock that occurred if the player set up an infinite against **Skulking Colony**


**Multiplayer:**

-  Fixed player names with brackets (e.g. clan tags like ) causing errors in multiplayer
-  Fixed state divergence related to **Mad Science**, **Juggling**, and mixed combat speeds
-  Fixed **Strangle** power proccing off of other player's card plays
-  Fixed occasional black screen when loading into a saved multiplayer combat
-  Fixed occasional state divergence caused by **Stampede** + **Headbutt** cards
-  Other players' HP bars no longer go offscreen when resizing the window while in settings
-  Disallow **War Historian Repy** duplicate **Lantern Key** card bonus if you are in multiplayer
