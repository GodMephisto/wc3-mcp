---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/items-all-about-items.98895/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Items All About Items
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/items-all-about-items.98895/>
#### XeNiM666

#### XeNiM666

 Level 4

JoinedJul 19, 2008Messages37

Resources: 2

#### Resources
Icons 1Tutorials 1

 Items

What are Items?

Items are basically objects according to wikipedia. Actually, it means this: Item, entry

in a list, or one object in a collection of objects. It's like that. Items are things

that are dropped from creeps, which help you to survive longer or fight better.

The Item Editor

The Item Editor is a part of the Object Editor which allows you to create new items or edit

existing items.

This will explain the context of the Item Editor Itself.

Abilities - Abilities

This is first and foremost what the item actually does. What it gives you and what's in it

that can make you stronger. Remember that items can only have 4 abilities and 1 maximum

active ability.

Art - Button Position (X)

Art - Button Position (Y)

This is where the item will be located when you buy it. Not in the inventory but in the

shop. This graph will show you the button positions in X,Y form.

Art - Interface Icon

This is the icon of your item. The picture of your item. This is like the signature of the

item. 2 items of the same icon usually means 2 icons of the same type or 2 different items

with different icons which can cause confusion.

Art - Model Used

This is the model of your item. No, not the model where you pick it up and it shows the item being held by the unit.

Art - Scaling Value

How big your model is.

Art - Selection Size - Editor

This size of the selection circle BUT only in the World Editor.

Art - Tinting Color

As usual the color of the model. Some models cannot be colored.

Combat - Armor Type

This is basically the sound of the item when someone attacks it.

Stats - Actively Used

This is checked when one of the abilities includes one active spell like Blink, Command,

etc.. Uncheck this when your using Spellbooks in your item.

Stats - Can be dropped

This is usually True but when you want an item to not be dropped use this. This is used in

DotA's Divine Rapier and Aegis of the Immortal (I think).

Stats - Can Be Sold By Merchants

Usually True. This is used for Quest items so whatever you do it cannot be sold.

Stats - Can Be Sold To Merchants

This is like "Can be Sold". True usually unless you like it to be Unique and cannot

be sold even for 1,000,000 Gold like DotA's Divine Rapier.

Stats - Classification

It's like the groupings of the items. If you want to group them like Rare items go to

Campaign Class, Normal Items go to Misc Class, etc...

Stats - Cooldown Group

Not sure. But i think this is the stacking group of the items like the order strings in

Abilities.

Stats - Drop When Carrier Dies

It's in the title. If you die, the item will be dropped to the ground, free for anyone to

pick it up.

Stats - Gold Cost

How much the item would cost in Gold.

Stats - Hit Points

How much life does it have. If you want it to be "Cannot be destroyed", the you might

as well set his to 500,000.

Stats - Ignore Cooldown

Even though the ability has a cooldown, it will be set to 0 when this is True. Almost 100%

of the time, it's False.

Stats - Include as Random Choice

If True, then this item will be included if you put a "Random Item" item on the map.

Stats - Level (Unclassified)

Stats - Level

THIS IS NOT THE REQUIED LEVEL OF YOUR ITEM!!! It is just merely a

grading system for the item. e.g. Level 1 items for Normal, Level 2 items for Magical,

etc... The only difference between the 2 is Unclassified can go up to 10 and Classified can

only go up to 8. I think this cannot be changed in the Gameplay Constants.

Stats - Lumber Cost

How much the item would cost in Lumber.

Stats - Number of Charges

This is used to set how many times an active item can be used. Example: If set to 2, then

the item can only cast Blink (for example) for 2 times. Set this to 0 for Unlimited

Charges.

Stats - Perishable

Again, not 100% sure. I think this applies to "Cannot be destroyed" and "Can Be

Sold To Merchants". Set this to True for Indestructible Items.

Stats - Priority

Stats - Stock Maximum

The maximum stock of an item. It's the number below the item when it's at a shop.

Stats - Stock Replenish Interval

This is the time in which when you buy an item, this is the time to regain 1 stock. The

interval only works it's magic when it is NOT equal to the Stock Maximum of the item.

