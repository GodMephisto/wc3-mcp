---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/typecasting.232039/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Typecasting
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/typecasting.232039/>
#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

TypeCastingBy Almia

[TD][/TD]

 What is Typecasting?

[TD][/TD]

[TD]Typecasting is a way of converting a given type to wanted type in Warcraft 3. Warcraft 3's common.j has certain functions that can already typecast:

GetHandleId -> Converts a handle to an integer

StringHash -> Converts a string to an integer

S2I -> Unlike StringHash, S2I converts a string to an integer depending on its characters, like 1,2,3 etc.

I2S -> Reversed S2I

R2I -> Converts a real to an integer

I2R -> Reversed R2I

R2S -> Similar to I2S, though real

S2R -> Reversed R2S

R2SW -> Converts real to string with exact width and precision

[/TD][TD][/TD]

 Other Typecasting

[TD][/TD][TD]

There were some times where coders wanted to typecast handles; they where sad those days. However, a few coders found ways to typecast them through abusing a bug with the game engine.

 Typecasting Methods

( Click the tabs above to view the methods )

[TD]

### Warcraft 3 1.23

### Patch 1.24

### Modern

In the Warcraft 3 1.23 versions below, a return bug was discovered.

The return bug is an inconsistency in the way the game engine handled function returns. When they check code for syntax errors, they only check the type of the last return value. However, when the code actually ran, the game engine would end the function at the first return, and automatically convert the type. This bug allowed any type to be converted into another, whether it was valid or not. This bug allows to typecast types very easily.

Here are some of the most known uses of this bug:

H2I ( Handle to Integer )

 JASS:

function H2I takes handle h returns integer
 return h
 return 0
endfunction

- This function can typecast a handle and return its ID. The ID of a handle can then be used for many things.

The first and most notable use of this function was in Local Handle Vars by KaTTaNa, but since that system relies on gamecache, it is slow and is no longer used in most maps.

As of patch 1.24, Blizzard has provided a native H2I, GetHandleId.

I2H ( Integer to Handle )

 JASS:

function I2H takes integer i returns handle
 return i
 return null
endfunction

- I2H is the opposite of H2I - it typecasts a handle ID back into a handle. Because this does not increase the reference count for the given handle, it is possible that handles returned by this function become recycled (invalid). Therefore, the I2H function shouldn't be used.

StringToInt ( String to Integer )

 JASS:

function String2Int takes string s returns integer
 return s
 return 0
endfunction

- String2Int takes a string, and returns the index for it in the Warcraft III engine's String Table.

IntToString ( Integer to String )

 JASS:

function Int2String takes integer i returns string
 return i
 return null // or return ""
endfunction

- Int2String is the inverse of String2Int, taking the index to a string in the String Table, and returning the string held in that slot.

C2I ( Code to Integer )

 JASS:

function C2I takes code c returns integer
 return c
 return 0
endfunction

- The C2I function is used to typecast a code into the position in the map code. This function in combination with I2C was used to create code arrays (out of integers)

I2C ( Code to Integer )

 JASS:

function I2C takes integer i returns code
 return i
 return null
endfunction

- The I2C function is used to typecast any integer into a code variable. However, an exploit was soon found, which allowed arbitrary byte code to be executed. This arguably caused the need for Patch 1.24.

 As of Patch 1.24, the traditional return bug no longer works. To cover up the lost functionality, the functions GetHandleId (compare to H2I) and StringHash (compare to String2Int) have been introduced.

When releasing the patch Blizzard also noted a native with the name H2I will be added in the future:

 We ask map makers to not create a GetHandleId alias function with the name H2I, as we will add a native H2I function to JASS in the future.


Click to expand...

An alternative return bug, which can be used to typecast, that works both before and after Patch 1.24 has been invented by Jesus4Lyf and Azlier.

The most commonly known return bug is the ability to typecast one Jass type into another using multiple return statements.

The Return Nothing bug lets you retrieve the last returned value. It involves calling a function which returns the desired data, and making a return impossible in the calling function. It is one function call less efficient than the traditional return bug functions.

The ability to return nothing was first noticed by Deaod, the behavior of returning nothing was defined by Azlier, and the method to return nothing was perfected by Jesus4Lyf.

The Return Nothing bug can be used to typecast, first done by Jesus4Lyf, much like the commonly known return bug even after Patch 1.24.

Here are some example codes:

Here is an example of H2I, written to to use the Return Nothing bug:

 JASS:

function ReturnHandle takes handle h returns handle
 return h
 //The Return Nothing bug can extract this function's returned value, even as another type.
endfunction

function H2I takes handle h returns integer
 call ReturnHandle(h) //Set the last returned value to our given handle.
 if false then //This can never occur.
 return 0
 endif
 //Since our handle 'h' was the last returned value, that is what this function will return, but in integer form.
 //We have successfully typecasted 'h' into an integer.
endfunction

Patch 1.24 C2I and I2C by Artificial

 JASS:

function Int takes integer i returns integer
 return i
endfunction

function Code takes code c returns code
 return c
endfunction

function C2I takes code c returns integer
 call Code(c)
 if false then
 return 0
 endif
endfunction

function I2C takes integer i returns code
 call Int(i)
 if false then
 return null
 endif
endfunction

- The return values from C2I and I2C are not "real" integers or codes. They must be transformed into "real integers" and codes via cleaner functions that merely return them. The simplest way to clean them is to pass them through Code and Int after typecasting.

 JASS:

local integer off = Int(C2I(function DoNothing))
local code c = Code(C2I(off))

 The bug was fixed in Patch 1.24c. Yet another return bug still exists.

A different approach was discovered by weaaddar. It uses the ConvertFogState native and hashtable functions to do casts from integer to handle.

The hashtable return bug is a new kind of return bug made possible by the new Hashtable API. The bug uses a combination of the behaviour of the ConvertFogState native and the hashtable implementation. The ConvertFogState native is very similar to the old I2H function, it takes an integer and returns the same integer but typed as a fogstate. It's possible to save this fogstate to a hashtable using SaveFogStateHandle. From there you can use any of the Load functions from the Hashtable API to cast the fogstate to the desired handle type.

Example:

 JASS:

function SaveHandle takes hashtable ht, integer pkey, integer ckey, handle h returns nothing
 call SaveFogStateHandle(ht, pkey, ckey, ConvertFogState(GetHandleId(h)))
endfunction

function LoadAttackTypeHandle takes hashtable ht, integer pkey, integer ckey returns attacktype
 return ConvertAttackType(GetHandleId(LoadFogStateHandle(ht, pkey, ckey)))
endfunction

function LoadDamageTypeHandle takes hashtable ht, integer pkey, integer ckey returns damagetype
 return ConvertDamageType(GetHandleId(LoadFogStateHandle(ht, pkey, ckey)))
endfunction

Here are some codes that did this way:

Handle Library by Bribe

-









 JASS:

library Handle
/**************************************************************************************************************
 * Handle API
 * ¯¯¯¯¯¯¯¯¯¯
 * You can now save <every type> into hashtables and load almost** everything, thanks to these functions.
 *
 * Since many JASS types extend a handle instead of an agent, this library enables some very useful commands.
 * Thanks to KingKing for the "ConvertFogState" trick used in many of these functions and for inspiration.
 * Thanks to Vexorian for the awesome JassHelper; any/all of these functions will inline when you use them.
 *
 *
 * Example Useage
 * ¯¯¯¯¯¯¯¯¯¯¯¯¯¯
 * call SaveHandle(hash, 0, 0, CreateTextTag())
 * call SaveHandle(hash, 0, 1, TriggerAddAction(t, function thistype.onLoop))
 *
 * call SetPlayerState(u, LoadPlayerStateHandle(hash, 0, 2), GetPlayerState(u, LoadPlayerStateHandle(hash, 0, 3)))
 * call SetUnitState(p, LoadUnitStateHandle(hash, 0, 4), 9001)
 *
 *
 * function SaveHandle takes hashtable table, integer parentKey, integer childKey, handle h returns boolean
 *
 * -> This is basically the same thing as SaveAgentHandle, only it saves *any handle* -- it does not have to
 * be an agent-type. This is extremely useful for many reasons like generic saves and for saving types
 * which could never be saved before (attacktype, damagetype, pathingtype, etc.)
 *
 * function SaveHandleId takes hashtable table, integer parentKey, integer childKey, handle h returns nothing
 *
 * -> This is pretty much a shortcut from typing GetHandleId() each time you just want to save a handle's
 * integer reference.
 *
 * function Load$NAME$Handle takes hashtable table, integer parentKey, integer childKey returns $TYPE$
 *
 * -> Loads the handle of almost everything that wasn't in the hashtable API natives.
 *
 * function Load$NAME$HandleEx takes hashtable table, integer parentKey, integer childKey returns $TYPE$
 *
 * -> If you saved the handle as an integer, this will load/typecast the desired handle from that integer.
 * I recommend using this method instead of simple Load$NAME$Handle because it's faster to do it this way.
 * Just keep in mind that you can't use this function for types that were already in the hashtable native
 * API functions - they must be loaded normally or from KingKing's typecasting library.
 */

 function SaveHandle takes hashtable table, integer parentKey, integer childKey, handle h returns boolean
 return SaveFogStateHandle(table, parentKey, childKey, ConvertFogState(GetHandleId(h)))
 endfunction

 function SaveHandleId takes hashtable table, integer parentKey, integer childKey, handle h returns nothing
 call SaveInteger(table, parentKey, childKey, GetHandleId(h))
 endfunction


 //! textmacro ConvertHandleType takes NAME, TYPE
 function Load$NAME$Handle takes hashtable table, integer parentKey, integer childKey returns $TYPE$
 return Convert$NAME$(GetHandleId(LoadFogStateHandle(table, parentKey, childKey)))
 endfunction

 function Load$NAME$HandleEx takes hashtable table, integer parentKey, integer childKey returns $TYPE$
 return Convert$NAME$(LoadInteger(table, parentKey, childKey))
 endfunction
 //! endtextmacro

 //! runtextmacro ConvertHandleType("Race", "race")
 //! runtextmacro ConvertHandleType("AllianceType", "alliancetype")
 //! runtextmacro ConvertHandleType("RacePref", "racepreference")
 //! runtextmacro ConvertHandleType("IGameState", "igamestate")
 //! runtextmacro ConvertHandleType("FGameState", "fgamestate")
 //! runtextmacro ConvertHandleType("PlayerState", "playerstate")
 //! runtextmacro ConvertHandleType("PlayerScore", "playerscore")
 //! runtextmacro ConvertHandleType("PlayerGameResult", "playergameresult")
 //! runtextmacro ConvertHandleType("UnitState", "unitstate")
 //! runtextmacro ConvertHandleType("AIDifficulty", "aidifficulty")
 //! runtextmacro ConvertHandleType("GameEvent", "gameevent")
 //! runtextmacro ConvertHandleType("PlayerEvent", "playerevent")
 //! runtextmacro ConvertHandleType("PlayerUnitEvent", "playerunitevent")
 //! runtextmacro ConvertHandleType("WidgetEvent", "widgetevent")
 //! runtextmacro ConvertHandleType("DialogEvent", "dialogevent")
 //! runtextmacro ConvertHandleType("UnitEvent", "unitevent")
 //! runtextmacro ConvertHandleType("LimitOp", "limitop")
 //! runtextmacro ConvertHandleType("UnitType", "unittype")
 //! runtextmacro ConvertHandleType("GameSpeed", "gamespeed")
 //! runtextmacro ConvertHandleType("Placement", "placement")
 //! runtextmacro ConvertHandleType("StartLocPrio", "startlocprio")
 //! runtextmacro ConvertHandleType("GameDifficulty", "gamedifficulty")
 //! runtextmacro ConvertHandleType("GameType", "gametype")
 //! runtextmacro ConvertHandleType("MapFlag", "mapflag")
 //! runtextmacro ConvertHandleType("MapVisibility", "mapvisibility")
 //! runtextmacro ConvertHandleType("MapSetting", "mapsetting")
 //! runtextmacro ConvertHandleType("MapDensity", "mapdensity")
 //! runtextmacro ConvertHandleType("MapControl", "mapcontrol")
 //! runtextmacro ConvertHandleType("PlayerColor", "playercolor")
 //! runtextmacro ConvertHandleType("PlayerSlotState", "playerslotstate")
 //! runtextmacro ConvertHandleType("VolumeGroup", "volumegroup")
 //! runtextmacro ConvertHandleType("CameraField", "camerafield")
 //! runtextmacro ConvertHandleType("BlendMode", "blendmode")
 //! runtextmacro ConvertHandleType("RarityControl", "raritycontrol")
 //! runtextmacro ConvertHandleType("TexMapFlags", "texmapflags")
 //! runtextmacro ConvertHandleType("EffectType", "effecttype")
 //! runtextmacro ConvertHandleType("Version", "version")
 //! runtextmacro ConvertHandleType("ItemType", "itemtype")
 //! runtextmacro ConvertHandleType("AttackType", "attacktype")
 //! runtextmacro ConvertHandleType("DamageType", "damagetype")
 //! runtextmacro ConvertHandleType("WeaponType", "weapontype")
 //! runtextmacro ConvertHandleType("SoundType", "soundtype")
 //! runtextmacro ConvertHandleType("PathingType", "pathingtype")
