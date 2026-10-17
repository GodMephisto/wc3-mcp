---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/debugging.188204/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Debugging
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/debugging.188204/>
#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Debugging

Introduction:

A bug is a fault in code that causes it to not function properly under certain conditions. Debugging is the process of removing those bugs.

In this tutorial, you will learn how to check for errors in code and how to track down the source of the problems.

​

Debug Messages:

When debugging, your best friend will be in-game Debug Messages. Debug Messages are messages displayed in-game to show the value of something or to let you know that a trigger is running. For example, let's take a look at a trigger that will repeatedly generate a random integer.

Random Number
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Set Random = (Random integer number between 1 and 10)

Now, wouldn't you like to see what this number comes out as? To do this, simply display a message that reads the variable.

Random Number
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Set Random = (Random integer number between 1 and 10)

Game - Display to (All players) the text: (String(Random))

In-game, you will now see this:

Which is good. Now you know that the random numbers that are coming out are 7,6,1,9,3, and 10, in that order.

The message isn't particularly useful in that example, but it will be later on. Simple messages allow you to analyze what the problems are, and can lead you to a solution.

​

Simple Debugging:

Let's take a look at simple debugging. For example, you have a spell that is supposed to create an effect, and then heal the caster after a few seconds.

Heal
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Healing Wave

Actions
Set Heal_Caster = (Triggering unit)

Set Heal_Point = (Position of Heal_Caster)

Special Effect - Create a special effect at Heal_Point using Abilities\Spells\Orc\HealingWave\HealingWaveTarget.mdl

Special Effect - Destroy (Last created special effect)

Custom script: call RemoveLocation(udg_Heal_Point)

Wait 5.00 seconds

Unit - Set life of Heal_Caster to ((Life of Heal_Caster) + 75.00)

Now, this may work fine in some cases, but after debugging, you may notice that this does not work for more than one unit at the same time. The wrong unit ends up being healed!

So let us fix that. When debugging, you should first point out what the error is. In this case, the error is that the spell is not Multi-Unit Instanceable. Identifying the error is usually the hard part. In this case, however, it is a little bit easier. Each time the spell is cast, the triggering unit is assigned to the variable Heal_Caster. At the end, it heals the unit of the Heal_Caster variable. Thus, if UnitA casts the spell while UnitB is waiting on the 5 seconds for the heal, then the Heal_Caster variable will now be assigned to UnitA. UnitB will not receive the heal and UnitA will receive it twice. To fix this, you must make the spell MUI. Now, there are several ways to do this, but I'll show the simple way:

Heal
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Healing Wave

Actions
Set Heal_Point = (Position of (Triggering unit))

Special Effect - Create a special effect at Heal_Point using Abilities\Spells\Orc\HealingWave\HealingWaveTarget.mdl

Special Effect - Destroy (Last created special effect)

Custom script: call RemoveLocation(udg_Heal_Point)

Wait 5.00 seconds

Unit - Set life of (Triggering unit) to ((Life of (Triggering unit)) + 75.00)

Triggering Unit is local to the function of it being cast. Unlike global variables, triggering unit can point to different units depending on who it is executed for. In the situation above, it will heal UnitB and UnitA just fine.

​

Uninitialized Variables:

This is something that can get a lot of people to have problems, and it is very annoying to track down. For certain variables, you must initialize a value before using them. In GUI, this problem is less common, but in JASS it is very common.

Initial values are the values that variables have when they are declared. When you declare variables in GUI, this is how it will look like:

On the right is the listed initial values. For some, they have an initial value. For others, they have no initial value. In GUI, these are the variables which have initial values:

While strings say "none", they are initially declared, but as "". (essentially a null string)

Is it possible to change the initial value? Yes, for some. For others, no; at least not in GUI.

This shows an example of setting the initial value of an item-type variable. (which is actually an integer, but ignore that) If the part next to "Initial Value:" is in red text, then it is modifiable. If it is in blue text, it is not.

Now that you know what an initial value is, you can learn how to apply it. The primary variable that messes up triggers because of no initial value is the hashtable. If you are wondering why your hashtable is not working, be sure to have a trigger like this:

HashSetup
Events
Map initialization

Conditions

Actions
Hashtable - Create a hashtable

Set Object1 = (Last created hashtable)

Custom script: call DestroyTrigger(GetTriggeringTrigger())

