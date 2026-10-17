---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-a-maze.117136/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making a Maze
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-a-maze.117136/>
#### DarkLordX

#### DarkLordX

 Level 7

JoinedDec 3, 2008Messages189

Resources: 6

#### Resources
Maps 1Tutorials 5

Hi and welcome to my Maze-Tutorial. In this tutorial you will learn how to make a succesfull maze. Of course you don't learn all triggers you could use in it, but I will show you the basics of it...


Content:


1. What's a Maze?


2. Making the terrain


3. Triggers


3.1 Off-Path dying-trigger


3.2 Patroling-trigger


3.3 Shooting-trigger


3.4 Die by touching enemys-trigger


3.5 Revive at checkpoint-trigger


4. The End

##
 1. What's a Maze?


A Maze is a custom map, in which you mostly have to follow a provided path without touching the ground or enemys. Often you control a hero with has good speed and is agile. Very often, this is the classic Night-Elven Demonhunter. On the path there are patroling units which kill you if you touch them. So you have to AVOID touching them


Then often you have units like mortars who shoot at the path so that you have to have good timing if you don't want to run in the shot. Mostly the mortars have so much damage, that they kill you with one shot. At least you often have checkpoints after each "Level" of the Maze. If you reach it, normally all heroes will be revived there. Sometimes you have continues so if all of you die you restart at last checkpoint until you have no more continues...


Here I show you a mini-map of a Maze (Polar Escape by LimiteR):


Ok, now you know what a Maze is. Hurry and have a look on the next step

##
 2. Making the Terrain


The terrain of a Maze is very important. It shouldn't be too boring (e.g. only darkness) but it also shouldn't have every terrain-type next to each other (e.g. grass next to snow next to dirt and so on). Of course if you want to make a Maze like "World of Maze" or something like this you could or should use many terrain-types, but the players should know where they are safe and where they die...


Here I will show you a good terrain-type:


You see that its clear where you can walk and where you die. The grass is safe but the earth is deadly. So you have to walk carefully on the grass. Next I will show you a terrain thats not good:


Nobody will understand which terrain-type kills and which is safe. Maybe more than one type will kill? The players will have to try it out which costs them a continue or some heroes. Would be a bad start for your Maze. And I think you don't want to tell them in fact which ground kills and which is safe. They should understand it on their own, so: Keep it clear!

##
 3. Triggers


Triggers are very important in a Maze (like in 99.9% of all custom maps^^).


Kill the heroes if they come in range with an enemy or kill them if they go off of path. All this will be shown in the next steps...

##
 3.1 Off-Path killing-trigger


This trigger you really need in ALL kinds of Mazes. If players touch the ground which they should avoid of touching: They DIE!


Making this is a very easy trigger but it will be shown here, though:


(The easiest way is to make it with regions. Make regions where the heroes should die)

Off Path
Events
Unit - A Unit enters <DeathRegion1>

Unit - A Unit enters <DeathRegion2>

Unit - A Unit enters <DeathRegion3>

Conditions
(Unit-Type of (Triggering-Unit)) equals to <[YourPlayerHeroes]>

Actions
Kill <TriggeringUnit>

Of course there are other ways to make this regions, for example with the JASS Region On Die Quickwrite, but in the tutorial we will use the "basic" method.

##
 3.2 Patroling-trigger


In a Maze you have units which patrol on specific points on the map. Often they are controlled by Player 12 (Brown), but it can be whatever you want until its an ENEMY of all players. Also the units should be controlled by the Computer.


Now I'll show you the patroling-trigger:


(The enemys which patrol should ALL be invulnerable. Just go to Object-Editor [F6] and give them the "Invulnerable(Neutral)" ability)

Patrol 1
Events
Game - Initialization

Conditions

Actions
Unit - Order <A_Unit> to <Patrol> to <OnePatrolingRegion>

Now you have a unit which patrols all the time between it's start-position and the region you have made. Good job, go to next step!

##
 3.3 Shooting-trigger


The shooting-trigger is needed if you want to make mortars or other splash-attack units to shoot on the ground so that its a obstacle for players. You can make the units shooting permanently or shoot at specific events. (e.g. a unit enters a region)

Shots
Events
Time - Periodic Time Event - (Every (4 seconds) of game time)

