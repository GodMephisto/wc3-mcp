---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-create-spells-in-gui-the-beginning.201867/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Create Spells in GUI the Beginning
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-create-spells-in-gui-the-beginning.201867/>
#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

 Editor's note.


 I'm getting tired of viewing spells in the spell section lately due to most of them are just bunch of crap (yes you heard me).


They're not MUI and most of them break one or more rule besides the MUI rule. So this will be a selection of tutorials,


same concept as palaslayer had (or was it kwah? dont remember) - From zero to hero -.


~baassee

 Requirements


Decent GUI knowledge - Know about variables and manage to "create a trigger".

A world editor, either the standard WE or JNGP. WE Unlimited is banished from this tutorial, don't even mention it.

Time.

 Information about the tutorial.



This tutorial do not take leaks into consideration. Nor does it have a test map, I just say what the user should look at and which word/s it should search for in the search engine. Also this is not a complete tutorial how to make spells, this is just the very basics and a few helpers.


The next tutorial will probably tell all about leaks and it's importance to clear them but for now the user should be focused on how to proper code the basics as they say, you cannot build a house without a proper foundation right?


Also the use of BJs might be low but not explained why proper, will come in the next tutorial! Do not worry.

 Within the tutorial


Step 1 - Events Part 1

Step 1 - Events Part 2

Step 2 - Conditions

Step 3 - Actions


In this tutorial we are using GUI. In Warcraft III the coding language is call JASS (Just Another Scripting Syntax) and Warcraft III itself is coded in a language called C. GUI stands for Graphic User Interface and is there to make it more easier for beginners to create their own code. Basic GUI is JASS but with pictures and clickable objects. Of course in detail there are a lot more differences but in the end as an old friend said: "What you can do in JASS, I can do in GUI." - Paladon.

### Step 1 - Events Part 1

 - Back to the top.


The first thing you will need in your spell is an event. The spell related events are all in the Unit - Generic Unit Event. There are several of them so which one are you supposed to use? A few of them are quite... tricky.

Hint: Yeah I know that the events are in the Unit - Specific Unit Event as well. We're not supposed to use them, only in very rare occasions which people at this level do not use.


I'll list them here for you.

Unit - A unit Begins channeling an ability

Unit - A unit Begins casting an ability

Unit - A unit Stops casting an ability

Unit - A unit Starts the effect of an ability

Unit - A unit Finishes casting an ability
There's five of them and you can split them into two different sub-groups:

Start of a spell & End of a spell.


So the Start of a spell sub-group will contain:

Unit - A unit Begins channeling an ability

Unit - A unit Begins casting an ability

Unit - A unit Starts the effect of an ability
And the End of a spell sub-group:

Unit - A unit Stops casting an ability

Unit - A unit Finishes casting an ability
In the Start of a spell sub-group, the events are used - as written - when a spell are supposed to start as known as catch when ever a spell is casted. They run in different orders so when a spell is casted, the events will be ran in the order I've sorted them.

Unit - A unit Begins channeling an ability

Unit - A unit Begins casting an ability

Unit - A unit Starts the effect of an ability
So why did Blizzard create 3 different events to detect when a spell is casted? The simple answer is that they are ran at different levels as

Unit - A unit Begins channeling an ability
is ran when an ability is fired. This means that neither animation nor manacost & cooldown are fired.

Unit - A unit Begins casting an ability
This event is ran when the animation is fired. Still the manacost & cooldown are not fired. The thing with this event is that you can fire stop right after and fire a spell and animation but not remove cooldown nor manacost which will equal to a flaw that you can abuse.

Unit - A unit Starts the effect of an ability
This event is the event that is most commonly used by users that learned a bit about spellmaking. With this event animation, manacost & cooldown are all triggered which is perfect for a spell.

Hint: It's a typical mistake for beginners as they might use

Unit - A unit Begins casting an ability
 instead of

Unit - A unit Starts the effect of an ability
 because they just read and finds the "Begins" more proper to use. That is incorrect. Most users want there spell to fire off cooldown and manacost and therefore they should use the second one mentioned.


And the last two events of course! With explanation.

Unit - A unit Stops casting an ability
This one should be easy to understand. Does what it's supposed to do, it works when a unit is channeling an ability and when it stops with the stop ability or being stunned (or any other pausing spell), this event fires. However this event will not fire if the unit will move then another event will be fired (issue point order). So basic this event is not as reliable to use and I think it will not even fire if you abuse the flaw on the Begins casting an ability event (not tested though).