This will make sure that your hashtable is set and ready for use. Otherwise, you are assigning data to nothing. If a variable does not have any initial value, it is regarded as null. Since variables are pointers, are variable with a null value points to nothing.

​

Lag from Leaks:

Sometimes you have lag and you just don't know why. Could it be from leaks? Bad coding? Or is it just your computer? Well, there are methods to find out. A big problem is leaks. While a couple will hardly affect your map, leaks coupled with periodics can demolish a map. Take this trigger for example:

Leaky
Events
Time - Every 0.03 seconds of game time

Conditions

Actions
Unit Group - Pick every unit in (Units within 512.00 of ((Center of (Playable map area)) offset by (0.00, 0.00))) and do (Actions)
Loop - Actions

Yep, 3 leaks. A unit group and two location leaks. However, this is executed every 0.03 seconds. That means that in 1 second, you will have 100 leaks. In 60 seconds (1 minute), you will have 6000 leaks. After 1 hour of playtime, you will have 360,000 leaks. Just from this trigger. That will definitely lag a game.

How do I know how many are being produced? Well, simple calculations. However, in a real situation you will not be able to do these simple calculations and realize what is leaking in your map. That is why we have the magnificent handle counter. What is a handle? It is a reference counted object. Most objects are handles, except for the primitive types booleans, integers, reals, strings, and codes.

When you create a handle, it has a certain ID that can be found using:

Set SomeInteger = (Key SomeHandle)
Or:

native GetHandleId takes handle h returns integer

Most handles (some exceptions such as texttags, or ubersplats) begin their allocation at 0x100000 (that is hexadecimal for the value: 1048576). It increases by one for each handle in the map, and then removed ones open a slot for the next handle. For that reason, handle counters can sometimes return the wrong value (the higher value that gets returned is usually the correct one), but that is okay since the key part is the analysis.

Anyway, let's get to it! If you want to check if your map has leaks, you should create a trigger like this:

HandleCounter
Events
Time - Every 0.09 seconds of game time

Conditions

Actions
Set Location = (Point(0.00, 0.00))

Game - Display to (All players) the text: (String((Key (Location)) - 1048576))

Custom script: call RemoveLocation(udg_Location)

Or, in JASS:

 JASS:

function HandleCount takes nothing returns nothing
 local location L = Location(0,0)
 call BJDebugMsg(I2S(GetHandleId(L)-0x100000))
 call RemoveLocation(L)
 set L = null
endfunction

//===========================================================================
function InitTrig_HandleCounter takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterTimerEvent(t,0.09,true)
 call TriggerAddAction(t,function HandleCount)
endfunction

In a relatively empty map, that is the value I got. Notice how it is consistent. That is good news. If the values are relatively consistent (or are bouncing between two numbers), that means that there are no periodic leaks that are occurring. Make sure you properly test everything though. If you cast a spell and see a sudden rise, that must mean that the spell is leaking. (unless it returns back to its original value after the spell)

Now, let me show you what the results will look like using the trigger named Leaky mentioned above.

After waiting approximately 30 seconds, those are the numbers that were spat out. Notice that without the leaks, it stayed at 94 for 30 seconds and beyond. However, after the trigger was added, it went to 3000+ in only 30 seconds. That is a sign of leaking periodically, which is the usual cause for eventual lag in the map. Just notice for patterns of sudden increases, and it should be a relatively accurate method for detecting leaks.

​

Player Disconnections:

This is a bit more complex and definitely harder to seek out. There are key signs that lead to players being kicked, but it is not always the reason you think it to be.

