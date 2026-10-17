---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-positionate-frames.315860/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Positionate Frames
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-positionate-frames.315860/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

##
 Introduction

Frames positions are important and are the key to place frames where one wants them to be.

To positionate frames there exist 4 natives. All 4 have great uses.

 JASS:

native BlzFrameSetPoint takes framehandle frame, framepointtype point, framehandle relative, framepointtype relativePoint, real x, real y returns nothing
native BlzFrameSetAbsPoint takes framehandle frame, framepointtype point, real x, real y returns nothing
native BlzFrameClearAllPoints takes framehandle frame returns nothing
native BlzFrameSetAllPoints takes framehandle frame, framehandle relative returns nothing

To positionate frames one also needs the framepointtype constants.

 JASS:

 FRAMEPOINT_TOPLEFT
 FRAMEPOINT_TOP
 FRAMEPOINT_TOPRIGHT
 FRAMEPOINT_LEFT
 FRAMEPOINT_CENTER
 FRAMEPOINT_RIGHT
 FRAMEPOINT_BOTTOMLEFT
 FRAMEPOINT_BOTTOM
 FRAMEPOINT_BOTTOMRIGHT

##
 What the natives do?

BlzFrameSetAbsPoint takes one point of a Frame unbound that point and places it to a specific coordinates on the screen.

BlzFrameSetPoint places a point of FrameA relative to a point of FrameB. When FrameB moves FrameA's point will keep this rule and moves with it. The x/y-Offset Values are affected by FrameA Scale.

BlzFrameClearAllPoints removes all curent bound points of that frame.

BlzFrameSetAllPoints FrameA will copy FrameB in size and position. FrameA will update when FrameB is changed later.

​

##
 Cartesian coordinate system

coordinates and offset are both a factor of the total screen. 0.0/0.0 beeing the bottom left corner of the 4:3 screen. 0.8/0.6 is the top right corner of the 4:3 screen.

Positionating requiers the frame one wants to move and the FRAMEPOINT of that frame. One moves only the choosen FRAMEPOINT, if other FRAMEPOINTS are still located somewhere, the frame will be stretched to have both FRAMEPOINTS be filled as good that is possible, for a rectangle. This can (sometimes has to) be used to define size of frames.

Here one should positionate either FRAMEPOINT_TOPRIGHT and FRAMEPOINT_BOTTOMLEFT or FRAMEPOINT_TOPLEFT and FRAMEPOINT_BOTTOMRIGHT of the frame one wants to set its size.

When placing only one FramePoint the choosen FramePoint has a strong impact on the result. It limites the directions size/text can expand.

When using a point containing:

LEFT can only extend to the right x wise.

RIGHT can only extend to the left x wise.

CENTER extends equal in all directions

TOP can only extend downwards y wise

BOTTOM can only extend upwards y wise.​

If one simply wants to move a frame having a size one should first use BlzFrameClearAllPoints to free already placed Points.

SetPoint requires the relative frame to take space on the screen, that relative frame can be set on a later time, but until that happens the moved frame will displayed wierdly or not all.​

##
 Relative Position

Frames that belong logical to some other Frame benefit much when one sets their position relative to that frame they belong to or relative to a frame that is relative to the owner that could be contiuned, but in the end it is relative to the Owner Frame. This relative position is updated when the other Frame is moved. Because of that one only needs to set the position of the not relative posed Frame in that group and everything moves with it. In code the native doing that is BlzFrameSetPoint.

Setting Relative position is a little bit inversed. when one was to place something left to something one needs to place the right side (of the moving) to the left of the target.

 JASS:

BlzFrameSetPoint(frameLeft, FRAMEPOINT_RIGHT, frameTarget, FRAMEPOINT_LEFT, 0, 0)
BlzFrameSetPoint(frameRight, FRAMEPOINT_LEFT, frameTarget, FRAMEPOINT_RIGHT, 0, 0)

The x and y offset in BlzFrameSetPoint are added to the target Point. with +x the position anchored to is moved further to the right and with +y it is moved higher up. BlzFrameSetScale of the moving frame affect the x&y offset.

In Fdf one can only set position relative. But in Code one can also place them to a fixed position.​

##
 Example

Lets move the menu button somewhere else, somewhere to the right top corner.

 JASS:

function ReposMenuButtons takes nothing returns nothing
 local framehandle fh = BlzGetFrameByName("UpperButtonBarMenuButton",0)
 call BlzFrameSetAbsPoint(fh, FRAMEPOINT_RIGHT, 0.5, 0.5)
