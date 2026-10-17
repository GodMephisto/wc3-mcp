---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/everything-about-basic-attacks.307758/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Everything About Basic Attacks
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/everything-about-basic-attacks.307758/>
#### Yours Truly

#### Yours Truly

 Level 30

JoinedJan 9, 2016Messages1,487

Resources: 14

#### Resources
Models 10Maps 2Spells and Systems 1Tutorials 1

Everything About Basic Atacks

This tutorial is mainly for older patches, new natives to modify attack values exist in the later versions.

Table of contents:

1. Basic Attacks, what are they?

2. Requirements to Detect Basic Attacks

3. Detecting Basic Attacks

4. Resetting the Basic Attack Timer

5. Various Examples of Cool Basic Attack Related Abilities

6. Credits

1. Basic Attacks, what are they?

Basic attacks are the normal attacks all units uses (If their attacks aren't disabled in the object editor). At first, these basic attacks are not very modify-able because the editor doesn't actually have any natives that can change or even detect basic attacks. In this tutorial, I will explain how to do exactly that

2. Requirements To Detect Basic Attacks

Now keep in mind that the World Editor does not have anything that we can use to benefit from the basic attacks. Bash and Critical Strike may sound like a good idea,but using the "Ability being cast Equal to" condition for said abilities will not work. We have to get creative on this.

Using Bribe's magnificent Damage Engine System, we can create a workaround on how to detect basic attacks.

First of, we need to create an ability based on "Item Attack Corruption Bonus" and also a new buff that the ability will use. I recommend naming them both to "Basic Attack". Now change all the data values (Armor Penalty, Damage Bonus Dice, Enable Attack Index) to 0 but keep the duration to at least 0.01, like so:

Simply add the ability to your units, and it's basic attacks will apply the Basic Attack buff that we're going to use. Of course, adding it to ALL units may sound cumbersome, but you can always make a trigger that runs on the map initialization that adds the ability to units in the playable map region OR check if units enter playable map area, and then add the ability to it.

3. Detecting Basic Attacks

Now we proceed onto the triggering part of this tutorial. To detect when a basic attack is landed, we have to use the following triggers:

Basic Attack Trigger
Events
Game - DamageEvent becomes Equal to 1.00

Conditions
(DamageEventTarget has buff Basic Attack ) Equal to True

Actions
Unit - Destroy Basic Attack on DamageEventTarget

This trigger will run when a basic attack is hit.IT IS IMPORTANT THAT YOU REMOVE THE BUFF DIRECTLY AFTERWARDS! I haven't really found any other way to detect basic attacks other than this. I've been using this trick for a while and I thought I should share it.

With this trick, we can use basic attacks to make all sorts of crazy spells and abilities that relies on basic attacks to be proc'ed. We'll get to that in a moment.

4. Resetting the Basic Attack Timer

Some of you might not know what this is, but as a LoL player, basic attack resetting is pretty important there. These here are several examples of a Basic Attack Reset:

A Basic Attack reset is an attempt to deal the most damage possible in a short window by resetting the basic attack timer.

We can easily do this with an attack speed boosting spell. By raising the attack speed it gives up to 400% (4.00 in data value) for a 0.60 seconds, a unit will mimic the art of basic attack reset. Simply remove the buff the ability gives when it lands its first basic attack using the method at No.3, and you get yourself a basic attack reset! yay!

5. Various Examples of Cool Basic Attack Related Abilities

All of these abilities are included in the test map down below

Red = Passive abilities

Blue = Active abilities

Punishing Srikes

First basic attack on an enemy deals bonus damage equal to 15% of your Strength and 9% of enemy's maximum health. This effect cannot happen on the same target every 8 seconds.


Click to expand...

Endorser's Determination

Every third basic attack deal 5% of enemy's max health as bonus damage and heals you for 15% Strength.


Click to expand...

Colossal Smash

Gains bonus movement speed and your next basic attack will deal bonus damage equal to 90% Strength and stuns the target for 2 seconds.

Colossal Smash resets your basic attack timer.


Click to expand...

Hammer Swing

Next basic attack deals bonus damage equal to 10% of enemy's current health plus 75% Strength and knocks them back a short distance.

Hammer Swing resets your basic attack timer.


Click to expand...

Nature's Wrath

Calls upon the power of earth, empowring your weapon with the force of nature for 12 seconds. For the duration, your basic attacks deal 50 bonus damage plus 30% of Intelligence.


Click to expand...

6. Credits

Bribe's Damage Engine

Punishing Strikes icon (-Berz-)

Endorser's Determination icon (-Berz-)

Colossal Smash icon (The Panda)

Hammer Swing icon (NFWar)

Nature's Wrath icon (Hemske)

TO AVOID PROBLEMS, PLEASE SET THE BUFF DURATION TO 0.01 INSTEAD OF 0.05. FURTHER TESTING CONVINCED ME THAT LOWER DURATION REDUCES THE CHANCES OF CONFLICTS. THE TEST MAP BELOW USES 0.05 BUFF DURATION, SO PLEASE EDIT THAT IF YOU DOWNLOAD IT. THANK YOU FOR READING THIS OBNOXIOUSLY LARGE TEXT.

#### Attachments

Basic Attack Test Map.w3x

 91.5 KB

 · Views: 754




 Last edited by a moderator: Oct 2, 2020

#### Veritas 117

#### Veritas 117

 Level 14

JoinedNov 17, 2010Messages1,266

This is interesting. I use Looking_For_Help's DDS and it says it can differentiate between physical damage, spell damage, and coded damage. I really like this work around though, it really makes it fool proof when you only want basic attacks to trigger a proc. I use a lot of those with my items so that will be handy. Good work.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

In Damage Engine, if IsDamageSpell is false, the source was physical damage (basic attack). Unless you're trying to detect something very specific, I don't see a need for adding a buff. The buff also crashes the game when ordering a siege unit to attack ground.

Additionally, it messes with other in-game buffs (but maybe not so much with the buff stacking added in 1.30).

#### AGD

#### AGD

 Level 17

JoinedMar 29, 2016Messages688

Resources: 30

#### Resources
Spells and Systems 13JASS 12Lua 4Tutorials 1

Bribe said:

 In Damage Engine, if IsDamageSpell is false, the source was physical damage (basic attack). Unless you're trying to detect something very specific, I don't see a need for adding a buff.


Click to expand...

Afaik IsDamageSpell only checks for the attacktype of the damage event (ATTACK_TYPE_NORMAL?) no matter the cause of the event, while the tutorial is about detecting damage that are caused only by a 'normal attack'. Additionally, a unit can have 'spell' as its normal attack type which would set IsDamageSpell to true.

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

AGD said:

 Additionally, a unit can have 'spell' as its normal attack type which would set IsDamageSpell to true.


Click to expand...

If by "spell" you mean a magic attack type, then this is not the case with Bribe's DDS. I too similar feelings with Bribe when I saw Truly using a buff to make this detection work.

 I've never looked behind the scenes on how the IsDamageSpell boolean works, so @Bribe would be the most qualified to explain how it behaves.

#### AGD

#### AGD

 Level 17

JoinedMar 29, 2016Messages688

Resources: 30

#### Resources
Spells and Systems 13JASS 12Lua 4Tutorials 1

KILLCIDE said:

 If by "spell" you mean a magic attack type,


Click to expand...

No I really meant spell. In the object editor, under the attack type field you can see that you can set it to 'spell' (magic is also another attack type). Btw I just rechecked, this spell attack type I'm talking about is the ATTACK_TYPE_NORMAL in jass.

EDIT:

@xYours Trulyx , I suggest removing the buff after the damage detection =).

