---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/making-elevators.108366/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Making Elevators
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/making-elevators.108366/>
#### cool_is_i

#### cool_is_i

 Level 12

JoinedOct 23, 2007Messages565

Resources: 19

#### Resources
Icons 9Maps 8Tutorials 2

###
 Diffiuclty: 5/10

##
 Quick Links:

[click on a area of the list to skip to that point of the page]

Introduction

-Elevator Necessities-

1] About Pathing

2] About Elevator Levels

3] About Locating Elevator Doodads

4] About Elevator Walls

-A Simple Elevator-

1] Making the Terrain

2] Adding the Elevator

3] Placing Elevator Walls

4] Making the Elevator Rise

5] Making the Elevator Lower

6] Finishing off

#
 Introduction:

I read a tutorial on making elevators quite a while back, and dnt understand it at all.


So recently, i was playing around, and got it to work, as i knew more about triggers now then when i was younger.


And i decided to help other people make elevators, as they are a wicked addition to any dungeon, as a trap, or just to make it look cool.


This is a step by step tutorial on how to do this.


You can skip part 1 -Elevator Necessities- if you are good with the editor.

##
 How to do it:

###
 -Elevator Necessities-


1] The most important thing to do, is press P once the editor is opened up, this allows you to see pathing


[Pathing allows you to see where units can, and cannot walk:

Blue ground=Walkable |

Pink Ground=Unwalkable]


 2] Now, you gota know about elevator levels.


There are 3 levels:


Level 1 -

 Is flat ground


Level 2 -

 Is raised [by one] ground


Level 3 -

 Is raised [by two] ground


3] Heres how/where to find the elevator:


Doodads > Bridges and Ramps > Elevator(1/2)


And here is where the elevator switch is:


[the thing that a unit steps on, to activate the elevator]


Doodads > Bridges and Ramps > Foot Switch


4] Elevator Walls.


These are like path blockers.


[they are invisible, and just block units from entering that area]


There are two ways it can be placed:

##
 -A Simple Elevator-

Note: If you dont know what some of the stuff here is,


please read part 1 -Elevator Necessities-


The simple elevator lifts unit that step on it, up and down.


There are no switches involved.


 1] Now on to the more complex stuff


Make the terrain you want, this should be suitable:


2] Ok, now you must add the elevator:


Place it here, in the middle:


3] For a really simple elevator, you dont need foot switches, this works too.


Though you still have to add elevator walls.


Place therm here:


You MUST make sure they are facing the right angle,


they should be like this, and should block off every entrance to the elevator:


If the elevator blockers cannot be placed, hold shift, then place it.


[when you hold shift, it reduces the pathing size of the selected doodad]


Then double click on the bottom elevator wall, and reduce its health to 0


[if its 0, the doodad is dead, if its dead, units can walk though it allowing them to walk onto the elevator]


4] Now we make the triggers.


Press the

 near the top of the editor to open up the Trigger Editor.


Right click under Initialization, and make a new folder, lets name this Elevator.


Click on the folder, right click, new trigger, name this Elevator up.


Right click Events and make a new Event


[this is the thing that starts the action]


Heres how to do it:


Click Select a region, make a new region in this area:


Press space and drag over the area you want the region over.


[when you press space, it allows you to place things [units, regions, etc]]


Press space and click on the region to select it.


Then accept, the trigger should look like this:

Unit - A unit enters Region 000 <gen>
Now make and action [right click, new action]


Like this:


It should look like this:

Wait 2.00 seconds
The trigger so far:


-when a unit enters the selected region


-wait 2 seconds before carrying out the next action


Make another action,


revives a doodad:


make sure to revive the doodad that you "killed" earlier on.


Another action,


Raise the elevator:


Select the elevator for this trigger.


raise it to 2 [see part 1 -Elevator Necessities- ]


now add another wait 1s to the trigger list.


then kill, these doodads:


[killing these doodads allow units to walk on them, allowing them to leave the elevator]


it should look like this:


The trigger so far:


-when a unit enters the selected region


-wait 2 seconds [before carrying out the next action]


-revive elevator wall


-raise the elevator to the 1st floor


-wait 1 second [gives time for the elevator to get to the floor you want]


-kill 3 elevator walls

Elevator up
Events
Unit - A unit enters Region 000 <gen>

Conditions

Actions
Wait 2.00 seconds

Destructible - Resurrect Elevator Wall 0001 <gen> with 100.00 life and Show birth animation

Destructible - Set height of Elevator (2) 0000 <gen> to 2

Wait 1.00 seconds

Destructible - Kill Elevator Wall 0002 <gen>

Destructible - Kill Elevator Wall 0004 <gen>

Destructible - Kill Elevator Wall 0003 <gen>

 Thats it for raising it


4.5] Not necessary, but you can remove these triggers in Initialization to make your custom map run smoother.

Melee Game - Enforce victory/defeat conditions (for all players)
 - If you delete this, it stops the game from ending, if there is no player 2.

Melee Game - Create starting units (for all players)
 - When this is removed, it stops the computer from making starting units.


[5 Pesants + a Townhall [for humans]]


5] Now to lower it.


create a new trigger, lets call it, elevator down.


Make the same event as last time. [unit enters region]


For the actions, it is similar to last time as well


so you use the same technique:


-wait 2s


-revive these doodads:


-set height of elevator to 1


-wait 1s


-kill this doodad:

Elevator down
Events
Unit - A unit enters Region 000 <gen>

Conditions

Actions
Wait 2.00 seconds

Destructible - Resurrect Elevator Wall 0003 <gen> with 100.00 life and Show birth animation

Destructible - Resurrect Elevator Wall 0002 <gen> with 100.00 life and Show birth animation

Destructible - Resurrect Elevator Wall 0004 <gen> with 100.00 life and Show birth animation

Destructible - Set height of Elevator (2) 0000 <gen> to 1

Wait 1.00 seconds

Destructible - Kill Elevator Wall 0001 <gen>


6] The final step.


Add another action to the trigger: Elevator up


The trigger should look like this:


and another trigger, it should look like this:


then add these actions to Elevator Down, but reversed.

Trigger - Turn on Elevator up <gen>

Trigger - Turn off (This trigger)
Then finally select the Elevator Down trigger, and uncheck this box:


These triggers basically do this:


after the elevator goes up, turn this trigger off, [because it cant to up twice], and turn on Elevator down, so that the next unit to step on the elevator, will make it go down.


And thats it folks, wer done


I hope this helps someone,


and i will be adding more to this, about different elevators [including switches] and things like that.


Also take a look at the attached map


Cheers

#### Attachments

Elevator_Tutorial.w3x

 20.8 KB

 · Views: 781




 Last edited: Jan 23, 2009

#### TheBlooddancer

#### TheBlooddancer

 Level 22

JoinedJun 24, 2008Messages3,041

Resources: 2

#### Resources
Maps 1Tutorials 1

Nice tutorial, and you even highlight it.

+Rep.

/Tomorrow, i've spread to much today.

#### cool_is_i

#### cool_is_i

 Level 12

JoinedOct 23, 2007Messages565

Resources: 19

#### Resources
Icons 9Maps 8Tutorials 2

lol thanks

edit:

do i have to ask a mod to approve this? or will thye check it eventually?


 Last edited: Nov 27, 2008

#### whitecrusader13

#### whitecrusader13

 Level 4

JoinedJul 29, 2008Messages84

omg finally i did it xD

THX a lot!!!

#### cool_is_i

#### cool_is_i

 Level 12

JoinedOct 23, 2007Messages565

Resources: 19

#### Resources
Icons 9Maps 8Tutorials 2

hehe np bro, thats what this is for