Conditions

Actions
Unit - Order <Mortar_1> Targeting <ShootRegion_1>

You see that you have to make a region where the units shoot on. Of course you can also make many units shoot at one region. How you make the obstacles is your choice!

##


3.4 Die on touching enemys-trigger


Thats maybe the most important trigger after dying by touching forbidden terrain. You have to make that if a player's unit touchs an enemy's unit, the player's unit dies. This is a trigger which you have to make for each player.


It will look like this:

Touch_Enemys_Red
Events
Unit - A Unit comes withing <75> of <Reds_Hero>

Conditions
(Owner of (Triggering-Unit)) equals to <Enemy[Player12]>

Actions
Unit - Kill <Reds_Hero>

Now if red's hero touchs a monster controlled by the computer, his/her hero will die. Good job, you almost beated this tutorial. Two more steps to go...

##
 3.5 Revive at checkpoint-trigger


Also a very important trigger is the revive at checkpoint-trigger. In this trigger you make sure, if a player reaches a new checkpoint, all living heroes will die, and ALL heroes will respawn at the new checkpoint. For this trigger we need three variables:

Playing_Players - Real-Variable (No array, start-value[Number of maximal players])

Hero_Count - Real-Variable (No array, start-value[Number of Players/Heroes)

Continues - Real-Variable (No array, start-value[Number of continues you want to give the players]


You open the variable-editor by clicking on the big "X" in the Trigger-Editor [F4]. The following trigger will set the Playing_Player variable to the number of currently playing players . Let's say in your Maze can play 8 Players. Then your trigger looks like this:

Number_of_Players
Events
Time - Elapsed game-time equal to <0.01> seconds

Conditions

Actions
For each (Integer A) from 1 to <number of players>
Do - Actions
If/Then/Else - If Player(Integer A) slot status doesn't equal to <Playing> then do <Variable - Set Playing_Players = (Playing_Players) - 1

Now in the Playing_Players variable you have stored the number of playing players. Next you have to make a trigger for each player, if they leave during game:


(And you REMOVE their hero)

Player_1_Leaves
Events
Player - player leaves game

Conditions

Actions
Variable - Set Playing_Players = (Playing_Players) - 1

Unit - Remove (Reds_Hero) from the game

Next you have to make that everytime the Hero_Count variable becomes 0 (means no more heroes are living), you lose a continue and if you have more than one continue, they must be revived. Do this:

Hero_Count_Reaches_0
Events
Game - Value of Hero_Count becomes = 0

Conditions

Actions
If/Then/Else
If - conditions
If (Continues) bigger than 0

Then - actions
Hero - Revive (Red's_Hero) instantly at (LastCheckpointReached)

Hero - Revive (Blue's_hero) instanty at (LastCheckpointReached)

-------- etc --------

Else - Actions
For integer a from 1 to <number of players>
Game - Defeat Player(Integer A) with the message (Your_Message)

Now you have to make the last two triggers. The first one is to reduce Hero_Count variable by 1 if a hero dies. This will be done like this:

Red_Hero_Dies
Events
Unit - (Reds_Hero) dies

Conditions

Actions
Variable - Set Hero_Count = Hero_Count - 1


This trigger you make for each player. Now we come to the last trigger, the reviving at checkpoints. After we made the last triggers this can be easily done:

Checkpoint_1
Events
Unit - Unit enters <Checkpoint_1>

Conditions
(Unit-type of (Triggering-Unit)) equals to (YourHeroType[e.g.Demonhunter])

Actions
Unit-Group - Pick every unit in (Units of type YourHeroType) and do (Kill - Picked Unit)

Wait 1 second

Hero - Revive (Reds_Hero) instantly at <Checkpoint_1>

Hero - Revive (Blues_Hero) instantly at <Checkpoint_1>

-------- //[Do this for all heroes]\\ --------

Variable - Set Hero_Count = Playing_Players

After this, the entering hero and all other heroes die and will be revived at the checkpoint. But the heroes of the left players won't be revived because they have been removed before. So you did everything necessary that has to be done for a nice maze!

##
 4. The End


Congratulations!!!


You have made it through this tutorial (finally ) and you know the basics about making a Maze now. Now you can make any triggers that make difficulties for the players. Your creativity is needed here. Experiment with triggers who create units, make obstacles and so on. Nothing is forbidden!


But in the end, I would give you some more tips:


1. Don't make the levels TOO long. Nobody wants to play an endless level!


2. Don't forget to make many checkpoints if your Maze is hard. Anyways, do as many checkpoints as levels you have.


3. Think of fun! Don't make endless circles of path which patrolers on it. Think of new triggers which makes the game interesting (If you need some of them, you could also leave a comment or send me a message...)


4. Have fun by making your Mazes!!!


(I hope this tutorial could help you. If you have suggestions post me )


 Last edited by a moderator: Apr 14, 2009

#### VeljkoM

#### VeljkoM

 Level 21

JoinedJul 27, 2008Messages14,361

Resources: 4

#### Resources
Maps 3Tutorials 1

It is quiet good.Very useful and simple.

But may I say that you can also give enemy units creature "wander" ability.This way they will move random so people can't predict their movement.

#### varsaigen

#### varsaigen

 Level 24

JoinedJan 25, 2008Messages7,782

Resources: 27

#### Resources
Models 24Skins 3

oh this will be helpful! Good job! =D I always wondered how they did it. You make it look so simple too XD


I sense this will be helpful for anyone who wants to make a maze and may be new to the concept of WE mape making  A truly wonderful and helpful tutorial =D


Edit: Just saw VeljkoM's post and that reminded me. A possible, key to door, fire to torch thing could be useful too  or...... elemental torches! =D

#### nerovesper

#### nerovesper

 Level 13

JoinedSep 29, 2008Messages672

Resources: 2

#### Resources
Maps 1Spells and Systems 1

This gives me an idea of making a map like this. ^^

#### DarkLordX

#### DarkLordX

 Level 7

JoinedDec 3, 2008Messages189

Resources: 6

#### Resources
Maps 1Tutorials 5

Big THX you all. All my tutorials I made since now were...bad.


@Veljkom:


Yes, I know this would be easier, but always when I play a Maze I HATE it if they walk random...Then it isnt a real maze cuz you cant rush or something like this. I think on this way it's quite better (and looks better )

#### VeljkoM

#### VeljkoM

 Level 21

JoinedJul 27, 2008Messages14,361

Resources: 4

#### Resources
Maps 3Tutorials 1

Well it is nice to combine both ways.But you must make wandering unit to be able to die.

#### TheBlooddancer

#### TheBlooddancer

 Level 22

JoinedJun 24, 2008Messages3,041

Resources: 2

#### Resources
Maps 1Tutorials 1

Pretty nice, but there is a better way than regions to kill units in a maze.

Try look in spell section.

You might also wanna add how to make a slide.

(Look in tutorials if you don't know)

#### varsaigen

#### varsaigen

 Level 24

JoinedJan 25, 2008Messages7,782

Resources: 27

#### Resources
Models 24Skins 3

told ya this tutorial would inspire people =D

#### DarkLordX

#### DarkLordX

 Level 7

JoinedDec 3, 2008Messages189

Resources: 6

#### Resources
Maps 1Tutorials 5

thx versaigen^^

@blooddancer:

yes, i know it but at first and for a BEGINNERS tutorial thats enough i think

cuz sliding is a bit more complicated...

#### varsaigen

#### varsaigen

 Level 24

JoinedJan 25, 2008Messages7,782

Resources: 27

#### Resources
Models 24Skins 3

oh no! Not the double posting! D: Please merge posts. D: Mods do reveiw the tutorials, and I have seen people get neg repped for somethig as simple as doubleposting wheras others were not :/

#### SB-Remix

S

#### SB-Remix

Hmm I like this tutorial it has already helped me


good job (+rep)

#### manstie

#### manstie

 Level 12

JoinedMar 10, 2008Messages869

Resources: 4

#### Resources
Icons 1Maps 1Spells 1Tutorials 1

With the attacking place, i think you would need them to attack ground with that and make the unit have splash otherwise it will be 1 tiny spec that it will kill the unit

there are easier ways to do things when a unit goes off a terrain to kill them, instead of using 500 regions for it.

#### Wizzard-of-Hell

W

#### Wizzard-of-Hell

Wow! Cool tutorial!! It may will help me a lot!! +rep

#### Ricardo Irving

#### Ricardo Irving

 Level 9

JoinedMay 10, 2009Messages542

DarkLordX, I'm not sure if Blooddancer was talking about this, but you can use periodic time [every 0.01 seconds], use a condition which is about If The Unit is on This Type of Terrain etc, something like that, then Action - Unit - Kill The Unit [yes, this doesn't sound like the editor, but there's something like that, I dunno, condition - terrain type, and The Unit is what the unit is in the maze - you can use variables for multiplayer ease like Integer - Player Number, well I'm rambling, anyway, hope you get the first bit.

#### -Sh0ck3r-

#### -Sh0ck3r-

 Level 7

JoinedJun 29, 2008Messages156

Resources: 8

#### Resources
Maps 7Spells 1

Great!

#### manstie

#### manstie

 Level 12

JoinedMar 10, 2008Messages869

Resources: 4

#### Resources
Icons 1Maps 1Spells 1Tutorials 1

It isn't that great.

Work your on English well as please.

#### Moonire

M

#### Moonire

Umm I don't get the part that tell me to fill in lastcheckpoint reach.... I can't seem to be able to find that option anyway good job: Hero_Count_Reaches_0


 Events


 Game - Value of Hero_Count becomes = 0


 Conditions


 Actions


 If/Then/Else


 If - conditions


 If (Continues) bigger than 0


 Then - actions


 Hero - Revive (Red's_Hero) instantly at (LastCheckpointReached)


 Hero - Revive (Blue's_hero) instanty at (LastCheckpointReached)


 -------- etc --------


 Else - Actions


 For integer a from 1 to <number of players>


 Game - Defeat Player(Integer A) with the message (Your_Message

#### UnderCover_Spy

U

#### UnderCover_Spy

Custom TKILL

Events
Time - Every 0.05 seconds of game time

Actions

Unit Group - Pick every unit in Units and do (Actions)

Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Picked unit) is alive) Equal to True

Then - Actions
Set HeroPosition[0] = (Position of (Picked unit))

Set HeroOnPath = False

For each (Integer A) from 0 to 3, do (Actions)
Loop - Actions
Set HeroPosition[1] = (HeroPosition[0] offset by BugDistance towards (90.00 x (Real((Integer A)))) degrees)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
And - All (Conditions) are true
Conditions
(Terrain type at HeroPosition[1]) Not equal to PathTerrain

Then - Actions
Set HeroOnPath = True

Else - Actions

Custom script: call RemoveLocation(udg_HeroPosition[1])

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
HeroOnPath Equal to False

Then - Actions
Unit - Kill (Picked unit)

Else - Actions

Custom script: call RemoveLocation(udg_HeroPosition[0])

Else - Actions

RaWR!!!!!!!!!

#### UnderCover_Spy

U

#### UnderCover_Spy

Events
Unit - A unit enters Region w/e <gen>

Conditions
(Unit-type of (Triggering unit)) Equal to MazerUniT

ctions
Unit - Pause all units

Player Group - Pick every player in (All players) and do (Camera - Pan camera for (Picked player) to (Center of Region w/e <gen>) over 1.20 seconds)

Unit Group - Pick every unit in (Units of type Demon Hunter) and do (Unit - Kill (Picked unit))

Wait 2.00 seconds

Hero - Set (Triggering unit) Hero-level to ((Integer((String((Level of (Triggering unit)))))) + 1), Hide level-up graphics

Unit Group - Pick every unit in (Units of type Demon Hunter) and do (Hero - Instantly revive (Picked unit) at (Center of Region w/e <gen>), Hide revival graphics)

Unit - Unpause all units

Trigger - Turn off (This trigger)

Useing this trigger you wont have to make a Action to revive every dh one by one.

#### UnderCover_Spy

U

#### UnderCover_Spy

Events
Time - Every 2.00 seconds of game time

Actions
Set Collision = 60

Set PlayerPatrols = (Units owned by Player 12 (Brown))

Unit Group - Pick every unit in PlayerPatrols and do (Actions)
Loop - Actions
Trigger - Add to Collisions <gen> the event (Unit - A unit comes within (Real(Collision)) of (Picked unit))

-------------------------------------------------------------------------

Events

Conditions
(Current flying height of (Triggering unit)) Less than or equal to 0.10

(Owner of (Triggering unit)) Not equal to Player 12 (Brown)

Actions
Unit - Kill (Triggering unit)

Set your mazers roll pitch angle to 11.


These are some of the triggers i use in my mazes.


 Last edited by a moderator: Nov 12, 2009

#### kimberker

#### kimberker

 Level 3

JoinedNov 8, 2009Messages46

Hey,

Step 3.2

" Game - Initialization " and " Unit - Order <A_Unit> to <Patrol> to <OnePatrolingRegion "

Step 3.3

" Unit - Order <Mortar_1> Targeting <ShootRegion_1> "

Step 3.4

" (Owner of (Triggering-Unit)) equals to <Enemy[Player12]> "

Step 3.5

" If/Then/Else - If Player(Integer A) slot status doesn't equal to <Playing> then do <Variable - Set Playing_Players = (Playing_Players) - 1 "

HOW TO DO?!?!?!

On top of that theres " Do - Actions " Mine is " Loop - Action "

Best u make it in a video.

I cant find them...Help me


 Last edited: Dec 9, 2009

#### SteakedMeat

#### SteakedMeat

 Level 4

JoinedJun 4, 2009Messages70

the death thingy i tihnk is to much work, there is a trigger that will trigger a choosen terrian, grass for example, to kill entering unit.

#### Bloodspine

#### Bloodspine

 Level 3

JoinedMay 21, 2009Messages35

To the point and very easy  Helped ALOT! I might even post a map with you help pretty soon. Again thank you.

#### ZXavier88Z

#### ZXavier88Z

 Level 2

JoinedMay 21, 2010Messages15

Dude How can I make slidey Ice

#### Shantel

S

#### Shantel

Woulden't it be easier to make a Terrian killer, so people doesen't have like 650 regions etc ?

#### XAlexX

#### XAlexX

 Level 2

JoinedJun 16, 2010Messages22

Good,Nice and Usefull in same time!


Great Job!

#### Vorazim

#### Vorazim

 Level 3

JoinedMar 6, 2010Messages48

Thanks is rly helpful

#### DarkLordX

#### DarkLordX

 Level 7

JoinedDec 3, 2008Messages189

Resources: 6

#### Resources
Maps 1Tutorials 5

After a very long periode of being inactive, I came up with Warcraft 3 again

Shantel said:

 Woulden't it be easier to make a Terrian killer, so people doesen't have like 650 regions etc ?


Click to expand...


Yes, but then 1. You have to stick to the terrain and 2. The Mazers can be killed very stupidly. So if they only touch the wrong path a bit they immediately die which sucks a lot :/


@all (or many^^):


Thanks for your nice comments

#### jacobemslayer

#### jacobemslayer

 Level 5

JoinedJan 15, 2009Messages75

Resources: 4

#### Resources
Skins 1Tutorials 3

Events
Time - Every 0.03 seconds of game time

Conditions

Actions
If ((Terrain type at (Position of Hero[1])))Equal to Lordaeron Summer - Dirt) then do (Unit - Kill Hero[1]) else do (Do nothing)

Maybe thats easier, you should include it in your tutorial.. But you don't have to

 You must log in or register to reply here.


### Similar threads

Locked

 Continues


shadow487

Apr 7, 2010

World Editor Help Zone

Replies5Views1K

Apr 8, 2010
Arcsol

 Maze Making Tutorial For Beginners


DestroAnt

Apr 26, 2006

Tutorial Archive

Replies4Views4K

Mar 12, 2007
Wolverabid

 Reviving Heroes at a Changeable Region


dOT145

Mar 23, 2010

Trigger (GUI) Editor Tutorials

Replies12Views10K

Apr 30, 2010
PurgeandFire

 WIP: Maze Making.


HeretoDLstuff

Aug 19, 2007

Tutorial Archive

Replies26Views7K

Dec 10, 2007
PurplePoot

Poll

[Mapping]
 How to make a maze + tips


jordon

Dec 23, 2010

Tutorial Archive

Replies12Views2K

Jan 8, 2011
-Kobas-

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Trigger (GUI) Editor Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top