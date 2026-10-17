---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/memory-leaks.263410/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Memory Leaks
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/memory-leaks.263410/>
#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Memory leaks

For a quick lookup on most important actions, read Things That Leak.

Introduction

Object Leaks

Reference Leaks

Miscellaneous

Conclusion

Introduction

If your computer's memory keeps occupied with stuff you already lost access to, it's called memory leak. It slows down the computer for no benefits.

Having tons of leaks may lead to lag-spikes upto a total game freeze.

This tutorial will explain how to keep your code as leakfree as possible.

​

Object Leaks

What is an object leak?

An object is something you can create with triggers, like a unit, a unit group, or a floating text.

If an object exists, but you can't use it anymore it's considered an object leak.

All in-game created objects should always be destroyed after they fulfilled their purpose.

Example where objects are created:

Unit - Create 1 Footman for Player 1 (Red) at (Center of (Playable map area)) facing Default building facing degrees

It's easy to guess a new unit object is created, but watch out, there's more.

"(Center of (Playable map area))" creates a new point object. As it is, it will leak.

Unit Group - Pick every unit in (Units in (Playable map area)) and do (Actions)
Loop - Actions

"(Units in (Playable map area))" creates a new unit group object. As it is, it will leak.

So the above lines use objects to operate. Always when objects are involved, there's a risk to leak.

But be aware that not only actions might use objects, but also condtions can use them:

(Number of units in (Units in (Playable map area))) Equal to 0

"(Units in (Playable map area))" creates a new unit group object, too! As it is, it will leak, even it's in a condition.

​
How to prevent object leaks?

Some examples to destroy objects:

Destructible - Remove <Destructible> Object

Environment - Remove <Weathereffect>

Image - Destroy <Image>

Floating Text - Destroy <FloatingText>

Item - Remove <Item>

Leaderboard - Destroy <Leaderboard>

Lightning - Destroy <Lightning>

Multiboard - Destroy <Multiboard>

Unit - Remove <Unit> from the game

Example

We want to create a special effect, and to prevent a leak we'll call its destroy function after usage:

Special Effect - Create a special effect at ( ... ) using Abilities\Spells\Human\ThunderClap\ThunderClapCaster.mdl

Special Effect - Destroy (Last created special effect)
And voila, the effect does not leak!

Though, GUI does not provide all destroy functions we need.

Sometimes we have to use a custom script as help. But don't worry, it's always only one line.

Locations

Set Location = (Center of (Playable map area))

... Usage of Location ...

Custom script: call RemoveLocation(udg_Location)

Unit Groups

Set Group = Pick every unit in (Playable Map Area)

... Usage of Group ...

Custom script: call DestroyGroup(udg_Group)

For groups you can even use an other method.

If you set "bj_wantDestroyGroup" to "true" via custom script, it will automatically destroy the next unit group creation, that is done via GUI.

Custom script: set bj_wantDestroyGroup = true

Unit Group - Pick every unit in (Playable Map Area) and do (Unit - Hide (Picked unit))

Player Groups

Set PlayerGroup = (All enemies of Player 1 (Red))

... Usage of PlayerGroup ...

Custom script: call DestroyForce(udg_PlayerGroup)

Be warned that "(All Players)" doesn't leak, as it's a constant. Never destroy it!

Following doesn't leak, and can be used just like that:

Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions

There are some things which are worth an extra note:

### Overview

### Death Animation

### Units

### Items

Destroying something will still play its death animation.

Units get automatically removed after their corpse decayed.

Items are never being automatically removed.

If you for example destroy a special effect directly after creation, its death animation will still correctly play:

Special Effect - Create a special effect at Location using Abilities\Spells\Human\ThunderClap\ThunderClapCaster.mdl

Special Effect - Destroy (Last created special effect)

It's also applayable for items, units and destructibles. Killing/destroying will always play the death animation, while removing won't!

When a unit dies it still exists in the game, but it's just dead. Usually it lets some corpse behind and can be revived at some later point.

Only after the corpse decayed, it means the unit was fully removed from the game. Then it's the same as you would use:

Unit - Remove Unit from the game

