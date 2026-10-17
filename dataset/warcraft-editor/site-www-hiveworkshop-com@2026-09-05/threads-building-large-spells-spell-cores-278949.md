---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/building-large-spells-spell-cores.278949/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Building Large Spells Spell Cores
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/building-large-spells-spell-cores.278949/>
#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

Introduction

When working with large spells with many components data management poses a large challenge - while it's simple to pass data through each part as needed - it leads to lots of data duplication, normally to avoid recalculating data. This tutorial explains how to use Spell Cores in order to reduce data duplication while maintaining efficiency with minimal calculations, as it is dealing with large works this tutorial assumes you know how to make an approvable spell.

Notes:

 - This method be used with linked lists of hash tables - complications ensue when using dynamic indexing.

 - It's recommended you read Building large spells: StageIDs before continuing with this tutorial

​
The Concept

What is a spell core? - A Spell Core is generally the first component of any spell. But it has these key defining features:

- It is used to store all non-unique information needed for the spell along with its own information.

 - It persists until the end of the spell (or until the end of when non-unique information is needed)

 - It does not need to refer to anything physically (but it can and in most cases should)​
How is it useful?

- The spell core has one key use: Whenever referring to non-unique information in a spell the information is fetched from the spell core, removing the need to duplicate information or transfer it to each part of the spell as need be, this results in one-time calculations and minimal data duplication at the same time.

 - However it can also be used to tie components of spells to other parts as information such as location can be taken from the spell core

 - By utilising multiple Spell Cores in a single spell it's possible to create a "Core Tree" where any branch or leaf on the tree can easily access data from the Root of the tree but also other information like location from each branch that precedes it

 - If the two uses above are the only thing useful to you (as your existing method already minimises data duplication and calculations) these things will be explained in the "Advanced Spell Cores" tutorial (TBA)

​
Technically speaking a spell core can be nothing more than an extra instance in your spell and be purely data but it's less efficient than using something actually useful in your spell

The method is an implementation of Pointers in conjunction with Dynamic indexing/Linked Lists/Hash tables​

Implementing the method

For the sake of simplicity we'll be assuming the indexing method used for this spell is Dynamic indexing as indexing is simpler - However this method is significantly improved by using linked lists or hash tables​

Creating a spell core

The key to this method is using a SpellCore variable - you can call it whatever you like and it'll almost always be an integer this is because the value you're storing is the index of the Spell Core or the HandleID of a unit used as one (when using hash tables the SpellCore can be a unit variable) making one is fairly simple:

1) Do what you normally do for your chosen indexing method

2) Is this the Spell Core for this spell?

- If Yes - Set the Spell Core for that index to its own index

- Apply all non-unique data to this index​
- If No - Set the Spell Core for that index to match the relevant Spell Core for that instance

3) Done​

### Is Spell Core

### Isn't Spell Core

### GUI

### JASS

Set MaxIndex = (MaxIndex + 1)

Set SpellCore[MaxIndex] = MaxIndex

-------- Apply all non-unique data --------

-------- Set up everything else --------

 JASS:

set MaxIndex = MaxIndex + 1
set SpellCore[MaxIndex] = MaxIndex
//Apply all non-unique data
//Set up everything else

### GUI

### JASS

Set MaxIndex = (MaxIndex + 1)

Set SpellCore[MaxIndex] = SpellCore[Index]

-------- Set up everything else --------

 JASS:

set MaxIndex = MaxIndex + 1
set SpellCore[MaxIndex] = Index
//Set up everything else

But what is "All non-unique data"? Well taking the example from Building large spells: StageIDs our turret fires projectiles, all those projectiles deal the same amount of damage, have the same size, etc. and all our slowed units are slowed by the same amount, for the same duration, etc. all this data must be worked out in advance and there's two ways to do that:

- All data is worked out in a configuration specifically stating the value for every level (Passes level to each stage, does not use a spell core)

 - Data is worked out at the start of an instance using "Base + (PerLevel * level)" values where Base and PerLevel for each value is stated specifically in the configuration (uses a Spell Core)

 - Data is undetermined entirely by configuration but will be determined by another factor (e.g. Damage = Targets in AOE * DamagePerTarget or Damage = TimeChannelledFor * DamagePerSecondChannelled)​

