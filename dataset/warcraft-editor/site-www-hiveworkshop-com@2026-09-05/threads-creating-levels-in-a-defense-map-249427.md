---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/creating-levels-in-a-defense-map.249427/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Creating Levels in a Defense Map
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/creating-levels-in-a-defense-map.249427/>
#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

#

 Creating Levels in a Defense Map

As I was checking the maps in the pending map's section, I found some defense/tower defense maps having so many triggers for levels. This is very tasky that is why, I intend to create this simple tutorial on how to create levels or spawns with few triggers at hand.

## The requirements:
1. World Editor


2. Basic knowledge in triggering.

## Contents

Initialization of the Game

Setting Up the Creeps

Initial Spawns

The Creep Levels or Creep Spawns

End Level

Next Level

Download the Demo Map

## The Tutorial

### Initialization of the Game


The triggers below are included during the initialization of the first spawns

Init Trigger

inittialize game
Events
Time - Elapsed game time is 0.01 seconds

Conditions

Actions
-------- For the game initialization, it is a wise discission to initialize using time elapse --------

-------- because the timer window will not show up if you use map initialization event --------

-------- ==================================================== --------

-------- 1. Run the two essential triggers in creating the first spawns (Level 1) --------

Trigger - Run Set Creeps <gen> (checking conditions)

Trigger - Run firstSpawnTimer <gen> (checking conditions)

Setting Up the Creeps


Before we begin everything on creating levels, we must set up our creeps. The variables that you should have here are:


1. creepSpawnType[] - Unit-Type Array() -- Sets the kind of creeps to spawn in each level


2. goldLevelGiven[] - Integer Array () -- sets the gold given to the player after each round.


3. gold_LevelText[] - String Array() -- sets the text of gold amount recieved by the player after each round


4. creepNumLevel[] - Integer Array() -- Sets the number of creeps to spawn in each level.


5. levelWarnings[] - String Array() -- Sets the warning text to show to the player every time a new level is spawned.


6. LevelCounter - Integer None-Array -- Counts the number of spawns/levels.


See the example triggers below.

Set Creeps
Events

Conditions

Actions
-------- I would like to start counting from one --------

-------- You can start yours from zero --------

Set LevelCounter = 1

-------- This actions set the kind of creeps to spawn in every level, the number of creeps to spawn, the golds given --------

-------- at each end of wave, and the texts for warnings and the given gold --------

Set creepSpawnType[1] = Spider

-------- creepSpawnType is a unit type --------

Set goldLevelGiven[1] = 150

-------- goldGiven is integer --------

Set goldLevelText[1] = ((String(goldLevelGiven[1])) + is given.)

-------- goldLevelText is a String --------

Set creepNumLevel[1] = 3

-------- creepNumLevel is the creep count to spawn and is an integer --------

Set levelWarnings[1] = ((String(creepNumLevel[1])) + Level 1 creeps are coming.)

-------- levelWarnings warns the player about the coming creeps --------

-------- ==================NEXT LEVEL======================================== --------

Set creepSpawnType[2] = Satyr Trickster

Set goldLevelGiven[2] = 350

Set goldLevelText[2] = ((String(goldLevelGiven[2])) + is given.)

Set creepNumLevel[2] = 6

Set levelWarnings[2] = ((String(creepNumLevel[2])) + Level 2 creeps are coming.)

-------- ==================NEXT LEVEL======================================== --------

Set creepSpawnType[3] = Murloc Tiderunner

Set goldLevelGiven[3] = 250

Set goldLevelText[3] = ((String(goldLevelGiven[3])) + is given.)

Set creepNumLevel[3] = 8

Set levelWarnings[3] = ((String(creepNumLevel[3])) + Level 3 creeps are coming.)

### Initial Spawns


These triggers show you how to create the timer for the initial spawn of creeps or the first level.

First Spawn Timer

firstSpawnTimer
Events

Conditions

Actions
-------- In our first spawn, we need to create a different timer and a timer window aside from the timer window to use in a regular level or spawns. --------

Countdown Timer - Start startTimer as a One-shot timer that will expire in 5.00 seconds

Countdown Timer - Create a timer window for startTimer with title Next Level in:

-------- We should store our timer window in a variable because we have two timer windows --------

Set startTimerWindow = (Last created timer window)

