---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/opening-closing-gates.186506/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Opening Closing Gates
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/opening-closing-gates.186506/>
#### RazorHedgeFan

#### RazorHedgeFan

 Level 3

JoinedJan 3, 2011Messages14

Resources: 2

#### Resources
Maps 1Tutorials 1

# Opening/Closing Gates

-By RazorHedgeFan

Description:

In this tutorial, I will show you how to open and close gates in the editor using 3 different methods. Gates are means of entrances and exits which can open or close. In Warcraft III, gates are classified as destructibles, which means that they can be destroyed to be opened.​

Method 1: Regions

This method allows gates to open once units enter a region.


First, create a gate wherever you'd like. To do this, open the Doodad Palette, and go to Trees/Destructibles.


Next, switch to the Region Palette, and create a new region. Make the region decently big, so that it covers both sides of the gate by a decent margin. Name the region "Open Gate", as this will be the region in which, upon a unit entering, the gate will open. This is how it should look like:


Once the unit leaves the region, the door will close. But to do this, you must incorporate triggers. Open the Trigger Editor (F4) and create a trigger. The goal of the first trigger is to open the gate upon entering the Open Gate region, so let us do that.

Open The Gate
Events
Unit - A unit enters Open Gate <gen>

Conditions

Actions

That will register when a unit enters the region, so now we must open the gate. To do this, you must go to Actions -> Destructible -> Open/Close/Destroy Gate. Select the gate you want to open, and this should be the resulting trigger:

Open The Gate
Events
Unit - A unit enters Open Gate <gen>

Conditions

Actions
Destructible - Open Gate (Horizontal) 0045 <gen>

Now we must do the same, but instead close it upon leaving. You will use the same functions, but just change the event and change "Open" to "Close".

Close The Gate
Events
Unit - A unit leaves Open Gate <gen>

Conditions

Actions
Destructible - Close Gate (Horizontal) 0045 <gen>

However, there is one problem. Once you leave the gate, it will close on units that are still passing through. It is pretty funny, but most people probably wouldn't want that in their maps. To fix this, you must first create a Unit Group variable named GateUnits. Then, create the following trigger.

Close The Gate
Events
Unit - A unit leaves Open Gate <gen>

Conditions

Actions
Set GateUnits = (Units in Open Gate <gen>)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Number of units in GateUnits) Equal to 0

Then - Actions
Destructible - Close Gate (Horizontal) 0045 <gen>

Else - Actions

Custom script: call DestroyGroup(udg_GateUnits)

This will make sure that there is no one in the open gate region before closing it. There is also another problem that rises, in which if you have guards standing at the gate, it will never close. To fix this, basically add conditions to the grouping or in the If - Conditions to make sure it doesn't include anyone you don't want it to.

​

Method II: Chat Messages

This method of opening gates involves chat messages to queue opening and closing of gates. If you type -Open, the gate will open. If you type -Close, the gate will close.


For this, perform the same actions in Method I, except ignore making the regions. Simply make the gate, and then open the trigger editor.


For registering chat messages, we must use the event Player - Chat Message. Add this event for all the players. Then we must simply open the gates. You can either make a trigger for each gate, (eg: -Open1, -Open2), but you can also just use one trigger and open all gates upon the -open chat message.

Open The Gate
Events
Player - Player 1 (Red) types a chat message containing -open as An exact match

Player - Player 2 (Blue) types a chat message containing -open as An exact match

Player - Player 3 (Teal) types a chat message containing -open as An exact match

Player - Player 4 (Purple) types a chat message containing -open as An exact match

Player - Player 5 (Yellow) types a chat message containing -open as An exact match

Player - Player 6 (Orange) types a chat message containing -open as An exact match

Player - Player 7 (Green) types a chat message containing -open as An exact match

Player - Player 8 (Pink) types a chat message containing -open as An exact match

Player - Player 9 (Gray) types a chat message containing -open as An exact match

Player - Player 10 (Light Blue) types a chat message containing -open as An exact match

Player - Player 11 (Dark Green) types a chat message containing -open as An exact match

Player - Player 12 (Brown) types a chat message containing -open as An exact match

Conditions

Actions
Destructible - Open Gate (Horizontal) 0045 <gen>

-------- You can add other gates to open here as well --------

If you don't have all players playing the game, then just remove the events that don't correspond to users in the map. Now you must do the same thing for closing.

Close The Gate
Events
Player - Player 1 (Red) types a chat message containing -close as An exact match

Player - Player 2 (Blue) types a chat message containing -close as An exact match

Player - Player 3 (Teal) types a chat message containing -close as An exact match

