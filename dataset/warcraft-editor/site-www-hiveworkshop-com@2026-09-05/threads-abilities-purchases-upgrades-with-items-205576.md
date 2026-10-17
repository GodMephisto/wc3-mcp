---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/abilities-purchases-upgrades-with-items.205576/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Abilities Purchases Upgrades with Items
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/abilities-purchases-upgrades-with-items.205576/>
#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

# Abilities

Purchases & Upgrades with Items

This tutorial will teach you on how to purchase and upgrade abilities from shops, via items. This is heavily (ab)used in "Custom Hero" maps, where you can completely customize your Hero's spells. This system is fully MUI and MPI - all players can use it with unlimited count of different, or same, units.

Suggested readings are at the bottom of the page.

## Content of tutorial:
I - Setting the Data

II - Variables

III - Buy & Upgrade Trigger

### I - Setting the Data

First of all, before we go with the Triggers, we will need to declare some Object Data. If you already have your Abilities and purchasable Items, I still recommend you to read through this.

Okay, what we will need first are some skills. Go ahead and create few. In this tutorial, I will be using 4 skills - Barrage, Command Aura, Chain Lightning and a custom one, Death Gaze. I guess you already set your number of levels, mana cost, effects and other. That was easy enough, right? Make sure to check the requirements, since some Ladder abilities, such as Barrage, require certain upgrades to be used.

 Hint: You don't need to set the Learn tooltips! These abilities can only be upgraded via triggers. Also, use Auto Fill option (Right Click on a field, -Auto Fill Levels-) to make everything faster and easier!

Now, let's create some items. The items should carry the icon and description of the skill, and they should also mention what is upgraded with each level. Only one item per one skill is allowed. It is advised to not put any stock cooldown, and to remove the model of the item; because you don't want those small lost particles lingering around.

 Hint: Create items with the Classification of "Power Up"! Power up items can be picked even if your inventory is full, like Tomes or Runes, thus allowing you to upgrade skills even if your inventory is full. Be careful to remove the model - Power Up items leave those ugly particles.

### II - Variables

Now when we have our Abilities and Items, we need to set the Variables. Go to the Trigger Editor, enter the Variables, and then create five new variables, named "Skills", "MaxLevel", "Items", "TempUnit" and "TempPlayer". Last two will be used later.

Skills' type is Ability, Initial value of None, Array is True, and size is 1.

MaxLevel's type is Integer, Initial value of None, Array is True, and size is 1.

Items' type is Item-Type, Initial value of None, Array is True, and Size is 1.

TempUnit's type is Unit, Initial value of None, Array is False.

TempPlayer's type is Player, Initial value of None, Array is False.

Okay, now create a new trigger with an event at Map Initialization. The triggering part is easy now - you will just set the Variables that suit your needs, like I did in there.

Variables
Events
Map initialization

Conditions

Actions
Set Skills[0] = Barrage

Set Items[0] = Barrage

Set MaxLevel[0] = 12

Set Skills[1] = Command Aura

Set Items[1] = Command Aura

Set MaxLevel[1] = 20

Set Skills[2] = Chain Lightning

Set Items[2] = Chain Lightning

Set MaxLevel[2] = 6

Set Skills[3] = Death Gaze

Set Items[3] = Death Gaze

Set MaxLevel[3] = 3

 Hint: You can even add a Description variable, which will inform the user about the spell's increments per level. To do this, simply write down everything in a String type of variable, and display it by following the guide I provided bellow.

### III - Buy & Upgrade Trigger

Great. Now, when we stored the variables, we need a new trigger to check when an item is bought, and to increase the level of the ability. We will also check if the ability is at maximum level, and return the cost to player, as well as displaying proper messages. Here we use the TempUnit and TempPlayer variables to avoid leaks.

Buy and Upgrade Skills
Events
Unit - A unit Acquires an item

Conditions
(Item-class of (Item being manipulated)) Equal to Powerup

Actions
Set TempUnit = (Triggering Unit)

Set TempPlayer = (Owner of (Triggering Unit))

For each (Integer A) from 0 to 3, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Items[(Integer A)] Equal to (Item-type of (Item being manipulated))

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Skills[(Integer A)] for TempUnit Less than 1

