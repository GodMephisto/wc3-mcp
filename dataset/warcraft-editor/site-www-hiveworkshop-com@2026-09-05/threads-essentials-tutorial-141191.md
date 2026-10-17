---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/essentials-tutorial.141191/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Essentials Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/essentials-tutorial.141191/>
#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

#
 Deuterium's Essentials Tutorial

Bookmark:

I - Graphical User Interface (GUI)

1. Leaks

2. Avoiding the usage of Wait

3. Multi Unit Instanceability (MUI)

4. GUI Snippets

a. Event: Unit - A unit Begins casting an ability

b. Overlooking the double leak in Polar Projections

c. Replacing certain Event Response units with Triggering unit (also Jass related)

d. Using variables to retrieve the same information more than once

e. Avoiding the usage of Do nothing

f. Destroying leaks correctly (in case the objects are created inside a loop)

g. Avoiding the usage of Index A (or Index B)

h. Using Unit Group: Units In Range instead of Unit Group: Units In Region

August 23, 2009


- Released


August 26, 2009


- Fixed minor grammatical and typing mistakes


- Edited: I - 1. Leaks


- Edited: I - 4 - a. Event: Unit - A unit Begins casting an ability


- Added: I - 4 - f. Destroying leaks correctly (in case the objects are created inside a loop)


- Added: I - 4 - g. Avoiding the usage of Index A (or Index B)


August 29, 2009


- Changed I - 4 - c. Unit: Casting unit to I - 4 - c. Replacing certain Event Response units with Triggering unit


- Added: I - 4 - h. Using Unit Group: Units In Range instead of Unit Group: Units In Region


August 30, 2009


- Edited: I - 3. Multi Unit Instanceability (MUI)


- Edited: I - 4 - e. Avoiding the usage of Do nothing


- Edited: i - 4 - g. Avoiding the usage of Index A (or Index B)

aznricepuff


- Locating some of the grammatical and typing mistakes


- Pointing out some of the missing information


Dr Super Good


- Pointing out an issue on the MUI section


Eleandor


- Pointing out some of the missing information


Hanky


- Explaining some issues about using regions

##
 Introduction


To start with, this tutorial contains information on issues that tend to be usually unknown to beginner spell-makers and are yet essential to the efficiency of a trigger.

* I'd advice those who intend to read this tutorial to either have some basic knowledge in spell-making or to read Basics of Trigger Enhancing Spells by Daelin.


The purpose for such issues being unknown to beginner spell-makers is that all authors tend to overlook such efficiency snippets in order to make their tutorials easier to grasp - and that makes sense.


This tutorial doesn't necessary go into the deep details of the issues mentioned. However, it provides enough information which allow the reader to make an efficient spell.


What's more, this tutorial would be updated on the run as I encounter more problems with spell-making. Thus, there is a chance this would include some Jass snippets in the future too.

##
 I - Graphical User Interface (GUI)

###
 I - 1. Leaks

What are leaks?


Leaks are basically information (which does take up RAM during gameplay) which is never cleaned up from the game.


There are several types of leaks, but most are common in the manner in which they are avoided.


Generally, leaks are created by objects that extend handles (positions, unit groups, player groups, special effects, etc...).


Any uneducated use of such objects would lead to memory leaking, for instance:

Unit - Create 1 Footman for (Owner of (Triggering unit)) at (Position of (Triggering unit)) facing 0.00 degrees
Here, Position of (Triggering unit) would create a handle, but this handle is never destroyed.

How to avoid?


Generally, to avoid leaks, the object-handle to be used is set into a variable of it's type. For instance:

Set GroupVariable = (Units of type Footman)
Then, the variable is used to retrieve the handle information throughout actions. For instance:

Unit Group - Pick every unit in GroupVariable and do (Actions)
Once the user is done from the variable, he basically destroys the handle using certain Custom Script commands distinct for every object type. An example would be:

Custom script: call RemoveLocation(udg_PointVariable)
Here's a list of what the most usual commands are:

Object TypeCommand


Position/Locationcall RemoveLocation(<variable name>)


Unit Groupcall DestroyGroup(<variable name>)


Player Groupcall DestroyForce(<variable name>)

* In contrast to most other leaks, Special Effects can be destroyed using GUI actions:

Special Effect - Destroy (Last created special effect)
* Variables, when inserted into Custom Scripts are preceeded by the prefix: udg_ (which means: User Defined Global)

Leaking Example:

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- This action here leaks: Position of (Triggering unit) --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Unit - Create 1 Footman for (Owner of (Triggering unit)) at (Position of (Triggering unit)) facing 0.00 degrees
How to avoid?

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- First of all, we set the position into a point variable --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Set PointVariable = (Position of (Triggering unit))

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Then we freely use the variable in the actions to retrive the saved handle --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Unit - Create 1 Footman for (Owner of (Triggering unit)) at PointVariable facing 0.00 degrees

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Once we're done from the position, we remove it using Custom Script --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Custom script: call RemoveLocation(udg_PointVariable)

Leaking Example:

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- This action here leaks: Units of type Footman --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Unit Group - Pick every unit in (Units of type Footman) and do (Actions)
How to avoid?

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- First of all, we set the unit group into a unit group variable --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Set UGroupVariable = (Units of type Footman)

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Then we freely use the variable in the actions to retrive the saved handle --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Unit Group - Pick every unit in UGroupVariable and do (Actions)

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Once we're done from the unit group, we remove it using Custom Script --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Custom script: call DestroyGroup(udg_UGroupVariable)

Leaking Example:

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- This action here leaks: Player group(Player 1 (Red)) --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Player Group - Pick every player in (Player group(Player 1 (Red))) and do (Actions)
How to avoid?

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- First of all, we set the player group into a playergroup variable --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Set PGroupVariable = (Player group(Player 1 (Red)))

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Then we freely use the variable in the actions to retrive the saved handle --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Player Group - Pick every player in PGroupVariable and do (Actions)

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Once we're done from the unit group, we remove it using Custom Script --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Custom script: call DestroyForce(udg_PGroupVariable)

Leaking Example:

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- This action here leaks a special effect --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Special Effect - Create a special effect attached to the origin of (Triggering unit) using Abilities\Spells\Orc\CommandAura\CommandAura.mdl
How to avoid?


Now special effects are a bit different. There are two methods:

-------- ---------------------------------------------------------------------------------- --------

-------- Special Effect is created --------

-------- ---------------------------------------------------------------------------------- --------

Special Effect - Create a special effect attached to the origin of (Triggering unit) using Abilities\Spells\Orc\CommandAura\CommandAura.mdl

