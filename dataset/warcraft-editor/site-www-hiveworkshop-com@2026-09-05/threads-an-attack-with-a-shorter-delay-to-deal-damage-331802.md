---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/an-attack-with-a-shorter-delay-to-deal-damage.331802/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# An Attack with a Shorter Delay to Deal Damage
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/an-attack-with-a-shorter-delay-to-deal-damage.331802/>
#### AngelCraft

#### AngelCraft

 Level 8

JoinedMar 14, 2021Messages54

Resources: 2

#### Resources
Tutorials 2

 Several days ago I was looking for a method to make a dummy unit attack faster than 0.103 seconds (this happens if a unit's movement speed is 0 or it has the Root ability with false spin, also modifying the 0 value of Art - points damage, in a unit's item editor) so that the attack can deal damage to an attacked unit. But this does not happen in a unit with speed greater than 0 and that can rotate, since sometimes the attacks of this fictitious unit are usually less than 0.103s, reaching up to 0.003 seconds of delay to inflict damage.


Click to expand...

A simple explanation of an attack, of a dummy unit.

Most of the attack functions in the game are guided simply by the angle you need to launch an attack and the condition is that the attacking unit must be in front of the attacked unit. (Except for units whose movement speed is 0, "we won't use them"). If the dummy unit has an Art - Damage Point value of 0 and an instant attack, the delay time of a first attack that deals damage will be very low, and this can be measured with a timer. And the data they give are the following:

- They range from 0.003 seconds to about 0.34 seconds.

But as you know, these values cannot be controlled as the game gives quite random results within this range. The idea is to find a way to handle these time values.​

The method

According to the above, apparently the game gives random values, so using the following functions we can make it controllable and better yet, the much shorter interval.

 vJASS:

set dum = CreateUnit(GetOwningPlayer(source), OrbDummy_ID, GetUnitX(source)-50, GetUnitY(source), 0.00)
call UnitAddAbility(dum,MostSpeed_ID)
call UnitApplyTimedLife(dum, 'BTLF', 1)
call SetUnitPosition(dum,GetUnitX(targ)-50, GetUnitY(targ)-0.1)
set ang = Atan2(GetUnitY(targ)-GetUnitY(dum),GetUnitX(targ)-GetUnitX(dum))*bj_RADTODEG
call IssueTargetOrder(dum,"attack",targ)
call SetUnitFacingTimed(dum,ang,1)
set dum = null

As you can see, I am simply creating a dummy unit that will attack another unit. The difference is that there is an angle change via the SetUnitFacingTimed function, which allows a unit to rotate in a given time, but what happens here? Although this is a guess, apparently the game internally adjusts the firing angle of a unit attack, and the calculations for these are slow and therefore highly random. Using the SetUnitFacingTimed function helps with speed calculations to launch an attack and thus make the launch of an attack faster.

Now, for SetUnitFacingTimed to work well, it is necessary to readjust a new angle "and that the minimum turn time of the function must be greater than or equal to 0.1 seconds", for this we need to use a function to change the position of the attacking unit (you can also use SetUnitX / Y)

call SetUnitPosition (dum, GetUnitX (targ) -50, GetUnitY (targ) -0.1) , and at point "Y" I take a small distance of "-0.1".

After this, SetUnitFacingTimed will have to be based on a new angle due to the change in position of the dummy unit in front of the attacked unit.

So these are the lines of code for a unit to have a quick attack.

 vJASS:

call SetUnitPosition(dum,GetUnitX(targ)-50, GetUnitY(targ)-0.1)
set ang = Atan2(GetUnitY(targ)-GetUnitY(dum),GetUnitX(targ)-GetUnitX(dum))*bj_RADTODEG
call IssueTargetOrder(dum,"attack",targ)
call SetUnitFacingTimed(dum,ang,1)

The values that the dummy unit will give, between giving the order to attack and doing damage, will be "0.003 to 0.032" seconds. Most of the first attacks made on a unit will always give 0.032, or if the unit uses a missile type attack it will be "0.034" seconds maximum.​

