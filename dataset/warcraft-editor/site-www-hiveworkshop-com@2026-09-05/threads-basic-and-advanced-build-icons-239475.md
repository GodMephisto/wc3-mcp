---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/basic-and-advanced-build-icons.239475/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Basic and Advanced Build Icons
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/basic-and-advanced-build-icons.239475/>
#### aple

#### aple

 Level 12

JoinedMay 20, 2009Messages821

Resources: 5

#### Resources
Maps 2Spells and Systems 2Tutorials 1

 A small group of developers, known as ZanManagement, may have or may have not been the first to discover this. Either way, I found this out by looking through their mod WarCraft Vs StarCraft.

Here is a map template if you need additional help: Map <- Updated

First, we need to know of a few files in the War3.MPQ archive.

CommandStrings.txt, HumanAbilityFunc.txt, UndeadAbilityFunc.txt, OrcAbilityFunc.txt, and NightElfAbilityFunc.txt.

Extract these using an MPQ program, I suggest Ladik's MPQ Editor. You can find these files in the Units\ directory.

These files contain the first part of the necessary data to do this. Once you're done editing these, you can simply import them into any map you want.

Now, as we all know in the normal WC3 Map Editor, (And even in some modded editors,) you cannot edit the Build Ability.

These files, however, contain the necessary data needed to modify to do so!

I'm going to use Humans as an example:

This Ability is in HumanAbilityFunc.txt

[AHbu]

Art=ReplaceableTextures\CommandButtons\BTNBlank.blp

Buttonpos=2,1

Order=humanbuild

​

 Also, be sure to change the button positions of other abilities, such as Call to Arms or Unsummon Building. They will be in the way of the Advanced Build Button.

The way I have it set up in the Map Template is how I suggest you should set it up, the abilities being shifted over one. (Detonate is at 2,2 , Unsummon is at 3,2 , Sacrafice is at 2,2 , Etc)

[Just as a little flavor note, the A stands for Ability, H stands for Human, and BU stands for Build.]

Please note the Button Position, and the Button Art itself. These are important.

The Button Position needs to be in a position on the worker for the exact race, meaning if you make another Worker for the Human Race it will use this same button position for it's Build Ability.

The Button being blank is so it isn't visible.

This is in CommandStrings.txt

[CmdBuildHuman]

Tip=

Ubertip=

Hotkey=Z

​

 You can change the Hotkey to whatever you want. I suggest a far-off Hotkey that you will never use for any of your Hotkeys. When you decide on your Hotkey, MAKE SURE YOU MAKE IT CAPITAL! I''ve made that mistake before and it drove me insane.

All the other ones are, [CmdBuildOrc], [CmdBuildUndead], and [CmdBuildNightElf].

Notice that Tip and Ubertip are not blank, they have a single space after them, this is so when the icon is hovered over it doesn't show any text.

Now just import these into your map and you're done with the Hard/Easy part (Whichever you think)

The next thing you need to do is make a copy of the Mana Shield and Wind Walk abilities, since these are the easiest abilities to modify that do not target a unit, and have an Order String.

Remove all the buffs, effects, set all the data values to 0, (Such as Damage absorbed for Mana Shield, Speed Increase for Wind Walk, etc) and that should be good to go. Alternatively, a Channel ability with the order you are using in question should also be sufficient.

Now, we move on to the Triggers.

Human Build Advanced Build
Events
Unit - A unit Is issued an order with no target

Conditions
Or - Any (Conditions) are true
Conditions
(Unit-type of (Triggering unit)) Equal to Villager

-------- Checks for units issued an order with no target, than to make sure it's your worker -------

Actions
-------- IF WORKER PRESSED "BUILD" OR "ADVANCED BUILD" --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Issued order) Equal to (Order(manashieldon))

(Issued order) Equal to (Order(manashieldoff))

-------- This checks for Advanced Build --------
Then - Actions
-------- Upgraded Construction --------

Player - Make Weaponry Encampment Available for training/construction by (Owner of (Ordered unit))

Player - Make Villa Available for training/construction by (Owner of (Ordered unit))

Player - Make Keep Available for training/construction by (Owner of (Ordered unit))

Player - Make Settlement Unavailable for training/construction by (Owner of (Ordered unit))

Player - Make Farm Unavailable for training/construction by (Owner of (Ordered unit))

Player - Make Draft Camp Unavailable for training/construction by (Owner of (Ordered unit))

Player - Make Altar of Lost Souls Unavailable for training/construction by (Owner of (Ordered unit))

Player - Make Long House Unavailable for training/construction by (Owner of (Ordered unit))

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Issued order) Equal to (Order(windwalk))

-------- This checks for Basic Build --------
Then - Actions
-------- Normal Build --------

Player - Make Weaponry Encampment Unavailable for training/construction by (Owner of (Ordered unit))

Player - Make Keep Unavailable for training/construction by (Owner of (Ordered unit))