Then - Actions
Game - Display to TempPlayer for 1.00 seconds the text: TempPlayer + ( bought + (Name of (Item being manipulated))))

Unit - Add Skills[(Integer A)] to TempUnit

Skip remaining actions

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Skills[(Integer A)] for TempUnit) Less than MaxLevel[(IntegerA)]

Then - Actions
Game - Display to TempPlayer for 1.00 seconds the text: TempPlayer + ( upgraded + (Name of (Item being manipulated))))

Unit - Set level of Skills[(Integer A)] for TempUnit to ((Level of Skills[(Integer A)] for TempUnit) + 1)

Skip remaining actions

Else - Actions
Game - Display to TempPlayer for 2.00 seconds the text: Skill is at maximum level!

Player - Add 50 to TempPlayer Current lumber

Else - Actions

Explanation: The system detects when item is bought, and is that item from the class of Power Up. For Each Integer from 0 to X, optimizes the trigger along with Integer A, making it easier to modify later. X stands for the last Array number of your abilities. In this case, the Death Gaze is last, so it should be "from 0 to 3". The system checks if unit doesn't have the ability, and adds it to it if it returns true. If it has the ability, but it's not at maximum level, it upgrades the skill and displays the message. If the skills is at maximum level, the system displays the message and returns the cost.

We are finished! You will need to add variables for each ability, along with corresponding item, maximum level and the description, if you decide to go with it.

I hope that this tutorial helped you in acomplishing your goal, and I am looking forward to see your maps soon!

Suggested Readings:

https://www.hiveworkshop.com/forums/trigger-gui-editor-tutorials-279/array-tutorial-17789/

https://www.hiveworkshop.com/forums...quick-tutorial-common-triggering-tips-190802/

https://www.hiveworkshop.com/forums/general-mapping-tutorials-278/complete-list-things-leak-126761/

My Stuff:

Current project: https://www.hiveworkshop.com/forums/map-development-202/legendary-war-apheraz-181100/

Back to the Top


 Last edited: Dec 1, 2011

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

You should store Triggering unit and item type of item into variables. All players matching condition leaks a force.

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

I thought that the (All Players) does not leak.

11:39 AM Update: Stored Unit and Player within a variable.

04:42 PM Update: Fixed few typos.


 Last edited: Oct 18, 2011

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

All players doesn't leak, All players matching condition does.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

You should put the GUI triggers inside hidden tags

Using (Integer A) is bad (Using a global as an index for iteration? WTF Blizzard.)

You should use a new integer variable instead.

(Owner of (Triggering unit)) -> Triggering Player

Your trigger should look like this:

Buy and Upgrade Skills
Events
Unit - A unit Acquires an item

Conditions
(Item-class of (Item being manipulated)) Equal to Powerup

Actions
Set TempUnit = (Triggering Unit)

Set TempPlayer = (Triggering Player)

Set TempItem = (Item being manipulated)

Set TempType = (Item-type of TempItem)

Set TempString = (Name of TempItem)

For each TempInt from 0 to 3, do (Actions)
Loop - Actions
Set TempInteger2 = (Level of Skills[TempInt] for TempUnit)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Items[TempInt] Equal to TempType

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
TempInteger2 Less than 1

Then - Actions
Game - Display to TempPlayer for 1.00 seconds the text: TempPlayer + ( bought + TempString))

Unit - Add Skills[TempInt] to TempUnit

Skip remaining actions

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
TempInteger2 Less than MaxLevel[TempInt]

Then - Actions
Game - Display to TempPlayer for 1.00 seconds the text: TempPlayer + ( upgraded + TempString)

Unit - Set level of Skills[TempInt] for TempUnit to (TempInteger2 + 1)

Skip remaining actions

Else - Actions
Game - Display to TempPlayer for 2.00 seconds the text: Skill is at maximum level!

Player - Add 50 to TempPlayer Current lumber

Else - Actions

I hope this helps

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

Well, Integers A & B won't leak, and I didn't want to confuse the new users with explanations of custom integers and integers, plus arrays and their functionality... Ugh! That's in suggested readings.

This tutorial covers the very basics, and as such, I kept the thing's complicity at minimum level. It helps, if anyone wants to go forward a little advanced, but same, mode.

Oh, and thanks for suggestion to put the Triggers in the Hidden Tags.

