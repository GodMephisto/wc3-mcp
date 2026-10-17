---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/sharpcraft-first-person-camera.255084/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Sharpcraft First Person Camera
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/sharpcraft-first-person-camera.255084/>
#### oger-lord

#### oger-lord

 Level 8

JoinedJul 24, 2006Messages157

Resources: 6

#### Resources
Models 2Maps 2Ported Models 1Tutorials 1

#
 SharpCraft First Person Camera


This tutorial is about a simple first person camera system made with sharpcraft.


Keep in mind:


-You need SharpCraft to play the map


-Wc3 Engine is not made for such a camera


Screenshot how this can look like:

##
 Idea:

We place the cursor in the center of the screen.


Now we constantly check whether the cursor position changed and reset the position to the center.


We can use the distance of the cursor to the center to detect the speed of the motion.


With this information we are able to determine the new camera rotation.

##
 SharpCraft

I attached the correct SharpCraft version 1.2.4.0 with an additional script to set and get the mouse cursor position: download


There is a newer one, but this is the last stable version. This means you do not need to code any c# and you can directly manipulate the cursor with Jass/Wurst.

Add Sharpcraft to the Editor


If you use WurstScript simply add the SharpCraft folder to your Wurstpack folder and a new menu item will pop up in the editor.


//TODO add description for JNGP


Known Issues: You need multiple tries until SharpCraft successfully injects into wc3 when you start the map via editor.

##
 Code:

The following code segments will be Wurst Code, but it works similar in Jass.


You need to add the new native methods with

 JASS:

public native CenterMouse()
public native GetCenterScreenX() returns real
public native GetCenterScreenY() returns real
public native GetMouseScreenX() returns real
public native GetMouseScreenY() returns real


Here is an possible initialization of the camera:

 JASS:

SetCameraFieldForPlayer( Player(0), CAMERA_FIELD_FIELD_OF_VIEW, 115,0 )
SetCameraFieldForPlayer( Player(0), CAMERA_FIELD_FARZ, 5000,0 )
SetCameraFieldForPlayer( Player(0), CAMERA_FIELD_ANGLE_OF_ATTACK, 0,0 )
SetCameraFieldForPlayer( Player(0), CAMERA_FIELD_ZOFFSET,height, 0 )
SetCameraFieldForPlayer( Player(0), CAMERA_FIELD_TARGET_DISTANCE, 0, 0 )


Now you can simply use new defined natives to manipluate the camera, here is an example with the y axis:


(It works similar with the camera rotation and the x-axis)

 JASS:

//Check if the position of the cursor changed
if GetMouseScreenY() != GetCenterScreenY()
 //Calculate the new angle of attack
 angleOfAttack+= (GetCenterScreenY()-GetMouseScreenY())/4

 // Limit the view so you cannot rotate over your head
 if angleOfAttack >90
 angleOfAttack = 90
 else if angleOfAttack < -90
 angleOfAttack = -90
 //Adjust the new angle of attack
 SetCameraFieldForPlayer( Player(0), CAMERA_FIELD_ANGLE_OF_ATTACK, angleOfAttack, cameraTime )

//Reset the cursor back to the center
CenterMouse()

In wc3 you cannot set the distance to the target to zero, the lowest number possible is 100.


This means if you only change the angle of attack you get this effect:


To solve this effect you need to adjust the position of the camera.


You can do this with cosinus and sinus:

 JASS:

//Turn the rotation angle into a direction vector
angle a = rotation.asAngleDegrees()
vec2 direction = a.direction()

//Use the vector to calculate the correct new cam position
real newX=cameraPosition.x+100*CosBJ(angleOfAttack)*direction.x
real newY=cameraPosition.y+100*CosBJ(angleOfAttack)*direction.y

PanCameraToTimed( newX,newY,cameraTime )

//Do the same with the height of the cam
location loc = Location(GetCameraEyePositionX(),GetCameraEyePositionY())
//Dont forget to get the ground height
real newHeight = GetLocationZ(loc)
setCameraZ(newHeight+height+100*SinBJ(angleOfAttack))


Besides wc3 has the effect to automatically adjust the height of the camera to the ground.


Here is a work around/hack to gain full control over the height of the camera:

 JASS:

function setCameraZ(real z)
 real zz = GetCameraField(CAMERA_FIELD_ZOFFSET)+z-GetCameraTargetPositionZ()
 SetCameraField(CAMERA_FIELD_ZOFFSET,zz,- cameraTime)
 SetCameraField(CAMERA_FIELD_ZOFFSET,zz,cameraTime)