EDIT:

The only problem I can see with this technique is that units' attacks can no longer have orb effects. You have to code them, but the problem is that you can't dynamically change the model for attack missiles (which is important).


 Last edited: Aug 14, 2018

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Using Spell as a unit's attack type is not recommended. If you want to supplement a unit's attack with additional spell damage a couple of options are to either give it Searing Arrows, or give the unit Chaos damage and re-apply it as Spell Damage from a Damage Engine trigger (one can set the damage to 0.00 then simply issue a new UnitDamageTarget command to it - but setting a DamageEventType which said trigger filters out for to prevent recursion (or turning off the trigger as per the in-game recursion warning - a more efficient method).

The little locusts from the Crypt Lord ultimate Locust Swarm are the only units which deal spell damage by default. Depending on what you're trying to do, their damage type shouldn't be an issue.

I'd like to see examples of how you can't do it my way, as using the Orb effect is buggy. Nestharus and I used to experiment with the technique (him much more so) and how it was too unpredictable to rely on within a damage system.

#### AGD

#### AGD

 Level 17

JoinedMar 29, 2016Messages688

Resources: 30

#### Resources
Spells and Systems 13JASS 12Lua 4Tutorials 1

Bribe said:

 Using Spell as a unit's attack type is not recommended. If you want to supplement a unit's attack with additional spell damage a couple of options are to either give it Searing Arrows, or give the unit Chaos damage and re-apply it as Spell Damage from a Damage Engine trigger (one can set the damage to 0.00 then simply issue a new UnitDamageTarget command to it - but setting a DamageEventType which said trigger filters out for to prevent recursion (or turning off the trigger as per the in-game recursion warning - a more efficient method).

The little locusts from the Crypt Lord ultimate Locust Swarm are the only units which deal spell damage by default. Depending on what you're trying to do, their damage type shouldn't be an issue.


Click to expand...

Fair enough.

Bribe said:

 I'd like to see examples of how you can't do it my way, as using the Orb effect is buggy.


Click to expand...

What if you only want to detect damages coming from a normal attack (as is the aim of the tutorial) ? We know that any damage that is not ATTACK_TYPE_NORMAL will set IsDamageSpell to false and such damage can come from anywhere - triggered damage, and even spells (especially custom spells, not sure if there are any default spells that aren't ATTACK_TYPE_NORMAL).

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Triggered damage should be using DamageEventType, while spells dealing non-spell damage sounds like the one case where the Orb effect would work better. But why? Sounds like the spell would be better off being triggered if it's so uncommon and/or specific.

#### Yours Truly

#### Yours Truly

 Level 30

JoinedJan 9, 2016Messages1,487

Resources: 14

#### Resources
Models 10Maps 2Spells and Systems 1Tutorials 1

Thank you all for your replies and feedback, but I really don't understand what's going on here. Could y'all kindly compile all the opinions given into one statement so I could make the necessary changes to the tutorial?

#### AGD

#### AGD

 Level 17

JoinedMar 29, 2016Messages688

Resources: 30

#### Resources
Spells and Systems 13JASS 12Lua 4Tutorials 1

Bribe said:

 Triggered damage should be using DamageEventType, while spells dealing non-spell damage sounds like the one case where the Orb effect would work better. But why? Sounds like the spell would be better off being triggered if it's so uncommon and/or specific.


Click to expand...

The issue that I see in doing this is that it will add an extra work in the importing process, especially if a user just recently imported the system into a map that has most of the other things already in place. For example, let's say an AoS-type map has a total of 200 custom spells and about 20% of those deals non-spell damage. Then for all the 40 spells, you have to add a line that sets the DamageEventType before every call to UnitDamageTarget().

xYours Trulyx said:

 Thank you all for your replies and feedback, but I really don't understand what's going on here.


Click to expand...

We are currently discussing if this technique is even needed in the first place, as there's already a known way that aims to achieve the same thing.


 Last edited: Aug 15, 2018

#### Yours Truly

#### Yours Truly

 Level 30

JoinedJan 9, 2016Messages1,487

Resources: 14

#### Resources
Models 10Maps 2Spells and Systems 1Tutorials 1

AGD said:

 We are currently discussing if this technique is even needed in the first place, as there's already some known way of achieving the same thing.


Click to expand...

Oh. Then, I'm okay with this tutorial going down the graveyard. Just thought I'd share something that I used for a long time with the public.

#### Deleted member 247165

D

#### Deleted member 247165

This is actually a nice system.

#### ZethHolyblade

#### ZethHolyblade

 Level 11

JoinedMay 16, 2013Messages193

Resources: 9

#### Resources
Icons 3Skins 6

Very interesting, and might be just what I needed so I'll give it a try.

I've been making a sort of combo chain 'system' involving several abilities asigned to the same unit slot and enabling/disabling themselves in the right succession.

One of these is a 4-hit combo involving 2 abilities for each hit, that is 1 active (Immolation) as trigger and 1 passive (Cleave) as the effect/buff itself that boosts your basic attack:

War3 Cleave Combo Test 3 (Much Faster) - YouTube | HIVE

Problem is that I'm currently using the 'Unit being attacked' event since 'Unit takes damage' is restricted to specific units only. This becomes a hinderance sometimes because the unit might attack the wrong target (if the player reacts too slow or misclicks) 'performing' the ability even If it was issued to attack a different target at the very last second.

Needless to say that the buff gets disabled even if you didn't actually hit anything.

It gets even harder since I got two triggers working as soon as the first attack 'takes place'... one will mark the combo chain as failed if the unit performs a basic attack in between the triggering abilities and the other will also mark it as failed if the player doesn't trigger the next ability in time...

And don't ask me what happens whenever the unit gets an attacking speed buff (git gud, lol xD)

I'll let you know the results.

#### Yours Truly

#### Yours Truly

 Level 30

JoinedJan 9, 2016Messages1,487

Resources: 14

#### Resources
Models 10Maps 2Spells and Systems 1Tutorials 1

ZethHolyblade said:

Very interesting, and might be just what I needed so I'll give it a try.

I've been making a sort of combo chain 'system' involving several abilities asigned to the same unit slot and enabling/disabling themselves in the right succession.

One of these is a 4-hit combo involving 2 abilities for each hit, that is 1 active (Immolation) as trigger and 1 passive (Cleave) as the effect/buff itself that boosts your basic attack:

War3 Cleave Combo Test 3 (Much Faster) - YouTube | HIVE

Problem is that I'm currently using the 'Unit being attacked' event since 'Unit takes damage' is restricted to specific units only. This becomes a hinderance sometimes because the unit might attack the wrong target (if the player reacts too slow or misclicks) 'performing' the ability even If it was issued to attack a different target at the very last second.

Needless to say that the buff gets disabled even if you didn't actually hit anything.

It gets even harder since I got two triggers working as soon as the first attack 'takes place'... one will mark the combo chain as failed if the unit performs a basic attack in between the triggering abilities and the other will also mark it as failed if the player doesn't trigger the next ability in time...

And don't ask me what happens whenever the unit gets an attacking speed buff (git gud, lol xD)

I'll let you know the results.


Click to expand...

Good luck then. Little friendly advice here, put down a trigger that removes the basic attack detector buff at 3.Detecting Basic Attacks. Kinda lazy to edit the thread contents.

#### ZethHolyblade

#### ZethHolyblade

 Level 11

JoinedMay 16, 2013Messages193

Resources: 9

#### Resources
Icons 3Skins 6

xYours Trulyx said:

 Good luck then. Little friendly advice here, put down a trigger that removes the basic attack detector buff at 3.Detecting Basic Attacks. Kinda lazy to edit the thread contents.


Click to expand...

Surething.

Also, you mentioned something about resetting basic attacks. I'm familiar with terms like 'damage decay' and 'combo reset' from another game in which the more hits the combo had the weaker each attack became (down to a *0.30 cap)

Could this also be used similarly? :O

#### Yours Truly

#### Yours Truly

 Level 30

JoinedJan 9, 2016Messages1,487

Resources: 14

#### Resources
Models 10Maps 2Spells and Systems 1Tutorials 1

ZethHolyblade said:

 Surething.

Also, you mentioned something about resetting basic attacks. I'm familiar with terms like 'damage decay' and 'combo reset' from another game in which the more hits the combo had the weaker each attack became (down to a *0.30 cap)

Could this also be used similarly? :O


Click to expand...

No, I don't think so. Resetting basic attacks just resets the attack timer in LoL. The atk speed method basically gives you a lot of attack speed to the point where it feels like a basic attack reset.

But if you're interested, I could probably whip up a spell similar to what you wanted. It just won't be too soon because I'm having such a lazy holiday.

#### ZethHolyblade

#### ZethHolyblade

 Level 11

JoinedMay 16, 2013Messages193

Resources: 9

#### Resources
Icons 3Skins 6

xYours Trulyx said:

 No, I don't think so. Resetting basic attacks just resets the attack timer in LoL. The atk speed method basically gives you a lot of attack speed to the point where it feels like a basic attack reset.

But if you're interested, I could probably whip up a spell similar to what you wanted. It just won't be too soon because I'm having such a lazy holiday.


Click to expand...

No worries, take your time. I'd welcome another mapper's take on war3 triggers.

The only way I can think of would be 'healing' the enemy unit after the basic attack has taken place in order to simulate damage decay.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Grammar is fine, the layout is solid. Reasonably short for its complexity. Example map exists.

Approved, with the addition that I added a disclaimer at the top because this tutorial is out of date as new natives exist to modify object editor fields ingame now.

But I think this might still be useful for those that run older versions.

 You must log in or register to reply here.


### Similar threads

Locked

 Spells as basic attacks


Mitzui

May 13, 2011

World Editor Help Zone

Replies2Views1K

May 13, 2011
-Kobas-

 Creating some basic abilities


paskovich

Apr 22, 2007

Trigger (GUI) Editor Tutorials
23

Replies113Views104K

May 26, 2021
McPhisto2051

Locked

 Basic attack spell


Klann

Nov 30, 2011

World Editor Help Zone

Replies6Views2K

Dec 4, 2011
Klann

[Mapping]
 Spell as basic attack


-Kobas-

May 13, 2011

Tutorial Archive

Replies7Views13K

Jul 17, 2011
PurgeandFire

Locked

 Give both ranged/melee splash damage of their basic attack damage


gurgamel

Jul 2, 2015

World Editor Help Zone

Replies6Views5K

Jul 2, 2015
Dr Super Good

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