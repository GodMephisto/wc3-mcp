---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/chaosys-gui-tutorial-for-beginners.214275/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Chaosys GUI Tutorial for Beginners
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/chaosys-gui-tutorial-for-beginners.214275/>
#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

#

 Chaosy's GUI tutorial for beginners

##
 Introduction

This tutorial is made for those who are new to the world editor and want to learn the basics of GUI triggering. My explanations maybe isn't the "correct" way to explain it but as long you get the point is does not matter.​

###
 Tools needed

Warcraft 3

the World Editor/JNGP (Jass New Gen Pack)

some moments of your time​

###
 What is GUI?

GUI stands for Graphical User Interface and is an easier language made by blizzard. the original language is called JASS its basically the same thing but GUI is limited while JASS is much better.​

###
 How can I use GUI?

You open the trigger the editor and click on the icon which looks like a paper and to the left a trigger will be created in the selected folder and creates a blank trigger, now we can start.​

##
 Events

Events is in simple words when something is happening the events tell when the trigger should start. to use a event you can right click on the red flag and then "new event" then select the event you want. It will look like this

Crete a event
Events
Unit - A unit Dies

Conditions

Actions

There is some events to chose from so I will tell when each event will fire/start

Map Initialization - when the map starts

Destructible dies - when a specific destructible dies example a tree dies.

A destructible within region dies - When a destructible in a area dies example in a specific region

A dialog button is clicked for dialog - When a dialog button is clicked

The in-game time of day becomes equal to X - when the clock in-games becomes X the event don't support PM and AM it uses the 24 hour mode

Variable becomes equal to X - When a real variable becomes equal to X, if you don't know what a variable is don't worry I will explain it later

A saved game is loaded - when a player loads a saved game of the map

The game is about to be saved - When a player starts saving the map

The hero abilities button is clicked - Actually I'm not sure here since I have never seen someone use it, but I think it is when they try to learn a ability after leveling up.

The build structure button is clicked - when a player opens the panel of what his peons can build

player X (color) types a chart message containing X as an exact match - When player X writes X ingame

Player X (color) skips a cinematic sequence - when a player presses the button "esc"

Player X (color) selects a unit - when a player selects any unit in the map

Player X (color) presses the left arrow key - when a play presses >

Player X (color) current gold becomes equal to 1000.00 - when a players gold reaches 1000

Player X (color) changes alliance settings - when a player for examples ally another player or break an alliance

Player X (color) changes shared vision - same as above but only fires on specific alliance changes

player x (color) leaves the game with a victory - when a player who won the game leaves.

Player X (color) leaves the game with a defeat - same as above but lose instead of won.

Player X (color) leaves the game - a player leaves the game before its finished

Elapsed game time is X seconds - after the X seconds in-game

Every X seconds of game-time - every X seconds the trigger will start

Timer expires - when a countdown timers reaches 0

Unit <does something> (many options here) - When a unit does one of the selected things there are too many to write in here. this only fire when a specific unit does X

A unit owned by Player X (color) does something - same as above but fires when a unit owned by a player does X

A unit does something - When ANY unit does X

A unit enter region - when a units walk into a specific region

A unit leaves region - same as above but when he leaves a specific region

A unit comes within X range of unit - when a unit getting close enough into the units radar X

Units life becomes less than X - when a specific units life goes bellow 50 hp

Units mana becomes less than X - same as above but with mana

As you can see you can just read what they do without this guide but its just to clarify with other words.​

##
 Conditions

Conditions is in other words a filter which is very important to avoid bugs.

here is a example I want to create an undead minion when a footman dies. So we want it to fire when ANY unit dies right? now to the condition to filter out other units that are NOT a footman, like this.

Condition test
Events
Unit - A unit Dies

Conditions
(Unit-type of (Dying unit)) Equal to Footman

Actions

As you see the condition checks if the dying unit is a footman and if he is not he condition stops the trigger from continuing.​

##
 Actions

First I will list and explain the in need of better word "persons" you can use when triggering.

Last Created unit - last created unit from the create unit action

Last restored unit - last unit stored from a gamecache (very rarely used)

Last replaced unit - last replaced unit from the "replace unit" action

Last haunted gold min - last haunted gold mine created by triggers

picked unit - picked unit from a unitgroup

matching unit - used when checking things when using a unitgroup

random unit from unitgroup - one of the units in a unitgroup

attacked unit - a attacking unit answers to "unit is attacked/attacking event only

attacking unit - a attacked unit answers to "unit is attacked/attacking event only

buying unit - the unit who buys a unit or a item from a shop

cancelled structure - a building that got canceled

casting unit - unit that cast a spell

constructing structure - building being built

constructed structure - building that got finished recently

