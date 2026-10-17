---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/fixing-smartcamerapanbj-desync.243334/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Fixing Smartcamerapanbj Desync
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/fixing-smartcamerapanbj-desync.243334/>
#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

THIS ISSUE IS FIXED FOR PATCH 1.31+ ACCORDING TO REPORTS

Introduction of SmartCameraPanBJ

This Guide is made to fix

Camera - Pan Camera as necessary (timed)

As some of you may know, the function does desync on multiplayer maps (100% guaranteed).

What is Desync? Desync is an event in multiplayer games where all players that plays (or some players, in that case it's possible to be a server split) become disconnected except the host itself.

Copy the code into the Map Header (the one above all triggers folders)

 JASS:

function SmartCameraPanBJModified takes player whichPlayer, location loc, real duration returns nothing
 local real tx = GetLocationX(loc)
 local real ty = GetLocationY(loc)
 local real dx = tx - GetCameraTargetPositionX()
 local real dy = ty - GetCameraTargetPositionY()
 local real dist = SquareRoot(dx * dx + dy * dy)
 if (GetLocalPlayer() == whichPlayer) then
 if (dist >= bj_SMARTPAN_TRESHOLD_SNAP) then
 call PanCameraToTimed(tx, ty, 0)
 // Far away = snap camera immediately to point
 elseif (dist >= bj_SMARTPAN_TRESHOLD_PAN) then
 call PanCameraToTimed(tx, ty, duration)
 // Moderately close = pan camera over duration
 else
 // User is close, don't move camera
 endif
 endif
endfunction

to call the function :

call SmartCameraPanBJModified( Player(), Location(), Timer() )

Replace SmartCameraPanBJModified with your modified name.

Remember, Player() is the Player, Location() is the target location and Timer() is the time amount for the pan.

A recommendation for GUI Users is to set these 3 things into variables and call the function with those variables instead, ex (by PurgeandFire) :

Set TempPlayer = Player 1 (Red)

Set TempLoc = (Position of (Triggering unit))

Set TempDuration = 12.00

Custom script: call SmartCameraPanBJModified( udg_TempPlayer, udg_TempLoc, udg_TempDuration )

Notes : Global Variables uses udg_VariableName in Jass, so don't miss them when calling the custom native!

The Actual Problem with SmartCameraPanBJ

SmartCameraPanBJ desyncs was a mystery, in GUI, this can't be solved, but the actual problem lies on how the function execute itself.

 JASS:

function SmartCameraPanBJ takes player whichPlayer, location loc, real duration returns nothing
 local real dist
 if (GetLocalPlayer() == whichPlayer) then
 // Use only local code (no net traffic) within this block to avoid desyncs.

 set dist = DistanceBetweenPoints(loc, GetCameraTargetPositionLoc())
 if (dist >= bj_SMARTPAN_TRESHOLD_SNAP) then
 // If the user is too far away, snap the camera.
 call PanCameraToTimed(GetLocationX(loc), GetLocationY(loc), 0)
 elseif (dist >= bj_SMARTPAN_TRESHOLD_PAN) then
 // If the user is moderately close, pan the camera.
 call PanCameraToTimed(GetLocationX(loc), GetLocationY(loc), duration)
 else
 // User is close enough, so don't touch the camera.
 endif
 endif
endfunction

The code create location on local block (this one : set dist = DistanceBetweenPoints(loc, GetCameraTargetPositionLoc())), which causes desync as location may not be created at local block

How to solve this problem?

It might looks complicated, but it's not THAT complicated. Read below to know how to fix it :

The script below is the fixer for this issue (Credits to PurgeandFire) :

 JASS:

function SmartCameraPanBJ takes player whichPlayer, location loc, real duration returns nothing
 local real tx = GetLocationX(loc)
 local real ty = GetLocationY(loc)
 local real dx = tx - GetCameraTargetPositionX()
 local real dy = ty - GetCameraTargetPositionY()
 local real dist = SquareRoot(dx * dx + dy * dy)
 if (GetLocalPlayer() == whichPlayer) then
 if (dist >= bj_SMARTPAN_TRESHOLD_SNAP) then
 call PanCameraToTimed(tx, ty, 0)
 // Far away = snap camera immediately to point
 elseif (dist >= bj_SMARTPAN_TRESHOLD_PAN) then
 call PanCameraToTimed(tx, ty, duration)
 // Moderately close = pan camera over duration
 else
 // User is close, don't move camera
 endif
 endif
endfunction

However, if you directly copy this script to your map header (or a trigger), it will causes map compiling to fail, there are 2 workaround to this :

1. Copy the script to Map Header, to deal with the bug mentioned above, I recommend copy pasting this script instead :

 JASS:

function SmartCameraPanBJModified takes player whichPlayer, location loc, real duration returns nothing
 local real tx = GetLocationX(loc)
 local real ty = GetLocationY(loc)
 local real dx = tx - GetCameraTargetPositionX()
 local real dy = ty - GetCameraTargetPositionY()
 local real dist = SquareRoot(dx * dx + dy * dy)
 if (GetLocalPlayer() == whichPlayer) then
 if (dist >= bj_SMARTPAN_TRESHOLD_SNAP) then
 call PanCameraToTimed(tx, ty, 0)
 // Far away = snap camera immediately to point
 elseif (dist >= bj_SMARTPAN_TRESHOLD_PAN) then
 call PanCameraToTimed(tx, ty, duration)
 // Moderately close = pan camera over duration
 else
 // User is close, don't move camera
 endif
 endif
endfunction

to call the function :

call SmartCameraPanBJModified( Player(), Location(), Timer() )

Replace SmartCameraPanBJModified with your modified name.

Remember, Player() is the Player, Location() is the target location and Timer() is the time amount for the pan.

A recommendation for GUI Users is to set these 3 things into variables and call the function with those variables instead, ex (by PurgeandFire) :

Set TempPlayer = Player 1 (Red)

Set TempLoc = (Position of (Triggering unit))

Set TempDuration = 12.00

Custom script: call SmartCameraPanBJModified( udg_TempPlayer, udg_TempLoc, udg_TempDuration )

Notes : Global Variables uses udg_VariableName in Jass, so don't miss them when calling the custom native!

2. Override the original script at blizzard.j then import it to the map. This will allow you to use the default native without issues, because blizzard.j from the map will be read by the game instead of the default.

CREDITS

PurgeandFire : The Fixing Script and Helping Me with the workaround

Legal_Ease : On our test with https://www.hiveworkshop.com/forums/maps-564/high-road-aos-238438/?prev=status=p made by him, which let me to search for solutions once people point out the issue with SmartCameraPanBJ.

~Daffa the Mage


 Last edited: Dec 21, 2023

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

It looks good. Although you should give a sample of using it in GUI. To make it friendly, you can probably use the variable technique, ex:

Set TempPlayer = Player 1 (Red)

Set TempLoc = (Position of (Triggering unit))

Set TempDuration = 12.00

Custom script: call SmartCameraPanBJModified( udg_TempPlayer, udg_TempLoc, udg_TempDuration )

So that they only have to modify the variables, rather than modifying the actual code.

You should also name the function differently in the example. I know you tell them to change it, but it'd be better if you change it to SmartCameraPanBJModified just so that there is a lower chance of error (for hasty people who don't like to read, or people who may be lost at what to name it/how JASS works).

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Modified as suggested.