Unit - A unit Finishes casting an ability
When a unit finishes an ability, what else? Should only be used on channeling spells as they are over time and I think instant spells (missile, stomp whatever) will not trigger this event and even if it does you should use the effect event instead.

Hint: With this event you will not be able to retrieve information about the target unit of ability being cast. Special thanks to Pharaoh_ for pointing this out.

### Step 1 - Events Part 2

 - Back to the top.


So if there 3 events that can register when a spell is fired why isn't there only one? Because there are plenty of uses and situations where you don't want manacost and cooldown to trigger. A common example would be that you want a spell to require hp to cast and then you can use the "begins casting" event as it only plays animation and if the current hp is not correct then you can just order the casting unit to stop and therefore will not trigger the next event (the effect).


Or if you want a spell to fire instantly and probably pausing the unit/removing it from the players selection (and therefore it will automatically fire the effect event) you can use the channel event. It can also be used with the example above although animations are nice (first spell and then stand, looks a bit more realistic).


The effect event is of course for most of all spells that you will trigger. Manacost and cooldown are quite relevant things in a map.


Can you combine the events? YES you can! In different triggers of course though. Easiest example is that you wanna stop a unit before as one of the examples above. Let's say this time you create two triggers, trigger 1 and trigger 2.


Trigger 1 has the "begins casting an ability event" and Trigger 2 has the "starts an effect of an ability". Trigger 2 will add a buff to the caster and while the caster has this buff, it will be stopped when trying to fire off the spell again within the interval. Trigger 1 is there to make the magic happen. It checks if the triggering unit has the buff and if it has, then it orders the unit to stop else it does nothing. Piece of cake eh?

### Step 2 - Conditions

 - Back to the top.


This step is rather short as it's so little to do. The whole step is about this rather short line:

(Ability being cast) Equal to YourSpell
Wooow that's easy is it not? You just search for ability comparison and voila change Animate Dead (it's default if not deleted) to your spell and you're good to go!

Hint:In JASS you compare the unique object id with an integer comparison but that's a bit much now.


You really thought this step was this short? Well it's not, now what can we do as well just to give you some more inspiration.


In our example with the stop event we had two triggers right? One was checking a buff right? How would have those triggers looked?

Trigger 1
Events
Unit - A unit Begins casting an ability

Conditions
((Triggering unit) has buff YourBuff) Equal to True

Actions
Unit - Order (Triggering unit) to Stop

Trigger 2
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to YourSpell

Actions


Wooahh now a few more experienced users might think, why in hell don't you check the ability being cast in the first trigger? Well it's just a triggering tip but if there's only one spell that can give this buff then why should we check two booleans instead of one? Lesser code that does the same thing is always better. Beware though that if there are more possibilities to recieve this buff my way will not work (will be unreliable). Either way in trigger 1 we use a "boolean comparison", search for "Unit has specific buff" and change the buff to your custom buff. In the actions you search for in this example Unit - Issue order with no target and change unit into triggering unit. So what we have done is that if the unit has the buff, we stop the unit and it will not proceed to the next step! Great huh?

### Step 3 - Actions

 - Back to the top.


No more no less the most important(?) part - The actions. This part will not tell you everything, will tell you the most basic stuff and not tell about memory leaks (as mentioned in the info).


I'll keep this section rather short and point out the most relevant for users in the beginning.


One thing most users want their spells to do is damage. Alright so there are two different damage functions.

Unit - Damage Area


Unit - Damage Target


These will show up.

Unit - Cause (Triggering unit) to damage circular area after 0.00 seconds of radius 500.00 at (Center of (Playable map area)), dealing 100.00 damage of attack type Spells and damage type Normal

Unit - Cause (Triggering unit) to damage (Triggering unit), dealing 500.00 damage of attack type Spells and damage type Normal
Alright so there's quite much different between them although they do deal damage. The damage area one as it says, it damages an area. It requires a few more parameters as well so let's look into it more and compare them.

Unit - Cause (Triggering unit) to damage circular area after 0.00 seconds of radius 500.00 at (Center of (Playable map area)), dealing 100.00 damage of attack type Spells and damage type Normal
Triggering unit -> The casting unit - A Unit that will be the damage dealer.


0.00 -> real amount - The time until it should damage, 0 = instant.


500.00 -> real amount - the damage radius, the area that the damage should be dealt.


Center of (region) - point variable, the point where the action should start count its radius from


playable map area - region, yeah the point will be a point from a region (mostly)


