---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-creating-a-bar.316254/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Creating a Bar
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-creating-a-bar.316254/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

##
 What is a SIMPLESTATUSBAR

A SIMPLESTATUSBAR is a simple frame that shows a blp texture partly (from left to right) based on the SIMPLESTATUSBAR current value vs its max value. The blp Texture is stretched to the total frames size. On default the value can be from 0 to 100 (0 beeing empty, 100 full). 40 would show 40% of the blp.

Its also possible to have a border an overlay text and adding a background blp texture. The barTexture is above the background and will hide more of it as nearer the value is to the max value.

##
 Functions

This 3 functions are the ones you should use when alterting the bars Value.

 JASS:

native BlzFrameSetValue takes framehandle frame, real value returns nothing
native BlzFrameGetValue takes framehandle frame returns real
native BlzFrameSetMinMaxValue takes framehandle frame, real minValue, real maxValue returns nothing

BlzFrameSetValue sets the current value.

BlzFrameGetValue gets the local current value

BlzFrameSetMinMaxValue sets the min max values of the bar.

You also might use BlzFrameSetTexture takes framehandle frame, string texFile, integer flag, boolean blend to change the used textures during the game.​

##
 The Frame Definition File

There is no mainframe SIMPLESTATUSBAR in the default fdf, aka not createable without unwanted other frames. Therefore one has to write a custom one and load that custom one over a custom written toc.

Thats the fdf text for 3 SIMPLESTATUSBAR all beeing mainFrames. MyBarEx has a border while MyBar does not. The last has no additional features (background, border, text).

Without the border it is more suited for Faces. One could have the idea to hide or destroy the child but that is in that case no good idea, cause it would crash the game (its a String, Texture thing when one deals with simple frames).

MyBar.TOC

 Code:

war3mapImported\myBar.fdf

