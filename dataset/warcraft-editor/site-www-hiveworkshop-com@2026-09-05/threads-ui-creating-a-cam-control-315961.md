---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-creating-a-cam-control.315961/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Creating a Cam Control
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-creating-a-cam-control.315961/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

#
 Introduction

In this tutorial, I wanna show you, Sliders, Checkboxes and attaching Labels to the Sliders and the Checkbox. The sliders are used to control the fields of the cam. The checkbox will lock the current settings, preventing changes to the sliders as long it is checked. And the Labels will show information about the frames they are attached to.​

#
 What is a slider?

A slider is a frame for user input. It allows the user to choose a value between an upper and lower limit. You can define for each slider the limits as you please. A Sliders value is local and can differ for all players.

There exist 2 mainFrame Sliders in the default fdf. "EscMenuSliderTemplate" and "StandardSliderTemplate". Both are in fdfs not beeing loaded on default.​

#
 What is a checkbox?

A checkbox is a frame that toggles between 2 states. The checked and unchecked state. Checking and Unchecking are both own independent events. A checkbox checked state is local and can differ for each player.​

#
 What is a Label?

A Label is not a frameType its type is "TEXT", this "TEXT"-frames are used to display text, but also take over space and can throw frameevents when clicking or hovering them with the mouse.​

#
 Writing the toc

Cause there is no default loaded mainFrame of type Slider loaded. We first have to create a toc file loading the needed fdf containing the sliders.

We create a new toc-File, named "templates.toc". Its content shall be the text in the next box. Make sure that you add an empty line at the end (on the ptr my tocs failed to load without that empty line).

The toc-files text is not case sensitive.

 Code:

UI\FrameDef\Glue\standardtemplates.fdf
UI\FrameDef\UI\escmenutemplates.fdf
UI\FrameDef\Glue\battlenettemplates.fdf

After you created "templates.toc", wrote its content and saved it. Import the toc-file into your map, keep the path as it is.

Now we need to load the toc file in the maps code, I prefer to know if a toc-Load failed and write an additional function telling me that.

 JASS:

function LoadToc takes string s returns nothing
 if BlzLoadTOCFile(s) then
 call BJDebugMsg("Loaded: "+s)
 else
 call BJDebugMsg("Failed to Load: "+s)
 endif
endfunction

This is the code that will call the custom LoadToc function, which will load the tocFile being placed at "war3mapimported\\templates.toc" (in jass one has to write \\ to have \).

call LoadToc("war3mapimported\\templates.toc")

Now that the toc is loaded, we can start creating the sliders. In this tutorial, I use "EscMenuSliderTemplate".

I planed to create 3 sliders, allowing to change cam-distance, cam angle of attack and cam rotation.​

##
 Distance Slider + Label

Lets start with 1 slider and its label.

This code creates the slider "distance" and its label.

The sliders parent is the gameUI, but the labels parent is the slider. With the slider beeing the parent of the label, the label shares many actions applied to the slider, like visibility, scale ....

 JASS:

function CreateSliderDistance takes nothing returns nothing
 local framehandle fh = BlzCreateFrame("EscMenuSliderTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 local framehandle label = BlzCreateFrame("EscMenuLabelTextTemplate", fh, 0, 0)
 call BlzFrameSetPoint(label, FRAMEPOINT_LEFT, fh, FRAMEPOINT_RIGHT, 0, 0)
 call BlzFrameSetAbsPoint(fh, FRAMEPOINT_LEFT, 0.02, 0.5) //below the menu, quest buttons

 //call BlzFrameSetSize(fh, 0.139, 0.012) //default size in the fdf.

 call BlzFrameSetMinMaxValue(fh, 400, 3000) //limits user can choose; 400 to 3000
 call BlzFrameSetValue(fh, 1650) //starting value, should be used after one changed min max
 call BlzFrameSetStepSize(fh, 50) //value change from the previous value; how accurate the user can pick values.
endfunction

This is a function that will be constantly be executed by a timer. It will frequently read the current chosen value and set the cams distance to that value. It also updates the Labels text so we know the chosen value.

We read the slider by using BlzGetFrameByName using the frames name (EscMenuSliderTemplate) and its create context (0).

 JASS:

function UpdateCam takes nothing returns nothing
 call SetCameraField(CAMERA_FIELD_TARGET_DISTANCE, BlzFrameGetValue(BlzGetFrameByName("EscMenuSliderTemplate", 0)), 0)
 call BlzFrameSetText(BlzGetFrameByName("EscMenuLabelTextTemplate", 0), "Distance: " + R2SW(BlzFrameGetValue(BlzGetFrameByName("EscMenuSliderTemplate", 0)), 1, 1))
endfunction

Now we create another function which will start the timer doing the updating and call the functions doing the creation and loading.

 JASS:

function CreateCamControll takes nothing returns nothing
 call LoadToc("war3mapimported\\templates.toc")
 call CreateSliderDistance()
 call TimerStart(CreateTimer(), 0.4, true, function UpdateCam)
endfunction

The result of the current code should look somehow like that.

​

##
 Create other Sliders

Now that the first slider was a success we add 2 further sliders with labels.

The difference between the new sliders and the first slider is the createcontext and changed slider settings. We need to use unique createcontext numbers for frames using the same names to be able to access all of them with BlzGetFrameByName.

 JASS:

function UpdateCam takes nothing returns nothing
 call SetCameraField(CAMERA_FIELD_TARGET_DISTANCE, BlzFrameGetValue(BlzGetFrameByName("EscMenuSliderTemplate", 0)), 0)
 call SetCameraField(CAMERA_FIELD_ANGLE_OF_ATTACK, BlzFrameGetValue(BlzGetFrameByName("EscMenuSliderTemplate", 1)), 0)
 call SetCameraField(CAMERA_FIELD_ROTATION, BlzFrameGetValue(BlzGetFrameByName("EscMenuSliderTemplate", 2)), 0)
 call BlzFrameSetText(BlzGetFrameByName("EscMenuLabelTextTemplate", 0), "Distance: " + R2SW(BlzFrameGetValue(BlzGetFrameByName("EscMenuSliderTemplate", 0)), 1, 1))
 call BlzFrameSetText(BlzGetFrameByName("EscMenuLabelTextTemplate", 1), "Angle of Attack: " + R2SW(BlzFrameGetValue(BlzGetFrameByName("EscMenuSliderTemplate", 1)), 1, 1))
 call BlzFrameSetText(BlzGetFrameByName("EscMenuLabelTextTemplate", 2), "Rotation: " + R2SW(BlzFrameGetValue(BlzGetFrameByName("EscMenuSliderTemplate", 2)), 1, 1))
endfunction

function CreateSliderAngleOfAttack takes nothing returns nothing
 local framehandle fh = BlzCreateFrame("EscMenuSliderTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 1) //we use CreateContext 1, so the new slider will not overwrite the one saved in slot 0. CreateContext defines the integer the new frame and its children will use in the frame storage. We can read the frame storage with BlzGetFrameByName
 local framehandle label = BlzCreateFrame("EscMenuLabelTextTemplate", fh, 0, 1)
 call BlzFrameSetPoint(label, FRAMEPOINT_LEFT, fh, FRAMEPOINT_RIGHT, 0, 0)
 call BlzFrameSetAbsPoint(fh, FRAMEPOINT_LEFT, 0.02, 0.475) //below the menu, quest buttons

 call BlzFrameSetMinMaxValue(fh, 0, 360) //limits user can choose
 call BlzFrameSetValue(fh, 304) //startin value
 call BlzFrameSetStepSize(fh, 2) //
endfunction

function CreateSliderRotation takes nothing returns nothing
 local framehandle fh = BlzCreateFrame("EscMenuSliderTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 2) //CreateContext 2
 local framehandle label = BlzCreateFrame("EscMenuLabelTextTemplate", fh, 0, 2)
 call BlzFrameSetPoint(label, FRAMEPOINT_LEFT, fh, FRAMEPOINT_RIGHT, 0, 0)
 call BlzFrameSetAbsPoint(fh, FRAMEPOINT_LEFT, 0.02, 0.45) //below the menu, quest buttons

 call BlzFrameSetMinMaxValue(fh, 0, 360) //limits user can choose
 call BlzFrameSetValue(fh, 90) //startin value
 call BlzFrameSetStepSize(fh, 5) //

endfunction

function CreateCamControl takes nothing returns nothing
 call LoadToc("war3mapimported\\templates.toc") //use the custom function to load the Toc, the custom function prints success/fail loading the toc.
 call CreateSliderAngleOfAttack()
 call CreateSliderRotation()
 call CreateSliderDistance()
 call TimerStart(CreateTimer(), 0.4, true, function UpdateCam)
endfunction

Nice now we can change all 3 of them in real time in an ui.

But after having messed around with the settings something stupid like that might happen.

Might be good to add a reset button changing the sliders back to default.

Also it might be good to be able to block changes to the current choosen settings, if one found a good setting and want to protect that against easy missclicks. Lets add a checkbox that can be clicked, when beeing checked the sliders current values can not be changed. Also we add a button that will reset the sliders and with that cam settings.​

##
 Checkbox and Reset

Creating the CheckBox locking the sliders.

 JASS:

//The function beeing executed when the checkbox is checked/UnChecked.
function CheckBoxLockSliders takes nothing returns nothing
 local boolean enable = (BlzGetTriggerFrameEvent() == FRAMEEVENT_CHECKBOX_UNCHECKED) //calc the new state,
 if GetLocalPlayer() == GetTriggerPlayer() then //only do stuff for local player
 call BlzFrameSetEnable(BlzGetFrameByName("EscMenuSliderTemplate", 0), enable)
 call BlzFrameSetEnable(BlzGetFrameByName("EscMenuSliderTemplate", 1), enable)
 call BlzFrameSetEnable(BlzGetFrameByName("EscMenuSliderTemplate", 2), enable)
 endif
endfunction

function CreateCheckbox takes nothing returns nothing
 local trigger trig = CreateTrigger()
 local framehandle fh = BlzCreateFrame("QuestCheckBox", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 3) //CreateContext 3 would not be needed for the checkbox cause it uses different names, but we want to use the same labelFrame Type.
 local framehandle label = BlzCreateFrame("EscMenuLabelTextTemplate", fh, 0, 3)
 call BlzFrameSetPoint(label, FRAMEPOINT_LEFT, fh, FRAMEPOINT_RIGHT, 0, 0)
 call BlzFrameSetAbsPoint(fh, FRAMEPOINT_LEFT, 0.15, 0.40)
 call BlzFrameSetText(label, "Lock Sliders")
 call TriggerAddAction(trig, function CheckBoxLockSliders)
 call BlzTriggerRegisterFrameEvent(trig, fh, FRAMEEVENT_CHECKBOX_CHECKED) //execute function when checking the box
 call BlzTriggerRegisterFrameEvent(trig, fh, FRAMEEVENT_CHECKBOX_UNCHECKED) //executed when unchecking
endfunction

The code handling the resetbutton, its a function that is executed when the button is pressed and a function creating the button.

 JASS:

function ResetSliders takes nothing returns nothing
 if GetLocalPlayer() == GetTriggerPlayer() then
 call BlzFrameSetValue(BlzGetFrameByName("EscMenuSliderTemplate", 0), 1650)
 call BlzFrameSetValue(BlzGetFrameByName("EscMenuSliderTemplate", 1), 304)
 call BlzFrameSetValue(BlzGetFrameByName("EscMenuSliderTemplate", 2), 90)
 endif

 call BlzFrameSetEnable(BlzGetTriggerFrame(), false)//this button loses focus
 call BlzFrameSetEnable(BlzGetTriggerFrame(), true)
endfunction

function CreateResetButton takes nothing returns nothing
 local trigger trig = CreateTrigger()
 local framehandle fh = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0) //CreateContext 0, we can reuse it cause it has unique names compread to the sliders and labels.
 call BlzFrameSetSize(fh, 0.09, 0.024) //default size is to big
 call BlzFrameSetAbsPoint(fh, FRAMEPOINT_RIGHT, 0.15, 0.40)
 call BlzFrameSetText(fh, "Reset Cam")
 call TriggerAddAction(trig, function ResetSliders)
 call BlzTriggerRegisterFrameEvent(trig, fh, FRAMEEVENT_CONTROL_CLICK)
endfunction

We also need to update the create function CreateCamControl.

 JASS:

function CreateCamControl takes nothing returns nothing
 call LoadToc("war3mapimported\\templates.toc") //use the custom function to load the Toc, the custom function prints success/fail loading the toc.
 call CreateSliderAngleOfAttack()
 call CreateSliderRotation()
 call CreateSliderDistance()
 call CreateCheckbox()
 call CreateResetButton()
 call TimerStart(CreateTimer(), 0.4, true, function UpdateCam)
endfunction

Now the created frames should look like that.

The framedefinition wouldn't be needed for that tutorial, but now you don't have to search it, if you wana look on the slider definition.

 Code:

Frame "SLIDER" "EscMenuSliderTemplate" {
 Height 0.012,
 Width 0.139,
 SliderLayoutHorizontal,

 ControlBackdrop "EscMenuScrollBarBackdropTemplate",
 Frame "BACKDROP" "EscMenuScrollBarBackdropTemplate" {
 DecorateFileNames,
 BackdropTileBackground,
 BackdropBackground "EscMenuSliderBackground",
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.006,
 BackdropBackgroundSize 0.006,
 BackdropBackgroundInsets 0.0025 0.0025 0.0025 0.0025,
 BackdropEdgeFile "EscMenuSliderBorder",
 BackdropBlendAll,
 }

 ControlDisabledBackdrop "EscMenuScrollBarDisabledBackdrop",
 Frame "BACKDROP" "EscMenuScrollBarDisabledBackdrop" {
 DecorateFileNames,
 BackdropTileBackground,
 BackdropBackground "EscMenuSliderBackground",
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.006,
 BackdropBackgroundSize 0.006,
 BackdropBackgroundInsets 0.0025 0.0025 0.0025 0.0025,
 BackdropEdgeFile "EscMenuSliderDisabledBorder",
 BackdropBlendAll,
 }

 SliderThumbButtonFrame "EscMenuThumbButtonTemplate",
 Frame "BUTTON" "EscMenuThumbButtonTemplate" {
 Width 0.016,
 Height 0.016,

 ControlBackdrop "EscMenuThumbButtonBackdropTemplate",
 Frame "BACKDROP" "EscMenuThumbButtonBackdropTemplate" {
 DecorateFileNames,
 BackdropBlendAll,
 BackdropBackground "EscMenuSliderThumbButton",
 }

 ControlDisabledBackdrop "EscMenuThumbButtonDisabledBackdrop",
 Frame "BACKDROP" "EscMenuThumbButtonDisabledBackdrop" {
 DecorateFileNames,
 BackdropBlendAll,
 BackdropBackground "EscMenuSliderDisabledThumbButton",
 }
 }
}

 Code:

Frame "TEXT" "EscMenuLabelTextTemplate" {
 DecorateFileNames,
 FrameFont "EscMenuTextFont", 0.011, "",
 FontJustificationH JUSTIFYLEFT,
 FontJustificationV JUSTIFYMIDDLE,
 FontFlags "FIXEDSIZE",
 FontColor 0.99 0.827 0.0705 1.0,
 FontHighlightColor 1.0 1.0 1.0 1.0,
 FontDisabledColor 0.2 0.2 0.2 1.0,
 FontShadowColor 0.0 0.0 0.0 0.9,
 FontShadowOffset 0.002 -0.002,
}

The 3 attached Maps are the different states of the tutorial.

CamSlider1 is after the first slider was created.

CamSlider2 is after all 3 sliders were created.

CamSlider3 has 3 sliders the reset and the Sliderlock.​

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

Cam Sliders1.w3x

 17.6 KB

 · Views: 374



Cam Sliders2.w3x

 18.5 KB

 · Views: 418



Cam Sliders3.w3x

 19.9 KB

 · Views: 505




 Last edited by a moderator: Oct 9, 2020

#### The_Silent

#### The_Silent

 Level 46

JoinedFeb 4, 2008Messages3,511

Resources: 204

#### Resources
Models 63Icons 115Packs 8Skins 16Maps 1Ported Models 1

Is there a vertical slider?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

yes, one can create vertical sliders. But none of the predefined sliders is vertical. So one would have to define a custom fdf with such a vertical slider. One copies one of the predefined sliders changes:

SliderLayoutHorizontal -> SliderLayoutVertical

swaps values for Height, Width

one has to give the mainframe a different unique name.

Then one needs to create a toc loading that vertical slider file into your map.

Its more simple as it sounds. But vertical sliders might have a problem with labels.

Maybe one could use "SCROLLBAR"s for that but that might have unwanted sideeffects.

#### KeepVary

#### KeepVary

 Level 3

JoinedMay 12, 2019Messages12

I noticed that a slider can respond with mouse wheel event, but I didn't find a way to get mouse wheel's direction.

Am I missed something? Or Blz forgot this?

#### Coldgamer

#### Coldgamer

 Level 4

JoinedJun 4, 2019Messages37

Thanks for your Tutorial

Got a question about Multiplayer:

Is there any option to realize a scrollbar for every player?

For example, with the button, you could query the player (getPlayer) who had pressed the button.

But that wouldn't make sense with the slider.

Any suggestions?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

I didn't tested warcraft 3 scrollbars in depth yet.

What do you mean in detail? I kinda fail to understand the message you want to tell/ask here. Somehow sounds like a readycheck, a progressbar or something different. If you mean a bar that fills over time (progressbar), that is not a scrollbar. Creating such progressbars will be a tutorial soon.


 Last edited: Jun 12, 2019

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Looks awfully good. Does this basically do what DGUI failed to?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

yes, the frame natives allows to create new useable screen objects (frames) to which you can bind events and therefore executive code. Frames can also just be a visual thing to show something, Like a image/model/text.

One can also manipulate the existing default warcraft 3 ingame UI using the frame natives.

#### Coldgamer

#### Coldgamer

 Level 4

JoinedJun 4, 2019Messages37

Tasyen said:

 I didn't tested warcraft 3 scrollbars in depth yet.

What do you mean in detail? I kinda fail to understand the message you want to tell/ask here. Somehow sounds like a readycheck, a progressbar or something different. If you mean a bar that fills over time (progressbar), that is not a scrollbar. Creating such progressbars will be a tutorial soon.


Click to expand...

Sorry, my question was maybe confusing, didn't mean scrollbar -> i meant Slider

I just want to give every player in some orpg for example the option to change his camera distance like in your example.

But what i so far understand is, that everyone see the same slider, so if one player is changing the value everyone would be affected by this.

Just wondering if it's possible for every player to get their own slider.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Yes, all have and see the same slider. But its value and state is not shared to other players. Its all localy and can differ as long the frames data is not used in a synced manner, frames should still be create for all players. Setting camara is also a local action.

This example should work in multiplayer without any problem. For a real use case in a map, its a bit bad that the sliders are not relative to each other or are not in a new parent frame so its easy to hide them when wanted. But I wanted to keep that out of this tutorial.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Seems like I got catching up to do then, did not know the UI natives were that powerful.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

When one wants vertical sliders without a custom fdf and one is fine with the escmenu textures. Then the scrollbars from questdialog can be inherited to create such vertical sliders without any custom toc. Looks like that:

 JASS:

function Trig_At_0s_Actions takes nothing returns nothing
// create a vertical slider by inheriting from a Scrollbar. It will use esc menu textures
 local framehandle sliderFrame = BlzCreateFrameByType( "SLIDER", "TestSlider", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI,0), "QuestMainListScrollBar", 0 )
// clear the inherited attachment
 call BlzFrameClearAllPoints(sliderFrame)
// set pos and size
 call BlzFrameSetAbsPoint(sliderFrame, FRAMEPOINT_TOPLEFT, 0.40, 0.30 )
 call BlzFrameSetSize(sliderFrame, 0.012, 0.06 )
endfunction
//===========================================================================
function InitTrig_At_0s takes nothing returns nothing
 set gg_trg_At_0s = CreateTrigger( )
 call TriggerRegisterTimerEventSingle( gg_trg_At_0s, 0.0 )
 call TriggerAddAction( gg_trg_At_0s, function Trig_At_0s_Actions )
endfunction

#### Attachments

Vertical Slider.w3m

 16.3 KB

 · Views: 237



#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

Wow another of these never ending new possibilities I missed after 16 years of mapmaking retirement ^^

It is really amazing, although I admit having to understand so many new things that didn't even exist in map makers dreams back in the days is sometimes a bit heavy on my "brain load"

Anyways :

@Tasyen : this is extremely clever, thanks for that vertical slider... really brilliant, and I mean, no custom .loc needed... makes it even better in this context.

Thats exactly what I was looking for to replace the old ZoomOut cinematic Camera in my map.

This said, the questions asked by ColdGamer are of extreme importance for anyone who would like to make these sliders.

So when yu say "and can differ as long the frames data is not used in a synced manner, frames should still be create for all players", you mean that we can set one of the camera fileds (the one we want the slider to change) to the value of the slider, but we can not register FRAMEEVENT_SLIDER_VALUE_CHANGED to a Trigger in a multiplayer context ? Or that we can actualy register this event but not read the value from the event response BlzGetTriggerFrameValue ?

I don't like timed events when I can avoid them, would it be possible to use that event to check if a player changed the value, then in the callback function do something like :

 JASS:

if (GetTriggerPlayer()==GetLocalPlayer()) then
 call SetCameraField(CAMERA_FIELD_TARGET_DISTANCE, BlzFrameGetValue(BlzGetFrameByName("Distance", 0)), 0)
endif

Or would this desync ?

I also had another question. I am new to all this, and trying hard to understand. I tried to adapt your vertical slider (it does nothing in the example you have provided) to your previous sliders method. I was wondering if I did it properly :

 JASS:

function CreateVerticalSliderDistance takes nothing returns nothing
 local framehandle VSlider = BlzCreateFrameByType( "SLIDER", "Distance", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI,0), "QuestMainListScrollBar", 0 )

 call BlzFrameClearAllPoints(VSlider)
 call BlzFrameSetAlpha(VSlider, 0.8)
 call BlzFrameSetSize(VSlider, 0.012, 0.06)
 call BlzFrameSetAbsPoint(VSlider, FRAMEPOINT_TOPLEFT, 0.40, 0.30)
 call BlzFrameSetMinMaxValue(VSlider, 100, 4000)
 call BlzFrameSetValue(VSlider, 3000)
 call BlzFrameSetStepSize(VSlider, 50)
