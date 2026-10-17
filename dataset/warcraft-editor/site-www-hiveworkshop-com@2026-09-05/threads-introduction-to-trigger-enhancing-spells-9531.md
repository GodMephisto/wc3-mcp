---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/introduction-to-trigger-enhancing-spells.9531/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Introduction to Trigger Enhancing Spells
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/introduction-to-trigger-enhancing-spells.9531/>
#### whitenights

#### whitenights

 Level 3

JoinedMar 25, 2004Messages51

Resources: 1

#### Resources
Tutorials 1

This tutorial will take you through the basics of trigger-enhancing a spell. Covered in this tutorial are:

Using a dummy spell

Creating Special Effects

Good habits when creating spells


This tutorial assumes you have a very basic knowledge of the trigger editor and object editor. Trigger-enhanced spells vary widely, so this gives you basic skills to create a spell.

1) You will first need an idea.


In this tutorial, my idea is to create a mass-healing spell. The spell will target all units in an area and heal them. I want this to be a Hero ability.

2) Then you will want to create a map to place this ability in.


These maps are commonly called "testmaps" and they are what you download in the Spells area. The map dosn't need to be anything special. I usually create a 64x64 dirt map and make it better later. This tutorial will assume that you have made a blank map.

3) You will then want to make a dummy ability.

Explanations:


If you are saying "HELP!!!!!!! What's a dummy ability", then a dummy ability is an ability that only costs you mana, has an icon, and has the tooltips for the user to see. In other words, it is only something that the end user clicks on. All of the work for this spell will be done by triggers.


I want to use taunt for my dummy ability. I use taunt because taunt is an ability that works instantly. If you want to use a unit-targeted ability, Storm Bolt is good, and for a point-targeted spell, Shockwave works well.


So what do I have to do to create my dummy ability? well, Taunt is a unit ability, so I have to make it a Hero ability. I also don't want the actual effect of Taunt (making all units attack my Hero). I also want to change the tooltips and such to make it sound like my ability.

Step-by-Step:
Open up the Object Editor

Go to the Abilities tab

Click the "New Custom Ability" button (the fist right above the "Destructables" tab)

Set the Base Ability to Taunt. (If you can't see it, change "Hero" to "Unit").

Set the name to Heal All (This could be anything you want)

Start from the top and go down.
We don't want Taunt under Art - Caster, so get rid of that.

Let's change the Icon to Healing Wave.

The Area of Effect should be 0, so that the spell will not actually affect anyone.

The cooldown could be changed to 20 (seconds).

Let's change the Race to Human.

Change Targets Allowed to None.

Now lets do something else. Set Stats - Hero Ability to True.

Now scroll back up and set the Research Icon to Healing Wave.

Under Text, create Tooltips. Here were mine:
Text - Tooltip - Learn: "Learn Heal All"

Text - Tooltip - Learn - Extended: "Heal All will heal all units in a small area within your Hero."

Text - Tooltip - Normal: "Heal All"

Text - Tooltip - Normal - Extended: "Heal all units in a small area within your Hero"

Now let's place that in a unit. I won't go over how to do that because it is really easy. I put it in a Paladin in place of Holy Light.

4) Then you will create some triggers.

Explanations: These triggers will have to figure out every unit within a range of the casting Hero, create a Heal special effect, and heal them for 100 hit points.

Step-by-Step:

Open the Trigger Editor and create a new trigger.

Call the trigger "Heal All"


Make one Event:

Unit - A unit Starts the effect of an ability
Now, that will fire every time a unit casts an ability. The reason to use "Starts the effect of an ability" is because that will make the actions start once the mana has been deducted from a unit and the cooldown has started.


We need to maks sure which ability is being cast, don't we? We wouldn't want the healing actions to start when you cast Divine Shield or any other ability, right? So let's create a Condition.

(Ability being cast) Equal to Heal All
Now we create the meat of the triggers. The Actions. This is where everything happens. Follow along carefully or your ability will not work.

