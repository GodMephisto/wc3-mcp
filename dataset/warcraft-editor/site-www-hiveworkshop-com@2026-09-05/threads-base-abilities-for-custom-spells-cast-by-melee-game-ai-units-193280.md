---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/base-abilities-for-custom-spells-cast-by-melee-game-ai-units.193280/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Base Abilities for Custom Spells Cast by Melee Game AI Units
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/base-abilities-for-custom-spells-cast-by-melee-game-ai-units.193280/>
#### Boris_Spider

#### Boris_Spider

 Level 14

JoinedDec 12, 2009Messages1,027

Resources: 7

#### Resources
Icons 4Maps 2Tutorials 1

Base Abilities for Custom Spells used by AI Casters

For getting the AI to cast custom spells, appropriate base spells must be used. Many users will argue that all custom spells should be based off of Channel-Special. While this is recommended for human users, the AI will never cast spells based on Channel-Special. It's also true that you can trigger the AI casting. This makes sense for Boss Fights in arena or RPG style formats, but it's not as useful for melee-like AI. It makes no sense to have additional code to try and determine when, where, and how randomly the AI should cast it's spells when there's already code 'running in the background' to do this. There is some information throughout the internet on this matter, but it's a pain to find and very spotty at best. So I decided to assemble this list to help make life easier for any searching for this information.

This tutorial/list is intended to provide a list of abilities AI casters can use and when they'll use them. The regular melee game will use all abilities on the standard Blizzard maps (except for transform abilities like Destroyer Form/Bear Form/Crow Form). This tutorial focuses on custom maps with custom units casting custom abilities when desired via using ideal base abilities.

 -=|Base spells and when the AI decides to cast them|=-

~Acid Bomb -

~Aerial Shackles - Spammed immediately if allies are nearby. Casts if caster is under attack.

~Anti-magic Shell -

~Banish - Prefers casting on summoned units.

~Bladestorm - Used at random, appears to use immediately if many enemies nearby.

~Black Arrow & Parasite - The AI normally uses it when the target is dying, perhaps to spawn the unit faster giving that the target is almost dead. Parasite is always cast on the furthest target inside the range.

~Berserk - Used in combat, will not cast if outnumbered.

~Big Bad Voodoo -

~Blizzard/Rain of Fire - Will cast if there are at least 2 to 3 units in a group. Monsoon however never seems to cast.

~Bloodlust -

~Cannibalize - Spammed when a unit is injured, usually around 50% HP or less.

~Chain Lightning - Spammed at every cool down, appears to ignore stunned/disabled enemies.

~Charm - Spammed on anything that can be targeted; will not target things that can't attack (Though may target workers making buildings that attack)

~Cloud - Spams any defensive building (ranged and/or melee), usually when attacking towns

~Cold Arrows - PREFERRED over Searing Arrows in the case the hero/unit has both spells.

~Cripple -

~Curse - Spammed on engaged enemy units

~Dark Portal (Archimonde) - Uses whenever is available.

~Death Pact - Never

~Devour - Spammed on anything that can be targeted, may prefer units with higher levels.

~Defend - Activated in the same manner as Divine Shield, but only if the caster is being attacked by a unit with Piercing damage or Magic damage (If ability is set to shield against Magic damage as well).

~Dispel -

~Divine Shield - Casts when attacked, often just before the caster is dealt damage or hit by a non-friendly spell to avoid taking any damage. The health of the unit isn't a factor for it's casting.

~Drunken Haze -

~Finger of Death - Uses whenever is available

~Fan of Knives - Spams randomly if 2 to 3 enemies around, occasionally also uses on single targets if attacked.

~Faerie Fire - Spammed randomly on enemy units

~Farsight - Unused

~Fire Breath/Shockwave/Carrion Swarm/Impale - Will cast if there are at least 2 to 3 units in a cone.

~Flamestrike - May be cast if there are 2+ enemies standing in the AoE

~Forked Lightning - Spammed on enemy heroes and/or clusters of 2+ enemy units.

~Frost Nova - Spammed at every cool down

~Healing Wave/Heal/Rejuvenation - Used when caster or any nearby allies take significant/any/moderate damage.

~Holy Light/Death Coil - Spam on any nearby Undead units or friendly non-Undead units, prefers heroes. /Death Coil is same function as Holy Light but with opposite targets. It's been noticed while doing some Tank/Spank boss fights that if the hero is taken a LOT more damage then the heal, the healer or himself(if he has this spell) he STOPS casting it

~Hex - Spammed on non-heroes, may target a lone hero

