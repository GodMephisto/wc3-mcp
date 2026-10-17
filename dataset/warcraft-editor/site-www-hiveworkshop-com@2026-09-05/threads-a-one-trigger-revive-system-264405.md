---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/a-one-trigger-revive-system.264405/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# A One Trigger Revive System
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/a-one-trigger-revive-system.264405/>
#### MantisHair

#### MantisHair

 Level 4

JoinedJun 30, 2014Messages72

Resources: 1

#### Resources
Tutorials 1

# Player-Number Based Hero Revive

#

#

## Basic Description

##

##

It's JASS-free. It's scriptless. It's codeless. This is a hero revival system, that, believe it or not, only uses a single trigger. When a hero dies, the trigger uses the player number of the player who owns that hero to keep each hero separate. Of course, this means that the players must all have only one hero. Remember this, it's very important. Because of this, this system works best in RPG maps where players usually have one hero. In my example, I used a condition that makes the trigger affect only one Force (team). In theory, I could have as many of this trigger as I had teams. May require one copy of each variable per copy of the trigger.

##
 The Trigger (Yes, only one)

##

##

Wait... First is:

##
 A List Of Variables

##

##

All these Arrays! Looks complicated to someone who little triggering experience, but help keep everything separate, which is one of the benefits of this trigger.

Default Value is 0, empty, nothing unless otherwise noted.

-ReviveHero_Array

A Unit array to hold each of the heroes. The array Index maximum should be equal to the number of players who have a hero this trigger will revive.

-RevivePlayer_Array

An Integer array. Holds the player number of each player this trigger will revive a hero for. The array Index maximum should be equal to the number of players who have a hero this trigger will revive.

-RevTimer

A Timer array. Holds a separate timer for each player this trigger affects.

-TimerStart

A Real variable. Stores a Real value (I know it sounds/looks funny), in this case the starting time of the timer. Doesn't need to be an array, because all the timers are the same starting time (10 seconds for mine.)

-RevTimeWindow_Array

A Timer Window array. Holds a separate timer window for each player this trigger affects. The array Index maximum should be equal to the number of players who have a hero this trigger will revive.

## The Trigger Itself

##

The trigger as it appears, with my comments.

HeroRevive
Events
Unit - A unit Dies

Conditions
((Dying unit) is A Hero) Equal to True

((Dying unit) belongs to an ally of Player 1 (Red)) Equal to True

((Dying unit) has an item of type Ankh of Reincarnation) Equal to False

Actions
-------- Keeps track of which players have died --------

Set DeadYet[(Player number of (Owner of (Dying unit)))] = True

-------- Keeps the player's Hero in a unit array, Indexed to their player number --------

Set ReviveHero_Array[(Player number of (Owner of (Dying unit)))] = (Dying unit)

-------- Keeps the player's player number in an integer array indexed to their player number --------

Set RevivePlayer_Array[(Player number of (Owner of (Dying unit)))] = (Player number of (Owner of (Dying unit)))

-------- ^Now each player has an index slot equal to their player number --------

-------- A countdown until your character revives, one for each player --------

Countdown Timer - Start RevTimer[RevivePlayer_Array[(Player number of (Owner of (Dying unit)))]] as a One-shot timer that will expire in 10.00 seconds

-------- Later in the trigger, it becomes important to know the initial time of the timer --------

Set TimerStart = (Initial time for RevTimer[RevivePlayer_Array[(Player number of (Owner of (Dying unit)))]])

-------- A timer window gives the player(s) a visual countdown --------

Countdown Timer - Create a timer window for RevTimer[(Player number of (Owner of (Dying unit)))] with title (Name of (Owner of (Dying unit)))

-------- These keep each timer and timer window player-specific --------

Set RevTimeWindow_Array[(Player number of (Owner of (Dying unit)))] = (Last created timer window)

Set RevTimer[(Player number of (Owner of (Dying unit)))] = (Last started timer)

-------- This allows a timer to start, and then check when it expires (initial time = time elapsed), in one trigger --------

Wait until ((Integer(TimerStart)) Equal to (Integer((Elapsed time for RevTimer[RevivePlayer_Array[(Player number of (Owner of (Dying unit)))]])))), checking every 0.50 seconds

-------- Now that the timer has expired, removes the timer window for that player --------

Countdown Timer - Destroy RevTimeWindow_Array[(Player number of (Owner of (Dying unit)))]

-------- Revive the player's hero at their spawn point (just put all the player start locations on top of each other in the dungeon's first room) --------

Hero - Instantly revive ReviveHero_Array[(Player number of (Owner of (Dying unit)))] at ((Owner of (Dying unit)) start location), Show revival graphics

-------- I'm currently working on a system to move the revive location to each "checkpoint" a player passes --------

Feedback is much appreciated.


 Last edited: Apr 10, 2015

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

I'm a very anti wait user.

Wait/TriggerSleepAction has offset around 0.27 seconds, and it's not good.

#### MantisHair

#### MantisHair

 Level 4

JoinedJun 30, 2014Messages72

Resources: 1

#### Resources
Tutorials 1

About waits

I know some people don't like them. I know that waits are not perfect. 0.27 seconds is an acceptable margin of error in this trigger. (more than 1 second would be bad).

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

1. you don't need the AND bla bla bla because the AND applies by default.

2. Remove the *insert comment here* from the trigger, it completely ruins the readability. Post those comments in the tutorial itself.

3. the revive location is invalid. Most users want to revive the unit at the location where they spawned or died.

I think you need to work more on this if you want it to be approved.

Off Topic: I wonder if the wait here is inaccurate.. since he use a timer.. and timers are very accurate..

#### MantisHair

#### MantisHair

 Level 4

JoinedJun 30, 2014Messages72

Resources: 1

#### Resources
Tutorials 1

Feedback is good.

Removed the "AND..." thing form the conditions.


Made the comments actual trigger comments, and better explained within those comments.


The revive location being the player start location works in the design of the map I made this for, where all the player start locations are in the first room of the dungeon.

#### BloodDrunk

#### BloodDrunk

 Level 18

JoinedMay 11, 2012Messages2,108

Resources: 1

#### Resources
Maps 1

or you could just use indexing method, and 2 triggers, and make it work with every hero (even more per player).

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

It is not a bad technique, but: (1) it is more of a system rather than a tutorial (2) it is not scalable (e.g. for a non-MPI design). Also, the tags are messed up. Be sure to close each tag with a slash, e.g. [h2][/h2].

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

I agree with already mentioned points, and it is really more an already implemented system than a tutoria.

Are you still taking care about it?

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Submission:One-Trigger Revive System

Date:28 November 2016

Status:Graveyard

Note:

No response, no update, no better formatting. Graveyard.

 You must log in or register to reply here.


### Similar threads

Locked

[General]
 Hero Revival System


CampScouter

Sep 21, 2017

World Editor Help Zone

Replies6Views3K

Sep 21, 2017
BloodSoul

Locked

[General]
 Waygate sends different players to different places


MantisHair

Apr 8, 2015

World Editor Help Zone

Replies2Views2K

Apr 8, 2015
MantisHair

Locked

[Crash]
 WC3 Hero Revive Problem


pplDotA

Dec 25, 2018

World Editor Help Zone

Replies3Views2K

Dec 26, 2018
pplDotA

Locked

[Trigger]
 Hero Revival Trigger


En_Fuego

Jun 15, 2008

Triggers & Scripts

Replies8Views2K

Jun 16, 2008
En_Fuego

Locked

[Trigger]
 Hero Reviving


greasontim

Mar 26, 2009

Triggers & Scripts

Replies2Views2K

Mar 26, 2009
greasontim

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