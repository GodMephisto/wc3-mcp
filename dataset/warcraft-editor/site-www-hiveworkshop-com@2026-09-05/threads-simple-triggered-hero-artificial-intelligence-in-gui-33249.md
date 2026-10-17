---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/simple-triggered-hero-artificial-intelligence-in-gui.33249/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Simple Triggered Hero Artificial Intelligence in GUI
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/simple-triggered-hero-artificial-intelligence-in-gui.33249/>
#### DenZel94

#### DenZel94

 Level 7

JoinedJun 4, 2006Messages127

Resources: 7

#### Resources
Maps 3Tutorials 4

Contents

I. Some things to know about

 1. Uses of AI Triggers

 2. Difference of Triggered and AI Editor Scripts

II. The Hero Triggers

1.Creating the Hero

2.Learning and Using of Abilities

3.Attacking, Chasing and Retreating

4.Item System

I. Some Things to Know About

1. Uses of AI Triggering

AI triggering is for advanced Artificial Intelligence to those maps which the AI Editor cannot be applied.

Common genres that uses Triggered AI are AoS, Hero Arena, Hero Defense, RPG (Role Playing Game), etc.

Other things that aren't included (Tags, TDs (Tower Defense), TWs (TowerWars), Maul, CD's (Castle Defense) but aren't purposed for Triggered AI is because that these kind of games is too long and these kind of genres cannot be practiced even with AI Players.

2. Difference of Triggered and AI Editor Scripts

Triggered AI shows more advanced mechanics on the Heroes and its the map makers choice on what he wants to do with the Artificial Intelligence.

As of AI Scripts, it's mechanics is equal to every unit than just caring for the Hero.

AI Scripts are most used in Melee and Campaign maps.

II. The Hero Triggers

1. Creating the Hero

First, we have to create the Hero for the AI, depending on the style of how to get the hero.

Some styles are using a building to upgrade it to a hero, taverns and arenas.

The following codes are at the bottom and you can see if it is for its style (this trigger also gives random heroes of choice).

You can add some stuff like the ff. in the actions:

Actions
Player Group - Pick every Player in (All Players) and do Player - Make [Hero-Type] Unavailable for training/constructing for (Picked Player)

