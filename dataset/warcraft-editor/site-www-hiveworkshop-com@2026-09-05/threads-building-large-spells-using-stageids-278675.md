---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/building-large-spells-using-stageids.278675/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Building Large Spells Using Stageids
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/building-large-spells-using-stageids.278675/>
#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

Introduction

I've been asked many times before how I go about making large spells and where to start when doing so. So I've written this Tutorial to explain my method of doing it that I've used for years:

Creating large abilities with many parts can be daunting and difficult to wrap your head around. This tutorial explores a simplistic way to break the process down into more easily managed components that allow you to build up your spell at your own pace. This is not a tutorial for those who've just started out spell making but for those who wish to expand the size of their works but struggle to manage all the parts to their spells or simply don't know where to start. As such it assumes you know how to make an approvable spell

​
The Concept

This tutorial explores the "StageID" Methodology of creating large spells focusing on using distinct Parts and behaviours to your spell and putting them in separate "Stages" and filtering the main loop function of your spell to run these behaviours simultaneously:

- Think about the spell you want to create and all the components - if you've used Object Oriented Languages you'll be familiar with this process - think about what differences define these parts and what (if any) things they do in common, these form the basis of your Stages

 - Some complex behaviours may lead to needing additional stages, perhaps an object or entity changes behaviour completely halfway through?

 - With this method you do not need to worry about variable overlap when recycling so long as you follow the structure correctly which can make the process of spell creation faster and less hassle when debugging

 - The method allows you to use a single dynamic index/linked list/hashtable more easily when creating complex spells​

The method is an implementation of Switch/Enum

​
Implementing the method

Basic Structure

The key to this method is a StageID Variable/Value, it can be utilised by any indexing method as well as hash tables so you can use your preferred method. Apply the StageID the moment you create a part to your spell.

In this Tutorial we'll be using the following example: we want an ability which summons a turret, this turret then fires projectiles which slow units that they hit. This gives us three clear Stages:

- Turrets

 - Projectiles

 - Slowed Units​

We'll also be assuming our indexing method is the dynamic indexing method in this example but it should be simple enough to translate into the other methods

So in our loop we can immediately set up this layout as we know we want to filter based on the stages set out above

### GUI

### JASS

ST Slowing Turret Loop
Events
Time - Every 0.03 seconds of game time

Conditions

Actions
For each (Integer Index) from 1 to MaxIndex, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
StageID[Index] Equal to 1

Then - Actions
-------- Turret Actions --------

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
StageID[Index] Equal to 2

Then - Actions
-------- Projectile Actions --------

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
StageID[Index] Equal to 3

Then - Actions
-------- Slowed Unit Actions --------

Else - Actions

 JASS:

function ST_Slowing_Turret_Loop takes nothing returns nothing
 local integer i = 0

 loop
 exitwhen i > MaxIndex

 if StageID[i] == 1 then
 //Turret Actions
 elseif StageID[i] == 2 then
 //Projectile Actions
 else
 //Slowed unit Actions
 endif

 i = i + 1
 endloop

endfunction

Stage IDs can use any datatype you like so long as each one is distinct; for example instead of using "1, 2, 3" we could use "Turret, Projectile, Unit"

The GUI example can be cleared up by nesting the If statements in the Else section of the previous statement and the last one removed (taking the place of the else section of the second) but it's clearer in this format and can be moved later, in the case of JASS you can also replace the numbers with constant functions to make it even easier! In this case I'd recommend ST_TurretStageID(), ST_ProjectileStageID() and ST_UnitStageID()

​
Setting up the stages

So now we've got our general structure in place but, how does this help us?

Well we now have clear "IF" blocks to go to when we're working on a particular part of the spell so there's that

But more importantly there's never going to be any overlap between the stages so long as we index them correctly

So how do we index things correctly? Well Simple:

1) Do what you normally do for your chosen indexing method

2) Set the StageID for that index to match the relevent StageID in the if structure

3) Done​

### GUI

### JASS

Set MaxIndex = (MaxIndex + 1)

Set StageID[MaxIndex] = 1

-------- Set up everything else --------

 JASS:

set MaxIndex = MaxIndex + 1
set StageID[MaxIndex] = ST_TurretStageID()
//Set up everything else

​
Connecting the Stages

in complex spells however, these Stages tend to overlap - such as in our example where our turret shoots projectiles which slow units: They each lead on to the next stage!

No problem: We just do as we did before.

### GUI

### JASS

ST Slowing Turret Loop
Events
Time - Every 0.03 seconds of game time

Conditions

Actions
For each (Integer Index) from 1 to MaxIndex, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
StageID[Index] Equal to 1

Then - Actions
-------- Turret Actions --------

-------- (...) --------

-------- Turret fires a projectile! --------

Set MaxIndex = (MaxIndex + 1)

Set StageID[MaxIndex] = 2

-------- Set up projectile data --------

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
StageID[Index] Equal to 2

Then - Actions
-------- Projectile Actions --------

-------- (...) --------

-------- Projectile Hits a unit! --------

Set MaxIndex = (MaxIndex + 1)

Set StageID[MaxIndex] = 3

-------- Set up unit data --------

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
StageID[Index] Equal to 3

