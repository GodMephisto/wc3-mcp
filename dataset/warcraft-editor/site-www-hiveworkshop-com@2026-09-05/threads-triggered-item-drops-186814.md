---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/triggered-item-drops.186814/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Triggered Item Drops
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/triggered-item-drops.186814/>
#### Zinox

#### Zinox

 Level 4

JoinedSep 24, 2010Messages17

Resources: 3

#### Resources
Icons 1Maps 1Tutorials 1

# Triggered Item Drops

In this tutorial, you will learn how to make creeps drop items upon death using triggers. The tutorial will be split into 5 sections:

Why use triggered item drops?

How do they work?

What do we need?

How do I make one?

Advanced Options/Triggers

## Chapter 1: Why use triggered item drops?

Triggered item drops are used simply to make a creep drop one or more items.

Now, I know that some of you might be thinking:

"Why can't I easily put the drop table for the creep? I don't need triggers!"​

That may work in some specific cases, but otherwise it is not the most controlled method. If you give a creep an item drop table, when the creep dies, the table is gone. Unless you have a method for reviving units, the table will not be retained and the respawned creeps from then on will not drop the appropriate items. So, it is useless if you're making an ORPG (Online role playing game) if all the creeps have one chance to drop an item, am I correct?

If you got mixed up, here is the conclusion:

Respawned creeps will not drop the appropriate items when using the creep item table in the World Editor.

For example, this is a simple creep respawn:

[trigger=Creep revive]Creep Respawn

Events

 Unit - A unit owned by Player 12 (Brown) Dies

Conditions

Actions

 Wait 13.00 seconds

 Unit - Create 1 (Unit-type of (Triggering unit)) for Player 12 (Brown) at (Position of (Triggering unit)) facing Default building facing degrees

[/trigger]

The above trigger is creating a substitute for the dead creep, not reviving it, so the item drop table will be lost, which is why we must use triggers!

​

## Chapter 2: How do they work?

Triggered item drops are made using a percentage system similar to the ones used in item drop tables.

The trigger size varies between the number of items you want the creep to drop. In the trigger, we will put in a random number between 1 and 100, to represent the chances of dropping the item. Why from 1 to 100? Using 1 to 100 is the easiest way to represent percentages, and after all, is there such thing as a 234% chance to drop an item in an item drop table? Though it still works if you make the random number between 1 and 460, 1 to 100 makes it simpler to choose values.

Note: You can use a random integer between 1 and 1,000 for VERY RARE boss item drops, so here is another advantage of triggered item drops!​

Okay, so let's say we put the random number between 1 and 100, what now?

Now, we make conditions to resemble item percentages, you now... the old "if-then-else" actions.

We make the condition like this: If the random number between 1 and 100 is less than or equal to 30, then there will be a 30% chance to drop an item; understand better? That is for one item though. To make 2 items drop, you will need 2 conditions like this: If the random number between 1 and 100 is less than or equal to 30, then create a small potion; and now under the first "if-then-else" block, we put another "if-then-else" block and use the condition If it is less than or equal to 50.

You will learn more about multiple item drops in the last chapter.​

## Chapter 3:What do we need?

Note: Make sure you have understood chapter 1 and 2 before proceeding; if not, then reread them.

What we need in a triggered item drop is 1 integer variable, that's it. (as well as one location variable to remove leaks)

Okay, what we need is a an integer variable. Variables simply reference to objects or data for later or repeated use, and these are very important to any map. There are many types of variables, each can store different types of values.

Now, how do we make a variable? Open the trigger editor (f4).

Then open the variable editor from the trigger editor window (ctrl+B).

Here we are. If you click on Add Variable, you will find several types of variables. In this case, we want an integer variable, which is any whole number (non-decimal).

An integer can be set to any number you want, including random numbers, all from the action menu when creating triggers, it is the action: Set variable = value.

The purpose of this integer variable is to store the random number we retrieve. Simple, right?​

## Chapter 4: How do I make one?

Okay, you can relax now, the hard part is behind you!

Now that you hopefully understood the last 3 chapters, we shall begin with the trigger. In my first example, we will make a trigger for a kobold to drop a scroll of healing with 50% chance drop rate. First make the variable of type integer and name it: KoboldItemDrop, to stay organized.

