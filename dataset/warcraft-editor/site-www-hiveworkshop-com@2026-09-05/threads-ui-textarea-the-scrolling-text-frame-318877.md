---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-textarea-the-scrolling-text-frame.318877/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Textarea the Scrolling Text Frame
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-textarea-the-scrolling-text-frame.318877/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

##
 Introduction

TEXTAREA is a frame to display scrollable text. Normaly TextAreas have a BACKDROP showing a box and a Background.

TextAreas require a scrollbar. The scrollbar is at the right side of the TextArea and blocks a part of the space given to the TextArea. If the TextArea is big enough to display the whole text, the scrollbar is not shown. Even when the scrollbar is not shown it takes its space. For not scrollable Text one could simply use a TEXT-Frame.​

##
 Built in TextArea

Warcraft 3 has 2 builtIn TEXTAREA mainframes.

BattleNetTextAreaTemplate

EscMenuTextAreaTemplate

They are a good base to inherit, copy or one can just create them. If one wants to create one of them one would have to load them first using BlzLoadTOCFile.

The fdfs containing them are located at:

UI\Frames\framedef\glue\battlenettemplates.fdf

UI\Frames\framedef\ui\escmenutemplates.fdf​

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

TextArea can display colored Text using warcraft colorformat |cffff0000.​

##
 Screen-Space and Scrolling

On Default a TextArea will catch the screen-space given to it, making it unclickable for the playable world and sending mousewhell rolls to its scrollbar. One could disable the TextArea to stop the TextArea from taking the mouse Input which would make the screenspace useable by the playable game again, but that would also disable the mouse whell roll sending. One could still scroll when the mouse points directly to the scrollbar.

Disabling is done with:

call BlzFrameSetEnable(textArea, false)

​

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

QuestBackdropEscMenuBackdrop

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

BlzGetFrameByName is not able to return inherited childFrames. With the release of the native BlzFrameGetChild in V1.32.6, one is able to get the inherited childFrames during the runtime, in 1.32.6 this native ignores String&Texture childFrames.​

##
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

TextArea Tutorial.w3x

 17.8 KB

 · Views: 451




 Last edited by a moderator: Oct 5, 2020

#### lolreported

#### lolreported

 Level 20

JoinedAug 16, 2007Messages885

Resources: 25

#### Resources
Tools 1Maps 9Spells and Systems 15

Thanks for all the tutorials and script templates, very helpful!

You wouldn't happen to know how to manually set the current scroll value? Whenever I update the text in the text area the scroll wheel always starts at the bottom.

I've tried experimenting with these natives, but with with no success:

native BlzFrameSetValue takes framehandle frame, real value returns nothing

native BlzFrameSetStepSize takes framehandle frame, real stepSize returns nothing

I'm using "HeroSelectorTextArea" from HeroSelector.fdf in your Hero Selection Template if that matter.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

lolreported said:

 You wouldn't happen to know how to manually set the current scroll value? Whenever I update the text in the text area the scroll wheel always starts at the bottom.


Click to expand...

No, I also want to know how to do that.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Similar structure changes to the other ones.

Approved.

edit: I hope I put the right text to the right image in the table, would be nice for you to confirm because I am not sure about the visual difference.

#### Pwnica

#### Pwnica

 Level 12

JoinedMar 13, 2020Messages421

Resources: 3

#### Resources
Reforged HD Models 2Tutorials 1

Can this works for each player individually or it’s for all player one text area visible

#### Planetary

#### Planetary

 Level 15

JoinedFeb 7, 2020Messages399

Resources: 3

#### Resources
Maps 2Spells and Systems 1

Pwnica said:

 Can this works for each player individually or it’s for all player one text area visible


Click to expand...

You would need to use GetLocalPlayer() to display/update it for individual players. Frames by default show for everyone.

e.g.

 JASS:

if Player(0) == GetLocalPlayer() then
 // do show stuff for player 1
 // do text update stuff for player 1
endif

#### Pwnica

#### Pwnica

 Level 12

JoinedMar 13, 2020Messages421

Resources: 3

#### Resources
Reforged HD Models 2Tutorials 1

So is it possible and where I set the GetLocalPlayer() before I start to call BlzCreateFrame?

thx i think I got it..

#### Pwnica

#### Pwnica

 Level 12

