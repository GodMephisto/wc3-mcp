---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-make-a-chain-spell-gui-version.7015/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Make a Chain Spell GUI Version
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-make-a-chain-spell-gui-version.7015/>
#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

Bookmark

1. Introduction

2. Basic Concept

3. Object Editor Notions

4. Trigger Editor Notions

5. Fixing memory leaks

6. Final Note

1. Introduction


Ever wondered how to make a Chain/Bouncing spell in Gui, but no matter how hard you tried, you simply couldn’t find an easy way to do it? Well, then this tutorial is just for you. Now, I’m warning you that this tutorial does not allow you to make any chain spell and customize it as much as you would want. Gui has its own limits. However, you will be able to pick target units in chain and cast one or more multi-instanceable spells (this includes all blizzard spells) on them or at their position.


In order to understand this tutorial you need basic knowledge about Gui/manipulating the trigger editor. All codes in this tutorial were implemented and tested with Frozen Throne, version 1.20d but they should work with lower versions too.

2. Basic Concept


Chain/Bouncing spells are those spells which affects a certain number of units in a row, one by one. The units have common attributes (examples: enemy units of the caster, spellcasters, non-hero etc.). The targets of the spell may be chosen in a random manner or in a specific one (such as picking the units with the most mana). Examples of such spells are Chain Lightning, Healing Wave and Spirit Link.


When you want to design a chain spell, you must first picture it in mind. Which units do you want it to affect? Is it enemy units or friendly units? Do units need to have mana or only life is necessary? Can it affect hero units? Let’s say that our spell will affect enemy units with mana. Can affect heroes and cannot affect unorganic units. Next think about the effect of the spell. Do you want it to slow targets, decrease their damage or armor, make them lose life or change their owner? In this case, I’d say we make a chain mana burn, so the effect would be the same with the one of mana burn. Burns mana and depending on the amount burnt, unit loses the same amount of life.

3. Object Editor Notions


For those already familiarized with spellmaking, and so, the World Editor, the object editor is not something new. When making a custom spell you need to base your spell of an already existent Blizzard one. You may need auxiliary ones as well, cast by auxiliary units. Last but not least, your custom spells may require custom buffs. It is required that you pay attention to all these aspects.


For a chain spell, it is pretty clear that we will need a dummy spell for the original caster. That includes only the characteristics common to all the spells such as mana cost, cooldown, icon, description, levels etc. but except that, the spell does absolutely nothing.


A chain spell is usually a single unit target spell, so for a dummy spell I would recommend are any single unit buff spell with duration reduced to 0.01 (Cripple, Rejuvenation, Shadow Strike) or any chain spells (Spirit Link, Chain Lightning, Healing Wave). Keep in mind though that your dummy spell should not have the potential of breaking channeling spells (storm bolt, hex, aerial shackles) or be autocastable (slow, inner fire, curse). You should also avoid spells whose effects cannot be eliminated (such as Purge).


Some recommend the spell “Channel” for everything and so I will dedicate a paragraph to it as well. Channel is considered great because it was made specifically for being a dummy unit. It is the only spell whose target type can be modified, from no target (instant cast) to a unit or a point. Almost all fields are valid, from AoE to targets. You can even change the order ID through a special field, allowing multiple instances of the spell to be used by the same unit.


I disable all art for the spell, because I am usually triggering the effects myself. However, there are some specific fields of the spell that I will explain.

Data – Disable Other Abilities -> if set to true then caster is practically paused until spell wears off.

Data – Follow Through Time -> turns Channel into a channeling spell. This field represents its duration.

Data – Base Order ID -> this field can be used to change spell’s base order.

Data – Options -> multiple checkboxes. The important field is Visible which if disabled hides the icon of the spell, so normally you should ensure that it is active. Universal spells affect magic immune units (like ultimates).


For a dummy unit, I usually use the albatross unit, and hide it via triggers. However, in that case, you must be absolutely sure that all the spells made for the dummy cost 0 mana. If you want, you can make your own dummy.


Spells given to dummy units unlike the spell given to the caster is polarized (I put no value to the fields mentioned for caster) and so, their purpose is to make the action of a spell (slow an unit, partially remove mana, silence target etc.). For our example, we will use mana burn.


