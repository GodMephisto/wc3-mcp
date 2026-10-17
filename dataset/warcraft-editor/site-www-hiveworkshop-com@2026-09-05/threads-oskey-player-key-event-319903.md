---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/oskey-player-key-event.319903/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Oskey Player Key Event
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/oskey-player-key-event.319903/>
#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

###
 Introduction

With 1.31 Blizzard provided the feature to listen to keyboard presses and releases outside of arrowkeys and ESC. This feature is used over a new Event that can be registered to a trigger.​

###
 Simple usage

This is a small Lua example: a trigger runs when player red presses the A key then "A" is printed to the screen.

 Lua:

function Test()
 local trigger = CreateTrigger()
 BlzTriggerRegisterPlayerKeyEvent(trigger, Player(0), OSKEY_A, 0, true)
 TriggerAddAction(trigger, function()
 print("A")
 end)
end)

###
 BlzTriggerRegisterPlayerKeyEvent

The new event is created/registered with BlzTriggerRegisterPlayerKeyEvent

 JASS:

native BlzTriggerRegisterPlayerKeyEvent takes trigger whichTrigger, player whichPlayer, oskeytype key, integer metaKey, boolean keyDown returns event

BlzTriggerRegisterPlayerKeyEvent takes quite some arguments. Lets take a closer look onto them.

whichPlayer is the player which keyboard is listened, that player can start this new created event.

key is a new type insert in Warcraft 3 V1.31 (See List below)

type oskeytype extends handle

constant native ConvertOsKeyType takes integer i returns oskeytype

keyDown decides if that event fires when pressing(true) or when releasing(false) the key.

metaKey it is an integer but it is used as a bitfield. MetaKeys are "none"(0), "shift"(1), "control"(2), "alt"(4) and "META"(8) (windows key).

This MetaKeys can be combinded for example combining control and alt: 2 + 4 = 6. When the user holds control and alt and presses a oskey the event will only run when it was registered with metakey 6.

Inside the press event MetaKey-Keys always have the metakey they start themself or a bigger one if other MetaKeys are hold down during that process.

When holding down all Metakeys the result would be 1 + 2 + 4 + 8 = 15

The metakey used in that event

BlzGetTriggerPlayerIsKeyDown takes nothing returns boolean

Is this a press or release event?

GetTriggerPlayer takes nothing returns player

The player pressing/releasing the key

When the keyevent fires you can use some getter natives to know what started this event:

The oskey firing the eventBlzGetTriggerPlayerKey takes nothing returns oskeytype

The metakey used in that eventBlzGetTriggerPlayerMetaKey takes nothing returns integer

Is this a press or release event?BlzGetTriggerPlayerIsKeyDown takes nothing returns boolean

The player pressing/releasing the keyGetTriggerPlayer takes nothing returns player

The oskeytype table:

OSKEY_SHIFT, OSKEY_CONTROL, OSKEY_ALT can not evoke an keyevent use OSKEY_LSHIFT, OSKEY_LCONTROL, OSKEY_LALT, OSKEY_LMETA instead.

