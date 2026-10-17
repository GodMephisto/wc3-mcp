---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/things-you-should-know-when-using-triggers-gui.233242/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Things You Should Know When Using Triggers GUI
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/things-you-should-know-when-using-triggers-gui.233242/>
#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Things You Should Know When Using Triggers / GUI

An easy guide for GUIers to be able to learn from, from beginner to advanced levels.


There is also a list of helpful tutorials at the bottom of the tutorial.


If you don't understand something feel free to post and I will help when i can.

Table of Contents

Section 1 - Variable Editor

 - Chapter 1 - Which variables are handles and which aren't

 - Chapter 2 - Dealing with array sizes

 - Chapter 3 - Miscellaneous things about variables

Section 2 - Trigger Editor

 - Chapter 1 - Dealing with Local Variables

 - - Subchapter 1 - Shadowing Global Variables

 - Chapter 2 - Dealing with Waits

 - Chapter 3 - Trigger Events to be careful with

 - Chapter 4 - Dealing with Loops

 - Chapter 5 - Actions that can't be done at Map Init

 - Chapter 6 - When to store something into a variable

 - Chapter 7 - When to destroy / null a variable

 - Chapter 8 - Preloading

 - Chapter 9 - Use of Substrings

 - Chapter 10 - GetLocalPlayer

 - Chapter 11 - Dealing with HashTables

 - Chapter 12 - How to Index

 - Chapter 13 - The Do Nothing Action

 - Chapter 14 - The Or and And Conditions

 - Chapter 15 - Using Nested Loops

 - Chapter 16 - Multi-Dimensional Arrays

 - Chapter 17 - ITE's and Nesting them ( if then else)

 - Chapter 18 - Skip Remaining Actions

 - Chapter 19 - Floating Texts

 - Chapter 20 - Timers

 - Chapter 21 - The DRY-principle

 - Chapter 22 - Making your own Functions

 - Chapter 23 - How to Color Text

 - Chapter 24 - How to Move Unit without Disrupting Orders ( SetUnitX/Y)

Section 3 - Gameplay Constants

 - Chapter 1 - Xp from Killing Enemy Heroes

 - Chapter 2 - Xp from Killing Enemy Units

 - Chapter 3 - Xp from Killing Creeps

 - Chapter 4 - Xp from Till Your Hero Lvls

Section 4 - Object Editor

 - Chapter 1 - Channel Spell

 - Chapter 2 - How the damage works

Section 5 - How to find what isn't working in your trigger

 - Chapter 1 - Debugging

Section 6 - Other tutorials that can help you improve your GUI skills

Final Comments
People that helped with this tutorial

 Almia

 Zwiebelchen

 defskull

 Kobas

 ap0calypse

 PurgeandFire
First: Always aim for efficiency. I hear a lot of GUIers say they only need to worry about clearing leaks. However, there are other problems that can slow down your triggers. By aiming to be efficient, you limit the number of operations required and therefore the trigger will run as smoothly as possible.


It may not seem to make much of a difference. However, many maps have several systems and triggers running at the same time. If they are inefficient, they may degrade FPS, or even hit the op-limit. That is why you should aim to optimize your triggers.

Section 1 - Variable Editor


This section is all about the Variable Editor.

Chapter 1 - Which variables are handles and which aren't


First you should know what a variable is. A variable is a pointer to the data structure that is stored in the memory. That being said a handle is the internal data structure that the variable points to.


In the common.j, there is a list of all handles. The variable types that are not handles are listed below.

Variables Types That Don't Point To Handles

 Integers

 Reals

 Strings

 Booleans

 Code


It is important to know which variables are handles and which aren't, because handles should be destroyed/removed when they are not needed anymore. However, the types that aren't handles (listed above), known as primitives, do not need to be destroyed/removed.


There may be some other types you encounter, such as widget. Widgets are simply objects. There are three total: units, items, destructible. Note that these are also handles.

Chapter 2 - Dealing with Array Sizes


First, you should know what the op-limit is. It is the operation limit of Warcraft III's engine for a particular thread. A thread is basically a sequence of code.


Once you hit the op-limit, the rest of the code in that trigger/thread will never fire. It is almost like "Skip Remaining Actions". This can mess up your triggers.


The limit (according to PipeDream) is 300,000 byte code operations (which is trivial information unless you trace the byte code you use).


Certain functions will start a new thread, such as TriggerExecute() or ForForce(). ( These are jass function calls)


If you use those, then you'll effectively be able to do another 300k byte code operations before hitting the op limit (or until you start a new thread).


Each trigger's actions open a new thread, so if you hit the op-limit it will usually only break one trigger. However, you should still try to avoid it by being as efficient as possible.


Now, back to array sizes. They have a maximum index of 8191, and have 8192 indexes that can be used (0 to 8191).


When you are in the variable editor and are making an array, you'll see an option called "size". This determines how many slots are initialized with values. For example, if you input a size of 5 for a timer array, then a timer will be made for all slots up to [5], including 5. (0, 1, 2, 3, 4, 5) That makes 6 timers total. The only variables that would warrant having an array size higher than 1 are listed below.

Variables that should have an array size of more than 1.

 Dialog

 Timer
All the other types should have a size of 1. The other types don't need to be initialized. It will increase the speed of saving and loading if you set them to 1. If you have too many large sizes, you may event hit the op-limit.

Chapter 3 - Miscellaneous things about variables


You don't have to know this information but it may help you.

 Lowest integer you use is -2147483648

 Highest integer you can use 2147483647 (or 2^32-1)

 These limits are in place because Warcraft III uses 32-bit signed integers

Section 2 - Trigger Editor


This section is all about the Trigger Editor.

Chapter 1 - Dealing with Local Variables


These are not necessary to learn as a GUIers but they are very useful.


Local variables are versatile, and they are local to the function instance. This means that they can help you achieve MUI.


They are only usable through custom scripts, but you can use them in GUI using a method called shadowing (described in the next section).


Remember to null all local handles at the end of the function.


For an example on how to make a local variable, see the example below. They must be declared at the top of the function/actions.

-------- First you declare the keyword local then the name of the variable type then the name you want to assign the variable type --------

-------- local <variable type> <name> --------

-------- Here's what it should look like --------

Custom script: local unit u = GetTriggerUnit()

GetTriggerUnit() refers to (Triggering Unit) in GUI. For a list of the equivalents to GUI event responses, see this list:

List of unit "getter" functions, with the GUI and JASS equivalent.

GUI - JASS equivalent

 (Last created unit) = bj_lastCreatedUnit

 (Last restored unit) = bj_lastLoadedUnit

 (Last replaced unit) = bj_lastReplacedUnit

 (Last Haunted Gold Mine) = bj_lastHauntedGoldMine

 (Picked unit) = GetEnumUnit()

 (Matching unit) = GetFilterUnit()

 (Attacked unit) = GetTriggerUnit()

 (Attacking unit) = GetAttacker()

 (Buying unit) = GetBuyingUnit()

 (Cancelled structure) = GetCancelledStructure()

 (Casting unit) = GetSpellAbilityUnit()

 (Constructing structure) = GetConstructingStructure()

 (Constructed structure) = GetConstructedStructure()

 (Damage source) = GetEventDamageSource()

 (Decaying unit) = GetDecayingUnit()

 (Dying unit) = GetTriggerUnit()

 (Entering unit) = GetTriggerUnit()

 (Hero manipulating item) = GetManipulatingUnit()

 (Killing unit) = GetKillingUnit()

 (Learning Hero) = GetLearningUnit()

 (Leaving unit) = GetLeavingUnit()

 (Leveling Hero) = GetLevelingUnit()

 (Loading unit) = GetLoadedUnit()

 (Ordered unit) = GetOrderedUnit()

 (Ownership-changed unit) = GetChangingUnit()

 (Researching unit) = GetResearchingUnit()

 (Revivable Hero) = GetRevivableUnit()

 (Reviving Hero) = GetRevivingUnit()

 (Selling unit) = GetSellingUnit()

 (Sold unit) = GetSoldUnit()

 (Summoned unit) = GetSummonedUnit()

 (Summoning unit) = GetSummoningUnit()

 (Target unit of issued order) = GetOrderTargetUnit()

 (Target unit of ability being cast) = GetSpellTargetUnit()

 (Targeted unit) = GetEventTargetUnit()

 (Trained unit) = GetTrainedUnit()

 (Transporting unit) = GetTransportUnit()

 (Triggering unit) = GetTriggerUnit()

 (Rally-Point of (.....) as a unit) = GetUnitRallyUnit( replace with one above)

List of item "getter" functions, with the GUI and JASS equivalent.

GUI - JASS equivalent

 (Last created item) = GetLastCreatedItem()

 (Last dropped item) = GetLastRemovedItem()

 (Picked item) = GetEnumItem()

 (Matching item) = GetFilterItem()

 (Item being manipulated) = GetManipulatedItem()

 (Sold Item) = GetSoldItem()

 (Target item of issued order) = GetOrderTargetItem()

 (Target item of ability being cast) = GetSpellTargetItem()
SubChapter 1 - Shadowing Global Variables


Whenever you make a global variable in the editor, it will have the prefix udg_ in JASS or Custom Script.


udg_ stands for user defined global. For example, a variable "TempUnit" would be "udg_TempUnit" in custom script.


Shadowing global variables will allow us to use locals as input for GUI functions.


I will be using a unit variable, called tempUnit.


To shadow, you would declare a local like so:

Custom script: local unit udg_tempUnit = GetTriggerUnit()

instead of like this.

Custom script: local unit u = GetTriggerUnit()

Custom script: set udg_tempUnit = u

The difference is minimal but helpful. When you are making an action, you won't


find the local variables listed as an option. If you choose the global and have


the local variable with the same name, it will allow you to use the local for the action instead.

Dying unit
Events
Unit - A unit Dies

Conditions
((Triggering unit) is A Hero) Equal to True

Actions
Custom script: local unit udg_tempUnit = GetTriggerUnit()

Custom script: local location udg_tempPoint = Location( GetUnitX( udg_tempUnit), GetUnitY( udg_tempUnit))

Wait 10.00 seconds

Hero - Instantly revive tempUnit at tempPoint, Hide revival graphics

Custom script: set udg_tempUnit = null

Custom script: call RemoveLocation( udg_tempPoint)

Custom script: set udg_tempPoint = null

I also showed you how to use a point variable to get a unit's location.


All GUI triggers are eventually converted to JASS. So when you input a global


of the same name as a local variable, the interpreter will choose the local variable instead.


This means you can have things be MUI, even with the wait.

Chapter 2 - Dealing with Waits


First be careful with waits. They can be deadly if not used correctly.


Waits are also very inaccurate. Thanks to ap0calypse for telling me this.


Waits don't have to be "very inaccurate": only very short waits suffer from that problem.


(With a 0.50-second wait, the actual wait-time is about 50% off, but a 60-second wait is only about 0.4% off).


You can actually counter-act the inaccuracy of the wait: a wait will always add time, never subtract.


On average, the time it adds is about 0.25 seconds.


If you need a 1-second wait, you better turn that into a 0.75-second wait (which will be somewhere between 0.90 and 1.1, way more accurate than a 1.0-second wait).


There are a few things you should know. Waits don't make things MUI or non-MUI. That's a common misconception.


Waits also ignore the "waiting for players" dialog, which is the dialog menu that appears when a player is losing connection to a game.


In other words if you have a 30 second wait to respond a unit and a player loses connection for 35 seconds that unit will re-spawn during the dialog.


This can be unfair and taken advantage of. Although rare, it can be manipulated to a player's advantage.


I will show you a couple examples of triggers with waits that are MUI and triggers with waits that are not MUI.


This one waits 10 seconds and damages the target unit again. Still MUI.


The reason it is MUI is because of the local variables.

Untitled Trigger 001
Events
Unit - A unit Begins casting an ability

