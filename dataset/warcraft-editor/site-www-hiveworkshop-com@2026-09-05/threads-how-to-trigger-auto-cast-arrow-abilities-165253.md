---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-trigger-auto-cast-arrow-abilities.165253/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Trigger Auto Cast Arrow Abilities
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-trigger-auto-cast-arrow-abilities.165253/>
#### Stonebludgeon

#### Stonebludgeon

 Level 5

JoinedSep 4, 2008Messages35

Resources: 4

#### Resources
Maps 1Spells and Systems 2Tutorials 1

Triggering Auto-cast Abilities(Difficulty = Easy)

Introduction

This tutorial is made to show GUI users a very easy way to add triggered effects to abilities that auto-cast on attack, such as Searing Arrows. I made this tutorial because I was unable to find any easy GUI tutorial on the subject myself.

Q&A

Q: What do I need to know before I follow this tutorial?

A: How to copy an existing ability and how to edit it, and how to copy an existing buff and edit it (or in other words, not so much).

Q: What will I achieve?

A: You will create an auto-cast on attack ability. You will be able to trigger the effect that occurs on impact of the missile. The part of the ability I teach you will be MUI. (Multi-Unit Instanceability).

Setup

You will need to create a custom ability based off Black Arrow. You can change the icon and descriptions as desired. Damage Bonus can be reduced to 0, or different if you want it so. Same for Mana Cost. Now make a custom buff based off of the black arrow buff (I will still refer to this buff as "Black Arrow"). You will only have to change the name, so you can find it back in the trigger editor, as the icon will never show up (The buff will be instantly removed when placed on a unit).

Triggers

Two triggers are required for the ability to work properly. The first one detects whether a victim unit is attacked by another unit with black arrow. If so, the attacked unit will be added to the second trigger (I will refer to the unit that is damaged by the black arrow as "victim unit").

Detect
Events
Unit - A unit Is attacked

Unit - A unit Starts the effect of an ability

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Black Arrow

Then - Actions
Trigger - Add to Trigger <gen> the event (Unit - (Target unit of ability being cast) Takes damage)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Black Arrow for (Attacking unit)) Greater than 0

Then - Actions
Trigger - Add to Trigger <gen> the event (Unit - (Attacked unit) Takes damage)

Else - Actions

As you can see the victim unit is added to the second triggers' event when black arrow is casted, or when the unit is attacked by an unit with the ability. In the second trigger we detect whether or not the victim unit has the black arrow buff. If not, it wont trigger. But if it has the buff it will remove it, and the rest is up to you to fill in. For example I added an instant kill. But of course its up to you to make a custom ability of it now.

Trigger
Events

Conditions
((Triggering unit) has buff Black Arrow) Equal to True

Actions
Unit - Remove Black Arrow buff from (Triggering unit)

Unit - Kill (Triggering unit)

Notes

- Units are also added to the second trigger even if the autocast ability wasn't on. But that is fine, as it wont kill your computer or anything.

- To make the ability compatible with the black arrow ability you have to make a custom buff, which is not that much work.

- The second event kicks in at the moment the damage is taken, in other words, when the black arrow reaches its destination.

- The "A unit is attacked" event triggers when a unit is about to attack. The "A unit Starts the effect of an ability" event triggers when the unit has casted the spell (and lost its mana).

#### Attachments

[GUI] Autocast Trigger Tutorial.w3x

 16.6 KB

 · Views: 921




 Last edited by a moderator: May 4, 2010

#### Septimus

#### Septimus

 Level 31

JoinedMay 3, 2008Messages3,146

Resources: 50

#### Resources
Packs 1Maps 35Spells and Systems 8Tutorials 6

lol... that's god damn defective trigger.


Why do you need 2 useless trigger when it can be done at 1


Event - Unit is attacked


Condition -


Action - If attacked unit have buff xx equal to true then <insert action here> else

#### Darkness-4ever

#### Darkness-4ever

 Level 9

JoinedNov 4, 2007Messages932

