---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/tooltip-tutorial.51966/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Tooltip Tutorial
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/tooltip-tutorial.51966/>
#### DenZel94

#### DenZel94

 Level 7

JoinedJun 4, 2006Messages127

Resources: 7

#### Resources
Maps 3Tutorials 4

This is a tutorial about tooltips and related subjects.


So you may ask these:

What are "TOOLTIPS"?
Tooltips are what you see and read when you try to learn an ability, try to buy a unit or item, try to use an item or ability and information of a certain item or ability, passive or active.

Why do we need to know anything about tooltips?
Tooltips are the essential parts of a skill of which it will identify both you, and the seer, know what this certain ability, item or other objects do. If you cannot understand the tooltip, then you wouldn't really know what it does unless it is your second or third time to do it.

Kinds of Tooltips


Abilities:


There are at least 3 types of tooltips. These are Learn tooltips, Normal tooltips, and Turn Off tooltips. All of these tooltips are found with Hero type of abilities.

LEARN = These tooltips are seen when you try to learn an ability.

NORMAL = These tooltips are seen when you look at the information of an already learned ability or a unit ability.

TURN OFF = This tooltip has 2 uses which has different effects on Activate/Deactivate Abilities (e.g. Immolation) and Auto-cast abilities (e.g. Cold Arrows).


@ Activate/Deactivate = This is quite simple. It is seen when you want to deactivate it from its effect such as immolation.


@ Auto-cast = There are 2 parts in Turn Off, one is simply just "Turn Off" and the other is "Turn Off - Extended". Now they are seen below your "Normal - Extended" tooltip (commonly shown in light blue). What you right in Turn Off alone is when your activate its auto-cast and Turn Off - Extended is the opposite, it is when you deactivate the auto-cast.


Units:


There are 3 or 4 types of tooltips. These are Revive, Extended and Basic. All these tooltips are found with Hero type of unit.

BASIC - These tooltips are seen when you try to Buy a unit (with hotkey).

EXTENDED - These tooltips are seen when you try to buy a unit as information.

REVIVE - This tooltip is seen when you try to revive a hero.


Items:


There are only 2 typre: Basic and Extended.

Codes


Codes are strings (information) to create a special effect to your tooltips such as colors and spacing. Very widely known codes in Warcraft III are |cffffcc00, |r, and |n

Simple Codes


