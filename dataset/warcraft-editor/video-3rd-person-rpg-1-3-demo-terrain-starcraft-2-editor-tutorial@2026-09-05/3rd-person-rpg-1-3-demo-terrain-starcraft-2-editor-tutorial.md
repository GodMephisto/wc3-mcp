---
type: vendor-doc
package: video-onetwosc
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=-S_dyTVodkk
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 661
chapters: 9
timestamped: yes
---

# 3rd Person RPG (1/3) - Demo & Terrain (Starcraft 2 Editor Tutorial)

Source: <https://www.youtube.com/watch?v=-S_dyTVodkk>

Uploader: OneTwoSC | Duration: 11m01s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

Part 2: http://www.youtube.com/watch?v=B4BcC7wqrmQ
In this series I show how to design terrain for 3rd person view, set up and auto-rotate the camera, make a moving skybox, and alter the user interface with SC2Layouts.

www.sc2mapster.com

Map DL: http://www.mediafire.com/?5nqe8qj8cibjbvq

Demo & Part 1 (Terrain)
00:34 Demo
02:27 Part 1 Begins
03:55 Cliffs
05:33 Pathing Blockers
06:19 Trees
07:13 Tree Scale
08:07 Placing Zeratul
08:41 Placing Initial Camera

Done on patch 1.2 of SC2

## Chapters