First of all, a desync is basically a disconnection caused by non-synchronous actions. Say you kill a unit for one player only. Then the other player goes and decides to click on that unit (because it still appears alive for them), that will desync. It is alive and dead for different players. Generally, creating and destroying handles (with id's greater than 0x100000) will cause data to become out of sync and will lead to disconnections. To perform functions for certain players, people use the infamous GetLocalPlayer() function. For more information on that topic, please read my other tutorial:

http://www.thehelper.net/forums/showthread.php/89207-JASS-GetLocalPlayer()?p=722458#post722458

In maps with only GUI, you probably won't be using GetLocalPlayer() much. However, there are some blizzard functions to watch out for:

Camera - Pan camera as necessary for Player 1 (Red) to (Center of (Playable map area)) over 0.50 seconds
This will desync. Do not use this function, unless it isn't multiplayer. The reason why, is because of its source function:

 JASS:

function SmartCameraPanBJ takes player whichPlayer, location loc, real duration returns nothing
 local real dist
 if (GetLocalPlayer() == whichPlayer) then
 // Use only local code (no net traffic) within this block to avoid desyncs.

 set dist = DistanceBetweenPoints(loc, GetCameraTargetPositionLoc())
 if (dist >= bj_SMARTPAN_TRESHOLD_SNAP) then
 // If the user is too far away, snap the camera.
 call PanCameraToTimed(GetLocationX(loc), GetLocationY(loc), 0)
 elseif (dist >= bj_SMARTPAN_TRESHOLD_PAN) then
 // If the user is moderately close, pan the camera.
 call PanCameraToTimed(GetLocationX(loc), GetLocationY(loc), duration)
 else
 // User is close enough, so don't touch the camera.
 endif
 endif
endfunction

This line:

set dist = DistanceBetweenPoints(loc, GetCameraTargetPositionLoc())

Generates a handle from GetCameraTargetPositionLoc(). Blizzard didn't implement this function carefully enough, which is why it leads to disconnections. Basically, refrain from that function as much as possible.

The other functions only desync on macs, which I believe is UnitDamageArea() and terrain deformations (unconfirmed). Thus, you might want to test on a windows computer.

That covers GUI, but what if you know how to use GetLocalPlayer()? If you have desyncs and your map uses it directly, this is what you should do:

 Create a trigger, the name doesn't matter.

 Go to Edit -> Convert to Custom Text

 Erase everything you see.

 Now type in the following code:

 JASS:

globals
 a b
endglobals

 Save. You should get an error. For normal WE, it will probably say Statement Out of Function in the error message when you save. If you have JASS NewGenPack, it will say Undefined type a. The reason why I made a globals block is that if you have a syntax error in the type of the global, it will show the source code for the JASS and the structs. This way, you can see what it will all be converted to.

 Select all your code, and copy it. This is what mine looks like:

 JASS:

globals
 // Generated
unit gg_unit_Ogrh_0001= null
trigger gg_trg_Untitled_Trigger_001= null
a b

//JASSHelper struct globals:

endglobals

//===========================================================================
//
// Testing Map
//
// Warcraft III map script
// Generated by the Warcraft III World Editor
// Date: Wed Jan 26 18:34:50 2011
// Map Author: Unknown
//
//===========================================================================

//***************************************************************************
//*
//* Global Variables
//*
//***************************************************************************

function InitGlobals takes nothing returns nothing
endfunction

//***************************************************************************
//*
//* Unit Creation
//*
//***************************************************************************

//===========================================================================
function CreateUnitsForPlayer0 takes nothing returns nothing
 local player p= Player(0)
 local unit u
 local integer unitID
 local trigger t
 local real life

 set u=CreateUnit(p, 'Ogrh', - 419.3, - 146.5, 304.528)
endfunction

//===========================================================================
function CreatePlayerBuildings takes nothing returns nothing
endfunction

//===========================================================================
function CreatePlayerUnits takes nothing returns nothing
 call CreateUnitsForPlayer0()
endfunction

//===========================================================================
function CreateAllUnits takes nothing returns nothing
 call CreatePlayerBuildings()
 call CreatePlayerUnits()
endfunction

//***************************************************************************
//*
//* Custom Script Code
//*
//***************************************************************************
//TESH.scrollpos=0
//TESH.alwaysfold=0
//w3x
//***************************************************************************
//*
//* Triggers
//*
//***************************************************************************

//===========================================================================
// Trigger: Untitled Trigger 001
//===========================================================================
//TESH.scrollpos=0
//TESH.alwaysfold=0
function InitCustomTriggers takes nothing returns nothing
 //Function not found: call InitTrig_Untitled_Trigger_001()
endfunction

//***************************************************************************
//*
//* Players
//*
//***************************************************************************

function InitCustomPlayerSlots takes nothing returns nothing

 // Player 0
 call SetPlayerStartLocation(Player(0), 0)
 call SetPlayerColor(Player(0), ConvertPlayerColor(0))
 call SetPlayerRacePreference(Player(0), RACE_PREF_HUMAN)
 call SetPlayerRaceSelectable(Player(0), true)
 call SetPlayerController(Player(0), MAP_CONTROL_USER)

endfunction

function InitCustomTeams takes nothing returns nothing
 // Force: TRIGSTR_006
 call SetPlayerTeam(Player(0), 0)

endfunction

//***************************************************************************
//*
//* Main Initialization
//*
//***************************************************************************

//===========================================================================
function main takes nothing returns nothing
 call SetCameraBounds(- 3328.0 + GetCameraMargin(CAMERA_MARGIN_LEFT), - 3584.0 + GetCameraMargin(CAMERA_MARGIN_BOTTOM), 3328.0 - GetCameraMargin(CAMERA_MARGIN_RIGHT), 3072.0 - GetCameraMargin(CAMERA_MARGIN_TOP), - 3328.0 + GetCameraMargin(CAMERA_MARGIN_LEFT), 3072.0 - GetCameraMargin(CAMERA_MARGIN_TOP), 3328.0 - GetCameraMargin(CAMERA_MARGIN_RIGHT), - 3584.0 + GetCameraMargin(CAMERA_MARGIN_BOTTOM))
 call SetDayNightModels("Environment\\DNC\\DNCLordaeron\\DNCLordaeronTerrain\\DNCLordaeronTerrain.mdl", "Environment\\DNC\\DNCLordaeron\\DNCLordaeronUnit\\DNCLordaeronUnit.mdl")
 call NewSoundEnvironment("Default")
 call SetAmbientDaySound("LordaeronSummerDay")
 call SetAmbientNightSound("LordaeronSummerNight")
 call SetMapMusic("Music", true, 0)
 call CreateAllUnits()
 call InitBlizzard()

 call InitGlobals()
 call InitCustomTriggers()

endfunction

//***************************************************************************
//*
//* Map Configuration
//*
//***************************************************************************

function config takes nothing returns nothing
 call SetMapName("TRIGSTR_001")
 call SetMapDescription("TRIGSTR_003")
 call SetPlayers(1)
 call SetTeams(1)
 call SetGamePlacement(MAP_PLACEMENT_USE_MAP_SETTINGS)

 call DefineStartLocation(0, - 128.0, - 64.0)

 // Player setup
 call InitCustomPlayerSlots()
 call SetPlayerSlotAvailable(Player(0), MAP_CONTROL_USER)
 call InitGenericPlayerSlots()
endfunction

 You can press OK to the error and delete the trigger now. Paste the code into notepad or JASSCraft.

 Press CTRL+F and look up GetLocalPlayer() and SmartCameraPanBJ(). If you find the latter, then that means that you are using a Pan Camera as Necessary function in your map. If you find the first, you should look at the contents. You will have to know JASS for that, but otherwise find out what trigger it is in, and disable those triggers. Above every block, they will save which trigger the code is for. Such as:

 JASS:

//===========================================================================
// Trigger: Untitled Trigger 001
//===========================================================================

If you see that above your code, then the code is found under the trigger Untitled Trigger 001.

In the local player blocks, look out for anything with Create or Destroy in its name. Watch out for strings, TriggerSleepAction() or PolledWait(), SetRandomSeed(), and basically anything that may seem fishy. Read the tutorial linked above and it may explain some things for you.

If you would like a bit of an easier time testing for desyncs, check this out!

https://www.hiveworkshop.com/forums...multiple-warcraft-3-running-single-pc-226287/

This will allow you to run LAN with yourself for testing; that way you do not have to host the map or run LAN with another player/computer.

​

Odd Data:

This is a very broad topic, but generally watch out for data that is returned in an odd manner. In particular, I am talking about camera functions. While they take degrees for angle fields, they return the value in radians.

Cam
Events
Player - Player 1 (Red) skips a cinematic sequence

Conditions

Actions
Game - Display to (All players) the text: (Angle of Attack: + (String((Angle of attack of the current camera view))))

Game - Display to (All players) the text: (Roll: + (String((Roll of the current camera view))))

Game - Display to (All players) the text: (Rotation: + (String((Rotation of the current camera view))))

This returned all the values in radians. In degrees, it would be 356.21, 14, and 94.42 respectively. In things like sliding and knockbacks, you may want to use messages like the ones above, but for the angles and positions that you are using. Sometimes you may forget to use the appropriate angle form. (mostly in JASS) Such as:

 JASS:

call SetUnitX(u,GetUnitX(u)+15*Cos(GetUnitFacing(u)))

That should be:

 JASS:

call SetUnitX(u,GetUnitX(u)+15*Cos(GetUnitFacing(u)*bj_DEGTORAD))

Since the Cosine function's input is radians in Wc3.

​

Check all the code:

Make sure everything is checked properly. Sometimes the mistakes are pretty silly, such as forgetting to add an event or forgetting a condition. Check each part thoroughly, or you may overlook some things.

​
Infinite Loops:

Infinite loops are also tricky to catch. An infinite loop is where functions are called repeatedly over and over indefinitely. This will either cause a crash (it will freeze, and exit without any error messages), or it will cause the function to reach the operation limit and it will skip the remaining actions.

For example:

 JASS:

scope InfiniteLoop initializer Init
 function Test takes nothing returns nothing
 local boolean b = false
 call BJDebugMsg("Hello") //hello will display
 loop
 exitwhen b //exit the loop when b == true
 //which will never happen since it is false
 endloop
 call BJDebugMsg("World") //world will not display
 endfunction

 private function Init takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterPlayerEventEndCinematic(t,Player(0)) //register ESC
 call TriggerAddAction(t,function Test)
 endfunction
endscope

"Hello" will be displayed, but "World" will not be displayed. It stops after the loop, because once you hit the operation limit, no more functions can be called unless you start a new thread. (or maybe use TriggerSleepAction())

Here is another example, which is a bit harder to find:

Untitled Trigger 001
Events
Unit - A unit Is issued an order with no target

Conditions

Actions
Unit - Order (Triggering unit) to Stop

This trigger is fired when a unit is issued an order with no target, such as "Stop" or "Hold position". After that, it issues the triggering unit to stop. Since it is issuing that order (which has no target), it fires this event again:

Unit - A unit Is issued an order with no target
Which calls the order for the unit to stop, which fires the event again, and so on. It will keep firing and ordering a stop, and the game will freeze. After maybe a second or two, it will then close and show no error message. Thus, watch out for firing your own event again. To prevent this, you can simply do this:

Untitled Trigger 001
Events
Unit - A unit Is issued an order with no target

Conditions

Actions
Trigger - Turn off (This trigger)

Unit - Order (Triggering unit) to Stop

Trigger - Turn on (This trigger)

Turn off the trigger before performing the execution. This will still perform the action, but it won't fire the event.

​

Trigger Stops from Division:

Normally, actions are skipped from doing something like "Skip Remaining Actions" or return, but there is another thing that will cause a trigger to stop.

Untitled Trigger 001
Events
Player - Player 1 (Red) skips a cinematic sequence

Conditions

Actions
Game - Display to (All players) the text: Hello

Set SomeInteger = (5 / 0)

Game - Display to (All players) the text: World

That is division by 0. If you divide some number by 0, it will skip the remaining actions because (x/0) is undefined. Beware of this, as it can happen occasionally.

​

FAQ:

Q: My map is lagging, but I have no leaks. What is wrong?

A: Usually it is either your computer or generally inefficient code. Make sure it is optimized to its fullest.

Q: My spell is not working. It isn't retrieving the proper data. What is wrong?

A: Be sure to check the appropriate keys if you use hashtables. If you use indexing, make sure you have the right indexes. Use GetHandleId() (or the Key function) and display an ingame message on the data you get. If it returns 0, then you know that the data isn't being retrieved for some reason.

Q: My map has over 10,000 handles from the counter. Does that mean I am leaking 10,000 times?

A: If it is increasing constantly over 10,000, then you probably have some leaks. If it is staying consistent, don't worry about it. A lot of big maps will have a lot of things on the map already, such as doodads and destructibles and units, which already count towards the counter. Sometimes the counter may start at 8,000 if it is a big map, or sometimes 100 if it is a small map. It depends on the map.

Q: My handle count bounces between one number and another, which one is the right value?

A: That is because the way that handle id's get reused. It will go in the next slot available, so it is usually the higher number that is the correct value.

Q: How do I fix my GetLocalPlayer() problems?

A: It may be a bit complicated, so I suggest disabling whatever triggers that use it and testing the map to make sure that they are the source of the desyncs. After that, you can go to the Triggers&Scripts forum on this site for help.

That is it for this tutorial. Good luck with debugging!

#### Attachments

cam.jpg

 18.8 KB

 · Views: 4,061



rand.jpg

 3.9 KB

 · Views: 4,059



vars.jpg

 36.8 KB

 · Views: 4,154



ini.jpg

 24.2 KB

 · Views: 4,111



setini.jpg

 74.7 KB

 · Views: 4,127



hc_1.jpg

 15 KB

 · Views: 4,225



hc_2.jpg

 20.7 KB

 · Views: 4,084




 Last edited: Jul 9, 2013

#### supertoinkz

#### supertoinkz

 Level 28

JoinedDec 29, 2008Messages3,351

Resources: 46

#### Resources
Models 23Icons 17Skins 2Maps 2Tutorials 2

vBulletin Message

Invalid Attachment specified. If you followed a valid link, please notify the administrator

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Hmm.. Where is that? It doesn't show anything like that for me.

#### supertoinkz

#### supertoinkz

 Level 28

JoinedDec 29, 2008Messages3,351

Resources: 46

#### Resources
Models 23Icons 17Skins 2Maps 2Tutorials 2

Every picture except the pic of "Odd Data"

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Here you go example:

In-game, you will now see this:

[IMG]https://www.hiveworkshop.com/forums/attachment.php?attachmentid=96860&stc=1&d=1296089313[/IMG]

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Thanks guys, it should work now.  I'll let this sit in the tutorial submission for a little bit, and I'll approve it based on the replying posts.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

It works, really cool, I checked everything for 2nd time and I really like idea and how you explained everything

Vote for approval!

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Thanks.  I guess I'll approve, I doubt I'll get much more comments.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

never thought that there would be a tutorial about debugging XD...I used debugging messages to track indexing flaws...

#### Darkwing

#### Darkwing

 Level 6

JoinedMay 15, 2008Messages147

Resources: 2

#### Resources
Maps 1Tutorials 1

Yup I'm using game messages as a debugger also for a long time now, it really helped me get a hang on things since I have the internet for such short period, and I couldn't check these kinds of tutorials, it's rather kickass tutorial for the begginers!!!

+REP

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

 JASS:

function HandleCount takes nothing returns nothing
 local location L = Location(0,0)
 call BJDebugMsg(I2S(GetHandleId(L)-0x100000))
 call RemoveLocation(L)
 set L = null
endfunction

//===========================================================================
function InitTrig_HandleCounter takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterTimerEvent(t,0.09,true)
 call TriggerAddAction(t,function HandleCount)
endfunction

That crashes the editor.

#### Elbadruhel

#### Elbadruhel

 Level 6

JoinedDec 13, 2017Messages266

I know this thread is old, but since it looks like the people here knows about bugs and currently warcraft 3 is dying due to one, i feel the need to ressurrect the thread hoping some of you are still around here.

Blizzard broke something with their last patch, and now most custom games freeze around 10-15 mins for ALL players, is not lag, is not crash, the game just freezes for everyone at once. It is not due to something being activated, launched or used, it just "happens". Some people has suggested that it might be due to blizzard breaking the "every 0.01 seconds" trigger, but some maps are crashing even without using that function. And what is worse, as i said before, the game doesnt freezes when u use that (or other) function, just if "it exists" in the map.

Is a pain in the ass and is ruining a lot of maps, could you please give a look to it and discover what blizzard broke to cause this freezing? Cause they are not answering at all, they are completly silent since they released the patch and broke everything, so you are our last hope.

 You must log in or register to reply here.


### Similar threads

 Update TESH and add your own functions.


baassee

Oct 24, 2010

Misc/General Mapping Tutorials

Replies3Views5K

Oct 26, 2010
baassee

Locked

 help EDITOR NOT WORKING!!!


sarimdesert

Jan 30, 2011

World Editor Help Zone

Replies9Views2K

Jan 31, 2011
sarimdesert

 Complete list of things that leak


busterkomo

May 8, 2009

Misc/General Mapping Tutorials

Replies21Views28K

May 26, 2015
SAUS

 Make a Map Work for Patch 1.24


PurgeandFire

Apr 29, 2010

Misc/General Mapping Tutorials

Replies12Views32K

Jul 4, 2016
doido

 How to Disable Game Pausing


TriggerHappy

Jan 24, 2014

Misc/General Mapping Tutorials

Replies9Views5K

May 24, 2014
Tal'Rarity

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