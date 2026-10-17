---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/arrow-key-movement-camera-system.43201/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Arrow Key Movement Camera System
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/arrow-key-movement-camera-system.43201/>
#### OneEight7even

#### OneEight7even

 Level 3

JoinedOct 27, 2007Messages23

Resources: 1

#### Resources
Tutorials 1

this has been done before but i didnt really like any of the others ons so i made one myself.

Arrow Key Movement

1.0


1) Create 3 Variables , named "Up", "Right" , "Left". Make those 3 boolean arrays for the type and make a new variable called Players_Hero with type Unit Array. and another variable named TempPoint and its type point array.


2) Create a map init trigger and set some unit into the Players_Hero variable


3) Create a new trigger named "Up Pressed"


4) add this event to the trigger : Player - Player 1 (Red) Presses the Up Arrow key


5) add the variable up in the actions part and make it look like this


Set Up[(Player number of (Triggering player))] = True


6) Make that for all the arrow keys exept for down.


Example:

]Up Press
Events
Player - Player 1 (Red) Presses the Up Arrow key

Player - Player 2 (Blue) Presses the Up Arrow key

Player - Player 3 (Teal) Presses the Up Arrow key

Player - Player 4 (Purple) Presses the Up Arrow key

Conditions

Actions
Set Up[(Player number of (Triggering player))] = True

Right Press
Events
Player - Player 1 (Red) Presses the Right Arrow key

Player - Player 2 (Blue) Presses the Right Arrow key

Player - Player 3 (Teal) Presses the Right Arrow key

Conditions

Actions
Set Right[(Player number of (Triggering player))] = True

Left Press
Events
Player - Player 1 (Red) Presses the Left Arrow key

Player - Player 2 (Blue) Presses the Left Arrow key

Player - Player 3 (Teal) Presses the Left Arrow key

Player - Player 4 (Purple) Presses the Left Arrow key

Conditions

Actions
Set Left[(Player number of (Triggering player))] = True

1.1


Now we are going to create the "release triggers" you will basicly be doing the same thing as the pressed ones but like this

Up Release
Events
Player - Player 1 (Red) Releases the Up Arrow key

Player - Player 2 (Blue) Releases the Up Arrow key

Player - Player 3 (Teal) Releases the Up Arrow key

Player - Player 4 (Purple) Releases the Up Arrow key

Conditions

Actions
Set Up[(Player number of (Triggering player))] = False

Unit - Order PlayersHero[(Player number of (Triggering player))] to Stop

Now do that for all the arrow keys. exept dont add the Unit - Order Stop action in any of the other ones.

2.0


Create a new trigger named Movement. this trigger will be detecting the variables to order a unit to move. for example :


if the variable Up is = True then it will move that unit up.


1) create the event Time - Every 0.25 seconds of game time


2) add the action Player Group - Pick all players and do actions


 Loop actions


3) in the loop actions part create an If/Then/Else action. so far it should look like this :

Untitled Trigger 001
Events
Time - Every 0.25 seconds of game time

Conditions

Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions

Then - Actions

Else - Actions

in the If - Conditions part of the trigger add these conditions

Up[(Player number of (Picked player))] Equal to True

Left[(Player number of (Picked player))] Equal to False

Right[(Player number of (Picked player))] Equal to False
.


4) after that , create under the Then -Actions part the variable TempPoint like this

Set TempPoint[(Player number of (Picked player))] = (Position of PlayersHero[(Player number of (Picked player))])
 after that action were going to add the moving part of it it should look like this

Unit - Order PlayersHero[(Player number of (Picked player))] to Move To (TempPoint[(Player number of (Picked player))] offset by 90.00 towards (Facing of PlayersHero[(Player number of (Picked player))]) degrees)
Now do the same thing in more loops for all the variables. it should look like this at the end :

Movement
Events
Time - Every 0.25 seconds of game time

Conditions

Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Up[(Player number of (Picked player))] Equal to True

Left[(Player number of (Picked player))] Equal to False

Right[(Player number of (Picked player))] Equal to False

