---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-create-a-textbutton.315897/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Create a Textbutton
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-create-a-textbutton.315897/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

##
 Introduction

Warcraft 3 V1.31 provided 3 natives to create Frames.

 JASS:

native BlzCreateFrame takes string name, framehandle owner, integer priority, integer createContext returns framehandle
native BlzCreateSimpleFrame takes string name, framehandle owner, integer createContext returns framehandle
native BlzCreateFrameByType takes string typeName, string name, framehandle owner, string inherits, integer createContext returns framehandle

###
 BlzCreateFrame

BlzCreateFrame creates all non SimpleFrames defined in a fdf. Most of this frames allow attaching Frameevents and they can do alot of things. Most times you should use this one.

Lets checkout the arguments

string namethis is the name of the frame you want to create, you have to take it from a fdf. Its the 2.word in "" after Frame

framehandle ownerthe owner becoming the new frames parent

integer priorityNo Idea, should not be negative

createContextwhich index in the frame storage the new Frame and its child will take. The frame storage is accessed with BlzGetFrameByName. Can be any integer.

###
 BlzCreateSimpleFrame

BlzCreateSimpleFrame is used to create simpleFrames defined in a fdf. SimpleFrames have "SIMPLE" in their type name. Most Simpleframes can not have frameevents registered onto them. Creating them is not much different from creating normal Frames.

Arguments are like in BlzCreateFrame but it misses priority​

###
 BlzCreateFrameByType

BlzCreateFrameByType creates a Frame with a new wanted name. One has to define which Type and from which frame you take data over. The inheriting works for frames beeing createable with BlzCreate(simple)Frame. BlzCreateFrameByType is the best when you want to create an empty frame of type x or want to have frame y (from a fdf) with a different name.

Example:

Creates a button that behaves like "ScriptDialogButton" but will be named "MyButton".









 JASS:

call BlzCreateFrameByType("GLUETEXTBUTTON", "MyButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), "ScriptDialogButton", 0)

Looks kinda like a frameHead, if one would write INHERITS instead of the parentFrame.

It can be difficult to access the childrens of such created frames.​

One can only create MainFrames. I call frames MainFrames, if they have a FrameHead outside of any other FrameBody.

(Simple)Frames have to be loaded by tocs (fdf) to be createable.​

##
 Creating a Text-Button

In this example we create a clickable button which looks like the buttons ingame when one presses menu. We use for the button this framedefinition. The frame we want to create is loaded in on default.

Its name is "ScriptDialogButton", it takes over data from "EscMenuButtonTemplate".

 Code:

Frame "GLUETEXTBUTTON" "ScriptDialogButton" INHERITS WITHCHILDREN "EscMenuButtonTemplate" {
 UseActiveContext,
 ButtonText "ScriptDialogButtonText",
 Frame "TEXT" "ScriptDialogButtonText" INHERITS "EscMenuButtonTextTemplate" {
 Text "",
 }
}

Lets start the creation.

We create a frame based on the name "ScriptDialogButton", say ORIGIN_FRAME_GAME_UI is its parent and it uses createcontext 0.

Also we set the size and the position of that new created frame, beeing a button.

 JASS:

function CreateButton1 takes nothing returns nothing
 local framehandle mainButton = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0), 0,0)
 call BlzFrameSetSize(mainButton, 0.12, 0.05)
 call BlzFrameSetAbsPoint(mainButton, FRAMEPOINT_CENTER, 0.3,0.3)
endfunction

Here it is. No Text, has Textures hoverable and clickable.

Lets give that button a text. We access the child-Frame managing the ButtonText "ScriptDialogButtonText".

 JASS:

function CreateButton2 takes nothing returns nothing
 local framehandle mainButton = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI,0), 0,0)
 local framehandle buttonText = BlzGetFrameByName("ScriptDialogButtonText",0) //Get The buttons textChild
 call BlzFrameSetSize(mainButton, 0.12, 0.05)
 call BlzFrameSetAbsPoint(mainButton, FRAMEPOINT_CENTER, 0.3,0.3)
 call BlzFrameSetText(buttonText, "My ButtonText")
endfunction

Better with the text on the button. But we want that some code runs when the button is clicked.

 JASS:

function ButtonCallBack takes nothing returns nothing
 call BJDebugMsg(GetPlayerName(GetTriggerPlayer()) + " pressed: "+ BlzFrameGetText(BlzGetTriggerFrame()))
endfunction

function CreateButton3 takes nothing returns nothing
 local trigger trig = CreateTrigger() //The Trigger Handling the Frameevent
 local framehandle mainButton = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI,0), 0,0)
 call BlzFrameSetSize(mainButton, 0.12, 0.05)
 call BlzFrameSetAbsPoint(mainButton, FRAMEPOINT_CENTER, 0.3,0.3)
 call BlzFrameSetText(mainButton, "My ButtonText") //Cause of a line in scriptDialogButton 'ButtonText "ScriptDialogButtonText",' one can directly read/write text of the button.

 call BlzTriggerRegisterFrameEvent(trig, mainButton, FRAMEEVENT_CONTROL_CLICK)
 call TriggerAddAction(trig, function ButtonCallBack) //Function ButtonCallBack will run when mainButton is clicked