Conditions
(Ability being cast) Equal to mySpell

Actions
Custom script: local unit u1 = GetTriggerUnit()

Custom script: local unit u2 = GetSpellTargetUnit()

Wait 10.00 seconds

Custom script: set udg_caster = u1

Custom script: set udg_target = u2

Unit - Cause caster to damage target, dealing 500.00 damage of attack type Spells and damage type Normal

Custom script: set u1 = null

Custom script: set u2 = null

This one damages the target unit 10 more times after a 10 second wait. Still MUI.


This is also MUI because of the local variables.

Untitled Trigger 001 Copy
Events
Unit - A unit Begins casting an ability

Conditions
(Ability being cast) Equal to mySpell

Actions
Custom script: local unit u1 = GetTriggerUnit()

Custom script: local unit u2 = GetSpellTargetUnit()

Wait 10.00 seconds

Custom script: set udg_caster = u1

Custom script: set udg_target = u2

For each (Integer tempInt) from 1 to 10, do (Actions)
Loop - Actions
Unit - Cause caster to damage target, dealing 500.00 damage of attack type Spells and damage type Normal

Custom script: set u1 = null

Custom script: set u2 = null

This is also MUI. The reason this is MUI is because triggering unit is a local event response (behaves like a local variable).

MUI
Events
Unit - A unit Dies

Conditions

Actions
Wait 10.00 seconds

Hero - Instantly revive (Triggering unit) at (Center of (Playable map area)), Hide revival graphics

This is not MUI. This is not MUI because entering unit is not treated as a local variable.

nonMUI
Events
Unit - A unit enters No region

Conditions

Actions
Wait 2.00 seconds

Unit - Remove (Entering unit) from the game

Chapter 3 - Trigger Events to be careful with


There are a few events you should be careful with. This normally involves two triggers but it can also happen with just one.


The main reason for being careful is that they are prone to infinite loops.


This can happen rather easily and is often just a simple mistake. Let me show you an example of an infinite loop:

infinite loop trig 1
Events
Unit - A unit Acquires an item

Conditions

Actions
Hero - Drop (Item being manipulated) from (Triggering unit)

infinite loop trig 2
Events
Unit - A unit Loses an item

Conditions

Actions
Hero - Give (Item being manipulated) to (Triggering unit)

The reason why this is an infinite loop is because of what you do with the item. It is not because of the events.


First a unit picks up an item. Then the event fires. Then the first trigger drops the item.


The second trigger will fire, because the unit has "lost" an item. Then it gives the item to the hero. This fires the first trigger's event.


Then it will repeat, getting picked up, dropped, picked up, dropped, and so on. This is a classic example of an infinite loop.


There are a lot of other types, even with multiple triggers. You want to look out for events that have a create/destroy aspect.


For Example: 1st event: Unit begins construction (create). Order unit to cancel construction.


2nd event: Unit cancels construction (destroy). Order unit to begin building again.


There is also the event "A unit dies". If you revive the unit and have another trigger that kills the revived unit after it gets revived.


That can cause an infinite loop. This can happen in single triggers as well:

infinite loop trig 3
Events
Unit - Footman 001 <gen> is damaged

Conditions

Actions
-------- Do actions here --------

Unit - Cause TempUnit to damage (Triggering Unit), dealing 50.00 damage of attack type Spells and damage type Normal

In order to fix this, you should disable the trigger before the action, and then re-enable it after the action. This will prevent the event from firing due to that one action:

infinite loop trig 3
Events
Unit - Footman 001 <gen> is damaged

Conditions

Actions
-------- Do actions here --------

Trigger - Turn off (This trigger)

Unit - Cause TempUnit to damage (Triggering Unit), dealing 50.00 damage of attack type Spells and damage type Normal

Trigger - Turn on (This trigger)

Chapter 4 - Dealing with Loops


There are a few things you should know about things when looping.


Always use your own integer. Yes I know that there are these two lines.

For each (Integer A) from 1 to 10, do (Actions)
Loop - Actions

For each (Integer B) from 1 to 10, do (Actions)
Loop - Actions


But the bad thing about these is that they are slightly slower and can cause bugs when being used.


This bug will occur mostly when you use loops within a loop, or run a trigger that uses that looping integer.


To avoid this problem entirely, you should use your own integer variable for looping instead.

Chapter 5 - Actions that can't be done on Map Initialization


As you may have noticed already some actions do not work on Map Initialization.


For these you could to make a trigger with this event.


Thanks Almia for reminding me about dialogs.

Time - Elapsed game time is 0.00 seconds

These actions can't be created/used on Map Initialization

 Multiboards

 Timers

 Leaderboards

 Quests

 Dialogs

 Anything that is timed. ex pan camera (timed)

 No applying a fade filter over x seconds
These Actions can be done at Map Init but they are prone to de-syncs.

 Getting a Player's slot status
Chapter 6 - When to store something into a variable


This section is about when you should store data in a variable.


Storing things into variables is a way to improve the efficiency of your trigger by reducing the amount of function calls.


It is also easier to edit. For example, if you have a bunch of functions that use (Triggering unit), you can just assign


a variable to (Triggering unit). If you ever want to change it to something else, you just change the variable instead of all the functions.


A variable is usually a faster option. When you use something like (Triggering unit), it has a function call. However, reading a variable


does not have a function call and is much faster. Generally, he more parentheses in a function, the longer it takes to read/write.


examples:

dying unit
Events
Unit - A unit Dies

Conditions

Actions
Set integerArray[(Player number of (Owner of (Killing unit)))] = (integerArray[(Player number of (Owner of (Killing unit)))] + 1)

Game - Display to (All players) the text: (Player + ((String((Player number of (Owner of (Killing unit))))) + ( Has Killed + ((String((Player number of (Owner of (Killing unit))))) + units.))))

Unit - Remove (Triggering unit) from the game

Also notice how hard it is to read.


Here is the efficient trigger. Notice how much easier it is to read and how easy it would be to modify if you ever need to.

dying unit
Events
Unit - A unit Dies

Conditions

Actions
Set tempInt = (Player number of (Owner of (Killing unit)))

Set integerArray[tempInt] = (integerArray[tempInt] + 1)

Game - Display to (All players) the text: (Player + ((String(tempInt)) + ( Has Killed + ((String(tempInt)) + units.))))

Unit - Remove (Triggering unit) from the game

If you use the function only once or twice in your trigger, you don't need to set it to a variable.

Chapter 7 - When to destroy / null a variable


You should remember to destroy handles using the functions below, and null the variable.


They should be destroyed and nulled once you no longer need them. You usually do this at the end of a trigger.


Here is a list of custom scripts used to destroy handles. Place your variables in the parentheses ().


If you are using a global variable, remember to add the prefix udg_. For example:

Custom script: call RemoveLocation(udg_TempLoc)
Main Handle Destroying Functions

 call RemoveLocation() - Location

 call DestroyTimer() - Countdown Timer (Note: always pause the timer before you destroy it.)

 call DestroyLightning() - Lightning

 call DestroyTextTag() - Floating Text

 call DestroyTrigger() - Trigger

 call DestroyForce() - Player group

 call RemoveRect() - Region

 set bj_wantDestroyGroup = true - Unit group Set this before you create the group

 call DestroyGroup() - Unit Group

Here are a few others you may use. Some of them have GUI equivalents.

 call DestroyBoolExpr()

 call DestroyCondition()

 call DestroyDefeatCondition()

 call DestroyEffect()

 call DestroyFilter()

 call DestroyFogModifier()

 call DestroyImage()

 call DestroyItemPool()

 call DestroyLeaderboard()

 call DestroyMultiboard()

 call DestroyQuest()

 call DestroyTimerDialog()

 call DestroyUbersplat()

 call DestroyUnitPool()

 call RemoveDestructable()

 call RemoveItem()

 call RemoveRect()

 call RemoveRegion()

 call RemoveUnit()

 call RemoveWeatherEffect()

 call TriggerRemoveAction()

 call TriggerRemoveCondition()
Handles you should destroy that do not need custom script.

 Special Effects
Handles that should never be destroyed / nulled.

These can cause crashes or make some triggers no longer work. Generally, avoid destroying the default globals. Destroy your own.

 Player group: All Players

 Rect: Entire Map

 Rect: Playable Map Area
Chapter 8 - Preloading


Preloading is a good thing to do for any map. All you do is add all the abilities you use in an ability array.


When you don't preload abilities you can get a lag spike when it is first cast.


Preloading will resolve this issue. Just set it in the array and then


on the event "Time elapsed 0.00 seconds", loop through the ability list you created and add (then remove) the ability to a unit on the map.

Preload abilities
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
-------- Here we set the variables. --------

Set abilities[1] = Acid Bomb

Set abilities[2] = Animate Dead

Set abilities[3] = Attribute Bonus

Set abilities[4] = Avatar

Set abilities[5] = Avatar (Neutral)

-------- Now we will do the actions to preload --------

Set tempPoint = (Center of (Playable map area))

Unit - Create 1 Footman for Player 1 (Red) at tempPoint facing Default building facing degrees

Set tempUnit = (Last created unit)

For each (Integer tempInt) from 1 to 5, do (Actions)
Loop - Actions
Unit - Add abilities[tempInt] to tempUnit

Unit - Remove abilities[tempInt] from tempUnit

Unit - Remove tempUnit from the game

Custom script: call RemoveLocation( udg_tempPoint)

Custom script: set udg_tempPoint = null

Custom script: set udg_tempUnit = null

-------- Here is were we destroy the trigger since it isn't needed anymore. --------

Custom script: call DestroyTrigger( GetTriggeringTrigger())

Chapter 9 - Use of Substrings


Substrings allow you to cut down strings, getting certain parts of them.


First let me describe a little how to work with substrings.


This is how you make a "-give x" command.

substrings
Events
Player - Player 1 (Red) types a chat message containing -give as A substring

Player - Player 2 (Blue) types a chat message containing -give as A substring

Player - Player 3 (Teal) types a chat message containing -give as A substring

Player - Player 4 (Purple) types a chat message containing -give as A substring

Player - Player 5 (Yellow) types a chat message containing -give as A substring

Player - Player 6 (Orange) types a chat message containing -give as A substring

Player - Player 7 (Green) types a chat message containing -give as A substring

Player - Player 8 (Pink) types a chat message containing -give as A substring

Conditions

Actions
Set tempPlayer = (Triggering player)

Set tempString = (Entered chat string)

Set subStr = (Substring(tempString, 7, 7))

Set subStrMoney = (Substring(tempString, 9, (Length of tempString)))

Player - Set tempPlayer Current gold to ((Player 1 (Red) Current gold) - tempMoney)

Custom script: set udg_givePlayer = Player( S2I( udg_subStr - 1) )

Player - Add (Integer(subStrMoney)) to givePlayer Current gold

Set tempString = <Empty String>

Set subStr = <Empty String>

Set subStrMoney = <Empty String>

Custom script: set udg_tempPlayer = null

Custom script: set udg_givePlayer = null


Take a look at the events. I have it set so each player types in "-give x xx". ( x=player number, xx= money)


I also have each one listed as a substring. This basically means that they'll have it as part of their entry. If you put "exact match", then it will expect


the player to type exactly "-give" without anything afterward. Now I will explain how substrings work.


You need to put in the string "tempString" in there first for both of the subStrings I have there.


For the first integer, I put 7. You just count the letters in -give. Count the hyphen and add a space. That makes 7.


That is your starting point for the substring. We first want the number of the player.


Then we do the same for the number afterward, but our numbers have changed so we need to account for this.


We count like before. Count the "-give " including the space, and then the slot where the player number is, and then the space afterward. You should get 9.


That will go all the way to the end of the string (tempString).


Then you can just subtract the gold from the triggering player and give it to the other player.


The custom script translates the player number into the player. Then you add the gold to the player specified.


Finally, null the variables and you're done.

