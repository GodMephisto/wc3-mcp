---
type: vendor-doc
package: video-onetwosc
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=J_5SDcM8AH8
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 399
timestamped: yes
---

# WoW Camera & Controls Part 4 (Starcraft 2 Editor Tutorial)

Source: <https://www.youtube.com/watch?v=J_5SDcM8AH8>

Uploader: OneTwoSC | Duration: 6m39s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

This is the last video in this tutorial-series 
Make sure to watch in 720p!

In this tutorial-series I make a WASD control system like World of Warcraft, and also a left/right mouse camera similar to WoW as well. And finally, I do a simple attack (right click).

http://www.sc2mapster.com

## Transcript

**[0:00](https://www.youtube.com/watch?v=J_5SDcM8AH8&t=0s)** um so now that we've got that we're gonna do another one so control copy make sure it's not inside here make sure it's make sure it's outside i don't know how i do that make sure it there we go make sure it's outside not inside this thing it's a separative statement and this time let's do um maybe the players holding these two if the player is holding w and a we want it to kind of want the unit to travel

**[0:26](https://www.youtube.com/watch?v=J_5SDcM8AH8&t=26s)** northwest if you understand what i'm saying so the way polar coordinates work is that 0 degrees is east and 90 degrees is north and 180 is west and 270 is south so if that still doesn't make sense you can always just copy exactly what i'm doing and then it'll work um so instead of going plus 0.1 we want to turn at a little angle now because players holding w and a so we want to do run angle plus 45

**[0:58](https://www.youtube.com/watch?v=J_5SDcM8AH8&t=58s)** degrees um and the reason for this is that our run angle is going to be the straight w direction and we want to go us at a 45 degree angle of that because they're holding a as well and i hope that makes sense so that that's good and the next condition we need to do is what if the player is holding um w and d so set this to false which means they're traveling northeast from a compass kind of point of view so

**[1:26](https://www.youtube.com/watch?v=J_5SDcM8AH8&t=86s)** w and d and if they're doing that we want to do um minus 45 degrees or you could do plus 220 or plus 315 degrees i think but keeping the numbers simple okay minus 45 and then copy paste again so now what if the player is just traveling west they're just holding a so we just want to do just a and get rid of the d on get rid of that okay and if they're just if they just want to travel with the a button

**[2:09](https://www.youtube.com/watch?v=J_5SDcM8AH8&t=129s)** then we need to make them walk plus 90. okay and now what if they are just holding the d button and so we gotta do another one make if they're just holding the d here and if they're just holding d we want them to go minus 90.

**[2:36](https://www.youtube.com/watch?v=J_5SDcM8AH8&t=156s)** so the opposite sort of thing keep it simple copy paste okay and what do i have next in my notes um oh yeah so if the players now if the player is only holding the s key let's change this back to a false if the player just wants to walk backwards we just have to make him walk plus 180 you could do minus 180 it doesn't matter i just like plus um and the only issue with this that's different than rpgs is that

**[3:12](https://www.youtube.com/watch?v=J_5SDcM8AH8&t=192s)** he's going to walk towards the camera he's not going to walk backwards towards the camera and we can't really at least as far as i'm concerned we can't really change that um this is gonna work fine though in this case it doesn't really matter okay and now if he's wants to travel uh southwest which is w i mean which is s and a so s is on and a is on and if he's got s and a on we want him to go plus 135 degrees okay

**[3:47](https://www.youtube.com/watch?v=J_5SDcM8AH8&t=227s)** and just take my word for these numbers and then if he wants to go with s and d now i'm just covering all the bases here s and d okay and i'm going to order this and the direction is going to be plus 225 okay and that's pretty much it for unit movement if everything went well and um i think we can actually go test this now so i'm gonna go in game okay i'm in game um w is forward s is back a is left d is right i can do the

**[4:32](https://www.youtube.com/watch?v=J_5SDcM8AH8&t=272s)** diagonals too uh left mouse scrolls around rightmost changes his facing and works pretty good but we do have one bug which i know i've forgot to fix because you can see he's kind of being jumpy when we hold the right mouse and move at the same time he runs well without right mouse but as soon as we hold right mouse he gets sort of jumpy so let's go fix that in this thing we want to do after existing orders and i don't know

**[5:01](https://www.youtube.com/watch?v=J_5SDcM8AH8&t=301s)** why it just works like this but it does after existing orders and then we want to change i think our run i think our run condition is okay i think we can just run this and we'll see hopefully our unit movement condition act trigger i mean not our run condition okay so let's go test this okay and yeah now i'm holding right mouse and i'm moving and it's much smoother the guy is running properly there's some

**[5:32](https://www.youtube.com/watch?v=J_5SDcM8AH8&t=332s)** drones so i'm going to right click this guy good and now i can drag around move back i'm going to right-click this guy um this is actually pretty fun so yeah i really hope this tutorial helps people because i've seen a lot of camera tutorials out there and nothing's really solid or smooth is what i've done here and this is sort of completely usable but it's up to you where you want to go with this i mean if you want to

**[6:03](https://www.youtube.com/watch?v=J_5SDcM8AH8&t=363s)** add abilities and stuff you can make dialog buttons um if you understand what they are because i did hide the game ui um and the sort of trick here is that we don't actually have our unit selected uh as a warcraft 3 unit because if we do we see these we see our move and things going off crazily because of our triggers running so quickly um but it's pretty cool and we don't actually our guy won't actually attack

**[6:28](https://www.youtube.com/watch?v=J_5SDcM8AH8&t=388s)** we actually have to right click so it's kind of realistic in that sense it's not just auto attack and so i hope this tutorial helps it's been pretty fun doing and thanks for watching