endfunction

As one might see in this piece of code one does not access the Textchild still it works, the reason for that is a line in the frame definition 'ButtonText "ScriptDialogButtonText",'. It tells the button uses this Text Frame to manages its text, so that we can read/write the text directly over the mainButton. Simelar lines exist in other cases too, they make the usage between child and parent stronger and more dependent.

This is "EscMenuButtonTemplate", "ScriptDialogButton" INHERITS from it, taking over much data.

 Code:

Frame "GLUETEXTBUTTON" "EscMenuButtonTemplate" {
 Width 0.228,
 Height 0.035,
 ControlStyle "AUTOTRACK|HIGHLIGHTONMOUSEOVER",
 ButtonPushedTextOffset 0.002f -0.002f,

 ControlBackdrop "ButtonBackdropTemplate",
 Frame "BACKDROP" "ButtonBackdropTemplate" INHERITS "EscMenuButtonBackdropTemplate" {
 }

 ControlPushedBackdrop "ButtonPushedBackdropTemplate",
 Frame "BACKDROP" "ButtonPushedBackdropTemplate" INHERITS "EscMenuButtonPushedBackdropTemplate" {
 }

 ControlDisabledBackdrop "ButtonDisabledBackdropTemplate",
 Frame "BACKDROP" "ButtonDisabledBackdropTemplate" INHERITS "EscMenuButtonDisabledBackdropTemplate" {
 }

 ControlDisabledPushedBackdrop "ButtonDisabledPushedBackdropTemplate",
 Frame "BACKDROP" "ButtonDisabledPushedBackdropTemplate" INHERITS "EscMenuButtonDisabledPushedBackdropTemplate" {
 }

 ControlMouseOverHighlight "ButtonMouseOverHighlightTemplate",
 Frame "HIGHLIGHT" "ButtonMouseOverHighlightTemplate" INHERITS "EscMenuButtonMouseOverHighlightTemplate" {
 }
}

##
 Keyboard Focus

TextButtons have the habit to take control of the keyboard focus as soon they are pressed. When such a button got the keyboard focus hotkeys won't work anymore and pressing return/space will do another control-click. The user can give the focus back to the game by left clicking on the playable world.

As mapper you also have the power to shift the focus. CanFight found a nice, easy way to solve the keyboard focus problem: inside the buttons pressed callback one disables and enables the button, then it counts as clicked and the button loses focus.

 JASS:

call BlzFrameSetEnable(BlzGetTriggerFrame(), false) //disable the clicked button
call BlzFrameSetEnable(BlzGetTriggerFrame(), true) //enable it again.

Edit: Attached the map with the demo code.

Edit: Added Keyboard Focus.​

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

MyButton.w3x

 17.3 KB

 · Views: 675




 Last edited by a moderator: Oct 6, 2020

#### Wareditor

#### Wareditor

 Level 15

JoinedJan 16, 2009Messages764

Resources: 4

#### Resources
Maps 4

Nice to see tutorials poping up about the new UI natives. Good job!

#### Rahko

#### Rahko

 Level 9

JoinedJul 30, 2018Messages445

Resources: 2

#### Resources
Spells and Systems 2

Could you possibly post an example map for study for someone who doesn't know JASS that well? Looking at the code, it doesn't seem too complicated, but I'm not very familiar with JASS and how it should be implemented in a map. Would be very, very grateful.

Actually, never mind. I got it working!

Edit: Further questions: is there a way to temporarily hide or disable the button? And can this be done to a certain player only? (Does this button appear to all players?)

Edit2: I see there are natives for it: BlzFrameSetEnable and BlzFrameSetVisible, but how do I transfer the framehandle to the ButtonCallBack function?

Edit3: Sorry for all the mess, but I found this approach to work. Any thoughts? Is this viable or is there a better way to do this?

 JASS:

library TurnButton
globals
 private framehandle turnButton
endglobals

function ButtonClick takes nothing returns nothing
 if (udg_TurnCurrentIs == 1 and GetTriggerPlayer() == Player(0)) or (udg_TurnCurrentIs == 2 and GetTriggerPlayer() == Player(1)) then
 call TriggerExecute( gg_trg_Turn_Start )
 call BlzFrameSetEnable(turnButton, false)
 else
 call DisplayTextToPlayer(GetTriggerPlayer(), 0, 0, "It's not your turn.")
 endif
endfunction

function ButtonReEnable takes nothing returns nothing
 call BlzFrameSetEnable(turnButton, true)
endfunction

