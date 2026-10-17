---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/dps-damage-per-second-calculation-for-dummies.262029/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# DPS Damage Per Second Calculation for Dummies
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/dps-damage-per-second-calculation-for-dummies.262029/>
#### Mr.Pockets00000

#### Mr.Pockets00000

 Level 9

JoinedJan 1, 2010Messages197

Resources: 15

#### Resources
Skins 2Maps 10Spells and Systems 1Tutorials 2

This is a very basic and straightforward was to calculate DPS. You could use these formulas to create a DPS program if you wanted.


Let's start with some important assumptions


*Assume:

Normal Attack/Armor Type

No evasion

No armor(defense= 0, damage reduction= 0%) or other damage reducing mediums


Now given this sample information:

Attack Cooldown= 0.75

Attack Damage Base= 9

Number of Dice= 1

Number of Sides per Die= 1


This is a very basic attack, that is guaranteed to deal 10-10 damage each attack.


Let's look at what we have so far:


We have a unit that does 1 attack every 0.75 seconds


That's 0.75 seconds per 1 attack or 3/4 seconds/attack.


But we need attacks per second for DPS calculation.


So that's attacks/second.


Notice how seconds/attackis the inverse of attacks/second.


So now we have to flip our fraction of 0.75/1 over to 1/0.75.

Do some math reduction:


1/0.75= 1/(3/4)= 1*4/3= 4/3 attacks/second


So guess how we get dps now?


Damage is reallydamage per attack ordamage/attack.


(damage/attack)*(attacks/second) do your cancellation and bada-bing bada-boom (damage/second)


So we're dealing 10 damage per attack.


So we need to take 10 damage * 4/3 attack/second


And watch the magic happen:

40/3 DPS!!!!


Do some long division and presto:


13.3333333--------->(goes on forever dog)


or 13 1/3 dps

Calculating Damage Actually Done in 2 seconds:


Take our previous problem, and let's say our unit lived for 2 seconds in a fight and was attacking the entirety of those 2 seconds.


To get the actual damage done, you'll need to multiple DPS*2


So:

4/3*10*2 right?

WRONG


To get the damage actually done, you need to take in account that it's impossible to complete a partial attack. IE, you cannot do 1/3 of an attack.


So 4/3*10*2 won't work

OMG DO WE GIVE UP?!

NO!


So we know that 4/3 attacks/second * 10 seconds will give us a fractional number of attacks, which is impossible, as you cannot complete a fractional amount of attacks.


So take 4/3 * 10 and get 13 and 1/3 attacks. You now have to drop the 1/3 to maintain correctness.


And:


4/3*10*2= 26 and 2/3


Where:


13*2= only 26.


Do you see how you're off by 2/3 if you don't account for this?

Minimum, Maximum, and Average DPS:


Now given this sample information:

Attack Cooldown= 0.75

Attack Damage Base= 9

Number of Dice= 1

Number of Sides per Die= 2


This gives us the same attack as above, except now with 10-11 damage.


You can find minimum DPS by using 10 as your damage value, then maximum DPS by using 11 as your damage value.

10 (damage/attack) * 4/3 (attack/second) = 13 1/3 minimum dps

11 (damage/attack) * 4/3 (attack/second)= 14 2/3 maximum dps


Now this is cool, but you'll probably confuse the user by displaying excessive numbers like this. You're best off taking the minimum and maximum dps and dividing them by 2, giving us the average dps.


13 1/3 + 14 2/3 = 28


28/2= 14 average dps <-------- display this to to the user, It'll make their life easier and yours too. It should reduce the questions like WTF IS THIS MIN AND MAX SHIT MAN!!!!!!


*These are all things that interrupt normal damage flow. If you use different armor types, evasion, and different armor amounts, you'll have to alter your dps calculation accordingly.


 Last edited by a moderator: Sep 9, 2015

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

DPS is normally referenced as average damage per second per duration of spell or attack so the first equation where you get 26.66_ is correct compared to the second equation where you get 20.

Also your title is a little offensive to anyone that needs to look at this tutorial. It is a bad title. Changing it to beginners is more preferred and less offensive.

If this is to be a tutorial it should also cover how to determine min and max DPS when you have several number of dice with several sides.

Edit:

After looking a bit more your DPS is off. It should be 12 DPS with .75 speed and 9 damage.

1 / 0.75 * 9 = 12 not 13.33_

You can also do the above by 9 * 4 attacks since there are 4 attacks in 3 seconds. Then divide 36 by 3 and you get 12 to do the second check on your math.

Also having the times 2 is misleading as that would not be DPS that would be DP2S.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

the truncation is bad idea, because sure it works for you in this special case, but if I extend the time I measure your dps over 10 seconds for instance, then suddenly I have huge error if I consider 1 attack instead of 4/3 attacks per second

#### Mr.Pockets00000

#### Mr.Pockets00000

 Level 9

JoinedJan 1, 2010Messages197

Resources: 15

#### Resources
Skins 2Maps 10Spells and Systems 1Tutorials 2

Base damage 9 with dice 1 and 1 makes it 10


------


You don't cut out the fraction until 4/3 is multiplied by the number of seconds, and that is to find damage actually done, not DPS in that case


------

deathismyfriend said:

 DPS is normally referenced as average damage per second per duration of spell or attack so the first equation where you get 26.66_ is correct compared to the second equation where you get 20.


Also your title is a little offensive to anyone that needs to look at this tutorial. It is a bad title. Changing it to beginners is more preferred and less offensive.


If this is to be a tutorial it should also cover how to determine min and max DPS when you have several number of dice with several sides.


Edit:


After looking a bit more your DPS is off. It should be 12 DPS with .75 speed and 9 damage.


1 / 0.75 * 9 = 12 not 13.33_


You can also do the above by 9 * 4 attacks since there are 4 attacks in 3 seconds. Then divide 36 by 3 and you get 12 to do the second check on your math.


Also having the times 2 is misleading as that would not be DPS that would be DP2S.


Click to expand...


read that more closely


 Mr.Pockets00000 said:



Number of Dice= 1

Number of Sides per Die= 1


This is a very basic attack, that is guaranteed to deal 10-10 damage each attack.


Click to expand...


 Last edited by a moderator: Mar 8, 2015

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

you do not need to post 3 times, you can just post it all in one post...

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

This is useful information for today game programming, but the tuts looks too boring. Add some color wont hurts (not colorful)

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

your calculation is still invalid, since what you do is

floor(attack_per_second) * damage * seconds, and you should do floor(attack_per_second * seconds) * damage.

Because with your way, when I want to calculate unit's dps with 4/3 attacks per second over 10 seconds, it will say 1*10*10 which is 100. With my corrected calculation it is 130, because you will only hit your enemy 13 and 1/3 times in 10 seconds, whereas with your formula you will hit him 10 times.

This is why I said the formula is incorrect and only fitting your scenario.

#### Mr.Pockets00000

#### Mr.Pockets00000

 Level 9

JoinedJan 1, 2010Messages197

Resources: 15

#### Resources
Skins 2Maps 10Spells and Systems 1Tutorials 2

edo494 said:

 your calculation is still invalid, since what you do is

floor(attack_per_second) * damage * seconds, and you should do floor(attack_per_second * seconds) * damage.

Because with your way, when I want to calculate unit's dps with 4/3 attacks per second over 10 seconds, it will say 1*10*10 which is 100. With my corrected calculation it is 130, because you will only hit your enemy 13 and 1/3 times in 10 seconds, whereas with your formula you will hit him 10 times.

This is why I said the formula is incorrect and only fitting your scenario.


Click to expand...

In this situation you're correct. I didn't mean to say you always drop the fractional part, but what is true which you must recognize is this 10 seconds * 4/3 attacks/second = 13 & 1/3 attacks. When you're calculating the damage that was ACTUALLY DONE IN THAT TIME (10 SECONDS) you must chop off the 1/3 because you CANNOT do 1/3 a attack.

When you're calculating DPS, you can leave the fraction but when you're calculating damage done over given time you must drop the fraction or you're saying it's possible to do 1/3 a attack.

So yes for your equation its 4/3 * 10seconds * 10 damage. 4/3 * 10 is 13 and 1/3, but you have to drop the 1/3 or you get a damage value that's impossible in exactly 10 seconds. (you'd get that you did 133 rather than 130, that's an error of 3 damage that you didn't actually do)