endfunction

Not what we wanted. The Menu button stretched far to the right, pushed the Allies & Log button to the right and did not move lower. It seems his left side is still connected to the Quest button.

It should work better, if we clear the points of Menu button.

 JASS:

function ReposMenuButtons2 takes nothing returns nothing
 local framehandle fh = BlzGetFrameByName("UpperButtonBarMenuButton",0)
 call BlzFrameClearAllPoints(fh)
 call BlzFrameSetAbsPoint(fh, FRAMEPOINT_RIGHT, 0.5, 0.5)
endfunction

Well the Menu button moved, also the Allies & Log buttons, the texts are bit off, but it moved away from the Quest button and is not stretched.

Lets move the Allies button below the Menu button and the Log button below the Allies button.

 JASS:

function ReposMenuButtons3 takes nothing returns nothing
 local framehandle allies = BlzGetFrameByName("UpperButtonBarAlliesButton",0)
 local framehandle menu = BlzGetFrameByName("UpperButtonBarMenuButton",0)
 local framehandle log = BlzGetFrameByName("UpperButtonBarChatButton",0)
 call BlzFrameClearAllPoints(allies)
 call BlzFrameClearAllPoints(log)
 call BlzFrameClearAllPoints(menu)
 call BlzFrameSetAbsPoint(menu, FRAMEPOINT_RIGHT, 0.5, 0.5) //Pos Menu
 call BlzFrameSetPoint(allies, FRAMEPOINT_TOP, menu, FRAMEPOINT_BOTTOM, 0.0, 0.0) //Pos allies below menu
 call BlzFrameSetPoint(log, FRAMEPOINT_TOP, allies, FRAMEPOINT_BOTTOM, 0.0, 0.0) //Pos log below allies
endfunction

Menu Allies and Log in a col, text is where it should be, I like it.

They are to much in the center, but that is cause we moved menu with FRAMEPOINT_RIGHT. And cause of the wrong information of the total x size, the missinformation was 0.6 as max of the 4:3, but 0.8 is the right end of the 4:3 Screen not 0.6. 0.5 is much nearer to 0.4 then to 0.8 why it is more centered.​

##
 Frame Pos in FDF

When one creates frames in fdf one can also set the position of frames. Here one has 3 options:

SetPoint FramePoint-Own, FrameName, FramePoint-FrameName, xOffset , yOffset,

SetAllPoints,

Anchor FramePoint, x, y, (only Texture/String)​
SetPoint can be seen as BlzFrameSetPoint(self, FramePoint-Own, BlzGetFrameByName(FrameName, 0), FramePoint-FrameName, x, y). With UseActiveContext, the Frame will connect to the relative Frame with the same CreateContext the current Frame is created with.

 Code:

Frame "BACKDROP" "Test" {
 Width 0.1,
 Height 0.1,
 SetPoint TOPLEFT, "ConsoleUI", TOPLEFT, 0, 0,
 BackdropBackground "ReplaceableTextures\CommandButtons\BTNHeroPaladin",
}

That Frame of type BACKDROP with Name Test displays, after it's loading & creation, a Paladin-Icon at the Top Left of the screen (Would have to be loaded over a TOC first).

SetAllPoints is equal to BlzFrameSetAllPoints(self, parent).

Anchor is BlzFrameSetPoint(self, point, parent, point, x, y).​

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

[JASS/AI] - UI: List - Default MainFrames (built in CreateAble)​

#### Attachments

Repos Menu Buttons.w3x

 16.7 KB

 · Views: 615




 Last edited: Nov 21, 2024

#### purparisien

#### purparisien

 Level 43

JoinedJul 17, 2011Messages977

Resources: 157

#### Resources
Models 119Icons 13Skins 2Maps 19Ported Models 4

Its look very interesting. Can you attach an exemple map as you did with your previous tuto ? I would like to test it out

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

A demo map was uploaded.

#### purparisien

#### purparisien

 Level 43

JoinedJul 17, 2011Messages977

Resources: 157

#### Resources
Models 119Icons 13Skins 2Maps 19Ported Models 4

Thanks you so much ! +rep


 Last edited: Jun 13, 2019

#### purparisien

#### purparisien

 Level 43

JoinedJul 17, 2011Messages977

Resources: 157

#### Resources
Models 119Icons 13Skins 2Maps 19Ported Models 4