Player - Make Villa Unavailable for training/construction by (Owner of (Ordered unit))

Player - Make Settlement Available for training/construction by (Owner of (Ordered unit))

Player - Make Farm Available for training/construction by (Owner of (Ordered unit))

Player - Make Draft Camp Available for training/construction by (Owner of (Ordered unit))

Player - Make Altar of Lost Souls Available for training/construction by (Owner of (Ordered unit))

Player - Make Long House Available for training/construction by (Owner of (Ordered unit))

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions

 If you did this right, than what will happen is:

You click or press the Hotkey for the Basic Build icon, and it will disable certain buildings and enable others, than force the Hotkey for the REAL build icon to be pressed.

This is my first tutorial, so go easy. Also, if you already know this than please don't be too mean. ^^

I will let you know now, that this MAY or MAY NOT cause lag in Multiplayer, I don't know for sure since I never tested.

There are a few side effects to doing this.

-The Original Build icon is still clickable, and will show you the buildings that are currently enabled.

-You get two annoying click sounds instead of one.

-Acolytes won't be able to sacrifice properly.

-Build commands will be reset if Advanced Build is clicked before

But other than that, it is FULLY functional.

EDIT:

Also in my example here, I show how this can be used to make a unique mechanic. The way I was setting this up was there are several buildings that can be upgraded into an improved counter-part.

However, if you already have the improved counterpart on the field you can build it straight-up without having to build the original building.

This made this race slow to tech at the start, but able to keep up with production later on. They are meant to be a more macro-oriented race, instead of micro. Lots of cheap units, with lower stats, most having abilities that spawn more units.

If you have any suggestions on how to make it simpler for beginners to read, please let me know in this Thread or PM if you want.

Credits go to the ZanManagement Team, which is who I learned this off of. Go check out their website or other projects. =p

For an alternative implementation, you may want to check this out: Basic and Advanced Build Icons


 Last edited: Oct 22, 2017

#### TaShadan

#### TaShadan

 Level 13

JoinedJul 2, 2008Messages1,179

Resources: 1

#### Resources
Maps 1

Could you add an example map?

#### DivineLight

#### DivineLight

 Level 13

JoinedJan 30, 2012Messages1,297

Resources: 5

#### Resources
Icons 1Maps 2Spells and Systems 2

 Ladik's MPQ Editor


Click to expand...

you should link your suggestion so people know where they can get it

#### aple

#### aple

 Level 12

JoinedMay 20, 2009Messages821

Resources: 5

#### Resources
Maps 2Spells and Systems 2Tutorials 1

TaShadan said:

 Could you add an example map?


Click to expand...


Will do, I will work on it today.

DivineLight said:

you should link your suggestion so people know where they can get it

Click to expand...


Right, I should have guessed that...I'll link that in a moment.

#### TaShadan

#### TaShadan

 Level 13

JoinedJul 2, 2008Messages1,179

Resources: 1

#### Resources
Maps 1

thx! Iam looking forward to it.

#### aple

#### aple

 Level 12

JoinedMay 20, 2009Messages821

Resources: 5

#### Resources
Maps 2Spells and Systems 2Tutorials 1

I just realized that I've done some of this info wrong, so once I finish the tutorial map I'm going to do some overhauling to this Tutorial.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Just use channel as base spell rather than mana shield or windwalk...

#### aple

#### aple

 Level 12

JoinedMay 20, 2009Messages821

Resources: 5

#### Resources
Maps 2Spells and Systems 2Tutorials 1

Adiktuz said:

 Just use channel as base spell rather than mana shield or windwalk...


Click to expand...

You need two separate Order Strings for this to work. Mana Shield and Wind Walk are the easiest candidates for that.

Edit: Order Strings that don't have a target. Maybe they're aren't the easiest. I'm just using them because that's what ZanManagement used. Why fix what ain't broke?


 Last edited: Aug 15, 2013

#### aple

#### aple

 Level 12

JoinedMay 20, 2009Messages821

Resources: 5

#### Resources
Maps 2Spells and Systems 2Tutorials 1

The Map Template has been added, and I added a bit more info.

(Also tried to make it look better)

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Simply because channel is a spell that does nothing on it's own... so it's better to use it for dummy things...

#### TaShadan

#### TaShadan

 Level 13

JoinedJul 2, 2008Messages1,179

Resources: 1

#### Resources
Maps 1

There is a bug which i also had when i tried to create an advanced building system with making buildings available/unavailable.

If you build several normal buildings with the shift key and afterwards select a second worker and build an advanced building all normal building orders will be canceled.

Maybe you could use replace unit (having a second worker with other building set) instead ?

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Another idea that does not kill the localization:

You can integrate abilities into the build menu by creating a spellbook with the same order ("humanbuild" for human building), giving this ability to the unit and afterwards deactivating the spellbook ability for the player. If you disable the shared cooldown flag of the spellbook, it won't close when casting an ability from it. Then proceed with the order trigger like above. Disable the initially unwanted structures at map init.

