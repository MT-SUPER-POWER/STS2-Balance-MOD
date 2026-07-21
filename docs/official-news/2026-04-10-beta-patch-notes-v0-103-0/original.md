---
date: "2026-04-10"
version: "v0.103.0"
title: "Beta Patch Notes - v0.103.0"
source: "https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1829528821308663"
author: "demileaf"
---

Hey, Slayers! Casey here. This week's beta patch brings two major changes to how scoring works for the Daily Leaderboards!

First and foremost, **the leaderboards will now only show your friends' scores**. We'll add a way to view global scores later. In general, we don't really want to incorporate invasive anti-cheat protection into the game so it's sort of impossible for us to prevent people putting in the maximum score.

Secondly, no more magic numbers. While there's still points accrued for unlocking new cards and such, the score that's sent to the leaderboards and how it's sorted is based on **whether you won, how many badges you accrued, and how quickly you finished the run** (in that order). This should clarify why you did better on a Daily run compared to your friends.

Because the schema has changed, **leaderboards between main branch and beta branch will be separate going forward**. We'll eventually add more features like sorting and whatnot, but in the meantime, do let us know what you think of these changes! Apologies in advance for any bugs...

![](https://clan.fastly.steamstatic.com/images/44971832/0a579206f292b9be5d094f00d9bb7628e80302ee.png)
Now let's take a look at the rest of the updates, including updated art (like Nonupeipe above), some reworks and balance changes, a new Ironclad card, and new Neow relics (one of which may be very familiar to seasoned Slayers).

### **CONTENT AND BALANCE:**

Hey everyone, Anthony here. This week I will be grouping balance and content changes together to better talk about how each section changes overall.

**Ancients:**

- Added new Neow relics:

- Neow's Bones: "Upon pickup, gain 2 random Neow Relics. Add 1 random Curse to your Deck."
- Phial Holster: "Upon pickup, gain 1 potion slot and procure 2 random potions."
- Winged Boots: "You may ignore paths when choosing the next rooms to travel to 3 times."

We have three new Neow options you can choose from. I especially like adding more variety to Neow, so we will be looking to expand her set of relics even more over time.

**Ironclad:**

- Added new Ironclad card, **Not Yet**: "Rare - Skill - Cost 2 | Heal 10(13) HP. Exhaust."
- Deprecated **Grapple** card
- Changed **Colossus** card: rarity decreased from Rare -> Uncommon

This set of changes is directly aimed at improving Ironclad's survivability in a big way. **Colossus** moving to the Uncommon pool helps to lessen the need for Rare defensive options, and should also help Ironclad players that are missing out on Weak. To make room for an Uncommon Colossus, an existing Uncommon had to be cut. **Grapple** was too low impact (and redundant with Juggernaut) so it made the most sense to cut. **Not Yet** gives the Ironclad a much needed healing option.

**Silent:**

- Changed **Acrobatics** card: rarity increased from Common -> Uncommon
- Reworked **Follow Through** card: "Uncommon - Attack - Cost 1 | Deal 6(8) damage to ALL enemies. If the last card you played this turn was a Skill, apply 1(2) Weak to ALL enemies." -> "Common - Attack - Cost 1 | Deal 7(9) damage. If you have 5 or more other cards in your Hand, hits an additional time."
- Buffed **Memento Mori** card: base damage increased from 8(10) -> 9(11)
- Buffed **Speedster** card:

- Damage increased from 1(2) -> 2
- Upgrade changed from +1 damage -> Gains Innate

![](https://clan.fastly.steamstatic.com/images/44971832/9c6d818e32e25d708adfbc70d4580700d0ee80f6.png)

**Acrobatics** has consistently been a problem as a Common, completely dominating both win rates and pick rates. When compared to the other cards in its family: Skim, Parse, Glimmer, and Burning Pact, it stood out as the only Common. At Uncommon I expect it to still dominate pick rates, while giving me the breathing room to not overly nerf Sly cards, and in fact maybe even give Silent buffs. I will be watching to see how this change impacts her pool in particular this week. This is *still* an experiment, so it could be reverted if this change ultimately ends up being worse for the game.

**Regent:**

- Changed **Regalite** relic: now grants Block for *any* card created, not just Colorless cards

This change makes **Regalite** consistent with Arsenal, and gives more love to the Regent's Status-generating cards.

**Enemies:**

- Deprecated Tunneleing Twosome fight from the Hive
- Nerfed **Living Fog**: no longer increases the amount of bombs it spawns over time
- Nerfed **Skulking Colony**: HP decreased from 74(79) -> 70(75)
- Nerfed **Byrdonis**: HP decreased from 91(99)-94(99) -> 81(90)-84(90)
- Nerfed **Doormaker**:

- Overall damage numbers nerfed and moves moved around to better align with the doormaker powers
- No longer applies debuffs
- Gains less strength

**Doormaker** maintains his troublesome powers, but gets *several* changes to make the fight more manageable and feel less oppressive. Additionally, I have started focusing on removing some of the spikiness from Act 1, with the long term plan to shift some of the difficulty from Act 1 -> Act 3. The **Skulking Colony** change is hopefully the last for a while, as the minor HP adjustment allows killing it one turn earlier at Ascension 8.

### **ART:**


- Added portrait art for the following cards:

- Adaptive Strike
- Shadow Shield
- Smokestack
- Bad Luck
- Outbreak

- Added new art for Nonupeipe Ancient background
- Added new art for the following Neow Ancient relics:

- Hefty Tablet
- Neow's Talisman

- Updated art for Osty in character select screen

### **USER INTERFACE & EXPERIENCE:**


- Players on the beta branch now submit their scores to a separate beta-only leaderboard
- Added a scrollbar to the Run History screen
- Badges now show up in Run History screen
- Cleaned up the screen when loading a multiplayer Daily Run so that it matches the standard Daily Run screen
- Daily Leaderboards now show off floor, badge count, and time instead of score
- In multiplayer games, gold score is averaged out across all players to make score uploading consistent for the new schema
- Updated Leaderboard view in Game Over screen
- Leaderboards shown in the Game Over screen in Dailies now fit better
- Your name on the Leaderboards are now highlighted blue so you can more easily find yourself
- Reworded **Spiraling Whirlpool** event to explicitly state that it can only enchant a basic Strike or Defend
- Added wrapping navigation to Relic Inventory bar on controller
- Big divination button hotkey in Crystal Sphere event no longer uses deck view button on controller

### **WRITING:**


- **Archaic Tooth** relic now properly says "Transform a starter card *into* an Ancient version" instead of "*with* an Ancient version".
- **Flak Cannon** card description now highlights "Status" in gold
- **Flanking** card now properly specifies that it only doubles attack damage
- **Iteration**, **Rocket Punch**, and **Trash to Treasure** card descriptions now say "Status" instead of "Status card"
- **Pael's Tooth** relic now says "Cards devoured" instead of "Cards in the Pot"
- Updated translations for various languages
- Fixed translation errors for the following languages:

- German
- Japanese
- Russian

- Updated credits

### **BUG FIXES:**

**General:**

- Fixed a blackscreen-at-launch issue related to Steam crashing
- Fixed softlock related to opening a secret debug menu
- Fixed an issue where, if the player dragged a multi-target attack out of their hand, the damage amount previewed on the attack wouldn't include their Strength if some enemies had Vulnerable while others didn't
- "Ascension unlocked" text is no longer displayed in Daily/Custom runs
- Fixed leaderboards not being centered properly on non-standard aspect ratios
- Fixed cards in hand not being deselected when pressing the top panel hotkey button
- Fixed Merchant hand not initially hovering over the first card in the shop when on controller
- Players and enemies' maximum Block has been increased to 999,999,999
- Negative power amounts can no longer stack below -999,999,999

**Silent:**

- Fixed **Blade of Ink** card listing the generated Enchanted Shivs in the Run History combat rewards
- Fixed **Knife Trap** card causing the Exhaust Pile UI to display an incorrect number of cards

**Regent:**

- Fixed "to All enemies" not appearing in the **Sovereign Blade** Forge animation when playing **Seeking Edge** card
- Fixed **Reflect** card enchanted with Imbued wearing off before the player turn actually begins

**Necrobinder:**

- Fixed **Borrowed Time+** card not having green text in the upgrade preview
- Fixed self-Doom killing the player if they kill the enemy via **Stampede** card first

**Enemies:**

- Fixed **Knowledge Demon** background music not playing finale when you defeat him
- Fixed **Queen** background music not ending if you kill her before the Amalgam
- Fixed missing **Crossbow Ruby Raider** fire animation
- Poison now always procs before **The Insatiable** tries to eat you

**Events:**

- Fixed being able to drink the fruit potion in the **Stone of All Time** event if you open the popup as the screen is fading out

**Multiplayer:**

- Fixed being able to play potions in multiplayer when you are dead
- Fixed **Belt Buckle** relic removing Dexterity if another player gets a potion from **Alchemy** card
- Fixed **History Course** relic playing **Believe in You** card before the start-of-turn Energy reset
- Fixed **Smokestack** power proccing off of other player's Statuses
- Fixed strange behavior after exiting the timeline via a Steam Invite
- Fixed some state divergences when using console commands in combat
- Fixed some instances of "unexpected noconnection packet" error
- Fixed state divergence related to upgrading a **Monologue** card enchanted with Royally Approved
- Fixed timing-based softlock related to **Golden Compass** relic