Then - Actions
-------- Slowed Unit Actions --------

Else - Actions

 JASS:

function ST_Slowing_Turret_Loop takes nothing returns nothing
 local integer i = 0

 loop
 exitwhen i > MaxIndex

 if StageID[i] == ST_TurretStageID() then
 //Turret Actions
 // (...)
 //Turret fires a projectile!
 set MaxIndex = MaxIndex + 1
 set StageID[MaxIndex] = ST_ProjectileStageID()
 //Set up projectile data
 elseif StageID[i] == ST_ProjectileStageID() then
 //Projectile Actions
 // (...)
 //Projectile hits a unit!
 set MaxIndex = MaxIndex + 1
 set StageID[MaxIndex] = ST_UnitStageID()
 //Set up slowed unit Data
 else
 //Slowed unit Actions
 endif

 i = i + 1
 endloop

endfunction

Congratulations, if you follow this structure you now no longer have to worry about variable overlapping when it comes to your recycling and you have clear, distinct sections to work on each part of your spell.

If you want to change the behaviour of a specific entity (projectile lands and becomes a turret) you simply change it's own StageID at the appropriate time and you don't need to create a new index

If you have common behaviours between StageIDs then you can separate based on that common factor before separating by the StageID (check if the StageID is either of those two, then later check which one it is specifically)

When you recycle things you only need to swap the data specific to that StageID with the recycled one - to recycle minimal data you should check what stageID the one you're swapping it with is in to know what data needs to be swapped, otherwise you would still need to swap all data

Examples of Spells that use StageIDs

### Dynamic Indexing

### Linked Lists

### Hashtables

 JASS - Chaos Envoy

 GUI - Helios Collision

 JASS - Punishing Particles

 GUI - Explosive Toxic Gas

 GUI - Dark Portal

 GUI - Void Sphere

​
Wrap Up

If you still find it hard to understand, feel free to PM me any time.

Thank you for reading.

~Tank-Commander

​


 Last edited: Jun 7, 2016

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Requesting use of
​

#### Meatmuffin

#### Meatmuffin

 Level 11

JoinedJul 25, 2014Messages490

Resources: 10

#### Resources
Maps 2Spells and Systems 7Tutorials 1

Instant vote for approval, first time I used this was not too

long ago, in an unsubmitted spell of mine, worked perfectly.

Really useful if you want lots of unique movement having

units in your ability.

I also submitted a spell in the Zephyr contest using this, just

as Tank-Commander, for a much clearer example you can

check both our spells out. Do note that they are both written

in JASS. I think KILLCIDE will be submitting a GUI entry

in the contest using this, maybe then you can link some

spell examples on how to use StageID's.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

I always use this on my previous spells

Good thing Tank created a tutorial for the semi-beginners

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Awesome tutorial. A few people have asked about this in the past, and you've explained it well.

It really helps organize your logic and keep things compact, which is always a plus from me.

Approved!

EDIT: I've moved this to GUI triggers, even though this contains some JASS. Is that okay? Or would you rather have it elsewhere? (e.g. general mapping)

#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

I think it's perfectly fine for it to be in the GUI tutorial section - it wouldn't really fit in any non-specific section like general mapping

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

If you are willing to add samples:

https://www.hiveworkshop.com/forums/spells-569/dark-portal-v1-5-a-221829/

https://www.hiveworkshop.com/forums/spells-569/void-sphere-v1-4-a-224044/

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

This tutorial essentially teaches how to write functions in GUI without custom script. Highly recommended.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Xonok said:

 This tutorial essentially teaches how to write functions in GUI without custom script. Highly recommended.


Click to expand...


"write functions"


in what way? please englighten me.

#### Xonok

#### Xonok

 Level 21

JoinedMar 27, 2012Messages3,231

Resources: 8

#### Resources
Spells 3Tutorials 5

Well, in JASS I would do a similar thing by having separate timers and a callback function for each.


This approach here puts them all on a single timer and lets you index multiple kinds of data in a single array by just giving each a type. The end result is the same as if you had a function for each type, but it's easier so it can be done in GUI.

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

Minor syntax error in your JASS code. When you're comparing if something is equal to something else, you're only using one = instead of two

#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

Fixed the comparisons

 You must log in or register to reply here.


### Similar threads

 Building large spells: Spell Cores


Tank-Commander

May 7, 2016

Tutorial Archive

Replies26Views6K

Oct 8, 2017
IcemanBo

 MUI Spells Using Artificial Waits


Tank-Commander

Sep 28, 2012

Trigger (GUI) Editor Tutorials

Replies32Views20K

Oct 16, 2015
BETABABY

Locked

 Help Indexing


HolyBear

Dec 28, 2013

World Editor Help Zone

Replies2Views2K

Dec 29, 2013
IcemanBo

Locked

[Trigger]
 Mui


Wolf_Wing

Dec 26, 2012

Triggers & Scripts

Replies16Views2K

Jan 2, 2013
Ofel

Locked

[Trigger]
 Why dynamic indexing is flawed and people should use linked list instead


Maker

Jun 27, 2013

Triggers & Scripts
2

Replies55Views10K

Sep 19, 2013
Maker

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