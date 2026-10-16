# The Big UI-Frame Tutorial

By Tasyen, Hive Workshop. Source https://www.hiveworkshop.com/pastebin/e23909d8468ff4942ccea268fbbcafd1.20598

Full text. Each diagram is linked to its source on Hive Workshop, and the original page is under `raw/`.

Table of Content

Introduction

GUI and Frames

Position on Screen & Frames (code driven)
Set Pos Natives

Relative Position

4:3 Limitation

Access Frames
Access Frames By Name

Change Text in Resource Lumber

Unit Info Panel

Access Frames by Child
Table

Origin-Frames

Hide Default UI

Move Default UI

Simple UI changes

Hovering Command/ItemButton

WhatTypeOfFrame

Frame Parent-Child

Frame Order by Level

Create Frames
BACKDROP without FDF

Text without FDF

TextButton without FDF

TextButton with AutoCast Model Without FDF

Sprite RockBoltMissile Without FDF

Icon Button without custom FDF

ScritpDialogButon 2 Icon Button

SimpleButton Icon without custom FDF

SimpleFrame - Text without custom FDF

Slider without custom FDF

Checkbox without FDF

ModelBar without FDF

2-Face without FDF

Bar without FDF

Create a Textarea in QuestDialog

Create A Frame that is always Left of Screen

Creating a Multipage Frame

Keyboard Focus

Tooltips
Example

BoxedText ToolTips Dynamic amount of Lines

SimpleFrame ToolTip

FrameEvents

Frames & Multiplayer

Frame Save & Load

TOC

FDF

FDF-Actions

Frame Pos in FDF

FrameTypes
Backdrop

(Glue)(Text)Button
Hotkeys

Checkbox

Dialog - Yes/No

EditBox - Textinput

ListBox

Frame

Highlight

Popupmenu

RadioGroup

Slider

Sprite

StatusBar

TextArea

Text

SimpleFrames
String

Texture

TextureFraction

Layer

SimpleButton

SimpleStatusBar

SimpleCheckBox

TextMarkUP String & TEXT
Fonts

FontFlags

Text-Alignment

Text-Shadow

Errors

##
 Introduction

With patch V1.31 for Warcraft 3, Blizzard added for map makers the capability to modify the ingame UI by code, with the same feature one can also create own custom UI-Elements. This UI-Elements are called Frames. The Minimap, Command-, Item-, Hero-, MenuButtons, the Chat, Multiboards, TimerDialogs, LeaderBoards, and many more which are not part of the playerable world. All of them are examples of such Frames.

Frames can display Text, Textures and Models. There are also interactive ones that can be used for user input like a Button, a Slider or an Editbox (a textfield the user can write text into). This UI-Features can use 2 FileTypes which one doesn't need otherwise: TOC and FDF.

While TOC and FDF are quite powerful one can use alot of UI-Features without custom ones.

Following content tries to help or describe using this UI-Frame-api introduced in V1.31.

##
 GUI and Frames

The nativ GUI-TriggerData for world Editor does not provide actions to use the frame natives.

One can extend the used data with "Allow Local Files" to have more actions in GUI and with that also Frame natives. World Editor provides its Trigger-features mostly with this 2 files "UI/TriggerData.txt" and "UI/TriggerStrings". You might find some mode that extends GUI data base with frame natives.

Grapes made a triggerdata-Mode for Reforged GrapesOfWath's Extended GUI

The gui example in this document, dont use the linked mode as I made it in Warcraft 3 V1.31.1 so there might be some differences.

###
 GUI and Frames has some problems though, which you should know, if you want to use it.

You should do Frame manipulation inside game time elapsed event triggers, 0seconds is enough. map init might give wrong/wonky results.

The 3rd digit after 0.xxx matters much in UI, but the input dialog only allows 0.xx values. maybe something like 1/1000 works.

Set/Add text action adds the string key for constant strings instead of the content ("triggerstr..."), use conversation Externalized string for such fixed strings. does not happen for "your house " + " my house"

TriggerActions need to run for all players in mp, dont have conditions like Frame Is Visible which would only some player run the actions -> multiplayer desync, if you want such use an if inside the Actions-Section of a Trigger.

All your custom Frame-Events need to be added over action "Trigger - Add New Event". The events itself are mp safe aswell as the Event Getter values Trigger Frame Value. But not the FrameGetText or FrameGetValue or Frame Is visible

One is going to use GetLocalPlayer() in some way when your map is played in multiplayer, learn how to use it without producing mp-desync.

##
 Position on Screen & Frames

Positions on Screen are an important knowlage when using this whole UI-Frame feature. Warcraft 3 was at the beginning a 4:3 only Game. Hence their own UI-FrameWork has a coordinate system that makes sense for the 4:3 part.

The Position goes from 0.0/0.0 at the Bottomleft of the minimap to the Topright with 0.8/0.6 (Topright of the Upkeep Level). The used Resolution does not matter, this values do not change. This Values are also used for Size of Frames. Pixel based size/Coords are only doable with some conversion math.

There exist 4 natives to manipulate the position of a Frame with code.

 JASS:

native BlzFrameSetPoint takes framehandle frame, framepointtype point, framehandle relative, framepointtype relativePoint, real x, real y returns nothing
native BlzFrameSetAbsPoint takes framehandle frame, framepointtype point, real x, real y returns nothing
native BlzFrameClearAllPoints takes framehandle frame returns nothing
native BlzFrameSetAllPoints takes framehandle frame, framehandle relative returns nothing

Each has great usage.

BlzFrameSetAbsPoint takes one point of a Frame unbound that point and places it to a specific coordinates on the screen.

BlzFrameSetPoint places a point of FrameA relative to a point of FrameB. When FrameB moves FrameA's point will keep this rule and moves with it. The x/y-Offset Values are affected by FrameA Scale.

BlzFrameClearAllPoints removes all curent bound points of that frame.

BlzFrameSetAllPoints FrameA will copy FrameB in size and position. FrameA will update when FrameB is changed later.

As one might see, warcraft's UI use a type named framepointtype, the constants for this type in code are:

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

Using different Points alters the result and can do some wanted Results more easy. Fiddling the left to the left part of the screen can be complicated when one moves the CENTER of a Frame.

one can place more then one point for a Frame. In such a case the taken space on the screen does not fit the frames size anymore. The game tries to fullfill all set points as good that is possible for a rectangle.

Therefore if one simply wants to move a Frame, one first has to clear all set points with BlzFrameClearAllPoints before doing the actual position setting.

When only one FramePoint is placed the choosen FramePoint has a strong impact on the result. It limites the directions the Frame expands.

When using a point containing:

LEFT can only extend to the right x wise ->.

RIGHT can only extend to the left x wise <-.

CENTER extends equal in all directions <->

TOP can only extend downwards y wise

BOTTOM can only extend upwards y wise.​

One can imagine Frames as rectangles when one sets size and position.

###
 Relative Position

Frames that belong logical to some other Frame benefit much when one sets their position relative to that frame they belong to or relative to a frame that is relative to the owner that could be contiuned, but in the end it is relative to the Owner Frame. This relative position is updated when the other Frame is moved. Because of that one only needs to set the position of the not relative posed Frame in that group and everything moves with it. In code the native doing that is BlzFrameSetPoint.

Setting Relative position is a little bit inversed. When one wants to place FrameA left to FrameB one needs to place the right side (of the moving) to the left of the target.

 JASS:

BlzFrameSetPoint(frameLeft, FRAMEPOINT_RIGHT, frameTarget, FRAMEPOINT_LEFT, 0, 0)
BlzFrameSetPoint(frameRight, FRAMEPOINT_LEFT, frameTarget, FRAMEPOINT_RIGHT, 0, 0)

The x and y offset in BlzFrameSetPoint are added to the target Point. with +x the position anchored is moved further to the right and with +y it is moved higher up. BlzFrameSetScale of the moved frame affect the x&y offset.

In Fdf one can set position only relative to Frames known by Name.

This Lua code creates 2 Backdrop one that has a fixed position the other frequently changes the used FramePoint to pos to the same position. This works out of the Box. It should display the impact of usind different FramePoints. As bigger the Frame is as more the used FramePoint matters.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 -- create the moving frame
 local frame = BlzCreateFrame("QuestButtonBaseTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 BlzFrameSetSize(frame, 0.1, 0.1)
 -- the not movingframe to show the difference more clear.
 local screenCenter = BlzCreateFrameByType("BACKDROP", "", frame, "", 0)
 BlzFrameSetSize(screenCenter, 0.02, 0.02)
 BlzFrameSetTexture(screenCenter, "replaceabletextures\\teamcolor\\teamcolor00", 0, false)
 BlzFrameSetAbsPoint(screenCenter, FRAMEPOINT_CENTER, 0.4, 0.3)

 -- every 0.4 seconds change the current used Framepoint
 local counter = 0
 TimerStart(CreateTimer(), 0.4, true, function()
 BlzFrameClearAllPoints(frame)
 BlzFrameSetAbsPoint(frame, ConvertFramePointType(counter), 0.4, 0.3)
 counter = counter + 1

 if counter > 8 then
 counter = 0
 end
 end)
 end
end

Creating a linked row of frames is doable without any math by using two opposite framepoints and BlzFrameSetPoint. The direction of the row is the 2. point used in the native. Such a Row will keep being linked as long only the frame that has no row Link is moved.

This BlzFrameSetPoint(newFrame, FRAMEPOINT_LEFT, prevFrame, FRAMEPOINT_RIGHT, 0, 0) results into Frame1, 2 and 3 ->. One could add a little space between them by using 0.003 for x offset.

While BlzFrameSetPoint(newFrame, FRAMEPOINT_RIGHT, prevFrame, FRAMEPOINT_LEFT , 0, 0) Results into Frame3, 2 and 1 <-. if one wants a small gab between beaware that now you have to use a negative xOffset, -0.003 for example. With a +xoffset the frames would overlap.

BlzFrameSetPoint(newFrame, FRAMEPOINT_TOP, prevFrame, FRAMEPOINT_BOTTOM, 0, 0) ↓

BlzFrameSetPoint(newFrame, FRAMEPOINT_BOTTOM, prevFrame, FRAMEPOINT_TOP, 0, 0) ↑

###
 4:3 Limitation

Most custom created Frames from the Frame group can not leave the 4:3 part of the screen. If a part of them leave it, they become malformed. A TEXT-Frame might cut off some chars. A BACKDROP becomes smaller and with that also Buttons and many others etc.

SimpleFrames are free from that limitation.

One can free Frames from that 4:3 limitation by creating them for a different Parent then GAME_UI, in Reforged such a Parent could be ("ConsoleUIBackdrop",0). Through that don't comes with a price. Using ("ConsoleUIBackdrop",0) as parent pushes them to a lower Layer, below SimpleFrames.

One also could use BlzGetFrameByName("Leaderboard", 0) or BlzGetFrameByName("Multiboard", 0). This are above SimpleFrames but one has to create them first with the none Frame api.

Your custom Frames can also be a child's child of that other Parents, the 4:3 Limitation is still gone.

A Lua example to spawn a ScriptDialogButton right to the command buttons using "Leaderboard" as parent.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 CreateLeaderboardBJ(bj_FORCE_ALL_PLAYERS, "title")
 local parent = BlzGetFrameByName("Leaderboard", 0)
 BlzFrameSetSize(parent, 0, 0)
 BlzFrameSetVisible(BlzGetFrameByName("LeaderboardBackdrop", 0), false)
 BlzFrameSetVisible(BlzGetFrameByName("LeaderboardTitle", 0), false)

 local button1 = BlzCreateFrameByType("GLUETEXTBUTTON", "name", parent, "ScriptDialogButton", 0)

 BlzFrameSetAbsPoint(button1, FRAMEPOINT_LEFT , 0.85, 0.1)
 end
end

First one creates the LeaderBoard with the normal api. Then one gets the new frame sets the size to 0 to make the screen space it takes clickable for the playable-world. Then one hides the Title and Backdrop Frame (to fix a grafic glitch). After that one uses it as parent of the Frame.

If one wants to use LeaderBoards, one has to save the parentFrame in a global and has to show it with BlzFrameSetVisible after the other was shown.

###
 Fullscreen Frame

Instead of using the Warcraft 3 4:3 positions one can pos frames relative to a custom created Frame which is fullscreen. That fullscreen size can be done by setting the size of the Frame every 0.xx seconds to

BlzFrameSetSize(relative, BlzGetLocalClientWidth()/BlzGetLocalClientHeight()*0.6, 0.6)

That Frame also would be placed to the Bottom of the screen:

BlzFrameSetAbsPoint(relative, FRAMEPOINT_BOTTOM, 0.4, 0.0)

The Frame has to be able to leave the 4:3 Screen for this to work.

Now one poses Frame relative to the relative.

##
 Access Frames

The Frames of warcraft 3 are stored in a storage which can be read with a name and a number this number is called CreateContext. The native that reads that storage: BlzGetFrameByName takes string name, integer createContext returns framehandle. With unique CreateContext one is capable to reuse the same frame (name) and is still capable to access all of them over their name which should kinda tell their behaviour. But CreateContext does not matter much for Frames having only one instance in such a case 0 is used. BlzGetFrameByName("ConsoleUI", 0) looks in that storage for a frame named "ConsoleUI" with CreateContext 0 and returns a reference to it, or null when such was not found. In Lua it does still return a framehandle with a HandleId of 0.

When a new Frame is created it will take the his slot in the storage, name & createContext. All ChildFrames created with the same Creation will also take their slots with their names and the same createcontext as the current creation.

BlzGetFrameByName is used to access child-Frames or to get builtin Frames like the UnitInfo Frames, Resource Frames and others. This is an List of Names for frames found in the Blizzard fdfs for warcraft 3 Default Names for BlzGetFrameByName.

LeaderBoard/Multiboard/TimerDialog do only work when the map created such ones.

LogDialog is only used in SinglePlayer.

Chat Frames are only used in Multiplayer.

Quest Frames are created after the Quest Button was clicked (can be forced with code, first line opens the quest dialog the second closes it)

call BlzFrameClick(BlzGetFrameByName("UpperButtonBarQuestsButton", 0))

 call BlzFrameClick(BlzGetFrameByName("QuestAcceptButton", 0))​

If there is no nr next to a name, one uses "0".

For example BlzGetFrameByName("HelpTextArea",0) would return the frame handling the help Text Area.

[attachment 374778](https://www.hiveworkshop.com/attachments/f10menuframes-jpg.374778/)[attachment 374779](https://www.hiveworkshop.com/attachments/helppanel-jpg.374779/)[attachment 374780](https://www.hiveworkshop.com/attachments/options-jpg.374780/)[attachment 374781](https://www.hiveworkshop.com/attachments/tipspanel-jpg.374781/)[attachment 374782](https://www.hiveworkshop.com/attachments/confirmquitpanel-jpg.374782/)[attachment 374783](https://www.hiveworkshop.com/attachments/insideendgamepanel-jpg.374783/)[attachment 374784](https://www.hiveworkshop.com/attachments/logdialog-jpg.374784/)[attachment 374786](https://www.hiveworkshop.com/attachments/alliancedialog-jpg.374786/)[attachment 374787](https://www.hiveworkshop.com/attachments/chatdialog-jpg.374787/)[attachment 374804](https://www.hiveworkshop.com/attachments/questdialog-jpg.374804/)[attachment 374847](https://www.hiveworkshop.com/attachments/resourcebarframe-jpg.374847/)[attachment 374848](https://www.hiveworkshop.com/attachments/upperbuttonbarframe-jpg.374848/)[attachment 374849](https://www.hiveworkshop.com/attachments/simpleinfopaneliconally-jpg.374849/)[attachment 374852](https://www.hiveworkshop.com/attachments/heroinfo-jpg.374852/)[attachment 374853](https://www.hiveworkshop.com/attachments/simpleinfopaneldestructabledetail-jpg.374853/)[attachment 374854](https://www.hiveworkshop.com/attachments/simpleinfopanelitemdetail-jpg.374854/)[attachment 374855](https://www.hiveworkshop.com/attachments/simpleinfopanelcargodetail-jpg.374855/)[attachment 374906](https://www.hiveworkshop.com/attachments/minimapframe-jpg.374906/)[attachment 374909](https://www.hiveworkshop.com/attachments/simpleinfopanelicons-jpg.374909/)[attachment 374910](https://www.hiveworkshop.com/attachments/simpleinfopanelunitdetail-jpg.374910/)[attachment 374911](https://www.hiveworkshop.com/attachments/simpleinfopanelbuildingdetail-jpg.374911/)

### 1.31

### 1.32.10

 Code:

AllianceAcceptButton
AllianceAcceptButtonText
AllianceBackdrop
AllianceCancelButton
AllianceCancelButtonText
AllianceDialog
AllianceDialogScrollBar
AllianceSlot [0 to 23]
AllianceTitle
AlliedVictoryCheckBox
AlliedVictoryLabel
AllyCheckBox [0 to 23]
AllyHeader
AmbientCheckBox
AmbientLabel
AnimQualityLabel
AnimQualityValue
BottomButtonPanel
ButtonBackdropTemplate
ButtonDisabledBackdropTemplate
ButtonDisabledPushedBackdropTemplate
ButtonPushedBackdropTemplate
CancelButtonText
ChatDialog
ChatBackdrop
ChatTitle
ChatPlayerRadioButton
ChatAlliesRadioButton
ChatObserversRadioButton
ChatEveryoneRadioButton
ChatPlayerLabel
ChatAlliesLabel
ChatObserversLabel
ChatEveryoneLabel
ChatPlayerMenu
ChatAcceptButton
ChatAcceptButtonText
ChatHistoryDisplayBackdrop
ChatHistoryDisplay
ChatHistoryScrollBar
ChatHistoryLabel
ChatInfoText
CinematicBottomBorder
CinematicDialogueText
CinematicPanel
CinematicPortrait
CinematicPortraitBackground
CinematicPortraitCover
CinematicScenePanel
CinematicSpeakerText
CinematicTopBorder
ColorBackdrop [0 to 23]
ColorBorder [0 to 23]
ConfirmQuitCancelButton
ConfirmQuitCancelButtonText
ConfirmQuitMessageText
ConfirmQuitPanel
ConfirmQuitQuitButton
ConfirmQuitQuitButtonText
ConfirmQuitTitleText
ConsoleUI
CustomKeysLabel
CustomKeysValue
DecoratedMapListBox
DeleteCancelButton
DeleteCancelButtonText
DeleteDeleteButton
DeleteDeleteButtonText
DeleteMessageText
DeleteOnly
DeleteTitleText
DifficultyLabel
DifficultyValue
EndGameButton
EndGameButtonText
EndGamePanel
EndGameTitleText
EnviroCheckBox
EnviroLabel
EscMenuBackdrop
EscMenuDeleteContainer
EscMenuMainPanel
EscMenuOptionsPanel
EscMenuOverwriteContainer
EscMenuSaveGamePanel
EscMenuSaveLoadContainer
EscOptionsLightsMenu
EscOptionsLightsPopupMenuArrow
EscOptionsLightsPopupMenuBackdrop
EscOptionsLightsPopupMenuDisabledBackdrop
EscOptionsLightsPopupMenuMenu
EscOptionsLightsPopupMenuTitle
EscOptionsOcclusionMenu
EscOptionsOcclusionPopupMenuArrow
EscOptionsOcclusionPopupMenuBackdrop
EscOptionsOcclusionPopupMenuDisabledBackdrop
EscOptionsOcclusionPopupMenuMenu
EscOptionsOcclusionPopupMenuTitle
EscOptionsParticlesMenu
EscOptionsParticlesPopupMenuArrow
EscOptionsParticlesPopupMenuBackdrop
EscOptionsParticlesPopupMenuDisabledBackdrop
EscOptionsParticlesPopupMenuMenu
EscOptionsParticlesPopupMenuTitle
EscOptionsResolutionMenu
EscOptionsResolutionPopupMenuArrow
EscOptionsResolutionPopupMenuBackdrop
EscOptionsResolutionPopupMenuDisabledBackdrop
EscOptionsResolutionPopupMenuMenu
EscOptionsResolutionPopupMenuTitle
EscOptionsShadowsMenu
EscOptionsShadowsPopupMenuArrow
EscOptionsShadowsPopupMenuBackdrop
EscOptionsShadowsPopupMenuDisabledBackdrop
EscOptionsShadowsPopupMenuMenu
EscOptionsShadowsPopupMenuTitle
EscOptionsWindowModeMenu
EscOptionsWindowModePopupMenuArrow
EscOptionsWindowModePopupMenuBackdrop
EscOptionsWindowModePopupMenuDisabledBackdrop
EscOptionsWindowModePopupMenuMenu
EscOptionsWindowModePopupMenuTitle
ExitButton
ExitButtonText
ExtraHighLatencyLabel
ExtraHighLatencyRadio
FileListFrame
FormationToggleCheckBox
FormationToggleLabel
GameplayButton
GameplayButtonText
GameplayPanel
GameplayTitleText
GameSpeedLabel
GameSpeedSlider
GameSpeedValue
GammaBrightLabel
GammaDarkLabel
GammaLabel
GammaSlider
GoldBackdrop [0 to 23]
GoldHeader
GoldText [0 to 23]
HealthBarsCheckBox
HealthBarsLabel
HelpButton
HelpButtonText
HelpOKButton
HelpOKButtonText
HelpPanel
HelpTextArea
HelpTitleText
HighLatencyLabel
HighLatencyRadio
InfoPanelIconAllyFoodIcon [7]
InfoPanelIconAllyFoodValue [7]
InfoPanelIconAllyGoldIcon [7]
InfoPanelIconAllyGoldValue [7]
InfoPanelIconAllyTitle [7]
InfoPanelIconAllyUpkeep [7]
InfoPanelIconAllyWoodIcon [7]
InfoPanelIconAllyWoodValue [7]
InfoPanelIconBackdrop [0 to 5]
InfoPanelIconHeroAgilityLabel [6]
InfoPanelIconHeroAgilityValue [6]
InfoPanelIconHeroIcon [6]
InfoPanelIconHeroIntellectLabel [6]
InfoPanelIconHeroIntellectValue [6]
InfoPanelIconHeroStrengthLabel [6]
InfoPanelIconHeroStrengthValue [6]
InfoPanelIconLabel [0 to 5]
InfoPanelIconLevel [0 to 5]
InfoPanelIconValue [0 to 5]
InsideConfirmQuitPanel
InsideEndGamePanel
InsideHelpPanel
InsideMainPanel
InsideTipsPanel
KeyScrollFastLabel
KeyScrollLabel
KeyScrollSlider
KeyScrollSlowLabel
LatencyInfo1
LatencyInfo2
Leaderboard
LeaderboardBackdrop
LeaderboardListContainer
LeaderboardTitle
LightsLabel
LoadGameButton
LoadGameButtonText
LoadGameCancelButton
LoadGameCancelButtonText
LoadGameLoadButton
LoadGameLoadButtonText
LoadGameTitleText
LoadOnly
LogArea
LogAreaBackdrop
LogAreaScrollBar
LogBackdrop
LogDialog
LogOkButton
LogOkButtonText
LogTitle
LowLatencyLabel
LowLatencyRadio
LumberBackdrop [0 to 23]
LumberHeader
LumberText [0 to 23]
MainPanel
MapListBoxBackdrop
MapListScrollBar
ModelDetailLabel
ModelDetailValue
MouseScrollDisable
MouseScrollDisableLabel
MouseScrollFastLabel
MouseScrollLabel
MouseScrollSlider
MouseScrollSlowLabel
MovementCheckBox
MovementLabel
Multiboard
MultiboardBackdrop
MultiboardListContainer
MultiboardMinimizeButton
MultiboardTitle
MultiboardTitleBackdrop
MusicCheckBox
MusicVolumeHighLabel
MusicVolumeLabel
MusicVolumeLowLabel
MusicVolumeSlider
NetworkButton
NetworkButtonText
NetworkLabel
NetworkPanel
NetworkTitleText
ObserverCameraCheckBox
ObserverCameraString
ObserverFogCheckBox
ObserverFogString
ObserverVisionMenu
ObserverVisionMenuArrow
ObserverVisionMenuBackdrop
ObserverVisionMenuDisabledBackdrop
ObserverVisionMenuTitle
ObserverVisionPopupMenu
ObserverVisionPopupMenuMenuBackdropTemplate
OcclusionLabel
OKButtonText
OptionsButton
OptionsButtonText
OptionsCancelButton
OptionsOKButton
OptionsPanel
OptionsPreviousButton
OptionsPreviousButtonText
OptionsTitleText
OverwriteCancelButton
OverwriteCancelButtonText
OverwriteMessageText
OverwriteOnly
OverwriteOverwriteButton
OverwriteOverwriteButtonText
OverwriteTitleText
ParticlesLabel
PauseButton
PauseButtonText
PlayerNameLabel [0 to 23]
PlayersHeader
PositionalCheckBox
PositionalLabel
PreviousButton
PreviousButtonText
ProviderLabel
ProviderValue
"QuestAcceptButton",0
"QuestAcceptButtonText",0
"QuestBackdrop",0
"QuestDetailsTitle",0
"QuestDialog",0
"QuestDisplay",0
"QuestDisplayBackdrop",0
"QuestDisplayScrollBar",0
"QuestItemListContainer",0
"QuestItemListItem",6
"QuestItemListItem",7
"QuestItemListItem",8
"QuestItemListItem",9
"QuestItemListItemTitle",6
"QuestItemListItemTitle",7
"QuestItemListItemTitle",8
"QuestItemListItemTitle",9
"QuestItemListScrollBar",0
"QuestListItem",0
"QuestListItem",1
"QuestListItem",2
"QuestListItem",3
"QuestListItem",4
"QuestListItem",5
"QuestListItemBorder",0
"QuestListItemBorder",1
"QuestListItemBorder",2
"QuestListItemBorder",3
"QuestListItemBorder",4
"QuestListItemBorder",5
"QuestListItemButton",0
"QuestListItemButton",1
"QuestListItemButton",2
"QuestListItemButton",3
"QuestListItemButton",4
"QuestListItemButton",5
"QuestListItemComplete",0
"QuestListItemComplete",1
"QuestListItemComplete",2
"QuestListItemComplete",3
"QuestListItemComplete",4
"QuestListItemComplete",5
"QuestListItemCompletedHighlight",0
"QuestListItemCompletedHighlight",1
"QuestListItemCompletedHighlight",2
"QuestListItemCompletedHighlight",3
"QuestListItemCompletedHighlight",4
"QuestListItemCompletedHighlight",5
"QuestListItemFailedHighlight",0
"QuestListItemFailedHighlight",1
"QuestListItemFailedHighlight",2
"QuestListItemFailedHighlight",3
"QuestListItemFailedHighlight",4
"QuestListItemFailedHighlight",5
"QuestListItemIconContainer",0
"QuestListItemIconContainer",1
"QuestListItemIconContainer",2
"QuestListItemIconContainer",3
"QuestListItemIconContainer",4
"QuestListItemIconContainer",5
"QuestListItemSelectedHighlight",0
"QuestListItemSelectedHighlight",1
"QuestListItemSelectedHighlight",2
"QuestListItemSelectedHighlight",3
"QuestListItemSelectedHighlight",4
"QuestListItemSelectedHighlight",5
"QuestListItemTitle",0
"QuestListItemTitle",1
"QuestListItemTitle",2
"QuestListItemTitle",3
"QuestListItemTitle",4
"QuestListItemTitle",5
"QuestMainContainer",0
"QuestMainListScrollBar",0
"QuestMainTitle",0
"QuestOptionalContainer",0
"QuestOptionalListScrollBar",0
"QuestOptionalTitle",0
"QuestSubtitleValue",0
"QuestTitleValue",0
QuitButton
QuitButtonText
ResolutionLabel
ResourceBarFrame
ResourceBarGoldText
ResourceBarLumberText
ResourceBarSupplyText
ResourceBarUpkeepText
ResourceTradingTitle
RestartButton
RestartButtonText
ReturnButton
ReturnButtonText
SaveAndLoad
SaveGameButton
SaveGameButtonText
SaveGameCancelButton
SaveGameCancelButtonText
SaveGameDeleteButton
SaveGameDeleteButtonText
SaveGameFileEditBox
SaveGameFileEditBoxText
SaveGameSaveButton
SaveGameSaveButtonText
SaveGameTitleText
SaveOnly
ShadowsLabel
SimpleBuildingActionLabel [0 & 1]
SimpleBuildingDescriptionValue [1]
SimpleBuildingNameValue [1]
SimpleBuildQueueBackdrop [1]
SimpleBuildTimeIndicator [0 & 1]
SimpleClassValue
SimpleDestructableNameValue [4]
SimpleHeroLevelBar
SimpleHoldDescriptionValue [2]
SimpleHoldNameValue [2]
SimpleInfoPanelBuildingDetail [1]
SimpleInfoPanelCargoDetail [2]
SimpleInfoPanelDestructableDetail [4]
SimpleInfoPanelIconAlly [7]
SimpleInfoPanelIconArmor [2]
SimpleInfoPanelIconDamage [0 & 1]
SimpleInfoPanelIconFood [4]
SimpleInfoPanelIconGold [5]
SimpleInfoPanelIconHero [6]
SimpleInfoPanelIconHeroText [6]
SimpleInfoPanelIconRank [3]
SimpleInfoPanelItemDetail [3]
SimpleInfoPanelUnitDetail
SimpleItemDescriptionValue [3]
SimpleItemNameValue [3]
SimpleNameValue
SimpleObserverPanel
SimpleProgressIndicator
SimpleUnitStatsPanel
SoundButton
SoundButtonText
SoundCheckBox
SoundPanel
SoundTitleText
SoundVolumeHighLabel
SoundVolumeLabel
SoundVolumeLowLabel
SoundVolumeSlider
SubgroupCheckBox
SubgroupLabel
SubtitlesCheckBox
SubtitlesLabel
TextureQualityLabel
TextureQualityValue
TimerDialog [0 to x]
TimerDialogBackdrop [0 to x]
TimerDialogValue [0 to x]
TimerDialogTitle [0 to x]
TipsBackButton
TipsBackButtonText
TipsButton
TipsButtonText
TipsNextButton
TipsNextButtonText
TipsOKButton
TipsOKButtonText
TipsPanel
TipsTextArea
TipsTitleText
TooltipsCheckBox
TooltipsLabel
UnitCheckBox
UnitLabel
UnitsCheckBox [0 to 23]
UnitsHeader
UpperButtonBarAlliesButton
UpperButtonBarChatButton
UpperButtonBarFrame
UpperButtonBarMenuButton
UpperButtonBarQuestsButton
VideoButton
VideoButtonText
VideoPanel
VideoTitleText
VisionCheckBox [0 to 23]
VisionHeader
VSyncCheckBox
VSyncLabel
WindowModeLabel
WouldTheRealOptionsTitleTextPleaseStandUp

 Code:

BrowseFrames = {
 {"AllianceAcceptButton", 0}
 ,{"AllianceAcceptButtonText", 0}
 ,{"AllianceBackdrop", 0}
 ,{"AllianceCancelButton", 0}
 ,{"AllianceCancelButtonText", 0}
 ,{"AllianceDialog", 0}
 ,{"AllianceDialogScrollBar", 0}
 ,{"AllianceSlot", 0}
 ,{"AllianceSlot", 1}
 ,{"AllianceSlot", 2}
 ,{"AllianceSlot", 3}
 ,{"AllianceSlot", 4}
 ,{"AllianceSlot", 5}
 ,{"AllianceSlot", 6}
 ,{"AllianceSlot", 7}
 ,{"AllianceSlot", 8}
 ,{"AllianceSlot", 9}
 ,{"AllianceSlot", 10}
 ,{"AllianceSlot", 11}
 ,{"AllianceSlot", 12}
 ,{"AllianceSlot", 13}
 ,{"AllianceSlot", 14}
 ,{"AllianceSlot", 15}
 ,{"AllianceSlot", 16}
 ,{"AllianceSlot", 17}
 ,{"AllianceSlot", 18}
 ,{"AllianceSlot", 19}
 ,{"AllianceSlot", 20}
 ,{"AllianceSlot", 21}
 ,{"AllianceSlot", 22}
 ,{"AllianceSlot", 23}
 ,{"AllianceTitle", 0}
 ,{"AlliedVictoryCheckBox", 0}
 ,{"AlliedVictoryLabel", 0}
 ,{"AllyCheckBox", 0}
 ,{"AllyCheckBox", 1}
 ,{"AllyCheckBox", 2}
 ,{"AllyCheckBox", 3}
 ,{"AllyCheckBox", 4}
 ,{"AllyCheckBox", 5}
 ,{"AllyCheckBox", 6}
 ,{"AllyCheckBox", 7}
 ,{"AllyCheckBox", 8}
 ,{"AllyCheckBox", 9}
 ,{"AllyCheckBox", 10}
 ,{"AllyCheckBox", 11}
 ,{"AllyCheckBox", 12}
 ,{"AllyCheckBox", 13}
 ,{"AllyCheckBox", 14}
 ,{"AllyCheckBox", 15}
 ,{"AllyCheckBox", 16}
 ,{"AllyCheckBox", 17}
 ,{"AllyCheckBox", 18}
 ,{"AllyCheckBox", 19}
 ,{"AllyCheckBox", 20}
 ,{"AllyCheckBox", 21}
 ,{"AllyCheckBox", 22}
 ,{"AllyCheckBox", 23}
 ,{"AllyHeader", 0}
 ,{"AmbientCheckBox", 0}
 ,{"AmbientLabel", 0}
 ,{"AnimQualityLabel", 0}
 ,{"AnimQualityValue", 0}
 ,{"BottomButtonPanel", 0}
 ,{"CancelButtonText", 0}
 ,{"ChatDialog", 0}
 ,{"ChatBackdrop", 0}
 ,{"ChatTitle", 0}
 ,{"ChatPlayerRadioButton", 0}
 ,{"ChatAlliesRadioButton", 0}
 ,{"ChatObserversRadioButton", 0}
 ,{"ChatEveryoneRadioButton", 0}
 ,{"ChatPlayerLabel", 0}
 ,{"ChatAlliesLabel", 0}
 ,{"ChatObserversLabel", 0}
 ,{"ChatEveryoneLabel", 0}
 ,{"ChatPlayerMenu", 0}
 ,{"ChatAcceptButton", 0}
 ,{"ChatAcceptButtonText", 0}
 ,{"ChatHistoryDisplayBackdrop", 0}
 ,{"ChatHistoryDisplay", 0}
 ,{"ChatHistoryScrollBar", 0}
 ,{"ChatHistoryLabel", 0}
 ,{"ChatInfoText", 0}
 ,{"CinematicBottomBorder", 0}
 ,{"CinematicDialogueText", 0}
 ,{"CinematicPanel", 0}
 ,{"CinematicPanelControlFrame", 0}
 ,{"CinematicPortrait", 0}
 ,{"CinematicPortraitBackground", 0}
 ,{"CinematicPortraitCover", 0}
 ,{"CinematicScenePanel", 0}
 ,{"CinematicSpeakerText", 0}
 ,{"CinematicTopBorder", 0}
 ,{"ColorBackdrop", 0}
 ,{"ColorBackdrop", 1}
 ,{"ColorBackdrop", 2}
 ,{"ColorBackdrop", 3}
 ,{"ColorBackdrop", 4}
 ,{"ColorBackdrop", 5}
 ,{"ColorBackdrop", 6}
 ,{"ColorBackdrop", 7}
 ,{"ColorBackdrop", 8}
 ,{"ColorBackdrop", 9}
 ,{"ColorBackdrop", 10}
 ,{"ColorBackdrop", 11}
 ,{"ColorBackdrop", 12}
 ,{"ColorBackdrop", 13}
 ,{"ColorBackdrop", 14}
 ,{"ColorBackdrop", 15}
 ,{"ColorBackdrop", 16}
 ,{"ColorBackdrop", 17}
 ,{"ColorBackdrop", 18}
 ,{"ColorBackdrop", 19}
 ,{"ColorBackdrop", 20}
 ,{"ColorBackdrop", 21}
 ,{"ColorBackdrop", 22}
 ,{"ColorBackdrop", 23}
 ,{"ColorBorder", 0}
 ,{"ColorBorder", 1}
 ,{"ColorBorder", 2}
 ,{"ColorBorder", 3}
 ,{"ColorBorder", 4}
 ,{"ColorBorder", 5}
 ,{"ColorBorder", 6}
 ,{"ColorBorder", 7}
 ,{"ColorBorder", 8}
 ,{"ColorBorder", 9}
 ,{"ColorBorder", 10}
 ,{"ColorBorder", 11}
 ,{"ColorBorder", 12}
 ,{"ColorBorder", 13}
 ,{"ColorBorder", 14}
 ,{"ColorBorder", 15}
 ,{"ColorBorder", 16}
 ,{"ColorBorder", 17}
 ,{"ColorBorder", 18}
 ,{"ColorBorder", 19}
 ,{"ColorBorder", 20}
 ,{"ColorBorder", 21}
 ,{"ColorBorder", 22}
 ,{"ColorBorder", 23}
 ,{"CommandBarFrame", 0}
 ,{"CommandButton_0", 0}
 ,{"CommandButton_1", 0}
 ,{"CommandButton_10", 0}
 ,{"CommandButton_11", 0}
 ,{"CommandButton_2", 0}
 ,{"CommandButton_3", 0}
 ,{"CommandButton_4", 0}
 ,{"CommandButton_5", 0}
 ,{"CommandButton_6", 0}
 ,{"CommandButton_7", 0}
 ,{"CommandButton_8", 0}
 ,{"CommandButton_9", 0}
 ,{"ConfirmQuitCancelButton", 0}
 ,{"ConfirmQuitCancelButtonText", 0}
 ,{"ConfirmQuitMessageText", 0}
 ,{"ConfirmQuitPanel", 0}
 ,{"ConfirmQuitQuitButton", 0}
 ,{"ConfirmQuitQuitButtonText", 0}
 ,{"ConfirmQuitTitleText", 0}
 ,{"ConsoleUI", 0}
 ,{"ConsoleUIBackdrop", 0}
 ,{"CustomKeysLabel", 0}
 ,{"CustomKeysValue", 0}
 ,{"DecoratedMapListBox", 0}
 ,{"DeleteCancelButton", 0}
 ,{"DeleteCancelButtonText", 0}
 ,{"DeleteDeleteButton", 0}
 ,{"DeleteDeleteButtonText", 0}
 ,{"DeleteMessageText", 0}
 ,{"DeleteOnly", 0}
 ,{"DeleteTitleText", 0}
 ,{"DifficultyLabel", 0}
 ,{"DifficultyValue", 0}
 ,{"EndGameButton", 0}
 ,{"EndGameButtonText", 0}
 ,{"EndGamePanel", 0}
 ,{"EndGameTitleText", 0}
 ,{"EnviroCheckBox", 0}
 ,{"EnviroLabel", 0}
 ,{"EscMenuBackdrop", 0}
 ,{"EscMenuDeleteContainer", 0}
 ,{"EscMenuMainPanel", 0}
 ,{"EscMenuOptionsPanel", 0}
 ,{"EscMenuOverwriteContainer", 0}
 ,{"EscMenuSaveGamePanel", 0}
 ,{"EscMenuSaveLoadContainer", 0}
 ,{"EscOptionsLightsMenu", 0}
 ,{"EscOptionsLightsPopupMenuArrow", 0}
 ,{"EscOptionsLightsPopupMenuBackdrop", 0}
 ,{"EscOptionsLightsPopupMenuDisabledBackdrop", 0}
 ,{"EscOptionsLightsPopupMenuMenu", 0}
 ,{"EscOptionsLightsPopupMenuTitle", 0}
 ,{"EscOptionsOcclusionMenu", 0}
 ,{"EscOptionsOcclusionPopupMenuArrow", 0}
 ,{"EscOptionsOcclusionPopupMenuBackdrop", 0}
 ,{"EscOptionsOcclusionPopupMenuDisabledBackdrop", 0}
 ,{"EscOptionsOcclusionPopupMenuMenu", 0}
 ,{"EscOptionsOcclusionPopupMenuTitle", 0}
 ,{"EscOptionsParticlesMenu", 0}
 ,{"EscOptionsParticlesPopupMenuArrow", 0}
 ,{"EscOptionsParticlesPopupMenuBackdrop", 0}
 ,{"EscOptionsParticlesPopupMenuDisabledBackdrop", 0}
 ,{"EscOptionsParticlesPopupMenuMenu", 0}
 ,{"EscOptionsParticlesPopupMenuTitle", 0}
 ,{"EscOptionsResolutionMenu", 0}
 ,{"EscOptionsResolutionPopupMenuArrow", 0}
 ,{"EscOptionsResolutionPopupMenuBackdrop", 0}
 ,{"EscOptionsResolutionPopupMenuDisabledBackdrop", 0}
 ,{"EscOptionsResolutionPopupMenuMenu", 0}
 ,{"EscOptionsResolutionPopupMenuTitle", 0}
 ,{"EscOptionsShadowsMenu", 0}
 ,{"EscOptionsShadowsPopupMenuArrow", 0}
 ,{"EscOptionsShadowsPopupMenuBackdrop", 0}
 ,{"EscOptionsShadowsPopupMenuDisabledBackdrop", 0}
 ,{"EscOptionsShadowsPopupMenuMenu", 0}
 ,{"EscOptionsShadowsPopupMenuTitle", 0}
 ,{"EscOptionsWindowModeMenu", 0}
 ,{"EscOptionsWindowModePopupMenuArrow", 0}
 ,{"EscOptionsWindowModePopupMenuBackdrop", 0}
 ,{"EscOptionsWindowModePopupMenuDisabledBackdrop", 0}
 ,{"EscOptionsWindowModePopupMenuMenu", 0}
 ,{"EscOptionsWindowModePopupMenuTitle", 0}
 ,{"ExitButton", 0}
 ,{"ExitButtonText", 0}
 ,{"ExtraHighLatencyLabel", 0}
 ,{"ExtraHighLatencyRadio", 0}
 ,{"FileListFrame", 0}
 ,{"FormationButton", 0}
 ,{"FormationToggleCheckBox", 0}
 ,{"FormationToggleLabel", 0}
 ,{"GameplayButton", 0}
 ,{"GameplayButtonText", 0}
 ,{"GameplayPanel", 0}
 ,{"GameplayTitleText", 0}
 ,{"GameSpeedLabel", 0}
 ,{"GameSpeedSlider", 0}
 ,{"GameSpeedValue", 0}
 ,{"GammaBrightLabel", 0}
 ,{"GammaDarkLabel", 0}
 ,{"GammaLabel", 0}
 ,{"GammaSlider", 0}
 ,{"GoldBackdrop", 0}
 ,{"GoldBackdrop", 1}
 ,{"GoldBackdrop", 2}
 ,{"GoldBackdrop", 3}
 ,{"GoldBackdrop", 4}
 ,{"GoldBackdrop", 5}
 ,{"GoldBackdrop", 6}
 ,{"GoldBackdrop", 7}
 ,{"GoldBackdrop", 8}
 ,{"GoldBackdrop", 9}
 ,{"GoldBackdrop", 10}
 ,{"GoldBackdrop", 11}
 ,{"GoldBackdrop", 12}
 ,{"GoldBackdrop", 13}
 ,{"GoldBackdrop", 14}
 ,{"GoldBackdrop", 15}
 ,{"GoldBackdrop", 16}
 ,{"GoldBackdrop", 17}
 ,{"GoldBackdrop", 18}
 ,{"GoldBackdrop", 19}
 ,{"GoldBackdrop", 20}
 ,{"GoldBackdrop", 21}
 ,{"GoldBackdrop", 22}
 ,{"GoldBackdrop", 23}
 ,{"GoldHeader", 0}
 ,{"GoldText", 0}
 ,{"GoldText", 1}
 ,{"GoldText", 2}
 ,{"GoldText", 3}
 ,{"GoldText", 4}
 ,{"GoldText", 5}
 ,{"GoldText", 6}
 ,{"GoldText", 7}
 ,{"GoldText", 8}
 ,{"GoldText", 9}
 ,{"GoldText", 10}
 ,{"GoldText", 11}
 ,{"GoldText", 12}
 ,{"GoldText", 13}
 ,{"GoldText", 14}
 ,{"GoldText", 15}
 ,{"GoldText", 16}
 ,{"GoldText", 17}
 ,{"GoldText", 18}
 ,{"GoldText", 19}
 ,{"GoldText", 20}
 ,{"GoldText", 21}
 ,{"GoldText", 22}
 ,{"GoldText", 23}
 ,{"HDCinematicBackground", 0}
 ,{"HDCinematicPortraitCover", 0}
 ,{"HealthBarsCheckBox", 0}
 ,{"HealthBarsLabel", 0}
 ,{"HelpButton", 0}
 ,{"HelpButtonText", 0}
 ,{"HelpOKButton", 0}
 ,{"HelpOKButtonText", 0}
 ,{"HelpPanel", 0}
 ,{"HelpTextArea", 0}
 ,{"HelpTitleText", 0}
 ,{"HighLatencyLabel", 0}
 ,{"HighLatencyRadio", 0}
 ,{"InfoPanelIconAllyFoodIcon", 7}
 ,{"InfoPanelIconAllyFoodValue", 7}
 ,{"InfoPanelIconAllyGoldIcon", 7}
 ,{"InfoPanelIconAllyGoldValue", 7}
 ,{"InfoPanelIconAllyTitle", 7}
 ,{"InfoPanelIconAllyUpkeep", 7}
 ,{"InfoPanelIconAllyWoodIcon", 7}
 ,{"InfoPanelIconAllyWoodValue", 7}
 ,{"InfoPanelIconBackdrop", 0}
 ,{"InfoPanelIconBackdrop", 1}
 ,{"InfoPanelIconBackdrop", 2}
 ,{"InfoPanelIconBackdrop", 3}
 ,{"InfoPanelIconBackdrop", 4}
 ,{"InfoPanelIconBackdrop", 5}
 ,{"InfoPanelIconHeroAgilityLabel", 6}
 ,{"InfoPanelIconHeroAgilityValue", 6}
 ,{"InfoPanelIconHeroIcon", 6}
 ,{"InfoPanelIconHeroIntellectLabel", 6}
 ,{"InfoPanelIconHeroIntellectValue", 6}
 ,{"InfoPanelIconHeroStrengthLabel", 6}
 ,{"InfoPanelIconHeroStrengthValue", 6}
 ,{"InfoPanelIconLabel", 0}
 ,{"InfoPanelIconLabel", 1}
 ,{"InfoPanelIconLabel", 2}
 ,{"InfoPanelIconLabel", 3}
 ,{"InfoPanelIconLabel", 4}
 ,{"InfoPanelIconLabel", 5}
 ,{"InfoPanelIconLevel", 0}
 ,{"InfoPanelIconLevel", 1}
 ,{"InfoPanelIconLevel", 2}
 ,{"InfoPanelIconLevel", 3}
 ,{"InfoPanelIconLevel", 4}
 ,{"InfoPanelIconLevel", 5}
 ,{"InfoPanelIconValue", 0}
 ,{"InfoPanelIconValue", 1}
 ,{"InfoPanelIconValue", 2}
 ,{"InfoPanelIconValue", 3}
 ,{"InfoPanelIconValue", 4}
 ,{"InfoPanelIconValue", 5}
 ,{"InsideConfirmQuitPanel", 0}
 ,{"InsideEndGamePanel", 0}
 ,{"InsideHelpPanel", 0}
 ,{"InsideMainPanel", 0}
 ,{"InsideTipsPanel", 0}
 ,{"InventoryButton_0", 0}
 ,{"InventoryButton_1", 0}
 ,{"InventoryButton_2", 0}
 ,{"InventoryButton_3", 0}
 ,{"InventoryButton_4", 0}
 ,{"InventoryButton_5", 0}
 ,{"InventoryCoverTexture", 0}
 ,{"InventoryText", 0}
 ,{"KeyScrollFastLabel", 0}
 ,{"KeyScrollLabel", 0}
 ,{"KeyScrollSlider", 0}
 ,{"KeyScrollSlowLabel", 0}
 ,{"LatencyInfo1", 0}
 ,{"LatencyInfo2", 0}
 ,{"Leaderboard", 0}
 ,{"LeaderboardBackdrop", 0}
 ,{"LeaderboardListContainer", 0}
 ,{"LeaderboardTitle", 0}
 ,{"LightsLabel", 0}
 ,{"LoadGameButton", 0}
 ,{"LoadGameButtonText", 0}
 ,{"LoadGameCancelButton", 0}
 ,{"LoadGameCancelButtonText", 0}
 ,{"LoadGameLoadButton", 0}
 ,{"LoadGameLoadButtonText", 0}
 ,{"LoadGameTitleText", 0}
 ,{"LoadOnly", 0}
 ,{"LogArea", 0}
 ,{"LogAreaBackdrop", 0}
 ,{"LogAreaScrollBar", 0}
 ,{"LogBackdrop", 0}
 ,{"LogDialog", 0}
 ,{"LogOkButton", 0}
 ,{"LogOkButtonText", 0}
 ,{"LogTitle", 0}
 ,{"LowLatencyLabel", 0}
 ,{"LowLatencyRadio", 0}
 ,{"LumberBackdrop", 0}
 ,{"LumberBackdrop", 1}
 ,{"LumberBackdrop", 2}
 ,{"LumberBackdrop", 3}
 ,{"LumberBackdrop", 4}
 ,{"LumberBackdrop", 5}
 ,{"LumberBackdrop", 6}
 ,{"LumberBackdrop", 7}
 ,{"LumberBackdrop", 8}
 ,{"LumberBackdrop", 9}
 ,{"LumberBackdrop", 10}
 ,{"LumberBackdrop", 11}
 ,{"LumberBackdrop", 12}
 ,{"LumberBackdrop", 13}
 ,{"LumberBackdrop", 14}
 ,{"LumberBackdrop", 15}
 ,{"LumberBackdrop", 16}
 ,{"LumberBackdrop", 17}
 ,{"LumberBackdrop", 18}
 ,{"LumberBackdrop", 19}
 ,{"LumberBackdrop", 20}
 ,{"LumberBackdrop", 21}
 ,{"LumberBackdrop", 22}
 ,{"LumberBackdrop", 23}
 ,{"LumberHeader", 0}
 ,{"LumberText", 0}
 ,{"LumberText", 1}
 ,{"LumberText", 2}
 ,{"LumberText", 3}
 ,{"LumberText", 4}
 ,{"LumberText", 5}
 ,{"LumberText", 6}
 ,{"LumberText", 7}
 ,{"LumberText", 8}
 ,{"LumberText", 9}
 ,{"LumberText", 10}
 ,{"LumberText", 11}
 ,{"LumberText", 12}
 ,{"LumberText", 13}
 ,{"LumberText", 14}
 ,{"LumberText", 15}
 ,{"LumberText", 16}
 ,{"LumberText", 17}
 ,{"LumberText", 18}
 ,{"LumberText", 19}
 ,{"LumberText", 20}
 ,{"LumberText", 21}
 ,{"LumberText", 22}
 ,{"LumberText", 23}
 ,{"MainPanel", 0}
 ,{"MapListBoxBackdrop", 0}
 ,{"MapListScrollBar", 0}
 ,{"MiniMapAllyButton", 0}
 ,{"MinimapButtonBar", 0}
 ,{"MiniMapCreepButton", 0}
 ,{"MiniMapFrame", 0}
 ,{"MinimapSignalButton", 0}
 ,{"MiniMapTerrainButton", 0}
 ,{"ModelDetailLabel", 0}
 ,{"ModelDetailValue", 0}
 ,{"MouseScrollDisable", 0}
 ,{"MouseScrollDisableLabel", 0}
 ,{"MouseScrollFastLabel", 0}
 ,{"MouseScrollLabel", 0}
 ,{"MouseScrollSlider", 0}
 ,{"MouseScrollSlowLabel", 0}
 ,{"MovementCheckBox", 0}
 ,{"MovementLabel", 0}
 ,{"Multiboard", 0}
 ,{"MultiboardBackdrop", 0}
 ,{"MultiboardListContainer", 0}
 ,{"MultiboardMinimizeButton", 0}
 ,{"MultiboardTitle", 0}
 ,{"MultiboardTitleBackdrop", 0}
 ,{"Multiboard", 1638} --AllyResourceDisplay
 ,{"MusicCheckBox", 0}
 ,{"MusicVolumeHighLabel", 0}
 ,{"MusicVolumeLabel", 0}
 ,{"MusicVolumeLowLabel", 0}
 ,{"MusicVolumeSlider", 0}
 ,{"NetworkButton", 0}
 ,{"NetworkButtonText", 0}
 ,{"NetworkLabel", 0}
 ,{"NetworkPanel", 0}
 ,{"NetworkTitleText", 0}
 ,{"OcclusionLabel", 0}
 ,{"OKButtonText", 0}
 ,{"OptionsButton", 0}
 ,{"OptionsButtonText", 0}
 ,{"OptionsCancelButton", 0}
 ,{"OptionsOKButton", 0}
 ,{"OptionsPanel", 0}
 ,{"OptionsPreviousButton", 0}
 ,{"OptionsPreviousButtonText", 0}
 ,{"OptionsTitleText", 0}
 ,{"OverwriteCancelButton", 0}
 ,{"OverwriteCancelButtonText", 0}
 ,{"OverwriteMessageText", 0}
 ,{"OverwriteOnly", 0}
 ,{"OverwriteOverwriteButton", 0}
 ,{"OverwriteOverwriteButtonText", 0}
 ,{"OverwriteTitleText", 0}
 ,{"ParticlesLabel", 0}
 ,{"PauseButton", 0}
 ,{"PauseButtonText", 0}
 ,{"PlayerNameLabel", 0}
 ,{"PlayerNameLabel", 1}
 ,{"PlayerNameLabel", 2}
 ,{"PlayerNameLabel", 3}
 ,{"PlayerNameLabel", 4}
 ,{"PlayerNameLabel", 5}
 ,{"PlayerNameLabel", 6}
 ,{"PlayerNameLabel", 7}
 ,{"PlayerNameLabel", 8}
 ,{"PlayerNameLabel", 9}
 ,{"PlayerNameLabel", 10}
 ,{"PlayerNameLabel", 11}
 ,{"PlayerNameLabel", 12}
 ,{"PlayerNameLabel", 13}
 ,{"PlayerNameLabel", 14}
 ,{"PlayerNameLabel", 15}
 ,{"PlayerNameLabel", 16}
 ,{"PlayerNameLabel", 17}
 ,{"PlayerNameLabel", 18}
 ,{"PlayerNameLabel", 19}
 ,{"PlayerNameLabel", 20}
 ,{"PlayerNameLabel", 21}
 ,{"PlayerNameLabel", 22}
 ,{"PlayerNameLabel", 23}
 ,{"PlayersHeader", 0}
 ,{"PositionalCheckBox", 0}
 ,{"PositionalLabel", 0}
 ,{"PreviousButton", 0}
 ,{"PreviousButtonText", 0}
 ,{"ProviderLabel", 0}
 ,{"ProviderValue", 0}
 ,{"QuestAcceptButton",0}
 ,{"QuestAcceptButtonText",0}
 ,{"QuestBackdrop",0}
 ,{"QuestDetailsTitle",0}
 ,{"QuestDialog",0}
 ,{"QuestDisplay",0}
 ,{"QuestDisplayBackdrop",0}
 ,{"QuestDisplayScrollBar",0}
 ,{"QuestItemListContainer",0}
 ,{"QuestItemListItem",6}
 ,{"QuestItemListItem",7}
 ,{"QuestItemListItem",8}
 ,{"QuestItemListItem",9}
 ,{"QuestItemListItemTitle",6}
 ,{"QuestItemListItemTitle",7}
 ,{"QuestItemListItemTitle",8}
 ,{"QuestItemListItemTitle",9}
 ,{"QuestItemListScrollBar",0}
 ,{"QuestListItem",0}
 ,{"QuestListItem",1}
 ,{"QuestListItem",2}
 ,{"QuestListItem",3}
 ,{"QuestListItem",4}
 ,{"QuestListItem",5}
 ,{"QuestListItemBorder",0}
 ,{"QuestListItemBorder",1}
 ,{"QuestListItemBorder",2}
 ,{"QuestListItemBorder",3}
 ,{"QuestListItemBorder",4}
 ,{"QuestListItemBorder",5}
 ,{"QuestListItemButton",0}
 ,{"QuestListItemButton",1}
 ,{"QuestListItemButton",2}
 ,{"QuestListItemButton",3}
 ,{"QuestListItemButton",4}
 ,{"QuestListItemButton",5}
 ,{"QuestListItemComplete",0}
 ,{"QuestListItemComplete",1}
 ,{"QuestListItemComplete",2}
 ,{"QuestListItemComplete",3}
 ,{"QuestListItemComplete",4}
 ,{"QuestListItemComplete",5}
 ,{"QuestListItemCompletedHighlight",0}
 ,{"QuestListItemCompletedHighlight",1}
 ,{"QuestListItemCompletedHighlight",2}
 ,{"QuestListItemCompletedHighlight",3}
 ,{"QuestListItemCompletedHighlight",4}
 ,{"QuestListItemCompletedHighlight",5}
 ,{"QuestListItemFailedHighlight",0}
 ,{"QuestListItemFailedHighlight",1}
 ,{"QuestListItemFailedHighlight",2}
 ,{"QuestListItemFailedHighlight",3}
 ,{"QuestListItemFailedHighlight",4}
 ,{"QuestListItemFailedHighlight",5}
 ,{"QuestListItemIconContainer",0}
 ,{"QuestListItemIconContainer",1}
 ,{"QuestListItemIconContainer",2}
 ,{"QuestListItemIconContainer",3}
 ,{"QuestListItemIconContainer",4}
 ,{"QuestListItemIconContainer",5}
 ,{"QuestListItemSelectedHighlight",0}
 ,{"QuestListItemSelectedHighlight",1}
 ,{"QuestListItemSelectedHighlight",2}
 ,{"QuestListItemSelectedHighlight",3}
 ,{"QuestListItemSelectedHighlight",4}
 ,{"QuestListItemSelectedHighlight",5}
 ,{"QuestListItemTitle",0}
 ,{"QuestListItemTitle",1}
 ,{"QuestListItemTitle",2}
 ,{"QuestListItemTitle",3}
 ,{"QuestListItemTitle",4}
 ,{"QuestListItemTitle",5}
 ,{"QuestMainContainer",0}
 ,{"QuestMainListScrollBar",0}
 ,{"QuestMainTitle",0}
 ,{"QuestOptionalContainer",0}
 ,{"QuestOptionalListScrollBar",0}
 ,{"QuestOptionalTitle",0}
 ,{"QuestSubtitleValue",0}
 ,{"QuestTitleValue",0}
 ,{"QuitButton", 0}
 ,{"QuitButtonText", 0}
 ,{"ResolutionLabel", 0}
 ,{"ResourceBarFrame", 0}
 ,{"ResourceBarGoldText", 0}
 ,{"ResourceBarLumberText", 0}
 ,{"ResourceBarSupplyText", 0}
 ,{"ResourceBarUpkeepText", 0}
 ,{"ResourceTradingTitle", 0}
 ,{"RestartButton", 0}
 ,{"RestartButtonText", 0}
 ,{"ReturnButton", 0}
 ,{"ReturnButtonText", 0}
 ,{"SaveAndLoad", 0}
 ,{"SaveGameButton", 0}
 ,{"SaveGameButtonText", 0}
 ,{"SaveGameCancelButton", 0}
 ,{"SaveGameCancelButtonText", 0}
 ,{"SaveGameDeleteButton", 0}
 ,{"SaveGameDeleteButtonText", 0}
 ,{"SaveGameFileEditBox", 0}
 ,{"SaveGameFileEditBoxText", 0}
 ,{"SaveGameSaveButton", 0}
 ,{"SaveGameSaveButtonText", 0}
 ,{"SaveGameTitleText", 0}
 ,{"SaveOnly", 0}
 ,{"ShadowsLabel", 0}
 ,{"SimpleBuildingActionLabel", 0}
 ,{"SimpleBuildingActionLabel", 1}
 ,{"SimpleBuildingDescriptionValue", 1}
 ,{"SimpleBuildingNameValue", 1}
 ,{"SimpleBuildQueueBackdrop", 1}
 ,{"SimpleBuildTimeIndicator", 0}
 ,{"SimpleBuildTimeIndicator", 1}
 ,{"SimpleClassValue", 0}
 ,{"SimpleDestructableNameValue", 4}
 ,{"SimpleHeroLevelBar", 0}
 ,{"SimpleHoldDescriptionValue", 2}
 ,{"SimpleHoldNameValue", 2}
 ,{"SimpleInfoPanelBuildingDetail", 1}
 ,{"SimpleInfoPanelCargoDetail", 2}
 ,{"SimpleInfoPanelDestructableDetail", 4}
 ,{"SimpleInfoPanelIconAlly", 7}
 ,{"SimpleInfoPanelIconArmor", 2}
 ,{"SimpleInfoPanelIconDamage", 0}
 ,{"SimpleInfoPanelIconDamage", 1}
 ,{"SimpleInfoPanelIconFood", 4}
 ,{"SimpleInfoPanelIconGold", 5}
 ,{"SimpleInfoPanelIconHero", 6}
 ,{"SimpleInfoPanelIconHeroText", 6}
 ,{"SimpleInfoPanelIconRank", 3}
 ,{"SimpleInfoPanelItemDetail", 3}
 ,{"SimpleInfoPanelUnitDetail", 0}
 ,{"SimpleInventoryBar", 0}
 ,{"SimpleInventoryCover", 0}
 ,{"SimpleItemDescriptionValue", 3}
 ,{"SimpleItemNameValue", 3}
 ,{"SimpleNameValue", 0}
 ,{"SimpleProgressIndicator", 0}
 ,{"SimpleUnitStatsPanel", 0}
 ,{"SoundButton", 0}
 ,{"SoundButtonText", 0}
 ,{"SoundCheckBox", 0}
 ,{"SoundPanel", 0}
 ,{"SoundTitleText", 0}
 ,{"SoundVolumeHighLabel", 0}
 ,{"SoundVolumeLabel", 0}
 ,{"SoundVolumeLowLabel", 0}
 ,{"SoundVolumeSlider", 0}
 ,{"SubgroupCheckBox", 0}
 ,{"SubgroupLabel", 0}
 ,{"SubtitlesCheckBox", 0}
 ,{"SubtitlesLabel", 0}
 ,{"TextureQualityLabel", 0}
 ,{"TextureQualityValue", 0}
 ,{"TipsBackButton", 0}
 ,{"TipsBackButtonText", 0}
 ,{"TipsButton", 0}
 ,{"TipsButtonText", 0}
 ,{"TipsNextButton", 0}
 ,{"TipsNextButtonText", 0}
 ,{"TipsOKButton", 0}
 ,{"TipsOKButtonText", 0}
 ,{"TipsPanel", 0}
 ,{"TipsTextArea", 0}
 ,{"TipsTitleText", 0}
 ,{"TooltipsCheckBox", 0}
 ,{"TooltipsLabel", 0}
 ,{"UnitCheckBox", 0}
 ,{"UnitLabel", 0}
 ,{"UnitsCheckBox", 0}
 ,{"UnitsCheckBox", 1}
 ,{"UnitsCheckBox", 2}
 ,{"UnitsCheckBox", 3}
 ,{"UnitsCheckBox", 4}
 ,{"UnitsCheckBox", 5}
 ,{"UnitsCheckBox", 6}
 ,{"UnitsCheckBox", 7}
 ,{"UnitsCheckBox", 8}
 ,{"UnitsCheckBox", 9}
 ,{"UnitsCheckBox", 10}
 ,{"UnitsCheckBox", 11}
 ,{"UnitsCheckBox", 12}
 ,{"UnitsCheckBox", 13}
 ,{"UnitsCheckBox", 14}
 ,{"UnitsCheckBox", 15}
 ,{"UnitsCheckBox", 16}
 ,{"UnitsCheckBox", 17}
 ,{"UnitsCheckBox", 18}
 ,{"UnitsCheckBox", 19}
 ,{"UnitsCheckBox", 20}
 ,{"UnitsCheckBox", 21}
 ,{"UnitsCheckBox", 22}
 ,{"UnitsCheckBox", 23}
 ,{"UnitsHeader", 0}
 ,{"UpperButtonBarAlliesButton", 0}
 ,{"UpperButtonBarChatButton", 0}
 ,{"UpperButtonBarFrame", 0}
 ,{"UpperButtonBarMenuButton", 0}
 ,{"UpperButtonBarQuestsButton", 0}
 ,{"VideoButton", 0}
 ,{"VideoButtonText", 0}
 ,{"VideoPanel", 0}
 ,{"VideoTitleText", 0}
 ,{"VisionCheckBox", 0}
 ,{"VisionCheckBox", 1}
 ,{"VisionCheckBox", 2}
 ,{"VisionCheckBox", 3}
 ,{"VisionCheckBox", 4}
 ,{"VisionCheckBox", 5}
 ,{"VisionCheckBox", 6}
 ,{"VisionCheckBox", 7}
 ,{"VisionCheckBox", 8}
 ,{"VisionCheckBox", 9}
 ,{"VisionCheckBox", 10}
 ,{"VisionCheckBox", 11}
 ,{"VisionCheckBox", 12}
 ,{"VisionCheckBox", 13}
 ,{"VisionCheckBox", 14}
 ,{"VisionCheckBox", 15}
 ,{"VisionCheckBox", 16}
 ,{"VisionCheckBox", 17}
 ,{"VisionCheckBox", 18}
 ,{"VisionCheckBox", 19}
 ,{"VisionCheckBox", 20}
 ,{"VisionCheckBox", 21}
 ,{"VisionCheckBox", 22}
 ,{"VisionCheckBox", 23}
 ,{"VisionHeader", 0}
 ,{"VSyncCheckBox", 0}
 ,{"VSyncLabel", 0}
 ,{"WindowModeLabel", 0}
 ,{"WouldTheRealOptionsTitleTextPleaseStandUp", 0}
 }

There are also 2 further ways to access frames one is BlzGetOriginFrame which allows to get frames Blizzard provided a special access to. And new with Warcraft 3 V1.32.6 one can get a childFrame at an index. Using this 2 natives;

 JASS:

native BlzFrameGetChildrenCount takes framehandle frame returns integer
native BlzFrameGetChild takes framehandle frame, integer index returns framehandle

###
 Change Text in Resource Lumber

This is a small example to change the displayed Text in the Lumber ResourceField at top Right of the screen. The first thing one has to do is to get a reference to the wanted Frame. Here BlzGetFrameByName is used. After that one uses BlzFrameSetText takes framehandle frame, string text returns nothing with the wanted result and it is done.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 -- get the frame named "ResourceBarLumberText" with creationcontext 0
 local frame = BlzGetFrameByName("ResourceBarLumberText" , 0)
 -- change the current displayed Text to "Lumber"
 BlzFrameSetText(frame , "Lumber")
 end
end

SetLumberText
Events
Time - Elapsed game time is 1.00 seconds

Conditions

Actions
Frame - Set Text of: (Get Frame By Name:ResourceBarLumberText Context:0) to (Externalized string(Lumber))

When having the Lua code in your map your screen should look kinda like that.

As soon the local player needs to update the lumber text, your custom text will be lost.

One could also change the text shown in the other resourceboxes, By using a different name inside BlzGetFrameByName.

For Gold use: "ResourceBarGoldText"

Lumber is named "ResourceBarLumberText"

Food is "ResourceBarSupplyText"

For the box showing the Upkeep level (no, low, high) use "ResourceBarUpkeepText"
Using BlzGetFrameByName for only one player can desync the game, hence it is recommended to set it to variable before the GetLocalPlayer Block and using that variable inside the Local Block.

###
 Unit Info Panel

The Unit Info Panel the one at the bottom center of the UI when only one unit is selected can be found in the fdf "ui\framedef\ui\simpleinfopanel.fdf". They are all SimpleFrames.

BlzGetFrameByName("SimpleInfoPanelIconDamage",0)

"SimpleInfoPanelIconDamage",1

"SimpleInfoPanelIconArmor",2

"SimpleInfoPanelIconRank",3

"SimpleInfoPanelIconFood",4

"SimpleInfoPanelIconGold",5

This frames are shown/hidden based on the current's units features.

They have Child-Frames with the same names, only the createContext differs.

"InfoPanelIconBackdrop" the image

"InfoPanelIconLevel" the shown techLevel (text)

"InfoPanelIconLabel" Damage/Armor..

"InfoPanelIconValue" the amount

BlzGetFrameByName("InfoPanelIconBackdrop", 0) would access the iconFrame of Attack 1 (object Editor).

Moving this UnitInfo frames is best done by moving the IconFrames: "InfoPanelIconBackdrop", Because The Texts pos themself relative to the IconFrame and the Container Frames like "SimpleInfoPanelIconDamage" are frequently reposed based on the current Unit's need. With breaking the bound from the Icon to the container one still has the whole functionality but total control over the position.

One can read and write onto this Frames but except for "InfoPanelIconLabel" the game update the displayed content of this frames very often. One could get rid of some of this displayed Texts by using BlzFrameSetFont(frame, "",0,0) onto it, or by moving it away. When one reads this Text one has to remember that the displayed value is a local only thing for anyone having a different unit selected the value will not match, so be careful what you do with that.

All of the UnitInfo-Hero Frames use 6 as CreateContext.

"SimpleInfoPanelIconHero" is the container.

"InfoPanelIconHeroIcon" is the Big Icon showing the primary Attribute.

"SimpleInfoPanelIconHeroText" is an additional logical container Frame for the Attribute String-Frames.

The visibility of the container Frames are a way to know that the local player has selected one hero only. Or a good Parent for custom SimpleFrames displaying something for heroes only.

"InfoPanelIconHeroStrengthLabel"

"InfoPanelIconHeroStrengthValue"

"InfoPanelIconHeroAgilityLabel"

"InfoPanelIconHeroAgilityValue"

"InfoPanelIconHeroIntellectLabel"

"InfoPanelIconHeroIntellectValue"

The alliance info when having one allied building selected, all of them use CreateContext 7.

"SimpleInfoPanelIconAlly", 7

"InfoPanelIconAllyTitle"

"InfoPanelIconAllyGoldIcon"

"InfoPanelIconAllyGoldValue"

"InfoPanelIconAllyWoodIcon"

"InfoPanelIconAllyWoodValue"

"InfoPanelIconAllyFoodIcon"

"InfoPanelIconAllyFoodValue"

"InfoPanelIconAllyUpkeep"

The Textures are anchored to the previous Textures and the Strings next to the Textures. Except for "InfoPanelIconAllyTitle" & "InfoPanelIconAllyGoldIcon" this are anchored to "SimpleInfoPanelIconAlly".

This frames update as soon the local player selects an allied building. The update includes setting the text for the Value SimpleFrames and reposing ("SimpleInfoPanelIconAlly", 7).

This are other SimpleFrames in that unit info frame.

"SimpleInfoPanelUnitDetail", 0

"SimpleNameValue", 0 The name at the top

Anchor TOP​
"SimpleHeroLevelBar", 0 Exp bar has values from 0 to 1.0

TOP, "SimpleNameValue", BOTTOM​
"SimpleProgressIndicator", 0 Remaing Time bar from 0 to 1.0

TOP, "SimpleNameValue", BOTTOM​
"SimpleBuildTimeIndicator", 0

TOPLEFT, "SimpleInfoPanelUnitDetail", TOPLEFT, 0.061250, -0.038125,​
"SimpleBuildingActionLabel", 0

CENTER, "SimpleInfoPanelUnitDetail", TOPLEFT, 0.11375, -0.029875,​
"SimpleUnitStatsPanel", 0 parent of SimpleClassValue

"SimpleClassValue", 0 The displayed Text below the name (in the bar).

TOP, "SimpleNameValue", BOTTOM​

There are also other Frames shown at the bottom center UI, instead of "SimpleInfoPanelUnitDetail", they and "SimpleInfoPanelUnitDetail" share one Parent.

"SimpleInfoPanelBuildingDetail", 1

This Frame is shown when one unit is selected and it trains something.

"SimpleBuildingNameValue", 1

Anchor TOP​
"SimpleBuildingDescriptionValue", 1

TOP, "SimpleBuildingNameValue", BOTTOM​
"SimpleBuildTimeIndicator", 1

TOPLEFT, "SimpleInfoPanelBuildingDetail", TOPLEFT, 0.061250, -0.038125​
"SimpleBuildingActionLabel", 1

CENTER, "SimpleInfoPanelBuildingDetail", TOPLEFT, 0.11375, -0.029875​
"SimpleBuildQueueBackdropFrame", 1

"SimpleBuildQueueBackdrop", 1

SetPoint BOTTOMLEFT, "SimpleInfoPanelBuildingDetail", BOTTOMLEFT

BOTTOMRIGHT, "SimpleInfoPanelBuildingDetail", BOTTOMRIGHT​

"SimpleInfoPanelCargoDetail", 2

This Frame is shown when one unit is selected and it transports units.

"SimpleHoldNameValue", 2

Anchor TOP​
"SimpleHoldDescriptionValue", 2

TOP, "SimpleHoldNameValue", BOTTOM​

"SimpleInfoPanelItemDetail", 3

This Frame is shown when an item on the ground is selected.

"SimpleItemNameValue", 3

Anchor TOP​
"SimpleItemDescriptionValue", 3

TOP, "SimpleItemNameValue", BOTTOM​

"SimpleInfoPanelDestructableDetail", 4

This Frame is shown when a destructable is selected (like a Gate, cage etc).

"SimpleDestructableNameValue", 4

Anchor TOP​

###
 Access Frames by Child

Warcraft 3 Patch 1.32.6 added 2 new Frame natives.

BlzFrameGetChildrenCount(frame) -> integer

Returns the amount of non String/Texture childFrames.​
BlzFrameGetChild (frame, index) -> frame

Returns the non String/Texture childFrame with that index: should be called with 0 to BlzFrameGetChildrenCount(frame) - 1. Child-Frames take Indexes based on their Level. The child with the lowest Level takes the lowest index.​

This natives include inherited ChildFrames which couldn't be found with BlzGetFrameByName. Now we can actually know why, because the inherited Frames lost their name.

BlzFrameGetChild has some limit (in 1.32.6), it is not able to find String/Texture childs of SimpleFrames. Additional On first sight it looks like the indexes taken by childs are wierd, but it follows rules.

The rules are: The Child with the highest Level takes the highest index. If multiple children have the same Level the last added takes the highest of them.

Haven't tested so many types yet, but now to GLUETEXTBUTTON. The example Button is ScriptDialogButton, using BlzFrameGetChildrenCount onto such returns 6 (4 Backdrop, 1 Text and 1 Highlight). The Backdrops and the Highlight won't have names when testing with ScriptDialogButton, because they were inherited from EscMenuButtonTemplate, in case you do a test and might wonder.

An Lua example code creates a ScriptDialogButton after that looping all found childs for each print the index & name.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 local frame = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 BlzFrameSetText(frame, "text")
 print(BlzFrameGetChildrenCount(frame))
 for int = 0, BlzFrameGetChildrenCount(frame) - 1 do
 print(int, BlzFrameGetName(BlzFrameGetChild(frame, int)))
 end
 end
end

The Example code should print this:

0

1

2

3

4 ScriptDialogButtonText

5

GLUETEXTBUTTON have a fixed order for their Children, regardless how they are placed in fdf. First all set ControlBackdrops use indexes, after them a TEXT child follows, regardless if your Button got one in fdf, after the TEXT all loose children are listed, in the ordered they were added. A loose Frame with a lower line number in fdf is added before one with a higher line number. At the end the highlight Frames are placed.

When one does not set a functional BACKDROP then the total amount of slots taken reduces. But even an empty GLUETEXTBUTTON will have 1 child-Frame the TEXT.

Any Frame you add by code counts as loose Frame and will be placed to the end of the loose FrameList with that also pushing the indexes of the possible Highlight ChildFrames.

the Text child is at index 4 (which is a bit wierd The inherited Button got 5 childs, for some reason the highlight was pushed to index 5). Maybe it has to do with Level have to check. No, I now created "EscMenuButtonTemplate" the one ScriptDialogButton inherits and the game tells this thing would have 6 Childs but in fdf there are only 5, it seems like the Text-Child is on default created for that GLUETEXTBUTTON Type. Frame "GLUETEXTBUTTON" "MyButton" {} has one child according to the native, when moving the child it moves the text out of the button.

The inherited childFrames also forget their names (logical otherwise one could use BlzGetFrameByName onto them).

The childFrames of a GLUETEXTBUTTON are sorted, backdrops, text, highlight.

The highlight frames are pushed to the highest indexes while the text takes the slot after the backdrops.

The Loose childFrames are placed in the order they were found and added after the text, regardless if by code or fdf.

In the image a custom GLUETEXTBUTTON was written in fdf to do the test which you can see at the right part of the image.

After creating 2 Loose childFrames with code using BlzCreateFrameByType, The childs use these indexes:

 Lua:

BlzCreateFrameByType("FRAME", "CodeFrameA", frame, "",0)
BlzCreateFrameByType("FRAME", "CodeFrameB", frame, "",0)

A Lua function to iterate /print childs of a frame and their childs.

 Lua:

function PrintChildFrames(frame, prefix)
 print(prefix, BlzFrameGetName(frame))
 if BlzFrameGetChildrenCount(frame) > 0 then
 for int = 0, BlzFrameGetChildrenCount(frame) - 1 do
 PrintChildFrames(BlzFrameGetChild(frame, int), prefix.."."..int)
 end
 end
end

One could change it to write to a file by replacing print with Preload

The inital call would then be PreloadGenStart PrintChildFrames PreloadGenEnd

WARNING

in 1.32.6 exists a bug. BlzGetOriginFrame does not return a valid frame (GetHandleId 0) when one got the expected originframe over BlzFrameGetChild first.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 local frameA = BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0)
 local frameAA = BlzFrameGetChild(frameA, 3)
 local frameAAA = BlzFrameGetChild(frameAA, 0)
 local frameB = BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT, 0)
 print(GetHandleId(frameAA),GetHandleId(frameAAA), GetHandleId(frameB))
 print(frameAAA == frameB)
 print("done")
 end
end

when one pushes frameB before frameAAA it works as wanted/expected, but right now frameB would be nil/GetHandleId 0.

Results of tests using BlzFrameGetChild.

Names useable in BlzGetFrameByName are written like that "Name". [0 to x] includes 0 and x.

### ConsoleUI

### BottomCenterUI

### GAME_UI

### Game UI Simple

### TOOLTIP

 Code:

Children of "ConsoleUI"/ORIGIN_FRAME_SIMPLE_UI_PARENT
Count: 6 to 7
[0] "ConsoleTopBar", 0
[1] "ConsoleBottomBar", 0
 [0] "ConsoleBottomBarOverlay", 0
 [1] "CommandBarFrame", 0
 [0 to 11] "CommandButton_0",0 to "CommandButton_11",0
 [0] ORIGIN_FRAME_COMMAND_BUTTON
 Created when required
 [0] Cooldown
 [1]
 [2] ChargeBox
 [2] "SimpleInfoPanelUnitDetail", 0 Parent ?
 [3] IdleWorkerFace
 [0] Button
 [0] Charges Box (created with the first idle worker)
 [4] ORIGIN_FRAME_UBERTOOLTIP ?
[2] "ResourceBarFrame",0
 [0] Mouse Listener (Gold)
 [1] Mouse Listener (Lumber)
 [2] Mouse Listener (UpKeep)
 [3] Mouse Listener (Food)
 [???] Fps/Apm/Ping display
 [x] "ResourceBarGoldText",0
 [x] "ResourceBarLumberText",0
 [x] "ResourceBarSupplyText",0
 [x] "ResourceBarUpkeepText",0
[3] "UpperButtonBarFrame",0
 [0] Quest/"UpperButtonBarQuestsButton",0
 [1] Menu/"UpperButtonBarMenuButton",0
 [2] Alliance/"UpperButtonBarAlliesButton",0
 [3] Log/"UpperButtonBarChatButton",0
[4] "MiniMapButtonBar",0
 [0 to 4] Buttons 0(Top), 4(Bottom)
[5] command button mouse deadzone
[6] Hero ButtonBar
 [0 to 6] HeroButtons
 [0] HP-Bar
 [1] MP-Bar
 [2] Unspent-SkillPointsBox (only when this slot ever had unspent SkillPoints)
[7] World Object Hover info (exist after any object was hovered with the mouse)

 Code:

Children of BottomCenterUI
Count: 8
[0] Single Unit "SimpleInfoPanelUnitDetail"
 [0] "SimpleHeroLevelBar",0
 [0] visual bar
 [1] The Exp-Tooltipbox
 [1] "SimpleProgressIndicator",0
 [0] visual Box
 [2] "SimpleBuildTimeIndicator",0
 [0]
 [3] "SimpleUnitStatsPanel",0
 [x] "SimpleClassValue",0
 [4] "SimpleInfoPanelIconDamage",0
 [0] Mouse Listener
 [5] "SimpleInfoPanelIconDamage",1
 [0] Mouse Listener
 [6] "SimpleInfoPanelIconArmor",2
 [0] Mouse Listener
 [7] "SimpleInfoPanelIconRank",3
 [0] Mouse Listener
 [8] "SimpleInfoPanelIconFood",4
 [0] Mouse Listener
 [9 "SimpleInfoPanelIconGold",5
 [0] Mouse Listener
 [10] "SimpleInfoPanelIconHero",6
 [0] "SimpleInfoPanelIconHeroText",6
 [1] Mouse Listener
 [11] "SimpleInfoPanelIconAlly",7
 [0] Gold - Mouse Listener
 [1] Lumber - Mouse Listener
 [2] Food - Mouse Listener
 [3] UpKeep - Mouse Listener
 [12] ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR
 [0 to 7] BuffIcons + mouse Listeners
 [x] ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR_LABEL
 [13]
[1] Trainer "SimpleInfoPanelBuildingDetail",1
 [0] "SimpleBuildTimeIndicator",1
 [0] Bar Border
 [1] Que Button Backgrounds
 [2] Que Button next to the Progressbar
 [3] Que Bottom Button Container
 [0 to 5] Button
 [4]
[2] Transporter "SimpleInfoPanelCargoDetail",2
 [0] CargoContainer
 [0 to 9] Cargo Button Container (includes Box, pos themself the first time a Transporter with content is selected)
 [0] Button
 [1] Life Bar
 [2] Mana Bar
[3] Item on Ground "SimpleInfoPanelItemDetail",3
[4] selectable Gate/tree/etc "SimpleInfoPanelDestructableDetail",4
[5] multiple unit group selection
 [0] ContainerGroupButtons
 [0 to 11] ButtonContainer
 [0] Highlighter Background
 [1] Button
 [2] Life Bar
 [3] Mana Bar
[6] "SimpleInventoryBar",0
 [0 to 5] "InventoryButton_0",0 to "InventoryButton_5",0
 [0]
[7] "SimpleInventoryCover",0 (covers the inventory buttons when no inventory)

Children of ORIGIN_FRAME_GAME_UI

Count: 14 on default. Without Multiboard, Leaderboard, TimerDialog, nor having quest Dialog clicked. With them it has 19.

After anyone clicked the quest dialog 2 further ones are added

Custom added Frames takes slots from [8] pushing others back after Chat/Log when created with prio(0)

 Code:

[0] ORIGIN_FRAME_WORLD_FRAME
 [0]
[1] Probably Parent of World Object Hover info box & HP/Mana Bars
[2] BottomUI Black Background "ConsoleUIBackdrop"
[3] Parent of ORIGIN_FRAME_PORTRAIT
 [0] ORIGIN_FRAME_PORTRAIT
 [1 to 7] ORIGIN_FRAME_HERO_BUTTON_INDICATOR
[4] Parent of InventoryText, Related to the visuals of SimpleFrames, hiding it makes all SimpleFrames not visible but still clickable
 [0] "InventoryText"
[5] Day Time Clock & Minimap are affected by this one, maybe ancestor/Parent
 [0] Affects the visual daytime clock (moving it has wierd effects)
 [0] Day Time Clock Mouse Listener
 [1] "MinimapFrame"
[6] CinematicPanel
 [0] "CinematicPanelControlFrame"
 [0] "HDCinematicBackground"
 [1] "CinematicBottomBorder"
 [2] "CinematicScenePanel"
 [1] "CinematicPortraitBackground"
 [2] "CinematicPortrait"
 [3] "CinematicPortraitCover"
 [4] "HDCinematicPortraitCover"
 [5] "CinematicSpeakerText"
 [6] "CinematicDialogueText"
 [3] "CinematicTopBorder"
[7] "ChatDialog"/"LogDialog"
 [0] "LogBackdrop"
 [1] "LogTitle"
 [2] "LogOkButton"
 [3] "LogAreaBackdrop"
 [0] "LogArea"
 ChatDialog takes LogDialogs Place in multiplayer
 [0] "ChatBackdrop"
 [1] "ChatTitle"
 [2] "ChatPlayerRadioButton"
 [3] "ChatAlliesRadioButton"
 [4] "ChatObserversRadioButton"
 [5] "ChatEveryoneRadioButton"
 [6] "ChatPlayerLabel"
 [7] "ChatAlliesLabel"
 [8] "ChatObserversLabel"
 [9] "ChatEveryoneLabel"
 [10] "ChatPlayerMenu"
 [11] "ChatAcceptButton"
 [12] "ChatHistoryDisplayBackdrop"
 [0] "ChatHistoryDisplay"
 [13] "ChatHistoryLabel"
 [14] "ChatInfoText"
[8] "AllianceDialog"
 [0] "AllianceBackdrop"
 [1] "AllianceTitle"
 [2] "ResourceTradingTitle"
 [3] "PlayersHeader"
 [4] "AllyHeader"
 [5] "VisionHeader"
 [6] "UnitsHeader"
 [7] "GoldHeader"
 [8] "LumberHeader"
 [9] "AllianceAcceptButton"
 [10] "AllianceCancelButton"
 [11] "AlliedVictoryCheckBox"
 [12] "AlliedVictoryLabel"
 [13] "AllianceDialogScrollBar"
 [14 to 37] "AllianceSlot"
 [0] "ColorBackdrop"
 [0] "ColorBorder"
 [1] "PlayerNameLabel"
 [2] "AllyCheckBox"
 [3] "VisionCheckBox"
 [4] "UnitsCheckBox"
 [5] "GoldBackdrop"
 [0] "GoldText"
 [6] "LumberBackdrop"
 [0] "LumberText"
[x] "Leaderboard"
 [0] "LeaderboardBackdrop"
 [1] "LeaderboardTitle"
 [2] "LeaderboardListContainer"
[x] "Multiboard"
 [0] "MultiboardMinimizeButton"
 [1] "MultiboardTitleBackdrop"
 [2] "MultiboardTitle"
 [3] "MultiboardBackdrop"
 [4] "MultiboardListContainer"
 [0] No idea
 [0 to rowCount - 1] row
 [0 to colCount - 1] cell
 [0] IconContainer
 [1] TextContainer
[9] No idea
 [0]
[10] Inside Menu (F10)
 [0] "EscMenuBackdrop"
 [0] "EscMenuMainPanel"
 [0] "MainPanel"
 [0] "WouldTheRealOptionsTitleTextPleaseStandUp"
 [1] "InsideMainPanel"
 [0] "PauseButton"
 [1] "SaveGameButton"
 [2] "LoadGameButton"
 [3] "OptionsButton"
 [4] "HelpButton"
 [5] "TipsButton"
 [6] "EndGameButton"
 [7] "ReturnButton"
 [1] "EndGamePanel"
 [0] "InsideEndGamePanel"
 [0] "EndGameTitleText"
 [1] "RestartButton"
 [2] "QuitButton"
 [3] "ExitButton"
 [4] "PreviousButton"
 [2] "ConfirmQuitPanel"
 [0] "InsideConfirmQuitPanel"
 [0] "ConfirmQuitTitleText"
 [1] "ConfirmQuitMessageText"
 [2] "ConfirmQuitQuitButton"
 [3] "ConfirmQuitCancelButton"
 [3] "HelpPanel"
 [0] "InsideHelpPanel"
 [0] "HelpTitleText"
 [1] "HelpTextArea"
 [2] "HelpOKButton"
 [4] "TipsPanel"
 [0] "InsideTipsPanel"
 [0] "TipsTitleText"
 [1] "TipsTextArea"
 [2] "TipsBackButton"
 [3] "TipsNextButton"
 [4] "TipsOKButton"
 [1] "EscMenuOptionsPanel"
 [0] "OptionsPanel"
 [0] "OptionsTitleText"
 [1] "GameplayButton"
 [2] "VideoButton"
 [3] "SoundButton"
 [4] "NetworkButton"
 [5] "OptionsPreviousButton"
 [1] "GameplayPanel"
 [0] "GameplayTitleText"
 [1] "GameSpeedLabel"
 [2] "GameSpeedSlider"
 [3] "GameSpeedValue"
 [4] "MouseScrollLabel"
 [5] "MouseScrollSlider"
 [6] "MouseScrollSlowLabel"
 [7] "MouseScrollFastLabel"
 [8] "MouseScrollDisable"
 [9] "MouseScrollDisableLabel"
 [10] "KeyScrollLabel"
 [11] "KeyScrollSlider"
 [12] "KeyScrollSlowLabel"
 [13] "KeyScrollFastLabel"
 [14] "TooltipsCheckBox"
 [15] "TooltipsLabel"
 [16] "SubgroupCheckBox"
 [17] "SubgroupLabel"
 [18] "FormationToggleCheckBox"
 [19] "FormationToggleLabel"
 [20] "HealthBarsCheckBox"
 [21] "HealthBarsLabel"
 [22] "CustomKeysLabel"
 [23] "CustomKeysValue"
 [24] "DifficultyLabel"
 [25] "DifficultyValue"
 [2] "VideoPanel"
 [0] "VideoTitleText"
 [1] "GammaLabel"
 [2] "GammaSlider"
 [3] "GammaDarkLabel"
 [4] "GammaBrightLabel"
 [5] "WindowModeLabel"
 [6] "ResolutionLabel"
 [7] "VSyncLabel"
 [8] "ParticlesLabel"
 [9] "LightsLabel"
 [10] "ShadowsLabel"
 [11] "OcclusionLabel"
 [12] "ModelDetailLabel"
 [13] "ModelDetailValue"
 [14] "AnimQualityLabel"
 [15] "AnimQualityValue"
 [16] "TextureQualityLabel"
 [17] "TextureQualityValue"
 [18] "EscOptionsWindowModeMenu"
 [19] "EscOptionsResolutionMenu"
 [20] "VSyncCheckBox"
 [21] "EscOptionsParticlesMenu"
 [22] "EscOptionsLightsMenu"
 [23] "EscOptionsShadowsMenu"
 [24] "EscOptionsOcclusionMenu"
 [3] "SoundPanel"
 [0] "SoundTitleText"
 [1] "SoundCheckBox"
 [2] "SoundVolumeLabel"
 [3] "SoundVolumeSlider"
 [4] "SoundVolumeLowLabel"
 [5] "SoundVolumeHighLabel"
 [6] "MusicCheckBox"
 [7] "MusicVolumeLabel"
 [8] "MusicVolumeSlider"
 [9] "MusicVolumeLowLabel"
 [10] "MusicVolumeHighLabel"
 [11] "AmbientCheckBox"
 [12] "AmbientLabel"
 [13] "MovementCheckBox"
 [14] "MovementLabel"
 [15] "UnitCheckBox"
 [16] "UnitLabel"
 [17] "SubtitlesCheckBox"
 [18] "SubtitlesLabel"
 [19] "ProviderLabel"
 [20] "ProviderValue"
 [21] "EnviroCheckBox"
 [22] "EnviroLabel"
 [23] "PositionalCheckBox"
 [24] "PositionalLabel"
 [4] "NetworkPanel"
 [0] "NetworkTitleText"
 [1] "NetworkLabel"
 [2] "LatencyInfo1"
 [3] "LatencyInfo2"
 [4] "LowLatencyRadio"
 [5] "HighLatencyRadio"
 [6] "ExtraHighLatencyRadio"
 [7] "LowLatencyLabel"
 [8] "HighLatencyLabel"
 [9] "ExtraHighLatencyLabel"
 [5] "BottomButtonPanel"
 [0] "OptionsOKButton"
 [1] "OptionsCancelButton"
 [2] "EscMenuSaveGamePanel"
 [0] "EscMenuSaveLoadContainer"
 [0] "SaveAndLoad"
 [0] "FileListFrame"
 [0] "DecoratedMapListBox"
 [1] "LoadOnly"
 [0] "LoadGameTitleText"
 [1] "LoadGameLoadButton"
 [2] "LoadGameCancelButton"
 [2] "SaveOnly"
 [0] "SaveGameTitleText"
 [1] "SaveGameSaveButton"
 [2] "SaveGameDeleteButton"
 [3] "SaveGameCancelButton"
 [4] "SaveGameFileEditBox"
 [1] "EscMenuOverwriteContainer"
 [0] "OverwriteOnly"
 [0] "OverwriteTitleText"
 [1] "OverwriteMessageText"
 [2] "OverwriteOverwriteButton"
 [3] "OverwriteCancelButton"
 [2] "EscMenuDeleteContainer"
 [0] "DeleteOnly"
 [0] "DeleteTitleText"
 [1] "DeleteMessageText"
 [2] "DeleteDeleteButton"
 [3] "DeleteCancelButton"
[11] Chat Input box
 [0]
 [1]
 [0]
 [0]
 [1]
 [2]
 [3]
 [4]
 [1]
 [2]
 [3]
[12] Option Menu
 [0] "EscOptionsWindowModePopupMenuMenu"
 [1] "EscOptionsResolutionPopupMenuMenu"
 [2] "EscOptionsParticlesPopupMenuMenu"
 [3] "EscOptionsLightsPopupMenuMenu"
 [4] "EscOptionsShadowsPopupMenuMenu"
 [5] "EscOptionsOcclusionPopupMenuMenu"
[13] Mouse Cursor (Parent)

 Code:

[0] ORIGIN_FRAME_WORLD_FRAME
[1] Probably Parent of World Object Hover info box & HP/Mana Bars
[2] BottomUI Black Background "ConsoleUIBackdrop"
[3] Parent of ORIGIN_FRAME_PORTRAIT
[4] InventoryText, Related to the visuals of SimpleFrames, hiding it makes all SimpleFrames not visible but still clickable
[5] Day Time Clock & Minimap are affected by this one, maybe ancestor/Parent
[6] CinematicPanel
[7] Chat/LogDialog
[8] Multiboard
[9] Leaderboard
[10] TimerDialog
[11] No Idea
[12] QuestDialog
[13] AllianceDialog
[14] No Idea
[15] EscMenuBackdrop
[16] Chat Input
[17] EscOptions
[18] Mouse Cursor (Parent)

 Code:

Children of ORIGIN_FRAME_TOOLTIP
Count: 1

Children of ORIGIN_FRAME_UBERTOOLTIP
Count: 1

##
 Hovering Command/ItemButton

Sometimes one wants to know when the local player points with the mouse onto a commandbutton. Because there is no GetMouseon Screen X/Y, there are 2 likely ways one could come up with. One is to create custom textureless BUTTON overlapping the wanted Buttons and the other is creating tooltips for the Buttons (which is only done when you use SIMPLEFRAMEs, hence one might first go for the overlapping BUTTON version).

With BlzFrameSetTooltip it is possible to know that the local player hovers an item or command button. One creates an SIMPLEFRAME with BlzCreateFrameByType and define it as tooltip then one checks in a timer running every 0.xx seconds for the visibility of the tooltip. This does not seem to have any sideeffect, does not even stop the normal tooltip. Because the set Tooltip works that well I disrecomment the overlapping BUTTONs. The BUTTONs take much more setup and the only gain is the event is sync over network without additional work, although this is probably a disadvantage because tooltips are naturally local only and other players don't care about the text shown in your Tooltipbox.

 Lua:

-- creates tooltip frames (for command buttons) which are checked 32 times a second when a different tooltip is visible function HoversCommandButton is called with the new index, beaware this is async
do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 local commandButtonTooltip = {}
 local frame
 local button

 -- saves the last selected Button, async
 CurrentSelectedButtonIndex = nil
 --create one tooltip frame for each command button
 for int = 0, 11 do
 button = BlzGetOriginFrame(ORIGIN_FRAME_COMMAND_BUTTON, int)
 frame = BlzCreateFrameByType("SIMPLEFRAME", "", button, "", 0)
 BlzFrameSetTooltip(button, frame)
 BlzFrameSetVisible(frame, false)
 commandButtonTooltip[int] = frame
 end

 TimerStart(CreateTimer(), 1.0/32, true, function()
 local selectedAnything = false
 -- loop all tooltips and check for the visible one
 for int = 0, 11 do
 if BlzFrameIsVisible(commandButtonTooltip[int]) then
 selectedAnything = true
 -- the new selected is not the same as the current one?
 if CurrentSelectedButtonIndex ~= int then
 HoversCommandButton(int)
 end
 CurrentSelectedButtonIndex = int
 end
 end
 -- now selects nothing?
 if not selectedAnything and CurrentSelectedButtonIndex then
 HoversCommandButton(nil)
 CurrentSelectedButtonIndex = nil
 end
 end)
 print("done")
 end
end

function HoversCommandButton(commandButtonindex)
 if not commandButtonindex then
 print("Now points at nothing")
 else
 print("Now points at Button:", commandButtonindex)
 end
end

###
 WhatTypeOfFrame

When one accesses frames, it can be of use to know to what group it belongs, because some frame natives are not supported by all frame-groups and might crash the game.

One way how one would know if any frame belongs to Frame, SimpleFrame or String/Texture is by having 2 Test Frames which one tries to make children of the wanted frame. Then one compares current parent with the frame, if they match it belongs to that group. Below is such an working Lua code.

 Lua:

function WhatTypeOfFrame(frame)
 -- tells you if the Frame is a SimpleFrame, Frame or String/Texture
 -- nil => no frame
 -- 0 => Frame group
 -- 1 => SimpleFrame group
 -- 2 => String/Texture

 -- create TestFrames?
 if GetHandleId(BlzGetFrameByName("WhatTypeOfFrameTestFrame", 0)) == 0 then
 BlzCreateFrameByType("FRAME", "WhatTypeOfFrameTestFrame", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
 BlzCreateFrameByType("SIMPLEFRAME", "WhatTypeOfFrameTestFrameSimple", BlzGetFrameByName("ConsoleUI", 0), "", 0)
 end

 -- Is this a frameObject at all?
 if not string.find(tostring(frame), "framehandle:") then return nil end

 if GetHandleId(frame) == 0 then
 return nil
 end

 -- when the TestFrame can become the frame's child it is a Frame
 BlzFrameSetParent(BlzGetFrameByName("WhatTypeOfFrameTestFrame", 0), frame)
 if GetHandleId(BlzFrameGetParent(BlzGetFrameByName("WhatTypeOfFrameTestFrame", 0))) == GetHandleId(frame) then
 BlzFrameSetParent(BlzGetFrameByName("WhatTypeOfFrameTestFrame", 0), BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0))
 return 0
 end

 -- when the TestFrameSimple can become the frame's child it is a SimpleFrame
 BlzFrameSetParent(BlzGetFrameByName("WhatTypeOfFrameTestFrameSimple", 0), frame)
 if GetHandleId(BlzFrameGetParent(BlzGetFrameByName("WhatTypeOfFrameTestFrameSimple", 0))) == GetHandleId(frame) then
 BlzFrameSetParent(BlzGetFrameByName("WhatTypeOfFrameTestFrameSimple", 0), BlzGetFrameByName("ConsoleUI", 0))
 return 1
 end

 -- when this point is reached: Then frame is a valid frameObject but can't have children. Therefore it is a String/Texture.
 return 2
end

###
 Origin-Frames

For some Frames Blizzard provided another way to get a frame reference to them. They are called OriginFrame which have an own getter native and a bunch of constants.

 JASS:

native BlzGetOriginFrame takes originframetype frameType, integer index returns framehandle
constant originframetype ORIGIN_FRAME_GAME_UI = ConvertOriginFrameType(0)
constant originframetype ORIGIN_FRAME_COMMAND_BUTTON = ConvertOriginFrameType(1)
constant originframetype ORIGIN_FRAME_HERO_BAR = ConvertOriginFrameType(2)
constant originframetype ORIGIN_FRAME_HERO_BUTTON = ConvertOriginFrameType(3)
constant originframetype ORIGIN_FRAME_HERO_HP_BAR = ConvertOriginFrameType(4)
constant originframetype ORIGIN_FRAME_HERO_MANA_BAR = ConvertOriginFrameType(5)
constant originframetype ORIGIN_FRAME_HERO_BUTTON_INDICATOR = ConvertOriginFrameType(6)
constant originframetype ORIGIN_FRAME_ITEM_BUTTON = ConvertOriginFrameType(7)
constant originframetype ORIGIN_FRAME_MINIMAP = ConvertOriginFrameType(8)
constant originframetype ORIGIN_FRAME_MINIMAP_BUTTON = ConvertOriginFrameType(9)
constant originframetype ORIGIN_FRAME_SYSTEM_BUTTON = ConvertOriginFrameType(10)
constant originframetype ORIGIN_FRAME_TOOLTIP = ConvertOriginFrameType(11)
constant originframetype ORIGIN_FRAME_UBERTOOLTIP = ConvertOriginFrameType(12)
constant originframetype ORIGIN_FRAME_CHAT_MSG = ConvertOriginFrameType(13)
constant originframetype ORIGIN_FRAME_UNIT_MSG = ConvertOriginFrameType(14)
constant originframetype ORIGIN_FRAME_TOP_MSG = ConvertOriginFrameType(15)
constant originframetype ORIGIN_FRAME_PORTRAIT = ConvertOriginFrameType(16)
constant originframetype ORIGIN_FRAME_WORLD_FRAME = ConvertOriginFrameType(17)
constant originframetype ORIGIN_FRAME_SIMPLE_UI_PARENT = ConvertOriginFrameType(18)
constant originframetype ORIGIN_FRAME_PORTRAIT_HP_TEXT = ConvertOriginFrameType(19)
constant originframetype ORIGIN_FRAME_PORTRAIT_MANA_TEXT = ConvertOriginFrameType(20)
constant originframetype ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR = ConvertOriginFrameType(21)
constant originframetype ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR_LABEL = ConvertOriginFrameType(22)

ORIGIN_FRAME_SIMPLE_UI_PARENT, ORIGIN_FRAME_PORTRAIT_HP_TEXT, ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR, ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR_LABEL were not included in the initial releas in 1.31.1 They were added in 1.32.2

When no indexes are is mentioned use index 0. But it seems to give you the last right index when a to high index is given.

ConstantIndexesinfo2img

ORIGIN_FRAME_GAME_UIwithout it nothing will be displayed.

ORIGIN_FRAME_WORLD_FRAMEthe visible playground, units, items, effects, fog... every object participating in the game is displayed on it.[attachment 324631](https://www.hiveworkshop.com/attachments/world-jpg.324631/)

ORIGIN_FRAME_HERO_BARparent of all HERO_BUTTONS, HeroButtons share the same visibility.[attachment 324622](https://www.hiveworkshop.com/attachments/herobar-jpg.324622/)

ORIGIN_FRAME_HERO_BUTTON 0 to 6the clickable buttons of own/allied heroes on the left of the screen[attachment 324623](https://www.hiveworkshop.com/attachments/herobuttons-jpg.324623/)

ORIGIN_FRAME_HERO_HP_BAR0 to 6connected to HeroButtons[attachment 324621](https://www.hiveworkshop.com/attachments/hero_hp-mana_bar-jpg.324621/)

ORIGIN_FRAME_HERO_MANA_BAR0 to 6connected to HeroButtons

ORIGIN_FRAME_HERO_BUTTON_INDICATOR0 to 6The glowing when a hero has skillpoints; connected to HeroButtons. They reappear when a hero gains a new skillpoint even when all originframes are hidden.[attachment 324619](https://www.hiveworkshop.com/attachments/hero_button_indicator-jpg.324619/)

ORIGIN_FRAME_ITEM_BUTTON0 to 5Items in the inventory. Reappear/updates every selection, When its parent is visible. In 1.32.x Moving this glitches instead one has to move BlzGetFrameByName("InventoryButton_0", 0)... to ("InventoryButton_5", 0)[attachment 324625](https://www.hiveworkshop.com/attachments/itembuttons-jpg.324625/)

ORIGIN_FRAME_COMMAND_BUTTON0 to 11the buttons to order units around, like ITEM_BUTTON, Reappear/update every selection. In 1.32.x Moving this glitches instead one has to move BlzGetFrameByName("CommandButton_0", 0)... to ("CommandButton_11", 0)[attachment 324618](https://www.hiveworkshop.com/attachments/command_button-jpg.324618/)

ORIGIN_FRAME_SYSTEM_BUTTON0 to 3Menu, allies, Log/chat, Quest. To stop the Hotkey one needs to disable the frame. When one of the menues is closed the buttons are renabled automatically.[attachment 324633](https://www.hiveworkshop.com/attachments/system-buttons-jpg.324633/)

ORIGIN_FRAME_PORTRAITFace of the main selected Unit, uses a different coordination system 0,0 being the absolute bottomLeft which makes it difficult to be used together with other frames (not 4:3, like for the others)[attachment 324628](https://www.hiveworkshop.com/attachments/portrait-jpg.324628/)

ORIGIN_FRAME_MINIMAP

ORIGIN_FRAME_MINIMAP_BUTTON0 to 40 is the Button at top to 4 Button at the bottom[attachment 324626](https://www.hiveworkshop.com/attachments/minimap-buttons-jpg.324626/)

ORIGIN_FRAME_TOOLTIP

ORIGIN_FRAME_UBERTOOLTIPHandles the Tooltip window

ORIGIN_FRAME_CHAT_MSG

ORIGIN_FRAME_UNIT_MSGprint frame for game display message / DisplayTextToPlayer[attachment 324630](https://www.hiveworkshop.com/attachments/unit_msg-jpg.324630/)

ORIGIN_FRAME_TOP_MSGFrame: UpKeep Change Warning Message, below the dayTime Clock[attachment 324629](https://www.hiveworkshop.com/attachments/top-msg-jpg.324629/)

ORIGIN_FRAME_PORTRAIT_HP_TEXTV1.32+ Does not exist until any selection was done and a short time passed. Force a Selection and wait a short time. Does only pos itself correctly with FRAMEPOINT_CENTER. Probably a String-Frame. (BlzFrameGetText, BlzFrameSetFont are working onto it and BlzFrameSetAlpha/BlzFrameSetLevel crash the game like String-Frames tend to do)[attachment 352087](https://www.hiveworkshop.com/attachments/bloodmagefacehptext-jpg.352087/)

ORIGIN_FRAME_PORTRAIT_MANA_TEXTLike ORIGIN_FRAME_PORTRAIT_HP_TEXT[attachment 352088](https://www.hiveworkshop.com/attachments/bloodmagefacemptext-jpg.352088/)

ORIGIN_FRAME_UNIT_PANEL_BUFF_BARV1.32+ The Frame has a size to fit all 8 possible Buffs[attachment 352090](https://www.hiveworkshop.com/attachments/buffbar-jpg.352090/)

ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR_LABELV1.32+ Attached to ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR on default. Probably a String-Frame.[attachment 352089](https://www.hiveworkshop.com/attachments/bufflabel-jpg.352089/)

ORIGIN_FRAME_SIMPLE_UI_PARENTV1.32+

###
 WARNING

BlzGetFrameByName/BlzFrameGetParent and BlzGetOriginFrame will occupy one handleId each time they return a frame that was not yet in the game (for your code). Means using that the first time in a GetLocalPlayer block with a name - createContext combo desyncs the game in multiplayer.

###
 Hide Default UI

In case one does not need any of this builtin Frames one can use BlzHideOriginFrames(true) which hides everything except for Command Buttons, Chat, Messages, TimerDialogs, Multiboards and LeaderBoards. This hidden Frames can be made visible again as long you find a way to access them. They are not destroyed.

From 1.32 one has now a Fullscreen Game with an black box at the bottom. Which can be hidden with BlzFrameSetVisible(BlzGetFrameByName("ConsoleUIBackdrop",0), false)

There are other options, if one wants the Frame logical visible but not seeable: For example one could reduced the ySize.

BlzFrameSetSize(BlzGetFrameByName("ConsoleUIBackdrop",0), 0, 0.0001)

HideUI
Events
Time - Elapsed game time is 1.00 seconds

Conditions

Actions
Frame - Hide Origin Frames: True

Frame - show - (Get Frame By Name:ConsoleUIBackdrop Context:0) -_ False

Frame - Set Size of (Get Frame By Name:ConsoleUIBackdrop Context:0) to 0.00/(0.10 / 10000.00)

Frame - Set all Points for:WorldFrame to GameUI

In version 1.31.1 the Playerable ground did not extend to the Bottom. Can be fixed with a line of code, but there is no Black Box Frame. It is a replacement in 1.32 for the extended World Frame. The code that makes the world Frame Fullscreen BlzFrameSetAllPoints(BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0))

###
 Alternative Hide only Bottom UI

There are also some other options to "hide" parts of the UI. One is to move the bottom Of "ConsoleUI" a little bit lower that has to be done at map Init, doing it later leads to crashes.

BlzFrameSetAbsPoint(BlzGetFrameByName("ConsoleUI", 0), FRAMEPOINT_BOTTOM, 0.4, -0.18)

When one plans to move some originframes, it is also advised to use this line of code BlzEnableUIAutoPosition(false). That will stop the reposing of some of this builtin frames when the user changes resolution or windowMode.

###
 Alternative Hide UI by Game Interface

While this frame api hiding is nice it has downsides. If one just wants to get rid of the visual background changing the used textures for the UI has far less sideeffects.

By setting the used Textures in game interface to UI\Widgets\EscMenu\Human\blank-background.blp it is gone.

Or in the map's file war3mapSkin.txt (basicly the same but without using World Editor). To edit this file, your map needs to include it already, have some change in world editor game interface. Then you have 2 options: save map as folder or use a mpq editor In both cases you should close your map in world editor and warcraft 3 and make a backup copy of your map before doing it.

Mpq Editor take the file out of the map, edit & add the lines below in the section [CustomSkin], then save the war3mapSkin-file and put it back into the map with mpq edotor.

With Map as folder you can directly open & edit war3mapSkin.txt, after your edited it, close all programs targeting the file and open the folder in world editor and save it as map file.

[CustomSkin]

ConsoleInventoryCoverTexture=UI\Widgets\EscMenu\Human\blank-background.blp

ConsoleTexture01=UI\Widgets\EscMenu\Human\blank-background.blp

ConsoleTexture02=UI\Widgets\EscMenu\Human\blank-background.blp

ConsoleTexture03=UI\Widgets\EscMenu\Human\blank-background.blp

ConsoleTexture04=UI\Widgets\EscMenu\Human\blank-background.blp

ConsoleTexture05=UI\Widgets\EscMenu\Human\blank-background.blp

ConsoleTexture06=UI\Widgets\EscMenu\Human\blank-background.blp

###
 Move Default UI

As moving some Frames can be a bit wierd this section tells how to move the frames of the default UI.

When it mentions a frame is anchored with something than this means you can move the frame by just placing this point somewhere else. Or you use BlzFrameClearAllPoints first than place it to the wanted position.

ORIGIN_FRAME_WORLD_FRAME

one can set points of the World to make the playable part smaller.

sadly the left & right limitation is not working correctly and one can still order units outside of the world

but displaying gets limited.

 Lua:

local frame = BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0)
BlzFrameClearAllPoints(frame)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.1, 0.55)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_BOTTOMRIGHT, 0.7, 0.15)

ORIGIN_FRAME_HERO_BAR

Reposed by the game while BlzHideOriginFrames(false)

Anchored with TopLeft

touching it also reposes Hero Buttons

 Lua:

BlzHideOriginFrames(true)
local frame = BlzGetOriginFrame(ORIGIN_FRAME_HERO_BAR, 0)
BlzFrameSetVisible(frame, true)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.1, 0.55)

ORIGIN_FRAME_HERO_BUTTON

To move it you need either BlzHideOriginFrames(true) or BlzEnableUIAutoPosition(false)

Anchored with TopLeft

 Lua:

BlzEnableUIAutoPosition(false)
local frame = BlzGetOriginFrame(ORIGIN_FRAME_HERO_BUTTON, 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.1, 0.55)
-- Move 2.Button
local frame = BlzGetOriginFrame(ORIGIN_FRAME_HERO_BUTTON, 1)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.3, 0.25)

ORIGIN_FRAME_HERO_HP_BAR

Anchored with 2 Points I think.

 Lua:

local frame = BlzGetOriginFrame(ORIGIN_FRAME_HERO_HP_BAR, 0)
BlzFrameClearAllPoints(frame)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.3, 0.25)

ORIGIN_FRAME_HERO_MANA_BAR

Anchored to ORIGIN_FRAME_HERO_HP_BAR with TOP

 Lua:

local frame = BlzGetOriginFrame(ORIGIN_FRAME_HERO_MANA_BAR, 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOP, 0.3, 0.25)

ORIGIN_FRAME_HERO_BUTTON_INDICATOR

Anchored to ORIGIN_FRAME_HERO_BUTTON with Multiple Points.

 Lua:

local frame = BlzGetOriginFrame(ORIGIN_FRAME_HERO_BUTTON_INDICATOR, 0)
BlzFrameClearAllPoints(frame)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_BOTTOMRIGHT, 0.3, 0.25)

ORIGIN_FRAME_ITEM_BUTTON

Warcraft 3 V1.32+ you need to use the name Version to move it

Anchored with TopLeft

 Lua:

local frame = BlzGetOriginFrame(ORIGIN_FRAME_ITEM_BUTTON, 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.4, 0.3)
-- Move 2.Button
local frame = BlzGetOriginFrame(ORIGIN_FRAME_ITEM_BUTTON, 1)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.3, 0.2)

-- Warcraft 3 V1.32+
local frame = BlzGetFrameByName("InventoryButton_0", 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.4, 0.3)
-- Move 2.Button
local frame = BlzGetFrameByName("InventoryButton_1", 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.3, 0.2)

ORIGIN_FRAME_COMMAND_BUTTON

Warcraft 3 V1.32+ you need to use the name Version to move it

Anchored with TopLeft

 Lua:

local frame = BlzGetOriginFrame(ORIGIN_FRAME_COMMAND_BUTTON, 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.4, 0.3)
-- Move 2.Button
local frame = BlzGetOriginFrame(ORIGIN_FRAME_COMMAND_BUTTON, 1)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.3, 0.2)

-- Warcraft 3 V1.32+
local frame = BlzGetFrameByName("CommandButton_0", 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.4, 0.3)
-- Move 2.Button
local frame = BlzGetFrameByName("CommandButton_1", 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.3, 0.2)

ORIGIN_FRAME_SYSTEM_BUTTON

They are equal with their Name version.

"UpperButtonBarQuestsButton" anchored with TopLeft.

"UpperButtonBarMenuButton" anchored with Left.

"UpperButtonBarAlliesButton" anchored with Left.

"UpperButtonBarChatButton" anchored with Left.

 Lua:

BlzFrameSetAbsPoint( BlzGetFrameByName("UpperButtonBarQuestsButton", 0), FRAMEPOINT_TOPLEFT, 0.4, 0.3)
BlzFrameSetAbsPoint(BlzGetFrameByName("UpperButtonBarMenuButton", 0), FRAMEPOINT_LEFT, 0.4, 0.35)
BlzFrameSetAbsPoint(BlzGetFrameByName("UpperButtonBarAlliesButton", 0), FRAMEPOINT_LEFT, 0.4, 0.38)
BlzFrameSetAbsPoint(BlzGetFrameByName("UpperButtonBarChatButton", 0), FRAMEPOINT_LEFT, 0.4, 0.4)

ORIGIN_FRAME_PORTRAIT

Size changes with resolution

Anchored with Bottomleft

To move it you need either BlzHideOriginFrames(true) or BlzEnableUIAutoPosition(false)

 Lua:

BlzEnableUIAutoPosition(false)
local frame = BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT, 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_BOTTOMLEFT, 0.4, 0.3)

ORIGIN_FRAME_MINIMAP

Place either Topright & Bottomleft or TopLeft & Bottomleft to move/resize it.

In V1.31.1 one should BlzFrameClearAllPoints(BlzGetOriginFrame(ORIGIN_FRAME_MINIMAP, 0)) at map init. Then the first postions placed is treated correctly as map center. This position placing can happen at a Later time. A second position placing afterwards will bug the minimap (The mouse click will be treated as relative to the first position which is most likely the edge of the minimap).

 Lua:

local frame = BlzGetOriginFrame(ORIGIN_FRAME_MINIMAP, 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_BOTTOMLEFT, 0.4, 0.3)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPRIGHT, 0.6, 0.5)

 JASS:

// Move out of 4:3 V1.32+
function MoveMinimapOutOf43Screen takes nothing returns nothing
 local framehandle parent = BlzGetFrameByName("ConsoleUIBackdrop", 0)
 local framehandle frame = BlzGetFrameByName("MiniMapFrame", 0)
 call BlzFrameSetParent(frame, parent)
 call BlzFrameSetAbsPoint(frame, FRAMEPOINT_BOTTOMLEFT, 0.8, 0.3)
 call BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPRIGHT, 0.95, 0.45)
endfunction

// Move out of 4:3 V1.31.1
function MoveMinimapOutOf43Screen takes nothing returns nothing
 local leaderboard lb = CreateLeaderboardBJ(bj_FORCE_ALL_PLAYERS, "title")
 local framehandle parent = BlzGetFrameByName("Leaderboard", 0)
 local framehandle frame
 call BlzFrameSetSize(parent, 0, 0)
 call BlzFrameSetVisible(BlzGetFrameByName("LeaderboardBackdrop", 0), false)
 call BlzFrameSetVisible(BlzGetFrameByName("LeaderboardTitle", 0), false)
 set frame = BlzGetOriginFrame(ORIGIN_FRAME_MINIMAP, 0)
 call BlzFrameSetParent(frame, parent)
 call BlzFrameSetAbsPoint(frame, FRAMEPOINT_BOTTOMLEFT, 0.8, 0.3)
 call BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPRIGHT, 0.95, 0.45)
endfunction

Credits: sotzaii_shuen

ORIGIN_FRAME_MINIMAP_BUTTON

In V1.32+; also can use "MinimapSignalButton" "MiniMapTerrainButton" "MiniMapAllyButton" "MiniMapCreepButton" "FormationButton" instead.

Anchored with Topleft.

 Lua:

local frame = BlzGetOriginFrame(ORIGIN_FRAME_MINIMAP_BUTTON, 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.4, 0.5)
-- Move 2.Button
local frame = BlzGetOriginFrame(ORIGIN_FRAME_MINIMAP_BUTTON, 1)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.4, 0.47)

ORIGIN_FRAME_UBERTOOLTIP

TooltipBox for most default UI.

Anchored with BottomRight.

I would advise to move it with an Bottom Point as it expands upwards.

 Lua:

local frame = BlzGetOriginFrame(ORIGIN_FRAME_UBERTOOLTIP, 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_BOTTOMRIGHT, 0.4, 0.2)

-- if one does not want to place FRAMEPOINT_BOTTOMRIGHT
local frame = BlzGetOriginFrame(ORIGIN_FRAME_UBERTOOLTIP, 0)
BlzFrameClearAllPoints(frame)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.4, 0.3)

ORIGIN_FRAME_CHAT_MSG

Place either Topright & Bottomleft or TopLeft & Bottomleft to move it.

Can crash the game with bad size/coords.

 Lua:

local frame = BlzGetOriginFrame(ORIGIN_FRAME_CHAT_MSG, 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_BOTTOMLEFT, 0.4, 0.3)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPRIGHT, 0.6, 0.5)

ORIGIN_FRAME_UNIT_MSG

 Lua:

BlzFrameSetAbsPoint(BlzGetOriginFrame(ORIGIN_FRAME_UNIT_MSG, 0), FRAMEPOINT_BOTTOMLEFT, 0.4, 0.3)

Or Place either Topright & Bottomleft or TopLeft & Bottomleft to move it.

Can crash the game with bad size/coords.

The Y-Size should not be bigger than 0.25

 Lua:

local frame = BlzGetOriginFrame(ORIGIN_FRAME_UNIT_MSG, 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_BOTTOMLEFT, 0.0, 0.2)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPRIGHT, 0.5, 0.45)

ORIGIN_FRAME_TOP_MSG

Can crash the game with bad size/coords. Hence I suggest to use BlzFrameClearAllPoints and pos only one point.

 Lua:

local frame = BlzGetOriginFrame(ORIGIN_FRAME_TOP_MSG, 0)
BlzFrameClearAllPoints(frame)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.4, 0.5)

ORIGIN_FRAME_PORTRAIT_HP_TEXT

ORIGIN_FRAME_PORTRAIT_MANA_TEXT

Warcraft 3 V1.32+ only

Don't exist until any selection was done

Moving them is a hussle, it only works fine when moving with Center.

Might be easier to just copy the Displayed Text with BlzFrameGetText(BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT_MANA_TEXT, 0)) as they update even during BlzHideOriginFrames(true).

 Lua:

local frame = BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT_HP_TEXT, 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.4, 0.3)

local frame = BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT_MANA_TEXT, 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.4, 0.28)

ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR

Warcraft 3 V1.32+ only

Anchored to "SimpleInfoPanelUnitDetail"'s BottomRight with BottomRight.

Means to move it in V1.31.1 move "SimpleInfoPanelUnitDetail"'s BottomRight.

 Lua:

local frame = BlzGetOriginFrame(ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR, 0)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_BOTTOMRIGHT, 0.4, 0.3)

ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR_LABEL

Warcraft 3 V1.32+ only

Anchored to ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR with Right

It can be hidden by setting the Frame's text to "" either by a native or in game interface COLON_STATUS.

 Lua:

local frame = BlzGetOriginFrame(ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR_LABEL, 0)
BlzFrameClearAllPoints(frame)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.1, 0.5)

APM/PING/FPS:

No access

Anchored to Topright of "ResourceBarFrame"'s Topright Offset ca -0.44/-0.03

 Lua:

BlzFrameSetAbsPoint(BlzGetFrameByName("ResourceBarFrame", 0), FRAMEPOINT_TOPRIGHT, 0.4 + 0.44, 0.3 + 0.04)

"ResourceBarFrame"

The IconFrames don't have names hence can't be accessed by code.

This IconFrames are anchored with TopLeft to "ResourceBarFrame"'s TopLeft.

The Texts can by Name and createContext 0. They are anchored with TopRight to "ResourceBarFrame"'s TopRight.

A Resource's TooltipListener follows the Text-Frame and can be hidden using BlzFrameSetVisible onto the childs of "ResourceBarFrame", Checkout Access Frames by Child.

"ResourceBarGoldText"

"ResourceBarLumberText"

"ResourceBarSupplyText"

"ResourceBarUpkeepText"

 Lua:

BlzFrameSetAbsPoint(BlzGetFrameByName("ResourceBarGoldText", 0), FRAMEPOINT_TOPRIGHT, 0.4, 0.3)
BlzFrameSetAbsPoint(BlzGetFrameByName("ResourceBarLumberText", 0), FRAMEPOINT_TOPRIGHT, 0.4, 0.3)
BlzFrameSetAbsPoint(BlzGetFrameByName("ResourceBarSupplyText", 0), FRAMEPOINT_TOPRIGHT, 0.4, 0.3)
BlzFrameSetAbsPoint(BlzGetFrameByName("ResourceBarUpkeepText", 0), FRAMEPOINT_TOPRIGHT, 0.4, 0.3)

Single Unit Selection:

"SimpleInfoPanelUnitDetail", 0

"SimpleNameValue",0

"SimpleClassValue",0

"SimpleHeroLevelBar",0

"SimpleProgressIndicator",0

"SimpleBuildTimeIndicator",0

"SimpleClassValue", "SimpleHeroLevelBar" & "SimpleProgressIndicator" are Anchored to "SimpleNameValue" with Top.

"SimpleClassValue" y-Offset -0.0055 while the others have -0.0015

 Lua:

BlzFrameSetAbsPoint(BlzGetFrameByName("SimpleNameValue", 0), FRAMEPOINT_TOP, 0.2, 0.5)
BlzFrameSetAbsPoint(BlzGetFrameByName("SimpleClassValue", 0), FRAMEPOINT_TOP, 0.2, 0.45 -0.004 )
BlzFrameSetAbsPoint(BlzGetFrameByName("SimpleHeroLevelBar", 0), FRAMEPOINT_TOP, 0.2, 0.45)
BlzFrameSetAbsPoint(BlzGetFrameByName("SimpleProgressIndicator", 0), FRAMEPOINT_TOP, 0.2, 0.45)

Moving any "SimpleInfoPanelIcon directly is not recommented as the game reposes them based on current need.

"SimpleInfoPanelIconDamage",0

"SimpleInfoPanelIconDamage",1

"SimpleInfoPanelIconArmor",2

"SimpleInfoPanelIconRank",3

"SimpleInfoPanelIconFood",4

"SimpleInfoPanelIconGold",5

Move single UnitInfoPanels, is best done by moving the "InfoPanelIconBackdrop"

 Lua:

BlzFrameSetAbsPoint(BlzGetFrameByName("InfoPanelIconBackdrop", 0), FRAMEPOINT_TOPLEFT, 0.2, 0.3)
BlzFrameSetAbsPoint(BlzGetFrameByName("InfoPanelIconBackdrop", 1), FRAMEPOINT_TOPLEFT, 0.4, 0.3)
BlzFrameSetAbsPoint(BlzGetFrameByName("InfoPanelIconBackdrop", 2), FRAMEPOINT_TOPLEFT, 0.4, 0.35)
BlzFrameSetAbsPoint(BlzGetFrameByName("InfoPanelIconBackdrop", 3), FRAMEPOINT_TOPLEFT, 0.4, 0.4)
BlzFrameSetAbsPoint(BlzGetFrameByName("InfoPanelIconBackdrop", 4), FRAMEPOINT_TOPLEFT, 0.4, 0.45)
BlzFrameSetAbsPoint(BlzGetFrameByName("InfoPanelIconBackdrop", 5), FRAMEPOINT_TOPLEFT, 0.4, 0.5)

Trainer Selection:

Size: 0.188125 0.1140625

"SimpleInfoPanelBuildingDetail", 1

This Frame is shown when one unit is selected and it trains something.

"SimpleBuildingNameValue", 1

Anchor TOP​
"SimpleBuildingDescriptionValue", 1

TOP, "SimpleBuildingNameValue", BOTTOM​
"SimpleBuildTimeIndicator", 1

TOPLEFT, "SimpleInfoPanelBuildingDetail", TOPLEFT, 0.061250, -0.038125​
"SimpleBuildingActionLabel", 1

CENTER, "SimpleInfoPanelBuildingDetail", TOPLEFT, 0.11375, -0.029875​
"SimpleBuildQueueBackdropFrame", 1

"SimpleBuildQueueBackdrop", 1

SetPoint BOTTOMLEFT, "SimpleInfoPanelBuildingDetail", BOTTOMLEFT

BOTTOMRIGHT, "SimpleInfoPanelBuildingDetail", BOTTOMRIGHT​

 Lua:

-- moving the whole thing
local frame = BlzGetFrameByName("SimpleInfoPanelBuildingDetail", 1)
BlzFrameClearAllPoints(frame)
BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.2, 0.4)
BlzFrameSetSize(frame, 0.188125, 0.1140625)

-- scaling it down
BlzFrameSetScale(BlzGetFrameByName("SimpleInfoPanelBuildingDetail", 1), 0.8)

-- one could move around the individual frames
BlzFrameSetAbsPoint(BlzGetFrameByName("SimpleBuildingNameValue", 1), FRAMEPOINT_TOP, 0.2, 0.3)
BlzFrameSetAbsPoint(BlzGetFrameByName("SimpleBuildingDescriptionValue", 1), FRAMEPOINT_TOP, 0.4, 0.3)
BlzFrameSetAbsPoint(BlzGetFrameByName("SimpleBuildTimeIndicator", 1), FRAMEPOINT_TOPLEFT, 0.1, 0.5)
BlzFrameSetAbsPoint(BlzGetFrameByName("SimpleBuildingActionLabel", 1), FRAMEPOINT_CENTER, 0.3, 0.5)
BlzFrameSetAbsPoint(BlzGetFrameByName("SimpleBuildQueueBackdrop", 1), FRAMEPOINT_BOTTOMLEFT, 0.4, 0.5)

Transporter selection:

"SimpleInfoPanelCargoDetail", 2

"SimpleHoldNameValue", 2

Anchor TOP​
"SimpleHoldDescriptionValue", 2

TOP, "SimpleHoldNameValue", BOTTOM​

 Lua:

BlzFrameSetAbsPoint(BlzGetFrameByName("SimpleHoldNameValue", 2), FRAMEPOINT_TOP, 0.2, 0.3)
BlzFrameSetAbsPoint(BlzGetFrameByName("SimpleHoldDescriptionValue", 2), FRAMEPOINT_TOP, 0.4, 0.3)

Item On Ground selection:

"SimpleInfoPanelItemDetail", 3

"SimpleItemNameValue", 3

Anchor TOP

"SimpleItemDescriptionValue", 3

TOP, "SimpleItemNameValue", BOTTOM

 Lua:

BlzFrameSetAbsPoint(BlzGetFrameByName("SimpleItemNameValue", 3), FRAMEPOINT_TOP, 0.2, 0.3)
BlzFrameSetAbsPoint(BlzGetFrameByName("SimpleItemDescriptionValue", 3), FRAMEPOINT_TOP, 0.4, 0.3)

Gate/Tree/Rock selection:

"SimpleInfoPanelDestructableDetail",4

"SimpleDestructableNameValue",4 Anchored Top

 Lua:

BlzFrameSetAbsPoint(BlzGetFrameByName("SimpleDestructableNameValue", 4), FRAMEPOINT_TOP, 0.2, 0.3)

Group Selection:

This Frame can only be accessed using the Frame Child api (V1.32.6+).

It is anchored to Parent of "SimpleInfoPanelUnitDetail", 0.

 Lua:

--Move the whole thing
local unitFrame = BlzGetFrameByName("SimpleInfoPanelUnitDetail", 0)
local bottomCenterUI = BlzFrameGetParent(unitFrame)
local groupFrame = BlzFrameGetChild(bottomCenterUI, 5)
BlzFrameClearAllPoints(groupFrame)
BlzFrameSetAbsPoint(groupFrame, FRAMEPOINT_CENTER, 0.4, 0.3)
BlzFrameSetSize(groupFrame, 0.188125, 0.1140625)
--Scale it up
BlzFrameSetScale(groupFrame, 1.5)

Move single Buttons of the groupSelection:

 Lua:

--Move 1.Button
local unitFrame = BlzGetFrameByName("SimpleInfoPanelUnitDetail", 0)
local bottomCenterUI = BlzFrameGetParent(unitFrame)
local groupFrame = BlzFrameGetChild(bottomCenterUI, 5)
local buttonContainerFrame = BlzFrameGetChild(groupFrame, 0)
local buttonFrame = BlzFrameGetChild(buttonContainerFrame, 0)
BlzFrameSetAbsPoint(buttonFrame, FRAMEPOINT_TOPLEFT, 0.4, 0.3)
--Move 2.Button
buttonFrame = BlzFrameGetChild(buttonContainerFrame, 1)
BlzFrameSetAbsPoint(buttonFrame, FRAMEPOINT_TOPLEFT, 0.4, 0.34)

HeroInfo Frames:

"SimpleInfoPanelIconHero", 6 is the container.

Anchored with TopLeft

"InfoPanelIconHeroIcon", 6 is the Big Icon showing the primary Attribute.

Anchored Left

"SimpleInfoPanelIconHeroText", 6

Anchored with many points to "SimpleInfoPanelIconHeroText" & "SimpleInfoPanelIconHero"

Following are anchored with TopLeft and are childs of "SimpleInfoPanelIconHeroText".

"InfoPanelIconHeroStrengthLabel"

"InfoPanelIconHeroStrengthValue"

"InfoPanelIconHeroAgilityLabel"

"InfoPanelIconHeroAgilityValue"

"InfoPanelIconHeroIntellectLabel"

"InfoPanelIconHeroIntellectValue"

 Lua:

--Move the whole thing
BlzFrameSetAbsPoint(BlzGetFrameByName("SimpleInfoPanelIconHero", 6), FRAMEPOINT_TOPLEFT, 0.1, 0.5)
--Move the text display
BlzFrameSetAbsPoint(BlzGetFrameByName("InfoPanelIconHeroStrengthLabel", 6), FRAMEPOINT_TOPLEFT, 0.2, 0.5)
--Move the Prim Stat Icon & it's Tooltip Listener
BlzFrameSetAbsPoint(BlzGetFrameByName("InfoPanelIconHeroIcon", 6), FRAMEPOINT_LEFT, 0.2, 0.3)

Ally Selection:

"SimpleInfoPanelIconAlly", 7

Anchored with TopLeft, repeated by the game -> do not move ("SimpleInfoPanelIconAlly", 7)

"InfoPanelIconAllyTitle"

Anchored with TopLeft

"InfoPanelIconAllyGoldIcon"

Anchored with TopLeft

"InfoPanelIconAllyGoldValue"

"InfoPanelIconAllyWoodIcon"

"InfoPanelIconAllyWoodValue"

"InfoPanelIconAllyFoodIcon"

"InfoPanelIconAllyFoodValue"

"InfoPanelIconAllyUpkeep"

The Textures are anchored to the previous Textures and the Strings next to the Textures.

But "InfoPanelIconAllyTitle" & "InfoPanelIconAllyGoldIcon" are anchored to "SimpleInfoPanelIconAlly".

The tooltip Listener will follow their Icon.

 Lua:

--Move the whole thing
BlzFrameSetAbsPoint(BlzGetFrameByName("InfoPanelIconAllyTitle", 7), FRAMEPOINT_TOPLEFT, 0.4, 0.3)
BlzFrameSetAbsPoint(BlzGetFrameByName("InfoPanelIconAllyGoldIcon", 7), FRAMEPOINT_TOPLEFT, 0.4, 0.3 -0.009)

Multiboard:

Using UI-Frame api you can do some stuff with Multiboards. You can show more than one at a time, move them around or change their textures.

Multiboard position is affected by current resolution. With x-pos 0.8 it moves to right screen border (V1.31.1).

"Multiboard"

"MultiboardMinimizeButton"

"MultiboardTitleBackdrop"

"MultiboardTitle"

"MultiboardBackdrop"

"MultiboardListContainer"

 Lua:

BlzFrameSetAbsPoint(BlzGetFrameByName("Multiboard", 0), FRAMEPOINT_TOPRIGHT, 0.4, 0.5)

Leaderboard:

call BlzFrameSetAbsPoint( BlzGetFrameByName("Leaderboard", 0), FRAMEPOINT_TOPRIGHT, 0.8, 0.3) moves it to the right screen border.

If a map has a Leaderboard & a Multiboard the Leaderboard gets reposed below the Multiboard when "MultiboardMinimizeButton" is clicked

"Leaderboard", 0

Anchored Topright

"LeaderboardBackdrop", 0

"LeaderboardTitle", 0

"LeaderboardListContainer", 0

 Lua:

BlzFrameSetAbsPoint(BlzGetFrameByName("Leaderboard", 0), FRAMEPOINT_TOPRIGHT, 0.4, 0.3)

TimerDialog:

Like Multiboard & Leaderboard, abs pos FRAMEPOINT_TOPRIGHT with 0.8 is the right screen border

TimerDialog is bad to use with UI-Frame API, just create your own using UI-Frame API.

Dialog:

Dialog is bad to use with UI-Frame API.

Unit Info Panel

##
 Simple UI changes

This section is a collection of UI changes to do various small edits to the default UI.

Hide the Hero-Exps tooltip for the whole game:

 JASS:

call BlzFrameSetTooltip(BlzGetFrameByName("SimpleHeroLevelBar", 0), BlzCreateFrameByType("SIMPLEFRAME", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0))

Frame - Create Frame by Type: SIMPLEFRAME name: HideXPTooltip parent: (Get Frame By Name:ConsoleUI Context:0) inherits: <Leerer String> CreateContext: 0

Frame - (Get Frame By Name:SimpleHeroLevelBar Context:0) uses (Get Frame By Name:HideXPTooltip Context:0) as Tooltip
Hide the Hero-Attributes:

 JASS:

call BlzFrameSetVisible(BlzGetFrameByName("SimpleInfoPanelIconHeroText", 6), false)
call BlzFrameSetSize(BlzGetFrameByName("InfoPanelIconHeroIcon", 6), 0.00001, 0.00001)

Frame - show - (Get Frame By Name:SimpleInfoPanelIconHeroText Context:6) -_ False

Frame - Set Size of (Get Frame By Name:InfoPanelIconHeroIcon Context:6) to (1.00 / 10000.00)/(1.00 / 10000.00)
Disable the UpKeep-Tooltip (Warcraft 3 V1.32.6+):

 JASS:

call BlzFrameSetVisible(BlzFrameGetChild(BlzGetFrameByName("ResourceBarFrame", 0), 2), false)

Make the Cinematic Messages Center Aligned:

 JASS:

call BlzFrameSetTextAlignment(BlzGetFrameByName("CinematicDialogueText", 0), TEXT_JUSTIFY_MIDDLE, TEXT_JUSTIFY_CENTER)

Expand Quest-Description's space by taking Quest-Defeat-Condition reserved space.

 JASS:

function QuestCheck takes nothing returns nothing
 call BlzFrameClick(BlzGetFrameByName("UpperButtonBarQuestsButton", 0))
 call BlzFrameClick(BlzGetFrameByName("QuestAcceptButton", 0))
 call BlzFrameSetSize(BlzGetFrameByName("QuestItemListContainer", 0), 0.01, 0.01)
 call BlzFrameSetSize(BlzGetFrameByName("QuestItemListScrollBar", 0), 0.001, 0.001)
endfunction

Hide BuffBar, as the BuffBar is reshown with selection a simple BlzFrameSetVisible false won't do it. Instead one creates a hidden parent and changes parentship:

 JASS:

function Test takes nothing returns nothing
 local framehandle newParent = BlzCreateFrameByType("SIMPLEFRAME", "", BlzGetFrameByName("ConsoleUI", 0), "", 0)
 call BlzFrameSetVisible(newParent, false)
 call BlzFrameSetParent(BlzGetOriginFrame(ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR, 0), newParent)
endfunction

##
 Frame Child & Frame

Frame - Parent is a powerful and useful concept of warcraft 3's UI-Frame System.

Every frame has a parent frame. Any frame can have any amount of child frames. In the same time any child frame also can have any amount of child frames.

natives provided to get and set the current parent of a frame during the game.

native BlzFrameSetParent takes framehandle frame, framehandle parent returns nothingnative BlzFrameGetParent takes framehandle frame returns framehandle

A parent is also set when creating a new frame, as all create frame natives require an owner/parent.

In many examples custom Frames are created as child of GAME_UI which is done to simple down the example. But it is ineffective to create all custom Frames for GAME_UI, if one has Frames that are logical connected it would make more sense to collecte them as childs of one Frame.

For Frames "FRAME" could be used.

For SimpleFrames "SIMPLEFRAME".

Both are created with BlzCreateFrameByType

FrameA child of Game_UI

FrameB child of FrameA

FrameC child of FrameA

FrameD child of FrameA

FrameA is the parent a logical container of FrameB, FrameC and FrameD.

###
 Why I should bother this concept?

The FrameParent concept simples down a lot of stuff you might want to accomplish. Want to create a new option in menu, create your Frame for ("EscMenuMainPanel", 0). Now the frame requires the visibility of ("EscMenuMainPanel", 0) and you can focus on the behaviour of the frame itself instead of handling the visiblity which can be quite complicated when injecting into the normal warcraft 3 UI. In my opinion the most powerful effect of frame-parentship.

Want to add a new UnitInfo (if you find a place for it on the screen) create it for ("SimpleInfoPanelUnitDetail", 0) then it only will be visible when the local player has only one unit selected. Through this is a bit nah, cause you need to know the selected unit to update the displayed data (in such unit cases).

If you wouldn't use this parent concept for visibility you would have to find out with triggers/code that the local player has only one unit selected, or is inside the menu for the example and also not in cinematic mode.

###
 What are the Effects of Parent-Child

A child frame can only be visible when it's parent frame is visible. BlzFrameIsVisible also checks for the visibility of the parent frame. New created frames copy scale and alpha from their parent on default. This values can differ for the children when they are set (for the child frame) after the child frame was added to /created for the parent frame.

BlzFrameSetScale

BlzFrameSetAlpha

When changing Parents the alpha value of the new Parent is copied.​
BlzFrameSetLevel

Frames created for a parent frame also have the same Level.​

A child tends to be visible above and is prefered by mouse events over its parent.

###
 2 Types of children

I split childFrames in 2 Types loose children and functional children.

Functional child frames can only be defined and created inside fdf such children have a strong connection to their parent and perform some functionality for the parentFrame hence the name. An example of such functional child frame is ButtonText "ScriptDialogButtonText",

This childFrame styles the text for it's parent frame "ScriptDialogButton". Functional childFrames copy even more from the ParentFrame, their position is automatically handled and they copy the enable state.

All ChildFrames not being functional child-Frames are Loose child-Frames. They can be added in fdf and code. They do not perform any direct functionality for their parent. This code creates a new loose child frame for parentFrame. BlzCreateFrame("frameName", parentFrame, 0, 0) The position of a loose childFrame is on default independent from it's parent, it does not even require the parent frame to be placed on the screen.

That behaviour can be used to create Frames with a higher FrameLevel then their type supports: Create A BUTTON set its FrameLevel and create the wanted Frames using him as parent for example TEXT-Frames.

Want to show/hide a whole bunch of Frames collect them under one ParentFrame and change the ParentFrame's visibility. Now it only takes one line to toggle the visibility.

###
 Parent Limitations?

SimpleFrames can not be children nor parent of Frames. Frames can not be Children/Parent of SimpleFrames. Some might now say: "but I created a BUTTON for SIMPLEFRAME in fdf or, I created SIMPLEFRAME for a BUTTON during the game."

Yes, you did but on a more accurate inspection, by comparing HandleIds of the parent of the new frame and the wanted/should be parent one might see they do not match, in both cases you see the handleId of another frame, I call that a substitute parent.

When that happens one might at first wonder why visibility and other parent behaviours do not apply. But the reason: the wanted parent is not the real parent and so it does not copy the visibility/alpha... .

In 1.31 BlzFrameSetParent can bug sometimes (it speeds up the animation of "SPRITE" and can empower the glowing of "HIGHLIGHT" when changing the parent for this frames directly).

###
 Example

This is a Lua code creating a clickable Button that is only visible while the player is inside the Menu (F10), the code executes itself when insert into a map in Lua mode.

When the button is clicked the message "action in Menu" is printed onto the screen.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 local buttonFrame = BlzCreateFrameByType("BUTTON", "", BlzGetFrameByName("EscMenuMainPanel", 0), "ScoreScreenTabButtonTemplate", 0)
 local iconFrame = BlzCreateFrameByType("BACKDROP", "", buttonFrame, "", 0)
 BlzFrameSetAllPoints(iconFrame, buttonFrame)
 BlzFrameSetSize(buttonFrame, 0.03, 0.03)
 BlzFrameSetAbsPoint(buttonFrame, FRAMEPOINT_TOPLEFT, 0.1, 0.45)
 BlzFrameSetTexture(iconFrame, "ReplaceableTextures\\WorldEditUI\\Doodad-Cinematic.blp", 0, true)
 local trigger = CreateTrigger()
 TriggerAddAction(trigger, function()
 print("action in Menu")
 end)
 BlzTriggerRegisterFrameEvent(trigger, buttonFrame, FRAMEEVENT_CONTROL_CLICK)
 end
end

ButtonInF10Menu
Events
Time - Elapsed game time is 1.00 seconds

Conditions

Actions
Frame - Create Frame by Type: BUTTON name: MyEscMenuMainPanelButton parent: (Get Frame By Name:EscMenuMainPanel Context:0) inherits: ScoreScreenTabButtonTemplate CreateContext: 0

-------- Load the frames into frame variables --------

Set Frame = (Get Frame By Name:MyEscMenuMainPanelButton Context:0)

Frame - Create Frame by Type: BACKDROP name: MyEscMenuMainPanelButtonIcon parent: Frame inherits: <Leerer String> CreateContext: 0

-------- Load the frames into frame variables --------

Set FrameIcon = (Get Frame By Name:MyEscMenuMainPanelButtonIcon Context:0)

Frame - Set Size of Frame to 0.04/0.04

Frame - Set Absolute Point for: Frame place FRAMEPOINT_TOPLEFT to 0.10/0.45

Frame - Set all Points for:FrameIcon to Frame

Frame - Set Texture of FrameIcon to ReplaceableTextures\CommandButtons\BTNHeroPaladin flags: 0 blend False

-------- Add Event to the other trigger --------

Trigger - Add to ButtonInF10MenuAction <gen> the event (Frame - Frame Event: FRAMEEVENT_CONTROL_CLICK)

ButtonInF10MenuAction
Events

Conditions

Actions
Game - Display to (All players) the text: Custom F10 Button C...

-------- Remove Focus, so space/enter dont click the button again --------

Frame - Enable:(Get Triggering Frame) False

Frame - Enable:(Get Triggering Frame) True

Although this is a bit vague cause currently it also is visible while choosing to end game and not visible while being inside options. If you plan on doing something like that you have to find out more about that frames in detail and which is the one suited for your wanted goal. This menu frames are in "ui/framedef/ui" when looking with cascview.

This was written in mind for warcraft 3 V1.31.1.

##
 Frame Order by Level

When multiple Frames are at the same space there is an rule that tells who will take the mouse Events and is displayed on top. FrameLevel set with BlzFrameSetLevel manages that, it only matters when Frames colide/overlap on the screen. Only visible Frames matter for the FrameLevel order.

FrameLevel works different for SimpleFrames and Frames.

###
 For Frames:

FrameLevel define the order in which child-Frames are above their Parent. Frames only compete with their siblings. They are ordered descending. The childFrame with the highest Level is onTop. the one with the lowest Level is directly above the parent but below it's brothers and sisters. A Frame's children are below a sibling with a higher level.

FrameLevel only matter for the current Children when you give a FrameChild an high Level and create further silberlings it will not be above them.

###
 For SimpleFrames:

The one with the highest Level is on Top, parentship does not matter. When a SimpleFrame is created it copies the current Level of the Parent so it is above it. But one can change the Levels to have the parent on top, this also means that later changes to Level of the Parent are not taken over by child-SimpleFrames except for Texture/Strings, they do.

One could say both use the same rule except that Frames start new Layers in which the Frame's children are placed while SimpleFrames have only one Layer, started by the SimpleFrame Ancient. But the special SimpleFrame children Texture/String are in Layers started by their parent SimpleFrame. Texture/String can't have children hence the layer mechanic for that group is quite simple.

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

SIMPLEBUTTON control-click-Event is prioritized against non-SimpleFrames with a higher Level.

In Warcraft 3 Versions 1.31.1 using BlzFrameSetLevel onto this FrameTypes crashes the game, there is no crash in V1.32.2.

BACKDROP

FRAME

HIGHLIGHT

MODEL

SPRITE

TEXT

TIMERTEXT

A SIMPLEFRAME with a FrameLevel of 2 or higher is displayed over unit lifebars and covers them, when created as child of GAME_UI.

##
 Create-Frames

Warcraft 3 provides 3 natives to create Frames into the running match.

 JASS:

native BlzCreateFrame takes string name, framehandle owner, integer priority, integer createContext returns framehandle
native BlzCreateSimpleFrame takes string name, framehandle owner, integer createContext returns framehandle
native BlzCreateFrameByType takes string typeName, string name, framehandle owner, string inherits, integer createContext returns framehandle

BlzCreateFrame & BlzCreateSimpleFrame are used to create MainFrames (the ones outside of Blocks) defined in a FDF which has to be loaded into the game with TOC.

BlzCreateFrame creates MainFrames which are not SimpleFrames. Most of this frames allow attaching various Frameevents and they can do alot of things.

BlzCreateSimpleFrame creates MainFrames of the Simple group. SimpleFrames have "SIMPLE" in their type name. Most Simpleframes can not have frameevents registered onto them

Lets checkout the arguments

string name
this is the name of the frame you want to create, you have to take it from a loaded fdf. Its the 2.word in "" after Frame.

framehandle owner
A reference to some already existing Frame, it becomes the parent of the new frame. A SimpleFrame wants a SimpleFrame and a Frame wants a Frame, Mixing them tends to fail or break some features of Child-Parent.

integer priority
Should be a natural number (greater equal to 0).

integer createContext
The index in the frame storage the new Frame and it's Child-Frames will take. The frame storage is accessed with BlzGetFrameByName("name", createContext)

BlzCreateFrameByType defines a new Frame and creates it in the same call. Hence one has to tell quite a bunch of things about the new Frame. If you want to create your Frames without fdf/TOC this native is the way to go.

Checkout the changed arguments for BlzCreateFrameByType

string typeName
this is the basic behaviour of the new frame. It is the 1.word in "" after Frame. FrameTypes

string name
This is the name the new Frame is given.

string inherits
The name of the frame you to copy, you have to take it from a loaded fdf. Its the 2.word in "" after Frame. Technically in fdf it is INHERITS WITHCHILDREN.

If one want the click sound feature of GLUE Frames one needs to create them with BlzCreateFrame.

###
 How to practicaly create a Frame

The previous was explaining the create natives, but it did not show yet how to bring a new Frame into the game. To add a new Frame into the game one has to perform this steps, they can alter when using fdf.

Select and Get The Parent Frame.

Create the new Frame as child of the selected Parent Frame

customize Frame's position/size/texture/text... .

First one has to select a parentFrame.

 JASS:

local framehandle parent = BlzGetFrameByName("ConsoleUIBackdrop", 0)

After that one creates the new Frame as child of that parent. "QuestButtonDisabledBackdropTemplate" is a name of a Frame defined in fdf.

 JASS:

local framehandle frame = BlzCreateFrame("QuestButtonDisabledBackdropTemplate", parent, 0, 0)

Now that the Frame was created one has to set position/size/texture/text, if they were not already set in fdf. In which order you set position size/texture/text does not matter.

 JASS:

call BlzFrameSetSize(frame, 0.08, 0.08)
call BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.4, 0.5)

###
 BACKDROP Without FDF

It is quite common to display some image onto the screen, this can be done with only some lines of code. One can create a BACKDROP using BlzCreateFrameByType, afterwards set texture, position and size of that BACKDROP and then you have a visible image on the screen.

BACKDROP can use all the ImageTypes Warcraft 3 supports: BLP, TGA, DDS. DDS can only be used in Warcraft 3 V1.32+. For DDS the compression type matters, in my Test only DXT1/DXT5 worked (generated with GIMP).

The Example creating a backdrop with BlzCreateFrameByType is from (xorkatoss)

 JASS:

local framehandle blademaster = BlzCreateFrameByType("BACKDROP", "Blademaster", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
call BlzFrameSetSize(blademaster, 0.04, 0.04)
call BlzFrameSetAbsPoint(blademaster, FRAMEPOINT_CENTER, 0.4, 0.3)
call BlzFrameSetTexture(blademaster, "ReplaceableTextures\\CommandButtons\\BTNHeroBlademaster",0, true)

BackdropNoFDF
Events
Time - Elapsed game time is 1.00 seconds

Conditions

Actions
Frame - Create Frame by Type: BACKDROP name: icon parent: GameUI inherits: <Leerer String> CreateContext: 0

Set FrameIcon = (Get Frame By Name:icon Context:0)

Frame - Set Size of FrameIcon to 0.04/0.04

Frame - Set Absolute Point for: FrameIcon place FRAMEPOINT_CENTER to 0.40/0.30

Frame - Set Texture of FrameIcon to ReplaceableTextures\CommandButtons\BTNHeroBlademaster.blp flags: 0 blend False

One also might like to create a Box BackDrop, there are some made by Blizzard. One could use them without any custom TOC.

Some BACKDROPS might be unsuited for some sizes.

This are the names of this Frames:

"EscMenuBackdrop"

"QuestButtonBaseTemplate"

"ScoreScreenButtonBackdropTemplate"

"QuestButtonDisabledBackdropTemplate"

"QuestButtonPushedBackdropTemplate"

"Leaderboard"

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 local frame = BlzCreateFrame("EscMenuBackdrop", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.1, 0.3)
 BlzFrameSetSize(frame, 0.08, 0.08)

 frame = BlzCreateFrame("QuestButtonBaseTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.2, 0.3)
 BlzFrameSetSize(frame, 0.08, 0.08)

 frame = BlzCreateFrame("ScoreScreenButtonBackdropTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.3, 0.3)
 BlzFrameSetSize(frame, 0.08, 0.08)

 frame = BlzCreateFrame("QuestButtonDisabledBackdropTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.4, 0.3)
 BlzFrameSetSize(frame, 0.08, 0.08)

 frame = BlzCreateFrame("QuestButtonPushedBackdropTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.5, 0.3)
 BlzFrameSetSize(frame, 0.08, 0.08)

 frame = BlzCreateFrame("Leaderboard", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.6, 0.3)
 BlzFrameSetSize(frame, 0.08, 0.08)
 end
end

Boxes
Events
Time - Elapsed game time is 1.00 seconds

Conditions

Actions
Frame - Create Frame name: EscMenuBackdrop parent: GameUI Priority: 0 CreateContext: 0

Set FrameIcon = (Get Frame By Name:EscMenuBackdrop Context:0)

Frame - Set Size of FrameIcon to 0.08/0.08

Frame - Set Absolute Point for: FrameIcon place FRAMEPOINT_CENTER to 0.10/0.30

-------- ------ --------

Frame - Create Frame name: QuestButtonBaseTemplate parent: GameUI Priority: 0 CreateContext: 0

Set FrameIcon = (Get Frame By Name:QuestButtonBaseTemplate Context:0)

Frame - Set Size of FrameIcon to 0.08/0.08

Frame - Set Absolute Point for: FrameIcon place FRAMEPOINT_CENTER to 0.20/0.30

-------- ------ --------

Frame - Create Frame name: ScoreScreenButtonBackdropTemplate parent: GameUI Priority: 0 CreateContext: 0

Set FrameIcon = (Get Frame By Name:ScoreScreenButtonBackdropTemplate Context:0)

Frame - Set Size of FrameIcon to 0.08/0.08

Frame - Set Absolute Point for: FrameIcon place FRAMEPOINT_CENTER to 0.30/0.30

-------- ------ --------

Frame - Create Frame name: Leaderboard parent: GameUI Priority: 0 CreateContext: 0

Set FrameIcon = (Get Frame By Name:Leaderboard Context:0)

Frame - Set Size of FrameIcon to 0.08/0.08

Frame - Set Absolute Point for: FrameIcon place FRAMEPOINT_CENTER to 0.40/0.30

If you wana change border Size, well then you have to handle BACKDROP in fdf there one make the border smaller or bigger and apply some other settings.

###
 Text without FDF

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 -- create a TEXT Frame
 local frame = BlzCreateFrameByType("TEXT", "MyTextFrame", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
 -- Set the current Text, you can use the Warcraft 3 Color Code
 BlzFrameSetText(frame, "Text on |cffffcc00Screen|r")
 -- pos the frame
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.4, 0.3)
 -- stop this frame from taking control of the mouse input, Might have sideeffects if the TEXT-Frame has an enable Color (but this does not have such).
 BlzFrameSetEnable(frame, false)
 -- the text is kinda small, but one can not use the FontNative onto TEXT-Frames (nether in V1.31 nor V1.32). Therefore one could scale it.
 BlzFrameSetScale(frame, 2)
 end
end

TextNoFDF
Events
Time - Elapsed game time is 1.00 seconds

Conditions

Actions
-------- create a TEXT Frame --------

Frame - Create Frame by Type: TEXT name: MyTextFrame parent: GameUI inherits: <Leerer String> CreateContext: 0

Set Frame = (Get Frame By Name:MyTextFrame Context:0)

-------- Set the current Text, you can use the Warcraft 3 Color Code --------

Frame - Set Text of: Frame to (<Leerer String> + Text on |cffffcc00Screen|r)

-------- pos the frame --------

Frame - Set Absolute Point for: Frame place FRAMEPOINT_CENTER to 0.40/0.30

-------- stop this frame from taking control of the mouse input, Might have sideeffects if the TEXT-Frame has an enable Color (but this does not have such). --------

Frame - Enable:Frame False

-------- the text is kinda small, but one can not use the FontNative onto TEXT-Frames (nether in V1.31 nor V1.32). Therefore one could scale it. --------

Frame - Set Scale of Frame to 2.00

BlzFrameSetScale scales relative position x&y-offset of the scaled Frame, displaces the TEXT-Frame. To counter that use wanted-offsets/scale. 0.05, 0.01 scale 2 -> 0.05/2, 0.01/2

###
 TextButton Without FDF

Many want an additional clickable Button when it is clicked something should happen.

Let's create a GLUETEXTBUTTON looking like the Buttons found in Dialogs. "ScriptDialogButton" is their name.

But the game has some hardcoded behaviour for "ScriptDialogButton" when using Dialogs. Therefore using BlzCreateFrame can end with some Problems. This problem can be avoided with BlzCreateFrameByType giving our Frames a different name. Then there is not colision with the Dialog usage. (Sabe found that problem)

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

 -- create the trigger handling the Button Clicking
 local trigger = CreateTrigger()

 -- register the Click event
 BlzTriggerRegisterFrameEvent(trigger, button, FRAMEEVENT_CONTROL_CLICK)

 -- this happens when the button is clicked
 TriggerAddAction(trigger, function()
 local frame = BlzGetTriggerFrame()
 print(BlzFrameGetName(frame),"was Clicked")
 end)
 end
end

FrameExampleTextButtonNoFDF
Events
Time - Elapsed game time is 1.00 seconds

Conditions

Actions
-------- First Execution create Frame --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Execution count of (This trigger)) Equal to 1

Then - Actions
-------- create a new Button which inherits from "ScriptDialogButton" --------

Frame - Create Frame by Type: GLUETEXTBUTTON name: MyScriptDialogButton parent: GameUI inherits: ScriptDialogButton CreateContext: 0

Set Frame = (Get Frame By Name:MyScriptDialogButton Context:0)

-------- place the Button to the center of the Screen --------

Frame - Set Absolute Point for: Frame place FRAMEPOINT_CENTER to 0.40/0.30

-------- set the Button's text --------

Frame - Set Text of: Frame to (<Leerer String> + My Button Text)

-------- Add Click Event --------

Trigger - Add to (This trigger) the event (Frame - Frame Event: FRAMEEVENT_CONTROL_CLICK)

Else - Actions
-------- all further executions are clicks --------

Game - Display to (All players) the text: ((Get Name of Frame: (Get Triggering Frame)) + ( was clicked by + (Name of (Triggering player))))

-------- Remove Focus, so space/enter dont click the button again --------

Frame - Enable:(Get Triggering Frame) False

Frame - Enable:(Get Triggering Frame) True

One could create a Blue Button with "DebugButton" instead of "ScriptDialogButton".

###
 TextButton with AutoCast Model Without FDF

This example creates a ScriptDialogButton at the center of the Screen and places an animated AutoCast model (SPRITE) over/around it.

Visual parts of Models do not care about FrameSize, but one can scale it using BlzFrameSetScale. The default CommandButton Size is 0.039, therefore Buttonsize/0.039 fits the model to the Size of your button.

 Lua:

function Test()
 -- create the button and place it onto the center of the screen
 local button = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 BlzFrameSetAbsPoint(button, FRAMEPOINT_CENTER, 0.4, 0.3)
 BlzFrameSetSize(button, 0.039, 0.039)

 -- create a Sprite copy the button's positions
 local model = BlzCreateFrameByType("SPRITE", "SpriteName", button, "", 0)
 BlzFrameSetAllPoints(model, button)
 -- apply the model
 BlzFrameSetModel(model, "UI\\Feedback\\Autocast\\UI-ModalButtonOn.mdl", 0)
 -- Models don't care about Frame Size, to resize them one needs to scale them. The default CommandButton has a Size of 0.039.
 BlzFrameSetScale(model, BlzFrameGetWidth(button)/0.039)
end

FrameExampleTextButtonAutoCastEffectNoFDF
Events
Time - Elapsed game time is 1.00 seconds

Conditions

Actions
-------- create the button and place it onto the center of the screen --------

Frame - Create Frame by Type: GLUETEXTBUTTON name: MyScriptDialogButton parent: GameUI inherits: ScriptDialogButton CreateContext: 0

Set Frame = (Get Frame By Name:MyScriptDialogButton Context:0)

Frame - Set Absolute Point for: Frame place FRAMEPOINT_CENTER to 0.40/0.30

Frame - Set Size of Frame to 0.04/0.04

-------- create a Sprite copy the button's positions --------

Frame - Create Frame by Type: SPRITE name: SpriteName parent: Frame inherits: <Leerer String> CreateContext: 0

Set FrameIcon = (Get Frame By Name:SpriteName Context:0)

Frame - Set all Points for:FrameIcon to Frame

Frame - Set Model Frame:FrameIcon Model: UI\Feedback\Autocast\UI-ModalButtonOn.mdl Camera: 0

-------- Models don't care about Frame Size, to resize them one needs to scale them. The default CommandButton has a Size of 0.039. --------

Frame - Set Scale of FrameIcon to ((Get Width of Frame: Frame) / (0.39 / 10.00))

###
 Sprite RockBoltMissile Without FDF

How to use an World Model inside the UI-Frames using the Frame-SPRITE.

 Lua:

function Test2()
 -- create a Sprite copy the button's positions
 local model = BlzCreateFrameByType("SPRITE", "SpriteName", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
 BlzFrameSetAbsPoint(model, FRAMEPOINT_CENTER, 0.4, 0.3)
 BlzFrameSetSize(model, 0.01, 0.01)
 -- apply the model
 BlzFrameSetModel(model, "Abilities\\Weapons\\RockBoltMissile\\RockBoltMissile.mdl", 0)
 -- Models don't care about Frame Size, But world Object Models are huge . To use them in the UI one has to scale them down alot.
 BlzFrameSetScale(model, 0.00006)
 -- play death animation
 -- birth = 0
 -- death = 1
 -- stand = 2
 -- morph = 3
 -- alternate = 4
 BlzFrameSetSpriteAnimate(model, 1, 0)
end

###
 IconButton without custom FDF

Another common thing is creating an Button visualy represented by an CommandButton-Icon. Here one creates 2 Frames a BUTTON the clickable part and a BACKDROP showing the Icon.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 -- create a new BUTTON inheriting from "ScoreScreenTabButtonTemplate". With the inherit one gains a yellow on mouse hover glowing and blocks right clicks onto the button from reaching the ground below the button
 local button = BlzCreateFrameByType("BUTTON", "MyIconButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "ScoreScreenTabButtonTemplate", 0)
 --create a BACKDROP for Button which displays the Texture
 local buttonIconFrame = BlzCreateFrameByType("BACKDROP", "MyIconButtonIcon", button, "", 0)
 -- buttonIcon will mimic buttonFrame in size and position
 BlzFrameSetAllPoints(buttonIconFrame, button)
 -- place the Button to the left center of the Screen
 BlzFrameSetAbsPoint(button, FRAMEPOINT_CENTER, 0.1, 0.3)
 -- set the Button's Size
 BlzFrameSetSize(button, 0.03, 0.03)
 -- set the texture
 BlzFrameSetTexture(buttonIconFrame, "ReplaceableTextures\\CommandButtons\\BTNSelectHeroOn", 0, false)


 -- create the trigger handling the Button Clicking
 local trigger = CreateTrigger()

 -- register the Click event
 BlzTriggerRegisterFrameEvent(trigger, button, FRAMEEVENT_CONTROL_CLICK)

 -- this happens when the button is clicked
 TriggerAddAction(trigger, function()
 local frame = BlzGetTriggerFrame()
 print(BlzFrameGetName(frame),"was Clicked")
 end)
 end
end

FrameExampleIconButtonNoFDF
Events
Time - Elapsed game time is 1.00 seconds

Conditions

Actions
-------- First Execution create Frame --------

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Execution count of (This trigger)) Equal to 1

Then - Actions
-------- create a new Button which inherits from "ScriptDialogButton" --------

Frame - Create Frame by Type: BUTTON name: MyIconButton parent: GameUI inherits: ScoreScreenTabButtonTemplate CreateContext: 0

Set Frame = (Get Frame By Name:MyIconButton Context:0)

Frame - Create Frame by Type: BACKDROP name: MyIconButtonIcon parent: Frame inherits: <Leerer String> CreateContext: 0

Set FrameIcon = (Get Frame By Name:MyIconButtonIcon Context:0)

Frame - Set all Points for:FrameIcon to Frame

Frame - Set Texture of FrameIcon to ReplaceableTextures\CommandButtons\BTNSelectHeroOn flags: 0 blend False

Frame - Set Size of Frame to 0.04/0.04

Frame - Set Absolute Point for: Frame place FRAMEPOINT_TOPLEFT to 0.30/0.55

-------- Add Click Event --------

Trigger - Add to (This trigger) the event (Frame - Frame Event: FRAMEEVENT_CONTROL_CLICK)

-------- --- --------

-------- Create Tooltip Text with Box --------

Frame - Create Frame by Type: FRAME name: MyLeaderBoard parent: Frame inherits: Leaderboard CreateContext: 0

Set FrameTool = (Get Frame By Name:MyLeaderBoard Context:0)

Frame - Create Frame by Type: TEXT name: MyLeaderBoardText parent: FrameTool inherits: <Leerer String> CreateContext: 0

Set FrameText = (Get Frame By Name:MyLeaderBoardText Context:0)

-------- Box is around the text --------

Frame - Set Point - Frame: FrameTool place FRAMEPOINT_TOPLEFT to FrameTexts FRAMEPOINT_TOPLEFT with offset: -0.01/0.01

Frame - Set Point - Frame: FrameTool place FRAMEPOINT_BOTTOMRIGHT to FrameTexts FRAMEPOINT_BOTTOMRIGHT with offset: 0.01/-0.01

Frame - Frame uses FrameTool as Tooltip

Frame - Set Absolute Point for: FrameText place FRAMEPOINT_TOPLEFT to 0.40/0.30

-------- Text has a specific xSIze and displays further new lines when this xSize is exeeded --------

Frame - Set Size of FrameText to 0.10/0.00

Frame - Set Text of: FrameText to (<Leerer String> + My TooltipText)

Else - Actions
-------- all further executions are clicks --------

Game - Display to (All players) the text: ((Get Name of Frame: (Get Triggering Frame)) + ( was clicked by + (Name of (Triggering player))))

-------- Remove Focus, so space/enter dont click the button again --------

Frame - Enable:(Get Triggering Frame) False

Frame - Enable:(Get Triggering Frame) True

There is another loaded inherit option "IconButtonTemplate" (blue light).

###
 ScritpDialogButon 2 Icon Button

This is an example for Warcraft 3 V1.32.6+ to create an Icon Button which will become smaller when clicked. The example uses no custom fdf code nor onclick code which adjusts sizes. The Lua code runs out of the box in Warcraft 3.

ScriptDialogButton has 4 backdrops, one for each state: Default, Pushed, Disabled, PushedDisabled. Using the child api one can access them change texture and and repose the pushed State (to have the shrinked effect while holding down).

 Lua:

do
 -- hook the function to make this execute itself
 local real = MarkGameStarted
 function MarkGameStarted()
 -- run the previous function with that name
 real()
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


 -- create the trigger handling the Button Clicking
 local trigger = CreateTrigger()

 -- register the Click event
 BlzTriggerRegisterFrameEvent(trigger, button, FRAMEEVENT_CONTROL_CLICK)

 -- this happens when the button is clicked
 TriggerAddAction(trigger, function()
 local frame = BlzGetTriggerFrame()
 print(BlzFrameGetName(frame),"was Clicked")
 end)
 end
end

Similiar code as above but does not use child native hence requires to load in the EscMenuTemplates fdf and works in V1.31

 Lua:

do
 -- hook the function to make this execute itself
 local real = MarkGameStarted
 function MarkGameStarted()
 -- run the previous function with that name
 real()
 BlzLoadTOCFile("war3mapImported\\Templates.toc")
 -- create a new Frame using the blueprint "EscMenuButtonTemplate" from a fdf. That Frame has a yellow on mouse hover glowing and a backdrop for each state Default Pushed, disabled
 local button = BlzCreateFrame("EscMenuButtonTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 -- place the Button to the left center of the Screen
 BlzFrameSetAbsPoint(button, FRAMEPOINT_CENTER, 0.65, 0.3)
 -- set the Button's Size
 BlzFrameSetSize(button, 0.049, 0.049)
 -- get the Backdrop for default State
 local buttonIconFrame = BlzGetFrameByName("ButtonBackdropTemplate", 0)
 -- get the Backdrop for pushed State
 local buttonIconPushedFrame = BlzGetFrameByName("ButtonPushedBackdropTemplate", 0)
 -- get the Backdrop for pushed State
 local buttonIconPushedFrame = BlzGetFrameByName("ButtonPushedBackdropTemplate", 0)

 -- set the textures
 BlzFrameSetTexture(buttonIconFrame, BlzGetAbilityIcon(FourCC('Hpal')), 0, false)
 BlzFrameSetTexture(buttonIconPushedFrame, BlzGetAbilityIcon(FourCC('Hpal')), 0, false)

 -- make the pushedIconFrame smaller, it copies the button so it has to be unbind first
 BlzFrameClearAllPoints(buttonIconPushedFrame)
 BlzFrameSetPoint(buttonIconPushedFrame, FRAMEPOINT_BOTTOMLEFT, button, FRAMEPOINT_BOTTOMLEFT, 0.005, 0.005)
 BlzFrameSetPoint(buttonIconPushedFrame, FRAMEPOINT_TOPRIGHT, button, FRAMEPOINT_TOPRIGHT, -0.005, -0.005)

 -- create the trigger handling the Button Clicking
 local trigger = CreateTrigger()

 -- register the Click event
 BlzTriggerRegisterFrameEvent(trigger, button, FRAMEEVENT_CONTROL_CLICK)

 -- this happens when the button is clicked
 TriggerAddAction(trigger, function()
 local frame = BlzGetTriggerFrame()
 print(BlzFrameGetName(frame),"was Clicked")
 end)
 end
end

###
 SimpleButton Icon without custom FDF

An example how to create a Icon simplebutton without custom fdf. has some overload as it spawns visuals of UpperButtonBarButtonTemplate, but it works out of the box.

 Lua:

function TestSimpleButton()
 frame = BlzCreateSimpleFrame("UpperButtonBarButtonTemplate", BlzGetFrameByName("ConsoleUI", 0), 0)
 icon = BlzCreateFrameByType("SIMPLESTATUSBAR", "name", frame, "", 0)
 BlzFrameSetValue(icon, 100)
 BlzFrameSetAllPoints(icon, frame)
 BlzFrameSetTexture(icon, BlzGetAbilityIcon(FourCC'Hpal'), 0, true)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.4, 0.4)
 BlzFrameSetEnable(icon, false)
 Trig = CreateTrigger()
 for i = 1, 3 do BlzTriggerRegisterFrameEvent(Trig, frame, ConvertFrameEventType(i)) end
 BlzTriggerRegisterFrameEvent(Trig, frame, FRAMEEVENT_CONTROL_CLICK)
 BlzTriggerRegisterFrameEvent(Trig, frame, FRAMEEVENT_MOUSE_LEAVE) -- only reforged
 BlzTriggerRegisterFrameEvent(Trig, frame, FRAMEEVENT_MOUSE_ENTER) -- only reforged
 TriggerAddAction(Trig, function()
 print("frame action", GetHandleId(BlzGetTriggerFrameEvent()))
 end)
end

###
 SimpleFrame - Text without custom FDF

Example that Displays Text on screen for the SimpleFrame Family. As Far i know one cant create String Frame directly. Therefore we create a default loaded blueprint that is only simpleframe + the String: "SimpleInfoPanelDestructableDetail"

 Lua:

frame = BlzCreateSimpleFrame("SimpleInfoPanelDestructableDetail", BlzGetFrameByName("ConsoleUI", 0), 0)
textFrame = BlzGetFrameByName("SimpleDestructableNameValue", 0) -- this is the String Frame we want
BlzFrameClearAllPoints(textFrame) -- unbind the textFrame
BlzFrameSetAbsPoint(textFrame, FRAMEPOINT_CENTER, 0.4, 0.3) -- place it on center of screen
BlzFrameSetFont(textFrame, "fonts/frizqt__.ttf", 0.013,0) -- use custom fontsize SkinManagerGetLocalPath("InfoPanelTextFont")
BlzFrameSetText(textFrame, "My Custom SimpleFrame TExt")

BlzFrameSetVisible(frame, false) -- visible needs to be called on the simpleFrame Parent

###
 Slider without custom FDF

Vertical Slider with an Value changed Event. Inside the Event it displays the new selected Value as message. This works out of the box.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 -- create a vertical slider by inheriting from a Scrollbar. It will use esc menu textures
 local sliderFrame = BlzCreateFrameByType( "SLIDER", "TestSlider", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI,0), "QuestMainListScrollBar", 0 )
 -- clear the inherited attachment
 BlzFrameClearAllPoints(sliderFrame)
 -- set pos and size
 BlzFrameSetAbsPoint(sliderFrame, FRAMEPOINT_TOPLEFT, 0.40, 0.30 )
 BlzFrameSetSize(sliderFrame, 0.012, 0.06 )
 -- define the area the user can choose from
 BlzFrameSetMinMaxValue(sliderFrame, 0, 1000)
 -- how accurate the user can choose value
 BlzFrameSetStepSize(sliderFrame, 1)

 local trigger = CreateTrigger()

 -- register the Slider Event
 BlzTriggerRegisterFrameEvent(trigger, sliderFrame, FRAMEEVENT_SLIDER_VALUE_CHANGED)

 -- this happens when the Slider is pushed
 TriggerAddAction(trigger, function()
 local frame = BlzGetTriggerFrame()
 print(BlzFrameGetName(frame), "new Value", BlzGetTriggerFrameValue())
 end)
 end
end

FrameExampleSliderNoFDF
Events
Time - Elapsed game time is 1.00 seconds

Conditions

Actions
If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Execution count of (This trigger)) Equal to 1

Then - Actions
-------- Vertical Slider with an Value changed Event --------

Frame - Create Frame by Type: SLIDER name: TestSlider parent: GameUI inherits: QuestMainListScrollBar CreateContext: 0

Set Frame = (Get Frame By Name:TestSlider Context:0)

Frame - Clear all Points for: Frame

Frame - Set Size of Frame to 0.01/0.06

Frame - Set Absolute Point for: Frame place FRAMEPOINT_TOPLEFT to 0.40/0.55

Frame - Set Min/Max for Frame min: 0.00 max: 1000.00

Frame - Set Step Size for Frame to 1.00

Trigger - Add to (This trigger) the event (Frame - Frame Event: FRAMEEVENT_SLIDER_VALUE_CHANGED)

Else - Actions
Game - Display to (All players) the text: ((Get Name of Frame: (Get Triggering Frame)) + (( new Value + ((String((Get Triggering Frame Value), 1, 1)) + by )) + (Name of (Triggering player))))

Horizontal Slider with a TEXT-Frame to it's right displaying the current value. While the Mouse points at the slider rolling the Mouse wheel results into a changed Value, this requires to have imported Templates.toc with EscMenuTemplates.fdf in the map running this Lua code. TOC.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 -- load the Blizzard Templates
 local loaded = BlzLoadTOCFile("war3mapImported\\Templates.toc")
 -- create the SliderFrame
 local sliderFrame = BlzCreateFrame("EscMenuSliderTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 -- create a TEXT Fame
 local sliderLable = BlzCreateFrame("EscMenuLabelTextTemplate", sliderFrame, 0, 2)

 -- place the Label to the right of the Slider's
 BlzFrameSetPoint(sliderLable, FRAMEPOINT_LEFT, sliderFrame, FRAMEPOINT_RIGHT, 0, 0)
 -- pos the sliderFrame on the screen
 BlzFrameSetAbsPoint(sliderFrame, FRAMEPOINT_LEFT, 0.02, 0.45)

 -- limits user can choose
 BlzFrameSetMinMaxValue(sliderFrame, 0, 360)
 -- set current Value
 BlzFrameSetValue(sliderFrame, 90)

 BlzFrameSetText(sliderLable, " = 90")

 -- how accurate the user can setup the Slider, only multiplies of 5 right now, 5, 10, 15 ...
 BlzFrameSetStepSize(sliderFrame, 5)

 local trigger = CreateTrigger()
 -- register the Slider Event
 BlzTriggerRegisterFrameEvent(trigger, sliderFrame, FRAMEEVENT_SLIDER_VALUE_CHANGED)
 -- this happens when the Slider changes it's value, by user or code
 TriggerAddAction(trigger, function()
 -- only the value of the own Slider is known
 if GetLocalPlayer() == GetTriggerPlayer() then
 BlzFrameSetText(sliderLable, " = " .. R2I(BlzGetTriggerFrameValue()))
 end
 end)

 -- scorllable with mousewheel
 trigger = CreateTrigger()
 -- register the Mouse Wheel Event for the Slider
 BlzTriggerRegisterFrameEvent(trigger, sliderFrame, FRAMEEVENT_MOUSE_WHEEL)
 -- this happens when the Mouse wheel is rolled while it points at the slider
 TriggerAddAction(trigger, function()

 -- BlzGetTriggerFrameValue() tells us in which direction the wheel was rolled
 local add
 if BlzGetTriggerFrameValue() > 0 then
 add = 5
 else
 add = -5
 end

 -- the scrolling should only affect the triggering Player
 if GetLocalPlayer() == GetTriggerPlayer() then
 BlzFrameSetValue(sliderFrame, BlzFrameGetValue(sliderFrame) + add)
 end
 end)
 end
end

##
 Checkbox without FDF

Cause there is a default loaded Checkbox one can use that one to create a checkbox without bothering toc nor fdf as long one knows the name.

Possible ones are "QuestCheckBox", "QuestCheckBox2", "QuestCheckBox3". All of them use EscMenu Textures but have differnt Sizes. Although that does not matter much one could just change the size with BlzFrameSetSize

This Lua code works out of the Box and will create a small Checkbox at the center of the screen.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()

 local frame = BlzCreateFrame("QuestCheckBox2", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 local trigger = CreateTrigger()
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_CENTER, 0.4, 0.3)

 BlzTriggerRegisterFrameEvent(trigger, frame, FRAMEEVENT_CHECKBOX_CHECKED)
 BlzTriggerRegisterFrameEvent(trigger, frame, FRAMEEVENT_CHECKBOX_UNCHECKED)
 TriggerAddAction(trigger, function()
 if BlzGetTriggerFrameEvent() == FRAMEEVENT_CHECKBOX_CHECKED then
 print("CheckBox was checked")
 else
 print("CheckBox was unchecked")
 end
 end)
 end
end

FrameExampleCheckBoxNoFDF
Events
Time - Elapsed game time is 1.00 seconds

Conditions

Actions
-------- Vertical Slider with an Value changed Event --------

Frame - Create Frame name: QuestCheckBox2 parent: GameUI Priority: 0 CreateContext: 0

Set Frame = (Get Frame By Name:QuestCheckBox2 Context:0)

Frame - Set Absolute Point for: Frame place FRAMEPOINT_TOPLEFT to 0.55/0.55

Trigger - Add to FrameExampleCheckBoxNoFDFCheck <gen> the event (Frame - Frame Event: FRAMEEVENT_CHECKBOX_CHECKED)

Trigger - Add to FrameExampleCheckBoxNoFDFUnCheck <gen> the event (Frame - Frame Event: FRAMEEVENT_CHECKBOX_UNCHECKED)

FrameExampleCheckBoxNoFDFCheck
Events

Conditions

Actions
Game - Display to (All players) the text: ((Get Name of Frame: (Get Triggering Frame)) + ( was checked + (Name of (Triggering player))))

FrameExampleCheckBoxNoFDFUnCheck
Events

Conditions

Actions
Game - Display to (All players) the text: ((Get Name of Frame: (Get Triggering Frame)) + ( was unchecked + (Name of (Triggering player))))

There also 2 other checkboxes one could use.

"ScoreScreenBottomCheckButtonTemplate" is only a mouse hover highlight and a checked Texture both a yellow light. On default loaded. Could be used together with a a BACKDROP.

And there is "EscMenuRadioButtonTemplate" not loaded on default one would have to Load UI\FrameDef\UI\escmenutemplates.fdf into the game before using it.

##
 ModelBar without FDF

This example shows how one can display a Bar Model using "STATUSBAR" without any fdf.

It is said "STATUSBAR" only supports 1s animations, sequzenzes after 1s are droped.

TriggerHappy encountered a situation in which units using the same model as the STATUSBAR were affected by the Bar's current animation.

 Lua:

do
 local real = MarkGameStarted

 function MarkGameStarted()
 real()
 local bar = BlzCreateFrameByType("STATUSBAR", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
 BlzFrameSetAbsPoint(bar, FRAMEPOINT_TOPLEFT, 0.4, 0.3)
 -- Screen Size does not matter but has to be there
 BlzFrameSetSize(bar, 0.00001, 0.00001)

 -- Models don't care about Frame Size, But world Object Models are huge . To use them in the UI one has to scale them down alot.
 BlzFrameSetScale(bar, 1)

 --BlzFrameSetModel(bar, "ui/feedback/cooldown/ui-cooldown-indicator.mdx", 0)
 --BlzFrameSetModel(bar, "ui/feedback/XpBar/XpBarConsole.mdx", 0)
 BlzFrameSetModel(bar, "ui/feedback/progressbar/timerbar.mdx", 0)
 --BlzFrameSetModel(bar, "ui/feedback/buildprogressbar/buildprogressbar.mdx", 0)
 --BlzFrameSetMinMaxValue(bar, 0, 100)
 local i = 0
 TimerStart(CreateTimer(), 0.1, true, function()
 BlzFrameSetValue(bar, i)
 i = i + 1
 --i = GetRandomInt(0, 100)
 print(BlzFrameGetValue(bar))
 end)

 print("done")
 end
end

FrameExampleModelBarNoFdf
Events
Time - Elapsed game time is 1.00 seconds

Conditions

Actions
-------- ModelBar --------

Frame - Create Frame by Type: STATUSBAR name: MyModelBar parent: GameUI inherits: <Leerer String> CreateContext: 0

Set Frame = (Get Frame By Name:MyModelBar Context:0)

Frame - Set Absolute Point for: Frame place FRAMEPOINT_TOPLEFT to 0.30/0.26

-------- Screen Size does not matter but has to be there --------

Frame - Set Size of Frame to 0.01/0.01

Frame - Set Scale of Frame to 1.00

Frame - Set Model Frame:Frame Model: ui/feedback/progressbar/timerbar.mdx Camera: 0

FrameExampleModelBarNoFdfTimer
Events
Time - Every 0.10 seconds of game time

Conditions

Actions
-------- ModelBar --------

Set Frame = (Get Frame By Name:MyModelBar Context:0)

If (All Conditions are True) then do (Then Actions) else do (Else Actions)
If - Conditions
(Get Value of Frame: Frame) Greater than or equal to 100.00

Then - Actions
Frame - Set Value of Frame to 0.00

Else - Actions

Frame - Set Value of Frame to ((Get Value of Frame: Frame) + 1.00)

##
 2-Face without FDF

One could Create&Place 2 SIMPLESTATUSBARs at the same spot and using different Textures to show progress/cooldown and alike. To change the current progress one uses BlzFrameSetValue onto the ForeGround to shows Value% from the left Side of it and 100-value% of the background from the right side.

[attachment 386704](https://www.hiveworkshop.com/attachments/arthasface2-gif.386704/)[attachment 386706](https://www.hiveworkshop.com/attachments/2faceblizzard-jpg.386706/)

 Lua:

function Create2Face(parent, xSize, ySize, createContext)
 local twoFace = BlzCreateFrameByType("SIMPLEFRAME", "2Face", parent, "", createContext)
 -- creating SIMPLEFRAME that way Sets a Point(s) for it, clear them
 BlzFrameClearAllPoints(twoFace)

 local backGround = BlzCreateFrameByType("SIMPLESTATUSBAR", "2FaceBackGround", twoFace, "", createContext)
 local foreGround = BlzCreateFrameByType("SIMPLESTATUSBAR", "2FaceForeGround", backGround, "", createContext)
 -- creating SIMPLESTATUSBAR that way Sets a Point(s) for it, clear them
 BlzFrameClearAllPoints(foreGround)
 BlzFrameClearAllPoints(backGround)
 -- mimic Size/Pos
 BlzFrameSetAllPoints(foreGround, twoFace)
 BlzFrameSetAllPoints(backGround, twoFace)

 BlzFrameSetSize(twoFace, xSize, ySize)
 BlzFrameSetValue(backGround, 100)
 -- default Textures
 BlzFrameSetTexture(backGround, "Replaceabletextures\\CommandButtons\\BTNTemp", 0, false)
 BlzFrameSetTexture(foreGround, "Replaceabletextures\\CommandButtons\\BTNTemp", 0, false)
 return twoFace
end

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 Create2Face(BlzGetFrameByName("ConsoleUI", 0), 0.04, 0.04, 0)
 BlzFrameSetAbsPoint(BlzGetFrameByName("2Face", 0), FRAMEPOINT_CENTER, 0.4, 0.3)

 -- Arthas/Undead-Arthas
 BlzFrameSetTexture(BlzGetFrameByName("2FaceForeGround", 0), "Replaceabletextures\\CommandButtons\\BTNArthas", 0, false)
 BlzFrameSetTexture(BlzGetFrameByName("2FaceBackGround", 0), "Replaceabletextures\\CommandButtons\\BTNHeroDeathKnight", 0, false)

 -- Blizzard/ Disabled-Blizzard
 BlzFrameSetTexture(BlzGetFrameByName("2FaceBackGround", 0), "Replaceabletextures\\CommandButtonsDisabled\\DISBTNBlizzard", 0, false)
 BlzFrameSetTexture(BlzGetFrameByName("2FaceForeGround", 0), "Replaceabletextures\\CommandButtons\\BTNBlizzard", 0, false)

 -- Randomize the value
 TimerStart(CreateTimer(), 0.2, true, function()
 BlzFrameSetValue(BlzGetFrameByName("2FaceForeGround", 0), BlzFrameGetValue(BlzGetFrameByName("2FaceForeGround", 0)) + GetRandomInt(-3, 3))
 end)
 end
end

##
 Bar without FDF

This example shows how one can create a SIMPLESTATUSBAR without any fdf and place a BACKDROP Layerwise below it. All SimpleFrames are in one Layer or in subLayers which only Texture/String can inhabit. They can not create own Layers.

Hence we have to pick a Layer for the BACKDROP so it ends lower than the SIMPLESTATUSBAR(SimpleFrames). Good FrameParents/Ancestors for that are BlzGetFrameByName("ConsoleUIBackdrop", 0) or BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0) or BlzFrameGetParent(BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT, 0)) basicly any Child of Gameui before the one that starts SimpleFrames.

BlzGetFrameByName("ConsoleUIBackdrop", 0) is the best of them because the others are below HP/Mp bars, but it exist not in Warcraft3 V1.31.1.

Creating Bars that way has the downside that the Bar and its Background are not interal connected. One needs to use hide/show/Scale/Alpha/... one each. But the + is one does not have to bother fdf.

 Lua:

do
 local real = MarkGameStarted

 function MarkGameStarted()
 real()
 BlzLoadTOCFile("war3mapImported\\Templates.toc")
 local back = BlzCreateFrame("EscMenuControlBackdropTemplate", BlzGetFrameByName("ConsoleUIBackdrop", 0), 0, 0)
 BlzFrameSetAbsPoint(back, FRAMEPOINT_CENTER, 0.4, 0.3)
 BlzFrameSetSize(back, 0.1, 0.03)
 local bar = BlzCreateFrameByType("SIMPLESTATUSBAR", "", BlzGetFrameByName("ConsoleUI", 0), "", 0)
 --BlzFrameSetAllPoints(bar, back)
 BlzFrameSetPoint(bar, FRAMEPOINT_TOPLEFT, back, FRAMEPOINT_TOPLEFT, 0.006, -0.006)
 BlzFrameSetPoint(bar, FRAMEPOINT_BOTTOMRIGHT, back, FRAMEPOINT_BOTTOMRIGHT, -0.006, 0.006)
 BlzFrameSetTexture(bar, "ReplaceableTextures\\CommandButtons\\BTNPriest", 0, false)
 BlzFrameSetVertexColor(bar, BlzConvertColor(255, 255, 100, 255))
 BlzFrameSetVertexColor(back, BlzConvertColor(255, 255, 100, 255))
 --BlzFrameSetMinMaxValue(frame, 0, 100)
 local i = 0
 TimerStart(CreateTimer(), 0.1, true, function()
 BlzFrameSetValue(bar, i)
 i = i + 1
 end)

 end
end

Instead of the Priest Icon one could use a Texture more fited for Bars like the one from the xp-Bar.

BlzFrameSetTexture(bar, "ui\\feedback\\xpbar\\human-bigbar-fill", 0, true)

Because it is a gray texture it might be smart to color it which can be done with 2 natives (Crash in Warcraft3 V1.31)

BlzFrameSetVertexColor(bar, BlzConvertColor(255, 255, 100, 255))

Instead of such coloring one could use the teamcolor textures for easy simple one colored bars.

 Lua:

BlzFrameSetTexture(bar, "replaceabletextures\\teamcolor\\teamcolor00", 0, false)
BlzFrameSetTexture(bar, "replaceabletextures\\teamcolor\\teamcolor01", 0, false)
BlzFrameSetTexture(bar, "replaceabletextures\\teamcolor\\teamcolor02", 0, false)
BlzFrameSetTexture(bar, "replaceabletextures\\teamcolor\\teamcolor09", 0, false)
BlzFrameSetTexture(bar, "replaceabletextures\\teamcolor\\teamcolor10", 0, false)
BlzFrameSetTexture(bar, "replaceabletextures\\teamcolor\\teamcolor20", 0, false)
BlzFrameSetTexture(bar, "replaceabletextures\\teamcolor\\teamcolor27", 0, false)

Sometimes one wants the colorTexture used by a player, that can be done with some simple string work.

 Lua:

function GetPlayerColorTexture(player)
 if GetHandleId(GetPlayerColor(player)) < 10 then
 return "ReplaceableTextures\\TeamColor\\TeamColor0"..GetHandleId(GetPlayerColor(player))
 else
 return "ReplaceableTextures\\TeamColor\\TeamColor"..GetHandleId(GetPlayerColor(player))
 end
end

###
 EDITBOX without custom FDF

One could create a EditBox after one Loaded in the Templates.toc. Although one would not be able to change the text size inside the Editbox.

###
 Create a Textarea in QuestDialog

In this example I show you how one creates a Textarea as child of QuestDialog. I made this for someone on Discord but I think it would be a nice addition because it shows the interaction with existing Frames and is compact.

The Textarea is created as child of the QuestDialog so it is only shown when QuestDialog is shown. It takes the same place as the quest info text, displays the quest text. Because they take the same place it would not be smart to show both at the same Time, they would overlap and make it confusing. But we only have limited Space and want to stay in the visual QuestDialog, hence we make it so that only one of them is shown and we add a button to swap which is shown right now.

This example requires loading in ui\framedef\ui\escmenutemplates.fdf contained in Templates.toc to create the TEXTAREA. The whole thing would work with a BACKDROP which would remove the requirement.

This is the code doing this.

 JASS:

function QuestLogFrameAction takes nothing returns nothing
 // do this only for the clicking player
 if GetLocalPlayer() == GetTriggerPlayer() then
 // toggle visibility for ("QuestDisplayBackdrop", 0)
 call BlzFrameSetVisible(BlzGetFrameByName("QuestDisplayBackdrop", 0), not BlzFrameIsVisible(BlzGetFrameByName("QuestDisplayBackdrop", 0)))
 // ("CustomLogFrame", 0) is shown when the other is not shown
 call BlzFrameSetVisible(BlzGetFrameByName("CustomLogFrame", 0), not BlzFrameIsVisible(BlzGetFrameByName("QuestDisplayBackdrop", 0)))
 endif
endfunction
function CreateQuestLogFrame takes nothing returns nothing
 local framehandle textArea
 local framehandle buttonX
 local trigger tri
 // load in ui\framedef\ui\escmenutemplates.fdf
 call BlzLoadTOCFile("war3mapImported\\Templates.toc")
 // Some Frames do not exist until required. QuestDialog is such one. Force open it to bring it into existince.
 call BlzFrameClick(BlzGetFrameByName("UpperButtonBarQuestsButton", 0))
 // force closing QuestDialog
 call BlzFrameClick(BlzGetFrameByName("QuestAcceptButton", 0))
 // Create a TEXTAREA based on "EscMenuTextAreaTemplate" with the new Name "CustomLogFrame" as child of ("QuestDialog", 0)
 set textArea = BlzCreateFrameByType("TEXTAREA", "CustomLogFrame", BlzGetFrameByName("QuestDialog", 0), "EscMenuTextAreaTemplate",0)
 // copy the positions of ("QuestDisplayBackdrop", 0)
 call BlzFrameSetAllPoints(textArea, BlzGetFrameByName("QuestDisplayBackdrop", 0))

 // text
 call BlzFrameSetText(textArea, "This is the customLogFrame")
 // hide it on default
 call BlzFrameSetVisible(textArea, false)

 // Create a button at the bottom left of the QuestDialog. It will swap the current shown Frame
 set buttonX = BlzCreateFrame("ScriptDialogButton", BlzGetFrameByName("QuestDialog", 0),0,0)
 call BlzFrameSetPoint(buttonX, FRAMEPOINT_TOPLEFT, BlzGetFrameByName("QuestDisplayBackdrop", 0), FRAMEPOINT_BOTTOMLEFT, 0, 0)
 call BlzFrameSetSize(buttonX, 0.1, BlzFrameGetHeight(buttonX))
 call BlzFrameSetText(buttonX, "Show/Hide Log")
 // create the buttons actionTrigger
 set tri = CreateTrigger()
 call BlzTriggerRegisterFrameEvent(tri, buttonX, FRAMEEVENT_CONTROL_CLICK)
 call TriggerAddAction(tri, function QuestLogFrameAction)

 set buttonX = null
 set textArea = null
 set tri = null
endfunction

##
 Create A Frame that is always Left of Screen

This example shows how one creates 2 Frames that can leave 4:3 and that frames will also be always on the maximum left of the screen even when the screen resolution changes.

 Lua:

do
 -- provides the parent that can Leave 4:3 and with that our frames
 local function GetParent()
 -- try to use "ConsoleUIBackdrop"
 local parent = BlzGetFrameByName("ConsoleUIBackdrop", 0)
 if GetHandleId(parent) == 0 then
 -- it failed "ConsoleUIBackdrop" can not be used

 -- therefore use a Multiboard
 parent = BlzGetFrameByName("Multiboard", 0)
 if GetHandleId(parent) ~= 0 then return parent end

 -- therefore use a Leaderboard
 -- try attaching to a existing one
 parent = BlzGetFrameByName("Leaderboard", 0)
 if GetHandleId(parent) == 0 then
 -- create one and make it not seeable
 CreateLeaderboardBJ(bj_FORCE_ALL_PLAYERS, "title")
 parent = BlzGetFrameByName("Leaderboard", 0)
 BlzFrameSetSize(parent, 0, 0)
 BlzFrameSetVisible(BlzGetFrameByName("LeaderboardBackdrop", 0), false)
 BlzFrameSetVisible(BlzGetFrameByName("LeaderboardTitle", 0), false)
 end
 end
 return parent
 end

 local function Update()
 -- update the full screen frame to current Resolution
 BlzFrameSetSize(FullScreenFrame, BlzGetLocalClientWidth()/BlzGetLocalClientHeight()*0.6, 0.6)
 end

 local function InitFullScreenFrames()
 -- Make the FullScreenFrames
 -- to allow FullScreenParent to expand over 4:3 it needs a Parent that can do such GetParent() gives us that one
 local frame = BlzCreateFrameByType("FRAME", "FullScreenParent", GetParent(), "", 0)
 FullScreenParent = frame

 -- Lets make another Frame which size is the whole screen
 -- it is hidden to not take control and dont have visuals.
 -- as child of FullScreenParent it can expand outside of 4:3
 frame = BlzCreateFrameByType("FRAME", "FullScreenFrame", frame, "", 0)
 BlzFrameSetVisible(frame, false)
 BlzFrameSetSize(frame, 0.8, 0.6)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_BOTTOM, 0.4, 0)
 FullScreenFrame = frame
 end

 function CreateDemoFrames()
 local parentFrame = BlzCreateFrameByType("FRAME", "TestFRAME", FullScreenParent, "", 0 )

 -- create a vertical Slider that is a child of FullScreenParent to allow it to leave 4:3
 local sliderFrame = BlzCreateFrameByType( "SLIDER", "TestSlider", parentFrame, "QuestMainListScrollBar", 0 )
 BlzFrameClearAllPoints(sliderFrame)
 BlzFrameSetSize(sliderFrame, 0.012, 0.06 )
 BlzFrameSetMinMaxValue(sliderFrame, 0, 1000)
 -- Attach sliderFrame to FullScreenFrame
 BlzFrameSetPoint(sliderFrame, FRAMEPOINT_TOPLEFT, FullScreenFrame, FRAMEPOINT_TOPLEFT, 0.005, -0.2)

 local buttonFrame = BlzCreateFrame("ScriptDialogButton", parentFrame,0,0)
 BlzFrameClearAllPoints(buttonFrame)
 BlzFrameSetSize(buttonFrame, 0.12, 0.035)
 BlzFrameSetPoint(buttonFrame, FRAMEPOINT_TOPLEFT, sliderFrame, FRAMEPOINT_BOTTOM, 0, 0)
 end

 -- this would be an outside thing that calls the others
 function MasterInit()
 InitFullScreenFrames()
 CreateDemoFrames()
 if FrameLoaderAdd then FrameLoaderAdd(InitFullScreenFrames) end
 if FrameLoaderAdd then FrameLoaderAdd(CreateDemoFrames) end
 TimerStart(CreateTimer(), 0.5, true, Update)
 end
 --xpcall(MasterInit, print)
end

There is another approach for the always left frame, if one did not BlzHideOriginFrames(true)/BlzEnableUIAutoPosition(false) then one can use a position of (ORIGIN_FRAME_HERO_BUTTON, 0). This does not require a timer and works while the game is paused.

BlzFrameSetPoint(frame, FRAMEPOINT_TOPLEFT, BlzGetOriginFrame(ORIGIN_FRAME_HERO_BUTTON, 0), FRAMEPOINT_TOPLEFT, 0, 0)

###
 Creating a Multipage Frame

The previous examples are quite basic and show one type of Frame while that is quite useful it does not show the interaction of multiple Frames and does not show much of the parent-child rule. This example is more complex with multiple Frame Types, events and shows a way to create a hardcoded hide able Box with 3 pages each with different content. The pages are changed by clicking a page button at the bottom.

The first Page shows infos about Uther the second a col of Buttons creating an unit and the 3. a simple calculator that + - * / for 2 changeable numbers

This example is done without any custom fdf but requires loading escmenutemplates.fdf. It also shows how bad UI-Frame code can end up.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 -- allow to generate Frames from ui\framedef\ui\escmenutemplates.fdf
 BlzLoadTOCFile("war3mapImported\\Templates.toc")
 -- create a hidden Frame a container for all
 local windowcontainerFrame = BlzCreateFrameByType("FRAME", "", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
 -- create a box as child of the container
 local boxFrame = BlzCreateFrameByType("BACKDROP", "", windowcontainerFrame, "EscMenuBackdrop", 0)
 BlzFrameSetSize(boxFrame, 0.4, 0.4)
 BlzFrameSetAbsPoint(boxFrame, FRAMEPOINT_CENTER, 0.4, 0.3)

 -- The option to close (hide) the box
 local closeButton = BlzCreateFrameByType("GLUETEXTBUTTON", "", boxFrame, "ScriptDialogButton", 0)
 BlzFrameSetSize(closeButton, 0.03, 0.03)
 BlzFrameSetText(closeButton, "X")
 BlzFrameSetPoint(closeButton, FRAMEPOINT_TOPRIGHT, boxFrame, FRAMEPOINT_TOPRIGHT, 0, 0)
 -- this trigger handles clicking the close Button, it hides the Logical super Parent when the close Button is clicked for the clicking Player.
 local closeTrigger = CreateTrigger()
 BlzTriggerRegisterFrameEvent(closeTrigger, closeButton, FRAMEEVENT_CONTROL_CLICK)
 TriggerAddAction(closeTrigger, function()
 if GetLocalPlayer() == GetTriggerPlayer() then
 BlzFrameSetVisible(windowcontainerFrame, false)
 end
 end)

 -- Because one can close (hide) the box, one also should be able to show it again, this is done with an button that is only visible while the player is in Menu (F10)
 local showButton = BlzCreateFrameByType("GLUETEXTBUTTON", "", BlzGetFrameByName("InsideMainPanel",0), "ScriptDialogButton", 0)
 BlzFrameSetSize(showButton, 0.08, 0.04)
 BlzFrameSetText(showButton, "show Info Frame")
 BlzFrameSetAbsPoint(showButton, FRAMEPOINT_BOTTOMLEFT, 0, 0.2)

 local showTrigger = CreateTrigger()
 BlzTriggerRegisterFrameEvent(showTrigger, showButton, FRAMEEVENT_CONTROL_CLICK)
 TriggerAddAction(showTrigger, function()
 if GetLocalPlayer() == GetTriggerPlayer() then
 BlzFrameSetVisible(windowcontainerFrame, true)
 end
 end)

 -- create 3 Buttons based on ScriptDialogButton
 local buttonPage1 = BlzCreateFrameByType("GLUETEXTBUTTON", "", boxFrame, "ScriptDialogButton", 0)
 local buttonPage2 = BlzCreateFrameByType("GLUETEXTBUTTON", "", boxFrame, "ScriptDialogButton", 0)
 local buttonPage3 = BlzCreateFrameByType("GLUETEXTBUTTON", "", boxFrame, "ScriptDialogButton", 0)

 BlzFrameSetSize(buttonPage1, 0.08, 0.03)
 BlzFrameSetSize(buttonPage2, 0.08, 0.03)
 BlzFrameSetSize(buttonPage3, 0.08, 0.03)

 BlzFrameSetText(buttonPage1, "Page 1")
 BlzFrameSetText(buttonPage2, "Page 2")
 BlzFrameSetText(buttonPage3, "Page 3")

 -- pos the 2.Button at the bottom
 BlzFrameSetPoint(buttonPage2, FRAMEPOINT_BOTTOM, boxFrame, FRAMEPOINT_BOTTOM, 0, 0.017)
 -- the 3. Button to it's right
 BlzFrameSetPoint(buttonPage3, FRAMEPOINT_LEFT, buttonPage2, FRAMEPOINT_RIGHT, 0, 0)
 -- the 1. Button to it's left
 BlzFrameSetPoint(buttonPage1, FRAMEPOINT_RIGHT, buttonPage2, FRAMEPOINT_LEFT, 0, 0)
 -- Now we got the button in order: 1 2 3, but don't have to bother the real position and sizes

 -- create a Trigger handling the Page Button Clicks
 local pageTrigger = CreateTrigger()
 BlzTriggerRegisterFrameEvent(pageTrigger, buttonPage1, FRAMEEVENT_CONTROL_CLICK)
 BlzTriggerRegisterFrameEvent(pageTrigger, buttonPage2, FRAMEEVENT_CONTROL_CLICK)
 BlzTriggerRegisterFrameEvent(pageTrigger, buttonPage3, FRAMEEVENT_CONTROL_CLICK)

 -- create a container Frame for each Page, this frames are mostly logical. They simple down the page swapping a lot.
 local containerFramePage1 = BlzCreateFrameByType("FRAME", "", boxFrame, "", 0)
 local containerFramePage2 = BlzCreateFrameByType("FRAME", "", boxFrame, "", 0)
 local containerFramePage3 = BlzCreateFrameByType("FRAME", "", boxFrame, "", 0)
 -- the containers are a bit smaller than the box to avoid colisions with the border and the buttons at the bottom
 -- the containers are given a size to place contentFrames relative to the containers. This allows to place it to any valid coords and it will still work as wanted.
 BlzFrameSetSize(containerFramePage1, 0.36, 0.3)
 BlzFrameSetSize(containerFramePage2, 0.36, 0.3)
 BlzFrameSetSize(containerFramePage3, 0.36, 0.3)
 -- place the containers to the center of the box
 BlzFrameSetPoint(containerFramePage1, FRAMEPOINT_TOP, boxFrame, FRAMEPOINT_TOP, 0, -0.03)
 BlzFrameSetPoint(containerFramePage2, FRAMEPOINT_TOP, boxFrame, FRAMEPOINT_TOP, 0, -0.03)
 BlzFrameSetPoint(containerFramePage3, FRAMEPOINT_TOP, boxFrame, FRAMEPOINT_TOP, 0, -0.03)

 -- hide the page containers on default
 BlzFrameSetVisible(containerFramePage1, false)
 BlzFrameSetVisible(containerFramePage2, false)
 BlzFrameSetVisible(containerFramePage3, false)

 TriggerAddAction(pageTrigger, function()
 local clickedButton = BlzGetTriggerFrame()
 -- only the active player should be affected
 if GetLocalPlayer() == GetTriggerPlayer() then
 -- hide all pages, could be optimized monitoring the current selected but won't do that in this example.
 BlzFrameSetVisible(containerFramePage1, false)
 BlzFrameSetVisible(containerFramePage2, false)
 BlzFrameSetVisible(containerFramePage3, false)

 -- show the page based on the clicked button.
 if clickedButton == buttonPage1 then
 BlzFrameSetVisible(containerFramePage1, true)
 elseif clickedButton == buttonPage2 then
 BlzFrameSetVisible(containerFramePage2, true)
 elseif clickedButton == buttonPage3 then
 BlzFrameSetVisible(containerFramePage3, true)
 end
 end
 end)

 -- Page 1 Content
 -- A page that shows some text about uther taken from https://wow.gamepedia.com/Uther_the_Lightbringer
 local parent = containerFramePage1
 local frame = BlzCreateFrameByType("BACKDROP", "", parent, "", 0)
 BlzFrameSetSize(frame, 0.04, 0.04)
 BlzFrameSetPoint(frame, FRAMEPOINT_TOPLEFT, parent, FRAMEPOINT_TOPLEFT, 0, 0)
 BlzFrameSetTexture(frame, "ReplaceableTextures\\CommandButtons\\BTNHeroPaladin", 0, false)

 frame = BlzCreateFrameByType("TEXT", "", parent, "", 0)
 BlzFrameSetPoint(frame, FRAMEPOINT_TOPRIGHT, parent, FRAMEPOINT_TOPRIGHT, 0, 0)
 BlzFrameSetText(frame, "Paladin of the Silver Hand")
 BlzFrameSetScale(frame, 1.2)

 frame = BlzCreateFrameByType("TEXT", "", parent, "", 0)
 BlzFrameSetPoint(frame, FRAMEPOINT_TOPRIGHT, parent, FRAMEPOINT_TOPRIGHT, 0, -0.02)
 BlzFrameSetText(frame, "Uther")
 BlzFrameSetScale(frame, 1.3)

 frame = BlzCreateFrameByType("TEXTAREA", "", parent, "EscMenuTextAreaTemplate", 0)
 BlzFrameSetPoint(frame, FRAMEPOINT_BOTTOMRIGHT, parent, FRAMEPOINT_BOTTOMRIGHT, 0, 0)
 BlzFrameSetPoint(frame, FRAMEPOINT_TOPLEFT, parent, FRAMEPOINT_TOPLEFT, 0, -0.05)
 BlzFrameSetText(frame, "Lord Uther the Lightbringer, or Sire Uther Lightbringer, was the first of the five paladins of the Knights of the Silver Hand along with Turalyon, Saidan Dathrohan, Tirion Fordring, and Gavinrad the Dire. He led his order in the battle against the Horde during the Second War. During the Third War, Uther was betrayed and murdered by his beloved pupil, Prince Arthas, while defending the urn carrying the ashes of Arthas' father, King Terenas. After death, his soul was deemed worthy of entering the plane of Bastion within the Shadowlands. ")
 BlzFrameAddText(frame, "|cffffcc00Personality|r|nThough zealous and weathered, Uther's eyes show kindness and wisdom. He is Lordaeron's self-appointed defender, but regrets that violence is the only way to solve some problems. Possessing a rich, commanding voice and great physical strength, Uther is also capable of gentleness and compassion, though he does not suffer fools. He is the epitome of the paladin warrior — a mighty foe to his enemies and a bastion of hope to his allies.")
 BlzFrameAddText(frame, "|cffffcc00In combat|r|nUther strides directly into melee, placing himself in the center of the most brutal combat. He places himself in danger to spare his allies. He is at his peak against demons and undead, and brings his full array of spells and abilities to bear against these creatures — smites, banishing strikes, power turning, searing light from his hammer, hooks of binding and dispel evil. He uses lay on hands to blast undead that resist his hammer. Against truly mighty opponents, Uther attacks with his Big Smash feat. He prefers leading others into battle but fights alone if the situation warrants. Uther endangers himself to help others if he must, and is willing to sacrifice himself for others — but he does not do so foolishly, as he knows how valuable he is to Lordaeron")
 BlzFrameAddText(frame, "|cffffcc00Equipment|r")
 BlzFrameAddText(frame, "|cffffcc00Hammer of the Lightbringer|r|nThe two-handed hammer’s haft is polished mahogany, while the head is adamantine. A silver hand emblem rests in a bed of gold design on either side. This mighty weapon was forged when Archbishop Faol created the Knights of the Silver Hand, and the archbishop bequeathed it to the order’s first Grand Master — Uther the Lightbringer. A group of paladins recovered the hammer after Uther's death, but none has thought himself worthy of carrying the legendary weapon.")
 BlzFrameAddText(frame, "|cffffcc00Gloves of the Silver Hand|r|nUther Lightbringer is said to have been the first to enchant these gloves to aid him in the battle against the Scourge. These are large, padded leather and mail gloves bleached pure white with the holy symbols of the Silver Hand burned into the palms. Although they are large mail items, they are remarkably light.")
 BlzFrameAddText(frame, "Although the original shroud that covered the fallen paladin Uther Lightbringer of the Knights of the Silver Hand was lost years ago in frequent skirmishes between the Scourge and the Alliance, rumors of the shroud remain. Some of the remaining priests of the Holy Light have infused linen with power in honor of their fallen champion. These shrouds are made of soft, white linen, about 6 feet by 3 feet. The divine magic used to create these creates a gray image of a dead paladin, usually the face of the creator of the shroud.")
 BlzFrameAddText(frame, "Uther is universally known as the Lightbringer but he hasn't been always addressed as such. In fact, it was General Turalyon who got the idea of this nickname after seeing the inspiration that the leader of the Silver Hand had on his men. When he was charged by Khadgar and Uther as Supreme Commander of the Alliance he responded: \"And I thank you, Uther the Lightbringer, \" Turalyon replied, and he saw the older Paladin's eyes widen at the new title. \"For so shall you be known henceforth, in honor of the Holy Light you brought us this day.\" Uther bowed, clearly pleased, then turned without another word and walked back toward the other knights of the Silver Hand, no doubt to tell them their marching orders.")

 -- Page 2 Content
 -- a col of custom Buttons this Page is bad done one actually should define a function to prevent the repeating same lines, but I won't do that in this example.
 parent = containerFramePage2
 local buttonTrigger = CreateTrigger()
 local buttonData = {}
 TriggerAddAction(buttonTrigger, function()
 CreateUnit(GetTriggerPlayer(), buttonData[BlzGetTriggerFrame()], 0, 0, 0)
 end)

 local prevFrame, button, icon, text, unitId
 unitId = FourCC("Hpal")
 button = BlzCreateFrameByType("BUTTON", "", parent, "IconButtonTemplate", 0)
 icon = BlzCreateFrameByType("BACKDROP", "", button, "", 0)
 text = BlzCreateFrameByType("TEXT", "", button, "", 0)
 buttonData[button] = unitId
 BlzTriggerRegisterFrameEvent(buttonTrigger, button, FRAMEEVENT_CONTROL_CLICK)
 BlzFrameSetEnable(text, false)
 BlzFrameSetSize(button, 0.2, 0.05)
 BlzFrameSetSize(icon, 0.04, 0.04)
 BlzFrameSetPoint(icon, FRAMEPOINT_LEFT, button, FRAMEPOINT_LEFT, 0.01, 0)
 BlzFrameSetPoint(text, FRAMEPOINT_LEFT, icon, FRAMEPOINT_RIGHT, 0.01, 0)
 BlzFrameSetPoint(text, FRAMEPOINT_RIGHT, button, FRAMEPOINT_RIGHT, 0, 0)
 BlzFrameSetTexture(icon, BlzGetAbilityIcon(unitId), 0, false)
 BlzFrameSetText(text, GetObjectName(unitId))
 BlzFrameSetPoint(button, FRAMEPOINT_TOP, parent, FRAMEPOINT_TOP, 0, 0)
 prevFrame = button

 unitId = FourCC("Hamg")
 button = BlzCreateFrameByType("BUTTON", "", parent, "IconButtonTemplate", 0)
 icon = BlzCreateFrameByType("BACKDROP", "", button, "", 0)
 text = BlzCreateFrameByType("TEXT", "", button, "", 0)
 buttonData[button] = unitId
 BlzTriggerRegisterFrameEvent(buttonTrigger, button, FRAMEEVENT_CONTROL_CLICK)
 BlzFrameSetEnable(text, false)
 BlzFrameSetSize(button, 0.2, 0.05)
 BlzFrameSetSize(icon, 0.04, 0.04)
 BlzFrameSetPoint(icon, FRAMEPOINT_LEFT, button, FRAMEPOINT_LEFT, 0.01, 0)
 BlzFrameSetPoint(text, FRAMEPOINT_LEFT, icon, FRAMEPOINT_RIGHT, 0.01, 0)
 BlzFrameSetPoint(text, FRAMEPOINT_RIGHT, button, FRAMEPOINT_RIGHT, 0, 0)
 BlzFrameSetTexture(icon, BlzGetAbilityIcon(unitId), 0, false)
 BlzFrameSetText(text, GetObjectName(unitId))
 BlzFrameSetPoint(button, FRAMEPOINT_TOP, prevFrame, FRAMEPOINT_BOTTOM, 0, 0)
 prevFrame = button

 unitId = FourCC("Hmkg")
 button = BlzCreateFrameByType("BUTTON", "", parent, "IconButtonTemplate", 0)
 icon = BlzCreateFrameByType("BACKDROP", "", button, "", 0)
 text = BlzCreateFrameByType("TEXT", "", button, "", 0)
 buttonData[button] = unitId
 BlzTriggerRegisterFrameEvent(buttonTrigger, button, FRAMEEVENT_CONTROL_CLICK)
 BlzFrameSetEnable(text, false)
 BlzFrameSetSize(button, 0.2, 0.05)
 BlzFrameSetSize(icon, 0.04, 0.04)
 BlzFrameSetPoint(icon, FRAMEPOINT_LEFT, button, FRAMEPOINT_LEFT, 0.01, 0)
 BlzFrameSetPoint(text, FRAMEPOINT_LEFT, icon, FRAMEPOINT_RIGHT, 0.01, 0)
 BlzFrameSetPoint(text, FRAMEPOINT_RIGHT, button, FRAMEPOINT_RIGHT, 0, 0)
 BlzFrameSetTexture(icon, BlzGetAbilityIcon(unitId), 0, false)
 BlzFrameSetText(text, GetObjectName(unitId))
 BlzFrameSetPoint(button, FRAMEPOINT_TOP, prevFrame, FRAMEPOINT_BOTTOM, 0, 0)
 prevFrame = button

 unitId = FourCC("Hblm")
 button = BlzCreateFrameByType("BUTTON", "", parent, "IconButtonTemplate", 0)
 icon = BlzCreateFrameByType("BACKDROP", "", button, "", 0)
 text = BlzCreateFrameByType("TEXT", "", button, "", 0)
 buttonData[button] = unitId
 BlzTriggerRegisterFrameEvent(buttonTrigger, button, FRAMEEVENT_CONTROL_CLICK)
 BlzFrameSetEnable(text, false)
 BlzFrameSetSize(button, 0.2, 0.05)
 BlzFrameSetSize(icon, 0.04, 0.04)
 BlzFrameSetPoint(icon, FRAMEPOINT_LEFT, button, FRAMEPOINT_LEFT, 0.01, 0)
 BlzFrameSetPoint(text, FRAMEPOINT_LEFT, icon, FRAMEPOINT_RIGHT, 0.01, 0)
 BlzFrameSetPoint(text, FRAMEPOINT_RIGHT, button, FRAMEPOINT_RIGHT, 0, 0)
 BlzFrameSetTexture(icon, BlzGetAbilityIcon(unitId), 0, false)
 BlzFrameSetText(text, GetObjectName(unitId))
 BlzFrameSetPoint(button, FRAMEPOINT_TOP, prevFrame, FRAMEPOINT_BOTTOM, 0, 0)
 prevFrame = button

 unitId = FourCC("Hvwd")
 button = BlzCreateFrameByType("BUTTON", "", parent, "IconButtonTemplate", 0)
 icon = BlzCreateFrameByType("BACKDROP", "", button, "", 0)
 text = BlzCreateFrameByType("TEXT", "", button, "", 0)
 buttonData[button] = unitId
 BlzTriggerRegisterFrameEvent(buttonTrigger, button, FRAMEEVENT_CONTROL_CLICK)
 BlzFrameSetEnable(text, false)
 BlzFrameSetSize(button, 0.2, 0.05)
 BlzFrameSetSize(icon, 0.04, 0.04)
 BlzFrameSetPoint(icon, FRAMEPOINT_LEFT, button, FRAMEPOINT_LEFT, 0.01, 0)
 BlzFrameSetPoint(text, FRAMEPOINT_LEFT, icon, FRAMEPOINT_RIGHT, 0.01, 0)
 BlzFrameSetPoint(text, FRAMEPOINT_RIGHT, button, FRAMEPOINT_RIGHT, 0, 0)
 BlzFrameSetTexture(icon, BlzGetAbilityIcon(unitId), 0, false)
 BlzFrameSetText(text, GetObjectName(unitId))
 BlzFrameSetPoint(button, FRAMEPOINT_TOP, prevFrame, FRAMEPOINT_BOTTOM, 0, 0)
 prevFrame = button

 unitId = FourCC("Hjai")
 button = BlzCreateFrameByType("BUTTON", "", parent, "IconButtonTemplate", 0)
 icon = BlzCreateFrameByType("BACKDROP", "", button, "", 0)
 text = BlzCreateFrameByType("TEXT", "", button, "", 0)
 buttonData[button] = unitId
 BlzTriggerRegisterFrameEvent(buttonTrigger, button, FRAMEEVENT_CONTROL_CLICK)
 BlzFrameSetEnable(text, false)
 BlzFrameSetSize(button, 0.2, 0.05)
 BlzFrameSetSize(icon, 0.04, 0.04)
 BlzFrameSetPoint(icon, FRAMEPOINT_LEFT, button, FRAMEPOINT_LEFT, 0.01, 0)
 BlzFrameSetPoint(text, FRAMEPOINT_LEFT, icon, FRAMEPOINT_RIGHT, 0.01, 0)
 BlzFrameSetPoint(text, FRAMEPOINT_RIGHT, button, FRAMEPOINT_RIGHT, 0, 0)
 BlzFrameSetTexture(icon, BlzGetAbilityIcon(unitId), 0, false)
 BlzFrameSetText(text, GetObjectName(unitId))
 BlzFrameSetPoint(button, FRAMEPOINT_TOP, prevFrame, FRAMEPOINT_BOTTOM, 0, 0)
 prevFrame = button

 -- Page 3 Content
 parent = containerFramePage3
 local editBox1, editBox2, result, editboxTrigger

 editBox1 = BlzCreateFrameByType("EDITBOX", "", parent, "EscMenuEditBoxTemplate", 0)
 editBox2 = BlzCreateFrameByType("EDITBOX", "", parent, "EscMenuEditBoxTemplate", 0)
 result = BlzCreateFrameByType("TEXT", "", parent, "", 0)
 BlzFrameSetText(result, "Result")
 BlzFrameSetScale(result, 1.4)
 BlzFrameSetSize(editBox1, 0.1, 0.04)
 BlzFrameSetSize(editBox2, 0.1, 0.04)
 BlzFrameSetPoint(editBox1, FRAMEPOINT_TOPLEFT, parent, FRAMEPOINT_TOPLEFT, 0, 0)
 BlzFrameSetPoint(editBox2, FRAMEPOINT_TOPLEFT, editBox1, FRAMEPOINT_TOPRIGHT, 0, 0)
 BlzFrameSetPoint(result, FRAMEPOINT_BOTTOM, parent, FRAMEPOINT_BOTTOM, 0, 0)

 button = BlzCreateFrameByType("GLUETEXTBUTTON", "", parent, "ScriptDialogButton", 0)
 BlzFrameSetSize(button, 0.03, 0.03)
 BlzFrameSetText(button, "+")
 BlzFrameSetPoint(button, FRAMEPOINT_LEFT, parent, FRAMEPOINT_LEFT, 0, 0)
 editboxTrigger = CreateTrigger()
 BlzTriggerRegisterFrameEvent(editboxTrigger, button, FRAMEEVENT_CONTROL_CLICK)
 TriggerAddAction(editboxTrigger, function()
 BlzFrameSetText(result, (tonumber(BlzFrameGetText(editBox1)) + tonumber(BlzFrameGetText(editBox2))))
 end)
 prevFrame = button

 button = BlzCreateFrameByType("GLUETEXTBUTTON", "", parent, "ScriptDialogButton", 0)
 BlzFrameSetSize(button, 0.03, 0.03)
 BlzFrameSetText(button, "-")
 BlzFrameSetPoint(button, FRAMEPOINT_LEFT, prevFrame, FRAMEPOINT_RIGHT, 0, 0)
 editboxTrigger = CreateTrigger()
 BlzTriggerRegisterFrameEvent(editboxTrigger, button, FRAMEEVENT_CONTROL_CLICK)
 TriggerAddAction(editboxTrigger, function()
 BlzFrameSetText(result, (tonumber(BlzFrameGetText(editBox1)) - tonumber(BlzFrameGetText(editBox2))))
 end)
 prevFrame = button

 button = BlzCreateFrameByType("GLUETEXTBUTTON", "", parent, "ScriptDialogButton", 0)
 BlzFrameSetSize(button, 0.03, 0.03)
 BlzFrameSetText(button, "*")
 BlzFrameSetPoint(button, FRAMEPOINT_LEFT, prevFrame, FRAMEPOINT_RIGHT, 0, 0)
 editboxTrigger = CreateTrigger()
 BlzTriggerRegisterFrameEvent(editboxTrigger, button, FRAMEEVENT_CONTROL_CLICK)
 TriggerAddAction(editboxTrigger, function()
 BlzFrameSetText(result, (tonumber(BlzFrameGetText(editBox1)) * tonumber(BlzFrameGetText(editBox2))))
 end)
 prevFrame = button

 button = BlzCreateFrameByType("GLUETEXTBUTTON", "", parent, "ScriptDialogButton", 0)
 BlzFrameSetSize(button, 0.03, 0.03)
 BlzFrameSetText(button, "/")
 BlzFrameSetPoint(button, FRAMEPOINT_LEFT, prevFrame, FRAMEPOINT_RIGHT, 0, 0)
 editboxTrigger = CreateTrigger()
 BlzTriggerRegisterFrameEvent(editboxTrigger, button, FRAMEEVENT_CONTROL_CLICK)
 TriggerAddAction(editboxTrigger, function()
 BlzFrameSetText(result, (tonumber(BlzFrameGetText(editBox1)) / tonumber(BlzFrameGetText(editBox2))))
 end)
 prevFrame = button


 print("done")
 end
end

##
 Keyboard Focus

(Text)Buttons have the habit to keep the keyboard focus as soon they are pressed (with the mouse). When such a button got the keyboard focus, then hotkeys won't work anymore and pressing return/space will do another control-click. The user can give the focus back to the game by left clicking on the playable world.

As mapper you also have the power to shift the focus. CanFight found a nice, easy way to solve the keyboard focus problem: inside the buttons pressed callback one disables and enables the button, then it counts as clicked and the button loses focus.

 Lua:

-- Buttons of the Frame group tend to keep the input focus when clicked.
-- By disabling them and enabling them again this focus is cleared.
BlzFrameSetEnable(frame, false)
BlzFrameSetEnable(frame, true)

This can also be done for only the clicking player, in case the enable state of the button is not the same for all players.

 Lua:

if GetTriggerPlayer() == GetLocalPlayer() then
 BlzFrameSetEnable(frame, false)
 BlzFrameSetEnable(frame, true)
end

Sadly this Technique can produce a bug, when the Button is clicked and a pressed arrow key is released almost at the same time the camera will keep panning even without holding the arrow key (discovered by Uncle), another mouse click by the user somewhere stops the auto panning.

Uncle found later a way to stop the paning by code using StopCameraForPlayerBJ() or StopCamera()

Editboxes will also Keep the keyboard focus when they are clicked.

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

It also makes tooltip a child of frame, (not for Simpleframes).

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

##
 FrameEvents

 JASS:

constant frameeventtype FRAMEEVENT_CONTROL_CLICK = ConvertFrameEventType(1)
constant frameeventtype FRAMEEVENT_MOUSE_ENTER = ConvertFrameEventType(2)
constant frameeventtype FRAMEEVENT_MOUSE_LEAVE = ConvertFrameEventType(3)
constant frameeventtype FRAMEEVENT_MOUSE_UP = ConvertFrameEventType(4)
constant frameeventtype FRAMEEVENT_MOUSE_DOWN = ConvertFrameEventType(5)
constant frameeventtype FRAMEEVENT_MOUSE_WHEEL = ConvertFrameEventType(6)
constant frameeventtype FRAMEEVENT_CHECKBOX_CHECKED = ConvertFrameEventType(7)
constant frameeventtype FRAMEEVENT_CHECKBOX_UNCHECKED = ConvertFrameEventType(8)
constant frameeventtype FRAMEEVENT_EDITBOX_TEXT_CHANGED = ConvertFrameEventType(9)
constant frameeventtype FRAMEEVENT_POPUPMENU_ITEM_CHANGED = ConvertFrameEventType(10)
constant frameeventtype FRAMEEVENT_MOUSE_DOUBLECLICK = ConvertFrameEventType(11)
constant frameeventtype FRAMEEVENT_SPRITE_ANIM_UPDATE = ConvertFrameEventType(12)
constant frameeventtype FRAMEEVENT_SLIDER_VALUE_CHANGED = ConvertFrameEventType(13)
constant frameeventtype FRAMEEVENT_DIALOG_CANCEL = ConvertFrameEventType(14)
constant frameeventtype FRAMEEVENT_DIALOG_ACCEPT = ConvertFrameEventType(15)
constant frameeventtype FRAMEEVENT_EDITBOX_ENTER = ConvertFrameEventType(16)

This is the result of tests about Frame-Events and Frame-Levels done in 1.31.1.

FRAMEEVENT_CONTROL_CLICK - when activading a Frame either by releasing the left mouse button (the original mouse click has to be inside the Button) or when a Frame has Focus and space or enter/return are pressed. This Event happens before FRAMEEVENT_MOUSE_UP.

In fdf ControlStyle "CLICKONMOUSEDOWN", let this event happen at mouse click instead of release.

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

###
 FrameEvent Getters

 JASS:

BlzGetTriggerFrame returns framehandle
BlzGetTriggerFrameEvent returns frameeventtype
BlzGetTriggerFrameValue returns real
BlzGetTriggerFrameText returns string
GetTriggerPlayer returns player

###
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

##
 Frames & Multiplayer

Warcraft 3 uses for its network functionality a variation of a technique called Lockstep protocol. It is a technic which shares only a bare minimum of data, only the needed ones like user inputs and some sync tests to know players are still playing the same game. Outside of the shared inputs each user's machine simulates the game on it own.

Using this means that you can do anything with Frames for only one user as long the result when using that Frame remains the same for all players (as long the result affects the warcraft 3 game simulation at all). The bare minimum that should be the same is the exisitence and the events for a Frame.

Means one could do almost everything in GetLocalPlayer for frames (position, size visibility, texture, color, text ...), But that is playing with fire, when using it wrong many desyns will happen by your frames.

Hence results for this natives are not guarnted to be equal for all players:

 JASS:

BlzFrameGetText
BlzFrameGetTextSizeLimit
BlzFrameGetEnable
BlzFrameGetAlpha
BlzFrameGetValue
BlzFrameGetHeight
BlzFrameGetWidth
BlzFrameGetParent
BlzFrameIsVisible

###
 Frames and HandleId

When an Frame enters the map's script, it takes a handleId. This should not happen in a GetLocalPlayer block. This applies to all of them BlzCreateFrame, BlzCreateFrameByType, BlzCreateSimpleFrame, BlzGetOriginFrame, BlzGetFrameByName, BlzFrameGetParent, BlzFrameGetChild.

One can reserve handleIds by just calling the way to get the frame. Then afterwards one can get the frame with the getter native in a GetLocalPlayer without a desync.

call BlzGetFrameByName("ConsoleUI", 0)

###
 Input & Current State

Frames handling user input won't sync their state/displayed data. User input has only an effect onto the frame for the player doing that input. When a checkbox is displayed and User A clicks it; only for User A will the checkbox be in the checked state. This also applies to editboxes and sliders. Therefore using BlzFrameGetText(Editbox) or BlzFrameGetValue(Slider) are likely to produce a disconnect, cause their values are meant to be local and differ. How one can use this input?

The answer are FrameEvents inside this events the new state/value is shared with all players. When one needs that value save it to a variable inside the event's Trigger so that all players know it now (although they don't display it in their instance of that frame). Now use the store value for the wanted behaviour that has to happen for all players.

###
 Example

How one uses only one Frame displaying different thing for different players, without an desync happen?

One way is to seperate the display and the data. The data exists for all players but the displayed data onto the Frame differs.

Let's say one has Goldcosts that is saved in an array onto playerIndex.

Another way with one frame is to have all data localy and as soon something happens that would alter the game simulation from this point, one starts an shared Event with all the needed data. BlzSendSyncData could be a possible candidat for that. This requires a good understanding and care what does alter the game also how to dump down everything to a small sync message. This is playing with Fire and requires to know what will alter the game simulation.

Instead of using only one Frame, one could create one Frame for each player. This requires the least usage of GetLocalPlayer and knowlage. But more Frames and Events. Everyone sees only the own Frame but still does all the actions the others do. CreateContext is here a great help, if one does not want to create Frame-Variables.

Let's visualy the differnt techiques on a example. When an player selects an unit an iconButton will display the icon of the selected Unit and when the Button is clicked an Unit of this Type is create for the clicking Player. The button should work for all players at the same time.

The example will not comment the frame creation, if you have problems with that you might look into Icon Button. This example might place some version in a bad light, but each has quite some advantage.

### One Frame

### Local only share when required

### One for every Player

1.Version: One Frame with Data known to anyone.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 -- Give Vision
 FogMaskEnable(false)
 FogEnable(false)
 EnableWorldFogBoundary(false)

 local button = BlzCreateFrameByType("BUTTON", "MyIconButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "ScoreScreenTabButtonTemplate", 0)
 local buttonIconFrame = BlzCreateFrameByType("BACKDROP", "MyIconButtonIcon", button, "", 0)
 BlzFrameSetAllPoints(buttonIconFrame, button)
 BlzFrameSetAbsPoint(button, FRAMEPOINT_CENTER, 0.4, 0.5)
 BlzFrameSetSize(button, 0.03, 0.03)
 BlzFrameSetTexture(buttonIconFrame, "ReplaceableTextures\\CommandButtons\\BTNSelectHeroOn", 0, false)

 -- setup the selection data sharing
 local selectedUnitType = __jarray(0)
 local selectionTrigger = CreateTrigger()
 TriggerRegisterAnyUnitEventBJ(selectionTrigger, EVENT_PLAYER_UNIT_SELECTED)
 TriggerAddAction(selectionTrigger, function()
 -- get the unitTypeId from the selected Unit
 local unitCode = GetUnitTypeId(GetTriggerUnit())
 -- save it onto the active player, all players do that.
 selectedUnitType[GetTriggerPlayer()] = unitCode
 -- update the displayed Texture for the button this only happens for the active player
 if GetLocalPlayer() == GetTriggerPlayer() then
 BlzFrameSetTexture(buttonIconFrame, BlzGetAbilityIcon(unitCode), 0, false)
 end
 end)

 -- the button clicking
 local trigger = CreateTrigger()
 BlzTriggerRegisterFrameEvent(trigger, button, FRAMEEVENT_CONTROL_CLICK)
 TriggerAddAction(trigger, function()
 local player = GetTriggerPlayer()
 CreateUnit(player, selectedUnitType[player] , GetPlayerStartLocationX(player), GetPlayerStartLocationY(player), 0)
 end)
 end
end

2.Version all data is localy on click share it. This Version is actually quite bad for the used example, because all data and events are synced on default.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real() -- Give Vision
 FogMaskEnable(false)
 FogEnable(false)
 EnableWorldFogBoundary(false)

 local button = BlzCreateFrameByType("BUTTON", "MyIconButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "ScoreScreenTabButtonTemplate", 0)
 local buttonIconFrame = BlzCreateFrameByType("BACKDROP", "MyIconButtonIcon", button, "", 0)
 BlzFrameSetAllPoints(buttonIconFrame, button)
 BlzFrameSetAbsPoint(button, FRAMEPOINT_CENTER, 0.4, 0.5)
 BlzFrameSetSize(button, 0.03, 0.03)
 BlzFrameSetTexture(buttonIconFrame, "ReplaceableTextures\\CommandButtons\\BTNSelectHeroOn", 0, false)

 -- setup the selection trigger
 local selectedUnitType = 0
 local selectionTrigger = CreateTrigger()
 TriggerRegisterAnyUnitEventBJ(selectionTrigger, EVENT_PLAYER_UNIT_SELECTED)
 TriggerAddAction(selectionTrigger, function()
 -- do only something for the active player
 if GetLocalPlayer() == GetTriggerPlayer() then
 -- get the unitTypeId from the selected Unit
 local unitCode = GetUnitTypeId(GetTriggerUnit())
 -- save it onto the active player, all players do that.
 selectedUnitType = unitCode
 -- update the displayed Texture for the button
 BlzFrameSetTexture(buttonIconFrame, BlzGetAbilityIcon(unitCode), 0, false)
 end
 end)

 -- the button clicking
 local trigger = CreateTrigger()
 BlzTriggerRegisterFrameEvent(trigger, button, FRAMEEVENT_CONTROL_CLICK)
 TriggerAddAction(trigger, function()

 if GetLocalPlayer() == GetTriggerPlayer() then
 BlzSendSyncData("CreateUnitByButton", selectedUnitType)
 end
 end)

 local syncTrigger = CreateTrigger()
 for int = 0, bj_MAX_PLAYERS - 1 do
 BlzTriggerRegisterPlayerSyncEvent(syncTrigger, Player(int), "CreateUnitByButton", false)
 end
 TriggerAddAction(syncTrigger, function()
 local player = GetTriggerPlayer()
 CreateUnit(player, BlzGetTriggerSyncData() , GetPlayerStartLocationX(player), GetPlayerStartLocationY(player), 0)
 end)
 end
end

3. Version each player has an own Frame all Frames are created and update for all players: It uses GetLocalPlayer() only once, right after the creation.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real() -- Give Vision

 -- Give Vision
 FogMaskEnable(false)
 FogEnable(false)
 EnableWorldFogBoundary(false)

 local buttonTrigger = CreateTrigger()

 -- create one button for each player
 for int = 0, bj_MAX_PLAYERS - 1 do
 local button = BlzCreateFrameByType("BUTTON", "MyIconButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "ScoreScreenTabButtonTemplate", int)
 local buttonIconFrame = BlzCreateFrameByType("BACKDROP", "MyIconButtonIcon", button, "", int)
 BlzFrameSetVisible(button, GetLocalPlayer() == Player(int))
 BlzFrameSetAllPoints(buttonIconFrame, button)
 BlzFrameSetAbsPoint(button, FRAMEPOINT_CENTER, 0.4, 0.5)
 BlzFrameSetSize(button, 0.03, 0.03)
 BlzFrameSetTexture(buttonIconFrame, "ReplaceableTextures\\CommandButtons\\BTNSelectHeroOn", 0, false)
 BlzTriggerRegisterFrameEvent(trigger, button, FRAMEEVENT_CONTROL_CLICK)
 end

 -- setup the selection data sharing
 local selectedUnitType = __jarray(0)
 local selectionTrigger = CreateTrigger()
 TriggerRegisterAnyUnitEventBJ(selectionTrigger, EVENT_PLAYER_UNIT_SELECTED)
 TriggerAddAction(selectionTrigger, function()
 local player = GetTriggerPlayer()
 -- get the unitTypeId from the selected Unit
 local unitCode = GetUnitTypeId(GetTriggerUnit())
 -- save it onto the active player, all players do that.
 selectedUnitType[player] = unitCode
 -- update the displayed Texture for the button
 BlzFrameSetTexture(BlzGetFrameByName("MyIconButtonIcon", GetPlayerId(player)), BlzGetAbilityIcon(unitCode), 0, false)

 end)

 TriggerAddAction(buttonTrigger, function()
 local player = GetTriggerPlayer()
 CreateUnit(player, selectedUnitType[player] , GetPlayerStartLocationX(player), GetPlayerStartLocationY(player), 0)
 end)
 end
end

##
 Frame Save & Load

Sadly there exists a dangerous Bug with Frames in Warcraft 3 which can crash the game. This bug can happen when a custom map created Custom UI-Frames and The map is Saved & Loaded using the warcraft 3 saving. After the Loading all such custom created Frames become broken, hideen and unuseable. If you use such a broken Frame the game can crash. This applies from version 1.31 the first version with custom UI upto 1.32.9 PTR (The time I wrote this).

One can workaround the bug, either by disabling saving/Loading or by recreating all custom Frames on loading. While this sounds difficult to do it is quite simple for static frames.

One basicly only needs to put all the Frame creation functions into a array and run that thing again when the map is loaded.

This would be such a system that runs to which one can register Framecreation functions

 Lua:

-- in 1.31 and upto 1.32.9 PTR (when I wrote this). Frames are not correctly saved and loaded, breaking the game.
-- This runs all functions added to it with a 0s delay after the game was loaded.
FrameLoader = {
 OnLoadTimer = function ()
 for _,v in ipairs(FrameLoader) do v() end
 end
 ,OnLoadAction = function()
 TimerStart(FrameLoader.Timer, 0, false, FrameLoader.OnLoadTimer)
 end
}
function FrameLoaderAdd(func)
 if not FrameLoader.Timer then
 FrameLoader.Trigger = CreateTrigger()
 FrameLoader.Timer = CreateTimer()
 TriggerRegisterGameEvent(FrameLoader.Trigger, EVENT_GAME_LOADED)
 TriggerAddAction(FrameLoader.Trigger, FrameLoader.OnLoadAction)
 end
 table.insert(FrameLoader, func)
end

And a example that uses it.

 Lua:

-- HideMinDamageV3
do
 local realFunction = MarkGameStarted
 local timer, damageA, damageB, parentA, parentB, damageA2, damageB2, text, index
 local function update(sourceFrame, targetFrame)
 text = BlzFrameGetText(sourceFrame)
 index = string.find(text, " - ", 1, true)
 BlzFrameSetText(targetFrame, string.sub( text, index + 3))
 end
 local function Init()
 BlzLoadTOCFile("war3mapImported\\HideMinDamage.toc")
 if not timer then timer = CreateTimer() end
 damageA = BlzGetFrameByName("InfoPanelIconValue", 0)
 damageB = BlzGetFrameByName("InfoPanelIconValue", 1)
 parentA = BlzGetFrameByName("SimpleInfoPanelIconDamage",0)
 parentB = BlzGetFrameByName("SimpleInfoPanelIconDamage",1)
 BlzCreateSimpleFrame("CustomDamageString", parentA, 0)
 damageA2 = BlzGetFrameByName("CustomDamageStringValue", 0)
 BlzCreateSimpleFrame("CustomDamageString", parentB, 1)
 damageB2 = BlzGetFrameByName("CustomDamageStringValue", 1)
 BlzFrameSetFont(damageA, "", 0, 0)
 BlzFrameSetFont(damageB, "", 0, 0)

 TimerStart(timer, 0.05, true, function()
 if BlzFrameIsVisible(parentA) then
 update(damageA, damageA2)
 end
 if BlzFrameIsVisible(parentB) then
 update(damageB, damageB2)
 end
 end)
 end
 function MarkGameStarted()
 realFunction()
 realFunction = nil

 Init()
 if FrameLoaderAdd then FrameLoaderAdd(Init) end
 end
end

 JASS:

library HideMinDamageText initializer init_function requires FrameLoader
// HideMinDamageV3
 globals
 private framehandle DamageA
 private framehandle DamageB
 private framehandle DamageA2
 private framehandle DamageB2
 private framehandle ParentA
 private framehandle ParentB
 private string Text
 private integer Index
 private integer LoopA
 private integer LoopAEnd
 endglobals
 private function find takes nothing returns nothing
 set LoopAEnd = StringLength(Text) - 1
 set LoopA = 1
 loop
 exitwhen LoopA >= LoopAEnd
 if SubString(Text, LoopA, LoopA +3) == " - " then
 set Index = LoopA + 3
 return
 endif
 set LoopA = LoopA + 1
 endloop
 set Index = 0
 endfunction

 private function update takes nothing returns nothing
 if BlzFrameIsVisible(ParentA) then
 set Text = BlzFrameGetText(DamageA)
 call find()
 call BlzFrameSetText(DamageA2, SubString(Text, Index, StringLength(Text)))
 endif
 if BlzFrameIsVisible(ParentB) then
 set Text = BlzFrameGetText(DamageB)
 call find()
 call BlzFrameSetText(DamageB2, SubString(Text, Index, StringLength(Text)))
 endif
 endfunction
 private function At0s takes nothing returns nothing
 call BlzLoadTOCFile("war3mapImported\\HideMinDamage.toc")
 set ParentA = BlzGetFrameByName("SimpleInfoPanelIconDamage", 0)
 set ParentB = BlzGetFrameByName("SimpleInfoPanelIconDamage", 1)
 set DamageA = BlzGetFrameByName("InfoPanelIconValue", 0)
 set DamageB = BlzGetFrameByName("InfoPanelIconValue", 1)
 call BlzCreateSimpleFrame("CustomDamageString", ParentA, 0)
 set DamageA2 = BlzGetFrameByName("CustomDamageStringValue", 0)
 call BlzCreateSimpleFrame("CustomDamageString", ParentB, 1)
 set DamageB2 = BlzGetFrameByName("CustomDamageStringValue", 1)
 call BlzFrameSetFont(DamageA, "", 0, 0)
 call BlzFrameSetFont(DamageB, "", 0, 0)
 call TimerStart(GetExpiredTimer(), 0.05, true, function update)
 endfunction
 private function init_function takes nothing returns nothing
 call FrameLoaderAdd(function At0s)
 call TimerStart(CreateTimer(), 0, false, function At0s)
 endfunction
endlibrary

 JASS:

library FrameLoader initializer init_function
// in 1.31 and upto 1.32.9 PTR (when I wrote this). Frames are not correctly saved and loaded, breaking the game.
// This library runs all functions added to it with a 0s delay after the game was loaded.
// function FrameLoaderAdd takes code func returns nothing
 // func runs when the game is loaded.
 globals
 private trigger eventTrigger = CreateTrigger()
 private trigger actionTrigger = CreateTrigger()
 private timer t = CreateTimer()
 endglobals
 function FrameLoaderAdd takes code func returns nothing
 call TriggerAddAction(actionTrigger, func)
 endfunction

 private function timerAction takes nothing returns nothing
 call TriggerExecute(actionTrigger)
 endfunction
 private function eventAction takes nothing returns nothing
 call TimerStart(t, 0, false, function timerAction)
 endfunction
 private function init_function takes nothing returns nothing
 call TriggerRegisterGameEvent(eventTrigger, EVENT_GAME_LOADED)
 call TriggerAddAction(eventTrigger, function eventAction)
 endfunction
endlibrary

##
 TOC

Probably means Table of Content (WaterKnight)

A TOC file is kind of an batch file listing one or more fdf. Each line is a path to such a fdf. The path is where the game reads the fdf from, it should match the path you import them into your map. When it is executed one can use/gets the content from the listed fdfs.

During the game one loads a TOC-File with native BlzLoadTOCFile takes string TOCFile returns boolean,call BlzLoadTOCFile("war3mapimported\\SomeCustom.toc")

A map can Load any amount of TOC-Files.

The paths inside TOC-File are not case sensitve.

The order of the fdf inside the TOC matters when one loads fdf that includes other fdf that are also mentioned and with that loaded in the same TOC-File. It is recommented to first list the ones that are included in the later ones. (GetLocalPlayer, have to test that)

Reforged only: When 2 TOCs load a Frame with the same name the newer one will overwrite the older one. That can be used to reload some frames for example ConsoleUI or ResourceBarFrame to give nameless Textures names, although this TOC-Loading has to happen in Config or in the root to update a default fdf. Beaware that this also suffers from the Save&Load Bug, you custom Toc will not be loaded when on Loads the game from the Reforged Menu directly, although it works when one starts the map and Loads a Saved Game of the same Map (which would be single player only currently).

The TOC ends with one or two empty lines depends on the end of line sequence used for that file.

If it's "CRLF", you need at least 1 empty line.

If it's "LF", you need at least 2 empty lines. (GetLocalPlayer)​

###
 Loading in Templates

The templates are not Loaded in on default. Therefore let's load them into the game so we can directly create them.

First create a file named Templates.TOC

The content of the file should be that, care for the empty ending line.

 Code:

UI\FrameDef\Glue\standardtemplates.fdf
UI\FrameDef\UI\escmenutemplates.fdf
UI\FrameDef\Glue\battlenettemplates.fdf

After "Templates.toc" was created and saved import the toc-file into your map, keep the path as it is.

Now load it in your code so you can create frames mentioned in standardtemplates.fdf and escmenutemplates.fdf and battlenettemplates.fdf.

BlzLoadTOCFile("war3mapImported\\Templates.toc")

##
 FDF

A Frame Definition File is a fileType used in warcraft 3 to define an UI-Frame. Fdf has a second use case, they can define StringLists with such a StringList one can set Localized Strings.

Blizzard's fdf are found in the casc at this places:

ui/framedef/ui/

ui/framedef/glue/

_locales/dede.w3mod:ui/framedef

The ones in _locals contain localized Strings for that language. In the mpq version they are not an own "folder" but in a Local.mpq.

Glue have frame blueprints for the old game menu before Warcraft 3 V1.32.

UI contains frame blueprints that matter during a match.

Inside fdf one can write Text which is ignored by the fdf parser. There are Line comments and area commentents.

 Code:

// I am a Line comment

/* Opens the Area Comment
*
*
*/ End the Area Comment

###
 The Fdf syntax

There exists something like 105 fdf-actions each of this actions is followed by an group of arguments which are either seperated by whitespace or by comma + optional whitespace. Between the first argument and the Action is whitespace. The used rule of seperation depends on the Action. Some FDF-Actions have multiple Versions with different amount of arguments.

An example for each:

FontColor 0.99 0.827 0.0705 1.0,

FrameFont "EscMenuTextFont", 0.010, "",​

Actions not opening a Block { } end with a comma. Only the Actions: Frame, String, Texture, Layer and StringList open Blocks. All of them except for Layer can exist outside of Blocks. IncludeFile and StringList are Actions only written outside of Blocks. Other fdf-Actions have to be in a Block. Actions in a Block belong to the Action that started the Block most times defining a Frame. One Line can contain multiple Actions in a row without any Line seperator.

I personaly call Frames defined outside of Blocks MainFrame, when I talk about them. Only such MainFrames can be Created & Inherited. Frames can have any amount of Child-Frames which also can have Child-Frames, but each Frame has only one Parent.

Fdf Frames support a behaviour called INHERITS in which a Frame copys the Fdf Actions from another Frame. There also exists an advanced version INHERITS WITHCHILDREN, this also copies the Child-Frames and uses their fdf-Actions. In fdf the Frame one wants to inherit from has to be from the same fdf or from an included one.

For the Fdf-Action Frame: The names of MainFrames in one fdf have to be unique. If another MainFrame with the same name in the current fdf is encountered the remaining File-content is skiped. ChildFrames can have the same name as their brothers, their parent or be empty "".

Each Frame uses one of the Warcraft 3 FrameTypes, with the FrameType the behaviour and the set of possible fdf-Actions for that Frame is choosen. This FrameTypes can be placed into 2 groups SimpleFrames (all of them start with SIMPLE) and the others which I just call Frames (a bit confusing, cause all are Frames). One should do this seperation cause they have different features, techniques, rules and it is difficult to fit them together (logical not visual).

In fdf paths are written only with one "\".

The upto date version of Warcraft 3 tells you about syntax errors of custom fdf. When they are loaded over a toc file in your map. To inspect such a error log run your map with the fdf and toc then close the game and check Users\User\Documents\Warcraft III\Logs\War3Log.txt.

Such an Fdf-error line in that Log-file could look like that:

 Code:

11/8 15:44:54.356 Error (war3mapImported\Test.fdf:4): Expected ",", but found "Height"

Sadly it seems to only tell you about the first encountered error and won't tell you about any error if a toc does not have that needed empty ending line.

The fdf structure often looks like that. For simplity Actions that are not: Frame, IncludeFile and StringList. Are replaced with placeholders .....

 Code:

IncludeFile "UI\FrameDef\UI\EscMenuTemplates.fdf",

Frame "BUTTON" "ButtonTemplate" {
 ....
 ....
}

//
Frame "BUTTON" "FrameNameA" INHERITS WITHCHILDREN "ButtonTemplate" {
 ....
 ....
 ....
 Frame "BACKDROP" "FrameNameB" {
 ....
 ....
 ....
 Frame "TEXT" "FrameNameC" INHERITS "EscMenuLabelTextSmallTemplate" {
 }
 }
}

StringList {
 MYLABEL "My Label Text",
 MYVALUE "My Value Text",
 MYNAME "My Name Text",
 MyMultLineText "Line 1
Line 2
Line 3
Line 4
Line 5",
}

Strings in a StringList don't need to be one lined. They also can go over multiple lines like MyMultLineText.

This fdf-content would define 4 Frames and 4 Localized Strings(MYLABEL, MYVALUE, MYNAME and MyMultLineText).

The Frames are:

"ButtonTemplate" (BUTTON)

"FrameNameA" (BUTTON) (copies fdf-Actions from "ButtonTemplate")

"FrameNameB" (BACKDROP) (Child of "FrameNameA")

"FrameNameC" (TEXT) (Child of "FrameNameB") (copies EscMenuLabelTextSmallTemplate which is included from "UI\FrameDef\UI\EscMenuTemplates.fdf")​

Of this 4 Frames only 2 can be created/Inherited, the MainFrames "ButtonTemplate" and "FrameNameA". One can not create "FrameNameB" nor "FrameNameC" directly. They will be created as sideeffect when one creates the MainFrame being their Ancestor.

If one writes a misstake in a fdf the remaining text of the current file is skiped, the Frame in which the misstake happened will still be created to this point.

##
 FDF-Actions

This FdfActions are used quite often hence, they got a List for themself. The remaining Belong to specific Types and mentioned in their Frametypes (if they it is mentioned).

 Code:

 Frame
 Format1: Frame "FRAMETYPE" "FrameName" {
 Format2: Frame "FRAMETYPE" "FrameName" INHERITS "FrameNameInherited" {
 Format3: Frame "FRAMETYPE" "FrameName" INHERITS WITHCHILDREN "FrameNameInherited" {
 Purpose: Define a new Frame of "FRAMETYPE" with "FrameName", in Foramt2 it will clone most fdf Actions from the inherited Frame. WithChildren will also clone fdfActions for child-Frames. The FrameName can be empty "", but it has to be there.

 Width
 Format1: Width 0.2,
 Format2: Width 0.0153f,
 Purpose: Initial Width of this Frame.

 Height
 Format1: Height 0.1,
 Format2: Height 0.0134f,
 Purpose: Initial Height of this Frame.

 ControlBackdrop
 Format: ControlBackdrop "FrameName",
 Purpose: Define a Functional BACKDROP Child-Frame for the enabled State, used in Frames of the Frame Group except for BACKDROP, TEXT, SPRITE, MODEL, DIALOG and FRAME. It is common that the functional Child-Frame is defined in the next Line.

 ControlDisabledBackdrop
 Format: ControlDisabledBackdrop "FrameName",
 Purpose: Like ControlBackdrop but for disabled Frames. Can be skiped then ControlBackdrop will also be used in disabled State.

 SetPoint
 Format: SetPoint OwnPoint, "FrameName", AttachedPoint, xOffset, yOffset,
 Purpose: Pos the frame with its own FramePoint to AttachedPoint of FrameName with xOffset and yOffset (+x is right +y is up), When the Frame is created the Frame basicly calls BlzFrameSetPoint(self, FramePoint-Own, BlzGetFrameByName(FrameName, 0), FramePoint-FrameName, x, y).

 UseActiveContext
 Format: UseActiveContext,
 Purpose: Alters the behaviour of SetPoint (in fdf) in the Frame it is used, instead of using BlzGetFrameByName(name, 0) it will call BlzGetFrameByName(name, createContext).

 IncludeFile
 Format: IncludeFile "FilePath",
 Purpose: Frames from the Included Fdf can be inherited in this fdf. Should be used outside of Frames. One can use multiple IncludeFile actions in one fdf.

 DecorateFileNames
 Format: DecorateFileNames,
 Purpose: In this Frame all FdfActions asking for FilePaths will use variableNames instead which are taken from a StringList or some Txt-File like GameInterface.

 LayerStyle
 Format1: LayerStyle "IGNORETRACKEVENTS",
 Purpose: Does not trigger Frame Mouse events and can't be clicked. TEXT and FRAME might find good usage for this.
 Format2: LayerStyle "NOSHADING",
 Purpose: WhoKnows
 Format3: LayerStyle "NOSHADING|IGNORETRACKEVENTS", //Both
 Format4: LayerStyle "SETSVIEWPORT",
 Purpose: Limits the visuals of the Layer & SubLayers started by this frame to this frames taken space. The children are still interactive, even when not seeable at a spot.

 Texture
 Format1: Texture {
 Format2: Texture "FrameName" {
 Format3: Texture "FrameName" INHERITS "FrameNameInherited" {
 Purpose: Defines a Texture-Frame, This is only used for SimpleFrames. An Image. More to that In category SimpleFrames.

 String
 Format1: String {
 Format2: String "FrameName" {
 Format3: String "FrameName" INHERITS "FrameNameInherited" {
 Purpose: Defines a String-Frame, This is only used for SimpleFrames. A Text. More to that In category SimpleFrames.

 Alpha
 Format: Alpha 0 to 255,
 Purpose: "calls" BlzFrameSetAlpha(this, value), makes the frame "transparent" or more stable

 ToolTip
 Format: ToolTip FrameName,
 Example: ToolTip "CodeTextFrame",
 Purpose: When this Frame is created it "calls" BlzFrameSetTooltip(this, BlzGetFrameByName(frameName, createdContext))

 DoNotRegisterName
 Format: DoNotRegisterName,
 Purpose: This Frame is not added into the storage accessed by BlzGetFrameByName

###
 Frame Pos in FDF

When one creates frames in fdf one can also set the position of frames. Here one has 2 to 3 options based on the current Frame:

SetPoint FramePoint-Own, FrameName, FramePoint-FrameName, xOffset , yOffset,

SetAllPoints,

Anchor FramePoint, x, y, (only Texture/String)​
SetPoint & Anchor use this Points in fdf:

 JASS:

TOPLEFT
TOP
TOPRIGHT
LEFT
CENTER
RIGHT
BOTTOMLEFT
BOTTOM
BOTTOMRIGHT

SetPoint can be seen as BlzFrameSetPoint(self, FramePoint-Own, BlzGetFrameByName(FrameName, 0), FramePoint-FrameName, x, y). With UseActiveContext, the Frame will connect to the relative Frame with the same CreateContext the current Frame is created with.

This is an example fdf-Frame without UseActiveContext. It is not used because the frame shall attach itself to ("ConsoleUI", 0) even when created with a CreateContext that is not 0.

That Frame of type BACKDROP with Name Test displays, after it's loading & creation, a Paladin-Icon at the Top Left of the screen (Would have to be loaded over a TOC first).

 Code:

Frame "BACKDROP" "Test" {
 Width 0.1,
 Height 0.1,
 SetPoint TOPLEFT, "ConsoleUI", TOPLEFT, 0, 0,
 BackdropBackground "ReplaceableTextures\CommandButtons\BTNHeroPaladin",
}

SetAllPoints equal to BlzFrameSetAllPoints(self, parent).

Anchor is BlzFrameSetPoint(self, point, parent, point, x, y), but Anchor can only be used in Texture/String and only once in each.

Using INHERITS and SetPoint together can be difficult to manage.

The fdf posing uses most rules explained here: Relative Position

###
 FrameTypes

This are the FrameTypes used in the fdfAction Frame, they uppercase only and a expected to be inside "".

 Code:

BACKDROP
BASE (TODO)
BUTTON
CHATDISPLAY
CHECKBOX
CONTROL
DIALOG
EDITBOX
FRAME
GLUEBUTTON
GLUECHECKBOX
GLUEEDITBOX
GLUEPOPUPMENU
GLUETEXTBUTTON
HIGHLIGHT
LISTBOX
LISTBOXITEM (TODO)
LISTBUTTON (TODO)
MENU
MESSAGE (TODO)
MODEL
POPUPMENU
RADIOGROUP (TODO)
SCROLL (TODO)
SCROLLBAR
SIMPLEBUTTON
SIMPLECHECKBOX
SIMPLEFRAME
SIMPLEGRID (TODO)
SIMPLEMESSAGEFRAME (TODO)
SIMPLESTATUSBAR
SIMPLETOP (TODO)
SLASHCHATBOX
SLIDER
SPRITE
STATUSBAR
TEXT
TEXTAREA
TEXTBUTTON
TIMERTEXT

GLUE versions tend to send an audio Feedback on click (when created with BlzCreateFrame)

SLASHCHATBOX, EDITBOX, GLUEEDITBOX are kinda the same thing.

 Code:

Alpha x,
AlphaMode text,
Anchor POINT, x, x,
BackdropBackground text,
BackdropBackgroundInsets x x x x,
BackdropBackgroundSize x,
BackdropBlendAll,
BackdropBottomFile text,
BackdropCornerFlags text,
BackdropCornerSize x,
BackdropEdgeFile text,
BackdropCornerFile text,
BackdropHalfSides,
BackdropLeftFile text,
BackdropMirrored,
BackdropRightFile text,
BackdropTileBackground,
BackdropTopFile text,
BackgroundArt text,
BarTexture text,
ButtonPushedTextOffset x x,
ButtonText text,
ChatDisplayBorderSize x,
ChatDisplayEditBox FrameName,
ChatDisplayLineHeight x,
ChatDisplayScrollBar FrameName,
CheckBoxCheckHighlight text,
CheckBoxDisabledCheckHighlight text,
Checked,
CheckedTexture text,
ControlBackdrop FrameName,
ControlDisabledBackdrop FrameName,
ControlDisabledPushedBackdrop FrameName,
ControlFocusHighlight text,
ControlMouseOverHighlight text,
ControlPushedBackdrop text,
ControlShortcutKey text,
ControlStyle text,
CursorSprite model,
DecorateFileNames,
DialogBackdrop FrameName,
DialogOkButton FrameName,
DialogCancelButton FrameName,
DisabledText text text,
DisabledTexture text,
DoNotRegisterName,
EditBorderSize x,
EditCursorColor x x x x,
EditHighlightColor red green blue alpha,
EditHighlightInitial,
EditMaxChars int,
EditSetFocus,
EditText text,
EditTextColor red green blue alpha,
EditTextFrame frameName,
EditTextOffset x y,
File text,
Font text, x,
FontColor x x x (x),
FontDisabledColor x x x (x),
FontFlags text,
FontHighlightColor x x x (x),
FontJustificationH JUSTIFYH,
FontJustificationV JUSTIFYV,
FontJustificationOffset x x,
FontShadowColor x x x (x),
FontShadowOffset x x,
Frame text text INHERITS WITHCHILDREN text {
FrameFont text, x, text,
Height x,
HighlightAlphaFile text,
HighlightAlphaMode text,
HighlightColor x x x (x),
HighlightText text text,
HighlightType text,
IncludeFile text,
ID int,
LayerStyle text,
ListBoxItem frameName,
ListBoxItemHeight x,
ListBoxBorder x,
ListBoxScrollBar FrameName,
ListBoxStyle flagString,
MenuBorder x,
MenuItem text, x,
MenuItemHeight x,
MenuTextHighlightColor x x x (x),
NormalText text text,
NormalTexture text,
PopupArrowFrame FrameName,
PopupButtonInset x,
PopupMenuFrame FrameName,
PopupTitleFrame FrameName,
PushedTexture text,
ScrollBarDecButtonFrame FrameName,
ScrollBarIncButtonFrame FrameName,
SetAllPoints,
SetPoint POINT, FrameName, POINT, x, x,
SliderInitialValue x,
SliderInitialValue x,
SliderLayoutHorizontal,
SliderLayoutVertical,
SliderMaxValue x,
SliderMinValue x,
SliderStepSize x,
SliderThumbButtonFrame FrameName,
SpriteCamera int,
SpriteScale x x x,
StatusBarSprite model,
TabFocusDefault,
TabFocusNext FrameName,
TabFocusPush,
TexCoord x, x, x, x,
Text text,
TextAreaInset x,
TextAreaLineGap x,
TextAreaLineHeight x,
TextAreaMaxLines x,
TextAreaScrollBar FrameName,
TextLength x,
ToolTip FrameName,
UseActiveContext,
UseHighlight text,
Width x,
Frame text text (INHERITS) (WITHCHILDREN) (text)
String (text) (INHERITS) (text)
Texture (text) (INHERITS) (text)
Layer (text)
StringList

##
 BACKDROP

Backdrops manage the visual textures of the Frame group, they are borders, backgrounds or images (for non simple frames). Most frames beeing more than simple text have functional child-Frames of Type backdrop that manage the textures beeing shown.

The most common functional Backdrop are:

ControlBackdrop <name>,
<name> is the name of the frame beeing used, it also is most time declared right below the control line. This backdrop is used when the parentFrame is enabled and basicly every Frame has such a thing.

ControlDisabledBackdrop <name>,
backdrop when the parentFrame is disabled.

The BackDrop FDF-Actions:

 Code:

 BackdropBackground
 Format: BackdropBackground filepath,
 Purpose: Defines the background/main texture.

 BackdropBackgroundInsets
 Format: BackdropBackgroundInsets real real real real,
 Format: BackdropBackgroundInsets Right TOP Bottom Left,
 Example: BackdropBackgroundInsets 0.004 0.004 0.004 0.004,
 Purpose: With Positive Numbers BackdropBackground will in smaller area then the BACKDROP takes. -Numbers allow to extend a Side. Survives Texture swaping by Code.

 BackdropBackgroundSize
 Format: BackdropBackgroundSize real,
 Example: BackdropBackgroundSize 0.032,
 Purpose: in Tile mode, the size of each tile

 BackdropBlendAll
 Format: BackdropBlendAll,
 Purpose: Allows to see the stuff below the Frame. Maybe:(Use transparency by alpha channels. An image with alpha channel transparency but without this, will be displayed wrong.

 BackdropBottomFile
 Format: BackdropBottomFile FilePath,
 Example: BackdropBottomFile "UI\Widgets\ButtonBottom.blp",
 Purpose: Sets the Texture for the bottom part of the Border

 BackdropCornerFile
 Format: BackdropCornerFile FilePath,
 Example: BackdropCornerFile "UI\Widgets\ButtonCorners.blp",
 Purpose:

 BackdropCornerFlags
 Format: BackdropCornerFlags Text,
 Example: BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R", //Whole Border
 Example: BackdropCornerFlags "UL|UR|T|L|R", //No Bottom Border
 Purpose: Edge/BorderFiles displayed, one can skip some and order does not matter. Using this without seting border files will crash the game when this Backdrop is displayed.

 BackdropCornerSize
 Format: BackdropCornerSize real,
 Example: BackdropCornerSize 0.048,
 Purpose: Size of the border/edge.

 BackdropEdgeFile
 Format: BackdropEdgeFile FilePath,
 Example: BackdropEdgeFile "UI\Widgets\BattleNet\bnet-inputbox-border.blp",
 Purpose: Set the BorderFile for that BACKDROP, this file contains all Parts of the Border as fragments next to each other.

 BackdropHalfSides
 Format: BackdropHalfSides,
 Purpose: WhoKnows

 BackdropLeftFile
 Format: BackdropLeftFile FilePath,
 Example: BackdropLeftFile "UI\Widgets\ButtonLeft.blp",
 Purpose: Left part of the Border

 BackdropMirrored
 Format: BackdropMirrored,
 Purpose: Mirror the displayed BackdropBackground. Left and Right <->. Survives Texture swaping by Code.

 BackdropRightFile
 Format: BackdropRightFile FilePath,
 Example: BackdropRightFile "UI\Widgets\ButtonRight.blp",
 Purpose: Right part of the Border

 Format: BackdropTileBackground,
 Example: BackdropTileBackground,
 Purpose: Fills the Frame with instances of the texture. Without the background file is stretched. One also should set BackdropBackgroundSize when using this feature. Can be used in BlzFrameSetTexture(frame, file, 1, blend).

 BackdropTopFile
 Format: BackdropTopFile FilePath,
 Example: BackdropTopFile "UI\Widgets\ButtonTop.blp",
 Purpose: Top part of the Border

Example from "UI/FrameDef/UI/EscMenuTemplates.fdf"

 Code:

Frame "BACKDROP" "EscMenuControlBackdropTemplate" {
 DecorateFileNames,
 BackdropTileBackground,
 BackdropBackground "EscMenuEditBoxBackground",
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.0125,
 BackdropBackgroundSize 0.256,
 BackdropBackgroundInsets 0.005 0.005 0.005 0.005,
 BackdropEdgeFile "EscMenuEditBoxBorder",
 BackdropBlendAll,
}

Custom Example

 Code:

Frame "BACKDROP" "FootManIcon" {
 Width 0.1,
 Height 0.1,
 SetPoint CENTER, "ConsoleUI", CENTER, 0, 0,
 BackdropBackground "ReplaceableTextures\CommandButtons\BTNFootman",
}

Using BlzFrameSetTexture will drop the BackdropEdgeFile settings.

##
 Button

Buttons are powerful and useful UI-Frames. A Button in Warcraft 3 is only a space on the screen that can be pressed. The pressing can send an audio Feedback (GLUE created with BlzCreateFrame) and one can catch the pressing with a TriggerEvent which allows executing Code. But if the button is only the clickable space where does the displayed Image, the Text and the Highlight come from?

Each is an own functional child-Frame, doing one job. They mimic size and Position of the parent Frame. There is a BACKDROP that is shown when the BUTTON is enabled, another is shown, when pressed, when the BUTTON is disabled and so on, each such is an own ChildFrame....

###
 Example

Here is an Image of the text inside a fdf of a GLUETEXTBUTTON containg most, when not all strong connected childFrames. In the Image: text that is connected to Childframes is in a colored box. Also there are some comments right to the Frames in different color.

The TEXT-ChildFrame is not needed to set the text of the TEXTBUTTON by Code, but it is needed to style it in the fdf.

The highlighter does only work when ControlStyle mentions that behaviour.

A (GLUE)BUTTON is basicly the same without the ButtonText feature/child-Frame.

###
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

How one would create this "HeroSelectorButton". One can't cause it is not loaded.

 JASS:

//Create "HeroSelectorButton", for game UI
 local framehandle buttonFrame = BlzCreateFrame("HeroSelectorButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
//By Having created "HeroSelectorButton" also its children are created, this children are accessed using BlzGetFrameByName right after the creation or later one if the slots were not taken by other frames.
 local framehandle buttonIconFrame = BlzGetFrameByName("HeroSelectorButtonIcon", 0)
//Set a Texture
 call BlzFrameSetTexture(buttonIconFrame, "ReplaceableTextures\\CommandButtons\\BTNSelectHeroOn", 0, true)

###
 Hotkeys in fdf

(GLUE)(TEXT)BUTTONs can have set Hotkeys right in the fdf. This is done by giving the Button a Parent Frame that has the fdfAction TabFocusPush, aswell as creating a StringList with the wanted Hotkeys. Then one creates Buttons as child of the Frame with TabFocusPush, and setups ControlShortcutKey "StringName in StringList",. This hotkeys will consider the current active main input Frame, when created for GAMEUI they will stop listen when the player is in a Menu or types in chat messages.

For some wierd reason the Hotkeys don't work out of the Box (when created for GAMEUI), first one has to hide and show the Parent Frame. Then the hotkeys will listen and start events when clicked. The hotkeys will fire a control-click Event which does not keep the keyboard focus unlike when the event is started with a mouse click.

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
 ControlShortcutKey "KeyMyButton1",
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

##
 Checkbox

A checkbox is an Frame that has 2 states unchecked and checked. This state is visualy shown with a function Highlight child-Frame.

 Code:

 CheckBoxCheckHighlight
 Format: CheckBoxCheckHighlight "FrameName",
 Example: CheckBoxCheckHighlight "BattleNetRadioButtonHighlightTemplate",
 Purpose: Frame beeing created that is used as CheckBoxCheckHighlight. It is a frame of type HIGHLIGHT. Displays while the CheckBox is checked and enabled.

 CheckBoxDisabledCheckHighlight
 Format: CheckBoxDisabledCheckHighlight FrameName,
 Example: CheckBoxDisabledCheckHighlight "EscMenuDisabledCheckHighlightTemplate",
 Purpose: Same as CheckBoxCheckHighlight in disabled State.

Example CheckBox from Blizzard:

 Code:

Frame "GLUECHECKBOX" "EscMenuCheckBoxTemplate" {
 Width 0.024,
 Height 0.024,

 ControlBackdrop "EscMenuCheckBoxBackdrop",
 Frame "BACKDROP" "EscMenuCheckBoxBackdrop" {
 DecorateFileNames,
 BackdropBlendAll,
 BackdropBackground "EscMenuCheckBoxBackground",
 }

 ControlDisabledBackdrop "EscMenuDisabledCheckBoxBackdrop",
 Frame "BACKDROP" "EscMenuDisabledCheckBoxBackdrop" {
 DecorateFileNames,
 BackdropBlendAll,
 BackdropBackground "EscMenuDisabledCheckBoxBackground",
 }

 ControlPushedBackdrop "EscMenuCheckBoxPushedBackdrop",
 Frame "BACKDROP" "EscMenuCheckBoxPushedBackdrop" {
 DecorateFileNames,
 BackdropBlendAll,
 BackdropBackground "EscMenuCheckBoxPushedBackground",
 }

 CheckBoxCheckHighlight "EscMenuCheckHighlightTemplate",
 Frame "HIGHLIGHT" "EscMenuCheckHighlightTemplate" {
 DecorateFileNames,
 HighlightType "FILETEXTURE",
 HighlightAlphaFile "EscMenuCheckBoxCheckHighlight",
 HighlightAlphaMode "BLEND",
 }

 CheckBoxDisabledCheckHighlight "EscMenuDisabledCheckHighlightTemplate",
 Frame "HIGHLIGHT" "EscMenuDisabledCheckHighlightTemplate" {
 DecorateFileNames,
 HighlightType "FILETEXTURE",
 HighlightAlphaFile "EscMenuDisabledCheckHighlight",
 HighlightAlphaMode "BLEND",
 }
}

##
 Dialog - Yes/No

Dialog is a Frame to give the player the option to accept or decline. But the dialogevents are only useable when the dialog's buttons were defined in fdf.

 Code:

DialogBackdrop
Format: DialogBackdrop FrameName,
Example: DialogBackdrop "MyOptionsConfirmDialogBackdrop",
Purpose: ControlBackdrop for DIALOG

DialogOkButton
Format: DialogOkButton FrameName,
Example: DialogOkButton "ConfirmOKButton",
Purpose: GLUETEXTBUTTON for DIALOG used as ok. The GLUETEXTBUTTON has to be a direct child of the dialog.

DialogCancelButton
Format: DialogCancelButton FrameName,
Example: DialogCancelButton "ConfirmCancelButton",
Purpose: GLUETEXTBUTTON for DIALOG used as cancel. The GLUETEXTBUTTON has to be a direct child of the dialog.

Example pseudo Lua code, expects war3mapimported\\Test.toc & war3mapimported\\Test.fdf

 Lua:

BlzLoadTOCFile("war3mapimported\\Test.toc")
local dia = BlzCreateFrame("MySimplyDialog", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
local trigger = CreateTrigger()
TriggerAddAction(trigger, function()
 if BlzGetTriggerFrameEvent() == FRAMEEVENT_DIALOG_ACCEPT then
 print("Dialog Accept")
 else
 print("Dialog Reject")
 end
 BlzFrameSetVisible(BlzFrameGetParent(BlzGetTriggerFrame()), false)
end)
BlzTriggerRegisterFrameEvent(trigger, dia, FRAMEEVENT_DIALOG_ACCEPT)
BlzTriggerRegisterFrameEvent(trigger, dia, FRAMEEVENT_DIALOG_CANCEL)

[attachment 390836](https://www.hiveworkshop.com/attachments/simplesmalldialog-jpg.390836/)

Small Simple Dialog

 Code:

IncludeFile "UI\FrameDef\UI\EscMenuTemplates.fdf",
IncludeFile "UI\FrameDef\Glue\StandardTemplates.fdf",
Frame "DIALOG" "MySimplyDialog" {
 Height 0.06,
 Width 0.09,
 SetPoint CENTER, "ConsoleUI", CENTER, 0, 0,

 DialogBackdrop "MySimplyDialogBackdrop",
 Frame "BACKDROP" "MySimplyDialogBackdrop" {
 UseActiveContext,
 SetAllPoints,
 DecorateFileNames,
 BackdropTileBackground,
 BackdropBackground "EscMenuEditBoxBackground",
 BackdropCornerFlags "UL|UR|BL|BR|T|L|B|R",
 BackdropCornerSize 0.0125,
 BackdropBackgroundInsets 0.005f 0.005f 0.005f 0.005f,
 BackdropEdgeFile "EscMenuEditBoxBorder",
 BackdropBlendAll,
 }

 Frame "TEXT" "MySimplyDialogTitleText" INHERITS "StandardTitleTextTemplate" {
 SetPoint TOP, "MySimplyDialog", TOP, 0.0, -0.005,
 Text "Kick Red",
 }

 DialogOkButton "MySimplyDialogOKButton",
 Frame "GLUETEXTBUTTON" "MySimplyDialogOKButton" INHERITS WITHCHILDREN "EscMenuButtonTemplate" {
 Width 0.04,
 Height 0.03,
 SetPoint BOTTOMRIGHT,"MySimplyDialog", BOTTOM, 0, 0.005625,
 ControlShortcutKey "KEY_OK_SHORTCUT",
 ButtonText "ConfirmOKButtonText",
 Frame "TEXT" "ConfirmOKButtonText" INHERITS "StandardButtonTextTemplate" {
 Text "YES",
 }
 }

 DialogCancelButton "MySimplyDialogCancelButton",
 Frame "GLUETEXTBUTTON" "MySimplyDialogCancelButton" INHERITS WITHCHILDREN "StandardButtonTemplate" {
 Width 0.04,
 Height 0.03,
 SetPoint BOTTOMLEFT,"MySimplyDialog", BOTTOM, 0, 0.005625,
 ControlShortcutKey "KEY_CANCEL_SHORTCUT",
 ButtonText "ConfirmCancelButtonText",
 Frame "TEXT" "ConfirmCancelButtonText" INHERITS "StandardButtonTextTemplate" {
 Text "NO",
 }
 }
}

##
 EditBox - User Text

Editboxes are single line text frames beeing editable by players. There are 4 predefined mainframe Editboxes in the default fdfs, but 2 of them are basicly "equal" to some other one.

 BattleNetEditBoxTemplate (same values as "StandardEditBoxTemplate")

 StandardEditBoxTemplate

 StandardDecoratedEditBoxTemplate (same values as "EscMenuEditBoxTemplate")

 EscMenuEditBoxTemplate
All of them are not loaded on default. In this tutorial I use "EscMenuEditBoxTemplate" from "UI\FrameDef\UI\escmenutemplates.fdf".

An Editbox is used to let the player type in text. There are 2 events for editboxes handling text input:

 Code:

FRAMEEVENT_EDITBOX_TEXT_CHANGED
FRAMEEVENT_EDITBOX_ENTER

In both events one uses BlzGetTriggerFrameText to get the text GetTriggerPlayer has inside its box during the event this text is synced.

BlzGetTriggerFrameText length won't exceed 255. Text after the 255. position is not contained inside BlzGetTriggerFrameText.

FRAMEEVENT_EDITBOX_ENTER, when the local player gave the editbox focus and presses enter/return. The currently local text in the editbox will be BlzGetTriggerFrameText.

FRAMEEVENT_EDITBOX_TEXT_CHANGED, when for the local player the text of the editbox changed. Happens on adding/Removing Text by player or by code (Setting the text synced will evoke one event for each player). This event will happen quite often.

Outside of this events one would need BlzFrameGetText to get the text, but BlzFrameGetText returns for each player the text he currently has in his editbox -> is not synced in multiplayer. Therefore one has to sync it using the frameevents.

###
 Example

Thats our Lua code for the demo. It Loads the custom tocFile, creates a frame of name "EscMenuEditBoxTemplate" and registeres 2 events to that frame. Also when the local player has its keyboard focus on the editbox and presses enter/return, its current insert text will be shown in the message frame and that message is saved in the gui variable udg_UserInput[playerIndex].

 Lua:

function EditBoxEnter()
 print("EditBoxEnter:")
 print(BlzGetTriggerFrameText())
 print(GetPlayerName(GetTriggerPlayer()))
 udg_UserInput[GetConvertedPlayerId(GetTriggerPlayer())] = BlzGetTriggerFrameText() --save the text of the local player in a synced manner.
 end

 function TEXT_CHANGED()
 --print("TEXT_CHANGED")
 --print(BlzGetTriggerFrameText())
 --print(GetPlayerName(GetTriggerPlayer()))
 end


do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 BlzLoadTOCFile("war3mapimported\\templates.toc")
 local editbox = BlzCreateFrame("EscMenuEditBoxTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0),0,0) --create the box
 local eventHandler
 BlzFrameSetAbsPoint(editbox, FRAMEPOINT_CENTER, 0.4, 0.3) -- pos the box
 BlzFrameSetSize(editbox, 0.2, 0.03) --set the boxs size
 eventHandler = CreateTrigger() --Create the FRAMEEVENT_EDITBOX_ENTER trigger
 TriggerAddAction(eventHandler, EditBoxEnter)
 BlzTriggerRegisterFrameEvent(eventHandler, editbox, FRAMEEVENT_EDITBOX_ENTER)
 eventHandler = CreateTrigger() --Create the FRAMEEVENT_EDITBOX_TEXT_CHANGED trigger
 TriggerAddAction(eventHandler, TEXT_CHANGED)
 BlzTriggerRegisterFrameEvent(eventHandler, editbox, FRAMEEVENT_EDITBOX_TEXT_CHANGED)
 end
end

This Trigger will print the text the local player has currently shown in his editbox. The currently Shown text is probably not synced and can desync the game, if it is used in a synced manner.

Press Esc
Events
Player - Player 1 (Red) skips a cinematic sequence

Player - Player 2 (Blue) skips a cinematic sequence

Conditions

Actions
Custom script: print(BlzFrameGetText(BlzGetFrameByName("EscMenuEditBoxTemplate",0)))

##
 Text Limits

One might want to limit the amount of Text the player can put into the editbox, one can set such a limit by code quite simple. The native one uses is BlzFrameSetTextSizeLimit takes framehandle frame, integer size.

After using BlzFrameSetTextSizeLimit(editbox, 10) the editbox can only contain 10 chars.

As said above it does not make much sense to allow a limit above 255 when the text has to be used inside Events in a synced manner.

An editbox can containt 4096 chars, if that amount is exceeded, it becomes invisible.(KeepVary)

On default an editbox uses an TextSizeLimit of -256 which allows any amount of input, but regardless of allowed input only 255 are useable inside the events.

One can read the current limit with BlzFrameGetTextSizeLimit(editbox).

The Editbox inputtext will be in one line even with a bigger height.

 Code:

EditBorderSize
 Format: EditBorderSize real,
 Example: EditBorderSize 0.009,
 Purpose: Offsets Text by that amount from the border

EditCursorColor
 Format: EditCursorColor real real real real,
 Example: EditCursorColor 1.0 1.0 1.0 1.0,
 Purpose: Defines the color of the EditBox's Cursor | while it has focus. The alpha value can be skiped then it is 1.0.

EditText
 Format: EditText text,
 Example: EditText "YES",
 Purpose: Initial Text in the EditBox

EditTextColor
 Format: EditTextColor red green blue alpha,
 Example: EditTextColor 1.0 1.0 1.0,
 Purpose: Defines the color of the EditBox's Text. The alpha value can be skiped then it is 1.0. FontColor inside EditTextFrame is stronger.

EditTextFrame
 Format: EditTextFrame FrameName,
 Example: EditTextFrame "SaveGameFileEditBoxText",
 Purpose: Style the Text of the EditBox. Expects a TEXT-Frame. For some Reasons EditTextFrame has no effect onto the Cursor. The EditBox itself can have a FrameFont to change the Cursors Font.

EditTextOffset
 Format: EditTextOffset x y,
 Example: EditTextOffset 0.01 0.0,
 Purpose: Offsets the Text inside the Editbox, the Text can not leave the EditBox using this. If it would leave the text becomes "hidden".

FrameFont "MasterFont", 0.015, "",
 FrameFont directly in the EditBox defines the Cursor's Font(Size).

EditHighlightColor
 Format: EditHighlightColor red green blue alpha,
 Example: EditHighlightColor 1.0 1.0 1.0 0.5,
 Purpose: Color of the EditBox's Marked Text Box. Alpha should not be 1.0 because it makes the marked text unreadable. The alpha value can be skiped then it is 1.0.

EditHighlightInitial
 Format: EditHighlightInitial,
 Purpose: The initial Text is marked. The Cursor FontSize and Text FontSize should match otherwise only a part of the initial text is marked.

EditMaxChars
 Format: EditMaxChars int,
 Example: EditMaxChars 11,
 Purpose: Can't write more than that amount of Chars into the EditBox. Chars that take more than one byte take multiple slots. For example 11 can be 1234567890A or ÄÄÄÄÄA as Ä take 2 bytes. Pasting text into with a longer text is rejected. Chars that require more than 1 byte

EditSetFocus
 Format: EditSetFocus,
 Purpose: Gives this Frame focus when it is created. The focus given that way is quite strong clicking on the ground does not transfer focus nor does hiding it, But clicking onto another custom UI took it away and the focus became normal.

Here is an example of an EditBox fdf with a bigger FontSize. EditBox have 2 FontSizes, the Cursor and the Text. FrameFont inside EditBox changes the cursor. Inside the TEXT child it changes the displayed Text.

 Code:

IncludeFile "UI/FrameDef/UI/EscMenuTemplates.fdf",

Frame "EDITBOX" "CustomCommandsEditBox" INHERITS WITHCHILDREN "EscMenuEditBoxTemplate" {
 Width 0.15,
 Height 0.037,
 DecorateFileNames,
 FrameFont "MasterFont", 0.015, "",

 EditTextFrame "CustomCommandsEditBoxText",
 Frame "TEXT" "CustomCommandsEditBoxText" INHERITS "EscMenuEditBoxTextTemplate" {
 FrameFont "MasterFont", 0.015, "",
 }
}

##
 ListBox

it seems that (In 1.32.6) the frame api lacks events to use LISTBOX making it kinda weak. But anyway here is an example how to create a ListBox using fdf + Lua.

 Code:

IncludeFile "UI\FrameDef\Glue\StandardTemplates.fdf",
IncludeFile "UI\FrameDef\UI\EscMenuTemplates.fdf",

Frame "LISTBOXITEM" "MyListBoxItemTemplate" {
}
Frame "LISTBOX" "MyListBox" {
 Height 0.08,
 Width 0.12,
 SetPoint CENTER, "ConsoleUI", CENTER, 0, 0,

 ControlBackdrop "ListBoxBackdrop",
 Frame "BACKDROP" "ListBoxBackdrop" INHERITS "EscMenuEditBoxBackdropTemplate" {
 UseActiveContext,
 }

 ListBoxScrollBar "ListScrollBar",
 Frame "SCROLLBAR" "ListScrollBar" INHERITS WITHCHILDREN "StandardScrollBarTemplate" {
 }

 Frame "LISTBOXITEM" "A" INHERITS WITHCHILDREN "MyListBoxItemTemplate" {
 Text "Item A",
 }
 Frame "LISTBOXITEM" "B" INHERITS WITHCHILDREN "MyListBoxItemTemplate" {
 Text "Item B",
 }
 Frame "LISTBOXITEM" "C" INHERITS WITHCHILDREN "MyListBoxItemTemplate" {
 Text "Item C",
 }
 Frame "LISTBOXITEM" "D" INHERITS WITHCHILDREN "MyListBoxItemTemplate" {
 Text "Item D",
 }
 Frame "LISTBOXITEM" "E" INHERITS WITHCHILDREN "MyListBoxItemTemplate" {
 Text "Item E",
 }
 Frame "LISTBOXITEM" "F" INHERITS WITHCHILDREN "MyListBoxItemTemplate" {
 Text "Item F",
 }
 Frame "LISTBOXITEM" "G" INHERITS WITHCHILDREN "MyListBoxItemTemplate" {
 Text "Item G",
 }
 Frame "LISTBOXITEM" "H" INHERITS WITHCHILDREN "MyListBoxItemTemplate" {
 Text "Item H",
 }
 Frame "LISTBOXITEM" "I" INHERITS WITHCHILDREN "MyListBoxItemTemplate" {
 Text "Item I",
 }
 Frame "LISTBOXITEM" "J" INHERITS WITHCHILDREN "MyListBoxItemTemplate" {
 Text "Item J",
 }


 ListBoxBorder 0.01, // how big is the used border used for offsetting the items

 ListBoxItemHeight 0.013,
 ListBoxItem ADDEND, "A", // position at which FrameName is added, sadly the target loses its name
 ListBoxItem ADDEND, "B",
 ListBoxItem ADDEND, "C",
 ListBoxItem ADDEND, "D",
 ListBoxItem ADDEND, "E",
 ListBoxItem ADDEND, "F",
 ListBoxItem ADDEND, "G",
 ListBoxItem ADDEND, "H",
 ListBoxItem ADDEND, "I",
 ListBoxItem ADDEND, "J",
}

 Lua:

function Test()
 local data = {}
 BlzLoadTOCFile("war3mapimported\\Test.toc")
 local dia = BlzCreateFrame("MyListBox", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)

 local trigger = CreateTrigger()
 TriggerAddAction(trigger, function()

 if data[BlzGetTriggerFrame()] then
 print(data[BlzGetTriggerFrame()], "Action")
 else
 print(BlzFrameGetName(BlzGetTriggerFrame()), "Action")
 end
 print(BlzFrameGetText(BlzGetTriggerFrame()))
 print(GetHandleId(BlzGetTriggerFrameEvent()), BlzGetTriggerFrameValue())

 end)
 local function registerFrameEvents(frame)
 for i= 0, 16 do
 BlzTriggerRegisterFrameEvent(trigger, frame, ConvertFrameEventType(i))
 end
 end

 registerFrameEvents(dia)

-- The FrameEvents for ListBoxs are lacking. one would have to find the ListBoxItems over the child api (because they lost their name) and register the items
 for i = 0, BlzFrameGetChildrenCount(dia) - 1 do
 local child = BlzFrameGetChild(dia, i)
 registerFrameEvents(child)
 data[child] = "Child "..i
 print("child Count of Child ", i,BlzFrameGetChildrenCount(child))
 if BlzFrameGetChildrenCount(child) > 0 then
 for j = 0, BlzFrameGetChildrenCount(child) - 1 do
 data[BlzFrameGetChild(child, j)] = "GrandChild "..j
 registerFrameEvents(BlzFrameGetChild(child, j))
 end
 end
 end

 print("done")
end

##
 Highlight

Highlight are only used as functional child-Frames. In Warcraft 3 V1.31 and 1.32 there is no way to alter the used Texture during the game.

Changing the Parent of a Highlight during the game can bug it making it overglow sometimes.

 Code:

 HighlightAlphaFile
 Format: HighlightAlphaFile FilePath,
 Examples: HighlightAlphaFile "UI\Glues\ScoreScreen\scorescreen-tab-hilight.blp",
 "UI\Widgets\BattleNet\bnet-button01-highlight-mouse.blp" //Blue glow
 "UI\Widgets\Glues\GlueScreen-Button-KeyboardHighlight.blp" //bluesurronding
 "UI\Widgets\Glues\GlueScreen-CampaignButton-KeyboardHighlight.blp" //bluesqure
 "UI\Widgets\Glues\GlueScreen-RadioButton-Button.blp" //Blue point
 "UI\Widgets\Glues\GlueScreen-RadioButton-ButtonDisabled.blp" //gray point
 "UI\Glues\ScoreScreen\scorescreen-tab-hilight.blp" //yellow glow
 "UI\Widgets\Glues\GlueScreen-Checkbox-Check.blp" //golden checked
 "UI\Widgets\Glues\GlueScreen-Checkbox-CheckDisabled.blp" //gray checked
 Purpose: Set the file beeing used by that Highlight

 HighlightAlphaMode
 Format1: HighlightAlphaMode "ADD",
 Format2: HighlightAlphaMode "BLEND",
 Purpose: Changes the way the Highlight interacts with the Frame below. "BLEND" is used by the Blizzard CheckBoxes.

 HighlightType
 HighlightType "FILETEXTURE",
 Purpose:
 HighlightType "SHADE",
 Purpose:

 HighlightColor
 Format: HighlightColor red green blue alpha,
 Example: HighlightColor 1.0 0.0 0.0 0.2,
 Purpose: Appears together with HighlightType "SHADE",

 Code:

Frame "HIGHLIGHT" "EscMenuCheckHighlightTemplate" {
 DecorateFileNames,
 HighlightType "FILETEXTURE",
 HighlightAlphaFile "EscMenuCheckBoxCheckHighlight",
 HighlightAlphaMode "BLEND",
}

##
 Frame

FRAME fights for mousecontrol of the Screen space given to him still can not have any FrameEvents. It is possible to give and use FRAME a(s) Tooltip.

They can not have functional Child-Frames. Making them a good usage for dummy Frames and logical Containers.

 Code:

Frame "FRAME" "MyFrame" {
LayerStyle "IGNORETRACKEVENTS", // this Frame itself will not control/Block the Mouse
}

##
 Popupmenu

In warcraft 3 a Popupmenu is a Button when it is clicked below it the selectable options appear. PopupMenu have various functional Childframes. The selectable options can only be set in fdf (Warcraft 3 version 1.31 and 1.32).

Checkout the Fdf below: "MyPopupTemplate" is a PopupMenu misses the selectable options but a setting for the visuals making it a good inherit source. Therefore it is inherited by "TestPopup" and "CustomCommandsPopup" they also setupt the chooseable options.

Inside code one can get the current selected option index with BlzFrameGetValue starting with 0, it also can return -1 when nothing was selected. Inside FRAMEEVENT_POPUPMENU_ITEM_CHANGED BlzGetTriggerFrameValue() returns the new choosen Value and it can be used synced. One can set the selected Value with BlzFrameSetValue, but that will not trigger a FRAMEEVENT_POPUPMENU_ITEM_CHANGED event.

This fdf is a custom PopupMenu with a EditBox, the idea is to have 3 selectable options, based on the choosen option the player sets it's gold Lumber or food to insert value.

In Warcraft 3 V1.31.x the MENU of a "POPUPMENU" breaks when the "POPUPMENU" is placed outside of the 4:3 Screen.

 Code:

IncludeFile "UI/FrameDef/UI/EscMenuTemplates.fdf",

Frame "POPUPMENU" "MyPopupTemplate" {
 Width 0.19625,
 Height 0.03,
 PopupButtonInset 0.01, // -x offset for PopupArrowFrame from RIGHT of the POPUPMENU

 // Background Enabled
 ControlBackdrop "MyPopupTemplateBackdropTemplate",
 Frame "BACKDROP" "MyPopupTemplateBackdropTemplate" INHERITS "EscMenuButtonBackdropTemplate" {
 }

 // Background Disabled
 ControlDisabledBackdrop "MyPopupTemplateDisabledBackdropTemplate",
 Frame "BACKDROP" "MyPopupTemplateDisabledBackdropTemplate" INHERITS "EscMenuButtonDisabledBackdropTemplate" {
 }

 // Text markup for the current selected Text, also can be used with a FrameEvent to know when someone starts selecting.
 PopupTitleFrame "PopupMenuTitleTemplate",
 Frame "GLUETEXTBUTTON" "PopupMenuTitleTemplate" INHERITS WITHCHILDREN "EscMenuPopupMenuTitleTemplate" {
 }

 // the Arrow at the right
 PopupArrowFrame "PopupMenuArrowTemplate",
 Frame "BUTTON" "PopupMenuArrowTemplate" INHERITS WITHCHILDREN "EscMenuPopupMenuArrowTemplate" {
 }

 // The Container for the selectable options
 // actulay it is smarter to not define this in the Template.
 //PopupMenuFrame "TestPopupMenu",
 //Frame "MENU" "TestPopupMenu" INHERITS WITHCHILDREN "EscMenuPopupMenuMenuTemplate" {
//
// }
}

Frame "POPUPMENU" "TestPopup" INHERITS WITHCHILDREN "MyPopupTemplate" {
 // The Container for the selectable options
 PopupMenuFrame "TestPopupMenu",
 Frame "MENU" "TestPopupMenu" INHERITS WITHCHILDREN "EscMenuPopupMenuMenuTemplate" {
 // the selectable options
 // they will try to load a Localized String
 MenuItem "TestA", -2,
 MenuItem "TestB", -2,
 MenuItem "TestC", -2,
 MenuItem "TestD", -2,
 }
}

Frame "POPUPMENU" "CustomCommandsPopup" INHERITS WITHCHILDREN "MyPopupTemplate" {
 PopupMenuFrame "CustomCommandsPopupMenu",
 Frame "MENU" "CustomCommandsPopupMenu" INHERITS WITHCHILDREN "EscMenuPopupMenuMenuTemplate" {
 MenuItem "COLON_GOLD", -2,
 MenuItem "COLON_LUMBER", -2,
 MenuItem "COLON_FOOD", -2,
 }
}

Frame "EDITBOX" "CustomCommandsEditBox" INHERITS WITHCHILDREN "EscMenuEditBoxTemplate" {
 Width 0.15,
 Height 0.037,
 DecorateFileNames,
 FrameFont "MasterFont", 0.015, "",
 SetPoint TOPLEFT, "CustomCommandsPopup", TOPRIGHT, 0.0, 0.0,

 EditTextFrame "CustomCommandsEditBoxText",
 Frame "TEXT" "CustomCommandsEditBoxText" INHERITS "EscMenuEditBoxTextTemplate" {
 FrameFont "MasterFont", 0.015, "",
 }
}

This is the Lua code that creates the PopupMenu and multiple Editboxes.

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 BlzLoadTOCFile( "war3mapImported\\TestPop.toc" )
 local currentFrame = nil
 local popupFrame = BlzCreateFrame("CustomCommandsPopup", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI,0), 0, 0 )
 BlzFrameSetAbsPoint( popupFrame, FRAMEPOINT_TOPLEFT, 0.20, 0.30)

 local editBox = {}
 local editBoxTrigger = CreateTrigger()
 TriggerAddAction(editBoxTrigger, function()
 SetPlayerState(GetTriggerPlayer(), editBox[BlzGetTriggerFrame()], tonumber(BlzGetTriggerFrameText()))

 -- clear the box for the Active Player
 if GetLocalPlayer() == GetTriggerPlayer() then
 BlzFrameSetText(BlzGetTriggerFrame(), "")
 BlzFrameSetFocus(BlzGetTriggerFrame(), false)
 end
 end)

 local function add(playerState)
 local tempBox = BlzCreateFrame("CustomCommandsEditBox", popupFrame, 0, 0)
 table.insert(editBox, tempBox)
 BlzTriggerRegisterFrameEvent(editBoxTrigger, tempBox, FRAMEEVENT_EDITBOX_ENTER)
 editBox[tempBox] = playerState
 BlzFrameSetVisible(tempBox, false)
 end

 -- create 3 boxes one for Gold, Lumber and Food
 add(PLAYER_STATE_RESOURCE_GOLD)
 add(PLAYER_STATE_RESOURCE_LUMBER)
 add(PLAYER_STATE_RESOURCE_FOOD_CAP)

 add = nil

 local trigger = CreateTrigger()
 TriggerAddAction(trigger, function()
 if GetLocalPlayer() == GetTriggerPlayer() then
 -- hide current Box
 BlzFrameSetVisible(currentFrame, false)

 -- change current Box and show it
 currentFrame = editBox[BlzGetTriggerFrameValue() + 1]
 BlzFrameSetVisible(currentFrame, true)

 -- give that box the keyboard input focus
 BlzFrameSetFocus(currentFrame, true)
 end
 end)
 BlzTriggerRegisterFrameEvent(trigger, popupFrame, FRAMEEVENT_POPUPMENU_ITEM_CHANGED)
 end
end

 Code:

PopupArrowFrame
 Format: PopupArrowFrame FrameName,
 Example: PopupArrowFrame "AdvancedPopupMenuArrow",
 Purpose: Choose the BUTTON-Frame of the POPUPMENU

PopupButtonInset
 Format: PopupButtonInset real,
 Example: PopupButtonInset 0.01,
 Purpose:

PopupMenuFrame
 Format: PopupMenuFrame FrameName,
 Example: PopupMenuFrame "RacePopupMenuMenu",
 Purpose: Choose the MENU for this POPMENU, the Menu contains the chooseable options.

PopupTitleFrame
 Format: PopupTitleFrame FrameName,
 Example: PopupTitleFrame "EscOptionsShadowsPopupMenuTitle",
 Purpose: Choose a GLUETEXTBUTTON for the PopupMenu

//This Belongs

MENU's Fdf-Action. Menu is strongly connected to PopupMenu

 Code:

// Example from EscMenuTemplates
Frame "MENU" "EscMenuPopupMenuMenuTemplate" {
 DecorateFileNames,
 Height 0.03,
 FrameFont "EscMenuTextFont",0.011,"",
 MenuTextHighlightColor 1.0 0.0 0,
 MenuItemHeight 0.014,
 MenuBorder 0.009,

 ControlBackdrop "EscMenuPopupMenuMenuBackdropTemplate",
 Frame "BACKDROP" "EscMenuPopupMenuMenuBackdropTemplate" INHERITS "EscMenuButtonBackdropTemplate" {
 DecorateFileNames,
 BackdropBackground "EscMenuEditBoxBackground",
 }
}

 Code:

MenuBorder
 Format: MenuBorder real,
 Example: MenuBorder 0.01,
 Purpose:

MenuItem
 Format: MenuItem Text, integer,
 Example: MenuItem "ON", -2,
 Purpose: Adds an Selectable option to a MENU / PopupMenuFrame. The number is always -2 in Blizzards fdfs.

MenuItemHeight
 Format: MenuItemHeight real,
 Example: MenuItemHeight 0.014,
 Purpose: The height of one option of the MENU. The used fontsize should fit into this, otherwise you might see no displayed Text.

MenuTextHighlightColor
 Format: MenuTextHighlightColor red green blue alpha,
 Example: MenuTextHighlightColor 0.99 0.827 0.0705 1.0,
 Purpose:

##
 RadioGroup

RadioGroup binds checkboxes into a group together with ControlStyle "EXCLUSIVE", one can make sure only one of them is selected. Each player can have a different selected.

Example of a 4 checkbox RadioGroup

 Code:

IncludeFile "UI\FrameDef\UI\EscMenuTemplates.fdf",
Frame "RADIOGROUP" "RadioGroup" {
 ControlStyle "EXCLUSIVE",

 Frame "GLUECHECKBOX" "CheckBoxA" INHERITS WITHCHILDREN "EscMenuCheckBoxTemplate" {
 Width 0.012,
 Height 0.012,
 SetPoint CENTER, "ConsoleUI", CENTER, 0, 0,
 }
 Frame "GLUECHECKBOX" "CheckBoxB" INHERITS WITHCHILDREN "EscMenuCheckBoxTemplate" {
 Width 0.012,
 Height 0.012,
 SetPoint CENTER, "ConsoleUI", CENTER, 0.1, 0,
 }
 Frame "GLUECHECKBOX" "CheckBoxC" INHERITS WITHCHILDREN "EscMenuCheckBoxTemplate" {
 Width 0.012,
 Height 0.012,
 SetPoint CENTER, "ConsoleUI", CENTER, 0.15, 0,
 }
 Frame "GLUECHECKBOX" "CheckBoxD" INHERITS WITHCHILDREN "EscMenuCheckBoxTemplate" {
 Width 0.012,
 Height 0.012,
 SetPoint CENTER, "ConsoleUI", CENTER, 0.2, 0,
 }

 RadioItem "CheckBoxA",
 RadioItem "CheckBoxB",
 RadioItem "CheckBoxC",
 RadioItem "CheckBoxD",
}

 Lua:

do
 local real = MarkGameStarted

 function MarkGameStarted()
 real()
 BlzLoadTOCFile("war3mapimported\\Test.toc")

 local frame = BlzCreateFrame("RadioGroup", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 local trigger = CreateTrigger()
 TriggerAddAction(trigger, function()
 print("Click", BlzFrameGetName(BlzGetTriggerFrame()), GetHandleId(BlzGetTriggerFrameEvent()), BlzGetTriggerFrameValue())
 end)
 BlzTriggerRegisterFrameEvent(trigger, BlzGetFrameByName("CheckBoxA", 0), FRAMEEVENT_CHECKBOX_CHECKED)
 BlzTriggerRegisterFrameEvent(trigger, BlzGetFrameByName("CheckBoxB", 0), FRAMEEVENT_CHECKBOX_CHECKED)
 BlzTriggerRegisterFrameEvent(trigger, BlzGetFrameByName("CheckBoxC", 0), FRAMEEVENT_CHECKBOX_CHECKED)
 BlzTriggerRegisterFrameEvent(trigger, BlzGetFrameByName("CheckBoxD", 0), FRAMEEVENT_CHECKBOX_CHECKED)
 BlzTriggerRegisterFrameEvent(trigger, BlzGetFrameByName("CheckBoxA", 0), FRAMEEVENT_CHECKBOX_UNCHECKED)
 BlzTriggerRegisterFrameEvent(trigger, BlzGetFrameByName("CheckBoxB", 0), FRAMEEVENT_CHECKBOX_UNCHECKED)
 BlzTriggerRegisterFrameEvent(trigger, BlzGetFrameByName("CheckBoxC", 0), FRAMEEVENT_CHECKBOX_UNCHECKED)
 BlzTriggerRegisterFrameEvent(trigger, BlzGetFrameByName("CheckBoxD", 0), FRAMEEVENT_CHECKBOX_UNCHECKED)

 print("done")
 end

end

##
 SLIDER/Scrollbar

Slider/Scrollbar are interactive frames to set a value inside the bounds of a min and max value, the min and max are valid values. The user is able to set the value by draging the button of the Slider/Scrollbar or clicking inside the Slider/Scrollbar. Warcraft 3 has Vertical or Horizontal Sliders/Scrollbars.

Scrollbar is more a functional child feature, while Slider is better as a free code driven frame. The code api works great for Slider. Not so for Scrollbar.

None-Event API

 JASS:

native BlzFrameSetValue takes framehandle frame, real value returns nothing
native BlzFrameGetValue takes framehandle frame returns real
native BlzFrameSetMinMaxValue takes framehandle frame, real minValue, real maxValue returns nothing
native BlzFrameSetStepSize takes framehandle frame, real stepSize returns nothing

BlzFrameSetValue sets the current value for any player running this code to value, it will respect MinMax of the Slider. This will trigger one FRAMEEVENT_SLIDER_VALUE_CHANGED for every player running it. The event is triggered by other players even when the call happens inside a GetLocalPlayer block. Means if your Map has 8 users and don't run BlzFrameSetValue inside GetLocalPlayer you trigger the event 8 times.

BlzFrameGetValue returns the current value of the local player in the Slider, as it is interactive treat it as a dangerous async value.

Event-code API

 JASS:

constant frameeventtype FRAMEEVENT_MOUSE_WHEEL = ConvertFrameEventType(6)
constant frameeventtype FRAMEEVENT_SLIDER_VALUE_CHANGED = ConvertFrameEventType(13)

native BlzGetTriggerFrame takes nothing returns framehandle
native BlzGetTriggerFrameEvent takes nothing returns frameeventtype
native BlzGetTriggerFrameValue takes nothing returns real
constant native GetTriggerPlayer takes nothing returns player

FRAMEEVENT_SLIDER_VALUE_CHANGED is the event that happens when a player sets or got forced (BlzFrameSetValue) onto a value. This is a synced event it happens for all players with the value choosen by GetTriggerPlayer(). The event happens even when the value did not change (for example if the user drags the Button to the max it is very likely that this event happens multiple times in a row)

The event can be seen as GetTriggerPlayer() set BlzGetTriggerFrame() to BlzGetTriggerFrameValue(). if you have a Trigger with multiple frameEvents BlzGetTriggerFrameEvent() can tell you which frameEvent happened.

On default SLIDER do not have scroll by mouse wheel. But it is not much work to code it, when one uses a standard stepsize like 1. Then one does not even need any data maping.

Example code to make SLIDER scrollable by mouse wheel. The problem with this code is that it does not work when the user points at the SLIDER's Button.

The code would have to be placed into a Trigger Action/Condition with event FRAMEEVENT_MOUSE_WHEEL.

 Lua:

local frame = BlzGetTriggerFrame()
-- only set the value of the active player
if GetLocalPlayer() == GetTriggerPlayer() then
 -- Inside a Wheel Event +/- Value can be used to know the direction of rolling
 if BlzGetTriggerFrameValue() > 0 then
 BlzFrameSetValue(frame, BlzFrameGetValue(frame) + 1)
 else
 BlzFrameSetValue(frame, BlzFrameGetValue(frame) - 1)
 end
end

In case you don't care about SLIDER in fdf they are quite good createable without, as shown here: Slider without custom FDF

Slider/Scrollbar Fdf-Actions:

 Code:

 SliderInitialValue
 Format: SliderInitialValue integer,
 Example: SliderInitialValue 2,
 Purpose: The Slider starts with that value.

 SliderLayoutHorizontal
 Format: SliderLayoutHorizontal,
 Purpose: Default, Slider goes from Left(min) to Right(max).

 SliderLayoutVertical
 Format: SliderLayoutVertical,
 Purpose: The Slider goes from Bottom(min) to Top(max)

 SliderMaxValue
 Format: SliderMaxValue integer,
 Example: SliderMaxValue 100,
 Purpose: Default, 100. Max number chooseable

 SliderMinValue
 Format: SliderMinValue integer,
 Example: SliderMinValue 0,
 Purpose: Default, 0. Min number chooseable

 SliderStepSize
 Format: SliderStepSize integer,
 Example: SliderStepSize 1,
 Purpose: How accurate the user can choose values.

 SliderThumbButtonFrame
 Format: SliderThumbButtonFrame "FrameName",
 Example: SliderThumbButtonFrame "EscMenuScrollThumbButton",
 Purpose: The BUTTON-Frame used to move the SLIDER/SCROLLBAR Value.

 ScrollBarDecButtonFrame
 Format: ScrollBarDecButtonFrame "FrameName",
 Example: ScrollBarDecButtonFrame "EscMenuScrollBarDecButton",
 Purpose: Choose a BUTTON handling the Dec click.

 ScrollBarIncButtonFrame
 Format: ScrollBarIncButtonFrame "FrameName",
 Example: ScrollBarIncButtonFrame "StandardScrollBarIncButton",
 Purpose: Choose a BUTTON handling the Inc click.

##
 SPRITE

SPRITE is a non-Interactive frame displaying a warcraft 3 model and loops it's animation.

Changing the Parent of a Sprite during the game can bug, speeding up the displayed animation.

 Code:

 BackgroundArt
 Format: BackgroundArt FilePath,
 Example: BackgroundArt "UI\Glues\ScoreScreen\ScoreScreen-TopBar\ScoreScreen-TopBar.mdl",

 SpriteCamera
 Format: SpriteCamera int,
 Example: SpriteCamera 0,

 SpriteScale
 Format: SpriteScale x x x,
 Example: SpriteScale 0.1 0 0,
 Purpose: Scales the used Model. The 2. and 3. number are fake and don't seem to do anything.

 LayerStyle "NOSHADING",

##
 STATUSBAR

STATUSBAR is a non-Interactive frame displaying a warcraft 3 model. Unlike Sprite it does not loop the animation but the current animationTime can be set using BlzFrameSetValue as % of Min/Max to AnimationTime.

It is said "STATUSBAR" only supports 1s animations, sequzenzes after 1s are droped.

TriggerHappy encountered a situation in which units using the same model as the STATUSBAR were affected by the Bar's current animation.

This type is quite good createable with code only ModelBar without FDF.

Example of a Text used as Button With Cooldown in fdf using StatusBar for the cooldown. It is a Text to not keep focus when clicked.

 Code:

Frame "TEXT" "ButtonWithCooldown" {
 Width 0.024,
 Height 0.024,
 ControlStyle "AUTOTRACK|HIGHLIGHTONMOUSEOVER|CLICKONMOUSEDOWN",
 Text "1", // need a Text to be rendered

 ControlBackdrop "ButtonWithCooldownBackdrop",
 Frame "BACKDROP" "ButtonWithCooldownBackdrop" {
 }

 ControlDisabledBackdrop "ButtonWithCooldownBackdropDisabled",
 Frame "BACKDROP" "ButtonWithCooldownBackdropDisabled" {
 }

 ControlPushedBackdrop "ButtonWithCooldownBackdropPushed",
 Frame "BACKDROP" "ButtonWithCooldownBackdropPushed" {
 BackdropBackgroundInsets 0.002 0.002 0.002 0.002,
 }

 ControlMouseOverHighlight "ButtonWithCooldownHighLight",
 Frame "HIGHLIGHT" "ButtonWithCooldownHighLight" {
 HighlightType "FILETEXTURE",
 HighlightAlphaFile "UI\Glues\ScoreScreen\scorescreen-tab-hilight.blp",
 HighlightAlphaMode "ADD",
 }
 Frame "STATUSBAR" "ButtonWithCooldownSprite" {
 SetAllPoints,
 DecorateFileNames,
 StatusBarSprite "CommandButtonCooldown",
 LayerStyle "IGNORETRACKEVENTS",
 SpriteScale 0.6153 0 0, // 0.024/0.039
 //Alpha 200,
 }
}

Lua code using the code above

 Lua:

function TestButtonWithCooldown()
 BlzLoadTOCFile("war3mapImported/test.toc")
 local frame = BlzCreateFrame("ButtonWithCooldown", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
 BlzFrameSetTexture(BlzGetFrameByName("ButtonWithCooldownBackdrop", 0), BlzGetAbilityIcon(FourCC'Hpal'), 0, false)
 BlzFrameSetTexture(BlzGetFrameByName("ButtonWithCooldownBackdropPushed", 0), BlzGetAbilityIcon(FourCC'Hpal'), 0, false)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.4, 0.3)
 local max = 20
 local remainCd = max
 BlzFrameSetMinMaxValue(BlzGetFrameByName("ButtonWithCooldownSprite", 0), 0, max)
 TimerStart(CreateTimer(), 0.1, true, function()
 remainCd = remainCd - 0.1
 BlzFrameSetValue(BlzGetFrameByName("ButtonWithCooldownSprite", 0), max - remainCd)
 end)
end

##
 TextArea

TEXTAREA is a frame to display scrollable text. Normaly TextAreas have a BACKDROP showing a box and a Background.

TextAreas require a scrollbar. The scrollbar is at the right side of the TextArea and blocks a part of the space given to the TextArea. If the TextArea is big enough to display the whole text, the scrollbar is not shown. Even when the scrollbar is not shown it takes its space. For not scrollable Text one could simply use a TEXT-Frame.

##
 Built in TextArea

Warcraft 3 has 2 builtIn TEXTAREA mainframes.

BattleNetTextAreaTemplate

EscMenuTextAreaTemplate

They are a good base to inherit, copy or one can just create them. If one wants to create one of them one would have to load them first using BlzLoadTOCFile.

The fdfs containing them are located at:

UI\Frames\framedef\glue\battlenettemplates.fdf

UI\Frames\framedef\ui\escmenutemplates.fdf

##
 Creating builtin TextArea

This jass Code creates both TextAreas set the poistions, the size and fills them with text. It is quite important to give a TEXTAREA a size being big enough to display the scrollbar otherwise the game crashs on the attempt to display the TEXTAREA. The Height of the TextArea should be atleast 0.03. That is quite small, but if one forgets to set the size without having a default size, it will be 0 and the game crashs.

 JASS:

function CreateDefaultTextAreas takes nothing returns nothing
 local framehandle textArea = BlzCreateFrame("BattleNetTextAreaTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0),0,0)
 local framehandle textArea2 = BlzCreateFrame("EscMenuTextAreaTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0),0,0)
 local integer index = 1
 call BlzFrameSetAbsPoint(textArea, FRAMEPOINT_TOPLEFT, 0.1, 0.5)
 call BlzFrameSetAbsPoint(textArea2, FRAMEPOINT_TOPLEFT, 0.5, 0.5)
 call BlzFrameSetSize(textArea, 0.25, 0.25)
 call BlzFrameSetSize(textArea2, 0.25, 0.25)
 loop
 exitwhen index == 120
 call BlzFrameAddText(textArea, I2S(index)+": Test Text")
 call BlzFrameAddText(textArea2, I2S(index)+": Test Text")
 set index = index + 1
 endloop
endfunction

//===========================================================================
function InitTrig_Create_TextAreaTemplates takes nothing returns nothing
 call BlzLoadTOCFile( "war3mapImported\\Templates.toc" )
 call TimerStart(CreateTimer(), 0.0, false, function CreateDefaultTextAreas)
endfunction

##
 Seting text of a TextArea

To set the text of a textarea one uses BlzFrameAddText(textArea, newLine) or BlzFrameSetText(textArea, totalText). BlzFrameAddText adds a line Seperator before the new added text, if the TextArea already contained text.

For a TextArea BlzFrameGetText seems to be quite fugitive, after 1 second it returned null, while it was returning the total text in the same function as the text was set. I didn't tested this much.

TextArea can display colored Text using warcraft colorformat |cffff0000.

##
 Screen-Space and Scrolling

On Default a TextArea will catch the screen-space given to it, making it unclickable for the playable world and sending mousewhell rolls to its scrollbar. One could disable the TextArea to stop the TextArea from taking the mouse Input which would make the screenspace useable by the playable game again, but that would also disable the mouse whell roll sending. One could still scroll when the mouse points directly to the scrollbar.

Disabling is done with:

call BlzFrameSetEnable(textArea, false)

##
 TEXTAREA-FDF

This is a custom TEXTAREA named MyTextArea which has some comments describing the result of fields of TEXTAREA inside fdf:

TEXTAREA rejects most TEXT font Settings one might know when one used TEXT-Frame. Hence the text appereance can't be changed much.

 Code:

IncludeFile "UI\FrameDef\UI\EscMenuTemplates.fdf", //Frames from that fdf can be inherited in this file
IncludeFile "UI\FrameDef\UI\QuestDialog.fdf",

Frame "TEXTAREA" "MyTextArea" {
 DecorateFileNames,
 FrameFont "InfoPanelTextFont", 0.020, "",

 TextAreaLineHeight 0.018, //Should fit used font Size, a high difference shows no text or much space between lines.

 TextAreaLineGap 0.00, //adds that much space between 2 Lines, addtional to TextAreaLineHeight.

 TextAreaInset 0.035, //Moves the Text and the scrollbar into the middle from all sides by this amount. This increases the min size for game crash.

 TextAreaMaxLines 20, //Only that amount of lines are displayed, if more lines are added oldest are removed.

 TextAreaScrollBar "MyTextAreaScrollBar",
 Frame "SCROLLBAR" "MyTextAreaScrollBar" INHERITS WITHCHILDREN "EscMenuScrollBarTemplate" {
 }

 ControlBackdrop "MyTextAreaBackdrop",
 Frame "BACKDROP" "MyTextAreaBackdrop" INHERITS "QuestButtonBaseTemplate" {
 }
}

jass Code creating MyTextArea from above.

 JASS:

function CreateMyTextArea takes nothing returns nothing
 local framehandle textArea = BlzCreateFrame("MyTextArea", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0),0,0)
 local integer index = 1
 call BlzFrameSetAbsPoint(textArea, FRAMEPOINT_CENTER, 0.4, 0.3)
 call BlzFrameSetSize(textArea, 0.25, 0.25)
 loop
 exitwhen index == 120
 call BlzFrameAddText(textArea, I2S(index)+": Test Text")
 set index = index + 1
 endloop
endfunction
//===========================================================================
function InitTrig_Create_MyTextArea takes nothing returns nothing
 call BlzLoadTOCFile( "war3mapImported\\MyTextArea.toc" )
 call TimerStart(CreateTimer(), 0.0, false, function CreateMyTextArea)
endfunction

QuestBackdrop

EscMenuBackdrop

##
 Inherited TextArea

If one is overall fine with a TextArea and only wants some small changes, like a different textsize. Then one could inherit from the wanted TextArea and mention only the changed textsize. That way one can have small (in definition) , powerful and still valid frames.

That could look like this

 Code:

IncludeFile "UI\FrameDef\glue\battlenettemplates.fdf", //Frames from that fdf can be inherited in this file

Frame "TEXTAREA" "BigBnetTextArea" INHERITS WITHCHILDREN "BattleNetTextAreaTemplate" {
 DecorateFileNames,
 FrameFont "MasterFont", 0.020, "",
 TextAreaLineHeight 0.015,
}

"BigBnetTextArea" is basicly "BattleNetTextAreaTemplate" but has a bigger TextSize and the lines are bigger, to fit better with the increased textsize.

To inherit a Frame's childFrames, one adds WITHCHILDREN after INHERITS.

Codewise one can not access inherited childFrames using BlzGetFrameByName. With the release of the native BlzFrameGetChild in V1.32.6 one is able to get the inherited childFrames during the runtime, in 1.32.6 this native ignores String&Texture childFrames.

##
 TEXT

TEXT is the FrameType for Frames that has multiple uses, it displays text on the screen or it styles the text of a more advanced Frame. Unlike its equal String for SimpleFrames, TEXT can have various Frameevents and will take control of the Screenspace taken by it's size or it's displayed text. If one does not want that one can either disable the TEXT-Frame with BlzFrameSetEnable(frame, false) by code or add the fdfaction LayerStyle "IGNORETRACKEVENTS",. LayerStyle can not be reverted during the game but BlzFrameSetEnable might change the color when the TEXT-Frame has set colors for a disabled state.

When it is used as style, it is defined as functional child-Frame in fdf. Like shown below.

 Code:

ButtonText "HelpButtonText",
Frame "TEXT" "HelpButtonText" INHERITS "EscMenuButtonTextTemplate" {
 Text "KEY_HELP",
}

TextMarkUP String & TEXT

##
 SimpleFrames

String

Texture

TextureFraction

Layer

SimpleButton

SimpleStatusBar

SimpleCheckBox

If you don't know how to use a custom toc-File, therefore can't use a custom fdf you should first do a tutorial about toc: UI: toc-Files.

SimpleFrames are another group of frames. They are low in amount of types but frames of this group manage most of the direct ingame UI, like the unitinfo, the ability and item buttons.

Unlike Frames, Simpleframes require fdf and can not be created in a useful way without such a fdf defining a SimpleFrame first. All SimpleFrames can be moved out of the 4:3 screen. Most custom created Simpleframes do not fight for mousecontrol. For the displaying SimpleFrames are below Frames.

If I write SIMPLEFRAME in uppercase only, i talk about this specific type otherwise i address the group.

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

Inside a fdf, such String/Texture can be defined outside of Simpleframes to have a base to inherit from. But one can create them only as part of a Simpleframe during the game.

String/Texture-Frames do not contain UseActiveContext/DecorateFileNames, they use it when their Parent uses it.

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

BlzDestroyFrame

###
 Anchor

SimpleFrames have another keyword to pos child frames Anchor. Anchor is a shortcut for: "SetPoint pointA, parent, pointA, x, y,". One can still use the other 2; SetPoint and SetAllPoints.

Anchor TOPLEFT, 0.1, 0.05,

pos the own TOPLEFT to the parents TOPLEFT with 0.1 x and 0.05 y offset.

Anchor as the others SetPoint require the attached frame to take a part of the screen by having atleast Width or Height or both.

Otherwise it can happen that the Texture/String attaches itself to the parent of the parent.

Without a point the String/Texture will center itself to the parent or that ones parent (which can be the 4:3 Screen when SimpleFrames are created for ORIGIN_FRAME_GAME_UI), if the direct parent fails.

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

Using this font native with a fontsize differing much from the initial fontsize can break the displayed text.

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

###
 TextureFraction

Textures have a feature called TexCoord with it they take only a part of an given image and display that over the space the Texture takes. The TexCoord setting is kept even when changing the current image by code. TextCoord has nothing to do with the space taken on the screen.

Format: TexCoord Left, Right, Up, Bottom

TexCoord 0, 1, 0, 0.125, (take the total x; y-wise 0 to 12.5% from the top of the Image-File)

TexCoord 0.0, 0.5, 0.0, 0.5, (the topLeft part) see image below only a fraction of the Paladin icon.

One can also produce a Tile using TextCoord, to fill the Frame's given screen space with copies of that image. This is also done with TexCoord, by giving numbers higher than 1 for right and bottom.

TexCoord 0, 2, 0, 2,

A Texture can be rotated by 180 degree by placing TOPLEFT & TOPRIGHT in reversed positions (Topleft where Topright should be and Topright where TopLeft should be).

[attachment 390235](https://www.hiveworkshop.com/attachments/texture-rotated-jpg.390235/)

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

Layer are a fdf feature for Simpleframes that can be used to setup the order of displaying. Inside a Layer only String and Texture can be defined. Blizzard used 2 Layers in their fdf for warcraft 3.

Layer "BACKGROUND" {...

Layer "ARTWORK" {...

Layer "ARTWORK_OVERLAY" {...

"ARTWORK" is displayed above "BACKGROUND".

Layer "BACKGROUND" of a child SIMPLEFRAME is above the Layer "ARTWORK" of the parent.

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

This would be the definition for an simple IconButton inside fdf:

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

##
 SimpleStatusBar

A SIMPLESTATUSBAR is a simple frame that shows a texture partly (from left to right) based on the SIMPLESTATUSBAR current value vs its max value. The Texture is stretched to the total frames size. On default the value can be from 0 to 100 (0 beeing empty, 100 full). 40 of 100 would show 40% of the Image. A SIMPLESTATUSBAR can not have any Frameevents and does not block mouse clicks.

##
 Functions

Codewise there are 3 functions handling the bar's Value.

 JASS:

native BlzFrameSetValue takes framehandle frame, real value returns nothing
native BlzFrameGetValue takes framehandle frame returns real
native BlzFrameSetMinMaxValue takes framehandle frame, real minValue, real maxValue returns nothing

BlzFrameSetValue sets the current value.

BlzFrameGetValue gets the local current value

BlzFrameSetMinMaxValue sets the min max values of the bar.

You also might use BlzFrameSetTexture takes framehandle frame, string texFile, integer flag, boolean blend to change the used textures during the game.

It is also possible to use a white Texture and change the color of the bar using









 JASS:

BlzFrameSetTexture(bar, "ui\\feedback\\progressbar\\human-statbar-color", 0, true)
BlzFrameSetVertexColor(bar, BlzConvertColor(255, 255, 255, 1))

 The BlzFrameSetVertexColor line crashed for me in 1.31.1, but work fine in 1.32.1 (reforged).

To bring a custom SIMPLESTATUSBAR into your game one creates one. This either can be done by defining one in fdf, loading it and then creating or by code only

Instead of Borders&Backgrounds one could use CommandButton-Icons. To for examle show Arthas corruption state or to show cooldowns.

##
 The Frame Definition File

There is no mainframe of type SIMPLESTATUSBAR loaded on default, hence not createable without unwanted other frames. Therefore one has to write a custom one and load that custom one over a custom written toc.

By adding Additional SimpleFrames one can have a border an overlay text and adding a background blp texture. The barTexture is above the background and will hide more of it as nearer the value is to the max value.

 Code:

 BarTexture
 Format: BarTexture "TexturePath",
 Purpose: Define the Texture filling the Frame from left to right, based on the value of the SIMPLESTATUSBAR.

// A "SIMPLESTATUSBAR" using the progress Textures as filling texture with a Black Background. The color of the "CustomBar" has to be changed after it's creation.
Frame "SIMPLESTATUSBAR" "CustomBar" {
 Width 0.07,
 Height 0.012,
 BarTexture "ui\feedback\progressbar\human-statbar-color",
 Layer "BACKGROUND" {
 Texture "CustomBarBackground" {
 File "textures\black32",
 }
 }
}

String "MyBarTextTemplate" {
 //FontColor 0.99 0.427 0.0705 1.0, //Red Green Blue Alpha 0.0 to 1.0
 FontColor 1.0 1.0 1.0 1.0,
 FontShadowColor 0.0 0.0 0.0 0.9,
 FontShadowOffset 0.001 -0.001,
 Font "MasterFont", 0.01, //MasterFont is only valid with "DecorateFileNames,"
}
Frame "SIMPLESTATUSBAR" "MyBarEx" {
 Width 0.07, //Default Width
 Height 0.012, //Default Height
 BarTexture "Replaceabletextures\Teamcolor\Teamcolor00", //Default BarTexture
 Layer "BACKGROUND" { //A simpleFrames thing, where this is placed layer wise
 Texture "MyBarExBackground"{ //the BACKGROUND Texture named "MyBarExBackground" its also a frame and a child
 File "textures\black32", //Default Texture for "MyBarExBackground"
 }
 }
 Frame "SIMPLEFRAME" "MyBarExFrame" { //Child of "MyBarEx"
 DecorateFileNames, //Lookup FileNames in a StringList
 SetAllPoints, //copy "MyBarEx"
 Layer "ARTWORK" {
 Texture "MyBarExBorder" {
 File "SimpleXpBarBorder", //Default Texture for "MyBarExBorder"
 }
 String "MyBarExText" INHERITS "MyBarTextTemplate" {
 Text "MyBarEx", //Default Text for "MyBarExText" which takes over Data from "MyBarTextTemplate"
 }
 }
 }
}

Frame "SIMPLESTATUSBAR" "MyBar" {
 Width 0.07,
 Height 0.012,
 BarTexture "Replaceabletextures\Teamcolor\Teamcolor00",
 Layer "BACKGROUND" {
 Texture "MyBarBackground" {
 File "Replaceabletextures\Teamcolor\Teamcolor27",
 }
 }
 Frame "SIMPLEFRAME" "MyBarFrame" {
 SetAllPoints,
 DecorateFileNames,
 Layer "ARTWORK" {
 String "MyBarText" INHERITS "MyBarTextTemplate" {
 Text "MyBar",
 }
 }
 }
}
Frame "SIMPLESTATUSBAR" "MySimpleBar" {
 Width 0.07,
 Height 0.012,
}
}

Thats the fdf text for 3 SIMPLESTATUSBAR all beeing mainFrames. MyBarEx has a border while MyBar does not. The last has no additional features (background, border, text).

Without the border it is more suited for Faces.

ui\feedback\buildprogressbar\human-statbar-color3

One also could use the Playercolors:

replaceabletextures\teamcolor\teamcolor00

replaceabletextures\teamcolor\teamcolor01

...

replaceabletextures\teamcolor\teamcolor10

...

replaceabletextures\teamcolor\teamcolor27

SIMPLECHECKBOX: I wasn't able to attach events nor get the value in any way last time I tested it (V1.31PTR) so it is kinda useless right now. Therefore I won't have an explanition here.

##
 TextMarkUP String & TEXT

Inside fdf one can setup the color of a String/TEXT Frame with some fdfActions. But it is also possible to color Text with the warcraft 3 color codes when setting the text as prefix prefix |cff0021ff.

 Code:

 FontColor
 Format1: FontColor red green blue alpha,
 Format2: FontColor red green blue,
 Example: FontColor 1.0 0.201 1.0 1.0,
 Purpose: Color of the text in enabled state. Can be skiped in which case the Text will be white. Alpha is 1.0 if skiped.

 FontDisabledColor
 Format1: FontDisabledColor red green blue alpha,
 Format2: FontDisabledColor red green blue,
 Purpose: Like FontColor but for disabled Frames. Skipable.

 FontHighlightColor
 Format1: FontHighlightColor red green blue alpha,
 Format2: FontHighlightColor red green blue,
 Purpose: Like FontColor but for Frames that have focus. Skipable.

 FrameFont
 Format: FrameFont FontName, size, text,
 Example: FrameFont "MasterFont", 0.011, "",
 Purpose: used by Frames, MasterFont is valid when in the same Frame DecorateFileNames is used otherwise you write the filePath.

 Font
 Format: Font FontName, size,
 Example: Font "InfoPanelTextFont", 0.009,
 Purpose: Styles the text of a String the Text-Child Frame of SIMPLEFRAME.

TEXT/String can have dynamic height with automatic new Line wraps by seting one Point and using BlzFrameSetSize(x, 0). It is told that the Relative position does not update good, if that happens setting the Size again fixes it.

##
 Fonts

Warcraft 3 (Reforged V1.32.2) has fonts for all Languages found inside war3.w3mod:fonts refered by you just with "Fonts\name":

2002.ttf

2002b.ttf

arheigb_bd.ttf

arheiu20m.ttf

arheiu20_md.ttf

arlishuu20_db.ttf

arlishuu30_md.ttf

bl.ttf

blizzardglobal-v5_4.ttf

blq55.ttf

blq55web.ttf

blq85web.ttf

dffn_b31.ttf

dfheilt.ttf

dfheimd.ttf

dfkgothicmd.ttf

dfst-m3u.ttf

frizqt__.ttf

ghei00b_t.ttf

inconsolata-regular.ttf

nimrodmt.ttf

nim_____.ttf

thowr___.ttf

tt5500m_.ttf

ulei00m.ttf

fonts\dfst-m3u.ttf

fonts\nim_____.ttf

fonts\thowr___.ttf

fonts\tt5500m_.ttf

from _locales

fonts\dffn_b31.ttf

fonts\dfheimd.ttf

fonts\frizqt__.ttf

Inside fdf the path to them is not used (by Blizzard, you can do that if you want) instead variables defined in war3.w3mod:ui\war3skins.txt are.

If you want to use such a font Variable in fdf the same Frame needs the FdfAction DecorateFileNames,.

(From warcraft 3 version 1.32) This fonts can also be used in code when using the function SkinManagerGetLocalPath. For example: SkinManagerGetLocalPath("MasterFont") to get the filepath "Fonts\BLQ55Web.ttf"

From that file (From Warcraft 3 V1.32.2)[Box]

// fonts can be optionally overriden by locale

// Each font property will first be queried with the <property>_<locale> and the game will use that version if it exists.

// If the locale specific property does not exist it will fallback to the property name without the locale tag.

// For example to override the master font for korean you would add a new entry

// MasterFont:koKR=Fonts\DFHeiMd.ttf

//

// fonts - default

MasterFont=Fonts\BLQ55Web.ttf

MasterFontBold=Fonts\BLQ85Web.ttf

MessageFont=Fonts\BLQ55Web.ttf

ChatFont=Fonts\BLQ55Web.ttf

EscMenuTextFont=Fonts\BLQ55Web.ttf

InfoPanelTextFont=Fonts\BLQ55Web.ttf

TextTagFont=Fonts\BLQ55Web.ttf[/box]

In 1.31.1 the locals could have own Fonts and individual war3skins.txt-Files. Like war3.w3mod:_locales\dede.w3mod:ui\war3skins.txt

##
 Text Shadow/Color

TEXT and String Frames can have a TextShadow this can only be set in fdf with this actions:

 Code:

 FontShadowColor
 Format1: FontShadowColor red green blue alpha,
 Format2: FontShadowColor red green blue,
 Example: FontShadowColor 0.0 0.0 0.0 0.9,
 Purpose: Color of the Text's shadow. Alpha is 1.0 if skiped.

 FontShadowOffset
 Format: FontShadowOffset x y,
 Example: FontShadowOffset 0.001 -0.001,
 Purpose: This is a relative offset to the default Text.

##
 FontFlags

FontFlags alter the display of text given. One can combine multiple Flags with | like this: FontFlags "IGNORECOLORCODES|IGNORENEWLINES",.

The effect of FontFlag is kept after using BlzFrameSetText onto such a Frame.

 Code:

Format: FontFlags text,

Example: FontFlags "PASSWORDFIELD",
Purpose: The text will be displayed only with *

Example: FontFlags "IGNORECOLORCODES",
Purpose: |cffff00a0 is not parsed and displayed on the screen text

Example: FontFlags "IGNORENEWLINES",
Purpose: |n & \n have no effect and are displayed on the screen

Example: FontFlags "NONPROPORTIONAL",
Purpose: monospaced font (same amount of horizontal space)

Example: FontFlags "NOWRAP",
Purpose: Displays only the text fitting into the first Line

Example: FontFlags "FIXEDSIZE",
Purpose:

Example: FontFlags "FIXEDCOLOR",
Purpose:

Example: FontFlags "HIGHLIGHTONMOUSEOVER",
Purpose:

Example fdf

 Code:

Frame "TEXT" "MediumTextColor" {
 DecorateFileNames,
 FrameFont "MasterFont", 0.015, "",
 Text "|cffff00a0Test123Text|r|nAber Mein Ha13 ist besser",
 FontFlags "IGNORECOLORCODES",
 SetPoint CENTER, "ConsoleUI", CENTER, 0, -0.06,
}
Frame "TEXT" "MediumTextNewLine" {
 DecorateFileNames,
 FrameFont "MasterFont", 0.015, "",
 Text "|cffff00a0Test123Text|r|nAber Mein Ha13 ist besser",
 FontFlags "IGNORENEWLINES",
 SetPoint CENTER, "ConsoleUI", CENTER, 0, -0.09,
}

Frame "TEXT" "MediumTextNewLine2" {
 DecorateFileNames,
 Width 0.1,
 Height 0.1,
 FrameFont "MasterFont", 0.01, "",
 Text "|cffff00a0Test123Text|r Aber Mein Ha13 ist besser",
 FontFlags "NOWRAP",
 SetPoint TOPRIGHT, "ConsoleUI", TOPRIGHT, -0.1, -0.09,
}
Frame "TEXT" "MediumTextProport" {
 DecorateFileNames,
 Width 0.1,
 Height 0.1,
 FrameFont "MasterFont", 0.01, "",
 Text "Aber Mein Ha13 ist besser",
 FontFlags "NONPROPORTIONAL",
 SetPoint TOPLEFT, "ConsoleUI", TOPLEFT, 0.1, -0.09,
}

##
 Text-Alignment

TextAlignment is only important when an text is given a rect. inside Fdf the Horzion and vertical alignment is seperated:

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

in Code it is one native:

BlzFrameSetTextAlignment takes framehandle frame, textaligntype vert, textaligntype horz

###
 vertical

TEXT_JUSTIFY_TOP

TEXT_JUSTIFY_MIDDLE

TEXT_JUSTIFY_BOTTOM

###
 horizontal

TEXT_JUSTIFY_LEFT

TEXT_JUSTIFY_CENTER

TEXT_JUSTIFY_RIGHT

When an text is given only a Point, it will be a single line text. In such a case the type of point used can defines the direction the text will extend to with having more text chars.

With TOP, CENTER and BOTTOM the text will extend equal to both sides, the given point is the center.<|>

With TOPLEFT, LEFT and BOTTOMLEFT the text will extend to the right. |>

With RIGHT, TOPRIGHT and RIGHT the text will extend to the left. <|

This can be important when having a TEXT-Frame near the 4:3 border, when the text expands into the 16:9 area a part of the text of the TEXT-Frame is not displayed.

In the Image the BACKDROP has an bigger size then the TEXT-Frames so one can see the rects border and text clearly.

This code creates the shown image

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
--the displayed texts
 local texts = {
 "TOP LEFT",
 "TOP Center",
 "TOP RIGHT",
 "Middle LEFT",
 "Middle Center",
 "Middle RIGHT",
 "Bottom LEFT",
 "Bottom Center",
 "Bottom RIGHT"
 }
 --the text justifiers
 local justify = {
 {TEXT_JUSTIFY_TOP, TEXT_JUSTIFY_LEFT},
 {TEXT_JUSTIFY_TOP, TEXT_JUSTIFY_CENTER},
 {TEXT_JUSTIFY_TOP, TEXT_JUSTIFY_RIGHT},
 {TEXT_JUSTIFY_MIDDLE, TEXT_JUSTIFY_LEFT},
 {TEXT_JUSTIFY_MIDDLE, TEXT_JUSTIFY_CENTER},
 {TEXT_JUSTIFY_MIDDLE, TEXT_JUSTIFY_RIGHT},
 {TEXT_JUSTIFY_BOTTOM , TEXT_JUSTIFY_LEFT},
 {TEXT_JUSTIFY_BOTTOM , TEXT_JUSTIFY_CENTER},
 {TEXT_JUSTIFY_BOTTOM , TEXT_JUSTIFY_RIGHT}
 }

 local back = BlzCreateFrame("QuestButtonBaseTemplate",BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0),0, 0)
 BlzFrameSetSize(back, 0.28, 0.28)
 BlzFrameSetAbsPoint(back, FRAMEPOINT_CENTER, 0.4, 0.3)

 for index = 1, 9 do
 local textFrame = BlzCreateFrameByType("TEXT", "", back,"",0)
 BlzFrameSetAbsPoint(textFrame, FRAMEPOINT_CENTER, 0.4, 0.3)
 BlzFrameSetSize(textFrame, 0.25, 0.25)
 BlzFrameSetTextAlignment(textFrame, justify[index][1], justify[index][2])
 BlzFrameSetText(textFrame, texts[index])
 end
 end
end

##
 Errors

Show/hide onto a null Frame can crash the current running Thread.

Creating a Frame with a empty framehandle variable as parent leads to dc when an framenative is used onto it.

BlzFrameGetChild does not check for out of bounce Exceptions.

Some Frames do not exist until the local game requires them.

Using BlzFrameSetTooltip twice with the same combination crashes the game when the Frame is hovered.

Can't Undo BlzFrameSetTooltip.

BlzFrameSetFont does only work onto String-SimpleFrame.

BlzFrameSetFont onto SimpleFrame crashs the game.

String and Textures crash the game when you use some FrameNatives onto them.

non SimpleFrames used as Tooltip that could leave 4:3 sometimes are limited to 4:3. This can be avoided by creating an empty FRAME parent for the tooltip which is used as Tooltip.

Using BlzFrameSetParent to make a Parent the child of it's offspring crashes the game with a endless loop

In Warcraft 3 V1.31.x the MENU of a "POPUPMENU" breaks when the "POPUPMENU" is placed outside of the 4:3 Screen.

Move/Size HeroBar -> repos HeroButtons.

Using Allianz Dialog and it's children can desync replays, because they do only exist in mp but not in the replay and therefore result into a dismatching handleId-Block.

Warcraft 3's Save&Load does not include TOC UI-Frame/API usage, worse using loaded UIframe-variables in a UI-frame native will crash the game.

in 1.32.6 exists a bug. BlzGetOriginFrame does not return a valid frame (GetHandleId 0) when one got the expected originframe over BlzFrameGetChild first.

Creating Frames of Type "BACKDROP", "TEXTAREA", "SIMPLEMESSAGEFRAME", "DIALOG" or "CONTROL" can crash the game, if they have a wrong setup.

The game can crash when moving/resizing ORIGIN_FRAME_CHAT_MSG/ORIGIN_FRAME_UNIT_MSG/ORIGIN_FRAME_TOP_MSG using bad values.

Some Frames do not exist until the local player did something, this can lead to various problems upto desync.

Some Frames are not created until the local player tabed into the game when it was minimized (fullscreen mode).

Reading a not closed StringList will crash the game.

BlzFrameSetParent does not remove the frame from the oldParent child List it then is children of both. For simpleframes BlzFrameSetParent works correct, 1 parent only (2025 NOV 26)

When you change parent of a simpleframe with a hidden parent, the simpleframe is hidden until the new parent is hidden & shown. (2025 NOV 26)

BlzFrameSetScale can break a custom Font, do the scaling later not right at creation.

#### Attachments

Templates Imported.jpg

 35.6 KB

 · Views: 7,377



Img a page 1 cut.jpg

 86.1 KB

 · Views: 7,836



Img a page 2 cut.jpg

 58 KB

 · Views: 8,061



Img a page 3 cut.jpg

 51.2 KB

 · Views: 7,730



Wierd Childframe Order.jpg

 195.7 KB

 · Views: 8,972



Child Order2.jpg

 25.6 KB

 · Views: 8,532



FrameName.jpg

 15.9 KB

 · Views: 7,724



PlayerColor Bars.jpg

 56.9 KB

 · Views: 6,385



XpBar.jpg

 3.6 KB

 · Views: 6,324



PriestBar.jpg

 4 KB

 · Views: 6,369



XpBar Colored.jpg

 2.4 KB

 · Views: 6,386



Button with AutoCast and RockBolt Sprite.jpg

 11.2 KB

 · Views: 6,559



ImportToc.jpg

 103.9 KB

 · Views: 3,635



 Last edited:
 Sunday at 8:56 AM

Home

Pastebin

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top