@Adiktuz: Windwalk, Mana Shield and the like are better for this task because they are immediately and parallel castable. Channel does not provide that option.

I also want to remark that giving the build menu some black button does not hide it. If the button was hidden, the hotkey would not work either.

#### TaShadan

#### TaShadan

 Level 13

JoinedJul 2, 2008Messages1,179

Resources: 1

#### Resources
Maps 1

Will there be a contruction animation with the spellbook solution?

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

? It's the normal build method and you select the structures like above with the "Player - Set Training/Construction Availability Of Unit"-action. Just in this case, you enter the build menu by itself and there is a switch button inside. It's also safer than the Key Press emulation above because the unit might not even be selected/have another unit as primary selection/have another ability with the same hotkey (you also have to account for a user's custom keys), so this might trigger something else.

#### TaShadan

#### TaShadan

 Level 13

JoinedJul 2, 2008Messages1,179

Resources: 1

#### Resources
Maps 1

As far as i understand you will still have the problem i discribed in the last post. Could you provide a demo map?

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

blub

edit: Yes, your described problem appears. Maybe this can be solved via just in time set-changing because once the structure is there, it's good to go. For example catch when a unit comes within building range.

#### Attachments

buildSwitch.w3x

 17.8 KB

 · Views: 250



#### TaShadan

#### TaShadan

 Level 13

JoinedJul 2, 2008Messages1,179

Resources: 1

#### Resources
Maps 1

I really hope someone could solve that problem

BTW an other ´way would be to replace the worker with a second worker and give him different available building. I tried that and it worked so far, but i had a problem. If the player presses ESC to abort the build order

its still the "second" worker with advanced buildings. But when i tried it i didnt know about the possibility to change the position and hotkey for the build ability. I will try it that way again.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Replacing the unit is what we try to avoid.

#### TaShadan

#### TaShadan

 Level 13

JoinedJul 2, 2008Messages1,179

Resources: 1

#### Resources
Maps 1

I created a second peasant and triggered this. Now it is possible to build with several workers without breaking the order, but now the problem is that the worker using the ability stops every action ordered before.

Human Build Icons
Events
Unit - A unit Is issued an order with no target

Conditions
Or - Any (Conditions) are true
Conditions
(Unit-type of (Triggering unit)) Equal to Peasant

(Unit-type of (Triggering unit)) Equal to Peasant

Actions
-------- IF WORKER PRESSED "BUILD" OR "ADVANCED BUILD" --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Issued order) Equal to (Order(manashieldon))

(Issued order) Equal to (Order(manashieldoff))

Then - Actions
-------- Advanced Construction --------

Unit - Replace (Triggering unit) with a Peasant using The old unit's relative life and mana

Selection - Select (Last replaced unit) for (Owner of (Triggering unit))

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Issued order) Equal to (Order(windwalk))

Then - Actions
-------- Basic Construction --------

Unit - Replace (Triggering unit) with a Peasant using The old unit's relative life and mana

Selection - Select (Last replaced unit) for (Owner of (Triggering unit))

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions


Tried to improve it. Now i can add more build orders of the same type (basic or advanced) without stopping the previous build orders.


#1

Human Build Icons Copy
Events
Unit - A unit Is issued an order with no target

Conditions
Or - Any (Conditions) are true
Conditions
(Unit-type of (Triggering unit)) Equal to Peasant

Actions
-------- IF WORKER PRESSED "BUILD" OR "ADVANCED BUILD" --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Issued order) Equal to (Order(manashieldon))

(Issued order) Equal to (Order(manashieldoff))

Then - Actions
-------- Advanced Construction --------

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Issued order) Equal to (Order(windwalk))

Then - Actions
-------- Basic Construction --------

Unit - Replace (Triggering unit) with a Peasant using The old unit's relative life and mana

Selection - Select (Last replaced unit) for (Owner of (Triggering unit))

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions

#2

Human Build Icons
Events
Unit - A unit Is issued an order with no target

Conditions
Or - Any (Conditions) are true
Conditions
(Unit-type of (Triggering unit)) Equal to Peasant

Actions
-------- IF WORKER PRESSED "BUILD" OR "ADVANCED BUILD" --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Issued order) Equal to (Order(manashieldon))

(Issued order) Equal to (Order(manashieldoff))

Then - Actions
-------- Advanced Construction --------

Unit - Replace (Triggering unit) with a Peasant using The old unit's relative life and mana

Selection - Select (Last replaced unit) for (Owner of (Triggering unit))

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Issued order) Equal to (Order(windwalk))

Then - Actions
-------- Basic Construction --------

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions

#### aple

#### aple

 Level 12

JoinedMay 20, 2009Messages821

Resources: 5

#### Resources
Maps 2Spells and Systems 2Tutorials 1

Yeah, I forgot about that.

