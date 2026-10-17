---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-make-a-sword-slash-ability-no-target.184270/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Make a Sword Slash Ability No Target
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-make-a-sword-slash-ability-no-target.184270/>
#### sPy

#### sPy

 Level 22

JoinedApr 10, 2009Messages380

Resources: 77

#### Resources
Models 52Icons 23Tutorials 2

~How to make a Sword/Slash Ability~

Introduction

In this tutorial, I will teach you how to make a slash ability. It is pretty simple, so no worries!

The slash ability is a no target type spell. For example, if you presses the ability, the units that is in front of you will automatically takes damage.

All we need is some abilities and triggers.Now, let's get started, shall we?

Unit:

A Dummy (I based the dummy from the chicken, and then I replaced the Wander ability to Locust. I removed the model of the chicken, replaced the path of the model into something like ".mdl" and then I removed the shadow of the chicken. Also, change the movement type from foot to fly to prevent collision bugs.)

Variables:

MeleeCaster (Unit Variable)

MeleeDummy (Unit Variable)

MeleePoints (Point Array Variable)

Abilities

Firstly, let us make the abilities first. Without the abilities, the triggers can't be done.

Let's set up the spells now, we'll go step by step.

Make a custom Summon Bear first, I'll name it "Melee". I'll show you by pictures:

In this picture, I'm not making the ability as a hero spell because it looks kinda weird in my opinion.

I'm making the Summon Bear as "low profile" as possible by changing the summoned unit into a dummy, set it's duration to 0.01 second, and then I removed it's summoning effects.

Also, you'll need to make a custom buff for this ability. To do that, simply go to the Buff section and then create a custom buff based on "Bear". All you got to do at the buff is only removing it's effect so our "tricks" won't be "noticed".

This one we'll use Fan of Knives spell as base, here's the picture of the ability's data:

For this one, I'll add 100 damage as the description of the Melee ability says.

I'm putting "Number of targets field" to 0 because it means infinite, which I can hurt as many opponent I want. So basically, if you want to hurt 1 unit only, you should put it to 1.

You might be noticing why I'm putting the projectile missile speed to 99999, it is because I wanted it to become instant damage, and I set the blood as it's missile to make it looked more interesting.

For the "Maximum Damage" I put it 999999 so the ability can handle as many opponent as you want.

Trigger

It time for the final part now

For some of you, you might be thinking, "Oh no this will be my worst nightmare". Relax, it's pretty easy.

Let's get started! Here's the trigger:

Melee
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Melee

Actions
-------- I'll create some variables at first to make things easier. --------

Set MeleeCaster = (Triggering unit)

Set MeleePoints[1] = (Position of MeleeCaster)

Set MeleePoints[2] = (MeleePoints[1] offset by 50.00 towards (Facing of MeleeCaster) degrees)

-------- Now let's create a dummy to cast the damage that we has just created. --------

Unit - Create 1 Dummy for (Owner of MeleeCaster) at MeleePoints[2] facing Default building facing degrees

Set MeleeDummy = (Last created unit)

-------- I'm adding expiration time to make it disappear after 0.5 second, and give it an ability. --------

Unit - Add a 0.50 second Generic expiration timer to MeleeDummy

Unit - Add Melee Damage to MeleeDummy

-------- It's time to cast them! So let's add this action. --------

Unit - Order MeleeDummy to Night Elf Warden - Fan Of Knives

-------- Time to remove leaks in order to prevent ingame lags. --------

Custom script: call RemoveLocation(udg_MeleePoints[1])

Custom script: call RemoveLocation(udg_MeleePoints[2])

-------- You're done! --------

-------- Q: Why do create dummy unit at MeleePoints[2] but not [1]? --------

-------- A: Because hero attacks are facing front, so we're going create it a little in front--------

-------- Q: What is "Facing of MeleeCaster"? --------

-------- A: It means the facing direction of the caster, so it means if you want your dummy face your caster's left side, it should be "Facing of MeleeCaster + 90.00" and "Facing of MeleeCaster - 90.00" for right side.--------

-------- Q: Why do you use Warden - Fan of Knives ability to order dummy cast spells? --------

-------- A: Because we the melee damage spell was based from Fan of Knives. --------

See? Pretty simple right?

That's all for now, good luck in triggering! Here, I added a sample map in case you wanted to see how it works.

#### Attachments

