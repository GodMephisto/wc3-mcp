---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/hero-passive-transformation.198482/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Hero Passive Transformation
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/hero-passive-transformation.198482/>
#### Lordshinjo

L

#### Lordshinjo

##
 Goal

The goal of this tutorial is to transform a hero passively (without using an active ability that could be disabled or interrupt his current order), without using the Chaos ability, since it is completely buggy and only usable with regular units.


(see PitzerMike's brilliant Ability Guide for more details)

##
 Method

Say you want to transform a hero, whose unit-type is "Unit Type 1", into "Unit Type 2".

You need to create an ability based on Bear Form [Abrf] (though any other permanent transformation probably works too).

Then (this is the hard part of this method) put in "Unit Type 1" in the "Alternate Form Unit" field of the spell, and "Unit Type 2" in the "Normal Form Unit", as illustrated in this image:

Now, to transform your unit, you just need to add and remove the Bear Form ability to your unit to transform:

Actions
Unit - Add Transformation 1->2 to YourUnit

Unit - Remove Transformation 1->2 from YourUnit

Your unit is now permanently transformed into "Unit Type 2", and will not morph back at any point without using this method again, or some morphing ability.

##
 How not to fail at using this method

Fill out the data fields correctly: the "Normal Form" field is the unit you morph TO, the "Alternate Form" field is the unit you morph FROM.

You only need to add and remove the ability: do not set the ability as unavailable for the player owning the unit, or nothing will happen.

##
 Known (minor) bugs

This method can create some weird results in particular circumstances, but they can easily be avoided:
If the unit is hexed when you try to transform it, the unit will permanently gain some movement speed. (also named the 1.25 bug)


This can be avoided by simply checking whether the unit is hexed or not before transforming it. (then use whatever method you see fit to make sure it transforms when it stops being hexed)

If the unit you morph to uses animations with a tag (like "alternate"), and the unit is currently idling, it will stay in its base animations (with no tag) until it stops idling.


This can be avoided by manually adding the tag to the unit.

Animation - Add the alternate animation tag to YourUnit

#### Attachments

objectdata.png

 17.3 KB

 · Views: 10,318



#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Excellent

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

Very nice. I didn't notice any side-effects. The handle id is the same, items and ability levels remain the same, no stat bugs.


This is great for changing damage/armor type or toggling between melee/ranged.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Works perfectly, and is a nice tutorial.


~Approved.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Wow, really cool! never realized that method when its so much just a basic logic...

+rep!

#### Weep

#### Weep

 Level 10

JoinedJul 12, 2009Messages321

Resources: 3

#### Resources
Models 2Spells and Systems 1

Fantastic! This doesn't bug with armor and damage like Chaos does.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Great resource, thanks for providing this for us.

#### watermelon_1234

#### watermelon_1234

 Level 14

JoinedNov 18, 2007Messages1,084

Resources: 10

#### Resources
Spells and Systems 9JASS 1

Wow, I can't believe I never saw anything like this until now.

Good job! XD

Edit: Hm, if I transform an Archer to a Dryad, the mana regeneration doesn't seem to work. Does regeneration work correctly for heroes? Never mind it does.


 Last edited: Aug 4, 2011

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Aside from your mentioned minor bugs, do you know which stats fail to get removed/applied by the new type? For example have heard about cast point or the build menu remaining.


Thing is, you could have a base unit type and two abilities for each other unit type to convert the base into the target type and backwards. This way, it would be possible to arbitrarily change a unit's type ingame with a linear increment of objects. Nowadays, I use to trigger nearly all unit stats but resetting the model/icon is a bit problematic and therefore this would be a shortcut.

#### Lucasvn8

L

#### Lucasvn8

i cant make him choose footman or bandit at same time

#### Bloodbath

#### Bloodbath

 Level 9

JoinedDec 15, 2009Messages523

WTH im super impressed exactly what iv been wanting +rep

#### Shoto

#### Shoto

 Level 7

JoinedMay 11, 2010Messages278

Resources: 2

#### Resources
Spells 2

so if i switch my Unit Type 1 to Unit Type 2, and then want to switch it back, do i have to make another ability that works the other way around? (switch places of Unit Type 1 and 2)

or can i just re-run the trigger?

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

yeah you need another ability which is the exact reverse of the first...

#### Nestharus

#### Nestharus

 Level 31

JoinedJul 10, 2007Messages6,306

Resources: 32

#### Resources
Tools 2Maps 2Spells and Systems 5Tutorials 23

Bugged...


Using this makes it impossible for transformed units to build buildings.

edit


chaos has same bug D:

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

I said it once, and I'm going to say it again: Blizzard is trolling.

They haven't fixed a single reported bug in ages (1.26 patch doesn't count, they only fixed a bug that appeared when they tried to fix a bug in 1.25)

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Ok then don't have transformed units build buildings -_-

