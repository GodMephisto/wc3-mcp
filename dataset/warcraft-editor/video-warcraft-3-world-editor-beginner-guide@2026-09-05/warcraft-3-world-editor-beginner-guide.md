---
type: vendor-doc
package: video-keytorch
package_version: latest
language: transcript
source_page: https://www.youtube.com/watch?v=HpsOwfMmGO0
source_url: video
extractor: automatic captions (en)
exported: 2026-09-05
duration_s: 1046
chapters: 22
timestamped: yes
---

# Warcraft 3 World Editor - Beginner guide!

Source: <https://www.youtube.com/watch?v=HpsOwfMmGO0>

Uploader: KeyTorch | Duration: 17m26s | Captions: automatic (en)

_Automatic captions. Expect errors in names and technical terms._

## Description

I've tried to make this tutorial as dense with information as i could, without going too fast (hopefully). Let me know in the comments if you have any questions and I'll be more than happy to help you if i can.


I was using version 1.32 of the world editor when i made this video.


Suggestions to go further with your custom game:
Make a respawn trigger.
Add a shop with custom (or normal) items.
Add a wave boss.
Add wave clearing income.
Only spawn HeroSelectionWisps for the number of players in the game.


If you're stuck;  https://www.hiveworkshop.com/ is a good resource for help. And of course you've always got google :)


Timestamps:


0:00 Overview
0:21 Making the terrain
1:02 Adding regions
1:25 Adding trees
1:42 Setting map name
1:51 Adding livestock
2:06 Rotating units
2:16 Intro to triggers
2:57 Setting up the initialization trigger
3:40 Setting up the wave trigger
5:16 Testing the map
5:34 Setting up players and teams
6:46 Making the hero selection system
11:08 Creating a custom castle building
11:41 Creating the lose trigger
12:30 Adding more wave triggers
14:00 Variables and arrays explained
14:30 Array explained
14:45 Wave triggers finished
15:05 Making the win condition
15:35 Putting the icing on the cake
16:15 Ending

Software used:
Vegas Pro Edit 16
Affinity Designer
Audacity
Warcraft 3 Reforged

## Chapters

- [0:00](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=0s) Overview
- [0:21](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=21s) Making the terrain
- [1:02](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=62s) Adding regions
- [1:25](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=85s) Adding trees
- [1:42](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=102s) Setting map name
- [1:51](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=111s) Adding livestock
- [2:06](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=126s) Rotating units
- [2:16](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=136s) Intro to triggers
- [2:57](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=177s) Setting up the initialization trigger
- [3:40](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=220s) Setting up the wave trigger
- [5:16](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=316s) Testing the map
- [5:34](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=334s) Setting up players and teams
- [6:46](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=406s) Making the hero selection system
- [11:08](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=668s) Creating a custom castle building
- [11:41](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=701s) Creating the lose trigger
- [12:30](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=750s) Adding more wave triggers
- [14:00](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=840s) Variables and arrays explained
- [14:30](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=870s) Array explained
- [14:45](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=885s) Wave triggers finished
- [15:05](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=905s) Making the win condition
- [15:35](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=935s) Putting the icing on the cake
- [16:15](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=975s) Ending

## Transcript

**[0:00](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=0s)** welcome to this Warcraft 3 world editor tutorial today I'll show you how you can make your own custom game in Warcraft 3 as an example I'll be making a Defender Castle type of game and the result of what we'll be making you can see on the screen right now it should teach you enough about the editor to make many different types of custom games let's not waste any more time and get right into it open the world editor and scroll

**[0:24](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=24s)** out so you can see the whole map you'll probably see that the whole screen goes black this happens because the view distance of the editor is too low we can fix this by holding down control and scrolling down with your scroll wheel I'm going to open the terrain pallet and make my map grassy I also add three lanes a section for the hero selection and some room for the castle and some shops the next step is to add some regions

**[1:04](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=64s)** with the region pallet these regions will be used to spawn units when we set up the rules for our game give the regions a logical name as we will be referencing the names later I'm also going to make a region called creep attack which is where all the monsters will go to attack the castle make your map a little bit more beautiful by adding some trees with the D dead pallet hit control s to save your map but you

**[1:34](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=94s)** might get this message just press yes and you'll also get this message cuz we haven't changed the default name of our map so let's do that go to scenario map description and let's call this defend the castle let's add some livestock to our map open the natural passive team and you can choose a pig for example place them out choose a rabbit choose a sheep place them out and if you want you can rotate them

**[2:07](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=127s)** by marking them holding down control and then using your left Mouse button to rotate them but they will just wand around anyway so it doesn't really matter which way you place them let's dive into triggers these are basically the rules of your map we already have a pre-made trigger called the melee initialization but we don't want it because it gives us a normal game mode with the base building and stuff so just