Hey Tasyen, I like this system a lot. Is it possible to display ressources information in the menu and allies spaces, once we move the buttons away ? I would like to add more ressources in a map by using these spaces. Thanks

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Yes, but that is only partly connceted to the positionate Frames tutorial.

What you would have to do is creating an image and a text frame. You could either use simpleFrames or Frames.

In simpleFrames you would give the simpleFrame an texture and a String child. The Texture would be the icon and the String the shown text. resourcebar.fdf shows that, although they did not gave the textures names which disallows changing the icon later on.

With Frames you would need a Frame of Type "BACKDROP" and one of Type "TEXT".

#### burner89

#### burner89

 Level 3

JoinedMar 11, 2019Messages31

It it possible to move stuff outside of the 4:3 area? If so could you explain for a noob

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

burner89 said:

 It it possible to move stuff outside of the 4:3 area?


Click to expand...

It is possible for simpleFrames by using x values above 0.8 or below 0.0 . One has to consider that models are not simpleframes. Moving them outside of 4:3 will break shown cooldowns for item/command buttons. Also for the same reason one can not move out the 3d Face nor the minimap.

Moves the MenuButton into the 16:9 top left corner.

 JASS:

call BlzFrameSetAbsPoint(BlzGetOriginFrame(ORIGIN_FRAME_SYSTEM_BUTTON, 3), FRAMEPOINT_TOPLEFT, -0.14, 0.6)

To move the minimap one has to define/move 2 points either BOTTOMLEFT and TOPRIGHT, or BOTTOMRIGHT and TOPLEFT. The rect they enclose is the clickable space of the minimap, its not the size of the minimap-model displayed.

#### burner89

#### burner89

 Level 3

JoinedMar 11, 2019Messages31

1) Is there a way to fix the broken cooldowns?

2) How can I stretch a frame after having it positioned? I want to stretch the HP/Mana bars on the hero frame.


 Last edited: Jun 19, 2019

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

burner89 said:

 1) Is there a way to fix the broken cooldowns?


Click to expand...

Do not move them into 16:9, No Idea.

burner89 said:

 2) How can I stretch a frame after having it positioned? I want to stretch the HP/Mana bars on the hero frame.


Click to expand...

You can try out call BlzFrameSetSize(frame, x, y) often that works sometimes it does not. For some frames it works better by positionating BOTTOMLEFT and TOPRIGHT or BOTTOMRIGHT and TOPLEFT.

One can stretch that bar with call BlzFrameSetSize(BlzGetOriginFrame(ORIGIN_FRAME_HERO_HP_BAR,0), 0.2, 0.01)

#### DarkLigthing

#### DarkLigthing

 Level 3

JoinedMar 19, 2018Messages3

Resources: 1

#### Resources
Maps 1

Why does it works wrong for me ?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

When the function executes at map init, the text disposing happens also for me. With Elapsed game time 0.00 seconds the result is much better at least for me.

#### iown_azz

#### iown_azz

 Level 8

JoinedJun 16, 2008Messages333

Resources: 2

#### Resources
Tutorials 2

The only issue I have with the -0.1 is that if you change the window size of Warcraft III as small as the ConsoleUI, then what is in the -0.1 is hidden. Anything that is not a child of ConsoleUI, when you put 0.0, 0.0 it snaps to the corner of the Window and moves when the Window is resized.

I can't figure out how to make children of ConsoleUI child of the window ratio.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

What Frame does that can you show a small example. I only know of some Frames following the WindowSize. Portrait somehow uses a different coord system, but it keeps at bottomLeft when posed at 0,0. Multiboard, LeaderBoard, HeroButtons (but only on init), TimerDialog are others I know of (they keep at top right/left), but I think there is game code doing that in the background and not really a frame api feature.

Instead of using this -values one could create a fullscreen Frame and pos all your Frames relative to that Frame. The fullscreen would have to be frequently updated to be fullscreen with a fast running timer and be posed at the beginning to center so it expands in both directions equaly: Inside that Timer one updates the Size; the formular is not from me.

BlzFrameSetSize(fullscreenFrame, BlzGetLocalClientWidth()/BlzGetLocalClientHeight()*0.6, 0.6)


 Last edited: Jul 23, 2020

#### iown_azz

#### iown_azz

 Level 8

JoinedJun 16, 2008Messages333

Resources: 2

#### Resources
Tutorials 2