/*
* Handles that were already in the hashtable API but cannot be saved with <SaveAgentHandle>:
*
* triggeraction -> LoadTriggerActionHandle
* texttag -> LoadTextTagHandle
* unitpool -> LoadUnitPoolHandle
* itempool -> LoadItemPoolHandle
*
* **Things which still cannot be typecasted:
*
* terraindeformation
* weathereffect
* camerasetup
*/
//*************************************************************************************************************
endlibrary

- In this snippet,all kinds of handles are saved through two different ways. The difference between loading is that Bribe coded different loading functions for types.The types are loaded by ConvertTypes (see common.j)

- The code mentions that there are types that still cannot be saved/loaded by typecasting, but those are minor problems.

Typecasting Library by kingkingyyk3

-









 JASS:

//////////////////////////////////////////
// Typecasting 2.0.1
// by kingking
//
// This library provides some type
// converting functions.
//
// ====================================
// Functions provided :
// ====================================
// Agent2Widget(agent) -> widget
// Agent2Group(agent) -> group
// Agent2Trigger(agent) -> trigger
// Agent2Timer(agent) -> timer
// Agent2Location(agent) -> location
// Agent2Effect(agent) -> effect
// Agent2Unit(agent) -> unit
// Agent2Item(agent) -> item
// Widget2Unit(widget) -> unit
// Widget2Destructable(widget) -> destructable
// Widget2Item(widget) -> item
//
// Due to the usage of CovertFogState in hashtable, I2X
// is available again.
//
// Int2Widget(integer) -> widget
// Int2Destructable(integer) -> destructable
// Int2Item(integer) -> item
// Int2Unit(integer) -> unit
// Int2Ability(integer) -> ability
// Int2Timer(integer) -> timer
// Int2Trigger(integer) -> trigger
// Int2TriggerCondition(integer) -> triggercondition
// Int2TriggerAction(integer) -> triggeraction
// Int2Force(integer) -> force
// Int2Group(integer) -> group
// Int2Location(integer) -> location
// Int2Rect(integer) -> rect
// Int2Sound(integer) -> sound
// Int2Effect(integer) -> effect
// Int2UnitPool(integer) -> unitpool
// Int2ItemPool(integer) -> itempool
// Int2Quest(integer) -> quest
// Int2QuestItem(integer) -> questitem
// Int2DefeatCondition(integer) -> defeatcondition
// Int2TimerDialog(integer) -> timerdialog
// Int2Leaderboard(integer) -> leaderboard
// Int2Multiboard(integer) -> multiboard
// Int2MultiboardItem(integer) -> multiboarditem
// Int2Trackable(integer) -> trackable
// Int2Dialog(integer) -> dialog
// Int2Button(integer) -> button
// Int2TextTag(integer) -> texttag
// Int2Ubersplat(integer) -> ubersplat
// Int2Region(integer) -> region
// Int2FogState(integer) -> fogstate
// Int2FogModifier(integer) -> fogmodifier
//
// Requirement :
// Wc3 1.24b or newer
// Jasshelper 0.A.2.9 or newer
///////////////////////////////////////
library Typecasting

 globals
 private hashtable Data = InitHashtable()
 endglobals

 //! textmacro Typecasting takes ParentName, parenttype, TypeName, type
 function $ParentName$2$TypeName$ takes $parenttype$ object returns $type$
 call Save$ParentName$Handle(Data,0,0,object)
 return Load$TypeName$Handle(Data,0,0)
 endfunction
 //! endtextmacro
 //! runtextmacro Typecasting ("Agent","agent","Widget","widget")
 //! runtextmacro Typecasting ("Agent","agent","Group","group")
 //! runtextmacro Typecasting ("Agent","agent","Trigger","trigger")
 //! runtextmacro Typecasting ("Agent","agent","Timer","timer")
 //! runtextmacro Typecasting ("Agent","agent","Location","location")
 //! runtextmacro Typecasting ("Agent","agent","Effect","effect")
 //! runtextmacro Typecasting ("Agent","agent","Unit","unit")
 //! runtextmacro Typecasting ("Agent","agent","Item","item")
 //! runtextmacro Typecasting ("Widget","widget","Unit","unit")
 //! runtextmacro Typecasting ("Widget","widget","Destructable","destructable")
 //! runtextmacro Typecasting ("Widget","widget","Item","item")

 //! textmacro Typecasting_I2X takes TypeName, type
 function Int2$TypeName$ takes integer id returns $type$
 call SaveFogStateHandle(Data,0,0,ConvertFogState(id))
 return Load$TypeName$Handle(Data,0,0)
 endfunction
 //! endtextmacro
 //! runtextmacro Typecasting_I2X("Unit", "unit")
 //! runtextmacro Typecasting_I2X("Effect", "effect")
 //! runtextmacro Typecasting_I2X("Trigger", "trigger")
 //! runtextmacro Typecasting_I2X("Timer", "timer")
 //! runtextmacro Typecasting_I2X("Widget", "widget")
 //! runtextmacro Typecasting_I2X("Group", "group")
 //! runtextmacro Typecasting_I2X("Location", "location")
 //! runtextmacro Typecasting_I2X("Item", "item")
 //! runtextmacro Typecasting_I2X("Destructable", "destructable")
 //! runtextmacro Typecasting_I2X("Ability", "ability")
 //! runtextmacro Typecasting_I2X("TriggerCondition", "triggercondition")
 //! runtextmacro Typecasting_I2X("TriggerAction", "triggeraction")
 //! runtextmacro Typecasting_I2X("Force", "force")
 //! runtextmacro Typecasting_I2X("Rect", "rect")
 //! runtextmacro Typecasting_I2X("Sound", "sound")
 //! runtextmacro Typecasting_I2X("UnitPool", "unitpool")
 //! runtextmacro Typecasting_I2X("ItemPool", "itempool")
 //! runtextmacro Typecasting_I2X("Quest", "quest")
 //! runtextmacro Typecasting_I2X("QuestItem", "questitem")
 //! runtextmacro Typecasting_I2X("DefeatCondition", "defeatcondition")
 //! runtextmacro Typecasting_I2X("TimerDialog", "timerdialog")
 //! runtextmacro Typecasting_I2X("Leaderboard", "leaderboard")
 //! runtextmacro Typecasting_I2X("Multiboard", "multiboard")
 //! runtextmacro Typecasting_I2X("MultiboardItem", "multiboarditem")
 //! runtextmacro Typecasting_I2X("Trackable", "trackable")
 //! runtextmacro Typecasting_I2X("Dialog", "dialog")
 //! runtextmacro Typecasting_I2X("Button", "button")
 //! runtextmacro Typecasting_I2X("TextTag", "texttag")
 //! runtextmacro Typecasting_I2X("Image", "image")
 //! runtextmacro Typecasting_I2X("Ubersplat", "ubersplat")
 //! runtextmacro Typecasting_I2X("Region", "region")
 //! runtextmacro Typecasting_I2X("FogState", "fogstate")
 //! runtextmacro Typecasting_I2X("FogModifier", "fogmodifier")
