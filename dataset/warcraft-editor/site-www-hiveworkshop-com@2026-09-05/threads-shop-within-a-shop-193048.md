---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/shop-within-a-shop.193048/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Shop Within a Shop
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/shop-within-a-shop.193048/>
#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Shop Within a Shop

by Adiktuz

Description:

This tutorial will teach you on how to create a shop within a shop. In this tutorial, I will teach you on how to create a shop which sells Weapons and Armors

Tools Needed:

-World Editor (NewGenWE as much as possible)

-Knowledge on how to create basic shops

-Knowledge on how to create items

-A bit of triggering knowledge

Part 1 - Creating the Shops

The first thing that you need to do is to open up your object editor and create the main shop, this would be the shop that is visible to the user. Make sure you add the abilities, Sell Item and Select Unit

Next, create the dummy shops. The dummy shops are the shops that are within the main shop. These dummy shops are also the ones that will sell the items.

-->Set the model for these units to none.mdl, shadow to none, movement type to none, and size of selection circle, collision size, food cost, sight radius to 0.

-->Also add the ability, Sell Item. And also add the items you want the shop to sell.

-->Make sure have them configured not to show on the minimap (Object Data -> Minimap display -> false)

-->For this tutorial, create two dummy shops, one for weapons, one for armors

-->make sure you create the shop in a place that the hero cannot go

-->you might also need a trigger which will prevent the camera from going to that area (just if you want)

Next, go to the ability editor and create a new ability based on Select Unit. Set the activation range to 999999999 and cast range to 9999999. If you want the selector to show up on the shop, just check the corresponding checkbox.

-->Add this ability to the dummy shops

That's it for now, for the shops.

Part 2 - Creating the Selection Items

Now that we've made the shops, its time to create the dummy items that will enable us to access the sub-shops.

Go to the item editor and create an item for each sub-shop that the main shop will have

-->Make sure they have no gold cost, set Stock maximum to 1, Stock replenish to a low value, and Stock Start Delay to 0

-->For this tutorial, make two, one for weapon and one for armor

-->You can also make the item of type Power-Up, and set it to use upon pickup, though you need to create a dummy ability which will act as the ability of the item, doing this would allow you to remove the RemoveItem part, and will also allow you to access the shops when inventory is full.

Now add these items to the items sold by the main shop

Also, create an item which will be used to go back to the main shop menu

Be sure to add the item to the items sold by each dummy shop

If you use the vjass library, don't forget to set the RETURN_ITEM to the rawcode of the item

Part 3 - Setting up the triggers [For GUI]
Now that the shops and accessor items are set-up, its time for the triggers

Create a new trigger

Set the event to Unit - A unit sells an item (from shop)

On the actions create an in-then-else equal to the number of sub-shops

On each If - Condition, put, Item Type of Item Sold is equal to blahblah

On the Then - Action, put, Selection - Select Blahblah for Player

Note:

If you use the GUI way, be sure to add the dummy shops into the map and set the variables to them...

And be sure to create the variables, I assume you already know how...

Also, the triggers on the test map is not updated, so just copy the triggers here...

Here's a sample trigger:

Events
Unit - A unit sells an item (from shop)

Actions
set TempPlayer = Owner of Buying Unit

set TempInt = Player Number of TempPlayer

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Item Type of Sold Item is equal to Weapon

Then - Actions
Selection - Clear Selection for TempPlayer

Item - Remove Sold Item

Selection - Select WeaponDummyShop for TempPlayer

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Item Type of Sold Item is equal to Armor

Then - Actions
Selection - Clear Selection for TempPlayer

Item - Remove Sold Item

Selection - Select ArmorDummyShop for TempPlayer

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Item Type of Sold Item is equal to Back

Then - Actions
Selection - Clear Selection for TempPlayer

Item - Remove Sold Item

Selection - Select MainShop[TempInt] for TempPlayer

Else - Actions

For JASS/vJASS, I've created a small library...

 JASS:

library ShopAllocation initializer init

 globals
 private integer array ITEMS
 private unit array SHOPS
 private hashtable ShopHash = InitHashtable()
 private unit Shop = null
 private player TempPlayer = null
 private unit array SHOPS_MAIN
 private integer RETURN_ITEM = 'I000' //change this to the rawcode of the item
 endglobals

 private function ChangeShop takes nothing returns boolean
 local integer temp = GetItemTypeId(GetSoldItem())
 set TempPlayer = GetOwningPlayer(GetBuyingUnit())
 set Shop = LoadUnitHandle(ShopHash, temp, 0)
 if Shop != null then
 call RemoveItem(GetSoldItem())
 if (GetLocalPlayer() == TempPlayer) then
 call ClearSelection()
 call SelectUnit(Shop, true)
 set SHOPS_MAIN[GetPlayerId(TempPlayer)] = GetTriggerUnit()
 endif
 endif
 if temp == RETURN_ITEM then
 call ClearSelection()
 call SelectUnit(SHOPS_MAIN[GetPlayerId(TempPlayer)], true)
 endif
 return false
 endfunction

 //Use this function to link dummy shops to their accessor item

 function AllocateShop takes unit u, integer id returns nothing
 call SaveUnitHandle(ShopHash, id, 0, u)
 endfunction

 private function init takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_SELL_ITEM)
 call TriggerAddCondition(t, Condition(function ChangeShop))
 endfunction


endlibrary

NOTES:

--> I suggest placing the dummy shops right next to the vendor so that the camera won't move when you click the dummy portrait

April 8, 2011 - 9:59am GMT+8

-Made the dummy shops show the select user ability

-Set some other attributes of the shops to 0

I've attached a test map below...

#### Attachments

ClassifiedShop.w3x

 18.5 KB

 · Views: 1,098




 Last edited: Jun 7, 2011

#### Kino

#### Kino

 Level 24

JoinedOct 12, 2008Messages1,783

Resources: 22

#### Resources
Models 5Maps 17

Shop within a Shop?


We need to go deeper.


On Topic: Really intresting, its basic yet Ive never thought of such a system.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Test map will follow... ^_^

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great stuff. A really cool technique, and certainly useful.


Some small notes, however.

 Tell the reader to disable the minimap display as well (of the dummy units), else it will appear on the minimap.

 Sight radius, food cost, etc. should be also 0. Maybe have them base it off a unit other than peasant, like the phoenix egg or something.

 You should have the units created at the vendor instead, because when you click on the portrait, then it will shift the camera to the vendor.

 Hold shift+click the selection scale, and type in 0 instead of 0.10. 0.10 is still slightly visible, while 0 will have no selection.

 It would be really cool if this could work with multiple heroes. Maybe have some sort of "Select User" ability, so it can tell what the unit should have. (like the ones that the normal shops have, eg: merchants, arcane vaults, etc.)

 If possible, maybe you can have a cancel button to revert back to select the normal vendor.


The last 2 are moreso simple suggestions. This is still useful as a sample without it. The other ones will probably need a fix, however. Also, make sure your signature is detached from the first post.


Otherwise, nice job and cool technique.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

yeah, I forgot about the foodcosts etc...


The dummy vendors doesn't show for me, but yeah maybe it would be helpful to remind them...


for the third suggestion, well it depends on the user... but maybe I'll change my suggestion to that one... (never thought about the camera shift since I originally did this on my current project which has a locked camera)


Will update this later (detached signature for now)... ^_^


EDIT: Updated!

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Good job. I like the outcome. ~Approved.

Moved to general mapping. Misc is for external tutorials, but relating to wc3 or this site in some way.

#### Orcnet

#### Orcnet

 Level 30

JoinedJul 31, 2010Messages5,248

Resources: 5

#### Resources
Maps 4Tutorials 1

Nicely done tutorial, you also provided a map sample for beginners who still cannot apply with triggers and such, and to see its details are promised, I say this one is really useful, good job for sharing this to us.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

my first thought was to upload the vjass system then I thought it was too simple to be approved as a system resource, that's when I thought of writing it up as a tutorial since I've already seen numerous people asking of something like this... so instead of writing it up again and again, we could just link this... ^_^

