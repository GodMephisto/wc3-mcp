---
type: vendor-doc
package: video-lake
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=HQhfBwu--5g
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 706
timestamped: yes
---

# Round Dev Log - Part 2: Tornadoes! [Warcraft 3 Map Making)

Source: <https://www.youtube.com/watch?v=HQhfBwu--5g>

Uploader: Lake | Duration: 11m46s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

In this episode we focus on one of the primary mechanics of the round, which is the Storm Callers Tornado ability!
Feel free to ask questions if you're curious about something.

For more info on the map and download visit:
https://www.hiveworkshop.com/threads/...

Wurst Script:
https://wurstlang.org/

## Transcript

**[0:07](https://www.youtube.com/watch?v=HQhfBwu--5g&t=7s)** hey guys and welcome to the second part of my round development video look if you're new here then this is my small series where I take you through how I develop around for my hero defense Warcraft 3 map called laddies alright last time we came up with a round idea finish creating the units and added all the units into the round and also of course sort of the basic round structure in the code this means that we're ready

**[0:39](https://www.youtube.com/watch?v=HQhfBwu--5g&t=39s)** to get started with the mechanics of the unit's been around and I'm actually thinking that we will start with a more interesting mechanic which I talked about last time which is the tornado ability for the naggers storm : all right let me just really quick explain how that turning ability is going to work so when they spawn they will start channeling the ability and they will keep channeling as long as they hardline

**[1:09](https://www.youtube.com/watch?v=HQhfBwu--5g&t=69s)** when they start the channel they will spawn a tornado for each of the gladiators in the arena now it's only one because we haven't implemented any multiplayer yet the tornado will then follow the gladiator that it's assigned to and if any gladiator is standing near a tornado they will take damage periodically so the idea is that you have to run away from these Tomatoes alright so I think this introduction has

**[1:34](https://www.youtube.com/watch?v=HQhfBwu--5g&t=94s)** lasted for long enough let's just get started programming this ability and see where it goes all right so we're jumping straight into the programming this time I'm programming in woods and if you don't know what was this then I can briefly explain it as a programming language with which you can create or craft three Maps in this video or any other videos in this series for that matter I won't go into too much detail with the code I

**[2:02](https://www.youtube.com/watch?v=HQhfBwu--5g&t=122s)** will of course explain what I'm doing and why I'm doing it but the syntax and all that I won't explain but if you want me to elaborate a little bit on the worst part then just leave a comment down below anyways let's just get back to what's going on in the video right now so the very first thing that we're actually doing here is creating the ability and this is one of the places where waste is really

**[2:27](https://www.youtube.com/watch?v=HQhfBwu--5g&t=147s)** powerful because I can actually make an ability without even entering the world editor so the ability in itself which we're creating here is just a placeholder ability which is meant to keep the storm colors occupied and the placeholder ability is built upon the tranquility ability from the keeper of the groove all right so here I'm just changing the settings for disability so that it actually doesn't do anything at

**[2:54](https://www.youtube.com/watch?v=HQhfBwu--5g&t=174s)** all and I'm also just setting the duration to be incredibly high so that I won't stop casting this in the middle of the fight so it's a good idea to keep testing while you're creating so I just created the ability and therefore I'm just going to test if the ability even works as I intended all right so it's kind of working the ability is there and the unit can cast it and there's no cooldown there is no mana cost on it but

**[3:25](https://www.youtube.com/watch?v=HQhfBwu--5g&t=205s)** if you notice something then well the effect is kind of still there isn't it okay so I just tried to fix this with the effect and but I can't a hard time figuring out which one I actually have to change in order to remove the effect but in the end I figure that and look at this we have an ability which does absolutely nothing okay so the next step is to make what we call a class in boost which is basically a blueprint for

**[3:59](https://www.youtube.com/watch?v=HQhfBwu--5g&t=239s)** objects and we have to make a class for this tornado which keeps track of each units costing this class is the point in our code where we are controlling the abilities such as keeping track of the tornadoes that the ability spawns and again we are going to test this and we want to test here that the update is running and the ability is stuck correctly when the Consular stops casting it now this will print the line every time

**[4:30](https://www.youtube.com/watch?v=HQhfBwu--5g&t=270s)** at update and checks if the cars like we're still casting and the cast is still alive and we also just checking that the system is stopping correctly when the caster stops casting and everything seems to be working just fine so I realized the casting of this ability looks a little bit though so I decided to add an animation effect to the caster but these mangas Aaron actually don't even have a channel ability which is

**[4:57](https://www.youtube.com/watch?v=HQhfBwu--5g&t=297s)** kind of odd because I'm quite sure there's a mission where you see them channel something but the way around this is basically to keep repeating this their spell animation which is what I'm doing here I'm just adding a ton which keeps restarting this spell animation and look at that that looks pretty cool I'm quite happy with oh yeah I just realized that they don't through the animation at first so you

**[5:27](https://www.youtube.com/watch?v=HQhfBwu--5g&t=327s)** have to wait like those 1.5 seconds before they start the animation even though the ability action has done so I just quickly added that cool so now we have a pretty boring ability it looks cool when they were casting it but it doesn't really do anything but of course that's going to be their next step in the process all right perhaps that was a little bit of a life we're not going to make their build be cool just yet because we have

**[5:58](https://www.youtube.com/watch?v=HQhfBwu--5g&t=358s)** to make the tomatoes first and actually we can make the tomatoes quite separate from the ability to begin with and then we add the tomatoes later and the first step in that process is to figure out how the tomatoes are going to look so that's just what I'm trying to do here trying out different sizes and figuring out if I have to change the color of its NATO which I realized you actually could that's pretty cool so we have to create

**[6:25](https://www.youtube.com/watch?v=HQhfBwu--5g&t=385s)** a new class for the tomato which keep track of the tomato effect and tanana's ability it's also going to keep track of all the damage stats and all but we're just going to wait with that just a little bit removing of the tornado is quite simple I just have a timer which keeps updating the position to move a little bit further to watch the gladiator which is the tornados type just for testing purposes we are going to add one tornado

**[6:55](https://www.youtube.com/watch?v=HQhfBwu--5g&t=415s)** when the ability is being cast we were apparently brief succeeded in creating partly invisible tomato okay so clearly something has gone wrong with the coloring here or at least so I thought after trying a few attempts on fixing the coloring I realized that I have made a mistake I didn't use the model which I thought I use apparently there are a lot of models for tomatoes so I tried and figure out which one lost a quick one

**[7:30](https://www.youtube.com/watch?v=HQhfBwu--5g&t=450s)** and sure enough that seemed to fix the problem I mean come on how easy was that that looks pretty cool I mean they don't do any damage but it looks cool that is all the counts right okay so even though I'm saying it looks cold I think it could look better the thing is these tomatoes are actually meant to be big or the model is at least which means when you compress it down to a smaller size it gets a little bit intense you

**[8:01](https://www.youtube.com/watch?v=HQhfBwu--5g&t=481s)** see all this kind of glowing effect around it instead I realized that there is a smaller model for small tornado which doesn't even have this glowing thing so I just found the model for that was that implemented that and and this is what it ended up looking like all right now it looks cool okay let's be honest it's not enough that it looks good it kinda have to do some sort of damage right so I just added a new timer

**[8:37](https://www.youtube.com/watch?v=HQhfBwu--5g&t=517s)** which deals damage to all units around the tornado every second and I thought that was really easy to implement but I even failed at that I realized that I wasn't register the source of the damage correctly and therefore no damage will be told so here we go now we have damage dealing tornadoes all right now we've been working so hard and actually creating tornadoes but we also have to destroy them again so I just

**[9:10](https://www.youtube.com/watch?v=HQhfBwu--5g&t=550s)** implemented a really simple solution which checks every time the tornado moves if the unit which it is targeting is still alive and if it's not well then it should destroy this name of course we should also destroy this in a know if this source is there so I just added an exact copy of the other code which just checks the source instead of the target but this didn't work the first time around because I added functionality

**[9:39](https://www.youtube.com/watch?v=HQhfBwu--5g&t=579s)** into the client which enables me to turn off the triggering of destroying the tornado when the source died and I had turned that off so of course I had to enable that before anything work now everything seems to work just like it should I thought that adding the tornado to the ability as I did was a temporary solution but in the end I actually decided that it works just fine essays so instead of changing anything I'll

**[10:11](https://www.youtube.com/watch?v=HQhfBwu--5g&t=611s)** just keep it just like it is right now so for the final test I just removed all the printing statements and added a lot more dagger and this is the result of all the word how we find so far so that's actually the end of this episode we created one of the mechanics for the rounds but we still have a few more to go and we actually even have a mechanic's still for the stormcaller I mentioned and in the first episode

**[10:57](https://www.youtube.com/watch?v=HQhfBwu--5g&t=657s)** which is the the immolation kind of ability that you'll have so the next time we'll take a look at that and we'll also dive into the murmured ins abilities so I hope you enjoyed this episode if you have any questions just leave a comment down below if you want to know when the next episode is coming out just subscribe and if you liked the video hit the like button thanks for watching and see you guys next time

**[11:25](https://www.youtube.com/watch?v=HQhfBwu--5g&t=685s)** [Music]
