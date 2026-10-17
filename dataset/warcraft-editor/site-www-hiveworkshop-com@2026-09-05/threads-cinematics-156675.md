---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/cinematics.156675/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Cinematics
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/cinematics.156675/>
#### warcraftIV

W

#### warcraftIV

cinematics

Summary :

Introduction

Initialization

Cameras

Atmosphere and Terrain

Sounds

Transmissions

Precise Timing

Dynamic Cinematic

Additionnal Tips

Examples Cinematics

Introduction :


This Tutorial will cover many of the advanced things about Cinematics, as well as some tips and examples


It's my dirst tutorial, so suggestions and criticism are welcome.

Initialization :


Cinematics don't like that by themselves; they require some triggers to initialize.


Commons cinematics' initialization functions set up the music, atmosphere (fog,sky...) and the cinematic mode.

Initializations
Events
Time - Elapsed game time is 0.00 seconds

Conditions

Actions
Cinematic - Turn cinematic mode On for (All players)

Cinematic - Disable user control for (All players)

Cinematic - Turn on letterbox mode (hide interface) for (All players): fade out over 2.00 seconds

Sound - Clear the music list

Sound - Set Ambient Sounds to 0.00%

Environment - Set sky to Lordaeron Winter Sky

Sound - Stop music Immediately

Custom script: call StopSound(bj_nightAmbientSound, true, true)

Custom script: call StopSound(bj_dayAmbientSound, true, true)

Cinematic - Turn cinematic mode On for (All players
This turn on the cinematic for all players

Cinematic - Turn on letterbox mode (hide interface) for (All players): fade out over 2.00 seconds
This turn the letterbox on for all players, which hides interface and let you "read" the actors' dialogs

Cinematic - Turn subtitle display override On
This is optionnal but forces player to have subtitles turned ON

Cinematic - Turn subtitle display override On
Prevents players from selecting units or controlling them

Cinematic - Fade out over 0.00 seconds using texture Black Mask and color (0.00%, 0.00%, 0.00%) with 0.00% transparency
This fades out with a black mask which is suggested and used by many cinematic makers

Sound - Clear the music list
This clear the music list

Sound - Stop music Immediately
Stop race's music

Environment - Set sky to Lordaeron Summer Sky
This is optionnal but recommended because you can create a better atmosphere.Remember to change the sky depending in your tileset

Cameras :


Cameras are extremely importants when making cinematics as they change the point of view.


To create a camera, open the "camera palette"


You'll see 3 buttons :

Create Camera : Create a camera from the current view

View the Selected Camera : View the camera as you should in-game

Set Camera to Current View : Set the camera to the current view


Just click on "Create Camera" at the moment


Right-click on the camera in the camera list and click on rename camera


You'll see a list of properties to customize a camera :

Camera Name:


This is siply the name of your camera

Target X and Target Y:


These are the coordinates of the cameras

Z Offset:


This measures the Z offset, by raising this values the camera will raise if you set this to a negative value, the camera will be under the ground

Rotation:


This determines the rotation of the camera, it is like the facing. If you set it to 90.00 degrees, the camera will turn and face 90.00 degrees.

Angle of Attack :


This determines how much the camera is off of the ground, though it is not like the Z offset. The Z offset sets the target point to rise/lower. If you change the AoA, it will change the angle of the camera going up and down.

Distance:


This determines the distance from the camera and the target point.

Roll:


This determines the tilt of your camera. If you change it, your camera will tilt sideways in an angle and change the view.

Field of View:


The width the camera can see.

Far Clipping:


How far a camera can see objects

Applying Cameras :


to apply a camera simply use this trigger

Camera - Apply <Camera> for <Player> over <Real> seconds
This will apply the camera for the player and will take X seconds to apply

Atmosphere and Terrain :


Having a nice terrain a good atmosphere is very important to your cinematic.It can impress the viewers a lot, especially with a good camera.


As this is a cinematic tutorial and not a terraining one, I'll just pass the terrain part throught, you can check terraining tutorial to learn more about tiles/doodads variations and fogs.

Transmissions :


Transmissions are used to make units talk and speak, though they do not actually speak unless they use a sound. They actually create subtitles at the bottom of your screen. To set up a transmission, click "Actions" then go to the Cinematics category and click "Transmission From Unit" from the selection bar.

]Send transmission to (All Players) from Unit named Name: Play No Sound and display Message. Modify Duration: Add 0.00 seconds and Wait
Here a small example :