Player - Player 4 (Purple) types a chat message containing -close as An exact match

Player - Player 5 (Yellow) types a chat message containing -close as An exact match

Player - Player 6 (Orange) types a chat message containing -close as An exact match

Player - Player 7 (Green) types a chat message containing -close as An exact match

Player - Player 8 (Pink) types a chat message containing -close as An exact match

Player - Player 9 (Gray) types a chat message containing -close as An exact match

Player - Player 10 (Light Blue) types a chat message containing -close as An exact match

Player - Player 11 (Dark Green) types a chat message containing -close as An exact match

Player - Player 12 (Brown) types a chat message containing -close as An exact match

Conditions

Actions
Destructible - Close Gate (Horizontal) 0045 <gen>

-------- You can add other gates to close here as well --------

Method III: Unit Range (Medium)

This method is a bit more advanced. It doesn't require regions, but it effectively does the same thing. It will check if a unit enters the range of the gate, and it will open it if so. It will close the gate if not. In this method, we will also discuss dead gates, as well as multiple gates in the game.


So first, let's take into consideration that you have 3 gates. (or more) If you have a situation like this, you will probably want to use an array to easily sort your gates. So first, create a destructible array called Gate. you will want an initialization trigger to set the gate variables to their corresponding destructibles:

Gate Initialization
Events
Map initialization

Conditions

Actions
Set Gate[1] = Gate (Horizontal) 0100 <gen>

Set Gate[2] = Gate (Horizontal) 0045 <gen>

Set Gate[3] = Gate (Horizontal) 0099 <gen>

Now we will want to fix whether or not a gate is destroyed. When a gate destroys, it will fire the event: Destructible - Destructible Dies. So this is exactly what we'll be using. Make a new trigger, and name it whatever you'd like. We will loop through the gate indexes and register a death event on initialization for everyone one of them onto a that new trigger you have made:

Gate Initialization
Events
Map initialization

Conditions

Actions
Set Gate[1] = Gate (Horizontal) 0100 <gen>

Set Gate[2] = Gate (Horizontal) 0045 <gen>

Set Gate[3] = Gate (Horizontal) 0099 <gen>

For each (Integer A) from 1 to 3, do (Actions)
Loop - Actions
Trigger - Add to Gate Destroyed <gen> the event (Destructible - Gate[(Integer A)] dies)

Gate Destroyed
Events

Conditions

Actions
For each (Integer A) from 1 to 3, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Dying destructible) Equal to Gate[(Integer A)]

Then - Actions
Set Gate[(Integer A)] = No destructible

Else - Actions

These triggers will need some explanation. What we are doing is making sure that actions for the gates won't be performed any more once the gate is dead. We register for each index of gate (1 to 3) a death event in the initialization trigger. If you use more or less than 3 gates, such as 6 gates, then you would change the loop to "1 to 6". In the "gate destroyed" trigger, you will check which gate was destroyed. For example, if Gate (Horizontal) 0045 <gen> was destroyed, then the trigger would figure out that Gate[2] is equal to that gate. Once we figure out which variable is set to that gate, then we will basically make that variable point to nothing. The variable will point to "no destructible" (null), so now opening and closing that variable will no longer close that gate.


This doesn't make sense at the moment, but it will once we make the opening/closing trigger.


To open/close the gate, we will repeatedly check if there are any units in range of the gate. If there are some, then we will open the gate. If there aren't any, then we will close it. Create a Unit Group variable named GateUnits, create a Point variable named GatePoint, and a real array named GateLife.


This is what the trigger will look like:

Gate OpenClose
Events
Time - Every 0.40 seconds of game time

Conditions

Actions
For each (Integer A) from 1 to 3, do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Gate[(Integer A)] Not equal to No destructible

Then - Actions
Set GatePoint = (Position of Gate[(Integer A)])

Set GateUnits = (Units within 350.00 of GatePoint matching ((((Matching unit) is A structure) Equal to False) and (((Matching unit) is alive) Equal to True)))

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Number of units in GateUnits) Greater than 0

Then - Actions
Set GateLife[(Integer A)] = (Current life of Gate[(Integer A)])

Trigger - Turn off Gate Destroyed <gen>

Destructible - Open Gate[(Integer A)]

Trigger - Turn on Gate Destroyed <gen>

Else - Actions
Destructible - Close Gate[(Integer A)]

Destructible - Set life of Gate[(Integer A)] to GateLife[(Integer A)]

Custom script: call RemoveLocation(udg_GatePoint)

Custom script: call DestroyGroup(udg_GateUnits)

Else - Actions

