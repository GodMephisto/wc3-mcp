---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ids-infinite-number-of-item-classes.162448/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Ids Infinite Number of Item Classes
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ids-infinite-number-of-item-classes.162448/>
#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

 Item Drop System by -Kobas-


Item Drop System

by -Kobas-

Requirements

 * Basic knowledge of the Object Editor

 * Basic knowledge of the Trigger Editor

 * Poor knowledge in variables.

 * A moment of your time  (Around 5 min).

Introduction

Have you ever wondered what the Item Drop System (IDS) is? If your answer is ‘no’, then allow me to explain it for you:

If you’ve ever played a map such as Gaia's retaliation, you know that you are allowed to carry only 1 item of the same type (Weapon, Armor, etc.). Meaning, that if a player picks up an item of a type which was already in his inventory, it will be automatically dropped, and therefore player will be un-able to carry two items of this type simultaneously. It's simple as that!

How does my system works?

My system utilizes item levels in order to make item classes which you will use in your map. Therefore, the number of your item classes will greatly increase! How is that possible?

Let me explain: You can only have 7 item classes (by default in World Editor) whereas item levels can be indefinite!

And here’s an example:

Worse WayBetter Way

1. Permanent = Weapon

2. Campaign = Armor

3. Power Up = Helm

4. Purchasable = Shield

5. Miscellaneous = Misc

6. Artifact = Artifact

7. Charged = Potion1. Level 1 = Weapon

2. Level 2 = Armor

3. Level 3 = Helm

10. Level 10 = Shield

25. Level 25 = Misc

10. Level 100 = Artifact

999. Level 999 = Material

You must be wondering: “That sounds great, but isn’t 8th a maximum item level?” Allow me to show you some magic:

1) Select any item (class and item level in this case doesn't matter)

2) Select the field that indicates the item level

3) Press Shift+Enter

4) Now enter any value you want to be the new maximum item level

Using that, you can also set many other options as well, such as: negative hit and mana point regeneration, influence of auras (to reduce or increase damage, attack or movement speed, armor…), ect. You’ll agree with me when I say that it’s a very useful thing!

And for the end I will show you how to stack some items

Let's start!

1. Okay, let’s start making our IDS now! I know, you will maybe be surprised as how to make an entire system and pack it into a single trigger, well it’s impossible! And here’s an example why:

A) Imagine we have a 100 item classes; that means that the trigger would have a 100 parts with each one containing at least 5 commands; that’s 500 commands (or 100 Microsoft Word pages if you want)! Therefore, it’s impossible to make the IDS, not to mention what happens with 1 000 or 1 000 000 item classes.

B) On the other hand, a system can be made with more ways than one: it can be sqeezed into a lot smaller and much more complicated code (I really don’t want to get into that), or it can be separated into several, simpler, triggers. Basically, it can be exactly like you need (want) it to be which is a plus because you can adjust it to your needs.

That’s why I’ll try my best to explain to you the essense of making a IDS trigger!

2. You can now open your map to test the new code we’re making.

3. Create a new trigger and, of course, choose a name for it.

4. Once you’ve done that, copy this code in it (in other words, make your trigger looking like this one here).

Item System
Events
Unit - A unit Acquires an item

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item level of (Item being manipulated)) Equal to 1

Then - Actions
Set Item = 0

For each (Integer Item_Slot) from 1 to 6, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item level of (Item carried by (Hero manipulating item) in slot (Item_Slot))) Equal to 1

Then - Actions
Set Item = (Item + 1)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Item Greater than 1

Then - Actions
Hero - Drop (Item being manipulated) from (Hero manipulating item)

Game - Display to (All players) the text: You can carry only one level 1 item!

Else - Actions

Else - Actions

5. You noticed that this trigger uses item level 1. So let’s set the levels of all items on which the trigger will affect to a value of 1 (other levels are not important to us right now)

6. Save the map and go ahead and test it. You will notice that the trigger doesn’t allow you to carry two items of level 1! If that’s the case, then you succeeded in making an IDS! Congratulations!

Now you might be thinking:

1. “Hey, this is so easy! How come I didn’t think of it before?!” If you’re thinking like this, then you already know enough about IDS to make one yourself. And there are a few ways to do it from this tutorial:

A) Copy the trigger several times and only set your own item levels for your item classes (the easiest, but the most unprofessional way to do it).

