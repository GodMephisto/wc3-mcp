---
type: vendor-doc
package: video-sunfire-spawn
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=2fp61SdAgHs
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 4230
timestamped: yes
---

# WC: Reforged World Editor Tutorial (Part 3: terrain tip, basic dialog cinematic)

Source: <https://www.youtube.com/watch?v=2fp61SdAgHs>

Uploader: Sunfire Spawn | Duration: 70m30s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

In part 3 of this tutorial series we learn how to make a basic dialog cinematic and I tease a dedicated project I'm developing within Warcraft Reforged!

Elin'dil teaser with Reforged graphics: https://www.youtube.com/watch?v=QnKLul8FsTk

Music from Freesound.org: http://freesound.org/people/Migfus20/

## Transcript

**[0:01](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1s)** Hello, my fellow Warcraft content creators. Welcome back for part three of my Warcraft 3 world editor tutorial series, specifically for Reforged.

**[0:12](https://www.youtube.com/watch?v=2fp61SdAgHs&t=12s)** Okay, so couple of things are going to be different. Uh, I'm coming back to this channel with some different kinds of gaming content, but before I get to that stuff, uh, I wanted to, of course, contribute to this tutorial series again. And not only that, um I wanted to finally make something cool in this engine, uh you know, with this stuff.

**[0:33](https://www.youtube.com/watch?v=2fp61SdAgHs&t=33s)** So, I've actually been cooking for ah jeez, almost like four months straight. Four months straight. Actually working every day. Trust me, I got the uh I got the time lapses to back it up.

**[0:45](https://www.youtube.com/watch?v=2fp61SdAgHs&t=45s)** I've got a big open world project. I can't make any promises. I can't cuz they they could turn to sweet little lies in a moment. I can't make any promises about the mechanics of the game. However, at the end of this tutorial, if you're interested, I'm going to go ahead and showcase some of the terrain for you. But, uh, without further ado, that's kind of my olive branch for having taken so long to get

**[1:07](https://www.youtube.com/watch?v=2fp61SdAgHs&t=67s)** back to this. No, it will not take you 4 years to render these cinematics.

**[1:13](https://www.youtube.com/watch?v=2fp61SdAgHs&t=73s)** But, let's go ahead and let's get started. So, I'm going to go ahead and I'm going to give you guys a uh small uh easy advanced terrain tip that I think I meant to drop in one of the previous videos, but no matter, we have to start a new map here anyways, probably. So, anyways, uh so you're probably looking at a new map. Let's go ahead and let's hit close map. And if it asks you to save, don't. And then we're going to hit

**[1:39](https://www.youtube.com/watch?v=2fp61SdAgHs&t=99s)** new map.

**[1:41](https://www.youtube.com/watch?v=2fp61SdAgHs&t=101s)** Now, this is something that I do that's a little bit different uh from others.

**[1:45](https://www.youtube.com/watch?v=2fp61SdAgHs&t=105s)** It's It's not that I've never seen this done. It's just that I don't think people do it nearly as often as they should. Basically, change the width and the height to whatever you'd like.

**[1:54](https://www.youtube.com/watch?v=2fp61SdAgHs&t=114s)** Change the tile set to whatever you'd like. In my case, I'm going to go with Deran or Delaren, whatever you prefer. We're making it up anyway. It's fantasy. Come on. I mean, so the uh initial cliff level will by default always be two and we're going to keep it at that. But we are going to change the initial water level from none to deep water. And you'll see in a moment why I do this. Again, make the

**[2:23](https://www.youtube.com/watch?v=2fp61SdAgHs&t=143s)** map as big as you want. I'm just making it this big for the sake of, you know, performance and tutorial. So, all right.

**[2:30](https://www.youtube.com/watch?v=2fp61SdAgHs&t=150s)** Here we are. We got this big blank space of water. Why on earth would I want to boot my maps up like this? Well, it's so I can do this.

**[2:43](https://www.youtube.com/watch?v=2fp61SdAgHs&t=163s)** So, I'm using the raise tool in the terrain. And the circular motion I'm doing is no mistake either. I'm not trying to meme or sound pretentious, but it's the Fibonacci sequence. It's a natural thing that occurs in nature. So if you kind of in general when you're doing these hills start in the middle and spiral out you'll create these natural thresholds. And if you select your uh you'll create these natural

**[3:06](https://www.youtube.com/watch?v=2fp61SdAgHs&t=186s)** hills. And if you select your plateau tool you can sort of see how these rivers almost feel like they carve themselves out in a way through your terrain. And then you can select the largest brush and then go over this and you can smooth that out. And then we can uh get annoyed more.

**[3:27](https://www.youtube.com/watch?v=2fp61SdAgHs&t=207s)** I'm just kidding. Get rid of the get rid of the grid. And then we can take a look here. You've got Well, I mean, come on.

**[3:35](https://www.youtube.com/watch?v=2fp61SdAgHs&t=215s)** I think you can see I think you can grasp immediately why I do this. It's because it looks more realistic. Not only that, in an engine where we are working with so, you know, little precious player space, even with the largest map, you know, you're looking at a fraction the size generally of most world of most open world games today.

**[3:55](https://www.youtube.com/watch?v=2fp61SdAgHs&t=235s)** You know, the maps in this can't really get that big. So, uh, player space is important. It's really important. And when there is this, uh, pathing barrier. So, I've pressed P so you can see the pathing. Uh there is this frustrating issue where this always creates a pathing barrier.

**[4:14](https://www.youtube.com/watch?v=2fp61SdAgHs&t=254s)** And it's not that I don't want there to be walls that my units can't get over.

**[4:18](https://www.youtube.com/watch?v=2fp61SdAgHs&t=258s)** And it's not that I don't want there to be water that they can't get through.

**[4:21](https://www.youtube.com/watch?v=2fp61SdAgHs&t=261s)** It's that I want it to be where I decide. And the cliffs just don't let you choose that. And not only that, if you want units to be able to get over this, these ramps just don't.

**[4:33](https://www.youtube.com/watch?v=2fp61SdAgHs&t=273s)** They just don't. I I don't know. I don't know, man. I mean, look at this. you're creating a block of of unoccupiable space for every block of space that you're trying to create with these walls. It's just not great. It gets clustered at a certain point. So, I really prefer this method because your units can walk seamlessly in and out of the water. And if you're worried about the deep water, shallow water

**[5:03](https://www.youtube.com/watch?v=2fp61SdAgHs&t=303s)** functionality of the cliff types and getting the water that way, well, check this out. If I press W, I get rid of the water. You'll notice that the deep water is still impathable. There's a certain ratio. There's a certain point where it becomes impathable. And then if you want it to be impathable, but not that deep, you can just select plateau, select that pink spot, and then you can do this. So, if you still want this to

**[5:30](https://www.youtube.com/watch?v=2fp61SdAgHs&t=330s)** be a river divide, you know, you can you can still have it be. You'll notice I got rid of that unit.

**[5:41](https://www.youtube.com/watch?v=2fp61SdAgHs&t=341s)** So, it just creates a difference. It's a different way to do it. Not only that, you can pair it with this terrain method.

**[5:51](https://www.youtube.com/watch?v=2fp61SdAgHs&t=351s)** I mean, there's no reason that you can't just do both. I understand, you know, in in real life, you know, rocky shores, cliff uh you know, cliffs alongside oceans is not, you know, impossible.

**[6:04](https://www.youtube.com/watch?v=2fp61SdAgHs&t=364s)** So, you could just have both.

**[6:06](https://www.youtube.com/watch?v=2fp61SdAgHs&t=366s)** Now, there is one thing though. There is one issue that different cliff types and this terrain building method can cause.

**[6:14](https://www.youtube.com/watch?v=2fp61SdAgHs&t=374s)** Say that I build these and then I get rid of them and that happens. Okay.

**[6:20](https://www.youtube.com/watch?v=2fp61SdAgHs&t=380s)** Well, now I can't get rid of these.

**[6:22](https://www.youtube.com/watch?v=2fp61SdAgHs&t=382s)** What's going on?

**[6:24](https://www.youtube.com/watch?v=2fp61SdAgHs&t=384s)** There's another frustrating thing that happens when this occurs. If you try to put water back through here, that spot with that old cliff type will do this with the water. So, if this has been happening to you, here's the way that I fix it. I just copy paste a section at a time this water back in. Now, this is assuming you're willing to commit to this terrain method. Obviously, I commit to it. So, if something is junking it

**[6:59](https://www.youtube.com/watch?v=2fp61SdAgHs&t=419s)** up, then I'm going to go through and you can get larger sections and get rid of it.

**[7:11](https://www.youtube.com/watch?v=2fp61SdAgHs&t=431s)** And then just raise that back out of the out of the water like so.

**[7:17](https://www.youtube.com/watch?v=2fp61SdAgHs&t=437s)** And again, just try and be just try and have where the land is going to be be very defined to you. Try not to rework your bodies of water too much because that just that stuff just starts happening and it gets really really frustrating. But you'll notice it doesn't happen if you haven't gone back over with a new texture. The reason this is occurring is because a lot of the time when you select one of these

**[7:38](https://www.youtube.com/watch?v=2fp61SdAgHs&t=458s)** textures, you'll notice that it's saying tiled walls even though I'm doing just a flat level. So now if I go and I try and do this.

**[7:48](https://www.youtube.com/watch?v=2fp61SdAgHs&t=468s)** So it's really a cliff type confliction.

**[7:51](https://www.youtube.com/watch?v=2fp61SdAgHs&t=471s)** And the reason that the shape is so junked up is because it causes that cliff type confliction all around it as you see. So it clusters up and it deforms the shape. It's it's no good.

**[8:02](https://www.youtube.com/watch?v=2fp61SdAgHs&t=482s)** It's no fun. And when it's happening to you, you know, if you don't want to have to go pasting everything back through, you can just try raising the terrain up above the water.

**[8:14](https://www.youtube.com/watch?v=2fp61SdAgHs&t=494s)** and then just dealing with it in the spaces where you can manage it, right, without having to totally redo, say, this space that you've been developing. So, excuse me.

**[8:30](https://www.youtube.com/watch?v=2fp61SdAgHs&t=510s)** So, that's basically a new terrain building method for you um and a way to fix it. And not only that, like I said, this is truly truly the cause of this is the uh cliff type confliction, not my terrain method. So if it starts happening to you, that's a way to fix both, whichever whichever way you decide to you decide to create your maps. But so anyways, that's just a quick tip. But that's not what we're here for. We're

**[8:58](https://www.youtube.com/watch?v=2fp61SdAgHs&t=538s)** going to create a basic cinematic. So you can do things say like extended dialogue Bethesda style character RPG where you have multiple choices and things like that, but that stuff is kind of tough to script and I'm actually not very familiar with it myself. So I'm going to go ahead and learn it so that I can teach it to you in the next part.

**[9:18](https://www.youtube.com/watch?v=2fp61SdAgHs&t=558s)** But for this part, I'm just going to teach you how to make a basic cinematic where two units are kind of talking to each other.

**[9:26](https://www.youtube.com/watch?v=2fp61SdAgHs&t=566s)** So, let's get a couple of custom units really quick.

**[10:01](https://www.youtube.com/watch?v=2fp61SdAgHs&t=601s)** Okay. So, right here, we're going to go ahead and we're going to make it so that this guy, no matter which unit he is owned by, he's green. And I'm going to change his tint while we're at it so that he's really green.

**[10:19](https://www.youtube.com/watch?v=2fp61SdAgHs&t=619s)** We're going to place this unit right here.

**[10:22](https://www.youtube.com/watch?v=2fp61SdAgHs&t=622s)** He's going to be looking out to the ocean.

**[10:25](https://www.youtube.com/watch?v=2fp61SdAgHs&t=625s)** And he loves green. So, we're going to put some grass around him.

**[10:31](https://www.youtube.com/watch?v=2fp61SdAgHs&t=631s)** Maybe a little bit of an oasis going on with this uh this desert aisle. And over here, we're going to put some purple tile.

**[10:40](https://www.youtube.com/watch?v=2fp61SdAgHs&t=640s)** And I think you know where I'm going with this.

**[10:44](https://www.youtube.com/watch?v=2fp61SdAgHs&t=644s)** We're going to put purple mage, but we're going to actually make him purple.

**[11:06](https://www.youtube.com/watch?v=2fp61SdAgHs&t=666s)** Okay, so we've got our purple mage and we've got our green mage.

**[11:15](https://www.youtube.com/watch?v=2fp61SdAgHs&t=675s)** I think I'll maybe do this for the purple mage side.

**[11:30](https://www.youtube.com/watch?v=2fp61SdAgHs&t=690s)** Just adding some general props.

**[11:34](https://www.youtube.com/watch?v=2fp61SdAgHs&t=694s)** And just in case you're wondering what's up with the uh why is it showing old and new? Well, it's not supposed to be showing any of the new models, but I have it set to the old models for the sake of performance. And so, it's just this is I don't know what it's doing. I don't know. I don't know. But I am going to show you the cinematic and reforge graphics. Don't worry about that. So, I'm just placing these so that there's I

**[11:55](https://www.youtube.com/watch?v=2fp61SdAgHs&t=715s)** don't know vaguely generally purple stuff over there. You know, maybe we can get some uh where's like uh where's that uh Yeah, why not? There we go.

**[12:23](https://www.youtube.com/watch?v=2fp61SdAgHs&t=743s)** Okay. And then on this side, we're going to give him a whole bunch of green trees cuz my man's all about green. He loves green. That's what's up.

**[12:36](https://www.youtube.com/watch?v=2fp61SdAgHs&t=756s)** And he's going to be looking out on a vast green country with a canyon through it.

**[12:53](https://www.youtube.com/watch?v=2fp61SdAgHs&t=773s)** Now, one of the other neat things about the ability um provided by this terrain method is you can stack to the maximum ceiling since you set it to a cliff type of two. That's an initial level, but then you set it to deep water. It technically starts at zero. So any terrain that you're developing up from that deep water is from the base level.

**[13:14](https://www.youtube.com/watch?v=2fp61SdAgHs&t=794s)** I like to just call that the ocean level. And I think you can see why. But anyways, I'm going to really put this up and put some mountains up behind it.

**[13:34](https://www.youtube.com/watch?v=2fp61SdAgHs&t=814s)** And given that we've already gone over some basic terrain methods, I'm going to take this opportunity to go over another slightly more advanced one with some warnings about it as well.

**[13:55](https://www.youtube.com/watch?v=2fp61SdAgHs&t=835s)** So, excuse me. We're going to talk about custom terrain. So, if you go into advanced, you'll find modify tile set.

**[14:08](https://www.youtube.com/watch?v=2fp61SdAgHs&t=848s)** Now, the warning I'm giving you is to be careful about basically tiles that have the same name as ones that you already have. They are differentiated by an ID. But I've still seen it glitch out even in that case.

**[14:23](https://www.youtube.com/watch?v=2fp61SdAgHs&t=863s)** But the main warning is going to come with if you have two cliff types that are named the same because say I want to add snow to this for these mountains.

**[14:34](https://www.youtube.com/watch?v=2fp61SdAgHs&t=874s)** So, I'm going to go with Lorderon winter. Well, check this out. You see this cliff texture space used says two.

**[14:42](https://www.youtube.com/watch?v=2fp61SdAgHs&t=882s)** Well, if I add snow, you'll notice that now there's a third cliff type. So, this is okay. It's adding the snow cliff type. And I actually do want that for these mountains. But say that I tried to add, you know, um, cityscapes square tiles.

**[15:04](https://www.youtube.com/watch?v=2fp61SdAgHs&t=904s)** You'll notice that that goes up to four.

**[15:06](https://www.youtube.com/watch?v=2fp61SdAgHs&t=906s)** So watch this cliff texture space because guess what?

**[15:15](https://www.youtube.com/watch?v=2fp61SdAgHs&t=915s)** These will conflict with each other.

**[15:23](https://www.youtube.com/watch?v=2fp61SdAgHs&t=923s)** Did you see it?

**[15:37](https://www.youtube.com/watch?v=2fp61SdAgHs&t=937s)** There we go. See that?

**[15:41](https://www.youtube.com/watch?v=2fp61SdAgHs&t=941s)** That will just happen to you and you'll be like, uh, and it's like happening down here, too. And it's like, it totally screws it up. It junks it up.

**[15:50](https://www.youtube.com/watch?v=2fp61SdAgHs&t=950s)** It's no good. So, that's why I give you that warning. Watch out for cliff types that are named the same as another cliff type because look, now you can't get rid of it. Look at that.

**[16:03](https://www.youtube.com/watch?v=2fp61SdAgHs&t=963s)** And I'm just kidding. You actually can get rid of it. You can get rid of it the exact same way that I showed you how to get rid of the the other stuff. You'll notice the cliff types bug out, but you just reset it to water. That's what I do.

**[16:39](https://www.youtube.com/watch?v=2fp61SdAgHs&t=999s)** So, it's very frustrating. And in some cases, if it's messing with a building or a city or something you've already put a lot of work into, you may have to get down to the block byblock level where you just do it one by one. But, it's possible to be precise about it.

**[16:55](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1015s)** But, so in this case, I'm just going to leave this. I'm going to take some sections of plateau here. Kind of create like some shallow waters and stuff like that down here.

**[17:09](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1029s)** I'm going to go into dooddads. I'm going to go into environment.

**[17:14](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1034s)** I'm going to add some plant life.

**[17:20](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1040s)** I'm going to go into village cuz village has this river rushes and this can be nice to use if you're not feeling like customizing a whole bunch right now just cuz it has these variations and you can just put that in water.

**[17:35](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1055s)** Then we'll add some rocks and some more shrubs.

**[17:50](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1070s)** Then we'll add trees.

**[18:04](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1084s)** And what's cool about these these rock chunks, uh, you can actually, these are targetable since they're placeable out of the ch uh, excuse me, what did I just say? Placeable out of the, uh, trees and destructibles. Uh, these are actually targetable rocks and you can actually they do this slant that can actually really be useful in a cinematic context, but you'll have to make them non-targetable. Otherwise,

**[18:27](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1107s)** when you start to play the game, they'll just be kind of frustrating to deal with. You know, you'll be trying to click on an area for your mun uh for your unit to move and they'll be constantly going to break these big rocks. But I'm going to use them here anyways.

**[18:51](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1131s)** And I'm also going to use some waterfall effects.

**[19:11](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1151s)** So, I'm going to plateau this area that the waterfall is coming from and kind of dig it in like this.

**[19:34](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1174s)** Again, I'm not trying to make it look its absolute best or perfect or anything.

**[19:38](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1178s)** Uh you can use page up and page down to change the size of the waterfalls.

**[19:43](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1183s)** If you're not happy with the default that it has set, you can open the object editor. You can go into doodads, go to water, find the waterfall that you're using, and right here you'll see editor minimum and maximum scale. Personally, when I'm making stuff these days, I tend to just get to a point where I by default set it to five and 0.01. So, I can make it whatever size I decide. So, now we can reselect this. And you see it

**[20:16](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1216s)** can be all these different kinds of sizes, right? So, that's my max. That's five.

**[20:24](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1224s)** And what's cool about the the 0.01, 01.

**[20:30](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1230s)** You can flatten these out and you can actually wind up saving some performance space using a ton of waterfalls and rivers and stuff and instead just use these effects. I think you even see kind of Blizzard doing that themselves, you know, in various cases.

**[20:55](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1255s)** Okay, so we got a kind of jumbled looking green landscape over here that this dude is clearly all about. Let's back him up a little bit.

**[21:05](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1265s)** Let's give him some trim grass here.

**[21:16](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1276s)** Cuz he's just all about that green, baby. He's all about it.

**[21:25](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1285s)** Oh, I don't want these floating lily pads in the in the grass over there. Oh, you know what? While we're here, if you want to simulate wind, you actually can take some of these water uh environmental things like say like uh where's like their seaweed? See, if you just place it on land, it just kind of looks like a windy grass object. So, you could just use that for that. By the way, I've had that idea.

**[21:56](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1316s)** Let's throw some of this seaweed down here. And then let's also go into cinematic.

**[22:03](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1323s)** No, excuse me. Let's go into environment.

**[22:07](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1327s)** Not even that. Why are birds Okay, birds are props. Yeah, birds are cameras. So, birds are props. Okay. I was looking for birds. All right.

**[22:24](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1344s)** Okay. We're going to put a fire pit right here. So, th this is this is too the these two magi are strong. You know, they're stuck on this aisle. Uh, one of them really does not like the color green. And he's going to let you know it. He's definitely going to let you know that. Let's go ahead and let's make these snowy mountains over here. I was going to make with this custom tile set.

**[22:51](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1371s)** It's a bit much.

**[22:55](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1375s)** So, you'll see I can stack these crazy high due to uh this terrain method and having started down at deep water.

**[23:09](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1389s)** So you can stack them pretty high. And I personally I like to just kind of jumble them a little bit. You know, mountains are mountains are jumbled.

**[23:20](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1400s)** Put that down a little bit. And if this uh crazy top looks like too much to you, you can smooth it out this way.

**[23:29](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1409s)** Maybe reduce some of these props now since they've been lifted up high out of the water.

**[23:48](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1428s)** And then let's add a region to the snowy area.

**[23:58](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1438s)** We'll call this region snow.

**[24:02](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1442s)** And for this one, I'm going to add a weather effect of snow.

**[24:10](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1450s)** So, it's snowing over there.

**[24:13](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1453s)** I mean, we are trying to make a cinematic, aren't we?

**[24:19](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1459s)** All right. So, let's go ahead and let's also set our default sky.

**[24:24](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1464s)** This is also where you can change other map properties. So, if you're tired of your maps being just another Warcraft 3 map, this is actually where you change that. So, we'll say Sunfire Take Two, cuz this is actually my second time trying to make this video. All right. And for the description, we're going to go ahead and we're going to say a cinematic tutorial for YouTube.

**[24:51](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1491s)** And the author, we're going to change it to me.

**[25:00](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1500s)** Okay. This is also where you can say set a global weather environment. If you want it to be constantly raining throughout the entire map space, if you want it to be windy, foggy. I think in this case, I'm going to go ahead and I'm going to put a constant light wind. Let's see how this looks.

**[25:21](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1521s)** Yeah, I like that.

**[25:23](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1523s)** That's all right.

**[25:25](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1525s)** We're just going to have that be there basically the whole time. Okay.

**[25:34](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1534s)** As you go down the line, you'll notice that there's these other options and things that you can change. I don't want to get too much into this, but uh you can also make it so that if you only use the standard definition models or if you only use the HD models, uh you can set it to be classified that way, which it will then flag it with a warning when somebody is booting that up in the custom game menu. Uh we're going to go

**[25:56](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1556s)** ahead and keep this open. It's going to be for everybody. We're going to go to loading screen. And here you can change a couple of other things. if you don't want that, you know, test bootup screen anymore. You have access to all of the loading screens as they are in here.

**[26:10](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1570s)** We'll go ahead and we'll just pick a random one. I'll just go with this undead one. And I'll say Sunfire spawns tutorial.

**[26:21](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1581s)** And the subtitle will be for the YouTubes.

**[26:30](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1590s)** And then there would be a description.

**[26:32](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1592s)** Now, ordinarily, if you were playing a, you know, actual uh excuse me, if you were playing an actual Blizzard produced uh campaign, uh the way it would read is basically between levels and stuff, you would have this description that would be like Arthus now knowing that he can find the blade goes to Northrand and it would show the map and have that whole like description, right? So, this is an opportunity to write any anything you

**[26:58](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1618s)** want, any kind of extended description, but for in this case, I'm just going to keep it simple and I'm going to say this is a generic fantasy intermission description.

**[27:13](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1633s)** Lol. Okay, so that's how you can customize the map name, all that kind of stuff. You'll notice I haven't even saved it yet. So here it will show up as untitled one, but even in Warcraft it will now show up as whatever I titled it as. So without further ado, let's go into uh the map preferences again because there was someone uh one thing I wanted to change in here. You can change the sky here. So I'm going to

**[27:41](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1661s)** set it to fall sky.

**[27:44](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1664s)** So there you go. Now it will automatically default back to this skybox whenever um a cinematic is over because you can change the skybox in cinematics too.

**[28:01](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1681s)** Excuse me, didn't mean to open that.

**[28:06](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1686s)** Uh you can also change the time of day while you're in here. So you can make it say fix to be, you know, midnight, dawn, whatever. So, in this case, let's go ahead and let's just say fixed at midnight, so it's always nighttime.

**[28:21](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1701s)** So, we've got quite the scene going on here.

**[28:29](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1709s)** I think it's about time that we start programming this cinematic.

**[28:33](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1713s)** So, what's up with this uh this red mage? Why do I got this red mage? Well, this red mage is here so that I can give you a ingame way of starting a cinematic. So, we're going to program one that starts right when the game starts. And then we're going to program one that starts if this unit dies. And as a matter of fact, we're going to want to make killing him very, very easy so that he goes down quick. So, I'm going

**[29:01](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1741s)** to go into stats and we're going to go to hit points maximum. We're just going to give him one hit point. So, he'll die very, very fast. Even though this is technically a support class, I don't have to go tweak their damage now. Okay.

**[29:16](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1756s)** So, it's the final boss of the trigger editor. It's the final boss of Warcraft 3 world editor in general. It's this.

**[29:28](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1768s)** It's the melee initialization. How do we beat this final boss?

**[29:32](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1772s)** We delete it. And then we create a new action. And this one is going to be intro cinematic.

**[29:40](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1780s)** So this one will start on map initialization which is default. So that's going to be the event that starts the cinematic.

**[29:48](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1788s)** We're going to have it turn on cinematic mode.

**[29:58](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1798s)** And then we are going to apply camera.

**[30:02](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1802s)** Apply camera object for player one red.

**[30:04](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1804s)** Select a camera. We haven't created one yet. So, we're going to go ahead and we're going to start.

**[30:15](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1815s)** We'll start here.

**[30:21](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1821s)** Let me get rid of some of these trees.

**[30:23](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1823s)** Move them over a little bit.

**[30:26](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1826s)** Clogging up my scene.

**[30:29](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1829s)** There we go. Okay.

**[30:35](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1835s)** go ahead and create a camera here.

**[30:39](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1839s)** And I want it to do a bit of a zoom while I'm while while it's doing this intro. So, I'm going to go ahead and copy this and paste it. Double click it.

**[30:48](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1848s)** I'm going to open up the camera. Select a camera. As I go through the workflow, this is just how I create cameras. So, we're going to have it kind of do this.

**[30:57](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1857s)** It's going to move in and kind of arc up a little bit. So, we're going to hit create camera. Then, we're going to have it pan to that other camera over the course of 5 seconds.

**[31:09](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1869s)** We're also going to program a fade in.

**[31:12](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1872s)** So, you'll want to go to cinematic fade filter.

**[31:18](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1878s)** Now, if you do fade out and back in, basically this is always going to set it to it will be at its peak fade out at half of whatever the time that you set here. So that's how that one works. If you want to have a fade in or a fade out that are isolated and that have separate timing, then you want to do this. You want to go in here and you want to hit fade in. So this is just going to start it black, right? Well, actually

**[31:44](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1904s)** technically white, but we're going to set it to black. So it's just going to start it black and then it's going to fade in over those 5 seconds. So we'll kind of have this camera be set. This fade in will start. The cinematic mode will begin. Now, we want a wait action that's going to be 5 seconds so that the camera has time to do that. We've got time for the fade in to happen and all that stuff. But here's another thing.

**[32:09](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1929s)** Even though we have set a default sky, I want the skybox to change for this particular cinematic. So, I'm going to go to environment, set sky, set sky to, and we're going to change it to fogged sky.

**[32:28](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1948s)** So, when the game starts, it's going to run this cinematic. It's going to change the sky from what I've set it to now to this skybox. And then it's going to start fading in and panning over here.

**[32:37](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1957s)** But I wanted to do one more thing.

**[32:43](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1963s)** We're also going to create a new weather effect. It's just going to start raining. So, it's raining now.

**[32:53](https://www.youtube.com/watch?v=2fp61SdAgHs&t=1973s)** Now that we've panned in, I want one of these units to talk. So, we're going to go to cinematic transmission from unit. So, this is going to send transmission to all players from unit named name. So, this is a little frustrating. You do have a name plate in the uh classic graphic style, but if you go to the Reforge uh graphic style, it's only going to show the name that you enter here. So, just make it say whatever you want it to. If

**[33:23](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2003s)** you want it to match the unit that it is, go ahead and do that. But I'd also like to remind you of say Lord of the Rings. Nobody at the beginning, at least among the hobbits, is supposed to know that Aragorn is Aragorn. They know him as Strider or Ranger or whatever. So this way you could still say name a unit that's potentially a big spoiler character and you could say mysterious stranger. So in that case, let's go

**[33:52](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2032s)** ahead and let's do this. So we're going to go to variable, but you'll notice we don't have one yet. So we're going to select a unit. In this case, it's going to be Mr. Purple, the purple mage. And he is definitely not purple. Okay, he's not.

**[34:14](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2054s)** And it's going to play no sound. Now, I don't want to get too branched off here, but you can have it play different sound effects. Um, those kinds of things. You know, old school RPGs used to kind of have this thing where when dialogue would appear, you'd almost hear a character just go like and they would just make some noise and then you would just see text like, you know, maybe old school Nintendo stuff, you know. Um, so

**[34:40](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2080s)** you could sort of get away with that with in this, but it's also a way that you could through the variables provide voice dialogue and even your own music, but again, that's outside the scope of this video. So that's I'm just trying to tell you what the sound function does and how you could theoretically use it.

**[34:56](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2096s)** In this case, we're just going to go with text. So, purple mage named definitely not purple is going to say, you know, and then we're going to have this modify duration add how many seconds. So, don't worry about add. Don't customize that for the time being. We're just going to say 3 seconds. So, that's how long this message is going to appear after that wait timer. So, let's go ahead and let's add a wait timer down here. That's going

**[35:27](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2127s)** to be just slightly longer than that dialogue.

**[35:32](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2132s)** I do this because it tends to just kind of glitch out a little bit less.

**[35:38](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2138s)** It would seem if I program it this way.

**[35:44](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2144s)** So, he's going to say, you know, and we're going to wait for a few seconds, and then it's going to cut to a new camera angle. And I want it to cut, not fade. So, we're going to keep that at apply that camera over 0 seconds.

**[36:02](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2162s)** And we're going to cut to this angle.

**[36:08](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2168s)** I'm going to copy and paste this transmission so that I can just open it up. And it's the same name. It's the same Well, it's the same unit, same name. And then I can just change the text and the time.

**[36:21](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2181s)** and he's gonna say, "I hate green." And it's going to be two seconds.

**[36:33](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2193s)** So, we're going to put one in that waits for two seconds.

**[36:38](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2198s)** And I'm not changing the camera angle, so that's why. And then there's going to be a new dialogue from him right here.

**[36:48](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2208s)** Actually, you know what?

**[36:54](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2214s)** Now, I do want to pause here. So, we're going to go with 2.10.

**[37:03](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2223s)** Copy the camera object and and paste the camera.

**[37:11](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2231s)** And now we're going to cut to this wide shot like this.

**[37:20](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2240s)** And I want the purple one to turn around and look at the green one.

**[37:28](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2248s)** So, we're going to go into unit, make unit, face unit.

**[37:38](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2258s)** So, we're going to make purple mage face. Select a unit. green mage uh over about one second. So, he'll do it kind of quick. So, he's going to turn around and he's going to look at the green mage.

**[38:00](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2280s)** And after one second, we're gonna get a dialogue from him that says, "I hate everything, every thing that's green.

**[38:32](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2312s)** And then it's going to cut closer on his face.

**[38:48](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2328s)** And he's going to say And I hate you.

**[39:01](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2341s)** I don't, by the way. He does. I don't.

**[39:07](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2347s)** And then we're just going to cut to a wide shot while he's staring at the green mage and all the green stuff.

**[39:26](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2366s)** And we're going to wait for one second.

**[39:31](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2371s)** And then we're going to have the green mage turn and look at the purple mage even slower.

**[39:50](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2390s)** So, we're going to have green mage face purple mage over the course of 4 seconds, and we're going to wait for those 4 seconds.

**[40:05](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2405s)** And then we're going to cut back in on the mage, the green mage.

**[40:13](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2413s)** We're going to cut in close on him.

**[40:19](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2419s)** with the fire like that. No, that that's good. That's good for what's coming.

**[40:33](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2433s)** And then the green mage is going to give his first line of dialogue. So, we'll copy this transmission, paste it, but we got to change all this stuff.

**[40:43](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2443s)** So now it's going to be coming from the green mage and he's not ashamed of his identity.

**[40:51](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2451s)** He's not hiding anything. So he's just green mage.

**[40:58](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2458s)** And he's going to say, "Are you seriously doing this right now?" over the course of 3 seconds. So again, with dialogues, you want just kind of like a 10 seconds for the dialogue to end before you cut. It just helps it.

**[41:21](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2481s)** Otherwise, you'll have dialogue windows that tend to kind of bleed over into the next one, into the next shot. So after that, we're going to cut back to the purple mage.

**[41:48](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2508s)** And he's just going to say, so we'll copy purple mage one. He's just going to say, I'm not scared of you.

**[42:05](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2525s)** And we're going to wait 2.10 seconds.

**[42:16](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2536s)** And then we're going to cut back to a wide shot.

**[42:30](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2550s)** You know, the two of them staring at each other.

**[42:38](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2558s)** And I want it to be doing a slight zoom.

**[42:42](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2562s)** So I'm going to copy and paste this.

**[42:45](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2565s)** Double click this. Select a new camera.

**[42:48](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2568s)** We're going to be zooming in slightly.

**[42:52](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2572s)** I'm going to make it do that over the course of about 6 seconds.

**[42:57](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2577s)** So we're going to wait for 6 seconds.

**[43:11](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2591s)** And then we're just going to cut back to the purple mage again.

**[43:25](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2605s)** And he's going to be like What are you doing?

**[43:44](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2624s)** And I want it to be kind of quick.

**[43:47](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2627s)** We're getting kind of slapsticky here.

**[43:52](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2632s)** So, he's going to say, "What are you doing?" It's only going to last for like 1 second. So, we'll make this last for 1.1.

**[44:05](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2645s)** And then we're going to cut back to that same pan again.

**[44:15](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2655s)** And this time, the camera is going to be shaking.

**[44:22](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2662s)** So, you'll want to go to cinematic.

**[44:29](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2669s)** Or actually it might be in camera.

**[44:33](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2673s)** Yeah. Okay. Excuse me. So it's going to be in camera. Shake camera for player one red with a magnitude. I don't want it to be that ridiculous. We'll say one.

**[44:45](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2685s)** So this is a way, by the way, that you can I think you can see lots of different ways that you can use this.

**[44:51](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2691s)** It's shaky cam. You know, you could apply this to a shot that's showing horses running. You could apply it to uh a battle scene. You could apply it to a volcano erupting. You could apply it to a city being destroyed, a war battle, you know, earthquake, all kinds of stuff, you know. So, but in this context, we're just going to use it for representing whatever kind of weird overcharged power he seems to be cooking

**[45:12](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2712s)** up there. Now we're going to cut back again to the purple guy and he's just going to say you can't.

**[45:33](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2733s)** It's going to wait for 1.10 10 seconds and then we're going to cut back to that same panning and 6second shot again, but this time the magnitude is going to be nuts.

**[45:46](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2746s)** It's it it's going to be we're going to make we're going to make it a four. So now the camera is really shaking and then we're just going to cut to a steady wide shot. So I'm going to make a new camera for this.

**[46:10](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2770s)** We're just going to cut to this.

**[46:18](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2778s)** And after just 1 second, 1.10 10 seconds.

**[46:27](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2787s)** The purple one is is going to uh he's going to explode.

**[46:36](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2796s)** Uh I guess I should give a uh a warning here. It may be a little gruesome. So, he's just going to explode and we're just going to see that. Okay.

**[46:46](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2806s)** and then the cinematic is just gonna end.

**[46:58](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2818s)** So, I'd like to remind you if you uh say you have something like a uh camera action that is locking your uh your camera to one unit or anything like that, you will have to resume that action here. Uh, given that I'm just using the the standard in-game camera with without locking it to any units, I'm just going to let it shut off because I can just scroll and then it will reset the camera to the standard

**[47:22](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2842s)** camera. So, it should just end the cinematic here basically and then it will resume control over my unit. So, there is one other thing I think I'm going to add here just for fun. So when he starts casting this thing whenever when it starts doing this zoom let's change the skybox.

**[47:46](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2866s)** So set sky and let's go ahead and let's set it to the outland sky.

**[47:54](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2874s)** And then when the cinematic ends it should just set it back to our default sky. But for the moment, for the for the last moments of this cinematic, that's kind of what I wanted to do. So, it's like he's calling upon some kind of real cataclysmic forces that is way beyond this purple mage's understanding. You have no idea. I mean, you just you should not have been trashing green.

**[48:16](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2896s)** Trashing green to the wrong mage, dude.

**[48:18](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2898s)** Wrong mage. Okay. So, after that happens, let's go ahead and let's uh let's get a second cinematic that can be going on, right? Because I did tell you I wanted to make one that's going to be based on map initialization.

**[48:35](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2915s)** So, when the game starts, but then let's have one that's based on a different event. So, let's have maybe a follow-up cinematic to this. How are we going to accomplish this? So, here's the deal.

**[48:46](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2926s)** Who's this red guy? Why did I put him here? Well, this is why. First, at the beginning of this action, let's go and let's make it hide this unit. So, unit hide.

**[49:03](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2943s)** Select a unit, red mage.

**[49:07](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2947s)** So, the reason I'm doing this is so that I can create a trigger that is not activated until the end of this cinematic. Basically, it's going to go like this.

**[49:22](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2962s)** Excuse me.

**[49:24](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2964s)** New action follow-up cinematic events unit dies. So, in the event that the red mage dies, we're going to start a new cinematic.

**[49:48](https://www.youtube.com/watch?v=2fp61SdAgHs&t=2988s)** So, cinematic mode, apply camera.

**[50:00](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3000s)** Select camera.

**[50:08](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3008s)** So, if he dies, it's going to apply this camera instantly and we're going to get, excuse me, new action. We're going to get a line of dialogue.

**[50:29](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3029s)** So transmission from unit from the green mage named actually hates red.

**[50:45](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3045s)** And the green mage is going to say finally.

**[50:52](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3052s)** And he's going to that message is going to last for only about 1 second.

**[50:58](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3058s)** So, we're then going to put a weight action for 1.10.

**[51:11](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3071s)** And then we're going to get another transmission from him.

**[51:16](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3076s)** It's going to say, "The two most atrocious the two most atrocious colors.

**[51:37](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3097s)** And that's going to go for 3 seconds.

**[51:41](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3101s)** So, we'll set this to a 310.

**[51:52](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3112s)** And then we're going to get a cut.

**[52:01](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3121s)** And I want it to be this kind of overhead shot like it's like it's almost like a grand cosmic thing at this point.

**[52:12](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3132s)** I'm going to copy this one since it's shorter.

**[52:16](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3136s)** And he's just going to say gone at last.

**[52:27](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3147s)** And then we will cut back down to him.

**[52:46](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3166s)** We'll wait a second.

**[52:49](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3169s)** And he's going to say, "I see a future that is only green." That will last for about 2 seconds. So, I'll make this 2.10 so the dialogue has time to finish.

**[53:18](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3198s)** And then this cinematic will end.

**[53:29](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3209s)** And when it does, it's going to turn this guy green.

**[53:43](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3223s)** So, we've got we've got a permanent world change going on here, too. This is this is a player choice now. Okay. So, at the end of this cinematic, we need it to unhide that unit. So, it hides him here at the beginning of this, but I want it to unhide this unit when this cinematic ends.

**[54:08](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3248s)** So, we'll hit unhide red mage.

**[54:14](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3254s)** So, he'll show up. And with him showing up, killing him now gives us access to this cinematic.

**[54:22](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3262s)** Let's go give it a test.

**[54:26](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3266s)** So, saving my map. Don't worry about any of these warnings. We're going to call it 2.0 you know, YouTube test cuz like I said, I already tried to make this earlier. Now, we're going to go ahead and we're going to hit that test function. So, I'm going to let you know right now, um, if the test isn't working, just try booting up your map in Warcraft Reforged itself. So save the game and then close the world

**[54:54](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3294s)** editor, open Reforged, find your map and then open it that way and see if it really is, you know, just a game crashing thing or if it if it boots up fine that way. It should boot up fine that way. And also then you can view it with the Reforge graphics. So here we're going to run the test function. If it boots up, it should show it first with these models and the these effects and stuff. And then after that, I'll go boot

**[55:19](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3319s)** it up in game and we can see it with the reforged models. All right. So, let's give this a shot.

**[55:27](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3327s)** All right. So, you can see here it chose the loading screen that I wanted. It says Sunfire Spawns tutorial for the YouTubes. This is a generic fantasy intermission description. Lol. So, I'll go ahead and click and it should start this cinematic.

**[55:48](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3348s)** You know, and he should turn to face him.

**[55:58](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3358s)** I hate green.

**[56:04](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3364s)** Here he goes. He looks at him. I hate everything that's green.

**[56:12](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3372s)** And I hate you.

**[56:20](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3380s)** So now the green slowly turns and looks at him.

**[56:27](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3387s)** Are you seriously doing this right now?

**[56:34](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3394s)** I'm not scared of you.

**[56:41](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3401s)** Okay, now we get the zoom.

**[56:48](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3408s)** What are you doing?

**[56:54](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3414s)** More intense zoom. Sky is red.

**[57:00](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3420s)** You can't.

**[57:07](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3427s)** Oh god.

**[57:10](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3430s)** Yes, he can.

**[57:13](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3433s)** And now.

**[57:19](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3439s)** So, it looks like I forgot to put in a stop shaking. So, let's go do that really quick.

**[57:33](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3453s)** So, back in here for the intro cinematic. Let's go and let's find this.

**[57:37](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3457s)** So, we're going to have the Where's the shaky cam? Where does it start?

**[57:43](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3463s)** Here we go. Shake the camera. Shake the camera. And then right here, right before the unit explodes, let's let's let's have then let's let's have the camera stop shaking then.

**[57:56](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3476s)** So, we'll say camera stop swaying and shaking for player one red. All right. Now let's jump back to that cinematic.

**[58:09](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3489s)** Okay, so we are resuming this, you know.

**[58:19](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3499s)** So he's going to say, I hate green.

**[58:26](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3506s)** Turns and looks at him. I hate everything that's green.

**[58:32](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3512s)** We got these birds flying around in the scene. And I hate you.

**[58:43](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3523s)** Okay, green.

**[58:45](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3525s)** The menacing slow turn.

**[58:48](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3528s)** Are you seriously doing this right now?

**[58:52](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3532s)** Like, we're stuck on this island. You You can't hang with a color that I'm not scared of you.

**[59:00](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3540s)** Okay, so now I should do my shaky cam.

**[59:03](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3543s)** Excuse me.

**[59:08](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3548s)** I love that this music starts.

**[59:14](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3554s)** What are you doing?

**[59:16](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3556s)** Oh god, you can't.

**[59:28](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3568s)** Okay. Wait, wait, wait.

**[59:41](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3581s)** Okay, so there we go. If I scroll in and out, you'll see it resumes to camera function. You'll see that the red guy is now visible and he's a bloody paste over here. So, let's kill the red guy finally.

**[59:59](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3599s)** And you'll see his name switches to actually hates red. The two most atrocious colors.

**[1:00:10](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3610s)** Gone at last.

**[1:00:18](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3618s)** I see future that is only green.

**[1:00:25](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3625s)** Now that's So now the sky is green and you'll notice it didn't switch it back. So it didn't go back to the default color. So it will maintain that skybox whatever you set it to for the rest of the game until you change it back. And obviously it won't stop shaking the camera until you set that to turn off as well. So if you want to see how some of this terrain looks as you're moving through it and stuff, say like if you go up to the high

**[1:00:53](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3653s)** cliff, you will be able to see sort of the skybox edges. That's if you're using default uh camera functions. But aside from that, using this terrain method honestly just looks great. There's really no reason that you shouldn't do it. It's pathable. We can also break these rock chunks. Like I said, I think it'll take him forever, though.

**[1:01:12](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3672s)** He doesn't have very high base damage.

**[1:01:18](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3678s)** He'll be doing this all day.

**[1:01:21](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3681s)** Yeah. But so it's pathable, like I said.

**[1:01:25](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3685s)** So, I hope this was helpful to you guys.

**[1:01:27](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3687s)** It showed you how to do a lot of different dynamic things. I gave you this uh sort of uh helpful uh advanced terrain tip, just uh more realistic look maybe to the water and the rivers and stuff. So, let's give this one more sort of eye candy thing. And let's go ahead and let's boot this up in the Warcraft 3 Reforged game itself and see what all of this looks like with the updated graphics.

**[1:01:54](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3714s)** Okay, so here we are. This is the Warcraft 3 Reforged game. Let me make sure that it's actually set to my Reforged graphics because it likes to Yep. See, likes to do that to me. So, I'll go ahead and set that and save it.

**[1:02:08](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3728s)** We'll go into the custom games menu.

**[1:02:14](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3734s)** Never mind that. I wouldn't worry about that. That's nothing important at all.

**[1:02:18](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3738s)** And let's go see. You'll see. Yeah, my YouTube rules. That's why I had to redo this. So, we've got Sunfire Take 2, a cinematic tutorial for YouTube. So, we'll go ahead and we'll create a game here. So, it's just us. You'll see it's got that basic description. You can see that this is our map. We've got the snowy mountains over there, the water here, and we've got the uh the purple platform, all that stuff. So, let's go

**[1:02:42](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3762s)** ahead and let's boot this up. Let's see what it looks like in the actual game itself.

**[1:02:50](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3770s)** So you can see it's chosen this this uh Tearsfall Glades map. This is a generic fantasy intermission description. LOL.

**[1:02:57](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3777s)** So all this stuff boots up the same.

**[1:03:00](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3780s)** Let's see what it looks like.

**[1:03:19](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3799s)** I hate green. And he flips his hair. Re remastered graphics as sass. Oh, okay.

**[1:03:25](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3805s)** All right. So, see, this is a problem with the difference between the uh the graphics. I couldn't see that that tree was going to be cutting in. There we go.

**[1:03:33](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3813s)** And I hate you.

**[1:03:45](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3825s)** Are you seriously doing this right now?

**[1:03:54](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3834s)** I'm not scared of you.

**[1:03:59](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3839s)** Definitely not purple.

**[1:04:01](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3841s)** Oh boy.

**[1:04:05](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3845s)** Flips his hair.

**[1:04:07](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3847s)** What are you doing?

**[1:04:13](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3853s)** Oh no.

**[1:04:19](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3859s)** You can't Okay. Um, uh, normal camera control.

**[1:04:42](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3882s)** Same deal. We can see how everything looks in Reforged.

**[1:04:47](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3887s)** You can see how this looks with the reforged graphics. All that stuff. Looks pretty good.

**[1:04:54](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3894s)** And if I go and I kill the red mage, finally the two most atrocious colors gone at last.

**[1:05:20](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3920s)** I see future that is only green.

**[1:05:28](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3928s)** Yeah, here we go. He gets his uh he gets his green future.

**[1:05:33](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3933s)** Congrats, bro. So, here you go. This was a pretty basic cinematic. You know, a little goofy, but you can see how there's a lot of variables that you can do with it. And just there you can see that there's two different types of events and ways that you can do these cinematics. So, if you have a cinematic that you like and you just want to change how it starts, if you want to change the camera angles, change the

**[1:05:55](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3955s)** timing, anything like that, you could just swap that stuff around however you please, you know. So, hopefully this was useful to you. I was kind of making it just kind of on the fly. But, I do have one thing. I'm going to go ahead and I'm going to show you now. Uh, I will just say right now that I'm going to cut off the dialogue uh because I don't want to make any sweet little lies. I can't make any promises about the functionality of

**[1:06:18](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3978s)** the game, but I can give you a cinematic overview of this project that I've been cooking up. So, sit tight. I'll go ahead and I'll boot that up for you guys right now.

**[1:06:32](https://www.youtube.com/watch?v=2fp61SdAgHs&t=3992s)** Okay. So, this is fun. It's actually glitching out with the reforged assets on me right now. And unfortunately, I'm running out the clock on this project today. So, I'll tell you what. I'm just going to let the I'm going to let the standard uh the the standard definition version of this run. And at the end of this, I'm going to go ahead. This is my end screen. This that's what's up. This is my YouTube end screen. So, I'm going

**[1:06:56](https://www.youtube.com/watch?v=2fp61SdAgHs&t=4016s)** to go ahead and I'm just going to put a link here where you can go look at the reforged version of this thing that I've been working on. I mean, this is kind of fun. You do get to see both sort of the old school style versus, you know, the updated graphics. But yeah, anyways, this this stuff isn't going to work until I give my system a restart and I probably try to boot the game up in uh gaming mode. So, unfortunately, this is

**[1:07:23](https://www.youtube.com/watch?v=2fp61SdAgHs&t=4043s)** just this is what we got to do. We got to I mean, I got to get this video done.

**[1:07:27](https://www.youtube.com/watch?v=2fp61SdAgHs&t=4047s)** It's got it's got to happen. I'm already running out the clock anyways. It's like an hour long, this video. So, anyway, you know, uh so this is a big city.

**[1:07:37](https://www.youtube.com/watch?v=2fp61SdAgHs&t=4057s)** You're going to get to do a lot of cool stuff here. Again, I don't want to fill you in on too many mechanics because I'm still figuring out how the whole thing is going to play out mechanically. Uh here's some farmlands. There's kind of a monarchy. You know, they sort of tax these lands. This is one of their uh keeps right here where, you know, they keep some people. Here's uh one of the starting areas for the game. So, you can

**[1:08:00](https://www.youtube.com/watch?v=2fp61SdAgHs&t=4080s)** see that there's some units here.

**[1:08:01](https://www.youtube.com/watch?v=2fp61SdAgHs&t=4081s)** There's some stuff to do, all that kind of stuff. And you'll notice it's still, like I said, using some of the uh the updated models, but like a like a I don't know, like a lower resolution version of them, I suppose, you know.

**[1:08:16](https://www.youtube.com/watch?v=2fp61SdAgHs&t=4096s)** So, lots of different biomes. I want there to be lots of cool stuff to do. Uh lots of secrets to uncover. Uh, I actually had to double the render distance in order to make these cinematics even possible because it kept cutting off and fading things out. And even when I tried to uh override it to make it further than it already is, that seems to be the max I can get, which is about vanilla wowish levels of render distance, which is

**[1:08:48](https://www.youtube.com/watch?v=2fp61SdAgHs&t=4128s)** enough for me. You know, this is just a fun little project, so it's all right.

**[1:08:52](https://www.youtube.com/watch?v=2fp61SdAgHs&t=4132s)** But you can see I've got lots of different castles. Uh to each of them there is a uh a stone color, then a facade color, so that every single one of the castles is actually different. I start all of the assets from scratch. So lots of very different looking castles.

**[1:09:09](https://www.youtube.com/watch?v=2fp61SdAgHs&t=4149s)** Uh they they're super unfinished, too.

**[1:09:12](https://www.youtube.com/watch?v=2fp61SdAgHs&t=4152s)** You know, all this stuff is kind of growing and changing over time.

**[1:09:18](https://www.youtube.com/watch?v=2fp61SdAgHs&t=4158s)** Uh I'm learning more, too. Of course.

**[1:09:21](https://www.youtube.com/watch?v=2fp61SdAgHs&t=4161s)** you know, I have to in order to evolve this product or product, it's not a product. I can't sell this this project.

**[1:09:28](https://www.youtube.com/watch?v=2fp61SdAgHs&t=4168s)** In order to evolve this project into uh what I really want it to be. So, right about here, it's going to say end of demo, you know, just sort of end of cinematic.

**[1:09:42](https://www.youtube.com/watch?v=2fp61SdAgHs&t=4182s)** And all right, there we go. So, I'll go ahead give my system a restart and I'll make a separate video that's the remastered graphics for this whole sort of uh cinematic. And I hope this video was useful to you guys. Thanks so much to everybody who's like subscribed to this channel and stuff recently. Um, I didn't expect anybody to really watch this tutorial series. I thought when I made this stuff it would just go to the wall.

**[1:10:10](https://www.youtube.com/watch?v=2fp61SdAgHs&t=4210s)** So, you know, here I am. We're going to keep making it. We're going to come back with the next part soon. And here's a little bit of a standard definition old school unit model glimpse at what I've been cooking up for a few months. So anyways, hope you guys like this and uh I'll see you in the next part.
