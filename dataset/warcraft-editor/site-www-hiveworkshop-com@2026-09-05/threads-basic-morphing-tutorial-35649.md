---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/basic-morphing-tutorial.35649/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Basic Morphing Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/basic-morphing-tutorial.35649/>
#### FamousPker49

#### FamousPker49

 Level 18

JoinedMay 27, 2007Messages1,689

Resources: 3

#### Resources
Models 1Skins 1Tutorials 1

INTRODUCTION:


Morphing is a common question asked by people who are new to map making, and it is quite simple to perform. I have selected a few units that "Morph" to demonstrate how to properly change the units from one to the other.


1.) First off you need to select your model, and make a few dummy abilities.


2.) When you first make the unit that you are going to morph in the Object Editor, after you have finished, just copy that unit and paste it so that it has all of the same stats. Then add an "Editor Suffix" to designate which unit is the morphed and which is the normal unit.


3.)Now you need to make the dummy abilities, that will make your unit actually morph into the desired form. I have included two examples, a hero morphing ability, and a unit morphing ability. For the hero base ability, I use Illidian's "Metamorphosis" ability, for the unit ability, I use the Gargoyle's "Stone Form" ability, because the unit is flying. If it was a ground unit, I would use the Druid of the Claw's "Bear-Form" ability. Now you need to clear all fields, like so.


Then change the "Normal Form" and "Alternate Form" to their correct units. We will call this first abilty the "Morph" ability, because this is what will cause the unit to morph. Now just repeat that process, for all morphing abilities you will have. Then copy the ability(s) you made and paste them, and rename them the opposite action of the morph. The opposite action ability you pasted we will refer to as the "Unmorph" ability.


4.) Go back to the units you created and give the normal unit the "Morph" Ability and give the alternate form unit the "Unmorph" ability. Now you are done with the Object Editor.


5.)Open up the Trigger Editor. Create a new Category named "Morphing" and create a new Trigger with the same name as your "Morph" Ability. So for this map, it is called Ball-Form. Now for the Event use "A Unit starts the effect of an ability" DO NOT USE "A Unit begins casting an ability" Then for the conditions, use an Ability Comparison, and set it equal to your "Morph" ability. Now for the Action, you need to go to Animation's and use the "Play (Triggering Unit)'s _____ Animation." For this model, the animation is named "Morph", depending on who created the model or whatever, it may have a different name which you can find when changing the model of your custom unit and going to the small display and clicking the arrows above the picture to play the different animations. So all this trigger looks like is:

Ball Form
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Ball Form

Actions
Animation - Play (Triggering unit)'s morph animation

6.) Now when the unit is in it's morphed form, if not unchanged, it will walk like it does in it's normal form. To correct this, we make a Trigger, I name mine to "Walk in Whatever Form." To detect when a unit needs to move, we use the Event "A Unit is issued an order targeting a point" then to make sure only the morphed unit will be picked, set the Condition to a "Unit Type of (Triggering Unit) equal to (Morphed Form of Your Unit)" then for the action, you would play the unit's "walk alternate" animation. And, if you want to increase or decrease speed when the unit is morphed, you can also add a Unit action "Unit-Set (Triggering Unit) movement speed to X" so your trigger should look something like this.

Walk in Ball Form
Events
Unit - A unit Is issued an order targeting a point

Conditions
(Unit-type of (Triggering unit)) Equal to Droideka (Ball-Form)

Actions
Animation - Play (Ordered unit)'s walk alternate animation

Unit - Set (Ordered unit) movement speed to 400.00

7.) Now you need to have your unit be able to "Unmorph." Again we use the same basic layout as your "Morph" ability. The event will be "Unit starts the effect of an ability", but instead, your condition will be an Ability comparison, and set that equal to your "Unmorph" ability. Then you need to play the unit's unmorph animation, with this unit it is called "morph alternate." You do not need to change the movement speed, because it will only change when your unit is morphed so your trigger should end up something like this.