The numbers inside the oskeytype constants like $08 are hexadecimal numbers.

 JASS:

 constant oskeytype OSKEY_BACKSPACE = ConvertOsKeyType($08)
 constant oskeytype OSKEY_TAB = ConvertOsKeyType($09)
 constant oskeytype OSKEY_CLEAR = ConvertOsKeyType($0C)
 constant oskeytype OSKEY_RETURN = ConvertOsKeyType($0D)
 constant oskeytype OSKEY_SHIFT = ConvertOsKeyType($10)
 constant oskeytype OSKEY_CONTROL = ConvertOsKeyType($11)
 constant oskeytype OSKEY_ALT = ConvertOsKeyType($12)
 constant oskeytype OSKEY_PAUSE = ConvertOsKeyType($13)
 constant oskeytype OSKEY_CAPSLOCK = ConvertOsKeyType($14)
 constant oskeytype OSKEY_KANA = ConvertOsKeyType($15)
 constant oskeytype OSKEY_HANGUL = ConvertOsKeyType($15)
 constant oskeytype OSKEY_JUNJA = ConvertOsKeyType($17)
 constant oskeytype OSKEY_FINAL = ConvertOsKeyType($18)
 constant oskeytype OSKEY_HANJA = ConvertOsKeyType($19)
 constant oskeytype OSKEY_KANJI = ConvertOsKeyType($19)
 constant oskeytype OSKEY_ESCAPE = ConvertOsKeyType($1B)
 constant oskeytype OSKEY_CONVERT = ConvertOsKeyType($1C)
 constant oskeytype OSKEY_NONCONVERT = ConvertOsKeyType($1D)
 constant oskeytype OSKEY_ACCEPT = ConvertOsKeyType($1E)
 constant oskeytype OSKEY_MODECHANGE = ConvertOsKeyType($1F)
 constant oskeytype OSKEY_SPACE = ConvertOsKeyType($20)
 constant oskeytype OSKEY_PAGEUP = ConvertOsKeyType($21)
 constant oskeytype OSKEY_PAGEDOWN = ConvertOsKeyType($22)
 constant oskeytype OSKEY_END = ConvertOsKeyType($23)
 constant oskeytype OSKEY_HOME = ConvertOsKeyType($24)
 constant oskeytype OSKEY_LEFT = ConvertOsKeyType($25)
 constant oskeytype OSKEY_UP = ConvertOsKeyType($26)
 constant oskeytype OSKEY_RIGHT = ConvertOsKeyType($27)
 constant oskeytype OSKEY_DOWN = ConvertOsKeyType($28)
 constant oskeytype OSKEY_SELECT = ConvertOsKeyType($29)
 constant oskeytype OSKEY_PRINT = ConvertOsKeyType($2A)
 constant oskeytype OSKEY_EXECUTE = ConvertOsKeyType($2B)
 constant oskeytype OSKEY_PRINTSCREEN = ConvertOsKeyType($2C)
 constant oskeytype OSKEY_INSERT = ConvertOsKeyType($2D)
 constant oskeytype OSKEY_DELETE = ConvertOsKeyType($2E)
 constant oskeytype OSKEY_HELP = ConvertOsKeyType($2F)
 constant oskeytype OSKEY_0 = ConvertOsKeyType($30)
 constant oskeytype OSKEY_1 = ConvertOsKeyType($31)
 constant oskeytype OSKEY_2 = ConvertOsKeyType($32)
 constant oskeytype OSKEY_3 = ConvertOsKeyType($33)
 constant oskeytype OSKEY_4 = ConvertOsKeyType($34)
 constant oskeytype OSKEY_5 = ConvertOsKeyType($35)
 constant oskeytype OSKEY_6 = ConvertOsKeyType($36)
 constant oskeytype OSKEY_7 = ConvertOsKeyType($37)
 constant oskeytype OSKEY_8 = ConvertOsKeyType($38)
 constant oskeytype OSKEY_9 = ConvertOsKeyType($39)
 constant oskeytype OSKEY_A = ConvertOsKeyType($41)
 constant oskeytype OSKEY_B = ConvertOsKeyType($42)
 constant oskeytype OSKEY_C = ConvertOsKeyType($43)
 constant oskeytype OSKEY_D = ConvertOsKeyType($44)
 constant oskeytype OSKEY_E = ConvertOsKeyType($45)
 constant oskeytype OSKEY_F = ConvertOsKeyType($46)
 constant oskeytype OSKEY_G = ConvertOsKeyType($47)
 constant oskeytype OSKEY_H = ConvertOsKeyType($48)
 constant oskeytype OSKEY_I = ConvertOsKeyType($49)
 constant oskeytype OSKEY_J = ConvertOsKeyType($4A)
 constant oskeytype OSKEY_K = ConvertOsKeyType($4B)
 constant oskeytype OSKEY_L = ConvertOsKeyType($4C)
 constant oskeytype OSKEY_M = ConvertOsKeyType($4D)
 constant oskeytype OSKEY_N = ConvertOsKeyType($4E)
 constant oskeytype OSKEY_O = ConvertOsKeyType($4F)
 constant oskeytype OSKEY_P = ConvertOsKeyType($50)
 constant oskeytype OSKEY_Q = ConvertOsKeyType($51)
 constant oskeytype OSKEY_R = ConvertOsKeyType($52)
 constant oskeytype OSKEY_S = ConvertOsKeyType($53)
 constant oskeytype OSKEY_T = ConvertOsKeyType($54)
 constant oskeytype OSKEY_U = ConvertOsKeyType($55)
 constant oskeytype OSKEY_V = ConvertOsKeyType($56)
 constant oskeytype OSKEY_W = ConvertOsKeyType($57)
 constant oskeytype OSKEY_X = ConvertOsKeyType($58)
 constant oskeytype OSKEY_Y = ConvertOsKeyType($59)
 constant oskeytype OSKEY_Z = ConvertOsKeyType($5A)
 constant oskeytype OSKEY_LMETA = ConvertOsKeyType($5B)
 constant oskeytype OSKEY_RMETA = ConvertOsKeyType($5C)
 constant oskeytype OSKEY_APPS = ConvertOsKeyType($5D)
 constant oskeytype OSKEY_SLEEP = ConvertOsKeyType($5F)
 constant oskeytype OSKEY_NUMPAD0 = ConvertOsKeyType($60)
 constant oskeytype OSKEY_NUMPAD1 = ConvertOsKeyType($61)
 constant oskeytype OSKEY_NUMPAD2 = ConvertOsKeyType($62)
 constant oskeytype OSKEY_NUMPAD3 = ConvertOsKeyType($63)
 constant oskeytype OSKEY_NUMPAD4 = ConvertOsKeyType($64)
 constant oskeytype OSKEY_NUMPAD5 = ConvertOsKeyType($65)
 constant oskeytype OSKEY_NUMPAD6 = ConvertOsKeyType($66)
 constant oskeytype OSKEY_NUMPAD7 = ConvertOsKeyType($67)
 constant oskeytype OSKEY_NUMPAD8 = ConvertOsKeyType($68)
 constant oskeytype OSKEY_NUMPAD9 = ConvertOsKeyType($69)
 constant oskeytype OSKEY_MULTIPLY = ConvertOsKeyType($6A)
 constant oskeytype OSKEY_ADD = ConvertOsKeyType($6B)
 constant oskeytype OSKEY_SEPARATOR = ConvertOsKeyType($6C)
 constant oskeytype OSKEY_SUBTRACT = ConvertOsKeyType($6D)
 constant oskeytype OSKEY_DECIMAL = ConvertOsKeyType($6E)
 constant oskeytype OSKEY_DIVIDE = ConvertOsKeyType($6F)
 constant oskeytype OSKEY_F1 = ConvertOsKeyType($70)
 constant oskeytype OSKEY_F2 = ConvertOsKeyType($71)
 constant oskeytype OSKEY_F3 = ConvertOsKeyType($72)
 constant oskeytype OSKEY_F4 = ConvertOsKeyType($73)
 constant oskeytype OSKEY_F5 = ConvertOsKeyType($74)
 constant oskeytype OSKEY_F6 = ConvertOsKeyType($75)
 constant oskeytype OSKEY_F7 = ConvertOsKeyType($76)
 constant oskeytype OSKEY_F8 = ConvertOsKeyType($77)
 constant oskeytype OSKEY_F9 = ConvertOsKeyType($78)
 constant oskeytype OSKEY_F10 = ConvertOsKeyType($79)
 constant oskeytype OSKEY_F11 = ConvertOsKeyType($7A)
 constant oskeytype OSKEY_F12 = ConvertOsKeyType($7B)
 constant oskeytype OSKEY_F13 = ConvertOsKeyType($7C)
 constant oskeytype OSKEY_F14 = ConvertOsKeyType($7D)
 constant oskeytype OSKEY_F15 = ConvertOsKeyType($7E)
 constant oskeytype OSKEY_F16 = ConvertOsKeyType($7F)
 constant oskeytype OSKEY_F17 = ConvertOsKeyType($80)
 constant oskeytype OSKEY_F18 = ConvertOsKeyType($81)
 constant oskeytype OSKEY_F19 = ConvertOsKeyType($82)
 constant oskeytype OSKEY_F20 = ConvertOsKeyType($83)
 constant oskeytype OSKEY_F21 = ConvertOsKeyType($84)
 constant oskeytype OSKEY_F22 = ConvertOsKeyType($85)
 constant oskeytype OSKEY_F23 = ConvertOsKeyType($86)
 constant oskeytype OSKEY_F24 = ConvertOsKeyType($87)
 constant oskeytype OSKEY_NUMLOCK = ConvertOsKeyType($90)
 constant oskeytype OSKEY_SCROLLLOCK = ConvertOsKeyType($91)
 constant oskeytype OSKEY_OEM_NEC_EQUAL = ConvertOsKeyType($92)
 constant oskeytype OSKEY_OEM_FJ_JISHO = ConvertOsKeyType($92)
 constant oskeytype OSKEY_OEM_FJ_MASSHOU = ConvertOsKeyType($93)
 constant oskeytype OSKEY_OEM_FJ_TOUROKU = ConvertOsKeyType($94)
 constant oskeytype OSKEY_OEM_FJ_LOYA = ConvertOsKeyType($95)
 constant oskeytype OSKEY_OEM_FJ_ROYA = ConvertOsKeyType($96)
 constant oskeytype OSKEY_LSHIFT = ConvertOsKeyType($A0)
 constant oskeytype OSKEY_RSHIFT = ConvertOsKeyType($A1)
 constant oskeytype OSKEY_LCONTROL = ConvertOsKeyType($A2)
 constant oskeytype OSKEY_RCONTROL = ConvertOsKeyType($A3)
 constant oskeytype OSKEY_LALT = ConvertOsKeyType($A4)
 constant oskeytype OSKEY_RALT = ConvertOsKeyType($A5)
 constant oskeytype OSKEY_BROWSER_BACK = ConvertOsKeyType($A6)
 constant oskeytype OSKEY_BROWSER_FORWARD = ConvertOsKeyType($A7)
 constant oskeytype OSKEY_BROWSER_REFRESH = ConvertOsKeyType($A8)
 constant oskeytype OSKEY_BROWSER_STOP = ConvertOsKeyType($A9)
 constant oskeytype OSKEY_BROWSER_SEARCH = ConvertOsKeyType($AA)
 constant oskeytype OSKEY_BROWSER_FAVORITES = ConvertOsKeyType($AB)
 constant oskeytype OSKEY_BROWSER_HOME = ConvertOsKeyType($AC)
 constant oskeytype OSKEY_VOLUME_MUTE = ConvertOsKeyType($AD)
 constant oskeytype OSKEY_VOLUME_DOWN = ConvertOsKeyType($AE)
 constant oskeytype OSKEY_VOLUME_UP = ConvertOsKeyType($AF)
 constant oskeytype OSKEY_MEDIA_NEXT_TRACK = ConvertOsKeyType($B0)
 constant oskeytype OSKEY_MEDIA_PREV_TRACK = ConvertOsKeyType($B1)
 constant oskeytype OSKEY_MEDIA_STOP = ConvertOsKeyType($B2)
 constant oskeytype OSKEY_MEDIA_PLAY_PAUSE = ConvertOsKeyType($B3)
 constant oskeytype OSKEY_LAUNCH_MAIL = ConvertOsKeyType($B4)
 constant oskeytype OSKEY_LAUNCH_MEDIA_SELECT = ConvertOsKeyType($B5)
 constant oskeytype OSKEY_LAUNCH_APP1 = ConvertOsKeyType($B6)
 constant oskeytype OSKEY_LAUNCH_APP2 = ConvertOsKeyType($B7)
 constant oskeytype OSKEY_OEM_1 = ConvertOsKeyType($BA)
 constant oskeytype OSKEY_OEM_PLUS = ConvertOsKeyType($BB)
 constant oskeytype OSKEY_OEM_COMMA = ConvertOsKeyType($BC)
 constant oskeytype OSKEY_OEM_MINUS = ConvertOsKeyType($BD)
 constant oskeytype OSKEY_OEM_PERIOD = ConvertOsKeyType($BE)
 constant oskeytype OSKEY_OEM_2 = ConvertOsKeyType($BF)
 constant oskeytype OSKEY_OEM_3 = ConvertOsKeyType($C0)
 constant oskeytype OSKEY_OEM_4 = ConvertOsKeyType($DB)
 constant oskeytype OSKEY_OEM_5 = ConvertOsKeyType($DC)
 constant oskeytype OSKEY_OEM_6 = ConvertOsKeyType($DD)
 constant oskeytype OSKEY_OEM_7 = ConvertOsKeyType($DE)
 constant oskeytype OSKEY_OEM_8 = ConvertOsKeyType($DF)
 constant oskeytype OSKEY_OEM_AX = ConvertOsKeyType($E1)
 constant oskeytype OSKEY_OEM_102 = ConvertOsKeyType($E2)
 constant oskeytype OSKEY_ICO_HELP = ConvertOsKeyType($E3)
 constant oskeytype OSKEY_ICO_00 = ConvertOsKeyType($E4)
 constant oskeytype OSKEY_PROCESSKEY = ConvertOsKeyType($E5)
 constant oskeytype OSKEY_ICO_CLEAR = ConvertOsKeyType($E6)
 constant oskeytype OSKEY_PACKET = ConvertOsKeyType($E7)
 constant oskeytype OSKEY_OEM_RESET = ConvertOsKeyType($E9)
 constant oskeytype OSKEY_OEM_JUMP = ConvertOsKeyType($EA)
 constant oskeytype OSKEY_OEM_PA1 = ConvertOsKeyType($EB)
 constant oskeytype OSKEY_OEM_PA2 = ConvertOsKeyType($EC)
 constant oskeytype OSKEY_OEM_PA3 = ConvertOsKeyType($ED)
 constant oskeytype OSKEY_OEM_WSCTRL = ConvertOsKeyType($EE)
 constant oskeytype OSKEY_OEM_CUSEL = ConvertOsKeyType($EF)
 constant oskeytype OSKEY_OEM_ATTN = ConvertOsKeyType($F0)
 constant oskeytype OSKEY_OEM_FINISH = ConvertOsKeyType($F1)
 constant oskeytype OSKEY_OEM_COPY = ConvertOsKeyType($F2)
 constant oskeytype OSKEY_OEM_AUTO = ConvertOsKeyType($F3)
 constant oskeytype OSKEY_OEM_ENLW = ConvertOsKeyType($F4)
 constant oskeytype OSKEY_OEM_BACKTAB = ConvertOsKeyType($F5)
 constant oskeytype OSKEY_ATTN = ConvertOsKeyType($F6)
 constant oskeytype OSKEY_CRSEL = ConvertOsKeyType($F7)
 constant oskeytype OSKEY_EXSEL = ConvertOsKeyType($F8)
 constant oskeytype OSKEY_EREOF = ConvertOsKeyType($F9)
 constant oskeytype OSKEY_PLAY = ConvertOsKeyType($FA)
 constant oskeytype OSKEY_ZOOM = ConvertOsKeyType($FB)
 constant oskeytype OSKEY_NONAME = ConvertOsKeyType($FC)
 constant oskeytype OSKEY_PA1 = ConvertOsKeyType($FD)[/INDENT]
 constant oskeytype OSKEY_OEM_CLEAR = ConvertOsKeyType($FE)

