---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/creating-a-turn-based-battle-system.107252/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Creating a Turn Based Battle System
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/creating-a-turn-based-battle-system.107252/>
#### Avator

#### Avator

 Level 18

JoinedAug 23, 2008Messages2,319

Resources: 3

#### Resources
Maps 1Tutorials 2

This tutorial will guide you through making an easy turn-based battle system for a single player game. In this tutorial I will go for a system as used in Final Fantasy which uses an Active Time Battle gauge which fills over time and allows a unit to perform an action when full.

Tutorial:

Tutorial dificulty: 2/10

Requirements:

Basic triggering knowledge

Basic variable knowledge

Able to read

Variables used:

Boolean:

-BattleMode (Checks if the unit can enter a battle)

Integer:

-RANDOM (Random number from 1 to 100 to indicate a chance)

-AllyATB (The ATB points that are gained over time while in battle)

Point:

-BattleRoomPosition (The points in the battle room where the units will stand by default)

-BattleLocation (A variable which refers to the location of where the unit came from)

Unit:

-ActiveCharacter (The unit that will do a command)

-TempTarget (The targeted unit to attack-once when ordered to be attacked)

-AllyBattleUnit (The units that form AllyBattleGroup)

-EnemyBattleUnit (The units that form EnemyBattleGroup)

Unit Group:

-BattleGroup (Contains the units with which you fight in a battle)

-AllyBattleGroup (Contains units that can activate the Entering Battle Mode trigger)

-EnemyBattleGroup (Contains the units you fight)

System:

Preperations:

Set aquisition range of all units to 0 in the Object Editor, so they can't attack by themselves. If you need units with a aquisition range, you need to create units that are only for battle and load their info at the Entering Battle Mode 2 trigger. Also set the units you want to use in battle to AllyBattleUnit[x]. Create as many arrays for this unit variable as you want to indicate the maximum size of a party.

Step 1: Entering battle

First of all, you need to make a trigger that activates the Battle Mode, which is the mode that activates when you enter a turn-based battle. For A Final Fantasy game, that will be a random moment while moving.

Entering Battle Mode 1
Events
Unit - A unit is issued an order targeting a point

Unit - A unit is issued an order targeting an object

Conditions
(Triggering unit) is in AllyBattleGroup

Actions
Set BattleMode = True

Entering Battle Mode 2
Events
Time - Every 1 second of the game time

Conditions
BattleMode equal to True

Actions
Set RANDOM = (Random number between 1 and 100)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
RANDOM Less than or equal to 10

Then - Actions
Trigger - Turn on Battle Preperations <gen>

Trigger - Turn off Entering Battle Mode 1 <gen>

Trigger - Turn off (This trigger)

Else - Actions

Battle Preparations is a trigger that contains all preparations before the actual battle starts. Like the ATB Gauge, battle camera's, etc. Whatever you want to have as preparations before battle can be made here. You do however need to set all units you're going in battle with in the BattleGroup Unit Group. Also change ownership of them to Neutral Passive, so you can't control them when it's not their turn. An example of a Battle Preparations trigger is:

Battle Preparations
Events

Conditions

Actions
Unit - Move AllyBattleUnit[1] instantly to BattleRoomPosition[1], facing (Center of Battle Room <gen>)

Unit - Move AllyBattleUnit[2] <gen> instantly to BattleRoomPosition[2], facing (Center of Battle Room <gen>)

Unit - Move AllyBattleUnit[3] instantly to BattleRoomPosition[3], facing (Center of Battle Room <gen>)

Camera - Apply Battle Camera 1 <gen> for Player 1 (Red) over 2.00 seconds

Camera - Apply Battle Camera 2 <gen> for Player 1 (Red) over 1.00 seconds

Camera - Apply Battle Camera 3 <gen> for Player 1 (Red) over 3.00 seconds

Camera - Apply Battle Camera 4 <gen> for Player 1 (Red) over 2.50 seconds

Trigger - Turn on ATB Gauge <gen>

Set AllyATB[1] = 0

Set AllyATB[2] = 0

Set AllyATB[3] = 0

Step 2: ATB Gauge

The Active Time Battle Gauge is a bar that indicates at what point a unit is ready to perform an action. The bar slowly fills and when it's full, you're able to command your unit. There are plenty of other ways to order units 1 by 1 to perform an action, but to give you a little idea of the situation, this is how you could make an ATB gauge trigger:

ATB Gauge
Events
Time - Every 0.50 seconds of game time

Conditions