100.00 - real amount, the damage that should be damaged duuh


spells - attack type, 'causes it to deal different damage to different armors


normal - damage type, if it's magic it will deal magic damage along with spells attack type, universal will be pure damage and the rest will not affect the damage

Hint:The Damage area will damage allies, neutrals and enemies within the area so if the hero is standing within the area, it will take damage.

Unit - Cause (Triggering unit) to damage (Triggering unit), dealing 500.00 damage of attack type Spells and damage type Normal
Triggering Unit (1st) - Same as above


Triggering Unit (2nd) - the target of the spell that should recieve the damage


500.00 - same as above


spells - same as above


normal - same as above

Hint:You can choose both damage dealer and target so you can actually make the enemy damage the casting hero as well as damaging the enemy. You can also set the damage dealer and the damage reciever to the same unit and basically it will damage itself.

Hint:This applies to both damage functions. With the use of a negative value, the units will be healed instead of damaged. Although I recommend not using this way and using the one below instead.


I hope you now see the differences between them.


What else might you wanna do in your first spell? Maybe you wanna heal something? Yeah and maybe restore mana as well? YEAH! Alrighty then...


So there are 4 different things that can affect your hp to heal (and as well the damage actions as said above)

Unit - Set life of (Triggering unit) to 100.00%

Unit - Set mana of (Triggering unit) to 100.00%

Unit - Set life of (Triggering unit) to (Life of (Triggering unit))

Unit - Set mana of (Triggering unit) to 0.00
So the first two are percentage stuff and the other two are values. The second and fourth ones are for mana and the others for health. So basic let's make it easy, we want to heal a unit 100 hp right? Then we should not use the percentage even though it's possible with some calculations, the value action is as made for this. And not only that, we want to reduce the mana even more with a 50.


They both have the parameters unit and real. But the thing is that we cannot basic just set the value into 100 because then the hp will be 100 and not "current health + 100". So we look into the real value. It's already set to the current health of the triggering unit BUT we need to do a little bit more. When you double click it, you will see that the function is called "Unit - Property". Now go to the top of that list and find "Arithmetic" which will make it possible to add/reduce/multiply/divide things.


Now it will display itself as 1.00 + 1.00 right? Now press the first one and find the Unit - Property again. And in the second one we change the value to 100 so it will look like this in the end:

Unit - Set life of (Triggering unit) to ((Life of (Triggering unit)) + 100.00)
Now if you didn't notice you could within the Unit - Property function change life. You could change it to Life/Mana/Max Life/Max Mana. So how to make the mana go away? Well just pick the second function (Set mana to value) and do the same as you did here with Unit - Property but change it to mana and change the + into a - and you're good to go.

Unit - Set mana of (Triggering unit) to ((Mana of (Triggering unit)) - 50.00)
Hint:The health reduce can be used to damage a unit as well as healing a unit. The thing is about doing health reduction with this is that when a unit's hp reaches below 0.405 it will instantly be 0 and thus die. BUT you will not recieve any bounty for killing the unit nor any score in the ending score.


I think this will be enough for the action part, there's so much you can do in the action part that will not fit in right here, as said, it's a beginners guide!


Just for last I will say that there are a two more things that are quite useful.


First is comments. Comments are useless textlines that you will only see within the trigger editor and nowhere else (aka not in-game). They are useful as they can explain more within the trigger and as well make it more readable.


Compare this:

Trigger 1
Events
Unit - A unit Begins casting an ability

Conditions
((Triggering unit) has buff YourBuff) Equal to True

Actions
Unit - Order (Triggering unit) to Stop

With this:

Trigger 1
Events
Unit - A unit Begins casting an ability

Conditions
((Triggering unit) has buff Acid Bomb) Equal to True

Actions
-------- ------------------------------------------- --------

-------- This is a stop order --------

-------- It will cause the unit to stop --------

-------- ----------------------------------- --------

Unit - Order (Triggering unit) to Stop

It's lame I know but it will look a lot better with more actions to come!


The last thing is custom script, which are JASS basically in GUI format instead of converting the trigger. Will come up in other tutorials.


Hope you enjoyed this tutorial. More will come. From zero to hero and beyond!


~baassee


Changelog


v1.0 2011-08-18 - Released


v1.0b 2011-08-19 - Fixed a few things mentioned by Pharaoh_, also added a bit more information to a few parts and redid a few sentences. A few hints as been changed as well (more info).


 Last edited by a moderator: Oct 22, 2011

#### Skycraft

#### Skycraft

 Level 17