Curiosities

- When I create a unit within an event like EVENT_PLAYER_UNIT_SPELL_EFFECT or similar, it is usually that the created dummy unit, if I order it to attack after a while, say 0.02 seconds (with a timer), the attack and damage interval will be 0.012 seconds. . , sometimes it doesn't happen. Note that for this to happen, the unit that casts a spell the Art - Casting Point must be 0. So far I can't find an explanation for this. "But if there is a way to handle this, it could give attacks down to values of 0.003 seconds minimum, it all depends on how long you wait to order an attack (the maximum wait for the attack order 0.029 seconds on the timer for the dummy), after having created a unit in those events. "(It also happens if in the same timer I create the unit, and order it to attack with the method).

- This method can be applied to all types of units even if their Art - Damage Point value is higher than 0. You can also add an ability that increases attack speed to 400% so that each first attack is faster. (This can be seen as an example in the map that I leave at the end of the post, which demonstrates this, this also works with illusions).

- There is a small chance that an attack of this delay time can be dodged, only if the attacked unit moves too fast, changing the angle that the dummy points to the attacked unit when turning, lengthening the delay interval. . . (This can be avoided by using a longer range for the dummies, so that the attack does not miss, but still the delay time of the attack will be slower)​

Uses in which this method can be applied

At this time, Blizzard does not include a feature that performs an instant attack while continuing to disrupt the current order of units. Therefore, this applies to attack-based systems such as Dota 1's Ember Spirit Panda, or also as a quick way to apply attack enhancements such as Ice Breath, Liquid Fire, and other situations where the effect of The orbs can be accumulated with arrows. etc.​

Credits

Thanks to WereElf for his Multishop system that made me realize that there was a method to shorten the attack time and also to Dr. Good, for the attack calculations that the game performs.

Note: It is possible that there is a different method than the one found that can be used for very fast attacks "like Pause / Unpause by anitarf that I couldn't find yet", but if it exists it could be published if they know about it. This method can be complemented with the leantrop that uses the arrow ability for instant attacks.

Click to expand...

I attached the test map with the implemented method and what can be done. I also include the WereElf Custom MultiShot system map altered a bit so Dummy Orbs units can attack faster. Also implemented is the BlzSetUnitAttackCooldown function, which makes the unit ignore the Damage Point and Cooldown of an Attack.​

#### Attachments

An Attack with a Plus Speed.w3x

 131.1 KB

 · Views: 157



Custom Multishot v2.2ca.w3x

 124.9 KB

 · Views: 129




 Last edited by a moderator: Apr 10, 2021

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I don't want to say this is useless but what is the proposed usage of this?

If you want to deal damage instantly to something you just use the deal damage to unit action (GUI).

If you want flavor, make unit play animation.

In old patches you could make the argument of wanting it to scale with the unit's current attack damage.

With new natives this is no longer a problem however.

The only scenario I could think of (in a few seconds x)) is if you want to trigger a "is attacked" event but this sounds like a solution to bad practice.

However, my possibly limited vision aside this is well written and structure/design is simple but clean so nothing to worry about on that front.

My only nitpick is that you mix an orange color and a yellow color for your titles for some reason.

#### AngelCraft

#### AngelCraft

 Level 8

JoinedMar 14, 2021Messages54

Resources: 2

#### Resources
Tutorials 2

Chaosy said:

 I don't want to say this is useless but what is the proposed usage of this?

If you want to deal damage instantly to something you just use the deal damage to unit action (GUI).

If you want flavor, make unit play animation.

In old patches you could make the argument of wanting it to scale with the unit's current attack damage.

With new natives this is no longer a problem however.

The only scenario I could think of (in a few seconds x)) is if you want to trigger a "is attacked" event but this sounds like a solution to bad practice.

However, my possibly limited vision aside this is well written and structure/design is simple but clean so nothing to worry about on that front.

