---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/stacking-items.45226/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Stacking Items
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/stacking-items.45226/>
#### DigitalDemon

#### DigitalDemon

 Level 6

JoinedJun 1, 2007Messages98

Resources: 5

#### Resources
Icons 2Maps 2Tutorials 1

ITEM STACKING TUTORIAL

 There have been so many questions regarding the combining and/or stacking of items so i decided to make a tutorial!

1] Start

 - Create a blank map

 - Make a hero inside it

 - Make 3 (or more) of the same item near the hero.

2] Stacking Items

 - The triggers used for this are not complex. In fact, it is easier than most things.

Basically, what you have to do is to check if the player has an item in his inventory that is the same as the item he picked up. If he does then it stacks the item, otherwise it just.. does nothing and gives the item as normal.

Stack
Events
Unit - A unit Acquires an item

Conditions
((Hero manipulating item) has an item of type (Item-type of (Item being manipulated))) Equal to True

(Item being manipulated) Not equal to (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated)))

Actions
Item - Set charges remaining in (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated))) to ((Charges remaining in (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated)))) + (Charges remaining in (Item being manipulated)))

Hero - Drop (Item being manipulated) from (Hero manipulating item)

Item - Remove (Item being manipulated)

Of course, to shorten the lines you could just assign variable to those things but it's your choice!

Events
Unit - A unit Acquires an item

This makes the trigger run when a player picks up an item.

Conditions
((Hero manipulating item) has an item of type (Item-type of (Item being manipulated))) Equal to True

(Item being manipulated) Not equal to (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated)))

This checks if the player has the item and also if the item the player carries is NOT the same item he picked up. This prevents loops and crashes.

Actions
Item - Set charges remaining in (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated))) to ((Charges remaining in (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated)))) + (Charges remaining in (Item being manipulated)))

Hero - Drop (Item being manipulated) from (Hero manipulating item)

Item - Remove (Item being manipulated)

The first line adds the amount of charges in the picked up item to the amount of charges in the item the hero already had.

The second line makes the hero drop the item he just picked up and the third one removes it!

And you're finished! Now any items you pick up will be automatically stacked.

The only problem is if you want to unstack the items.

How would you do that?

Well, you could, for example make a trigger which takes half the charges in the chosen slot and drops that many. The problem is that it can become odd numbers and you can't have half an item... The WE automatically rounds it down so you lose 1 charge.

Unstack
Events
Player - Player 1 (Red) types a chat message containing -unstack as A substring

Player - Player 2 (Blue) types a chat message containing -unstack as A substring

Player - Player 3 (Teal) types a chat message containing -unstack as A substring

Player - Player 4 (Purple) types a chat message containing -unstack as A substring

Player - Player 5 (Yellow) types a chat message containing -unstack as A substring

Player - Player 6 (Orange) types a chat message containing -unstack as A substring

Player - Player 7 (Green) types a chat message containing -unstack as A substring

Player - Player 8 (Pink) types a chat message containing -unstack as A substring

Player - Player 9 (Gray) types a chat message containing -unstack as A substring

Player - Player 10 (Light Blue) types a chat message containing -unstack as A substring

Player - Player 11 (Dark Green) types a chat message containing -unstack as A substring

Player - Player 12 (Brown) types a chat message containing -unstack as A substring

Conditions

Actions
Set HeroPos = (Position of Hero[(Player number of (Triggering player))])

Set Slot = (Integer((Substring((Entered chat string), 10, 10))))

Set Charges = ((Charges remaining in (Item carried by Hero[(Player number of (Triggering player))] in slot Slot)) / 2)

Item - Set charges remaining in (Item carried by Hero[(Player number of (Triggering player))] in slot Slot) to Charges

Item - Create (Item-type of (Item carried by Hero[(Player number of (Triggering player))] in slot Slot)) at HeroPos

Item - Set charges remaining in (Last created item) to Charges

Custom script: call RemoveLocation(udg_HeroPos)