B) Copy the actions inside the trigger and set item levels there (easier, but still an unprofessional way).

C) Copy the parts of the actions and the rest put under a same condition for all items you can carry in hero’s inventory once or two or even tree or four times... (the hardest, but the most professional way)

2. “Well I kind of understand what’s all this about, but how to make and set up a trigger by myself?” Well, I made the rest of the tutorial especially for you guys! So read carefully and I’m sure that with a little effort you will learn how the whole system works once you try to set up the trigger I attached on this tutorial.

1. Trigger Event

Events
Unit - A unit Acquires an item

This option can be changed so that it affects a special unit(s) or all units that are controlled by Red (Blue,Teal...Dark Green) player.

2. Trigger Condition

Conditions
I usually don't use this option but you can add something here as well!

Example: Add something like this so it only affects Undead units!

Conditions
((Hero manipulating item) is Undead) Equal to True

3. Making The Main Part of The Trigger

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item level of (Item being manipulated)) Equal to 1

Then - Actions
Set Item = 0

This is the main part of the trigger were you can change values of item Levels which you use like item Classes

You can change 1 into (0,1...n)

Also we are now seeing the first variable in our trigger: Item

You can press Ctrl+B to open a Variable window and then create one simple integer variable (You can name it 'Item' (I use that name here) or anything else you want)

Note: Will we meet another variable soon!

4. Item Counter

For each (Integer Item_Slot) from 1 to 6, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item level of (Item carried by (Hero manipulating item) in slot (Item_Slot))) Equal to 1

Then - Actions
Set Item = (Item + 1)

Else - Actions

This part of the trigger is used to count the number of items that hero carries (in our case it will count all level 1 items)

We can now see another variable: Item_Slot (create it like the first one (Item Integer))

I don't have the time to explain why I use that custom variable instead of just the well known one Integer A, but I will only say that: Integer A (I'm going to ignore Integer B since it is the same as Integer A and both function alike) is a predefined global variable. Thus, the information it might carry while looping could be changed due to any interception by other triggers which also contain Integer A. What happens then - once the looping of the original trigger resumes - is that the value of Integer A is different leading to faulty looping - i.e. bug! (We don't want that now do we )

5. Item drop + Text message (or not)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Item Greater than 1

Then - Actions
Hero - Drop (Item being manipulated) from (Hero manipulating item)

Game - Display to (All players) the text: You can carry only one weapon!

Else - Actions

Else - Actions
Finally we've come to the last part of the trigger!

Item Greater than 1 condition defines how many items per class will your hero carry!

Example: You can change it to 2 for Misc items

Hero - Drop (Item being manipulated) from (Hero manipulating item)

This Action will drop the item when a hero tries to pick it up. (Ground below Hero)

You can also add triggers that can:

Instantly move an item to any location that you want

Destroy Item (Why that?)

Create another item (You have crazy map who knows )

Game - Display to (All players) the text: You can carry only one weapon!

In this case, a text message will be shown to all players who tried to carry two weapon items.

Can be changed to show:

Any message to your allies

Any message to you

Any message in format of Floating Text

Any message in Format of Quest Message (Hint, Warning, Quest Update,...)

Also, you can even delete it if you don't want anything to be written on the screen!

This is simple and easy to understand I think

6. Example

This is only one of the many examples of how your trigger can look. This one is used in one silly map!

This trigger affect only 1 Player is not MUI!

(MUI what is that? For example: Multi Unit Instanceable (MUI) spell is a spell which would function normally if it's cast while another instance of the same spell is running.)

This trigger affect only melee units!

This tutorial works with items level 1,2 and 3

It is only example! Main trigger is above!

Item System
Events
Unit - A unit owned by Player 5 (Yellow) Acquires an item

Conditions
((Hero manipulating item) is A melee attacker) Equal to True

Actions
Set Item = 0

-------- Setting value of Item from random to 0 --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item level of (Item being manipulated)) Equal to 1

Then - Actions
For each (Integer Item_Slot) from 1 to 6, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item level of (Item carried by (Hero manipulating item) in slot Item_Slot)) Equal to 1

Then - Actions
Set Item = (Item + 1)

-------- It will increase to 1 if hero have 1 item level 1, to 2 if hero have 2 items level 1... --------

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Item Greater than 1

