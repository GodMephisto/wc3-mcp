---
type: vendor-doc
package: site-www-hiveworkshop-com
package_version: latest
language: reference
source_page: https://www.hiveworkshop.com/threads/make-a-map-hero-defense-with-ogre-magi.355765/
source_url: https://www.hiveworkshop.com
extractor: HTML extraction
exported: 2026-09-05
---

# Make a Map Hero Defense with Ogre Magi
Acquired via HTML extraction from <https://www.hiveworkshop.com/threads/make-a-map-hero-defense-with-ogre-magi.355765/>
#### SunLord

#### SunLord

 Level 8

JoinedNov 5, 2012Messages79

Resources: 9

#### Resources
Maps 5Spells and Systems 3Tutorials 1

##
 Ogre X Human


This is my tutorial for how to make a hero defense map. When it finish, we can use this base code as system for hero defense map

Hope it is of some use to you

List of content video #1:

Draw Terrain/Destruct/Doodad

Create The Base

Pick up Hero

Create region for spawn enemy

Make wave running

List of content video #2:

Pan camera when start the game, pick hero

Timer board for wave

Bounty gold player enemy

Shop item

Random pick hero

Condition lose game

Can you give me suggestions on what I will do in the next video!

List of content video #3:

Spliting file and something about Player Struct

Revive Hero

Safe Area for healing join by region ( Use Way Gate Ability )

Wave boss

List of content video #4:

Drop chance item (Drop all, Drop by Chance, Drop one)

Combine item (recipe)
List to do next video is:

Creep area
1-EventSpell

 JASS:

struct SPELL
 static method SpellEffect takes nothing returns boolean
 local integer abicode = GetSpellAbilityId()
 local unit caster = GetSpellAbilityUnit() //GetSpellAbilityUnit() is unit as caster of spell now
 local unit target = GetSpellTargetUnit() // GetSpellTargetUnit() is unit as target of spell now

 //ABILITY: PICK HERO
 if abicode == 'A000' then // 'A000' is ID of spell Pick Hero in wisp unit, you can check ID in F6 (Object manager) press Ctrl + D, it will show up

 //Change owner
 if IsUnitType(target, UNIT_TYPE_HERO) == true then // IsUnitType use for check type of unit like as hero, structure, sleeping, undead...
 call.PickAHero(caster, target)
 endif
 //GetUnitTypeId: unit whichUnit => Get FourCC ( ID of unit)
 if GetUnitTypeId(target) == 'e001' then
 //Make roll here
 call.RandomHero(caster)
 endif

 endif

 return false
 endmethod
 private static method PickAHero takes unit caster, unit target returns nothing
 local integer pid = GetPlayerId(GetOwningPlayer(caster))
 call PanCameraToTimedLocForPlayer(GetOwningPlayer(caster), GetRectCenter(udg_TheRevive), 1) // Pan camera to TheRevive Region , we need catch Player who is casting this spell for pan camera, I use GetOwningPlayer
 call SetUnitOwner(target, GetOwningPlayer(caster), true) //SetUnitOwner use for change a unit for a new player
 call KillUnit(caster) // Kill wisp //KillUnit is kill unit !
 set GAME.PLAYERS [ pid ].Hero = target
 call SetUnitPosition(target, GetRectCenterX(udg_TheRevive), GetRectCenterY(udg_TheRevive)) //SetUnitPosition use for move a unit to a postion (it will correct the position to move to if there is something at that position)
 endmethod
 private static method RandomHero takes unit caster returns nothing
 local unit array List
 local unit e = null
 local group g = null
 local integer i = 0
 local integer roll = 0


 set g = CreateGroup() // Then i create a group
 call GroupEnumUnitsInRect(g, udg_PickHeroArea, null) // Get all unit in region udg_PickHeroArea set it in group
 loop
 set e = FirstOfGroup(g)
 exitwhen e == null
 if IsUnitType(e, UNIT_TYPE_HERO) == true then
 set i = i + 1
 set List [ i ] = e
 endif
 call GroupRemoveUnit(g, e)
 endloop
 call GroupClear(g) //
 call DestroyGroup(g)
 set e = null

 if i == 0 then
 call BJDebugMsg("Not have hero in hero area for pick")
 else
 set roll = GetRandomInt(1, i) // This function roll 1 to i , i increase when in group have hero
 call.PickAHero(caster, List [ roll ])
 endif
 endmethod
