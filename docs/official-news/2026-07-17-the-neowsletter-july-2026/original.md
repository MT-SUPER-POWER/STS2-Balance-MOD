---
date: "2026-07-17"
version: ""
title: "The Neowsletter - July 2026"
source: "https://steamstore-a.akamaihd.net/news/externalpost/steam_community_announcements/1838407329260598"
author: "demileaf"
---

![](https://clan.fastly.steamstatic.com/images/44971832/68ace34988009b245bba077ec4a2582d5cabb320.png)

The July Neowsletter is here, Slayers! Last month we bid farewell to the Doormaker, but you know what they say: when one door closes... a giant floating hourglass creature appears! In this issue we're taking a look behind the scenes at the process of bringing the Doormaker's replacement, Aeonglass, to life!

### Aeonglass Animated

Mega Crit’s animator Chris here with a quick dive into the animation process for our latest boss: **Aeonglass**. Most of you know that this critter replaces the Doormaker. What you probably don't know is that, as that character's design evolved away from door-making, we explored other concepts, including a multi-faceted pillar of death that could rotate its sections to reveal different capabilities--sort of a Swiss Army rock. I even did an animation test to see if I could fake the 3D rotation using 2D software.

![](https://clan.fastly.steamstatic.com/images/44971832/8f8792c30304b8c92c5948c229477a09336bd197.png)
![](https://clan.fastly.steamstatic.com/images/44971832/f32d7e06ed671f254d544ef4ef069e0aa3594118.gif)

But alas, all versions of the Doormaker eventually fell prey to the march of time and it became clear that Aeonglass was here to stay.

![](https://clan.fastly.steamstatic.com/images/44971832/6121098dd6a50007d597435dea91ed0f3a38f9f1.png)
The art was finalized (see Option B above) and I was clear to animate. But before I got too carried away, I did some tests to make sure I could simulate 3D rotating metal rings in 2D space. The result wasn’t much to look at, but as a proof of concept it worked well enough for us to move ahead.

![](https://clan.fastly.steamstatic.com/images/44971832/898f298d98688c9072645b8d96574361be0aef78.gif)

So how does a giant hourglass thingy with rotating rings cast spells, fight, and generally do stuff? Only one way to find out: doodling. Take the image included here. It shows that one of my first ideas for an attack involved locking the rings in a fixed position resembling a big atom. While I liked the idea of the rings forming a recognizable shape while emitting a blast, I later realized that aligning the rings horizontally like a disk would create a more distinctive silhouette. Not only that, but the curve of the disk suggests a horizontal sweeping motion. With a little bit of VFX, we could launch some sort of slice attack. Our giant hunk of metal and glass was developing an attitude!

![](https://clan.fastly.steamstatic.com/images/44971832/bebbc11ee6d65dfaba413f86be13b9ce68710942.png)

Attitude secured, it was time to animate. This part of the process takes a while and I won’t get into the details, but Aeonglass did require a few special techniques. For instance, when our Art Director Marlowe handed me the artwork, she requested that the stuff in the hourglass move more like lava lamp goo than the traditional sand. There are a few ways to approach goo animation, but I opted to use a shader. In fact, I made this goo using the same smoke shader used to make the body of the Living Smog. Since my animation software doesn't support shaders, all I see while I'm working is a striped placeholder image where the goo will be.

![](https://clan.fastly.steamstatic.com/images/44971832/8266fe44f3992ff5351b5c50a8c73ee9f80ebead.gif)

When the animation is played back in-engine, the placeholder is replaced by an endless stream of time globs slipping away into eternity.

![](https://clan.fastly.steamstatic.com/images/44971832/37f96ab06c915ff9f4bda409ea56a48d6203ea5c.gif)

Another unusual technique was setting up the rings to play on a separate timeline from the rest of the character. This means that the character can go from standing still to getting knocked back to debuffing you with "Wither," all without interrupting the smooth rotation of the rings. Keeping the rotation separate is also easier for me because tweaking it can be done by editing one animation instead of three.

Speaking of time globs, I don't want to use up too many of yours! Let's see what else is going on in this Neowsletter...

### Merch Madness

![](https://clan.fastly.steamstatic.com/images/44971832/5d4cbe9eb93e4d0ee850a0c4265d7d8047ca9b2e.png)
*Ho ho!* Looks like the Merchant heard about the merchandising ventures of some other Spire inhabitants and had to get in on the action! Thanks to the latest *Makeship x Slay the Spire drop*, you can buy a plush keychain of the Merchant's faithful Courier, gleaming pins of his wares, and even a plushie of the Merchant and his rug! His real rug might not be for sale... but no one ever said anything about a mini, plush replica of it!

These limited edition products are available only to the speediest of patrons, so pre-order yours before **[this collection](makeship.com/slay-the-spire) disappears on August 1st at 5AM PST!**

![](https://clan.fastly.steamstatic.com/images/44971832/bfed5f7d857348b6f89ff670a0cd03f628014d08.png)
You helped bring their concepts to life and now they're finally here--the *Youtooz figures of the Necrobinder and Regent* are [available now](https://youtooz.com/collections/slay-the-spire)! The newest additions to the Spire's roster of climbers have taken smaller, vinyl-based forms in order to grace your shelves and watch over your future runs. The collection is complete... for now.

![](https://clan.fastly.steamstatic.com/images/44971832/34f124194db80deef8169628975851e76303b9e6.png)
Lastly, we've got a cool collab to highlight: turns out the Ironclad was enjoying StS2's co-op mode so much he decided to join another multiplayer game?! You can play as a floppier, less-limbed version of everyone's favorite warrior in [Heave Ho 2](https://store.steampowered.com/app/2802740/Heave_Ho_2/) (which literally just came out)!

### Connections Section

![](https://clan.fastly.steamstatic.com/images/44971832/b807194dac8589b38580f74ee022b450b721a30b.png)
The last Slay the Spire Connections was a little tricky... hopefully [this one](https://custom-connections-game.vercel.app/Lg0v4IrhEzSGMhdsODcp) is more chill ;)

### Map Masterpieces

Time for our usual showcase of the community's cool in-game map art!

![](https://clan.fastly.steamstatic.com/images/44971832/901e590c89002d426c350d314c147532bd6fd070.png)
The Defect admiring its array of orbs by **DogOnPluto**!

![](https://clan.fastly.steamstatic.com/images/44971832/ac4a7e8c5feb9c8968dd73a1d5868b0b20ed0def.png)
The Regent (playing a different type of ball than most of the world is talking about right now) by **thugzmcdubbz**!

![](https://clan.fastly.steamstatic.com/images/44971832/93c3b6ab3465c61bafd858b44dcc843a607d9227.png)

The Ironclad literally carrying the Regent by **rattraps**!

![](https://clan.fastly.steamstatic.com/images/44971832/c8d82a8d65f37458666d79f665c2bbb3fc3927d2.png)

The Defect asking for his favorite treats by **Lumin**!

### Community Corner

Thank you to **pp** for this month's super cool cover image (check out Neow's custom shades)! Special shoutout to our runner up **valarts** who actually 3D-modeled their submission! This one makes me wonder... is the Defect waterproof?

![](https://clan.fastly.steamstatic.com/images/44971832/9ca67d72b06276f0a48cf8dc9d8cdc66ba946c06.jpg)

Next month's theme is: **Slay the Summer - Festival Edition**! You could include water lanterns, food stalls, fireworks, whatever you like--just make it August-y! If you'd like to submit your art for consideration, reminder that it must be submitted to us via fanmail@megacrit.com or in our [Discord's](https://discord.gg/slaythespire) fanart channel, have dimensions of 800x450px, feature no text, and leave room for a title! Please make your submission by **Friday, August 7th!**

On with the rest of the community highlights!

![](https://clan.fastly.steamstatic.com/images/44971832/fdcddc1c013712cd070ad42b771fcbc5e1bd2131.gif)
An animated version of the Ironclad's Evil Eye card by (https://www.youtube.com/@Qumerredem)!

![](https://clan.fastly.steamstatic.com/images/44971832/b95c57e0cf4b06c9aa72c0dc4857114c246d8cdc.jpg)
Awesome cosplay by (https://www.reddit.com/r/slaythespire/comments/1u8ec9p/my_silent_cosplay/) of the Silent partaking of her own poison!

![](https://clan.fastly.steamstatic.com/images/44971832/5a0d2b424721600703e569591c71db208191a07e.jpg)
A desk setup that SLAYS by (https://bsky.app/profile/ches-sky-nut.bsky.social) featuring his very own gorgeous fanart!

That's all for this month's issue, see you in the next one!