Then - Actions
Hero - Drop (Item being manipulated) from (Hero manipulating item)

Set LeakDestroy = Center of (Position of Circle of Power 0014 <gen>)

Item - Move (Last dropped item) to (LeakDestroy)

Custom script: call RemoveLocation(udg_LeakDestroy)

-------- You can see one new action: It will instantly move item to Circle of Power 0014! --------

-------- You can also see that I created another variable LeakDestroy (Variable Type: Point). Trigger can work without it but we need it because we want to destroy memory leak! --------

-------- What is memory leak, again I don't have time to explain what memory leak is because this tutorial want to teach you different thing --------

-------- But I can tell you that we clean (destroy) memory leaks only for one reason, we reduce lag, if you want to learn more, find tutorials about memory leaks on this site --------

Game - Display to (All enemies of (Owner of (Hero manipulating item))) for 10.00 seconds the text: Wanted to carry 2 F...

-------- It show enemy players for 10 seconds that Player who pick item wanted to carry 2 Flags at same time! --------

Else - Actions

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item level of (Item being manipulated)) Equal to 2

Then - Actions
For each (Integer Item_Slot) from 1 to 6, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item level of (Item carried by (Hero manipulating item) in slot Item_Slot)) Equal to 2

Then - Actions
Set Item = (Item + 1)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Item Greater than 3

Then - Actions
Hero - Drop (Item being manipulated) from (Hero manipulating item)

Game - Display to (Player group((Owner of (Hero manipulating item)))) for 10.00 seconds the text: Hero can carry only...

-------- It show to player for 10 seconds that he can't pick more than 3 potion items! --------

Else - Actions

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item level of (Item being manipulated)) Equal to 3

Then - Actions
For each (Integer Item_Slot) from 1 to 6, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item level of (Item carried by (Hero manipulating item) in slot Item_Slot)) Equal to 3

Then - Actions
Set Item = (Item + 1)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Item Greater than 3

Then - Actions
Hero - Drop (Item being manipulated) from (Hero manipulating item)

Game - Display to (Player group((Owner of (Hero manipulating item)))) for 10.00 seconds the text: Hero can carry only...

-------- It show to player for 10 seconds that he can't pick more than 3 potion items! --------

Else - Actions

Else - Actions

-------- Trigger can go to oo --------

Are you creating RPG map and you want to stack some more items into single one increasing it's charges!

Check this trigger example then:

Events
Unit - A unit Acquires an item

Conditions
Or - Any (Conditions) are true
Conditions
(Item-type of (Item being manipulated)) Equal to Greater Potion of Healing

(Item-type of (Item being manipulated)) Equal to Greater Potion of Mana

(Item-type of (Item being manipulated)) Equal to Lesser Potion of Healing

(Item-type of (Item being manipulated)) Equal to Lesser Potion of Mana

(Item-type of (Item being manipulated)) Equal to Potion of Healing

(Item-type of (Item being manipulated)) Equal to Potion of Mana

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Hero manipulating item) has an item of type (Item-type of (Item being manipulated))) Equal to True

(Item being manipulated) Not equal to (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated)))

Then - Actions
Item - Set charges remaining in (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated))) to ((Charges remaining in (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated)))) + (Charges remaining in (Item being manipulated)))

Hero - Drop (Item being manipulated) from (Hero manipulating item)

Item - Remove (Last dropped item)

Else - Actions

Author's notes:

This IDS is not perfect! You can find a lot better systems than mine (as well as a lot of worse ones! And I do mean A LOT!!!)

If this tutorial helped you to understand how the IDS works (or at least gave you a glimpse of it), then my job here is done!

If you want to learn more about Memory Leaks or Integer A check this tutorial

THIS MESSAGE HAS BEEN SPOOFED FOR GRAMMAR AND ETHICAL ERRORS BY: The_Mirai

#### Attachments

Item Drop System.w3x

 18.5 KB

 · Views: 1,006




 Last edited: Jul 20, 2010

#### Darkness-4ever

#### Darkness-4ever

 Level 9

JoinedNov 4, 2007Messages932

Maybe you haven't heard, but recently the tutorial moderator had a tornado run right through his house, so he won't be approving anything anytime soon.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Darkness-4ever said:

 Maybe you haven't heard, but recently the tutorial moderator had a tornado run right through his house, so he won't be approving anything anytime soon.