- [0:00](https://www.youtube.com/watch?v=-S_dyTVodkk&t=0s) <Untitled Chapter 1>
- [0:34](https://www.youtube.com/watch?v=-S_dyTVodkk&t=34s) Demo
- [2:27](https://www.youtube.com/watch?v=-S_dyTVodkk&t=147s) Part 1 Begins
- [3:55](https://www.youtube.com/watch?v=-S_dyTVodkk&t=235s) Cliffs
- [5:33](https://www.youtube.com/watch?v=-S_dyTVodkk&t=333s) Pathing Blockers
- [6:19](https://www.youtube.com/watch?v=-S_dyTVodkk&t=379s) Trees
- [7:13](https://www.youtube.com/watch?v=-S_dyTVodkk&t=433s) Tree Scale
- [8:07](https://www.youtube.com/watch?v=-S_dyTVodkk&t=487s) Placing Zeratul
- [8:41](https://www.youtube.com/watch?v=-S_dyTVodkk&t=521s) Placing Initial Camera

## Transcript

**[0:00](https://www.youtube.com/watch?v=-S_dyTVodkk&t=0s)** welcome everybody um this better be a pretty mind-blowing tutorial for you guys because uh I think I found out a lot of cool stuff um the title does say third person RPG but it is going to be more so about the uh the technical side um this isn't going to be about how to make your hero level um as I just made a giant tutorial about that um so this is going to be more about the uh interface and doing kind of a technical side with

**[0:26](https://www.youtube.com/watch?v=-S_dyTVodkk&t=26s)** the Skybox and doing the camera and um why don't we go ahead and take a look at how it'll look at the end okay so you can see I'm in game here and the interface has been pretty manipulated um it is still still all blizzard elements though um and this is because as of patch 1.2 we can actually move around their their own things we don't have to use dialogues for a custom inventory I mean custom uh UI um so when

**[0:53](https://www.youtube.com/watch?v=-S_dyTVodkk&t=53s)** I click on him you can see I got my little wireframe up here my attack and stuff down here all my abilities are kind of spread out on the Bottom now and the mini map's up here um as far as the controls go it's just just sort of like regular Starcraft 2 where you select a thing and then you select where you want to go um and the camera just rotates automatically so that's what I've that's what I'm going to show for this tutorial

**[1:14](https://www.youtube.com/watch?v=-S_dyTVodkk&t=74s)** and that's what we're going to make um now you can use the up and down arrow to switch the angle and uh wherever you click the camera sort of rotates too um and you're going to see why they have this fog here there's actually a good reason why it's sort of foggy into the White Abyss um and that's the best I could figure out for this tutorial but uh after doing this tutor you could probably come out and figure something

**[1:38](https://www.youtube.com/watch?v=-S_dyTVodkk&t=98s)** better maybe um but anyways yeah this pretty cool it's sort of a third person little RPG um and Custom Custom uh UI and sort of a moving fog system to stop the map from rendering everything across like the entire game World um so the tutorial is going to be split into three parts the first part is going to sort of be the basic terrain set up and just set up basically uh and part two is going to actually be where

**[2:07](https://www.youtube.com/watch?v=-S_dyTVodkk&t=127s)** you do the camera and the movement and this this the fog and everything and part three is going to be where we actually tear up the blizzard inventory uh I don't know why I keep saying inventory blizzard UI I mean um to sort of be all this kind of clean change from their default UI so uh let's go ahead and get started in the editor Okay so in this case I am going to make a new map and um I don't know if I'm going to just

**[2:37](https://www.youtube.com/watch?v=-S_dyTVodkk&t=157s)** pick agria but uh I don't know if you're going to do that for yours but anyways I'm just going to select some default dependencies make sure we have all the campaign ones just so we have Zer tool and stuff um 64 by 64 is good and agria is good um that's what I actually used in this demo that you can see in the back uh same thing so um first thing we're going to do is zoom out a bit here and make hit o to

**[3:02](https://www.youtube.com/watch?v=-S_dyTVodkk&t=182s)** make sure your camera bounds are on and then go to map map bounds and then we're just going to push them all to the edge um because when you do make a third person map you want to have as much space as you can um and I made a relatively small map here but I'm guessing most third person Maps would be 256x 256 so and you'd still want to extend this to the edge so you get as much playable area as you can so um as I

**[3:27](https://www.youtube.com/watch?v=-S_dyTVodkk&t=207s)** said this is part one so in part one we're going to do more of the R here um if that's not important to you then you can switch to part two um but I also will at the end of this part is I will position the first camera position so you might want to watch the end the end part of this part one for that um okay so um when you think back to World of Warcraft or things they don't actually use Cliffs like we would here in the

**[3:50](https://www.youtube.com/watch?v=-S_dyTVodkk&t=230s)** terrain editor if you're on layer terrain um in Starcraft 2 you paint your terrain like this and that's how you do your walls but in uh in in World of Warcraft what they actually do was they would paint this height tool here and pick a large size large fall off and even larger size maybe 10 and then just start painting the mountains like this so this would be the edge of the map and they would just

**[4:15](https://www.youtube.com/watch?v=-S_dyTVodkk&t=255s)** paint it up sort of like that um and then you build and then you'd have to paint rocks so go around and just paint some walls sort of like this and uh that's sort of the edge of the world uh don't let the player kind of see behind these things so we just paint the cliffs like this and let's say you could keep going and I'll just fast forward through this part okay so let's say now you've painted all the train here and then what

**[4:43](https://www.youtube.com/watch?v=-S_dyTVodkk&t=283s)** you do right after that is um switch to the texture tool here and switch to agria rock and then change the brush size down a bit and then Paint Rock along the edges sort of like that and I will fast forward through this part too okay and then uh after you've painted The Rock here and maybe just spread some grass out just to make it look a little bit nicer um that's not really the whole point of this tutorial but still nice to

**[5:10](https://www.youtube.com/watch?v=-S_dyTVodkk&t=310s)** have a good quality uh terrain to work with and I'm going to paint some height just to kind of vary up the train a bit because when you're uh when you're when you're down low like this it look starts to look really flat different than regular RTS sort of view so you definitely have to paint some sort of variation on the train it can't just be perfectly flat like you find in most Maps um and that's a bit better

**[5:33](https://www.youtube.com/watch?v=-S_dyTVodkk&t=333s)** now and now after we've done this hit H to switch to the pathing tool and we're going to paint pathing no pathing uh size maybe three and we're just going to paint it around the edge here so that uh they can't walk off the edge of the map or walk up to it too far just paint it along the rock sort of so it's natural where it would block because otherwise they could walk on these Hills which wouldn't be too

**[5:57](https://www.youtube.com/watch?v=-S_dyTVodkk&t=357s)** good okay and now they're blocked off so that's good and you'd actually want to paint uh this whole area behind so they couldn't blink to it or anything but um I'll leave that to you to do that in yours um anyways actually well maybe I should just quickly do a swipe through here because this will be downloadable all right anyway so we got that and now we're going to place some trees so hit D to go to doad and um

**[6:23](https://www.youtube.com/watch?v=-S_dyTVodkk&t=383s)** we're actually going to do some stuff with the Trees Too to make it more uh realistic when you're down in third person view um so search for uh tree here there's too many dads taking a while uh okay agria tree so I'm going to place a few of them kind of spread them out don't make them too dense cuz when you're down low in third person view it starts to get a lot more congested when you have all these trees

**[6:55](https://www.youtube.com/watch?v=-S_dyTVodkk&t=415s)** everywhere um you'll even notice that as my example is World of Warcraft they they actually have big trees but they're really spread out um not much to get in the way of the player um okay so we have a bunch of trees in this very strange looking forest from from the overhead view but um now what we're going to do is switch to the data editor and we're going to scale the trees up so if you go to the actors Tab and all dude ads that

**[7:20](https://www.youtube.com/watch?v=-S_dyTVodkk&t=440s)** you place on the editor are just actors um and search for tree cuz actors are just the visual models that are in the game uh find tree agria and uh let's hit the field art- scale and let's make it 222 and now we can see this looks a little bit cooler now if you're down low in here feels more like a real sort of large forest and the like I said the map is pretty small that we chose so it might not look right but you definitely

**[7:51](https://www.youtube.com/watch?v=-S_dyTVodkk&t=471s)** want to make a map that is uh oops that is bounds of click this check that off and then make it all the way to 256 um because when you're so up close on the map it actually starts to have a lot less space um and now that we've done that we're going to just place the starting Zer tool and the first camera and then we'll be done with part one um so hit the U to go to the units layer of the train editor here and we're going to place a

**[8:22](https://www.youtube.com/watch?v=-S_dyTVodkk&t=502s)** Zer tool as soon as it finally loads uh okay so for player one we're going to drop a hero of Zer tool and I'm going to place him right here in the center and then double click him and make sure he's facing 90 on his rotation okay now uh hit C to go to cameras press middle Mouse down to make sure that you're on the default game camera and then zoom in a bit Center it on him and hit create camera and I'm going to uncheck show

**[8:56](https://www.youtube.com/watch?v=-S_dyTVodkk&t=536s)** camera so I can see a bit better and then right click again and go modify properties check off preview in the train editor and let's increase the field of view a bit so it's more first person style um let's lower the distance quite a bit and then let's uh change the pitch so that we can kind of this will be sort of the first default third person view up the Z off set a bit and then let's lower the distance

**[9:26](https://www.youtube.com/watch?v=-S_dyTVodkk&t=566s)** even more there we go I think that's a good good workable size um maybe increase the field of view a bit so you can see a bit rounder and we'll press okay and just press view to make sure you see it looks good it's about centered on him that's pretty good let me view it again okay and uh one thing you actually want to do that I just missed um is back in here in the camera change the far clip to 55 so

**[9:57](https://www.youtube.com/watch?v=-S_dyTVodkk&t=597s)** what the far clip will do is it'll make like this black wall that you can see back here um you can see as I move back that far clip should start to kick in um well actually this map is pretty small but um if I go view this camera and if I lower the far clip really small you're going to start to see the black wall come in there we go um so what this does what this means is that anything beyond this black wall is not going to

**[10:24](https://www.youtube.com/watch?v=-S_dyTVodkk&t=624s)** get rendered by your computer which is good for the fact of running performance wise but then it's bad for the fact that you have this black wall and that you can't see beyond that point um so we're going to increase it to 55 and that works out PR pretty good frames per second still and then in part two we're going to actually make a moving Skybox that moves with your hero with the fog so it sort of looks like it goes

**[10:49](https://www.youtube.com/watch?v=-S_dyTVodkk&t=649s)** so you don't see that black wall so it sort of looks like it goes off to Infinity um okay and that's it for part one so I'll see you in part two
