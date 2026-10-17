---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/ui-originframes.316034/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# UI Originframes
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/ui-originframes.316034/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Jump to Hiding

Jump to ChildFrames

Originframetypes are built in frames managing the default game ui. ORIGIN_FRAME_GAME_UI is our key to generate new container frames. In other tutorials and in many examples, ORIGIN_FRAME_GAME_UI is used as parent for custom created frames. That is done because every frame needs a parent.

Many OriginFrames are nothing special. One can think of originframes as another way to access this frames.

One can get a framehandle out of such an originframetype with that native. We need framehandles to use it inside the framehandle natives.

 JASS:

native BlzGetOriginFrame takes originframetype frameType, integer index returns framehandle

For the first argument pick one of the constants, for the second one choose in most cases 0. If the origin frame has more than 1 entity, you can get other ones of that originframetype by using a higher number.

Example Frame-GameUI: BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0)

##
 OriginframeType InfoTable

������

ConstantIndexesinfo2img

 ORIGIN_FRAME_GAME_UIwithout it nothing will be displayed.

 ORIGIN_FRAME_WORLD_FRAMEthe visible playground, units, items, effects, fog... every object participating in the game is displayed on it.

 ORIGIN_FRAME_HERO_BARparent of all HERO_BUTTONS, HeroButtons share the same visibility.

 ORIGIN_FRAME_HERO_BUTTON0 to 6the clickable buttons of own/allied heroes on the left of the screen

 ORIGIN_FRAME_HERO_HP_BAR0 to 6connected to HeroButtons

 ORIGIN_FRAME_HERO_MANA_BAR0 to 6connected to HeroButtons

 ORIGIN_FRAME_HERO_BUTTON_INDICATOR0 to 6The glowing when a hero has skillpoints; connected to HeroButtons. They reappear when a hero gains a new skillpoint even when all originframes are hidden.

 ORIGIN_FRAME_ITEM_BUTTON0 to 5Items in the inventory. Reappear/updates every selection, When its parent is visible. In 1.32.x Moving this glitches instead one has to move BlzGetFrameByName("InventoryButton_0", 0)... to ("InventoryButton_5", 0)

 ORIGIN_FRAME_COMMAND_BUTTON0 to 11the buttons to order units around, like ITEM_BUTTON, Reappear/update every selection. In 1.32.x Moving this glitches instead one has to move BlzGetFrameByName("CommandButton_0", 0)... to ("CommandButton_11", 0)

 ORIGIN_FRAME_SYSTEM_BUTTON0 to 3Menu, allies, Log/chat, Quest

 ORIGIN_FRAME_PORTRAITFace of the main selected Unit, uses a different coordination system 0,0 being the absolute bottomLeft which makes it difficult to be used together with other frames (not 4:3, like for the others)

 ORIGIN_FRAME_MINIMAP

 ORIGIN_FRAME_MINIMAP_BUTTON0 to 40 is the Button at top to 4 Button at the bottom

 ORIGIN_FRAME_TOOLTIP

 ORIGIN_FRAME_UBERTOOLTIPHandles the Tooltip window

 ORIGIN_FRAME_CHAT_MSG

 ORIGIN_FRAME_UNIT_MSGprint frame for game display message / DisplayTextToPlayer

 ORIGIN_FRAME_TOP_MSGFrame: UpKeep Change Warning Message, below the dayTime Clock

 ORIGIN_FRAME_PORTRAIT_HP_TEXTV1.32+ Does not exist until any selection was done and a short time passed. Force a Selection and wait a short time. Does only pos itself correctly with

FRAMEPOINT_CENTER. Likely a String-Frame with showing many of the behaviours a String-Frame have (crash one SetAlpha/Level, BlzFrameGetText returns the shown Value, BlzFrameSetFont works...).

 ORIGIN_FRAME_PORTRAIT_MANA_TEXTLike ORIGIN_FRAME_PORTRAIT_HP_TEXT

 ORIGIN_FRAME_UNIT_PANEL_BUFF_BARThe Frame has a size to fit all 8 possible Buffs

 ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR_LABELV1.32+ Attached to ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR on default

 ORIGIN_FRAME_SIMPLE_UI_PARENTV1.32+

##
 Hiding default UI

There exists a native to hide all of them. call BlzHideOriginFrames(true), After one used it the screen will look like that. The ability buttons will appear as soon the player selected a unit.

From Reforged onwards the World Frame covers the full screen on default. But that black background box might disturb one still. It is additional BACKDROP Blizzard added for the Bottom UI but it is not hidden by BlzHideOriginFrames hence one needs an additional line of code that hides:

call BlzFrameSetVisible(BlzGetFrameByName("ConsoleUIBackdrop",0), false)

There are other options, if one wants the Frame logical visible but not seeable: For example one could reduced the ySize.