endlibrary

- In this library, agents,widgets and integers are converted to a given type.

 The most widely known custom typecasting method was the Hashtable and ConvertType bug.

[/TD]

[/TD][TD][/TD]

 Credits

[TD][/TD][TD]

kingkingyyk3

Bribe

Jesus4Lyf

Azlier

PurgeandFire111

weaaddar

Deaod

[TD][/TD]

All the people above are the people who proposed the Typecasting. PurgeAndFire, though haven't proposed something, was the one who taught me this kind of typecasting

[/TD][TD][/TD]


 Last edited: Apr 4, 2013

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Reference of some parts of the tutorial:

http://wiki.thehelper.net/wc3/Return_Bug

http://wiki.thehelper.net/Return_Nothing

#### Greenwhy

#### Greenwhy

 Level 7

JoinedSep 9, 2007Messages253

Resources: 1

#### Resources
Icons 1

tabs?

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Yes,in the typecasting methods.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Useful, however it should be a jass tutorial?

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

I just marked it with miscellaneous because this thing is not that commonly used.

But you got a point with that.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

You missed R2SW above in tutorial header.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Oh thanks.

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Technically, it is "as of Patch 1.23b the traditional return bug no longer works":

http://forums.battle.net/thread.html?topicId=17730193178

I fixed some small grammar mistakes, and did some minor rephrasing/added spaces. Also, the images for certain users aren't showing up. You can either fix the link or remove the image.

