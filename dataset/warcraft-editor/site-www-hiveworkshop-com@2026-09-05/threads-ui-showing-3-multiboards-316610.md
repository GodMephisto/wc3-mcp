---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-showing-3-multiboards.316610/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Showing 3 Multiboards
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-showing-3-multiboards.316610/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

The Frame natives are quite powerful and allow us to show more than 1 working multiboard at the same time.

There are some stuff one needs to know to do that. First one has to know that after a multiboard is shown the first time, its framehandle can be accessed with BlzGetFrameByName("Multiboard", 0). By having that frame one can change its position and visibility using the frame natives, which we will do in this example. One could look at how to positionate Frames for more details about that positionating subject.

As wrote above, when Multiboards are been shown the first time, they will take createcontext 0, in GUI that happens when creating a multiboard. Also further multiboards will also use createcontext 0. Means one has to save the frame in a variable before you create another multiboard (GUI). That has to be done cause showing another multiboard will hide the previous shown one and overwrite the framestorage we load with BlzGetFrameByName. Therefore we have to save all wanted multiboard-Frames in varibales. After all multiboards were created + shown once. Show all multiboard frames using BlzFrameSetVisible(multiFrame, true).

##
 Example

In this example 3 multiboards are created and this 3 are positionated somewhere else. Multiboard 1 is below Board 2 and Board 2 below Board 3. Cause in this example the Boards are below each other (y wise), their position has to be updated frequently based on them beeing open or not. If one would not do that the Multiboards would overlap and be shown only partly which does not look good.

Thats how it should look. Only In 4:3 Resolution. In 16:9 they are a stair.

That is the Lua code creating the "good" looking example.

First 6 variables are defined, we need them to frequently repos the multiboards.

It follows the function UpdateMultiboards which reposes multi2 and multi1 based on the state of multi 3 or 2. If the multiboard above is open then pos the multiboard below its MultiboardListContainer, if it is closed place it directly below the multiboard.

CreateMultiboards() creates the multiboards. It has to be exectued for that thing to work. Cause we requier the boards to be visible it should be called not before elapsed time 0.00.

 Lua:

multi1 = nil
multi1Container = nil --that is the frame containing the multiboard items.
multi2 = nil
multi2Container = nil
multi3 = nil
multi3Container = nil

function UpdateMultiboards()
 --update 5 times a second the position of the multiboards.
 --this has to be done cause multiboards can be opened and closed. In closed and opened state I repos them to save screen space. One could use events but nah to much extra work, set up a trigger events...

 BlzFrameClearAllPoints(multi1)
 if BlzFrameIsVisible(multi2Container) then --multiboard 2 is open?
 --yes, pos multiboard 1 below the container.
 BlzFrameSetPoint(multi1, FRAMEPOINT_TOPRIGHT, multi2Container, FRAMEPOINT_BOTTOMRIGHT,0,0)
 else
 --no, pos multiboard 1 below multiboard2.
 BlzFrameSetPoint(multi1, FRAMEPOINT_TOPRIGHT, multi2, FRAMEPOINT_BOTTOMRIGHT,0,0)
 end

 BlzFrameClearAllPoints(multi2)
 if BlzFrameIsVisible(multi3Container) then
 BlzFrameSetPoint(multi2, FRAMEPOINT_TOPRIGHT, multi3Container, FRAMEPOINT_BOTTOMRIGHT,0,0)
 else
 BlzFrameSetPoint(multi2, FRAMEPOINT_TOPRIGHT, multi3, FRAMEPOINT_BOTTOMRIGHT,0,0)
 end

end
function CreateMultiboards()

 CreateMultiboardBJ( 4, 2, "Board1" )
 multi1 = BlzGetFrameByName("Multiboard",0)
 multi1Container = BlzGetFrameByName("MultiboardListContainer",0)

 CreateMultiboardBJ( 3, 3, "Board2" )
 multi2 = BlzGetFrameByName("Multiboard",0)
 multi2Container = BlzGetFrameByName("MultiboardListContainer",0)

 CreateMultiboardBJ( 3, 1, "Board3" )
 multi3 = BlzGetFrameByName("Multiboard",0)
 multi3Container = BlzGetFrameByName("MultiboardListContainer",0)

 BlzFrameClearAllPoints(multi1)
 BlzFrameSetPoint(multi1, FRAMEPOINT_TOPRIGHT, multi2, FRAMEPOINT_BOTTOMRIGHT,0,0)

 BlzFrameClearAllPoints(multi2)
 BlzFrameSetPoint(multi2, FRAMEPOINT_TOPRIGHT, multi3, FRAMEPOINT_BOTTOMRIGHT,0,0)

 BlzFrameClearAllPoints(multi3)
 BlzFrameSetAbsPoint(multi3, FRAMEPOINT_TOPRIGHT, 0.5,0.55)

 BlzFrameSetVisible(multi1, true)
 BlzFrameSetVisible(multi2, true)
 BlzFrameSetVisible(multi3, true)
 TimerStart(CreateTimer(), 0.2, true, UpdateMultiboards)