JoinedMar 13, 2020Messages421

Resources: 3

#### Resources
Reforged HD Models 2Tutorials 1

Planetary said:

 You would need to use GetLocalPlayer() to display/update it for individual players. Frames by default show for everyone.

e.g.

 JASS:

if Player(0) == GetLocalPlayer() then
 // do show stuff for player 1
 // do text update stuff for player 1
endif

Click to expand...

can this sort of things cause desync? And how I figure it out?

#### Planetary

#### Planetary

 Level 15

JoinedFeb 7, 2020Messages399

Resources: 3

#### Resources
Maps 2Spells and Systems 1

Pwnica said:

 So is it possible and where I set the GetLocalPlayer() before I start to call BlzCreateFrame?


Click to expand...

You don't want to create frames for local player, only hide/show/update them.

I can give you a rudimentary example, but if you don't know about GetLocalPlayer() you can read through something like this: GetLocalPlayer

And if you use it incorrectly on data that requires net code, it can desync.

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
 // example: leave it shown for Player 1 (do nothing), hide for Player 2 and 3:
 if Player(1) == GetLocalPlayer() or Player(2) == GetLocalPlayer() then
 call BlzFrameSetVisible(textArea, false)
 call BlzFrameSetVisible(textArea2, false)
 endif
 // end example
endfunction

//===========================================================================
function InitTrig_Create_TextAreaTemplates takes nothing returns nothing
 call BlzLoadTOCFile( "war3mapImported\\Templates.toc" )
 call TimerStart(CreateTimer(), 0.0, false, function CreateDefaultTextAreas)
endfunction

#### Pwnica

#### Pwnica

 Level 12

JoinedMar 13, 2020Messages421

Resources: 3

#### Resources
Reforged HD Models 2Tutorials 1

Damn, wanted to use this text area for each player to show some stats like

Armor

Health Points

Mana Points

Power

Spelldamage

Spellcrit/Crit Chance %

Spellcrit/Crit %

Dodge Chance %

Block Chance %

Block %

Spell Lifesteal/Lifesteal

Magical Damage Reduction %

Health Regeneration

Mana Regeneration %

Movementspeed

so that’s not an option

#### Planetary

#### Planetary

 Level 15

JoinedFeb 7, 2020Messages399

Resources: 3

#### Resources
Maps 2Spells and Systems 1

Pwnica said:

 Damn, wanted to use this text area for each player to show some stats like

Armor

Health Points

Mana Points

Power

Spelldamage

Spellcrit/Crit Chance %

Spellcrit/Crit %

Dodge Chance %

Block Chance %

Block %

Spell Lifesteal/Lifesteal

Magical Damage Reduction %

Health Regeneration

Mana Regeneration %

Movementspeed

so that’s not an option


Click to expand...

No, that is an option. You can read from data and update the text for the local player, you just cannot set data that requires synchronization (e.g. make a frame locally which creates a framehandle).

#### Pwnica

#### Pwnica

 Level 12

JoinedMar 13, 2020Messages421

Resources: 3

#### Resources
Reforged HD Models 2Tutorials 1

Planetary said:

 No, that is an option. You can read from data and update the text for the local player, you just cannot set data that requires synchronization (e.g. make a frame locally which creates a framehandle).


Click to expand...

but if I use this like a spell that turns it visible for the local player would this sort of things cause a desync?

And where I can find a framehandle tutorial I started jass some days ago and im still a noob it would be my skill level of jass to update the stats when clicking on the spell and showing the stats but never heard of framehandles...

#### Planetary

#### Planetary

 Level 15

JoinedFeb 7, 2020Messages399

Resources: 3

#### Resources
Maps 2Spells and Systems 1

Pwnica said:

 but if I use this like a spell that turns it visible for the local player would this sort of things cause a desync?

And where I can find a framehandle tutorial I started jass some days ago and im still a noob it would be my skill level of jass to update the stats when clicking on the spell and showing the stats but never heard of framehandles...


Click to expand...

It's not really organized anywhere. I'd spend time looking through these, though: ui-fdf | HIVE

#### Pwnica

#### Pwnica

 Level 12

JoinedMar 13, 2020Messages421

Resources: 3

#### Resources
Reforged HD Models 2Tutorials 1

Planetary said:

 It's not really organized anywhere. I'd spend time looking through these, though: ui-fdf | HIVE

