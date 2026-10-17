---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/creep-respawn-gui.65987/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Creep Respawn GUI
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/creep-respawn-gui.65987/>
#### SkriK

#### SkriK

 Level 16

JoinedMar 26, 2004Messages569

Resources: 35

#### Resources
Icons 34Tutorials 1

Creep Respawn (GUI) - Difficulty: 3/10


-------------------------

Introduction


This tutorial explains how to make units owned by a certain player to respawn at their original position after a given amount of time.

The Tutorial


-------------------------

Setup


There are a few things you'll need to set up. Let's begin with the variables.

Variables


- Open your Trigger Editor and press CTRL+B to open the variable manager.


- While there press CTRL+N to create a new variable.


- Set "Variable Type" to Integer and set "Variable Name" to Temp_Integer. Then press OK.


- Press CTRL+N again to create another variable. Set this one's variable type to Point and name it Creep_Point. In this one, you'll also check the [ ] Array field for the variable. Then press OK.


- One last time, press CTRL+N to create a new variable. Set it's type to Real and name it Respawn_Time. Then press OK.


By now, you should have these 3 variables:

The Map


Now, to respawn units you'll need some units. So go ahead and place some units on your map. Set these unit's owner to Neutral Hostile.


-------------------------

Triggers

Initialization


We'll start off with the Map Initialization trigger, which are the triggers that are run when the map has finished loading.

Map Initialization
Events
Map initialization

Conditions

Actions
Set Respawn_Time = 5.00

Unit Group - Pick every unit in (Units in (Playable map area) owned by Neutral Hostile) and do (Actions)
Loop - Actions
Set Temp_Integer = (Temp_Integer + 1)

Unit - Set the custom value of (Picked unit) to Temp_Integer

Set Creep_Point[Temp_Integer] = (Position of (Picked unit))

By doing this you stored the position for all units owned by Neutral Hostile into the variable you created which will be used to respawn the unit at that location.

Note: The amount you set for Respawn_Time is how long the next trigger will wait before respawning the unit.

The Respawn

Respawn
Events
Unit - A unit Dies

Conditions
(Owner of (Triggering unit)) Equal to Neutral Hostile

(Custom Value of (Triggering unit)) Greater than 0

Actions
Custom script: local integer i = GetUnitTypeId(GetTriggerUnit())

Custom script: local integer ii = GetUnitUserData(GetTriggerUnit())

Wait Respawn_Time game-time seconds

Custom script: call SetUnitUserData(CreateUnit(Player(12),i,GetLocationX(udg_Creep_Point[ii]),GetLocationY(udg_Creep_Point[ii]),270),ii)

Respawn
Events
Unit - A unit Dies

Conditions
(Owner of (Triggering unit)) Equal to Neutral Hostile

(Custom Value of (Triggering unit)) Greater than 0

Actions
Wait Respawn_Time game-time seconds

Unit - Create 1 (Unit-type of (Triggering unit)) for Neutral Hostile at Creep_Point[(Custom value of (Triggering unit))] facing Default building facing (270.0) degrees

Unit - Set the custom value of (Last created unit) to (Custom value of (Triggering unit))

Adding a creep to Respawn


To add creeps, that are spawned during the game's progress, to the "Respawn" trigger you'll have to make this trigger:

Add Creep to Respawn
Events
Unit - A unit enters (Playable map area)

Conditions
(Owner of (Triggering unit)) Equal to Neutral Hostile

Actions
Set Temp_Integer = (Temp_Integer + 1)

Unit - Set the custom value of (Triggering unit) to Temp_Integer

Set Creep_Point[Temp_Integer] = (Position of (Triggering unit))

And that's that! All finished.


In conclusion: This will make a unit owned by Neutral Hostile that dies respawn at it's original position after a set amount of time.

Footnote


This system has only been tested in a small scale and I am totally unaware of leaks, so if anyone could point them out it would be appreciated.