damage source - the unit that deals damage

decaying unit - unit that is decaying

dying unit - a unit that recently died

entering unit - unit that enters a region

hero manipulating item - hero that drops/pick up/uses a item

killing unit - a unit who killed another unit

learning hero - hero that learned a skill recently

leaving unit - unit that are leaving a region

leveling hero - hero unit that just leveled up

loading unit - unit that is loaded into a transport (ship,zeplin)

ordered unit - a unit which is ordered to move/cast a spell/stop/patrol

ownership changed unit - a unit that now are owned by another player

researching unit - building who begins/cancel/finishes a upgrade/research

reviveable hero - a heroes revive cooldown has ended

selling unit - the selling building

sold unit - the unit sold from a shop (not trained bought)

summoned unit - unit summoned by a spell

summoning unit - a unit who is summoning another unit

target of issued order - target by a order attacked/repair/move to

target unit of ability being cast - unit targeted by a spell

targeted unit - unit being targeted

trained unit - a unit who recently got trained from a barrack

transporting unit - unit loads units into a transport (ship,zeplin)

triggering unit - the unit who activate the event/trigger this one is much faster than the others so use this instead of killing/dying unit etc if you can

Actions is after the event has fired and passed the condition (filter) the trigger should do something right? else it would be pointless to use the trigger if it doesn't do anything. So to continue on the trigger above we want a undead minion to spawn when a footman dies, then we right click on the action tab and click on "unit" now list of available actions you can use we will use the create unit action to spawn our undead minion.

Spawn action
Events
Unit - A unit Dies

Conditions
(Unit-type of (Dying unit)) Equal to Footman

Actions
Unit - Create 1 Ghoul for Player 1 (Red) at (Center of (Playable map area)) facing Default building facing degrees

its not that hard right? now a undead unit will spawn at the center of the map when a footman dies, this is very buggy action but we will ignore it since its just for showing how it works.

correct
Events
Unit - A unit Dies

Conditions
(Unit-type of (Triggering unit)) Equal to Footman

Actions
Set LOCATION_VARIABLE = (Position of (Triggering unit))

Unit - Create 1 Ghoul for (Owner of (Killing unit)) at LOCATION_VARIABLE facing Default building facing degrees

Custom script: call RemoveLocation(udg_LOCATION_VARIABLE)

##
 Variables

Variables are in very simple words are stored values which are faster to use for your computer. Also if you store a unit in a trigger its still stored if you use the variable in another trigger​

###
 ok how do I create a variable then?

in the trigger editor click on the yellow "X" in toolbar and a window will popup.

Variable creator toolbar info

green X - create a new variable

yellow X - changes the variable

red X - deletes a variable​

###
 Ok i know how to create a variable but what does these field mean?

the variable name can be named anything that suit the kind of variable.

variable type is WHAT can be stored in this variable

array will be used and declared later on​

###
 I can name my variable as I want the "space" is not working!

Instead of naming a variable "i am cool" you must name it "i_am_cool" it does not support space in the names​

###
 I got my varible ready to use how do I store something into it?

by the set variable action

Set example_variable = Battle Roar

###
 Example time with variables!

Now to show how it work we will create a trigger, I want it to when a hero reaches level 10 he will get a message saying congratulations!.

I will use a "string type" variable named gratz text string = text

variable usage example
Events
Unit - A unit Gains a level

Conditions
(Level of (Triggering unit)) Equal to 10

Actions
Set gratz_message = CONGRATULATIONS!!

Game - Display to (All players) the text: gratz_message

##
 Array variables

Array variables is a way to reduce the amount of variables in a map, if you checks the box "array" in the variable creator it will be like this when you store it.

array show
Events

Conditions

Actions
Set Example_variable[1] = Battle Roar

Now you might be a bit confused "why make that number why cant I just skip it?" What i meant with reducing the amout of variables is that in the small number box (array) you can instead of creating 2 unit variables create one with a array.

example

unit array
Events

Conditions

Actions
Set UNTIS_VARIABLE[1] = (Triggering unit)

Set UNTIS_VARIABLE[2] = (Transporting unit)

Set UNTIS_VARIABLE[3] = (Trained unit)

as you see instead of creating 3 different variables I use one single variable to store 3 units​

##
 Leaks

Leaks are a VERY important part of GUI it increases the memory your computer uses to run the map GREATLY, this means one bad coded trigger can destroy the game if there is a lot of leaks in it. The most common ones is point leak, its fixed by storing a point into a variable and then when you not need the point anymore you remove it with

Custom script: call RemoveLocation(udg_point_variable_name_here)
As this is a tutorial for beginners i will not show the other ones but if you still want to know more about leaks here is a few links for you