My only nitpick is that you mix an orange color and a yellow color for your titles for some reason.


Click to expand...

A simple answer, Blizzard didn't give a feature to do instant real attacks until now.

We only now have functions to get values of a unit such as base damage and attack type, but even other functions are missing, such as obtaining the green and red bonus numbers, the current attack speed or events to detect when an attack is launched. attack or missile, also a way to do damage without generating alerts to the enemies, without this data it is still not possible to make a complete system of custom projectiles. Also the current versions have problems with old maps, and the new functions have one or another bug that is still not solved until now.

What I put here is a complement to expand the attack systems, since you can have attacks carried out up to a time of 0.003 seconds with all its benefits.

"And why of the colored titles", simply to visually separate one subject from another. (Is there a problem with this?)

Greetings ...

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

AngelCraft said:

 A simple answer, Blizzard didn't give a feature to do instant real attacks until now.


Click to expand...

Yeah I get that, my question was what it would be used for.

When I have made my maps I have never had the need for an instant attack where an unperceivable delay was ruining an idea.

Yes this is very subjective, I might have been lucky or just have a lacking imagination which is why I asked.

AngelCraft said:

 We only now have functions to get values of a unit such as base damage and attack type, but even other functions are missing, such as obtaining the green and red bonus numbers, the current attack speed or events to detect when an attack is launched. attack or missile, also a way to do damage without generating alerts to the enemies, without this data it is still not possible to make a complete system of custom projectiles. Also the current versions have problems with old maps, and the new functions have one or another bug that is still not solved until now.

What I put here is a complement to expand the attack systems, since you can have attacks carried out up to a time of 0.003 seconds with all its benefits.


Click to expand...

Fair, sure then if you want it for scaling purposes I can see it being useful.

AngelCraft said:

 "And why of the colored titles", simply to visually separate one subject from another. (Is there a problem with this?)


Click to expand...

NoIjustlikeconsistency.

Wont force you to change it.

#### AngelCraft

#### AngelCraft

 Level 8

JoinedMar 14, 2021Messages54

Resources: 2

#### Resources
Tutorials 2

Chaosy said:

 When I have made my maps I have never had the need for an instant attack where an unperceivable delay was ruining an idea.

Yes this is very subjective, I might have been lucky or just have a lacking imagination which is why I asked.


Click to expand...

Actually this method is for the use of projectile systems as I mentioned in the main post or rapid attacks. Usually most use projectile libraries, but these have certain requirements to function, such as the unit having an instant attack type. (So this could help these systems or other types of utilities, while we wait for Clasico to implement new functions for attacks).

There is still a method (I guess) to do an instant attack, a trick made by Anitarf commented on by Zwiebelchen that I have been looking for but can't find such a method, it just mentions that it exists (apparently). Anyway this is other method ¯\.㋡./¯.

Greetings...


 Last edited: Apr 11, 2021

 You must log in or register to reply here.


### Similar threads

[Spell]
 I don't want knockback-effect to affect buildings or magic immunes.


Dinodin

Apr 13, 2026

World Editor Help Zone

Replies2Views184

Apr 13, 2026
Dinodin

[JASS/AI]
 A very fast attack. Using the change of an attack index.


AngelCraft

Sep 4, 2022

Tutorial Submission

Replies11Views5K

Sep 22, 2022
fenix140

Locked

 Make Paralyzing Cask spell to only bounce to organic units.


Dinodin

Oct 11, 2023

World Editor Help Zone

Replies8Views1K

Oct 12, 2023
Dinodin

Locked

[JASS]
 Item various Problems


Flame_Phoenix

Sep 1, 2007

Triggers & Scripts

Replies38Views3K

Sep 9, 2007
Flame_Phoenix

Locked

[vJASS]
 Lag problem


Galdr

Oct 29, 2012

Triggers & Scripts

Replies4Views3K

Oct 30, 2012
edo494

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