Jeez Sep, harsh much? But yes, your trigger is very defective, in that your second trigger would end up firing everytime the unit is attacked by any unit so long as it has the buff after the event is added. And poorly constructed, the conditions and actions from the second trigger can simply be placed into another if/then/else in the first, I'm certain there are better ways of coding an autocast ability trigger more effectively that can be used to a greater scope.

#### Stonebludgeon

#### Stonebludgeon

 Level 5

JoinedSep 4, 2008Messages35

Resources: 4

#### Resources
Maps 1Spells and Systems 2Tutorials 1

@ Septimus: the unit is attacked trigger procs at the wrong moment, even before the attack is even made. This can be abused, and wont work as the unit wont have the buff by then

@ Darkness-4ever: the buff is removed when the dark arrow hits target

As you dont seem to understand how it works I will update it soon, and add a small example map.

#### Septimus

#### Septimus

 Level 31

JoinedMay 3, 2008Messages3,146

Resources: 50

#### Resources
Packs 1Maps 35Spells and Systems 8Tutorials 6

Stonebludgeon said:

 @ Septimus: the unit is attacked trigger procs at the wrong moment, even before the attack is even made. This can be abused, and wont work as the unit wont have the buff by then

@ Darkness-4ever: the buff is removed when the dark arrow hits target

As you dont seem to understand how it works I will update it soon, and add a small example map.


Click to expand...

Why do you even bother to remove the buff and then only kill the targeted unit? For your information, wait are highly defective and should not be used at all.

If there is 2 unit attacked and get black arrow buff at the same time, the wait would malfunction since it would be overwrite.

Kill
Events
Unit - A unit Is attacked

Conditions

Actions
If (((Triggering unit) has buff Black Arrow) Equal to True) then do (Unit - Kill (Triggering unit)) else do (Do nothing)

This trigger are far simple and work at the same principal with yours. If the unit was attacked and does not have black arrow buff, the event would not be fired at all.

For your information, if the attack are not even made at all. How do you expect the attacked unit to have the black arrow buff. :=/

for your information, you cannot make it MUI unless you use indexing, timer or hashtable.

MUI could never been done with wait.

#### Stonebludgeon

#### Stonebludgeon

 Level 5

JoinedSep 4, 2008Messages35

Resources: 4

#### Resources
Maps 1Spells and Systems 2Tutorials 1

Please read my tutorial a little more careful and try to reproduce it OR try out the map I added to the tutorial. I appriciate the fact that you reply to my post, but please try to be less ignorant.

What your saying does not work. The reason is simple: your "A unit is attacked" event will trigger when the unit starts its attack, and then it hasnt even fired. Ofcourse there is no black arrow buff on the unit then (only if it was shot earlier on, but that is inconsistent).

Thats why I use the "A unit takes damage" event. This event triggers on impact of the missile, when the buff is just placed on the unit. Then the action immediately removes the buff.

Finally, the trigger is MUI even with wait because it only uses local variables (but im not sure what you mean with the wait being highly defective). For more information check this out: http://world-editor-tutorials.thehelper.net/cat_usersubmit.php?view=51725

I hope its clear to you now.


 Last edited: Apr 27, 2010

#### Septimus

#### Septimus

 Level 31

JoinedMay 3, 2008Messages3,146

Resources: 50

#### Resources
Packs 1Maps 35Spells and Systems 8Tutorials 6

 Please read my tutorial a little more careful and try to reproduce it OR try out the map I added to the tutorial. I appriciate the fact that you reply to my post, but please try to be less ignorant.


Click to expand...


I am not trying to be ignorant, but you have to give a good elaboration or else it won't help at all (In this case, the elaboration of yours was poor). Triggering and scripting have various way to be done and yet produce an same output at the same time, you got to make an sample of technique that you deem to be unfit and compare it with your technique by describing what issues separate both of the technique


Here is an example of elaboration.


Create unit xx at xx point offset 60


Create unit xx at xx point offset 120


Create unit xx at xx point offset 180


Create unit xx at xx point offset 240


This method was create 4 unit at xx point with bla bla bla bla.. but it isn't good because bla bla bla


Loop 1 to 4


Create unit xx at xx point offset loop*60