If you are using the first listed method only then the uses of this methodology are reserved to the "Advanced Spell Cores" tutorial (TBA)​

Fetching data from the Spell Core

Whenever you need to fetch information stored in the spell core you nest indices within each other in order to get back to the Spell Core's index in the following format: Data[SpellCore[Index]]

So say we wanted to damage a unit because our projectile from our turret has hit a unit

### GUI

### JASS

Unit - Cause (Turret[SpellCore[Index]]) to damage (Target) dealing (ProjDamage[SpellCore[Index]]) damage of attack type TurretAttackType and damage type TurretDamageType

 JASS:

call UnitDamageTarget(Turret[SpellCore[Index]], Target, ProjDamage[SpellCore[Index]], true, false, TurretAttackType, TurretDamageType)

Practical Application

So let's take our example again and see how using a SpellCore with it would look like as opposed to not using one, we'll assume the data has already been applied to the Spell Core correctly and that the Turret is the Spell Core. We'll also only do it for the projectile damage to showcase how much is saved for three values (Turret, health, and mana damage) by using a Spell Core over not using one

### Using Spell Core

### Not using Spell Core

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

Set SpellCore[MaxIndex] = Index

-------- Set up projectile data --------

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
StageID[Index] Equal to 2

Then - Actions
-------- Projectile Actions --------

-------- (...) --------

-------- Projectile Hits a unit! --------

Unit - Cause (Turret[SpellCore[Index]]) to damage (Target) dealing (ProjHealthDamage[SpellCore[Index]]) damage of attack type TurretAttackType and damage type TurretDamageType

Unit - Set mana of (Target) to (Mana of (Target) - ProjManaDamage[SpellCore[Index]])

-------- Index unit --------

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
 set SpellCore[MaxIndex] = i
 //Set up projectile data
 elseif StageID[i] == ST_ProjectileStageID() then
 //Projectile Actions
 // (...)
 //Projectile hits a unit!
 call UnitDamageTarget(Turret[SpellCore[i]], Target, ProjHealthDamage[SpellCore[i]], true, false, TurretAttackType, TurretDamageType)
 call SetUnitState(Target, UNIT_STATE_MANA, GetUnitState(Target, UNIT_STATE_MANA) - ProjManaDamage[SpellCore[i]])
 //Index unit
 set MaxIndex = MaxIndex + 1
 set StageID[MaxIndex] = ST_UnitStageID()
 //Set up slowed unit Data
 else
 //Slowed unit Actions
 endif

 i = i + 1
 endloop

endfunction

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

Set Turret[MaxIndex] = Turret[Index]

Set ProjHealthDamage[MaxIndex] = ProjHealthDamage[Index]

Set ProjManaDamage[MaxIndex] = ProjManaDamage[Index]

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
StageID[Index] Equal to 2

Then - Actions
-------- Projectile Actions --------

-------- (...) --------

-------- Projectile Hits a unit! --------

Unit - Cause (Turret[Index]) to damage (Target) dealing (ProjHealthDamage[Index]) damage of attack type TurretAttackType and damage type TurretDamageType

Unit - Set mana of (Target) to (Mana of (Target) - ProjManaDamage[Index])

-------- Index unit --------

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
 set Turret[MaxIndex] = Turret[Index]
 set ProjHealthDamage[MaxIndex] = ProjHealthDamage[i]
 set ProjManaDamage[MaxIndex] = ProjManaDamage[i]
 elseif StageID[i] == ST_ProjectileStageID() then
 //Projectile Actions
 // (...)
 //Projectile hits a unit!
 call UnitDamageTarget(Turret[i], Target, ProjHealthDamage[i], true, false, TurretAttackType, TurretDamageType)
 call SetUnitState(Target, UNIT_STATE_MANA, GetUnitState(Target, UNIT_STATE_MANA) - ProjManaDamage[i])
 //Index unit
 set MaxIndex = MaxIndex + 1
 set StageID[MaxIndex] = ST_UnitStageID()
 //Set up slowed unit Data
 else
 //Slowed unit Actions
 endif

 i = i + 1
 endloop