Click to expand...

brother I know you are some skilled person in Lua/Jass but you have a conversation with a new noob mapper so what is ui fdf? And what means it’s not really organized?

Sorry for wasting your time but I don’t know this sort of things and want to understand it a bit...

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Pwnica said:

 ui fdf


Click to expand...

UI-fdf is a tag under which most of my tutorials are collected and can be found. This tags are a feature of hiveworkshop.com. Tags used by a thread, can be seen right under the Title name. You can click such a tag to get a List of Threads using the tag.

Pwnica said:

 And where I can find a framehandle tutorial


Click to expand...

At the end of the tutorials you find links to other such tutorials, this is a newer edit. Chaosy told me it would link them better than the tag.

Or you could open the Tutorial Pastebin link in my signatur which opens a big (incomplete) info book about warcraft 3 UI-Frames, mostly a collection of the tutorials.

#### Uncle

#### Uncle

Warcraft Moderator

 Level 76

JoinedAug 10, 2018Messages8,351

Resources: 1

#### Resources
Spells and Systems 1

Is there any way to clear a TextArea?

For now I'm destroying and recreating the framehandle whenever I want to reset it.

#### Pwnica

#### Pwnica

 Level 12

JoinedMar 13, 2020Messages421

Resources: 3

#### Resources
Reforged HD Models 2Tutorials 1

Uncle said:

 Is there any way to clear a TextArea?

For now I'm destroying and recreating the framehandle whenever I want to reset it.


Click to expand...

question can you update it? if yes you can also clear it... using a variable string as text and if you clear the variable before updating again you clear it... if im not wrong.. same i did with custom info panel from tasyen...

#### Uncle

#### Uncle

Warcraft Moderator

 Level 76

JoinedAug 10, 2018Messages8,351

Resources: 1

#### Resources
Spells and Systems 1

I'll have to test it out. I assumed that would leave a blank line in the text area because you aren't removing the text, you're just setting it to an empty string "".

#### Pwnica

#### Pwnica

 Level 12

JoinedMar 13, 2020Messages421

Resources: 3

#### Resources
Reforged HD Models 2Tutorials 1

Uncle said:

 I'll have to test it out. I assumed that would leave a blank line in the text area because you aren't removing the text, you're just setting it to an empty string "".


Click to expand...

Idk if I misunderstood you... just want to give the Helper some help back can I ask what exactly you mean by clearing ?

Remove all text in it?

#### HolyWolf

#### HolyWolf

 Level 3

JoinedMay 31, 2019Messages35

lolreported said:

 Thanks for all the tutorials and script templates, very helpful!

You wouldn't happen to know how to manually set the current scroll value? Whenever I update the text in the text area the scroll wheel always starts at the bottom.

I've tried experimenting with these natives, but with with no success:

native BlzFrameSetValue takes framehandle frame, real value returns nothing

native BlzFrameSetStepSize takes framehandle frame, real stepSize returns nothing

I'm using "HeroSelectorTextArea" from HeroSelector.fdf in your Hero Selection Template if that matter.


Click to expand...

Tasyen said:

 No, I also want to know how to do that.


Click to expand...

Years passed, I wonder if there is a way now.

I don't need to set the scroll value, all I need is to set the textarea at top (instead of bottom) each time I update the text.

Anyone knows how to do it?

#### Seago

S

#### Seago

thank you for your tutorials very much.

 i didn't realize i'm just a beginner until my friend recommened them to me.

 You must log in or register to reply here.


### Similar threads

Locked

 TEXT frame with scrolling


chopinski

Mar 4, 2023

Triggers & Scripts

Replies10Views2K

Jan 27, 2024
Wind_w

 UI: Reading a FDF


Tasyen

May 29, 2019

Scripting Tutorials

Replies15Views16K

Dec 1, 2020
Tasyen

 UI: Change Lumber Text


Tasyen

May 29, 2019

Scripting Tutorials

Replies17Views9K

Apr 1, 2021
pr114

 UI: Create a TextButton


Tasyen

May 30, 2019

Scripting Tutorials

Replies12Views12K

Jan 11, 2021
Tasyen

 UI: EditBox - Text Input


Tasyen

Jun 12, 2019

Scripting Tutorials

Replies15Views10K

Apr 6, 2023
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