Then - Actions
Set TempPoint[(Player number of (Picked player))] = (Position of PlayersHero[(Player number of (Picked player))])

Unit - Order PlayersHero[(Player number of (Picked player))] to Move To (TempPoint[(Player number of (Picked player))] offset by 90.00 towards (Facing of PlayersHero[(Player number of (Picked player))]) degrees)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Right[(Player number of (Picked player))] Equal to True

Then - Actions
Set TempPoint[(Player number of (Picked player))] = (Position of PlayersHero[(Player number of (Picked player))])

Unit - Order PlayersHero[(Player number of (Picked player))] to Move To (TempPoint[(Player number of (Picked player))] offset by 90.00 towards ((Facing of PlayersHero[(Player number of (Picked player))]) + -90.00) degrees)

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Left[(Player number of (Picked player))] Equal to True

Then - Actions
Set TempPoint[(Player number of (Picked player))] = (Position of PlayersHero[(Player number of (Picked player))])

Unit - Order PlayersHero[(Player number of (Picked player))] to Move To (TempPoint[(Player number of (Picked player))] offset by 90.00 towards ((Facing of PlayersHero[(Player number of (Picked player))]) + 90.00) degrees)

Else - Actions

You're still not done. now for the diaganol movement part. we wont be actually ordering the unit to move diaganol we will move him instantly to point. this is pretty simple so im just going to post the trigger for diaganols.

Stop Turn
Events
Time - Every 0.30 seconds of game time

Conditions

Actions
Player Group - Pick every player in (All players) and do (Actions)
Loop - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Up[(Player number of (Picked player))] Equal to True

Left[(Player number of (Picked player))] Equal to True

Then - Actions
Unit - Make PlayersHero[(Player number of (Picked player))] face ((Facing of PlayersHero[(Player number of (Picked player))]) + 65.00) over 0.10 seconds

Set TempPoint[(Player number of (Picked player))] = ((Position of PlayersHero[(Player number of (Picked player))]) offset by 10.00 towards (Facing of PlayersHero[(Player number of (Picked player))]) degrees)

Unit - Move PlayersHero[(Player number of (Picked player))] instantly to TempPoint[(Player number of (Picked player))]

Animation - Play PlayersHero[(Player number of (Picked player))]'s walk animation

Else - Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Up[(Player number of (Picked player))] Equal to True

Right[(Player number of (Picked player))] Equal to True

Then - Actions
Unit - Make PlayersHero[(Player number of (Picked player))] face ((Facing of PlayersHero[(Player number of (Picked player))]) + -65.00) over 0.10 seconds

Set TempPoint[(Player number of (Picked player))] = ((Position of PlayersHero[(Player number of (Picked player))]) offset by -10.00 towards (Facing of PlayersHero[(Player number of (Picked player))]) degrees)

Unit - Move PlayersHero[(Player number of (Picked player))] instantly to TempPoint[(Player number of (Picked player))]

Animation - Play PlayersHero[(Player number of (Picked player))]'s walk animation

Else - Actions

There you go you made an arrow key movement system. in case you didnt understand it or i didnt explain well take a look at the triggers your self in the demo map (Camera system is in demo map also). im pretty sure i spelled alot of things wrong so dont tell me about it. idk if im good or not at explaining things either soooo

#### Attachments

187 Arrow Key Movement 1.1.w3x

 32 KB

 · Views: 3,306




 Last edited by a moderator: Feb 10, 2008

#### madcat2

#### madcat2

 Level 14

JoinedAug 25, 2005Messages1,105

Resources: 14

#### Resources
Icons 6Skins 6User Interface 1Tutorials 1

Good Tutorial but it'd probably be better with some pictures here and there.

~/\/\@D

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

I suggest replacing the







 Code:

 tags with [trigger] tags, which are specifically built for GUI.

For the spoilers, you could use [plain][hidden=Spoiler](content)[/hidden][/plain], which would produce:

[hidden=Spoiler](content)[/hidden]

Also, Unit - Move Unit (Instantly) resets animations as far as I know, so that would look a little jumpy.

#### SoCal_420

S

#### SoCal_420

nice system i could use it.

#### Kieve