ZanManagement didn't run into that issue yet because they only had Zerg recreated. So it was a lot harder for that issue to occur.

I'll try and come up with something.

Also, this system wasn't entirely finished when they released the map. I do believe they said this was something that they were only using temporarily, that they needed to make a better system for.

I'll see what I can come up with, but my knowledge of Triggers is still fairly limited. I'm very new to Triggers, so I practice with stuff other people made.

Edit: Woah. While I was typing this, it was still one page. o_o

#### aple

#### aple

 Level 12

JoinedMay 20, 2009Messages821

Resources: 5

#### Resources
Maps 2Spells and Systems 2Tutorials 1

WaterKnight said:

 blub

edit: Yes, your described problem appears. Maybe this can be solved via just in time set-changing because once the structure is there, it's good to go. For example catch when a unit comes within building range.


Click to expand...

The whole reason this system does what it does is to emulate Basic and Advanced Build from WarCraft II, StarCraft, StarCraft II, etc.

The way you have it set up doesn't do that at all.

It's also a bit harder to navigate, because you have to worry about the switch ability not having the same hotkey as any of the buildings. Whereas the other way you just have to worry about it not mixing with the other Worker Commands.

TaShadan said:

 I created a second peasant and triggered this. Now it is possible to build with several workers without breaking the order, but now the problem is that the worker using the ability stops every action ordered before.

Human Build Icons
Events
Unit - A unit Is issued an order with no target

Conditions
Or - Any (Conditions) are true
Conditions
(Unit-type of (Triggering unit)) Equal to Peasant

(Unit-type of (Triggering unit)) Equal to Peasant

Actions
-------- IF WORKER PRESSED "BUILD" OR "ADVANCED BUILD" --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Issued order) Equal to (Order(manashieldon))

(Issued order) Equal to (Order(manashieldoff))

Then - Actions
-------- Advanced Construction --------

Unit - Replace (Triggering unit) with a Peasant using The old unit's relative life and mana

Selection - Select (Last replaced unit) for (Owner of (Triggering unit))

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Issued order) Equal to (Order(windwalk))

Then - Actions
-------- Basic Construction --------

Unit - Replace (Triggering unit) with a Peasant using The old unit's relative life and mana

Selection - Select (Last replaced unit) for (Owner of (Triggering unit))

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions

Tried to improve it. Now i can add more build orders of the same type (basic or advanced) without stopping the previous build orders.

#1

Human Build Icons Copy
Events
Unit - A unit Is issued an order with no target

Conditions
Or - Any (Conditions) are true
Conditions
(Unit-type of (Triggering unit)) Equal to Peasant

Actions
-------- IF WORKER PRESSED "BUILD" OR "ADVANCED BUILD" --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Issued order) Equal to (Order(manashieldon))

(Issued order) Equal to (Order(manashieldoff))

Then - Actions
-------- Advanced Construction --------

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Issued order) Equal to (Order(windwalk))

Then - Actions
-------- Basic Construction --------

Unit - Replace (Triggering unit) with a Peasant using The old unit's relative life and mana

Selection - Select (Last replaced unit) for (Owner of (Triggering unit))

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions

#2

Human Build Icons
Events
Unit - A unit Is issued an order with no target

Conditions
Or - Any (Conditions) are true
Conditions
(Unit-type of (Triggering unit)) Equal to Peasant

Actions
-------- IF WORKER PRESSED "BUILD" OR "ADVANCED BUILD" --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Issued order) Equal to (Order(manashieldon))

(Issued order) Equal to (Order(manashieldoff))

Then - Actions
-------- Advanced Construction --------

Unit - Replace (Triggering unit) with a Peasant using The old unit's relative life and mana

Selection - Select (Last replaced unit) for (Owner of (Triggering unit))

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Issued order) Equal to (Order(windwalk))

Then - Actions
-------- Basic Construction --------

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions

Click to expand...

This would work if there was a similar system in SC2 that allows different units to act like they were the same unit. But unfortanetely this will cause there to be 2 different units that you'll have to switch back and forth by pressing the Tab key.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

aple said:

 It's also a bit harder to navigate, because you have to worry about the switch ability not having the same hotkey as any of the buildings. Whereas the other way you just have to worry about it not mixing with the other Worker Commands.


Click to expand...

The total set of objects in the build menu is static (and there are always only max 10 buildings at a time) whereas outside the unit may gain new abilities.

@TaShadan: Replacing the unit kills a lot, like all the buffs on the unit, the order, the animation, any runtime-set characteristic. At least use passive morphing then.

#### aple

#### aple

 Level 12

JoinedMay 20, 2009Messages821

Resources: 5

#### Resources
Maps 2Spells and Systems 2Tutorials 1

There's unfortanetely no other way for this to be done, unless something in JASS can help us.

Enable/Disable Unit is the only thing that can remove the button. Which is all we really need to do, so if there is a way in JASS to remove the button without actually disabling the tech than we'd be good to go.