Chapter 10 - GetLocalPlayer


Some of you may have seen some people create effects or other things specific for one player.


A lot of times, this is for showing each person their own multiboard.


GetLocalPlayer() is the function that allows for that. It is powerful, but dangerous. It can cause de-syncs if used incorrectly.


De-syncs are when one or more users get disconnected from the game.


De-syncs can only happen in multiplayer (LAN or battle.net).


Here is a short example of how GetLocalPlayer() can be used.


This only shows to one player like it's supposed to and doesn't cause de-syncs.

Weather effects trig
Events
Player - Player 1 (Red) types a chat message containing -weather as A substring

Player - Player 2 (Blue) types a chat message containing -weather as A substring

Player - Player 3 (Teal) types a chat message containing -weather as A substring

Player - Player 4 (Purple) types a chat message containing -weather as A substring

Player - Player 5 (Yellow) types a chat message containing -weather as A substring

Player - Player 6 (Orange) types a chat message containing -weather as A substring

Player - Player 7 (Green) types a chat message containing -weather as A substring

Player - Player 8 (Pink) types a chat message containing -weather as A substring

Conditions

Actions
Custom script: local integer udg_tempInt

Set tempInt = ((Player number of (Triggering player)) - 1)

Set tempString = (Entered chat string)

Set subStr = (Substring(tempString, 10, (Length of tempString)))

Set tempRegion = (Entire map)

Environment - Remove weatherEff[tempInt]

Custom script: if GetLocalPlayer() == Player( udg_tempInt) then

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
subStr Equal to snow

Then - Actions
Environment - Create at tempRegion the weather effect Northrend Snow (Heavy)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
subStr Equal to rain

Then - Actions
Environment - Create at tempRegion the weather effect Ashenvale Rain (Heavy)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
subStr Equal to moonlight

Then - Actions
Environment - Create at tempRegion the weather effect Rays Of Moonlight

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
subStr Equal to wind

Then - Actions
Environment - Create at tempRegion the weather effect Outland Wind (Heavy)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
subStr Equal to random

Then - Actions
Custom script: set udg_tempInt = GetRandomInt( 1, 4)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
tempInt Equal to 1

Then - Actions
Environment - Create at tempRegion the weather effect Northrend Snow (Heavy)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
tempInt Equal to 2

Then - Actions
Environment - Create at tempRegion the weather effect Ashenvale Rain (Heavy)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
tempInt Equal to 3

Then - Actions
Environment - Create at tempRegion the weather effect Rays Of Moonlight

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
tempInt Equal to 4

Then - Actions
Environment - Create at tempRegion the weather effect Outland Wind (Heavy)

Else - Actions

Custom script: set udg_tempInt = 0

Else - Actions

Set weatherEff[tempInt] = (Last created weather effect)

Custom script: endif

Environment - Turn weatherEff[tempInt] On

Set tempString = <Empty String>

Set subStr = <Empty String>

Set tempRegion = No region


Just note that GetLocalPlayer() is a pretty extensive subject. If you create or destroy an agent in it, it will de-syncs (weather effects are not agents). There are some other rules too. Before using it, you should look at a tutorial. I have a link to a tutorial, made by Chaosy, at the end.

Chapter 11 - Dealing with HashTables


Hashtables are very powerful and you should definitely learn how to use them. They can help you achieve MUI.


Hashtables are basically two dimensional arrays that can store various variable types.


They have a "parent" key and a "child" key. Kind of like array 1 and array 2.


First you save a value. In this case the value is 1. Then you choose were to store it.


In this case i store it in parent key 0 and then in child key 0.


Then the action after that is how you load it. First you need a temp integer to store the loaded value in.


Then you load the value by using the parent and child keys.

Hashtable - Save 1 as 0 of 0 in Hash

Set tempInt = (Load 0 of 0 from Hash)
You can even use it to save other things for a specific unit.


Let's save a unit's health, for instance. Just remember health is a real value.


You may be wondering what the "Key of (Picked Unit)" is. This is their handle ID.


A handle ID is a specific value given to every handle on the map, whether it be a rect, unit, item, etc.


So "Key of (Picked Unit)" is an integer that points to a certain unit on the map.

Hashtable - Save (Life of (Triggering unit)) as 0 of (Key (Picked unit)) in Hash


There are some hashtable actions you should not click on--they'll crash the editor.

Hashtable actions you should not click on.

 Hashtable - Save Widget

 Hashtable - Save Ability

 Hashtable - Save Eventid

 Hashtable - Save Region

 Hashtable - Save Itempool

 Hashtable - Save Multiboarditem

 Hashtable - Save Trackable
Those are the basics of hashtables. If you want to learn more, see the tutorial linked at the end.

Chapter 12 - How to Index


Indexing is another powerful method. It is faster than hashtables, and it is another option to make something MUI.


In this tutorial, I will give exact names. I normally shorten them. I'll tell you how later.


Let's make a simple spell that damages units every second for 10 seconds, and that length will increase 5 seconds per level.


For damage we will make it do 200 for level 1 and 75 more for each level after that.


First let's think of a spell name. I am horrible with naming things so I will just call it "Nova".

Here is what you need to index.

 An integer called maxIndexNova

 A unit array called casterUnitsIndexNova

 A unit array called targetUnitsIndexNova

 An integer array called timeLeftNova

 A real called damageNova

 An integer called durationNova // used for the amount of base damage

 A real called damageIncreaseNova // used for the amount the damage increases per lvl

 An integer called durationIncreaseNova //used to track the damage to be dealt

 An integer array called spellLvlNova

 A real array damageToDealNova // this is the amount of damage that will be dealt
As you may have noticed I put the name of the spell in there. This is to make it so that you know what spell these are used for.

Map init
Events
Map initialization

Conditions

Actions
-------- This is just to make it easy for you to change the damage and duration --------

-------- Also the initial duration of the spell and the increasing duration per lvl --------

Set damageNova = 200.00

Set damageIncreaseNova = 75.00

Set durationNova = 10

Set durationIncreaseNova = 5

Nova spell Casting
Events
Unit - A unit Begins casting an ability

Conditions
(Ability being cast) Equal to Nova

Actions
-------- --------

-------- Here we increase the max index --------

-------- --------

Set maxIndexNova = (maxIndexNova + 1)

-------- --------

-------- Here we set the caster unit in the array --------

-------- --------

Set casterUnitsIndexNova[maxIndexNova] = (Triggering unit)

-------- --------

-------- Here we set the target unit in the array --------

-------- --------

Set targetUnitsIndexNova[maxIndexNova] = (Target unit of ability being cast)

-------- --------

-------- Here we set the level of the spell that was used --------

-------- --------

Set spellLvlNova[maxIndexNova] = (Level of Nova for casterUnitsIndexNova[maxIndexNova])

-------- --------

-------- Here we set the damage that we will do --------

-------- We use a formula damageToDeal = ( base damage + ( increasing damage x spell lvl)) --------

-------- --------

Set damageToDealNova[maxIndexNova] = (damageNova + (damageIncreaseNova x (Real(spellLvlNova[maxIndexNova]))))

-------- --------

-------- Here we set the duration of the spell. --------

-------- We use a formula time Left = ( base duration + ( increasing duration x spell lvl)) --------

-------- --------

Set timeLeftNova[maxIndexNova] = (durationNova + (durationIncreaseNova x spellLvlNova[maxIndexNova]))

-------- --------

-------- Then we check if the other trigger that deals the damage is running --------

-------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
maxIndexNova Equal to 1

Then - Actions
Trigger - Turn on Nova spell Damaging <gen>

Else - Actions

Nova spell Damaging
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
-------- --------

-------- Here we deal the damage and decrease the time --------

-------- --------

For each (Integer tempInt) from 1 to maxIndexNova, do (Actions)
Loop - Actions
-------- --------

-------- Here we damage the unit --------

-------- --------

Unit - Cause casterUnitsIndexNova[tempInt] to damage targetUnitsIndexNova[tempInt], dealing damageToDealNova[tempInt] damage of attack type Chaos and damage type Normal

-------- --------

-------- Here we decrease the time Left for that unit --------

-------- --------

Set timeLeftNova[tempInt] = (timeLeftNova[tempInt] - 1)

-------- --------

-------- Then finally we check to see if the time has ended for that unit. --------

-------- If it did run out then we remove the unit from the array and lower the indexes. --------

-------- Also check if the target is dead since there is no reason to damage a dead unit --------

-------- We also null the handle variables --------

-------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
timeLeftNova[tempInt] Equal to 0

(Life of targetUnitsIndexNova[tempInt]) Less than or equal to 0.41

Then - Actions
-------- --------

-------- Here we set the caster unit in the array --------

-------- --------

Set casterUnitsIndexNova[tempInt] = casterUnitsIndexNova[maxIndexNova]

-------- null the last index --------

Set casterUnitsIndexNova[maxIndexNova] = No unit

-------- --------

-------- Here we set the target unit in the array --------

-------- --------

Set targetUnitsIndexNova[tempInt] = targetUnitsIndexNova[maxIndexNova]

-------- null the last index --------

Set targetUnitsIndexNova[maxIndexNova] = No unit

-------- --------

-------- Here we set the level of the spell of the last index to that of this index --------

-------- --------

Set spellLvlNova[tempInt] = spellLvlNova[maxIndexNova]

-------- --------

-------- Here we set the damage of the last index to that of the current index --------

-------- --------

Set damageToDealNova[tempInt] = damageToDealNova[maxIndexNova]

-------- --------

-------- --------

-------- Here we decrease the max index --------

-------- --------

Set maxIndexNova = (maxIndexNova - 1)

-------- --------

-------- --------

-------- Here we decrease the current index to allow for the index we moved to be run rather than skipped this time --------

-------- --------

Set tempInt = (tempInt - 1)

Else - Actions

-------- --------

-------- Here we check if there are any more indexes to deal damage to if there aren't we shut off this trigger --------

-------- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
maxIndexNova Equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

What i would've called the variables to keep them short

 maxIndexNova //this one is fine

 cUIndexNova // caster unit

 tUIndexNova // target unit

 timeLeftNova // this is fine

 damageNova // this is ok

 durNova // duration

 damageIncNova // damage increase

 durIncNova //duration increase

 spellLvlNova // this one is fine

 damageToDealNova // this one is ok

Map init
Events
Map initialization

Conditions

Actions
Set damageNova = 200.00

Set damageIncreaseNova = 75.00

Set durationNova = 10

Set durationIncreaseNova = 5

Nova spell Casting
Events
Unit - A unit Begins casting an ability

Conditions
(Ability being cast) Equal to Nova

Actions
Set maxIndexNova = (maxIndexNova + 1)

Set casterUnitsIndexNova[maxIndexNova] = (Triggering unit)

Set targetUnitsIndexNova[maxIndexNova] = (Target unit of ability being cast)

Set spellLvlNova[maxIndexNova] = (Level of Nova for casterUnitsIndexNova[maxIndexNova])

Set damageToDealNova[maxIndexNova] = (damageNova + (damageIncreaseNova x (Real(spellLvlNova[maxIndexNova]))))

Set timeLeftNova[maxIndexNova] = (durationNova + (durationIncreaseNova x spellLvlNova[maxIndexNova]))

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
maxIndexNova Equal to 1

Then - Actions
Trigger - Turn on Nova spell Damaging <gen>

Else - Actions

Nova spell Damaging
Events
Time - Every 1.00 seconds of game time

Conditions

Actions
For each (Integer tempInt) from 1 to maxIndexNova, do (Actions)
Loop - Actions
Unit - Cause casterUnitsIndexNova[tempInt] to damage targetUnitsIndexNova[tempInt], dealing damageToDealNova[tempInt] damage of attack type Chaos and damage type Normal

Set timeLeftNova[tempInt] = (timeLeftNova[tempInt] - 1)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
timeLeftNova[tempInt] Equal to 0

