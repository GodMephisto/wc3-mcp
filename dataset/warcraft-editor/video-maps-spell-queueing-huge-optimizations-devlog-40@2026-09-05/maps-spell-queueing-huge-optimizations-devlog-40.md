---
type: vendor-doc
package: video-elegon
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=hIgTHJusrpw
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 597
chapters: 6
timestamped: yes
---

# Maps, Spell Queueing & HUGE Optimizations | Devlog 40

Source: <https://www.youtube.com/watch?v=hIgTHJusrpw>

Uploader: Elegon | Duration: 9m57s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

Elegon is an MMORPG I’m solo developing in Godot Engine and SpacetimeDB, inspired by games such as Classic World of Warcraft (Vanilla, TBC, WOTLK), OS RuneScape and many more.

Elegon will never contain any pay-to-win or cosmetic microtransactions. Rather, it will be supported through a simple monthly subscription cost.

𝔒𝔣𝔣𝔦𝔠𝔦𝔞𝔩 𝔚𝔢𝔟𝔰𝔦𝔱𝔢: https://www.elegon.app
𝔖𝔲𝔭𝔭𝔬𝔯𝔱 𝔐𝔢: https://patreon.com/Elegon
𝔚𝔦𝔰𝔥𝔩𝔦𝔰𝔱 𝔬𝔫 𝔖𝔱𝔢𝔞𝔪: https://store.steampowered.com/app/4235360/Elegon/
𝔇𝔦𝔰𝔠𝔬𝔯𝔡: https://discord.gg/ZHguChjG83
𝔅𝔞𝔠𝔨𝔤𝔯𝔬𝔲𝔫𝔡 𝔐𝔲𝔰𝔦𝔠: https://www.youtube.com/channel/UCtPuRdRH_W26ZxmDSELUJFw

Try SpacetimeDB!
https://spacetimedb.com/?referral=keone

𝔗𝔦𝔪𝔢𝔰𝔱𝔞𝔪𝔭𝔰
00:00 Intro
00:28 Main Map
02:46 Spell Queueing
04:14 Soft Loading
05:34 Server-side Optimizations
08:18 Wrapping up

## Chapters