There are several things worth noting in this trigger, so I'll break them down. First off, the event is meant to fire every 0.40 seconds to periodically check if there are any units in range. If there are, it will open the gate, and otherwise, close it.


The loop will loop through all the gates in the map that you have set in the initialization. If you have 5, set it to "1 to 5", or if you have 123, then set it to "1 to 123". The condition immediately afterward corresponds to the gate destroying trigger. It checks if the variable has no value, and if it does have a value, it won't perform the following functions.


The next part groups the units in range of the gate. If there are 1 or more units in the area, it will open the gate. If there are 0 units in the area, it will close the gate. However, you might have noticed some key things:

Trigger - Turn off Gate Destroyed <gen>

Destructible - Open Gate[(Integer A)]

Trigger - Turn on Gate Destroyed <gen>
Why do you need to turn off the trigger? Well, it is because when you open the gate, it fires the "death" event. Thus, we must disable it and turn it back on so that it won't count as us "destroying" the gate.


The next part is this:

Destructible - Close Gate[(Integer A)]
Destructible - Set life of Gate[(Integer A)] to GateLife[(Integer A)]

When you open the gate, the gate no longer has any life. Once you close it, it is back at max life. To prevent this, we set the life of the gate before we open it, and set the life back once we close it. The custom scripts at the end are just for cleaning leaks.


Now you should be done with this method. Make sure you have all 3 of the triggers mentioned throughout this method.​

Final Notes:

You are done! Now you can open and close gates in as many different ways as you want! Enjoy!

​


 Last edited by a moderator: Jan 13, 2011

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Not bad, just needs some grammar fixing and it should be good to go. Also, some organization would be nice. (headers)

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Please explain how we can use variables in triggers above and make them MPI...

Also you need to organize your tutorial (edit wall of text)

#### RazorHedgeFan

#### RazorHedgeFan

 Level 3

JoinedJan 3, 2011Messages14

Resources: 2

#### Resources
Maps 1Tutorials 1

Il update as soon as possible


EDIT: is it nice? Reply if i did something wrong. And it's acceptable to put that sign with red "THIS IS NOT RECOMMANDED FOR EASY USERS"?


 Last edited: Jan 5, 2011

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

Some BB Codes are used wrong and they need to be improved/fixed!

Example:

 H2]Way II: Typing in game E.G:-Open [ and gate open] (Verry easy difficult)[/H2]

so first we want to spawn the gate.Now you spawned gate


Click to expand...

I guess you missed [

## Way II: Typing in game E.G:-Open [ and gate open] (Verry easy difficult)

so first we want to spawn the gate.Now you spawned gate

#### RazorHedgeFan

#### RazorHedgeFan

 Level 3

JoinedJan 3, 2011Messages14

Resources: 2

#### Resources
Maps 1Tutorials 1

fixed, Thanks for saying

#### jordon

#### jordon

 Level 6

JoinedJan 26, 2009Messages98

Resources: 8

#### Resources
Maps 2Tutorials 6

It should look like this, except for the missing triggers and variables and random stuff hidden

 Hello everybody,

in this tutorial I will show you how to open or close a gate in WE in 3 ways

--Way I: Regions (Easy Difficult)

First we need gate. So spawn a gate where you want.

Now, create a Region where you want, named: Open Region (Make another one on the other side of door named Open Door2)

(Optional, but recommended): Create a footswitch and place region on it

Now create another Region near "Open Region" Named: Close Region (make another one on the other side of door named Close Door2)

We have the Regions So what's the point. When player steps on region the door will either open or close.

Now open Trigger Editor (F4 hotkey).

Create a new Category

after you created the category create 2 trigger by pressing new trigger and one should be named: Open Gate and the other Close gate.

At open gate, you gonna add this

Trigger

So that’s it we have it

now we will make a trigger for close gate.

Go to Close gate trigger and add this

Trigger

F.A.Q

A: At close/open gate trigger why this: Unit- A unit enters Close gate or open?

Q: Remember when we placed the region? Well when a player enters that region something will happen to the player.

Q: ok, I got it but what it will happen?

A: Well I added at actions: Close gate or open gate.

Q: And what’s the matter with condition?

A: Well let’s say a unit enters region that isn't owned by you. The condition we did add its false then trigger will not apply actions. Will do nothing

Way II: Typing in game E.G:-Open [and gate open] (Very easy difficult)

so first we want to spawn the gate. Now you spawned gate

Go to Trigger Editor (f4 Hotkey), and make 2 triggers and name the first ‘Open door’ and the second ‘Close door’.

Now open "Open door trigger”. We need the following in it.

Trigger

Ok now at Close gate

Trigger

F.A.Q

Q: Well...

A. I think you will find out alone. Don't you think it is so easy? Even my cat knows what to do

WayIII Unit Range (Medium)

THIS IS NOT RECOMMANDED FOR EASY USERS

First Create 2 Gates

then we want The Variables

Variables

Note: Destructible-Array (2) Because we have 2 gates

Ok so now we have the variables. Let's get to Triggers

Create a Trigger called however you want. I'll call it "Gate Initialization".

This is the Trigger you should use

Gate Initialization

We've done with first trigger.

Now let's start with 2nd one. I'll call it Gate Dead

Gate Dead

The 2nd Trigger is done, now the final one. I’ll call it open/close gate.

Open/close gate

Open/close gate

F.A.Q

Q. What does Gate initialization does?

A. It setups our trigger. Remember when we did Gate Variable?

Q. What does Gate Dead do?

A. Well let's say someone destroy the gate. This trigger allows us to make the gate stay dead.

Q. What does Open/Close Door do?

A. Seriously? I will tell you information, not such a question. So Open/close door will open when you're entering the 350 range of each door. (Remember? We've created 2 doors) .But it will not open if the unit is dead. Or if a unit dies near door when the door is actually open, it will close.

