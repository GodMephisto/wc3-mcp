---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/single-target-spell-for-multiple-units.242153/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Single Target Spell for Multiple Units
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/single-target-spell-for-multiple-units.242153/>
#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

I just want to share my experience and knowledge with others and i don't think everyone knows about this. Especially those who just starts modding.​

Be aware that I don't recommend this tutorial if you don't understand basic triggering yet.​

​

 Table of Contents


Things You May Need

Introduction

Lets get this started
Object Editor

Trigger

Useful Links

Credits

​

​

​

##
 Things You May Need

​

You must understand English

You have Warcraft III and World Editor

Knowledge about Triggers/Trigger Editor

Knowledge about Object Editor

​

##
 Introduction

​

Did you ever wonder how to use a single target ability against multiple units?​

For example: Sleep, you can only use Sleep against single unit/target but you want to use it against multiple units, like units within range or something.​

We need a little bit of triggering to do this.​

If you think triggering is hard to understand, it is not as hard as you think! but i will try to explain it to you as simple as i can.​

​

## Lets get this started
​

Without further ado, let's get down to the business. We will start with Object Editor then we will use trigger. We will make a Sleep ability with instant cast which will put units within range to sleep.​

​

###
 Object Editor

​

I believe you're already familiar with Object Editor, so let's create the base ability. We will use Roar for this.​

​

Create a New Ability based on Roar (You may use any other instant cast abilities such as; Thunder Clap, War Stomp, Battle Roar/Roar, etc)
Art - Caster, change it to your desired special effect, this special effect will appeared when you cast the spell.

Data - Damage Increase (%), change it to 0.00, because we don't want the damage increase effect from this ability.

Stats - Buff, delete the available value. Buff is status effect which will appeared at Unit's status bar. Note that even if we delete the buff the buff will still appear at unit's status.

Stats - Duration - Hero

 Stats - Duration - Unit, change both of them to 0.00.

Stats - Target Allowed, change it to none

This will be our unit's ability. This is the ability which will we click in the game, You may change its description to whatever you like.

Create a New Ability based on Sleep (Because we will need Sleep buff to put units to sleep, same to stun. You could use Storm Bolt/Fire Bolt to stun your target)
Data - Stun Duration, i'm not sure what is this for. But i will change it to 0.00

Stats - Cast Range, change it to 99999 to make sure our Dummy Unit could reach the target, but this still has a problem because of visibility

Stats - Duration - Hero

 Stats - Duration - Unit, change these to your desired duration. I will change it to 2 second.

Stats - Mana Cost, change it to 0, so our Dummy Unit can use the ability without mana.

This will be our dummy's ability. It's an ability where you can't see it in the game. In order to do this we will need a help from a unit to cast the ability at the enemies. This unit is what the so called Dummy.)

Create a New Unit, i will base it on Footman. You may use any other unit, though.
Abilities - Normal, Add the sleep ability and Locust ability to the list. Locust ability allows our Dummy's Hit Points and Mana bar to be hidden

Art - Animation - Cast Backswing, change this to 0.000 otherwise this method won't work properly

Art - Animation - Cast Point, change this to 0.000 otherwise this method won't work properly

Art - Model File, use custom path and write "Dummy" on it. Because there is no Model File with "Dummy" as a name, if you put this inside the world editor it will appear as Green Box. But in the game it will appear without any model file.

Art - Shadow Image (Unit), change it to none. So Dummy will have no shadow beneath it.

Damage, we will make the Dummy has no damage. So it won't attack enemy unit.
Combat - Attack 1 - Damage Base, change it to 0

Combat - Attack 1 - Damage Number of Dice, change it to 0

Combat - Attack 1 - Damage Sides per Die, change it to 0

Movement - Type, change it to Fly. So Dummy won't collide with other units in battlefield.

Movement - Speed, change it to 0 otherwise this method won't work properly.

​

Take it slow and just follow the steps. You will understand how it works eventually.​

Now, we will begin with the triggering part. Is it hard? No, of course not. You will be just fine.​

​

###
 Trigger

​

Trigger
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Sleep

Actions-
-------- This is the guy who use the ability --------