Now, in order to obtain the chain reduction effect (the spell loses strength as additional targets are hit), we will need to use multiple levels for any spell given to the dummy unit. The number of levels should be equivalent to the maximum number of targets the chain spell can affect. The strength of the spell should decrease with its level, so practically the effect on the initial target will have the strength of the first level.


For example, our chain mana burn can affect up to 6 targets will burn 50 mana to the first target and for every additional target, its effect will decrease with 15% for each target. So the mana burnt per levels should like this:


To add the values automatically you will have to use the Auto Fill Levels. Start from level 1, set base value to the value for level 1 (in this case 50.00) and the Previous Level Factor to 0.85 (because formula is (100-decrease)/100, where decrease is 15% in this case).


Note: If you want to instead increase the effect of the spell with each target, then the formula is ((100+decrease)/100). That would be 1.15 in this case.


Now, problem comes when you have multiple levels for the original dummy spell. In that case, there are two possibilities: you either make for each level a different spell to give to the dummy unit (in this case, you will practically need a mana burn for each level), or multiply the number of levels for dummy’s spell with the number of levels the original spell should have. In this tutorial I will use the second variant.


In this case, let’s say we want a 3 levels mana burn that does 50/75/100 damage to up to 4/6/8 targets. The formula we will develop in the trigger requires that we have the same number of sub-levels for each level of the spell. So we will need our mana burn spell to have 8*3=24 levels.


For sub-levels 1-8, apply the reduction for level 1, just like above (but for 8 levels). Then for sub-levels 9-16 apply the reduction for level 2. Last but not least, apply the reductions for sub-level 17-24. This is how my reduction looks (I know 24 levels is huge but believe me, this is easier to implement than 3 distinct spells).


To do the auto-filling, first start from sub-level 1 with the starting value for level 1 (50). Then make another auto-filling starting from sub-level 9 with the starting value for level 2 (75). Lastly make an auto-filling from sub-level 17 with the starting value for level 3 (100). Voila! Your ability for the dummy unit should be ready.


Note: If you wish to cast on the targets of the chain multiple buff spells, then give all the spells the same buff. Even though spell steal will be able to steal only one of them at a time so it will not be compatible with it, the unit will apparently have only one buff upon it.


A last important spell will be the one that triggers the effect of the spell on each unit. Instead of complicating ourselves with dummy units we move from an unit to another, we can easily use already existent buff spells that will help us obtaining this. Base this spell off Shadow Strike (effect will only be missile because for lightning spells you would need such a spell with a buff ). The spell should last only 0.6 second and have the same buff the abilities for the dummy units should have. If you do not use such abilities (like in our case) then make one right now and give it to the effect spell. Now to make the spells do nothing but show their effect. Change mana cost and cooldown to 0 and range to a maximum value. You can change the appearance of the missile from the values of the field Art – Missile Art. Art – Missile Homing enable should be left to true because that way, it will follow the target. Give it a missile arc only if you don’t want it to go straight. And now to make the spell do nothing, change Data - Decay Power to 3600.00 and Data – Decaying Damage, Data – Initial Damage and Data – Movement Speed Factor to 0.00.


Note: Make sure all spells for dummy units affect the same type of units the chain spell should (in this case air, ground, neutral, enemy, organic).

4. Trigger Editor Notions


If the Object Editor part seemed difficult, I am warning you from the beginning that this won’t be easier. So take it step by step and reread parts you didn’t get. You may not understand everything from the first lecture.


For this spell you will need only one trigger which will do everything. Since it will be really complex, I suggest you add comments here and there and do it step by step. Test the spell after adding a new element to the spell so that you don’t realize in the end that you have no idea where the bug is coming from.


So let’s get started. First, let’s take care of the global variables. Using them will mean that the spell loses its multi-instanceability. Sorry, but that’s the only way to do it. You will need two unit variables for each spell: one to store the current target of the spell (which I called targ) and one to store the previous target of the spell (called prev). For the chain mana burn spell I called them MB_targ and MB_prev.


Next variable is needed to store the level of the chain spell when cast (MB_level). Next you also need a variable which stores the maximum number of targets that can be affected by the spell (MB_targetsnum) while another variable stores the current number of the target (MB_cur). All these variables are integers.


Lastly you will need a unit group variable (MB_alltargs) to stored the units already affected by the spell so that we can ensure that an unit will not be affected twice by the spell.


Now let’s create the trigger of the spell. Add the classical event and condition of a spell.