Deploy From Ball Form
Events
Unit - A unit Starts the effect of an Ability

Conditions
(Ability being cast) Equal to Deploy From Ball Form

Actions
Animation - Play (Triggering unit)'s morph alternate animation

8.) Now you can test your map to make sure everything works correctly, and you have just created your first morphing ability! I hope that this tutorial is helpful, and if you really need to see what needs to be done, i have included two examples using some of Olofmoleman's models. Good Luck and thank you for your time.

#### Attachments

Morphing.w3x

 1.5 MB

 · Views: 475




 Last edited by a moderator: Jun 21, 2007

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Basic Morphing Tutorial looks helpful (at first glance). I have edited the document to remove the images linked from imageshack and inserted the attached images in their proper places.


This material is (partially) covered in Ralle's World Editor FAQ. However this provides greater detail and may indeed be approved after more moderators comment on it.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Sorry, what I don't get here is...

what's wrong with Metamorphosis? It plays Morph, and can do all the other stuff that you did there.

#### FamousPker49

#### FamousPker49

 Level 18

JoinedMay 27, 2007Messages1,689

Resources: 3

#### Resources
Models 1Skins 1Tutorials 1

well sometimes, if you have a different model, with for some reason diff animation names this will come in handy, and i had no idea that metamorhposis actually did all that.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Well, the movespeed can just be on the Alternate unit.

As for the alternate form, just add "alternate" to the Required Animation Names (should it need alternate), and it should work fine.

#### FamousPker49

#### FamousPker49

 Level 18

JoinedMay 27, 2007Messages1,689

Resources: 3

#### Resources
Models 1Skins 1Tutorials 1

i tried just changing the movespeed in the object editor and it didnt work, im glad you are trying to dismantle my tutorial lol

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Does it really not? Odd. If so, just set their Move Speed when casting the spell, not when ordering them.

Also, setting movespeed with triggers can have major problems should you have movespeed-modifying buffs on the unit in question.

As for Alternate, that's exactly what Blizz does, so that definately should work ><

#### FamousPker49

#### FamousPker49

 Level 18

JoinedMay 27, 2007Messages1,689

Resources: 3

#### Resources
Models 1Skins 1Tutorials 1

yes i know my tutorial is now almost worthless lol and the alternate does work, although i havent tried it with a model that has diff anim names

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

famouspker49 said:

 yes i know my tutorial is now almost worthless lol and the alternate does work, although i havent tried it with a model that has diff anim names


Click to expand...

[self="https://www.hiveworkshop.com/forums/showthread.php?t=32295"]Welcome to the "Graveyard"[/self]


~ Thread entombed.

#### strongwill

#### strongwill

 Level 9

JoinedFeb 18, 2015Messages78

Resources: 9

#### Resources
Models 9

How to post or start my own thread? I'm trying to give Dragon Kinght an animation so he could morph from man to dragon. And I feel like I'm missing something. Anyone have an experience on morphing animation?

 You must log in or register to reply here.


### Similar threads

Locked

 Morphing Problem


Mechcash

Mar 10, 2021

World Editor Help Zone

Replies4Views1K

Mar 14, 2021
Mechcash

 Setting Model to Morphed Demon Hunter, Robo-Goblin etc..


Ralle

Jun 22, 2006

Tutorial Archive

Replies6Views4K

Mar 24, 2007
Wolverabid

Locked

 Changing abilities


AyeKantSpel

Sep 13, 2015

World Editor Help Zone

Replies2Views1K

Sep 15, 2015
Wietlol

 Unit Replacement Tutor (Very Easy)


Slaydon

Jun 3, 2008

Tutorial Archive

Replies5Views2K

Jun 4, 2008
Dagguh

A

Locked

[Trigger]
 Trigger Custom Morph Abilities


AJR3

Nov 18, 2007

Triggers & Scripts

Replies5Views3K

Nov 19, 2007
Wraithwynd

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Tutorial Archive

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top