Now let's start the trigger. Create the event: Unit - A unit of Player 12 (Brown) dies. If the creeps or hostile creatures in your map belong to a different player, change the player that the event is corresponding to. Put the condition that the triggering unit is a kobold.

Now we need to set the variable to a random number between 1 and 100. To do this, go to the Set Variable action and then choose KoboldItemDrop as the variable to set. Go to the value field, and choose Math - Random number between 1 and 100. The reason we must use a variable for this is that if we choose a random number between 1 and 100, it won't be the same number as last time. Once we get our first random number, we will store it so that will be the number we reference to.

Create an if-then-else block. Use Integer Comparison in the if-conditions (this is to compare KoboldItemDrop) and put in the first field the KoboldItemDrop variable.

In the second field, you will put less than or equal to. In the third field, you will put 50

Now under the "Then" actions, put the item dropped at the position of triggering unit (scroll of healing).

Now, our trigger should be something like this:

Kobold Item Drop

Events
Unit - A unit owned by Player 12 (Brown) Dies

Conditions
(Unit-type of (Triggering unit)) Equal to Kobold

Actions
Set KoboldItemDrop = (Random integer number between 1 and 100)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
KoboldItemDrop Less than or equal to 50

Then - Actions
Set ItemDropLoc = Position of (Triggering Unit)

Item - Create Scroll of Healing at ItemDropLoc

Custom script: call RemoveLocation(udg_ItemDropLoc)

Else - Actions
Do nothing

This means that we set the integer to a random number, and if that number is less than or equal to 50 (between 1 and 50) it will drop the item.

This is for 1 item however. If you want 2 or more items to drop, you would need more if-then-else blocks. If you want the items to drop at an individual chance, the if-then-else blocks will be independent, or stand-alone outside of any other blocks. However, if you want only 1 item to drop per creep, you will need a chain of if-then-else blocks. The first "if" will be for the lowest percentage, and then the next block would occur in the "else" section of the first. That "if" would have the next percentage, and so on for all the items in its drop table.

Kobold Item Drop

Events
Unit - A unit owned by Player 12 (Brown) Dies

Conditions
(Unit-type of (Triggering unit)) Equal to Kobold

Actions
Set KoboldItemDrop = (Random integer number between 1 and 100)