endstruct

2-EventUnitDeath

 JASS:

struct UNIT_DEATH
 static method Checking takes nothing returns boolean
 local unit killer = GetKillingUnit() // This get killing unit in event EVENT_PLAYER_UNIT_DEATH
 local unit dying = GetDyingUnit() // This get dying unit in event EVENT_PLAYER_UNIT_DEATH
 local integer pid = GetPlayerId(GetOwningPlayer(GetDyingUnit()))
 if dying == udg_TheBase then
 call CustomDefeatBJ(Player(0), "You lose,my son!")
 return false
 endif
 if dying == GAME.PLAYERS [ pid ].Hero then
 call PLAYER_ACTION.Revive(pid)
 endif
 return false
 endmethod
endstruct

3-Wave

 JASS:

struct WAVE
 //I will deal with issues related to waves, and wave monsters here
 // It our Timer, we need one for control the wave countdown
 //Setting level now
 static integer Level = 1
 // We made Creep ID use FourCC ( ID of unit), FourCC is integer, then set it type variable is integer
 static integer array CreepID
 static timer t = null
 static timer timerfood = null
 static timerdialog timerwindow = null
 private static method WaveExcuted takes nothing returns nothing
 //This is excuted function when timer expried
 local integer n = 1
 local location loc = null // Location is a point (have X,Y) in the map
 //udg_ is the syntax to call the variable you set in trigger (F4). udg_ + name of variable
 loop
 exitwhen n > 8
 //GetRandomLocInRect: rect whichRect
 set loc = GetRandomLocInRect(udg_Way1) // GetRandomLocInRect is take a Location have X,Y random and contain Rect udg_Way1 rect is Region in map. is wass setup udg_Way1 in trigger Setting
 //CreateUnit: player id, integer unitid, real x, real y, real face
 call CreateUnit(Player(10),.CreepID [.Level ], GetLocationX(loc), GetLocationY(loc), 0) // GetLocationX, GetLocationY get X,Y of loc u are set it before. 'n000' is ID of creep
 //RemoveLocation: location whichLocation
 call RemoveLocation(loc) // Remove Location and set new value for way2
 set loc = GetRandomLocInRect(udg_Way2)
 call CreateUnit(Player(10),.CreepID [.Level ], GetLocationX(loc), GetLocationY(loc), 0)
 call RemoveLocation(loc)
 set loc = GetRandomLocInRect(udg_Way3)
 call CreateUnit(Player(10),.CreepID [.Level ], GetLocationX(loc), GetLocationY(loc), 0)
 call RemoveLocation(loc)
 set n = n + 1
 endloop
 //When all unit is show up, then open then timer checkfood, it's logic
 set.timerfood = CreateTimer()
 call TimerStart(.timerfood, 1, true, function thistype.CheckFood) //

 call DestroyTimer(.t)
 call DestroyTimerDialog(.timerwindow) // Destroy timerwindow when timer excuted
 endmethod
 private static method BossWave takes nothing returns nothing
 local location loc = null
 local integer BOSS_LV3 = 'n003'
 if.Level == 3 then
 set loc = GetRandomLocInRect(udg_Way2)
 call CreateUnit(Player(10), BOSS_LV3, GetLocationX(loc), GetLocationY(loc), 0)
 call RemoveLocation(loc)
 endif
 //When all unit is show up, then open then timer checkfood, it's logic
 set.timerfood = CreateTimer()
 call TimerStart(.timerfood, 1, true, function thistype.CheckFood) //

 call DestroyTimer(.t)
 call DestroyTimerDialog(.timerwindow) // Destroy timerwindow when timer excuted
 endmethod
 private static method IsBossWave takes nothing returns boolean
 // Call here all level is boss wave
 if.Level == 3 or.Level == 5 then
 return true
 endif
 return false
 endmethod
 private static method SetupWave takes nothing returns nothing
 //We can setup type of unit here
 set.CreepID [ 1 ] = 'n000' // ==> It's creep we can call in level 1
 set.CreepID [ 2 ] = 'n001' // ==> It's creep we can call in level 2
 //It is start, you can setup here but i dont want do it here, make a timer setup all in 0.1s after
 call.CallNewWave()
 call DestroyTimer(GetExpiredTimer()) // This for destroy timer call one-time , it call to the timer excuted this function
 endmethod
 private static method CallNewWave takes nothing returns nothing
 //by same code, i will make a new method use same code
 set.t = CreateTimer()
 if.IsBossWave() == false then
 call TimerStart(.t, 10, false, function thistype.WaveExcuted)
 set.timerwindow = CreateTimerDialogBJ(.t, "Wave " + I2S(.Level))
 else
 call TimerStart(.t, 10, false, function thistype.BossWave)
 set.timerwindow = CreateTimerDialogBJ(.t, "Wave " + I2S(.Level) + "-- BOSS")
 endif
 call TimerDialogDisplayBJ(true,.timerwindow) // It's function show/hide timer window
 endmethod
 private static method CheckFood takes nothing returns nothing
 // call BJDebugMsg("Food used:" + I2S(GetPlayerState(Player(10), PLAYER_STATE_RESOURCE_FOOD_USED)) ) //BJDebugMsg will dump string in game, u can see it
 // I need destroy this timer when checkfood == 0 done
 if GetPlayerState(Player(10), PLAYER_STATE_RESOURCE_FOOD_USED) == 0 then
 //Call next wave !
 set.Level =.Level + 1
 call.CallNewWave()
 //Remember destroy this timer
 call DestroyTimer(.timerfood)
 endif
 endmethod
 private static method onInit takes nothing returns nothing
 call TimerStart(CreateTimer(), 1, false, function thistype.SetupWave) // call one!
 endmethod
