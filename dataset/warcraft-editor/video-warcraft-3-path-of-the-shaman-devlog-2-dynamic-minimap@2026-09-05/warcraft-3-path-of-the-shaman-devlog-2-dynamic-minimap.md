---
type: vendor-doc
package: video-path-of-the-shaman
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=jivGzzgpHnM
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 978
timestamped: yes
---

# Warcraft 3: Path of the Shaman - Devlog #2 - Dynamic Minimap!

Source: <https://www.youtube.com/watch?v=jivGzzgpHnM>

Uploader: Path of the Shaman | Duration: 16m18s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

Some brief showcase and development discussion of dynamic minimap system for WC3.
Custom Warcraft 3 RPG map "Path of the Shaman".
Support me at PATREON:  https://www.patreon.com/c/wc3pots
Hiveworkshop: https://www.hiveworkshop.com/threads/path-of-the-shaman.367310/

## Transcript

**[0:00](https://www.youtube.com/watch?v=jivGzzgpHnM&t=0s)** Welcome to another mapping session or let's just say that I'm creating custom mini map system.

**[0:15](https://www.youtube.com/watch?v=jivGzzgpHnM&t=15s)** I will show you what I've been working on with.

**[0:19](https://www.youtube.com/watch?v=jivGzzgpHnM&t=19s)** So this system basically um makes the use of uh multiple hundreds of uh mini maps and dynamically ups the updates the mini map but I think it's easier to just show what it does than to me try to explain Basically, the um basic vanilla mini map is very low quality and especially for bigger maps. It's hard to see any details.

**[1:06](https://www.youtube.com/watch?v=jivGzzgpHnM&t=66s)** So, I wanted to get more like a wolf style World of Warcraft style map, mini map. And I also figured that what if we would could uh make like a open map enlarge version two. But let's just see.

**[1:24](https://www.youtube.com/watch?v=jivGzzgpHnM&t=84s)** I'm trying to position the vanilla map location.

**[1:31](https://www.youtube.com/watch?v=jivGzzgpHnM&t=91s)** Oh, and by the way, this map is u the mini map testing map. So this is not the path of the sham map.

**[1:39](https://www.youtube.com/watch?v=jivGzzgpHnM&t=99s)** I wanted to get it right before I transfer it to the real map.

**[1:51](https://www.youtube.com/watch?v=jivGzzgpHnM&t=111s)** But the mini map pictures here are from the real map.

**[1:57](https://www.youtube.com/watch?v=jivGzzgpHnM&t=117s)** So this is what I mean.

**[2:04](https://www.youtube.com/watch?v=jivGzzgpHnM&t=124s)** I have my debug messages active. So basically it's switching the mini map based on the camera and this picture is taken from world editor uh with this kind of function.

**[2:33](https://www.youtube.com/watch?v=jivGzzgpHnM&t=153s)** So basically I just give the entire map, take a screenshot, then I scale it to [music] proper size and then I did some AI upscaling [music] stuff also.

**[2:48](https://www.youtube.com/watch?v=jivGzzgpHnM&t=168s)** And then I [music] had this Python script that chunk it into smaller pieces.

**[2:54](https://www.youtube.com/watch?v=jivGzzgpHnM&t=174s)** [music] Basically like chunk here, chunk here, chunk here, you know, [music] made it into small chunks.

**[3:11](https://www.youtube.com/watch?v=jivGzzgpHnM&t=191s)** And this enlarge function currently works with S key.

**[3:18](https://www.youtube.com/watch?v=jivGzzgpHnM&t=198s)** So it could be much better. But think about it. If this map would be the basic vanilla map, it would look very horrible.

**[3:30](https://www.youtube.com/watch?v=jivGzzgpHnM&t=210s)** So this is basically from in game when you think about it.

**[3:38](https://www.youtube.com/watch?v=jivGzzgpHnM&t=218s)** In-game screenshot even though it's from world editor.

**[3:45](https://www.youtube.com/watch?v=jivGzzgpHnM&t=225s)** Let's see if the horse season is now.

**[3:48](https://www.youtube.com/watch?v=jivGzzgpHnM&t=228s)** Yeah, it's kind of could be a little slightly lower.

**[3:55](https://www.youtube.com/watch?v=jivGzzgpHnM&t=235s)** >> I am not afraid. As you wish. For honor.

**[3:59](https://www.youtube.com/watch?v=jivGzzgpHnM&t=239s)** For my people, it shall be done. As you wish.

**[4:04](https://www.youtube.com/watch?v=jivGzzgpHnM&t=244s)** >> I think for my people. For honor it shall be done.

**[4:10](https://www.youtube.com/watch?v=jivGzzgpHnM&t=250s)** >> As you wish.

**[4:13](https://www.youtube.com/watch?v=jivGzzgpHnM&t=253s)** For honor.

**[4:14](https://www.youtube.com/watch?v=jivGzzgpHnM&t=254s)** >> Why doesn't it?

**[4:15](https://www.youtube.com/watch?v=jivGzzgpHnM&t=255s)** >> It shall be done.

**[4:18](https://www.youtube.com/watch?v=jivGzzgpHnM&t=258s)** For my people.

**[4:20](https://www.youtube.com/watch?v=jivGzzgpHnM&t=260s)** >> For honor. It shall be done as you wish.

**[4:23](https://www.youtube.com/watch?v=jivGzzgpHnM&t=263s)** For my people. For honor. For my people.

**[4:28](https://www.youtube.com/watch?v=jivGzzgpHnM&t=268s)** >> Camera. Apparently should update to update this view.

**[4:32](https://www.youtube.com/watch?v=jivGzzgpHnM&t=272s)** >> It shall be done as you wish for my people. It shall be done for honor. As you wish. As you wish. It shall be done.

**[4:42](https://www.youtube.com/watch?v=jivGzzgpHnM&t=282s)** >> I would like to have like a small cool looking border here. But >> for my people.

**[4:48](https://www.youtube.com/watch?v=jivGzzgpHnM&t=288s)** >> That's another story.

**[4:49](https://www.youtube.com/watch?v=jivGzzgpHnM&t=289s)** >> For honor. For my people. As you wish.

**[4:54](https://www.youtube.com/watch?v=jivGzzgpHnM&t=294s)** >> It shall be done for honor.

**[4:56](https://www.youtube.com/watch?v=jivGzzgpHnM&t=296s)** >> As you wish.

**[4:58](https://www.youtube.com/watch?v=jivGzzgpHnM&t=298s)** >> This camera bound needs to go to uh outside the camera bound or how do you say it to update this view. So, we need to force the update when we enlarge the map.

**[5:16](https://www.youtube.com/watch?v=jivGzzgpHnM&t=316s)** And I hate to say it, I'm just using so heavily this AI stuff.

**[5:24](https://www.youtube.com/watch?v=jivGzzgpHnM&t=324s)** So, bear with me because I know some jazz and scripting and coding and you know, but this sometimes this makes my life so much easier.

**[5:49](https://www.youtube.com/watch?v=jivGzzgpHnM&t=349s)** Where's the It's just not tell too much. I've noticed that sometimes this co-pilot works or cloud works best if you don't write too much stuff at same prompt.

**[6:22](https://www.youtube.com/watch?v=jivGzzgpHnM&t=382s)** But sometimes it needs you to write lots of details. But then I've noticed that if you write lots of details, it kind of like a likes to pick what it thinks that are relevant information in the prompt. So, and it skips them.

**[6:44](https://www.youtube.com/watch?v=jivGzzgpHnM&t=404s)** Let's see what it tells us.

**[6:54](https://www.youtube.com/watch?v=jivGzzgpHnM&t=414s)** I have an unholy alliance with the gold pilot cloud.

**[7:02](https://www.youtube.com/watch?v=jivGzzgpHnM&t=422s)** Many systems are have have mutes um usually manually created by me, but they also have lots of AI stuff.

**[7:17](https://www.youtube.com/watch?v=jivGzzgpHnM&t=437s)** So, I know that many of you guys will say that I'm a a [snorts] noob or something, and you're probably right.

**[7:35](https://www.youtube.com/watch?v=jivGzzgpHnM&t=455s)** But let's just be realistic. Uh there's so much stuff to do and we need to have the stuff to do uh done by at least at some point. So this speeds the process quite a bit at least for me.

**[8:03](https://www.youtube.com/watch?v=jivGzzgpHnM&t=483s)** Let's see how it works now.

**[8:09](https://www.youtube.com/watch?v=jivGzzgpHnM&t=489s)** I don't even know if that toggle updating was the real issue, but let's see.

**[8:19](https://www.youtube.com/watch?v=jivGzzgpHnM&t=499s)** And that's the thing.

**[8:22](https://www.youtube.com/watch?v=jivGzzgpHnM&t=502s)** You kind of have to really know the full uh code uh or or the script or you have to have kind of understanding that what you're going to want or what you want to do or what you want to change.

**[8:37](https://www.youtube.com/watch?v=jivGzzgpHnM&t=517s)** You have to be able to tell it to AI.

**[8:41](https://www.youtube.com/watch?v=jivGzzgpHnM&t=521s)** >> I am not afraid. As you wish.

**[8:46](https://www.youtube.com/watch?v=jivGzzgpHnM&t=526s)** >> For honor.

**[8:47](https://www.youtube.com/watch?v=jivGzzgpHnM&t=527s)** >> Yeah. Now it works.

**[8:48](https://www.youtube.com/watch?v=jivGzzgpHnM&t=528s)** >> For my people.

**[8:49](https://www.youtube.com/watch?v=jivGzzgpHnM&t=529s)** >> As I say, >> it shall be done as you wish. For my people. For honor. It shall be done as you wish. For honor it shall be done.

**[8:59](https://www.youtube.com/watch?v=jivGzzgpHnM&t=539s)** For my people. For honor it shall be done as you wish. For my people. For honor, for my people, it shall be done as you wish.

**[9:09](https://www.youtube.com/watch?v=jivGzzgpHnM&t=549s)** >> At best, >> is the position correct?

**[9:12](https://www.youtube.com/watch?v=jivGzzgpHnM&t=552s)** >> For my people, it shall be done.

**[9:16](https://www.youtube.com/watch?v=jivGzzgpHnM&t=556s)** >> For honor, as you wish.

**[9:18](https://www.youtube.com/watch?v=jivGzzgpHnM&t=558s)** >> Let's see. [music] >> I'll pass a recording for a while. This system there.

**[9:30](https://www.youtube.com/watch?v=jivGzzgpHnM&t=570s)** So here is the path of the Sherman full map in its entirely form.

**[9:37](https://www.youtube.com/watch?v=jivGzzgpHnM&t=577s)** So this is how I take took the picture.

**[9:47](https://www.youtube.com/watch?v=jivGzzgpHnM&t=587s)** You can probably see that there's many unfinished stones. I when I uh progress with terining and stuff or change something, I also then have to take a new screenshot and make it into smaller chunks and you know do the process again.

**[10:07](https://www.youtube.com/watch?v=jivGzzgpHnM&t=607s)** But now that I know the process, it's going to be lots of easier.

**[10:17](https://www.youtube.com/watch?v=jivGzzgpHnM&t=617s)** Let me just save this map as a folder so I can easily uh transfer the mini map files.

**[10:28](https://www.youtube.com/watch?v=jivGzzgpHnM&t=628s)** All right.

**[10:30](https://www.youtube.com/watch?v=jivGzzgpHnM&t=630s)** Now I have imported the mini map files and put the script inside the map trigger triggers and let's see how it works in the big brother map.

**[10:48](https://www.youtube.com/watch?v=jivGzzgpHnM&t=648s)** There could be could be multiple issues, but let's see. [snorts] H the map you now see is we can't really tell much about it because it's preloading states. It's basically loading sounds and music and [snorts] stuff like that.

**[11:19](https://www.youtube.com/watch?v=jivGzzgpHnM&t=679s)** >> [screaming] >> Okay, it kind of works.

**[11:35](https://www.youtube.com/watch?v=jivGzzgpHnM&t=695s)** The spirits are once again restless. I'm on the way. I see the elements are with there should be plenty of stags and frogs around the forest to create my flask.

**[11:49](https://www.youtube.com/watch?v=jivGzzgpHnM&t=709s)** >> Not quite right position.

**[11:50](https://www.youtube.com/watch?v=jivGzzgpHnM&t=710s)** >> Herbs will definitely be >> as water this water here and this water in the map. So it's not totally incorrect position. I wonder why some changes need to be done somewhere.

**[12:14](https://www.youtube.com/watch?v=jivGzzgpHnM&t=734s)** A wise plan >> [music] >> The elements are with me.

**[13:25](https://www.youtube.com/watch?v=jivGzzgpHnM&t=805s)** a wise plan.

**[13:31](https://www.youtube.com/watch?v=jivGzzgpHnM&t=811s)** >> Yeah, it definitely definitely could be a lot lots of better, but not too bad.

**[13:42](https://www.youtube.com/watch?v=jivGzzgpHnM&t=822s)** First thing I noticed that these mini map icons are shown here.

**[13:50](https://www.youtube.com/watch?v=jivGzzgpHnM&t=830s)** I don't know if that's good or bad.

**[13:53](https://www.youtube.com/watch?v=jivGzzgpHnM&t=833s)** Basically, it's giving indication where they are.

**[13:56](https://www.youtube.com/watch?v=jivGzzgpHnM&t=836s)** >> I see.

**[13:57](https://www.youtube.com/watch?v=jivGzzgpHnM&t=837s)** >> For example, this one is >> I'm on the way.

**[14:00](https://www.youtube.com/watch?v=jivGzzgpHnM&t=840s)** >> I should head this way or this travel location. I have to travel here.

**[14:07](https://www.youtube.com/watch?v=jivGzzgpHnM&t=847s)** >> I see.

**[14:08](https://www.youtube.com/watch?v=jivGzzgpHnM&t=848s)** >> So, I I don't know if that's good or bad.

**[14:15](https://www.youtube.com/watch?v=jivGzzgpHnM&t=855s)** Uh but this map could be clustered at the edges with this map. icons. So, it could become quite messy.

**[14:26](https://www.youtube.com/watch?v=jivGzzgpHnM&t=866s)** Don't know.

**[14:27](https://www.youtube.com/watch?v=jivGzzgpHnM&t=867s)** >> The elements are with me. I'm on the way.

**[14:31](https://www.youtube.com/watch?v=jivGzzgpHnM&t=871s)** >> A wise plan.

**[14:45](https://www.youtube.com/watch?v=jivGzzgpHnM&t=885s)** >> The elements are with me. I see a wise plan. I'm on the way. A wise plan. I'm on the way. I forgot that I do not have empty flask in my stack. I need to search for a place. That's >> I see the elements are with [groaning and screaming] I see. I'm on the way. A wise plan. I'm on the way. [clears throat] with me.

**[15:22](https://www.youtube.com/watch?v=jivGzzgpHnM&t=922s)** >> A wise plan, I see. I see. We're under attack.

**[15:27](https://www.youtube.com/watch?v=jivGzzgpHnM&t=927s)** >> A wise plan. The elements are with me.

**[15:31](https://www.youtube.com/watch?v=jivGzzgpHnM&t=931s)** The elements are wise plan. I see. I'm on the way.

**[15:42](https://www.youtube.com/watch?v=jivGzzgpHnM&t=942s)** Talk them. I'm on the way.

**[15:50](https://www.youtube.com/watch?v=jivGzzgpHnM&t=950s)** A wise plan, I say. The elements are with me.

**[16:05](https://www.youtube.com/watch?v=jivGzzgpHnM&t=965s)** >> The elements are with me.

**[16:11](https://www.youtube.com/watch?v=jivGzzgpHnM&t=971s)** >> Uh-huh.

**[16:13](https://www.youtube.com/watch?v=jivGzzgpHnM&t=973s)** Well, that's then that
