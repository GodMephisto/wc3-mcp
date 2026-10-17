---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-create-objects-with-angle-and-distance-gui-jass.241367/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Create Objects with Angle and Distance GUI JASS
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-create-objects-with-angle-and-distance-gui-jass.241367/>
#### nhocklanhox6

#### nhocklanhox6

 Level 18

JoinedFeb 12, 2012Messages414

Resources: 31

#### Resources
Models 7Spells and Systems 23Tutorials 1

 Table of contents


•What you need

•Ready

•Start

-----------------

•Get Angle

•Get Distance

•Get smallest distance and count

Have you ever met this situation ?...

Sometimes, you're doing something, and to complete it, you need to create an arrow (Just example) but you don't know how to create it and you are lazy to create one by one..

I have a solution to help you:

So...

How to turn this:

Into this:

I/ What you need

So, you need to generate points and turn it into the shape you want

Like this:

You can use anything you want not necessarily the rect. Just know that the point xy.

II/ Ready

Well, you must remember two thing: Angle and Distance, i don't want to tell you what is that..

NOW EXAMPLE:

Suppose i have 4 rect:

• Rect 1: gg_rct_1 (left point)

• Rect 2: gg_rct_2 (up point)

• Rect 3: gg_rct_3 (right point)

• Rect 4: gg_rct_4 (down point)

A/ Get angle

Well, You need to know the angle between the two points for each, but how ?, here's your answer:

-JASS-

local real a = Atan2(GetRectCenterY(gg_rct_4) - GetRectCenterY(gg_rct_2), GetRectCenterX(gg_rct_4) - GetRectCenterX(gg_rct_2))

-GUI-

Set LEAKS = (Center of 2 <gen>)

Set LEAKS1 = (Center of 4 <gen>)

Set Angle = (Angle from LEAKS to LEAKS1)
In this code: you see gg_rct_2 and gg_rct_4, gg_rct_2 is the main point you use for all code to make this arrow.

You're done !

B/ Get distance

You must get the distance between two point too, like this:

-JASS-

local real dx = GetRectCenterX(gg_rct_4) - GetRectCenterX(gg_rct_2)
local real dy = GetRectCenterY(gg_rct_4) - GetRectCenterY(gg_rct_2)
local real distance = SquareRoot(dx * dx + dy * dy)

-GUI-

Set Distance = (Distance between LEAKS and LEAKS1)
B/ Get smallest distance and count

IMPORTANT STAGE: You must have count of <effect,unit or stuff> you using to make this arrow, for example, i want to create 20 effect, like this:

-JASS-

local integer i = 20

-GUI-

Set Count = 20
Now you must get the smallest distance, like this

-JASS-

local real logic = distance/i

-GUI-

Set MinDistance = (Distance / (Real(Count)))
Now you have the smallest distance , you're done !!!!

C/ START

Now you must write the loop, like this:

-JASS-

 JASS:

local real d = 0.
local real cos = Cos(a)
local real sin = Sin(a)
loop
 exitwhen i == 0
 set x = GetRectCenterX(gg_rct_2) + d * cos
 set y = GetRectCenterY(gg_rct_2) + d * sin
 call AddSpecialEffect("Abilities\\Spells\\Human\\FlameStrike\\FlameStrikeDamageTarget.mdl",x,y)
 set d = d + logic
 set i = i - 1
 endloop

-GUI-

For each (Integer A) from 1 to Count, do (Actions)
Loop - Actions
Set DistanceInc = (DistanceInc + MinDistance)

Special Effect - Create a special effect at (LEAKS offset by DistanceInc towards Angle degrees) using Abilities\Spells\Human\FlameStrike\FlameStrikeDamageTarget.mdl

Custom script: call RemoveLocation(udg_LEAKS1)
with

"Abilities\\Spells\\Human\\FlameStrike\\FlameStrikeDamageTarget.mdl" is string model of effect i want to use, d is value increases every i subtract, a is Angle you did the previous.

Here is the results

-JASS-

 JASS:

local integer i = 20
 local real x
 local real y
 local real dx = GetRectCenterX(gg_rct_4) - GetRectCenterX(gg_rct_2)
 local real dy = GetRectCenterY(gg_rct_4) - GetRectCenterY(gg_rct_2)
 local real distance = SquareRoot(dx * dx + dy * dy)
 local real d = 0.
 local real logic = distance/i
 local real a = Atan2(GetRectCenterY(gg_rct_4) - GetRectCenterY(gg_rct_2), GetRectCenterX(gg_rct_4) - GetRectCenterX(gg_rct_2))
 local real cos = Cos(a)
 local real sin = Sin(a)
 //First arrow
 loop
 exitwhen i == 0
 set x = GetRectCenterX(gg_rct_2) + d * cos
 set y = GetRectCenterY(gg_rct_2) + d * sin
 call AddSpecialEffect("Abilities\\Spells\\Human\\FlameStrike\\FlameStrikeDamageTarget.mdl",x,y)
 set d = d + logic
 set i = i - 1
 endloop

-GUI-

Actions

Set DistanceInc = 0.00

Set LEAKS = (Center of 2 <gen>)

Set LEAKS1 = (Center of 4 <gen>)

Set Angle = (Angle from LEAKS to LEAKS1)

Set Distance = (Distance between LEAKS and LEAKS1)

Set Count = 20

Set MinDistance = (Distance / (Real(Count)))

Custom script: call RemoveLocation(udg_LEAKS1)

For each (Integer A) from 1 to Count, do (Actions)
Loop - Actions
Set DistanceInc = (DistanceInc + MinDistance)

Set LEAKS1 = (LEAKS offset by DistanceInc towards Angle degrees)

Special Effect - Create a special effect at LEAKS1 using Abilities\Spells\Human\FlameStrike\FlameStrikeDamageTarget.mdl

Custom script: call RemoveLocation(udg_LEAKS1)

Custom script: call RemoveLocation(udg_LEAKS)

And you do the same with the gg_rct_1 and gg_rct_3

And you will get the arrow like me ^^!

So if the angle and distance change in game, what can i do ??.....

You must do the same thing like that ...

-JASS-

 JASS:

function Trig_Untitled_Trigger_001_Actions takes nothing returns nothing
 local real x = GetUnitX(GetTriggerUnit())
 local real y = GetUnitY(GetTriggerUnit())
 local real o = GetUnitX(GetSpellTargetUnit())
 local real z = GetUnitY(GetSpellTargetUnit())
 local real dx = x-o
 local real dy = y-z
 local real distance = SquareRoot(dx * dx + dy * dy)
 local real angle = Atan2(z - y, o - x)
 local integer i = 20
 local real min = distance/i
 local real m = 0.
 local real cos = Cos(angle)
 local real sin = Sin(angle)
 loop
 exitwhen i < 0
 set m = m + min
 set o = x+m*cos
 set z = y+m*sin
 call AddSpecialEffect("Abilities\\Spells\\Other\\BreathOfFire\\BreathOfFireDamage.mdl",o,z)
 set i = i - 1
 endloop
endfunction

function a takes nothing returns boolean
 return GetSpellAbilityId() == 'A000'
endfunction