endfunction

As can be seen, while if we were transferring one piece of information the code lines and efficiency remain the same - but for every additional piece of information needed a line is saved, so for in your large spells (in which large quantities of data will be needed at every stage) the amount of data duplication saved by using this method becomes phenomenal and cuts down the code lines you're using

​
Recycling

There's a number of things to be aware of when you recycle spells which make use of a spell core structure:

- When the spell core is recycled the data stored on it should not be accessed by any other components of the spell as the data will be overwritten by another index thus making it inaccurate and causing bugs

- If you are using linked lists you can however use this data for one iteration, the same iteration that the spell core was recycled on (assuming you do not null the data when you recycle it)

- As a consequence generally speaking the Spell Core should outlast all components that reference it, unless they no longer need the data​
- If you don't want to keep the physical presence of a Spell Core around longer than the components that use it (for example the turret dies before all bullets do) you can destroy the physical effect of it but keep the data until you know all the other components are dead

- You can use a secondary loop checking for "SpellCore[TempNode] == Node" to locate all indexes which use the current node as a SpellCore, be aware that the Spell Core will often reference itself and thus will find itself when searching this way. You can use this to destroy all components when the Spell Core expires or mark them to change behaviour/recycle/whatever else

- Linked lists can use TempNode = NextNode[Node] as a starting point as all things referencing the Spell Core will be indexed after it, thus avoiding this collision​

There's a good reason why this method is generally not to be used with dynamic indexing - whenever you recycling indexes move (unlike hash tables and linked lists) meaning that whenever you recycle you must first locate every index which uses the index you're recycling as a spell core (only do this when what you're recycling is a spell core) and update their SpellCore

Wrap Up

If you still find it hard to understand, feel free to PM me any time.

Thank you for reading.

~Tank-Commander

​


 Last edited: Jan 25, 2017

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

Bumping because I want my vector physics tutorial.

#### FUJI

#### FUJI

 Level 21

JoinedAug 13, 2013Messages1,709

Resources: 16

#### Resources
Spells and Systems 16

Tank-Commander said:

 There's a good reason why this method is generally not to be used with dynamic indexing


Click to expand...

* The only not easy to understand is why did you choose to show the Dynamic Indexing method in this spell core if it is not generally use? ^^

* Or:

Tank-Commander said:

 - whenever you recycling indexes move (unlike hash tables and linked lists) meaning that whenever you recycle you must first locate every index which uses the index you're recycling as a spell core (only do this when what you're recycling is a spell core) and update their SpellCore


Click to expand...

Include that also in your illustration maybe?

I fully understand the concept that it is like a 2D array ( substitution ) on unique data informations which you need to save per phase or stage instead of passing informations one by one manually which makes writing the code very slow. You must also include how will you deindex using this spell core which is not showed in the method illustration if possible.

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

jakeZinc said:

 The only not easy to understand is why did you choose to show the Dynamic Indexing method in this spell core if it is not generally use? ^^


Click to expand...

Tank-Commander said:

 For the sake of simplicity


Click to expand...

#### FUJI

#### FUJI

 Level 21

JoinedAug 13, 2013Messages1,709

Resources: 16

#### Resources
Spells and Systems 16

If it is for simplicity then its okay but make it also usability in cases ^^.

Its better to show this method on the recommended ones if it is possible but Im not gonna force you to do so

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

The allocation for linked list would make the tutorial convoluted  the idea you get from it is that you use the ID from the "first" instance.

jakeZinc said:

 You must also include how will you deindex using this spell core which is not showed in the method illustration if possible.


Click to expand...

I think this would be nice as well.

#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

The method is deindexed the same way other things are, the exception is when using dynamic indexing where essentially you must run a secondary loop when you deindex to reassign the home node, heck often you need to recycle those things as well to mark the spell as completed unless they only need their unique data (alternatively make sure the spell core is recycled only when everything else is finished which is what you should be doing when using a spell core for obvious reasons; recycling the spell core and not recycling anything else will lead to bugs), I may add it to the hidden read only if using dynamic indexing though frankly using them in combination is a bad idea (due to needing to run the secondary loop)

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Hey.