Please comment and criticize the content and be picky about grammar and spelling errors, i don't want it to be faulty.


-------------------------

Hope it was useful! And hope it hasn't been done before! Over and out. ~SkriK

#### Attachments

Untitled-3.gif

 1.6 KB

 · Views: 24,580




 Last edited: Mar 23, 2009

#### underscore

#### underscore

 Level 6

JoinedMar 17, 2008Messages150

Resources: 6

#### Resources
Skins 1Tutorials 5

Cancel the signature and there's already a creep revival here.

#### SkriK

#### SkriK

 Level 16

JoinedMar 26, 2004Messages569

Resources: 35

#### Resources
Icons 34Tutorials 1

Done and awww :<

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

 there's already a creep revival here.


Click to expand...

Could you please provide a link? Search yields no such systems except this one.

#### underscore

#### underscore

 Level 6

JoinedMar 17, 2008Messages150

Resources: 6

#### Resources
Skins 1Tutorials 5

This?

Or maybe I was wrong ....

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

No, that's for making spawns for TDs, etc, so I think this one is a first of the type.

#### underscore

#### underscore

 Level 6

JoinedMar 17, 2008Messages150

Resources: 6

#### Resources
Skins 1Tutorials 5

Oh... First Type

#### ElectricSaiyan

#### ElectricSaiyan

 Level 9

JoinedJul 20, 2005Messages414

Resources: 4

#### Resources
Models 2Tutorials 2

Couldn't you just use a wait timer and then use Replace Unit - Dying Unit with Type of Dying Unit??

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Late reply, but this replaces them where they started, not where they died.

#### SkriK

#### SkriK

 Level 16

JoinedMar 26, 2004Messages569

Resources: 35

#### Resources
Icons 34Tutorials 1

Oooo, so it wasn't a rerun? What can i do to get it aproved?

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

I'll review this tomorrow morning, assuming I don't forget, and assuming I have nothing to do in computer engineering (Otherwise, tomorrow afternoon).

EDIT: Here we go

Respawn_Time isn't really needed, just inline it.

(Triggering Unit) is generally more stable than (Dying Unit), especially after waits. I suggest you use that.

You should store the unit type and custom value of the dying unit in locals before the wait, so that if the respawn time is longer than the decay time it still works.

PolledWait should be used (Wait x Game-Time Seconds) as it factors in Pause Game.

Perhaps add a function to add a new unit to the list of ones that respawn? If you don't know Jass, here's one that would work;

 JASS:

function AddUnitToRespawn takes unit whichUnit returns nothing
 set udg_Temp_Integer = udg_Temp_Integer + 1
 call SetUnitUserData(whichUnit,udg_Temp_Integer)
 set udg_Creep_Point[udg_Temp_Integer] = GetUnitLoc(whichUnit)
endfunction

Neutral hostile units with Custom Value 0 (no respawn point) should not be attempted to revive (this will cause them to appear in the middle of the map).


 Last edited: May 2, 2008

#### SkriK

#### SkriK

 Level 16

JoinedMar 26, 2004Messages569

Resources: 35

#### Resources
Icons 34Tutorials 1

PurplePoot said:

 I'll review this tomorrow morning, assuming I don't forget, and assuming I have nothing to do in computer engineering (Otherwise, tomorrow afternoon).


EDIT: Here we go

[1]Respawn_Time isn't really needed, just inline it.


[2](Triggering Unit) is generally more stable than (Dying Unit), especially after waits. I suggest you use that.


[3]You should store the unit type and custom value of the dying unit in locals before the wait, so that if the respawn time is longer than the decay time it still works.


[4]PolledWait should be used (Wait x Game-Time Seconds) as it factors in Pause Game.


[5]Perhaps add a function to add a new unit to the list of ones that respawn? If you don't know Jass, here's one that would work;

 JASS:

function AddUnitToRespawn takes unit whichUnit returns nothing
 set udg_Temp_Integer = udg_Temp_Integer + 1
 call SetUnitUserData(whichUnit,udg_Temp_Integer)
 set udg_Creep_Point[udg_Temp_Integer] = GetUnitLoc(whichUnit)
endfunction

[6]Neutral hostile units with Custom Value 0 (no respawn point) should not be attempted to revive (this will cause them to appear in the middle of the map).

Click to expand...

1: Sorry, but what does "Inline" mean? (Triggerwise)

2: Fixed

3: Locals? Is that variables that are cleared using custom text after being used?

4: Fixed

5: Done:

Unit - Create 1 Footman for Neutral Hostile at (The Point You Selected)) facing Default building facing (270.0) degrees

Set Temp_Integer = (Temp_Integer + 1)

Unit - Set the custom value of (Last created unit) to Temp_Integer

Set Creep_Point[Temp_Integer] = (Position of (Last created unit))
6: It is set to 1 before storing the first unit.


Will add triggers to the tut once you confirm they're correct.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

 1: Sorry, but what does "Inline" mean? (Triggerwise)


Click to expand...

Get rid of the constant and sub in its value wherever it is used (since this is only used in one, easily accessible place).

 3: Locals? Is that variables that are cleared using custom text after being used?


Click to expand...

Jass only (though you can accomplish them via "Custom Script"), they are variables which pertain only to a specific instance of a specific function.

There is even a trick to make them usable in GUI.

Basically, while this will require Custom scripts for the trigger, it will fix the potential mentioned bug. You would change your death trigger to (This form also fixes said bug):

I added comments to explain what's going on

Respawn
Events
Unit - A unit Dies

Conditions
(Owner of (Triggering unit)) Equal to Neutral Hostile

(Custom Value of (Triggering Unit)) Greater than 0

Actions
Custom script: local integer i = GetUnitTypeId(GetTriggerUnit())

-------- The previous line declares a new local integer, i, which is equal to the type of the dying unit --------

Custom script: local integer ii = GetUnitUserData(GetTriggerUnit())

-------- The previous line declares a new local integer, ii, which is equal to the custom value of the dying unit --------

Wait Respawn_Time seconds

Custom script: call SetUnitUserData(CreateUnit(Player(12),i,GetLocationX(udg_Creep_Point[ii]),GetLocationY(udg_Creep_Point[ii]),270),ii)

-------- The previous line creates a new unit for neutral hostile at the old respawn point of the old type, and sets its custom value to the value of the old unit --------

 6: It is set to 1 before storing the first unit.


Click to expand...