Click to expand...


Moderators : 2


Pyritie, hawk900


Tornado?

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

*It's not MUI.


*Lacks "Unit loses an item" trigger.


*You should present the item caps in a better way. Use an initialization trigger.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Maker said:

 *It's not MUI.


Click to expand...

I know what MUI is but ?

Maker said:

 *Lacks "Unit loses an item" trigger.


Click to expand...

I don't understand what you want to say?

Maker said:

 *You should present the item caps in a better way. Use an initialization trigger.


Click to expand...

Item Caps in better way? Can you pls select text from above or write where I make mistake


 Last edited: Mar 30, 2010

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

-Kobas- said:

 I know that it is GUI, I wrote MUI somewere or?


I don't understand what you want to say?


Click to expand...


Now that I look at it, it seems to be MUI afterall. Never mind that "loses and item" trigger. I misinterpreted the trigger at first.


You could move that Items = 0 at the very beginning of actions, since it is set anyway, regardless of the item's level.

 Item Caps in better way? Can you pls select text from above or write where I make mistake


Click to expand...


It's not a mistake per se. It just would be easier to use variables for those item caps, and set them in an initialization trigger rather than using hard coded values inside the code.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Maker said:

 Now that I look at it, it seems to be MUI afterall. Never mind that "loses and item" trigger. I misinterpreted the trigger at first.

You could move that Items = 0 at the very beginning of actions, since it is set anyway, regardless of the item's level.


Click to expand...

Oh, I see what you want to say, oki it will be done! Thanks

Maker said:

 It's not a mistake per se. It just would be easier to use variables for those item caps, and set them in an initialization trigger rather than using hard coded values inside the code.


Click to expand...

Maybe you are right, bit as I write above this tutorial was created to teach people something. I thinks that even dude who first time open WE can look at this and say: Wow I can do that!

That is the point! I don't want to teach people like you, because you already knows this (Maybe in different way)

P.S. if more people want from me to make it that way it shall be done

#### Etzer

#### Etzer

 Level 13

JoinedFeb 18, 2009Messages1,378

Tornado?

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Darkness-4ever said:

 Maybe you haven't heard, but recently the tutorial moderator had a tornado run right through his house, so he won't be approving anything anytime soon.


Click to expand...


That tornado


Crazy joke


Btw, I asked moderator to check tutorial and he say OK!

#### Vakama_Lynx

#### Vakama_Lynx

 Level 7

JoinedMay 8, 2009Messages102

Resources: 2

#### Resources
Models 1Templates 1

Useful. lol better than that "you can only carry one item of this type" message that pops up and is so annoying

#### Kwah

#### Kwah

 Level 24

JoinedMay 9, 2007Messages3,561

Resources: 12

#### Resources
Icons 7Maps 3Tutorials 2

I've done a few things to a portion of the tutorial to demonstrate some of the ideas that I want you to address. I mostly left the colours and formatting you had, and the formatting overall was fine. However, the way you write isn't really good for a tutorial setting. Don't CnP what I wrote, obviously and feel free to keep your own spin on it. However, the edited (first bit) and commented (latter bit), portion that I've shown should cover most of the issues I have with the tutorial. Go through and fix these problems throughout the rest of the tutorial and bump the thread when you've done some work.


Nonetheless, it's a good start, and one that does have potential.

Item Drop System

 Requirments


 A basic knowledge of the Object Editor

 A basic knowledge of the Trigger Editor

 Limited knowledge in variables.

 A moment of your time

Introduction


You may find yourself asking, what is an Item Drop System (IDS)? Let me explain:


If you ever have played a map like Gaia's Retaliation you know that you are allowed to carry only 1 item of the same type (Weapon, Armor, etc.). Meaning, that if a player picks up an item of a type which was already in his inventory, it will be automatically dropped, and therefore player will be un-able to carry two items of this type simultaneously. It's simple as that!


How does this system work?


The system uses the level of items to create your own classes rather than being limited to using only the number of classes available in the game.

 The Bad



Permanent --------- Weapon


Campaign ---------- Armor


Power Up ---------- Shield


...


Purchasable -------- Rock


 The Good



Level 1. ---------------- Weapon


Level 2. --------------- Shield