#### TaShadan

#### TaShadan

 Level 13

JoinedJul 2, 2008Messages1,179

Resources: 1

#### Resources
Maps 1

WaterKnight said:

 The total set of objects in the build menu is static (and there are always only max 10 buildings at a time) whereas outside the unit may gain new abilities.


@TaShadan: Replacing the unit kills a lot, like all the buffs on the unit, the order, the animation, any runtime-set characteristic. At least use passive morphing then.


Click to expand...


Which morph do you suggest? I tried unbearform and bearform but iam not able to do it


UPDATE: ok i did it


I created a new ability based on bear form and changed the morph time to 0.1 and the alternative and normal form to the 2 peasants.

Human Build Icons
Events
Unit - A unit Is issued an order with no target

Conditions
Or - Any (Conditions) are true
Conditions
(Unit-type of (Triggering unit)) Equal to Peasant

Actions
-------- IF WORKER PRESSED "BUILD" OR "ADVANCED BUILD" --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Issued order) Equal to (Order(bearform))

Then - Actions
-------- Advanced Construction --------

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Issued order) Equal to (Order(windwalk))

Then - Actions
-------- Basic Construction --------

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions

Human Build Icons Copy
Events
Unit - A unit Is issued an order with no target

Conditions
Or - Any (Conditions) are true
Conditions
(Unit-type of (Triggering unit)) Equal to Peasant 2

Actions
-------- IF WORKER PRESSED "BUILD" OR "ADVANCED BUILD" --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Or - Any (Conditions) are true
Conditions
(Issued order) Equal to (Order(manashieldon))

(Issued order) Equal to (Order(manashieldoff))

Then - Actions
-------- Advanced Construction --------

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Issued order) Equal to (Order(unbearform))

Then - Actions
-------- Basic Construction --------

-------- FORCE "BUILD" KEY --------

Game - Force (Owner of (Ordered unit)) to press the key Z

Else - Actions

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

TaShadan said:

 Which morph do you suggest? I tried unbearform and bearform but iam not able to do it

Click to expand...

https://www.hiveworkshop.com/forums...rials-278/hero-passive-transformation-198482/

And no, it's not restricted to heroes.

#### TaShadan

#### TaShadan

 Level 13

JoinedJul 2, 2008Messages1,179

Resources: 1

#### Resources
Maps 1

Now i have something strange. The advanced worker cant build anything. He walks the the ordered position but nothing happens.....

I also realized a problem with all the solutions. If you use hotkeys to build, for example B + F to build a farm, if you are too fast nothing happens and you have to press f again. Oh Blizzard why didnt you implement that hardcoded....

 And no, it's not restricted to heroes.


Click to expand...

Sorry iam not stupid... every ability can be changed to unit.


 Last edited: Aug 17, 2013

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Thanks for the submission aple. And thank you, TaShadan and WaterKnight for the feedback.

The tutorial itself looks fine. It just needs a test map (apparently it was removed). Perhaps you can also fine tune it in accordance to TaShadan's suggestions.

#### RiotZ

#### RiotZ

 Level 22

JoinedJul 25, 2009Messages3,087

Resources: 3

#### Resources
Maps 2Tutorials 1

Channel can be made to use two separate order strings... /facepalm

#### loktar

#### loktar

 Level 47

JoinedNov 2, 2004Messages2,348

Resources: 90

#### Resources
Models 48Icons 30Tools 2Maps 4Spells and Systems 5Tutorials 1

##
 Melee Classics


The way I did it is much cleaner and more efficient imo:

First, make sure all basic and advanced buildings are added to your worker's Structures Built.

I made two copies of the Channel ability and changed the following:

-Hero Ability: False

-Levels: 1

-All other Data/Stats: 0/False