Set Caster = (Triggering unit)

-------- The Owner of Caster --------

Set Owner = (Triggering Player)

-------- Store Caster's position into a variable --------

Set TempPoint = (Position of Caster)

-------- We create our dummy --------

Unit - Create 1 Dummy for Owner at TempPoint facing Default building facing degrees

-------- Store him into a variable --------

Set DummyUnit = (Last created unit)

-------- We gives him the dummy's sleep ability --------

Unit - Add Sleep (Dummy) to DummyUnit

-------- We gives him an expiration timer, so he won't stay in the map forever --------

Unit - Add a 1.00 second Generic expiration timer to DummyUnit

-------- Pick units within 500 range of position of the caster --------

Set Group = (Units within 500.00 of TempPoint)

Unit Group - Pick every unit in Group and do (Actions)
Loop - Actions
-------- Store the picked unit into a variable first, because we'll use them more than once --------

Set TempUnit = (Picked unit

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(TempUnit belongs to an enemy of Owner) Equal to True

((TempUnit is alive) Equal to True)

Then - Actions
-------- Order him to use it against all units within 500 range of Position of the caster --------

Unit - Order DummyUnit to Undead Dreadlord - Sleep TempUnit

Else - Actions

-------- We remove potential leaks --------

Custom script: call RemoveLocation(udg_TempPoint)

Custom script: call DestroyGroup(udg_Group)

​

###
 Useful Links

​

Things that leak

Check this to know more about things that leak

Variables

To know more about Variables, if you don't understand this tutorial. I recommend you to read this first

Things a GUI User Should Know

A Guide about GUI trigger from Beginning to Advanced levels

​

So how was it?​

Quite simple right?​

I hope this can be useful for anyone who read this​

and you can use the test map if you want to confirm the validity of it or if you want to learn directly from the test map. You can download it - here​

​

###
 Credits

​

deathismyfriend

Maker

chobibo

jakeZinc

PurgeandFire

I'd like to thank these guys, for supporting and helping me in order to improve this tutorial.​

​

And Thank you for reading all of this ^^​

My thanks doesn't apply to those who just skip the tutorial and read this part, make sure to read them thoroughly!​

​

#### Attachments

Sleep Test.w3x

 18.4 KB

 · Views: 862




 Last edited: Aug 19, 2024

#### GreeN!X

#### GreeN!X

 Level 23

JoinedOct 20, 2012Messages3,068

Resources: 4

#### Resources
Icons 1Maps 1Templates 1Tutorials 1

The images aren't showing onii-san. They need some fixin'


Anyway, this is a pretty nice and simple tutorial you have. You could add some more details to what you're doing and why though.

syltman said:

 Tutorials should be retarded proof.


Click to expand...

Oh and Maybe add some other examples too? Like AoE Sleep, AoE Roots. They use the same process but it would still be pretty nice to add here (imo).


Still a T-up from me. This could be useful to beginners in triggering/ the WE.

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

really? the image are showing for me :\

yeah, i will

of course, and i will include the test map

i just need a time to do that

and yeah, i thought this will be useful for beginners at triggering

because i was also like that

btw, thanks hera-chan

#### GreeN!X

#### GreeN!X

 Level 23

JoinedOct 20, 2012Messages3,068

Resources: 4

#### Resources
Icons 1Maps 1Templates 1Tutorials 1

When I try to open the images in a new tab, it's showed as an invalid attachment for me. :/

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

how about now?

#### GreeN!X

#### GreeN!X

 Level 23

JoinedOct 20, 2012Messages3,068

Resources: 4

#### Resources
Icons 1Maps 1Templates 1Tutorials 1

Mucho mejor de.

EDIT:

Much better.


 Last edited: Oct 2, 2013

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

what does that supposed to mean?

lol, thanks


 Last edited: Oct 3, 2013

#### nhocklanhox6

#### nhocklanhox6

 Level 18

JoinedFeb 12, 2012Messages414

Resources: 31

#### Resources
Models 7Spells and Systems 23Tutorials 1

+Rep, nice tutorials . Good Job !

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

thanks onee-san

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

change color. That green is horrible i can't even read the tutorial lol.

In your triggers however you should show them how to do it with only 1 dummy unit even if the spell is cast 1k times.

For this

Set Group = (Units within 200.00 of TempPoint matching ((((Matching unit) is alive) Equal to True) and (((Matching unit) belongs to an enemy of (Owner of Caster)) Equal to True)))
You should get rid of the conditions and just put an ITE in your group loop.

Also you should show them how to remove dead units.

And anything used twice or more should be stored into a variable. Example: Owner of Caster.

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

You should create only one dummy and make it cast the ability on every target. Remember to remove the dummy unit afterwards.

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

Sure, will do

it seems i forgot to remove the dummy >.>

How to create only one dummy? I usually use this so i don't know how

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Create dummy before group loop.

Move dummy unit and order it to do whatever in the group loop.

Then destroy unit after.

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

Okay, thanks

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Remove the Dummy after use.

Attach the image to the tutorial [or put it at Hive's Abum].

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

i already attach it to the tutorial

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I was thinking of creating this long ago but I thought it was too simple.

I dont like the colour you use btw ( in the thread not your posts )

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

Don't forget to set the dummy's art fields Cast Backswing and Cast Point to 0.


 Last edited: Oct 3, 2013

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

Chaosy, yeah, i'll change it.. suggestion for the color??

Chobibo, okay

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

I think it's better to use

Custom script - set bj_wantDestroyGroup = true
Instead of using a group variable that need more memory to be allocate (AFAIK).

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

well, i'm trying to be beginner friendly

because maybe they don't understand about custom scripts yet, so. . .

call RemoveLocation is the easiest script AFAIK, cuz it's the first script i learned xD

#### chobibo

#### chobibo

 Level 22

JoinedSep 24, 2005Messages4,821

That's right, make it as newbie friendly as possible.

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

no suggestions about the text color????

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Rheiko said:

no suggestions about the text color????

Click to expand...


use the default color for posts.

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

lol, okay then

~~~~

Updated

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

Attach a test map.

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

Okay, i will

#### FUJI

#### FUJI

 Level 21

JoinedAug 13, 2013Messages1,709

Resources: 16

#### Resources
Spells and Systems 16

It is hurting my eyes to read the trigger, must do make spaces from comments and don't use condition icon in comments, use a comment icon like adding a ---- ----.

EDIT: Fix the table of contents, some links are bringing me in forums.

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

lol, i will

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

You should mention that you need to set the backswing and cast point to 0 (as chobibo said) in the main post. Otherwise it won't work properly.

Also, a test map would be a good addition to the tutorial.

Otherwise, it looks nice.

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

^I believe movement speed of 0 is also essential.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

I prefer to make the unit under the enumeration of group instead of making 1 dummy, for some reason 1 dummy can fail with 1 simple mistake.

#### FUJI

#### FUJI

 Level 21

JoinedAug 13, 2013Messages1,709

Resources: 16

#### Resources
Spells and Systems 16

@Rheiko

You may want to link knowledge about variables ^^.

EDIT:

Also, the red text and the body of the tutorial hurts my eyes, seperate them by space or enter. Don't make it all Italic letters maybe bold lettters can give attention.

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

~Updated

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

You could mention that the Death type should be set to Can't raise, does not decay so the dummy won't decay there for 90 seconds, it is removed after after the expiration timer and Art - death time.

You could also state that Locust makes the unit invulnerable and unclickable and not collide with other units.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

add expiration timer to the dummy, or the map will lag like shit.

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

Daffa the Mage said:

 add expiration timer to the dummy, or the map will lag like shit.


Click to expand...


In the trigger he posted, there is an expiration timer

#### ILH

#### ILH

Model Reviewer

 Level 35

JoinedMay 8, 2012Messages1,602

Resources: 35

#### Resources
Models 31Icons 1Spells and Systems 2Tutorials 1

Congrats Rheiko!


You're tutorial was very useful for newbies


+Rep

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Maker said:

 In the trigger he posted, there is an expiration timer

Click to expand...


I guess I was a little off guard.


Nevermind then.

Worth for Approval!

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

@maker, will do.. thanks =)

@ILH, thanks

@Daffa, lol and Thanks

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Just played BvO lately and notices that the example skill in this tutorial somehow makes me remember of Tosen's Shikai (First Skill).

Anyway, isn't this suppose to be approved by now?

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

well, PnF is busy

he will approve this when he had a time..

by the way, what is Tosen's first skill? i forgot

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice work. Approved.

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

I would base the ability on Channel. It is best that new users learn to use that ability

The dummy's death type should be can't raise, does not decay so it does not decay there for 90 seconds

Tell people the Locust makes the dummy invulnerable. Often I see Locust paired with invulnerability ability in spells.

The separate invulnerability ability is not needed

You could set the level of the dummy ability to the ability level of the caster

Affected units become invulnerable briefly in the test map

Rather than setting three values to 0 for the dummy to disable its attack, set attacks enabled to none

The dummy gives a huge amount of vision. Usually it is not a problem since the hero is close and give even more vision.

Still, you could set the dummys vision to a much lesser value

Locust disables collision so there is no need to set the movement type to fly

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

Thank you, maker

I'll update it later

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Rheiko said:

My thanks doesn't apply to those who just skip the tutorial and read this part

Click to expand...

Oops.

Anyways. One thing i see is that you should add in a notation and an example of how to do this type of spell using only one dummy unit for the whole game.

On any spell that you think will get used more than a few times you should consider creating only one dummy unit at the beginning of the map. You should then move that dummy unit when you need it and order it to cast the spell onto the units. ( It is not needed but it is a good suggestion for people reading your tutorial)

#### Rheiko

#### Rheiko

Spell Reviewer

 Level 28

JoinedAug 27, 2013Messages4,263

Resources: 16

#### Resources
Icons 2Spells and Systems 12Tutorials 2

okay, thanks ^^

#### FUJI

#### FUJI

 Level 21

JoinedAug 13, 2013Messages1,709

Resources: 16

#### Resources
Spells and Systems 16

deathismyfriend said:

 Oops.


Anyways. One thing i see is that you should add in a notation and an example of how to do this type of spell using only one dummy unit for the whole game.


On any spell that you think will get used more than a few times you should consider creating only one dummy unit at the beginning of the map. You should then move that dummy unit when you need it and order it to cast the spell onto the units. ( It is not needed but it is a good suggestion for people reading your tutorial)


Click to expand...


It is optional ^^, but i prefer creating 1 dummy in every spell per cast in group enumeration and killing the dummy.

#### Malhorne

#### Malhorne

 Level 18

JoinedSep 14, 2012Messages3,413

Resources: 6

#### Resources
Spells and Systems 4JASS 1Tutorials 1

If you cast many times this spell it can lags that's all.

#### deathismyfriend

#### deathismyfriend

 Level 29

JoinedOct 24, 2012Messages6,545

Resources: 18

#### Resources
Spells and Systems 16Tutorials 2

Yes it is optional but new users should know how to do it.

@Malhorne

That is why i suggested for the TO to put it in the tutorial.

 You must log in or register to reply here.


### Similar threads

 Making any Spell Autocasting


Ralle

Mar 15, 2006

Trigger (GUI) Editor Tutorials

Replies12Views59K

Dec 22, 2017
Rogue Kessler

 Introduction To Trigger-Enhancing Spells


whitenights

Jul 27, 2004

Trigger (GUI) Editor Tutorials

Replies6Views36K

Jun 8, 2016
TheLordOfChaos201

 Making a Grenade Type Spell


PrisonLove

Jul 15, 2008

Trigger (GUI) Editor Tutorials

Replies18Views19K

Mar 17, 2018
Bobastian

B

 Spells Importing Tutorial


Daelin

Nov 26, 2006

Trigger (GUI) Editor Tutorials

Replies2Views38K

Oct 26, 2020
FranzLiszt

 How to make a Sword/Slash Ability (No Target)


sPy

Dec 6, 2010

Trigger (GUI) Editor Tutorials

Replies9Views19K

Mar 2, 2014
PurgeandFire

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