function CreateEndTurnButton takes nothing returns nothing
 local trigger t = CreateTrigger()
 set turnButton = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI,0), 0,0)
 call BlzFrameSetSize(turnButton, 0.11, 0.04)
 call BlzFrameSetAbsPoint(turnButton, FRAMEPOINT_CENTER, 0.26,0.164)
 call BlzFrameSetText(turnButton, "End Turn")
 call BlzTriggerRegisterFrameEvent(t, turnButton, FRAMEEVENT_CONTROL_CLICK)
 call TriggerAddAction(t, function ButtonClick)
endfunction
endlibrary


 Last edited: May 30, 2019

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Sabe said:

 Edit: Further questions: is there a way to temporarily hide or disable the button? And can this be done to a certain player only? (Does this button appear to all players?)


Click to expand...

A frame uses the default handlepool, hence it should be created for all players. A Frameevent is a synced event, if a player presses the button and evokes the event, it will be sent to/happen for all players with TriggerPlayer beeing the player pressing the button.

The frame state is not synced; you can change the text, value, position, visibility or usability (SetEnable) localy without having to fear any direct desync. It still can desync, if you changed something localy and use that changed thing for something game relevant. But that is quite unlikely to happen for a button.

#### Coldgamer

#### Coldgamer

 Level 4

JoinedJun 4, 2019Messages37

First, I want to say thank you for your awesome Tutorials!

I have question about the Button: Is it possible to detect which player has pressed the button?

I want that a player can open a menu over the button for example but I want to recognize which player has pressed the button,

so that the menu only appears for the Player, who has pressed the button.

I hopy my question is clear, my english is not that good

Thanks and regards

#### Elprede

#### Elprede

 Level 21

JoinedOct 17, 2012Messages954

Resources: 9

#### Resources
Icons 1Tools 2Spells and Systems 6

##
 The Sundering of Worlds


Refer to the player pressing the button as GetTriggerPlayer().

#### Rahko

#### Rahko

 Level 9

JoinedJul 30, 2018Messages445

Resources: 2

#### Resources
Spells and Systems 2

Hey!

I'd like to mention, that the Blizzard Dialog Buttons actually use the same "ScriptDialogButton" and thus take into use the context integers. set turnButton = BlzCreateFrame("ScriptDialogButton", BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI,0), 0,0) the last 0 on this, for example. So you could mention this in case people want to make more custom buttons and use the dialog windows at the same time.

I'm not sure by which logic the integers for the dialog buttons are chosen. I have 13 custom buttons in total and they are all created at map initialization. At elapsed time 1.00 I create the dialog, but its buttons still take integers 1 and 2.

Edit: Yep, the Blizzard Dialog Buttons will automatically take up the integers from 1 on (i.e. first created always takes integer 1, second one takes integer 2, third one takes integer 3 and so on, even if there was already a custom button with the said integer). Doesn't matter if they were made later than the custom buttons.


 Last edited: Jun 18, 2019

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Yes, that can happen when one uses precreated frames. Although the custom created frames would not be lost when the game takes over the createcontext. One loses the BlzGetFrameByName access.

If one wants to be on the safe side one should create new Frames with new unique names.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Much like the other ones, solid and extremely useful.

Approved.

#### GIMLI_2

#### GIMLI_2

 Level 18

JoinedMar 21, 2011Messages1,622

Resources: 3

#### Resources
Maps 2Spells and Systems 1

Is it possible to create a GLUEBUTTON with text? If i add a child text frame to the GLUEBUTTON, it is not klickable on the text part. Is it possible to fix this or do i have to use GLUETEXTBUTTON?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

you can use call BlzFrameSetEnable(textFrame, false) to disable the clickability of the TEXT-Frame.

Or create a textFrame with the fdf acition LayerStyle "IGNORETRACKEVENTS",

Or that GLUETEXTBUTTON

#### GIMLI_2

#### GIMLI_2

 Level 18

JoinedMar 21, 2011Messages1,622

Resources: 3

#### Resources
Maps 2Spells and Systems 1

Ok thanks! A bit offtopic, is there a way to set the alpha of a parent frame without affecting child frames?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

no directly. But after having changed the alpha of the parent you could set the alpha of all children.

 You must log in or register to reply here.


### Similar threads

 UI: GLUEBUTTON


Tasyen

Sep 9, 2019

Scripting Tutorials

Replies32Views15K

Nov 16, 2023
Tasyen

 UI: Frames and Tooltips


Tasyen

Jun 27, 2019

Scripting Tutorials

Replies45Views21K

Oct 27, 2024
RPG_F

 UI: Creating a Bar


Tasyen

Jun 12, 2019

Scripting Tutorials

Replies38Views18K

Oct 2, 2025
KenArok

 UI: Reading a FDF


Tasyen

May 29, 2019

Scripting Tutorials

Replies15Views16K

Dec 1, 2020
Tasyen

 UI: Creating a Cam control


Tasyen

Jun 1, 2019

Scripting Tutorials

Replies24Views10K

Mar 4, 2021
ESTO

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