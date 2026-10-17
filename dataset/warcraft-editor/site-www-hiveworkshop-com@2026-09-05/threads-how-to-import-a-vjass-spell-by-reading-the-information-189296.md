---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-import-a-vjass-spell-by-reading-the-information.189296/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Import a vJASS Spell by Reading the Information
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-import-a-vjass-spell-by-reading-the-information.189296/>
#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

#
 Import a vJASS spell

##
 by reading the information.

###
 Editor's Note.

Please beware that the pictures are made for another tutorial so don't get confused. It shows the same thing and no I haven't uploaded that tutorial yet.


This tutorial was made because of the great amount of people without the knowledge about JASS and not knowing what they are talking about, complaining about how to import this and all that. This tutorial is there to shut their mouths and solve this problem.

###
 Credits:

Credits goes to xBlackRose for his Electric Focus spell which I am going to use as an example.

###
 What you will learn in this tutorial:

Learn how to import a spell made in vJASS (works for ZINC too)

Using the skill of reading to get the necessary information.

Teaching you how to change certain values and how to change numbers.

###
 What I am requiring from you! :

The ability to read proper English.

Get and update JASSHelper. This tutorial will help you with that.

Common sense.

*Some* knowledge about the object and trigger editor.

No, the point above does not mean that you have to know JASS/vJASS not even GUI. Almost. Some GUI is needed.

###
 Step 1 - Converting a Trigger.


This is the first step and it's very simple. This is a test for you. If you can make through this simple step, then you probably have the knowledge that I required.

First we open the JNGP Editor- JassNewGenPack Editor.

Open up the Trigger Editor by clicking the icon or pressing F4.

Now right click on a trigger or a category and press the "New Trigger"

Now another hard step is comming, the converting part. Press Edit and look after the line "Convert to Custom Text".

This was the first step, you're making progress!


Now move on to the next step.

###
 Step 2 - Read.


Now when the hard part is done, now let's begin with the easy part.


In this tutorial I am using xBlackRose's Electric Focus v1.02

So first you open that map.

The first you will see in the trigger editor are 4 categories. 2 of them are relevant for the user. The "Systems (Required)" category and the Trigger in the Ability: Electric Focus category.

First we copy all the text in the Electric Focus trigger. Click somewhere and press CTRL + A and then CTRL + C or right click and push copy.

Next we open our own map.

Go to our trigger and presses again CTRL + A but instead we paste it all in which will replace the previous stuff with the spell code.

Now didn't we see a category with the word required next to it? Well what are we waiting for? Create 4 triggers or why not copy the whole category itself? Yes let's do that and paste it into our own map.

Now you think, hey shouldn't we copy the ability too? The answer is not yet. It will come when we arrive at the globals section.

Now the most essential part, read. So I've copied the documentation, not the whole spell code. I've will take it step by step within each of these hidden tags.

 JASS:

scope ElectricFocus initializer OnInit // requires T32, AIDS, GroupUtils, TimerUtils
//*-----------------------------------------------------------------------------------
//* __________________________________________________________________________________
//* ELECTRIC FOCUS - v1.02 (08/02/2011)
//* ¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯
//*-----------------------------------------------------------------------------------
//* Background
//* ¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯
//* ••• Entire concept credited to Builder Bob •••
//*
//* The ability was created by Builder Bob for Spell Contest 3 | Theme: Icons
//* hosted at TheHelper.net. I only recoded the ability.
//*

Alright this part told us about where the idea came from. Next!

 JASS:

//* Description
//* ¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯
//* The hero will begin focusing electricity infront of him, electrocuting any
//* enemy unit that gets in the electricity's way; dealing damage. The electricity
//* is directed towards the hero's gaze, so turning around will turn the
//* electricity. During this, attack and movement is disabled.
//*

The description about the spell. Next!

 JASS:

//* Technical Notes
//* ¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯
//* • The base ability is based on Immolation, the event is based off an
//* immediate order that checks for "immolation" and "unimmolation".
//*
//* • The movement is disabled via a stop order upon every other order. A timer
//* calls the stop 0.00 seconds after. This is why TimerUtils is needed.
//*
//* • Attack is disabled by adding Cargo Hold.
//*
//* • To check for anything standing in the way, a loop is used to group enum and
//* dest enum everything along the line. If a match is found, the lightning
//* focuses there.
//*

Now these facts are essential.


First it tells us that we shouldn't use immolation based spells when using this.


Then it tells how the movement is disabled.


How it enums. Next.

 JASS:

//* Requirements
//* ¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯
//* • JassHelper
//* • Timer32 (T32)
//* • Automatic Indexing & Data Storage (AIDS)
//* • GroupUtils
//* • TimerUtils
//*

Here again we see the required libraries but we already imported those didn't we? Next.

 JASS:

//* Changelog
//* ¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯
//* v1.02
//* • Renamed the "activateElectricFocus" method to "activate".
//* • Renamed the "deactivateElectricFocus" method to "deactivate".
//* • Shifted the "implement T32x" to the bottom of the Data struct.
//* • Shifted the "//! runtextmacro AIDS()" to the bottom of the Data struct.
//* • Removed the use of the onDestroy method in Lightning struct.
//*
//*-----------------------------------------------------------------------------------
//*
//* v1.01
//* • Changed TempUnit to whichUnit in deactivateElectricFocus method, this was
//* causing the unit to be unable to attack after it's mana depleted (TempUnit wasn't
//* set, therefore Cargo Hold ability was never removed).
//* • Added more comments to help in understanding the code.
//* • Nulled the timer variable (t) in OnOrderResponse function.
//* • Nulled the eventid variable (id) in OnOrderResponse function.
//* • Only vulnerable destructables are damaged by Electric Focus.
//* • Added a new global that allows users to modify the lightning fade duration.
//*
//* • New base damage is 40, and increments by 40 per level.
//* • New base attacktype changed from ATTACK_TYPE_MAGIC to ATTACK_TYPE_NORMAL.
//*
//*-----------------------------------------------------------------------------------
//*
//* v1.00
//* • Initial release
//*
//*-----------------------------------------------------------------------------------
//*

The changelog, for those who code it might be intresting for others probably not. Next.

 JASS:

//* Credits
//* ¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯
//* • Vexorian for:
//* • JassHelper
//* • TimerUtils
//* • Jesus4Lyf for:
//* • AIDS
//* • T32
//* • Rising_Dusk for GroupUtils
//* • Builder Bob for the original code / concept.
//*
//*-----------------------------------------------------------------------------------

Now this is an important part. You have to give credit to all these too for the required libraries and idea. But don't forget to credit BlackRose too as it's Rose's spell.

###
 Step 3 - Calibration Section


Now we've reached the part of the information which is meant for the user. The user can change it. The information is after each jass tag part within the hidden tag.

 JASS:

//*-----------------------------------------------------------------------------------
//* CALIBRATION SECTION
//*-----------------------------------------------------------------------------------
 globals
 private constant integer AID__electricFocus = 'A000' // The rawcode of the ability.

 private constant string ANIM__focusing = "channel" // What animation the caster plays when focusing?
 private constant string ANIM__onDestHit = "death" // Destructables play their death animation repeatedly when hit.

 private constant real BEAM_COLLISION = 64.00 // The size of area the lightning checks for when
 // searching for a target. A low radius will be
 // more taxing on performance, but will yield a
 // higher precision. High radius will be less
 // taxing, but may be less accurate.

 private constant string LIGHTNING__codeName = "CLPB" // Chain Lightning: Primary
 private constant real LIGHTNING__heightOffset = 45.00 // How high the lightning is from the ground.
 private constant real LIGHTNING__fadeDuration = 1.00 // How long does it take for the lightning to fade?

 // Effect that is created upon the tail of the lightning focus.
 private constant string SFX__focusTailFlash = "Abilities\\Weapons\\FarseerMissile\\FarseerMissile.mdx"

 private constant boolean STAT__blockedByDests = true // Do destructables block the lightning?
 private constant boolean STAT__damageDests = true // Does the lightning damage them?
 private constant real STAT__destDamageRatio = 0.50 // How much of regular damage is dealt to destructables?
 // If you want instant kill, set it to a really high number.

 private constant real STAT__unitSearchRadius = 64.00 // Size of area the focus will scan for, when searching units.
 private constant real STAT__destSearchRadius = 64.00 // Size of area the focus will scan for, when searching destructables.

 // Your every day attacktype/damagetype/weapontype configurables
 private constant attacktype STAT__attackType = ATTACK_TYPE_NORMAL
 private constant damagetype STAT__damageType = DAMAGE_TYPE_MAGIC
 private constant weapontype STAT__weaponType = WEAPON_TYPE_WHOKNOWS
 endglobals

