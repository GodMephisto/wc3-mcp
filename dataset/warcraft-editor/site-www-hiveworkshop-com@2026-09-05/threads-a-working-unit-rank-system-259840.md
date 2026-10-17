---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/a-working-unit-rank-system.259840/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# A Working Unit Rank System
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/a-working-unit-rank-system.259840/>
#### GenFrogKing

#### GenFrogKing

 Level 3

JoinedNov 25, 2014Messages24

Resources: 1

#### Resources
Tutorials 1

A Working Unit Rank System

~ A Tutorial ~

I have completed a unit rank system that I intend to use for my SciFi mod (Title: Dubious But Forthcoming). I wish to share the rank systems' functionality because I don't think anyone else has done it quite the same way as I have, and I, of course, believe someone may find this useful!

What Makes a Unit Rank System?

Of course, when one sets out to make a system such as this, he or she inevitably wants to do everything done in other popular games out there. The ex-Command & Conquer series (because 4 killed it) is a good example of this. Some of the common characteristics these unit rank systems have are:

-A visible kill count

-A visible rank GUI

-Unit perks per rank (less common)

-Multible Unit Instanceability (every unit ranks up)​

Some Immediate Ideas:

At first, one might think of remaking their faction entirely out of heroes and simply setting the stats like "Stats - Hero - Hide Hero Death Message" to true. That way you don't have to be berated by something reminiscent of Bungie's old Myth series... "Casualties." (You're real old if you know what I'm talking about.) Other people tend towards using abilities to display rank and kills. And still others will use hashtables or JASS which, although useful, are more complicated than we need for this task!

The problem with the hero system is that it doesn't adequately show kill count, and I suspect there could be errors in the game's engine if you started creating armies of over a hundred units that were technically heroes. You could set experience gained for every unit to 1, but then you have a giant loading bar displaying kill count; that wouldn't really look appealing. Plus, what if you wanted actual heroes in your game anyways? Abilities are a viable option but are a hassle because of the 100 tooltips you'd have to program- not to mention the fact that you could only tally 100 kills.

So what's the optimal choice? Glad you asked! Attached are some files you may download and follow along with. I recommend you import Rank_Color.blp, Rank_Corporal.mdx, DISPASBTN_Rank_Corporal.tga, DISPASBTN_Rank_Conscript.tga, PASBTN_Rank_Corporal.tga, and PASBTN_Rank_Conscript.tga into a map if you wish to do so.

Set the directories for the image files as so:

Rank_Color.blp

ReplaceableTextures\PassiveButtons\PASBTN_Rank_Conscript.tga

ReplaceableTextures\PassiveButtons\PASBTN_Rank_Corporal.tga

ReplaceableTextures\CommandButtonsDisabled\DISPASBTN_Rank_Conscript.tga

ReplaceableTextures\CommandButtonsDisabled\DISPASBTN_Rank_Corporal.tga

Using an Item for kill count:

The other option is to use an item and tally charges on it. Here you need one tooltip and can tally more kills (up to 1000). First, create a new item and choose an icon to represent your kill count.

I personally like using the "Ankh of Reincarnation" icon because I'm going for a cyberpunk feel in my mod. In this item's table of defined values, set everything that can be set to False to False. For example, set "Stats - Can Be Dropped" to false. We don't want the user dropping or using the item in any sort of way.​
From here on, we're going to be placing (through triggers) this item into each unit's inventory in slot 1 as they're trained, so the next step is to go through the object editor and give any units you want to be able to level up an inventory- in "Abilities - Normal" give them "Inventory (Hero)" for example.

Displaying Rank GUI

The next problem is showing the rank! Here we could effectively use an ability or an item. It really doesn't matter thanks to the spellbook ability which can condense space tremendously. For the sake of this tutorial, I'll use an item. I also went one step further and developed a model to be used as an attachment.

Using an Item for rank:

Now, create another item and choose an icon for it. For me, I used the "PASBTN_Rank_Conscript.tga" to represent the lack of rank at the start.

I developed my own TGAs using Gimp based on the entire military E series and O series. You can use whatever images you want really. For example, the Scrin faction in C&C 3 have an entirely different icon for each level than GDI. Once again, set this item's values to false.​

This item (as far as this tutorial goes) will rest solely in slot 2. Next, create a trigger that gives units these items as they're created. It should go something like this...

Events
Unit - A unit Finishes training a unit

Conditions
(Item carried by (Triggering unit) in slot 1) Equal to No item

(Item carried by (Triggering unit) in slot 2) Equal to No item