JoinedNov 11, 2010Messages1,974

There is already a tutorial for spells in GUI.

https://www.hiveworkshop.com/forums/trigger-gui-editor-tutorials-279/basics-trigger-enhancing-spells-7334/


Not to mention there is another tutorial already made on MUI.

https://www.hiveworkshop.com/forums/trigger-gui-editor-tutorials-279/hashtables-mui-133407/


And they both have buttloads of information stacked into them :3

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

It's pretty old you know? Has a few flaws as well and Daelin is not active anymore.

I never said that exactly this tutorial is the one that is going to make it MUI, besides wyrm's tutorial is not for the beginners. It's a bunch of tutorials and I'll try to keep them as short as possible.

#### Pharaoh_

#### Pharaoh_

 Level 40

JoinedNov 6, 2008Messages8,316

Resources: 13

#### Resources
Icons 4Spells and Systems 7User Interface 1Tutorials 1

Looks cool so far, we all appreciate tutorials that want to raise the standards of the Spells section.

However,

* I wouldn't use the word "bug" into an event that doesn't actually bug, but more of works this way. Newbies do not comprehend why you call it a bug and they recycle this label, when they don't actually know why. "Begins casting an ability" timing lies between "Issues an order" and "Starts the effect of an ability".

* If I recall, -you might also want to mention-, that "Finishes casting an ability" can't retrieve the (Target unit of ability being cast). This event can also be used if we want to remove the casting unit from the game. If we use Starts the effect of an ability and kill it, I think it will crash the game (too bad I can't test it at the moment -forgive me if I'm wrong-).

* The "0.405" life part seems to be quite of a myth in GUI; it keeps the unit alive at minimum 0.41. I am assuming it has to do with SetUnitState(), which is less accurate than the SetWidgetLife() [speed-wise, most likely].

* Some terminology issues; I noticed you use "trigger" (verb), to explain an initiation of a process (e.g. this event triggers cooldown). Since everything stated in this tutorial is implemented withing a Trigger, it would be better if you used some synonyms, like "fire"/"activate"/"generate" (depending on its use), because I initially had a hard time figuring out the way it was used.

GUI/Jass: You might as well explain the difference of these terms, since the tutorial is addressed to new spellmakers.

* The hints are too good to be left with that font size. They are so important, that they could form a new tutorial! :]

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

Why did my fix reply get deleted? Lawl

Currently writing the second tutorial.

#### noob

#### noob

 Level 14

JoinedSep 28, 2011Messages968

Resources: 5

#### Resources
Maps 4Tutorials 1

For a unit finish casting an ability it is when the ability finish so if it is a shockwave it is when the shockwave arrive at destination and finish this is genereraly when the spell stop having effects so if you want to give an area of effect to fireball just make a damage on circle when A unit Finishes casting an ability on the target

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

noob said:

 For a unit finish casting an ability it is when the ability finish so if it is a shockwave it is when the shockwave arrive at destination and finish this is genereraly when the spell stop having effects so if you want to give an area of effect to fireball just make a damage on circle when A unit Finishes casting an ability on the target


Click to expand...


Finish is fired when the unit finishes their animation or when the unit finishes casting a spell (if the spell has a cast time). It isn't necessarily when the shockwave is finished, it is just a coincidence that the unit finishes their animation by the time the shockwave goes away.


Anyway, I don't see anything wrong with this tutorial, I'm going to go ahead and approve it since I think GUI users could benefit off this. (even if there are alternative tutorials)

 You must log in or register to reply here.


### Similar threads

 How to trigger Auto-cast (Arrow) Abilities


Stonebludgeon

Apr 26, 2010

Trigger (GUI) Editor Tutorials

Replies12Views13K

May 6, 2010
Stonebludgeon

Sticky

 How to get your GUI spell approved


Maker

Nov 24, 2011

Trigger (GUI) Editor Tutorials

Replies12Views18K

Jan 10, 2019
Daffa

 Making any Spell Autocasting


Ralle

Mar 15, 2006

Trigger (GUI) Editor Tutorials

Replies12Views59K

Dec 22, 2017
Rogue Kessler

 Creating some basic abilities


paskovich

Apr 22, 2007

Trigger (GUI) Editor Tutorials
23

Replies113Views104K

May 26, 2021
McPhisto2051

 How to begin in triggering, for the complete noob


hawk_767

Jan 24, 2009

Trigger (GUI) Editor Tutorials

Replies23Views32K

Dec 28, 2014
Madcapercaillie

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