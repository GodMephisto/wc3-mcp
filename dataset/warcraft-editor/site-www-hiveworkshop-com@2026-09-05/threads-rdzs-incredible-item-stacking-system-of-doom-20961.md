---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/rdzs-incredible-item-stacking-system-of-doom.20961/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Rdzs Incredible Item Stacking System of Doom
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/rdzs-incredible-item-stacking-system-of-doom.20961/>
#### Rao Dao Zao

#### Rao Dao Zao

 Level 24

JoinedMar 29, 2004Messages2,009

Resources: 5

#### Resources
Maps 4Tutorials 1

Item stacking is a nasty business. And since I've been grappling with it recently for my All-In-One Micro Map, I thought it good to pass on my findings in the form of a pleasant little tutorial.


What you need:


 - Basic triggering knowledge


 - Basic item editing knowledge


For your stacking item, you need two items - a "runic" one (Stats - Use Automatically When Acquired = True) and a "non-runic" one (Stats - Use Automatically When Acquired = False). Now, the player will be interacting with both of these in slightly different ways, so the same name is probably essential. However, the Item Editor lacks an "Editor - Suffix" field, so to avoid confusion give both versions different names and change them back when you're finished. Why? Well, with a runic item, even when your inventory is full, you'll still be able to stack.


Now, you can create a very simple item stacking system thus:

Stacking HealPotion
Events
Unit - A unit owned by Player 1 (Red) Acquires an item

Conditions
(Item-type of (Last created item)) Equal to Potion of Healing (Runic)

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Hero manipulating item) has an item of type Potion of Healing (Non-Runic)) Equal to True

Then - Actions
Item - Set charges remaining in (Item carried by (Hero manipulating item) of type Potion of Healing (Non_Runic)) to ((Charges remaining in (Item carried by (Hero manipulating item) of type Potion of Healing(Non-Runic))) + 1)

Else - Actions
Hero - Create Potion of Healing (Non-Runic) and give it to (Hero manipulating item)

For those of you unable to read triggers outside the editor (like me), this fires when the hero picks the rune (which handily disappears), then checks to see if the hero has the inventory (non-runic) version of the potion. If he/she/it does have the non-runic potion, then add one charge to it, if not, then create a non-runic potion. Done. If you can't be bothered catering for acquisition of the non-runic version, the buck stops here. If you want a totally fool-proof system, keep reading.


So, what if the player has a full inventory and no non-runic potion to add to? We get a nice non-runic potion deposited on the ground. Oh oh. In the heat of battle, the hero might pick up the runic version somewhere else after dropping something, and later go back to that initially dropped non-runic potion - argh, we've got two stacks! This can be very nasty if you've dropped a large stack in a stash or backpack and forgotten about it, then built up a new stack. Maybe two heroes have two different stacks, and one wants to give its stack to the other - again, two large stacks. This might not happen too often, but it's an inconvenience for the poor player.


So we need something a little more complex. Note that in a multiplayer, or even multihero environment, you may need to resort to local variables and jass and all that madness. But we don't like that, so I'll stick to the easy version.