Melee.w3x

 20 KB

 · Views: 1,281




 Last edited: Dec 7, 2010

#### Grey Nightmare

#### Grey Nightmare

 Level 20

JoinedFeb 24, 2009Messages2,999

Resources: 1

#### Resources
Tutorials 1

Not bad, seems ok (I don't think it leaks etc).

A tad basic atm though, perhaps you could add an explanation or some variant examples.

Like limiting it to hurting one unit at a time or having multiple abilities on one unit (swipe arc, jab, right, left?).

Otherwise good job, some newer members (maybe older to =?) could find this useful.

#### Darkzealot_128

#### Darkzealot_128

 Level 12

JoinedJun 29, 2010Messages785

Resources: 10

#### Resources
Maps 6Spells 1Tutorials 3

Smart object editor work, leaks have been removed (good), clean grammar and spelling, but I don't believe it's necessary to create a whole tutorial over this.

#### 88WaRCraFT3

#### 88WaRCraFT3

 Level 13

JoinedJun 9, 2009Messages1,134

Resources: 5

#### Resources
Icons 2Spells and Systems 3

Really cool idea, i was always messing up, and losing much much time to make a aoe melee spell  gj

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Short, but a lot of newer people look for ways to make spells like this. Overall, a nice tutorial and good explanations so I see no reason not to approve it.


~Approved.

#### Sheephunter

#### Sheephunter

 Level 11

JoinedDec 21, 2008Messages766

Resources: 3

#### Resources
Maps 1Spells 2

There is other ways to make this without dummys, but simple spell overall for new players.

#### Orcnet

#### Orcnet

 Level 30

JoinedJul 31, 2010Messages5,248

Resources: 5

#### Resources
Maps 4Tutorials 1

Nice!

this is a so unique, I'll try the demo map right away!

#### ubran

#### ubran

 Level 5

JoinedSep 29, 2008Messages170

Another thing almost exactly like this is a non-targettable slam using only 1 dummy spell like Thunderclap and placing the dummy unit slightly in front of the caster unit then cast the damaging Thunderclap spell for an instant cast slam or multiple slams that slow directly in front of the caster. This would make positioning more important for RPGs and AoSes than with a plain old large area ground stomp and would look good with most heroe's attack slam animations.


Of course, the popular way to do this is to use thunderclap for a non-targettable (or storm bolt to aim the spell) and making a complex system of coordinates just to overcomplicate things. :]

#### Andser

#### Andser

 Level 6

JoinedJan 25, 2011Messages63

Resources: 8

#### Resources
Models 1Maps 4Spells and Systems 2Tutorials 1

Doesn't mean the summoned dummy unit (from the spell, not trigger) a leak? because it's a lot more efficient to use "Channel" as the spell that initiates the trigger. It can be more real if you set the "disable other spells" to true in the channel spell, because you can't cast spells while you are attacking.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Andser said:

 Doesn't mean the summoned dummy unit (from the spell, not trigger) a leak? because it's a lot more efficient to use "Channel" as the spell that initiates the trigger. It can be more real if you set the "disable other spells" to true in the channel spell, because you can't cast spells while you are attacking.


Click to expand...


It's duration is set to 0.01 so it will be removed quickly (there may be other memory leaked related to creating a unit, but I won't go into that). I think he just wanted it there to display an effect, or something similar. But yes, generally we base it off Channel, and I can encourage you to do so.

 You must log in or register to reply here.


### Similar threads

Locked

 Attack spell won't work


PROXY

May 19, 2013

World Editor Help Zone

Replies5Views1K

May 19, 2013
PROXY

 Making a Grenade Type Spell


PrisonLove

Jul 15, 2008

Trigger (GUI) Editor Tutorials

Replies18Views19K

Mar 17, 2018
Bobastian

B

 Making any Spell Autocasting


Ralle

Mar 15, 2006

Trigger (GUI) Editor Tutorials

Replies12Views59K

Dec 22, 2017
Rogue Kessler

 Single Target Spell for Multiple Units


Rheiko

Oct 2, 2013

Trigger (GUI) Editor Tutorials
2

Replies64Views19K

Jul 7, 2017
Daffa

 Introduction To Trigger-Enhancing Spells


whitenights

Jul 27, 2004

Trigger (GUI) Editor Tutorials

Replies6Views36K

Jun 8, 2016
TheLordOfChaos201

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