###
 Keypress Demo

With this Lua script one can test the possible key presses which is also part of the demo map

 Lua:

function Test()
 print("Create Keys")
 for index = 8,255 do
 local trigger = CreateTrigger()
 TriggerAddAction(trigger, function()
 print("OsKey:",index, "meta",BlzGetTriggerPlayerMetaKey())
 end)
 local key = ConvertOsKeyType(index)
 for metaKey = 0,15,1 do
 BlzTriggerRegisterPlayerKeyEvent(trigger, Player(0), key, metaKey, true)
 BlzTriggerRegisterPlayerKeyEvent(trigger, Player(0), key, metaKey, false)
 end
 end
 print("Done")
end)

When an user holds the left mousebutton while the cursor points on playable ground, no oskeyevent will fire. Frames that block the playable ground (Controlstyle "AUTOTRACK" or example "FRAME") which don't keep focus do not disable oskeyevents even when the left mousebutton is hold.

As long a Frame holds focus no oskey event will be sent.

While typing in chat messages no oskey event will fire (kinda a consequenz of the statement above)

The Menus(F10) at top Left also take focus and disabling oskeyevents from that player while he has them open.​

#### Attachments

OsHotkey Tests2.w3x

 16.1 KB

 · Views: 433




 Last edited: Apr 15, 2024

