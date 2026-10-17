---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ils-item-level-up-system.173891/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Ils Item Level Up System
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ils-item-level-up-system.173891/>
#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

 Item Drop System by -Kobas-


Item Level Up System

by -Kobas-

Requirements

 * Basic knowledge of the Object Editor

 * Basic knowledge of the Trigger Editor

 * Poor knowledge in variables.

 * A moment of your time  (Around 5 min).

Introduction

Have you ever wondered what the Item Level UP System (ILS) is? If your answer is ‘no’, then allow me to explain it for you:

If you’ve ever played a map such as Pudge Wars or Dota, you know that some items have levels (Example for item with levels from Dota is Dagon).

With this system you will be able to create items that will be upgradable...

How does my system works?

My system use item type variables (array) to define item levels!

And here’s an example:

# Set ILS_Hero = (Hero manipulating item)

# Set ILS_Items[1] = Item - Recipe (Item that we buy from shop)

# Set ILS_Items[2] = Item - Level 1/5 (Hero gain this item)

# Set ILS_Items[3] = Item - Level 2/5 (Hero gain this item)

# Set ILS_Items[4] = Item - Level 3/5 (Hero gain this item)

# Set ILS_Items[5] = Item - Level 4/5 (Hero gain this item)

# Set ILS_Items[6] = Item - Level 5/5 (Hero gain this item)

You must be wondering: “How many levels my system can have"

Well answer will be: "unlimited"...

This example show setup trigger for item with 5 levels!

Let's start!

1. Okay, let’s start making our ILS now! I know, you will maybe be surprised as how to make an entire system and pack it into a single trigger, well it’s impossible! And here’s an example why:

A) Imagine we have a 100 levels per item; that means that the trigger would have a 100 parts with each one containing at least 5 commands; that’s 500 commands (or 100 Microsoft Word pages if you want)! Therefore, it’s impossible to make the ILS, not to mention what happens with 1 000 or 1 000 000 item levels.

B) On the other hand, a system can be made with more ways than one: it can be sqeezed into a lot smaller and much more complicated code (I really don’t want to get into that), or it can be separated into several, simpler, triggers. Basically, it can be exactly like you need (want) it to be which is a plus because you can adjust it to your needs.

That’s why I’ll try my best to explain to you the essense of making a ILS trigger!

2. You can now open your map to test the new code we’re making.

3. Create a new trigger and, of course, choose a name for it.

4. Once you’ve done that, copy this code in it (in other words, make your trigger looking like this one here).

ILS Item Level Up System
Events

Conditions

Actions
Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(ILS_Hero has an item of type ILS_Items[2]) Equal to (==) True