Remember:

(Attacks/Second) * (Seconds) = Attacks (cannot be fractional, must be rounded down)

Edit: I actually had it slightly worded wrong before, correcting now

Edit: Sorry I didn't know what I was thinking, I had dropped the fractional part at the wrong moment.

edo494 said:

 you do not need to post 3 times, you can just post it all in one post...


Click to expand...

correct, my apologies


 Last edited: Jan 29, 2015

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

There is no such thing as "minimum" or "maximum" DPS. DPS is always an average, as it's a blind estimation based on your stats under the assumption of an infinitely long fight.

That's why truncating the DPS for incomplete attacks is also bull. You want to take incomplete attacks into account, because no fight only goes 1 second and the truncated value is of no practical use.

Plus, your calculation - even if it would matter, is wrong anyway, as the first attack comes instantly and doesn't take the attack cooldown into account.

Plus, who cares about theoretical DPS anyway? I was expecting this tutorial to explain how to build a classic DPS meter. The basics for that should cover:

1) How to detect when combat starts

2) How to keep track of the total damage dealt of each unit (using a unit indexer and a damage detection engine)

3) How to properly count the combat time for each unit (note: looping through an array, incrementing integers for each unit is highly ineffective - better solution is to count up a global timer and only store the time of combat entry for each unit, then take the difference to the combat entry to get the current combat duration)

4) Methods of displaying the DPS as total damage dealt / combat time (for example, showing DPS of currently selected unit in a multiboard or using floating texts)

Now that would be a useful tutorial. But what you submitted is useless garbage.

#### edo494

#### edo494

 Level 23

JoinedApr 16, 2012Messages4,041

Resources: 6

#### Resources
Spells and Systems 2JASS 4

if we want to go to precise levels, you should also account for the animation time it takes to hit your enemy, unless that is already reduced by wc3 with attack speed.

#### Mr.Pockets00000

#### Mr.Pockets00000

 Level 9

JoinedJan 1, 2010Messages197

Resources: 15

#### Resources
Skins 2Maps 10Spells and Systems 1Tutorials 2

Zwiebelchen said:

 There is no such thing as "minimum" or "maximum" DPS. DPS is always an average, as it's a blind estimation based on your stats under the assumption of an infinitely long fight.

That's why truncating the DPS for incomplete attacks is also bull. You want to take incomplete attacks into account, because no fight only goes 1 second and the truncated value is of no practical use.

Plus, your calculation - even if it would matter, is wrong anyway, as the first attack comes instantly and doesn't take the attack cooldown into account.

Plus, who cares about theoretical DPS anyway? I was expecting this tutorial to explain how to build a classic DPS meter. The basics for that should cover:

1) How to detect when combat starts

2) How to keep track of the total damage dealt of each unit (using a unit indexer and a damage detection engine)

3) How to properly count the combat time for each unit (note: looping through an array, incrementing integers for each unit is highly ineffective - better solution is to count up a global timer and only store the time of combat entry for each unit, then take the difference to the combat entry to get the current combat duration)

4) Methods of displaying the DPS as total damage dealt / combat time (for example, showing DPS of currently selected unit in a multiboard or using floating texts)

Now that would be a useful tutorial. But what you submitted is useless garbage.


Click to expand...

Zwiebelchen said:

 There is no such thing as "minimum" or "maximum" DPS. DPS is always an average, as it's a blind estimation based


Click to expand...

DPS is not blind estimation. You have a bad definition of DPS. Blind estimation would be YEH KNOW ITS GOT LIEK 100 DAMAGE. I BET THAT MEANS IT DOES BIG DAMAGE EVERY SECOND  I'm actually trying to do things as objectively as I possibly can.

Zwiebelchen said:

 That's why truncating the DPS for incomplete attacks is also bull.


Click to expand...

I didn't truncate damage for DPS, I truncated damage for DAMAGE ACTUALLY DONE IN X TIME. If you think that Damage PER second or Damage/Second is the same as DAMAGE ACTUALLY DONE IN X TIME then your understanding of unit analysis is too weak. They're close but not the same. Damage per second is Damage everyone one second (theoretical). Real Damage Done is what actually happened. IE, if the attack never happend, the damage never happened. Ever tried initiating an attack and then killing the unit before the attack is even finished? What happens? The damage is cancelled.