(Life of targetUnitsIndexNova[tempInt]) Less than or equal to 0.41

Then - Actions
Set casterUnitsIndexNova[tempInt] = casterUnitsIndexNova[maxIndexNova]

Set casterUnitsIndexNova[maxIndexNova] = No unit

Set targetUnitsIndexNova[tempInt] = targetUnitsIndexNova[maxIndexNova]

Set targetUnitsIndexNova[maxIndexNova] = No unit

Set spellLvlNova[tempInt] = spellLvlNova[maxIndexNova]

Set damageToDealNova[tempInt] = damageToDealNova[maxIndexNova]

Set maxIndexNova = (maxIndexNova - 1)

Set tempInt = (tempInt - 1)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
maxIndexNova Equal to 0

Then - Actions
Trigger - Turn off (This trigger)

Else - Actions

Chapter 13 - The Do Nothing Action


This section is about the "Do Nothing" action. There are only a few things about it.


This action is completely useless. Never use it. The reason for this is it does exactly what it says.


It is just meant as a filler for GUI. It is inefficient.

Chapter 14 - The "Or" and "And" Conditions


Here are a couple of examples of when and how to use these conditions.


This one uses the "Or" condition. It fires if either of these conditions are true when the event fires.

Conditions
Or - Any (Conditions) are true
Conditions
((Triggering unit) is A structure) Equal to True

((Triggering unit) has (Last created item)) Equal to True

This one uses the "And" condition. It fires when both of these conditions are true when the event fires.

Conditions
((Triggering unit) is A structure) Equal to True

((Triggering unit) has (Last created item)) Equal to True

This one fires when the first condition and either of the other two are true when the event fires.

Conditions
(Ability being cast) Equal to Animate Dead

Or - Any (Conditions) are true
Conditions
((Triggering unit) is A structure) Equal to True

((Triggering unit) has (Last created item)) Equal to True

This one fires when the first condition and one of the options in the "Or" conditions are true when the event fires.


The "And" condition is only needed when used for 2 or more conditions inside an "Or" condition.

Conditions
(Ability being cast) Equal to Animate Dead

Or - Any (Conditions) are true
Conditions
((Triggering unit) is A structure) Equal to True

((Triggering unit) has (Last created item)) Equal to True

And - All (Conditions) are true
Conditions
(Ability being cast) Equal to Animate Dead

(Destructible-type of (Last created destructible)) Equal to Summer Tree Wall

Chapter 15 - Using Nested Loops


This will be about nested loops. Nested basically means put inside.

nesting loops
Events
Time - Elapsed game time is 5.00 seconds

Conditions

Actions
For each (Integer index) from 1 to 10, do (Actions)
Loop - Actions
Game - Display to (All players) the text: (index = + (String(index)))

For each (Integer tempInt) from 1 to 10, do (Actions)
Loop - Actions
Game - Display to (All players) the text: (tempInt = + (String(tempInt)))

These can be very useful. If you make it like this u can see that it displays the integers on screen as follows.

 index = 1

 tempInt = 1

 tempInt = 2

 tempInt = 3

 tempInt = 4

 tempInt = 5

 tempInt = 6

 tempInt = 7

 tempInt = 8

 tempInt = 9

 tempInt = 10

 index = 2

 tempInt = 1

 tempInt = 2

 tempInt = 3

 tempInt = 4

 tempInt = 5

 tempInt = 6

 tempInt = 7

 tempInt = 8

 tempInt = 9

 tempInt = 10

 index = 3

 All the way until index = 10, then it displays tempInt from 1 to 10 and it ends.
The reason this can be useful is for storing data very easily rather than typing each one of these out.


For a better understanding of how these can be useful read the next chapter.

Chapter 16 - Multi-Dimensional Arrays


These are very simple to understand despite their name.

integerArray[1]

The one is the index. To make this multi-dimensional you change the one to a math equation.

Set integerArray[((Player number of (Triggering player)) * 20 + 1)] = 1

20 is the width of the variable so if u need to store 20 different things use that as the width. The 1 tells were exactly it goes in that width area.


Note: if u change the width it will overwrite other things


Another useful way is to set things in one array for all players. Lets set the numbers 1 to 10 in a single array for each person.


We can do this by using a nested loop like in the chapter above this.

For each (Integer index) from 1 to 10, do (Actions)
Loop - Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
Set playerInteger[(((Player number of (Picked player)) x 10) + index)] = index

So if we display playerInteger[ 15] it will display 5.


These types of things can be used for spawning creeps, for keeping track of regions, and many other things.

data[((p x 7) + i)
Were p is the players integer, 7 is the number of regions, and i is what determines the index.

Chapter 17 - If Then Else's and Nesting Them


ITE is an abbreviation for "if - then - else".


I will show you an example of an efficient ITE and an inefficient ITE. Basically this is a "unit enters region" event in which the player loses a life. It's for a TD.


Here is a bad way to have nested ITE’s.

Nested ITE’s
Events
Unit - A unit enters Region 2 <gen>

Conditions
(Owner of (Triggering unit)) Equal to Neutral Hostile

(Triggering unit) Equal to bossUnit1

(Triggering unit) Equal to bossUnit2

(Triggering unit) Equal to bossUnit3

Actions
Set tempUnit = (Triggering unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering unit) Equal to bossUnit1

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
waveCounter Equal to 30

Then - Actions
-------- remove life --------

Else - Actions

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering unit) Equal to bossUnit2

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
waveCounter Equal to 30

Then - Actions
-------- remove life --------

Else - Actions

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering unit) Equal to bossUnit3

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
waveCounter Equal to 30

Then - Actions
-------- remove life --------

Else - Actions

Else - Actions

There are 2 options to fix this one involves nesting the ITE’s the other involves not nesting.


The first one in this case is still a bad choice but it will show you some different ways on how to do it.


The reason it is still bad is that it will only take out health when the other condition is true which wouldn't be good.

Nested ITE’s
Events
Unit - A unit enters Region 2 <gen>

Conditions
(Owner of (Triggering unit)) Equal to Neutral Hostile

tempUnit Equal to bossUnit1

tempUnit Equal to bossUnit2

tempUnit Equal to bossUnit3

Actions
Set tempUnit = (Triggering unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
waveCounter Equal to 30

Then - Actions
-------- remove life --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering unit) Equal to bossUnit1

Then - Actions

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering unit) Equal to bossUnit2

Then - Actions

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Triggering unit) Equal to bossUnit3

Then - Actions

Else - Actions

Else - Actions

This one on the other hand is probably your best option for nesting and efficiency.


Note how they are nested in the else block and that i was able to get rid of one of the ITE’s, when you do it this way it is faster.

Nested ITE’s
Events
Unit - A unit enters Region 2 <gen>

Conditions
(Owner of (Triggering unit)) Equal to Neutral Hostile

(Triggering unit) Equal to bossUnit1

(Triggering unit) Equal to bossUnit2

(Triggering unit) Equal to bossUnit3

Actions
Set tempUnit = (Triggering unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
tempUnit Equal to bossUnit1

Then - Actions

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
tempUnit Equal to bossUnit2

Then - Actions

Else - Actions
-------- bossUnit3 actions go here --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
waveCounter Equal to 30

Then - Actions
-------- remove life --------

Else - Actions

These are only a few ways on how to use nested ITE’s.


There are many combinations you can use and many you shouldn't use.


The easiest way to find out what's the best way is to make it up and see how you can shorten the amount of ITE's you have.

Chapter 18 - Skip Remaining Actions


This is a nice little action here. It does exactly as the meaning of the name.


These can be used to stop a trigger from running if a condition is met.


For example: I want to create 1 unit every 2 seconds 100 times, but if my unit dies i want to stop creating units.

skip remaining actions
Events
Time - Elapsed game time is 5.00 seconds

Conditions

Actions
Custom script: local integer udg_tempInt = 1

Custom script: local location udg_tempPoint = Location( GetRectCenterX( gg_rct_Region_1), GetRectCenterY( gg_rct_Region_1))

For each (Integer tempInt) from 1 to 100, do (Actions)
Loop - Actions
Game - Display to (All players) the text: 1

Unit - Create 1 Archmage for Neutral Hostile at tempPoint facing Default building facing degrees

Game - Display to (All players) the text: 2

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Life of Archmage 0003 <gen>) Less than 0.41

Then - Actions
Skip remaining actions

Else - Actions

Wait 2.00 seconds

Custom script: call RemoveLocation( udg_tempPoint)

Custom script: set udg_tempPoint = null

Game - Display to (All players) the text: 3

This will do exactly what I want. It will run one extra time after I die and then it will stop.

Chapter 19 - Floating Texts


Floating texts are text that "float" in-game on the map, similar to bounty and experience text.


They are often used to show damages. You can also use it to show players' quests or the names of people / buildings.


There are some bad things about floating text though. You can only have 100 floating texts on the map at once.


There is also one exception to this rule, if you create them using GetLocalPlayer() you can make 100 for each player.


There are a few actions you should learn with floating texts.


First is the action to destroy one. When you are done with the text you should destroy it or you will quickly hit the maximum number of floating text.


Next, and one of the most important, is Floating text - permanent / expired. You need to set this to disable permanence if you want to destroy the text using the lifespan actions.


Next is the "set velocity". This allows you to move the text in a direction of your choosing. You see this in damage systems.


Next is the "Lifespan". This lets you set a specific lifespan for the floating text. You see this in damage systems as well.


Finally we have the fading age. As its name says, it sets the age at which the text will fade away, making it invisible.


To use this, you also need the lifespan, as that determines when it becomes invisible (it is also destroyed this way).

Chapter 20 - Timers


Timers also known as countdown timers in GUI are another useful tool. Note: There are timers and countdown timers in JASS.


Countdown timers are a lot more accurate than using waits. This reason alone makes them useful.


If you make a spell and trigger a periodic effect with waits, it will often look very messy or choppy.


On the other hand, if you use countdown timers then the effects will look the same every time.


You can use these to show when a unit will be revived, when an event you make will end / start, and plenty of other things.

Chapter 21 - The DRY-principle


This chapter will explain the DRY-principle. Thanks to ap0calypse again.


First you should know that DRY stands for "Don't Repeat Yourself"-Principle.


I see many new people who copy/paste every trigger 12 times for each player.


You can always use 1 trigger with 12 events that works just the same as 12 separate triggers.


Lowering the amount of triggers you use by using the DRY method is a way to improve your map's efficiency and reduce the size of the map.


Take advantage of loops and multiple events. The difference is the efficiency and the ease of use and editing.


If you look at my Substrings example. That is an example of using the DRY-principle.

Chapter 22 - Making your own Functions


This will be about making your own functions. Thanks to Kobas for this example and how to use it.


Making function in GUI is done by splitting trigger Actions into 2 or more functions and use them later.

Test
Events

Conditions

Actions
Custom script: endfunction

Custom script: function ShowTestMsg takes nothing returns nothing

Game - Display to (All players) the text: -Test-

RunTest
Events
Unit - A unit Dies

Conditions

Actions
Custom script: call ShowTestMsg()

Note: The trigger with "call ..." must be placed below trigger with the function.

Chapter 23 - How to Color Text


This will help you make colored text to display through a message, multiboard, floating text, and so on.


Applying color to text is very easy. Yes, you can change them using the RBG values in the change color actions.


However, I like to do it through strings. You make strings and set the values of the color using their hexadecimal values.


You can find the numbers by using a RGB color to hexadecimal color chart.


To color text is very easy. Yes you can change them using the RBG values in the change color action.


Just note that when you do this, you place the alpha value first.


The alpha value represents the transparency. In most cases, it won't have any effect.


This way lets you change any letter in a string. You can also use these strings to color letters on your map header or quest guide.

Colors GUI
Events
Map initialization

Conditions

Actions
-------- red --------

Set playerColors[1] = |c00ff0303

-------- blue --------

Set playerColors[2] = |c000042ff