endstruct

4-Order

 JASS:

struct ORDER
 static integer Move = 851986
 static integer Almove = 851988
 static integer Attack = 851983
 static integer Drain = 852487
 // Add more channel skill here, unit order will not cancel the skill
 //Check order skill by here : https://docs.google.com/spreadsheets/d/1Thrt5b69SRfWvQoBZkMa6RN5TAPBXrG6L_QmSB_3WRI/edit?usp=sharing
 static method IO takes unit u, integer order returns boolean
 return GetUnitCurrentOrder(u) == order // Returns true or false when comparing the input order id value with the current unit's order value
 endmethod
 //IsNotAction is check a unit not action or not do something.
 static method IsNotAction takes unit u returns boolean
 return not(.IO(u,.Move) or.IO(u,.Almove) or.IO(u,.Attack) or.IO(u,.Drain))
 endmethod
 // Order Attack :
 //I use a timer to make a process check if the enemy is attacking towards the base or not? If it doesn't do anything or isn't the type of unit that can attack, I'll order it to attack again.
 private static method OrderAttack takes nothing returns nothing
 local group g = null
 local unit e = null
 // I need to group the enemy player's units and check each enemy one by one.
 set g = CreateGroup() // Then i create a group
 call GroupEnumUnitsOfPlayer(g, Player(10), null) // GroupEnumUnitsOfPlayer Collect all of player 11 current enemies into group g
 //Basically loop is used to iterate within its container, you need exitwhen condition for it to exit. If you use a GUI, its mechanism is in the ForEach function
 loop
 set e = FirstOfGroup(g)
 exitwhen e == null
 //GetUnitState: unit whichUnit, unitstate whichUnitState => I use it for check unit is alive
 //GetPlayerId:player whichPlayer if id == 0 then it's player 1, then if == 10 then it's player 11.
 //ORDER.IsNotAction(e) Call struct ORDER and check if the unit's current order is not doing anything. If it is doing nothing, order it to attack again. This is useful because if you order it to attack continuously, it will destroy what it is currently doing for example when that unit is draining mana on 1 enemy. meaning it is still causing damage and the action should not be canceled.
 if GetUnitState(e, UNIT_STATE_LIFE) > 0 and IsUnitType(e, UNIT_TYPE_STRUCTURE) == false and GetPlayerId(GetOwningPlayer(e)) == 10 and ORDER.IsNotAction(e) then
 //IssuePointOrder: unit whichUnit, string order, real x, real y => Here I order the enemy to attack the TheRevive region, it is near TheBase, so the unit will also attack TheBase.
 call IssuePointOrder(e, "attack", GetRectCenterX(udg_TheRevive), GetRectCenterY(udg_TheRevive))
 endif
 call GroupRemoveUnit(g, e) // By removing units from the group in the loop, the number of units in the group will gradually decrease, and when unit e == null means when the group is empty, you end the loop you just processed.
 endloop
 //One action to do when you finish processing groups if you don't use them anymore is groupclear and destroygroup, assign the value unit e = null although sometimes it is not necessary
 call GroupClear(g) //
 call DestroyGroup(g)
 set e = null

 endmethod
 private static method onInit takes nothing returns nothing
 call TimerStart(CreateTimer(), 2, true, function thistype.OrderAttack) // if true it become every 2 second call OrderAttack one time

 endmethod