Cinematic - Send transmission to (All players) from Demon Hunter 0001 <gen> named Demon Hunter :: Play IllidanYesAttack2 <gen> and display Die Fool!. Modify duration: Set to 2.5 seconds and Wait
I thinks you understand that this actions show "Die Fooll !" in 2.5 seconds and play sound "IllidanYesAttack2

NOTE : You shouldn't use the "wait" part of this action use Trigger queue instead as seen below

Precise Timing :


Whe "Wait" action waits always longer than requested, no matter if it is a normal or a gametime "Wait". And the worst: the amount of delay always varies, so you often can't just make the waits shorter and everything is fine. This is annoying especially if you want to time animations and effects in action scenes.


There is two solutions to correct this :

First Solution :


Use a periodic event like used in Infrane's and Anitarf's JASS Cinematic System. If you don't want to do nearly everything in JASS, there is another alternative to these damn "Wait" actions: Use "Elapsed gametime equal to x seconds" events. They are exact. But they have obviuosly huge disadvantages:

You cannot use them in playable maps

if you change any timing value at the beginning of the cinematic, you often have to adjust every following trigger which can be very much work

Second Solution :


We can use a trigger queue and a timer to get our actions perfectly timed


The trigger queue is an old features from Roc it's just as the name say, a queue of trigger : the triggers will be executed in the way you added them.


so if you add Trigger1, then Trigger2 and then Trigger3, Trigger1 is the first that will be executed


All queued cinematic triggers do not need any event or condition.


You should have a folder for initialization and general triggers. In this folder, I devide the simple initialization triggers and the general triggers by comments. You can do this as you like, but you should know what is where and be able to find it easily. I have two initialization triggers: One to fire at "Map Initialization" and one to fire at "Elapsed game-time is 0.00 seconds".


Create also a new folder called "Queued Cinematic Triggers".

Scene 01

Events

Conditions

Actions

-------- Setup for next trigger in queue --------

Set RunningTrigger = (This trigger)

Countdown Timer - Start NextScene as a One-shot timer that will expire in 2.00 seconds

-------- Add this scene's triggers --------

-------- This cinematic trigger's actions --------
You can change the 2.00 seconds to the time you'd like the next scene to start


If you understood trigger queue, you'll know that as long the first trigger is in the trigger queue, the others won't fire.So we have to remove the trigger form the queue using this :

Start Next Scene

Events

Time - NextScene expires

Conditions

Actions

Trigger - Remove RunningTrigger from the trigger queue
Dynamic Cinematics :


What make the cinematic really epic is the way the cinematic maker have a high control over the units, moving them, creating special effects, using animations etc...

Additionnal Tips :

Camera Tips :


-When editing Camera Values check the "Preview Values in Main Window". This will now make it so that if you edit a value it will move the camera in the World Editor so you can see what it will look like.


-You can black border to the screen (cinematic-only map recommended) by going to Advanced - Game Interface and changing :

Image: Cinematic Border

Image: Console Background

Image: Game Menu Background
Change the values to :

 UI\Widgets\EscMenu\Human\blank-background.blp


Click to expand...

-Make dynamic cameras, it is always good that the scene is moving, it makes it more dynamic. A scene where the camera is at the same point the entire time is boring and static.


-Don't use too much sway shake camera, this is generally bad. It is good using it in small measures, but in big proportions it will give the viewer headache.


-Using preview camera motion. When you are creating dynamic, moving cameras, you often have to test it which gets annonying. Trigger editor has a good command that can allow you to see how the actual camera movement will be in game.


You will be able too see the movement of this camera or any other in your trigger (preview camera motion button is on the right center of the trigger editor. A trigger must have a camera movement for the button to appear.)


-Gives your camera a name, if you have just lot of Camera X, you'll get lost.


Try giving them a name depending on their use.

