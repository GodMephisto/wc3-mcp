---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-give-a-unit-the-ability-to-fly.201936/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Give a Unit the Ability to Fly
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-give-a-unit-the-ability-to-fly.201936/>
#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Unit Flying Ability

(The Right Way)

Table of Contents

Introduction

The Problem

The Solution

Tips

Wrap-Up

Introduction

This is quite a simple tutorial. I made it quite short and straight to the point because there's

not much to say. Basically, what we've been doing to give units the ability to fly is wrong.

Some Jass coders already know this, but most GUI coders don't.

The Problem

Most GUI coders do this to give a unit the ability to fly:

Unit - Add Crow Form to SomeUnit

Unit - Remove Crow Form from SomeUnit

This is wrong..

Why? Because:

Assume you have a unit that has the ability 'Arav' (Raven Form/Crow Form).

Now, if we were to add the ability to this unit, the function would fail this attempt and

UnitAddAbility will return false. After we remove the ability (In the second

line of the GUI method), the original 'Arav' ability will be removed.

We can conclude that the GUI method is unsafe, bug-prone and WRONG!

The Solution

As I mentioned before, UnitAddAbility will return false if the ability adding has failed.

We can take advantage of this to write a snippet that attempts to add the ability, checks if the ability

was added, and if it was, the ability will be removed. Luckily, we can do this with no overhead at all.

Or, you can simply check the level of the ability. If it's 0, add and remove the abilities.

If not, do nothing. (No! Don't use the action 'Do Nothing' because all it does is slow you down!)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Crow Form for Unit) Equal to 0

Then - Actions
Unit - Add Crow Form to Unit

Unit - Remove Crow Form from Unit

Else - Actions

But, I'd strongly recommend to Always use this faster, safer and better method instead:

Custom script: if UnitAddAbility(udg_Unit,'Amrf') then

Custom script: call UnitRemoveAbility(udg_Unit,'Amrf')

Custom script: endif

Those three lines would attempt to add the ability to the unit.

If the condition (UnitAddAbility) returns false, the ability won't be removed. If it returns true,

the ability will be removed.

Tips

 If you're making a map, you can totally avoid these checks by

creating an alternate crow form ability that you'll never use. (Credits to noob)

 Some people use 'Amrf', some use 'Arav', there's pretty much

no difference. If you're a GUIer and you're not sure what those are,

they're codes that correspond to each ability.

 This method should be applied for ALL abilities.

Wrap-Up

That's pretty much it. I hope you like this short but informing tutorial and I hope you

won't be making the mistake of blindly adding and removing unit abilities again.

Thank you for reading.

~Magtheridon96


 Last edited: Nov 8, 2011

#### Bannar

#### Bannar

 Level 26

JoinedMar 19, 2008Messages3,141

Resources: 23

#### Resources
Spells and Systems 5JASS 15Tutorials 1Wurst 2

No matter how short it is (although it could be added into other GUI tips tutorial), if the meaning is important - and I'm sure it is - I vote for approval.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

I think the curve ->

~Magtheridon96


Click to expand...

sold it

#### Jazztastic

#### Jazztastic

 Level 16

JoinedApr 4, 2011Messages995

Resources: 13

#### Resources
Spells and Systems 10Tutorials 3

Magtheridon96 said:

 I think the curve ->


sold it

Click to expand...


God I wish I had spread enough rep around to rep you again.

#### Luorax

#### Luorax

 Level 16

JoinedAug 7, 2009Messages1,406

Resources: 2

#### Resources
Maps 1Tutorials 1

Yea, nice tutorial for GUI users - length is really not something that matters, but usefulness. (I used to use the "bad method" too when I was a beginner [GUI] triggerer  Sad story)

However I still consider the simple AutoFly snippet more easy and more useful

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

The curve really DID sell it ^^

#### The_Silent

#### The_Silent

 Level 46

JoinedFeb 4, 2008Messages3,511

Resources: 204

#### Resources
Models 63Icons 115Packs 8Skins 16Maps 1Ported Models 1

You could really do this without using custom scripts

#### Luorax

#### Luorax

 Level 16

JoinedAug 7, 2009Messages1,406

Resources: 2

#### Resources
Maps 1Tutorials 1

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Crow Form for (Triggering unit)) Equal to 0

Then - Actions
Unit - Add Crow Form to (Triggering unit)

Else - Actions

There you go. But that custom script is faster, it's only 3 lines of JASS script, and you know what it does. But obvisouly you have to decide which method you wanna use.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Thank you Luorax


~Updated

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

The GUI version is not needed in this case whatever you may say. Just mention it but also show the JASS conversions of both methods (GUI/JASS) and they will see the amount of extra calls (in this case 2 but whatever).

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

You're probably right

I dont have the WE right now, so I'll just guess what functions are called xD

I think this:

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Crow Form for SomeUnit) Equal to 0

Then - Actions
Unit - Add Crow Form to SomeUnit

Unit - Remove Crow Form from SomeUnit

Else - Actions

is converted to this:

 JASS:

function SomeReaaaaaaaallyLongFunctionName0001001 takes nothing returns boolean
 return not (GetUnitAbilityLevelSwapped('Arav',udg_SomeUnit) == 0) == false