Tasyen said:

 What Frame does that can you show a small example. I only know of some Frames following the WindowSize. Portrait somehow uses a different coord system, but it keeps at bottomLeft when posed at 0,0. Multiboard, LeaderBoard, HeroButtons (but only on init), TimerDialog are others I know of (they keep at top right/left), but I think there is game code doing that in the background and not really a frame api feature.

Instead of using this -values one could create a fullscreen Frame and pos all your Frames relative to that Frame. The fullscreen would have to be frequently updated to be fullscreen with a fast running timer and be posed at the beginning to center so it expands in both directions equaly: Inside that Timer one updates the Size; the formular is not from me.

BlzFrameSetSize(fullscreenFrame, BlzGetLocalClientWidth()/BlzGetLocalClientHeight()*0.6, 0.6)

Click to expand...

I'll make a video tomorrow

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Minor changes made, extremely useful and possibly one of the first ones needed when fiddling with the UI.

Approved.

#### Drake53

#### Drake53

 Level 20

JoinedJan 1, 2018Messages777

Not sure if anyone cares but since it's related I'll post some research I did here that hasn't been mentioned yet. I only used abs points for my tests.

When you set multiple points that 'conflict' with each other, there is a certain priority in which points are used for further calculations.

This order is the same as in common.j, meaning that a lower index in ConvertFramePointType has a higher priority:

TopLeft > Left > BottomLeft (left)

Top > Center > Bottom (center X)

TopRight > Right > BottomRight (right)

TopLeft > Top > TopRight (top)

Left > Center > Right (center Y)

BottomLeft > Bottom > BottomRight (bottom)

If at least two of the three horizontal or vertical points are set, the width or height is calculated based on these points, otherwise the width and height from BlzFrameSetSize is used.

If all three are set (for horizontal: left, center, and right; for vertical: top, center, and bottom), the center point is ignored in width/height calculation.

#### GIMLI_2

#### GIMLI_2

 Level 18

JoinedMar 21, 2011Messages1,622

Resources: 3

#### Resources
Maps 2Spells and Systems 1

how can i position a frame in front of another one without making it a child of the other frame?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

GIMLI_2 said:

 how can i position a frame in front of another one without making it a child of the other frame?


Click to expand...

If both Frames are non-SimpleFrames: then you need to find the first shared ancestor and give the starter of this branch/Layer(a child of that shared ancestor) a higher Level than the other starter of the other one.

Or if they both children of the same Parent you can increase the Level of one or create them in a different order.

UI - The concept of Parent-Frames

Tasyen said:

 For Frames:

FrameLevel define the order in which child-Frames are above their Parent. Frames only compete with their siblings. They are ordered descending. The childFrame with the highest Level is onTop. the one with the lowest Level is directly above the parent but below it's brothers and sisters. A Frame's children are below a sibling with a higher level.

For SimpleFrames:

The one with the highest Level is on Top parentship does not matter.


Click to expand...

BlzFrameSetLevel(frame, level) is used to change the Level.

#### Ericqwe

#### Ericqwe

 Level 4

JoinedOct 24, 2021Messages61

How to modify the position of the button of the Missions


 Last edited: May 2, 2022

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

I assume you mean the button to open the questdialog which would be: BlzGetFrameByName("UpperButtonBarQuestsButton",0). Then use this frame for the moving shown in the tutorial snippets.

One can also access all of the QuestDialog.

But general you need to somehow get a access to the framehandle.

Either by knowing its name and number than one uses BlzGetFrameByName(name, createContext)

###
 Default Names for BlzGetFrameByName


Introduction This list was generated by reading/interpreting all fdf files from warcraft 3 ptr 1.31 and reading all words in "". LeaderBoard/Multiboard/TimerDialog do only work when the map created such ones. LogDialog is only used in SinglePlayer. Chat Frames are only used in Multiplayer. Quest...

 www.hiveworkshop.com


or for some there is BlzGetOriginFrame(frameType, index). Origin Frames is a special access to some frames blizzard provided.

###
 UI: OriginFrames


Jump to Hiding Jump to ChildFrames Originframetypes are built in frames managing the default game ui. ORIGIN_FRAME_GAME_UI is our key to generate new container frames. In other tutorials and in many examples, ORIGIN_FRAME_GAME_UI is used as parent for custom created frames. That is done because...

 www.hiveworkshop.com


or moving down/up in the frame family tree using BlzFrameGetParent(frame)/BlzFrameGetChild(frame, index)

###
 The Big UI-Frame Tutorial


 www.hiveworkshop.com


###
 The Big UI-Frame Tutorial


 www.hiveworkshop.com