BlzFrameSetSize(BlzGetFrameByName("ConsoleUIBackdrop",0), 0, 0.0001)

There is a not see able Frame At the default position of the CommandButton that blocks mouse clicking at that spot, this frame's position and size is affected by ConsoleUI hence before Warcraft 3 V1.32.6, it was suggested to do things to "ConsoleUI" to get this spot useable when the UI was altered much anyway. With 1.32.6 one can access that Frame and hide it directly with such an action. BlzFrameSetVisible(BlzFrameGetChild(BlzGetFrameByName("ConsoleUI", 0), 5), false)

###
 Alternative Hide only Bottom UI

There are also some other options to "hide" parts of the UI. One is to move the bottom Of "ConsoleUI" a little bit lower that has to be done at map Init, doing it later leads to crashes.

BlzFrameSetAbsPoint(BlzGetFrameByName("ConsoleUI", 0), FRAMEPOINT_BOTTOM, 0.4, -0.18)

When one plans to move some originframes, it is also advised to use this line of code BlzEnableUIAutoPosition(false). That will stop the reposing of some of this built in frames when the user changes resolution or windowMode.

1.32.6 gave the ability to acces the children of Frames.

BlzFrameGetChildrenCount(frame) : integer

Returns the amount of non String/Texture childFrames.​
BlzFrameGetChild (frame, index) -> frame

Returns the non String/Texture childFrame with that index: should be called with 0 to BlzFrameGetChildrenCount(frame) - 1. Child-Frames take Indexes based on their Level. The child with the lowest Level takes the lowest index.​

Children of "ConsoleUI"/ORIGIN_FRAME_SIMPLE_UI_PARENT

Count: 8 to 9

[0] command button Parent

[1] BottomCenter UI parent (unit info- Inventory)

[2] ResourceBarFrame

 [0] Mouse Listener (Gold)

 [1] Mouse Listener (Lumber)

 [2] Mouse Listener (UpKeep)

 [3] Mouse Listener (Food)

 [???] Fps/Apm/Ping display

[3] UpperButtonBarFrame

 [0] Quest

 [1] Menu

 [2] Alliance

 [3] Log

[4] MiniMapButtonBar

 [0 to 4] Buttons 0(Top), 4(Bottom)

[5] command button mouse deadzone

[6] Hero ButtonBar

 [0 to 6] HeroButtons

[7] Idle worker Button Container

 [0] Button

 [0] Charges Box (created with the first idle worker)

[8] World Object Hover info (exist after any object was hovered with the mouse)

Children of BottomCenterUI

Count: 8

[0] Single Unit "SimpleInfoPanelUnitDetail"

[1] Trainer "SimpleInfoPanelBuildingDetail"

 [0] SimpleBuildTimeIndicator

 [0] Bar Border

 [1] Que Button Backgrounds

 [2] Que Button next to the Progressbar

 [3] Que Bottom Button Container

 [0 to 5] Button

 [4]

[2] Transporter "SimpleInfoPanelCargoDetail"

 [0] CargoContainer

 [0 to 9] Cargo Button Container (includes Box)

 [0] Button

 [1] Life Bar

 [2] Mana Bar

[3] Item on Ground "SimpleInfoPanelItemDetail"

[4] selectable Gate/tree/etc "SimpleInfoPanelDestructableDetail"

[5] multiple unit group selection

 [0] ContainerGroupButtons

 [0 to 11] ButtonContainer

 [0] Highlighter Background

 [1] Button

 [2] Life Bar

 [3] Mana Bar

[6] Inventory Parent

 [0 to 5] Inventory Button

[7] Inventory Cover (covers the inventory buttons when no inventory)

Children of ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR:

Count: 8

[0 to 7]: BuffIcons + mouse Listeners

Children of ORIGIN_FRAME_GAME_UI

Count: 14 on default. Without Multiboard, Leaderboard, TimerDialog, nor having quest Dialog clicked. With them it has 19.

[0] ORIGIN_FRAME_WORLD_FRAME

[1] Somehow affects the World Object Hover info box

[2] BottomUI Black Background "ConsoleUIBackdrop"

[3] Parent of ORIGIN_FRAME_PORTRAIT

[4] InventoryText, Related to the visuals of SimpleFrames, hiding it makes all SimpleFrames not visible but still clickable

[5] Day Time Clock & Minimap are affected by this one, maybe ancestor/Parent

 [0] Affects the visual daytime clock (moving it has wierd effects)

 [0] Day Time Clock Mouse Listener

 [1] MinimapFrame

[6] CinematicPanel

[7] Chat/LogDialog

[8] AllianceDialog

[9] No idea

[10] Inside Menu (F10) (Probably EscMenuBackdrop)

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

[13] Mouse Cursor