That's it have fun!

Click to expand...

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Great job.  I reorganized the tutorial and fixed some grammar mistakes. I just did it to follow the hive's tutorial standards. Also, kudos on the last trigger, I never knew that opening fired the death event and that closing restored life. It was well constructed.


~Approved.

#### ironman47

#### ironman47

 Level 8

JoinedDec 12, 2010Messages280

Resources: 4

#### Resources
Maps 4

What no lever method


I once showed someone how to use a single lever to open and close a gate.


Simple yes, but for some they got no idea. (we were all noobs once)


I like the range method. Never thought of that one.


But I would simplify the triggers, like in the region method. You used a variable and really didn't need to. Instead of using a variable in the last trigger your condition could be:


 Number of units in region open gate equal to 0


Just easier thats all

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

ironman47 said:

 What no lever method


I once showed someone how to use a single lever to open and close a gate.


Simple yes, but for some they got no idea. (we were all noobs once)


I like the range method. Never thought of that one.


But I would simplify the triggers, like in the region method. You used a variable and really didn't need to. Instead of using a variable in the last trigger your condition could be:


 Number of units in region open gate equal to 0


Just easier thats all

Click to expand...


Sadly, (Units in (Region)) leaks a unit group variable. That is why you have to use a unit group, to prevent a leak from forming. ;D

#### ironman47

#### ironman47

 Level 8

JoinedDec 12, 2010Messages280

Resources: 4

#### Resources
Maps 4

PurgeandFire111 said:

 Sadly, (Units in (Region)) leaks a unit group variable. That is why you have to use a unit group, to prevent a leak from forming. ;D


Click to expand...

So the game creates a variable for you when you use (units in region) and never cleans it. That's just poor programing on blizz's part. Maybe they didn't see it as a problem. After all I've never experienced a problem from this. Now I'm wondering what else leaks. I seen a post about leaks somewhere.


Thanks for the info

#### Orcnet

#### Orcnet

 Level 30

JoinedJul 31, 2010Messages5,248

Resources: 5

#### Resources
Maps 4Tutorials 1

this is good for Defenses or LOAP maps, anyways I like your tutorial thanks for the help!

#### Mr.Bear

#### Mr.Bear

 Level 4

JoinedOct 9, 2013Messages83

how do you open a gate using a key?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

necromancer lol

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Mr.Bear said:

 how do you open a gate using a key?


Click to expand...


It is the same method as "Regions". Just add a condition to check if the unit has the key item. (Triggering unit) has <Item> Equal to true (something like that).

#### Mr.Bear

#### Mr.Bear

 Level 4

JoinedOct 9, 2013Messages83

sorry but i couldn't understand

so i create the "unit enters region"

then i go to condition and i select "trigger comparision" right?

but then there is the "value" equal to "value"

how do i do this?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Mr.Bear said:

 sorry but i couldn't understand


so i create the "unit enters region"


then i go to condition and i select "trigger comparision" right?


but then there is the "value" equal to "value"


how do i do this?


Click to expand...


It should be a boolean comparison. And then you just change the left side to the condition you want (e.g. <Unit> has some item). The middle and the right side can stay the same (Equals to True).


I don't have access to the editor for a while, but hopefully that'll steer you towards the right direction. If not, you can feel free to make a thread in the Triggers & Scripts forum and I'm sure someone will help you out.

#### Valandil

#### Valandil

 Level 9