Actions
For each (Integer A) from 1 to (Number of units in AllyBattleGroup), do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(AllyBattleUnit[(Integer A)] is alive) Equal to True

Then - Actions
Set AllyATB[(Integer A)] = (AllyATB[(Integer A)] + 5)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
AllyATB[(Integer A)] Greater than or equal to 100

Then - Actions
Trigger - Turn on Command 1 <gen>

Trigger - Turn on Command 2 <gen>

Trigger - Turn on Command 3 <gen>

Trigger - Turn off (This trigger)

Set ActiveCharacter = AllyBattleUnit[(Integer A)]

Else - Actions

Else - Actions

This is just an example and there are many other ways to do this. You can use anything you want as long as you keep those last 5 Actions in your trigger.

Step 3: Command

Now you have a unit selected when he's ready to perform a command. The most easy way is by selecting the unit and allowing it to perform only 1 actions.

Command 1
Events

Conditions

Actions
Unit - Change ownership of ActiveCharacter to Player 1 (Red), change color.

Selection - Select ActivaCharacter for Player 1 (Red)

Set BattleLocation = Position of ActiveCharacter

Command 2
Events
Unit - A unit finished casting an ability

Conditions

Actions
Unit - Change owner of ActiveCharacter to Neutral Passive

Unit - Order ActiveCharacter to move to BattleLocation

Trigger - Turn off Command 1 <gen>

Trigger - Turn off Command 2 <gen>

Trigger - Turn off (This trigger)

Command 3
Events
Unit - A unit is issued an order targeting an object

Conditions
(Issued order) equal to Attack

Actions
Set TempTarget to (Attacked unit)

Unit - Order ActiveCharacter to Attack-Once TempTarget

Player - Change owner of ActiveCharacter to Neutral Passive

Wait until (current order) of ActiveCharacter is order(stop), checking every 0.10 second

Unit - Order ActiveCharacter to move to BattleLocation

Trigger - Turn off Command 1 <gen>

Trigger - Turn off Command 3 <gen>

Trigger - Turn off (This trigger)

Step 4: Victory & defeat

Ofcourse you need to set when you win and when you lose. You use a simple trigger for this.

Victory and defeat
Events
Unit - A unit dies

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Dying unit) is in EnemyBattleGroup

Then - Actions
Unit Group - Remove (Dying unit) from EnemyBattleGroup

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Dying unit) is in AllyBattleGroup

Then - Actions
Unit Group - Remove (Dying unit) from AllyBattleGroup

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
EnemyBattleGroup is empty

Then - Actions
Trigger - Turn off ATB Gauge <gen>

Trigger - Turn off (This trigger)

Trigger - Turn on Entering Battle Mode 1 <gen>

Trigger - Turn on Entering Battle Mode 2 <gen>

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
AllyBattleGroup is empty

Then - Actions
Trigger - Turn off ATB Gauge <gen>

Trigger - Turn off (This trigger)

Else - Actions

In addition to the last 2 If/Then/Else commands, you need to add the triggers that do when you want when you won or lost a turn-based battle. This can be 'Game - Victory' or 'Game - Defeat', but you can also just go on and activate another party when you lose, or you continue with the game when you win.


 Last edited: Dec 9, 2008

#### TheBlooddancer

#### TheBlooddancer

 Level 22

JoinedJun 24, 2008Messages3,041

Resources: 2

#### Resources
Maps 1Tutorials 1

You lack a 'Unit - ' In Command 3, and shouldnt Unit - A unit is issued an order targeting no object also be added as an event (Such as stomp, stop, channel and so on)

And you might want to create a small test map, so that people could see what they are going to create.

Im not the reviewer type, but i would say that if you added more commands, and such, so that people easier could create a minigame, this would stand for my approval.

#### Avator

#### Avator

 Level 18

JoinedAug 23, 2008Messages2,319

Resources: 3

#### Resources
Maps 1Tutorials 2

Fixed the 'Unit - ' thing.

It's well possible that a player wastes his turn because he uses Move and is marked as an action and abilities with no target are already marked under 'Finishes casting an ability'. So those triggers are correct. If the player inserts any abilities or actions, he/she should check if it's still compatible with this system.

I want to create a test map, but to be able to test it, you need an ATB system, which takes a while to make. If you got any idea's on a system that can be used for this, please tell me so I can create the test map.

I'm already working on a battle demo for my Final Fantasy IX campaign, so if I got no suggestions for a system for the test map, then I'll use that demo as test map.

