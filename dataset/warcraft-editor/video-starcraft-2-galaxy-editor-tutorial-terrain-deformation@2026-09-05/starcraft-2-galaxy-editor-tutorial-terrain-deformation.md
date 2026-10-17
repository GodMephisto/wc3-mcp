---
type: vendor-doc
package: video-raven67854
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=Ga5bPD2AAM4
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 303
timestamped: yes
---

# Starcraft 2 Galaxy Editor Tutorial - Terrain Deformation

Source: <https://www.youtube.com/watch?v=Ga5bPD2AAM4>

Uploader: raven67854 | Duration: 5m03s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

https://discord.com/invite/6THUkfy
Member - https://youtube.com/c/raven67854/join
Patreon - https://www.patreon.com/raven67854

In this tutorial I show you guys how to do terrain deformation in the Starcraft 2 galaxy editor.


HatiStarcraft:
http://www.youtube.com/watch?v=Ct2zYTvh9u0

Written tutorial by HatiStarcraft:
http://forums.sc2mapster.com/development/tutorials/895-data-terrain-actors-how-to-do-live-otf-terrain-deformation/

Map FIle:
http://www.file-pasta.com/file/0/terraindeformationtutorial.SC2Map

*** Social / Information ***
Twitter - https://twitter.com/Raven67854
Patreon - https://www.patreon.com/raven67854
Become a member - https://youtube.com/c/raven67854/join
Steam Group - http://steamcommunity.com/groups/RGC67854
TwitchTV - http://www.twitch.tv/raven67854
Discord - https://discord.com/invite/6THUkfy

#gameplay #gaming #games #raven67854

## Transcript

**[0:00](https://www.youtube.com/watch?v=Ga5bPD2AAM4&t=0s)** hello everyone my name is Raven welcome to my terrain deformation tutorial uh in Starcraft 2 I first saw this tutorial uh done by hadti Starcraft and he has also released a text based tutorial uh both his example video and his text tutorial are in the description below all right so let's go ahead and get started um first we need a unit that we're going to use for for the training deformation so up here where you see

**[0:36](https://www.youtube.com/watch?v=Ga5bPD2AAM4&t=36s)** units select unit then where you see player one we're going to swap over to terrain and then find our Marauder just put him somewhere down there all right now we're going to give him something to shoot at so switch over to player two go to Zerg and select the changel Marines and just place a few of them Around The Marauder all right hit space to deselect all right so the next thing we need to do is we need to go up here to our data

**[1:15](https://www.youtube.com/watch?v=Ga5bPD2AAM4&t=75s)** editor or you can hit F7 now we need to set up our Marauder so that it can deform the terrain so go to actors if you don't have this tab where you see data type hit the drop down it select actors and then just select then just sorry don't select right click and hit add object and then in the object's name type terrain deformation then ID just hit suggest actor type scroll down to the bottom to where you see terrain deformer and hit

**[1:56](https://www.youtube.com/watch?v=Ga5bPD2AAM4&t=116s)** okay all right the next thing we need to do this we need to right click and add event now we need to select our message type let's see here we need to select effect sorry I couldn't find it you could also just hit E I just remember remember that all right and then for Source name you're going to go to Marauder um whatever use for the source name will be whatever effect will cause the teranium deformation so we're going to use our

**[2:46](https://www.youtube.com/watch?v=Ga5bPD2AAM4&t=166s)** source name which is our Marauder launch missile and then for our subname we're going to select impact so it only works when the grenade impacts and then under Action Impact select message type and then you scroll down till you find create and select create all right there's one last thing we have to do we have to go over to our actors's tab uh we have to adjust our blend time which is if you highlight over blend

**[3:14](https://www.youtube.com/watch?v=Ga5bPD2AAM4&t=194s)** time it'll tell you the blend time when Terin is being deformed by the actor we're just going to put it at 2.0 so it will gradually deform the terrain our height data is how much it will either raise or lower the terrain since this is a we're going to do a height date of 1.0 and an influence radius which is the size we're going to do at 1.0 now we just need to select our footprint and I'm going to select one by

**[3:46](https://www.youtube.com/watch?v=Ga5bPD2AAM4&t=226s)** one one by one creep all right so now if you were to go over to units and if you don't have it once more go to data type find units find your Marauder in the search bar you'll notice at the bottom left we have terrain deformation in our Marauders actors tab all right so now if you go up to here where you see SC test document or you can h control F9 and we're going to go on ahead and test out and see if

**[4:19](https://www.youtube.com/watch?v=Ga5bPD2AAM4&t=259s)** it works all right so you guys probably have a screen that says you've tied just go on ahead and hit return to game jump over to your Marauder select your Marauder and then just attack your enem the change lanes and you can see the train slowly deforming all right so that concludes this tutorial do you have any questions uh just post a comment or send a message as well uh I'll be posting the save map thank you