#### Orcnet

#### Orcnet

 Level 30

JoinedJul 31, 2010Messages5,248

Resources: 5

#### Resources
Maps 4Tutorials 1

Its good to see your helping the hive with your ability, I like it

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

^.^

and its been a while since I uploaded something... so busy with my map... but if not for that, I wont have thought of this tut...

#### Orcnet

#### Orcnet

 Level 30

JoinedJul 31, 2010Messages5,248

Resources: 5

#### Resources
Maps 4Tutorials 1

hahaha! don't worry, well good luck with yah project. as well as your triggering skills!

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

For the GUI trigger part...

Isn't suppose the 2nd condition (the "Item Type of Sold Item is equal to Armor") should be INSIDE the IF/THEN/ELSE function ?

It should belong IN the ELSE condition, right ?

Also, you're setting the TempPlayer by using the local variable, right ?

Should it get nulled once you've used it ? (I don't know about this one, my JASS knowledge is minimum, just take a shot lol)

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

It was on the else part, the spacing was just a bit off... (fixed it, to avoid confusion)

for the Jass part, TempPlayer was a global (I declared it on the globals block)

and the GUI TempPlayer is not equal to the jass TempPlayer global (you only need one of those scripts, depending on whether you want GUI or vJASS)

EDIT: I've added a note for those who would want to use a real model (to have a portrait) for the dummy shops... ^_^

if anybody wants a test map which shows that kind of sub-shops, just tell me...

#### deaththekid

#### deaththekid

 Level 7

JoinedApr 4, 2011Messages117

Resources: 10

#### Resources
Icons 7Maps 2Spells 1

this system is like in diablo 2 trrade system because of the category [Armors] [Weapons] [Others] [Misc]

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Updated with notes about how to make an item to return to the main shop menu...

also updated the posted triggers in line with this (the one on the test map is not yet updated)

#### Marsal

#### Marsal

 Level 17

JoinedJun 24, 2009Messages1,408

Resources: 19

#### Resources
Maps 1Spells and Systems 18

Really nice and useful but isn't it easier to make a power up tome like item and use manipulates item event? With it you don't have to remove the item.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

I'm not used to using P-ups that much... ^_^

but maybe I could add that as a suggestion... thanks!

#### Ramsey

#### Ramsey

 Level 8

JoinedOct 19, 2008Messages168

Resources: 4

#### Resources
Maps 4

greatest thing ever

#### SadOnix

#### SadOnix

 Level 4

JoinedDec 14, 2023Messages47

For People which are new: when refering to TempInt its the Variable Integer

When you click the Icon in the MainShop and the Dummyshop wont open->

Triggers --> Search for Text --> set Variable --> Here your Variable for 1 of the 2 Dummyshops --> Chose the invisible Shop on Map so the Select Trigger

knows what you are meaning.

Easy peezy for Most. i struggled a bit so just in case someone else new finding your awesome Shop in a Shop Tutorial, here the Part thats missing for Newbies like me.


 Last edited: Apr 30, 2024

 You must log in or register to reply here.


### Similar threads

Locked

 Shop Folder Not Working


Ziggy101

Jan 6, 2020

World Editor Help Zone

Replies19Views3K

Jun 18, 2020
Ziggy101

Locked

[General]
 Shop Refresher HELP Pls


saio

Jan 28, 2022

World Editor Help Zone

Replies4Views1K

Jan 29, 2022
Wrda

Locked

 My shop system isn't working, please help a brotha


medelsvensson

Mar 27, 2015

World Editor Help Zone

Replies3Views1K

Mar 27, 2015
Wietlol

Locked

 Mobile Racial Shop, and Sell Unit


Kam

Jul 6, 2014

World Editor Help Zone

Replies8Views3K

Jul 7, 2014
Kam

Locked

[Solved]
 Global shop - target specific unit


Droof

Nov 22, 2021

World Editor Help Zone

Replies3Views2K

Nov 23, 2021
Uncle

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