Oh, and before you use the unstack trigger, you need to define Hero[x] and create the other variables.

Example map:

#### Attachments

test1.w3x

 17.5 KB

 · Views: 1,694




 Last edited: Dec 21, 2007

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

There are ones already.

EG: https://www.hiveworkshop.com/forums/showthread.php?t=20961

#### DigitalDemon

#### DigitalDemon

 Level 6

JoinedJun 1, 2007Messages98

Resources: 5

#### Resources
Icons 2Maps 2Tutorials 1

Mine is simpler to understand IMO. ( No offense )

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Also, this is more of a system than a tutorial. However, I find that Rao's is as easy to understand, personally, or even easier. (Not that it would really make much of a difference to me...)

#### madcat2

#### madcat2

 Level 14

JoinedAug 25, 2005Messages1,105

Resources: 14

#### Resources
Icons 6Skins 6User Interface 1Tutorials 1

No, hurt to have two. Theres different people in this world and some may find the other easier to understand and some might find this one easier to understand. Basically if the other one is to hard to understand they can come over here and try this one and see if it helps, and if both are just not geting to them they go ask in the trigger or jass pages but at least they may have a idea how the system may work from reading the one thats easier for them, so the jass and trigger people only have to help in the parts needed. So basically you can just attach this one to the other and mark it as "If you had trouble with the other one try this one." Because giving them a second option never hurts.

#### ShaDowPoWeR

#### ShaDowPoWeR

 Level 3

JoinedNov 18, 2007Messages48

PurplePoot said:

 There are ones already.

EG: https://www.hiveworkshop.com/forums/showthread.php?t=20961

Click to expand...

I think This one is better than the other

#### Basinator

#### Basinator

 Level 4

JoinedJul 25, 2007Messages79

Mega necro, but got a few qs

((Hero manipulating item) has an item of type (Item-type of (Item being manipulated))) Equal to True


Click to expand...

Where do I can find this?

 Conditions

((Hero manipulating item) has an item of type (Item-type of (Item being manipulated))) Equal to True

(Item being manipulated) Not equal to (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated)))


Click to expand...

Also, I don't really understand what is happening here, can someone explain?

Thanks!

#### muzzel

#### muzzel

 Level 14

JoinedJun 27, 2008Messages1,322

Resources: 2

#### Resources
JASS 1Wurst 1

Considering the last post in this threat was 6 years ago im not sure if he will answer.

#### Basinator

#### Basinator

 Level 4

JoinedJul 25, 2007Messages79

Who said it has to be him who answers?

#### vypur85

#### vypur85

 Level 5

JoinedJan 27, 2014Messages164

Resources: 1

#### Resources
Maps 1

> Where do I can find this?

It's a Boolean comparison in the condition list.

> Also, I don't really understand what is happening here...

The first condition is to check if the unit already has that item or not. If already has then proceed to stacking.

The second condition is to ensure the item is not the item the hero just acquired.

Here's a scenario:

You pick up a Gun item.

If your unit already has Gun item, then proceed to stack.

The Gun that your unit already has, is not the Gun that your unit just picked up from the ground.

That's all.

#### Basinator

#### Basinator

 Level 4

JoinedJul 25, 2007Messages79

Thanks for the explanation, I although I am still derping around inside myself with "itemBeingManipulated" and "HeroManipulatingItem"

#### ngkn92

#### ngkn92

 Level 4

JoinedOct 28, 2012Messages82

about unstack

Why do u have to /2 and lose 1 stack?

Do like this,

set int_1 = charge/2

(the number hero keep)​
set int_2 = charge-int_1

(hero drop this)​
Done.

btw, I keep got the issue that sometimes it stack, sometimes it doesn't.

*edited:

the problem solved anyway, just me trying to be stupid.


 Last edited: May 29, 2014

#### emperor_d3st

#### emperor_d3st

 Level 18

JoinedApr 13, 2008Messages1,629

Resources: 2

#### Resources
Ported Models 2