Otherwise the tutorial looks good. From what I can tell, the tutorial seems accurate. Fix the images and that one line and it should be fine.

#### -Kobas-

#### -Kobas-

 Level 39

JoinedJan 17, 2010Messages6,111

Resources: 33

#### Resources
Icons 2Skins 1Tools 3Maps 10Spells and Systems 5Templates 5Tutorials 7

I suggest reducing credits size, took 20-25% of whole size.

You can use [stable] image [c] image [c] image [/stable] to rearrange images from

x

x

x

x

into x x x x

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

@PurgeAndFire111

They are working for me.

There are some images in Hive that shows for certain users but hides for other users(discovered this in the Maps section)

@Kobas

Kay.Let me try.

edit

@PurgeAndFire111

I see what you mean

Let me fix it.

edit

Updated.

Removed the pictures of the credited users,so that the page won't be dominated by my tutorial

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Cool, looks good. Approved.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Thanks

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Okay I tested something.

It seems that each type of handle has it's designated memory addresses to determine whether or not a handle is valid to that type.

I tried converting an item to a widget, then convert it to a unit. When printing it's handle id, it gives me 0.

I also tried converting the item's id to widget, then convert it to unit. It still gives me 0.

I also tried it for destructables and it also gave me 0.

Thoughts?

Is it because of the hashtable method or is it just that it has an specific memory address?

#### PurgeandFire

#### PurgeandFire

 Level 48

JoinedNov 11, 2006Messages7,777

Resources: 21

#### Resources
Icons 1Tools 1Spells and Systems 5JASS 4Tutorials 10

Agents are stored with type information in memory. Hashtables check against those types before loading.

As far as hashtables go, it checks if the type of the object that is stored conforms to the type you are loading. So in your case, you've stored a unit and you are trying to load an item. The native notices that the type unit does not conform to the item type, so it returns null. However, if you try to load it as a widget, it works just fine because unitdoes conform to widget (unit extends widget).

#### Zwiebelchen

#### Zwiebelchen

 Level 35

JoinedSep 17, 2009Messages7,234

Resources: 14

#### Resources
Models 5Packs 1Tools 1Maps 1Spells and Systems 1JASS 4Tutorials 1

Or in simpler terms:

You can "convert" item to widget (because all items are widgets), but you can not convert a widget to a unit (because not all widgets are units).

#### DracoL1ch

#### DracoL1ch

 Level 19

JoinedDec 12, 2010Messages2,078

Resources: 6

#### Resources
Maps 1Tutorials 5

typecasting is the biggest issue and win for wc3 community. sadly blizzards couldnt maintain it properly and banned it completely instead.

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Tossing this back as a submission for a while as the layout is broken.

It might be an easy fix soo just putting it there for a while.

#### Ralle

#### Ralle

Owner

 Level 81

JoinedOct 6, 2004Messages10,393

Resources: 43

#### Resources
Tools 5Maps 9Sounds 1Tutorials 28

##
 Warhammer 40,000 TD


I just thought it was never reviewed. But whew. Before realising you had put it back I thought about how it was older than my 6 year old who can talk and wipe herself.

#### Almia

#### Almia

 Level 33

JoinedApr 24, 2012Messages5,099

Resources: 40

#### Resources
Tools 1Spells and Systems 33JASS 2Tutorials 4

Ralle said:

 I just thought it was never reviewed. But whew. Before realising you had put it back I thought about how it was older than my 6 year old who can talk and wipe herself.


Click to expand...

I never realized this was older than I thought it was lol

 You must log in or register to reply here.


### Similar threads

 Handle


Bribe

Jul 1, 2010

Graveyard

Replies2Views3K

Feb 6, 2011
Bribe

 Shared Memory


WaterKnight

Dec 28, 2010

Tutorial Archive

Replies2Views5K

Dec 29, 2010
WaterKnight

 Table2


Vercas

Jan 9, 2010

Graveyard

Replies4Views2K

Jan 10, 2010
TriggerHappy

Locked

 Help me improve this :P


WereElf

Mar 2, 2016

Triggers & Scripts

Replies8Views2K

Mar 8, 2016
WereElf

 I2H without Typecasting


Yixx

Jul 31, 2009

Graveyard

Replies7Views2K

Dec 22, 2009
TriggerHappy

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Tutorial Archive

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top