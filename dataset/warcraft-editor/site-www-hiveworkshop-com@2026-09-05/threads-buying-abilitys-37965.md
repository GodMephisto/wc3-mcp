---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/buying-abilitys.37965/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Buying Abilitys
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/buying-abilitys.37965/>
#### Parrothead

#### Parrothead

 Level 13

JoinedJun 10, 2007Messages777

Resources: 17

#### Resources
Maps 10Spells 2Tutorials 5

Buying Abilities

Buying Abilities Through Items

This tutorial will teach you how to make a system allowing the user to buy a skill from a shop (e.g. Avatar) through the purchase of a temporary item that will give you that skill. This can come in handy in games in which you wish for players to customize their heroes. For this tutorial I will use the Infernal and Dark Ritual spells.

The Items

 First of all, make a dummy item for each skill you're adding, like this:

Basically, this will be the item the user buys when they want to learn a specific skill. You may modify any of the values to best suite your map. You will need a new item every time you want a different purchasable skill.

Variables

Next, you will need to create a trigger that gives you the spell, but before you create the triggers, you will need this variable:

Check 'array'. The variable stores most of the needed information, and you cannot make this trigger without it. To access the Variable Manager, go to the Trigger Editor and look for a green X, or press CTRL+B.

After creating this variable and the object data, you will be ready to start making the trigger.

One-level skills

Some Trigger
Events
Unit - A unit Acquires an item

Conditions
(Item-type of (Item being manipulated)) Equal to Summon Infernal

Actions
Set skill[(Player number of (Owner of triggering unit))] = Inferno

Wait 0.20 seconds

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of skill[(Player number of (Owner of triggering unit))] for (Triggering unit)) Less than 1

Then - Actions
Unit - Add skill[(Player number of (Owner of triggering unit))] to (Triggering unit)

Game - Display to (Player group((Owner of triggering unit))) for 10.00 seconds the text: You have bought the...

Else - Actions
Game - Display to (Player group((Owner of triggering unit))) for 5.00 seconds the text: You already have th...

Player - Add 350 to (Owner of triggering unit) Current lumber

Ignore the parts cut off of the trigger - it is just telling you that you bought the skill. Now when somebody buys the Infernal item, it will add the skill to the hero/unit buying it and the item will disappear. This trigger will also give a refund if the spell is already learned. Make this trigger for every spell you want to sell at shops. The included condition is necessary. If you do not include it, whenever you buy any item it will give you the infernal spell, which is definitely not desirable.

Leveling abilities from triggers

For our example of a leveling ability, I will use the Dark Ritual spell.

Again, a dummy item will be used. This item should be very similar to the item used for Infernal - the tooltips, gold cost, icon, etc, should be changed though (If desired).

Some Trigger
Events
Unit - A unit Acquires an item

Conditions
(Item-type of (Item being manipulated)) Equal to Dark Ritual

Actions
Set skill[(Player number of (Triggering player))] = Dark Ritual

Wait 0.20 seconds

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of skill[(Player number of (Owner of triggering unit))]for (Triggering unit)) Less than 1

Then - Actions
Unit - Add skill[(Player number of (Owner of triggering unit))] to (Triggering unit)

Game - Display to (All players) for 10.00 seconds the text: Dark Ritual Learned...

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of skill[(Player number of (Owner of triggering unit))] for (Triggering unit)) Less than 3

Then - Actions
Unit - Increase level of skill[(Player number of (Owner of triggering unit))] for (Triggering unit)

Else - Actions
Game - Display to (Player group((Owned of triggering unit))) for 5.00 seconds the text: This spell is...

Player - Add 100 to (Owner of triggering unit) Current lumber

This would do the same exact thing as the infernal spell, but it will allow you to further level the ability by buying it multiple times. You can set a max on the trigger to also refund it if they attempt to buy the spell more times than the maximum ability level.

Second Way