This doesn't take into account the scenario when your inventory is full.

#### 6oranges

6

#### 6oranges

This file attached has a revised version of the trigger above. By placing items in a certain region those items are the only ones that will stack. Also I replaced "Unit - A unit Acquires an item" with has been issued an order.

Advantages:

• Allows you to stack Items when inventory full without extra rune item.

• Allows you to choose which items are stack able.

• Only takes two triggers for all stack able items you choose.

Disadvantages:

• When you hold shift to do multiple things in a row after picking up a stack able item all further commands are not guaranteed to activate. This is because picking up items too far away (more than 200) causes your hero to move to the item and then attempt to pick it up again. This move resets your shift queue.

A workaround is: when holding shift to pick up one of the stack able items then some other order first tell your hero to move next to the item and then pick it up.

#### Attachments

Stacking.w3x

 91.3 KB

 · Views: 375



#### mYster4teN

#### mYster4teN

 Level 1

JoinedSep 14, 2017Messages3

hey 6oranges! you saved my ideas on my new survival map! i was looking for such a simple way like this!

#### mYster4teN

#### mYster4teN

 Level 1

JoinedSep 14, 2017Messages3

I've made a way you can "unstack" your items ins 6oranges Triggers:

When the dummy ability called "unstack" is casted By your hero, one charge of an item located in hero's slot 1 will appear in your inventory, or in the ground if the inventory is full, and you can stack the items back by simply draging the item over another of the same type in the hero's inventory.

(The [Item - Set custom value of (Last created item) to 9] and them to 0 after the [Hero - Give (Last created item) to (Casting unit)] Actions are just to avoid the unstacked charge to come back to the original stack.

 And i needed to add another condition at "Stacking" trigger, to complete the action).

#### Attachments

Stacking_Improved.w3x

 92.9 KB

 · Views: 354



#### Thualluandra

#### Thualluandra

 Level 2

JoinedJun 19, 2017Messages8

DigitalDemon said:

ITEM STACKING TUTORIAL

 There have been so many questions regarding the combining and/or stacking of items so i decided to make a tutorial!

1] Start

 - Create a blank map

 - Make a hero inside it

 - Make 3 (or more) of the same item near the hero.

2] Stacking Items

 - The triggers used for this are not complex. In fact, it is easier than most things.

Basically, what you have to do is to check if the player has an item in his inventory that is the same as the item he picked up. If he does then it stacks the item, otherwise it just.. does nothing and gives the item as normal.

Stack
Events
Unit - A unit Acquires an item

Conditions
((Hero manipulating item) has an item of type (Item-type of (Item being manipulated))) Equal to True

(Item being manipulated) Not equal to (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated)))

Actions
Item - Set charges remaining in (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated))) to ((Charges remaining in (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated)))) + (Charges remaining in (Item being manipulated)))

Hero - Drop (Item being manipulated) from (Hero manipulating item)

Item - Remove (Item being manipulated)

Of course, to shorten the lines you could just assign variable to those things but it's your choice!

Events
Unit - A unit Acquires an item

This makes the trigger run when a player picks up an item.

Conditions
((Hero manipulating item) has an item of type (Item-type of (Item being manipulated))) Equal to True

(Item being manipulated) Not equal to (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated)))

This checks if the player has the item and also if the item the player carries is NOT the same item he picked up. This prevents loops and crashes.

Actions
Item - Set charges remaining in (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated))) to ((Charges remaining in (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated)))) + (Charges remaining in (Item being manipulated)))

Hero - Drop (Item being manipulated) from (Hero manipulating item)

Item - Remove (Item being manipulated)

The first line adds the amount of charges in the picked up item to the amount of charges in the item the hero already had.

The second line makes the hero drop the item he just picked up and the third one removes it!

And you're finished! Now any items you pick up will be automatically stacked.

The only problem is if you want to unstack the items.

How would you do that?

