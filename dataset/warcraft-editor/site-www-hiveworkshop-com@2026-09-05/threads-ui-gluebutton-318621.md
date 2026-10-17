---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-gluebutton.318621/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Gluebutton
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-gluebutton.318621/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

##
 Button

Buttons are powerful and useful UI-Frames. A Button in Warcraft 3 is only a space on the screen that can be pressed. The pressing can send an audio Feedback (GLUE created with BlzCreateFrame) and one can catch the pressing with a TriggerEvent which allows executing Code. But if the button is only the clickable space where does the displayed Image, the Text and the Highlight come from?

Jump to without fdf

Hotkeys

Each is an own strong connected childFrame being used as some function. Such strong conneted childs mimic size and Position of the parent Frame. There is a BACKDROP that is shown when the BUTTON is enabled, another is shown when the BUTTON is disabled and so on, each such is an own ChildFrame....

​

##
 Example

Here is an Image of the text inside a fdf of a GLUETEXTBUTTON containg most, when not all strong connected childFrames. In the Image: text that is connected to Childframes is in a colored box. Also there are some comments right to the Frames in different color.

ControlStyle "AUTOTRACK", Mouse Clicks onto the button won't affect/reach the world below the button, can't order units or select Units.

The TEXT-ChildFrame is not needed to set the text of the TEXTBUTTON by Code, but it is needed to style it in the fdf.

The highlighter does only work when ControlStyle mentions that behaviour.

A (GLUE)BUTTON is basicly the same without the ButtonText feature/child-Frame.​

##
 Unneeded ChildFrames

Child-Frames that you do not need can often be skiped.

Therefore this is still a valid GLUEBUTTON: even with only mentioning ControlBackdrop, ControlDisabledBackdrop and ControlMouseOverHighlight.

Means a texture in enabled state, a texture for disabled state and a glowing for mouse hovering.

 Code:

Frame "GLUEBUTTON" "HeroSelectorButton" {
 Width 0.035,
 Height 0.035,
 ControlStyle "AUTOTRACK|HIGHLIGHTONMOUSEOVER",

 ControlBackdrop "HeroSelectorButtonIcon",
 Frame "BACKDROP" "HeroSelectorButtonIcon" {
 }

 ControlDisabledBackdrop "HeroSelectorButtonIconDisabled",
 Frame "BACKDROP" "HeroSelectorButtonIconDisabled" {
 }

 ControlMouseOverHighlight "HeroSelectorButtonHighLight",
 Frame "HIGHLIGHT" "HeroSelectorButtonHighLight" {
 HighlightType "FILETEXTURE",
 HighlightAlphaFile "UI\Glues\ScoreScreen\scorescreen-tab-hilight.blp",
 HighlightAlphaMode "ADD",
 }
}

An example how one would create this "HeroSelectorButton". One can't cause it is not loaded.

 JASS:

//Create "HeroSelectorButton", for game UI
 local framehandle buttonFrame = BlzCreateFrame("HeroSelectorButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
//By Having created "HeroSelectorButton" also its children are created, this children are accessed using BlzGetFrameByName right after the creation or later one if the slots were not taken by other frames.
 local framehandle buttonIconFrame = BlzGetFrameByName("HeroSelectorButtonIcon", 0)
//Set a Texture
 call BlzFrameSetTexture(buttonIconFrame, "ReplaceableTextures\\CommandButtons\\BTNSelectHeroOn", 0, true)
// Place the buttonFrame to the center of the screen
 call BlzFrameSetAbsPoint(buttonFrame, FRAMEPOINT_CENTER, 0.4, 0.3)
// Give that buttonFrame a size
 call BlzFrameSetSize(buttonFrame, 0.05, 0.05)

###
 Hotkeys in fdf

(GLUE)(TEXT)BUTTONs can have set Hotkeys right in the fdf. This is done by giving the Button a Parent Frame that has the fdfAction TabFocusPush, aswell as creating a StringList with the wanted Hotkeys. Then one creates Buttons as child of the Frame with TabFocusPush, and setups ControlShortcutKey "StringName in StringList",. This hotkeys will consider the current active main input Frame, when created for GAMEUI they will stop listen when the player is in a Menu or types in chat messages.

For some wierd reason the Hotkeys don't work out of the Box (when created for GAMEUI), first one has to hide and show the Parent Frame. Then the hotkeys will listen and fire when clicked. The hotkeys will fire a control-click Event which does not keep the keyboard focus unlike when the event is started with a mouse click.

Buttons added by code (to a Parent with TabFocusPush,) during the runtime will not use ControlShortcutKey, making this quite static (like most in fdf). One could use OsKeyEvents for something more dynamic.

An example fdf:

 Code:

IncludeFile "UI\FrameDef\UI\EscMenuTemplates.fdf",

// The Hotkeys for the Buttons
StringList {
 KeyMyButton1 "1",
 KeyMyButton2 "2",
 KeyMyButton3 "3",
}

Frame "FRAME" "MyButtonF" {
 Width 0.1,
 Height 0.1,
 TabFocusPush, // Enables ControlShortcutKey for children
 LayerStyle "IGNORETRACKEVENTS", // this Frame itself will not control/Block the Mouse
 Frame "BUTTON" "MyButton1" INHERITS WITHCHILDREN "EscMenuButtonTemplate" {
 ControlShortcutKey "KeyMyButton1", // read "KeyMyButton1" and use it's value as Hotkey
 SetPoint TOP, "MyButtonF", TOP, 0, 0,
 }
 Frame "BUTTON" "MyButton2" INHERITS WITHCHILDREN "EscMenuButtonTemplate" {
 ControlShortcutKey "KeyMyButton2",
 SetPoint TOP, "MyButton1", BOTTOM, 0, 0,
 }
 Frame "BUTTON" "MyButton3" INHERITS WITHCHILDREN "EscMenuButtonTemplate" {
 ControlShortcutKey "KeyMyButton3",
 SetPoint TOP, "MyButton2", BOTTOM, 0, 0,
 }

}

The result of the fdf. It creates 3 Strings (KeyMyButton1, KeyMyButton2, KeyMyButton3) and Defines FRAME "MyButtonF" as well as it's 3 Child ButtonFrames "MyButton1", "MyButton2" and "MyButton3".

The Lua code to Load the fdf/toc create the Frames, trigger and events to listen to the Button clicks.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 BlzLoadTOCFile("war3mapImported\\Test.toc")
 local frame = BlzCreateFrame("MyButtonF", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)

 BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.6, 0.3)

 local trigger = CreateTrigger()

 BlzTriggerRegisterFrameEvent(trigger, BlzGetFrameByName("MyButton1", 0), FRAMEEVENT_CONTROL_CLICK)
 BlzTriggerRegisterFrameEvent(trigger, BlzGetFrameByName("MyButton2", 0), FRAMEEVENT_CONTROL_CLICK)
 BlzTriggerRegisterFrameEvent(trigger, BlzGetFrameByName("MyButton3", 0), FRAMEEVENT_CONTROL_CLICK)
 TriggerAddAction(trigger, function()
 print("click", BlzFrameGetName(BlzGetTriggerFrame()))
 end)

 -- for some reason the buttons did not use hotkeys on default but after the were hidden and shown the hotkeys work
 BlzFrameSetVisible(frame, false)
 BlzFrameSetVisible(frame, true)
 end