edo494 said:

 if we want to go to precise levels, you should also account for the animation time it takes to hit your enemy, unless that is already reduced by wc3 with attack speed.


Click to expand...

Thank you for displaying an actual argument. That's something that I should have ventured into deeper. I agree, I overlooked that for example a footman takes about 0.3 seconds to launch an attack. That's very relevant to if the attack will be completed or not.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Mr.Pockets00000 said:

 DPS is not blind estimation. You have a bad definition of DPS. Blind estimation would be YEH KNOW ITS GOT LIEK 100 DAMAGE. I BET THAT MEANS IT DOES BIG DAMAGE EVERY SECOND  I'm actually trying to do things as objectively as I possibly can.


I didn't truncate damage for DPS, I truncated damage for DAMAGE ACTUALLY DONE IN X TIME. If you think that Damage PER second or Damage/Second is the same as DAMAGE ACTUALLY DONE IN X TIME then your understanding of unit analysis is too weak. They're close but not the same. Damage per second is Damage everyone one second (theoretical). Real Damage Done is what actually happened. IE, if the attack never happend, the damage never happened. Ever tried initiating an attack and then killing the unit before the attack is even finished? What happens? The damage is cancelled.


Click to expand...


DPS is damage per second. Since the second is an arbitrary time value used for damage done in a second not a specific second then it is average damage per second real damage per second.


You should also take into account that you need to have each units attack speed which (correct if wrong) WC3 does not have an action to get a units attack speed. That means that every unit will have to have its attack speed calculated. This can be done using a system which for your RDPS you should show them how to make the system to make this tutorial even worth using.


You can also show them how to make several small systems for calculating DPS several different ways.


If you try to make this tutorial better then I believe others will help you to do so. As it is now I can't see it getting approved.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

The tutorial itself is interesting, but there isn't much the average user can do with the information. Perhaps they can make a DPS meter, but most of those use real-time attacks rather than just displaying DPS. But I suppose this tutorial is good for balancing. Overall, this tutorial could use some structure so that the user knows exactly what he will get out of it and what he can do with the information (usually this is satisfied with an introduction).

That doesn't mean this tutorial doesn't warrant approval--but I feel like it could be expanded. Personally, I would like to know more about the subtleties of attacks. Edo made a good suggestion, could you look into defining how attacks behave in wc3 and seeing if it affects dps in any way? And while it is useful to consider the 0 armor case, it may be good to have a small section detailing what one would do in the case where units have armor. It may help a user balance their map's units.

#### sentrywiz

S

#### sentrywiz

As @deathismyfriend said, the title is offensive. I don't know how to calculate DPS, that doesn't make me a retard.

Idk, maybe I'm just a "retard" but I didn't understand anything from this tutorial.

Because by reading it, it sounds more like the author is boasting than giving us "retards" a concise way to calculate dps.

The tutorial is a mess, calculation after calculation without explaining why. Like flipping fractions and math reduction. Reduction? I never heard that term. Subtraction or division maybe.

I vote for this to be completely re-written. It looks more of a "look how smart i am" rather than trying to help... us... retards

Also, offensive title.


 Last edited by a moderator: Sep 3, 2015

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I've changed the title.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

It has been a while and I still think this tutorial needs some expansion for it to be approved. I'm going to graveyard it for now. Feel free to PM me if you'd like to make changes to it.

 You must log in or register to reply here.


### Similar threads

Locked

 Regarding DPS


priest170234

Dec 31, 2010

World Editor Help Zone

Replies3Views1K

Dec 31, 2010
Maker

Locked

 How To Determine DPS


Rispetto

Jun 22, 2012

Gamer's Hub

Replies0Views2K

Jun 22, 2012
Rispetto

Locked

 DPS And Attack Animation Damage Point


Melth

Jan 19, 2014

World Editor Help Zone

Replies4Views9K

Jan 19, 2014
Meticulous

Locked

 Dps


Vercas

Jan 3, 2010

World Editor Help Zone

Replies6Views1K

Jan 4, 2010
M0RT

Locked

 Spell


Lembidi

Oct 12, 2011

Requests

Replies7Views2K

Oct 18, 2011
Kazeon

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