https://www.hiveworkshop.com/forums...quick-tutorial-common-triggering-tips-190802/

https://www.hiveworkshop.com/forums/trigger-gui-editor-tutorials-279/basic-memory-leaks-5889/

https://www.hiveworkshop.com/forums/triggers-scripts-269/things-leak-35124/​

##
 Challenge yourself

To try what I have showed you I got a task for you, when a grunt dies show the message "a grunt has fallen" and after that create a new grunt somewhere

challenge
Events
Unit - A unit Dies

Conditions
(Unit-type of (Triggering unit)) Equal to Grunt

Actions
Set grunt_string = A grunt has fallen!

Game - Display to (All players) the text: grunt_string

Set LOCATION_VARIABLE = (Center of (Playable map area))

Unit - Create 1 Grunt for Player 1 (Red) at LOCATION_VARIABLE facing Default building facing degrees

Custom script: call RemoveLocation(udg_LOCATION_VARIABLE)

##
 If I was unclear somewhere please ask and I'm open for improvement please comment bellow if I have missed something or ideas for improving

Thanks for reading my tutorial I hope it helped!

//Chaosy


 Last edited: Dec 25, 2016

#### Saif

#### Saif

 Level 11

JoinedJul 9, 2009Messages927

@Darkgrom, you have something wrong. At your ending trigger, you said that you have to spawn a grunt at a random area right ? what you did in your trigger is you are reviving a grunt at the center of the map.

Should'nt it be

Location_Variable= (Random point in (Playable map area))
and what if the owner of the dying unit was not player 1 ?? instead create a player variable and then set it to the owner of dying unit.

GruntOwner= (Owner of Dying Unit)
and then,

Unit - Create 1 Grunt for GruntOwner at Location_Variable facing Default building facing degrees
then clear it with

call RemoveLocation(udg_Location_Variable)
try to teach the leaks in the thread itself instead of linking URL's which contains it. You can put the Linking of the URL's after you have taught us about the leaks.

and for the string message, you do not have to set a string in such a common event, you can just apply it right away, and you have used gratz_message which will not show anything as the string which contains the "Grunt has Fallen" message is grunt_string.

good job on the tutorial, but still needs a lot more information which could not be seen on any other GUI Tutorials. +rep

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

No i meant it does not matter where it spawn and its for beginners i dont want to make it more advanced the tutorial is supposed to show people HOW to trigger so they can start some scripts on their own.

the gratz_message is to show how a variable work only

The leak will just coonfuse people if they are new to it

anyway thanks for the comment i will update it later today


 Last edited: May 14, 2012

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

A lot of people like to look through multiple tutorials, and I think this tutorial may work out for some beginners. There are a few spelling changes, but it is still readable and this tutorial addresses what needs to be addressed without overcomplicating the matter.

As such, I'll approve. =) And don't worry, I'll change the title lol.

#### KeepitMovinBaby

#### KeepitMovinBaby

 Level 6

JoinedJun 5, 2015Messages185

Nice Tutorial

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Fixed spelling errors and grammar errors.

Also made it look a bit nicer structure wise.

Thanks though.

#### Flux

#### Flux

 Level 23

JoinedFeb 6, 2014Messages2,466

Resources: 30

#### Resources
Maps 1Spells and Systems 21JASS 6Tutorials 2

In the Challenge yourself portion, the string variable name does not match.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

True. Fixing.

#### KILLCIDE

#### KILLCIDE

 Level 37

JoinedJul 22, 2015Messages3,484

Resources: 20

#### Resources
Models 2Icons 10Spells and Systems 7Tutorials 1

 Darkgrom's GUI tutorial for beginners


Click to expand...

Darkgrom's GUI tutorial

Click to expand...

Darkgrom's

Click to expand...

​

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Old username. I thought I removed all of the references.

Updated.

 You must log in or register to reply here.


### Similar threads

 Complete and Concise GUI Tutorial


Zypher

Dec 28, 2008

Trigger (GUI) Editor Tutorials

Replies23Views13K

Jan 21, 2014
deathismyfriend

 Basic Triggering


Ralle

Jan 3, 2006

Trigger (GUI) Editor Tutorials

Replies4Views39K

Jul 7, 2016
billel

 Variables


Ralle

Jan 3, 2006

Trigger (GUI) Editor Tutorials

Replies11Views42K

Jul 17, 2021
Daffa

 A Complete Beginners Guide to Hashtables


Jazztastic

Jun 15, 2011

Trigger (GUI) Editor Tutorials
2

Replies57Views45K

Nov 12, 2024
PurgeandFire

 Creep Respawn (GUI)


SkriK

Apr 14, 2008

Trigger (GUI) Editor Tutorials
23

Replies149Views103K

Sep 7, 2020
Atreus

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