MyBar.fdf

 Code:

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
 BarTexture "Replaceabletextures\Teamcolor\Teamcolor00.blp", //Default BarTexture
 Layer "BACKGROUND" { //A simpleFrames thing, where this is placed layer wise
 Texture "MyBarExBackground"{ //the BACKGROUND Texture named "MyBarExBackground" its also a frame and a child
 File "Replaceabletextures\Teamcolor\Teamcolor27.blp", //Default Texture for "MyBarExBackground"
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
 BarTexture "Replaceabletextures\Teamcolor\Teamcolor00.blp",
 Layer "BACKGROUND" {
 Texture "MyBarBackground" {
 File "Replaceabletextures\Teamcolor\Teamcolor27.blp",
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

##
 Example MyBar

This jass code creates 3 simpleStatusBars 2 "MyBarEx" and 1 "MyBar". The "MyBarEx" will show the % life and mana of the unit selected. "MyBar" shows the 2 background bar usage with paladin enabled and paladin disabled icon.

Cause a SIMPLESTATUSBAR can only show blp one uses teamcolors to give a bar a color.

 JASS:

function UpdateBars takes nothing returns nothing
 local unit u = GetTriggerUnit()
 call BlzFrameSetValue(BlzGetFrameByName("MyBarEx",1), GetUnitLifePercent(u)) //Load the Frame at ("MyBarEx",1) and set its value to the %HP
 call BlzFrameSetValue(BlzGetFrameByName("MyBarEx",2), GetUnitManaPercent(u))
 set u = null
endfunction

function MyBarCreate takes nothing returns nothing
 local framehandle bar = BlzCreateSimpleFrame("MyBarEx", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 1) //Create Bar at createContext 1
 local framehandle bar2 = BlzCreateSimpleFrame("MyBarEx", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 2) //createContext 2
 local framehandle bar4 = BlzCreateSimpleFrame("MyBar", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 4) //createContext 4, other names so would not be needed.
 call BlzFrameSetAbsPoint(bar, FRAMEPOINT_CENTER, 0.5, 0.3) // pos the bar
 call BlzFrameSetPoint(bar2, FRAMEPOINT_TOP, bar, FRAMEPOINT_BOTTOM, 0.0, 0.0) // pos bar2 below bar
 call BlzFrameSetPoint(bar4, FRAMEPOINT_BOTTOM, bar, FRAMEPOINT_TOP, 0.0, 0.0) // pos bar4 above bar
 call BlzFrameSetSize(bar4, 0.04, 0.04) //change the size of bar4

 call BlzFrameSetValue(bar4, 35) //Starting value for bar 4.

 call BlzFrameSetTexture(bar, "Replaceabletextures\\Teamcolor\\Teamcolor00.blp", 0, true) //change the BarTexture of bar to color red
 call BlzFrameSetTexture(bar2, "Replaceabletextures\\Teamcolor\\Teamcolor01.blp", 0, true) //color blue for bar2
 call BlzFrameSetTexture(bar4, "Replaceabletextures\\CommandButtons\\BTNHeroPaladin.blp", 0, true) //bar4 to Paladin-Face
 call BlzFrameSetTexture(BlzGetFrameByName("MyBarBackground",4), "Replaceabletextures\\CommandButtonsDisabled\\DISBTNHeroPaladin.blp", 0, true) //Change the background to DisabledPaladin-Face. ("MyBarBackground", 4) belongs to Bar4. would Bar4 be a "MyBarEx" one would have to write "MyBarExBackground" cause they are named differently in fdf.

 call BlzFrameSetText(BlzGetFrameByName("MyBarExText",1), "Life")
 call BlzFrameSetText(BlzGetFrameByName("MyBarExText",2), "Mana")
 call BlzFrameSetText(BlzGetFrameByName("MyBarText",4), I2S(R2I(BlzFrameGetValue(bar4)))+"%")

 call DisplayTimedTextToPlayer(GetLocalPlayer(),0,0, 99999, "Select an unit to update the Bars")
endfunction

//===========================================================================
function InitTrig_MyBar takes nothing returns nothing
 local trigger trig = CreateTrigger()
 set gg_trg_MyBar = CreateTrigger()
 call TriggerRegisterTimerEventSingle( gg_trg_MyBar, 0.00 )
 call TriggerAddAction( gg_trg_MyBar, function MyBarCreate )
 call BlzLoadTOCFile("war3mapimported\\mybar.toc")
 call TriggerRegisterPlayerSelectionEventBJ(trig, Player(0), true )
 call TriggerAddAction(trig, function UpdateBars)
endfunction

There is also the SimpleStatusbar_face2 map which randomly changes the face of arthas between living and deathknight.

 JASS:

function Change takes nothing returns nothing
 local framehandle fh = BlzGetFrameByName("MyBar",0)
 call BlzFrameSetValue(fh, BlzFrameGetValue(fh) + GetRandomReal(-3,3))
 set fh = null
endfunction
function Trig_Nahkampf_Initialisierung_Actions takes nothing returns nothing
 local framehandle fh = BlzCreateSimpleFrame("MyBar", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0)
 call BlzFrameSetAbsPoint(fh, FRAMEPOINT_CENTER, 0.5, 0.4)
 call BlzFrameSetValue(fh, 50)
 call BlzFrameSetText(BlzGetFrameByName("MyBarText",0), "")
 call BlzFrameSetTexture(BlzGetFrameByName("MyBarBackground",0), "Replaceabletextures\\CommandButtons\\BTNHeroDeathKnight.blp", 0,true)
 call BlzFrameSetTexture(fh, "Replaceabletextures\\CommandButtons\\BTNArthas.blp", 0,true)
 call BlzFrameSetSize(fh, 0.08, 0.08)
 call TimerStart(CreateTimer(), 0.08, true, function Change)
endfunction

//===========================================================================
function InitTrig_MyBar takes nothing returns nothing
 set gg_trg_MyBar = CreateTrigger( )
 call TriggerRegisterTimerEventSingle( gg_trg_MyBar, 0.00 )
 call TriggerAddAction( gg_trg_MyBar, function Trig_Nahkampf_Initialisierung_Actions )
 call LoadToc("war3mapimported\\mybar.toc")
endfunction

It is also possible to use a white Texture and change the color of the bar using









 JASS:

BlzFrameSetTexture(bar, "ui\\feedback\\progressbar\\human-statbar-color", 0, true)
BlzFrameSetVertexColor(bar, BlzConvertColor(255, 255, 255, 1))

 This lines crashed for me in 1.31.1, but work fine in 1.32.1 (reforged).​

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

MyBar.w3x

 19.5 KB

 · Views: 1,042



SimpleStatusBar_Face2.w3x

 17.7 KB

 · Views: 656




 Last edited by a moderator: Oct 7, 2020

#### xorkatoss

#### xorkatoss

 Level 20

JoinedJul 12, 2010Messages1,746

Resources: 10

#### Resources
Models 6Maps 1Spells and Systems 3

Awesome example of how to make hp/mana bars!

Can I just leave this here for people that want the bars to show numbers?

 JASS:

function UpdateBars takes nothing returns nothing
 local unit u = GetTriggerUnit()
 local string h = I2S(R2I(GetUnitStateSwap(UNIT_STATE_LIFE, u)))
 local string mh = I2S(BlzGetUnitMaxHP(u))
 local string m = I2S(R2I(GetUnitStateSwap(UNIT_STATE_MANA, u)))
 local string mm = I2S(BlzGetUnitMaxMana(u))
 call BlzFrameSetValue(BlzGetFrameByName("MyBarEx",1), GetUnitLifePercent(u))
 call BlzFrameSetValue(BlzGetFrameByName("MyBarEx",2), GetUnitManaPercent(u))
 call BlzFrameSetText(BlzGetFrameByName("MyBarExText",1), h + "/" + mh)
 call BlzFrameSetText(BlzGetFrameByName("MyBarExText",2), m + "/" + mm)
 set u = null
 set h = null
 set mh = null
 set m = null
 set mm = null
endfunction

p.s I'm not sure if this the best and more efficient way to do it but it gets the job done.

#### deepstrasz

#### deepstrasz

Map Reviewer

 Level 84

JoinedJun 4, 2009Messages22,555

Resources: 7

#### Resources
Maps 4Spells and Systems 2Sounds 1

Could this be converted to GUI one day? I direly hope so.

#### FeelsGoodMan

#### FeelsGoodMan

 Level 46

JoinedDec 13, 2018Messages2,366

Resources: 46

#### Resources
Models 1Tools 1Maps 3Templates 3Ported Models 34User Interface 1Tutorials 3

##
 Acolyte of Life


deepstrasz said:

 Could this be converted to GUI one day? I direly hope so.


Click to expand...

I dream of that too, for us normal GUI peasants.

#### WyrmSlayer

#### WyrmSlayer

 Level 7

JoinedApr 18, 2010Messages104

Resources: 4

#### Resources
Maps 2Spells and Systems 2

Do this hp mp bar apply for all players?

(After changing the event of unit selecting to apply for all players?)

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

When one would add more selection Events, then each selection would overwrite the shown value of the Frame, which is shown to everyone.

To not have this happen one either surrounds the updating of the Frame in GetLocalPlayer so it differs or create 1 Frame for each Player and make them right after the creation invisible for other players which again needs GetLocalPlayer, but only once at the creation, not in the writing.

#### Rhobox

#### Rhobox

 Level 4

JoinedJul 31, 2019Messages19

I attempted to port this over to Lua to no avail. I have it initalize in a map init function and I've tried a very short timer (0.1s and 1s) and nothing makes the bars actually show up.

 Code:

function MyBarCreate()
 local framehandle bar = BlzCreateSimpleFrame("MyBarEx", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 1)
 local framehandle bar2 = BlzCreateSimpleFrame("MyBarEx", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 2)
 local framehandle bar4 = BlzCreateSimpleFrame("MyBar", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI,0), 4)

 BJDebugMsg("Variables created")

 BlzFrameSetAbsPoint(bar, FRAMEPOINT_CENTER, 0.5, 0.3)
 BlzFrameSetPoint(bar2, FRAMEPOINT_TOP, bar, FRAMEPOINT_BOTTOM, 0,0, 0.0)
 BlzFrameSetPoint(bar4, FRAMEPOINT_BOTTOM, bar, FRAMEPOINT_TOP, 0.0, 0.0)
 BlzFrameSetSize(bar4, 0.04, 0.04)

 BJDebugMsg("Locations assigned")

 BlzFrameSetValue(bar4, 35)
 BJDebugMsg("Values Assigned")

 BlzFrameSetTexture(bar, "Replaceabletextures\\Teamcolor\\Teamcolor00.blp", 0, true)
 BlzFrameSetTexture(bar2, "Replaceabletextures\\Teamcolor\\Teamcolor01.blp", 0, true)
 BlzFrameSetTexture(bar4, "Replaceabletextures\\CommandButtons\\BTNHeroPaladin.blp", 0, true)
 BlzFrameSetTexture(BlzGetFrameByName("MyBarBackground", 4), "Replaceabletextures\\CommandButtonsDisabled\\DISBTNHeroPaladin.blp", 0, true)
 local framehandle fh = BlzGetFrameByName("MyBarExText",1)
 BlzFrameSetText(fh, "Life")
 fh = BlzGetFrameByName("MyBarExText",2)
 BlzFrameSetText(fh, "Mana")
 fh = BlzGetFrameByName("MyBarText",4)
 BlzFrameSetText(fh, tostring(R2I(BlzFrameGetValue(bar4))).."%")
 BJDebugMsg("Bars Initialized")
end

function UpdateBars()
 local unit u = GetTriggerUnit()
 BlzFrameSetValue(BlzGetFrameByName("MyBarEx", 1), GetUnitLifePercent(u))
 BlzFrameSetValue(BlzGetFrameByName("MyBarEx", 2), GetUnitManaPercent(u))
 u = nil
 BJDebugMsg("Wow!")
end

function InitTrig_MyBar()
 local trigger trig = CreateTrigger()
 BlzLoadTOCFile("war3mapimported\\MyBar.toc")
 TriggerRegisterPlayerSelectionEventBJ(trig, Player(0), true)
 TriggerAddAction(trig, UpdateBars)
end

For some reason it was erroring when I wasn't calling the BlzGetFrameByName outside the BlzFrameSetText area...

If you are curious here is the other piece:

 Code:

function Initialize_Lua()
 InitTrig_MyBar()
 SpawnHero()
 InitTrig_Change_Hero_Stats()
 InitTrig_Damage_tags()
 InitTrig_Override_Damage()
 InitTrig_Prevent_Death()
end

TimerStart(CreateTimer(), 1.00, false, function()
 BJDebugMsg("Attempting to create bars")
 MyBarCreate()
end)

Hey so I found my issue. I was working on my map having it saved as a folder. This seems to have a bug where it can't save properly using the save command and it deletes the contents of said folder and creates a backup file. Saving again saves properly, however it was clearing my war3mapImported folder every time, so imports just weren't happening.

When I no longer use the folder saves it now works fine. Thanks!


 Last edited by a moderator: Sep 24, 2019

#### bgcy_ro002

#### bgcy_ro002

 Level 5

JoinedSep 6, 2013Messages21

Resources: 5

#### Resources
Icons 1Spells and Systems 2User Interface 2

Hello Tasyen,

Annoying you again.

I want create a HP-sphere interface and i was designed it.

But i dont know how to vertically changes the face of arthas between living and deathknight (not horizontally as your demo).

Could you help me guide my problem or other solution if have?

If you don't have time, could you give me some suggestions? Or could you give me any-demo have with that, I will research them by myself.

Thank you

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Sry, I don't know how one can setup a SIMPLESTATUSBAR to be top-bottom wise.

#### bgcy_ro002

#### bgcy_ro002

 Level 5

JoinedSep 6, 2013Messages21

Resources: 5

#### Resources
Icons 1Spells and Systems 2User Interface 2

 But i dont know how to vertically changes the face of arthas between living and deathknight (not horizontally as your demo).


Click to expand...

Yeah, it worked. This is a waste of resources but I think I still accept it.

Hmmm, i think i will remake texture. looks pretty bad

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

@bgcy_ro002 : Hello there, it would be nice for other players if you explained how you managed to change the HP / Mana Status vertically ;-)

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

Hello @Tasyen !!!

For a change, I am not posting a request or a question, but I just wanted tothank you a million times for your patience and time.

With your restless advice, I have finally managed to get the custom UI I wanted for my map. Well let's say at least considering there are still things that are impossible until Blizzards fixes some culprits of the API.

This said, reading your tutorials again and again allowed me to understand so much more, and everytime I was lost your advice put me back on track.

Without you, this attempt would have been a failure !

It wasn't an easy task especially while learning Lua and dealing with its strange excessive tolerance with variables, meaning that sometimes a hidden typo somewhere in a small function you had nearly completely forgotten about, renders most of your map buggy and crashing in unexpected ways.

I also wanted to tell you about this HP Bar (same things of course would apply to a Mana Bar or any other Bar based on your nice tutorial).

I have a special trigger that manages all my players selections. And I also consider when a player unit or a creep takes damages (works fine, even without any damage detection system). It also takes into account when a creep (Ball in my map as you know ^^) dies or when a tower is killed by a Ball, or even when a tower is sold.

It works perfectly!!! At least locally, I hope it will in multiplayer because I haven't been able to test it in that context yet, but I am confident, as I use an array to store each player Bar, hide them for other players once created, so all I need to do later on is update the values.

Something nice too I have managed is make the Bar take the color of the owner of the selected unit rather than the color of the owner of the Bar.

To make sure the system works, I also have used an Array of units (again a small array of size the number of players in the game) to store the current Main selected unit by a player. This prevents collisions in case of multiple selections.

Alas, for reasons I have not yet found, when I try to select a unit within a group of selected units, it selects it and deselects the others, instead of just putting this one ahead. God knows what setting I messed up with. I wish I remembered !

So my system has a small drawback, as I have not managed the case of main selected units in a multiple selection, a scenario you warned me about before I started working on the Bars.

Besides thanking you, I also post this to help players that would want to know how I have done it.

Truth is it is very simple, and I don't think I need to post any Lua triggers here as what I have described should be enough for any player with enough base knowledge to start tinkering with the UI API, to find out very easily.

Now here are a couple of screenshots of my UI. The first one is showing the (Self updating) HP Bar on one of the player's towers taking damage :

Notes :

- I am using a custom white teamcolor for tha bar texture as you suggested, a whiter version of the one found in the game data.

- I am using the default tooltip background for the Bar background texture. (both these have been changed in the fdf file)

- Once the tower dies and is removed from the game, the Range Indicator is removed, and the Bar looses its gauge and text.

This one is when you select another player's unit :

There is no Range Indicator for other players units to avoid rendering visuals too messy. Of course the bar also updates itself when the unit looses life.

I have not yet taken into consideration the case when a unit gains HP because it never happens in my map (for the moment at least), but it would be pretty easy to implement.

There you are, there might be small details to improve like making the vertical camera zoom slider (something else I am grateful to you for) have the same height as the Portrait, or improving the background textures for the Infopanel, Resource bar or Command Bar. But I am very satisfied overall with the result.

If anyone else is ready this and needs some advice on achieving something similar, do not hesitate to contact me.

Again, master @Tasyen : THANK YOU !!!

EDIT : Anyone knows when there will be a dedicated UI section in the tutorials section ?

If would be nice to centralize knowledge since the introduction of the API.

And we could also then rate the hard work made here.

EDIT 2 : For those wondering, the reason I do not display Max HP besides HP is not a technical limitation, but just the fact that I would not have enough width to display both as the Balls in my maps can end up with considerable HP.


 Last edited: Apr 18, 2020

#### bgcy_ro002

#### bgcy_ro002

 Level 5

JoinedSep 6, 2013Messages21

Resources: 5

#### Resources
Icons 1Spells and Systems 2User Interface 2

Macadamia said:

@bgcy_ro002 : Hello there, it would be nice for other players if you explained how you managed to change the HP / Mana Status vertically ;-)


Click to expand...

I just use TexCoord of SIMPLEFRAME but i use more SIMPLEFRAME for each pixel

and I used C/C++ language to generate fdf files

 Code:

#include "stdio.h"
#include "string.[code]h"
int main() {

 /* Declare variables */
 FILE *f;
 f = fopen("HP-sphere.fdf.", "w");


 for(unsigned int i=1; i<128; ++i) {
 fprintf(f, "Frame \"SIMPLEFRAME\" \"HPS%d\" {\n", i);
 fprintf(f, " Width 0.04,\n");
 fprintf(f, " Height 0.04,\n");
 fprintf(f, " Texture \"HPS%dV\" {\n", i);
 fprintf(f, " TexCoord 0.0, 1.0, %f, 1.0,\n", (float)(128.00F - (float)i) / 128.00F);
 fprintf(f, " }\n");
 fprintf(f, "}\n\n");
 }

 fprintf(f, "Frame \"SIMPLEFRAME\" \"HPS128\" {\n");
 fprintf(f, " Width 0.04,\n");
 fprintf(f, " Height 0.04,\n");
 fprintf(f, " Texture \"HPS128V\" {\n");
 fprintf(f, " }\n");
 fprintf(f, "}");
 return 0;
}

 Code:

Frame "SIMPLEFRAME" "HPS1" {
 Width 0.04,
 Height 0.04,
 Texture "HPS1V" {
 TexCoord 0.0, 1.0, 0.992188, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS2" {
 Width 0.04,
 Height 0.04,
 Texture "HPS2V" {
 TexCoord 0.0, 1.0, 0.984375, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS3" {
 Width 0.04,
 Height 0.04,
 Texture "HPS3V" {
 TexCoord 0.0, 1.0, 0.976563, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS4" {
 Width 0.04,
 Height 0.04,
 Texture "HPS4V" {
 TexCoord 0.0, 1.0, 0.968750, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS5" {
 Width 0.04,
 Height 0.04,
 Texture "HPS5V" {
 TexCoord 0.0, 1.0, 0.960938, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS6" {
 Width 0.04,
 Height 0.04,
 Texture "HPS6V" {
 TexCoord 0.0, 1.0, 0.953125, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS7" {
 Width 0.04,
 Height 0.04,
 Texture "HPS7V" {
 TexCoord 0.0, 1.0, 0.945313, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS8" {
 Width 0.04,
 Height 0.04,
 Texture "HPS8V" {
 TexCoord 0.0, 1.0, 0.937500, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS9" {
 Width 0.04,
 Height 0.04,
 Texture "HPS9V" {
 TexCoord 0.0, 1.0, 0.929688, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS10" {
 Width 0.04,
 Height 0.04,
 Texture "HPS10V" {
 TexCoord 0.0, 1.0, 0.921875, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS11" {
 Width 0.04,
 Height 0.04,
 Texture "HPS11V" {
 TexCoord 0.0, 1.0, 0.914063, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS12" {
 Width 0.04,
 Height 0.04,
 Texture "HPS12V" {
 TexCoord 0.0, 1.0, 0.906250, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS13" {
 Width 0.04,
 Height 0.04,
 Texture "HPS13V" {
 TexCoord 0.0, 1.0, 0.898438, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS14" {
 Width 0.04,
 Height 0.04,
 Texture "HPS14V" {
 TexCoord 0.0, 1.0, 0.890625, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS15" {
 Width 0.04,
 Height 0.04,
 Texture "HPS15V" {
 TexCoord 0.0, 1.0, 0.882813, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS16" {
 Width 0.04,
 Height 0.04,
 Texture "HPS16V" {
 TexCoord 0.0, 1.0, 0.875000, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS17" {
 Width 0.04,
 Height 0.04,
 Texture "HPS17V" {
 TexCoord 0.0, 1.0, 0.867188, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS18" {
 Width 0.04,
 Height 0.04,
 Texture "HPS18V" {
 TexCoord 0.0, 1.0, 0.859375, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS19" {
 Width 0.04,
 Height 0.04,
 Texture "HPS19V" {
 TexCoord 0.0, 1.0, 0.851563, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS20" {
 Width 0.04,
 Height 0.04,
 Texture "HPS20V" {
 TexCoord 0.0, 1.0, 0.843750, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS21" {
 Width 0.04,
 Height 0.04,
 Texture "HPS21V" {
 TexCoord 0.0, 1.0, 0.835938, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS22" {
 Width 0.04,
 Height 0.04,
 Texture "HPS22V" {
 TexCoord 0.0, 1.0, 0.828125, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS23" {
 Width 0.04,
 Height 0.04,
 Texture "HPS23V" {
 TexCoord 0.0, 1.0, 0.820313, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS24" {
 Width 0.04,
 Height 0.04,
 Texture "HPS24V" {
 TexCoord 0.0, 1.0, 0.812500, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS25" {
 Width 0.04,
 Height 0.04,
 Texture "HPS25V" {
 TexCoord 0.0, 1.0, 0.804688, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS26" {
 Width 0.04,
 Height 0.04,
 Texture "HPS26V" {
 TexCoord 0.0, 1.0, 0.796875, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS27" {
 Width 0.04,
 Height 0.04,
 Texture "HPS27V" {
 TexCoord 0.0, 1.0, 0.789063, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS28" {
 Width 0.04,
 Height 0.04,
 Texture "HPS28V" {
 TexCoord 0.0, 1.0, 0.781250, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS29" {
 Width 0.04,
 Height 0.04,
 Texture "HPS29V" {
 TexCoord 0.0, 1.0, 0.773438, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS30" {
 Width 0.04,
 Height 0.04,
 Texture "HPS30V" {
 TexCoord 0.0, 1.0, 0.765625, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS31" {
 Width 0.04,
 Height 0.04,
 Texture "HPS31V" {
 TexCoord 0.0, 1.0, 0.757813, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS32" {
 Width 0.04,
 Height 0.04,
 Texture "HPS32V" {
 TexCoord 0.0, 1.0, 0.750000, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS33" {
 Width 0.04,
 Height 0.04,
 Texture "HPS33V" {
 TexCoord 0.0, 1.0, 0.742188, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS34" {
 Width 0.04,
 Height 0.04,
 Texture "HPS34V" {
 TexCoord 0.0, 1.0, 0.734375, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS35" {
 Width 0.04,
 Height 0.04,
 Texture "HPS35V" {
 TexCoord 0.0, 1.0, 0.726563, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS36" {
 Width 0.04,
 Height 0.04,
 Texture "HPS36V" {
 TexCoord 0.0, 1.0, 0.718750, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS37" {
 Width 0.04,
 Height 0.04,
 Texture "HPS37V" {
 TexCoord 0.0, 1.0, 0.710938, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS38" {
 Width 0.04,
 Height 0.04,
 Texture "HPS38V" {
 TexCoord 0.0, 1.0, 0.703125, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS39" {
 Width 0.04,
 Height 0.04,
 Texture "HPS39V" {
 TexCoord 0.0, 1.0, 0.695313, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS40" {
 Width 0.04,
 Height 0.04,
 Texture "HPS40V" {
 TexCoord 0.0, 1.0, 0.687500, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS41" {
 Width 0.04,
 Height 0.04,
 Texture "HPS41V" {
 TexCoord 0.0, 1.0, 0.679688, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS42" {
 Width 0.04,
 Height 0.04,
 Texture "HPS42V" {
 TexCoord 0.0, 1.0, 0.671875, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS43" {
 Width 0.04,
 Height 0.04,
 Texture "HPS43V" {
 TexCoord 0.0, 1.0, 0.664063, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS44" {
 Width 0.04,
 Height 0.04,
 Texture "HPS44V" {
 TexCoord 0.0, 1.0, 0.656250, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS45" {
 Width 0.04,
 Height 0.04,
 Texture "HPS45V" {
 TexCoord 0.0, 1.0, 0.648438, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS46" {
 Width 0.04,
 Height 0.04,
 Texture "HPS46V" {
 TexCoord 0.0, 1.0, 0.640625, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS47" {
 Width 0.04,
 Height 0.04,
 Texture "HPS47V" {
 TexCoord 0.0, 1.0, 0.632813, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS48" {
 Width 0.04,
 Height 0.04,
 Texture "HPS48V" {
 TexCoord 0.0, 1.0, 0.625000, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS49" {
 Width 0.04,
 Height 0.04,
 Texture "HPS49V" {
 TexCoord 0.0, 1.0, 0.617188, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS50" {
 Width 0.04,
 Height 0.04,
 Texture "HPS50V" {
 TexCoord 0.0, 1.0, 0.609375, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS51" {
 Width 0.04,
 Height 0.04,
 Texture "HPS51V" {
 TexCoord 0.0, 1.0, 0.601563, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS52" {
 Width 0.04,
 Height 0.04,
 Texture "HPS52V" {
 TexCoord 0.0, 1.0, 0.593750, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS53" {
 Width 0.04,
 Height 0.04,
 Texture "HPS53V" {
 TexCoord 0.0, 1.0, 0.585938, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS54" {
 Width 0.04,
 Height 0.04,
 Texture "HPS54V" {
 TexCoord 0.0, 1.0, 0.578125, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS55" {
 Width 0.04,
 Height 0.04,
 Texture "HPS55V" {
 TexCoord 0.0, 1.0, 0.570313, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS56" {
 Width 0.04,
 Height 0.04,
 Texture "HPS56V" {
 TexCoord 0.0, 1.0, 0.562500, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS57" {
 Width 0.04,
 Height 0.04,
 Texture "HPS57V" {
 TexCoord 0.0, 1.0, 0.554688, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS58" {
 Width 0.04,
 Height 0.04,
 Texture "HPS58V" {
 TexCoord 0.0, 1.0, 0.546875, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS59" {
 Width 0.04,
 Height 0.04,
 Texture "HPS59V" {
 TexCoord 0.0, 1.0, 0.539063, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS60" {
 Width 0.04,
 Height 0.04,
 Texture "HPS60V" {
 TexCoord 0.0, 1.0, 0.531250, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS61" {
 Width 0.04,
 Height 0.04,
 Texture "HPS61V" {
 TexCoord 0.0, 1.0, 0.523438, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS62" {
 Width 0.04,
 Height 0.04,
 Texture "HPS62V" {
 TexCoord 0.0, 1.0, 0.515625, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS63" {
 Width 0.04,
 Height 0.04,
 Texture "HPS63V" {
 TexCoord 0.0, 1.0, 0.507813, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS64" {
 Width 0.04,
 Height 0.04,
 Texture "HPS64V" {
 TexCoord 0.0, 1.0, 0.500000, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS65" {
 Width 0.04,
 Height 0.04,
 Texture "HPS65V" {
 TexCoord 0.0, 1.0, 0.492188, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS66" {
 Width 0.04,
 Height 0.04,
 Texture "HPS66V" {
 TexCoord 0.0, 1.0, 0.484375, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS67" {
 Width 0.04,
 Height 0.04,
 Texture "HPS67V" {
 TexCoord 0.0, 1.0, 0.476563, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS68" {
 Width 0.04,
 Height 0.04,
 Texture "HPS68V" {
 TexCoord 0.0, 1.0, 0.468750, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS69" {
 Width 0.04,
 Height 0.04,
 Texture "HPS69V" {
 TexCoord 0.0, 1.0, 0.460938, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS70" {
 Width 0.04,
 Height 0.04,
 Texture "HPS70V" {
 TexCoord 0.0, 1.0, 0.453125, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS71" {
 Width 0.04,
 Height 0.04,
 Texture "HPS71V" {
 TexCoord 0.0, 1.0, 0.445313, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS72" {
 Width 0.04,
 Height 0.04,
 Texture "HPS72V" {
 TexCoord 0.0, 1.0, 0.437500, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS73" {
 Width 0.04,
 Height 0.04,
 Texture "HPS73V" {
 TexCoord 0.0, 1.0, 0.429688, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS74" {
 Width 0.04,
 Height 0.04,
 Texture "HPS74V" {
 TexCoord 0.0, 1.0, 0.421875, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS75" {
 Width 0.04,
 Height 0.04,
 Texture "HPS75V" {
 TexCoord 0.0, 1.0, 0.414063, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS76" {
 Width 0.04,
 Height 0.04,
 Texture "HPS76V" {
 TexCoord 0.0, 1.0, 0.406250, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS77" {
 Width 0.04,
 Height 0.04,
 Texture "HPS77V" {
 TexCoord 0.0, 1.0, 0.398438, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS78" {
 Width 0.04,
 Height 0.04,
 Texture "HPS78V" {
 TexCoord 0.0, 1.0, 0.390625, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS79" {
 Width 0.04,
 Height 0.04,
 Texture "HPS79V" {
 TexCoord 0.0, 1.0, 0.382813, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS80" {
 Width 0.04,
 Height 0.04,
 Texture "HPS80V" {
 TexCoord 0.0, 1.0, 0.375000, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS81" {
 Width 0.04,
 Height 0.04,
 Texture "HPS81V" {
 TexCoord 0.0, 1.0, 0.367188, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS82" {
 Width 0.04,
 Height 0.04,
 Texture "HPS82V" {
 TexCoord 0.0, 1.0, 0.359375, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS83" {
 Width 0.04,
 Height 0.04,
 Texture "HPS83V" {
 TexCoord 0.0, 1.0, 0.351563, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS84" {
 Width 0.04,
 Height 0.04,
 Texture "HPS84V" {
 TexCoord 0.0, 1.0, 0.343750, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS85" {
 Width 0.04,
 Height 0.04,
 Texture "HPS85V" {
 TexCoord 0.0, 1.0, 0.335938, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS86" {
 Width 0.04,
 Height 0.04,
 Texture "HPS86V" {
 TexCoord 0.0, 1.0, 0.328125, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS87" {
 Width 0.04,
 Height 0.04,
 Texture "HPS87V" {
 TexCoord 0.0, 1.0, 0.320313, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS88" {
 Width 0.04,
 Height 0.04,
 Texture "HPS88V" {
 TexCoord 0.0, 1.0, 0.312500, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS89" {
 Width 0.04,
 Height 0.04,
 Texture "HPS89V" {
 TexCoord 0.0, 1.0, 0.304688, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS90" {
 Width 0.04,
 Height 0.04,
 Texture "HPS90V" {
 TexCoord 0.0, 1.0, 0.296875, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS91" {
 Width 0.04,
 Height 0.04,
 Texture "HPS91V" {
 TexCoord 0.0, 1.0, 0.289063, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS92" {
 Width 0.04,
 Height 0.04,
 Texture "HPS92V" {
 TexCoord 0.0, 1.0, 0.281250, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS93" {
 Width 0.04,
 Height 0.04,
 Texture "HPS93V" {
 TexCoord 0.0, 1.0, 0.273438, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS94" {
 Width 0.04,
 Height 0.04,
 Texture "HPS94V" {
 TexCoord 0.0, 1.0, 0.265625, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS95" {
 Width 0.04,
 Height 0.04,
 Texture "HPS95V" {
 TexCoord 0.0, 1.0, 0.257813, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS96" {
 Width 0.04,
 Height 0.04,
 Texture "HPS96V" {
 TexCoord 0.0, 1.0, 0.250000, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS97" {
 Width 0.04,
 Height 0.04,
 Texture "HPS97V" {
 TexCoord 0.0, 1.0, 0.242188, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS98" {
 Width 0.04,
 Height 0.04,
 Texture "HPS98V" {
 TexCoord 0.0, 1.0, 0.234375, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS99" {
 Width 0.04,
 Height 0.04,
 Texture "HPS99V" {
 TexCoord 0.0, 1.0, 0.226563, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS100" {
 Width 0.04,
 Height 0.04,
 Texture "HPS100V" {
 TexCoord 0.0, 1.0, 0.218750, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS101" {
 Width 0.04,
 Height 0.04,
 Texture "HPS101V" {
 TexCoord 0.0, 1.0, 0.210938, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS102" {
 Width 0.04,
 Height 0.04,
 Texture "HPS102V" {
 TexCoord 0.0, 1.0, 0.203125, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS103" {
 Width 0.04,
 Height 0.04,
 Texture "HPS103V" {
 TexCoord 0.0, 1.0, 0.195313, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS104" {
 Width 0.04,
 Height 0.04,
 Texture "HPS104V" {
 TexCoord 0.0, 1.0, 0.187500, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS105" {
 Width 0.04,
 Height 0.04,
 Texture "HPS105V" {
 TexCoord 0.0, 1.0, 0.179688, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS106" {
 Width 0.04,
 Height 0.04,
 Texture "HPS106V" {
 TexCoord 0.0, 1.0, 0.171875, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS107" {
 Width 0.04,
 Height 0.04,
 Texture "HPS107V" {
 TexCoord 0.0, 1.0, 0.164063, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS108" {
 Width 0.04,
 Height 0.04,
 Texture "HPS108V" {
 TexCoord 0.0, 1.0, 0.156250, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS109" {
 Width 0.04,
 Height 0.04,
 Texture "HPS109V" {
 TexCoord 0.0, 1.0, 0.148438, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS110" {
 Width 0.04,
 Height 0.04,
 Texture "HPS110V" {
 TexCoord 0.0, 1.0, 0.140625, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS111" {
 Width 0.04,
 Height 0.04,
 Texture "HPS111V" {
 TexCoord 0.0, 1.0, 0.132813, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS112" {
 Width 0.04,
 Height 0.04,
 Texture "HPS112V" {
 TexCoord 0.0, 1.0, 0.125000, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS113" {
 Width 0.04,
 Height 0.04,
 Texture "HPS113V" {
 TexCoord 0.0, 1.0, 0.117188, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS114" {
 Width 0.04,
 Height 0.04,
 Texture "HPS114V" {
 TexCoord 0.0, 1.0, 0.109375, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS115" {
 Width 0.04,
 Height 0.04,
 Texture "HPS115V" {
 TexCoord 0.0, 1.0, 0.101563, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS116" {
 Width 0.04,
 Height 0.04,
 Texture "HPS116V" {
 TexCoord 0.0, 1.0, 0.093750, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS117" {
 Width 0.04,
 Height 0.04,
 Texture "HPS117V" {
 TexCoord 0.0, 1.0, 0.085938, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS118" {
 Width 0.04,
 Height 0.04,
 Texture "HPS118V" {
 TexCoord 0.0, 1.0, 0.078125, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS119" {
 Width 0.04,
 Height 0.04,
 Texture "HPS119V" {
 TexCoord 0.0, 1.0, 0.070313, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS120" {
 Width 0.04,
 Height 0.04,
 Texture "HPS120V" {
 TexCoord 0.0, 1.0, 0.062500, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS121" {
 Width 0.04,
 Height 0.04,
 Texture "HPS121V" {
 TexCoord 0.0, 1.0, 0.054688, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS122" {
 Width 0.04,
 Height 0.04,
 Texture "HPS122V" {
 TexCoord 0.0, 1.0, 0.046875, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS123" {
 Width 0.04,
 Height 0.04,
 Texture "HPS123V" {
 TexCoord 0.0, 1.0, 0.039063, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS124" {
 Width 0.04,
 Height 0.04,
 Texture "HPS124V" {
 TexCoord 0.0, 1.0, 0.031250, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS125" {
 Width 0.04,
 Height 0.04,
 Texture "HPS125V" {
 TexCoord 0.0, 1.0, 0.023438, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS126" {
 Width 0.04,
 Height 0.04,
 Texture "HPS126V" {
 TexCoord 0.0, 1.0, 0.015625, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS127" {
 Width 0.04,
 Height 0.04,
 Texture "HPS127V" {
 TexCoord 0.0, 1.0, 0.007813, 1.0,
 }
}
Frame "SIMPLEFRAME" "HPS128" {
 Width 0.04,
 Height 0.04,
 Texture "HPS128V" {
 }
}

#### Attachments

upload_2020-5-14_16-26-14.png

 3.6 MB

 · Views: 526



upload_2020-5-14_16-26-32.png

 3.6 MB

 · Views: 549



upload_2020-5-14_16-27-37.png

 3.6 MB

 · Views: 494



#### bgcy_ro002

#### bgcy_ro002

 Level 5

JoinedSep 6, 2013Messages21

Resources: 5

#### Resources
Icons 1Spells and Systems 2User Interface 2

My UI with photoshop design

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

Wow great approach I love it, it reminds me what process I went through to conceive the model for my scrolling text panels on the floor, I should have thought about that, I have no excuse

Amazing job, and it really looks great. Hats off, and thanks for sharing, much appreciated !!!

#### micca

#### micca

 Level 9

JoinedDec 31, 2016Messages320

Resources: 1

#### Resources
Maps 1

@Macadamia wow that looks so amazing! Is the map available for download? I'd like to try something so custom UI based like that.

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

Yes, currently battling a couple last bugs and I will release the latest version. It will be quite a leap from the old version.

Still wondering if I should post it in my signature thread in Map Development or submit it in Map Resources as the map seems to reach maturity

#### micca

#### micca

 Level 9

JoinedDec 31, 2016Messages320

Resources: 1

#### Resources
Maps 1

Looking forward to it, will definitially play it.

#### Macadamia

#### Macadamia

 Level 12

JoinedJan 30, 2020Messages875

Well thanks, I hope it will be enjoyable.

So far it seems I got rid of all the remaining bugs - at least all those I could find -

Need to do some multiplayer tests to try to find potential desyncs or other bugs I would have missed,

and also to make sure the balance has not suffered too much from the huge amount of changes I have made since v1.3b.

I don't expect this map to have the most amazing gameplay among TDs, but at least I am sure it will be the only one of its kind in many aspects.

@Tasyen : sorry, I did not intend to hijack this important thread about UI !

#### SebioL

#### SebioL

 Level 4

JoinedMay 19, 2020Messages315

Resources: 3

#### Resources
Reforged HD Models 3

Tasyen said:

##
 What is a SIMPLESTATUSBAR

A SIMPLESTATUSBAR is a simple frame that shows a blp texture partly (from left to right) based on the SIMPLESTATUSBAR current value vs its max value. The blp Texture is stretched to the total frames size. On default the value can be from 0 to 100 (0 beeing empty, 100 full). 40 would show 40% of the blp. Its also possible to have a border an overlay text and adding a background blp texture. The barTexture is above the background and will hide more of it as nearer the value is to the max value ...

It is also possible to use a white Texture and change the color of the bar using









 JASS:

BlzFrameSetTexture(bar, "ui\\feedback\\progressbar\\human-statbar-color", 0, true)
BlzFrameSetVertexColor(bar, BlzConvertColor(255, 255, 255, 1))

 This lines crashed for me in 1.31.1, but work fine in 1.32.1 (reforged).


Click to expand...

It looks very good, but is it my impression or does it not update in real time?

It may only be on your test map, this lack of sync... But it seems that just having the unit selected is not enough.

The bar of "Life" or "Mana" to be updated, it is necessary to always be clicking again on the unit to show the current values.

PS. Forgive me if there is already a code with this verification in real time, but in the test map of topic, the status of the bar is not occurring instantly.

bgcy_ro002 said:

 I just use TexCoord of SIMPLEFRAME but i use more SIMPLEFRAME for each pixel

and I used C/C++ language to generate fdf files

. . .

[/code][/SPOILER]

View attachment 354824

Click to expand...

By the way ... your bar designer is great, Congrts.!


 Last edited: Sep 21, 2020

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

SebioL said:

 It looks very good, but is it my impression or does it not update in real time?

It may only be on your test map, this lack of sync... But it seems that just having the unit selected is not enough.

The bar of "Life" or "Mana" to be updated, it is necessary to always be clicking again on the unit to show the current values.

PS. Forgive me if there is already a code with this verification in real time, but in the test map of topic, the status of the bar is not occurring instantly.


Click to expand...

It is like you say the bar only updates on selection. There is no demo code that frequently updates the bars.

#### SebioL

#### SebioL

 Level 4

JoinedMay 19, 2020Messages315

Resources: 3

#### Resources
Reforged HD Models 3

Tasyen said:

 It is like you say the bar only updates on selection. There is no demo code that frequently updates the bars.


Click to expand...

 I understand.. I thought it already existed as a direct link in sync with the pre-existing values of life and mana.

The ideal would be to auto-update, mainly in UI maps like bgcy_ro002, when they are of the 3rd person/ Dota type, it would be essential.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Threw a few more hidden tags for some of the bigger jass sections. Otherwise nice and clean. And of course very useful, used it myself in the past.

Approved.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Just found out one could create a "SIMPLESTATUSBAR" with code only using

BlzCreateFrameByType. I didn't though about that yet because SIMPLEBUTTON didn't work so good when created with BlzCreateFrameByType.

selfexecuting Lua example

 Lua:

do
 local real = MarkGameStarted

 function MarkGameStarted()
 real()
 local frame = BlzCreateFrameByType("SIMPLESTATUSBAR", "", BlzGetFrameByName("ConsoleUI", 0), "", 0)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPLEFT, 0.4, 0.3)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_BOTTOMRIGHT, 0.47, 0.26)

 BlzFrameSetTexture(frame, "ReplaceableTextures\\CommandButtons\\BTNPriest", 0, false)
 --BlzFrameSetMinMaxValue(frame, 0, 100)
 TimerStart(CreateTimer(), 1, true, function()
 BlzFrameSetValue(frame, GetRandomInt(0, 100))
 end)

 end
end

This does not have Text-OverLay/Border/Background.

Just a priest icon that is partly shown based on the value.

#### Ericqwe

#### Ericqwe

 Level 4

JoinedOct 24, 2021Messages61

Macadamia said:

 Hello @Tasyen !!!

For a change, I am not posting a request or a question, but I just wanted tothank you a million times for your patience and time.

With your restless advice, I have finally managed to get the custom UI I wanted for my map. Well let's say at least considering there are still things that are impossible until Blizzards fixes some culprits of the API.

This said, reading your tutorials again and again allowed me to understand so much more, and everytime I was lost your advice put me back on track.

Without you, this attempt would have been a failure !

It wasn't an easy task especially while learning Lua and dealing with its strange excessive tolerance with variables, meaning that sometimes a hidden typo somewhere in a small function you had nearly completely forgotten about, renders most of your map buggy and crashing in unexpected ways.

I also wanted to tell you about this HP Bar (same things of course would apply to a Mana Bar or any other Bar based on your nice tutorial).

I have a special trigger that manages all my players selections. And I also consider when a player unit or a creep takes damages (works fine, even without any damage detection system). It also takes into account when a creep (Ball in my map as you know ^^) dies or when a tower is killed by a Ball, or even when a tower is sold.

It works perfectly!!! At least locally, I hope it will in multiplayer because I haven't been able to test it in that context yet, but I am confident, as I use an array to store each player Bar, hide them for other players once created, so all I need to do later on is update the values.

Something nice too I have managed is make the Bar take the color of the owner of the selected unit rather than the color of the owner of the Bar.

To make sure the system works, I also have used an Array of units (again a small array of size the number of players in the game) to store the current Main selected unit by a player. This prevents collisions in case of multiple selections.

Alas, for reasons I have not yet found, when I try to select a unit within a group of selected units, it selects it and deselects the others, instead of just putting this one ahead. God knows what setting I messed up with. I wish I remembered !

So my system has a small drawback, as I have not managed the case of main selected units in a multiple selection, a scenario you warned me about before I started working on the Bars.

Besides thanking you, I also post this to help players that would want to know how I have done it.

Truth is it is very simple, and I don't think I need to post any Lua triggers here as what I have described should be enough for any player with enough base knowledge to start tinkering with the UI API, to find out very easily.

Now here are a couple of screenshots of my UI. The first one is showing the (Self updating) HP Bar on one of the player's towers taking damage :

View attachment 352548

Notes :

I am using a custom white teamcolor for tha bar texture as you suggested, a whiter version of the one found in the game data.

I am using the default tooltip background for the Bar background texture. (both these have been changed in the fdf file)

Once the tower dies and is removed from the game, the Range Indicator is removed, and the Bar looses its gauge and text.

This one is when you select another player's unit :

View attachment 352549

There is no Range Indicator for other players units to avoid rendering visuals too messy. Of course the bar also updates itself when the unit looses life.

I have not yet taken into consideration the case when a unit gains HP because it never happens in my map (for the moment at least), but it would be pretty easy to implement.

There you are, there might be small details to improve like making the vertical camera zoom slider (something else I am grateful to you for) have the same height as the Portrait, or improving the background textures for the Infopanel, Resource bar or Command Bar. But I am very satisfied overall with the result.

If anyone else is ready this and needs some advice on achieving something similar, do not hesitate to contact me.

Again, master @Tasyen : THANK YOU !!!

EDIT : Anyone knows when there will be a dedicated UI section in the tutorials section ?

If would be nice to centralize knowledge since the introduction of the API.

And we could also then rate the hard work made here.

EDIT 2 : For those wondering, the reason I do not display Max HP besides HP is not a technical limitation, but just the fact that I would not have enough width to display both as the Balls in my maps can end up with considerable HP.


Click to expand...

And the map

#### ThompZon

#### ThompZon

 Level 28

JoinedJan 30, 2020Messages857

Resources: 20

#### Resources
Models 10Icons 3Packs 1Skins 1Maps 2Spells and Systems 1Ported Icons 2

I've been trying this out.

I want 3 layers in my UI (maybe 4). One bottom layer for background, one for an empty bar and finally a full SIMPLESTATUSBAR. (maybe a text on top).

The code places

Background: BACKDROP with parent: BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0)

Empty Bar: BACKDROP with parent: BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0)

Full Bar: SIMPLESTATUSBAR with parent: BlzGetFrameByName("ConsoleUI", 0)

The background and the empty bar is visible, but the SIMPLESTATUSBAR is not and I don't understand why. It should be in front of the 2 BACKDROPs, right?

Code in spoiler and 1.31 test-map attached.

 JASS:

globals
 framehandle ui_mainBackDrop = null

 framehandle ui_hpBarFull = null
 framehandle ui_hpBarEmpty = null
 framehandle ui_hpBarText = null

 framehandle ui_energyBarFull = null
 framehandle ui_energyBarEmpty = null
 framehandle ui_energyBarText = null

 constant real UI_MAIN_LEFT = 0.25
 constant real UI_MAIN_RIGHT = 0.55
 constant real UI_MAIN_TOP = 0.11
 constant real UI_MAIN_BOT = 0.0
 constant real UI_MAIN_INSET = 0.009
 constant real UI_MAIN_PADDING = 0.001

 constant real UI_BAR_ICON_SIZE_W = 0.0182
 constant real UI_BAR_ICON_SIZE_H = 0.0208
 constant real UI_BAR_ICON_LEFT = UI_MAIN_LEFT + UI_MAIN_INSET
 constant real UI_BAR_ICON_RIGHT = UI_BAR_ICON_LEFT + UI_BAR_ICON_SIZE_W

 constant real UI_BAR_LEFT = UI_BAR_ICON_LEFT + UI_BAR_ICON_SIZE_W
 constant real UI_BAR_RIGHT = UI_MAIN_RIGHT - UI_MAIN_INSET
 constant real UI_BAR_ENERGY_BOT = UI_MAIN_INSET * 0.8
 constant real UI_BAR_ENERGY_TOP = UI_BAR_ENERGY_BOT + UI_BAR_ICON_SIZE_H
 constant real UI_BAR_HP_BOT = UI_BAR_ENERGY_TOP + UI_MAIN_PADDING
 constant real UI_BAR_HP_TOP = UI_BAR_HP_BOT + UI_BAR_ICON_SIZE_H
endglobals

function hideBaseGui takes nothing returns nothing
 call BlzFrameSetAbsPoint(BlzGetFrameByName("ConsoleUI", 0), FRAMEPOINT_BOTTOM, 0.4, -0.18)
 call BlzFrameSetAllPoints(BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0))
 call BlzEnableUIAutoPosition(false)
 call BlzHideOriginFrames(true)
endfunction

function uiInit takes nothing returns nothing
 set ui_mainBackDrop = BlzCreateFrameByType("BACKDROP", "ui_mainBackDrop", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
 call BlzFrameSetAbsPoint(ui_mainBackDrop, FRAMEPOINT_TOPLEFT, UI_MAIN_LEFT, UI_MAIN_TOP)
 call BlzFrameSetAbsPoint(ui_mainBackDrop, FRAMEPOINT_BOTTOMRIGHT, UI_MAIN_RIGHT, UI_MAIN_BOT)
 call BlzFrameSetTexture(ui_mainBackDrop, "UI\\ScifiCardX2Rot.blp", 0, true)

//HP BAR
 set ui_hpBarEmpty = BlzCreateFrameByType("BACKDROP", "hpEmpty", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "", 0)
 call BlzFrameSetAbsPoint(ui_hpBarEmpty, FRAMEPOINT_TOPLEFT, UI_BAR_LEFT, UI_BAR_HP_TOP)
 call BlzFrameSetAbsPoint(ui_hpBarEmpty, FRAMEPOINT_BOTTOMRIGHT, UI_BAR_RIGHT, UI_BAR_HP_BOT)
 call BlzFrameSetTexture(ui_hpBarEmpty, "UI\\ScifiBarEmpty.blp", 0, true)
 set ui_hpBarFull = BlzCreateFrameByType("SIMPLESTATUSBAR", "hpFull", BlzGetFrameByName("ConsoleUI", 0), "", 0)
 call BlzFrameSetTexture(ui_hpBarFull, "UI\\ScifiBarFull.blp", 0, true)
 call BlzFrameSetAbsPoint(ui_hpBarFull, FRAMEPOINT_TOPLEFT, UI_BAR_LEFT, UI_BAR_HP_TOP)
 call BlzFrameSetAbsPoint(ui_hpBarFull, FRAMEPOINT_BOTTOMRIGHT, UI_BAR_RIGHT, UI_BAR_HP_BOT)
 call BlzFrameSetValue(ui_hpBarFull, 65)
endfunction

#### Attachments

UITestMap.w3x

 115.8 KB

 · Views: 153



#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


ConsoleUIBackdrop is layered below ORIGIN_FRAME_GAME_UI, so any frame with it as parent will always be below the ones that have the latter as parent.

ConsoleUIBackdrop is not accessible on 1.31, so BlzGetFrameByName("ConsoleUIBackdrop", 0) should not even return a valid frame.

After some testing, if you use ORIGIN_FRAME_WORLD_FRAME as the parent for all three frames, your bar will show up on top.


 Last edited: Oct 5, 2022

#### ThompZon

#### ThompZon

 Level 28

JoinedJan 30, 2020Messages857

Resources: 20

#### Resources
Models 10Icons 3Packs 1Skins 1Maps 2Spells and Systems 1Ported Icons 2

SmitingDevil said:

 ConsoleUIBackdrop is layered below ORIGIN_FRAME_GAME_UI, so any frame with it as parent will always be below the ones that have the latter as parent.

ConsoleUIBackdrop is not accessible on 1.31, so BlzGetFrameByName("ConsoleUIBackdrop", 0) should not even return a valid frame.

After some testing, if you use ORIGIN_FRAME_WORLD_FRAME as the parent for all three frames, your bar will show up on top.


Click to expand...

It thought it should. But it don't for some unknown reason...

My result when changing BlzGetFrameByName("ConsoleUI", 0) -> BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0)...

Also: I didn't realize that "ConsoleUI" didn't exist in 1.31, I thought that only the "ConsoleUIBackdrop" was new for later versions.

Edit2: barFull should look like:

(as seen if switching the empty <-> full images in the code)

#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


Currently in your test map, you have the bar value set to 65: call BlzFrameSetValue(ui_hpBarFull, 65)

You would set the value to 100 to get the full bar.

ORIGIN_FRAME_GAME_UI did not work for me either, but ORIGIN_FRAME_WORLD_FRAME did.

Typically, if you want a frame to be above another, you would set the latter as the parent of the former.

So, I would set the background BACKDROP as the parent of the empty bar, and then set the empty bar as the parent of the full bar.

However, setting the empty bar as the parent of the full bar is not necessary as long as the full bar's parent is above the background BACKDROP.

It also helps that the empty bar only has an outline and nothing in the middle.


 Last edited: Oct 6, 2022

#### ThompZon

#### ThompZon

 Level 28

JoinedJan 30, 2020Messages857

Resources: 20

#### Resources
Models 10Icons 3Packs 1Skins 1Maps 2Spells and Systems 1Ported Icons 2

SmitingDevil said:

 Currently in your test map, you have the bar value set to 65: call BlzFrameSetValue(ui_hpBarFull, 65)

You would set the value to 100 to get the full bar.

ORIGIN_FRAME_GAME_UI did not work for me either, but ORIGIN_FRAME_WORLD_FRAME did.

Typically, if you want a frame to be above another, you would set the latter as the parent of the former.

So, I would set the background BACKDROP as the parent of the empty bar, and then set the empty bar as the parent of the full bar.

However, setting the empty bar as the parent of the full bar is not necessary as long as the full bar's parent is above the background BACKDROP.

It also helps that the empty bar only has an outline and nothing in the middle.


Click to expand...

I'm aware that I set it to 65%. It's a good idea because I want to ensure that I can see both "empty" and "full" with a minimal code in this example.

I will try the ORIGIN_FRAME_WORLD_FRAME instead of ORIGIN_FRAME_GAME_UI later today (hopefully). Thanks!

#### ThompZon

#### ThompZon

 Level 28

JoinedJan 30, 2020Messages857

Resources: 20

#### Resources
Models 10Icons 3Packs 1Skins 1Maps 2Spells and Systems 1Ported Icons 2

Changing to:

Background: BACKDROP with parent: BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0)

Empty Bar: BACKDROP with parent: Background

Full Bar: SIMPLESTATUSBAR with parent: EmptyBar

Made it work as expected, thank you very much!

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

SIMPLESTATUSBAR can be rotated by placing it with Topleft and BottomRight. Revert the expected coords, so TopLeft is lower and further right than BottomRight.

This also works with Texture

 Lua:

bar = BlzCreateFrameByType("SIMPLESTATUSBAR", "", BlzGetFrameByName("ConsoleUI", 0), "", 0)
BlzFrameClearAllPoints(bar)
BlzFrameSetAbsPoint(bar, FRAMEPOINT_TOPLEFT, 0.5, 0.2)
BlzFrameSetAbsPoint(bar, FRAMEPOINT_BOTTOMRIGHT, 0.3, 0.3)
BlzFrameSetTexture(bar, "ReplaceableTextures\\CommandButtons\\BTNPriest", 0, false)
BlzFrameSetValue(bar, 60)

#### Coldgamer

#### Coldgamer

 Level 4

JoinedJun 4, 2019Messages37

Hey Tasyen, I was wondering if it is possible to attach a Tooltip to the bar? I tried this and I can see the bar but no tooltip:

 JASS:

local framehandle bar
local framehandle tooltip

set bar = BlzCreateSimpleFrame("MyBarEx", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), i)
call BlzFrameSetAbsPoint(bar, FRAMEPOINT_CENTER, 0.235, 0.09)
call BlzFrameSetTexture(bar, "Replaceabletextures\\Teamcolor\\Teamcolor03.blp", 0, true)
call BlzFrameSetText(BlzGetFrameByName("MyBarExText", i), "Evade")

set tooltip = BlzCreateSimpleFrame("MyBarTooltip", bar, 0)
call BlzFrameSetPoint(tooltip, FRAMEPOINT_TOP, bar, FRAMEPOINT_BOTTOM, 0, -0.01)
call BlzFrameSetText(tooltip, "Just a tooltip") //
call BlzFrameSetTooltip(bar, tooltip)

Thanks for your work!

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

The bar created does not take mouse control, therefore it can not have a tooltip. I dont know how to make the bar take control by itself. So I suggest to create a simplebutton ontop of the bar and make the simplebutton have the tooltip.

 Lua:

sButton = BlzCreateFrameByType("SIMPLEBUTTON", "", bar, "", 0)
BlzFrameSetAllPoints(sButton, bar)
BlzFrameSetTooltip(sButton, yourTooltip)
BlzFrameSetVisible(yourTooltip, false)

#### Coldgamer

#### Coldgamer

 Level 4

JoinedJun 4, 2019Messages37

Ah, the issue with the missing mouse control explains everything. Thanks a lot for the tip!

I’ve now created an invisible button on the bar and attached the tooltip to it. Works perfectly!

#### GrapesOfWath

#### GrapesOfWath

Sound Reviewer

 Level 20

JoinedMar 31, 2012Messages309

Resources: 29

#### Resources
Models 5Skins 1Tools 2Maps 4Sounds 17

very helpful tutorial, thanks for all that you do

#### KenArok

#### KenArok

 Level 12

JoinedSep 27, 2016Messages175

Resources: 4

#### Resources
Models 2Maps 1Reforged HD Models 1

how to update mybarextext without potentially got desynced? i really hate to use blzframesettext inside the GetLocalPlayer.. i usually set a global variable for the framehandle, but it doesn't seem to work here, always got crashed out from the game when i tested it.. i didnt test to store the framehandle to hashtable yet..


 Last edited: Oct 2, 2025

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

to avoid desyncs you reserve handleIds for all frames (that you touch in trigger), after you created it, in jass there is the theory that strings can desync to, when one believes it, then is safer to make the strings outside of localplayer block.

 JASS:

call BlzCreateSimpleFrame("MyBar", BlzGetFrameByName("ConsoleUI", 0), 0)
call BlzGetFrameByName("MyBarText",0)
call BlzGetFrameByName("MyBarBackground",0)

function update takes nothing returns nothing
 local string newText = ""
 local string wanaText = GetUnitLifePercent(udg_Unit)
 if GetLocalPlayer() == Player(0) then
 set newText = wanaText
 else
 endif
 call BlzFrameSetText(BlzGetFrameByName("MyBarText",0), newText)
endfunction

#### KenArok

#### KenArok

 Level 12

JoinedSep 27, 2016Messages175

Resources: 4

#### Resources
Models 2Maps 1Reforged HD Models 1

Tasyen said:

 to avoid desyncs you reserve handleIds for all frames (that you touch in trigger), after you created it, in jass there is the theory that strings can desync to, when one believes it, then is safer to make the strings outside of localplayer block.

 JASS:

call BlzCreateSimpleFrame("MyBar", BlzGetFrameByName("ConsoleUI", 0), 0)
call BlzGetFrameByName("MyBarText",0)
call BlzGetFrameByName("MyBarBackground",0)

function update takes nothing returns nothing
 local string newText = ""
 local string wanaText = GetUnitLifePercent(udg_Unit)
 if GetLocalPlayer() == Player(0) then
 set newText = wanaText
 else
 endif
 call BlzFrameSetText(BlzGetFrameByName("MyBarText",0), newText)
endfunction

Click to expand...

strings can desync, everyone must believe that.. besides, your solution is similar to displaying special effects for certain players too, that makes perfect sense.. thanks..

wait.. it may cause other player's text changed to "empty string" .. since BlzGetFrameByName("MyBarText",0) is global..

TESTED .. not desynced..

 JASS:

function UpdateBars takes nothing returns nothing
 local unit u = GetTriggerUnit()
 local integer pdex = GetConvertedPlayerId(GetTriggerPlayer())
 local real hp = GetUnitLifePercent(u)
 local real mp = GetUnitManaPercent(u)
 call BlzFrameSetValue(BlzGetFrameByName("MyBarEx",pdex*10+1), hp)
 call BlzFrameSetValue(BlzGetFrameByName("MyBarEx",pdex*10+2), mp)
 call BlzFrameSetText(BlzGetFrameByName("MyBarExText",pdex*10+1), R2S(hp))
 call BlzFrameSetText(BlzGetFrameByName("MyBarExText",pdex*10+2), R2S(mp))
endfunction

function CreateMyBar takes integer pdex returns nothing
 local framehandle hp = BlzCreateSimpleFrame("MyBarEx", BlzGetFrameByName("ConsoleUIBackdrop", 0), pdex*10+1)
 local framehandle mp = BlzCreateSimpleFrame("MyBarEx", BlzGetFrameByName("ConsoleUIBackdrop", 0), pdex*10+2)

 call BlzFrameSetAbsPoint(hp, FRAMEPOINT_TOPLEFT, -0.1, 0.3)
 call BlzFrameSetPoint(mp, FRAMEPOINT_TOP, hp, FRAMEPOINT_BOTTOM, 0.0, 0.0)
 call BlzFrameSetVisible(hp, ConvertedPlayer(pdex) == GetLocalPlayer())
 call BlzFrameSetVisible(mp, ConvertedPlayer(pdex) == GetLocalPlayer())

 call BlzFrameSetTexture(hp, "Replaceabletextures\\Teamcolor\\Teamcolor00.blp", 0, true)
 call BlzFrameSetTexture(mp, "Replaceabletextures\\Teamcolor\\Teamcolor01.blp", 0, true)

 call DisplayTimedTextToPlayer(GetLocalPlayer(),0,0, 99999, "Select an unit to update the Bars")
endfunction

//===========================================================================
function InitTrig_MyBar takes nothing returns nothing
 local trigger trig = CreateTrigger()
 local integer i = 1
 loop
 exitwhen i > 23
 call CreateMyBar(i)
 set i = i + 1
 endloop
 call BlzLoadTOCFile("war3mapimported\\mybar.toc")
 call TriggerRegisterPlayerSelectionEventBJ(trig, Player(0), true )
 call TriggerRegisterPlayerSelectionEventBJ(trig, Player(1), true )
 call TriggerAddAction(trig, function UpdateBars)
endfunction


 Last edited: Oct 2, 2025

 You must log in or register to reply here.


### Similar threads

 UI: Creating a Cam control


Tasyen

Jun 1, 2019

Scripting Tutorials

Replies24Views10K

Mar 4, 2021
ESTO

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

Locked

 How do I make this MUI?


cvbbest

Apr 9, 2022

World Editor Help Zone

Replies2Views2K

Apr 13, 2022
cvbbest

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