#### Aniki

#### Aniki

 Level 13

JoinedNov 7, 2014Messages571

Resources: 21

#### Resources
Tools 1Maps 1Spells and Systems 14JASS 3Tutorials 2

 The numbers inside the oskeytype constants like $08 are hexadecimal numbers.


Click to expand...

The numbers also seem to be these "Virtual-Key Codes".

 MetaKeys are "none"(0), "control"(1), "shift"(2), "alt"(4) and "META"(8) (windows key).


Click to expand...

I think 'shift' is 1 and 'ctrl' is 2.

 With this Lua script one can test the possible key presses which is also part of the demo map


Click to expand...

I think this script might be "better":

 Lua:

(function()

local c_Key_Mod_None = 0
local c_Key_Mod_Shift = 1 << 0
local c_Key_Mod_Ctrl = 1 << 1
local c_Key_Mod_Alt = 1 << 2
local c_Key_Mod_Meta = 1 << 3

local function on_key_input()
 local key = GetHandleId(BlzGetTriggerPlayerKey())
 local meta = BlzGetTriggerPlayerMetaKey()
 local is_down = BlzGetTriggerPlayerIsKeyDown()

 local s = ''

 if 0 ~= meta & c_Key_Mod_Ctrl then
 s = s .. 'ctrl+'
 end
 if 0 ~= meta & c_Key_Mod_Shift then
 s = s .. 'shift+'
 end
 if 0 ~= meta & c_Key_Mod_Alt then
 s = s .. 'alt+'
 end
 if 0 ~= meta & c_Key_Mod_Meta then
 s = s .. 'meta+'
 end

 s = s .. string.format('<0x%02X>', key)
 s = s .. (is_down and ' down' or ' up')

 print(s)
end

local t = CreateTrigger()
TriggerAddAction(t, on_key_input)
 -- loop over only the players that are present, i.e don't use this sort of loop:
 -- for p = 0, bj_MAX_PLAYERS-1 do

 local p = 0
 local pp = Player(p)

 for k = 0x00, 0xFF do
 local key = ConvertOsKeyType(k)
 for meta = 0, 15 do
 BlzTriggerRegisterPlayerKeyEvent(t, pp, key, meta, true)
 BlzTriggerRegisterPlayerKeyEvent(t, pp, key, meta, false)
 end
 end

 -- end

end)()