Example: In DotA, every item replenishes 1 stock in 3 seconds when you buy one.

Stats - Stock Start Delay

This is the time (begins in the start of the game) where all items become available.

Example: In DotA, the heroes have a Stock Start Delay of 15 seconds which means that you

have to wait 15 seconds to pick a hero.

Stats - Use Automatically When Acquired

When you pick up an item (if set to True), it uses the item automatically. Usually

used in Runes.

Stats - Valid Target for Transformation

Techtree - Requirements

Add units/buildings/researches to this value if you want an item to have requirements.

Example: In normal melee games, all races require the level 3 main base to get an orb.

Techtree - Requirements - Levels

Needs a value above if you want it to require (for example), Level 3 Moon Glaive. Add

the Upgrade Moon Glaive on the value above and add to this value 3.

Text - Description

This is the text or description when you click an item which is still on the ground.

Text - Hotkey

The Hotkey of the item in the shop.

Text - Name

The name of the item.

Text - Tooltip - Basic

This is the text on the top that says "Purchase". Example: Purchase Boots of Speed

Text - Tooltip - Extended

The main description of the item in the inventory. This basically tells you what the item

does and some Descriptions of it or some Histories perhaps.

How do I create a custom Item?

4 Choices:

 - Click File - New Custom Item

 - Click the Treasure Box Icon on the upper Toolbar

 - Right-Click on the Item and select New Custom Item

 - Press Ctrl+Shift+N


Click to expand...

Then edit the abilities, icons, models and descriptions and viola, you have your own custom item.

Some neat Tips and Tricks about Items.

Recipes

Some or maybe many people know what recipes are. But for those who do not know them yet, they are 2 or more items that needs each other in order to make 1 stronger item.

How to make recipies

Step 1: Creating the Item

Create 3 or 2 versions of the item, 1 for the shop, 1 for the scroll (for me, this is optional) and 1 for the item itself.

Step 2: Replacing the Item(This can be skipped if you do not want the scroll)

Do this trigger: This trigger basically replaces the one that you buy on the shop and replaces it with the scroll.

Items Scroll
Events
Unit - A unit Acquires an item

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item-type of (Item being manipulated)) Equal to |CFF91C877Boots of Haste (Shop)|r

Then - Actions
Item - Remove (Item being manipulated)

Hero - Create |CFF91C877Boots of Haste (Scroll Recipe)|r and give it to (Hero manipulating item)

Else - Actions

Just do different If/Then/Else's for every item that has a recipe. Shouldn't be too hard.

Step 3: Combining the Item

This trigger combines the said items into a more stronger version of the item. In this trigger, it combines the Boots of Speed and the Gloves of Haste into a Boots of Haste.

Item Combine
Events
Unit - A unit Acquires an item

Conditions
Or - Any (Conditions) are true
Conditions
(Item-type of (Item being manipulated)) Equal to |CFFFDC689Boots of Speed|r

(Item-type of (Item being manipulated)) Equal to |CFFFDC689Gloves of Haste|r

(Item-type of (Item being manipulated)) Equal to |CFF91C877Boots of Haste Recipe|r

((Hero manipulating item) has an item of type |CFFFDC689Boots of Speed|r) Equal to True

((Hero manipulating item) has an item of type |CFFFDC689Gloves of Haste|r) Equal to True

((Hero manipulating item) has an item of type |CFF91C877Boots of Haste (Scroll Recipe)|r) Equal to True

Actions
Set Item[1] = |CFFFDC689Boots of Speed|r

Set Item[2] = |CFFFDC689Gloves of Haste|r

Set Item[3] = |CFF91C877Boots of Haste (Scroll Recipe)|r

For each (Integer A) from 1 to 6, do (Actions)
Loop - Actions
For each (Integer B) from 1 to 3, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item-type of (Item carried by (Hero manipulating item) in slot (Integer A))) Equal to Item[(Integer B)]

Then - Actions
Item - Remove (Item carried by (Hero manipulating item) in slot (Integer A))

Else - Actions

Special Effect - Create a special effect attached to the origin of (Hero manipulating item) using Abilities\Spells\Items\AIlm\AIlmTarget.mdl