But some (that aren't initialized with this sytem) won't have it and will bug, which would be fixed by a simple condition.

#### Lestat(br)

#### Lestat(br)

 Level 10

JoinedOct 2, 2005Messages396

Resources: 8

#### Resources
Models 7Icons 1

A tip for the ones who wants to put a individual respawn time for each unit-type:


Instead of:

Wait Respawn_Time seconds


Put:

Wait (Real((Point-Value of (Triggering unit)))) seconds


And in the Unit Editor change the Point-Value for this unit-type.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Speaking of which, Wait Game-Time Seconds should be used to prevent bugs.

Also, is this going to be updated?

#### SkriK

#### SkriK

 Level 16

JoinedMar 26, 2004Messages569

Resources: 35

#### Resources
Icons 34Tutorials 1

Yeah, i will update it. Just been a bit busy/lazy lately.

#### Oziris

#### Oziris

 Level 27

JoinedOct 14, 2006Messages4,161

Resources: 7

#### Resources
Icons 1Skins 3StarCraft II Resources 1Tutorials 2

i dont know how to do the second part:


(Owner of (Triggering unit)) Equal to Neutral Hostile


(Custom Value of (Triggering unit)) Greater than 0


i cant find it....

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Integer Comparison, then inside it, (Custom Value of Unit)

#### onix_noob

#### onix_noob

 Level 9

JoinedMay 30, 2008Messages430

Resources: 4

#### Resources
Maps 4

thank you for the great trigger +rep for the good work i found a problem why maximum wait time in all trigger of this type can be around 60-100 seconds when i add wait 320 seconds trigger even don't run why this happen??


 Last edited: Aug 17, 2008

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Fixed a few typos, and approved.

#### SkriK

#### SkriK

 Level 16

JoinedMar 26, 2004Messages569

Resources: 35

#### Resources
Icons 34Tutorials 1

PurplePoot said:

 Fixed a few typos, and approved.


Click to expand...

Great! Thank you ^^

#### Shampoohero

#### Shampoohero

 Level 2

JoinedAug 17, 2008Messages11

Well, i did what i was supposed to do in your tutorial but when i tested it out the the spawn doubled. Ex: 1,2,4,8 etc.


nvm i got it fixed i found out what i did wrong


 Last edited by a moderator: Oct 30, 2009

#### oXReapeRXo

#### oXReapeRXo

 Level 4

JoinedJun 7, 2008Messages87

+rep, I needed that thanks

#### RiskDude

#### RiskDude

 Level 3

JoinedJul 18, 2008Messages53

a little problem here, i made everything like in tutorial and ingame the respawn time is something like Unit - A unit Dies, Action - Immediatelly Respawn :| and i made every thing like in the tut

#### marbib

#### marbib

 Level 3

JoinedDec 12, 2008Messages41

Resources: 2

#### Resources
Maps 2

omg how do we do the "loop action "section ???????????????????

#### SkriK

#### SkriK

 Level 16

JoinedMar 26, 2004Messages569

Resources: 35

#### Resources
Icons 34Tutorials 1

Loop??

#### leet.firefox

#### leet.firefox

 Level 17

JoinedNov 18, 2008Messages1,537

marbib said:

 omg how do we do the "loop action "section ???????????????????


Click to expand...


Unit Group - Pick Every Unit In Unit Group And Do Multiple Actions

#### SkriK

#### SkriK

 Level 16

JoinedMar 26, 2004Messages569

Resources: 35

#### Resources
Icons 34Tutorials 1

You need all the variables set up before doing it.

#### Davdav2

#### Davdav2

 Level 4

JoinedSep 2, 2008Messages40

Resources: 1

#### Resources
Maps 1

???? I understood... NOTHING! BLARGH! Damn, i hate variables...

#### Kruing

K

#### Kruing

Hey, im having problems with the condition:


(Custom Value of (Triggering unit)) Greater than 0


So please anyone help?

#### kirtap1001

#### kirtap1001

 Level 5

JoinedDec 22, 2008Messages52

Resources: 3

#### Resources
Icons 2Tutorials 1

Thanx It works Great! Perfect to my RPG game...

#### kirtap1001

#### kirtap1001

 Level 5

JoinedDec 22, 2008Messages52

Resources: 3

#### Resources
Icons 2Tutorials 1

Kruing said:

 Hey, im having problems with the condition:


(Custom Value of (Triggering unit)) Greater than 0


So please anyone help?

Click to expand...

GO to New Condition.


Scroll Down to "Integer Comparison".


Then Klick on "(Number of units in(Units in(Playable map Area)))"


Scroll Down to "Unit-Custom value of unit"


Then i think you can get the rest.

#### Serefkana

#### Serefkana

 Level 2

JoinedJan 28, 2009Messages15

Thanks for this respawn trigger, mine respawned where the creep died(not good if its pulled XD )


+rep


1question, what exactly is "custom value"? is that like a predefined variable all units have that is defaulted to 0?

#### Kruing

K

#### Kruing

kirtap1001 said:

 GO to New Condition.


Scroll Down to "Integer Comparison".


Then Klick on "(Number of units in(Units in(Playable map Area)))"


Scroll Down to "Unit-Custom value of unit"


Then i think you can get the rest.

Click to expand...


Really big thanks

#### TheBlooddancer

#### TheBlooddancer

 Level 22

JoinedJun 24, 2008Messages3,041

Resources: 2

#### Resources
Maps 1Tutorials 1

You could say so.

The custom value is null to start out with, and if sat it keeps staying like that untill changed.

You cay that custom value is an integer(or real) assigned to a unit.

#### Slaydon

#### Slaydon

 Level 20

JoinedJan 6, 2008Messages2,622

Resources: 10

#### Resources
Icons 4Tutorials 6

Possible to make it respawn th place they are dying in?

#### PsychoNerdial

#### PsychoNerdial

 Level 12

JoinedJan 6, 2009Messages844

PurplePoot said:

Respawn
Events
Unit - A unit Dies

Conditions
(Owner of (Triggering unit)) Equal to Neutral Hostile

(Custom Value of (Triggering Unit)) Greater than 0

Actions
Custom script: local integer i = GetUnitTypeId(GetTriggerUnit())

-------- The previous line declares a new local integer, i, which is equal to the type of the dying unit --------

Custom script: local integer ii = GetUnitUserData(GetTriggerUnit())

-------- The previous line declares a new local integer, ii, which is equal to the custom value of the dying unit --------

Wait Respawn_Time seconds

Custom script: call SetUnitUserData(CreateUnit(Player(12),i,GetLocationX(udg_Creep_Point[ii]),GetLocationY(udg_Creep_Point[ii]),270),ii)

-------- The previous line creates a new unit for neutral hostile at the old respawn point of the old type, and sets its custom value to the value of the old unit --------

But some (that aren't initialized with this sytem) won't have it and will bug, which would be fixed by a simple condition.


Click to expand...


But isn't only possible to set 1 local in every trigger in GUI?

#### magerox

#### magerox

 Level 5

JoinedMar 18, 2009Messages53

Resources: 2

#### Resources
Maps 2

Where can I find


Unit - Set the custom value of (Picked unit) to Temp_Integer


Help me please, thanks.

#### FriXionX

#### FriXionX

 Level 8

JoinedAug 1, 2008Messages420

Unit - Set Custom Value


Then just change the red 'Unit' to (Picked Unit) and change the 0 to Temp_Integer (Variable)

#### magerox

#### magerox

 Level 5

JoinedMar 18, 2009Messages53

Resources: 2

#### Resources
Maps 2

FriXionX said:

 Unit - Set Custom Value


Then just change the red 'Unit' to (Picked Unit) and change the 0 to Temp_Integer (Variable)


Click to expand...


Thanks for telling me, World Editor should make all "Unit - Set" together so it can be found easily.

#### Asgard_Ragna

#### Asgard_Ragna

 Level 10

JoinedJan 14, 2006Messages160

Resources: 3

#### Resources
Spells and Systems 3

PsychoNerdial said:

 But isn't only possible to set 1 local in every trigger in GUI?


Click to expand...


Just if u use custom script to localize Global Variables...at least as far as I know.

#### medion555

#### medion555

 Level 9

JoinedJun 20, 2008Messages476

Resources: 2

#### Resources
Icons 1Maps 1

How do i get the do actions i cant get it. thx

#### LunarLite

#### LunarLite

 Level 8

JoinedApr 8, 2009Messages499

Good job!

Thanx for the first post for the respawn system SkriK, really helped me out with my orpg map.


just a question:


is it possible to make them respawn facing the same angle as when they were placed?


ifso please post the triggers


ofc +Rep is given!

#### Dr. Boom

#### Dr. Boom

 Level 16

JoinedMay 1, 2008Messages1,605

Resources: 4

#### Resources
Maps 1Spells 2Tutorials 1

Seas =)


First: Ofc +rep =)


