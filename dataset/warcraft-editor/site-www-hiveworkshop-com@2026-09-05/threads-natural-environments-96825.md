---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/natural-environments.96825/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Natural Environments
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/natural-environments.96825/>
#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

Natural Environments

Terrain tutorial by Void.

(uses the Ultimate Terraining Map)

Choosing a Camera Angle

Creating Height Variation

Rivers, Lakes and Waterfalls

Everything About Rocks

Placing your Trees

Shrubs and Grasses

Tile Variation for Dipshits

Choosing the Right Distance Fog

Additional Fog Tricks

Lighting -- the "Epic" Effect

____________________________

Choosing a Camera Angle

 This section is all about choosing the right camera angle. A bad camera angle can completely screw up your picture, which is not good at all! Most of this can be done in the terrain editor by using control and shift with the mouse buttons, but for the sake of being efficient, and since it has more features, in this tutorial we will be using the Camera Editor.

To access the Camera Editor, open up the Camera Palette, create a new camera and then right click it. Then select "Edit Camera Properties". You will see something like this:

Target X/Y: This is where the camera target points. You can change this easier by selecting the camera in the editor and moving it with the mouse.

Z Offset: Changing this value raises or lowers the camera off the ground. I suggest values between 200-800.

Rotation: This is the horizontal rotation of the camera.

AoA: This is the vertical rotation of the camera. An angle of 360 is exactly level with the ground.

Distance: This is how far away the camera is from its target. The higher the number, the further out the camera is zoomed.

Roll: This is hard to explain, but it's the exact same thing as setting a negative Max Roll Angle for a doodad or unit in the object editor.

FoV: This is like zoom but more complicated. The default value is 70, lowest 20 and highest 120. I don't tend to do much with this, but you can play around with it.

FarZ: This value sets where that annoying black clipping effect occurs. Set it to whatever you want, I just max it out.

____________________________

Natural Height Variation

 This section is all about the most commonly overlooked part of terraining, creating height variation. It's a lot more complicated than just picking a brush and raising the terrain, that is, if you want it to look natural. In this section you will learn:

Hills and Mountains

Making Cliffs

Plateaus

__________________________________________________

1. Hills and Mountains

Ok. So now that you have whatever camera angle you want to use (I used a 350 AoA camera with Z offset of 400), we are going to make a mountain in front of the camera. First, take the raise tool at brush size 8 (circular) and make a large mound in front of the camera.

Now that we have a large, shapeless mound, this is usually where people stop and go onto the next step. Don't. Take a raise tool at size 5 (you can also use lower) and put some variation all over the mountain.

It's still not done. It still looks like a big lumpy blob. To add sharper details, however, we must add cliffs to it...

__________________________________________________

2. Making Cliffs with the Raise Tool

If you are reading this tutorial you probably already know not to use the blizzard cliff tools... right?

Now, we are going to add some variation to our lumpy blob and make some cliffs. First, take the raise tool at a brush size of 3 (again, you can also use lower for this, I use both) and use it liberally above and below the steep parts of the mountain.

Another great way to make cliffs, this only works in flat-ish areas, not really on mountains, is to use the plateau tool and a large raise brush. This is probably the only time you should EVER use the plateau tool when creating hills and mountains, however. First, make some hills, then cut edges about 1/4 into them with plateau tool, and then apply a large raise brush (5 or 8) over the whole thing. That is how I made the hills in my Dawn terrain.

__________________________________________________

3. Plateaus without the Plateau Tool

Before you start, let me tell you that the plateau tool is really bad. It's like using the smudge tool in a painting. It's not as bad as blizzard cliffs or noise, but it's still bad and should be used VERY rarely.

To make a plateau, we will use the raise and lower tools, along with the smooth tool. First, raise up the land around where you want a plateau with a brush size of 3 or 5.

However, you see now that it is still kinda bumpy and not flat at all. Here is where the smooth tool comes in. Apply it with a brush size of 2 or 3 all over your plateau but not the edges. The difference is very subtle but it really is worth doing.

Congratulations! You are now a master of height variation! (not really...), keep fooling around with it. It takes a lot of practice to get good at, but remember always, too much smoothness = BAD.

____________________________

Water and Waterfalls

 This section of the tutorial is about how to properly make bodies of water and waterfalls. This carries on in the next section (rocks), so you might want to read that one too if you're just here for waterfalls.

In this section we will mainly be using doodads from the Water section; Water, and Waterfall(2).

__________________________________________________

1. Lakes