So it's usually not required to clean up after normal unit deaths, as the game cleans them up on its own.

Items never get properly removed after they died, or were used (Tomes/Powerups).

If you want dead items being automatically cleaned up you can take usage of a system which does it for you:

For GUI: Item Cleanup 1.3

For JASS: [System] Item Cleanup

Technique of removal

We often use variables that help us to refer to the object being destroyed.

Important is that you don't overwrite the variable with other values before the object is actually destroyed!

Let's go by simple example where we want to destroy two locations:

Good example:Bad example:

Set loc = Location_1

Unit - Move Unit_1 to loc

Custom script: call RemoveLocation(udg_loc)

Set loc = Location_2

Unit - Move Unit_2 to loc

Custom script: call RemoveLocation(udg_loc)

Set loc = Location_1

Unit - Move Unit_1 to loc

Set loc = Location_2 // OVERWRITTEN

Unit - Move Unit_2 to loc

Custom script: call RemoveLocation(udg_loc)

In the bad example you lost access to Location_1, because the variable was overwritten.

Location_1 can't be removed anymore, so it will leak.

​

Reference Leaks

<< If you don't use JASS you can skip this chapter. >>​

What is a reference leak?

Firstly, only children types by the agent type are affected by the reference leak. Other handles don't suffer from it.

An agent basically contains one address to an object, and one reference counter for it.

The object itself is what we have access to. It can be a unit, group, timer, or what ever. You have powers to destroy this object on yourself.

The reference counter is the number of variables pointing on this agent. It increases/decreases automatically when ever you assign variables.

 C:

struct Agent
{
 Object* object;
 int referenceCounter;
};

The agent struct you see above automatically allocates and also deallocates its memory, once it's not needed anymore.

For us, it's only important to know that next to the object itself, there's a bit more memory in use for the agent.

And there's actually a problem with this memory being automatically deallocated, even if we destroy the used object behind.

To let this memory for the agent automatically deallocate, you have to destroy the object and have to ensure its reference counter is 0, meaning there are no more variables pointing on it.

If the agent can not be deallocated anymore, due to an reference counter above 0, this is what is called reference leak. Its agent's id, the handleid, can never be recycled.​

How to prevent reference leaks?

We want to ensure an agent's reference counter is 0, when we're about to destroy its object.

To achieve this we always let our variables point on something else, after they fullfilled their purpose and before they fall out of scope. null comes handy here, as you may apply it to any agent type.

Example:

 JASS:

function foo takes nothing returns nothing
 local group g = CreateGroup()

 call DestroyGroup(g)
 set g = null
endfunction

At the top of the function we increase the reference counter for the agent on which g refers to, but we also ensure it's decreased again at the end of the function. And done! The agent won't have any issue to free its memory after the function has ended, because its reference counter is 0.​

Technique of removal

When the variable does point to null, it does of course also mean that it doesn't point to the agent anymore. So, if you want to destroy the object, you firstly must ensure to destroy the object before you null the variable.

Good example:Bad Example:

 JASS:

local group g = CreateGroup()
call DestroyGroup(g)
set g = null

 JASS:

local group g = CreateGroup()
set g = null
call DestroyGroup(g)

In the bad example the destroy function will fail, because g does not point to the group anymore. It would result in an object leak.​

Miscellaneous

Leaks And Data Types

If something can leak depends on its data type:

### Overview

### Primitives

### Enums

### Objects

Primitives never leak.

Enums never leak.

Objects can leak!

There exist 6 primitive/native data types: integer , boolean , real , string , code , handle

As a GUI user you maybe didn't read something about code or handle so far, but don't worry, it's not really relevant for GUI.

You can imagine enums as special form of an integer but with a new name.

 JASS:

race
alliancetype
racepreference
gamestate igamestate fgamestate
playerstate
playerscore
playergameresult
unitstate
aidifficulty
eventid
gameevent
playerevent
playerunitevent
unitevent
limitop
widgetevent
dialogevent
unittype
gamespeed
gamedifficulty
gametype
mapflag
mapvisibility
mapsetting
mapdensity
mapcontrol
fogstate
playerslotstate
volumegroup
camerafield
playercolor
placement
startlocprio
raritycontrol
blendmode
texmapflags
effecttype
version
itemtype
attacktype
damagetype
weapontype
soundtype
pathingtype