endfunction

function TheActionslol takes nothing returns nothing
 if SomeReaaaaaaaallyLongFunctionName0001001() then
 call UnitAddAbilityBJ('Arav',udg_SomeUnit)
 call UnitRemoveAbilityBJ('Arav',udg_SomeUnit)
 else
 endif
endfunction

Something like that

#### Bannar

#### Bannar

 Level 26

JoinedMar 19, 2008Messages3,141

Resources: 23

#### Resources
Spells and Systems 5JASS 15Tutorials 1Wurst 2

You probably know that this condition









 JASS:

function SomeReaaaaaaaallyLongFunctionName0001001 takes nothing returns boolean
 return not (GetUnitAbilityLevelSwapped('Arav',udg_SomeUnit) == 0) == false
endfunction

Looks actually in GUI much worse XD

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Actually, it's a fact that all if conditions look like that, but trigger conditions are an EPIC FAIL XD

 JASS:

function Conditions takes nothing returns boolean
 if (not (0==1)) then
 return false
 endif
 return true
endfunction

Actually Blizzard is smart cause they didnt do this:

 JASS:

function Conditions takes nothing returns boolean
 if (not (0==1)) then
 return false
 else
 return true
 endif
endfunction

 lol

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

Not that smart though...

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

I think they did this on purpose :/