-Base Order ID/Order String: militia for Advanced Build and militiaoff for Basic Build (change it to something else if you're using the Militia ability, like manashieldon/off)

-Options: Visible

These can be used for any number of races (unless you want different icons).

Next, I defined a few variables:

-BuildAdvanced[]: Unit-Type Array (one for every race); Advanced Buildings Preset

-BuildBasic[]: Unit-Type Array (one for every race); Basic Buildings Preset

-BuildCount[]: Integer Array (Default: 0); Keeps track of number of Buildings per Set

-BuildCurrent[]: Ability Array; Keeps track of active Build Set

-Set TempAbility[]: Current ability being cast, per build set

-Set TempPlayer[]: Owner of currently switching worker, per build set

Finally, three triggers (per race):

-Build Set: Here we set the presets for available buildings

Build Set
Events
Map initialization

Conditions

Actions
-------- BASIC (Index: 0) --------

-------- Building Type 1 --------

Set BuildBasic[BuildCount[0]] = Building Type 1

Set BuildCount[0] = (BuildCount[0] + 1)

-------- Building Type 2 --------

Set BuildBasic[BuildCount[0]] = Building Type 2

Set BuildCount[0] = (BuildCount[0] + 1)

-------- ADVANCED (Index: 1) --------

-------- Building Type 3 --------

Set BuildAdvanced[BuildCount[1]] = Building Type 3

Set BuildCount[1] = (BuildCount[1] + 1)

-------- Building Type 4 --------

Set BuildAdvanced[BuildCount[1]] = Building Type 4

Set BuildCount[1] = (BuildCount[1] + 1)

-Build Basic: This is where we enable basic buildings and disable advanced buildings

Build Basic
Events
Unit - A unit Begins casting an ability

Conditions
(Ability being cast) Equal to (==) Build Basic

(Unit-type of (Triggering unit)) Equal to (==) Peasant

Actions
Trigger - Turn off (This trigger)

Trigger - Turn off Build Advanced <gen>

Set TempAbility[0] = (Ability being cast)

Set TempPlayer[0] = (Triggering player)

Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
-------- If Build Set is already active, skip Build Set change --------

BuildCurrent[(Player number of (TempPlayer[0])] Not equal to (!=) TempAbility[0]

Then - Actions
-------- Activate Basic Build Set --------

Do ActionFor each (Integer A) from 0 to (BuildCount[0] - 1), do (Player - Make BuildBasic[(Integer A)] Available for training/construction by (TempPlayer[0])

-------- Deactivate Advanced Build Set --------

Do ActionFor each (Integer A) from 0 to (BuildCount[1] - 1), do (Player - Make BuildAdvanced[(Integer A)] Unavailable for training/construction by (TempPlayer[0])

-------- Update Current Build Set --------

Set BuildCurrent[(Player number of TempPlayer[0])] = TempAbility[0]

Else - Actions
Do nothing

Unit - Order (Triggering unit) to Build

Trigger - Turn on (This trigger)

Trigger - Turn on Build Advanced <gen>

-Build Advanced: Enable advanced buildings and disable basic buildings

Build Advanced
Events
Unit - A unit Begins casting an ability

Conditions
(Ability being cast) Equal to (==) Build Advanced

(Unit-type of (Triggering unit)) Equal to (==) Peasant

Actions
Trigger - Turn off (This trigger)

Trigger - Turn off Build Basic <gen>

Set TempAbility[1] = (Ability being cast)

Set TempPlayer[1] = (Triggering player)

Multiple FunctionsIf (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
-------- If Build Set is already active, skip Build Set change --------

BuildCurrent[(Player number of TempPlayer[1])] Not equal to (!=) TempAbility[1]

Then - Actions
-------- Activate Advanced Build Set --------
Do ActionFor each (Integer A) from 0 to (BuildCount[1] - 1), do (Player - Make BuildAdvanced[(Integer A)] Available for training/construction by (TempPlayer[1])

-------- Deactivate Basic Build Set --------
Do ActionFor each (Integer A) from 0 to (BuildCount[0] - 1), do (Player - Make BuildBasic[(Integer A)] Unavailable for training/construction by (TempPlayer[1])

-------- Update Current Build Set --------

Set BuildCurrent[(Player number of (TempPlayer[1])] = TempAbility[1]

Else - Actions
Do nothing

Unit - Order (Triggering unit) to Build

Trigger - Turn on (This trigger)

Trigger - Turn on Build Basic <gen>

Two major flaws:

-Builder's orders are cancelled whenever either Build button is pushed (still better than forcing UI key imo, since that's unreliable and adds a second click sound)

-Scenario-specific buildings Tech-Tree becomes obsolete (also the case in OP tutorial)

+Probably the Acolyte thing mentioned in OP, not tested

Other than that, as far as I can tell, it works flawlessly, and should function just as well in multiplayer games (MP not tested).

If there is a way to detect the Build Menu and/or a Spell Book being opened, the first flaw would be fixed. I don't think it's possible though, they don't respond to any ability/order event.

A much better solution would be morphing the worker to one with different Buildings Constructed, but when you do that, the morphed worker won't be able to build the new buildings for some reason.

#### Attachments

Advanced Build Test.w3x

 21.9 KB

 · Views: 331




 Last edited: Dec 20, 2013

#### TaShadan

#### TaShadan

 Level 13

JoinedJul 2, 2008Messages1,179

Resources: 1

#### Resources
Maps 1

 -Builder's orders are cancelled whenever either Build button is pushed


Click to expand...

Iam a guy with fairly high apm and this is the worst flaw for a rts game IMO.

#### loktar

#### loktar

 Level 47

JoinedNov 2, 2004Messages2,348

Resources: 90

#### Resources
Models 48Icons 30Tools 2Maps 4Spells and Systems 5Tutorials 1

##
 Melee Classics


Hm yeah, it is pretty annoying.

Is the Force UI Key method reliable at all in your opinion? I could live with the double click sound, but if it's not reliable, I'd rather have orders be cancelled.


 Last edited: Dec 19, 2013

#### TaShadan

#### TaShadan

 Level 13

JoinedJul 2, 2008Messages1,179

Resources: 1

#### Resources
Maps 1

I dont know if Force UI is better than your solution, but i think its not good enough aswell. Could you upload a test map with your solution?

#### loktar

#### loktar

 Level 47

JoinedNov 2, 2004Messages2,348

Resources: 90

#### Resources
Models 48Icons 30Tools 2Maps 4Spells and Systems 5Tutorials 1

##
 Melee Classics


Yeah, neither is exactly ideal. I really wish there was a way to catch build menu/spell book open events. Or to morph a worker and the available structures actually working


Here's a test map.


Edit: I had the idea of keeping track of the orders worker units are given, in array with the unit ID as the index, but I can't figure out a way to get the ordered unit's ID


I updated the post with the triggers, improved efficiency a bit, also updated test map


 Last edited: Dec 20, 2013

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Overall, there isn't a tutorial like this and I think this gets the general idea off. Of course, it has its drawbacks, but it works pretty well judging by the test map. I linked loktar's post at the end as an alternative implementation.

Otherwise, I see no reason not to approve this. Approved!

#### aple

#### aple

 Level 12

JoinedMay 20, 2009Messages821

Resources: 5

#### Resources
Maps 2Spells and Systems 2Tutorials 1

I am still working on this to get it fully MUI compatible and I feel like I'm coming very close.

EDIT: This CAN be made MUI compatible, but it requires Spellbooks. Which are glitchy as fuck and you won't see a Gold/Lumber cost unless you manually put it in the description of the building. But it CAN be made completely functional, and Humans and Orcs and Night Elves WILL build normally.

I'll post a new tutorial or update this one with a second method if anyone requests it.

For those that'll know what to do, though:

You need to hide the normal build icon via the text files->Then make all the appropriate spells->When the spell fires, issue a build order to the worker.

Ex:

 JASS:

function Trig_Test_Copy_Conditions takes nothing returns boolean
 if ( not ( GetUnitTypeId(GetOrderedUnit()) == 'hpea' ) ) then
 return false
 endif
 return true
endfunction

function Trig_Test_Copy_Func002C takes nothing returns boolean
 if ( not ( GetIssuedOrderIdBJ() == 1751674741 ) ) then //This is the order of the spell that will go out for "Build Tiny" spells. It'll be a random jumble of numbers. THIS particular one is actually the normal order that goes out when a Peasant is told to build a Farm.
 return false
 endif
 return true
endfunction

function Trig_Test_Copy_Actions takes nothing returns nothing
 if ( Trig_Test_Copy_Func002C() ) then
 call DisplayTextToForce( bj_FORCE_PLAYER[0], "TRIGSTR_058" )
 call IssueImmediateOrderBJ( GetTriggerUnit(), "stop" )
 call IssueBuildOrderById(GetTriggerUnit(), 'BUILDING', GetOrderPointX(), GetOrderPointY())
 else
 endif
endfunction

//===========================================================================
function InitTrig_Test_Copy takes nothing returns nothing
 set gg_trg_Test_Copy = CreateTrigger( )
 call TriggerRegisterAnyUnitEventBJ( gg_trg_Test_Copy, EVENT_PLAYER_UNIT_ISSUED_POINT_ORDER )
 call TriggerAddCondition( gg_trg_Test_Copy, Condition( function Trig_Test_Copy_Conditions ) )
 call TriggerAddAction( gg_trg_Test_Copy, function Trig_Test_Copy_Actions )
endfunction

(Big thanks goes to IceManBo for helping me)

This will also be GREATLY helpful, though I forgot who made it. I DID NOT MAKE IT:

 JASS:

function Trig_orders_Actions takes nothing returns nothing
 local integer order = GetIssuedOrderId()
 call BJDebugMsg(GetUnitName(GetOrderedUnit()) + " has been ordered " + OrderId2String(order)+ " ("+I2S(order)+")")
endfunction

function InitTrig_orders takes nothing returns nothing
 local trigger t = CreateTrigger()
 call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_ISSUED_TARGET_ORDER )
 call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_ISSUED_POINT_ORDER )
 call TriggerRegisterAnyUnitEventBJ(t, EVENT_PLAYER_UNIT_ISSUED_ORDER )
 call TriggerAddAction(t, function Trig_orders_Actions)
endfunction

Make a new trigger->Name it "orders" (No quotes) and convert it to text then paste that.


 Last edited: Apr 21, 2014

#### JCarrill0

#### JCarrill0

 Level 20

JoinedMay 25, 2004Messages1,518

Resources: 9

#### Resources
Packs 1Maps 5User Interface 3

If & When you get a GUI for this, please let me know! I plan to use it in future maps of mine


 Last edited: Apr 18, 2026

#### XicorD

#### XicorD

 Level 3

JoinedFeb 21, 2014Messages28

So I extract the files from the .MPQ and then just leave them outside or I put them back on the MPQ after I edit them?

#### JCarrill0

#### JCarrill0

 Level 20

JoinedMay 25, 2004Messages1,518

Resources: 9

#### Resources
Packs 1Maps 5User Interface 3

I just used the link they provided, imported them into my map, and followed the instructions. I never had to extract anything from any .mpq

#### XicorD

#### XicorD

 Level 3

JoinedFeb 21, 2014Messages28

I've followed all the steps and it is awesome! Thanks, but I have three build abilities, the normal one, the basic and the advanced, and I don't know how to delete or hide the normal one...

(I was using the more efficent and cleaner way)

#### XicorD

#### XicorD

 Level 3

JoinedFeb 21, 2014Messages28

Ehrrr... Are the users controlled by the computer affected when you add this trigger?

#### JCarrill0

#### JCarrill0

 Level 20

JoinedMay 25, 2004Messages1,518

Resources: 9

#### Resources
Packs 1Maps 5User Interface 3

XicorD said:

 Ehrrr... Are the users controlled by the computer affected when you add this trigger?


Click to expand...

no, ai is not effefcted

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

AI Doesn't react to this system properly however, you need to use a triggered AI of some sort.

#### XicorD

#### XicorD

 Level 3

JoinedFeb 21, 2014Messages28

But... guys... Is there a way to hide the original Build (race) spell from the worker? I have 3 build spells on the Peasant... The original one, the basic and the advanced, in the demo the original one has the yellow cross icon and it is in another position, how do I make this?


 Last edited: Jun 12, 2014

#### JCarrill0

#### JCarrill0

 Level 20

JoinedMay 25, 2004Messages1,518

Resources: 9

#### Resources
Packs 1Maps 5User Interface 3

XicorD said:

 But... guys... Is there a way to hide the original Build (race) spell from the worker? I have 3 build spells on the Peasant... The original one, the basic and the advanced, in the demo the original one has the yellow cross icon and it is in another position, how do I make this?


Click to expand...

you got to download the map, and use the imported files the map gives you. It's pretty easy to do.


Read post #1 again, but fully this time. You probably missed it.

#### XicorD

#### XicorD

 Level 3

JoinedFeb 21, 2014Messages28

Thanks! I already made it.

#### XicorD

#### XicorD

 Level 3

JoinedFeb 21, 2014Messages28

Hmm, everything was working okay till i made the build basic and advanced for two races and not only one ('don't know how to explain myself), it is maybe because I used two different spells? I mean...

Build Basic (Human)

Build Advanced (Human)

Build Basic (Illidan's Forces)

Build Advanced (Illidan's Forces)

I am using the cleaner and more efficent way given by loktar. I wanted to make two spells for each race so they all can have different icons instead of only one.

EDIT: I tried using the same spell I used for the humans but it keeps not working... It shows all the buildings instead of only some of them, like, you press Basic Structures and it shows you the basic and advanced one.

EDIT... Again: SHIT, DON'T WORRY. I already solved it, I forgot adding "Map initialization" in the Build Set of the Illidan's Forces.

#### bigboss

#### bigboss

 Level 3

JoinedDec 4, 2008Messages24

Resources: 1

#### Resources
Models 1

hi everyone hmmm i recently tested loktar's method and its fine but when i try it in multiplayer if only one player casts the build abilities it affects all the players who has the same builder type (ex: P1 Peasant casts Build but it also activates for P2 Peasant) can anyone confirmed this??

#### warcraftmodding123

#### warcraftmodding123

 Level 7

JoinedFeb 20, 2016Messages274

Does someone here have a working trigger map example of advanced building?

thanks.

#### Rogue Kessler

#### Rogue Kessler

 Level 10

JoinedApr 4, 2010Messages288

Resources: 5

#### Resources
Maps 4Tutorials 1

I just found this tutorial when I was looking for something else. This is a really clever way to do this!

Thanks for taking the time to write this up! +Rep

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

@aple , could you by chance fix the missing images? Attaching to a post would be best.

 You must log in or register to reply here.


### Similar threads

Locked

[Spell]
 Having trouble with Advanced Build


SNIper of DARKness

Aug 5, 2024

World Editor Help Zone

Replies9Views1K

Aug 11, 2024
Luciano Alencar Boi

Locked

 Disabling icons without actually disabling the building/unit/ability?


aple

Jan 13, 2014

Requests

Replies4Views1K

Jan 13, 2014
aple

Locked

[Solved]
 Detect Building/Structure Menu Click


Droof

Apr 13, 2022

World Editor Help Zone

Replies1Views1K

Apr 13, 2022
Droof

Locked

 How do I make a "Techtree choice"?


tdrsmynm

May 11, 2021

Triggers & Scripts

Replies2Views1K

May 12, 2021
tdrsmynm

Locked

 Ability Selection System


Greenwhy

Nov 12, 2011

World Editor Help Zone

Replies16Views2K

Nov 16, 2011
Adiktuz

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