However, this method of mine simply make it bla bla bla

 Finally, the trigger is MUI even with wait because it only uses local variables (but im not sure what you mean with the wait being highly defective). For more information check this out: http://world-editor-tutorials.thehelper.net/cat_usersubmit.php?view=51725


I hope its clear to you now.


Click to expand...


Tutorial at thehelper site was a complete jokes. Almost 70% of it ain't half that useful than wc3c and most of it misdirect the user more than helping it.


Wait are inaccurate and not all spells can be 100% local variable.


And your tutorial does not follow the proper format and guidelines by the staff as well. No matter how good a tutorial would be, if it doesn't follow the guidelines.. it would never been approve.


 Last edited: Apr 28, 2010

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

@Septimus: Waits are MUI, but Globals aren't MUI when used with waits.

The thing about local variables is that they are "local" to the function in which they are being executed in. Say you were to do this:

 JASS:

function MUI takes nothing returns nothing
 local unit x = GetTriggerUnit()
 call TriggerSleepAction(5) //equivalent of "Wait" in GUI
 call KillUnit(x)
 set x = null
endfunction

That is MUI. =) When you use global variables though, such as this:

 JASS:

function NotMUI takes nothing returns nothing
 set udg_Unit = GetTriggerUnit()
 call TriggerSleepAction(5)
 call KillUnit(udg_Unit)
endfunction

Then it is not MUI. =( Since it is a global variable, it has a chance to be overwritten if one function is being executed during the 5 second wait time.

Triggering Unit acts in a similar fashion to a local variable. It is local to the function, thus:

 JASS:

function WhoaThisIsMUI takes nothing returns nothing
 call TriggerSleepAction(5)
 call KillUnit(GetTriggerUnit())
endfunction

Despite you doing the wait for 5 seconds and then using (Triggering Unit), it will work just fine since Triggering Unit is local to the function.

The reason why we don't use waits often though is that most spells are timer-oriented, where they usually use a timer interval of 0.03125 (32 executions per second) or something close to that value. =D (Maybe 0.03 or 0.04) At those intervals, you need a lot of accuracy, where it must be low. However, waits have their minimums, where the lowest wait I've recorded was about 0.7825 or so. It also varies a lot where it can wait anywhere from 0.7825-0.250 when inputting "0" or some other low time eg 0.04 in for the wait.

However, when using something like maybe 1 or so seconds or more, the difference in terms of accuracy becomes less noticeable. That is where it becomes more valid to use. And although you can't beat the timer's accuracy, it is faster to use a wait than to use a timer with hashtables/indexing (if the wait is long enough).

Aaaaandddd... Tutorials at the helper are not complete jokes. =) Some of them may be old, which might mislead a little bit due to that, but there are still many good ones.

@Stonedbludgeon: The tutorial is pretty good. I'll try to formally review it later.

#### Septimus

#### Septimus

 Level 31

JoinedMay 3, 2008Messages3,146

Resources: 50

#### Resources
Packs 1Maps 35Spells and Systems 8Tutorials 6

 @Septimus: Waits are MUI, but Globals aren't MUI when used with waits.


Click to expand...


I know about that, but do you seriously expect a spells to be 100% local variable especially when timer and loop involves?


ohh... i notice where my mistake is, i forgot to mention global variable at here. That is why you and stonedbludgeon confused about it.

 for your information, you cannot make it MUI with global variable unless you use indexing, timer or hashtable.


Click to expand...

 The reason why we don't use waits often though is that most spells are timer-oriented, where they usually use a timer interval of 0.03125 (32 executions per second) or something close to that value. =D (Maybe 0.03 or 0.04) At those intervals, you need a lot of accuracy, where it must be low. However, waits have their minimums, where the lowest wait I've recorded was about 0.7825 or so. It also varies a lot where it can wait anywhere from 0.7825-0.250 when inputting "0" or some other low time eg 0.04 in for the wait.


Click to expand...


That is the ugly part of wait.

 Aaaaandddd... Tutorials at the helper are not complete jokes. =) Some of them may be old, which might mislead a little bit due to that, but there are still many good ones.


Click to expand...