endfunction

Thank you for what you have shared so far, and thank you for any further help you will be able to provide !

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Macadamia said:

 So when yu say "and can differ as long the frames data is not used in a synced manner, frames should still be create for all players", you mean that we can set one of the camera fileds (the one we want the slider to change) to the value of the slider, but we can not register FRAMEEVENT_SLIDER_VALUE_CHANGED to a Trigger in a multiplayer context ? Or that we can actualy register this event but not read the value from the event response BlzGetTriggerFrameValue ?


Click to expand...

For this example there was no need for any SliderFrame-Event and I wanted to skip that whole FrameEvent stuff. The Slider is used only localy without any effect onto other players.

Frames and their Events have to be created for all players. Their position on screen, their interactive current value is not known by others. It is on default not shared, hence one can say each player has an own Frame. Like Player B won't see/know the Slider value Player A selected.

Macadamia said:

 I don't like timed events when I can avoid them, would it be possible to use that event to check if a player changed the value, then in the callback function do something like :

Or would this desync ?


Click to expand...

True if you don't want Timers, frameevent are the way to go. Your short example is fine as long BlzGetFrameByName("Distance", 0) was used for all players in an early time, BlzGetFrameByName will occupy one handleId when it returns a frame that hasn't got an handleId yet.

But you could improve your example a bit and make it safe from such problems. Inside the frameevent one has some native getters to get values/infos about the current Event. This values are synced and known by all Players. Inside FRAMEEVENT_SLIDER_VALUE_CHANGED: BlzGetTriggerFrameValue() is the new value GetTriggerPlayer() selected for the used Frame: BlzGetTriggerFrame(). This event and value runs for every player in the game.

 JASS:

native BlzGetTriggerFrame takes nothing returns framehandle
native BlzGetTriggerFrameEvent takes nothing returns frameeventtype
native BlzGetTriggerFrameValue takes nothing returns real
native BlzGetTriggerFrameText takes nothing returns string
native GetTriggerPlayer takes nothing returns player

If the actions of that event only should happen for the player that used that frame one has to use the GetTriggerPlayer() == GetLocalPlayer() Block like you have used it.

Macadamia said:

 I also had another question. I am new to all this, and trying hard to understand. I tried to adapt your vertical slider (it does nothing in the example you have provided) to your previous sliders method. I was wondering if I did it properly :


Click to expand...

call BlzFrameSetAlpha(VSlider, 0.8) wants an integer from 0 to 255

 -> call BlzFrameSetAlpha(VSlider, 204)

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

Wow thank you. That is what I call mastering a subject.

Precious information, clear explanations... if you are not the only one of your kind, the community has evolved to an extent that I would never have expected.

You have my absolute respect. But above all, thank you for your time. Really much appreciated.

Will now try to implement this properly. Hope you don't mind being added to maps Credits

PS : yes JassHelper knocked me on the head about the integer required for alpha, but forgot to edit my post.

Just came back from town, and tried the slider, it indeed works like a charm !!!