|c-colorcode- ~ This is a code which would give the text color than just white (or plainly #FFFFFF). |cffffcc00 is usually used in hotkeys, Level emphasis (see above) and other things.


Want more colors? Go to this link:

http://www.virtuelle-identitaet.ch/index.php?option=com_content&task=view&id=19&Itemid=32

PS: |cff must be the suffix and also remove the # sign to make the color code properly work.


|r ~ This code ends the color codes or is a full stop of a color code. Typing Hel|cffffcc00l|ro will only "yellow" the l, but if you type Hel|cffffcc00lo, it will "yellow" o as well or even until the end of the tooltip (or messages in the trigger editor).


|n ~ Rather simple. It doesn't relate anything to color but it is helpful in spacing and acts as a line breaker in the World Editor, both tooltips and Trigger Editor. See below for an example:


Very Uncommon!


%d = This gets the integer level of a current skill + 1, which is used in Tooltip - Learn such as "Learn |cffffcc00R|ressurection" - [|cffffcc00Level %d|r]". If, for example, the current level is 3, so it would look like this = "Learn Ressurection - [Level 4]

Raw Codes


Raw Codes are codes seen in raw detail rather than the real and simple name itself (such as Water Elemental = Simple).


When you see some tooltips, you'll see gibberish somewhat like this which, when you look for that data, you don't know where to get it.


We will use "Bash" as our skill:


Now, go the View Menu of the Trigger Editor and check "Display Values as Raw Data" or simple Ctrl + D


Now you'll see more gibberish. The left side is not seen in reality, but edited by yours truly.


If you have the understood above, let us see again the "Normal - Extended" tooltip for Bash and let us identify the gibberish (let's call it Raw Codes now).


PS: To make your custom spells work, you have to change the left (before the comma) raw code to the raw code of the custom spell.


Now let's go to summon spells. Summon spells have different raw codes than the other skills because their raw data doesn't come from the skill, but a unit (specifically, the unit being summoned)


Let's have "Summon Water Elemental" based.


Now, the boxed raw codes do not exist in the skill. So you have to look for it. You will find it in the unit section. Since we are basing the skill to "Summon Water Elemental", then we get the Water Elemental unit


As you see above, the raw code in the tooltip has the raw code of water elemental. The only problem is, how do we get mindmg1, maxdmg1 and realHP? Well, they do not exist in the unit (in my knowledge) and maxdmg1 and mindmg1 cannot be obtained or changed but these are only in the based unit. Only realHP can get the real hp of a unit, not the base.


mindmg1 = Minimum Damage of the based unit


maxdmg1 = Maximum Damage of the based unit


realHP = Total Base Hit Points (HP) of a unit

Extra: Auto-Fill


Auto-Fill is used to automatically fill all the tooltips or values at one time depending on the desired increment (otherwise, tooltips are consecutives, 1, 2, 3 etc..)


This is an image to where you'll find Auto-Fill, we are using an edited "Avatar" skill which has 5 levels.


When you press the one highlighted blue, a window will open (image is below).


"Start from level" states what number will start first


"Replace with level" is the number or text (CASE SENSITIVE) that it will be replaced with numbers. Example is changing it to Capital A. All capital As will be replaced with the number, raw coded or not.


After doing so, you'll have this result:


~


Well, the tutorial is done! Very simple isn't it? Well, good luck at your future mapping!


-


This has been brought by yours truly,


Thy FuKiN n0oB


------------


Thanks to ~GaLs and Terrabull for their help.


ORIGINAL LINK: http://www.thehelper.net/forums/showthread.php?p=658073#post658073

#### Attachments

herotooltipsem2.jpg

 17.2 KB

 · Views: 10,076



learntooltipym8.png

 56.4 KB

 · Views: 10,073



normaltooltipds4.jpg

 13.7 KB

 · Views: 9,840



turnoffimmozx3.jpg

 10 KB

 · Views: 9,998



turnoffcaha4.jpg

 33.5 KB

 · Views: 9,835



unittooltipspi2.jpg

 12.2 KB

 · Views: 9,550



unitbandebr2.jpg

 19 KB

 · Views: 9,830



unitreviverg6.jpg

 7.4 KB

 · Views: 9,712



13216403wc9.jpg

 60.6 KB

 · Views: 9,934



itemtooltipszs0.jpg

 10.1 KB

 · Views: 9,837



gibberishcodesgf7.jpg

 17.3 KB

 · Views: 9,547



displayvaluesasrawdatackm3.jpg

 16.2 KB

 · Views: 9,593



moregibberisheo7.jpg

 81.2 KB

 · Views: 9,916



identifysm1.jpg

 20.7 KB

 · Views: 9,632



waterelementalbq1.jpg

 6.5 KB

 · Views: 9,486



unitgibberishkj1.jpg

 19.7 KB

 · Views: 9,543



autofill1lg1.jpg

 29.3 KB

 · Views: 9,609



autofill2kk6.jpg

 19.8 KB

 · Views: 9,515



autofill3xt7.jpg

 37.7 KB

 · Views: 9,567




 Last edited by a moderator: Feb 15, 2008

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

Very good tutorial, If I would be moderator I would have aproved it after seeing half of it. I'll give you rep, hope it gets aproved. No critics or anything, except; that you could add a list containing the topics and subtopics that you are going to work with.

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

General Tutorial Submitting Rules & Guidelines - README!

 All image files must be attached to your post. We don't wish to be dependent on other sites (to avoid situations in which the server that hosts images fails, thus damaging the tutorial).


Click to expand...

Please physically attach the images to the tutorial. Thanks.

#### JetFangInferno

#### JetFangInferno

 Level 37

JoinedJul 22, 2004Messages1,002

Resources: 170

#### Resources
Models 170

Awesome tutorial! Nice I can edit tooltips =) +rep

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Tutorial Approved!

Images attached.


~ Thread moved to General Mapping Tutorials.

#### inverted

#### inverted

 Level 10

JoinedFeb 8, 2008Messages431

Resources: 2

#### Resources
Maps 1Tutorials 1

Thank you very much sir, this helped alot  +rep

#### Elven_God

#### Elven_God

 Level 8

JoinedJun 25, 2007Messages165

Resources: 4

#### Resources
Icons 1Skins 1Maps 2

That is helpful and useful... good tutorial! +rep

#### Parrothead

#### Parrothead

 Level 13

JoinedJun 10, 2007Messages777

Resources: 17

#### Resources
Maps 10Spells 2Tutorials 5

Very impressive tutorial!

#### MindWorX

#### MindWorX

Patron

 Level 21

JoinedAug 3, 2004Messages710

Resources: 8

#### Resources
Tools 4Tutorials 4

You should update the tutorial and add explanations of ",%" and ",.", both can be added in any real reference, and will either multiply by 100, or display the number with decimals.


For example <A000,Area1,.> or <A000,DataA1,%>.

#### Aluuna

#### Aluuna

 Level 2

JoinedMay 1, 2008Messages11

Resources: 1

#### Resources
Maps 1

your a lifesaver dude

#### Morcrist

M

#### Morcrist

Another good tip is for when your ability code happens to end in a 1. Rare, but eventually it'll happen. What happens when you try to autofill this, for example...?


Does <A001,DataA1> damage.


Well, it'll look something like this...


Does <A001,DataA1> damage.


Does <A002,DataA2> damage.


Does <A003,DataA3> damage.


...which we don't want.


So to FIX that problem, do THIS for your base text:


Does <A001,DataA1>> damage.


And THIS for your replace:


1>


That way it'll only replace the 1>'s with the new numbers, giving us the proper...


Does <A001,DataA1> damage.


Does <A001,DataA2> damage.


Does <A001,DataA3> damage.


So yeah, admittedly it only happens once every blue moon when your ability code HAPPENS to end in a 1. But still!


Take care.

#### SlayerII

#### SlayerII

 Level 9

JoinedAug 21, 2008Messages533

Resources: 3

#### Resources
Skins 1Maps 1Tutorials 1

ty morcrits now my map works

#### Lestat(br)

#### Lestat(br)

 Level 10

JoinedOct 2, 2005Messages396

Resources: 8

#### Resources
Models 7Icons 1

Is there a code tag to make some text invisible by the players? (I know, odd question, but I need this)

#### Dimadelic

#### Dimadelic

 Level 6

JoinedNov 1, 2008Messages31

Resources: 13

#### Resources
Tools 3Maps 10

Thanks the author!!!

#### Saishy

#### Saishy

 Level 12

JoinedMar 23, 2008Messages942

Resources: 1

#### Resources
Models 1

Its possible to get a unit status? I mean, we can get its hp...

Also, can we do arithmetic inside the tooltip?

#### VeljkoM

#### VeljkoM

 Level 21

JoinedJul 27, 2008Messages14,361

Resources: 4

#### Resources
Maps 3Tutorials 1

Finally.I have waited for this.You are good person.

#### thereallywhitekid

#### thereallywhitekid

 Level 6

JoinedNov 18, 2008Messages119

Resources: 2

#### Resources
Spells and Systems 1Tutorials 1

An absolute perfect tutorial for my maps. Thank goodness you have made this tutorial. I now understand more about tooltips from World Editor thanks to you. +rep

#### ipw3n

I

#### ipw3n

is there an ability editor in roc? if so where.

#### MindWorX

#### MindWorX

Patron

 Level 21

JoinedAug 3, 2004Messages710

Resources: 8

#### Resources
Tools 4Tutorials 4

There isn't.

#### Archangel_Tidusx

#### Archangel_Tidusx

 Level 16

JoinedFeb 24, 2007Messages1,097

Resources: 49

#### Resources
Models 9Icons 27Skins 1Maps 12

Okay, I saw how you inserted the raw values for normal abilities, but I've been trying with custom and I can't seem to get it right

#### FelSheep

#### FelSheep

 Level 2

JoinedMay 21, 2009Messages12

This tutorial is for WE newbies and those that doesn't have knowledge with color, unit, upgrade, doodad, ability and buff/effect codes. I have mastered making tootips for units and abilities used in my map. Also, no math, even arithmetic, is allowed in tooltips.

#### meatfactory

#### meatfactory

 Level 14

JoinedJan 3, 2009Messages400

Resources: 31

#### Resources
Models 1Icons 4Maps 20Templates 6

Doesn't seem to get the Hero's attribute to display on a tooltip (0) ... any ideas?

#### MindWorX

#### MindWorX

Patron

 Level 21

JoinedAug 3, 2004Messages710

Resources: 8

#### Resources
Tools 4Tutorials 4

Almost 5 years after the last post. I'm pretty sure that counts as necroing.

And by attributes, do you mean Strength, Agility and Intelligence?

#### meatfactory

#### meatfactory

 Level 14

JoinedJan 3, 2009Messages400

Resources: 31

#### Resources
Models 1Icons 4Maps 20Templates 6

MindWorX said:

 Almost 5 years after the last post. I'm pretty sure that counts as necroing.


And by attributes, do you mean Strength, Agility and Intelligence?


Click to expand...


No I mean the length and depth of the heroes dick's and vg... of course I mean str agi and int

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Well, for tooltips, they are static (opposed to dynamic). Once you enter a tooltip, it is saved in a file and it uses that tooltip throughout. The numbers, text, etc. won't change. So when you do the Data<ACl3, ...> stuff, it'll just read that field and put whatever is in there. As such, when you ask for a unit's attribute, it'll just read off whatever is the initial value. It won't update with the attribute. So if you have a tooltip that says "22 intelligence", and the hero gets a +2 intelligence tome, the tooltip will stay as "22 intelligence". Sadly, tooltips aren't actually that dynamic. :\

tldr; The Data<####, ####> stuff is just a shortcut (useful if you need to autofill or if you change the values a lot, e.g. for balancing). Apart from that, they don't offer any "extra" features, they just save you some typing.

#### meatfactory

#### meatfactory

 Level 14

JoinedJan 3, 2009Messages400

Resources: 31

#### Resources
Models 1Icons 4Maps 20Templates 6

PurgeandFire said:

 Well, for tooltips, they are static (opposed to dynamic). Once you enter a tooltip, it is saved in a file and it uses that tooltip throughout. The numbers, text, etc. won't change. So when you do the Data<ACl3, ...> stuff, it'll just read that field and put whatever is in there. As such, when you ask for a unit's attribute, it'll just read off whatever is the initial value. It won't update with the attribute. So if you have a tooltip that says "22 intelligence", and the hero gets a +2 intelligence tome, the tooltip will stay as "22 intelligence". Sadly, tooltips aren't actually that dynamic. :\


tldr; The Data<####, ####> stuff is just a shortcut (useful if you need to autofill or if you change the values a lot, e.g. for balancing). Apart from that, they don't offer any "extra" features, they just save you some typing.


Click to expand...


Thanks for the info... what about the file that the data is saved? Is it accessible?

#### TheAyalalalalon

#### TheAyalalalalon

 Level 5

JoinedFeb 18, 2016Messages96

Hello

I am not able to make my tooltips show correctly.

I am trying to use the <o006,dmgplus1> (Attack damage base) and <o006,rangeN1> (Unit´s range) but it keeps loading as "0" in the map

The o006 is a custom unit based on Serpent Ward <osp1>

Did i miss something in the code?

#### Marcell

#### Marcell

 Level 4

JoinedJan 17, 2018Messages112

Great guide, it's just missing aura tool tips displayed on other units. Like Endurance aura

#### Death Adder

#### Death Adder

 Level 12

JoinedNov 3, 2013Messages989

One thing I don't get is, in hero abilities there's only one field for 'Learn', but you can use %d to get the integer of the next level (the one you'd learn), this seems like it's dynamic isn't it?

If so, I wonder if there's some way to use %d instead of a number for the <AbilCode,DataA1> Cool1, Dur1, etc. and have say DataA%d or Cool%d instead. Basically, when you have 0 levels of the ability and want to learn it for the first time, it would use the data for level 1, at level 1 it would show the data for level 2, etc.

I already tried doing it like that DataA%d and the like, but it didn't work. But since %d works on it's own, I wonder if there's some way. For a bit of a scrappy solution you could actually do something like "base value + %d * level increase value", or if you're lucky the value could be the same as the level...

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Death Adder said:

 One thing I don't get is, in hero abilities there's only one field for 'Learn', but you can use %d to get the integer of the next level (the one you'd learn), this seems like it's dynamic isn't it?

If so, I wonder if there's some way to use %d instead of a number for the <AbilCode,DataA1> Cool1, Dur1, etc. and have say DataA%d or Cool%d instead. Basically, when you have 0 levels of the ability and want to learn it for the first time, it would use the data for level 1, at level 1 it would show the data for level 2, etc.

I already tried doing it like that DataA%d and the like, but it didn't work. But since %d works on it's own, I wonder if there's some way. For a bit of a scrappy solution you could actually do something like "base value + %d * level increase value", or if you're lucky the value could be the same as the level...


Click to expand...

That's neat! I didn't know about that. What happens if you do <'A000',Data%d1>? What does the tooltip end up looking like in game?

Maybe it's parsed in one-pass, so while it does end up translating %d, it might still try to process <'A000',Data%d1> directly (without replacing %d) and fail.

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

Inside learn tip there are %d placeholder. All kinds of <DATA> refs arent real placeholders therefore you cant do anythng about it

#### kentzeus

#### kentzeus

 Level 7

JoinedJul 4, 2007Messages249

Anyone know how to show two decimals? <,.> only shows one decimal, I tried <,..> but it doesn't work.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

The field references also supports ,+ instead of ,% ,.

<A000,Area1,+> displays a positive number even when the field is negative.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Tasyen said:

 The field references also supports ,+ instead of ,% ,.

<A000,Area1,+> displays a positive number even when the field is negative.


Click to expand...

This is a very big information for me. Thanks!

 You must log in or register to reply here.


### Similar threads

 Raw Data Tutorial


Oninuva

Apr 9, 2007

Misc/General Mapping Tutorials

Replies2Views15K

Apr 25, 2007
Wolverabid

Locked

 Tooltip field references validation tool


Barade

Mar 2, 2017

Warcraft Editing Tools

Replies1Views1K

Mar 2, 2017
DracoL1ch

Locked

 Display Damage of a Custom Unit in it's Extended Tooltip


Bigabig

Mar 11, 2020

World Editor Help Zone

Replies4Views2K

Mar 13, 2020
Macadamia

Locked

 Dynamic Values in Tooltips <A000,DataA1>


tml616

Jun 17, 2009

World Editor Help Zone

Replies2Views3K

Jun 17, 2009
tml616

C

Locked

 Incorrect Values in Tooltips


CaptainRandom

Oct 14, 2008

World Editor Help Zone

Replies5Views2K

Oct 14, 2008
CaptainRandom

C

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