Level 3. ---------------- Potion


Level 10. --------------- Boots


Level 451. ------------- Thingamawatsits


With the levels you can use an effectively infinite number of your own item classes, whereas with the old method, you are limited to just a few!

Let's start!


1. You must know that this trigger affects only one Class (Ofc we will use item level) so that people can learn how to actually make the IDS and not just copy-paste it. But still, at the end of this tutorial you will find system which I used in my map or an example of how it can look. So be patient and try to learn! Here is an example of the reader not really having a clue what you're talking about. The phrase "You must know that this trigger affects only one Class (Ofc we will use item level), isn't easily comprehensible. Really try to work on this, as obviously understanding is key to having a good tutorial.


2. Ok, you can now open your map or create new one if you want to test the system first! The wording is fine, but it just looks un-professional. Having a casual tone of voice is fine, and often a great thing in a tutorial. You probably want something more like "Okay, now you can open your map up, and test the system out!".


Subtle grammar can really help add to the polish.


3. Create new trigger! (You pick the name for it )


4. Then just modify it like that one below (It will drop the second level 1 item if a player tries to acquire another level 1 item) Again, clarify.


5. Change all item levels to 1,2..n for each class that you use or else this trigger will fail! More clarification!


Hint: What will happen if you want to change item level to level 9,10...n,


you can't! Why then I wrote you can make n item classes


I will show you magic: Shift+One Left Mouse Click+Another One Left Mouse Click = OMG you can put there any number you want Here I'd try to phrase it more like "You'll find that you can't enter the number you want! I'll show you a little bit of magic. If you select Item Level, and while holding down Shift, hit Enter you'll be able to enter whatever number you want!


You can use it for everything else from making negative mana and life regeneration to negative Hit points (omg who will use that). It's great that you mention this, but I pretty much refuse to accept a tutorial that uses the word omg in it. Keep it clean.


Btw here is the trigger! Btw has the same stamp of death as omg. Use full words plz. Also, you probably want to introduce what 'the trigger' is.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

hawk900 said:

 I've done a few things to a portion of the tutorial to demonstrate some of the ideas that I want you to address. I mostly left the colours and formatting you had, and the formatting overall was fine. However, the way you write isn't really good for a tutorial setting. Don't CnP what I wrote, obviously and feel free to keep your own spin on it. However, the edited (first bit) and commented (latter bit), portion that I've shown should cover most of the issues I have with the tutorial. Go through and fix these problems throughout the rest of the tutorial and bump the thread when you've done some work.


Nonetheless, it's a good start, and one that does have potential.

Item Drop System

 Requirments


 A basic knowledge of the Object Editor

 A basic knowledge of the Trigger Editor

 Limited knowledge in variables.

 A moment of your time

Introduction


You may find yourself asking, what is an Item Drop System (IDS)? Let me explain:


If you ever have played a map like Gaia's Retaliation you know that you are allowed to carry only 1 item of the same type (Weapon, Armor, etc.). Meaning, that if a player picks up an item of a type which was already in his inventory, it will be automatically dropped, and therefore player will be un-able to carry two items of this type simultaneously. It's simple as that!


How does this system work?


The system uses the level of items to create your own classes rather than being limited to using only the number of classes available in the game.

 The Bad



Permanent --------- Weapon


Campaign ---------- Armor


Power Up ---------- Shield


...


Purchasable -------- Rock


 The Good



Level 1. ---------------- Weapon


Level 2. --------------- Shield


Level 3. ---------------- Potion


Level 10. --------------- Boots


Level 451. ------------- Thingamawatsits


With the levels you can use an effectively infinite number of your own item classes, whereas with the old method, you are limited to just a few!

Let's start!


1. You must know that this trigger affects only one Class (Ofc we will use item level) so that people can learn how to actually make the IDS and not just copy-paste it. But still, at the end of this tutorial you will find system which I used in my map or an example of how it can look. So be patient and try to learn! Here is an example of the reader not really having a clue what you're talking about. The phrase "You must know that this trigger affects only one Class (Ofc we will use item level), isn't easily comprehensible. Really try to work on this, as obviously understanding is key to having a good tutorial.


2. Ok, you can now open your map or create new one if you want to test the system first! The wording is fine, but it just looks un-professional. Having a casual tone of voice is fine, and often a great thing in a tutorial. You probably want something more like "Okay, now you can open your map up, and test the system out!".


