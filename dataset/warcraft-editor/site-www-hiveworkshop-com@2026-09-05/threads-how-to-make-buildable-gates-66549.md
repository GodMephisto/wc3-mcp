---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/how-to-make-buildable-gates.66549/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# How to Make Buildable Gates
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/how-to-make-buildable-gates.66549/>
#### Mashintao

#### Mashintao

 Level 4

JoinedFeb 1, 2008Messages25

Resources: 2

#### Resources
Maps 1Tutorials 1

How to Make Buildable Gates!

Introduction

 This tutorial will show you in a step by step process, how to make both horizontal and vertical 'smart gates' that may be built by any standard builder unit. It requires you to make 6 rects, 5 variables, 5 triggers, 4 custom abilities, 4 gates, and 1 builder unit. No uploading is required. The gates will open and close by selecting them and clicking on the open/close abilities. If a unit is blocking the gate while it is closing, then the unit will be moved out of the way in a direction depending on which side of the gate they are on. (Note: Vertical gates will appear to be Horizontal until they are fully constructed.)

-----------------------------------------------------------------------------------------------------------------------

Part 1: The Abilities

 Okay, time to get started! Open the Object Editor by pressing F6, and switch over to the "Abilities" tab. Here, you are going to make two sets of Open and Close abilities that your gates will utilize.

Under the "Neutral Hostile - Units" list, right-click the "Summon Sea Elemental" spell and select "New Custom Ability." Name this new ability as "Open Gate" and hit 'OK'. Then, copy the newly created "Open Gate" spell and paste it 3 more times so you have a total of 4 custom abilities.

Edit their names so that you have 2 "Open Gate" abilities and two "Close Gate" abilities - one of each for vertical and horizontal gates. Also change the "Editor Suffix" property to show (vertical) and (horizontal) so that you can distinguish between the two types.

Change the "Data - number of units summoned" to 0 for all of the abilities, edit the Tooltips and Icons to display something appropriate for a gate opening/closing ability. You may want to set the duration, cooldown, and mana cost to 0, and the Button Position (X,Y) to 0 also.

You should now have 4 properly labeled abilities that have no effect.

Part 2: The Gates

 Now you can create the gates themselves! With the Object Editor still open, switch over to the Units tab. You will need to select a starter building to base your gate off of. Let's use a Human Farm to make things easier.

Right-click on Farm (located in Human -> Melee -> Buildings) and select "New custom Unit," name it as "Gate" and hit "OK."

Again copy the newly created Gate unit and paste it 3 more times so you have 4 Gates total.

Edit the Editor Suffix properties to say (open)(horizontal), (open)(vertical), (closed)(horizontal), and (closed)(vertical) so you can distinguish between the 4 gates.

For each gate, change the "Abilities - Normal" property to its corresponding Open/Close abilities. You should give the following gates these abilities:

Gate(open)(horizontal) = Close Gate(horizontal)

Gate(closed)(horizontal) = Open Gate(horizontal)

Gate(open)(vertical) = Close Gate(vertical)

Gate(closed)(vertical) = Open Gate(vertical)

Now you need to change the statistics to be more Gate appropriate. For all 4 Gates, make the following changes: Model File = Doodads\LordaeronSummer\Terrain\Gate\Gate.mdl ; Unit Soundset = NONE ; Food Produced = 0 ; Techtree - Upgrades Used = NONE ; Ground Texture = NONE

For your vertical gates only: Art - Shadow Texture = ShadowGate3 ; Pathing Map = PathTextures\RoundDoor1Path.tga ; Button Position (X) = 1.

For your horizontal gates only: Art - Shadow Texture = ShadowGate1 ; Pathing Map = PathTextures\Gate1Path.tga ; Button Postion (X) = 0.

Lastly, change the tooltips to say something relative to which gate type they are, and change the Collision Size of the (open) Gates to 0 and the Pathing Map to NONE.

Part 3: The Triggers

 These are the triggers which make your gates function properly. Also, there is a bug that these triggers fix, where if a unit is inside of a gate while it is being closed the gate would hop to a new location. These triggers will instead move the unit out of the way so that the gate can close properly. A separate trigger is necessary to rotate the vertical gates due to the fact that there is no model for them. So here are the triggers you need to make in the trigger editor. I trust you will be able to duplicate them properly? (note that I used "H" and "V" to abbreviate horizontal and vertical.)