(Level of Inventory (Hero) for (Triggering unit)) Greater than 0

Actions
Set tempUnit = (Triggering unit)

Hero - Create [Item For Kill Count - The Ankh] and give it to tempUnit

Hero - Create [Item Showing Initial Rank - Conscript] and give it to tempUnit

Upon training a unit, assuming it has an inventory, that unit will receive two items that will now tally everything as far as rank goes. You can see how it looks in my first attachment named "Oink."

Creating the Next Rank

Now, before we proceed, we'll need to create one more item. Once more, turn all the stats for this item to false. Call this item "Corporal." For the sake of this tutorial, set its "Art - Icon - Game Interface" to the imported "PASBTN_Corporal.tga."

I included the model attachment just in case you wished to include that in your rank GUI. To attach it, you'll need to create an ability... call it "Rank Corporal" ... then in "Art - Target" use "Corporal.mdx" and in "Art - Attachment Point 1" enter in "overhead." Then return to your Corporal item and under its "Abilities - Abilities" give it the new ability "Rank Corporal" that you just created.​

Play Warcraft, Rank Up, Get Pig!

Handling the kills and ranking up is straightforward from here.

The trigger looks like this...

Events
Unit - A unit Dies

Conditions
(Level of Inventory (Hero) for (Killing unit)) Greater than 0

Actions
Item - Set charges remaining in (Item carried by (Killing unit) in slot 1) to ((Charges remaining in (Item carried by (Killing unit) in slot 1)) + 1) //Remember slot 1 is our Kill Count item

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Charges remaining in (Item carried by (Killing unit) in slot 1)) Equal to Rank_Kills[0] //An array that has a predefined number of kills for the first rank.

Then - Actions
Set Random_Needed = (Charges remaining in (Item carried by (Killing unit) in slot 2)) //A random integer that temporarily stores the number of perk point (charges) before the item is removed

Item - Remove (Item carried by (Killing unit) in slot 2)

Hero - Create (First Rank Item) and give it to (Killing unit)

Item - Set charges remaining in (Item carried by (Killing unit) in slot 2) to (Random_Needed + 2) //This tallies the points the unit has to spend on perks like increased HP

Else - Actions
Do nothing

In this tutorial, our "First Rank Item" is the "Corporal" item we just created. Of course, the first rank in the E series is private... my picture showed corporal, but I digress.

Perk Points

Unit Perks are handled in an ability based on spellbook for me. This is relatively straightforward to set up from here. Just choose which abilities you want and put them into your spellbook ability.

I simply re-purposed the "manual of health," "gold coins" and the item which permanently increases attack whose name I forgot...​

The slightly tricky part for these perks is the triggering which I handled like below. Please note, that I created three separate ability objects based on the "Channel" ability. It is strongly recommended by many on these forums to use "Channel" whenever creating a custom ability which is what's going on here.

Events
Unit - A unit Begins channeling an ability

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Upgrade Health (rank points) //Upgrade Health is the name I gave one of my perk abilities

Then - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Charges remaining in (Item carried by (Triggering unit) in slot 2)) Greater than 0

Then - Actions
Item - Set charges remaining in (Item carried by (Triggering unit) in slot 2) to ((Charges remaining in (Item carried by (Triggering unit) in slot 2)) - 1)

Hero - Create (Health Item) and give it to (Triggering unit) //In this case, my Health Item was simply "Manual of Health" which gives +50 HP bonus permanently

Else - Actions
Game - Display to (All players matching ((Owner of (Casting unit)) Equal to (Matching player))) the text: This unit has no points to spend //No Free Perks!

Unit - Order (Triggering unit) to Stop //Would look silly if the unit played an animation whenever it wanted to

Else - Actions
Do Nothing

The Downside of This System

Some of you may be thinking... well what if I want to carry more items? This system will be taking away two of my six slots! This is true. The fortunate thing here is that there is a way to carry nearly unlimited items with JASS or even use the spellbook ability to "pseudo-carry" items saved as abilities, but alas that is another tutorial found elsewhere.

The Ups

-We can track kill counts with little stress on the system.

-We have two sources of visible rank GUI

-Our units can acquire perks

-Each trigger is generic (meaning it will target any unit regardless of race or color)

-We can easily limit or expand upon the functionality of this system

Well... this sums up my first tutorial. Hope it was helpful to someone! +1 to rep if you liked.

#### Attachments

Oink.jpg

 770.8 KB

 · Views: 589



RankUp!.jpg

 743.6 KB

 · Views: 580



Points!.jpg

 736 KB

 · Views: 541