This is a base for a turn-based system. triggers and commands can be added by the demands of the player to make it work for whatever they want to use it for. I don't think it's a good idea to focus this tutorial on 1 game type, like an RPG, mini-game, strategy game or anything else. So I keep it with a base for an easy to use turn-based system.

Thanks for your time to check this out and thanks for reminding me of the bugs in it

#### Kratos1987

#### Kratos1987

 Level 5

JoinedJul 13, 2008Messages109

Resources: 3

#### Resources
Maps 3

hey, i can't seem to find the trigger


Player - Force Player 1 (Red) to select ActiveCharacter (command 1)


Also, i think it'll help alot if you also gave triggers for "ATB gauge" and "Battle Preperations"

#### Avator

#### Avator

 Level 18

JoinedAug 23, 2008Messages2,319

Resources: 3

#### Resources
Maps 1Tutorials 2

Sorry about that, it seems to be

Selection - Select ActiveCharacter for Player 1 (Red)
I will add a trigger that will make clear what the ATB Gauge and Battle Preperations triggers are about.

EDIT: The ATB Gauge and Battle Preparations triggers have been added.


 Last edited: Dec 9, 2008

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

- Might want to check for leaks...

- I find it rather limiting. Good for a simple map, but most maps that would use a battle system like this are rather complex.

Overall rating: 2.5/5

#### Avator

#### Avator

 Level 18

JoinedAug 23, 2008Messages2,319

Resources: 3

#### Resources
Maps 1Tutorials 2

For some reason I always get an error when I want to remove leaks, so I'm not going to go through all that again.

If you could tell me what I should improve, I can improve it. Just 'rather limiting' doesn't give me a good view of what I need to change...

#### Kratos1987

#### Kratos1987

 Level 5

JoinedJul 13, 2008Messages109

Resources: 3

#### Resources
Maps 3

Sorry to bother you again, just wondering. would you need a trigger for battles to start from moving around (like in Final fantasy games) with a random battle. and another trigger to raddomly produce enemy units and the amount of enemy units?

#### Avator

#### Avator

 Level 18

JoinedAug 23, 2008Messages2,319

Resources: 3

#### Resources
Maps 1Tutorials 2

The randomly encountering of enemies like in the Final Fantasy games is shown in the Entering Battle Mode triggers. If you want to create random opponents, you need to add those to Battle Preparations like this:

Set RANDOM = Random number between 1 and 7

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
RANDOM equal to 1

Then - Actions
Unit - Create 1 Footman at BattleRoomPosition[4], facing center of Battle Room

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
RANDOM equal to 2

Then - Actions
Unit - Create 1 Rifleman at BattleRoomPosition[4], facing center of Battle Room

Else - Actions

Here, I took RANDOM = Random number between 1 and 7, but 7 is just an example. Range it to how many different enemy types you're able to encounter in that area.

I took BattleRoomPosition[4], because in the tutorial there were 3 positions taken. It might be useful to set 2 arrayed Point variables for this: 1 for allies and 1 for enemies.

Repeat the If/Then/Else trigger for every unit type you can encounter, like I did above with a Footman or Rifleman.

Once you've done that, copy and paste it for the maximum amount of opponents you can battle at once. That means the first set of Actions (as displayed above) is for the first enemy in the enemy group, and the copies you make are for the 2nd, 3rd, etc. unit is the enemy group.

#### imek

#### imek

 Level 3

JoinedAug 18, 2016Messages42

I can't find any of the conditions for the victory and defeat section. I'm a noob please keep that in mind. Can someone please direct me to where I can locate the conditions?

 You must log in or register to reply here.


### Similar threads

Locked

 Turn Based Battle System


Radicool

Sep 26, 2014

Requests

Replies0Views2K

Sep 26, 2014
Radicool

 Unit movement with keyboard(GUI Trigger)


NiX_GR_

Sep 8, 2007

Tutorial Archive

Replies2Views2K

Sep 9, 2007
Wolverabid

Locked

[Trigger]
 Turn Based Battle System


Radicool

Sep 27, 2014

Triggers & Scripts

Replies5Views2K

Sep 28, 2014
Radicool

 This is a good way to make a orpg


jordon

Jul 9, 2009

Tutorial Archive

Replies4Views1K

Jul 16, 2009
jordon

[GUI]
 Scrolling hero selction


mj1994

Oct 31, 2009

Tutorial Archive

Replies0Views1K

Oct 31, 2009
mj1994

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Tutorial Archive

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top