Set ItemDropLoc = Position of (Triggering Unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
KoboldItemDrop Less than or equal to 50

Then - Actions
Item - Create Scroll of Healing at ItemDropLoc

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
KoboldItemDrop Less than or equal to 60

Then - Actions
Item - Create <any item you choose> at ItemDropLoc

Else - Actions

Custom script: call RemoveLocation(udg_ItemDropLoc)

Under the final else - actions, you can add more blocks for more items. The thing to notice though is the second condition. If the first condition isn't met, then the remaining numbers that the random can be in is 50 to 100. Thus, the chance for the second item to drop is 10%. This is for single item drops though. For independent drops, this is how it would look like:

Kobold Item Drop

Events
Unit - A unit owned by Player 12 (Brown) Dies

Conditions
(Unit-type of (Triggering unit)) Equal to Kobold

Actions
Set KoboldItemDrop = (Random integer number between 1 and 100)

Set ItemDropLoc = Position of (Triggering Unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
KoboldItemDrop Less than or equal to 50

Then - Actions
Item - Create Scroll of Healing at ItemDropLoc

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
KoboldItemDrop Less than or equal to 60

Then - Actions
Item - Create <any item you choose> at ItemDropLoc

Else - Actions

Custom script: call RemoveLocation(udg_ItemDropLoc)

For the single item drop, the order matters. You want the items with the lowest chance of dropping to drop, but if not that, then drop one of the less rare items. For independent drops, the order doesn't matter. Using this method, however, you can also do single drops. You can simply check if the conditions are met, set a boolean to true, and then just drop a random one out of the items that have the boolean equal to true.​

## Chapter 5:Advanced Options/Triggers

Now that you understand normal item drops, how about we try a bit more advanced triggers?

For example, we can do special things such as make items drop depending on the level of the hero killing the unit.

In the following example, we have a unit called Chest and it revives every 13 seconds. If the level of the hero killing it is 10 and below, then it has a 35% chance to drop a healing potion and a 50% chance to drop a mana potion. However, if the level of the hero is 11 or greater, then there is a 35% chance to drop a greater healing potion and a 50% chance to drop a greater mana potion.

Start off with the standard item drop:

ItemDrop

Events
A unit owned by Player 12 (Brown) Dies

Conditions
(Unit-type of (Triggering unit)) Equal to Chest

Actions
Next, we must make sure that the unit killing the chest is a hero, and you can do that with a boolean comparison.

ItemDrop

Events
A unit owned by Player 12 (Brown) Dies

Conditions
And - All (Conditions) are true
Conditions
(Unit-type of (Triggering unit)) Equal to Chest

(Triggering unit is (A hero)) Equal to True

Actions
Now that that's done, we put again like in normal triggered item drops the "Set Variable: <Variable name>" to a random number between 1 and 100, and then put the if-then-else block again.

ItemDrop

Events
A unit owned by Player 12 (Brown) Dies

Conditions
(Unit-type of (Triggering unit)) Equal to Chest

(Triggering unit is (A hero)) Equal to True

Actions
Set ChestItemDrop = Random Integer between 1 and 100

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions

Then - Actions

Else - Actions

But this time, under the "if- conditions" we put another Integer Comparison, comparing the level of the hero. Click on integer comparison and change the first field like we used to do in Chapter 4, but this time choose Hero - Hero level comparison. Next, change the "Equal to" to "Less than or equal to" in order to represent the "level 10 or lower condition" now, our trigger should look like this:

ItemDrop

Events
A unit owned by Player 12 (Brown) Dies

Conditions
(Unit-type of (Triggering unit)) Equal to Chest

(Triggering unit is (A hero)) Equal to True

Actions
Set ChestItemDrop = Random Integer between 1 and 100

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Hero level of (Triggering unit)) Less than or equal to 10

Then - Actions

Else - Actions

Now, under the "Then - Actions" put another if-then-else block, and now we do EXACTLY the same as we did in Chapter 4, we do the integer comparison of the Set Variable, put the condition that it is less than or equal to 35, and under the 'Then - Actions' Drop small potion of healing, and under the else.....well.....I probably think you now know how to do the rest of items dropped as explained in chapter 4, just put the other item dropped a mana potion at 50% chance.

ItemDrop

Events
A unit owned by Player 12 (Brown) Dies

Conditions
(Unit-type of (Triggering unit)) Equal to Chest

(Triggering unit is (A hero)) Equal to True

Actions
Set ChestItemDrop = Random Integer between 1 and 100

Set ChestTempLoc = Position of (Triggering Unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Hero level of (Triggering unit)) Less than or equal to 10

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ChestItemDrop less than or equal to 35

Then - Actions
Item - Create 1 healing potion at ChestTempLoc

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ChestItemdrop less than or equal to 85

Then - Actions
Item - Create 1 Mana potion at ChestTempLoc

Else - Actions

Else - Actions

Custom script: call RemoveLocation(udg_ChestTempLoc)

After you have finished the above, notice the "Else - Actions" at the very bottom of the trigger. That was the else action of the first if-then-else block of the trigger. That is where we will put the triggers for if they are 11+. Since the first condition stated that the hero level must be less than or equal to 10, the only other levels available are 11+, so no need for any new conditions.

The trigger will end up looking like this:

ItemDrop

Events
A unit owned by Player 12 (Brown) Dies

Conditions
(Unit-type of (Triggering unit)) Equal to Chest

(Triggering unit is (A hero)) Equal to True

Actions
Set ChestItemDrop = Random Integer between 1 and 100

Set ChestTempLoc = Position of (Triggering Unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Hero level of (Triggering unit)) Less than or equal to 10

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ChestItemDrop Less than or equal to 35

Then - Actions
Item - Create 1 Healing Potion at ChestTempLoc

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ChestItemDrop less than or equal to 85

Then - Actions
Item - Create 1 Mana Potion at ChestTempLoc

Else - Actions

Else - Actions
-------- The hero's level is 11 or greater --------

If - Conditions
ChestItemDrop Less than or equal to 35

Then - Actions
Item - Create 1 Greater Healing Potion at ChestTempLoc

Else - Actions
If - Conditions
ChestItemDrop Less than or equal to 85

Then - Actions
Item - Create 1 Greater Mana Potion at ChestTempLoc

Else - Actions

Custom script: call RemoveLocation(udg_ChestTempLoc)

Now the drop chances for 11+ are:

1. Greater Healing Potion : 35%

2. Greater Mana Potion : 50%

Now that is how we do the item drop according to hero level, which you can apply to any of the creeps you want. If you have some sort of class system, you can add more if-then-else blocks and, for example, drop a wand for a mage hero or drop a sword for a warrior hero.

Overall, triggered item drops are much better than the normal item tables. They add for ample control on the situation, and can allow for a lot more creativity.

If you find any of this confusing, just PM me and I'll try to help you out. Enjoy the tutorial and good luck with your item drops.

​

#### Attachments

Trigger editor.PNG

 10.8 KB

 · Views: 6,196



Variable Editor.PNG

 11.6 KB

 · Views: 6,134



set variable.PNG

 4.6 KB

 · Views: 6,211



Condition action.PNG

 4.5 KB

 · Views: 6,230



Kitem.PNG

 5.5 KB

 · Views: 6,095



Math.PNG

 7.3 KB

 · Views: 583



Ifthenelse.PNG

 11.9 KB

 · Views: 6,224



0 and 50.PNG

 6.5 KB

 · Views: 591



scroll of healing.PNG

 15.9 KB

 · Views: 756



Full trig.PNG

 14.9 KB

 · Views: 658



Second random higher.PNG

 22.1 KB

 · Views: 699



Capture.PNG

 26.7 KB

 · Views: 664



Capture1.PNG

 24.1 KB

 · Views: 6,502




 Last edited by a moderator: Jan 21, 2011

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

You can right click on trigger and select copy as text or whatever and paste it here between [trigger][/trigger] tags.

You need to edit tutorial form and fix images!

Also this tutorial can't be approved unit you don't fix your triggers, or should I say memory leaks!

Other that that tutorial is really simple and it was already explained if I remember right!

Only difference is that you explained it very nice!

Please add more options, explain more events and so on:

Example:

Item drop different for different unit levels

Item drop different for different unit types

....

Oh your final trigger don't need that "not equal" even because it is already checked in your first if then else function!

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

Zinox said:

 Now under the else actions, put do nothing if those are the only 2 items you want to drop. BTW, 70-50=20 right? that means by subtracting the 2 numbers you get the percentage of the drop,

That's all for this tutorial, I hope you understood how to make Triggered item drops!

Note: If you have any question, or need some help with it in case something went wrong, PM me, I'll answer you're question.


Click to expand...

There are 101 numbers in [0,100]. You should use [1,100].

You can use Rnd(1,1000) if you want to get smaller than 1% chance to drop some very rare boss items.

I'd use chances like this.

Untitled Trigger 029
Events

Conditions

Actions
Set Int = (Random integer number between 1 and 100)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Int Less than or equal to 50

Then - Actions
-------- Drop item 1 --------

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Int Less than or equal to 80

Then - Actions
-------- Drop item 2 --------

Else - Actions
-------- Drop nothing --------

50% chance to drop item 1, 30% chance to drop item 2, 20% chance to drop nothing.

Or multiple item drops:

Untitled Trigger 029
Events

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Random integer number between 1 and 100) Less than or equal to 40

