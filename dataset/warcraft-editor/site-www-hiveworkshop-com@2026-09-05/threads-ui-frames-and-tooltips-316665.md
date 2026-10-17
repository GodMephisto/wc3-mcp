---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-frames-and-tooltips.316665/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Frames and Tooltips
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-frames-and-tooltips.316665/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

### new

### old

##
 ToolTips

In the warcraft 3's UI-framework, Tooltips are also frames. With that they can be more complex have child-Frames and be from various frametypes. Therefore they can display alot of different things.

Most Frames can have a Tooltip, but it has to be from the same Group: frame<->frame (BUTTON - FRAME); simpleframe <-> simpleframe (SIMPLEBUTTON - SIMPLEFRAME). From the SimpleFrame group only SIMPLEBUTTON can have a Tooltip.

This capitel covers Tooltips for the Frame group. While most is the same for both groups, there are small differences.

A Tooltip-Frames is visible, if the mouse does point at the primary Frame. The enable State of a Frame does not alter the Tooltip behaviour but hiding the Frame does.

The frame needs to be able to take control of the mouse on the screen.

It can happen that the Tooltip-Frame itself gets malformed if it leaves the 4:3 Screen, the children of the Tooltip-Frame are unaffected, hence it is suggested to add an empty FRAME parent for the acutal wanted Tooltip-Frame.

One tooltip-frame can serve many as tooltip.

Text is displayed bold/wrongly when it is the tooltip of multiple Frames. Happens also as child of the tooltip.

Tooltips can either be given in map script or fdf.

In fdf ToolTip "FrameName",

Inside mapscript one uses BlzFrameSetTooltip takes framehandle frame, framehandle tooltip returns nothing

Using this native activades the tooltip visible rule. Sadly calling this native does not hide a simpleframe tooltip.

​

###
 Tooltip Examples

