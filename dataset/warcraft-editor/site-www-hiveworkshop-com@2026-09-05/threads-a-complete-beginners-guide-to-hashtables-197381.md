---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/a-complete-beginners-guide-to-hashtables.197381/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# A Complete Beginners Guide to Hashtables
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/a-complete-beginners-guide-to-hashtables.197381/>
#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

#
 A Complete Beginners Guide to Hashtables


Written because even after reading all the hashtable tutorials on this site, I still had questions.

##
 Table of Contents

Requirements to Fully Appreciate this Tutorial

Hashtables and You
What is a Hashtable?

Why use a Hashtable

What does that Trigger Above Mean?

Handles and Hashtables
What is a Handle

How can we use Handles Effectively?

Where can Handles Take You?

Set udg_Handle = GetHandleId(udg_Unit)

Useful Examples of the Hashtable in Action
Tank Attack!, a Spell with Movement

Missile Launch, a Spell with Knockback

Triggers

Miscellaneous Hashtable Information

Credits
Change Log

Future Changes to Come

## Requirements to Fully Appreciate this Tutorial

Basic knowledge of GUI scripting

The ability to follow directions

Full comprehension of the English language

A desire to learn

Patience

## Hashtables and You

### What is a Hashtable?

Hashtables are data structures. A variable array works by storing multiple pieces of data, imagine it as the X axis in a graph. Now imagine a variable array that is complicated enough to store data not just along the X axis, but also the Y. This allows us to have a much more varied variable (pardon the wordplay) Hashtables work by saving the value of the variable until you need it later. It then loads the value of the variable so it can be used with the exact same data as before.

### Why use a Hashtable?

Simply put, hashtables are easy ways to make your spells MUI. Spells that are not MUI will immediately be rejected from the Spells section of THW. Unless you only have a single instance of a unit on your map, not being MUI will cause multiple casts of the spell to malfunction. Hashtables have allowed me to advance worlds ahead of were I was when I started triggering. They are open ended and any spell that isn't instant will need hashtables or recycling.


At the same time, using hashtables has its drawbacks. It is sometimes quite annoying to have to load every value, and if you make a mistake in a spell it can be quite a long time before you realize you didn't save or load a value correctly. Hashtables are also slower than arrays.


Now let's look at how to make a hashtable.​

Events
Map initialization

Conditions

Actions
Hashtable - Create a hashtable

Set EX_Hash = (Last created hashtable)

### What does that Trigger Above Mean?

Broken down, the trigger means that at the start of the map, a hashtable will be created. The hashtable will be assigned the variable EX_Hash. Most hashtables are going to be created at the start of the map. You need to assign the hashtable a variable name. This is because you will probably have multiple hashtables. If you import a spell or a system, it will probably have a hashtable associated with it. If you assign your variables to (last created hashtable) instead of your hashtable variable, you may accidently load a value from a different hashtable!​

## Handles and Hashtables

### What is a Handle?

A handle is anything that is not a real, boolean, integer, or string. Handles can be saved with hashtables, allowing us to extract exact information portaining to that handle whenever we want to. Simply put, it's like saving a new variable for each handle, even if the variable is changed later, we can load the value of whatever the handle is and still have the original starting variable.


Here is an example of a hashtable without handles. While it is still useful, it can be even better with them​

Events
Map initialization

Conditions

Actions
Hashtable - Create a hashtable

Set EX_Hash = (Last created hashtable)

Events

Conditions

Actions
Set EX_Real = 20.00

Hashtable - Save EX_Real as 1 of 1 in EX_Hash

Set EX_Real = 4566.00

Set EX_Real = 12345.00

Set EX_Real = 100000000.00

Set EX_Real = (Load 1 of 1 from EX_Hash)

In this example, when the even happens, the real number 20 will be saved into value 1 of 1 in the hashtable EX_Hash. Afterwards, you can change the variable however you like, and it will not affect the value 1 of 1 in the hashtable. You can load the value after changing EX_Real, and the loaded value will be what you first saved it as, in this example the number is 20.​

### How can we use Handles Effectively?

Handles are used in conjunction with hashtables. A handle describes the value being saved. If you were saving EX_Real to the triggering unit of an ability, you change it to say (key (triggering unit)). Key simply is a way of identifying handles. Let's look below at a simple damage over time spell that uses handles to correctly damage every unit targetted. Please note that leaks will be addressed fully in this example.​

Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Damage Over Time

Actions
Set EX_Target = (Target unit of ability being cast)

Set EX_Damage = 20.00

Set EX_Timer = 5.00

Unit Group - Add (Target unit of ability being cast) to EX_Group

Hashtable - Save EX_Timer as 1 of (Key (Target unit of ability being cast)) in EX_Hash

Events
Time - Every 1.00 seconds of game time

Conditions

Actions
Unit Group - Pick every unit in EX_Group and do (Actions)
Loop - Actions
Set EX_Timer = (Load 1 of (Key (Picked unit)) from EX_Hash)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
EX_Timer Greater than 0.00

Then - Actions
Unit - Set life of (Picked unit) to ((Life of (Picked unit)) - EX_Damage)

Hashtable - Save (EX_Timer - 1.00) as 1 of (Key (Picked unit)) in EX_Hash

Else - Actions
Unit Group - Remove (Picked unit) from EX_Group

Hashtable - Clear all child hashtables of child (Key (Picked unit)) in EX_Hash

This is a lot to look at. Spend a minute or two trying to roughly get the idea of what is happening in this spell. The targetted unit of the ability "Damage Over Time" is being added to the group "EX_Group". Every second, members of the group are checked to see how much time they have left in the spell. If they are not done with the duration of the spell, they suffer twenty damage. If they are done with the duration of the spell, they are removed from the damage group, and the child hashtables are cleared. This means that all that extra data solely belonging to the picked unit is destroyed. It is the hashtable equivelent of destroying memory leaks. Always make sure to include it at the end of spells. The absolute most important thing to pay attention to right now is how the hashtable values are saved. They correspond to how you identify the unit. EX_Timer is saved in the first part as (key (target unit of ability being cast)). At the second trigger it is being referenced to with (key (picked unit)). This is because the unit is being selected as picked unit instead of target unit of ability being cast. This difference in handles is crucial to understanding the beast known as the hashtable. If you are going to use a trigger like this, please make it more efficient by assigning unit references (picked unit, for example) to variables and then calling them.

Quick guide for recreating this trigger

EX_Timer is a real

EX_Group is a unit group

EX_Damage is a real
​

### Where can Handles Take You?

Handles are quite possibly the most important parts of hashtables. It allows it to be MUI, and it gives hashtables their dynamic quality. You must learn to effectively use handles if you want your spells to be MUI.​