Special Effect - Destroy (Last created special effect)

Hero - Create |CFF91C877Boots of Haste|r and give it to (Hero manipulating item)

And you're done!!!

Random Drops

For those who are wondering what this is, its a system that when you kill an enemy, it will have a chance to drop an item. This is very simple. Just do this trigger:

Chance
Events
Unit - A unit Dies

Conditions
(Owner of (Killing unit)) Not equal to Neutral Hostile

(Owner of (Triggering unit)) Equal to Neutral Hostile

(Random integer number between 1 and 1000000) Less than or equal to (Hero level of (Killing unit))

Actions
Set TempPoint = (Position of (Triggering unit))

Item - Create |CFF82CA9CSoulstone|r at (Position of (Triggering unit)) [COLOR="Green"]// You can have any item here[/COLOR]

Custom script: call RemoveLocation(udg_TempPoint)

A very simple trigger that can be easily implemented.

Level Restrictions

Many people has asked on how to do this. So I will teach you. Like the upper tips, its a very simple trigger:

Required Level
Events
Unit - A unit Acquires an item

Conditions

Actions
-------- ---- ---- ---- ---- ---- ---- ---- REQUIRED LEVEL 2 ---- ---- ---- ---- ---- ---- ---- --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Hero manipulating item) is A Hero) Equal to True

(Level of (Hero manipulating item)) Less than 2

Or - Any (Conditions) are true
(Item-type of (Item being manipulated)) Equal to |CFF91C877Ring of Regeneration|r

(Item-type of (Item being manipulated)) Equal to |CFF91C877Periapt of Vitality|r

(Item-type of (Item being manipulated)) Equal to |CFF91C877Pendant of Energy|r

(Item-type of (Item being manipulated)) Equal to |CFF91C877Mask of Death|r

(Item-type of (Item being manipulated)) Equal to |CFFFDC689Staff of Wisdom|r

(Item-type of (Item being manipulated)) Equal to |CFFFDC689Gloves of Speed|r

(Item-type of (Item being manipulated)) Equal to |CFFFDC689Axe of Power|r

Then - Actions
Hero - Drop (Item being manipulated) from (Hero manipulating item)

Floating Text - Create floating text that reads (Required level of + ((Name of (Item being manipulated)) + is 2)) above (Hero manipulating item) with Z offset 0.00, using font size 10.00, color (100.00%, 0.00%, 0.00%), and 0.00% transparency

Floating Text - Change (Last created floating text): Disable permanence

Floating Text - Change the lifespan of (Last created floating text) to 2.50 seconds

Floating Text - Change the fading age of (Last created floating text) to 1.00 seconds

Floating Text - Set the velocity of (Last created floating text) to 90.00 towards 90.00 degrees

Else - Actions

Like the others, its a very simple trigger that can be easily implemented. Just put comments in to group the required levels like what I did.

Spell Books

A very nice way to make items have more than 4 abilities, BUT only one active one in 2 easy steps.

If you put an active ability in an item that has the spell book, it will not use the ability instead it will display you the spell book.

How to Combine Charges

If you want to combine 2 1 charge potions or just stack multiple charges on 1 item, simply copy the code below. To change the maximum number of charges, change the value of MAXIMUM on the code. This does not require vJASS.

 JASS:

function Trig_Combine_Items_Conditions takes nothing returns boolean
 return GetItemCharges(GetManipulatedItem()) > 0
endfunction