Well, you could, for example make a trigger which takes half the charges in the chosen slot and drops that many. The problem is that it can become odd numbers and you can't have half an item... The WE automatically rounds it down so you lose 1 charge.

Unstack
Events
Player - Player 1 (Red) types a chat message containing -unstack as A substring

Player - Player 2 (Blue) types a chat message containing -unstack as A substring

Player - Player 3 (Teal) types a chat message containing -unstack as A substring

Player - Player 4 (Purple) types a chat message containing -unstack as A substring

Player - Player 5 (Yellow) types a chat message containing -unstack as A substring

Player - Player 6 (Orange) types a chat message containing -unstack as A substring

Player - Player 7 (Green) types a chat message containing -unstack as A substring

Player - Player 8 (Pink) types a chat message containing -unstack as A substring

Player - Player 9 (Gray) types a chat message containing -unstack as A substring

Player - Player 10 (Light Blue) types a chat message containing -unstack as A substring

Player - Player 11 (Dark Green) types a chat message containing -unstack as A substring

Player - Player 12 (Brown) types a chat message containing -unstack as A substring

Conditions

Actions
Set HeroPos = (Position of Hero[(Player number of (Triggering player))])

Set Slot = (Integer((Substring((Entered chat string), 10, 10))))

Set Charges = ((Charges remaining in (Item carried by Hero[(Player number of (Triggering player))] in slot Slot)) / 2)

Item - Set charges remaining in (Item carried by Hero[(Player number of (Triggering player))] in slot Slot) to Charges

Item - Create (Item-type of (Item carried by Hero[(Player number of (Triggering player))] in slot Slot)) at HeroPos

Item - Set charges remaining in (Last created item) to Charges

Custom script: call RemoveLocation(udg_HeroPos)

Oh, and before you use the unstack trigger, you need to define Hero[x] and create the other variables.

Example map:


Click to expand...

I've tested that bro and I realize you forgot something.

This can prevent any bugs with items that doesn't have any charge or 0 charges, like permanent items.

I found that out because I got 2 Claws of Attack and it stacked to one. And I didn't want that.

Just add this conditions to the list and it'll all work fine.

(Charges remaining in (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated)))) Greater than 0

#### Sleepless

#### Sleepless

 Level 4

JoinedSep 25, 2015Messages61

Resources: 1

#### Resources
Spells and Systems 1

Doesnt work.

Bugs out when you have a free inventory slot before the item you pick up

#### Luashine

#### Luashine

 Level 23

JoinedJan 3, 2022Messages518

Resources: 3

#### Resources
Tools 2Tutorials 1

FYI: this was needed for old game versions. Item stacks had to be triggered manually.

For Reforged* you can use the new gameplay constant "ItemStackingEnabled" to allow item stacking and the 'ista' aka "stackMax" of item stats to specify the maximum stack limit. It worked great for me using 'wswd' aka Invisible Warden as a test subject.

* the corresponding event natives first appeared in 1.32.10.18820 API

See screenshots by Uncle here: How do I make certain items stackable?

 You must log in or register to reply here.


### Similar threads

Locked

 About item stacking made by DigitalDemon


zicron

Sep 9, 2009

World Editor Help Zone

Replies1Views1K

Sep 13, 2009
zicron

 RDZ's Incredible Item Stacking System of Doom


Rao Dao Zao

Oct 5, 2005

Trigger (GUI) Editor Tutorials

Replies1Views22K

Feb 15, 2015
emperor_d3st

Locked

 Item Stacking


Garfield1337

Feb 10, 2010

World Editor Help Zone

Replies8Views2K

Feb 12, 2010
Berb

Locked

[Trigger]
 Stack Item


Seppuku89

Sep 25, 2008

Triggers & Scripts

Replies5Views2K

Sep 26, 2008
Captain Griffen

 IDS (Infinite Number of Item Classes)


-Kobas-

Mar 29, 2010

Trigger (GUI) Editor Tutorials

Replies25Views47K

Oct 7, 2015
BPower

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