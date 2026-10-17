---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/using-3d-points-within-world-editor.287597/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Using 3D Points Within World Editor
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/using-3d-points-within-world-editor.287597/>
#### Aniki

#### Aniki

 Level 13

JoinedNov 7, 2014Messages571

Resources: 21

#### Resources
Tools 1Maps 1Spells and Systems 14JASS 3Tutorials 2

Using 3d points within World Editor

The Warcraft III [The Frozen Throne]'s map editor (WE, World Editor) has a tool palette from which the Terrain Palette, Doodad Palette, Unit Palette, Region Palette and the Camera Palette can be accesed.

This means that WE has first class support for terraining, doodads, units, regions and cameras, but no points, unlike the Starcraft 2 map editor.

With that said, we can repurpose one of the tool palette's objects and turn it into a 3d point. The one that I think is best suited is the Camera object.

We can see the camera objects within WE when the Camera Palette is active or when the View/Camera Objects setting is enabled. We can turn a camera into a 3d point by changing a few of it's properties:

The important ones are: Target X, Target Y, Z Offset, Angle of Attack (AoA) and Distance. I recommend that you use 270.00 for AoA and 100 for distance this would make the camera point downwards and make it small.

The Target X, Taget Y and Z Offset would of course be our point's X, Y and Z coordinates respectively.

When we create a camera object with the Camera Palette, WE will make it accessable to us by generating some Jass2 code:

 JASS:

globals
 camerasetup gg_cam_Top_of_Mountain = null
endglobals

function CreateCameras takes nothing returns nothing

 set gg_cam_Top_of_Mountain = CreateCameraSetup()
 call CameraSetupSetField(gg_cam_Top_of_Mountain, CAMERA_FIELD_ZOFFSET, 0.0, 0.0)
 call CameraSetupSetField(gg_cam_Top_of_Mountain, CAMERA_FIELD_ROTATION, 90.0, 0.0)
 call CameraSetupSetField(gg_cam_Top_of_Mountain, CAMERA_FIELD_ANGLE_OF_ATTACK, 270.0, 0.0)
 call CameraSetupSetField(gg_cam_Top_of_Mountain, CAMERA_FIELD_TARGET_DISTANCE, 100.0, 0.0)
 call CameraSetupSetField(gg_cam_Top_of_Mountain, CAMERA_FIELD_ROLL, 0.0, 0.0)
 call CameraSetupSetField(gg_cam_Top_of_Mountain, CAMERA_FIELD_FIELD_OF_VIEW, 120.0, 0.0)
 call CameraSetupSetField(gg_cam_Top_of_Mountain, CAMERA_FIELD_FARZ, 101.0, 0.0)
 call CameraSetupSetDestPosition(gg_cam_Top_of_Mountain, 439.0, 306.5, 0.0)

endfunction

We can see that cameras are represented by the type camerasetup. Also note the name of the global variable that WE assigned to our camera: it is the one we used in the Camera Palette but space characters were replaced by underscores and it has the prefix: gg_cam_

The way we can get the x, y and z coordinates from the camera object is:

 JASS:

globals
 location loc = Location(0.0, 0.0)
 real px
 real py
 real pz
endglobals

...

set px = CameraSetupGetDestPositionX(gg_cam_Top_of_Mountain)
set py = CameraSetupGetDestPositionY(gg_cam_Top_of_Mountain)
call MoveLocation(loc, px, py)
set pz = CameraSetupGetField(gg_cam_Top_of_Mountain, CAMERA_FIELD_ZOFFSET) + GetLocationZ(loc)

...

And there you have it, 3d points within WE which have first class spupport and are easy to use via script.

#### Attachments

top-of-mountain.png

 389.7 KB

 · Views: 420



#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

 ut no points, unlike the Starcraft 2 map editor.


Click to expand...

You can use a small region for that purpose. Not as good, I admit. But it works.

Also, misleading title.

There are 3d points Warcraft, named x,y,z reals.

And you do not really teach how to use that.

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Aniki said:

 WE will make it accessable to us by generating some Jass2 code:


Click to expand...

Isnt such code like shown auto generated internaly when you create a camera?

I'm not very sure how/where you will use the 3D camera point.

It's actually only for scripts, and not for you to work in editor? (like from title I assumed "Using 3d points within World Editor").

Can you add example what to do with it?

Or is it only how to get x/y/z of a set up camera with scripts?

#### IcemanBo

#### IcemanBo

 Level 38

JoinedSep 6, 2013Messages6,741

Resources: 27

#### Resources
Tools 1Maps 3Spells and Systems 11Templates 1JASS 6Tutorials 5

Submission: Using 3D Points In World Editor

Date:31 December 2016

Status:Graveyard

Note:

Seems actually interesting, but moving to grayevard due to lacking response.

When you find time for it, just post and we will re-evaluate.

 You must log in or register to reply here.


### Similar threads

Locked

[JASS]
 Camera Error


Avator

Dec 25, 2008

Triggers & Scripts

Replies4Views1K

Dec 25, 2008
Avator

Locked

[JASS]
 CBS Easy camera help


Flame_Phoenix

Jul 31, 2008

Triggers & Scripts

Replies6Views1K

Aug 1, 2008
Squiggy

[Mapping]
 Ping Mini-map in a very simple way using coordinates


mckill2009

Jun 27, 2011

Tutorial Archive

Replies6Views4K

Oct 4, 2011
PurgeandFire

 Advanced Third Person Camera


Megafyr

Oct 4, 2009

Tutorial Archive

Replies10Views22K

Jun 23, 2010
Bommelding

 Cameras


PurgeandFire

Mar 5, 2010

Misc/General Mapping Tutorials

Replies21Views47K

Oct 30, 2018
GrandMaitre

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