Now this section is very important, so I'll take it step by step below:

private constant integer AID__electricFocus = 'A000' // The rawcode of the ability.

Now what does this mean? It means the id of the ability in the object editor. So now copy the ability into your map. When done, press CTRL + D which will show the rawcode, often something like AXXX. Put it between the ' ' marks, that is very important so 'AXXX'.

private constant string ANIM__focusing = "channel" // What animation the caster plays when focusing?

We read the comment after the // mark. It says that it plays a certain animation. Change it to your needs, maybe you want spell or attack or even move?

private constant string ANIM__onDestHit = "death" // Destructables play their death animation repeatedly when hit.

Another animation of the destructables. Maybe show "birth" animation instead of death?

private constant real BEAM_COLLISION = 64.00 // The size of area the lightning checks for when
 // searching for a target. A low radius will be
 // more taxing on performance, but will yield a
 // higher precision. High radius will be less
 // taxing, but may be less accurate.

Yeah the bb-code fucked up but anyway the radius of the beam or collision. Speaks for itself.

private constant string LIGHTNING__codeName = "CLPB" // Chain Lightning: Primary

Now this is the lightning code. A GUI user can get this code by the action create a lightning and convert it to custom text. The lightning will be 4 letters within "" marks.

private constant real LIGHTNING__heightOffset = 45.00 // How high the lightning is from the ground.

Speaks for itself.

private constant real LIGHTNING__fadeDuration = 1.00 // How long does it take for the lightning to fade?

Speaks for itself.

// Effect that is created upon the tail of the lightning focus.
 private constant string SFX__focusTailFlash = "Abilities\\Weapons\\FarseerMissile\\FarseerMissile.mdx"

The effect created at the end of the lightning. You can get the string from the GUI action create special effect. Remember that in JASS, it's double // slices between the words instead of / one in GUI.

private constant boolean STAT__blockedByDests = true // Do destructables block the lightning?

Speaks for itself.

private constant boolean STAT__damageDests = true // Does the lightning damage them?

Speaks for itself.

private constant real STAT__destDamageRatio = 0.50 // How much of regular damage is dealt to destructables?
 // If you want instant kill, set it to a really high number.

Now this is important to read. If you want to kill it, set it into a really high number. Now in GUI you usally only have kill or not kill. This has the option of damaging for a certain amount. Based of the normal spell damage, just multiplied with this ratio.

private constant real STAT__unitSearchRadius = 64.00 // Size of area the focus will scan for, when searching units.

Speaks for itself.

private constant real STAT__destSearchRadius = 64.00 // Size of area the focus will scan for, when searching destructables.

Speaks for itself.

 JASS:

 // Your every day attacktype/damagetype/weapontype configurables
 private constant attacktype STAT__attackType = ATTACK_TYPE_NORMAL
 private constant damagetype STAT__damageType = DAMAGE_TYPE_MAGIC
 private constant weapontype STAT__weaponType = WEAPON_TYPE_WHOKNOWS

These are so important that I have to have them all at once. Basic the types. ATTACK_TYPE_NORMAL and DAMAGE_TYPE_MAGIC is for spell damage while the WEAPON_TYPE_WHOKNOWS is for the sound. There are plenty to choose from but I'd leaved it as this.