end

The fdf of the multiboard.

 Code:

// -- INCLUDE FILES ---------------------------------------------------------

IncludeFile "UI\FrameDef\UI\EscMenuTemplates.fdf",

// -- LOCAL TEMPLATES -------------------------------------------------------

// -- FRAMES ----------------------------------------------------------------

Frame "FRAME" "Multiboard" {
 Height 0.024,
 Width 0.024,

 Frame "GLUETEXTBUTTON" "MultiboardMinimizeButton" {
 SetAllPoints,

 ControlBackdrop "ButtonBackdropTemplate",
 Frame "BACKDROP" "ButtonBackdropTemplate" {
 DecorateFileNames,
 BackdropBackground "MultiboardMinimizeButtonEnabled",
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.0125,
 BackdropBackgroundInsets 0.005f 0.005f 0.005f 0.005f,
 BackdropEdgeFile "MultiboardBorder",
 }

 ControlPushedBackdrop "ButtonPushedBackdropTemplate",
 Frame "BACKDROP" "ButtonPushedBackdropTemplate" {
 DecorateFileNames,
 BackdropBackground "MultiboardMinimizeButtonPushed",
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.0125,
 BackdropBackgroundInsets 0.005f 0.005f 0.005f 0.005f,
 BackdropEdgeFile "MultiboardBorder",
 }

 ControlDisabledBackdrop "ButtonDisabledBackdropTemplate",
 Frame "BACKDROP" "ButtonDisabledBackdropTemplate" {
 DecorateFileNames,
 BackdropBackground "EscMenuButtonBackground",
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.0125,
 BackdropBackgroundInsets 0.005f 0.005f 0.005f 0.005f,
 BackdropEdgeFile "MultiboardBorder",
 }

 ControlDisabledPushedBackdrop "ButtonDisabledPushedBackdropTemplate",
 Frame "BACKDROP" "ButtonDisabledPushedBackdropTemplate" {
 DecorateFileNames,
 BackdropBackground "MultiboardMinimizeButtonDisabled",
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.0125,
 BackdropBackgroundInsets 0.005f 0.005f 0.005f 0.005f,
 BackdropEdgeFile "MultiboardBorder",
 }
 }

 Frame "BACKDROP" "MultiboardTitleBackdrop" {
 Width 0.2f,
 //Height 0.011f,
 SetPoint TOPRIGHT, "MultiboardMinimizeButton", TOPLEFT, 0.0057, 0.0,
 SetPoint BOTTOMRIGHT, "MultiboardMinimizeButton", BOTTOMLEFT, 0.0057, 0.0,
 UseActiveContext,
 SetAllPoints,
 DecorateFileNames,
 BackdropTileBackground,
 BackdropBackground "MultiboardBackground",
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.0125,
 BackdropBackgroundInsets 0.005f 0.005f 0.005f 0.005f,
 BackdropEdgeFile "MultiboardBorder",
 BackdropBlendAll,
 }

 Frame "TEXT" "MultiboardTitle" INHERITS "EscMenuLabelTextTemplate" {
 UseActiveContext,
 SetPoint TOPLEFT, "MultiboardTitleBackdrop", TOPLEFT, 0.0, 0.0,
 SetPoint BOTTOMRIGHT, "MultiboardTitleBackdrop", BOTTOMRIGHT, 0.0, 0.0,
 FrameFont "MasterFont", 0.011, "",
 FontJustificationH JUSTIFYCENTER,
 }

 Frame "BACKDROP" "MultiboardBackdrop" {
 UseActiveContext,
 SetPoint TOPRIGHT, "MultiboardMinimizeButton", BOTTOMRIGHT, 0.0, 0.0057,
 SetPoint TOPLEFT, "MultiboardTitleBackdrop", BOTTOMLEFT, 0.0, 0.0057,
 DecorateFileNames,
 BackdropTileBackground,
 BackdropBackground "MultiboardBackground",
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.0125,
 BackdropBackgroundInsets 0.005f 0.005f 0.005f 0.005f,
 BackdropEdgeFile "MultiboardBorder",
 BackdropBlendAll,
 }

 Frame "FRAME" "MultiboardListContainer" {
 UseActiveContext,
 SetPoint TOPLEFT, "MultiboardBackdrop", TOPLEFT, 0.001f, -0.0048,
 SetPoint BOTTOMRIGHT, "MultiboardBackdrop", BOTTOMRIGHT, -0.001f, 0.001,
 }
}

###
 Other UI-Frame Tutorials

The following links might provide more insight into this subject.

UI: Change Lumber Text

[JASS/AI] - UI: Create a TextButton

[JASS/AI] - UI: Positionate Frames (important)

UI: toc-Files

UI: Reading a FDF

UI - The concept of Parent-Frames

[JASS/AI] - UI: FrameEvents and FrameTypes

UI: Frames and Tooltips

[JASS/AI] - UI: Creating a Bar

UI - Simpleframes

UI: What are BACKDROPs?

UI: GLUEBUTTON