edit: how do i get this approved?


 Last edited: Dec 1, 2008

#### TheBlooddancer

#### TheBlooddancer

 Level 22

JoinedJun 24, 2008Messages3,041

Resources: 2

#### Resources
Maps 1Tutorials 1

You wont. We lack GUI moderators, lately. You must wait.

#### Airandius

#### Airandius

 Level 17

JoinedJun 12, 2007Messages1,261

Resources: 8

#### Resources
Icons 2Maps 1Ported Icons 4Tutorials 1

Nice tut m8, though no GUI Tutorial Modz here, so wait for a few months till there is one.

#### Cheezeman

#### Cheezeman

 Level 10

JoinedAug 19, 2008Messages491

Resources: 3

#### Resources
Spells 1Tutorials 2

TheBlooddancer said:

 You wont. We lack GUI moderators, lately. You must wait.


Click to expand...

Haha I can tell.

I've had my tutorial uploaded since like august or something...

It isn't as awesome as this guy's, but it's my first

https://www.hiveworkshop.com/forums/f283/create-sucessfull-timer-window-reuploaded-97651/

+Rep to you, cool_is_i

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Okay first off, get rid of all the color tags. They're unnecessary. Use

#
 ,

##
 , and

###
 tags instead.

#### Airandius

#### Airandius

 Level 17

JoinedJun 12, 2007Messages1,261

Resources: 8

#### Resources
Icons 2Maps 1Ported Icons 4Tutorials 1

Pyritie said:

 Okay first off, get rid of all the color tags. They're unnecessary. Use

#
 ,

##
 , and

###
 tags instead.

Click to expand...

#

##

###


Indeed, you do spam colors a bit, you can use them to highlight the header tags to make it less boring though.


I always make my headers colored cyan cause that's flashy yet a very relaxing color. ^^


Also there is a trigger to open/close elevator walls, it's better to use them then to resurrect/kill them all the time.

#### zDToXic

#### zDToXic

 Level 3

JoinedDec 28, 2008Messages35

Very nice iam use this 4 my ORPG thxs

#### cool_is_i

#### cool_is_i

 Level 12

JoinedOct 23, 2007Messages565

Resources: 19

#### Resources
Icons 9Maps 8Tutorials 2

Eh,

i dont think i spammed colors

Green = explination

Blue = Numbering

Teal = progress

etc etc

#### Airandius

#### Airandius

 Level 17

JoinedJun 12, 2007Messages1,261

Resources: 8

#### Resources
Icons 2Maps 1Ported Icons 4Tutorials 1

cool_is_i said:

 Eh,


i dont think i spammed colors


Green = explination


Blue = Numbering


Teal = progress


etc etc


Click to expand...


Stick with one for headers.  That's more then enough.


To much colors would make it happy, and we don't want that! (Joke)


Nah, it will only fry our brains.

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

cool_is_i said:

 Eh,


i dont think i spammed colors


Green = explination


Blue = Numbering


Teal = progress


etc etc


Click to expand...


Just use headers or at least font colors that go well with the hive's theme.

#### s4nji

#### s4nji

 Level 15

JoinedSep 1, 2008Messages475

Resources: 23

#### Resources
Models 20Packs 2Spells and Systems 1

Man! I don't found even think about 'em! Thx!

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Oh, and please remove your signature from the post too. :3

#### s4nji

#### s4nji

 Level 15

JoinedSep 1, 2008Messages475

Resources: 23

#### Resources
Models 20Packs 2Spells and Systems 1

Pyritie said:

 Oh, and please remove your signature from the post too. :3


Click to expand...

Umm... Why ?

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

s4nji said:

 Umm... Why ?

Click to expand...


Not you.

#### Airandius

#### Airandius

 Level 17

JoinedJun 12, 2007Messages1,261

Resources: 8

#### Resources
Icons 2Maps 1Ported Icons 4Tutorials 1

s4nji said:

 Umm... Why ?

