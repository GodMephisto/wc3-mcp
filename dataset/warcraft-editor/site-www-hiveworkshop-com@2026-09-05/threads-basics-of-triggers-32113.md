---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/basics-of-triggers.32113/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Basics of Triggers
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/basics-of-triggers.32113/>
#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Note, that the tutorial is still in progress. Feel free to post comments & suggestions for improval

Basics of Triggers

By Archian

My ten-year-old cousin has just started warcraft III modding, and has begun experimenting with the Trigger Editor.

I promised that I would make a little tutorial relating to how to make some *very* basic, but useful triggers for custom maps.

This tutorial is also meant as a source of information that may encourage newcomer's to start triggering and from there, develop their knowlege.

So when you're done reading this tutorial, go inside the World Editor and do some experimenting yourself

Click the images for larger versions.

The "target group" of this tutorial is of course; beginners.

Table of Content

Introduction to The World Editors' "Trigger Editor"

GUI (Graphical User Interface)

Trigger Functions: "ECA" (Events, Conditions & Actions)

Trigger Examples

Introduction to The World Editors' "Trigger Editor"

The trigger editor is where the map script is written, either in GUI triggers or directly in JASS.

The map script controls any in-game events not dictated by the user. Triggers are essential for creating maps with complicated goals and atmosphere.

Trigger Editor: (Menu Bar\Module\Trigger Editor)