function Trig_Combine_Items_Actions takes nothing returns nothing
 local integer ITEMCOUNT
 local integer ITEMLOOP
 local integer CHARGES
 local integer MAXIMUM
 local item NEWITEM
 local unit OURUNIT

 set MAXIMUM = 100
 set ITEMCOUNT = 0
 set ITEMLOOP = 0
 set CHARGES = 0
 set NEWITEM = GetManipulatedItem()
 set OURUNIT = GetManipulatingUnit()

 loop
 exitwhen ITEMLOOP > 6
 if ((GetItemTypeId(NEWITEM)) == (GetItemTypeId(UnitItemInSlot(OURUNIT, ITEMLOOP-1)))) then
 if ((GetItemCharges(UnitItemInSlot(OURUNIT, ITEMLOOP-1)) + GetItemCharges(NEWITEM)) <= MAXIMUM) then
 if not ( (UnitItemInSlot(OURUNIT, ITEMLOOP-1)) == (NEWITEM)) then
 set CHARGES = (GetItemCharges(UnitItemInSlot(OURUNIT, ITEMLOOP-1))) + GetItemCharges(NEWITEM)
 call SetItemCharges( UnitItemInSlot(OURUNIT, ITEMLOOP-1), CHARGES )
 call RemoveItem( NEWITEM )
 set ITEMLOOP=7
 endif
 endif
 endif
 if ITEMLOOP < 7 then
 set ITEMLOOP = ITEMLOOP + 1
 endif
 endloop
endfunction

//===========================================================================
function InitTrig_Combine_Charges takes nothing returns nothing
 local trigger t = CreateTrigger( )
 call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_PICKUP_ITEM )
 call TriggerAddCondition( t, Condition( function Trig_Combine_Items_Conditions ) )
 call TriggerAddAction( t, function Trig_Combine_Items_Actions )
endfunction

How to have Spell Books into your items

Step 1: Creating the Spell Book

Create a custom Spell Book ability, located under the category Abilities - Special - Items - Spellbook. After that, put in the wanted abilities into the spell book and set the Data - Maximum Spells to how many abilities you have inside the Spellbook. After creating, put the spell book in the item and set the Stats - Actively Used to False

Step 2: Disabling the Spell Book

I know what you're saying, "Disable the spellbook wtf?!" Yeah, its needed so you whatever you will do, cannot use the spellbook. It's simple really, once you put the spell book in the item do this in a trigger:

Disable Spellbooks
Events
Map initialization

Conditions

Actions
Set PlayerGroup = (All players matching ((((Matching player) slot status) Equal to Is playing) and (((Matching player) controller) Equal to User)))

Player Group - Pick every player in PlayerGroup and do (Actions)
Loop - Actions
Player - Disable Spell Book for (Picked player)

-------- Disable other spell books here --------

Custom script: call DestroyForce(udg_PlayerGroup)

Just add the spellbook abilities in the Player Group function.

TADA!!! Your done!!!

 Even if you have spellbooks in your items, you can only have 1 active ability


Click to expand...

Well that's all about items that I know!

Please go easy on me, this is my first tutorial..

PLEASE post comments and feedbacks. It is greatly appreciated!!!:thup::thup:

~XeNiM666


 Last edited by a moderator: Dec 25, 2017

#### Oziris

#### Oziris

 Level 27

JoinedOct 14, 2006Messages4,161

Resources: 7

#### Resources
Icons 1Skins 3StarCraft II Resources 1Tutorials 2

Very nice tutorials it has some stuff that ther is not on hive tuts.+rep

#### Airandius

#### Airandius

 Level 17

JoinedJun 12, 2007Messages1,261

Resources: 8

#### Resources
Icons 2Maps 1Ported Icons 4Tutorials 1

- It gives a very good explanation about items and also gives alot of ways how you can use them.


However you should try to figure out what "Stats - Valid Target for Transformation" and "Stats - Priority" does. If you realy can't figure it out and it's unimportant you can just remove them.


- It's a good thing that you gave an example of where it has been used: "Stats - Can be dropped".


- I like it how you also gave some tips how you could use items.


- You also gave some triggers for example: level restrictions.


- I'm not sure about what you are trying to say with: "Even if you have spellbooks in your items, you can only have 1 active ability" at the end of your tutorial.


A item can have the ability spellbook with multiple active spells inside that spellbook.


-------------------------------------------------------------------------------------


Overal I find this a very good tutorial and it should be approved, however fix the things you don't know and remove that quote, it's incorrect.


(The default wc3 editor has a spellbook with multiple active spells inside of it.)

#### Callahan

#### Callahan

 Level 37

JoinedMar 28, 2004Messages1,613

Resources: 132

#### Resources
Models 108Icons 12Packs 2Skins 2Maps 5Spells and Systems 2Tutorials 1