K

#### Kieve

Is [that] how button (hold) functions work? Hm. Need the pair of Press and Release event triggers.


Seems so obvious, and yet it never occurred to me before. Wonderful.


Not too keen on the rest of the code, but that's personal preference - easy to understand at least.

#### Need_O2

#### Need_O2

 Level 19

JoinedAug 24, 2007Messages2,885

would be better with JASS

hey dont look at me like that Im a Lamer

My job is causing annoying situations

Ups including much leaks too

#### CombatTheWombat

#### CombatTheWombat

 Level 14

JoinedJul 30, 2007Messages887

Resources: 12

#### Resources
Models 2Icons 1Skins 6Spells and Systems 1Tutorials 2

hmmmm, dont use jass for anything you can do in gui, as the saying goes, "don't fix what isn't broken"

#### PurplePoot

#### PurplePoot

 Level 40

JoinedDec 14, 2005Messages10,532

Resources: 3

#### Resources
Maps 1Spells 1Tutorials 1

 hmmmm, dont use jass for anything you can do in gui, as the saying goes, "don't fix what isn't broken"


Click to expand...

That's absolutely not true, but on the other hand, some people use GUI too, so GUI tuts are useful too...

#### Wolverabid

#### Wolverabid

 Level 32

JoinedOct 23, 2006Messages5,286

Resources: 5

#### Resources
Tutorials 5

Tutorial Approved!

Map makers should find this tutorial helpful and useful.


~ Thread moved to Trigger (GUI) Editor Tutorials.

#### Deathcom3s

#### Deathcom3s

 Level 15

JoinedDec 24, 2007Messages1,403

Resources: 2

#### Resources
Models 1Icons 1

Nice tut, I'm going to use this in my RPG.

#### ~Void~

#### ~Void~

 Level 36

JoinedJul 1, 2007Messages6,667

Resources: 26

#### Resources
Icons 19Skins 3Tutorials 4

CombatTheWombat said:

 hmmmm, dont use jass for anything you can do in gui, as the saying goes, "don't fix what isn't broken"


Click to expand...


But GUI is leaktastic!

#### underscore

#### underscore

 Level 6

JoinedMar 17, 2008Messages150

Resources: 6

#### Resources
Skins 1Tutorials 5

Thank you for the tutorial. It helped me make my Role-playing a little nice. 5/5

#### Kidd|WhiteHeaven|

#### Kidd|WhiteHeaven|

 Level 8

JoinedNov 4, 2006Messages265

Resources: 6

#### Resources
Maps 2Tutorials 4

Great and usefull. Especially for dodgeball type games.

Note: FPS view cameras really are hard.

#### Pandarian-Warrior

P

#### Pandarian-Warrior

dude...i just learned how to make an arrow-key movement system.


thank you, thank you very much

#### Bill_14

B

#### Bill_14

my opinion is do not try to make such a system


i made one for my rpg map, and the unit is walking awfully, it is like right clicking 4 times a seconds in front of the unit. Also if there is an obstacle in front of the unit, it won't go str8.


it's good however to show to people how to do this, even for practice, good work

#### OneEight7even

#### OneEight7even

 Level 3

JoinedOct 27, 2007Messages23

Resources: 1

#### Resources
Tutorials 1

Lol thanks for all the comments

#### Oziris

#### Oziris

 Level 27

JoinedOct 14, 2006Messages4,161

Resources: 7

#### Resources
Icons 1Skins 3StarCraft II Resources 1Tutorials 2

Can u make W up S down D right and A left ? becous ther is a new tutorial called zoom in and rotating with arrow keys and in most rpgs Movement is on W A S D !


 Last edited: May 10, 2008

#### OneEight7even

#### OneEight7even

 Level 3

JoinedOct 27, 2007Messages23

Resources: 1

#### Resources
Tutorials 1

no


A is for attack


s is for stop

#### Re8eL

#### Re8eL

 Level 7

JoinedDec 11, 2004Messages92

Resources: 5

#### Resources
Skins 3Maps 2

Its a nice way to make a movement system