Objects can be something like effect , unit , timer , group , multiboard.

And basically much more! Everything that you can create or destroy is an object!

Things that always leak

There are some leaks that you can't prevent.

A unit will always keep a tiny bit of memory, even when removed properly. 0.04 kb and unpreventable. (Link)

Terrain deformations leak. Including default abilities like shockwave. (reference leak)

Very much BJs (Blizzard JASS functions) which are mostly used in GUI have reference leaks.

You might always look them up to see how a BJ is defined at blizzard.

Leaks and RAM

Location: 0.361 KB

Group: 0.62 KB + 0.040 KB for each unit in the group.

Effect: 11.631 KB
So 10'000 locations take up ~1/3 GB, which is definitely a concern.

Source: Automatic Memory Leak Destroyer - Wc3C.net​

Other Tips

If the black screen freezes after game-end it may be an indicator for huge amount of leaks. Don't worry if it's a only a few seconds - it might also be the normal memory usage your map needs.

Lags are not necessarily a proof for leaks. Highly inefficient coding may also result in lags for example.

An indicator for major leaks might be noticed in task manager. Just watch the memory usage of WC3 while running the map.

If it's increasing very fast and continuously, you might check your triggers again. (code that runs periodicly or is called very often is most important)

As GUI handles certain object initialization by default, (for arrays index [0] and [1]), it does technically leak once if you create for example a unit group for the first time and so overwrite the initialy created group, without having destroyed it. Example creation:

Set Group = (Units owned by Player 1(Red))

^But this leak fires only once, as you would destroy your self-created groups each time after usage. So this is good to know, but has no real effect.

Conclusion

Always remove objects after usage, else you will have an object leak.

Always null agent type variables, else you will have a reference leak.

Object leak is the major leak, but the reference leak is still a leak so don't neglect it.
That's it, thanks for reading! If you have anything to add or did not understand something, feel free to post. ​


 Last edited: Jun 30, 2020

#### LordDz

#### LordDz

 Level 25

JoinedMay 11, 2007Messages4,643

Resources: 8

#### Resources
Maps 8

It's a nice tutorial, but don't we already have this?

https://www.hiveworkshop.com/forums/triggers-scripts-269/things-leak-35124/

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

"Things That Leak" is okay, but in my opinion the user could be teached some more general information about leaks.

It is like an extension, but in tutorial section. There will be probably a link in "Things That Leak" that will link here.

Also I have not really found anything about reference leaks on THW. These explainations might be new for most users.

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

I have my own definition of leak, I think this is more general, including "memory leak" defined in other programming languages as well. I'm not an expert so don't trust me.


Memory leak is where you create something (in this case agent) which is never been automatically destroyed by the system except if the program is ended (halt). Thus if you de-reference it without destroying, you will never be able access it anymore whereas it's still there. It consumes memory, but you have no access and can not remove it anymore except if the program (in this case, the map) is closed. So you can imagine how memory leaks make your map become heavier over times.


Phew, I think wikipedia (Memory leak) define it better after all LOL. In short, memory leak is allocating memory space without deallocating that space later.


Other information:


Someone can indicate whether his/her map is badly leakful if the map needs some "freeze moment" when the game is closed, including restart, return to menu, or exit game. Perhaps you can add this.


 Last edited by a moderator: Jul 19, 2015

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

LordDz said:

 It's a nice tutorial, but don't we already have this?

https://www.hiveworkshop.com/forums/triggers-scripts-269/things-leak-35124/

Click to expand...

It was decided through PM's that IcemanBo's guide would prove useful since it gives a deeper explanation of what is going on with leaks. In general, it is better to know the reason behind removing leaks than just knowing the functions to remove them.

