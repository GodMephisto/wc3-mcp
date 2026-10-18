---
type: vendor-doc
source_repo: "https://github.com/nazarpunk/wc3-ujapi"
source_path: "sdk/common.j.lua"
ref: HEAD
---

-- ============================================================================
-- Native types. All native functions take extended handle types when
-- possible to help prevent passing bad values to native functions
-- 
---@class agent:handle @all reference counted objects
---@class event:agent @a reference to an event registration
---@class player:agent @a single player reference
---@class war3image:agent @UjAPI an interactive game object that serves as a base to nearly every model-based object.
---@class widget:war3image @an interactive game object with life
---@class unit:widget @a single unit reference
---@class destructable:widget
---@class item:widget
---@class ability:agent
---@class buff:ability
---@class force:agent
---@class group:agent
---@class trigger:agent
---@class triggercondition:agent
---@class triggeraction:agent
---@class timer:agent
---@class location:agent
---@class region:agent
---@class rect:agent
---@class boolexpr:agent
---@class sound:agent
---@class conditionfunc:boolexpr
---@class filterfunc:boolexpr
---@class unitpool:handle
---@class itempool:handle
---@class race:handle
---@class alliancetype:handle
---@class gamestate:handle
---@class igamestate:gamestate
---@class fgamestate:gamestate
---@class playerstate:handle
---@class playerscore:handle
---@class playergameresult:handle
---@class unitstate:handle
---@class aidifficulty:handle
---@class eventid:handle
---@class gameevent:eventid
---@class playerevent:eventid
---@class playerunitevent:eventid
---@class unitevent:eventid
---@class limitop:eventid
---@class widgetevent:eventid
---@class dialogevent:eventid
---@class unittype:handle
---@class projectiletype:handle @UjAPI
---@class gamespeed:handle
---@class gamedifficulty:handle
---@class mapvisibility:handle
---@class mapsetting:handle
---@class mapdensity:handle
---@class mapcontrol:handle
---@class minimapicon:handle @UjAPI
---@class playerslotstate:handle
---@class volumegroup:handle
---@class camerafield:handle
---@class camerasetup:handle
---@class playercolor:handle
---@class placement:handle
---@class startlocprio:handle
---@class raritycontrol:handle
---@class blendmode:handle
---@class texmapflags:handle
---@class effect:war3image
---@class effecttype:handle
---@class weathereffect:handle
---@class terraindeformation:handle
---@class fogstate:handle
---@class fogmodifier:agent
---@class dialog:agent
---@class button:agent
---@class quest:agent
---@class questitem:agent
---@class defeatcondition:agent
---@class timerdialog:agent
---@class leaderboard:agent
---@class multiboard:agent
---@class multiboarditem:agent
---@class trackable:war3image
---@class gamecache:agent
---@class version:handle
---@class itemtype:handle
---@class texttag:handle
---@class attacktype:handle
---@class damagetype:handle
---@class weapontype:handle
---@class soundtype:handle
---@class lightning:handle
---@class pathingtype:handle
---@class mappedfield:handle @UjAPI
---@class mappedtype:handle @UjAPI
---@class attachmenttype:mappedtype @UjAPI
---@class bonetype:attachmenttype @UjAPI
---@class animtype:mappedtype @UjAPI
---@class subanimtype:animtype @UjAPI
---@class cursoranimtype:mappedtype @UjAPI
---@class image:handle
---@class ubersplat:handle
---@class hashtable:agent
---@class sprite:war3image @UjAPI
---@class projectile:war3image @UjAPI
---@class doodad:war3image @UjAPI
---@class framehandle:agent @UjAPI
---@class commandbuttoneffect:handle @UjAPI
---@class originframetype:handle @UjAPI
---@class framepointtype:handle @UjAPI
---@class textaligntype:handle @UjAPI
---@class frameeventtype:handle @UjAPI
---@class oskeytype:handle @UjAPI
---@class mousebuttontype:handle @UjAPI
---@class agentdatafield:handle @UjAPI
---@class abilityintegerfield:agentdatafield @UjAPI
---@class abilityrealfield:agentdatafield @UjAPI
---@class abilitybooleanfield:agentdatafield @UjAPI
---@class abilitystringfield:agentdatafield @UjAPI
---@class abilityintegerlevelfield:abilityintegerfield @UjAPI
---@class abilityreallevelfield:abilityrealfield @UjAPI
---@class abilitybooleanlevelfield:abilitybooleanfield @UjAPI
---@class abilitystringlevelfield:abilitystringfield @UjAPI
---@class abilityintegerlevelarrayfield:abilityintegerlevelfield @UjAPI
---@class abilityreallevelarrayfield:abilityreallevelfield @UjAPI
---@class abilitybooleanlevelarrayfield:abilitybooleanlevelfield @UjAPI
---@class abilitystringlevelarrayfield:abilitystringlevelfield @UjAPI
---@class destructablestringfield:agentdatafield @UjAPI
---@class itemintegerfield:agentdatafield @UjAPI
---@class itemrealfield:agentdatafield @UjAPI
---@class itembooleanfield:agentdatafield @UjAPI
---@class itemstringfield:agentdatafield @UjAPI
---@class unitintegerfield:agentdatafield @UjAPI
---@class unitrealfield:agentdatafield @UjAPI
---@class unitbooleanfield:agentdatafield @UjAPI
---@class unitstringfield:agentdatafield @UjAPI
---@class unitweaponintegerfield:agentdatafield @UjAPI
---@class unitweaponrealfield:agentdatafield @UjAPI
---@class unitweaponbooleanfield:agentdatafield @UjAPI
---@class unitweaponstringfield:agentdatafield @UjAPI
---@class flagtype:handle @UjAPI
---@class racepreference:flagtype
---@class gametype:flagtype
---@class mapflag:flagtype
---@class movetype:flagtype @UjAPI
---@class pathingaitype:flagtype @UjAPI
---@class collisiontype:flagtype @UjAPI
---@class targetflag:flagtype @UjAPI
---@class unitcategory:flagtype @UjAPI
---@class pathingflag:flagtype @UjAPI
---@class layoutstyleflag:flagtype @UjAPI
---@class gridstyleflag:flagtype @UjAPI
---@class layerstyleflag:flagtype @UjAPI
---@class controlstyleflag:flagtype @UjAPI
---@class framestate:flagtype @UjAPI
---@class abilitytype:flagtype @UjAPI
---@class armortype:handle @UjAPI
---@class heroattribute:handle @UjAPI
---@class defensetype:handle @UjAPI
---@class regentype:handle @UjAPI
---@class timetype:handle @UjAPI
---@class variabletype:handle @UjAPI
---@class renderstage:handle @UjAPI
---@class connectiontype:handle @UjAPI
---@class jassthread:handle @UjAPI
---@class handlelist:agent @UjAPI
---@class textfilehandle:agent @UjAPI
---@class orderhandle:agent @UjAPI
---@class tradestate:handle @UjAPI

---@param i integer
---@return race
function ConvertRace (i) end
---@param i integer
---@return alliancetype
function ConvertAllianceType (i) end
---@param i integer
---@return racepreference
function ConvertRacePref (i) end
---@param i integer
---@return igamestate
function ConvertIGameState (i) end
---@param i integer
---@return fgamestate
function ConvertFGameState (i) end
---@param i integer
---@return playerstate
function ConvertPlayerState (i) end
---@param i integer
---@return playerscore
function ConvertPlayerScore (i) end
---@param i integer
---@return playergameresult
function ConvertPlayerGameResult (i) end
---@param i integer
---@return unitstate
function ConvertUnitState (i) end
---@param i integer
---@return aidifficulty
function ConvertAIDifficulty (i) end
---@param i integer
---@return gameevent
function ConvertGameEvent (i) end
---@param i integer
---@return playerevent
function ConvertPlayerEvent (i) end
---@param i integer
---@return playerunitevent
function ConvertPlayerUnitEvent (i) end
---@param i integer
---@return widgetevent
function ConvertWidgetEvent (i) end
---@param i integer
---@return dialogevent
function ConvertDialogEvent (i) end
---@param i integer
---@return unitevent
function ConvertUnitEvent (i) end
---@param i integer
---@return limitop
function ConvertLimitOp (i) end
---@param i integer
---@return unittype
function ConvertUnitType (i) end
---@param i integer
---@return gamespeed
function ConvertGameSpeed (i) end
---@param i integer
---@return placement
function ConvertPlacement (i) end
---@param i integer
---@return startlocprio
function ConvertStartLocPrio (i) end
---@param i integer
---@return gamedifficulty
function ConvertGameDifficulty (i) end
---@param i integer
---@return gametype
function ConvertGameType (i) end
---@param i integer
---@return mapflag
function ConvertMapFlag (i) end
---@param i integer
---@return mapvisibility
function ConvertMapVisibility (i) end
---@param i integer
---@return mapsetting
function ConvertMapSetting (i) end
---@param i integer
---@return mapdensity
function ConvertMapDensity (i) end
---@param i integer
---@return mapcontrol
function ConvertMapControl (i) end
---@param i integer
---@return playercolor
function ConvertPlayerColor (i) end
---@param i integer
---@return playerslotstate
function ConvertPlayerSlotState (i) end
---@param i integer
---@return volumegroup
function ConvertVolumeGroup (i) end
---@param i integer
---@return camerafield
function ConvertCameraField (i) end
---@param i integer
---@return blendmode
function ConvertBlendMode (i) end
---@param i integer
---@return raritycontrol
function ConvertRarityControl (i) end
---@param i integer
---@return texmapflags
function ConvertTexMapFlags (i) end
---@param i integer
---@return fogstate
function ConvertFogState (i) end
---@param i integer
---@return effecttype
function ConvertEffectType (i) end
---@param i integer
---@return version
function ConvertVersion (i) end
---@param i integer
---@return itemtype
function ConvertItemType (i) end
---@param i integer
---@return attacktype
function ConvertAttackType (i) end
---@param i integer
---@return damagetype
function ConvertDamageType (i) end
---@param i integer
---@return weapontype
function ConvertWeaponType (i) end
---@param i integer
---@return soundtype
function ConvertSoundType (i) end
---@param i integer
---@return pathingtype
function ConvertPathingType (i) end
---@author UjAPI
---@param i integer
---@return projectiletype
function ConvertProjectileType (i) end
---@author UjAPI
---@param i integer
---@return mappedfield
function ConvertMappedField (i) end
---@author UjAPI
---@param i integer
---@return attachmenttype
function ConvertAttachmentType (i) end
---@author UjAPI
---@param i integer
---@return bonetype
function ConvertBoneType (i) end
---@author UjAPI
---@param i integer
---@return animtype
function ConvertAnimType (i) end
---@author UjAPI
---@param i integer
---@return subanimtype
function ConvertSubAnimType (i) end
---@author UjAPI
---@param i integer
---@return cursoranimtype
function ConvertCursorAnimType (i) end
---@author UjAPI
---@param i integer
---@return originframetype
function ConvertOriginFrameType (i) end
---@author UjAPI
---@param i integer
---@return framepointtype
function ConvertFramePointType (i) end
---@author UjAPI
---@param i integer
---@return textaligntype
function ConvertTextAlignType (i) end
---@author UjAPI
---@param i integer
---@return frameeventtype
function ConvertFrameEventType (i) end
---@author UjAPI
---@param i integer
---@return oskeytype
function ConvertOsKeyType (i) end
---@author UjAPI
---@param i integer
---@return mousebuttontype
function ConvertMouseButtonType (i) end
---@author UjAPI
---@param i integer
---@return abilityintegerfield
function ConvertAbilityIntegerField (i) end
---@author UjAPI
---@param i integer
---@return abilityrealfield
function ConvertAbilityRealField (i) end
---@author UjAPI
---@param i integer
---@return abilitybooleanfield
function ConvertAbilityBooleanField (i) end
---@author UjAPI
---@param i integer
---@return abilitystringfield
function ConvertAbilityStringField (i) end
---@author UjAPI
---@param i integer
---@return abilityintegerlevelfield
function ConvertAbilityIntegerLevelField (i) end
---@author UjAPI
---@param i integer
---@return abilityreallevelfield
function ConvertAbilityRealLevelField (i) end
---@author UjAPI
---@param i integer
---@return abilitybooleanlevelfield
function ConvertAbilityBooleanLevelField (i) end
---@author UjAPI
---@param i integer
---@return abilitystringlevelfield
function ConvertAbilityStringLevelField (i) end
---@author UjAPI
---@param i integer
---@return abilityintegerlevelarrayfield
function ConvertAbilityIntegerLevelArrayField (i) end
---@author UjAPI
---@param i integer
---@return abilityreallevelarrayfield
function ConvertAbilityRealLevelArrayField (i) end
---@author UjAPI
---@param i integer
---@return abilitybooleanlevelarrayfield
function ConvertAbilityBooleanLevelArrayField (i) end
---@author UjAPI
---@param i integer
---@return abilitystringlevelarrayfield
function ConvertAbilityStringLevelArrayField (i) end
---@author UjAPI
---@param i integer
---@return destructablestringfield
function ConvertDestructableStringField (i) end
---@author UjAPI
---@param i integer
---@return itemintegerfield
function ConvertItemIntegerField (i) end
---@author UjAPI
---@param i integer
---@return itemrealfield
function ConvertItemRealField (i) end
---@author UjAPI
---@param i integer
---@return itembooleanfield
function ConvertItemBooleanField (i) end
---@author UjAPI
---@param i integer
---@return itemstringfield
function ConvertItemStringField (i) end
---@author UjAPI
---@param i integer
---@return unitintegerfield
function ConvertUnitIntegerField (i) end
---@author UjAPI
---@param i integer
---@return unitrealfield
function ConvertUnitRealField (i) end
---@author UjAPI
---@param i integer
---@return unitbooleanfield
function ConvertUnitBooleanField (i) end
---@author UjAPI
---@param i integer
---@return unitstringfield
function ConvertUnitStringField (i) end
---@author UjAPI
---@param i integer
---@return unitweaponintegerfield
function ConvertUnitWeaponIntegerField (i) end
---@author UjAPI
---@param i integer
---@return unitweaponrealfield
function ConvertUnitWeaponRealField (i) end
---@author UjAPI
---@param i integer
---@return unitweaponbooleanfield
function ConvertUnitWeaponBooleanField (i) end
---@author UjAPI
---@param i integer
---@return unitweaponstringfield
function ConvertUnitWeaponStringField (i) end
---@author UjAPI
---@param i integer
---@return movetype
function ConvertMoveType (i) end
---@author UjAPI
---@param i integer
---@return pathingaitype
function ConvertPathingAIType (i) end
---@author UjAPI
---@param i integer
---@return collisiontype
function ConvertCollisionType (i) end
---@author UjAPI
---@param i integer
---@return targetflag
function ConvertTargetFlag (i) end
---@author UjAPI
---@param i integer
---@return armortype
function ConvertArmorType (i) end
---@author UjAPI
---@param i integer
---@return heroattribute
function ConvertHeroAttribute (i) end
---@author UjAPI
---@param i integer
---@return defensetype
function ConvertDefenseType (i) end
---@author UjAPI
---@param i integer
---@return regentype
function ConvertRegenType (i) end
---@author UjAPI
---@param i integer
---@return unitcategory
function ConvertUnitCategory (i) end
---@author UjAPI
---@param i integer
---@return pathingflag
function ConvertPathingFlag (i) end
---@author UjAPI
---@param i integer
---@return timetype
function ConvertTimeType (i) end
---@author UjAPI
---@param i integer
---@return variabletype
function ConvertVariableType (i) end
---@author UjAPI
---@param i integer
---@return renderstage
function ConvertRenderStage (i) end
---@author UjAPI
---@param i integer
---@return layoutstyleflag
function ConvertLayoutStyleFlag (i) end
---@author UjAPI
---@param i integer
---@return gridstyleflag
function ConvertGridStyleFlag (i) end
---@author UjAPI
---@param i integer
---@return layerstyleflag
function ConvertLayerStyleFlag (i) end
---@author UjAPI
---@param i integer
---@return controlstyleflag
function ConvertControlStyleFlag (i) end
---@author UjAPI
---@param i integer
---@return framestate
function ConvertFrameState (i) end
---@author UjAPI
---@param i integer
---@return abilitytype
function ConvertAbilityType (i) end
---@author UjAPI
---@param i integer
---@return connectiontype
function ConvertConnectionType (i) end
---@author UjAPI
---@param i integer
---@return tradestate
function ConvertTradeState (i) end

---@param orderNameString string
---@return integer
function OrderId (orderNameString) end
---@param orderId integer
---@return string
function OrderId2String (orderId) end
---@param unitTypeIdString string
---@return integer
function UnitId (unitTypeIdString) end
---@param unitTypeId integer
---@return string
function UnitId2String (unitTypeId) end

-- Not currently working correctly...
---@param abilityIdString string
---@return integer
function AbilityId (abilityIdString) end
---@param abilityTypeId integer
---@return string
function AbilityId2String (abilityTypeId) end

-- Looks up the "name" field for any object (unit, item, ability)
---@param objectTypeId integer
---@return string
function GetObjectName (objectTypeId) end

---@return integer
function GetBJMaxPlayers () end
---@return integer
function GetBJPlayerNeutralVictim () end
---@return integer
function GetBJPlayerNeutralExtra () end
---@return integer
function GetBJMaxPlayerSlots () end
---@return integer
function GetPlayerNeutralPassive () end
---@return integer
function GetPlayerNeutralAggressive () end

---@author UjAPI
---@return integer
function GetJassArrayLimit () end
---@author UjAPI
---@return integer
function GetTextTagLimit () end

-- ===================================================
-- Game Constants
-- ===================================================

FALSE = false ---@type boolean
TRUE = true ---@type boolean
JASS_MAX_ARRAY_SIZE = GetJassArrayLimit() ---@type integer @UjAPI
TEXT_TAG_MAX_SIZE = GetTextTagLimit() ---@type integer @UjAPI

PLAYER_NEUTRAL_PASSIVE = GetPlayerNeutralPassive() ---@type integer
PLAYER_NEUTRAL_AGGRESSIVE = GetPlayerNeutralAggressive() ---@type integer

PLAYER_COLOR_RED = ConvertPlayerColor(0) ---@type playercolor
PLAYER_COLOR_BLUE = ConvertPlayerColor(1) ---@type playercolor
PLAYER_COLOR_CYAN = ConvertPlayerColor(2) ---@type playercolor
PLAYER_COLOR_PURPLE = ConvertPlayerColor(3) ---@type playercolor
PLAYER_COLOR_YELLOW = ConvertPlayerColor(4) ---@type playercolor
PLAYER_COLOR_ORANGE = ConvertPlayerColor(5) ---@type playercolor
PLAYER_COLOR_GREEN = ConvertPlayerColor(6) ---@type playercolor
PLAYER_COLOR_PINK = ConvertPlayerColor(7) ---@type playercolor
PLAYER_COLOR_LIGHT_GRAY = ConvertPlayerColor(8) ---@type playercolor
PLAYER_COLOR_LIGHT_BLUE = ConvertPlayerColor(9) ---@type playercolor
PLAYER_COLOR_AQUA = ConvertPlayerColor(10) ---@type playercolor
PLAYER_COLOR_BROWN = ConvertPlayerColor(11) ---@type playercolor
PLAYER_COLOR_MAROON = ConvertPlayerColor(12) ---@type playercolor
PLAYER_COLOR_NAVY = ConvertPlayerColor(13) ---@type playercolor
PLAYER_COLOR_TURQUOISE = ConvertPlayerColor(14) ---@type playercolor
PLAYER_COLOR_VIOLET = ConvertPlayerColor(15) ---@type playercolor
PLAYER_COLOR_WHEAT = ConvertPlayerColor(16) ---@type playercolor
PLAYER_COLOR_PEACH = ConvertPlayerColor(17) ---@type playercolor
PLAYER_COLOR_MINT = ConvertPlayerColor(18) ---@type playercolor
PLAYER_COLOR_LAVENDER = ConvertPlayerColor(19) ---@type playercolor
PLAYER_COLOR_COAL = ConvertPlayerColor(20) ---@type playercolor
PLAYER_COLOR_SNOW = ConvertPlayerColor(21) ---@type playercolor
PLAYER_COLOR_EMERALD = ConvertPlayerColor(22) ---@type playercolor
PLAYER_COLOR_PEANUT = ConvertPlayerColor(23) ---@type playercolor

RACE_HUMAN = ConvertRace(1) ---@type race
RACE_ORC = ConvertRace(2) ---@type race
RACE_UNDEAD = ConvertRace(3) ---@type race
RACE_NIGHTELF = ConvertRace(4) ---@type race
RACE_DEMON = ConvertRace(5) ---@type race
RACE_OTHER = ConvertRace(7) ---@type race

PLAYER_GAME_RESULT_VICTORY = ConvertPlayerGameResult(0) ---@type playergameresult
PLAYER_GAME_RESULT_DEFEAT = ConvertPlayerGameResult(1) ---@type playergameresult
PLAYER_GAME_RESULT_TIE = ConvertPlayerGameResult(2) ---@type playergameresult
PLAYER_GAME_RESULT_NEUTRAL = ConvertPlayerGameResult(3) ---@type playergameresult

ALLIANCE_PASSIVE = ConvertAllianceType(0) ---@type alliancetype
ALLIANCE_HELP_REQUEST = ConvertAllianceType(1) ---@type alliancetype
ALLIANCE_HELP_RESPONSE = ConvertAllianceType(2) ---@type alliancetype
ALLIANCE_SHARED_XP = ConvertAllianceType(3) ---@type alliancetype
ALLIANCE_SHARED_SPELLS = ConvertAllianceType(4) ---@type alliancetype
ALLIANCE_SHARED_VISION = ConvertAllianceType(5) ---@type alliancetype
ALLIANCE_SHARED_CONTROL = ConvertAllianceType(6) ---@type alliancetype
ALLIANCE_SHARED_ADVANCED_CONTROL = ConvertAllianceType(7) ---@type alliancetype
ALLIANCE_RESCUABLE = ConvertAllianceType(8) ---@type alliancetype
ALLIANCE_SHARED_VISION_FORCED = ConvertAllianceType(9) ---@type alliancetype

VERSION_REIGN_OF_CHAOS = ConvertVersion(0) ---@type version
VERSION_FROZEN_THRONE = ConvertVersion(1) ---@type version

ATTACK_TYPE_NORMAL = ConvertAttackType(0) ---@type attacktype
ATTACK_TYPE_MELEE = ConvertAttackType(1) ---@type attacktype
ATTACK_TYPE_PIERCE = ConvertAttackType(2) ---@type attacktype
ATTACK_TYPE_SIEGE = ConvertAttackType(3) ---@type attacktype
ATTACK_TYPE_MAGIC = ConvertAttackType(4) ---@type attacktype
ATTACK_TYPE_CHAOS = ConvertAttackType(5) ---@type attacktype
ATTACK_TYPE_HERO = ConvertAttackType(6) ---@type attacktype

DAMAGE_TYPE_UNKNOWN = ConvertDamageType(0) ---@type damagetype
DAMAGE_TYPE_NORMAL = ConvertDamageType(4) ---@type damagetype
DAMAGE_TYPE_ENHANCED = ConvertDamageType(5) ---@type damagetype
DAMAGE_TYPE_FIRE = ConvertDamageType(8) ---@type damagetype
DAMAGE_TYPE_COLD = ConvertDamageType(9) ---@type damagetype
DAMAGE_TYPE_LIGHTNING = ConvertDamageType(10) ---@type damagetype
DAMAGE_TYPE_POISON = ConvertDamageType(11) ---@type damagetype
DAMAGE_TYPE_DISEASE = ConvertDamageType(12) ---@type damagetype
DAMAGE_TYPE_DIVINE = ConvertDamageType(13) ---@type damagetype
DAMAGE_TYPE_MAGIC = ConvertDamageType(14) ---@type damagetype
DAMAGE_TYPE_SONIC = ConvertDamageType(15) ---@type damagetype
DAMAGE_TYPE_ACID = ConvertDamageType(16) ---@type damagetype
DAMAGE_TYPE_FORCE = ConvertDamageType(17) ---@type damagetype
DAMAGE_TYPE_DEATH = ConvertDamageType(18) ---@type damagetype
DAMAGE_TYPE_MIND = ConvertDamageType(19) ---@type damagetype
DAMAGE_TYPE_PLANT = ConvertDamageType(20) ---@type damagetype
DAMAGE_TYPE_DEFENSIVE = ConvertDamageType(21) ---@type damagetype
DAMAGE_TYPE_DEMOLITION = ConvertDamageType(22) ---@type damagetype
DAMAGE_TYPE_SLOW_POISON = ConvertDamageType(23) ---@type damagetype
DAMAGE_TYPE_SPIRIT_LINK = ConvertDamageType(24) ---@type damagetype
DAMAGE_TYPE_SHADOW_STRIKE = ConvertDamageType(25) ---@type damagetype
DAMAGE_TYPE_UNIVERSAL = ConvertDamageType(26) ---@type damagetype

WEAPON_TYPE_WHOKNOWS = ConvertWeaponType(0) ---@type weapontype
WEAPON_TYPE_METAL_LIGHT_CHOP = ConvertWeaponType(1) ---@type weapontype
WEAPON_TYPE_METAL_MEDIUM_CHOP = ConvertWeaponType(2) ---@type weapontype
WEAPON_TYPE_METAL_HEAVY_CHOP = ConvertWeaponType(3) ---@type weapontype
WEAPON_TYPE_METAL_LIGHT_SLICE = ConvertWeaponType(4) ---@type weapontype
WEAPON_TYPE_METAL_MEDIUM_SLICE = ConvertWeaponType(5) ---@type weapontype
WEAPON_TYPE_METAL_HEAVY_SLICE = ConvertWeaponType(6) ---@type weapontype
WEAPON_TYPE_METAL_MEDIUM_BASH = ConvertWeaponType(7) ---@type weapontype
WEAPON_TYPE_METAL_HEAVY_BASH = ConvertWeaponType(8) ---@type weapontype
WEAPON_TYPE_METAL_MEDIUM_STAB = ConvertWeaponType(9) ---@type weapontype
WEAPON_TYPE_METAL_HEAVY_STAB = ConvertWeaponType(10) ---@type weapontype
WEAPON_TYPE_WOOD_LIGHT_SLICE = ConvertWeaponType(11) ---@type weapontype
WEAPON_TYPE_WOOD_MEDIUM_SLICE = ConvertWeaponType(12) ---@type weapontype
WEAPON_TYPE_WOOD_HEAVY_SLICE = ConvertWeaponType(13) ---@type weapontype
WEAPON_TYPE_WOOD_LIGHT_BASH = ConvertWeaponType(14) ---@type weapontype
WEAPON_TYPE_WOOD_MEDIUM_BASH = ConvertWeaponType(15) ---@type weapontype
WEAPON_TYPE_WOOD_HEAVY_BASH = ConvertWeaponType(16) ---@type weapontype
WEAPON_TYPE_WOOD_LIGHT_STAB = ConvertWeaponType(17) ---@type weapontype
WEAPON_TYPE_WOOD_MEDIUM_STAB = ConvertWeaponType(18) ---@type weapontype
WEAPON_TYPE_CLAW_LIGHT_SLICE = ConvertWeaponType(19) ---@type weapontype
WEAPON_TYPE_CLAW_MEDIUM_SLICE = ConvertWeaponType(20) ---@type weapontype
WEAPON_TYPE_CLAW_HEAVY_SLICE = ConvertWeaponType(21) ---@type weapontype
WEAPON_TYPE_AXE_MEDIUM_CHOP = ConvertWeaponType(22) ---@type weapontype
WEAPON_TYPE_ROCK_HEAVY_BASH = ConvertWeaponType(23) ---@type weapontype

PATHING_TYPE_ANY = ConvertPathingType(0) ---@type pathingtype
PATHING_TYPE_WALKABILITY = ConvertPathingType(1) ---@type pathingtype
PATHING_TYPE_FLYABILITY = ConvertPathingType(2) ---@type pathingtype
PATHING_TYPE_BUILDABILITY = ConvertPathingType(3) ---@type pathingtype
PATHING_TYPE_PEONHARVESTPATHING = ConvertPathingType(4) ---@type pathingtype
PATHING_TYPE_BLIGHTPATHING = ConvertPathingType(5) ---@type pathingtype
PATHING_TYPE_FLOATABILITY = ConvertPathingType(6) ---@type pathingtype
PATHING_TYPE_AMPHIBIOUSPATHING = ConvertPathingType(7) ---@type pathingtype

MAPPED_FIELD_ATTACHMENT = ConvertMappedField(0) ---@type mappedfield @UjAPI
MAPPED_FIELD_BONE = ConvertMappedField(1) ---@type mappedfield @UjAPI
MAPPED_FIELD_ANIMATION = ConvertMappedField(2) ---@type mappedfield @UjAPI
MAPPED_FIELD_SUB_ANIMATION = ConvertMappedField(3) ---@type mappedfield @UjAPI
MAPPED_CURSOR_SUB_ANIMATION = ConvertMappedField(4) ---@type mappedfield @UjAPI

ATTACHMENT_TYPE_CHEST = ConvertAttachmentType(FourCC('ches'--[[1667786099--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_FEET = ConvertAttachmentType(FourCC('feet'--[[1717921140--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_FOOT = ConvertAttachmentType(FourCC('foot'--[[1718579060--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_HAND = ConvertAttachmentType(FourCC('hand'--[[1751215716--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_HEAD = ConvertAttachmentType(FourCC('head'--[[1751474532--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_ORIGIN = ConvertAttachmentType(FourCC('orig'--[[1869769063--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_OVERHEAD = ConvertAttachmentType(FourCC('over'--[[1870030194--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_SPRITE = ConvertAttachmentType(FourCC('spri'--[[1936749161--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_WEAPON = ConvertAttachmentType(FourCC('weap'--[[2003132784--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_ALTERNATE = ConvertAttachmentType(FourCC('alte'--[[1634497637--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_LEFT = ConvertAttachmentType(FourCC('left'--[[1818584692--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_RIGHT = ConvertAttachmentType(FourCC('righ'--[[1919510376--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_MOUNT = ConvertAttachmentType(FourCC('moun'--[[1836021102--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_REAR = ConvertAttachmentType(FourCC('rear'--[[1919246706--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_SMART = ConvertAttachmentType(FourCC('smar'--[[1936548210--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_FIRST = ConvertAttachmentType(FourCC('firs'--[[1718186611--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_SECOND = ConvertAttachmentType(FourCC('seco'--[[1936024431--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_THIRD = ConvertAttachmentType(FourCC('thir'--[[1952999794--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_FOURTH = ConvertAttachmentType(FourCC('four'--[[1718580594--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_FIFTH = ConvertAttachmentType(FourCC('fift'--[[1718183540--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_SIXTH = ConvertAttachmentType(FourCC('sixt'--[[1936291956--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_SMALL = ConvertAttachmentType(FourCC('smal'--[[1936548204--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_MEDIUM = ConvertAttachmentType(FourCC('medi'--[[1835361385--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_LARGE = ConvertAttachmentType(FourCC('larg'--[[1818325607--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_GOLD = ConvertAttachmentType(FourCC('gold'--[[1735355492--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_RALLYPOINT = ConvertAttachmentType(FourCC('rall'--[[1918987372--]])) ---@type attachmenttype @UjAPI
ATTACHMENT_TYPE_EAT_TREE = ConvertAttachmentType(FourCC('eatt'--[[1700885620--]])) ---@type attachmenttype @UjAPI

BONE_TYPE_CHEST = ConvertBoneType(FourCC('ches'--[[1667786099--]])) ---@type bonetype @UjAPI
BONE_TYPE_FOOT = ConvertBoneType(FourCC('foot'--[[1718579060--]])) ---@type bonetype @UjAPI
BONE_TYPE_HAND = ConvertBoneType(FourCC('hand'--[[1751215716--]])) ---@type bonetype @UjAPI
BONE_TYPE_HEAD = ConvertBoneType(FourCC('head'--[[1751474532--]])) ---@type bonetype @UjAPI
BONE_TYPE_TURRET = ConvertBoneType(FourCC('turr'--[[1953854066--]])) ---@type bonetype @UjAPI
BONE_TYPE_ALTERNATE = ConvertBoneType(FourCC('alte'--[[1634497637--]])) ---@type bonetype @UjAPI
BONE_TYPE_LEFT = ConvertBoneType(FourCC('left'--[[1818584692--]])) ---@type bonetype @UjAPI
BONE_TYPE_RIGHT = ConvertBoneType(FourCC('righ'--[[1919510376--]])) ---@type bonetype @UjAPI
BONE_TYPE_MOUNT = ConvertBoneType(FourCC('moun'--[[1836021102--]])) ---@type bonetype @UjAPI
BONE_TYPE_SMART = ConvertBoneType(FourCC('smar'--[[1936548210--]])) ---@type bonetype @UjAPI

ANIM_TYPE_BIRTH = ConvertAnimType(0) ---@type animtype @UjAPI
ANIM_TYPE_DEATH = ConvertAnimType(1) ---@type animtype @UjAPI
ANIM_TYPE_DECAY = ConvertAnimType(2) ---@type animtype @UjAPI
ANIM_TYPE_DISSIPATE = ConvertAnimType(3) ---@type animtype @UjAPI
ANIM_TYPE_STAND = ConvertAnimType(4) ---@type animtype @UjAPI
ANIM_TYPE_WALK = ConvertAnimType(5) ---@type animtype @UjAPI
ANIM_TYPE_ATTACK = ConvertAnimType(6) ---@type animtype @UjAPI
ANIM_TYPE_MORPH = ConvertAnimType(7) ---@type animtype @UjAPI
ANIM_TYPE_SLEEP = ConvertAnimType(8) ---@type animtype @UjAPI
ANIM_TYPE_SPELL = ConvertAnimType(9) ---@type animtype @UjAPI
ANIM_TYPE_PORTRAIT = ConvertAnimType(10) ---@type animtype @UjAPI

SUBANIM_TYPE_ROOTED = ConvertSubAnimType(11) ---@type subanimtype @UjAPI
SUBANIM_TYPE_ALTERNATE_EX = ConvertSubAnimType(12) ---@type subanimtype @UjAPI
SUBANIM_TYPE_LOOPING = ConvertSubAnimType(13) ---@type subanimtype @UjAPI
SUBANIM_TYPE_SLAM = ConvertSubAnimType(14) ---@type subanimtype @UjAPI
SUBANIM_TYPE_THROW = ConvertSubAnimType(15) ---@type subanimtype @UjAPI
SUBANIM_TYPE_SPIKED = ConvertSubAnimType(16) ---@type subanimtype @UjAPI
SUBANIM_TYPE_FAST = ConvertSubAnimType(17) ---@type subanimtype @UjAPI
SUBANIM_TYPE_SPIN = ConvertSubAnimType(18) ---@type subanimtype @UjAPI
SUBANIM_TYPE_READY = ConvertSubAnimType(19) ---@type subanimtype @UjAPI
SUBANIM_TYPE_CHANNEL = ConvertSubAnimType(20) ---@type subanimtype @UjAPI
SUBANIM_TYPE_DEFEND = ConvertSubAnimType(21) ---@type subanimtype @UjAPI
SUBANIM_TYPE_VICTORY = ConvertSubAnimType(22) ---@type subanimtype @UjAPI
SUBANIM_TYPE_TURN = ConvertSubAnimType(23) ---@type subanimtype @UjAPI
SUBANIM_TYPE_LEFT = ConvertSubAnimType(24) ---@type subanimtype @UjAPI
SUBANIM_TYPE_RIGHT = ConvertSubAnimType(25) ---@type subanimtype @UjAPI
SUBANIM_TYPE_FIRE = ConvertSubAnimType(26) ---@type subanimtype @UjAPI
SUBANIM_TYPE_FLESH = ConvertSubAnimType(27) ---@type subanimtype @UjAPI
SUBANIM_TYPE_HIT = ConvertSubAnimType(28) ---@type subanimtype @UjAPI
SUBANIM_TYPE_WOUNDED = ConvertSubAnimType(29) ---@type subanimtype @UjAPI
SUBANIM_TYPE_LIGHT = ConvertSubAnimType(30) ---@type subanimtype @UjAPI
SUBANIM_TYPE_MODERATE = ConvertSubAnimType(31) ---@type subanimtype @UjAPI
SUBANIM_TYPE_SEVERE = ConvertSubAnimType(32) ---@type subanimtype @UjAPI
SUBANIM_TYPE_CRITICAL = ConvertSubAnimType(33) ---@type subanimtype @UjAPI
SUBANIM_TYPE_COMPLETE = ConvertSubAnimType(34) ---@type subanimtype @UjAPI
SUBANIM_TYPE_GOLD = ConvertSubAnimType(35) ---@type subanimtype @UjAPI
SUBANIM_TYPE_LUMBER = ConvertSubAnimType(36) ---@type subanimtype @UjAPI
SUBANIM_TYPE_WORK = ConvertSubAnimType(37) ---@type subanimtype @UjAPI
SUBANIM_TYPE_TALK = ConvertSubAnimType(38) ---@type subanimtype @UjAPI
SUBANIM_TYPE_FIRST = ConvertSubAnimType(39) ---@type subanimtype @UjAPI
SUBANIM_TYPE_SECOND = ConvertSubAnimType(40) ---@type subanimtype @UjAPI
SUBANIM_TYPE_THIRD = ConvertSubAnimType(41) ---@type subanimtype @UjAPI
SUBANIM_TYPE_FOURTH = ConvertSubAnimType(42) ---@type subanimtype @UjAPI
SUBANIM_TYPE_FIFTH = ConvertSubAnimType(43) ---@type subanimtype @UjAPI
SUBANIM_TYPE_ONE = ConvertSubAnimType(44) ---@type subanimtype @UjAPI
SUBANIM_TYPE_TWO = ConvertSubAnimType(45) ---@type subanimtype @UjAPI
SUBANIM_TYPE_THREE = ConvertSubAnimType(46) ---@type subanimtype @UjAPI
SUBANIM_TYPE_FOUR = ConvertSubAnimType(47) ---@type subanimtype @UjAPI
SUBANIM_TYPE_FIVE = ConvertSubAnimType(48) ---@type subanimtype @UjAPI
SUBANIM_TYPE_SMALL = ConvertSubAnimType(49) ---@type subanimtype @UjAPI
SUBANIM_TYPE_MEDIUM = ConvertSubAnimType(50) ---@type subanimtype @UjAPI
SUBANIM_TYPE_LARGE = ConvertSubAnimType(51) ---@type subanimtype @UjAPI
SUBANIM_TYPE_UPGRADE = ConvertSubAnimType(52) ---@type subanimtype @UjAPI
SUBANIM_TYPE_DRAIN = ConvertSubAnimType(53) ---@type subanimtype @UjAPI
SUBANIM_TYPE_FILL = ConvertSubAnimType(54) ---@type subanimtype @UjAPI
SUBANIM_TYPE_CHAINLIGHTNING = ConvertSubAnimType(55) ---@type subanimtype @UjAPI
SUBANIM_TYPE_EATTREE = ConvertSubAnimType(56) ---@type subanimtype @UjAPI
SUBANIM_TYPE_PUKE = ConvertSubAnimType(57) ---@type subanimtype @UjAPI
SUBANIM_TYPE_FLAIL = ConvertSubAnimType(58) ---@type subanimtype @UjAPI
SUBANIM_TYPE_OFF = ConvertSubAnimType(59) ---@type subanimtype @UjAPI
SUBANIM_TYPE_SWIM = ConvertSubAnimType(60) ---@type subanimtype @UjAPI
SUBANIM_TYPE_ENTANGLE = ConvertSubAnimType(61) ---@type subanimtype @UjAPI
SUBANIM_TYPE_BERSERK = ConvertSubAnimType(62) ---@type subanimtype @UjAPI

CURSORANIM_TYPE_NORMAL = ConvertCursorAnimType(0) ---@type cursoranimtype @UjAPI
CURSORANIM_TYPE_SELECT = ConvertCursorAnimType(1) ---@type cursoranimtype @UjAPI
CURSORANIM_TYPE_TARGET = ConvertCursorAnimType(2) ---@type cursoranimtype @UjAPI
CURSORANIM_TYPE_TARGET_SELECT = ConvertCursorAnimType(3) ---@type cursoranimtype @UjAPI
CURSORANIM_TYPE_INVALID_TARGET = ConvertCursorAnimType(4) ---@type cursoranimtype @UjAPI
CURSORANIM_TYPE_HOLD_ITEM = ConvertCursorAnimType(5) ---@type cursoranimtype @UjAPI
CURSORANIM_TYPE_SCROLL = ConvertCursorAnimType(6) ---@type cursoranimtype @UjAPI
CURSORANIM_TYPE_LEFT = ConvertCursorAnimType(7) ---@type cursoranimtype @UjAPI
CURSORANIM_TYPE_RIGHT = ConvertCursorAnimType(8) ---@type cursoranimtype @UjAPI
CURSORANIM_TYPE_UP = ConvertCursorAnimType(9) ---@type cursoranimtype @UjAPI
CURSORANIM_TYPE_DOWN = ConvertCursorAnimType(10) ---@type cursoranimtype @UjAPI

-- ===================================================
-- Map Setup Constants
-- ===================================================

RACE_PREF_HUMAN = ConvertRacePref(1) ---@type racepreference
RACE_PREF_ORC = ConvertRacePref(2) ---@type racepreference
RACE_PREF_NIGHTELF = ConvertRacePref(4) ---@type racepreference
RACE_PREF_UNDEAD = ConvertRacePref(8) ---@type racepreference
RACE_PREF_DEMON = ConvertRacePref(16) ---@type racepreference
RACE_PREF_RANDOM = ConvertRacePref(32) ---@type racepreference
RACE_PREF_USER_SELECTABLE = ConvertRacePref(64) ---@type racepreference

MAP_CONTROL_USER = ConvertMapControl(0) ---@type mapcontrol
MAP_CONTROL_COMPUTER = ConvertMapControl(1) ---@type mapcontrol
MAP_CONTROL_RESCUABLE = ConvertMapControl(2) ---@type mapcontrol
MAP_CONTROL_NEUTRAL = ConvertMapControl(3) ---@type mapcontrol
MAP_CONTROL_CREEP = ConvertMapControl(4) ---@type mapcontrol
MAP_CONTROL_NONE = ConvertMapControl(5) ---@type mapcontrol

GAME_TYPE_MELEE = ConvertGameType(1) ---@type gametype
GAME_TYPE_FFA = ConvertGameType(2) ---@type gametype
GAME_TYPE_USE_MAP_SETTINGS = ConvertGameType(4) ---@type gametype
GAME_TYPE_BLIZ = ConvertGameType(8) ---@type gametype
GAME_TYPE_ONE_ON_ONE = ConvertGameType(16) ---@type gametype
GAME_TYPE_TWO_TEAM_PLAY = ConvertGameType(32) ---@type gametype
GAME_TYPE_THREE_TEAM_PLAY = ConvertGameType(64) ---@type gametype
GAME_TYPE_FOUR_TEAM_PLAY = ConvertGameType(128) ---@type gametype

MAP_FOG_HIDE_TERRAIN = ConvertMapFlag(1) ---@type mapflag
MAP_FOG_MAP_EXPLORED = ConvertMapFlag(2) ---@type mapflag
MAP_FOG_ALWAYS_VISIBLE = ConvertMapFlag(4) ---@type mapflag

MAP_USE_HANDICAPS = ConvertMapFlag(8) ---@type mapflag
MAP_OBSERVERS = ConvertMapFlag(16) ---@type mapflag
MAP_OBSERVERS_ON_DEATH = ConvertMapFlag(32) ---@type mapflag

MAP_FIXED_COLORS = ConvertMapFlag(128) ---@type mapflag

MAP_LOCK_RESOURCE_TRADING = ConvertMapFlag(256) ---@type mapflag
MAP_RESOURCE_TRADING_ALLIES_ONLY = ConvertMapFlag(512) ---@type mapflag

MAP_LOCK_ALLIANCE_CHANGES = ConvertMapFlag(1024) ---@type mapflag
MAP_ALLIANCE_CHANGES_HIDDEN = ConvertMapFlag(2048) ---@type mapflag

MAP_CHEATS = ConvertMapFlag(4096) ---@type mapflag
MAP_CHEATS_HIDDEN = ConvertMapFlag(8192) ---@type mapflag

MAP_LOCK_SPEED = ConvertMapFlag(8192*2) ---@type mapflag
MAP_LOCK_RANDOM_SEED = ConvertMapFlag(8192*4) ---@type mapflag
MAP_SHARED_ADVANCED_CONTROL = ConvertMapFlag(8192*8) ---@type mapflag
MAP_RANDOM_HERO = ConvertMapFlag(8192*16) ---@type mapflag
MAP_RANDOM_RACES = ConvertMapFlag(8192*32) ---@type mapflag
MAP_RELOADED = ConvertMapFlag(8192*64) ---@type mapflag

MAP_PLACEMENT_RANDOM = ConvertPlacement(0) ---@type placement
MAP_PLACEMENT_FIXED = ConvertPlacement(1) ---@type placement
MAP_PLACEMENT_USE_MAP_SETTINGS = ConvertPlacement(2) ---@type placement
MAP_PLACEMENT_TEAMS_TOGETHER = ConvertPlacement(3) ---@type placement

MAP_LOC_PRIO_LOW = ConvertStartLocPrio(0) ---@type startlocprio
MAP_LOC_PRIO_HIGH = ConvertStartLocPrio(1) ---@type startlocprio
MAP_LOC_PRIO_NOT = ConvertStartLocPrio(2) ---@type startlocprio

MAP_DENSITY_NONE = ConvertMapDensity(0) ---@type mapdensity
MAP_DENSITY_LIGHT = ConvertMapDensity(1) ---@type mapdensity
MAP_DENSITY_MEDIUM = ConvertMapDensity(2) ---@type mapdensity
MAP_DENSITY_HEAVY = ConvertMapDensity(3) ---@type mapdensity

MAP_DIFFICULTY_EASY = ConvertGameDifficulty(0) ---@type gamedifficulty
MAP_DIFFICULTY_NORMAL = ConvertGameDifficulty(1) ---@type gamedifficulty
MAP_DIFFICULTY_HARD = ConvertGameDifficulty(2) ---@type gamedifficulty
MAP_DIFFICULTY_INSANE = ConvertGameDifficulty(3) ---@type gamedifficulty

MAP_SPEED_SLOWEST = ConvertGameSpeed(0) ---@type gamespeed
MAP_SPEED_SLOW = ConvertGameSpeed(1) ---@type gamespeed
MAP_SPEED_NORMAL = ConvertGameSpeed(2) ---@type gamespeed
MAP_SPEED_FAST = ConvertGameSpeed(3) ---@type gamespeed
MAP_SPEED_FASTEST = ConvertGameSpeed(4) ---@type gamespeed

PLAYER_SLOT_STATE_EMPTY = ConvertPlayerSlotState(0) ---@type playerslotstate
PLAYER_SLOT_STATE_PLAYING = ConvertPlayerSlotState(1) ---@type playerslotstate
PLAYER_SLOT_STATE_LEFT = ConvertPlayerSlotState(2) ---@type playerslotstate

-- ===================================================
-- Sound Constants
-- ===================================================

SOUND_VOLUMEGROUP_UNITMOVEMENT = ConvertVolumeGroup(0) ---@type volumegroup
SOUND_VOLUMEGROUP_UNITSOUNDS = ConvertVolumeGroup(1) ---@type volumegroup
SOUND_VOLUMEGROUP_COMBAT = ConvertVolumeGroup(2) ---@type volumegroup
SOUND_VOLUMEGROUP_SPELLS = ConvertVolumeGroup(3) ---@type volumegroup
SOUND_VOLUMEGROUP_UI = ConvertVolumeGroup(4) ---@type volumegroup
SOUND_VOLUMEGROUP_MUSIC = ConvertVolumeGroup(5) ---@type volumegroup
SOUND_VOLUMEGROUP_AMBIENTSOUNDS = ConvertVolumeGroup(6) ---@type volumegroup
SOUND_VOLUMEGROUP_FIRE = ConvertVolumeGroup(7) ---@type volumegroup

-- ===================================================
-- Game, Player, and Unit States
-- 
-- For use with TriggerRegister<X>StateEvent
-- 
-- ===================================================

GAME_STATE_DIVINE_INTERVENTION = ConvertIGameState(0) ---@type igamestate
GAME_STATE_DISCONNECTED = ConvertIGameState(1) ---@type igamestate
GAME_STATE_TIME_OF_DAY = ConvertFGameState(2) ---@type fgamestate

PLAYER_STATE_GAME_RESULT = ConvertPlayerState(0) ---@type playerstate

-- current resource levels
-- 
PLAYER_STATE_RESOURCE_GOLD = ConvertPlayerState(1) ---@type playerstate
PLAYER_STATE_RESOURCE_LUMBER = ConvertPlayerState(2) ---@type playerstate
PLAYER_STATE_RESOURCE_HERO_TOKENS = ConvertPlayerState(3) ---@type playerstate
PLAYER_STATE_RESOURCE_FOOD_CAP = ConvertPlayerState(4) ---@type playerstate
PLAYER_STATE_RESOURCE_FOOD_USED = ConvertPlayerState(5) ---@type playerstate
PLAYER_STATE_FOOD_CAP_CEILING = ConvertPlayerState(6) ---@type playerstate

PLAYER_STATE_GIVES_BOUNTY = ConvertPlayerState(7) ---@type playerstate
PLAYER_STATE_ALLIED_VICTORY = ConvertPlayerState(8) ---@type playerstate
PLAYER_STATE_PLACED = ConvertPlayerState(9) ---@type playerstate
PLAYER_STATE_OBSERVER_ON_DEATH = ConvertPlayerState(10) ---@type playerstate
PLAYER_STATE_OBSERVER = ConvertPlayerState(11) ---@type playerstate
PLAYER_STATE_UNFOLLOWABLE = ConvertPlayerState(12) ---@type playerstate

-- taxation rate for each resource
-- 
PLAYER_STATE_GOLD_UPKEEP_RATE = ConvertPlayerState(13) ---@type playerstate
PLAYER_STATE_LUMBER_UPKEEP_RATE = ConvertPlayerState(14) ---@type playerstate

-- cumulative resources collected by the player during the mission
-- 
PLAYER_STATE_GOLD_GATHERED = ConvertPlayerState(15) ---@type playerstate
PLAYER_STATE_LUMBER_GATHERED = ConvertPlayerState(16) ---@type playerstate

PLAYER_STATE_NO_CREEP_SLEEP = ConvertPlayerState(25) ---@type playerstate

UNIT_STATE_LIFE = ConvertUnitState(0) ---@type unitstate
UNIT_STATE_MAX_LIFE = ConvertUnitState(1) ---@type unitstate
UNIT_STATE_MANA = ConvertUnitState(2) ---@type unitstate
UNIT_STATE_MAX_MANA = ConvertUnitState(3) ---@type unitstate

AI_DIFFICULTY_NEWBIE = ConvertAIDifficulty(0) ---@type aidifficulty
AI_DIFFICULTY_NORMAL = ConvertAIDifficulty(1) ---@type aidifficulty
AI_DIFFICULTY_INSANE = ConvertAIDifficulty(2) ---@type aidifficulty

-- player score values
PLAYER_SCORE_UNITS_TRAINED = ConvertPlayerScore(0) ---@type playerscore
PLAYER_SCORE_UNITS_KILLED = ConvertPlayerScore(1) ---@type playerscore
PLAYER_SCORE_STRUCT_BUILT = ConvertPlayerScore(2) ---@type playerscore
PLAYER_SCORE_STRUCT_RAZED = ConvertPlayerScore(3) ---@type playerscore
PLAYER_SCORE_TECH_PERCENT = ConvertPlayerScore(4) ---@type playerscore
PLAYER_SCORE_FOOD_MAXPROD = ConvertPlayerScore(5) ---@type playerscore
PLAYER_SCORE_FOOD_MAXUSED = ConvertPlayerScore(6) ---@type playerscore
PLAYER_SCORE_HEROES_KILLED = ConvertPlayerScore(7) ---@type playerscore
PLAYER_SCORE_ITEMS_GAINED = ConvertPlayerScore(8) ---@type playerscore
PLAYER_SCORE_MERCS_HIRED = ConvertPlayerScore(9) ---@type playerscore
PLAYER_SCORE_GOLD_MINED_TOTAL = ConvertPlayerScore(10) ---@type playerscore
PLAYER_SCORE_GOLD_MINED_UPKEEP = ConvertPlayerScore(11) ---@type playerscore
PLAYER_SCORE_GOLD_LOST_UPKEEP = ConvertPlayerScore(12) ---@type playerscore
PLAYER_SCORE_GOLD_LOST_TAX = ConvertPlayerScore(13) ---@type playerscore
PLAYER_SCORE_GOLD_GIVEN = ConvertPlayerScore(14) ---@type playerscore
PLAYER_SCORE_GOLD_RECEIVED = ConvertPlayerScore(15) ---@type playerscore
PLAYER_SCORE_LUMBER_TOTAL = ConvertPlayerScore(16) ---@type playerscore
PLAYER_SCORE_LUMBER_LOST_UPKEEP = ConvertPlayerScore(17) ---@type playerscore
PLAYER_SCORE_LUMBER_LOST_TAX = ConvertPlayerScore(18) ---@type playerscore
PLAYER_SCORE_LUMBER_GIVEN = ConvertPlayerScore(19) ---@type playerscore
PLAYER_SCORE_LUMBER_RECEIVED = ConvertPlayerScore(20) ---@type playerscore
PLAYER_SCORE_UNIT_TOTAL = ConvertPlayerScore(21) ---@type playerscore
PLAYER_SCORE_HERO_TOTAL = ConvertPlayerScore(22) ---@type playerscore
PLAYER_SCORE_RESOURCE_TOTAL = ConvertPlayerScore(23) ---@type playerscore
PLAYER_SCORE_TOTAL = ConvertPlayerScore(24) ---@type playerscore

-- ===================================================
-- Game, Player and Unit Events
-- 
-- When an event causes a trigger to fire these
-- values allow the action code to determine which
-- event was dispatched and therefore which set of
-- native functions should be used to get information
-- about the event.
-- 
-- Do NOT change the order or value of these constants
-- without insuring that the JASS_GAME_EVENTS_WAR3 enum
-- is changed to match.
-- 
-- ===================================================

-- ===================================================
-- For use with TriggerRegisterGameEvent
-- ===================================================

EVENT_GAME_VICTORY = ConvertGameEvent(0) ---@type gameevent
EVENT_GAME_END_LEVEL = ConvertGameEvent(1) ---@type gameevent

EVENT_GAME_VARIABLE_LIMIT = ConvertGameEvent(2) ---@type gameevent
EVENT_GAME_STATE_LIMIT = ConvertGameEvent(3) ---@type gameevent

EVENT_GAME_TIMER_EXPIRED = ConvertGameEvent(4) ---@type gameevent

EVENT_GAME_ENTER_REGION = ConvertGameEvent(5) ---@type gameevent
EVENT_GAME_LEAVE_REGION = ConvertGameEvent(6) ---@type gameevent

EVENT_GAME_TRACKABLE_HIT = ConvertGameEvent(7) ---@type gameevent
EVENT_GAME_TRACKABLE_TRACK = ConvertGameEvent(8) ---@type gameevent

EVENT_GAME_SHOW_SKILL = ConvertGameEvent(9) ---@type gameevent
EVENT_GAME_BUILD_SUBMENU = ConvertGameEvent(10) ---@type gameevent

-- ===================================================
-- For use with TriggerRegisterPlayerEvent
-- ===================================================

EVENT_PLAYER_STATE_LIMIT = ConvertPlayerEvent(11) ---@type playerevent
EVENT_PLAYER_ALLIANCE_CHANGED = ConvertPlayerEvent(12) ---@type playerevent

EVENT_PLAYER_DEFEAT = ConvertPlayerEvent(13) ---@type playerevent
EVENT_PLAYER_VICTORY = ConvertPlayerEvent(14) ---@type playerevent
EVENT_PLAYER_LEAVE = ConvertPlayerEvent(15) ---@type playerevent
EVENT_PLAYER_CHAT = ConvertPlayerEvent(16) ---@type playerevent
EVENT_PLAYER_END_CINEMATIC = ConvertPlayerEvent(17) ---@type playerevent

-- ===================================================
-- For use with TriggerRegisterPlayerUnitEvent
-- ===================================================

EVENT_PLAYER_UNIT_ATTACKED = ConvertPlayerUnitEvent(18) ---@type playerunitevent
EVENT_PLAYER_UNIT_RESCUED = ConvertPlayerUnitEvent(19) ---@type playerunitevent

EVENT_PLAYER_UNIT_DEATH = ConvertPlayerUnitEvent(20) ---@type playerunitevent
EVENT_PLAYER_UNIT_DECAY = ConvertPlayerUnitEvent(21) ---@type playerunitevent

EVENT_PLAYER_UNIT_DETECTED = ConvertPlayerUnitEvent(22) ---@type playerunitevent
EVENT_PLAYER_UNIT_HIDDEN = ConvertPlayerUnitEvent(23) ---@type playerunitevent

EVENT_PLAYER_UNIT_SELECTED = ConvertPlayerUnitEvent(24) ---@type playerunitevent
EVENT_PLAYER_UNIT_DESELECTED = ConvertPlayerUnitEvent(25) ---@type playerunitevent

EVENT_PLAYER_UNIT_CONSTRUCT_START = ConvertPlayerUnitEvent(26) ---@type playerunitevent
EVENT_PLAYER_UNIT_CONSTRUCT_CANCEL = ConvertPlayerUnitEvent(27) ---@type playerunitevent
EVENT_PLAYER_UNIT_CONSTRUCT_FINISH = ConvertPlayerUnitEvent(28) ---@type playerunitevent

EVENT_PLAYER_UNIT_UPGRADE_START = ConvertPlayerUnitEvent(29) ---@type playerunitevent
EVENT_PLAYER_UNIT_UPGRADE_CANCEL = ConvertPlayerUnitEvent(30) ---@type playerunitevent
EVENT_PLAYER_UNIT_UPGRADE_FINISH = ConvertPlayerUnitEvent(31) ---@type playerunitevent

EVENT_PLAYER_UNIT_TRAIN_START = ConvertPlayerUnitEvent(32) ---@type playerunitevent
EVENT_PLAYER_UNIT_TRAIN_CANCEL = ConvertPlayerUnitEvent(33) ---@type playerunitevent
EVENT_PLAYER_UNIT_TRAIN_FINISH = ConvertPlayerUnitEvent(34) ---@type playerunitevent

EVENT_PLAYER_UNIT_RESEARCH_START = ConvertPlayerUnitEvent(35) ---@type playerunitevent
EVENT_PLAYER_UNIT_RESEARCH_CANCEL = ConvertPlayerUnitEvent(36) ---@type playerunitevent
EVENT_PLAYER_UNIT_RESEARCH_FINISH = ConvertPlayerUnitEvent(37) ---@type playerunitevent
EVENT_PLAYER_UNIT_ISSUED_ORDER = ConvertPlayerUnitEvent(38) ---@type playerunitevent
EVENT_PLAYER_UNIT_ISSUED_POINT_ORDER = ConvertPlayerUnitEvent(39) ---@type playerunitevent
EVENT_PLAYER_UNIT_ISSUED_TARGET_ORDER = ConvertPlayerUnitEvent(40) ---@type playerunitevent
EVENT_PLAYER_UNIT_ISSUED_UNIT_ORDER = ConvertPlayerUnitEvent(40) ---@type playerunitevent

EVENT_PLAYER_HERO_LEVEL = ConvertPlayerUnitEvent(41) ---@type playerunitevent
EVENT_PLAYER_HERO_SKILL = ConvertPlayerUnitEvent(42) ---@type playerunitevent

EVENT_PLAYER_HERO_REVIVABLE = ConvertPlayerUnitEvent(43) ---@type playerunitevent

EVENT_PLAYER_HERO_REVIVE_START = ConvertPlayerUnitEvent(44) ---@type playerunitevent
EVENT_PLAYER_HERO_REVIVE_CANCEL = ConvertPlayerUnitEvent(45) ---@type playerunitevent
EVENT_PLAYER_HERO_REVIVE_FINISH = ConvertPlayerUnitEvent(46) ---@type playerunitevent
EVENT_PLAYER_UNIT_SUMMON = ConvertPlayerUnitEvent(47) ---@type playerunitevent
EVENT_PLAYER_UNIT_DROP_ITEM = ConvertPlayerUnitEvent(48) ---@type playerunitevent
EVENT_PLAYER_UNIT_PICKUP_ITEM = ConvertPlayerUnitEvent(49) ---@type playerunitevent
EVENT_PLAYER_UNIT_USE_ITEM = ConvertPlayerUnitEvent(50) ---@type playerunitevent

EVENT_PLAYER_UNIT_LOADED = ConvertPlayerUnitEvent(51) ---@type playerunitevent
EVENT_PLAYER_UNIT_DAMAGED = ConvertPlayerUnitEvent(308) ---@type playerunitevent @UjAPI
EVENT_PLAYER_UNIT_DAMAGING = ConvertPlayerUnitEvent(315) ---@type playerunitevent @UjAPI
EVENT_PLAYER_UNIT_ATTACK_FINISHED = ConvertPlayerUnitEvent(317) ---@type playerunitevent @UjAPI
EVENT_PLAYER_UNIT_DECAY_FINISHED = ConvertPlayerUnitEvent(319) ---@type playerunitevent @UjAPI

EVENT_PLAYER_UNIT_REINCARNATION_START = ConvertPlayerUnitEvent(325) ---@type playerunitevent @UjAPI
EVENT_PLAYER_UNIT_REINCARNATION_END = ConvertPlayerUnitEvent(327) ---@type playerunitevent @UjAPI
EVENT_PLAYER_UNIT_REVIVED = ConvertPlayerUnitEvent(329) ---@type playerunitevent @UjAPI

-- ===================================================
-- For use with TriggerRegisterUnitEvent
-- ===================================================

EVENT_UNIT_DAMAGED = ConvertUnitEvent(52) ---@type unitevent
EVENT_UNIT_DAMAGING = ConvertUnitEvent(314) ---@type unitevent @UjAPI
EVENT_UNIT_DEATH = ConvertUnitEvent(53) ---@type unitevent
EVENT_UNIT_DECAY = ConvertUnitEvent(54) ---@type unitevent
EVENT_UNIT_DETECTED = ConvertUnitEvent(55) ---@type unitevent
EVENT_UNIT_HIDDEN = ConvertUnitEvent(56) ---@type unitevent
EVENT_UNIT_SELECTED = ConvertUnitEvent(57) ---@type unitevent
EVENT_UNIT_DESELECTED = ConvertUnitEvent(58) ---@type unitevent

EVENT_UNIT_STATE_LIMIT = ConvertUnitEvent(59) ---@type unitevent

-- Events which may have a filter for the "other unit"
-- 
EVENT_UNIT_ACQUIRED_TARGET = ConvertUnitEvent(60) ---@type unitevent
EVENT_UNIT_TARGET_IN_RANGE = ConvertUnitEvent(61) ---@type unitevent
EVENT_UNIT_ATTACKED = ConvertUnitEvent(62) ---@type unitevent
EVENT_UNIT_RESCUED = ConvertUnitEvent(63) ---@type unitevent

EVENT_UNIT_CONSTRUCT_CANCEL = ConvertUnitEvent(64) ---@type unitevent
EVENT_UNIT_CONSTRUCT_FINISH = ConvertUnitEvent(65) ---@type unitevent

EVENT_UNIT_UPGRADE_START = ConvertUnitEvent(66) ---@type unitevent
EVENT_UNIT_UPGRADE_CANCEL = ConvertUnitEvent(67) ---@type unitevent
EVENT_UNIT_UPGRADE_FINISH = ConvertUnitEvent(68) ---@type unitevent

-- Events which involve the specified unit performing
-- training of other units
-- 
EVENT_UNIT_TRAIN_START = ConvertUnitEvent(69) ---@type unitevent
EVENT_UNIT_TRAIN_CANCEL = ConvertUnitEvent(70) ---@type unitevent
EVENT_UNIT_TRAIN_FINISH = ConvertUnitEvent(71) ---@type unitevent

EVENT_UNIT_RESEARCH_START = ConvertUnitEvent(72) ---@type unitevent
EVENT_UNIT_RESEARCH_CANCEL = ConvertUnitEvent(73) ---@type unitevent
EVENT_UNIT_RESEARCH_FINISH = ConvertUnitEvent(74) ---@type unitevent

EVENT_UNIT_ISSUED_ORDER = ConvertUnitEvent(75) ---@type unitevent
EVENT_UNIT_ISSUED_POINT_ORDER = ConvertUnitEvent(76) ---@type unitevent
EVENT_UNIT_ISSUED_TARGET_ORDER = ConvertUnitEvent(77) ---@type unitevent

EVENT_UNIT_HERO_LEVEL = ConvertUnitEvent(78) ---@type unitevent
EVENT_UNIT_HERO_SKILL = ConvertUnitEvent(79) ---@type unitevent

EVENT_UNIT_HERO_REVIVABLE = ConvertUnitEvent(80) ---@type unitevent
EVENT_UNIT_HERO_REVIVE_START = ConvertUnitEvent(81) ---@type unitevent
EVENT_UNIT_HERO_REVIVE_CANCEL = ConvertUnitEvent(82) ---@type unitevent
EVENT_UNIT_HERO_REVIVE_FINISH = ConvertUnitEvent(83) ---@type unitevent

EVENT_UNIT_SUMMON = ConvertUnitEvent(84) ---@type unitevent

EVENT_UNIT_DROP_ITEM = ConvertUnitEvent(85) ---@type unitevent
EVENT_UNIT_PICKUP_ITEM = ConvertUnitEvent(86) ---@type unitevent
EVENT_UNIT_USE_ITEM = ConvertUnitEvent(87) ---@type unitevent

EVENT_UNIT_LOADED = ConvertUnitEvent(88) ---@type unitevent

EVENT_UNIT_ATTACK_FINISHED = ConvertUnitEvent(316) ---@type unitevent @UjAPI
EVENT_UNIT_DECAY_FINISHED = ConvertUnitEvent(318) ---@type unitevent @UjAPI

EVENT_UNIT_REINCARNATION_START = ConvertUnitEvent(326) ---@type unitevent @UjAPI
EVENT_UNIT_REINCARNATION_END = ConvertUnitEvent(328) ---@type unitevent @UjAPI
EVENT_UNIT_REVIVED = ConvertUnitEvent(330) ---@type unitevent @UjAPI

-- ===================================================
-- For use with TriggerRegisterWidgetEvent
-- ===================================================

EVENT_WIDGET_DEATH = ConvertWidgetEvent(89) ---@type widgetevent
EVENT_WIDGET_DAMAGING = ConvertWidgetEvent(400) ---@type widgetevent @UjAPI
EVENT_WIDGET_DAMAGED = ConvertWidgetEvent(401) ---@type widgetevent @UjAPI

-- ===================================================
-- For use with TriggerRegisterDialogEvent
-- ===================================================

EVENT_DIALOG_BUTTON_CLICK = ConvertDialogEvent(90) ---@type dialogevent
EVENT_DIALOG_CLICK = ConvertDialogEvent(91) ---@type dialogevent

-- ===================================================
-- Frozen Throne Expansion Events
-- Need to be added here to preserve compat
-- ===================================================

-- ===================================================
-- For use with TriggerRegisterGameEvent
-- ===================================================

EVENT_GAME_LOADED = ConvertGameEvent(256) ---@type gameevent
EVENT_GAME_TOURNAMENT_FINISH_SOON = ConvertGameEvent(257) ---@type gameevent
EVENT_GAME_TOURNAMENT_FINISH_NOW = ConvertGameEvent(258) ---@type gameevent
EVENT_GAME_SAVE = ConvertGameEvent(259) ---@type gameevent

EVENT_GAME_AGENT_DESTROYED = ConvertGameEvent(800) ---@type gameevent @UjAPI
EVENT_GAME_AGENT_ARRIVAL = ConvertGameEvent(801) ---@type gameevent @UjAPI
EVENT_GAME_AGENT_CANT_PATH = ConvertGameEvent(802) ---@type gameevent @UjAPI
EVENT_GAME_AGENT_WARP_START = ConvertGameEvent(803) ---@type gameevent @UjAPI
EVENT_GAME_AGENT_WARP_END = ConvertGameEvent(804) ---@type gameevent @UjAPI
EVENT_GAME_WIDGET_DAMAGING = ConvertGameEvent(805) ---@type gameevent @UjAPI
EVENT_GAME_WIDGET_DAMAGED = ConvertGameEvent(806) ---@type gameevent @UjAPI
EVENT_GAME_WIDGET_DEATH = ConvertGameEvent(807) ---@type gameevent @UjAPI

EVENT_GAME_HACK_DETECTED = ConvertGameEvent(850) ---@type gameevent @UjAPI
-- ===================================================
-- For use with TriggerRegisterPlayerEvent
-- ===================================================

EVENT_PLAYER_ARROW_LEFT_DOWN = ConvertPlayerEvent(261) ---@type playerevent
EVENT_PLAYER_ARROW_LEFT_UP = ConvertPlayerEvent(262) ---@type playerevent
EVENT_PLAYER_ARROW_RIGHT_DOWN = ConvertPlayerEvent(263) ---@type playerevent
EVENT_PLAYER_ARROW_RIGHT_UP = ConvertPlayerEvent(264) ---@type playerevent
EVENT_PLAYER_ARROW_DOWN_DOWN = ConvertPlayerEvent(265) ---@type playerevent
EVENT_PLAYER_ARROW_DOWN_UP = ConvertPlayerEvent(266) ---@type playerevent
EVENT_PLAYER_ARROW_UP_DOWN = ConvertPlayerEvent(267) ---@type playerevent
EVENT_PLAYER_ARROW_UP_UP = ConvertPlayerEvent(268) ---@type playerevent

EVENT_PLAYER_MOUSE_DOWN = ConvertPlayerEvent(305) ---@type playerevent @UjAPI
EVENT_PLAYER_MOUSE_UP = ConvertPlayerEvent(306) ---@type playerevent @UjAPI
EVENT_PLAYER_MOUSE_MOVE = ConvertPlayerEvent(307) ---@type playerevent @UjAPI
EVENT_PLAYER_SYNC_DATA = ConvertPlayerEvent(309) ---@type playerevent @UjAPI
EVENT_PLAYER_KEY = ConvertPlayerEvent(311) ---@type playerevent @UjAPI
EVENT_PLAYER_KEY_DOWN = ConvertPlayerEvent(312) ---@type playerevent @UjAPI
EVENT_PLAYER_KEY_UP = ConvertPlayerEvent(313) ---@type playerevent @UjAPI

EVENT_PLAYER_WIDGET_TRACK = ConvertPlayerEvent(320) ---@type playerevent @UjAPI
EVENT_PLAYER_WIDGET_GHOST_TRACK = ConvertPlayerEvent(321) ---@type playerevent @UjAPI
EVENT_PLAYER_WIDGET_CLICK = ConvertPlayerEvent(322) ---@type playerevent @UjAPI
EVENT_PLAYER_WIDGET_GHOST_CLICK = ConvertPlayerEvent(323) ---@type playerevent @UjAPI
EVENT_PLAYER_TERRAIN_CLICK = ConvertPlayerEvent(324) ---@type playerevent @UjAPI

EVENT_PLAYER_TRADE_RESOURCE = ConvertPlayerEvent(350) ---@type playerevent @UjAPI
-- ===================================================
-- For use with TriggerRegisterPlayerUnitEvent
-- ===================================================

EVENT_PLAYER_UNIT_SELL = ConvertPlayerUnitEvent(269) ---@type playerunitevent
EVENT_PLAYER_UNIT_CHANGE_OWNER = ConvertPlayerUnitEvent(270) ---@type playerunitevent
EVENT_PLAYER_UNIT_SELL_ITEM = ConvertPlayerUnitEvent(271) ---@type playerunitevent
EVENT_PLAYER_UNIT_SPELL_CHANNEL = ConvertPlayerUnitEvent(272) ---@type playerunitevent
EVENT_PLAYER_UNIT_SPELL_CAST = ConvertPlayerUnitEvent(273) ---@type playerunitevent
EVENT_PLAYER_UNIT_SPELL_EFFECT = ConvertPlayerUnitEvent(274) ---@type playerunitevent
EVENT_PLAYER_UNIT_SPELL_FINISH = ConvertPlayerUnitEvent(275) ---@type playerunitevent
EVENT_PLAYER_UNIT_SPELL_ENDCAST = ConvertPlayerUnitEvent(276) ---@type playerunitevent
EVENT_PLAYER_UNIT_PAWN_ITEM = ConvertPlayerUnitEvent(277) ---@type playerunitevent

EVENT_PLAYER_UNIT_BUFF_RECEIVED = ConvertPlayerUnitEvent(500) ---@type playerunitevent @UjAPI
EVENT_PLAYER_UNIT_BUFF_REFRESHED = ConvertPlayerUnitEvent(501) ---@type playerunitevent @UjAPI
EVENT_PLAYER_UNIT_BUFF_ENDED = ConvertPlayerUnitEvent(502) ---@type playerunitevent @UjAPI
EVENT_PLAYER_UNIT_BUFF_REMOVED = ConvertPlayerUnitEvent(508) ---@type playerunitevent @UjAPI

EVENT_PLAYER_UNIT_ABILITY_ADDED = ConvertPlayerUnitEvent(503) ---@type playerunitevent @UjAPI
EVENT_PLAYER_UNIT_ABILITY_REMOVED = ConvertPlayerUnitEvent(504) ---@type playerunitevent @UjAPI
EVENT_PLAYER_UNIT_ABILITY_AUTOCAST_ON = ConvertPlayerUnitEvent(505) ---@type playerunitevent @UjAPI
EVENT_PLAYER_UNIT_ABILITY_AUTOCAST_OFF = ConvertPlayerUnitEvent(506) ---@type playerunitevent @UjAPI
EVENT_PLAYER_UNIT_ABILITY_LEVEL_CHANGED = ConvertPlayerUnitEvent(507) ---@type playerunitevent @UjAPI

EVENT_PLAYER_UNIT_PROJECTILE_LAUNCH = ConvertPlayerUnitEvent(600) ---@type playerunitevent @UjAPI
EVENT_PLAYER_UNIT_PROJECTILE_HIT = ConvertPlayerUnitEvent(601) ---@type playerunitevent @UjAPI

-- ===================================================
-- For use with TriggerRegisterUnitEvent
-- ===================================================

EVENT_UNIT_SELL = ConvertUnitEvent(286) ---@type unitevent
EVENT_UNIT_CHANGE_OWNER = ConvertUnitEvent(287) ---@type unitevent
EVENT_UNIT_SELL_ITEM = ConvertUnitEvent(288) ---@type unitevent
EVENT_UNIT_SPELL_CHANNEL = ConvertUnitEvent(289) ---@type unitevent
EVENT_UNIT_SPELL_CAST = ConvertUnitEvent(290) ---@type unitevent
EVENT_UNIT_SPELL_EFFECT = ConvertUnitEvent(291) ---@type unitevent
EVENT_UNIT_SPELL_FINISH = ConvertUnitEvent(292) ---@type unitevent
EVENT_UNIT_SPELL_ENDCAST = ConvertUnitEvent(293) ---@type unitevent
EVENT_UNIT_PAWN_ITEM = ConvertUnitEvent(294) ---@type unitevent

EVENT_UNIT_BUFF_RECEIVED = ConvertUnitEvent(510) ---@type unitevent @UjAPI
EVENT_UNIT_BUFF_REFRESHED = ConvertUnitEvent(511) ---@type unitevent @UjAPI
EVENT_UNIT_BUFF_ENDED = ConvertUnitEvent(512) ---@type unitevent @UjAPI
EVENT_UNIT_BUFF_REMOVED = ConvertUnitEvent(518) ---@type unitevent @UjAPI

EVENT_UNIT_ABILITY_ADDED = ConvertUnitEvent(513) ---@type unitevent @UjAPI
EVENT_UNIT_ABILITY_REMOVED = ConvertUnitEvent(514) ---@type unitevent @UjAPI
EVENT_UNIT_ABILITY_AUTOCAST_ON = ConvertUnitEvent(515) ---@type unitevent @UjAPI
EVENT_UNIT_ABILITY_AUTOCAST_OFF = ConvertUnitEvent(516) ---@type unitevent @UjAPI
EVENT_UNIT_ABILITY_LEVEL_CHANGED = ConvertUnitEvent(517) ---@type unitevent @UjAPI

EVENT_UNIT_PROJECTILE_LAUNCH = ConvertUnitEvent(610) ---@type unitevent @UjAPI
EVENT_UNIT_PROJECTILE_HIT = ConvertUnitEvent(611) ---@type unitevent @UjAPI

-- ===================================================
-- Limit Event API constants
-- variable, player state, game state, and unit state events
-- ( do NOT change the order of these... )
-- ===================================================
LESS_THAN = ConvertLimitOp(0) ---@type limitop
LESS_THAN_OR_EQUAL = ConvertLimitOp(1) ---@type limitop
EQUAL = ConvertLimitOp(2) ---@type limitop
GREATER_THAN_OR_EQUAL = ConvertLimitOp(3) ---@type limitop
GREATER_THAN = ConvertLimitOp(4) ---@type limitop
NOT_EQUAL = ConvertLimitOp(5) ---@type limitop

-- ===================================================
-- Unit Type Constants for use with IsUnitType()
-- ===================================================

UNIT_TYPE_HERO = ConvertUnitType(0) ---@type unittype
UNIT_TYPE_DEAD = ConvertUnitType(1) ---@type unittype
UNIT_TYPE_STRUCTURE = ConvertUnitType(2) ---@type unittype

UNIT_TYPE_FLYING = ConvertUnitType(3) ---@type unittype
UNIT_TYPE_GROUND = ConvertUnitType(4) ---@type unittype

UNIT_TYPE_ATTACKS_FLYING = ConvertUnitType(5) ---@type unittype
UNIT_TYPE_ATTACKS_GROUND = ConvertUnitType(6) ---@type unittype

UNIT_TYPE_MELEE_ATTACKER = ConvertUnitType(7) ---@type unittype
UNIT_TYPE_RANGED_ATTACKER = ConvertUnitType(8) ---@type unittype

UNIT_TYPE_GIANT = ConvertUnitType(9) ---@type unittype
UNIT_TYPE_SUMMONED = ConvertUnitType(10) ---@type unittype
UNIT_TYPE_STUNNED = ConvertUnitType(11) ---@type unittype
UNIT_TYPE_PLAGUED = ConvertUnitType(12) ---@type unittype
UNIT_TYPE_SNARED = ConvertUnitType(13) ---@type unittype

UNIT_TYPE_UNDEAD = ConvertUnitType(14) ---@type unittype
UNIT_TYPE_MECHANICAL = ConvertUnitType(15) ---@type unittype
UNIT_TYPE_PEON = ConvertUnitType(16) ---@type unittype
UNIT_TYPE_SAPPER = ConvertUnitType(17) ---@type unittype
UNIT_TYPE_TOWNHALL = ConvertUnitType(18) ---@type unittype
UNIT_TYPE_ANCIENT = ConvertUnitType(19) ---@type unittype

UNIT_TYPE_TAUREN = ConvertUnitType(20) ---@type unittype
UNIT_TYPE_POISONED = ConvertUnitType(21) ---@type unittype
UNIT_TYPE_POLYMORPHED = ConvertUnitType(22) ---@type unittype
UNIT_TYPE_SLEEPING = ConvertUnitType(23) ---@type unittype
UNIT_TYPE_RESISTANT = ConvertUnitType(24) ---@type unittype
UNIT_TYPE_ETHEREAL = ConvertUnitType(25) ---@type unittype
UNIT_TYPE_MAGIC_IMMUNE = ConvertUnitType(26) ---@type unittype

-- ===================================================
-- Projectile Type Constants for use with IsProjectileType()
-- ===================================================

PROJECTILE_TYPE_BULLET = ConvertProjectileType(0) ---@type projectiletype @UjAPI
PROJECTILE_TYPE_MISSILE = ConvertProjectileType(1) ---@type projectiletype @UjAPI
PROJECTILE_TYPE_ARTILLERY = ConvertProjectileType(2) ---@type projectiletype @UjAPI

PROJECTILE_TYPE_VISIBLE = ConvertProjectileType(4) ---@type projectiletype @UjAPI
PROJECTILE_TYPE_DEAD = ConvertProjectileType(5) ---@type projectiletype @UjAPI

-- ===================================================
-- Unit Type Constants for use with ChooseRandomItemEx()
-- ===================================================

ITEM_TYPE_PERMANENT = ConvertItemType(0) ---@type itemtype
ITEM_TYPE_CHARGED = ConvertItemType(1) ---@type itemtype
ITEM_TYPE_POWERUP = ConvertItemType(2) ---@type itemtype
ITEM_TYPE_ARTIFACT = ConvertItemType(3) ---@type itemtype
ITEM_TYPE_PURCHASABLE = ConvertItemType(4) ---@type itemtype
ITEM_TYPE_CAMPAIGN = ConvertItemType(5) ---@type itemtype
ITEM_TYPE_MISCELLANEOUS = ConvertItemType(6) ---@type itemtype
ITEM_TYPE_UNKNOWN = ConvertItemType(7) ---@type itemtype
ITEM_TYPE_ANY = ConvertItemType(8) ---@type itemtype

-- Deprecated, should use ITEM_TYPE_POWERUP
ITEM_TYPE_TOME = ConvertItemType(2) ---@type itemtype

-- ===================================================
-- Animatable Camera Fields
-- ===================================================

CAMERA_FIELD_TARGET_DISTANCE = ConvertCameraField(0) ---@type camerafield
CAMERA_FIELD_FARZ = ConvertCameraField(1) ---@type camerafield
CAMERA_FIELD_ANGLE_OF_ATTACK = ConvertCameraField(2) ---@type camerafield
CAMERA_FIELD_FIELD_OF_VIEW = ConvertCameraField(3) ---@type camerafield
CAMERA_FIELD_ROLL = ConvertCameraField(4) ---@type camerafield
CAMERA_FIELD_ROTATION = ConvertCameraField(5) ---@type camerafield
CAMERA_FIELD_ZOFFSET = ConvertCameraField(6) ---@type camerafield
CAMERA_FIELD_NEARZ = ConvertCameraField(7) ---@type camerafield @UjAPI
CAMERA_FIELD_LOCAL_PITCH = ConvertCameraField(8) ---@type camerafield @UjAPI
CAMERA_FIELD_LOCAL_YAW = ConvertCameraField(9) ---@type camerafield @UjAPI
CAMERA_FIELD_LOCAL_ROLL = ConvertCameraField(10) ---@type camerafield @UjAPI

BLEND_MODE_NONE = ConvertBlendMode(0) ---@type blendmode
BLEND_MODE_DONT_CARE = ConvertBlendMode(0) ---@type blendmode
BLEND_MODE_KEYALPHA = ConvertBlendMode(1) ---@type blendmode
BLEND_MODE_BLEND = ConvertBlendMode(2) ---@type blendmode
BLEND_MODE_ADDITIVE = ConvertBlendMode(3) ---@type blendmode
BLEND_MODE_MODULATE = ConvertBlendMode(4) ---@type blendmode
BLEND_MODE_MODULATE_2X = ConvertBlendMode(5) ---@type blendmode

RARITY_FREQUENT = ConvertRarityControl(0) ---@type raritycontrol
RARITY_RARE = ConvertRarityControl(1) ---@type raritycontrol
RARITY_QUEUE = ConvertRarityControl(2) ---@type raritycontrol @UjAPI

TEXMAP_FLAG_NONE = ConvertTexMapFlags(0) ---@type texmapflags
TEXMAP_FLAG_WRAP_U = ConvertTexMapFlags(1) ---@type texmapflags
TEXMAP_FLAG_WRAP_V = ConvertTexMapFlags(2) ---@type texmapflags
TEXMAP_FLAG_WRAP_UV = ConvertTexMapFlags(3) ---@type texmapflags

FOG_OF_WAR_MASKED = ConvertFogState(1) ---@type fogstate
FOG_OF_WAR_FOGGED = ConvertFogState(2) ---@type fogstate
FOG_OF_WAR_VISIBLE = ConvertFogState(4) ---@type fogstate
FOG_OF_WAR_RECT = ConvertFogState(8) ---@type fogstate @UjAPI
FOG_OF_WAR_RADIUS = ConvertFogState(16) ---@type fogstate @UjAPI
FOG_OF_WAR_RADIUS_LOCATION = ConvertFogState(32) ---@type fogstate @UjAPI
FOG_OF_WAR_STARTED = ConvertFogState(64) ---@type fogstate @UjAPI
FOG_OF_WAR_USE_SHARED_VISION = ConvertFogState(128) ---@type fogstate @UjAPI
FOG_OF_WAR_AFTER_UNITS = ConvertFogState(256) ---@type fogstate @UjAPI

-- ===================================================
-- Camera Margin constants for use with GetCameraMargin
-- ===================================================

CAMERA_MARGIN_LEFT = 0 ---@type integer
CAMERA_MARGIN_RIGHT = 1 ---@type integer
CAMERA_MARGIN_TOP = 2 ---@type integer
CAMERA_MARGIN_BOTTOM = 3 ---@type integer

-- ===================================================
-- Effect API constants
-- ===================================================

EFFECT_TYPE_EFFECT = ConvertEffectType(0) ---@type effecttype
EFFECT_TYPE_TARGET = ConvertEffectType(1) ---@type effecttype
EFFECT_TYPE_CASTER = ConvertEffectType(2) ---@type effecttype
EFFECT_TYPE_SPECIAL = ConvertEffectType(3) ---@type effecttype
EFFECT_TYPE_AREA_EFFECT = ConvertEffectType(4) ---@type effecttype
EFFECT_TYPE_MISSILE = ConvertEffectType(5) ---@type effecttype
EFFECT_TYPE_LIGHTNING = ConvertEffectType(6) ---@type effecttype

SOUND_TYPE_EFFECT = ConvertSoundType(0) ---@type soundtype
SOUND_TYPE_EFFECT_LOOPED = ConvertSoundType(1) ---@type soundtype

-- ===================================================
-- Custom UI API constants
-- ===================================================

ORIGIN_FRAME_GAME_UI = ConvertOriginFrameType(0) ---@type originframetype @UjAPI
ORIGIN_FRAME_COMMAND_BUTTON = ConvertOriginFrameType(1) ---@type originframetype @UjAPI
ORIGIN_FRAME_HERO_BAR = ConvertOriginFrameType(2) ---@type originframetype @UjAPI
ORIGIN_FRAME_HERO_BUTTON = ConvertOriginFrameType(3) ---@type originframetype @UjAPI
ORIGIN_FRAME_HERO_HP_BAR = ConvertOriginFrameType(4) ---@type originframetype @UjAPI
ORIGIN_FRAME_HERO_MANA_BAR = ConvertOriginFrameType(5) ---@type originframetype @UjAPI
ORIGIN_FRAME_HERO_BUTTON_INDICATOR = ConvertOriginFrameType(6) ---@type originframetype @UjAPI
ORIGIN_FRAME_ITEM_BUTTON = ConvertOriginFrameType(7) ---@type originframetype @UjAPI
ORIGIN_FRAME_MINIMAP = ConvertOriginFrameType(8) ---@type originframetype @UjAPI
ORIGIN_FRAME_MINIMAP_BUTTON = ConvertOriginFrameType(9) ---@type originframetype @UjAPI
ORIGIN_FRAME_SYSTEM_BUTTON = ConvertOriginFrameType(10) ---@type originframetype @UjAPI
ORIGIN_FRAME_TOOLTIP = ConvertOriginFrameType(11) ---@type originframetype @UjAPI
ORIGIN_FRAME_UBERTOOLTIP = ConvertOriginFrameType(12) ---@type originframetype @UjAPI
ORIGIN_FRAME_CHAT_MSG = ConvertOriginFrameType(13) ---@type originframetype @UjAPI
ORIGIN_FRAME_UNIT_MSG = ConvertOriginFrameType(14) ---@type originframetype @UjAPI
ORIGIN_FRAME_TOP_MSG = ConvertOriginFrameType(15) ---@type originframetype @UjAPI
ORIGIN_FRAME_PORTRAIT = ConvertOriginFrameType(16) ---@type originframetype @UjAPI
ORIGIN_FRAME_WORLD_FRAME = ConvertOriginFrameType(17) ---@type originframetype @UjAPI
ORIGIN_FRAME_CONSOLE_UI = ConvertOriginFrameType(18) ---@type originframetype @UjAPI
ORIGIN_FRAME_PORTRAIT_TEXT = ConvertOriginFrameType(19) ---@type originframetype @UjAPI
ORIGIN_FRAME_BUFF_BAR = ConvertOriginFrameType(20) ---@type originframetype @UjAPI
ORIGIN_FRAME_BUFF_BAR_TEXT = ConvertOriginFrameType(21) ---@type originframetype @UjAPI
ORIGIN_FRAME_BUFF_BAR_INDICATOR = ConvertOriginFrameType(22) ---@type originframetype @UjAPI
ORIGIN_FRAME_TIME_OF_DAY_INDICATOR = ConvertOriginFrameType(23) ---@type originframetype @UjAPI
ORIGIN_FRAME_LEADERBOARD = ConvertOriginFrameType(24) ---@type originframetype @UjAPI
ORIGIN_FRAME_MULTIBOARD = ConvertOriginFrameType(25) ---@type originframetype @UjAPI
ORIGIN_FRAME_INFO_BAR = ConvertOriginFrameType(26) ---@type originframetype @UjAPI
ORIGIN_FRAME_COMMAND_BAR = ConvertOriginFrameType(27) ---@type originframetype @UjAPI
ORIGIN_FRAME_RESOURCE_BAR = ConvertOriginFrameType(28) ---@type originframetype @UjAPI
ORIGIN_FRAME_RESOURCE_BAR_TEXTURE = ConvertOriginFrameType(29) ---@type originframetype @UjAPI
ORIGIN_FRAME_RESOURCE_BAR_TEXT = ConvertOriginFrameType(30) ---@type originframetype @UjAPI
ORIGIN_FRAME_UPPERBUTTON_BAR = ConvertOriginFrameType(31) ---@type originframetype @UjAPI
ORIGIN_FRAME_UPPERBUTTON_BAR_BUTTON = ConvertOriginFrameType(32) ---@type originframetype @UjAPI
ORIGIN_FRAME_PEON_BAR = ConvertOriginFrameType(33) ---@type originframetype @UjAPI
ORIGIN_FRAME_PLAYER_MESSAGE = ConvertOriginFrameType(34) ---@type originframetype @UjAPI
ORIGIN_FRAME_UNIT_MESSAGE = ConvertOriginFrameType(35) ---@type originframetype @UjAPI
ORIGIN_FRAME_CHAT_MESSAGE = ConvertOriginFrameType(36) ---@type originframetype @UjAPI
ORIGIN_FRAME_TOP_MESSAGE = ConvertOriginFrameType(37) ---@type originframetype @UjAPI
ORIGIN_FRAME_CHAT_EDITBAR = ConvertOriginFrameType(38) ---@type originframetype @UjAPI
ORIGIN_FRAME_CINEMATIC_PANEL = ConvertOriginFrameType(39) ---@type originframetype @UjAPI
ORIGIN_FRAME_COMMAND_BUTTON_COOLDOWN_INDICATOR = ConvertOriginFrameType(40) ---@type originframetype @UjAPI
ORIGIN_FRAME_COMMAND_BUTTON_AUTOCAST_FRAME = ConvertOriginFrameType(41) ---@type originframetype @UjAPI
ORIGIN_FRAME_COMMAND_BUTTON_CHARGES_FRAME = ConvertOriginFrameType(42) ---@type originframetype @UjAPI
ORIGIN_FRAME_COMMAND_BUTTON_CHARGES_TEXT = ConvertOriginFrameType(43) ---@type originframetype @UjAPI
ORIGIN_FRAME_CURSOR_FRAME = ConvertOriginFrameType(44) ---@type originframetype @UjAPI
ORIGIN_FRAME_INVENTORY_COVER_FRAME = ConvertOriginFrameType(45) ---@type originframetype @UjAPI
ORIGIN_FRAME_UNIT_TIP = ConvertOriginFrameType(46) ---@type originframetype @UjAPI
ORIGIN_FRAME_ITEM_BUTTON_COOLDOWN_INDICATOR = ConvertOriginFrameType(47) ---@type originframetype @UjAPI
ORIGIN_FRAME_ITEM_BUTTON_AUTOCAST_FRAME = ConvertOriginFrameType(48) ---@type originframetype @UjAPI
ORIGIN_FRAME_ITEM_BUTTON_CHARGES_FRAME = ConvertOriginFrameType(49) ---@type originframetype @UjAPI
ORIGIN_FRAME_ITEM_BUTTON_CHARGES_TEXT = ConvertOriginFrameType(50) ---@type originframetype @UjAPI
ORIGIN_FRAME_TRAINABLE_BUTTON = ConvertOriginFrameType(51) ---@type originframetype @UjAPI
ORIGIN_FRAME_CARGO_BUTTON = ConvertOriginFrameType(52) ---@type originframetype @UjAPI
ORIGIN_FRAME_GROUP_BUTTON = ConvertOriginFrameType(53) ---@type originframetype @UjAPI
ORIGIN_FRAME_FPS_TEXT = ConvertOriginFrameType(54) ---@type originframetype @UjAPI
ORIGIN_FRAME_MEMORY_TEXT = ConvertOriginFrameType(55) ---@type originframetype @UjAPI
ORIGIN_FRAME_SIMPLE_TOP = ConvertOriginFrameType(56) ---@type originframetype

FRAMEPOINT_TOPLEFT = ConvertFramePointType(0) ---@type framepointtype @UjAPI
FRAMEPOINT_TOP = ConvertFramePointType(1) ---@type framepointtype @UjAPI
FRAMEPOINT_TOPRIGHT = ConvertFramePointType(2) ---@type framepointtype @UjAPI
FRAMEPOINT_LEFT = ConvertFramePointType(3) ---@type framepointtype @UjAPI
FRAMEPOINT_CENTER = ConvertFramePointType(4) ---@type framepointtype @UjAPI
FRAMEPOINT_RIGHT = ConvertFramePointType(5) ---@type framepointtype @UjAPI
FRAMEPOINT_BOTTOMLEFT = ConvertFramePointType(6) ---@type framepointtype @UjAPI
FRAMEPOINT_BOTTOM = ConvertFramePointType(7) ---@type framepointtype @UjAPI
FRAMEPOINT_BOTTOMRIGHT = ConvertFramePointType(8) ---@type framepointtype @UjAPI

TEXT_JUSTIFY_TOP = ConvertTextAlignType(0) ---@type textaligntype @UjAPI
TEXT_JUSTIFY_MIDDLE = ConvertTextAlignType(1) ---@type textaligntype @UjAPI
TEXT_JUSTIFY_BOTTOM = ConvertTextAlignType(2) ---@type textaligntype @UjAPI
TEXT_JUSTIFY_LEFT = ConvertTextAlignType(3) ---@type textaligntype @UjAPI
TEXT_JUSTIFY_CENTER = ConvertTextAlignType(4) ---@type textaligntype @UjAPI
TEXT_JUSTIFY_RIGHT = ConvertTextAlignType(5) ---@type textaligntype @UjAPI

FRAMEEVENT_CONTROL_CLICK = ConvertFrameEventType(1) ---@type frameeventtype @UjAPI
FRAMEEVENT_MOUSE_ENTER = ConvertFrameEventType(2) ---@type frameeventtype @UjAPI
FRAMEEVENT_MOUSE_LEAVE = ConvertFrameEventType(3) ---@type frameeventtype @UjAPI
FRAMEEVENT_MOUSE_UP = ConvertFrameEventType(4) ---@type frameeventtype @UjAPI
FRAMEEVENT_MOUSE_DOWN = ConvertFrameEventType(5) ---@type frameeventtype @UjAPI
FRAMEEVENT_MOUSE_WHEEL = ConvertFrameEventType(6) ---@type frameeventtype @UjAPI
FRAMEEVENT_CHECKBOX_CHECKED = ConvertFrameEventType(7) ---@type frameeventtype @UjAPI
FRAMEEVENT_CHECKBOX_UNCHECKED = ConvertFrameEventType(8) ---@type frameeventtype @UjAPI
FRAMEEVENT_EDITBOX_TEXT_CHANGED = ConvertFrameEventType(9) ---@type frameeventtype @UjAPI
FRAMEEVENT_POPUPMENU_ITEM_CHANGED = ConvertFrameEventType(10) ---@type frameeventtype @UjAPI
FRAMEEVENT_FRAME_ITEM_CHANGED = ConvertFrameEventType(10) ---@type frameeventtype @UjAPI
FRAMEEVENT_MOUSE_DOUBLECLICK = ConvertFrameEventType(11) ---@type frameeventtype @UjAPI
FRAMEEVENT_SPRITE_ANIM_UPDATE = ConvertFrameEventType(12) ---@type frameeventtype @UjAPI
FRAMEEVENT_SLIDER_VALUE_CHANGED = ConvertFrameEventType(13) ---@type frameeventtype @UjAPI
FRAMEEVENT_DIALOG_CANCEL = ConvertFrameEventType(14) ---@type frameeventtype @UjAPI
FRAMEEVENT_DIALOG_ACCEPT = ConvertFrameEventType(15) ---@type frameeventtype @UjAPI
FRAMEEVENT_EDITBOX_ENTER = ConvertFrameEventType(16) ---@type frameeventtype @UjAPI
FRAMEEVENT_CHECKBOX_CHANGED = ConvertFrameEventType(17) ---@type frameeventtype @UjAPI
FRAMEEVENT_CONTROL_RELEASE = ConvertFrameEventType(18) ---@type frameeventtype @UjAPI
FRAMEEVENT_CONTROL_DRAG = ConvertFrameEventType(19) ---@type frameeventtype @UjAPI

-- ===================================================
-- OS Key constants
-- ===================================================

OSKEY_LBUTTON = ConvertOsKeyType(1) ---@type oskeytype @UjAPI
OSKEY_RBUTTON = ConvertOsKeyType(2) ---@type oskeytype @UjAPI
OSKEY_CANCEL = ConvertOsKeyType(3) ---@type oskeytype @UjAPI
OSKEY_MBUTTON = ConvertOsKeyType(4) ---@type oskeytype @UjAPI
OSKEY_XBUTTON1 = ConvertOsKeyType(5) ---@type oskeytype @UjAPI
OSKEY_XBUTTON2 = ConvertOsKeyType(6) ---@type oskeytype @UjAPI
OSKEY_UNDEFINED = ConvertOsKeyType(7) ---@type oskeytype @UjAPI
OSKEY_BACKSPACE = ConvertOsKeyType(8) ---@type oskeytype @UjAPI
OSKEY_TAB = ConvertOsKeyType(9) ---@type oskeytype @UjAPI
OSKEY_CLEAR = ConvertOsKeyType(12) ---@type oskeytype @UjAPI
OSKEY_RETURN = ConvertOsKeyType(13) ---@type oskeytype @UjAPI
OSKEY_SHIFT = ConvertOsKeyType(16) ---@type oskeytype @UjAPI
OSKEY_CONTROL = ConvertOsKeyType(17) ---@type oskeytype @UjAPI
OSKEY_ALT = ConvertOsKeyType(18) ---@type oskeytype @UjAPI
OSKEY_PAUSE = ConvertOsKeyType(19) ---@type oskeytype @UjAPI
OSKEY_CAPSLOCK = ConvertOsKeyType(20) ---@type oskeytype @UjAPI
OSKEY_KANA = ConvertOsKeyType(21) ---@type oskeytype @UjAPI
OSKEY_HANGUL = ConvertOsKeyType(21) ---@type oskeytype @UjAPI
OSKEY_JUNJA = ConvertOsKeyType(23) ---@type oskeytype @UjAPI
OSKEY_FINAL = ConvertOsKeyType(24) ---@type oskeytype @UjAPI
OSKEY_HANJA = ConvertOsKeyType(25) ---@type oskeytype @UjAPI
OSKEY_KANJI = ConvertOsKeyType(25) ---@type oskeytype @UjAPI
OSKEY_ESCAPE = ConvertOsKeyType(27) ---@type oskeytype @UjAPI
OSKEY_CONVERT = ConvertOsKeyType(28) ---@type oskeytype @UjAPI
OSKEY_NONCONVERT = ConvertOsKeyType(29) ---@type oskeytype @UjAPI
OSKEY_ACCEPT = ConvertOsKeyType(30) ---@type oskeytype @UjAPI
OSKEY_MODECHANGE = ConvertOsKeyType(31) ---@type oskeytype @UjAPI
OSKEY_SPACE = ConvertOsKeyType(32) ---@type oskeytype @UjAPI
OSKEY_PAGEUP = ConvertOsKeyType(33) ---@type oskeytype @UjAPI
OSKEY_PAGEDOWN = ConvertOsKeyType(34) ---@type oskeytype @UjAPI
OSKEY_END = ConvertOsKeyType(35) ---@type oskeytype @UjAPI
OSKEY_HOME = ConvertOsKeyType(36) ---@type oskeytype @UjAPI
OSKEY_LEFT = ConvertOsKeyType(37) ---@type oskeytype @UjAPI
OSKEY_UP = ConvertOsKeyType(38) ---@type oskeytype @UjAPI
OSKEY_RIGHT = ConvertOsKeyType(39) ---@type oskeytype @UjAPI
OSKEY_DOWN = ConvertOsKeyType(40) ---@type oskeytype @UjAPI
OSKEY_SELECT = ConvertOsKeyType(41) ---@type oskeytype @UjAPI
OSKEY_PRINT = ConvertOsKeyType(42) ---@type oskeytype @UjAPI
OSKEY_EXECUTE = ConvertOsKeyType(43) ---@type oskeytype @UjAPI
OSKEY_PRINTSCREEN = ConvertOsKeyType(44) ---@type oskeytype @UjAPI
OSKEY_INSERT = ConvertOsKeyType(45) ---@type oskeytype @UjAPI
OSKEY_DELETE = ConvertOsKeyType(46) ---@type oskeytype @UjAPI
OSKEY_HELP = ConvertOsKeyType(47) ---@type oskeytype @UjAPI
OSKEY_0 = ConvertOsKeyType(48) ---@type oskeytype @UjAPI
OSKEY_1 = ConvertOsKeyType(49) ---@type oskeytype @UjAPI
OSKEY_2 = ConvertOsKeyType(50) ---@type oskeytype @UjAPI
OSKEY_3 = ConvertOsKeyType(51) ---@type oskeytype @UjAPI
OSKEY_4 = ConvertOsKeyType(52) ---@type oskeytype @UjAPI
OSKEY_5 = ConvertOsKeyType(53) ---@type oskeytype @UjAPI
OSKEY_6 = ConvertOsKeyType(54) ---@type oskeytype @UjAPI
OSKEY_7 = ConvertOsKeyType(55) ---@type oskeytype @UjAPI
OSKEY_8 = ConvertOsKeyType(56) ---@type oskeytype @UjAPI
OSKEY_9 = ConvertOsKeyType(57) ---@type oskeytype @UjAPI
OSKEY_A = ConvertOsKeyType(65) ---@type oskeytype @UjAPI
OSKEY_B = ConvertOsKeyType(66) ---@type oskeytype @UjAPI
OSKEY_C = ConvertOsKeyType(67) ---@type oskeytype @UjAPI
OSKEY_D = ConvertOsKeyType(68) ---@type oskeytype @UjAPI
OSKEY_E = ConvertOsKeyType(69) ---@type oskeytype @UjAPI
OSKEY_F = ConvertOsKeyType(70) ---@type oskeytype @UjAPI
OSKEY_G = ConvertOsKeyType(71) ---@type oskeytype @UjAPI
OSKEY_H = ConvertOsKeyType(72) ---@type oskeytype @UjAPI
OSKEY_I = ConvertOsKeyType(73) ---@type oskeytype @UjAPI
OSKEY_J = ConvertOsKeyType(74) ---@type oskeytype @UjAPI
OSKEY_K = ConvertOsKeyType(75) ---@type oskeytype @UjAPI
OSKEY_L = ConvertOsKeyType(76) ---@type oskeytype @UjAPI
OSKEY_M = ConvertOsKeyType(77) ---@type oskeytype @UjAPI
OSKEY_N = ConvertOsKeyType(78) ---@type oskeytype @UjAPI
OSKEY_O = ConvertOsKeyType(79) ---@type oskeytype @UjAPI
OSKEY_P = ConvertOsKeyType(80) ---@type oskeytype @UjAPI
OSKEY_Q = ConvertOsKeyType(81) ---@type oskeytype @UjAPI
OSKEY_R = ConvertOsKeyType(82) ---@type oskeytype @UjAPI
OSKEY_S = ConvertOsKeyType(83) ---@type oskeytype @UjAPI
OSKEY_T = ConvertOsKeyType(84) ---@type oskeytype @UjAPI
OSKEY_U = ConvertOsKeyType(85) ---@type oskeytype @UjAPI
OSKEY_V = ConvertOsKeyType(86) ---@type oskeytype @UjAPI
OSKEY_W = ConvertOsKeyType(87) ---@type oskeytype @UjAPI
OSKEY_X = ConvertOsKeyType(88) ---@type oskeytype @UjAPI
OSKEY_Y = ConvertOsKeyType(89) ---@type oskeytype @UjAPI
OSKEY_Z = ConvertOsKeyType(90) ---@type oskeytype @UjAPI
OSKEY_LMETA = ConvertOsKeyType(91) ---@type oskeytype @UjAPI
OSKEY_RMETA = ConvertOsKeyType(92) ---@type oskeytype @UjAPI
OSKEY_APPS = ConvertOsKeyType(93) ---@type oskeytype @UjAPI
OSKEY_SLEEP = ConvertOsKeyType(95) ---@type oskeytype @UjAPI
OSKEY_NUMPAD0 = ConvertOsKeyType(96) ---@type oskeytype @UjAPI
OSKEY_NUMPAD1 = ConvertOsKeyType(97) ---@type oskeytype @UjAPI
OSKEY_NUMPAD2 = ConvertOsKeyType(98) ---@type oskeytype @UjAPI
OSKEY_NUMPAD3 = ConvertOsKeyType(99) ---@type oskeytype @UjAPI
OSKEY_NUMPAD4 = ConvertOsKeyType(100) ---@type oskeytype @UjAPI
OSKEY_NUMPAD5 = ConvertOsKeyType(101) ---@type oskeytype @UjAPI
OSKEY_NUMPAD6 = ConvertOsKeyType(102) ---@type oskeytype @UjAPI
OSKEY_NUMPAD7 = ConvertOsKeyType(103) ---@type oskeytype @UjAPI
OSKEY_NUMPAD8 = ConvertOsKeyType(104) ---@type oskeytype @UjAPI
OSKEY_NUMPAD9 = ConvertOsKeyType(105) ---@type oskeytype @UjAPI
OSKEY_MULTIPLY = ConvertOsKeyType(106) ---@type oskeytype @UjAPI
OSKEY_ADD = ConvertOsKeyType(107) ---@type oskeytype @UjAPI
OSKEY_SEPARATOR = ConvertOsKeyType(108) ---@type oskeytype @UjAPI
OSKEY_SUBTRACT = ConvertOsKeyType(109) ---@type oskeytype @UjAPI
OSKEY_DECIMAL = ConvertOsKeyType(110) ---@type oskeytype @UjAPI
OSKEY_DIVIDE = ConvertOsKeyType(111) ---@type oskeytype @UjAPI
OSKEY_F1 = ConvertOsKeyType(112) ---@type oskeytype @UjAPI
OSKEY_F2 = ConvertOsKeyType(113) ---@type oskeytype @UjAPI
OSKEY_F3 = ConvertOsKeyType(114) ---@type oskeytype @UjAPI
OSKEY_F4 = ConvertOsKeyType(115) ---@type oskeytype @UjAPI
OSKEY_F5 = ConvertOsKeyType(116) ---@type oskeytype @UjAPI
OSKEY_F6 = ConvertOsKeyType(117) ---@type oskeytype @UjAPI
OSKEY_F7 = ConvertOsKeyType(118) ---@type oskeytype @UjAPI
OSKEY_F8 = ConvertOsKeyType(119) ---@type oskeytype @UjAPI
OSKEY_F9 = ConvertOsKeyType(120) ---@type oskeytype @UjAPI
OSKEY_F10 = ConvertOsKeyType(121) ---@type oskeytype @UjAPI
OSKEY_F11 = ConvertOsKeyType(122) ---@type oskeytype @UjAPI
OSKEY_F12 = ConvertOsKeyType(123) ---@type oskeytype @UjAPI
OSKEY_F13 = ConvertOsKeyType(124) ---@type oskeytype @UjAPI
OSKEY_F14 = ConvertOsKeyType(125) ---@type oskeytype @UjAPI
OSKEY_F15 = ConvertOsKeyType(126) ---@type oskeytype @UjAPI
OSKEY_F16 = ConvertOsKeyType(127) ---@type oskeytype @UjAPI
OSKEY_F17 = ConvertOsKeyType(128) ---@type oskeytype @UjAPI
OSKEY_F18 = ConvertOsKeyType(129) ---@type oskeytype @UjAPI
OSKEY_F19 = ConvertOsKeyType(130) ---@type oskeytype @UjAPI
OSKEY_F20 = ConvertOsKeyType(131) ---@type oskeytype @UjAPI
OSKEY_F21 = ConvertOsKeyType(132) ---@type oskeytype @UjAPI
OSKEY_F22 = ConvertOsKeyType(133) ---@type oskeytype @UjAPI
OSKEY_F23 = ConvertOsKeyType(134) ---@type oskeytype @UjAPI
OSKEY_F24 = ConvertOsKeyType(135) ---@type oskeytype @UjAPI
OSKEY_NUMLOCK = ConvertOsKeyType(144) ---@type oskeytype @UjAPI
OSKEY_SCROLLLOCK = ConvertOsKeyType(145) ---@type oskeytype @UjAPI
OSKEY_OEM_NEC_EQUAL = ConvertOsKeyType(146) ---@type oskeytype @UjAPI
OSKEY_OEM_FJ_JISHO = ConvertOsKeyType(146) ---@type oskeytype @UjAPI
OSKEY_OEM_FJ_MASSHOU = ConvertOsKeyType(147) ---@type oskeytype @UjAPI
OSKEY_OEM_FJ_TOUROKU = ConvertOsKeyType(148) ---@type oskeytype @UjAPI
OSKEY_OEM_FJ_LOYA = ConvertOsKeyType(149) ---@type oskeytype @UjAPI
OSKEY_OEM_FJ_ROYA = ConvertOsKeyType(150) ---@type oskeytype @UjAPI
OSKEY_LSHIFT = ConvertOsKeyType(160) ---@type oskeytype @UjAPI
OSKEY_RSHIFT = ConvertOsKeyType(161) ---@type oskeytype @UjAPI
OSKEY_LCONTROL = ConvertOsKeyType(162) ---@type oskeytype @UjAPI
OSKEY_RCONTROL = ConvertOsKeyType(163) ---@type oskeytype @UjAPI
OSKEY_LALT = ConvertOsKeyType(164) ---@type oskeytype @UjAPI
OSKEY_RALT = ConvertOsKeyType(165) ---@type oskeytype @UjAPI
OSKEY_BROWSER_BACK = ConvertOsKeyType(166) ---@type oskeytype @UjAPI
OSKEY_BROWSER_FORWARD = ConvertOsKeyType(167) ---@type oskeytype @UjAPI
OSKEY_BROWSER_REFRESH = ConvertOsKeyType(168) ---@type oskeytype @UjAPI
OSKEY_BROWSER_STOP = ConvertOsKeyType(169) ---@type oskeytype @UjAPI
OSKEY_BROWSER_SEARCH = ConvertOsKeyType(170) ---@type oskeytype @UjAPI
OSKEY_BROWSER_FAVORITES = ConvertOsKeyType(171) ---@type oskeytype @UjAPI
OSKEY_BROWSER_HOME = ConvertOsKeyType(172) ---@type oskeytype @UjAPI
OSKEY_VOLUME_MUTE = ConvertOsKeyType(173) ---@type oskeytype @UjAPI
OSKEY_VOLUME_DOWN = ConvertOsKeyType(174) ---@type oskeytype @UjAPI
OSKEY_VOLUME_UP = ConvertOsKeyType(175) ---@type oskeytype @UjAPI
OSKEY_MEDIA_NEXT_TRACK = ConvertOsKeyType(176) ---@type oskeytype @UjAPI
OSKEY_MEDIA_PREV_TRACK = ConvertOsKeyType(177) ---@type oskeytype @UjAPI
OSKEY_MEDIA_STOP = ConvertOsKeyType(178) ---@type oskeytype @UjAPI
OSKEY_MEDIA_PLAY_PAUSE = ConvertOsKeyType(179) ---@type oskeytype @UjAPI
OSKEY_LAUNCH_MAIL = ConvertOsKeyType(180) ---@type oskeytype @UjAPI
OSKEY_LAUNCH_MEDIA_SELECT = ConvertOsKeyType(181) ---@type oskeytype @UjAPI
OSKEY_LAUNCH_APP1 = ConvertOsKeyType(182) ---@type oskeytype @UjAPI
OSKEY_LAUNCH_APP2 = ConvertOsKeyType(183) ---@type oskeytype @UjAPI
OSKEY_OEM_1 = ConvertOsKeyType(186) ---@type oskeytype @UjAPI
OSKEY_OEM_PLUS = ConvertOsKeyType(187) ---@type oskeytype @UjAPI
OSKEY_OEM_COMMA = ConvertOsKeyType(188) ---@type oskeytype @UjAPI
OSKEY_OEM_MINUS = ConvertOsKeyType(189) ---@type oskeytype @UjAPI
OSKEY_OEM_PERIOD = ConvertOsKeyType(190) ---@type oskeytype @UjAPI
OSKEY_OEM_2 = ConvertOsKeyType(191) ---@type oskeytype @UjAPI
OSKEY_OEM_3 = ConvertOsKeyType(192) ---@type oskeytype @UjAPI
OSKEY_OEM_4 = ConvertOsKeyType(219) ---@type oskeytype @UjAPI
OSKEY_OEM_5 = ConvertOsKeyType(220) ---@type oskeytype @UjAPI
OSKEY_OEM_6 = ConvertOsKeyType(221) ---@type oskeytype @UjAPI
OSKEY_OEM_7 = ConvertOsKeyType(222) ---@type oskeytype @UjAPI
OSKEY_OEM_8 = ConvertOsKeyType(223) ---@type oskeytype @UjAPI
OSKEY_OEM_AX = ConvertOsKeyType(225) ---@type oskeytype @UjAPI
OSKEY_OEM_102 = ConvertOsKeyType(226) ---@type oskeytype @UjAPI
OSKEY_ICO_HELP = ConvertOsKeyType(227) ---@type oskeytype @UjAPI
OSKEY_ICO_00 = ConvertOsKeyType(228) ---@type oskeytype @UjAPI
OSKEY_PROCESSKEY = ConvertOsKeyType(229) ---@type oskeytype @UjAPI
OSKEY_ICO_CLEAR = ConvertOsKeyType(230) ---@type oskeytype @UjAPI
OSKEY_PACKET = ConvertOsKeyType(231) ---@type oskeytype @UjAPI
OSKEY_OEM_RESET = ConvertOsKeyType(233) ---@type oskeytype @UjAPI
OSKEY_OEM_JUMP = ConvertOsKeyType(234) ---@type oskeytype @UjAPI
OSKEY_OEM_PA1 = ConvertOsKeyType(235) ---@type oskeytype @UjAPI
OSKEY_OEM_PA2 = ConvertOsKeyType(236) ---@type oskeytype @UjAPI
OSKEY_OEM_PA3 = ConvertOsKeyType(237) ---@type oskeytype @UjAPI
OSKEY_OEM_WSCTRL = ConvertOsKeyType(238) ---@type oskeytype @UjAPI
OSKEY_OEM_CUSEL = ConvertOsKeyType(239) ---@type oskeytype @UjAPI
OSKEY_OEM_ATTN = ConvertOsKeyType(240) ---@type oskeytype @UjAPI
OSKEY_OEM_FINISH = ConvertOsKeyType(241) ---@type oskeytype @UjAPI
OSKEY_OEM_COPY = ConvertOsKeyType(242) ---@type oskeytype @UjAPI
OSKEY_OEM_AUTO = ConvertOsKeyType(243) ---@type oskeytype @UjAPI
OSKEY_OEM_ENLW = ConvertOsKeyType(244) ---@type oskeytype @UjAPI
OSKEY_OEM_BACKTAB = ConvertOsKeyType(245) ---@type oskeytype @UjAPI
OSKEY_ATTN = ConvertOsKeyType(246) ---@type oskeytype @UjAPI
OSKEY_CRSEL = ConvertOsKeyType(247) ---@type oskeytype @UjAPI
OSKEY_EXSEL = ConvertOsKeyType(248) ---@type oskeytype @UjAPI
OSKEY_EREOF = ConvertOsKeyType(249) ---@type oskeytype @UjAPI
OSKEY_PLAY = ConvertOsKeyType(250) ---@type oskeytype @UjAPI
OSKEY_ZOOM = ConvertOsKeyType(251) ---@type oskeytype @UjAPI
OSKEY_NONAME = ConvertOsKeyType(252) ---@type oskeytype @UjAPI
OSKEY_PA1 = ConvertOsKeyType(253) ---@type oskeytype @UjAPI
OSKEY_OEM_CLEAR = ConvertOsKeyType(254) ---@type oskeytype @UjAPI

-- ===================================================
-- Mouse Button constants
-- ===================================================

MOUSE_BUTTON_TYPE_LEFT = ConvertMouseButtonType(1) ---@type mousebuttontype @UjAPI
MOUSE_BUTTON_TYPE_MIDDLE = ConvertMouseButtonType(2) ---@type mousebuttontype @UjAPI
MOUSE_BUTTON_TYPE_RIGHT = ConvertMouseButtonType(3) ---@type mousebuttontype @UjAPI

-- ===================================================
-- Meta Keys constants
-- ===================================================

META_KEY_NONE = 0 ---@type integer @UjAPI
META_KEY_SHIFT = 1 ---@type integer @UjAPI
META_KEY_CONTROL = 2 ---@type integer @UjAPI
META_KEY_ALT = 4 ---@type integer @UjAPI
META_KEY_WINDOWS = 8 ---@type integer @UjAPI
-- To make a "meta key combination" simply add the values you need, so ALT + SHIFT => metaKey = META_KEY_ALT + META_KEY_SHIFT

-- ===================================================
-- Chat Recipient constants
-- ===================================================

CHAT_RECIPIENT_ALL = 0 ---@type integer @UjAPI
CHAT_RECIPIENT_ALLIES = 1 ---@type integer @UjAPI
CHAT_RECIPIENT_REFEREES = 2 ---@type integer @UjAPI
CHAT_RECIPIENT_OBSERVERS = 2 ---@type integer @UjAPI
CHAT_RECIPIENT_PRIVATE = 3 ---@type integer @UjAPI
CHAT_RECIPIENT_UNKNOWN = 4 ---@type integer @UjAPI
-- use CHAT_RECIPIENT_UNKNOWN to ignore text stating to which recipient the text was sent.

-- ===================================================
-- Instanced Object Operation API constants
-- ===================================================

-- Ability
ABILITY_IF_BUTTON_POSITION_NORMAL_X = ConvertAbilityIntegerField(FourCC('abpx'--[[1633841272--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_BUTTON_POSITION_NORMAL_Y = ConvertAbilityIntegerField(FourCC('abpy'--[[1633841273--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_BUTTON_HOTKEY_NORMAL = ConvertAbilityIntegerField(FourCC('ahky'--[[1634233209--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_BUTTON_HOTKEY_ACTIVATED = ConvertAbilityIntegerField(FourCC('auhk'--[[1635084395--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_BUTTON_POSITION_ACTIVATED_X = ConvertAbilityIntegerField(FourCC('aubx'--[[1635082872--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_BUTTON_POSITION_ACTIVATED_Y = ConvertAbilityIntegerField(FourCC('auby'--[[1635082873--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_BUTTON_HOTKEY_RESEARCH = ConvertAbilityIntegerField(FourCC('arhk'--[[1634887787--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_BUTTON_POSITION_RESEARCH_X = ConvertAbilityIntegerField(FourCC('arpx'--[[1634889848--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_BUTTON_POSITION_RESEARCH_Y = ConvertAbilityIntegerField(FourCC('arpy'--[[1634889849--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_BUTTON_HOTKEY_ALL = ConvertAbilityIntegerField(FourCC('ahtk'--[[1634235499--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_BUTTON_POSITION_SPELLBOOK_X = ConvertAbilityIntegerField(FourCC('asbx'--[[1634951800--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_BUTTON_POSITION_SPELLBOOK_Y = ConvertAbilityIntegerField(FourCC('asby'--[[1634951801--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_MISSILE_SPEED = ConvertAbilityIntegerField(FourCC('amsp'--[[1634562928--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_TARGET_ATTACHMENTS = ConvertAbilityIntegerField(FourCC('atac'--[[1635017059--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_CASTER_ATTACHMENTS = ConvertAbilityIntegerField(FourCC('acac'--[[1633902947--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_PRIORITY = ConvertAbilityIntegerField(FourCC('apri'--[[1634759273--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_LEVELS = ConvertAbilityIntegerField(FourCC('alev'--[[1634493814--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_REQUIRED_LEVEL = ConvertAbilityIntegerField(FourCC('arlv'--[[1634888822--]])) ---@type abilityintegerfield @UjAPI
ABILITY_IF_LEVEL_SKIP_REQUIREMENT = ConvertAbilityIntegerField(FourCC('alsk'--[[1634497387--]])) ---@type abilityintegerfield @UjAPI

ABILITY_BF_HERO_ABILITY = ConvertAbilityBooleanField(FourCC('aher'--[[1634231666--]])) ---@type abilitybooleanfield @UjAPI
ABILITY_BF_ITEM_ABILITY = ConvertAbilityBooleanField(FourCC('aite'--[[1634301029--]])) ---@type abilitybooleanfield @UjAPI
ABILITY_BF_CHECK_DEPENDENCIES = ConvertAbilityBooleanField(FourCC('achd'--[[1633904740--]])) ---@type abilitybooleanfield @UjAPI
ABILITY_BF_HOMING = ConvertAbilityBooleanField(FourCC('amho'--[[1634560111--]])) ---@type abilitybooleanfield @UjAPI

ABILITY_RF_ARF_MISSILE_ARC = ConvertAbilityRealField(FourCC('amac'--[[1634558307--]])) ---@type abilityrealfield @UjAPI
ABILITY_RF_AURA_REFRESH_TIME = ConvertAbilityRealField(FourCC('artm'--[[1634890861--]])) ---@type abilityrealfield @UjAPI

ABILITY_SF_NAME = ConvertAbilityStringField(FourCC('anam'--[[1634623853--]])) ---@type abilitystringfield @UjAPI
ABILITY_SF_ICON_NORMAL = ConvertAbilityStringField(FourCC('aart'--[[1633776244--]])) ---@type abilitystringfield @UjAPI
ABILITY_SF_ICON_ACTIVATED = ConvertAbilityStringField(FourCC('auar'--[[1635082610--]])) ---@type abilitystringfield @UjAPI
ABILITY_SF_ICON_RESEARCH = ConvertAbilityStringField(FourCC('arar'--[[1634886002--]])) ---@type abilitystringfield @UjAPI
ABILITY_SF_EFFECT_SOUND = ConvertAbilityStringField(FourCC('aefs'--[[1634035315--]])) ---@type abilitystringfield @UjAPI
ABILITY_SF_EFFECT_SOUND_LOOPING = ConvertAbilityStringField(FourCC('aefl'--[[1634035308--]])) ---@type abilitystringfield @UjAPI

ABILITY_ILF_MANA_COST = ConvertAbilityIntegerLevelField(FourCC('amcs'--[[1634558835--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_WAVES = ConvertAbilityIntegerLevelField(FourCC('Hbz1'--[[1214413361--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_SHARDS = ConvertAbilityIntegerLevelField(FourCC('Hbz3'--[[1214413363--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_UNITS_TELEPORTED = ConvertAbilityIntegerLevelField(FourCC('Hmt1'--[[1215132721--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SUMMONED_UNIT_COUNT_HWE2 = ConvertAbilityIntegerLevelField(FourCC('Hwe2'--[[1215784242--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_IMAGES = ConvertAbilityIntegerLevelField(FourCC('Omi1'--[[1332570417--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_CORPSES_RAISED_UAN1 = ConvertAbilityIntegerLevelField(FourCC('Uan1'--[[1432448561--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MORPHING_FLAGS = ConvertAbilityIntegerLevelField(FourCC('Eme2'--[[1164797234--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_STRENGTH_BONUS_NRG5 = ConvertAbilityIntegerLevelField(FourCC('Nrg5'--[[1316120373--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DEFENSE_BONUS_NRG6 = ConvertAbilityIntegerLevelField(FourCC('Nrg6'--[[1316120374--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_TARGETS_HIT = ConvertAbilityIntegerLevelField(FourCC('Ocl2'--[[1331915826--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DETECTION_TYPE_OFS1 = ConvertAbilityIntegerLevelField(FourCC('Ofs1'--[[1332114225--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_SUMMONED_UNITS_OSF2 = ConvertAbilityIntegerLevelField(FourCC('Osf2'--[[1332962866--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_SUMMONED_UNITS_EFN1 = ConvertAbilityIntegerLevelField(FourCC('Efn1'--[[1164340785--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_CORPSES_RAISED_HRE1 = ConvertAbilityIntegerLevelField(FourCC('Hre1'--[[1215456561--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_STACK_FLAGS = ConvertAbilityIntegerLevelField(FourCC('Hca4'--[[1214472500--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MINIMUM_NUMBER_OF_UNITS = ConvertAbilityIntegerLevelField(FourCC('Ndp2'--[[1315205170--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_NUMBER_OF_UNITS_NDP3 = ConvertAbilityIntegerLevelField(FourCC('Ndp3'--[[1315205171--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_UNITS_CREATED_NRC2 = ConvertAbilityIntegerLevelField(FourCC('Nrc2'--[[1316119346--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SHIELD_LIFE = ConvertAbilityIntegerLevelField(FourCC('Ams3'--[[1097691955--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MANA_LOSS_AMS4 = ConvertAbilityIntegerLevelField(FourCC('Ams4'--[[1097691956--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_GOLD_PER_INTERVAL_BGM1 = ConvertAbilityIntegerLevelField(FourCC('Bgm1'--[[1114074417--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAX_NUMBER_OF_MINERS = ConvertAbilityIntegerLevelField(FourCC('Bgm3'--[[1114074419--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_CARGO_CAPACITY = ConvertAbilityIntegerLevelField(FourCC('Car1'--[[1130459697--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_CREEP_LEVEL_DEV3 = ConvertAbilityIntegerLevelField(FourCC('Dev3'--[[1147500083--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAX_CREEP_LEVEL_DEV1 = ConvertAbilityIntegerLevelField(FourCC('Dev1'--[[1147500081--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_GOLD_PER_INTERVAL_EGM1 = ConvertAbilityIntegerLevelField(FourCC('Egm1'--[[1164406065--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DEFENSE_REDUCTION = ConvertAbilityIntegerLevelField(FourCC('Fae1'--[[1180788017--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DETECTION_TYPE_FLA1 = ConvertAbilityIntegerLevelField(FourCC('Fla1'--[[1181507889--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_FLARE_COUNT = ConvertAbilityIntegerLevelField(FourCC('Fla3'--[[1181507891--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAX_GOLD = ConvertAbilityIntegerLevelField(FourCC('Gld1'--[[1198285873--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MINING_CAPACITY = ConvertAbilityIntegerLevelField(FourCC('Gld3'--[[1198285875--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_NUMBER_OF_CORPSES_GYD1 = ConvertAbilityIntegerLevelField(FourCC('Gyd1'--[[1199137841--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DAMAGE_TO_TREE = ConvertAbilityIntegerLevelField(FourCC('Har1'--[[1214345777--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_LUMBER_CAPACITY = ConvertAbilityIntegerLevelField(FourCC('Har2'--[[1214345778--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_GOLD_CAPACITY = ConvertAbilityIntegerLevelField(FourCC('Har3'--[[1214345779--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DEFENSE_INCREASE_INF2 = ConvertAbilityIntegerLevelField(FourCC('Inf2'--[[1231971890--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_INTERACTION_TYPE = ConvertAbilityIntegerLevelField(FourCC('Neu2'--[[1315271986--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_GOLD_COST_NDT1 = ConvertAbilityIntegerLevelField(FourCC('Ndt1'--[[1315206193--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_LUMBER_COST_NDT2 = ConvertAbilityIntegerLevelField(FourCC('Ndt2'--[[1315206194--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DETECTION_TYPE_NDT3 = ConvertAbilityIntegerLevelField(FourCC('Ndt3'--[[1315206195--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_STACKING_TYPE_POI4 = ConvertAbilityIntegerLevelField(FourCC('Poi4'--[[1349478708--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_STACKING_TYPE_POA5 = ConvertAbilityIntegerLevelField(FourCC('Poa5'--[[1349476661--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_CREEP_LEVEL_PLY1 = ConvertAbilityIntegerLevelField(FourCC('Ply1'--[[1349286193--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_CREEP_LEVEL_POS1 = ConvertAbilityIntegerLevelField(FourCC('Pos1'--[[1349481265--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MOVEMENT_UPDATE_FREQUENCY_PRG1 = ConvertAbilityIntegerLevelField(FourCC('Prg1'--[[1349674801--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ATTACK_UPDATE_FREQUENCY_PRG2 = ConvertAbilityIntegerLevelField(FourCC('Prg2'--[[1349674802--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MANA_LOSS_PRG6 = ConvertAbilityIntegerLevelField(FourCC('Prg6'--[[1349674806--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_UNITS_SUMMONED_TYPE_ONE = ConvertAbilityIntegerLevelField(FourCC('Rai1'--[[1382115633--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_UNITS_SUMMONED_TYPE_TWO = ConvertAbilityIntegerLevelField(FourCC('Rai2'--[[1382115634--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAX_UNITS_SUMMONED = ConvertAbilityIntegerLevelField(FourCC('Ucb5'--[[1432576565--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ALLOW_WHEN_FULL_REJ3 = ConvertAbilityIntegerLevelField(FourCC('Rej3'--[[1382378035--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_UNITS_CHARGED_TO_CASTER = ConvertAbilityIntegerLevelField(FourCC('Rpb5'--[[1383096885--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_UNITS_AFFECTED = ConvertAbilityIntegerLevelField(FourCC('Rpb6'--[[1383096886--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DEFENSE_INCREASE_ROA2 = ConvertAbilityIntegerLevelField(FourCC('Roa2'--[[1383031090--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAX_UNITS_ROA7 = ConvertAbilityIntegerLevelField(FourCC('Roa7'--[[1383031095--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ROOTED_WEAPONS = ConvertAbilityIntegerLevelField(FourCC('Roo1'--[[1383034673--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_UPROOTED_WEAPONS = ConvertAbilityIntegerLevelField(FourCC('Roo2'--[[1383034674--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_UPROOTED_DEFENSE_TYPE = ConvertAbilityIntegerLevelField(FourCC('Roo4'--[[1383034676--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ACCUMULATION_STEP = ConvertAbilityIntegerLevelField(FourCC('Sal2'--[[1398893618--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_OWLS = ConvertAbilityIntegerLevelField(FourCC('Esn4'--[[1165192756--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_STACKING_TYPE_SPO4 = ConvertAbilityIntegerLevelField(FourCC('Spo4'--[[1399877428--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_UNITS = ConvertAbilityIntegerLevelField(FourCC('Sod1'--[[1399809073--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SPIDER_CAPACITY = ConvertAbilityIntegerLevelField(FourCC('Spa1'--[[1399873841--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_INTERVALS_BEFORE_CHANGING_TREES = ConvertAbilityIntegerLevelField(FourCC('Wha2'--[[1466458418--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_AGILITY_BONUS = ConvertAbilityIntegerLevelField(FourCC('Iagi'--[[1231120233--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_INTELLIGENCE_BONUS = ConvertAbilityIntegerLevelField(FourCC('Iint'--[[1231646324--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_STRENGTH_BONUS_ISTR = ConvertAbilityIntegerLevelField(FourCC('Istr'--[[1232303218--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ATTACK_BONUS = ConvertAbilityIntegerLevelField(FourCC('Iatt'--[[1231123572--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DEFENSE_BONUS_IDEF = ConvertAbilityIntegerLevelField(FourCC('Idef'--[[1231316326--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SUMMON_1_AMOUNT = ConvertAbilityIntegerLevelField(FourCC('Isn1'--[[1232301617--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SUMMON_2_AMOUNT = ConvertAbilityIntegerLevelField(FourCC('Isn2'--[[1232301618--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_EXPERIENCE_GAINED = ConvertAbilityIntegerLevelField(FourCC('Ixpg'--[[1232629863--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_HIT_POINTS_GAINED_IHPG = ConvertAbilityIntegerLevelField(FourCC('Ihpg'--[[1231581287--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MANA_POINTS_GAINED_IMPG = ConvertAbilityIntegerLevelField(FourCC('Impg'--[[1231908967--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_HIT_POINTS_GAINED_IHP2 = ConvertAbilityIntegerLevelField(FourCC('Ihp2'--[[1231581234--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MANA_POINTS_GAINED_IMP2 = ConvertAbilityIntegerLevelField(FourCC('Imp2'--[[1231908914--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DAMAGE_BONUS_DICE = ConvertAbilityIntegerLevelField(FourCC('Idic'--[[1231317347--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ARMOR_PENALTY_IARP = ConvertAbilityIntegerLevelField(FourCC('Iarp'--[[1231123056--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ENABLED_ATTACK_INDEX_IOB5 = ConvertAbilityIntegerLevelField(FourCC('Iob5'--[[1232036405--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_LEVELS_GAINED = ConvertAbilityIntegerLevelField(FourCC('Ilev'--[[1231840630--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAX_LIFE_GAINED = ConvertAbilityIntegerLevelField(FourCC('Ilif'--[[1231841638--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAX_MANA_GAINED = ConvertAbilityIntegerLevelField(FourCC('Iman'--[[1231905134--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_GOLD_GIVEN = ConvertAbilityIntegerLevelField(FourCC('Igol'--[[1231515500--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_LUMBER_GIVEN = ConvertAbilityIntegerLevelField(FourCC('Ilum'--[[1231844717--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DETECTION_TYPE_IFA1 = ConvertAbilityIntegerLevelField(FourCC('Ifa1'--[[1231446321--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_CREEP_LEVEL_ICRE = ConvertAbilityIntegerLevelField(FourCC('Icre'--[[1231254117--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MOVEMENT_SPEED_BONUS = ConvertAbilityIntegerLevelField(FourCC('Imvb'--[[1231910498--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_HIT_POINTS_REGENERATED_PER_SECOND = ConvertAbilityIntegerLevelField(FourCC('Ihpr'--[[1231581298--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SIGHT_RANGE_BONUS = ConvertAbilityIntegerLevelField(FourCC('Isib'--[[1232300386--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DAMAGE_PER_DURATION = ConvertAbilityIntegerLevelField(FourCC('Icfd'--[[1231251044--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MANA_USED_PER_SECOND = ConvertAbilityIntegerLevelField(FourCC('Icfm'--[[1231251053--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_EXTRA_MANA_REQUIRED = ConvertAbilityIntegerLevelField(FourCC('Icfx'--[[1231251064--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DETECTION_RADIUS_IDET = ConvertAbilityIntegerLevelField(FourCC('Idet'--[[1231316340--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MANA_LOSS_PER_UNIT_IDIM = ConvertAbilityIntegerLevelField(FourCC('Idim'--[[1231317357--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DAMAGE_TO_SUMMONED_UNITS_IDID = ConvertAbilityIntegerLevelField(FourCC('Idid'--[[1231317348--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_NUMBER_OF_UNITS_IREC = ConvertAbilityIntegerLevelField(FourCC('Irec'--[[1232233827--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DELAY_AFTER_DEATH_SECONDS = ConvertAbilityIntegerLevelField(FourCC('Ircd'--[[1232233316--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_RESTORED_LIFE = ConvertAbilityIntegerLevelField(FourCC('irc2'--[[1769104178--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_RESTORED_MANA_1_FOR_CURRENT = ConvertAbilityIntegerLevelField(FourCC('irc3'--[[1769104179--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_HIT_POINTS_RESTORED = ConvertAbilityIntegerLevelField(FourCC('Ihps'--[[1231581299--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MANA_POINTS_RESTORED = ConvertAbilityIntegerLevelField(FourCC('Imps'--[[1231908979--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_NUMBER_OF_UNITS_ITPM = ConvertAbilityIntegerLevelField(FourCC('Itpm'--[[1232367725--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_CORPSES_RAISED_CAD1 = ConvertAbilityIntegerLevelField(FourCC('Cad1'--[[1130456113--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_TERRAIN_DEFORMATION_DURATION_MS = ConvertAbilityIntegerLevelField(FourCC('Wrs3'--[[1467118387--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_UNITS = ConvertAbilityIntegerLevelField(FourCC('Uds1'--[[1432646449--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DETECTION_TYPE_DET1 = ConvertAbilityIntegerLevelField(FourCC('Det1'--[[1147499569--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_GOLD_COST_PER_STRUCTURE = ConvertAbilityIntegerLevelField(FourCC('Nsp1'--[[1316188209--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_LUMBER_COST_PER_USE = ConvertAbilityIntegerLevelField(FourCC('Nsp2'--[[1316188210--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DETECTION_TYPE_NSP3 = ConvertAbilityIntegerLevelField(FourCC('Nsp3'--[[1316188211--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_SWARM_UNITS = ConvertAbilityIntegerLevelField(FourCC('Uls1'--[[1433170737--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAX_SWARM_UNITS_PER_TARGET = ConvertAbilityIntegerLevelField(FourCC('Uls3'--[[1433170739--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_SUMMONED_UNITS_NBA2 = ConvertAbilityIntegerLevelField(FourCC('Nba2'--[[1315070258--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_CREEP_LEVEL_NCH1 = ConvertAbilityIntegerLevelField(FourCC('Nch1'--[[1315137585--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ATTACKS_PREVENTED = ConvertAbilityIntegerLevelField(FourCC('Nsi1'--[[1316186417--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_NUMBER_OF_TARGETS_EFK3 = ConvertAbilityIntegerLevelField(FourCC('Efk3'--[[1164340019--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_SUMMONED_UNITS_ESV1 = ConvertAbilityIntegerLevelField(FourCC('Esv1'--[[1165194801--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_NUMBER_OF_CORPSES_EXH1 = ConvertAbilityIntegerLevelField(FourCC('exh1'--[[1702389809--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ITEM_CAPACITY = ConvertAbilityIntegerLevelField(FourCC('inv1'--[[1768846897--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_NUMBER_OF_TARGETS_SPL2 = ConvertAbilityIntegerLevelField(FourCC('spl2'--[[1936747570--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ALLOW_WHEN_FULL_IRL3 = ConvertAbilityIntegerLevelField(FourCC('irl3'--[[1769106483--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_DISPELLED_UNITS = ConvertAbilityIntegerLevelField(FourCC('idc3'--[[1768186675--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_LURES = ConvertAbilityIntegerLevelField(FourCC('imo1'--[[1768779569--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NEW_TIME_OF_DAY_HOUR = ConvertAbilityIntegerLevelField(FourCC('ict1'--[[1768125489--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NEW_TIME_OF_DAY_MINUTE = ConvertAbilityIntegerLevelField(FourCC('ict2'--[[1768125490--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_UNITS_CREATED_MEC1 = ConvertAbilityIntegerLevelField(FourCC('mec1'--[[1835361073--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MINIMUM_SPELLS = ConvertAbilityIntegerLevelField(FourCC('spb3'--[[1936745011--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_SPELLS = ConvertAbilityIntegerLevelField(FourCC('spb4'--[[1936745012--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DISABLED_ATTACK_INDEX = ConvertAbilityIntegerLevelField(FourCC('gra3'--[[1735549235--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ENABLED_ATTACK_INDEX_GRA4 = ConvertAbilityIntegerLevelField(FourCC('gra4'--[[1735549236--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAXIMUM_ATTACKS = ConvertAbilityIntegerLevelField(FourCC('gra5'--[[1735549237--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_BUILDING_TYPES_ALLOWED_NPR1 = ConvertAbilityIntegerLevelField(FourCC('Npr1'--[[1315992113--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_BUILDING_TYPES_ALLOWED_NSA1 = ConvertAbilityIntegerLevelField(FourCC('Nsa1'--[[1316184369--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ATTACK_MODIFICATION = ConvertAbilityIntegerLevelField(FourCC('Iaa1'--[[1231118641--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SUMMONED_UNIT_COUNT_NPA5 = ConvertAbilityIntegerLevelField(FourCC('Npa5'--[[1315987765--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_UPGRADE_LEVELS = ConvertAbilityIntegerLevelField(FourCC('Igl1'--[[1231514673--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_SUMMONED_UNITS_NDO2 = ConvertAbilityIntegerLevelField(FourCC('Ndo2'--[[1315204914--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_BEASTS_PER_SECOND = ConvertAbilityIntegerLevelField(FourCC('Nst1'--[[1316189233--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_TARGETS_ALLOWED = ConvertAbilityIntegerLevelField(FourCC('atar'--[[1635017074--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_TARGET_TYPE = ConvertAbilityIntegerLevelField(FourCC('Ncl2'--[[1315138610--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_OPTIONS = ConvertAbilityIntegerLevelField(FourCC('Ncl3'--[[1315138611--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ARMOR_PENALTY_NAB3 = ConvertAbilityIntegerLevelField(FourCC('Nab3'--[[1315004979--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_WAVE_COUNT_NHS6 = ConvertAbilityIntegerLevelField(FourCC('Nhs6'--[[1315468086--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAX_CREEP_LEVEL_NTM3 = ConvertAbilityIntegerLevelField(FourCC('Ntm3'--[[1316252979--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MISSILE_COUNT = ConvertAbilityIntegerLevelField(FourCC('Ncs3'--[[1315140403--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SPLIT_ATTACK_COUNT = ConvertAbilityIntegerLevelField(FourCC('Nlm3'--[[1315728691--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_GENERATION_COUNT = ConvertAbilityIntegerLevelField(FourCC('Nlm6'--[[1315728694--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ROCK_RING_COUNT = ConvertAbilityIntegerLevelField(FourCC('Nvc1'--[[1316381489--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_WAVE_COUNT_NVC2 = ConvertAbilityIntegerLevelField(FourCC('Nvc2'--[[1316381490--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_PREFER_HOSTILES_TAU1 = ConvertAbilityIntegerLevelField(FourCC('Tau1'--[[1415673137--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_PREFER_FRIENDLIES_TAU2 = ConvertAbilityIntegerLevelField(FourCC('Tau2'--[[1415673138--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_MAX_UNITS_TAU3 = ConvertAbilityIntegerLevelField(FourCC('Tau3'--[[1415673139--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NUMBER_OF_PULSES = ConvertAbilityIntegerLevelField(FourCC('Tau4'--[[1415673140--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SUMMONED_UNIT_TYPE_HWE1 = ConvertAbilityIntegerLevelField(FourCC('Hwe1'--[[1215784241--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SUMMONED_UNIT_UIN4 = ConvertAbilityIntegerLevelField(FourCC('Uin4'--[[1432972852--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SUMMONED_UNIT_OSF1 = ConvertAbilityIntegerLevelField(FourCC('Osf1'--[[1332962865--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SUMMONED_UNIT_TYPE_EFNU = ConvertAbilityIntegerLevelField(FourCC('Efnu'--[[1164340853--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SUMMONED_UNIT_TYPE_NBAU = ConvertAbilityIntegerLevelField(FourCC('Nbau'--[[1315070325--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SUMMONED_UNIT_TYPE_NTOU = ConvertAbilityIntegerLevelField(FourCC('Ntou'--[[1316253557--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SUMMONED_UNIT_TYPE_ESVU = ConvertAbilityIntegerLevelField(FourCC('Esvu'--[[1165194869--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SUMMONED_UNIT_TYPES = ConvertAbilityIntegerLevelField(FourCC('Nef1'--[[1315268145--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SUMMONED_UNIT_TYPE_NDOU = ConvertAbilityIntegerLevelField(FourCC('Ndou'--[[1315204981--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ALTERNATE_FORM_UNIT_EMEU = ConvertAbilityIntegerLevelField(FourCC('Emeu'--[[1164797301--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_PLAGUE_WARD_UNIT_TYPE = ConvertAbilityIntegerLevelField(FourCC('Aplu'--[[1097886837--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ALLOWED_UNIT_TYPE_BTL1 = ConvertAbilityIntegerLevelField(FourCC('Btl1'--[[1114926129--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_NEW_UNIT_TYPE = ConvertAbilityIntegerLevelField(FourCC('Cha1'--[[1130914097--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_RESULTING_UNIT_TYPE_ENT1 = ConvertAbilityIntegerLevelField(FourCC('ent1'--[[1701737521--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_CORPSE_UNIT_TYPE = ConvertAbilityIntegerLevelField(FourCC('Gydu'--[[1199137909--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_ALLOWED_UNIT_TYPE_LOA1 = ConvertAbilityIntegerLevelField(FourCC('Loa1'--[[1282367793--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_UNIT_TYPE_FOR_LIMIT_CHECK = ConvertAbilityIntegerLevelField(FourCC('Raiu'--[[1382115701--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_WARD_UNIT_TYPE_STAU = ConvertAbilityIntegerLevelField(FourCC('Stau'--[[1400136053--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_EFFECT_ABILITY = ConvertAbilityIntegerLevelField(FourCC('Iobu'--[[1232036469--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_CONVERSION_UNIT = ConvertAbilityIntegerLevelField(FourCC('Ndc2'--[[1315201842--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_UNIT_TO_PRESERVE = ConvertAbilityIntegerLevelField(FourCC('Nsl1'--[[1316187185--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_UNIT_TYPE_ALLOWED = ConvertAbilityIntegerLevelField(FourCC('Chl1'--[[1130916913--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SWARM_UNIT_TYPE = ConvertAbilityIntegerLevelField(FourCC('Ulsu'--[[1433170805--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_RESULTING_UNIT_TYPE_COAU = ConvertAbilityIntegerLevelField(FourCC('coau'--[[1668243829--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_UNIT_TYPE_EXHU = ConvertAbilityIntegerLevelField(FourCC('exhu'--[[1702389877--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_WARD_UNIT_TYPE_HWDU = ConvertAbilityIntegerLevelField(FourCC('hwdu'--[[1752654965--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_LURE_UNIT_TYPE = ConvertAbilityIntegerLevelField(FourCC('imou'--[[1768779637--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_UNIT_TYPE_IPMU = ConvertAbilityIntegerLevelField(FourCC('ipmu'--[[1768975733--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_FACTORY_UNIT_ID = ConvertAbilityIntegerLevelField(FourCC('Nsyu'--[[1316190581--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_SPAWN_UNIT_ID_NFYU = ConvertAbilityIntegerLevelField(FourCC('Nfyu'--[[1315338613--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_DESTRUCTIBLE_ID = ConvertAbilityIntegerLevelField(FourCC('Nvcu'--[[1316381557--]])) ---@type abilityintegerlevelfield @UjAPI
ABILITY_ILF_UPGRADE_TYPE = ConvertAbilityIntegerLevelField(FourCC('Iglu'--[[1231514741--]])) ---@type abilityintegerlevelfield @UjAPI

ABILITY_RLF_CASTING_TIME = ConvertAbilityRealLevelField(FourCC('acas'--[[1633902963--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CAST_BACK_SWING = ConvertAbilityRealLevelField(FourCC('acbs'--[[1633903219--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CAST_POINT = ConvertAbilityRealLevelField(FourCC('acpt'--[[1633906804--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DURATION_NORMAL = ConvertAbilityRealLevelField(FourCC('adur'--[[1633973618--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DURATION_HERO = ConvertAbilityRealLevelField(FourCC('ahdu'--[[1634231413--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_COOLDOWN = ConvertAbilityRealLevelField(FourCC('acdn'--[[1633903726--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_AREA_OF_EFFECT = ConvertAbilityRealLevelField(FourCC('aare'--[[1633776229--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CAST_RANGE = ConvertAbilityRealLevelField(FourCC('aran'--[[1634885998--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_HBZ2 = ConvertAbilityRealLevelField(FourCC('Hbz2'--[[1214413362--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BUILDING_REDUCTION_HBZ4 = ConvertAbilityRealLevelField(FourCC('Hbz4'--[[1214413364--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_HBZ5 = ConvertAbilityRealLevelField(FourCC('Hbz5'--[[1214413365--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAXIMUM_DAMAGE_PER_WAVE = ConvertAbilityRealLevelField(FourCC('Hbz6'--[[1214413366--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_REGENERATION_INCREASE = ConvertAbilityRealLevelField(FourCC('Hab1'--[[1214341681--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CASTING_DELAY = ConvertAbilityRealLevelField(FourCC('Hmt2'--[[1215132722--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_OWW1 = ConvertAbilityRealLevelField(FourCC('Oww1'--[[1333229361--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_OWW2 = ConvertAbilityRealLevelField(FourCC('Oww2'--[[1333229362--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CHANCE_TO_CRITICAL_STRIKE = ConvertAbilityRealLevelField(FourCC('Ocr1'--[[1331917361--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_MULTIPLIER_OCR2 = ConvertAbilityRealLevelField(FourCC('Ocr2'--[[1331917362--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_BONUS_OCR3 = ConvertAbilityRealLevelField(FourCC('Ocr3'--[[1331917363--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CHANCE_TO_EVADE_OCR4 = ConvertAbilityRealLevelField(FourCC('Ocr4'--[[1331917364--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_DEALT_PERCENT_OMI2 = ConvertAbilityRealLevelField(FourCC('Omi2'--[[1332570418--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_TAKEN_PERCENT_OMI3 = ConvertAbilityRealLevelField(FourCC('Omi3'--[[1332570419--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ANIMATION_DELAY = ConvertAbilityRealLevelField(FourCC('Omi4'--[[1332570420--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_TRANSITION_TIME = ConvertAbilityRealLevelField(FourCC('Owk1'--[[1333226289--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_INCREASE_PERCENT_OWK2 = ConvertAbilityRealLevelField(FourCC('Owk2'--[[1333226290--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BACKSTAB_DAMAGE = ConvertAbilityRealLevelField(FourCC('Owk3'--[[1333226291--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_AMOUNT_HEALED_DAMAGED_UDC1 = ConvertAbilityRealLevelField(FourCC('Udc1'--[[1432642353--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LIFE_CONVERTED_TO_MANA = ConvertAbilityRealLevelField(FourCC('Udp1'--[[1432645681--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LIFE_CONVERTED_TO_LIFE = ConvertAbilityRealLevelField(FourCC('Udp2'--[[1432645682--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_INCREASE_PERCENT_UAU1 = ConvertAbilityRealLevelField(FourCC('Uau1'--[[1432450353--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LIFE_REGENERATION_INCREASE_PERCENT = ConvertAbilityRealLevelField(FourCC('Uau2'--[[1432450354--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CHANCE_TO_EVADE_EEV1 = ConvertAbilityRealLevelField(FourCC('Eev1'--[[1164277297--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_INTERVAL = ConvertAbilityRealLevelField(FourCC('Eim1'--[[1164537137--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_DRAINED_PER_SECOND_EIM2 = ConvertAbilityRealLevelField(FourCC('Eim2'--[[1164537138--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BUFFER_MANA_REQUIRED = ConvertAbilityRealLevelField(FourCC('Eim3'--[[1164537139--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAX_MANA_DRAINED = ConvertAbilityRealLevelField(FourCC('Emb1'--[[1164796465--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BOLT_DELAY = ConvertAbilityRealLevelField(FourCC('Emb2'--[[1164796466--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BOLT_LIFETIME = ConvertAbilityRealLevelField(FourCC('Emb3'--[[1164796467--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ALTITUDE_ADJUSTMENT_DURATION = ConvertAbilityRealLevelField(FourCC('Eme3'--[[1164797235--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LANDING_DELAY_TIME = ConvertAbilityRealLevelField(FourCC('Eme4'--[[1164797236--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ALTERNATE_FORM_HIT_POINT_BONUS = ConvertAbilityRealLevelField(FourCC('Eme5'--[[1164797237--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVE_SPEED_BONUS_INFO_PANEL_ONLY = ConvertAbilityRealLevelField(FourCC('Ncr5'--[[1315140149--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_BONUS_INFO_PANEL_ONLY = ConvertAbilityRealLevelField(FourCC('Ncr6'--[[1315140150--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LIFE_REGENERATION_RATE_PER_SECOND = ConvertAbilityRealLevelField(FourCC('ave5'--[[1635149109--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_STUN_DURATION_USL1 = ConvertAbilityRealLevelField(FourCC('Usl1'--[[1433627697--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_DAMAGE_STOLEN_PERCENT = ConvertAbilityRealLevelField(FourCC('Uav1'--[[1432450609--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_UCS1 = ConvertAbilityRealLevelField(FourCC('Ucs1'--[[1432580913--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAX_DAMAGE_UCS2 = ConvertAbilityRealLevelField(FourCC('Ucs2'--[[1432580914--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DISTANCE_UCS3 = ConvertAbilityRealLevelField(FourCC('Ucs3'--[[1432580915--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_FINAL_AREA_UCS4 = ConvertAbilityRealLevelField(FourCC('Ucs4'--[[1432580916--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_UIN1 = ConvertAbilityRealLevelField(FourCC('Uin1'--[[1432972849--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DURATION = ConvertAbilityRealLevelField(FourCC('Uin2'--[[1432972850--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_IMPACT_DELAY = ConvertAbilityRealLevelField(FourCC('Uin3'--[[1432972851--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_TARGET_OCL1 = ConvertAbilityRealLevelField(FourCC('Ocl1'--[[1331915825--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_REDUCTION_PER_TARGET = ConvertAbilityRealLevelField(FourCC('Ocl3'--[[1331915827--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_EFFECT_DELAY_OEQ1 = ConvertAbilityRealLevelField(FourCC('Oeq1'--[[1332048177--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_TO_BUILDINGS = ConvertAbilityRealLevelField(FourCC('Oeq2'--[[1332048178--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_UNITS_SLOWED_PERCENT = ConvertAbilityRealLevelField(FourCC('Oeq3'--[[1332048179--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_FINAL_AREA_OEQ4 = ConvertAbilityRealLevelField(FourCC('Oeq4'--[[1332048180--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_EER1 = ConvertAbilityRealLevelField(FourCC('Eer1'--[[1164276273--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_DEALT_TO_ATTACKERS = ConvertAbilityRealLevelField(FourCC('Eah1'--[[1164011569--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LIFE_HEALED = ConvertAbilityRealLevelField(FourCC('Etq1'--[[1165259057--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HEAL_INTERVAL = ConvertAbilityRealLevelField(FourCC('Etq2'--[[1165259058--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BUILDING_REDUCTION_ETQ3 = ConvertAbilityRealLevelField(FourCC('Etq3'--[[1165259059--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_INITIAL_IMMUNITY_DURATION = ConvertAbilityRealLevelField(FourCC('Etq4'--[[1165259060--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAX_LIFE_DRAINED_PER_SECOND_PERCENT = ConvertAbilityRealLevelField(FourCC('Udd1'--[[1432642609--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BUILDING_REDUCTION_UDD2 = ConvertAbilityRealLevelField(FourCC('Udd2'--[[1432642610--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ARMOR_DURATION = ConvertAbilityRealLevelField(FourCC('Ufa1'--[[1432772913--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ARMOR_BONUS_UFA2 = ConvertAbilityRealLevelField(FourCC('Ufa2'--[[1432772914--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_AREA_OF_EFFECT_DAMAGE = ConvertAbilityRealLevelField(FourCC('Ufn1'--[[1432776241--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SPECIFIC_TARGET_DAMAGE_UFN2 = ConvertAbilityRealLevelField(FourCC('Ufn2'--[[1432776242--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_BONUS_HFA1 = ConvertAbilityRealLevelField(FourCC('Hfa1'--[[1214669105--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_DEALT_ESF1 = ConvertAbilityRealLevelField(FourCC('Esf1'--[[1165190705--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_INTERVAL_ESF2 = ConvertAbilityRealLevelField(FourCC('Esf2'--[[1165190706--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BUILDING_REDUCTION_ESF3 = ConvertAbilityRealLevelField(FourCC('Esf3'--[[1165190707--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_BONUS_PERCENT = ConvertAbilityRealLevelField(FourCC('Ear1'--[[1164014129--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DEFENSE_BONUS_HAV1 = ConvertAbilityRealLevelField(FourCC('Hav1'--[[1214346801--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HIT_POINT_BONUS = ConvertAbilityRealLevelField(FourCC('Hav2'--[[1214346802--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_BONUS_HAV3 = ConvertAbilityRealLevelField(FourCC('Hav3'--[[1214346803--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_HAV4 = ConvertAbilityRealLevelField(FourCC('Hav4'--[[1214346804--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CHANCE_TO_BASH = ConvertAbilityRealLevelField(FourCC('Hbh1'--[[1214408753--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_MULTIPLIER_HBH2 = ConvertAbilityRealLevelField(FourCC('Hbh2'--[[1214408754--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_BONUS_HBH3 = ConvertAbilityRealLevelField(FourCC('Hbh3'--[[1214408755--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CHANCE_TO_MISS_HBH4 = ConvertAbilityRealLevelField(FourCC('Hbh4'--[[1214408756--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_HTB1 = ConvertAbilityRealLevelField(FourCC('Htb1'--[[1215586865--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_AOE_DAMAGE = ConvertAbilityRealLevelField(FourCC('Htc1'--[[1215587121--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SPECIFIC_TARGET_DAMAGE_HTC2 = ConvertAbilityRealLevelField(FourCC('Htc2'--[[1215587122--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_PERCENT_HTC3 = ConvertAbilityRealLevelField(FourCC('Htc3'--[[1215587123--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_REDUCTION_PERCENT_HTC4 = ConvertAbilityRealLevelField(FourCC('Htc4'--[[1215587124--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ARMOR_BONUS_HAD1 = ConvertAbilityRealLevelField(FourCC('Had1'--[[1214342193--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_AMOUNT_HEALED_DAMAGED_HHB1 = ConvertAbilityRealLevelField(FourCC('Hhb1'--[[1214800433--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_EXTRA_DAMAGE_HCA1 = ConvertAbilityRealLevelField(FourCC('Hca1'--[[1214472497--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_FACTOR_HCA2 = ConvertAbilityRealLevelField(FourCC('Hca2'--[[1214472498--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_FACTOR_HCA3 = ConvertAbilityRealLevelField(FourCC('Hca3'--[[1214472499--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_INCREASE_PERCENT_OAE1 = ConvertAbilityRealLevelField(FourCC('Oae1'--[[1331782961--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_INCREASE_PERCENT_OAE2 = ConvertAbilityRealLevelField(FourCC('Oae2'--[[1331782962--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_REINCARNATION_DELAY = ConvertAbilityRealLevelField(FourCC('Ore1'--[[1332897073--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_OSH1 = ConvertAbilityRealLevelField(FourCC('Osh1'--[[1332963377--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAXIMUM_DAMAGE_OSH2 = ConvertAbilityRealLevelField(FourCC('Osh2'--[[1332963378--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DISTANCE_OSH3 = ConvertAbilityRealLevelField(FourCC('Osh3'--[[1332963379--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_FINAL_AREA_OSH4 = ConvertAbilityRealLevelField(FourCC('Osh4'--[[1332963380--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_GRAPHIC_DELAY_NFD1 = ConvertAbilityRealLevelField(FourCC('Nfd1'--[[1315333169--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_GRAPHIC_DURATION_NFD2 = ConvertAbilityRealLevelField(FourCC('Nfd2'--[[1315333170--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_NFD3 = ConvertAbilityRealLevelField(FourCC('Nfd3'--[[1315333171--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SUMMONED_UNIT_DAMAGE_AMS1 = ConvertAbilityRealLevelField(FourCC('Ams1'--[[1097691953--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_AMS2 = ConvertAbilityRealLevelField(FourCC('Ams2'--[[1097691954--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_AURA_DURATION = ConvertAbilityRealLevelField(FourCC('Apl1'--[[1097886769--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_APL2 = ConvertAbilityRealLevelField(FourCC('Apl2'--[[1097886770--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DURATION_OF_PLAGUE_WARD = ConvertAbilityRealLevelField(FourCC('Apl3'--[[1097886771--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_AMOUNT_OF_HIT_POINTS_REGENERATED = ConvertAbilityRealLevelField(FourCC('Oar1'--[[1331786289--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_DAMAGE_INCREASE_AKB1 = ConvertAbilityRealLevelField(FourCC('Akb1'--[[1097556529--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_LOSS_ADM1 = ConvertAbilityRealLevelField(FourCC('Adm1'--[[1097100593--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SUMMONED_UNIT_DAMAGE_ADM2 = ConvertAbilityRealLevelField(FourCC('Adm2'--[[1097100594--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_EXPANSION_AMOUNT = ConvertAbilityRealLevelField(FourCC('Bli1'--[[1114401073--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_INTERVAL_DURATION_BGM2 = ConvertAbilityRealLevelField(FourCC('Bgm2'--[[1114074418--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_RADIUS_OF_MINING_RING = ConvertAbilityRealLevelField(FourCC('Bgm4'--[[1114074420--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_INCREASE_PERCENT_BLO1 = ConvertAbilityRealLevelField(FourCC('Blo1'--[[1114402609--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_INCREASE_PERCENT_BLO2 = ConvertAbilityRealLevelField(FourCC('Blo2'--[[1114402610--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SCALING_FACTOR = ConvertAbilityRealLevelField(FourCC('Blo3'--[[1114402611--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HIT_POINTS_PER_SECOND_CAN1 = ConvertAbilityRealLevelField(FourCC('Can1'--[[1130458673--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAX_HIT_POINTS = ConvertAbilityRealLevelField(FourCC('Can2'--[[1130458674--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_DEV2 = ConvertAbilityRealLevelField(FourCC('Dev2'--[[1147500082--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_UPDATE_FREQUENCY_CHD1 = ConvertAbilityRealLevelField(FourCC('Chd1'--[[1130914865--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_UPDATE_FREQUENCY_CHD2 = ConvertAbilityRealLevelField(FourCC('Chd2'--[[1130914866--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SUMMONED_UNIT_DAMAGE_CHD3 = ConvertAbilityRealLevelField(FourCC('Chd3'--[[1130914867--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_PERCENT_CRI1 = ConvertAbilityRealLevelField(FourCC('Cri1'--[[1131571505--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_REDUCTION_PERCENT_CRI2 = ConvertAbilityRealLevelField(FourCC('Cri2'--[[1131571506--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_REDUCTION_CRI3 = ConvertAbilityRealLevelField(FourCC('Cri3'--[[1131571507--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CHANCE_TO_MISS_CRS = ConvertAbilityRealLevelField(FourCC('Crs1'--[[1131574065--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_FULL_DAMAGE_RADIUS_DDA1 = ConvertAbilityRealLevelField(FourCC('Dda1'--[[1147429169--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_FULL_DAMAGE_AMOUNT_DDA2 = ConvertAbilityRealLevelField(FourCC('Dda2'--[[1147429170--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_PARTIAL_DAMAGE_RADIUS = ConvertAbilityRealLevelField(FourCC('Dda3'--[[1147429171--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_PARTIAL_DAMAGE_AMOUNT = ConvertAbilityRealLevelField(FourCC('Dda4'--[[1147429172--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BUILDING_DAMAGE_FACTOR_SDS1 = ConvertAbilityRealLevelField(FourCC('Sds1'--[[1399092017--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAX_DAMAGE_UCO5 = ConvertAbilityRealLevelField(FourCC('Uco5'--[[1432579893--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVE_SPEED_BONUS_UCO6 = ConvertAbilityRealLevelField(FourCC('Uco6'--[[1432579894--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_TAKEN_PERCENT_DEF1 = ConvertAbilityRealLevelField(FourCC('Def1'--[[1147495985--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_DEALT_PERCENT_DEF2 = ConvertAbilityRealLevelField(FourCC('Def2'--[[1147495986--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_FACTOR_DEF3 = ConvertAbilityRealLevelField(FourCC('Def3'--[[1147495987--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_FACTOR_DEF4 = ConvertAbilityRealLevelField(FourCC('Def4'--[[1147495988--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_DEF5 = ConvertAbilityRealLevelField(FourCC('Def5'--[[1147495989--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CHANCE_TO_DEFLECT = ConvertAbilityRealLevelField(FourCC('Def6'--[[1147495990--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DEFLECT_DAMAGE_TAKEN_PIERCING = ConvertAbilityRealLevelField(FourCC('Def7'--[[1147495991--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DEFLECT_DAMAGE_TAKEN_SPELLS = ConvertAbilityRealLevelField(FourCC('Def8'--[[1147495992--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_RIP_DELAY = ConvertAbilityRealLevelField(FourCC('Eat1'--[[1164014641--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_EAT_DELAY = ConvertAbilityRealLevelField(FourCC('Eat2'--[[1164014642--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HIT_POINTS_GAINED_EAT3 = ConvertAbilityRealLevelField(FourCC('Eat3'--[[1164014643--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_AIR_UNIT_LOWER_DURATION = ConvertAbilityRealLevelField(FourCC('Ens1'--[[1164866353--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_AIR_UNIT_HEIGHT = ConvertAbilityRealLevelField(FourCC('Ens2'--[[1164866354--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MELEE_ATTACK_RANGE = ConvertAbilityRealLevelField(FourCC('Ens3'--[[1164866355--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_INTERVAL_DURATION_EGM2 = ConvertAbilityRealLevelField(FourCC('Egm2'--[[1164406066--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_EFFECT_DELAY_FLA2 = ConvertAbilityRealLevelField(FourCC('Fla2'--[[1181507890--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MINING_DURATION = ConvertAbilityRealLevelField(FourCC('Gld2'--[[1198285874--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_RADIUS_OF_GRAVESTONES = ConvertAbilityRealLevelField(FourCC('Gyd2'--[[1199137842--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_RADIUS_OF_CORPSES = ConvertAbilityRealLevelField(FourCC('Gyd3'--[[1199137843--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HIT_POINTS_GAINED_HEA1 = ConvertAbilityRealLevelField(FourCC('Hea1'--[[1214603569--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_INCREASE_PERCENT_INF1 = ConvertAbilityRealLevelField(FourCC('Inf1'--[[1231971889--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_AUTOCAST_RANGE = ConvertAbilityRealLevelField(FourCC('Inf3'--[[1231971891--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LIFE_REGEN_RATE = ConvertAbilityRealLevelField(FourCC('Inf4'--[[1231971892--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_GRAPHIC_DELAY_LIT1 = ConvertAbilityRealLevelField(FourCC('Lit1'--[[1281979441--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_GRAPHIC_DURATION_LIT2 = ConvertAbilityRealLevelField(FourCC('Lit2'--[[1281979442--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_LSH1 = ConvertAbilityRealLevelField(FourCC('Lsh1'--[[1282631729--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_GAINED = ConvertAbilityRealLevelField(FourCC('Mbt1'--[[1298297905--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HIT_POINTS_GAINED_MBT2 = ConvertAbilityRealLevelField(FourCC('Mbt2'--[[1298297906--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_AUTOCAST_REQUIREMENT = ConvertAbilityRealLevelField(FourCC('Mbt3'--[[1298297907--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_WATER_HEIGHT = ConvertAbilityRealLevelField(FourCC('Mbt4'--[[1298297908--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ACTIVATION_DELAY_MIN1 = ConvertAbilityRealLevelField(FourCC('Min1'--[[1298755121--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_INVISIBILITY_TRANSITION_TIME = ConvertAbilityRealLevelField(FourCC('Min2'--[[1298755122--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ACTIVATION_RADIUS = ConvertAbilityRealLevelField(FourCC('Neu1'--[[1315271985--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_AMOUNT_REGENERATED = ConvertAbilityRealLevelField(FourCC('Arm1'--[[1098018097--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_POI1 = ConvertAbilityRealLevelField(FourCC('Poi1'--[[1349478705--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_FACTOR_POI2 = ConvertAbilityRealLevelField(FourCC('Poi2'--[[1349478706--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_FACTOR_POI3 = ConvertAbilityRealLevelField(FourCC('Poi3'--[[1349478707--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_EXTRA_DAMAGE_POA1 = ConvertAbilityRealLevelField(FourCC('Poa1'--[[1349476657--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_POA2 = ConvertAbilityRealLevelField(FourCC('Poa2'--[[1349476658--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_FACTOR_POA3 = ConvertAbilityRealLevelField(FourCC('Poa3'--[[1349476659--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_FACTOR_POA4 = ConvertAbilityRealLevelField(FourCC('Poa4'--[[1349476660--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_AMPLIFICATION = ConvertAbilityRealLevelField(FourCC('Pos2'--[[1349481266--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CHANCE_TO_STOMP_PERCENT = ConvertAbilityRealLevelField(FourCC('War1'--[[1466004017--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_DEALT_WAR2 = ConvertAbilityRealLevelField(FourCC('War2'--[[1466004018--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_FULL_DAMAGE_RADIUS_WAR3 = ConvertAbilityRealLevelField(FourCC('War3'--[[1466004019--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HALF_DAMAGE_RADIUS_WAR4 = ConvertAbilityRealLevelField(FourCC('War4'--[[1466004020--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SUMMONED_UNIT_DAMAGE_PRG3 = ConvertAbilityRealLevelField(FourCC('Prg3'--[[1349674803--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_UNIT_PAUSE_DURATION = ConvertAbilityRealLevelField(FourCC('Prg4'--[[1349674804--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HERO_PAUSE_DURATION = ConvertAbilityRealLevelField(FourCC('Prg5'--[[1349674805--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HIT_POINTS_GAINED_REJ1 = ConvertAbilityRealLevelField(FourCC('Rej1'--[[1382378033--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_POINTS_GAINED_REJ2 = ConvertAbilityRealLevelField(FourCC('Rej2'--[[1382378034--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MINIMUM_LIFE_REQUIRED = ConvertAbilityRealLevelField(FourCC('Rpb3'--[[1383096883--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MINIMUM_MANA_REQUIRED = ConvertAbilityRealLevelField(FourCC('Rpb4'--[[1383096884--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_REPAIR_COST_RATIO = ConvertAbilityRealLevelField(FourCC('Rep1'--[[1382379569--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_REPAIR_TIME_RATIO = ConvertAbilityRealLevelField(FourCC('Rep2'--[[1382379570--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_POWERBUILD_COST = ConvertAbilityRealLevelField(FourCC('Rep3'--[[1382379571--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_POWERBUILD_RATE = ConvertAbilityRealLevelField(FourCC('Rep4'--[[1382379572--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_NAVAL_RANGE_BONUS = ConvertAbilityRealLevelField(FourCC('Rep5'--[[1382379573--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_INCREASE_PERCENT_ROA1 = ConvertAbilityRealLevelField(FourCC('Roa1'--[[1383031089--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LIFE_REGENERATION_RATE = ConvertAbilityRealLevelField(FourCC('Roa3'--[[1383031091--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_REGEN = ConvertAbilityRealLevelField(FourCC('Roa4'--[[1383031092--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_INCREASE = ConvertAbilityRealLevelField(FourCC('Nbr1'--[[1315074609--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SALVAGE_COST_RATIO = ConvertAbilityRealLevelField(FourCC('Sal1'--[[1398893617--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_IN_FLIGHT_SIGHT_RADIUS = ConvertAbilityRealLevelField(FourCC('Esn1'--[[1165192753--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HOVERING_SIGHT_RADIUS = ConvertAbilityRealLevelField(FourCC('Esn2'--[[1165192754--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HOVERING_HEIGHT = ConvertAbilityRealLevelField(FourCC('Esn3'--[[1165192755--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DURATION_OF_OWLS = ConvertAbilityRealLevelField(FourCC('Esn5'--[[1165192757--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_FADE_DURATION = ConvertAbilityRealLevelField(FourCC('Shm1'--[[1399352625--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAY_NIGHT_DURATION = ConvertAbilityRealLevelField(FourCC('Shm2'--[[1399352626--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ACTION_DURATION = ConvertAbilityRealLevelField(FourCC('Shm3'--[[1399352627--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_FACTOR_SLO1 = ConvertAbilityRealLevelField(FourCC('Slo1'--[[1399615281--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_FACTOR_SLO2 = ConvertAbilityRealLevelField(FourCC('Slo2'--[[1399615282--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_SPO1 = ConvertAbilityRealLevelField(FourCC('Spo1'--[[1399877425--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_FACTOR_SPO2 = ConvertAbilityRealLevelField(FourCC('Spo2'--[[1399877426--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_FACTOR_SPO3 = ConvertAbilityRealLevelField(FourCC('Spo3'--[[1399877427--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ACTIVATION_DELAY_STA1 = ConvertAbilityRealLevelField(FourCC('Sta1'--[[1400135985--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DETECTION_RADIUS_STA2 = ConvertAbilityRealLevelField(FourCC('Sta2'--[[1400135986--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DETONATION_RADIUS = ConvertAbilityRealLevelField(FourCC('Sta3'--[[1400135987--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_STUN_DURATION_STA4 = ConvertAbilityRealLevelField(FourCC('Sta4'--[[1400135988--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_BONUS_PERCENT = ConvertAbilityRealLevelField(FourCC('Uhf1'--[[1432905265--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_UHF2 = ConvertAbilityRealLevelField(FourCC('Uhf2'--[[1432905266--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LUMBER_PER_INTERVAL = ConvertAbilityRealLevelField(FourCC('Wha1'--[[1466458417--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ART_ATTACHMENT_HEIGHT = ConvertAbilityRealLevelField(FourCC('Wha3'--[[1466458419--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_TELEPORT_AREA_WIDTH = ConvertAbilityRealLevelField(FourCC('Wrp1'--[[1467117617--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_TELEPORT_AREA_HEIGHT = ConvertAbilityRealLevelField(FourCC('Wrp2'--[[1467117618--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LIFE_STOLEN_PER_ATTACK = ConvertAbilityRealLevelField(FourCC('Ivam'--[[1232494957--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_BONUS_IDAM = ConvertAbilityRealLevelField(FourCC('Idam'--[[1231315309--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CHANCE_TO_HIT_UNITS_PERCENT = ConvertAbilityRealLevelField(FourCC('Iob2'--[[1232036402--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CHANCE_TO_HIT_HEROS_PERCENT = ConvertAbilityRealLevelField(FourCC('Iob3'--[[1232036403--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CHANCE_TO_HIT_SUMMONS_PERCENT = ConvertAbilityRealLevelField(FourCC('Iob4'--[[1232036404--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DELAY_FOR_TARGET_EFFECT = ConvertAbilityRealLevelField(FourCC('Idel'--[[1231316332--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_DEALT_PERCENT_OF_NORMAL = ConvertAbilityRealLevelField(FourCC('Iild'--[[1231645796--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_RECEIVED_MULTIPLIER = ConvertAbilityRealLevelField(FourCC('Iilw'--[[1231645815--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_REGENERATION_BONUS_AS_FRACTION_OF_NORMAL = ConvertAbilityRealLevelField(FourCC('Imrp'--[[1231909488--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_INCREASE_ISPI = ConvertAbilityRealLevelField(FourCC('Ispi'--[[1232302185--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_IDPS = ConvertAbilityRealLevelField(FourCC('Idps'--[[1231319155--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_DAMAGE_INCREASE_CAC1 = ConvertAbilityRealLevelField(FourCC('Cac1'--[[1130455857--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_COR1 = ConvertAbilityRealLevelField(FourCC('Cor1'--[[1131377201--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_INCREASE_ISX1 = ConvertAbilityRealLevelField(FourCC('Isx1'--[[1232304177--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_WRS1 = ConvertAbilityRealLevelField(FourCC('Wrs1'--[[1467118385--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_TERRAIN_DEFORMATION_AMPLITUDE = ConvertAbilityRealLevelField(FourCC('Wrs2'--[[1467118386--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_CTC1 = ConvertAbilityRealLevelField(FourCC('Ctc1'--[[1131701041--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_EXTRA_DAMAGE_TO_TARGET = ConvertAbilityRealLevelField(FourCC('Ctc2'--[[1131701042--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_CTC3 = ConvertAbilityRealLevelField(FourCC('Ctc3'--[[1131701043--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_REDUCTION_CTC4 = ConvertAbilityRealLevelField(FourCC('Ctc4'--[[1131701044--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_CTB1 = ConvertAbilityRealLevelField(FourCC('Ctb1'--[[1131700785--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CASTING_DELAY_SECONDS = ConvertAbilityRealLevelField(FourCC('Uds2'--[[1432646450--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_LOSS_PER_UNIT_DTN1 = ConvertAbilityRealLevelField(FourCC('Dtn1'--[[1148481073--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_TO_SUMMONED_UNITS_DTN2 = ConvertAbilityRealLevelField(FourCC('Dtn2'--[[1148481074--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_TRANSITION_TIME_SECONDS = ConvertAbilityRealLevelField(FourCC('Ivs1'--[[1232499505--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_DRAINED_PER_SECOND_NMR1 = ConvertAbilityRealLevelField(FourCC('Nmr1'--[[1315795505--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CHANCE_TO_REDUCE_DAMAGE_PERCENT = ConvertAbilityRealLevelField(FourCC('Ssk1'--[[1400073009--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MINIMUM_DAMAGE = ConvertAbilityRealLevelField(FourCC('Ssk2'--[[1400073010--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_IGNORED_DAMAGE = ConvertAbilityRealLevelField(FourCC('Ssk3'--[[1400073011--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_FULL_DAMAGE_DEALT = ConvertAbilityRealLevelField(FourCC('Hfs1'--[[1214673713--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_FULL_DAMAGE_INTERVAL = ConvertAbilityRealLevelField(FourCC('Hfs2'--[[1214673714--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HALF_DAMAGE_DEALT = ConvertAbilityRealLevelField(FourCC('Hfs3'--[[1214673715--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HALF_DAMAGE_INTERVAL = ConvertAbilityRealLevelField(FourCC('Hfs4'--[[1214673716--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BUILDING_REDUCTION_HFS5 = ConvertAbilityRealLevelField(FourCC('Hfs5'--[[1214673717--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAXIMUM_DAMAGE_HFS6 = ConvertAbilityRealLevelField(FourCC('Hfs6'--[[1214673718--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_PER_HIT_POINT = ConvertAbilityRealLevelField(FourCC('Nms1'--[[1315795761--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_ABSORBED_PERCENT = ConvertAbilityRealLevelField(FourCC('Nms2'--[[1315795762--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_WAVE_DISTANCE = ConvertAbilityRealLevelField(FourCC('Uim1'--[[1432972593--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_WAVE_TIME_SECONDS = ConvertAbilityRealLevelField(FourCC('Uim2'--[[1432972594--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_DEALT_UIM3 = ConvertAbilityRealLevelField(FourCC('Uim3'--[[1432972595--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_AIR_TIME_SECONDS_UIM4 = ConvertAbilityRealLevelField(FourCC('Uim4'--[[1432972596--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_UNIT_RELEASE_INTERVAL_SECONDS = ConvertAbilityRealLevelField(FourCC('Uls2'--[[1433170738--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_RETURN_FACTOR = ConvertAbilityRealLevelField(FourCC('Uls4'--[[1433170740--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_RETURN_THRESHOLD = ConvertAbilityRealLevelField(FourCC('Uls5'--[[1433170741--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_RETURNED_DAMAGE_FACTOR = ConvertAbilityRealLevelField(FourCC('Uts1'--[[1433695025--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_RECEIVED_DAMAGE_FACTOR = ConvertAbilityRealLevelField(FourCC('Uts2'--[[1433695026--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DEFENSE_BONUS_UTS3 = ConvertAbilityRealLevelField(FourCC('Uts3'--[[1433695027--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_BONUS_NBA1 = ConvertAbilityRealLevelField(FourCC('Nba1'--[[1315070257--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SUMMONED_UNIT_DURATION_SECONDS_NBA3 = ConvertAbilityRealLevelField(FourCC('Nba3'--[[1315070259--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_PER_SUMMONED_HITPOINT = ConvertAbilityRealLevelField(FourCC('Cmg2'--[[1131243314--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CHARGE_FOR_CURRENT_LIFE = ConvertAbilityRealLevelField(FourCC('Cmg3'--[[1131243315--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HIT_POINTS_DRAINED = ConvertAbilityRealLevelField(FourCC('Ndr1'--[[1315205681--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_POINTS_DRAINED = ConvertAbilityRealLevelField(FourCC('Ndr2'--[[1315205682--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DRAIN_INTERVAL_SECONDS = ConvertAbilityRealLevelField(FourCC('Ndr3'--[[1315205683--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LIFE_TRANSFERRED_PER_SECOND = ConvertAbilityRealLevelField(FourCC('Ndr4'--[[1315205684--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_TRANSFERRED_PER_SECOND = ConvertAbilityRealLevelField(FourCC('Ndr5'--[[1315205685--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BONUS_LIFE_FACTOR = ConvertAbilityRealLevelField(FourCC('Ndr6'--[[1315205686--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BONUS_LIFE_DECAY = ConvertAbilityRealLevelField(FourCC('Ndr7'--[[1315205687--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BONUS_MANA_FACTOR = ConvertAbilityRealLevelField(FourCC('Ndr8'--[[1315205688--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BONUS_MANA_DECAY = ConvertAbilityRealLevelField(FourCC('Ndr9'--[[1315205689--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CHANCE_TO_MISS_PERCENT = ConvertAbilityRealLevelField(FourCC('Nsi2'--[[1316186418--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_MODIFIER = ConvertAbilityRealLevelField(FourCC('Nsi3'--[[1316186419--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_MODIFIER = ConvertAbilityRealLevelField(FourCC('Nsi4'--[[1316186420--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_TDG1 = ConvertAbilityRealLevelField(FourCC('Tdg1'--[[1415866161--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MEDIUM_DAMAGE_RADIUS_TDG2 = ConvertAbilityRealLevelField(FourCC('Tdg2'--[[1415866162--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MEDIUM_DAMAGE_PER_SECOND = ConvertAbilityRealLevelField(FourCC('Tdg3'--[[1415866163--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SMALL_DAMAGE_RADIUS_TDG4 = ConvertAbilityRealLevelField(FourCC('Tdg4'--[[1415866164--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SMALL_DAMAGE_PER_SECOND = ConvertAbilityRealLevelField(FourCC('Tdg5'--[[1415866165--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_AIR_TIME_SECONDS_TSP1 = ConvertAbilityRealLevelField(FourCC('Tsp1'--[[1416851505--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MINIMUM_HIT_INTERVAL_SECONDS = ConvertAbilityRealLevelField(FourCC('Tsp2'--[[1416851506--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_NBF5 = ConvertAbilityRealLevelField(FourCC('Nbf5'--[[1315071541--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAXIMUM_RANGE = ConvertAbilityRealLevelField(FourCC('Ebl1'--[[1164078129--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MINIMUM_RANGE = ConvertAbilityRealLevelField(FourCC('Ebl2'--[[1164078130--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_TARGET_EFK1 = ConvertAbilityRealLevelField(FourCC('Efk1'--[[1164340017--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAXIMUM_TOTAL_DAMAGE = ConvertAbilityRealLevelField(FourCC('Efk2'--[[1164340018--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAXIMUM_SPEED_ADJUSTMENT = ConvertAbilityRealLevelField(FourCC('Efk4'--[[1164340020--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DECAYING_DAMAGE = ConvertAbilityRealLevelField(FourCC('Esh1'--[[1165191217--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_FACTOR_ESH2 = ConvertAbilityRealLevelField(FourCC('Esh2'--[[1165191218--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_FACTOR_ESH3 = ConvertAbilityRealLevelField(FourCC('Esh3'--[[1165191219--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DECAY_POWER = ConvertAbilityRealLevelField(FourCC('Esh4'--[[1165191220--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_INITIAL_DAMAGE_ESH5 = ConvertAbilityRealLevelField(FourCC('Esh5'--[[1165191221--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAXIMUM_LIFE_ABSORBED = ConvertAbilityRealLevelField(FourCC('abs1'--[[1633841969--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAXIMUM_MANA_ABSORBED = ConvertAbilityRealLevelField(FourCC('abs2'--[[1633841970--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_INCREASE_BSK1 = ConvertAbilityRealLevelField(FourCC('bsk1'--[[1651731249--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_INCREASE_BSK2 = ConvertAbilityRealLevelField(FourCC('bsk2'--[[1651731250--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_TAKEN_INCREASE = ConvertAbilityRealLevelField(FourCC('bsk3'--[[1651731251--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LIFE_PER_UNIT = ConvertAbilityRealLevelField(FourCC('dvm1'--[[1685482801--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_PER_UNIT = ConvertAbilityRealLevelField(FourCC('dvm2'--[[1685482802--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LIFE_PER_BUFF = ConvertAbilityRealLevelField(FourCC('dvm3'--[[1685482803--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_PER_BUFF = ConvertAbilityRealLevelField(FourCC('dvm4'--[[1685482804--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SUMMONED_UNIT_DAMAGE_DVM5 = ConvertAbilityRealLevelField(FourCC('dvm5'--[[1685482805--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_BONUS_FAK1 = ConvertAbilityRealLevelField(FourCC('fak1'--[[1717660465--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MEDIUM_DAMAGE_FACTOR_FAK2 = ConvertAbilityRealLevelField(FourCC('fak2'--[[1717660466--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SMALL_DAMAGE_FACTOR_FAK3 = ConvertAbilityRealLevelField(FourCC('fak3'--[[1717660467--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_FULL_DAMAGE_RADIUS_FAK4 = ConvertAbilityRealLevelField(FourCC('fak4'--[[1717660468--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HALF_DAMAGE_RADIUS_FAK5 = ConvertAbilityRealLevelField(FourCC('fak5'--[[1717660469--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_EXTRA_DAMAGE_PER_SECOND = ConvertAbilityRealLevelField(FourCC('liq1'--[[1818849585--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_LIQ2 = ConvertAbilityRealLevelField(FourCC('liq2'--[[1818849586--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_REDUCTION_LIQ3 = ConvertAbilityRealLevelField(FourCC('liq3'--[[1818849587--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAGIC_DAMAGE_FACTOR = ConvertAbilityRealLevelField(FourCC('mim1'--[[1835625777--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_UNIT_DAMAGE_PER_MANA_POINT = ConvertAbilityRealLevelField(FourCC('mfl1'--[[1835428913--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HERO_DAMAGE_PER_MANA_POINT = ConvertAbilityRealLevelField(FourCC('mfl2'--[[1835428914--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_UNIT_MAXIMUM_DAMAGE = ConvertAbilityRealLevelField(FourCC('mfl3'--[[1835428915--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HERO_MAXIMUM_DAMAGE = ConvertAbilityRealLevelField(FourCC('mfl4'--[[1835428916--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_COOLDOWN = ConvertAbilityRealLevelField(FourCC('mfl5'--[[1835428917--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DISTRIBUTED_DAMAGE_FACTOR_SPL1 = ConvertAbilityRealLevelField(FourCC('spl1'--[[1936747569--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LIFE_REGENERATED = ConvertAbilityRealLevelField(FourCC('irl1'--[[1769106481--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_REGENERATED = ConvertAbilityRealLevelField(FourCC('irl2'--[[1769106482--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_LOSS_PER_UNIT_IDC1 = ConvertAbilityRealLevelField(FourCC('idc1'--[[1768186673--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SUMMONED_UNIT_DAMAGE_IDC2 = ConvertAbilityRealLevelField(FourCC('idc2'--[[1768186674--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ACTIVATION_DELAY_IMO2 = ConvertAbilityRealLevelField(FourCC('imo2'--[[1768779570--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LURE_INTERVAL_SECONDS = ConvertAbilityRealLevelField(FourCC('imo3'--[[1768779571--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_BONUS_ISR1 = ConvertAbilityRealLevelField(FourCC('isr1'--[[1769173553--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_REDUCTION_ISR2 = ConvertAbilityRealLevelField(FourCC('isr2'--[[1769173554--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_BONUS_IPV1 = ConvertAbilityRealLevelField(FourCC('ipv1'--[[1768977969--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LIFE_STEAL_AMOUNT = ConvertAbilityRealLevelField(FourCC('ipv2'--[[1768977970--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LIFE_RESTORED_FACTOR = ConvertAbilityRealLevelField(FourCC('ast1'--[[1634956337--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MANA_RESTORED_FACTOR = ConvertAbilityRealLevelField(FourCC('ast2'--[[1634956338--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACH_DELAY = ConvertAbilityRealLevelField(FourCC('gra1'--[[1735549233--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_REMOVE_DELAY = ConvertAbilityRealLevelField(FourCC('gra2'--[[1735549234--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HERO_REGENERATION_DELAY = ConvertAbilityRealLevelField(FourCC('Nsa2'--[[1316184370--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_UNIT_REGENERATION_DELAY = ConvertAbilityRealLevelField(FourCC('Nsa3'--[[1316184371--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_NSA4 = ConvertAbilityRealLevelField(FourCC('Nsa4'--[[1316184372--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HIT_POINTS_PER_SECOND_NSA5 = ConvertAbilityRealLevelField(FourCC('Nsa5'--[[1316184373--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_TO_SUMMONED_UNITS_IXS1 = ConvertAbilityRealLevelField(FourCC('Ixs1'--[[1232630577--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_IXS2 = ConvertAbilityRealLevelField(FourCC('Ixs2'--[[1232630578--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SUMMONED_UNIT_DURATION = ConvertAbilityRealLevelField(FourCC('Npa6'--[[1315987766--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SHIELD_COOLDOWN_TIME = ConvertAbilityRealLevelField(FourCC('Nse1'--[[1316185393--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_NDO1 = ConvertAbilityRealLevelField(FourCC('Ndo1'--[[1315204913--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SUMMONED_UNIT_DURATION_SECONDS_NDO3 = ConvertAbilityRealLevelField(FourCC('Ndo3'--[[1315204915--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MEDIUM_DAMAGE_RADIUS_FLK1 = ConvertAbilityRealLevelField(FourCC('flk1'--[[1718381361--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SMALL_DAMAGE_RADIUS_FLK2 = ConvertAbilityRealLevelField(FourCC('flk2'--[[1718381362--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_FULL_DAMAGE_AMOUNT_FLK3 = ConvertAbilityRealLevelField(FourCC('flk3'--[[1718381363--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MEDIUM_DAMAGE_AMOUNT = ConvertAbilityRealLevelField(FourCC('flk4'--[[1718381364--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SMALL_DAMAGE_AMOUNT = ConvertAbilityRealLevelField(FourCC('flk5'--[[1718381365--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_PERCENT_HBN1 = ConvertAbilityRealLevelField(FourCC('Hbn1'--[[1214410289--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_REDUCTION_PERCENT_HBN2 = ConvertAbilityRealLevelField(FourCC('Hbn2'--[[1214410290--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAX_MANA_DRAINED_UNITS = ConvertAbilityRealLevelField(FourCC('fbk1'--[[1717726001--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_RATIO_UNITS_PERCENT = ConvertAbilityRealLevelField(FourCC('fbk2'--[[1717726002--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAX_MANA_DRAINED_HEROS = ConvertAbilityRealLevelField(FourCC('fbk3'--[[1717726003--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_RATIO_HEROS_PERCENT = ConvertAbilityRealLevelField(FourCC('fbk4'--[[1717726004--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SUMMONED_DAMAGE = ConvertAbilityRealLevelField(FourCC('fbk5'--[[1717726005--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DISTRIBUTED_DAMAGE_FACTOR_NCA1 = ConvertAbilityRealLevelField(FourCC('nca1'--[[1852006705--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_INITIAL_DAMAGE_PXF1 = ConvertAbilityRealLevelField(FourCC('pxf1'--[[1886938673--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_PXF2 = ConvertAbilityRealLevelField(FourCC('pxf2'--[[1886938674--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PER_SECOND_MLS1 = ConvertAbilityRealLevelField(FourCC('mls1'--[[1835823921--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BEAST_COLLISION_RADIUS = ConvertAbilityRealLevelField(FourCC('Nst2'--[[1316189234--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_AMOUNT_NST3 = ConvertAbilityRealLevelField(FourCC('Nst3'--[[1316189235--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_RADIUS = ConvertAbilityRealLevelField(FourCC('Nst4'--[[1316189236--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_DELAY = ConvertAbilityRealLevelField(FourCC('Nst5'--[[1316189237--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_FOLLOW_THROUGH_TIME = ConvertAbilityRealLevelField(FourCC('Ncl1'--[[1315138609--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ART_DURATION = ConvertAbilityRealLevelField(FourCC('Ncl4'--[[1315138612--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_PERCENT_NAB1 = ConvertAbilityRealLevelField(FourCC('Nab1'--[[1315004977--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_REDUCTION_PERCENT_NAB2 = ConvertAbilityRealLevelField(FourCC('Nab2'--[[1315004978--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_PRIMARY_DAMAGE = ConvertAbilityRealLevelField(FourCC('Nab4'--[[1315004980--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SECONDARY_DAMAGE = ConvertAbilityRealLevelField(FourCC('Nab5'--[[1315004981--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_INTERVAL_NAB6 = ConvertAbilityRealLevelField(FourCC('Nab6'--[[1315004982--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_GOLD_COST_FACTOR = ConvertAbilityRealLevelField(FourCC('Ntm1'--[[1316252977--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LUMBER_COST_FACTOR = ConvertAbilityRealLevelField(FourCC('Ntm2'--[[1316252978--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVE_SPEED_BONUS_NEG1 = ConvertAbilityRealLevelField(FourCC('Neg1'--[[1315268401--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_BONUS_NEG2 = ConvertAbilityRealLevelField(FourCC('Neg2'--[[1315268402--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_AMOUNT_NCS1 = ConvertAbilityRealLevelField(FourCC('Ncs1'--[[1315140401--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_INTERVAL_NCS2 = ConvertAbilityRealLevelField(FourCC('Ncs2'--[[1315140402--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAX_DAMAGE_NCS4 = ConvertAbilityRealLevelField(FourCC('Ncs4'--[[1315140404--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BUILDING_DAMAGE_FACTOR_NCS5 = ConvertAbilityRealLevelField(FourCC('Ncs5'--[[1315140405--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_EFFECT_DURATION = ConvertAbilityRealLevelField(FourCC('Ncs6'--[[1315140406--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SPAWN_INTERVAL_NSY1 = ConvertAbilityRealLevelField(FourCC('Nsy1'--[[1316190513--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SPAWN_UNIT_DURATION = ConvertAbilityRealLevelField(FourCC('Nsy3'--[[1316190515--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SPAWN_UNIT_OFFSET = ConvertAbilityRealLevelField(FourCC('Nsy4'--[[1316190516--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LEASH_RANGE_NSY5 = ConvertAbilityRealLevelField(FourCC('Nsy5'--[[1316190517--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SPAWN_INTERVAL_NFY1 = ConvertAbilityRealLevelField(FourCC('Nfy1'--[[1315338545--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LEASH_RANGE_NFY2 = ConvertAbilityRealLevelField(FourCC('Nfy2'--[[1315338546--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_CHANCE_TO_DEMOLISH = ConvertAbilityRealLevelField(FourCC('Nde1'--[[1315202353--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_MULTIPLIER_BUILDINGS = ConvertAbilityRealLevelField(FourCC('Nde2'--[[1315202354--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_MULTIPLIER_UNITS = ConvertAbilityRealLevelField(FourCC('Nde3'--[[1315202355--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_MULTIPLIER_HEROES = ConvertAbilityRealLevelField(FourCC('Nde4'--[[1315202356--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BONUS_DAMAGE_MULTIPLIER = ConvertAbilityRealLevelField(FourCC('Nic1'--[[1315529521--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DEATH_DAMAGE_FULL_AMOUNT = ConvertAbilityRealLevelField(FourCC('Nic2'--[[1315529522--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DEATH_DAMAGE_FULL_AREA = ConvertAbilityRealLevelField(FourCC('Nic3'--[[1315529523--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DEATH_DAMAGE_HALF_AMOUNT = ConvertAbilityRealLevelField(FourCC('Nic4'--[[1315529524--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DEATH_DAMAGE_HALF_AREA = ConvertAbilityRealLevelField(FourCC('Nic5'--[[1315529525--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DEATH_DAMAGE_DELAY = ConvertAbilityRealLevelField(FourCC('Nic6'--[[1315529526--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_AMOUNT_NSO1 = ConvertAbilityRealLevelField(FourCC('Nso1'--[[1316187953--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PERIOD = ConvertAbilityRealLevelField(FourCC('Nso2'--[[1316187954--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_DAMAGE_PENALTY = ConvertAbilityRealLevelField(FourCC('Nso3'--[[1316187955--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_PERCENT_NSO4 = ConvertAbilityRealLevelField(FourCC('Nso4'--[[1316187956--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_ATTACK_SPEED_REDUCTION_PERCENT_NSO5 = ConvertAbilityRealLevelField(FourCC('Nso5'--[[1316187957--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_SPLIT_DELAY = ConvertAbilityRealLevelField(FourCC('Nlm2'--[[1315728690--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_MAX_HITPOINT_FACTOR = ConvertAbilityRealLevelField(FourCC('Nlm4'--[[1315728692--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_LIFE_DURATION_SPLIT_BONUS = ConvertAbilityRealLevelField(FourCC('Nlm5'--[[1315728693--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_WAVE_INTERVAL = ConvertAbilityRealLevelField(FourCC('Nvc3'--[[1316381491--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_BUILDING_DAMAGE_FACTOR_NVC4 = ConvertAbilityRealLevelField(FourCC('Nvc4'--[[1316381492--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_FULL_DAMAGE_AMOUNT_NVC5 = ConvertAbilityRealLevelField(FourCC('Nvc5'--[[1316381493--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_HALF_DAMAGE_FACTOR = ConvertAbilityRealLevelField(FourCC('Nvc6'--[[1316381494--]])) ---@type abilityreallevelfield @UjAPI
ABILITY_RLF_INTERVAL_BETWEEN_PULSES = ConvertAbilityRealLevelField(FourCC('Tau5'--[[1415673141--]])) ---@type abilityreallevelfield @UjAPI

ABILITY_BLF_PERCENT_BONUS_HAB2 = ConvertAbilityBooleanLevelField(FourCC('Hab2'--[[1214341682--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_USE_TELEPORT_CLUSTERING_HMT3 = ConvertAbilityBooleanLevelField(FourCC('Hmt3'--[[1215132723--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_NEVER_MISS_OCR5 = ConvertAbilityBooleanLevelField(FourCC('Ocr5'--[[1331917365--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_EXCLUDE_ITEM_DAMAGE = ConvertAbilityBooleanLevelField(FourCC('Ocr6'--[[1331917366--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_BACKSTAB_DAMAGE = ConvertAbilityBooleanLevelField(FourCC('Owk4'--[[1333226292--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_INHERIT_UPGRADES_UAN3 = ConvertAbilityBooleanLevelField(FourCC('Uan3'--[[1432448563--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_MANA_CONVERSION_AS_PERCENT = ConvertAbilityBooleanLevelField(FourCC('Udp3'--[[1432645683--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_LIFE_CONVERSION_AS_PERCENT = ConvertAbilityBooleanLevelField(FourCC('Udp4'--[[1432645684--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_LEAVE_TARGET_ALIVE = ConvertAbilityBooleanLevelField(FourCC('Udp5'--[[1432645685--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_PERCENT_BONUS_UAU3 = ConvertAbilityBooleanLevelField(FourCC('Uau3'--[[1432450355--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_DAMAGE_IS_PERCENT_RECEIVED = ConvertAbilityBooleanLevelField(FourCC('Eah2'--[[1164011570--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_MELEE_BONUS = ConvertAbilityBooleanLevelField(FourCC('Ear2'--[[1164014130--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_RANGED_BONUS = ConvertAbilityBooleanLevelField(FourCC('Ear3'--[[1164014131--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_FLAT_BONUS = ConvertAbilityBooleanLevelField(FourCC('Ear4'--[[1164014132--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_NEVER_MISS_HBH5 = ConvertAbilityBooleanLevelField(FourCC('Hbh5'--[[1214408757--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_PERCENT_BONUS_HAD2 = ConvertAbilityBooleanLevelField(FourCC('Had2'--[[1214342194--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_CAN_DEACTIVATE = ConvertAbilityBooleanLevelField(FourCC('Hds1'--[[1214542641--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_RAISED_UNITS_ARE_INVULNERABLE = ConvertAbilityBooleanLevelField(FourCC('Hre2'--[[1215456562--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_PERCENTAGE_OAR2 = ConvertAbilityBooleanLevelField(FourCC('Oar2'--[[1331786290--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_SUMMON_BUSY_UNITS = ConvertAbilityBooleanLevelField(FourCC('Btl2'--[[1114926130--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_CREATES_BLIGHT = ConvertAbilityBooleanLevelField(FourCC('Bli2'--[[1114401074--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_EXPLODES_ON_DEATH = ConvertAbilityBooleanLevelField(FourCC('Sds6'--[[1399092022--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_ALWAYS_AUTOCAST_FAE2 = ConvertAbilityBooleanLevelField(FourCC('Fae2'--[[1180788018--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_REGENERATE_ONLY_AT_NIGHT = ConvertAbilityBooleanLevelField(FourCC('Mbt5'--[[1298297909--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_SHOW_SELECT_UNIT_BUTTON = ConvertAbilityBooleanLevelField(FourCC('Neu3'--[[1315271987--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_SHOW_UNIT_INDICATOR = ConvertAbilityBooleanLevelField(FourCC('Neu4'--[[1315271988--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_CHARGE_OWNING_PLAYER = ConvertAbilityBooleanLevelField(FourCC('Ans6'--[[1097757494--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_PERCENTAGE_ARM2 = ConvertAbilityBooleanLevelField(FourCC('Arm2'--[[1098018098--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_TARGET_IS_INVULNERABLE = ConvertAbilityBooleanLevelField(FourCC('Pos3'--[[1349481267--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_TARGET_IS_MAGIC_IMMUNE = ConvertAbilityBooleanLevelField(FourCC('Pos4'--[[1349481268--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_KILL_ON_CASTER_DEATH = ConvertAbilityBooleanLevelField(FourCC('Ucb6'--[[1432576566--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_NO_TARGET_REQUIRED_REJ4 = ConvertAbilityBooleanLevelField(FourCC('Rej4'--[[1382378036--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_ACCEPTS_GOLD = ConvertAbilityBooleanLevelField(FourCC('Rtn1'--[[1383362097--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_ACCEPTS_LUMBER = ConvertAbilityBooleanLevelField(FourCC('Rtn2'--[[1383362098--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_PREFER_HOSTILES_ROA5 = ConvertAbilityBooleanLevelField(FourCC('Roa5'--[[1383031093--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_PREFER_FRIENDLIES_ROA6 = ConvertAbilityBooleanLevelField(FourCC('Roa6'--[[1383031094--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_ROOTED_TURNING = ConvertAbilityBooleanLevelField(FourCC('Roo3'--[[1383034675--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_ALWAYS_AUTOCAST_SLO3 = ConvertAbilityBooleanLevelField(FourCC('Slo3'--[[1399615283--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_HIDE_BUTTON = ConvertAbilityBooleanLevelField(FourCC('Ihid'--[[1231579492--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_USE_TELEPORT_CLUSTERING_ITP2 = ConvertAbilityBooleanLevelField(FourCC('Itp2'--[[1232367666--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_IMMUNE_TO_MORPH_EFFECTS = ConvertAbilityBooleanLevelField(FourCC('Eth1'--[[1165256753--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_DOES_NOT_BLOCK_BUILDINGS = ConvertAbilityBooleanLevelField(FourCC('Eth2'--[[1165256754--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_AUTO_ACQUIRE_ATTACK_TARGETS = ConvertAbilityBooleanLevelField(FourCC('Gho1'--[[1198026545--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_IMMUNE_TO_MORPH_EFFECTS_GHO2 = ConvertAbilityBooleanLevelField(FourCC('Gho2'--[[1198026546--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_DO_NOT_BLOCK_BUILDINGS = ConvertAbilityBooleanLevelField(FourCC('Gho3'--[[1198026547--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_INCLUDE_RANGED_DAMAGE = ConvertAbilityBooleanLevelField(FourCC('Ssk4'--[[1400073012--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_INCLUDE_MELEE_DAMAGE = ConvertAbilityBooleanLevelField(FourCC('Ssk5'--[[1400073013--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_MOVE_TO_PARTNER = ConvertAbilityBooleanLevelField(FourCC('coa2'--[[1668243762--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_CAN_BE_DISPELLED = ConvertAbilityBooleanLevelField(FourCC('cyc1'--[[1668899633--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_IGNORE_FRIENDLY_BUFFS = ConvertAbilityBooleanLevelField(FourCC('dvm6'--[[1685482806--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_DROP_ITEMS_ON_DEATH = ConvertAbilityBooleanLevelField(FourCC('inv2'--[[1768846898--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_CAN_USE_ITEMS = ConvertAbilityBooleanLevelField(FourCC('inv3'--[[1768846899--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_CAN_GET_ITEMS = ConvertAbilityBooleanLevelField(FourCC('inv4'--[[1768846900--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_CAN_DROP_ITEMS = ConvertAbilityBooleanLevelField(FourCC('inv5'--[[1768846901--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_REPAIRS_ALLOWED = ConvertAbilityBooleanLevelField(FourCC('liq4'--[[1818849588--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_CASTER_ONLY_SPLASH = ConvertAbilityBooleanLevelField(FourCC('mfl6'--[[1835428918--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_NO_TARGET_REQUIRED_IRL4 = ConvertAbilityBooleanLevelField(FourCC('irl4'--[[1769106484--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_DISPEL_ON_ATTACK = ConvertAbilityBooleanLevelField(FourCC('irl5'--[[1769106485--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_AMOUNT_IS_RAW_VALUE = ConvertAbilityBooleanLevelField(FourCC('ipv3'--[[1768977971--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_SHARED_SPELL_COOLDOWN = ConvertAbilityBooleanLevelField(FourCC('spb2'--[[1936745010--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_SLEEP_ONCE = ConvertAbilityBooleanLevelField(FourCC('sla1'--[[1936482609--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_ALLOW_ON_ANY_PLAYER_SLOT = ConvertAbilityBooleanLevelField(FourCC('sla2'--[[1936482610--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_DISABLE_OTHER_ABILITIES = ConvertAbilityBooleanLevelField(FourCC('Ncl5'--[[1315138613--]])) ---@type abilitybooleanlevelfield @UjAPI
ABILITY_BLF_ALLOW_BOUNTY = ConvertAbilityBooleanLevelField(FourCC('Ntm4'--[[1316252980--]])) ---@type abilitybooleanlevelfield @UjAPI

ABILITY_SLF_ICON_NORMAL = ConvertAbilityStringLevelField(FourCC('aart'--[[1633776244--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_CASTER = ConvertAbilityStringLevelField(FourCC('acat'--[[1633902964--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_TARGET = ConvertAbilityStringLevelField(FourCC('atat'--[[1635017076--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_SPECIAL = ConvertAbilityStringLevelField(FourCC('asat'--[[1634951540--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_EFFECT = ConvertAbilityStringLevelField(FourCC('aeat'--[[1634034036--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_AREA_EFFECT = ConvertAbilityStringLevelField(FourCC('aaea'--[[1633772897--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_LIGHTNING_EFFECTS = ConvertAbilityStringLevelField(FourCC('alig'--[[1634494823--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_MISSILE_ART = ConvertAbilityStringLevelField(FourCC('amat'--[[1634558324--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_TOOLTIP_LEARN = ConvertAbilityStringLevelField(FourCC('aret'--[[1634887028--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_TOOLTIP_LEARN_EXTENDED = ConvertAbilityStringLevelField(FourCC('arut'--[[1634891124--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_TOOLTIP_NORMAL = ConvertAbilityStringLevelField(FourCC('atp1'--[[1635020849--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_TOOLTIP_TURN_OFF = ConvertAbilityStringLevelField(FourCC('aut1'--[[1635087409--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_TOOLTIP_NORMAL_EXTENDED = ConvertAbilityStringLevelField(FourCC('aub1'--[[1635082801--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_TOOLTIP_TURN_OFF_EXTENDED = ConvertAbilityStringLevelField(FourCC('auu1'--[[1635087665--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_NORMAL_FORM_UNIT_EME1 = ConvertAbilityStringLevelField(FourCC('Eme1'--[[1164797233--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_SPAWNED_UNITS = ConvertAbilityStringLevelField(FourCC('Ndp1'--[[1315205169--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_ABILITY_FOR_UNIT_CREATION = ConvertAbilityStringLevelField(FourCC('Nrc1'--[[1316119345--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_NORMAL_FORM_UNIT_MIL1 = ConvertAbilityStringLevelField(FourCC('Mil1'--[[1298754609--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_ALTERNATE_FORM_UNIT_MIL2 = ConvertAbilityStringLevelField(FourCC('Mil2'--[[1298754610--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_BASE_ORDER_ID_ANS5 = ConvertAbilityStringLevelField(FourCC('Ans5'--[[1097757493--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_MORPH_UNITS_GROUND = ConvertAbilityStringLevelField(FourCC('Ply2'--[[1349286194--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_MORPH_UNITS_AIR = ConvertAbilityStringLevelField(FourCC('Ply3'--[[1349286195--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_MORPH_UNITS_AMPHIBIOUS = ConvertAbilityStringLevelField(FourCC('Ply4'--[[1349286196--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_MORPH_UNITS_WATER = ConvertAbilityStringLevelField(FourCC('Ply5'--[[1349286197--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_UNIT_TYPE_ONE = ConvertAbilityStringLevelField(FourCC('Rai3'--[[1382115635--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_UNIT_TYPE_TWO = ConvertAbilityStringLevelField(FourCC('Rai4'--[[1382115636--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_UNIT_TYPE_SOD2 = ConvertAbilityStringLevelField(FourCC('Sod2'--[[1399809074--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_SUMMON_1_UNIT_TYPE = ConvertAbilityStringLevelField(FourCC('Ist1'--[[1232303153--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_SUMMON_2_UNIT_TYPE = ConvertAbilityStringLevelField(FourCC('Ist2'--[[1232303154--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_RACE_TO_CONVERT = ConvertAbilityStringLevelField(FourCC('Ndc1'--[[1315201841--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_PARTNER_UNIT_TYPE = ConvertAbilityStringLevelField(FourCC('coa1'--[[1668243761--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_PARTNER_UNIT_TYPE_ONE = ConvertAbilityStringLevelField(FourCC('dcp1'--[[1684238385--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_PARTNER_UNIT_TYPE_TWO = ConvertAbilityStringLevelField(FourCC('dcp2'--[[1684238386--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_REQUIRED_UNIT_TYPE = ConvertAbilityStringLevelField(FourCC('tpi1'--[[1953524017--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_CONVERTED_UNIT_TYPE = ConvertAbilityStringLevelField(FourCC('tpi2'--[[1953524018--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_SPELL_LIST = ConvertAbilityStringLevelField(FourCC('spb1'--[[1936745009--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_BASE_ORDER_ID_SPB5 = ConvertAbilityStringLevelField(FourCC('spb5'--[[1936745013--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_BASE_ORDER_ID_NCL6 = ConvertAbilityStringLevelField(FourCC('Ncl6'--[[1315138614--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_ABILITY_UPGRADE_1 = ConvertAbilityStringLevelField(FourCC('Neg3'--[[1315268403--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_ABILITY_UPGRADE_2 = ConvertAbilityStringLevelField(FourCC('Neg4'--[[1315268404--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_ABILITY_UPGRADE_3 = ConvertAbilityStringLevelField(FourCC('Neg5'--[[1315268405--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_ABILITY_UPGRADE_4 = ConvertAbilityStringLevelField(FourCC('Neg6'--[[1315268406--]])) ---@type abilitystringlevelfield @UjAPI
ABILITY_SLF_SPAWN_UNIT_ID_NSY2 = ConvertAbilityStringLevelField(FourCC('Nsy2'--[[1316190514--]])) ---@type abilitystringlevelfield @UjAPI

-- Buff
BUFF_SF_ICON_NORMAL = ConvertAbilityStringField(FourCC('fart'--[[1717662324--]])) ---@type abilitystringfield @UjAPI
BUFF_SF_TOOLTIP_NORMAL = ConvertAbilityStringField(FourCC('ftip'--[[1718905200--]])) ---@type abilitystringfield @UjAPI
BUFF_SF_TOOLTIP_NORMAL_EXTENDED = ConvertAbilityStringField(FourCC('fube'--[[1718968933--]])) ---@type abilitystringfield @UjAPI

-- Destructable
DESTRUCTABLE_SF_NAME = ConvertDestructableStringField(FourCC('bnam'--[[1651401069--]])) ---@type destructablestringfield @UjAPI
DESTRUCTABLE_SF_MODEL = ConvertDestructableStringField(FourCC('bfil'--[[1650878828--]])) ---@type destructablestringfield @UjAPI

-- Item
ITEM_IF_TINTING_COLOR = ConvertItemIntegerField(FourCC('iclt'--[[1768123508--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_TINTING_COLOR_RED = ConvertItemIntegerField(FourCC('iclr'--[[1768123506--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_TINTING_COLOR_GREEN = ConvertItemIntegerField(FourCC('iclg'--[[1768123495--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_TINTING_COLOR_BLUE = ConvertItemIntegerField(FourCC('iclb'--[[1768123490--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_TINTING_COLOR_ALPHA = ConvertItemIntegerField(FourCC('ical'--[[1768120684--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_ARMOR_TYPE = ConvertItemIntegerField(FourCC('iarm'--[[1767993965--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_COOLDOWN_GROUP = ConvertItemIntegerField(FourCC('icid'--[[1768122724--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_GOLD_COST = ConvertItemIntegerField(FourCC('igol'--[[1768386412--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_HIT_POINTS = ConvertItemIntegerField(FourCC('ihpc'--[[1768452195--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_LEVEL_UNCLASSIFIED = ConvertItemIntegerField(FourCC('ilvo'--[[1768715887--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_LEVEL = ConvertItemIntegerField(FourCC('ilev'--[[1768711542--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_LUMBER_COST = ConvertItemIntegerField(FourCC('ilum'--[[1768715629--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_NUMBER_OF_CHARGES = ConvertItemIntegerField(FourCC('iuse'--[[1769304933--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_PRIORITY = ConvertItemIntegerField(FourCC('ipri'--[[1768977001--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_STOCK_MAXIMUM = ConvertItemIntegerField(FourCC('isto'--[[1769174127--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_STOCK_REPLENISH_INTERVAL = ConvertItemIntegerField(FourCC('istr'--[[1769174130--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_STOCK_START_DELAY = ConvertItemIntegerField(FourCC('isst'--[[1769173876--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_MAX_HIT_POINTS = ConvertItemIntegerField(FourCC('ihtp'--[[1768453232--]])) ---@type itemintegerfield @UjAPI
ITEM_IF_HOTKEY = ConvertItemIntegerField(FourCC('ihot'--[[1768451956--]])) ---@type itemintegerfield @UjAPI

ITEM_RF_SCALING_VALUE = ConvertItemRealField(FourCC('isca'--[[1769169761--]])) ---@type itemrealfield @UjAPI
ITEM_RF_SELECTION_SIZE = ConvertItemRealField(FourCC('issc'--[[1769173859--]])) ---@type itemrealfield @UjAPI

ITEM_BF_ACTIVELY_USED = ConvertItemBooleanField(FourCC('iusa'--[[1769304929--]])) ---@type itembooleanfield @UjAPI
ITEM_BF_CAN_BE_DROPPED = ConvertItemBooleanField(FourCC('idro'--[[1768190575--]])) ---@type itembooleanfield @UjAPI
ITEM_BF_CAN_BE_SOLD_TO_MERCHANTS = ConvertItemBooleanField(FourCC('ipaw'--[[1768972663--]])) ---@type itembooleanfield @UjAPI
ITEM_BF_DROPPED_WHEN_CARRIER_DIES = ConvertItemBooleanField(FourCC('idrp'--[[1768190576--]])) ---@type itembooleanfield @UjAPI
ITEM_BF_IGNORE_COOLDOWN = ConvertItemBooleanField(FourCC('iicd'--[[1768514404--]])) ---@type itembooleanfield @UjAPI
ITEM_BF_INCLUDE_AS_RANDOM_CHOICE = ConvertItemBooleanField(FourCC('iprn'--[[1768977006--]])) ---@type itembooleanfield @UjAPI
ITEM_BF_PERISHABLE = ConvertItemBooleanField(FourCC('iper'--[[1768973682--]])) ---@type itembooleanfield @UjAPI
ITEM_BF_USE_AUTOMATICALLY_WHEN_ACQUIRED = ConvertItemBooleanField(FourCC('ipow'--[[1768976247--]])) ---@type itembooleanfield @UjAPI
ITEM_BF_VALID_FOR_TRANSFORMATION = ConvertItemBooleanField(FourCC('imor'--[[1768779634--]])) ---@type itembooleanfield @UjAPI

ITEM_SF_ABILITY_LIST = ConvertItemStringField(FourCC('iabi'--[[1767989865--]])) ---@type itemstringfield @UjAPI
ITEM_SF_ICON = ConvertItemStringField(FourCC('iico'--[[1768514415--]])) ---@type itemstringfield @UjAPI
ITEM_SF_MODEL_USED = ConvertItemStringField(FourCC('ifil'--[[1768319340--]])) ---@type itemstringfield @UjAPI
ITEM_SF_DESCRIPTION = ConvertItemStringField(FourCC('ides'--[[1768187251--]])) ---@type itemstringfield @UjAPI
ITEM_SF_NAME = ConvertItemStringField(FourCC('unam'--[[1970168173--]])) ---@type itemstringfield @UjAPI
ITEM_SF_TOOLTIP_NORMAL = ConvertItemStringField(FourCC('utip'--[[1970563440--]])) ---@type itemstringfield @UjAPI
ITEM_SF_TOOLTIP_EXTENDED = ConvertItemStringField(FourCC('utub'--[[1970566498--]])) ---@type itemstringfield @UjAPI

-- Unit
UNIT_IF_DEFENSE_TYPE = ConvertUnitIntegerField(FourCC('udty'--[[1969517689--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_ARMOR_TYPE = ConvertUnitIntegerField(FourCC('uarm'--[[1969320557--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_LOOPING_FADE_IN_RATE = ConvertUnitIntegerField(FourCC('ulfi'--[[1970038377--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_LOOPING_FADE_OUT_RATE = ConvertUnitIntegerField(FourCC('ulfo'--[[1970038383--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_STRENGTH_BASE = ConvertUnitIntegerField(FourCC('ustr'--[[1970500722--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_AGILITY_BASE = ConvertUnitIntegerField(FourCC('uagi'--[[1969317737--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_INTELLIGENCE_BASE = ConvertUnitIntegerField(FourCC('uint'--[[1969843828--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_STRENGTH = ConvertUnitIntegerField(FourCC('ustc'--[[1970500707--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_AGILITY = ConvertUnitIntegerField(FourCC('uagc'--[[1969317731--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_INTELLIGENCE = ConvertUnitIntegerField(FourCC('uinc'--[[1969843811--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_STRENGTH_PERMANENT = ConvertUnitIntegerField(FourCC('ustm'--[[1970500717--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_AGILITY_PERMANENT = ConvertUnitIntegerField(FourCC('uagm'--[[1969317741--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_INTELLIGENCE_PERMANENT = ConvertUnitIntegerField(FourCC('uinm'--[[1969843821--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_STRENGTH_WITH_BONUS = ConvertUnitIntegerField(FourCC('ustb'--[[1970500706--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_AGILITY_WITH_BONUS = ConvertUnitIntegerField(FourCC('uagb'--[[1969317730--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_INTELLIGENCE_WITH_BONUS = ConvertUnitIntegerField(FourCC('uinb'--[[1969843810--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_STRENGTH_BONUS = ConvertUnitIntegerField(FourCC('ust+'--[[1970500651--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_AGILITY_BONUS = ConvertUnitIntegerField(FourCC('uag+'--[[1969317675--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_INTELLIGENCE_BONUS = ConvertUnitIntegerField(FourCC('uin+'--[[1969843755--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_FOOD_USED = ConvertUnitIntegerField(FourCC('ufoo'--[[1969647471--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_FOOD_PRODUCED = ConvertUnitIntegerField(FourCC('ufma'--[[1969646945--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_GOLD_COST = ConvertUnitIntegerField(FourCC('ugol'--[[1969713004--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_GOLD_BOUNTY_AWARDED_NUMBER_OF_DICE = ConvertUnitIntegerField(FourCC('ubdi'--[[1969382505--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_GOLD_BOUNTY_AWARDED_BASE = ConvertUnitIntegerField(FourCC('ubba'--[[1969381985--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_GOLD_BOUNTY_AWARDED_SIDES_PER_DIE = ConvertUnitIntegerField(FourCC('ubsi'--[[1969386345--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_LUMBER_COST = ConvertUnitIntegerField(FourCC('ulum'--[[1970042221--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_LUMBER_BOUNTY_AWARDED_NUMBER_OF_DICE = ConvertUnitIntegerField(FourCC('ulbd'--[[1970037348--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_LUMBER_BOUNTY_AWARDED_BASE = ConvertUnitIntegerField(FourCC('ulba'--[[1970037345--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_LUMBER_BOUNTY_AWARDED_SIDES_PER_DIE = ConvertUnitIntegerField(FourCC('ulbs'--[[1970037363--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_LEVEL = ConvertUnitIntegerField(FourCC('ulev'--[[1970038134--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_FORMATION_RANK = ConvertUnitIntegerField(FourCC('ufor'--[[1969647474--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_ORIENTATION_INTERPOLATION = ConvertUnitIntegerField(FourCC('uori'--[[1970238057--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_ELEVATION_SAMPLE_POINTS = ConvertUnitIntegerField(FourCC('uept'--[[1969582196--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_PROPER_NAMES_COUNT = ConvertUnitIntegerField(FourCC('upru'--[[1970303605--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_PROPER_NAME_INDEX = ConvertUnitIntegerField(FourCC('uprd'--[[1970303588--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_PROPER_NAME_GENERATION = ConvertUnitIntegerField(FourCC('uprg'--[[1970303591--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_HOTKEY = ConvertUnitIntegerField(FourCC('uhot'--[[1969778548--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_TINTING_COLOR = ConvertUnitIntegerField(FourCC('uclt'--[[1969450100--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_TINTING_COLOR_RED = ConvertUnitIntegerField(FourCC('uclr'--[[1969450098--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_TINTING_COLOR_GREEN = ConvertUnitIntegerField(FourCC('uclg'--[[1969450087--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_TINTING_COLOR_BLUE = ConvertUnitIntegerField(FourCC('uclb'--[[1969450082--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_TINTING_COLOR_ALPHA = ConvertUnitIntegerField(FourCC('ucal'--[[1969447276--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_MOVE_TYPE = ConvertUnitIntegerField(FourCC('umvt'--[[1970108020--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_TARGETED_AS = ConvertUnitIntegerField(FourCC('utar'--[[1970561394--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_UNIT_CLASSIFICATION = ConvertUnitIntegerField(FourCC('utyp'--[[1970567536--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_HIT_POINTS_REGENERATION_TYPE = ConvertUnitIntegerField(FourCC('uhrt'--[[1969779316--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_PLACEMENT_PREVENTED_BY = ConvertUnitIntegerField(FourCC('upar'--[[1970299250--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_PRIMARY_ATTRIBUTE = ConvertUnitIntegerField(FourCC('upra'--[[1970303585--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_COLLISION_TYPE = ConvertUnitIntegerField(FourCC('ucot'--[[1969450868--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_PATHING_AI = ConvertUnitIntegerField(FourCC('upai'--[[1970299241--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_PATHING_TYPE = ConvertUnitIntegerField(FourCC('upat'--[[1970299252--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_SEPARATION_GROUP_NUMBER = ConvertUnitIntegerField(FourCC('urpg'--[[1970434151--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_SEPARATION_PARAMETER = ConvertUnitIntegerField(FourCC('urpp'--[[1970434160--]])) ---@type unitintegerfield @UjAPI
UNIT_IF_SEPARATION_PRIORITY = ConvertUnitIntegerField(FourCC('urpr'--[[1970434162--]])) ---@type unitintegerfield @UjAPI

UNIT_RF_STRENGTH_PER_LEVEL = ConvertUnitRealField(FourCC('ustp'--[[1970500720--]])) ---@type unitrealfield @UjAPI
UNIT_RF_AGILITY_PER_LEVEL = ConvertUnitRealField(FourCC('uagp'--[[1969317744--]])) ---@type unitrealfield @UjAPI
UNIT_RF_INTELLIGENCE_PER_LEVEL = ConvertUnitRealField(FourCC('uinp'--[[1969843824--]])) ---@type unitrealfield @UjAPI
UNIT_RF_HIT_POINTS_REGENERATION_RATE = ConvertUnitRealField(FourCC('uhpr'--[[1969778802--]])) ---@type unitrealfield @UjAPI
UNIT_RF_MANA_REGENERATION = ConvertUnitRealField(FourCC('umpr'--[[1970106482--]])) ---@type unitrealfield @UjAPI
UNIT_RF_DEATH_TIME = ConvertUnitRealField(FourCC('udtm'--[[1969517677--]])) ---@type unitrealfield @UjAPI
UNIT_RF_FLY_HEIGHT = ConvertUnitRealField(FourCC('ufyh'--[[1969650024--]])) ---@type unitrealfield @UjAPI
UNIT_RF_TURN_RATE = ConvertUnitRealField(FourCC('umvr'--[[1970108018--]])) ---@type unitrealfield @UjAPI
UNIT_RF_ELEVATION_SAMPLE_RADIUS = ConvertUnitRealField(FourCC('uerd'--[[1969582692--]])) ---@type unitrealfield @UjAPI
UNIT_RF_FOG_OF_WAR_SAMPLE_RADIUS = ConvertUnitRealField(FourCC('ufrd'--[[1969648228--]])) ---@type unitrealfield @UjAPI
UNIT_RF_MAXIMUM_PITCH_ANGLE_DEGREES = ConvertUnitRealField(FourCC('umxp'--[[1970108528--]])) ---@type unitrealfield @UjAPI
UNIT_RF_MAXIMUM_ROLL_ANGLE_DEGREES = ConvertUnitRealField(FourCC('umxr'--[[1970108530--]])) ---@type unitrealfield @UjAPI
UNIT_RF_SCALING_VALUE = ConvertUnitRealField(FourCC('usca'--[[1970496353--]])) ---@type unitrealfield @UjAPI
UNIT_RF_ANIMATION_RUN_SPEED = ConvertUnitRealField(FourCC('urun'--[[1970435438--]])) ---@type unitrealfield @UjAPI
UNIT_RF_SELECTION_SCALE = ConvertUnitRealField(FourCC('ussc'--[[1970500451--]])) ---@type unitrealfield @UjAPI
UNIT_RF_SELECTION_CIRCLE_HEIGHT = ConvertUnitRealField(FourCC('uslz'--[[1970498682--]])) ---@type unitrealfield @UjAPI
UNIT_RF_SHADOW_IMAGE_HEIGHT = ConvertUnitRealField(FourCC('ushh'--[[1970497640--]])) ---@type unitrealfield @UjAPI
UNIT_RF_SHADOW_IMAGE_WIDTH = ConvertUnitRealField(FourCC('ushw'--[[1970497655--]])) ---@type unitrealfield @UjAPI
UNIT_RF_SHADOW_IMAGE_CENTER_X = ConvertUnitRealField(FourCC('ushx'--[[1970497656--]])) ---@type unitrealfield @UjAPI
UNIT_RF_SHADOW_IMAGE_CENTER_Y = ConvertUnitRealField(FourCC('ushy'--[[1970497657--]])) ---@type unitrealfield @UjAPI
UNIT_RF_ANIMATION_WALK_SPEED = ConvertUnitRealField(FourCC('uwal'--[[1970757996--]])) ---@type unitrealfield @UjAPI
UNIT_RF_DEFENSE = ConvertUnitRealField(FourCC('udfc'--[[1969514083--]])) ---@type unitrealfield @UjAPI
UNIT_RF_SIGHT_RADIUS = ConvertUnitRealField(FourCC('usir'--[[1970497906--]])) ---@type unitrealfield @UjAPI
UNIT_RF_SIGHT_RADIUS_DAY = ConvertUnitRealField(FourCC('usid'--[[1970497892--]])) ---@type unitrealfield @UjAPI
UNIT_RF_SIGHT_RADIUS_NIGHT = ConvertUnitRealField(FourCC('usin'--[[1970497902--]])) ---@type unitrealfield @UjAPI
UNIT_RF_PRIORITY = ConvertUnitRealField(FourCC('upri'--[[1970303593--]])) ---@type unitrealfield @UjAPI
UNIT_RF_SPEED = ConvertUnitRealField(FourCC('umvc'--[[1970108003--]])) ---@type unitrealfield @UjAPI
UNIT_RF_OCCLUDER_HEIGHT = ConvertUnitRealField(FourCC('uocc'--[[1970234211--]])) ---@type unitrealfield @UjAPI
UNIT_RF_HP = ConvertUnitRealField(FourCC('uhpc'--[[1969778787--]])) ---@type unitrealfield @UjAPI
UNIT_RF_MANA = ConvertUnitRealField(FourCC('umpc'--[[1970106467--]])) ---@type unitrealfield @UjAPI
UNIT_RF_ACQUISITION_RANGE = ConvertUnitRealField(FourCC('uacq'--[[1969316721--]])) ---@type unitrealfield @UjAPI
UNIT_RF_CAST_BACK_SWING = ConvertUnitRealField(FourCC('ucbs'--[[1969447539--]])) ---@type unitrealfield @UjAPI
UNIT_RF_CAST_POINT = ConvertUnitRealField(FourCC('ucpt'--[[1969451124--]])) ---@type unitrealfield @UjAPI
UNIT_RF_MINIMUM_ATTACK_RANGE = ConvertUnitRealField(FourCC('uamn'--[[1969319278--]])) ---@type unitrealfield @UjAPI
UNIT_RF_COLLISION_SIZE = ConvertUnitRealField(FourCC('ucol'--[[1969450860--]])) ---@type unitrealfield @UjAPI
UNIT_RF_HEIGHT = ConvertUnitRealField(FourCC('umvh'--[[1970108008--]])) ---@type unitrealfield @UjAPI
UNIT_RF_HEIGHT_MINIMUM = ConvertUnitRealField(FourCC('umvf'--[[1970108006--]])) ---@type unitrealfield @UjAPI
UNIT_RF_SPEED_BASE = ConvertUnitRealField(FourCC('umvs'--[[1970108019--]])) ---@type unitrealfield @UjAPI
UNIT_RF_SPEED_MINIMUM = ConvertUnitRealField(FourCC('umis'--[[1970104691--]])) ---@type unitrealfield @UjAPI
UNIT_RF_SPEED_MAXIMUM = ConvertUnitRealField(FourCC('umas'--[[1970102643--]])) ---@type unitrealfield @UjAPI
-- Get Only Fields
UNIT_RF_HEALTH_FROM_BONUS_STRENGTH = ConvertUnitRealField(FourCC('uhs+'--[[1969779499--]])) ---@type unitrealfield @UjAPI
UNIT_RF_MANA_FROM_BONUS_INTELLIGENCE = ConvertUnitRealField(FourCC('umi+'--[[1970104619--]])) ---@type unitrealfield @UjAPI
UNIT_RF_DEFENSE_BONUS = ConvertUnitRealField(FourCC('udf+'--[[1969514027--]])) ---@type unitrealfield @UjAPI
UNIT_RF_DEFENSE_BASE = ConvertUnitRealField(FourCC('udfb'--[[1969514082--]])) ---@type unitrealfield @UjAPI
UNIT_RF_SPEED_BONUS = ConvertUnitRealField(FourCC('umv+'--[[1970107947--]])) ---@type unitrealfield @UjAPI

UNIT_BF_RAISABLE = ConvertUnitBooleanField(FourCC('urai'--[[1970430313--]])) ---@type unitbooleanfield @UjAPI
UNIT_BF_DECAYABLE = ConvertUnitBooleanField(FourCC('udec'--[[1969513827--]])) ---@type unitbooleanfield @UjAPI
UNIT_BF_IS_A_BUILDING = ConvertUnitBooleanField(FourCC('ubdg'--[[1969382503--]])) ---@type unitbooleanfield @UjAPI
UNIT_BF_USE_EXTENDED_LINE_OF_SIGHT = ConvertUnitBooleanField(FourCC('ulos'--[[1970040691--]])) ---@type unitbooleanfield @UjAPI
UNIT_BF_NEUTRAL_BUILDING_SHOWS_MINIMAP_ICON = ConvertUnitBooleanField(FourCC('unbm'--[[1970168429--]])) ---@type unitbooleanfield @UjAPI
UNIT_BF_HERO_HIDE_HERO_INTERFACE_ICON = ConvertUnitBooleanField(FourCC('uhhb'--[[1969776738--]])) ---@type unitbooleanfield @UjAPI
UNIT_BF_HERO_HIDE_HERO_MINIMAP_DISPLAY = ConvertUnitBooleanField(FourCC('uhhm'--[[1969776749--]])) ---@type unitbooleanfield @UjAPI
UNIT_BF_HERO_HIDE_HERO_DEATH_MESSAGE = ConvertUnitBooleanField(FourCC('uhhd'--[[1969776740--]])) ---@type unitbooleanfield @UjAPI
UNIT_BF_HIDE_MINIMAP_DISPLAY = ConvertUnitBooleanField(FourCC('uhom'--[[1969778541--]])) ---@type unitbooleanfield @UjAPI
UNIT_BF_SCALE_PROJECTILES = ConvertUnitBooleanField(FourCC('uscb'--[[1970496354--]])) ---@type unitbooleanfield @UjAPI
UNIT_BF_SELECTION_CIRCLE_ON_WATER = ConvertUnitBooleanField(FourCC('usew'--[[1970496887--]])) ---@type unitbooleanfield @UjAPI
UNIT_BF_HAS_WATER_SHADOW = ConvertUnitBooleanField(FourCC('ushr'--[[1970497650--]])) ---@type unitbooleanfield @UjAPI
UNIT_BF_SEPARATION_ENABLED = ConvertUnitBooleanField(FourCC('urpo'--[[1970434159--]])) ---@type unitbooleanfield @UjAPI

UNIT_SF_HERO_ABILITY_LIST = ConvertUnitStringField(FourCC('uhab'--[[1969774946--]])) ---@type unitstringfield @UjAPI
UNIT_SF_ABILITY_LIST = ConvertUnitStringField(FourCC('uabi'--[[1969316457--]])) ---@type unitstringfield @UjAPI
UNIT_SF_ICON_NORMAL = ConvertUnitStringField(FourCC('uico'--[[1969841007--]])) ---@type unitstringfield @UjAPI
UNIT_SF_ICON_SCORE_SCREEN = ConvertUnitStringField(FourCC('ussi'--[[1970500457--]])) ---@type unitstringfield @UjAPI
UNIT_SF_MODEL = ConvertUnitStringField(FourCC('umdl'--[[1970103404--]])) ---@type unitstringfield @UjAPI
UNIT_SF_PORTRAIT = ConvertUnitStringField(FourCC('upor'--[[1970302834--]])) ---@type unitstringfield @UjAPI
UNIT_SF_SHADOW_IMAGE_UNIT = ConvertUnitStringField(FourCC('ushu'--[[1970497653--]])) ---@type unitstringfield @UjAPI
UNIT_SF_GROUND_TEXTURE = ConvertUnitStringField(FourCC('uubs'--[[1970627187--]])) ---@type unitstringfield @UjAPI
UNIT_SF_SPECIAL = ConvertUnitStringField(FourCC('uspa'--[[1970499681--]])) ---@type unitstringfield @UjAPI
UNIT_SF_NAME = ConvertUnitStringField(FourCC('unam'--[[1970168173--]])) ---@type unitstringfield @UjAPI
UNIT_SF_PROPER_NAMES = ConvertUnitStringField(FourCC('upro'--[[1970303599--]])) ---@type unitstringfield @UjAPI
UNIT_SF_PROPER_NAME = ConvertUnitStringField(FourCC('uprn'--[[1970303598--]])) ---@type unitstringfield @UjAPI
UNIT_SF_TOOLTIP_AWAKEN = ConvertUnitStringField(FourCC('uawt'--[[1969321844--]])) ---@type unitstringfield @UjAPI
UNIT_SF_TOOLTIP_NORMAL = ConvertUnitStringField(FourCC('utip'--[[1970563440--]])) ---@type unitstringfield @UjAPI
UNIT_SF_TOOLTIP_EXTENDED = ConvertUnitStringField(FourCC('utub'--[[1970566498--]])) ---@type unitstringfield @UjAPI
UNIT_SF_TOOLTIP_REVIVE = ConvertUnitStringField(FourCC('utpr'--[[1970565234--]])) ---@type unitstringfield @UjAPI

-- Unit Weapon
UNIT_WEAPON_IF_ATTACKS_ENABLED = ConvertUnitWeaponIntegerField(FourCC('uaen'--[[1969317230--]])) ---@type unitweaponintegerfield @UjAPI
UNIT_WEAPON_IF_ATTACK_DAMAGE_NUMBER_OF_DICE = ConvertUnitWeaponIntegerField(FourCC('ua1d'--[[1969303908--]])) ---@type unitweaponintegerfield @UjAPI
UNIT_WEAPON_IF_ATTACK_DAMAGE_BASE = ConvertUnitWeaponIntegerField(FourCC('ua1b'--[[1969303906--]])) ---@type unitweaponintegerfield @UjAPI
UNIT_WEAPON_IF_ATTACK_DAMAGE_SIDES_PER_DIE = ConvertUnitWeaponIntegerField(FourCC('ua1s'--[[1969303923--]])) ---@type unitweaponintegerfield @UjAPI
UNIT_WEAPON_IF_ATTACK_MAXIMUM_NUMBER_OF_TARGETS = ConvertUnitWeaponIntegerField(FourCC('utc1'--[[1970561841--]])) ---@type unitweaponintegerfield @UjAPI
UNIT_WEAPON_IF_ATTACK_ATTACK_TYPE = ConvertUnitWeaponIntegerField(FourCC('ua1t'--[[1969303924--]])) ---@type unitweaponintegerfield @UjAPI
UNIT_WEAPON_IF_ATTACK_WEAPON_TYPE = ConvertUnitWeaponIntegerField(FourCC('ua1w'--[[1969303927--]])) ---@type unitweaponintegerfield @UjAPI
UNIT_WEAPON_IF_ATTACK_WEAPON_SOUND = ConvertUnitWeaponIntegerField(FourCC('ucs1'--[[1969451825--]])) ---@type unitweaponintegerfield @UjAPI
UNIT_WEAPON_IF_ATTACK_AREA_OF_EFFECT_TARGETS = ConvertUnitWeaponIntegerField(FourCC('ua1p'--[[1969303920--]])) ---@type unitweaponintegerfield @UjAPI
UNIT_WEAPON_IF_ATTACK_TARGETS_ALLOWED = ConvertUnitWeaponIntegerField(FourCC('ua1g'--[[1969303911--]])) ---@type unitweaponintegerfield @UjAPI
-- Get Only Fields
UNIT_WEAPON_IF_ATTACK_DAMAGE_BASE_MINIMUM = ConvertUnitWeaponIntegerField(FourCC('udm1'--[[1969515825--]])) ---@type unitweaponintegerfield @UjAPI
UNIT_WEAPON_IF_ATTACK_DAMAGE_BASE_MAXIMUM = ConvertUnitWeaponIntegerField(FourCC('udh1'--[[1969514545--]])) ---@type unitweaponintegerfield @UjAPI
UNIT_WEAPON_IF_ATTACK_DAMAGE_BONUS = ConvertUnitWeaponIntegerField(FourCC('ud1+'--[[1969500459--]])) ---@type unitweaponintegerfield @UjAPI
UNIT_WEAPON_IF_ATTACK_DAMAGE_BONUS_FROM_STATS = ConvertUnitWeaponIntegerField(FourCC('udbs'--[[1969513075--]])) ---@type unitweaponintegerfield @UjAPI

UNIT_WEAPON_RF_ATTACK_BACKSWING_POINT = ConvertUnitWeaponRealField(FourCC('ubs1'--[[1969386289--]])) ---@type unitweaponrealfield @UjAPI
UNIT_WEAPON_RF_ATTACK_DAMAGE_POINT = ConvertUnitWeaponRealField(FourCC('udp1'--[[1969516593--]])) ---@type unitweaponrealfield @UjAPI
UNIT_WEAPON_RF_ATTACK_BASE_COOLDOWN = ConvertUnitWeaponRealField(FourCC('ua1c'--[[1969303907--]])) ---@type unitweaponrealfield @UjAPI
UNIT_WEAPON_RF_ATTACK_DAMAGE_LOSS_FACTOR = ConvertUnitWeaponRealField(FourCC('udl1'--[[1969515569--]])) ---@type unitweaponrealfield @UjAPI
UNIT_WEAPON_RF_ATTACK_DAMAGE_FACTOR_MEDIUM = ConvertUnitWeaponRealField(FourCC('uhd1'--[[1969775665--]])) ---@type unitweaponrealfield @UjAPI
UNIT_WEAPON_RF_ATTACK_DAMAGE_FACTOR_SMALL = ConvertUnitWeaponRealField(FourCC('uqd1'--[[1970365489--]])) ---@type unitweaponrealfield @UjAPI
UNIT_WEAPON_RF_ATTACK_DAMAGE_SPILL_DISTANCE = ConvertUnitWeaponRealField(FourCC('usd1'--[[1970496561--]])) ---@type unitweaponrealfield @UjAPI
UNIT_WEAPON_RF_ATTACK_DAMAGE_SPILL_RADIUS = ConvertUnitWeaponRealField(FourCC('usr1'--[[1970500145--]])) ---@type unitweaponrealfield @UjAPI
UNIT_WEAPON_RF_ATTACK_PROJECTILE_SPEED = ConvertUnitWeaponRealField(FourCC('ua1z'--[[1969303930--]])) ---@type unitweaponrealfield @UjAPI
UNIT_WEAPON_RF_ATTACK_PROJECTILE_ARC = ConvertUnitWeaponRealField(FourCC('uma1'--[[1970102577--]])) ---@type unitweaponrealfield @UjAPI
UNIT_WEAPON_RF_ATTACK_AREA_OF_EFFECT_FULL_DAMAGE = ConvertUnitWeaponRealField(FourCC('ua1f'--[[1969303910--]])) ---@type unitweaponrealfield @UjAPI
UNIT_WEAPON_RF_ATTACK_AREA_OF_EFFECT_MEDIUM_DAMAGE = ConvertUnitWeaponRealField(FourCC('ua1h'--[[1969303912--]])) ---@type unitweaponrealfield @UjAPI
UNIT_WEAPON_RF_ATTACK_AREA_OF_EFFECT_SMALL_DAMAGE = ConvertUnitWeaponRealField(FourCC('ua1q'--[[1969303921--]])) ---@type unitweaponrealfield @UjAPI
UNIT_WEAPON_RF_ATTACK_RANGE = ConvertUnitWeaponRealField(FourCC('ua1r'--[[1969303922--]])) ---@type unitweaponrealfield @UjAPI
UNIT_WEAPON_RF_ATTACK_SPEED_BONUS = ConvertUnitWeaponRealField(FourCC('us1+'--[[1970483499--]])) ---@type unitweaponrealfield @UjAPI

UNIT_WEAPON_BF_ATTACK_SHOW_UI = ConvertUnitWeaponBooleanField(FourCC('uwu1'--[[1970763057--]])) ---@type unitweaponbooleanfield @UjAPI
UNIT_WEAPON_BF_ATTACKS_ENABLED = ConvertUnitWeaponBooleanField(FourCC('uaen'--[[1969317230--]])) ---@type unitweaponbooleanfield @UjAPI
UNIT_WEAPON_BF_ATTACK_PROJECTILE_HOMING_ENABLED = ConvertUnitWeaponBooleanField(FourCC('umh1'--[[1970104369--]])) ---@type unitweaponbooleanfield @UjAPI

UNIT_WEAPON_SF_ATTACK_PROJECTILE_ART = ConvertUnitWeaponStringField(FourCC('ua1m'--[[1969303917--]])) ---@type unitweaponstringfield @UjAPI

-- Move Type
MOVE_TYPE_UNKNOWN = ConvertMoveType(0) ---@type movetype @UjAPI
MOVE_TYPE_FOOT = ConvertMoveType(1) ---@type movetype @UjAPI
MOVE_TYPE_FLY = ConvertMoveType(2) ---@type movetype @UjAPI
MOVE_TYPE_HORSE = ConvertMoveType(4) ---@type movetype @UjAPI
MOVE_TYPE_HOVER = ConvertMoveType(8) ---@type movetype @UjAPI
MOVE_TYPE_FLOAT = ConvertMoveType(16) ---@type movetype @UjAPI
MOVE_TYPE_AMPHIBIOUS = ConvertMoveType(32) ---@type movetype @UjAPI
MOVE_TYPE_UNBUILDABLE = ConvertMoveType(64) ---@type movetype @UjAPI

-- Pathing AI Type
PATHING_AI_TYPE_FOOT = ConvertPathingAIType(0) ---@type pathingaitype @UjAPI
PATHING_AI_TYPE_AMPHIBIOUS = ConvertPathingAIType(64) ---@type pathingaitype @UjAPI
PATHING_AI_TYPE_FLOAT = ConvertPathingAIType(128) ---@type pathingaitype @UjAPI
PATHING_AI_TYPE_FLY = ConvertPathingAIType(192) ---@type pathingaitype @UjAPI

-- Collision Type
COLLISION_TYPE_NONE = ConvertCollisionType(0) ---@type collisiontype @UjAPI
COLLISION_TYPE_ANY = ConvertCollisionType(1) ---@type collisiontype @UjAPI
COLLISION_TYPE_FOOT = ConvertCollisionType(2) ---@type collisiontype @UjAPI
COLLISION_TYPE_AIR = ConvertCollisionType(4) ---@type collisiontype @UjAPI
COLLISION_TYPE_BUILDING = ConvertCollisionType(8) ---@type collisiontype @UjAPI
COLLISION_TYPE_HARVESTER = ConvertCollisionType(16) ---@type collisiontype @UjAPI
COLLISION_TYPE_BLIGHTED = ConvertCollisionType(32) ---@type collisiontype @UjAPI
COLLISION_TYPE_FLOAT = ConvertCollisionType(64) ---@type collisiontype @UjAPI
COLLISION_TYPE_AMPHIBIOUS = ConvertCollisionType(128) ---@type collisiontype @UjAPI
COLLISION_TYPE_GROUND = ConvertCollisionType(202) ---@type collisiontype @UjAPI

-- Target Flag
TARGET_FLAG_NONE = ConvertTargetFlag(1) ---@type targetflag @UjAPI
TARGET_FLAG_GROUND = ConvertTargetFlag(2) ---@type targetflag @UjAPI
TARGET_FLAG_AIR = ConvertTargetFlag(4) ---@type targetflag @UjAPI
TARGET_FLAG_STRUCTURE = ConvertTargetFlag(8) ---@type targetflag @UjAPI
TARGET_FLAG_WARD = ConvertTargetFlag(16) ---@type targetflag @UjAPI
TARGET_FLAG_ITEM = ConvertTargetFlag(32) ---@type targetflag @UjAPI
TARGET_FLAG_TREE = ConvertTargetFlag(64) ---@type targetflag @UjAPI
TARGET_FLAG_WALL = ConvertTargetFlag(128) ---@type targetflag @UjAPI
TARGET_FLAG_DEBRIS = ConvertTargetFlag(256) ---@type targetflag @UjAPI
TARGET_FLAG_DECORATION = ConvertTargetFlag(512) ---@type targetflag @UjAPI
TARGET_FLAG_BRIDGE = ConvertTargetFlag(1024) ---@type targetflag @UjAPI
TARGET_FLAG_SELF = ConvertTargetFlag(4096) ---@type targetflag @UjAPI
TARGET_FLAG_PLAYER_UNITS = ConvertTargetFlag(8192) ---@type targetflag @UjAPI
TARGET_FLAG_ALLIED = ConvertTargetFlag(16384) ---@type targetflag @UjAPI
TARGET_FLAG_FRIEND = ConvertTargetFlag(24576) ---@type targetflag @UjAPI
TARGET_FLAG_NEUTRAL = ConvertTargetFlag(32768) ---@type targetflag @UjAPI
TARGET_FLAG_ENEMY = ConvertTargetFlag(65536) ---@type targetflag @UjAPI
TARGET_FLAG_NOT_SELF = ConvertTargetFlag(122880) ---@type targetflag @UjAPI
TARGET_FLAG_VULNERABLE = ConvertTargetFlag(1048576) ---@type targetflag @UjAPI
TARGET_FLAG_INVULNERABLE = ConvertTargetFlag(2097152) ---@type targetflag @UjAPI
TARGET_FLAG_HERO = ConvertTargetFlag(4194304) ---@type targetflag @UjAPI
TARGET_FLAG_NON_HERO = ConvertTargetFlag(8388608) ---@type targetflag @UjAPI
TARGET_FLAG_ALIVE = ConvertTargetFlag(16777216) ---@type targetflag @UjAPI
TARGET_FLAG_DEAD = ConvertTargetFlag(33554432) ---@type targetflag @UjAPI
TARGET_FLAG_ORGANIC = ConvertTargetFlag(67108864) ---@type targetflag @UjAPI
TARGET_FLAG_MECHANICAL = ConvertTargetFlag(134217728) ---@type targetflag @UjAPI
TARGET_FLAG_NON_SUICIDAL = ConvertTargetFlag(268435456) ---@type targetflag @UjAPI
TARGET_FLAG_SUICIDAL = ConvertTargetFlag(536870912) ---@type targetflag @UjAPI
TARGET_FLAG_NON_ANCIENT = ConvertTargetFlag(1073741824) ---@type targetflag @UjAPI
TARGET_FLAG_ANCIENT = ConvertTargetFlag(2147483648) ---@type targetflag @UjAPI
TARGET_FLAG_EMPTY = ConvertTargetFlag(4294967295) ---@type targetflag @UjAPI

-- ability types
ABILITY_TYPE_POSITIVE = ConvertAbilityType(0) ---@type abilitytype @UjAPI
ABILITY_TYPE_NEGATIVE = ConvertAbilityType(1) ---@type abilitytype @UjAPI
ABILITY_TYPE_AURA = ConvertAbilityType(2) ---@type abilitytype @UjAPI
ABILITY_TYPE_BUFF = ConvertAbilityType(3) ---@type abilitytype @UjAPI
ABILITY_TYPE_TIMED_LIFE = ConvertAbilityType(4) ---@type abilitytype @UjAPI
ABILITY_TYPE_PHYSICAL = ConvertAbilityType(5) ---@type abilitytype @UjAPI
ABILITY_TYPE_MAGICAL = ConvertAbilityType(6) ---@type abilitytype @UjAPI
ABILITY_TYPE_AUTODISPEL = ConvertAbilityType(7) ---@type abilitytype @UjAPI

-- defense type
DEFENSE_TYPE_LIGHT = ConvertDefenseType(0) ---@type defensetype @UjAPI
DEFENSE_TYPE_MEDIUM = ConvertDefenseType(1) ---@type defensetype @UjAPI
DEFENSE_TYPE_LARGE = ConvertDefenseType(2) ---@type defensetype @UjAPI
DEFENSE_TYPE_FORT = ConvertDefenseType(3) ---@type defensetype @UjAPI
DEFENSE_TYPE_NORMAL = ConvertDefenseType(4) ---@type defensetype @UjAPI
DEFENSE_TYPE_HERO = ConvertDefenseType(5) ---@type defensetype @UjAPI
DEFENSE_TYPE_DIVINE = ConvertDefenseType(6) ---@type defensetype @UjAPI
DEFENSE_TYPE_NONE = ConvertDefenseType(7) ---@type defensetype @UjAPI

-- Hero Attribute
HERO_ATTRIBUTE_STR = ConvertHeroAttribute(1) ---@type heroattribute @UjAPI
HERO_ATTRIBUTE_INT = ConvertHeroAttribute(2) ---@type heroattribute @UjAPI
HERO_ATTRIBUTE_AGI = ConvertHeroAttribute(3) ---@type heroattribute @UjAPI

-- Armor Type
ARMOR_TYPE_WHOKNOWS = ConvertArmorType(0) ---@type armortype @UjAPI
ARMOR_TYPE_FLESH = ConvertArmorType(1) ---@type armortype @UjAPI
ARMOR_TYPE_METAL = ConvertArmorType(2) ---@type armortype @UjAPI
ARMOR_TYPE_WOOD = ConvertArmorType(3) ---@type armortype @UjAPI
ARMOR_TYPE_ETHREAL = ConvertArmorType(4) ---@type armortype @UjAPI
ARMOR_TYPE_STONE = ConvertArmorType(5) ---@type armortype @UjAPI

-- Regeneration Type
REGENERATION_TYPE_NONE = ConvertRegenType(0) ---@type regentype @UjAPI
REGENERATION_TYPE_ALWAYS = ConvertRegenType(1) ---@type regentype @UjAPI
REGENERATION_TYPE_BLIGHT = ConvertRegenType(2) ---@type regentype @UjAPI
REGENERATION_TYPE_DAY = ConvertRegenType(3) ---@type regentype @UjAPI
REGENERATION_TYPE_NIGHT = ConvertRegenType(4) ---@type regentype @UjAPI

-- Unit Category
UNIT_CATEGORY_GIANT = ConvertUnitCategory(1) ---@type unitcategory @UjAPI
UNIT_CATEGORY_UNDEAD = ConvertUnitCategory(2) ---@type unitcategory @UjAPI
UNIT_CATEGORY_SUMMONED = ConvertUnitCategory(4) ---@type unitcategory @UjAPI
UNIT_CATEGORY_MECHANICAL = ConvertUnitCategory(8) ---@type unitcategory @UjAPI
UNIT_CATEGORY_PEON = ConvertUnitCategory(16) ---@type unitcategory @UjAPI
UNIT_CATEGORY_SAPPER = ConvertUnitCategory(32) ---@type unitcategory @UjAPI
UNIT_CATEGORY_TOWNHALL = ConvertUnitCategory(64) ---@type unitcategory @UjAPI
UNIT_CATEGORY_ANCIENT = ConvertUnitCategory(128) ---@type unitcategory @UjAPI
UNIT_CATEGORY_NEUTRAL = ConvertUnitCategory(256) ---@type unitcategory @UjAPI
UNIT_CATEGORY_WARD = ConvertUnitCategory(512) ---@type unitcategory @UjAPI
UNIT_CATEGORY_STANDON = ConvertUnitCategory(1024) ---@type unitcategory @UjAPI
UNIT_CATEGORY_TAUREN = ConvertUnitCategory(2048) ---@type unitcategory @UjAPI

-- Pathing Flag
PATHING_FLAG_UNWALKABLE = ConvertPathingFlag(2) ---@type pathingflag @UjAPI
PATHING_FLAG_UNFLYABLE = ConvertPathingFlag(4) ---@type pathingflag @UjAPI
PATHING_FLAG_UNBUILDABLE = ConvertPathingFlag(8) ---@type pathingflag @UjAPI
PATHING_FLAG_UNPEONHARVEST = ConvertPathingFlag(16) ---@type pathingflag @UjAPI
PATHING_FLAG_BLIGHTED = ConvertPathingFlag(32) ---@type pathingflag @UjAPI
PATHING_FLAG_UNFLOATABLE = ConvertPathingFlag(64) ---@type pathingflag @UjAPI
PATHING_FLAG_UNAMPHIBIOUS = ConvertPathingFlag(128) ---@type pathingflag @UjAPI
PATHING_FLAG_UNITEMPLACABLE = ConvertPathingFlag(256) ---@type pathingflag @UjAPI

TIME_TYPE_YEAR = ConvertTimeType(0) ---@type timetype @UjAPI
TIME_TYPE_MONTH = ConvertTimeType(1) ---@type timetype @UjAPI
TIME_TYPE_DAY_OF_WEEK = ConvertTimeType(2) ---@type timetype @UjAPI
TIME_TYPE_DAY = ConvertTimeType(3) ---@type timetype @UjAPI
TIME_TYPE_HOUR = ConvertTimeType(4) ---@type timetype @UjAPI
TIME_TYPE_MINUTE = ConvertTimeType(5) ---@type timetype @UjAPI
TIME_TYPE_SECOND = ConvertTimeType(6) ---@type timetype @UjAPI
TIME_TYPE_MILLISECOND = ConvertTimeType(7) ---@type timetype @UjAPI

VARIABLE_TYPE_NOTHING = ConvertVariableType(0) ---@type variabletype @UjAPI
VARIABLE_TYPE_UNKNOWN = ConvertVariableType(1) ---@type variabletype @UjAPI
VARIABLE_TYPE_NULL = ConvertVariableType(2) ---@type variabletype @UjAPI
VARIABLE_TYPE_CODE = ConvertVariableType(3) ---@type variabletype @UjAPI
VARIABLE_TYPE_INTEGER = ConvertVariableType(4) ---@type variabletype @UjAPI
VARIABLE_TYPE_REAL = ConvertVariableType(5) ---@type variabletype @UjAPI
VARIABLE_TYPE_STRING = ConvertVariableType(6) ---@type variabletype @UjAPI
VARIABLE_TYPE_HANDLE = ConvertVariableType(7) ---@type variabletype @UjAPI
VARIABLE_TYPE_BOOLEAN = ConvertVariableType(8) ---@type variabletype @UjAPI
VARIABLE_TYPE_INTEGER_ARRAY = ConvertVariableType(9) ---@type variabletype @UjAPI
VARIABLE_TYPE_REAL_ARRAY = ConvertVariableType(10) ---@type variabletype @UjAPI
VARIABLE_TYPE_STRING_ARRAY = ConvertVariableType(11) ---@type variabletype @UjAPI
VARIABLE_TYPE_HANDLE_ARRAY = ConvertVariableType(12) ---@type variabletype @UjAPI
VARIABLE_TYPE_BOOLEAN_ARRAY = ConvertVariableType(13) ---@type variabletype @UjAPI

RENDER_STAGE_SKYBOX = ConvertRenderStage(0) ---@type renderstage @UjAPI
RENDER_STAGE_TERRAIN = ConvertRenderStage(1) ---@type renderstage @UjAPI
RENDER_STAGE_FOG_OF_WAR = ConvertRenderStage(2) ---@type renderstage @UjAPI
RENDER_STAGE_TERRAIN_SHADOW = ConvertRenderStage(3) ---@type renderstage @UjAPI
RENDER_STAGE_WATER_SHADOW = ConvertRenderStage(4) ---@type renderstage @UjAPI
RENDER_STAGE_BLIGHT = ConvertRenderStage(5) ---@type renderstage @UjAPI
RENDER_STAGE_WEATHER = ConvertRenderStage(6) ---@type renderstage @UjAPI
RENDER_STAGE_SELECTION_CIRCLE = ConvertRenderStage(7) ---@type renderstage @UjAPI
RENDER_STAGE_INDICATOR = ConvertRenderStage(8) ---@type renderstage @UjAPI
RENDER_STAGE_FOOTPRINT = ConvertRenderStage(9) ---@type renderstage @UjAPI
RENDER_STAGE_DOODAD = ConvertRenderStage(10) ---@type renderstage @UjAPI
RENDER_STAGE_SPRITE = ConvertRenderStage(11) ---@type renderstage @UjAPI
RENDER_STAGE_SELECTION_SPRITE = ConvertRenderStage(12) ---@type renderstage @UjAPI
RENDER_STAGE_DECORATION = ConvertRenderStage(13) ---@type renderstage @UjAPI
RENDER_STAGE_WATER = ConvertRenderStage(14) ---@type renderstage @UjAPI
RENDER_STAGE_SELECTION_BOX = ConvertRenderStage(15) ---@type renderstage @UjAPI
RENDER_STAGE_TEXTURE = ConvertRenderStage(16) ---@type renderstage @UjAPI
RENDER_STAGE_OCCLUSION_MASK = ConvertRenderStage(17) ---@type renderstage @UjAPI
RENDER_STAGE_BUILDING = ConvertRenderStage(18) ---@type renderstage @UjAPI
RENDER_STAGE_UBERSPLAT = ConvertRenderStage(19) ---@type renderstage @UjAPI
RENDER_STAGE_LIGHTNING = ConvertRenderStage(20) ---@type renderstage @UjAPI
RENDER_STAGE_TEXTTAG = ConvertRenderStage(21) ---@type renderstage @UjAPI

LAYOUT_STYLE_BOUNDING_FRAME_POSITION = ConvertLayoutStyleFlag(1) ---@type layoutstyleflag @UjAPI
LAYOUT_STYLE_ALWAYS_TRACK = ConvertLayoutStyleFlag(2) ---@type layoutstyleflag @UjAPI
LAYOUT_STYLE_NO_ENV = ConvertLayoutStyleFlag(4) ---@type layoutstyleflag @UjAPI

GRID_STYLE_JUSTIFY_LEFT = ConvertGridStyleFlag(8) ---@type gridstyleflag @UjAPI
GRID_STYLE_JUSTIFY_RIGHT = ConvertGridStyleFlag(16) ---@type gridstyleflag @UjAPI
GRID_STYLE_JUSTIFY_TOP = ConvertGridStyleFlag(32) ---@type gridstyleflag @UjAPI
GRID_STYLE_JUSTIFY_BOTTOM = ConvertGridStyleFlag(64) ---@type gridstyleflag @UjAPI
GRID_STYLE_JUSTIFY_MIDDLE = ConvertGridStyleFlag(128) ---@type gridstyleflag @UjAPI
GRID_STYLE_JUSTIFY_CENTER = ConvertGridStyleFlag(256) ---@type gridstyleflag @UjAPI
GRID_STYLE_PACK_ITEMS = ConvertGridStyleFlag(512) ---@type gridstyleflag @UjAPI

LAYER_STYLE_SVIEWPOINT = ConvertLayerStyleFlag(1) ---@type layerstyleflag @UjAPI
LAYER_STYLE_IGNORE_TRACK_EVENTS = ConvertLayerStyleFlag(2) ---@type layerstyleflag @UjAPI
LAYER_STYLE_SHADING = ConvertLayerStyleFlag(4) ---@type layerstyleflag @UjAPI
LAYER_STYLE_SCROLL = ConvertLayerStyleFlag(8) ---@type layerstyleflag @UjAPI
LAYER_STYLE_NO_DEPTH_SET = ConvertLayerStyleFlag(16) ---@type layerstyleflag @UjAPI
LAYER_STYLE_NO_DEPTH_TEST = ConvertLayerStyleFlag(32) ---@type layerstyleflag @UjAPI

CONTROL_STYLE_AUTOTRACK = ConvertControlStyleFlag(1) ---@type controlstyleflag @UjAPI
CONTROL_STYLE_CLICK_MOUSE_DOWN = ConvertControlStyleFlag(2) ---@type controlstyleflag @UjAPI
CONTROL_STYLE_RELEASE_NOTIFY = ConvertControlStyleFlag(4) ---@type controlstyleflag @UjAPI
CONTROL_STYLE_DRAG = ConvertControlStyleFlag(8) ---@type controlstyleflag @UjAPI
CONTROL_STYLE_HIGHLIGHT_ON_FOCUS = ConvertControlStyleFlag(16) ---@type controlstyleflag @UjAPI
CONTROL_STYLE_DRAW = ConvertControlStyleFlag(32) ---@type controlstyleflag @UjAPI
CONTROL_STYLE_HIGHLIGHT_FOCUS = ConvertControlStyleFlag(32) ---@type controlstyleflag @UjAPI
CONTROL_STYLE_HIGHLIGHT_ON_MOUSE_OVER = ConvertControlStyleFlag(64) ---@type controlstyleflag @UjAPI
CONTROL_STYLE_SLIDER_STEP = ConvertControlStyleFlag(128) ---@type controlstyleflag @UjAPI
CONTROL_STYLE_HIGHLIGHT = ConvertControlStyleFlag(256) ---@type controlstyleflag @UjAPI
CONTROL_STYLE_EXCLUSIVE = ConvertControlStyleFlag(512) ---@type controlstyleflag @UjAPI
CONTROL_STYLE_AT_LEAST_ONE = ConvertControlStyleFlag(1024) ---@type controlstyleflag @UjAPI

FRAME_STATE_SIMPLE_BUTTON_DISABLED = ConvertFrameState(0) ---@type framestate @UjAPI
FRAME_STATE_SIMPLE_BUTTON_ENABLED = ConvertFrameState(1) ---@type framestate @UjAPI
FRAME_STATE_SIMPLE_BUTTON_PUSHED = ConvertFrameState(2) ---@type framestate @UjAPI
FRAME_STATE_SIMPLE_BUTTON_CURRENT = ConvertFrameState(3) ---@type framestate @UjAPI

FRAME_STATE_CONTROL_CURRENT = ConvertFrameState(0) ---@type framestate @UjAPI
FRAME_STATE_CONTROL_ENABLED = ConvertFrameState(1) ---@type framestate @UjAPI
FRAME_STATE_CONTROL_PUSHED = ConvertFrameState(2) ---@type framestate @UjAPI
FRAME_STATE_CONTROL_DISABLED = ConvertFrameState(3) ---@type framestate @UjAPI
FRAME_STATE_CONTROL_CHECK_ENABLED = ConvertFrameState(5) ---@type framestate @UjAPI
FRAME_STATE_CONTROL_CHECK_DISABLED = ConvertFrameState(6) ---@type framestate @UjAPI

BORDER_FLAG_UPPER_LEFT = 1 ---@type integer @UjAPI
BORDER_FLAG_UPPER_RIGHT = 2 ---@type integer @UjAPI
BORDER_FLAG_BOTTOM_LEFT = 4 ---@type integer @UjAPI
BORDER_FLAG_BOTTOM_RIGHT = 8 ---@type integer @UjAPI
BORDER_FLAG_TOP = 16 ---@type integer @UjAPI
BORDER_FLAG_LEFT = 32 ---@type integer @UjAPI
BORDER_FLAG_BOTTOM = 64 ---@type integer @UjAPI
BORDER_FLAG_RIGHT = 128 ---@type integer @UjAPI
BORDER_FLAG_ALL = 255 ---@type integer @UjAPI

CONNECTION_TYPE_SINGLE_PLAYER = ConvertConnectionType(0) ---@type connectiontype @UjAPI
CONNECTION_TYPE_LOCAL_GAME = ConvertConnectionType(1) ---@type connectiontype @UjAPI
CONNECTION_TYPE_BATTLE_NET = ConvertConnectionType(2) ---@type connectiontype @UjAPI
CONNECTION_TYPE_REPLAY = ConvertConnectionType(3) ---@type connectiontype @UjAPI

TRADE_STATE_CANCELLED = ConvertTradeState(1) ---@type tradestate @UjAPI
TRADE_STATE_IGNORE_TRADING_LOCK = ConvertTradeState(2) ---@type tradestate @UjAPI
TRADE_STATE_IGNORE_TRADING_ALLIES_ONLY = ConvertTradeState(4) ---@type tradestate @UjAPI
TRADE_STATE_IGNORE_SOURCE_UPKEEP = ConvertTradeState(8) ---@type tradestate @UjAPI
TRADE_STATE_IGNORE_SOURCE_TAX = ConvertTradeState(16) ---@type tradestate @UjAPI
TRADE_STATE_IGNORE_SOURCE_LOSS = ConvertTradeState(32) ---@type tradestate @UjAPI
TRADE_STATE_IGNORE_TARGET_UPKEEP = ConvertTradeState(64) ---@type tradestate @UjAPI
TRADE_STATE_IGNORE_TARGET_TAX = ConvertTradeState(128) ---@type tradestate @UjAPI
TRADE_STATE_IGNORE_TARGET_LOSS = ConvertTradeState(256) ---@type tradestate @UjAPI
TRADE_STATE_IGNORE_NOTIFICATION = ConvertTradeState(512) ---@type tradestate @UjAPI

-- ============================================================================
-- MathAPI
---@param degrees real
---@return real
function Deg2Rad (degrees) end
---@param radians real
---@return real
function Rad2Deg (radians) end

---@param radians real
---@return real
function Sin (radians) end
---@param radians real
---@return real
function Cos (radians) end
---@param radians real
---@return real
function Tan (radians) end

-- Expect values between -1 and 1...returns 0 for invalid input
---@param y real
---@return real
function Asin (y) end
---@param x real
---@return real
function Acos (x) end
---@param x real
---@return real
function Atan (x) end

-- Returns 0 if x and y are both 0
---@param y real
---@param x real
---@return real
function Atan2 (y, x) end

-- Returns 0 if x <= 0
---@param x real
---@return real
function SquareRoot (x) end

-- computes x to the y power
-- y == 0.0 => 1
-- x ==0.0 and y < 0	=> 0
-- 
---@param x real
---@param power real
---@return real
function Pow (x, power) end

-- ============================================================================
-- String Utility API
---@param i integer
---@return real
function I2R (i) end
---@param r real
---@return integer
function R2I (r) end
---@param i integer
---@return string
function I2S (i) end
---@param r real
---@return string
function R2S (r) end
---@param r real
---@param width integer
---@param precision integer
---@return string
function R2SW (r, width, precision) end
---@param s string
---@return integer
function S2I (s) end
---@param s string
---@return real
function S2R (s) end
---@param whichHandle handle
---@return integer
function GetHandleId (whichHandle) end
---@param source string
---@param start integer
---@param ends integer
---@return string
function SubString (source, start, ends) end
---@param s string
---@return integer
function StringLength (s) end
---@param source string
---@param upper boolean
---@return string
function StringCase (source, upper) end
---@param s string
---@return integer
function StringHash (s) end

---@param source string
---@return string
function GetLocalizedString (source) end
---@param source string
---@return integer
function GetLocalizedHotkey (source) end

-- ============================================================================
-- Map Setup API
-- 
-- These are native functions for describing the map configuration
-- these funcs should only be used in the "config" function of
-- a map script. The functions should also be called in this order
-- ( i.e. call SetPlayers before SetPlayerColor...
-- 

---@param name string
function SetMapName (name) end
---@param description string
function SetMapDescription (description) end

---@param teamcount integer
function SetTeams (teamcount) end
---@param playercount integer
function SetPlayers (playercount) end

---@param whichStartLoc integer
---@param x real
---@param y real
function DefineStartLocation (whichStartLoc, x, y) end
---@param whichStartLoc integer
---@param whichLocation location
function DefineStartLocationLoc (whichStartLoc, whichLocation) end
---@param whichStartLoc integer
---@param prioSlotCount integer
function SetStartLocPrioCount (whichStartLoc, prioSlotCount) end
---@param whichStartLoc integer
---@param prioSlotIndex integer
---@param otherStartLocIndex integer
---@param priority startlocprio
function SetStartLocPrio (whichStartLoc, prioSlotIndex, otherStartLocIndex, priority) end
---@param whichStartLoc integer
---@param prioSlotIndex integer
---@return integer
function GetStartLocPrioSlot (whichStartLoc, prioSlotIndex) end
---@param whichStartLoc integer
---@param prioSlotIndex integer
---@return startlocprio
function GetStartLocPrio (whichStartLoc, prioSlotIndex) end

---@param whichGameType gametype
---@param value boolean
function SetGameTypeSupported (whichGameType, value) end
---@param whichMapFlag mapflag
---@param value boolean
function SetMapFlag (whichMapFlag, value) end
---@param whichPlacementType placement
function SetGamePlacement (whichPlacementType) end
---@param whichspeed gamespeed
function SetGameSpeed (whichspeed) end
---@param whichdifficulty gamedifficulty
function SetGameDifficulty (whichdifficulty) end
---@param whichdensity mapdensity
function SetResourceDensity (whichdensity) end
---@param whichdensity mapdensity
function SetCreatureDensity (whichdensity) end

---@return integer
function GetTeams () end
---@return integer
function GetPlayers () end

---@param whichGameType gametype
---@return boolean
function IsGameTypeSupported (whichGameType) end
---@return gametype
function GetGameTypeSelected () end
---@param whichMapFlag mapflag
---@return boolean
function IsMapFlagSet (whichMapFlag) end

---@return placement
function GetGamePlacement () end
---@return gamespeed
function GetGameSpeed () end
---@return gamedifficulty
function GetGameDifficulty () end
---@return mapdensity
function GetResourceDensity () end
---@return mapdensity
function GetCreatureDensity () end
---@param whichStartLocation integer
---@return real
function GetStartLocationX (whichStartLocation) end
---@param whichStartLocation integer
---@return real
function GetStartLocationY (whichStartLocation) end
---@param whichStartLocation integer
---@return location
function GetStartLocationLoc (whichStartLocation) end


---@param whichPlayer player
---@param whichTeam integer
function SetPlayerTeam (whichPlayer, whichTeam) end
---@param whichPlayer player
---@param startLocIndex integer
function SetPlayerStartLocation (whichPlayer, startLocIndex) end
-- forces player to have the specified start loc and marks the start loc as occupied
-- which removes it from consideration for subsequently placed players
-- ( i.e. you can use this to put people in a fixed loc and then
-- use random placement for any unplaced players etc )
---@param whichPlayer player
---@param startLocIndex integer
function ForcePlayerStartLocation (whichPlayer, startLocIndex) end
---@param whichPlayer player
---@param color playercolor
function SetPlayerColor (whichPlayer, color) end
---@param sourcePlayer player
---@param otherPlayer player
---@param whichAllianceSetting alliancetype
---@param value boolean
function SetPlayerAlliance (sourcePlayer, otherPlayer, whichAllianceSetting, value) end
---@param sourcePlayer player
---@param otherPlayer player
---@param whichResource playerstate
---@param rate integer
function SetPlayerTaxRate (sourcePlayer, otherPlayer, whichResource, rate) end
---@param whichPlayer player
---@param whichRacePreference racepreference
function SetPlayerRacePreference (whichPlayer, whichRacePreference) end
---@param whichPlayer player
---@param value boolean
function SetPlayerRaceSelectable (whichPlayer, value) end
---@param whichPlayer player
---@param controlType mapcontrol
function SetPlayerController (whichPlayer, controlType) end
---@param whichPlayer player
---@param name string
function SetPlayerName (whichPlayer, name) end

---@param whichPlayer player
---@param flag boolean
function SetPlayerOnScoreScreen (whichPlayer, flag) end

---@param whichPlayer player
---@return integer
function GetPlayerTeam (whichPlayer) end
---@param whichPlayer player
---@return integer
function GetPlayerStartLocation (whichPlayer) end
---@param whichPlayer player
---@return playercolor
function GetPlayerColor (whichPlayer) end
---@param whichPlayer player
---@return boolean
function GetPlayerSelectable (whichPlayer) end
---@param whichPlayer player
---@return mapcontrol
function GetPlayerController (whichPlayer) end
---@param whichPlayer player
---@return playerslotstate
function GetPlayerSlotState (whichPlayer) end
---@param sourcePlayer player
---@param otherPlayer player
---@param whichResource playerstate
---@return integer
function GetPlayerTaxRate (sourcePlayer, otherPlayer, whichResource) end
---@param whichPlayer player
---@param pref racepreference
---@return boolean
function IsPlayerRacePrefSet (whichPlayer, pref) end
---@param whichPlayer player
---@return string
function GetPlayerName (whichPlayer) end

-- ============================================================================
-- Timer API
-- 
---@return timer
function CreateTimer () end
---@param whichTimer timer
function DestroyTimer (whichTimer) end
---@param whichTimer timer
---@param timeout real
---@param periodic boolean
---@param handlerFunc code
function TimerStart (whichTimer, timeout, periodic, handlerFunc) end
---@param whichTimer timer
---@return real
function TimerGetElapsed (whichTimer) end
---@param whichTimer timer
---@return real
function TimerGetRemaining (whichTimer) end
---@param whichTimer timer
---@return real
function TimerGetTimeout (whichTimer) end
---@param whichTimer timer
function PauseTimer (whichTimer) end
---@param whichTimer timer
function ResumeTimer (whichTimer) end
---@return timer
function GetExpiredTimer () end

-- ============================================================================
-- Group API
-- 
---@return group
function CreateGroup () end
---@param whichGroup group
function DestroyGroup (whichGroup) end
---@param whichGroup group
---@param whichUnit unit
function GroupAddUnit (whichGroup, whichUnit) end
---@param whichGroup group
---@param whichUnit unit
function GroupRemoveUnit (whichGroup, whichUnit) end
---@param whichGroup group
function GroupClear (whichGroup) end
---@param whichGroup group
---@param unitname string
---@param filter boolexpr
function GroupEnumUnitsOfType (whichGroup, unitname, filter) end
---@param whichGroup group
---@param whichPlayer player
---@param filter boolexpr
function GroupEnumUnitsOfPlayer (whichGroup, whichPlayer, filter) end
---@param whichGroup group
---@param unitname string
---@param filter boolexpr
---@param countLimit integer
function GroupEnumUnitsOfTypeCounted (whichGroup, unitname, filter, countLimit) end
---@param whichGroup group
---@param whichRect rect
---@param filter boolexpr
function GroupEnumUnitsInRect (whichGroup, whichRect, filter) end
---@param whichGroup group
---@param whichRect rect
---@param filter boolexpr
---@param countLimit integer
function GroupEnumUnitsInRectCounted (whichGroup, whichRect, filter, countLimit) end
---@param whichGroup group
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
function GroupEnumUnitsInRange (whichGroup, x, y, radius, filter) end
---@param whichGroup group
---@param whichLocation location
---@param radius real
---@param filter boolexpr
function GroupEnumUnitsInRangeOfLoc (whichGroup, whichLocation, radius, filter) end
---@param whichGroup group
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
---@param countLimit integer
function GroupEnumUnitsInRangeCounted (whichGroup, x, y, radius, filter, countLimit) end
---@param whichGroup group
---@param whichLocation location
---@param radius real
---@param filter boolexpr
---@param countLimit integer
function GroupEnumUnitsInRangeOfLocCounted (whichGroup, whichLocation, radius, filter, countLimit) end
---@param whichGroup group
---@param whichPlayer player
---@param filter boolexpr
function GroupEnumUnitsSelected (whichGroup, whichPlayer, filter) end

---@param whichGroup group
---@param orderName string
---@return boolean
function GroupImmediateOrder (whichGroup, orderName) end
---@param whichGroup group
---@param orderId integer
---@return boolean
function GroupImmediateOrderById (whichGroup, orderId) end
---@param whichGroup group
---@param orderName string
---@param x real
---@param y real
---@return boolean
function GroupPointOrder (whichGroup, orderName, x, y) end
---@param whichGroup group
---@param orderName string
---@param whichLocation location
---@return boolean
function GroupPointOrderLoc (whichGroup, orderName, whichLocation) end
---@param whichGroup group
---@param orderId integer
---@param x real
---@param y real
---@return boolean
function GroupPointOrderById (whichGroup, orderId, x, y) end
---@param whichGroup group
---@param orderId integer
---@param whichLocation location
---@return boolean
function GroupPointOrderByIdLoc (whichGroup, orderId, whichLocation) end
---@param whichGroup group
---@param orderName string
---@param targetWidget widget
---@return boolean
function GroupTargetOrder (whichGroup, orderName, targetWidget) end
---@param whichGroup group
---@param orderId integer
---@param targetWidget widget
---@return boolean
function GroupTargetOrderById (whichGroup, orderId, targetWidget) end

-- This will be difficult to support with potentially disjoint, cell-based regions
-- as it would involve enumerating all the cells that are covered by a particularregion
-- a better implementation would be a trigger that adds relevant units as they enter
-- and removes them if they leave...
---@param whichGroup group
---@param callback code
function ForGroup (whichGroup, callback) end
---@param whichGroup group
---@return unit
function FirstOfGroup (whichGroup) end

-- ============================================================================
-- Force API
-- 
---@return force
function CreateForce () end
---@param whichForce force
function DestroyForce (whichForce) end
---@param whichForce force
---@param whichPlayer player
function ForceAddPlayer (whichForce, whichPlayer) end
---@param whichForce force
---@param whichPlayer player
function ForceRemovePlayer (whichForce, whichPlayer) end
---@param whichForce force
function ForceClear (whichForce) end
---@param whichForce force
---@param filter boolexpr
function ForceEnumPlayers (whichForce, filter) end
---@param whichForce force
---@param filter boolexpr
---@param countLimit integer
function ForceEnumPlayersCounted (whichForce, filter, countLimit) end
---@param whichForce force
---@param whichPlayer player
---@param filter boolexpr
function ForceEnumAllies (whichForce, whichPlayer, filter) end
---@param whichForce force
---@param whichPlayer player
---@param filter boolexpr
function ForceEnumEnemies (whichForce, whichPlayer, filter) end
---@param whichForce force
---@param callback code
function ForForce (whichForce, callback) end

-- ============================================================================
-- Region and Location API
-- 
---@param minx real
---@param miny real
---@param maxx real
---@param maxy real
---@return rect
function Rect (minx, miny, maxx, maxy) end
---@param min location
---@param max location
---@return rect
function RectFromLoc (min, max) end
---@param whichRect rect
function RemoveRect (whichRect) end
---@param whichRect rect
---@param minx real
---@param miny real
---@param maxx real
---@param maxy real
function SetRect (whichRect, minx, miny, maxx, maxy) end
---@param whichRect rect
---@param min location
---@param max location
function SetRectFromLoc (whichRect, min, max) end
---@param whichRect rect
---@param newCenterX real
---@param newCenterY real
function MoveRectTo (whichRect, newCenterX, newCenterY) end
---@param whichRect rect
---@param newCenterLoc location
function MoveRectToLoc (whichRect, newCenterLoc) end

---@param whichRect rect
---@return real
function GetRectCenterX (whichRect) end
---@param whichRect rect
---@return real
function GetRectCenterY (whichRect) end
---@param whichRect rect
---@return real
function GetRectMinX (whichRect) end
---@param whichRect rect
---@return real
function GetRectMinY (whichRect) end
---@param whichRect rect
---@return real
function GetRectMaxX (whichRect) end
---@param whichRect rect
---@return real
function GetRectMaxY (whichRect) end

---@return region
function CreateRegion () end
---@param whichRegion region
function RemoveRegion (whichRegion) end

---@param whichRegion region
---@param whichRect rect
function RegionAddRect (whichRegion, whichRect) end
---@param whichRegion region
---@param whichRect rect
function RegionClearRect (whichRegion, whichRect) end

---@param whichRegion region
---@param x real
---@param y real
function RegionAddCell (whichRegion, x, y) end
---@param whichRegion region
---@param whichLocation location
function RegionAddCellAtLoc (whichRegion, whichLocation) end
---@param whichRegion region
---@param x real
---@param y real
function RegionClearCell (whichRegion, x, y) end
---@param whichRegion region
---@param whichLocation location
function RegionClearCellAtLoc (whichRegion, whichLocation) end

---@param x real
---@param y real
---@return location
function Location (x, y) end
---@param whichLocation location
function RemoveLocation (whichLocation) end
---@param whichLocation location
---@param newX real
---@param newY real
function MoveLocation (whichLocation, newX, newY) end
---@param whichLocation location
---@return real
function GetLocationX (whichLocation) end
---@param whichLocation location
---@return real
function GetLocationY (whichLocation) end

-- This function is asynchronous. The values it returns are not guaranteed synchronous between each player.
-- If you attempt to use it in a synchronous manner, it may cause a desync.
---@param whichLocation location
---@return real
function GetLocationZ (whichLocation) end

---@param whichRegion region
---@param whichUnit unit
---@return boolean
function IsUnitInRegion (whichRegion, whichUnit) end
---@param whichRegion region
---@param x real
---@param y real
---@return boolean
function IsPointInRegion (whichRegion, x, y) end
---@param whichRegion region
---@param whichLocation location
---@return boolean
function IsLocationInRegion (whichRegion, whichLocation) end

-- Returns full map bounds, including unplayable borders, in world coordinates
---@return rect
function GetWorldBounds () end

-- ============================================================================
-- Native trigger interface
-- 
---@return trigger
function CreateTrigger () end
---@param whichTrigger trigger
function DestroyTrigger (whichTrigger) end
---@param whichTrigger trigger
function ResetTrigger (whichTrigger) end
---@param whichTrigger trigger
function EnableTrigger (whichTrigger) end
---@param whichTrigger trigger
function DisableTrigger (whichTrigger) end
---@param whichTrigger trigger
---@return boolean
function IsTriggerEnabled (whichTrigger) end

---@param whichTrigger trigger
---@param flag boolean
function TriggerWaitOnSleeps (whichTrigger, flag) end
---@param whichTrigger trigger
---@return boolean
function IsTriggerWaitOnSleeps (whichTrigger) end

---@return unit
function GetFilterUnit () end
---@return unit
function GetEnumUnit () end

---@return destructable
function GetFilterDestructable () end
---@return destructable
function GetEnumDestructable () end

---@return item
function GetFilterItem () end
---@return item
function GetEnumItem () end

---@return player
function GetFilterPlayer () end
---@return player
function GetEnumPlayer () end

---@return trigger
function GetTriggeringTrigger () end
---@return eventid
function GetTriggerEventId () end
---@param whichTrigger trigger
---@return integer
function GetTriggerEvalCount (whichTrigger) end
---@param whichTrigger trigger
---@return integer
function GetTriggerExecCount (whichTrigger) end

---@param funcName string
function ExecuteFunc (funcName) end

-- ============================================================================
-- Boolean Expr API ( for compositing trigger conditions and unit filter funcs...)
-- ============================================================================
---@param operandA boolexpr
---@param operandB boolexpr
---@return boolexpr
function And (operandA, operandB) end
---@param operandA boolexpr
---@param operandB boolexpr
---@return boolexpr
function Or (operandA, operandB) end
---@param operand boolexpr
---@return boolexpr
function Not (operand) end
---@param func code
---@return conditionfunc
function Condition (func) end
---@param c conditionfunc
function DestroyCondition (c) end
---@param func code
---@return filterfunc
function Filter (func) end
---@param f filterfunc
function DestroyFilter (f) end
---@param e boolexpr
function DestroyBoolExpr (e) end

-- ============================================================================
-- Trigger Game Event API
-- ============================================================================

---@param whichTrigger trigger
---@param varName string
---@param opcode limitop
---@param limitval real
---@return event
function TriggerRegisterVariableEvent (whichTrigger, varName, opcode, limitval) end

-- EVENT_GAME_VARIABLE_LIMIT
-- constant native string GetTriggeringVariableName takes nothing returns string

-- Creates it's own timer and triggers when it expires
---@param whichTrigger trigger
---@param timeout real
---@param periodic boolean
---@return event
function TriggerRegisterTimerEvent (whichTrigger, timeout, periodic) end

-- Triggers when the timer you tell it about expires
---@param whichTrigger trigger
---@param whichTimer timer
---@return event
function TriggerRegisterTimerExpireEvent (whichTrigger, whichTimer) end

---@param whichTrigger trigger
---@param whichState gamestate
---@param opcode limitop
---@param limitval real
---@return event
function TriggerRegisterGameStateEvent (whichTrigger, whichState, opcode, limitval) end

---@param whichTrigger trigger
---@param whichDialog dialog
---@return event
function TriggerRegisterDialogEvent (whichTrigger, whichDialog) end
---@param whichTrigger trigger
---@param whichButton button
---@return event
function TriggerRegisterDialogButtonEvent (whichTrigger, whichButton) end

-- EVENT_GAME_STATE_LIMIT
---@return gamestate
function GetEventGameState () end

---@param whichTrigger trigger
---@param whichGameEvent gameevent
---@return event
function TriggerRegisterGameEvent (whichTrigger, whichGameEvent) end

-- EVENT_GAME_VICTORY
---@return player
function GetWinningPlayer () end


---@param whichTrigger trigger
---@param whichRegion region
---@param filter boolexpr
---@return event
function TriggerRegisterEnterRegion (whichTrigger, whichRegion, filter) end

-- EVENT_GAME_ENTER_REGION
---@return region
function GetTriggeringRegion () end
---@return unit
function GetEnteringUnit () end

-- EVENT_GAME_LEAVE_REGION

---@param whichTrigger trigger
---@param whichRegion region
---@param filter boolexpr
---@return event
function TriggerRegisterLeaveRegion (whichTrigger, whichRegion, filter) end
---@return unit
function GetLeavingUnit () end

---@param whichTrigger trigger
---@param whichTrackable trackable
---@return event
function TriggerRegisterTrackableHitEvent (whichTrigger, whichTrackable) end
---@param whichTrigger trigger
---@param whichTrackable trackable
---@return event
function TriggerRegisterTrackableTrackEvent (whichTrigger, whichTrackable) end

-- EVENT_GAME_TRACKABLE_HIT
-- EVENT_GAME_TRACKABLE_TRACK
---@return trackable
function GetTriggeringTrackable () end

-- EVENT_DIALOG_BUTTON_CLICK
---@return button
function GetClickedButton () end
---@return dialog
function GetClickedDialog () end

-- EVENT_GAME_TOURNAMENT_FINISH_SOON
---@return real
function GetTournamentFinishSoonTimeRemaining () end
---@return integer
function GetTournamentFinishNowRule () end
---@return player
function GetTournamentFinishNowPlayer () end
---@param whichPlayer player
---@return integer
function GetTournamentScore (whichPlayer) end

-- EVENT_GAME_SAVE
---@return string
function GetSaveBasicFilename () end

-- ============================================================================
-- Trigger Player Based Event API
-- ============================================================================

---@param whichTrigger trigger
---@param whichPlayer player
---@param whichPlayerEvent playerevent
---@return event
function TriggerRegisterPlayerEvent (whichTrigger, whichPlayer, whichPlayerEvent) end

-- EVENT_PLAYER_DEFEAT
-- EVENT_PLAYER_VICTORY
---@return player
function GetTriggerPlayer () end

---@param whichTrigger trigger
---@param whichPlayer player
---@param whichPlayerUnitEvent playerunitevent
---@param filter boolexpr
---@return event
function TriggerRegisterPlayerUnitEvent (whichTrigger, whichPlayer, whichPlayerUnitEvent, filter) end

-- EVENT_PLAYER_HERO_LEVEL
-- EVENT_UNIT_HERO_LEVEL
---@return unit
function GetLevelingUnit () end

-- EVENT_PLAYER_HERO_SKILL
-- EVENT_UNIT_HERO_SKILL
---@return unit
function GetLearningUnit () end
---@return integer
function GetLearnedSkill () end
---@return integer
function GetLearnedSkillLevel () end

-- EVENT_PLAYER_HERO_REVIVABLE
---@return unit
function GetRevivableUnit () end

-- EVENT_PLAYER_HERO_REVIVE_START
-- EVENT_PLAYER_HERO_REVIVE_CANCEL
-- EVENT_PLAYER_HERO_REVIVE_FINISH
-- EVENT_UNIT_HERO_REVIVE_START
-- EVENT_UNIT_HERO_REVIVE_CANCEL
-- EVENT_UNIT_HERO_REVIVE_FINISH
-- EVENT_PLAYER_UNIT_REINCARNATION_START
-- EVENT_UNIT_REINCARNATION_START
-- EVENT_PLAYER_UNIT_REINCARNATION_END
-- EVENT_UNIT_REINCARNATION_END
-- EVENT_PLAYER_UNIT_REVIVED
-- EVENT_UNIT_REVIVED
---@return unit
function GetRevivingUnit () end

-- EVENT_PLAYER_UNIT_ATTACKED
---@return unit
function GetAttacker () end

-- EVENT_PLAYER_UNIT_RESCUED
---@return unit
function GetRescuer () end

-- EVENT_PLAYER_UNIT_DEATH
---@return unit
function GetDyingUnit () end
---@return unit
function GetKillingUnit () end

-- EVENT_PLAYER_UNIT_DECAY
---@return unit
function GetDecayingUnit () end

-- EVENT_PLAYER_UNIT_SELECTED
-- constant native GetSelectedUnit takes nothing returns unit

-- EVENT_PLAYER_UNIT_CONSTRUCT_START
---@return unit
function GetConstructingStructure () end

-- EVENT_PLAYER_UNIT_CONSTRUCT_FINISH
-- EVENT_PLAYER_UNIT_CONSTRUCT_CANCEL
---@return unit
function GetCancelledStructure () end
---@return unit
function GetConstructedStructure () end

-- EVENT_PLAYER_UNIT_RESEARCH_START
-- EVENT_PLAYER_UNIT_RESEARCH_CANCEL
-- EVENT_PLAYER_UNIT_RESEARCH_FINISH
---@return unit
function GetResearchingUnit () end
---@return integer
function GetResearched () end

-- EVENT_PLAYER_UNIT_TRAIN_START
-- EVENT_PLAYER_UNIT_TRAIN_CANCEL
---@return integer
function GetTrainedUnitType () end

-- EVENT_PLAYER_UNIT_TRAIN_FINISH
---@return unit
function GetTrainedUnit () end

-- EVENT_PLAYER_UNIT_DETECTED
---@return unit
function GetDetectedUnit () end

-- EVENT_PLAYER_UNIT_SUMMONED
---@return unit
function GetSummoningUnit () end
---@return unit
function GetSummonedUnit () end

-- EVENT_PLAYER_UNIT_LOADED
---@return unit
function GetTransportUnit () end
---@return unit
function GetLoadedUnit () end

-- EVENT_PLAYER_UNIT_SELL
---@return unit
function GetSellingUnit () end
---@return unit
function GetSoldUnit () end
---@return unit
function GetBuyingUnit () end

-- EVENT_PLAYER_UNIT_SELL_ITEM
---@return item
function GetSoldItem () end

-- EVENT_PLAYER_UNIT_CHANGE_OWNER
---@return unit
function GetChangingUnit () end
---@return player
function GetChangingUnitPrevOwner () end

-- EVENT_PLAYER_UNIT_DROP_ITEM
-- EVENT_PLAYER_UNIT_PICKUP_ITEM
-- EVENT_PLAYER_UNIT_USE_ITEM
---@return unit
function GetManipulatingUnit () end
---@return item
function GetManipulatedItem () end

-- EVENT_PLAYER_UNIT_ISSUED_ORDER
---@return unit
function GetOrderedUnit () end
---@return integer
function GetIssuedOrderId () end

-- EVENT_PLAYER_UNIT_ISSUED_POINT_ORDER
---@return real
function GetOrderPointX () end
---@return real
function GetOrderPointY () end
---@return location
function GetOrderPointLoc () end

-- EVENT_PLAYER_UNIT_ISSUED_TARGET_ORDER
---@return widget
function GetOrderTarget () end
---@return destructable
function GetOrderTargetDestructable () end
---@return item
function GetOrderTargetItem () end
---@return unit
function GetOrderTargetUnit () end

-- EVENT_UNIT_SPELL_CHANNEL
-- EVENT_UNIT_SPELL_CAST
-- EVENT_UNIT_SPELL_EFFECT
-- EVENT_UNIT_SPELL_FINISH
-- EVENT_UNIT_SPELL_ENDCAST
-- EVENT_PLAYER_UNIT_SPELL_CHANNEL
-- EVENT_PLAYER_UNIT_SPELL_CAST
-- EVENT_PLAYER_UNIT_SPELL_EFFECT
-- EVENT_PLAYER_UNIT_SPELL_FINISH
-- EVENT_PLAYER_UNIT_SPELL_ENDCAST
---@return unit
function GetSpellAbilityUnit () end
---@return integer
function GetSpellAbilityId () end
---@return ability
function GetSpellAbility () end
---@return location
function GetSpellTargetLoc () end
---@return real
function GetSpellTargetX () end
---@return real
function GetSpellTargetY () end
---@return destructable
function GetSpellTargetDestructable () end
---@return item
function GetSpellTargetItem () end
---@return unit
function GetSpellTargetUnit () end

---@param whichTrigger trigger
---@param whichPlayer player
---@param whichAlliance alliancetype
---@return event
function TriggerRegisterPlayerAllianceChange (whichTrigger, whichPlayer, whichAlliance) end
---@param whichTrigger trigger
---@param whichPlayer player
---@param whichState playerstate
---@param opcode limitop
---@param limitval real
---@return event
function TriggerRegisterPlayerStateEvent (whichTrigger, whichPlayer, whichState, opcode, limitval) end

-- EVENT_PLAYER_STATE_LIMIT
---@return playerstate
function GetEventPlayerState () end

---@param whichTrigger trigger
---@param whichPlayer player
---@param chatMessageToDetect string
---@param exactMatchOnly boolean
---@return event
function TriggerRegisterPlayerChatEvent (whichTrigger, whichPlayer, chatMessageToDetect, exactMatchOnly) end

-- EVENT_PLAYER_CHAT

-- returns the actual string they typed in ( same as what you registered for if you required exact match )
---@return string
function GetEventPlayerChatString () end

-- returns the string that you registered for
---@return string
function GetEventPlayerChatStringMatched () end

---@param whichTrigger trigger
---@param whichWidget widget
---@return event
function TriggerRegisterDeathEvent (whichTrigger, whichWidget) end

-- ============================================================================
-- Trigger Unit Based Event API
-- ============================================================================

-- returns handle to unit which triggered the most recent event when called from
-- within a trigger action function...returns null handle when used incorrectly

---@return unit
function GetTriggerUnit () end

---@param whichTrigger trigger
---@param whichUnit unit
---@param whichState unitstate
---@param opcode limitop
---@param limitval real
---@return event
function TriggerRegisterUnitStateEvent (whichTrigger, whichUnit, whichState, opcode, limitval) end

-- EVENT_UNIT_STATE_LIMIT
---@return unitstate
function GetEventUnitState () end

---@param whichTrigger trigger
---@param whichUnit unit
---@param whichEvent unitevent
---@return event
function TriggerRegisterUnitEvent (whichTrigger, whichUnit, whichEvent) end

-- EVENT_UNIT_DAMAGED
---@return real
function GetEventDamage () end
---@return unit
function GetEventDamageSource () end

-- EVENT_UNIT_DEATH
-- EVENT_UNIT_DECAY
-- Use the GetDyingUnit and GetDecayingUnit funcs above

-- EVENT_UNIT_DETECTED
---@return player
function GetEventDetectingPlayer () end

---@param whichTrigger trigger
---@param whichUnit unit
---@param whichEvent unitevent
---@param filter boolexpr
---@return event
function TriggerRegisterFilterUnitEvent (whichTrigger, whichUnit, whichEvent, filter) end

-- EVENT_UNIT_ACQUIRED_TARGET
-- EVENT_UNIT_TARGET_IN_RANGE
---@return unit
function GetEventTargetUnit () end

-- EVENT_UNIT_ATTACKED
-- Use GetAttacker from the Player Unit Event API Below...

-- EVENT_UNIT_RESCUEDED
-- Use GetRescuer from the Player Unit Event API Below...

-- EVENT_UNIT_CONSTRUCT_CANCEL
-- EVENT_UNIT_CONSTRUCT_FINISH

-- See the Player Unit Construction Event API above for event info funcs

-- EVENT_UNIT_TRAIN_START
-- EVENT_UNIT_TRAIN_CANCELLED
-- EVENT_UNIT_TRAIN_FINISH

-- See the Player Unit Training Event API above for event info funcs

-- EVENT_UNIT_SELL

-- See the Player Unit Sell Event API above for event info funcs

-- EVENT_UNIT_DROP_ITEM
-- EVENT_UNIT_PICKUP_ITEM
-- EVENT_UNIT_USE_ITEM
-- See the Player Unit/Item manipulation Event API above for event info funcs

-- EVENT_UNIT_ISSUED_ORDER
-- EVENT_UNIT_ISSUED_POINT_ORDER
-- EVENT_UNIT_ISSUED_TARGET_ORDER

-- See the Player Unit Order Event API above for event info funcs

---@param whichTrigger trigger
---@param whichUnit unit
---@param range real
---@param filter boolexpr
---@return event
function TriggerRegisterUnitInRange (whichTrigger, whichUnit, range, filter) end

---@param whichTrigger trigger
---@param condition boolexpr
---@return triggercondition
function TriggerAddCondition (whichTrigger, condition) end
---@param whichTrigger trigger
---@param whichCondition triggercondition
function TriggerRemoveCondition (whichTrigger, whichCondition) end
---@param whichTrigger trigger
function TriggerClearConditions (whichTrigger) end

---@param whichTrigger trigger
---@param actionFunc code
---@return triggeraction
function TriggerAddAction (whichTrigger, actionFunc) end
---@param whichTrigger trigger
---@param whichAction triggeraction
function TriggerRemoveAction (whichTrigger, whichAction) end
---@param whichTrigger trigger
function TriggerClearActions (whichTrigger) end
---@param timeout real
function TriggerSleepAction (timeout) end
---@param whichSound sound
---@param offset real
function TriggerWaitForSound (whichSound, offset) end
---@param whichTrigger trigger
---@return boolean
function TriggerEvaluate (whichTrigger) end
---@param whichTrigger trigger
function TriggerExecute (whichTrigger) end
---@param whichTrigger trigger
function TriggerExecuteWait (whichTrigger) end
function TriggerSyncStart () end
function TriggerSyncReady () end

-- ============================================================================
-- Widget API
---@param whichWidget widget
---@return real
function GetWidgetLife (whichWidget) end
---@param whichWidget widget
---@param newLife real
function SetWidgetLife (whichWidget, newLife) end
---@param whichWidget widget
---@return real
function GetWidgetX (whichWidget) end
---@param whichWidget widget
---@return real
function GetWidgetY (whichWidget) end
---@return widget
function GetTriggerWidget () end

-- ============================================================================
-- Destructable Object API
-- Facing arguments are specified in degrees
---@param objectTypeId integer
---@param x real
---@param y real
---@param facing real
---@param scale real
---@param variation integer
---@return destructable
function CreateDestructable (objectTypeId, x, y, facing, scale, variation) end
---@param objectTypeId integer
---@param x real
---@param y real
---@param z real
---@param facing real
---@param scale real
---@param variation integer
---@return destructable
function CreateDestructableZ (objectTypeId, x, y, z, facing, scale, variation) end
---@param objectTypeId integer
---@param x real
---@param y real
---@param facing real
---@param scale real
---@param variation integer
---@return destructable
function CreateDeadDestructable (objectTypeId, x, y, facing, scale, variation) end
---@param objectTypeId integer
---@param x real
---@param y real
---@param z real
---@param facing real
---@param scale real
---@param variation integer
---@return destructable
function CreateDeadDestructableZ (objectTypeId, x, y, z, facing, scale, variation) end
---@param whichDestructable destructable
function RemoveDestructable (whichDestructable) end
---@param whichDestructable destructable
function KillDestructable (whichDestructable) end
---@param whichDestructable destructable
---@param flag boolean
function SetDestructableInvulnerable (whichDestructable, flag) end
---@param whichDestructable destructable
---@return boolean
function IsDestructableInvulnerable (whichDestructable) end
---@param whichRect rect
---@param filter boolexpr
---@param actionFunc code
function EnumDestructablesInRect (whichRect, filter, actionFunc) end
---@param whichDestructable destructable
---@return integer
function GetDestructableTypeId (whichDestructable) end
---@param whichDestructable destructable
---@return real
function GetDestructableX (whichDestructable) end
---@param whichDestructable destructable
---@return real
function GetDestructableY (whichDestructable) end
---@param whichDestructable destructable
---@param life real
function SetDestructableLife (whichDestructable, life) end
---@param whichDestructable destructable
---@return real
function GetDestructableLife (whichDestructable) end
---@param whichDestructable destructable
---@param max real
function SetDestructableMaxLife (whichDestructable, max) end
---@param whichDestructable destructable
---@return real
function GetDestructableMaxLife (whichDestructable) end
---@param whichDestructable destructable
---@param life real
---@param birth boolean
function DestructableRestoreLife (whichDestructable, life, birth) end
---@param whichDestructable destructable
---@param whichAnimation string
function QueueDestructableAnimation (whichDestructable, whichAnimation) end
---@param whichDestructable destructable
---@param whichAnimation string
function SetDestructableAnimation (whichDestructable, whichAnimation) end
---@param whichDestructable destructable
---@param speedFactor real
function SetDestructableAnimationSpeed (whichDestructable, speedFactor) end
---@param whichDestructable destructable
---@param flag boolean
function ShowDestructable (whichDestructable, flag) end
---@param whichDestructable destructable
---@return real
function GetDestructableOccluderHeight (whichDestructable) end
---@param whichDestructable destructable
---@param height real
function SetDestructableOccluderHeight (whichDestructable, height) end
---@param whichDestructable destructable
---@return string
function GetDestructableName (whichDestructable) end
---@return destructable
function GetTriggerDestructable () end

-- ============================================================================
-- Item API
---@param itemTypeId integer
---@param x real
---@param y real
---@return item
function CreateItem (itemTypeId, x, y) end
---@param whichItem item
function RemoveItem (whichItem) end
---@param whichItem item
---@return player
function GetItemPlayer (whichItem) end
---@param whichItem item
---@return integer
function GetItemTypeId (whichItem) end
---@param whichItem item
---@return real
function GetItemX (whichItem) end
---@param whichItem item
---@return real
function GetItemY (whichItem) end
---@param whichItem item
---@param x real
---@param y real
function SetItemPosition (whichItem, x, y) end
---@param whichItem item
---@param flag boolean
function SetItemDropOnDeath (whichItem, flag) end
---@param whichItem item
---@param flag boolean
function SetItemDroppable (whichItem, flag) end
---@param whichItem item
---@param flag boolean
function SetItemPawnable (whichItem, flag) end
---@param whichItem item
---@param whichPlayer player
---@param changeColor boolean
function SetItemPlayer (whichItem, whichPlayer, changeColor) end
---@param whichItem item
---@param flag boolean
function SetItemInvulnerable (whichItem, flag) end
---@param whichItem item
---@return boolean
function IsItemInvulnerable (whichItem) end
---@param whichItem item
---@param show boolean
function SetItemVisible (whichItem, show) end
---@param whichItem item
---@return boolean
function IsItemVisible (whichItem) end
---@param whichItem item
---@return boolean
function IsItemOwned (whichItem) end
---@param whichItem item
---@return boolean
function IsItemPowerup (whichItem) end
---@param whichItem item
---@return boolean
function IsItemSellable (whichItem) end
---@param whichItem item
---@return boolean
function IsItemPawnable (whichItem) end
---@param itemTypeId integer
---@return boolean
function IsItemIdPowerup (itemTypeId) end
---@param itemTypeId integer
---@return boolean
function IsItemIdSellable (itemTypeId) end
---@param itemTypeId integer
---@return boolean
function IsItemIdPawnable (itemTypeId) end
---@param whichRect rect
---@param filter boolexpr
---@param actionFunc code
function EnumItemsInRect (whichRect, filter, actionFunc) end
---@param whichItem item
---@return integer
function GetItemLevel (whichItem) end
---@param whichItem item
---@return itemtype
function GetItemType (whichItem) end
---@param whichItem item
---@param unitTypeId integer
function SetItemDropID (whichItem, unitTypeId) end
---@param whichItem item
---@return string
function GetItemName (whichItem) end
---@param whichItem item
---@return integer
function GetItemCharges (whichItem) end
---@param whichItem item
---@param charges integer
function SetItemCharges (whichItem, charges) end
---@param whichItem item
---@return integer
function GetItemUserData (whichItem) end
---@param whichItem item
---@param data integer
function SetItemUserData (whichItem, data) end

-- ============================================================================
-- Unit API
-- Facing arguments are specified in degrees
---@param whichPlayer player
---@param unitTypeId integer
---@param x real
---@param y real
---@param facing real
---@return unit
function CreateUnit (whichPlayer, unitTypeId, x, y, facing) end
---@param whichPlayer player
---@param unitname string
---@param x real
---@param y real
---@param facing real
---@return unit
function CreateUnitByName (whichPlayer, unitname, x, y, facing) end
---@param whichPlayer player
---@param unitTypeId integer
---@param whichLocation location
---@param facing real
---@return unit
function CreateUnitAtLoc (whichPlayer, unitTypeId, whichLocation, facing) end
---@param whichPlayer player
---@param unitname string
---@param whichLocation location
---@param facing real
---@return unit
function CreateUnitAtLocByName (whichPlayer, unitname, whichLocation, facing) end
---@param whichPlayer player
---@param unitTypeId integer
---@param x real
---@param y real
---@param facing real
---@return unit
function CreateCorpse (whichPlayer, unitTypeId, x, y, facing) end

---@param whichUnit unit
function KillUnit (whichUnit) end
---@param whichUnit unit
function RemoveUnit (whichUnit) end
---@param whichUnit unit
---@param show boolean
function ShowUnit (whichUnit, show) end

---@param whichUnit unit
---@param whichUnitState unitstate
---@param newVal real
function SetUnitState (whichUnit, whichUnitState, newVal) end
---@param whichUnit unit
---@param newX real
function SetUnitX (whichUnit, newX) end
---@param whichUnit unit
---@param newY real
function SetUnitY (whichUnit, newY) end
---@param whichUnit unit
---@param newX real
---@param newY real
function SetUnitPosition (whichUnit, newX, newY) end
---@param whichUnit unit
---@param whichLocation location
function SetUnitPositionLoc (whichUnit, whichLocation) end
---@param whichUnit unit
---@param facingAngle real
function SetUnitFacing (whichUnit, facingAngle) end
---@param whichUnit unit
---@param facingAngle real
---@param duration real
function SetUnitFacingTimed (whichUnit, facingAngle, duration) end
---@param whichUnit unit
---@param newSpeed real
function SetUnitMoveSpeed (whichUnit, newSpeed) end
---@param whichUnit unit
---@param newHeight real
---@param rate real
function SetUnitFlyHeight (whichUnit, newHeight, rate) end
---@param whichUnit unit
---@param newTurnSpeed real
function SetUnitTurnSpeed (whichUnit, newTurnSpeed) end
---@param whichUnit unit
---@param newPropWindowAngle real
function SetUnitPropWindow (whichUnit, newPropWindowAngle) end
---@param whichUnit unit
---@param newAcquireRange real
function SetUnitAcquireRange (whichUnit, newAcquireRange) end
---@param whichUnit unit
---@param creepGuard boolean
function SetUnitCreepGuard (whichUnit, creepGuard) end

---@param whichUnit unit
---@return real
function GetUnitAcquireRange (whichUnit) end
---@param whichUnit unit
---@return real
function GetUnitTurnSpeed (whichUnit) end
---@param whichUnit unit
---@return real
function GetUnitPropWindow (whichUnit) end
---@param whichUnit unit
---@return real
function GetUnitFlyHeight (whichUnit) end

---@param whichUnit unit
---@return real
function GetUnitDefaultAcquireRange (whichUnit) end
---@param whichUnit unit
---@return real
function GetUnitDefaultTurnSpeed (whichUnit) end
---@param whichUnit unit
---@return real
function GetUnitDefaultPropWindow (whichUnit) end
---@param whichUnit unit
---@return real
function GetUnitDefaultFlyHeight (whichUnit) end

---@param whichUnit unit
---@param whichPlayer player
---@param changeColor boolean
function SetUnitOwner (whichUnit, whichPlayer, changeColor) end
---@param whichUnit unit
---@param whichColor playercolor
function SetUnitColor (whichUnit, whichColor) end

---@param whichUnit unit
---@param scaleX real
---@param scaleY real
---@param scaleZ real
function SetUnitScale (whichUnit, scaleX, scaleY, scaleZ) end
---@param whichUnit unit
---@param timeScale real
function SetUnitTimeScale (whichUnit, timeScale) end
---@param whichUnit unit
---@param blendTime real
function SetUnitBlendTime (whichUnit, blendTime) end
---@param whichUnit unit
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function SetUnitVertexColor (whichUnit, red, green, blue, alpha) end

---@param whichUnit unit
---@param whichAnimation string
function QueueUnitAnimation (whichUnit, whichAnimation) end
---@param whichUnit unit
---@param whichAnimation string
function SetUnitAnimation (whichUnit, whichAnimation) end
---@param whichUnit unit
---@param whichAnimation integer
function SetUnitAnimationByIndex (whichUnit, whichAnimation) end
---@param whichUnit unit
---@param whichAnimation string
---@param rarity raritycontrol
function SetUnitAnimationWithRarity (whichUnit, whichAnimation, rarity) end
---@param whichUnit unit
---@param animProperties string
---@param add boolean
function AddUnitAnimationProperties (whichUnit, animProperties, add) end

---@param whichUnit unit
---@param whichBone string
---@param lookAtTarget unit
---@param offsetX real
---@param offsetY real
---@param offsetZ real
function SetUnitLookAt (whichUnit, whichBone, lookAtTarget, offsetX, offsetY, offsetZ) end
---@param whichUnit unit
function ResetUnitLookAt (whichUnit) end

---@param whichUnit unit
---@param byWhichPlayer player
---@param flag boolean
function SetUnitRescuable (whichUnit, byWhichPlayer, flag) end
---@param whichUnit unit
---@param range real
function SetUnitRescueRange (whichUnit, range) end

---@param whichHero unit
---@param newStr integer
---@param permanent boolean
function SetHeroStr (whichHero, newStr, permanent) end
---@param whichHero unit
---@param newAgi integer
---@param permanent boolean
function SetHeroAgi (whichHero, newAgi, permanent) end
---@param whichHero unit
---@param newInt integer
---@param permanent boolean
function SetHeroInt (whichHero, newInt, permanent) end

---@param whichHero unit
---@param includeBonuses boolean
---@return integer
function GetHeroStr (whichHero, includeBonuses) end
---@param whichHero unit
---@param includeBonuses boolean
---@return integer
function GetHeroAgi (whichHero, includeBonuses) end
---@param whichHero unit
---@param includeBonuses boolean
---@return integer
function GetHeroInt (whichHero, includeBonuses) end

---@param whichHero unit
---@param howManyLevels integer
---@return boolean
function UnitStripHeroLevel (whichHero, howManyLevels) end

---@param whichHero unit
---@return integer
function GetHeroXP (whichHero) end
---@param whichHero unit
---@param newXpVal integer
---@param showEyeCandy boolean
function SetHeroXP (whichHero, newXpVal, showEyeCandy) end

---@param whichHero unit
---@return integer
function GetHeroSkillPoints (whichHero) end
---@param whichHero unit
---@param skillPointDelta integer
---@return boolean
function UnitModifySkillPoints (whichHero, skillPointDelta) end

---@param whichHero unit
---@param xpToAdd integer
---@param showEyeCandy boolean
function AddHeroXP (whichHero, xpToAdd, showEyeCandy) end
---@param whichHero unit
---@param level integer
---@param showEyeCandy boolean
function SetHeroLevel (whichHero, level, showEyeCandy) end
---@param whichHero unit
---@return integer
function GetHeroLevel (whichHero) end
---@param whichUnit unit
---@return integer
function GetUnitLevel (whichUnit) end
---@param whichHero unit
---@return string
function GetHeroProperName (whichHero) end
---@param whichHero unit
---@param flag boolean
function SuspendHeroXP (whichHero, flag) end
---@param whichHero unit
---@return boolean
function IsSuspendedXP (whichHero) end
---@param whichHero unit
---@param abilityTypeId integer
function SelectHeroSkill (whichHero, abilityTypeId) end
---@param whichUnit unit
---@param abilityTypeId integer
---@return integer
function GetUnitAbilityLevel (whichUnit, abilityTypeId) end
---@param whichUnit unit
---@param abilityTypeId integer
---@return integer
function DecUnitAbilityLevel (whichUnit, abilityTypeId) end
---@param whichUnit unit
---@param abilityTypeId integer
---@return integer
function IncUnitAbilityLevel (whichUnit, abilityTypeId) end
---@param whichUnit unit
---@param abilityTypeId integer
---@param level integer
---@return integer
function SetUnitAbilityLevel (whichUnit, abilityTypeId, level) end
---@param whichHero unit
---@param x real
---@param y real
---@param doEyecandy boolean
---@return boolean
function ReviveHero (whichHero, x, y, doEyecandy) end
---@param whichHero unit
---@param loc location
---@param doEyecandy boolean
---@return boolean
function ReviveHeroLoc (whichHero, loc, doEyecandy) end
---@param whichUnit unit
---@param exploded boolean
function SetUnitExploded (whichUnit, exploded) end
---@param whichUnit unit
---@param flag boolean
function SetUnitInvulnerable (whichUnit, flag) end
---@param whichUnit unit
---@param flag boolean
function PauseUnit (whichUnit, flag) end
---@param whichHero unit
---@return boolean
function IsUnitPaused (whichHero) end
---@param whichUnit unit
---@param flag boolean
function SetUnitPathing (whichUnit, flag) end

function ClearSelection () end
---@param whichUnit unit
---@param flag boolean
function SelectUnit (whichUnit, flag) end

---@param whichUnit unit
---@return integer
function GetUnitPointValue (whichUnit) end
---@param unitTypeId integer
---@return integer
function GetUnitPointValueByType (unitTypeId) end
---@param unitTypeId integer
---@param newPointValue integer
function SetUnitPointValueByType (unitTypeId, newPointValue) end

---@param whichUnit unit
---@param whichItem item
---@return boolean
function UnitAddItem (whichUnit, whichItem) end
---@param whichUnit unit
---@param itemTypeId integer
---@return item
function UnitAddItemById (whichUnit, itemTypeId) end
---@param whichUnit unit
---@param itemTypeId integer
---@param itemSlot integer
---@return boolean
function UnitAddItemToSlotById (whichUnit, itemTypeId, itemSlot) end
---@param whichUnit unit
---@param whichItem item
function UnitRemoveItem (whichUnit, whichItem) end
---@param whichUnit unit
---@param itemSlot integer
---@return item
function UnitRemoveItemFromSlot (whichUnit, itemSlot) end
---@param whichUnit unit
---@param whichItem item
---@return boolean
function UnitHasItem (whichUnit, whichItem) end
---@param whichUnit unit
---@param itemSlot integer
---@return item
function UnitItemInSlot (whichUnit, itemSlot) end
---@param whichUnit unit
---@return integer
function UnitInventorySize (whichUnit) end

---@param whichUnit unit
---@param whichItem item
---@param x real
---@param y real
---@return boolean
function UnitDropItemPoint (whichUnit, whichItem, x, y) end
---@param whichUnit unit
---@param whichItem item
---@param slot integer
---@return boolean
function UnitDropItemSlot (whichUnit, whichItem, slot) end
---@param whichUnit unit
---@param whichItem item
---@param target widget
---@return boolean
function UnitDropItemTarget (whichUnit, whichItem, target) end

---@param whichUnit unit
---@param whichItem item
---@return boolean
function UnitUseItem (whichUnit, whichItem) end
---@param whichUnit unit
---@param whichItem item
---@param x real
---@param y real
---@return boolean
function UnitUseItemPoint (whichUnit, whichItem, x, y) end
---@param whichUnit unit
---@param whichItem item
---@param target widget
---@return boolean
function UnitUseItemTarget (whichUnit, whichItem, target) end

---@param whichUnit unit
---@return real
function GetUnitX (whichUnit) end
---@param whichUnit unit
---@return real
function GetUnitY (whichUnit) end
---@param whichUnit unit
---@return location
function GetUnitLoc (whichUnit) end
---@param whichUnit unit
---@return real
function GetUnitFacing (whichUnit) end
---@param whichUnit unit
---@return real
function GetUnitMoveSpeed (whichUnit) end
---@param whichUnit unit
---@return real
function GetUnitDefaultMoveSpeed (whichUnit) end
---@param whichUnit unit
---@param whichUnitState unitstate
---@return real
function GetUnitState (whichUnit, whichUnitState) end
---@param whichUnit unit
---@return player
function GetOwningPlayer (whichUnit) end
---@param whichUnit unit
---@return integer
function GetUnitTypeId (whichUnit) end
---@param whichUnit unit
---@return race
function GetUnitRace (whichUnit) end
---@param whichUnit unit
---@return string
function GetUnitName (whichUnit) end
---@param whichUnit unit
---@return integer
function GetUnitFoodUsed (whichUnit) end
---@param whichUnit unit
---@return integer
function GetUnitFoodMade (whichUnit) end
---@param unitTypeId integer
---@return integer
function GetFoodMade (unitTypeId) end
---@param unitTypeId integer
---@return integer
function GetFoodUsed (unitTypeId) end
---@param whichUnit unit
---@param useFood boolean
function SetUnitUseFood (whichUnit, useFood) end

---@param whichUnit unit
---@return location
function GetUnitRallyPoint (whichUnit) end
---@param whichUnit unit
---@return unit
function GetUnitRallyUnit (whichUnit) end
---@param whichUnit unit
---@return destructable
function GetUnitRallyDestructable (whichUnit) end

---@param whichUnit unit
---@param whichGroup group
---@return boolean
function IsUnitInGroup (whichUnit, whichGroup) end
---@param whichUnit unit
---@param whichForce force
---@return boolean
function IsUnitInForce (whichUnit, whichForce) end
---@param whichUnit unit
---@param whichPlayer player
---@return boolean
function IsUnitOwnedByPlayer (whichUnit, whichPlayer) end
---@param whichUnit unit
---@param whichPlayer player
---@return boolean
function IsUnitAlly (whichUnit, whichPlayer) end
---@param whichUnit unit
---@param whichPlayer player
---@return boolean
function IsUnitEnemy (whichUnit, whichPlayer) end
---@param whichUnit unit
---@param whichPlayer player
---@return boolean
function IsUnitVisible (whichUnit, whichPlayer) end
---@param whichUnit unit
---@param whichPlayer player
---@return boolean
function IsUnitDetected (whichUnit, whichPlayer) end
---@param whichUnit unit
---@param whichPlayer player
---@return boolean
function IsUnitInvisible (whichUnit, whichPlayer) end
---@param whichUnit unit
---@param whichPlayer player
---@return boolean
function IsUnitFogged (whichUnit, whichPlayer) end
---@param whichUnit unit
---@param whichPlayer player
---@return boolean
function IsUnitMasked (whichUnit, whichPlayer) end
---@param whichUnit unit
---@param whichPlayer player
---@return boolean
function IsUnitSelected (whichUnit, whichPlayer) end
---@param whichUnit unit
---@param whichRace race
---@return boolean
function IsUnitRace (whichUnit, whichRace) end
---@param whichUnit unit
---@param whichUnitType unittype
---@return boolean
function IsUnitType (whichUnit, whichUnitType) end
---@param whichUnit unit
---@param whichSpecifiedUnit unit
---@return boolean
function IsUnit (whichUnit, whichSpecifiedUnit) end
---@param whichUnit unit
---@param otherUnit unit
---@param distance real
---@return boolean
function IsUnitInRange (whichUnit, otherUnit, distance) end
---@param whichUnit unit
---@param x real
---@param y real
---@param distance real
---@return boolean
function IsUnitInRangeXY (whichUnit, x, y, distance) end
---@param whichUnit unit
---@param whichLocation location
---@param distance real
---@return boolean
function IsUnitInRangeLoc (whichUnit, whichLocation, distance) end
---@param whichUnit unit
---@return boolean
function IsUnitHidden (whichUnit) end
---@param whichUnit unit
---@return boolean
function IsUnitIllusion (whichUnit) end

---@param whichUnit unit
---@param whichTransport unit
---@return boolean
function IsUnitInTransport (whichUnit, whichTransport) end
---@param whichUnit unit
---@return boolean
function IsUnitLoaded (whichUnit) end

---@param unitTypeId integer
---@return boolean
function IsHeroUnitId (unitTypeId) end
---@param unitTypeId integer
---@param whichUnitType unittype
---@return boolean
function IsUnitIdType (unitTypeId, whichUnitType) end

---@param whichUnit unit
---@param whichPlayer player
---@param share boolean
function UnitShareVision (whichUnit, whichPlayer, share) end
---@param whichUnit unit
---@param suspend boolean
function UnitSuspendDecay (whichUnit, suspend) end
---@param whichUnit unit
---@param whichUnitType unittype
---@return boolean
function UnitAddType (whichUnit, whichUnitType) end
---@param whichUnit unit
---@param whichUnitType unittype
---@return boolean
function UnitRemoveType (whichUnit, whichUnitType) end

---@param whichUnit unit
---@param abilityTypeId integer
---@return boolean
function UnitAddAbility (whichUnit, abilityTypeId) end
---@param whichUnit unit
---@param abilityTypeId integer
---@return boolean
function UnitRemoveAbility (whichUnit, abilityTypeId) end
---@param whichUnit unit
---@param permanent boolean
---@param abilityTypeId integer
---@return boolean
function UnitMakeAbilityPermanent (whichUnit, permanent, abilityTypeId) end
---@param whichUnit unit
---@param removePositive boolean
---@param removeNegative boolean
function UnitRemoveBuffs (whichUnit, removePositive, removeNegative) end
---@param whichUnit unit
---@param removePositive boolean
---@param removeNegative boolean
---@param magic boolean
---@param physical boolean
---@param timedLife boolean
---@param aura boolean
---@param autoDispel boolean
function UnitRemoveBuffsEx (whichUnit, removePositive, removeNegative, magic, physical, timedLife, aura, autoDispel) end
---@param whichUnit unit
---@param removePositive boolean
---@param removeNegative boolean
---@param magic boolean
---@param physical boolean
---@param timedLife boolean
---@param aura boolean
---@param autoDispel boolean
---@return boolean
function UnitHasBuffsEx (whichUnit, removePositive, removeNegative, magic, physical, timedLife, aura, autoDispel) end
---@param whichUnit unit
---@param removePositive boolean
---@param removeNegative boolean
---@param magic boolean
---@param physical boolean
---@param timedLife boolean
---@param aura boolean
---@param autoDispel boolean
---@return integer
function UnitCountBuffsEx (whichUnit, removePositive, removeNegative, magic, physical, timedLife, aura, autoDispel) end
---@param whichUnit unit
---@param add boolean
function UnitAddSleep (whichUnit, add) end
---@param whichUnit unit
---@return boolean
function UnitCanSleep (whichUnit) end
---@param whichUnit unit
---@param add boolean
function UnitAddSleepPerm (whichUnit, add) end
---@param whichUnit unit
---@return boolean
function UnitCanSleepPerm (whichUnit) end
---@param whichUnit unit
---@return boolean
function UnitIsSleeping (whichUnit) end
---@param whichUnit unit
function UnitWakeUp (whichUnit) end
---@param whichUnit unit
---@param buffTypeId integer
---@param duration real
function UnitApplyTimedLife (whichUnit, buffTypeId, duration) end
---@param whichUnit unit
---@param flag boolean
---@return boolean
function UnitIgnoreAlarm (whichUnit, flag) end
---@param whichUnit unit
---@return boolean
function UnitIgnoreAlarmToggled (whichUnit) end
---@param whichUnit unit
function UnitResetCooldown (whichUnit) end
---@param whichUnit unit
---@param constructionPercentage integer
function UnitSetConstructionProgress (whichUnit, constructionPercentage) end
---@param whichUnit unit
---@param upgradePercentage integer
function UnitSetUpgradeProgress (whichUnit, upgradePercentage) end
---@param whichUnit unit
---@param flag boolean
function UnitPauseTimedLife (whichUnit, flag) end
---@param whichUnit unit
---@param flag boolean
function UnitSetUsesAltIcon (whichUnit, flag) end

---@param whichUnit unit
---@param delay real
---@param radius real
---@param x real
---@param y real
---@param amount real
---@param attack boolean
---@param ranged boolean
---@param attackType attacktype
---@param damageType damagetype
---@param weaponType weapontype
---@return boolean
function UnitDamagePoint (whichUnit, delay, radius, x, y, amount, attack, ranged, attackType, damageType, weaponType) end
---@param whichUnit unit
---@param target widget
---@param amount real
---@param attack boolean
---@param ranged boolean
---@param attackType attacktype
---@param damageType damagetype
---@param weaponType weapontype
---@return boolean
function UnitDamageTarget (whichUnit, target, amount, attack, ranged, attackType, damageType, weaponType) end

---@param whichUnit unit
---@param orderName string
---@return boolean
function IssueImmediateOrder (whichUnit, orderName) end
---@param whichUnit unit
---@param orderId integer
---@return boolean
function IssueImmediateOrderById (whichUnit, orderId) end
---@param whichUnit unit
---@param orderName string
---@param x real
---@param y real
---@return boolean
function IssuePointOrder (whichUnit, orderName, x, y) end
---@param whichUnit unit
---@param orderName string
---@param whichLocation location
---@return boolean
function IssuePointOrderLoc (whichUnit, orderName, whichLocation) end
---@param whichUnit unit
---@param orderId integer
---@param x real
---@param y real
---@return boolean
function IssuePointOrderById (whichUnit, orderId, x, y) end
---@param whichUnit unit
---@param orderId integer
---@param whichLocation location
---@return boolean
function IssuePointOrderByIdLoc (whichUnit, orderId, whichLocation) end
---@param whichUnit unit
---@param orderName string
---@param targetWidget widget
---@return boolean
function IssueTargetOrder (whichUnit, orderName, targetWidget) end
---@param whichUnit unit
---@param orderId integer
---@param targetWidget widget
---@return boolean
function IssueTargetOrderById (whichUnit, orderId, targetWidget) end
---@param whichUnit unit
---@param orderName string
---@param x real
---@param y real
---@param instantTargetWidget widget
---@return boolean
function IssueInstantPointOrder (whichUnit, orderName, x, y, instantTargetWidget) end
---@param whichUnit unit
---@param orderId integer
---@param x real
---@param y real
---@param instantTargetWidget widget
---@return boolean
function IssueInstantPointOrderById (whichUnit, orderId, x, y, instantTargetWidget) end
---@param whichUnit unit
---@param orderName string
---@param targetWidget widget
---@param instantTargetWidget widget
---@return boolean
function IssueInstantTargetOrder (whichUnit, orderName, targetWidget, instantTargetWidget) end
---@param whichUnit unit
---@param orderId integer
---@param targetWidget widget
---@param instantTargetWidget widget
---@return boolean
function IssueInstantTargetOrderById (whichUnit, orderId, targetWidget, instantTargetWidget) end
---@param whichPeon unit
---@param unitToBuild string
---@param x real
---@param y real
---@return boolean
function IssueBuildOrder (whichPeon, unitToBuild, x, y) end
---@param whichPeon unit
---@param unitTypeId integer
---@param x real
---@param y real
---@return boolean
function IssueBuildOrderById (whichPeon, unitTypeId, x, y) end

---@param whichPlayer player
---@param neutralStructure unit
---@param unitToBuild string
---@return boolean
function IssueNeutralImmediateOrder (whichPlayer, neutralStructure, unitToBuild) end
---@param whichPlayer player
---@param neutralStructure unit
---@param unitTypeId integer
---@return boolean
function IssueNeutralImmediateOrderById (whichPlayer, neutralStructure, unitTypeId) end
---@param whichPlayer player
---@param neutralStructure unit
---@param unitToBuild string
---@param x real
---@param y real
---@return boolean
function IssueNeutralPointOrder (whichPlayer, neutralStructure, unitToBuild, x, y) end
---@param whichPlayer player
---@param neutralStructure unit
---@param unitTypeId integer
---@param x real
---@param y real
---@return boolean
function IssueNeutralPointOrderById (whichPlayer, neutralStructure, unitTypeId, x, y) end
---@param whichPlayer player
---@param neutralStructure unit
---@param unitToBuild string
---@param target widget
---@return boolean
function IssueNeutralTargetOrder (whichPlayer, neutralStructure, unitToBuild, target) end
---@param whichPlayer player
---@param neutralStructure unit
---@param unitTypeId integer
---@param target widget
---@return boolean
function IssueNeutralTargetOrderById (whichPlayer, neutralStructure, unitTypeId, target) end

---@param whichUnit unit
---@return integer
function GetUnitCurrentOrder (whichUnit) end

---@param whichUnit unit
---@param amount integer
function SetResourceAmount (whichUnit, amount) end
---@param whichUnit unit
---@param amount integer
function AddResourceAmount (whichUnit, amount) end
---@param whichUnit unit
---@return integer
function GetResourceAmount (whichUnit) end

---@param waygate unit
---@return real
function WaygateGetDestinationX (waygate) end
---@param waygate unit
---@return real
function WaygateGetDestinationY (waygate) end
---@param waygate unit
---@param x real
---@param y real
function WaygateSetDestination (waygate, x, y) end
---@param waygate unit
---@param activate boolean
function WaygateActivate (waygate, activate) end
---@param waygate unit
---@return boolean
function WaygateIsActive (waygate) end

---@param itemTypeId integer
---@param currentStock integer
---@param stockMax integer
function AddItemToAllStock (itemTypeId, currentStock, stockMax) end
---@param whichUnit unit
---@param itemTypeId integer
---@param currentStock integer
---@param stockMax integer
function AddItemToStock (whichUnit, itemTypeId, currentStock, stockMax) end
---@param unitTypeId integer
---@param currentStock integer
---@param stockMax integer
function AddUnitToAllStock (unitTypeId, currentStock, stockMax) end
---@param whichUnit unit
---@param unitTypeId integer
---@param currentStock integer
---@param stockMax integer
function AddUnitToStock (whichUnit, unitTypeId, currentStock, stockMax) end

---@param itemTypeId integer
function RemoveItemFromAllStock (itemTypeId) end
---@param whichUnit unit
---@param itemTypeId integer
function RemoveItemFromStock (whichUnit, itemTypeId) end
---@param unitTypeId integer
function RemoveUnitFromAllStock (unitTypeId) end
---@param whichUnit unit
---@param unitTypeId integer
function RemoveUnitFromStock (whichUnit, unitTypeId) end

---@param slots integer
function SetAllItemTypeSlots (slots) end
---@param slots integer
function SetAllUnitTypeSlots (slots) end
---@param whichUnit unit
---@param slots integer
function SetItemTypeSlots (whichUnit, slots) end
---@param whichUnit unit
---@param slots integer
function SetUnitTypeSlots (whichUnit, slots) end

---@param whichUnit unit
---@return integer
function GetUnitUserData (whichUnit) end
---@param whichUnit unit
---@param data integer
function SetUnitUserData (whichUnit, data) end

-- ============================================================================
-- Player API
---@param number integer
---@return player
function Player (number) end
---@return player
function GetLocalPlayer () end
---@param whichPlayer player
---@param otherPlayer player
---@return boolean
function IsPlayerAlly (whichPlayer, otherPlayer) end
---@param whichPlayer player
---@param otherPlayer player
---@return boolean
function IsPlayerEnemy (whichPlayer, otherPlayer) end
---@param whichPlayer player
---@param whichForce force
---@return boolean
function IsPlayerInForce (whichPlayer, whichForce) end
---@param whichPlayer player
---@return boolean
function IsPlayerObserver (whichPlayer) end
---@param x real
---@param y real
---@param whichPlayer player
---@return boolean
function IsVisibleToPlayer (x, y, whichPlayer) end
---@param whichLocation location
---@param whichPlayer player
---@return boolean
function IsLocationVisibleToPlayer (whichLocation, whichPlayer) end
---@param x real
---@param y real
---@param whichPlayer player
---@return boolean
function IsFoggedToPlayer (x, y, whichPlayer) end
---@param whichLocation location
---@param whichPlayer player
---@return boolean
function IsLocationFoggedToPlayer (whichLocation, whichPlayer) end
---@param x real
---@param y real
---@param whichPlayer player
---@return boolean
function IsMaskedToPlayer (x, y, whichPlayer) end
---@param whichLocation location
---@param whichPlayer player
---@return boolean
function IsLocationMaskedToPlayer (whichLocation, whichPlayer) end

---@param whichPlayer player
---@return race
function GetPlayerRace (whichPlayer) end
---@param whichPlayer player
---@return integer
function GetPlayerId (whichPlayer) end
---@param whichPlayer player
---@param includeIncomplete boolean
---@return integer
function GetPlayerUnitCount (whichPlayer, includeIncomplete) end
---@param whichPlayer player
---@param unitName string
---@param includeIncomplete boolean
---@param includeUpgrades boolean
---@return integer
function GetPlayerTypedUnitCount (whichPlayer, unitName, includeIncomplete, includeUpgrades) end
---@param whichPlayer player
---@param includeIncomplete boolean
---@return integer
function GetPlayerStructureCount (whichPlayer, includeIncomplete) end
---@param whichPlayer player
---@param whichPlayerState playerstate
---@return integer
function GetPlayerState (whichPlayer, whichPlayerState) end
---@param whichPlayer player
---@param whichPlayerScore playerscore
---@return integer
function GetPlayerScore (whichPlayer, whichPlayerScore) end
---@param sourcePlayer player
---@param otherPlayer player
---@param whichAllianceSetting alliancetype
---@return boolean
function GetPlayerAlliance (sourcePlayer, otherPlayer, whichAllianceSetting) end

---@param whichPlayer player
---@return real
function GetPlayerHandicap (whichPlayer) end
---@param whichPlayer player
---@return real
function GetPlayerHandicapXP (whichPlayer) end
---@param whichPlayer player
---@param handicap real
function SetPlayerHandicap (whichPlayer, handicap) end
---@param whichPlayer player
---@param handicap real
function SetPlayerHandicapXP (whichPlayer, handicap) end

---@param whichPlayer player
---@param techid integer
---@param maximum integer
function SetPlayerTechMaxAllowed (whichPlayer, techid, maximum) end
---@param whichPlayer player
---@param techid integer
---@return integer
function GetPlayerTechMaxAllowed (whichPlayer, techid) end
---@param whichPlayer player
---@param techid integer
---@param levels integer
function AddPlayerTechResearched (whichPlayer, techid, levels) end
---@param whichPlayer player
---@param techid integer
---@param setToLevel integer
function SetPlayerTechResearched (whichPlayer, techid, setToLevel) end
---@param whichPlayer player
---@param techid integer
---@param specificonly boolean
---@return boolean
function GetPlayerTechResearched (whichPlayer, techid, specificonly) end
---@param whichPlayer player
---@param techid integer
---@param specificonly boolean
---@return integer
function GetPlayerTechCount (whichPlayer, techid, specificonly) end

---@param whichPlayer player
---@param newOwner integer
function SetPlayerUnitsOwner (whichPlayer, newOwner) end
---@param whichPlayer player
---@param toWhichPlayers force
---@param flag boolean
function CripplePlayer (whichPlayer, toWhichPlayers, flag) end

---@param whichPlayer player
---@param abilityTypeId integer
---@param avail boolean
function SetPlayerAbilityAvailable (whichPlayer, abilityTypeId, avail) end

---@param whichPlayer player
---@param whichPlayerState playerstate
---@param value integer
function SetPlayerState (whichPlayer, whichPlayerState, value) end
---@param whichPlayer player
---@param gameResult playergameresult
function RemovePlayer (whichPlayer, gameResult) end

-- Used to store hero level data for the scorescreen
-- before units are moved to neutral passive in melee games
-- 
---@param whichPlayer player
function CachePlayerHeroData (whichPlayer) end

-- ============================================================================
-- Fog of War API
---@param whichPlayer player
---@param whichState fogstate
---@param whichRect rect
---@param useSharedVision boolean
function SetFogStateRect (whichPlayer, whichState, whichRect, useSharedVision) end
---@param whichPlayer player
---@param whichState fogstate
---@param centerX real
---@param centerY real
---@param radius real
---@param useSharedVision boolean
function SetFogStateRadius (whichPlayer, whichState, centerX, centerY, radius, useSharedVision) end
---@param whichPlayer player
---@param whichState fogstate
---@param center location
---@param radius real
---@param useSharedVision boolean
function SetFogStateRadiusLoc (whichPlayer, whichState, center, radius, useSharedVision) end
---@param enable boolean
function FogMaskEnable (enable) end
---@return boolean
function IsFogMaskEnabled () end
---@param enable boolean
function FogEnable (enable) end
---@return boolean
function IsFogEnabled () end

---@param whichPlayer player
---@param whichState fogstate
---@param whichRect rect
---@param useSharedVision boolean
---@param afterUnits boolean
---@return fogmodifier
function CreateFogModifierRect (whichPlayer, whichState, whichRect, useSharedVision, afterUnits) end
---@param whichPlayer player
---@param whichState fogstate
---@param centerX real
---@param centerY real
---@param radius real
---@param useSharedVision boolean
---@param afterUnits boolean
---@return fogmodifier
function CreateFogModifierRadius (whichPlayer, whichState, centerX, centerY, radius, useSharedVision, afterUnits) end
---@param whichPlayer player
---@param whichState fogstate
---@param center location
---@param radius real
---@param useSharedVision boolean
---@param afterUnits boolean
---@return fogmodifier
function CreateFogModifierRadiusLoc (whichPlayer, whichState, center, radius, useSharedVision, afterUnits) end
---@param whichFogModifier fogmodifier
function DestroyFogModifier (whichFogModifier) end
---@param whichFogModifier fogmodifier
function FogModifierStart (whichFogModifier) end
---@param whichFogModifier fogmodifier
function FogModifierStop (whichFogModifier) end

-- ============================================================================
-- Game API
---@return version
function VersionGet () end
---@param whichVersion version
---@return boolean
function VersionCompatible (whichVersion) end
---@param whichVersion version
---@return boolean
function VersionSupported (whichVersion) end

---@param doScoreScreen boolean
function EndGame (doScoreScreen) end

-- Async only!
---@param newLevel string
---@param doScoreScreen boolean
function ChangeLevel (newLevel, doScoreScreen) end
---@param doScoreScreen boolean
function RestartGame (doScoreScreen) end
function ReloadGame () end
-- %%% SetCampaignMenuRace is deprecated.It must remain to support
-- old maps which use it, but all new maps should use SetCampaignMenuRaceEx
---@param whichRace race
function SetCampaignMenuRace (whichRace) end
---@param campaignIndex integer
function SetCampaignMenuRaceEx (campaignIndex) end
function ForceCampaignSelectScreen () end

---@param saveFileName string
---@param doScoreScreen boolean
function LoadGame (saveFileName, doScoreScreen) end
---@param saveFileName string
function SaveGame (saveFileName) end
---@param sourceDirName string
---@param destDirName string
---@return boolean
function RenameSaveDirectory (sourceDirName, destDirName) end
---@param sourceDirName string
---@return boolean
function RemoveSaveDirectory (sourceDirName) end
---@param sourceSaveName string
---@param destSaveName string
---@return boolean
function CopySaveGame (sourceSaveName, destSaveName) end
---@param saveName string
---@return boolean
function SaveGameExists (saveName) end
function SyncSelections () end
---@param whichFloatGameState fgamestate
---@param value real
function SetFloatGameState (whichFloatGameState, value) end
---@param whichFloatGameState fgamestate
---@return real
function GetFloatGameState (whichFloatGameState) end
---@param whichIntegerGameState igamestate
---@param value integer
function SetIntegerGameState (whichIntegerGameState, value) end
---@param whichIntegerGameState igamestate
---@return integer
function GetIntegerGameState (whichIntegerGameState) end


-- ============================================================================
-- Campaign API
---@param cleared boolean
function SetTutorialCleared (cleared) end
---@param campaignNumber integer
---@param missionNumber integer
---@param available boolean
function SetMissionAvailable (campaignNumber, missionNumber, available) end
---@param campaignNumber integer
---@param available boolean
function SetCampaignAvailable (campaignNumber, available) end
---@param campaignNumber integer
---@param available boolean
function SetOpCinematicAvailable (campaignNumber, available) end
---@param campaignNumber integer
---@param available boolean
function SetEdCinematicAvailable (campaignNumber, available) end
---@return gamedifficulty
function GetDefaultDifficulty () end
---@param g gamedifficulty
function SetDefaultDifficulty (g) end
---@param whichButton integer
---@param visible boolean
function SetCustomCampaignButtonVisible (whichButton, visible) end
---@param whichButton integer
---@return boolean
function GetCustomCampaignButtonVisible (whichButton) end
function DoNotSaveReplay () end

-- ============================================================================
-- Dialog API
---@return dialog
function DialogCreate () end
---@param whichDialog dialog
function DialogDestroy (whichDialog) end
---@param whichDialog dialog
function DialogClear (whichDialog) end
---@param whichDialog dialog
---@param messageText string
function DialogSetMessage (whichDialog, messageText) end
---@param whichDialog dialog
---@param buttonText string
---@param hotkey integer
---@return button
function DialogAddButton (whichDialog, buttonText, hotkey) end
---@param whichDialog dialog
---@param doScoreScreen boolean
---@param buttonText string
---@param hotkey integer
---@return button
function DialogAddQuitButton (whichDialog, doScoreScreen, buttonText, hotkey) end
---@param whichPlayer player
---@param whichDialog dialog
---@param flag boolean
function DialogDisplay (whichPlayer, whichDialog, flag) end

-- Creates a new or reads in an existing game cache file stored
-- in the current campaign profile dir
-- 
---@return boolean
function ReloadGameCachesFromDisk () end

---@param campaignFile string
---@return gamecache
function InitGameCache (campaignFile) end
---@param whichCache gamecache
---@return boolean
function SaveGameCache (whichCache) end

---@param whichGamecache gamecache
---@param missionKey string
---@param key string
---@param value integer
function StoreInteger (whichGamecache, missionKey, key, value) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
---@param value real
function StoreReal (whichGamecache, missionKey, key, value) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
---@param value boolean
function StoreBoolean (whichGamecache, missionKey, key, value) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
---@param whichUnit unit
---@return boolean
function StoreUnit (whichGamecache, missionKey, key, whichUnit) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
---@param value string
---@return boolean
function StoreString (whichGamecache, missionKey, key, value) end

---@param whichGamecache gamecache
---@param missionKey string
---@param key string
function SyncStoredInteger (whichGamecache, missionKey, key) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
function SyncStoredReal (whichGamecache, missionKey, key) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
function SyncStoredBoolean (whichGamecache, missionKey, key) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
function SyncStoredUnit (whichGamecache, missionKey, key) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
function SyncStoredString (whichGamecache, missionKey, key) end

---@param whichGamecache gamecache
---@param missionKey string
---@param key string
---@return boolean
function HaveStoredInteger (whichGamecache, missionKey, key) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
---@return boolean
function HaveStoredReal (whichGamecache, missionKey, key) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
---@return boolean
function HaveStoredBoolean (whichGamecache, missionKey, key) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
---@return boolean
function HaveStoredUnit (whichGamecache, missionKey, key) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
---@return boolean
function HaveStoredString (whichGamecache, missionKey, key) end

---@param whichGamecache gamecache
function FlushGameCache (whichGamecache) end
---@param whichGamecache gamecache
---@param missionKey string
function FlushStoredMission (whichGamecache, missionKey) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
function FlushStoredInteger (whichGamecache, missionKey, key) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
function FlushStoredReal (whichGamecache, missionKey, key) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
function FlushStoredBoolean (whichGamecache, missionKey, key) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
function FlushStoredUnit (whichGamecache, missionKey, key) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
function FlushStoredString (whichGamecache, missionKey, key) end

-- Will return 0 if the specified value's data is not found in the cache
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
---@return integer
function GetStoredInteger (whichGamecache, missionKey, key) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
---@return real
function GetStoredReal (whichGamecache, missionKey, key) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
---@return boolean
function GetStoredBoolean (whichGamecache, missionKey, key) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
---@return string
function GetStoredString (whichGamecache, missionKey, key) end
---@param whichGamecache gamecache
---@param missionKey string
---@param key string
---@param whichPlayer player
---@param x real
---@param y real
---@param facing real
---@return unit
function RestoreUnit (whichGamecache, missionKey, key, whichPlayer, x, y, facing) end


---@return hashtable
function InitHashtable () end

---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param value integer
function SaveInteger (whichHashtable, parentKey, childKey, value) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param value real
function SaveReal (whichHashtable, parentKey, childKey, value) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param value boolean
function SaveBoolean (whichHashtable, parentKey, childKey, value) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param value string
---@return boolean
function SaveStr (whichHashtable, parentKey, childKey, value) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichPlayer player
---@return boolean
function SavePlayerHandle (whichHashtable, parentKey, childKey, whichPlayer) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichWidget widget
---@return boolean
function SaveWidgetHandle (whichHashtable, parentKey, childKey, whichWidget) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichDestructable destructable
---@return boolean
function SaveDestructableHandle (whichHashtable, parentKey, childKey, whichDestructable) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichItem item
---@return boolean
function SaveItemHandle (whichHashtable, parentKey, childKey, whichItem) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichUnit unit
---@return boolean
function SaveUnitHandle (whichHashtable, parentKey, childKey, whichUnit) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichAbility ability
---@return boolean
function SaveAbilityHandle (whichHashtable, parentKey, childKey, whichAbility) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichTimer timer
---@return boolean
function SaveTimerHandle (whichHashtable, parentKey, childKey, whichTimer) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichTrigger trigger
---@return boolean
function SaveTriggerHandle (whichHashtable, parentKey, childKey, whichTrigger) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichTriggercondition triggercondition
---@return boolean
function SaveTriggerConditionHandle (whichHashtable, parentKey, childKey, whichTriggercondition) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichTriggeraction triggeraction
---@return boolean
function SaveTriggerActionHandle (whichHashtable, parentKey, childKey, whichTriggeraction) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichEvent event
---@return boolean
function SaveTriggerEventHandle (whichHashtable, parentKey, childKey, whichEvent) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichForce force
---@return boolean
function SaveForceHandle (whichHashtable, parentKey, childKey, whichForce) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichGroup group
---@return boolean
function SaveGroupHandle (whichHashtable, parentKey, childKey, whichGroup) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichLocation location
---@return boolean
function SaveLocationHandle (whichHashtable, parentKey, childKey, whichLocation) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichRect rect
---@return boolean
function SaveRectHandle (whichHashtable, parentKey, childKey, whichRect) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichBoolexpr boolexpr
---@return boolean
function SaveBooleanExprHandle (whichHashtable, parentKey, childKey, whichBoolexpr) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichSound sound
---@return boolean
function SaveSoundHandle (whichHashtable, parentKey, childKey, whichSound) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichEffect effect
---@return boolean
function SaveEffectHandle (whichHashtable, parentKey, childKey, whichEffect) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichUnitpool unitpool
---@return boolean
function SaveUnitPoolHandle (whichHashtable, parentKey, childKey, whichUnitpool) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichItempool itempool
---@return boolean
function SaveItemPoolHandle (whichHashtable, parentKey, childKey, whichItempool) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichQuest quest
---@return boolean
function SaveQuestHandle (whichHashtable, parentKey, childKey, whichQuest) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichQuestitem questitem
---@return boolean
function SaveQuestItemHandle (whichHashtable, parentKey, childKey, whichQuestitem) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichDefeatcondition defeatcondition
---@return boolean
function SaveDefeatConditionHandle (whichHashtable, parentKey, childKey, whichDefeatcondition) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichTimerdialog timerdialog
---@return boolean
function SaveTimerDialogHandle (whichHashtable, parentKey, childKey, whichTimerdialog) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichLeaderboard leaderboard
---@return boolean
function SaveLeaderboardHandle (whichHashtable, parentKey, childKey, whichLeaderboard) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichMultiboard multiboard
---@return boolean
function SaveMultiboardHandle (whichHashtable, parentKey, childKey, whichMultiboard) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichMultiboarditem multiboarditem
---@return boolean
function SaveMultiboardItemHandle (whichHashtable, parentKey, childKey, whichMultiboarditem) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichTrackable trackable
---@return boolean
function SaveTrackableHandle (whichHashtable, parentKey, childKey, whichTrackable) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichDialog dialog
---@return boolean
function SaveDialogHandle (whichHashtable, parentKey, childKey, whichDialog) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichButton button
---@return boolean
function SaveButtonHandle (whichHashtable, parentKey, childKey, whichButton) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichTexttag texttag
---@return boolean
function SaveTextTagHandle (whichHashtable, parentKey, childKey, whichTexttag) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichLightning lightning
---@return boolean
function SaveLightningHandle (whichHashtable, parentKey, childKey, whichLightning) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichImage image
---@return boolean
function SaveImageHandle (whichHashtable, parentKey, childKey, whichImage) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichUbersplat ubersplat
---@return boolean
function SaveUbersplatHandle (whichHashtable, parentKey, childKey, whichUbersplat) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichRegion region
---@return boolean
function SaveRegionHandle (whichHashtable, parentKey, childKey, whichRegion) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichFogState fogstate
---@return boolean
function SaveFogStateHandle (whichHashtable, parentKey, childKey, whichFogState) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichFogModifier fogmodifier
---@return boolean
function SaveFogModifierHandle (whichHashtable, parentKey, childKey, whichFogModifier) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichAgent agent
---@return boolean
function SaveAgentHandle (whichHashtable, parentKey, childKey, whichAgent) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichHashtable hashtable
---@return boolean
function SaveHashtableHandle (whichHashtable, parentKey, childKey, whichHashtable) end

---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return integer
function LoadInteger (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return real
function LoadReal (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return boolean
function LoadBoolean (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return string
function LoadStr (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return player
function LoadPlayerHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return widget
function LoadWidgetHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return destructable
function LoadDestructableHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return item
function LoadItemHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return unit
function LoadUnitHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return ability
function LoadAbilityHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return timer
function LoadTimerHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return trigger
function LoadTriggerHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return triggercondition
function LoadTriggerConditionHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return triggeraction
function LoadTriggerActionHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return event
function LoadTriggerEventHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return force
function LoadForceHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return group
function LoadGroupHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return location
function LoadLocationHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return rect
function LoadRectHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return boolexpr
function LoadBooleanExprHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return sound
function LoadSoundHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return effect
function LoadEffectHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return unitpool
function LoadUnitPoolHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return itempool
function LoadItemPoolHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return quest
function LoadQuestHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return questitem
function LoadQuestItemHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return defeatcondition
function LoadDefeatConditionHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return timerdialog
function LoadTimerDialogHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return leaderboard
function LoadLeaderboardHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return multiboard
function LoadMultiboardHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return multiboarditem
function LoadMultiboardItemHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return trackable
function LoadTrackableHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return dialog
function LoadDialogHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return button
function LoadButtonHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return texttag
function LoadTextTagHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return lightning
function LoadLightningHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return image
function LoadImageHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return ubersplat
function LoadUbersplatHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return region
function LoadRegionHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return fogstate
function LoadFogStateHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return fogmodifier
function LoadFogModifierHandle (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return hashtable
function LoadHashtableHandle (whichHashtable, parentKey, childKey) end

---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return boolean
function HaveSavedInteger (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return boolean
function HaveSavedReal (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return boolean
function HaveSavedBoolean (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return boolean
function HaveSavedString (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return boolean
function HaveSavedHandle (whichHashtable, parentKey, childKey) end

---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
function RemoveSavedInteger (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
function RemoveSavedReal (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
function RemoveSavedBoolean (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
function RemoveSavedString (whichHashtable, parentKey, childKey) end
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
function RemoveSavedHandle (whichHashtable, parentKey, childKey) end

---@param whichHashtable hashtable
function FlushParentHashtable (whichHashtable) end
---@param whichHashtable hashtable
---@param parentKey integer
function FlushChildHashtable (whichHashtable, parentKey) end


-- ============================================================================
-- Randomization API
---@param lowBound integer
---@param highBound integer
---@return integer
function GetRandomInt (lowBound, highBound) end
---@param lowBound real
---@param highBound real
---@return real
function GetRandomReal (lowBound, highBound) end

---@return unitpool
function CreateUnitPool () end
---@param whichPool unitpool
function DestroyUnitPool (whichPool) end
---@param whichPool unitpool
---@param unitTypeId integer
---@param weight real
function UnitPoolAddUnitType (whichPool, unitTypeId, weight) end
---@param whichPool unitpool
---@param unitTypeId integer
function UnitPoolRemoveUnitType (whichPool, unitTypeId) end
---@param whichPool unitpool
---@param whichPlayer player
---@param x real
---@param y real
---@param facing real
---@return unit
function PlaceRandomUnit (whichPool, whichPlayer, x, y, facing) end

---@return itempool
function CreateItemPool () end
---@param whichItemPool itempool
function DestroyItemPool (whichItemPool) end
---@param whichItemPool itempool
---@param itemTypeId integer
---@param weight real
function ItemPoolAddItemType (whichItemPool, itemTypeId, weight) end
---@param whichItemPool itempool
---@param itemTypeId integer
function ItemPoolRemoveItemType (whichItemPool, itemTypeId) end
---@param whichItemPool itempool
---@param x real
---@param y real
---@return item
function PlaceRandomItem (whichItemPool, x, y) end

-- Choose any random unit/item. (NP means Neutral Passive)
---@param level integer
---@return integer
function ChooseRandomCreep (level) end
---@return integer
function ChooseRandomNPBuilding () end
---@param level integer
---@return integer
function ChooseRandomItem (level) end
---@param whichType itemtype
---@param level integer
---@return integer
function ChooseRandomItemEx (whichType, level) end
---@param seed integer
function SetRandomSeed (seed) end

-- ============================================================================
-- Visual API
---@param a real
---@param b real
---@param c real
---@param d real
---@param e real
function SetTerrainFog (a, b, c, d, e) end
function ResetTerrainFog () end

---@param a real
---@param b real
---@param c real
---@param d real
---@param e real
function SetUnitFog (a, b, c, d, e) end
---@param style integer
---@param zstart real
---@param zend real
---@param density real
---@param red real
---@param green real
---@param blue real
function SetTerrainFogEx (style, zstart, zend, density, red, green, blue) end
---@param toPlayer player
---@param x real
---@param y real
---@param message string
function DisplayTextToPlayer (toPlayer, x, y, message) end
---@param toPlayer player
---@param x real
---@param y real
---@param duration real
---@param message string
function DisplayTimedTextToPlayer (toPlayer, x, y, duration, message) end
---@param toPlayer player
---@param x real
---@param y real
---@param duration real
---@param message string
function DisplayTimedTextFromPlayer (toPlayer, x, y, duration, message) end
function ClearTextMessages () end
---@param terrainDNCFile string
---@param unitDNCFile string
function SetDayNightModels (terrainDNCFile, unitDNCFile) end
---@param skyModelFile string
function SetSkyModel (skyModelFile) end
---@param b boolean
function EnableUserControl (b) end
---@param b boolean
function EnableUserUI (b) end
---@param b boolean
function SuspendTimeOfDay (b) end
---@param r real
function SetTimeOfDayScale (r) end
---@return real
function GetTimeOfDayScale () end
---@param flag boolean
---@param fadeDuration real
function ShowInterface (flag, fadeDuration) end
---@param flag boolean
function PauseGame (flag) end
---@param whichUnit unit
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function UnitAddIndicator (whichUnit, red, green, blue, alpha) end
---@param whichWidget widget
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function AddIndicator (whichWidget, red, green, blue, alpha) end
---@param x real
---@param y real
---@param duration real
function PingMinimap (x, y, duration) end
---@param x real
---@param y real
---@param duration real
---@param red integer
---@param green integer
---@param blue integer
---@param extraEffects boolean
function PingMinimapEx (x, y, duration, red, green, blue, extraEffects) end
---@param flag boolean
function EnableOcclusion (flag) end
---@param introText string
function SetIntroShotText (introText) end
---@param introModelPath string
function SetIntroShotModel (introModelPath) end
---@param b boolean
function EnableWorldFogBoundary (b) end
---@param modelName string
function PlayModelCinematic (modelName) end
---@param movieName string
function PlayCinematic (movieName) end
---@param key string
function ForceUIKey (key) end
function ForceUICancel () end
function DisplayLoadDialog () end
---@param iconPath string
function SetAltMinimapIcon (iconPath) end
---@param flag boolean
function DisableRestartMission (flag) end

---@return texttag
function CreateTextTag () end
---@param whichTextTag texttag
function DestroyTextTag (whichTextTag) end
---@param whichTextTag texttag
---@param s string
---@param height real
function SetTextTagText (whichTextTag, s, height) end
---@param whichTextTag texttag
---@param x real
---@param y real
---@param heightOffset real
function SetTextTagPos (whichTextTag, x, y, heightOffset) end
---@param whichTextTag texttag
---@param whichUnit unit
---@param heightOffset real
function SetTextTagPosUnit (whichTextTag, whichUnit, heightOffset) end
---@param whichTextTag texttag
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function SetTextTagColor (whichTextTag, red, green, blue, alpha) end
---@param whichTextTag texttag
---@param xvel real
---@param yvel real
function SetTextTagVelocity (whichTextTag, xvel, yvel) end
---@param whichTextTag texttag
---@param flag boolean
function SetTextTagVisibility (whichTextTag, flag) end
---@param whichTextTag texttag
---@param flag boolean
function SetTextTagSuspended (whichTextTag, flag) end
---@param whichTextTag texttag
---@param flag boolean
function SetTextTagPermanent (whichTextTag, flag) end
---@param whichTextTag texttag
---@param age real
function SetTextTagAge (whichTextTag, age) end
---@param whichTextTag texttag
---@param lifespan real
function SetTextTagLifespan (whichTextTag, lifespan) end
---@param whichTextTag texttag
---@param fadepoint real
function SetTextTagFadepoint (whichTextTag, fadepoint) end

---@param reserved integer
function SetReservedLocalHeroButtons (reserved) end
---@return integer
function GetAllyColorFilterState () end
---@param state integer
function SetAllyColorFilterState (state) end
---@return boolean
function GetCreepCampFilterState () end
---@param state boolean
function SetCreepCampFilterState (state) end
---@param enableAlly boolean
---@param enableCreep boolean
function EnableMinimapFilterButtons (enableAlly, enableCreep) end
---@param state boolean
---@param ui boolean
function EnableDragSelect (state, ui) end
---@param state boolean
---@param ui boolean
function EnablePreSelect (state, ui) end
---@param state boolean
---@param ui boolean
function EnableSelect (state, ui) end

-- ============================================================================
-- Trackable API
---@param trackableModelPath string
---@param x real
---@param y real
---@param facing real
---@return trackable
function CreateTrackable (trackableModelPath, x, y, facing) end

-- ============================================================================
-- Quest API
---@return quest
function CreateQuest () end
---@param whichQuest quest
function DestroyQuest (whichQuest) end
---@param whichQuest quest
---@param title string
function QuestSetTitle (whichQuest, title) end
---@param whichQuest quest
---@param description string
function QuestSetDescription (whichQuest, description) end
---@param whichQuest quest
---@param iconPath string
function QuestSetIconPath (whichQuest, iconPath) end

---@param whichQuest quest
---@param required boolean
function QuestSetRequired (whichQuest, required) end
---@param whichQuest quest
---@param completed boolean
function QuestSetCompleted (whichQuest, completed) end
---@param whichQuest quest
---@param discovered boolean
function QuestSetDiscovered (whichQuest, discovered) end
---@param whichQuest quest
---@param failed boolean
function QuestSetFailed (whichQuest, failed) end
---@param whichQuest quest
---@param enabled boolean
function QuestSetEnabled (whichQuest, enabled) end

---@param whichQuest quest
---@return boolean
function IsQuestRequired (whichQuest) end
---@param whichQuest quest
---@return boolean
function IsQuestCompleted (whichQuest) end
---@param whichQuest quest
---@return boolean
function IsQuestDiscovered (whichQuest) end
---@param whichQuest quest
---@return boolean
function IsQuestFailed (whichQuest) end
---@param whichQuest quest
---@return boolean
function IsQuestEnabled (whichQuest) end

---@param whichQuest quest
---@return questitem
function QuestCreateItem (whichQuest) end
---@param whichQuestItem questitem
---@param description string
function QuestItemSetDescription (whichQuestItem, description) end
---@param whichQuestItem questitem
---@param completed boolean
function QuestItemSetCompleted (whichQuestItem, completed) end

---@param whichQuestItem questitem
---@return boolean
function IsQuestItemCompleted (whichQuestItem) end

---@return defeatcondition
function CreateDefeatCondition () end
---@param whichCondition defeatcondition
function DestroyDefeatCondition (whichCondition) end
---@param whichCondition defeatcondition
---@param description string
function DefeatConditionSetDescription (whichCondition, description) end

function FlashQuestDialogButton () end
function ForceQuestDialogUpdate () end

-- ============================================================================
-- Timer Dialog API
---@param whichTimer timer
---@return timerdialog
function CreateTimerDialog (whichTimer) end
---@param whichDialog timerdialog
function DestroyTimerDialog (whichDialog) end
---@param whichDialog timerdialog
---@param title string
function TimerDialogSetTitle (whichDialog, title) end
---@param whichDialog timerdialog
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function TimerDialogSetTitleColor (whichDialog, red, green, blue, alpha) end
---@param whichDialog timerdialog
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function TimerDialogSetTimeColor (whichDialog, red, green, blue, alpha) end
---@param whichDialog timerdialog
---@param speedMultFactor real
function TimerDialogSetSpeed (whichDialog, speedMultFactor) end
---@param whichDialog timerdialog
---@param display boolean
function TimerDialogDisplay (whichDialog, display) end
---@param whichDialog timerdialog
---@return boolean
function IsTimerDialogDisplayed (whichDialog) end
---@param whichDialog timerdialog
---@param timeRemaining real
function TimerDialogSetRealTimeRemaining (whichDialog, timeRemaining) end

-- ============================================================================
-- Leaderboard API

-- Create a leaderboard object
---@return leaderboard
function CreateLeaderboard () end
---@param whichLeaderboard leaderboard
function DestroyLeaderboard (whichLeaderboard) end

---@param whichLeaderboard leaderboard
---@param show boolean
function LeaderboardDisplay (whichLeaderboard, show) end
---@param whichLeaderboard leaderboard
---@return boolean
function IsLeaderboardDisplayed (whichLeaderboard) end

---@param whichLeaderboard leaderboard
---@return integer
function LeaderboardGetItemCount (whichLeaderboard) end

---@param whichLeaderboard leaderboard
---@param count integer
function LeaderboardSetSizeByItemCount (whichLeaderboard, count) end
---@param whichLeaderboard leaderboard
---@param label string
---@param value integer
---@param whichPlayer player
function LeaderboardAddItem (whichLeaderboard, label, value, whichPlayer) end
---@param whichLeaderboard leaderboard
---@param index integer
function LeaderboardRemoveItem (whichLeaderboard, index) end
---@param whichLeaderboard leaderboard
---@param whichPlayer player
function LeaderboardRemovePlayerItem (whichLeaderboard, whichPlayer) end
---@param whichLeaderboard leaderboard
function LeaderboardClear (whichLeaderboard) end

---@param whichLeaderboard leaderboard
---@param ascending boolean
function LeaderboardSortItemsByValue (whichLeaderboard, ascending) end
---@param whichLeaderboard leaderboard
---@param ascending boolean
function LeaderboardSortItemsByPlayer (whichLeaderboard, ascending) end
---@param whichLeaderboard leaderboard
---@param ascending boolean
function LeaderboardSortItemsByLabel (whichLeaderboard, ascending) end

---@param whichLeaderboard leaderboard
---@param whichPlayer player
---@return boolean
function LeaderboardHasPlayerItem (whichLeaderboard, whichPlayer) end
---@param whichLeaderboard leaderboard
---@param whichPlayer player
---@return integer
function LeaderboardGetPlayerIndex (whichLeaderboard, whichPlayer) end
---@param whichLeaderboard leaderboard
---@param label string
function LeaderboardSetLabel (whichLeaderboard, label) end
---@param whichLeaderboard leaderboard
---@return string
function LeaderboardGetLabelText (whichLeaderboard) end

---@param toPlayer player
---@param whichLeaderboard leaderboard
function PlayerSetLeaderboard (toPlayer, whichLeaderboard) end
---@param toPlayer player
---@return leaderboard
function PlayerGetLeaderboard (toPlayer) end

---@param whichLeaderboard leaderboard
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function LeaderboardSetLabelColor (whichLeaderboard, red, green, blue, alpha) end
---@param whichLeaderboard leaderboard
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function LeaderboardSetValueColor (whichLeaderboard, red, green, blue, alpha) end
---@param whichLeaderboard leaderboard
---@param showLabel boolean
---@param showNames boolean
---@param showValues boolean
---@param showIcons boolean
function LeaderboardSetStyle (whichLeaderboard, showLabel, showNames, showValues, showIcons) end

---@param whichLeaderboard leaderboard
---@param whichItem integer
---@param val integer
function LeaderboardSetItemValue (whichLeaderboard, whichItem, val) end
---@param whichLeaderboard leaderboard
---@param whichItem integer
---@param val string
function LeaderboardSetItemLabel (whichLeaderboard, whichItem, val) end
---@param whichLeaderboard leaderboard
---@param whichItem integer
---@param showLabel boolean
---@param showValue boolean
---@param showIcon boolean
function LeaderboardSetItemStyle (whichLeaderboard, whichItem, showLabel, showValue, showIcon) end
---@param whichLeaderboard leaderboard
---@param whichItem integer
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function LeaderboardSetItemLabelColor (whichLeaderboard, whichItem, red, green, blue, alpha) end
---@param whichLeaderboard leaderboard
---@param whichItem integer
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function LeaderboardSetItemValueColor (whichLeaderboard, whichItem, red, green, blue, alpha) end

-- ============================================================================
-- Multiboard API
-- ============================================================================

-- Create a multiboard object
---@return multiboard
function CreateMultiboard () end
---@param whichMultiboard multiboard
function DestroyMultiboard (whichMultiboard) end

---@param whichMultiboard multiboard
---@param show boolean
function MultiboardDisplay (whichMultiboard, show) end
---@param whichMultiboard multiboard
---@return boolean
function IsMultiboardDisplayed (whichMultiboard) end

---@param whichMultiboard multiboard
---@param minimize boolean
function MultiboardMinimize (whichMultiboard, minimize) end
---@param whichMultiboard multiboard
---@return boolean
function IsMultiboardMinimized (whichMultiboard) end
---@param whichMultiboard multiboard
function MultiboardClear (whichMultiboard) end

---@param whichMultiboard multiboard
---@param label string
function MultiboardSetTitleText (whichMultiboard, label) end
---@param whichMultiboard multiboard
---@return string
function MultiboardGetTitleText (whichMultiboard) end
---@param whichMultiboard multiboard
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function MultiboardSetTitleTextColor (whichMultiboard, red, green, blue, alpha) end

---@param whichMultiboard multiboard
---@return integer
function MultiboardGetRowCount (whichMultiboard) end
---@param whichMultiboard multiboard
---@return integer
function MultiboardGetColumnCount (whichMultiboard) end

---@param whichMultiboard multiboard
---@param count integer
function MultiboardSetColumnCount (whichMultiboard, count) end
---@param whichMultiboard multiboard
---@param count integer
function MultiboardSetRowCount (whichMultiboard, count) end

-- broadcast settings to all items
---@param whichMultiboard multiboard
---@param showValues boolean
---@param showIcons boolean
function MultiboardSetItemsStyle (whichMultiboard, showValues, showIcons) end
---@param whichMultiboard multiboard
---@param value string
function MultiboardSetItemsValue (whichMultiboard, value) end
---@param whichMultiboard multiboard
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function MultiboardSetItemsValueColor (whichMultiboard, red, green, blue, alpha) end
---@param whichMultiboard multiboard
---@param width real
function MultiboardSetItemsWidth (whichMultiboard, width) end
---@param whichMultiboard multiboard
---@param iconPath string
function MultiboardSetItemsIcon (whichMultiboard, iconPath) end


-- funcs for modifying individual items
---@param whichMultiboard multiboard
---@param row integer
---@param column integer
---@return multiboarditem
function MultiboardGetItem (whichMultiboard, row, column) end
---@param whichMultiboardItem multiboarditem
function MultiboardReleaseItem (whichMultiboardItem) end

---@param whichMultiboardItem multiboarditem
---@param showValue boolean
---@param showIcon boolean
function MultiboardSetItemStyle (whichMultiboardItem, showValue, showIcon) end
---@param whichMultiboardItem multiboarditem
---@param val string
function MultiboardSetItemValue (whichMultiboardItem, val) end
---@param whichMultiboardItem multiboarditem
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function MultiboardSetItemValueColor (whichMultiboardItem, red, green, blue, alpha) end
---@param whichMultiboardItem multiboarditem
---@param width real
function MultiboardSetItemWidth (whichMultiboardItem, width) end
---@param whichMultiboardItem multiboarditem
---@param iconFileName string
function MultiboardSetItemIcon (whichMultiboardItem, iconFileName) end

-- meant to unequivocally suspend display of existing and
-- subsequently displayed multiboards
-- 
---@param flag boolean
function MultiboardSuppressDisplay (flag) end

-- ============================================================================
-- Camera API
---@param x real
---@param y real
function SetCameraPosition (x, y) end
---@param x real
---@param y real
function SetCameraQuickPosition (x, y) end
---@param x1 real
---@param y1 real
---@param x2 real
---@param y2 real
---@param x3 real
---@param y3 real
---@param x4 real
---@param y4 real
function SetCameraBounds (x1, y1, x2, y2, x3, y3, x4, y4) end
function StopCamera () end
---@param duration real
function ResetToGameCamera (duration) end
---@param x real
---@param y real
function PanCameraTo (x, y) end
---@param x real
---@param y real
---@param duration real
function PanCameraToTimed (x, y, duration) end
---@param x real
---@param y real
---@param zOffsetDest real
function PanCameraToWithZ (x, y, zOffsetDest) end
---@param x real
---@param y real
---@param zOffsetDest real
---@param duration real
function PanCameraToTimedWithZ (x, y, zOffsetDest, duration) end
---@param cameraModelFile string
function SetCinematicCamera (cameraModelFile) end
---@param x real
---@param y real
---@param radiansToSweep real
---@param duration real
function SetCameraRotateMode (x, y, radiansToSweep, duration) end
---@param whichField camerafield
---@param value real
---@param duration real
function SetCameraField (whichField, value, duration) end
---@param whichField camerafield
---@param offset real
---@param duration real
function AdjustCameraField (whichField, offset, duration) end
---@param whichUnit unit
---@param xoffset real
---@param yoffset real
---@param inheritOrientation boolean
function SetCameraTargetController (whichUnit, xoffset, yoffset, inheritOrientation) end
---@param whichUnit unit
---@param xoffset real
---@param yoffset real
function SetCameraOrientController (whichUnit, xoffset, yoffset) end

---@return camerasetup
function CreateCameraSetup () end
---@param whichCameraSetup camerasetup
---@param whichField camerafield
---@param value real
---@param duration real
function CameraSetupSetField (whichCameraSetup, whichField, value, duration) end
---@param whichCameraSetup camerasetup
---@param whichField camerafield
---@return real
function CameraSetupGetField (whichCameraSetup, whichField) end
---@param whichCameraSetup camerasetup
---@param x real
---@param y real
---@param duration real
function CameraSetupSetDestPosition (whichCameraSetup, x, y, duration) end
---@param whichCameraSetup camerasetup
---@return location
function CameraSetupGetDestPositionLoc (whichCameraSetup) end
---@param whichCameraSetup camerasetup
---@return real
function CameraSetupGetDestPositionX (whichCameraSetup) end
---@param whichCameraSetup camerasetup
---@return real
function CameraSetupGetDestPositionY (whichCameraSetup) end
---@param whichCameraSetup camerasetup
---@param doPan boolean
---@param panTimed boolean
function CameraSetupApply (whichCameraSetup, doPan, panTimed) end
---@param whichCameraSetup camerasetup
---@param zDestOffset real
function CameraSetupApplyWithZ (whichCameraSetup, zDestOffset) end
---@param whichCameraSetup camerasetup
---@param doPan boolean
---@param forceDuration real
function CameraSetupApplyForceDuration (whichCameraSetup, doPan, forceDuration) end
---@param whichCameraSetup camerasetup
---@param zDestOffset real
---@param forceDuration real
function CameraSetupApplyForceDurationWithZ (whichCameraSetup, zDestOffset, forceDuration) end

---@param mag real
---@param velocity real
function CameraSetTargetNoise (mag, velocity) end
---@param mag real
---@param velocity real
function CameraSetSourceNoise (mag, velocity) end

---@param mag real
---@param velocity real
---@param vertOnly boolean
function CameraSetTargetNoiseEx (mag, velocity, vertOnly) end
---@param mag real
---@param velocity real
---@param vertOnly boolean
function CameraSetSourceNoiseEx (mag, velocity, vertOnly) end

---@param factor real
function CameraSetSmoothingFactor (factor) end

---@param filename string
function SetCineFilterTexture (filename) end
---@param whichMode blendmode
function SetCineFilterBlendMode (whichMode) end
---@param whichFlags texmapflags
function SetCineFilterTexMapFlags (whichFlags) end
---@param minu real
---@param minv real
---@param maxu real
---@param maxv real
function SetCineFilterStartUV (minu, minv, maxu, maxv) end
---@param minu real
---@param minv real
---@param maxu real
---@param maxv real
function SetCineFilterEndUV (minu, minv, maxu, maxv) end
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function SetCineFilterStartColor (red, green, blue, alpha) end
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function SetCineFilterEndColor (red, green, blue, alpha) end
---@param duration real
function SetCineFilterDuration (duration) end
---@param flag boolean
function DisplayCineFilter (flag) end
---@return boolean
function IsCineFilterDisplayed () end

---@param unitTypeId integer
---@param color playercolor
---@param speakerTitle string
---@param text string
---@param sceneDuration real
---@param voiceoverDuration real
function SetCinematicScene (unitTypeId, color, speakerTitle, text, sceneDuration, voiceoverDuration) end
function EndCinematicScene () end
---@param flag boolean
function ForceCinematicSubtitles (flag) end

---@param whichMargin integer
---@return real
function GetCameraMargin (whichMargin) end

-- These return values for the local players camera only...
---@return real
function GetCameraBoundMinX () end
---@return real
function GetCameraBoundMinY () end
---@return real
function GetCameraBoundMaxX () end
---@return real
function GetCameraBoundMaxY () end
---@param whichField camerafield
---@return real
function GetCameraField (whichField) end
---@return real
function GetCameraTargetPositionX () end
---@return real
function GetCameraTargetPositionY () end
---@return real
function GetCameraTargetPositionZ () end
---@return location
function GetCameraTargetPositionLoc () end
---@return real
function GetCameraEyePositionX () end
---@return real
function GetCameraEyePositionY () end
---@return real
function GetCameraEyePositionZ () end
---@return location
function GetCameraEyePositionLoc () end

-- ============================================================================
-- Sound API
-- 
---@param environmentName string
function NewSoundEnvironment (environmentName) end

---@param fileName string
---@param looping boolean
---@param is3D boolean
---@param stopwhenoutofrange boolean
---@param fadeInRate integer
---@param fadeOutRate integer
---@param eaxSetting string
---@return sound
function CreateSound (fileName, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate, eaxSetting) end
---@param fileName string
---@param looping boolean
---@param is3D boolean
---@param stopwhenoutofrange boolean
---@param fadeInRate integer
---@param fadeOutRate integer
---@param SLKEntryName string
---@return sound
function CreateSoundFilenameWithLabel (fileName, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate, SLKEntryName) end
---@param soundLabel string
---@param looping boolean
---@param is3D boolean
---@param stopwhenoutofrange boolean
---@param fadeInRate integer
---@param fadeOutRate integer
---@return sound
function CreateSoundFromLabel (soundLabel, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate) end
---@param soundLabel string
---@param fadeInRate integer
---@param fadeOutRate integer
---@return sound
function CreateMIDISound (soundLabel, fadeInRate, fadeOutRate) end

---@param soundHandle sound
---@param soundLabel string
function SetSoundParamsFromLabel (soundHandle, soundLabel) end
---@param soundHandle sound
---@param cutoff real
function SetSoundDistanceCutoff (soundHandle, cutoff) end
---@param soundHandle sound
---@param channel integer
function SetSoundChannel (soundHandle, channel) end
---@param soundHandle sound
---@param volume integer
function SetSoundVolume (soundHandle, volume) end
---@param soundHandle sound
---@param pitch real
function SetSoundPitch (soundHandle, pitch) end

-- the following method must be called immediately after calling "StartSound"
---@param soundHandle sound
---@param millisecs integer
function SetSoundPlayPosition (soundHandle, millisecs) end

-- these calls are only valid if the sound was created with 3d enabled
---@param soundHandle sound
---@param minDist real
---@param maxDist real
function SetSoundDistances (soundHandle, minDist, maxDist) end
---@param soundHandle sound
---@param inside real
---@param outside real
---@param outsideVolume integer
function SetSoundConeAngles (soundHandle, inside, outside, outsideVolume) end
---@param soundHandle sound
---@param x real
---@param y real
---@param z real
function SetSoundConeOrientation (soundHandle, x, y, z) end
---@param soundHandle sound
---@param x real
---@param y real
---@param z real
function SetSoundPosition (soundHandle, x, y, z) end
---@param soundHandle sound
---@param x real
---@param y real
---@param z real
function SetSoundVelocity (soundHandle, x, y, z) end
---@param soundHandle sound
---@param whichUnit unit
function AttachSoundToUnit (soundHandle, whichUnit) end

---@param soundHandle sound
function StartSound (soundHandle) end
---@param soundHandle sound
---@param killWhenDone boolean
---@param fadeOut boolean
function StopSound (soundHandle, killWhenDone, fadeOut) end
---@param soundHandle sound
function KillSoundWhenDone (soundHandle) end

-- Music Interface. Note that if music is disabled, these calls do nothing
---@param musicName string
---@param random boolean
---@param index integer
function SetMapMusic (musicName, random, index) end
function ClearMapMusic () end

---@param musicName string
function PlayMusic (musicName) end
---@param musicName string
---@param frommsecs integer
---@param fadeinmsecs integer
function PlayMusicEx (musicName, frommsecs, fadeinmsecs) end
---@param fadeOut boolean
function StopMusic (fadeOut) end
function ResumeMusic () end

---@param musicFileName string
function PlayThematicMusic (musicFileName) end
---@param musicFileName string
---@param frommsecs integer
function PlayThematicMusicEx (musicFileName, frommsecs) end
function EndThematicMusic () end

---@param volume integer
function SetMusicVolume (volume) end
---@param millisecs integer
function SetMusicPlayPosition (millisecs) end
---@param millisecs integer
function SetThematicMusicPlayPosition (millisecs) end

-- other music and sound calls
---@param soundHandle sound
---@param duration integer
function SetSoundDuration (soundHandle, duration) end
---@param soundHandle sound
---@return integer
function GetSoundDuration (soundHandle) end
---@param musicFileName string
---@return integer
function GetSoundFileDuration (musicFileName) end

---@param vgroup volumegroup
---@param scale real
function VolumeGroupSetVolume (vgroup, scale) end
function VolumeGroupReset () end

---@param soundHandle sound
---@return boolean
function GetSoundIsPlaying (soundHandle) end
---@param soundHandle sound
---@return boolean
function GetSoundIsLoading (soundHandle) end

---@param soundHandle sound
---@param byPosition boolean
---@param rectwidth real
---@param rectheight real
function RegisterStackedSound (soundHandle, byPosition, rectwidth, rectheight) end
---@param soundHandle sound
---@param byPosition boolean
---@param rectwidth real
---@param rectheight real
function UnregisterStackedSound (soundHandle, byPosition, rectwidth, rectheight) end

-- ============================================================================
-- Effects API
-- 
---@param whichRect rect
---@param effectID integer
---@return weathereffect
function AddWeatherEffect (whichRect, effectID) end
---@param whichEffect weathereffect
function RemoveWeatherEffect (whichEffect) end
---@param whichEffect weathereffect
---@param enable boolean
function EnableWeatherEffect (whichEffect, enable) end

---@param x real
---@param y real
---@param radius real
---@param depth real
---@param duration integer
---@param permanent boolean
---@return terraindeformation
function TerrainDeformCrater (x, y, radius, depth, duration, permanent) end
---@param x real
---@param y real
---@param radius real
---@param depth real
---@param duration integer
---@param count integer
---@param spaceWaves real
---@param timeWaves real
---@param radiusStartPct real
---@param limitNeg boolean
---@return terraindeformation
function TerrainDeformRipple (x, y, radius, depth, duration, count, spaceWaves, timeWaves, radiusStartPct, limitNeg) end
---@param x real
---@param y real
---@param dirX real
---@param dirY real
---@param distance real
---@param speed real
---@param radius real
---@param depth real
---@param trailTime integer
---@param count integer
---@return terraindeformation
function TerrainDeformWave (x, y, dirX, dirY, distance, speed, radius, depth, trailTime, count) end
---@param x real
---@param y real
---@param radius real
---@param minDelta real
---@param maxDelta real
---@param duration integer
---@param updateInterval integer
---@return terraindeformation
function TerrainDeformRandom (x, y, radius, minDelta, maxDelta, duration, updateInterval) end
---@param deformation terraindeformation
---@param duration integer
function TerrainDeformStop (deformation, duration) end
function TerrainDeformStopAll () end

---@param modelName string
---@param x real
---@param y real
---@return effect
function AddSpecialEffect (modelName, x, y) end
---@param modelName string
---@param where location
---@return effect
function AddSpecialEffectLoc (modelName, where) end
---@param modelName string
---@param targetWidget widget
---@param attachPointName string
---@return effect
function AddSpecialEffectTarget (modelName, targetWidget, attachPointName) end
---@param whichEffect effect
function DestroyEffect (whichEffect) end

---@param abilityString string
---@param whichEffectType effecttype
---@param x real
---@param y real
---@return effect
function AddSpellEffect (abilityString, whichEffectType, x, y) end
---@param abilityString string
---@param whichEffectType effecttype
---@param where location
---@return effect
function AddSpellEffectLoc (abilityString, whichEffectType, where) end
---@param abilityTypeId integer
---@param whichEffectType effecttype
---@param x real
---@param y real
---@return effect
function AddSpellEffectById (abilityTypeId, whichEffectType, x, y) end
---@param abilityTypeId integer
---@param whichEffectType effecttype
---@param where location
---@return effect
function AddSpellEffectByIdLoc (abilityTypeId, whichEffectType, where) end
---@param modelName string
---@param whichEffectType effecttype
---@param targetWidget widget
---@param attachPoint string
---@return effect
function AddSpellEffectTarget (modelName, whichEffectType, targetWidget, attachPoint) end
---@param abilityTypeId integer
---@param whichEffectType effecttype
---@param targetWidget widget
---@param attachPoint string
---@return effect
function AddSpellEffectTargetById (abilityTypeId, whichEffectType, targetWidget, attachPoint) end

---@param codeName string
---@param checkVisibility boolean
---@param x1 real
---@param y1 real
---@param x2 real
---@param y2 real
---@return lightning
function AddLightning (codeName, checkVisibility, x1, y1, x2, y2) end
---@param codeName string
---@param checkVisibility boolean
---@param x1 real
---@param y1 real
---@param z1 real
---@param x2 real
---@param y2 real
---@param z2 real
---@return lightning
function AddLightningEx (codeName, checkVisibility, x1, y1, z1, x2, y2, z2) end
---@param whichBolt lightning
---@return boolean
function DestroyLightning (whichBolt) end
---@param whichBolt lightning
---@param checkVisibility boolean
---@param x1 real
---@param y1 real
---@param x2 real
---@param y2 real
---@return boolean
function MoveLightning (whichBolt, checkVisibility, x1, y1, x2, y2) end
---@param whichBolt lightning
---@param checkVisibility boolean
---@param x1 real
---@param y1 real
---@param z1 real
---@param x2 real
---@param y2 real
---@param z2 real
---@return boolean
function MoveLightningEx (whichBolt, checkVisibility, x1, y1, z1, x2, y2, z2) end
---@param whichBolt lightning
---@return real
function GetLightningColorA (whichBolt) end
---@param whichBolt lightning
---@return real
function GetLightningColorR (whichBolt) end
---@param whichBolt lightning
---@return real
function GetLightningColorG (whichBolt) end
---@param whichBolt lightning
---@return real
function GetLightningColorB (whichBolt) end
---@param whichBolt lightning
---@param r real
---@param g real
---@param b real
---@param a real
---@return boolean
function SetLightningColor (whichBolt, r, g, b, a) end

---@param abilityString string
---@param whichEffectType effecttype
---@param index integer
---@return string
function GetAbilityEffect (abilityString, whichEffectType, index) end
---@param abilityTypeId integer
---@param whichEffectType effecttype
---@param index integer
---@return string
function GetAbilityEffectById (abilityTypeId, whichEffectType, index) end
---@param abilityString string
---@param whichSoundType soundtype
---@return string
function GetAbilitySound (abilityString, whichSoundType) end
---@param abilityTypeId integer
---@param whichSoundType soundtype
---@return string
function GetAbilitySoundById (abilityTypeId, whichSoundType) end

-- ============================================================================
-- Terrain API
-- 
---@param x real
---@param y real
---@return integer
function GetTerrainCliffLevel (x, y) end
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function SetWaterBaseColor (red, green, blue, alpha) end
---@param val boolean
function SetWaterDeforms (val) end
---@param x real
---@param y real
---@return integer
function GetTerrainType (x, y) end
---@param x real
---@param y real
---@return integer
function GetTerrainVariance (x, y) end
---@param x real
---@param y real
---@param terrainType integer
---@param variation integer
---@param area integer
---@param shape integer
function SetTerrainType (x, y, terrainType, variation, area, shape) end
---@param x real
---@param y real
---@param whichPathingType pathingtype
---@return boolean
function IsTerrainPathable (x, y, whichPathingType) end
---@param x real
---@param y real
---@param whichPathingType pathingtype
---@param flag boolean
function SetTerrainPathable (x, y, whichPathingType, flag) end

-- ============================================================================
-- Image API
-- 
---@param file string
---@param sizeX real
---@param sizeY real
---@param sizeZ real
---@param posX real
---@param posY real
---@param posZ real
---@param originX real
---@param originY real
---@param originZ real
---@param imageType integer
---@return image
function CreateImage (file, sizeX, sizeY, sizeZ, posX, posY, posZ, originX, originY, originZ, imageType) end
---@param whichImage image
function DestroyImage (whichImage) end
---@param whichImage image
---@param flag boolean
function ShowImage (whichImage, flag) end
---@param whichImage image
---@param flag boolean
---@param height real
function SetImageConstantHeight (whichImage, flag, height) end
---@param whichImage image
---@param x real
---@param y real
---@param z real
function SetImagePosition (whichImage, x, y, z) end
---@param whichImage image
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function SetImageColor (whichImage, red, green, blue, alpha) end
---@param whichImage image
---@param flag boolean
function SetImageRender (whichImage, flag) end
---@param whichImage image
---@param flag boolean
function SetImageRenderAlways (whichImage, flag) end
---@param whichImage image
---@param flag boolean
---@param useWaterAlpha boolean
function SetImageAboveWater (whichImage, flag, useWaterAlpha) end
---@param whichImage image
---@param imageType integer
function SetImageType (whichImage, imageType) end

-- ============================================================================
-- Ubersplat API
-- 
---@param x real
---@param y real
---@param name string
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
---@param forcePaused boolean
---@param noBirthTime boolean
---@return ubersplat
function CreateUbersplat (x, y, name, red, green, blue, alpha, forcePaused, noBirthTime) end
---@param whichSplat ubersplat
function DestroyUbersplat (whichSplat) end
---@param whichSplat ubersplat
function ResetUbersplat (whichSplat) end
---@param whichSplat ubersplat
function FinishUbersplat (whichSplat) end
---@param whichSplat ubersplat
---@param flag boolean
function ShowUbersplat (whichSplat, flag) end
---@param whichSplat ubersplat
---@param flag boolean
function SetUbersplatRender (whichSplat, flag) end
---@param whichSplat ubersplat
---@param flag boolean
function SetUbersplatRenderAlways (whichSplat, flag) end

-- ============================================================================
-- Blight API
-- 
---@param whichPlayer player
---@param x real
---@param y real
---@param radius real
---@param addBlight boolean
function SetBlight (whichPlayer, x, y, radius, addBlight) end
---@param whichPlayer player
---@param whichRect rect
---@param addBlight boolean
function SetBlightRect (whichPlayer, whichRect, addBlight) end
---@param whichPlayer player
---@param x real
---@param y real
---@param addBlight boolean
function SetBlightPoint (whichPlayer, x, y, addBlight) end
---@param whichPlayer player
---@param whichLocation location
---@param radius real
---@param addBlight boolean
function SetBlightLoc (whichPlayer, whichLocation, radius, addBlight) end
---@param whichPlayer player
---@param x real
---@param y real
---@param facing real
---@return unit
function CreateBlightedGoldmine (whichPlayer, x, y, facing) end
---@param x real
---@param y real
---@return boolean
function IsPointBlighted (x, y) end

-- ============================================================================
-- Doodad API
-- 
---@param x real
---@param y real
---@param radius real
---@param doodadID integer
---@param nearestOnly boolean
---@param animName string
---@param animRandom boolean
function SetDoodadAnimation (x, y, radius, doodadID, nearestOnly, animName, animRandom) end
---@param whichRect rect
---@param doodadID integer
---@param animName string
---@param animRandom boolean
function SetDoodadAnimationRect (whichRect, doodadID, animName, animRandom) end

-- ============================================================================
-- Computer AI interface
-- 
---@param whichPlayer player
---@param script string
function StartMeleeAI (whichPlayer, script) end
---@param whichPlayer player
---@param script string
function StartCampaignAI (whichPlayer, script) end
---@param whichPlayer player
---@param command integer
---@param data integer
function CommandAI (whichPlayer, command, data) end
---@param whichPlayer player
---@param pause boolean
function PauseCompAI (whichPlayer, pause) end
---@param whichPlayer player
---@return aidifficulty
function GetAIDifficulty (whichPlayer) end

---@param whichUnit unit
function RemoveGuardPosition (whichUnit) end
---@param whichUnit unit
function RecycleGuardPosition (whichUnit) end
---@param whichPlayer player
function RemoveAllGuardPositions (whichPlayer) end

-- ============================================================================
---@param cheatStr string
function Cheat (cheatStr) end
---@return boolean
function IsNoVictoryCheat () end
---@return boolean
function IsNoDefeatCheat () end

---@param filename string
function Preload (filename) end
---@param timeout real
function PreloadEnd (timeout) end

function PreloadStart () end
function PreloadRefresh () end
function PreloadEndEx () end

function PreloadGenClear () end
function PreloadGenStart () end
---@param filename string
function PreloadGenEnd (filename) end
---@param filename string
function Preloader (filename) end

-- ================Custom natives=====================

-- ===================================================
-- Conversion API
-- 

-- integers in jass use 4 bytes, that is 32 bits so you can do something like this: BitwiseGetBit( 0xFF001122, 31 ), this will return 1 (as 4th byte is 0xFF which is 11111111 in bits).
---@author UjAPI
---@param i integer
---@param bitIndex integer
---@return integer
function BitwiseGetBit (i, bitIndex) end
---@author UjAPI
---@param i integer
---@param bitIndex integer
---@param bitValue integer
---@return integer
function BitwiseSetBit (i, bitIndex, bitValue) end
-- integers in jass use 4 bytes, so you can do something like this: BitwiseGetByte( 0xFF001122, 3 ), this will return 0xFF and to get 0x22 you need to: BitwiseGetByte( 0xFF001122, 0 ).
---@author UjAPI
---@param i integer
---@param byteIndex integer
---@return integer
function BitwiseGetByte (i, byteIndex) end
---@author UjAPI
---@param i integer
---@param byteIndex integer
---@param byteValue integer
---@return integer
function BitwiseSetByte (i, byteIndex, byteValue) end

---@author UjAPI
---@param i integer
---@return integer
function BitwiseNOT (i) end
---@author UjAPI
---@param a integer
---@param b integer
---@return integer
function BitwiseAND (a, b) end
---@author UjAPI
---@param a integer
---@param b integer
---@return integer
function BitwiseOR (a, b) end
---@author UjAPI
---@param a integer
---@param b integer
---@return integer
function BitwiseXOR (a, b) end
---@author UjAPI
---@param i integer
---@param bitsToShift integer
---@return integer
function BitwiseShiftLeft (i, bitsToShift) end
---@author UjAPI
---@param i integer
---@param bitsToShift integer
---@return integer
function BitwiseShiftLeftLogical (i, bitsToShift) end
---@author UjAPI
---@param i integer
---@param bitsToShift integer
---@return integer
function BitwiseShiftRight (i, bitsToShift) end
---@author UjAPI
---@param i integer
---@param bitsToShift integer
---@return integer
function BitwiseShiftRightLogical (i, bitsToShift) end
---@author UjAPI
---@param byte1 integer
---@param byte2 integer
---@param byte3 integer
---@param byte4 integer
---@return integer
function BitwiseToInteger (byte1, byte2, byte3, byte4) end

---@author UjAPI
---@param i integer
---@return string
function Id2String (i) end
---@author UjAPI
---@param idString string
---@return integer
function String2Id (idString) end
---@author UjAPI
---@param i integer
---@return string
function IntToHex (i) end
---@author UjAPI
---@param i integer
---@return string
function IntToChar (i) end
---@author UjAPI
---@param i integer
---@return string
function IntToRoman (i) end
---@author UjAPI
---@param hex string
---@return integer
function HexToInt (hex) end
---@author UjAPI
---@param alpha integer
---@param red integer
---@param green integer
---@param blue integer
---@return integer
function ConvertColour (alpha, red, green, blue) end

-- ===================================================
-- Typecasting API Main
-- 
-- Something to inverse GetHandleId, meaning it will convert any given number to a "handle", for safety it's limited to HandleIds >= 0x100000 aka how game limits real handles from "fake".
---@author UjAPI
---@param i integer
---@return handle
function IntegerToHandle (i) end
-- Converts provided memory address and converts it into handle, this may only work with real handles, fakes most likely will cause fatal error.
---@author UjAPI
---@param i integer
---@return handle
function AddressToHandle (i) end
-- Converts given handle and returns its address in game, for safety it's limited to HandleIds >= 0x100000 aka how game limits real handles from "fake".
---@author UjAPI
---@param whichHandle handle
---@return integer
function HandleToAddress (whichHandle) end
-- 

-- ===================================================
-- Typecasting API Basic (all functions below are IntegerToHandle, since handle is pretty much an integer with start point of 0x100000)
-- 
---@author UjAPI
---@param i integer
---@return code
function I2C (i) end
---@author UjAPI
---@param c code
---@return integer
function C2I (c) end
---@author UjAPI
---@param whichHandle handle
---@return handle
function HandleToHandle (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return agent
function HandleToAgent (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return event
function HandleToEvent (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return widget
function HandleToWidget (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return unit
function HandleToUnit (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return destructable
function HandleToDestructable (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return item
function HandleToItem (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return ability
function HandleToAbility (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return buff
function HandleToBuff (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return force
function HandleToForce (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return group
function HandleToGroup (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return trigger
function HandleToTrigger (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return triggercondition
function HandleToTriggercondition (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return triggeraction
function HandleToTriggeraction (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return timer
function HandleToTimer (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return location
function HandleToLocation (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return region
function HandleToRegion (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return rect
function HandleToRect (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return boolexpr
function HandleToBoolexpr (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return sound
function HandleToSound (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return conditionfunc
function HandleToConditionfunc (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return filterfunc
function HandleToFilterfunc (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return unitpool
function HandleToUnitpool (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return itempool
function HandleToItempool (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return race
function HandleToRace (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return alliancetype
function HandleToAlliancetype (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return racepreference
function HandleToRacepreference (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return gamestate
function HandleToGamestate (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return igamestate
function HandleToIGamestate (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return fgamestate
function HandleToFGamestate (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return playerstate
function HandleToPlayerstate (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return playerscore
function HandleToPlayerscore (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return playergameresult
function HandleToPlayergameresult (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return unitstate
function HandleToUnitstate (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return aidifficulty
function HandleToAIDifficulty (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return eventid
function HandleToEventid (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return gameevent
function HandleToGameevent (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return playerevent
function HandleToPlayerevent (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return playerunitevent
function HandleToPlayerunitevent (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return unitevent
function HandleToUnitevent (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return limitop
function HandleToLimitop (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return widgetevent
function HandleToWidgetevent (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return dialogevent
function HandleToDialogevent (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return unittype
function HandleToUnittype (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return gamespeed
function HandleToGamespeed (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return gamedifficulty
function HandleToGamedifficulty (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return gametype
function HandleToGametype (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return mapflag
function HandleToMapflag (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return mapvisibility
function HandleToMapvisibility (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return mapsetting
function HandleToMapsetting (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return mapdensity
function HandleToMapdensity (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return mapcontrol
function HandleToMapcontrol (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return playerslotstate
function HandleToPlayerslotstate (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return volumegroup
function HandleToVolumegroup (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return camerafield
function HandleToCamerafield (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return camerasetup
function HandleToCamerasetup (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return playercolor
function HandleToPlayercolor (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return placement
function HandleToPlacement (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return startlocprio
function HandleToStartlocprio (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return raritycontrol
function HandleToRaritycontrol (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return blendmode
function HandleToBlendmode (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return texmapflags
function HandleToTexmapflags (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return effect
function HandleToEffect (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return effecttype
function HandleToEffecttype (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return weathereffect
function HandleToWeathereffect (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return terraindeformation
function HandleToTerraindeformation (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return fogstate
function HandleToFogstate (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return fogmodifier
function HandleToFogmodifier (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return dialog
function HandleToDialog (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return button
function HandleToButton (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return quest
function HandleToQuest (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return questitem
function HandleToQuestitem (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return defeatcondition
function HandleToDefeatcondition (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return timerdialog
function HandleToTimerdialog (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return leaderboard
function HandleToLeaderboard (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return multiboard
function HandleToMultiboard (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return multiboarditem
function HandleToMultiboarditem (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return trackable
function HandleToTrackable (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return gamecache
function HandleToGamecache (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return version
function HandleToVersion (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return itemtype
function HandleToItemtype (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return texttag
function HandleToTexttag (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return attacktype
function HandleToAttacktype (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return damagetype
function HandleToDamagetype (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return weapontype
function HandleToWeapontype (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return soundtype
function HandleToSoundtype (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return lightning
function HandleToLightning (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return pathingtype
function HandleToPathingtype (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return image
function HandleToImage (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return ubersplat
function HandleToUbersplat (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return hashtable
function HandleToHashtable (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return animtype
function HandleToAnimType (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return subanimtype
function HandleToSubAnimType (whichHandle) end
-- 

-- ============================================================================
-- Jass Data API
-- 
---@author UjAPI
---@return integer
function GetHandleCount () end
---@author UjAPI
---@return integer
function GetNextHandleIndex () end
---@author UjAPI
---@return integer
function GetStringCount () end
-- 

-- ============================================================================
-- Execution API
-- 

-- Jass VM API
---@author UjAPI
---@return jassthread
function GetJassMainThread () end
---@author UjAPI
---@return jassthread
function GetJassCurrentThread () end

-- RunScriptEx - this generates a new thread for any given .j file, which can be supplemented by "parent" script, "helper" (this is usually either common.ai or blizzard.j) and the script you want to run. Function returns id of newly created thread, returns 0 on failure.
---@author UjAPI
---@param parentScriptFile string
---@param helperScriptFile string
---@param jassScriptFile string
---@return jassthread
function RunJassScriptEx (parentScriptFile, helperScriptFile, jassScriptFile) end
---@author UjAPI
---@param helperScriptFile string
---@param jassScriptFile string
---@return jassthread
function RunJassScript (helperScriptFile, jassScriptFile) end
---@author UjAPI
---@param jassScriptFile string
---@return jassthread
function RunJassScriptSimple (jassScriptFile) end
-- 

---@author UjAPI
---@param thread jassthread
---@return boolean
function StopJassThread (thread) end

---@author UjAPI
---@param thread jassthread
---@param variableName string
---@return integer
function GetJassGlobalInteger (thread, variableName) end
---@author UjAPI
---@param thread jassthread
---@param variableName string
---@return real
function GetJassGlobalReal (thread, variableName) end
---@author UjAPI
---@param thread jassthread
---@param variableName string
---@return string
function GetJassGlobalString (thread, variableName) end
---@author UjAPI
---@param thread jassthread
---@param variableName string
---@return handle
function GetJassGlobalHandle (thread, variableName) end
---@author UjAPI
---@param thread jassthread
---@param variableName string
---@return boolean
function GetJassGlobalBoolean (thread, variableName) end

---@author UjAPI
---@param thread jassthread
---@param variableName string
---@param value integer
---@return boolean
function SetJassGlobalInteger (thread, variableName, value) end
---@author UjAPI
---@param thread jassthread
---@param variableName string
---@param value real
---@return boolean
function SetJassGlobalReal (thread, variableName, value) end
---@author UjAPI
---@param thread jassthread
---@param variableName string
---@param value string
---@return boolean
function SetJassGlobalString (thread, variableName, value) end
---@author UjAPI
---@param thread jassthread
---@param variableName string
---@param value handle
---@return boolean
function SetJassGlobalHandle (thread, variableName, value) end
---@author UjAPI
---@param thread jassthread
---@param variableName string
---@param value boolean
---@return boolean
function SetJassGlobalBoolean (thread, variableName, value) end

---@author UjAPI
---@param variableName string
---@return integer
function GetJassLocalInteger (variableName) end
---@author UjAPI
---@param variableName string
---@return real
function GetJassLocalReal (variableName) end
---@author UjAPI
---@param variableName string
---@return string
function GetJassLocalString (variableName) end
---@author UjAPI
---@param variableName string
---@return handle
function GetJassLocalHandle (variableName) end
---@author UjAPI
---@param variableName string
---@return boolean
function GetJassLocalBoolean (variableName) end

---@author UjAPI
---@param variableName string
---@param value integer
---@return boolean
function SetJassLocalInteger (variableName, value) end
---@author UjAPI
---@param variableName string
---@param value real
---@return boolean
function SetJassLocalReal (variableName, value) end
---@author UjAPI
---@param variableName string
---@param value string
---@return boolean
function SetJassLocalString (variableName, value) end
---@author UjAPI
---@param variableName string
---@param value handle
---@return boolean
function SetJassLocalHandle (variableName, value) end
---@author UjAPI
---@param variableName string
---@param value boolean
---@return boolean
function SetJassLocalBoolean (variableName, value) end
-- 

-- Jass Operations
---@author UjAPI
---@return boolean
function IsOperationLimitEnabled () end
---@author UjAPI
---@param enable boolean
function EnableOperationLimit (enable) end
---@author UjAPI
---@return integer
function GetOperationLimit () end
---@author UjAPI
---@param opLimit integer
function SetOperationLimit (opLimit) end
---@author UjAPI
---@param funcName string
---@return code
function GetCodeByName (funcName) end
---@author UjAPI
---@param c code
function ExecuteCode (c) end
---@author UjAPI
---@param funcName string
function ExecuteFuncEx (funcName) end
-- 

-- ============================================================================
-- Utility API
-- 

-- Math API
---@author UjAPI
---@param r real
---@return integer
function MathRound (r) end

---@author UjAPI
---@param r real
---@return real
function MathRealRound (r) end
---@author UjAPI
---@param r real
---@return real
function MathRealFloor (r) end
---@author UjAPI
---@param r real
---@return real
function MathRealCeil (r) end
---@author UjAPI
---@param r real
---@return real
function MathRealAbs (r) end
---@author UjAPI
---@param r real
---@return real
function MathRealLog (r) end
---@author UjAPI
---@param r real
---@return real
function MathRealLn (r) end
---@author UjAPI
---@param dividend real
---@param divisor real
---@return real
function MathRealModulo (dividend, divisor) end
---@author UjAPI
---@param a real
---@param b real
---@return real
function MathRealMin (a, b) end
---@author UjAPI
---@param a real
---@param b real
---@return real
function MathRealMax (a, b) end
---@author UjAPI
---@param r real
---@return integer
function MathRealSign (r) end
---@author UjAPI
---@param value real
---@param min real
---@param max real
---@return real
function MathRealClamp (value, min, max) end
---@author UjAPI
---@param a real
---@param b real
---@param t real
---@return real
function MathRealLerp (a, b, t) end

---@author UjAPI
---@param i integer
---@return integer
function MathIntegerAbs (i) end
---@author UjAPI
---@param i integer
---@return real
function MathIntegerLog (i) end
---@author UjAPI
---@param i integer
---@return real
function MathIntegerLn (i) end
---@author UjAPI
---@param dividend integer
---@param divisor integer
---@return integer
function MathIntegerModulo (dividend, divisor) end
---@author UjAPI
---@param a integer
---@param b integer
---@return integer
function MathIntegerMin (a, b) end
---@author UjAPI
---@param a integer
---@param b integer
---@return integer
function MathIntegerMax (a, b) end
---@author UjAPI
---@param i integer
---@return integer
function MathIntegerSign (i) end
---@author UjAPI
---@param value integer
---@param min integer
---@param max integer
---@return integer
function MathIntegerClamp (value, min, max) end

-- Angle / Axis / Point API | All natives operate in Degrees!

-- These natives act identical to basic Sin/Cos/Tan/etc. but they take degrees instead of radians, so you won't need to do Deg2Rad or Rad2Deg in multiple functions.
---@author UjAPI
---@param r real
---@return real
function MathSinDeg (r) end
---@author UjAPI
---@param r real
---@return real
function MathCosDeg (r) end
---@author UjAPI
---@param r real
---@return real
function MathTanDeg (r) end

---@author UjAPI
---@param x real
---@param angle real
---@param distance real
---@return real
function MathPointProjectionX (x, angle, distance) end
---@author UjAPI
---@param y real
---@param angle real
---@param distance real
---@return real
function MathPointProjectionY (y, angle, distance) end
---@author UjAPI
---@param fromX real
---@param fromY real
---@param toX real
---@param toY real
---@return real
function MathAngleBetweenPoints (fromX, fromY, toX, toY) end
---@author UjAPI
---@param fromX real
---@param fromY real
---@param toX real
---@param toY real
---@return real
function MathDistanceBetweenPoints (fromX, fromY, toX, toY) end
---@author UjAPI
---@param fromLoc location
---@param toLoc location
---@return real
function MathAngleBetweenLocations (fromLoc, toLoc) end
---@author UjAPI
---@param fromLoc location
---@param toLoc location
---@return real
function MathDistanceBetweenLocations (fromLoc, toLoc) end

---@author UjAPI
---@param x real
---@param y real
---@return real
function GetAxisZ (x, y) end
-- 

-- String API
---@author UjAPI
---@param s string
---@param whichString string
---@param caseSensitive boolean
---@return boolean
function StringContains (s, whichString, caseSensitive) end
---@author UjAPI
---@param s string
---@param whichString string
---@param caseSensitive boolean
---@return integer
function StringFind (s, whichString, caseSensitive) end
---@author UjAPI
---@param s string
---@param whichString string
---@param caseSensitive boolean
---@return integer
function StringFindFirstOf (s, whichString, caseSensitive) end
---@author UjAPI
---@param s string
---@param whichString string
---@param caseSensitive boolean
---@return integer
function StringFindFirstNotOf (s, whichString, caseSensitive) end
---@author UjAPI
---@param s string
---@param whichString string
---@param caseSensitive boolean
---@return integer
function StringFindLastOf (s, whichString, caseSensitive) end
---@author UjAPI
---@param s string
---@param whichString string
---@param caseSensitive boolean
---@return integer
function StringFindLastNotOf (s, whichString, caseSensitive) end
---@author UjAPI
---@param s string
---@param whichString string
---@param caseSensitive boolean
---@return integer
function StringCount (s, whichString, caseSensitive) end
---@author UjAPI
---@param s string
---@param caseSensitive boolean
---@return string
function StringTrimLeft (s, caseSensitive) end
---@author UjAPI
---@param s string
---@param caseSensitive boolean
---@return string
function StringTrimRight (s, caseSensitive) end
---@author UjAPI
---@param s string
---@param caseSensitive boolean
---@return string
function StringTrim (s, caseSensitive) end
---@author UjAPI
---@param s string
---@param caseSensitive boolean
---@return string
function StringReverse (s, caseSensitive) end
---@author UjAPI
---@param s string
---@param whichString string
---@param replaceWith string
---@param caseSensitive boolean
---@return string
function StringReplace (s, whichString, replaceWith, caseSensitive) end
---@author UjAPI
---@param s string
---@param whichString string
---@param whichPosition integer
---@param caseSensitive boolean
---@return string
function StringInsert (s, whichString, whichPosition, caseSensitive) end
-- 

-- Debug API
---@author UjAPI
---@param enable boolean
function ConsoleEnable (enable) end
---@author UjAPI
---@param s string
function ConsolePrint (s) end
---@author UjAPI
---@param s string
function ConsolePause (s) end
-- 

-- Text File API
---@author UjAPI
---@param filePath string
---@return textfilehandle
function TextFileOpen (filePath) end
---@author UjAPI
---@param filePath string
---@return boolean
function TextFileExists (filePath) end
---@author UjAPI
---@param filePath string
---@return integer
function TextFileGetSizeByPath (filePath) end
---@author UjAPI
---@param whichTextFile textfilehandle
---@return integer
function TextFileGetSize (whichTextFile) end
---@author UjAPI
---@param whichTextFile textfilehandle
---@return string
function TextFileGetPath (whichTextFile) end
---@author UjAPI
---@param whichTextFile textfilehandle
function TextFileClose (whichTextFile) end
---@author UjAPI
---@param whichTextFile textfilehandle
function TextFileClear (whichTextFile) end
---@author UjAPI
---@param whichTextFile textfilehandle
function TextFileDelete (whichTextFile) end
---@author UjAPI
---@param whichTextFile textfilehandle
---@return integer
function TextFileCountLines (whichTextFile) end
---@author UjAPI
---@param whichTextFile textfilehandle
---@param lineNumber integer
---@return string
function TextFileReadLine (whichTextFile, lineNumber) end
---@author UjAPI
---@param whichTextFile textfilehandle
---@return string
function TextFileReadAllLines (whichTextFile) end
---@author UjAPI
---@param whichTextFile textfilehandle
---@return string
function TextFileRead (whichTextFile) end
---@author UjAPI
---@param whichTextFile textfilehandle
---@param text string
function TextFileWriteLine (whichTextFile, text) end
-- 

-- Misc API
---@author UjAPI
---@return string
function GetUjAPIVersion () end
---@author UjAPI
---@return string
function GetGameVersion () end
---@author UjAPI
---@return string
function GetLocale () end

---@author UjAPI
---@param sectionName string
---@param optionName string
---@param index integer
---@return string
function GetMiscDataString (sectionName, optionName, index) end
---@author UjAPI
---@param sectionName string
---@param optionName string
---@param index integer
---@param value string
function SetMiscDataString (sectionName, optionName, index, value) end

-- raceName can be null/empty
---@author UjAPI
---@param raceName string
---@param sectionName string
---@param optionName string
---@param index integer
---@return string
function GetSkinDataString (raceName, sectionName, optionName, index) end
---@author UjAPI
---@param raceName string
---@param sectionName string
---@param optionName string
---@param index integer
---@param value string
function SetSkinDataString (raceName, sectionName, optionName, index, value) end

---@author UjAPI
---@param sectionName string
---@return string
function GetFDFDataString (sectionName) end
---@author UjAPI
---@param sectionName string
---@param value string
function SetFDFDataString (sectionName, value) end

---@author UjAPI
---@return integer
function GetWheelDelta () end
---@author UjAPI
---@return real
function GetFPS () end

---@author UjAPI
---@return real
function GetVisionMax () end
---@author UjAPI
---@param value real
function SetVisionMax (value) end
---@author UjAPI
---@return real
function GetAttackSpeedMinBonus () end
---@author UjAPI
---@param value real
function SetAttackSpeedMinBonus (value) end
---@author UjAPI
---@return real
function GetAttackSpeedMaxBonus () end
---@author UjAPI
---@param value real
function SetAttackSpeedMaxBonus (value) end
---@author UjAPI
---@return real
function GetMoveSpeedMinAllowed () end
---@author UjAPI
---@param value real
function SetMoveSpeedMinAllowed (value) end
---@author UjAPI
---@return real
function GetMoveSpeedMaxAllowed () end
---@author UjAPI
---@param value real
function SetMoveSpeedMaxAllowed (value) end

---@author UjAPI
---@param whichField mappedfield
---@param whichMapType mappedtype
---@return string
function GetMappedTypeName (whichField, whichMapType) end
-- 

-- UI API
---@author UjAPI
---@return boolean
function IsSelectionEnabled () end
---@author UjAPI
---@param enable boolean
function EnableSelection (enable) end
---@author UjAPI
---@return boolean
function IsSelectionCircleEnabled () end
---@author UjAPI
---@param enable boolean
function EnableSelectionCircle (enable) end
---@author UjAPI
---@return boolean
function IsTargetIndicatorEnabled () end
---@author UjAPI
---@param enable boolean
function EnableTargetIndicator (enable) end
---@author UjAPI
---@param renderStage renderstage
---@return boolean
function IsRenderStage (renderStage) end
---@author UjAPI
---@param renderStage renderstage
---@param enable boolean
function EnableRenderStage (renderStage, enable) end
---@author UjAPI
---@return boolean
function IsStatbarEnabled () end
---@author UjAPI
---@param enable boolean
function EnableStatbar (enable) end
-- 

-- Map API
---@author UjAPI
---@return real
function GetMapFogZ () end
---@author UjAPI
---@param z real
function SetMapFogZ (z) end
---@author UjAPI
---@return integer
function GetMapFogColour () end
---@author UjAPI
---@param colour integer
function SetMapFogColour (colour) end
-- 

-- Trigger API
---@author UjAPI
---@param whichTrigger trigger
---@return integer
function TriggerCountEvents (whichTrigger) end
---@author UjAPI
---@param whichTrigger trigger
---@return integer
function TriggerCountConditions (whichTrigger) end
---@author UjAPI
---@param whichTrigger trigger
---@return integer
function TriggerCountActions (whichTrigger) end

---@author UjAPI
---@param whichTrigger trigger
---@param whichEvent event
function TriggerRemoveEvent (whichTrigger, whichEvent) end
---@author UjAPI
---@param whichTrigger trigger
---@param whichEventId eventid
---@param removeAllOccurences boolean
function TriggerRemoveEventId (whichTrigger, whichEventId, removeAllOccurences) end
---@author UjAPI
---@param whichTrigger trigger
function TriggerClearEvents (whichTrigger) end

---@author UjAPI
---@param whichTrigger trigger
function TriggerClear (whichTrigger) end
-- 

-- Sound API
---@author UjAPI
---@param whichSound sound
function RemoveSound (whichSound) end

---@author UjAPI
---@param whichSound sound
---@param fileName string
---@param looping boolean
---@param is3D boolean
---@param stopwhenoutofrange boolean
---@param fadeInRate integer
---@param fadeOutRate integer
---@param eaxSetting string
---@return sound
function ReplaceSound (whichSound, fileName, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate, eaxSetting) end
---@author UjAPI
---@param whichSound sound
---@param fileName string
---@param looping boolean
---@param is3D boolean
---@param stopwhenoutofrange boolean
---@param fadeInRate integer
---@param fadeOutRate integer
---@param SLKEntryName string
---@return sound
function ReplaceSoundWithLabel (whichSound, fileName, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate, SLKEntryName) end
---@author UjAPI
---@param whichSound sound
---@param soundLabel string
---@param looping boolean
---@param is3D boolean
---@param stopwhenoutofrange boolean
---@param fadeInRate integer
---@param fadeOutRate integer
---@return sound
function ReplaceSoundFromLabel (whichSound, soundLabel, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate) end
---@author UjAPI
---@param whichSound sound
---@param soundLabel string
---@param fadeInRate integer
---@param fadeOutRate integer
---@return sound
function ReplaceMIDISound (whichSound, soundLabel, fadeInRate, fadeOutRate) end
-- 

-- Time API
---@author UjAPI
---@param whichTimeType timetype
---@return integer
function GetSystemTime (whichTimeType) end
---@author UjAPI
---@param whichTimeType timetype
---@return integer
function GetLocalTime (whichTimeType) end
---@author UjAPI
---@param isLocalTime boolean
---@param isMiliseconds integer
---@return string
function GetTimeStamp (isLocalTime, isMiliseconds) end
---@author UjAPI
---@return integer
function GetTickCount () end
-- 

-- Benchmark API
---@author UjAPI
function BenchmarkStart () end
---@author UjAPI
function BenchmarkEnd () end
---@author UjAPI
function BenchmarkReset () end
---@author UjAPI
---@param benchType integer
---@return string
function BenchmarkGetElapsed (benchType) end
-- 

-- Screen API
---@author UjAPI
---@param fov real
function SetScreenFieldOfView (fov) end
---@author UjAPI
---@param flag boolean
function SetWidescreenState (flag) end
-- 

-- Window API
---@author UjAPI
---@return boolean
function IsWindowActive () end
---@author UjAPI
---@return boolean
function IsWindowMode () end
---@author UjAPI
---@return integer
function GetWindowWidth () end
---@author UjAPI
---@return integer
function GetWindowHeight () end
---@author UjAPI
---@return integer
function GetClientWidth () end
---@author UjAPI
---@return integer
function GetClientHeight () end
---@author UjAPI
---@return integer
function GetWindowX () end
---@author UjAPI
---@return integer
function GetWindowY () end
---@author UjAPI
---@return integer
function GetWindowCenterX () end
---@author UjAPI
---@return integer
function GetWindowCenterY () end
-- 

-- Cursor API
---@author UjAPI
---@return boolean
function IsCursorEnabled () end
---@author UjAPI
---@param enable boolean
function SetCursorEnabled (enable) end

---@author UjAPI
---@return boolean
function IsCursorAnimationLocked () end
---@author UjAPI
---@param isLock boolean
function SetCursorAnimationLocked (isLock) end
-- 

-- Mouse API
---@author UjAPI
---@return real
function GetMouseScreenRelativeX () end
---@author UjAPI
---@return real
function GetMouseScreenRelativeY () end
---@author UjAPI
---@return real
function GetMouseScreenAbsoluteX () end
---@author UjAPI
---@return real
function GetMouseScreenAbsoluteY () end

---@author UjAPI
---@param x real
function SetMouseScreenRelativeX (x) end
---@author UjAPI
---@param y real
function SetMouseScreenRelativeY (y) end
---@author UjAPI
---@param x real
---@param y real
function SetMouseScreenRelativePosition (x, y) end
---@author UjAPI
---@param x real
function SetMouseScreenAbsoluteX (x) end
---@author UjAPI
---@param y real
function SetMouseScreenAbsoluteY (y) end
---@author UjAPI
---@param x real
---@param y real
function SetMouseScreenAbsolutePosition (x, y) end

---@author UjAPI
---@return integer
function GetMouseScreenPixelX () end
---@author UjAPI
---@return integer
function GetMouseScreenPixelY () end

---@author UjAPI
---@param x integer
function SetMouseScreenPixelX (x) end
---@author UjAPI
---@param y integer
function SetMouseScreenPixelY (y) end
---@author UjAPI
---@param x integer
---@param y integer
function SetMouseScreenPixelPosition (x, y) end

---@author UjAPI
---@return real
function GetMouseWorldX () end
---@author UjAPI
---@return real
function GetMouseWorldY () end
---@author UjAPI
---@return real
function GetMouseWorldZ () end
-- 

-- Chat API
---@author UjAPI
---@param toPlayer player
---@param message string
function DisplayWarningMessage (toPlayer, message) end
---@author UjAPI
---@param toPlayer player
---@param duration real
---@param message string
function DisplayTimedWarningMessage (toPlayer, duration, message) end
-- if whichPlayer is null, then players name text will be omitted, set recipient to CHAT_RECIPIENT_UNKNOWN to omit associated text.
---@author UjAPI
---@param whichPlayer player
---@param recipient integer
---@param duration real
---@param addToLog boolean
---@param message string
function DisplayChatMessageEx (whichPlayer, recipient, duration, addToLog, message) end
---@author UjAPI
---@param whichPlayer player
---@param recipient integer
---@param message string
function DisplayChatMessage (whichPlayer, recipient, message) end
---@author UjAPI
---@param whichPlayer player
---@param recipient integer
---@param duration real
---@param message string
function DisplayTimedChatMessage (whichPlayer, recipient, duration, message) end
-- 
---@author UjAPI
---@param toPlayer player
---@param message string
function DisplayTopMessage (toPlayer, message) end
---@author UjAPI
---@param toPlayer player
---@param duration real
---@param message string
function DisplayTimedTopMessage (toPlayer, duration, message) end
-- 

-- Handle API
-- This function is meant mostly for debugging, for example, to get all units in the map pass '+w3u' as agentBaseTypeId, '+ply' for players, '+mdb' for multiboards, '+frm' for frames accessed/created from jass/lua.
---@author UjAPI
---@param whichHandle handle
---@return boolean
function IsHandleDestroyed (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return integer
function GetHandleReferenceCount (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return integer
function GetHandleBaseTypeId (whichHandle) end
---@author UjAPI
---@param whichHandle handle
---@return string
function GetHandleBaseTypeName (whichHandle) end

---@author UjAPI
---@return handle
function GetFilterHandle () end
---@author UjAPI
---@return handle
function GetEnumHandle () end

---@author UjAPI
---@param handleBaseTypeId integer
---@param filter boolexpr
---@param handlerFunc code
function EnumHandlesOfType (handleBaseTypeId, filter, handlerFunc) end
-- 

---@author UjAPI
---@param whichQuestItem questitem
function DestroyQuestItem (whichQuestItem) end

-- AntiHack API
---@author UjAPI
---@param enable boolean
function AntiHackEnable (enable) end
---@author UjAPI
---@param enable boolean
function AntiHackEnableAddressCheck (enable) end
---@author UjAPI
---@param enable boolean
function AntiHackEnableBreakpointCheck (enable) end
---@author UjAPI
---@param enable boolean
function AntiHackEnableProcessCheck (enable) end
---@author UjAPI
---@param enable boolean
function AntiHackEnableModuleCheck (enable) end
---@author UjAPI
---@param enable boolean
function AntiHackEnableKick (enable) end

-- Trigger API

-- EVENT_GAME_HACK_DETECTED
---@author UjAPI
---@return integer
function GetTriggerHackId () end
---@author UjAPI
---@return integer
function GetTriggerHackType () end
---@author UjAPI
---@return integer
function GetTriggerHackLine () end
-- 

-- ============================================================================
-- Hashtable API
-- 
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichHandle handle
---@return boolean
function SaveHandle (whichHashtable, parentKey, childKey, whichHandle) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichCode code
---@return boolean
function SaveCode (whichHashtable, parentKey, childKey, whichCode) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichAttackType attacktype
---@return boolean
function SaveAttackTypeHandle (whichHashtable, parentKey, childKey, whichAttackType) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichDamageType damagetype
---@return boolean
function SaveDamageTypeHandle (whichHashtable, parentKey, childKey, whichDamageType) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichWeaponType weapontype
---@return boolean
function SaveWeaponTypeHandle (whichHashtable, parentKey, childKey, whichWeaponType) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichBuff buff
---@return boolean
function SaveBuffHandle (whichHashtable, parentKey, childKey, whichBuff) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichsprite sprite
---@return boolean
function SaveSpriteHandle (whichHashtable, parentKey, childKey, whichsprite) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichWar3Image war3image
---@return boolean
function SaveWar3ImageHandle (whichHashtable, parentKey, childKey, whichWar3Image) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichDoodad doodad
---@return boolean
function SaveDoodadHandle (whichHashtable, parentKey, childKey, whichDoodad) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichFile textfilehandle
---@return boolean
function SaveTextFileHandle (whichHashtable, parentKey, childKey, whichFile) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichProjectile projectile
---@return boolean
function SaveProjectileHandle (whichHashtable, parentKey, childKey, whichProjectile) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichFrame framehandle
---@return boolean
function SaveFrameHandle (whichHashtable, parentKey, childKey, whichFrame) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@param whichHandleList handlelist
---@return boolean
function SaveHandleList (whichHashtable, parentKey, childKey, whichHandleList) end

---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return handle
function LoadHandle (whichHashtable, parentKey, childKey) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return code
function LoadCode (whichHashtable, parentKey, childKey) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return attacktype
function LoadAttackTypeHandle (whichHashtable, parentKey, childKey) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return damagetype
function LoadDamageTypeHandle (whichHashtable, parentKey, childKey) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return weapontype
function LoadWeaponTypeHandle (whichHashtable, parentKey, childKey) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return buff
function LoadBuffHandle (whichHashtable, parentKey, childKey) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return sprite
function LoadSpriteHandle (whichHashtable, parentKey, childKey) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return war3image
function LoadWar3ImageHandle (whichHashtable, parentKey, childKey) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return doodad
function LoadDoodadHandle (whichHashtable, parentKey, childKey) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return textfilehandle
function LoadTextFileHandle (whichHashtable, parentKey, childKey) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return projectile
function LoadProjectileHandle (whichHashtable, parentKey, childKey) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return framehandle
function LoadFrameHandle (whichHashtable, parentKey, childKey) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
---@return handlelist
function LoadHandleList (whichHashtable, parentKey, childKey) end
-- 

-- ============================================================================
-- Player API
-- 
---@author UjAPI
---@return player
function GetHostPlayer () end
-- 

-- ============================================================================
-- Force API
-- 
---@author UjAPI
---@param whichForce force
---@param whichPlayer player
---@return boolean
function ForceHasPlayer (whichForce, whichPlayer) end
---@author UjAPI
---@param whichForce force
---@return integer
function ForceCountPlayers (whichForce) end
-- 

-- ============================================================================
-- Game API
-- 
---@author UjAPI
---@return connectiontype
function GetConnectionType () end
---@author UjAPI
---@return boolean
function IsReplay () end
-- 

-- ============================================================================
-- Fog Modifier API
-- 
---@author UjAPI
---@param whichFogModifier fogmodifier
---@return player
function GetFogModifierForPlayer (whichFogModifier) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@param whichPlayer player
function SetFogModifierForPlayer (whichFogModifier, whichPlayer) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@param whichState fogstate
---@return boolean
function IsFogModifierFogState (whichFogModifier, whichState) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@param whichState fogstate
---@param isSet boolean
function SetFogModifierFogState (whichFogModifier, whichState, isSet) end
-- For FOG_OF_WAR_RECT: GetX/Y returns CenterX/CenterY | GetRadius returns area of a rectangle: minX + maxX * minY + maxY.
-- For FOG_OF_WAR_RADIUS and FOG_OF_WAR_RADIUS_LOCATION behaves normally.
-- Same logic applies to Setters.
---@author UjAPI
---@param whichFogModifier fogmodifier
---@return real
function GetFogModifierX (whichFogModifier) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@param x real
function SetFogModifierX (whichFogModifier, x) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@return real
function GetFogModifierY (whichFogModifier) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@param y real
function SetFogModifierY (whichFogModifier, y) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@return real
function GetFogModifierZ (whichFogModifier) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@param z real
function SetFogModifierZ (whichFogModifier, z) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@return real
function GetFogModifierRadius (whichFogModifier) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@param radius real
function SetFogModifierRadius (whichFogModifier, radius) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@param x real
---@param y real
---@param radius real
function SetFogModifierRadiusEx (whichFogModifier, x, y, radius) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@param x real
---@param y real
function SetFogModifierPosition (whichFogModifier, x, y) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@param x real
---@param y real
---@param z real
function SetFogModifierPositionWithZ (whichFogModifier, x, y, z) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@param whichLocation location
function SetFogModifierPositionLoc (whichFogModifier, whichLocation) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@return rect
function GetFogModifierRect (whichFogModifier) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@param whichRect rect
function SetFogModifierRect (whichFogModifier, whichRect) end
---@author UjAPI
---@param whichFogModifier fogmodifier
---@param minX real
---@param minY real
---@param maxX real
---@param maxY real
function SetFogModifierRectEx (whichFogModifier, minX, minY, maxX, maxY) end
-- 

-- ============================================================================
-- Group API
---@author UjAPI
---@param whichGroup group
---@return integer
function GroupGetCount (whichGroup) end
---@author UjAPI
---@param whichGroup group
---@param whichUnit unit
---@return boolean
function GroupContainsUnit (whichGroup, whichUnit) end
---@author UjAPI
---@param whichGroup group
---@param index integer
---@return unit
function GroupGetUnitByIndex (whichGroup, index) end
---@author UjAPI
---@param whichGroup group
---@return unit
function GroupForEachUnit (whichGroup) end
---@author UjAPI
---@param destGroup group
---@param sourceGroup group
---@return integer
function GroupAddGroupEx (destGroup, sourceGroup) end
---@author UjAPI
---@param destGroup group
---@param sourceGroup group
---@return integer
function GroupRemoveGroupEx (destGroup, sourceGroup) end
-- 

-- ============================================================================
-- Player Trade Event API | EVENT_PLAYER_TRADE_RESOURCE
---@author UjAPI
---@return player
function GetTradeSource () end
---@author UjAPI
---@param whichPlayer player
function SetTradeSource (whichPlayer) end
---@author UjAPI
---@return player
function GetTradeTarget () end
---@author UjAPI
---@param whichPlayer player
function SetTradeTarget (whichPlayer) end
---@author UjAPI
---@return integer
function GetTradeGold () end
---@author UjAPI
---@param amount integer
function SetTradeGold (amount) end
---@author UjAPI
---@return integer
function GetTradeLumber () end
---@author UjAPI
---@param amount integer
function SetTradeLumber (amount) end
---@author UjAPI
---@param whichTradeState tradestate
---@return boolean
function IsTradeState (whichTradeState) end
---@author UjAPI
---@param whichTradeState tradestate
---@param isSet boolean
function SetTradeState (whichTradeState, isSet) end
---@author UjAPI
---@param fromPlayer player
---@param toPlayer player
---@param gold integer
---@param lumber integer
function TradePlayerResources (fromPlayer, toPlayer, gold, lumber) end
-- 

-- ============================================================================
-- Handle List API
-- For some of the functions, follow these rules: handleTypeId is base typeId of the object, such as '+w3u' for units, more on this below. And last, but not least typeId is the id of a widget/ability/buff, i.e. 'hfoo' for footman, etc.
-- Handle Type Id List:
-- Handle = 0 (NULL) | Agent = '+w3a' (for any agent) | Widget = '+w3w' | Unit = '+w3u' | Item = 'item' | Destructable = '+w3d' | Ability = 'abil' | Buff = 'buff' | Effect = 'efct' | Projectile = 'proj' | Frame = '+frm'
-- For any handleTypeId that is not present here, you can use GetHandleBaseTypeId on any handle to get its handleTypeId.
-- Note: Projectile = 'proj' is custom, it does not exist internally.

---@author UjAPI
---@return handlelist
function HandleListCreate () end
---@author UjAPI
---@param whichHandleList handlelist
function HandleListDestroy (whichHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichHandle handle
function HandleListAddHandle (whichHandleList, whichHandle) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichHandle handle
function HandleListRemoveHandle (whichHandleList, whichHandle) end
---@author UjAPI
---@param destHandleList handlelist
---@param sourceHandleList handlelist
---@return integer
function HandleListAddList (destHandleList, sourceHandleList) end
---@author UjAPI
---@param destHandleList handlelist
---@param sourceHandleList handlelist
---@return integer
function HandleListRemoveList (destHandleList, sourceHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
---@return integer
function HandleListRemoveEmpty (whichHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
function HandleListClear (whichHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichHandle handle
---@return boolean
function HandleListContainsHandle (whichHandleList, whichHandle) end
---@author UjAPI
---@param whichHandleList handlelist
---@param typeId integer
---@return boolean
function HandleListContainsTypeId (whichHandleList, typeId) end

---@author UjAPI
---@param whichHandleList handlelist
---@return integer
function HandleListGetCount (whichHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
---@param handleTypeId integer
---@return integer
function HandleListGetCountEx (whichHandleList, handleTypeId) end

---@author UjAPI
---@param whichHandleList handlelist
---@return integer
function HandleListGetAgentCount (whichHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
---@return integer
function HandleListGetWidgetCount (whichHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
---@return integer
function HandleListGetUnitCount (whichHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
---@return integer
function HandleListGetItemCount (whichHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
---@return integer
function HandleListGetDestructableCount (whichHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
---@return integer
function HandleListGetDoodadCount (whichHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
---@return integer
function HandleListGetAbilityCount (whichHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
---@return integer
function HandleListGetBuffCount (whichHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
---@return integer
function HandleListGetSpriteCount (whichHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
---@return integer
function HandleListGetEffectCount (whichHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
---@return integer
function HandleListGetProjectileCount (whichHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
---@return integer
function HandleListGetFrameCount (whichHandleList) end
---@author UjAPI
---@param whichHandleList handlelist
---@return integer
function HandleListGetOrderCount (whichHandleList) end

---@author UjAPI
---@param whichHandleList handlelist
---@param index integer
---@return handle
function HandleListGetHandleByIndex (whichHandleList, index) end
---@author UjAPI
---@param whichHandleList handlelist
---@param handleTypeId integer
---@param index integer
---@return handle
function HandleListGetHandleByIndexEx (whichHandleList, handleTypeId, index) end
---@author UjAPI
---@param whichHandleList handlelist
---@param typeId integer
---@param index integer
---@return handle
function HandleListGetHandleById (whichHandleList, typeId, index) end

---@author UjAPI
---@param whichHandleList handlelist
---@param index integer
---@return agent
function HandleListGetAgentByIndex (whichHandleList, index) end
---@author UjAPI
---@param whichHandleList handlelist
---@param index integer
---@return widget
function HandleListGetWidgetByIndex (whichHandleList, index) end
---@author UjAPI
---@param whichHandleList handlelist
---@param index integer
---@return unit
function HandleListGetUnitByIndex (whichHandleList, index) end
---@author UjAPI
---@param whichHandleList handlelist
---@param index integer
---@return item
function HandleListGetItemByIndex (whichHandleList, index) end
---@author UjAPI
---@param whichHandleList handlelist
---@param index integer
---@return destructable
function HandleListGetDestructableByIndex (whichHandleList, index) end
---@author UjAPI
---@param whichHandleList handlelist
---@param index integer
---@return doodad
function HandleListGetDoodadByIndex (whichHandleList, index) end
---@author UjAPI
---@param whichHandleList handlelist
---@param index integer
---@return ability
function HandleListGetAbilityByIndex (whichHandleList, index) end
---@author UjAPI
---@param whichHandleList handlelist
---@param index integer
---@return buff
function HandleListGetBuffByIndex (whichHandleList, index) end
---@author UjAPI
---@param whichHandleList handlelist
---@param index integer
---@return sprite
function HandleListGetSpriteByIndex (whichHandleList, index) end
---@author UjAPI
---@param whichHandleList handlelist
---@param index integer
---@return effect
function HandleListGetEffectByIndex (whichHandleList, index) end
---@author UjAPI
---@param whichHandleList handlelist
---@param index integer
---@return projectile
function HandleListGetProjectileByIndex (whichHandleList, index) end
---@author UjAPI
---@param whichHandleList handlelist
---@param index integer
---@return framehandle
function HandleListGetFrameByIndex (whichHandleList, index) end
---@author UjAPI
---@param whichHandleList handlelist
---@param index integer
---@return orderhandle
function HandleListGetOrderByIndex (whichHandleList, index) end

---@author UjAPI
---@return handle
function HandleListGetFilterHandle () end
---@author UjAPI
---@return agent
function HandleListGetFilterAgent () end
---@author UjAPI
---@return widget
function HandleListGetFilterWidget () end
---@author UjAPI
---@return unit
function HandleListGetFilterUnit () end
---@author UjAPI
---@return item
function HandleListGetFilterItem () end
---@author UjAPI
---@return destructable
function HandleListGetFilterDestructable () end
---@author UjAPI
---@return doodad
function HandleListGetFilterDoodad () end
---@author UjAPI
---@return ability
function HandleListGetFilterAbility () end
---@author UjAPI
---@return buff
function HandleListGetFilterBuff () end
---@author UjAPI
---@return sprite
function HandleListGetFilterSprite () end
---@author UjAPI
---@return effect
function HandleListGetFilterEffect () end
---@author UjAPI
---@return projectile
function HandleListGetFilterProjectile () end
---@author UjAPI
---@return framehandle
function HandleListGetFilterFrame () end
---@author UjAPI
---@return orderhandle
function HandleListGetFilterOrder () end

---@author UjAPI
---@return handle
function HandleListGetEnumHandle () end
---@author UjAPI
---@return agent
function HandleListGetEnumAgent () end
---@author UjAPI
---@return widget
function HandleListGetEnumWidget () end
---@author UjAPI
---@return unit
function HandleListGetEnumUnit () end
---@author UjAPI
---@return item
function HandleListGetEnumItem () end
---@author UjAPI
---@return destructable
function HandleListGetEnumDestructable () end
---@author UjAPI
---@return doodad
function HandleListGetEnumDoodad () end
---@author UjAPI
---@return ability
function HandleListGetEnumAbility () end
---@author UjAPI
---@return buff
function HandleListGetEnumBuff () end
---@author UjAPI
---@return sprite
function HandleListGetEnumSprite () end
---@author UjAPI
---@return effect
function HandleListGetEnumEffect () end
---@author UjAPI
---@return projectile
function HandleListGetEnumProjectile () end
---@author UjAPI
---@return framehandle
function HandleListGetEnumFrame () end
---@author UjAPI
---@return orderhandle
function HandleListGetEnumOrder () end

---@author UjAPI
---@param whichHandleList handlelist
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
function HandleListEnumInRange (whichHandleList, x, y, radius, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param x real
---@param y real
---@param radius real
---@param handleTypeId integer
---@param filter boolexpr
function HandleListEnumInRangeEx (whichHandleList, x, y, radius, handleTypeId, filter) end

---@author UjAPI
---@param whichHandleList handlelist
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
function HandleListEnumAgentsInRange (whichHandleList, x, y, radius, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
function HandleListEnumWidgetsInRange (whichHandleList, x, y, radius, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
function HandleListEnumUnitsInRange (whichHandleList, x, y, radius, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
function HandleListEnumItemsInRange (whichHandleList, x, y, radius, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
function HandleListEnumDestructablesInRange (whichHandleList, x, y, radius, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
function HandleListEnumDoodadsInRange (whichHandleList, x, y, radius, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
function HandleListEnumSpecialEffectsInRange (whichHandleList, x, y, radius, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
function HandleListEnumProjectilesInRange (whichHandleList, x, y, radius, filter) end

---@author UjAPI
---@param whichHandleList handlelist
---@param whichLocation location
---@param radius real
---@param filter boolexpr
function HandleListEnumInRangeOfLoc (whichHandleList, whichLocation, radius, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichLocation location
---@param radius real
---@param handleTypeId integer
---@param filter boolexpr
function HandleListEnumInRangeOfLocEx (whichHandleList, whichLocation, radius, handleTypeId, filter) end

---@author UjAPI
---@param whichHandleList handlelist
---@param whichLocation location
---@param radius real
---@param filter boolexpr
function HandleListEnumAgentsInRangeOfLoc (whichHandleList, whichLocation, radius, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichLocation location
---@param radius real
---@param filter boolexpr
function HandleListEnumWidgetsInRangeOfLoc (whichHandleList, whichLocation, radius, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichLocation location
---@param radius real
---@param filter boolexpr
function HandleListEnumUnitsInRangeOfLoc (whichHandleList, whichLocation, radius, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichLocation location
---@param radius real
---@param filter boolexpr
function HandleListEnumItemsInRangeOfLoc (whichHandleList, whichLocation, radius, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichLocation location
---@param radius real
---@param filter boolexpr
function HandleListEnumDestructablesInRangeOfLoc (whichHandleList, whichLocation, radius, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichLocation location
---@param radius real
---@param filter boolexpr
function HandleListEnumDoodadsInRangeOfLoc (whichHandleList, whichLocation, radius, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichLocation location
---@param radius real
---@param filter boolexpr
function HandleListEnumSpecialEffectsInRangeOfLoc (whichHandleList, whichLocation, radius, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichLocation location
---@param radius real
---@param filter boolexpr
function HandleListEnumProjectilesInRangeOfLoc (whichHandleList, whichLocation, radius, filter) end

---@author UjAPI
---@param whichHandleList handlelist
---@param whichRect rect
---@param filter boolexpr
function HandleListEnumInRect (whichHandleList, whichRect, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichRect rect
---@param handleTypeId integer
---@param filter boolexpr
function HandleListEnumInRectEx (whichHandleList, whichRect, handleTypeId, filter) end

---@author UjAPI
---@param whichHandleList handlelist
---@param whichRect rect
---@param filter boolexpr
function HandleListEnumAgentsInRect (whichHandleList, whichRect, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichRect rect
---@param filter boolexpr
function HandleListEnumWidgetsInRect (whichHandleList, whichRect, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichRect rect
---@param filter boolexpr
function HandleListEnumUnitsInRect (whichHandleList, whichRect, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichRect rect
---@param filter boolexpr
function HandleListEnumItemsInRect (whichHandleList, whichRect, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichRect rect
---@param filter boolexpr
function HandleListEnumDestructablesInRect (whichHandleList, whichRect, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichRect rect
---@param filter boolexpr
function HandleListEnumDoodadsInRect (whichHandleList, whichRect, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichRect rect
---@param filter boolexpr
function HandleListEnumSpecialEffectsInRect (whichHandleList, whichRect, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichRect rect
---@param filter boolexpr
function HandleListEnumProjectilesInRect (whichHandleList, whichRect, filter) end

---@author UjAPI
---@param whichHandleList handlelist
---@param handleTypeId integer
---@param filter boolexpr
function HandleListEnumById (whichHandleList, handleTypeId, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param handleTypeId integer
---@param typeId integer
---@param filter boolexpr
function HandleListEnumByIdEx (whichHandleList, handleTypeId, typeId, filter) end

---@author UjAPI
---@param whichHandleList handlelist
---@param whichUnit unit
---@param filter boolexpr
function HandleListEnumUnitAbilities (whichHandleList, whichUnit, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichUnit unit
---@param filter boolexpr
function HandleListEnumUnitBuffs (whichHandleList, whichUnit, filter) end
---@author UjAPI
---@param whichHandleList handlelist
---@param whichUnit unit
---@param filter boolexpr
function HandleListEnumUnitOrders (whichHandleList, whichUnit, filter) end

---@author UjAPI
---@param whichHandleList handlelist
---@param c code
function HandleListForEach (whichHandleList, c) end
---@author UjAPI
---@param whichHandleList handlelist
---@param handleTypeId integer
---@param c code
function HandleListForEachById (whichHandleList, handleTypeId, c) end
---@author UjAPI
---@param whichHandleList handlelist
---@param handleTypeId integer
---@param typeId integer
---@param c code
function HandleListForEachByIdEx (whichHandleList, handleTypeId, typeId, c) end
-- 

-- ============================================================================
-- TextTag API
-- 
---@author UjAPI
---@param whichTextTag texttag
---@return boolean
function IsTextTagVisible (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@return boolean
function IsTextTagSuspended (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@return real
function IsTextTagPermanent (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@return real
function GetTextTagX (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@param x real
function SetTextTagX (whichTextTag, x) end
---@author UjAPI
---@param whichTextTag texttag
---@return real
function GetTextTagY (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@param y real
function SetTextTagY (whichTextTag, y) end
---@author UjAPI
---@param whichTextTag texttag
---@return real
function GetTextTagZ (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@param z real
function SetTextTagZ (whichTextTag, z) end
---@author UjAPI
---@param whichTextTag texttag
---@return real
function GetTextTagScreenX (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@return real
function GetTextTagScreenY (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@return real
function GetTextTagHeight (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@param height real
function SetTextTagHeight (whichTextTag, height) end
---@author UjAPI
---@param whichTextTag texttag
---@return location
function GetTextTagPositionLoc (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@param whichLocation location
function SetTextTagPositionLoc (whichTextTag, whichLocation) end
---@author UjAPI
---@param whichTextTag texttag
---@return integer
function GetTextTagColour (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@return integer
function GetTextTagAlpha (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@param alpha integer
function SetTextTagAlpha (whichTextTag, alpha) end
---@author UjAPI
---@param whichTextTag texttag
---@return real
function GetTextTagVelocityX (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@param velX real
function SetTextTagVelocityX (whichTextTag, velX) end
---@author UjAPI
---@param whichTextTag texttag
---@return real
function GetTextTagVelocityY (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@param velY real
function SetTextTagVelocityY (whichTextTag, velY) end
---@author UjAPI
---@param whichTextTag texttag
---@return real
function GetTextTagVelocityZ (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@param velZ real
function SetTextTagVelocityZ (whichTextTag, velZ) end
---@author UjAPI
---@param whichTextTag texttag
---@return real
function GetTextTagAge (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@return real
function GetTextTagLifespan (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@return real
function GetTextTagFadepoint (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@return integer
function GetTextTagShadowColour (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@param colour integer
function SetTextTagShadowColour (whichTextTag, colour) end
---@author UjAPI
---@param whichTextTag texttag
---@return integer
function GetTextTagShadowAlpha (whichTextTag) end
---@author UjAPI
---@param whichTextTag texttag
---@param alpha integer
function SetTextTagShadowAlpha (whichTextTag, alpha) end
---@author UjAPI
---@param whichTextTag texttag
---@return string
function GetTextTagText (whichTextTag) end
-- 

-- ============================================================================
-- Lightning API
-- 
---@author UjAPI
---@param whichBolt lightning
---@return real
function GetLightningSourceX (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@param value real
function SetLightningSourceX (whichBolt, value) end
---@author UjAPI
---@param whichBolt lightning
---@return real
function GetLightningSourceY (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@param value real
function SetLightningSourceY (whichBolt, value) end
---@author UjAPI
---@param whichBolt lightning
---@return real
function GetLightningSourceZ (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@param value real
function SetLightningSourceZ (whichBolt, value) end
---@author UjAPI
---@param whichBolt lightning
---@return location
function GetLightningSourcePositionLoc (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@param whichLocation location
function SetLightningSourcePositionLoc (whichBolt, whichLocation) end
---@author UjAPI
---@param whichBolt lightning
---@return real
function GetLightningTargetX (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@param value real
function SetLightningTargetX (whichBolt, value) end
---@author UjAPI
---@param whichBolt lightning
---@return real
function GetLightningTargetY (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@param value real
function SetLightningTargetY (whichBolt, value) end
---@author UjAPI
---@param whichBolt lightning
---@return real
function GetLightningTargetZ (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@param value real
function SetLightningTargetZ (whichBolt, value) end
---@author UjAPI
---@param whichBolt lightning
---@return location
function GetLightningTargetPositionLoc (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@param whichLocation location
function SetLightningTargetPositionLoc (whichBolt, whichLocation) end
---@author UjAPI
---@param whichBolt lightning
---@return real
function GetLightningScreenX (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@return real
function GetLightningScreenY (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@return integer
function GetLightningColour (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@return integer
function GetLightningColourA (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@return integer
function GetLightningColourR (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@return integer
function GetLightningColourG (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@return integer
function GetLightningColourB (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
---@return boolean
function SetLightningColour (whichBolt, red, green, blue, alpha) end
---@author UjAPI
---@param whichBolt lightning
---@return real
function GetLightningLength (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@param value real
function SetLightningLength (whichBolt, value) end
---@author UjAPI
---@param whichBolt lightning
---@return real
function GetLightningWidth (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@param value real
function SetLightningWidth (whichBolt, value) end
---@author UjAPI
---@param whichBolt lightning
---@return real
function GetLightningNoiseScaling (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@param value real
function SetLightningNoiseScaling (whichBolt, value) end
---@author UjAPI
---@param whichBolt lightning
---@return real
function GetLightningTextureCoordinates (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@param value real
function SetLightningTextureCoordinates (whichBolt, value) end
---@author UjAPI
---@param whichBolt lightning
---@return real
function GetLightningDuration (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@param value real
function SetLightningDuration (whichBolt, value) end
---@author UjAPI
---@param whichBolt lightning
---@return string
function GetLightningTexture (whichBolt) end
---@author UjAPI
---@param whichBolt lightning
---@param textureName string
function SetLightningTexture (whichBolt, textureName) end
-- 

-- ============================================================================
-- Image API
-- 
---@author UjAPI
---@param file string
---@param sizeX real
---@param sizeY real
---@param posX real
---@param posY real
---@param posZ real
---@param imageType integer
---@return image
function CreateImageSimple (file, sizeX, sizeY, posX, posY, posZ, imageType) end
---@author UjAPI
---@param whichImage image
---@return integer
function GetImageType (whichImage) end
---@author UjAPI
---@param whichImage image
---@return real
function GetImageX (whichImage) end
---@author UjAPI
---@param whichImage image
---@param sizeX real
function SetImageX (whichImage, sizeX) end
---@author UjAPI
---@param whichImage image
---@return real
function GetImageY (whichImage) end
---@author UjAPI
---@param whichImage image
---@param sizeY real
function SetImageY (whichImage, sizeY) end
---@author UjAPI
---@param whichImage image
---@return real
function GetImageZ (whichImage) end
---@author UjAPI
---@param whichImage image
---@param sizeZ real
function SetImageZ (whichImage, sizeZ) end
---@author UjAPI
---@param whichImage image
---@return location
function GetImagePositionLoc (whichImage) end
---@author UjAPI
---@param whichImage image
---@param whichLocation location
function SetImagePositionLoc (whichImage, whichLocation) end
---@author UjAPI
---@param whichImage image
---@return real
function GetImageScreenX (whichImage) end
---@author UjAPI
---@param whichImage image
---@return real
function GetImageScreenY (whichImage) end
---@author UjAPI
---@param whichImage image
---@return real
function GetImageOriginX (whichImage) end
---@author UjAPI
---@param whichImage image
---@param originX real
function SetImageOriginX (whichImage, originX) end
---@author UjAPI
---@param whichImage image
---@return real
function GetImageOriginY (whichImage) end
---@author UjAPI
---@param whichImage image
---@param originY real
function SetImageOriginY (whichImage, originY) end
---@author UjAPI
---@param whichImage image
---@param origin real
function SetImageOrigin (whichImage, origin) end
---@author UjAPI
---@param whichImage image
---@param originX real
---@param originY real
function SetImageOriginEx (whichImage, originX, originY) end
---@author UjAPI
---@param whichImage image
---@return real
function GetImageSizeX (whichImage) end
---@author UjAPI
---@param whichImage image
---@param sizeX real
function SetImageSizeX (whichImage, sizeX) end
---@author UjAPI
---@param whichImage image
---@return real
function GetImageSizeY (whichImage) end
---@author UjAPI
---@param whichImage image
---@param sizeY real
function SetImageSizeY (whichImage, sizeY) end
---@author UjAPI
---@param whichImage image
---@param size real
function SetImageSize (whichImage, size) end
---@author UjAPI
---@param whichImage image
---@param sizeX real
---@param sizeY real
function SetImageSizeEx (whichImage, sizeX, sizeY) end
---@author UjAPI
---@param whichImage image
---@return integer
function GetImageColour (whichImage) end
---@author UjAPI
---@param whichImage image
---@return boolean
function IsImageRendered (whichImage) end
---@author UjAPI
---@param whichImage image
---@return boolean
function IsImageRenderAlways (whichImage) end
---@author UjAPI
---@param whichImage image
---@return boolean
function IsImageRenderAboveWater (whichImage) end
---@author UjAPI
---@param whichImage image
---@return boolean
function IsImageVisible (whichImage) end
---@author UjAPI
---@param whichImage image
---@return real
function GetImageConstantHeight (whichImage) end
---@author UjAPI
---@param whichImage image
---@return string
function GetImageTexture (whichImage) end
---@author UjAPI
---@param whichImage image
---@param imagePath string
function SetImageTexture (whichImage, imagePath) end
-- 

-- ============================================================================
-- Timer API
-- 
---@author UjAPI
---@param whichTimer timer
---@return boolean
function TimerIsPaused (whichTimer) end
---@author UjAPI
---@param whichTimer timer
function TimerRestart (whichTimer) end
---@author UjAPI
---@param whichTimer timer
---@return boolean
function TimerIsPeriodic (whichTimer) end
---@author UjAPI
---@param whichTimer timer
---@param isPeriodic boolean
function TimerSetPeriodic (whichTimer, isPeriodic) end
---@author UjAPI
---@param whichTimer timer
---@param remaining real
function TimerSetRemaining (whichTimer, remaining) end
---@author UjAPI
---@param whichTimer timer
---@param timeout real
function TimerSetTimeout (whichTimer, timeout) end
---@author UjAPI
---@param whichTimer timer
---@return code
function TimerGetCallback (whichTimer) end
---@author UjAPI
---@param whichTimer timer
---@param whichFunction code
function TimerSetCallback (whichTimer, whichFunction) end
-- 

-- ============================================================================
-- Ability API
-- 

-- Base Field API
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilityintegerfield
---@return integer
function GetAbilityBaseIntegerFieldById (abilityTypeId, whichField) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilityintegerfield
---@param value integer
---@return boolean
function SetAbilityBaseIntegerFieldById (abilityTypeId, whichField, value) end

---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilitybooleanfield
---@return boolean
function GetAbilityBaseBooleanFieldById (abilityTypeId, whichField) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilitybooleanfield
---@param value boolean
---@return boolean
function SetAbilityBaseBooleanFieldById (abilityTypeId, whichField, value) end

---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilityrealfield
---@return real
function GetAbilityBaseRealFieldById (abilityTypeId, whichField) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilityrealfield
---@param value real
---@return boolean
function SetAbilityBaseRealFieldById (abilityTypeId, whichField, value) end

---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilitystringfield
---@return string
function GetAbilityBaseStringFieldById (abilityTypeId, whichField) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilitystringfield
---@param value string
---@return boolean
function SetAbilityBaseStringFieldById (abilityTypeId, whichField, value) end

---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilityintegerlevelfield
---@param level integer
---@return integer
function GetAbilityBaseIntegerLevelFieldById (abilityTypeId, whichField, level) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilityintegerlevelfield
---@param level integer
---@param value integer
---@return boolean
function SetAbilityBaseIntegerLevelFieldById (abilityTypeId, whichField, level, value) end

---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilitybooleanlevelfield
---@param level integer
---@return boolean
function GetAbilityBaseBooleanLevelFieldById (abilityTypeId, whichField, level) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilitybooleanlevelfield
---@param level integer
---@param value boolean
---@return boolean
function SetAbilityBaseBooleanLevelFieldById (abilityTypeId, whichField, level, value) end

---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilityreallevelfield
---@param level integer
---@return real
function GetAbilityBaseRealLevelFieldById (abilityTypeId, whichField, level) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilityreallevelfield
---@param level integer
---@param value real
---@return boolean
function SetAbilityBaseRealLevelFieldById (abilityTypeId, whichField, level, value) end

---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilitystringlevelfield
---@param level integer
---@return string
function GetAbilityBaseStringLevelFieldById (abilityTypeId, whichField, level) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilitystringlevelfield
---@param level integer
---@param value string
---@return boolean
function SetAbilityBaseStringLevelFieldById (abilityTypeId, whichField, level, value) end

---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilityintegerlevelarrayfield
---@param level integer
---@param index integer
---@return integer
function GetAbilityBaseIntegerLevelArrayFieldById (abilityTypeId, whichField, level, index) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilityintegerlevelarrayfield
---@param level integer
---@param index integer
---@param value integer
---@return boolean
function SetAbilityBaseIntegerLevelArrayFieldById (abilityTypeId, whichField, level, index, value) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilityintegerlevelarrayfield
---@param level integer
---@param value integer
---@return boolean
function AddAbilityBaseIntegerLevelArrayFieldById (abilityTypeId, whichField, level, value) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilityintegerlevelarrayfield
---@param level integer
---@param value integer
---@return boolean
function RemoveAbilityBaseIntegerLevelArrayFieldById (abilityTypeId, whichField, level, value) end

---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilitybooleanlevelarrayfield
---@param level integer
---@param index integer
---@return boolean
function GetAbilityBaseBooleanLevelArrayFieldById (abilityTypeId, whichField, level, index) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilitybooleanlevelarrayfield
---@param level integer
---@param index integer
---@param value boolean
---@return boolean
function SetAbilityBaseBooleanLevelArrayFieldById (abilityTypeId, whichField, level, index, value) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilitybooleanlevelarrayfield
---@param level integer
---@param value boolean
---@return boolean
function AddAbilityBaseBooleanLevelArrayFieldById (abilityTypeId, whichField, level, value) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilitybooleanlevelarrayfield
---@param level integer
---@param value boolean
---@return boolean
function RemoveAbilityBaseBooleanLevelArrayFieldById (abilityTypeId, whichField, level, value) end

---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilityreallevelarrayfield
---@param level integer
---@param index integer
---@return real
function GetAbilityBaseRealLevelArrayFieldById (abilityTypeId, whichField, level, index) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilityreallevelarrayfield
---@param level integer
---@param index integer
---@param value real
---@return boolean
function SetAbilityBaseRealLevelArrayFieldById (abilityTypeId, whichField, level, index, value) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilityreallevelarrayfield
---@param level integer
---@param value real
---@return boolean
function AddAbilityBaseRealLevelArrayFieldById (abilityTypeId, whichField, level, value) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilityreallevelarrayfield
---@param level integer
---@param value real
---@return boolean
function RemoveAbilityBaseRealLevelArrayFieldById (abilityTypeId, whichField, level, value) end

---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilitystringlevelarrayfield
---@param level integer
---@param index integer
---@return string
function GetAbilityBaseStringLevelArrayFieldById (abilityTypeId, whichField, level, index) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilitystringlevelarrayfield
---@param level integer
---@param index integer
---@param value string
---@return boolean
function SetAbilityBaseStringLevelArrayFieldById (abilityTypeId, whichField, level, index, value) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilitystringlevelarrayfield
---@param level integer
---@param value string
---@return boolean
function AddAbilityBaseStringLevelArrayFieldById (abilityTypeId, whichField, level, value) end
---@author UjAPI
---@param abilityTypeId integer
---@param whichField abilitystringlevelarrayfield
---@param level integer
---@param value string
---@return boolean
function RemoveAbilityBaseStringLevelArrayFieldById (abilityTypeId, whichField, level, value) end
-- 

-- Field API
---@author UjAPI
---@param whichAbility ability
---@param whichField abilityintegerfield
---@return integer
function GetAbilityIntegerField (whichAbility, whichField) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilityintegerfield
---@param value integer
---@return boolean
function SetAbilityIntegerField (whichAbility, whichField, value) end

---@author UjAPI
---@param whichAbility ability
---@param whichField abilitybooleanfield
---@return boolean
function GetAbilityBooleanField (whichAbility, whichField) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilitybooleanfield
---@param value boolean
---@return boolean
function SetAbilityBooleanField (whichAbility, whichField, value) end

---@author UjAPI
---@param whichAbility ability
---@param whichField abilityrealfield
---@return real
function GetAbilityRealField (whichAbility, whichField) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilityrealfield
---@param value real
---@return boolean
function SetAbilityRealField (whichAbility, whichField, value) end

---@author UjAPI
---@param whichAbility ability
---@param whichField abilitystringfield
---@return string
function GetAbilityStringField (whichAbility, whichField) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilitystringfield
---@param value string
---@return boolean
function SetAbilityStringField (whichAbility, whichField, value) end

---@author UjAPI
---@param whichAbility ability
---@param whichField abilityintegerlevelfield
---@param level integer
---@return integer
function GetAbilityIntegerLevelField (whichAbility, whichField, level) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilityintegerlevelfield
---@param level integer
---@param value integer
---@return boolean
function SetAbilityIntegerLevelField (whichAbility, whichField, level, value) end

---@author UjAPI
---@param whichAbility ability
---@param whichField abilitybooleanlevelfield
---@param level integer
---@return boolean
function GetAbilityBooleanLevelField (whichAbility, whichField, level) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilitybooleanlevelfield
---@param level integer
---@param value boolean
---@return boolean
function SetAbilityBooleanLevelField (whichAbility, whichField, level, value) end

---@author UjAPI
---@param whichAbility ability
---@param whichField abilityreallevelfield
---@param level integer
---@return real
function GetAbilityRealLevelField (whichAbility, whichField, level) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilityreallevelfield
---@param level integer
---@param value real
---@return boolean
function SetAbilityRealLevelField (whichAbility, whichField, level, value) end

---@author UjAPI
---@param whichAbility ability
---@param whichField abilitystringlevelfield
---@param level integer
---@return string
function GetAbilityStringLevelField (whichAbility, whichField, level) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilitystringlevelfield
---@param level integer
---@param value string
---@return boolean
function SetAbilityStringLevelField (whichAbility, whichField, level, value) end

---@author UjAPI
---@param whichAbility ability
---@param whichField abilityintegerlevelarrayfield
---@param level integer
---@param index integer
---@return integer
function GetAbilityIntegerLevelArrayField (whichAbility, whichField, level, index) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilityintegerlevelarrayfield
---@param level integer
---@param index integer
---@param value integer
---@return boolean
function SetAbilityIntegerLevelArrayField (whichAbility, whichField, level, index, value) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilityintegerlevelarrayfield
---@param level integer
---@param value integer
---@return boolean
function AddAbilityIntegerLevelArrayField (whichAbility, whichField, level, value) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilityintegerlevelarrayfield
---@param level integer
---@param value integer
---@return boolean
function RemoveAbilityIntegerLevelArrayField (whichAbility, whichField, level, value) end

---@author UjAPI
---@param whichAbility ability
---@param whichField abilitybooleanlevelarrayfield
---@param level integer
---@param index integer
---@return boolean
function GetAbilityBooleanLevelArrayField (whichAbility, whichField, level, index) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilitybooleanlevelarrayfield
---@param level integer
---@param index integer
---@param value boolean
---@return boolean
function SetAbilityBooleanLevelArrayField (whichAbility, whichField, level, index, value) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilitybooleanlevelarrayfield
---@param level integer
---@param value boolean
---@return boolean
function AddAbilityBooleanLevelArrayField (whichAbility, whichField, level, value) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilitybooleanlevelarrayfield
---@param level integer
---@param value boolean
---@return boolean
function RemoveAbilityBooleanLevelArrayField (whichAbility, whichField, level, value) end

---@author UjAPI
---@param whichAbility ability
---@param whichField abilityreallevelarrayfield
---@param level integer
---@param index integer
---@return real
function GetAbilityRealLevelArrayField (whichAbility, whichField, level, index) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilityreallevelarrayfield
---@param level integer
---@param index integer
---@param value real
---@return boolean
function SetAbilityRealLevelArrayField (whichAbility, whichField, level, index, value) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilityreallevelarrayfield
---@param level integer
---@param value real
---@return boolean
function AddAbilityRealLevelArrayField (whichAbility, whichField, level, value) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilityreallevelarrayfield
---@param level integer
---@param value real
---@return boolean
function RemoveAbilityRealLevelArrayField (whichAbility, whichField, level, value) end

---@author UjAPI
---@param whichAbility ability
---@param whichField abilitystringlevelarrayfield
---@param level integer
---@param index integer
---@return string
function GetAbilityStringLevelArrayField (whichAbility, whichField, level, index) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilitystringlevelarrayfield
---@param level integer
---@param index integer
---@param value string
---@return boolean
function SetAbilityStringLevelArrayField (whichAbility, whichField, level, index, value) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilitystringlevelarrayfield
---@param level integer
---@param value string
---@return boolean
function AddAbilityStringLevelArrayField (whichAbility, whichField, level, value) end
---@author UjAPI
---@param whichAbility ability
---@param whichField abilitystringlevelarrayfield
---@param level integer
---@param value string
---@return boolean
function RemoveAbilityStringLevelArrayField (whichAbility, whichField, level, value) end
-- 

---@author UjAPI
---@param whichAbility ability
---@return boolean
function ResetAbilityFieldData (whichAbility) end

---@author UjAPI
---@param abilityTypeId integer
---@param source widget
---@param target widget
---@return boolean
function IsAbilityBaseTargetAllowed (abilityTypeId, source, target) end

-- Normal API
---@author UjAPI
---@param abilityTypeId integer
---@return ability
function CreateAbility (abilityTypeId) end
---@author UjAPI
---@param whichAbility ability
function RemoveAbility (whichAbility) end

---@author UjAPI
---@return ability
function GetTriggerAbility () end
---@author UjAPI
---@return integer
function GetTriggerAbilityPreviousLevel () end
---@author UjAPI
---@param whichAbility ability
---@param whichAbilityType abilitytype
---@return boolean
function IsAbilityType (whichAbility, whichAbilityType) end
---@author UjAPI
---@param whichAbility ability
---@return unit
function GetAbilityOwner (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@param whichUnit unit
function SetAbilityOwner (whichAbility, whichUnit) end
---@author UjAPI
---@param whichAbility ability
---@return ability
function GetAbilityOwningAbility (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@param whichSpellbook ability
function SetAbilityOwningAbility (whichAbility, whichSpellbook) end
---@author UjAPI
---@param whichAbility ability
---@return item
function GetAbilityOwningItem (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@param whichItem item
function SetAbilityOwningItem (whichAbility, whichItem) end
---@author UjAPI
---@param whichAbility ability
---@return integer
function GetAbilityOrderId (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@param orderId integer
function SetAbilityOrderId (whichAbility, orderId) end
---@author UjAPI
---@param whichAbility ability
function ResetAbilityOrder (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@return integer
function GetAbilityLevel (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@param level integer
---@return integer
function SetAbilityLevel (whichAbility, level) end
---@author UjAPI
---@param whichAbility ability
---@return integer
function GetAbilityBaseTypeId (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@return integer
function GetAbilityTypeId (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@param whichWidget widget
---@return boolean
function IsAbilityTargetAllowed (whichAbility, whichWidget) end
---@author UjAPI
---@param whichAbility ability
---@return boolean
function IsAbilityOnCooldown (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@return boolean
function IsAbilityEnabled (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@param enable boolean
function SetAbilityEnabled (whichAbility, enable) end
---@author UjAPI
---@param whichAbility ability
---@return boolean
function IsAbilityVisible (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@param show boolean
function ShowAbility (whichAbility, show) end
---@author UjAPI
---@param whichAbility ability
---@return boolean
function IsAbilityEnabledEx (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@param enable boolean
function SetAbilityEnabledEx (whichAbility, enable) end
---@author UjAPI
---@param whichAbility ability
---@return boolean
function IsAbilityUsable (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@return boolean
function IsAbilityAutocastEnabled (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@return real
function GetAbilityCastTime (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@param castTime real
function SetAbilityCastTime (whichAbility, castTime) end
---@author UjAPI
---@param whichAbility ability
---@return real
function GetAbilityCastPoint (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@param castPoint real
function SetAbilityCastPoint (whichAbility, castPoint) end
---@author UjAPI
---@param whichAbility ability
---@return real
function GetAbilityBackswing (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@param backswing real
function SetAbilityBackswing (whichAbility, backswing) end
---@author UjAPI
---@param whichAbility ability
---@return real
function GetAbilityCooldown (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@param cooldown real
function SetAbilityCooldown (whichAbility, cooldown) end
---@author UjAPI
---@param whichAbility ability
---@return real
function GetAbilityRemainingCooldown (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@param cooldown real
function SetAbilityRemainingCooldown (whichAbility, cooldown) end
---@author UjAPI
---@param whichAbility ability
---@param cooldown real
function StartAbilityCooldown (whichAbility, cooldown) end
---@author UjAPI
---@param whichAbility ability
---@param hide boolean
---@param disable boolean
function DisableAbility (whichAbility, hide, disable) end
---@author UjAPI
---@param whichAbility ability
---@param show boolean
---@param enable boolean
function EnableAbility (whichAbility, show, enable) end
---@author UjAPI
---@param whichAbility ability
---@return boolean
function CastAbility (whichAbility) end
---@author UjAPI
---@param whichAbility ability
---@param targX real
---@param targY real
---@return boolean
function CastAbilityGround (whichAbility, targX, targY) end
---@author UjAPI
---@param whichAbility ability
---@param target widget
---@return boolean
function CastAbilityTarget (whichAbility, target) end

---@author UjAPI
---@return ability
function GetFilterAbility () end
---@author UjAPI
---@return ability
function GetEnumAbility () end

---@author UjAPI
---@param whichUnit unit
---@param whichBoolexpr boolexpr
---@param whichCode code
---@return boolean
function EnumUnitAbilities (whichUnit, whichBoolexpr, whichCode) end
-- 

-- ============================================================================
-- Buff API
-- 

-- Base Field API
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilityintegerfield
---@return integer
function GetBuffBaseIntegerFieldById (buffTypeId, whichField) end
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilityintegerfield
---@param value integer
---@return boolean
function SetBuffBaseIntegerFieldById (buffTypeId, whichField, value) end

---@author UjAPI
---@param buffTypeId integer
---@param whichField abilitybooleanfield
---@return boolean
function GetBuffBaseBooleanFieldById (buffTypeId, whichField) end
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilitybooleanfield
---@param value boolean
---@return boolean
function SetBuffBaseBooleanFieldById (buffTypeId, whichField, value) end

---@author UjAPI
---@param buffTypeId integer
---@param whichField abilityrealfield
---@return real
function GetBuffBaseRealFieldById (buffTypeId, whichField) end
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilityrealfield
---@param value real
---@return boolean
function SetBuffBaseRealFieldById (buffTypeId, whichField, value) end

---@author UjAPI
---@param buffTypeId integer
---@param whichField abilitystringfield
---@return string
function GetBuffBaseStringFieldById (buffTypeId, whichField) end
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilitystringfield
---@param value string
---@return boolean
function SetBuffBaseStringFieldById (buffTypeId, whichField, value) end

---@author UjAPI
---@param buffTypeId integer
---@param whichField abilityintegerfield
---@param index integer
---@return integer
function GetBuffBaseIntegerArrayFieldById (buffTypeId, whichField, index) end
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilityintegerfield
---@param index integer
---@param value integer
---@return boolean
function SetBuffBaseIntegerArrayFieldById (buffTypeId, whichField, index, value) end
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilityintegerfield
---@param value integer
---@return boolean
function AddBuffBaseIntegerArrayFieldById (buffTypeId, whichField, value) end
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilityintegerfield
---@param value integer
---@return boolean
function RemoveBuffBaseIntegerArrayFieldById (buffTypeId, whichField, value) end

---@author UjAPI
---@param buffTypeId integer
---@param whichField abilitybooleanfield
---@param index integer
---@return boolean
function GetBuffBaseBooleanArrayFieldById (buffTypeId, whichField, index) end
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilitybooleanfield
---@param index integer
---@param value boolean
---@return boolean
function SetBuffBaseBooleanArrayFieldById (buffTypeId, whichField, index, value) end
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilitybooleanfield
---@param value boolean
---@return boolean
function AddBuffBaseBooleanArrayFieldById (buffTypeId, whichField, value) end
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilitybooleanfield
---@param value boolean
---@return boolean
function RemoveBuffBaseBooleanArrayFieldById (buffTypeId, whichField, value) end

---@author UjAPI
---@param buffTypeId integer
---@param whichField abilityrealfield
---@param index integer
---@return real
function GetBuffBaseRealArrayFieldById (buffTypeId, whichField, index) end
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilityrealfield
---@param index integer
---@param value real
---@return boolean
function SetBuffBaseRealArrayFieldById (buffTypeId, whichField, index, value) end
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilityrealfield
---@param value real
---@return boolean
function AddBuffBaseRealArrayFieldById (buffTypeId, whichField, value) end
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilityrealfield
---@param value real
---@return boolean
function RemoveBuffBaseRealArrayFieldById (buffTypeId, whichField, value) end

---@author UjAPI
---@param buffTypeId integer
---@param whichField abilitystringfield
---@param index integer
---@return string
function GetBuffBaseStringArrayFieldById (buffTypeId, whichField, index) end
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilitystringfield
---@param index integer
---@param value string
---@return boolean
function SetBuffBaseStringArrayFieldById (buffTypeId, whichField, index, value) end
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilitystringfield
---@param value string
---@return boolean
function AddBuffBaseStringArrayFieldById (buffTypeId, whichField, value) end
---@author UjAPI
---@param buffTypeId integer
---@param whichField abilitystringfield
---@param value string
---@return boolean
function RemoveBuffBaseStringArrayFieldById (buffTypeId, whichField, value) end
-- 

-- Field API
---@author UjAPI
---@param whichBuff buff
---@param whichField abilityintegerfield
---@return integer
function GetBuffIntegerField (whichBuff, whichField) end
---@author UjAPI
---@param whichBuff buff
---@param whichField abilityintegerfield
---@param value integer
---@return boolean
function SetBuffIntegerField (whichBuff, whichField, value) end

---@author UjAPI
---@param whichBuff buff
---@param whichField abilitybooleanfield
---@return boolean
function GetBuffBooleanField (whichBuff, whichField) end
---@author UjAPI
---@param whichBuff buff
---@param whichField abilitybooleanfield
---@param value boolean
---@return boolean
function SetBuffBooleanField (whichBuff, whichField, value) end

---@author UjAPI
---@param whichBuff buff
---@param whichField abilityrealfield
---@return real
function GetBuffRealField (whichBuff, whichField) end
---@author UjAPI
---@param whichBuff buff
---@param whichField abilityrealfield
---@param value real
---@return boolean
function SetBuffRealField (whichBuff, whichField, value) end

---@author UjAPI
---@param whichBuff buff
---@param whichField abilitystringfield
---@return string
function GetBuffStringField (whichBuff, whichField) end
---@author UjAPI
---@param whichBuff buff
---@param whichField abilitystringfield
---@param value string
---@return boolean
function SetBuffStringField (whichBuff, whichField, value) end

---@author UjAPI
---@param whichBuff buff
---@param whichField abilityintegerfield
---@param index integer
---@return integer
function GetBuffIntegerArrayField (whichBuff, whichField, index) end
---@author UjAPI
---@param whichBuff buff
---@param whichField abilityintegerfield
---@param index integer
---@param value integer
---@return boolean
function SetBuffIntegerArrayField (whichBuff, whichField, index, value) end
---@author UjAPI
---@param whichBuff buff
---@param whichField abilityintegerfield
---@param value integer
---@return boolean
function AddBuffIntegerArrayField (whichBuff, whichField, value) end
---@author UjAPI
---@param whichBuff buff
---@param whichField abilityintegerfield
---@param value integer
---@return boolean
function RemoveBuffIntegerArrayField (whichBuff, whichField, value) end

---@author UjAPI
---@param whichBuff buff
---@param whichField abilitybooleanfield
---@param index integer
---@return boolean
function GetBuffBooleanArrayField (whichBuff, whichField, index) end
---@author UjAPI
---@param whichBuff buff
---@param whichField abilitybooleanfield
---@param index integer
---@param value boolean
---@return boolean
function SetBuffBooleanArrayField (whichBuff, whichField, index, value) end
---@author UjAPI
---@param whichBuff buff
---@param whichField abilitybooleanfield
---@param value boolean
---@return boolean
function AddBuffBooleanArrayField (whichBuff, whichField, value) end
---@author UjAPI
---@param whichBuff buff
---@param whichField abilitybooleanfield
---@param value boolean
---@return boolean
function RemoveBuffBooleanArrayField (whichBuff, whichField, value) end

---@author UjAPI
---@param whichBuff buff
---@param whichField abilityrealfield
---@param index integer
---@return real
function GetBuffRealArrayField (whichBuff, whichField, index) end
---@author UjAPI
---@param whichBuff buff
---@param whichField abilityrealfield
---@param index integer
---@param value real
---@return boolean
function SetBuffRealArrayField (whichBuff, whichField, index, value) end
---@author UjAPI
---@param whichBuff buff
---@param whichField abilityrealfield
---@param value real
---@return boolean
function AddBuffRealArrayField (whichBuff, whichField, value) end
---@author UjAPI
---@param whichBuff buff
---@param whichField abilityrealfield
---@param value real
---@return boolean
function RemoveBuffRealArrayField (whichBuff, whichField, value) end

---@author UjAPI
---@param whichBuff buff
---@param whichField abilitystringfield
---@param index integer
---@return string
function GetBuffStringArrayField (whichBuff, whichField, index) end
---@author UjAPI
---@param whichBuff buff
---@param whichField abilitystringfield
---@param index integer
---@param value string
---@return boolean
function SetBuffStringArrayField (whichBuff, whichField, index, value) end
---@author UjAPI
---@param whichBuff buff
---@param whichField abilitystringfield
---@param value string
---@return boolean
function AddBuffStringArrayField (whichBuff, whichField, value) end
---@author UjAPI
---@param whichBuff buff
---@param whichField abilitystringfield
---@param value string
---@return boolean
function RemoveBuffStringArrayField (whichBuff, whichField, value) end
-- 

---@author UjAPI
---@param whichBuff buff
---@return boolean
function ResetBuffFieldData (whichBuff) end

-- Normal API
-- Supported buffs are available here: https://github.com/UnryzeC/UjAPI/blob/main/TypeData/WC3BuffListSupportedInBuffAPI.txt
---@author UjAPI
---@param buffTypeId integer
---@return buff
function CreateBuff (buffTypeId) end
---@author UjAPI
---@param whichBuff buff
function RemoveBuff (whichBuff) end

---@param whichBuff buff
---@param whichAbilityType abilitytype
---@return boolean
function IsBuffType (whichBuff, whichAbilityType) end
---@author UjAPI
---@param whichBuff buff
---@return integer
function GetBuffTypeId (whichBuff) end
---@author UjAPI
---@param whichBuff buff
---@return integer
function GetBuffBaseTypeId (whichBuff) end
---@author UjAPI
---@param whichbuff buff
---@return unit
function GetBuffOwner (whichbuff) end
---@author UjAPI
---@param whichBuff buff
---@param whichUnit unit
function SetBuffOwner (whichBuff, whichUnit) end
---@author UjAPI
---@param whichbuff buff
---@return ability
function GetBuffOwningAbility (whichbuff) end
---@author UjAPI
---@param whichBuff buff
---@param whichAbility ability
function SetBuffOwningAbility (whichBuff, whichAbility) end
---@author UjAPI
---@param whichBuff buff
---@return boolean
function IsBuffDispellable (whichBuff) end
---@author UjAPI
---@param whichBuff buff
---@param isSet boolean
function SetBuffDispellable (whichBuff, isSet) end
---@author UjAPI
---@param whichBuff buff
---@return integer
function GetBuffLevel (whichBuff) end
---@author UjAPI
---@param whichBuff buff
---@param level integer
function SetBuffLevel (whichBuff, level) end
---@author UjAPI
---@param whichBuff buff
---@return real
function GetBuffRemainingDuration (whichBuff) end
---@author UjAPI
---@param whichBuff buff
---@param duration real
function SetBuffRemainingDuration (whichBuff, duration) end
---@author UjAPI
---@param whichBuff buff
---@param pause boolean
function PauseBuff (whichBuff, pause) end
---@author UjAPI
---@param whichBuff buff
function RefreshBuff (whichBuff) end

---@author UjAPI
---@return buff
function GetFilterBuff () end
---@author UjAPI
---@return buff
function GetEnumBuff () end

---@author UjAPI
---@param whichUnit unit
---@param whichBoolexpr boolexpr
---@param whichCode code
---@return boolean
function EnumUnitBuffs (whichUnit, whichBoolexpr, whichCode) end

---@author UjAPI
---@return buff
function GetTriggerBuff () end
---@author UjAPI
---@return ability
function GetTriggerBuffSourceAbility () end
---@author UjAPI
---@return unit
function GetTriggerBuffSourceUnit () end
---@author UjAPI
---@return unit
function GetTriggerBuffTarget () end
-- 

-- ============================================================================
-- War3Image API
-- 
-- This is API for the "lowest" in terms of hierarchy object type for any and all widgets. Sprites and doodads are exception, however this API can distinguish between them and handle accordingly.
---@author UjAPI
---@param whichWar3Image war3image
---@return sprite
function GetWar3ImageSprite (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@return boolean
function IsWar3ImageVisible (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param visible boolean
function SetWar3ImageVisible (whichWar3Image, visible) end
---@author UjAPI
---@param whichWar3Image war3image
---@return boolean
function IsWar3ImageInvulnerable (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param invulnerable boolean
function SetWar3ImageInvulnerable (whichWar3Image, invulnerable) end
---@author UjAPI
---@param whichWar3Image war3image
---@return real
function GetWar3ImageX (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@return real
function GetWar3ImageY (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@return real
function GetWar3ImageZ (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@return location
function GetWar3ImagePositionLoc (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param whichLocation location
function SetWar3ImagePositionLoc (whichWar3Image, whichLocation) end
---@author UjAPI
---@param whichWar3Image war3image
---@param x real
---@param y real
function SetWar3ImagePosition (whichWar3Image, x, y) end
---@author UjAPI
---@param whichWar3Image war3image
---@param x real
---@param y real
---@param z real
function SetWar3ImagePositionWithZ (whichWar3Image, x, y, z) end
---@author UjAPI
---@param whichWar3Image war3image
---@param x real
function SetWar3ImageX (whichWar3Image, x) end
---@author UjAPI
---@param whichWar3Image war3image
---@param y real
function SetWar3ImageY (whichWar3Image, y) end
---@author UjAPI
---@param whichWar3Image war3image
---@param z real
function SetWar3ImageZ (whichWar3Image, z) end
---@author UjAPI
---@param whichWar3Image war3image
function ResetWar3ImageZ (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@return real
function GetWar3ImageHeight (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param height real
function SetWar3ImageHeight (whichWar3Image, height) end
---@author UjAPI
---@param whichWar3Image war3image
---@return real
function GetWar3ImageScreenX (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@return real
function GetWar3ImageScreenY (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@return playercolor
function GetWar3ImagePlayerColour (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param color playercolor
function SetWar3ImagePlayerColour (whichWar3Image, color) end
---@author UjAPI
---@param whichWar3Image war3image
---@return integer
function GetWar3ImageVertexColour (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function SetWar3ImageVertexColour (whichWar3Image, red, green, blue, alpha) end
---@author UjAPI
---@param whichWar3Image war3image
---@return real
function GetWar3ImageTimeScale (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param timeScale real
function SetWar3ImageTimeScale (whichWar3Image, timeScale) end
---@author UjAPI
---@param whichWar3Image war3image
---@return real
function GetWar3ImageScale (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param scale real
function SetWar3ImageScale (whichWar3Image, scale) end
---@author UjAPI
---@param whichWar3Image war3image
---@return real
function GetWar3ImageFacing (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param facing real
---@param isInstant boolean
function SetWar3ImageFacing (whichWar3Image, facing, isInstant) end
---@author UjAPI
---@param whichWar3Image war3image
---@return real
function GetWar3ImageMatrixScaleX (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@return real
function GetWar3ImageMatrixScaleY (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@return real
function GetWar3ImageMatrixScaleZ (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param x real
---@param y real
---@param z real
function SetWar3ImageMatrixScale (whichWar3Image, x, y, z) end
---@author UjAPI
---@param whichWar3Image war3image
function ResetWar3ImageMatrix (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param yaw real
---@param pitch real
---@param roll real
---@param eulerOrder integer
function SetWar3ImageOrientationEx (whichWar3Image, yaw, pitch, roll, eulerOrder) end
---@author UjAPI
---@param whichWar3Image war3image
---@param yaw real
---@param pitch real
---@param roll real
function SetWar3ImageOrientation (whichWar3Image, yaw, pitch, roll) end
---@author UjAPI
---@param whichWar3Image war3image
---@return real
function GetWar3ImageYaw (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param yaw real
function SetWar3ImageYaw (whichWar3Image, yaw) end
---@author UjAPI
---@param whichWar3Image war3image
---@return real
function GetWar3ImagePitch (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param pitch real
function SetWar3ImagePitch (whichWar3Image, pitch) end
---@author UjAPI
---@param whichWar3Image war3image
---@return real
function GetWar3ImageRoll (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param roll real
function SetWar3ImageRoll (whichWar3Image, roll) end
---@author UjAPI
---@param whichWar3Image war3image
---@return string
function GetWar3ImageModel (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param modelFile string
function SetWar3ImageModel (whichWar3Image, modelFile) end
---@author UjAPI
---@param whichWar3Image war3image
---@param modelFile string
---@param playerId integer
function SetWar3ImageModelEx (whichWar3Image, modelFile, playerId) end
---@author UjAPI
---@param whichWar3Image war3image
---@param textureName string
---@param materialId integer
---@param textureIndex integer
function SetWar3ImageMaterialTexture (whichWar3Image, textureName, materialId, textureIndex) end
---@author UjAPI
---@param whichWar3Image war3image
---@param textureName string
---@param textureIndex integer
function SetWar3ImageTexture (whichWar3Image, textureName, textureIndex) end
---@author UjAPI
---@param whichWar3Image war3image
---@param textureName string
---@param textureIndex integer
function SetWar3ImageReplaceableTexture (whichWar3Image, textureName, textureIndex) end
---@author UjAPI
---@param whichWar3Image war3image
---@param whichObject string
---@return real
function GetWar3ImageModelObjectX (whichWar3Image, whichObject) end
---@author UjAPI
---@param whichWar3Image war3image
---@param whichObject string
---@return real
function GetWar3ImageModelObjectY (whichWar3Image, whichObject) end
---@author UjAPI
---@param whichWar3Image war3image
---@param whichObject string
---@return real
function GetWar3ImageModelObjectZ (whichWar3Image, whichObject) end
---@author UjAPI
---@param whichWar3Image war3image
---@param whichObject string
---@return location
function GetWar3ImageModelObjectPositionLoc (whichWar3Image, whichObject) end
---@author UjAPI
---@param whichWar3Image war3image
---@return integer
function GetWar3ImageCurrentAnimationId (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@return string
function GetWar3ImageCurrentAnimationName (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param animIndex integer
---@param rarity raritycontrol
function SetWar3ImageAnimationWithRarityByIndex (whichWar3Image, animIndex, rarity) end
---@author UjAPI
---@param whichWar3Image war3image
---@param animationName string
---@param rarity raritycontrol
function SetWar3ImageAnimationWithRarity (whichWar3Image, animationName, rarity) end
---@author UjAPI
---@param whichWar3Image war3image
---@param animIndex integer
function SetWar3ImageAnimationByIndex (whichWar3Image, animIndex) end
---@author UjAPI
---@param whichWar3Image war3image
---@param animationName string
function SetWar3ImageAnimation (whichWar3Image, animationName) end
---@author UjAPI
---@param whichWar3Image war3image
---@param animIndex integer
function QueueWar3ImageAnimationByIndex (whichWar3Image, animIndex) end
---@author UjAPI
---@param whichWar3Image war3image
---@param animationName string
function QueueWar3ImageAnimation (whichWar3Image, animationName) end
---@author UjAPI
---@param whichWar3Image war3image
---@return real
function GetWar3ImageAnimationOffsetPercent (whichWar3Image) end
---@author UjAPI
---@param whichWar3Image war3image
---@param percent real
function SetWar3ImageAnimationOffsetPercent (whichWar3Image, percent) end
-- 

-- ============================================================================
-- Sprite API
-- 
-- Note: any axis setter is ignored by sprites created via AddSpriteToTarget, since they inherit nearly all data from sprite they are attached to.

---@author UjAPI
---@param isUber boolean
---@return sprite
function CreateSprite (isUber) end
---@author UjAPI
---@param whichSprite sprite
---@param targetSprite sprite
---@param attachPointName string
---@return sprite
function AttachSpriteToTarget (whichSprite, targetSprite, attachPointName) end
---@author UjAPI
---@param modelName string
---@param targetSprite sprite
---@param attachPointName string
---@return sprite
function AddSpriteToTarget (modelName, targetSprite, attachPointName) end
---@author UjAPI
---@param whichSprite sprite
---@return integer
function GetSpriteChildrenCount (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@param index integer
---@return sprite
function GetSpriteChildById (whichSprite, index) end

---@author UjAPI
---@param whichSprite sprite
function DetachSprite (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
function RemoveSprite (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@return boolean
function IsSpriteUber (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@return real
function GetSpriteX (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@return real
function GetSpriteY (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@return real
function GetSpriteZ (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@return real
function GetSpriteHeight (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@return location
function GetSpritePositionLoc (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@param x real
---@param y real
function SetSpritePosition (whichSprite, x, y) end
---@author UjAPI
---@param whichSprite sprite
---@param x real
---@param y real
---@param z real
function SetSpritePositionWithZ (whichSprite, x, y, z) end
---@author UjAPI
---@param whichSprite sprite
---@param loc location
function SetSpritePositionLoc (whichSprite, loc) end
---@author UjAPI
---@param whichSprite sprite
---@param x real
function SetSpriteX (whichSprite, x) end
---@author UjAPI
---@param whichSprite sprite
---@param y real
function SetSpriteY (whichSprite, y) end
---@author UjAPI
---@param whichSprite sprite
---@param z real
function SetSpriteZ (whichSprite, z) end
---@author UjAPI
---@param whichSprite sprite
---@param height real
function SetSpriteHeight (whichSprite, height) end
---@author UjAPI
---@param whichSprite sprite
---@return real
function GetSpriteScreenX (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@return real
function GetSpriteScreenY (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@return real
function GetSpriteScale (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@param scale real
function SetSpriteScale (whichSprite, scale) end
---@author UjAPI
---@param whichSprite sprite
---@return real
function GetSpriteTimeScale (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@param timescale real
function SetSpriteTimeScale (whichSprite, timescale) end
---@author UjAPI
---@param whichSprite sprite
---@return playercolor
function GetSpritePlayerColour (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@param color playercolor
function SetSpritePlayerColour (whichSprite, color) end
---@author UjAPI
---@param whichSprite sprite
---@return integer
function GetSpriteColour (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@param colour integer
function SetSpriteColour (whichSprite, colour) end
---@author UjAPI
---@param whichSprite sprite
---@param alpha integer
function SetSpriteAlpha (whichSprite, alpha) end
---@author UjAPI
---@param whichSprite sprite
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function SetSpriteVertexColour (whichSprite, red, green, blue, alpha) end
---@author UjAPI
---@param whichSprite sprite
---@return real
function GetSpriteMatrixScaleX (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@return real
function GetSpriteMatrixScaleY (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@return real
function GetSpriteMatrixScaleZ (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@param x real
---@param y real
---@param z real
function SetSpriteMatrixScale (whichSprite, x, y, z) end
---@author UjAPI
---@param whichSprite sprite
function ResetSpriteMatrix (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@param yaw real
---@param pitch real
---@param roll real
---@param eulerOrder integer
function SetSpriteOrientationEx (whichSprite, yaw, pitch, roll, eulerOrder) end
---@author UjAPI
---@param whichSprite sprite
---@return real
function GetSpriteYaw (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@param yaw real
function SetSpriteYaw (whichSprite, yaw) end
---@author UjAPI
---@param whichSprite sprite
---@return real
function GetSpriteFacing (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@param facing real
function SetSpriteFacing (whichSprite, facing) end
---@author UjAPI
---@param whichSprite sprite
---@return real
function GetSpritePitch (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@param pitch real
function SetSpritePitch (whichSprite, pitch) end
---@author UjAPI
---@param whichSprite sprite
---@return real
function GetSpriteRoll (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@param roll real
function SetSpriteRoll (whichSprite, roll) end
---@author UjAPI
---@param whichSprite sprite
---@param yaw real
---@param pitch real
---@param roll real
function SetSpriteOrientation (whichSprite, yaw, pitch, roll) end
---@author UjAPI
---@param whichSprite sprite
---@param textureName string
---@param materialId integer
---@param textureIndex integer
function SetSpriteMaterialTexture (whichSprite, textureName, materialId, textureIndex) end
---@author UjAPI
---@param whichSprite sprite
---@param textureName string
---@param textureIndex integer
function SetSpriteTexture (whichSprite, textureName, textureIndex) end
---@author UjAPI
---@param whichSprite sprite
---@param textureName string
---@param textureIndex integer
function SetSpriteReplaceableTexture (whichSprite, textureName, textureIndex) end
---@author UjAPI
---@param whichSprite sprite
---@return string
function GetSpriteModel (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@param modelName string
function SetSpriteModel (whichSprite, modelName) end
---@author UjAPI
---@param whichSprite sprite
---@param modelName string
---@param playerColour integer
function SetSpriteModelEx (whichSprite, modelName, playerColour) end
-- whichObject can be bone, reference, sound, aka any object of a model
---@author UjAPI
---@param whichSprite sprite
---@param whichObject string
---@return real
function GetSpriteModelObjectX (whichSprite, whichObject) end
---@author UjAPI
---@param whichSprite sprite
---@param whichObject string
---@return real
function GetSpriteModelObjectY (whichSprite, whichObject) end
---@author UjAPI
---@param whichSprite sprite
---@param whichObject string
---@return real
function GetSpriteModelObjectZ (whichSprite, whichObject) end
---@author UjAPI
---@param whichSprite sprite
---@param whichObject string
---@return location
function GetSpriteModelObjectPositionLoc (whichSprite, whichObject) end
---@author UjAPI
---@param whichSprite sprite
---@return integer
function GetSpriteCurrentAnimationId (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@return string
function GetSpriteCurrentAnimationName (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@param animIndex integer
---@param rarity raritycontrol
function SetSpriteAnimationWithRarityByIndex (whichSprite, animIndex, rarity) end
---@author UjAPI
---@param whichSprite sprite
---@param animationName string
---@param rarity raritycontrol
function SetSpriteAnimationWithRarity (whichSprite, animationName, rarity) end
---@author UjAPI
---@param whichSprite sprite
---@param animIndex integer
function SetSpriteAnimationByIndex (whichSprite, animIndex) end
---@author UjAPI
---@param whichSprite sprite
---@param animationName string
function SetSpriteAnimation (whichSprite, animationName) end
---@author UjAPI
---@param whichSprite sprite
---@param animIndex integer
function QueueSpriteAnimationByIndex (whichSprite, animIndex) end
---@author UjAPI
---@param whichSprite sprite
---@param animationName string
function QueueSpriteAnimation (whichSprite, animationName) end
---@author UjAPI
---@param whichSprite sprite
---@return real
function GetSpriteAnimationOffsetPercent (whichSprite) end
---@author UjAPI
---@param whichSprite sprite
---@param percent real
function SetSpriteAnimationOffsetPercent (whichSprite, percent) end
-- 

-- ============================================================================
-- Doodad API
-- 
---@author UjAPI
---@param objectTypeId integer
---@param x real
---@param y real
---@param facing real
---@param scale real
---@param variation integer
---@return doodad
function CreateDoodad (objectTypeId, x, y, facing, scale, variation) end
---@author UjAPI
---@param objectTypeId integer
---@param x real
---@param y real
---@param z real
---@param facing real
---@param scale real
---@param variation integer
---@return doodad
function CreateDoodadZ (objectTypeId, x, y, z, facing, scale, variation) end
---@author UjAPI
---@param whichDoodad doodad
function RemoveDoodad (whichDoodad) end
---@author UjAPI
---@return integer
function GetDoodadCount () end
---@author UjAPI
---@param index integer
---@return doodad
function GetDoodadByIndex (index) end
---@author UjAPI
---@param whichDoodad doodad
---@return integer
function GetDoodadIndex (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@return integer
function GetDoodadColour (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param colour integer
function SetDoodadColour (whichDoodad, colour) end
---@author UjAPI
---@param whichDoodad doodad
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function SetDoodadVertexColour (whichDoodad, red, green, blue, alpha) end
---@author UjAPI
---@param whichDoodad doodad
---@return real
function GetDoodadScale (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param scale real
function SetDoodadScale (whichDoodad, scale) end
---@author UjAPI
---@param whichDoodad doodad
---@return real
function GetDoodadTimeScale (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param timeScale real
function SetDoodadTimeScale (whichDoodad, timeScale) end
---@author UjAPI
---@param whichDoodad doodad
---@return real
function GetDoodadX (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param x real
function SetDoodadX (whichDoodad, x) end
---@author UjAPI
---@param whichDoodad doodad
---@return real
function GetDoodadY (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param y real
function SetDoodadY (whichDoodad, y) end
---@author UjAPI
---@param whichDoodad doodad
---@return real
function GetDoodadZ (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param z real
function SetDoodadZ (whichDoodad, z) end
---@author UjAPI
---@param whichDoodad doodad
---@param x real
---@param y real
function SetDoodadPosition (whichDoodad, x, y) end
---@author UjAPI
---@param whichDoodad doodad
---@param x real
---@param y real
---@param z real
function SetDoodadPositionWithZ (whichDoodad, x, y, z) end
---@author UjAPI
---@param whichDoodad doodad
---@return location
function GetDoodadPositionLoc (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param whichLocation location
function SetDoodadPositionLoc (whichDoodad, whichLocation) end
---@author UjAPI
---@param whichDoodad doodad
---@return real
function GetDoodadScreenX (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@return real
function GetDoodadScreenY (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@return real
function GetDoodadMatrixScaleX (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@return real
function GetDoodadMatrixScaleY (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@return real
function GetDoodadMatrixScaleZ (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param x real
---@param y real
---@param z real
function SetDoodadMatrixScale (whichDoodad, x, y, z) end
---@author UjAPI
---@param whichDoodad doodad
function ResetDoodadMatrix (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param yaw real
---@param pitch real
---@param roll real
---@param eulerOrder integer
function SetDoodadOrientationEx (whichDoodad, yaw, pitch, roll, eulerOrder) end
---@author UjAPI
---@param whichDoodad doodad
---@return real
function GetDoodadYaw (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param yaw real
function SetDoodadYaw (whichDoodad, yaw) end
---@author UjAPI
---@param whichDoodad doodad
---@return real
function GetDoodadFacing (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param facing real
function SetDoodadFacing (whichDoodad, facing) end
---@author UjAPI
---@param whichDoodad doodad
---@return real
function GetDoodadPitch (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param pitch real
function SetDoodadPitch (whichDoodad, pitch) end
---@author UjAPI
---@param whichDoodad doodad
---@return real
function GetDoodadRoll (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param roll real
function SetDoodadRoll (whichDoodad, roll) end
---@author UjAPI
---@param whichDoodad doodad
---@param yaw real
---@param pitch real
---@param roll real
function SetDoodadOrientation (whichDoodad, yaw, pitch, roll) end
---@author UjAPI
---@param whichDoodad doodad
---@return playercolor
function GetDoodadPlayerColour (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param color playercolor
function SetDoodadPlayerColour (whichDoodad, color) end
---@author UjAPI
---@param whichDoodad doodad
---@return string
function GetDoodadModel (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param modelFile string
function SetDoodadModel (whichDoodad, modelFile) end
---@author UjAPI
---@param whichDoodad doodad
---@param modelFile string
---@param playerId integer
function SetDoodadModelEx (whichDoodad, modelFile, playerId) end
---@author UjAPI
---@param whichDoodad doodad
---@param textureName string
---@param materialId integer
---@param textureIndex integer
function SetDoodadMaterialTexture (whichDoodad, textureName, materialId, textureIndex) end
---@author UjAPI
---@param whichDoodad doodad
---@param textureName string
---@param textureIndex integer
function SetDoodadTexture (whichDoodad, textureName, textureIndex) end
---@author UjAPI
---@param whichDoodad doodad
---@param textureName string
---@param textureIndex integer
function SetDoodadReplaceableTexture (whichDoodad, textureName, textureIndex) end
---@author UjAPI
---@param whichDoodad doodad
---@return boolean
function IsDoodadVisible (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param isShow boolean
function ShowDoodad (whichDoodad, isShow) end
---@author UjAPI
---@param whichDoodad doodad
---@param animIndex integer
---@param rarity raritycontrol
function SetDoodadAnimationWithRarityByIndex (whichDoodad, animIndex, rarity) end
---@author UjAPI
---@param whichDoodad doodad
---@param animationName string
---@param rarity raritycontrol
function SetDoodadAnimationWithRarity (whichDoodad, animationName, rarity) end
---@author UjAPI
---@param whichDoodad doodad
---@param animIndex integer
function SetDoodadAnimationByIndex (whichDoodad, animIndex) end
---@author UjAPI
---@param whichDoodad doodad
---@param animationName string
function SetDoodadAnimationEx (whichDoodad, animationName) end
---@author UjAPI
---@param whichDoodad doodad
---@param animIndex integer
function QueueDoodadAnimationByIndex (whichDoodad, animIndex) end
---@author UjAPI
---@param whichDoodad doodad
---@param animationName string
function QueueDoodadAnimation (whichDoodad, animationName) end
---@author UjAPI
---@param whichDoodad doodad
---@return real
function GetDoodadAnimationOffsetPercent (whichDoodad) end
---@author UjAPI
---@param whichDoodad doodad
---@param percent real
function SetDoodadAnimationOffsetPercent (whichDoodad, percent) end

---@author UjAPI
---@return doodad
function GetFilterDoodad () end
---@author UjAPI
---@return doodad
function GetEnumDoodad () end

---@author UjAPI
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
---@param handlerFunc code
function EnumDoodadsInRange (x, y, radius, filter, handlerFunc) end
---@author UjAPI
---@param x real
---@param y real
---@param radius real
---@param typeId integer
---@param nearestOnly boolean
---@param filter boolexpr
---@param handlerFunc code
function EnumDoodadsInRangeEx (x, y, radius, typeId, nearestOnly, filter, handlerFunc) end

---@author UjAPI
---@param whichRect rect
---@param filter boolexpr
---@param handlerFunc code
function EnumDoodadsInRect (whichRect, filter, handlerFunc) end
---@author UjAPI
---@param whichRect rect
---@param typeId integer
---@param nearestOnly boolean
---@param filter boolexpr
---@param handlerFunc code
function EnumDoodadsInRectEx (whichRect, typeId, nearestOnly, filter, handlerFunc) end
-- 

-- ============================================================================
-- SpecialEffect API
-- 
---@author UjAPI
---@param whichEffect effect
---@return sprite
function GetSpecialEffectSprite (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@return boolean
function IsSpecialEffectVisible (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@param visibility boolean
function SetSpecialEffectVisible (whichEffect, visibility) end
---@author UjAPI
---@param whichEffect effect
---@return real
function GetSpecialEffectX (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@return real
function GetSpecialEffectY (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@return real
function GetSpecialEffectZ (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@return real
function GetSpecialEffectHeight (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@return location
function GetSpecialEffectPositionLoc (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@param x real
---@param y real
function SetSpecialEffectPosition (whichEffect, x, y) end
---@author UjAPI
---@param whichEffect effect
---@param x real
---@param y real
---@param z real
function SetSpecialEffectPositionWithZ (whichEffect, x, y, z) end
---@author UjAPI
---@param whichEffect effect
---@param loc location
function SetSpecialEffectPositionLoc (whichEffect, loc) end
---@author UjAPI
---@param whichEffect effect
---@param x real
function SetSpecialEffectX (whichEffect, x) end
---@author UjAPI
---@param whichEffect effect
---@param y real
function SetSpecialEffectY (whichEffect, y) end
---@author UjAPI
---@param whichEffect effect
---@param z real
function SetSpecialEffectZ (whichEffect, z) end
---@author UjAPI
---@param whichEffect effect
---@param height real
function SetSpecialEffectHeight (whichEffect, height) end
---@author UjAPI
---@param whichEffect effect
---@return real
function GetSpecialEffectScreenX (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@return real
function GetSpecialEffectScreenY (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@return real
function GetSpecialEffectScale (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@param scale real
function SetSpecialEffectScale (whichEffect, scale) end
---@author UjAPI
---@param whichEffect effect
---@return real
function GetSpecialEffectTimeScale (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@param timescale real
function SetSpecialEffectTimeScale (whichEffect, timescale) end
---@author UjAPI
---@param whichEffect effect
---@return playercolor
function GetSpecialEffectPlayerColour (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@param color playercolor
function SetSpecialEffectPlayerColour (whichEffect, color) end
---@author UjAPI
---@param whichEffect effect
---@return integer
function GetSpecialEffectColour (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@param colour integer
function SetSpecialEffectColour (whichEffect, colour) end
---@author UjAPI
---@param whichEffect effect
---@param alpha integer
function SetSpecialEffectAlpha (whichEffect, alpha) end
---@author UjAPI
---@param whichEffect effect
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function SetSpecialEffectVertexColour (whichEffect, red, green, blue, alpha) end
---@author UjAPI
---@param whichEffect effect
---@return real
function GetSpecialEffectMatrixScaleX (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@return real
function GetSpecialEffectMatrixScaleY (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@return real
function GetSpecialEffectMatrixScaleZ (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@param x real
---@param y real
---@param z real
function SetSpecialEffectMatrixScale (whichEffect, x, y, z) end
---@author UjAPI
---@param whichEffect effect
function ResetSpecialEffectMatrix (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@param yaw real
---@param pitch real
---@param roll real
---@param eulerOrder integer
function SetSpecialEffectOrientationEx (whichEffect, yaw, pitch, roll, eulerOrder) end
---@author UjAPI
---@param whichEffect effect
---@return real
function GetSpecialEffectYaw (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@param yaw real
function SetSpecialEffectYaw (whichEffect, yaw) end
---@author UjAPI
---@param whichEffect effect
---@return real
function GetSpecialEffectFacing (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@param facing real
function SetSpecialEffectFacing (whichEffect, facing) end
---@author UjAPI
---@param whichEffect effect
---@return real
function GetSpecialEffectPitch (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@param pitch real
function SetSpecialEffectPitch (whichEffect, pitch) end
---@author UjAPI
---@param whichEffect effect
---@return real
function GetSpecialEffectRoll (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@param roll real
function SetSpecialEffectRoll (whichEffect, roll) end
---@author UjAPI
---@param whichEffect effect
---@param yaw real
---@param pitch real
---@param roll real
function SetSpecialEffectOrientation (whichEffect, yaw, pitch, roll) end
---@author UjAPI
---@param whichEffect effect
---@param textureName string
---@param materialId integer
---@param textureIndex integer
function SetSpecialEffectMaterialTexture (whichEffect, textureName, materialId, textureIndex) end
---@author UjAPI
---@param whichEffect effect
---@param textureName string
---@param textureIndex integer
function SetSpecialEffectTexture (whichEffect, textureName, textureIndex) end
---@author UjAPI
---@param whichEffect effect
---@param textureName string
---@param textureIndex integer
function SetSpecialEffectReplaceableTexture (whichEffect, textureName, textureIndex) end
---@author UjAPI
---@param whichEffect effect
---@return string
function GetSpecialEffectModel (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@param modelName string
function SetSpecialEffectModel (whichEffect, modelName) end
---@author UjAPI
---@param whichEffect effect
---@param modelName string
---@param playerColour integer
function SetSpecialEffectModelEx (whichEffect, modelName, playerColour) end
---@author UjAPI
---@param whichEffect effect
---@param whichObject string
---@return real
function GetSpecialEffectModelObjectX (whichEffect, whichObject) end
---@author UjAPI
---@param whichEffect effect
---@param whichObject string
---@return real
function GetSpecialEffectModelObjectY (whichEffect, whichObject) end
---@author UjAPI
---@param whichEffect effect
---@param whichObject string
---@return real
function GetSpecialEffectModelObjectZ (whichEffect, whichObject) end
---@author UjAPI
---@param whichEffect effect
---@param whichObject string
---@return location
function GetSpecialEffectModelObjectPositionLoc (whichEffect, whichObject) end
---@author UjAPI
---@param whichEffect effect
---@return integer
function GetSpecialEffectCurrentAnimationId (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@return string
function GetSpecialEffectCurrentAnimationName (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@param animIndex integer
---@param rarity raritycontrol
function SetSpecialEffectAnimationWithRarityByIndex (whichEffect, animIndex, rarity) end
---@author UjAPI
---@param whichEffect effect
---@param animationName string
---@param rarity raritycontrol
function SetSpecialEffectAnimationWithRarity (whichEffect, animationName, rarity) end
---@author UjAPI
---@param whichEffect effect
---@param animIndex integer
function SetSpecialEffectAnimationByIndex (whichEffect, animIndex) end
---@author UjAPI
---@param whichEffect effect
---@param animationName string
function SetSpecialEffectAnimation (whichEffect, animationName) end
---@author UjAPI
---@param whichEffect effect
---@param animIndex integer
function QueueSpecialEffectAnimationByIndex (whichEffect, animIndex) end
---@author UjAPI
---@param whichEffect effect
---@param animationName string
function QueueSpecialEffectAnimation (whichEffect, animationName) end
---@author UjAPI
---@param whichEffect effect
---@return real
function GetSpecialEffectAnimationOffsetPercent (whichEffect) end
---@author UjAPI
---@param whichEffect effect
---@param percent real
function SetSpecialEffectAnimationOffsetPercent (whichEffect, percent) end

---@author UjAPI
---@return effect
function GetTriggerSpecialEffect () end
---@author UjAPI
---@return effect
function GetFilterSpecialEffect () end
---@author UjAPI
---@return effect
function GetEnumSpecialEffect () end

---@author UjAPI
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
---@param handlerFunc code
function EnumSpecialEffectsInRange (x, y, radius, filter, handlerFunc) end
-- 

-- ============================================================================
-- Trackable API
-- Since trackables are extension of effects, all the functions do exactly the same thing.
-- 
---@author UjAPI
---@param whichTrackable trackable
---@return sprite
function GetTrackableSprite (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@return boolean
function IsTrackableVisible (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@param visibility boolean
function SetTrackableVisible (whichTrackable, visibility) end
---@author UjAPI
---@param whichTrackable trackable
---@return real
function GetTrackableX (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@return real
function GetTrackableY (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@return real
function GetTrackableZ (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@return real
function GetTrackableHeight (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@return location
function GetTrackablePositionLoc (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@param x real
---@param y real
function SetTrackablePosition (whichTrackable, x, y) end
---@author UjAPI
---@param whichTrackable trackable
---@param x real
---@param y real
---@param z real
function SetTrackablePositionWithZ (whichTrackable, x, y, z) end
---@author UjAPI
---@param whichTrackable trackable
---@param loc location
function SetTrackablePositionLoc (whichTrackable, loc) end
---@author UjAPI
---@param whichTrackable trackable
---@param x real
function SetTrackableX (whichTrackable, x) end
---@author UjAPI
---@param whichTrackable trackable
---@param y real
function SetTrackableY (whichTrackable, y) end
---@author UjAPI
---@param whichTrackable trackable
---@param z real
function SetTrackableZ (whichTrackable, z) end
---@author UjAPI
---@param whichTrackable trackable
---@param height real
function SetTrackableHeight (whichTrackable, height) end
---@author UjAPI
---@param whichTrackable trackable
---@return real
function GetTrackableScreenX (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@return real
function GetTrackableScreenY (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@return real
function GetTrackableScale (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@param scale real
function SetTrackableScale (whichTrackable, scale) end
---@author UjAPI
---@param whichTrackable trackable
---@return real
function GetTrackableTimeScale (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@param timescale real
function SetTrackableTimeScale (whichTrackable, timescale) end
---@author UjAPI
---@param whichTrackable trackable
---@return playercolor
function GetTrackablePlayerColour (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@param color playercolor
function SetTrackablePlayerColour (whichTrackable, color) end
---@author UjAPI
---@param whichTrackable trackable
---@return integer
function GetTrackableColour (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@param colour integer
function SetTrackableColour (whichTrackable, colour) end
---@author UjAPI
---@param whichTrackable trackable
---@param alpha integer
function SetTrackableAlpha (whichTrackable, alpha) end
---@author UjAPI
---@param whichTrackable trackable
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function SetTrackableVertexColour (whichTrackable, red, green, blue, alpha) end
---@author UjAPI
---@param whichTrackable trackable
---@return real
function GetTrackableMatrixScaleX (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@return real
function GetTrackableMatrixScaleY (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@return real
function GetTrackableMatrixScaleZ (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@param x real
---@param y real
---@param z real
function SetTrackableMatrixScale (whichTrackable, x, y, z) end
---@author UjAPI
---@param whichTrackable trackable
function ResetTrackableMatrix (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@param yaw real
---@param pitch real
---@param roll real
---@param eulerOrder integer
function SetTrackableOrientationEx (whichTrackable, yaw, pitch, roll, eulerOrder) end
---@author UjAPI
---@param whichTrackable trackable
---@return real
function GetTrackableYaw (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@param yaw real
function SetTrackableYaw (whichTrackable, yaw) end
---@author UjAPI
---@param whichTrackable trackable
---@return real
function GetTrackableFacing (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@param facing real
function SetTrackableFacing (whichTrackable, facing) end
---@author UjAPI
---@param whichTrackable trackable
---@return real
function GetTrackablePitch (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@param pitch real
function SetTrackablePitch (whichTrackable, pitch) end
---@author UjAPI
---@param whichTrackable trackable
---@return real
function GetTrackableRoll (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@param roll real
function SetTrackableRoll (whichTrackable, roll) end
---@author UjAPI
---@param whichTrackable trackable
---@param yaw real
---@param pitch real
---@param roll real
function SetTrackableOrientation (whichTrackable, yaw, pitch, roll) end
---@author UjAPI
---@param whichTrackable trackable
---@param textureName string
---@param materialId integer
---@param textureIndex integer
function SetTrackableMaterialTexture (whichTrackable, textureName, materialId, textureIndex) end
---@author UjAPI
---@param whichTrackable trackable
---@param textureName string
---@param textureIndex integer
function SetTrackableTexture (whichTrackable, textureName, textureIndex) end
---@author UjAPI
---@param whichTrackable trackable
---@param textureName string
---@param textureIndex integer
function SetTrackableReplaceableTexture (whichTrackable, textureName, textureIndex) end
---@author UjAPI
---@param whichTrackable trackable
---@return string
function GetTrackableModel (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@param modelName string
function SetTrackableModel (whichTrackable, modelName) end
---@author UjAPI
---@param whichTrackable trackable
---@param modelName string
---@param playerColour integer
function SetTrackableModelEx (whichTrackable, modelName, playerColour) end
---@author UjAPI
---@param whichTrackable trackable
---@param whichObject string
---@return real
function GetTrackableModelObjectX (whichTrackable, whichObject) end
---@author UjAPI
---@param whichTrackable trackable
---@param whichObject string
---@return real
function GetTrackableModelObjectY (whichTrackable, whichObject) end
---@author UjAPI
---@param whichTrackable trackable
---@param whichObject string
---@return real
function GetTrackableModelObjectZ (whichTrackable, whichObject) end
---@author UjAPI
---@param whichTrackable trackable
---@param whichObject string
---@return location
function GetTrackableModelObjectPositionLoc (whichTrackable, whichObject) end
---@author UjAPI
---@param whichTrackable trackable
---@return integer
function GetTrackableCurrentAnimationId (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@return string
function GetTrackableCurrentAnimationName (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@param animIndex integer
---@param rarity raritycontrol
function SetTrackableAnimationWithRarityByIndex (whichTrackable, animIndex, rarity) end
---@author UjAPI
---@param whichTrackable trackable
---@param animationName string
---@param rarity raritycontrol
function SetTrackableAnimationWithRarity (whichTrackable, animationName, rarity) end
---@author UjAPI
---@param whichTrackable trackable
---@param animIndex integer
function SetTrackableAnimationByIndex (whichTrackable, animIndex) end
---@author UjAPI
---@param whichTrackable trackable
---@param animationName string
function SetTrackableAnimation (whichTrackable, animationName) end
---@author UjAPI
---@param whichTrackable trackable
---@param animIndex integer
function QueueTrackableAnimationByIndex (whichTrackable, animIndex) end
---@author UjAPI
---@param whichTrackable trackable
---@param animationName string
function QueueTrackableAnimation (whichTrackable, animationName) end
---@author UjAPI
---@param whichTrackable trackable
---@return real
function GetTrackableAnimationOffsetPercent (whichTrackable) end
---@author UjAPI
---@param whichTrackable trackable
---@param percent real
function SetTrackableAnimationOffsetPercent (whichTrackable, percent) end

---@author UjAPI
---@return trackable
function GetTriggerTrackable () end
---@author UjAPI
---@return trackable
function GetFilterTrackable () end
---@author UjAPI
---@return trackable
function GetEnumTrackable () end

---@author UjAPI
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
---@param handlerFunc code
function EnumTrackablesInRange (x, y, radius, filter, handlerFunc) end
-- 

-- ============================================================================
-- Widget API
-- 
---@author UjAPI
---@return widget
function GetWidgetUnderCursor () end
---@author UjAPI
---@param whichWidget widget
---@return sprite
function GetWidgetSprite (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@return integer
function GetWidgetTypeId (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@return string
function GetWidgetName (whichWidget) end
---@author UjAPI
---@return boolean
function IsWidgetTipEnabled () end
---@author UjAPI
---@param enable boolean
function SetWidgetTipEnabled (enable) end
---@author UjAPI
---@param whichWidget widget
---@return boolean
function IsWidgetVisible (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param visible boolean
function SetWidgetVisible (whichWidget, visible) end
---@author UjAPI
---@param whichWidget widget
---@return boolean
function IsWidgetInvulnerable (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param invulnerable boolean
function SetWidgetInvulnerable (whichWidget, invulnerable) end
---@author UjAPI
---@param whichWidget widget
---@param target widget
---@param whichFlags targetflag
---@return boolean
function IsWidgetTargetAllowed (whichWidget, target, whichFlags) end
---@author UjAPI
---@param whichWidget widget
---@return real
function GetWidgetZ (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@return location
function GetWidgetPositionLoc (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param whichLocation location
function SetWidgetPositionLoc (whichWidget, whichLocation) end
---@author UjAPI
---@param whichWidget widget
---@param x real
---@param y real
function SetWidgetPosition (whichWidget, x, y) end
---@author UjAPI
---@param whichWidget widget
---@param x real
---@param y real
---@param z real
function SetWidgetPositionWithZ (whichWidget, x, y, z) end
---@author UjAPI
---@param whichWidget widget
---@param x real
function SetWidgetX (whichWidget, x) end
---@author UjAPI
---@param whichWidget widget
---@param y real
function SetWidgetY (whichWidget, y) end
---@author UjAPI
---@param whichWidget widget
---@param z real
function SetWidgetZ (whichWidget, z) end
---@author UjAPI
---@param whichWidget widget
function ResetWidgetZ (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@return real
function GetWidgetHeight (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param height real
function SetWidgetHeight (whichWidget, height) end
---@author UjAPI
---@param whichWidget widget
---@return real
function GetWidgetScreenX (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@return real
function GetWidgetScreenY (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@return playercolor
function GetWidgetPlayerColour (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param color playercolor
function SetWidgetPlayerColour (whichWidget, color) end
---@author UjAPI
---@param whichWidget widget
---@return integer
function GetWidgetVertexColour (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function SetWidgetVertexColour (whichWidget, red, green, blue, alpha) end
---@author UjAPI
---@param whichWidget widget
---@return real
function GetWidgetTimeScale (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param timeScale real
function SetWidgetTimeScale (whichWidget, timeScale) end
---@author UjAPI
---@param whichWidget widget
---@return real
function GetWidgetScale (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param scale real
function SetWidgetScale (whichWidget, scale) end
---@author UjAPI
---@param whichWidget widget
---@return real
function GetWidgetFacing (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param facing real
---@param isInstant boolean
function SetWidgetFacing (whichWidget, facing, isInstant) end
---@author UjAPI
---@param whichWidget widget
---@return real
function GetWidgetMatrixScaleX (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@return real
function GetWidgetMatrixScaleY (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@return real
function GetWidgetMatrixScaleZ (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param x real
---@param y real
---@param z real
function SetWidgetMatrixScale (whichWidget, x, y, z) end
---@author UjAPI
---@param whichWidget widget
function ResetWidgetMatrix (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param yaw real
---@param pitch real
---@param roll real
---@param eulerOrder integer
function SetWidgetOrientationEx (whichWidget, yaw, pitch, roll, eulerOrder) end
---@author UjAPI
---@param whichWidget widget
---@param yaw real
---@param pitch real
---@param roll real
function SetWidgetOrientation (whichWidget, yaw, pitch, roll) end
---@author UjAPI
---@param whichWidget widget
---@return real
function GetWidgetYaw (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param yaw real
function SetWidgetYaw (whichWidget, yaw) end
---@author UjAPI
---@param whichWidget widget
---@return real
function GetWidgetPitch (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param pitch real
function SetWidgetPitch (whichWidget, pitch) end
---@author UjAPI
---@param whichWidget widget
---@return real
function GetWidgetRoll (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param roll real
function SetWidgetRoll (whichWidget, roll) end
---@author UjAPI
---@param whichWidget widget
---@return string
function GetWidgetModel (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param modelFile string
function SetWidgetModel (whichWidget, modelFile) end
---@author UjAPI
---@param whichWidget widget
---@param modelFile string
---@param playerId integer
function SetWidgetModelEx (whichWidget, modelFile, playerId) end
---@author UjAPI
---@param whichWidget widget
---@param textureName string
---@param materialId integer
---@param textureIndex integer
function SetWidgetMaterialTexture (whichWidget, textureName, materialId, textureIndex) end
---@author UjAPI
---@param whichWidget widget
---@param textureName string
---@param textureIndex integer
function SetWidgetTexture (whichWidget, textureName, textureIndex) end
---@author UjAPI
---@param whichWidget widget
---@param textureName string
---@param textureIndex integer
function SetWidgetReplaceableTexture (whichWidget, textureName, textureIndex) end
---@author UjAPI
---@param whichWidget widget
---@param whichObject string
---@return real
function GetWidgetModelObjectX (whichWidget, whichObject) end
---@author UjAPI
---@param whichWidget widget
---@param whichObject string
---@return real
function GetWidgetModelObjectY (whichWidget, whichObject) end
---@author UjAPI
---@param whichWidget widget
---@param whichObject string
---@return real
function GetWidgetModelObjectZ (whichWidget, whichObject) end
---@author UjAPI
---@param whichWidget widget
---@param whichObject string
---@return location
function GetWidgetModelObjectPositionLoc (whichWidget, whichObject) end
---@author UjAPI
---@param whichWidget widget
---@return integer
function GetWidgetCurrentAnimationId (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@return string
function GetWidgetCurrentAnimationName (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param animIndex integer
---@param rarity raritycontrol
function SetWidgetAnimationWithRarityByIndex (whichWidget, animIndex, rarity) end
---@author UjAPI
---@param whichWidget widget
---@param animationName string
---@param rarity raritycontrol
function SetWidgetAnimationWithRarity (whichWidget, animationName, rarity) end
---@author UjAPI
---@param whichWidget widget
---@param animIndex integer
function SetWidgetAnimationByIndex (whichWidget, animIndex) end
---@author UjAPI
---@param whichWidget widget
---@param animationName string
function SetWidgetAnimation (whichWidget, animationName) end
---@author UjAPI
---@param whichWidget widget
---@param animIndex integer
function QueueWidgetAnimationByIndex (whichWidget, animIndex) end
---@author UjAPI
---@param whichWidget widget
---@param animationName string
function QueueWidgetAnimation (whichWidget, animationName) end
---@author UjAPI
---@param whichWidget widget
---@return real
function GetWidgetAnimationOffsetPercent (whichWidget) end
---@author UjAPI
---@param whichWidget widget
---@param percent real
function SetWidgetAnimationOffsetPercent (whichWidget, percent) end

---@author UjAPI
---@param whichTrigger trigger
---@param whichWidget widget
---@param whichWidgetEvent widgetevent
---@return event
function TriggerRegisterWidgetEvent (whichTrigger, whichWidget, whichWidgetEvent) end
-- 

-- ============================================================================
-- Destructable API
-- 

---@author UjAPI
---@return destructable
function GetDestructableUnderCursor () end

-- Field API
---@author UjAPI
---@param whichDestructable destructable
---@param whichField destructablestringfield
---@return string
function GetDestructableStringField (whichDestructable, whichField) end
---@author UjAPI
---@param whichDestructable destructable
---@param whichField destructablestringfield
---@param value string
---@return boolean
function SetDestructableStringField (whichDestructable, whichField, value) end
-- 

---@author UjAPI
---@param whichDestructable destructable
---@return integer
function GetDestructableVariation (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param variation integer
function SetDestructableVariation (whichDestructable, variation) end
---@author UjAPI
---@param whichDestructable destructable
---@param variation integer
---@param ignoreStateCheck boolean
function SetDestructableVariationEx (whichDestructable, variation, ignoreStateCheck) end
---@author UjAPI
---@param whichDestructable destructable
---@return boolean
function IsDestructableBlighted (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param flag boolean
function SetDestructableBlighted (whichDestructable, flag) end
---@author UjAPI
---@param whichDestructable destructable
---@return sprite
function GetDestructableSprite (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@return real
function GetDestructableZ (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
function ResetDestructableZ (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@return real
function GetDestructableHeight (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param height real
function SetDestructableHeight (whichDestructable, height) end
---@author UjAPI
---@param whichDestructable destructable
---@param x real
---@param y real
---@param z real
function SetDestructablePositionWithZ (whichDestructable, x, y, z) end
---@author UjAPI
---@param whichDestructable destructable
---@param x real
---@param y real
function SetDestructablePosition (whichDestructable, x, y) end
---@author UjAPI
---@param whichDestructable destructable
---@return location
function GetDestructablePositionLoc (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param whichLocation location
function SetDestructablePositionLoc (whichDestructable, whichLocation) end
---@author UjAPI
---@param whichDestructable destructable
---@param x real
function SetDestructableX (whichDestructable, x) end
---@author UjAPI
---@param whichDestructable destructable
---@param y real
function SetDestructableY (whichDestructable, y) end
---@author UjAPI
---@param whichDestructable destructable
---@param z real
function SetDestructableZ (whichDestructable, z) end
---@author UjAPI
---@param whichDestructable destructable
---@return real
function GetDestructableScreenX (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@return real
function GetDestructableScreenY (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@return playercolor
function GetDestructablePlayerColour (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param color playercolor
function SetDestructablePlayerColour (whichDestructable, color) end
---@author UjAPI
---@param whichDestructable destructable
---@return integer
function GetDestructableVertexColour (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function SetDestructableVertexColour (whichDestructable, red, green, blue, alpha) end
---@author UjAPI
---@param whichDestructable destructable
---@return real
function GetDestructableTimeScale (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param timeScale real
function SetDestructableTimeScale (whichDestructable, timeScale) end
---@author UjAPI
---@param whichDestructable destructable
---@return real
function GetDestructableScale (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param scale real
function SetDestructableScale (whichDestructable, scale) end
---@author UjAPI
---@param whichDestructable destructable
---@return real
function GetDestructableFacing (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param facing real
---@param isInstant boolean
function SetDestructableFacing (whichDestructable, facing, isInstant) end
---@author UjAPI
---@param whichDestructable destructable
---@return real
function GetDestructableMatrixScaleX (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@return real
function GetDestructableMatrixScaleY (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@return real
function GetDestructableMatrixScaleZ (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param x real
---@param y real
---@param z real
function SetDestructableMatrixScale (whichDestructable, x, y, z) end
---@author UjAPI
---@param whichDestructable destructable
function ResetDestructableMatrix (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param yaw real
---@param pitch real
---@param roll real
---@param eulerOrder integer
function SetDestructableOrientationEx (whichDestructable, yaw, pitch, roll, eulerOrder) end
---@author UjAPI
---@param whichDestructable destructable
---@param yaw real
---@param pitch real
---@param roll real
function SetDestructableOrientation (whichDestructable, yaw, pitch, roll) end
---@author UjAPI
---@param whichDestructable destructable
---@return real
function GetDestructableYaw (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param yaw real
function SetDestructableYaw (whichDestructable, yaw) end
---@author UjAPI
---@param whichDestructable destructable
---@return real
function GetDestructablePitch (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param pitch real
function SetDestructablePitch (whichDestructable, pitch) end
---@author UjAPI
---@param whichDestructable destructable
---@return real
function GetDestructableRoll (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param roll real
function SetDestructableRoll (whichDestructable, roll) end
---@author UjAPI
---@param whichDestructable destructable
---@return string
function GetDestructableModel (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param modelFile string
function SetDestructableModel (whichDestructable, modelFile) end
---@author UjAPI
---@param whichDestructable destructable
---@param modelFile string
---@param playerId integer
function SetDestructableModelEx (whichDestructable, modelFile, playerId) end
---@author UjAPI
---@param whichDestructable destructable
---@param textureName string
---@param materialId integer
---@param textureIndex integer
function SetDestructableMaterialTexture (whichDestructable, textureName, materialId, textureIndex) end
---@author UjAPI
---@param whichDestructable destructable
---@param textureName string
---@param textureIndex integer
function SetDestructableTexture (whichDestructable, textureName, textureIndex) end
---@author UjAPI
---@param whichDestructable destructable
---@param textureName string
---@param textureIndex integer
function SetDestructableReplaceableTexture (whichDestructable, textureName, textureIndex) end
---@author UjAPI
---@param whichDestructable destructable
---@param whichObject string
---@return real
function GetDestructableModelObjectX (whichDestructable, whichObject) end
---@author UjAPI
---@param whichDestructable destructable
---@param whichObject string
---@return real
function GetDestructableModelObjectY (whichDestructable, whichObject) end
---@author UjAPI
---@param whichDestructable destructable
---@param whichObject string
---@return real
function GetDestructableModelObjectZ (whichDestructable, whichObject) end
---@author UjAPI
---@param whichDestructable destructable
---@param whichObject string
---@return location
function GetDestructableModelObjectPositionLoc (whichDestructable, whichObject) end
---@author UjAPI
---@param whichDestructable destructable
---@return integer
function GetDestructableCurrentAnimationId (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@return string
function GetDestructableCurrentAnimationName (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param animIndex integer
---@param rarity raritycontrol
function SetDestructableAnimationWithRarityByIndex (whichDestructable, animIndex, rarity) end
---@author UjAPI
---@param whichDestructable destructable
---@param animationName string
---@param rarity raritycontrol
function SetDestructableAnimationWithRarity (whichDestructable, animationName, rarity) end
---@author UjAPI
---@param whichDestructable destructable
---@param animIndex integer
function SetDestructableAnimationByIndex (whichDestructable, animIndex) end
---@author UjAPI
---@param whichDestructable destructable
---@param animIndex integer
function QueueDestructableAnimationByIndex (whichDestructable, animIndex) end
---@author UjAPI
---@param whichDestructable destructable
---@return real
function GetDestructableAnimationOffsetPercent (whichDestructable) end
---@author UjAPI
---@param whichDestructable destructable
---@param percent real
function SetDestructableAnimationOffsetPercent (whichDestructable, percent) end

---@author UjAPI
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
---@param handlerFunc code
function EnumDestructablesInRange (x, y, radius, filter, handlerFunc) end
-- 

-- ============================================================================
-- Item API
-- 

-- Base Field API
---@author UjAPI
---@param itemTypeId integer
---@param whichField itemintegerfield
---@return integer
function GetBaseItemIntegerFieldById (itemTypeId, whichField) end
---@author UjAPI
---@param itemTypeId integer
---@param whichField itemintegerfield
---@param value integer
---@return boolean
function SetBaseItemIntegerFieldById (itemTypeId, whichField, value) end

---@author UjAPI
---@param itemTypeId integer
---@param whichField itembooleanfield
---@return boolean
function GetBaseItemBooleanFieldById (itemTypeId, whichField) end
---@author UjAPI
---@param itemTypeId integer
---@param whichField itembooleanfield
---@param value boolean
---@return boolean
function SetBaseItemBooleanFieldById (itemTypeId, whichField, value) end

---@author UjAPI
---@param itemTypeId integer
---@param whichField itemrealfield
---@return real
function GetBaseItemRealFieldById (itemTypeId, whichField) end
---@author UjAPI
---@param itemTypeId integer
---@param whichField itemrealfield
---@param value real
---@return boolean
function SetBaseItemRealFieldById (itemTypeId, whichField, value) end

---@author UjAPI
---@param itemTypeId integer
---@param whichField itemstringfield
---@return string
function GetBaseItemStringFieldById (itemTypeId, whichField) end
---@author UjAPI
---@param itemTypeId integer
---@param whichField itemstringfield
---@param value string
---@return boolean
function SetBaseItemStringFieldById (itemTypeId, whichField, value) end
-- 

-- Field API
---@author UjAPI
---@param whichItem item
---@param whichField itemintegerfield
---@return integer
function GetItemIntegerField (whichItem, whichField) end
---@author UjAPI
---@param whichItem item
---@param whichField itemintegerfield
---@param value integer
---@return boolean
function SetItemIntegerField (whichItem, whichField, value) end

---@author UjAPI
---@param whichItem item
---@param whichField itembooleanfield
---@return boolean
function GetItemBooleanField (whichItem, whichField) end
---@author UjAPI
---@param whichItem item
---@param whichField itembooleanfield
---@param value boolean
---@return boolean
function SetItemBooleanField (whichItem, whichField, value) end

---@author UjAPI
---@param whichItem item
---@param whichField itemrealfield
---@return real
function GetItemRealField (whichItem, whichField) end
---@author UjAPI
---@param whichItem item
---@param whichField itemrealfield
---@param value real
---@return boolean
function SetItemRealField (whichItem, whichField, value) end

---@author UjAPI
---@param whichItem item
---@param whichField itemstringfield
---@return string
function GetItemStringField (whichItem, whichField) end
---@author UjAPI
---@param whichItem item
---@param whichField itemstringfield
---@param value string
---@return boolean
function SetItemStringField (whichItem, whichField, value) end
-- 

-- Normal API
---@author UjAPI
---@return item
function GetTriggerItem () end
---@author UjAPI
---@return item
function GetItemUnderCursor () end
---@author UjAPI
---@param whichItem item
---@return unit
function GetItemOwner (whichItem) end
---@author UjAPI
---@param whichItem item
---@return boolean
function IsItemDroppable (whichItem) end
---@author UjAPI
---@param whichItem item
---@return sprite
function GetItemSprite (whichItem) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemZ (whichItem) end
---@author UjAPI
---@param whichItem item
---@param x real
---@param y real
---@param z real
function SetItemPositionWithZ (whichItem, x, y, z) end
---@author UjAPI
---@param whichItem item
---@param x real
function SetItemX (whichItem, x) end
---@author UjAPI
---@param whichItem item
---@param y real
function SetItemY (whichItem, y) end
---@author UjAPI
---@param whichItem item
---@param z real
function SetItemZ (whichItem, z) end
---@author UjAPI
---@param whichItem item
function ResetItemZ (whichItem) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemHeight (whichItem) end
---@author UjAPI
---@param whichItem item
---@param height real
function SetItemHeight (whichItem, height) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemScreenX (whichItem) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemScreenY (whichItem) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemLife (whichItem) end
---@author UjAPI
---@param whichItem item
---@param life real
function SetItemLife (whichItem, life) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemMaxLife (whichItem) end
---@author UjAPI
---@param whichItem item
---@param maxLife real
function SetItemMaxLife (whichItem, maxLife) end
---@author UjAPI
---@param whichItem item
---@param abilityTypeId integer
---@return ability
function GetItemAbilityById (whichItem, abilityTypeId) end
---@author UjAPI
---@param whichItem item
---@param index integer
---@return ability
function GetItemAbilityByIndex (whichItem, index) end
---@author UjAPI
---@param whichItem item
---@param whichAbility ability
---@return boolean
function ItemAddAbility (whichItem, whichAbility) end
---@author UjAPI
---@param whichItem item
---@param whichAbility ability
---@return boolean
function ItemRemoveAbility (whichItem, whichAbility) end
---@author UjAPI
---@param whichItem item
---@param abilityTypeId integer
---@return boolean
function ItemAddAbilityById (whichItem, abilityTypeId) end
---@author UjAPI
---@param whichItem item
---@param abilityTypeId integer
---@return boolean
function ItemRemoveAbilityById (whichItem, abilityTypeId) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemCooldown (whichItem) end
---@author UjAPI
---@param whichItem item
---@param cooldown real
function SetItemCooldown (whichItem, cooldown) end
---@author UjAPI
---@param whichUnit unit
---@param whichItem item
---@param cooldown real
function StartItemCooldown (whichUnit, whichItem, cooldown) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemRemainingCooldown (whichItem) end
---@author UjAPI
---@param whichItem item
---@param cooldown real
function SetItemRemainingCooldown (whichItem, cooldown) end
---@author UjAPI
---@param whichItem item
---@return playercolor
function GetItemPlayerColour (whichItem) end
---@author UjAPI
---@param whichItem item
---@param color playercolor
function SetItemPlayerColour (whichItem, color) end
---@author UjAPI
---@param whichItem item
---@return integer
function GetItemVertexColour (whichItem) end
---@author UjAPI
---@param whichItem item
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function SetItemVertexColour (whichItem, red, green, blue, alpha) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemTimeScale (whichItem) end
---@author UjAPI
---@param whichItem item
---@param timeScale real
function SetItemTimeScale (whichItem, timeScale) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemScale (whichItem) end
---@author UjAPI
---@param whichItem item
---@param scale real
function SetItemScale (whichItem, scale) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemFacing (whichItem) end
---@author UjAPI
---@param whichItem item
---@param facing real
---@param isInstant boolean
function SetItemFacing (whichItem, facing, isInstant) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemMatrixScaleX (whichItem) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemMatrixScaleY (whichItem) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemMatrixScaleZ (whichItem) end
---@author UjAPI
---@param whichItem item
---@param x real
---@param y real
---@param z real
function SetItemMatrixScale (whichItem, x, y, z) end
---@author UjAPI
---@param whichItem item
function ResetItemMatrix (whichItem) end
---@author UjAPI
---@param whichItem item
---@param yaw real
---@param pitch real
---@param roll real
---@param eulerOrder integer
function SetItemOrientationEx (whichItem, yaw, pitch, roll, eulerOrder) end
---@author UjAPI
---@param whichItem item
---@param yaw real
---@param pitch real
---@param roll real
function SetItemOrientation (whichItem, yaw, pitch, roll) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemYaw (whichItem) end
---@author UjAPI
---@param whichItem item
---@param yaw real
function SetItemYaw (whichItem, yaw) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemPitch (whichItem) end
---@author UjAPI
---@param whichItem item
---@param pitch real
function SetItemPitch (whichItem, pitch) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemRoll (whichItem) end
---@author UjAPI
---@param whichItem item
---@param roll real
function SetItemRoll (whichItem, roll) end
---@author UjAPI
---@param whichItem item
---@return string
function GetItemModel (whichItem) end
---@author UjAPI
---@param whichItem item
---@param modelFile string
function SetItemModel (whichItem, modelFile) end
---@author UjAPI
---@param whichItem item
---@param modelFile string
---@param playerColourId integer
function SetItemModelEx (whichItem, modelFile, playerColourId) end
---@author UjAPI
---@param whichItem item
---@param textureName string
---@param materialId integer
---@param textureIndex integer
function SetItemMaterialTexture (whichItem, textureName, materialId, textureIndex) end
---@author UjAPI
---@param whichItem item
---@param textureName string
---@param textureIndex integer
function SetItemTexture (whichItem, textureName, textureIndex) end
---@author UjAPI
---@param whichItem item
---@param textureName string
---@param textureIndex integer
function SetItemReplaceableTexture (whichItem, textureName, textureIndex) end
---@author UjAPI
---@param whichItem item
---@param whichObject string
---@return real
function GetItemModelObjectX (whichItem, whichObject) end
---@author UjAPI
---@param whichItem item
---@param whichObject string
---@return real
function GetItemModelObjectY (whichItem, whichObject) end
---@author UjAPI
---@param whichItem item
---@param whichObject string
---@return real
function GetItemModelObjectZ (whichItem, whichObject) end
---@author UjAPI
---@param whichItem item
---@param whichObject string
---@return location
function GetItemModelObjectPositionLoc (whichItem, whichObject) end
---@author UjAPI
---@param whichItem item
---@return integer
function GetItemCurrentAnimationId (whichItem) end
---@author UjAPI
---@param whichItem item
---@return string
function GetItemCurrentAnimationName (whichItem) end
---@author UjAPI
---@param whichItem item
---@param animIndex integer
---@param rarity raritycontrol
function SetItemAnimationWithRarityByIndex (whichItem, animIndex, rarity) end
---@author UjAPI
---@param whichItem item
---@param animationName string
---@param rarity raritycontrol
function SetItemAnimationWithRarity (whichItem, animationName, rarity) end
---@author UjAPI
---@param whichItem item
---@param animIndex integer
function SetItemAnimationByIndex (whichItem, animIndex) end
---@author UjAPI
---@param whichItem item
---@param animationName string
function SetItemAnimation (whichItem, animationName) end
---@author UjAPI
---@param whichItem item
---@param animIndex integer
function QueueItemAnimationByIndex (whichItem, animIndex) end
---@author UjAPI
---@param whichItem item
---@param animationName string
function QueueItemAnimation (whichItem, animationName) end
---@author UjAPI
---@param whichItem item
---@return real
function GetItemAnimationOffsetPercent (whichItem) end
---@author UjAPI
---@param whichItem item
---@param percent real
function SetItemAnimationOffsetPercent (whichItem, percent) end

---@author UjAPI
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
---@param handlerFunc code
function EnumItemsInRange (x, y, radius, filter, handlerFunc) end
-- 
-- 

-- ============================================================================
-- Unit API
-- 

-- Base Field API
-- Unit Base
---@author UjAPI
---@param unitTypeId integer
---@param whichField unitintegerfield
---@return integer
function GetUnitBaseIntegerFieldById (unitTypeId, whichField) end
---@author UjAPI
---@param unitTypeId integer
---@param whichField unitintegerfield
---@param value integer
---@return boolean
function SetUnitBaseIntegerFieldById (unitTypeId, whichField, value) end

---@author UjAPI
---@param unitTypeId integer
---@param whichField unitbooleanfield
---@return boolean
function GetUnitBaseBooleanFieldById (unitTypeId, whichField) end
---@author UjAPI
---@param unitTypeId integer
---@param whichField unitbooleanfield
---@param value boolean
---@return boolean
function SetUnitBaseBooleanFieldById (unitTypeId, whichField, value) end

---@author UjAPI
---@param unitTypeId integer
---@param whichField unitrealfield
---@return real
function GetUnitBaseRealFieldById (unitTypeId, whichField) end
---@author UjAPI
---@param unitTypeId integer
---@param whichField unitrealfield
---@param value real
---@return boolean
function SetUnitBaseRealFieldById (unitTypeId, whichField, value) end

---@author UjAPI
---@param unitTypeId integer
---@param whichField unitstringfield
---@return string
function GetUnitBaseStringFieldById (unitTypeId, whichField) end
---@author UjAPI
---@param unitTypeId integer
---@param whichField unitstringfield
---@param value string
---@return boolean
function SetUnitBaseStringFieldById (unitTypeId, whichField, value) end

-- Unit Base Weapon
---@author UjAPI
---@param unitTypeId integer
---@param whichField unitweaponintegerfield
---@param index integer
---@return integer
function GetUnitBaseWeaponIntegerFieldById (unitTypeId, whichField, index) end
---@author UjAPI
---@param unitTypeId integer
---@param whichField unitweaponintegerfield
---@param index integer
---@param value integer
---@return boolean
function SetUnitBaseWeaponIntegerFieldById (unitTypeId, whichField, index, value) end

---@author UjAPI
---@param unitTypeId integer
---@param whichField unitweaponbooleanfield
---@param index integer
---@return boolean
function GetUnitBaseWeaponBooleanFieldById (unitTypeId, whichField, index) end
---@author UjAPI
---@param unitTypeId integer
---@param whichField unitweaponbooleanfield
---@param index integer
---@param value boolean
---@return boolean
function SetUnitBaseWeaponBooleanFieldById (unitTypeId, whichField, index, value) end

---@author UjAPI
---@param unitTypeId integer
---@param whichField unitweaponrealfield
---@param index integer
---@return real
function GetUnitBaseWeaponRealFieldById (unitTypeId, whichField, index) end
---@author UjAPI
---@param unitTypeId integer
---@param whichField unitweaponrealfield
---@param index integer
---@param value real
---@return boolean
function SetUnitBaseWeaponRealFieldById (unitTypeId, whichField, index, value) end

---@author UjAPI
---@param unitTypeId integer
---@param whichField unitweaponstringfield
---@param index integer
---@return string
function GetUnitBaseWeaponStringFieldById (unitTypeId, whichField, index) end
---@author UjAPI
---@param unitTypeId integer
---@param whichField unitweaponstringfield
---@param index integer
---@param value string
---@return boolean
function SetUnitBaseWeaponStringFieldById (unitTypeId, whichField, index, value) end
-- 

-- Field API
-- Unit
---@author UjAPI
---@param whichUnit unit
---@param whichField unitintegerfield
---@return integer
function GetUnitIntegerField (whichUnit, whichField) end
---@author UjAPI
---@param whichUnit unit
---@param whichField unitintegerfield
---@param value integer
---@return boolean
function SetUnitIntegerField (whichUnit, whichField, value) end

---@author UjAPI
---@param whichUnit unit
---@param whichField unitbooleanfield
---@return boolean
function GetUnitBooleanField (whichUnit, whichField) end
---@author UjAPI
---@param whichUnit unit
---@param whichField unitbooleanfield
---@param value boolean
---@return boolean
function SetUnitBooleanField (whichUnit, whichField, value) end

---@author UjAPI
---@param whichUnit unit
---@param whichField unitrealfield
---@return real
function GetUnitRealField (whichUnit, whichField) end
---@author UjAPI
---@param whichUnit unit
---@param whichField unitrealfield
---@param value real
---@return boolean
function SetUnitRealField (whichUnit, whichField, value) end

---@author UjAPI
---@param whichUnit unit
---@param whichField unitstringfield
---@return string
function GetUnitStringField (whichUnit, whichField) end
---@author UjAPI
---@param whichUnit unit
---@param whichField unitstringfield
---@param value string
---@return boolean
function SetUnitStringField (whichUnit, whichField, value) end

-- Unit Weapon
---@author UjAPI
---@param whichUnit unit
---@param whichField unitweaponintegerfield
---@param index integer
---@return integer
function GetUnitWeaponIntegerField (whichUnit, whichField, index) end
---@author UjAPI
---@param whichUnit unit
---@param whichField unitweaponintegerfield
---@param index integer
---@param value integer
---@return boolean
function SetUnitWeaponIntegerField (whichUnit, whichField, index, value) end

---@author UjAPI
---@param whichUnit unit
---@param whichField unitweaponbooleanfield
---@param index integer
---@return boolean
function GetUnitWeaponBooleanField (whichUnit, whichField, index) end
---@author UjAPI
---@param whichUnit unit
---@param whichField unitweaponbooleanfield
---@param index integer
---@param value boolean
---@return boolean
function SetUnitWeaponBooleanField (whichUnit, whichField, index, value) end

---@author UjAPI
---@param whichUnit unit
---@param whichField unitweaponrealfield
---@param index integer
---@return real
function GetUnitWeaponRealField (whichUnit, whichField, index) end
---@author UjAPI
---@param whichUnit unit
---@param whichField unitweaponrealfield
---@param index integer
---@param value real
---@return boolean
function SetUnitWeaponRealField (whichUnit, whichField, index, value) end

---@author UjAPI
---@param whichUnit unit
---@param whichField unitweaponstringfield
---@param index integer
---@return string
function GetUnitWeaponStringField (whichUnit, whichField, index) end
---@author UjAPI
---@param whichUnit unit
---@param whichField unitweaponstringfield
---@param index integer
---@param value string
---@return boolean
function SetUnitWeaponStringField (whichUnit, whichField, index, value) end
-- 

-- Normal API
---@author UjAPI
---@return unit
function GetUnitUnderCursor () end
---@author UjAPI
---@param whichUnit unit
---@return sprite
function GetUnitSprite (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param breakOrder boolean
---@param checkPathing boolean
---@param x real
---@param y real
---@param z real
function SetUnitPositionEx (whichUnit, breakOrder, checkPathing, x, y, z) end
---@author UjAPI
---@param whichUnit unit
---@param x real
---@param y real
---@param z real
function SetUnitPositionWithZ (whichUnit, x, y, z) end
---@author UjAPI
---@param whichUnit unit
---@param z real
function SetUnitZ (whichUnit, z) end
---@author UjAPI
---@param whichUnit unit
function ResetUnitZ (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitScale (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitHeight (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param height real
function SetUnitHeight (whichUnit, height) end
---@param whichUnit unit
---@return real
function GetUnitStatbarHeight (whichUnit) end
---@param whichUnit unit
---@param height real
function SetUnitStatbarHeight (whichUnit, height) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitScreenX (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitScreenY (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param newId integer
function SetUnitTypeId (whichUnit, newId) end
---@author UjAPI
---@param whichPlayer player
---@return integer
function GetUnitSelectedCountByPlayer (whichPlayer) end
---@author UjAPI
---@param whichPlayer player
---@return unit
function GetUnitSelected (whichPlayer) end
---@author UjAPI
---@param whichPlayer player
---@param index integer
---@return unit
function GetUnitInSelectionByIndex (whichPlayer, index) end
---@author UjAPI
---@param whichPlayer player
---@return unit
function GetFirstUnitInSelection (whichPlayer) end
---@author UjAPI
---@param whichPlayer player
---@return unit
function GetLastUnitInSelection (whichPlayer) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitAlive (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitDead (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitMoving (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitStunned (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitInvulnerable (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitHero (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitTower (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitShop (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitInventoryEnabled (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param enable boolean
---@param ignoreErrorMessages boolean
function UnitEnableInventory (whichUnit, enable, ignoreErrorMessages) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitMovementEnabled (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param enable boolean
---@param fullDisable boolean
function UnitEnableMovement (whichUnit, enable, fullDisable) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitAttackEnabled (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param enable boolean
---@param extraFlag boolean
function UnitEnableAttack (whichUnit, enable, extraFlag) end
---@author UjAPI
---@param whichUnit unit
---@param additionalCheck boolean
---@return boolean
function IsUnitStateNormal (whichUnit, additionalCheck) end
---@author UjAPI
---@param whichUnit unit
function RedrawUnit (whichUnit) end
---@author UjAPI
---@param whichUnit unit
function UpdateUnitInfoBar (whichUnit) end
---@author UjAPI
---@param whichUnit unit
function UnitUnapplyUpgrades (whichUnit) end
---@author UjAPI
---@param whichUnit unit
function UnitApplyUpgrades (whichUnit) end

-- Unit Ability API
---@author UjAPI
---@param whichUnit unit
---@param abilityTypeId integer
---@param checkForDuplicates boolean
---@return boolean
function UnitAddAbilityEx (whichUnit, abilityTypeId, checkForDuplicates) end
---@author UjAPI
---@param whichUnit unit
---@param abilityTypeId integer
---@param removeDuplicates boolean
---@return boolean
function UnitRemoveAbilityEx (whichUnit, abilityTypeId, removeDuplicates) end

---@author UjAPI
---@param whichUnit unit
---@param alsoCountBuffs boolean
---@return integer
function CountUnitAbilities (whichUnit, alsoCountBuffs) end
---@author UjAPI
---@param whichUnit unit
---@param abilityTypeId integer
---@return ability
function GetUnitAbility (whichUnit, abilityTypeId) end
---@author UjAPI
---@param whichUnit unit
---@param abilityTypeId integer
---@param id integer
---@return ability
function GetUnitAbilityEx (whichUnit, abilityTypeId, id) end
---@author UjAPI
---@param whichUnit unit
---@param index integer
---@return ability
function GetUnitAbilityByIndex (whichUnit, index) end
---@author UjAPI
---@param whichUnit unit
---@param abilityTypeId integer
---@return boolean
function IsUnitAbilityVisible (whichUnit, abilityTypeId) end
---@author UjAPI
---@param whichUnit unit
---@param abilityTypeId integer
---@param show boolean
function ShowUnitAbility (whichUnit, abilityTypeId, show) end
---@author UjAPI
---@param whichUnit unit
---@param abilityTypeId integer
---@param show boolean
---@param checkDuplicates boolean
function ShowUnitAbilityEx (whichUnit, abilityTypeId, show, checkDuplicates) end
---@author UjAPI
---@param whichUnit unit
---@param abilityTypeId integer
---@param hide boolean
---@param disable boolean
function DisableUnitAbility (whichUnit, abilityTypeId, hide, disable) end
---@author UjAPI
---@param whichUnit unit
---@param abilityTypeId integer
---@param hide boolean
---@param disable boolean
---@param checkDuplicates boolean
function DisableUnitAbilityEx (whichUnit, abilityTypeId, hide, disable, checkDuplicates) end
---@author UjAPI
---@param whichUnit unit
---@param abilityTypeId integer
---@param show boolean
---@param enable boolean
function EnableUnitAbility (whichUnit, abilityTypeId, show, enable) end
---@author UjAPI
---@param whichUnit unit
---@param abilityTypeId integer
---@param show boolean
---@param enable boolean
---@param checkDuplicates boolean
function EnableUnitAbilityEx (whichUnit, abilityTypeId, show, enable, checkDuplicates) end
-- 

-- Unit Buff API
---@author UjAPI
---@param whichUnit unit
---@param whichBuff buff
---@return boolean
function UnitAddBuff (whichUnit, whichBuff) end
---@author UjAPI
---@param whichUnit unit
---@param whichBuff buff
---@param checkForDuplicates boolean
---@return boolean
function UnitAddBuffEx (whichUnit, whichBuff, checkForDuplicates) end
---@author UjAPI
---@param whichUnit unit
---@param buffTypeId integer
---@return boolean
function UnitAddBuffById (whichUnit, buffTypeId) end
---@author UjAPI
---@param whichUnit unit
---@param buffTypeId integer
---@param checkForDuplicates boolean
---@return boolean
function UnitAddBuffByIdEx (whichUnit, buffTypeId, checkForDuplicates) end

---@author UjAPI
---@param whichUnit unit
---@return integer
function CountUnitBuffs (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param buffTypeId integer
---@return buff
function GetUnitBuff (whichUnit, buffTypeId) end
---@author UjAPI
---@param whichUnit unit
---@param buffTypeId integer
---@param id integer
---@return buff
function GetUnitBuffEx (whichUnit, buffTypeId, id) end
---@author UjAPI
---@param whichUnit unit
---@param index integer
---@return buff
function GetUnitBuffByIndex (whichUnit, index) end
---@author UjAPI
---@param whichUnit unit
---@param buffTypeId integer
---@return integer
function GetUnitBuffLevel (whichUnit, buffTypeId) end
-- 

---@author UjAPI
---@param whichUnit unit
function UnitCancelTimedLife (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitRemainingTimedLife (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param duration real
function SetUnitRemainingTimedLife (whichUnit, duration) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitGhosted (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param state boolean
---@param transitionTime real
function SetUnitGhosted (whichUnit, state, transitionTime) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitSelectable (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param selectable boolean
function SetUnitSelectable (whichUnit, selectable) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitTargetable (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param targetable boolean
function SetUnitTargetable (whichUnit, targetable) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitTruesightImmune (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param state boolean
function SetUnitTruesightImmuneState (whichUnit, state) end
---@author UjAPI
---@param whichUnit unit
---@param whichPlayer player
---@param flag boolean
function SetUnitVisibleByPlayer (whichUnit, whichPlayer, flag) end
---@author UjAPI
---@param whichUnit unit
---@param whichPlayer player
---@param flag boolean
function SetUnitDetectableByPlayer (whichUnit, whichPlayer, flag) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitZ (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitDamageReduction (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param resistType integer
---@return real
function GetUnitMagicResistByType (whichUnit, resistType) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitEluneMagicResist (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitRunicMagicResist (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitTotalMagicResist (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitFlyHeightEnabled (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param enable boolean
function SetUnitFlyHeightEnabled (whichUnit, enable) end
-- Gathering API
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitGatherer (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return integer
function GetUnitResourceCurrent (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param amount integer
function SetUnitResourceCurrent (whichUnit, amount) end
---@author UjAPI
---@param whichUnit unit
---@param resourceType integer
---@return integer
function GetUnitResourceCapacity (whichUnit, resourceType) end
---@author UjAPI
---@param whichUnit unit
---@param resourceType integer
---@param amount integer
function SetUnitResourceCapacity (whichUnit, resourceType, amount) end
---@author UjAPI
---@param whichUnit unit
---@param resourceType integer
---@return integer
function GetUnitResourcePerGather (whichUnit, resourceType) end
---@author UjAPI
---@param whichUnit unit
---@param resourceType integer
---@param amount integer
function SetUnitResourcePerGather (whichUnit, resourceType, amount) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitResourceGatherInterval (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param interval real
function SetUnitResourceGatherInterval (whichUnit, interval) end
-- 
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitCurrentSight (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param realValue real
function SetUnitCurrentSight (whichUnit, realValue) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitAttackRemainingCooldown (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param time real
function SetUnitAttackRemainingCooldown (whichUnit, time) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitAttackRemainingDamagePoint (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param time real
function SetUnitAttackRemainingDamagePoint (whichUnit, time) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitAttackRemainingBackswing (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param time real
function SetUnitAttackRemainingBackswing (whichUnit, time) end
-- ignoreDistance only works if isInstant is set to true.
---@author UjAPI
---@param whichUnit unit
---@param whichTarget widget
---@param ignoreDistance boolean
---@param isInstant boolean
function UnitAttackTarget (whichUnit, whichTarget, ignoreDistance, isInstant) end
---@author UjAPI
---@param whichUnit unit
---@param x real
---@param y real
---@param z real
---@param ignoreDistance boolean
---@param isInstant boolean
function UnitAttackTargetPointZ (whichUnit, x, y, z, ignoreDistance, isInstant) end
---@author UjAPI
---@param whichUnit unit
---@param x real
---@param y real
---@param ignoreDistance boolean
---@param isInstant boolean
function UnitAttackTargetPoint (whichUnit, x, y, ignoreDistance, isInstant) end
---@author UjAPI
---@param whichUnit unit
---@param whichLocation location
---@param ignoreDistance boolean
---@param isInstant boolean
function UnitAttackTargetPointLoc (whichUnit, whichLocation, ignoreDistance, isInstant) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function UnitAttackCancel (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function UnitAttackRestart (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return boolean
function UnitAttackFinish (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@return attacktype
function GetUnitAttackTypeByIndex (whichUnit, attackIndex) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@param whichAttackType attacktype
function SetUnitAttackTypeByIndex (whichUnit, attackIndex, whichAttackType) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@return weapontype
function GetUnitWeaponTypeByIndex (whichUnit, attackIndex) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@param whichWeaponType weapontype
function SetUnitWeaponTypeByIndex (whichUnit, attackIndex, whichWeaponType) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@return integer
function GetUnitWeaponSoundByIndex (whichUnit, attackIndex) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@param weaponSound integer
function SetUnitWeaponSoundByIndex (whichUnit, attackIndex, weaponSound) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@return integer
function GetUnitBaseDamageByIndex (whichUnit, attackIndex) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@param damage integer
function SetUnitBaseDamageByIndex (whichUnit, attackIndex, damage) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@return integer
function GetUnitBonusDamageByIndex (whichUnit, attackIndex) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@param bonusDamage integer
function SetUnitBonusDamageByIndex (whichUnit, attackIndex, bonusDamage) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@return integer
function GetUnitDamageDicesSideByIndex (whichUnit, attackIndex) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@param dicesSides integer
function SetUnitDamageDicesSideByIndex (whichUnit, attackIndex, dicesSides) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@return integer
function GetUnitDamageDicesByIndex (whichUnit, attackIndex) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@param dices integer
function SetUnitDamageDicesByIndex (whichUnit, attackIndex, dices) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@return integer
function GetUnitMinimumDamageByIndex (whichUnit, attackIndex) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@return integer
function GetUnitMaximumDamageByIndex (whichUnit, attackIndex) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@return real
function GetUnitAttackRangeByIndex (whichUnit, attackIndex) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@param range real
function SetUnitAttackRangeByIndex (whichUnit, attackIndex, range) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitAttackSpeed (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param attackSpeed real
function SetUnitAttackSpeed (whichUnit, attackSpeed) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@return real
function GetUnitAttackCooldownByIndex (whichUnit, attackIndex) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@param attackCooldown real
function SetUnitAttackCooldownByIndex (whichUnit, attackIndex, attackCooldown) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@return real
function GetUnitBackswingPointByIndex (whichUnit, attackIndex) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@param backswing real
function SetUnitBackswingPointByIndex (whichUnit, attackIndex, backswing) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@return real
function GetUnitDamagePointByIndex (whichUnit, attackIndex) end
---@author UjAPI
---@param whichUnit unit
---@param attackIndex integer
---@param damagePoint real
function SetUnitDamagePointByIndex (whichUnit, attackIndex, damagePoint) end
---@author UjAPI
---@param whichUnit unit
---@return integer
function GetUnitAttacksEnabledIndex (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return defensetype
function GetUnitArmourType (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param whichArmour defensetype
function SetUnitArmourType (whichUnit, whichArmour) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitArmour (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param armour real
function SetUnitArmour (whichUnit, armour) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitTimeScale (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitTotalMoveSpeed (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitBaseMoveSpeed (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param baseMoveSpeed real
function SetUnitBaseMoveSpeed (whichUnit, baseMoveSpeed) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitBonusMoveSpeedPercent (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param bonusMoveSpeedPercent real
function SetUnitBonusMoveSpeedPercent (whichUnit, bonusMoveSpeedPercent) end
---@author UjAPI
---@param whichUnit unit
---@return playercolor
function GetUnitPlayerColour (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param color playercolor
function SetUnitPlayerColour (whichUnit, color) end
---@author UjAPI
---@param whichUnit unit
---@return integer
function GetUnitVertexColour (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param whichItem item
---@param itemSlot integer
---@return boolean
function UnitAddItemToSlot (whichUnit, whichItem, itemSlot) end
---@author UjAPI
---@param whichUnit unit
---@param x real
---@param y real
---@return boolean
function ReviveUnit (whichUnit, x, y) end
---@author UjAPI
---@param whichUnit unit
---@param loc location
---@return boolean
function ReviveUnitLoc (whichUnit, loc) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitCurrentLife (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param life real
function SetUnitCurrentLife (whichUnit, life) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitMaxLife (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param maxLife real
function SetUnitMaxLife (whichUnit, maxLife) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitLifeRegen (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param lifeRegen real
function SetUnitLifeRegen (whichUnit, lifeRegen) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitCurrentMana (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param mana real
function SetUnitCurrentMana (whichUnit, mana) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitMaxMana (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param maxMana real
function SetUnitMaxMana (whichUnit, maxMana) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitManaRegen (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param manaRegen real
function SetUnitManaRegen (whichUnit, manaRegen) end
---@author UjAPI
---@param whichUnit unit
---@return heroattribute
function GetHeroPrimaryAttribute (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param whichHeroAttribute heroattribute
function SetHeroPrimaryAttribute (whichUnit, whichHeroAttribute) end
---@author UjAPI
---@param whichUnit unit
---@return string
function GetUnitModel (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param modelName string
function SetUnitModel (whichUnit, modelName) end
---@author UjAPI
---@param whichUnit unit
---@param modelName string
---@param playercolourId integer
function SetUnitModelEx (whichUnit, modelName, playercolourId) end
---@author UjAPI
---@param whichUnit unit
---@param textureName string
---@param materialId integer
---@param textureIndex integer
function SetUnitMaterialTexture (whichUnit, textureName, materialId, textureIndex) end
---@author UjAPI
---@param whichUnit unit
---@param textureName string
---@param textureIndex integer
function SetUnitTexture (whichUnit, textureName, textureIndex) end
---@author UjAPI
---@param whichUnit unit
---@param textureName string
---@param textureIndex integer
function SetUnitReplaceableTexture (whichUnit, textureName, textureIndex) end
---@author UjAPI
---@param whichUnit unit
---@return collisiontype
function GetUnitCollisionType (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param collisionType collisiontype
function SetUnitCollisionType (whichUnit, collisionType) end
---@author UjAPI
---@param whichUnit unit
---@return pathingaitype
function GetUnitPathingAIType (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param pathingAIType pathingaitype
function SetUnitPathingAIType (whichUnit, pathingAIType) end
---@author UjAPI
---@param whichUnit unit
---@return pathingtype
function GetUnitPathingType (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param pathingType pathingtype
function SetUnitPathingType (whichUnit, pathingType) end
---@author UjAPI
---@param whichUnit unit
---@return integer
function GetUnitItemSlots (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param itemSlots integer
function SetUnitItemSlots (whichUnit, itemSlots) end
---@author UjAPI
---@param whichUnit unit
---@param facing real
---@param isInstant boolean
function SetUnitFacingEx (whichUnit, facing, isInstant) end
---@author UjAPI
---@param whichUnit unit
---@param facing real
function SetUnitFacingInstant (whichUnit, facing) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitMinimapX (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitMinimapY (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitRallyPointX (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitRallyPointY (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return integer
function GetHeroExperienceCurrent (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param level integer
---@return integer
function GetHeroExperienceRequiredAt (whichUnit, level) end
---@author UjAPI
---@param whichUnit unit
---@param state boolean
function UnitApplySilence (whichUnit, state) end
---@author UjAPI
---@param whichUnit unit
---@param state boolean
function UnitDisableAbilities (whichUnit, state) end
---@author UjAPI
---@param whichUnit unit
---@param pause boolean
function PauseUnitEx (whichUnit, pause) end
---@author UjAPI
---@param whichUnit unit
---@param state boolean
function SetUnitStunned (whichUnit, state) end
---@author UjAPI
---@param whichUnit unit
---@return integer
function GetUnitStunCounter (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param stunCounter integer
function SetUnitStunCounter (whichUnit, stunCounter) end
---@author UjAPI
---@param whichUnit unit
---@param killer unit
function SetUnitKiller (whichUnit, killer) end
---@author UjAPI
---@param whichUnit unit
---@param killer unit
function KillUnitEx (whichUnit, killer) end
---@author UjAPI
---@param whichUnit unit
---@return widget
function GetUnitTarget (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return unit
function GetUnitTargetUnit (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return item
function GetUnitTargetItem (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return destructable
function GetUnitTargetDestructable (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param unitTypeId integer
---@param unitFlags integer
---@param updateHealthState boolean
---@param updateManaState boolean
---@param healthStateId integer
---@param manaStateId integer
---@param updateScale boolean
---@param replaceAbilities boolean
---@param whichAbility ability
---@param resetBuildingAnimation boolean
function MorphUnitToTypeIdEx (whichUnit, unitTypeId, unitFlags, updateHealthState, updateManaState, healthStateId, manaStateId, updateScale, replaceAbilities, whichAbility, resetBuildingAnimation) end
---@author UjAPI
---@param whichUnit unit
---@param unitTypeId integer
function MorphUnitToTypeId (whichUnit, unitTypeId) end
---@author UjAPI
---@param whichUnit unit
---@param whichObject string
---@return real
function GetUnitModelObjectX (whichUnit, whichObject) end
---@author UjAPI
---@param whichUnit unit
---@param whichObject string
---@return real
function GetUnitModelObjectY (whichUnit, whichObject) end
---@author UjAPI
---@param whichUnit unit
---@param whichObject string
---@return real
function GetUnitModelObjectZ (whichUnit, whichObject) end
---@author UjAPI
---@param whichUnit unit
---@param whichObject string
---@return location
function GetUnitModelObjectPositionLoc (whichUnit, whichObject) end
---@author UjAPI
---@param whichUnit unit
---@param animIndex integer
---@param rarity raritycontrol
function SetUnitAnimationWithRarityByIndex (whichUnit, animIndex, rarity) end
---@author UjAPI
---@param whichUnit unit
---@param animIndex integer
function QueueUnitAnimationByIndex (whichUnit, animIndex) end
---@author UjAPI
---@param whichUnit unit
---@return integer
function GetUnitCurrentAnimationId (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return string
function GetUnitCurrentAnimationName (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitAnimationOffsetPercent (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param percent real
function SetUnitAnimationOffsetPercent (whichUnit, percent) end
-- Unit Orientation API, these only work if AutoOrientation is set to false. Note, this will disable auto yaw/pitch/roll updates as well, you will have to do them manually.
---@author UjAPI
---@param whichUnit unit
---@return boolean
function IsUnitAutoOrientationEnabled (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param enable boolean
function UnitEnableAutoOrientation (whichUnit, enable) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitYaw (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param yaw real
function SetUnitYaw (whichUnit, yaw) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitPitch (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param pitch real
function SetUnitPitch (whichUnit, pitch) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitRoll (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param roll real
function SetUnitRoll (whichUnit, roll) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitMatrixScaleX (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitMatrixScaleY (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetUnitMatrixScaleZ (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param x real
---@param y real
---@param z real
function SetUnitMatrixScale (whichUnit, x, y, z) end
---@author UjAPI
---@param whichUnit unit
function ResetUnitMatrix (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param yaw real
---@param pitch real
---@param roll real
---@param eulerOrder integer
function SetUnitOrientationEx (whichUnit, yaw, pitch, roll, eulerOrder) end
---@author UjAPI
---@param whichUnit unit
---@param yaw real
---@param pitch real
---@param roll real
function SetUnitOrientation (whichUnit, yaw, pitch, roll) end
-- 

-- Building API
---@author UjAPI
---@param whichPlayer player
---@param unitTypeId integer
---@param x real
---@param y real
---@param facing real
---@return unit
function CreateBuilding (whichPlayer, unitTypeId, x, y, facing) end
---@author UjAPI
---@param whichPlayer player
---@param unitTypeId integer
---@param x real
---@param y real
---@param facing real
---@param isAutoBuild boolean
---@param workersCanAssist boolean
---@return unit
function CreateBuildingEx (whichPlayer, unitTypeId, x, y, facing, isAutoBuild, workersCanAssist) end

-- Construction API
---@author UjAPI
---@param whichUnit unit
---@return real
function UnitGetConstructionProgress (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function UnitGetConstructionRemainingTime (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param time real
function UnitSetConstructionRemainingTime (whichUnit, time) end
-- 

-- Upgrade API
---@author UjAPI
---@param whichUnit unit
---@return real
function UnitGetUpgradeProgress (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@return real
function UnitGetUpgradeRemainingTime (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param time real
function UnitSetUpgradeRemainingTime (whichUnit, time) end
-- 

-- Unit Training API
---@author UjAPI
---@param whichUnit unit
---@return real
function UnitGetTrainingProgress (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param trainingPercentage integer
function UnitSetTrainingProgress (whichUnit, trainingPercentage) end
---@author UjAPI
---@param whichUnit unit
---@return real
function UnitGetTrainingRemainingTime (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param time real
function UnitSetTrainingRemainingTime (whichUnit, time) end
---@author UjAPI
---@param whichUnit unit
---@param index integer
---@return integer
function UnitGetTrainingTypeIdAt (whichUnit, index) end
---@author UjAPI
---@param whichUnit unit
---@param index integer
function UnitCancelTrainingAt (whichUnit, index) end
---@author UjAPI
---@param whichUnit unit
---@param index integer
---@param typeId integer
function UnitSetTrainingTypeIdAt (whichUnit, index, typeId) end
-- 

-- Unit Research API
---@author UjAPI
---@param whichUnit unit
---@return real
function UnitGetResearchProgress (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param trainingPercentage integer
function UnitSetResearchProgress (whichUnit, trainingPercentage) end
---@author UjAPI
---@param whichUnit unit
---@return real
function UnitGetResearchRemainingTime (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param time real
function UnitSetResearchRemainingTime (whichUnit, time) end
---@author UjAPI
---@param whichUnit unit
---@param index integer
---@return integer
function UnitGetResearchTypeIdAt (whichUnit, index) end
---@author UjAPI
---@param whichUnit unit
---@param index integer
function UnitCancelResearchAt (whichUnit, index) end
---@author UjAPI
---@param whichUnit unit
---@param index integer
---@param typeId integer
function UnitSetResearchTypeIdAt (whichUnit, index, typeId) end
-- 

-- Illusion API
-- All created illusions are created without timed life, this can and should be handled by the mapmaker.
---@author UjAPI
---@param whichPlayer player
---@param unitTypeId integer
---@param x real
---@param y real
---@param facing real
---@return unit
function CreateIllusion (whichPlayer, unitTypeId, x, y, facing) end
---@author UjAPI
---@param whichPlayer player
---@param unitTypeId integer
---@param whichLocation location
---@param facing real
---@return unit
function CreateIllusionAtLoc (whichPlayer, unitTypeId, whichLocation, facing) end
---@author UjAPI
---@param whichUnit unit
---@return unit
function CreateIllusionFromUnit (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param copyPassives boolean
---@return unit
function CreateIllusionFromUnitEx (whichUnit, copyPassives) end

-- Damage Dealt and Received are multipliers, not direct values. These will only work on illusions created by abilities/items or by: CreateUnitIllusion, CreateUnitIllusionAtLoc and CreateIllusionFromUnit.
---@author UjAPI
---@param whichUnit unit
---@return real
function GetIllusionDamageDealt (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param multiplier real
function SetIllusionDamageDealt (whichUnit, multiplier) end
---@author UjAPI
---@param whichUnit unit
---@return real
function GetIllusionDamageReceived (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param multiplier real
function SetIllusionDamageReceived (whichUnit, multiplier) end
-- 

-- Unit Order API
---@author UjAPI
---@param whichUnit unit
---@param orderId integer
---@return boolean
function QueueImmediateOrderById (whichUnit, orderId) end
---@author UjAPI
---@param whichUnit unit
---@param orderId integer
---@param x real
---@param y real
---@return boolean
function QueuePointOrderById (whichUnit, orderId, x, y) end
---@author UjAPI
---@param whichUnit unit
---@param orderId integer
---@param targetWidget widget
---@return boolean
function QueueTargetOrderById (whichUnit, orderId, targetWidget) end
---@author UjAPI
---@param whichUnit unit
---@param orderId integer
---@param x real
---@param y real
---@param instantTargetWidget widget
---@return boolean
function QueueInstantPointOrderById (whichUnit, orderId, x, y, instantTargetWidget) end
---@author UjAPI
---@param whichUnit unit
---@param orderId integer
---@param targetWidget widget
---@param instantTargetWidget widget
---@return boolean
function QueueInstantTargetOrderById (whichUnit, orderId, targetWidget, instantTargetWidget) end
---@author UjAPI
---@param whichPeon unit
---@param unitTypeId integer
---@param x real
---@param y real
---@return boolean
function QueueBuildOrderById (whichPeon, unitTypeId, x, y) end
---@author UjAPI
---@param whichPlayer player
---@param neutralStructure unit
---@param unitTypeId integer
---@return boolean
function QueueNeutralImmediateOrderById (whichPlayer, neutralStructure, unitTypeId) end
---@author UjAPI
---@param whichPlayer player
---@param neutralStructure unit
---@param unitTypeId integer
---@param x real
---@param y real
---@return boolean
function QueueNeutralPointOrderById (whichPlayer, neutralStructure, unitTypeId, x, y) end
---@author UjAPI
---@param whichPlayer player
---@param neutralStructure unit
---@param unitTypeId integer
---@param target widget
---@return boolean
function QueueNeutralTargetOrderById (whichPlayer, neutralStructure, unitTypeId, target) end
---@author UjAPI
---@param whichUnit unit
---@return integer
function GetUnitOrderCount (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param index integer
---@return orderhandle
function GetUnitOrderByIndex (whichUnit, index) end
---@author UjAPI
---@param whichUnit unit
---@param orderId integer
---@param index integer
---@return orderhandle
function GetUnitOrderByOrderId (whichUnit, orderId, index) end
---@author UjAPI
---@param whichUnit unit
---@param index integer
---@return integer
function GetUnitOrderIdByIndex (whichUnit, index) end
---@author UjAPI
---@param whichUnit unit
---@param index integer
---@return boolean
function UnitRemoveOrderByIndex (whichUnit, index) end
---@author UjAPI
---@param whichUnit unit
---@param orderId integer
---@param eraseAllSimilar boolean
---@return boolean
function UnitRemoveOrderByOrderId (whichUnit, orderId, eraseAllSimilar) end
---@author UjAPI
---@param whichUnit unit
function UnitReverseOrders (whichUnit) end
---@author UjAPI
---@param whichUnit unit
---@param onlyQueued boolean
function UnitClearOrders (whichUnit, onlyQueued) end
---@author UjAPI
---@param whichUnit unit
---@param clearQueue boolean
function UnitForceStopOrder (whichUnit, clearQueue) end
-- 
-- 

-- ============================================================================
-- Order API
-- Naming is reversed to avoid clashes with GetOrderTarget and so on.
-- 
---@author UjAPI
---@return orderhandle
function GetTriggerOrder () end
---@author UjAPI
---@param whichOrder orderhandle
---@return orderhandle
function OrderGetNext (whichOrder) end
---@author UjAPI
---@param whichOrder orderhandle
---@return integer
function OrderGetId (whichOrder) end
---@author UjAPI
---@param whichOrder orderhandle
---@return real
function OrderGetTargetX (whichOrder) end
---@author UjAPI
---@param whichOrder orderhandle
---@return real
function OrderGetTargetY (whichOrder) end
---@author UjAPI
---@param whichOrder orderhandle
---@return location
function OrderGetTargetLoc (whichOrder) end
-- Patrol orders only?
---@author UjAPI
---@param whichOrder orderhandle
---@return real
function OrderGetSourceX (whichOrder) end
---@author UjAPI
---@param whichOrder orderhandle
---@return real
function OrderGetSourceY (whichOrder) end
---@author UjAPI
---@param whichOrder orderhandle
---@return location
function OrderGetSourceLoc (whichOrder) end
-- 
---@author UjAPI
---@param whichOrder orderhandle
---@return widget
function OrderGetTarget (whichOrder) end
---@author UjAPI
---@param whichOrder orderhandle
---@return destructable
function OrderGetTargetDestructable (whichOrder) end
---@author UjAPI
---@param whichOrder orderhandle
---@return item
function OrderGetTargetItem (whichOrder) end
---@author UjAPI
---@param whichOrder orderhandle
---@return unit
function OrderGetTargetUnit (whichOrder) end
-- 

-- ============================================================================
-- Projectile API
-- For Projectile Type Ids: https://github.com/UnryzeC/UjAPI/blob/main/TypeData/ in there check out these files: WC3BulletList.txt / WC3MissileList.txt / WC3ArtilleryList.txt
-- 
---@author UjAPI
---@param projectileTypeId integer
---@return projectile
function CreateProjectile (projectileTypeId) end
---@author UjAPI
---@param owner unit
---@param projectileTypeId integer
---@param attackIndex integer
---@return projectile
function CreateProjectileEx (owner, projectileTypeId, attackIndex) end
---@author UjAPI
---@param whichProjectile projectile
---@param owner unit
---@param attackIndex integer
function SetProjectileUnitData (whichProjectile, owner, attackIndex) end
---@author UjAPI
---@param whichProjectile projectile
function KillProjectile (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
function RemoveProjectile (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
function LaunchProjectile (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param whichWidget widget
function LaunchProjectileTarget (whichProjectile, whichWidget) end
---@author UjAPI
---@param whichProjectile projectile
---@param x real
---@param y real
---@param z real
function LaunchProjectileAt (whichProjectile, x, y, z) end

---@author UjAPI
---@param whichProjectile projectile
---@return sprite
function GetProjectileSprite (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param whichType projectiletype
---@return boolean
function IsProjectileType (whichProjectile, whichType) end
---@author UjAPI
---@param whichProjectile projectile
---@return boolean
function IsProjectileAlive (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@return boolean
function IsProjectileVisible (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param visibility boolean
function SetProjectileVisible (whichProjectile, visibility) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileX (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param x real
function SetProjectileX (whichProjectile, x) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileY (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param y real
function SetProjectileY (whichProjectile, y) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileZ (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param z real
function SetProjectileZ (whichProjectile, z) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileScreenX (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileScreenY (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileHeight (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param height real
function SetProjectileHeight (whichProjectile, height) end
---@author UjAPI
---@param whichProjectile projectile
---@return location
function GetProjectilePositionLoc (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param x real
---@param y real
function SetProjectilePosition (whichProjectile, x, y) end
---@author UjAPI
---@param whichProjectile projectile
---@param x real
---@param y real
---@param z real
function SetProjectilePositionWithZ (whichProjectile, x, y, z) end
---@author UjAPI
---@param whichProjectile projectile
---@param loc location
function SetProjectilePositionLoc (whichProjectile, loc) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileScale (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param scale real
function SetProjectileScale (whichProjectile, scale) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileTimeScale (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param timescale real
function SetProjectileTimeScale (whichProjectile, timescale) end
---@author UjAPI
---@param whichProjectile projectile
---@return playercolor
function GetProjectilePlayerColour (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param color playercolor
function SetProjectilePlayerColour (whichProjectile, color) end
---@author UjAPI
---@param whichProjectile projectile
---@return integer
function GetProjectileColour (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param colour integer
function SetProjectileColour (whichProjectile, colour) end
---@author UjAPI
---@param whichProjectile projectile
---@param alpha integer
function SetProjectileAlpha (whichProjectile, alpha) end
---@author UjAPI
---@param whichProjectile projectile
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
function SetProjectileVertexColour (whichProjectile, red, green, blue, alpha) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileMatrixScaleX (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileMatrixScaleY (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileMatrixScaleZ (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param x real
---@param y real
---@param z real
function SetProjectileMatrixScale (whichProjectile, x, y, z) end
---@author UjAPI
---@param whichProjectile projectile
function ResetProjectileMatrix (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param yaw real
---@param pitch real
---@param roll real
---@param eulerOrder integer
function SetProjectileOrientationEx (whichProjectile, yaw, pitch, roll, eulerOrder) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileYaw (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param yaw real
function SetProjectileYaw (whichProjectile, yaw) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileFacing (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param facing real
function SetProjectileFacing (whichProjectile, facing) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectilePitch (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param pitch real
function SetProjectilePitch (whichProjectile, pitch) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileRoll (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param roll real
function SetProjectileRoll (whichProjectile, roll) end
---@author UjAPI
---@param whichProjectile projectile
---@param yaw real
---@param pitch real
---@param roll real
function SetProjectileOrientation (whichProjectile, yaw, pitch, roll) end
---@author UjAPI
---@param whichProjectile projectile
---@param textureName string
---@param materialId integer
---@param textureIndex integer
function SetProjectileMaterialTexture (whichProjectile, textureName, materialId, textureIndex) end
---@author UjAPI
---@param whichProjectile projectile
---@param textureName string
---@param textureIndex integer
function SetProjectileTexture (whichProjectile, textureName, textureIndex) end
---@author UjAPI
---@param whichProjectile projectile
---@param textureName string
---@param textureIndex integer
function SetProjectileReplaceableTexture (whichProjectile, textureName, textureIndex) end
---@author UjAPI
---@param whichProjectile projectile
---@return string
function GetProjectileModel (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param modelName string
function SetProjectileModel (whichProjectile, modelName) end
---@author UjAPI
---@param whichProjectile projectile
---@param modelName string
---@param playerColour integer
function SetProjectileModelEx (whichProjectile, modelName, playerColour) end
---@author UjAPI
---@param whichProjectile projectile
---@param whichObject string
---@return real
function GetProjectileModelObjectX (whichProjectile, whichObject) end
---@author UjAPI
---@param whichProjectile projectile
---@param whichObject string
---@return real
function GetProjectileModelObjectY (whichProjectile, whichObject) end
---@author UjAPI
---@param whichProjectile projectile
---@param whichObject string
---@return real
function GetProjectileModelObjectZ (whichProjectile, whichObject) end
---@author UjAPI
---@param whichProjectile projectile
---@param whichObject string
---@return location
function GetProjectileModelObjectPositionLoc (whichProjectile, whichObject) end
---@author UjAPI
---@param whichProjectile projectile
---@return integer
function GetProjectileCurrentAnimationId (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@return string
function GetProjectileCurrentAnimationName (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param animIndex integer
---@param rarity raritycontrol
function SetProjectileAnimationWithRarityByIndex (whichProjectile, animIndex, rarity) end
---@author UjAPI
---@param whichProjectile projectile
---@param animationName string
---@param rarity raritycontrol
function SetProjectileAnimationWithRarity (whichProjectile, animationName, rarity) end
---@author UjAPI
---@param whichProjectile projectile
---@param animIndex integer
function SetProjectileAnimationByIndex (whichProjectile, animIndex) end
---@author UjAPI
---@param whichProjectile projectile
---@param animationName string
function SetProjectileAnimation (whichProjectile, animationName) end
---@author UjAPI
---@param whichProjectile projectile
---@param animIndex integer
function QueueProjectileAnimationByIndex (whichProjectile, animIndex) end
---@author UjAPI
---@param whichProjectile projectile
---@param animationName string
function QueueProjectileAnimation (whichProjectile, animationName) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileAnimationOffsetPercent (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param percent real
function SetProjectileAnimationOffsetPercent (whichProjectile, percent) end

---@author UjAPI
---@param whichProjectile projectile
---@return unit
function GetProjectileSource (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param whichUnit unit
function SetProjectileSource (whichProjectile, whichUnit) end
---@author UjAPI
---@param whichProjectile projectile
---@return ability
function GetProjectileSourceAbility (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param whichAbility ability
function SetProjectileSourceAbility (whichProjectile, whichAbility) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileTargetX (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param x real
function SetProjectileTargetX (whichProjectile, x) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileTargetY (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param y real
function SetProjectileTargetY (whichProjectile, y) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileTargetZ (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param z real
function SetProjectileTargetZ (whichProjectile, z) end
---@author UjAPI
---@param whichProjectile projectile
---@return location
function GetProjectileTargetPositionLoc (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param loc location
function SetProjectileTargetPositionLoc (whichProjectile, loc) end
---@author UjAPI
---@param whichProjectile projectile
---@param x real
---@param y real
function SetProjectileTargetPosition (whichProjectile, x, y) end
---@author UjAPI
---@param whichProjectile projectile
---@param x real
---@param y real
---@param z real
function SetProjectileTargetPositionWithZ (whichProjectile, x, y, z) end
---@author UjAPI
---@param whichProjectile projectile
---@return widget
function GetProjectileTarget (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@return unit
function GetProjectileTargetUnit (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@return item
function GetProjectileTargetItem (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@return destructable
function GetProjectileTargetDestructable (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param whichWidget widget
function SetProjectileTarget (whichProjectile, whichWidget) end
---@author UjAPI
---@param whichProjectile projectile
---@return attacktype
function GetProjectileAttackType (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param whichAttackType attacktype
function SetProjectileAttackType (whichProjectile, whichAttackType) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileDamage (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param damage real
function SetProjectileDamage (whichProjectile, damage) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileAreaOfEffectFullDamage (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param area real
function SetProjectileAreaOfEffectFullDamage (whichProjectile, area) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileAreaOfEffectMediumDamage (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param area real
function SetProjectileAreaOfEffectMediumDamage (whichProjectile, area) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileAreaOfEffectSmallDamage (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param area real
function SetProjectileAreaOfEffectSmallDamage (whichProjectile, area) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileAreaOfEffectMediumDamageFactor (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param factor real
function SetProjectileAreaOfEffectMediumDamageFactor (whichProjectile, factor) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileAreaOfEffectSmallDamageFactor (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param factor real
function SetProjectileAreaOfEffectSmallDamageFactor (whichProjectile, factor) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileSpeed (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param speed real
function SetProjectileSpeed (whichProjectile, speed) end
---@author UjAPI
---@param whichProjectile projectile
---@return real
function GetProjectileArc (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param arc real
function SetProjectileArc (whichProjectile, arc) end
---@author UjAPI
---@param whichProjectile projectile
---@return weapontype
function GetProjectileWeaponType (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param whichWeaponType weapontype
function SetProjectileWeaponType (whichProjectile, whichWeaponType) end
---@author UjAPI
---@param whichProjectile projectile
---@return damagetype
function GetProjectileDamageType (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param whichDamageType damagetype
function SetProjectileDamageType (whichProjectile, whichDamageType) end
---@author UjAPI
---@param whichProjectile projectile
---@return integer
function GetProjectileDamageFlags (whichProjectile) end
---@author UjAPI
---@param whichProjectile projectile
---@param flags integer
function SetProjectileDamageFlags (whichProjectile, flags) end

---@author UjAPI
---@return projectile
function GetTriggerProjectile () end
---@author UjAPI
---@return projectile
function GetFilterProjectile () end
---@author UjAPI
---@return projectile
function GetEnumProjectile () end
---@author UjAPI
---@return unit
function GetTriggerProjectileSource () end
---@author UjAPI
---@return widget
function GetTriggerProjectileTarget () end
---@author UjAPI
---@return unit
function GetTriggerProjectileTargetUnit () end
---@author UjAPI
---@return item
function GetTriggerProjectileTargetItem () end
---@author UjAPI
---@return destructable
function GetTriggerProjectileTargetDestructable () end

---@author UjAPI
---@param x real
---@param y real
---@param radius real
---@param filter boolexpr
---@param handlerFunc code
function EnumProjectilesInRange (x, y, radius, filter, handlerFunc) end
---@author UjAPI
---@param whichRect rect
---@param filter boolexpr
---@param handlerFunc code
function EnumProjectilesInRect (whichRect, filter, handlerFunc) end
-- 

-- ============================================================================
-- Frame API
-- 
---@author UjAPI
---@param whichType originframetype
---@param index integer
---@return framehandle
function GetOriginFrame (whichType, index) end
-- native EnableUIAutoPosition takes boolean flag returns nothing // not active for now
---@author UjAPI
---@param flag boolean
function HideOriginFrames (flag) end
---@author UjAPI
---@param upperHeight real
---@param bottomHeight real
function EditBlackBorders (upperHeight, bottomHeight) end
---@author UjAPI
---@param TOCFile string
---@return boolean
function LoadTOCFile (TOCFile) end
---@author UjAPI
---@param templateName string
---@param whichParent framehandle
---@param priority integer
---@param createContext integer
---@return framehandle
function CreateFrame (templateName, whichParent, priority, createContext) end
---@author UjAPI
---@param templateName string
---@param whichParent framehandle
---@param createContext integer
---@return framehandle
function CreateSimpleFrame (templateName, whichParent, createContext) end
---@author UjAPI
---@param whichSimpleParent framehandle
---@param whichSpriteFrame framehandle
---@return framehandle
function CreateSimpleGlueFrame (whichSimpleParent, whichSpriteFrame) end
---@author UjAPI
---@param frameType string
---@param contextName string
---@param whichParent framehandle
---@param templateName string
---@param createContext integer
---@return framehandle
function CreateFrameByType (frameType, contextName, whichParent, templateName, createContext) end
---@author UjAPI
---@param whichFrame framehandle
---@return boolean
function IsFrameRegion (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@return boolean
function IsFrameSimple (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@return boolean
function IsFrameComplex (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
function DestroyFrame (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameScreenX (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameScreenY (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param point framepointtype
---@return framehandle
function GetFrameRelativePointParent (whichFrame, point) end
---@author UjAPI
---@param whichFrame framehandle
---@param point framepointtype
---@return framepointtype
function GetFrameRelativePointType (whichFrame, point) end
---@author UjAPI
---@param whichFrame framehandle
---@param point framepointtype
---@return real
function GetFrameRelativePointX (whichFrame, point) end
---@author UjAPI
---@param whichFrame framehandle
---@param point framepointtype
---@return real
function GetFrameRelativePointY (whichFrame, point) end
---@author UjAPI
---@param whichFrame framehandle
---@param point framepointtype
---@param relativeFrame framehandle
---@param relativePoint framepointtype
---@param x real
---@param y real
function SetFrameRelativePoint (whichFrame, point, relativeFrame, relativePoint, x, y) end
---@author UjAPI
---@param whichFrame framehandle
---@param point framepointtype
---@return real
function GetFrameAbsolutePointX (whichFrame, point) end
---@author UjAPI
---@param whichFrame framehandle
---@param point framepointtype
---@return real
function GetFrameAbsolutePointY (whichFrame, point) end
---@author UjAPI
---@param whichFrame framehandle
---@param point framepointtype
---@param x real
---@param y real
function SetFrameAbsolutePoint (whichFrame, point, x, y) end
---@author UjAPI
---@param whichFrame framehandle
function ClearFrameAllPoints (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param relativeFrame framehandle
---@return boolean
function SetFrameAllPoints (whichFrame, relativeFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param isShow boolean
function ShowFrame (whichFrame, isShow) end
---@author UjAPI
---@param whichFrame framehandle
---@return boolean
function IsFrameVisible (whichFrame) end
---@author UjAPI
---@param frameName string
---@param createContext integer
---@return framehandle
function GetFrameByName (frameName, createContext) end
---@author UjAPI
---@param frameName string
---@param createContext integer
---@return framehandle
function GetCFrameByName (frameName, createContext) end
---@author UjAPI
---@param frameName string
---@param createContext integer
---@return framehandle
function GetCSimpleFontStringByName (frameName, createContext) end
---@author UjAPI
---@param frameName string
---@param createContext integer
---@return framehandle
function GetCSimpleTextureByName (frameName, createContext) end
---@author UjAPI
---@param frameName string
---@param createContext integer
---@return framehandle
function GetCSimpleFrameByName (frameName, createContext) end
---@author UjAPI
---@return framehandle
function GetFrameUnderCursor () end
---@author UjAPI
---@param whichFrame framehandle
---@param listId integer
---@return integer
function GetFrameChildrenCountEx (whichFrame, listId) end
---@author UjAPI
---@param whichFrame framehandle
---@return integer
function GetFrameChildrenCount (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param listId integer
---@param index integer
---@return framehandle
function GetFrameChildEx (whichFrame, listId, index) end
---@author UjAPI
---@param whichFrame framehandle
---@param index integer
---@return framehandle
function GetFrameChild (whichFrame, index) end
---@author UjAPI
---@param whichFrame framehandle
---@return string
function GetFrameTypeName (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@return string
function GetFrameName (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param contextName string
function SetFrameName (whichFrame, contextName) end
---@author UjAPI
---@param whichFrame framehandle
---@return integer
function GetFrameContext (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param createContext integer
function SetFrameContext (whichFrame, createContext) end
---@author UjAPI
---@param whichFrame framehandle
---@param contextName string
---@param createContext integer
function SetFrameContextName (whichFrame, contextName, createContext) end
---@author UjAPI
---@param whichFrame framehandle
---@param whichMouseButton mousebuttontype
function ClickFrameEx (whichFrame, whichMouseButton) end
---@author UjAPI
---@param whichFrame framehandle
function ClickFrame (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param text string
function SetFrameText (whichFrame, text) end
---@author UjAPI
---@param whichFrame framehandle
---@return string
function GetFrameText (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param text string
function AddFrameText (whichFrame, text) end
---@author UjAPI
---@param whichFrame framehandle
---@param textSize integer
function SetFrameTextSizeLimit (whichFrame, textSize) end
---@author UjAPI
---@param whichFrame framehandle
---@return integer
function GetFrameTextSizeLimit (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param stateId integer
---@return integer
function GetFrameTextColourEx (whichFrame, stateId) end
---@author UjAPI
---@param whichFrame framehandle
---@param stateId integer
---@param colour integer
function SetFrameTextColourEx (whichFrame, stateId, colour) end
---@author UjAPI
---@param whichFrame framehandle
---@return integer
function GetFrameTextColour (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param colour integer
function SetFrameTextColour (whichFrame, colour) end
---@author UjAPI
---@param whichFrame framehandle
---@return boolean
function IsFrameFocused (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param isFocus boolean
---@return boolean
function SetFrameFocus (whichFrame, isFocus) end
---@author UjAPI
---@param whichFrame framehandle
---@return string
function GetFrameModel (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param model string
---@param cameraIndex integer
function SetFrameModel (whichFrame, model, cameraIndex) end
---@author UjAPI
---@param whichFrame framehandle
---@return framestate
function GetFrameState (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param whichFrameState framestate
function SetFrameState (whichFrame, whichFrameState) end
---@author UjAPI
---@param whichFrame framehandle
---@return boolean
function IsFrameEnabled (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param enabled boolean
function SetFrameEnabled (whichFrame, enabled) end
---@author UjAPI
---@param whichFrame framehandle
---@param whichLayoutStyle layoutstyleflag
---@return boolean
function IsFrameLayoutFlag (whichFrame, whichLayoutStyle) end
---@author UjAPI
---@param whichFrame framehandle
---@param whichLayoutStyle layoutstyleflag
---@param isSet boolean
function SetFrameLayoutFlag (whichFrame, whichLayoutStyle, isSet) end
---@author UjAPI
---@param whichFrame framehandle
---@param whichGridStyle gridstyleflag
---@return boolean
function IsFrameGridFlag (whichFrame, whichGridStyle) end
---@author UjAPI
---@param whichFrame framehandle
---@param whichGridStyle gridstyleflag
---@param isSet boolean
function SetFrameGridFlag (whichFrame, whichGridStyle, isSet) end
---@author UjAPI
---@param whichFrame framehandle
---@param whichLayerStyle layerstyleflag
---@return boolean
function IsFrameLayerFlag (whichFrame, whichLayerStyle) end
---@author UjAPI
---@param whichFrame framehandle
---@param whichLayerStyle layerstyleflag
---@param isSet boolean
function SetFrameLayerFlag (whichFrame, whichLayerStyle, isSet) end
---@author UjAPI
---@param whichFrame framehandle
---@param whichControlStyle controlstyleflag
---@return boolean
function IsFrameControlFlag (whichFrame, whichControlStyle) end
---@author UjAPI
---@param whichFrame framehandle
---@param whichControlStyle controlstyleflag
---@param isSet boolean
function SetFrameControlFlag (whichFrame, whichControlStyle, isSet) end
---@author UjAPI
---@param whichFrame framehandle
---@param textureId integer
---@return integer
function GetFrameColourEx (whichFrame, textureId) end
---@author UjAPI
---@param whichFrame framehandle
---@param textureId integer
---@param colour integer
function SetFrameColourEx (whichFrame, textureId, colour) end
---@author UjAPI
---@param whichFrame framehandle
---@return integer
function GetFrameColour (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param colour integer
function SetFrameColour (whichFrame, colour) end
---@author UjAPI
---@param whichFrame framehandle
---@param alpha integer
---@param red integer
---@param green integer
---@param blue integer
function SetFrameVertexColour (whichFrame, alpha, red, green, blue) end
---@author UjAPI
---@param whichFrame framehandle
---@param textureId integer
---@param alpha integer
---@param red integer
---@param green integer
---@param blue integer
function SetFrameVertexColourEx (whichFrame, textureId, alpha, red, green, blue) end
---@author UjAPI
---@param whichFrame framehandle
---@param textureId integer
---@return integer
function GetFrameAlphaEx (whichFrame, textureId) end
---@author UjAPI
---@param whichFrame framehandle
---@param textureId integer
---@param alpha integer
function SetFrameAlphaEx (whichFrame, textureId, alpha) end
---@author UjAPI
---@param whichFrame framehandle
---@return integer
function GetFrameAlpha (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param alpha integer
function SetFrameAlpha (whichFrame, alpha) end
---@author UjAPI
---@param whichFrame framehandle
---@param textureId integer
---@return string
function GetFrameTexture (whichFrame, textureId) end
---@author UjAPI
---@param whichFrame framehandle
---@param textureId integer
---@param backgroundTextureFile string
---@param allowTransparency boolean
---@param blend boolean
---@param borderTextureFile string
---@param borderFlags integer
---@param isControlBackdrop boolean
function SetFrameBackdropTexture (whichFrame, textureId, backgroundTextureFile, allowTransparency, blend, borderTextureFile, borderFlags, isControlBackdrop) end
---@author UjAPI
---@param whichFrame framehandle
---@param textureId integer
---@param backgroundTextureFile string
---@param blend boolean
---@param borderTextureFile string
---@param borderFlags integer
function SetFrameTextureEx (whichFrame, textureId, backgroundTextureFile, blend, borderTextureFile, borderFlags) end
---@author UjAPI
---@param whichFrame framehandle
---@param textureFile string
---@param textureId integer
---@param blend boolean
function SetFrameTexture (whichFrame, textureFile, textureId, blend) end
---@author UjAPI
---@param whichFrame framehandle
---@param textureId integer
---@return blendmode
function GetFrameBlendMode (whichFrame, textureId) end
---@author UjAPI
---@param whichFrame framehandle
---@param textureId integer
---@param whichMode blendmode
function SetFrameBlendMode (whichFrame, textureId, whichMode) end
---@author UjAPI
---@param whichFrame framehandle
---@param tooltipFrame framehandle
function SetFrameTooltip (whichFrame, tooltipFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param enable boolean
function SetFrameMouseCaged (whichFrame, enable) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameValue (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param value real
function SetFrameValue (whichFrame, value) end
---@author UjAPI
---@param whichFrame framehandle
---@param value real
---@param isFireEvent boolean
function SetFrameValueEx (whichFrame, value, isFireEvent) end
---@author UjAPI
---@param whichFrame framehandle
---@param valueId integer
---@return real
function GetFrameMinMaxValues (whichFrame, valueId) end
---@author UjAPI
---@param whichFrame framehandle
---@param minVal real
---@param maxVal real
function SetFrameMinMaxValues (whichFrame, minVal, maxVal) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameStepSize (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param stepSize real
function SetFrameStepSize (whichFrame, stepSize) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameWidth (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param width real
function SetFrameWidth (whichFrame, width) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameHeight (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param height real
function SetFrameHeight (whichFrame, height) end
---@author UjAPI
---@param whichFrame framehandle
---@param width real
---@param height real
function SetFrameSize (whichFrame, width, height) end
---@author UjAPI
---@param whichFrame framehandle
---@param scale real
function SetFrameScale (whichFrame, scale) end
---@author UjAPI
---@param whichFrame framehandle
---@return integer
function GetFramePriority (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param priority integer
function SetFramePriority (whichFrame, priority) end
---@author UjAPI
---@param whichFrame framehandle
---@param whichParent framehandle
function SetFrameParent (whichFrame, whichParent) end
---@author UjAPI
---@param whichFrame framehandle
---@return framehandle
function GetFrameParent (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param fontName string
---@param size real
---@param flags integer
function SetFrameFont (whichFrame, fontName, size, flags) end
-- CSimpleFontString: 0 - x Scale, 1 - y Scale, 2 - x Shadow, 3 - y Shadow | CTextFrame: 0 - x, 1 - y, 2 - x Shadow, 3 - y Shadow, 4 = FontJustificationOffset | CEditBox: 0 - x | 1 - y | 2 - text scale "Border Scale"
---@author UjAPI
---@param whichFrame framehandle
---@param id integer
---@return real
function GetFrameTextAlignmentValue (whichFrame, id) end
---@author UjAPI
---@param whichFrame framehandle
---@param id integer
---@param offset real
function SetFrameTextAlignmentValue (whichFrame, id, offset) end
---@author UjAPI
---@param whichFrame framehandle
---@param verticalAlign textaligntype
---@param horizontalAlign textaligntype
function SetFrameTextAlignment (whichFrame, verticalAlign, horizontalAlign) end
---@author UjAPI
---@param whichFrame framehandle
---@param verticalAlign textaligntype
function SetFrameTextVerticalAlignment (whichFrame, verticalAlign) end
---@author UjAPI
---@param whichFrame framehandle
---@param horizontalAlign textaligntype
function SetFrameTextHorizontalAlignment (whichFrame, horizontalAlign) end
---@author UjAPI
---@param whichFrame framehandle
---@return boolean
function GetFrameCheckState (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param isCheck boolean
function SetFrameCheckState (whichFrame, isCheck) end
-- 

---@author UjAPI
---@param texturePath string
---@return boolean
function SetMiniMapTexture (texturePath) end

-- CSlider / CScollBar API | Scrollbar extends slider, so both use the same logic.
---@author UjAPI
---@param whichFrame framehandle
---@return framehandle
function GetFrameSlider (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@return framehandle
function AddFrameSlider (whichFrame) end
-- 

-- CListBox / CMenu / CPopupMenu / CRadioGroup API
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameItemsBorder (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param value real
function SetFrameItemsBorder (whichFrame, value) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameItemsHeight (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param value real
function SetFrameItemsHeight (whichFrame, value) end

-- These functions return CListBoxItem frames for CListBox / CMenu / CPopupMenu and CCheckBox/CGlueCheckBox for CRadioGroup.
---@author UjAPI
---@param whichFrame framehandle
---@param text string
---@param frameToAdd framehandle
---@return framehandle
function AddFrameListItem (whichFrame, text, frameToAdd) end
---@author UjAPI
---@param whichFrame framehandle
---@return integer
function GetFrameListItemCount (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param id integer
---@return framehandle
function GetFrameListItemById (whichFrame, id) end
---@author UjAPI
---@param whichFrame framehandle
---@param id integer
---@param listBoxItem framehandle
function SetFrameListItemById (whichFrame, id, listBoxItem) end
---@author UjAPI
---@param whichFrame framehandle
---@param frameToFind framehandle
---@return framehandle
function GetFrameListItemByFrame (whichFrame, frameToFind) end
---@author UjAPI
---@param whichFrame framehandle
---@param frameToFind framehandle
---@param listBoxItem framehandle
function SetFrameListItemByFrame (whichFrame, frameToFind, listBoxItem) end
---@author UjAPI
---@param whichFrame framehandle
---@param listBoxItem framehandle
function RemoveFrameListItem (whichFrame, listBoxItem) end
---@author UjAPI
---@param whichFrame framehandle
---@param id integer
function RemoveFrameListItemById (whichFrame, id) end
---@author UjAPI
---@param whichFrame framehandle
---@param listBoxItem framehandle
function RemoveFrameListItemByFrame (whichFrame, listBoxItem) end
-- 

-- CListBoxItem API
---@author UjAPI
---@param listBoxItem framehandle
---@return framehandle
function GetFrameItemOwner (listBoxItem) end
---@author UjAPI
---@param listBoxItem framehandle
---@param whichFrame framehandle
function SetFrameItemOwner (listBoxItem, whichFrame) end
-- 

-- Highlight API
---@author UjAPI
---@param whichFrame framehandle
---@param highlightId integer
---@return framehandle
function GetFrameHighlight (whichFrame, highlightId) end
---@author UjAPI
---@param whichFrame framehandle
---@param highlightId integer
---@return string
function GetFrameHighlightTexture (whichFrame, highlightId) end
---@author UjAPI
---@param whichFrame framehandle
---@param highlightId integer
---@param texturePath string
---@param blendMode blendmode
function SetFrameHighlightTexture (whichFrame, highlightId, texturePath, blendMode) end
-- 

-- Backdrop API | Border API | For border flags refer to BORDER_FLAG. For CBackdropFrame and its children and for CSimpleFrame, backdropId has to be always 0.
-- For CFrames that contain backdrops, use ids to differentiate between them, this is similar to CSimpleButton states, etc.
---@author UjAPI
---@param whichFrame framehandle
---@param backdropId integer
---@return framehandle
function GetFrameBackdrop (whichFrame, backdropId) end
---@author UjAPI
---@param whichFrame framehandle
---@param backdropId integer
---@return boolean
function IsFrameBorderEnabled (whichFrame, backdropId) end
---@author UjAPI
---@param whichFrame framehandle
---@param backdropId integer
---@param isEnable boolean
function SetFrameBorderEnabled (whichFrame, backdropId, isEnable) end
---@author UjAPI
---@param whichFrame framehandle
---@param backdropId integer
---@return integer
function GetFrameBorderFlags (whichFrame, backdropId) end
---@author UjAPI
---@param whichFrame framehandle
---@param backdropId integer
---@param borderFlag integer
function SetFrameBorderFlags (whichFrame, backdropId, borderFlag) end
---@author UjAPI
---@param whichFrame framehandle
---@param backdropId integer
---@return real
function GetFrameBorderSize (whichFrame, backdropId) end
---@author UjAPI
---@param whichFrame framehandle
---@param backdropId integer
---@param value real
function SetFrameBorderSize (whichFrame, backdropId, value) end
---@author UjAPI
---@param whichFrame framehandle
---@param backdropId integer
---@return real
function GetFrameBackgroundSize (whichFrame, backdropId) end
---@author UjAPI
---@param whichFrame framehandle
---@param backdropId integer
---@param value real
function SetFrameBackgroundSize (whichFrame, backdropId, value) end
---@author UjAPI
---@param whichFrame framehandle
---@param backdropId integer
---@param insetId integer
---@return real
function GetFrameBackgroundInsetById (whichFrame, backdropId, insetId) end
---@author UjAPI
---@param whichFrame framehandle
---@param backdropId integer
---@param insetId integer
---@param value real
function SetFrameBackgroundInsetById (whichFrame, backdropId, insetId, value) end
---@author UjAPI
---@param whichFrame framehandle
---@param backdropId integer
---@param minX real
---@param minY real
---@param maxX real
---@param maxY real
function SetFrameBackgroundInsets (whichFrame, backdropId, minX, minY, maxX, maxY) end
-- 

-- Grid API
---@author UjAPI
---@param grid framehandle
---@return integer
function GetFrameGridRows (grid) end
---@author UjAPI
---@param grid framehandle
---@return integer
function GetFrameGridColumns (grid) end
---@author UjAPI
---@param grid framehandle
---@param row integer
---@param column integer
function SetFrameGridSize (grid, row, column) end
---@author UjAPI
---@param grid framehandle
---@param row integer
---@param column integer
---@return framehandle
function GetFrameGridFrame (grid, row, column) end
---@author UjAPI
---@param grid framehandle
---@param id integer
---@return framehandle
function GetFrameGridFrameById (grid, id) end
---@author UjAPI
---@param grid framehandle
---@param row integer
---@param column integer
---@param whichFrame framehandle
function SetFrameGridFrame (grid, row, column, whichFrame) end
---@author UjAPI
---@return boolean
function IsBuffBarRenderDuplicates () end
---@author UjAPI
---@param allow boolean
function SetBuffBarRenderDuplicates (allow) end
-- 

-- Trigger Frame API
---@author UjAPI
---@return framehandle
function GetTriggerFrame () end
---@author UjAPI
---@return frameeventtype
function GetTriggerFrameEvent () end
---@author UjAPI
---@return variabletype
function GetTriggerFrameVariableType () end
---@author UjAPI
---@return integer
function GetTriggerFrameInteger () end
---@author UjAPI
---@return real
function GetTriggerFrameReal () end
---@author UjAPI
---@return boolean
function GetTriggerFrameBoolean () end
---@author UjAPI
---@return string
function GetTriggerFrameString () end
---@author UjAPI
---@return mousebuttontype
function GetTriggerFrameMouseButton () end
---@author UjAPI
---@return framehandle
function GetTriggerFrameTargetFrame () end

---@author UjAPI
---@param whichTrigger trigger
---@param whichFrame framehandle
---@param eventId frameeventtype
---@return event
function TriggerRegisterFrameEvent (whichTrigger, whichFrame, eventId) end
---@author UjAPI
---@param whichFrame framehandle
---@param whichMouseButton mousebuttontype
---@param isAdd boolean
function RegisterFrameMouseButton (whichFrame, whichMouseButton, isAdd) end
-- 

-- Frame Sprite API
-- Copies the logic of Effect API / Trackable API | works only on CSpriteFrame | CStatusBar | CCursorFrame | CTimeOfDayIndicator
-- For Cursor animations refer to: https://github.com/UnryzeC/UjAPI/blob/main/TypeData/WC3CursorAnimations.txt

---@author UjAPI
---@param whichFrame framehandle
---@return sprite
function GetFrameSprite (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameSpriteScale (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param scale real
function SetFrameSpriteScale (whichFrame, scale) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameSpriteTimeScale (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param timescale real
function SetFrameSpriteTimeScale (whichFrame, timescale) end
---@author UjAPI
---@param whichFrame framehandle
---@return playercolor
function GetFrameSpritePlayerColour (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param color playercolor
function SetFrameSpritePlayerColour (whichFrame, color) end
---@author UjAPI
---@param whichFrame framehandle
---@return integer
function GetFrameSpriteAlpha (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param alpha integer
---@return boolean
function SetFrameSpriteAlpha (whichFrame, alpha) end
---@author UjAPI
---@param whichFrame framehandle
---@return integer
function GetFrameSpriteColour (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param colour integer
---@return boolean
function SetFrameSpriteColour (whichFrame, colour) end
---@author UjAPI
---@param whichFrame framehandle
---@param red integer
---@param green integer
---@param blue integer
---@param alpha integer
---@return boolean
function SetFrameSpriteVertexColour (whichFrame, red, green, blue, alpha) end
---@author UjAPI
---@param whichFrame framehandle
---@param yaw real
---@param pitch real
---@param roll real
---@param eulerOrder integer
---@return boolean
function SetFrameSpriteOrientationEx (whichFrame, yaw, pitch, roll, eulerOrder) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameSpriteYaw (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param yaw real
---@return boolean
function SetFrameSpriteYaw (whichFrame, yaw) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameSpriteFacing (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param facing real
---@return boolean
function SetFrameSpriteFacing (whichFrame, facing) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameSpritePitch (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param pitch real
---@return boolean
function SetFrameSpritePitch (whichFrame, pitch) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameSpriteRoll (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param roll real
---@return boolean
function SetFrameSpriteRoll (whichFrame, roll) end
---@author UjAPI
---@param whichFrame framehandle
---@param yaw real
---@param pitch real
---@param roll real
function SetFrameSpriteOrientation (whichFrame, yaw, pitch, roll) end
---@author UjAPI
---@param whichFrame framehandle
---@param textureName string
---@param materialId integer
---@param textureIndex integer
function SetFrameSpriteMaterialTexture (whichFrame, textureName, materialId, textureIndex) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameSpriteMatrixScaleX (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameSpriteMatrixScaleY (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameSpriteMatrixScaleZ (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param sizeX real
---@param sizeY real
---@param sizeZ real
function SetFrameSpriteMatrixScale (whichFrame, sizeX, sizeY, sizeZ) end
---@author UjAPI
---@param whichFrame framehandle
function ResetFrameSpriteMatrix (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param textureName string
---@param textureIndex integer
function SetFrameSpriteTexture (whichFrame, textureName, textureIndex) end
---@author UjAPI
---@param whichFrame framehandle
---@param textureName string
---@param textureIndex integer
function SetFrameSpriteReplaceableTexture (whichFrame, textureName, textureIndex) end
---@author UjAPI
---@param whichFrame framehandle
---@return string
function GetFrameSpriteModel (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param modelName string
function SetFrameSpriteModel (whichFrame, modelName) end
---@author UjAPI
---@param whichFrame framehandle
---@param modelName string
---@param playerColour integer
function SetFrameSpriteModelEx (whichFrame, modelName, playerColour) end
---@author UjAPI
---@param whichFrame framehandle
---@return integer
function GetFrameSpriteCurrentAnimationId (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@return string
function GetFrameSpriteCurrentAnimationName (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param animIndex integer
---@param rarity raritycontrol
function SetFrameSpriteAnimationWithRarityByIndex (whichFrame, animIndex, rarity) end
---@author UjAPI
---@param whichFrame framehandle
---@param animIndex integer
function SetFrameSpriteAnimationByIndex (whichFrame, animIndex) end
---@author UjAPI
---@param whichFrame framehandle
---@param animIndex integer
function QueueFrameSpriteAnimationByIndex (whichFrame, animIndex) end
---@author UjAPI
---@param whichFrame framehandle
---@param animationName string
---@param rarity raritycontrol
function SetFrameSpriteAnimationWithRarity (whichFrame, animationName, rarity) end
---@author UjAPI
---@param whichFrame framehandle
---@param animationName string
function SetFrameSpriteAnimation (whichFrame, animationName) end
---@author UjAPI
---@param whichFrame framehandle
---@param animationName string
function QueueFrameSpriteAnimation (whichFrame, animationName) end
---@author UjAPI
---@param whichFrame framehandle
---@return real
function GetFrameSpriteAnimationOffsetPercent (whichFrame) end
---@author UjAPI
---@param whichFrame framehandle
---@param percent real
function SetFrameSpriteAnimationOffsetPercent (whichFrame, percent) end
-- 

-- ============================================================================
-- Sync API
-- 

---@author UjAPI
---@return player
function GetTriggerSyncPlayer () end

-- Variable Sync API
---@author UjAPI
---@return variabletype
function GetSyncedVariableType () end
---@author UjAPI
---@return string
function GetSyncedVariableName () end
---@author UjAPI
---@return integer
function GetSyncedInteger () end
---@author UjAPI
---@return real
function GetSyncedReal () end
---@author UjAPI
---@return string
function GetSyncedString () end
---@author UjAPI
---@return handle
function GetSyncedHandle () end
---@author UjAPI
---@return boolean
function GetSyncedBoolean () end

---@author UjAPI
---@param variableName string
function SyncInteger (variableName) end
---@author UjAPI
---@param variableName string
function SyncReal (variableName) end
---@author UjAPI
---@param variableName string
function SyncString (variableName) end
---@author UjAPI
---@param variableName string
function SyncHandle (variableName) end
---@author UjAPI
---@param variableName string
function SyncBoolean (variableName) end

---@author UjAPI
---@param whichTrigger trigger
---@param whichPlayer player
---@param variableName string
---@return event
function TriggerRegisterPlayerVariableSyncEvent (whichTrigger, whichPlayer, variableName) end
-- 

-- Hashtable Sync API
---@author UjAPI
---@return variabletype
function GetSyncSavedVariableType () end
---@author UjAPI
---@return hashtable
function GetSyncSavedHashtable () end
---@author UjAPI
---@return integer
function GetSyncSavedParentKey () end
---@author UjAPI
---@return integer
function GetSyncSavedChildKey () end

---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
function SyncSavedInteger (whichHashtable, parentKey, childKey) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
function SyncSavedReal (whichHashtable, parentKey, childKey) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
function SyncSavedString (whichHashtable, parentKey, childKey) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
function SyncSavedHandle (whichHashtable, parentKey, childKey) end
---@author UjAPI
---@param whichHashtable hashtable
---@param parentKey integer
---@param childKey integer
function SyncSavedBoolean (whichHashtable, parentKey, childKey) end

---@author UjAPI
---@param whichTrigger trigger
---@param whichPlayer player
---@param whichHashtable hashtable
---@return event
function TriggerRegisterPlayerHashtableDataSyncEvent (whichTrigger, whichPlayer, whichHashtable) end
-- 

-- Prefix Sync API
---@author UjAPI
---@return string
function GetTriggerSyncPrefix () end
---@author UjAPI
---@return string
function GetTriggerSyncData () end

---@author UjAPI
---@param prefix string
---@param data string
---@return boolean
function SendSyncData (prefix, data) end

---@author UjAPI
---@param whichTrigger trigger
---@param whichPlayer player
---@param prefix string
---@param fromServer boolean
---@return event
function TriggerRegisterPlayerSyncEvent (whichTrigger, whichPlayer, prefix, fromServer) end
-- 

-- Key/KeyEvent API
---@author UjAPI
---@param whichKey oskeytype
---@return boolean
function IsKeyPressed (whichKey) end
---@author UjAPI
---@param whichMouseButton mousebuttontype
---@return boolean
function IsMouseKeyPressed (whichMouseButton) end
---@author UjAPI
---@return boolean
function GetTriggerPlayerIsKeyDown () end

-- EVENT_PLAYER_MOUSE_DOWN
-- EVENT_PLAYER_MOUSE_UP
-- EVENT_PLAYER_KEY
-- EVENT_PLAYER_KEY_DOWN
-- EVENT_PLAYER_KEY_UP
-- EVENT_PLAYER_WIDGET_CLICK
-- EVENT_PLAYER_WIDGET_GHOST_CLICK
-- EVENT_PLAYER_TERRAIN_CLICK
---@author UjAPI
---@return oskeytype
function GetTriggerPlayerKey () end
---@author UjAPI
---@return mousebuttontype
function GetTriggerPlayerMouseButton () end
---@author UjAPI
---@return integer
function GetTriggerPlayerMetaKey () end

---@author UjAPI
---@param whichTrigger trigger
---@param whichPlayer player
---@param whichKey oskeytype
---@param whichMetaKey integer
---@param isKeyDown boolean
---@return event
function TriggerRegisterPlayerKeyEvent (whichTrigger, whichPlayer, whichKey, whichMetaKey, isKeyDown) end
-- 

-- Mouse Event API
---@author UjAPI
---@return boolean
function GetMouseMoveEventScreenAxisEnabled () end
---@author UjAPI
---@param enable boolean
function SetMouseMoveEventScreenAxisEnabled (enable) end
---@author UjAPI
---@return boolean
function GetMouseMoveEventWorldAxisEnabled () end
---@author UjAPI
---@param enable boolean
function SetMouseMoveEventWorldAxisEnabled (enable) end
---@author UjAPI
---@return integer
function GetMouseMoveEventDelay () end
---@author UjAPI
---@param delay integer
function SetMouseMoveEventDelay (delay) end

-- EVENT_PLAYER_MOUSE_MOVE
-- EVENT_PLAYER_WIDGET_TRACK
-- EVENT_PLAYER_WIDGET_GHOST_TRACK
-- EVENT_PLAYER_WIDGET_CLICK
-- EVENT_PLAYER_WIDGET_GHOST_CLICK
-- EVENT_PLAYER_TERRAIN_CLICK
---@author UjAPI
---@return real
function GetTriggerPlayerMouseWorldX () end
---@author UjAPI
---@return real
function GetTriggerPlayerMouseWorldY () end
---@author UjAPI
---@return real
function GetTriggerPlayerMouseWorldZ () end

-- EVENT_PLAYER_MOUSE_MOVE
---@author UjAPI
---@return real
function GetTriggerPlayerMouseScreenX () end
---@author UjAPI
---@return real
function GetTriggerPlayerMouseScreenY () end
-- 

-- ============================================================================
-- Damage Event API
-- 

-- Refer to https://github.com/UnryzeC/UjAPI/blob/main/TypeData/WC3DamageData.txt
---@author UjAPI
---@return integer
function GetEventDamageFlags () end
---@author UjAPI
---@param flag integer
---@return boolean
function GetEventDamageIsFlag (flag) end
---@author UjAPI
---@param flag integer
---@param isSet boolean
---@return boolean
function SetEventDamageFlag (flag, isSet) end

---@author UjAPI
---@return integer
function GetEventDamageExtraFlags () end
---@author UjAPI
---@param flag integer
---@return boolean
function GetEventDamageIsExtraFlag (flag) end
---@author UjAPI
---@param flag integer
---@param isSet boolean
---@return boolean
function SetEventDamageExtraFlag (flag, isSet) end

---@author UjAPI
---@return unit
function GetEventDamageTarget () end

---@author UjAPI
---@return attacktype
function GetEventAttackType () end
---@author UjAPI
---@param attackType attacktype
---@return boolean
function SetEventAttackType (attackType) end

---@author UjAPI
---@return damagetype
function GetEventDamageType () end
---@author UjAPI
---@param damageType damagetype
---@return boolean
function SetEventDamageType (damageType) end

---@author UjAPI
---@return integer
function GetEventDamageTypeFlags () end
---@author UjAPI
---@param damageFlags integer
---@return boolean
function SetEventDamageTypeFlags (damageFlags) end

---@author UjAPI
---@return weapontype
function GetEventWeaponType () end
---@author UjAPI
---@param weaponType weapontype
---@return boolean
function SetEventWeaponType (weaponType) end

---@author UjAPI
---@return boolean
function GetEventIsAttack () end
---@author UjAPI
---@return boolean
function GetEventIsRanged () end
---@author UjAPI
---@return boolean
function GetEventIsCritical () end

---@author UjAPI
---@return real
function GetEventPreDamage () end

---@author UjAPI
---@param damage real
function SetEventDamage (damage) end
-- 

-- ============================================================================
-- BlzNatives Start
-- 
---@param whichStartLoc integer
---@param prioSlotCount integer
function SetEnemyStartLocPrioCount (whichStartLoc, prioSlotCount) end
---@param whichStartLoc integer
---@param prioSlotIndex integer
---@param otherStartLocIndex integer
---@param priority startlocprio
function SetEnemyStartLocPrio (whichStartLoc, prioSlotIndex, otherStartLocIndex, priority) end


---@param taggedString string
---@return string
function ParseTags (taggedString) end

-- EVENT_COMMAND_BUTTON_CLICK
---@param whichTrigger trigger
---@param whichAbility integer
---@param orderName string
---@return event
function TriggerRegisterCommandEvent (whichTrigger, whichAbility, orderName) end
---@param whichTrigger trigger
---@param whichUpgrade integer
---@return event
function TriggerRegisterUpgradeCommandEvent (whichTrigger, whichUpgrade) end

-- For EVENT_PLAYER_UNIT_PICKUP_ITEM, returns the item absorbing the picked up item in case it is stacking.
-- Returns null if the item was a powerup and not a stacking item.

-- EVENT_PLAYER_UNIT_STACK_ITEM
-- Source is the item that is losing charges, Target is the item getting charges.

---@param whichPlayer player
---@return real
function GetPlayerHandicapReviveTime (whichPlayer) end
---@param whichPlayer player
---@return real
function GetPlayerHandicapDamage (whichPlayer) end
---@param whichPlayer player
---@param handicap real
function SetPlayerHandicapReviveTime (whichPlayer, handicap) end
---@param whichPlayer player
---@param handicap real
function SetPlayerHandicapDamage (whichPlayer, handicap) end

---@param maxCheckpointSaves integer
function SetMaxCheckpointSaves (maxCheckpointSaves) end
---@param saveFileName string
---@param showWindow boolean
function SaveGameCheckpoint (saveFileName, showWindow) end

---@param portraitDNCFile string
function SetPortraitLight (portraitDNCFile) end
---@param whichUnit unit
---@param red integer
---@param green integer
---@param blue integer
---@param pingPath string
---@param fogVisibility fogstate
---@return minimapicon
function CreateMinimapIconOnUnit (whichUnit, red, green, blue, pingPath, fogVisibility) end
---@param where location
---@param red integer
---@param green integer
---@param blue integer
---@param pingPath string
---@param fogVisibility fogstate
---@return minimapicon
function CreateMinimapIconAtLoc (where, red, green, blue, pingPath, fogVisibility) end
---@param x real
---@param y real
---@param red integer
---@param green integer
---@param blue integer
---@param pingPath string
---@param fogVisibility fogstate
---@return minimapicon
function CreateMinimapIcon (x, y, red, green, blue, pingPath, fogVisibility) end
---@param key string
---@return string
function SkinManagerGetLocalPath (key) end
---@param whichMinimapIcon minimapicon
function DestroyMinimapIcon (whichMinimapIcon) end
---@param whichMinimapIcon minimapicon
---@param visible boolean
function SetMinimapIconVisible (whichMinimapIcon, visible) end
---@param whichMinimapIcon minimapicon
---@param doDestroy boolean
function SetMinimapIconOrphanDestroy (whichMinimapIcon, doDestroy) end


---@param distance real
function CameraSetFocalDistance (distance) end
---@param scale real
function CameraSetDepthOfFieldScale (scale) end

---@param cinematicAudio boolean
function SetCinematicAudio (cinematicAudio) end

---@param soundHandle sound
---@param fadeIn boolean
function StartSoundEx (soundHandle, fadeIn) end

---@param volume integer
function SetThematicMusicVolume (volume) end

---@param soundHandle sound
---@param animationLabel string
---@return boolean
function SetSoundFacialAnimationLabel (soundHandle, animationLabel) end
---@param soundHandle sound
---@param groupLabel string
---@return boolean
function SetSoundFacialAnimationGroupLabel (soundHandle, groupLabel) end
---@param soundHandle sound
---@param animationSetFilepath string
---@return boolean
function SetSoundFacialAnimationSetFilepath (soundHandle, animationSetFilepath) end

-- Subtitle support that is attached to the soundHandle rather than as disperate data with the legacy UI
---@param soundHandle sound
---@param speakerName string
---@return boolean
function SetDialogueSpeakerNameKey (soundHandle, speakerName) end
---@param soundHandle sound
---@return string
function GetDialogueSpeakerNameKey (soundHandle) end
---@param soundHandle sound
---@param dialogueText string
---@return boolean
function SetDialogueTextKey (soundHandle, dialogueText) end
---@param soundHandle sound
---@return string
function GetDialogueTextKey (soundHandle) end

-- ============================================================================
-- Machinima API
-- ============================================================================

-- Automation Test
---@param testType string
function AutomationSetTestType (testType) end
---@param testName string
function AutomationTestStart (testName) end
function AutomationTestEnd () end
function AutomationTestingFinished () end

-- JAPI Functions

-- native BlzFourCC2S takes integer value returns string
-- native BlzS2FourCC takes string value returns integer

---@param abilityTypeId integer
---@param orderName string
---@return commandbuttoneffect
function CreateCommandButtonEffect (abilityTypeId, orderName) end
---@param whichUprgade integer
---@return commandbuttoneffect
function CreateUpgradeCommandButtonEffect (whichUprgade) end
---@param abilityTypeId integer
---@return commandbuttoneffect
function CreateLearnCommandButtonEffect (abilityTypeId) end
---@param whichEffect commandbuttoneffect
function DestroyCommandButtonEffect (whichEffect) end

-- Bit Operations

-- Intanced Object Operations
-- Ability

-- Item

-- Unit

-- Unit Weapon

-- Skin
-- native BlzGetDestructableSkintakes destructable whichDestructable returns integer
-- native BlzSetDestructableSkintakes destructable whichDestructable, integer skinId returns nothing



-- returns the number of orders the unit currently has queued up
-- clears either all orders or only queued up orders
-- stops the current order and optionally clears the queue
-- BlzNatives End