Its a nice way to make a movement system, but I think iv created one much better for my new Heavy Combat 3 map in production, its similar to Heavy Combat 2's Movement system but a lot smoother and you can strafe.

I may release it in the spell area and as a tutorial after HC3 is complete.

UPDATE:

Oh, and WASD Movement is impossible. (or atleast GOOD WASD movement.)

#### Discohydra

#### Discohydra

 Level 5

JoinedDec 16, 2007Messages160

Does this make the unit move like a tank? Like.....

Up + Right = Turn right and move forward

Up + left = Turn left and move forward

Down + left = turn right while going backwards

Down + right = turn left while going backwards

If it is this is the exact thing i was looking for.

If not, Ill try making my own.

#### Worldedit10

#### Worldedit10

 Level 2

JoinedAug 4, 2008Messages19

Resources: 1

#### Resources
Maps 1

Can u make a video of this tutorial cuz there is something i cant do.


ill show u the picture

http://img140.imageshack.us/img140/470/jkbl0.png


i cant get Unit - Order PlayersHero[(Player number of (Picked player))] to Move To (TempPoint[(Player number of (Picked player))]


its offset by 90.00 towards (Facing of PlayersHero[(Player number of (Picked player))]) degrees) i cant do

#### RiskDude

#### RiskDude

 Level 3

JoinedJul 18, 2008Messages53

is there any map with 3rd camera included i really wanna see how it is cuz im noob at AI  PLEASE!!

#### Worldedit10

#### Worldedit10

 Level 2

JoinedAug 4, 2008Messages19

Resources: 1

#### Resources
Maps 1

RiskDude said:

 is there any map with 3rd camera included i really wanna see how it is cuz im noob at AI  PLEASE!!


Click to expand...


There are many maps with 3rd cam, its easy to fix it but the hard part is to but in triggers so you can play by the keyboard.

YouTube - How to make a 3rd person camera tutorial wc3


there is a tutorial to put third cam to ur map.

#### RiskDude

#### RiskDude

 Level 3

JoinedJul 18, 2008Messages53

10x i saw it yesterday  now its piece of cake for me to make it :X

#### RiskDude

#### RiskDude

 Level 3

JoinedJul 18, 2008Messages53

 Can u make W up S down D right and A left ? becous ther is a new tutorial called zoom in and rotating with arrow keys and in most rpgs Movement is on W A S D !


Click to expand...

 no


A is for attack


s is for stop


Click to expand...


yes you can, you just change the hot keys from the attack and stop with whatever you want and then change the arrow keys with WASD keys. It's a video on youtube that a german made it but it has an error on the triggers.

#### XShooterX

#### XShooterX

 Level 3

JoinedJan 21, 2008Messages25

Resources: 1

#### Resources
Tutorials 1

Nice Tutorial, easily done and usefull for many maps (in my opinion)

#### RedFlash

#### RedFlash

 Level 7

JoinedOct 8, 2008Messages200

How can I make my hero turn faster like if I just tap the key instead of holding it for 1 second

#### xXx-Tricky-xXx

#### xXx-Tricky-xXx

 Level 9

JoinedJun 20, 2004Messages229

Resources: 9

#### Resources
Icons 7Maps 2

RedFlash said:

 How can I make my hero turn faster like if I just tap the key instead of holding it for 1 second


Click to expand...


arrow keys dont really work well with frequent pressing as it takes a little bit longer for it to recognize its being pressed compared to a traditional ability being casted when a hotkey is pressed. But you can simply just add an extra turning mechanism but place it on the triggers with the pressing events. Tapping would probably work without that if the periodic ran at .01 instead but probably is not needed for a third person view. It most definitely needed for a first person view as everything has to be as precise and exact as possible. And anyone who says using cameras that run at .01 is laggy and not needed is an idiot and has no clue what they are talking about. There is a miracle worker out there for maps based on keyboard detection and its called ListChecker. Simply start the map through LAN, then use the Bnet connection command and your hosting at LAN settings but people on Bnet can access it therefor when the map starts the delay is reduced by more than half.