@IcemanBo: One small fix before it is ready for approval (didn't notice in my first skim):

 Basicly you only have to null the variable that refered to your object.

If you can't ensure that a variable will point on a new object, you will have to null it.

Otherwise the handle ID would be like for ever in use and take up space for absolutly no reason.

Attention, you don't have to null all handle type object, but only agents.

The reason for this is that only agents do not handle to reuse the handle ID automatically.


Click to expand...

The handle ID leak only occurs with local variables. There is a minor leak associated with not nulling globals, but it is really insignificant (most likely just things associated with an objects reference count, which is static and probably takes up a few bytes at most). It is optional to remove in the case of globals. In locals, it is mandatory (at least for approval)--for all non-permanent agents.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Okay, PurgeandFire (the wise) explained to me via PM why not nulling globals leaks a bit different as with locals.


It is an other type of reference leak, which I did not know. But the point is, that it can be totaly ignored for realistic usage.


Changed.

#### Tal'Rarity

#### Tal'Rarity

 Level 26

JoinedMar 15, 2012Messages2,892

Resources: 25

#### Resources
Models 2Maps 8Spells and Systems 12Ported Models 1Tutorials 2

Doesn't creating new units/hero's in-game make permanent leaks that aren't cleanable? I know they do, but some likely won't believe me. Also you should make a note about shockwave as well war stomp. This is likely only for triggers/code, but technically its all code. Good tutorial. By the way you should always clean as many leaks as possible even when your map/game ends to prevent leaks from carrying over to your next game. Though what you can do to clear your memory is to restart WC3 entirely, close it then start it back up again.

Edit: It's easy enough to test quickly if anyone needs to find out for themselves. Just clean all the leaks you know how too then continue to make units and remove them about 10,000 times for a noticeable number. Same with other stuff if you don't clean it, at least we get to clean some of the leaks I suppose. =)

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Yes, units will always use a bit of memory. Also added something from Dalvengyr's post.


There might be some more things that always leak, I did not analyse each single function.


There also might be some more interesting things to know that I could add to "Miscellaneous".


I'm positive that we can gather most interesting stuff together with community's participation.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

Dalvengyr said:

 I have my own definition of leak, I think this is more general, including "memory leak" defined in other programming languages as well. I'm not an expert so don't trust me.


Memory leak is where you create something (in this case agent) which is never been automatically destroyed by the system except if the program is ended (halt). Thus if you de-reference it without destroying, you will never be able access it anymore whereas it's still there. It consumes memory, but you have no access and can not remove it anymore except if the program (in this case, the map) is closed. So you can imagine how memory leaks make your map become heavier over times.


Phew, I think wikipedia (Memory leak) define it better after all LOL. In short, memory leak is allocating memory space without deallocating that space later.


Other information:


Someone can indicate whether his/her map is badly leakful if the map needs some "freeze moment" when the game is closed, including restart, return to menu, or exit game. Perhaps you can add this.


Click to expand...


since Jass is event driven, this is not true, because I can lose all references to a unit, but I can still get more, if the unit fires some event.


Wiki definition is also not really that good, because all memory is deallocated when you end the program, or at worst, when you turn off computer, so technically you never create memory leaks by that definition.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

I think this tutorial is straight and to-the-point. The "Things that leak" sticky isn't as descriptive and tutorial-esque as this submission. I vote for approval.

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

The in-depth details were useful. +rep

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

I think you should replace "a certain boolean = true"

with "bj_wantDestroyGroup = true"

It's a good tutorial

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

^Changed it. Also I came up with some more imporvements.

#### blancfaye7

#### blancfaye7

 Level 10

JoinedMay 21, 2014Messages580

Resources: 1

#### Resources
Maps 1

So even when a unit dies in-game (or should I say, finished decaying), it causes leaks? (I am not asking of how much it would leak, but an answer to that is appreciated as well.)

BTW, best tutorial about leaks. Ever.


 Last edited: Apr 11, 2015

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Yes, units will permanently occupy a small part of memory, even after removed properply. (0.04kb iirc)


Ty!

#### blancfaye7

#### blancfaye7

 Level 10

JoinedMay 21, 2014Messages580

Resources: 1

#### Resources
Maps 1

IcemanBo said:

 Yes, units will permanently occupy a small part of memory, even after removed properply. (0.04kb iirc)

Ty!

Click to expand...