XeNiM666 said:

Stats - Perishable


Again, not 100% sure. I think this applies to "Cannot be destroyed" and "Can Be


Sold To Merchants". Set this to True for Indestructible Items.

Click to expand...


No.If it's true then charged item will disappear from your inventory when their charge reach 0.


If it's false the item still remain in your inventory even with 0 charge.

#### Linaze

#### Linaze

 Level 24

JoinedFeb 28, 2007Messages3,478

Resources: 4

#### Resources
Models 1Tutorials 3

Though this tutorial covers some very nice stuff, some things are very simple. However on the whole, it is a good and useful tutorial to new people.

#### RiskDude

#### RiskDude

 Level 3

JoinedJul 18, 2008Messages53

Nice tutorial, keep the good work up bro

#### akblabla

#### akblabla

 Level 2

JoinedApr 2, 2009Messages27

 Stats - Priority


 ? ? ?


Click to expand...


It is the priority for the ai to get it. like if it has a item with much lower priority it drops the current and take the new. Only for melee maps.

#### Sorenkj

#### Sorenkj

 Level 7

JoinedOct 20, 2007Messages174

Resources: 17

#### Resources
Icons 11Maps 2Ported Icons 4

When the item is dropped, (level require) isn't there a way on giving the hero the amount of gold and lumber back?

#### Tr1NiTY

#### Tr1NiTY

 Level 4

JoinedAug 5, 2007Messages68

Resources: 4

#### Resources
Maps 4

anyway, how to make a stackable potions?


for example ; i have 3 potions in my inventory, then i pick up 1 potion. How to make my potions in my inventory have 4 charges instead of 1 3 potions and 1 1 potion [2slot]

#### XeNiM666

#### XeNiM666

 Level 4

JoinedJul 19, 2008Messages37

Resources: 2

#### Resources
Icons 1Tutorials 1

 anyway, how to make a stackable potions?


Click to expand...

UPDATED FIRST POST...

It now shows how to do that...

#### GGwarcarft

G

#### GGwarcarft

How do I create a custom Item or new items

#### XeNiM666

#### XeNiM666

 Level 4

JoinedJul 19, 2008Messages37

Resources: 2

#### Resources
Icons 1Tutorials 1

Ummm.. please read the tutorial first.

 How do I create a custom Item?

4 Choices:

 - Click File - New Custom Item

- Click the Treasure Box Icon on the upper Toolbar

- Right-Click on the Item and select New Custom Item

- Press Ctrl+Shift+N


Click to expand...

Click to expand...

#### Sheephunter

#### Sheephunter

 Level 11

JoinedDec 21, 2008Messages766

Resources: 3

#### Resources
Maps 1Spells 2

I get an error when i try to stack the items whit that Jass Code... Can you help me?

#### XeNiM666

#### XeNiM666

 Level 4

JoinedJul 19, 2008Messages37

Resources: 2

#### Resources
Icons 1Tutorials 1

Oops, i forgot to localize the trigger. Try it now.

 JASS:

function Trig_Combine_Items_Conditions takes nothing returns boolean
 return GetItemCharges(GetManipulatedItem()) > 0
endfunction

function Trig_Combine_Items_Actions takes nothing returns nothing
 local integer ITEMCOUNT
 local integer ITEMLOOP
 local integer CHARGES
 local integer MAXIMUM
 local item NEWITEM
 local unit OURUNIT

 set MAXIMUM = 100
 set ITEMCOUNT = 0
 set ITEMLOOP = 0
 set CHARGES = 0
 set NEWITEM = GetManipulatedItem()
 set OURUNIT = GetManipulatingUnit()

 loop
 exitwhen ITEMLOOP > 6
 if ((GetItemTypeId(NEWITEM)) == (GetItemTypeId(UnitItemInSlot(OURUNIT, ITEMLOOP-1)))) then
 if ((GetItemCharges(UnitItemInSlot(OURUNIT, ITEMLOOP-1)) + GetItemCharges(NEWITEM)) <= MAXIMUM) then
 if not ( (UnitItemInSlot(OURUNIT, ITEMLOOP-1)) == (NEWITEM)) then
 set CHARGES = (GetItemCharges(UnitItemInSlot(OURUNIT, ITEMLOOP-1))) + GetItemCharges(NEWITEM)
 call SetItemCharges( UnitItemInSlot(OURUNIT, ITEMLOOP-1), CHARGES )