I have two issues though :

1) I didn't manage to position the slider at the bottom left corner of my screen using this I found on one of your older tutorials

 JASS:

 call BlzFrameSetAbsPoint(VSlider, FRAMEPOINT_TOPLEFT, -0.14, 0.6)

As soon as I give x a negative value, the slider seems to vanish :'( this is really a pity as the bottom left corner seemed ideal for that vertical slider.

2) I did not manage to create a label for the slider. For sure there is something I didn't properly understand, but I tried several ways, including this :

 JASS:

 local framehandle label = BlzCreateFrame("TEXT", VSlider, 0, 1)
...
 call BlzFrameSetText(label, "Camera distance slider :")

But nothing happened. I suppose I should use BlzCreateFrameByType again, but I don't know what I should use as a typeName and what it inherits from...


 Last edited: Feb 28, 2020

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Macadamia said:

 1) I didn't manage to position the slider at the bottom left corner of my screen using this I found on one of your older tutorials

As soon as I give x a negative value, the slider seems to vanish :'( this is really a pity as the bottom left corner seemed ideal for that vertical slider.


Click to expand...

There are 2 Groups of Frames SimpleFrames and the others, I call them Frames. Right now most default frames and custom SimpleFrames can Leave the 4:3 part of the screen which is 0/0 to 0.8/0.6, custom Frames can not leave that 4:3 screen. If a part of such Frame would leave it the frame will be malformed.

[JASS/AI] - UI: Positionate Frames

Macadamia said:

 2) I did not manage to create a label for the slider. For sure there is something I didn't properly understand, but I tried several ways, including this :

But nothing happened. I suppose I should use BlzCreateFrameByType again, but I don't know what I should use as a typeName and what it inherits from...


Click to expand...

BlzCreateFrameByType allows you to define and create a new Frame during the game, with the other 2 Create natives you can only create Frames definied in fdf. At the beginning of that post is a List of FrameTypes UI: Reading a FDF. That is the FrameType, the second argument is the frame's name, inherits is the name of an mainframe in the loaded fdfs.

You can inherit only from mainFrames (the ones having a definition outside of other frames in fdf), which is difficult to see without opening a fdf, also a List:

[JASS/AI] - UI: List - Default MainFrames

Although it is not required to inherit anything, one can just create a empty "TEXT"-Frame they have a default Font and size.

 JASS:

call BlzCreateFrameByType("TEXT", "MyFrameName", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)

If you are not happy with the Size of the Chars use BlzFrameSetScale onto the "TEXT"-Frame, reason is BlzFrameSetFont only works on String-Frames (hopefuly that changes in the future and also TEXT can be changed).

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

Thank you very much, once again.

It works...

I had found a good compromise for the slider, with the right scaling, it seems to fit perfectly just next to the portrait (on the left) on the basic custom UI skin I am using

With your help, I can now create a frame for the label, and I think I will need it use absolute positioning to make sure it fits on a readable position near the slider, and probably give it a noticeable color (like red). I will follow your advice for the size, although one last question, why not use BlzFrameSetSize like for the slider ?

The Font will fit perfectly once I set the proper scale and coloring.

