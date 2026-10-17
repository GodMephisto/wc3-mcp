---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-frameevents-and-frametypes.318309/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Frameevents and Frametypes
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-frameevents-and-frametypes.318309/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

This is the result of tests about Frame-Events and Frame-Levels.

##
 FrameEvents

FRAMEEVENT_CONTROL_CLICK - when activading a Frame either by releasing the left mouse button (the original mouse click has to be inside the Button) or when a Frame has Focus and space or enter/return are pressed. This Event happens before FRAMEEVENT_MOUSE_UP.

FRAMEEVENT_MOUSE_ENTER - the mouse Cursor enters the frame

FRAMEEVENT_MOUSE_LEAVE - the mouse Cursor leaves the frame

FRAMEEVENT_MOUSE_UP - when releasing the left, right or wheel mouse button and the mouse cursor is currently inside the frame

FRAMEEVENT_MOUSE_DOWN - does nothing or no Frame accept it.

FRAMEEVENT_MOUSE_WHEEL - happens when the mouse is over the Frame and the Wheel is rolled. The direction of the rolling can be detected in the Event by checking BlzGetTriggerFrameValue

+120 -> Forward

-120 -> Backwards​
I would check for: bigger 0 or smaller 0

FRAMEEVENT_CHECKBOX_CHECKED - check an unchecked checkbox

FRAMEEVENT_CHECKBOX_UNCHECKED - uncheck a checked checkbox

FRAMEEVENT_EDITBOX_TEXT_CHANGED - remove or add text. By user or Code.

FRAMEEVENT_POPUPMENU_ITEM_CHANGED - user selected an Option in a popupmenu BlzGetTriggerFrameValue tells you which index.

FRAMEEVENT_MOUSE_DOUBLECLICK - does nothing or no Frame accept it.

FRAMEEVENT_SPRITE_ANIM_UPDATE - ?

FRAMEEVENT_SLIDER_VALUE_CHANGED - when altering the value of a slider or scrollbar. By user or Code.

FRAMEEVENT_DIALOG_CANCEL - When activading the cancel button of a Dialog

FRAMEEVENT_DIALOG_ACCEPT - When activading the accept button of a Dialog

FRAMEEVENT_EDITBOX_ENTER - Pressing enter/return when the Frame has Focus.

​

###
 FrameEvent Getters

 JASS:

BlzGetTriggerFrame takes nothing returns framehandle
BlzGetTriggerFrameEvent takes nothing returns frameeventtype
BlzGetTriggerFrameValue takes nothing returns real
BlzGetTriggerFrameText takes nothing returns string

###
 Frame Level

When multiple Frames are at the same space there is an rule that tells who will take the mouse Events and is displayed on top. FrameLevel set with BlzFrameSetLevel manages that, it only matters when Frames colide/overlap on the screen. Only visible Frames matter for the FrameLevel order.

FrameLevel works different for SimpleFrames and Frames.

For Frames:

FrameLevel define the order in which child-Frames are above their Parent. Frames only compete with their siblings. They are ordered descending. The childFrame with the highest Level is onTop. the one with the lowest Level is directly above the parent but below it's brothers and sisters. A Frame's children are below a sibling with a higher level.

For SimpleFrames:

The one with the highest Level is on Top parentship does not matter.

An example:

ButtonA child of GAMEUI Level 2

ButtonB child of GAMEUI Level 3

ButtonC child of GAMEUI Level 1

ButtonAA child of ButtonA Level 1

ButtonAB child of ButtonA Level 5

ButtonBA child of ButtonB Level 3

ButtonBB child of ButtonB Level 1

ButtonBBA child of ButtonBB Level 6

ButtonCA child of ButtonC Level 8

ButtonCB child of ButtonC Level 7​
on top of GAMEUI is ButtonB, he has the highest Level of GAMEUI.

ButtonBA has a higher Level than ButtonBB hence he is on top of ButtonB.