Next.

 JASS:

 //*
 //* STAT: Damage
 //* ¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯
 //* Level 1: 80 DPS
 //* Level 2: 120 DPS
 //* Level 3: 160 DPS
 //* Level 4: 200 DPS
 //*
 private constant function STAT__GetDamage takes integer level returns real
 return (40.00 * level) + 40.00
 endfunction

Here we set the damage of the function. It's not hard at all yet it's important to know what you are doing.


Basic you have the level of the ability for the casting hero. So if you want to change something, change it after the return.


You can change it to: return (20 * level / level) + 15 * 123 * 0.01 * level


But that's all up to you, I wouldn't touch it.

 JASS:

 //*
 //* STAT: Maximum Distance
 //* ¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯
 //* Level 1: 700
 //* Level 2: 800
 //* Level 3: 900
 //* Level 4: 1000
 //*
 private constant function STAT__GetDistance takes integer level returns real
 return (100.00 * level) + 600.00
 endfunction

So this is the maximum distance, speaks for itself?


That was probably the end but there are a unit filter too which we will look at.

 JASS:

 //*
 //* FILTER: Unit Hit
 //* ¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯¯
 //* • Living
 //* • Within height
 //* • Is enemy
 //*
 private function FILTER__UnitHitCondition takes nothing returns boolean
 set TempUnit = GetFilterUnit()
 return IsUnitType( TempUnit, UNIT_TYPE_DEAD ) == false and /* Living
 */ GetUnitFlyHeight( TempUnit ) <= LIGHTNING__heightOffset and /* Within height
 */ IsUnitEnemy( TempUnit, TempData.owningPlayer ) // Enemy
 endfunction

I'll just explain this, not how you can change it.


First the spell checks if the unit is dead.


Second if the unit is within the heightoffset which is a global constant.


Third it checks if the unit is an enemy.

###
 Step 4 - Last check


The last final step, recheck. Have we forgot something? Yes we have, in the object editor there are 2 buffs which are used by the ability. Import these and change the buffs in the ability.


Now test the spell, if you managed to get this far. It should work.


If not, contact the author of the spell. This was made out of a user-made spell from the hive spell section. Please beware that there can be alot of configurations in a few of the vJASS spells out there or they are as this one, simple and easy to import and configure


Have a nice day and thanks for reading my tutorial.


Yours faithfully.


~baassee


 Last edited: Feb 9, 2011

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

This post is reserved for the future and is a mark that I have to remake the whole tutorial if xBlackRose doesn't accept that I am using the spell as an example. Waiting currently for permission.

EDIT:

Gained xBlackRose's permission.


 xBlackRose said:



 I wouldn't mind you using it anyways. Thanks for considering me in this and letting me know.


Click to expand...


 Last edited: Feb 10, 2011

#### ap0calypse

#### ap0calypse

 Level 28

JoinedJan 26, 2007Messages4,787

Resources: 5

#### Resources
Maps 3Spells and Systems 1Tutorials 1


 baassee said:



 If you can make thru this simple step,


Click to expand...

through*

You could use a list-tag for Step 2.

Also, in hidden-tags: use [size=2]...[/size], that will make it more readable.

For the rest: great tutorial, could be extremely useful.

When someone is having trouble we this again we can just link this page instead of saying the entire story again

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

Thru is just american english although not so formal

list tag for step 2? ill fix

where shall the size be, around the text in the hidden tag (not the jass)?

#### ap0calypse

#### ap0calypse

 Level 28

JoinedJan 26, 2007Messages4,787

Resources: 5

#### Resources
Maps 3Spells and Systems 1Tutorials 1

JASS is readable in hidden-tags, so perhaps everything but the JASS-tags ^^

And yes, it's American English, but I really prefer through  (for tuts at least).

Again: great tutorial, might help many ^^

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

ap0calypse said:

 JASS is readable in hidden-tags, so perhaps everything but the JASS-tags ^^


Click to expand...


Fixed. Thanks a lot!

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Good. A much needed tutorial. ~Approved.

