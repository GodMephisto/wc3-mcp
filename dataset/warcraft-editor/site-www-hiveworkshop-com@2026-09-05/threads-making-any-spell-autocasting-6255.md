---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-any-spell-autocasting.6255/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making Any Spell Autocasting
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-any-spell-autocasting.6255/>
#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


Introduction

Many spells in Warcraft can be cast automatically. This function is called autocast. It is not possible to just tick an option and the selected ability will be able to autocast. You will have to go deeper. You will have to use triggers and maybe an ability that also can autocast, to launch the selected.

There are many ways of making spells autocast. But I will primary be focusing on a mixture of triggers and dummy abilities. This way, when wanting to make a spell autocast, you activate a dummy spell that each time it runs/attacks it orders the unit to cast the real spell. This way when turning on/off autocast, will be like normal, however you will have two ability-icons counting as one.

Types

All spells/abilities can be divided into 3 groups of how their target: Unit, Point, No-Target. Listed below are one of each type:

Unit - Order (Triggering unit) to Orc Raider - Ensnare (Target unit of ability being cast)

Unit - Order (Triggering unit) to Orc Witch Doctor - Sentry Ward (Center of (Playable map area))

Unit - Order (Triggering unit) to Orc Tauren Chieftain - War Stomp
The first is targetting a unit. This is spells like Ensnare, Heal and Shadow Strike.

The second is targetting ground. This is spells like Sentry Ward, Far Sight and Earth Quake.

The third doesn't have a target. This is spells like War Stomp, Roar and Feral Spirit.

So now we know that there are three kinds of spells. But now is the problem, what autocasting spells are there? Well.. The only spells I know that have autocast are target-unit spells. But it's still possible to make the others autocast aswell. Because you can make the spell target the point of the unit you want to hit.

In all my examples I will be using the same method for doing the autocast. I will be using a dummy spell which will be based of Searing Arrow.

Custom Spells

To make a trigger launch with a custom spell you will have to look at the 'Order String - Use/Turn On' and make sure it is the only occurence on that unit, else more spells will be launched at the same time. When selecting the ability / action, instead of choosing preset you choose function, then Conversion - Convert String to Order and write the Order String in that field.

Unit - Order (Some Unit) to (Order(Some_Spell_Name)) (Target unit of ability being cast)
Making the DummySpell

For each autocasting spell, I will recommend you having a dummy spell. I based my dummy spells off Searing Arrow which is the 'Priestess of the Moon' ability. I have removed ALL models, damages, levels and anything that makes it be like a normal spell. Also removed mana cost and set cooldown to 5 just for standard.

Normally I set the range of the dummy spell to the same value of the spell to be casted. But you have to do it another way with abilities like War Stomp which is a No-Target spell. You have to set the range of the dummy spell to maximum the area of effect of the spell to run.

Example One - Targetting a Unit

Always remember to set the cooldown of the dummy ability to the cooldown of the ability it should start or less than it. Also remember to set the range to the same.

Uses Dummy
Events
Unit - A unit Begins channeling an ability

Conditions
(Ability being cast) Equal to Dummy

Actions
Unit - Order (Triggering unit) to Orc Raider - Ensnare (Target unit of ability being cast)

The trigger runs each time an ability has been started, then it checks that the ability is the Dummy Spell, if it is, it will order the unit to use Ensnare.

Example Two - Targetting Ground

This one is a bit more tricky since you can't just make the unit hit the unit which the Dummy Spell will hit, you will have to localize the targetted unit and hit it. Remember to set the cooldown of the dummy ability to the cooldown of the ability it should start or less than it.

Uses Dummy2
Events
Unit - A unit Begins channeling an ability

Conditions
(Ability being cast) Equal to Dummy2

Actions
Unit - Order (Triggering unit) to Human Blood Mage - Flame Strike (Position of (Target unit of ability being cast))

The trigger runs each time an ability has been started, then it checks that the ability is the Dummy Spell, if it is, it will order the unit to use Flame Strike.

Example Three - No Target

This is the one that could cause most problems because the dummy is a ranged ability while this is not ranged, however it is not that hard.

Always remember to set the range of the Dummy Spell to ½ of the range the standard ability has, else it can cause problems:

Let's say the Dummy Spell has 2000 range and the unit has War Stomp, the range of it is 1000. So when the Dummy Spell hits some unit, it will do War Stomp, but the unit won't be harmed because it's too far away. Also remember to set the cooldown to atleast the same of the original ability.

Uses Dummy3
Events
Unit - A unit Begins channeling an ability

Conditions
(Ability being cast) Equal to Dummy3

Actions
Unit - Order (Triggering unit) to Orc Tauren Chieftain - War Stomp

Bonus Example - Autocasting without double icons

Ofcourse it would be cooler with just one icon for a spell but then it would be hard to make it so you can choose autocast or not. This following example will order a unit to keep on doing Flame Strike, Ensnare and War Stomp on the units it attacks or is being attacked by:

No Dummy
Events
Unit - A unit Is attacked

Conditions

Actions
Unit - Order (Attacking unit) to Human Blood Mage - Flame Strike (Position of (Target unit of ability being cast))

Unit - Order (Attacking unit) to Orc Raider - Ensnare (Target unit of ability being cast)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
((Attacking unit) is in (Units within 1000.00 of (Position of (Attacked unit)))) Equal to True

Then - Actions
Unit - Order (Attacking unit) to Orc Tauren Chieftain - War Stomp

Else - Actions

This example shows ALL functions within one trigger. You can ofcourse cut it out in parts with an action in each.

Well I hope you learned something about making your own autocasting spells now!

Ralle


 Last edited: Oct 26, 2006

#### Sunreaver

#### Sunreaver

 Level 18

JoinedAug 22, 2013Messages1,290

Resources: 16

#### Resources
Models 2Packs 1Maps 13

Can u make a dummy map pleaseee Ralle Bossie

#### Necziaxxus

#### Necziaxxus

 Level 4

JoinedFeb 24, 2015Messages50

This is a 9 year old post. How'd you even find this Sunreaver =]

#### Sunreaver

#### Sunreaver

 Level 18

JoinedAug 22, 2013Messages1,290

Resources: 16

#### Resources
Models 2Packs 1Maps 13

Looking for spell help

#### The Kurd

#### The Kurd

 Level 3

JoinedOct 29, 2013Messages51

what about autocasting custom spells?

#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


@The Kurd The tutorial mentions that at the beginning.


 Last edited: Nov 18, 2017

#### The Kurd

#### The Kurd

 Level 3

JoinedOct 29, 2013Messages51

GhostHunter123 said:

@The Kurd The tutorial mentions that at the beginning.


Click to expand...

Im not sure what that part of the tutorial is saying. is there a way to rephrase it? It could just be I have been working to long and my brain isn't working anymore. The line of script he has as an example doesn't seem to have an option for the spell I made or a "put custom spell here" spot.

#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


I was wrong. This tutorial is entirely for custom spells or pretty much any spell that is not autocastable by default.

You have to order the unit to cast the spell that your custom spell is based on.

The (order(some spell name)) generalizes what the unit must do. It is not an actual action that you can select in the GUI database.


 Last edited: Nov 18, 2017

#### The Kurd

#### The Kurd

 Level 3

JoinedOct 29, 2013Messages51

GhostHunter123 said:

 I was wrong. This tutorial is entirely for custom spells.

You have to order the unit to cast the spell that your custom spell is based on.

The order(some spell name) generalizes what the unit must do.


Click to expand...

I tried this out and it doesn't seem to be working. I'm using the exact first trigger with a modified version of slow (for auto cast) to auto cast a modified version of chain lightning but when it is cast all that happens is the base dummy ability is cast.


 Last edited: Nov 18, 2017

#### The Kurd

#### The Kurd

 Level 3

JoinedOct 29, 2013Messages51

Unit - A unit Begins channeling an ability

(Ability being cast) Equal to Shoot (Flintlock) "slow"

Unit - Order (Casting unit) to Orc Far Seer - Chain Lightning (Target unit of ability being cast)

#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


Order the unit to cast slow, not chain lightning, since your spell is based on slow.

#### Rogue Kessler

#### Rogue Kessler

 Level 10

JoinedApr 4, 2010Messages288

Resources: 5

#### Resources
Maps 4Tutorials 1