After anyone clicked the quest dialog 2 further ones are added

Custom added Frames takes slots after Chat/Log

With all

[0] ORIGIN_FRAME_WORLD_FRAME

[1] Somehow affects the World Object Hover info box

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

Children of ORIGIN_FRAME_PORTRAIT

Count: 0

Children of Parent of ORIGIN_FRAME_PORTRAIT

Count: 8

[0] ORIGIN_FRAME_PORTRAIT

[1 to 7] ORIGIN_FRAME_HERO_BUTTON_INDICATOR

Children of ORIGIN_FRAME_WORLD_FRAME

Count: 1

[0] No idea

Children of ORIGIN_FRAME_HERO_BAR

Count: 7

The Hero Buttons

Children of ORIGIN_FRAME_HERO_BUTTON

Count: 3

[0] HP-Bar

[1] MP-Bar

[2] Unspent-SkillPointsBox (only when this slot ever had unspent SkillPoints)

Children of ORIGIN_FRAME_TOOLTIP

Count: 1

Children of ORIGIN_FRAME_UBERTOOLTIP

Count: 1

Children of ORIGIN_FRAME_ITEM_BUTTON/ORIGIN_FRAME_COMMAND_BUTTON

Count: 0, 1 or 3 depends, if this slot ever was used or contained charges

Children of "SimpleInfoPanelIconDamage"...

Count: 1

[0] The mouse hover listener.

##
 Hiding default UI

There exists a native to hide all of them. call BlzHideOriginFrames(true), After one used it the screen will look like that. The ability buttons will appear as soon the player selected a unit after hide was used.

### 1.31

### Beta Nov 19 2019

After one has hidden the default ui, it would be great to use the now gained space with the playable game. The next jass line extents the world to fullscreen. The World_FRAME will copy all points of Game_UI.

 JASS:

call BlzFrameSetAllPoints(BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0))

In this reforge beta version the world Frame is fullscreen, but there was a black background box added behind the bottom UI which is not hidden by HideOrigin. It can be hidden with:

call BlzFrameSetVisible(BlzGetFrameByName("ConsoleUIBackdrop",0), false)

In the Beta also new fdf was deployed allowing to access item and Command Buttons over BlzGetFrameByName for some reason in this version moving the frames with their originframe access buggs.

("CommandButton_0", 0)

("CommandButton_11", 0)

("InventoryButton_0", 0)

("InventoryButton_5", 0)

("MiniMapFrame", 0)

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

OriginFrame HideShow.w3x

 18.4 KB

 · Views: 771



OriginFrame HideShow Reforged.w3x

 18.7 KB

 · Views: 651




 Last edited: Apr 12, 2021

#### Wrda

#### Wrda

Spell Reviewer

 Level 29

JoinedNov 18, 2012Messages2,025

Resources: 7

#### Resources
Maps 2Spells and Systems 4Lua 1

I still wonder if it's possible to hide some, and for a specific player.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

I think that should be doable, afterall frames values/state/pos are local only, handleId and events are synced.

#### Wareditor

#### Wareditor

 Level 15

JoinedJan 16, 2009Messages764

Resources: 4

#### Resources
Maps 4

One interesting note is that the portrait is not on the 4:3 screen but on the whole screen.

(0,0) would place it at the the bottom left corner of your screen and (0.8,0.6) at the top right corner.

This makes it quite hard (but possible with some math) to place the portrait in relation with other elements that are on the 4:3 screen.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

So this shows how to "get" a frame, how to set/hide a specific frame then?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Chaosy said:

 So this shows how to "get" a frame, how to set/hide a specific frame then?


Click to expand...

One uses BlzFrameSetVisible takes framehandle frame, boolean visible returns nothing on the frame. This is not recommended for SimpleFrame childs of type String or Texture.

That line would show the Hero_Bar. One should use it, if one wants the hero Icons back after one used BlzHideOriginFrames

 JASS:

call BlzFrameSetVisible(BlzGetOriginFrame(ORIGIN_FRAME_HERO_BAR,0), true)

#### Wrda

#### Wrda

Spell Reviewer

 Level 29

JoinedNov 18, 2012Messages2,025

Resources: 7

#### Resources
Maps 2Spells and Systems 4Lua 1

Tasyen said:

 One uses BlzFrameSetVisible takes framehandle frame, boolean visible returns nothing on the frame. This is not recommended for SimpleFrame childs of type String or Texture.

That line would show the Hero_Bar. One should use it, if one wants the hero Icons back after one used BlzHideOriginFrames

 JASS:

call BlzFrameSetVisible(BlzGetOriginFrame(ORIGIN_FRAME_HERO_BAR,0), true)

Click to expand...

But if one wants to show/hide for one player, then we have to do it by local player, right? It's the only option?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