PS: maybe Blizzard should try to document at least some of their new APIs...

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Aniki said:

 I think 'shift' is 1 and 'ctrl' is 2


Click to expand...

You are right. I mixed that up, Thanks for pointing that out. Even my own code shows that .

Aniki said:

 The numbers also seem to be these "Virtual-Key Codes".


Click to expand...

Thanks for sharing that.

Your code shows the Hex values like they are written in common.j which is great so one does not need a converter (but if one uses world Editor (like me) it only works after one added a 2. % into string.format)

Aniki said:

 PS: maybe Blizzard should try to document at least some of their new APIs...


Click to expand...

Would be great, but they seem busy with fixing and releasing new stuff.

#### Azoth

#### Azoth

 Level 4

JoinedSep 10, 2013Messages39

Meh ... and its not possible to convert this in GUI?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Blizzard did not port Oskey events to GUI. Hence you either have to wait until they do, use a custom GUI TriggerDataBase having them or use in jass/Lua written code for the oskey events.

#### Azoth

#### Azoth

 Level 4

JoinedSep 10, 2013Messages39

Can someone change the function in that way, that W/S/A/D down+up (1 example is enough) will run a GUI trigger + put the information "TriggerPlayer" in that trigger

This must be a great possibility to change my ArrowKey-Movement system to a WSAD-Movement system, isnt it?

#### Azoth

#### Azoth

 Level 4

JoinedSep 10, 2013Messages39

And why i can save and test the attachted file, but when i copy+paste the (Custom)Trigger, it shows me an syntax error message?

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Azoth said:

 And why i can save and test the attachted file, but when i copy+paste the (Custom)Trigger, it shows me an syntax error message?