#### Luorax

#### Luorax

 Level 16

JoinedAug 7, 2009Messages1,406

Resources: 2

#### Resources
Maps 1Tutorials 1

Magtheridon96 said:

 I said it once, and I'm going to say it again: Blizzard is trolling.


They haven't fixed a single reported bug in ages (1.26 patch doesn't count, they only fixed a bug that appeared when they tried to fix a bug in 1.25)


Click to expand...


No, everything is going according to the planes:


More people getting tired of the buggy WC3 map editor->more people buying StartCraft 2->More money for Blizzard->Profit->(More people realizing that the StarCraft 2's atmosphere sucks, no matter how great its editor is->More people leaving Blizzard's games; but this is just an additional info)

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

SC2 was too little, too late.

WC3 lives on because of modding, but as far as the game itself goes, it is impossible to "balance" because it is only "balanced" for extreme pro players with 120+ APM on a lazy day.

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

Chaos however has the advantage that it requires only 1 ability per target unit type if you want to freely morph anything into anything.


Btw, the armor bug with Chaos allows to modify base armor in an inconvenient way.

#### Glow_Stick

G

#### Glow_Stick

Question - Ability: Transform

If the game needed 3 forms; a DPS, a Tank and a Healer all in one... how can you make the ability change you when needed? (I.E. being attacked heavily changes you to tank, allies have low health change to healer and when there are enemies nearby instant change to DPS)

A screenshot: https://www.hiveworkshop.com/forums/members/217583-albums5559-picture57830.html

could you show a trigger(s) that can do this? thanks!

I have a feeling it isn't that difficult now that I typed it out haha

~Glow_Stick

- also a music producer (Dubstep too!) on newgrounds com, name: Opaix (Electrolord99 username) -

#### gorillabull

#### gorillabull

 Level 17

JoinedJul 17, 2011Messages1,863

Resources: 5

#### Resources
Skins 1Spells 4

Glow_Stick said:

 If the game needed 3 forms; a DPS, a Tank and a Healer all in one... how can you make the ability change you when needed? (I.E. being attacked heavily changes you to tank, allies have low health change to healer and when there are enemies nearby instant change to DPS)

A screenshot: https://www.hiveworkshop.com/forums/members/217583-albums5559-picture57830.html

could you show a trigger(s) that can do this? thanks!

I have a feeling it isn't that difficult now that I typed it out haha

~Glow_Stick

- also a music producer (Dubstep too!) on newgrounds com, name: Opaix (Electrolord99 username) -


Click to expand...

well you will need 3 units for that and you will only be able to morph to a healer when morphed to a tank.

if you start with a dps then you can morph to a tank and then to a healer and then back to dps

its not a good idea to automatically change the ability based on conditions tho

#### Dominic74

#### Dominic74

 Level 5

JoinedJul 17, 2010Messages140

I cant get this method to work, my Warcraft Crash EVERY time, what can i do about that?

#### Orion-2K14

#### Orion-2K14

 Level 1

JoinedOct 14, 2014Messages2

Hi!


I need a Help , I'm creating a custom map like a ''DotA'' , but have a problem!


I'm making skill ''Change Courier Type'' (Replaces old unit-type with a new unit).


It's working but , when I start of another ability within Courier like ''Burst'' (Makes courier move faster for 20 seconds. Cooldown: 40 seconds) , then start again with ''Change Courier Form'' It will replace old unit with a new unit , but after changing set's ''Burst'' cooldown to 0.


Any idea how to fix it? , Replaced unit's save ability cooldowns of prewious unit.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

You can make all of the units already for the players, then hide the others. If one casts burst, make them all cast burst. Then when you use GCT, just hide the current courier (and possibly place it in a safe place, together with all the other couriers), then get another courier from the list, put it on the old courier's position and unhide it..

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

I have two questions about this method:

1) does it require to set all abilities as "permanent" if I want them to carry over to the new unit?

2) does the UnitTypeId change? I know that the handle ID remains the same, but is my morphed unit still considered the same type as the original?

So, for example, if my original unit is a footman and the new unit is an archer, will any "Is unit a footman" checks still return true even when morphed?

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