I think so, yes. One has to do use GetLocalPlayer for that. In that context some people on hive discord said that using BlzGetOriginFrame inside a local block can disconnect. They recommended to use BlzGetOriginFrame synced and do only the action (show, hide, enable, repos ...) inside the GetLocalPlayer block.

Edit: BlzGetOriginFrame and/or BlzGetFrameByName will dc the game, if used localy only.

Edit Edit Added 16 July 2019 : This is only partly correct. BlzGetFrameByName/BlzGetOriginFrames can be used in GetLocalPlayer blocks without desync when the function using the same arguments was called at an earlier time for all players. My explanition for that is, this functions are creating a new handle when that frame did not have a handle yet. I was wondering before why the handleIds of basic frames where so instable different in my tests. When the first call is in a GetLocalPlayer block the handle stack gets desync and the game breaks. Custom Created Frames get handles at creation and won't desync, when they were created for all players.


 Last edited: Jul 16, 2019

#### burner89

#### burner89

 Level 3

JoinedMar 11, 2019Messages31

How do I show a units HP/mana numbers and the inventory/minimap after hiding the UI? And I can't seem to make the unit's 3d portrait hidden either.


 Last edited: Jun 17, 2019

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

burner89 said:

 show a units HP/mana numbers


Click to expand...

I do not know how to access that frame. Its one of the 5 (buffs, time clock texture, hp/Mp, Idle wroker and targeting error/help message)

burner89 said:

 minimap after hiding the UI? And I can't seem to make the unit's 3d portrait hidden either.


Click to expand...

Some examples to make frames visible again.

 JASS:

call BlzFrameSetVisible(BlzGetFrameByName("UpperButtonBarFrame",0),true) //Show Quests, Menu, Allies, Log
call BlzFrameSetVisible(BlzGetFrameByName("ResourceBarFrame",0),true) //Show Gold, Lumber, food and Upkeep; also enables /fps /ping /apm
call BlzFrameSetVisible(BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT,0), true) //show the 3D Face
call BlzFrameSetVisible(BlzGetOriginFrame(ORIGIN_FRAME_HERO_BAR,0), true) //Show the Hero Icons at left top of the screen
call BlzFrameSetVisible(BlzGetOriginFrame(ORIGIN_FRAME_MINIMAP,0), true) //Show MiniMap
call BlzFrameSetVisible(BlzFrameGetParent(BlzGetFrameByName("SimpleInfoPanelUnitDetail",0)),true)//Show the parent of the SimpleInfoPanelUnitDetail will show unit stats and inventory
call BlzFrameSetParent(BlzFrameGetParent(BlzGetOriginFrame(ORIGIN_FRAME_ITEM_BUTTON, 0)), BlzFrameGetParent(BlzGetOriginFrame(ORIGIN_FRAME_COMMAND_BUTTON, 0)))//Change parentShip of the Inventory to the command_Buttons Parent. Normaly the Inventory belongs to the SimpleInfoPanelUnitDetail parent, which could be still hidden.


 Last edited: Jun 25, 2019

#### burner89

#### burner89

 Level 3

JoinedMar 11, 2019Messages31

Tasyen said:

 I do not know how to access that frame. Its one of the 4 (buffs, time clock texture, hp/Mp and targeting error/help message)

Some examples to make frames visible again.

 JASS:

call BlzFrameSetVisible(BlzGetFrameByName("UpperButtonBarFrame",0),true) //Show Quests, Menu, Allies, Log
call BlzFrameSetVisible(BlzGetFrameByName("ResourceBarFrame",0),true) //Show Gold, Lumber, food and Upkeep; also enables /fps /ping /apm
call BlzFrameSetVisible(BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT,0), true) //show the 3D Face
call BlzFrameSetVisible(BlzGetOriginFrame(ORIGIN_FRAME_HERO_BAR,0), true) //Show the Hero Icons at left top of the screen
call BlzFrameSetVisible(BlzGetOriginFrame(ORIGIN_FRAME_MINIMAP,0), true) //Show MiniMap
call BlzFrameSetVisible(BlzFrameGetParent(BlzGetFrameByName("SimpleInfoPanelUnitDetail",0)),true)//Show the parent of the SimpleInfoPanelUnitDetail will show unit stats and inventory
call BlzFrameSetParent(BlzFrameGetParent(BlzGetOriginFrame(ORIGIN_FRAME_ITEM_BUTTON, 0)), BlzFrameGetParent(BlzGetOriginFrame(ORIGIN_FRAME_COMMAND_BUTTON, 0)))//Change parentShip of the Inventory to the command_Buttons Parent. Normaly the Inventory belongs to the SimpleInfoPanelUnitDetail parent, which could be still hidden.

Click to expand...

Cool thanks for your help. Sorry I'm a big noob when it comes to this so I have a dumb question, if I change "true" to "false" would it remove them instead?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Yes, false would hide them instead.