I hope I don't miss anything.

EDIT :

When I submit it as a Tutorial at Blizzard Modding, there's something else that I want to tell everyone.


 Moyack@BlizzMod said:



 This tutorial gave me an idea: make a snippet code in vjass to solve this issue and offer it as a third solution.


Click to expand...


 Last edited: Oct 23, 2013

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Nice. Approved.

And yeah, there used to be a snippet called "SmartCameraPanBJND" or "SmartCameraPanND" (ND = No Desync) on the old wc3jass. I believe it would be very fitting to make another snippet like that.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

I see, sadly wc3jass got shutdown and it's gone.

Unless you have it somewhere in your data?

anyway thanks for the approval, I hope this can be useful for people who usually get desync at their maps.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

*Tutorial Necromancy*

Alright, some minor cleanup with the old Tutorial. I hope it's useful to some people.

Also, if anyone would like to point out mistakes, I would be pleased to fix them.

#### venger07

#### venger07

 Level 16

JoinedDec 21, 2010Messages592

Resources: 11

#### Resources
Models 2Icons 5Maps 3Spells and Systems 1

##
 Zombie Island Survival


How to use pan camera without leak?

I used this custom script " call SmartCameraPanBJModified( udg_TempPlayer, udg_TempLoc, udg_TempDuration ) " but it gave me an error saying "expected a function name"


[EDIT] I removed "Modified" and it doesn't give an error anymore.. so the custom script should be like this


 call SmartCameraPanBJ( udg_TempPlayer, udg_TempLoc, udg_TempDuration )

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

venger07 said:

 I used this custom script " call SmartCameraPanBJModified( udg_TempPlayer, udg_TempLoc, udg_TempDuration ) " but it gave me an error saying "expected a function name"