Oh my... That is indeed bad for my map

Good thing I did not make any dummy units at all.

But the spawns...

Ah well...  Thanks for the info!

You should also add that this function:

Camera - Pan Camera as needed...
 also causes permanent leaks.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Ah, right. This function anyway should not be used, and I will note it shortly, thanks.

 Oh my... That is indeed bad for my map


Click to expand...

But well, 0.04 kb is not much. Don't think too much about it.

#### danger_friendGR

#### danger_friendGR

 Level 1

JoinedApr 30, 2013Messages4

Special

Ye that's so helpful icemanbo is da best and he is SPECIAL!!!He knows better!

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

approved

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Since Unit groups are initialized at Map Initialization as Empty Group, that means the first time using

Set Group = (Units within Range of Location)
will create a leak since ^that action overwrites the Empty group right?

 JASS:

//Map Init
set udg_Group = CreateGroup()

//When using the GUI Pick Units the first time, the previously pointed unit group that was
//created at Map Init leaks
function GetUnitsInRangeOfLocMatching takes real radius, location whichLocation, boolexpr filter returns group
 local group g = CreateGroup()
 call GroupEnumUnitsInRangeOfLoc(g, whichLocation, radius, filter)
 call DestroyBoolExpr(filter)
 return g
endfunction

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Technically seen, yes a group does already exist, and overwriting it does cause a leak.


I've seen people destroying them in the spells init trigger for GroupArray[0] and [1].


It won't hurt noone, because one-fired leaks are like nothing, but it can be noted unter Miscs.

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

Flux said:

 Since Unit groups are initialized at Map Initialization as Empty Group, that means the first time using

Set Group = (Units within Range of Location)
will create a leak since ^that action overwrites the Empty group right


Click to expand...


I have been living my life wrong this whole time ;_;

#### Rasler

#### Rasler

 Level 7

JoinedJan 15, 2016Messages137

i HAVE a few questions. In my new project i use transitions map, where it "changes" map constantly (not loads). When it changes, it means that the map is starting with 0 memory leaks? its like restarting and the memory used is destroyed?

#### Rasler

#### Rasler

 Level 7

JoinedJan 15, 2016Messages137

GhostHunter123 said:

 IcemanBo: Tutorial says group array index [0] and [1] leak but shows a non-array group as example. The case for non-array groups is not clarified.


Rasler: if your "maps" are just areas of a single map, then the memory leaks, if there are any, remain.


Click to expand...


Areas?, i say that are totally diferent maps. Like rexxars campaign. But changing maps instead of loading them..

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Rasler said:

 Areas?, i say that are totally diferent maps. Like rexxars campaign. But changing maps instead of loading them..


Click to expand...


If it is like Rexxar's campaign, then yes all the previous map's memory will be cleared before loading the next map. So it is kinda like starting fresh.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Updated slightly and also added "Technique of removal" to chapter "Refrerence Leaks",

because I still see sometimes that it is confusing people.

Edit Udate again.

Btw GhostHunter123, sorry I forgot to answer you.

If a variable is initialisized already it does point to an object.

So when you overwrite an already initialisized variable, it will lead to leak.

Non-arrays group variables are initialisized.

Array group variablesa are initialisized with index [0] and [2].


 Last edited: Jan 30, 2016

#### Aniki

#### Aniki

 Level 13

JoinedNov 7, 2014Messages571

Resources: 21

#### Resources
Tools 1Maps 1Spells and Systems 14JASS 3Tutorials 2

It's probably worth mentioning that functions that return handles should probably not use a local variable in the return statement


because that would prevent the handle-id of ever getting recycled by the game: (source),


that thread is a bit heated but very fun to read =)


example:


handle-id leak:

 JASS:

function CreateTransparentFootman takes player p returns unit
 local unit u = CreateUnit(p, 'hfoo', 0, 0, 0)
 call SetUnitVertexColor(u, 255, 255, 255, 96)
 return u
endfunction


no handle-id leak:

 JASS:

globals
 unit footman
endglobals