Click to expand...

Your map probably is in Jass mode, while the code posted here and in the map are in Lua.

#### maddeem

#### maddeem

 Level 24

JoinedJan 1, 2011Messages1,506

Resources: 34

#### Resources
Models 5Icons 3Skins 1Maps 6Spells and Systems 18Lua 1

 When an user holds the left mousebutton while the cursor points on playable ground, no oskeyevent will fire. Frames that block the playable ground (Controlstyle "AUTOTRACK" or example "FRAME") which don't keep focus do not disable oskeyevents even when the left mousebutton is hold.

As long a Frame holds focus no oskey event will be sent.


Click to expand...

Any work around for the left click disabling input?

#### ScorpioT1000

#### ScorpioT1000

 Level 5

JoinedJun 26, 2013Messages49

Resources: 1

#### Resources
Tools 1

I found the solution

On click use

 Code:

if(GetLocalPlayer() == GetTriggerPlayer()) then
 EnableUserControl(true)
end

also you can import my Indaxia/lua-inputMovement for movement purposes

#### maddeem

#### maddeem

 Level 24

JoinedJan 1, 2011Messages1,506

Resources: 34

#### Resources
Models 5Icons 3Skins 1Maps 6Spells and Systems 18Lua 1

Does this cause sync issues?

#### ScorpioT1000

#### ScorpioT1000

 Level 5

JoinedJun 26, 2013Messages49

Resources: 1

#### Resources
Tools 1

no as you can see local player check but ain't tested yet

blizzard.j uses it with GetLocalPlayer check as well

#### maddeem

#### maddeem

 Level 24

JoinedJan 1, 2011Messages1,506

Resources: 34

#### Resources
Models 5Icons 3Skins 1Maps 6Spells and Systems 18Lua 1

Very cool discovery. I'll test to see if this is sync safe.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Looks nice on the first view. On a second view there is a problem that it closes Menu/Quest/Alliance/Chat when one presses this buttons with the mouse (I also had a feeling there was something off with selecting units).

That also could be a wanted feature.

Whatever, thanks for sharing this.

My test code

 Lua:

do
 local real = MarkGameStarted
 function MarkGameStarted()
 real()
 local trigger = CreateTrigger()
 BlzTriggerRegisterPlayerKeyEvent(trigger, Player(0), OSKEY_A, 0, true)
 TriggerAddAction(trigger, function()
 print("A")
 end)
 trigger = CreateTrigger()
 TriggerRegisterPlayerEvent(trigger, Player(0), EVENT_PLAYER_MOUSE_DOWN)
 TriggerAddAction(trigger, function()
 print("Mouse")
 if(GetLocalPlayer() == GetTriggerPlayer()) then
 EnableUserControl(true)
 end
 end)
 print("done")
 end
end

Edited: Replaced selfexecution with a less problematic approach


 Last edited: Sep 22, 2020

#### MrModify

#### MrModify

 Level 4

JoinedMar 4, 2020Messages21

Resources: 1

#### Resources
Tutorials 1

How do you get tutorials reviewed? Mine is invisible since march.

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Not sure if this is the right place, but is there any way to detect Mouse scrolling?

I cannot find an appropriate key, and PLAYER_MOUSE_DOWN only refers to mouse clicks.

I do see there is a 'frame event mouse wheel';

 Lua:

BlzTriggerRegisterFrameEvent(Trig_Mouse, BlzGetOriginFrame(ORIGIN_FRAME_WORLD, 0), FRAMEEVENT_MOUSE_WHEEL)

But mouse scrolling does not activate this trigger.

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Frames can only use some Frameevents based on their type, I made a list in this post: [JASS/AI] - UI: FrameEvents and FrameTypes