(From left to right: Variables... (Ctrl+B), New Category (Ctrl+G), New Trigger (Ctrl+T), New Trigger Comment (Ctrl+M), New Event (Ctrl+E), New Condition (Ctrl+D), New Action (Ctrl+R).

Button:Functions:

Variable Editor ~  MrApple's Beginner's Guide to Variables.

Creates a new category for triggers; useful in custom maps with many triggers to keep a better overview.

Creates a new trigger.

A "comment category" which you can use for "headlines" under (for example) categories so each trigger "group" is kept well organized.

Creates a new event for a trigger.

Creates a new condition for a trigger.

Creates a new action for a trigger.

Of course, you can just right-click on the list, a trigger or a category to create the above mentioned.

GUI (Graphical User Interface)

A graphical user interface, is a particular case of user interface for interacting with a computer which employs graphical images and widgets in addition to text to represent the information and actions available to the user.

Usually the actions are performed through direct manipulation of the graphical elements.

Trigger Functions: "ECA" (Events, Conditions & Actions)

Note; each individual trigger can be created using various of countless (describing many) methods.

You will most likely come to discover that as you gain more experience.

There are 3 "categories" of trigger functions (Events, Conditions & Actions):

Trigger Functions
Events

Conditions

Actions

Notice, that these could be compared with the "cause and effect" theory. The cause and effect (Event and Action) with an optional "Condition" included.

When the event(s) occur, all conditions are met, the action(s) will take place.

Events

A trigger will start when any of the events listed occur.

Note; you can "run" a trigger from another trigger without having those events occuring.

Conditions

Conditions are specific requirements which the trigger must observe.

It cannot perform it's actions unless all the condition requirements have been covered.

Note; that you can have 'And' / 'Or' conditions as well. Triggers that are run from another trigger have the option to ignore these conditions.

Actions

If the event occurs, all conditions have been met, then this is the action(s) that the trigger will perform.

Note; that you can may 'Run' other triggers or turn other triggers OFF or ON as well.

Note; You may put any new triggers in the original Melee Initialization category created by the world editor, but you are not obligated to.

Categories have no influence on the functionality of the trigger, so it doesn't matter which order you put them in.

Enable, Initially On & Trigger Comments

Notice that there are 3 fields at the top of each trigger (highlight'ed below).

Enabled - Determines whether or not the trigger is enabled in your map. If 'disabled', the trigger has no function.

Initially On - Triggers can be turned ON and OFF. When checked, the trigger will initially be ON when the game begins.

Trigger Comment - Is a text field area where you can add any comments regarding the specific trigger. Keep in mind that it has no impact on functionality.

Trigger Examples

Below is shown some basic trigger samples.

Opening A Gate #1

In this example, I will demonstrate a way to open a Gate without any condition(s).

Here's the trigger i'm using:

Gate
Events
Destructible - Lever 0039 <gen> dies

Conditions

Actions
Destructible - Open Gate (Horizontal) 0038 <gen>

Opening A Gate #2

In this example, I shall demonstrate a way to open a Gate with a condition.

Here's the trigger:

Gate
Events
Destructible - Lever 0039 <gen> dies

Conditions
(Owner of (Triggering unit)) Equal to Player 1 (Red)

Actions
Destructible - Open Gate (Horizontal) 0038 <gen>

The Sunken Brigde

Here's the trigger i'm using for this demonstration:

Sunken Brigde
Events
Unit - A unit enters Region 000 <gen>

Conditions
(Owner of (Entering unit)) Equal to Player 1 (Red)

Actions
Destructible - Resurrect Short Natural Bridge (Vertical) 0000 <gen> with 2500.00 life and Show birth animation

Animation - Change Short Natural Bridge (Vertical) 0000 <gen>'s animation speed to 95.00% of its original speed

Trigger - Turn off (This trigger)

The Specific Key

Coming soon...

Links

Basics of Regions

 » A MUST READ! If you're new.

Trigger (GUI)/ AI editor Tutorials

Tutorials for beginners

To be continued »

#### adam067

A

#### adam067

hey ive just started war3 modding and i would rly like to now to to set a unit patrol on a target region because im been trying for ages.

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Try to ask your question here. And i guarantee you will get your question answered

#### ringmaster

R

#### ringmaster

how do u get the lever?

#### Herman

#### Herman

 Level 12

JoinedAug 20, 2007Messages866

I like this

This looks like it could be a tremendously great tutorial if it was continued


I would also suggest adding in a little section on loops, how they work and what they do, but add that later, much later so players don't get overwhelmed

#### x-calibur

#### x-calibur

 Level 3

JoinedJan 9, 2007Messages36

this is maybe can help some users

#### Aeroblyctos

#### Aeroblyctos

 Level 37

JoinedAug 14, 2006Messages7,610

Resources: 26

#### Resources
Models 1Packs 1Maps 24

This is a simple, but very good tutorial. I like the pictures you have taken and good luck putting more stuff to this tutorial. This will also save much of my time, because now I don't need to show all n00b map makers how to do basic triggers. This tutorial explains it.

Nice job, Archian.

#### pistolwhip116

P

#### pistolwhip116

That helps alot (10/10)

I just started trigger modding and i am amazed at how many things I can do with them


World editor is one of the easiest-to-use editors I've ever used


(UT04,Gmax,and EA command and conquer are the worst.)


your tutorial is genius (for all the new modders)!!!

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Thank ye sir...

#### ThE0RaVeN

T

#### ThE0RaVeN

yea how do i make my units look like they are walkin on the walls cuz i got some models and i have no idea how to make them walk on the models do i have to make a trigger to make them fly or somthing

#### pistolwhip116

P

#### pistolwhip116

FIRST nice rep I feel loved =-)

#### damurloc

#### damurloc

 Level 2

JoinedAug 30, 2007Messages23

This is most helping, thank you oh repped one...

Im very glad about this, since im kinda a newbie in triggers and other stuff... am good at terraining of course... ANYWAY just wanted to thank you... do something else now... don't keep reading...

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

ThE0RaVeN said:

 yea how do i make my units look like they are walkin on the walls cuz i got some models and i have no idea how to make them walk on the models do i have to make a trigger to make them fly or somthing


Click to expand...

I made this tutorial long ago, might be of use to you.

I think I posted it here at the Hive as well, but I'll redirect you to it here: http://www.wc3campaigns.net/showthread.php?t=85356

Now any further questions should be posted in the World Editor Help Zone forum.

#### Ostaph13

#### Ostaph13

 Level 4

JoinedJun 27, 2008Messages15

Resources: 2

#### Resources
Tutorials 2

Good

This is a good Tutorial for people who don't know triggering!

#### ants376

#### ants376

 Level 3

JoinedMay 25, 2008Messages24

Resources: 1

#### Resources
Skins 1

i wanted to ask u something on the leader bords i needed one for ctf, and how to do a basic ctf if u can make aforem on that thatll be good

#### Pompan

P

#### Pompan

ringmaster said:

 how do u get the lever?


Click to expand...


Press the 'Object manager', then go to 'Destructibles', open the folder 'Trees/Destructibles' in there you can find the Lever. Right click it when you've fount it and click on 'Select in tool palette', now you can create levers!


Hope this brings abit help to you!

#### Pompan

P

#### Pompan

By the way, great help Archian!


Opening a gate #1 and The sunken bridge, worked perfectly. But Opening a gate #2 didn't work out for me .. I wonder why


Anyways, as I've already said great help! +rep

#### XxHunterkillerxX

#### XxHunterkillerxX

 Level 5

JoinedMar 24, 2009Messages72

Resources: 10

#### Resources
Maps 10

Thanks for this tutorial!  Love you!


__________________


"I am... XxHunterkillerxX, god of the galaxy Osore."


 Last edited: Apr 12, 2009

#### naitsirk

#### naitsirk

 Level 10

JoinedJan 24, 2009Messages606

Resources: 2

#### Resources
Spells 2

This is realy helpfull for people like me but i don't get the Sunken bridge to work

#### naitsirk

#### naitsirk

 Level 10

JoinedJan 24, 2009Messages606

Resources: 2

#### Resources
Spells 2

You have to put life to 0% of the bridge to make it work?? that was teh only way i got it to work...

#### AndresMachado

#### AndresMachado

 Level 3

JoinedMar 1, 2014Messages41

Hello! I am TOTALLY new on this, I've been checking out your tutorial and I do get it perfectly! VERY nice thank you VERY much!


But a little question, if I want the door to be 'Invulnerable' so it can't be destroyed and the only way to open it would be the lever, what shall I do?


Thank you!

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

AndresMachado said:

 Hello! I am TOTALLY new on this, I've been checking out your tutorial and I do get it perfectly! VERY nice thank you VERY much!


But a little question, if I want the door to be 'Invulnerable' so it can't be destroyed and the only way to open it would be the lever, what shall I do?


Thank you!


Click to expand...


You would simply make a trigger on map initialization to make the gate invulnerable:

Invulnerable Gate
Events
Map Initialization

Conditions

Actions
Destructible - Make (Gate 0001 <gen>) Invulnerable

#### Grazilda

#### Grazilda

 Level 1

JoinedMay 13, 2018Messages2

Archian said:

 Note, that the tutorial is still in progress. Feel free to post comments & suggestions for improval

Basics of Triggers

By Archian

My ten-year-old cousin has just started warcraft III modding, and has begun experimenting with the Trigger Editor.

I promised that I would make a little tutorial relating to how to make some *very* basic, but useful triggers for custom maps.

This tutorial is also meant as a source of information that may encourage newcomer's to start triggering and from there, develop their knowlege.

So when you're done reading this tutorial, go inside the World Editor and do some experimenting yourself

Click the images for larger versions.

The "target group" of this tutorial is of course; beginners.

Table of Content

Introduction to The World Editors' "Trigger Editor"

GUI (Graphical User Interface)

Trigger Functions: "ECA" (Events, Conditions & Actions)

Trigger Examples

Introduction to The World Editors' "Trigger Editor"

The trigger editor is where the map script is written, either in GUI triggers or directly in JASS.

The map script controls any in-game events not dictated by the user. Triggers are essential for creating maps with complicated goals and atmosphere.

Trigger Editor: (Menu Bar\Module\Trigger Editor)View attachment 4505

View attachment 4507

(From left to right: Variables... (Ctrl+B), New Category (Ctrl+G), New Trigger (Ctrl+T), New Trigger Comment (Ctrl+M), New Event (Ctrl+E), New Condition (Ctrl+D), New Action (Ctrl+R).

Button:Functions:

View attachment 4508Variable Editor ~  MrApple's Beginner's Guide to Variables.

View attachment 4498Creates a new category for triggers; useful in custom maps with many triggers to keep a better overview.

View attachment 4501Creates a new trigger.

View attachment 4504A "comment category" which you can use for "headlines" under (for example) categories so each trigger "group" is kept well organized.

View attachment 4500Creates a new event for a trigger.

View attachment 4499Creates a new condition for a trigger.

View attachment 4497Creates a new action for a trigger.

Of course, you can just right-click on the list, a trigger or a category to create the above mentioned.

View attachment 4502

GUI (Graphical User Interface)

A graphical user interface, is a particular case of user interface for interacting with a computer which employs graphical images and widgets in addition to text to represent the information and actions available to the user.

Usually the actions are performed through direct manipulation of the graphical elements.

Trigger Functions: "ECA" (Events, Conditions & Actions)

Note; each individual trigger can be created using various of countless (describing many) methods.

You will most likely come to discover that as you gain more experience.

There are 3 "categories" of trigger functions (Events, Conditions & Actions):

Trigger Functions
Events

Conditions

Actions

Notice, that these could be compared with the "cause and effect" theory. The cause and effect (Event and Action) with an optional "Condition" included.

When the event(s) occur, all conditions are met, the action(s) will take place.

View attachment 4494Events

A trigger will start when any of the events listed occur.

Note; you can "run" a trigger from another trigger without having those events occuring.

View attachment 4493Conditions

Conditions are specific requirements which the trigger must observe.

It cannot perform it's actions unless all the condition requirements have been covered.

Note; that you can have 'And' / 'Or' conditions as well. Triggers that are run from another trigger have the option to ignore these conditions.

View attachment 4491Actions

If the event occurs, all conditions have been met, then this is the action(s) that the trigger will perform.

Note; that you can may 'Run' other triggers or turn other triggers OFF or ON as well.

Note; You may put any new triggers in the original Melee Initialization category created by the world editor, but you are not obligated to.

Categories have no influence on the functionality of the trigger, so it doesn't matter which order you put them in.

Enable, Initially On & Trigger Comments

Notice that there are 3 fields at the top of each trigger (highlight'ed below).

View attachment 4506

Enabled - Determines whether or not the trigger is enabled in your map. If 'disabled', the trigger has no function.

Initially On - Triggers can be turned ON and OFF. When checked, the trigger will initially be ON when the game begins.

Trigger Comment - Is a text field area where you can add any comments regarding the specific trigger. Keep in mind that it has no impact on functionality.

Trigger Examples

Below is shown some basic trigger samples.

Opening A Gate #1View attachment 4494View attachment 4491

In this example, I will demonstrate a way to open a Gate without any condition(s).

Here's the trigger i'm using:

Gate
Events
Destructible - Lever 0039 <gen> dies

Conditions

Actions
Destructible - Open Gate (Horizontal) 0038 <gen>

View attachment 4496

Opening A Gate #2View attachment 4494View attachment 4493View attachment 4491

In this example, I shall demonstrate a way to open a Gate with a condition.

Here's the trigger:

Gate
Events
Destructible - Lever 0039 <gen> dies

Conditions
(Owner of (Triggering unit)) Equal to Player 1 (Red)

Actions
Destructible - Open Gate (Horizontal) 0038 <gen>

View attachment 4495

The Sunken BrigdeView attachment 4494View attachment 4493View attachment 4491

Here's the trigger i'm using for this demonstration:

Sunken Brigde
Events
Unit - A unit enters Region 000 <gen>

Conditions
(Owner of (Entering unit)) Equal to Player 1 (Red)

Actions
Destructible - Resurrect Short Natural Bridge (Vertical) 0000 <gen> with 2500.00 life and Show birth animation

Animation - Change Short Natural Bridge (Vertical) 0000 <gen>'s animation speed to 95.00% of its original speed

Trigger - Turn off (This trigger)

View attachment 4503

View attachment 4492

The Specific Key

Coming soon...

Links

Basics of Regions

 » A MUST READ! If you're new.

Trigger (GUI)/ AI editor Tutorials

Tutorials for beginners

To be continued »

Click to expand...

I have a question?

Will the trigger will work withlut the event just conditions and actions?

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Grazilda said:

 I have a question?

Will the trigger will work withlut the event just conditions and actions?


Click to expand...

You need to call the said trigger from another trigger which has an event.

#### Superfrycook

#### Superfrycook

 Level 47

JoinedApr 12, 2019Messages832

Resources: 291

#### Resources
Packs 1Skins 1Maps 29Reforged HD Models 231Reforged HD Skins 18Reforged HD Icons 8Sounds 3

##
 Republic Anthology


How can I create a trigger by destroying an enemy base?

#### Warseeker

#### Warseeker

 Level 32

JoinedFeb 18, 2014Messages3,726

Resources: 6

#### Resources
Maps 5Tutorials 1

Ken Lenard Navarro said:

 How can I create a trigger by destroying an enemy base?


Click to expand...

Trigger
Events
Time - Every 2.00 seconds of game time

Conditions
(Count structures controlled by (Enemy Player) (Exclude incomplete structures)) Equal to 0

Actions
-------- Do your stuff here --------


 Last edited: Aug 16, 2019

#### dantles1992

#### dantles1992

 Level 6

JoinedAug 15, 2022Messages116

I apologize for posting here but I have a question, I am making private edits on a map that has a "pick a race" trigger so I wanna change the name of that one race, how can I do that? pleas can anyone tell me

 You must log in or register to reply here.


### Similar threads

 Basic Triggering


Ralle

Jan 3, 2006

Trigger (GUI) Editor Tutorials

Replies4Views39K

Jul 7, 2016
billel

 Opening/Closing Gates


RazorHedgeFan

Jan 4, 2011

Trigger (GUI) Editor Tutorials

Replies29Views29K

Mar 23, 2016
Drankore

 Basics of a Dialog


Orc_Tamer

Mar 7, 2007

Trigger (GUI) Editor Tutorials

Replies6Views29K

Dec 5, 2019
tough_zai

 Create precise timing cinematic using real variable


Septimus

Apr 23, 2009

Trigger (GUI) Editor Tutorials

Replies17Views9K

Jun 27, 2009
Dem0n_Hunter

 Game Caches


Bob27

Mar 18, 2006

Trigger (GUI) Editor Tutorials

Replies34Views52K

Jul 21, 2018
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