function CreateTransparentFootman takes player p returns unit
 local unit u = CreateUnit(p, 'hfoo', 0, 0, 0)
 call SetUnitVertexColor(u, 255, 255, 255, 96)
 set footman = u
 set u = null
 return footman

 // or avoiding locals all together
 // set footman = CreateUnit(p, 'hfoo', 0, 0, 0)
 // call SetUnitVertexColor(footman, 255, 255, 255, 96)
 // return footman
endfunction

// the last X'd vars from Blizzard.j could be used (eg.: bj_lastCreatedUnit) I guess, but those are for GUI and there are
// some handle types that are missing (eg.: region), so it's probably better to declare dedicated globals for those functions that return handles


And of course there's no point in nulling handles if they are never destroyed, i.e they are always recycled (timers, units, groups, etc.).

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Yeah, thanks.


Will think of where to add a part about it.


The solution is kind of ugly, that's why some just prefer taking the small (4 bytes?) leak..


but it might be mentioned, you're very right.

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

Go with bj_lastCreatedUnit as global unit variable.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Added it as "ExtraNote" in "Reference Leaks" -> "Technique of removel"

#### Skyflash

#### Skyflash

 Level 5

JoinedMar 6, 2015Messages130

Resources: 1

#### Resources
Spells and Systems 1

Dear IcemanBo

can you update your post and mention these leaks which is Increasing Handle counts?

1-Destructible Leak

Destructible - Pick every destructible within Radius of Point and do (Actions)
Loop - Actions

Solution

Custom script:set Rect= GetRectFromCircleBJ(Point,Radius)

Destructible - Pick every destructible in Rect and do (Actions)
Loop - Actions

Reason

 JASS:

function EnumDestructablesInCircleBJ takes real radius, location loc, code actionFunc returns nothing
 local rect r

 if (radius >= 0) then
 set bj_enumDestructableCenter = loc
 set bj_enumDestructableRadius = radius
 set r = GetRectFromCircleBJ(loc, radius)
 call EnumDestructablesInRect(r, filterEnumDestructablesInCircleBJ, actionFunc)
 call RemoveRect(r)
 //they have forgotten to Null local rect +1 Handle Leaks
 endif
endfunction

2-Most Common Leak that we are seeing these days Unit-Pick in Range

Unit Group - Pick every unit in (Units within Radius of Point) and do (Actions)
Loop - Actions

Solution

Custom script: set udg_group=CreateGroup()

Custom script: call GroupEnumUnitsInRange(group, GetLocationX(Point), GetLocationY(Point), Radius, null)

Unit Group - Pick every unit in (Units in Rect) and do (Actions)
Loop - Actions

Custom script:call DestroyGroup(udg_group)

Reason

 JASS:

function GetUnitsInRangeOfLocMatching takes real radius, location whichLocation, boolexpr filter returns group
 local group g = CreateGroup()
 call GroupEnumUnitsInRangeOfLoc(g, whichLocation, radius, filter)
 call DestroyBoolExpr(filter)
 return g
endfunction
// they forgot to remove Group and null the Global g +2 Handle Leaks


 Last edited: Mar 16, 2016

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Hey. Thanks for posting!

First there is to mention that you are right, that many BJs don't null local agents.

But for GUI usage it's pretty negligible, and mods probably won't ever enforce the user to change it. (GUI)

But there is also to say, that not the handle per se leaks, but it's assigned id. link,

so for example this statement was not very correct:

// they forgot to remove Group and null the Global g +2 Handle Leaks

Click to expand...

..., because it returns the group handle, and does not (handle-) leak.

I actually named some functions in "Things that always leak", but I did not finish it, because there are just too many of them I think.

What I can do is to write a all for one warning when using BJs, that they might leak.

But as long as they are not known for special bugs, like the "Camera - Pan camera as necessary...", they're mostly accepted for GUI users.

What do you say?

If you want to discuss further, please do!

#### Skyflash

#### Skyflash

 Level 5

JoinedMar 6, 2015Messages130

Resources: 1

#### Resources
Spells and Systems 1

IcemanBo said:

 Hey. Thanks for posting!

First there is to mention that you are right, that many BJs don't null local agents.