ButtonBBA is a child of ButtonBB, but it's parent ButtonBB is lower then ButtonBA hence he is also lower than ButtonBA.

ButtonCA and ButtonCB have high levels but their parent ButtonC is the lowest in GAMEUI so they are also the lowest.

The onTop Total is ButtonBA.

The Frame ontop also takes all the mouse control.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 local parent = BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0)
 --local parent = BlzGetFrameByName("EscMenuMainPanel", 0)
 local buttonA = BlzCreateFrameByType("GLUETEXTBUTTON", "A", parent, "ScriptDialogButton", 0)
 local buttonB = BlzCreateFrameByType("GLUETEXTBUTTON", "B", parent, "ScriptDialogButton", 0)
 local buttonC = BlzCreateFrameByType("GLUETEXTBUTTON", "C", parent, "ScriptDialogButton", 0)
 local buttonAA = BlzCreateFrameByType("GLUETEXTBUTTON", "AA", buttonA, "ScriptDialogButton", 0)
 local buttonAB = BlzCreateFrameByType("GLUETEXTBUTTON", "AB", buttonA, "ScriptDialogButton", 0)
 local buttonBA = BlzCreateFrameByType("GLUETEXTBUTTON", "BA", buttonB, "ScriptDialogButton", 0)
 local buttonBB = BlzCreateFrameByType("GLUETEXTBUTTON", "BB", buttonB, "ScriptDialogButton", 0)
 local buttonBBA = BlzCreateFrameByType("GLUETEXTBUTTON", "BBA", buttonBB, "ScriptDialogButton", 0)
 local buttonCA = BlzCreateFrameByType("GLUETEXTBUTTON", "CA", buttonC, "ScriptDialogButton", 0)
 local buttonCB = BlzCreateFrameByType("GLUETEXTBUTTON", "CB", buttonC, "ScriptDialogButton", 0)

 BlzFrameSetLevel(buttonA, 2)
 BlzFrameSetLevel(buttonB, 3)
 BlzFrameSetLevel(buttonC, 1)

 BlzFrameSetLevel(buttonAA, 1)
 BlzFrameSetLevel(buttonAB, 5)

 BlzFrameSetLevel(buttonBA, 3)
 BlzFrameSetLevel(buttonBB, 1)
 BlzFrameSetLevel(buttonBBA, 6)

 BlzFrameSetLevel(buttonCB, 8)
 BlzFrameSetLevel(buttonCA, 7)

 BlzFrameSetText(buttonA, "A")
 BlzFrameSetText(buttonB, "B")
 BlzFrameSetText(buttonC, "C")
 BlzFrameSetText(buttonAA, "AA")
 BlzFrameSetText(buttonAB, "AB")
 BlzFrameSetText(buttonBA, "BA")
 BlzFrameSetText(buttonBB, "BB")
 BlzFrameSetText(buttonCA, "CA")
 BlzFrameSetText(buttonCB, "CB")
 BlzFrameSetText(buttonBBA, "BBA")

 BlzFrameSetAbsPoint(buttonC, FRAMEPOINT_CENTER, 0.24, 0.34)
 BlzFrameSetAbsPoint(buttonCA, FRAMEPOINT_CENTER, 0.26, 0.36)
 BlzFrameSetAbsPoint(buttonCB, FRAMEPOINT_CENTER, 0.28, 0.38)
 BlzFrameSetAbsPoint(buttonA, FRAMEPOINT_CENTER, 0.3, 0.4)
 BlzFrameSetAbsPoint(buttonAA, FRAMEPOINT_CENTER, 0.32, 0.42)
 BlzFrameSetAbsPoint(buttonAB, FRAMEPOINT_CENTER, 0.34, 0.44)
 BlzFrameSetAbsPoint(buttonB, FRAMEPOINT_CENTER, 0.36, 0.46)
 BlzFrameSetAbsPoint(buttonBB, FRAMEPOINT_CENTER, 0.38, 0.48)
 BlzFrameSetAbsPoint(buttonBBA, FRAMEPOINT_CENTER, 0.4, 0.5)
 BlzFrameSetAbsPoint(buttonBA, FRAMEPOINT_CENTER, 0.42, 0.52)
 end