1) Yes.

2) Changes.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Maker said:

 1) Yes.

2) Changes.


Click to expand...

Damn. Workaround-city again.

Is there any other good solution for changing a hero attack to ranged? Warclub ability shoud work, right?


 Last edited: Jul 7, 2015

#### Paillan

#### Paillan

 Level 20

JoinedJan 22, 2011Messages3,996

Resources: 7

#### Resources
Models 3Maps 3Sounds 1

I think this method no longer works, but can anyone else confirm me?

#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2

Paillan said:

 I think this method no longer works, but can anyone else confirm me?


Click to expand...

It still works as of 1.27b.

#### Insanity_AI

#### Insanity_AI

 Level 11

JoinedJun 30, 2017Messages99

Resources: 2

#### Resources
Spells and Systems 2

Thanks to illbean#1337 on Discord, we discovered a bug regarding this method.

Target point of ability being cast seems to get lost after the transformation is performed. Could be that some other event responses get lost as well, haven't tested any further.

Events
Unit - A unit Starts the effect of an ability

Conditions

Actions
Set VariableSet location1 = (Target point of ability being cast)

Game - Display to (All players) the text: ((String((X of location1))) + ( + String((Y of location1)))))

Unit - Add Transform to Militia to (Triggering Unit)

Unit - Remove Transform to Militia to (Triggering Unit)

Set VariableSet location2 = (Target point of ability being cast)

Game - Display to (All players) the text: ((String((X of location2))) + ( + String((Y of location2)))))

Result:

So to anyone having weird issues in the future, make sure to initialize your variables with event responses you'll be using in the spell before the actual transformation, and then use those variables after the transformation.

PS: This was tested on Reforged with Classic graphics

#### Attachments

Trigger Event Response bug.w3m

 17.3 KB

 · Views: 94



#### Illbean

#### Illbean

 Level 6

JoinedSep 5, 2022Messages33

Resources: 2

#### Resources
Spells and Systems 1Ported Icons 1

Insanity_AI said:

 Target point of ability being cast seems to get lost after the transformation is performed.


Click to expand...

From my understanding so far using this system, it only affects the the same trigger the transformation happened in.

Because in one of the spells i'm creating for the Druid Class i'm making, the Hero transforms (using this exact method) and THEN jumps to the targeted location using the initiating spell.

This breaks if I apply the Jump AFTER the transformation, but it works fine if I apply it BEFORE the transformation, just like Insanity_AI mentioned.

However, while in one of my other forms, already transformed, I can, without issue cast the jump spell.

-----------------------

From some testing while making more spells, it seems that WC3 is running triggers 1 after the other in the same order they're listed in the trigger editor. Meaning top most trigger goes first, and bottom most goes last.

This means that if we're using this system to transform a unit, if u've got another trigger that needs the unit to be a certain unit type when triggering from "Starts the effect of an ability", that trigger needs to be either after or before the transformation skill.

Example:

My transformation skill is Warstomp.

-Trigger - Warstomp Transformation-

Event: Unit starts the effect of an ability, ability = Warstomp

Actions: Transform from Human into Tauren

-Trigger - Warstomp Cool Effect

Event: Unit starts the effect of an ability, ability = Warstomp

Condition: Unit type of casting unit = Human

Action: Cool Effect

In this example, the second trigger (Warstomp Cool Effect) will not go off because when the game gets to this trigger, the unit will already be a Tauren, and will thus faul the "= Human" check.


 Last edited: Mar 26, 2023

 You must log in or register to reply here.


### Similar threads

Locked

[Solved]
 Passive Hero Transformation :(.


Kusanagi Kuro

Jul 31, 2013

World Editor Help Zone

Replies3Views2K

Jul 31, 2013
Kusanagi Kuro

Locked

 Morphing


DexterFstone

Nov 25, 2020

World Editor Help Zone

Replies3Views1K

Nov 25, 2020
DexterFstone

 Disable an ability for a specific hero/unit


WaterKnight

Nov 20, 2012

Misc/General Mapping Tutorials

Replies19Views23K

Jul 2, 2020
Cheshire

Locked

 Hero Druid Transform Malfunction


Silverfish423

Dec 13, 2008

World Editor Help Zone

Replies7Views2K

Dec 17, 2008
Silverfish423

Locked

 Locust Swarm and Transformation


Verdun

Mar 27, 2013

World Editor Help Zone

Replies4Views2K

Mar 27, 2013
Xonok

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