UI: TEXTAREA the scrolling Text Frame

UI: EditBox - Text Input

[JASS/AI] - UI: Creating a Cam control

UI: Showing 3 Multiboards

UI: OriginFrames

Default Names for BlzGetFrameByName (Access to Existing game-Frames)

[JASS/AI] - UI: List - Default MainFrames (built in CreateAble)

#### Attachments

Multiboards and Frames.w3x

 17.1 KB

 · Views: 292




 Last edited: Apr 15, 2022

#### Rahko

#### Rahko

 Level 9

JoinedJul 30, 2018Messages445

Resources: 2

#### Resources
Spells and Systems 2

Why do the multiboards align like this in my game?

(Same happened in my own map when I imported that to it.)

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

For some Reasons: Some Frames are displayed different when the screen-resolution is 4:3 or 16:9. my Images were taken in window mode in 4:3.

#### Rahko

#### Rahko

 Level 9

JoinedJul 30, 2018Messages445

Resources: 2

#### Resources
Spells and Systems 2

Yeah, I figured, too, that it had to be something to do with the widescreen. They probably added some sort of multiplier for the x axis for the certain frames that can be put to the edge of the widescreen, instead of, say, simply allowing the x point to go beyond 0.8. One implication of this is that the gap actually grows if you put the original frame closer to the edge.

I fixed it by putting -0.1325 on the x axis for the other two multiboards, so they line up straight now (when the origin frame is at 0.8 x. At 0.5 x (like in your example), -0.0325 x is enough to align the other frames straight).

#### lolreported

#### lolreported

 Level 20

JoinedAug 16, 2007Messages885

Resources: 25

#### Resources
Tools 1Maps 9Spells and Systems 15

Any solution found for the misalignment of the multiboards?

The multiboards align perfectly when the relative is closed, but when it's opened the bottom multiboard is squished towards the right side.

I'd rather not rely on manually setting the x and y values, as this will mess with different resolutions and aspect ratios.

I'm using the code above as base.

I had to adjust the framepoints for when the top multiboard is closed though.

call BlzFrameSetPoint(mbLeaderboardHandle, FRAMEPOINT_TOP, mbScoreboardHandle, FRAMEPOINT_BOTTOM,0,0)


 Last edited: Apr 2, 2020

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I would argue that this is not really a tutorial.

You don't really teach someone how to do it, it's very similar to a system submission in my opinion given that it's along the lines of:

Copy this code and don't sweat about the details.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

You can graveyard it, if you want.

#### SinisterLuffy

#### SinisterLuffy

 Level 1

JoinedApr 8, 2020Messages109

Instead of the graveyard, why not move it to the Spell Section for Templates?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

I'll leave it up for a few more days so Tasyen can copy the code if he wants to make it a spell submission isntead.

I cannot straight up move it there because there needs to be a picture and tags etc.

#### orgiamode

#### orgiamode

 Level 2

JoinedAug 30, 2023Messages3

Hello, Tasyen teacher, I am a Korean Warcraft user.

I'm studying Frame by looking at the posts you posted through a translator.

If you don't mind, can I ask your advice?

Where do MultiboardMinimizeButtonEnabled and MultiboardBorder refer to the string table when calling file names through DecorateFileNames within fdf?

I couldn't find those keywords in datestring, globalstring, infopanelstring, and networkstring that you mentioned in other posts.

I'm using this site for the first time, so the reply may be late. I apologize if I've been rude...

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

They are from UI/war3skins.txt or gameinterface inside world editor (left side when in raw data view). war3skins.txt moved a around in some warcraft 3 versions it is in localized files.

In Reforged you can read this stringtable in mapscript using SkinManagerGetLocalPath(source).


 Last edited: Aug 30, 2023

#### orgiamode

#### orgiamode

 Level 2

JoinedAug 30, 2023Messages3

Tasyen said:

 They are from UI/war3skins.txt or gameinterface inside world editor (left side when in raw data view). war3skins.txt moved a around in some warcraft 3 versions it is in localized files.

In Reforged you can read this stringtable in mapscript using SkinManagerGetLocalPath(source).


Click to expand...

I found it right away Thank you !!

 You must log in or register to reply here.


### Similar threads

Locked

[General]
 Mouse-cursor jumps on UI button click


Antares

Jan 7, 2024

World Editor Help Zone

Replies6Views2K

Jan 9, 2024
Antares

 UI: Reading a FDF


Tasyen

May 29, 2019

Scripting Tutorials

Replies15Views16K

Dec 1, 2020
Tasyen

 UI: Positionate Frames


Tasyen

May 29, 2019

Scripting Tutorials

Replies25Views19K

Feb 11, 2026
Wareditor

 UI: What are BACKDROPs?


Tasyen

Jun 1, 2019

Scripting Tutorials

Replies27Views13K

Jan 12, 2022
q531620267

 UI: Frames and Tooltips


Tasyen

Jun 27, 2019

Scripting Tutorials

Replies45Views21K

Oct 27, 2024
RPG_F

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