There are lots of ways to do various things within the World Editor (like a cinematic, for example), and there is another way to do this as well. This way is somewhat easier (if you know how to manage variables) and can be more effective as it is more organized. Only continue if you know the World Editor beyond a novice level.

Variable Setup

 First of all, we will need a lot of variables, and we will set them all in a single trigger, so right now create a trigger like this which we will use later.

Set Variables
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
-------- We will set our variables here. --------

Now, we will need to create our variables. We will need one for skills, the items, and descriptions (optional).

Skills-Ability Array- Size 1-Value None

Description-String Array- Size 1-Value None

Item-Item-Type Array- Size 1-Value None

Now that are variables are properly set, we may continue on. Go to the trigger you set up. Now we will place our variables in that trigger. Go to Action -> New Action -> All -> Set Variable by right clicking Actions.

Set Variables
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
-------- We will set our variables here. --------

-------- Each number corresponds to only 1 skill. So, the Inferno variable will ALWAYS be 0, and Dark Ritual will ALWAYS have 1. Do not change these up as you will experience glitches. --------

Set skills[0] = Inferno

Set skills[1] = Dark Ritual

Set Items[0] = Summon Infernal

Set Items[1] = Dark Ritual

Set Description[0] = <Your info here>

Set Description[1] = Dark Ritual is a level 1 normal ability with 3 levels.

If you do not see square brackets to place numbers in your variables, then you didn't make the variable an array, and you must fix the variables. Do so by reading rereading the beginning of the tutorial.

Keep setting variables for each item, skill, and description in this trigger. Now that this trigger is done and our variables are set, we may continue on.

The Second Way - The Trigger

We want it to make it so when a unit buys the item it will add the ability. Thus, make a new trigger. The event should be as follows:

Unit - A unit Acquires an item
A generic event that is perfect for what we are doing. Make sure the items purchased in shops (Dark Ritual, Infernal) have there own category, I will use Powerup for mine. Now create this condition:

(Item-class of (Item being manipulated)) Equal to Powerup
(Change Powerup to the category you have your purchasable items in) The item being manipulated is the one the object buys. Now to finish up this trigger, do these actions:

Actions
For each (Integer A) from 0 to <Last array slot you used>, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Items[(Integer A)] Equal to (Item-type of (Item being manipulated))

(Level of skill[(Player number of (Owner of triggering unit))]for (Triggering unit)) Less than 1
Then - Actions

Game - Display to (All players) for 1.00 seconds the text: Spell Learned...

Unit - Add skill[(Player number of (Owner of triggering unit))] to (Triggering unit)
Custom script: exitwhen true

Else - Actions

Game - Display to (Player group((Owned of triggering unit))) for 5.00 seconds the text: This spell is...

Player - Add [Gold Amount] to (Owner of triggering unit) Current lumber

DOWNLOAD DEMO

 Edits


Edit1: Did what Steel_Cold Suggested.

Edit2: Added more information and used trigger tags.

Edit3: Made it more clear and simple (Will add the second way to do this).

Edit4: Added a second way to do this and 2 mirrors to download the demo.

Edit5: Added another variable screenshot and fixed up way one a tad.

Edit6: Fixed up way one again and added text part to way number 2.

Edit7: Finalized way 2 actions so that it will get approved.

Edit8: Engrish improved from 3 years ago: the tutorial is now better understandable .

~Thanks to everyone who helped in this tutorial (Including Purplepoot, myself, and some people who suggested content to be placed here)


 Last edited: Oct 23, 2010

#### Steel_Cold

#### Steel_Cold

 Level 5

JoinedApr 16, 2005Messages153

You could also include in the tutorial a way to level up the abilities through buying them

#### wyrmlord

#### wyrmlord

 Level 11

JoinedOct 13, 2005Messages233

Resources: 5

#### Resources
Tools 1Maps 1Tutorials 3

Here are my suggestions:

I would suggest using

tags for the triggers instead of posting the images of them. [*]In addition, I recommend you explain in some detail what each trigger does because all you're doing right now is just posting triggers with very little or no information.