-------- ---------------------------------------------------------------------------------- --------

-------- If no special effect is to be created before destroying it, this could be done: --------

-------- ---------------------------------------------------------------------------------- --------

Special Effect - Destroy (Last created special effect)
Or:

-------- ---------------------------------------------------------------------------------- --------

-------- Special Effect is created --------

-------- ---------------------------------------------------------------------------------- --------

Special Effect - Create a special effect attached to the origin of (Triggering unit) using Abilities\Spells\Orc\CommandAura\CommandAura.mdl

-------- ---------------------------------------------------------------------------------- --------

-------- We store the Last created special effect in a variable --------

-------- ---------------------------------------------------------------------------------- --------

Set EffectVariable = (Last created special effect)

-------- ---------------------------------------------------------------------------------- --------

-------- Then special effect is destroyed --------

-------- ---------------------------------------------------------------------------------- --------

Special Effect - Destroy EffectVariable

Using bj_wantDestroyGroup:


This Custom Script command can only be used to destroy group leaks instantly after the group is created and it's actions run. This method doesn't require any array:

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- First thing we do is set the bj_wantDestroyGroup = true --------

-------- The custom script line needs to be added before every call to a unit group looping action --------

-------- If it is set to true, it will cause only the next group to be used by such an action to be destroyed; it is not persistent --------

-------- (aznricepuff, Essentials Tutorial, Post #13) --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Custom script: set bj_wantDestroyGroup = true

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Now we just pick the group and run the actions; no leak to worry about --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Unit Group - Pick every unit in (Units currently selected by Player 1 (Red)) and do (Actions)
Loop - Actions
Unit - Kill (Picked unit)

The custom script line needs to be added before every call to a unit group looping action and that it if set to true, it will cause only the next group to be used by such an action to be destroyed; it is not persistent.

###
 I - 2. Avoiding the usage of Wait


Well, using the Wait action might not seem like a real problem, but it can be a problem in certain cases.


To start with the facts, the minimal value for Wait (or TriggerSleepAction() in Jass) would drop down to 0.27 seconds and not less whatsoever - even if the inserted number is less than 0.27 seconds.


Also, people tend to dislike the usage of Wait since they consider it not to be very precise. What's more, it would also ruin the Multi Instanceability (MUI'ness) of a spell - that would be explained later in this tutorial.


So let's start.


Suppose this simple case:

Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Blink

Actions
// The actions that come before the wait

-------- -------------------------------------------------------------------------------------------------------------- --------

Wait 0.30 seconds

-------- -------------------------------------------------------------------------------------------------------------- --------

// The actions that come after the wait

How would we fix this? Well, pretty simple, and this applies to all cases. Definitely, some basic mathematical wit is required for each different case; I'll be giving examples of some cases.


Back to this case; the solution for the issue would consist of two triggers, one which is triggered when the ability is casted, and the other which runs every certain seconds:

Main Trigger
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Blink

Actions
-------- The actions that come before the wait --------

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Then, we turn on the Loop Trigger --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Trigger - Turn on Loop Trigger <gen>

Loop Trigger (Initially off)
Events
-------- -------------------------------------------------------------------------------------------------------------- --------

-------- We set an event which runs every certain seconds --------

-------- The periodic time can vary, it depends on what the user needs --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Time - Every 0.01 seconds of game time

Conditions

Actions
-------- -------------------------------------------------------------------------------------------------------------- --------

-------- We use a real variable to track the time --------

-------- The time added to the counter equal to the periodic time in the events --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Set Counter = (Counter + 0.01)

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Now, we check if the time did exceed the time it's supposed to wait --------

-------- -------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Counter Greater than or equal to 0.30

Then - Actions
// The actions that come after the wait

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Don't forget to turn off the trigger afte the actions are triggered --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Trigger - Turn off (This trigger)

Else - Actions

Scenario:

Looping with Wait
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Blink

Actions
// Any actions before the loop

For each (Integer IntegerVar) from 1 to 10, do (Actions)
Loop - Actions
// The actions through which we loop

Wait 0.10 seconds

// Any actions after the loop

This would basically make the actions run every 0.1 seconds for 10 times, thus basically running every 0.1 seconds for 1 second. Thus there are many ways to do this, yet I'll demonstrate the best.

Solution:

Main Trigger
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Blink

Actions
// The actions before the loop

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Then, we turn on the Loop Trigger --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Trigger - Turn on Loop Trigger <gen>

Loop Trigger (Initially off)
Events
-------- -------------------------------------------------------------------------------------------------------------- --------

-------- We set the periodic time equal to the time gap between each loop instance --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Time - Every 0.10 seconds of game time

Conditions

Actions
// The actions through which we loop

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- We use a integer variable to track how many times the loop had run --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Set InstanceCounter = (InstanceCounter + 1)

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Now we check if enough instances have run --------

-------- -------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
InstanceCounter Greater than or equal to 10

Then - Actions
// The actions that come after the wait

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Turn off the trigger afte the actions are triggered --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Trigger - Turn off (This trigger)

Else - Actions

Scenarion:

Having more than one Wait
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Blink

Actions
-------- -------------------------------------------------------------------------------------------------------------- --------

-------- So let's consider this complicated case --------

-------- -------------------------------------------------------------------------------------------------------------- --------

// First set of actions

Wait 0.15 seconds

// Second set of actions

Wait 0.05 seconds

// Final set of actions

Solution:


First of all, to make the trigger most efficient, we find a greatest common periodic time between all Wait values. In this case, the greatest common denominator of 0.15 and 0.20 is 0.05.

Main Trigger
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Blink

Actions
-------- -------------------------------------------------------------------------------------------------------------- --------

-------- As usual, we start with the initial actions --------

-------- -------------------------------------------------------------------------------------------------------------- --------

// First set of actions

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Then we turn on the looping trigger --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Trigger - Turn on Loop Trigger <gen>

Loop Trigger (Initially off)
Events
-------- -------------------------------------------------------------------------------------------------------------- --------

-------- The periodic time picked by the user --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Time - Elapsed game time is 0.05 seconds

Conditions

Actions
-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Now we track the time with our counter --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Set WaitCounter = (WaitCounter + 0.05)

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- We check if 0.15 seconds have passed --------

-------- Also since we want this to run only once, we flag it with a boolean --------

-------- Basically, if boolean is true - meaning these actions have ran - then they won't run anymore --------

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Now here, the flag could have been not used --------

-------- and instead we could use the condition WaitCounter equal to 0.15 --------

-------- and it would work just fine --------

-------- but what if we couldn't find a common denominator for the periodic time? --------