Then - Actions
-------- Drop item 1 --------

Else - Actions
-------- Drop nothing --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Random integer number between 1 and 100) Less than or equal to 30

Then - Actions
-------- Drop item 2 --------

Else - Actions
-------- Drop nothing --------

40% chance to drop item 1 AND 30% chance to drop item 2.

P(drop something) = 1-P(no drop #1)*P(no drop #2) = 1-.6*.7 = 0,58 -> 58% chance to drop something.

P(no drop) = 1-P(drop something) = 1-0,58 = 0,42 -> 42%

P(drop two items) = P(drop #1)*P(drop #2) = 0,4*0,3 = 0,12 -> 12% chance to drop two items.

#### Zinox

#### Zinox

 Level 4

JoinedSep 24, 2010Messages17

Resources: 3

#### Resources
Icons 1Maps 1Tutorials 1

Ok, i'll fix image mistakes, add a few more triggers to make a few more advanced item drops. Change how the tutorial looks a bit. thanks for the replies, I'll work on it soon

#### Zinox

#### Zinox

 Level 4

JoinedSep 24, 2010Messages17

Resources: 3

#### Resources
Icons 1Maps 1Tutorials 1

Ok, edited the tutorial:


Added an extra chapter on more advanced item dropping


Fixed images with

trigger
 tags


if you found another error/mistake in anything please say so ill fix them right away.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Some of the functions are a little messed up (the indenting) in the trigger tags. (mostly the if/then/elses) You might want to fix that.

Otherwise, this tutorial is looking good. Fix that, and then I'll fix grammar mistakes/organization and it will be ready for approval.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Fixed the grammar and organization, and revamped some things. I fixed the triggers up a bit, optimized here and there, and removed some simple point leaks.


Good job. ~Approved.

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

Zinox said:

ItemDrop

Events
A unit owned by Player 12 (Brown) Dies

Conditions
(Unit-type of (Triggering unit)) Equal to Chest

(Triggering unit is (A hero)) Equal to True

Actions
Set ChestItemDrop = Random Integer between 1 and 100

Set ChestTempLoc = Position of (Triggering Unit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Hero level of (Triggering unit)) Less than or equal to 10

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ChestItemDrop Less than or equal to 35

Then - Actions
Item - Create 1 Healing Potion at ChestTempLoc

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
ChestItemDrop less than or equal to 50

Then - Actions
Item - Create 1 Mana Potion at ChestTempLoc

Else - Actions

Else - Actions
-------- The hero's level is 11 or greater --------

If - Conditions
ChestItemDrop Less than or equal to 35

Then - Actions
Item - Create 1 Greater Healing Potion at ChestTempLoc

Else - Actions
If - Conditions
ChestItemDrop Less than or equal to 50

Then - Actions
Item - Create 1 Greater Mana Potion at ChestTempLoc

Else - Actions

Custom script: call RemoveLocation(udg_ChestTempLoc)

Now the drop chances for 11+ are:


1. Greater Healing Potion : 35%


2. Greater Mana Potion : 50%


Click to expand...


Actually the are


1. Greater Healing Potion : 35%


2. Greater Mana Potion : 15%


The mana potion can only drop if the random number is between 35 and 50. (35 is excluded)

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Whoops, I overlooked that. Fixed.

#### Augustus_Magnus

#### Augustus_Magnus

 Level 7

JoinedJun 2, 2016Messages117

Resources: 6

#### Resources
Maps 6

I'm quite stuck, how dd you create that "Set ItemDropLoc" or something?

#### Augustus_Magnus

#### Augustus_Magnus

 Level 7

JoinedJun 2, 2016Messages117

Resources: 6

#### Resources
Maps 6

Augustus_Magnus said:

 I'm quite stuck, how dd you create that "Set ItemDropLoc" or something?

Click to expand...

#### Attachments

0020.PNG

 20.1 KB

 · Views: 530



#### sashablanche

#### sashablanche

 Level 1

JoinedMay 19, 2017Messages3

yeah me too sadly no reply yet

#### Hotwer

#### Hotwer

 Level 7

JoinedMar 10, 2013Messages366

Set ItemDropLoc is a statement of Set from variable of type Region (Rect). Just use Set on this variable and then select Position of (Triggering Unit)


 Last edited: May 27, 2017

#### Augustus_Magnus

#### Augustus_Magnus

 Level 7

JoinedJun 2, 2016Messages117

Resources: 6

#### Resources
Maps 6

Okay. Thank you.

#### Null

#### Null

 Level 27

JoinedDec 3, 2018Messages909

Resources: 171

#### Resources
Icons 133Skins 6Maps 7Ported Icons 25

This tutorial is very helpful. Thank you !

#### tdrsmynm

#### tdrsmynm

 Level 7

JoinedJul 10, 2016Messages211

Does this have a sample map???

I'm not really good with triggers, and I want to reverse engineer it. Thank you!

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

tdrsmynm said:

 Does this have a sample map???

I'm not really good with triggers, and I want to reverse engineer it. Thank you!


Click to expand...

Check out one of the Founding of Durotar maps, they use pretty much the same system as the one in this tutorial.

#### Rotgut

#### Rotgut

 Level 2

JoinedDec 29, 2019Messages8

Is it new now in reforged that you can simply:

Item - Create (some item) at (Position of (Triggering unit))

Without this set area and then remove area?

#### Nosiaz

#### Nosiaz

 Level 4

JoinedFeb 28, 2020Messages48

I have do this, that was working but now the unit doesnt re-spawn, i have use 1 variable for each mob for doesnt have conflict if i use aldready the same, where my trigger is wrong plz ??

#### Attachments

mob.PNG

 47.3 KB

 · Views: 213



#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

Nosiaz said:

 I have do this, that was working but now the unit doesnt re-spawn, i have use 1 variable for each mob for doesnt have conflict if i use aldready the same, where my trigger is wrong plz ??


Click to expand...

It doesn't respawn because you are not storing it's position after it dies you are simply using (Position of Triggering Unit) now if it was a specific unit ir will work fine but since it's a generic event it won't unless you store each unit's position in a variable so you can revive it later.

#### Nosiaz

#### Nosiaz

 Level 4

JoinedFeb 28, 2020Messages48

"each unit's position" ? A genetif variable i hope ? If this is for all the creature its just gonna be impossible lel, anyway i will try but im very bad in variable i dont really know how to do then, and how to use then...

If someone can tell me more about what i have to do for this work, could be amazing


 Last edited: Mar 4, 2020

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

Nosiaz said:

 "each unit's position" ? A genetif variable i hope ? If this is for all the creature its just gonna be impossible lel, anyway i will try but im very bad in variable i dont really know how to do then, and how to use then...

If someone can tell me more about what i have to do for this work, could be amazing


Click to expand...

I'd recommend seperating the creep respawn from the item drop to make things a lot more easier. Here's a good GUI creep respawn system that you may find useful : Creep Respawn (GUI)

#### Nosiaz

#### Nosiaz

 Level 4

JoinedFeb 28, 2020Messages48

its a great GUI, its work well, thx

#### josquin111

#### josquin111

 Level 2

JoinedAug 28, 2020Messages19

Can I put more than one triggering unit in condition? Like if my Kobold and KoboldMage have the same item drops can i just put them along to save a shit ton of time

I also can't seem to find the ''position'' of triggering unit in value. I set my variable to Region and I can't find it


 Last edited: Jan 11, 2021

#### PaulieD

#### PaulieD

 Level 2

JoinedAug 22, 2020Messages18

If I have a random unit (40% Wendigo and 60% Wendigo Shaman) and I want only Wendigo Shaman drop one of three possible items (Tome of Str, Agi, or Int), how would the trigger look like? Will appreciate any help

And to mention, I don't need them to respawn, only want to avoid that the item will be dropped from regular Wendigo...

 You must log in or register to reply here.


### Similar threads

 Item-drop-system


SlayerII

Mar 6, 2009

Trigger (GUI) Editor Tutorials

Replies24Views20K

Feb 28, 2010
SlayerII

 ILS (Item Level Up System)


-Kobas-

Jul 24, 2010

Trigger (GUI) Editor Tutorials

Replies11Views23K

Jul 13, 2011
Ignitedstar

 Basic drop sistems


alhadrian

Aug 30, 2010

Tutorial Archive

Replies7Views3K

Dec 11, 2010
PurgeandFire

 IDS (Infinite Number of Item Classes)


-Kobas-

Mar 29, 2010

Trigger (GUI) Editor Tutorials

Replies25Views47K

Oct 7, 2015
BPower

 TaaZ's Item System


TaaZ

Jul 27, 2005

Trigger (GUI) Editor Tutorials

Replies3Views15K

Jun 3, 2019
Cespie

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