But for GUI usage it's pretty negligible, and mods probably won't ever enforce the user to change it. (GUI)

But there is also to say, that not the handle per se leaks, but it's assigned id. link,

so for example this statement was not very correct:

..., because it returns the group handle, and does not (handle-) leak.

I actually named some functions in "Things that always leak", but I did not finish it, because there are just too many of them I think.

What I can do is to write a all for one warning when using BJs, that they might leak.

But as long as they are not known for special bugs, like the "Camera - Pan camera as necessary...", they're mostly accepted for GUI users.

What do you say?

If you want to discuss further, please do!


Click to expand...

Yes you are right but as a user who don't likes hard codes I would prefer to have clean code even if I had to use jass little bit but actually its too bad to see these leaks on our approved resources because maybe no one has noticed these leaks before or they are just getting ignored


 Last edited: Mar 17, 2016

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

You're of course always free to your own ways for writing code.

I don't know if all GUI user will re-write the BJ functions, though, to avoid a reference leak.

If one can easily avoid it, then yes, but if not, then myself would probably not enforce it.

Addes this for now at "Things that always leak".

 I named some important ones, but there are actually more BJs that leak reference(s).

I won't list them all here, but if you use JNGP you easily can look it up in the functions list if you want to get sure.


Click to expand...

#### Arad MNK

#### Arad MNK

 Level 17

JoinedDec 11, 2014Messages2,003

Resources: 6

#### Resources
Maps 3Spells and Systems 2Tutorials 1

 I named some important ones, but there are actually more BJs that leak reference(s).

I won't list them all here, but if you use JNGP you easily can look it up in the functions list if you want to get sure.


Click to expand...

Add this again

Under TESH, "Function list..." ;p

Or opening Blizzard/Common.j .

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

 A unit will always keep a tiny bit of memory, even when removed properly. (0.04 kb and unpreventable)


Click to expand...

0.04 kilobits? or you meant to write 0.04 kilobytes (0.04 KB)?

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

I think kilobytes.

#### Avahor

#### Avahor

 Level 16

JoinedSep 29, 2008Messages370

Resources: 13

#### Resources
Skins 6Maps 6Templates 1

hi mate:

There's a sintax error on reference leaks -> Technique of removal -> Extra notes -> good Example

 JASS:

function someUnitReturn
 bj_lastCreatedUnit = CreateUnit( ... )
 return u
 // Hooray! We still have access to the global variable :)
endfunction

var "u" isn't defined. that would cause some misunderstanding on readers.

Edit: very good and needed tutorial

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

Avahor said:

 hi mate:


There's a sintax error on reference leaks -> Technique of removal -> Extra notes -> good Example

 JASS:

function someUnitReturn
 bj_lastCreatedUnit = CreateUnit( ... )
 return u
 // Hooray! We still have access to the global variable :)
endfunction


var "u" isn't defined. that would cause some misunderstanding on readers.


Edit: very good and needed tutorial


Click to expand...


Furthermore, shouldn't it return the global not the local? Also, you mispelled local, under "Technique of Removal"

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Oh, right!


It should be return bj_lastCreatedUnit. Changed it.

 very good and needed tutorial


Click to expand...

Thanks

 Also, you mispelled local , under "Technique of Removal"


Click to expand...

Lol, three times. Copy & paste ftw.


Fixed. Thanks, guys.

#### blancfaye7

#### blancfaye7

 Level 10

JoinedMay 21, 2014Messages580

Resources: 1

#### Resources
Maps 1

 If the black screen freezes after game-end it may be an indicator for huge amount of leaks.


Click to expand...


Well...! My map does this.

 But well, 0.04 kb is not much. Don't think too much about it.


Click to expand...

I'm always pointing to permanent unit leaks. My triggers don't cause leaks (as rechecked by other people too; except for the permanent and once per game leaks.)


Bah! I guess I need to calculate if it's really them.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

If the map is big enough/has enough "stuff", it might freeze for a bit anyway. It just happens in general when a map has used a lot of memory.

Just be sure to check your triggers for leaks and you'll be fine.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Back then I remember when we played our beloved ice escape maps we had a black screen for several or more minutes. (depending on game time)