Subtle grammar can really help add to the polish.


3. Create new trigger! (You pick the name for it )


4. Then just modify it like that one below (It will drop the second level 1 item if a player tries to acquire another level 1 item) Again, clarify.


5. Change all item levels to 1,2..n for each class that you use or else this trigger will fail! More clarification!


Hint: What will happen if you want to change item level to level 9,10...n,


you can't! Why then I wrote you can make n item classes


I will show you magic: Shift+One Left Mouse Click+Another One Left Mouse Click = OMG you can put there any number you want Here I'd try to phrase it more like "You'll find that you can't enter the number you want! I'll show you a little bit of magic. If you select Item Level, and while holding down Shift, hit Enter you'll be able to enter whatever number you want!


You can use it for everything else from making negative mana and life regeneration to negative Hit points (omg who will use that). It's great that you mention this, but I pretty much refuse to accept a tutorial that uses the word omg in it. Keep it clean.


Btw here is the trigger! Btw has the same stamp of death as omg. Use full words plz. Also, you probably want to introduce what 'the trigger' is.

Click to expand...


Thanks a lot man, I will soon edit tutorial and ask The_Mirai to check grammar again!

#### Kwah

#### Kwah

 Level 24

JoinedMay 9, 2007Messages3,561

Resources: 12

#### Resources
Icons 7Maps 3Tutorials 2

Approved, feel free to make more changes.

#### Garfield1337

#### Garfield1337

 Level 20

JoinedJul 6, 2009Messages1,885

Resources: 4

#### Resources
Maps 1Spells and Systems 3

To make it functional for every item class (If there is large amount of classes)


you may use this:

Item System
Events
Unit - A unit Acquires an item

Conditions

Actions
Set Item = 0

For each (Integer Slot_Integer) from 1 to 6, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item level of (Item carried by (Hero manipulating item) in slot Slot_Integer)) Equal to (Item level of (Item being manipulated))

Then - Actions
Set Item = (Item + 1)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Item Greater than 1

Then - Actions
Hero - Drop (Item being manipulated) from (Hero manipulating item)

Game - Display to (All players) for 5.00 seconds the text: Can't carry more th...

Else - Actions

However,it will drop acquired item whenever you have 2 of it's type,no matter what level that item is.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

 However,it will drop acquired item whenever you have 2 of it's type,no matter what level that item is.


Click to expand...

Well if you use potions in your ORPG map, this trigger is fail, because you maybe want to stack them!

Example

Events
Unit - A unit Acquires an item

Conditions
Or - Any (Conditions) are true
Conditions
(Item-type of (Item being manipulated)) Equal to Greater Potion of Healing

(Item-type of (Item being manipulated)) Equal to Greater Potion of Mana

(Item-type of (Item being manipulated)) Equal to Lesser Potion of Healing

(Item-type of (Item being manipulated)) Equal to Lesser Potion of Mana

(Item-type of (Item being manipulated)) Equal to Potion of Healing

(Item-type of (Item being manipulated)) Equal to Potion of Mana

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Hero manipulating item) has an item of type (Item-type of (Item being manipulated))) Equal to True

(Item being manipulated) Not equal to (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated)))

Then - Actions
Item - Set charges remaining in (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated))) to ((Charges remaining in (Item carried by (Hero manipulating item) of type (Item-type of (Item being manipulated)))) + (Charges remaining in (Item being manipulated)))

Hero - Drop (Item being manipulated) from (Hero manipulating item)

Item - Remove (Last dropped item)

Else - Actions

Well after all it can be used for sure, there will be always some unique map, so your trigger can find some use

#### Garfield1337

#### Garfield1337

 Level 20

JoinedJul 6, 2009Messages1,885

Resources: 4

#### Resources
Maps 1Spells and Systems 3

You can put

(Item-class of (Item being manipulated)) Not equal to Charged
In main trigger condition,or just list items that shouldn't be dropped there.

#### Arisato

#### Arisato

 Level 18

JoinedDec 17, 2009Messages1,114

Resources: 25

#### Resources
Models 10Icons 1Maps 8Spells and Systems 1Tutorials 5

Perfect!


But there "is" a more simplified way by just using booleans