Chain Mana Burn
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Chain Mana Burn

Now first thing to do is initialize the variables. Initially the “previous” unit is the caster, because that’s the source of the effect. The Target unit of the ability is obviously the current target. For the number of targets it should be simple. At level 1 we initially have 4 targets, while with level number of targets increase by 2.

Actions
Set MB_prev = (Triggering unit)

Set MB_targ = (Target unit of ability being cast)

Set MB_level = (Level of Chain Mana Burn for MB_prev)

Set MB_targetsnum = (4 + (2 x (MB_level - 1)))

Next comes the large loop which will keep rolling until the maximum number of targets has been affected. We will later see how we can interrupt it if there are no more valid targets. So, add the following loop to the actions

For each (Integer MB_cur) from 1 to MB_targetsnum, do (Actions)
Loop – Actions
Unit Group - Add MB_targ to MB_alltargs

What this loop will do is increase the value of the current target until it is greater than the number of maximum targets, point at which it will stop. As you can notice I added the target to the group of affected units. At the beginning of each loop, the target will be added to the group.


Now, there are four important steps our spell must follow: Cast the effect spell on the target, wait until the spell affects the unit, add the effect of the chain spell to the target and search for a new target. Then the procedure repeats thanks to the loop.


Let’s start with casting the effect spell on the target. Since the effect starts from the previous target, create the dummy unit there and order it to cast the spell. I also changed flying height or else the albatross would’ve looked weird up in the air. You don’t have to do this or hide the unit if you create your own proper dummy unit.

Unit - Create 1 Albatross for (Owner of (Triggering unit)) at (Position of MB_prev) facing Default building facing degrees
Unit - Hide (Last created unit)

Animation - Change (Last created unit) flying height to 100.00 at 0.00

Unit - Add Chain Mana Burn (EFFECT) to (Last created unit)

Unit - Order (Last created unit) to Night Elf Warden – Shadow Strike MB_targ

Unit - Add a 1.00 second Generic expiration timer to (Last created unit)

Next comes the waiting phase which is very simple. You only have to wait for the target to have the buff of the effect spell. To avoid bugs, I would also end this wait period if the target would die. Because if target is killed before the missile reaches it or just as missile reaches it, the buff will never appear, and so, it will never end. Make sure the checking frequency has a slight lower value than the duration of the effect spell. Watch out that the lowest wait value is 0.27 so anything beyond it is automatically set to this limit.

Wait until (((MB_targ has buff Chain Mana Burn ) Equal to True) or ((MB_targ is dead) Equal to True)), checking every 0.50 seconds
Now it’s time we applied the effect on the target. It’s pretty simple. Just create dummies at its position for each spell and cast it on the unit. The sub-level formula is like this: Maximum_Number_Of_Target*(Level-1)+Current_Target.

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(MB_targ is dead) Equal to False

Then - Actions
Unit - Create 1 Albatross for (Owner of (Triggering unit)) at (Position of MB_targ) facing Default building facing degrees

Unit - Hide (Last created unit)

Animation - Change (Last created unit) flying height to 100.00 at 0.00

Unit - Add a 1.00 second Generic expiration timer to (Last created unit)

Unit - Add Mana Burn (4 Chain Spell) to (Last created unit)

Unit - Set level of Mana Burn (4 Chain Spell) for (Last created unit) to ((8 x (MB_level - 1)) + MB_cur)

Unit - Order (Last created unit) to Night Elf Demon Hunter - Mana Burn MB_targ

Else - Actions
Set MB_cur = (MB_targetsnum + 1)

Note that I added validation to end loop if wait stage ended because of unit being dead. If that happens, the chain spell is over.


And now we finally come to the last and probably the most complicated part: detecting the new target of the chain. Depending on your tastes and preferences, this may be easier or more complicated. Restrictions you will probably need to watch for:


Magic Immune units.


Non-organic units.


Dead Units.


Is enemy/ally.


Has been previously targeted by spell.


Also watch out for the range and change it to your taste from code to code. The range from which units are picked actually represents the maximum distance between two targets.


First, switch the current target to the previous unit since we need to get a new target. Now, the easy way is to pick a random unit within the range of the previous unit matching a certain condition. For this, you will also need another unit group variable (MB_TempGroup) which we will use to pick the targets of the chain. Here is how the code would look.

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
MB_cur Less than MB_targetsnum