end

SIMPLEBUTTON seems to be prioritized even against Frames with a higher Level.

In Warcraft 3 Versions 1.31.1 using BlzFrameSetLevel onto this FrameTypes crashes the game, there is no crash in V1.32.2.

BACKDROP

FRAME

HIGHLIGHT

MODEL

SPRITE

TEXT

TIMERTEXT

A SIMPLEFRAME with a FrameLevel of 2 or higher is displayed over unit lifebars and covers them, when created as child of GAME_UI.​

##
 FrameTypes

Which FrameTypes can have which frameevents?

BACKDROP

None​
BUTTON

CONTROL_CLICK, MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL​
CHATDISPLAY

MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL (CONTROL_CLICK with BlzFrameClick)​
CHECKBOX

MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL, CHECKBOX_CHECKED, CHECKBOX_UNCHECKED (CONTROL_CLICK with BlzFrameClick)​
CONTROL

MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP​
DIALOG

DIALOG_CANCEL, DIALOG_ACCEPT​
EDITBOX

MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL, EDITBOX_TEXT_CHANGED, EDITBOX_ENTER (CONTROL_CLICK with BlzFrameClick)​
FRAME

None, (Blocks previous created Frames of Level 0)​
GLUEBUTTON

CONTROL_CLICK, MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL​
GLUECHECKBOX

MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL, CHECKBOX_CHECKED, CHECKBOX_UNCHECKED (CONTROL_CLICK with BlzFrameClick)​
GLUEEDITBOX

MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL, EDITBOX_TEXT_CHANGED, EDITBOX_ENTER (CONTROL_CLICK with BlzFrameClick)​
GLUEPOPUPMENU

MOUSE_ENTER, MOUSE_LEAVE, MOUSE_WHEEL, CONTROL_CLICK, POPUPMENU_ITEM_CHANGED​
GLUETEXTBUTTON

CONTROL_CLICK, MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL​
HIGHLIGHT

None​
LISTBOX

MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL​
MENU

MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL​
MODEL

MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL (CONTROL_CLICK with BlzFrameClick)

(Only in the screen space taken, this has nothing to do with the visual part)​
POPUPMENU

MOUSE_ENTER, MOUSE_LEAVE, MOUSE_WHEEL, CONTROL_CLICK, POPUPMENU_ITEM_CHANGED​
SCROLLBAR

MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL, SLIDER_VALUE_CHANGED​
SIMPLEBUTTON

CONTROL_CLICK​
SIMPLECHECKBOX

None​
SIMPLEFRAME

None​
SIMPLESTATUSBAR

None​
SLASHCHATBOX

MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL, EDITBOX_TEXT_CHANGED, EDITBOX_ENTER (CONTROL_CLICK with BlzFrameClick)​
SLIDER

MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL, SLIDER_VALUE_CHANGED (CONTROL_CLICK with BlzFrameClick)​
SPRITE

None​
TEXT

CONTROL_CLICK, MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL​
TEXTAREA

MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL​
TEXTBUTTON

CONTROL_CLICK, MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL​
TIMERTEXT

CONTROL_CLICK, MOUSE_ENTER, MOUSE_LEAVE, MOUSE_UP, MOUSE_WHEEL​