#### aider

#### aider

 Level 8

JoinedJul 14, 2010Messages235

Resources: 2

#### Resources
Maps 2

Thanks a lot for this tutorial! But a simple question, what's the difference between Jass and vJass? It's the same method when importing spells? You can have both Jass and vJass in one map right?

EDIT: Thank you ap0calypse, for the good explanations!


 Last edited: Mar 3, 2011

#### ap0calypse

#### ap0calypse

 Level 28

JoinedJan 26, 2007Messages4,787

Resources: 5

#### Resources
Maps 3Spells and Systems 1Tutorials 1

aider said:

 Thanks a lot for this tutorial! But a simple question, what's the difference between Jass and vJass? It's the same method when importing spells? You can have both Jass and vJass in one map right?  Thanks!


Click to expand...

There are 2 explanations: the shorter (rather incorrect) one, and the longer (correct) one.

Short version:


vJass is an extension of JASS. It uses new functions to create difficult systems more easily.


Because of this, you need the JNGP editor, because the regular world editor can't read these new scripts and a lot of errors will pop up.

Longer version:


If you're wondering why the editor can't read it yet warcraft can, then you've stumbled across an interesting question.


The brilliant part of vJass is that it does NOT add anything new (yes, contradiction of what I have said before).


It uses functions of regular JASS to create 'new' ones, these 'new' functions can be translated to regular JASS by using the JNGP's compiler.


Once the code is compiled, the map will be in regular JASS and vJass scripts do not exist (until you re-open the map in the editor again).


At least, that's why I find it brilliant. You do NOT need any extension to the game, yet it does make the life of a coder a lot more easier


And yes: you can use both JASS and vJass in the same map, for the same reason as the long version: because once compiled, vJass is converted to regular JASS.


"The same method to import spells" - well, kind-of: yes.


A few differences are that it's harder to set up all variables (if you don't know anything about JASS) and you don't need JNGP when importing a JASS spell/system.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

Huhuhh , no matter how good this tutorial is, my WE (or maybe JNGP) is fucked up and most of my spells are getting errors when I tried to save them...Im using 1.24e but JNGP still 1.21...

 And yes: you can use both JASS and vJass in the same map, for the same reason as the long version: because once compiled, vJass is converted to regular JASS.


Click to expand...

That's if you're using JNGP...I cant even save a map containing vJASS codes in normal WE...

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

JNGP should be 1.21. The JNGP version doesn't matter in this case, it's the version of the JASSHelper that compiles the code.

And no you cannot save vJASS in the normal WE because it requires JASSHelper to compile the vJASS/ZINC into JASS. That's the solution to your problem sir.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

JNGP is fully compatible with the newer versions of wc3... but you need to update your jass helper because the one that comes with the last JNGP, was outdated...

the only thing that is not fully supported by NewGen is the GUI hashtables...

#### ShadyGuy

S

#### ShadyGuy

any ideas why im only getting "private outside library/scope definition" errors?

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

That is that you used the keyword private outside a scope or a library. Check your code. You may have copied it wrong.

 You must log in or register to reply here.


### Similar threads

 vJASS Spell Templates


Magtheridon96

May 13, 2012

JASS Tutorials

Replies32Views19K

Mar 29, 2016
PurgeandFire

 [vJASS] Dynamic Indexing Tutorial


Malhorne

Dec 25, 2013

JASS Tutorials
2

Replies58Views16K

Jul 13, 2015
Zwiebelchen

 Efficient Spell Cloning


AGD

Mar 20, 2017

JASS Tutorials

Replies3Views6K

Jun 19, 2018
PurgeandFire

Locked

 [vJASS] The Glaive is not Showing


doomhammer99

Dec 7, 2012

Triggers & Scripts

Replies4Views1K

Dec 9, 2012
ap0calypse

Locked

[General]
 some questions


TheAncient

Nov 22, 2011

World Editor Help Zone

Replies6Views1K

Nov 30, 2011
Maker

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

JASS Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top