Before you begin making the triggers, you need to create 6 rects (also known as region) and 5 point variables with the following names and dimensions (the dimensions don't have to be exact, just close):

HoriDetect : 1000 x 200

HoriTop : 500 x 200

HoriBottom : 500 x 200

HoriTP : (point Variable)

HoriBP : (point Variable)

VertDetect : 200 x 1000

VertLeft : 200 x 500

VertRight : 200 x 500

VertLP : (point Variable)

VertRP : (point Variable)

VertLocation : (point Variable)

(points variables are created in the Variable Editor, which can be accessed by pressing Ctrl + B from the trigger editor)

If you are wondering about how to make the Rect (region) variables have the right dimensions, just create your regions anywhere on the map and edit the "right" and "top" fields to the proper dimensions. Set the "left" and "bottom" fields to 0. The "right" field needs to have the first dimension value and the "top" field needs to have the second value. Here is how it should look:

Trigger for opening Horizontal Gates:

H Gate Open
Events
Unit - A unit Begins casting an ability

Conditions
(Ability being cast) Equal to Open Gate (H)

Actions
Unit - Replace (Casting unit) with a Gate (opened)(horizontal) using The old unit's relative life and mana

Animation - Play (Last replaced unit)'s Death Alternate animation

Trigger for closing Horizontal Gates:

H Gate Close
Events
Unit - A unit Begins casting an ability

Conditions
(Ability being cast) Equal to Close Gate (H)

Actions
Rect - Center HoriDetect <gen> on (Position of (Casting unit))

Rect - Center HoriTop <gen> on ((Position of (Casting unit)) offset by (0.00, 100.00))

Rect - Center HoriBottom <gen> on ((Position of (Casting unit)) offset by (0.00, -100.00))

Set HoriBP = ((Position of (Casting unit)) offset by (-200.00, -300.00))

Set HoriTP = ((Position of (Casting unit)) offset by (200.00, 300.00))

Wait 0.02 seconds

Custom script: set bj_wantDestroyGroup = true

Unit Group - Pick every unit in (Units in HoriDetect <gen>((((Matching unit) is A structure) Equal to False) and (((Matching unit) is A ground unit) Equal to True))) and do (Actions)
Loop - Actions
If ((HoriTop <gen> contains (Picked unit)) Equal to True) then do (Unit - Move (Picked unit) instantly to HoriTP) else do (Do nothing)

If ((HoriBottom <gen> contains (Picked unit)) Equal to True) then do (Unit - Move (Picked unit) instantly to HoriBP) else do (Do nothing)

Wait 0.02 seconds

Custom script: call RemoveLocation(udg_HoriTP)

Custom script: call RemoveLocation(udg_HoriBP)

Unit - Replace (Casting unit) with a Gate (closed)(horizontal) using The old unit's relative life and mana

Animation - Play (Last replaced unit)'s stand animation

Trigger for making Vertical Gates actually be Vertical:

V Gate Rotate
Events
Unit - A unit Finishes construction

Conditions
(Unit-type of (Constructed structure)) Equal to Gate (closed)(vertical)

Actions
Set VertLocation = (Position of (Constructed structure))

Unit - Remove (Constructed structure) from the game

Unit - Create 1 Gate (closed)(vertical) for (Owner of (Constructed structure)) at VertLocation facing 0.00 degrees

Custom script: call RemoveLocation(udg_VertLocation)

Trigger for opening Vertical Gates:

V Gate Open
Events
Unit - A unit Begins casting an ability

Conditions
(Ability being cast) Equal to Open Gate (V)

Actions
Unit - Replace (Casting unit) with a Gate (opened)(vertical) using The old unit's relative life and mana

Animation - Play (Last replaced unit)'s Death Alternate animation

Trigger for closing Vertical Gates:

V Gate Close
Events
Unit - A unit Begins casting an ability

Conditions
(Ability being cast) Equal to Close Gate (V)

Actions
Rect - Center VertDetect <gen> on (Position of (Casting unit))

Rect - Center VertLeft <gen> on ((Position of (Casting unit)) offset by (-100.00, 0.00))

Rect - Center VertRight <gen> on ((Position of (Casting unit)) offset by (100.00, 0.00))

Set VertLP = ((Position of (Casting unit)) offset by (-300.00, -200.00))

Set VertRP = ((Position of (Casting unit)) offset by (300.00, 200.00))

Wait 0.02 seconds

Custom script: set bj_wantDestroyGroup = true

Unit Group - Pick every unit in (Units in VertDetect <gen>((((Matching unit) is A structure) Equal to False) and (((Matching unit) is A ground unit) Equal to True))) and do (Actions)
Loop - Actions
If ((VertLeft <gen> contains (Picked unit)) Equal to True) then do (Unit - Move (Picked unit) instantly to VertLP) else do (Do nothing)

If ((VertRight <gen> contains (Picked unit)) Equal to True) then do (Unit - Move (Picked unit) instantly to VertRP) else do (Do nothing)

Wait 0.02 seconds

Custom script: call RemoveLocation(udg_VertRP)

Custom script: call RemoveLocation(udg_VertLP)

Unit - Replace (Casting unit) with a Gate (closed)(vertical) using The old unit's relative life and mana

Animation - Play (Last replaced unit)'s stand animation

I apologize for the length of these, but it is necessary for determining which side of the gate to move blocking units to.

Part 4: The Builder

 I have saved the most difficult part for last!

Copy a builder of your choice! (peon, peasant, etc) And then Paste a new one entitled "Gate Builder!"

Edit his Techtree - Structures Built to "Gate (closed)(horizontal)" and "Gate (closed)(vertical)"!

Wow, that was rough wasn't it?! Of course, you could simply just edit the structures built of the original builder unit instead of making a whole new one. But that wouldn't be quite as original now, would it?

Additional Information

 There are other methods of making gates work, but this is a simple version that should suit most purposes just fine. If you are capable of making or acquiring a custom Vertical Gate model yourself, then that would cut out the need for the rotation trigger.

 A few side notes:
This tutorial was designed for use with World Editor Unlimited 1.20. I see no reason that you might encounter any problems with other editors, but just FYI...

If you decide to change the size of your gates, be sure that you scale the size of all the rects to match the size of your gate. The triggers will also have to be altered to properly place the rects on each side of the gate to ensure that blocking units are detected and moved properly.

If you prefer to use to chat input or unit proximity to open your gates instead of using abilities, this is also possible, but the method to do so is not included in this tutorial.

I hope this helped you some!


 Last edited: May 4, 2009

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

You can have the gates morph using Metamorphosis provided you give them the Zone Indicator pathing rather than none at all for the open forms (A blank pathing map will cause a game crash upon metamorphosis).

#### Mashintao

#### Mashintao

 Level 4

JoinedFeb 1, 2008Messages25

Resources: 2

#### Resources
Maps 1Tutorials 1

I suppose they could morph, yes, but what would that change? One action from each trigger? The vertical model rotation would still be required, the ability detection for death alternate/stand animations would still be required, and the blocking unit mover would be needed still too.

I guess I could put that in as an alternative method..? That might make things more complicated though... hmmm..... o.0

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

You need no triggers with the alternate method, if you import a vertical gate model (15 kb or so, pretty much just rotated on the axis), and it at least saves the triggers for horizontal gates even if you don't (Needs no triggers). You can just add an Animation Tag to the open gate so that you don't need to play the animation via triggers.

#### Mashintao

#### Mashintao

 Level 4

JoinedFeb 1, 2008Messages25

Resources: 2

#### Resources
Maps 1Tutorials 1

One purpose of this tutorial was to provide a way of having functional gates without doing any model editing or importing. I am well aware that there are 'cleaner' methods for gates, but they are much more complicated (assuming you do the model editing yourself instead of begging), and what I have shown here in my tutorial suits most purposes just as well.


So far I have been unable to reproduce a triggerless gate opening ability with metamorphosis. Structures with the gate model do no properly display their death alternate animation unless they are first replaced with a new version of itself and then prompted to display it with a trigger. This may work with a destructible doodad, but a worker cannot build a destructible, which is also the purpose of this tutorial.


I followed your description of making a permanent metamorphosis ability that changes a closed gate into an opened model with "death,alternate" (or "death alternate") as the required animation name, but I am getting no results aside from client crashes. And yes, I put zone indicator as the pathing map, too. But if you say there is a way, then I will keep working at it.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

If you can't get it to work, don't bother, it's not that big of a deal.

Fix up the leaks in the triggering and this will be approvable.

#### Mashintao

#### Mashintao

 Level 4

JoinedFeb 1, 2008Messages25

Resources: 2

#### Resources
Maps 1Tutorials 1

Leaks fixed to the best of my ability.

#### underscore

#### underscore

 Level 6

JoinedMar 17, 2008Messages150

Resources: 6

#### Resources
Skins 1Tutorials 5

 Leaks fixed to the best of my ability.


Click to expand...

Q: What is your best ability?

Anyways, good tutorial helpful for pro and noob users.

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

You stored the points in the variables correctly, but forgot to remove them via RemoveLocation.

You need to destroy the Regions you make, too, via RemoveRect. Not sure if there is a GUI equivalent for this one.

#### Mashintao

#### Mashintao

 Level 4

JoinedFeb 1, 2008Messages25

Resources: 2

#### Resources
Maps 1Tutorials 1

Erm, yeah I forgot the most important part didn't I? Should be fixed up now. I was tired last night.

underscore said:


Q: What is your best ability?

Click to expand...


0.o I have yet to figure that out!

#### Mashintao

#### Mashintao

 Level 4

JoinedFeb 1, 2008Messages25

Resources: 2

#### Resources
Maps 1Tutorials 1

Wait a minute, why do those regions need to be removed if they are created in the editor and not by the trigger? Repositioning them shouldn't leak, right? They are being reused and not having new ones created. If they are removed then new ones would have to be remade.

(updated triggers, 4/25/08)


 Last edited: Apr 26, 2008

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

Didn't see that you were repositioning them, whoops.

However, you still need to RemoveLocation all your locations.

#### Mashintao

#### Mashintao

 Level 4

JoinedFeb 1, 2008Messages25

Resources: 2

#### Resources
Maps 1Tutorials 1

Final update is now done... hopefully...

#### Archian

#### Archian

Site Director

 Level 69

JoinedJan 1, 2006Messages3,220

Resources: 16

#### Resources
Skins 2Templates 1Sounds 1Tutorials 12

Thread approved.

#### ray12342

#### ray12342

 Level 6

JoinedNov 23, 2006Messages62

Resources: 11

#### Resources
Models 6Maps 4Tutorials 1

Good tutorial But theres about 50 ways i can think of to make buildable gates some only useing 2 triggers and 2 abilitys

#### BlinkBoy

#### BlinkBoy

 Level 23

JoinedNov 11, 2004Messages2,062

Resources: 17

#### Resources
Models 5Icons 1Tools 4Maps 1StarCraft II Resources 1JASS 2Tutorials 3

There's a way in which the only thing you need is photoshop and 1 Morphing ability.


all you do is to create a full Magenta pathmap to the gate, then takes that map path and color the middle with blue. Afterwards you create 2 gates and you assign each of the new pathmaps, then you add a morphing ability between the 2 and that's it.

#### SiK

S

#### SiK

Inthresting... Very usefull to do the real time strategy games!

#### qqluqq

#### qqluqq

 Level 13

JoinedJun 4, 2008Messages299

Resources: 14

#### Resources
Models 5Icons 2Skins 1Tools 1Maps 5

could be lots of simpeler but i want i tut to make a waygate


that i dunno how to do

#### Archimondus

#### Archimondus

 Level 4

JoinedApr 11, 2008Messages47

Resources: 2

#### Resources
Maps 1Tutorials 1

The builder isn't the hardest part. . . Its easy. . . Its for newbs.

#### Statharas

#### Statharas

 Level 23

JoinedJul 9, 2008Messages2,552

Resources: 20

#### Resources
Icons 1Maps 1StarCraft II Resources 5Tutorials 13

Transform

Have you noticed that with this way you can make spells like Transformation?


Like:Unit Uses ability


Conditions:Unitruid uses ability:Tree of Life (Part of WoW's spells from druid class)


Actions:Wait 2 seconds


 |_Change Trigering unit to Tree of Life


 Same Life-mana


and the oposite


Conditions:Unit:Tree of Life uses abilityruid


 Actions:Wait 2 seconds


 |_Change Trigering unit to Druid


 Same Life-mana


Would work......hehe..

#### Statharas

#### Statharas

 Level 23

JoinedJul 9, 2008Messages2,552

Resources: 20

#### Resources
Icons 1Maps 1StarCraft II Resources 5Tutorials 13

The Waygate

if you put the player ownership of a unit


to Waygate Start apearing near your hero and with the trigger to move at the Waygate end it should work.....


Usable:


Trigger:Unit casts an ability


Conditions:Unit is (Example)Portaler


Actions:create unit Zone Indicator next to triggering unit


Wait 0.01 sec


Create Zone Indicator End at Targeted Location


Give Zone Indicator the Waygate ability


Set Zone Indicator's Waygate ability to Last created Zone Indicator End


Wait 15 seconds


Kill Unit Zone Indicator


Kill Zone Indicator End


making triggers that do the things above would make it gr8


If I'm good at this +Rep me please

#### kentzeus

#### kentzeus

 Level 7

JoinedJul 4, 2007Messages249

what is "rects" ?

#### Statharas

#### Statharas

 Level 23

JoinedJul 9, 2008Messages2,552

Resources: 20

#### Resources
Icons 1Maps 1StarCraft II Resources 5Tutorials 13

....?


Rects?

#### nathanman

#### nathanman

 Level 4

JoinedNov 14, 2007Messages50

Resources: 2

#### Resources
Tutorials 2

Rects are WE Unlimited reigions.

#### sordurar

#### sordurar

 Level 3

JoinedMay 13, 2006Messages18

Resources: 1

#### Resources
Maps 1

how you do the vertlocation trigger at the rotation triggering?

#### zipeater1990

#### zipeater1990

 Level 3

JoinedJun 13, 2008Messages59

You can make a very nice gate JUST by using 2 model (open & close version, respectively) AND Tinker's Transform Skill (You know, the tank one, forgot its name). Use open gate as the base unit and close gate as the alternate unit, and you got it! Open gate that cast the transform skill become closed gate!

#### Avex

#### Avex

 Level 18

JoinedFeb 9, 2008Messages1,499

Resources: 27

#### Resources
Icons 19Skins 2Maps 4Tutorials 2

"Rects" are the renamed "Regions" from WE to WEU and UMSWE.

#### ZaZzer

#### ZaZzer

 Level 3

JoinedOct 21, 2008Messages37

Resources: 1

#### Resources
Maps 1

I am in great need for some basic tips when it comes to trigger editing, i do know what event/condition/action does

#### Wc3IsSoFun123

#### Wc3IsSoFun123

 Level 3

JoinedApr 27, 2008Messages18

Resources: 2

#### Resources
Tools 1Tutorials 1

Great tutorial but i cant change Data-Summoned unit count to 0,do i need WE-Unlimited to do thaT?

#### Mashintao

#### Mashintao

 Level 4

JoinedFeb 1, 2008Messages25

Resources: 2

#### Resources
Maps 1Tutorials 1

Wc3IsSoFun123 said:

 Great tutorial but i cant change Data-Summoned unit count to 0,do i need WE-Unlimited to do thaT?


Click to expand...


To change that value to 0, you do need WE-Unlimited. However, you can either make a simple trigger to remove the summoned unit, or you can set the duration to 0.01. Although I'm uncertain if that would cause the gate to be displaced or not. If it does, then add a wait timer at the start of the triggers to allow the summon to dissipate.


... or you could just get WE-Unlimited.

#### naitsirk

#### naitsirk

 Level 10

JoinedJan 24, 2009Messages606

Resources: 2

#### Resources
Spells 2

i have no idea how i am to make the Varibles is it a torturial for Varible making??

#### Mashintao

#### Mashintao

 Level 4

JoinedFeb 1, 2008Messages25

Resources: 2

#### Resources
Maps 1Tutorials 1

I assume you are wondering about how to make the region variables have the right dimensions? Basically, just create your regions anywhere on the map and edit the "right" and "top" fields to the proper dimensions. Set the "left" and "bottom" fields to 0. The "right" field needs to have the first dimension value and the "top" field needs to have the second value. Here is how it should look:

#### saphiree

#### saphiree

 Level 13

JoinedMar 23, 2008Messages1,213

Resources: 1

#### Resources
Maps 1

Dude. WEU dont work for me. It crash and stuff, and i tried to DL from different places. So i better just try to remove the unit... :S

#### Orthon

#### Orthon

 Level 7

JoinedJul 10, 2009Messages71

Resources: 8

#### Resources
Icons 6Skins 2

Yeah, it's better to use morph like druid of the claw bear form

#### danny-345o

#### danny-345o

 Level 2

JoinedJun 23, 2009Messages16

I can't find this trigger "Unit - A unit Begins casting an ability" plzz help me.

#### saphiree

#### saphiree

 Level 13

JoinedMar 23, 2008Messages1,213

Resources: 1

#### Resources
Maps 1

Its under events, subcategory "generic unit event" or "specific unit event", and then its somewhere there.

#### naitsirk

#### naitsirk

 Level 10

JoinedJan 24, 2009Messages606

Resources: 2

#### Resources
Spells 2

I can't find ((HoriTop <gen> contains (Picked unit)) Equal to True) any1 can tell me what Conditions This is??

#### saphiree

#### saphiree

 Level 13

JoinedMar 23, 2008Messages1,213

Resources: 1

#### Resources
Maps 1

Uhm im not sure. But some things in this tut require you to have WEU or some other custom WE. Maybe thats why you cant find it.

#### naitsirk

#### naitsirk

 Level 10

JoinedJan 24, 2009Messages606

Resources: 2

#### Resources
Spells 2

((HoriTop <gen> contains (Picked unit)) Equal to True) is this rect Consists of Point or Please you coud have explained it better

#### naitsirk

#### naitsirk

 Level 10

JoinedJan 24, 2009Messages606

Resources: 2

#### Resources
Spells 2

saphiree said:

 Uhm im not sure. But some things in this tut require you to have WEU or some other custom WE. Maybe thats why you cant find it.


Click to expand...


Well... I am using WEU

#### naitsirk

#### naitsirk

 Level 10

JoinedJan 24, 2009Messages606

Resources: 2

#### Resources
Spells 2

never mind I figured It out

#### naitsirk

#### naitsirk

 Level 10

JoinedJan 24, 2009Messages606

Resources: 2

#### Resources
Spells 2

YAY!! I got it working This Torturial Helped me Become Better in condisions and Actions So i Thank you

#### Tommy gun

#### Tommy gun

 Level 8

JoinedOct 4, 2009Messages108

Resources: 21

#### Resources
Models 1Icons 16Skins 1Tools 1Maps 2

Make Wall

How to i make wall But can a units in there ?????????

#### 88WaRCraFT3

#### 88WaRCraFT3

 Level 13

JoinedJun 9, 2009Messages1,134

Resources: 5

#### Resources
Icons 2Spells and Systems 3

Can you put your map for download?


Im trying to create that but i cant.

#### millzy

#### millzy

 Level 15

JoinedJul 9, 2008Messages1,553

Resources: 7

#### Resources
Maps 5Spells and Systems 2

iv got a problem with vertical gates when building them side by side the second one ends up far away from the first 1 and iv also got a problem with the open animations


im using the bear form way


i made a thread here it is

#### DGTal

D

#### DGTal

I try your tutorial, but some trigger are impossible to do (and im using the weu). Can we have a map with the tuto on? i Wanna see where did i do some mistake. And if i use custom model gate (phdoodaddoor), did i need to change something?

#### Reaper51

#### Reaper51

 Level 11

JoinedAug 24, 2009Messages706

Resources: 2

#### Resources
Maps 2

This really works! Now i have working gates

#### Landscaper

#### Landscaper

 Level 2

JoinedNov 24, 2008Messages14

"HoriTP : (point Variable)"

Ok. Is it a region or a variable?

If region, then what do I put for right and top?

If variable, then for what?

#### Reaper51

#### Reaper51

 Level 11

JoinedAug 24, 2009Messages706

Resources: 2

#### Resources
Maps 2

Landscaper said:

 "HoriTP : (point Variable)"


Ok. Is it a region or a variable?


If region, then what do I put for right and top?


If variable, then for what?


Click to expand...


It's a variable for your gate triggers.

#### Zalashji

#### Zalashji

 Level 4

JoinedJul 5, 2009Messages41

Resources: 3

#### Resources
Maps 3

I don't know really what's wrong, but i can't change the "Data - Summoned Unit Count" to 0. I don't know if it's something wrong with me??? Or your thread, but i suppose it's me???

 You must log in or register to reply here.


### Similar threads

 Opening/Closing Gates


RazorHedgeFan

Jan 4, 2011

Trigger (GUI) Editor Tutorials

Replies29Views29K

Mar 23, 2016
Drankore

Locked

[Trigger]
 iv got some problems with buildable gates


millzy

Nov 18, 2009

Triggers & Scripts

Replies19Views4K

Nov 29, 2009
CrueIIntentions

Locked

[Trigger]
 Serious problem with gate system


klemen

Aug 8, 2010

Triggers & Scripts

Replies2Views2K

Aug 8, 2010
klemen

Locked

[Trigger]
 Gate trigger with transfer issue


The Count

Jun 29, 2015

Triggers & Scripts

Replies7Views2K

Jun 29, 2015
The Count

Locked

[Trigger]
 Destroy killed gates


Phire126

Sep 25, 2007

Triggers & Scripts

Replies6Views2K

Sep 28, 2007
Eleandor

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