##
 Important: Display Bugs and how to fix them

This is the most important part of the tutorial because you will notice very fast that some models will simply disappear in game.


In this section I will explain why this is happening and what you can do against it.

###
 Principle:

Warcraft 3 checks the terrain to detect whether a model is displayed or not.


As a result models disappear when you look at the sky and no terrain near the model is left on the screen.


This is extremely annoying when you have models larger than the original wc3 trees.

###
 Doodads

1. Best Solution:


Fill doodads with terrain. For example when you have a house you can place terrain into the building.


As a result every time you look at the top of the building there will be terrain on the screen -> the building is still visible.


For some Doodads the invisible tile could be useful to place terrain with the invisible tile at the position.


Remember: Most important is that the height of the terrain is nearly as hight as the doodad.


To achieve this you can use the miscdata.txt.


Example:


2. Sometimes this is not possible, for example for large trees. In this case you can edit the model and multiply the MinimumExetent and MaximumExtent with a value like 10.


This means the cuboid that wc3 checks whether the model should be visible or not is much larger. As a result the possibility that the models is displayed correct is increased.


This method fix the issue, but it is bad for the performance. Because now the model is rendered even when it is not on the screen.


Besides it is no longer possible to easily select the model in the editor.


You can use Magos Model Editor: Edit -> Model Properties

###
 Units

For Unit Models you can increase the Bounds Radius.


Open Magos -> Edit -> Model Properties

###
 Performance:

The Performance is not good because Warcraft 3 is rendering all models, even those are hidden behind another object:

###
 Tips to increase the Performance:

Use less doodads! (and try to keep the polygon count low)


A useful technique I created is to merge doodads in an area to a single model.


This means houses build out of single doodads are bad, but a single large house/rock formation is good.


To achive this I created the Doodad2Model Merger.

#### Attachments

SharpCraft.rar

 261.5 KB

 · Views: 611



Cam_Distance.png

 36 KB

 · Views: 1,608



Performance.png

 36.8 KB

 · Views: 1,574



terrain.png

 37.5 KB

 · Views: 1,580



magos1.png

 22.1 KB

 · Views: 1,593



WurstSC.png

 2.5 KB

 · Views: 1,526



Example_Screenshot_medium.jpg

 276.3 KB

 · Views: 2,111




 Last edited by a moderator: Jun 11, 2015

#### oger-lord

#### oger-lord

 Level 8

JoinedJul 24, 2006Messages157

Resources: 6

#### Resources
Models 2Maps 2Ported Models 1Tutorials 1

I made a test map!

You move with WASD and rotate the camera with the mouse.

The attached rar contains the w3x and the .wurst scripts used.

If you want to play the map:

-Add the map to your Warcraft 3 Map folder

-Download SharpCraft from the post above

-Make sure you use only 1 Monitor

-Start Wc3 via the batch file "Sharpcraft.Game.exe -window.bat"

If you want to edit the map:

-You need Wurst

-Create a project with the attached map and script files

I would like to hear some feedback.

#### Attachments

FPSCamera.rar

 37.7 KB

 · Views: 402




 Last edited: Jul 25, 2014

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Not worth it. Not in my opinion anyway. You need both wurst and sharpcraft, just a giant nope.


 Last edited: Jan 9, 2022

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Chaosy said:

 Not worth it. Not in my opinion anyway. You need both wurst and shapecraft, just a giant nope.


Click to expand...


You only need Wurst to edit the map. You can run the map as long as you have SharpCraft.


----


@oger_lord: Good tutorial. A video would be a great addition, if possible. I'll have to review the code/technique a little closer, but it seems like it should be approvable (I'll probably make some grammar fixes as well to make it more coherent). Once I get my PC back, I'll test it and give a proper review.


For now, it might be interesting to consider hooking natives. SharpCraft allows you to hook on normal natives, and perform particular actions when a specific input is given. For example, MindWorX's SharpCraft example hooks onto the "Cheat" native and does something specific when it is passed a particular command.


Similarly, instead of declaring new natives, you can overwrite hashtable functions and return the coordinates when you're given particular values. Then you can just write function wrappers for it. For example:

 JASS:

function GetCenterScreenX() takes nothing returns real
 call LoadReal(null, -4395043590, 3450843590)
endfunction


Imagine if you hooked onto "LoadReal", and when given the input "null", and keys "-4395043590" and "3450843590" (random numbers), you would return the mouse coordinates. Why is this better than directly declaring new natives? Two reasons:


(1) You can view the map in the lobby (potentially allowing multiplayer).


(2) The map will open even if you don't have SharpCraft. As such, you can detect if the user is using sharpcraft via code: if he is using SharpCraft, give him the FPS camera. If he isn't using SharpCraft, give him a default alternative camera. If the gameplay entirely relies on SharpCraft natives, then all this work isn't that necessary. But that is how I envision SharpCraft being used well.


Sorry, I've gone off on a bit of a tangent. Ideally, I see SharpCraft as being used to grant additional features, but I think the map should still be playable in regular Wc3 (if it wants to be successful). It is really just food for thought. You don't have to adjust your tutorial for it, but it might be interesting to play around with (I personally haven't messed around with that idea yet).

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

Excellent tutorial, indeed a very useful one. I highly recommend this for those who want to archive something larger than a simple choppy regular camera. +rep


 Last edited: Jul 30, 2014

#### oger-lord

#### oger-lord

 Level 8

JoinedJul 24, 2006Messages157

Resources: 6

#### Resources
Models 2Maps 2Ported Models 1Tutorials 1

 For now, it might be interesting to consider hooking natives. SharpCraft allows you to hook on normal natives, and perform particular actions when a specific input is given. For example, MindWorX's SharpCraft example hooks onto the "Cheat" native and does something specific when it is passed a particular command.


Click to expand...

Hm, I cant find the example. All the SharpCraft stuff ist not organized.

#### muzzel

#### muzzel

 Level 14

JoinedJun 27, 2008Messages1,322

Resources: 2

#### Resources
JASS 1Wurst 1

Chaosy said:

 Not worth it. Not in my opinion anyway. You need both wurst and shapecraft, just a giant nope.


Click to expand...


Stupid comment, this is a tutorial and even points out the downsides of the approach. At no point its recommended to apply this to all maps.


@Oggy: great tutorial and i like the drawings

#### A Void

#### A Void

 Level 26

JoinedMar 29, 2011Messages2,601

Resources: 44

#### Resources
Models 12Icons 8Skins 2Tools 1Spells and Systems 1Ported Models 1Ported Icons 12Tutorials 7

muzzel said:

 Stupid comment, this is a tutorial and even points out the downsides of the approach. At no point its recommended to apply this to all maps.


@Oggy: great tutorial and i like the drawings

Click to expand...


I agree, I also wanted to put out the fact that he wrote a stupid comment, this tutorial is awesome and it deserves to be learned by those who are in-need of such a implementation. We need more tutorials about SharpCraft in general...

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Approved and moved to JASS/AI scripts. I feel like since this deals with scripting, it belongs there.

Great tutorial, and the notes about improving performance are definitely useful (even for those who just resort to a regular first person/third person camera that looks down terrain).

#### BladeMaster3 (TVWK)

#### BladeMaster3 (TVWK)

 Level 5

JoinedApr 13, 2019Messages56

Resources: 4

#### Resources
Maps 4

oger-lord said:

 I made a test map!

You move with WASD and rotate the camera with the mouse.

The attached rar contains the w3x and the .wurst scripts used.

If you want to play the map:

-Add the map to your Warcraft 3 Map folder

-Download SharpCraft from the post above

-Make sure you use only 1 Monitor

-Start Wc3 via the batch file "Sharpcraft.Game.exe -window.bat"

If you want to edit the map:

-You need Wurst

-Create a project with the attached map and script files

I would like to hear some feedback.


Click to expand...

I DOWNLOAD IT AND OPEN IT WITH SHARPCRAFT BUT I CANT RUN

 You must log in or register to reply here.


### Similar threads

Locked

 3rd Person Camera


Berb

Jun 2, 2010

Triggers & Scripts

Replies12Views3K

Jun 2, 2010
Berb

Locked

 Mouse scroll camera help


manstie

May 28, 2010

World Editor Help Zone

Replies2Views2K

May 28, 2010
manstie

 [system]Orbital Camera


iorus

Apr 7, 2012

Graveyard

Replies5Views2K

Oct 2, 2012
Magtheridon96

 Sharpcraft for multiplayer


Kazeon

Mar 11, 2015

Misc/General Mapping Tutorials

Replies9Views4K

Jun 12, 2015
Kazeon

[vJASS]
 UserCamera


Trigger.edge

Sep 30, 2015

Graveyard

Replies24Views6K

Feb 21, 2017
IcemanBo

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top