#### burner89

#### burner89

 Level 3

JoinedMar 11, 2019Messages31

Tasyen said:

 Yes, false would hide them instead.


Click to expand...

Hmmm for some reason this code

 Code:

function InitUI takes nothing returns nothing


 call BlzHideOriginFrames(true)
 call BlzFrameSetAllPoints(BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0))
 call BlzFrameSetVisible(BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT,0), false)

endfunction

looks like this

Am I doing something wrong?


 Last edited: Jun 17, 2019

#### Wrda

#### Wrda

Spell Reviewer

 Level 29

JoinedNov 18, 2012Messages2,025

Resources: 7

#### Resources
Maps 2Spells and Systems 4Lua 1

And what are you trying to achieve exactly?

#### burner89

#### burner89

 Level 3

JoinedMar 11, 2019Messages31

Wrda said:

 And what are you trying to achieve exactly?


Click to expand...

To get rid of the 3d unit portrait

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

the 3d-face should be hidden after using call BlzHideOriginFrames(true), atleast that happens for me.

#### Wrda

#### Wrda

Spell Reviewer

 Level 29

JoinedNov 18, 2012Messages2,025

Resources: 7

#### Resources
Maps 2Spells and Systems 4Lua 1

Yes, calling BlzFrameSetVisible(BlzGetOriginFrame(ORIGIN_FRAME_PORTRAIT,0), false) makes no sense if you're already hiding everything...

#### burner89

#### burner89

 Level 3

JoinedMar 11, 2019Messages31

 well, it looks like this with just that...wtf???

#### Wrda

#### Wrda

Spell Reviewer

 Level 29

JoinedNov 18, 2012Messages2,025

Resources: 7

#### Resources
Maps 2Spells and Systems 4Lua 1

Probably because you deleted









 JASS:

call BlzFrameSetAllPoints(BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0))

 too?

For future reference use jass tags instead of code ones when showing code.

#### burner89

#### burner89

 Level 3

JoinedMar 11, 2019Messages31

Wrda said:

 Probably because you deleted









 JASS:

call BlzFrameSetAllPoints(BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0))

 too?

For future reference use jass tags instead of code ones when showing code.


Click to expand...

Nope, I put it back in and it just removes the black bar. Of course the unit portrait/buttons disappear when a unit isn't selected, but when I select the villager it comes back.


 Last edited: Jun 17, 2019

#### burner89

#### burner89

 Level 3

JoinedMar 11, 2019Messages31

Figured all the other stuff out. This jass stuff is confusing lol.

question:

How do I get ORIGIN_FRAME_HERO_HP_BAR to show up without the hero image?

Edit: Nevermind, I just detached it from the parent and moved it off the screen


 Last edited: Jun 19, 2019

#### KeepVary

#### KeepVary

 Level 3

JoinedMay 12, 2019Messages12

Is there any way to get framehandle of the origin chat editbox? I'm not sure if it is a normal frame.

#### wc3neverdies

#### wc3neverdies

 Level 9

JoinedSep 20, 2015Messages385

Resources: 2

#### Resources
Models 1Maps 1

While i was testing this i found out that if you hide all the origin frames and select a unit and then click on the position on the portrait on the screen it still center the camera to that unit. I tired to use

 JASS:

BlzDestroyFrame

But the game crashes

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Maybe try to reduce the size of the portrait instead by posing FRAMEPOINT_TOPLEFT and FRAMEPOINT_BOTTOMRIGHT close to each other.

#### wc3neverdies

#### wc3neverdies

 Level 9

JoinedSep 20, 2015Messages385

Resources: 2

#### Resources
Models 1Maps 1

Tasyen said:

 Maybe try to reduce the size of the portrait instead by posing FRAMEPOINT_TOPLEFT and FRAMEPOINT_BOTTOMRIGHT close to each other.


Click to expand...

I will try this, thank you

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Awesome reference. Approved!

#### lishi1608

#### lishi1608

 Level 3

JoinedOct 9, 2012Messages28

Is there an fdf file for the OriginFrames? What if I want to make extra inventory UIs, do I need to write a new fdf from scratch, and handle events, tooltips, icons?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

lishi1608 said:

 Is there an fdf file for the OriginFrames? What if I want to make extra inventory UIs, do I need to write a new fdf from scratch, and handle events, tooltips, icons?


Click to expand...

I did not find any inventory fdf. Currently one would have to recreate the whole behaviour when one wants additional visible/clickable inventory slots. With everything that your map needs of them which includes probably: control click events, tooltips, icons, cooldown models and charges.

Each inventory slot would be a GLUEBUTTON, BUTTON or SIMPLEBUTTON.

If you find basic types you can use. Then you don't need to create a new fdf, Althrough fdf is cleaner and more powerful if it works.