**[2:29](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=149s)** delete it press the new trigger button to create a new trigger and we will create our own initialization trigger a trigger consists of three parts the events are the events that happen in the game which will trigger the trigger the conditions are conditions that must be met for the actions to take place and the actions are what the trigger will do when it gets activated right click events choose new event and then choose map

**[2:56](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=176s)** initialization by choosing that this trigger will be activated when the map loads let's set up some new actions for our map press G to go to the first action that starts with that letter then choose the set time of day and accept the default value which is noon then press G again and find the turn day/ night cycle on off and accept the default value this will make sure our game is always set to daytime when you

**[3:23](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=203s)** save your game now you'll get this message and that's totally fine that's what we want we often end up with a lot of triggers so sorting them into categories can be smart create a new category and call it unit spawn we're now going to create the trigger that's going to spawn the attacking units for the sake of testing we will make them spawn after 5 Seconds of game time we then create the spawning action

**[3:49](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=229s)** look for unit-c create units facing angle this action will let you spawn a specific number of units choose which unit type to spawn what team it belongs to where to spawn it and which angle it will face let's only choose where it spawns for now copy paste the action two times and change the spawn position so we have one Spawn action for each region just to make it a little bit more interesting change the spawning units in

**[4:21](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=261s)** two of the actions if we started the map now the units would just be standing there we need need to tell them where to go add another action and find the unit Group Issue order targeting a point then choose what region to move the units from make them attack move to that means that they will attack all the Hostile units in their way and lastly choose the region you want them to move to drag the action below the unit

**[4:50](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=290s)** creation actions we need to do this because the actions gets executed from top to bottom if we had the move action at the top the units would not have been created yet and the game would not know what to move copy paste the action three times and change the regions also remember to change the destination region to the creep attack region now for the font part Let's test our map to do that just click this red

**[5:18](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=318s)** button up here now let's see if anything happens up here oh look at that guys nice our next step would be to set up the players and the teams to do this go to scenario and then player properties as you see all the players except player one is grayed out if we tried to host our game now we would only have one player spot let's add four more player spots let's also add a computer controlled player that's going to be on

**[5:53](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=353s)** our team and a player which the enemy units will belong to next up we must set up the teams click the forces tab then check the use custom forces box then click the add Force button and rename the teams to something more fitting then check all the boxes under allied Allied Victory and share Vision also check the fixed player settings this box makes the teams fixed so no one can join the wrong team here's an example with fixed player

**[6:22](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=382s)** settings off and here's an example with fixed player settings on to finish the team setup move player 12 down to the enemy team when you try to save the game now you'll need to accept to have the new players starting positions autogenerated they will get spread out over the map just gather them in the middle so the map doesn't get so cluttered the next chapter in our journey is to make the hero selection

**[6:50](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=410s)** system we want to give each player a whsp to pick your hero with and five Heroes to choose from we could go into the unit pallet and find the Wisp but that will give the players a wisp with all the abilities that a normal wisp has so I'll delete these wisps and create a custom wisp with no abilities to create the custom wisp you open the object editor the first tab lists all the pre-made units in the game to create a

**[7:15](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=435s)** custom unit we can copy an already existing unit if you see this message just click okay you will not be able to overwrite any existing IDs so don't worry since we copied this from a normal wisp we need to get rid of the abilities so the only thing this unit can do is to move around now we place out our custom wisps and assign one to each player we'll use the circle of power to be our hero picker portal you find them

**[7:56](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=476s)** in the unit pallet change the team to neutral p passive set the category to campaign and you will find the circle of power under buildings any of the three circles are okay to use pick five Heroes that you like and place them on the map make sure they're assigned to Player 11 add five regions one for each Hero Portal give them reasonable names also add a region where you want the heroes to spawn when they're picked

**[8:38](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=518s)** let's create the logic for the hero picker system create a new category for the new triggers we'll create we will make one trigger for each portal the first one will be the template for the other four for the event we will set it to unit unit enters region and choose the region for the topmost arrow for the actions we will choose unit create units facing angle the options here gets a little bit more advanced for the player

**[9:06](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=546s)** will choose owner of unit then change it from triggering unit to event response entering unit then change the spawn location to the hero spawn region we created earlier then create a small action that removes the entering Wisp from the game as a quality of life fix to our selection system we can make sure that the hero automatically gets selected when it spawns we can do this with an action called selection select unit for

**[9:40](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=580s)** player I choose the wrong function here at first sorry if that confused you I also managed to choose the wrong event response I need to select the last created unit function instead which is at the top I corrected this later on the next step is to make sure that the trigger creates the correct hero when you walk into the portal the trigger is also starting to be finished so you can copy paste the trigger four

**[10:07](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=607s)** times one for each portal name them correctly and now you need to update each trigger with the correct unit and the correct regions another nice quality of life feature would be to have the camera instantly move to your newly spawned hero to do this we need to create a new camera object enter the camera pallet and click the create camera button place the camera correctly then give it a logical name now we need to update the herrow

