---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/passive-ability-with-a-cooldown-attack-based.211589/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Passive Ability with a Cooldown Attack Based
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/passive-ability-with-a-cooldown-attack-based.211589/>
#### Arisato

#### Arisato

 Level 18

JoinedDec 17, 2009Messages1,114

Resources: 25

#### Resources
Models 10Icons 1Maps 8Spells and Systems 1Tutorials 5

 Tutorial


Passive Abilities With Cooldown

 Tutorial


Introduction

I seen many people posted some threads about it and most people who knew how to make a passive ability with a cooldown wouldn't answer it, so here are the tricks to do this yourself (example:Geminate Attack from DotA!(Well IceFrog's spell is triggered too, but making the passive cooldown is the same like the one provided in this tutorial))

Well the trick is actually simple, NO TRIGGERING SKILLS NEEDED!

Yes i mean it, even if you have no actual knowledge about triggering, and you only know HOW to use Object Editor etc etc, you can make it.


 Start Here


The Actions

So lets get started

What will you be needing:

-World Edit

-Knowledge about the Object Editor

-A Brain

First of all , create an ability which you want it to be cast passively , doesn't matter if the ability is an active ability, i will use chain lightning as my passive ability,and i will name the ability "Twin Lightning", doesn't matter.

Here's the image:

Now create an ability based off the Orb of Lightning.

Here's the image:

Now here's the deal

Set the "Data - Effect Ability" in the ability you've just made to Twin Lightning or what name/ability you want, you can set the chance too, it's all yours

I guess that covers it all, let me show you a gameplay screenshot if you don't believe me

 Notes


Notes:

The icon will appear to be darker than the actual icon. To fix this, you should import a custom version of the icon and make the regular icon have a path prefix of "DISPAS" and make the actual DISPAS have a path prefix of "PAS". For example, if you have a custom version of an icon named "MyPassiveAbility.blp", then you would make the normal PASBTN have a path of:

ReplaceableTextures\CommandButtonsDisabled\DISPASMyPassiveAbility.blp

And make the other disabled version have a path of:

ReplaceableTextures\CommandButtons\PASMyPassiveAbility.blp

Then in the object editor, assign it to the icon with the path:

ReplaceableTextures\CommandButtons\PASMyPassiveAbility.blp