Test Code for FrameLevel Tests.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 local frameA = BlzCreateFrame("DebugButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 local frameAChild = BlzCreateFrame("DebugButton", frameA, 0, 0)
 local frameB = BlzCreateFrame("DebugButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 BlzFrameSetAbsPoint(frameA, FRAMEPOINT_CENTER, 0.4, 0.3)
 BlzFrameSetAbsPoint(frameAChild, FRAMEPOINT_CENTER, 0.4, 0.3)
 BlzFrameSetAbsPoint(frameB, FRAMEPOINT_CENTER, 0.4, 0.3)
 BlzFrameSetLevel(frameA, 0)
 BlzFrameSetLevel(frameB, 0)
 BlzFrameSetLevel(frameAChild, 0)
 for index = 1, 16, 1 do
 local trigger = CreateTrigger()
 BlzTriggerRegisterFrameEvent(trigger, frameA, ConvertFrameEventType(index))
 BlzTriggerRegisterFrameEvent(trigger, frameAChild, ConvertFrameEventType(index))
 BlzTriggerRegisterFrameEvent(trigger, frameB, ConvertFrameEventType(index))
 TriggerAddAction(trigger, function()
 if BlzGetTriggerFrame() == frameA then
 print("frameA")
 elseif BlzGetTriggerFrame() == frameB then
 print("frameB")
 else
 print("frameAChild")
 end
 print("Event",index)
 print("TriggerValue", BlzGetTriggerFrameValue())
 print("TriggerText", BlzGetTriggerFrameText())
 end)

 end

 BlzFrameSetText(frameA, "frameA")
 BlzFrameSetText(frameB, "frameB")
 BlzFrameSetText(frameAChild, "frameAChild")
 end
end

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


 Last edited by a moderator: Oct 10, 2020

#### xorkatoss

#### xorkatoss

 Level 20

JoinedJul 12, 2010Messages1,746

Resources: 10

#### Resources
Models 6Maps 1Spells and Systems 3

Hmm I was wondering about something...I guess since these are the only frame events available you can't yet add keyboard events right?

like when you use QWER for abilities etc. it would be pretty awesome if you could create do that >_<

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Blizzard added keyboard events named oskey [JASS/AI] - Oskey - Player Key Event.

There is also a shortcut frame attribute in fdf, but I failed to get it working.

#### xorkatoss

#### xorkatoss

 Level 20

JoinedJul 12, 2010Messages1,746

Resources: 10

#### Resources
Models 6Maps 1Spells and Systems 3

oh thanks I didn't know you had such tutorials

#### bgcy_ro002

#### bgcy_ro002

 Level 5

JoinedSep 6, 2013Messages21

Resources: 5

#### Resources
Icons 1Spells and Systems 2User Interface 2

Hello Tasyen,

I was created a button with 3 frame events (CONTROL_CLICK, MOUSE_ENTER and MOUSE_LEAVE).

But it just only work with frame event CONTRL_CLICK. Event MOUSE ENTER and MOUSE_LEAVE doesn't work.

Could you help me point out my problem? I tried other ways but the 2 events enter/leave still didn't work.

My FDF file:

 Code:

Texture "MySimpleButtonButtonHighlight" {
 File "UI\Widgets\Console\Human\CommandButton\human-activebutton.blp",
 AlphaMode "ADD",
}

Frame "SIMPLEBUTTON" "MySimpleButtonGlowing" {
 Width 0.035,
 Height 0.035,
 UseHighlight "MySimpleButtonButtonHighlight",
 Texture "MySimpleButtonTexture" {
 }
}

My script:

 Code:

function Action_Click takes nothing returns boolean
 call DisplayTextToForce( GetPlayersAll(), "Detected action: Clicked button BTNAcorn" )
 return false
endfunction

function Action_Enter takes nothing returns boolean
 call DisplayTextToForce( GetPlayersAll(), "Detected action: Mouse enter button BTNAcorn" )
 return false
endfunction

function Action_Leave takes nothing returns boolean
 call DisplayTextToForce( GetPlayersAll(), "Detected action: Mouse leave button BTNAcorn" )
 return false
endfunction

function Trig_Simple_Button_Actions takes nothing returns nothing
 local framehandle frame
 local trigger trigger_click
 local trigger trigger_enter
 local trigger trigger_leave

 call BlzLoadTOCFile("war3mapImported\\MySimpleButton.toc")
 call BlzLoadTOCFile("war3mapImported\\MyStatusBar.toc")
 set frame = BlzCreateSimpleFrame("MySimpleButtonGlowing", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0)
 call BlzFrameSetAbsPoint(frame, FRAMEPOINT_BOTTOMLEFT , 8 * 0.80/udg_SCREEN_WIDTH2 , 154 * 0.60/udg_SCREEN_HEIGHT2)
 call BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPRIGHT , 72 * 0.80/udg_SCREEN_WIDTH2 , 218 * 0.60/udg_SCREEN_HEIGHT2)
 call BlzFrameSetTexture(BlzGetFrameByName("MySimpleButtonTexture", 0), "ReplaceableTextures\\CommandButtons\\BTNAcorn.blp", 0, true)

 set trigger_click = CreateTrigger()
 call BlzTriggerRegisterFrameEvent(trigger_click, frame, FRAMEEVENT_CONTROL_CLICK)
 call TriggerAddCondition(trigger_click, Condition( function Action_Click))

 set trigger_enter = CreateTrigger()
 call BlzTriggerRegisterFrameEvent(trigger_enter, frame, FRAMEEVENT_MOUSE_ENTER)
 call TriggerAddCondition(trigger_enter, Condition( function Action_Enter))

 set trigger_leave = CreateTrigger()
 call BlzTriggerRegisterFrameEvent(trigger_leave, frame, FRAMEEVENT_MOUSE_LEAVE)
 call TriggerAddCondition(trigger_leave, Condition( function Action_Leave))

endfunction

//===========================================================================
function InitTrig_Simple_Button takes nothing returns nothing
 set gg_trg_Simple_Button = CreateTrigger( )
 call TriggerRegisterTimerEventSingle( gg_trg_Simple_Button, 0.00 )
 call TriggerAddAction( gg_trg_Simple_Button, function Trig_Simple_Button_Actions )
endfunction

Below is my result:

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Frames of type "SIMPLEBUTTON" can not have this events. You might have to use some other FrameType Like "BUTTON" (which is no SimpleFrame-Type, hence you would have to change the fdf or create one without fdf UI: GLUEBUTTON).

Another option is to use Tooltips and check for Visibility of the Tooltip (which would be async).

#### SebioL

#### SebioL

 Level 4

JoinedMay 19, 2020Messages315

Resources: 3

#### Resources
Reforged HD Models 3

@Tasyen , Forgive me if this question is too ignorant, if it has nothing to do and if it is something impossible to change in the game settings.

But... Using variations in mouse scrolling, would it be possible through the JASS code to modify the variation in mouse scrolling to enlarge or reduce the limit Zoom approach over the map pattern?

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Most of the tutorials in the series are approved now, did the same things I did for the other ones. Approved.

#### Lt_Hawkeye

#### Lt_Hawkeye

 Level 12

JoinedJul 4, 2016Messages636

Is it possible to detect a frame click with the right mouse event or even distinguish between which mouse click was done?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

the frameevent can't. I suggest to add a EVENT_PLAYER_MOUSE_UP trigger. In it you set the last clicked mouse button and then check in the frameevent, if the right button was the last (for that player). Does not work during paused game.

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

It seems 1.36 has broken most of the frameevents.

Mouse Enter and Mouse Leave frame events fire repeatedly, even if the mouse is not moving.

Also, Mouse Down and Mouse Up seem to no longer do anything.

The mouse also twitches around like crazy when these events fire.

#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


I noticed that Mouse Enter and Mouse Leave frame events work properly with this simple frame in UI Utils on the live patch:

 JASS:

Frame "SIMPLEBUTTON" "UIUtilsSimpleButton"
{
 DecorateFileNames,
 // SetPoint TOPLEFT, "UIUtilsSimpleButton", TOPLEFT, 0.002, 0,
 NormalText "UIUtilsTextFormatNormal" "",
 DisabledText "UIUtilsTextFormatDisabled" "",
 HighlightText "UIUtilsTextFormatHighlight" "",
 Height 0.047,
 Width 0.047,
 ButtonPushedTextOffset 0.001 -0.001,
 NormalTexture "UpperMenuButtonBackground",
 PushedTexture "UpperMenuButtonPushedBackground",
 DisabledTexture "UpperMenuButtonDisabledBackground",
 UseHighlight "UpperMenuButtonHighlight",
}

The mouse does not twitch and the game properly registers the mouse leaving and entering the frame. There is no infinite loop. What could be reason?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Oh, I didn't know that Enter & Leave work for simplebutton in V1.36. They did not in V1.31.

simplebutton are quite different from gluebutton. gluebutton can have shortcuts, highlight children, keep input focus, tab focus swapping, belong to a different family, the functional children that perform features like highlight start new layers and can have children, ...

Maybe it is just some bug.

#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


After playing around with frames for a bit, I discovered that mouse enter and leave events also work properly for POPUPMENU on the latest patch.


 Last edited: Sep 13, 2023

#### Donach

#### Donach

 Level 6

JoinedJan 12, 2011Messages110

Resources: 2

#### Resources
Maps 1Tutorials 1

SmitingDevil said:

 After playing around with frames for a bit, I discovered that mouse enter and leave events also work properly for POPUPMENU on the latest patch.


Click to expand...

Not sure if it's just me, but is the SIMPLEBUTTON registering CLick event properly when out of 4:3 screen?

#### Donach

#### Donach

 Level 6

JoinedJan 12, 2011Messages110

Resources: 2

#### Resources
Maps 1Tutorials 1

Zwiebelchen said:

 It seems 1.36 has broken most of the frameevents.

Mouse Enter and Mouse Leave frame events fire repeatedly, even if the mouse is not moving.

Also, Mouse Down and Mouse Up seem to no longer do anything.

The mouse also twitches around like crazy when these events fire.


Click to expand...

I've solved this by wrapping "BUTTON" inside "SIMPLEFRAME". This makes you have to use it in the 4:3 screen though...

Example:

 JASS:

Frame "SIMPLEFRAME" "SimpleHeroInventoryItem" {
 Frame "BUTTON" "HeroInventoryItemButtonFrame" INHERITS "ButtonItemButtonTemplate" {
 ...
 }
}

#### Gaspar

#### Gaspar

 Level 4

JoinedNov 2, 2013Messages62

Donach said:

 I've solved this by wrapping "BUTTON" inside "SIMPLEFRAME". This makes you have to use it in the 4:3 screen though...

Example:

 JASS:

Frame "SIMPLEFRAME" "SimpleHeroInventoryItem" {
 Frame "BUTTON" "HeroInventoryItemButtonFrame" INHERITS "ButtonItemButtonTemplate" {
 ...
 }
}

Click to expand...

How did u implement that? did u got a map test?

 You must log in or register to reply here.


### Similar threads

 UI: Frames and Tooltips


Tasyen

Jun 27, 2019

Scripting Tutorials

Replies45Views21K

Oct 27, 2024
RPG_F

 UI: Create a TextButton


Tasyen

May 30, 2019

Scripting Tutorials

Replies12Views12K

Jan 11, 2021
Tasyen

 UI: Positionate Frames


Tasyen

May 29, 2019

Scripting Tutorials

Replies25Views19K

Feb 11, 2026
Wareditor

 UI - The concept of Parent-Frames


Tasyen

Nov 19, 2019

Scripting Tutorials

Replies20Views11K

Oct 10, 2020
Chaosy

 UI: GLUEBUTTON


Tasyen

Sep 9, 2019

Scripting Tutorials

Replies32Views15K

Nov 16, 2023
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