So, the new update now covers the Hidden Tags for the Triggers, a note that the system is MUI and MPI and few more typo fixes.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice tutorial. It is well formatted. The centering makes it a bit harder to read but it is probably just due to my gigantic wide monitor. ;P ~Approved.

#### Greenwhy

#### Greenwhy

 Level 7

JoinedSep 9, 2007Messages253

Resources: 1

#### Resources
Icons 1

Apheraz Lucent said:

 Skills' type is Ability, Initial value of None, Array is True, and size is 1.

MaxLevel's type is Integer, Initial value of None, Array is True, and size is 1.

Items' type is Item-Type, Initial value of None, Array is True, and Size is 1.


Click to expand...

shouldn't the array sizes be 3 for the following?

Items[3]

MaxLevel[3]

Set Skills[3]

because you are using:

Set Skills[0]

Set Skills[1]

Set Skills[2]

Set Skills[3]

etc


 Last edited: Dec 1, 2011

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

I edited the tutorial to remove the center tags, achieving better visibility, as PurgeandFire111 suggested me. Oh, and feel free to post me a Visitor Message, or a new post in this thread when you edit what you wanted to write.

@Greenwhy,

The arrays are left free at the unlimited amount, so that you don't need to change the variables every time you want to implement a new combination.

#### Greenwhy

#### Greenwhy

 Level 7

JoinedSep 9, 2007Messages253

Resources: 1

#### Resources
Icons 1

I didn't know an array size of 1 makes it unlimited? does this mean if I set the array size to 1 I can use triggers to refer to any integer for that array and it will work? It doesnt say that in the Array Tutorial....

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

Arrays automatically reserve some memory, they always have 8192 indexes. The initial size sets the value of the indexes to the given value.

For example if you have an integer array, you set initial value to 5 and initial size to 3, indexes from 0 to 3 are set to value 5. Other indexes will hold value 0.

If you leave initial size to 1, indexes from 0 to 1 will be initialized with the default value.

So the indexes work, you don't have to set the initial size.

For some types like timers, dialogs and triggers it can be useful to set the initial size if you use an array. By default, the indexes ill be initialized for indexes 0 and 1. So if you tried to use dialog[2] it would not work. You'd need to set initial size to 2 so it would then create a dialog for that index. I usually create those with triggers.

#### Greenwhy

#### Greenwhy

 Level 7

JoinedSep 9, 2007Messages253

Resources: 1

#### Resources
Icons 1

I think I understand, this should be added to the array tutorial I think. I always set my array size to how many indexes (indices?) I was going to use in that array even though I often set the initial values manually before they are called.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

 indexes (indices?)


Click to expand...

Yes, indices.

That array tutorial isn't really a reliable one.

All you need to know about arrays is that the minimum index is 0, the maximum index is 8191 (8190 is used as the max because replays are buggy with index 8191), and arrays in warcraft III are dynamic.

That's pretty much it. It doesn't need a tutorial.

#### Greenwhy

#### Greenwhy

 Level 7

JoinedSep 9, 2007Messages253

Resources: 1

#### Resources
Icons 1

Game - Display to TempPlayer for 2.00 seconds the text: Skill is at maximum level!
I can't find this action. I can only find "Game - Text Message" which can be sent to a player group (or convert player to player group).


How do you send a message to only one player without converting to a player group?

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

Um... I set the player in a variable, and display it then.

#### Greenwhy

#### Greenwhy

 Level 7

JoinedSep 9, 2007Messages253

Resources: 1

#### Resources
Icons 1

This must be a really dumb question.... The only way I have found to display a message to a single player is like this....

Game - Display to (Player group(TempPlayer)) the text: max level
What am I missing? I can't find a trigger action which sends a message to only 1 player.

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

I typed out all these triggers by myself, and it's certain that I made mistakes somewhere. Thanks.

#### Greenwhy

#### Greenwhy

 Level 7

JoinedSep 9, 2007Messages253

Resources: 1

#### Resources
Icons 1

Ahhhh ok. I was excited because I thought there was an easier way to message a single player. I will continue using the "convert player to player group" method.

#### The Wanderer

#### The Wanderer

 Level 4

JoinedNov 25, 2013Messages132

