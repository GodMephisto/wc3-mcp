---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-reading-a-fdf.315850/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Reading a Fdf
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-reading-a-fdf.315850/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

##
 Introduction

A fdf (Frame definition file) is an fileType used in warcraft 3 to define information and default behaviour of an UI-Frame. In this tutorial I want to tell you how to get frames by reading a fdf.

​

##
 FrameTypes

From the FrameType one can say much about what this frame will do in most cases. This is a list of FrameTypes found in the default fdf of warcraft 3.

 Code:

BACKDROP
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
MENU
MODEL
POPUPMENU
SCROLLBAR
SIMPLEBUTTON
SIMPLECHECKBOX
SIMPLEFRAME
SIMPLESTATUSBAR
SLASHCHATBOX
SLIDER
SPRITE
TEXT
TEXTAREA
TEXTBUTTON
TIMERTEXT

"GLUE" Frame variants have builtin onpress Feedback-sounds, when created with BlzCreateFrame.

Inside a fdf "//" starts a comment and text in the same line after "//" is ignored.

One can also create multiLine Comments as seen in _locales/<language>/ui/framedef/globalstrings.

 Code:

/* Opens the multi line Comment
*
*
*/ End the MultiLine Comment

A fdf starts with an includeFile list. (If a fdf does not need to include, then there is none.)

IncludeFile is used when a frame in this fdf wants to "INHERITS" a frame beeing defined in another fdf.

 Code:

// -- INCLUDE FILES ---------------------------------------------------------

IncludeFile "UI\FrameDef\UI\EscMenuTemplates.fdf",

The important things for us are the Frames. That is such a Frame inside "questdialog.fdf".

 Code:

Frame "FRAME" "QuestItemListItem" { //Main-Frame Head
 Height 0.012, //default Height
 Width 0.15, //default Width

 Frame "TEXT" "QuestItemListItemTitle" INHERITS "EscMenuButtonTextTemplate" { //Child-Frame Head
 UseActiveContext, //SetPoint in this Frame will ask for a Frame with the same CreateContext
 SetPoint LEFT, "QuestItemListItem", LEFT, 0.002, 0, //Place LEFT of "QuestItemListItemTitle" to LEFT of "QuestItemListItem" offset by 0.002*windowX Size
 SetPoint RIGHT, "QuestItemListItem", RIGHT, -0.002, 0,//Place RIGHT of "QuestItemListItemTitle" to RIGHT of "QuestItemListItem" offset of -0.002*windowXSize
 FrameFont "MasterFont", 0.010,"",
 FontColor 1.0 1.0 1.0 1.0, //RGBA (red green blue alpha) 0 to 1.0 each, default Color
 FontHighlightColor 1.0 1.0 1.0 1.0, //Used when clicked
 FontDisabledColor 0.4 0.5 0.6 0.7, //Used when Frame is SetEnable(fale)
 FontJustificationH JUSTIFYLEFT, //Text is placed at the left part of the frame
 FontJustificationOffset 0.01 0.001,
 Text "QuestItemListItemTitle", //Default Text displayed
 }
}

Alot of text so lets split it up into peaces explaining each one individual.

The main-Frame Head

 Code:

Frame "FRAME" "QuestItemListItem" {

Its type/behaviour is "FRAME" and this frame is named "QuestItemListItem". Accessing or creating it requiers the name.

Text in all lines between "{" "}" can be called Framebody, it belongs to this Frame. As seen above a Frame can contain other Frames this sub-Frames also have a body.

The Framebody of "QuestItemListItem" defines default size which created "QuestItemListItem" will use, if not said otherwise:

 Code:

 Height 0.012,
 Width 0.15,

Inside "QuestItemListItem" there is a child-Frame defined.

 Code:

Frame "TEXT" "QuestItemListItemTitle" INHERITS "EscMenuButtonTextTemplate" {

The child is named "QuestItemListItemTitle" and is of type "TEXT".

"QuestItemListItemTitle" does also takeover (INHERITS) values from another Frame named "EscMenuButtonTextTemplate". "EscMenuButtonTextTemplate" is not located in the same fdf. Hence it is defined in a fdf included with IncludeFile on top of the text written in "questdialog.fdf".

 Code:

Frame "TEXT" "EscMenuButtonTextTemplate" {
 DecorateFileNames, //Inside this Frame Filenames are lookups in a string table like GameInterface etc.
 FrameFont "EscMenuTextFont", 0.013, "",
 FontJustificationH JUSTIFYCENTER, //Place the Text in the center < > in regards left right
 FontJustificationV JUSTIFYMIDDLE, //place the Text in the middle in regards up down
 FontJustificationOffset 0.0 -0.002,
 FontFlags "FIXEDSIZE",
 FontColor 0.99 0.827 0.0705 1.0,
 FontHighlightColor 1.0 1.0 1.0 1.0,
 FontDisabledColor 0.5 0.5 0.5 1.0,
 FontShadowColor 0.0 0.0 0.0 0.9,
 FontShadowOffset 0.002 -0.002,
}

INHERITS can be followed by another word WITHCHILDREN.

To change the text of "QuestItemListItem" one would have to access "QuestItemListItemTitle" cause "QuestItemListItemTitle" manages the text displayed. While QuestItemListItem manages position and size of that frame.​

##
 SimpleFrame

There exist another Type of frames, they are called SimpleFrames all of them have SIMPLE in their Typename. This type of frames also can have special Frame-childrens beeing called Texture or String, both are accessable with BlzGetFrameByName. Many default Ingame-UI Frames are simpleFrames.

Frames are displayed over SimpleFrams.

This is the fdf of such a SimpleFrame its from the file "simpleinfopanel.fdf"

Each Texture is a frame showing an image, each String is a FrameShowingText.

 Code:

Frame "SIMPLEFRAME" "SimpleInfoPanelIconDamage" {
 UseActiveContext,
 SetAllPoints,
 DecorateFileNames,
 Height 0.03125,

 // --- icon -------------------------------------------------------------
 Texture "InfoPanelIconBackdrop" INHERITS "InfoPanelIconTemplate" {
 File "HeroStrengthIcon",
 }

 // --- icon # -----------------------------------------------------------
 String "InfoPanelIconLevel" INHERITS "SimpleInfoPanelAttributeTextTemplate" {
 SetPoint CENTER, "InfoPanelIconBackdrop", BOTTOMRIGHT, -0.007625, 0.006875,
 }

 // --- label ------------------------------------------------------------
 String "InfoPanelIconLabel" INHERITS "SimpleInfoPanelLabelTextTemplate" {
 SetPoint TOPLEFT, "InfoPanelIconBackdrop", TOPRIGHT, 0.0, -0.003,
 Text "COLON_DAMAGE",
 Font "InfoPanelTextFont", 0.007,
 }

 // --- value ------------------------------------------------------------
 String "InfoPanelIconValue" INHERITS "SimpleInfoPanelValueTextTemplate" {
 Width 0.055,
 SetPoint TOPLEFT, "InfoPanelIconLabel", BOTTOMLEFT, 0.002625, -0.003,
 Font "InfoPanelTextFont",0.0075,
 }
}

This "SimpleInfoPanelIconDamage" is the defintion of the Frame showing attackdamage, you see this frame in the game when selecting 1 unit only.

"InfoPanelIconBackdrop" manages the icon (ingame its the icon if the attackType).

"InfoPanelIconLevel" manges the upgrade Level inside the icon (when your unit benefits from techs) (In the image "0")

"InfoPanelIconLabel" tells you what "InfoPanelIconValue" has to be interpreted as. (In the image below "Schaden:")

"InfoPanelIconValue" is the amount of attack shown. (In the image below "12 - 13")

##
 Fdf Syntax

There exists something like 105 fdf-actions each of this actions is followed by an group of arguments which are either seperated by whitespace or by comma + optional whitespace. Between the first argument and the Action is whitespace. The used rule of seperation depends on the Action. Some FDF-Actions have multiple Versions with different amount of arguments.

An example for each:

FontColor 0.99 0.827 0.0705 1.0,

FrameFont "EscMenuTextFont", 0.010, "",​
Actions not opening a Block { } end with a comma. Only the Actions: Frame, String, Texture, Layer and StringList open Blocks. All of them except for Layer can exist outside of Blocks. IncludeFile and StringList are Actions only written outside of Blocks. Other fdf-Actions have to be in a Block. Actions in a Block belong to the Action that started the Block most times defining a Frame.

I personaly call Frames defined outside of Blocks MainFrame, when I talk about them. Only such MainFrames can be Created & Inherited. Frames can have any amount of Child-Frames which also can have Child-Frames, but each Frame has only one Parent.

Fdf Frames support a behaviour called INHERITS in which a Frame copys the Fdf Actions from another Frame. There also exists an advanced version INHERITS WITHCHILDREN, this also copies the Child-Frames and uses their fdf-Actions. In fdf the Frame one wants to inherit from has to be from the same fdf or from an included one.

For the Fdf-Action Frame: The names of MainFrames in one fdf have to be unique. If another MainFrame with the same name in the current fdf is encountered the remaining File-content is skiped. ChildFrames can have the same name as their brothers, their parent or be empty "".

Each Frame uses one of the Warcraft 3 FrameTypes, with the FrameType the behaviour and the set of possible fdf-Actions for that Frame is choosen. This FrameTypes can be placed into 2 groups SimpleFrames (all of them start with SIMPLE) and the others which I just call Frames (a bit confusing, cause all are Frames). One should do this seperation cause they have different features, techniques, rules and it is difficult to fit them together (logical not visual).

In fdf paths are written only with one "\".

The upto date version of Warcraft 3 tells you about syntax errors of custom fdf. When they are loaded over a toc file in your map. To inspect such a error log run your map with the fdf and toc then close the game and check Users\<user>\Documents\Warcraft III\Logs\War3Log.txt.

Such an Fdf-error line in that Log-file could look like that:

 Code:

11/8 15:44:54.356 Error (war3mapImported\Test.fdf:4): Expected ",", but found "Height"

Sadly it seems to only tell you about the first encountered error and won't tell you about any error if a toc does not have that needed empty ending line.

##
 FDF-Keywords

A List of Key words used in fdf. This are not FrameTypes, This are the actions inside Frame Bodies.

This List only list the names of the actions, for real use look at the list below it.

 Code:

AlphaMode
Anchor
BackdropBackground
BackdropBackgroundInsets
BackdropBackgroundSize
BackdropBlendAll
BackdropBottomFile
BackdropCornerFile
BackdropCornerFlags
BackdropCornerSize
BackdropEdgeFile
BackdropHalfSides
BackdropLeftFile
BackdropMirrored
BackdropRightFile
BackdropTileBackground
BackdropTopFile
BackgroundArt
BarTexture
ButtonPushedTextOffset
ButtonText
ChatDisplayBorderSize
ChatDisplayEditBox
ChatDisplayLineHeight
ChatDisplayScrollBar
CheckBoxCheckHighlight
CheckBoxDisabledCheckHighlight
CheckedTexture
ControlBackdrop
ControlDisabledBackdrop
ControlDisabledPushedBackdrop
ControlFocusHighlight
ControlMouseOverHighlight
ControlPushedBackdrop
ControlShortcutKey
ControlStyle
DecorateFileNames
DialogBackdrop
DisabledText
DisabledTexture
EditBorderSize
EditCursorColor
EditTextFrame
File
Font
FontColor
FontDisabledColor
FontFlags
FontHighlightColor
FontJustificationH
FontJustificationOffset
FontJustificationV
FontShadowColor
FontShadowOffset
Frame
FrameFont
Height
HighlightAlphaFile
HighlightAlphaMode
HighlightColor
HighlightText
HighlightType
IncludeFile
Layer
LayerStyle
ListBoxBorder
ListBoxScrollBar
MenuBorder
MenuItem
MenuItemHeight
MenuTextHighlightColor
NormalText
NormalTexture
PopupArrowFrame
PopupButtonInset
PopupMenuFrame
PopupTitleFrame
PushedTexture
ScrollBarDecButtonFrame
ScrollBarIncButtonFrame
SetAllPoints
SetPoint
SliderInitialValue
SliderLayoutHorizontal
SliderLayoutVertical
SliderMaxValue
SliderMinValue
SliderStepSize
SliderThumbButtonFrame
String
StringList
TabFocusDefault
TabFocusNext
TabFocusPush
TexCoord
Text
TextAreaInset
TextAreaLineGap
TextAreaLineHeight
TextAreaMaxLines
TextAreaScrollBar
TextLength
Texture
UseActiveContext
UseHighlight
Width

variations of Keywords found in blizzard's fdf:

 Code:

AlphaMode "ADD",
AlphaMode "ALPHAKEY",

Anchor BOTTOMLEFT,-0.256,0,
Anchor BOTTOMRIGHT,0,0,
Anchor TOPLEFT, 0.0, -0.003,
Anchor TOPRIGHT, -0.0914, -0.003125,

BackdropBackground "EscMenuBackground",
BackdropBackground "EscMenuBlankBackground",
BackdropBackground "EscMenuEditBoxBackground",
BackdropBackground "MultiboardBackground",
BackdropBackground "MultiboardMinimizeButtonDisabled",
BackdropBackground "EscMenuBackground",
BackdropBackground "EscMenuButtonBackground",
BackdropBackground "EscMenuButtonDisabledPushedBackground",
BackdropBackground "EscMenuCheckBoxPushedBackground",
BackdropBackground "HeroAgilityIcon",
BackdropBackground "HeroIntelligenceIcon",
BackdropBackground "QuestDialogCompletedBackground",
BackdropBackground "UI\Widgets\BattleNet\bnet-inputbox-back.blp",
BackdropBackground "UI\Widgets\BattleNet\bnet-mainmenu-clans-disabled.blp",

BackdropBackgroundInsets 0.0 0.0 0.01 0.0,
BackdropBackgroundInsets 0.0 0.01 0.0 0.0,
BackdropBackgroundInsets 0.0025 0.0025 0.0025 0.0025,
BackdropBackgroundInsets 0.003 0.003 0.003 0.003,

BackdropBackgroundSize 0.128,

BackdropBlendAll,

BackdropBottomFile "UI\Widgets\HeavyBorderBottom.blp",
BackdropBottomFile "UI\Widgets\LightBorderBottom.blp",
BackdropCornerFile "UI\Widgets\ButtonCorners.blp",
BackdropCornerFile "UI\Widgets\LightBorderCorners.blp",

BackdropCornerFlags "BL|BR|B",
BackdropCornerFlags "UL|UR|T",

BackdropCornerSize 0.0125,
BackdropCornerSize 0.006,
BackdropCornerSize 0.008,
BackdropCornerSize 0.0125,
BackdropCornerSize 0.0155,

BackdropEdgeFile "EscMenuBorder",
BackdropEdgeFile "EscMenuEditBoxBorder",
BackdropEdgeFile "EscMenuButtonBorder",
BackdropEdgeFile "CinematicBorder",
BackdropEdgeFile "EscMenuButtonBorder",
BackdropEdgeFile "EscMenuButtonDisabledPushedBorder",
BackdropEdgeFile "EscMenuButtonPushedBorder",
BackdropEdgeFile "UI\Glues\ScoreScreen\scorescreen-buttonbackground.blp",
BackdropEdgeFile "UI\Widgets\BattleNet\bnet-dialoguebox-border.blp",
BackdropEdgeFile "UI\widgets\BattleNet\bnet-tooltip-border.blp",
BackdropEdgeFile "UI\Widgets\Glues\GlueScreen-Button1-BackdropBorder-Disabled.blp",
BackdropEdgeFile "UI\Widgets\Glues\GlueScreen-Button1-BackdropBorder-DisabledDown.blp",
BackdropEdgeFile "UI\Widgets\Glues\GlueScreen-Button1-BorderedBackdropBorder-Disabled.blp",
BackdropEdgeFile "UI\Widgets\Glues\GlueScreen-Button2-BackdropBorder-Disabled.blp",

BackdropHalfSides,

BackdropLeftFile "UI\Widgets\ButtonLeft.blp",

BackdropMirrored,

BackdropRightFile "UI\Widgets\HeavyBorderRight.blp",

BackdropTileBackground,

BackdropTopFile "UI\Widgets\ButtonTop.blp",

BackgroundArt "UI\Glues\BattleNet\BattlenetLoginGlue\BattlenetLoginGlue.mdl",
BackgroundArt "UI\Glues\BattleNet\BattleNetTeamLevelBar\BattleNetTeamLevelBar.mdl",

BarTexture "SimpleBuildTimeIndicator",

ButtonPushedTextOffset -0.0015f -0.0015f,
ButtonPushedTextOffset 0.001 -0.001,

ButtonText "AddFriendButtonText",
ButtonText "AdvancedOptionsButtonText",

ChatDisplayBorderSize 0.01,
ChatDisplayEditBox "BattleNetChatDisplayEditBoxTemplate",
ChatDisplayLineHeight 0.01,
ChatDisplayScrollBar "BattleNetChatDisplayScrollBarTemplate",

CheckBoxCheckHighlight "BattleNetRadioButtonHighlightTemplate",
CheckBoxCheckHighlight "EscMenuRadioButtonHighlightTemplate",

CheckBoxDisabledCheckHighlight "BattleNetDisabledRadioButtonHighlightTemplate",
CheckBoxDisabledCheckHighlight "EscMenuDisabledCheckHighlightTemplate",

CheckedTexture "ReplayCheckBoxCheck",

ControlBackdrop "ActionMenuBackdrop",
ControlBackdrop "AdvancedPopupMenuBackdrop",

ControlDisabledBackdrop "BattleNetCheckBoxDisabledBackdrop",
ControlDisabledBackdrop "BattleNetPopupMenuDisabledBackdropTemplate",

ControlFocusHighlight "CampaignCameraButtonFocusHighlightTemplate",
ControlFocusHighlight "IconicButtonFocusHighlightTemplate",

ControlMouseOverHighlight "BorderedButtonMouseOverHighlightTemplate",
ControlMouseOverHighlight "ButtonMouseOverHighlightTemplate",

ControlPushedBackdrop "BattleNetRadioButtonPushedBackdrop",
ControlPushedBackdrop "BorderedButtonPushedBackdropTemplate",

ControlShortcutKey "BNET_LADDER_SHORTCUT",
ControlShortcutKey "BNET_PASSWORD_RECOVERY_SHORTCUT",

ControlStyle "AUTOTRACK|HIGHLIGHTONFOCUS|HIGHLIGHTONMOUSEOVER",
ControlStyle "AUTOTRACK|HIGHLIGHTONMOUSEOVER",
ControlStyle "AUTOTRACK",

DecorateFileNames,

DialogBackdrop "BattleNetDialogBackdropTemplate",
DialogBackdrop "CustomFilterDialogBackdrop",

DisabledText "UpperButtonBarButtonDisabledTextTemplate" "ALLIES",
DisabledText "UpperButtonBarButtonDisabledTextTemplate" "CHAT",

DisabledTexture "UpperMenuButtonDisabledBackground",

EditBorderSize 0.009,
EditCursorColor 1.0 1.0 1.0,
EditTextFrame "AccountNameEditBoxText",

File "ConsoleTexture01",
File "ConsoleTexture03",
File "ConsoleTexture04",
File "ConsoleTexture05",
File "ConsoleTexture06",
File "GoldIcon",
File "HeroStrengthIcon",
File "LumberIcon",
File "SimpleBuildTimeIndicatorBorder",

Font "InfoPanelTextFont", 0.0085,
Font "InfoPanelTextFont",0.0085,
Font "InfoPanelTextFont",0.01,
Font "MasterFont",0.008,
Font "MasterFont",0.01,

FontColor 0.99 0.827 0.0705 1.0,
FontDisabledColor 0.5 0.5 0.5 1.0,

FontFlags "FIXEDSIZE",
FontFlags "PASSWORDFIELD",

FontHighlightColor 1.0 1.0 1.0 1.0,

FontJustificationH JUSTIFYCENTER,
FontJustificationH JUSTIFYLEFT,
FontJustificationH JUSTIFYRIGHT,

FontJustificationOffset 0.0 -0.001,
FontJustificationOffset 0.0 -0.002,
FontJustificationOffset 0.0 0.0,
FontJustificationOffset 0.01 0.0,
FontJustificationOffset 0.01 0.001,

FontJustificationV JUSTIFYTOP,
FontJustificationV JUSTIFYMIDDLE,
FontJustificationV JUSTIFYBOTTOM,

FontShadowColor 0.0 0.0 0.0 0.9,
FontShadowOffset 0.001 -0.001,

FrameFont "EscMenuTextFont", 0.011, "",
FrameFont "EscMenuTextFont",0.011,"",
FrameFont "InfoPanelTextFont", 0.011, "",
FrameFont "InfoPanelTextFont", 0.013, "",
FrameFont "MasterFont", 0.007, "",
FrameFont "MasterFont", 0.01, "",
FrameFont "MasterFont", 0.011, "",
FrameFont "MasterFont", 0.01171, "",

Frame "GLUEBUTTON" "HeroSelectorButton" {

Height 0.024,
Height 0.48f,

HighlightAlphaFile "EscMenuButtonMouseOverHighlight",
HighlightAlphaFile "EscMenuCheckBoxCheckHighlight",
HighlightAlphaFile "EscMenuDisabledRadioButtonSelectedHighlight",
HighlightAlphaFile "UI\Glues\ScoreScreen\scorescreen-tab-hilight.blp",
HighlightAlphaFile "UI\Widgets\Glues\GlueScreen-Button-KeyboardHighlight.blp",
HighlightAlphaFile "UI\Widgets\Glues\GlueScreen-Checkbox-Check.blp",
HighlightAlphaFile "UI\Widgets\Glues\GlueScreen-Checkbox-CheckDisabled.blp",
HighlightAlphaFile "UI\Widgets\Glues\GlueScreen-RadioButton-ButtonDisabled.blp",

HighlightAlphaMode "ADD",
HighlightAlphaMode "BLEND",

HighlightColor 0.0 0.0 1.0 0.1,
HighlightColor 1.0 0.0 0.0 0.2,

HighlightText "UpperButtonBarButtonHighlightTextTemplate" "KEY_ALLIES",
HighlightText "UpperButtonBarButtonHighlightTextTemplate" "KEY_CHAT",

HighlightType "FILETEXTURE",
HighlightType "SHADE",

IncludeFile "UI\FrameDef\UI\InfoPanelTemplates.fdf",

LayerStyle "NOSHADING",
LayerStyle "IGNORETRACKEVENTS",
LayerStyle "NOSHADING|IGNORETRACKEVENTS",

ListBoxBorder 0.01,
ListBoxScrollBar "StandardListBoxScrollBarTemplate",

MenuBorder 0.009,

MenuItem "NORMAL", -2,
MenuItem "WINDOW_MODE_WINDOWED", -2,

MenuItemHeight 0.0082,
MenuItemHeight 0.011,

MenuTextHighlightColor 0.99 0.827 0.0705 1.0,

NormalText "UpperButtonBarButtonTextTemplate" "KEY_ALLIES",
NormalText "UpperButtonBarButtonTextTemplate" "KEY_MENU",

NormalTexture "ReplayCheckBoxNormal",

PopupArrowFrame "CampaignPopupMenuArrow",
PopupArrowFrame "CustomPopupMenuArrow",

PopupButtonInset 0.01,
PopupButtonInset 0.015,

PopupMenuFrame "TeamMemberRaceMenuMenu3",
PopupMenuFrame "TextureQualityPopupMenuMenu",

PopupTitleFrame "EscOptionsLightsPopupMenuTitle",
PopupTitleFrame "PopupMenuTitle",
PopupTitleFrame "ReplayVisionMenuTitle",
PopupTitleFrame "TeamMemberPopupMenuTitle",

PushedTexture "UpperMenuButtonPushedBackground",

ScrollBarDecButtonFrame "EscMenuScrollBarDecButton",
ScrollBarIncButtonFrame "EscMenuScrollBarIncButton",

SetAllPoints,

SetPoint BOTTOM, "BuildTimeIndicator", TOP, 0.0, 0.00325,
SetPoint BOTTOM, "ChatDialog", BOTTOM, 0.0, 0.03,
SetPoint BOTTOMLEFT, "AllyHeader", BOTTOMRIGHT, 0.004, 0.0,
SetPoint BOTTOMLEFT, "BattleNetChatPanel", BOTTOMLEFT, 0.01125, 0.02125,
SetPoint BOTTOMRIGHT, "QuestListItem", BOTTOMRIGHT, -0.003, 0,
SetPoint BOTTOMRIGHT, "AllianceDialog", BOTTOM, -0.003, 0.03,
SetPoint CENTER, "GameSaveSplashDialog", CENTER, 0.0, 0.0,
SetPoint CENTER, "IconBackdrop4", BOTTOMRIGHT, -0.007625, 0.006875,
SetPoint LEFT, "AllianceAcceptButton", RIGHT, 0.005, 0.0,
SetPoint LEFT, "AlliedVictoryCheckBox", RIGHT, 0.01, 0.0,
SetPoint RIGHT, "ChannelEnterButton", LEFT, -0.02, 0.0,
SetPoint RIGHT, "ChatDialog", TOPRIGHT, -0.031, -0.0765,
SetPoint TOP, "AnonSearchTitle", BOTTOM, 0.0, -0.005,
SetPoint TOP, "AuthorLabel", BOTTOM, 0.0, -0.004,
SetPoint TOPLEFT, "LeaderboardTitle", BOTTOMLEFT, -0.02f, -0.002,
SetPoint TOPLEFT,"WindowModeBackdrop",BOTTOMLEFT, 0.0, 0.01375,
SetPoint TOPRIGHT, "AdvancedOptionsPane", TOPRIGHT, -0.004, -0.03,

SliderInitialValue 0,
SliderInitialValue 1,

SliderLayoutHorizontal,
SliderLayoutVertical,

SliderMaxValue 2,
SliderMaxValue 4,

SliderMinValue 0,

SliderStepSize 1,

SliderThumbButtonFrame "BattleNetThumbButton",
SliderThumbButtonFrame "EscMenuScrollThumbButton",
SliderThumbButtonFrame "StandardThumbButton",

String "UpperButtonBarButtonTextTemplate" {

StringList {

TabFocusDefault,
TabFocusNext "AddProfileButton",
TabFocusNext "BackButton",

TabFocusPush,

TexCoord 0, 0.33984375, 0, 0.125,
TexCoord 0, 1, 0.4140625, 1,
TexCoord 0.0, 0.6640625, 0.25, 0.421875,
TexCoord 0.0, 0.6640625, 0.75, 0.921875,

Text "30",

TextAreaInset 0.005,
TextAreaInset 0.0,

TextAreaLineGap 0.001,
TextAreaLineGap 0.0015,

TextAreaLineHeight 0.011,
TextAreaLineHeight 0.015,

TextAreaMaxLines 128,
TextAreaMaxLines 32,

TextAreaScrollBar "ChatScrollBar",

TextLength 8,

Texture "InfoPanelIconAllyFoodIcon" INHERITS "ResourceIconTemplate" {

UseActiveContext,

UseHighlight "UpperMenuButtonHighlight",

Width 0.24,
Width 0.417f,

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


 Last edited: Nov 8, 2020

#### WaterKnight

#### WaterKnight

 Level 26

JoinedAug 18, 2009Messages4,100

Resources: 7

#### Resources
Maps 1Tutorials 6

There are also multiline comments:

 JASS:

/*
* I
* am
* a
* multline
* comment
*/

as found in the GlobalStrings.fdf. You can also have StringLists:

 JASS:

StringList {
 ATTR1 "VALUE1"
 ATTR2 "VALUE2"
 ...
}

Do the FrameTypes come with individual logic and therefore attributes that make sense?

Is the comma after IncludeFile after each such line or only as a separator?

Does an inner frame inherit all the attributes of the enclosing frame? If so, what inheritance has precedence?

What does the WITHCHILDREN do? Would that be like the heir gets all the inner frames of the parent frame?

What are the units measured in? Is there a complete reference?


 Last edited: Jun 8, 2019

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

WaterKnight said:

 Do the FrameTypes come with individual logic and therefore attributes that make sense?


Click to expand...

Each FrameType has an inner logic and can have special attributes.

For example SIMPLESTATUSBAR has a "BarTexture" while a simplecheckbox has

NormalTexture

PushedTexture

CheckedTexture​

WaterKnight said:

 Is the comma after IncludeFile after each such line or only as a separator?


Click to expand...

Each expression in the fdf ends with a "," this is requiered, except for the framehead which opens a body with "{"

Each file you want to include has to start with IncludeFile followed by its path.

This is valid

IncludeFile "UI\FrameDef\UI\EscMenuTemplates.fdf", IncludeFile "UI\FrameDef\GLUE\standardtemplates.fdf",

whitespace could be skiped​

also this is valid

IncludeFile "UI\FrameDef\UI\EscMenuTemplates.fdf",

IncludeFile "UI\FrameDef\GLUE\standardtemplates.fdf",​

this is not valid

IncludeFile "UI\FrameDef\UI\EscMenuTemplates.fdf","UI\FrameDef\GLUE\standardtemplates.fdf",​

WaterKnight said:

 What does the WITHCHILDREN do? Would that be like the heir gets all the inner frames of the parent frame?


Click to expand...

When using WITHCHILDREN also the childrens code will run and are copied in behaviour although I failed to access the inherited children with BlzGetFrameByName.

WaterKnight said:

 What are the units measured in? Is there a complete reference?


Click to expand...

The units are from the 4:3 Screen the bottom left is 0.0/0.0 (minimap) and the top right of the 4:3 Screen is 0.8/0.6 (text box next to food)

#### Kazeon

#### Kazeon

 Level 34

JoinedOct 12, 2011Messages3,450

Resources: 54

#### Resources
Icons 3Skins 1Tools 1Maps 7Spells and Systems 31JASS 4Tutorials 7

I must commend you for all these UI tutorials. But I have several questions:

Say I defined a frame with child frame like this:

 Code:

Frame "SIMPLESTATUSBAR" "parent"
{
 Width 0.07,
 Height 0.012,
 BarTexture "Textures\white.blp",

 Layer "BACKGROUND"
 {
 Texture "bg"
 {
 File "Textures\black32.blp"
 }
 }

 Layer "FOREGROUND"
 {
 Texture "bg"
 {
 File "Textures\white.blp"
 }
 }

 Frame "SIMPLEFRAME" "child"

 {
 Layer "BACKGROUND"
 {
 Texture "bg"
 {
 File "Textures\white.blp"
 }
 }
 }
}

1. Is it even valid? Can I name the "layer" to whatever I like or is there a convention for it?

2. How to change the parent's "background" and "foreground" texture in-game? What code to use?

3. If I instantiate the parent frame twice, would the second parent's child frame have the same "createContext" parameter as the parent?

4. I noticed you use some names for Files, not a file path. Is there a list of what name we can use, like this "SimpleXpBarBorder"?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Quilnez said:

 Is it even valid? Can I name the "layer" to whatever I like or is there a convention for it?


Click to expand...

For Layers I copied default fdfs which uses "ARTWORK" and "BACKGROUND", I don't know their real purpose or if they need this names. MyBar Example worked better with the border and Text overlay beeing in Layer "ARTWORK" in a child.

More important are Frame, String and Texture names.

Quilnez said:

 How to change the parent's "background" and "foreground" texture in-game? What code to use?


Click to expand...

You have to give the Textures in the frame unique names. You called both Textures "bg" which disallows accessing them afterwards using BlzGetFrameByName("bg", createcontext of parent). You should maybe name them "bgParent", "bgChild" or something you think is suited. Then you can access them with BlzGetFrameByName("bgParent", createcontext of parent) / BlzGetFrameByName("bgChild", createcontext of parent).

Quilnez said:

 If I instantiate the parent frame twice, would the second parent's child frame have the same "createContext" parameter as the parent?


Click to expand...

Childs use the same createContext as their parent. Although I don't kinow how to access a inherited WITHCHILDREN child frame.

If you define positions in frames you also should write <UseActiveContext,>, without that non strong binded childs will stick to the first created one.

Quilnez said:

 4. I noticed you use some names for Files, not a file path. Is there a list of what name we can use, like this "SimpleXpBarBorder"?


Click to expand...

When one writes <DecorateFileNames,> into a frame the filepaths are looked up in a stringList. Gameinterface is such a list when checking the checkbox "show rawNames" one can see the names they are the ones shown at the left.

There are also 4 default fdf defining StringLists in:

war3.w3mod:_locales\<yourlocal>:ui\framedef\datestrings.fdf

war3.w3mod:_locales\<yourlocal>:ui\framedef\globalstrings.fdf

war3.w3mod:_locales\<yourlocal>:ui\framedef\infopanelstrings.fdf

war3.w3mod:_locales\<yourlocal>:ui\framedef\networkstrings.fdf

war3.w3mod:ui\war3skins.txt


 Last edited: Feb 3, 2021

#### Wareditor

#### Wareditor

 Level 15

JoinedJan 16, 2009Messages764

Resources: 4

#### Resources
Maps 4

@Quilnez

For the names you are looking for UI\war3skins.txt

Also you can define new ones or overwrite them in war3mapSkin.txt

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Frame Definition Files (fdf) can be placed into localized path that way one can support multlanguages or add new GetLocalizedString values. That is done by adding a prefix to the imported path. (but not inside the toc).

if one has "mystrings.fdf" which is loaded from "war3mapimported\mystrings.fdf" then one could add a german version by importing another fdf into the map to "_Locales\deDE.w3mod\war3mapImported\mystrings.fdf". Inside the toc one still reads "war3mapimported\mystrings.fdf", but for german users the game will now load "_Locales\deDE.w3mod\war3mapImported\mystrings.fdf" instead.

In the map uploaded to this post I added 3 Localized Strings "MYTEST", "MYLABEL" and "MYVALUE" in german english and France.

Which are displayed at 0.0s

#### Attachments

Localized Fdf StringList.w3x

 17 KB

 · Views: 436



#### yxq1122

#### yxq1122

 Level 8

JoinedJun 5, 2018Messages188

Resources: 3

#### Resources
Tools 3

It would be great if you list and explain the properties such as FrameFont.

How can we get the full list of these properties?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

When I wrote that I though it would be a good idea to have this properties inside the tutorials of the frameType using it. This tutorial was meant to be more unspecific.

I am not so sure about this Text-Fonts myself, Niklas (in Hive Discord) said something recently inside Discord.

 it works a bit different iirc

you dont say the .ttf or .otf filename

but the constant you stored them on in war3mapSkin.txt

[CustomSkin]

ChatFont=Font.otf

EscMenuTextFont=Font.otf

TextTagFont=Font.otf

InfoPanelTextFont=Font.otf

MessageFont=Font.otf

MasterFont=Font.otf


Click to expand...

The first string is one of this variables. the second is the size in the Warcraft 3 screen Units (0.0 to 1.0) the 3. no idea.

#### GetLocalPlayer

#### GetLocalPlayer

 Level 4

JoinedApr 11, 2018Messages24

It seems you can't create frames from a FDF if that FDF has been included in another FDF.

An example of a FDF named "Test.fdf":

 Code:

IncludeFile "ui\framedef\ui\escmenutemplates.fdf",

// Some frames, e.t.c

If my TOC file is

 Code:

test.fdf
ui\framedef\ui\escmenutemplates.fdf

I can't create anything from "ui\framedef\ui\escmenutemplates.fdf". But it works if I place the included FDF before the including one.

 Code:

ui\framedef\ui\escmenutemplates.fdf
test.fdf

Now I can create frames from "ui\framedef\ui\escmenutemplates.fdf" as well as from "Test.fdf" that includes "ui\framedef\ui\escmenutemplates.fdf".

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Hello Tasyen,

Thanks for this great tutorial.

I was wondering if it could be possible to make changes to existing blizzard fdf files, instead of making entirely new ones.

For instance, I want to use Resource Bar, but strip the element 'ResourceBarUpkeepText'

I have attempted to extract the Blizzard resource bar file, delete the part about 'ResourceBarUpkeepText' and then import the file into the map in the ui\framedef\ui\resourcebar.fdf location. Unfortunately this does not work at all, and I'm now at a loss.

Do you have any idea if this would be possible.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

One can not overwrite fdf.

But In 1.32 one can load fdf by a toc generating frames with the same names. The loading has to happen in function config or in the Lua root, then yours are generated instead of the blizzard's one. Although removing a frame that the game expects might result into problems.

You might be better off with moving that String Frame out of the Screen at 0s.

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Thanks, great to know!

For me the working solution now is to change ResourceBarUpkeepText to "".

#### Planetary

#### Planetary

 Level 15

JoinedFeb 7, 2020Messages399

Resources: 3

#### Resources
Maps 2Spells and Systems 1

I discovered a headache-inducing quirk with the FrameFont property that people might want to know about.

For some reason, for specific .ttf fonts, putting the font face declaration anywhere but at the top causes it to not work.

This works:

 Lua:

Frame "TEXT" "FrameHeaderText" {
 FrameFont "war3mapImported\fonts-tarzan.ttf", 0.01, "",
 FontJustificationH JUSTIFYCENTER,
 FontJustificationV JUSTIFYCENTER,
 FontColor 1.0 1.0 1.0 1.0,
}

This does not work, and defaults to the normal font face:

 Lua:

Frame "TEXT" "FrameHeaderText" {
 FontJustificationH JUSTIFYCENTER,
 FontJustificationV JUSTIFYCENTER,
 FontColor 1.0 1.0 1.0 1.0,
 FrameFont "war3mapImported\fonts-tarzan.ttf", 0.01, "",
}

However, this Consolas version does work with the font face not placed at the top:

 Lua:

Frame "TEXT" "FrameBaseText" {
 FontJustificationH JUSTIFYLEFT,
 FontJustificationV JUSTIFYTOP,
 FrameFont "war3mapImported\fonts-consolas.ttf", 0.009, "",
 FontColor 1.0 1.0 1.0 1.0,
}

#### Attachments

upload_2020-8-15_12-33-2.png

 81.7 KB

 · Views: 202




 Last edited: Aug 15, 2020

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

A bit of a brick wall in terms of text, but approved regardless.

I consider this a misc or general mapping tutorial, but since it is kinda apart of a series of jass tutorials I am putting it in the same section.

Can probably be argued otherwise but it makes sense to me.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

It seems to be like the files mentioned in the IncludeFile list should be sorted by Alphabet (abc). It is no problem, if they are not sorted and do not Include the same Files.. I did some test and now think that this is the case.

My fdf used was the one below. By reordering the IncludeFiles it sometimes fails to create the frame.

My guess for the failing is OPTIONSMENU & PLAYERSLOT both Include

"UI\FrameDef\Glue\StandardTemplates.fdf"

 Code:

IncludeFile "UI\FRAMEDEF\GLUE\BATTLENETTEMPLATES.FDF",
IncludeFile "UI\FRAMEDEF\GLUE\OPTIONSMENU.FDF",
IncludeFile "UI\FRAMEDEF\GLUE\PLAYERSLOT.FDF",
IncludeFile "UI\FRAMEDEF\UI\ESCMENUTEMPLATES.FDF",

Frame "FRAME" "SuperParent" {
 Width 0.2,
 Height 0.2,
 Frame "POPUPMENU" "PopUp1" INHERITS WITHCHILDREN "OptionsPopupMenuTemplate" {
 Width 0.1775,
 Height 0.025,
 UseActiveContext,
 SetPoint BOTTOMLEFT, "SuperParent", BOTTOMLEFT, 0, 0,
 PopupMenuFrame "PopUp1PopUpMenu",
 Frame "MENU" "PopUp1PopUpMenu" INHERITS WITHCHILDREN "StandardPopupMenuMenuTemplate" {
 MenuItem "A", -2,
 MenuItem "B", -2,
 MenuItem "C", -2,
 }
 }
 Frame "GLUECHECKBOX" "CheckBox1" INHERITS WITHCHILDREN "EscMenuCheckBoxTemplate" {
 Width 0.024,
 Height 0.024,
 UseActiveContext,
 SetPoint BOTTOMLEFT, "SuperParent", BOTTOMLEFT, 0, 0,
 }
 Frame "GLUECHECKBOX" "CheckBox2" INHERITS WITHCHILDREN "BattleNetRadioButtonTemplate" {
 Width 0.024,
 Height 0.024,
 UseActiveContext,
 SetPoint BOTTOMLEFT, "SuperParent", BOTTOMRIGHT, 0, 0,
 }
 Frame "TEXTAREA" "TextArea1" INHERITS WITHCHILDREN "BattleNetTextAreaTemplate" {
 Width 0.1,
 Height 0.1,
 UseActiveContext,
 SetPoint TOP, "SuperParent", TOP, 0, 0,
 }
 Frame "POPUPMENU" "PopUp2" {
 Width 0.08,
 Height 0.03,
 UseActiveContext,
 SetPoint BOTTOMLEFT, "SuperParent", BOTTOMLEFT, 0, 0.03,
 PopupButtonInset 0.01,
 ControlBackdrop "PopUpBackdrop",
 Frame "BACKDROP" "PopUpBackdrop" INHERITS "EscMenuButtonBackdropTemplate" {
 }
 ControlDisabledBackdrop "PopUpBackdropDisabled",
 Frame "BACKDROP" "PopUpBackdropDisabled" INHERITS "EscMenuButtonDisabledBackdropTemplate" {
 }
 PopupTitleFrame "PopUpTitle",
 Frame "GLUETEXTBUTTON" "PopUpTitle" INHERITS WITHCHILDREN "EscMenuPopupMenuTitleTemplate" {
 }
 PopupArrowFrame "PopupArrow",
 Frame "BUTTON" "PopupArrow" INHERITS WITHCHILDREN "EscMenuPopupMenuArrowTemplate" {
 }
 PopupMenuFrame "PopUp2PopUpMenu",
 Frame "MENU" "PopUp2PopUpMenu" INHERITS WITHCHILDREN "EscMenuPopupMenuMenuTemplate" {
 MenuItem "A", -2,
 MenuItem "B", -2,
 MenuItem "C", -2,
 MenuItem "D", -2,
 }
 }
 Frame "POPUPMENU" "PopUp3" INHERITS WITHCHILDREN "PlayerSlotPopupMenu" {
 Width 0.109,
 Height 0.025,
 UseActiveContext,
 SetPoint BOTTOMLEFT, "SuperParent", BOTTOMLEFT, 0.1, 0.03,
 PopupMenuFrame "PopUp3PopUpMenu",
 Frame "MENU" "PopUp3PopUpMenu" INHERITS WITHCHILDREN "StandardPopupMenuMenuTemplate" {
 MenuItem "Sa", -2,
 MenuItem "Fa", -2,
 MenuItem "Da", -2,
 }
 }
}

Edit: Loading gigantic fdf with like 5000000 line can eat up alot of Ram and take some serious amount of seconds.


 Last edited: Dec 2, 2020

 You must log in or register to reply here.


### Similar threads

 UI: Frames and Tooltips


Tasyen

Jun 27, 2019

Scripting Tutorials

Replies45Views21K

Oct 27, 2024
RPG_F

 UI: GLUEBUTTON


Tasyen

Sep 9, 2019

Scripting Tutorials

Replies32Views15K

Nov 16, 2023
Tasyen

 UI: Creating a Bar


Tasyen

Jun 12, 2019

Scripting Tutorials

Replies38Views18K

Oct 2, 2025
KenArok

 UI - Simpleframes


Tasyen

Nov 25, 2019

Scripting Tutorials

Replies30Views16K

Sep 27, 2025
Tasyen

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