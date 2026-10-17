---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-editbox-text-input.316239/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Editbox Text Input
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-editbox-text-input.316239/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

##
 Introduction

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

Outside of this events one would need BlzFrameGetText to get the text, but BlzFrameGetText returns for each player the text he currently has in his editbox -> is not synced in multiplayer. Therefore one has to sync it using the frameevents.​

##
 Example

Thats our Lua code for the demo. It Loads the custom tocFile, creates a frame of name "EscMenuEditBoxTemplate" and registeres 2 events to that frame. Also when the local player has its keyboard focus on the editbox and presses enter/return, its current insert text will be shown in the message frame and that message is saved in the gui variable udg_UserInput[playerIndex].

 Lua:

function LoadToc()
 BlzLoadTOCFile("war3mapimported\\so.toc")
end

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

function CreateBox()
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

This Trigger executes the loading and creation functions.

Init
Events
Map initialization

Conditions

Actions
Custom script: LoadToc()

Custom script: CreateBox()

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

Inside FDF one can add an EditTextFrame to an Editbox to change the font of the input text.

 Code:

EditTextFrame "SaveGameFileEditBoxText",
Frame "TEXT" "SaveGameFileEditBoxText" INHERITS "EscMenuEditBoxTextTemplate" {
}

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

Editbox.jpg

 3.8 KB

 · Views: 9,266



Editbox Pressed Enter.jpg

 20.2 KB

 · Views: 9,186



Editbox (2).w3x

 17.3 KB

 · Views: 395




 Last edited by a moderator: Oct 4, 2020

#### _Guhun_

#### _Guhun_

 Level 12

JoinedJun 12, 2010Messages413

Resources: 17

#### Resources
Models 1Maps 1Spells and Systems 9JASS 4Tutorials 2

Thanks! This is exactly what I needed to know about player input through frames.

#### KeepVary

#### KeepVary

 Level 3

JoinedMay 12, 2019Messages12

Tasyen said:

 Even when using BlzFrameGetText the text length doesn't seem to exceed 1082.


Click to expand...

I've put in over a million chars to the EditBox, but BlzFrameGetText still can get all of them (although gamerun become extremely slow).

It didn't seem has a limit.

Here's the code I used to test:

 Lua:

function FrameTest()
 print("toc loaded:", BlzLoadTOCFile("war3mapImported/CustomToc.toc"))
 -- Needs a custom toc that include "UI\FrameDef\Glue\BattleNetTemplates.fdf"

 local null = BlzGetFrameByName("", 0)
 local gui = BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0)
 local point = {
 c = FRAMEPOINT_CENTER
 }
 local eventList = {
 editbox_text = FRAMEEVENT_EDITBOX_TEXT_CHANGED,
 editbox_enter = FRAMEEVENT_EDITBOX_ENTER
 }

 local function createTemplate(template, x, y, w, h, parent, id)
 x = x or 0; y = y or 0; parent = parent or gui; id = id or 0
 local frame = BlzCreateFrame(template, parent, 0, id)
 if frame ~= null then
 BlzFrameSetAbsPoint(frame, point.c, x, y)
 if w and h then
 BlzFrameSetSize(frame, w, h)
 end
 else
 print('Failed on create "' .. template .. '"')
 end
 return frame
 end

 local function addEvent(frame, event, func)
 local trg = CreateTrigger()
 BlzTriggerRegisterFrameEvent(trg, frame, eventList[event])
 TriggerAddAction(trg, func)
 end

 local function editboxTest()
 local ebox = createTemplate("BattleNetEditBoxTemplate", 0.3, 0.5, 0.2, 0.04)
 local tarea = createTemplate("BattleNetTextAreaTemplate", 0.3, 0.3, 0.2, 0.3)
 addEvent(ebox, "editbox_text", function()
 ClearTextMessages()
 print("EditBox length:", #BlzFrameGetText(BlzGetTriggerFrame()))
 end)
 -- When press enter/return in the EditBox, a callback will get it's text and set into the TextArea.
 addEvent(ebox, "editbox_enter", function()
 local txt = BlzFrameGetText(BlzGetTriggerFrame())
 BlzFrameSetText(tarea, txt)
 print("TextArea length:", #txt)
 end)
 local testText = ""
 for i = 1, 5000 do -- This will make a string that has nearly 120000 chars, then set it into the EditBox.
 local line = "this is test text " .. i
 testText = testText .. line .. "|n"
 end
 BlzFrameSetText(ebox, testText)
 end
 editboxTest()
end

Btw, if text in the EditBox got a length that over 4096, they'll become invisible.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

KeepVary said:

 I've put in over a million chars to the EditBox, but

BlzFrameGetText

still can get all of them (although gamerun become extremely slow).

It didn't seem has a limit.


Click to expand...

Intressting. I hardcoded that text with in one line in world editor. Might also be that 1082 is the allowed amount of text per BlzFrameSetText.

#### KeepVary

#### KeepVary

 Level 3

JoinedMay 12, 2019Messages12

Tasyen said:

 Intressting. I hardcoded that text with in one line in world editor. Might also be that 1082 is the allowed amount of text per BlzFrameSetText.


Click to expand...

I tested the code (in my post above) in Jass version, found that a string variable in Jass got a 4096 length limit.

Then I tried hardcoding an one line string as you do, found that if the string's length exceeded 1100, jasshelper will show a syntax error that said "String literal size limit exceeded /unclosed string", else when less than or equal to 1100, the string will limited in 1023 chars.

I guess this is why you found a number of 1082, and perhaps it's because of the difference in charset (WE language) that I got a 1023.

I've also tested hardcoding in Lua version, it doesn't have the problem.

Anyway, there's nothing about those natives, it's a Jass problem with WE.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

@KeepVary Thanks for those tests, that is really useful to know. The ~1024 char limit is definitely a JASS issue (and iirc it can get close to 2048 with concatenation), it's really cool that Lua doesn't have that constraint!

@Tasyen Do you mind updating the first post with that information? According to post #3, it seems there is no practical limit to how much text you can put/retrieve from a text input frame, but beyond 4096 characters, it becomes invisible. (maybe it is worth setting 4096 as the limit then?) After that it should be good for approval.

P.S. Great tutorial btw, I really like that you included that tip about retrieving frame text in a synchronous manner.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

I made a practical misstake when recommenting to create the EditBox Templates. I mean it is not wrong but their TextStyling is bad. It is better to do it like shown in "SaveGameFileEditBox" from "escmenusavegamepanel.fdf". One inherits one of the templates, defines a functional child Frame: EditTextFrame is managing the Text style.

 Code:

IncludeFile "UI\FrameDef\UI\EscMenuTemplates.fdf",

Frame "EDITBOX" "YourEditBox" INHERITS WITHCHILDREN "EscMenuEditBoxTemplate" {
 Width 0.362,
 Height 0.037,

 EditTextFrame "YourEditBoxText",
 Frame "TEXT" "YourEditBoxText" INHERITS "EscMenuEditBoxTextTemplate" {
 }
}

Edit: The cursor has an own FrameFont, which has to be setuped inside the EDITBOX directly, it is not taken from EditTextFrame.


 Last edited: Jan 27, 2020

#### Rahko

#### Rahko

 Level 9

JoinedJul 30, 2018Messages445

Resources: 2

#### Resources
Spells and Systems 2

Here, I made a JASS version in case someone has no idea how to work with code.

 JASS:

function LoadToc takes nothing returns nothing
 call BlzLoadTOCFile("war3mapimported\\so.toc")
endfunction

function EditBoxEnter takes nothing returns nothing
 call DisplayTextToForce(GetPlayersAll(), "EditBoxEnter:")
 call DisplayTextToForce(GetPlayersAll(), BlzGetTriggerFrameText())
 call DisplayTextToForce(GetPlayersAll(), GetPlayerName(GetTriggerPlayer()))
 set udg_UserInput[GetConvertedPlayerId(GetTriggerPlayer())] = BlzGetTriggerFrameText() // save the text of the local player in a synced manner.
endfunction

function TEXT_CHANGED takes nothing returns nothing
endfunction

function CreateBox takes nothing returns nothing
 local framehandle editbox = BlzCreateFrame("EscMenuEditBoxTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0),0,0) // create the box
 local trigger eventHandler

 call BlzFrameSetAbsPoint(editbox, FRAMEPOINT_CENTER, 0.4, 0.3) // Position of the box
 call BlzFrameSetSize(editbox, 0.2, 0.03) // Boxes size

 set eventHandler = CreateTrigger() // Create the FRAMEEVENT_EDITBOX_ENTER trigger
 call TriggerAddAction(eventHandler, function EditBoxEnter)
 call BlzTriggerRegisterFrameEvent(eventHandler, editbox, FRAMEEVENT_EDITBOX_ENTER)

 set eventHandler = CreateTrigger() // Create the FRAMEEVENT_EDITBOX_TEXT_CHANGED trigger
 call TriggerAddAction(eventHandler, function TEXT_CHANGED)
 call BlzTriggerRegisterFrameEvent(eventHandler, editbox, FRAMEEVENT_EDITBOX_TEXT_CHANGED)
endfunction

#### Attachments

Editbox JASS.w3x

 18.2 KB

 · Views: 228



#### Ice

#### Ice

 Level 10

JoinedSep 4, 2004Messages153

Resources: 24

#### Resources
Models 5Icons 16Skins 3

how would one remove the textbox after hitting enter and or atleast clear the entered text from box?

#### Rahko

#### Rahko

 Level 9

JoinedJul 30, 2018Messages445

Resources: 2

#### Resources
Spells and Systems 2

I made you an example already. Check out the private conversation.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Ice said:

 how would one remove the textbox after hitting enter and or atleast clear the entered text from box?


Click to expand...

One sets the Frames text only for the Triggering Player to "", that is done inside the FRAMEEVENT_EDITBOX_ENTER Action.

 Lua:

if GetLocalPlayer() == GetTriggerPlayer() then
 BlzFrameSetText(BlzGetTriggerFrame(), "")
end

That will also trigger a FRAMEEVENT_EDITBOX_TEXT_CHANGED with that player as Triggering Player.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Took the liberty to make some changes as the author happens to have a bunch of tutorials I figure it would be a pain to update all of them.

Changes made:

1. Changed one title to h2 from h3 to match the other titles

2. Changed the title color to orange (from default) to match the last title

3. Applied an indent after each title

4. Swapped the image links to direct img tags since they are small enough to be viewed in full size directly

5. Added introduction title at the top

Sending this to the JASS/AI section as that is where the other tutorials in the series are even if I would possibly argue that this is not a "jass" heavy tutorial.

If @Tasyen wants it in another section, poke me. And preferably mark which sub-forum you are "applying" for by setting a prefix to the thread.

#### speige

#### speige

 Level 4

JoinedApr 2, 2023Messages12

@Tasyen Have you found a way to do a multi-line edit box? Basically a mix between the read-only TextArea and the single-line EditBox? Maybe with another control or with a custom TOC+FDF?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

speige said:

@Tasyen Have you found a way to do a multi-line edit box? Basically a mix between the read-only TextArea and the single-line EditBox? Maybe with another control or with a custom TOC+FDF?


Click to expand...

I don't know of such, never tried :| (and I only know a part of it).

But EDITBOX can contain huge amounts of text (async only, the synced event text is limited to ~250 bytes). support ctrl + c and ctrl + v, shift+ pos1 & shift + end keys. Which makes it good for coping in text from a proper text editor or taking it out of warcraft 3 (in single player).

Huge Text also can lag a bit.

#### speige

#### speige

 Level 4

JoinedApr 2, 2023Messages12

@Tasyen I wasn't able to find anything, so I coded my own

###
 GitHub - speige/WC3TextEditor


Contribute to speige/WC3TextEditor development by creating an account on GitHub.

 github.com


Hopefully it'll be helpful for someone else as well

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

One could combine multiple Frames to mimic a multiline editbox: one has a editbox with 0 alpha ("hidden" but still working) and constantly displays the editbox onto a TEXT. But that way one would lose the cursor and scrolling would have to be handled.

Well An Lua example; requires escmenutemplates.fdf Loaded in

 Lua:

local Box = BlzCreateFrame("EscMenuEditBoxBackdropTemplate", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0, 0)
BlzFrameSetSize(Box, 0.2, 0.2)
BlzFrameSetAbsPoint(Box, FRAMEPOINT_TOPRIGHT, 0.78, 0.4)
local EditBox = BlzCreateFrameByType("EDITBOX", "EditBox2", Box, "", 0)
BlzFrameSetAlpha(EditBox, 0)
local TextFrame = BlzCreateFrame("EscMenuEditBoxTextTemplate", Box, 0, 0)
BlzFrameSetPoint(TextFrame, FRAMEPOINT_TOPLEFT, Box, FRAMEPOINT_TOPLEFT, 0.008, -0.008)
BlzFrameSetPoint(TextFrame, FRAMEPOINT_BOTTOMRIGHT, Box, FRAMEPOINT_BOTTOMRIGHT, -0.008, 0.008)
BlzFrameSetAllPoints(EditBox, Box)
BlzFrameSetEnable(TextFrame, false)
TimerStart(CreateTimer(), 0.05, true, function()
 BlzFrameSetText(TextFrame, BlzFrameGetText(EditBox))
end)

 You must log in or register to reply here.


### Similar threads

 UI: Change Lumber Text


Tasyen

May 29, 2019

Scripting Tutorials

Replies17Views9K

Apr 1, 2021
pr114

 UI: FrameEvents and FrameTypes


Tasyen

Aug 27, 2019

Scripting Tutorials

Replies16Views12K

Dec 7, 2024
Gaspar

 UI: TEXTAREA the scrolling Text Frame


Tasyen

Sep 20, 2019

Scripting Tutorials

Replies20Views9K

Dec 6, 2021
Seago

S

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