After all this, I think I need to apologize :

I noticed how many tutorials you have worked hard to write on the UI subject, and I probably wouldn't have made you spent so much time helping me if I had taken enough time to read them all before Hijacking this tutorial page. And I definitely intend to read them all as one of my objectives on my current project is to learn how to make my own UI and being able to enhance it with little beauties like your sliders. But I am first trying to reach a point where I can leave the project ready for people to have fun playing it while I will be able to start working on more ambitious features.

The problem is the map has become quite technical on many aspects, and thus quite challenging for someone who only came back to mapmaking since December.

I love what I am doing, I never give up, but assimilating so much new knowledge just after having caught up with the one I had lost after 16 years of "retirement" probably made me "lazy" some busy days.

So please accept my apologies, and be sure I really deeply appreciate the time you dedicated to help me !

OK, after some testing and fine tuning, the Label of the Slider finally ended up becoming a tooltip, making it much more visible on mouse-over. It was not that easy to read on top of the UI Skin.

Everything works as I was hoping now !!! @Tasyen : you're in the map's credits, that the least I could do ;-)

Now, for anyone who would read this thread with the same issues as myself, here is my final trigger in Jass2 :

 JASS:

function UpdateCamera takes nothing returns nothing
 local player ThePlayer=GetTriggerPlayer()
 local real Distance=BlzGetTriggerFrameValue()

 if (ThePlayer==GetLocalPlayer()) then
 call SetCameraField(CAMERA_FIELD_TARGET_DISTANCE, Distance, 0)
 endif
 set ThePlayer=null
endfunction

function InitTrig_Camera_Controls takes nothing returns nothing
 local trigger CameraControl=CreateTrigger()
 local framehandle VSlider = BlzCreateFrameByType( "SLIDER", "Distance", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI,0), "QuestMainListScrollBar", 0 )
 local framehandle SliderToolTip = BlzCreateFrameByType("TEXT", "SliderTitle", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
 local integer Color = BlzConvertColor(255, 255, 255, 0) // Change values of alpha, red, green and blue for another custom color

 // Set parametees for the Slider...
 call BlzFrameSetSize(VSlider, 0.015, 0.085)
 call BlzFrameSetAbsPoint(VSlider, FRAMEPOINT_BOTTOMLEFT, 0.20, 0.022)
 call BlzFrameSetMinMaxValue(VSlider, 600, 4000)
 call BlzFrameSetValue(VSlider, 2500)
 call BlzFrameSetStepSize(VSlider, 50)

 // ... and for the SliderToolTip
 call BlzFrameSetText(SliderToolTip, "USE THIS SLIDER TO CONTROL THE CAMERA ZOOM")
 call BlzFrameSetScale(SliderToolTip, 1.60)
 call BlzFrameSetTextColor(SliderToolTip, Color)
 call BlzFrameSetAbsPoint(SliderToolTip, FRAMEPOINT_BOTTOMLEFT, 0.22, 0.16)
 call BlzFrameSetTooltip(VSlider, SliderToolTip)

 // Set Initial camera distance for all players
 call SetCameraField(CAMERA_FIELD_TARGET_DISTANCE, 2500, 0)

 // Add event and callback to trigger for the Slider
 call BlzTriggerRegisterFrameEvent(CameraControl, VSlider, FRAMEEVENT_SLIDER_VALUE_CHANGED)
 call TriggerAddAction(CameraControl, function UpdateCamera)

 // nulling never hurts !
 set VSlider=null
 set CameraControl=null
endfunction


 Last edited: Feb 29, 2020

#### iown_azz

#### iown_azz

 Level 8

JoinedJun 16, 2008Messages333

Resources: 2

#### Resources
Tutorials 2

I fixed it for Lua if any one wants to see what it looks like...

just call createCamControl() some where.

I also changed it so the player can't make the camera go upside down

 Lua:

function LoadToc(s)
 if BlzLoadTOCFile(s) then
 BJDebugMsg("Loaded: " .. s)
 else
 BJDebugMsg("Failed to load: " .. s)
 end
end

function updateCam()
 SetCameraField(CAMERA_FIELD_TARGET_DISTANCE, BlzFrameGetValue(BlzGetFrameByName("EscMenuSliderTemplate", 0)), 0)
 SetCameraField(CAMERA_FIELD_ANGLE_OF_ATTACK, BlzFrameGetValue(BlzGetFrameByName("EscMenuSliderTemplate", 1)), 0)
 SetCameraField(CAMERA_FIELD_ROTATION, BlzFrameGetValue(BlzGetFrameByName("EscMenuSliderTemplate", 2)), 0)
 BlzFrameSetText(BlzGetFrameByName("EscMenuLabelTextTemplate", 0), "Distance: " .. R2SW(BlzFrameGetValue(BlzGetFrameByName("EscMenuSliderTemplate", 0)), 1, 1))
 BlzFrameSetText(BlzGetFrameByName("EscMenuLabelTextTemplate", 1), "Angle of Attack: " .. R2SW(BlzFrameGetValue(BlzGetFrameByName("EscMenuSliderTemplate", 1)), 1, 1))
 BlzFrameSetText(BlzGetFrameByName("EscMenuLabelTextTemplate", 2), "Rotation: " .. R2SW(BlzFrameGetValue(BlzGetFrameByName("EscMenuSliderTemplate", 2)), 1, 1))
end

function createSliderDistance()
 local fh = BlzCreateFrame("EscMenuSliderTemplate", BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), 0, 0)
 local label = BlzCreateFrame("EscMenuLabelTextTemplate", fh, 0, 0)
 BlzFrameSetPoint(label, FRAMEPOINT_LEFT, fh, FRAMEPOINT_RIGHT, 0, 0)
 BlzFrameSetAbsPoint(fh, FRAMEPOINT_LEFT, 0.02, 0.5) --below the menu, quest buttons
 BlzFrameSetMinMaxValue(fh, 600, 3000) --limits user can choose
 BlzFrameSetValue(fh, 1650) --starting value, should be used after one changed min max
 BlzFrameSetStepSize(fh, 50)