like if a unit takes an item, that boolean will be true, and if he take another item with the same level, and the trigger checks that if the boolean is true, it will pop out a game text message and drop the item ^_^!


That way that is MUI, but this one is just perfect too, we have more advantage at doing this that and more

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Well I told above already: example trigger isn't MUI because I wanted to show all what can be created! You can simple add different event and fix some actions or events and it will become MUI

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

BUMP

Bugbuster96, dude, you have time until end of this day to check and prove your bug!

In case you are wrong I want to ask you to delete your posts!

Thanks

Nothing personal, I just don't want to see comments like that here

#### Bugbuster96

#### Bugbuster96

 Level 7

JoinedAug 11, 2009Messages163

Resources: 5

#### Resources
Maps 2Spells 1Templates 1Tutorials 1

Kobas i am sorry for wasting your time, you were right, I got confused of the items.

btw i read all before I've commented. -.-

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

No problem dude, I asked you for that so people don't start bothering me about it!

#### Lich Prince

#### Lich Prince

Patron

 Level 12

JoinedMar 18, 2008Messages865

Very useful tutorial, especially for those many RPG-maker beginners.

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

Oh my... This is indeed a quite good tutorial, and I'm already finding a good use of it in my project. Thank you very much, I really appreciate your tutorial creations.

+rep

#### Arcsol

#### Arcsol

 Level 7

JoinedOct 26, 2005Messages190

Resources: 8

#### Resources
Icons 7Maps 1

Thanks a lot for this tut! It really helped me a bunch

#### SnowYell

#### SnowYell

 Level 10

JoinedJun 13, 2010Messages398

Pretty simple and great explaining!

Ehm, is this MUI? Like does it work for multiple players looting and not sharing the same Variables?

Edit: Okay I realize the Variable resets and overwrites other players using it. So it will just run over and over again, counting.. So I guess it is MUI.

Great! Thanks!


 Last edited: Oct 7, 2015

#### BPower

#### BPower

 Level 19

JoinedMar 18, 2012Messages1,716

Resources: 23

#### Resources
Spells and Systems 17JASS 5Tutorials 1

Yes it's MUI, but the execution in the example is done very poor.

Here is the code you should use:

Item System
Events
Unit - A unit Acquires an item

Conditions

Actions
Set IDS_Level = (Item level of (Item being manipulated))

Set IDS_Unit = (Triggering unit)

Set IDS_Item = 0

Set IDS_Size = (Size of inventory for IDS_Unit)

For each (Integer IDS_LoopInteger) from 1 to IDS_Size, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item level of (Item carried by IDS_Unit in slot IDS_LoopInteger)) Equal to IDS_Level

Then - Actions
Set IDS_Item = (IDS_Item + 1)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
IDS_Item Greater than 1

Then - Actions
Hero - Drop (Item being manipulated) from IDS_Unit

Game - Display to (All players) for 5.00 seconds the text: (Can't carry more than one level + ((String(IDS_Level)) + Item!))

Else - Actions

Set IDS_Unit = No unit

It only requires one condition block, because the item level is saved to an integer variable, which is used in the if/the/else comparison.

So now it compares picked item level == item level in slot, instead of x == item level in slot.

Inventory size (IDS_Size) variable can make the loop smaller, as some units don't have 6 inventory slots, but just 2.

IDS_Unit is just used to optimize the performance a bit.

 You must log in or register to reply here.


### Similar threads

 Item-drop-system


SlayerII

Mar 6, 2009

Trigger (GUI) Editor Tutorials

Replies24Views20K

Feb 28, 2010
SlayerII

Locked

[Trigger]
 Ids problems.


Smargoos

May 1, 2011

Triggers & Scripts

Replies6Views2K

May 5, 2011
Smargoos

 ILS (Item Level Up System)


-Kobas-

Jul 24, 2010

Trigger (GUI) Editor Tutorials

Replies11Views23K

Jul 13, 2011
Ignitedstar

 RDZ's Incredible Item Stacking System of Doom


Rao Dao Zao

Oct 5, 2005

Trigger (GUI) Editor Tutorials

Replies1Views22K

Feb 15, 2015
emperor_d3st

 Stacking Items


DigitalDemon

Dec 21, 2007

Trigger (GUI) Editor Tutorials

Replies18Views27K

Sep 18, 2025
Luashine

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