JoinedSep 14, 2012Messages237

Resources: 2

#### Resources
Maps 1Templates 1

Here i made with help of Marshmallo the simple gate system where if the gate is destroyed it will remain destroyed ...

#### Attachments

gates.w3x

 17.3 KB

 · Views: 318



#### Nemesis_

#### Nemesis_

 Level 5

JoinedJun 13, 2015Messages129

How do I make it so that when two(if not more) units die, the gate opens? Been fumbling with triggers awhile now but cant find anything.

#### LordDz

#### LordDz

 Level 25

JoinedMay 11, 2007Messages4,643

Resources: 8

#### Resources
Maps 8

Events:

Unit1 dies

Unit2 dies

Conditions:

unit1 == dead

unit 2 == dead

Actions:

destructible - kill zeh gate of doom and ride away in a glory of the heroism you did saving the peagulls that were trapped behind zeh gate of zeh doom.

#### Nemesis_

#### Nemesis_

 Level 5

JoinedJun 13, 2015Messages129

Oh I see, thought it was more complicated. Thank you!

#### Nemesis_

#### Nemesis_

 Level 5

JoinedJun 13, 2015Messages129

what is the unit=dead condition under? never use triggers so much

#### Crystalknoct

#### Crystalknoct

 Level 7

JoinedAug 15, 2012Messages318

Resources: 2

#### Resources
Maps 1Spells 1

Nemesis_ said:

 what is the unit=dead condition under? never use triggers so much


Click to expand...

Boolean comparison

#### Nemesis_

#### Nemesis_

 Level 5

JoinedJun 13, 2015Messages129

Thanks a lot!

#### zero_hero

#### zero_hero

 Level 1

JoinedNov 15, 2015Messages5

Hi


I was able to follow the part of the tutorial and I have created a gate which open and close when one unit is stepping on the trigger (region).


However, I simply cannot figure out how to make the next part of the tutorial where I make the stay open until all units have gotten through.


My problem is that I can't figure out what to select in the Action Type ?


I have tried to select Unit Group, however, I'm not sure what to do next.


How and where do I create a Unit Group variable named GateUnits ?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Press the yellow X button to create variables.Or use the keybind for it. You can find the keybind in your world editor settings. (or just google it)

#### Drankore

#### Drankore

 Level 1

JoinedMar 18, 2016Messages3

How to create this string (Number of units in GateUnits) Greater than 0


EDIT1: Thanks I found already


EDIT2: In method III when I look at gate using camera it automatically dies.


How to fix that?


 Last edited by a moderator: Mar 19, 2016

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Drankore said:

 In method III when I look at gate using camera it automatically dies.


How to fix that?


Click to expand...


It shouldn't automatically die just by looking at the gate with the camera since the method doesn't really involve the camera. Are you sure a unit didn't pass by closely?


I recommend taking a look at this line:

Set GateUnits = (Units within 350.00 of GatePoint matching ((((Matching unit) is A structure) Equal to False) and (((Matching unit) is alive) Equal to True)))
Filter it as necessary so that it only opens when you want to. For example, if you have some neutral hostile enemy near it, you'll probably want to add a condition "(Owner of (Matching unit) Not equal to Neutral Hostile)", otherwise it'll just open right away.

#### Drankore

#### Drankore

 Level 1

JoinedMar 18, 2016Messages3

The error is in this line

 Destructible - Set life of Gate[(Integer A)] to GateLife[(Integer A)]

I think it is taking default value of GateLife that's why it is dying

#### Drankore

#### Drankore

 Level 1

JoinedMar 18, 2016Messages3

I changed the default value so the gate didn't die but when i reach the gate it open and when i go away from the gate it simply dies

 You must log in or register to reply here.


### Similar threads

Locked

 Gate Close Trigger Not Working? Plz Help


Gnuoy

Sep 12, 2021

World Editor Help Zone

Replies6Views1K

Sep 14, 2021
Gnuoy

 How to Make Buildable Gates


Mashintao

Apr 21, 2008

Trigger (GUI) Editor Tutorials
2

Replies56Views45K

Aug 10, 2020
TheFireMage

Locked

 Closed vertical iron gate letting units trough...


Retria

Feb 23, 2016

World Editor Help Zone

Replies1Views1K

Feb 23, 2016
A]mun

Locked

 Gates open/close issue


ZiGOR

Mar 30, 2019

World Editor Help Zone

Replies5Views2K

Apr 1, 2019
Rahko

 Make an Openable/Closable Gate


Silk060

Nov 18, 2008

Tutorial Archive

Replies9Views2K

Jan 3, 2009
Pyritie

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