Then - Actions
Set MB_prev = MB_targ

Set MB_targ = (Random unit from (Units within 500.00 of (Position of MB_prev) matching ((((Matching unit) is A structure) Equal to False) and ((((Matching unit) is Mechanical) Equal to False) and ((((Matching unit) is Magic Immune) Equal to False) and ((((Matching unit) is

Else – Actions

This way however, spell is not optimal since it will just pick a random target who may have a far too small mana pool. Wouldn’t it be better if the spell would pick the units with the most mana? Well, this is pretty easy to acquire and would be easy to change for other parameters such as life and distance.


For this variant you will need a real variable (MB_SMax). And now let’s get started. First we need for every loop to initialize MB_SMax with an impossible value that could be beaten by any unit in the range. For picking units with “highest value” I would start with a value lower than the one possible (almost always a negative value) while for the “lower value” you would have to choose the highest impossible value (for example for a max range of 500.00, 501.00 is enough).


What you need to actually change is adding a new condition which checks if the stat relating the unit (in this case, current mana) is greater or lower than the one stored into the variable. If condition is correct then you can put the unit into the variable before. Code would look like this:

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
MB_cur Less than MB_targetsnum

Then - Actions
Set MB_prev = MB_targ

Set MB_SMax = -1.00

Unit Group - Pick every unit in (Units within 500.00 of (Position of MB_prev)) and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Picked unit) is A structure) Equal to False

((Picked unit) is Mechanical) Equal to False

((Picked unit) is Magic Immune) Equal to False

((Picked unit) belongs to an enemy of (Owner of (Triggering unit))) Equal to True

((Picked unit) is dead) Equal to False

((Picked unit) is in MB_alltargs) Equal to False

(Max mana of (Picked unit)) Greater than 0.00

MB_SMax Less than (Mana of (Picked unit))

Then - Actions
Set MB_targ = (Picked unit)

Set MB_SMax = (Mana of (Picked unit))

Else - Actions

Else – Actions

At the end of the trigger we clear the MB_alltargs group because the next time the spell is cast, if the units remain in it, they won’t be affected by the spell anymore.

Unit Group - Remove all units from MB_alltargs
The spell should now work!

5. Fixing memory leaks


For those still not experienced with memory leaks, I will do my best to explain this area as brief as possible. This tutorial is not dedicated to this chapter, so I will not be able to teach you how to solve all memory leaks. However, it is important to understand what they are and try to avoid them as possible.


Memory leaks represent a waste of memory. It’s a place of computer’s memory which is occupied by an element of the game, but even though we no longer need to keep that place of memory occupied, it remains so. As a conclusion, that place of memory remains used and unavailable until you exit the game. A few memory leaks are not a problem, but if their number dramatically increases, it can greatly slow up your game, or even freeze it, if you run out of memory.


An example of memory leak, existent even in our code is a point. Every point once created, is stored into the memory, until manually removed (or until you exit the game). There is no specific action made by Blizzard to fix this problem, but you can solve it, via the Custom Script action. To remove a point from the memory your custom script action should look like this:

Custom script: call RemoveLocation(udg_var)
The only element to modify here would be var which is the name of a point variable. So yes, before easily removing a point, you should first add it to a variable, use it through that variable, and then remove the variable. So, for our spell, we need the point variable I called MB_TempPoint.


Groups other than those from variables also leak. Everytime you use an expression in place of a group, it will leak unless you previously store it to a temporary variable, use in place of the expression the variable and then remove it from the memory. To remove a group, the general expression is like this:

Custom script: call DestroyGroup(udg_var)
In this case, var is a group varaible. Here is the final trigger:

Chain Mana Burn
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Chain Mana Burn

Actions
Set MB_prev = (Triggering unit)

Set MB_targ = (Target unit of ability being cast)

Set MB_level = (Level of Unknown (A000) for MB_prev)

Set MB_targetsnum = (4 + (2 x (MB_level - 1)))

For each (Integer MB_cur) from 1 to MB_targetsnum, do (Actions)
Loop - Actions
Unit Group - Add MB_targ to MB_alltargs

-------- Effect Part --------

Set MB_TempPoint = (Position of MB_prev)

Unit - Create 1 Albatross for (Owner of (Triggering unit)) at MB_TempPoint facing Default building facing degrees

Custom script: call RemoveLocation(udg_MB_TempPoint)

Animation - Change (Last created unit) flying height to 50.00 at 0.00

Unit - Hide (Last created unit)

Unit - Add Chain Mana Burn (EFFECT) to (Last created unit)

Unit - Order (Last created unit) to Night Elf Warden - Shadow Strike MB_targ

Unit - Add a 1.50 second Generic expiration timer to (Last created unit)

-------- Waiting part --------

Wait until (((MB_targ has buff Chain Mana Burn ) Equal to True) or ((MB_targ is dead) Equal to True)), checking every 0.35 seconds

-------- Spell part --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(MB_targ is dead) Equal to False

Then - Actions
Set MB_TempPoint = (Position of MB_targ)

Unit - Create 1 Albatross for (Owner of (Triggering unit)) at MB_TempPoint facing Default building facing degrees

Custom script: call RemoveLocation(udg_MB_TempPoint)

Unit - Hide (Last created unit)

Unit - Add a 1.00 second Generic expiration timer to (Last created unit)

Unit - Add Mana Burn (4 Chain Spell) to (Last created unit)

Unit - Set level of Mana Burn (4 Chain Spell) for (Last created unit) to ((8 x (MB_level - 1)) + MB_cur)

Unit - Order (Last created unit) to Night Elf Demon Hunter - Mana Burn MB_targ

Else - Actions
Set MB_cur = (MB_targetsnum + 1)

-------- Choice Part --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
MB_cur Less than MB_targetsnum

Then - Actions
Set MB_prev = MB_targ

Set MB_SMax = -1.00

Set MB_TempPoint = (Position of MB_prev)

Set MB_TmpGroup = (Units within 500.00 of MB_TempPoint)

Unit Group - Pick every unit in MB_TmpGroup and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Picked unit) is A structure) Equal to False

((Picked unit) is Mechanical) Equal to False

((Picked unit) is Magic Immune) Equal to False

((Picked unit) belongs to an enemy of (Owner of (Triggering unit))) Equal to True

((Picked unit) is dead) Equal to False

((Picked unit) is in MB_alltargs) Equal to False

(Max mana of (Picked unit)) Greater than 0.00

MB_SMax Less than (Mana of (Picked unit))

Then - Actions
Set MB_targ = (Picked unit)

Set MB_SMax = (Mana of (Picked unit))

Else - Actions

Else - Actions

Custom script: call RemoveLocation(udg_MB_TempPoint)

Custom script: call DestroyGroup(udg_MB_TmpGroup)

Unit Group - Remove all units from MB_alltargs

I have attached a map with the spell itself if you still want to investigate some elements of it.

6. Final Note


You should now be able to easily make your own custom chain spell in GUI with only one trigger, and a couple of variables. To further improve this spell, you can make it instanceable for different players. To do that replace all variables with arrays and use as an index the Owner of the caster. If you find any problems or have any suggestions for this tutorial, do not hesitate to post them here. Comments are also welcome!


~Daelin

#### Attachments

img3.png

 11.2 KB

 · Views: 1,678



img4.jpg

 40.1 KB

 · Views: 4,015



Chain test.w3x

 21.6 KB

 · Views: 2,051




 Last edited by a moderator: Feb 27, 2007

#### dower39

#### dower39

 Level 2

JoinedAug 30, 2017Messages15

Uhm Daelin. Can u make this in a MUI?

 You must log in or register to reply here.


### Similar threads

Locked

[Trigger]
 Mana Wave as Healing Wave


P3in

Jan 5, 2022

Triggers & Scripts

Replies2Views2K

Jan 6, 2022
P3in

 Spells - Making a Chain spell (GUI Version)


Daelin

Oct 30, 2006

Tutorial Archive

Replies1Views3K

Mar 11, 2007
Wolverabid

Locked

[Trigger]
 Chain spell problem.


Landy

Jul 27, 2007

Triggers & Scripts

Replies2Views1K

Jul 30, 2007
Daelin

Locked

[JASS]
 Life or Death !! Help fast rep+ !!! Chain spell


Flame_Phoenix

Feb 17, 2008

Triggers & Scripts

Replies16Views2K

Mar 2, 2008
Herman

Locked

 Chain Lightning Advanced Spell Request


PrinceYaser

Jan 27, 2017

Requests

Replies16Views3K

Jan 29, 2017
PrinceYaser

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