[/LIST]

#### Parrothead

#### Parrothead

 Level 13

JoinedJun 10, 2007Messages777

Resources: 17

#### Resources
Maps 10Spells 2Tutorials 5

Updated

#### Eleandor

#### Eleandor

 Level 21

JoinedAug 21, 2005Messages3,698

Resources: 3

#### Resources
Models 1Maps 1Tutorials 1

I myself don't care a lot about spelling errors in posts, but in a tutorial you should not make such errors as "dissapear" or "there" instead of "their".

Now that you used trigger tags, you can remove the attachments. Other than that, it's a useful tutorial to newbies I guess

#### Ivanneth

#### Ivanneth

 Level 8

JoinedJun 1, 2006Messages253

Resources: 9

#### Resources
Maps 7Tutorials 2

Nice enough, I guess. Reminds me of TGS ...


Maybe, though, you should go into more detail, so that people that just started triggering, or aren't so good with some things, can understand it better.

#### Parrothead

#### Parrothead

 Level 13

JoinedJun 10, 2007Messages777

Resources: 17

#### Resources
Maps 10Spells 2Tutorials 5

Will this get approved?

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

The method could be majorly improved, eg;

-Create an ability array, Skills

-Create an Item-type array, Items

-Create a string array, descriptions

Init
Events
Map Initialization

Conditions

Actions
Set Skills[0] = Dark Ritual

Set Items[0] = Purchase Dark Ritual

Set Descriptions[0] = "Dark Ritual"

Set Skills[1] = Frost Nova

Set Items[0] = Purchase Frost Nova

Set Descriptions[1] = "Frost Nova"

...

Buy
Events
Unit - A Unit Acquires an Item

Conditions
(Item-class of (Acquired Item)) Equal to <item class for purchased skills>

Actions
For Each Integer A from 0 to <last slot in the Skills array>, do Actions
Loop - Actions
If (All Conditions are True) Then do (Then actions) Else do (Else actions)
If - Conditions
Items[(For Loop Integer A)] Equal to (Item-Type of (Acquired Item))

Then - Actions
-------- add all the add ability stuff here, display message from Descriptions[0] --------

Custom script: exitwhen true

Else - Actions

The wait in your above trigger makes it unweildy, also the fact that you would either have to have a trigger for every item, or a giant mass of if-statements.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

~Revive~

Well?

#### Parrothead

#### Parrothead

 Level 13

JoinedJun 10, 2007Messages777

Resources: 17

#### Resources
Maps 10Spells 2Tutorials 5

Ill try to update it.


Edit: Updated the tutorial with fixed spelling mistakes (some didnt catch most), I added the second way to add to do this (Thx PurplePoop +Repz0r) Fixed up the triggers and organized the tutorial, Hopefully it may have a chance at approval now. I made 2 ways to do it (1 is harder but better for people who want really dont wand to use variables) and 2 (uses variables and harder things but is easier to do). Added 2 download mirrors incase of one not working ( I dont know why)


 Last edited: Nov 1, 2007

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

It could still use some polish (spelling, readability, etc) - I could tweak it, if you so desire.

#### Parrothead

#### Parrothead

 Level 13

JoinedJun 10, 2007Messages777

Resources: 17

#### Resources
Maps 10Spells 2Tutorials 5

Do what you want with it

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

I finally got around to updating it - now, it could use some more work on your part.

In way 1, you use Triggering Player. This will not work in this case (a Unit triggers the event) - you will need Owner of (Triggering Unit).

Could you please actually include the most useful actions for the Second way (i.e. the part where it actually displays the message and such - people reading the tutorial may want to know that)?

#### Parrothead

#### Parrothead

 Level 13

JoinedJun 10, 2007Messages777

Resources: 17

#### Resources
Maps 10Spells 2Tutorials 5

Fixed up way one, going to school so I will do way 2 later.

#### sonkan1