Game - Display to (All Players) the text: (Name of Player (#) + picked + [Name of your Hero]

Hero - Create [item] and give it to (Last Created Unit)

[I]Building Upgrade to Hero[/I]
Events
Time - Elapsed game time is [...] seconds

Conditions
(Player [#](color) controller) Equal to Computer

Actions
If (All Condition are true) then do (Actions) Else do (Else Actions)
If - Conditions
(Random integer between 1 and 2) Equal to 1

Then - Actions
Set AIHero[Array#] = (Building Of Player[#])

Unit - Order [Player [#]'s building] to train/upgrade to a [desired hero]

[If you would like to add something here, then add it]

Else - Actions
Unit - Order [Player [#]'s building] to train/upgrade to a [desired hero]

[If you would like to add something here, then add it]

[I]Hero Tavern[/I]
Events
Time - Elapsed game time is [...] seconds

Conditions
(Player [#](color) controller) Equal to Computer

Actions
If (All Condition are true) then do (Actions) Else do (Else Actions)
If - Conditions
(Random integer between 1 and 2) Equal to 1

Then - Actions
Set Point = (Random Point of (Region))

Unit - Create 1 [Hero] for [Player (#)] at [Point] facing [Default Facing Degrees]

Custom script: call RemoveLocation(udg_Point)

Else - Actions
Set Point = (Random Point of (Region))

Unit - Create 1 [Hero] for [Player (#)] at [Point] facing [Default Facing Degrees]

Custom script: call RemoveLocation(udg_Point)

Set AIHero[Array#] = (Last Created Unit)

[I]Hero Arena[/I]
Events
Time - Elapsed game time is [...] seconds

Conditions
(Player [#](color) controller) Equal to Computer

Actions
If (All Condition are true) then do (Actions) Else do (Else Actions)
If - Conditions
(Random integer between 1 and 2) Equal to 1

Then - Actions
Set BlaHeroLoc = (Random Point in (Region))

Unit - Order [Hero Picker of Player (#)] to Move to [BlaHeroLoc]

Custom script: call RemoveLocation(udg_BlaHeroLoc)

Else - Actions
Set BlaHeroLoc = (Random Point in (Region))

Unit - Order [Hero Picker of Player (#)] to Move to [BlaHeroLoc]

Custom script: call RemoveLocation(udg_BlaHeroLoc)

Setting the Variable to the Arena.. Add these Actions

Actions
[Add your Actions Here]

If (Owner of (Entering Unit)) Equal to (Player[#]) and (Player[#] controller) Equal to Computer then do Set AIHero[Array#] = (Last Created Unit) else do Do Nothing

2. Learning and Using of Abilities

Now, what we need is a trigger that makes the Hero Artificial Intelligence to Learn Abilities and know when to use some abilities, especially the triggered ones.

The order of learning of abilities is the best ability (Most Ultimate) to the Least Best (Attribute Bonus or the weakest ability). Even though the Ultimate has a level requirement, it means the AI will try to Learn the Ultimate first than the others, meaning, at level 6 or something, instead of learning the other abilities, the Hero will learn the Ultimate.

[I]Learn Abilities[/I]
Events
Unit - A unit Gains a Level

Conditions
(Player [#](color) controller) Equal to Computer

(Owner of (Leveling/Triggering Unit) Equal to Player [#](color)

Actions
Hero - Learn skill for (Leveling/Triggering Unit): [Strongest Ability/Ultimate of First Hero]

Hero - Learn skill for (Leveling/Triggering Unit): [Strongest Ability/Ultimate of Second Hero]

Hero - Learn skill for (Leveling/Triggering Unit): [Moderate Strong Ability of First Hero]

Hero - Learn skill for (Leveling/Triggering Unit): [Moderate Strong Ability Second Hero]

Hero - Learn skill for (Leveling/Triggering Unit): [Moderate Weak Ability of First Hero]

Hero - Learn skill for (Leveling/Triggering Unit): [Moderate Weak Ability of Second Hero]

Hero - Learn skill for (Leveling/Triggering Unit): [Weakest Ability of First Hero]

Hero - Learn skill for (Leveling/Triggering Unit): [Weakest Ability of Second Hero]

You can also make the Hero randomly learn abilities with the If/Then/Else Actions.

Also remember that this can choose 2 random heroes so you better put all of their abilities in.

*=Optional

[I]Usage of Abilities[/I]
Events
Time - Every 1 seconds of the game

Conditions
(Player [#](color) controller) Equal to Computer

Actions
Set VarUnitGroup = (Units within [distance] of (Position of (AIHero[Array#]) matching (((Matching Unit) is Alive) Equal to True) and ((Matching Unit) belongs to an enemy of (Owner of (AIHero[Array#])) Equal to True) and (((Matching Unit) is A Hero) Equal to True)

Unit - Pick every unit in VarUnitGroup then do (Actions) else do (Else Actions)
Loop - Actions
If (All Condition are true) then do (Actions) Else do (Else Actions)
If - Conditions
(Integer((Life of (Picked Unit)))) Less than or Equal to [desired life]

Then - Actions
Unit - Order [Hero Picker of Player (#)] to [Ability] [to (Picked Unit)/(Position of (Picked Unit))]

Custom script: call DestroyGroup(udg_VarUnitGroup)

Else - Actions
Custom script: call DestroyGroup(udg_VarUnitGroup)

I'm not quite sure about Custom Script, because the Variable is still going to be used again, so better use the other setting of the variable.

3. Attacking, Chasing and Retreating

Lastly, we need the AI Hero to farm, have intelligence to chase heroes and also intelligence when to retreat.

There are many kinds of attacking depending on genres, there is lanes, arenas, defense.

(Feature on defending base on AoS and Hero Defense Genre AI will be added soon).

[I]Attacking - Lanes Part One[/I]
Events
Time - Every 1 seconds of the game

Conditions
(Player[#](color) controller) Equal to Computer

Actions
Set RandomLane[Array#] = (Random integer from 1 and 3)

Unit Group - Pick every unit in [Base Region] and do (If (Percentage Life of (Picked Unit) Greater than or Equal to 85) and ((Owner of (Picked Unit)) Equal to (Player[#])) then do (If RandomLane[Array#] Equal to 1) then do (Unit - Order (Picked Unit) to Attack - Move to (Center of [Mid Of Lane 1]) else do (If RandomLane[Array#] Equal to 2) then do (Unit - Order (Picked Unit) to Attack - Move to (Center of [Mid Of Lane 2]) else do (Unit - Order (Picked Unit) to Attack - Move to (Center of [Mid Of Lane 3])

If RandomLane[Array#] Equal to 1 then do Turn On (Charging - After Locking - Lanes[1] Part One) else do Do Nothing

If RandomLane[Array#] Equal to 1 then do Turn On (Attacking - After Casting Spells - Lanes[1] Part One)

If RandomLane[Array#] Equal to 3 then do Turn On (Charging - After Locking - Lanes[3] Part One) else do Do Nothing

If RandomLane[Array#] Equal to 3 then do Turn On (Attacking - After Casting Spells - Lanes[3] Part One)

We have to create a Integer Variable Lane Randomizer because using the "Chance" style will sometimes not work.

You can find out whats the use of the Turn On triggers after reading Attacking.

[I]Attacking - Lanes Part Two[/I]
Events
Unit - A unit enters Mid Of Lane1

Unit - A unit enters Mid Of Lane2

Unit - A unit enters Mid Of Lane3

Conditions
(Player[#](color) controller) Equal to Computer

Actions
Set EnemyBase = (Center of (Enemy Base)

Unit Group - Pick every unit in (Units within 500 of Mid Of Lane1) and do (If ((Owner of (Picked Unit)) Equal to (Player[#]))) then do (Unit - Order (Picked Unit) to Attack - Move to (EnemyBase) else do Do Nothing

Unit Group - Pick every unit in (Units within 500 of Mid Of Lane2) and do (If ((Owner of (Picked Unit)) Equal to (Player[#]))) then do (Unit - Order (Picked Unit) to Attack - Move to (EnemyBase) else do Do Nothing

Unit Group - Pick every unit in (Units within 500 of Mid Of Lane3) and do (If ((Owner of (Picked Unit)) Equal to (Player[#]))) then do (Unit - Order (Picked Unit) to Attack - Move to (EnemyBase) else do Do Nothing

Trigger - Turn On (Charging - After Locking - Lanes Part Two)

Trigger - Turn On (Attacking - After Casting Spells - Lanes Part Two)

Trigger - Turn Off (Attacking - After Casting Spells - Lanes[1] Part One)

Trigger - Turn Off (Attacking - After Casting Spells - Lanes[3] Part One)

Trigger - Turn Off (Charging - After Locking - Lanes[1] Part One)

Trigger - Turn Off (Charging - After Locking - Lanes[3] Part One)

Custom script: call RemoveLocation(udg_EnemyBase)

An example of AoS Lane Attacking, this trigger has 3 lanes. Mid Of Lane is not a variable, it is a region.

Put this region in the Middle of each lane.

[I]Attacking - Arena[/I]
Events
Time - Every 1 seconds of the game

Conditions
(Player[#](color) controller) Equal to Computer

Actions
Set RandomPointArena = (Random Point in (Arena Region))

Unit Group - Pick Every Unit in [Fountain Spot for Player[#]) and do (If (Percentage Life of (Picked Unit) Greater than or Equal to 85) and ((Owner of (Picked Unit) Equal to Player[#]) then do Unit - Order AIHero(Array#) to Attack-Move to Random Point of [RandomPointArena] else do Do Nothing

Custom script: call RemoveLocation(udg_RandomPointArena)

This is the most simplest Attacking Trigger for AI in Arena. I'm not sure if there are still many.

[I]Attacking - After Casting Spells - Lanes[1] Part One (Initially On)[/I]
Events
Unit - A unit Starts the Effect of an Ability

Conditions
(Owner of (Triggering Unit)) Equal to (Player[#](color))

(Player[#](color) controller) Equal to Computer

Actions
Set MidLane[1] = (Center of (Mid Lane 1))

Unit - Order (Triggering Unit) to Attack Move to (Mid of Lane1)

Custom script: call RemoveLocation(udg_MidLane[1])

Lane1 = West; Lane2 = Middle; Lane3 = East

[I]Attacking - After Casting Spells - Lanes[3] Part One (Initially On)[/I]
Events
Unit - A unit Starts the Effect of an Ability

Conditions
(Owner of (Triggering Unit)) Equal to (Player[#](color))

(Player[#](color) controller) Equal to Computer

Actions
Set MidLane[3] = (Center of (Mid of Lane3)

Unit - Order (Triggering Unit) to Attack Move to (Mid of Lane3)

Custom script: call RemoveLocation(udg_MidLane[3])

Lane1 = West; Lane2 = Middle; Lane3 = East

[I]Attacking - After Casting Spells - Lanes Part Two (Initially On)[/I]
Events
Unit - A unit Starts the Effect of an Ability

Conditions
(Owner of (Triggering Unit)) Equal to (Player[#](color))

(Player[#](color) controller) Equal to Computer

Actions
Set EnemyBase = (Enemy Base Ragion)

Unit - Order (Triggering Unit) to Attack Move to (EnemyBase)

Custom script: call RemoveRect(udg_EnemyBase)

[I]Charging - Lock Target[/I]
Events
Time - Every 1 seconds of the game

Conditions
(Player [#](color) controller) Equal to Computer

Actions
Set VarUnitGroup = (Units within [distance] of (Position of (AIHero[Array#]) matching (((Matching Unit) is Alive) Equal to True) and ((Matching Unit) belongs to an enemy of (Owner of (AIHero[Array#])) Equal to True) and (((Matching Unit) is A Hero) Equal to True)

Unit - Pick every unit in VarUnitGroup then do (Actions) else do (Else Actions)
Loop - Actions
If (All Condition are true) then do (Actions) Else do (Else Actions)
If - Conditions
(Integer((Life of (Picked Unit)))) Less than or Equal to [desired life]

Then - Actions
Set Target[Array#] = (Picked Unit)

Unit - Order AIHero[Array#] to Attack Target[Array#]

Set VarUnitGroup = (Units in (Region(0.00, 0.00, 0.00, 0.00))

Custom script: call DestroyGroup(udg_VarUnitGroup[Array#])

Else - Actions
Set VarUnitGroup = (Units in (Region(0.00, 0.00, 0.00, 0.00))

Custom script: call DestroyGroup(udg_VarUnitGroup[Array#])

[I]Charging - After Locking - Arena[/I]
Events
Unit - A unit dies

Conditions
(Dying Unit) Equal to Target[Array#] or (Dying Unit) Equal to AIHero[Array#]

Actions
Set RandomPointArena = (Random Point of (Arena))

Unit - Order (AIHero[Array#]) to Attack Move to (Random Point of (Arena))

Custom script: call RemoveLocation(udg_RandomPointArena)

[I]Charging - After Locking - Lanes[1] Part One (Initially On)[/I]
Events
Unit - A unit dies

Conditions
(Dying Unit) Equal to Target[Array#] or (Dying Unit) Equal to AIHero[Array#]

Actions
Set MidLane[1] = (Center of (Mid Of Lane1))

Unit - Order (AIHero[Array#]) to Attack Move to MidLane[1]

Custom script: call RemoveLocation(udg_MidLane[1])

Lane1 = West; Lane2 = Middle; Lane3 = East;

[I]Charging - After Locking - Lanes[3] Part One (Initially On)[/I]
Events
Unit - A unit dies

Conditions
(Dying Unit) Equal to Target[Array#] or (Dying Unit) Equal to AIHero[Array#]

Actions
Set MidLane[3] = (Center of (Mid Of Lane3))

Unit - Order (AIHero[Array#]) to Attack Move to (MidLane[3])

Custom script: call RemoveLocation(udg_MidLane[3])

Lane1 = West; Lane2 = Middle; Lane3 = East;

[I]Charging - After Locking - Lanes Part Two (Initially On)[/I]
Events
Unit - A unit dies

Conditions
(Dying Unit) Equal to Target[Array#] or (Dying Unit) Equal to AIHero[Array#]

Actions
Set EnemyBase = (Center of (Enemy Base Region))

Unit - Order (AIHero[Array#]) to Attack Move to EnemyBase

Custom script: call RemoveLocation(udg_EnemyBase)

[I]Retreat[/I]
Events
Time - Every 1 seconds of the game

Conditions
(Player[#](color) controller) Equal to Computer

Actions
Set VarUnitGroup = (Units in (Playable Map Area))

Unit Group - Pick Every Unit in (VarUnitGroup) and do (If (Life of (Picked Unit) Less than or Equal to 400) and ((Owner of (Picked Unit) Equal to Player[#]) then do Unit - Order AIHero(Array#) to Move to Center of [Fountain Point of Player[#]/Base] else do Do Nothing

Custom script: call DestroyGroup(udg_VarUnitGroup)

4. Item System

The only thing we need now is to make the Hero AI buy items.

[I]Going Near Shops[/I]
Events
Player - Player[#]'s Current Gold becomes Greater than or Equal to [..]

Conditions
(Player[#](color) controller) Equal to Computer

Actions
Set ItemSpot = (Center of (Item Spot Region))

Unit - Order AIHero[Array#] to Move to ItemSpot

Custom script: call RemoveLocation(udg_ItemSpot)

The Item Spot Region must be near a shop but is not in the Base Region. Remember that the AI will attack if he enters the Base Region.

[I]Buying Items[/I]
Events
Unit - A unit enters [Item Spot Region]

Conditions
(Owner of (Entering Unit)) Equal to (Player[#](color)

(Player[#](color) controller) Equal to Computer

Actions
If (All Conditions are true) then do (Actions) else do (Else Actions)
If - Conditions
(Player[#](color)'s Current Gold) Greater than or Equal to [..]

Then - Actions
If (All Conditions are true) then do (Actions) else do (Else Actions)
If - Conditions
Or - Any conditions are true
(Item type of (Item type carried by AIHero[Array#] in slot 1) Equal to (Item Type of No Item)

(Item type of (Item type carried by AIHero[Array#] in slot 2) Equal to (Item Type of No Item)

(Item type of (Item type carried by AIHero[Array#] in slot 3) Equal to (Item Type of No Item)

(Item type of (Item type carried by AIHero[Array#] in slot 4) Equal to (Item Type of No Item)

(Item type of (Item type carried by AIHero[Array#] in slot 5) Equal to (Item Type of No Item)

(Item type of (Item type carried by AIHero[Array#] in slot 6) Equal to (Item Type of No Item)

Then - Actions
Item - Create [item] at [anywhere]

Set AIItem[Array#] = (Last Created Item)

Hero - Give AIItem[Array#] to AIHero[Array#]

Player - Add -[..] Current Gold to Player[#](color)

Turn Off (this trigger)

Turn On (Advanced Items 1/Selling Items)

Else - Actions
Do Nothing

Else - Actions
Do Nothing

You can also use random integers to make the hero choose from random items.

[I]Selling Items (Initially On)[/I]
Actions
[B]Item - Remove AIItem[Array#] from the game[/B]

[B]Player - Add +[..] Current Gold to Player[#](color)[/B]

Add these actions to more Advanced items to replace older items for money

Thank you for reading this tutorial..


 Last edited: May 5, 2007

#### Diablo-dk

#### Diablo-dk

 Level 10

JoinedNov 10, 2004Messages351

Resources: 3

#### Resources
Maps 1Spells and Systems 1Tutorials 1

Why don't you use trigger tags instead of code tags?

#### DenZel94

#### DenZel94

 Level 7

JoinedJun 4, 2006Messages127

Resources: 7

#### Resources
Maps 3Tutorials 4

Already edited it, sorry..

EDIT: Remember that this triggers have leaks and I'm too lazy to fix it.. And I think Memory Leaks are only fixed when it is not used again..

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Please detach your signature from the tutorial.

You will certainly have to address the leaks in the triggers: Some of our oler (and very basic) tutorials might contain leaks, however, new tutorials should be leak-free.

Our best spell moderators will comment on the validity of the system itself.

With your permission, I will edit the document to improve the spelling, grammar and format.

#### DenZel94

#### DenZel94

 Level 7

JoinedJun 4, 2006Messages127

Resources: 7

#### Resources
Maps 3Tutorials 4

Signature has been detached and some leaks has been fixed.

I'll fix the others tomorrow afternoon GMT+8. Philippine Time for sure.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

 Custom Script: call Remove/Destroy|VariableType|(udg_Variable[Array])


Click to expand...

You should probably change that, it may cause several GUI user brains to explode.

 Set Point[Array#] = (Random Point of (Region))


Click to expand...

Not much reason to use an array for antileak.

Usage of Abilities will only ever check the first unit, far as I can tell

 Set VarUnitGroup[Array#]


Click to expand...

Same with point

Also, I couldn't follow alot of that. Finally, you could always issue a sell order for selling items >< (not 100% sure this can be done in GUI, though)

#### DenZel94

#### DenZel94

 Level 7

JoinedJun 4, 2006Messages127

Resources: 7

#### Resources
Maps 3Tutorials 4

PurplePoot said:

 Not much reason to use an array for antileak.

Usage of Abilities will only ever check the first unit, far as I can tell


Click to expand...

Oh, okay I'll fix it..

EDIT: Oh, I put arrays because in creating the Hero, some players have different hero creation points so the arrays makes sense, right?

Also the same as in UnitGroup array, every AI player needs an individual UnitGroup for retreating and blah...

I have a weird feeling on why I put Custom Scripts on those points that are going to be used again...

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

If the groups are only used instantaneously, as they seem to be (same with the points), you don't need an array since they won't interfere.

#### DenZel94

#### DenZel94

 Level 7

JoinedJun 4, 2006Messages127

Resources: 7

#### Resources
Maps 3Tutorials 4

No. If they use it instantaneously and they are in different positions, then it would get confusion, right? Or am i just wrong?

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

If they're used instantaneously and not more than once at any time in any given trigger, they won't interfere. Thus, unless you need to do an offset or a point saved through a wait, it needn't be an array. (and an offset should just be a second variable)

#### DenZel94

#### DenZel94

 Level 7

JoinedJun 4, 2006Messages127

Resources: 7

#### Resources
Maps 3Tutorials 4

Okay thanks PurplePoot, really appreciate it.. I shouldn't have thought of it.

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

DenZel94: I have edited this tutorial to bring its format somewhat closer to Hive norms.

[edit]


If you could (in the future) create a simple map including your techniques, it would be most helpful. Thanks.


~ Tutorial approved and moved to Trigger (GUI) Editor Tutorials.


 Last edited: Jun 22, 2007

#### Oziris

#### Oziris

 Level 27

JoinedOct 14, 2006Messages4,161

Resources: 7

#### Resources
Icons 1Skins 3StarCraft II Resources 1Tutorials 2

Hey can u post the map for me ?with the triggers


 Last edited: Sep 10, 2007

#### daw_guy

#### daw_guy

 Level 5

JoinedDec 12, 2006Messages128

Resources: 2

#### Resources
Maps 2

Ty for this tutorial.I learned many things.


Next time I make a hero areana i am so using this guide to make ai

#### UchihaSasuke.gr

#### UchihaSasuke.gr

 Level 20

JoinedMar 11, 2007Messages2,822

Resources: 6

#### Resources
Models 1Icons 4Tutorials 1

Finally a good tutorial I might need.

~900 posts!

#### DenZel94

#### DenZel94

 Level 7

JoinedJun 4, 2006Messages127

Resources: 7

#### Resources
Maps 3Tutorials 4

Thank you for the positive feedback you guys ^^..

Okay, I'll try to create a test map for this...

On second thought, nvm. I'll do it when many people requests for it. There are currently 2, Mod Himself and the newbie person man!

~

Yours Truly,

Thy FuKiN n0oB


 Last edited: Nov 1, 2007

#### F4talWr4th

#### F4talWr4th

 Level 6

JoinedAug 5, 2006Messages126

Resources: 2

#### Resources
Maps 1Tutorials 1

Perfect! now this AI tutorial is good for people who want more than 3 heroes to control! This is probably great for Fable/The Roots: Gates of Chaos (N-gage Diablo 2 Clone) maps! XD

#### Provacateur

#### Provacateur

 Level 3

JoinedOct 11, 2013Messages29

Hi there a few questions

1. you are using lane in your attack trigger, what if i dont have a lane? i just have 1 big map where the unit when created is set to attack to move to, a region when my Castle is..


1. should i still use lane?


2. And are you using a new variable in your guide?, for cheaking attacks, or have i complitly misunderstood something.


3. can i use the trigger inderpendetly or shall i make every trigger in order to get the AI seem intelligent... fx can i you only the attack trigger, or learn spells trigger...


3.5 what kind of array are you using to those triggers, and what Means # used in your script..


ty for helping me out.

#### JoHnyW

#### JoHnyW

 Level 7

JoinedOct 8, 2007Messages154

Resources: 5

#### Resources
Models 1Icons 2Maps 2

Move Hero to Tavern then buy it ?

I'm working on AI for footy frenzy I want AI to move wisp to a certain tavern in which there is option to hire special unit called 'random hero' I want AI to to it this way how would trigger look like ?

#### Shinki

#### Shinki

 Level 1

JoinedMay 7, 2018Messages2

I want you to do Map.w3x, you are the only one who can help me.

#### Shinki

#### Shinki

 Level 1

JoinedMay 7, 2018Messages2

I want you to do Map.w3x, you are the only one who can help me.

#### DarkIllidan

#### DarkIllidan

 Level 12

JoinedApr 27, 2009Messages237

Resources: 4

#### Resources
Maps 2Reforged HD Models 2

Is there an example map of this AI system?

#### aloH

#### aloH

 Level 8

JoinedMar 7, 2018Messages133

Holiiii

 You must log in or register to reply here.


### Similar threads

Locked

 Can i shrink this trigger?


JFAMAP

Dec 14, 2021

World Editor Help Zone

Replies11Views2K

Dec 22, 2021
JFAMAP

 Hero Selection Systems


Tonks

Mar 23, 2007

Trigger (GUI) Editor Tutorials

Replies12Views18K

Jun 8, 2023
dantles1992

 How to create an artificial level-up system


frettory

Nov 22, 2012

Trigger (GUI) Editor Tutorials

Replies14Views9K

Mar 13, 2013
frettory

 MUI Spells Using Artificial Waits


Tank-Commander

Sep 28, 2012

Trigger (GUI) Editor Tutorials

Replies32Views20K

Oct 16, 2015
BETABABY

S

 How to make an AI for your map


sentrywiz

Dec 18, 2013

Trigger (GUI) Editor Tutorials

Replies48Views38K

Nov 19, 2019
GAcrewkent

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