~Immolation - Activates only if Immolation deals damage and there are viable targets present; it may favor use after losing hitpoints

~Inner Fire - Spammed randomly? May be a preference for high level units (e.g. Frost Wyrm or Tauren)

~Invisibility - Occasionally casts on retreating friendly heroes with moderate to low health

~Life Drain - Spammed at every cool down.

~Locust Swarm / Voodoo Spirits -

~Mana Burn - Spammed on anything with mana

~Mana Drain -

~Mana Flare -

~Mirror Image - Cast if the caster is attacked a certain number of times. Used immediately if the caster is about to be hit by a spell.

~Parasite - Always casts on the furthest target inside the range.

~Phase Shift -

~Purge -

~Raise Dead - Spammed at every cool down if nearby corpses are available; but only in combat

~Resurrection - Cast if there are friendly dead units nearby.

~Roar/Howl of Terror - Used when engaging enemies

~Searing Arrows - Uses it whenever the CD is off.

~Serpent Ward - Plants near enemies every cool down.

~Silence - Appears to spam on anything that has mana. [May target all units if it reduces the damage dealt of those targeted - needs confirmation.] Preference for units with at least 75 mana as well as player units and non-heroes over heroes.

~Sleep - Cast while in combat, seems to prefer non heroes.

~Slow - Spammed randomly? May be a preference for high level units (e.g. Frost Wyrm or Tauren)

~Soul Burn - Only activates on heroes.

~Shadow Strike - Spammed at every cool down unless victim is already afflicted with Shadow Strike (may prefer target that has less then 100% of his health. Never studied to determine the threshold, but a user has never seen it used on a 100% target)

~Spirit-Link - Casts on friendly units that are engaged

~Stasis Trap -

~Storm Bolt/Fire Bolt/Frost Bolt - Spammed at every cool down, has preference to target heroes.

~Starfall - Uses when there is 2 or more enemy units close.

~Thunderclap/Warstomp - Will cast if there are 2 to 3 units around the caster. May not be used if air units are present (despite being allowed to hit them to).

~Unholy Frenzy -

~Vengeance -

~Water Elemental - Casts when caster or nearby allies are engaging enemies.

~Wind Walk - Used when the AI-controlled units are near death

 -=|Important things to note|=-

~When the AI casts customs spells, it may rapidly cast them and cause the SFX to display incorrectly - if at all. Some spells with lightning effects don't display properly, what causes this is still unknown.

~The AI only casts Forked Lightning/Chain Lightning if the damage dealt is greater than 0. When using this as a base spell set the damage dealt to 0.01 to ensure use. If it's set to 0, the AI will NOT cast it. This also applies to Immolation.

~Shockwave may lag due to its terrain deformation, use Carrionswarm or Breath of Fire instead.

If abilities aren't mentioned it's because none of the contributors have submitted the casting trends of the AI. Please submit your experiences working with custom AI base abilities. The more information I have the more useful others may find this tutorial.

Future Plans:

~See if AI has preference for casting certain spells over others. (e.g. I've noticed that the AI will spam Charm before it casts Forked Lightning)

~Determine if setting a damage spell's damage to zero 'prevents' casting (Confirmed with Forked Lightning and Chain Lightning).

Test Map

Attached is the current AI test map. Open it up in-game and watch which spells are used. There are a few MUI problems with the custom spells I'm using, but they don't interfere with the map. You can control red's units to see which spells are where and get an idea of what you can do. I'll be adding additional base spells (and IDing them) as I have time.

~Lightning Strike is based off of Blizzard

~Thunderwrath is based off of CarrionSwarm

~Lightning Speed is based off of Silence

Contributors:

~Boris_Spider

~kino

~mckill2009

~myronbeg

~Shyrony

~gjpaixao13

~Teuncreemers

~watermelon_1234

~Sephalo

~ngkn92

~Dark Axl

Last Updated: 8/16/2014

Related Tutorial:

https://www.hiveworkshop.com/forums/general-mapping-tutorials-278/ai-editor-162687/ by VeljkoM

#### Attachments

AI_Test_Map.w3x

 120 KB

 · Views: 2,302




 Last edited: Sep 27, 2014

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

This stuff is solid information. I like it.


However:

 ~Fire Breath/Shockwave/Carrion Swarm/Impale - Will cast if there are at least 2 to 3 units in a cone.


Click to expand...


I've had breath of fire casted when I was fighting 1 unit vs 1 unit, against a computer player. Maybe my memory is off, but I think you might want to check that out.

#### Boris_Spider

#### Boris_Spider

 Level 14

JoinedDec 12, 2009Messages1,027

Resources: 7

#### Resources
Icons 4Maps 2Tutorials 1

PurgeandFire111 said:

 I've had breath of fire casted when I was fighting 1 unit vs 1 unit, against a computer player. Maybe my memory is off, but I think you might want to check that out.


Click to expand...


Thanks, will check it out soon.


//\\oo//\\

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

 the AI will never cast spells base on Channel-Special.


Click to expand...

That's true, channel base type ability for AI is useless since they will not cast it except when you change the base order ID and order string, I use this before but now no more...

I'd also like to add that the ORDER of casting a custom spell by the AI dummy is this...

Untitled Trigger 001
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Storm Bolt

Actions
Set pt = (Position of (Triggering unit))

Unit - Create 1 Dummy for (Triggering player) at pt facing Default building facing degrees

Unit - Add a 1.00 second Generic expiration timer to (Last created unit)

Unit - Add Chain Lightning (Neutral Hostile) to (Last created unit)

Unit - Set level of Chain Lightning (Neutral Hostile) for (Last created unit) to (Level of Chain Lightning for (Triggering unit))

Unit - Order (Last created unit) to Orc Far Seer - Chain Lightning (Target unit of ability being cast)

Custom script: call RemoveLocation(udg_pt)

Not this...

Untitled Trigger 001
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Storm Bolt

Actions
Set pt = (Position of (Triggering unit))

Unit - Create 1 Dummy for (Triggering player) at pt facing Default building facing degrees

Unit - Add Chain Lightning (Neutral Hostile) to (Last created unit)

Unit - Set level of Chain Lightning (Neutral Hostile) for (Last created unit) to (Level of Chain Lightning for (Triggering unit))

Unit - Order (Last created unit) to Orc Far Seer - Chain Lightning (Target unit of ability being cast)

Unit - Add a 1.00 second Generic expiration timer to (Last created unit)

Custom script: call RemoveLocation(udg_pt)

In all, these are useful information +rep...

#### Kino

#### Kino

 Level 24

JoinedOct 12, 2008Messages1,783

Resources: 22

#### Resources
Models 5Maps 17

I can confirm that Flamestrike is used if there at least 2 enemies standing within AOE.


Also, nice to see you finally finished it.

#### VeljkoM

#### VeljkoM

 Level 21

JoinedJul 27, 2008Messages14,361

Resources: 4

#### Resources
Maps 3Tutorials 1

I linked this to my AI Editor Tutorial if that is ok. I have given you credits.

#### Boris_Spider

#### Boris_Spider

 Level 14

JoinedDec 12, 2009Messages1,027

Resources: 7

#### Resources
Icons 4Maps 2Tutorials 1

mckill2009 said:

 That's true, channel base type ability for AI is useless since they will not cast it except when you change the base order ID and order string, I use this before but now no more...


I'd also like to add that the ORDER of casting a custom spell by the AI dummy is this...

Untitled Trigger 001
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Storm Bolt

Actions
Set pt = (Position of (Triggering unit))

Unit - Create 1 Dummy for (Triggering player) at pt facing Default building facing degrees

Unit - Add a 1.00 second Generic expiration timer to (Last created unit)

Unit - Add Chain Lightning (Neutral Hostile) to (Last created unit)

Unit - Set level of Chain Lightning (Neutral Hostile) for (Last created unit) to (Level of Chain Lightning for (Triggering unit))

Unit - Order (Last created unit) to Orc Far Seer - Chain Lightning (Target unit of ability being cast)

Custom script: call RemoveLocation(udg_pt)


Not this...

Untitled Trigger 001
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Storm Bolt

Actions
Set pt = (Position of (Triggering unit))

Unit - Create 1 Dummy for (Triggering player) at pt facing Default building facing degrees

Unit - Add Chain Lightning (Neutral Hostile) to (Last created unit)

Unit - Set level of Chain Lightning (Neutral Hostile) for (Last created unit) to (Level of Chain Lightning for (Triggering unit))

Unit - Order (Last created unit) to Orc Far Seer - Chain Lightning (Target unit of ability being cast)

Unit - Add a 1.00 second Generic expiration timer to (Last created unit)

Custom script: call RemoveLocation(udg_pt)


In all, these are useful information +rep...


Click to expand...


Thanks! Although I don't plan on adding the trigger que to the tutorial since the melee-game AI is determining when to cast. It's still interesting though and sheds some light on why custom spells are triggerd as they are.

Kino said:

 I can confirm that Flamestrike is used if there at least 2 enemies standing within AOE.


Also, nice to see you finally finished it.


Click to expand...


~Interesting, when I tried using it as a base the AI never seemed to cast it. Of course I had the damage set to 0, which may matter. I'll check into it this weekend, I don't have much time during the week.


~ //\\^^//\\

VeljkoM said:

 I linked this to my AI Editor Tutorial if that is ok. I have given you credits.


Click to expand...


Cool! I'm glad you found it useful.


//\\^^//\\

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I'm going to go ahead and approve this. However, feel free to update it whenever you want, and I'll probably check it out.


~Approved.

#### Boris_Spider

#### Boris_Spider

 Level 14

JoinedDec 12, 2009Messages1,027

Resources: 7

#### Resources
Icons 4Maps 2Tutorials 1

PurgeandFire111 said:

 I'm going to go ahead and approve this. However, feel free to update it whenever you want, and I'll probably check it out.


~Approved.


Click to expand...


Cool! I'll update as I figure out the AI's capabilities.


//\\oo//\\

#### Boris_Spider

#### Boris_Spider

 Level 14

JoinedDec 12, 2009Messages1,027

Resources: 7

#### Resources
Icons 4Maps 2Tutorials 1

Update:

I haven't forgotten about this tut. The test map is nearly complete. I'll start verifying in a couple weeks probably. The test map will be attached and will have instructions on how to test other base abilities.

//\\oo//\\

#### puggsoy

#### puggsoy

 Level 7

JoinedMay 13, 2011Messages310

This is extremely helpful. Personally I'm kinda surprised that somebody only got round to making this after so many years of WC3 modding, it really is something a lot of people would want/need. It's what I need, anyway.


+rep

#### myronbeg

#### myronbeg

 Level 4

JoinedMay 1, 2011Messages81

It's good to share info with other people, but I'm not suprised with all these AI casting spells. No matter what custom spells the user will put on the units, eventually the AI controlled units will try to use the spells based on the rawcode data of the ability. However, they will never LEARN skills, like Heroes.


How about putting Wind Walk when the AI-controlled units are almost dying?


How about Cloud when the enemy approaches to attack enemy's tower defense building?


Holy Light will spam on any nearby Undead units but at the same time also concern about bleeding units.


To be honest, Curse doesn't just apply on AI-controlled units, but we Users will also do the same thing. That's because they are auto-caster.


Cannibalize will happen when a unit is injured.


EDIT: I have discovered many more AI using spells cuz I was play with AIs

#### Boris_Spider

#### Boris_Spider

 Level 14

JoinedDec 12, 2009Messages1,027

Resources: 7

#### Resources
Icons 4Maps 2Tutorials 1

myronbeg said:

 It's good to share info with other people, but I'm not suprised with all these AI casting spells. No matter what custom spells the user will put on the units, eventually the AI controlled units will try to use the spells based on the rawcode data of the ability. However, they will never LEARN skills, like Heroes.


How about putting Wind Walk when the AI-controlled units are almost dying?


How about Cloud when the enemy approaches to attack enemy's tower defense building?


Holy Light will spam on any nearby Undead units but at the same time also concern about bleeding units.


To be honest, Curse doesn't just apply on AI-controlled units, but we Users will also do the same thing. That's because they are auto-caster.


Cannibalize will happen when a unit is injured.


EDIT: I have discovered many more AI using spells cuz I was play with AIs


Click to expand...


Thanks for the additional info!


Update: The test map has finally been set up. I'm working on balancing a spell's damage, then I can upload the map and start demonstrating base abilities.


//\\^^//\\

#### Boris_Spider

#### Boris_Spider

 Level 14

JoinedDec 12, 2009Messages1,027

Resources: 7

#### Resources
Icons 4Maps 2Tutorials 1

Update:


Current test map has finally been added. I'll diversify the base spells as possible.


//\\oo//\\

#### Kino

#### Kino

 Level 24

JoinedOct 12, 2008Messages1,783

Resources: 22

#### Resources
Models 5Maps 17

That test map is seriously entertaining just to watch, Im serious.

#### watermelon_1234

#### watermelon_1234

 Level 14

JoinedNov 18, 2007Messages1,084

Resources: 10

#### Resources
Spells and Systems 9JASS 1

Wow, I didn't expect you to make such an interesting test-map. XD

Anyway, the tutorial is really nice. It would help though if you categorized your abilities (like how they target stuff) or at least make them alphabetized.

#### Boris_Spider

#### Boris_Spider

 Level 14

JoinedDec 12, 2009Messages1,027

Resources: 7

#### Resources
Icons 4Maps 2Tutorials 1

Kino said:

 That test map is seriously entertaining just to watch, Im serious.


Click to expand...

I've lost about 3 hours making it (everything's from scratch, except the imported spells) and around 10 Hrs watching it. That's why it took me so long to upload it. ::/