### Set udg_VARIABLE = GetHandleId(udg_UNIT)

A great little line, this baby is highly important to hashtables. Used with custom script, this will automatically pick the correct key corresponding to the unit. In the custom script, VARIABLE will be the name of an Integer variable, while UNIT will be the name of the unit variable. Look below for an example of how to use this.

Set Unit = (Picked unit)

Custom script: set udg_Handle = GetHandleId(udg_Unit)

-------- Handle is my integer variable, Unit is my unit variable --------

Hashtable - Save Timer as 1 of Handle in Hash

## Useful Examples of the Hashtable in Action


Please note that triggers are in the third section. Both spells are by Jazztastic, please give credit if used. Main purpose of these is to show examples of how hashtables and handles can be used effectively in spells

### Tank Attack!, a Spell With Movement

Most spells use dummy units to deal damage and create visual effects for the spell. "Movement" is a term I use when talking about physically moving the dummy unit to help create effects for a spell.


Provided for you is the trigger to "Tank Attack!", a spell that creates a horde of miniature tanks to run over your enemies. Tanks are created every 0.04 seconds. 25 tanks are created over 1 second.​

### Missile Launch, a Spell With Knockback

Knockback is when a spell causes a unit to slide backwards from its spot. It is effectively stunned because it can take no actions. Knockback is a desireable effect to have in a map because it looks nice, and is more interesting than just a stun.


Provided for you is the trigger to "Missile Launch", a spell that shoots a missile at target location. Upon coming within range of a unit, the missile explodes, damaging all units in X range and knocking them back a random distance between X and X. It is similar to Tank Attack! in that it uses movement, but I consider this a step up from Tank Attack!. I recommend using imported explosions for the explosion effects.

​

### The Triggers and Explainations for Tank Attack! and Missile Launch

​

Variables
Events
Map initialization

Conditions

Actions
-------- ------------------- --------

-------- Tank Attack --------

-------- ------------------- --------

Hashtable - Create a hashtable

Set TA_Hash = (Last created hashtable)

Set TA_Aoe = 100.00

Set TA_Damage = 1.00

-------- ------------------- --------

-------- Missile Launch --------

-------- ------------------- --------

Hashtable - Create a hashtable

Set ML_Hash = (Last created hashtable)

Set ML_Aoe = 120.00

I personally like to keep my hashtables seperate for my spells. It is not necessary, but I recommend it to help keep track of your stuff. Also, a few variables that only need to be loaded once are present in this map init trigger​

Tank Attack Cast
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Copter Attack!

Actions
Set TA_Caster = (Triggering unit)

Sound - Play SteamTankBeep <gen>

Set TA_Level = (Level of (Ability being cast) for TA_Caster)

Set TA_Target = (Target point of ability being cast)

Set TA_Time = 1.00

Unit Group - Add TA_Caster to TA_Group1

Hashtable - Save TA_Level as 1 of (Key (Triggering unit)) in TA_Hash

Hashtable - Save TA_Time as 2 of (Key (Triggering unit)) in TA_Hash

Custom script: call RemoveLocation(udg_TA_Target)

Trigger - Turn on Tank Attack Loop <gen>

Tank Attack Loop
Events
Time - Every 0.04 seconds of game time

Conditions

Actions
Unit Group - Pick every unit in TA_Group1 and do (Actions)
Loop - Actions
Set TA_Level = (Load 1 of (Key (Picked unit)) from TA_Hash)

Set TA_Time = (Load 2 of (Key (Picked unit)) from TA_Hash)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
TA_Time Greater than 0.00

Then - Actions
Set TA_Caster = (Picked unit)

Set TA_Point1 = (Position of TA_Caster)

Unit - Create 1 Mini Tank for (Owner of TA_Caster) at TA_Point1 facing ((Facing of TA_Caster) + (Random real number between -25.00 and 25.00)) degrees

Set TA_Tank = (Last created unit)

Unit - Add a 2.00 second Generic expiration timer to TA_Tank

Unit - Make TA_Tank Explode on death

Unit Group - Add TA_Tank to TA_Group2

Hashtable - Save TA_Level as 1 of (Key (Last created unit)) in TA_Hash

Hashtable - Save (TA_Time - 0.04) as 2 of (Key (Picked unit)) in TA_Hash

Custom script: call RemoveLocation(udg_TA_Point1)

Set TA_Loops = (TA_Loops + 1)

Else - Actions
Hashtable - Clear all child hashtables of child (Key (Picked unit)) in TA_Hash

Unit Group - Remove (Picked unit) from TA_Group1

Unit Group - Pick every unit in TA_Group2 and do (Actions)
Loop - Actions
Sound - Play SteamTankMovement <gen>

Set TA_Level = (Load 1 of (Key (Picked unit)) from TA_Hash)

Set TA_Tank = (Picked unit)

Set TA_Point1 = (Position of TA_Tank)

Set TA_Point2 = (TA_Point1 offset by 25.00 towards (Facing of TA_Tank) degrees)

Unit - Move TA_Tank instantly to TA_Point2

Custom script: set bj_wantDestroyGroup = true

Unit Group - Pick every unit in (Units within TA_Aoe of TA_Point2 matching (((Matching unit) belongs to an enemy of (Owner of TA_Tank)) Equal to True)) and do (Actions)
Loop - Actions
Special Effect - Create a special effect attached to the chest of (Triggering unit) using Objects\Spawnmodels\Human\HumanBlood\BloodElfSpellThiefBlood.mdl

Special Effect - Destroy (Last created special effect)

Unit - Cause TA_Tank to damage (Picked unit), dealing (TA_Damage x (Real(TA_Level))) damage of attack type Spells and damage type Normal

Tank Attack End
Events
Unit - A unit Dies

Conditions
(Unit-type of (Triggering unit)) Equal to Mini Tank

Actions
Unit Group - Remove (Triggering unit) from TA_Group2

Hashtable - Clear all child hashtables of child (Key (Triggering unit)) in TA_Hash

