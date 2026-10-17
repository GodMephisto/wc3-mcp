---
type: vendor-doc
package: video-onetwosc
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=us2XYlrkBy0
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 923
timestamped: yes
---

# Arrays & Loops (Starcraft 2 Editor Tutorial)

Source: <https://www.youtube.com/watch?v=us2XYlrkBy0>

Uploader: OneTwoSC | Duration: 15m23s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

In this tutorial I show the reasoning and use for arrays and loops in the trigger editor.

www.sc2mapster.com

Done on patch 1.2.1 of SC2

## Transcript

**[0:00](https://www.youtube.com/watch?v=us2XYlrkBy0&t=0s)** Hey everyone. So, welcome to what might seem like a boring tutorial or um too novice for a lot of people, but um there is some really good concepts that I want to go over with regards to arrays and loops and what they are in the trigger editor. Um I see a lot of people that don't really know how to use them or there's new mappers that come and they they do things the hard way and the long way and we have a lot of good tools to

**[0:23](https://www.youtube.com/watch?v=us2XYlrkBy0&t=23s)** use that um greatly simplify things. Um this is with regards to the trigger editor. Um, so first I'm just going to show like what an array is and then after I'm going to show in conjunction with sort of using arrays is uh using loops. And so let's open the trigger editor here. So you have your map or whatever. Um, and this tutorial is not going to be like, okay, here follow me, do this. It's going to be more of just

**[0:44](https://www.youtube.com/watch?v=us2XYlrkBy0&t=44s)** an explanation of um of how they work.

**[0:48](https://www.youtube.com/watch?v=us2XYlrkBy0&t=48s)** Um, so it's not going to be as exciting as my last third person RPG tutorial or anything, but uh these are things that I use all the time. And if you know how to use them, you can do some really um quick quick ways of doing hard things.

**[1:01](https://www.youtube.com/watch?v=us2XYlrkBy0&t=61s)** Um so arrays are just ways a list of multiple variables in a sense. So if you have your triggers over and your variables over here and you have um let's say a new variable, call it player money and or player points, let's say, cuz money you can actually get you can get their minerals pretty easily. But let's say you have your point system.

**[1:21](https://www.youtube.com/watch?v=us2XYlrkBy0&t=81s)** And you have player point points one for player one. And then copy paste player points two for player two and player points three for player three. And you have these three separate variables. And um let's say that you have another trigger and you want to give player points when they get a kill. Oops, I spelled that wrong. Um, so let's say you have, sorry I have to do this on the spot here, but let's say you have a unit

**[1:46](https://www.youtube.com/watch?v=us2XYlrkBy0&t=106s)** dies and then you want to reward add a point to whichever player was the killing player. Well then you can go to variable modify variable and we want to increase. But how do we know which one to increase? Well, we have to check which is the killing player. So then now we got to go add a new action general if then else and check if the uh new condition here. We have to check if the owner of the killing unit is player one.

**[2:11](https://www.youtube.com/watch?v=us2XYlrkBy0&t=131s)** Then we'll modify player one points to be plus one. Copy paste that. The owner killing units player two. Then we got to modify player points two to be plus one and owner of the killing units three.

**[2:27](https://www.youtube.com/watch?v=us2XYlrkBy0&t=147s)** Then we modify player points three plus one. And you can see just to do something as giving whatever points for when a unit is killed um takes this giant amount of thing. And now I'm going to show you the exact same thing as what I've just done here except using an array. So instead of these three variables, we'll just take one and just call it player points and check on array here and size three because we want to

**[2:51](https://www.youtube.com/watch?v=us2XYlrkBy0&t=171s)** have three things in it. Um you can actually have 01 23. um Blizzard gave us that option. So it's actually four things, but we're not going to use the zero spot because we're using the one, two, and three spot of this array. So an array will allow us to get a certain index into it and get the variable in that index. So if that makes sense. So instead of going checking if player one points or player two points or player

**[3:14](https://www.youtube.com/watch?v=us2XYlrkBy0&t=194s)** three points, we can just go to player points bracket one bracket to get player one's points. And this will give us a lot more flexibility in doing stuff like this. So instead of these three actions, all we have to do is this action or actually I'll just copy paste one of these. Modify player points instead of three just pick our array here and instead of zero cuz zero isn't really a player and we could hardcode in one but

**[3:36](https://www.youtube.com/watch?v=us2XYlrkBy0&t=216s)** then we'd be back where we started. We can just go function killing player or owner of the killing unit killing player and same thing plus one. And then we can delete all these three and that's essentially the same thing as those giant things we just had. So that's the advantage of arrays is that it's like a list of variables that you can easily easily get into an index of it which gives you a lot of advantage because the killing

**[3:59](https://www.youtube.com/watch?v=us2XYlrkBy0&t=239s)** player is just a number. So if the killing player was one it would go to modify player points one to be plus one and same for two or three and that's the advantage of array. you can delete these three things now and we've made everything sort of nice and more cleaner and also more efficient in here um than doing those three if statements which just wastes memory, wastes game time and could potentially slow the game down if

**[4:21](https://www.youtube.com/watch?v=us2XYlrkBy0&t=261s)** you had way too many. Um if you for instance if you had 12 players and a bunch of if statements for all that now um so that's really what arrays are and anything could be an array. I mean, if you had points, for instance, for instance, um, in a lot of maps, say you have in the terrain here, you drop some points, for instance, where you want the zerglings to, uh, well, in my case, it' be zerglings cuz I always use them. But

**[4:42](https://www.youtube.com/watch?v=us2XYlrkBy0&t=282s)** say you had zerg spawn points, and you had a defense map in the center, and you have these four points where the zerg needs to spawn, but you want to randomize it. You want to say, well, every wave I want the zerglings to spawn at a random point. Um well the easiest way to do that is to take a point array of size four in this case because we have four points and in media initialization or wherever the game

**[5:04](https://www.youtube.com/watch?v=us2XYlrkBy0&t=304s)** initialization is you can just go variable set variable and set player points I should probably change this to be zerg points so it makes sense set zerg points that and set zerg points too to be so it's a little bit of grunt work now but it'll make things really cool So points three. So we store these into indexes in this array variable array of points. So now we have these all stored in zerg points 1 2 3 4. And then what we can do

**[5:35](https://www.youtube.com/watch?v=us2XYlrkBy0&t=335s)** is later on like say after um say after 30 seconds we want to spawn the first wave. Then we can just go unit create units facing angle. Let's say you want to create 10 uh zerglings, of course, four player, 15 hostile at point. Now, here's where you want to randomize the point. It'd be really hard to randomize one of these. You'd have to do if a bunch of if then else's and crap. So, you can just go variable zerg

**[6:05](https://www.youtube.com/watch?v=us2XYlrkBy0&t=365s)** points. And because you want to randomize it, we can just go function math random integer random integer between one and four. And so now this will create 10 zurglings at a random at one of these random points because it's going to find zerg points random number between one and four. And we set these right here to be specific points. And it doesn't matter the order. You know, I could have I could have made this to be

**[6:30](https://www.youtube.com/watch?v=us2XYlrkBy0&t=390s)** 2 or I mean sorry 2 and zerg points three to be uh oops I just did the same thing. um points two to be 3 and and then uh this to be 0 2. So you mean it's just that you have to cover each one. So you could do this completely out of order. It's just these all have to be set before this occurs because if these aren't set, they're initially all just no point. So you need to make sure you set them. It's

**[6:55](https://www.youtube.com/watch?v=us2XYlrkBy0&t=415s)** the same with doing regular variables is you always got to set them before you use them later. Um so anyways, that's another advantage of arrays is you got to do some advanced stuff like that. And I've even used it just a few days ago for one of my maps where I want to spawn like sort of the Zerg waves at a random point. So you're not so the campaign sort of well not campaign it's a cooperative map but so the cooperative

**[7:14](https://www.youtube.com/watch?v=us2XYlrkBy0&t=434s)** map is always different. It's not just the same units coming the same route every time cuz once people figure things out they get too they get too good at your game. So you got to keep it random.

**[7:23](https://www.youtube.com/watch?v=us2XYlrkBy0&t=443s)** Um now loops. What's a loop? Well, a loop is just a way of repeating things um that generally have the same sort of thing you need to do. So, for instance, a simple loop here is if I wanted to instead of this is going to make a wave after 30 seconds, but what if I wanted to make a wave come every 30 seconds.

**[7:43](https://www.youtube.com/watch?v=us2XYlrkBy0&t=463s)** So, I can just make a new action here.

**[7:45](https://www.youtube.com/watch?v=us2XYlrkBy0&t=465s)** And this is an example one example of a loop. There's many different kinds. Um while here, so this will execute the loop while the conditions are true. So, while whatever conditions on here are true, it's going to do these actions.

**[7:56](https://www.youtube.com/watch?v=us2XYlrkBy0&t=476s)** So, I could just say while uh I don't know, if I wanted this to run forever because it' be the waves. I could say while uh where is it game, you could check like the game game time or something. Um I think if you just leave it blank and you put nothing in there, it'll just run forever. So, let's say I want to do this. I want to create 10 zurglings at a random point and then add this little weight thing in here. And I

**[8:24](https://www.youtube.com/watch?v=us2XYlrkBy0&t=504s)** should probably keep one up here. So the first wave So the first wave will after 30 seconds it'll go into here. It'll see there's no condition. So it'll probably go forever. Don't quote me on that though. But um if you wanted to run it, if you wanted to run a condition here, well maybe I should add one. Um if you wanted to run a condition, you'd probably pick one uh that would run forever. I don't know. I can't find

**[8:45](https://www.youtube.com/watch?v=us2XYlrkBy0&t=525s)** anything. while mission time is greater than or equal to zero.

**[8:52](https://www.youtube.com/watch?v=us2XYlrkBy0&t=532s)** So while basically the map is constantly running. Um create 10 zurglings at a random point and you could order them and do stuff. Then you wait 30 seconds and then after this wait finishes. It goes back up here checks the condition.

**[9:03](https://www.youtube.com/watch?v=us2XYlrkBy0&t=543s)** Yes, miss mission time is still greater than zero. Makes the zurglings wait 30 seconds. So this is important to have this weight here because if this weight was not there, this would run like every 0001 seconds and it would basically blow up your map and blow up this battleet game, whatever. So you need to have this weight in here so that you delay the waves between each other cuz this is looping forever. Now if you wanted to

**[9:24](https://www.youtube.com/watch?v=us2XYlrkBy0&t=564s)** say, well, I only want the waves to come for the first 120 seconds, then you could say, well, the mission time is less than 120, create 10 circlings, wait 30. So, it's this is going to run four times potentially because it'll run the first time, wait 30, go up here, check, and then eventually this this won't pass and it'll move on to whatever actions are below here. Uh, which in this case are none. And that's one example of a

**[9:48](https://www.youtube.com/watch?v=us2XYlrkBy0&t=588s)** loop. Um, there are other examples of a loop which are nice. For instance, if you wanted to set everybody's resources at the start of the game, you could do it the slow way, which is player modify player property modify player one minerals set to, let's say, 100. So, you want everybody to be at 100 minerals.

**[10:03](https://www.youtube.com/watch?v=us2XYlrkBy0&t=603s)** Then you could set player two and so on.

**[10:06](https://www.youtube.com/watch?v=us2XYlrkBy0&t=606s)** Or you can just go player group, pick each player in a player group, pick each player in active players. And by the way, active players is all computer or human players in the game that aren't hostile or neutral. So it would be any AI you added and anybody on battleet, including yourself, but not player zero or 15 or any other neutrals or hostiles you added. So we're going to pick each player an active player and just drag

**[10:30](https://www.youtube.com/watch?v=us2XYlrkBy0&t=630s)** one of these under. I can delete the other one now. And then we can just modify instead of player one, you just go to function P picked player. So now it's going to go through every active player and for that picked player, it's going to set their minerals to 100. And if you had 12 players, this makes it very nice because you wouldn't want to punch out 12 times setting someone's resources. As well, it reduces

**[10:51](https://www.youtube.com/watch?v=us2XYlrkBy0&t=651s)** duplication. So if you had to type out set everybody's minerals to 100 12 times, if you wanted to change it later to 50 minerals, then you'd have to punch it in for all 12 people. whereas now you can just change it once and it'll affect them all. Now, if you wanted to have everybody with starting out with different minerals, then this might not be the best method. So, you might have to go the slow way. Um, and this

**[11:10](https://www.youtube.com/watch?v=us2XYlrkBy0&t=670s)** likewise with player group, you can do unit group. So, you can they have a nice loop for that too, which is pick each unit in unit group. So, you could pick each unit in uh units. This is what I generally use a lot. So, we could pick, I don't know, say there's a bunch of zerglings on the map, and we want to pick all zerglings in the entire map owned by only player 15. And this doesn't matter because

**[11:32](https://www.youtube.com/watch?v=us2XYlrkBy0&t=692s)** we're picking zurgling units, but it's good that we avoid dead ones in case there are any. And we could, you know, unit. We could kill them all maybe at the end or something. Where's kill unit, kill, and instead of trigger unit, find picked unit, just like it's picked player here.

**[11:46](https://www.youtube.com/watch?v=us2XYlrkBy0&t=706s)** Picked unit will refer to whatever you're looping through these zurgling units. So we could put this at the end here. So after 120 seconds, this will finish. This loop will finish and it'll move on to here and then it will pick every unit, every zurgling unit in the entire map and kill that unit. And you could do more actions in here. Like you could display, which would be really stupid, but I'm saying you can do this. You

**[12:09](https://www.youtube.com/watch?v=us2XYlrkBy0&t=729s)** could just play zurgling killed for all players, but this would probably if there was like 30 zerglings on the map, um, this would display 30 times, which would be really dumb. So I'm going to delete that. But I'm just saying you can do more actions like I did in here. You could wait between each one if you wanted to stagger it like wait a 05 seconds because you know if you kill every unit on the map at once it's going

**[12:30](https://www.youtube.com/watch?v=us2XYlrkBy0&t=750s)** to create a lag spike. So if you delay it between each one by a very slight amount um you might reduce the lag spike. And uh this is actually a good tip for any sort of map that you're doing is um if you have a lot of stuff running and you're doing stuff like pick each unit in unit group and it's going to pick all of them and do it in a fraction of a second. Well, that's going to that generally like especially in

**[12:49](https://www.youtube.com/watch?v=us2XYlrkBy0&t=769s)** Warcraft 3, it would cause a little hiccup or uh your map would lag every 4 seconds or something. So, if you just put a little bit of a delay, this is even a big delay, probably even smaller like that or something. Um I think I hope that's there's a minimum delay, but I think this is still above it. Um if you put a little bit of a delay, it'll do each thing individually, wait a bit, then do the next one, then do the next

**[13:11](https://www.youtube.com/watch?v=us2XYlrkBy0&t=791s)** one, then do the next one. So, it'll kind of stagger it. won't kill all the computers at once if you try to do something insane. Um like order 5,000 units to move or something like one of those million ultralist type of maps. Um but it's generally not too bad in most cases. So um but that's that kind of loop. And then there is of course the for loop uh which is useful. It get like uh you need to have a local variable

**[13:36](https://www.youtube.com/watch?v=us2XYlrkBy0&t=816s)** just call it temp int of type integer and then you just go set it here. So, it's going to go for for every integer from 1 to 10. You could do something.

**[13:47](https://www.youtube.com/watch?v=us2XYlrkBy0&t=827s)** So, you could say um I don't know. Well, let's say from 1 to four, you could go unit create unit facing angle. Create one. Let's say a billboard. That's really dumb. But let's create a billboard for player one at zerg points function I mean variable temp. So, what I just made, while kind of pointless, illustrates the for loop, which is just an advanced version of the while loop. The while loop will wait

**[14:15](https://www.youtube.com/watch?v=us2XYlrkBy0&t=855s)** until this condition. The for loop will wait until it hits four because you're increasing by one each time. So, it's going to start at one. It's going to go in here, create one billboard at zerg points one, which is this point here.

**[14:26](https://www.youtube.com/watch?v=us2XYlrkBy0&t=866s)** Then, it's going to go back up here, right here, and it's going to start go to two, create a billboard at temp zerg points two, then go to three, then go to four. Then, it's going to finish and move on. And that's basically all I wanted to say about arrays and loops. Um I hope I've provided enough examples and sort of explained it in a useful way. Um because these are basically programming concepts. So if you don't know

**[14:47](https://www.youtube.com/watch?v=us2XYlrkBy0&t=887s)** programming and you're figuring this out, good for you because you are figuring out programming pretty much and that's all trigger editors are is highle sort of nice looking trig uh nice looking programming language. Um, but yeah, definitely use arrays and loops and keeps your it keeps everything sort of much cleaner and I personally use it a lot and um you can do a lot more advanced stuff with it as you can see as

**[15:12](https://www.youtube.com/watch?v=us2XYlrkBy0&t=912s)** I'm doing here as I'm creating stuff at an array using a loop. It's just they're so closely related. So, uh, thanks for watching and I hope to have more tutorials out soon.