Second: But i got a problem with:

 JASS:

Custom script: call SetUnitUserData(CreateUnit(Player(12),i,GetLocationX(udg_Creep_Point[ii]),GetLocationY(udg_Creep_Point[ii]),270,ii)


Always shown up a syntax Error from Jass Helper. I looked over it some times, but I cant find any mistake. Are I'm blind or can I ignore this - because the trigger doesn't turn off.

#### Goffterdom

#### Goffterdom

 Level 18

JoinedOct 25, 2006Messages1,173

Tip: for those who want to improve the respawning system :


Just add another variable "Creep_Facing" (Real, Array on).


Groupe unité - Pick every unit in (Units in (Playable map area) owned by Neutre Hostile) and do (Actions)


 Boucle - Actions


-->


 Set Creep_Face[Temp_Integer] = (Facing of (Picked unit))


Then change the trigger when the unit dies.


Unité - Create 1 (Unit-type of (Triggering unit)) for Neutre Hostile at Creep_Point[(Custom value of (Triggering unit))] facing Creep_Face[(Custom value of (Triggering unit))] degrees

#### xxALMIRxxx

#### xxALMIRxxx

 Level 2

JoinedApr 2, 2009Messages29

SkriK said:

Creep Respawn (GUI)


-------------------------


[can you make a map with that tiggers , cuz I´m noobb and i cant make it by my own so be nice and make an map with that trigger in it


i hope you gonna help me

Click to expand...

#### togera90

#### togera90

 Level 4

JoinedMay 20, 2009Messages96

Unit Group - Pick every unit in (Units in (Playable map area) owned by Neutral Hostile) and do (Actions)


there no way to do that


you cant add the "owned by neautral hostile" i dont see a way to do that


(Owner of (Triggering unit)) Equal to Neutral Hostile


 (Custom Value of (Triggering unit)) Greater than 0


i suck at conditions can u tell me where to find those?


(Owner of (Triggering unit)) Equal to Neutral Hostile


 (Custom Value of (Triggering unit)) Greater than 0


can you tell me how to get to those?

[email protected]


Custom script: call SetUnitUserData(CreateUnit(Player(12),i,GetLocationX(udg_Creep_Point[ii]),GetLocationY(udg_Creep_Point[ii]),270),ii)


forget about those other comments i needed help on i figured them out because im smart...but that custom script up there -----^ isnt letting me enable my trigger any help?


and it didnt work for me anyone :[


 Last edited by a moderator: Oct 30, 2009

#### swe.kill

#### swe.kill

 Level 2

JoinedJun 6, 2009Messages14

hey how i get the things i write to the other menu ???? i have testad evrything please help!


how i get the variables to triggers plese help


 Last edited by a moderator: Oct 30, 2009

#### Bwathke

#### Bwathke

 Level 9

JoinedMay 22, 2009Messages491

Resources: 1

#### Resources
Tutorials 1

Does not work at all :/ sorry to tell you...........

#### zarock

#### zarock

 Level 5

JoinedJul 23, 2008Messages186

i dont know why but i did all thatt it says in the totorail but it dosnt work at all

 You must log in or register to reply here.


### Similar threads

Locked

[Solved]
 Unit created with respawn system doesn't do anything


DoomBlade

Jul 19, 2020

Triggers & Scripts

Replies6Views1K

Jul 20, 2020
DoomBlade

Locked

 Need help respawning units from 2 different players.


aussj5link

Dec 28, 2014

World Editor Help Zone

Replies8Views2K

Dec 28, 2014
aussj5link

Locked

[Trigger]
 Creeps dont Respawn after Raise Dead


penguinJAM

Apr 14, 2009

Triggers & Scripts

Replies10Views2K

Apr 14, 2009
Archangel_Tidusx

Locked

 Issue With Creature Respawn Question


C.B.King

Nov 21, 2019

World Editor Help Zone

Replies2Views809

Nov 22, 2019
C.B.King

Locked

[Trigger]
 Respawn and Max Level


NeC-ChRiSSi

Jul 8, 2008

Triggers & Scripts

Replies8Views1K

Jul 9, 2008
ShadowMan

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