Erm, so it's goal is basicly not to create a new main instance, but this tequnique is basicly just for creating child instances, based on the main instance? And not to create new main instances.

So the created child instance will just refer to the main instance's index, and use it as reference for all actions, like damage, target/caster, etc.

The example would be the same as I would do this?:

Turret instance created (main instance):

Set MaxIndex_Turret = MaxIndex_Turret + 1

Set Target[MaxIndex_Turret] = ...

Set Damage[MaxIndex_Turret] = ...
and when projectiles created (child instances):

Set MaxIndex_Projectile = MaxIndex_Projectile + 1

Set Core[MaxIndex_Projectile] = Current_TurretIndex
==

I am not very sure why a linked list is better than dynamic indexing.

I mean, lets assume we use one linked list. All nodes, no matter if child or parent instance, must be recycled seperatly, or do I miss something?

When using dynamic indexing, I would not use a loop either, like implied in the tutorial.

I would also just have some destroy conditions, and if one is met, then the node will be indexed, similar behaviour like with linked list. (?)

==

But in case it really isn't the same like with linked list, then it would make probably sense to take

linked lists as requirement (maybe with tutotrial link), and to set it up just properly without the un-recommended dynamic indexing example.

 Advanced Spell Cores" tutorial (TBA)


Click to expand...

What is TBA?

#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

Yeah that's pretty much exactly what it is.

TBA means To Be Assigned or To Be Announced (depending on context) I've been staggering writing these tutorials in order of approval (one gets approved I write the next one)

The issue with dynamic indexing is a result of changing index numbers of the cores resulting in the children "forgetting" where the parent is or the need of extra markers depending on the spell structure but another more glaring issue is when a spell core expires e.g. in a given array

[1][2][3][4][5]

assume [2] is a spell core which is about to expire and all others are children of [2] (1 getting to where it is as a result of a previously recycled index)

if [2] expires in this instance and all other entities are destroyed at the same time as the core, [1] is now going to be out of sync by the timer rate, also any processes [1] attempts using data from the now-expired core will fail (obviously), a check like IsAlive[Parent] will also not necessarily work if a new instance is started within the gap of running instances, particularly if that instance is also a spell core

i.e.

[1][2][3][4][5] -> [1][recycle][recycle][recycle][recycle] -> 0.03 passes and a new spell core appeared [1][2 (spell core but not the original)][empty][empty][empty] | [1] fails to be recycled if a simple isAlive check is used