call RemoveItem( NEWITEM )
 set ITEMLOOP=7
 endif
 endif
 endif
 if ITEMLOOP < 7 then
 set ITEMLOOP = ITEMLOOP + 1
 endif
 endloop
endfunction

//===========================================================================
function InitTrig_Combine_Charges takes nothing returns nothing
 local trigger t = CreateTrigger( )
 call TriggerRegisterAnyUnitEventBJ( t, EVENT_PLAYER_UNIT_PICKUP_ITEM )
 call TriggerAddCondition( t, Condition( function Trig_Combine_Items_Conditions ) )
 call TriggerAddAction( t, function Trig_Combine_Items_Actions )
endfunction

#### beadhanger

B

#### beadhanger

I find this tutorial extremly usefull since i havent been working a lot on items. + rep

#### Dreadnought[dA]

#### Dreadnought[dA]

 Level 12

JoinedFeb 23, 2007Messages1,030

Resources: 1

#### Resources
Tutorials 1

Omg that blue text kills my eyes even worse than all caps!

#### Alkonis

#### Alkonis

 Level 5

JoinedAug 27, 2007Messages138

You did not write that JASS code.

Please cite its owner.

#### PlayerName

P

#### PlayerName

Nice tutorial!

Great tutorial , explanes almost everything you need when you are a noob in item creating!


But, there is one thing ; When you making item recipes do you need all that in the trigger?  I just use this :

#### 13rOkEn_IcE

#### 13rOkEn_IcE

 Level 2

JoinedNov 4, 2009Messages18

Thx for showing how to combinde Items! and it is a very good tutorial!

#### LordDz

#### LordDz

 Level 25

JoinedMay 11, 2007Messages4,643

Resources: 8

#### Resources
Maps 8

Please use any other color than blue/dark colors.

#### TheUnknownHunter

#### TheUnknownHunter

 Level 3

JoinedFeb 24, 2016Messages13

Resources: 7

#### Resources
Icons 7

How To Use The Code IN Your How to Combine Charges

#### SoulStealer

#### SoulStealer

 Level 3

JoinedAug 28, 2017Messages37

Hey,How do you check if a Hero has a certain item?

#### Cake is a Lie

#### Cake is a Lie

 Level 4

JoinedOct 1, 2017Messages96

Um,perishable works when the item has charges. When the charges of an item reaches 0,the item vanishes or should I say,perishes.

Just like DotA's Diffusal Blade...the blade didn't vanish because the "Stats - Perishable" is set to false...

I hope I am helpful...

#### Null

#### Null

 Level 27

JoinedDec 3, 2018Messages909

Resources: 171

#### Resources
Icons 133Skins 6Maps 7Ported Icons 25

HOw about the quests when you must pick items and then the items must be destroyed ???

 You must log in or register to reply here.


### Similar threads

Locked

 Simple Item Limit Question.


C.B.King

Apr 4, 2020

World Editor Help Zone

Replies0Views1K

Apr 4, 2020
C.B.King

Locked

[JASS]
 Item Stack JASS - Please Verify


Bloodbane7

Oct 25, 2009

Triggers & Scripts

Replies14Views2K

Oct 30, 2009
Adiktuz

Locked

[Trigger]
 Problem with creating set items


Dragonkillerrus

Jun 5, 2017

Triggers & Scripts

Replies4Views1K

Jun 12, 2017
Noobsmoke

Locked

[Trigger]
 Keep an item in the same slot


Googlieeyes

Aug 14, 2009

Triggers & Scripts

Replies19Views2K

Aug 16, 2009
Drenith

Locked

 My items can be duplicated


FaceJaraxxus

Jun 24, 2023

Triggers & Scripts

Replies2Views1K

Jun 24, 2023
Duckfarter

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Misc/General Mapping Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top