Let's create a TextButton with a TEXT Tooltip. This works out of the Box without any custom Toc or custom fdf.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 -- create a new Button which inherits from "ScriptDialogButton"
 local button = BlzCreateFrameByType("GLUETEXTBUTTON", "MyScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "ScriptDialogButton", 0)
 -- place the Button to the center of the Screen
 BlzFrameSetAbsPoint(button, FRAMEPOINT_CENTER, 0.4, 0.3)
 -- set the Button's text
 BlzFrameSetText(button, "My Button Text")

 -- Create a TEXT-Frame
 local tooltipFrame = BlzCreateFrameByType("TEXT", "MyScriptDialogButtonTooltip", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
 -- tooltipFrame becomes button's tooltip
 BlzFrameSetTooltip(button, tooltipFrame)
 -- Place the Tooltip above the Button
 BlzFrameSetPoint(tooltipFrame, FRAMEPOINT_BOTTOM, button, FRAMEPOINT_TOP, 0, 0.01)
 -- Prevent the TEXT from taking mouse control
 BlzFrameSetEnable(tooltipFrame, false)
 BlzFrameSetText(tooltipFrame, "Nothing will Happen when you click this Button.")
 end
end

###
 BoxText tooltip

The last example works but a text in a Box would be better. This is done by creating an BACKDROP the Box and a TEXT Frame as the child of the Box. The Box becomes the tooltip, as Child the Text will share the visibility of it's Parent the box.

For the BACKDROP we use "QuestButtonBaseTemplate".

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 -- create a new Button which inherits from "ScriptDialogButton"
 local button = BlzCreateFrameByType("GLUETEXTBUTTON", "MyScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "ScriptDialogButton", 0)
 -- place the Button to the center of the Screen
 BlzFrameSetAbsPoint(button, FRAMEPOINT_CENTER, 0.4, 0.3)
 -- set the Button's text
 BlzFrameSetText(button, "My Button Text")

 -- Create the Background a Backdrop
 local tooltipFrameBackGround = BlzCreateFrame("QuestButtonBaseTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 BlzFrameSetSize(tooltipFrameBackGround, 0.25, 0.025)
 -- Create the Text as child of the Background
 local tooltipFrameText = BlzCreateFrameByType("TEXT", "MyScriptDialogButtonTooltip", tooltipFrameBackGround, "", 0)
 -- Copy Size and Position with a small offset.
 BlzFrameSetPoint(tooltipFrameText, FRAMEPOINT_BOTTOMLEFT, tooltipFrameBackGround, FRAMEPOINT_BOTTOMLEFT, 0.01, 0.01)
 BlzFrameSetPoint(tooltipFrameText, FRAMEPOINT_TOPRIGHT, tooltipFrameBackGround, FRAMEPOINT_TOPRIGHT, -0.01, -0.01)
 -- The background becomes the button's tooltip, the Text as child of the background will share the visibility
 BlzFrameSetTooltip(button, tooltipFrameBackGround)
 -- Place the Tooltip above the Button
 BlzFrameSetPoint(tooltipFrameBackGround, FRAMEPOINT_BOTTOM, button, FRAMEPOINT_TOP, 0, 0.01)
 -- Prevent the TEXT from taking mouse control
 BlzFrameSetEnable(tooltipFrameText, false)
 BlzFrameSetText(tooltipFrameText, "Nothing will Happen when you click this Button.")
 end
end

​

###
 BoxedText ToolTips Dynamic amount of Lines

The previous example is okay, but there is something quite a hassle about it, one has to set the size of the box. it would be much better when the box fits itself to the text.

This can be done, Instead of making the text fit into the box one makes the Box extend to the text's postion. The game even supports one in handling multilines with using 0 as Height for the TEXT-Frame. But one has to now care that the box is relative to the text means one has to place the text with the box's additional size.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 -- create a new Button which inherits from "ScriptDialogButton"
 local button = BlzCreateFrameByType("GLUETEXTBUTTON", "MyScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "ScriptDialogButton", 0)
 -- place the Button to the center of the Screen
 BlzFrameSetAbsPoint(button, FRAMEPOINT_CENTER, 0.4, 0.3)
 -- set the Button's text
 BlzFrameSetText(button, "My Button Text")

 -- Create the Background a Backdrop
 local tooltipFrameBackGround = BlzCreateFrame("QuestButtonBaseTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)

 -- Create the Text as child of the Background
 local tooltipFrameText = BlzCreateFrameByType("TEXT", "MyScriptDialogButtonTooltip", tooltipFrameBackGround, "", 0)
 -- the text has a Width of 0.25, and starts a new line when the given text is longer
 BlzFrameSetSize(tooltipFrameText, 0.25, 0)
 -- Copy Size and Position with a small offset.
 BlzFrameSetPoint(tooltipFrameBackGround, FRAMEPOINT_BOTTOMLEFT, tooltipFrameText, FRAMEPOINT_BOTTOMLEFT, -0.01, -0.01)
 BlzFrameSetPoint(tooltipFrameBackGround, FRAMEPOINT_TOPRIGHT, tooltipFrameText, FRAMEPOINT_TOPRIGHT, 0.01, 0.01)
 -- The background becomes the button's tooltip, the Text as child of the background will share the visibility
 BlzFrameSetTooltip(button, tooltipFrameBackGround)
 -- Place the Tooltip above the Button
 BlzFrameSetPoint(tooltipFrameText, FRAMEPOINT_BOTTOM, button, FRAMEPOINT_TOP, 0, 0.01)
 -- Prevent the TEXT from taking mouse control
 BlzFrameSetEnable(tooltipFrameText, false)
 BlzFrameSetText(tooltipFrameText, "Nothing will Happen when you click this Button. But here a longer text. The only son of King Terenas, Arthas is an idealistic, yet somewhat rash, young man who dreams of one day succeeding his father as King of Lordaeron. Arthas became an apprentice paladin at nineteen and has served as a favorite pupil of Uther the Lightbringer ever since. Though Arthas loves the kindly Uther as an uncle, he longs to take command of his own destiny and become a hero like those brave veterans who fought the orcs during the Second War. ")
 end
end

###
 SimpleFrame ToolTip

a working example of an SimpleFrameTooltip. It creates a "SIMPLEBUTTON" based on a Blueprint from fdf and a code driven "SIMPLESTATUSBAR". The "SIMPLESTATUSBAR" is used as icon and tooltip.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 local simpleFrame = BlzCreateSimpleFrame("UpperButtonBarButtonTemplate", BlzGetFrameByName("ConsoleUI", 0), 0)
 BlzFrameSetAbsPoint(simpleFrame, FRAMEPOINT_CENTER, 0.4, 0.3)

 local icon = BlzCreateFrameByType("SIMPLESTATUSBAR", "", BlzGetFrameByName("ConsoleUI", 0), "", 0)
 BlzFrameClearAllPoints(icon)
 BlzFrameSetPoint(icon, FRAMEPOINT_BOTTOM, simpleFrame, FRAMEPOINT_TOP, 0, 0.006)
 BlzFrameSetSize(icon, 0.04, 0.04)
 BlzFrameSetTexture(icon, "ReplaceableTextures\\CommandButtons\\BTNPriest", 0, false)
 BlzFrameSetValue(icon, 100)
 BlzFrameSetTooltip(simpleFrame, icon)
 -- SimpleFrameTooltip is not hidden by calling BlzFrameSetTooltip, hide it
 BlzFrameSetVisible(icon, false)

 end
end

Many blizzard's made BACKDROPs can be used. One could look here UI: List - Default MainFrames.

BlzFrameIsVisible(frame) also returns the correct value for frames working as tooltips, This is async hence quite fast but also dangerous. It could be used to perform an IsFrame hovered without enter/Leaver events (which are sending network packages). When doing that one creates an empty "FRAME" and defines it as Tooltip sets up a way to know which tooltip and Frame belong together and periodicly check for the visiblity of the Tooltip "FRAME", if it is visible and not the last hovered one you have the async enter event.

Undoing the tooltip state is problematic.

​

##
 Introduction

In the UI-frame natives, tooltips are frames that are on default hidden. When one hovers the frame the tooltip is assigned to, the tooltip becomes visible. Tooltips itself are also frames that can have childs and be customized.

That is the native one uses to make a tooltip-Frame become some frames tooltip.

BlzFrameSetTooltip takes framehandle frame, framehandle tooltip returns nothing

##
 SimpleFrames and Tooltips

SimpleFrames can normally not have tooltips and they can also not be a tooltip for Frames. A SIMPLEBUTTON can have other SimpleFrames as tooltip.

One can give a SimpleFrame a tooltip by creating a substitute FRAME which copies all points of the simpleframe. That substitute only handles the tooltip and is empty. When one hovers the SimpleFrame the substitute FRAME kicks in and shows its tooltip giving the feeling that it is the SimpleFrames tooltip. That way also SimpleFrames or BACKDROPs can have tooltips.

##
 Example 1

In example 1, we create 3 Frames: a BACKDROP, a TEXT and a FRAME. The BACKDROP shows a Paladin icon. The FRAME copies the points of the BACKDROP and handles the Tooltip showing. As last the TEXT-Frame is the tooltip itself which is shown when one hovers the Paladin icon with the mouse.

The FRAME is needed in this example because BACKDROPs can not have events nor tooltips. FRAME can't have events either but can have a tooltip.

In this example all frames are created with BlzCreateFrameByType, that is done so one does not have to bother fdf for this example.

BlzCreateFrameByType takes string typeName, string name, framehandle owner, string inherits, integer createContext

 Lua:

function Face()
 local face = BlzCreateFrameByType("BACKDROP", "Face", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)--Create a new frame of Type BACKDROP
 local faceHover = BlzCreateFrameByType("FRAME", "FaceFrame", face,"", 0) --face is a BACKDROP it can not have events nor a tooltip, thats why one creates an empty frame managing that.
 local tooltip = BlzCreateFrameByType("TEXT", "FaceFrameTooltip", face,"", 0)--Create a new frame of Type TEXT
 --faceHover would be unneeded if face would support events/tooltip

 BlzFrameSetAllPoints(faceHover, face) --faceHover copies the size and position of face.
 BlzFrameSetTooltip(faceHover, tooltip) --when faceHover is hovered with the mouse frame tooltip becomes visible.

 BlzFrameSetSize(face, 0.04, 0.04)
 BlzFrameSetAbsPoint(face, FRAMEPOINT_CENTER, 0.4, 0.3)
 BlzFrameSetAbsPoint(tooltip, FRAMEPOINT_CENTER, 0.2, 0.3)
 BlzFrameSetText(tooltip, "Human Paladin Face")

 BlzFrameSetTexture(face, "ReplaceableTextures\\CommandButtons\\BTNHeroPaladin",0, true)--face uses paladin blp as texture.
end

That code has to be executed. It does not run by itself.

Start
Events
Map initialization

Conditions

Actions
Custom script: Face()

##
 Example 2

The tooltip in the previous example is not really good looking, it's just some Text on the screen. I want it to be in a box and look similar to the default tooltip. Well because such a frame does not exist in the default fdf and one can not manipulate the shown text of the default tooltip frame (as far as I know), one has to create it.

That will be our Boxed Text fdf. "BoxedText" is the frame we create. What can that "BoxedText" do? It mimics the default Tooltip-Box and has 2 children: "BoxedTextTitle" and "BoxedTextValue". The title is shown in the first line and below that title BoxedTextValue will be placed. BoxedTextValue has a smaller Text size than BoxedTextTitle.

 Code:

// -- LOCAL TEMPLATES -------------------------------------------------------
Frame "BACKDROP" "BoxedTextBackgroundTemplate" {
 DecorateFileNames, //Look-Up Names in some String table (for example gameinterface)
 BackdropTileBackground, //Tile mode enabled
 BackdropBackground "ToolTipBackground", //BackgroundFile
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.008, //higher numbers make the corners bigger.
 BackdropBackgroundInsets 0.0022 0.0022 0.0022 0.0022, //makes the background smaller, from the outside.
 BackdropEdgeFile "ToolTipBorder", //the border File
 BackdropBlendAll,
}
// -- Frames -------------------------------------------------------
Frame "BACKDROP" "BoxedText" INHERITS "BoxedTextBackgroundTemplate" {
 UseActiveContext,

 Frame "TEXT" "BoxedTextTitle" {
 UseActiveContext,
 DecorateFileNames,
 SetPoint TOPLEFT, "BoxedText", TOPLEFT, 0.005, -0.005, //Positionate "BoxedTextSimpleTitle"'s TOPLEFT to "BoxedText"'s TOPLEFT with an offset
 SetPoint TOPRIGHT, "BoxedText", TOPRIGHT, -0.005, -0.005,
 FontFlags "FIXEDSIZE",
 FrameFont "MasterFont", 0.014, "",
 FontColor 1.0 1.0 1.0 1.0, //Red Green Blue Alpha 0.0 to 1.0
 FontShadowColor 0.0 0.0 0.0 0.9,
 FontShadowOffset 0.001 -0.001,
 }

 Frame "TEXT" "BoxedTextValue" {
 UseActiveContext,
 DecorateFileNames,
 SetPoint TOPLEFT, "BoxedText", TOPLEFT, 0.005, -0.02,
 SetPoint BOTTOMRIGHT, "BoxedText", BOTTOMRIGHT, -0.005, 0.005,
 FontFlags "FIXEDSIZE",
 FrameFont "MasterFont", 0.012, "",
 FontColor 1.0 1.0 1.0 1.0,
 FontShadowColor 0.0 0.0 0.0 0.9,
 FontShadowOffset 0.001 -0.001,
 }
}

Now we need to change the code a bit. Instead of TEXT, we now create a Frame of name "BoxedText". Also the tooltip is not a TEXT anymore, it's now a box, means we have to access the children of the box showing the text. They are BoxedTextValue and BoxedTextTitle.

 Lua:

function Face2()
 BlzLoadTOCFile("war3mapimported\\BoxedText.toc")
 local face = BlzCreateFrameByType("BACKDROP", "Face", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)--Create a new frame of Type BACKDROP
 local faceHover = BlzCreateFrameByType("FRAME", "FaceFrame", face,"", 0) --face is a BACKDROP it can not have events nor a tooltip, thats why one creates an empty frame managing that.
 local tooltip = BlzCreateFrame("BoxedText", face, 0, 0)--Create the BoxedText Frame
 --faceHover would be unneeded if face would support events/tooltip

 BlzFrameSetAllPoints(faceHover, face) --faceHover copies the size and position of face.
 BlzFrameSetTooltip(faceHover, tooltip) --when faceHover is hovered with the mouse frame tooltip becomes visible.

 BlzFrameSetSize(face, 0.04, 0.04)
 BlzFrameSetAbsPoint(face, FRAMEPOINT_CENTER, 0.4, 0.3)
 BlzFrameSetAbsPoint(tooltip, FRAMEPOINT_CENTER, 0.2, 0.3)
 BlzFrameSetSize(tooltip, 0.15, 0.08)

 BlzFrameSetText(BlzGetFrameByName("BoxedTextValue",0), "Human Paladin Face, but it is not uther.")--BoxedText has a child showing the text, set that childs Text.
 BlzFrameSetText(BlzGetFrameByName("BoxedTextTitle",0), "Paladin")--BoxedText has a child showing the Title-text, set that childs Text.

 BlzFrameSetTexture(face, "ReplaceableTextures\\CommandButtons\\BTNHeroPaladin",0, true)--face uses paladin blp as texture.
end

That code does not execute itself, one has to execute it.

Start
Events
Map initialization

Conditions

Actions
Custom script: Face2()

That is nice, Text in a box, also similar to the default one.

Instead of creating such a BoxedText one could use a TEXTAREA. TEXTAREA does most of BoxedText, but it has a scrollbar blocking a part of the right side and it requires the box to have a min size (else it crashes the game / the thread). Also it becomes a bit tricky with the title.

ChangeLog: Added <UseActiveContext,> to the children in fdf. Without it, the tooltip text inside the box would work wrong for createcontext not be 0.

Added the version from the big UI tutorial the old is still there in a tab

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

Tooltip 3.w3x

 18 KB

 · Views: 888




 Last edited: Feb 23, 2023

#### Coldgamer

#### Coldgamer

 Level 4

JoinedJun 4, 2019Messages37

Thanks for your tutorial, could you mabye make a JASS example? Can't convert my map to lua

#### TriggerHappy

#### TriggerHappy

 Level 40

JoinedJun 23, 2007Messages4,065

Resources: 24

#### Resources
Tools 1Spells and Systems 11JASS 10Tutorials 2

Coldgamer said:

 Thanks for your tutorial, could you mabye make a JASS example? Can't convert my map to lua

Click to expand...

The code is almost identical to JASS. Just add a couple keywords and you're good to go.

#### Coldgamer

#### Coldgamer

 Level 4

JoinedJun 4, 2019Messages37

TriggerHappy said:

 The code is almost identical to JASS. Just add a couple keywords and you're good to go.


Click to expand...

I've already tried it:

 Code:

function face2 takes nothing returns nothing
 call LoadToc("war3mapimported\\BoxedText.toc")
 local framehandle face = BlzCreateFrameByType("BACKDROP", "Face", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
 local framehandle faceHover = BlzCreateFrameByType("FRAME", "FaceFrame", face,"", 0)
 local framehandle tooltip = BlzCreateFrame("BoxedText", face, 0, 0)

 call BlzFrameSetAllPoints(faceHover, face)
 call BlzFrameSetTooltip(faceHover, tooltip)

 call BlzFrameSetSize(face, 0.04, 0.04)
 call BlzFrameSetAbsPoint(face, FRAMEPOINT_CENTER, 0.4, 0.3)
 call BlzFrameSetAbsPoint(tooltip, FRAMEPOINT_CENTER, 0.2, 0.3)
 call BlzFrameSetPoint(tooltip, FRAMEPOINT_BOTTOM, face, FRAMEPOINT_TOP, 0.0, 0.0)
 call BlzFrameSetSize(tooltip, 0.15, 0.08)

 call BlzFrameSetText(BlzGetFrameByName("BoxedTextValue", 0), "Human Paladin Face, but it is not uther.")
 call BlzFrameSetText(BlzGetFrameByName("BoxedTextTitle", 0), "Paladin")

 call BlzFrameSetTexture(face, "ReplaceableTextures\\CommandButtons\\BTNHeroPaladin",0, true)
endfunction

But the JASS-Compiler throws an error relatively early:


 Last edited: Jul 4, 2019

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

in jass local variables have to be created as 1.actions of a function. LoadToc is no local and is done before locals are created, making the locals the 2.Actions which is not allowed.

Do the toc loading in an seperate funtion which executes before that one runs. Or use the vanila one BlzLoadTOCFile and save that in a local boolean.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Very useful tutorial! Thanks for writing this up. Approved!

#### xorkatoss

#### xorkatoss

 Level 20

JoinedJul 12, 2010Messages1,746

Resources: 10

#### Resources
Models 6Maps 1Spells and Systems 3

@Coldgamer no fear xorkatoss is near!

 JASS:

function CreateUther takes nothing returns nothing
 local framehandle face = BlzCreateFrameByType("BACKDROP", "Face", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
 local framehandle faceHover = BlzCreateFrameByType("FRAME", "FaceFrame", face,"", 0)
 local framehandle tooltip = BlzCreateFrame("BoxedText", face, 0, 0)
 call BlzFrameSetAllPoints(faceHover, face)
 call BlzFrameSetTooltip(faceHover, tooltip)

 call BlzFrameSetSize(face, 0.04, 0.04)
 call BlzFrameSetAbsPoint(face, FRAMEPOINT_CENTER, 0.4, 0.3)
// call BlzFrameSetAbsPoint(tooltip, FRAMEPOINT_CENTER, 0.2, 0.3)
 call BlzFrameSetPoint(tooltip, FRAMEPOINT_BOTTOM, face, FRAMEPOINT_TOP, 0.0, 0.0)
 call BlzFrameSetSize(tooltip, 0.15, 0.08)
 call BlzFrameSetText(BlzGetFrameByName("BoxedTextValue", 0), "Human Paladin Face, but it is not uther.")
 call BlzFrameSetText(BlzGetFrameByName("BoxedTextTitle", 0), "Paladin")

 call BlzFrameSetTexture(face, "ReplaceableTextures\\CommandButtons\\BTNHeroPaladin",0, true)
endfunction
//===========================================================================
function InitTrig_Uther takes nothing returns nothing
 set gg_trg_Uther = CreateTrigger( )
 call TriggerRegisterTimerEventSingle( gg_trg_Uther, 1.00 )
 call TriggerAddAction( gg_trg_Uther, function CreateUther)
 call BlzLoadTOCFile("war3mapimported\\BoxedText.toc")
endfunction

Who would have though jass was so easy?

I had to convert this for me because lua is so inconvenient, having all your code in the map header is a HUGE mess, I mean who even though creating lua was gonna be useful? am I right? lol

anyway so I thought why not share it here to help some people, enjoy.

#### Attachments

Tooltip jass.w3x

 8.8 KB

 · Views: 612



#### Uncle

#### Uncle

Warcraft Moderator

 Level 76

JoinedAug 10, 2018Messages8,351

Resources: 1

#### Resources
Spells and Systems 1

Still a bit confused on how to make use of this one.

I want 20 different frames of "BoxedText" for my Inventory System but how would I go about doing so? If I use BlzFrameSetText(BlzGetFrameByName("BoxedTextValue",0) how do I reference a specific frame?

Sorry if this is unclear or a stupid question. Anyway, here's an idea of what I'm trying to make work, I cropped out the Button creation and other stuff so hopefully it will still make sense:

 Lua:

function CreateTooltip(player)
 --[tooltipframe] is a unique Integer that I change each time I run CreateTooltip(). So each frame will have it's own unique index.
 InventorySlots[player][tooltipframe] = BlzCreateFrame("BoxedText", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 1, 0)
 BlzFrameSetAbsPoint(InventorySlots[player][tooltipframe], FRAMEPOINT_CENTER, 0.4, 0.3)
 BlzFrameSetSize(InventorySlots[player][tooltipframe], 0.15, 0.12)
 BlzFrameSetVisible(InventorySlots[player][tooltipframe], false)
 MouseEnter(player)
end

 Lua:

function MouseEnter(player)
 local trigger = CreateTrigger()
 BlzTriggerRegisterFrameEvent(trigger, InventorySlots[player][buttonframe], FRAMEEVENT_MOUSE_ENTER)
 TriggerAddAction(trigger, function()
 BlzFrameSetVisible(InventorySlots[player][tooltipframe], true)
 BlzFrameSetText(BlzGetFrameByName("BoxedTextValue",0), InventorySlots[player][itemdescription])
 --InventorySlots[player][itemdescription] will be a unique String associated with this specific Inventory Slot
 end)
end

With this setup, only the last created Tooltipframe will display any text. So how do I reference specific Frames using: BlzFrameSetText(BlzGetFrameByName("BoxedTextValue",0), "some string"

I had this working before using:









 Lua:

--Instead of "BoxedText" Frame I used:
InventorySlots[player][tooltipframe] = BlzCreateFrame("EscMenuTextAreaTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 1, 0)

--Instead of GetFrameByName I used:
BlzFrameSetText(InventorySlots[player][tooltipframe], InventorySlots[player][itemdescription])


 Last edited: Aug 23, 2019

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

This BoxedText Frame has 2 TEXT child-Frames: "BoxedTextTitle" and "BoxedTextValue". When creating "BoxedText" the child Frames are also created and get the same createcontext number, that is the last argument in the create function. Each of the now created Frames takes a Slot in an inner storage. When using BlzGetFrameByName one reads that inner storage and gets the Frame saved with that Name at that index/createcontext. In case one wants to Access Frames with BlzGetFrameByName one has to avoid losing Frames hence unique names/createcontexts.

But that is not nessacary when saving all later edited Frames in variables. After creating the mainframe "BoxedText", you use BlzGetFrameByName and also save the child Frames in variables then you can access them later using the variables, without having to care about createcontext or framenames.

 Lua:

--Create the mainframe "BoxedText"
local tooltip = BlzCreateFrame("BoxedText", parent, 0, 0)
--Get the childFrames and save them in variables
local tooltipValue = BlzGetFrameByName("BoxedTextValue",0)
local tooltipTitle = BlzGetFrameByName("BoxedTextTitle",0)
--Set the text of the childFrames
BlzFrameSetText(tooltipValue, "Super Long Text")
BlzFrameSetText(tooltipTitle, "Title")

Uncle said:

 I had this working using: InventorySlots[player][tooltipframe] = BlzCreateFrame("EscMenuTextAreaTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 1, 0) instead of the "BoxedText" frame.


Click to expand...

That is cause a TEXTAREA is all in one BACKDROP, SCROLLBAR and TEXT.

One can also set the text of a child Frame if the child is setuped as for example as ButtonText in the fdf as seen in "ScriptDialogButton".


 Last edited: Aug 23, 2019

#### Uncle

#### Uncle

Warcraft Moderator

 Level 76

JoinedAug 10, 2018Messages8,351

Resources: 1

#### Resources
Spells and Systems 1

Excellent, I got it working. Much appreciated!

#### Rahko

#### Rahko

 Level 9

JoinedJul 30, 2018Messages445

Resources: 2

#### Resources
Spells and Systems 2

Great tutorial, as always!

Anyway, I now have two questions:

First - The tooltip apparently makes a button un-clickable. Do you know a workaround for this? Is there a way to make the frames "transparent" so it would recognize mouseover and clicking through the different frames?

Second - Is it possible to make the background on the tooltip itself a bit more transparent? I moved it to the right corner above the command card and replicated the size and everything of the default tooltip, so now it kinda sticks out, that the background is not as transparent. I checked out fdf file, but there's nothing about the background's color.


 Last edited: Dec 15, 2019

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Sabe said:

 First - The tooltip apparently makes a button un-clickable. Do you know a workaround for this? Is there a way to make the frames "transparent" so it would recognize mouseover and clicking through the different frames?


Click to expand...

When having a button you can skip the hoverFrame then the button has the Tooltip directly. The text frames of the tooltip fight for mouse control, if you disable them with BlzFrameSetEnable(frame, false) then they won't anymore. One can also disable the mouse control for an frame in fdf by adding LayerStyle "IGNORETRACKEVENTS", to the frames capable of mouse events

Sabe said:

 Second - Is it possible to make the background on the tooltip itself a bit more transparent? I moved it to the right corner above the command card and replicated the size and everything of the default tooltip, so now it kinda sticks out, that the background is not as transparent. I checked out fdf file, but there's nothing about the background's color.


Click to expand...

There is BlzFrameSetAlpha(frame, 0 to 255) codewise to make a frame less visible. It will affect child-Frames as well.

#### Rahko

#### Rahko

 Level 9

JoinedJul 30, 2018Messages445

Resources: 2

#### Resources
Spells and Systems 2

Tasyen said:

 When having a button you can skip the hoverFrame then the button has the Tooltip directly.


Click to expand...

Right, of course! Thanks a lot for the help again!

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

I was wondering if there is a way to (semi) automatically resize a frame based on the amount of text in it.

I am currently using a frame definition that is very similar to your example in this tutorial:

 Lua:

Frame "BACKDROP" "Tooltip" {
 DecorateFileNames,
 BackdropTileBackground,
 BackdropBackground "ToolTipBackground",
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.008,
 BackdropBackgroundInsets 0.0022 0.0022 0.0022 0.0022,
 BackdropEdgeFile "ToolTipBorder",
 BackdropBlendAll,
 UseActiveContext,
 Frame "TEXT" "TooltipTitle" {
 UseActiveContext,
 DecorateFileNames,
 SetPoint TOPLEFT, "Tooltip", TOPLEFT, 0.0043, -0.005,
 SetPoint TOPRIGHT, "Tooltip", TOPRIGHT, -0.0043, -0.005,
 FontFlags "FIXEDSIZE",
 FrameFont "MasterFont", 0.011, "",
 FontColor 1.0 1.0 1.0 1.0,
 FontShadowColor 0.0 0.0 0.0 0.9,
 FontShadowOffset 0.001 -0.001,
 }
 Frame "TEXT" "TooltipText" {
 UseActiveContext,
 DecorateFileNames,
 SetPoint TOPLEFT, "Tooltip", TOPLEFT, 0.0043, -0.0275,
 SetPoint BOTTOMRIGHT, "Tooltip", BOTTOMRIGHT, -0.0043, -0.0275,
 FontFlags "FIXEDSIZE",
 FrameFont "MasterFont", 0.011, "",
 FontColor 1.0 1.0 1.0 1.0,
 FontShadowColor 0.0 0.0 0.0 0.9,
 FontShadowOffset 0.001 -0.001,
 }
}

Then I run the following code to create the frame, give it a size and position, and add text;

 Lua:

local f = BlzCreateFrame("Tooltip", f, 0, 0)
BlzFrameSetAbsPoint(f, FRAMEPOINT_BOTTOMRIGHT, 0.7992, 0.063)
BlzFrameSetSize(f, 0.2884, 0.109)
local f2 = BlzGetFrameByName("TooltipText", 0)
BlzFrameSetText(f2, BlzGetAbilityResearchExtendedTooltip(skill, 0))

In this example I aim to make a tooltip with variable text, containing Skill information.

After creation, I can see that the text that is displayed in "TooltipText" respects the width that I have set for the highest level frame "Tooltip".

However, if there is a lot of text, it might overflow the backdrop frame.

Perhaps there is a keyword that automatically updates the size of the actual text frame, which I can then use to resize the backdrop?

Otherwise I would have to make a function that counts characters and linebreaks and then adjusts the frame size accordingly.


 Last edited: Aug 31, 2020

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

pr114 said:

 I was wondering if there is a way to (semi) automatically resize a frame based on the amount of text in it.


Click to expand...

One can have a variable Height by using 0 inside BlzFrameSetSize(frame, x, 0) (or not mentioning it in fdf) then it is autocalced, but you still need a vaild Width.

With no Height it scales to the bottom/Top, depends which point you placed the Text-Frame with.

If the Text manges the size one has to revert the relative position. Then the text is abs posed but the backdrop's position depends on the text.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 local backDrop = BlzCreateFrame("QuestButtonBaseTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 local text = BlzCreateFrameByType("TEXT", "TestText", backDrop, "", 0)
 BlzFrameSetAbsPoint(text, FRAMEPOINT_TOPRIGHT, 0.8, 0.55)
 BlzFrameSetSize(text, 0.35, 0)

 BlzFrameSetPoint(backDrop, FRAMEPOINT_TOPLEFT, text, FRAMEPOINT_TOPLEFT, -0.01, 0.01)
 BlzFrameSetPoint(backDrop, FRAMEPOINT_BOTTOMRIGHT, text, FRAMEPOINT_BOTTOMRIGHT, 0.005, -0.01)
 BlzFrameSetText(text, BlzGetAbilityExtendedTooltip(FourCC("AHfs"), 0))

 backDrop = BlzCreateFrame("QuestButtonBaseTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 text = BlzCreateFrameByType("TEXT", "TestText", backDrop, "", 0)
 BlzFrameSetAbsPoint(text, FRAMEPOINT_TOPRIGHT, 0.42, 0.55)
 BlzFrameSetSize(text, 0.2, 0)

 BlzFrameSetPoint(backDrop, FRAMEPOINT_TOPLEFT, text, FRAMEPOINT_TOPLEFT, -0.01, 0.01)
 BlzFrameSetPoint(backDrop, FRAMEPOINT_BOTTOMRIGHT, text, FRAMEPOINT_BOTTOMRIGHT, 0.01, -0.01)
 BlzFrameSetText(text, BlzGetAbilityTooltip(FourCC("AHfs"), 0))
 end
end

Edited: Replaced selfexecution with a less problematic approach


 Last edited: Sep 22, 2020

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Thanks, works great!

#### ESTO

#### ESTO

 Level 3

JoinedApr 17, 2017Messages29

Seems to be bugged when you create a custom buttom in the command grid, the tooltip won't show up. To check if my code is wrong i moved the button around a bit and as soon as it was out of the command grid the tooltip showed up again when the mouse is hovered over my button.

Below a link to the screenshot I made to illustrate what I mean. My custom button resides next to "Patrol" and the tooltip is not working there.

https://cdn.discordapp.com/attachments/311662737015046144/765804950566731776/unknown.png

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

The mouseinteraction prioritizes SimpleFrames. There is a SimpleFrame behind the commandbar which controls the mouse .

Even though it looks like the button is above, for the mouse interaction it is not.

Mouse interaction in one group of Frames (Simple)Frame is rule by Level/Parentship

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Setting a Frame as Tooltip can break it's capability to leave the 4:3 Screen. It is suggested to avoid such a case by adding an empty Parent FRAME for the tooltip which becomes internal the Tooltip for the game.

Using BlzFrameSetTooltip twice with the same frame pair, crashes the game when the Frame is hovered.

BlzFrameSetTooltip(frame, tooltip)

BlzFrameSetTooltip(frame, tooltip)


 Last edited: Nov 23, 2020

#### GIMLI_2

#### GIMLI_2

 Level 18

JoinedMar 21, 2011Messages1,622

Resources: 3

#### Resources
Maps 2Spells and Systems 1

How is it possible to hide a tooltip?

If you hover over the frame that has the tooltip attached, it seems like the tooltip automatically gets visible and 100% alpha.

I thought about nulling the tooltip and adding it back later, but that threw me an error.

I also thought about switching to an invisible dummy frame as a tooltip, but that threw me an error aswell. It seems like you cannot swap a tooltip?

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

If BlzFrameSetTooltip on the hoverframe to a dummy frame doesn't work, try this:

-Set visibility of the tooltip frame to false

-Set position of the tooltip frame to outside the visible area

#### GIMLI_2

#### GIMLI_2

 Level 18

JoinedMar 21, 2011Messages1,622

Resources: 3

#### Resources
Maps 2Spells and Systems 1

pr114 said:

 -Set position of the tooltip frame to outside the visible area


Click to expand...

Does that mean outside the 4:3 area? or set negative position values?

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

I mean set negative. What I usually do that works is 0, -.1.

#### GIMLI_2

#### GIMLI_2

 Level 18

JoinedMar 21, 2011Messages1,622

Resources: 3

#### Resources
Maps 2Spells and Systems 1

doesnt work :/ it buggs out the tooltip background, but it won't disappear.

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Or try this: BlzFrameSetEnable to false on the hover frame.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

One can create a "dummy" Frame and create the Tooltip content as child of that "dummy". That way one can use BlzFrameSetVisible onto the content to show/hide it while the game still manages the hover by visibility, but the content is only shown if you want it. That way one can also create more complex tooltips.

Edit: I also suggest to do that, if the Tooltip is placed outside of the 4:3 Screen. The game did enforce the Tooltip into 4:3 while it's children were free.

 Lua:

do
 local real = InitBlizzard
 function InitBlizzard()
 real()

local frame = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 local tool = BlzCreateFrameByType("FRAME", "", frame, "", 0)
 local content = BlzCreateFrameByType("TEXT", "", tool, "", 0)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.4, 0.3)
 BlzFrameSetAbsPoint(content, FRAMEPOINT_TOP, 0.4, 0.24)
 BlzFrameSetText(content, "Some Text|nit is great")
 BlzFrameSetTooltip(frame, tool)

 TimerStart(CreateTimer(), 1, true, function()
 BlzFrameSetVisible(content, not BlzFrameIsVisible(content))
 end)

end
end


 Last edited: Feb 9, 2021

#### GIMLI_2

#### GIMLI_2

 Level 18

JoinedMar 21, 2011Messages1,622

Resources: 3

#### Resources
Maps 2Spells and Systems 1

Is there a way to format a text box, so the text is right-justified?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Warcraft 3 Text UI text supports various aligns.

BlzFrameSetTextAlignment takes framehandle frame, textaligntype vert, textaligntype horz

Though TextAlignment only works if you give that text either: a position and a Size or 2 positions.

vertical

TEXT_JUSTIFY_TOP

TEXT_JUSTIFY_MIDDLE

TEXT_JUSTIFY_BOTTOM

horizontal

TEXT_JUSTIFY_LEFT

TEXT_JUSTIFY_CENTER

TEXT_JUSTIFY_RIGHT

TEXT_JUSTIFY_TOP, TEXT_JUSTIFY_LEFT

TEXT_JUSTIFY_TOP, TEXT_JUSTIFY_CENTER

TEXT_JUSTIFY_TOP, TEXT_JUSTIFY_RIGHT

TEXT_JUSTIFY_MIDDLE, TEXT_JUSTIFY_LEFT

TEXT_JUSTIFY_MIDDLE, TEXT_JUSTIFY_CENTER

TEXT_JUSTIFY_MIDDLE, TEXT_JUSTIFY_RIGHT

TEXT_JUSTIFY_BOTTOM , TEXT_JUSTIFY_LEFT

TEXT_JUSTIFY_BOTTOM , TEXT_JUSTIFY_CENTER

TEXT_JUSTIFY_BOTTOM , TEXT_JUSTIFY_RIGHT

Can also be done in fdf:

 Code:

 FontJustificationH
 Format1: FontJustificationH JUSTIFYLEFT,
 Format2: FontJustificationH JUSTIFYCENTER,
 Format3: FontJustificationH JUSTIFYRIGHT,
 Purpose: When an TEXT frame is given a rect, it will pos the displayed text x wise at that TextJustify

 FontJustificationV
 Format1: FontJustificationV JUSTIFYTOP,
 Format2: FontJustificationV JUSTIFYMIDDLE,
 Format3: FontJustificationV JUSTIFYBOTTOM,
 Purpose: When an TEXT frame is given a rect, it will pos the displayed text y-wise at that TextJustify

 FontJustificationOffset
 Format: FontJustificationOffset x y,
 Example: FontJustificationOffset 0.01 0.0,
 Purpose: Displays the Text with that offset to it's frames position.

#### Beckx

#### Beckx

 Level 7

JoinedDec 29, 2019Messages89

Resources: 2

#### Resources
Spells and Systems 1Lua 1

@Tasyen

Hello, is it please possible to dynamically change tooltip text linked to the tooltiped frame ... i mean you got SimpleButton X and Button Y both have tooltip Z and if tooltip Z is summoned by SimpleButton X it shows different text than if its summoned by Button Y ... solution could be catching MOUSE_ENTER event but it cannot be used on SimpleButton so ... i don't know if there is any other way which can catch the moment when tooltip is displayed as well as frame which caused it.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

you could give each Button an empty (Simple)Frame Tooltip and check in a timer-callback, if they are shown using BlzFrameIsVisible. Then display Text/Images based on which Fake-Tooltip is currently shown. Beaware that the result of BlzFrameIsVisible differs between players.

#### Beckx

#### Beckx

 Level 7

JoinedDec 29, 2019Messages89

Resources: 2

#### Resources
Spells and Systems 1Lua 1

Tasyen said:

 you could give each Button an empty (Simple)Frame Tooltip and check in a timer-callback, if they are shown using BlzFrameIsVisible. Then display Text/Images based on which Fake-Tooltip is currently shown. Beaware that the result of BlzFrameIsVisible differs between players.


Click to expand...

That's actually really smart, i am even able to detect mouse_enter / mouse_leave events on simplebuttons this way thanks.

#### Teo_omant

#### Teo_omant

 Level 4

JoinedNov 4, 2019Messages47

Resources: 2

#### Resources
Maps 2

Hello. Which native function is responsible for automatically changing the size of the frame from the number of text in it?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Teo_omant said:

 Hello. Which native function is responsible for automatically changing the size of the frame from the number of text in it?


Click to expand...

The Size of a Text/String-Frame extends when the Frame's dimensions were not limited and it is required.

Autoresizing is based on the FramePoints you used to place the Frame.

You can also set a Point and set Size of the dynamic dimension by setting it to 0.

Example of a Text in Box Frame in which the Box mimics the Text-Frame's Size

UI: Frames and Tooltips | HIVE (hiveworkshop.com)

#### xorkatoss

#### xorkatoss

 Level 20

JoinedJul 12, 2010Messages1,746

Resources: 10

#### Resources
Models 6Maps 1Spells and Systems 3

@Tasyen first of all sorry for necro-posting...

I seem to have a desync problem when I try to change the values of the "BoxedText" in a local block.

Here is my code that is causing desync:

 JASS:

 set text = GetItemName(UnitItemInSlot(u, 0))
 set text2 = BlzGetItemExtendedTooltip(UnitItemInSlot(u, 0))

 if GetLocalPlayer() == GetTriggerPlayer() then
 call BlzFrameSetText(BlzGetFrameByName("BoxedTextTitle", 1), text)
 call BlzFrameSetText(BlzGetFrameByName("BoxedTextValue", 1), text2)
 endif

here is my normal code that works BUT all players can see the change:

 JASS:

 set text = GetItemName(UnitItemInSlot(u, 0))
 call BlzFrameSetText(BlzGetFrameByName("BoxedTextTitle", 1), text)
 set text = BlzGetItemExtendedTooltip(UnitItemInSlot(u, 0))
 call BlzFrameSetText(BlzGetFrameByName("BoxedTextValue", 1), text)

any idea why local blocks cause a desync when changing the text??

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

xorkatoss said:

 any idea why local blocks cause a desync when changing the text??


Click to expand...

When an frame enter's the map's script (the first time) it gets a new handleId when that happens in a Local player block the handleIds don't match anymore and you get a desync.

Reserve HandleIds in a sync manner right after the creation of the frame. You don't need to store them or anything just let Warcraft 3 give them a handleId, after that you can use BlzGetFrameByName (for this args) in local block without desync.

.... BlzCreateFrame ...

call BlzGetFrameByName("BoxedTextTitle", 1)

call BlzGetFrameByName("BoxedTextValue", 1)

This also applies to BlzGetOriginFrame, BlzFrameGetChild and BlzFrameGetParent, if they let a frame enter your map script the frame gets a handleId.


 Last edited: Jan 19, 2022

#### xorkatoss

#### xorkatoss

 Level 20

JoinedJul 12, 2010Messages1,746

Resources: 10

#### Resources
Models 6Maps 1Spells and Systems 3

@Tasyen the god of UI Natives strikes again!

didn't think this was such a simple fix I thought I was doing something wrong in jass

thanks again!

#### TTT

#### TTT

 Level 4

JoinedApr 21, 2017Messages41

hello! your tutorial is very useful. i tried to imitate your case, but encountered a problem.

i wanted to justify the text size, but it didn't work. why?

（0.014 and 0.012, but in game is same size）

 Frame "Text" "TalentTitle" {

 FontFlags "FIXEDSIZE",

 FrameFont "MasterFont", 0.014, "",

 FontJustificationH JUSTIFYCENTER, //居中对齐

 FontJustificationV JUSTIFYTOP, //字体对齐V

 }

 Frame "Text" "TalentInfo" {

 FontFlags "FIXEDSIZE",

 FrameFont "war3mapImported\leisel_UI_Font.TTF", 0.012, "", //字体路径 大小

 FontJustificationH JUSTIFYCENTER, //居中对齐

 FontJustificationV JUSTIFYTOP, //字体对齐V

 }


Click to expand...

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

"MasterFont" requires DecorateFileNames, in the frame blueprint. Without DecorateFileNames, the game searches for a Font-File with the name MasterFont which it can't find.

With it, the game will look for the entry in a StringTable under the key "MasterFont" and then searches for the file named as the value gained from the table.

Good luck


 Last edited: May 17, 2022

#### TTT

#### TTT

 Level 4

JoinedApr 21, 2017Messages41

Tasyen said:

 "MasterFont" requires DecorateFileNames, in the frame blueprint. Without DecorateFileNames, the game searches for a Font-File with the name MasterFont which it can't find.

With it, the game will look for the entry in a StringTable under the key "MasterFont" and then searches for the file named as the value gained from the table.

Good luck


Click to expand...

THANKS, i wrote the wrong path. it's my problem

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

Hello Tasyen,

thank you for your tutorials. I've built my entire custom UI on their back. The frame tooltip works great as well, but I'm having an issue trying to copy what you suggested to commenters earlier in the thread about setting the height to 0 to auto-align it with the text height. Here is what it looks like when I do that:

And this is my code

 JASS:

 set heroIconFrame = BlzCreateFrameByType("BACKDROP", "heroIconFrame", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
 call BlzFrameSetSize(heroIconFrame, 0.0385, 0.0385)
 call BlzFrameSetAbsPoint(heroIconFrame, FRAMEPOINT_TOPLEFT, 0.156, 0.1470)
 call BlzFrameSetVisible( heroIconFrame , false )
 call BlzFrameSetTexture( heroIconFrame , "ReplaceableTextures\\CommandButtons\\BTNBlackWithEdges.blp" , 0 , true )

 set heroIconFrameHover = BlzCreateFrameByType("FRAME" , "heroIconFrameHover" , heroIconFrame , "" , 0)
 call BlzFrameSetAllPoints( heroIconFrameHover , heroIconFrame )

 set heroIconFrameTooltip = BlzCreateFrame("BoxedText", heroIconFrameHover, 0, 0)
 call BlzFrameSetAbsPoint( heroIconFrameTooltip , FRAMEPOINT_BOTTOMRIGHT , 0.8 , 0.1625 )
 call BlzFrameSetTooltip( heroIconFrameHover , heroIconFrameTooltip )
 call BlzFrameSetSize( heroIconFrameTooltip , 0.29 , 0.0 )

 call BlzFrameSetText(BlzGetFrameByName("BoxedTextValue",0) , "Text" ) //Also tried setting the height of this frame to 0.
 //I'm guessing I have to set this to 0 and change the fdf so that the backdrop size is subject to the textbox size and
 //not the other way around? But I don't know how I go about doing that.
 call BlzFrameSetText(BlzGetFrameByName("BoxedTextTitle",0) , "Title" )

I'm using your fdf template for BoxedText in the OP with just some numbers changed around.

 Code:

// -- LOCAL TEMPLATES -------------------------------------------------------
Frame "BACKDROP" "BoxedTextBackgroundTemplate" {
 DecorateFileNames, //Look-Up Names in some String table (for example gameinterface)
 BackdropTileBackground, //Tile mode enabled
 BackdropBackground "ToolTipBackground", //BackgroundFile
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.008, //higher numbers make the corners bigger.
 BackdropBackgroundInsets 0.0022 0.0022 0.0022 0.0022, //makes the background smaller, from the outside.
 BackdropEdgeFile "ToolTipBorder", //the border File
 BackdropBlendAll,
}
// -- Frames -------------------------------------------------------
Frame "BACKDROP" "BoxedText" INHERITS "BoxedTextBackgroundTemplate" {
 UseActiveContext,

 Frame "TEXT" "BoxedTextTitle" {
 UseActiveContext,
 DecorateFileNames,
 SetPoint TOPLEFT, "BoxedText", TOPLEFT, 0.005, -0.005, //Positionate "BoxedTextSimpleTitle"'s TOPLEFT to "BoxedText"'s TOPLEFT with an offset
 SetPoint TOPRIGHT, "BoxedText", TOPRIGHT, -0.005, -0.005,
 FontFlags "FIXEDSIZE",
 FrameFont "MasterFont", 0.011, "",
 FontColor 1.0 1.0 1.0 1.0, //Red Green Blue Alpha 0.0 to 1.0
 FontShadowColor 0.0 0.0 0.0 0.9,
 FontShadowOffset 0.001 -0.001,
 }

 Frame "TEXT" "BoxedTextValue" {
 UseActiveContext,
 DecorateFileNames,
 SetPoint TOPLEFT, "BoxedText", TOPLEFT, 0.005, -0.0275,
 SetPoint BOTTOMRIGHT, "BoxedText", BOTTOMRIGHT, -0.005, 0.005,
 FontFlags "FIXEDSIZE",
 FrameFont "MasterFont", 0.011, "",
 FontColor 1.0 1.0 1.0 1.0,
 FontShadowColor 0.0 0.0 0.0 0.9,
 FontShadowOffset 0.001 -0.001,
 }
}

I'm also wondering if it's possible to easily add the separator line between the title and the text that's present in standard tooltips. I'm trying to replicate them exactly.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Antares A said:

 thank you for your tutorials. I've built my entire custom UI on their back. The frame tooltip works great as well, but I'm having an issue trying to copy what you suggested to commenters earlier in the thread about setting the height to 0 to auto-align it with the text height. Here is what it looks like when I do that:

Click to expand...

That image shows that a BACKDROP has not enough Size to place it's border Texture.

That is my old fdf, with fixed size and without a seperator.

BoxedText is the Box and the other Frames are relative to it and they also are inside it. If the box does not have a x & y -Size the other Frames can not be placed/displayed.

call BlzFrameSetSize( heroIconFrameTooltip , 0.29 , 0.0 ) is wrong it expects a y Size in that setup.

A seperator would be a BACKDROP with a choosen texture, could be a playercolor or V1.32 should have a texture for it. Then you change the text's position to include the seperator. I assume the seperator would go below the BoxedTextTitle and you would just lower the top positions of BoxedTextValue.

ToolTipBackground=UI\Widgets\ToolTips\Human\human-tooltip-background.blp

ToolTipBorder=UI\Widgets\ToolTips\Human\human-tooltip-border.blp

ToolTipHorizontalSeparator=UI\Widgets\ToolTips\Human\HorizontalSeparator.blp

That ySize = 0 is another approach in which the relative positions of the TooltipFrames are changed. You have a TEXT Frame with Size (x, 0) and a position, The box and other Tooltip-content is (direct or indirect) relative to the TEXT Frame. When the TEXT displays text the game adjustes the ySize and all frame relative to it update aswell. But important: The TEXT is a child or silberling of the box which makes it impossible to setup only with fdf.

In TasButtonList I made such a custom TooltipFrame which takes more space when more Text is displayed and has a seperator. Could check its tooltip fdf & map script for a example.

HeroScoreFrame has also the "autoscaling" tooltip but it is far simpler, no seperator only Text in Box.

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

I was able to come up with something based on your suggestions and it worked perfectly, thank you!

I anchored only the topleft and topright points of BoxedTextValue to BoxedText, then added these two lines in my map:

 JASS:

call BlzFrameSetSize(BlzFrameGetChild(heroIconFrameTooltip,1) , 0.28 , 0.0 )
call BlzFrameSetSize(heroIconFrameTooltip , 0.29 , BlzFrameGetHeight(BlzFrameGetChild(heroIconFrameTooltip,1)) + 0.0315)

Adding the separator with a new backdrop frame also worked perfectly.

 Code:

// -- LOCAL TEMPLATES -------------------------------------------------------
Frame "BACKDROP" "BoxedTextBackgroundTemplate" {
 DecorateFileNames, //Look-Up Names in some String table (for example gameinterface)
 BackdropTileBackground, //Tile mode enabled
 BackdropBackground "ToolTipBackground", //BackgroundFile
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.008, //higher numbers make the corners bigger.
 BackdropBackgroundInsets 0.0022 0.0022 0.0022 0.0022, //makes the background smaller, from the outside.
 BackdropEdgeFile "ToolTipBorder", //the border File
 BackdropBlendAll,
}
// -- Frames -------------------------------------------------------
Frame "BACKDROP" "BoxedText" INHERITS "BoxedTextBackgroundTemplate" {
 UseActiveContext,

 Frame "TEXT" "BoxedTextTitle" {
 UseActiveContext,
 DecorateFileNames,
 SetPoint TOPLEFT, "BoxedText", TOPLEFT, 0.004, -0.0045, //Positionate "BoxedTextSimpleTitle"'s TOPLEFT to "BoxedText"'s TOPLEFT with an offset
 SetPoint TOPRIGHT, "BoxedText", TOPRIGHT, -0.004, -0.0045,
 FontFlags "FIXEDSIZE",
 FrameFont "MasterFont", 0.011, "",
 FontColor 1.0 1.0 1.0 1.0, //Red Green Blue Alpha 0.0 to 1.0
 FontShadowColor 0.0 0.0 0.0 0.9,
 FontShadowOffset 0.001 -0.001,
 }

 Frame "TEXT" "BoxedTextValue" {
 UseActiveContext,
 DecorateFileNames,
 SetPoint TOPLEFT, "BoxedText", TOPLEFT, 0.004, -0.0275,
 SetPoint TOPRIGHT, "BoxedText", TOPRIGHT, -0.004, -0.0275,
 FontFlags "FIXEDSIZE",
 FrameFont "MasterFont", 0.011, "",
 FontColor 1.0 1.0 1.0 1.0,
 FontShadowColor 0.0 0.0 0.0 0.9,
 FontShadowOffset 0.001 -0.001,
 }

 Frame "BACKDROP" "BoxedTextSeparator" {
 DecorateFileNames,
 BackdropBackground "ToolTipHorizontalSeparator",
 SetPoint TOPLEFT, "BoxedText", TOPLEFT, 0.008, -0.021,
 SetPoint BOTTOMRIGHT, "BoxedText", TOPRIGHT, -0.008, -0.0215,
 BackdropBlendAll,
 }
}

[/HIDEN]

#### Attachments

tooltip.png

 36.8 KB

 · Views: 120



#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Added the version from the big UI tutorial the old version is still there in a tab.

#### Produtor

#### Produtor

 Level 6

JoinedOct 31, 2011Messages107

xorkatoss said:

@Coldgamer no fear xorkatoss is near!

 JASS:

function CreateUther takes nothing returns nothing
 local framehandle face = BlzCreateFrameByType("BACKDROP", "Face", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
 local framehandle faceHover = BlzCreateFrameByType("FRAME", "FaceFrame", face,"", 0)
 local framehandle tooltip = BlzCreateFrame("BoxedText", face, 0, 0)
 call BlzFrameSetAllPoints(faceHover, face)
 call BlzFrameSetTooltip(faceHover, tooltip)

 call BlzFrameSetSize(face, 0.04, 0.04)
 call BlzFrameSetAbsPoint(face, FRAMEPOINT_CENTER, 0.4, 0.3)
// call BlzFrameSetAbsPoint(tooltip, FRAMEPOINT_CENTER, 0.2, 0.3)
 call BlzFrameSetPoint(tooltip, FRAMEPOINT_BOTTOM, face, FRAMEPOINT_TOP, 0.0, 0.0)
 call BlzFrameSetSize(tooltip, 0.15, 0.08)
 call BlzFrameSetText(BlzGetFrameByName("BoxedTextValue", 0), "Human Paladin Face, but it is not uther.")
 call BlzFrameSetText(BlzGetFrameByName("BoxedTextTitle", 0), "Paladin")

 call BlzFrameSetTexture(face, "ReplaceableTextures\\CommandButtons\\BTNHeroPaladin",0, true)
endfunction
//===========================================================================
function InitTrig_Uther takes nothing returns nothing
 set gg_trg_Uther = CreateTrigger( )
 call TriggerRegisterTimerEventSingle( gg_trg_Uther, 1.00 )
 call TriggerAddAction( gg_trg_Uther, function CreateUther)
 call BlzLoadTOCFile("war3mapimported\\BoxedText.toc")
endfunction

Who would have though jass was so easy?

I had to convert this for me because lua is so inconvenient, having all your code in the map header is a HUGE mess, I mean who even though creating lua was gonna be useful? am I right? lol

anyway so I thought why not share it here to help some people, enjoy.


Click to expand...

How do I update the text inside the box with custom script?

#### Uncle

#### Uncle

Warcraft Moderator

 Level 76

JoinedAug 10, 2018Messages8,351

Resources: 1

#### Resources
Spells and Systems 1

Santacruz said:

 How do I update the text inside the box with custom script?


Click to expand...

Custom script: call BlzFrameSetText( YourFrame, "The Text" )
Custom script allows you to write code in your triggers. So you can do just about anything you see in Jass/Lua code inside of it. That means you should use the code as a frame of reference. You can literally copy and paste the important line of code into your custom script.

Frames have their own variable type called Framehandle, just like how you have variable types for Units/Items/Players/Points/Integers, etc... This variable type cannot be created in GUI's standard Variable Editor but is accessible in code form. You can see in the code above that the frames are being stored in local framehandle variables, which is not ideal for you since local variables can only be accessed inside of the function that created them. Instead, you can store the frames in a global variable which you can then access anywhere at any time.

 vJASS:

globals
 framehandle tooltip
endglobals

function CreateUther takes nothing returns nothing
 local framehandle face = BlzCreateFrameByType("BACKDROP", "Face", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
 local framehandle faceHover = BlzCreateFrameByType("FRAME", "FaceFrame", face,"", 0)
 tooltip = BlzCreateFrame("BoxedText", face, 0, 0)
 call BlzFrameSetAllPoints(faceHover, face)
 call BlzFrameSetTooltip(faceHover, tooltip)

 call BlzFrameSetSize(face, 0.04, 0.04)
 call BlzFrameSetAbsPoint(face, FRAMEPOINT_CENTER, 0.4, 0.3)
 call BlzFrameSetPoint(tooltip, FRAMEPOINT_BOTTOM, face, FRAMEPOINT_TOP, 0.0, 0.0)
 call BlzFrameSetSize(tooltip, 0.15, 0.08)
 call BlzFrameSetText(BlzGetFrameByName("BoxedTextValue", 0), "Human Paladin Face, but it is not uther.")
 call BlzFrameSetText(BlzGetFrameByName("BoxedTextTitle", 0), "Paladin")

 call BlzFrameSetTexture(face, "ReplaceableTextures\\CommandButtons\\BTNHeroPaladin",0, true)
endfunction
//===========================================================================
function InitTrig_Uther takes nothing returns nothing
 set gg_trg_Uther = CreateTrigger( )
 call TriggerRegisterTimerEventSingle( gg_trg_Uther, 1.00 )
 call TriggerAddAction( gg_trg_Uther, function CreateUther)
 call BlzLoadTOCFile("war3mapimported\\BoxedText.toc")
endfunction

I created a framehandle variable called tooltip, which I can then reference in GUI like so:

Events
Time - Elapsed game time is 2.00 seconds

Conditions

Actions
Custom script: call BlzFrameSetText( tooltip, "hello" )


 Last edited: Oct 27, 2024

#### RPG_F

#### RPG_F

 Level 4

JoinedFeb 24, 2018Messages46

how to find the basic original TooltipFrame's textframe, what's the tag name or id?

 You must log in or register to reply here.


### Similar threads

 UI: Create a TextButton


Tasyen

May 30, 2019

Scripting Tutorials

Replies12Views12K

Jan 11, 2021
Tasyen

 UI - The concept of Parent-Frames


Tasyen

Nov 19, 2019

Scripting Tutorials

Replies20Views11K

Oct 10, 2020
Chaosy

 UI: FrameEvents and FrameTypes


Tasyen

Aug 27, 2019

Scripting Tutorials

Replies16Views12K

Dec 7, 2024
Gaspar

 UI - Simpleframes


Tasyen

Nov 25, 2019

Scripting Tutorials

Replies30Views16K

Sep 27, 2025
Tasyen

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