//\\^^//\\

watermelon_1234 said:

 Wow, I didn't expect you to make such an interesting test-map. XD

Anyway, the tutorial is really nice. It would help though if you categorized your abilities (like how they target stuff) or at least make them alphabetized.


Click to expand...

That's what I plan to do next. A list of recommended base abilities will be present too. Thanks for the suggestion!

//\\^^//\\

#### VeljkoM

#### VeljkoM

 Level 21

JoinedJul 27, 2008Messages14,361

Resources: 4

#### Resources
Maps 3Tutorials 1

Fun little map indeed and will serve great purpose in further testing.

By the way I was wondering when you tested abilities like Sleep or Hex did you set it to last equally on heroes and units or left it to default to last longer on units? Perhaps AI prefers to cast these spells on units because they last longer?

#### Boris_Spider

#### Boris_Spider

 Level 14

JoinedDec 12, 2009Messages1,027

Resources: 7

#### Resources
Icons 4Maps 2Tutorials 1

VeljkoM said:

 Fun little map indeed and will serve great purpose in further testing.


By the way I was wondering when you tested abilities like Sleep or Hex did you set it to last equally on heroes and units or left it to default to last longer on units? Perhaps AI prefers to cast these spells on units because they last longer?


Click to expand...


No, but I will. Thanks for suggesting that.


