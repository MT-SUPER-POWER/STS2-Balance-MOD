---
date: "2026-04-16"
version: "v0.103.1"
title: "Beta Hotfix Patch Notes - v0.103.1"
source: "https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1830163047256473"
author: "demileaf"
---

Hey Slayers, we've got a quick beta branch hotfix for you today! There won't be a big beta patch this week, but you can expect the beta content, changes, and fixes to come to the main branch soon.

**BUG FIXES:**

**General:**

- Fixed occasional crash when upgrading a card at rest site
- Fixed crash that would sometimes occur when entering rooms

- When transitioning between rooms, old assets were being freed immediately while the game was still rendering them on screen. This caused rare crashes to desktop, most often when entering events, shops, or campsites. The fix defers the actual freeing to the end of the frame, after rendering is complete.

- Fixed an issue with the Game Over screen after discovering multiple enemies while playing in Polish
- Fixed rare combat errors that could occur when re-entering combat
- Fixed softlock on run histories from older versions of the game
- **Not Yet** card can no longer be generated in combat by effects like Skill Potion
- Score is now correct when at Ascension 10

**Multiplayer:**

- **Colorful Philosophers** event can no longer be encountered if a player has only Ironclad unlocked
- Fixed curse mismatching when some players would take **Neow's Bones** relic
- Fixed state divergence when **Neow's Bones** relic would trigger multiple reward screens
- Fixed not being able to load into a Daily multiplayer run