-------- teal --------

Set playerColors[3] = |c001ce6b9

-------- purple --------

Set playerColors[4] = |c00540081

-------- yellow --------

Set playerColors[5] = |c00fffc01

-------- orange --------

Set playerColors[6] = |c00feba0e

-------- green --------

Set playerColors[7] = |c0020c000

-------- pink --------

Set playerColors[8] = |cffe55bb0

-------- gray --------

Set playerColors[9] = |cffc0c0c0

-------- light blue --------

Set playerColors[10] = |cffb0e2ff

-------- dark green --------

Set playerColors[11] = |cff006400

-------- brown --------

Set playerColors[12] = |cff8b4513

-------- gold --------

Set playerColors[13] = |cffffcc00

-------- this is how to end the color block --------

Set colorEnd = |r


This will display the colors from red to brown in a message.

Colors GUI Display
Events
Player - Player 1 (Red) types a chat message containing Hello as An exact match

Conditions

Actions
For each (Integer tempInt) from 1 to 12, do (Actions)
Loop - Actions
Game - Display to (All players) the text: (playerColors[tempInt] + (Hello + colorEnd))

It can be more complex to display a message of a player that lost something. Let's say that we want to display a message to the player that lost a building in their color.


We can make a simple trigger like this to display the color of that player and tell him / her that the building was destroyed.

Colors GUI Building Death
Events
Unit - A unit Dies

Conditions
((Triggering unit) is A structure) Equal to True

Actions
Set tempInt = (Player number of (Owner of (Triggering unit)))

Game - Display to (All players) the text: (playerColors[tempInt] + (Hello + colorEnd))

You can also make many other things. You can make something like this.


In other words the number is red. Number 2 is blue and so on.

Colors GUI 1
Events
Player - Player 1 (Red) types a chat message containing Hello as An exact match

Conditions

Actions
For each (Integer tempInt) from 1 to 12, do (Actions)
Loop - Actions
Set tempString = (tempString + (playerColors[tempInt] + ((String(tempInt)) + (colorEnd + ))))

Game - Display to (All players) the text: tempString

Chapter 24 - How to Move Unit without Disrupting Orders ( SetUnitX/Y)


This chapter will show you how to move units without disrupting there orders.


It is actually quite simple but may look confusing.


To move units without disrupting there orders instead of using the GUI action move unit instantly you use custom script and use call SetUnitX/Y( unit, x/y).

move units
Events
Player - Player 1 (Red) skips a cinematic sequence

Conditions

Actions
Set tempPoint = (Center of (Playable map area))

Set tempUnit = Peasant 0007 <gen>

Set tempX = (X of tempPoint)

Set tempY = (Y of tempPoint)

Custom script: call SetUnitX( udg_tempUnit, udg_tempX)

Custom script: call SetUnitY( udg_tempUnit, udg_tempY)

Custom script: call RemoveLocation( udg_tempPoint)

This is what you see done by most people when they use GUI. But i have an option that is better than this efficiency wise.


Notice the location that i had to create when i went to move the unit. Creating a location takes a lot of processing power. Not just creating though, as now you have to clean that location.


Now i will show you the way you should do it. It requires slightly more custom script knowledge but its a simple extra step.

move units
Events
Player - Player 1 (Red) skips a cinematic sequence

Conditions

Actions
Set tempUnit = Peasant 0007 <gen>

Custom script: call SetUnitX( udg_tempUnit, GetUnitX( udg_tempUnit))

Custom script: call SetUnitY( udg_tempUnit, GetUnitY( udg_tempUnit))

Now you can see the big difference. It's a simple piece of extra code. All you need is GetUnitX( unit) and GetUnitY( unit).


This will now do what the above trigger did but more efficiently and with less actions.


 Last edited by a moderator: Mar 9, 2014

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Section 3 - Gameplay Constants

In this I will cover easy ways to find out what is the cause of your trigger not working.

Chapter 1 - Xp from Killing Enemy Heroes

These four factors determines the experience you get from killing Enemy Heroes

These are the preset values in Gameplay constants

 Hero XP Gained - Hero, Constant factor: 100

 Hero XP Gained - Hero, Level factor: 0

 Hero XP Gained - Hero, Previous value factor: 1

 Hero XP Gained - Hero, Table: 100, 120, 160, 220, 300
The hero Table are the preset numbers which means a lvl 1 unit u gain 100, a lvl 2 u get 120, and so on.

You may be asking what happens at lvl 6. Well that's were the other numbers come in.

Here is the formula for Experience gained from lvl 6 onwards.

Experience gained = Previous value * Previous value factor + Level * Level factor + Constant factor

So for a lvl 6 unit u put in the values that are called for.

 Previous value = 300

 Previous value factor = 1

 Level = 6

 Level factor = 0

 Constant factor = 100
So for lvl 6 u gain 400 Xp

Chapter 2 - Xp from Killing Enemy Units

These four factors determines the experience you get from killing Enemy Units

These are the preset values in Gameplay constants

 Hero XP Gained - Normal, Constant factor: 5

 Hero XP Gained - Normal, Level factor: 5

 Hero XP Gained - Normal, Previous value factor: 1

 Hero XP Gained - Normal, Table: 25
As you probably noticed this is in the same format.

The formula is also the exact same so I’ll let you do that.

Experience gained = Previous value * Previous value factor + Level * Level factor + Constant factor

Chapter 3 - Xp from Killing Creeps

This factor determines the experience you get from killing Creeps.

These are the preset values in Gameplay constants

 Hero XP Gained - Creep reduction table: 80, 70, 60, 50, 0

It is different since you only get one thing.

When you kill a creep you don't get the full xp for that unit.

Instead this table is based on percentage.

Here's how the xp is determined.

 lvl 1 you get 80 percent

 lvl 2 you get 70 percent

 lvl 3 you get 60 percent

 lvl 4 you get 50 percent

 lvl 5 you get 00 percent
For a lvl 6 or higher you get the percentage of the last percent in the table.

So lvl 6 and higher gets 00 percent.

Chapter 4 - Xp Till Your Hero Lvls

These four factors determines the experience you get from killing Enemy Units

These are the preset values in Gameplay constants

 Hero XP Required, Constant factor: 0

 Hero XP Required, Level factor: 100

 Hero XP Required, Previous value factor: 1

 Hero XP Required, Table: 200
Pretty sure i don't have to explain much on this one again.

Just use this formula

Experience gained = Previous value * Previous value factor + Level * Level factor + Constant factor

Section 4 - Object Editor

In this section i will show you how to do some things in the Object Editor.

Chapter 1 - Channel Spell

The channel spell, which is a hero ability, is probably the most useful ability in wc3. It is the only spell that can change its base order id.

Base order id is the id used by wc3's game engine when you cast a spell.

Base ability is the ability you base a spell off of.

For example if you have two abilities that you trigger based off of the same ability then only one of them will ever be cast.(only counts if both abilities are on the same unit)

The reason for this is the base order id of these spells remains the same even if you make two different abilities with one base ability.

The reason why channel is such a useful spell is because it is the only spell whose base order id that you can change.

Since this spell has a base order id that you can change it makes it so you can put many abilities based off of this spell onto that same unit.

There's two other important things you need to know about when using this spell.

First is that this spell is preset to disable other abilities when it is casted. This can be easily changed in the Object Editor.

This field is called Level 1 - Data - Disable Other Abilities (Ncl5)

The other important thing you need to know about this ability is it is initially set to invisible. Which means that you will not be able to see the ability.

To change this you have a few options.

Here are the options

 Visible

 Targeting Image

 Physical Spell

 Universal Spell

 Unique Cast
All you have to do is select the visible option. You can click multiple options at once for this field.

This field is called Level 1 - Data - Options (Ncl3)

Chapter 2 - How the damage works

Well as you probably know there is a minimum damage and a maximum damage. What you may not know is were these values come from.

Here is were those values come from

 Combat - Attack 1/2 - Damage Base

 Combat - Attack 1/2 - Damage Number of Dice

 Combat - Attack 1/2 - Damage Sides per Die
These three values determine how much minimum and maximum damage a unit does.

Damage is determined by a dice roll that is why there is dice and sides per dice.

First ill explain the formula for the minimum damage.

This is the formula for the minimum damage.

 Minimum Damage = Damage Base + Damage Number of Dice

I said before that damage is determined by a dice roll i will now explain what i meant.

If you take a pair of dice lets say 2 dice each has 6 sides with numbers 1 through 6.

The lowest numbers you can get is two 1s so u add that to the damage base.

This is how the minimum damage is calculated and how the chances of damage are determined.

Here is the formula for the maximum damage.

 Maximum damage = Damage Base + Damage Number of Dice x Damage Sides per Die

As i said above about the dice roll. The lowest numbers you could get when you roll dice added to the Damage Base were the minimum damage dealt.

That means that the highest possible numbers when you roll the two dice plus the Damage Base is the maximum damage dealt.

This is were those two formulas come from.

Section 5 - How to find what isn't working in your trigger

In this i will cover easy ways to find out what is the cause of your trigger not working.

Chapter 1 - Debugging

Debugging a trigger is probably the fastest way to see were a trigger isn't working at.

Here is how it works. Create a lot of debug messages while increasing the number.

Game - Display to (All players) the text: 1

This will show you which action the trigger stops at if you place them after every action.

Preload abilities
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
-------- Here we set the variables. --------

Game - Display to (All players) the text: 1

Set abilities[1] = Acid Bomb

Game - Display to (All players) the text: 2

Set abilities[2] = Animate Dead

Game - Display to (All players) the text: 3

Set abilities[3] = Attribute Bonus

Game - Display to (All players) the text: 4

Set abilities[4] = Avatar

Game - Display to (All players) the text: 5

Set abilities[5] = Avatar (Neutral)

Game - Display to (All players) the text: 6

-------- Now we will do the actions to preload --------

Set tempPoint = (Center of (Playable map area))

Game - Display to (All players) the text: 7

Unit - Create 1 Footman for Player 1 (Red) at tempPoint facing Default building facing degrees

Game - Display to (All players) the text: 8

Set tempUnit = (Last created unit)

Game - Display to (All players) the text: 9

For each (Integer tempInt) from 1 to 5, do (Actions)
Loop - Actions
Unit - Add abilities[tempInt] to tempUnit

Unit - Remove abilities[tempInt] from tempUnit

Unit - Remove tempUnit from the game

Game - Display to (All players) the text: 10

Custom script: call RemoveLocation( udg_tempPoint)

Game - Display to (All players) the text: 11

Custom script: set udg_tempPoint = null

Game - Display to (All players) the text: 12

Custom script: set udg_tempUnit = null

Game - Display to (All players) the text: 13

-------- Here is were we destroy the trigger since it isn't needed anymore. --------

Custom script: call DestroyTrigger( GetTriggeringTrigger())

Game - Display to (All players) the text: 14

In this i used the brute force approach. Basically i mean i put an action after every action.

For Example lets say the number 5 was the last thing to display in the game.

That means this is the bad line.

[HIDDEN"Hypothetical bad line"]

Set abilities[5] = Avatar (Neutral)

You can do this another way and post message after a couple lines like 5 or 10 lines.

Then you will narrow down what block has the bad line in it.

Then you move the messages into the block of bad code.

This example will show you how to do this. I will assume that the same line was bad in the other example.

Preload abilities
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
-------- Here we set the variables. --------

Game - Display to (All players) the text: 1

Set abilities[1] = Acid Bomb

Set abilities[2] = Animate Dead

Set abilities[3] = Attribute Bonus

Set abilities[4] = Avatar

Set abilities[5] = Avatar (Neutral)

-------- Now we will do the actions to preload --------

Game - Display to (All players) the text: 2

Set tempPoint = (Center of (Playable map area))

Unit - Create 1 Footman for Player 1 (Red) at tempPoint facing Default building facing degrees