end

function createSliderAngleOfAttack()
 local fh = BlzCreateFrame("EscMenuSliderTemplate", BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), 0, 1) --we use CreateContext 1, so the new slider will not overwrite the one saved in slot 0. CreateContext defines the integer the new frame and its children will use in the frame storage. We can read the frame storage with BlzGetFrameByName
 local label = BlzCreateFrame("EscMenuLabelTextTemplate", fh, 0, 1)
 BlzFrameSetPoint(label, FRAMEPOINT_LEFT, fh, FRAMEPOINT_RIGHT, 0, 0)
 BlzFrameSetAbsPoint(fh, FRAMEPOINT_LEFT, 0.02, 0.475) --below the menu, quest buttons
 BlzFrameSetMinMaxValue(fh, 270, 335) --limits user can choose
 BlzFrameSetValue(fh, 304) --startin value
 BlzFrameSetStepSize(fh, 2)
end

function createSliderRotation()
 local fh = BlzCreateFrame("EscMenuSliderTemplate", BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), 0, 2) --CreateContext 2
 local label = BlzCreateFrame("EscMenuLabelTextTemplate", fh, 0, 2)
 BlzFrameSetPoint(label, FRAMEPOINT_LEFT, fh, FRAMEPOINT_RIGHT, 0, 0)
 BlzFrameSetAbsPoint(fh, FRAMEPOINT_LEFT, 0.02, 0.45) --below the menu, quest buttons
 BlzFrameSetMinMaxValue(fh, 0, 360) --limits user can choose
 BlzFrameSetValue(fh, 90) --startin value
 BlzFrameSetStepSize(fh, 5)
end

--The function beeing executed when the checkbox is checked/UnChecked.
function checkBoxLockSliders()
 if BlzGetTriggerFrameEvent() == FRAMEEVENT_CHECKBOX_UNCHECKED then
 local enable = true --calc the new state,
 else
 local enable = false --calc the new state,
 end
 if GetLocalPlayer() == GetTriggerPlayer() then --only do stuff for local player
 BlzFrameSetEnable(BlzGetFrameByName("EscMenuSliderTemplate", 0), enable)
 BlzFrameSetEnable(BlzGetFrameByName("EscMenuSliderTemplate", 1), enable)
 BlzFrameSetEnable(BlzGetFrameByName("EscMenuSliderTemplate", 2), enable)
 end
end

function createCheckbox()
 local trig = CreateTrigger()
 local fh = BlzCreateFrame("QuestCheckBox", BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), 0, 3) --CreateContext 3 would not be needed for the checkbox cause it uses different names, but we want to use the same labelFrame Type.
 local label = BlzCreateFrame("EscMenuLabelTextTemplate", fh, 0, 3)
 BlzFrameSetPoint(label, FRAMEPOINT_LEFT, fh, FRAMEPOINT_RIGHT, 0, 0)
 BlzFrameSetAbsPoint(fh, FRAMEPOINT_LEFT, 0.15, 0.40)
 BlzFrameSetText(label, "Lock Sliders")
 TriggerAddAction(trig, checkBoxLockSliders) -- maybe need to fix
 BlzTriggerRegisterFrameEvent(trig, fh, FRAMEEVENT_CHECKBOX_CHECKED) --execute function when checking the box
 BlzTriggerRegisterFrameEvent(trig, fh, FRAMEEVENT_CHECKBOX_UNCHECKED) --executed when unchecking
end

function resetSliders()
 if GetLocalPlayer() == GetTriggerPlayer() then
 BlzFrameSetValue(BlzGetFrameByName("EscMenuSliderTemplate", 0), 1650)
 BlzFrameSetValue(BlzGetFrameByName("EscMenuSliderTemplate", 1), 304)
 BlzFrameSetValue(BlzGetFrameByName("EscMenuSliderTemplate", 2), 90)
 end
 BlzFrameSetEnable(BlzGetTriggerFrame(), false) --this button loses focus
 BlzFrameSetEnable(BlzGetTriggerFrame(), true)
end

function createResetButton()
 local trig = CreateTrigger()
 local fh = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), 0, 0) --CreateContext 0, we can reuse it cause it has unique names compread to the sliders and labels.
 BlzFrameSetSize(fh, 0.09, 0.024) --default size is to big
 BlzFrameSetAbsPoint(fh, FRAMEPOINT_RIGHT, 0.15, 0.40)
 BlzFrameSetText(fh, "Reset Cam")
 TriggerAddAction(trig, resetSliders) --maybe need fixing
 BlzTriggerRegisterFrameEvent(trig, fh, FRAMEEVENT_CONTROL_CLICK)
end

function createCamControl()
 LoadToc("war3mapimported\\templates.toc") --use the custom function to load the Toc, the custom function prints success/fail loading the toc.
 createSliderAngleOfAttack()
 createSliderRotation()
 createSliderDistance()
 createCheckbox()
 createResetButton()
 TimerStart(CreateTimer(), 0.4, true, updateCam)

end

There was a few bugs that I didn't realize while converting


 Last edited: Jul 24, 2020

#### SebioL

#### SebioL

 Level 4

JoinedMay 19, 2020Messages315

Resources: 3

#### Resources
Reforged HD Models 3

I don't even know what to say !!! And to be honest, this camera function by UI is a real evolution in terms of cameras in recent times or the best for Wc3, as I haven't seen anything like it yet.

Lately, this was all I was hoping for to improve my personal map, as i use Mod tiny, with very small objects, it was always difficult to work with the limitations of the standard camera that until today Reforged itself has not offered us anything innovative or leastways more modern. This function will make the game much more practical in relation to the archaic poorly made cameras in GUI or by Chat-Message.