Before we start with this, let me tell you how bad blizzard water is and how it clashes with custom models. If I may make another reference to photoshop, it's like using a filter to make something that would look better if you did it yourself with brushes.

So to make a lake, first we need to lower a place for the lake to be. Use the lower tool of about size 3 where you want the lake to be.

Now, grab the doodad called Water (just water, you might want to edit it in the object editor to tint it, but I'm not going to for this). Go to a flat area on the map and place a few of them next to each other. Raise them a bit, and drag them over to your lake area. Why do this? It makes sure the height of the doodads are perfectly even to avoid clipping and rough edges. If your lake doesn't look how you want it of course feel free to smooth and raise/lower as much as you want.

__________________________________________________

2. Waterfalls

Waterfalls are amazing. Really, waterfalls alone make your terrain look 20% more epic. So this bit will teach you how to make waterfalls with the Waterfall(2) doodad. You might want to refer to the Everything Rocks section which has a bit on waterfalls as well.

First, go into the Object Editor and find Waterfall(2). All you need to do now is set the maximum scaling value to 100 and minimum to 0.01, that way you can make the waterfalls whatever size you want. I'd also suggest you turn off random scaling in the doodad palette.

Ok, now that you have that done, go into your terrain and put down 2 or 3 waterfalls in the exact same place. Since the UTM's Waterfail doodad is really thin and ugly (better than Blizzard's, though), stacking them makes it look a bit better.

Waterfalls usually look better if you have a bunch of them facing different angles and LOTS of extreme scaling variation.

____________________________

Everything About Rocks

 The goal of this section is to teach you how to use rock doodads while avoiding rockspam or emptiness. Since the previous section was about waterfalls, that's what we will do first. But before you start, go into object editor and set the maximum scaling value of whichever rock type you will be using to 100 and minimum to 0.01. That allows rocks to be whatever size you want. Also take off "random scaling" in the doodad palette. Pro terrainers scale things themselves

__________________________________________________

1. Waterfalls

This will teach you (hopefully) how to make your boring waterfalls look better, and how to cover up those ugly wave things at the top and bottom of the waterfall doodad.

First, grab some rocks, and place a few bigger sized ones in the middle of the waterfall. I also suggest that you use variations 1, 4 and 8 only. (this might be different for rock types like sunken ruins and snowy). Change their scaling using the + and - buttons, and the PgUp/PgDn/Home/End buttons.

Then, take some smaller rocks and put them at the base and top of the waterfall, and along the sides of the lake. Really that's all there is to it, just keep repeating the process until you think you have enough rocks.

__________________________________________________

2. Cliffs, Covering and Creating them

This will teach you how to make your cliffs more than just rock tiles. This will mainly use larger scaled rocks, but a few smaller ones at the top and bottom of the cliffs.

First, find a cliff you want to cover, and put a few large-scaled rocks over the steep edge like this:

Now, you might notice that the tops of the cliffs look really jagged and ugly. You can cover them up with a short and wide, flat rock.

You can also create cliffs where there is no height, but just be careful to avoid rockspamming.

____________________________

Placing Your Trees

This section is all about placing trees. It's somewhat short, because really it's quite simple, but it's an important and often overlooked problem. First of all, for this section, you might want to turn random scaling back on and make sure you have random rotation on.

Now, I like to tell people that tree variation is really important. If you just want to use one kind of tree, go ahead, but here's my suggestion: Pick out one or two pine trees. Pick out one non-pine tree. Pick out one dead tree. Those 3 or 4 trees are the ones you will use in your terrain (any more can look bad, be careful how many different kind of trees you use, that goes for rocks and plants as well).

For this terrain I chose Pine (City Summer), Night Elf Green (3) and Deathwood. Now, start by putting down the deciduous trees (NE Green) at key locations like above the waterfall, at the bottom of the lake and on top of the cliff. Doing so makes them stick out more to people who see your terrain, which is good. It makes them focus more on the environment you have created.

Now, place your pine trees in the empty spaces in the foreground. Use a lot of them

Ok. Now we need to fill up that empty background. Copy your deciduous and coniferous trees and make the copies have a default scaling value of 0.5. Then do the same thing as stated before but in the background.

If you want to use dead trees, you can go ahead. I'm not going to use them in this terrain. However unless it's winter or a dark terrain you should probably use them sparingly.

____________________________

Shrubs and Grasses

 The aim of this section is to teach you how to properly place shrubs and grasses. Usually when placing plants it's best to start with the bigger plants and work your way down.

__________________________________________________