-------- Now, we will show our timer window to the player to give him time to prepare --------

Countdown Timer - Show startTimerWindow

First Spawn Timer Expires

firstSpawnEnd
Events
Time - startTimer expires

Conditions

Actions
-------- If our firstSpawnTimer expires we should do the following: --------

-------- 1. Hide or even destroy our first spawn--timer window. --------

Countdown Timer - Hide startTimerWindow

-------- 2. Tell the player that the first level is approaching --------

Game - Display to (All players) for 5.00 seconds the text: levelWarnings[LevelCounter]

-------- 3. Turn on the creation of creeps --------

Trigger - Turn on Spawn <gen>

### The Creep Levels or Creep Spawns


As the game progresses, these three triggers function. I have the trigger commented for better understanding.

Spawn


Triggers for spawning the creeps

Spawn
Events
Time - Every 0.05 seconds of game time

Conditions

Actions
-------- This trigger is initially off. --------

-------- First thing to do is to store the locations where the spawn should occur and where our creeps will go --------

-------- 1. The spawn location --------

Set Spawner = (Random point in spawnArea <gen>)

-------- 2. The location where our creeps will go --------

Set tempPoint = (Random point in attackArea <gen>)

-------- By using the if statement below, we set as to how many are allowed to spawn --------

-------- in each level --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Number of living creepSpawnType[LevelCounter] units owned by Player 12 (Brown)) Equal to creepNumLevel[LevelCounter]

Then - Actions
-------- We then turn off this trigger if the desired number of spawned creeps is meet --------

Trigger - Turn off (This trigger)

Else - Actions
-------- If the number of creeps is still not enough, the creation will still continue --------

Unit - Create 1 creepSpawnType[LevelCounter] for Player 12 (Brown) at Spawner facing Default building facing degrees

-------- We order each creeps to attack and move towards the set dump area we call as tempPoint --------

Unit - Order (Last created unit) to Attack-Move To tempPoint

-------- If everything is done, we should remove memory leaks --------

Custom script: call RemoveLocation(udg_Spawner)

Custom script: call RemoveLocation(udg_tempPoint)

-------- This ends the creation of creeps --------

You could find some more on fixing memory leaks with https://www.hiveworkshop.com/forums/triggers-scripts-269/things-leak-35124/

End Level


Trigger on how the level ends

endLevel
Events
Player - Player 12 (Brown)'s Food used becomes Less than or equal to 0.00

Conditions

Actions
-------- The level will end if all creeps are dead --------

-------- Player - Player 12 (Brown)'s Food used becomes Less than or equal to 0.00 --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
LevelCounter Less than 3

Then - Actions
-------- We have to set the last level and here we have up to level 3 --------

-------- Then, we create coundown timer for the next wave of creeps. --------

Countdown Timer - Start levelTimer as a One-shot timer that will expire in 3.00 seconds

-------- Create the timer window --------

Countdown Timer - Create a timer window for levelTimer with title Next Level in:

-------- Save it in a variable --------

Set levelTimerWindow = (Last created timer window)

-------- Show the timer window to the player so he can be alerted --------

Countdown Timer - Show levelTimerWindow

Else - Actions
-------- We put here the action when the last level is reached. Here, the last level is 3 as stated in our if statement - --------

Game - Display to (All players) for 30.00 seconds the text: NO MORE CREEPS TO S...

Next Level


Trigger for the creation of the next level which actually points to Spawn trigger

nextLevel
Events
Time - levelTimer expires

Conditions

Actions
-------- In this trigger, you can see that we work on a different expiring timer than the --------

-------- expired timer of the first spawn. This is because, this will be used for the leveling purpose. --------

-------- If the level timer expires, we need to hide it --------

Countdown Timer - Hide levelTimerWindow

-------- We then add 1 to our level (the next spawns) --------

Set LevelCounter = (LevelCounter + 1)

-------- Then we display the alert text --------

-------- Take note that the text we used is actually a variable we get in the Set Creeps trigger --------

-------- Instead of using levelWarnings[0], levelWarnings[1], etc we just say levelWarnings[LevelCounter] --------

Game - Display to (All players) for 5.00 seconds the text: levelWarnings[LevelCounter]

-------- Now, we have to turn on The Spawn trigger to create units for the next level --------