if you are making a first person oriented camera control, then I would recommend doing away completely with the move to point command and use a sliding effect with index animations so you are not actually sliding and looks like a legit movement. WASD isnt impossible, but press and release detection is. Instead you have to improvise and be creative. Use the arrow keys to move forward, backward, and left and right to strafe and you have a very accurate FPS movement. Then use WASD to look left, right, up, and down by tapping them. It can feel very awkward at first but it is definitely the most efficient way without problematic trackables and quickly becomes easy to get the hang of if done correctly.

#### RedFlash

#### RedFlash

 Level 7

JoinedOct 8, 2008Messages200

Hmm would this arrow key system would be okay for like a game your hero is faces numerous number of enemies though? Like armies, gangs of heros etc

#### markusbb

#### markusbb

 Level 5

JoinedMar 14, 2009Messages33

Resources: 7

#### Resources
Maps 7

lollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollollol

#### Andser

#### Andser

 Level 6

JoinedJan 25, 2011Messages63

Resources: 8

#### Resources
Models 1Maps 4Spells and Systems 2Tutorials 1

There's a better way i already saw somewhere (on hive), because you can't play the units walk animation using gui --- you must use CustomScript

call SetUnitAnimationByIndex(udg_UNIT,X)

X = Animation number, not all units have the same number for the walk animation so you will have to find out which one it is.

udg_UNIT = UNIT is a variable (you can call it whatever), it plays the walk animation of the unit which is saved in the variable.

Next:

the Move Unit Instantly is also buggy (of course because it is GUI), use this:

1. Create 2 variables (for example tempX and tempY) type = Point

2. when the trigger runs (the one which moves the unit) save the point where the unit is going to move into the variables (X, Y)

Set tempX = (X of ((Position of UNIT) offset by SPEED towards (Facing of UNIT) degrees))
and

Set tempY = (Y of ((Position of UNIT) offset by SPEED towards (Facing of UNIT) degrees))
in this case SPEED is the amount of points the unit will move (i used acceleration which is a bit more harder to make, it only needs time) but for a normal movement speed use this: Unit_Speed/(1/Periodic time in the event)

its around 12 if the unit has a movement speed of 400 and the trigger runs every 0.03 seconds.

Then, after the position of the unit is saved into variables you have to use 2 custom scripts:

call SetUnitX( udg_UNIT, udg_tempX) ---> write there the name of the variable you used in my case it was tempX

call SetUnitY( udg_UNIT, udg_tempY) with this the same.

RedFlash said:

 How can I make my hero turn faster like if I just tap the key instead of holding it for 1 second


Click to expand...

You can add an action right into the trigger which runs the rotation.

Forward
Events
Player - Player 1 (Red) Presses the Right Arrow key

Conditions

Actions
Unit - Make UNIT face ((Facing of UNIT) - 30) over 0.10 seconds

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

^Forgive him, it's 7 years old.


 Last edited: Apr 10, 2014

#### Andser

#### Andser

 Level 6

JoinedJan 25, 2011Messages63

Resources: 8

#### Resources
Models 1Maps 4Spells and Systems 2Tutorials 1

7 years old, so the moderators can close it, I wrote what was needed to make it better and that's all.

 You must log in or register to reply here.


### Similar threads

Locked

 Problem with Camera Trigger


nome

Sep 5, 2010

Triggers & Scripts

Replies4Views2K

Sep 5, 2010
nome

Locked

[Trigger]
 Problem with a System


cHeRoL

Apr 10, 2009

Triggers & Scripts

Replies2Views1K

Apr 10, 2009
cHeRoL

Locked

 1st Person Shooter Camera and Movement


satient

Sep 23, 2009

World Editor Help Zone

Replies6Views2K

Sep 25, 2009
Tau Empire

Locked

[Trigger]
 Arrow Keys - randomly getting stuck in "pressed" position


Shyster

Oct 7, 2016

Triggers & Scripts

Replies29Views4K

Nov 10, 2016
KILLCIDE

K

 PSN Hero Selection


Kieve

Nov 23, 2007

Trigger (GUI) Editor Tutorials

Replies2Views8K

Nov 26, 2007
Kieve

K

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