Then - Actions
Hero - Drop (Item carried by ILS_Hero of type ILS_Items[1]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Drop (Item carried by ILS_Hero of type ILS_Items[2]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Create ILS_Items[3] and give it to ILS_Hero

Special Effect - Create a special effect attached to the origin of ILS_Hero using Abilities\Spells\Demon\DarkPortal\DarkPortalTarget.mdl

Special Effect - Destroy (Last created special effect)

Else - Actions
Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(ILS_Hero has an item of type ILS_Items[3]) Equal to (==) True

Then - Actions
Hero - Drop (Item carried by ILS_Hero of type ILS_Items[1]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Drop (Item carried by ILS_Hero of type ILS_Items[3]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Create ILS_Items[4] and give it to ILS_Hero

Special Effect - Create a special effect attached to the origin of ILS_Hero using Abilities\Spells\Demon\DarkPortal\DarkPortalTarget.mdl

Special Effect - Destroy (Last created special effect)

Else - Actions
Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(ILS_Hero has an item of type ILS_Items[4]) Equal to (==) True

Then - Actions
Hero - Drop (Item carried by ILS_Hero of type ILS_Items[1]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Drop (Item carried by ILS_Hero of type ILS_Items[4]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Create ILS_Items[5] and give it to ILS_Hero

Special Effect - Create a special effect attached to the origin of ILS_Hero using Abilities\Spells\Demon\DarkPortal\DarkPortalTarget.mdl

Special Effect - Destroy (Last created special effect)

Else - Actions
Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(ILS_Hero has an item of type ILS_Items[5]) Equal to (==) True

Then - Actions
Hero - Drop (Item carried by ILS_Hero of type ILS_Items[1]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Drop (Item carried by ILS_Hero of type ILS_Items[5]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Create ILS_Items[6] and give it to ILS_Hero

Special Effect - Create a special effect attached to the origin of ILS_Hero using Abilities\Spells\Demon\DarkPortal\DarkPortalTarget.mdl

Special Effect - Destroy (Last created special effect)

Else - Actions
Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(ILS_Hero has an item of type ILS_Items[6]) Equal to (==) True

Then - Actions
Hero - Drop (Item carried by ILS_Hero of type ILS_Items[1]) from ILS_Hero

Item - Remove (Last dropped item)

Player - Add ItemGoldCost to (Owner of ILS_Hero) Current gold

Else - Actions
Hero - Drop (Item carried by ILS_Hero of type ILS_Items[1]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Create ILS_Items[2] and give it to ILS_Hero

Special Effect - Create a special effect attached to the origin of ILS_Hero using Abilities\Spells\Demon\DarkPortal\DarkPortalTarget.mdl

Special Effect - Destroy (Last created special effect)

5. You noticed that this trigger uses 5 item levels!

6. Now we need setup trigger that will use this one above:

Here copy code below:

ILS Setup
Events
Unit - A unit Acquires an item

Conditions

Actions
Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item-type of (Item being manipulated)) Equal to (==) Ancient Sword

Then - Actions
Set ILS_Hero = (Hero manipulating item)

Set ILS_Items[1] = Ancient Sword

Set ILS_Items[2] = Ancient Sword - Level 1/5

Set ILS_Items[3] = Ancient Sword - Level 2/5

Set ILS_Items[4] = Ancient Sword - Level 3/5

Set ILS_Items[5] = Ancient Sword - Level 4/5

Set ILS_Items[6] = Ancient Sword - Level 5/5

-------- xxxxxxxxxxxxxxxxxxxxxxxxx --------

Trigger - Run ILS Item Level Up System <gen> (ignoring conditions)

Else - Actions

7. Save the map and go ahead and test it. You will notice that hero item will be upgraded when you buy item recipe again! If that’s the case, then you succeeded in making an ILS! Congratulations!

Now you might be thinking:

1. “Hey, this is so easy! How come I didn’t think of it before?!” If you’re thinking like this, then you already know enough about ILS to make one yourself. And there are a few ways to do it from this tutorial:

A) Copy the trigger several times and only set your own item levels for your item. (the easiest, but the most unprofessional way to do it).

B) Copy the actions inside the trigger and set item levels there (easier, but still an unprofessional way).

C) Copy the parts of the actions and the rest put under a same condition for all items that have same number of upgrades... (the hardest, but the most professional way)

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

Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item-type of (Item being manipulated)) Equal to Soul

Then - Actions
Set ItemGoldCost = 10000

Set ILS_Hero = (Hero manipulating item)

Set ILS_Items[1] = Soul Gem

Set ILS_Items[2] = Claws of Attack +3

Set ILS_Items[3] = Claws of Attack +6

Set ILS_Items[4] = Claws of Attack +9

Set ILS_Items[5] = Claws of Attack +12

Set ILS_Items[6] = Claws of Attack +15

-------- xxxxxxxxxxxxxxxxxxxxxxxxx --------

Trigger - Run ILS Item Level Up System <gen> (ignoring conditions)

Else - Actions

This is the main part of the trigger were you can change values of item Levels or Item Gold Cost!

Also we are now seeing the first variable in our trigger: ItemGoldCost

You can press Ctrl+B to open a Variable window and then create one simple integer variable (You can name it 'ItemGoldCost' (I use that name here) or anything else you want)

Now we need to create Unit variable: name "ILS_Hero"

And at the end item type variable (array) name "ILS_Items"

4. Item Level Counter

Then - Actions
Hero - Drop (Item carried by ILS_Hero of type ILS_Items[1]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Drop (Item carried by ILS_Hero of type ILS_Items[2]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Create ILS_Items[3] and give it to ILS_Hero

Special Effect - Create a special effect attached to the origin of ILS_Hero using Abilities\Spells\Demon\DarkPortal\DarkPortalTarget.mdl

Special Effect - Destroy (Last created special effect)

This part of the trigger will check item level and increase it size or remove recipe and return money:

Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(ILS_Hero has an item of type ILS_Items[6]) Equal to (==) True

Then - Actions
Hero - Drop (Item carried by ILS_Hero of type ILS_Items[1]) from ILS_Hero

Item - Remove (Last dropped item)

Player - Add ItemGoldCost to (Owner of ILS_Hero) Current gold

Else - Actions
Hero - Drop (Item carried by ILS_Hero of type ILS_Items[1]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Create ILS_Items[2] and give it to ILS_Hero

Special Effect - Create a special effect attached to the origin of ILS_Hero using Abilities\Spells\Demon\DarkPortal\DarkPortalTarget.mdl

Special Effect - Destroy (Last created special effect)

5. Item drop + Text message (or not)

Special Effect - Create a special effect attached to the origin of ILS_Hero using AncientExplode.mdx

Special Effect - Destroy (Last created special effect)
This one will create special effect at hero position if item gain new level!

You can also use something like:

Set TempPoint = (Position of (Hero manipulating item))

Floating Text - Create floating text that reads xxxxxxx at (TempPoint) with Z offset 0.00, using font size 10.00, color (100.00%, 100.00%, 100.00%), and 0.00% transparency

Custom Script: call RemoveLocation (udg_TempPoint)
To show message above hero...

Game - Display to (All players) the text: Your item is upgraded!

In this case, a text message will be shown to all players.

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

This tutorial works with items that have 5 levels

It is only example! Main trigger is above!

ILS Setup
Events
Unit - A unit owned by Player 5 (Yellow) Acquires an item

Conditions
((Hero manipulating item) is A melee attacker) Equal to True

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item-type of (Item being manipulated)) Equal to Soul

Then - Actions
Set ItemGoldCost = 5000

Set ILS_Hero = (Hero manipulating item)

Set ILS_Items[1] = Soul

Set ILS_Items[2] = Shadow Orb +1

Set ILS_Items[3] = Shadow Orb +2

Set ILS_Items[4] = Shadow Orb +3

Set ILS_Items[5] = Shadow Orb +4

Set ILS_Items[6] = Shadow Orb +5

-------- xxxxxxxxxxxxxxxxxxxxxxxxx --------

Trigger - Run ILS Item Level Up System <gen> (ignoring conditions)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Item-type of (Item being manipulated)) Equal to Soul Gem

Then - Actions
Set ItemGoldCost = 10000

Set ILS_Hero = (Hero manipulating item)

Set ILS_Items[1] = Soul Gem

Set ILS_Items[2] = Claws of Attack +3

Set ILS_Items[3] = Claws of Attack +6

Set ILS_Items[4] = Claws of Attack +9

Set ILS_Items[5] = Claws of Attack +12

Set ILS_Items[6] = Claws of Attack +15

-------- xxxxxxxxxxxxxxxxxxxxxxxxx --------

Trigger - Run ILS Item Level Up System <gen> (ignoring conditions)

Else - Actions

ILS Item Level Up System
Events

Conditions

Actions
Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(ILS_Hero has an item of type ILS_Items[2]) Equal to (==) True

Then - Actions
Hero - Drop (Item carried by ILS_Hero of type ILS_Items[1]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Drop (Item carried by ILS_Hero of type ILS_Items[2]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Create ILS_Items[3] and give it to ILS_Hero

Special Effect - Create a special effect attached to the origin of ILS_Hero using Abilities\Spells\Demon\DarkPortal\DarkPortalTarget.mdl

Special Effect - Destroy (Last created special effect)

Else - Actions
Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(ILS_Hero has an item of type ILS_Items[3]) Equal to (==) True

Then - Actions
Hero - Drop (Item carried by ILS_Hero of type ILS_Items[1]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Drop (Item carried by ILS_Hero of type ILS_Items[3]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Create ILS_Items[4] and give it to ILS_Hero

Special Effect - Create a special effect attached to the origin of ILS_Hero using Abilities\Spells\Demon\DarkPortal\DarkPortalTarget.mdl

Special Effect - Destroy (Last created special effect)

Else - Actions
Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(ILS_Hero has an item of type ILS_Items[4]) Equal to (==) True

Then - Actions
Hero - Drop (Item carried by ILS_Hero of type ILS_Items[1]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Drop (Item carried by ILS_Hero of type ILS_Items[4]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Create ILS_Items[5] and give it to ILS_Hero

Special Effect - Create a special effect attached to the origin of ILS_Hero using Abilities\Spells\Demon\DarkPortal\DarkPortalTarget.mdl

Special Effect - Destroy (Last created special effect)

Else - Actions
Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(ILS_Hero has an item of type ILS_Items[5]) Equal to (==) True

Then - Actions
Hero - Drop (Item carried by ILS_Hero of type ILS_Items[1]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Drop (Item carried by ILS_Hero of type ILS_Items[5]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Create ILS_Items[6] and give it to ILS_Hero

Special Effect - Create a special effect attached to the origin of ILS_Hero using Abilities\Spells\Demon\DarkPortal\DarkPortalTarget.mdl

Special Effect - Destroy (Last created special effect)

Else - Actions
Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(ILS_Hero has an item of type ILS_Items[6]) Equal to (==) True

Then - Actions
Hero - Drop (Item carried by ILS_Hero of type ILS_Items[1]) from ILS_Hero

Item - Remove (Last dropped item)

Player - Add ItemGoldCost to (Owner of ILS_Hero) Current gold

Else - Actions
Hero - Drop (Item carried by ILS_Hero of type ILS_Items[1]) from ILS_Hero

Item - Remove (Last dropped item)

Hero - Create ILS_Items[2] and give it to ILS_Hero

Special Effect - Create a special effect attached to the origin of ILS_Hero using Abilities\Spells\Demon\DarkPortal\DarkPortalTarget.mdl

Special Effect - Destroy (Last created special effect)

Author's notes:

This ILS is not perfect! You can find a lot better systems than mine (as well as a lot of worse ones! And I do mean A LOT!!!)

If this tutorial helped you to understand how the ILS works (or at least gave you a glimpse of it), then my job here is done!

If you want to learn more about Memory Leaks or Integer A check this tutorial

This system works fine with my Item Drop System triggers will work perfectly fine for each one...

You can combine them to create awesome stuff with items for your maps!

#### Attachments

Item Level Up System.w3x

 19 KB

 · Views: 2,044



#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

I don't have time right now, but I will fix triggers (add comments)

and check grammar for errors!

I hope that this tutorial can help you to understand items better!

#### Darkzealot_128

#### Darkzealot_128

 Level 12

JoinedJun 29, 2010Messages785

Resources: 10

#### Resources
Maps 6Spells 1Tutorials 3

This is a very well explained tutorial and also useful. gj

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Thanks but I still need to add comments into triggers as well!

But I am 2 busy, and maybe because of that lazy, these days!

#### Pharaoh_

#### Pharaoh_

 Level 40

JoinedNov 6, 2008Messages8,316

Resources: 13

#### Resources
Icons 4Spells and Systems 7User Interface 1Tutorials 1

To be completely honest, this doesn't enhance the knowledge of a user over a certain area in the World Editor. It should be released within the Spells section, not the Tutorials one. However, if people find it useful, then, so be it.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Pharaoh_ said:

 To be completely honest, this doesn't enhance the knowledge of a user over a certain area in the World Editor. It should be released within the Spells section, not the Tutorials one. However, if people find it useful, then, so be it.


Click to expand...

Thanks for opinion dude, well I will wait for tut mod to say something and then maybe upload it as system, who knows

#### Lich Prince

#### Lich Prince

Patron

 Level 12

JoinedMar 18, 2008Messages865

Nah Pharaoh_, it clearly describes how to do the ILS. I always wondered how they did it with those couple of items in DotA.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I think it has enough info to be approved. Nice job and great formatting.


~Approved.

#### Orcnet

#### Orcnet

 Level 30

JoinedJul 31, 2010Messages5,248

Resources: 5

#### Resources
Maps 4Tutorials 1

All I can say is "Great!!!!"

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

*Deep Chanting*

Please be MUI... Please be MUI... Please be...

#### xXxScoxXx

#### xXxScoxXx

 Level 2

JoinedJul 12, 2011Messages25

Quick question.

I'm new to editing, sorry. Variable are slightly confusing to me. How would i create the variables in this instance? I can't find a way to make an Item Level variable.

#### Ignitedstar

#### Ignitedstar

 Level 5

JoinedJun 30, 2004Messages170

defskull said:

*Deep Chanting*

Please be MUI... Please be MUI... Please be...

Click to expand...

There are no wait functions, so it is automatically MUI. >_>

Leaks are different story, though.

 You must log in or register to reply here.


### Similar threads

 Item-drop-system


SlayerII

Mar 6, 2009

Trigger (GUI) Editor Tutorials

Replies24Views20K

Feb 28, 2010
SlayerII

 IDS (Infinite Number of Item Classes)


-Kobas-

Mar 29, 2010

Trigger (GUI) Editor Tutorials

Replies25Views47K

Oct 7, 2015
BPower

 Triggered Item Drops


Zinox

Jan 8, 2011

Trigger (GUI) Editor Tutorials

Replies24Views31K

Feb 10, 2021
PaulieD

S

Locked

 Need Help w/ Item Doubler


sentrywiz

Sep 21, 2011

World Editor Help Zone

Replies5Views1K

Oct 4, 2011
sentrywiz

S

 RDZ's Incredible Item Stacking System of Doom


Rao Dao Zao

Oct 5, 2005

Trigger (GUI) Editor Tutorials

Replies1Views22K

Feb 15, 2015
emperor_d3st

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