- [0:00](https://www.youtube.com/watch?v=hIgTHJusrpw&t=0s) Intro
- [0:28](https://www.youtube.com/watch?v=hIgTHJusrpw&t=28s) Main Map
- [2:46](https://www.youtube.com/watch?v=hIgTHJusrpw&t=166s) Spell Queueing
- [4:14](https://www.youtube.com/watch?v=hIgTHJusrpw&t=254s) Soft Loading
- [5:34](https://www.youtube.com/watch?v=hIgTHJusrpw&t=334s) Server-side Optimizations
- [8:18](https://www.youtube.com/watch?v=hIgTHJusrpw&t=498s) Wrapping up

## Transcript

**[0:00](https://www.youtube.com/watch?v=hIgTHJusrpw&t=0s)** Hello everyone and welcome back to another update for Eleon, the MMORPG that I am solo developing. And this [music] week I've got plenty to talk about as always.

**[0:10](https://www.youtube.com/watch?v=hIgTHJusrpw&t=10s)** I'm going to be talking about more updates to the map system.

**[0:15](https://www.youtube.com/watch?v=hIgTHJusrpw&t=15s)** Other quality of life improvements such as spell queuing and soft loading as well as plenty of server side optimizations that I want to talk about as well.

**[0:24](https://www.youtube.com/watch?v=hIgTHJusrpw&t=24s)** >> [music] >> So plenty to go over this week and yeah, hope you enjoy.

**[0:28](https://www.youtube.com/watch?v=hIgTHJusrpw&t=28s)** So the first thing I'd like to talk about is the addition of a main map. So this is building upon the mini map system that I added last week.

**[0:38](https://www.youtube.com/watch?v=hIgTHJusrpw&t=38s)** And the way the main map works is if you press M by default on your keyboard, that will open the map panel. And this will show a map of the zone that you're currently in. Now when you first start, pretty much all of the map is going to be covered and like enshrouded in this like fog.

**[0:57](https://www.youtube.com/watch?v=hIgTHJusrpw&t=57s)** And as you explore and discover new areas, the fog will kind of like recede and you'll be able to actually see the discovered areas.

**[1:05](https://www.youtube.com/watch?v=hIgTHJusrpw&t=65s)** >> [music] >> So I'm really happy with actually how this turned out. And I definitely didn't want to have a situation where I was just kind of like showing the entire zone immediately to to new players.

**[1:18](https://www.youtube.com/watch?v=hIgTHJusrpw&t=78s)** So I feel like this is a good way of adding a map whilst still keeping the sense of adventure and discovery.

**[1:26](https://www.youtube.com/watch?v=hIgTHJusrpw&t=86s)** Now the way that this system works is the zone is kind of split up into a grid of cells. And each of these cells kind of represents one kind of like block of this fog texture. And depending on your player location, if you are in a cell that hasn't been discovered yet, then we will then mark that as discovered on the server and that part of the map will become revealed for you.

**[1:53](https://www.youtube.com/watch?v=hIgTHJusrpw&t=113s)** >> [music] >> And to make it a little bit less tedious, I've kind of made a like a round radius around the player. So you don't have to go and get every last little cell yourself.

**[2:04](https://www.youtube.com/watch?v=hIgTHJusrpw&t=124s)** It kind of works in that as you're moving around, all of the sort of like area around you will become discovered as you move.

**[2:13](https://www.youtube.com/watch?v=hIgTHJusrpw&t=133s)** So yeah, I'm really happy with it. I think it's you know, it's a little less granular than sort of like pixel perfect moving around discovering the map. I didn't really want that cuz that kind of leaves room for like tiny little areas that are like undiscovered. And then I also didn't want to go the other way where it's like you have a system like wow where if you step into an area, you kind of discover that whole area.

**[2:41](https://www.youtube.com/watch?v=hIgTHJusrpw&t=161s)** So this is kind of a midpoint between those two systems.

**[2:45](https://www.youtube.com/watch?v=hIgTHJusrpw&t=165s)** And I think it works quite well.

**[2:47](https://www.youtube.com/watch?v=hIgTHJusrpw&t=167s)** Next I'd like to talk about a new system that I've added which is spell queuing.

**[2:52](https://www.youtube.com/watch?v=hIgTHJusrpw&t=172s)** Now spell queuing has been kind of requested on user job for a while now.

**[2:57](https://www.youtube.com/watch?v=hIgTHJusrpw&t=177s)** It's been very requested. So I'm really happy to get that into the game now. The way that this works is that if you are towards the end of your current ability and you're about to come off cool down to press your next ability, if you press your next ability within a 1 second window before it's actually ready, then that ability will be queued up to instantly fire as soon as it's available.

**[3:25](https://www.youtube.com/watch?v=hIgTHJusrpw&t=205s)** So this is great primarily I would say for like casters. So let's say you're in the last second of casting fireball and then you want to I don't know, you could say like cast another fireball.

**[3:37](https://www.youtube.com/watch?v=hIgTHJusrpw&t=217s)** If you press the the button again within the last second, you can rest assured that that second fireball is going to go off instantly.

**[3:47](https://www.youtube.com/watch?v=hIgTHJusrpw&t=227s)** So this is kind of one of those systems that most people don't really think about when they're playing the game, but when it's missing, you kind of notice it, right? So it just it makes the combat and the game play feel a lot smoother.

**[4:00](https://www.youtube.com/watch?v=hIgTHJusrpw&t=240s)** And I know I did mention the mage class there, but you know, even on other classes like the knight with the melee abilities, >> [music] >> it still works quite nicely and it kind of smooths out that game play. So I'm really happy to have that in the game now.

**[4:13](https://www.youtube.com/watch?v=hIgTHJusrpw&t=253s)** Yeah.

**[4:15](https://www.youtube.com/watch?v=hIgTHJusrpw&t=255s)** Now I was also getting reports of issues around when players were dying and respawning, various parts of the user interface or the game state would be resetting.

**[4:26](https://www.youtube.com/watch?v=hIgTHJusrpw&t=266s)** And the reason for that is because previously when someone was respawning or recalling back to the spawn point, I would kind of trigger a full reload of that character into the world scene.

**[4:40](https://www.youtube.com/watch?v=hIgTHJusrpw&t=280s)** Which didn't really make a lot of sense, right? Because you're already in the world scene and usually when you're respawning, you're not traveling that far. So most of those assets and data can be kind of retained.

**[4:53](https://www.youtube.com/watch?v=hIgTHJusrpw&t=293s)** And that's exactly what I've done. So this past week I've implemented the idea of soft loading >> [music] >> whereby you still get sent to a loading screen when you respawn, but I'm not necessarily loading in all of the data for the scene again, right? Because most of that data is already there.

**[5:11](https://www.youtube.com/watch?v=hIgTHJusrpw&t=311s)** And this fixes a bunch of bugs.

**[5:15](https://www.youtube.com/watch?v=hIgTHJusrpw&t=315s)** So one of one of the main ones was kind of >> [music] >> you know, if you had messages in chat, previously they were getting kind of like wiped when you would respawn.

**[5:26](https://www.youtube.com/watch?v=hIgTHJusrpw&t=326s)** But now they kind of all get retained.

**[5:29](https://www.youtube.com/watch?v=hIgTHJusrpw&t=329s)** And that goes for pretty much any other game state.

**[5:35](https://www.youtube.com/watch?v=hIgTHJusrpw&t=335s)** Now as far as the client side goes, that's pretty much everything.

**[5:39](https://www.youtube.com/watch?v=hIgTHJusrpw&t=339s)** As always, there has been a bunch of bug fixes and little improvements here and there on the client side. But in terms [music] of kind of like main features that I've added this past week, that's everything.

**[5:51](https://www.youtube.com/watch?v=hIgTHJusrpw&t=351s)** Now next up I do want to talk about the server side changes that I've made and that has basically been all focused on server side performance and optimizations.

**[6:02](https://www.youtube.com/watch?v=hIgTHJusrpw&t=362s)** >> [music] >> So one thing that I've actually done is I've taken the time to refactor the directory structure of our server code.

**[6:10](https://www.youtube.com/watch?v=hIgTHJusrpw&t=370s)** So it's all a lot more readable and maintainable. And it just means that as the project grows on the server side, it's not going to be super complex for me to kind of like you know, maintain it.

**[6:24](https://www.youtube.com/watch?v=hIgTHJusrpw&t=384s)** So I'm very happy with how that's gone.

**[6:27](https://www.youtube.com/watch?v=hIgTHJusrpw&t=387s)** Now as well as that, I've also implemented plenty of systems on the server side.

**[6:32](https://www.youtube.com/watch?v=hIgTHJusrpw&t=392s)** >> [music] >> So I've kind of implemented a system whereby work is now being kind of like I guess load balanced a lot better than it previously was.

**[6:46](https://www.youtube.com/watch?v=hIgTHJusrpw&t=406s)** So there's a lot of things and actions that need to happen kind of like on a very regular tick.

**[6:53](https://www.youtube.com/watch?v=hIgTHJusrpw&t=413s)** So think for example like the the mobs wandering around the world. That is something where every mob needs to kind of be progressed every X amount of frames, right? Or every X amount of time.

**[7:07](https://www.youtube.com/watch?v=hIgTHJusrpw&t=427s)** So I've kind of made a bunch of optimizations around that system based on a couple of assumptions I can make about the game play, right? So mobs for example that don't have any players in their current chunk will have a significantly lower like up update tick than mobs that are in a chunk with other players.

**[7:33](https://www.youtube.com/watch?v=hIgTHJusrpw&t=453s)** And furthermore to that, mobs are actually actively in combat with a player will have the fastest update rate, right? So they will get very fast updates.

**[7:45](https://www.youtube.com/watch?v=hIgTHJusrpw&t=465s)** And this basically really takes a lot of weight off the server because if you think about it, in the world of Eleon, we're going to be having thousands and thousands of mobs. But only a small percentage of those mobs at any one time will actually need the super fast server updates.

**[8:03](https://www.youtube.com/watch?v=hIgTHJusrpw&t=483s)** So this is really great for now.

**[8:06](https://www.youtube.com/watch?v=hIgTHJusrpw&t=486s)** I'm already seeing massive performance improvements on the server side. But more importantly, it's really really scalable and great for later on as I continue to add more and more mobs to Eleon.

**[8:19](https://www.youtube.com/watch?v=hIgTHJusrpw&t=499s)** So that's kind of the main points, the main things that I've been working on this past week. I know this video has been a little bit rambly, but I'm kind of happy with that to be honest. I've wanted to have a little bit more of a laid back video this week.

**[8:32](https://www.youtube.com/watch?v=hIgTHJusrpw&t=512s)** Tiny tiny bit less structured.

**[8:35](https://www.youtube.com/watch?v=hIgTHJusrpw&t=515s)** And yeah, as always, if you want to see the the more granular updates and every single like change that I've made, do feel free to head over to the user drop page and check out the change log there where I've kind of got a write up of everything that I've been working on this past week if you really want to see every little change and bug fix.

**[8:59](https://www.youtube.com/watch?v=hIgTHJusrpw&t=539s)** But yeah, that's pretty much everything for this video.

**[9:02](https://www.youtube.com/watch?v=hIgTHJusrpw&t=542s)** I do also want to say a bit of an apology actually. So last video I kind of made a mistake with my OBS settings where I was cutting off some of my Patreon supporters. So they just weren't showing up on the screen.

**[9:17](https://www.youtube.com/watch?v=hIgTHJusrpw&t=557s)** And honestly I felt really bad for that one I when I went in and checked it. But yeah, that should all be working now.

**[9:26](https://www.youtube.com/watch?v=hIgTHJusrpw&t=566s)** And yeah, I do want to apologize for that last video. Some some of my supporters were cut off. So yeah, definitely happy to have that sorted now and I guess it's just part of the learning process for me.

**[9:39](https://www.youtube.com/watch?v=hIgTHJusrpw&t=579s)** But as always, thank you so much for watching everyone. Thank you all for your amazing support. And I look forward to seeing you in the next one.