1. Bushes and Ferns

Bushes and ferns are great in any terrain (almost). The ones I will use in this tutorial will be Bush(4) and Fern(1). You might want to tint them darker because the default model is very bright. Now, make sure random scaling and rotation are on! This is very important, otherwise your plants will look very unrealistic.

First, put bushes in key locations like cracks in between rocks and underneath trees. Try to use a lot, but not too much, shrubspamming is even worse than rockspamming.

Chances are you will probably use more ferns than bushes. So scale them down to about 60%, that way you can safely spam them without blocking out any details.

__________________________________________________

2. Grasses, flowers and other plants.

This section is all about the small plants like flowers, grass and cattails. Because cattails are more of a medium-sized plant, you should start by placing them around your water. One commonly ignored detail is that plants do NOT grow in a line, which is often done with cattails. Try to variate them.

Now, for grasses, something many terrainers I know do is spam grass. That's REALLY not good. It looks better and more realistic if you place grasses in clumps.

 Why? First of all, spamming grass covers up tile variation. Also, it looks very unnatural, more like a lawn than a wilderness. Grass grows in clumps. Period.

Now, the final step is placing flowers. I like small flowers, so I'm going to size mine down to 50%. You can do whatever you want. Flowers, like plants, also grow in clumps. However they grow in slightly bigger clumps, and the clumps are more spread out than with grass.

Usually the clumps grow outside the shadow of trees and rocks, where the sun is. Like this:

____________________________

Tile Variation

 I usually save tile variation to after I'm done placing environmental doodads. You can do whatever you like really, sometimes it's good to place tiles before to get a sense of where you're going to put things. But anyways, the goal of this section is to teach you one of the most important aspects of terraining-- Tile variation.

Although this may sound stupid, tile variation is all about randomness. Sure, you should have some idea where to put certain tiles, like using rough dirt where there's no grass, using rock on cliffs, etc, but in nature everything is all over the place.

Some terrain tutorials may tell you "don't place dirt tiles next to grass tiles, use transition" which is great and all, but you should ignore that rule. Instead, you should fool around with tilesets and find which tiles look good next to eachother, i.e. Dirt and Rough dirt, or grassy dirt and rock.

First, what I do, is put down the rough dirt and rock tiles. Put rock tiles on the cliffs, and rough dirt tiles on the steep slopes and open bits.

Now, for grassy dirt and grass tiles, grass tiles tend to look good under rocks (so the edges stick out from underneath) and grassy dirt tiles look good underneath grass and flowers. You should get something like this:

That's really all there is to it. It's really one of those things that needs practice and fooling around with to look great.

____________________________

Choosing the Right Distance Fog

 This section is VERY important. Distance fog is your friend, it allows a nice, smooth transition into the background and it hides mistakes and rough areas in your terrain.

Usually, I choose a start value of 0-1000 and an end value of around 5000-8000. It really depends on how big your terrain is. The further away the end value is, the clearer your midground and background will be. The further away the start value is, the clearer your foreground will be.

As for color, good day colors are Light Blue and Light Gray. They are the colors you see in nature in real life. Other colors can be used like yellowish-gray and such, but are not really recommended for environmental terrains. Some good night colors are very dark blue, very dark bluish-gray and very dark purple. Try to avoid using solid black fog.

For this terrain I will use a start value of 1000, end value of 4000 and a tint of 175/175/200. This gives the appearance of a slightly foggy morning, maybe after a rainy day.

____________________________

Fog Tricks

 This section is all about the great effects you can make with the glow and eerie fog doodads. The two main ways these are used are for waterfalls and "fog variation". Fog variation is basically variating the thickness of fog throughout your terrain with fog doodads.

To create fog variation, go into the object editor and find under cinematic the doodad Fog. Edit it so the tinting is 25/25/25 (this makes it more transparent, not darker). Now, without random scaling on, apply a bit of this fog where you think the concentrated areas would be. Good examples of these would be in the background (clouds), above and below waterfalls, above lakes, in forest clearings and on the side of cliffs. Here is what I produced for this terrain:

Now, the other great fog trick that I will talk about in this tutorial is the Eerie Fog doodad. Since it's a particle emitter, sizing it up doesn't increase the size of the particles, it just makes them more spread out, which looks really bad. So you should keep it at the same size. Also, you can't tint eerie fog.

Eerie Fog usually looks best at the base of waterfalls (as spray). You could consider this lighting, but personally since it's called Eerie Fog, I prefer to label it as a fog trick. To use Eerie Fog, just put one (only one!!!) at the base of each waterfall, like such:

____________________________

Lighting -- The "Epic" Effect

 This is my favorite part of terraining. You can do so much with lighting, and it's what gives your terrain the most epicness of all. It's a very subtle touch that can add contrast, brightness, color, whatever you want to your terrains. The lighting we will talk about in this tutorial is the most simple form, the White Light doodad. This goes very well with the sky, it can give the sky a lot of life. If you choose to use a sky doodad, it works even better.

Placing white light is very simple. Just select it in the list and move your mouse around until you find a spot that looks good to you. If you have cities in your terrain, white light is great to put there. If you have a dungeon or an evil castle, you might want to try blue light.

Using too much white light can produce an ugly clipping effect. I suggest you only use 1-5 lights in your terrain.

This is my final product:

I hope you learned a lot from my tutorial!

(you'd better have... it took 4 hours D

#### Attachments

tut-1.jpg

 22.8 KB

 · Views: 4,022



tut-2.jpg

 120.2 KB

 · Views: 4,099



tut-3.jpg

 119.4 KB

 · Views: 3,826



tut-4.jpg

 121.3 KB

 · Views: 3,844



tut-5.jpg

 116.8 KB

 · Views: 4,148



tut-10.jpg

 112 KB

 · Views: 2,559



tut-9.jpg

 4.4 KB

 · Views: 2,433



tut-8.jpg

 110.3 KB

 · Views: 2,763



tut-7.jpg

 112 KB

 · Views: 2,486



tut-6.jpg

 112.8 KB

 · Views: 2,637



tut-15.jpg

 181 KB

 · Views: 2,494



tut-14.jpg

 155.1 KB

 · Views: 2,655



tut-13.jpg

 130.9 KB

 · Views: 2,543



tut-12.jpg

 131.9 KB

 · Views: 2,497



tut-11.jpg

 127 KB

 · Views: 2,487



tut-16.jpg

 184.4 KB

 · Views: 2,516



tut-17.jpg

 188.6 KB

 · Views: 2,414



tut-18.jpg

 191.4 KB

 · Views: 2,490



tut-19.jpg

 194.8 KB

 · Views: 2,580



tut-20A.jpg

 24.3 KB

 · Views: 2,609



tut-24.jpg

 156.5 KB

 · Views: 2,713



tut-23.jpg

 202.7 KB

 · Views: 2,733



tut-22.jpg

 203.5 KB

 · Views: 2,792



tut-21.jpg

 200.4 KB

 · Views: 2,524



tut-20B.jpg

 25.6 KB

 · Views: 2,535



tut-25.jpg

 162.6 KB

 · Views: 2,732



tut-26.jpg

 163.3 KB

 · Views: 2,857



tut-27.jpg

 154.6 KB

 · Views: 4,574




 Last edited by a moderator: Jul 26, 2014

#### FamousPker49

#### FamousPker49

 Level 18

JoinedMay 27, 2007Messages1,689

Resources: 3

#### Resources
Models 1Skins 1Tutorials 1

Nice tutorial, but I feel like the fern shrubs are out of place there, or at least too bright for some reason.

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

Yeah I just noticed that too. But looking outside at the forest outside my house, the ferns are considerably brighter than the other shrubs. So whatever. That's my excuse ^.^

#### Just_Spectating

#### Just_Spectating

 Level 39

JoinedMay 16, 2007Messages7,272

Resources: 49

#### Resources
Models 13Icons 10Skins 26

Wooo Void Owns at this. +2rep

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Why are all the images linked from ImageShack if they're attached to the post as well?

Otherwise, very nice.

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

Because I didn't read that I had to attach them to the post until after I linked them all ;P

#### Naze

#### Naze

 Level 28

JoinedNov 12, 2007Messages2,338

Resources: 6

#### Resources
Icons 4Maps 1User Interface 1

Hey, that's nice.

But I think you should tint the ferns, their green is too vivid.

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

Fuck, fuck the fucking ferns, FFS.


-.-


No seriously, it's the light that's making them glow like that.

#### Just_Spectating

#### Just_Spectating

 Level 39

JoinedMay 16, 2007Messages7,272

Resources: 49

#### Resources
Models 13Icons 10Skins 26

the WRONG and RIGHT pictures are both WRONG

theres a cattail in the rock XD

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

LIES! Stop pointing out mistakes D:


It was a quick terrain lol.

#### Gilles

#### Gilles

 Level 34

JoinedSep 6, 2006Messages8,872

I use the plateau tool all the time! I just smooth and raise after.

Great tutorial, but I was hoping on you making one on the awesome mountains you always make in the background. I struggle with them.

#### Naze

#### Naze

 Level 28

JoinedNov 12, 2007Messages2,338

Resources: 6

#### Resources
Icons 4Maps 1User Interface 1

Let's point more mistakes on Void's tutorial!

Kidding, I think that we should really stop spoiling your thread...

~Void~ said:

 No seriously, it's the light that's making them glow like that.


Click to expand...

lol, it's kinda funny how we talk about plants that "glow"...


 Last edited: Aug 31, 2008

#### jadegolem

#### jadegolem

 Level 7

JoinedFeb 1, 2008Messages295

Resources: 2

#### Resources
Models 1Maps 1

i cant find water -_- WHERE IS IT I TRIED SEARCHING FOR IT T_T

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

Do you have the UTM?

#### cHeRoL

#### cHeRoL

 Level 11

JoinedMar 30, 2008Messages650

Resources: 5

#### Resources
Models 1Maps 2Spells and Systems 2

I like the terrain.

#### Celaquil

#### Celaquil

Patron

 Level 37

JoinedNov 24, 2007Messages4,427

Resources: 4

#### Resources
Maps 4

Good tutorial indeed ^^

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

Hmm, can someone of power come along and review this? It's been a while

#### Just_Spectating

#### Just_Spectating

 Level 39

JoinedMay 16, 2007Messages7,272

Resources: 49

#### Resources
Models 13Icons 10Skins 26

like hawkwing?

#### Oziris

#### Oziris

 Level 27

JoinedOct 14, 2006Messages4,161

Resources: 7

#### Resources
Icons 1Skins 3StarCraft II Resources 1Tutorials 2

Well my tutorial is like 3-6 weeks and no aprove.

#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

Oziris, yours has hardly been there 2 weeks. Void, just be patient  one of my tutorials got approved after so long that I didn't even know it still existed.

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

Well my last one got reviewed after about two days. Maybe it's just got to do with timing.

#### Oziris

#### Oziris

 Level 27

JoinedOct 14, 2006Messages4,161

Resources: 7

#### Resources
Icons 1Skins 3StarCraft II Resources 1Tutorials 2

 for me it is like a year!

#### Belgarath

#### Belgarath

 Level 21

JoinedSep 22, 2007Messages2,772

Resources: 13

#### Resources
Maps 3Spells and Systems 1Tutorials 9

Oziris... you posted your tutorial on the 29th August 2008. That's exactly 15 days ago.

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

Oziris said:

 for me it is like a year!


Click to expand...


Two weeks is a year? So that makes you 312 years old...

#### Linaze

#### Linaze

 Level 24

JoinedFeb 28, 2007Messages3,478

Resources: 4

#### Resources
Models 1Tutorials 3

~Void~, awesome tutorial. The Key to Terraining Like ~Void~! Oh well, good work and easy to follow, good result blabla, +rep.

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

Bump for approval...

#### Airandius

#### Airandius

 Level 17

JoinedJun 12, 2007Messages1,261

Resources: 8

#### Resources
Icons 2Maps 1Ported Icons 4Tutorials 1

A very nice tutorial that explains the basics about making a terrain, very usefull.


It should be approved, the only thing is, why are the images inside the tutorial from imageshack if you have perfectly working thumbnails?

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

Because the website I hosted this at first (thehelper.net) has an attachment limit.

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

What the hell, how long is it going to take for this to get reviewed? -.-

It's been over a month now since I first posted this. Wake up, moderators!

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

Wow. Just wow. Almost 4 months and no response.

#### Airandius

#### Airandius

 Level 17

JoinedJun 12, 2007Messages1,261

Resources: 8

#### Resources
Icons 2Maps 1Ported Icons 4Tutorials 1

Yeh, Void I have the same problem.

I asked about it, and it seems there is no GUI tut moderator atm, so we can wait for a very long time.. xD

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

Well that's fucking stupid. It's not THAT HARD to review a tutorial.

#### Ebisux

#### Ebisux

 Level 4

JoinedDec 27, 2008Messages72

Well,im not new to hive, but i finnaly made an account,


im trying to get into terrian making,


and i finished this, and have a pretty dumb question,


for a screenshot how do i get rid of the purple cubes?

#### Gilles

#### Gilles

 Level 34

JoinedSep 6, 2006Messages8,872

Sorry Void, I forgot about my new power to approve tutorials. If you PM me next time, I'll be sure to get on it quickly.

~Approved~ (I changed the name, I hope that's okay)

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

Ebisux said:

 Well,im not new to hive, but i finnaly made an account,

im trying to get into terrian making,

and i finished this, and have a pretty dumb question,

for a screenshot how do i get rid of the purple cubes?


Click to expand...

Hit "V", and it will display things as they are shown in game (i.e. without the cubes)

And thanks Gilles, it was about time someone came and looked at this

#### White-Lion

#### White-Lion

 Level 7

JoinedDec 18, 2008Messages400

i dont understand what editor are you using and how to get it

#### Airandius

#### Airandius

 Level 17

JoinedJun 12, 2007Messages1,261

Resources: 8

#### Resources
Icons 2Maps 1Ported Icons 4Tutorials 1

White-Lion said:

 i dont understand what editor are you using and how to get it

Click to expand...


You get it with the game, buy the game and you'll get the editor.

#### TheBlooddancer

#### TheBlooddancer

 Level 22

JoinedJun 24, 2008Messages3,041

Resources: 2

#### Resources
Maps 1Tutorials 1

Its called 'World Editor'. Remember, this is not WoW.

#### White-Lion

#### White-Lion

 Level 7

JoinedDec 18, 2008Messages400

 but where can i find the water and those cool looking trees ^^

#### LordDz

#### LordDz

 Level 25

JoinedMay 11, 2007Messages4,643

Resources: 8

#### Resources
Maps 8

White-Lion said:

 but where can i find the water and those cool looking trees ^^


Click to expand...


How long have you spent in the editor? Make a map, fuck around on it. Test EVERYTHING, you can't do anything wrong.

#### Archael

#### Archael

 Level 2

JoinedJan 26, 2009Messages12

Wheres the fog doodad and whats UTM???

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

There's a link.

#### xDeathKnightx

#### xDeathKnightx

 Level 18

JoinedJul 14, 2008Messages1,353

Resources: 24

#### Resources
Icons 8Packs 3Maps 13

kk, awesome Terrain ~Void~ this is purely epic, you are a legend.


 Last edited: Aug 6, 2009

#### KryptFriendz

#### KryptFriendz

 Level 2

JoinedJun 14, 2009Messages13

Yeah seriously? since when is there a water doodad? And where is the grass doodad? I've had warcraft iii for 5 years and have used the editor soo much. I have no idea what your talking about.

#### Verify110

#### Verify110

 Level 2

JoinedAug 15, 2008Messages25

oh... those were ferns...


ohhhh


I thought they were like some blight effect or sumtin.

#### mpcopy

#### mpcopy

 Level 2

JoinedMay 20, 2008Messages12

Nice.


~Void~ = uber terrianer, you pwn!

#### Priest233

#### Priest233

 Level 5

JoinedMay 11, 2008Messages93

My water looks a bit strange, so you know how i can fix it?

#### Attachments

Water.jpg

 426.5 KB

 · Views: 310



#### Cosmic Storm

C

#### Cosmic Storm

Trees?

Where do you get thoughs amazing looking trees from!?

#### Nagir

#### Nagir

 Level 6

JoinedSep 6, 2008Messages99

Resources: 8

#### Resources
Maps 7Templates 1

If you made a whole map with the terrain you've shown on your pictures + more, plz an u send it to me, or at least upload it here on the Hive Workshop?

#### yeohxin

#### yeohxin

 Level 10

JoinedMar 19, 2010Messages622

Resources: 4

#### Resources
Maps 2Spells 2

why me WE no hav the fog dooded one?

 You must log in or register to reply here.


### Similar threads

 Terraining Tut. - Forests


HernanG

Dec 4, 2006

Misc/General Mapping Tutorials

Replies5Views40K

Jun 14, 2015
Remixer

 Playable Terrain: Overgrown Forests


Belgarath

May 3, 2008

Misc/General Mapping Tutorials

Replies10Views20K

May 22, 2016
the.BIT

 How to Make Easy Waterfalls


RoboHippo

Nov 10, 2009

Misc/General Mapping Tutorials

Replies13Views20K

Jul 18, 2015
The Nightraven

 Useful Tips for Night Forest Terrains.


Belgarath

Nov 7, 2008

Misc/General Mapping Tutorials

Replies12Views10K

Apr 6, 2009
Linaze

 Terrain: Forest Cliffs


Jattenalle

May 6, 2007

Misc/General Mapping Tutorials

Replies15Views21K

Nov 1, 2008
Dimadelic

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Misc/General Mapping Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top