endstruct

5-Player

 JASS:

struct PLAYER
 integer Id = - 1
 unit Hero = null
 timer ReviveTimer = null
 timerdialog ReviveDialog = null

endstruct
struct PLAYER_ACTION
 static method PlayerReviveHero takes nothing returns nothing
 local integer n = 0
 local location loc = null
 loop
 exitwhen n > bj_MAX_PLAYERS
 if GAME.PLAYERS [ n ].ReviveTimer == GetExpiredTimer() then
 // Is timer of player now
 set loc = GetRandomLocInRect(udg_TheRevive)
 call ReviveHero(GAME.PLAYERS [ n ].Hero, GetLocationX(loc), GetLocationY(loc), false)
 call RemoveLocation(loc)
 call DestroyTimerDialog(GAME.PLAYERS [ n ].ReviveDialog) // Destroy timerwindow when timer excuted
 endif
 set n = n + 1
 endloop
 call DestroyTimer(GetExpiredTimer())
 endmethod
 static method Revive takes integer pid returns nothing
 local real time = 0
 set time = GetHeroLevel(GAME.PLAYERS [ pid ].Hero) + 10
 set GAME.PLAYERS [ pid ].ReviveTimer = CreateTimer()
 call TimerStart(GAME.PLAYERS [ pid ].ReviveTimer, 10, false, function thistype.PlayerReviveHero)
 set GAME.PLAYERS [ pid ].ReviveDialog = CreateTimerDialogBJ(GAME.PLAYERS [ pid ].ReviveTimer, GetPlayerName(Player(pid)))
 call TimerDialogDisplayBJ(true, GAME.PLAYERS [ pid ].ReviveDialog)
 endmethod
endstruct

99-Game

 JASS:

// For about struct, method , onInit read document here: https://www.hiveworkshop.com/threads/introduction-to-struct.207381/
struct GAME
 static PLAYER array PLAYERS [30]
 // I will put all Logic Game in GAME struct
 // private static method SpellCondition takes nothing returns boolean
 // //GetSpellAbilityId() is get id of ability in event of trigger you called.
 // if GetSpellAbilityId() == 'A000' then // 'A000' is ID of spell Pick Hero in wisp unit, you can check ID in F6 (Object manager) press Ctrl + D, it will show up
 // return true
 // endif
 // return false
 // endmethod
 static integer WISP = 'e000'
 static method IsPlayerOnline takes player p returns boolean
 return GetPlayerSlotState(p) == PLAYER_SLOT_STATE_PLAYING and GetPlayerController(p) == MAP_CONTROL_USER
 endmethod
 private static method SetupGame takes nothing returns nothing
 local trigger CastSpell = CreateTrigger() // Create a trigger
 local trigger UnitDeath = CreateTrigger() // Create a trigger
 local integer n = 0
 local location loc = null
 call TriggerRegisterAnyUnitEventBJ(CastSpell, EVENT_PLAYER_UNIT_SPELL_EFFECT) // Register event for trigger
 // call TriggerAddCondition( CastSpell, Condition( function thistype.SpellCondition ) ) // This is condition of CastSpell Trigger, if return value is true then if trigger have Action, trigger will call the action
 call TriggerAddAction(CastSpell, function SPELL.SpellEffect) // Action will call here
 //Event unit death
 call TriggerRegisterAnyUnitEventBJ(UnitDeath, EVENT_PLAYER_UNIT_DEATH)
 call TriggerAddAction(UnitDeath, function UNIT_DEATH.Checking)

 //Setup vision (Player(0) is Player 1. 10 is Player 11)
 call CreateFogModifierRectBJ(true, Player(0), FOG_OF_WAR_VISIBLE, gg_rct_Battle) // CreateFogModifierRectBJ Used for displaying the map in a specified region with the selected player
 call CreateFogModifierRectBJ(true, Player(10), FOG_OF_WAR_VISIBLE, gg_rct_Battle) //Player 10 is player enemy

 // Setup Enemy
 call SetPlayerAllianceStateBJ(Player(10), Player(GetPlayerNeutralPassive()), bj_ALLIANCE_UNALLIED_VISION) // SetPlayerAllianceStateBJ Used to reset the alliance status between two players, bj_ALLIANCE_UNALLIED_VISION makes both sides enemies and has vision of each other, including invisibility.
 call SetPlayerAllianceStateBJ(Player(GetPlayerNeutralPassive()), Player(10), bj_ALLIANCE_UNALLIED_VISION)
 //PanCameraToTimedLocForPlayer: player whichPlayer, location loc, real duration
 // GetRectCenter rect whichRect
 call PanCameraToTimedLocForPlayer(Player(0), GetRectCenter(udg_PickHeroArea), 0)
 //Setup bounty when unit die for player 11
 call SetPlayerFlagBJ(PLAYER_STATE_GIVES_BOUNTY, true, Player(10))
 //Destroy timer
 call DestroyTimer(GetExpiredTimer()) // This for destroy timer call one-time
 set n = 0
 loop
 exitwhen n > bj_MAX_PLAYERS
 if(.IsPlayerOnline(Player(n))) then
 set loc = GetRandomLocInRect(udg_PickHeroArea)
 call CreateUnit(Player(n),.WISP, GetLocationX(loc), GetLocationY(loc), 0)
 call RemoveLocation(loc)
 set PLAYERS[n] = PLAYER.create()
 set PLAYERS[n].Id = n
 set n = n + 1
 endif
 endloop
 endmethod

 private static method onInit takes nothing returns nothing
 //It is start, you can setup here but i dont want do it here, make a timer setup all in 0.1s after
 call TimerStart(CreateTimer(), 0.1, false, function thistype.SetupGame) // if false it call once time
 endmethod
endstruct

 JASS:

struct DropItem
 static real x
 static real y
 static boolean b
 static real rate
 static itempool ip
 static method DropAllAction takes nothing returns nothing
 local item i = GetEnumItem()
 //Test drop
 call CreateItem(GetItemTypeId(i), .x, .y)
 endmethod
 static method DropAll takes rect r , unit dyingunit returns nothing
 set .x = GetUnitX(dyingunit)
 set .y = GetUnitY(dyingunit)
 call EnumItemsInRectBJ( r, function thistype.DropAllAction )
 endmethod
 static method DropByChanceAction takes nothing returns nothing
 local item i = GetEnumItem()
 //Test drop
 if GetRandomReal(0, 100) <= LoadReal(udg_DropHastable, GetHandleId(i), StringHashBJ("drop")) then
 call CreateItem(GetItemTypeId(i), .x, .y)
 endif
 endmethod
 static method DropByChance takes rect r , unit dyingunit returns nothing
 set .x = GetUnitX(dyingunit)
 set .y = GetUnitY(dyingunit)
 call EnumItemsInRectBJ( r, function thistype.DropByChanceAction )
 endmethod
 static method DropOneAction takes nothing returns nothing
 local item i = GetEnumItem()
 if GetRandomReal(0, 100) <= LoadReal(udg_DropHastable, GetHandleId(i), StringHashBJ("drop")) then
 call ItemPoolAddItemType(.ip, GetItemTypeId(i), 1)
 endif
 endmethod
 static method DropOne takes rect r , unit dyingunit returns nothing
 // If have rate then item can drop or not
 set .b = false
 set .ip = CreateItemPool()
 set .x = GetUnitX(dyingunit)
 set .y = GetUnitY(dyingunit)
 call EnumItemsInRectBJ( r, function thistype.DropOneAction )
 call PlaceRandomItem(.ip, .x, .y)
 call DestroyItemPool(.ip)
 endmethod
endstruct

#### Attachments

MakeHeroDefense#4.w3x

 308 KB

 · Views: 62




 Last edited: Sep 24, 2024

#### Chaosy

#### Chaosy

Tutorial Reviewer

 Level 41

JoinedJun 9, 2011Messages13,295

Resources: 42

#### Resources
Icons 3Maps 1Spells and Systems 22Ported Icons 5Tutorials 11

Videos are always tricky because we don't really have real standards / rules for them.

I see a lot of room for improvement, I think a lot of parts can be edited to be either cut out entirely or sped up. The notepad could be replaced with edited text which is more efficient.

However as I said we're been very lenient in the past as to not discourage people so I don't really see that as an issue.

I do however think a demo map would be helpful.

#### SunLord

#### SunLord

 Level 8

JoinedNov 5, 2012Messages79

Resources: 9

#### Resources
Maps 5Spells and Systems 3Tutorials 1

##
 Ogre X Human


Chaosy said:

 Videos are always tricky because we don't really have real standards / rules for them.

I see a lot of room for improvement, I think a lot of parts can be edited to be either cut out entirely or sped up. The notepad could be replaced with edited text which is more efficient.

However as I said we're been very lenient in the past as to not discourage people so I don't really see that as an issue.

I do however think a demo map would be helpful.


Click to expand...

Thank you, I actually forgot to send in the demo map.

#### SunLord

#### SunLord

 Level 8

JoinedNov 5, 2012Messages79

Resources: 9

#### Resources
Maps 5Spells and Systems 3Tutorials 1

##
 Ogre X Human


Yeah, in future I will make video shorter videos serve more introductory content, someone have idea need have in a map hero defense? Comment, I will make it. Yeah, now I'm make a TD map. In future have new tutorial!

 You must log in or register to reply here.


### Similar threads

Locked

[Defense / Survival]
 Ham's Hero Defense


Ulfsire

Aug 28, 2018

Map Development

Replies6Views3K

Oct 18, 2018
Ulfsire

S

Locked

 Hero Defense Trigger (ask help)


sc4n

Jun 26, 2008

World Editor Help Zone

Replies6Views1K

Jun 26, 2008
terradont

X

Locked

[Defense / Survival]
 Solo Defense


xy4kuz4x

Jul 11, 2014

Idea Factory

Replies3Views1K

Jul 14, 2014
noob

Locked

 Defense of Hearthglen 1.12


matej

Dec 1, 2009

Map Development

Replies0Views2K

Dec 1, 2009
matej

Locked

 Hero defense trigger event


DemonHunterGI

Mar 12, 2018

Requests

Replies5Views1K

Mar 31, 2018
DemonHunterGI

Share:
BlueskyLinkedInRedditPinterestTumblrWhatsAppEmailShareLink

Home

Forums

Warcraft III

Warcraft 3 Tutorials

Scripting Tutorials

JASS Tutorials

 This site uses cookies to help personalise content, tailor your experience and to keep you logged in if you register.

By continuing to use this site, you are consenting to our use of cookies.


AcceptLearn more…

Back

Top