end

One still would need to write the toc and fdf. And import them into the map.

​

##
 Buttons only with Code

Fdf has nice Features, but if I would only want a button showing an Icon could that be created with Code alone?

Yes, using BlzCreateFrameByType one can create new empty Frames based on FrameType (behavoiur) and give them new names and even inherit other Frames.

One would create a GLUEBUTTON with parent GameUI and a BACKDROP using the created Button as parent. Also it would be required to mimic postion and size which BlzFrameSetAllPoints handles. Each of that is only one line of Code.

 JASS:

//Create a "GLUEBUTTON" named "Facebutton", the clickable Button, for game UI
 local framehandle buttonFrame = BlzCreateFrameByType("GLUEBUTTON", "FaceButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
//Create a BACKDROP named "FaceButtonIcon", the visible image, for buttonFrame.
 local framehandle buttonIconFrame = BlzCreateFrameByType("BACKDROP", "FaceButtonIcon", buttonFrame, "", 0)
//buttonIconFrame will mimic buttonFrame in size and position
 call BlzFrameSetAllPoints(buttonIconFrame, buttonFrame)
//Set a Texture
 call BlzFrameSetTexture(buttonIconFrame, "ReplaceableTextures\\CommandButtons\\BTNSelectHeroOn", 0, true)
// Place the buttonFrame to the center of the screen
 call BlzFrameSetAbsPoint(buttonFrame, FRAMEPOINT_CENTER, 0.4, 0.3)
// Give that buttonFrame a size
 call BlzFrameSetSize(buttonFrame, 0.05, 0.05)

And we have a Button with an Icon:

But what about the glowing, can that also be done only with code?

Yes, you see the empty string after the parentFrame in BlzCreateFrameByType, this is the Name of the mainFrame the new frame inherits from.​

##
 Inherit

Inherit is a technique in which data of another Frame is copied, one should only copy familiar frametypes.

When using BlzCreateFrameByType one can inherit data/behaviour from a loaded mainframe, a Frame with its framehead outside of a framebody, by writing the Name of the Frame one wants to inherit. For the glowing we need a mainframe with a ControlMouseOverHighlight or ControlFocusHighlight depends on the wanted behaviour. Inherited childrenFrames are not accessable with BlzGetFrameByName and don't get a handleId. With the release of the native BlzFrameGetChild in V1.32.6 one is able to get the inherited childFrames during the runtime, in 1.32.6 this native ignores String&Texture childFrames.

There are 2 Default loaded BUTTONs which could be inherited for glowing:

"IconButtonTemplate" (blue)

"ScoreScreenTabButtonTemplate" (yellow)

 JASS:

BlzCreateFrameByType("GLUEBUTTON", "MyFrameName", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "IconButtonTemplate", 0)
BlzCreateFrameByType("GLUEBUTTON", "MyFrameName", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "ScoreScreenTabButtonTemplate", 0)

The example from earlier now having a yellow glowing.

 JASS:

//Create a "GLUEBUTTON" named "Facebutton" inheriting "ScoreScreenTabButtonTemplate", the clickable Button, for game UI
 local framehandle buttonFrame = BlzCreateFrameByType("GLUEBUTTON", "FaceButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "ScoreScreenTabButtonTemplate", 0)
//Create a BACKDROP named "FaceButtonIcon", the visible image, for buttonFrame.
 local framehandle buttonIconFrame = BlzCreateFrameByType("BACKDROP", "FaceButtonIcon", buttonFrame, "", 0)
//buttonIconFrame will mimic buttonFrame in size and position
 call BlzFrameSetAllPoints(buttonIconFrame, buttonFrame)
//Set a Texture
 call BlzFrameSetTexture(buttonIconFrame, "ReplaceableTextures\\CommandButtons\\BTNSelectHeroOn", 0, true)
// Place the buttonFrame to the center of the screen
 call BlzFrameSetAbsPoint(buttonFrame, FRAMEPOINT_CENTER, 0.4, 0.3)
// Give that buttonFrame a size
 call BlzFrameSetSize(buttonFrame, 0.05, 0.05)

##
 Button Action

Most times when one creates a Button, that Button should do something when the user clicks it. In Warcraft 3 this is done over Triggers and Events. The most straightforward FrameEvent, for Buttons, is FRAMEEVENT_CONTROL_CLICK it happens when the user releases the left MouseButton when the initial mouseClick was inside the Button or when the Button has focus and the user clicks return/space.

An example Code, that prints a Message to the screen when a button was clicked when it was registered using ButtonAddAction:

 JASS:

function ButtonClickAction takes nothing returns nothing
 BJDebugMsg("Button Clicked")
endfunction

function ButtonAddAction takes framehandle buttonFrame returns nothing
 local trigger buttonEventTrigger = CreateTrigger()
 call TriggerAddAction(buttonEventTrigger, function ButtonClickAction)
 call BlzTriggerRegisterFrameEvent(buttonEventTrigger, buttonFrame, FRAMEEVENT_CONTROL_CLICK)
endfunction

###
 Focus

After a (GLUE)(TEXT)BUTTON was clicked (with the mouse) it will keep the clicking player's input focus. Blocking most hotkeys and oskey events as long they have the focus, even without any attached Trigger or FrameEvent. You can remove that focus from a clicked Button by disabling and enabling the clicked Button inside the TriggerAction then it loses the focus with the clicking. The user can also remove the focus with a left mouse click onto playable ground

 JASS:

function ButtonClickAction takes nothing returns nothing
 BJDebugMsg("Button Clicked")
 if GetLocalPlayer() == GetTriggerPlayer() then
 call BlzFrameSetEnable(BlzGetTriggerFrame(), false)
 call BlzFrameSetEnable(BlzGetTriggerFrame(), true)
 call StopCamera()
 endif
endfunction

Sadly this Technique can produce a bug, when the Button is clicked and a pressed arrow key is released almost at the same time the camera will keep panning even without holding the arrow key (discovered by Uncle), another mouse click by the user somewhere stops the auto panning.

Uncle found later a way to stop the paning by code using StopCameraForPlayerBJ() or StopCamera()

​

##
 FrameEvent

Inside Frameevents one can get some informations about the current event.

 Code:

BlzGetTriggerFrame()
BlzGetTriggerFrameEvent()
GetTriggerPlayer()
BlzGetTriggerFrameText()
BlzGetTriggerFrameValue()

The linked content is a list about frameevents and which frametypes can use them.

[JASS/AI] - UI: FrameEvents and FrameTypes

Edited: Replaced selfexecution with a less problematic approach

Edited: Added a demo map with a (re)selection example & a demo map for fdf hotkeys

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

Create Face Button.w3x

 19.3 KB

 · Views: 165



ControlShortcutKey 1.31.w3x

 18.2 KB

 · Views: 127




 Last edited: Aug 22, 2023

#### xorkatoss

#### xorkatoss

 Level 20

JoinedJul 12, 2010Messages1,746

Resources: 10

#### Resources
Models 6Maps 1Spells and Systems 3

ahh Tasyen with the epic UI tutorials again! I was actually looking for something like this.

One of the problems I had and I couldn't get it to work was that I was actually creating 2 glue buttons and using 1 for the click event and the other for the glow.

In the end I just figured it out that you can have 1 glue button to do both lol...(you might want to mention that)

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Thanks for your input.

xorkatoss said:

 In the end I just figured it out that you can have 1 glue button to do both lol...(you might want to mention that)


Click to expand...

Kinda wanted to do that with the last part called inherit.

#### mephistophelesss

#### mephistophelesss

 Level 1

JoinedSep 7, 2018Messages3

have u had the chance to get a working popup menu? I'm not sure how to get make the menu dropdown appear when you click on the gluetextbutton. I can actually change the value of the menu in game but I have to guess where the option would be in the popupmenu because nothing appear after I click on the gluetextbutton. Is it because it has to be activated in the map through event or am I doing something wrong in the fdf file? I'm using the same lines of code as EscOptionWindowModeMenu, i also tried other menu had the same issue.

 Code:

 Frame "POPUPMENU" "EscOptionsWindowModeMenu" {
 Width 0.1175,
 Height 0.022,
 PopupButtonInset 0.01,
 SetPoint LEFT, "WindowModeLabel", RIGHT, 0.004, 0.0,

 ControlBackdrop "EscOptionsWindowModePopupMenuBackdrop",
 Frame "BACKDROP" "EscOptionsWindowModePopupMenuBackdrop" INHERITS "EscMenuButtonBackdropTemplate" {
 }

 ControlDisabledBackdrop "EscOptionsWindowModePopupMenuDisabledBackdrop",
 Frame "BACKDROP" "EscOptionsWindowModePopupMenuDisabledBackdrop" INHERITS "EscMenuButtonDisabledBackdropTemplate" {
 }

 PopupTitleFrame "EscOptionsWindowModePopupMenuTitle",
 Frame "GLUETEXTBUTTON" "EscOptionsWindowModePopupMenuTitle" INHERITS WITHCHILDREN "EscMenuPopupMenuTitleTemplate" {
 FrameFont "MasterFont",0.0085,"",
 }

 PopupArrowFrame "EscOptionsWindowModePopupMenuArrow",
 Frame "BUTTON" "EscOptionsWindowModePopupMenuArrow" INHERITS WITHCHILDREN "EscMenuPopupMenuArrowTemplate" {
 }

 PopupMenuFrame "EscOptionsWindowModePopupMenuMenu",
 Frame "MENU" "EscOptionsWindowModePopupMenuMenu" INHERITS WITHCHILDREN "EscMenuPopupMenuMenuTemplate" {
 MenuItem "WINDOW_MODE_WINDOWED", -2,
 MenuItem "WINDOW_MODE_WINDOWED_FULLSCREEN", -2,
 MenuItem "WINDOW_MODE_FULLSCREEN", -2,
 FrameFont "MasterFont",0.009,"",
 }
 }

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Yes, I created a custom working PopUpMenu, but most was inherited from Escmenutemplates like you do. Do your Fdf has this line IncludeFile "UI/FrameDef/UI/EscMenuTemplates.fdf",, at the beginning of the fdf?

#### mephistophelesss

#### mephistophelesss

 Level 1

JoinedSep 7, 2018Messages3

if you were able to create them then there must be issues with my fdf. Yes i included both esc template and standard templates. Also the control seems to work since i'm able to choose from the menu, but the menu backdrop and menu items just dont appear, when I click on the gluetextbutton (I attached a video to show what I mean). Thank you for your pointers!

#### Attachments

2020-03-07 15-52-41.mp4

 405.1 KB

 · Views: 354




 Last edited: Mar 7, 2020

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

I can mimic the behaviour by setting the Alpha of the PopupMenuFrame (the child with the menu items) to 0 with code. BlzFrameSetAlpha

But if that is not it here some Ideas you can check / try.

What Frame did you create? Did you define a new custom POPUPMENU?

Did you redefine the same Frame? Not so good supported in V1.31 (I assume you use that version cause of the Textures the frame native framework, was imporved since it was released) try changing the names of your custom Frames, maybe add a prefix "My" to avoid name colisions. Something I did.

Some random thinking:

Clearing the Points of the Menu would look different, also you could not select options in that case.

GetLocalPlayer said something that loading fdf in one toc that are included in a fdf mentioned in the same fdf in a wierd order can produce problems (but you were able to create the frame).

If there were a problem in the PopupMenuFrame then you could not choose options.

Good luck.

#### mephistophelesss

#### mephistophelesss

 Level 1

JoinedSep 7, 2018Messages3

Tasyen said:

I can mimic the behaviour by setting the Alpha of the PopupMenuFrame (the child with the menu items) to 0 with code. BlzFrameSetAlpha

But if that is not it here some Ideas you can check / try.

What Frame did you create? Did you define a new custom POPUPMENU?

Did you redefine the same Frame? Not so good supported in V1.31 (I assume you use that version cause of the Textures the frame native framework, was imporved since it was released) try changing the names of your custom Frames, maybe add a prefix "My" to avoid name colisions. Something I did.

Some random thinking:

Clearing the Points of the Menu would look different, also you could not select options in that case.

GetLocalPlayer said something that loading fdf in one toc that are included in a fdf mentioned in the same fdf in a wierd order can produce problems (but you were able to create the frame).

If there were a problem in the PopupMenuFrame then you could not choose options.

Good luck.


Click to expand...

I'm working with Wc3 lastest patch.

I tried multiple pop-up menus from both the standard template and esc menu template, with no other lines of code but the pop-up menu frame and includes in my fdf (also using a prefix), and I always get the same thing. Not to mention I can create the menu frame and it appears on my screen without any problem. Anyway for now I think i'll just use another system I thought to replace the popupmenu, thanks for trying to help.

update: After more testing I've realized the error comes from compiling the map with wurst, not sure why.


 Last edited: Mar 9, 2020

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

I want to make some sort of a toggle button that shows a Disabled texture when it is toggled off.

Unfortunately, disabled buttons cannot be pressed, so I cannot make use of the Disable feature.

Therefore I want to make it so that the button texture changes when clicked between the Enabled and Disabled texture, and remember the button state with a variable.

But I cannot find the right child frame the is responsible for holding the texture!

This is a simple question, I hope you are able to help.

My FDF:

 Lua:

Frame "BUTTON" "Button" {
 Height 0.027,
 Width 0.027,
 ControlStyle "AUTOTRACK|HIGHLIGHTONMOUSEOVER",

 ControlBackdrop "ButtonTex",
 Frame "BACKDROP" "ButtonTex" {
 SetAllPoints,
 BackdropBlendAll,
 BackdropBackground "war3mapImported\BTNTimeUp.blp",
 }

 ControlPushedBackdrop "ButtonPush",
 Frame "BACKDROP" "ButtonPush" {
 SetAllPoints,
 BackdropBlendAll,
 BackdropBackground "war3mapImported\BTNTimeUp.blp",
 BackdropBackgroundInsets 0.001 0.001 0.001 0.001,
 }

 ControlMouseOverHighlight "ButtonGlow",
 Frame "HIGHLIGHT" "ButtonGlow" {
 SetAllPoints,
 DecorateFileNames,
 HighlightType "FILETEXTURE",
 HighlightAlphaFile "EscMenuButtonMouseOverHighlight",
 HighlightAlphaMode "ADD",
 }
}

How do I find the right frame to change the button texture?

The master frame "Button" is not the right one.

Changing frames using BlzFrameSetTexture on BlzGetFrameByName("ButtonTex", 0) doesn't work either, despite having this frame created with integer 0.

I have done a check and found following:

print(BlzGetFrameByName("ButtonTex", 0)) does return a frame handle

print(BlzFrameGetName(BlzGetFrameByName("ButtonTex", 0)) doesn't return a frame name.

The Frame Name for the "Button" frame can be retrieved.

If it is not possible, I guess the only way is to create a new button entirely (with a differing Frame Definition) each time it is toggled.[/code]

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

The behaviour you want to do is possible, I have done it in HeroSelector to toogle used categories.

Well the first test is to check if your frame creation was succeful at all, if you don't see the button. It sounds like that.

Sadly in Lua printing a frame can be buggy and it displays something looking like a valid framehandle for a nil frame. I advise printing GetHandleId instead, if that prints 0 then no frame was found.

There can be multiple reasons for that:

Errors in the toc (missing empty end line)

Error in the fdf-File

Not matching filenames in code and imported path (toc)

Creating a Frame as child of a SimpleFrame

Wrong FrameName

Wrong CreateContext

and more

BlzGetFrameByName("ButtonTex", 0) gives you the BACKDROP managing the visual in the enabled state (cause it was set to be ControlBackdrop), (ControlPushedBackdrop is shown while a player is clicking the button).

BlzGetFrameByName should be used right after the frame was created and that frame then should be saved into a variable which can then be later used. Otherwise you have to be careful to not overwritting a used slot in the frame storage with another new Frame.

You don't need SetAllPoints, for functional child-Frames. The game fits such frames automaticaly to the right place, if you did not told them otherwise.

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Thank you Tasyen,

Good to know that SetAllPoint is not needed.

I believe the FDF and TOC are fine as the button displays, is clickable, and it's related click event fires.

By testing to attempt to print the handle Id, it is clear I cannot reach the backdrop frame.

None of the button child frames appear to be found with using BlzGetFrameByName with the child frame name and the

 Lua:

 f = BlzCreateFrameByType("BUTTON", "Button", wf, "Button", 0)
 f2 = BlzGetFrameByName("ButtonTex", 0)
 f3 = BlzGetFrameByName("ButtonPush", 0)
 f4 = BlzGetFrameByName("ButtonGlow", 0)
 f5 = BlzGetFrameByName("Button", 0)
 print("f: "..GetHandleId(f))
 print("f2: "..GetHandleId(f2))
 print("f3: "..GetHandleId(f3))
 print("f4: "..GetHandleId(f4))
 print("f5: "..GetHandleId(f5))

f (main frame) produces a result, f2 through f4 will print '0'

f5 produces a result as well, meaning BlzGetFrameByName is succesful in finding the main frame at the creating integer 0.

Not really what I expected.

Were you able to reach button child frames in your application?

Otherwise, I would not mind too much to simply destroy and recreate the buttons for toggle. It seems not too demanding on game memory, although I like a more elegant solution

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Inherited child-Frames are unreachable by code. You inherited them with the way you created the frame with BlzCreateFrameByType. If you want to use this child frames in your code you need to create the Frame (defined in fdf) either with BlzCreateFrame or BlzCreateSimpleFrame.

BlzCreateFrameByType defines (for the creation only) a new Frame and creates it. While the other 2 create frame natives create a frame from a loaded definition.

Create your frame with BlzCreateFrame then you can access the child-Frames.


 Last edited: May 16, 2020

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Not much to say, used this as a reference many times myself. Extremely useful.

I did apply indents and put one of the images in a hidden tag, everything else was good enough IMO.

Approved.

#### KW~

#### KW~

 Level 2

JoinedFeb 7, 2020Messages18

I still can't add hotkeys to buttons. Keymybutton1 "1" means the number 1 button? No strings are printed after pressing in the game. Is keybutton 1 "B" the correct usage?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Keymybutton1 "1", means place "1" at Keymybutton1.

Then in the Fdf-Action ControlShortcutKey you use the Key "Keymybutton1".

KeymybuttonX "B", would place "B" at KeymybuttonX

Did you hide and show the parent as shown in the example Lua code, without the hotkeys do not work.

#### KW~

#### KW~

 Level 2

JoinedFeb 7, 2020Messages18

Tasyen said:

 Keymybutton1 "1", means place "1" at Keymybutton1.

Then in the Fdf-Action ControlShortcutKey you use the Key "Keymybutton1".

KeymybuttonX "B", would place "B" at KeymybuttonX

Did you hide and show the parent as shown in the example Lua code, without the hotkeys do not work.


Click to expand...

Thank you for your reply. After careful examination, it is found that there is no empty line in the last line of the TOC file.

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Hey @Tasyen,

I tried to create a Glow-Button by code, using this tutorial and your Hero-Selector resource as a reference.

The created glowbutton works perfectly, if located in the middle of the screen.

However, it loses both the glow and the tooltip on mouse hover as soon as I move it onto the console UI at the bottom of the screen.

I'm confused, because the button is clearly layered above console UI (i.e. its perfectly visible), so why is it losing mouse hover effects?

I already "debugged" this for 2 hours without any success, so your help would be much appreciated

Best regards,

Eikonium

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Eikonium said:

 Hey @Tasyen,

I tried to create a Glow-Button by code, using this tutorial and your Hero-Selector resource as a reference.

The created glowbutton works perfectly, if located in the middle of the screen.

However, it loses both the glow and the tooltip on mouse hover as soon as I move it onto the console UI at the bottom of the screen.

I'm confused, because the button is clearly layered above console UI (i.e. its perfectly visible), so why is it losing mouse hover effects?

I already "debugged" this for 2 hours without any success, so your help would be much appreciated

Best regards,

Eikonium


Click to expand...

For some Reason SimpleFrames have higher Prio than Frames. Make sure there is not SimpleFrame which don't wants the mouse control.

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Tasyen said:

 For some Reason SimpleFrames have higher Prio than Frames. Make sure there is not SimpleFrame which don't wants the mouse control.


Click to expand...

I just realized that the issue (mouse hover effects not working) only occurs, when placing the Glue Button above the command card UI on the bottom right corner of the screen. It can be solved by hiding both all command buttons and the command card mouse deadzone.

It took me a long and painful while to realize that the command card mouse deadzone still blocks mouse input even after calling BlzHideOriginFrames() (ouch).

But I wonder, why normal command buttons also block the mouse, even if they are layered below the Gluebutton. They are not Simpleframes after all (or are they?). Do you know something about it?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Eikonium said:

 But I wonder, why normal command buttons also block the mouse, even if they are layered below the Gluebutton. They are not Simpleframes after all (or are they?). Do you know something about it?


Click to expand...

They belong to the SimpleFrame Group, this is also the case for most of the default match UI.

Excludes 3D-Portrait, Hero Has SkillPoints, Cooldowns, Minimap, Leaderboard, Multiboard, TimerDialog, and the Menues opened by the Buttons At TOPLeft, they belong to the Frame Group.

#### TTT

#### TTT

 Level 4

JoinedApr 21, 2017Messages41

Follow your tutorial, i tried to wrote a main frame including three buttons(left middle right).

but actually one button is succeed（left）, the other two are failed（editor showed ）.

is there something wrong with what i wrote?

 IncludeFile "UI\FrameDef\Glue\StandardTemplates.fdf",

IncludeFile "UI\FrameDef\UI\EscMenuTemplates.fdf",

Frame "BACKDROP" "Talent" {

 BackdropTileBackground,

 BackdropBackground "UI\Widgets\ToolTips\Human\human-tooltip-background.blp",

 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",

 BackdropCornerSize 0.012,

 BackdropEdgeFile "UI\Widgets\EscMenu\Human\human-options-menu-border.blp",

 BackdropBlendAll,

 Frame "Text" "TalentTitle" {

 FrameFont "Fonts\dfst-m3u.ttf", 0.02, "",

 FontJustificationH JUSTIFYCENTER,

 FontJustificationV JUSTIFYTOP,

 FontColor 1.0 0.756 0.145 1.0, //Red Green Blue Alpha 0.0 to 1.0,

 }

 Frame "GLUETEXTBUTTON" "TalentLeft" {

 ControlStyle "AUTOTRACK|HIGHLIGHTONMOUSEOVER",

 ControlBackdrop "TalentLeftButtonBack",

 Frame "BACKDROP" "TalentLeftButtonBack"

 {

 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",

 BackdropCornerSize 0.012,

 BackdropEdgeFile "UI\Widgets\EscMenu\Human\human-options-menu-border.blp",

 BackdropBlendAll,

 }

 ControlPushedBackdrop "TalentLeftButtonPushedBack",

 Frame "BACKDROP" "TalentLeftButtonPushedBack" {

 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",

 BackdropCornerSize 0.012,

 BackdropEdgeFile "UI\Widgets\EscMenu\Human\human-options-menu-border.blp",

 BackdropBlendAll,

 }

 ControlMouseOverHighlight "TalentLeftButtonHig",

 Frame "HIGHLIGHT" "TalentLeftButtonHig" {

 HighlightType "FILETEXTURE",

 HighlightAlphaFile "UI\Widgets\EscMenu\Human\quest-button-highlight.blp",

 HighlightAlphaMode "ADD",

 LayerStyle "IGNORETRACKEVENTS",

 HighlightColor 0.0 0.0 1.0 0.1,

 HighlightType "SHADE",

 }

 }

 Frame "GLUETEXTBUTTON" "TalentMiddle" {

 ControlStyle "AUTOTRACK|HIGHLIGHTONMOUSEOVER",

 ControlBackdrop "TalentMiddleButtonBack",

 Frame "BACKDROP" "TalentMiddleButtonBack"

 {

 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",

 BackdropCornerSize 0.012,

 BackdropEdgeFile "UI\Widgets\EscMenu\Human\human-options-menu-border.blp",

 BackdropBlendAll,

 }

 ControlPushedBackdrop "TalentMiddleButtonPushedBack",

 Frame "BACKDROP" "TalentMiddleButtonPushedBack" {

 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",

 BackdropCornerSize 0.012,

 BackdropEdgeFile "UI\Widgets\EscMenu\Human\human-options-menu-border.blp",

 BackdropBlendAll,

 }

 ControlMouseOverHighlight "TalentMiddleButtonHig",

 Frame "HIGHLIGHT" "TalentMiddleButtonHig" {

 HighlightType "FILETEXTURE",

 HighlightAlphaFile "UI\Widgets\EscMenu\Human\quest-button-highlight.blp",

 HighlightAlphaMode "ADD",

 LayerStyle "IGNORETRACKEVENTS",

 HighlightColor 0.0 0.0 1.0 0.1,

 HighlightType "SHADE",

 }

 }

 Frame "GLUETEXTBUTTON" "TalentRight" {

 ControlStyle "AUTOTRACK|HIGHLIGHTONMOUSEOVER",

 ControlBackdrop "TalentRightButtonBack",

 Frame "BACKDROP" "TalentRightButtonBack"

 {

 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",

 BackdropCornerSize 0.012,

 BackdropEdgeFile "UI\Widgets\EscMenu\Human\human-options-menu-border.blp",

 BackdropBlendAll,

 }

 ControlPushedBackdrop "TalentRightButtonPushedBack",

 Frame "BACKDROP" "TalentRightButtonPushedBack" {

 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",

 BackdropCornerSize 0.012,

 BackdropEdgeFile "UI\Widgets\EscMenu\Human\human-options-menu-border.blp",

 BackdropBlendAll,

 }

 ControlMouseOverHighlight "TalentRightButtonHig",

 Frame "HIGHLIGHT" "TalentRightButtonHig" {

 HighlightType "FILETEXTURE",

 HighlightAlphaFile "UI\Widgets\EscMenu\Human\quest-button-highlight.blp",

 HighlightAlphaMode "ADD",

 LayerStyle "IGNORETRACKEVENTS",

 HighlightColor 0.0 0.0 1.0 0.1,

 HighlightType "SHADE",

 }

 }

}


Click to expand...

 function Trig_______UI__________tfcshActions takes nothing returns nothing

 call DzLoadToc( "UI\\myUIList.toc" )

 // ----- 主框 ------

 set udg_a_Talent[1] = DzCreateFrame("Talent", DzGetGameUI(), 0)

 call DzFrameSetSize( udg_a_Talent[1], 0.31, 0.23 )

 call DzFrameSetPoint( udg_a_Talent[1], 4, DzGetGameUI(), 4, 0.00, 0.10 )

 // ------ 标题 -----

 set udg_a_Talent[2] = DzFrameFindByName("TalentTitle", 0)

 call DzFrameSetSize( udg_a_Talent[2], 0.30, 0.05 )

 call DzFrameSetText( udg_a_Talent[2], "请选择你的开局天赋" )

 call DzFrameSetPoint( udg_a_Talent[2], 1, udg_a_Talent[1], 1, 0.00, -0.02 )

 // ------ 天赋1 ------

 set udg_a_Talent[3] = DzFrameFindByName("TalentLeft", 0)

 call DzFrameSetSize( udg_a_Talent[3], 0.05, 0.05 )

 call DzFrameSetPoint( udg_a_Talent[3], 3, udg_a_Talent[1], 3, 0.03, 0.02 )

 // ---- 正常 ----

 set udg_a_Talent[4] = DzFrameFindByName("TalentLeftButtonBack", 0)

 call DzFrameSetTexture( udg_a_Talent[4], "ReplaceableTextures\\CommandButtons\\BTNArthas.blp", 0 )

 // ---- 不正常 ----

 set udg_a_Talent[5] = DzFrameFindByName("TalentLeftButtonPushedBack", 0)

 call DzFrameSetTexture( udg_a_Talent[5], "ReplaceableTextures\\CommandButtons\\DISBTNArthas.blp", 0 )

 // ------ 天赋2 ------

 set udg_a_Talent[6] = DzFrameFindByName("TalentMiddle", 0)

 call DzFrameSetSize( udg_a_Talent[6], 0.05, 0.05 )

 call DzFrameSetPoint( udg_a_Talent[6], 3, udg_a_Talent[1], 3, 0.13, 0.02 )

 // ---- 正常 ----

 set udg_a_Talent[7] = DzFrameFindByName("TalentMiddleButtonBack", 0)

 call DzFrameSetTexture( udg_a_Talent[7], "ReplaceableTextures\\CommandButtons\\BTNArthas.blp", 0 )

 // ---- 不正常 ----

 set udg_a_Talent[8] = DzFrameFindByName("TalentMiddleButtonPushedBack", 0)

 call DzFrameSetTexture( udg_a_Talent[8], "ReplaceableTextures\\CommandButtons\\DISBTNArthas.blp", 0 )

 // ------ 天赋3 ------

 // ---- 正常 ----

 // ---- 不正常 ----

 // ----- 主框显示 ------

 call DzFrameShow( udg_a_Talent[1], true )

 // ewq

 // 111111111111

endfunction


Click to expand...

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Your Highlight-Frames have twice the fdf-action:

HighlightType

#### TTT

#### TTT

 Level 4

JoinedApr 21, 2017Messages41

Tasyen said:

 Your Highlight-Frames have twice the fdf-action:

HighlightType


Click to expand...

thank you

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

There is another way to create an IconButton in Warcraft 3 V1.32.6+ which even has the shrinking while it is hold down, by map script only.

One Creates a ScriptDialogButton and gets the functional child backdrops of it. Then one sets the textures and changes the position of the child at index 1 (pushed state).

Edit: or it is better to make the default State 0.01 bigger, because of fdf settings it is displayed smaller.

 Lua:

-- create a new BUTTON inheriting from "ScriptDialogButton". With the inherit one gains a yellow on mouse hover glowing and a backdrop for each state Default Pushed, disabled
local button = BlzCreateFrameByType("GLUETEXTBUTTON", "MyIconButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "ScriptDialogButton", 0)
-- place the Button to the left center of the Screen
BlzFrameSetAbsPoint(button, FRAMEPOINT_CENTER, 0.1, 0.3)
-- set the Button's Size
BlzFrameSetSize(button, 0.049, 0.049)
-- Backdrop for default State
local buttonIconFrame = BlzFrameGetChild(button, 0)
-- Backdrop for pushed State
local buttonIconPushedFrame = BlzFrameGetChild(button, 1)
-- Backdrop for disabled State
local buttonIconDisabledFrame = BlzFrameGetChild(button, 2)

-- set the textures
BlzFrameSetTexture(buttonIconFrame, BlzGetAbilityIcon(FourCC('Hpal')), 0, false)
BlzFrameSetTexture(buttonIconPushedFrame, BlzGetAbilityIcon(FourCC('Hpal')), 0, false)

-- make the pushedIconFrame smaller, it copies the button so it has to be unbind first
BlzFrameClearAllPoints(buttonIconPushedFrame)
BlzFrameSetPoint(buttonIconPushedFrame, FRAMEPOINT_BOTTOMLEFT, button, FRAMEPOINT_BOTTOMLEFT, 0.005, 0.005)
BlzFrameSetPoint(buttonIconPushedFrame, FRAMEPOINT_TOPRIGHT, button, FRAMEPOINT_TOPRIGHT, -0.005, -0.005)


 Last edited: Jul 10, 2022

#### AdaptiveTrigger

#### AdaptiveTrigger

 Level 4

JoinedJun 17, 2016Messages18

Resources: 1

#### Resources
Maps 1

Is it possible to place a clickable unit icon/button on the top left portion of the screen (where the hero icons show up — I believe this is the ORIGIN_FRAME_HERO_BAR or ORIGIN_FRAME_HERO_BUTTON part of the screen).

I was attempting to create my own hero icons (with health and mana bars), but for units that aren't actually heroes.

However, the button would just disappear if I moved it there. It would also just get stretched so that it couldn't overlap into that section of the screen. Maybe I'm doing something wrong, but I'm getting the sense that buttons just can't be placed there?


 Last edited: Sep 5, 2022

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

The Frames created as child of GameUI can not go further left than x = 0, nor further right than x = 0.8.

Need to create your button as child/offspring of a Frame that can leave the 4:3 Screen. Like BlzGetFrameByName("ConsoleUIBackdrop",0) or BlzGetFrameByName("Leaderboard", 0).

BlzGetFrameByName("ConsoleUIBackdrop",0) (Reforged only ,also below Console)

BlzGetFrameByName("Leaderboard",0) (Need to create and show a Leaderboard first using the Leaderboard api)

BlzGetFrameByName("Multiboard", 0) ^^^^^^

#### lanchengkai

#### lanchengkai

 Level 2

JoinedJul 14, 2022Messages4

how to change the clicked sound

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

You must use Button instead of Gluebutton (the latter has automated clicking sound).

And then create a trigger on the frame with the event: BlzTriggerRegisterFrameEvent.

In the trigger activation you can make it play a sound.

#### ThompZon

#### ThompZon

 Level 28

JoinedJan 30, 2020Messages857

Resources: 20

#### Resources
Models 10Icons 3Packs 1Skins 1Maps 2Spells and Systems 1Ported Icons 2

I tried the Leaderboard method putting clickable button above the "Attack"-command-icon, but it didn't seem to work to get it clickable.

I want a small button above the command-icons that pops up "hidden stats" on my map.

It is not clickable, even when having a leaderboard as a parent (except for the sliver of the button that is outside ConsoleUI, where the mouse is hovering in the image).

I've tried several different parents and no one can click below there I hover the mouse. Is it possible to make it more clickable?

Note: I'm on 1.31. Does that matter?

 JASS:

library DTCCustomStatFrames initializer InitHeroTalentButton requires DTCUtils, DTCStringUtils
 globals
 private constant real STAT_FRAME_WIDTH = 0.155
 private constant real STAT_FRAME_HEIGHT = 0.111
 private constant real STAT_TEXT_INSET = 0.022

 private framehandle statbackground = null
 private framehandle statText = null
 private framehandle statButton = null
 unit customStatUnit = null //async variable!!
 endglobals
 private function getCDR takes unit u returns string
 local real haste = GetSpellHaste(u)
 return R2Sx100(haste / (haste + 100.))
 endfunction
 function updateSelectedUnit takes nothing returns nothing
 local string statStr1
 local string statStr2
 local string statStr3
 local string statStr4
 if customStatUnit != null then
 set statStr1 = "|c00FF7800Damage: " + R2I2S(GetTotalDamage(customStatUnit)) + "|r|nMovement Speed: " + R2I2S(GetUnitMoveSpeed(customStatUnit))
 set statStr2 = "|n|c00FFC899Attack Speed: " + R2Sx100(GetTotalAttackSpeed(customStatUnit)) + "%%|nAttack Cooldown: " + R2SW(GetAttackCooldown(customStatUnit), 0, 2)
 set statStr3 = "s|n|c00DF00FFSpell Power: " + I2S(GetSpellPower(customStatUnit)) + "|n|c00D2B2FFSpell Haste: " + R2I2S(GetSpellHaste(customStatUnit)) + "|nCooldown Reduction: " + getCDR(customStatUnit)
 set statStr4 = "%%|n|c00FF4840Critical Hit Chance: " + R2Sx100(GetCriticalHitChanceInclAgi(customStatUnit)) + "%%|nCritical Hit Damage: " + R2Sx100(GetCriticalHitDamageInclAgi(customStatUnit)) + "%%"
 call BlzFrameSetText(statText, statStr1 + statStr2 + statStr3 + statStr4)
 endif
 endfunction
 function SetStatDisplayHero takes unit u returns nothing
 set customStatUnit = u
 endfunction
 private function toggleStatsFrame takes nothing returns nothing
 call BlzFrameSetEnable(statButton, false)
 call BlzFrameSetEnable(statButton, true)
 call BlzFrameSetVisible(statbackground, not BlzFrameIsVisible(statbackground))
 call BJDebugMsg("Toggle frame")
 endfunction
 function DelayedInitHeroTalentButton takes nothing returns nothing
 local trigger toggleStats = CreateTrigger()
 //Move Hero-primary-stat-icon
 local framehandle heroIcon = BlzGetFrameByName("InfoPanelIconHeroIcon", 6)
 local framehandle levelBar = BlzGetFrameByName("SimpleHeroLevelBar", 0)
 local framehandle leaderboardFrame
 //Dummy leaderboard to get a parent.
 call CreateLeaderboardBJ(bj_FORCE_ALL_PLAYERS, "title")
 set leaderboardFrame = BlzGetFrameByName("Leaderboard", 0)
 call BlzFrameSetSize(leaderboardFrame, 0, 0)
 call BlzFrameSetVisible(BlzGetFrameByName("LeaderboardBackdrop", 0), false)
 call BlzFrameSetVisible(BlzGetFrameByName("LeaderboardTitle", 0), false)

 call DestroyTimer(GetExpiredTimer())
 //Move hero primary-stat icon to give space for a talent-button
 call BlzFrameClearAllPoints(heroIcon)
 call BlzFrameSetPoint(heroIcon, FRAMEPOINT_TOP, levelBar, FRAMEPOINT_BOTTOM, 0.024, -0.006)
 //Stats Panel:
 set statbackground = BlzCreateFrame("ListBoxWar3", leaderboardFrame, 0, 0)
 set statText = BlzCreateFrameByType("TEXT", "statText", statbackground, "StandardInfoTextTemplate", 0)
 call BlzFrameSetAbsPoint(statbackground, FRAMEPOINT_BOTTOMRIGHT, 0.8, 0.16)
 //call BlzFrameSetPoint(statbackground, FRAMEPOINT_RIGHT, leaderboardFrame, FRAMEPOINT_RIGHT, 0., 0.)
 call BlzFrameSetSize(statbackground, STAT_FRAME_WIDTH, STAT_FRAME_HEIGHT)
 call BlzFrameClearAllPoints(statText)
 call BlzFrameSetText(statText, "Cooldown Reduction: XX.0%%|n2|n3|n4|n5|n6|n7|n8")
 call BlzFrameSetSize(statText, STAT_FRAME_WIDTH - STAT_TEXT_INSET, STAT_FRAME_HEIGHT - STAT_TEXT_INSET)
 //call BlzFrameSetPoint(statText, FRAMEPOINT_CENTER, statbackground, FRAMEPOINT_CENTER, 0., 0.)
 call BlzFrameSetPoint(statText, FRAMEPOINT_BOTTOM, statbackground, FRAMEPOINT_BOTTOM, 0., 0.01)
 call TimerStart(CreateTimer(), 0.4, true, function updateSelectedUnit)
 call BlzFrameSetVisible(statbackground, false)

 //Stats-panel-toggle-button
 set statButton = BlzCreateFrameByType("GLUETEXTBUTTON", "statsButton", leaderboardFrame, "ScriptDialogButton", 0)
 //set statButton = BlzCreateFrameByType("GLUETEXTBUTTON", "statsButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "ScriptDialogButton", 0)
 //set statButton = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0),0,0)
 //set statButton = BlzCreateFrame("ScriptDialogButton", BlzGetFrameByName("ConsoleUI", 0),0,0)
 //set statButton = BlzCreateFrame("ScriptDialogButton", BlzGetFrameByName("ConsoleUIBackdrop", 0),0,0)
 call BlzFrameSetAbsPoint(statButton, FRAMEPOINT_BOTTOMRIGHT, 0.8, 0.135)
 call BlzFrameSetSize(statButton, 0.06, 0.035)
 call BlzFrameSetText(statButton, "|cffFCD20DStats|r")
 call BlzFrameSetScale(statButton, 0.75)
 call BlzFrameSetLevel(statButton, 10)
 call BlzTriggerRegisterFrameEvent(toggleStats, statButton, FRAMEEVENT_CONTROL_CLICK)
 call TriggerAddAction(toggleStats, function toggleStatsFrame)
 //call BlzFrameSetVisible(BlzGetFrameByName("ConsoleUI", 0), true)

 call BJDebugMsg("Successfully init")

 set toggleStats = null
 set leaderboardFrame = null
 set heroIcon = null
 set levelBar = null
 endfunction
 function InitHeroTalentButton takes nothing returns nothing
 //call DelayedInitHeroTalentButton()
 call TimerStart(CreateTimer(), 0.4, false, function DelayedInitHeroTalentButton)
 endfunction
endlibrary

Edit: I got my answer on Discord. This spot is covery by a invisible simpleframe taking mouseinput. The Blocking SimpleFrame is only accessible by Child-api (I.E. not 1.31.1). Can make a simplebutton instead that needs fdf, but I chose to move the button.


 Last edited: Apr 29, 2023

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

That spot is taken by a invisible simpleframe which covers the default command card position and a little bit more. That simpleframe has a higher prio than most non simpleframes and hence they are not interactive at that spot.

With Frame child api one could disable it without sideeffects, ConsoleUI[5].

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Added a jass demo map which creates an custom icon button which can be used to reselect or pan to the last selected own unit. The demo map was meant to be an own tutorial, but no won't do that but it might still be good as this tutorial lacks a demo map and someone was asking for demo map. The map was small enough ~80 lines of jass

Also added a Lua demo map. it shows buttons with fdf hotkeys. 1 2 & 3


 Last edited: Aug 22, 2023

#### emperor_d3st

#### emperor_d3st

 Level 18

JoinedApr 13, 2008Messages1,629

Resources: 2

#### Resources
Ported Models 2

Thank you very much for your dissertations on these topics.

I tried to register for my trigger FRAMEEVENT_MOUSE_DOWN

It does not work. After some googling it seems like this event does not work for any frame type due to some bug. So avoid it.


 Last edited: Sep 20, 2023

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

When one just wants to have a frame that does something onMouse click on some part of the screen. Then TEXT is even better than button as it does not even keep mouse focus, nor stops it hotkeys. When one adds a ControlStyle, then the event happens on mouse down.

 Code:

Frame "TEXT" "ClickFrame" {
 ControlStyle "CLICKONMOUSEDOWN",
}

Edit: TEXT even supports the backdrop states like button. Therefore TEXT can be a BUTTON replacement.

 Code:

Frame "TEXT" "TextB" {
 ControlStyle "CLICKONMOUSEDOWN",
 ControlBackdrop "ButtonBackdropTemplate",
 Frame "BACKDROP" "ButtonBackdropTemplate" {
 BackdropBackground "ReplaceableTextures\CommandButtons\BTNSorceress.tga",
 }
 ControlPushedBackdrop "ButtonPushedBackdropTemplate",
 Frame "BACKDROP" "ButtonPushedBackdropTemplate" {
 BackdropBackground "ReplaceableTextures\CommandButtons\BTNFootman.tga",
 }
}

Edit: TEXT needs to contain any text to show the icons.

#### Attachments

Text the Button.w3x

 16.6 KB

 · Views: 102




 Last edited: Nov 17, 2023

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

 UI: Frames and Tooltips


Tasyen

Jun 27, 2019

Scripting Tutorials

Replies45Views21K

Oct 27, 2024
RPG_F

 UI: Reading a FDF


Tasyen

May 29, 2019

Scripting Tutorials

Replies15Views16K

Dec 1, 2020
Tasyen

 UI: TEXTAREA the scrolling Text Frame


Tasyen

Sep 20, 2019

Scripting Tutorials

Replies20Views9K

Dec 6, 2021
Seago

S

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