[EDIT] I removed "Modified" and it doesn't give an error anymore.. so the custom script should be like this


 call SmartCameraPanBJ( udg_TempPlayer, udg_TempLoc, udg_TempDuration )


Click to expand...


You have to put this script in your map header:

 JASS:

function SmartCameraPanBJModified takes player whichPlayer, location loc, real duration returns nothing
 local real tx = GetLocationX(loc)
 local real ty = GetLocationY(loc)
 local real dx = tx - GetCameraTargetPositionX()
 local real dy = ty - GetCameraTargetPositionY()
 local real dist = SquareRoot(dx * dx + dy * dy)
 if (GetLocalPlayer() == whichPlayer) then
 if (dist >= bj_SMARTPAN_TRESHOLD_SNAP) then
 call PanCameraToTimed(tx, ty, 0)
 // Far away = snap camera immediately to point
 elseif (dist >= bj_SMARTPAN_TRESHOLD_PAN) then
 call PanCameraToTimed(tx, ty, duration)
 // Moderately close = pan camera over duration
 else
 // User is close, don't move camera
 endif
 endif
endfunction


Copy all of that, open your map, go to the trigger editor, click on the map's name in the upper left corner (above all the folders), and paste the code there. Then it should save without issues.


And this tutorial's main purpose is to prevent a desync. If you use SmartCameraPanBJ, then your map will disconnect players. So you should not use it.

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

venger07 said:

 I used this custom script " call SmartCameraPanBJModified( udg_TempPlayer, udg_TempLoc, udg_TempDuration ) " but it gave me an error saying "expected a function name"


[EDIT] I removed "Modified" and it doesn't give an error anymore.. so the custom script should be like this


 call SmartCameraPanBJ( udg_TempPlayer, udg_TempLoc, udg_TempDuration )


Click to expand...


Sorry for the late reply.


Purge explains it well in his post.


The SmartCameraPanBJ is the one which desync. The main code must exist in the map header as Purge states.

PurgeandFire said:

 You have to put this script in your map header:

 JASS:

function SmartCameraPanBJModified takes player whichPlayer, location loc, real duration returns nothing
 local real tx = GetLocationX(loc)
 local real ty = GetLocationY(loc)
 local real dx = tx - GetCameraTargetPositionX()
 local real dy = ty - GetCameraTargetPositionY()
 local real dist = SquareRoot(dx * dx + dy * dy)
 if (GetLocalPlayer() == whichPlayer) then
 if (dist >= bj_SMARTPAN_TRESHOLD_SNAP) then
 call PanCameraToTimed(tx, ty, 0)
 // Far away = snap camera immediately to point
 elseif (dist >= bj_SMARTPAN_TRESHOLD_PAN) then
 call PanCameraToTimed(tx, ty, duration)
 // Moderately close = pan camera over duration
 else
 // User is close, don't move camera
 endif
 endif
endfunction


Copy all of that, open your map, go to the trigger editor, click on the map's name in the upper left corner (above all the folders), and paste the code there. Then it should save without issues.


And this tutorial's main purpose is to prevent a desync. If you use SmartCameraPanBJ, then your map will disconnect players. So you should not use it.


Click to expand...


Thanks bro

#### Daffa

#### Daffa

Patron

 Level 50

JoinedJan 30, 2013Messages12,953

Resources: 157

#### Resources
Models 1Icons 2Packs 5Tools 4Maps 29Spells and Systems 107Reforged HD Icons 4Tutorials 5

Added a notification for 1.31 users. I've read form Bribe's fixing GUI script thread that SmartCameraPanBJ has been fixed.

 You must log in or register to reply here.


### Similar threads

Locked

 Desync checker for JASS snippets


LeP

Sep 5, 2019

Warcraft Editing Tools

Replies4Views4K

Sep 7, 2019
LeP

Locked

[JASS]
 GetCameraTargetPositionX/Y


Starquizer

Apr 12, 2012

Triggers & Scripts

Replies8Views3K

Apr 13, 2012
Maker

Locked

[JASS]
 Spell Help Needed


Quetzalcotl

Feb 14, 2010

Triggers & Scripts

Replies13Views2K

Feb 15, 2010
Quetzalcotl

Locked

[vJASS]
 HandleId bug


Starquizer

Mar 16, 2012

Triggers & Scripts

Replies16Views3K

Mar 19, 2012
Starquizer

Locked

[JASS]
 Not working trigger


Quetzalcotl

Oct 4, 2009

Triggers & Scripts

Replies3Views1K

Oct 4, 2009
Quetzalcotl

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