So when you recycle [2] you have to make all other instances aware that this spell core is expiring somehow in a way that's unique to it - a clear method being to loop through when you're going to recycle it and set a marker for all of its children before recycling it (i.e. you're a child of [2] if parent[node] == 2 & node != 2) and in that event set a boolean like wantRecycle[node] == true in order to recycle them or to recycle them then and there (though this will still put [1] in the above example out of sync slightly)

the forgetting issue occurs when the spell core is the last in an array

[1][2][3][4][5][6][7][8][9][10]

assume [8] is the spell core and 1-3 all belong to an instance about to expire while 4-10 belong to 8

[1][2][3][4][5][6][7][8][9][10] -> [10][9][3][4][5][6][7][8][empty][empty] (note that 4-10 all have parent[node] == 8) -> [10][9][8][4][5][6][7][empty][empty][empty]

now all the children need to have parent[node] changed to 3 as the core has been moved by the recycling method giving rise to the same problem as before

in addition if you use one dynamic array for all data you need to run a check whenever you recycle anything (if you use a generic recycle stage to dispose of things) via a parent[node] == node check (it is its own parent) or something similar

Both of these are avoided easily since linked lists don't move indexes around at all so parent[node] won't have to be changed and run in order of appearance (spell cores being normally made first, will run first and thus also avoids the desync issue mentioned first) so you can conveniently not have to deal with these things

If I'm missing a trick on how one could more easily avoid this (keeping in mind the tutorial is intended for both GUI and JASS users) while still using dynamic indexing then I'm all ears

I'm hesitant to say you -can't- use dynamic arrays for this method as I have implemented it myself in a number of my own submissions (Chaos Envoy for example does this) as said I just wouldn't recommend it since it gets messy particularly when debugging if you aren't aware of these issues

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Can/should childs even exist when core is expired? I mean:

Parent: 2

Child: 1 -> points to 2

Parent 3

Child: 4 -> points to 3

Parent "2" gets recylced and gets on the recycle stack.

A new parent gets created and takes the "2" from the recylce stack.

Parent "2" gets a child: "5"

Child "5" points to "2"

Child "1" points to "2" (still ?)
Don't you face this problem?

#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

they can if you use dynamic indexing (even if you set them to destroy when the parent dies unless you use a second loop as I'd explained above) since linked lists will always go to the core first the problem doesn't occur (as core expires -> marks all children for destruction -> children are after parent at all times (no new core can be made in the interim)

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Ah, so you really force then all childs to be destroyed when the core is destroyed, and they aren't allowed to pass some more intervalsl? That would answer my concern.

#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

yeah essentially that's it

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

I think it's not so easy to understand from the tutorial at the moment what we worked out above.

The relation from instances that represent the spell core and instances that only use a reference to this spell core could also be made a bit more clear. That

they can't be existant alone, and mustn't be recycled at any later time the spell core itself is recycled.

Their recyling process (parent node / child node) does/can also differ. Always, when there is core data to be cleaned, like maybe an instance-specifig group.

When child node is recycled, it must recycle nothing but the index, while when spell core is recycled, every data has to be normaly cleared, and it must be ensured

that all childs are being recycled, too, before the system can apply any new instances.

It might be clear for you, but I think it's important to point these things out a bit more, how the whole destroy methds must work, and why.

#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

I think you've somewhat gone off on a tangent, some of those things aren't true, particularly if you're focusing on dynamic indexing

 - There's no reason why children can't persist beyond the spell core (again, my Chaos Envoy spell does this) - instead their behavior changes after that point, it's just that they can't use data stored on the spell core (non-unique data) such as (in the case of chaos envoy) the centre of the portal or the strength of the gravity

 - The recycling process is only different if you are using dynamic indexing, it is the same if you use linked lists regardless of if you're recycling a child or core (hence the recommendation against dynamic indexing), to my knowledge the same is true for hashtables, (flush the hashtable, set the node up to be recycled for linked lists) any of my more recent submissions are an example of it not being any different in the linked list case. (I've only used the dynamic indexing version in the tutorial to minimize the excess code used to set up the dynamic index, also to be consistent with the first part of this tutorial series)

 - children pretty much always have their own unique data that would need to be cleaned just like any other method - the entire concept of spell cores is just to prevent duplicating non-unique data (such as spell level, caster and such things), if your spell is only non-unique data then you don't really need any children in your spell

 - your last point can be rebutted as an extension of my first point: you don't need to ensure their death as the result of a spell core death unless their behavior beyond the point of said spell core death is dependent on it, you can also do the inversion (don't kill a spell core until all the children are dead) e.g. if you have a many-projectile producing spell you can use a counter of how many are alive to control the spell core (children[node] = x, and then when a projectile dies set children[parent[node]] = children[parent[node]] - 1) and kill the core when that hits 0, though that wouldn't work if you had two children types and one doesn't die until the core does and the others die of their own accord (and the number of ones that don't is dynamic)

The difficulty with including extras like that is that they very much so depend on what you're using the spell core to do (is it integrated into your spell as a component or is it abstract, do you want behavior changes after they die, does it have more than one kind of child? etc. which will modify how you will want to handle the recycling process and would easily double or triple the length of the tutorial just covering all those scenarios (if I were to do them for each indexing method) I can expand on the complications certainly

Some of those things are things I intended on covering in the advanced tutorial specifically because it gets very convoluted very fast - I want to keep this one as streamlined as possible and get the concept across

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

What is the point of keeping child node if it may point to unwanted values when parent node index is being re-used?

Tank-Commander said:

 instead their behavior changes after that point


Click to expand...

so from example from the tutorial now a new unit would damage an other unit with an other damage amunt.

There is data directly dependant on some core values. This relation is the whole topic from the tutorial I believe.

It doesn't really make sense to me if the core actually can be destroyed/recycled alone, and the nodes will just live longer independantly and just change "some behaviour".

The main point is/was that all the nodes use that shared data from the spell core they reference.

edit: Yes, you're right. What I wanted to say actually is that a child does recycle child-only data, and no parrent/core data.


 Last edited: Nov 25, 2016

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Tank? Was I wrong in my post above?

#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

The basic idea of spell cores is to reduce data yes, though as I mentioned with examples in chat it has other more advanced uses - so this tutorial focuses almost entirely on the data reduction

I've also already expressed my motives for stating why they can persist longer & that it isn't a bad thing, I imagine the only thing that would particularly be convincing would be counterexample as explaining behavior change conceptually is a bit vague:

the idea of dealing a different amount of damage after the spell core ends wouldn't exactly be a good example, cases were it would be more apt. would be were no data that is non-unique is used by the children after the core is dead (though again as I've mentioned you don't need to use this method for -all- non-unique data if you have a particular reason not to) so something like children with lifetime based effects (such as you created saplings through your spell that grow based on some condition, the spell core controls their growth rate and whatever effect they have on the nearby area) when fully grown or the spell times out they then spawn a summoned unit with stats based on how much they grew (all controlled via configurable variables potentially not even spell level), not on the spell core (the spell core is no longer needed, the children persist), they could now hold some other affect such as a light AOE Damage over time effect or some other such without the use of a spell core (and presence of the spell core would not save any data usage) and thus they would need to remain indexed themselves.

Keep in mind the concept and tutorial are written with large spells in mind so such a spell is certainly not out of the target scope, I hope the example provided clears it up a bit better than before

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Tank-Commander said:

 would be were no data that is non-unique is used by the children after the core is dead


Click to expand...

^that's indeed the very important point. With rest I think, I agree.

But so, to ensure we're on same page now.

When the parent node expires, we need to set their child's boolean like "parentDestroyed = true", right?

If this step would not exist, I don't think it can be properly ensured that the child node, or will function correctly, or the child node will identify a new indexed node as not destroyed parent node again, and again function not correctly.

What you think of calling them parent nodes, child nodes, parent data, and child data?

I somehow do not very easy when reading

Tank-Commander said:

 -all- non-unique data


Click to expand...

^I must halt myself a moment and think of which data it now actually is. Then I think, aha, it's the parent data that is shared and used by childs, too. Maybe it's me.

#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

If the children are to persist longer than the core then yeah something along the lines of "parentDestroyed = true" would be necessary in order for them to function correctly, if they are to be destroyed simultaneously then you can instead run a loop when the parent is killed (using the nextNode[parent] node as a starting point) finding all children were the parent is the spell core and recycling them immediately - in which case the boolean would not be needed (though you could set a flag for recycling on them next time their index is processed), alternatively using linked lists since children are always after the parent using this method in the linked list they can instead be identified for destruction through a boolean being set to true on the parent (as all children will be processed at least once before there's an opportunity for the parent index to be used up by something new, but this then necessitates not bothering to null all the data on the parent under the assumption that it's going to be reused) There's a number of ways to do it depending on how you structure your code

the difficulty with refering to parent data, parent node, child data child node is that the parent (core) will often have both unique and non-unique data - since it's wise to use it as more than just an abstract data store in the code and the unique data will not be utilised by the children (though they could utilise any of the data) there also comes conflation with OOP concepts which could confuse further since we're talking GUI and vanilla JASS.

It gets more confusing when you have multiple layers (the children create children which still see the core as their parent) hence using the term core for "where all the data is" rather than parent/child terminology.

In my example slowed units are children of the spell core even though they are created by being hit by bullets, which are also children of the spell core which starts making not a lot of sense if you asked me - moreover there's non-unique data on the spell core that is not used by the bullets but rather by the units that are slowed so which data used by what child is another thing to consider when picking terms

I think we'd need to get the opinion of others for what terms would be most appropriate so as to not confuse people unfamiliar with the concept

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

I think it's kind of simulating OOP, and so ok for me to talk about something like parent /child.

The form of usage is only dependant on if parent still exists, or not, but it doesn't really influence the terms I think.

The child node can use data like:

Parent Node Alive Parent Node Dead

Child Node (Alive)AllDataChildData

AllData = ParentData + ChildData

So yes, like maybe in your spell, nodes can exist, but for the user should be very clear, that what ever struture he uses, that under no circumstances the child still should use parent data in case the parent index might be reused already.

You absolutly can stay with terms like "Core" data, I find this term also very good, and it's matching. It was just my thoughts.

#### Tank-Commander

#### Tank-Commander

 Level 30

JoinedMay 26, 2009Messages1,821

Resources: 45

#### Resources
Packs 1Spells and Systems 41Tutorials 3

If you feel the clarification is necessary I'll add a section to the bottom addressing to take care when recycling things - it seemed fairly intuitive to me but obviously I would since I'm writing it XD


 Last edited: Jan 25, 2017

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Yo, I would prefer having this aspect included. With including it you might want to restructure the whole thing, I'm not sure, have honestly not read if fully again.

Okè, so just ping me again when you've found the time.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

It might be technicaly correct, but I believe it's written a bit too hard or complex, to easily understand everything.

Core data is data shared by multiple instances to prevent code-replication. It means that core data is non-unique, and all instances, refering to the core may use provided data.

The instances that use shared core data are not limited to it, and can of course also use instance specific data; but however once the shared core is being removed, it must be ensured that no further usage of the core data will be applied, but only instance specific data will remain for usage. It often even only makes sense to remove/recycle the core data exclusivly, if it's ensured that all instances that use it, are already destroyed, too.

It can be extended/wided a bit, too, so always parts are talked about.

Maybe even some short image might help to show shared / unique data, and that user is aware of the recycling aspect.

@others do only me find it could be simpler written?

#### Aniki

#### Aniki

 Level 13

JoinedNov 7, 2014Messages571

Resources: 21

#### Resources
Tools 1Maps 1Spells and Systems 14JASS 3Tutorials 2

Can't say I understood what T-C had in mine when I first read the tutorial.

It clicked for me at some point while trying to write the turret example.

I guess in my script the "Spell Core" would be the Turret which calculates some stuff

when the spell is casted and it is kept allocated until every reference to it has been destroyed.

I guess StageId(s) would be the state(s) in a state machine as well.

In hindsight the tutorial makes sense but I didn't get it, maybe more examples would've helped.

#### Attachments

turret.j

 16.9 KB

 · Views: 152



turret.object-data.txt

 4.8 KB

 · Views: 134



#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

@Tank-Commander -- Aniki is sceptical, too, that the Content is good, but should be explained in a simpler form. Maybe we should really try to bring a bit more ease into it.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Explained in posts above; I want to see it a bit more simpler, and I know Tank wants to be pushed to work. Sending gy for now, to push you to work. : )

 You must log in or register to reply here.


### Similar threads

 Building large spells using StageIDs


Tank-Commander

Apr 30, 2016

Trigger (GUI) Editor Tutorials

Replies11Views5K

Jun 7, 2016
Tank-Commander

Locked

 Help Indexing


HolyBear

Dec 28, 2013

World Editor Help Zone

Replies2Views2K

Dec 29, 2013
IcemanBo

[GUI]
 How to use Coordinates instead of Points(Locations) in GUI


Meatmuffin

Apr 17, 2016

Tutorial Archive

Replies21Views6K

Nov 21, 2016
IcemanBo

 MUI Spells Using Artificial Waits


Tank-Commander

Sep 28, 2012

Trigger (GUI) Editor Tutorials

Replies32Views20K

Oct 16, 2015
BETABABY

Locked

[Trigger]
 Mui


Wolf_Wing

Dec 26, 2012

Triggers & Scripts

Replies16Views2K

Jan 2, 2013
Ofel

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