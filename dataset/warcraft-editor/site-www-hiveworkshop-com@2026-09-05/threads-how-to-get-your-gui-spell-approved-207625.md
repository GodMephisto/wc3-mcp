---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-get-your-gui-spell-approved.207625/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Get Your GUI Spell Approved
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-get-your-gui-spell-approved.207625/>
#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

#
 How to get your GUI spell approved

##
 Introduction

When uploading a spell, it might not be clear what are the key points moderators look at when reviewing a spell. Often it takes several versions and reviews to get a spell approved. The first time a moderator takes a look at your spell, it might get rejected due to simple things.

Uploading your spell in as complete stage as possible increases you chances to get your spell approved sooner.

##
 Triggers

###
 Leaks

The spell must be leak free. The most common leaks are location leaks (position of...) and unit groups.

Information about leaks and how to get rid of them here: https://www.hiveworkshop.com/forums/triggers-scripts-269/things-leak-35124/

###
 Multi-instanceability

Also known as MUI. The spell must work with more than one unit using it at the same time. Always place two or more casters in your test map. If the ability is not passive, cast the spell with both units almost at the same time. The spell should work flawlessly.

Systems to help you make MUI, pick your favourite:

https://www.hiveworkshop.com/forums/spells-569/gui-unit-indexer-1-2-0-2-a-197329/

https://www.hiveworkshop.com/forums/spells-569/gui-dynamic-indexing-template-144325/

https://www.hiveworkshop.com/forums/trigger-gui-editor-tutorials-279/hashtables-mui-133407/

https://www.hiveworkshop.com/forums...orials-279/visualize-dynamic-indexing-241896/

If you are using waits, there is a good chance your spell is not MUI and will get rejected. Use timers or a looping trigger instead.

Example
Events
Unit - A unit Starts the effect of an ability

Conditions
(Ability being cast) Equal to Animate Dead

Actions
Set unit = (Triggering unit)

Wait 2.00 seconds

Unit - Kill unit

###
 Efficiency

Though efficiency is not that important in GUI, it can still make a difference between an approved an a rejected spell.

Repeated function calls

Whenever there is something inside brackets, it is a function call.

Unit - Kill (Triggering unit)

Triggering unit is in brackets, so it calls a function. Repeated function calls should be avoided.

Untitled Trigger 018
Events

Conditions

Actions
Unit - Add Acid Bomb to (Triggering unit)

Hero - Set (Triggering unit) Hero-level to 2, Hide level-up graphics

Unit - Make (Triggering unit) Invulnerable

Unit - Kill (Triggering unit)

->

Untitled Trigger 018
Events

Conditions

Actions
Set u = (Triggering unit)

Unit - Add Acid Bomb to u

Hero - Set u Hero-level to 2, Hide level-up graphics

Unit - Make u Invulnerable

Unit - Kill u

Triggers that are on for no reason

These include looping triggers most often. Usually looping triggers shouldn't be on if there are no active instances of the spell. Make sure you turn them off.

Trigger - Turn off (*trigger*)

Another example, if you detect the death of dummy units with a trigger, then turn off the trigger if there are no dummies in the map.

###
 Level support

Abilities must support more than one level. For example you can increase the damage of the spell:

Unit - Cause (Triggering unit) to damage (Target unit of ability being cast), dealing (20.00 + (10.00 x (Real((Level of Storm Bolt for (Triggering unit)))))) damage...

You shouold also use the ability level at the time of casting, if you have a periodic loop, you must store the ability level.

###
 Debugging tip

Use debugging messages. If you are using triggers that turn off for example, place a debug message right before you turn the trigger off:

Game - Display to (All players) for 5.00 seconds the text: Debug message

Trigger - Turn off (This trigger)

Use the Game - Display to... for debugging your spell. You can display the value of real and integers, unit types and so on, or just some string.

##
 Object editor

###
 Abilities

Make sure the tooltips do not suck. Look at standard Blizzard abilities for reference. Try to use proper grammar.

Make sure the hotkeys are correct and pay attention to the ability icon position.

You should use active icons for active spells and passive icons for passive spells. Passive icons look like they are pressed down.

Some might find "rainbow tooltips" annoying, so try to avoid using too many colours.

If you are using Channel as the base ability, add about a second of Follow though time. During that time, the caster won't auto-attack, it will keep casting the ability. Adjust the time so that the caster has enough time to end the casting animation, but doesn't start another loop of the animation.

###
 Units

If you are using a dummy, make sure it is set up correctly. Most common mistakes are giving it vision when it shouldn't and not setting it to Can't raise, does not decay.

If your dummy unit has Locust ability, it doesn't need invulnerability ability since Locust makes it invulnerable.

Dummy casters should have Art - Animation - Cast Point set to 0.000, so that they cast the ability instantly. of course, if you want delayed casting, you can adjust the value.

##
 Other

###
 Uploading

The spell must have an in-game screenshot.