Set tempUnit = (Last created unit)

Game - Display to (All players) the text: 3

For each (Integer tempInt) from 1 to 5, do (Actions)
Loop - Actions
Unit - Add abilities[tempInt] to tempUnit

Unit - Remove abilities[tempInt] from tempUnit

Unit - Remove tempUnit from the game

Game - Display to (All players) the text: 4

Custom script: call RemoveLocation( udg_tempPoint)

Custom script: set udg_tempPoint = null

Custom script: set udg_tempUnit = null

-------- Here is were we destroy the trigger since it isn't needed anymore. --------

Game - Display to (All players) the text: 5

Custom script: call DestroyTrigger( GetTriggeringTrigger())

The last integer to be displayed would be 1.

Then you change them around since everything after 2 is fine.

Preload abilities
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
-------- Here we set the variables. --------

Game - Display to (All players) the text: 1

Set abilities[1] = Acid Bomb

Game - Display to (All players) the text: 2

Set abilities[2] = Animate Dead

Game - Display to (All players) the text: 3

Set abilities[3] = Attribute Bonus

Game - Display to (All players) the text: 4

Set abilities[4] = Avatar

Game - Display to (All players) the text: 5

Set abilities[5] = Avatar (Neutral)

Game - Display to (All players) the text: 6

-------- Now we will do the actions to preload --------

Set tempPoint = (Center of (Playable map area))

Unit - Create 1 Footman for Player 1 (Red) at tempPoint facing Default building facing degrees

Set tempUnit = (Last created unit)

For each (Integer tempInt) from 1 to 5, do (Actions)
Loop - Actions
Unit - Add abilities[tempInt] to tempUnit

Unit - Remove abilities[tempInt] from tempUnit

Unit - Remove tempUnit from the game

Custom script: call RemoveLocation( udg_tempPoint)

Custom script: set udg_tempPoint = null

Custom script: set udg_tempUnit = null

-------- Here is were we destroy the trigger since it isn't needed anymore. --------

Custom script: call DestroyTrigger( GetTriggeringTrigger())

Now just like the first way the last number displayed is 5

And here is the bad action again.

Set abilities[5] = Avatar (Neutral)

Section 6 - Other tutorials that can help you improve your GUI skills

https://www.hiveworkshop.com/forums/triggers-scripts-269/things-leak-35124/

https://www.hiveworkshop.com/forums/trigger-gui-editor-tutorials-279/mui-triggers-waits-218354/

https://www.hiveworkshop.com/forums/trigger-gui-editor-tutorials-279/getlocalplayer-225738/

https://www.hiveworkshop.com/forums/trigger-gui-editor-tutorials-279/hashtables-mui-133407/

https://www.hiveworkshop.com/forums...279/mui-spells-using-artificial-waits-223315/

Spells - Channel

https://www.hiveworkshop.com/forums...ials-278/importance-efficiency-coding-120105/

https://www.hiveworkshop.com/forums...9/complete-beginners-guide-hashtables-197381/

https://www.hiveworkshop.com/forums...als-279/basics-trigger-enhancing-spells-7334/

https://www.hiveworkshop.com/forums...279/multi-instancible-gui-spell-making-34393/

https://www.hiveworkshop.com/forums/general-mapping-tutorials-278/debugging-188204/

Description of how a variable is a pointer

Wait time delay examples and times

Integer A vs. udg integer

Final Comments

If you have anything i should add to this please tell me on here or pm me.

I'm hoping that this can be an easy tutorial for any newcomers.


 Last edited: Aug 27, 2013

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Chapter 1:


There are also strings and booleans.


Also, you haven't explained what handles are and why it is important to know the difference.


This chapter basicly has no information in it. You should also explain the difference between handles, agents and widgets.


Chapter 2:


The explanation of why you should set the array size higher than 1 is lacking.


You mention the operation limit, but do not explain what it is.


Dealing with Local Variables:


You should give a list of useful unit getters here. GetTriggerUnit() can be used as a local variable anyway, so it's unlikely people will ever need GetTriggerUnit() with locals.


Also, you should mention that you can set the name of a local variable to udg_temp and then use a global variable named "temp" to access the local directly without needing to overwrite the global every passing time interval again.


Dealing with waits:


This chapter explains absolutely nothing. The example lacks a description that explains why the work and why not.


All you did was showing examples that work and examples that don't work, but people will not be able to tell why.


Dealing with Loops:


You lack an example of how to do that in GUI.


Actions that can't be done at map init:


A lot of actions and getters are missing. There are also actions that work, but are prone to desync in multiplayer games, like Getting the player slot state.


Chapter 5:


Doesn't give an example either.


When to destroy/null a variable:


This is dangerous and bullshit. You can't just say that to people! You should only destroy handles when you don't need them anymore, not "always"!


Nulling a global variable before changing it makes no sense and is a waste of action. Nulling globals is not required either.


There are some handles that should never be destroyed. This is also missing a list of JASS destructors in case there is no GUI equivalent.


Preloading & use of substrings:


The first useful chapters of your tutorial.


GetLocalPlayer:


Misses an explanation of what desyncs are.


It's also a bad example, because it creates handles inside a local block, which might handle IDs to collide. It doesn't instantly desync, but it might cause desyncs in longer games.


Also, it doesn't explain what GetLocalPlayer() actually does and why it works the way it works.


If you link to a seperate tutorial anyway, why even bother including it into this tutorial?


Debugging:


I think this should be in a seperate tutorial, discussing different approaches for different scenarios.


All in all, this tutorial serves no real purpose at its current state. It's a showcase of certain, highly situational examples with no real explanation at all.


People will read it, get confused and then move on, changing nothing.


If there is indeed a need for an "all in one" tutorial like this one, you should definitely mention hashtables and how to use them, as they are very powerful, especially for GUIers which can't use locals without custom code.

#### BunnyAng

#### BunnyAng

 Level 10

JoinedJun 9, 2012Messages824

Resources: 1

#### Resources
Spells and Systems 1

Wow, so this is the tutorial you wanted to show me. This really is useful, in fact, it can be very helpful for people who are new to spell-making. I actually learnt a lot. +rep for the effort.

Edit: Just +rep you yesterday lol gotta spread some.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

remove the custom scripts that is nothing you need to know in GUI

expect local player

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Zwiebelchen said:

 You should also explain the difference between handles, agents and widgets.


Click to expand...

i dont know the exact definitions. Thats y i didnt include them. If u or someone else wants to tell me them ill put them in.

Zwiebelchen said:

 Chapter 2:

The explanation of why you should set the array size higher than 1 is lacking.

You mention the operation limit, but do not explain what it is.


Click to expand...

I elaborated more on it just in case.

All of the other ones should be set to 1. The reason they should be set to one is that it is not needed to initialize them.

It will also increase saving and loading when setting them to 1. It will increase the speed at which the game starts.

If you have to many set to to high of values you will hit the op limit. Which will cause the game to crash or Triggers to fail. this is right in the dealing with array size one.

Zwiebelchen said:

 Actions that can't be done at map init:

A lot of actions and getters are missing. There are also actions that work, but are prone to desync in multiplayer games, like Getting the player slot state.


Click to expand...

This is another thing that i dont know everything about. If you know of more that i missed plz tell me and ill add them.

Zwiebelchen said:

 Dealing with waits:

This chapter explains absolutely nothing. The example lacks a description that explains why the work and why not.

All you did was showing examples that work and examples that don't work, but people will not be able to tell why.


Click to expand...

I'll elaborate more on this.

Zwiebelchen said:

 When to destroy/null a variable:

This is dangerous and bullshit. You can't just say that to people! You should only destroy handles when you don't need them anymore, not "always"!

Nulling a global variable before changing it makes no sense and is a waste of action. Nulling globals is not required either.

There are some handles that should never be destroyed. This is also missing a list of JASS destructors in case there is no GUI equivalent.


Click to expand...

i was told by maker that you should destroy / null point variables b4 changing them. otherwise they leak. Also i was told by a few ppl that globals do leak if they are not nulled. I should include a list of handles that shouldn't be destroyed. Again tho i dont know all of them. A list from someone more experienced with this would be helpful.

As for the JASS destructors ur right. i will include them in a list also.

Zwiebelchen said:

 GetLocalPlayer:

Misses an explanation of what desyncs are.

It's also a bad example, because it creates handles inside a local block, which might handle IDs to collide. It doesn't instantly desync, but it might cause desyncs in longer games.

Also, it doesn't explain what GetLocalPlayer() actually does and why it works the way it works.

If you link to a seperate tutorial anyway, why even bother including it into this tutorial?


Click to expand...

Like i said in the tutorial this is a list of the things GUIer's should know. Things that most commonly get asked by GUIer's. I will put a short definition of these up there. But i also link to other tutorials even if i cover some of the material. This tutorial is meant as a base tutorial so everyone learns the things at once instead of finding out about something they needed to know later. As for the desync i left the game running for about an hr and nothing happened when i tried it again so i don't believe it will desync. if i find out that it does i will change this.

Zwiebelchen said:

 If there is indeed a need for an "all in one" tutorial like this one, you should definitely mention hashtables and how to use them, as they are very powerful, especially for GUIers which can't use locals without custom code.


Click to expand...

I didnt get a chance to add this last night. I was hoping to put this tutorial here to get feedback on what i need to add. I was hoping for some ppl to give me lists or whatever to make this tutorial full. As i do not know everything about GUI.


 Last edited: Apr 19, 2013

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Chaosy said:

 remove the custom scripts that is nothing you need to know in GUI

expect local player


Click to expand...

which custom scripts ? plz elaborate .

Custom scripts are some things a GUIer should know imo. not all of them just the basis of how they work.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I do disagree, you don't NEED those. You can code, anything with normal GUI therefor you don't NEED it. If you like GUI you want GUI not jass

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Chaosy said:

 I do disagree, you don't NEED those. You can code, anything with normal GUI therefor you don't NEED it. If you like GUI you want GUI not jass


Click to expand...

again plz tell me what custom scripts u are referring to.

Also if they dont want to learn the custom scripts i have in here thats there choice. Im not telling them they have to im just giving them an option to. I think custom scripts are a good idea because of the local variables. i also give a list of GUI units - jass conversions and Gui item - jass conversions. I will put a statement in the Dealing with local variables saying that it is not necessary to learn these but it is helpful.

Updated. added a chapter on infinite looping.


 Last edited: Apr 19, 2013

#### Orthane

#### Orthane

 Level 7

JoinedJan 22, 2013Messages293

I expected nothing less of you death, +rep. I liked it so much I added it to my favorites toolbar for my browser.

Edit: Gah, I owe you like 2 rep but I have to spread more rep around before I can give you any.

Chaosy said:

 I do disagree, you don't NEED those. You can code, anything with normal GUI therefor you don't NEED it. If you like GUI you want GUI not jass


Click to expand...

Let me break down your hostile whatever Chaosy. GUI users can make GUI triggers, without jass those triggers make broke games. Its basically a REQUIREMENT to know jass in order to make good GUI. It shouldn't matter if he is helping people learn Jass in a GUI structure, it helps them with the "GUI CUSTOM SCRIPT" it also gives them an insight on jass and helps them learn it if they wish. This is by far one of the best GUI's I've seen.

I had a difficult time learning how to use an variable in the array integer for custom script. show them the following.

Add this:

Custom script: Call RemoveLocation(udg_AntiLeak[udg_CurrentIndex])
 This should give them a idea that they can do it. Embarassing as it is, I picked that up off of looking at someone else post aid for a trigger. I never even thought about it lol.


 Last edited: Apr 19, 2013

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Orthane said:

 I expected nothing less of you death, +rep. I liked it so much I added it to my favorites toolbar for my browser.

Edit: Gah, I owe you like 2 rep but I have to spread more rep around before I can give you any.


Click to expand...

thanks for ur reply lol i plan on adding hashtables and mybe a short indexing tutorial. ill have to see about the indexing tutorial.