Click to expand...


If somebody posts a tutorial it should not display a signature in the tutorial itself.


It's not directed to you, it's directed to the person who made this tutorial.

#### cool_is_i

#### cool_is_i

 Level 12

JoinedOct 23, 2007Messages565

Resources: 19

#### Resources
Icons 9Maps 8Tutorials 2

thanks m8

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

There, I removed all the color. Now please add

#

##
 and

###
 tags appropriately and I'll approve this.

#### cool_is_i

#### cool_is_i

 Level 12

JoinedOct 23, 2007Messages565

Resources: 19

#### Resources
Icons 9Maps 8Tutorials 2

Thanks alot

Tags, done and done.

Thx pyritie

Thx guys

#### Pyritie

#### Pyritie

 Level 49

JoinedNov 26, 2006Messages11,133

Resources: 52

#### Resources
Models 30Icons 9Packs 3Tools 1Maps 2Spells and Systems 1User Interface 1Tutorials 5

Alrighty then, approved.

#### Super-Luke

#### Super-Luke

 Level 3

JoinedApr 26, 2008Messages49

Great tut did it and worked great !


Just one point


You should switch round the wait and the resurecting of the wall, people can walk on then off and the elevator still goes up, if this happened in a game... well the elevator would be pointless


But great tut =) so well layed out even i could understand xD

#### cool_is_i

#### cool_is_i

 Level 12

JoinedOct 23, 2007Messages565

Resources: 19

#### Resources
Icons 9Maps 8Tutorials 2

thx ^^ glad it helped

#### RazorTroll

#### RazorTroll

 Level 2

JoinedMay 8, 2009Messages15

nice espaning

#### FatNastyOgre

#### FatNastyOgre

 Level 3

JoinedJul 19, 2009Messages24

Really Nice ^^

#### SengokuBasara

#### SengokuBasara

 Level 2

JoinedApr 23, 2009Messages11

i tested it out, is the unit suppose to be able to go down instantly off the elevator?

#### kimberker

#### kimberker

 Level 3

JoinedNov 8, 2009Messages46

When i make that, it goes up but dont not go down. I follow all ur steps. Help me...

#### Cweener

#### Cweener

 Level 6

JoinedOct 10, 2009Messages1,426

Resources: 4

#### Resources
Maps 4

helped complete my elevator

+rep

#### orcyngiser

#### orcyngiser

 Level 3

JoinedApr 9, 2010Messages60

Resources: 1

#### Resources
Maps 1

ingenius!

#### Warcraft_Tester

#### Warcraft_Tester

 Level 5

JoinedSep 21, 2010Messages142

Resources: 1

#### Resources
Spells 1

dude i know a easier way its very simple.

#### Orcnet

#### Orcnet

 Level 30

JoinedJul 31, 2010Messages5,248

Resources: 5

#### Resources
Maps 4Tutorials 1

This is the only tutorial that helps map makers do an elevator method, thanks for helping us!

 You must log in or register to reply here.


### Similar threads

Locked

 Elevator Usage.


Rigborn

May 23, 2018

Triggers & Scripts

Replies2Views1K

May 24, 2018
Rigborn

Locked

[Trigger]
 Elevator trigger help


phoenixlord882

Jan 16, 2009

Triggers & Scripts

Replies1Views993

Jan 16, 2009
Avator

K

Locked

[Trigger]
 Elevator Trap Trigger Help


Kzl

Jul 28, 2009

Triggers & Scripts

Replies4Views1K

Jul 28, 2009
Kzl

K

Locked

 Couple of trigger-related problems


The Druidinator

Sep 27, 2010

Map Development

Replies7Views2K

Oct 1, 2010
axleion

Locked

[Trigger]
 Info about Elevators =) how to place


Demonictroll

Aug 19, 2007

Triggers & Scripts

Replies2Views1K

Aug 20, 2007
MagnusCalgar

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