Trigger - Turn on Spawn <gen>

 useful tutorials


 You might also want to try the following tutorials which have some relationships to defense type of maps:


1. How to make a good TD from scratch


2. Ralle's Tower Defence Guide


3. Basics of a Tower Defense

Download the Demo Map

UP


 Last edited: Mar 16, 2014

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

To sum up everything, using this tutorial, you can create only 7 triggers for at least more than 40 waves of creep spawns.


 Last edited: Mar 15, 2014

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Overall it looks good and is a nice idea. From my initial reading:

Some of the trigger indenting is messed up. Also, you should clarify what a level is in a short introduction at the start of the tutorial. I usually call those "spawns" or "phases" so I didn't know what the tutorial was about at first.

#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2

It would look cleaner if you wrapped your triggers in

 tags.

and Instead of manually writing lists (1., 2., 3.) use the [list][/list] tags.

[list=1]

[*]Point one

[*]Point two

[*]Point three

[/list]

outputs:

Point one

Point two

Point three
I don't have any comments on the content of the tutorial yet, except for maybe link to https://www.hiveworkshop.com/forums/triggers-scripts-269/things-leak-35124/ in the section about removing leaks.

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

I have modified the post several times using list and other ways to solve the triggers indentation. I wasn't able to fix it.

#### Binsu

#### Binsu

 Level 30

JoinedNov 29, 2012Messages6,631

Resources: 5

#### Resources
Maps 4Templates 1

It looks great and this has potential as a tutorial. Will be helpful to those starter map maker that decided to make a Defense map.

The trigger looks organized and in my own knowledge (not saying that it is really leakless, just based on me though), it looks leakless. You've even provided a little tip and linked something about cleaning leaks which would also turn out helpful.

What I would suggest are for the thread's visuals. It would be great if you are to wrap those triggers with a stable tag as well ([stable][/stable]). Does not do much change but it shortens the space taken by the trigger. You should also cover those variable at the start in Bold font so it can be easily noticed instead of being left out. Lastly, you could also make a list where it serves as like the Table of Contents containing those steps in it. It'll give the steps an easy access just by clicking instead of scrolling down (well, we know the tutorial's just short but there are still lazy users who prefer that actually)

Good luck and good job!

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

 What I would suggest are for the thread's visuals. It would be great if you are to wrap those triggers with a stable tag as well (

). Does not do much change but it shortens the space taken by the trigger. You should also cover those variable at the start in Bold font so it can be easily noticed instead of being left out. Lastly, you could also make a list where it serves as like the Table of Contents containing those steps in it. It'll give the steps an easy access just by clicking instead of scrolling down (well, we know the tutorial's just short but there are still lazy users who prefer that actually)


Click to expand...

I will do.

EDIT:

Edited everything, updated, and done.


 Last edited: Mar 15, 2014

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Overall, I like the tutorial's attempt to generalize the spawning process so that you don't have to create so many duplicate triggers.

It looks pretty good. I suppose the only issue is that it doesn't allow for creep variation, but I think this is still approvable. Approved.

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

UPDATES:

updated adding some links to related tutorials.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

A noob recommendation :

Tell user to split the Configuration Triggers into several parts (Bounty, Message, Creeps, etc) if they intend to make long list of spawns.

 You must log in or register to reply here.


### Similar threads

Locked

[Trigger]
 Can you Find the Bug?!


VergilThazaar

Oct 10, 2009

Triggers & Scripts

Replies13Views1K

Oct 10, 2009
VergilThazaar

 Multiple Creep Spawn System


DoOs_101

Apr 12, 2007

Trigger (GUI) Editor Tutorials

Replies32Views41K

Oct 24, 2014
Chaosy

 Create a sucessfull timer with window (reuploaded)


Cheezeman

Sep 7, 2008

Trigger (GUI) Editor Tutorials

Replies27Views17K

Jan 8, 2021
impulse

 Upwards Counting Timer


Laserdude

Aug 10, 2010

Trigger (GUI) Editor Tutorials

Replies4Views12K

Jul 15, 2021
MacMardigan

 Create precise timing cinematic using real variable


Septimus

Apr 23, 2009

Trigger (GUI) Editor Tutorials

Replies17Views9K

Jun 27, 2009
Dem0n_Hunter

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