You could create one big BUTTON and let it cover the whole screen. This BUTTON can not be seen and will catch all the mouse wheel scroling. Buttons tend to keep the Focus when they are clicked (with the mouse9 therefore one would have to enable/disable the Button when it throws FRAMEEVENT_CONTROL_CLICK.

But it might have a negative effect on the game control. it would for sure disable the zooming in/out with the mouse wheel.

#### pr114

#### pr114

 Level 10

JoinedMar 26, 2017Messages378

Resources: 1

#### Resources
Icons 1

Hmm, I've tried it, but it didn't produce the desired results.

-It does pick up mouse wheel correctly, but cannot distinguish between scrolling up and down.

-Scrolling only works in the 3:4 screen, and not at the edges.

What I really want to accomplish, is to make mouse scroll up set the camera to a higher elevation than is normally possible (height of 2500). It is more intuitive, instead of making players need to type a command.

It seems that unfortunately, it is not possible through this method.

#### watterboy

#### watterboy

 Level 3

JoinedJan 20, 2020Messages48

I'm terrible with this stuff - I try making the bones of the trigger in GUI, then convert to custom text to insert the BlizTriggerRegister line, but I keep getting errors. No idea why they couldn't just add a GUI for this like in SC2. Can anybody tell me why this is giving me an error?

 Code:

function Trig_MoveStart_Copy_Actions takes nothing returns nothing
 call EnableTrigger( gg_trg_Moving )
endfunction

//===========================================================================
function InitTrig_MoveStart_Copy takes nothing returns nothing
 set gg_trg_MoveStart_Copy = CreateTrigger( )
 BlzTriggerRegisterPlayerKeyEvent(trigger,Player(0),OSKEY_W,0,true)
 call TriggerAddAction( gg_trg_MoveStart_Copy, function Trig_MoveStart_Copy_Actions )
endfunction

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

BlzTriggerRegisterPlayerKeyEvent(trigger,Player(0),OSKEY_W,0,true)

->

call BlzTriggerRegisterPlayerKeyEvent(trigger,Player(0),OSKEY_W,0,true)

in jass one needs call or set infront of most lines

#### watterboy

#### watterboy

 Level 3

JoinedJan 20, 2020Messages48

Tasyen said:

 BlzTriggerRegisterPlayerKeyEvent(trigger,Player(0),OSKEY_W,0,true)

->

call BlzTriggerRegisterPlayerKeyEvent(trigger,Player(0),OSKEY_W,0,true)

in jass one needs call or set infront of most lines


Click to expand...

Thank you for the reply - I am still getting an error on that line though.

 Code:

function Trig_MoveStart_Copy_Actions takes nothing returns nothing
 call EnableTrigger( gg_trg_Moving )
endfunction

//===========================================================================
function InitTrig_MoveStart_Copy takes nothing returns nothing
 set gg_trg_MoveStart_Copy = CreateTrigger( )
 call BlzTriggerRegisterPlayerKeyEvent(trigger,Player(0),OSKEY_W,0,true)
 call TriggerAddAction( gg_trg_MoveStart_Copy, function Trig_MoveStart_Copy_Actions )
endfunction

I'm almost positive its because I'm stupid, and I can't just replace the script line of the "if player presses forward key" with that "call BlzTriggerRegister" line - but I have no idea what I'm doing with jass

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

call BlzTriggerRegisterPlayerKeyEvent(trigger,Player(0),OSKEY_W,0,true)

->

call BlzTriggerRegisterPlayerKeyEvent(gg_trg_MoveStart_Copy ,Player(0),OSKEY_W,0,true)

#### watterboy

#### watterboy

 Level 3

JoinedJan 20, 2020Messages48

Tasyen said:

 call BlzTriggerRegisterPlayerKeyEvent(trigger,Player(0),OSKEY_W,0,true)

->

call BlzTriggerRegisterPlayerKeyEvent(gg_trg_MoveStart_Copy ,Player(0),OSKEY_W,0,true)


Click to expand...

Oh damn- booyah that did the trick. Thank you so much sir- finally having access to keyboard events is gonna be legendary.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Reformated slightly.

Added titles

Put some inline jass in a table to be easier to read

indented

#### Bribe

#### Bribe

 Level 52

JoinedSep 26, 2009Messages9,548

Resources: 34

#### Resources
Tools 2Maps 3Spells and Systems 13JASS 9Lua 3Tutorials 4

Does anyone know if there are keys to determine if a key is currently pressed (a non-event key) and if these natives will fire even if user control is disabled?

#### Cheshire

#### Cheshire

 Level 26

JoinedMar 29, 2020Messages1,482

Resources: 4

#### Resources
Icons 3Maps 1

Hey, a few question about this:

1. is there any way to detect more than one key at once besides the metakeys? like to give the order for diagonal movement on a wasd system?

2. if you use a key pressed (keydown=true) event it will keep firing over and over while the key is held down. however, once a different key (that fires Oskey) is pressed - even if you are still holding down the first key and you have already released the second key - the first one will no longer register. Is there a way to avoid this?

3. is there a condition to filter - key used in Oskey event == x?

#### maddeem

#### maddeem

 Level 24

JoinedJan 1, 2011Messages1,506

Resources: 34

#### Resources
Models 5Icons 3Skins 1Maps 6Spells and Systems 18Lua 1

Cheshire said:

 Hey, a few question about this:

1. is there any way to detect more than one key at once besides the metakeys? like to give the order for diagonal movement on a wasd system?

2. if you use a key pressed (keydown=true) event it will keep firing over and over while the key is held down. however, once a different key (that fires Oskey) is pressed - even if you are still holding down the first key and you have already released the second key - the first one will no longer register. Is there a way to avoid this?

3. is there a condition to filter - key used in Oskey event == x?


Click to expand...

1. Yes. Key press/release will fire even when other keys are pressed.

2. Yes, you must save key states in a boolean and do comparisons to see if the key state has changed before firing your events.









 JASS:

if BlzGetTriggerPlayerIsKeyDown() != keystate then
set keystate = BlzGetTriggerPlayerIsKeyDown()
. . .
endif

3. Yes BlzGetTriggerPlayerKey

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Tasyen said:

 When an user holds the left mousebutton while the cursor points on playable ground, no oskeyevent will fire. Frames that block the playable ground (Controlstyle "AUTOTRACK" or example "FRAME") which don't keep focus do not disable oskeyevents even when the left mousebutton is hold.


Click to expand...

Hey @Tasyen,

could you give an example of how to create such a frame that blocks playable ground (beyond 4:3 borders), but doesn't block oskey and click events?

I think, the creation method would even be a good addition to the tutorial, as you've already mentioned it there.

Thank you very much and have a nice day!

#### Tasyen

#### Tasyen

 Level 40

JoinedJul 18, 2010Messages2,476

Resources: 82

#### Resources
Tools 3Maps 3Spells and Systems 25User Interface 15JASS 3Lua 10Tutorials 23

Eikonium said:

 Hey @Tasyen,

could you give an example of how to create such a frame that blocks playable ground (beyond 4:3 borders), but doesn't block oskey and click events?

I think, the creation method would even be a good addition to the tutorial, as you've already mentioned it there.

Thank you very much and have a nice day!


Click to expand...

Here you go: creates the key/mouse trigger for player red, than a FRAME that fills the screen and beyond. It disables selecting any unit and giving orders but key event and mouse down works.

One has to call the function NoPlayAbleGroundClick() for it to do anything.

 Lua:

function NoPlayAbleGroundClick()
 local trigger = CreateTrigger()
 TriggerAddAction(trigger, function()
 print("OsKey:", GetHandleId(BlzGetTriggerPlayerKey()), "meta",BlzGetTriggerPlayerMetaKey())
 end)
 for index = 8,255 do
 local key = ConvertOsKeyType(index)
 for metaKey = 0,15,1 do
 BlzTriggerRegisterPlayerKeyEvent(trigger, Player(0), key, metaKey, true)
 BlzTriggerRegisterPlayerKeyEvent(trigger, Player(0), key, metaKey, false)
 end
 end
 trigger = CreateTrigger()
 TriggerRegisterPlayerEvent(trigger, Player(0), EVENT_PLAYER_MOUSE_DOWN)
 TriggerAddAction(trigger, function()
 print("MouseClick:", BlzGetTriggerPlayerMouseX(), BlzGetTriggerPlayerMouseY())
 end)

 local parent
 if GetLocalizedString("REFORGED") ~= "REFORGED" then
 -- 1.32+
 parent = BlzGetFrameByName("ConsoleUIBackdrop", 0)
 else
 -- 1.31.1
 parent = BlzGetFrameByName("Leaderboard", 0)
 -- already have a useable Learboard, then just use it?
 if GetHandleId(parent) == 0 then
 -- create a "hidden" one and use it as parent
 CreateLeaderboardBJ(bj_FORCE_ALL_PLAYERS, "title")
 parent = BlzGetFrameByName("Leaderboard", 0)
 BlzFrameSetSize(parent, 0, 0)
 BlzFrameSetVisible(BlzGetFrameByName("LeaderboardBackdrop", 0), false)
 BlzFrameSetVisible(BlzGetFrameByName("LeaderboardTitle", 0), false)
 -- the timer is only required, if your map wants to use a normal leaderboard
 TimerStart(CreateTimer(), 0.1, true, function()
 BlzFrameSetVisible(parent, true)
 end)
 end
 end
 local frame = BlzCreateFrameByType("FRAME", "", parent, "", 0)
 BlzFrameSetAbsPoint(frame, FRAMEPOINT_BOTTOM, 0.4, 0)
 BlzFrameSetSize(frame, 100, 1)
end

#### Eikonium

#### Eikonium

Patron

 Level 23

JoinedJul 10, 2009Messages545

Resources: 15

#### Resources
Maps 2Spells and Systems 6Lua 5Tutorials 2

Thank you very much, Sir!

#### emperor_d3st

#### emperor_d3st

 Level 18

JoinedApr 13, 2008Messages1,629

Resources: 2

#### Resources
Ported Models 2

Sir Tasyen the wise: spreading knowledge. May he live forever.

 You must log in or register to reply here.


### Similar threads

Locked

[General]
 vJass dollar sign in Reforged?


looking_for_help

Dec 27, 2020

World Editor Help Zone

Replies2Views2K

Dec 27, 2020
looking_for_help

Locked

 WASD movement help


WyrmSlayer

Dec 13, 2019

Triggers & Scripts

Replies2Views2K

Dec 13, 2019
WyrmSlayer

Locked

[Crash]
 Oskey Crash And UI Button Crash Question


C.B.King

Jun 21, 2022

World Editor Help Zone

Replies5Views1K

Jun 21, 2022
Uncle

Locked

 Unit play animation backwards when moving in the opposite direction of facing


wc3neverdies

Aug 1, 2019

World Editor Help Zone

Replies4Views1K

Aug 16, 2019
Pyrogasm

Locked

[Solved]
 JASS WASD Movement working only for Player(0)


wc3neverdies

Aug 16, 2019

Triggers & Scripts

Replies11Views2K

Aug 17, 2019
wc3neverdies

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