For this you will need two variables. These will hold the number of non-runic potions you have (I'll get to that later) and one to hold the number of charges in the newly acquired item. For this example: Item_TempPotionCount and Item_TempChargeCount.


First of all, the trigger must respond to an item being acquired. The item must be either of the two versions of the stackable item.

Potion Stacking Heal
Events
Unit - A unit owned by Player 1 (Red) Acquires an item

Conditions
Or - Any (Conditions) are true
Conditions
(Item-type of (Item being manipulated)) Equal to Potion of Healing (Runic)

(Item-type of (Item being manipulated)) Equal to Potion of Healing (Non-Runic)

Actions

...

Now, we get down to the hard-core bit.


We must make sure our temporary variables are reset to avoid spillage from previous trigger executions. At this stage, you'd be initialising your local variables.

Set Item_TempPotionCount = 0

Set Item_TempChargeCount = 0
Now, we must discover how many non-runic potions we have to decide what sort of stacking to do. This loop checks every inventory slot the hero has (NOTE: you can use "1 to Unit - Size of Inventory" for the loop condition if you're messing with many different heroes with different inventory sizes) for Potions of Healing that are Non-Runic. If it finds one, it adds 1 to the counter.

For each (Integer A) from 1 to 6, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item-type of (Item carried by (Hero manipulating item) in slot (Integer A))) Equal to Potion of Healing

Then - Actions
Set Item_TempPotionCount = (Item_TempPotionCount + 1)

Else - Actions

So we know how many non-runic potions we have. This means we know what the hero has just acquired and what he/she/it already has. If we currently have 2 non-runic potions, then the hero has a stack and has picked up another one. So we take the number of charges in the new stack and remove it.

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Item_TempPotionCount Equal to 2

Then - Actions
Set Item_TempChargeCount = (Charges remaining in (Item being manipulated))

Item - Remove (Item being manipulated)

Else - Actions

...
Simple. Now, in that "else" section we must add another IF - if we have only one runic potion (the else section here houses what happens when we have 0 non-runic potions). And within this IF we need another one. Because here there is ambiguity - did the Hero acquire a rune and have a stack already, or did the hero have nothing and pick up a stack? Just check if he picked up a runic potion. If this is true, then set the number of charges. Leave this "else" section blank, because if you've just picked up a stack it's got all its charges.


And finally, if TempPotionCount is not 2 and it's not 1 either, it's 0. So we create a non-runic potion for the hero, and again leave TempChargeCount at 0.

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Item_TempPotionCount Equal to 1

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item-type of (Item being manipulated)) Equal to Potion of Healing

Then - Actions
Set Item_TempChargeCount = (Charges remaining in (Item being manipulated))

Else - Actions

Else - Actions
Hero - Create Potion of Healing and give it to (Hero manipulating item)

And our last action is to add on the charges to the item we have. If you have just been given a new item or picked a stack, then TempChargeCount is 0; so 0 charges will be added to the item's default.

Item - Set charges remaining in (Item carried by (Hero manipulating item) of type Potion of Healing) to ((Charges remaining in (Item carried by (Hero manipulating item) of type Potion of Healing)) + Item_TempChargeCount)
So there you go. A simpler-than-it-looks way of stacking items that accounts for all eventualities. Just in case all that passed you by, here's a simple algorithm anybody should be able to understand without my rambling:


0. Initialise variables: TempPotionCount and TempChargeCount to 0.


1. Establish how many non-runic potions we have in the hero's inventory.


2. If we have 2, we've picked a stack while we have a stack - set charges and remove the picked item.


3. If we have 1,


4. If we picked a rune while we have a stack - set charges.


5. Else we picked a stack off the ground - do nothing.


6. If we have 0, we've picked a rune while we have no stack - create a non-runic potion.


7. Add the number of charges onto the non-runic potion in the inventory.


Thank you, and good night.


 RDZ


 Last edited: May 17, 2008

#### emperor_d3st

#### emperor_d3st

 Level 18

JoinedApr 13, 2008Messages1,629

Resources: 2

#### Resources
Ported Models 2

This doesn't take into account the scenario when your inventory is full.

 You must log in or register to reply here.


### Similar threads

Locked

 Item Stacking System


Faith

Aug 28, 2010

Triggers & Scripts

Replies10Views2K

Aug 28, 2010
Faith

 Stacking Items


DigitalDemon

Dec 21, 2007

Trigger (GUI) Editor Tutorials

Replies18Views27K

Sep 18, 2025
Luashine

Locked

[Trigger]
 Item Stacking System not working.


Drazhar

Apr 27, 2011

Triggers & Scripts

Replies3Views976

Apr 27, 2011
Pharaoh_

 IDS (Infinite Number of Item Classes)


-Kobas-

Mar 29, 2010

Trigger (GUI) Editor Tutorials

Replies25Views47K

Oct 7, 2015
BPower

Locked

[Trigger]
 Stacking Items Trigger


Mallaboro

Feb 23, 2010

Triggers & Scripts

Replies5Views4K

Apr 27, 2010
LunarLite

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