Rank_Color.blp

 2.4 KB

 · Views: 199



Rank_Corporal.mdx

 5.1 KB

 · Views: 186



DISPASBTN_Rank_Corporal.tga

 5.5 KB

 · Views: 184



PASBTN_Rank_Corporal.tga

 5.5 KB

 · Views: 182



DISPASBTN_Rank_Conscript.tga

 364 bytes

 · Views: 196



PASBTN_Rank_Conscript.tga

 364 bytes

 · Views: 183




 Last edited by a moderator: Jun 11, 2015

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

If this is a tutorial it should be in the tutorial forum not the triggers and scripts forum.

Also never use casting unit / trained unit use triggering unit.

Anything used twice or more should be stored into a variable and the variable should be used.

If this is a tutorial it should be structured a little better.

#### Strikest

#### Strikest

 Level 10

JoinedAug 27, 2012Messages363

Resources: 5

#### Resources
Icons 1Maps 1Spells and Systems 2Ported Models 1

Also you should upload the icon you used in case people just want to duplicate what you showed in the tutorial exactly.

#### GenFrogKing

#### GenFrogKing

 Level 3

JoinedNov 25, 2014Messages24

Resources: 1

#### Resources
Tutorials 1

Oh, ok. I'll make the corrections. Sorry about posting in the wrong forum. Is there anyway to transfer it?

@deathismyfriend: The problem with using "triggering unit" when I start a trigger with "a unit finishes training a unit" is that there are two units that could be referenced- a "training unit" and a "trained unit." I guess that's what you're referring to when you say "Anything used twice or more should be stored into a variable and the variable should be used." How would I go about doing that, though?


 Last edited: Nov 29, 2014

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

For instance you use training unit twice here.

Hero - Create [Item For Kill Count] and give it to (Trained unit)

Hero - Create [Item Showing Initial Rank] and give it to (Trained unit)
You should do this.

Set tempUnit = (Trained unit)

Hero - Create [Item For Kill Count] and give it to tempUnit

Hero - Create [Item Showing Initial Rank] and give it to tempUnit
Also when referring to a unit finishes training triggering unit only refers to trained unit. It will never refer to the training unit. That is why i said never use trained unit use triggering unit instead. You also changed the original trigger that I posted that for so I can't show you the correct way to do it.

Never use casting unit use triggering unit.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I sense a great lack of variables. Use more variables to increase the readability.

The system seems to lack in functionality as well. Using items to display the values is kinda bad since it takes away 2 out of 6 possible item slots. You could use floating texts and display the kills/rank for the selected unit(s) only for example.

#### GenFrogKing

#### GenFrogKing

 Level 3

JoinedNov 25, 2014Messages24

Resources: 1

#### Resources
Tutorials 1

Nice pun, but what do you mean? How would more variables increase readability? I left this particular system's tutorial low on finality, so people can change this system how they want to.

I assure you, my system works plenty well. It happens that this system suits the needs of my mod, so I thought I'd share it in case anyone else prefers it. It has tradeoffs just like any other rank system and requires some tweaks to perfect, but compared to a lot of the other system's I've seen, this one gives me the most bang for the small price of two item slots.

If someone needs all six slots (or more), there are workarounds with JASS or using the spellbook as an ability combined with triggers. I don't need all six, so I'm golden there.

Floating texts are far too difficult, in my opinion- especially when you start dealing with every unit on the board- plus the text would clutter the screen if there were enough units!

#### Pinzu

#### Pinzu

 Level 15

JoinedNov 30, 2007Messages1,202

Resources: 7

#### Resources
Spells and Systems 5JASS 1Tutorials 1

Why not attach test map? ^^

Killing unit and Triggering unit should also be set to tempUnit.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice tutorial overall. Minor trigger optimizations aside, the tutorial is approvable. Although there is one questionable statement:

 JASS can really slow things down when we start to run multiple instances of a particular code.


Click to expand...

JASS is the innards of GUI. It won't slow things down any more than GUI would. Since you haven't been active in a while, I'll remove that statement and move the tutorial to its category.

I've seen people ask questions regarding these types of rank systems, and since I like this implementation, consider this tutorial approved.

 You must log in or register to reply here.


### Similar threads

 How to create an artificial level-up system


frettory

Nov 22, 2012

Trigger (GUI) Editor Tutorials

Replies14Views9K

Mar 13, 2013
frettory

 Stacking Items


DigitalDemon

Dec 21, 2007

Trigger (GUI) Editor Tutorials

Replies18Views27K

Sep 18, 2025
Luashine

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