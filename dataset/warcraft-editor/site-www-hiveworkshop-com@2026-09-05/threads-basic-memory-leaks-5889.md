---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/basic-memory-leaks.5889/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Basic Memory Leaks
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/basic-memory-leaks.5889/>
#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


by thunder_eye...


Lets start with a brief overview of how leaks happen, or more precisely, how handle object leaks happen. These memory leaks are what we deal with the most, there are others, like string leaks (which can't be prevented at all, but are usually insignificant), and the problem with local handle variables (which is a part of handle object leaks but won't be covered here for the sake of keeping it short and simple, because local variables are only used in JASS anyway). By the way, custom scripts are JASS.


Handles are game objects... units, doodads, destructibles, items, special effects and also regions, locations, unit groups, player groups... all this stuff. Things that are not handles are integers, reals, booleans; these are just numbers. They are stored in their variables and are erased the moment the variable is set to a new value.


Handle variables, on the other hand, are pointers: they only contain a number that points to where in the game's memory the object is located. Handle objects exist independent of variables. They can exist without any variables pointing to them, in that case, they can't be removed from memory because there's no way to tell the functions that are used to remove them where in the memory they are. Such unreferenced handles are only cleared from memory at the end of a game by a garbage collector (the source of end game delay), but while the game lasts, they accumulate and can cause performance issues.


Now, there are plenty of functions that create handles. Center of region, random point in region, position of unit, target point of ability being cast... and many others create a location object, which is usually used to create units at, or special effects, or for measuring distances between points... Units in region, units owned by player, units in range,... all these functions give you a Unit Group. Then there are player groups and all other things already mentioned and more.


You may only need these objects for a moment, but the game can't know that, so they don't just go away by themselves when you no longer need them. The location doesn't just disappear once a unit is created at it. We must use special functions to dispose of the handles we only create temporarily and don't need them to last. The most commonly used are:

RemoveLocation()


DestroyGroup()


DestroyForce()


...


There are more, but these are all an apprentice triggerer really needs to know. There is a GUI action for removing a special effect, so we don't need to know the JASS function for that, units are removed from memory automatically once their corpses decay, and anything else is usually too rare to care about. If we are practical, the only thing we really need to care about are temporary handles that are created dynamically and rapidly, in triggers that run often or loop a lot. So what if a cinematic trigger that runs only once leaks a few locations? It's the tens of thousands of locations or unit groups that can leak on a fast periodic trigger that matter.


As has already been mentioned, to remove a temporary handle that has served it's purpose, we need to know where it is, we need to have a variable pointing to it. The only way to do that is to point a variable to it when we create it. For that, we first need some variables that we'll use for this purpose, in my examples I'll name them tempPoint and tempGroup. They are global variables that you define in the variable editor, and because of that they get the "udg_" prefix in JASS scripts.


Now, let's say we want to create a footman at the center of playable map area. If we didn't care about memory leaks, we would do it like this:

Unit - Create 1 Footman for (Player 1 - Red) at (center of (playable map area)) facing 0.00 degrees


However, the (center of (playable map area)) creates a location object at which the footman is then created, and this location then "leaks" because we can't remove it, because there's no way to know where in memory it is. We avoid this leak like this:

Set tempPoint = (center of (playable map area))


Unit - Create 1 Footman for (Player 1 - Red) at tempPoint facing 0.00 degrees


Custom script:

call RemoveLocation( udg_tempPoint )


and similarly for everything else, let's say we want to kill all units in a region:

Set tempGroup = units in TheUnholyRegionOfVanillaFlavouredDoom <gen>


Unit group - pick every unit in tempGroup and do: Kill (picked unit)


Custom script:

call DestroyGroup( udg_tempGroup )


That is about it concerning the basics of memory leaks. Remember not to worry about every petty detail, if you have an old map which you want to fix a bit, just clear the most critical special effect, unit group and location leaks and any noticeable problems with lag will be solved. On future maps, you can afford to be a bit more precise and thorough in hunting memory leaks.


One last note, it is said that destroying triggers after you no longer need them can be quite effective performance-wise, especially with long initialization triggers that run once and are then not used again. To do that, just put the following line at the end of the trigger.

call DestroyTrigger(GetTriggeringTrigger())

Leak Check 1.3 is a handy tool to help find and eliminate memory leaks.


 Last edited by a moderator: Mar 28, 2007

#### troylowyee

#### troylowyee

 Level 5

JoinedApr 13, 2008Messages184

Resources: 3

#### Resources
Maps 3

hi i have a question, what if i created units at a region instead? do i have to create a variable?

#### pOke

#### pOke

 Level 13

JoinedMar 24, 2013Messages1,105

Resources: 1

#### Resources
Maps 1

If you're doing Center of or Random Point in X Region. Yes you need to do as shown above and Set that in a variable and remove it via call RemoveLocation(udg_myVariable)

#### troylowyee

#### troylowyee

 Level 5

JoinedApr 13, 2008Messages184

Resources: 3

#### Resources
Maps 3

Oh thanks i didnt know that, my map lags after awhile and im trying to clear as much leaks as possible!

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Read this newer thread by Ralle, it explains pretty straight forward how to remove all important objects: Things That Leak

#### troylowyee

#### troylowyee

 Level 5

JoinedApr 13, 2008Messages184

Resources: 3

#### Resources
Maps 3

what about pick every unit in (playable map area)?

Or what about panning the camera to center of region?

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

troylowyee said:

 pick every unit in (playable map area)?


Click to expand...

makes a group

troylowyee said:

 panning the camera to center of region?


Click to expand...

makes a point/location

You need to use variables for destroying them after usage, like showed here or in linked thread (linked thread is advised).

#### troylowyee

#### troylowyee

 Level 5

JoinedApr 13, 2008Messages184

Resources: 3

#### Resources
Maps 3

I have read the thread, its really useful but if the pan camera always leaks then what can i use to pan camera? etc when a unit spawns and you want to pan it to the unit

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

troylowyee said:

 but if the pan camera always leaks then what can i use to pan camera?


Click to expand...

why you say "always leaks" ?

#### Michael the Boombringer

#### Michael the Boombringer

 Level 5

JoinedDec 18, 2016Messages92

What happens if memory leaks?

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Explained here: Memory Leaks

 You must log in or register to reply here.


### Similar threads

 Memory Leaks


IcemanBo

Mar 6, 2015

Trigger (GUI) Editor Tutorials
2

Replies95Views79K

Jul 25, 2024
corpuzmarvicjune

 How to remove Memory Leaks IN GUI


KingB00ker

Jul 6, 2009

Tutorial Archive

Replies18Views6K

Jul 12, 2009
Pyritie

 Essentials Tutorial


Deuterium

Aug 22, 2009

Trigger (GUI) Editor Tutorials
2

Replies78Views26K

Dec 8, 2014
Justin Weaver

 Reviving Heroes at a Changeable Region


dOT145

Mar 23, 2010

Trigger (GUI) Editor Tutorials

Replies12Views10K

Apr 30, 2010
PurgeandFire

 [Reviewed: BlinkBoy] Understanding triggers and creating them.


Zypher

Aug 20, 2008

Trigger (GUI) Editor Tutorials

Replies9Views6K

Sep 5, 2008
BlinkBoy

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