#### sonkan1

 Level 8

JoinedJan 18, 2007Messages331

Resources: 2

#### Resources
Models 2

This toto is verry mutch like mine... (merged into the secrets of the spell book) so i think this one also shuld get merged! its kinda complicated....

nice though.

#### Parrothead

#### Parrothead

 Level 13

JoinedJun 10, 2007Messages777

Resources: 17

#### Resources
Maps 10Spells 2Tutorials 5

Updated Again .

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

In "The Second Way",

You need to do stuff before the "exitwhen true"

You don't actually add the ability or anything, just display a message.

#### Parrothead

#### Parrothead

 Level 13

JoinedJun 10, 2007Messages777

Resources: 17

#### Resources
Maps 10Spells 2Tutorials 5

Updated once more.

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

~Thread Approved.

#### underscore

#### underscore

 Level 6

JoinedMar 17, 2008Messages150

Resources: 6

#### Resources
Skins 1Tutorials 5

Is this like the spell system in Imagica ORPG? Where in you buy an orb?

#### Parrothead

#### Parrothead

 Level 13

JoinedJun 10, 2007Messages777

Resources: 17

#### Resources
Maps 10Spells 2Tutorials 5

Never played that RPG before so I really can't answer that.

#### SpongejjBob

#### SpongejjBob

 Level 3

JoinedJun 19, 2008Messages18

Resources: 1

#### Resources
Tutorials 1

Thanks man, this tutorial is really helpful

#### Velmarshal

#### Velmarshal

 Level 16

JoinedMar 9, 2008Messages2,175

Just a typo, he wrote "abilitys" instead "abilities".

#### Parrothead

#### Parrothead

 Level 13

JoinedJun 10, 2007Messages777

Resources: 17

#### Resources
Maps 10Spells 2Tutorials 5

Yes please fix that typo .

#### jongzky

J

#### jongzky

nah. all gui users can do this by themselves. I also did this in my map without any tutorials. I rate this 1/5. hha its so easy to make.

#### Parrothead

#### Parrothead

 Level 13

JoinedJun 10, 2007Messages777

Resources: 17

#### Resources
Maps 10Spells 2Tutorials 5

Oh what a special little troll you are.

EDIT: All right, how about you make your own tutorial telling how to do it?


 Last edited: Dec 18, 2008

#### splashy5

#### splashy5

 Level 9

JoinedJul 22, 2007Messages261

Resources: 13

#### Resources
Models 10Tutorials 3

There is another way to do this with less effort..

#### Attachments

Trigger Bash.JPG

 33.8 KB

 · Views: 474



#### mnadeau1992

#### mnadeau1992

 Level 4

JoinedFeb 18, 2009Messages54

maibe

yeah maibe, but in this way you gonna take a long while before seting 200 spells...


i found better the tutorial methods

#### .L.

#### .L.

 Level 2

JoinedOct 26, 2008Messages15

Something is worng...

Well, In this manner, If they bought 8 or more different abilities, then the other abilities wouldn't show up, Do you have a solution for this, like making ability limits. ex. Number of skill allowed : 4, or something like that. but overall 4/5

#### Tirco

#### Tirco

 Level 2

JoinedFeb 7, 2009Messages25

wery nice and usefull! i did something like this, but i made you upgrade a skill =P

#### Bwathke

#### Bwathke

 Level 9

JoinedMay 22, 2009Messages491

Resources: 1

#### Resources
Tutorials 1

I have played this on Battle.Net Diablo except if you drop the scroll you loose the spell but it dosent have to do that and I found a way to make the script work easily tell me if its buggy. Nice tutorial also.

Unit - Paladin 0000 <gen> Acquires an item

(Item-type of (Item being manipulated)) Equal to Scroll Of Healing

Unit - Add Heal to (Triggering unit)

But this is for a certain unit only but you can change that.

#### dragor-X

#### dragor-X

 Level 6

JoinedFeb 26, 2008Messages171

Resources: 1