Note that this may appear as green when the game is paused. (I'm not positive about this, but I assume that would happen) However, the problem where it is darkened will be fixed.

Also note that this technique as a whole is subject to whatever bugs/quirks the orb abilities already have.

 Test Map


Click Here

#### Attachments

example.png

 80.8 KB

 · Views: 8,198



example2.png

 22 KB

 · Views: 7,900



example3.png

 51.3 KB

 · Views: 8,258



twin lightning in cooldown.png

 947.3 KB

 · Views: 9,895



twin lightning in effect.png

 580.8 KB

 · Views: 8,782



Twin Lightning lol.png

 629.5 KB

 · Views: 9,034



Passive Testmap.w3x

 47.2 KB

 · Views: 1,909




 Last edited: Jun 28, 2012

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Images in the

 tags are not showing up for me. Make sure they are attached to the hiveworkshop and not an external image hosting website.

#### watermelon_1234

#### watermelon_1234

 Level 14

JoinedNov 18, 2007Messages1,084

Resources: 10

#### Resources
Spells and Systems 9JASS 1

The reason images aren't showing is because he didn't put any image link there.

#### Arisato

#### Arisato

 Level 18

JoinedDec 17, 2009Messages1,114

Resources: 25

#### Resources
Models 10Icons 1Maps 8Spells and Systems 1Tutorials 5

Lol, it works for me?

Updated, image should appear


 Last edited: Sep 30, 2016

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

There are many more types of passive abilities than the crappy onAttack orb.

#### Arisato

#### Arisato

 Level 18

JoinedDec 17, 2009Messages1,114

Resources: 25

#### Resources
Models 10Icons 1Maps 8Spells and Systems 1Tutorials 5

What does that have to do with the tutorial?

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

The title does not fit. I was hoping for a tutorial how to generally make passive abilities with cooldown.

#### fakedrake

#### fakedrake

 Level 3

JoinedApr 5, 2010Messages31

Resources: 1

#### Resources
Maps 1

The reason that icon gets darker when you learn it is that it automatically becomes a DISBTN type icon. So use a brighter DISBTN can solve the problem, but then when you open the menu then everything goes dark except this icon XD, which is kinda annoying

#### Arisato

#### Arisato

 Level 18

JoinedDec 17, 2009Messages1,114

Resources: 25

#### Resources
Models 10Icons 1Maps 8Spells and Systems 1Tutorials 5

WaterKnight said:

 The title does not fit. I was hoping for a tutorial how to generally make passive abilities with cooldown.


Click to expand...

You can set the ability to be a channel ability , and trigger what it would do, it will still have the cooldown, this is flexible if you don't know.

And Orb effects destroy this skill, missile modifiers don't seem to destroy this ability, (tested a poison arrow + this)

@fakedrake, you can change the BTN into brighter and leave the DISBTN dark, i think that's how i solved it? or vice versa?i can't seem to remember

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Arisato said:

 You can set the ability to be a channel ability , and trigger what it would do, it will still have the cooldown, this is flexible if you don't know.


Click to expand...


And it will still go only onAttack. But passives can also fire on other ocassions such as a nearby unit dies, the unit gets healed, receives damage/drops below life, casts another spell/uses an item, kills a unit, idles, ...


The problem is that you cannot trigger the ability instantly via trigger, not immediately perform an attack against some dummy unit or get to cooldown otherwise. At least, I do not know of such a way and was hoping to get some clue as I was reading the thread title.


Btw, orb abilities have another bug. If a unit attacks automatically out of a nearby enemy aggravating, not because you ordered it, it won't have an order and orb effects won't trigger then.

#### Arisato

#### Arisato

 Level 18

JoinedDec 17, 2009Messages1,114

Resources: 25

#### Resources
Models 10Icons 1Maps 8Spells and Systems 1Tutorials 5

then you can trigger it if a unit is attacked, then order it to attack, but check if the unit has an ability that needs to be ordered to trigger the passive.

#### Remixer

#### Remixer

Map Reviewer

 Level 34

JoinedFeb 19, 2011Messages2,130

Resources: 85

#### Resources
Models 31Icons 7Packs 1Skins 5Maps 39Templates 1Tutorials 1

Maybe you could use "custom" icon and use "wrong" paths so the DISBTN icon looks like BTN version? Could work.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

it should be entitled, "Making an onAttack passive ability with cooldown"

#### Arisato

#### Arisato

 Level 18

JoinedDec 17, 2009Messages1,114

Resources: 25

#### Resources
Models 10Icons 1Maps 8Spells and Systems 1Tutorials 5

Remixer said:

 Maybe you could use "custom" icon and use "wrong" paths so the DISBTN icon looks like BTN version? Could work.


Click to expand...

Adiktuz said:

 it should be entitled, "Making an onAttack passive ability with cooldown"


Click to expand...

@Yes, it could work.

@Adiktuz That's kinda too long.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

but that's the best title for this cause just passive ability with cooldown is misleading... XD...

#### HammerFist

#### HammerFist

 Level 21

JoinedMar 22, 2012Messages1,549

Resources: 33

#### Resources
Models 18Icons 8Skins 1Maps 2Spells and Systems 4

Man I have problem i have world edit and knowledge but i have no brain can someone help me xD

#### Arisato

#### Arisato

 Level 18

JoinedDec 17, 2009Messages1,114

Resources: 25

#### Resources
Models 10Icons 1Maps 8Spells and Systems 1Tutorials 5

dafuq did i just read

#### eubz

#### eubz

 Level 42

JoinedMar 29, 2011Messages3,754

Resources: 163

#### Resources
Models 77Icons 25Packs 1Skins 1Tools 1Maps 43Spells and Systems 7Tutorials 8

Is there a need for passive abilities to cooldown?

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Sure, especially to require skill and not just having permanently boosted stats or random and unreliable proc-chance.

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

eubz said:

 Is there a need for passive abilities to cooldown?


Click to expand...

You obviously does not play many types of custom map just yet (especially AoS-type which include this special features and add unique gameplay to the map)

On Thread

When I first enter this thread, I was hoping that when my unit walks about 500 range away, it will trigger this ability itself and has its cooldown (judging by the thread topic name...)

But, it's truly misled me as this only occurs when it's onAttack by referring to WaterKnight

Can you please change the title as to avoid further confusion for newcomers

Something like: Making a passive ability with a cooldown (attack-based) ?

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

@eubz: Once of the popular maps that has it is DotA, Nerubian weaver's Geminate attack uses this kind of mechanic...

@OP: And again and again, we want you to rename this thread please...

#### sonofjay

#### sonofjay

 Level 28

JoinedOct 28, 2011Messages4,750

Resources: 27

#### Resources
Packs 1Maps 20Templates 5Tutorials 1

very useful

#### Arisato

#### Arisato

 Level 18

JoinedDec 17, 2009Messages1,114

Resources: 25

#### Resources
Models 10Icons 1Maps 8Spells and Systems 1Tutorials 5

Making a passive ability with a cooldown (attack-based)

That's better

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Sorry for the lateness to review. I have checked this out and it seems like a workable technique. I changed the title to "Passive ability with a cooldown (attack-based)", hopefully that is okay. Usually you don't have to have the "Making a..." part since it is implied. (since the thread is a tutorial)

However, you should make a note at the end, preferably looking something like this:

##
 Notes:

The icon will appear to be darker than the actual icon. To fix this, you should import a custom version of the icon and make the regular icon have a path prefix of "DISPAS" and make the actual DISPAS have a path prefix of "PAS". For example, if you have a custom version of an icon named "MyPassiveAbility.blp", then you would make the normal PASBTN have a path of:

ReplaceableTextures\CommandButtonsDisabled\DISPASMyPassiveAbility.blp

And make the other disabled version have a path of:

ReplaceableTextures\CommandButtons\PASMyPassiveAbility.blp

Then in the object editor, assign it to the icon with the path:

ReplaceableTextures\CommandButtons\PASMyPassiveAbility.blp

Note that this may appear as green when the game is paused. (I'm not positive about this, but I assume that would happen) However, the problem where it is darkened will be fixed.

Also note that this technique as a whole is subject to whatever bugs/quirks the orb abilities already have.


Click to expand...

You can actually just use that text directly if you want, and I'll approve.

#### Arisato

#### Arisato

 Level 18

JoinedDec 17, 2009Messages1,114

Resources: 25

#### Resources
Models 10Icons 1Maps 8Spells and Systems 1Tutorials 5

Done : )

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Approved.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

orbs doesnt have cooldowns coz it triggers on each chance, so the title is misleading,


a real passive cooldown however is triggered with timers, but this is very useful indeed...

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

 orbs doesnt have cooldowns coz it triggers on each chance


Click to expand...

Data - Chance to Hit Heros (%): 100.00

Data - Chance to Hit Summons (%): 100.00

Data - Chance to Hit Units (%): 100.00

Or I just misunderstood you ?

Well, it is called as cooldown because it will trigger each time you ordered an attack.

Just like spell, when you order it to cast, it will fire but can't be re-fired when it is in cooldown, same as for the orb.

For first ordered attack, it will trigger it, but for the next (while on cooldown), it will trigger.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

 Well, it is called as cooldown because it will trigger each time you ordered an attack.


Click to expand...

it will trigger on each chance, chance it triggered by attack ofc...

the cooldown is from the 'EffectAbility' (chain lightning), not the orb itself...

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

So why do you said that the title is misleading, at first (yes it was) but then again, it was improved to (Attack-based), meaning that this cooldown will proc via Attack (ordered), it seems that it leads us to the right place.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

coz chainlightning/firebolt/frost nova, etc...is an active ability...orb is passive but it


doesnt have cooldowns, like the "Data - Damage Bonus" and the 'chances'...


also I may add that you dont need to impost a 'PAS' (passive) icon for the 'Data - Effect Ability'


coz it will be automatically be passive once implemented...

#### Arisato

#### Arisato

 Level 18

JoinedDec 17, 2009Messages1,114

Resources: 25

#### Resources
Models 10Icons 1Maps 8Spells and Systems 1Tutorials 5

Actually when the skill is on cooldown it's not orb affected.

#### ukn0wnD3str0y3r

#### ukn0wnD3str0y3r

 Level 9

JoinedFeb 15, 2013Messages369

Resources: 8

#### Resources
Maps 2Spells and Systems 6

Thanks for this tutorial, but i'm failing at every level it dont upgrade...


Level 1 - 10 seconds cooldown


Level 2 - 8 seconds cooldown


Level 3 - 6 seconds cooldown


Level 4 - 4 seconds cooldown


But it didn't upgrade every level o.o it only stays at Level 1 - 10 seconds cooldown

#### MantisHair

#### MantisHair

 Level 4

JoinedJun 30, 2014Messages72

Resources: 1

#### Resources
Tutorials 1

This fix does require triggering a little.

For all the people who are complaining that it doesn't fire when the unit starts a fight without an attack order, here's a solution.

A trigger like so:

Autotarget attack
Events

Conditions

Actions
Unit - Order *A unit variable of some kind, for the unit who has this ability* to Attack (Targeted unit)

When the unit is from a tavern (alter this for your Hero pick system), connect the attack trigger to something like this:

Set Autoattack
Events
Unit - A unit Sells a unit

Conditions
(Unit-type of (Sold unit)) Equal to *insert your unit*

Actions
Set *the variable for your unit* = (Sold unit)

Trigger - Add to Autotarget attack (*the first trigger*) <gen> the event (Unit - *the variable for your unit, again* Acquires a target)

If multiple units have the ability, set the *variable for your unit* as an array, but beware that having 20 (or lots in general) units using this trigger system lags badly. Lag causes FRUSTRATION!


 Last edited: Apr 6, 2015

#### SayazEmil

#### SayazEmil

 Level 4

JoinedDec 3, 2014Messages65

Cool Spell Amigo. Is there a possibilty to add that the spelldamage is based on strength for expample?

#### LordPerenolde II

#### LordPerenolde II

 Level 23

JoinedNov 20, 2005Messages1,295

Resources: 5

#### Resources
Models 1Maps 4

Thank you for this! It works so nice! Too bad for the icon glitch, but that may be sorted out somehow.


 Last edited: Aug 26, 2015

#### LordPerenolde II

#### LordPerenolde II

 Level 23

JoinedNov 20, 2005Messages1,295

Resources: 5

#### Resources
Models 1Maps 4

Is it possible to also increase the attack range with the same orb ability?

#### Spacebuns

#### Spacebuns

 Level 23

JoinedApr 20, 2010Messages943

Resources: 25

#### Resources
Models 5Skins 1Maps 2Spells and Systems 1Reforged HD Models 5Ported Models 4Sounds 1Tutorials 6

Attack range can only be changed through upgrades, so no.

#### Silva

#### Silva

 Level 6

JoinedFeb 6, 2015Messages266

Worked. Although, there are things in the object editor I didn't understand.


Would have hoped if this tutorial had more details about that.

#### Rubellu Sidus

#### Rubellu Sidus

 Level 21

JoinedJan 14, 2014Messages570

Resources: 39

#### Resources
Models 32Icons 5Maps 2

##
 The Resistance of Aethernia


Well, I wanted to make the "Tidebringer" ability from DoTA using this tutorial, but with Passive Abilities its not working, it don't have cooldown. Cooldown only with Active Spells.

#### Daliox1

#### Daliox1

 Level 7

JoinedSep 3, 2020Messages197

Very usefull, thank you

#### SenPie69

#### SenPie69

 Level 9

JoinedMar 26, 2020Messages220

Resources: 1

#### Resources
Maps 1

been looking for this last year and all i get is triggers, and finally after a year i saw this AND WITHOUT a trigger one bit~

thank you good sir! i wish i could upvote this three times lol

#### Alyssa-Quinxzel

#### Alyssa-Quinxzel

 Level 2

JoinedApr 14, 2024Messages4

Followed the instructions thoroughly and works perfect for me. Thanks!

 You must log in or register to reply here.


### Similar threads

 Passive ability with cooldown - Best method!


leandrotp

Jan 27, 2016

Misc/General Mapping Tutorials

Replies34Views24K

Nov 23, 2024
Bender

Locked

 Passive with cooldown.


CreatorD3292

Jan 9, 2014

World Editor Help Zone

Replies10Views3K

Jan 13, 2014
Adiktuz

Locked

 passive skill with cooldown


paskovich

Jul 2, 2006

Triggers & Scripts

Replies8Views2K

Jul 9, 2006
Ramza

Locked

 Passive ability with a cooldown


Maximilianx

Mar 25, 2009

World Editor Help Zone

Replies20Views7K

Mar 27, 2009
Interceptor

Locked

 Passive Icons (BTN->PASS)


Ninja_Duels

Jan 8, 2010

Requests

Replies6Views2K

Jan 10, 2010
Ninja_Duels

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