After 1 year or so we found it is because of memory leaks and removed them.


Though during this "leak-time" the map was somehow still playable pretty well, even after long sessions. Idk how.


If your screen freezes for a few seconds for 1 hour of playing, I don't think it is a real problem.


And as PnF said, it also might be of normal map's memory consumption.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

the most likely cause of this is that wc3 starts to deallocate all the memory that the game allocated during that map, and if you have to deallocate chuncks of like 20 bytes, but overall 400 MB, it takes extremly long(in comparision, calling destructor of std::vector<int> in C++, which deallocates the memory) with like couple dozens of milions of integers will take like few seconds too, and that is one giant contiguous array, it isnt split among many many places)

#### blancfaye7

#### blancfaye7

 Level 10

JoinedMay 21, 2014Messages580

Resources: 1

#### Resources
Maps 1

Just what I wanted to know and hear... thanks guys.

Nice updates since I last visited last year though. Keep it up!

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

PurgeandFire said:

 Just be sure to check your triggers for leaks and you'll be fine.


Click to expand...


And for God's sake please do not use Hive's leak tester and manually check your own leaks. Please.

#### blancfaye7

#### blancfaye7

 Level 10

JoinedMay 21, 2014Messages580

Resources: 1

#### Resources
Maps 1

KILLCIDE said:

 And for God's sake please do not use Hive's leak tester and manually check your own leaks. Please.


Click to expand...


Regarding these, it feels like this is something I need to watch out for myself IMHO.

#### StoPCampinGn00b

#### StoPCampinGn00b

 Level 32

JoinedApr 2, 2013Messages3,954

Resources: 1

#### Resources
Maps 1

Pretend the use of "I" or "me" in my post is myself as a new modder.


 What is a memory leak said:



 Memory leak is if you can't refer to a used part in the memory.

The memory part is occupied, but during runtime the user has no

possibility anymore to read or manipulate the said part in memory.

You can say that leaked memory is unuseable and will hold up space for just nothing


Click to expand...

This doesn't tell my what a memory leak is in relation to modding WC3 / SC2. It gives the base technical coding details of what it does in general, but not what's actually important to know from my perspective. It means very little to anyone who has no background in modding, which is why it was probably skimmed over by anyone reading this.

What's important to know for your readers is more valuable than having a spiffy English class definition.


 Why are memory leaks bad said:



 Single leaks won't affect anything but the dignity of a programer,

but tons of leaks instead will heavily affect the runtime process and may

lead to unsmooth execution, lag-spikes, and in extreme cases to unuseability of the application.


Click to expand...

There should be more emphasis on leaks causing maps to have bugs, crashes, and lag. It states so, but in more vague and general ways without mentioning maps.

So yeah, those are my suggestions . This came to my mind because I'm suggesting to link this in our JASS / Leak Checker feature up top.


 Last edited: Sep 17, 2016

#### Deleted member 219079

D

#### Deleted member 219079

Amen!

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

At first I want to introduce the reader of what a memory leak actually is, in general. I think it's fine.

Though I agree I should add something to relate also to warcraft specificaly in the end of the introduction. I will add a little paragraph for it, thanks.

 You must log in or register to reply here.


### Similar threads

 Basic Memory Leaks


Ralle

Jan 3, 2006

Trigger (GUI) Editor Tutorials

Replies10Views62K

Mar 30, 2017
IcemanBo

 How to remove Memory Leaks IN GUI


KingB00ker

Jul 6, 2009

Tutorial Archive

Replies18Views6K

Jul 12, 2009
Pyritie

Sticky

 Things That Leak


Ralle

Jun 10, 2007

Triggers & Scripts
262728

Replies1KViews338K

Jun 17, 2024
Wrda

Locked

[Trigger]
 Does this memory leak? help me check~


jareph

Jul 17, 2008

Triggers & Scripts

Replies3Views2K

Jul 17, 2008
Huurka

 [GUI] Advanced Triggering Tips


Bribe

Mar 1, 2011

Trigger (GUI) Editor Tutorials

Replies23Views27K

Dec 20, 2012
PurgeandFire

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