Unit Group - Pick every unit in (Units within 500.00 of (Position of (Casting unit))) and do (Actions)
Whew... what did that do? It picked every unit within 500.00 of the Casting Unit and did (Actions) that we have not added yet. Let's go over a few important points. The Position of the Casting Unit is where the center of the range starts. You cannot do (Triggering unit) here. That is a common mistake. It MUST be (Casting unit). Now let's add some actions.

Loop - Actions
Unit - Set life of (Picked unit) to ((Life of (Picked unit)) + 100.00)

Special Effect - Create a special effect attached to the chest of (Picked unit) using Abilities\Spells\Human\Heal\HealTarget.mdl

Let's explain what went on there. First it set the life of the Picked Unit to the Life of the Picked unit plus 100. It then created a heal animation attached to the chest of the picked unit. (Abilities\Spells\Human\Heal\HealTarget.mdl is Heal under Abilities). This isn't really tough to understand compared to some spells.

Good habits when creating spells:

Delete the Melee Initialization trigger. It's really easy to do and makes the spell look better.

KISS!!! KEEP IT SIMPLE, STUPID!!! This is the easiest way to make a trigger-enhanced healing spell.

To test your beautiful spell:
Create the unit with the ability attached to it and make some units around it (damage them if you can).

Hit contol-F9 and enjoy.

To make this spell better you could do some things:

Make the spell not target the Hero or any enemy units

Make better tooltips

Add 3 levels to it (maybe the range increases wih every level?)

Do some hotkeys for it.

When you post your ability to the site:


Keep the Hero alive (give him Ankh's of ressruection at the least). Give some creeps and units to test your ability on. Set the name, author, and desciption in the Map Properties menu. Make sure you set the Author to your user account. Make a screenshot (at least 256x256 pixels, usually more). Make a post immediatly after you submit so that you can post more info. about your spell. Keep the map description short and to the point and say everything you wanted to say in your post.


 Last edited by a moderator: Mar 28, 2007

#### Tidus

T

#### Tidus

I CANT FUCKING DO THE FIRST LOOP ACTION ACTION I CAN ONLY DO

Unit- Set life of (picked unit) to (life of (picked unit))

I cant find +100 AND IM GETTING FUCKING PISSED HOW THE FUCK TO DO THIS!!!!!!!!!!

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

why do people bump these old threads? -.- just create a thread at the world editor help zone..

#### Davian_Thule

#### Davian_Thule

 Level 2

JoinedMay 3, 2015Messages16

This is awesome! But how do we pick group units specifically? I mean I don't want it to heal Player 12 (Brown).

#### gio1786

G

#### gio1786

guys after I am creating new trigger,I cant find an option: "Unit - A unit Starts the effect of an ability" of this, its normal or my editor is not working ? )

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

You should be able to find it if you create a new event, look for it in the event list thoroughly. You'll find it there.

#### TheLordOfChaos201

#### TheLordOfChaos201

 Level 19

JoinedJul 2, 2011Messages2,161

I thought those was going to show how to create enhanced spell triggers or pre-existing spells

what your demonstrating here is only 1 spell, and, a spell that doesn't need trigs to run because you could use the item ruin heal which does the same thing.

 You must log in or register to reply here.


### Similar threads

 Making any Spell Autocasting


Ralle

Mar 15, 2006

Trigger (GUI) Editor Tutorials

Replies12Views59K

Dec 22, 2017
Rogue Kessler

 Spell System for GUI MUI Coding


Daffa

May 5, 2019

Trigger (GUI) Editor Tutorials

Replies7Views4K

Oct 3, 2020
Chaosy

 Basics of Trigger Enhancing Spells


Daelin

Nov 26, 2006

Trigger (GUI) Editor Tutorials

Replies10Views92K

Jul 8, 2026
Shyvv

Locked

[Trigger]
 Trigger enhanced AoE healing spell...


Amathor929

Oct 15, 2011

Triggers & Scripts

Replies7Views2K

Oct 16, 2011
baassee

 How to make a Sword/Slash Ability (No Target)


sPy

Dec 6, 2010

Trigger (GUI) Editor Tutorials

Replies9Views19K

Mar 2, 2014
PurgeandFire

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