Most of them have very poor elaboration about detail, else what makes you think I leave thehelper and join hiveworkshop and wc3c

#### Stonebludgeon

#### Stonebludgeon

 Level 5

JoinedSep 4, 2008Messages35

Resources: 4

#### Resources
Maps 1Spells and Systems 2Tutorials 1

Why did you start about Global variables if Im only using Locals? :S

And the minimum of a wait function is 0.20 seconds, which makes it pretty useless sometimes.

Thanks for your time

#### Septimus

#### Septimus

 Level 31

JoinedMay 3, 2008Messages3,146

Resources: 50

#### Resources
Packs 1Maps 35Spells and Systems 8Tutorials 6

 Why did you start about Global variables if Im only using Locals? :S


Click to expand...


If you view my upper post, I already mention that spells cannot be 100% local variable especially loop or timer involves. There would always been some sort of setting that require global variable to be used at the specific spells.


You can make a spells 100% local, but not all spells could do that especially in GUI. View approve GUI spells at hive section, all of it involves global variables.


That is why wait should never been used.

 And the minimum of a wait function is 0.20 seconds, which makes it pretty useless sometimes.


Click to expand...


They're always useless and that is the fact that cannot be change.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

I decided I'll approve this tutorial since people will most likely use this technique often. It would be better with a damage detection system (since that way you wouldn't end up adding damage events for the same unit for the same trigger), but this gets the basics down and teaches a nice technique for autocasts.

Nice job, I'll fix grammar and then I'll approve it.

#### Stonebludgeon

#### Stonebludgeon

 Level 5

JoinedSep 4, 2008Messages35

Resources: 4

#### Resources
Maps 1Spells and Systems 2Tutorials 1

Thank you for the appoval. And one could add something that checks if the unit has already been attacked (but does this really cause any lag at all?).

Melee Initialization
Events
Map initialization

Conditions

Actions
Hashtable - Create a hashtable

Set HashTable1 = (Last created hashtable)

Detect
Events
Unit - A unit Is attacked

Unit - A unit Starts the effect of an ability

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Black Arrow

(Load 0 of (Key (Target unit of ability being cast)) from (HashTable1)) Not equal to True

Then - Actions
Trigger - Add to Trigger <gen> the event (Unit - (Target unit of ability being cast) Takes damage)

Hashtable - Save True as 0 of (Key (Target unit of ability being cast)) in (HashTable1)

Else - Actions

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Level of Black Arrow for (Attacking unit)) Greater than 0

(Load 0 of (Key (Attacked unit)) from (HashTable1)) Not equal to True

Then - Actions
Trigger - Add to Trigger <gen> the event (Unit - (Attacked unit) Takes damage)

Hashtable - Save True as 0 of (Key (Attacked unit)) in (HashTable1)

Else - Actions

And the last one (I don't know if this is necessary):

Clear Hashtable
Events
Unit - A unit Dies

Conditions
(Load 0 of (Key (Dying unit)) from (HashTable1)) Equal to True

Actions
Hashtable - Clear all child hashtables of child (Key (Dying unit)) in HashTable1

 You must log in or register to reply here.


### Similar threads

 Making any Spell Autocasting


Ralle

Mar 15, 2006

Trigger (GUI) Editor Tutorials

Replies12Views59K

Dec 22, 2017
Rogue Kessler

 Arrow Key Movement / Camera System


OneEight7even

Nov 15, 2007

Trigger (GUI) Editor Tutorials

Replies32Views28K

Apr 10, 2014
Andser

 How to: Create Spells in GUI - The Beginning


baassee

Aug 18, 2011

Trigger (GUI) Editor Tutorials

Replies6Views32K

Oct 22, 2011
PurgeandFire

 Flexible Arrow Key Movement System


RazorbladeRay

Apr 24, 2010

Trigger (GUI) Editor Tutorials

Replies10Views12K

Sep 29, 2010
xorkatoss

 Creating some basic abilities


paskovich

Apr 22, 2007

Trigger (GUI) Editor Tutorials
23

Replies113Views104K

May 26, 2021
McPhisto2051

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