//\\^^//\\

#### Tirlititi

#### Tirlititi

 Level 12

JoinedJul 11, 2010Messages422

Resources: 14

#### Resources
Models 7Maps 2Spells and Systems 4JASS 1

I think that, indeed, autocast spells have the same "event for firing" for their autocast and for their IA use (which means their autocast doesn't have to be enabled for IAs).

 ~Frost Nova - Spammed at every cooldown


Click to expand...

This is not true. I use Frost Nova as a base ability for occasionnaly casts. It may be casted when the unit is damaged or when the fight begins (ie. when the caster sees an enemy and goes toward him). It seems to depend on the number of allies/ennemies around, though. Anyway, it is much less used than Chain Lightning, for instance.

 ~Curse - Spammed on engaged enemy units


Click to expand...

For my experience, it depends on the target's damages compared with the caster's current HP (and maybe its allies' around). I think it's the same IA than the autocast's one but I'm not sure for Curse.

Inner Fire is, like Frost Nova, casted when the caster or its allies are damaged and not even always at the first shot. I also use it as an occasionnaly spell. If I remember well, I compared the autocast and the IA use of this spell and concluded it was the same. I'm maybe mistaken, it was some times ago.

This topic is really a good idea. I wish you good luck because it's hard to fullfill and I rarely saw any information about it on the internet.

#### Mr_Bean

#### Mr_Bean

 Level 17

JoinedFeb 11, 2011Messages1,860

Resources: 7

#### Resources
Maps 2Spells and Systems 5

mckill2009 said:

 I'd also like to add that the ORDER of casting a custom spell by the AI dummy is this...

Untitled Trigger 001
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Storm Bolt

Actions
Set pt = (Position of (Triggering unit))

Unit - Create 1 Dummy for (Triggering player) at pt facing Default building facing degrees

Unit - Add a 1.00 second Generic expiration timer to (Last created unit)

Unit - Add Chain Lightning (Neutral Hostile) to (Last created unit)

Unit - Set level of Chain Lightning (Neutral Hostile) for (Last created unit) to (Level of Chain Lightning for (Triggering unit))

Unit - Order (Last created unit) to Orc Far Seer - Chain Lightning (Target unit of ability being cast)

Custom script: call RemoveLocation(udg_pt)

Not this...

Untitled Trigger 001
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Storm Bolt

Actions
Set pt = (Position of (Triggering unit))

Unit - Create 1 Dummy for (Triggering player) at pt facing Default building facing degrees

Unit - Add Chain Lightning (Neutral Hostile) to (Last created unit)

Unit - Set level of Chain Lightning (Neutral Hostile) for (Last created unit) to (Level of Chain Lightning for (Triggering unit))

Unit - Order (Last created unit) to Orc Far Seer - Chain Lightning (Target unit of ability being cast)

Unit - Add a 1.00 second Generic expiration timer to (Last created unit)

Custom script: call RemoveLocation(udg_pt)

Click to expand...

Why is this the case? I always use the one you said not to use

I found that if you add the expiration before ordering the unit to cast, it doesn't cast for some reason (maybe 1 second is too short?). But what is wrong with using the one you said not to?

You must also add this line:

AI - Ignore (Last created unit)'s guard position
If you don't then the computer will tell the dummy to do something else, therefore stopping it from casting its spell.

Also, you set the trigger to set the level of the dummy's ability to the level of Chain Lightning for the casting unit, but the condition says:

(Ability being cast) Equal to Storm Bolt

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

do I really have to prove that mr bean???...

#### Attachments

test.w3x

 16.8 KB

 · Views: 367



#### Mr_Bean

#### Mr_Bean

 Level 17

JoinedFeb 11, 2011Messages1,860

Resources: 7

#### Resources
Maps 2Spells and Systems 5

From your test map I learned that the enemy's priest didn't cast because you didn't ignore it's guard position. What were you actually trying to prove?

EDIT: Okay, so it works if you put the expiration timer just after the unit is created and not at the end.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

and why would you add another line of code (AI - ignore...) and make it 8 lines insead of 7?, it's useless and slower...

#### Mr_Bean

#### Mr_Bean

 Level 17

JoinedFeb 11, 2011Messages1,860

Resources: 7

#### Resources
Maps 2Spells and Systems 5

So that if the owner of the dummy is controlled by a computer player, the dummy will still cast the ability. If the owner is a computer and you leave out this line THE ABILITY WON'T BE CAST.

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

instead of adding another line, just re-arrange the ORDER of the trigger, stated in post#21...


like I said, adding another line (AI - ignore...) is useless and slower...

#### Cosmas

#### Cosmas

 Level 5

JoinedAug 12, 2010Messages87

Resources: 3

#### Resources
Maps 3

Useful in melee maps and totaly useless in custom map. All spells are used only if someone from pc team near spell caster is in combat, it is easier to use trigger if unit is attacked then and no need to disable dummy unit from controll.


 Last edited: Sep 9, 2011

#### Tirlititi

#### Tirlititi

 Level 12

JoinedJul 11, 2010Messages422

Resources: 14

#### Resources
Models 7Maps 2Spells and Systems 4JASS 1

Watch it more closely, Cosmas. The IA for spells is really tricky and more complex than most of triggers you can make for simple creeps.


About the "AI - Ignore guard position", I never had this problem and I think the computer shouldn't use dummies if they are well-made.

#### Boris_Spider

#### Boris_Spider

 Level 14

JoinedDec 12, 2009Messages1,027

Resources: 7

#### Resources
Icons 4Maps 2Tutorials 1

I've got final exams next week, but after those I'll finally have the time to work on this tut. So far there's been a lot of positive feedback about it.

//\\^^//\\

#### HuanAk

#### HuanAk

 Level 9

JoinedApr 26, 2011Messages403

Resources: 1

#### Resources
Maps 1

This list is very helpful.


but I think it may depend on map or setting too ?


because in my map, Chain Lightning, Storm Bolt never cast by AI. and I have to put them into Orb with 100% chance to cast.


-----


lots of ability do not work on orb too.


anyone know the rules to decide what type of ability will work/not work on Orb? (eg, put them into Orb ability with 100% chance to cast).

#### Boris_Spider

#### Boris_Spider

 Level 14

JoinedDec 12, 2009Messages1,027

Resources: 7

#### Resources
Icons 4Maps 2Tutorials 1

HuanAk said:

 This list is very helpful.


but I think it may depend on map or setting too ?


because in my map, Chain Lightning, Storm Bolt never cast by AI. and I have to put them into Orb with 100% chance to cast.


-----


lots of ability do not work on orb too.


anyone know the rules to decide what type of ability will work/not work on Orb? (eg, put them into Orb ability with 100% chance to cast).


Click to expand...


Chain Lightning won't cast if the damage is set as "0".


I haven't looked into orb abilities yet, I will.


I will probably have time to finish this tut in about a month, if not sooner. I have been working on it, but haven't posted the update yet. I'll be laid off next month and time will be the only thing I have for the 2-3 weeks until I rotate back to school.


\\//xx\\//

#### Shyrony

#### Shyrony

 Level 7

JoinedOct 8, 2010Messages16

Resources: 7

#### Resources
Models 7

Don't forget about Death coil, it's treated just like Holy light, but for opposite targets.


Slow is also casted at any nearly foes as fast as how many foe there are, mana (if it use any), and cooldown.

#### Boris_Spider

#### Boris_Spider

 Level 14

JoinedDec 12, 2009Messages1,027

Resources: 7

#### Resources
Icons 4Maps 2Tutorials 1

Shyrony said:

 Don't forget about Death coil, it's treated just like Holy light, but for opposite targets.


Slow is also casted at any nearly foes as fast as how many foe there are, mana (if it use any), and cooldown.


Click to expand...


I've added Death Coil. I'm currently looking into Slow, but I believe you're correct.


Thank you for contributing!


//\\oo//\\

#### Weigazod

#### Weigazod

 Level 8

JoinedFeb 22, 2009Messages271

Resources: 1

#### Resources
Maps 1

Raise dead is only use in combat only, not at every cooldown.


War Stomp/Thunder Clap will not be used if enemies around is air units.


Mirror Image will be used if the unit is attacked. (Those damn blademasters always do this to me)

#### ratel

#### ratel

 Level 4

JoinedApr 7, 2012Messages63

Thank you,now I will be able to set the creeps to cast spells on heroes even if they are single

#### gjpaixao13

#### gjpaixao13

 Level 2

JoinedApr 13, 2011Messages13

Some Tips

Boris_Spider said:

 Chain Lightning won't cast if the damage is set as "0".


I haven't looked into orb abilities yet, I will.


I will probably have time to finish this tut in about a month, if not sooner. I have been working on it, but haven't posted the update yet. I'll be laid off next month and time will be the only thing I have for the 2-3 weeks until I rotate back to school.


\\//xx\\//


Click to expand...


Hello!!!


Great post, I've memorized all these spells and when to use then through the years !


Well, as for the Orb abilities, as for some auto-cast spells as Searing Arrows etc... I found that it bugs when the unit auto-target other unit.


To put it simple, turn Advanced Unit Control on the Ally property to test, and you must be on the same team as the tested player ofc.


Now, make the spell auto-cast and use it on a enemy.The spell will work fine and the CD wil fire.


But, put 2 units and order yours to attack ONE of them.After it killed that unit, yours will auto-target the other, but the CD wil NOT FIRE.


This happens with orb effects as spells(Slow,Purge etc Item Spells transformed to unit or hero spell).


I have other spells tested too.


Black Arrow and Parasite - > The ai normally uses it when the targe is dying, perhaps to spawn the unit faster giving that the target is almost dead.


Searing Arrows -> Uses it whenever the CD is off.


Cold Arrows -> PREFERED over Searing Arrows in the case the hero/unit has both spells.


Shadow Strike -> Uses when the target has less then % of his health.Never study to see the right %, but I never saw it using on a 100% target.


Dark Portal (Archimonde) -> Uses whenever is available.


Finger of Death -> Uses whenever is available


Starfall -> Uses when there is 2 or more enemy units close.


HOLY LIGHT/DEATH COIL -> One thing I notice, while doing some Tank/Spank boss fights , is that if the hero is taken a LOT more damage then the heal, the healer or himself(if he has this spell) STOPS casting it.


That said, thz for this post ! And +rep

#### peter1987kh2

#### peter1987kh2

 Level 1

JoinedJul 29, 2012Messages4

Help

My english not very good, i am from malaysia


I add a "attribute bonus" and a self make skill to all heros


I change the level limit to 100


I realise that, they wont self use the attribute bonus and the skill that add into them.


how? Can you help me?

#### Weigazod

#### Weigazod

 Level 8

JoinedFeb 22, 2009Messages271

Resources: 1

#### Resources
Maps 1

peter1987kh2 said:

 My english not very good, i am from malaysia


I add a "attribute bonus" and a self make skill to all heros


I change the level limit to 100


I realise that, they wont self use the attribute bonus and the skill that add into them.


how? Can you help me?

Click to expand...


Well, there are two ways but i do not have time to tell you in detail so you will have to learn how to do it in Hiveworkshop tutorial.


First, you use AI campaign.


Second, you can use trigger to make them do so.

#### peter1987kh2

#### peter1987kh2

 Level 1

JoinedJul 29, 2012Messages4

OK, I will try, but can you give me the link of Hiveworkshop tutorial?

#### mckill2009

#### mckill2009

 Level 30

JoinedMar 10, 2009Messages5,014

Resources: 43

#### Resources
Maps 9Spells and Systems 31JASS 2Tutorials 1

attribute can't be 'used', they can only be 'learned' so you have to trigger it...

#### Rainmaker

#### Rainmaker

 Level 3

JoinedJun 3, 2009Messages53

Fantastic tutorial, amazingly useful. Just one thing: if I understood you correctly, you said that melee AI doesn't use transform abilities (Destroyer Form etc.). How about campaign AI? It sure sucks if every such ability needs to be triggered manually...

#### VeljkoM

#### VeljkoM

 Level 21

JoinedJul 27, 2008Messages14,361

Resources: 4

#### Resources
Maps 3Tutorials 1

Think it is because transformation creates separate unit type. Perhaps if you order in AI script for AI to "train" he would transform existing unit, just he would also then recreate gone unit.

#### Tirlititi

#### Tirlititi

 Level 12

JoinedJul 11, 2010Messages422

Resources: 14

#### Resources
Models 7Maps 2Spells and Systems 4JASS 1

I think the AI handled by the AI editor and AI scripts (campaign AI are AI scripts) are no concern of how units use their abilities. It's more about what kind of units/building they will create and the size of the attacking/creeping/defending groups.

It sounds weird to me, though. AI really never use Bear Form and Destroyer Form?

#### VeljkoM

#### VeljkoM

 Level 21

JoinedJul 27, 2008Messages14,361

Resources: 4

#### Resources
Maps 3Tutorials 1

I have seen AI using those abilities. But I think AI doesn't really see them as just regular abilities since morphed units are treated differently then base units. Druid and bear is just not same for AI and won't cast it as it would think he lost a unit unless he has need to train morphed unit. Though I have yet to test this.

#### Sephalo

#### Sephalo

 Level 19

JoinedOct 12, 2007Messages1,821

Parasite is always cast on the furthest target inside the range.


So if 2 units are inside the 700 range, one of them is 300 away, the other one 600. Then the one that's 600 away will be the first choice target from an autocast Parasite.


(Found this out in my ORPG, since I base all autocast ranged spells on Parasite because it's the only autocast spell (not talking bout arrow enhanced spells) that has a missile effect)

#### watermelon_1234

#### watermelon_1234

 Level 14

JoinedNov 18, 2007Messages1,084

Resources: 10

#### Resources
Spells and Systems 9JASS 1

Silence - Casts on units with at least 75 mana. Seems to prefer player units and non-heroes over heroes.

EDIT:

Banish - Prefers casting on summoned units.


 Last edited: Sep 19, 2012

#### HammerFist

#### HammerFist

 Level 21

JoinedMar 22, 2012Messages1,549

Resources: 33

#### Resources
Models 18Icons 8Skins 1Maps 2Spells and Systems 4

I like this tutorial. It can be useful for maps with AI to do not create boring triggers for the spell casting.(mostly alternative melee)


Vote for approve and +REP

#### 13lade619

#### 13lade619

 Level 5

JoinedMay 29, 2009Messages63

Resources: 6

#### Resources
Skins 1Maps 5

This is awesome.

I discovered some of them by myself a long time ago too ^^.

Thanks for the list.

You can add these to the list:

Drunken Haze

Acid Bomb

Mana Drain

Inner Fire

#### Gekigengar

#### Gekigengar

 Level 16

JoinedAug 20, 2009Messages1,550

Resources: 3

#### Resources
Maps 1Spells 2

This is really useful.

I don't know how Slow is Used by the AI,

but sometimes they use it while engaging, or when an enemy is escaping.

sometimes its never used at all. @_@

i notice their SPAM too, which sometimes lead to Multiple cast instances weirdly during that split second.


 Last edited: Nov 20, 2012

#### ck5524209

#### ck5524209

 Level 3

JoinedNov 30, 2012Messages30

~Shadow Strike


this line has an error word "afflcited "

 You must log in or register to reply here.


### Similar threads

Locked

 AI Cast Custom Spell: Custom Script Help


All_Time_Pro

Nov 7, 2017

Requests

Replies1Views2K

Nov 8, 2017
All_Time_Pro

Locked

[Spell]
 Changing AI Spell/Ability Priority


Grimsnarl

Oct 17, 2022

World Editor Help Zone

Replies1Views1K

Oct 18, 2022
Warseeker

Locked

[General]
 Make AI cast spells


sethmachine

May 31, 2014

World Editor Help Zone

Replies3Views2K

May 31, 2014
Karzama

Locked

[Spell]
 Base abilities that are auto casted


fahrbian

Apr 4, 2019

World Editor Help Zone

Replies0Views1K

Apr 4, 2019
fahrbian

Locked

 Melee AI and custom abilities?


Marine

Mar 30, 2016

World Editor Help Zone

Replies4Views3K

Mar 30, 2016
Daffa

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top