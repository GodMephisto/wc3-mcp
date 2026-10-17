---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-simpleframes.320385/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Simpleframes
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-simpleframes.320385/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

###
 Table of Contents

Introduction

String

Texture

Layer

SimpleButton​

###
 Requirements

If you don't know how to use a custom toc-File, therefore can't use a custom fdf you should first do a tutorial about toc: UI: toc-Files.​

##
 Introduction

SimpleFrames are another group of frames. They are low in amount of types but frames of this group manage most of the direct ingame UI, like the unitinfo, the ability and item buttons.

Unlike Frames, Simpleframes require fdf and can not be created in a useful way without such a fdf defining a SimpleFrame first. All SimpleFrames can be moved out of the 4:3 screen. Most custom created Simpleframes do not fight for mousecontrol. For the displaying SimpleFrames are below Frames.

If I write SIMPLEFRAME in uppercase only, i talk about this specific type otherwise i address the group.​

##
 SimpleFrame Types

The mainframe able types in warcraft 3 for Simpleframes:

SIMPLEBUTTON

SIMPLECHECKBOX

SIMPLEFRAME

SIMPLESTATUSBAR

They are defined in the fdf like Frames are. The start of 2 such Simpleframe definitions.







 Code:

Frame "SIMPLEFRAME" "SimpleInfoPanelIconDamage" {...
Frame "SIMPLEBUTTON" "UpperButtonBarButtonTemplate" {...

###
 Special Children

SimpleFrames have 2 special frame-types which can not be real mainframes. During the game this types can only exist as children of any Simpleframe.

String

Texture

This 2 types are Simpleframes, means one can use the frame natives onto them. But their definition differs from the others, inside fdf.

 Code:

String "SimpleInfoPanelTitleTextDisabledTemplate" INHERITS "SimpleInfoPanelTitleTextTemplate" {...
Texture "InfoPanelIconBackdrop" INHERITS "InfoPanelIconTemplate" {...

Inside the game this frames would be accessed with:

BlzGetFrameByName("SimpleInfoPanelTitleTextDisabledTemplate", createcontext)

BlzGetFrameByName("InfoPanelIconBackdrop", createcontext)

Inside a fdf, such String/Texture can be defined outside of Simpleframes to have a base to inherit from. But one can create them only as part of a Simpleframe during the game.​

###
 Warning

The game does not like it, if you use this frame natives onto String/Texture, it will crash the game:

BlzFrameSetVisible

BlzFrameIsVisible

BlzFrameSetAlpha

BlzFrameSetLevel

BlzFrameSetEnable

BlzFrameSetScale

BlzFrameSetParent

BlzDestroyFrame​

###
 Anchor

SimpleFrames have another keyword to pos child frames Anchor. Anchor is a shortcut for: "SetPoint pointA, parent, pointA, x, y,". One can still use the other 2; SetPoint and SetAllPoints.

Anchor TOPLEFT, 0.1, 0.05,

pos the own TOPLEFT to the parents TOPLEFT with 0.1 x and 0.05 y offset.

Anchor as the others SetPoint require the attached frame to take a part of the screen by having atleast Width or Height or both.

Otherwise it can happen that the Texture/String attaches itself to the parent of the parent.

Without a point the String/Texture will center itself to the parent or that ones parent (which can be the 4:3 Screen when SimpleFrames are created for ORIGIN_FRAME_GAME_UI), if the direct parent fails.

​

##
 String

For Simpleframes String children display text onto the screen. One String can display one line of text or huge walls of text over multiple Lines, upto the whole screen and above that. I won't tell much about textmarkup for String, most is like with TEXT which should be an own subject.

It is recommented to create and place StringFrames at the earliest with the event "0s expired". At earlier times it can happen that the StringFrame is displaced or does not display any text until the user changes resolution.

String have a different keyword to setup the font then TEXT. Setting a font is required for a String to display visible text. There are 2 ways inside fdf to set Fonts either one adds DecorateFileNames, and uses for the fontFile a stringVariable from a StringList or write down the fontFile.

Font "InfoPanelTextFont",0.009,​
InfoPanelTextFont is only valid with DecorateFileNames, , the font is required for the String to be displayed. DecorateFileNames, has to be written inside the Parent of the String, if one reads the font that way.

The FontFile also could be refered directly but without DecorateFileNames, in the parent. The example is not the same font.

Font "fonts\nim_____.ttf", 0.009,​

###
 String Example

One defines a SIMPLEFRAME having a String child that childs shows the text.

In case you did skip Anchor the Width and Height are there to Anchor to the parent "TestString" which would fail otherwise.

 Code:

Frame "SIMPLEFRAME" "TestString" {
 Width 0.0001,
 Height 0.0001,
 DecorateFileNames,
 String "TestStringValue" {
 Anchor TOPLEFT, 0, 0,
 Font "InfoPanelTextFont", 0.01,
 }
}

The Lua code creating the SIMPLEFRAME from above and setting the text.

First some Debug text is printed so one knows the code is actually running at all. After that one loads the toc loading the fdf with the Simpleframe. Then the Simpleframe is created his point is set and the text of the String child is set.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 print("Start")
 BlzLoadTOCFile("war3mapImported\\your.toc")

 local parent = BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0)
 local frame = BlzCreateSimpleFrame("TestString", parent, 0)

 BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.4, 0.5)

 BlzFrameSetText(BlzGetFrameByName("TestStringValue", 0), "Test Text")

 print("Done")
 end
end

Theoretically the font also could be set inside the code using BlzFrameSetFont. For some wierd reason that does not work for Frame Type "TEXT" (V1.31.1).

BlzFrameSetFont(stringFrame, "fonts\\dfst-m3u.ttf", 0.016, 0)

BlzFrameSetFont(stringFrame, "fonts\\nim_____.ttf", 0.013, 0)

BlzFrameSetFont(stringFrame, "fonts\\thowr___.ttf", 0.012, 0)

Using this font native with a fontsize differing much from the initial fontsize can break the displayed text.​

##
 Texture

Texture is the visual part for Simpleframes (Images).

A Texture will if he does not have a own size copy the parent's size.

When seting the image inside the fdf one uses:

File FilePath,

Texture can have some configs:

A Texture can have an AlphaMode to change the usage of alpha/opaque.

AlphaMode "ALPHAKEY",

AlphaMode "ADD",

AlphaMode "DISABLE",

AlphaMode "BLEND",

AlphaMode "MOD",​

Might look into a wow ui wiki for more infos, but be careful some might not work as expected or at all. The reason why one can look into wow is. Wow and warcraft 3 are both from Blizzard and they shared parts of their source code, part of it is the ui system.

When using BlzFrameSetTexture the AlphaMode is overwritten.

fdf Alphamode can be an additional arg in the Texture fdfAction.

NormalTexture "ADD","textures\zap1b",

One can take only a fraction of an given image and display that to the space taken by the Texture, this affects the in fdf set image as by code. TextCoord has nothing to do with the space taken on the screen. TexCoord Left, Right, Up, Bottom

TexCoord 0, 1, 0, 0.125, (take the total x from 0 to 12.5% from the top of the Image-File)

TexCoord 0.0, 0.5, 0.0, 0.5, (the topLeft part) see image below only a fraction of the Paladin icon.

One can also produce a Tile using TextCoord, to fill the Frame's given screen space with copies of that image. This is also done with TexCoord, by giving numbers higher than 1 for right and bottom.

TexCoord 0, 2, 0, 2,

A Texture can be rotated by 180 degree by placing TOPLEFT & TOPRIGHT in reversed positions (Topleft where Topright should be and Topright where TopLeft should be).

###
 Texture example

Like with String this is an example how one would display just one Texture onto the screen. One defines a SIMPLEFRAME with Width and Height and a Texture which copies mimics the SIMPLEFRAME when not said otherwise.

 Code:

Frame "SIMPLEFRAME" "TestTexture"{
 Width 0.04,
 Height 0.04,
 Texture "TestTextureValue" {
 }
}

We load the toc mentioning the fdf with the SIMPLEFRAME and create the mainFrame "TestTexture". After that set the texture of the child to the Icon of the Paladin and pos the SIMPLEFRAME to center of the screen.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 BlzLoadTOCFile("war3mapImported\\your.toc")
 local frame = BlzCreateSimpleFrame("TestTexture", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0)
 BlzFrameSetTexture(BlzGetFrameByName("TestTextureValue", 0), "ReplaceableTextures\\CommandButtons\\BTNHeroPaladin", 0, true)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.4, 0.3)
 end
end

###
 Texture Coloring

A Texture can be colored during the game with the native BlzFrameSetVertexColor takes framehandle frame, integer color. To get the color integer one uses another native BlzConvertColor takes integer a, integer r, integer g, integer b

BlzFrameSetVertexColor(texture, BlzConvertColor(255, 255, 255, 1)) would display the texture without blue.

When one uses that line onto an Archmage Texture it would become yellowish like that image below

##
 Layer

Layer are a fdf feature for Simpleframes that can be used to set the order of String/Texture inside a SimpleFrame. Inside a Layer only String and Texture can be defined. Blizzard used 2 Layers in their fdf for warcraft 3.

Layer "BACKGROUND" {...

Layer "ARTWORK" {...

"ARTWORK" is displayed above "BACKGROUND".

The Layers of a SimpleFrame above, are both above a lower SimpleFrame.

Based on wow ui wikis there should be more, but I failed to create them. Probably they do not exist in warcraft 3 (V1.31.1). Theoretically one could think the HIGHLIGHT layer is implemented over the keyword UseHighlight <texture>, for SIMPLEBUTTON.

 Code:

Frame "SIMPLEFRAME" "TestTextureLayer" {
 Width 0.1,
 Height 0.1,

 Layer "ARTWORK" {
 Texture {
 Width 0.05,
 Height 0.05,
 Anchor BOTTOMLEFT, 0, 0,
 File "ReplaceableTextures\CommandButtons\BTNHeroMountainKing",
 }
 Texture {
 Width 0.05,
 Height 0.05,
 Anchor BOTTOMRIGHT, 0, 0,
 File "ReplaceableTextures\CommandButtons\BTNHeroBloodElfPrince",
 }
 }

 Layer "BACKGROUND" {
 Texture {
 Width 0.1,
 Height 0.1,
 Anchor CENTER, 0, 0,
 File "ReplaceableTextures\CommandButtons\BTNHeroPaladin",
 }
 }
}

##
 SIMPLEBUTTON

SimpleButton is the interactive Simpleframe. It is quite powerful for a Simpleframe, it can be clicked starting a FRAMEEVENT_CONTROL_CLICK, even when being moved out of the 4:3 Screen. When being clicked it will not keep the focus (which Frame-Buttons do). It controls the space on the screen, therefore one can not order/select units at that spot. Each SIMPLEBUTTON can have only one FRAMEEVENT_CONTROL_CLICK Event, when one registers another one (for the same SimpleButton) the previous event will not fire anymore.

Other Simpleframes can be the tooltip of a SIMPLEBUTTON (But the tooltip-SimpleFrame is not hidden with that call as it does with Frame, maybe a bug in 1.31.1).

One can use BlzFrameSetEnable on the Button to stop the user from starting events with that Buttons. But it will still control the screen space and the tooltip won't update anymore by (un)hovering the button. (Frame-Buttons stop controling the screenspace when being disabled, if they ever did)

Using BlzFrameSetVisible(button, false) will hide the button and it's children as it will free the screen space taken by them.

A SIMPLEBUTTON can have functional children to have a texture for different states and different Strings for that states. On default this functional children dont get names, to give them names one needs to add an optional arg NAMED "name", without this arg they are kinda static and unreachable with BlzGetFrameByName.

When one wants a Menu(F10) like Button then "ui\framedef\ui\upperbuttonbar.fdf" contains templates from blizzard for that.

This would be the definition for such an IconButton inside fdf:

 Code:

Frame "SIMPLEBUTTON" "MySimpleButton" {
 Width 0.039,
 Height 0.039,
 Texture "MySimpleButtonTexture" {
 }
}

During The game one would create with BlzCreateSimpleFrame "MySimpleButton" pos that frame and set the texture of "MySimpleButtonTexture" using BlzFrameSetTexture

The Lua code that creates that button showing a Paladin icon. When the button is clicked it prints "Button Click":

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 BlzLoadTOCFile("war3mapImported\\MySimpleButton.toc")

 local trigger = CreateTrigger()
 TriggerAddAction(trigger, function()
 print("Button Click")
 -- SimpleButton does not keep the focus.
 end)
 local button = BlzCreateSimpleFrame("MySimpleButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0)
 BlzFrameSetAbsPoint(button, FRAMEPOINT_CENTER, 0.9, 0.3)
 BlzFrameSetTexture(BlzGetFrameByName("MySimpleButtonTexture", 0), "ReplaceableTextures\\CommandButtons\\BTNHeroPaladin", 0, true)
 BlzTriggerRegisterFrameEvent(trigger, button, FRAMEEVENT_CONTROL_CLICK)
 end
end

If one would want a glowing while the button is hovered, one would add the functional child frame to the SIMPLEBUTTON, inside the fdf. The example Highlight is the yellow glowing one sees in scorescreen.

 Code:

Texture "MySimpleButtonButtonHighlight" {
 File "UI\Glues\ScoreScreen\scorescreen-tab-hilight.blp",
 AlphaMode "ADD",
}
Frame "SIMPLEBUTTON" "MySimpleButtonGlowing" {
 Width 0.039,
 Height 0.039,
 UseHighlight "MySimpleButtonButtonHighlight",
 Texture "MySimpleButtonTexture" {
 }
}

example simple Iconbutton with named functional children

 Code:

Texture "MyUpperMenuButtonHighlight" {
 File "UpperMenuButtonTexture",
 TexCoord 0.0, 0.6640625, 0.75, 0.921875,
 AlphaMode "ADD",
}
Texture "TextureTemplate" {}
Frame "SIMPLEBUTTON" "TestA" {
 DecorateFileNames,
 Width 0.039,
 Height 0.039,
 NormalTexture NAMED "MyIconA2","TextureTemplate",
 PushedTexture NAMED "MyIconB2","TextureTemplate",
 DisabledTexture NAMED "MyIconC2","TextureTemplate",
 UseHighlight "MyUpperMenuButtonHighlight",
}

then in Lua one loads toc, creates the simpleframe and sets the icons.

 Lua:

BlzLoadTOCFile("UI/Test.toc")
frame = BlzCreateSimpleFrame("TestA", BlzGetFrameByName("ConsoleUI", 0), 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.4, 0.33)
BlzFrameSetTexture(BlzGetFrameByName("MyIconA2", 0), BlzGetAbilityIcon(FourCC'Hpal'), 0, true)
BlzFrameSetTexture(BlzGetFrameByName("MyIconB2", 0), BlzGetAbilityIcon(FourCC'hfoo'), 0, true)
BlzFrameSetTexture(BlzGetFrameByName("MyIconC2", 0), BlzGetAbilityIcon(FourCC'ogru'), 0, true)

SIMPLESTATUSBAR was explained in another tutorial.

SIMPLECHECKBOX: I wasn't able to attach events nor get the value in any way last time I tested it (V1.31PTR) so it is kinda useless right now. Therefore I won't have an explanition here.

Edited 2025.09.20: Added NAMED and oneline Texture with alphamode

Edited: Replaced selfexecution with a less problematic approach​

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

SimpleButtonTest.w3x

 25.2 KB

 · Views: 721




 Last edited: Sep 20, 2025

#### shinzaku

#### shinzaku

 Level 1

JoinedFeb 3, 2020Messages7

Hi Tasyen,

Do you have any more information in terms of feedback events? I want the visual and auditory cues that happen when clicking, for example, a command card button.

I don't see anything regarding feedback in the commandbar.fdf or the consoleui.fdf so I may be looking in the wrong place

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

You don't see much in commanbar.fdf cause it is mostly code driven. GLUE-Frames display a feedback sound when clicked and were created with BlzCreateFrame. For SIMPLEBUTTON I don't know about built in Sounds. You probably have to start the sound by yourself, inside a Control_Click event. The Mouse click sound is from Sound/Interface/MouseClick1.flac.

It is intended by fdf that one has 1 functional Texture/ChildFrame for the different states, when one has such functional childs the game will show them based on the current state of the Button. Default, Pushed, Disabeled, Highlight, DisabledPushed.

For SIMPLEBUTTONs I do not know how one can create them so that one could change them during the game. For non Simple Buttons checkout another tutorial.

If you want a SIMPLEBUTTON that does not have to change the visuals during game you might checkout

upperbuttonbar.fdf

 Code:

Texture "UpperMenuButtonBackground" {
 File "UpperMenuButtonTexture",
 TexCoord 0.0, 0.6640625, 0.0, 0.171875,
}
Texture "UpperMenuButtonPushedBackground" {
 File "UpperMenuButtonTexture",
 TexCoord 0.0, 0.6640625, 0.25, 0.421875,
}
Texture "UpperMenuButtonDisabledBackground" {
 File "UpperMenuButtonTexture",
 TexCoord 0.0, 0.6640625, 0.5, 0.671875,
}
Texture "UpperMenuButtonHighlight" {
 File "UpperMenuButtonTexture",
 TexCoord 0.0, 0.6640625, 0.75, 0.921875,
 AlphaMode "ADD",
}
Frame "SIMPLEBUTTON" "UpperButtonBarButtonTemplate" {
 DecorateFileNames,
 Width 0.085,
 Height 0.022,
 ButtonPushedTextOffset 0.001 -0.001,
 NormalTexture "UpperMenuButtonBackground",
 PushedTexture "UpperMenuButtonPushedBackground",
 DisabledTexture "UpperMenuButtonDisabledBackground",
 UseHighlight "UpperMenuButtonHighlight",
}

#### shinzaku

#### shinzaku

 Level 1

JoinedFeb 3, 2020Messages7

Thank you so much!

#### Planetary

#### Planetary

 Level 15

JoinedFeb 7, 2020Messages399

Resources: 3

#### Resources
Maps 2Spells and Systems 1

I think a note should be added from your other tutorial UI: toc-Files that without a break at the end of a .toc file it will fail to load. Bashed my head against the wall for an hour before figuring that out.

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Hello Tasyen,

I was trying to use BlzFrameSetVertexColor but I had no luck.

I used exactly the code you mentioned on the texture frame and the texture doesn't show a different color:

 Lua:

BlzFrameSetVertexColor(BlzGetFrameByName("ButtonTex", 0), BlzConvertColor(255, 255, 255, 1))

Whereas setting alpha on the same framehandle does show its effects:

 Lua:

BlzFrameSetAlpha(BlzGetFrameByName("ButtonTex", 0), 135)

The frame has been created using the following fdf:

 Code:

Frame "BUTTON" "Button" {
 Height 0.016,
 Width 0.056,
 ControlStyle "AUTOTRACK|HIGHLIGHTONMOUSEOVER",
 ControlBackdrop "ButtonTex",
 Frame "BACKDROP" "ButtonTex" {
 BackdropBlendAll,
 }
 ControlPushedBackdrop "ButtonPush",
 Frame "BACKDROP" "ButtonPush" {
 BackdropBlendAll,
 BackdropBackgroundInsets 0.001 0.001 0.001 0.001,
 }
 ControlMouseOverHighlight "ButtonGlow",
 Frame "HIGHLIGHT" "ButtonGlow" {
 DecorateFileNames,
 HighlightType "FILETEXTURE",
 HighlightAlphaFile "EscMenuButtonMouseOverHighlight",
 HighlightAlphaMode "ADD",
 }
}

And then the frame is created with this line:

 Lua:

BlzCreateFrame("Button", BlzGetFrameByName("ConsoleUIBackdrop", 0), 0, 0)

#### Planetary

#### Planetary

 Level 15

JoinedFeb 7, 2020Messages399

Resources: 3

#### Resources
Maps 2Spells and Systems 1

I believe vertex coloring only works on textures for simple frames (won't work when applied to backdrops).

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Thanks for the response.

I have been trying to use Simpleframes instead of Backdrops to serve as ControlBackdrop and ControlPushedBackdrop, but was unsuccesful, I guess they really need to be backdrops?

I guess the only way forward would be to only use another frame type like "FRAME" and attach a click event. Although I wouldn't want to lose visues cues like BackdropBackgroundInsets and ControlMouseOverHighlight.

See, I try to make a spell selection system, where you can pick 4 spells out of a broader selection. I would like to have unselected spells with darkened icons, so the selected spells will stand out.

SetVertexColor would be perfect, but I'm afraid I will be forced to work with adjusting Alpha, which produces a less good looking effect.

#### Planetary

#### Planetary

 Level 15

JoinedFeb 7, 2020Messages399

Resources: 3

#### Resources
Maps 2Spells and Systems 1

pr114 said:

 Thanks for the response.

I have been trying to use Simpleframes instead of Backdrops to serve as ControlBackdrop and ControlPushedBackdrop, but was unsuccesful, I guess they really need to be backdrops?

I guess the only way forward would be to only use another frame type like "FRAME" and attach a click event. Although I wouldn't want to lose visues cues like BackdropBackgroundInsets and ControlMouseOverHighlight.

See, I try to make a spell selection system, where you can pick 4 spells out of a broader selection. I would like to have unselected spells with darkened icons, so the selected spells will stand out.

SetVertexColor would be perfect, but I'm afraid I will be forced to work with adjusting Alpha, which produces a less good looking effect.


Click to expand...

I personally despise the quirks of simple frames and have been instead opting to use a helper function that builds a backdrop + an invisible button on top.

You can then build frameenter/leave events to update the button's backdrop manually and with more control.

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Planetary said:

 I personally despise the quirks of simple frames and have been instead opting to use a helper function that builds a backdrop + an invisible button on top.

You can then build frameenter/leave events to update the button's backdrop manually and with more control.


Click to expand...

Oh, that is a good idea. It will preserve the highlighting and the button action.

The BackdropBackgroundInsets might be replicated by slightly reducing the backdrop frame size while pressing down, controlled with Mouse Up and Mouse Down events.

#### Atreus

#### Atreus

 Level 1

JoinedAug 24, 2020Messages15

Good

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Added the orange color to some titles, some indention and two titles at the start.

Approved

#### PROSHELDOTU

#### PROSHELDOTU

 Level 2

JoinedMay 5, 2013Messages7

Hi, @Tasyen )

Do you happen to be able to tell me how a regular Leaderboard created via CreateLeaderboard works outside of the frame position 0.8?

I looked at LeaderBoard.fdf and saw - Frame "FRAME" "Leaderboard".

But, outside of 4:3, only the SIMPLEFRAME type can work.

Indeed, I created my Leaderboard via BlzCreateFrame, and it didn't display at the X position greater than 0.8

And I can't figure out how then it is shown via CreateLeaderboard

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

You can use any custom frame outside the 4:3 screen if you use this as its parent: BlzGetFrameByName("ConsoleUIBackdrop", 0)

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

I guess Blizzard wanted that Leaderboard, TimerDialog and Multiboard are at the right end of the screen for wider resolutions and made special rules allowing them to break the 4:3 limitation which also affects their children, lucky for us.

I mean there are other Frames which have hardcoded special rules, like "SimpleInfoPanelIconDamage" the game always creates a Mouselistener for them.

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Hi Tasyen,

I want to change the icon that gets displayed as Idle Worker button. Just changing the peasant's icon of that race is not going to be a solution, because my map includes a 'random slots' mode where players might end up in a different team than their lobby race.

So I find that the frame that holds the worker icon is child index 7 of "ConsoleUI".

Using BlzFrameSetTexture on this frame doesn't do anything.

Based on the width and height I think it is the following texture definition in ui\framedef\ui\consoleui.fdf. Unfortunately the texture has no name.

 Code:

Texture {
 File "ConsoleTexture04",
 Width 0.032,
 Height 0.032,
 TexCoord 0, 1, 0, 0.125,
 AlphaMode "ALPHAKEY",
 Anchor TOPRIGHT,0,0,
 }

Now I've understood it is possible to adjust the game's fdf's.

I've adjusted the fdf by adding a title "WorkerIcon" to this texture. Then re-imported the fdf file under ui\framedef\ui\consoleui.fdf.

Then in the code I attempt to modify the frame texture (unsure of the creation id of the ConsoleUI frame:

for x=0,10 do

 local f = BlzGetFrameByName("WorkerIcon", x)

 if f then BlzFrameSetTexture(f, "check", 0, true) end

end

"check" is a valid blp file in my map.

So unfortunately, this doesn't do anything. The map does not crash, but the icon is still unchanged. Any idea what I do wrong, if what I'm doing is possible at all?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

The Idle Worker Button is not mentioned in fdf. The thing you posted is the TopRight background of the console below the ResourceBar, because Anchor TOPRIGHT,0,0, is at topright of the screen. While the Worker button should be either relative to left or to bottomleft.

I think that the worker Button belongs to SimpleFrames which prevents getting the FrameChilds handling the displayed Textures

I personaly just placed a custom Icon over the default one in CustomConsoleUI, costs the on click animation (becoming smaller during click).

Some other Idea would be you could just replace it with a custom button which then let the clicking player click the default button, would cost the tooltip.

Warcraft 3 does not support overwritting game fdfs by map imports. But there is a "hacky" unsafe way.

Since Reforged one can load in a custom fdf by Loading a TOC in the function config or in the Lua Root, that happens in the lobby before the game's fdfs are loaded and will take the names you have defined, if the rootFrames names you customly loaded in are the same as the game ones yours will replace the default ones.

But one can't do that if you ever plan to make your map Save&Load able with the warcraft 3 Save&Load System. I am also not sure how well that works with playing multiple different maps in a row.

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Thanks Tasyen. I thought this was the one, because I can move the idle worker button using child index 7 of "ConsoleUI".

I think to replace the Idle worker icon (showing the count of idle workers), there is no way around periodically checking all worker unit's current order, with a short interval. As I don't think there is a trigger that checks when their current order has become empty. For instance when a worker was cutting trees, but the trees in the surrounding area ran out.

This all sounds too performance intensive for me.

Interesting that you mention it is possible to overwrite fdf's like that. Though I'm not too keen when you say its an unsafe way. Besides, if I understand you correctly, as this frame is not contained in the fdf's, it is not possible to reach it by overwriting fdf's.

I will just go for a simple solution to put one generic idle worker icon for all races

#### Pwnica

#### Pwnica

 Level 12

JoinedMar 13, 2020Messages421

Resources: 3

#### Resources
Reforged HD Models 2Tutorials 1

Hi Tasyen... i did a simple button for 2 Players it works (i used Global Reals to transfer the rest of the action into Gui)

but it desync... why? (first time ever using lua and dont realy get it i learned the basic of jass for now)

 Code:

TimerStart(CreateTimer(),0,false, function()
 BlzLoadTOCFile("war3mapImported\\MySimpleButton.toc")
 BlzLoadTOCFile("war3mapImported\\MyStatusBar.toc")
 local trigger = CreateTrigger()
 TriggerAddAction(trigger, function()
 if GetLocalPlayer(1) == GetTriggerPlayer(1) then
 globals.udg_An1 = 1.00

 end
 if GetLocalPlayer(2) == GetTriggerPlayer(2) then
 globals.udg_An2 = 1.00

 end



end)

 local button = BlzCreateSimpleFrame("MySimpleButtonGlowing", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0)
 BlzFrameSetAbsPoint(button, FRAMEPOINT_LEFT, 0.062, 0.193)
 BlzFrameSetTexture(BlzGetFrameByName("MySimpleButtonTexture", 0), udg_Icon, 0, true)
 BlzTriggerRegisterFrameEvent(trigger, button, FRAMEEVENT_CONTROL_CLICK)

 globals.udg_An1 = 0.00
 globals.udg_An2 = 0.00

end)

sorry if i disturb you.. xD

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Your code looks like it was put just into the Lua root that is dangerous when it creates warcraft 3 objects. I also did that before i was aware of the dcs/problems it produces. These warcraft 3 objects are not stable and will be garbage collected at some random time which can differ between users (dc). it also makes it be called twice, once in the lobby and another one when the map starts.

Change the TimerStart thing into a function which you call from a GUI Trigger.

Or hook into a function that is called on default at map init/0s.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real() -- run the real function
 --your custom code
 end
end

GetLocalPlayer(1) and GetTriggerPlayer(1) take no arguments GetLocalPlayer(), GetTriggerPlayer()

If globals.udg_An1/2 is used as Event in a GUI Trigger than this will also desync. Because TriggerActions have to run for all players, if you set it only for one Player it only runs for only one player and with that the game dcs by running the TriggerAction. A GUI trigger without TriggerAction does almost nothing.

That Button Event happens for all players with GetTriggerPlayer() giving you the active player. Compare it with Player(x) instead of GetLocalPlayer().

Player(x) == GetTriggerPlayer()

#### Pwnica

#### Pwnica

 Level 12

JoinedMar 13, 2020Messages421

Resources: 3

#### Resources
Reforged HD Models 2Tutorials 1

Tasyen said:

 Your code looks like it was put just into the Lua root that is dangerous when it creates warcraft 3 objects. I also did that before i was aware of the dcs/problems it produces. These warcraft 3 objects are not stable and will be garbage collected at some random time which can differ between users (dc). it also makes it be called twice, once in the lobby and another one when the map starts.

Change the TimerStart thing into a function which you call from a GUI Trigger.

Or hook into a function that is called on default at map init/0s.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real() -- run the real function
 --your custom code
 end
end

GetLocalPlayer(1) and GetTriggerPlayer(1) take no arguments GetLocalPlayer(), GetTriggerPlayer()

If globals.udg_An1/2 is used as Event in a GUI Trigger than this will also desync. Because TriggerActions have to run for all players, if you set it only for one Player it only runs for only one player and with that the game dcs by running the TriggerAction. A GUI trigger without TriggerAction does almost nothing.

That Button Event happens for all players with GetTriggerPlayer() giving you the active player. Compare it with Player(x) instead of GetLocalPlayer().

Player(x) == GetTriggerPlayer()


Click to expand...

Thx for your anwser Tasyen and that you take the time i tryed what you said (what i understand so far) but then the button dont work.. it seems i dont get it and try it later again and take some time to understand some more lua...

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

Hi tasyen,

I've been trying to get the frames I create to be movable to the widescreen area. Here someone told me one can wrap a simpleframe around another frame, but it didn't work. The frame is bugged (long before I even get to the moving to the widescreen area part).

I've seen maps with very complex frame arrangements in the widescreen area, so something like this has to be possible. Do you have/could you do a tutorial on this or could you tell me what's going wrong here?

 Code:

Frame "SIMPLEFRAME" "HeroSelectionMenu" {
 Frame "BACKDROP" "HeroSelectionMenuChild" {
 UseActiveContext,
 DecorateFileNames, //Look-Up Names in some String table (for example gameinterface)
 BackdropTileBackground, //Tile mode enabled
 BackdropBackground "EscMenuBackground", //BackgroundFile
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.03, //higher numbers make the corners bigger.
 BackdropBackgroundSize 0.15, //changes the scale of the BackdropBackground texture.
 BackdropBackgroundInsets 0.0044 0.0044 0.0044 0.0044, //makes the background smaller, from the outside.
 BackdropEdgeFile "EscMenuBorder", //the border File
 BackdropBlendAll,
 }
}

 JASS:

 set heroSelectionMenu = BlzCreateSimpleFrame("HeroSelectionMenu", BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), 0)
 call BlzFrameSetAbsPoint(heroSelectionMenu, FRAMEPOINT_TOPLEFT, 0, 0.6)
 call BlzFrameSetAbsPoint(heroSelectionMenu, FRAMEPOINT_BOTTOMRIGHT, 0.8, 0)
 set heroSelectionMenu = BlzFrameGetChild(heroSelectionMenu, 0)

Thanks!

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

One can free Frames from the 4:3 Screen limitation by creating them for a different Parent then GAME_UI, in Reforged such a Parent could be BlzGetFrameByName("ConsoleUIBackdrop",0). Using ("ConsoleUIBackdrop",0) as parent pushes them to a lower Layer, below SimpleFrames.

One also could use BlzGetFrameByName("Leaderboard", 0) or BlzGetFrameByName("Multiboard", 0). This are above SimpleFrames but one has to create them first with the none Frame api.

Your custom Frames can also be a child's child of that other Parents, the 4:3 Limitation is still gone.

A Lua example to spawn a ScriptDialogButton right to the command buttons using an unseeable "Leaderboard" as parent.

 Lua:

 CreateLeaderboardBJ(bj_FORCE_ALL_PLAYERS, "title")
 local parent = BlzGetFrameByName("Leaderboard", 0)
 BlzFrameSetSize(parent, 0, 0)
 BlzFrameSetVisible(BlzGetFrameByName("LeaderboardBackdrop", 0), false)
 BlzFrameSetVisible(BlzGetFrameByName("LeaderboardTitle", 0), false)

 local button1 = BlzCreateFrameByType("GLUETEXTBUTTON", "name", parent, "ScriptDialogButton", 0)

 BlzFrameSetAbsPoint(button1, FRAMEPOINT_LEFT , 0.85, 0.1)

First one creates the LeaderBoard with the normal api. Then one gets the new frame sets the size to 0 to make the screen space it takes clickable for the playable-world. Then one hides the Title and Backdrop Frame (to fix a grafic glitch). After that one uses it as parent of the Frame.

If one wants to use LeaderBoards, one has to save the parentFrame in a global and has to show it with BlzFrameSetVisible after the other was shown.

###
 Fullscreen Frame

Instead of using the Warcraft 3 4:3 positions one can pos frames relative to a custom created Frame which is fullscreen. That fullscreen size can be done by setting the size of the Frame every 0.xx seconds to

BlzFrameSetSize(relative, BlzGetLocalClientWidth()/BlzGetLocalClientHeight()*0.6, 0.6)

That Frame also would be placed to the Bottom of the screen:

BlzFrameSetAbsPoint(relative, FRAMEPOINT_BOTTOM, 0.4, 0.0)

The Frame has to be able to leave the 4:3 Screen for this to work.

When one now poses left or right of yours to left or right of that scaling frame. Then one has an frame that moves further outside when possible.

Inspired from The Big UI-Frame Tutorial

 Lua:

do
 local function GetParent()
-- local parent = BlzGetFrameByName("ConsoleUIBackdrop", 0)
-- if GetHandleId(parent) == 0 then
 CreateLeaderboardBJ(bj_FORCE_ALL_PLAYERS, "title")
 parent = BlzGetFrameByName("Leaderboard", 0)
 BlzFrameSetSize(parent, 0, 0)
 BlzFrameSetVisible(BlzGetFrameByName("LeaderboardBackdrop", 0), false)
 BlzFrameSetVisible(BlzGetFrameByName("LeaderboardTitle", 0), false)
-- end
 return parent
 end
 local function Update()
 BlzFrameSetSize(FullScreenFrame, BlzGetLocalClientWidth()/BlzGetLocalClientHeight()*0.6, 0.6)
 end

 local function InitFrames()
 local frame = BlzCreateFrameByType("FRAME", "FullScreenParent", GetParent(), "", 0)
 FullScreenParent = frame
 frame = BlzCreateFrameByType("FRAME", "Fullscreen", frame, "", 0)
 BlzFrameSetVisible(frame, false)
 BlzFrameSetSize(frame, 0.8, 0.6)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_BOTTOM, 0.4, 0)
 FullScreenFrame = frame

 end
 function InitFullScreenFrame()
 InitFrames()
 if FrameLoaderAdd then FrameLoaderAdd(InitFrames) end
 Repeat(0.2, Update) -- every 0.2s
 end
end

---
--- on the creation of someFrame
BlzFrameSetPoint(someFrame, FRAMEPOINT_LEFT, FullScreenFrame, FRAMEPOINT_LEFT, 0, 0) -- at left


 Last edited: Mar 5, 2024

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

Thank you very much for your detailed response! I will try the leaderboard thing when I can.

I'm confused though which frames can be moved out of the 4:3 screen area? You said in the OP that all simpleframes can be moved out of it, but are there other frames that can be?

#### Uncle

#### Uncle

Warcraft Moderator

 Level 76

JoinedAug 10, 2018Messages8,351

Resources: 1

#### Resources
Spells and Systems 1

There's only two types of frames, standard and simple. Standard are limited to 4:3 unless you do what Tasyen said here:

 One can free Frames from the 4:3 Screen limitation by creating them for a different Parent then GAME_UI, in Reforged such a Parent could be BlzGetFrameByName("ConsoleUIBackdrop",0). Using ("ConsoleUIBackdrop",0) as parent pushes them to a lower Layer, below SimpleFrames.


Click to expand...

If he doesn't specifically say Simple then assume he's referring to the "standard" frames.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Antares A said:

 Thank you very much for your detailed response! I will try the leaderboard thing when I can.

I'm confused though which frames can be moved out of the 4:3 screen area? You said in the OP that all simpleframes can be moved out of it, but are there other frames that can be?


Click to expand...

Actual the frame just needs some flag set (2^6). Than the frame starts a "widescreen" "layer", which allows all its children to go into widescreen. But well frame api nor fdf can set the flag (as far I know in fdf it should belong to LayerStyle but did not find it  ) . Therefore one needs this specific default frames that start this "widescreen" "layers".


 Last edited: Mar 6, 2024

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

I guess I misunderstood

"You need to do this to move frames into the widescreen area"

"Simpleframes can be moved into the widescreen area"

as both needing to be true instead of either.

I'll have to try it! Thanks!

#### Antares

#### Antares

Spell Reviewer

 Level 35

JoinedDec 13, 2009Messages1,107

Resources: 49

#### Resources
Icons 10Skins 1Maps 2Spells and Systems 31Sounds 1JASS 1Tutorials 3

It took a few tries but I got it working. Thank you very much!

Could you just elaborate why the periodic trigger to set the size of the parent frame is needed? Is this just for the unlikely scenario that a player changes their resolution midgame or is there another reason?


 Last edited: Apr 2, 2024

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Antares A said:

 Could you just elaborate why the periodic trigger to set the size of the parent frame is needed? Is this just for the unlikely scenario that a player changes their resolution midgame or is there another reason?


Click to expand...

yes, a user could resize the game during game thats why its done, it is also that during tabed out in fullscreen the size changes.

I dont see it unlikely as I play in window mode, but might be just my view.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

sotzaii_shuen (hive discord) found out how to use a "new" fdf action that makes simplebuttons far more useful. It is NAMED, it allows to give the functional children of simplebuttons names which then allows access inside map script and therefore dynamic changes during the match.

 Code:

Texture "TextureTemplate" {}
Frame "SIMPLEBUTTON" "TestA" {
 DecorateFileNames,
 Width 0.085,
 Height 0.022,
 NormalTexture NAMED "MyIconA2","TextureTemplate",
 PushedTexture NAMED "MyIconB2","TextureTemplate",
 DisabledTexture NAMED "MyIconC2","TextureTemplate",
 UseHighlight "MyUpperMenuButtonHighlight",
}

 Lua:

BlzLoadTOCFile("UI/Test.toc")
frame = BlzCreateSimpleFrame("TestA", BlzGetFrameByName("ConsoleUI", 0), 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.4, 0.33)
BlzFrameSetTexture(BlzGetFrameByName("MyIconA2", 0), BlzGetAbilityIcon(FourCC'Hpal'), 0, true)
BlzFrameSetTexture(BlzGetFrameByName("MyIconB2", 0), BlzGetAbilityIcon(FourCC'hfoo'), 0, true)
BlzFrameSetTexture(BlzGetFrameByName("MyIconC2", 0), BlzGetAbilityIcon(FourCC'ogru'), 0, true)

I made a demo map, in which one can select a unit to change the functional children texture of the simplebutton to the selected unit's icon.

He also found out that one can set the alphamode of Textures in a fdf oneline, can be done to all simpleframe types with texture children.

Like (Normal/Disabled/Highlight)Texture "(DISABLE/BLEND/ALPHAKEY/ADD)", "texture path",

And this also works for String-ChildFrames.

#### Attachments

SimpleButtonNamedChildAccess.w3x

 13.5 KB

 · Views: 33



#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

sotzaii_shuen (hive discord) found another useful fdf feature.

Backdrop & UseBackdrop this 2 allow to use one BACKDROP inside any simpleframe. The intention of them seems to be to make a box + background or only one of them around/below the simpleframe.

Define a Backdrop used by SimpleFrames with UseBackdrop or placed directly inside a SimpleFrame.

The Backdrop can not be accessed by BlzGetFrameByName and copies the Frames position & Size.

It can do most what Frame "BACKDROP" can do.

Layer does not work on it and the border is higher then the background but the Backdrop is inside the SimpleFrames Layer. Textures & Strings inside Layer "BACKGROUND" often are lower than the Backdrop

Will show no background when BackdropBackground is empty or invalid. But a Border File must be correct when it is requested with BackdropEdgeFile.

Always uses BackdropBlendAll.

Like Texture uses DecorateFileNames from the parent simpleframe.

BackdropMirrored does not work.

 Code:

Frame "SIMPLESTATUSBAR" "SimpleBackdrop2" {
 DecorateFileNames,
 Width 0.1,
 Height 0.1,
 SetPoint CENTER, "ConsoleUI", CENTER,0.0,0.1,
 Backdrop {
 BackdropTileBackground,
 BackdropBackground "EscMenuEditBoxBackground",
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.0125,
 BackdropBackgroundSize 0.1,
 BackdropBackgroundInsets 0.005 0.005 0.005 0.005,
 BackdropEdgeFile "EscMenuEditBoxBorder",
 }
}

Backdrop "TestBackdrop" {
 BackdropTileBackground,
 BackdropBackground "EscMenuEditBoxBackground",
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.0125,
 BackdropBackgroundSize 0.256,
 BackdropBackgroundInsets 0.005 0.005 0.005 0.005,
 BackdropEdgeFile "EscMenuEditBoxBorder",
}

Frame "SIMPLEFRAME" "SimpleBackdrop" {
 DecorateFileNames,
 Width 0.1,
 Height 0.1,
 SetPoint CENTER, "ConsoleUI", CENTER,0.0,0.1,
 UseBackdrop "TestBackdrop",
}

Frame "SIMPLEFRAME" "SimpleBackdrop3" {
 Width 0.1,
 Height 0.1,
 SetPoint CENTER, "ConsoleUI", CENTER,0.0,0.1,
 Backdrop {
 BackdropBackground "ReplaceableTextures/CommandButtons/BTNPriest",
 BackdropMirrored,
 }
 Layer "ARTWORK" {
 Texture {
 Width 0.05,
 Height 0.05,
 File "ReplaceableTextures/CommandButtons/BTNPriest",
 Anchor CENTER,0,0,
 }
 }
 Layer "BACKGROUND" {
 Texture {
 Width 0.05,
 Height 0.05,
 File "ReplaceableTextures/CommandButtons/BTNHeroPaladin",
 Anchor CENTER,0,0.05,
 }
 }
}

 You must log in or register to reply here.


### Similar threads

 UI: Reading a FDF


Tasyen

May 29, 2019

Scripting Tutorials

Replies15Views16K

Dec 1, 2020
Tasyen

 UI: Frames and Tooltips


Tasyen

Jun 27, 2019

Scripting Tutorials

Replies45Views21K

Oct 27, 2024
RPG_F

 UI - The concept of Parent-Frames


Tasyen

Nov 19, 2019

Scripting Tutorials

Replies20Views11K

Oct 10, 2020
Chaosy

 UI: Create a TextButton


Tasyen

May 30, 2019

Scripting Tutorials

Replies12Views12K

Jan 11, 2021
Tasyen

 UI: Creating a Bar


Tasyen

Jun 12, 2019

Scripting Tutorials

Replies38Views18K

Oct 2, 2025
KenArok

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