Set TA_Loops = (TA_Loops - 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
TA_Loops Less than or equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

In the first trigger, notice the sound being played. You will have to use the sound editor to get this sound from the game. In the second trigger it is important to notice that both the movement for the units and their creation run off the same timer.

Quick guide for recreating this trigger

TA_Angle is a real

TA_Aoe is a real

TA_Caster is a unit

TA_Tank is a unit

TA_Damage is a real

TA_Group1 is a unit group

TA_Group2 is a unit group

TA_Hash is a hashtable

TA_Level is an integer

TA_Point1 is a point

TA_Point2 is a point

TA_Point3 is a point

TA_Target is a point

TA_Timer is a real

TA_Loops is an integer

​


Following are the triggers for Missile Launch

Missile Launch Cast
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Missile Launch

Actions
Set ML_Caster = (Triggering unit)

Set ML_Point1 = (Position of ML_Caster)

Unit - Create 1 Missile for (Triggering player) at ML_Point1 facing (Facing of ML_Caster) degrees

Unit Group - Add (Last created unit) to ML_Group

Set ML_Timer = 2.00

Hashtable - Save ML_Timer as 1 of (Key (Last created unit)) in ML_Hash

Trigger - Turn on Missile Launch Loop <gen>

Custom script: call RemoveLocation(udg_ML_Point1)

Missile Launch Loop
Events
Time - Every 0.04 seconds of game time

Conditions

Actions
Unit Group - Pick every unit in ML_Group and do (Actions)
Loop - Actions
Set ML_Timer = (Load 1 of (Key (Picked unit)) from ML_Hash)

Set ML_Missile = (Picked unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ML_Timer Greater than 0.00

Then - Actions
Hashtable - Save (ML_Timer - 0.04) as 1 of (Key (Picked unit)) in ML_Hash

Set ML_Point1 = (Position of ML_Missile)

Set ML_Point2 = (ML_Point1 offset by 30.00 towards (Facing of ML_Missile) degrees)

Unit - Move ML_Missile instantly to ML_Point2

Custom script: set bj_wantDestroyGroup = true

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Number of units in (Units within ML_Aoe of ML_Point2 matching (((Matching unit) belongs to an enemy of (Owner of ML_Missile)) Equal to True))) Equal to 0

Then - Actions
Custom script: set bj_wantDestroyGroup = true

Unit Group - Pick every unit in (Units within 300.00 of ML_Point2 matching (((Matching unit) belongs to an enemy of (Owner of ML_Missile)) Equal to True)) and do (Actions)
Loop - Actions
Unit - Cause ML_Missile to damage (Picked unit), dealing 250.00 damage of attack type Siege and damage type Demolition

Unit Group - Add (Picked unit) to ML_Group2

Set ML_Point3 = (Position of (Picked unit))

Set ML_Angle = (Angle from ML_Point2 to ML_Point3)

Set ML_Timer = 1.40

Set ML_Distance = ((Random real number between 50.00 and 150.00) x 0.04)

Hashtable - Save ML_Timer as 1 of (Key (Picked unit)) in ML_Hash

Hashtable - Save ML_Angle as 2 of (Key (Picked unit)) in ML_Hash

Hashtable - Save ML_Distance as 3 of (Key (Picked unit)) in ML_Hash

Set ML_Loops = (ML_Loops + 1)

Custom script: call RemoveLocation(udg_ML_Point3)

Unit - Explode ML_Missile

Special Effect - Create a special effect at ML_Point2 using war3mapImported\ExplosionBIG.mdx

Special Effect - Destroy (Last created special effect)

Set ML_Timer = 0.00

Hashtable - Save ML_Timer as 1 of (Key (Picked unit)) in ML_Hash

Else - Actions

Custom script: call RemoveLocation(udg_ML_Point1)

Custom script: call RemoveLocation(udg_ML_Point2)

Else - Actions
Hashtable - Clear all child hashtables of child (Key (Picked unit)) in TA_Hash

Unit - Explode ML_Missile

Unit Group - Remove ML_Missile from ML_Group

Unit Group - Pick every unit in ML_Group2 and do (Actions)
Loop - Actions
Set ML_KnockBackUnit = (Picked unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(ML_KnockBackUnit is alive) Equal to True

Then - Actions
Set ML_Timer = (Load 1 of (Key (Picked unit)) from ML_Hash)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ML_Timer Greater than 0.00

Then - Actions
Hashtable - Save (ML_Timer - 0.04) as 1 of (Key (Picked unit)) in ML_Hash

Set ML_Angle = (Load 2 of (Key (Picked unit)) from ML_Hash)

Set ML_Distance = (Load 3 of (Key (Picked unit)) from ML_Hash)

Set ML_Point1 = (Position of ML_KnockBackUnit)

Set ML_Point2 = (ML_Point1 offset by ML_Distance towards ML_Angle degrees)

Unit - Move ML_KnockBackUnit instantly to ML_Point2

Special Effect - Create a special effect attached to the origin of ML_KnockBackUnit using Abilities\Spells\Human\FlakCannons\FlakTarget.mdl

Special Effect - Destroy (Last created special effect)

Unit - Order ML_KnockBackUnit to Stop

Else - Actions
Hashtable - Clear all child hashtables of child (Key (Picked unit)) in ML_Hash

Unit Group - Remove (Picked unit) from ML_Group2

Set ML_Loops = (ML_Loops - 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ML_Loops Less than or equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

Else - Actions
Hashtable - Clear all child hashtables of child (Key (Picked unit)) in ML_Hash

Unit Group - Remove ML_KnockBackUnit from ML_Group2

Set ML_Loops = (ML_Loops - 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ML_Loops Less than or equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

Quick guide for recreating this trigger

ML_Angle is a real

ML_Aoe is a real

ML_Caster is a unit

ML_Distance is a real

ML_Group1 is a unit group

ML_Group2 is a unit group

ML_Hash is a hashtable

ML_KnockBackUnit is a unit

ML_Missile is a unit

ML_Point1 is a point

ML_Point2 is a point

ML_Point3 is a point

ML_Timer is a real

ML_Loops is an integer

​

## Miscellaneous Hashtable Information

You are only allowed to have 256 hashtables

Having the creation of the hashtable in the trigger will not only make it innefficient, it will also make it NOT MUI

Multiple spells can use the same hashtable. You should have no more than 10 hashtables in your map

Hashtables are also useful for systems, not just spells

## Credits

I would like to thank Wyrmlord for his awesome tutorial on hashtables. It helped get me going, but I didn't think it was clear enough, so heres mine  I would also like to thank Dr. SuperGood and Maker for some information. Finally, I would like to thank Archangel for some trigger corrections. Please comment and rate, I like feedback.​

### Change Log

 v1.0  Uploaded tutorial to The Hive.

 v1.1  Fixed incorrect information, fixed example triggers, added Change Log and Future Changes to Come sections.

 v1.2  Put [goto] and

​


 Last edited: Aug 13, 2011

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

Comment and rate please


 Last edited: Jun 15, 2011

#### Archangel678

#### Archangel678

 Level 10

JoinedApr 25, 2009Messages293

Resources: 6

#### Resources
Maps 1Spells 4Tutorials 1

I'd suggest adding a key, or a link to separate parts. (Click here, scrolls to basic info, click here 2, scrolls to Tank Attack!

Also, a map showing hashtables in action would help.

Also, this little line can help alot with hashtables, as it makes it much more efficient:

Custom script: set udg_YOURINTEGER = GetHandleId(udg_UNIT)
You probably should include linebreaks and documentation for your larger spells.

I recommend using AnarchainBedlams miniature tank model, because multiple units casting this spell with the regular steam tank's model will cause massive lag!

I'd suggest removing that line altogether, it can sound like advertising and has no major correlation with hashtables.

=======Triggering Errors/Suggestions=======

And - All (Conditions) are true
No point in that, its already set that way....

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
((Picked unit) is alive) Equal to True
Set ML_KnockBackUnit = (Picked unit)

You could set ML_KnockBackUnit = (Picked Unit) before the if/then/else.

Custom script: set bj_wantDestroyGroup = true
If (All Conditions are True) then do (Then Actions) else do (Else Actions)

What's the point of that? Remove the bj_wantDestroyGroup = true

If - ConditionAnd - All (Conditions) are true

Conditions
(ML_Group is empty) Equal to True

(ML_Group2 is empty) Equal to True

You should use an integer and increase it when the spell is cast and decrease it when the spell is over so that you're not checking if the groups are empty every loop.

This looks very good, very useful too.


 Last edited: Jun 15, 2011

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

Archangel I am fixing all your suggestions except for the last. I do not understand the method you are suggesting. How would using an integer be different, wouldn't you still have to check every loop whether the integer was zero or not?


Maker, thank you for the information. My bad for begging for rep.


I'm updating it now.


EDIT: I have updated my tutorial, with everything mentioned by both Archangel and Maker except for that integer check thing. Most notable I added change log and future updates section.

#### Archangel678

#### Archangel678

 Level 10

JoinedApr 25, 2009Messages293

Resources: 6

#### Resources
Maps 1Spells 4Tutorials 1

Integer thingy

Spell Cast:

Increase integer by 1.

Turn on loop

Spell Ends:

Decrease integer by 1.

If integer = 0

Turn off loop.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

Do like this during the casting time...

Set EX_Target = (Target unit of ability being cast)

Custom script: set udg_ID = GetHandleId(udg_EXTarget)

Set EX_Damage = 20.00

Set EX_Timer = 5.00

Unit Group - Add (Target unit of ability being cast) to EX_Group

Hashtable - Save EX_Timer as 1 of udg_ID in EX_Hash
While during the loop time...

Custom script: set udg_ID = GetHandleId(GetEnumUnit())

Set EX_Timer = (Load 1 of udg_ID from EX_Hash)
Where ID is an integer variable


But Maker said it's GUI so stick to GUI, but the above is more efficient and faster...

#### Bannar

#### Bannar

 Level 26

JoinedMar 19, 2008Messages3,141

Resources: 23

#### Resources
Spells and Systems 5JASS 15Tutorials 1Wurst 2

I won't reapet things already mentioned. However you could use hidden tags for triggers, making your tutorials cleaner and easier to step through.

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

Spinnaker said:

 I won't reaper things already mentioned. However you could you hidden tags for triggers, making your tutorials cleaner and easier to step through.


Click to expand...


A very good and easy idea. Thank you

 Do like this during the casting time...

Set EX_Target = (Target unit of ability being cast)
Custom script: set udg_ID = GetHandleId(udg_EXTarget)

Set EX_Damage = 20.00

Set EX_Timer = 5.00

Unit Group - Add (Target unit of ability being cast) to EX_Group

Hashtable - Save EX_Timer as 1 of udg_ID in EX_Hash


While during the loop time...

Custom script: set udg_ID = GetHandleId(GetEnumUnit())
Set EX_Timer = (Load 1 of udg_ID from EX_Hash)


Where ID is an integer variable


But Maker said it's GUI so stick to GUI, but the above is more efficient and faster...


Click to expand...


While efficiency is very very important in coding, this tutorial was aimed more at noobs. But you are very correct, making the handles via custom script it more efficient. I'll certainly change it in my version of the spell.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

It looks really good to me. Nice job on the tutorial.


~Approved.

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

PurgeandFire111 said:

 It looks really good to me. Nice job on the tutorial.


~Approved.


Click to expand...

HELL YES!

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

In your tank attack cast trigger you save target point of ability into a variable and then into a hashtable. Then you immediately remove it. This means the point no longer exists. After that you try to use it in the looping trigger. You should remove it when you don't need it anymore, most likely right before you flush the child hashtable.


Your number of units in xxx condition leaks.

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

These triggers are old, I need to do some updating. The target point isn't even used in the loop, I need to repost the new triggers. Btw Maker, is there a function similar to set bj_wantDestroyGroup = true that can be used for conditions? Or do I have to assign the group to a variable and then clear? Efficiency matters

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

Set the boolean to true before the condition

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

Maker said:

 Set the boolean to true before the condition

Click to expand...


I didn't quite get this part, but here's what I came up with:

Set ML_GroupCheck = (Units within ML_Aoe of ML_Point2 matching (((Matching unit) belongs to an enemy of (Owner of ML_Missile)) Equal to True))

Set ML_GroupCheckInteger = (Number of units in ML_GroupCheck)

If - Conditions
ML_GroupCheckInteger Greater than or equal to 1

Then - Actions
Do Stuff

Custom script: call DestroyForce(udg_ML_GroupCheck)
I'm pretty sure it works, cause its just replacing the if with a variable which replaced another variable which was cleared. So there shouldn't be any leaks. I don't think it's very efficient tho

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

Custom script: set bj_wantDestroyGroup = true

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Number of units in (Units in (Playable map area))) Equal to 0

Then - Actions

Else - Actions

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

Facepalm


. . . . . .. . . . . . . . . . . ,.-‘”. . . . . . . . . .``~.,


. . . . . . . .. . . . . .,.-”. . . . . . . . . . . . . . . . . .“-.,


. . . . .. . . . . . ..,/. . . . . . . . . . . . . . . . . . . . . . . ”:,


. . . . . . . .. .,?. . . . . . . . . . . . . . . . . . . . . . . . . . .\,


. . . . . . . . . /. . . . . . . . . . . . . . . . . . . . . . . . . . . . ,}


. . . . . . . . ./. . . . . . . . . . . . . . . . . . . . . . . . . . ,:`^`.}


. . . . . . . ./. . . . . . . . . . . . . . . . . . . . . . . . . ,:”. . . ./


. . . . . . .?. . . __. . . . . . . . . . . . . . . . . . . . :`. . . ./


. . . . . . . /__.(. . .“~-,_. . . . . . . . . . . . . . ,:`. . . .. ./


. . . . . . /(_. . ”~,_. . . ..“~,_. . . . . . . . . .,:`. . . . _/


. . . .. .{.._$;_. . .”=,_. . . .“-,_. . . ,.-~-,}, .~”; /. .. .}


. . .. . .((. . .*~_. . . .”=-._. . .“;,,./`. . /” . . . ./. .. ../


. . . .. . .\`~,. . ..“~.,. . . . . . . . . ..`. . .}. . . . . . ../


. . . . . .(. ..`=-,,. . . .`. . . . . . . . . . . ..(. . . ;_,,-”


. . . . . ../.`~,. . ..`-.. . . . . . . . . . . . . . ..\. . /\


. . . . . . \`~.*-,. . . . . . . . . . . . . . . . . ..|,./.....\,__


,,_. . . . . }.>-._\. . . . . . . . . . . . . . . . . .|. . . . . . ..`=~-,


. .. `=~-,_\_. . . `\,. . . . . . . . . . . . . . . . .\


. . . . . . . . . .`=~-,,.\,. . . . . . . . . . . . . . . .\


. . . . . . . . . . . . . . . . `:,, . . . . . . . . . . . . . `\. . . . . . ..__


. . . . . . . . . . . . . . . . . . .`=-,. . . . . . . . . .,%`>--==``


. . . . . . . . . . . . . . . . . . . . _\. . . . . ._,-%. . . ..`


Did not know that set bj_wantDestroyGroup = true works before conditions.

#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

Ok... I liked the tutorial and it was helpful, but there is one thing I belive that you compleetly left out


I was lead to belive that when you Have a Campaign, u use Hashtables to save the information about your units


and if I was to try to make a Campaign using hashtables after reading your Tutorial, I would still be shooting in the dark


Please make a section about Using Hashtables in Campaigns


Because some people might be reading this tutorial in order to make there campaign work properly.


I would also want to know where exactly the Handle information is saved, because i still don't understand how hashtables don't collapse on them selfs when the same trigger is run twice at the same time


Example:

Example Trigger

Events:

Unit - starts the effect of an abillity

Conditions:

Ability being cast equal to [Gauntlet of Ilusions]

Actions:

Set [Example Unit Variable] to [Triggering unit]

Hashtable: Save handle of [Example Unit Variable] as 1 of 1 in [Example Hashtable]

Wait: wait 8 Seconds

Set [Example Unit Variable] = to [Load 1 0f 1 in Example Hashtable]
When a unit casts the ability this trigger runs, and then if another unit casts the same ability in the 8 second long gap, then the original 1 of 1 handle in Example Hashtable will be changed to the triggering unit of the second cast


so 8 seconds later, both triggers will set example unit variable to the triggering unit of the second cast, instead of the triggering units of there own respective casts


How do hashtables evoid that problem?


Tank you for helping me understand hashtables


 Last edited: Jul 3, 2011

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

TDA, I don't believe that hashtables are used in campaigns for what you want. To save/load your hero and items between campaigns I believe that requires game caches. I'd suggest looking up a tutorial on those because I am not knowledgeable in game caches.


As far as your question regarding hashtable overwriting, hashtables avoid this by saving the value to another thing. If you click on the little tab when saving hashtables that says (get handle ID), it will say key ________. The ________ part contains tons and tons of things you can save the value to. When you load a hashtable value, you are loading the data saved to the handle. In your example trigger the value would be overwritten, because you don't use handles and you have a wait (you shouldn't use waits in spells unless you use local variables). Using two integers in hashtables is fine, but it doesn't utilize the power of handles.


Handles make hashtables MUI and powerful. If you look at my example spells, you will see that values are saved as an integer (any whole number, WE doesn't use negatives tho), and then key (picked unit), or (triggering unit). I am saving the value TO the unit. When I need the value again, I have to load by identifying the unit that has the correct value. I think I properly explained this in the tutorial, but if it wasn't clear I shall try to elaborate.

#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

I think your might be right about the Game Cache, because after all, I had no knolage in eather of the two b4 reading this tutorial and it would have been easy for me to get the two confused.


Anyways what are Local Variables? (The name would sugest that they are variables saved to the trigger they run in and are not to be sheared with other triggers) However I have never heard of them. Just tought that such a thing would be very helpful in making Mui spells


Can you point me to a tutorial in local Variables pls? cause if I'm right, they are just the thing I need


I am curently creating the Biggest and Baddest of all spells I have ever created, and I find that the ordinary [Triggering Unit] [Target of ability being cast] etc. tags are not enough, because the spell cast stops while the trigger keeps on running, and all of the tags get destroyed.


I tought I could use Hashtables to save them before they are lost, and then bring them back when I need them. what do you sugest I do?

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

I do suggest using hashtables for people who use GUI. While local variables are nice because they don't require a special method (hashtables or recycling) to make them MUI, they are hard to use because every function you want to use a variable in must be in custom script.


I'm sure that if you post your triggers in the help section someone can help you make your awesome spell MUI. If it's a specific hashtable question, I can help you there.

#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

So I can't use Hashtables to overwrite the problem of data being lost?


Such as Creating 2 units at a 0.25 interval, and then ordering the second unit to attack the first unit but having the problem of only getting the option [Last created unit] but not having any way to order it to attack the other unit.


Can I prevent this problem by Seting a Variable to last created unit at the moment when the first unit is created, then seting a hashtable to the value of the specific variable, and then 0.25 seconds later, setting the variable to the Hashtable?


and if I can... How exactly do I do it?


I belive this Question is 100% regarding Hashtables

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

Untitled Trigger 002
Events
Time - Every 0.25 seconds of game time

Conditions

Actions
Unit - Create 1 Footman for Player 1 (Red) at (Center of (Playable map area)) facing Default building facing degrees

Set UNIT1 = (Last created unit)

Unit - Create 1 Footman for Player 1 (Red) at (Center of (Playable map area)) facing Default building facing degrees

Set UNIT2 = (Last created unit)

Unit - Order UNIT2 to Attack UNIT1

You can do that without hashtables, from waht you described

#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

Beef_Is_Back said:

Untitled Trigger 002
Events
Time - Every 0.25 seconds of game time

Conditions

Actions
Unit - Create 1 Footman for Player 1 (Red) at (Center of (Playable map area)) facing Default building facing degrees

Set UNIT1 = (Last created unit)

Unit - Create 1 Footman for Player 1 (Red) at (Center of (Playable map area)) facing Default building facing degrees

Set UNIT2 = (Last created unit)

Unit - Order UNIT2 to Attack UNIT1

You can do that without hashtables, from waht you described


Click to expand...


I think you missunderstood

Untitled Trigger 002
Events
Time - Every 0.25 seconds of game time

Conditions

Actions
Unit - Create 1 Footman for Player 1 (Red) at (Center of (Playable map area)) facing Default building facing degrees

Set UNIT1 = (Last created unit)

Wait: 0.25 seconds

Unit - Create 1 Footman for Player 1 (Red) at (Center of (Playable map area)) facing Default building facing degrees

Set UNIT2 = (Last created unit)

Unit - Order UNIT2 to Attack UNIT1

It's supose to be mui, so adding that wait action wich you didn't makes it a potential desaster

#### indomitable1319

#### indomitable1319

 Level 19

JoinedJan 22, 2010Messages2,583

Resources: 5

#### Resources
Maps 3Spells and Systems 1Tutorials 1

I don't think that it will matter if you change the wait to 0.24 seconds, the players won't notice.


It will be MUI since it only happens in one trigger.


And that leaks.


Set the point into a variable, and in this case make the:

Set Temp_Point = (Center of (Playable map area))
In the Initialization trigger since the position doesn't change anyway.


A more efficient trigger would be:

Untitled Trigger 002
Events
Time - Every 0.25 seconds of game time

Conditions

Actions
Unit - Create 1 Footman for Player 1 (Red) at Temp_Point facing Default building facing degrees

Set UNIT1 = (Last created unit)

Wait 0.24 seconds

Unit - Create 1 Footman for Player 1 (Red) at Temp_Point facing Default building facing degrees

Set UNIT2 = (Last created unit)

Custom script: call RemoveLocation(udg_Temp_Point)

Unit - Order UNIT2 to Attack UNIT1

And won't creating footmen every .25 seconds lag?

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

If you really want to make this MUI without leaks and without using the variable editor...

Untitled Trigger 002

Events
Time - Every 0.25 seconds of game time

Conditions

Actions
Custom script: local unit u1 = CreateUnit(Player(0), 'hfoo', GetRectCenterX(bj_mapInitialPlayableArea), GetRectCenterY(bj_mapInitialPlayableArea), 0)

Custom script: local unit u2

Wait: 0.25 seconds

Custom script: set u2 = CreateUnit(Player(0), 'hfoo', GetRectCenterX(bj_mapInitialPlayableArea), GetRectCenterY(bj_mapInitialPlayableArea), 0)

Custom script: call IssueTargetOrder(u2, "attack", u1)

Custom script: set u1 = null

Custom script: set u2 = null

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

A 0.25 second wait is actually 0.35-0.45 seconds in single player.

#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

Yes I admit that the example was all wrong... but as you can see, it is just a Quote and then a quick edit


This is what I actualy want to fix


(Striped My map down to it's bones, to show you the spell)

View attachment Striped Down Vesion of My Map.w3x


Tank you in advance for your help

#### tagg1080

#### tagg1080

 Level 10

JoinedNov 28, 2008Messages655

Resources: 1

#### Resources
Tools 1

I feel like your understanding of what a hashtable is is somewhat lacking.

You are explaining them purely in the way that you have used it a little bit in some of your programing.

I would rather see an actual explaination of what a hashtable is, and then why they are much easier to use in this application. I don't like seeing a 'this is how I use them because they can do this', as that is honestly only helpful to noobs that want to barely learn kind of what something almost is in a roundabout way.

What is the difference between an array and a hashtable? Why would you use them over arrays? Why are they slower than arrays? Can arrays be faster/are there times where arrays are better?

Things like this come to mind.

Just my nit-picky comment.

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

"A Complete Beginners Guide to Hashtables", a name suggesting that this guide is for "noobs".


I've tried to write a better tutorial than the one by wyrmlord (His tutorial is great but I didn't understand a few parts, so I tried to streamline the information to the exact core of how to use and implement a hashtable). If you feel mine is lacking, write your own or shut the hell up, I'm trying to help. Of course someone who knows how to work a hashtable is going to feel like they're being lead by the nose through my tutorial, anyone who knows the content matter to any kind of tutorial will have a similar feeling. And based upon your comments I get the feeling you didn't even read the tutorial

 What is the difference between an array and a hashtable? Why would you use them over arrays? Why are they slower than arrays? Can arrays be faster/are there times where arrays are better?


Click to expand...


"Hashtables are data structures. A variable array works by storing multiple pieces of data, imagine it as the X axis in a graph. Now imagine a variable array that is complicated enough to store data not just along the X axis, but also the Y. This allows us to have a much more varied variable (pardon the wordplay) Hashtables work by saving the value of the variable until you need it later. It then loads the value of the variable so it can be used with the exact same data as before."


"At the same time, using hashtables has its drawbacks. It is sometimes quite annoying to have to load every value, and if you make a mistake in a spell it can be quite a long time before you realize you didn't save or load a value correctly. Hashtables are also slower than arrays."


A peice of information found at the very start of my tutorial. I don't really feel that arrays and hashtables are on a comparable level. They are different tools for performing different tasks, I often use arrays and hashtables together. As to your question why hashtables are slower, larger data structures take longer to load. Try drawing a graph along only along the X axis, it is a straight line. Now draw a graph utilizing both the X and Y axis. The graph will almost definitely be more complicated and take longer to draw. I didn't find that information necessary for a COMPLETE BEGINNGER to know.


But some dickhead feels the need to question the logic of a tutorial that took multiple hours to write in a 30 second post. Example spells are INCLUDED. They show the hashtable IN ACTION, so complete beginners can see and understand how to WEAVE THEM INTO TRIGGERING. I'm not telling them how to make spells, I'm showing them how I use hashtables in my spells.


Sorry to sound like an asshole, but I don't think any of the points you brought up are valid. If you're not a complete beginner at hashtables, you might not possibly be the target audience. Lets see some actual explaination as to why your comment doesn't deserve to be reported for spam when everything you wrote in it is complete bullshit and I covered in the tutorial or deemed it unnecessary to include due to the target audience being complete beginners. I feel like your understanding of the English language and how to read are somewhat lacking, but I didn't feel the need to point it out until you made a fool of yourself with a useless post. Good job

#### tagg1080

#### tagg1080

 Level 10

JoinedNov 28, 2008Messages655

Resources: 1

#### Resources
Tools 1

Hashtables are data structures. A variable array works by storing multiple pieces of data, imagine it as the X axis in a graph. Now imagine a variable array that is complicated enough to store data not just along the X axis, but also the Y. This allows us to have a much more varied variable (pardon the wordplay) Hashtables work by saving the value of the variable until you need it later. It then loads the value of the variable so it can be used with the exact same data as before."


That is not a very accurate definition,


It is a table of 'key's and 'values' that can be referenced by key.


In an array, you must reference the data by the number in which it was put into the array, a hashtable things are referenced by the 'keys'. Order doesn't matter. The actual values don't really matter either.


Since everything in Wc3 already has a unique 'key' assigned to it, it is much more helpful than an array because that key can be used in any hashtable for any value, arrays would make it extremely messy.


Saying "it can hold X and Y!" doesn't really make much sense.

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

Maybe not to a fucking retard. Array usage for referencing units most commonly is a unit indexer. Take all the possible units and put them onto the "X" axis by a number. You reference the units by calling their custom value. Only one set of data is being used. Hashtables assign peices of information TO another peice of information, similar to how a graph assigns a Y value to an X value (which is why on a calculator you see Y=, because Y is dependant on X). This value can be retrieved by referencing the other peice of data. Since Y is dependant on the X axis (and everyone who isn't mentally challenged that has completed 1 year of algebra knows this), similar to how the stored information is dependant on the object it is saved to, this is a perfect comparison. Calling it a table of keys and values is simply stating what it is, not how it performs and the concept behind it. In the literal sense of keys and values, the keys are the X axis and the values are the Y. You could hypothetically make an incredibly long array and use it as a hashtable, but it is easier to store data along two axis. I hope this makes sense because if it doesn't you need to find some common sense and stop being a dumbass. Now please shut the hell up with your stupid ass comments.

#### tagg1080

#### tagg1080

 Level 10

JoinedNov 28, 2008Messages655

Resources: 1

#### Resources
Tools 1

Beef_Is_Back said:

 Maybe not to a fucking retard. Array usage for referencing units most commonly is a unit indexer. Take all the possible units and put them onto the "X" axis by a number. You reference the units by calling their custom value. Only one set of data is being used. Hashtables assign peices of information TO another peice of information, similar to how a graph assigns a Y value to an X value (which is why on a calculator you see Y=, because Y is dependant on X). This value can be retrieved by referencing the other peice of data. Since Y is dependant on the X axis (and everyone who isn't mentally challenged that has completed 1 year of algebra knows this), similar to how the stored information is dependant on the object it is saved to, this is a perfect comparison. Calling it a table of keys and values is simply stating what it is, not how it performs and the concept behind it. In the literal sense of keys and values, the keys are the X axis and the values are the Y. You could hypothetically make an incredibly long array and use it as a hashtable, but it is easier to store data along two axis. I hope this makes sense because if it doesn't you need to find some common sense and stop being a dumbass. Now please shut the hell up with your stupid ass comments.


Click to expand...


Calm down, calm down.


The X and Y values don't really make sense to me, you are talking about them roughly how a domain and range of a function work? It is a fairly loose analogy in my head. If it makes sense to you though, power to you.


I was just pointing out that looking at a hashtable definition from a general sense, you didn't explain it very well. You only explained it in just one case that you can use them for only in wc3.


Sorry for critiquing, you may continue on.


Try to take criticism a little better in the future,

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

It's one of the simplest concepts in the world. . .


And moreover, why would I write information that isn't relevant to warcraft 3 on a warcraft 3 modding site? And any tutorial that extends beyond warcraft 3 would not be written in GUI, it would probably be written in C++ or even Java (ewwww). And this comment pissed me off.

 I would rather see an actual explaination of what a hashtable is, and then why they are much easier to use in this application. I don't like seeing a 'this is how I use them because they can do this', as that is honestly only helpful to noobs that want to barely learn kind of what something almost is in a roundabout way.


Click to expand...


Stupid. Giving examples is one of the best ways to learn. I used "this is how I use them because they can do this" as an EXAMPLE. Moreover, I went in depth to explain the actual logic behind a hashtable (last post), explaining it as a graph and you didn't comprehend. Actual explainations don't work with noobs, especially if they can't work with someone who thinks he's hot shit with hashtables can't comprehend it. Assuming you've completed the first year of highschool and didn't fail, you should understand basic graphing (via Algrebra 1).


Let me recap (again), so that you might fully comprehend the idea.


There are two main components to a graph, the X axis and the Y axis. A graph is basically a scale for measuring corresponding data. In the video game Warcraft 3, by Blizzard Entertainment, Hashtables and Variable Arrays among other things are used as methods to save data when crafting triggers. Variable arrays utilize only the X axis of a graph. You pick a name, and assign the value, saying that when X number appears, it has X value. This is not multi-unit instanceable in most cases, unless a proper recycling method is used or the effect is instant. Hashtables go a step further. They allow you to save data along both the X and Y axis' of the graph. In effect, this means that you can have some information dependant on other information, because the Y axis is always dependant on the X axis. Because of this, we can save any information to the X axis, in the case of most spells, a unit, and save further information portaining to this unit on the Y axis. It may seem like the graph is very limited, but think about it like this: Variables can hold any information, and the X and Y axis' store variables. That means that the value of Y does not have to equal the position of Y, for example, we could save the number 9001 as the 3rd Y value of X unit. Both Y and X are variables, which we can call within our hashtable to retrieve the values associated with these objects. Variables Arrays cannot do this, seeing as they only have 1 axis. While they can identify the data being presented, they cannot store additional data on top of the originially identified data.


 Last edited: Jul 9, 2011

#### TDA

#### TDA

 Level 4

JoinedOct 2, 2010Messages74

Resources: 1

#### Resources
Tutorials 1

tagg1080 said:

 I feel like your understanding of what a hashtable is is somewhat lacking.


You are explaining them purely in the way that you have used it a little bit in some of your programing.


I would rather see an actual explaination of what a hashtable is, and then why they are much easier to use in this application. I don't like seeing a 'this is how I use them because they can do this', as that is honestly only helpful to noobs that want to barely learn kind of what something almost is in a roundabout way.


What is the difference between an array and a hashtable? Why would you use them over arrays? Why are they slower than arrays? Can arrays be faster/are there times where arrays are better?


Things like this come to mind.


Just my nit-picky comment.


Click to expand...


Dudde! Mind your own buissnes Ur clearly over the line!


Ur practicly calling the guy a Noob... It makes you look Spoild, because hes tutorial is Very good, and you just want more


I did notece that you said "nit-picky" at the end of your coment


Nits Do not diserve to be Picked, and "nit-picking" is an huge understatement to what you did, anyway


I'm a Noob when it comes to Hashtables, which is the subject at hand, and I understood the tutorial just fine to a respectable extent.


Anyway... If you think your self so good at Tutorials, please take a look at the tutorial I have in my signature and tell me what you think of that one (Eather in PM or post on the tutorial thread)

#### Sharalala

S

#### Sharalala

thanks man, VERY useful guide, just 2 hours ago i had no idea what a hashtable was and how to make my spells MUI, now i already did 3 MUI spells by using the hashtables

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

Sharalala said:

 thanks man, VERY useful guide, just 2 hours ago i had no idea what a hashtable was and how to make my spells MUI, now i already did 3 MUI spells by using the hashtables

Click to expand...


It's great to know that my tutorial has helped someone. And simply using hashtables does not make a spell MUI btw. I'd suggest having 3 different units cast it at the same time, and seeing if any effects go wrong. That's what I usually do to check if a spell is MUI.


Good luck spellmaking, I hope to see some submitted spells from you

#### Skasi

#### Skasi

 Level 6

JoinedApr 18, 2011Messages47

Err, I'm not that much into hashtables yet so I need to ask, just to make sure.


In your example showing 'How we can use Handles Effectively.' I do not understand what the following line is needed for. To me it appears to be redundant.

Set EX_Target = (Target unit of ability being cast)
And wouldn't the following be more clear and efficient?

Set EX_Target = (Target unit of ability being cast)

Unit Group - Add EX_Target to EX_Group

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

Simple mistake, shoulda used the unit variable instead of (Target Unit of ability being cast), like you said. I whipped up those two triggers in under a minute, so I'm not suprised something was wrong. Nice catch.

#### noob

#### noob

 Level 14

JoinedSep 28, 2011Messages968

Resources: 5

#### Resources
Maps 4Tutorials 1

 thanks man, VERY useful guide, just 2 hours ago i had no idea what a hashtable was and how to make my spells MUI, now i already did 3 MUI spells by using the hashtables

Click to expand...

Make mui spells with arays because it is faster that using a group and an hastable and we never need to have a spell casted 8192 in a time or who affects 8192 units at a time and arrays are usable by all warcraft 3 version not only with version over 1.24 .

#### indomitable1319

#### indomitable1319

 Level 19

JoinedJan 22, 2010Messages2,583

Resources: 5

#### Resources
Maps 3Spells and Systems 1Tutorials 1

Hashtables are more flexible.


Besides, people are expected to play on the latest versions already.

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

If you have touched battle.net you can almost definitely use hashtables. On top of that, if you don't have the same version as the pre-1.24v you can't even play LAN together, so just get the idiot to update.


Both are viable options for spell creating. Dynamic indexing is about 8x faster than hashtables, but I find hashtables to be more readable. Really it depends on the user. You don't have to use hashtables if they aren't appealing to you, but don't make a stupid post on my tutorial just flat out saying never to use them because they're just stupid, they aren't stupid. To edit any spell using hashtables (of which there are many), you will need to understand hashtables. Also some jobs call for the right tools; every simple system I've written have been way, way easier to write in hashtables, for me at least. It's just the way my brain is wired.

#### shadowvzs

#### shadowvzs

 Level 17

JoinedNov 13, 2006Messages1,814

Resources: 7

#### Resources
Spells and Systems 7

i can save a complect item into harshtable with item handle? example in multiboard inventory i can save my armor to player number/equipment number to itemtable and later load for check if if i have armor or no?


example if i want load 1 1 from myitem table and its dont was declared then i get error or something false/no item/0?

#### SweetestMan

#### SweetestMan

 Level 2

JoinedApr 29, 2012Messages12

Resources: 1

#### Resources
Maps 1

how can you have the "loop-actions" thingy?

#### HammerFist

#### HammerFist

 Level 21

JoinedMar 22, 2012Messages1,549

Resources: 33

#### Resources
Models 18Icons 8Skins 1Maps 2Spells and Systems 4

Dude what to tell u except thx. I was always confused with hash things xD. This made me everthing clear Thatk's approve it and +rep

#### toanlove371

#### toanlove371

 Level 2

JoinedOct 25, 2011Messages15

good tutorial !

#### Blackair

#### Blackair

 Level 4

JoinedAug 18, 2011Messages73

Resources: 1

#### Resources
Maps 1

That was a great tutorial thank you ,almost i understand about hashtables

but there is a question about the two integers called Key in hashtables can a handle with same key be overwritten ??

#### Troll-Brain

#### Troll-Brain

 Level 17

JoinedApr 27, 2008Messages2,455

Resources: 1

#### Resources
JASS 1

Blackair said:

 That was a great tutorial thank you ,almost i understand about hashtables


but there is a question about the two integers called Key in hashtables can a handle with same key be overwritten ??


Click to expand...

https://www.hiveworkshop.com/threads/hashtable-question.216002/post-2148165


So yes it will be overwritten if you use 2 handle with the same keys, even if it's 2 different types of handles like an unit and an item.

#### Sunchips

#### Sunchips

 Level 30

JoinedJul 23, 2009Messages1,028

Resources: 125

#### Resources
Models 110Icons 8Packs 7

Events

Unit - A unit Starts the effect of an ability

Conditions

(Ability being cast) Equal to Damage Over Time

Actions

Set EX_Target = (Target unit of ability being cast)

Set EX_Damage = 20.00

Set EX_Timer = 5.00

Unit Group - Add (Target unit of ability being cast) to EX_Group

Hashtable - Save EX_Timer as 1 of (Key (Target unit of ability being cast)) in EX_Hash
When I try to follow you I can't find out how to use (Key (Target unit of ability being cast)) in EX_Hash

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

JNGP does not have the proper GUI arguments for the Key() function.

Check out this part:

https://www.hiveworkshop.com/forums...complete-beginners-guide-hashtables-197381/#j

You have to use that custom script instead.

#### Sunchips

#### Sunchips

 Level 30

JoinedJul 23, 2009Messages1,028

Resources: 125

#### Resources
Models 110Icons 8Packs 7

PurgeandFire111 said:

 JNGP does not have the proper GUI arguments for the Key() function.


Check out this part:

https://www.hiveworkshop.com/forums...complete-beginners-guide-hashtables-197381/#j


You have to use that custom script instead.


Click to expand...


Okay thanks but I found out that when I used NewGen WE it didn't show up in the list but when I used the normal WE it did. So it came with a patch?

 You must log in or register to reply here.


### Similar threads

 Hashtables and MUI


wyrmlord

Jun 26, 2009

Trigger (GUI) Editor Tutorials
345

Replies245Views126K

Jul 29, 2022
Chaosy

 How to create an artificial level-up system


frettory

Nov 22, 2012

Trigger (GUI) Editor Tutorials

Replies14Views9K

Mar 13, 2013
frettory

 General Spell Creation


Chaosy

Jun 11, 2013

Trigger (GUI) Editor Tutorials

Replies35Views10K

Feb 8, 2014
PurgeandFire

 Demystifying hashtables


Antares

May 25, 2024

Trigger (GUI) Editor Tutorials

Replies7Views3K

Oct 11, 2025
Antares

 Beginner's Guide to Lightning Effects


-Derp-

Jul 31, 2012

Trigger (GUI) Editor Tutorials

Replies15Views29K

Jan 1, 2017
KILLCIDE

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