this is a very useful tutorial for beginners like me! i think it should include a tutorial on how to remove an item if the level of the purchased ability reaches the max level, for purchasable class abilities purposes.

#### Ayrayen

#### Ayrayen

 Level 5

JoinedAug 6, 2009Messages136

Is there a Demo map where I can test this out?


Thanks in advance!

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

Will make one by the end of the day, and update the tutorial as well with some new content

#### giakyo

#### giakyo

 Level 2

JoinedJan 28, 2015Messages4

Resources: 1

#### Resources
Maps 1

this is a very useful tutorial for beginners like me!

thx bro Apheraz Lucent

But i can not finish this triggers. I do this

Buy and Upgrade Skills

 Events

 Unit - A unit Acquires an item

 Conditions

 (Item-class of (Item being manipulated)) Equal to Powerup

 Actions

 Set TempUnit = (Triggering Unit)

 Set TempPlayer = (Owner of (Triggering Unit))

 For each (Integer A) from 0 to 3, do (Actions)

 Loop - Actions

 If (All Conditions are True) then do (Then Actions) else do (Else Actions)

 If - Conditions

 Items[(Integer A)] Equal to (Item-type of (Item being manipulated))

 Then - Actions

 If (All Conditions are True) then do (Then Actions) else do (Else Actions)

 If - Conditions

 (Level of Skills[(Integer A)] for TempUnit Less than 1

 Then - Actions

 Game - Display to (Player group(TempPlayer)) for 1.00 seconds the text: ((Name of TempPlayer) + ( bought + (Name of (Item being manipulated))))

 Unit - Add Skills[(Integer A)] to TempUnit

 Skip remaining actions

 Else - Actions

 If (All Conditions are True) then do (Then Actions) else do (Else Actions)

 If - Conditions

 (Level of Skills[(Integer A)] for TempUnit) Less than MaxLevel[(IntegerA)]

 Then - Actions

 Game - Display to (Player group(TempPlayer)) for 1.00 seconds the text: ((Names of TempPlayer) + ( upgraded + (Name of (Item being manipulated))))

 Unit - Set level of Skills[(Integer A)] for TempUnit to ((Level of Skills[(Integer A)] for TempUnit) + 1)

 Skip remaining actions

 Else - Actions

 Game - Display to (Player group(TempPlayer)) for 2.00 seconds the text: Skill is at maximum level!

 Player - Add 50 to TempPlayer Current lumber

 Else - Actions

@to anyone that

I have a small problem. Can you help me, plzz?

Example: My hero has 10 potions, how I can throw out 1, so now my hero has 9 potions and 1 potion in located at the foot.

Ex2: My hero has 2 potions in located nearby. How to pick all but it is not grouped into one. I mean: my hero has 1 potion in slot1 and 1 potion in slot2, although they are the same.I have multiple copies of the same item so I can have the same item six times.

My language is translated from translate.google.com (sorry if you see any inconvenience)

Thx !!


 Last edited: Aug 2, 2015

#### Zordrak

#### Zordrak

 Level 8

JoinedDec 19, 2014Messages300

Resources: 1

#### Resources
Maps 1

Wonderful, perfect for my map!

#### Alexus_Kun

#### Alexus_Kun

 Level 1

JoinedJun 1, 2022Messages1

Hello, a question that is barrage? I don't have much knowledge of this but I wish I could create this system on my map

 You must log in or register to reply here.


### Similar threads

 Buying abilitys


Parrothead

Aug 8, 2007

Trigger (GUI) Editor Tutorials

Replies38Views36K

Jul 22, 2018
Razorburst

Locked

[Trigger]
 Added Hero Abilities


Aka Gambit

Dec 5, 2012

Triggers & Scripts

Replies17Views2K

Dec 6, 2012
Arhowk

 Aquiring Abilities with Items


PiCkstix

Dec 9, 2007

Trigger (GUI) Editor Tutorials

Replies3Views11K

Dec 12, 2007
PiCkstix

 ILS (Item Level Up System)


-Kobas-

Jul 24, 2010

Trigger (GUI) Editor Tutorials

Replies11Views23K

Jul 13, 2011
Ignitedstar

 Item-drop-system


SlayerII

Mar 6, 2009

Trigger (GUI) Editor Tutorials

Replies24Views20K

Feb 28, 2010
SlayerII

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