**[10:35](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=635s)** select triggers with a new action the one we're looking for is called camera apply camera object timed set the camera object to your newly created camera and the player to the owner of a unit and then the event response to entering unit drag the action to the bottom of the action list then copy paste this action to all the triggers we also need to remember to change the owner of the wave units to the enemy

**[11:01](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=661s)** team and it will be also nice to change the event to a periodic event that happens every 15 second we've comeone pretty far with our map right now but there are still some systems missing to make our map enjoyable we need to make more waves create a custom castle and a win and a lose trigger in the object editor find the castle in the fer human melee buildings create a custom unit from it and give it a super original

**[11:25](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=685s)** name like the greatest Castle remove its abilities set the HP to a th000 and remove any trainable units from it place it in the map and assign it to Player 11 now let's add the loose condition make sure to spell the trigger name correctly unlike me as you can see I've already started the trigger I've made an event that makes sure that the trigger is activated when a specific unit dies when you try to specify the unit you

**[11:54](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=714s)** might not find it in the drop down list in that case just use the select the unit button for the action we need to use a for Loop this is a pretty common function in programming and the way it works is that you go through a set of actions x amount of times and you can use the value of how many times the for Loop has looped as a value in your actions we need the for Loop in this trigger to tell all the players that the

**[12:19](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=739s)** game is over instead of just telling player one that the game is over it will use the number of times it has looped as the player number and loop through all the players telling them that the game is over now let's add some more waves copy them four times and give them proper names create a trigger called wave Master this trigger will control the other waves this trigger will activate one wave at a time so we need to make

**[12:43](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=763s)** sure that the wave triggers start in the off position let's set the wave Master to start after 10 seconds and then give all the players a message on the screen telling them that the game is starting then add a for Loop that will do a loop for each wave the loop will function like this it will turn on one wave trigger and wait for some time before it turns it off it will do this one time for all the five

**[13:11](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=791s)** wave triggers to give the trigger some time to spawn the units you can add a weight command like I do here it would also be nice to make the loop weight until all the units from the previous wave are killed before spawning more units you can do this with a weight for condition action add a boole on comparison condition and check if all the players of player 12 are dead also add some time for the players

**[13:45](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=825s)** to prepare for the next round now there's a big flaw here in our code this Loop will only turn the wave one trigger on and off five times we need to make sure that it turns each wave trigger on and off one time to fix this we need to add a variable to be more precise an array to add this click the green x button name the variable something logical we have to specify the correct type of data that this variable is going

**[14:12](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=852s)** to hold in our case triggers it's going to contain five of them so set the size to five if you continue to add systems to this map you might start to get a lot of variables so add a category to store them in an array consists of index numbers these index numbers store an object or a value but now that we have just decare the array it's empty so we need to store one trigger to each index we do this with an

**[14:42](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=882s)** action choose the set variable action and make one for each trigger when you're done with that you need to change the trigger the for Loop disables and enables set the trigger to the the array of waves variable and set the index to the integer a of the for Loop we can now finally make the wing condition set the event of this trigger to happen every 2 seconds of game time give it a condition that checks if the

**[15:13](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=913s)** wave Master trigger is on or off and create an action that displays to all the players that the game has been won now we just need to make sure that the wave Master turns itself off after it has finished and finally guys our map is fail wait a minute we forgot to put the icing on the cake guys but don't worry it'll only take a second just go to the camera palet copy the hero Spa camera and paste the copy at the hero selection area

**[15:45](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=945s)** rename it go to an initialization trigger and let's add a for Loop sort the actions for camera actions and pick the apply camera object timed action set the camera object to the one we just created and set the player number to integer a let's add some visibility for the player in case they're scared of the dark find the create VIs buil the modifier circle at the bottom of the list set the player integer to a the

**[16:02](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=962s)** area to Peak Hero 3 and change the radius to 2,000 copy it and change the region to Hero spawn and last but not least add bounty to the enemy team find the player Das turn player flag on/off action and set the gives bounty to on for player 12 the icing has been put on the cake and it's finally time to test our map so we have a framework of a castle defense game and this was just a tutorial for you to understand how you can build your

**[16:30](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=990s)** own game and out of this framework that we've built here for a game uh there is a lot of room for improvements but you don't need to do a lot to make it you know pretty fun just add a shop with some items add a Boss Wave and change up the units coming from each wave and yeah there's uh there is there are so many things you can do if you just have the fantasy uh enough to to you know F figure out some cool system or some cool

**[17:02](https://www.youtube.com/watch?v=HpsOwfMmGO0&t=1022s)** event that's going to happen so I hope you enjoy this tutorial and you learned a lot of stuff uh this was my first Warcraft 3 tutorial I'm not sure if I'm going to make more but I might so if you have any questions uh leave them in the comment section and I might get any ideas for what else to to make an make a tutorial about but uh yeah that's about it thank you for watching
