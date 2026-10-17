---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/effortless-and-efficient-unit-group-functions-for-gui-triggers.337325/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Effortless and Efficient Unit Group Functions for GUI Triggers
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/effortless-and-efficient-unit-group-functions-for-gui-triggers.337325/>
#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

##
Effortless and Efficient Unit Group functions for GUI Triggers

### Background​

 This is a tutorial for GUI users to learn how to use the new natives for Unit Group functions while mapmaking. A lot of people, I assume, are still using GUI triggers because they don't have more time to devote to learning a script. As someone who started off using GUI for years and relying on systems written for GUI, I know that people stick with GUI because it is intuitive and faster for doing simple things. If you have time to learn about the JASS script, I don't support staying with GUI, but I understand the decision nonetheless. Hopefully, after this tutorial, GUI users will be able to benefit from the new natives given to us all, and further stay away from the hilariously inefficient Unit Group actions that are provided by WE.

### ​

### Requirements & Notes before Reading​

I assume you know how to use the "Custom Script" action in GUI

In this tutorial I will use the variable names with and without the udg_ prefix interchangeably. That is because variables created in GUI are global, and hence have the udg_ prefix in "Custom Script".

### ​

### Doing Unit Group functions​

Natives you need:
BlzGroupGetSize takes group whichGroup returns integer => its syntax is BlzGroupGetSize(group)

BlzGroupUnitAt takes group whichGroup, integer index returns unit => its syntax is BlzGroupUnitAt(group,integer)

Variables you need:
A group variable: TempGroup - of course

2 integer variables: TempCount - the number of units in the unit group; TempIndex - the indexing element inside the group

A unit variable: TempEnum - equivalent to (Picked unit) in the unit group

How to:

Set the first variable: set udg_TempCount = BlzGroupGetSize(udg_TempGroup)

Create a For "each TempIndex variable" loop

Set the number of loops to TempCount

Inside the For loop: set udg_TempEnum = BlzGroupUnitAt(udg_TempGroup,udg_TempIndex-1)

It is done. Now inside each loop, udg_TempEnum will be the (udg_TempIndex-1)th unit from udg_TempGroup. You can do the same things to udg_TempEnum as you would with (Picked unit) inside a "Pick Every Unit And..." loop

Explanation:
BlzGroupGetSize is the faster way to count a unit group than the GUI function (which is actually a For loop function that counts units in the group one by one)

BlzGroupUnitAt picks out the Xth unit from a unit group. This function is zero-indexed, however, so the first unit will use index 0, the second unit will use index 1, and so on.

### ​

### Removing Units from the Unit Group?​

Before proceeding, bear in mind: If you do Unit Group functions this way, removing units will not be as straightforward as the case with the other GUI function. You shouldn't attempt to remove udg_TempEnum from the group mid-loop, because it will shift the indices of the remaining units backwards, causing some units to be missed in the loop.

Accessory natives you have:
BlzGroupRemoveGroupFast takes group whichGroup, group removeGroup returns integer => its syntax is BlzGroupRemoveGroupFast(group1,group2)

How to:

An accessory group variable: GroupToRemove

Inside the For loop: Add TempEnum to GroupToRemove

After the For loop is finished: call BlzGroupRemoveGroupFast(udg_GroupToRemove,udg_TempGroup)

Clear the GroupToRemove group.

Explanation:
BlzGroupRemoveGroupFast excludes all units from group2 that belong in group1. group1 is the one that remains the same and group2 is the one that loses the units. This function, as the name suggests, is faster than the GUI counterpart (which is also a loop that removes units one by one).

### ​

### Random Unit from Unit Group​

Using the new natives, it's also very simple to make a more efficient "Random Unit from Unit Group" function.
Basically, after setting the variable for the group's number of units, get a random number between 1 and that variable.

Then, use BlzGroupUnitAt to get the unit with that number in the unit group. And of course, since the function is zero-indexed, we move the number back by one.

How to:

### ​

### Additional remarks​

The opposite of BlzGroupRemoveGroupFast, BlzGroupAddGroupFast also exists, using the same syntax.


 Last edited: Jan 19, 2022

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Variables you need image can be cropped down into 1 5th of the height.

Or just display in text form, I think it's fair to say that anyone who is looking for this information knows how to create a variable.

Another suggestion, but not a demand/requirement

Given that you'd probably use this many times in a project, it might be worth making a 'system' for this.

Something along the lines of:

set tempGroup = whatever group

run trigger "TriggerThatContainsTheCustomScript"

GUI loop here

This would reduce the amount of custom scripts which might be desired.

Otherwise.. seems good to me.

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

Changes made.

Also I don't think a system is necessary for something this simple, because outside the GUI loop, only 1 function call is made for TempCount.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Well, calling it a system is overselling it anyway. It's just wrapping the custom script with a trigger.

This is common practice in GUI friendly systems, but as I said, it's not required.

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

Added a section about getting random unit from a group!

 You must log in or register to reply here.


### Similar threads

 [GUI] Advanced Triggering Tips


Bribe

Mar 1, 2011

Trigger (GUI) Editor Tutorials

Replies23Views27K

Dec 20, 2012
PurgeandFire

 Convenient Unit Group Filtering in GUI


KILLCIDE

Apr 23, 2016

Trigger (GUI) Editor Tutorials

Replies27Views12K

Dec 10, 2021
Darth Atroxa

Locked

 GUI unit group questions


IsaacNewbton

Apr 1, 2012

Triggers & Scripts

Replies5Views2K

Apr 2, 2012
IsaacNewbton

Locked

[Solved]
 Help me understand loops and actions per unit in group


SoooK

Feb 23, 2019

World Editor Help Zone

Replies9Views2K

Feb 24, 2019
IcemanBo

Locked

 How to destroy unit group with index (GUI)


jareph

Jun 22, 2008

World Editor Help Zone

Replies3Views4K

Jun 22, 2008
jareph

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