Describe what your spell does in the description.

Use a proper name for your ability. Include a version number. The version number should be 1.0 or at least close to it. if you upload v0.01, it signals that it might go through significant changes, so a moderator might skip it. Also, only completed abilities should be uploaded.

It is a good idea to have a change log, so people can see what has been changed between version. Very useful when a moderator checks your spell.

###
 Test map

Always add two or more units with your ability into the map so that the one who reviews your spell won't have to add units himself.

Make sure you have included importing instructions in the map file.

Put triggers that belong to your ability/system into one folder. Put unrelated triggers in other folder(s).


 Last edited by a moderator: Jan 18, 2014

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

I will add many more things tomorrow.

You can suggest your own things to add.

#### Marc Mamales

#### Marc Mamales

 Level 9

JoinedDec 26, 2010Messages475

Resources: 4

#### Resources
Maps 1Spells 2Tutorials 1

Nice guide

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

nice tut wich alot will need to read even myself xd

question does it matter how advanced a spell is to get approved?

#### Adiktuz

#### Adiktuz

 Level 36

JoinedOct 16, 2008Messages10,453

Resources: 30

#### Resources
Models 3Icons 2Packs 1Tools 1Maps 3Spells and Systems 17JASS 2Tutorials 1

not much, though too simple spells are normally rejected...

#### Magtheridon96

#### Magtheridon96

 Level 36

JoinedDec 12, 2008Messages7,384

Resources: 37

#### Resources
Icons 6Tools 1Maps 1Spells and Systems 10JASS 10Tutorials 9

You should also mention how using PolledWait/TriggerSleepAction is bad too.

There are also cases when users do something like this:

If (x == 1) Then
do something with 'blarg'

else
If (x == 2) Then
do something with 'foo'

else
If (x == 3) Then
do something with 'aybabtu'

else
If (x == 4) Then
do something with 'banana'

else

Instead, they should be saving 'blarg', 'foo', 'aybabtu', and 'banana' in an array:

Set array[1] = 'blarg'

Set array[2] = 'foo'

Set array[3] = 'aybabtu'

Set array[4] = 'banana'

And do this instead:

Do something with array[x]
I see this quite often, so I think it's worth mentioning.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Obviously a nice tutorial and very useful as a linkback for spell submissions.

Let me know when it is "finished" (in regards to post #2) by your standards and I'll approve it. (if it is already finished, then let me know since it is approvable in its current state as well)

#### watermelon_1234

#### watermelon_1234

 Level 14

JoinedNov 18, 2007Messages1,084

Resources: 10

#### Resources
Spells and Systems 9JASS 1

For the Triggers section, state that the user should only use arrays if they're actually going to use it as an array or like what Magtheridon96 stated, not to save variable space.

For the Units section, I suggest saying that the user mostly only needs to create one dummy unit to cast an ability on multiple units. The exceptions would be for abilities that are channeling and a few other random ones like Impale.

For the Test Map section, you could tell them to add different test units to allow better testing of the spell, like putting magic-immune units, flying units, mechanical units, etc.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I'm going to approve this since it is already in an approvable state.

#### Maker

#### Maker

 Level 37

JoinedMar 6, 2006Messages9,243

Resources: 18

#### Resources
Maps 2Spells and Systems 15Tutorials 1

Sweet.

#### maddeem

#### maddeem

 Level 24

JoinedJan 1, 2011Messages1,506

Resources: 34

#### Resources
Models 5Icons 3Skins 1Maps 6Spells and Systems 18Lua 1

Under the Other Category I would add that the triggers must be posted and in english.


And perhaps mention originality, if a system/spell already exists there is no reason to create another.


Credits for sure, like say someone used bribe's indexer, credits should be given. Also people shouldn't modify other's resources and repost it.


Otherwise, great tutorial ;p

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I've decided to sticky this thread for prospective spell makers.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Might want to add GPAG into reference, but optional.

 You must log in or register to reply here.


### Similar threads

 Making any Spell Autocasting


Ralle

Mar 15, 2006

Trigger (GUI) Editor Tutorials

Replies12Views59K

Dec 22, 2017
Rogue Kessler

 How to: Create Spells in GUI - The Beginning


baassee

Aug 18, 2011

Trigger (GUI) Editor Tutorials

Replies6Views32K

Oct 22, 2011
PurgeandFire

 Introduction To Trigger-Enhancing Spells


whitenights

Jul 27, 2004

Trigger (GUI) Editor Tutorials

Replies6Views36K

Jun 8, 2016
TheLordOfChaos201

 How to make a Chain spell (GUI Version)


Daelin

Aug 13, 2006

Trigger (GUI) Editor Tutorials

Replies1Views40K

Jan 14, 2019
dower39

 Spell System for GUI MUI Coding


Daffa

May 5, 2019

Trigger (GUI) Editor Tutorials

Replies7Views4K

Oct 3, 2020
Chaosy

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