Really, thank you very much, but as for the system itself ... I have only one point to emphasize, in the question of Zoom to give distance, I noticed that even choosing a specific distance in the sliding bars, there is also the possibility for to use an approximation interval , which can be done through the “Scrolling of mouse”, obtaining an even greater variation in zoom (although it is very limited- to my sadness). Just as it exists in the standard camera, but unfortunately it seems to be much smaller than the oscillation of the standard Scroll-mouse Blizzard, could we increase this "interval of zoom" as well to this system ?


 Last edited: Sep 25, 2020

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

The version you see here is only an example for slider usage. It is not a fully flashed out cam control. Users may only care about distance, one slider only. Which also could be placed more fitting into the default UI. I seen a really smart placed one, it was vertical next to the unit portrait.

As a mapper you probably want a slider for each camerafield existing.

I am unsure what "interval of zoom" means. But I don't know how to interact with/setup the default mouse wheel camera scrolling.

#### SebioL

#### SebioL

 Level 4

JoinedMay 19, 2020Messages315

Resources: 3

#### Resources
Reforged HD Models 3

Tasyen said:

 The version you see here is only an example for slider usage. It is not a fully flashed out cam control. Users may only care about distance, one slider only. Which also could be placed more fitting into the default UI. I seen a really smart placed one, it was vertical next to the unit portrait. As a mapper you probably want a slider for each camerafield existing. I am unsure what "interval of zoom" means. But I don't know how to interact with/setup the default mouse wheel camera scrolling.


Click to expand...

I know, I've been researching others UI of cameras by sliders, but apparently the only thing that changes in relation to your system, is only questions of interface designer, positions or sizes of bars. I don't see any significant changes to the system, properly.

I actually wanted more practicality in handling the camera, Wc3 was made at a time when Mouse were limited, really, and Reforged apparently wanted to keep that delay. The ideal would be to have total control only by the mouse (as in any modern game). If we could manipulate the mouse using vJASS codes, it would be perfect.

 Example: Just by rotating the scrolling, we could have a much larger Zoom variation and angle of attack, oscillating by itself until the desired limit. And just by pressing the scrolling and dragging the mouse to the left or right side, we could rotate 360°, and clicking once, it would freeze in the rotation it is in, clicking twice, it will return to the standard view. Anyway, this is just an old wish, but the sliding bars are already of great help! Sorry for the bad English.

This "interval of zoom", I was referring to because the Mouse Scrolling generates a Zoom variation by default, without the use of this slider system, just by turning the Blizzard standard scrolling, there is already a large variation (although i think it’s a limited zoom), but with the use of this bars slider system, I realized that the scrolling of the mouse still maintains a variation of Zoom, but much more reduced than the standard of Blizzard.

As apparently this variation still exists within the scrolling, appearing to have been modified...but in a reduced way (unfortunately), i thought that perhaps this variation of the scrolling of the mouse could be extended/increased to a desired value, with some code, only within this system.

Edit: Within the function for: call TimerStart(CreateTimer(), 0.4, true, function UpdateCam) ... I believe that by increasing to values above 1.0, you will be able to expand the Zoom scroll even more, but the accuracy to keep the scroll intervals more fixed on the camera will be less.


 Last edited: Sep 28, 2020

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Approved.

Did some indenting and grammar/typo fixing.

#### Jaccouille

#### Jaccouille

 Level 26

JoinedFeb 1, 2019Messages468

Resources: 66

#### Resources
Models 16Icons 3Tools 2Maps 2Ported Models 29Ported Icons 14

Hello, thanks for your tutorial, I just got a question, in my map, player have the option to go observer at the beginning by typing -obs, I wanted to provide them a custom UI to zoom/rotate camera and eventually toggle fog of war, I've noticed that the checkbox & "ScriptDialogButton" do not fire an event on click/box checked when the player clicking is in observer state, do you have any idea why?

My first guess would be that anything that inherited "EscMenuCheckBoxTemplate", such as the QuestCheckBox, are disabled for observer and therefore the event isn't triggered, well just a guess. Maybe if I use a checkbox from another panel it could work, I've seen a observerpanel.fdf file, should look into that

Questlog & ally window are disabled in observer state, I've read other tutorial and tried to make use of the replay panel contained in the replaypanel.fdf but I couldn't manage to position it properly.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

I don't know anything about observerPlayers and Frames. Maybe Observers are no real Players and can not trigger Events.

But true that observerpanel uses other checkboxes, SIMPLECHECKBOX which I failed to get any use out of when I tested them (No events, no current Value).

#### ESTO

#### ESTO

 Level 3

JoinedApr 17, 2017Messages29

You can even use these sliders while watching a replay. I dunno why it works, but it is fantastic. I pretty much copy and pasted your sample code and made some slight adjustments + I added a fourth small slider to show/hide as well as enable/disable the other 3. It works for observers / players and while watching a replay.

Very nice. Thanks.

 You must log in or register to reply here.


### Similar threads

 UI: Creating a Bar


Tasyen

Jun 12, 2019

Scripting Tutorials

Replies38Views18K

Oct 2, 2025
KenArok

 UI: Positionate Frames


Tasyen

May 29, 2019

Scripting Tutorials

Replies25Views19K

Feb 11, 2026
Wareditor

 UI: Create a TextButton


Tasyen

May 30, 2019

Scripting Tutorials

Replies12Views12K

Jan 11, 2021
Tasyen

 UI: Change Lumber Text


Tasyen

May 29, 2019

Scripting Tutorials

Replies17Views9K

Apr 1, 2021
pr114

 UI: What are BACKDROPs?


Tasyen

Jun 1, 2019

Scripting Tutorials

Replies27Views13K

Jan 12, 2022
q531620267

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