-------- Suppose we're going at intervals of 0.04 --------

-------- it'll skip the 0.15 - ... 0.12, 0.16... - thus the flag is usually needed --------

-------- -------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
WaitCounter Greater than or equal to 0.15

Flag Equal to False

Then - Actions
// Second set of actions

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- And we set flag to true, announcing that this have ran --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Set Flag = True

Else - Actions

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Now we want to start with the final set of actions --------

-------- Also some basic math, we already waited 0.15 seconds --------

-------- and we also want to wait an additional 0.05 seconds --------

-------- that is 0.20 seconds --------

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Here no flag is needed since it's the last wait --------

-------- and we're going to turn the trigger off instantly after the actions --------

-------- -------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
WaitCounter Greater than or equal to 0.20

Then - Actions
// Final set of actions

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- We turn off the trigger --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Trigger - Turn off (This trigger)

Else - Actions

###
 I - 3. Multi Unit Instanceability (MUI)


Simply said, a Multi Unit Instanceable (MUI) spell is a spell which would function normally if it's cast while another instance of the same spell is running.

* Instant spells (i.e. spells with no Wait at all) are already MUI due to the nature of trigger queuing.


Basically, the purpose for a spell not to be MUI is that in GUI global variables are used and more than one instance would interfere with the information the variables hold, and thus bugging the spell.


There are different approaches of making a spell MUI; however, I'm only going to explain Paladon's Indexing System, as it's my favorite, the simplest, and efficient enough. In this method, MUI'ness is achieved through giving each instance of the spell a unique array for its variables, thus preventing the interference of one instance to another. That would achieve spell's MUI'ness.


Other manners of making spells MUI are found in the following tutorials:

Dynamic Values Storage by Kingz

Hashtables and MUI by wyrmlord

[GUI] Efficient Arrays - Multiinstanceabilty for Timers, Custom Values, and more by PurplePoot


The following triggers explain how to make a spell MUI using Paladon's Indexing System:

Main Trigger
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Mirror Image

Actions
-------- ------------------------------------------------------------------------------------------------------------- --------

-------- Ok to start with, let me explain a bit --------

-------- IndexOne doesn't do anything other than check how may instances of the spell is running --------

-------- if IndexOne equal to 0, then no one is using the spell, if it's 1, then there's one unit --------

-------- if it's equal to n then there are n units using the abilities --------

-------- ------------------------------------------------------------------------------------------------------------- --------

-------- Since all the units here are using the same Loop Trigger (the point of MUI) we make a condition --------

-------- if IndexOne is originally zero, which means that no unit was using the ability which also means the Loop was off --------

-------- we make a condition to check if IndexOne equal to zero --------

-------- if it returns true then we turn on the Loop Trigger --------

-------- ------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
DT_IndexOne Equal to 0

Then - Actions
Trigger - Turn on Loop Trigger <gen>

Else - Actions

-------- ------------------------------------------------------------------------------------------------------------- --------

-------- Now to announce the number of units using the spell, we keep track of that by adding 1 to the value of IndexOne --------

-------- Suppose there were no units using the ability, now there's one, so this makes the value of IndexOne to 1 --------

-------- ------------------------------------------------------------------------------------------------------------- --------

Set DT_IndexOne = (DT_IndexOne + 1)

-------- ------------------------------------------------------------------------------------------------------------- --------

-------- Now, to make the whole thing MUI, we are going to use a different array for each unit --------

-------- Which is exactly like using a different variable --------

-------- So we using another index, IndexTwo --------

-------- This is gonna hold a different number for each instance so that each unit has his own array and thus his own variables --------

-------- Let's say the value of IndexTwo is already 3 units using the ability, it'll become 4 for this intance --------

-------- ------------------------------------------------------------------------------------------------------------- --------

Set DT_IndexTwo = (DT_IndexTwo + 1)

-------- ------------------------------------------------------------------------------------------------------------- --------

-------- This is a Boolean, which checks if the Loop Trigger should be running for that specific unit --------

-------- since the Loop might be running for different units having the ability, we don't want it to affect the unit who has it done --------

-------- it will be used as a condition in the Loop Trigger --------

-------- having it True announces that the unit is using the Loop Trigger and so it will get affected by the Loop Trigger --------

-------- As you see, it's indexed, and thus it's specific for each unit --------

-------- ------------------------------------------------------------------------------------------------------------- --------

Set DT_LoopIsOn[DT_IndexTwo] = True

-------- ------------------------------------------------------------------------------------------------------------- --------

-------- Now in the main trigger we use IndexTwo as an array for all variables with no exceptions --------

