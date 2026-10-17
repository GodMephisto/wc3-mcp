---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/units-damage.5895/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Units Damage
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/units-damage.5895/>
#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


Introduction


When setting the damage of a unit, there are three values which makes peoples mad cause they don't understand. Or atleast I didn't understand them when I started making maps. These three values are called: Base Damage, Number of Dice and Sides per Die.

Example one


If you want to set the damage of a unit, let's say we want to set the damage to:

Minimum Damage: 5


Maximum Damage: 50


Then you will need to edit the Base Damage, Number of Dice and the Sides per Die.


The values will be:

Damage Base = 4


Damage Number of Dice = 1


Damage Sides per Die = 45


There are always different ways of setting the damage. You can also do this:

Damage Base = 0


Damage Number of Dice = 5


Damage Sides per Die = 10


This gives the same result as the first example.

The formula


Minimum Damage = Damage Base + Damage Number of Dice


Maximum Damage = Damage Base + (Damage Number of Dice x Damage Sides per Die)

Note


I am not exactely sure what the Number of Dice do, but letting it be 1 all the time will not hurt you.


If your World Editor can't set the damage to a value high enough, use World Editor Unlimited, you can get it in the Download Section.

#### Dodokiller88

D

#### Dodokiller88

Number of Dice determines the number of hypothetical die. In order to incorporate this feature, one uses the formula:


Maximum Damage = Base Damage + (Sides per Die x Number of Die)


and


Minimum Damage = Base Damage + Number of Die

#### Darkdraak

#### Darkdraak

 Level 5

JoinedMar 11, 2006Messages195

Its pretty easy, base damage is damage without dice.


Number of dice is the number of dice that will be thrown, and sides per dice are the number of sides each dice will have. Number of dice can be used for not lineair damage lines. For example throwing with 2 six-sided dices have a small chance on a 2 or a 12 but a big chance on 7. 3 eight-sided dices have a small change on 3 or 24 but a big chance of throwing 10 or 11.


Which can be used to have a random damage around a certain amount. (damn math terms in dutch are different from english)

#### Shados

#### Shados

 Level 18

JoinedJan 24, 2006Messages1,938

Resources: 4

#### Resources
Maps 1Tutorials 3

Darkdraak has i right, it's loosely based on the DnD damage system, using different sided dice.

#### Daelin

#### Daelin

 Level 11

JoinedJul 20, 2004Messages2,759

Resources: 50

#### Resources
Models 22Icons 5Tutorials 23

This could simply go in a FAQ. I'll take such a FAQ in consideration and maybe add it in the general mapping section (about Object Editing).


~Daelin

#### MetalWarrior

#### MetalWarrior

 Level 9

JoinedJul 14, 2006Messages301

Resources: 7

#### Resources
Maps 7

Did you ever played D&D?


Well it don't really matter but if you did you may be sure to know that there are Dice with more or less than 6 faces.


Now the equation as I see it:


Damage = (Damage Base) + (Number of Dice*Sides per Dice)


Sides per Die = Random(Number of Face)


So... If I do that:


Damage Base = 49


Number of Dice = 1


Sides per Die = 6


My damage will be 50-55


Damage Base = 95


Number of Dice = 5


Sides per Die = 11


My damage will be 100-150


Damage Base = 24


Number of Dice = 1


Sides per Die = 1


My damage will be 25-25


You understand?

#### Shados

#### Shados

 Level 18

JoinedJan 24, 2006Messages1,938

Resources: 4

#### Resources
Maps 1Tutorials 3

Dude, it's already thoroughly explained and I even made a reference to DnD.


-*meow*

#### LordPerenolde II

#### LordPerenolde II

 Level 23

JoinedNov 20, 2005Messages1,295

Resources: 5

#### Resources
Models 1Maps 4

Hello. I'm sorry for necroposting.

I'd like to deeper explore the formula of damage in warcraft 3 because it seems I didn't get it quite enough.

I was going to update my campaign and rework some damage stats.

I was testing around with a couple of units. They are the same unit but one deals double the damage stats of the other.

Original

Damage Base = 4

Number of Dice = 2

Sides per Die = 4

My damage will be 6-12

Doubled

Damage Base = 10

Number of Dice = 2

Sides per Die = 7

My damage will be 12-24

While testing it out, though, I found out that the one with increased damage seems to inflict way more damage than expected. Is it because of the Damage Base of 10? Because I found out that the same final number may come by different data imput.

For example this

Damage Base = 8

Number of Dice = 4

Sides per Die = 4

My damage will still be 12-24

I was watching the latest @Jayborino playthrough video about my Wc2 remake and I found out that Balistas with 1 upgrade can mow down a Cannon Tower in 3, maybe 2 shots. I can't frankly recall if it was the same in the original game. Seemed such only with a full 2 times upgrade.

If that's the case then how can I exactly calculate and recreate the same damage of the original Warcraft 2 units into Warcraft 3? This has always been an aspect which I kinda overlooked, because all I originally did in Warcraft 1 was to test "how many hits it takes with unit X to kill unit Y" and so on. Something which I did too during Warcraft 2 remake development. But what if I can improve that even more?

At this time I feel I haven't really understood how Warcraft 3 damage system works.

 You must log in or register to reply here.


### Similar threads

 Hero Damage system


Rizz_Boomer

Jun 6, 2007

Tutorial Archive

Replies1Views2K

Jun 25, 2007
Wolverabid

[Misc]
 DPS (Damage Per Second) Calculation (For Dummies)


Mr.Pockets00000

Jan 28, 2015

Tutorial Archive

Replies15Views12K

Jun 4, 2018
PurgeandFire

Locked

 Hero's Damage


ukn0wnD3str0y3r

Aug 9, 2014

World Editor Help Zone

Replies4Views2K

Aug 10, 2014
ukn0wnD3str0y3r

 Damage Calculator, the Reverse


supertoinkz

Sep 20, 2009

Misc/General Mapping Tutorials

Replies20Views10K

Jul 2, 2010
supertoinkz

 How to Balance Damage


splashy5

Feb 15, 2008

Tutorial Archive

Replies1Views1K

Feb 15, 2008
PurplePoot

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