#### Killd0zer

#### Killd0zer

 Level 7

JoinedJun 14, 2009Messages235

Do you know if these methods could be used to modify tooltips? Like to get the text within a tooltips and change it?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

I tend to no. Even if one could, how would you know which tooltip you have their?

#### lishi1608

#### lishi1608

 Level 3

JoinedOct 9, 2012Messages28

Tasyen said:

 I did not find any inventory fdf. Currently one would have to recreate the whole behaviour when one wants additional visible/clickable inventory slots. With everything that your map needs of them which includes probably: control click events, tooltips, icons, cooldown models and charges.

Each inventory slot would be a GLUEBUTTON, BUTTON or SIMPLEBUTTON.

If you find basic types you can use. Then you don't need to create a new fdf, Althrough fdf is cleaner and more powerful if it works.


Click to expand...

Thanks for your reply. I thought it would be easier with new natives. sigh..

#### xorkatoss

#### xorkatoss

 Level 20

JoinedJul 12, 2010Messages1,746

Resources: 10

#### Resources
Models 6Maps 1Spells and Systems 3

@Tasyen goes at it again with the epic UI modifications!

Nobody gonna mention this?

It shows when selection is clear, when you select an item on the ground and when you select an enemy unit without inventory.

Is it possible to hide that without hiding the whole Inventory UI?


 Last edited: Jul 14, 2019

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Its a consequence of showing the parent of "SimpleInfoPanelUnitDetail". Which enable the inventory, the item on ground info, the destructable screen and that texture.

If one would only need to show the inventory and the unit stats. One can shift the parentship to the command buttons parent then one sees the itembuttons only when one has selected an unit.

 JASS:

call BlzFrameSetParent(BlzGetFrameByName("SimpleInfoPanelUnitDetail",0), BlzFrameGetParent(BlzGetOriginFrame(ORIGIN_FRAME_COMMAND_BUTTON, 0))) //show Unit Detail when seeing command card
call BlzFrameSetParent(BlzFrameGetParent(BlzGetOriginFrame(ORIGIN_FRAME_ITEM_BUTTON, 0)), BlzFrameGetParent(BlzGetOriginFrame(ORIGIN_FRAME_COMMAND_BUTTON, 0))) //show inventory when seen command card

If one needs the item on the ground description / destructable feature, no idea how without that texture.

#### Ungoliath

#### Ungoliath

 Level 21

JoinedMar 15, 2010Messages240

Resources: 9

#### Resources
Skins 2Maps 7

xorkatoss said:

@Tasyen goes at it again with the epic UI modifications!

Nobody gonna mention this?

View attachment 328020

It shows when selection is clear, when you select an item on the ground and when you select an enemy unit without inventory.

Is it possible to hide that without hiding the whole Inventory UI?

Click to expand...

I found a solution! What I did was...

Locate the "HumanUITile-InventoryCover" texture through Warcraft 3 Viewer. This is the texture that pops up next to item descriptions, etc.

I then saved the texture as a TGA file.

I then opened the file in Photoshop and made it transparent by editing the alpha channel.

I guess you can do this for much of the UI, as long as there's gameplay underneath?


 Last edited: Aug 8, 2019

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Ungoliath said:

 I found a solution! What I did was...

Locate the "HumanUITile-InventoryCover" texture through Warcraft 3 Viewer. This is the texture that pops up next to item descriptions, etc.

I then saved the texture as a TGA file.

I then opened the file in Photoshop and made it transparent by editing the alpha channel.

Click to expand...

That is a solution yeah.

Then one Imports that transparent blp into ones map overwritting the default one. But one does not need to base the blp on the default one. With a smaller blp it is also working.

ui\console\human\humanuitile-inventorycover.blp

ui\console\nightelf\nightelfuitile-inventorycover.blp

ui\console\orc\orcuitile-inventorycover.blp

ui\console\undead\undeaduitile-inventorycover.blp

To get rid of the inventory text one replaces the text in game interface to " " (a single space) of the field INVENTORY.

#### Aniki

#### Aniki

 Level 13

JoinedNov 7, 2014Messages571

Resources: 21

#### Resources
Tools 1Maps 1Spells and Systems 14JASS 3Tutorials 2

 After one has hidden the default ui, it would be great to use the now gained space with the playable game. The next jass line extents the world to fullscreen. The World_FRAME will copy all points of Game_UI.

 JASS:

call BlzFrameSetAllPoints(BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0))

Click to expand...

How does one revert ORIGIN_FRAME_WORLD_FRAME back to its "non-fullscreen" points?

This seems to work but the values might be wrong...

 Lua:

local f = BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0)
local r = BlzGetOriginFrame(ORIGIN_FRAME_GAME_UI, 0)

local pt