update: added hashtables.


 Last edited: Apr 20, 2013

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

lets say I got this trigger

Untitled Trigger 001 Copy

Events

Unit - A unit Begins casting an ability

Conditions

(Ability being cast) Equal to mySpell

Actions

Custom script: local unit u1 = GetTriggerUnit()

Custom script: local unit u2 = GetSpellTargetUnit()

Wait 10.00 seconds

Custom script: set udg_caster = u1

Custom script: set udg_target = u2

For each (Integer tempInt) from 1 to 10, do (Actions)

Loop - Actions

Unit - Cause caster to damage target, dealing 500.00 damage of attack type Spells and damage type Normal

Custom script: set u1 = null

Custom script: set u2 = null
the jass in there got no real function, you could just use GUI there

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Chaosy said:

 lets say I got this trigger

Untitled Trigger 001 Copy

Events

Unit - A unit Begins casting an ability

Conditions

(Ability being cast) Equal to mySpell

Actions

Custom script: local unit u1 = GetTriggerUnit()

Custom script: local unit u2 = GetSpellTargetUnit()

Wait 10.00 seconds

Custom script: set udg_caster = u1

Custom script: set udg_target = u2

For each (Integer tempInt) from 1 to 10, do (Actions)

Loop - Actions

Unit - Cause caster to damage target, dealing 500.00 damage of attack type Spells and damage type Normal

Custom script: set u1 = null

Custom script: set u2 = null
the jass in there got no real function, you could just use GUI there


Click to expand...


That was just to give examples of MUI and to get them to see that custom scripts / local variables are usefull.

#### Orthane

#### Orthane

 Level 7

JoinedJan 22, 2013Messages293

@chaosy

Custom script Locals and Variables and Get"GameHandle" Any advanced triggerman would know that jester. That the fact it's way easier to read because its not being the elephant in the room compared to the jass line that is a mouse. its more direct, faster and doesn't have all that extra text.

although it would make more sense if the trigger wasn't jiberish.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

actually, the code would be shorter without the custom scripts due you dont need to null at the end

also you use two variables that are the same thing which is bad (stupid)


 Last edited: Apr 20, 2013

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

I know this i just wanted to show a couple different ways to do it.

#### Orthane

#### Orthane

 Level 7

JoinedJan 22, 2013Messages293

Chaosy said:

 actually, the code would be shorter without the custom scripts due you dont need to null at the end


also you use two variables that are the same thing which is bad (stupid)


Click to expand...


Look, why are you going to be so negative, its one part of a Massive tutorial, overall its a beast. Be Postive, You give bogus examples now and again too Chaosy, don't deny it.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

@Chaosy

Nulling variables cleans leak.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Chaosy said:

 also you use two variables that are the same thing which is bad (stupid)


Click to expand...


what do u mean i use 2 variables that are the same ?

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

Chapter 7 - When To Destroy / Null Variable

1. You should tell them to include prefix of 'udg_' in front of variable name.

Chapter 8 - Preloading

1. I think you should only add those abilities, without removing them because when you remove the unit, the abilities is removed with the unit.

2.

 The reason preloading should always be done is to stop lag when you cast the ability for the first time.


Click to expand...

It's not because the unit casts the spell that causes the small spike, it is because the spell is preloaded (by default by the game engine) for the first time. For instance, you have Spell A, and when you cast Spell A, it will create a dummy unit and this dummy unit has Spell B. Now, when dummy is created (for the first time in the game), the Spell B is preloaded to that dummy, that's what causing the spike, not the cast of a spell. This usually happens to a spell that cause another spell to be related (like in this case, Spell A causes dummy to cast Spell B).

3. You should not always preload stuffs at Elapsed Time 0.00, you can do it at Hero Selection, Map Initialization, depends on the game status. For instance, DotA preloaded their Heroes' spells right after a hero is picked. Imagine if DotA preloaded all of the spells at Elapsed Time 0.00 ? It would cause heavy spikes around 10 - 30 seconds. For Map Initialization, you can use this method to preload all spells, but for a limited number of spells. For instance, an RPG map having total 16 abilities, you can do this BUT the price is, longer loading time on the screen. It's better to have longer loading time than heavy spike while in the game.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

defskull said:

 Chapter 7 - When To Destroy / Null Variable

1. You should tell them to include prefix of 'udg_' in front of variable name.

Chapter 8 - Preloading

1. I think you should only add those abilities, without removing them because when you remove the unit, the abilities is removed with the unit.

2.

It's not because the unit casts the spell that causes the small spike, it is because the spell is preloaded (by default by the game engine) for the first time. For instance, you have Spell A, and when you cast Spell A, it will create a dummy unit and this dummy unit has Spell B. Now, when dummy is created (for the first time in the game), the Spell B is preloaded to that dummy, that's what causing the spike, not the cast of a spell. This usually happens to a spell that cause another spell to be related (like in this case, Spell A causes dummy to cast Spell B).

3. You should not always preload stuffs at Elapsed Time 0.00, you can do it at Hero Selection, Map Initialization, depends on the game status. For instance, DotA preloaded their Heroes' spells right after a hero is picked. Imagine if DotA preloaded all of the spells at Elapsed Time 0.00 ? It would cause heavy spikes around 10 - 30 seconds. For Map Initialization, you can use this method to preload all spells, but for a limited number of spells. For instance, an RPG map having total 16 abilities, you can do this BUT the price is, longer loading time on the screen. It's better to have longer loading time than heavy spike while in the game.


Click to expand...

chapter 7

1. u are definitely right on that i thought i did put that in. but i missed it lol will do that asap.

chapter 8

1. i thought it would bug if u dont remove them.

2. i know y it lag spikes and in my map i preload 40 abilities each with 100 lvls at 0.00 timer no lag. im not sure about dota i never played. i figured 0.00 is best time to preload because u just dont notice it. the game normally starts b4 u notice the lag spike. also id rather a lag spike at the beginning than in the middle of the game. i will include a description about y it lags when u dont preload. i didnt think u could add abilities to units at map init. this doesnt cause a crash ?


 Last edited: Apr 20, 2013

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Orthane said:

 Look, why are you going to be so negative, its one part of a Massive tutorial, overall its a beast. Be Postive, You give bogus examples now and again too Chaosy, don't deny it.


Click to expand...

Maybe I am negative but it's just critism it's not like he needs to follow it.

And here is a another part of the tutorial

substrings

Events

Player - Player 1 (Red) types a chat message containing -give as A substring

Player - Player 2 (Blue) types a chat message containing -give as A substring

Player - Player 3 (Teal) types a chat message containing -give as A substring

Player - Player 4 (Purple) types a chat message containing -give as A substring

Player - Player 5 (Yellow) types a chat message containing -give as A substring

Player - Player 6 (Orange) types a chat message containing -give as A substring

Player - Player 7 (Green) types a chat message containing -give as A substring

Player - Player 8 (Pink) types a chat message containing -give as A substring

Conditions

Actions

Set tempPlayer = (Triggering player)

Set tempString = (Entered chat string)

Set subStr = (Substring(tempString, 7, 7))

Set subStrMoney = (Substring(tempString, 9, (Length of tempString)))

Player - Set tempPlayer Current gold to ((Player 1 (Red) Current gold) - tempMoney)

Custom script: set udg_givePlayer = Player( S2I( udg_subStr - 1) )

Player - Add (Integer(subStrMoney)) to givePlayer Current gold

Set tempString = <Empty String>

Set subStr = <Empty String>

Set subStrMoney = <Empty String>

Custom script: set udg_tempPlayer = null

Custom script: set udg_givePlayer = null
you don't null player variables in GUI.. and you dont need the custom script there is a fully working GUI action for that.

Almia said:

 @Chaosy

Nulling variables cleans leak.


Click to expand...

indeed but in GUI we dont null variables, that's why I said the local variables was un-needed they only make the code longer

anyways thanks for linking to my tutorial.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

@Chaosy

We do. All kinds of variables leaks,either globals or locals. Maker proved it(just ask maker)

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Chaosy said:

 Maybe I am negative but it's just critism it's not like he needs to follow it.

you don't null player variables in GUI.. and you dont need the custom script there is a fully working GUI action for that.

indeed but in GUI we dont null variables, that's why I said the local variables was un-needed they only make the code longer

anyways thanks for linking to my tutorial.


Click to expand...

i dont mind the criticism lol. its only to help improve.

if u dont null global variables they do leak. it is small but im all about efficiency if its one extra line for cleaning a leak then thats ok.

Maker showed me that globals do leak.

And ill always link to good tutorials. That tutorial has helped me a lot so if i right and i believe that another tutorial will help the ppl reading mine then i will link it.

edit: Almia beat me to it lol. didnt update thread before i wrote the post lol

#### Xerxes

#### Xerxes

 Level 20

JoinedApr 14, 2012Messages2,906

Resources: 8

#### Resources
Icons 4Maps 1Spells and Systems 3

This is a nice tutorial... even though I've been coding for more than a year now there's still a lot that I don't know about... thanks deathismyfriend!

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

@deathismyfriend


Ha!

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

glad it could help.

@Almia

not sure what the ha is for lol

#### Xerxes

#### Xerxes

 Level 20

JoinedApr 14, 2012Messages2,906

Resources: 8

#### Resources
Icons 4Maps 1Spells and Systems 3

deathismyfriend said:

 glad it could help.


@Almia


not sure what the ha is for lol


Click to expand...


I think he proved a point or something... that's what I'd use "ha!" for...

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

probably i just cant think of it atm lol

#### Xerxes

#### Xerxes

 Level 20

JoinedApr 14, 2012Messages2,906

Resources: 8

#### Resources
Icons 4Maps 1Spells and Systems 3

deathismyfriend said:

 probably i just cant think of it atm lol


Click to expand...


Hehe neither could I. Again, nice tut.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

i think it was about the broken hashtable actions that i just added to the hashtable section.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Yes. Go for it.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Almia said:

 Yes. Go for it.


Click to expand...


lol already posted the bad actions for hashtables up there. also expanded the list on custom scripts u could call to clean leaks. That part took forever lol.


Im hoping to include some things about gameplay constants in a future update. also i added some things that people may want to know about variables.


updated: added a short section on gameplay constants with


 Last edited: Apr 20, 2013

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

It looks great!

Section four seems small, but for the most part I couldn't think of anything else worth adding that hasn't been talked about already.

With so much information, I would recommend this to anyone,

wether they be learning, or even if they're more advanced.

I saw some things I didn't know, that's for sure.

Also, perhaps make the section titles larger than the others. The colors help I suppose,

but it could still be confusing.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Archange1 said:

 It looks great!

Section four seems small, but for the most part I couldn't think of anything else worth adding that hasn't been talked about already.

With so much information, I would recommend this to anyone,

wether they be learning, or even if they're more advanced.

I saw some things I didn't know, that's for sure.

Also, perhaps make the section titles larger than the others. The colors help I suppose,

but it could still be confusing.


Click to expand...

glad it helped and glad ur gonna recommend it thx.

i was thinking about changing the size i will do that soon im making food now lol.

updated: changed size of sections and chapters

#### Faith

#### Faith

 Level 14

JoinedDec 29, 2009Messages928

Resources: 3

#### Resources
Spells and Systems 1Tutorials 2

It looks much better now!

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Archange1 said:

 It looks much better now!

Click to expand...

thats good lol still got some things to edit to make it easier to read the lists and things like that but ill get to that soon i hope.

updated: made the lists more distinquishable


 Last edited: Apr 22, 2013

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

The explanation of the operation limit is still total fail. It still does not explain what the operation limit is. The OP limit has nothing to do with arrays or array sizes. The operation limit is only the limitation of possible memory operations per thread.


Initializing array variables reaches the OP-limit very fast because it indexes all 8096 array members in one thread, but still, the OP-limit is something very different.


