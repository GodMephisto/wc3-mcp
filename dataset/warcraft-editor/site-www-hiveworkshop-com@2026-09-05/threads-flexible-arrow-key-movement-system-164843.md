---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/flexible-arrow-key-movement-system.164843/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Flexible Arrow Key Movement System
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/flexible-arrow-key-movement-system.164843/>
#### RazorbladeRay

#### RazorbladeRay

 Level 3

JoinedApr 23, 2010Messages11

Resources: 1

#### Resources
Tutorials 1

Hello!


I'm new to this site but I was thinking I would make a tutorial of the system I'm currently working on for Role Playing Games (RPG's).


It's probably one of the few more complicated ones.


The only problem is the attack function. It doesn't work because of the movement. Though, I'm sure a skilled triggerer could bypass that limitation with a little work and effort.


First of all I'm going through the cameras. Then the movement and the last thing will be additions (sounds).

Cameras


Variables needed:


Angle - Real


Animation Speed (For Animations) - Real


Speed (For movement speed of the unit.) - Real


Unit [Or any other name] - Unit


The "Angle" variable is for the angle of the camera.


"Animation Speed" is for the speed of the animation the unit is performing.


"Speed" is of course for the movement speed of the target unit.


"Unit" is of course for the unit of the aim.


This first Trigger called "Initialization" is for the initialization of the camera at the start of the game. It's quite simple really.

Initialization
Events
Map initialization

Conditions

Actions
Set Angle = 0.00

Set AnimationSpeed = 0.00

Set Speed = 0.00

Set Unit = Your selected unit.

Simple wasn't it?


Next part is the "Camera" Trigger.


Variables:


Location [Array; Size 3] - Point Array


Heigth - Real


The "Location" Variable is for the location of the unit in aim.


The "Heigth" Variable is for use in a custom script (I'll get to that later.)


I'm not sure if the heigth trigger is needed now because I'm not the best in custom scripts but I'd suggest making it now.


Here come the trigger. Two Custom Script actions are used in this trigger:

Camera
Events
Time - Every 0.01 seconds of game time

Conditions
CameraSwitch Equal to False

Actions
Set Location[3] = (Position of Unit)

Custom script: set udg_Height=GetLocationZ(udg_Location[3])

Camera - Lock camera target for Player 1 (Red) to Footman 0001 <gen>, offset by (0.00, 0.00) using Default rotation

Camera - Set Player 1 (Red)'s camera Distance to target to 170.00 over 0.10 seconds

Camera - Set Player 1 (Red)'s camera Angle of attack to 345.00 over 0.01 seconds

Camera - Set Player 1 (Red)'s camera Field of view to 1000.00 over 0.01 seconds

Camera - Set Player 1 (Red)'s camera Height Offset to (Height + 100.00) over 0.10 seconds

Camera - Set Player 1 (Red)'s camera Rotation to (Facing of Footman 0001 <gen>) over 0.10 seconds

Custom script: call RemoveLocation(udg_Location[3])

Custom script: set udg_Height=GetLocationZ(udg_Location[3])
Additional Information:

 Distance: This is the distance to the target you want to have a camera attached to. Lower numbers: Smaller Distance.


Angle of Attack: This is the upwards or downwards angle of the camera. If you move the camera to lesser angles then it will go closer to the ground and if you want to move it further up, type a higher number. The highest amount functioning would obviously be 360* as with any other angle.


Height: This is the height of the camera, further up or lower to the ground. This one must be executed based on where the camera shall be positioned as to in the units field of view, higher than the units field of view or lower than the units field of view.


Field Of View: This is a zoom function. If you want to see units closer or further away then this is the setting to change.


Rotation: This changes the angle your camera is constantly redirected to while using the custom camera values. (Facing of Footman 0001) for an example will change the rotation value to the facing of the unit which sight you want to recreate.


Click to expand...


This part is used to set the heigth of the camera. The other actions are found in the "Camera" sections in "Actions".


P.S The Camera Settings such as Angle Of Attack and Distance To Target can be changed to whatever you desire. I use Second Person camera though.

Movement


Now these first parts have been easy but now come the tricky parts. Time for the movement.


Variables needed:


UpTrue - Boolean


DownTrue - Boolean


LeftTrue - Boolean


RightTrue - Boolean

Up Press
Events
Player - Owner of unit:(Unit) Presses the Up Arrow key

Conditions

Actions
Set UpTrue = True

That's easy really. That turns on a variable we'll need for

Up Release
Events
Player - Owner of unit:(Unit) Releases the Up Arrow key

Conditions

Actions
Set UpTrue = False

That turns of the effect which will eventually make the unit stop.


Now we create the exact same thing with all the arrow keys (Boolean Variables we've just created.)


When we're done with that we'll create the animation triggers.

AnimationRun
Events
Time - Every 0.50 seconds of game time

Conditions

Actions
Custom script: call SetUnitAnimationByIndex(udg_Unit, 6)

Custom script: call SetUnitTimeScale(udg_Unit, udg_AnimationSpeed)

Now let's approach this for what it is. It's actually two pretty simple scripts. The first thing is no need to go through.

AnimationRun
Actions
Custom script: call SetUnitAnimationByIndex(udg_Unit, 6)

This script activates animation 6 in the Animation Index for this unit. In my case a footman. Anyhow, the animation index 6 will make the unit run.

AnimationRun
Actions
Custom script: call SetUnitTimeScale(udg_Unit, udg_AnimationSpeed)

This will just use the time scale the animation is using. In this case the speed of the animation being used. Here is where we need the AnimationSpee variable.


I'll fill in all the animation triggers just for the cause:

AnimationStand
AnimationStand

Events
Time - Every 1 Seconds of Game Time

Conditions

Actions
Custom script: call SetUnitAnimationByIndex(udg_Unit, 1)

Custom script: call SetUnitTimeScale(udg_Unit, udg_AnimationSpeed)

Now this one is a tricky one because it will make the units head constantly and instantly jump back and forward in animation order. The only way to solve it is to start a timer (I just tried this myself and it works.) and after that every 10000000 seconds or something restart the animation.


Example:

AnimationStand
Events
Time - Every 10000000.00 seconds of game time

Time - AnimationStandTimer expires

Conditions

Actions
Custom script: call SetUnitAnimationByIndex(udg_Unit, 1)

Custom script: call SetUnitTimeScale(udg_Unit, udg_AnimationSpeed)

A really easy trigger with great animation results.

AnimationAttack
Events
Time - Every 3.00 seconds of game time

Conditions

Actions
Custom script: call SetUnitAnimationByIndex(udg_Unit, 5)

Custom script: call SetUnitTimeScale(udg_Unit, udg_AnimationSpeed)

Trigger - Turn off AnimationRun <gen>

Trigger - Turn off AnimationStand <gen>

This is one that I am not using. "Why not?"


Because it is difficult to match the time scale perfectly. The problem here is that if you stand and keep pushing the hotkey or using a spell manually you will kill anything within seconds. That is not good. That's why this is hard to match with the time scale.


Now comes the last step (almost except for additions such as sounds.); Moving and Turning.


The first one is like this:

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
UpTrue Equal to True

Then - Actions
Set Location[2] = ((Position of Unit) offset by Speed towards Angle degrees)

Unit - Move Unit instantly to Location[2]

Custom script: call RemoveLocation(udg_Location[2])

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Speed Less than 1.00

Then - Actions
Set Speed = (Speed + 0.02)

Set AnimationSpeed = (Speed + 0.02)

Else - Actions

Else - Actions

This probably looks very strange at first but we'll process it piece by piece.

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
UpTrue Equal to True

Then - Actions
Set Location[2] = ((Position of Unit) offset by Speed towards Angle degrees)

Speed will actually also determine if the unit that we're moving around will move forward or be sliding backwards.Next process piece will show what I mean.


Angle is the angle the unit is directing towards.


Next piece:

Unit - Move Unit instantly to Location[2]

Custom script: call RemoveLocation(udg_Location[2])

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
What we're doing here is moving the unit to Location[2] which was offset by Speed towards Angle. Once we're done with that we remove Location[2] By doing this we are allowing further movement.


Next Piece:

If - Conditions
Speed Less than 1.00

Then - Actions
Set Speed = (Speed + 0.02)

Set AnimationSpeed = (Speed + 0.02)

Else - Actions

What we're doing here is adjusting the speed of the unit. This is a cool feature the allows the unit to slowly speed up. This function is not necessary to speed the unit up while time goes but you need this function for the unit to be moving forward.


AnimationSpeed is needed to determine the speed of the animation which in this case is Index 6.


That's forward movement.

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
UpTrue Equal to False

Then - Actions
Set Location[2] = ((Position of Unit) offset by Speed towards Angle degrees)

Unit - Move Unit instantly to Location[2]

Custom script: call RemoveLocation(udg_Location[2])

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
Speed Greater than 0.00

Then - Actions
Set Speed = (Speed - 0.02)

Set AnimationSpeed = (AnimationSpeed - 0.01)

Else - Actions

Else - Actions

This is just the same but the reversed way. In this one you reduce the speed aswell as doing the same thing with the location until the speed reaches 0.


In this next part we'll be moving to the left. What's good about this system is that it will allow you to move in any angle from 0-360 Degrees by ones. Most system go 90 degrees from each step. This one does not. It goes manually in any angle!


Anyway here is left moving:

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
LeftTrue Equal to True

Then - Actions
Set Angle = (Angle + 1.00)

Unit - Make Unit face Angle over 0.10 seconds

Else - Actions

What we're doing here is moving to the left of course. The angle is on a curve as usual from 0 to 360. When we're adding (+) step by step the angle will change and when we move forward of course we will move to the current "Angle".


Right moving is almost the same:

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
RightTrue Equal to True

Then - Actions
Set Angle = (Angle - 1.00)

Unit - Make Unit face Angle over 0.10 seconds

Else - Actions

The only difference here is that we're moving backwards the degrees scale. That means will subtract (-) This allows us to move right which works just the same as left.


For right nor left we will never be using any animations besides the Index 1 (Stand) animation. When moving forward we will be using Index 6 Animation, (Run) and for Down arrow key we'll be using the animation Index 1 (Stand).


I haven't made a trigger for death animation yet but I will soon.

AnimationRun
Events
Time - Every 0.50 seconds of game time

Conditions

Actions
Custom script: call SetUnitAnimationByIndex(udg_Unit, 6)

Custom script: call SetUnitTimeScale(udg_Unit, udg_AnimationSpeed)


Back to these ones. As we move forward we will not just do it in the UpTrue Trigger. We will make it in a separate trigger. The thing with these ones and the reason I'm going back to this subject is that they will be Initially Off at Map Initialization, and later activated once we press up or down key. I haven't covered down key yet but I will get to that soon.


Anyway when we activate the animation triggers we go into the: UpPress trigger:

Up Press
Events
Player - Owner us unit <Unit> Presses the Up Arrow key

Conditions

Actions
Set UpTrue = True

Trigger - Turn on AnimationRun <gen>

Trigger - Turn off AnimationStand <gen>

This is how it should look. Simple isn't it? As you can see here we will turn on the AnimationRun trigger and Turn off AnimationStand.

Down Press
Events
Player - Owner of unit <Unit> Presses the Down Arrow key

Conditions

Actions
Set DownTrue = True

Trigger - Turn off AnimationRun <gen>

Trigger - Turn on AnimationStand <gen>

When we press The Down Arrow key we start this effect. It will turn on the AnimationStand and will stop any forward movement except the turning. I would advice to actually use this on the Up Release trigger aswell. Double effects are better than one even if they're the same. I think I covered it all now except the additions such as sounds.

Sounds


A really nice effect to add for a more reality kind of a feeling is to add movement sounds. I would use the Hero Movement Sounds 1 and 3 but 2,4, and the step <gen> works aswell.

StepSound
Events
Time - Every 2.00 seconds of game time

Time - SoundInstantTimer expires

Conditions
SoundOkay Equal to True

Actions
Sound - Play Step001[1] at 100.00% volume, attached to <Unit>

Wait 0.50 seconds

Sound - Play Step001[3] at 100.00% volume, attached to <Unit>

Wait 0.50 seconds

Sound - Play Step001[3] at 100.00% volume, attached to <Unit>

Wait 0.50 seconds

Sound - Play Step001[1] at 100.00% volume, attached to <Unit>

Wait 0.50 seconds

That's my trigger for it. To activate the trigger you of course place another function in the Up Press trigger.

Up Press
Events
Player - Player 1 (Red) Presses the Up Arrow key

Conditions

Actions
Set UpTrue = True

Set SoundOkay = True

Trigger - Turn on AnimationRun <gen>

Trigger - Turn off AnimationStand <gen>

Trigger - Turn on StepSound <gen>

Countdown Timer - Start SoundInstantTimer as a One-shot timer that will expire in 0.01 seconds

What you first need to do is import the sounds from Sound Editor. Mark the desired sound and right click on it. Once you've done that you press: Use as Sound. That is my tutorial. Any questions about it send me a mail on [email protected] or [email protected] . I'd suggest you send it to [email protected] since I'm rarely on [email protected] .

Thank You


About the attack-problems:


The problems with this function is the only real issue with the system. If anyone is interested in taking a look at my trigger for it then here it is. Sadly it doesn't work and it uses a spell for attack. I was thinking that maybe someone better than me at this kind of triggers would have use for it.

Attack
Events
Unit - A unit Starts the effect of an ability

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Ability being cast) Equal to Attack (Neutral Hostile)

(Owner of (Triggering unit)) Equal to Player001[(Player number of (Owner of (Triggering unit)))]

Then - Actions
Set Creep001[(Player number of (Owner of (Casting unit)))] = (Target unit of ability being cast)

Selection - Select Creep001[(Player number of (Owner of (Casting unit)))] for Player001[(Player number of (Owner of (Casting unit)))]

Unit Group - Pick every unit in (Units currently selected by (Owner of (Casting unit))) and do (Actions)
Loop - Actions
Custom script: call SetUnitAnimationByIndex(udg_Unit, 5)

Custom script: call SetUnitTimeScale(udg_Unit, udg_AnimationSpeed)

Sound - Play MetalHeavySliceFlesh1 <gen> at 100.00% volume, attached to (Target unit of ability being cast)

Unit - Cause (Casting unit) to damage Creep001[(Player number of (Owner of (Casting unit)))], dealing 12.00 damage of attack type Normal and damage type Normal

Wait 0.20 seconds

Unit - Wake up Creep001[(Player number of (Owner of (Casting unit)))]

Else - Actions

Thank you for your time reading and I hope this proved useful to you. If I missed something then send me a complain. Sorry for the different uses of the owner of a unit. It's supposed to be: Owner of Unit. Hope you understand all of this. It's a good camera system. Hope I can help people with alot of new games by spreading this on here. I've also added a file so you can take a look at the system. The attack spell won't work so no use in trying it.


/Rasmus

#### Attachments

Guardian Trigger Set.w3x

 37 KB

 · Views: 1,231




 Last edited: Aug 25, 2010

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

A well-done tutorial. Yeah, there are some problems but at least it is told what the problems are. This is definitely useful though for those who want to know how to make a simple arrow key system involving speed increases and decreases rather than abrupt movement.


~Approved.

#### Spectre4802

#### Spectre4802

 Level 4

JoinedJan 6, 2009Messages100

I have a problem with my map.


For some reason, it throws up an error whenever I try to run it and insists the fault lies with 'AnimationRun' and 'AnimationStand'.


The map's included - I was hoping you could tell me what's wrong with it.

#### Attachments

arrowkeymovementtest.w3x

 13.8 KB

 · Views: 417



#### Shadow_Hunteur

S

#### Shadow_Hunteur

Ho.... as you may imagine, is really difficult to find the fault for us, who are front of our computer !!

I advise you to disable, but not delete, any function and try. Until it work.

You can do it with a right click on the function, and disable function.

When you find which is wrong, you can say this here and so maybe we can help you.

Is the better advice I can give to you.

#### ekman70

#### ekman70

 Level 2

JoinedMay 24, 2010Messages12

how do the custom scripts work it's jus a error on my computer and it disabel itself plzz help me

#### bobojoto

#### bobojoto

 Level 4

JoinedMar 13, 2010Messages12

Resources: 4

#### Resources
Models 2Maps 2

lack of detail

#### defskull

#### defskull

 Level 33

JoinedMar 27, 2008Messages8,035

Resources: 33

#### Resources
Spells and Systems 33

A very nice tutorial !!!

My trigger lack of this technique which is far more advance than mine

This is my Key Movement System

Btw, I am new to GUI and thanks for this tutorial, I'm gonna do better than my old one !

#### RazorbladeRay

#### RazorbladeRay

 Level 3

JoinedApr 23, 2010Messages11

Resources: 1

#### Resources
Tutorials 1

Spectre4802 said:

 I have a problem with my map.


For some reason, it throws up an error whenever I try to run it and insists the fault lies with 'AnimationRun' and 'AnimationStand'.


The map's included - I was hoping you could tell me what's wrong with it.


Click to expand...


Move the unit away from cliffs. Tiny map, tiny clicking area.


Ekman70 stop sobbing and post the file on here.

#### Cihparg

#### Cihparg

 Level 16

JoinedJul 4, 2008Messages1,106

Resources: 9

#### Resources
Models 1Icons 5Maps 2Spells and Systems 1

You've typed udg_AnimationSpeed wrong, it is udg_Animation_Speed.


Change it to AnimationSpeed.

#### RazorbladeRay

#### RazorbladeRay

 Level 3

JoinedApr 23, 2010Messages11

Resources: 1

#### Resources
Tutorials 1

No, it's not because whenever I run his or my own maps, they work fine.


EDIT: You're a genius! It works now =D Thanks for your help. +Reputation for you. The strange thing however is that I used AnimationSpeed on my map and it worked fine.


 Last edited: Aug 26, 2010

#### xorkatoss

#### xorkatoss

 Level 20

JoinedJul 12, 2010Messages1,746

Resources: 10

#### Resources
Models 6Maps 1Spells and Systems 3

dude im too lazy to read all of that..


if you have a map the arrows movement system can you pls upload it here?


becase i already found one arrow movement system but it's optimized and when i copy to my map it gets error and i have no idea how to fix so can you pls upoad it here?


that would be great


EDIT : lol sry i didn't noticed that you posted the map here xD


btw i noticed that the unit cannot attack why?


can you fix that pls?


 Last edited: Sep 30, 2010

 You must log in or register to reply here.


### Similar threads

 Arrow Key Movement / Camera System


OneEight7even

Nov 15, 2007

Trigger (GUI) Editor Tutorials

Replies32Views28K

Apr 10, 2014
Andser

 Hero Selection Systems


Tonks

Mar 23, 2007

Trigger (GUI) Editor Tutorials

Replies12Views18K

Jun 8, 2023
dantles1992

Locked

[Trigger]
 Animation/Movment Help


streetpunk

Oct 16, 2015

Triggers & Scripts

Replies7Views2K

Oct 18, 2015
Nichilus

Locked

[Trigger]
 Arrow key movement


-Berz-

Sep 6, 2010

Triggers & Scripts

Replies29Views5K

Sep 19, 2010
-Berz-

 Gamespeed Keys


Saitek009

Oct 4, 2007

Trigger (GUI) Editor Tutorials

Replies10Views8K

Feb 6, 2008
PurplePoot

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