pt = FRAMEPOINT_BOTTOMLEFT
BlzFrameSetPoint(f, pt, r, pt, 0.0, 0.13)

pt = FRAMEPOINT_BOTTOMRIGHT
BlzFrameSetPoint(f, pt, r, pt, 0.0, 0.13)

pt = FRAMEPOINT_TOPRIGHT
BlzFrameSetPoint(f, pt, r, pt, 0.0, -0.020)

pt = FRAMEPOINT_TOPLEFT
BlzFrameSetPoint(f, pt, r, pt, 0.0, -0.020)

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

The values look good to me.

Ungoliath said:

 I found a solution! What I did was...

Locate the "HumanUITile-InventoryCover" texture through Warcraft 3 Viewer. This is the texture that pops up next to item descriptions, etc.

I then saved the texture as a TGA file.

I then opened the file in Photoshop and made it transparent by editing the alpha channel.

I guess you can do this for much of the UI, as long as there's gameplay underneath?


Click to expand...

The Reforge Beta (nov 19 2019) has access to that frame one can just set its alpha to 0 and it is not visible anymore.

call BlzFrameSetAlpha( BlzGetFrameByName("SimpleInventoryCover", 0), 0)

After one has hidden the default UI the Item Buttons Slots don't have a background anymore but still block the screen. Might good to know where they are by giving them the backgrounds back.

#### Attachments

Custom Item Slot Backgrounds.w3x

 18.4 KB

 · Views: 328




 Last edited: Nov 21, 2019

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

call BlzEnableUIAutoPosition(false) stops the game from reposing some UI-Frames when the User swaps resolution or window mode.

#### Varimathras.

#### Varimathras.

 Level 4

JoinedJul 9, 2007Messages50

Resources: 2

#### Resources
Icons 1Maps 1

Hey Tasyen,

Thanks for the amazing tutorials!

Now with reforged online, I found that with BlzHideOriginFrames() you are left with a black bar at the bottom at the width of the 4:3 screen.

You can get rid of it with BlzFrameSetVisible(BlzGetFrameByName("ConsoleUIBackdrop",0), false).

Maybe you could update your tutorial in that part?

I also have a question:

I seem to be unable to put frames outside the 4:3 screen. Even when I anchor something to the top-right of ORIGIN_FRAME_WORLD_FRAME, it will still be in the 4:3 screen. Also, if I add a positive offset in x, the frame will but cut off at the edge of the 4:3 screen.

Do you have a solution to this? I think it should be possible because the ORIGIN_FRAME_HERO_BAR is also outside, anchored to the left of the screen.

However, I couldn't find its fdf in ui/framedef/ui/.

 Code:

frame = BlzCreateFrameByType("BACKDROP", "myframe", BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), "", 0)
--BlzFrameSetPoint(frame, FRAMEPOINT_TOPRIGHT, BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), FRAMEPOINT_TOPRIGHT, -0.01, -0.06)
BlzFrameSetPoint(frame, FRAMEPOINT_TOPRIGHT, BlzGetOriginFrame(ORIGIN_FRAME_WORLD_FRAME, 0), FRAMEPOINT_TOPRIGHT, 0.1, -0.06)
BlzFrameSetSize(frame, 0.2, 0.3)
BlzFrameSetTexture(frame, "Textures\\black32.blp", 0, true)

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Varimathras. said:

 Now with reforged online, I found that with BlzHideOriginFrames() you are left with a black bar at the bottom at the width of the 4:3 screen.

You can get rid of it with BlzFrameSetVisible(BlzGetFrameByName("ConsoleUIBackdrop",0), false).

Maybe you could update your tutorial in that part?


Click to expand...

The infos were already there, but yeah should be the default one, the old is now in a hidden tag, I did not update the map, uploaded another one for reforged.

Varimathras. said:

 I seem to be unable to put frames outside the 4:3 screen. Even when I anchor something to the top-right of ORIGIN_FRAME_WORLD_FRAME, it will still be in the 4:3 screen. Also, if I add a positive offset in x, the frame will but cut off at the edge of the 4:3 screen.

Do you have a solution to this? I think it should be possible because the ORIGIN_FRAME_HERO_BAR is also outside, anchored to the left of the screen.


Click to expand...

Not really. For some reasons custom Frames can not leave the 4:3 Screen, I say custom cause there are Frames that can leave it like Multiboards, TimerDialogs, LeaderBoard unsure about that Hero Buttons they could be simple frames which would free them anyway from the restriction. Currently only custom SimpleFrames can leave the 4:3 Part.


 Last edited: Feb 14, 2020

#### dhguardianes

#### dhguardianes

 Level 12

JoinedJun 19, 2011Messages106

Resources: 18

#### Resources
Models 11Icons 1Skins 1Maps 2Spells and Systems 3