#### Resources
Maps 1

Hmm your 2nd way is wrong, totaly wrong! Sorry to say that... There's just some errors and some variable problems, there we go: a fix

(taken from my map (used your system and fixed the bugs))

Buy weapon Main
Events
Unit - A unit Acquires an item

Conditions
(Item-class of (Item being manipulated)) Equal to Artifact

Actions
Item - Make (Item being manipulated) Invulnerable

For each (Integer Integer01) from 0 to 1, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Item[Integer01] Equal to (Item-type of (Item being manipulated))

(Level of Skills[Integer01] for (Triggering unit)) Less than 1

Then - Actions
Game - Display to (All players matching ((Matching player) Equal to (Owner of (Triggering unit)))) the text: |cff00FF00Notice:|r...

Game - Display to (All players matching ((Matching player) Equal to (Owner of (Triggering unit)))) the text: (|cff00FF00Notice:|r + Description[Integer01])

Unit - Add Skills[Integer01] to (Triggering unit)

Custom script: exitwhen true

Else - Actions
Game - Display to (All players matching ((Owner of (Matching unit)) Equal to (Owner of (Triggering unit)))) the text: |cff00FF00Notice:|r...

EDIT: I even made a remove ability trigger, it's even easier to do than the add ability one:

Drop weapon Main
Events
Unit - A unit Loses an item

Conditions
(Item-class of (Item being manipulated)) Equal to Artifact

Actions
For each (Integer Integer01) from 0 to 1, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Item[Integer01] Equal to (Item-type of (Item being manipulated))

(Level of Skills[Integer01] for (Triggering unit)) Greater than or equal to 1

Then - Actions
Game - Display to (All players matching ((Matching player) Equal to (Owner of (Triggering unit)))) the text: |cff00FF00Notice:|r...

Unit - Remove Skills[Integer01] from (Triggering unit)

Custom script: exitwhen true

Else - Actions
Game - Display to (All players matching ((Owner of (Matching unit)) Equal to (Owner of (Triggering unit)))) the text: |cff00FF00Notice:|r...

And that's the trigger where the gold refound happens:

Return money
Events
Unit - A unit Pawns an item (to shop)

Conditions

Actions
For each (Integer Integer01) from 0 to 1, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Item[Integer01] Equal to (Item-type of (Sold Item))

(Level of Skills[Integer01] for (Triggering unit)) Greater than or equal to 1

Then - Actions
Game - Display to (All players matching ((Matching player) Equal to (Owner of (Triggering unit)))) the text: |cff00FF00Notice:|r...

Player - Add (Integer((Current life of (Sold Item)))) to (Owner of (Triggering unit)) Current gold

Hero - Drop (Sold Item) from (Triggering unit)

Item - Remove (Sold Item)

Custom script: exitwhen true

Else - Actions
Game - Display to (All players matching ((Owner of (Matching unit)) Equal to (Owner of (Triggering unit)))) the text: <Empty String>

If you put (Owner of (Trigger unit)) instead of the Integer01 then the array will be equal to the number of the player like player purple is player 4 so it'll add him the skill[4] but what if you only had 2 spells setted up like in your example and that instead of being the player red, you'd be player gray, then you won't have any spell added to your hero! :O credit me or rep me

You should also add a fonction for people who want the ability removed when the hero removes the item or do an event like activating the spell from the item (thunder clap based without dmg, buff, stuff...).

Edit please! Also edit your DEMO MAP and add credit if you put in my fix :O

Else it's flawless! People should always use 2nd way because the 1rst way is way too long! (Open the Battle Mage map and you'll see how long is the spell trigger for buy ability)

Edit: I personaly use the item HP as the cost recover cause I need it for my map but normally in your way of doing things it's better


 Last edited: Sep 11, 2009

#### SteakedMeat

#### SteakedMeat

 Level 4

JoinedJun 4, 2009Messages70

The first one was great! GOD JOB!!