###
 The Big UI-Frame Tutorial


 www.hiveworkshop.com


The moving part should be explained in this tutorial, but you can ask again if it is unclear, good luck.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Was trying some fdf stuff and found this:

LayerStyle "SETSVIEWPORT",

Purpose: Limits the visuals of the Layer & SubLayers started by this frame to this frames taken space. The children are still interactive, even when not seeable at a spot.

(The buttons are childs in fdf while the paladin backdrop is a map script added child.) (Bearbeitet)


 Last edited: May 15, 2023

#### RPG_F

#### RPG_F

 Level 4

JoinedFeb 24, 2018Messages46

how to set CommandButton parent is another frameUI

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

"CommandBarFrame",0 (Reforged) is a simpleframe, the map script api only allows to choose a simpleframe as a new parent for such.

You choose your other simpleframe and set the new parent and it is done.

V1.31.1 example code makes CommandBar a child of ResourceBar and hide ResourceBar. ResourceBar is just some other default simpleframe. You could use any other simpleframe instead.

 Lua:

BlzFrameSetParent(BlzFrameGetParent(BlzGetOriginFrame(ORIGIN_FRAME_COMMAND_BUTTON, 0)), BlzGetFrameByName("ResourceBarFrame", 0))
BlzFrameSetVisible(BlzGetFrameByName("ResourceBarFrame", 0), false)

Same for Reforged but using the name version as that is less like fail because of cirumstances( with the V1.31.1 apporach it fails when the parent of Command Button 0 is not the CommandBarFrame).

 Lua:

BlzFrameSetParent(BlzGetFrameByName("CommandBarFrame", 0), BlzGetFrameByName("ResourceBarFrame", 0))
BlzFrameSetVisible(BlzGetFrameByName("ResourceBarFrame", 0), false)

#### Wareditor

#### Wareditor

 Level 15

JoinedJan 16, 2009Messages764

Resources: 4

#### Resources
Maps 4

Tasyen said:

 Was trying some fdf stuff and found this:

LayerStyle "SETSVIEWPORT",

Purpose: Limits the visuals of the Layer & SubLayers started by this frame to this frames taken space. The children are still interactive, even when not seeable at a spot.

(The buttons are childs in fdf while the paladin backdrop is a map script added child.) (Bearbeitet)

View attachment 434049

Click to expand...

I just learned about this.

I have been experimenting with it and noticed that the children would exceed by a little the parent's viewport frame on the x-axis.

For instance, if you have a parent viewport with width 0.1 and a centered child on it with width 0.2, the child visual width would be bigger than 0.1.

I found that the viewport actual width depends on the window aspect ratio.

Basically, if want to cut an area of size {size_x, size_y}, set the viewport frame to size {size_x * 3/4 * 16/9 / window_aspect_ratio, size_y}.

#### Wareditor

#### Wareditor

 Level 15

JoinedJan 16, 2009Messages764

Resources: 4

#### Resources
Maps 4

Wareditor said:

 I just learned about this.

I have been experimenting with it and noticed that the children would exceed by a little the parent's viewport frame on the x-axis.

For instance, if you have a parent viewport with width 0.1 and a centered child on it with width 0.2, the child visual width would be bigger than 0.1.

I found that the viewport actual width depends on the window aspect ratio.

Basically, if want to cut an area of size {size_x, size_y}, set the viewport frame to size {size_x * 3/4 * 16/9 / window_aspect_ratio, size_y}.


Click to expand...

Actually a bit more complicated that what I thought.

Backdrops that are children to the viewport can become pretty messed up with different aspect ratios, to the point of becoming hardly usable.

I have given up trying to use this feature for now.

 You must log in or register to reply here.


### Similar threads

 UI - The concept of Parent-Frames


Tasyen

Nov 19, 2019

Scripting Tutorials

Replies20Views11K

Oct 10, 2020
Chaosy

 UI: Frames and Tooltips


Tasyen

Jun 27, 2019

Scripting Tutorials

Replies45Views21K

Oct 27, 2024
RPG_F

 UI: Creating a Cam control


Tasyen

Jun 1, 2019

Scripting Tutorials

Replies24Views10K

Mar 4, 2021
ESTO

 UI: Creating a Bar


Tasyen

Jun 12, 2019

Scripting Tutorials

Replies38Views18K

Oct 2, 2025
KenArok

 UI: Create a TextButton


Tasyen

May 30, 2019

Scripting Tutorials

Replies12Views12K

Jan 11, 2021
Tasyen

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