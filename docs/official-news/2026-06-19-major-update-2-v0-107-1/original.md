---
date: "2026-06-19"
version: "v0.107.1"
title: "Major Update #2 - v0.107.1"
source: "https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1835871199305790"
author: "demileaf"
---

Hey Slayers, our second major main branch update is here with 4 patches in 1 (not including hotfixes)! While we'll be covering a lot of things that beta players will already be used to, there are also a few big changes that will be totally new for everyone. One of those has been highly requested by the community in the past few days and another has been *very* highly requested by the community in the past few months, so let's get into it!

### RNG Rework

Ed here, one of the Mega Crit engineers. It turns out that everyone complaining about receiving Debt too often from Neow's Bones was right! We did, in fact, have some unusual RNG problems.

Huge shoutout to **tckmn** for doing the research - their [eight-hour descent into madness](https://www.twitch.tv/videos/2794202198) was an entertaining watch, and we wouldn't have caught the issue without it. If you're interested in the nitty-gritty, read [tckmn's full writeup](https://tck.mn/blog/correlated-randomness-sts2/). Here is a short version.

Most games, including STS2, use [pseudo-random number generators](https://en.wikipedia.org/wiki/Pseudorandom_number_generator) (PRNGs). A PRNG is fed a seed value, and given the same seed value, a PRNG will always generate the same sequence of random numbers.

Each run has one primary “run seed”, a sequence of twelve numbers and letters. However, a run actually contains multiple PRNGs that individually influence your deck draw, your combat rewards, your event choices, and the rest of the random elements in the run. Each one uses a seed derived from the run seed.

![](https://clan.fastly.steamstatic.com/images/44971832/5915358de97a7ce6a66ee0108542c5e518375853.png)

Since we fed different seeds to each PRNG, we expected their results to be completely unrelated. But we were wrong, and it turns out that our strategy allowed players to predict outcomes given knowledge of unrelated parts of the game.

Let’s visualize one example: the correlation between your first act and the curse you receive from Neow’s Bones. In both cases, we generate a decimal between 0 and 1, but from different PRNGs.

 - For your first act, if the decimal generated is less than 0.5, you get Overgrowth; otherwise, you get Underdocks.
 - For Neow's Bones’ curse, we use the decimal to pick from a list of ten curses. Between 0 and 0.1 you get Clumsy, between 0.1 and 0.2 you get Debt, and so on.

Here is a scatterplot of the raw numbers generated given 10000 random run seeds on v0.103.3:

![](https://clan.fastly.steamstatic.com/images/44971832/4b7659819d99f46643eb8d6dae63c1a764558d4d.png)

There is clear correlation in this result. For example, if our act roll is >0.5 (we’re in the Underdocks), then it’s highly likely that our Neow’s Bones curse roll will be <0.5, which cuts the potential curses you can obtain in half.

Aside from the balance ramifications: once knowledge of this exploit is out there, players are naturally incentivized to use it to play optimally. We don’t want to encourage rote memorization of correlation tables, as it is tedious and unfun.

Thus, in v0.107.1, we replaced our PRNG implementation with a more modern one ([xoshiro256**](https://prng.di.unimi.it/)). The resulting scatterplot is evenly distributed, exhibiting no human-detectable correlation.

![](https://clan.fastly.steamstatic.com/images/44971832/91c55e07f895f43e734b3328450336fbda3bf21f.png)

Rest assured that your suffering is now truly random!

Speaking of updates that will be new to all players, I’ve also been busy working on support for…

### Steam Workshop

We built STS2 with modding in mind, and the game has had an integrated mod loader from day one. It's been awesome seeing the outpouring of creativity from the community! But we purposefully haven't been advertising mod support, as we knew it still needed some work.

Previously, the only supported way to install mods was to download them and place them next to the executable. With this patch, **Steam Workshop support has officially been added!** Now, you can browse and install mods directly from the workshop, and they will be automatically downloaded to all devices where you play on Steam.

![](https://clan.fastly.steamstatic.com/images/44971832/41af9b81e0817b2a4bf6dbfc529690f24f993c0a.png)
If you are a mod creator, upload your mods using the tool (https://github.com/megacrit/sts2-mod-uploader).

And if you're interested in playing some mods, check out the [new workshop](https://steamcommunity.com/app/2868840/workshop/).

That’s all the updates from Ed! Passing it off to Casey to introduce…

### The Bestiary

![](https://clan.fastly.steamstatic.com/images/44971832/4512122e65f560656785dc6ffd2d42754c580e7c.gif)

Casey here! This patch brings in the **Bestiary**, which Beta testers will already be familiar with. This was a much requested feature for the first game, but at the time I didn't want to create it as there wasn't a good delivery mechanism for an overarching story. However, in the world of *Slay the Spire 2*, we have a lot more on the narrative side, more creature animations, etc. so we've been hard at work creating this final screen in the compendium (outside of the Achievements page, which is to come much later).

![](https://clan.fastly.steamstatic.com/images/44971832/1d1bb2a98c178e06bb8e09643d33e685f3c7fc8b.jpg)
Currently, players can find a list of monsters they've encountered here as well as their moves and animations. In the future we plan to include all sorts of information about the game's enemies, including stats and lore.

### Enemy Changes

![](https://clan.fastly.steamstatic.com/images/44971832/d1ba210793bd301704ae22a21f77f2611fbc2548.png)

As many of you have heard by now, **The Doormaker has been removed from the game.** As we said in the patch notes of the beta patch that brought this change: while Doormaker had interesting micro decisions in the fight, he was over the complexity threshold of what we want and had lingering issues. We decided that starting over fresh will let us hit what we actually want for an Act 3 boss.

Thus a new boss, **Aeonglass**, is here! We don't want to spoil it too much for non-beta testers, so that's all we're gonna say.

Another big change you'll notice is Hive's **Infested Prism** enemy has been totally reworked--but again, no spoilers here.

Alright, now it's time to get into the nitty gritty notes!

### CONTENT & BALANCE:

**General:**

- The Bestiary is now available in the Compendium
- Added three more Badges:

- Damage Leader - Dealt the most damage to enemies in multiplayer
- Debuffer - Applied the most Debuffs in multiplayer
- C-c-c-Combo - Played 20 cards in a single turn

- Terminal run modifier can no longer bring a player below 1 HP

**Ancients:**

- Added new **Neow** relics:

- **Kaleidoscope** - "Upon pickup, gain 2 card rewards with cards from other characters."
- **Fishing Rod** - "Every 3 normal combats, Upgrade a random card in your Deck."
- **Silken Tress** - "Enchant all cards in the first card reward with Glam. Upon pickup, lose all gold."

- Reworked **Tezcatara's Pumpkin Candle** relic: now extinguishes after 5 combats and can be Kindled at rest sites (was: extinguishes at the start of Act 3)
- Buffed **Neow's Scroll Boxes** relic: no longer makes player lose all gold
- Buffed **Neow's Booming Conch** relic: in addition to drawing extra cards, now gains 1 Energy at the start of Elite combats
- Buffed **Neow's Fury** card: now lets you choose which 2 cards (or fewer) to return from your Discard Pile instead of selecting randomly
- Buffed **Tezcatara's Nutritious Soup** relic: in addition to making your Strikes cost 0 and giving them Eternal, now makes all your Strikes deal 3 additional damage
- Moved **Tezcatara's Seal of Gold** relic to option 3 so it shares a pool with Pumpkin Candle
- Changed **Pael's Eye** relic: if an enemy has Vulnerable and you take an extra turn, the enemy's Vulnerable no longer decrements before your extra turn (also applies to Frail, Intangible, Weak, and other powers that decrement at the end of the enemy's turn)

**Enemies:**

- Replaced the Act 3 boss **Doormaker** with a brand-new boss, **Aeonglass** (its moveset and Wither / Withering Presence mechanics were refined throughout the beta)
- Reworked **Infested Prism**
- Reworked the Axebots fight into a single, more challenging Axebot
- Changed **Skulking Colony**:

- HP increased from 70(75) -> 75(80)
- Hardened Shell increased from 15 -> 20
- Smash move removed; now uses Zoom on turns 1 and 2
- No longer gains Block
- Now gains +1 Strength on its Strength-gain turn at high Ascension

- Changed **Haunted Ship**: now applies 3 Weak on Turn 1 instead of when it attacks; Ramming Speed move removed, now alternates between Swipe and Stomp (always starting with Swipe after Haunt)
- Changed **Punch Construct**: Fast Punch now leads into Strong Punch (instead of the reverse), Ready now leads into Fast Punch, and it applies Frail instead of Weak
- Adjusted enemy HP at the extremes of the Ascension curve (slightly lower at low Ascension, slightly higher at high Ascension):

- **Scrolls of Biting**: 31(32)-38(39) -> 30(33)-37(39)
- **Owl Magistrate**: 234(243) -> 231(247)
- **Slimed Berserker**: 266(276) -> 261(281)

- Buffed **Soul Fysh**: Scream move damage increased from 11(12) -> 13(15)
- Nerfed **Ruby Raider Assassin**: damage decreased from 11(12) -> 10(11)
- Nerfed **Vantom** at lower Ascensions
- Nerfed **Waterfall Giant**: HP healed at lower Ascension decreased from 15 -> 10(15)

**Events:**

- Changed **Slippery Bridge** event: no longer offers to remove the same card multiple times unless it has already offered every other card in your deck at least once
- Something *special* now happens if you have multiple **Lantern Key** cards (does not apply to multiplayer)

**Potions & Relics:**

- Buffed **The Boot** relic: damage increase now also applies to Osty's attacks

**Colorless:**

- Buffed **Gold Axe** card: in multiplayer, now deals damage equal to the number of cards played by ALL players, not just its owner
- Nerfed **Fasten** card: additional Block decreased from 5(7) -> 4(6)

**Ironclad:**

- Reworked **Conflagration** card: now deals 2 damage to ALL enemies 4(5) times (was: 8(9) AOE damage plus 2(3) per other Attack played this turn)
- Reworked **Drum of Battle** card: now a 1-cost Uncommon Skill that draws 2 cards and grants 2(3) Energy when Exhausted (the Energy gain respects Replay and card-duplication effects like Duplicator potion)
- Buffed **Howl From Beyond** card: trigger moved from start of turn -> end of turn
- Buffed **Entrench** card: can now gain extra Block from enchantments like Nimble
- Buffed **Unrelenting** card: damage increased from 12(18) -> 14(20)

**Silent:**

- Reworked and renamed **Follow Through** card -> **Scare** (rarity increased from Common -> Uncommon)
- Changed **Nightmare** card: if you select a card with an Affliction on it, that Affliction is now removed from the copies added to your hand the following turn
- Changed **Predator** card: rarity decreased from Uncommon -> Common
- Buffed **Untouchable** card: upgraded Block gain increased from +2 -> +3
- Buffed **Pounce** card: damage increased from 12(18) -> 14(20)
- Nerfed **Blade of Ink** card: Inky enchantment damage decreased from +2 -> +1

**Regent:**

- Reworked **Parry** card: instead of granting Block when you play Sovereign Blade, **Sovereign Blade** now gains 10(14) Block directly, so the Block shows on the Blade and is affected by powers like Dexterity and Frail
- Slightly reworked **Sword Sage** card: **Sovereign Blade** now gains Replay 1 (was: hits an additional time), which gives it synergy with Parry
- Changed **Astral Pulse** card: changed damage from 14(18) -> 6(8)x2
- Changed **Tyranny** card: now triggers before **Bombardment** card (so if Tyranny Exhausts Bombardment, Bombardment is played immediately after)
- Changed **Sealed Throne** card: upgrade changed from Innate -> lowering Energy cost by 1
- Buffed **Crescent Spear** card: base damage increased from 6 -> 8
- Buffed **Royalties** card: Gold gain increased from 30(35) -> 30(40)
- Buffed **Furnace** card: Forge increased from 4(6) -> 5(7)
- Buffed **Monarch's Gaze** card: Energy cost decreased from 3(2) -> 2(1)
- Nerfed **Reflect** card: Block gain decreased from 17(21) -> 15(20)
- Nerfed **Bulwark** card: Block decreased from 13(16) -> 12(15)
- Nerfed **Patter** card: Block gain decreased from 9(11) -> 8(10)
- Nerfed **Minion Sacrifice** card: Block decreased from 9(12) -> 8(11)

**Necrobinder:**

- Buffed **Death March** card: additional damage increased from 3(4) -> 4(6)
- Buffed **Sic 'Em** card: Summon increased from 2(3) -> 3(4)
- Buffed **The Scythe** card: damage increased from 3(4) -> 4(5)
- Nerfed **Debilitate** card: debuff decreased from 3(4) turns -> 2(3) turns

**Defect:**

- Changed **Fusion** card: Energy cost decreased from 2(1) -> 1; now Exhausts, and loses Exhaust on upgrade
- Changed **Shatter** card: now Evokes all of your Orbs twice; damage decreased from 11(15) -> 7(11)
- Changed **Tesla Coil+** card: damage decreased from 6 -> 4, but Lightning triggers increased from once -> twice
- Buffed **Hyperbeam** card: damage increased from 26(34) -> 28(36)
- Buffed **Synthesis** card: damage increased from 12(18) -> 14(20)
- Buffed **Uproar** card: damage increased from 5(7) -> 6(8)
- Buffed **Infused Core** relic: now also grants "Lightning Orbs deal 1 additional damage"

**Multiplayer:**

- Nerfed multiplayer scaling for enemy Artifact, Slippery, and Skittish powers
- **Stratagem** card is no longer banned from the colorless card pool in multiplayer (the original ban was due to a now-fixed bug)
- Buffed **Tag Team** card: Replay effect now triggers on "deal damage to ALL enemies" attacks too
- Changed **Fossil Stalker**: in multiplayer, now gains a set amount of Strength if it hits any player instead of gaining Strength for each player hit

### QUALITY OF LIFE:

- During the Gremlin Merc fight, if Gremlin Merc didn't steal any gold but Fat Gremlin escaped, you now receive 50% of the normal gold reward instead of 0%

### USER INTERFACE & EXPERIENCE:

**General:**

- Added an Ascension first-time tutorial popup for single player and multiplayer
- If errors occur on run start, a popup is now displayed instead of a black screen
- An error is now displayed when Steam crashes
- The feedback tool now displays an error if your feedback was not received, and added a translation-issue option to the in-game feedback form
- Implemented preferred Ascension for the random character option
- Loading a current_run.save with a modded character back into an unmodded game now shows an error instead of a black screen
- The player's hand now animates out of the way during the enemy turn
- Limited the number of cards rendered during the rest site Clone animation to 15
- Updated Ancient card text backgrounds to fit more text and be slightly darker and embossed

**Leaderboards:**

- Added a global rank paginator and adjusted arrow positions
- You can now see other players' scores near your own
- Moved the game over screen leaderboards higher to make room for the toggle

**Controller:**

- Added controller navigation for Badges in the game over and run history screens, the player info icons in run history, and the modifier icons on the top bar
- Improved controller navigation in the Fake Merchant shop
- Pause menu button navigation now wraps

**Ancients:**

- **Pael's Flesh** relic now visually increases the max energy shown in your Energy counter
- When **Vakuu** tells you how many times you've visited, the number is now accurate
- Hovering over a stale dialogue at Ancients now makes all dialogues opaque

**Enemies:**

- **Skulking Colony**'s health bar now changes color to indicate it cannot take more damage this turn
- Improved **Byrdonis**'s hovertips (Territorial now names Byrdonis, and hovering Byrdonis shows the Strength hovertip)

**Necrobinder:**

- **Sacrifice** card now previews how much Block it will gain

**Multiplayer:**

- Added a more informative error popup for the Steam BadCert multiplayer error

### ART:

**General:**

- Added Epoch art for:

- Friendship
- Booting Up
- Soup
- Poisoner
- Scribbles
- The Calm
- Devastation
- The 12
- The Giant

- Updated Epoch art for:

- Hello Hello!
- Friendship

**Characters:**

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

- Added new character animations:

- **Ironclad** heavy attack
- **Necrobinder** cast
- **Regent** Sovereign Blade
- **Defect** power up

**Ancients:**

- Added **Vakuu** background art and VFX
- Revised **Neow's Fury** artwork (now centered)

**Enemies:**

- Added new power art for Smoggy
- Updated enemy art for:

- **Tunneler**
- **Entomancer**
- **Mawler**

- Added a multi-hit animation for **Corpse Slug**
- Added an unstun animation for **Rock Bowlbug**
- Added **Skulking Colony** animations
- Added new **Soul Nexus** VFX
- Tweaked the **Vantom** explosion to look right on a shrunken Vantom
- Repositioned **Sludge Spinner**'s death landing spot
- Updated enemy animations for:

- **Vantom**
- **Ceremonial Beast**
- **Entomancer**

- Added phobia mode alternate art for:

- **Fossil Stalker**
- **Haunted Ship**
- **Skulking Colony**
- **The Obscura**
- **Twig Slime M**

**Events:**

- Added phobia mode alternate art for **Zen Weaver** event

### AUDIO:

- Added background music for **Soul Fysh** and **Kaiser Crab**
- Added SFX for:

- **Kaiser Crab** enemy
- **Skulking Colony** enemy
- Hyperbeam move
- Cards traveling to a pile

### WRITING:

- **Brilliant Scarf** relic description now specifies it only counts cards played "from your hand" (so it's clear it doesn't count auto-played cards)
- **Reaper Form** card wording now makes clear the Doom is applied by Reaper Form's power rather than the played card
- Fixed inconsistencies in **Jeweled Mask** relic's wording
- **Exterminate** card wording is now consistent with **Conflagration** card
- The word "Power" is no longer keyworded in **Lost Wisp** relic's description
- Reworded the Axebot death message to reflect that there is only one of them now
- Added more translated text and fixed translation errors across many languages

### BUG FIXES:

**General:**

- Fixed save files being lost or corrupted after a PC crash, blue screen, or power outage
- Made save operations fault-tolerant to prevent softlocks, and improved reliability of settings saves on startup
- Stopped cloud-syncing settings.save and fixed Steam Cloud sync errors and duplicate run history files when syncing many files
- Fixed black screen softlock when Steam disconnected during room transitions
- Fixed several crashes: applying powers to dying creatures, closing the game during startup, upgrading a card at a rest site, entering rooms, and opening the card pile screen while targeting an enemy with a controller
- Fixed a Linux crash during combat when UI elements are removed mid-frame, a Linux crash when ICU libraries are unavailable, and audio staying muted after pressing volume keys or alt-tabbing on Linux
- Restored the default rendering backend to D3D12 for Intel 620 GPUs
- Effects that make the next card free (Unrelenting, Void Form, etc.) now correctly take precedence over effects that increase all card costs (Entangled, Spiked Gauntlets, Borrowed Time, etc.)
- Replayed cards now show the appropriate damage if powers wore off between replays
- Fixed cards in hand getting stuck if you try to play them while they are being transformed
- The Night Terrors modifier can no longer kill you, and the Sealed Deck modifier is now affected by the multi-character card modifiers
- The Perfect badge is no longer awarded if you abandon a run at a boss
- Fixed several softlocks: returning to the main menu and resuming a run, playing Compact with status cards in hand, the tutorial shuffle popup, and run histories from older versions
- Fixed score being wrong at Ascension 10, wrong floor count when submitting a Daily score, and score separator lines showing while fetching or after a failure
- Fixed the potion hovertip not appearing when pressing "b" in the potion popup, and num keys for card selection not working when the mouse is too low on screen
- Fixed input settings errors for new keybindings added after saving
- Fixed a rare problem where the feedback screen was stuck open on launch
- Controller input is now ignored while the game window is unfocused
- The game no longer rapidly switches between "Controller Detected" and "Mouse Detected" with a controller connected
- Fixed broken controller navigation: off the top bar in the shop, through rest site options with two rows of relics, in the shop after buying out relics and colorless cards, and from relics to the reward screen after clearing the top reward
- Fixed being able to start a single player run via controller while the Ascension tutorial was open
- A loaded save with a deprecated boss can no longer roll the same double boss
- Fixed an issue with the Game Over screen after discovering multiple enemies while playing in Polish
- Fixed hovertips in the top bar sometimes displaying in the wrong language after switching languages and continuing a run
- Fixed being able to target dead enemies with a controller
- Fixed the odds of a follow-up potion reward sometimes going up instead of down after an Elite potion reward
- Fixed being able to open card inspection and toggle View Upgrades with the same button press
- Taking a card removal reward as your last reward no longer keeps the background faded out

**Ancients:**

- Fixed a crash when selecting Ancient relic options
- Players can no longer roll **Tezcatara's Nutritious Soup** blessing with no Strikes in their deck
- Fixed **Vakuu's Whispering Earring** relic not correctly playing **Stardust** card, auto-playing cards into the next turn after **Void Form**, and not letting **Unceasing Top** relic trigger when it plays your last card
- Fixed **Neow's Kaleidoscope** relic using up all of **Silver Crucible**'s charges
- Fixed **Neow's Silken Tress** relic in multiplayer only affecting card rewards for the host
- Fixed **Nonupeipe's Diamond Diadem** relic resetting its count after **Hellraiser** plays cards at the start of the turn
- **Leafy Poultice** relic no longer transforms the upgraded Strikes/Defends from **Neow's Talisman** when both come from **Neow's Bones**
- Fixed visual and navigation issues at Ancients: the next dialogue bubble peeking in early, navigating back to Neow's blessings from the Card Library, the card inspection button blocking the accept hotkey on **Pandora's Box**, and **Wish** card immediately selecting a grid card on the same controller press

**Enemies:**

- Fixed cards that should be Ethereal sometimes becoming non-Ethereal after killing Spectral Knight
- Fixed creature and boss VFX occasionally failing to appear
- Fixed **Flyconid** spore effect lingering when its attack is interrupted
- Fixed **Kaiser Crab** appearing if you load into an already completed boss room, and its **Rocket Claw** charge-up SFX looping forever
- Fixed a softlock when setting up an infinite against **Skulking Colony**, and Skulking Colony's per-turn damage cap now resets at the start of both turns (so Poison no longer counts toward it on your next turn)
- Fixed a softlock after defeating **Infested Prism**
- Fixed **Thieving Hopper** softlock when you have no cards for him to steal
- Recentered **Axebot** so it reads as a single enemy
- Fixed **Ceremonial Beast** background music transition not lining up with its stun
- Fixed **Living Fog** missing its smoke, and corrected its encounter name in the death quote
- Fixed **Test Subject** starting phase 2 with extra Strength when killed by **Sleight of Flesh** via **Enfeebling Touch**
- Fixed VFX glitch when **Snapping Jaxfruit** dies to the **Reflect** power
- **Inklet** and **Vantom**'s Slippery power no longer loses a stack when fully blocking a hit
- **Zapbot**'s High Voltage hovertip now correctly says it gains Strength at the end of its turn
- Fixed **Parafright** illusion keeping its negative Strength from **Dark Shackles** when killed for the turn
- Fixed **Waterfall Giant** not respawning and its steam spouts misbehaving after its death animation
- Hid **Lagavulin Matriarch**'s sleep particles when awake
- Fixed several **Test Subject** issues

**Events:**

- Fixed being unable to give the **Foul Potion** to the **Fake Merchant**
- Fixed cards from other classes appearing in the **Infested Automaton** event
- Fixed being unable to discard potions in the **Punch Off** event
- Fixed a softlock if you somehow die in the **Battleworn Dummy** event fight, and no longer get its rewards after saving and quitting
- All **Spoils Map** cards are now removed if you complete the quest with multiple copies, and you get 600 gold per copy of the quest card
- Fixed **Crystal Sphere** event giving potion rewards of the wrong rarity
- Fixed **Perfect Fit** enchantment not working if you play **Reboot** while the enchanted card is in your hand or draw pile
- **Gnarled Hammer** relic can no longer be traded at the **Relic Trader** event
- **War Historian Repy** event now removes all duplicate **Lantern Key** cards if you have more than two

**Potions & Relics:**

- Fixed **Mummified Hand** relic sometimes selecting cards that already cost 0
- Fixed **The Boot** relic amplifying **Bad Luck** curse's damage from 1 to 5 while you have Intangible, and dealing the correct 5 damage when you deal Skulking Colony 1-4 damage
- **Self-Forming Clay** relic is no longer triggered by the Terminal modifier's max HP loss
- **Bookmark** relic now applies its cost reduction to a card retained by a "Retain your Hand" effect like Equilibrium
- **Snecko Skull** relic now always triggers before **Unsettling Lamp** relic
- Fixed **Beetle Juice** potion making **Vantom**'s tail appear cut off
- Players can no longer throw the **Foul Potion** while the Merchant inventory is open, and using it on the Merchant no longer freezes the game on controller
- Fixed visual errors with the outline of some potion assets
- Fixed broken controller navigation from relics to card rewards after rerolling via **Driftwood**, and completing **Orrery**'s card selection in the shop no longer also clicks a shop item on controller
- Fixed **Pantograph** relic letting **Red Skull**'s Strength increase persist after healing you above 50% health, until an attack was played

**Colorless:**

- Fixed VFX sometimes failing to appear during the **Rolling Boulder** power effect
- Fixed a softlock when playing **Anointed** card with a Draw Pile larger than your hand can hold
- Fixed being able to highlight creatures while the **Purity** card selection screen is open

**Ironclad:**

- **Howl From Beyond** card now plays from the Exhaust pile after you've drawn your hand instead of before
- Fixed **Anger** card played by **Havoc** also Exhausting on play
- Fixed being able to select an upgraded card with **Armaments** card if it was added to your hand by **Largesse+** card
- **Not Yet** card can no longer be generated via a **Skill Potion**
- To avoid infinite-loop softlocks, **Hellraiser** power now plays only 9 cards per turn when all enemies have infinite HP, and properly plays "Strike" cards against infinite-HP enemies

**Silent:**

- Fixed being able to skip the **Survivor** card discard selection via the peek button
- Fixed **Fan of Knives** card VFX not playing
- You can no longer play cards before **Tools of the Trade** power triggers

**Regent:**

- **Bombardment** card now plays from the Exhaust pile after you've drawn your hand instead of before
- **I Am Invincible** card now triggers before you've discarded your hand instead of after
- **Regalite** relic no longer gains Block from enemies applying statuses
- **Summon Forth** card now puts existing **Sovereign Blade**s into your hand before Forging instead of after
- Fixed not being able to select cards with a controller when you choose your whole hand via **GUARDS!!!**

**Necrobinder:**

- Fixed a crash when Osty dies during a scene transition
- Fixed **Reave** card always bottom-decking the **Soul** card it creates
- Fixed **Fetch** card not glowing gold when drawn by **Gambling Chip** relic
- Fixed **Enfeebling Touch** power causing **Ceremonial Beast** to gain Strength after you stun it
- **Funerary Mask** relic now shuffles Souls into the Draw Pile before your hand is drawn

**Defect:**

- Fixed **Beckon** still damaging the player if combat is ended by Lightning orbs
- Fixed a spacing issue in the **Consuming Shadow** power hovertip
- **Scrape** no longer discards cards that cost zero Energy but have a Star cost

**Multiplayer:**

- Fixed a crash when using potions in multiplayer due to state divergence
- Fixed state divergences related to event combats, the **Crystal Sphere** event, long event option resolution, **Mad Science**/**Juggling**  cards with mixed combat speeds, and **Stampede** + **Headbutt** cards
- Fixed player names with brackets (e.g. clan tags) causing errors
- Fixed **Pael's Legion** and **Vambrace** relics not proccing when the owner applies Block to other players
- Fixed occasional black screen when loading into a saved multiplayer combat
- Fixed incorrect cards being shown when obtaining cards at certain events, and **Orrery** relic reporting the wrong cards to other players
- Potions are now canceled if targeted on a creature killed by another player
- Corrected **Test Subject** HP scaling in multiplayer
- **Dense Vegetation** can no longer appear if any player has less than 8 HP
- Fixed a softlock using **Driftwood** relic on your first run, and a rare softlock loading into a treasure room when a teammate uses a controller
- **Strangle** power no longer procs off other players' card plays
- Relics with turn-specific triggers like **Candelabra** no longer skip their trigger when another player takes an extra turn with **Pael's Eye**
- **Nightmare** card VFX no longer plays for all players
- Other players' HP bars no longer go offscreen when resizing the window in settings
- **Pillar of Creation**, **Supermassive**, and **Arsenal** cards now proc for the player who played **Largesse** rather than the recipient
- You can no longer buy items at the shop while transitioning rooms, and exiting the Merchant no longer skips history recording for remaining players
- The correct error message is now shown for a Steam multiplayer connection timeout (error 5008), and improved reporting for a rare connection error
- The "Show Multiplayer Drawings" tickbox is now correctly localized
- You can no longer start a loaded multiplayer game with only one person

### PERFORMANCE:

- Reduced stuttering when combat effects appear for the first time in a run

### MODDING:

- The base game no longer deletes progress from mods that are removed or errored

### LATEST BETA FIXES:

**General:**

- Fixed many instances of inconsistent RNG behavior (thanks tckmn!)
- Fixed more cases of daily runs diverging based on player network ID

**Multiplayer:**

- Fixed a state divergence caused by the host changing Ascension as the game is starting

**Modding:**

- Began publishing STS2.dll XML documentation
- Fixed Steam mods causing ReflectionHelper.ModTypes to return nothing