-------- (unless it's a universal variable which holds the same info for all the units) --------

-------- here go all your actions prior to the Looping --------

-------- ------------------------------------------------------------------------------------------------------------- --------

-------- Here, basically since the point variable would be set and destroyed instantly, we don't necessarily need to "Index" it --------

Set DT_VariableExamplePoint = (Position of (Triggering unit))

Unit - Create 1 Footman for (Owner of (Triggering unit)) at DT_VariableExamplePoint facing Default building facing degrees

-------- This last created unit however would be created and kept until the end of the cast, and thus is indexed --------

Set DT_VariableExampleUnit[DT_IndexTwo] = (Last created unit)

Custom script: call RemoveLocation(udg_DT_VariableExamplePoint)

Loop Trigger (Initially off)
Events
Time - Every 0.03 seconds of game time

Conditions

Actions
-------- ------------------------------------------------------------------------------------------------------------- --------

-------- Here we work slightly different --------

-------- to have the same Loop Trigger to run for all the units seperatly, we use a Loop --------

-------- This Loop uses IndexThree as it's integer, which is basically a different variable then IndexOne and IndexTwo --------

-------- This Loop runs from integer 1 (which is the minimal possible array number possible for IndexTwo) --------

-------- to IndexTwo (which is the highest number possible) --------

-------- Thus all the units using the spell whose IndexTwo varies from 1 to n, --------

-------- will have their actions running in variables with the same specific array integer used in the Main Trigger for each unit --------

-------- ------------------------------------------------------------------------------------------------------------- --------

For each (Integer DT_IndexThree) from 1 to DT_IndexTwo, do (Actions)
Loop - Actions
-------- ------------------------------------------------------------------------------------------------------------- --------

-------- Now IndexThree is used for all the arrays instead of IndexTwo --------

-------- ------------------------------------------------------------------------------------------------------------- --------

-------- This condition checks whether the Boolean is true or false, if it's true, it'll run for that specific unit --------

-------- If it's false, then it'll skip the actions for this specific array number and move on to the other array number and so on --------

-------- ------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
DT_LoopIsOn[DT_IndexThree] Equal to True

Then - Actions
-------- ------------------------------------------------------------------------------------------------------------- --------

-------- The WaitCounter which counts how long the loop has been running --------

-------- You can see the number which I add is equal to the timer event --------

-------- ------------------------------------------------------------------------------------------------------------- --------

Set DT_WaitCounter[DT_IndexThree] = (DT_WaitCounter[DT_IndexThree] + 0.03)

-------- ------------------------------------------------------------------------------------------------------------- --------

-------- Condition to check your counter and all your actions --------

-------- ------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
DT_WaitCounter[DT_IndexThree] Less than 10.00

Then - Actions
-------- The actions --------

Unit - Make DT_VariableExampleUnit[DT_IndexThree] face Default building facing over 0.00 seconds

Else - Actions
-------- ------------------------------------------------------------------------------------------------------------- --------

-------- Now the counter exceeded the time you specified so we reset the counter --------

-------- and announce that we're done of this instance --------

-------- We also destroy whatever has to be destroyed or removed --------

-------- ------------------------------------------------------------------------------------------------------------- --------

Unit - Add a 0.01 second Generic expiration timer to DT_VariableExampleUnit[DT_IndexThree]

Set DT_WaitCounter[DT_IndexThree] = 0.00

-------- ------------------------------------------------------------------------------------------------------------- --------

-------- We announce that this unit is done of it's actions by setting the Boolean to False --------

-------- ------------------------------------------------------------------------------------------------------------- --------

Set DT_LoopIsOn[DT_IndexThree] = False

-------- ------------------------------------------------------------------------------------------------------------- --------

-------- We subtract the value of IndexOne by 1 since one instance is done, and as we've said, --------

-------- IndexOne records the number of INSTANCES RUNNING only --------

-------- ------------------------------------------------------------------------------------------------------------- --------

Set DT_IndexOne = (DT_IndexOne - 1)

-------- ------------------------------------------------------------------------------------------------------------- --------

-------- Now this condition checks if there are no more instance running --------

-------- So, if IndexOne is zero, that implies that there are no instance running --------

-------- in that case, we set the value of IndexTwo back to zero --------

-------- (a sort of recycling method so that the value doesn't keep on summing up to a high value) --------

-------- and then since there are no instance running and using the Loop (all the Booleans would be False in that case) --------

-------- we turn the trigger off for the sake of efficiency --------

-------- ------------------------------------------------------------------------------------------------------------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
DT_IndexOne Equal to 0

Then - Actions
Set DT_IndexTwo = 0

Trigger - Turn off (This trigger)

Else - Actions

Else - Actions

###
 I - 4. GUI Snippets


These are basically some little GUI mistakes done by beginner spell-makers:

a. Event: Unit - A unit Begins casting an ability


This event is actually buggy and can cause problems in-game (unless used wisely) since this runs the trigger few moments before the cooldown runs and mana is consumed. So basically, the player (if talented enough) can keep on casting the ability without losing any mana or having a cooldown.


Instead - to solve this problem - the following event is used:

Unit - A unit Starts the effect of an ability
Nevertheless, the event Unit - A unit Begins casting an ability is not always useless. It's extremely useful in some situations; i.e. if you want to stop the casting of a spell before the mana is used and the cooldown is triggered through triggers (aznricepuff, Essential Tutorial, Post #13).

b. Overlooking the double leak in Polar Projections


Polar Projections do actually leak two locations instead of one:

Unit - Create 1 Footman for (Owner of (Triggering unit)) at ((Position of (Triggering unit)) offset by 200.00 towards 0.00 degrees) facing 0.00 degrees
The first leak is the central point: Position of (Triggering unit)


The second leak is the projected point(Position of (Triggering unit)) offset by 200.00 towards 0.00 degrees)


Solution:

Set TrigUnitLoc = (Position of (Triggering unit))

Set PolarProjLoc = (TrigPosition offset by 200.00 towards 0.00 degrees)

Unit - Create 1 Footman for (Owner of (Triggering unit)) at PolarProjLoc facing 0.00 degrees

Custom script: call RemoveLocation(udg_TrigUnitLoc)

Custom script: call RemoveLocation(udg_PolarProjLoc)

c. Replacing certain Event Response units with Triggering unit


Certain Event Response units basically call for the Triggering unit in the background of the game, thus instead we should manually and directly use Triggering unit in order to achieve better efficiency.


A list of these units would be:

GUI FormJass Form


Attacked unitGetAttackedUnitBJ()


Cancelled structureGetCancelledStructure()


Constructing structureGetConstructingStructure()


Decaying unitGetDecayingUnit()


Dying unitGetDyingUnit()


Entering unitGetEnteringUnit()


Hero manipulating itemGetManipulatingUnit()


Learning HeroGetLearningUnit()


Leaving unitGetLeavingUnit()


Leveling HeroGetLevelingUnit()


Loading unitGetLoadedUnitBJ()


Ordered unitGetOrderedUnit()


Ownership-changed unitGetChangingUnit()


Researching unitGetResearchingUnit()


Revivable HeroGetRevivableUnit()


Reviving HeroGetRevivingUnit()


Selling unitGetSellingUnit()


Summoned unitGetSummonedUnit()

d. Using variables to retrieve the same information more than once


Suppose that we have a trigger where we use Target unit of ability being cast more than once. Basically, every time we use it, the computer has to do some work and return the right unit.


Instead, we set Target unit of ability being cast into a variable before using it, and then we use the variable throughout the whole spell instance. Retrieving information from a variable is much faster.

* This applies to everything; this isn't specific to the case of Target unit of ability being cast.


Example:

Scenario
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Mirror Image

Actions
Set TargUnit = (Target unit of ability being cast)

Unit - Hide TargUnit

Unit - Kill TargUnit

Unit - Remove TargUnit from the game

e. Avoiding the usage of Do nothing


Basically and simply said, avoid the usage of Do nothing:

Do nothing
Do nothing does nothing but decrease the efficiency of a spell.


The only reason Blizzard added this Do nothing action is due to the presence of the single lined if/then/else action and the need to fill in the "else" part with it in case there are no suitable actions. So to avoid using the Do nothing, avoid using the single lined if/then/else action.

f. Destroying leaks correctly (in case the objects are created inside a loop)


Take this faulty example:

For each (Integer LoopingInteger) from 1 to 4, do (Actions)
Loop - Actions
-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Here, since the loop runs from 1 to 4, then 4 leaks are created --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Special Effect - Create a special effect attached to the chest of (Triggering unit) using Abilities\Spells\NightElf\shadowstrike\shadowstrike.mdl

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- However, we're detroying the leak outside of the loop, --------

-------- destroying thus only the last created leak - leaving 3 other leaks undestroyed --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Special Effect - Destroy (Last created special effect)
However, the solution to this is pretty simple:

For each (Integer LoopingInteger) from 1 to 4, do (Actions)
Loop - Actions
-------- -------------------------------------------------------------------------------------------------------------- --------

-------- Here - again - since the loop runs from 1 to 4, then 4 leaks are created --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Special Effect - Create a special effect attached to the chest of (Triggering unit) using Abilities\Spells\NightElf\shadowstrike\shadowstrike.mdl

-------- -------------------------------------------------------------------------------------------------------------- --------

-------- In contrast to the previous example, we're detroying the leak inside of the loop, --------

-------- destroying the last created leak directly after it get created --------

-------- -------------------------------------------------------------------------------------------------------------- --------

Special Effect - Destroy (Last created special effect)

* This applies to all leaking objects.

g. Avoiding the usage of Integer A (or Integer B)


This is a rather complicated issue to explain to beginner users. Yet - simply said - Integer A (I'm going to ignore Integer B since it is the same as Integer A and both function alike) is a predefined global variable. Thus, the information it might carry while looping could be changed due to any interception by other triggers which also contain Integer A. What happens then - once the looping of the original trigger resumes - is that the value of Integer A is different leading to faulty looping - i.e. bug.


 PurplePoot said:



 The actual reason is because (Integer A) is a global so two loops running it at once interfere with one another as they both access it and change it at the same time (more accurately, one changes it while the other is waiting for the first to finish).


Click to expand...


So just avoid using Integer A or Integer B. You could do so by picking the following action in the trigger editor:


The integer variable used should be unique for each trigger. If in one trigger - for example - LoopIntegerOne is used, a different trigger should use a different integer variable (LoopIntegerTwo for example).

h. Using Unit Group: Units In Range instead of Unit Group: Units In Region


As a remark, if used wisely and efficiently, Units In Region is not much of a problem. However, it is prefered to use Units In Range for several reasons.

Units In Region tend to be slower then Units In Range since the Units In Region requires an additional parameter than those needed for Units In Range: a region.


What's more, if the range at which the units should be involved in a group varies, then a new region would be needed for every call in the case of Units In Regions; however, in the case of Units In Range, only a different real number defining the new range would change. In such cases, Units In Range happens to be far more efficient then Units In Region.


Also, the shape of a region happens to be rectangular, which is pretty much impractical in spell making compared to the circular shape used by Units In Range.


So as a conclusion, avoid using Units In Region in the codes of custom spells since it is easily avoidable and never essential for any spell. In my opinion, the usage of Units In Region is only fine in cases where the the region is preset in the map for general mapping purposes.

#### Attachments

Integer A.jpg

 56 KB

 · Views: 6,713




 Last edited: Aug 30, 2009

#### -Berz-

#### -Berz-

 Level 37

JoinedMar 5, 2008Messages3,879

Resources: 143

#### Resources
Icons 127Packs 1Skins 2Spells and Systems 13

Nicely done

Simple explanation and nice and easy examples, I would recommend every new "GUI-er" to read these little facts, I would also make this sticky in spell section, because it's easy and if you don't get this, then quit right away.

Understanding this little facts, is a path to become a good GUI spell maker (enough to freely submit your resources without getting comments e.g. Fail, leaks, waits, non MUI...)

Deu, thanks for this writing this up!

There is one more thing that you could do Deu, use hidden tags for longer triggers

~Berz

#### Paladon

#### Paladon

 Level 31

JoinedDec 6, 2007Messages2,226

Resources: 51

#### Resources
Models 1Icons 29Packs 2Spells and Systems 19

Your example of MUIness is just one possible way.

I know you know that, but you should mention that.

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

@ -BerZeKeR-:

BZK, well I thought about that but I didn't since they are major parts of explanation...

the extra example triggers are hidden however, as you can see

@ Paladon:

I did


 Deuterium said:



 Other manners of making spells MUI are found in the following tutorials:

Dynamic Values Storage by Kingz

Hashtables and MUI by wyrmlord

[GUI] Efficient Arrays - Multiinstanceabilty for Timers, Custom Values, and more by PurplePoot


Click to expand...

Thanks to both for checking

#### Paladon

#### Paladon

 Level 31

JoinedDec 6, 2007Messages2,226

Resources: 51

#### Resources
Models 1Icons 29Packs 2Spells and Systems 19

The line below

 The following trigger explains how to make a spell MUI:


Click to expand...

There.

Otherwise, it looks as if every link explains my old way which does in fact do just one of them.


 Last edited: Aug 22, 2009

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

Fixed

#### Dark_Dragon

#### Dark_Dragon

 Level 15

JoinedJul 19, 2007Messages618

Resources: 11

#### Resources
Tools 1Spells and Systems 9Tutorials 1

impressive gj Deut! its well explained and k like BZK said xD it might be nice idea to make hidden tages on long triggers but its not like it cant live without it

ill suggest any new GUI-er to read this

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

Maybe I'll hide them once I update it... let me find more gramatical mistakes before I do so. But I mean, hiding them would be like hiding the main explanation :s and that sort of doesn't make sense to me...


But I'll think about that


Thanks for checking!

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

not bad not bad, rechecking again

EDIT:

You should mention in the MUI wayness that a boolean isnt needed if you do it right, like me

Mention about array stored points, needed in most cases

Variablize target event isnt always needed, ofc it's faster but you wont recognize it anyway

If you want to mention about varablize something that is an event stuff that is used more often is level of ability being cast

Hidden the triggers for perfection!

I see it very well made but you should have a spell inside that got loads of indexed stuffs so ppl get it better

Very good, sure about that one!

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1


 baassee said:



 You should mention in the MUI wayness that a boolean isnt needed if you do it right, like me

Click to expand...

Well yes true, you can use the unit comparison... but I mentioned that it could be done in different ways. I demonstrated the one that I prefer.


 baassee said:



 Mention about array stored points, needed in most cases

Click to expand...

I don't quite understand what you mean by this... :s


 baassee said:



 Variablize target event isnt always needed, ofc it's faster but you wont recognize it anyway


If you want to mention about varablize something that is an event stuff that is used more often is level of ability being cast

Click to expand...

Umm, yeah Level of ability being cast sounds better  maybe I'll fix that if I'm not too lazy


 baassee said:



 Hidden the triggers for perfection!


Click to expand...

Well I guess I'll hide all triggers... good?  (but I'll wait a bit until I've got more to update)


 baassee said:



 I see it very well made but you should have a spell inside that got loads of indexed stuffs so ppl get it better

Click to expand...

Well, I sort of hate coding in GUI nowadays... and what's more, I've been taking a break from coding lately  yet maybe some day I'll improve one of my GUI spells... make it as efficient as it could get, and then attach it here


 baassee said:



 Very good, sure about that one!


Click to expand...

Thanks!

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

Ok, there's some small grammar/typo issues:

 On contrast to most other leaks, Special Effects can be destroyed using GUI actions:


Click to expand...

In contrast to...

 Definetly, some basic mathematical wit is required for each different case, I'd be giving examples of some cases.


Click to expand...

Definitely, some basic...each different case; I'll be giving...

 if Index[1] is originally zero, which means that no unit was using the ability which mean the Loop was off


Click to expand...


...the ability which means the Loop was...


I have to go now, but I'll review this more in-depth later.

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

Oh thanks  I'll fix them all at once once you finish the whole review. I didn't use Microsoft Office to write this so well... couldn't locate all the typos.


Also, I want to ask you, do you agree with the guys that I should hide the triggers which aren't hidden? Because the purpose I didn't hide them is that they're a major part of the explanation... and thus hiding them didn't feel right to me.


EDIT:


Ok I did skim through the tutorial again to find some more grammatical mistakes... I fixed those that I found and the ones you mentioned.


Thanks


I'll wait for your opinion on the hidden thing...

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

I don't really have an opinion either way on hiding those triggers. If you think hiding them makes your tutorial more effective, then do it; otherwise, don't worry about it...I'm not gonna not approve it for something like that.


I found some more errors:

 There are several types of leaks, but most are common in the manner at which they are avoided.


Click to expand...


...in the manner in which they are avoided.

 Instant spells (i.e. spells with not Wait at all) are already MUI due to the nature of trigger queuing.


Click to expand...


...i.e. spells with no Wait at all....

 Instead - to solve this problem - the following even is used:


Click to expand...


I assume you meant to type "event" there.

 Do nothing does nothing except decreasing the efficiency of a spell.


Click to expand...


"...nothing except decrease the efficiency of a spell."


Ok, and now onto content-related stuff.

You might want to mention when discussing bj_wantDestroyGroup that the custom script line needs to be added before every call to a unit group looping action and that it if set to true, it will cause only the next group to be used by such an action to be destroyed; it is not persistent.

Event - Unit Starts Casting an Ability is not always useless. It's extremely useful in some situations; i.e. if you want to stop the casting of a spell before the mana is used and the cooldown is triggered.

Maybe if you could provide a list of all the useless event response actions and not just casting unit.

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1


 aznricepuff said:



 You might want to mention when discussing bj_wantDestroyGroup that the custom script line needs to be added before every call to a unit group looping action and that it if set to true, it will cause only the next group to be used by such an action to be destroyed; it is not persistent.


Click to expand...

Rightt... thanks for pointing that out!  Otherwise wouldn't have ever thought of mentioning this...


 aznricepuff said:



 Event - Unit Starts Casting an Ability is not always useless. It's extremely useful in some situations; i.e. if you want to stop the casting of a spell before the mana is used and the cooldown is triggered.


Click to expand...

Kept on telling myself not to forget mentioning this, yet I did... :s


 aznricepuff said:



 Maybe if you could provide a list of all the useless event response actions and not just casting unit.


Click to expand...

Sure... I'll go through all and check which are BJ's and which are natives...


I'll update this the following few days, going to be busy for a day or two.


Thanks for pointing my mistakes out

#### MortAr

#### MortAr

 Level 19

JoinedFeb 25, 2009Messages2,004

Resources: 11

#### Resources
Icons 6Packs 1Spells 2Tutorials 2

Woah! I make a fast preview of the tutorial and I was surprised!


Im definetily gonna read the whole of it when I have some time.


Great work, Deuterium.

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

Glad you like it  Well, I would be updating it these two days, adding one more snippet issue and improve what aznricepuff pointed out...

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

Updated... added 2 more snippets and edited some little issues and fixed some mistakes.

 Maybe if you could provide a list of all the useless event response actions and not just casting unit.


Click to expand...

Well, I might do so in the future, but testing for them would be sort of exhausting as I'd need to trigger every unit event in a map (which is difficult as I need to set the map to run the specific event) and retrieve the Id of the unit if I use: GetTriggerUnit() and see if I get an Id or not... and then compare it to the Id of the corresponding Event Response.


I might test the units which are mostly used instead of all - unless you know an easier way for testing for that.


Hope it's good enough to be approved now

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

Very good adding the Integer A/B problems and yeah I agree, hard to explain but I saw in one post from Purple that had a pretty good explenation, I will look for it if I got time. Very good.

Found it but not a big explenation but you still get it.

PurplePoot said:

 The actual reason is because (Integer A) is a global so two loops running it at once interfere with one another as they both access it and change it at the same time (more accurately, one changes it while the other is waiting for the first to finish).


Click to expand...

Look at the thread for more info and yes seed you even commented there!

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

Yes Purple had explained that... and I know where it is  but if I quote that, I would have to go deep in the explanation of the problem, complicating the whole thing for beginners...

Keeping it as it is... well, gives the reader a better chance of getting a brief idea of why not to use it.


 PurplePoot said:



 If you kill a unit, any triggers with A Unit Dies will interrupt the current thread rather than wait for it. The same goes for any other events or function calls.


Click to expand...

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

you really dont need to explain it more or? I get it, you get it, berzerk get it, my mom get it so we all get it (partly all)

#### -Berz-

#### -Berz-

 Level 37

JoinedMar 5, 2008Messages3,879

Resources: 143

#### Resources
Icons 127Packs 1Skins 2Spells and Systems 13

*off-topic

I don't get it

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

-BerZeKeR- said:

 *off-topic


I don't get it

Click to expand...


lols  [/offtopic]

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1


 -BerZeKeR- said:



 I don't get it

Click to expand...

Ok, so you know that triggers tend to queue to run one after the other? Well, some events tend to intercept not only the queue, but actually the actions of the trigger.

So in case it intercepts at the point of the Integer A loop, and the intercepting trigger itself uses Integer A, BAM, you ruined the loop for the intercepted trigger by changing the original value of Integer A...

Got it now?

#### -Berz-

#### -Berz-

 Level 37

JoinedMar 5, 2008Messages3,879

Resources: 143

#### Resources
Icons 127Packs 1Skins 2Spells and Systems 13

Yeah, but this is so rare that...

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1


 -BerZeKeR- said:



 Yeah, but this is so rare that...


Click to expand...

True indeed in the case of spells... but imagine having a WHOLE map with tons of triggers and custom made spells... well, it wouldn't be rare anymore.

And even if a bug is rare, that doesn't mean you don't avoid it

#### -Berz-

#### -Berz-

 Level 37

JoinedMar 5, 2008Messages3,879

Resources: 143

#### Resources
Icons 127Packs 1Skins 2Spells and Systems 13

You can also use Loop B

Yeah I get the point now

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1


 -BerZeKeR- said:



 Yeah, but this is so rare that...


Click to expand...

True indeed in the case of spells... but imagine having a WHOLE map with tons of triggers and custom made spells... well, it wouldn't be rare anymore.


And even if a bug is rare, that doesn't mean you don't avoid it

#### -Berz-

#### -Berz-

 Level 37

JoinedMar 5, 2008Messages3,879

Resources: 143

#### Resources
Icons 127Packs 1Skins 2Spells and Systems 13

I said i get it! xP [/spam]

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

Sorry... I told you FireFox is going crazy

#### mjllonir

#### mjllonir

 Level 9

JoinedMay 31, 2009Messages439

Resources: 5

#### Resources
Icons 1Spells and Systems 3Tutorials 1

This clarifies so much, and now I know what would make my triggers go better

Amazing tutorial, great job writing it! Rep to you!

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

 I might test the units which are mostly used instead of all - unless you know an easier way for testing for that.


Click to expand...


Go into blizzard.j and look at what the BJs do. If they just return GetTriggerUnit()...you know they're useless.

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

Deuterium said:

 True indeed in the case of spells... but imagine having a WHOLE map with tons of triggers and custom made spells... well, it wouldn't be rare anymore.


And even if a bug is rare, that doesn't mean you don't avoid it

Click to expand...


sounds like me, having tons of code in maps ...


YES very rare bug but I've seen what it can do and it's not good ... so avoid it

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

 Go into blizzard.j and look at what the BJs do. If they just return GetTriggerUnit()...you know they're useless.


Click to expand...

Well aznricepuff, here's the thing... that won't work since all event response units are natives except for GetAttackedUnitBJ().


Those that happen to be BJ's other than attacked unit return their natives (ex: GetLoadedUnitBJ() returns GetLoadedUnit()).


However, I ended up testing them yesterday and I've got a table of all those that function equally to GetTriggerUnit()... I'll post it as soon as I updated the tutorial.

#### Eleandor

#### Eleandor

 Level 21

JoinedAug 21, 2005Messages3,698

Resources: 3

#### Resources
Models 1Maps 1Tutorials 1

Any unit triggering the trigger is the "triggering unit". No table required because it's as simple as that. If a unit enters the region, the "triggering unit" is the entering unit. Etcetera.

Only in cases where there is no alternative, other units are used. For example: Target unit of issued order, since triggering unit already refers to the unit that was issued the order.

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

But don't you need to prove which works and which doesn't before you can state out such things to the readers...? and believe me, there happened to be one weird case where - referring to the event - GetTriggerUnit() replaces the unexpected event response unit instead of the one I was sure it'll replace.

Anyways, as I said, I already have the table set and ready, I just need to add one more snippet and update it. I'll let you know when I update it

EDIT:

And oh, well yes the table is required, since this tutorial is made for beginners. What's more, it's better to be specific and detailed when it doesn't confuse the reader

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

Updated:


- Changed I - 4 - c. Unit: Casting unit to I - 4 - c. Replacing certain Event Response units with Triggering unit


- Added: I - 4 - h. Using Unit Group: Units In Range instead of Unit Group: Units In Region

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

 Certain Event Respnse units basically call for the Triggering unit in the backgroud of the game, thus and instead we should manually and directly use Triggering unit in order to achieve better efficiency.


Click to expand...


Certain Event Response units basically call...in the background of the game, thus instead we should manually...


Just fix that and I can approve.

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

Fixed

#### Eleandor

#### Eleandor

 Level 21

JoinedAug 21, 2005Messages3,698

Resources: 3

#### Resources
Models 1Maps 1Tutorials 1

I find the leaks part lacking. There's no information on array leaks. There's no information on what precisely a leak is and how you can detect leaks yourself without having to browse through endless lists that don't cover them all anyway, or cover things that don't leak.

 How would we fix this? Well, pretty simple, and this applies to all cases. Definitely, some basic mathematical wit is required for each different case; I'll be giving examples of some cases.


Click to expand...

No. Just no. Use timers, don't overcomplicate things. Besides, if you got a wait action above .3 your accuracy increases a lot. Your average GUI newby won't need better accuracy.

 Units In Region tend to be slower then Units In Range since the Units In Region requires an additional parameter than those needed for Units In Range: a region.


Click to expand...

Is this a reason for something to be slower??? Get me some benchmarks first before you make claims based on nothing...

My intuition says the opposite. It takes 4 comparisons to check if a unit is in a rectangle, while it takes 2 multiplications, 1 addition, 1 squareroot and 1 comparison to check if a unit is in range. Both also return different results, they have different uses, so you can't even compare them.

 What's more, if the range at which the units should be involved in a group varies, then a new region would be needed for every call in the case of Units In Regions; however, in the case of Units In Range, only a different real number defining the new range would change. In such cases, Units In Range happens to be far more efficient then Units In Region.


Click to expand...

No.

I find it funny you're talking about efficiency in GUI in the first place. Efficiency is not "essential" and it's definitely not essential if you're going to use gui. If you want to encourage people to get as much efficiency as possible, learn jass. I certainly don't want to derail this topic into yet another jass vs gui thread, but the main reason GUI users give not to learn jass is that they don't need the efficiency and gui can do most of the things they need. Well, if they don't need to then don't even bother about writing "efficient" gui code. "Efficient" and "GUI" don't fit in the same sentence.

 Basically and simply said, avoid the usage of Do nothing:


Click to expand...

Maybe tell them that the only reason they added this action is because there is a if/then/else action (single instead of multiple actions). You must enter something in "else", which is why they made "Do nothing". IMO you should avoid the single if/then/else action anyway.

 So just avoid using Integer A or Integer B. You could do so by picking the following action in the trigger editor:


Click to expand...

Using a user declared Integer has the exact same problems as Integer A or Integer B. By not telling them *what* the real problem is, you're just encouraging them to make an Integer C variable and use it for all loops, and you end up having the same problems as before. Besides, the average GUI trigger will not have any problems with Integer A or Integer B.

I also find your naming bad. Variables like TargUnit? Why do you need to add "Unit"? Call it target, it's shorter, easier to read (everyone knows what a target is, but a "targ"?) and the only variables you can select in GUI will be units anyway. It's something a lot of people do and it's a bad habit.

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

Well I'll read your post more thoroughly some day later when I've got the time to edit the tutorial, but to quickly answer some of your conerns.

 Get me some benchmarks first before you make claims based on nothing...


Click to expand...

I wouldn't be mentioning anything here unless I already checked out whether it's true or not.

Apparently as I read your comments, I find that I missed to say alot of things, so thanks

 No. Just no. Use timers, don't overcomplicate things. Besides, if you got a wait action above .3 your accuracy increases a lot. Your average GUI newby won't need better accuracy.


Click to expand...

There's another problem with wait which is occur in case of game lagging or pausing the game. Also, using this way instead of timers make it easier to make the spell MUI with Indexing; however, I got tot check the hashtables case.

 I find it funny you're talking about efficiency in GUI in the first place. Efficiency is not "essential" and it's definitely not essential if you're going to use gui. If you want to encourage people to get as much efficiency as possible, learn jass. I certainly don't want to derail this topic into yet another jass vs gui thread, but the main reason GUI users give not to learn jass is that they don't need the efficiency and gui can do most of the things they need. Well, if they don't need to then don't even bother about writing "efficient" gui code. "Efficient" and "GUI" don't fit in the same sentence.


Click to expand...

Well, yet I do respect people's free will to pick whatever codin approach they want to use. Using GUI doesn't mean you can't improve some issues. And yes, Jass is far better...

Again, thanks for checking and giving advice

#### Eleandor

#### Eleandor

 Level 21

JoinedAug 21, 2005Messages3,698

Resources: 3

#### Resources
Models 1Maps 1Tutorials 1

 I wouldn't be mentioning anything here unless I already checked out whether it's true or not.


Click to expand...

Uh... that's still not a good reference. It's false until you get me some evidence...

#### Dr Super Good

#### Dr Super Good

Spell Reviewer

 Level 65

JoinedJan 18, 2005Messages27,369

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Why is DT_Index and array. You only ever use it statically so for efficency it should be 3 different variables.


DT_Index[1] = one


DT_Index[2] = two


DT_Index[3] = three

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

Oh, well if it is more efficient, then yeah sure  I'll keep that in mind for next update...

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1

Hotfix:

- Edited: I - 3. Multi Unit Instanceability (MUI)

- Edited: I - 4 - e. Avoiding the usage of Do nothing

- Edited: i - 4 - g. Avoiding the usage of Index A (or Index B)

Hopefully by now I have the major issues fixed.

Advice on how-to-improve is always welcome

#### Eleandor

#### Eleandor

 Level 21

JoinedAug 21, 2005Messages3,698

Resources: 3

#### Resources
Models 1Maps 1Tutorials 1

 The integer variable used should be unique for each trigger. If in one trigger - for example - LoopIntegerOne is used, a different trigger should use a different integer variable (LoopIntegerTwo for example).


Click to expand...

If you're not using wait actions inside the loop you can recycle the variables, this including Integer A and Integer B. Thus, like I said, if you don't use wait actions inside loops, the only reason you would need to use an integer C is when you need a triple nested loop. For everything else, Integer A and Integer B should be sufficient.

In fact, if you're using wait actions inside loops, using different variables for different triggers will still not be sufficient.

#### Deuterium

#### Deuterium

 Level 17

JoinedMar 17, 2009Messages1,350

Resources: 3

#### Resources
Spells and Systems 2Tutorials 1


 Eleandor said:



 Thus, like I said, if you don't use wait actions inside loops, the only reason you would need to use an integer C is when you need a triple nested loop. For everything else, Integer A and Integer B should be sufficient.


Click to expand...

I must say you're wrong. Go a few posts up, baassee quotes what the issue is...

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

There are actions that cause a thread to stop executing while it waits for something else to finish executing. In these cases another thread can interrupt a loop and cause global variable value corruption if you're not careful. For example, UnitDamageTarget() will cause the current thread to block until the damage is dealt, and any threads run by a trigger responding to EVENT_UNIT_DAMAGED will execute while the first thread waits. So you don't necessarily need to explicitly use waits for threads to interrupt one another.

#### Eleandor

#### Eleandor

 Level 21

JoinedAug 21, 2005Messages3,698

Resources: 3

#### Resources
Models 1Maps 1Tutorials 1

Uh, sure, but he doesn't say how to detect bugs like that. He just says to use different variables for each loop you have, and honestly that's just idiot. Especially when bugs like this happen in maybe 2% of the triggers you have.

#### aznricepuff

#### aznricepuff

 Level 11

JoinedFeb 22, 2006Messages752

Resources: 6

#### Resources
Maps 2Spells 3Tutorials 1

What's wrong with using different variables for each loop?


It's the GUI equivalent of doing:

 JASS:

local integer i = 0
loop
 exitwhen (i > 100)
 set i = i + 1
endloop


I'm pretty sure no JASS user would go through the trouble of using only one or two globals in every loop they had just because it PROBABLY wouldn't screw up. So why should GUI users do the same?

#### Eleandor

#### Eleandor

 Level 21

JoinedAug 21, 2005Messages3,698

Resources: 3

#### Resources
Models 1Maps 1Tutorials 1

Those are local variables. Creating a separate global variable for each iterator is just going to explode your variablelist when you don't have to. And the GUI variable list is already ugly.

 You must log in or register to reply here.


### Similar threads

 MUI Spells Using Artificial Waits


Tank-Commander

Sep 28, 2012

Trigger (GUI) Editor Tutorials

Replies32Views20K

Oct 16, 2015
BETABABY

 {DEPRECATED / OUTDATED} Dynamic Values Storage


Kingz

Mar 25, 2009

Trigger (GUI) Editor Tutorials
2

Replies71Views22K

Aug 8, 2009
NFWar

 Charged Ability Tutorial


Almia

Aug 5, 2012

Trigger (GUI) Editor Tutorials

Replies12Views13K

Mar 4, 2016
Almia

 State-based Passive Ability Tutorial


Blightsower

Sep 22, 2024

Trigger (GUI) Editor Tutorials

Replies8Views3K

Sep 23, 2024
Blightsower

 A Complete Beginners Guide to Hashtables


Jazztastic

Jun 15, 2011

Trigger (GUI) Editor Tutorials
2

Replies57Views45K

Nov 12, 2024
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