I seem to be having two problems.

My unit doesn't cast the autocast ability (based on Searing Arrows) when it attacks even when autocast is on, unless it was ordered directly to attack the target (as opposed to just noticing and attacking the enemy itself).

When the unit does use the autocast ability, the trigger I made based on this tutorial isn't ordering the unit to cast its other ability (based on Shadow Strike). Essentially the trigger doesn't work.
Here is my trigger:

Events

Unit - A unit Begins channeling an ability​
Conditions

(Ability being cast) Equal to (My dummy ability based on Searing Arrows)​
Actions

Unit - Order (Triggering unit) to Night Elf Warden - Shadow Strike (Target unit of ability being cast)

​
I tried changing the event to "Begins casting" or "Starts the effect of an ability". I also tried changing "Triggering unit" to "Casting unit" or even a variable that is already set to the unit I know I want, but none of that made any difference.

FWIW, I also don't have the option to choose "Function" for the order the way this tutorial suggests. I am only able to choose "Preset".

I know this is a necro post, but does anyone have any ideas? This system seems like it would be sweet if it actually worked.

#### Rogue Kessler

#### Rogue Kessler

 Level 10

JoinedApr 4, 2010Messages288

Resources: 5

#### Resources
Maps 4Tutorials 1

Jake Kessler said:

 I seem to be having two problems.

My unit doesn't cast the autocast ability (based on Searing Arrows) when it attacks even when autocast is on, unless it was ordered directly to attack the target (as opposed to just noticing and attacking the enemy itself).

When the unit does use the autocast ability, the trigger I made based on this tutorial isn't ordering the unit to cast its other ability (based on Shadow Strike). Essentially the trigger doesn't work.
Here is my trigger:

Events
Unit - A unit Begins channeling an ability

Conditions
(Ability being cast) Equal to (My dummy ability based on Searing Arrows)

Actions
Unit - Order (Triggering unit) to Night Elf Warden - Shadow Strike (Target unit of ability being cast)

I tried changing the event to "Begins casting" or "Starts the effect of an ability". I also tried changing "Triggering unit" to "Casting unit" or even a variable that is already set to the unit I know I want, but none of that made any difference.

FWIW, I also don't have the option to choose "Function" for the order the way this tutorial suggests. I am only able to choose "Preset".

I know this is a necro post, but does anyone have any ideas? This system seems like it would be sweet if it actually worked.


Click to expand...

So - I'm still not sure how to make this system work the way OP intended. But I've figured out a workaround that is functional and does what I need in my map. It uses 3 triggers, as follows:

Trigger 1

Events
Unit - A unit is issued an order with no target

Conditions
(Issued order) Equal to (Order(flamingarrows))

Actions
Trigger - Turn on Trigger 3

Trigger 2

Events
Unit - A unit is issued an order with no target

Conditions
(Issued order) Equal to (Order(unflamingarrows))

Actions
Trigger - Turn off Trigger 3

Trigger 3

Events
Unit - A unit Is attacked

Conditions
(Attacking unit) Equal to Unit_Garona

Actions
Unit - Order Unit_Garona to Night Elf Warden - Shadow Strike (Attacked unit)


 Last edited: Dec 22, 2017

 You must log in or register to reply here.


### Similar threads

Locked

 problem with Dummy Autocasting


Euroset

Nov 19, 2006

Triggers & Scripts

Replies1Views1K

Nov 19, 2006
Euroset

Locked

[General]
 Need help with autocasting abilities!


Mistablockout

Dec 8, 2017

World Editor Help Zone

Replies6Views2K

Dec 15, 2017
fireblasts

Locked

 Autocasting Firebolt


AznShadowFury

Dec 8, 2007

Triggers & Scripts

Replies8Views3K

Dec 16, 2007
Need_O2

 Introduction To Trigger-Enhancing Spells


whitenights

Jul 27, 2004

Trigger (GUI) Editor Tutorials

Replies6Views36K

Jun 8, 2016
TheLordOfChaos201

 How to make a Chain spell (GUI Version)


Daelin

Aug 13, 2006

Trigger (GUI) Editor Tutorials

Replies1Views40K

Jan 14, 2019
dower39

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