This helped me alot, i had a trigger before but i could only buy 1 ability and the nothing happened on rest + at max lvl i could still buy ability and loose lumber/gold so thx alot!

#### orcyngiser

#### orcyngiser

 Level 3

JoinedApr 9, 2010Messages60

Resources: 1

#### Resources
Maps 1

useful. very useful.

#### Provacateur

#### Provacateur

 Level 3

JoinedOct 11, 2013Messages29

I really cant find

(Level of skill[(Player number of (Owner of triggering unit))] for (Triggering unit)) Less than 1

which is used in the trigger, what trigger do i start with to end with Level of skill?

and i`ve also having a HUGE problem with the

 Set skill[(Player number of (Owner of triggering unit))] = Inferno

I have made 2 inferno. 1 builded on the Item that cast inferno´s ( like in the guide )

and one based on the spell that cast inferno´s..

in the guide they refer to the item being used.... but when i try edit the trigger, it can only find the spell ( when using custom ).. but i did look in all items as well, no custumized item there eiter...

So what am i doing wrong?

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

@TO

You should make your triggers more efficient so you don't mislead people.

Also don't use waits or Integer A / B.

You also leak player groups (force).

Edit8: Engrish improved from 3 years ago: the tutorial is now better understandable .

English not Engrish (I don't really care about spelling but i though you would like to know.

Take a look at my tutorial Things a GUIer Should Know

PurplePoot said:

 I finally got around to updating it - now, it could use some more work on your part.

In way 1, you use Triggering Player. This will not work in this case (a Unit triggers the event) - you will need Owner of (Triggering Unit).

Click to expand...

You should use triggering Player rather than owner of triggering unit.

It is a player unit event which means that triggering player will catch the unit picks up item event.

Check here.

 JASS:

call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_PICKUP_ITEM )

Note: Almost all of the Generic unit events in GUI will register a triggering player catch.

#### Provacateur

#### Provacateur

 Level 3

JoinedOct 11, 2013Messages29

Hi there, i'm trying to make custom spells following the guide here.

it's not the first time i try, but i always find a wall in this sentence.

(Item-type of (Item being manipulated)) Equal to Summon Infernal

i can't find neither the summon infernal or the dark ritual.

most times it look like this:

(Item-type of (Item being manipulated)) Equal to Summon Infernal

but nothing happen when people buy the stone.

then i tried the dark ritual, to se if i could find the right source there..

But the only time i can find Dark ritual is, with a much longer sentence.

looking like this.

Summon Infernal Equal to (Random level (Level of Dark Ritual for (Triggering unit)) Any Class item-type)

pls help, its so frustrating, that i can't climb that wall.

#### Razorburst

#### Razorburst

 Level 1

JoinedJul 22, 2018Messages8

can you post an demomap to see the system in game??

#### Razorburst

#### Razorburst

 Level 1

JoinedJul 22, 2018Messages8

also how to put requirements for the bought of the ability? level of hero o whatever

 You must log in or register to reply here.


### Similar threads

 Abilities - Purchases & Upgrades with Items


Apheraz Lucent

Oct 18, 2011

Trigger (GUI) Editor Tutorials

Replies23Views11K

May 29, 2023
Alexus_Kun

 Aquiring Abilities with Items


PiCkstix

Dec 9, 2007

Trigger (GUI) Editor Tutorials

Replies3Views11K

Dec 12, 2007
PiCkstix

 How to make Professions


Airandius

Sep 14, 2008

Trigger (GUI) Editor Tutorials

Replies43Views18K

Feb 14, 2010
Dark hamster

Locked

[Solved]
 Buying Abilities - Integer Issues


Apheraz Lucent

Sep 12, 2011

World Editor Help Zone

Replies7Views1K

Sep 13, 2011
Apheraz Lucent

 Making an Open Rpg


Ralle

Jan 12, 2006

Trigger (GUI) Editor Tutorials

Replies0Views47K

Jan 12, 2006
Ralle

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