Also, you can not explain the OP-limit by mentioning "threads" and then not explaining what those are.


I've read your updated version of this tutorial and feel it's a total overkill of information. There's a lot of stuff that has other, much more detailed tutorials available. Instead of trying to explain everything in one tutorial, you should only give a small overview about each topic, discuss why knowledge about this is important and then give the link to the detailed tutorial.


The object editor/channel and gameplay constants chapter should just give a short summary of what the specific topics are about and then link to the dedicated tutorials, for example.


Same goes for hashtables. There's a very good tutorial on hashtables on hive and you should just link to instead of trying to explain the topic on your own.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

how about splitting the chapters via [hr]SizeHere(mustbeinteger)[/hr] Example:


You also have a double-link there(Hashtables and MUI)

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Zwiebelchen said:

 The explanation of the operation limit is still total fail. It still does not explain what the operation limit is. The OP limit has nothing to do with arrays or array sizes. The operation limit is only the limitation of possible memory operations per thread.

Initializing array variables reaches the OP-limit very fast because it indexes all 8096 array members in one thread, but still, the OP-limit is something very different.

Also, you can not explain the OP-limit by mentioning "threads" and then not explaining what those are.

I've read your updated version of this tutorial and feel it's a total overkill of information. There's a lot of stuff that has other, much more detailed tutorials available. Instead of trying to explain everything in one tutorial, you should only give a small overview about each topic, discuss why knowledge about this is important and then give the link to the detailed tutorial.


Click to expand...

ill check on making the description for the op-limit a little more.

i did miss an explanation on threads.

This tutorial does have a lot of info. i wanted it to be that way. some of the stuff i wanted to stress the importance of them so i did make the explanation a little longer for those. I want to add more links to more detailed tutorials just like i did with a few of the tutorials in Section 4. Like i said b4 if u have any tutorials u think i should link to this then post me the link. I dont see this as an overkill because i dont explain every little detail for everything. this is one reason y i need more tutorials so i can say that there is a more detailed tutorial on that specific chapter. this way they can get a basis from this tutorial then move onto the more advanced / more detailed tutorial for a better understanding.

Almia said:

 how about splitting the chapters via [hr]SizeHere(mustbeinteger)[/hr] Example:

Click to expand...

umm not sure what this does ? that just the big line ?

Almia said:

 You also have a double-link there(Hashtables and MUI)


Click to expand...

lol didnt notice i double posted the one link.

ill change these things tomorrow

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Its a divider. Good for descriptions.The size determines how long the line is:

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Nice nice ill definitely use that.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

You can also use Tables,they are good seperators.

edit

I should make a tutorial template.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Almia said:

 You can also use Tables,they are good seperators.

edit

I should make a tutorial template.


Click to expand...

If u mean a tutorial on the bbcodes and things like that then ya definitely lol.

updated.


 Last edited: Apr 22, 2013

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

 It is important to know these variables because all handle variables should be nulled and destroyed/removed when needed.


Click to expand...

-> Handle variables should be nulled and destroyed/removed once you don't need them anymore.

 Highest integer you can store in an integer variable is -2147483647


Click to expand...

-> Highest integer you can store in an integer variable is 2147483647 or 2^32-1.

 Chapter 2 - Dealing with array sizes


Click to expand...

Explain multi dimensional arrays.

myArray[10][10]...[10] can be easily transformed into GUI code.

Note 10*10*...*10 limit must still be < 8190

 (Last restored unit) = GetLastRestoredUnitBJ()


Click to expand...

-> bj_lastLoadedUnit

 JASS:

function GetLastRestoredUnitBJ takes nothing returns unit
 return bj_lastLoadedUnit
endfunction

 (Last replaced unit) = GetLastReplacedUnitBJ()


Click to expand...

-> bj_lastReplacedUnit

 JASS:

function GetLastReplacedUnitBJ takes nothing returns unit
 return bj_lastReplacedUnit
endfunction

 (Attacked unit) = GetAttackedUnitBJ()


Click to expand...

-> GetTriggerUnit()

 JASS:

function GetAttackedUnitBJ takes nothing returns unit
 return GetTriggerUnit()
endfunction

 (Killing unit) = GetKillingUnitBJ()


Click to expand...

-> GetKillingUnit()

 JASS:

function GetKillingUnitBJ takes nothing returns unit
 return GetKillingUnit()
endfunction

 (Loading unit) = GetLoadedUnitBJ()


Click to expand...

-> GetLoadedUnit()

 JASS:

function GetLoadedUnitBJ takes nothing returns unit
 return GetLoadedUnit()
endfunction

 (Transporting unit) = GetTransportUnitBJ()


Click to expand...

-> GetTransportUnit()

 JASS:

function GetTransportUnitBJ takes nothing returns unit
 return GetTransportUnit()
endfunction

 Chapter 2 - Dealing with Waits


Click to expand...

You can explain what "shadowing globals mean", because it's awesome way for GUI user to make MUI spell.

Example local unit udg_Caster = GetTriggerUnit()

 Chapter 9 - Use of Substrings


Click to expand...

Trigger is really bad, you should use loop to register those Events from another trigger.

It's faster for coding and easier for editing.

 Chapter 13 - The Do Nothing Action

This is about the Do Nothing action. Well there is really only a few things to tell about it.

This action is completely useless. Never use it. The reason for this is it does exactly what it says.

It does nothing except slow your triggers down and it can cause bugs. It is never needed in any trigger.


Click to expand...

It won't slow your trigger down so much you notice that in game, EVER.

Frames won't drop.

Also there is use for it in single lines IF THE ELSE functions.

For starters it's always better to include Do Nothing into triggers just so they don't lost in IF THEN ELSE fields that GUI create for them.

Once they become more skilled they will learn about trigger efficiency and start to care about how code looks like.

Should I mention that single GUI function (Replace Unit) for example generate like 90% useless code that you don't need. Using those 2 or functions in your map equal more or less to 50 Do Nothing functions.

 Section 3 - Gameplay Constants


Click to expand...

I don't think this is needed here.

 Section 4 - Object Editor


Click to expand...

Useless as well. Why? You mentioned just 0.001% of whole thing.

For example you explained damage formula but not damage type, armor and attack types, reductions, many many thing, not to mention even model backs-wing animation and other shit.

 Chapter 1 - Debugging


Click to expand...

In last Hidden button you missed slash (or backslash, whatever) to make TRIGGER tag work.

More or less that's all from quick look.

Still I don't like how you missed few important things like:

- IF THEN ELSE functions, how they nest and why is sometimes some way better than another etc.

- Floating Texts wasn't mentioned. There are few tricks that GUI users should know.

- Making functions using GUI

- Using triggers as functions in GUI

- Skip Remaining Actions

- Timers

...

After all, GUI user should know all of this.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

ok u made some good points. im not sure if im going to remove the gameplay constants or the object editor although i may make other tutorials to cover those 2 fields so ill have to see about removing them. if enough ppl think i should remove them i will. let me know ur opinions plz.

as for the do nothing actions. the points u made im not really sure what u mean by them being usefull in the ITE's. also this tutorial is meant to get them started on efficient triggering ways from the time they read this. i dont want to include inefficient ways in this trigger as efficiency is a must in any game.

Yes i will include a chapter on shadowing globals. i think it will be a subchapter under the local variables chapter. ( fixed)

im not sure y i never inlined those custom scripts lol. that's a must ill do that also. ( fixed)

for mutli-dimensional arrays do u mean arrays inside arrays ? i never really used them in GUI. ( fixed)

i couldnt remember the highest number thing ( 2^32-1) thanks for that. ( fixed)

as for the first thing about destroying and nulling i knew something didnt sound right when i read it thanks. ( fixed)

the replace unit does generate a ton of useless code. mybe ill add a small chapter on this and other functions to stay away from since u can trigger it urself.

making functions in GUI i never did that so i will have to check that out unless someone wants to do that for me ull get full credit for it in the tutorial. ( fixed but will add a few more examples)

skip remaining actions / timers / and floating texts will be added. note: i haven't used these that much in GUI.

ITE's and nesting them will be included although im not sure what u mean by why is sometimes way better than another

and again thanks for ur opinions and suggestions.


 Last edited: Apr 23, 2013

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

 ok u made some good points. im not sure if im going to remove the gameplay constants or the object editor although i may make other tutorials to cover those 2 fields so ill have to see about removing them. if enough ppl think i should remove them i will. let me know ur opinions plz.


Click to expand...

Just cover them with new tutorials.

 as for the do nothing actions. the points u made im not really sure what u mean by them being usefull in the ITE's. also this tutorial is meant to get them started on efficient triggering ways from the time they read this. i dont want to include inefficient ways in this trigger as efficiency is a must in any game.


Click to expand...

Ok then, do as you wish.

 for mutli-dimensional arrays do u mean arrays inside arrays ? i never really used them in GUI.


Click to expand...

No I mean this:

example for 2D array (100x100)

my2Darray = (i * 100) + j where i,j=i,2,...,99

 making functions in GUI i never did that so i will have to check that out unless someone wants to do that for me ull get full credit for it in the tutorial.


Click to expand...

It's done by splitting trigger Actions into 2 or more functions and use them later.

Simple and useless example:

Test
Events

Conditions

Actions
Custom script: endfunction

Custom script: function ShowTestMsg takes nothing returns nothing

Game - Display to (All players) the text: -Test-

Because converted you get:

 JASS:

function Trig_Test_Actions takes nothing returns nothing
 endfunction
 function ShowTestMsg takes nothing returns nothing
 call DisplayTextToForce( GetPlayersAll(), "TRIGSTR_002" )
endfunction

//===========================================================================
function InitTrig_Test takes nothing returns nothing
 set gg_trg_Test = CreateTrigger( )
 call TriggerAddAction( gg_trg_Test, function Trig_Test_Actions )
endfunction

RunTest
Events
Unit - A unit Dies

Conditions

Actions
Custom script: call ShowTestMsg()

Note: trigger with call must be placed below trigger with function.

 ITE's and nesting them will be included although im not sure what u mean by why is sometimes way better than another


Click to expand...

Depends on what you need, you can sometimes split IF THEN ELSE, sometimes is best to nest them into each other and so on.

You will get than once when you start explaining or making examples.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

ok i see what ya mean by the ITE's lol.

The functions will definitely be added to tutorial. thanks for that.

i may change them into other tutorials. i just thought covering the main parts was a good idea.

as for the multi-dimensional arrays. Thats still one array just with math functions used for indexing ?

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

 as for the multi-dimensional arrays. Thats still one array just with math functions used for indexing ?


Click to expand...

Exactly

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

ok lol i wouldnt of thought of that as multi-dimensional but i see how it can be ill definitely put that in the tutorial. thanks for all ur help Kobas i added u to the people that helped list.

also im going to quote u on the function thing and give u credit and make one of my own for multiple examples. hopefully i have enough time to make all these changes b4 i leave lol

 You must log in or register to reply here.


### Similar threads

 Hero Selection Systems


Tonks

Mar 23, 2007

Trigger (GUI) Editor Tutorials

Replies12Views18K

Jun 8, 2023
dantles1992

 Simple Triggered Hero Artificial Intelligence in GUI


DenZel94

May 2, 2007

Trigger (GUI) Editor Tutorials

Replies22Views26K

Jul 1, 2021
aloH

 MUI Spells Using Artificial Waits


Tank-Commander

Sep 28, 2012

Trigger (GUI) Editor Tutorials

Replies32Views20K

Oct 16, 2015
BETABABY

 State-based Passive Ability Tutorial


Blightsower

Sep 22, 2024

Trigger (GUI) Editor Tutorials

Replies8Views3K

Sep 23, 2024
Blightsower

 GUI Equipment Manager - up to 11 items at once


Donach

Apr 16, 2012

Trigger (GUI) Editor Tutorials

Replies25Views15K

Jun 29, 2014
Tal'Rarity

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