//===========================================================================
function InitTrig_JASS takes nothing returns nothing
 set gg_trg_JASS = CreateTrigger( )
 call TriggerRegisterAnyUnitEventBJ( gg_trg_JASS, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition(gg_trg_JASS,Condition(function a))
 call TriggerAddAction( gg_trg_JASS, function Trig_Untitled_Trigger_001_Actions )
endfunction

-GUI-

Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Special Effect - GUI [W]

Actions
Set Point = (Position of (Triggering unit))

Set Point1 = (Position of (Target unit of ability being cast))

Set Angle = (Angle from Point to Point1)

Set Distance = (Distance between Point and Point1)

Set Count = 20

Set Min = (Distance / (Real(Count)))

Set M = 0.00

Custom script: call RemoveLocation(udg_Point1)

For each (Integer A) from 1 to Count, do (Actions)
Loop - Actions
Set M = (M + Min)

Set Point1 = (Point offset by M towards Angle degrees)

Special Effect - Create a special effect at Point1 using Abilities\Spells\Other\BreathOfFire\BreathOfFireDamage.mdl

Custom script: call RemoveLocation(udg_Point1)

Custom script: call RemoveLocation(udg_Point)

And the results is:

#### Attachments

untit226.jpg

 48.2 KB

 · Views: 260



untit227.jpg

 31.9 KB

 · Views: 238



untit228.jpg

 34 KB

 · Views: 261



untit229.jpg

 17.9 KB

 · Views: 228



untitl32.jpg

 46.6 KB

 · Views: 239



untitl34.jpg

 18.1 KB

 · Views: 257



Arrow GUI.w3x

 17.6 KB

 · Views: 201



Arrow.w3x

 17.2 KB

 · Views: 193



Dis and Ang GUI and JASS.w3x

 18.3 KB

 · Views: 187




 Last edited by a moderator: Feb 23, 2014

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

you should detach your signature, onee-san

nice tutorial

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

I would use variables for the Sin and Cos components since they are constant.

#### Ofel

#### Ofel

 Level 13

JoinedMar 29, 2012Messages542

Resources: 14

#### Resources
Spells and Systems 14

[rainbow]Nice![/rainbow]

+Rep

#### nhocklanhox6

#### nhocklanhox6

 Level 18

JoinedFeb 12, 2012Messages414

Resources: 31

#### Resources
Models 7Spells and Systems 23Tutorials 1

GUI updated

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

Nice onee-san, but why don't you make it leakless?

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Don't use integer A/B.

Fix leaks.

It is a nice tutorial though.

There is one thing i would suggest. That is showing people how to make it using a point that can change. Example : center of unit. Then make full arrow from the point of the unit.

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

I'm bad at math, which is why I want to know where you got the constant 0.01747 from? EDIT: Oh I get it now, it's conversion to radians. Sorry

Also since this is creating a straight line, you could store the computed values on the loop and just increment it.

 JASS:

 local integer i = 20
 local real x
 local real y
 local real dx = GetRectCenterX(gg_rct_4) - GetRectCenterX(gg_rct_2)
 local real dy = GetRectCenterY(gg_rct_4) - GetRectCenterY(gg_rct_2)
 local real distance = SquareRoot(dx * dx + dy * dy)
 local real d = 0.
 local real logic = distance/i
 local real a =Atan2(GetRectCenterY(gg_rct_4) - GetRectCenterY(gg_rct_2), GetRectCenterX(gg_rct_4) - GetRectCenterX(gg_rct_2))
 //First arrow

 set dx=logic*Cos(a)
 set dy=logic*Sin(a)

 loop
 exitwhen i == 0
 set x = x+dx
 set y = y+dy
 call AddSpecialEffect("Abilities\\Spells\\Human\\FlameStrike\\FlameStrikeDamageTarget.mdl",x,y)
 set i = i - 1
 endloop

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

once you master loops this is actually very easy.

#### nhocklanhox6

#### nhocklanhox6

 Level 18

JoinedFeb 12, 2012Messages414

Resources: 31

#### Resources
Models 7Spells and Systems 23Tutorials 1

Updated.

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

Remove the conversion, you don't need it. You're using Atan2, which returns values in radians.

#### nhocklanhox6

#### nhocklanhox6

 Level 18

JoinedFeb 12, 2012Messages414

Resources: 31

#### Resources
Models 7Spells and Systems 23Tutorials 1

chobibo said:

 Remove the conversion, you don't need it. You're using Atan2, which returns values in radians.


Click to expand...


It won't work as well if i remove it (Tested)

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

You must be kidding right?

 JASS:

function Trig_Arrow_Actions takes nothing returns nothing
 local integer i = 20
 local real x
 local real y
 local real dx = GetRectCenterX(gg_rct_4) - GetRectCenterX(gg_rct_2)
 local real dy = GetRectCenterY(gg_rct_4) - GetRectCenterY(gg_rct_2)
 local real distance = SquareRoot(dx * dx + dy * dy)
 local real d = 0.
 local real logic = distance/i
 local real a =Atan2(GetRectCenterY(gg_rct_4) - GetRectCenterY(gg_rct_2), GetRectCenterX(gg_rct_4) - GetRectCenterX(gg_rct_2))
 //First arrow
 loop
 exitwhen i == 0
 set x = GetRectCenterX(gg_rct_2) + d * Cos(a)
 set y = GetRectCenterY(gg_rct_2) + d * Sin(a)
 call AddSpecialEffect("Abilities\\Spells\\Human\\FlameStrike\\FlameStrikeDamageTarget.mdl",x,y)
 set d = d + logic
 set i = i - 1
 endloop
 //Seconds arrow
 set i = 10
 set d = 0.
 set dx = GetRectCenterX(gg_rct_1) - GetRectCenterX(gg_rct_2)
 set dy = GetRectCenterY(gg_rct_1) - GetRectCenterY(gg_rct_2)
 set distance = SquareRoot(dx * dx + dy * dy)
 set a = Atan2(GetRectCenterY(gg_rct_1) - GetRectCenterY(gg_rct_2), GetRectCenterX(gg_rct_1) - GetRectCenterX(gg_rct_2))
 set logic = distance/i
 loop
 exitwhen i == 0
 set x = GetRectCenterX(gg_rct_2) + d * Cos(a)
 set y = GetRectCenterY(gg_rct_2) + d * Sin(a)
 call AddSpecialEffect("Abilities\\Spells\\Human\\FlameStrike\\FlameStrikeDamageTarget.mdl",x,y)
 set d = d + logic
 set i = i - 1
 endloop
 //Final Arrow
 set i = 10
 set d = 0.
 set dx = GetRectCenterX(gg_rct_3) - GetRectCenterX(gg_rct_2)
 set dy = GetRectCenterY(gg_rct_3) - GetRectCenterY(gg_rct_2)
 set distance = SquareRoot(dx * dx + dy * dy)
 set a = Atan2(GetRectCenterY(gg_rct_3) - GetRectCenterY(gg_rct_2), GetRectCenterX(gg_rct_3) - GetRectCenterX(gg_rct_2))
 set logic = distance/i
 loop
 exitwhen i == 0
 set x = GetRectCenterX(gg_rct_2) + d * Cos(a)
 set y = GetRectCenterY(gg_rct_2) + d * Sin(a)
 call AddSpecialEffect("Abilities\\Spells\\Human\\FlameStrike\\FlameStrikeDamageTarget.mdl",x,y)
 set d = d + logic
 set i = i - 1
 endloop
endfunction

//===========================================================================
function InitTrig_Arrow takes nothing returns nothing
 set gg_trg_Arrow = CreateTrigger( )
 call TriggerRegisterTimerEventSingle( gg_trg_Arrow, 0.00 )
 call TriggerAddAction( gg_trg_Arrow, function Trig_Arrow_Actions )
endfunction

#### Attachments

Arrow.w3x

 17.2 KB

 · Views: 185



#### nhocklanhox6

#### nhocklanhox6

 Level 18

JoinedFeb 12, 2012Messages414

Resources: 31

#### Resources
Models 7Spells and Systems 23Tutorials 1

 JASS:

function Trig_Untitled_Trigger_001_Actions takes nothing returns nothing
 local real x = GetUnitX(GetTriggerUnit())
 local real y = GetUnitY(GetTriggerUnit())
 local real o = GetUnitX(GetSpellTargetUnit())
 local real z = GetUnitY(GetSpellTargetUnit())
 local real dx = x-o
 local real dy = y-z
 local real distance = SquareRoot(dx * dx + dy * dy)
 local real angle = Atan2(z - y, o - x)
 local integer i = 20
 local real min = distance/i
 local real m = 0.
 loop
 exitwhen i < 0
 set m = m + min
 set o = x+m*Cos(angle*0.0174532)
 set z = y+m*Sin(angle*0.0174532)
 call AddSpecialEffect("Abilities\\Spells\\Other\\BreathOfFire\\BreathOfFireDamage.mdl",o,z)
 set i = i - 1
 endloop
endfunction

function a takes nothing returns boolean
 return GetSpellAbilityId() == 'A000'
endfunction

//===========================================================================
function InitTrig_JASS takes nothing returns nothing
 set gg_trg_JASS = CreateTrigger( )
 call TriggerRegisterAnyUnitEventBJ( gg_trg_JASS, EVENT_PLAYER_UNIT_SPELL_EFFECT )
 call TriggerAddCondition(gg_trg_JASS,Condition(function a))
 call TriggerAddAction( gg_trg_JASS, function Trig_Untitled_Trigger_001_Actions )
endfunction

I'm not kidding. Add 57.29582 again and everything return to normal.

#### Attachments

Dis and Ang GUI and JASS.w3x

 17.3 KB

 · Views: 198



#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

Did you look at what I posted? It worked on that map.

EDIT: You didn't read what I posted properly, I suggested you to remove the conversion look at this:

 JASS:

 set o = x+m*Cos(angle*0.0174532)
 set z = y+m*Sin(angle*0.0174532)

Remove those too.

 JASS:

 set o = x+m*Cos(angle)
 set z = y+m*Sin(angle)

#### nhocklanhox6

#### nhocklanhox6

 Level 18

JoinedFeb 12, 2012Messages414

Resources: 31

#### Resources
Models 7Spells and Systems 23Tutorials 1

chobibo said:

 Did you look at what I posted? It worked on that map.


Click to expand...

I saw it already, but in my map, did you see that 0.0...

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

Look at my previous post, yet again.

#### nhocklanhox6

#### nhocklanhox6

 Level 18

JoinedFeb 12, 2012Messages414

Resources: 31

#### Resources
Models 7Spells and Systems 23Tutorials 1

chobibo said:

 Look at my previous post, yet again.


Click to expand...


Okay, i get it but, did you see that ?, on my map attachment, it doesn't work as well, until you add 57..... =.=


Excess is better than the lack

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

As chobibo said, it should work without it. If you think about it, you are first multiplying the angle by 180/pi:

angle * (180 / pi)


And then you are multiplying it by pi / 180 in your Cos and Sin:

 JASS:

Cos( [angle * (180 / pi)] * (pi / 180) )
Sin( [angle * (180 / pi)] * (pi / 180) )


(180 / pi) and (pi / 180) cancel out. It should work. Atan2 returns radians and cos/sin take radians, you don't need a conversion.


Just make sure that you have the angle like this:

local real angle = Atan2(...)


Without a conversion, and then just make the cos/sin parts become Cos(angle) and Sin(angle). (instead of Cos(angle * pi / 180) and Sin(angle * pi / 180))


The tutorial itself is good. As maker said, it is efficient to store Cos(angle) and Sin(angle) as locals, otherwise the loop will perform the functions several times (or even better, you can store m * Cos() and m * Sin()) since it is all constant.


Also, I know it is very pretty to have lots of colors, but I recommend just choosing one for headers (the titles and stuff) and then using regular text for the rest.  Otherwise, nice job.

#### nhocklanhox6

#### nhocklanhox6

 Level 18

JoinedFeb 12, 2012Messages414

Resources: 31

#### Resources
Models 7Spells and Systems 23Tutorials 1

Sorry, busy in this time ^^!

@PurgeandFire: Updated !

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Approved. It is useful. I might fix the grammar/spelling later on, but it is still readable in its current state so I'll approve.

EDIT: I just moved it to the GUI forum, but I added "GUI/JASS" to the title. I also fixed some of the GUI leaks (I forgot to mention them earlier).

 You must log in or register to reply here.


### Similar threads

Locked

[Solved]
 Help me a little


Sasuke32

Oct 7, 2011

Triggers & Scripts

Replies4Views1K

Oct 10, 2011
Bannar

Locked

 Interfaces and Structs


chopinski

Jun 21, 2020

Triggers & Scripts

Replies14Views3K

Jul 11, 2020
chopinski

Locked

[JASS]
 Increasing the damage according to distance between source and target?


FlorianGeyerGiebelstadt

Jul 15, 2023

Triggers & Scripts

Replies1Views1K

Jul 15, 2023
Uncle

Locked

[vJASS]
 multiple structs in one trigger usage


kawas11

Jun 9, 2015

Triggers & Scripts

Replies1Views1K

Jun 10, 2015
PurgeandFire

Locked

[vJASS]
 Parabola problem


pks9067

Sep 25, 2011

Triggers & Scripts

Replies5Views1K

Sep 25, 2011
Bannar

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