Terraining/ambience Tips:


-Try to increase the scaling of trees and buildings (especially in cinematic-only maps) because blizzard default tree have almost the size of a human.


-Remember to change the sky/weather/fog according to the Scene, don't add an outland sky to a cave or ray of light to an icy area


-Create dynamic background to impress the viewer a lot more.For example, in a city where two peoples are talking, try to make lot sounds and peoples moving in the background, this gives the feeling that the city isn't abandonned.


-An often overlooked feature warcraft brings are lights. Tho they seem not highly usable they are in fact quite usable.

 JASS:

call SetDayNightModels("","")

Here a useful link : How to : Create Day an Night Model


in the bottom, I included two models, including a glowing torch and a light source attachable to "chest" of a unit.


-Sound - making good sound atmosphere is by my opinion a 1/4 of a cinematic. The sound is a vital thing of a cinematic. As an example - when a horsmen rides add a ridding loop sound, if it rains add rain loop etc. Good music is the most important thing. You must control the music, fading it out in conversations, increasing the rhytm in the battle scenes, the music must follow the cinematic for the ultimate effect.

Actors and Special Effect :


-Try to use the right animation and maybe ,in battle cinematics, reduce actors animation speed when they're about to clash, it add some epicness


-When you have a only cinematic map, you can make realy cool spells, using bunch of effects, controling unit animations, actualy doing pretty much any spell


-Some battle cinematics makers order a units to move to a region to evade an attack, the problem is that it don't give the feeling of a evasion.


There some good ideas to evade an attack :


1-You could blink the unit (just don't make a blinking grunt)


2-Play "ready" animation (some models only) and use knockback so it look like the unit parried the attack


3-Make the unit jump (using flying height)


-You can play specific animation for unit, yes!


Use this animation native :

 JASS:

native SetUnitAnimationByIndex takes unit whichUnit, integer whichAnimation returns nothing

You should store the unit into a variable, like "Unit", in Jass the variable name will be udg_Unit

 JASS:

call SetUnitAnimationByIndex(udg_Unit, integer)

We still need that integer, which represents animation's ID, to play specific animation, you have to use animation ID


A good way to get the animation ID is to convert the model to .mdl using Magos Model Editor.


After being converted to .mdl, open the mode with Notepad and look for the animation you need.


If the animations you need is the 4th then use :

 JASS:

call SetUnitAnimationByIndex(udg_Unit, 3)

because JASS always start from 0


-It is possible to lock a unit's head OR torso by triggers using :

Animation - Lock (Last created unit)'s Head to face (Last created unit), offset by (0.00, 0.00, 90.00)
If you let 1 unit look at another his head or chest will follow this unit untill you reset the body part facing.

NOTE : It is possible to lock the facing to the unit itself, making only the offset factor matter

Random Tips :


-Use comments so you don't get lost.

Examples Cinematics :

The Spirit of Vengeance

#### Attachments

Cinematics Light.rar

 2.1 KB

 · Views: 112



#### raypack

#### raypack

 Level 31

JoinedDec 3, 2020Messages1,261

Resources: 10

#### Resources
Models 1Maps 8Tutorials 1

Insightful

 You must log in or register to reply here.


### Similar threads

[Mapping]
 Advanced Credit (Cinematic)


Bugz

Jul 18, 2011

Tutorial Archive

Replies8Views2K

Sep 11, 2011
PurgeandFire

 Item Recovery Tutorial [Reviewed: Ralle]


underscore

Mar 27, 2008

Tutorial Archive

Replies13Views3K

Jul 19, 2008
Ralle

 Create simplest 3rd Person Camera


Dieseldodge

Sep 1, 2008

Tutorial Archive

Replies15Views6K

Oct 4, 2008
Dieseldodge

Locked

 WE Hates me - ignores apply camera


Anti_Bodies

Jul 29, 2009

World Editor Help Zone

Replies5Views1K

Jul 30, 2009
Anti_Bodies

Locked

 Cinematic


Razosh

Jun 19, 2013

World Editor Help Zone

Replies6Views2K

Jun 19, 2013
Razosh

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Tutorial Archive

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top