They probably wanted to make GUI slow so that they could force serious wc3 modders to use JASS  (I'm on to you Blizzard! )

#### The_Silent

#### The_Silent

 Level 46

JoinedFeb 4, 2008Messages3,511

Resources: 204

#### Resources
Models 63Icons 115Packs 8Skins 16Maps 1Ported Models 1

Magtheridon96 said:

 Actually, it's a fact that all if conditions look like that, but trigger conditions are an EPIC FAIL XD

 JASS:

function Conditions takes nothing returns boolean
 if (not (0==1)) then
 return false
 endif
 return true
endfunction


Actually Blizzard is smart cause they didnt do this:

 JASS:

function Conditions takes nothing returns boolean
 if (not (0==1)) then
 return false
 else
 return true
 endif
endfunction

 lol


Click to expand...


It doesn't actually take a genius to figure this one out, lol xD

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

W8!

Guys!

I think I know why Blizzard did this:

 JASS:

function Conditions takes nothing returns boolean
 if not (something) then
 return false
 endif
 return true
endfunction

And not this:

 JASS:

function Conditions takes nothing returns boolean
 if something then
 return true
 endif
 return false
endfunction

It's because a trigger's events will fire quite often, so they wanted to make sure that the actions are NOT supposed to happen first

That sux xD

They probably didnt do this:

 JASS:

function Conditions takes nothing returns boolean
 return something
endfunction

Because that would make conditions look lame

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

Or that they were just lazy enough not to think about the last one.

#### Bannar

#### Bannar

 Level 26

JoinedMar 19, 2008Messages3,141

Resources: 23

#### Resources
Spells and Systems 5JASS 15Tutorials 1Wurst 2

baassee said:

 Or that they were just lazy enough not to think about the last one.


Click to expand...

Lazy? I think they were actually brain storming about condition function.









 JASS:

function Conditions takes nothing returns boolean
 if not (something) then
 return false
 endif
 return true
endfunction

Because it's dam hard to create such monster. Would you lose your free time to replace 3 line-script with 6 lines?  I think they got well paid for that stuff - it explains everything xD

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

It's not even a 3 liner, it's a one-liner.

 JASS:

function Conditions takes nothing returns boolean
 return something
endfunction

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

No guys !

 JASS:

function Conditions takes nothing returns boolean
 if (((something==true)==true)==true) then
 // Check again to make sure :O
 if (((something==true)==true)==true) then
 // Ok now we're sure :P
 return true
 endif
 // HAHA! The Jass Interpreter was wrong >:D
 return false
 endif
 // lol something==false :ogre_hurrhurr:
 return false
endfunction

N00bs

Ok, I think we should end this argument xD


 Last edited: Sep 6, 2011

#### noob

#### noob

 Level 14

JoinedSep 28, 2011Messages968

Resources: 5

#### Resources
Maps 4Tutorials 1

hey there is already one tutorial on flying height approved https://www.hiveworkshop.com/forums/general-mapping-tutorials-278/adding-fly-height-any-unit-17790/

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

But that one does it incorrectly

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

Use some tips from here. Your tutorial is unattractive and plain.

https://www.hiveworkshop.com/forums...t-tutorial-please-read-before-posting-119320/

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Oh Apheraz, how mean of you D:

You made me cry :'(

...

lol jk, I'll fix it xD

edit

Updated the tutorial.

Looks much better now :3


 Last edited: Oct 20, 2011

#### noob

#### noob

 Level 14

JoinedSep 28, 2011Messages968

Resources: 5

#### Resources
Maps 4Tutorials 1

Now you changed the tutorial and added colors it is unreadable and ugly. text is not for making ugly coulours.

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

The heading colors you should use are these:

Color Test

Color Test

Color Test

Color Test

Color Test

As they fit the Hive theme perfectly, and are not pain in the behind to be seen. Alternatively, any font color should be avoided - the one that is used to write everything is good enough - white color is standing out too much.

#
 the title

This is the wrong way of doing it.

Tips should be written in colors that stand out, if any.

## The Title
This is the right way of doing it.

Tips should be written in small letters, if any.

For color codes, just quote this message and copy them.

And that orange is okay, but please, no white in the tutorial.

There should also be "Tips & Hints" with some extra information, or maybe "Suggested Readings". Also, Content of tutorial won't hurt.

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

Red hurts the eyes... and dark blue is hard to read...

anyway, this is helpful for those GUIers out there...

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

If you red what I wrote, you would clearly notice "This is the wrong way" part.

Anyways, Magtheridon96, what you plan on doing? Something new, instead of aesthetic appearance? Maybe, you could go with a Jass version, too, like a bonus feature.

#### baassee

#### baassee

 Level 22

JoinedNov 14, 2008Messages3,253

Resources: 20

#### Resources
Spells and Systems 17Tutorials 3

Red is awesome to put out warnings. Hehe.

#### noob

#### noob

 Level 14

JoinedSep 28, 2011Messages968

Resources: 5

#### Resources
Maps 4Tutorials 1

I finaly understanded why units with crow form are able to change height it is simply because when they cast the spell they make the morph anim, change height and after become the new unit.


Another way to do what you do without check just create one new ability only for the trick based on crow form.


and when you want to change the heigt of an unit add and remove this ability.


Because no unit have this ability there is no problem.


This tutorial is partially jass so change it category to jass.


Because gui section is for the ones who does not know jass.


 Last edited: Oct 27, 2011

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

 Another way to do what you do without check just create one new ability only for the trick based on crow form.

and when you want to change the heigt of an unit add and remove this ability.

Because no unit have this ability there is no problem.


Click to expand...

True, but in public resources, it would be troublesome.

For a map, that makes sense.

Using the check can still save some space though.

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

The tutorial now has proper coloration and heading, and it also displays useful information that aren't seen in other tutorials.

I'm looking forward to see this approved.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

 The tutorial now has proper coloration and heading


Click to expand...

lol You posted that while I was editing the tutorial

How does it look now? ^^

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

One of colors that fit the Hive the most. Nice, and not painful for the eyes. But, now the red stands out too much. Tried with yellow/dark yellow? It would fit the theme and still draw attention, if your aim is to get a cohesive coloration.

Anyways, don't bold the red lines. Use this, this or this instead, with no colors at all, or use pure colors.

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

That's true.

Updated.

#### Apheraz Lucent

#### Apheraz Lucent

 Level 30

JoinedJan 31, 2010Messages3,545

Resources: 4

#### Resources
Tutorials 4

Sigh... We aim for perfection, but we can't see it's a moving target.

Aesthetically very good, the concept of tutorial is good as well, and the whole tutorial is useful.

Good Job, I have to say.

Oh, the only thing you can do now is to use points. The points will link your table of contents, and when you click, for an example, "Tips", the page will scroll down to that point.

If you need help with points implementation, ask me via Pm / Vm.

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

You should use 'Amrf' instead of 'Arav' as 'Amrf' is from Medivh which is not as common as 'Arav' which is from a DoT.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice updates.

~Approved.

#### gorillabull

#### gorillabull

 Level 17

JoinedJul 17, 2011Messages1,863

Resources: 5

#### Resources
Skins 1Spells 4

yea.. i have a question: if a unit is set to have Fly Movement type in the Object editor then why does not the function "set flying height" work?? you would have to set it to zero first and then change it or sometimes that does not work either is it a bug?

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

Warcraft III has a ton of bugs :/

I guess the most ideal thing to do is to give all units Foot Movement type.

If you want flying units, just detect when a unit of a certain type enters the map and give it a flying height

#### gorillabull

#### gorillabull

 Level 17

JoinedJul 17, 2011Messages1,863

Resources: 5

#### Resources
Skins 1Spells 4

hmm im gonna try that

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

I wrote a system for this: https://www.hiveworkshop.com/forums/submissions-414/snippet-flyheight-214371/

 You must log in or register to reply here.


### Similar threads

[GUI]
 How to properly use "Crow Form"


Vehster

Aug 28, 2011

Tutorial Archive

Replies8Views6K

Nov 9, 2011
PurgeandFire

Locked

[Trigger]
 Flying on water trigger


seurimas

Mar 22, 2008

Triggers & Scripts

Replies4Views1K

Mar 22, 2008
seurimas

 people who wnt to learn to fly tutorial


jasond10

Apr 20, 2011

Tutorial Archive

Replies10Views12K

Apr 23, 2011
PurgeandFire

Locked

 Help Flying a Unit


Rejected

Apr 18, 2011

Triggers & Scripts

Replies7Views1K

Apr 19, 2011
Pharaoh_

C

Locked

[Trigger]
 Making a ground unit fly


CheeseOfDoom

Oct 20, 2011

Triggers & Scripts

Replies7Views8K

Oct 23, 2011
defskull

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