Hey Tasyen, I want to move the minimap but no matter which ways i tried, i couldn't manage to do it. Can you show me how? Thanks

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

One needs to set 2 opposite corner Points to move the minimap, a Lua example.

 Lua:

do
 local realFunc = MarkGameStarted
 function MarkGameStarted()
 realFunc()
 local frame = BlzGetOriginFrame(ORIGIN_FRAME_MINIMAP, 0)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_BOTTOMLEFT, 0.2, 0.2)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_TOPRIGHT, 0.4, 0.4)
 end
end

Edited: Replaced selfexecution with a less problematic approach


 Last edited: Sep 22, 2020

#### bgcy_ro002

#### bgcy_ro002

 Level 5

JoinedSep 6, 2013Messages21

Resources: 5

#### Resources
Icons 1Spells and Systems 2User Interface 2

Hello Tasyen,

I want mouse click to console interface area and keep the console buttons (i just move the buttons).

It seems that the console interface area is still hidden there.

i tried fucntion







 Code:

BlzFrameSetAbsPoint(BlzGetFrameByName("ConsoleUI", 0), FRAMEPOINT_BOTTOM, 0.4, -0.18)

 but button position is deviated.

Could you help me how to mouse click to console interface area and keep the console buttons?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Did you use that line at map init?

 JASS:

BlzFrameSetAbsPoint(BlzGetFrameByName("ConsoleUI", 0), FRAMEPOINT_BOTTOM, 0.4, -0.18)

After the console was moved on can move the Command Buttons at for examaple 0.0s.

#### bgcy_ro002

#### bgcy_ro002

 Level 5

JoinedSep 6, 2013Messages21

Resources: 5

#### Resources
Icons 1Spells and Systems 2User Interface 2

Tasyen said:

 Did you use that line at map init?

 JASS:

BlzFrameSetAbsPoint(BlzGetFrameByName("ConsoleUI", 0), FRAMEPOINT_BOTTOM, 0.4, -0.18)

After the console was moved on can move the Command Buttons at for examaple 0.0s.


Click to expand...

I have understood my problem.

Thank you.

#### watterboy

#### watterboy

 Level 3

JoinedJan 20, 2020Messages48

So how exactly do I show the inventory of units after I've hidden origin frames and the console background?

This doesn't seem to work:

call BlzFrameSetVisible(BlzGetFrameByName("ORIGIN_FRAME_ITEM_BUTTON", 0), true)

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Hide origin Frames does not hide the item buttons itself, it hides one of their ancestors. A Frame needs all of his ancestor-Frames visible, to be visible itself.

The item buttons are an offspring of the Parent of the bottom center Unit Info Panel, hence you will end up with both of them visible.

If that is not a problem then this line of code would show them:

BlzFrameSetVisible(BlzFrameGetParent(BlzGetFrameByName("SimpleInfoPanelUnitDetail", 0)), true)

In the past I suggested to change parentship of the item buttons. But doing that stoped the item cooldown models form being shown.

#### watterboy

#### watterboy

 Level 3

JoinedJan 20, 2020Messages48

Tasyen said:

Hide origin Frames does not hide the item buttons itself, it hides one of their ancestors. A Frame needs all of his ancestor-Frames visible, to be visible itself.

The item buttons are an offspring of the Parent of the bottom center Unit Info Panel, hence you will end up with both of them visible.

If that is not a problem then this line of code would show them:

BlzFrameSetVisible(BlzFrameGetParent(BlzGetFrameByName("SimpleInfoPanelUnitDetail", 0)), true)

In the past I suggested to change parentship of the item buttons. But doing that stoped the item cooldown models form being shown.

Click to expand...

Thank you for elaborating! All these scripts befuddle me - these all seem extremely useful; I just wish Blizz built all of this stuff into the GUI; SC2 had more UI related stuff built right into its trigger editor - I was surprised to see that they didn't do the same with Reforged.

#### watterboy

#### watterboy

 Level 3

JoinedJan 20, 2020Messages48

Tasyen said:

Click to expand...

One other thing - is it normal that when you go in and out of cinematic mode that the game messes up the UI? I've been having to re-run the hide origins scripts whenever I go out of cinematic mode, but I may be doing something wrong

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

In reforged that happens to me as well (but not in 1.31.1). The bottom Center Unit Info Frames and the Portrait are visible again after having Entered & Left the cinimatic mode.

watterboy said:

 SC2 had more UI related stuff built right into its trigger editor - I was surprised to see that they didn't do the same with Reforged.


Click to expand...

Warcraft 3's GUI Triggers misses a lot of features that the game provide, even outside of this frame-UI features.

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

 UI - Simpleframes


Tasyen

Nov 25, 2019

Scripting Tutorials

Replies30Views16K

Sep 27, 2025
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