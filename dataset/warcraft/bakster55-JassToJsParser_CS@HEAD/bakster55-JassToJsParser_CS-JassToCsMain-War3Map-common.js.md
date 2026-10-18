---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/War3Map/common.js"
ref: HEAD
---







































































































































function ConvertRace(i) {
}
function ConvertAllianceType(i) {
}
function ConvertRacePref(i) {
}
function ConvertIGameState(i) {
}
function ConvertFGameState(i) {
}
function ConvertPlayerState(i) {
}
function ConvertPlayerScore(i) {
}
function ConvertPlayerGameResult(i) {
}
function ConvertUnitState(i) {
}
function ConvertAIDifficulty(i) {
}
function ConvertGameEvent(i) {
}
function ConvertPlayerEvent(i) {
}
function ConvertPlayerUnitEvent(i) {
}
function ConvertWidgetEvent(i) {
}
function ConvertDialogEvent(i) {
}
function ConvertUnitEvent(i) {
}
function ConvertLimitOp(i) {
}
function ConvertUnitType(i) {
}
function ConvertGameSpeed(i) {
}
function ConvertPlacement(i) {
}
function ConvertStartLocPrio(i) {
}
function ConvertGameDifficulty(i) {
}
function ConvertGameType(i) {
}
function ConvertMapFlag(i) {
}
function ConvertMapVisibility(i) {
}
function ConvertMapSetting(i) {
}
function ConvertMapDensity(i) {
}
function ConvertMapControl(i) {
}
function ConvertPlayerColor(i) {
}
function ConvertPlayerSlotState(i) {
}
function ConvertVolumeGroup(i) {
}
function ConvertCameraField(i) {
}
function ConvertBlendMode(i) {
}
function ConvertRarityControl(i) {
}
function ConvertTexMapFlags(i) {
}
function ConvertFogState(i) {
}
function ConvertEffectType(i) {
}
function ConvertVersion(i) {
}
function ConvertItemType(i) {
}
function ConvertAttackType(i) {
}
function ConvertDamageType(i) {
}
function ConvertWeaponType(i) {
}
function ConvertSoundType(i) {
}
function ConvertPathingType(i) {
}
function ConvertMouseButtonType(i) {
}
function ConvertAnimType(i) {
}
function ConvertSubAnimType(i) {
}
function ConvertOriginFrameType(i) {
}
function ConvertFramePointType(i) {
}
function ConvertTextAlignType(i) {
}
function ConvertFrameEventType(i) {
}
function ConvertOsKeyType(i) {
}
function ConvertAbilityIntegerField(i) {
}
function ConvertAbilityRealField(i) {
}
function ConvertAbilityBooleanField(i) {
}
function ConvertAbilityStringField(i) {
}
function ConvertAbilityIntegerLevelField(i) {
}
function ConvertAbilityRealLevelField(i) {
}
function ConvertAbilityBooleanLevelField(i) {
}
function ConvertAbilityStringLevelField(i) {
}
function ConvertAbilityIntegerLevelArrayField(i) {
}
function ConvertAbilityRealLevelArrayField(i) {
}
function ConvertAbilityBooleanLevelArrayField(i) {
}
function ConvertAbilityStringLevelArrayField(i) {
}
function ConvertUnitIntegerField(i) {
}
function ConvertUnitRealField(i) {
}
function ConvertUnitBooleanField(i) {
}
function ConvertUnitStringField(i) {
}
function ConvertUnitWeaponIntegerField(i) {
}
function ConvertUnitWeaponRealField(i) {
}
function ConvertUnitWeaponBooleanField(i) {
}
function ConvertUnitWeaponStringField(i) {
}
function ConvertItemIntegerField(i) {
}
function ConvertItemRealField(i) {
}
function ConvertItemBooleanField(i) {
}
function ConvertItemStringField(i) {
}
function ConvertMoveType(i) {
}
function ConvertTargetFlag(i) {
}
function ConvertArmorType(i) {
}
function ConvertHeroAttribute(i) {
}
function ConvertDefenseType(i) {
}
function ConvertRegenType(i) {
}
function ConvertUnitCategory(i) {
}
function ConvertPathingFlag(i) {
}
function OrderId(orderIdString) {
}
function OrderId2String(orderId) {
}
function UnitId(unitIdString) {
}
function UnitId2String(unitId) {
}
function AbilityId(abilityIdString) {
}
function AbilityId2String(abilityId) {
}
function GetObjectName(objectId) {
}
function GetBJMaxPlayers() {
}
function GetBJPlayerNeutralVictim() {
}
function GetBJPlayerNeutralExtra() {
}
function GetBJMaxPlayerSlots() {
}
function GetPlayerNeutralPassive() {
}
function GetPlayerNeutralAggressive() {
}
var FALSE=false
var TRUE=true
var JASS_MAX_ARRAY_SIZE=32768
var PLAYER_NEUTRAL_PASSIVE=GetPlayerNeutralPassive()
var PLAYER_NEUTRAL_AGGRESSIVE=GetPlayerNeutralAggressive()
var PLAYER_COLOR_RED=ConvertPlayerColor(0)
var PLAYER_COLOR_BLUE=ConvertPlayerColor(1)
var PLAYER_COLOR_CYAN=ConvertPlayerColor(2)
var PLAYER_COLOR_PURPLE=ConvertPlayerColor(3)
var PLAYER_COLOR_YELLOW=ConvertPlayerColor(4)
var PLAYER_COLOR_ORANGE=ConvertPlayerColor(5)
var PLAYER_COLOR_GREEN=ConvertPlayerColor(6)
var PLAYER_COLOR_PINK=ConvertPlayerColor(7)
var PLAYER_COLOR_LIGHT_GRAY=ConvertPlayerColor(8)
var PLAYER_COLOR_LIGHT_BLUE=ConvertPlayerColor(9)
var PLAYER_COLOR_AQUA=ConvertPlayerColor(10)
var PLAYER_COLOR_BROWN=ConvertPlayerColor(11)
var PLAYER_COLOR_MAROON=ConvertPlayerColor(12)
var PLAYER_COLOR_NAVY=ConvertPlayerColor(13)
var PLAYER_COLOR_TURQUOISE=ConvertPlayerColor(14)
var PLAYER_COLOR_VIOLET=ConvertPlayerColor(15)
var PLAYER_COLOR_WHEAT=ConvertPlayerColor(16)
var PLAYER_COLOR_PEACH=ConvertPlayerColor(17)
var PLAYER_COLOR_MINT=ConvertPlayerColor(18)
var PLAYER_COLOR_LAVENDER=ConvertPlayerColor(19)
var PLAYER_COLOR_COAL=ConvertPlayerColor(20)
var PLAYER_COLOR_SNOW=ConvertPlayerColor(21)
var PLAYER_COLOR_EMERALD=ConvertPlayerColor(22)
var PLAYER_COLOR_PEANUT=ConvertPlayerColor(23)
var RACE_HUMAN=ConvertRace(1)
var RACE_ORC=ConvertRace(2)
var RACE_UNDEAD=ConvertRace(3)
var RACE_NIGHTELF=ConvertRace(4)
var RACE_DEMON=ConvertRace(5)
var RACE_OTHER=ConvertRace(7)
var PLAYER_GAME_RESULT_VICTORY=ConvertPlayerGameResult(0)
var PLAYER_GAME_RESULT_DEFEAT=ConvertPlayerGameResult(1)
var PLAYER_GAME_RESULT_TIE=ConvertPlayerGameResult(2)
var PLAYER_GAME_RESULT_NEUTRAL=ConvertPlayerGameResult(3)
var ALLIANCE_PASSIVE=ConvertAllianceType(0)
var ALLIANCE_HELP_REQUEST=ConvertAllianceType(1)
var ALLIANCE_HELP_RESPONSE=ConvertAllianceType(2)
var ALLIANCE_SHARED_XP=ConvertAllianceType(3)
var ALLIANCE_SHARED_SPELLS=ConvertAllianceType(4)
var ALLIANCE_SHARED_VISION=ConvertAllianceType(5)
var ALLIANCE_SHARED_CONTROL=ConvertAllianceType(6)
var ALLIANCE_SHARED_ADVANCED_CONTROL=ConvertAllianceType(7)
var ALLIANCE_RESCUABLE=ConvertAllianceType(8)
var ALLIANCE_SHARED_VISION_FORCED=ConvertAllianceType(9)
var VERSION_REIGN_OF_CHAOS=ConvertVersion(0)
var VERSION_FROZEN_THRONE=ConvertVersion(1)
var ATTACK_TYPE_NORMAL=ConvertAttackType(0)
var ATTACK_TYPE_MELEE=ConvertAttackType(1)
var ATTACK_TYPE_PIERCE=ConvertAttackType(2)
var ATTACK_TYPE_SIEGE=ConvertAttackType(3)
var ATTACK_TYPE_MAGIC=ConvertAttackType(4)
var ATTACK_TYPE_CHAOS=ConvertAttackType(5)
var ATTACK_TYPE_HERO=ConvertAttackType(6)
var DAMAGE_TYPE_UNKNOWN=ConvertDamageType(0)
var DAMAGE_TYPE_NORMAL=ConvertDamageType(4)
var DAMAGE_TYPE_ENHANCED=ConvertDamageType(5)
var DAMAGE_TYPE_FIRE=ConvertDamageType(8)
var DAMAGE_TYPE_COLD=ConvertDamageType(9)
var DAMAGE_TYPE_LIGHTNING=ConvertDamageType(10)
var DAMAGE_TYPE_POISON=ConvertDamageType(11)
var DAMAGE_TYPE_DISEASE=ConvertDamageType(12)
var DAMAGE_TYPE_DIVINE=ConvertDamageType(13)
var DAMAGE_TYPE_MAGIC=ConvertDamageType(14)
var DAMAGE_TYPE_SONIC=ConvertDamageType(15)
var DAMAGE_TYPE_ACID=ConvertDamageType(16)
var DAMAGE_TYPE_FORCE=ConvertDamageType(17)
var DAMAGE_TYPE_DEATH=ConvertDamageType(18)
var DAMAGE_TYPE_MIND=ConvertDamageType(19)
var DAMAGE_TYPE_PLANT=ConvertDamageType(20)
var DAMAGE_TYPE_DEFENSIVE=ConvertDamageType(21)
var DAMAGE_TYPE_DEMOLITION=ConvertDamageType(22)
var DAMAGE_TYPE_SLOW_POISON=ConvertDamageType(23)
var DAMAGE_TYPE_SPIRIT_LINK=ConvertDamageType(24)
var DAMAGE_TYPE_SHADOW_STRIKE=ConvertDamageType(25)
var DAMAGE_TYPE_UNIVERSAL=ConvertDamageType(26)
var WEAPON_TYPE_WHOKNOWS=ConvertWeaponType(0)
var WEAPON_TYPE_METAL_LIGHT_CHOP=ConvertWeaponType(1)
var WEAPON_TYPE_METAL_MEDIUM_CHOP=ConvertWeaponType(2)
var WEAPON_TYPE_METAL_HEAVY_CHOP=ConvertWeaponType(3)
var WEAPON_TYPE_METAL_LIGHT_SLICE=ConvertWeaponType(4)
var WEAPON_TYPE_METAL_MEDIUM_SLICE=ConvertWeaponType(5)
var WEAPON_TYPE_METAL_HEAVY_SLICE=ConvertWeaponType(6)
var WEAPON_TYPE_METAL_MEDIUM_BASH=ConvertWeaponType(7)
var WEAPON_TYPE_METAL_HEAVY_BASH=ConvertWeaponType(8)
var WEAPON_TYPE_METAL_MEDIUM_STAB=ConvertWeaponType(9)
var WEAPON_TYPE_METAL_HEAVY_STAB=ConvertWeaponType(10)
var WEAPON_TYPE_WOOD_LIGHT_SLICE=ConvertWeaponType(11)
var WEAPON_TYPE_WOOD_MEDIUM_SLICE=ConvertWeaponType(12)
var WEAPON_TYPE_WOOD_HEAVY_SLICE=ConvertWeaponType(13)
var WEAPON_TYPE_WOOD_LIGHT_BASH=ConvertWeaponType(14)
var WEAPON_TYPE_WOOD_MEDIUM_BASH=ConvertWeaponType(15)
var WEAPON_TYPE_WOOD_HEAVY_BASH=ConvertWeaponType(16)
var WEAPON_TYPE_WOOD_LIGHT_STAB=ConvertWeaponType(17)
var WEAPON_TYPE_WOOD_MEDIUM_STAB=ConvertWeaponType(18)
var WEAPON_TYPE_CLAW_LIGHT_SLICE=ConvertWeaponType(19)
var WEAPON_TYPE_CLAW_MEDIUM_SLICE=ConvertWeaponType(20)
var WEAPON_TYPE_CLAW_HEAVY_SLICE=ConvertWeaponType(21)
var WEAPON_TYPE_AXE_MEDIUM_CHOP=ConvertWeaponType(22)
var WEAPON_TYPE_ROCK_HEAVY_BASH=ConvertWeaponType(23)
var PATHING_TYPE_ANY=ConvertPathingType(0)
var PATHING_TYPE_WALKABILITY=ConvertPathingType(1)
var PATHING_TYPE_FLYABILITY=ConvertPathingType(2)
var PATHING_TYPE_BUILDABILITY=ConvertPathingType(3)
var PATHING_TYPE_PEONHARVESTPATHING=ConvertPathingType(4)
var PATHING_TYPE_BLIGHTPATHING=ConvertPathingType(5)
var PATHING_TYPE_FLOATABILITY=ConvertPathingType(6)
var PATHING_TYPE_AMPHIBIOUSPATHING=ConvertPathingType(7)
var MOUSE_BUTTON_TYPE_LEFT=ConvertMouseButtonType(1)
var MOUSE_BUTTON_TYPE_MIDDLE=ConvertMouseButtonType(2)
var MOUSE_BUTTON_TYPE_RIGHT=ConvertMouseButtonType(3)
var ANIM_TYPE_BIRTH=ConvertAnimType(0)
var ANIM_TYPE_DEATH=ConvertAnimType(1)
var ANIM_TYPE_DECAY=ConvertAnimType(2)
var ANIM_TYPE_DISSIPATE=ConvertAnimType(3)
var ANIM_TYPE_STAND=ConvertAnimType(4)
var ANIM_TYPE_WALK=ConvertAnimType(5)
var ANIM_TYPE_ATTACK=ConvertAnimType(6)
var ANIM_TYPE_MORPH=ConvertAnimType(7)
var ANIM_TYPE_SLEEP=ConvertAnimType(8)
var ANIM_TYPE_SPELL=ConvertAnimType(9)
var ANIM_TYPE_PORTRAIT=ConvertAnimType(10)
var SUBANIM_TYPE_ROOTED=ConvertSubAnimType(11)
var SUBANIM_TYPE_ALTERNATE_EX=ConvertSubAnimType(12)
var SUBANIM_TYPE_LOOPING=ConvertSubAnimType(13)
var SUBANIM_TYPE_SLAM=ConvertSubAnimType(14)
var SUBANIM_TYPE_THROW=ConvertSubAnimType(15)
var SUBANIM_TYPE_SPIKED=ConvertSubAnimType(16)
var SUBANIM_TYPE_FAST=ConvertSubAnimType(17)
var SUBANIM_TYPE_SPIN=ConvertSubAnimType(18)
var SUBANIM_TYPE_READY=ConvertSubAnimType(19)
var SUBANIM_TYPE_CHANNEL=ConvertSubAnimType(20)
var SUBANIM_TYPE_DEFEND=ConvertSubAnimType(21)
var SUBANIM_TYPE_VICTORY=ConvertSubAnimType(22)
var SUBANIM_TYPE_TURN=ConvertSubAnimType(23)
var SUBANIM_TYPE_LEFT=ConvertSubAnimType(24)
var SUBANIM_TYPE_RIGHT=ConvertSubAnimType(25)
var SUBANIM_TYPE_FIRE=ConvertSubAnimType(26)
var SUBANIM_TYPE_FLESH=ConvertSubAnimType(27)
var SUBANIM_TYPE_HIT=ConvertSubAnimType(28)
var SUBANIM_TYPE_WOUNDED=ConvertSubAnimType(29)
var SUBANIM_TYPE_LIGHT=ConvertSubAnimType(30)
var SUBANIM_TYPE_MODERATE=ConvertSubAnimType(31)
var SUBANIM_TYPE_SEVERE=ConvertSubAnimType(32)
var SUBANIM_TYPE_CRITICAL=ConvertSubAnimType(33)
var SUBANIM_TYPE_COMPLETE=ConvertSubAnimType(34)
var SUBANIM_TYPE_GOLD=ConvertSubAnimType(35)
var SUBANIM_TYPE_LUMBER=ConvertSubAnimType(36)
var SUBANIM_TYPE_WORK=ConvertSubAnimType(37)
var SUBANIM_TYPE_TALK=ConvertSubAnimType(38)
var SUBANIM_TYPE_FIRST=ConvertSubAnimType(39)
var SUBANIM_TYPE_SECOND=ConvertSubAnimType(40)
var SUBANIM_TYPE_THIRD=ConvertSubAnimType(41)
var SUBANIM_TYPE_FOURTH=ConvertSubAnimType(42)
var SUBANIM_TYPE_FIFTH=ConvertSubAnimType(43)
var SUBANIM_TYPE_ONE=ConvertSubAnimType(44)
var SUBANIM_TYPE_TWO=ConvertSubAnimType(45)
var SUBANIM_TYPE_THREE=ConvertSubAnimType(46)
var SUBANIM_TYPE_FOUR=ConvertSubAnimType(47)
var SUBANIM_TYPE_FIVE=ConvertSubAnimType(48)
var SUBANIM_TYPE_SMALL=ConvertSubAnimType(49)
var SUBANIM_TYPE_MEDIUM=ConvertSubAnimType(50)
var SUBANIM_TYPE_LARGE=ConvertSubAnimType(51)
var SUBANIM_TYPE_UPGRADE=ConvertSubAnimType(52)
var SUBANIM_TYPE_DRAIN=ConvertSubAnimType(53)
var SUBANIM_TYPE_FILL=ConvertSubAnimType(54)
var SUBANIM_TYPE_CHAINLIGHTNING=ConvertSubAnimType(55)
var SUBANIM_TYPE_EATTREE=ConvertSubAnimType(56)
var SUBANIM_TYPE_PUKE=ConvertSubAnimType(57)
var SUBANIM_TYPE_FLAIL=ConvertSubAnimType(58)
var SUBANIM_TYPE_OFF=ConvertSubAnimType(59)
var SUBANIM_TYPE_SWIM=ConvertSubAnimType(60)
var SUBANIM_TYPE_ENTANGLE=ConvertSubAnimType(61)
var SUBANIM_TYPE_BERSERK=ConvertSubAnimType(62)
var RACE_PREF_HUMAN=ConvertRacePref(1)
var RACE_PREF_ORC=ConvertRacePref(2)
var RACE_PREF_NIGHTELF=ConvertRacePref(4)
var RACE_PREF_UNDEAD=ConvertRacePref(8)
var RACE_PREF_DEMON=ConvertRacePref(16)
var RACE_PREF_RANDOM=ConvertRacePref(32)
var RACE_PREF_USER_SELECTABLE=ConvertRacePref(64)
var MAP_CONTROL_USER=ConvertMapControl(0)
var MAP_CONTROL_COMPUTER=ConvertMapControl(1)
var MAP_CONTROL_RESCUABLE=ConvertMapControl(2)
var MAP_CONTROL_NEUTRAL=ConvertMapControl(3)
var MAP_CONTROL_CREEP=ConvertMapControl(4)
var MAP_CONTROL_NONE=ConvertMapControl(5)
var GAME_TYPE_MELEE=ConvertGameType(1)
var GAME_TYPE_FFA=ConvertGameType(2)
var GAME_TYPE_USE_MAP_SETTINGS=ConvertGameType(4)
var GAME_TYPE_BLIZ=ConvertGameType(8)
var GAME_TYPE_ONE_ON_ONE=ConvertGameType(16)
var GAME_TYPE_TWO_TEAM_PLAY=ConvertGameType(32)
var GAME_TYPE_THREE_TEAM_PLAY=ConvertGameType(64)
var GAME_TYPE_FOUR_TEAM_PLAY=ConvertGameType(128)
var MAP_FOG_HIDE_TERRAIN=ConvertMapFlag(1)
var MAP_FOG_MAP_EXPLORED=ConvertMapFlag(2)
var MAP_FOG_ALWAYS_VISIBLE=ConvertMapFlag(4)
var MAP_USE_HANDICAPS=ConvertMapFlag(8)
var MAP_OBSERVERS=ConvertMapFlag(16)
var MAP_OBSERVERS_ON_DEATH=ConvertMapFlag(32)
var MAP_FIXED_COLORS=ConvertMapFlag(128)
var MAP_LOCK_RESOURCE_TRADING=ConvertMapFlag(256)
var MAP_RESOURCE_TRADING_ALLIES_ONLY=ConvertMapFlag(512)
var MAP_LOCK_ALLIANCE_CHANGES=ConvertMapFlag(1024)
var MAP_ALLIANCE_CHANGES_HIDDEN=ConvertMapFlag(2048)
var MAP_CHEATS=ConvertMapFlag(4096)
var MAP_CHEATS_HIDDEN=ConvertMapFlag(8192)
var MAP_LOCK_SPEED=ConvertMapFlag(8192*2)
var MAP_LOCK_RANDOM_SEED=ConvertMapFlag(8192*4)
var MAP_SHARED_ADVANCED_CONTROL=ConvertMapFlag(8192*8)
var MAP_RANDOM_HERO=ConvertMapFlag(8192*16)
var MAP_RANDOM_RACES=ConvertMapFlag(8192*32)
var MAP_RELOADED=ConvertMapFlag(8192*64)
var MAP_PLACEMENT_RANDOM=ConvertPlacement(0)
var MAP_PLACEMENT_FIXED=ConvertPlacement(1)
var MAP_PLACEMENT_USE_MAP_SETTINGS=ConvertPlacement(2)
var MAP_PLACEMENT_TEAMS_TOGETHER=ConvertPlacement(3)
var MAP_LOC_PRIO_LOW=ConvertStartLocPrio(0)
var MAP_LOC_PRIO_HIGH=ConvertStartLocPrio(1)
var MAP_LOC_PRIO_NOT=ConvertStartLocPrio(2)
var MAP_DENSITY_NONE=ConvertMapDensity(0)
var MAP_DENSITY_LIGHT=ConvertMapDensity(1)
var MAP_DENSITY_MEDIUM=ConvertMapDensity(2)
var MAP_DENSITY_HEAVY=ConvertMapDensity(3)
var MAP_DIFFICULTY_EASY=ConvertGameDifficulty(0)
var MAP_DIFFICULTY_NORMAL=ConvertGameDifficulty(1)
var MAP_DIFFICULTY_HARD=ConvertGameDifficulty(2)
var MAP_DIFFICULTY_INSANE=ConvertGameDifficulty(3)
var MAP_SPEED_SLOWEST=ConvertGameSpeed(0)
var MAP_SPEED_SLOW=ConvertGameSpeed(1)
var MAP_SPEED_NORMAL=ConvertGameSpeed(2)
var MAP_SPEED_FAST=ConvertGameSpeed(3)
var MAP_SPEED_FASTEST=ConvertGameSpeed(4)
var PLAYER_SLOT_STATE_EMPTY=ConvertPlayerSlotState(0)
var PLAYER_SLOT_STATE_PLAYING=ConvertPlayerSlotState(1)
var PLAYER_SLOT_STATE_LEFT=ConvertPlayerSlotState(2)
var SOUND_VOLUMEGROUP_UNITMOVEMENT=ConvertVolumeGroup(0)
var SOUND_VOLUMEGROUP_UNITSOUNDS=ConvertVolumeGroup(1)
var SOUND_VOLUMEGROUP_COMBAT=ConvertVolumeGroup(2)
var SOUND_VOLUMEGROUP_SPELLS=ConvertVolumeGroup(3)
var SOUND_VOLUMEGROUP_UI=ConvertVolumeGroup(4)
var SOUND_VOLUMEGROUP_MUSIC=ConvertVolumeGroup(5)
var SOUND_VOLUMEGROUP_AMBIENTSOUNDS=ConvertVolumeGroup(6)
var SOUND_VOLUMEGROUP_FIRE=ConvertVolumeGroup(7)
var GAME_STATE_DIVINE_INTERVENTION=ConvertIGameState(0)
var GAME_STATE_DISCONNECTED=ConvertIGameState(1)
var GAME_STATE_TIME_OF_DAY=ConvertFGameState(2)
var PLAYER_STATE_GAME_RESULT=ConvertPlayerState(0)
var PLAYER_STATE_RESOURCE_GOLD=ConvertPlayerState(1)
var PLAYER_STATE_RESOURCE_LUMBER=ConvertPlayerState(2)
var PLAYER_STATE_RESOURCE_HERO_TOKENS=ConvertPlayerState(3)
var PLAYER_STATE_RESOURCE_FOOD_CAP=ConvertPlayerState(4)
var PLAYER_STATE_RESOURCE_FOOD_USED=ConvertPlayerState(5)
var PLAYER_STATE_FOOD_CAP_CEILING=ConvertPlayerState(6)
var PLAYER_STATE_GIVES_BOUNTY=ConvertPlayerState(7)
var PLAYER_STATE_ALLIED_VICTORY=ConvertPlayerState(8)
var PLAYER_STATE_PLACED=ConvertPlayerState(9)
var PLAYER_STATE_OBSERVER_ON_DEATH=ConvertPlayerState(10)
var PLAYER_STATE_OBSERVER=ConvertPlayerState(11)
var PLAYER_STATE_UNFOLLOWABLE=ConvertPlayerState(12)
var PLAYER_STATE_GOLD_UPKEEP_RATE=ConvertPlayerState(13)
var PLAYER_STATE_LUMBER_UPKEEP_RATE=ConvertPlayerState(14)
var PLAYER_STATE_GOLD_GATHERED=ConvertPlayerState(15)
var PLAYER_STATE_LUMBER_GATHERED=ConvertPlayerState(16)
var PLAYER_STATE_NO_CREEP_SLEEP=ConvertPlayerState(25)
var UNIT_STATE_LIFE=ConvertUnitState(0)
var UNIT_STATE_MAX_LIFE=ConvertUnitState(1)
var UNIT_STATE_MANA=ConvertUnitState(2)
var UNIT_STATE_MAX_MANA=ConvertUnitState(3)
var AI_DIFFICULTY_NEWBIE=ConvertAIDifficulty(0)
var AI_DIFFICULTY_NORMAL=ConvertAIDifficulty(1)
var AI_DIFFICULTY_INSANE=ConvertAIDifficulty(2)
var PLAYER_SCORE_UNITS_TRAINED=ConvertPlayerScore(0)
var PLAYER_SCORE_UNITS_KILLED=ConvertPlayerScore(1)
var PLAYER_SCORE_STRUCT_BUILT=ConvertPlayerScore(2)
var PLAYER_SCORE_STRUCT_RAZED=ConvertPlayerScore(3)
var PLAYER_SCORE_TECH_PERCENT=ConvertPlayerScore(4)
var PLAYER_SCORE_FOOD_MAXPROD=ConvertPlayerScore(5)
var PLAYER_SCORE_FOOD_MAXUSED=ConvertPlayerScore(6)
var PLAYER_SCORE_HEROES_KILLED=ConvertPlayerScore(7)
var PLAYER_SCORE_ITEMS_GAINED=ConvertPlayerScore(8)
var PLAYER_SCORE_MERCS_HIRED=ConvertPlayerScore(9)
var PLAYER_SCORE_GOLD_MINED_TOTAL=ConvertPlayerScore(10)
var PLAYER_SCORE_GOLD_MINED_UPKEEP=ConvertPlayerScore(11)
var PLAYER_SCORE_GOLD_LOST_UPKEEP=ConvertPlayerScore(12)
var PLAYER_SCORE_GOLD_LOST_TAX=ConvertPlayerScore(13)
var PLAYER_SCORE_GOLD_GIVEN=ConvertPlayerScore(14)
var PLAYER_SCORE_GOLD_RECEIVED=ConvertPlayerScore(15)
var PLAYER_SCORE_LUMBER_TOTAL=ConvertPlayerScore(16)
var PLAYER_SCORE_LUMBER_LOST_UPKEEP=ConvertPlayerScore(17)
var PLAYER_SCORE_LUMBER_LOST_TAX=ConvertPlayerScore(18)
var PLAYER_SCORE_LUMBER_GIVEN=ConvertPlayerScore(19)
var PLAYER_SCORE_LUMBER_RECEIVED=ConvertPlayerScore(20)
var PLAYER_SCORE_UNIT_TOTAL=ConvertPlayerScore(21)
var PLAYER_SCORE_HERO_TOTAL=ConvertPlayerScore(22)
var PLAYER_SCORE_RESOURCE_TOTAL=ConvertPlayerScore(23)
var PLAYER_SCORE_TOTAL=ConvertPlayerScore(24)
var EVENT_GAME_VICTORY=ConvertGameEvent(0)
var EVENT_GAME_END_LEVEL=ConvertGameEvent(1)
var EVENT_GAME_VARIABLE_LIMIT=ConvertGameEvent(2)
var EVENT_GAME_STATE_LIMIT=ConvertGameEvent(3)
var EVENT_GAME_TIMER_EXPIRED=ConvertGameEvent(4)
var EVENT_GAME_ENTER_REGION=ConvertGameEvent(5)
var EVENT_GAME_LEAVE_REGION=ConvertGameEvent(6)
var EVENT_GAME_TRACKABLE_HIT=ConvertGameEvent(7)
var EVENT_GAME_TRACKABLE_TRACK=ConvertGameEvent(8)
var EVENT_GAME_SHOW_SKILL=ConvertGameEvent(9)
var EVENT_GAME_BUILD_SUBMENU=ConvertGameEvent(10)
var EVENT_PLAYER_STATE_LIMIT=ConvertPlayerEvent(11)
var EVENT_PLAYER_ALLIANCE_CHANGED=ConvertPlayerEvent(12)
var EVENT_PLAYER_DEFEAT=ConvertPlayerEvent(13)
var EVENT_PLAYER_VICTORY=ConvertPlayerEvent(14)
var EVENT_PLAYER_LEAVE=ConvertPlayerEvent(15)
var EVENT_PLAYER_CHAT=ConvertPlayerEvent(16)
var EVENT_PLAYER_END_CINEMATIC=ConvertPlayerEvent(17)
var EVENT_PLAYER_UNIT_ATTACKED=ConvertPlayerUnitEvent(18)
var EVENT_PLAYER_UNIT_RESCUED=ConvertPlayerUnitEvent(19)
var EVENT_PLAYER_UNIT_DEATH=ConvertPlayerUnitEvent(20)
var EVENT_PLAYER_UNIT_DECAY=ConvertPlayerUnitEvent(21)
var EVENT_PLAYER_UNIT_DETECTED=ConvertPlayerUnitEvent(22)
var EVENT_PLAYER_UNIT_HIDDEN=ConvertPlayerUnitEvent(23)
var EVENT_PLAYER_UNIT_SELECTED=ConvertPlayerUnitEvent(24)
var EVENT_PLAYER_UNIT_DESELECTED=ConvertPlayerUnitEvent(25)
var EVENT_PLAYER_UNIT_CONSTRUCT_START=ConvertPlayerUnitEvent(26)
var EVENT_PLAYER_UNIT_CONSTRUCT_CANCEL=ConvertPlayerUnitEvent(27)
var EVENT_PLAYER_UNIT_CONSTRUCT_FINISH=ConvertPlayerUnitEvent(28)
var EVENT_PLAYER_UNIT_UPGRADE_START=ConvertPlayerUnitEvent(29)
var EVENT_PLAYER_UNIT_UPGRADE_CANCEL=ConvertPlayerUnitEvent(30)
var EVENT_PLAYER_UNIT_UPGRADE_FINISH=ConvertPlayerUnitEvent(31)
var EVENT_PLAYER_UNIT_TRAIN_START=ConvertPlayerUnitEvent(32)
var EVENT_PLAYER_UNIT_TRAIN_CANCEL=ConvertPlayerUnitEvent(33)
var EVENT_PLAYER_UNIT_TRAIN_FINISH=ConvertPlayerUnitEvent(34)
var EVENT_PLAYER_UNIT_RESEARCH_START=ConvertPlayerUnitEvent(35)
var EVENT_PLAYER_UNIT_RESEARCH_CANCEL=ConvertPlayerUnitEvent(36)
var EVENT_PLAYER_UNIT_RESEARCH_FINISH=ConvertPlayerUnitEvent(37)
var EVENT_PLAYER_UNIT_ISSUED_ORDER=ConvertPlayerUnitEvent(38)
var EVENT_PLAYER_UNIT_ISSUED_POINT_ORDER=ConvertPlayerUnitEvent(39)
var EVENT_PLAYER_UNIT_ISSUED_TARGET_ORDER=ConvertPlayerUnitEvent(40)
var EVENT_PLAYER_UNIT_ISSUED_UNIT_ORDER=ConvertPlayerUnitEvent(40)
var EVENT_PLAYER_HERO_LEVEL=ConvertPlayerUnitEvent(41)
var EVENT_PLAYER_HERO_SKILL=ConvertPlayerUnitEvent(42)
var EVENT_PLAYER_HERO_REVIVABLE=ConvertPlayerUnitEvent(43)
var EVENT_PLAYER_HERO_REVIVE_START=ConvertPlayerUnitEvent(44)
var EVENT_PLAYER_HERO_REVIVE_CANCEL=ConvertPlayerUnitEvent(45)
var EVENT_PLAYER_HERO_REVIVE_FINISH=ConvertPlayerUnitEvent(46)
var EVENT_PLAYER_UNIT_SUMMON=ConvertPlayerUnitEvent(47)
var EVENT_PLAYER_UNIT_DROP_ITEM=ConvertPlayerUnitEvent(48)
var EVENT_PLAYER_UNIT_PICKUP_ITEM=ConvertPlayerUnitEvent(49)
var EVENT_PLAYER_UNIT_USE_ITEM=ConvertPlayerUnitEvent(50)
var EVENT_PLAYER_UNIT_LOADED=ConvertPlayerUnitEvent(51)
var EVENT_PLAYER_UNIT_DAMAGED=ConvertPlayerUnitEvent(308)
var EVENT_PLAYER_UNIT_DAMAGING=ConvertPlayerUnitEvent(315)
var EVENT_UNIT_DAMAGED=ConvertUnitEvent(52)
var EVENT_UNIT_DAMAGING=ConvertUnitEvent(314)
var EVENT_UNIT_DEATH=ConvertUnitEvent(53)
var EVENT_UNIT_DECAY=ConvertUnitEvent(54)
var EVENT_UNIT_DETECTED=ConvertUnitEvent(55)
var EVENT_UNIT_HIDDEN=ConvertUnitEvent(56)
var EVENT_UNIT_SELECTED=ConvertUnitEvent(57)
var EVENT_UNIT_DESELECTED=ConvertUnitEvent(58)
var EVENT_UNIT_STATE_LIMIT=ConvertUnitEvent(59)
var EVENT_UNIT_ACQUIRED_TARGET=ConvertUnitEvent(60)
var EVENT_UNIT_TARGET_IN_RANGE=ConvertUnitEvent(61)
var EVENT_UNIT_ATTACKED=ConvertUnitEvent(62)
var EVENT_UNIT_RESCUED=ConvertUnitEvent(63)
var EVENT_UNIT_CONSTRUCT_CANCEL=ConvertUnitEvent(64)
var EVENT_UNIT_CONSTRUCT_FINISH=ConvertUnitEvent(65)
var EVENT_UNIT_UPGRADE_START=ConvertUnitEvent(66)
var EVENT_UNIT_UPGRADE_CANCEL=ConvertUnitEvent(67)
var EVENT_UNIT_UPGRADE_FINISH=ConvertUnitEvent(68)
var EVENT_UNIT_TRAIN_START=ConvertUnitEvent(69)
var EVENT_UNIT_TRAIN_CANCEL=ConvertUnitEvent(70)
var EVENT_UNIT_TRAIN_FINISH=ConvertUnitEvent(71)
var EVENT_UNIT_RESEARCH_START=ConvertUnitEvent(72)
var EVENT_UNIT_RESEARCH_CANCEL=ConvertUnitEvent(73)
var EVENT_UNIT_RESEARCH_FINISH=ConvertUnitEvent(74)
var EVENT_UNIT_ISSUED_ORDER=ConvertUnitEvent(75)
var EVENT_UNIT_ISSUED_POINT_ORDER=ConvertUnitEvent(76)
var EVENT_UNIT_ISSUED_TARGET_ORDER=ConvertUnitEvent(77)
var EVENT_UNIT_HERO_LEVEL=ConvertUnitEvent(78)
var EVENT_UNIT_HERO_SKILL=ConvertUnitEvent(79)
var EVENT_UNIT_HERO_REVIVABLE=ConvertUnitEvent(80)
var EVENT_UNIT_HERO_REVIVE_START=ConvertUnitEvent(81)
var EVENT_UNIT_HERO_REVIVE_CANCEL=ConvertUnitEvent(82)
var EVENT_UNIT_HERO_REVIVE_FINISH=ConvertUnitEvent(83)
var EVENT_UNIT_SUMMON=ConvertUnitEvent(84)
var EVENT_UNIT_DROP_ITEM=ConvertUnitEvent(85)
var EVENT_UNIT_PICKUP_ITEM=ConvertUnitEvent(86)
var EVENT_UNIT_USE_ITEM=ConvertUnitEvent(87)
var EVENT_UNIT_LOADED=ConvertUnitEvent(88)
var EVENT_WIDGET_DEATH=ConvertWidgetEvent(89)
var EVENT_DIALOG_BUTTON_CLICK=ConvertDialogEvent(90)
var EVENT_DIALOG_CLICK=ConvertDialogEvent(91)
var EVENT_GAME_LOADED=ConvertGameEvent(256)
var EVENT_GAME_TOURNAMENT_FINISH_SOON=ConvertGameEvent(257)
var EVENT_GAME_TOURNAMENT_FINISH_NOW=ConvertGameEvent(258)
var EVENT_GAME_SAVE=ConvertGameEvent(259)
var EVENT_GAME_CUSTOM_UI_FRAME=ConvertGameEvent(310)
var EVENT_PLAYER_ARROW_LEFT_DOWN=ConvertPlayerEvent(261)
var EVENT_PLAYER_ARROW_LEFT_UP=ConvertPlayerEvent(262)
var EVENT_PLAYER_ARROW_RIGHT_DOWN=ConvertPlayerEvent(263)
var EVENT_PLAYER_ARROW_RIGHT_UP=ConvertPlayerEvent(264)
var EVENT_PLAYER_ARROW_DOWN_DOWN=ConvertPlayerEvent(265)
var EVENT_PLAYER_ARROW_DOWN_UP=ConvertPlayerEvent(266)
var EVENT_PLAYER_ARROW_UP_DOWN=ConvertPlayerEvent(267)
var EVENT_PLAYER_ARROW_UP_UP=ConvertPlayerEvent(268)
var EVENT_PLAYER_MOUSE_DOWN=ConvertPlayerEvent(305)
var EVENT_PLAYER_MOUSE_UP=ConvertPlayerEvent(306)
var EVENT_PLAYER_MOUSE_MOVE=ConvertPlayerEvent(307)
var EVENT_PLAYER_SYNC_DATA=ConvertPlayerEvent(309)
var EVENT_PLAYER_KEY=ConvertPlayerEvent(311)
var EVENT_PLAYER_KEY_DOWN=ConvertPlayerEvent(312)
var EVENT_PLAYER_KEY_UP=ConvertPlayerEvent(313)
var EVENT_PLAYER_UNIT_SELL=ConvertPlayerUnitEvent(269)
var EVENT_PLAYER_UNIT_CHANGE_OWNER=ConvertPlayerUnitEvent(270)
var EVENT_PLAYER_UNIT_SELL_ITEM=ConvertPlayerUnitEvent(271)
var EVENT_PLAYER_UNIT_SPELL_CHANNEL=ConvertPlayerUnitEvent(272)
var EVENT_PLAYER_UNIT_SPELL_CAST=ConvertPlayerUnitEvent(273)
var EVENT_PLAYER_UNIT_SPELL_EFFECT=ConvertPlayerUnitEvent(274)
var EVENT_PLAYER_UNIT_SPELL_FINISH=ConvertPlayerUnitEvent(275)
var EVENT_PLAYER_UNIT_SPELL_ENDCAST=ConvertPlayerUnitEvent(276)
var EVENT_PLAYER_UNIT_PAWN_ITEM=ConvertPlayerUnitEvent(277)
var EVENT_UNIT_SELL=ConvertUnitEvent(286)
var EVENT_UNIT_CHANGE_OWNER=ConvertUnitEvent(287)
var EVENT_UNIT_SELL_ITEM=ConvertUnitEvent(288)
var EVENT_UNIT_SPELL_CHANNEL=ConvertUnitEvent(289)
var EVENT_UNIT_SPELL_CAST=ConvertUnitEvent(290)
var EVENT_UNIT_SPELL_EFFECT=ConvertUnitEvent(291)
var EVENT_UNIT_SPELL_FINISH=ConvertUnitEvent(292)
var EVENT_UNIT_SPELL_ENDCAST=ConvertUnitEvent(293)
var EVENT_UNIT_PAWN_ITEM=ConvertUnitEvent(294)
var LESS_THAN=ConvertLimitOp(0)
var LESS_THAN_OR_EQUAL=ConvertLimitOp(1)
var EQUAL=ConvertLimitOp(2)
var GREATER_THAN_OR_EQUAL=ConvertLimitOp(3)
var GREATER_THAN=ConvertLimitOp(4)
var NOT_EQUAL=ConvertLimitOp(5)
var UNIT_TYPE_HERO=ConvertUnitType(0)
var UNIT_TYPE_DEAD=ConvertUnitType(1)
var UNIT_TYPE_STRUCTURE=ConvertUnitType(2)
var UNIT_TYPE_FLYING=ConvertUnitType(3)
var UNIT_TYPE_GROUND=ConvertUnitType(4)
var UNIT_TYPE_ATTACKS_FLYING=ConvertUnitType(5)
var UNIT_TYPE_ATTACKS_GROUND=ConvertUnitType(6)
var UNIT_TYPE_MELEE_ATTACKER=ConvertUnitType(7)
var UNIT_TYPE_RANGED_ATTACKER=ConvertUnitType(8)
var UNIT_TYPE_GIANT=ConvertUnitType(9)
var UNIT_TYPE_SUMMONED=ConvertUnitType(10)
var UNIT_TYPE_STUNNED=ConvertUnitType(11)
var UNIT_TYPE_PLAGUED=ConvertUnitType(12)
var UNIT_TYPE_SNARED=ConvertUnitType(13)
var UNIT_TYPE_UNDEAD=ConvertUnitType(14)
var UNIT_TYPE_MECHANICAL=ConvertUnitType(15)
var UNIT_TYPE_PEON=ConvertUnitType(16)
var UNIT_TYPE_SAPPER=ConvertUnitType(17)
var UNIT_TYPE_TOWNHALL=ConvertUnitType(18)
var UNIT_TYPE_ANCIENT=ConvertUnitType(19)
var UNIT_TYPE_TAUREN=ConvertUnitType(20)
var UNIT_TYPE_POISONED=ConvertUnitType(21)
var UNIT_TYPE_POLYMORPHED=ConvertUnitType(22)
var UNIT_TYPE_SLEEPING=ConvertUnitType(23)
var UNIT_TYPE_RESISTANT=ConvertUnitType(24)
var UNIT_TYPE_ETHEREAL=ConvertUnitType(25)
var UNIT_TYPE_MAGIC_IMMUNE=ConvertUnitType(26)
var ITEM_TYPE_PERMANENT=ConvertItemType(0)
var ITEM_TYPE_CHARGED=ConvertItemType(1)
var ITEM_TYPE_POWERUP=ConvertItemType(2)
var ITEM_TYPE_ARTIFACT=ConvertItemType(3)
var ITEM_TYPE_PURCHASABLE=ConvertItemType(4)
var ITEM_TYPE_CAMPAIGN=ConvertItemType(5)
var ITEM_TYPE_MISCELLANEOUS=ConvertItemType(6)
var ITEM_TYPE_UNKNOWN=ConvertItemType(7)
var ITEM_TYPE_ANY=ConvertItemType(8)
var ITEM_TYPE_TOME=ConvertItemType(2)
var CAMERA_FIELD_TARGET_DISTANCE=ConvertCameraField(0)
var CAMERA_FIELD_FARZ=ConvertCameraField(1)
var CAMERA_FIELD_ANGLE_OF_ATTACK=ConvertCameraField(2)
var CAMERA_FIELD_FIELD_OF_VIEW=ConvertCameraField(3)
var CAMERA_FIELD_ROLL=ConvertCameraField(4)
var CAMERA_FIELD_ROTATION=ConvertCameraField(5)
var CAMERA_FIELD_ZOFFSET=ConvertCameraField(6)
var CAMERA_FIELD_NEARZ=ConvertCameraField(7)
var CAMERA_FIELD_LOCAL_PITCH=ConvertCameraField(8)
var CAMERA_FIELD_LOCAL_YAW=ConvertCameraField(9)
var CAMERA_FIELD_LOCAL_ROLL=ConvertCameraField(10)
var BLEND_MODE_NONE=ConvertBlendMode(0)
var BLEND_MODE_DONT_CARE=ConvertBlendMode(0)
var BLEND_MODE_KEYALPHA=ConvertBlendMode(1)
var BLEND_MODE_BLEND=ConvertBlendMode(2)
var BLEND_MODE_ADDITIVE=ConvertBlendMode(3)
var BLEND_MODE_MODULATE=ConvertBlendMode(4)
var BLEND_MODE_MODULATE_2X=ConvertBlendMode(5)
var RARITY_FREQUENT=ConvertRarityControl(0)
var RARITY_RARE=ConvertRarityControl(1)
var TEXMAP_FLAG_NONE=ConvertTexMapFlags(0)
var TEXMAP_FLAG_WRAP_U=ConvertTexMapFlags(1)
var TEXMAP_FLAG_WRAP_V=ConvertTexMapFlags(2)
var TEXMAP_FLAG_WRAP_UV=ConvertTexMapFlags(3)
var FOG_OF_WAR_MASKED=ConvertFogState(1)
var FOG_OF_WAR_FOGGED=ConvertFogState(2)
var FOG_OF_WAR_VISIBLE=ConvertFogState(4)
var CAMERA_MARGIN_LEFT=0
var CAMERA_MARGIN_RIGHT=1
var CAMERA_MARGIN_TOP=2
var CAMERA_MARGIN_BOTTOM=3
var EFFECT_TYPE_EFFECT=ConvertEffectType(0)
var EFFECT_TYPE_TARGET=ConvertEffectType(1)
var EFFECT_TYPE_CASTER=ConvertEffectType(2)
var EFFECT_TYPE_SPECIAL=ConvertEffectType(3)
var EFFECT_TYPE_AREA_EFFECT=ConvertEffectType(4)
var EFFECT_TYPE_MISSILE=ConvertEffectType(5)
var EFFECT_TYPE_LIGHTNING=ConvertEffectType(6)
var SOUND_TYPE_EFFECT=ConvertSoundType(0)
var SOUND_TYPE_EFFECT_LOOPED=ConvertSoundType(1)
var ORIGIN_FRAME_GAME_UI=ConvertOriginFrameType(0)
var ORIGIN_FRAME_COMMAND_BUTTON=ConvertOriginFrameType(1)
var ORIGIN_FRAME_HERO_BAR=ConvertOriginFrameType(2)
var ORIGIN_FRAME_HERO_BUTTON=ConvertOriginFrameType(3)
var ORIGIN_FRAME_HERO_HP_BAR=ConvertOriginFrameType(4)
var ORIGIN_FRAME_HERO_MANA_BAR=ConvertOriginFrameType(5)
var ORIGIN_FRAME_HERO_BUTTON_INDICATOR=ConvertOriginFrameType(6)
var ORIGIN_FRAME_ITEM_BUTTON=ConvertOriginFrameType(7)
var ORIGIN_FRAME_MINIMAP=ConvertOriginFrameType(8)
var ORIGIN_FRAME_MINIMAP_BUTTON=ConvertOriginFrameType(9)
var ORIGIN_FRAME_SYSTEM_BUTTON=ConvertOriginFrameType(10)
var ORIGIN_FRAME_TOOLTIP=ConvertOriginFrameType(11)
var ORIGIN_FRAME_UBERTOOLTIP=ConvertOriginFrameType(12)
var ORIGIN_FRAME_CHAT_MSG=ConvertOriginFrameType(13)
var ORIGIN_FRAME_UNIT_MSG=ConvertOriginFrameType(14)
var ORIGIN_FRAME_TOP_MSG=ConvertOriginFrameType(15)
var ORIGIN_FRAME_PORTRAIT=ConvertOriginFrameType(16)
var ORIGIN_FRAME_WORLD_FRAME=ConvertOriginFrameType(17)
var ORIGIN_FRAME_SIMPLE_UI_PARENT=ConvertOriginFrameType(18)
var ORIGIN_FRAME_PORTRAIT_HP_TEXT=ConvertOriginFrameType(19)
var ORIGIN_FRAME_PORTRAIT_MANA_TEXT=ConvertOriginFrameType(20)
var ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR=ConvertOriginFrameType(21)
var ORIGIN_FRAME_UNIT_PANEL_BUFF_BAR_LABEL=ConvertOriginFrameType(22)
var FRAMEPOINT_TOPLEFT=ConvertFramePointType(0)
var FRAMEPOINT_TOP=ConvertFramePointType(1)
var FRAMEPOINT_TOPRIGHT=ConvertFramePointType(2)
var FRAMEPOINT_LEFT=ConvertFramePointType(3)
var FRAMEPOINT_CENTER=ConvertFramePointType(4)
var FRAMEPOINT_RIGHT=ConvertFramePointType(5)
var FRAMEPOINT_BOTTOMLEFT=ConvertFramePointType(6)
var FRAMEPOINT_BOTTOM=ConvertFramePointType(7)
var FRAMEPOINT_BOTTOMRIGHT=ConvertFramePointType(8)
var TEXT_JUSTIFY_TOP=ConvertTextAlignType(0)
var TEXT_JUSTIFY_MIDDLE=ConvertTextAlignType(1)
var TEXT_JUSTIFY_BOTTOM=ConvertTextAlignType(2)
var TEXT_JUSTIFY_LEFT=ConvertTextAlignType(3)
var TEXT_JUSTIFY_CENTER=ConvertTextAlignType(4)
var TEXT_JUSTIFY_RIGHT=ConvertTextAlignType(5)
var FRAMEEVENT_CONTROL_CLICK=ConvertFrameEventType(1)
var FRAMEEVENT_MOUSE_ENTER=ConvertFrameEventType(2)
var FRAMEEVENT_MOUSE_LEAVE=ConvertFrameEventType(3)
var FRAMEEVENT_MOUSE_UP=ConvertFrameEventType(4)
var FRAMEEVENT_MOUSE_DOWN=ConvertFrameEventType(5)
var FRAMEEVENT_MOUSE_WHEEL=ConvertFrameEventType(6)
var FRAMEEVENT_CHECKBOX_CHECKED=ConvertFrameEventType(7)
var FRAMEEVENT_CHECKBOX_UNCHECKED=ConvertFrameEventType(8)
var FRAMEEVENT_EDITBOX_TEXT_CHANGED=ConvertFrameEventType(9)
var FRAMEEVENT_POPUPMENU_ITEM_CHANGED=ConvertFrameEventType(10)
var FRAMEEVENT_MOUSE_DOUBLECLICK=ConvertFrameEventType(11)
var FRAMEEVENT_SPRITE_ANIM_UPDATE=ConvertFrameEventType(12)
var FRAMEEVENT_SLIDER_VALUE_CHANGED=ConvertFrameEventType(13)
var FRAMEEVENT_DIALOG_CANCEL=ConvertFrameEventType(14)
var FRAMEEVENT_DIALOG_ACCEPT=ConvertFrameEventType(15)
var FRAMEEVENT_EDITBOX_ENTER=ConvertFrameEventType(16)
var OSKEY_BACKSPACE=ConvertOsKeyType(8)
var OSKEY_TAB=ConvertOsKeyType(9)
var OSKEY_CLEAR=ConvertOsKeyType(12)
var OSKEY_RETURN=ConvertOsKeyType(13)
var OSKEY_SHIFT=ConvertOsKeyType(16)
var OSKEY_CONTROL=ConvertOsKeyType(17)
var OSKEY_ALT=ConvertOsKeyType(18)
var OSKEY_PAUSE=ConvertOsKeyType(19)
var OSKEY_CAPSLOCK=ConvertOsKeyType(20)
var OSKEY_KANA=ConvertOsKeyType(21)
var OSKEY_HANGUL=ConvertOsKeyType(21)
var OSKEY_JUNJA=ConvertOsKeyType(23)
var OSKEY_FINAL=ConvertOsKeyType(24)
var OSKEY_HANJA=ConvertOsKeyType(25)
var OSKEY_KANJI=ConvertOsKeyType(25)
var OSKEY_ESCAPE=ConvertOsKeyType(27)
var OSKEY_CONVERT=ConvertOsKeyType(28)
var OSKEY_NONCONVERT=ConvertOsKeyType(29)
var OSKEY_ACCEPT=ConvertOsKeyType(30)
var OSKEY_MODECHANGE=ConvertOsKeyType(31)
var OSKEY_SPACE=ConvertOsKeyType(32)
var OSKEY_PAGEUP=ConvertOsKeyType(33)
var OSKEY_PAGEDOWN=ConvertOsKeyType(34)
var OSKEY_END=ConvertOsKeyType(35)
var OSKEY_HOME=ConvertOsKeyType(36)
var OSKEY_LEFT=ConvertOsKeyType(37)
var OSKEY_UP=ConvertOsKeyType(38)
var OSKEY_RIGHT=ConvertOsKeyType(39)
var OSKEY_DOWN=ConvertOsKeyType(40)
var OSKEY_SELECT=ConvertOsKeyType(41)
var OSKEY_PRINT=ConvertOsKeyType(42)
var OSKEY_EXECUTE=ConvertOsKeyType(43)
var OSKEY_PRINTSCREEN=ConvertOsKeyType(44)
var OSKEY_INSERT=ConvertOsKeyType(45)
var OSKEY_DELETE=ConvertOsKeyType(46)
var OSKEY_HELP=ConvertOsKeyType(47)
var OSKEY_0=ConvertOsKeyType(48)
var OSKEY_1=ConvertOsKeyType(49)
var OSKEY_2=ConvertOsKeyType(50)
var OSKEY_3=ConvertOsKeyType(51)
var OSKEY_4=ConvertOsKeyType(52)
var OSKEY_5=ConvertOsKeyType(53)
var OSKEY_6=ConvertOsKeyType(54)
var OSKEY_7=ConvertOsKeyType(55)
var OSKEY_8=ConvertOsKeyType(56)
var OSKEY_9=ConvertOsKeyType(57)
var OSKEY_A=ConvertOsKeyType(65)
var OSKEY_B=ConvertOsKeyType(66)
var OSKEY_C=ConvertOsKeyType(67)
var OSKEY_D=ConvertOsKeyType(68)
var OSKEY_E=ConvertOsKeyType(69)
var OSKEY_F=ConvertOsKeyType(70)
var OSKEY_G=ConvertOsKeyType(71)
var OSKEY_H=ConvertOsKeyType(72)
var OSKEY_I=ConvertOsKeyType(73)
var OSKEY_J=ConvertOsKeyType(74)
var OSKEY_K=ConvertOsKeyType(75)
var OSKEY_L=ConvertOsKeyType(76)
var OSKEY_M=ConvertOsKeyType(77)
var OSKEY_N=ConvertOsKeyType(78)
var OSKEY_O=ConvertOsKeyType(79)
var OSKEY_P=ConvertOsKeyType(80)
var OSKEY_Q=ConvertOsKeyType(81)
var OSKEY_R=ConvertOsKeyType(82)
var OSKEY_S=ConvertOsKeyType(83)
var OSKEY_T=ConvertOsKeyType(84)
var OSKEY_U=ConvertOsKeyType(85)
var OSKEY_V=ConvertOsKeyType(86)
var OSKEY_W=ConvertOsKeyType(87)
var OSKEY_X=ConvertOsKeyType(88)
var OSKEY_Y=ConvertOsKeyType(89)
var OSKEY_Z=ConvertOsKeyType(90)
var OSKEY_LMETA=ConvertOsKeyType(91)
var OSKEY_RMETA=ConvertOsKeyType(92)
var OSKEY_APPS=ConvertOsKeyType(93)
var OSKEY_SLEEP=ConvertOsKeyType(95)
var OSKEY_NUMPAD0=ConvertOsKeyType(96)
var OSKEY_NUMPAD1=ConvertOsKeyType(97)
var OSKEY_NUMPAD2=ConvertOsKeyType(98)
var OSKEY_NUMPAD3=ConvertOsKeyType(99)
var OSKEY_NUMPAD4=ConvertOsKeyType(100)
var OSKEY_NUMPAD5=ConvertOsKeyType(101)
var OSKEY_NUMPAD6=ConvertOsKeyType(102)
var OSKEY_NUMPAD7=ConvertOsKeyType(103)
var OSKEY_NUMPAD8=ConvertOsKeyType(104)
var OSKEY_NUMPAD9=ConvertOsKeyType(105)
var OSKEY_MULTIPLY=ConvertOsKeyType(106)
var OSKEY_ADD=ConvertOsKeyType(107)
var OSKEY_SEPARATOR=ConvertOsKeyType(108)
var OSKEY_SUBTRACT=ConvertOsKeyType(109)
var OSKEY_DECIMAL=ConvertOsKeyType(110)
var OSKEY_DIVIDE=ConvertOsKeyType(111)
var OSKEY_F1=ConvertOsKeyType(112)
var OSKEY_F2=ConvertOsKeyType(113)
var OSKEY_F3=ConvertOsKeyType(114)
var OSKEY_F4=ConvertOsKeyType(115)
var OSKEY_F5=ConvertOsKeyType(116)
var OSKEY_F6=ConvertOsKeyType(117)
var OSKEY_F7=ConvertOsKeyType(118)
var OSKEY_F8=ConvertOsKeyType(119)
var OSKEY_F9=ConvertOsKeyType(120)
var OSKEY_F10=ConvertOsKeyType(121)
var OSKEY_F11=ConvertOsKeyType(122)
var OSKEY_F12=ConvertOsKeyType(123)
var OSKEY_F13=ConvertOsKeyType(124)
var OSKEY_F14=ConvertOsKeyType(125)
var OSKEY_F15=ConvertOsKeyType(126)
var OSKEY_F16=ConvertOsKeyType(127)
var OSKEY_F17=ConvertOsKeyType(128)
var OSKEY_F18=ConvertOsKeyType(129)
var OSKEY_F19=ConvertOsKeyType(130)
var OSKEY_F20=ConvertOsKeyType(131)
var OSKEY_F21=ConvertOsKeyType(132)
var OSKEY_F22=ConvertOsKeyType(133)
var OSKEY_F23=ConvertOsKeyType(134)
var OSKEY_F24=ConvertOsKeyType(135)
var OSKEY_NUMLOCK=ConvertOsKeyType(144)
var OSKEY_SCROLLLOCK=ConvertOsKeyType(145)
var OSKEY_OEM_NEC_EQUAL=ConvertOsKeyType(146)
var OSKEY_OEM_FJ_JISHO=ConvertOsKeyType(146)
var OSKEY_OEM_FJ_MASSHOU=ConvertOsKeyType(147)
var OSKEY_OEM_FJ_TOUROKU=ConvertOsKeyType(148)
var OSKEY_OEM_FJ_LOYA=ConvertOsKeyType(149)
var OSKEY_OEM_FJ_ROYA=ConvertOsKeyType(150)
var OSKEY_LSHIFT=ConvertOsKeyType(160)
var OSKEY_RSHIFT=ConvertOsKeyType(161)
var OSKEY_LCONTROL=ConvertOsKeyType(162)
var OSKEY_RCONTROL=ConvertOsKeyType(163)
var OSKEY_LALT=ConvertOsKeyType(164)
var OSKEY_RALT=ConvertOsKeyType(165)
var OSKEY_BROWSER_BACK=ConvertOsKeyType(166)
var OSKEY_BROWSER_FORWARD=ConvertOsKeyType(167)
var OSKEY_BROWSER_REFRESH=ConvertOsKeyType(168)
var OSKEY_BROWSER_STOP=ConvertOsKeyType(169)
var OSKEY_BROWSER_SEARCH=ConvertOsKeyType(170)
var OSKEY_BROWSER_FAVORITES=ConvertOsKeyType(171)
var OSKEY_BROWSER_HOME=ConvertOsKeyType(172)
var OSKEY_VOLUME_MUTE=ConvertOsKeyType(173)
var OSKEY_VOLUME_DOWN=ConvertOsKeyType(174)
var OSKEY_VOLUME_UP=ConvertOsKeyType(175)
var OSKEY_MEDIA_NEXT_TRACK=ConvertOsKeyType(176)
var OSKEY_MEDIA_PREV_TRACK=ConvertOsKeyType(177)
var OSKEY_MEDIA_STOP=ConvertOsKeyType(178)
var OSKEY_MEDIA_PLAY_PAUSE=ConvertOsKeyType(179)
var OSKEY_LAUNCH_MAIL=ConvertOsKeyType(180)
var OSKEY_LAUNCH_MEDIA_SELECT=ConvertOsKeyType(181)
var OSKEY_LAUNCH_APP1=ConvertOsKeyType(182)
var OSKEY_LAUNCH_APP2=ConvertOsKeyType(183)
var OSKEY_OEM_1=ConvertOsKeyType(186)
var OSKEY_OEM_PLUS=ConvertOsKeyType(187)
var OSKEY_OEM_COMMA=ConvertOsKeyType(188)
var OSKEY_OEM_MINUS=ConvertOsKeyType(189)
var OSKEY_OEM_PERIOD=ConvertOsKeyType(190)
var OSKEY_OEM_2=ConvertOsKeyType(191)
var OSKEY_OEM_3=ConvertOsKeyType(192)
var OSKEY_OEM_4=ConvertOsKeyType(219)
var OSKEY_OEM_5=ConvertOsKeyType(220)
var OSKEY_OEM_6=ConvertOsKeyType(221)
var OSKEY_OEM_7=ConvertOsKeyType(222)
var OSKEY_OEM_8=ConvertOsKeyType(223)
var OSKEY_OEM_AX=ConvertOsKeyType(225)
var OSKEY_OEM_102=ConvertOsKeyType(226)
var OSKEY_ICO_HELP=ConvertOsKeyType(227)
var OSKEY_ICO_00=ConvertOsKeyType(228)
var OSKEY_PROCESSKEY=ConvertOsKeyType(229)
var OSKEY_ICO_CLEAR=ConvertOsKeyType(230)
var OSKEY_PACKET=ConvertOsKeyType(231)
var OSKEY_OEM_RESET=ConvertOsKeyType(233)
var OSKEY_OEM_JUMP=ConvertOsKeyType(234)
var OSKEY_OEM_PA1=ConvertOsKeyType(235)
var OSKEY_OEM_PA2=ConvertOsKeyType(236)
var OSKEY_OEM_PA3=ConvertOsKeyType(237)
var OSKEY_OEM_WSCTRL=ConvertOsKeyType(238)
var OSKEY_OEM_CUSEL=ConvertOsKeyType(239)
var OSKEY_OEM_ATTN=ConvertOsKeyType(240)
var OSKEY_OEM_FINISH=ConvertOsKeyType(241)
var OSKEY_OEM_COPY=ConvertOsKeyType(242)
var OSKEY_OEM_AUTO=ConvertOsKeyType(243)
var OSKEY_OEM_ENLW=ConvertOsKeyType(244)
var OSKEY_OEM_BACKTAB=ConvertOsKeyType(245)
var OSKEY_ATTN=ConvertOsKeyType(246)
var OSKEY_CRSEL=ConvertOsKeyType(247)
var OSKEY_EXSEL=ConvertOsKeyType(248)
var OSKEY_EREOF=ConvertOsKeyType(249)
var OSKEY_PLAY=ConvertOsKeyType(250)
var OSKEY_ZOOM=ConvertOsKeyType(251)
var OSKEY_NONAME=ConvertOsKeyType(252)
var OSKEY_PA1=ConvertOsKeyType(253)
var OSKEY_OEM_CLEAR=ConvertOsKeyType(254)
var ABILITY_IF_BUTTON_POSITION_NORMAL_X=ConvertAbilityIntegerField('abpx')
var ABILITY_IF_BUTTON_POSITION_NORMAL_Y=ConvertAbilityIntegerField('abpy')
var ABILITY_IF_BUTTON_POSITION_ACTIVATED_X=ConvertAbilityIntegerField('aubx')
var ABILITY_IF_BUTTON_POSITION_ACTIVATED_Y=ConvertAbilityIntegerField('auby')
var ABILITY_IF_BUTTON_POSITION_RESEARCH_X=ConvertAbilityIntegerField('arpx')
var ABILITY_IF_BUTTON_POSITION_RESEARCH_Y=ConvertAbilityIntegerField('arpy')
var ABILITY_IF_MISSILE_SPEED=ConvertAbilityIntegerField('amsp')
var ABILITY_IF_TARGET_ATTACHMENTS=ConvertAbilityIntegerField('atac')
var ABILITY_IF_CASTER_ATTACHMENTS=ConvertAbilityIntegerField('acac')
var ABILITY_IF_PRIORITY=ConvertAbilityIntegerField('apri')
var ABILITY_IF_LEVELS=ConvertAbilityIntegerField('alev')
var ABILITY_IF_REQUIRED_LEVEL=ConvertAbilityIntegerField('arlv')
var ABILITY_IF_LEVEL_SKIP_REQUIREMENT=ConvertAbilityIntegerField('alsk')
var ABILITY_BF_HERO_ABILITY=ConvertAbilityBooleanField('aher')
var ABILITY_BF_ITEM_ABILITY=ConvertAbilityBooleanField('aite')
var ABILITY_BF_CHECK_DEPENDENCIES=ConvertAbilityBooleanField('achd')
var ABILITY_RF_ARF_MISSILE_ARC=ConvertAbilityRealField('amac')
var ABILITY_SF_NAME=ConvertAbilityStringField('anam')
var ABILITY_SF_ICON_ACTIVATED=ConvertAbilityStringField('auar')
var ABILITY_SF_ICON_RESEARCH=ConvertAbilityStringField('arar')
var ABILITY_SF_EFFECT_SOUND=ConvertAbilityStringField('aefs')
var ABILITY_SF_EFFECT_SOUND_LOOPING=ConvertAbilityStringField('aefl')
var ABILITY_ILF_MANA_COST=ConvertAbilityIntegerLevelField('amcs')
var ABILITY_ILF_NUMBER_OF_WAVES=ConvertAbilityIntegerLevelField('Hbz1')
var ABILITY_ILF_NUMBER_OF_SHARDS=ConvertAbilityIntegerLevelField('Hbz3')
var ABILITY_ILF_NUMBER_OF_UNITS_TELEPORTED=ConvertAbilityIntegerLevelField('Hmt1')
var ABILITY_ILF_SUMMONED_UNIT_COUNT_HWE2=ConvertAbilityIntegerLevelField('Hwe2')
var ABILITY_ILF_NUMBER_OF_IMAGES=ConvertAbilityIntegerLevelField('Omi1')
var ABILITY_ILF_NUMBER_OF_CORPSES_RAISED_UAN1=ConvertAbilityIntegerLevelField('Uan1')
var ABILITY_ILF_MORPHING_FLAGS=ConvertAbilityIntegerLevelField('Eme2')
var ABILITY_ILF_STRENGTH_BONUS_NRG5=ConvertAbilityIntegerLevelField('Nrg5')
var ABILITY_ILF_DEFENSE_BONUS_NRG6=ConvertAbilityIntegerLevelField('Nrg6')
var ABILITY_ILF_NUMBER_OF_TARGETS_HIT=ConvertAbilityIntegerLevelField('Ocl2')
var ABILITY_ILF_DETECTION_TYPE_OFS1=ConvertAbilityIntegerLevelField('Ofs1')
var ABILITY_ILF_NUMBER_OF_SUMMONED_UNITS_OSF2=ConvertAbilityIntegerLevelField('Osf2')
var ABILITY_ILF_NUMBER_OF_SUMMONED_UNITS_EFN1=ConvertAbilityIntegerLevelField('Efn1')
var ABILITY_ILF_NUMBER_OF_CORPSES_RAISED_HRE1=ConvertAbilityIntegerLevelField('Hre1')
var ABILITY_ILF_STACK_FLAGS=ConvertAbilityIntegerLevelField('Hca4')
var ABILITY_ILF_MINIMUM_NUMBER_OF_UNITS=ConvertAbilityIntegerLevelField('Ndp2')
var ABILITY_ILF_MAXIMUM_NUMBER_OF_UNITS_NDP3=ConvertAbilityIntegerLevelField('Ndp3')
var ABILITY_ILF_NUMBER_OF_UNITS_CREATED_NRC2=ConvertAbilityIntegerLevelField('Nrc2')
var ABILITY_ILF_SHIELD_LIFE=ConvertAbilityIntegerLevelField('Ams3')
var ABILITY_ILF_MANA_LOSS_AMS4=ConvertAbilityIntegerLevelField('Ams4')
var ABILITY_ILF_GOLD_PER_INTERVAL_BGM1=ConvertAbilityIntegerLevelField('Bgm1')
var ABILITY_ILF_MAX_NUMBER_OF_MINERS=ConvertAbilityIntegerLevelField('Bgm3')
var ABILITY_ILF_CARGO_CAPACITY=ConvertAbilityIntegerLevelField('Car1')
var ABILITY_ILF_MAXIMUM_CREEP_LEVEL_DEV3=ConvertAbilityIntegerLevelField('Dev3')
var ABILITY_ILF_MAX_CREEP_LEVEL_DEV1=ConvertAbilityIntegerLevelField('Dev1')
var ABILITY_ILF_GOLD_PER_INTERVAL_EGM1=ConvertAbilityIntegerLevelField('Egm1')
var ABILITY_ILF_DEFENSE_REDUCTION=ConvertAbilityIntegerLevelField('Fae1')
var ABILITY_ILF_DETECTION_TYPE_FLA1=ConvertAbilityIntegerLevelField('Fla1')
var ABILITY_ILF_FLARE_COUNT=ConvertAbilityIntegerLevelField('Fla3')
var ABILITY_ILF_MAX_GOLD=ConvertAbilityIntegerLevelField('Gld1')
var ABILITY_ILF_MINING_CAPACITY=ConvertAbilityIntegerLevelField('Gld3')
var ABILITY_ILF_MAXIMUM_NUMBER_OF_CORPSES_GYD1=ConvertAbilityIntegerLevelField('Gyd1')
var ABILITY_ILF_DAMAGE_TO_TREE=ConvertAbilityIntegerLevelField('Har1')
var ABILITY_ILF_LUMBER_CAPACITY=ConvertAbilityIntegerLevelField('Har2')
var ABILITY_ILF_GOLD_CAPACITY=ConvertAbilityIntegerLevelField('Har3')
var ABILITY_ILF_DEFENSE_INCREASE_INF2=ConvertAbilityIntegerLevelField('Inf2')
var ABILITY_ILF_INTERACTION_TYPE=ConvertAbilityIntegerLevelField('Neu2')
var ABILITY_ILF_GOLD_COST_NDT1=ConvertAbilityIntegerLevelField('Ndt1')
var ABILITY_ILF_LUMBER_COST_NDT2=ConvertAbilityIntegerLevelField('Ndt2')
var ABILITY_ILF_DETECTION_TYPE_NDT3=ConvertAbilityIntegerLevelField('Ndt3')
var ABILITY_ILF_STACKING_TYPE_POI4=ConvertAbilityIntegerLevelField('Poi4')
var ABILITY_ILF_STACKING_TYPE_POA5=ConvertAbilityIntegerLevelField('Poa5')
var ABILITY_ILF_MAXIMUM_CREEP_LEVEL_PLY1=ConvertAbilityIntegerLevelField('Ply1')
var ABILITY_ILF_MAXIMUM_CREEP_LEVEL_POS1=ConvertAbilityIntegerLevelField('Pos1')
var ABILITY_ILF_MOVEMENT_UPDATE_FREQUENCY_PRG1=ConvertAbilityIntegerLevelField('Prg1')
var ABILITY_ILF_ATTACK_UPDATE_FREQUENCY_PRG2=ConvertAbilityIntegerLevelField('Prg2')
var ABILITY_ILF_MANA_LOSS_PRG6=ConvertAbilityIntegerLevelField('Prg6')
var ABILITY_ILF_UNITS_SUMMONED_TYPE_ONE=ConvertAbilityIntegerLevelField('Rai1')
var ABILITY_ILF_UNITS_SUMMONED_TYPE_TWO=ConvertAbilityIntegerLevelField('Rai2')
var ABILITY_ILF_MAX_UNITS_SUMMONED=ConvertAbilityIntegerLevelField('Ucb5')
var ABILITY_ILF_ALLOW_WHEN_FULL_REJ3=ConvertAbilityIntegerLevelField('Rej3')
var ABILITY_ILF_MAXIMUM_UNITS_CHARGED_TO_CASTER=ConvertAbilityIntegerLevelField('Rpb5')
var ABILITY_ILF_MAXIMUM_UNITS_AFFECTED=ConvertAbilityIntegerLevelField('Rpb6')
var ABILITY_ILF_DEFENSE_INCREASE_ROA2=ConvertAbilityIntegerLevelField('Roa2')
var ABILITY_ILF_MAX_UNITS_ROA7=ConvertAbilityIntegerLevelField('Roa7')
var ABILITY_ILF_ROOTED_WEAPONS=ConvertAbilityIntegerLevelField('Roo1')
var ABILITY_ILF_UPROOTED_WEAPONS=ConvertAbilityIntegerLevelField('Roo2')
var ABILITY_ILF_UPROOTED_DEFENSE_TYPE=ConvertAbilityIntegerLevelField('Roo4')
var ABILITY_ILF_ACCUMULATION_STEP=ConvertAbilityIntegerLevelField('Sal2')
var ABILITY_ILF_NUMBER_OF_OWLS=ConvertAbilityIntegerLevelField('Esn4')
var ABILITY_ILF_STACKING_TYPE_SPO4=ConvertAbilityIntegerLevelField('Spo4')
var ABILITY_ILF_NUMBER_OF_UNITS=ConvertAbilityIntegerLevelField('Sod1')
var ABILITY_ILF_SPIDER_CAPACITY=ConvertAbilityIntegerLevelField('Spa1')
var ABILITY_ILF_INTERVALS_BEFORE_CHANGING_TREES=ConvertAbilityIntegerLevelField('Wha2')
var ABILITY_ILF_AGILITY_BONUS=ConvertAbilityIntegerLevelField('Iagi')
var ABILITY_ILF_INTELLIGENCE_BONUS=ConvertAbilityIntegerLevelField('Iint')
var ABILITY_ILF_STRENGTH_BONUS_ISTR=ConvertAbilityIntegerLevelField('Istr')
var ABILITY_ILF_ATTACK_BONUS=ConvertAbilityIntegerLevelField('Iatt')
var ABILITY_ILF_DEFENSE_BONUS_IDEF=ConvertAbilityIntegerLevelField('Idef')
var ABILITY_ILF_SUMMON_1_AMOUNT=ConvertAbilityIntegerLevelField('Isn1')
var ABILITY_ILF_SUMMON_2_AMOUNT=ConvertAbilityIntegerLevelField('Isn2')
var ABILITY_ILF_EXPERIENCE_GAINED=ConvertAbilityIntegerLevelField('Ixpg')
var ABILITY_ILF_HIT_POINTS_GAINED_IHPG=ConvertAbilityIntegerLevelField('Ihpg')
var ABILITY_ILF_MANA_POINTS_GAINED_IMPG=ConvertAbilityIntegerLevelField('Impg')
var ABILITY_ILF_HIT_POINTS_GAINED_IHP2=ConvertAbilityIntegerLevelField('Ihp2')
var ABILITY_ILF_MANA_POINTS_GAINED_IMP2=ConvertAbilityIntegerLevelField('Imp2')
var ABILITY_ILF_DAMAGE_BONUS_DICE=ConvertAbilityIntegerLevelField('Idic')
var ABILITY_ILF_ARMOR_PENALTY_IARP=ConvertAbilityIntegerLevelField('Iarp')
var ABILITY_ILF_ENABLED_ATTACK_INDEX_IOB5=ConvertAbilityIntegerLevelField('Iob5')
var ABILITY_ILF_LEVELS_GAINED=ConvertAbilityIntegerLevelField('Ilev')
var ABILITY_ILF_MAX_LIFE_GAINED=ConvertAbilityIntegerLevelField('Ilif')
var ABILITY_ILF_MAX_MANA_GAINED=ConvertAbilityIntegerLevelField('Iman')
var ABILITY_ILF_GOLD_GIVEN=ConvertAbilityIntegerLevelField('Igol')
var ABILITY_ILF_LUMBER_GIVEN=ConvertAbilityIntegerLevelField('Ilum')
var ABILITY_ILF_DETECTION_TYPE_IFA1=ConvertAbilityIntegerLevelField('Ifa1')
var ABILITY_ILF_MAXIMUM_CREEP_LEVEL_ICRE=ConvertAbilityIntegerLevelField('Icre')
var ABILITY_ILF_MOVEMENT_SPEED_BONUS=ConvertAbilityIntegerLevelField('Imvb')
var ABILITY_ILF_HIT_POINTS_REGENERATED_PER_SECOND=ConvertAbilityIntegerLevelField('Ihpr')
var ABILITY_ILF_SIGHT_RANGE_BONUS=ConvertAbilityIntegerLevelField('Isib')
var ABILITY_ILF_DAMAGE_PER_DURATION=ConvertAbilityIntegerLevelField('Icfd')
var ABILITY_ILF_MANA_USED_PER_SECOND=ConvertAbilityIntegerLevelField('Icfm')
var ABILITY_ILF_EXTRA_MANA_REQUIRED=ConvertAbilityIntegerLevelField('Icfx')
var ABILITY_ILF_DETECTION_RADIUS_IDET=ConvertAbilityIntegerLevelField('Idet')
var ABILITY_ILF_MANA_LOSS_PER_UNIT_IDIM=ConvertAbilityIntegerLevelField('Idim')
var ABILITY_ILF_DAMAGE_TO_SUMMONED_UNITS_IDID=ConvertAbilityIntegerLevelField('Idid')
var ABILITY_ILF_MAXIMUM_NUMBER_OF_UNITS_IREC=ConvertAbilityIntegerLevelField('Irec')
var ABILITY_ILF_DELAY_AFTER_DEATH_SECONDS=ConvertAbilityIntegerLevelField('Ircd')
var ABILITY_ILF_RESTORED_LIFE=ConvertAbilityIntegerLevelField('irc2')
var ABILITY_ILF_RESTORED_MANA__1_FOR_CURRENT=ConvertAbilityIntegerLevelField('irc3')
var ABILITY_ILF_HIT_POINTS_RESTORED=ConvertAbilityIntegerLevelField('Ihps')
var ABILITY_ILF_MANA_POINTS_RESTORED=ConvertAbilityIntegerLevelField('Imps')
var ABILITY_ILF_MAXIMUM_NUMBER_OF_UNITS_ITPM=ConvertAbilityIntegerLevelField('Itpm')
var ABILITY_ILF_NUMBER_OF_CORPSES_RAISED_CAD1=ConvertAbilityIntegerLevelField('Cad1')
var ABILITY_ILF_TERRAIN_DEFORMATION_DURATION_MS=ConvertAbilityIntegerLevelField('Wrs3')
var ABILITY_ILF_MAXIMUM_UNITS=ConvertAbilityIntegerLevelField('Uds1')
var ABILITY_ILF_DETECTION_TYPE_DET1=ConvertAbilityIntegerLevelField('Det1')
var ABILITY_ILF_GOLD_COST_PER_STRUCTURE=ConvertAbilityIntegerLevelField('Nsp1')
var ABILITY_ILF_LUMBER_COST_PER_USE=ConvertAbilityIntegerLevelField('Nsp2')
var ABILITY_ILF_DETECTION_TYPE_NSP3=ConvertAbilityIntegerLevelField('Nsp3')
var ABILITY_ILF_NUMBER_OF_SWARM_UNITS=ConvertAbilityIntegerLevelField('Uls1')
var ABILITY_ILF_MAX_SWARM_UNITS_PER_TARGET=ConvertAbilityIntegerLevelField('Uls3')
var ABILITY_ILF_NUMBER_OF_SUMMONED_UNITS_NBA2=ConvertAbilityIntegerLevelField('Nba2')
var ABILITY_ILF_MAXIMUM_CREEP_LEVEL_NCH1=ConvertAbilityIntegerLevelField('Nch1')
var ABILITY_ILF_ATTACKS_PREVENTED=ConvertAbilityIntegerLevelField('Nsi1')
var ABILITY_ILF_MAXIMUM_NUMBER_OF_TARGETS_EFK3=ConvertAbilityIntegerLevelField('Efk3')
var ABILITY_ILF_NUMBER_OF_SUMMONED_UNITS_ESV1=ConvertAbilityIntegerLevelField('Esv1')
var ABILITY_ILF_MAXIMUM_NUMBER_OF_CORPSES_EXH1=ConvertAbilityIntegerLevelField('exh1')
var ABILITY_ILF_ITEM_CAPACITY=ConvertAbilityIntegerLevelField('inv1')
var ABILITY_ILF_MAXIMUM_NUMBER_OF_TARGETS_SPL2=ConvertAbilityIntegerLevelField('spl2')
var ABILITY_ILF_ALLOW_WHEN_FULL_IRL3=ConvertAbilityIntegerLevelField('irl3')
var ABILITY_ILF_MAXIMUM_DISPELLED_UNITS=ConvertAbilityIntegerLevelField('idc3')
var ABILITY_ILF_NUMBER_OF_LURES=ConvertAbilityIntegerLevelField('imo1')
var ABILITY_ILF_NEW_TIME_OF_DAY_HOUR=ConvertAbilityIntegerLevelField('ict1')
var ABILITY_ILF_NEW_TIME_OF_DAY_MINUTE=ConvertAbilityIntegerLevelField('ict2')
var ABILITY_ILF_NUMBER_OF_UNITS_CREATED_MEC1=ConvertAbilityIntegerLevelField('mec1')
var ABILITY_ILF_MINIMUM_SPELLS=ConvertAbilityIntegerLevelField('spb3')
var ABILITY_ILF_MAXIMUM_SPELLS=ConvertAbilityIntegerLevelField('spb4')
var ABILITY_ILF_DISABLED_ATTACK_INDEX=ConvertAbilityIntegerLevelField('gra3')
var ABILITY_ILF_ENABLED_ATTACK_INDEX_GRA4=ConvertAbilityIntegerLevelField('gra4')
var ABILITY_ILF_MAXIMUM_ATTACKS=ConvertAbilityIntegerLevelField('gra5')
var ABILITY_ILF_BUILDING_TYPES_ALLOWED_NPR1=ConvertAbilityIntegerLevelField('Npr1')
var ABILITY_ILF_BUILDING_TYPES_ALLOWED_NSA1=ConvertAbilityIntegerLevelField('Nsa1')
var ABILITY_ILF_ATTACK_MODIFICATION=ConvertAbilityIntegerLevelField('Iaa1')
var ABILITY_ILF_SUMMONED_UNIT_COUNT_NPA5=ConvertAbilityIntegerLevelField('Npa5')
var ABILITY_ILF_UPGRADE_LEVELS=ConvertAbilityIntegerLevelField('Igl1')
var ABILITY_ILF_NUMBER_OF_SUMMONED_UNITS_NDO2=ConvertAbilityIntegerLevelField('Ndo2')
var ABILITY_ILF_BEASTS_PER_SECOND=ConvertAbilityIntegerLevelField('Nst1')
var ABILITY_ILF_TARGET_TYPE=ConvertAbilityIntegerLevelField('Ncl2')
var ABILITY_ILF_OPTIONS=ConvertAbilityIntegerLevelField('Ncl3')
var ABILITY_ILF_ARMOR_PENALTY_NAB3=ConvertAbilityIntegerLevelField('Nab3')
var ABILITY_ILF_WAVE_COUNT_NHS6=ConvertAbilityIntegerLevelField('Nhs6')
var ABILITY_ILF_MAX_CREEP_LEVEL_NTM3=ConvertAbilityIntegerLevelField('Ntm3')
var ABILITY_ILF_MISSILE_COUNT=ConvertAbilityIntegerLevelField('Ncs3')
var ABILITY_ILF_SPLIT_ATTACK_COUNT=ConvertAbilityIntegerLevelField('Nlm3')
var ABILITY_ILF_GENERATION_COUNT=ConvertAbilityIntegerLevelField('Nlm6')
var ABILITY_ILF_ROCK_RING_COUNT=ConvertAbilityIntegerLevelField('Nvc1')
var ABILITY_ILF_WAVE_COUNT_NVC2=ConvertAbilityIntegerLevelField('Nvc2')
var ABILITY_ILF_PREFER_HOSTILES_TAU1=ConvertAbilityIntegerLevelField('Tau1')
var ABILITY_ILF_PREFER_FRIENDLIES_TAU2=ConvertAbilityIntegerLevelField('Tau2')
var ABILITY_ILF_MAX_UNITS_TAU3=ConvertAbilityIntegerLevelField('Tau3')
var ABILITY_ILF_NUMBER_OF_PULSES=ConvertAbilityIntegerLevelField('Tau4')
var ABILITY_ILF_SUMMONED_UNIT_TYPE_HWE1=ConvertAbilityIntegerLevelField('Hwe1')
var ABILITY_ILF_SUMMONED_UNIT_UIN4=ConvertAbilityIntegerLevelField('Uin4')
var ABILITY_ILF_SUMMONED_UNIT_OSF1=ConvertAbilityIntegerLevelField('Osf1')
var ABILITY_ILF_SUMMONED_UNIT_TYPE_EFNU=ConvertAbilityIntegerLevelField('Efnu')
var ABILITY_ILF_SUMMONED_UNIT_TYPE_NBAU=ConvertAbilityIntegerLevelField('Nbau')
var ABILITY_ILF_SUMMONED_UNIT_TYPE_NTOU=ConvertAbilityIntegerLevelField('Ntou')
var ABILITY_ILF_SUMMONED_UNIT_TYPE_ESVU=ConvertAbilityIntegerLevelField('Esvu')
var ABILITY_ILF_SUMMONED_UNIT_TYPES=ConvertAbilityIntegerLevelField('Nef1')
var ABILITY_ILF_SUMMONED_UNIT_TYPE_NDOU=ConvertAbilityIntegerLevelField('Ndou')
var ABILITY_ILF_ALTERNATE_FORM_UNIT_EMEU=ConvertAbilityIntegerLevelField('Emeu')
var ABILITY_ILF_PLAGUE_WARD_UNIT_TYPE=ConvertAbilityIntegerLevelField('Aplu')
var ABILITY_ILF_ALLOWED_UNIT_TYPE_BTL1=ConvertAbilityIntegerLevelField('Btl1')
var ABILITY_ILF_NEW_UNIT_TYPE=ConvertAbilityIntegerLevelField('Cha1')
var ABILITY_ILF_RESULTING_UNIT_TYPE_ENT1=ConvertAbilityIntegerLevelField('ent1')
var ABILITY_ILF_CORPSE_UNIT_TYPE=ConvertAbilityIntegerLevelField('Gydu')
var ABILITY_ILF_ALLOWED_UNIT_TYPE_LOA1=ConvertAbilityIntegerLevelField('Loa1')
var ABILITY_ILF_UNIT_TYPE_FOR_LIMIT_CHECK=ConvertAbilityIntegerLevelField('Raiu')
var ABILITY_ILF_WARD_UNIT_TYPE_STAU=ConvertAbilityIntegerLevelField('Stau')
var ABILITY_ILF_EFFECT_ABILITY=ConvertAbilityIntegerLevelField('Iobu')
var ABILITY_ILF_CONVERSION_UNIT=ConvertAbilityIntegerLevelField('Ndc2')
var ABILITY_ILF_UNIT_TO_PRESERVE=ConvertAbilityIntegerLevelField('Nsl1')
var ABILITY_ILF_UNIT_TYPE_ALLOWED=ConvertAbilityIntegerLevelField('Chl1')
var ABILITY_ILF_SWARM_UNIT_TYPE=ConvertAbilityIntegerLevelField('Ulsu')
var ABILITY_ILF_RESULTING_UNIT_TYPE_COAU=ConvertAbilityIntegerLevelField('coau')
var ABILITY_ILF_UNIT_TYPE_EXHU=ConvertAbilityIntegerLevelField('exhu')
var ABILITY_ILF_WARD_UNIT_TYPE_HWDU=ConvertAbilityIntegerLevelField('hwdu')
var ABILITY_ILF_LURE_UNIT_TYPE=ConvertAbilityIntegerLevelField('imou')
var ABILITY_ILF_UNIT_TYPE_IPMU=ConvertAbilityIntegerLevelField('ipmu')
var ABILITY_ILF_FACTORY_UNIT_ID=ConvertAbilityIntegerLevelField('Nsyu')
var ABILITY_ILF_SPAWN_UNIT_ID_NFYU=ConvertAbilityIntegerLevelField('Nfyu')
var ABILITY_ILF_DESTRUCTIBLE_ID=ConvertAbilityIntegerLevelField('Nvcu')
var ABILITY_ILF_UPGRADE_TYPE=ConvertAbilityIntegerLevelField('Iglu')
var ABILITY_RLF_CASTING_TIME=ConvertAbilityRealLevelField('acas')
var ABILITY_RLF_DURATION_NORMAL=ConvertAbilityRealLevelField('adur')
var ABILITY_RLF_DURATION_HERO=ConvertAbilityRealLevelField('ahdu')
var ABILITY_RLF_COOLDOWN=ConvertAbilityRealLevelField('acdn')
var ABILITY_RLF_AREA_OF_EFFECT=ConvertAbilityRealLevelField('aare')
var ABILITY_RLF_CAST_RANGE=ConvertAbilityRealLevelField('aran')
var ABILITY_RLF_DAMAGE_HBZ2=ConvertAbilityRealLevelField('Hbz2')
var ABILITY_RLF_BUILDING_REDUCTION_HBZ4=ConvertAbilityRealLevelField('Hbz4')
var ABILITY_RLF_DAMAGE_PER_SECOND_HBZ5=ConvertAbilityRealLevelField('Hbz5')
var ABILITY_RLF_MAXIMUM_DAMAGE_PER_WAVE=ConvertAbilityRealLevelField('Hbz6')
var ABILITY_RLF_MANA_REGENERATION_INCREASE=ConvertAbilityRealLevelField('Hab1')
var ABILITY_RLF_CASTING_DELAY=ConvertAbilityRealLevelField('Hmt2')
var ABILITY_RLF_DAMAGE_PER_SECOND_OWW1=ConvertAbilityRealLevelField('Oww1')
var ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_OWW2=ConvertAbilityRealLevelField('Oww2')
var ABILITY_RLF_CHANCE_TO_CRITICAL_STRIKE=ConvertAbilityRealLevelField('Ocr1')
var ABILITY_RLF_DAMAGE_MULTIPLIER_OCR2=ConvertAbilityRealLevelField('Ocr2')
var ABILITY_RLF_DAMAGE_BONUS_OCR3=ConvertAbilityRealLevelField('Ocr3')
var ABILITY_RLF_CHANCE_TO_EVADE_OCR4=ConvertAbilityRealLevelField('Ocr4')
var ABILITY_RLF_DAMAGE_DEALT_PERCENT_OMI2=ConvertAbilityRealLevelField('Omi2')
var ABILITY_RLF_DAMAGE_TAKEN_PERCENT_OMI3=ConvertAbilityRealLevelField('Omi3')
var ABILITY_RLF_ANIMATION_DELAY=ConvertAbilityRealLevelField('Omi4')
var ABILITY_RLF_TRANSITION_TIME=ConvertAbilityRealLevelField('Owk1')
var ABILITY_RLF_MOVEMENT_SPEED_INCREASE_PERCENT_OWK2=ConvertAbilityRealLevelField('Owk2')
var ABILITY_RLF_BACKSTAB_DAMAGE=ConvertAbilityRealLevelField('Owk3')
var ABILITY_RLF_AMOUNT_HEALED_DAMAGED_UDC1=ConvertAbilityRealLevelField('Udc1')
var ABILITY_RLF_LIFE_CONVERTED_TO_MANA=ConvertAbilityRealLevelField('Udp1')
var ABILITY_RLF_LIFE_CONVERTED_TO_LIFE=ConvertAbilityRealLevelField('Udp2')
var ABILITY_RLF_MOVEMENT_SPEED_INCREASE_PERCENT_UAU1=ConvertAbilityRealLevelField('Uau1')
var ABILITY_RLF_LIFE_REGENERATION_INCREASE_PERCENT=ConvertAbilityRealLevelField('Uau2')
var ABILITY_RLF_CHANCE_TO_EVADE_EEV1=ConvertAbilityRealLevelField('Eev1')
var ABILITY_RLF_DAMAGE_PER_INTERVAL=ConvertAbilityRealLevelField('Eim1')
var ABILITY_RLF_MANA_DRAINED_PER_SECOND_EIM2=ConvertAbilityRealLevelField('Eim2')
var ABILITY_RLF_BUFFER_MANA_REQUIRED=ConvertAbilityRealLevelField('Eim3')
var ABILITY_RLF_MAX_MANA_DRAINED=ConvertAbilityRealLevelField('Emb1')
var ABILITY_RLF_BOLT_DELAY=ConvertAbilityRealLevelField('Emb2')
var ABILITY_RLF_BOLT_LIFETIME=ConvertAbilityRealLevelField('Emb3')
var ABILITY_RLF_ALTITUDE_ADJUSTMENT_DURATION=ConvertAbilityRealLevelField('Eme3')
var ABILITY_RLF_LANDING_DELAY_TIME=ConvertAbilityRealLevelField('Eme4')
var ABILITY_RLF_ALTERNATE_FORM_HIT_POINT_BONUS=ConvertAbilityRealLevelField('Eme5')
var ABILITY_RLF_MOVE_SPEED_BONUS_INFO_PANEL_ONLY=ConvertAbilityRealLevelField('Ncr5')
var ABILITY_RLF_ATTACK_SPEED_BONUS_INFO_PANEL_ONLY=ConvertAbilityRealLevelField('Ncr6')
var ABILITY_RLF_LIFE_REGENERATION_RATE_PER_SECOND=ConvertAbilityRealLevelField('ave5')
var ABILITY_RLF_STUN_DURATION_USL1=ConvertAbilityRealLevelField('Usl1')
var ABILITY_RLF_ATTACK_DAMAGE_STOLEN_PERCENT=ConvertAbilityRealLevelField('Uav1')
var ABILITY_RLF_DAMAGE_UCS1=ConvertAbilityRealLevelField('Ucs1')
var ABILITY_RLF_MAX_DAMAGE_UCS2=ConvertAbilityRealLevelField('Ucs2')
var ABILITY_RLF_DISTANCE_UCS3=ConvertAbilityRealLevelField('Ucs3')
var ABILITY_RLF_FINAL_AREA_UCS4=ConvertAbilityRealLevelField('Ucs4')
var ABILITY_RLF_DAMAGE_UIN1=ConvertAbilityRealLevelField('Uin1')
var ABILITY_RLF_DURATION=ConvertAbilityRealLevelField('Uin2')
var ABILITY_RLF_IMPACT_DELAY=ConvertAbilityRealLevelField('Uin3')
var ABILITY_RLF_DAMAGE_PER_TARGET_OCL1=ConvertAbilityRealLevelField('Ocl1')
var ABILITY_RLF_DAMAGE_REDUCTION_PER_TARGET=ConvertAbilityRealLevelField('Ocl3')
var ABILITY_RLF_EFFECT_DELAY_OEQ1=ConvertAbilityRealLevelField('Oeq1')
var ABILITY_RLF_DAMAGE_PER_SECOND_TO_BUILDINGS=ConvertAbilityRealLevelField('Oeq2')
var ABILITY_RLF_UNITS_SLOWED_PERCENT=ConvertAbilityRealLevelField('Oeq3')
var ABILITY_RLF_FINAL_AREA_OEQ4=ConvertAbilityRealLevelField('Oeq4')
var ABILITY_RLF_DAMAGE_PER_SECOND_EER1=ConvertAbilityRealLevelField('Eer1')
var ABILITY_RLF_DAMAGE_DEALT_TO_ATTACKERS=ConvertAbilityRealLevelField('Eah1')
var ABILITY_RLF_LIFE_HEALED=ConvertAbilityRealLevelField('Etq1')
var ABILITY_RLF_HEAL_INTERVAL=ConvertAbilityRealLevelField('Etq2')
var ABILITY_RLF_BUILDING_REDUCTION_ETQ3=ConvertAbilityRealLevelField('Etq3')
var ABILITY_RLF_INITIAL_IMMUNITY_DURATION=ConvertAbilityRealLevelField('Etq4')
var ABILITY_RLF_MAX_LIFE_DRAINED_PER_SECOND_PERCENT=ConvertAbilityRealLevelField('Udd1')
var ABILITY_RLF_BUILDING_REDUCTION_UDD2=ConvertAbilityRealLevelField('Udd2')
var ABILITY_RLF_ARMOR_DURATION=ConvertAbilityRealLevelField('Ufa1')
var ABILITY_RLF_ARMOR_BONUS_UFA2=ConvertAbilityRealLevelField('Ufa2')
var ABILITY_RLF_AREA_OF_EFFECT_DAMAGE=ConvertAbilityRealLevelField('Ufn1')
var ABILITY_RLF_SPECIFIC_TARGET_DAMAGE_UFN2=ConvertAbilityRealLevelField('Ufn2')
var ABILITY_RLF_DAMAGE_BONUS_HFA1=ConvertAbilityRealLevelField('Hfa1')
var ABILITY_RLF_DAMAGE_DEALT_ESF1=ConvertAbilityRealLevelField('Esf1')
var ABILITY_RLF_DAMAGE_INTERVAL_ESF2=ConvertAbilityRealLevelField('Esf2')
var ABILITY_RLF_BUILDING_REDUCTION_ESF3=ConvertAbilityRealLevelField('Esf3')
var ABILITY_RLF_DAMAGE_BONUS_PERCENT=ConvertAbilityRealLevelField('Ear1')
var ABILITY_RLF_DEFENSE_BONUS_HAV1=ConvertAbilityRealLevelField('Hav1')
var ABILITY_RLF_HIT_POINT_BONUS=ConvertAbilityRealLevelField('Hav2')
var ABILITY_RLF_DAMAGE_BONUS_HAV3=ConvertAbilityRealLevelField('Hav3')
var ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_HAV4=ConvertAbilityRealLevelField('Hav4')
var ABILITY_RLF_CHANCE_TO_BASH=ConvertAbilityRealLevelField('Hbh1')
var ABILITY_RLF_DAMAGE_MULTIPLIER_HBH2=ConvertAbilityRealLevelField('Hbh2')
var ABILITY_RLF_DAMAGE_BONUS_HBH3=ConvertAbilityRealLevelField('Hbh3')
var ABILITY_RLF_CHANCE_TO_MISS_HBH4=ConvertAbilityRealLevelField('Hbh4')
var ABILITY_RLF_DAMAGE_HTB1=ConvertAbilityRealLevelField('Htb1')
var ABILITY_RLF_AOE_DAMAGE=ConvertAbilityRealLevelField('Htc1')
var ABILITY_RLF_SPECIFIC_TARGET_DAMAGE_HTC2=ConvertAbilityRealLevelField('Htc2')
var ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_PERCENT_HTC3=ConvertAbilityRealLevelField('Htc3')
var ABILITY_RLF_ATTACK_SPEED_REDUCTION_PERCENT_HTC4=ConvertAbilityRealLevelField('Htc4')
var ABILITY_RLF_ARMOR_BONUS_HAD1=ConvertAbilityRealLevelField('Had1')
var ABILITY_RLF_AMOUNT_HEALED_DAMAGED_HHB1=ConvertAbilityRealLevelField('Hhb1')
var ABILITY_RLF_EXTRA_DAMAGE_HCA1=ConvertAbilityRealLevelField('Hca1')
var ABILITY_RLF_MOVEMENT_SPEED_FACTOR_HCA2=ConvertAbilityRealLevelField('Hca2')
var ABILITY_RLF_ATTACK_SPEED_FACTOR_HCA3=ConvertAbilityRealLevelField('Hca3')
var ABILITY_RLF_MOVEMENT_SPEED_INCREASE_PERCENT_OAE1=ConvertAbilityRealLevelField('Oae1')
var ABILITY_RLF_ATTACK_SPEED_INCREASE_PERCENT_OAE2=ConvertAbilityRealLevelField('Oae2')
var ABILITY_RLF_REINCARNATION_DELAY=ConvertAbilityRealLevelField('Ore1')
var ABILITY_RLF_DAMAGE_OSH1=ConvertAbilityRealLevelField('Osh1')
var ABILITY_RLF_MAXIMUM_DAMAGE_OSH2=ConvertAbilityRealLevelField('Osh2')
var ABILITY_RLF_DISTANCE_OSH3=ConvertAbilityRealLevelField('Osh3')
var ABILITY_RLF_FINAL_AREA_OSH4=ConvertAbilityRealLevelField('Osh4')
var ABILITY_RLF_GRAPHIC_DELAY_NFD1=ConvertAbilityRealLevelField('Nfd1')
var ABILITY_RLF_GRAPHIC_DURATION_NFD2=ConvertAbilityRealLevelField('Nfd2')
var ABILITY_RLF_DAMAGE_NFD3=ConvertAbilityRealLevelField('Nfd3')
var ABILITY_RLF_SUMMONED_UNIT_DAMAGE_AMS1=ConvertAbilityRealLevelField('Ams1')
var ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_AMS2=ConvertAbilityRealLevelField('Ams2')
var ABILITY_RLF_AURA_DURATION=ConvertAbilityRealLevelField('Apl1')
var ABILITY_RLF_DAMAGE_PER_SECOND_APL2=ConvertAbilityRealLevelField('Apl2')
var ABILITY_RLF_DURATION_OF_PLAGUE_WARD=ConvertAbilityRealLevelField('Apl3')
var ABILITY_RLF_AMOUNT_OF_HIT_POINTS_REGENERATED=ConvertAbilityRealLevelField('Oar1')
var ABILITY_RLF_ATTACK_DAMAGE_INCREASE_AKB1=ConvertAbilityRealLevelField('Akb1')
var ABILITY_RLF_MANA_LOSS_ADM1=ConvertAbilityRealLevelField('Adm1')
var ABILITY_RLF_SUMMONED_UNIT_DAMAGE_ADM2=ConvertAbilityRealLevelField('Adm2')
var ABILITY_RLF_EXPANSION_AMOUNT=ConvertAbilityRealLevelField('Bli1')
var ABILITY_RLF_INTERVAL_DURATION_BGM2=ConvertAbilityRealLevelField('Bgm2')
var ABILITY_RLF_RADIUS_OF_MINING_RING=ConvertAbilityRealLevelField('Bgm4')
var ABILITY_RLF_ATTACK_SPEED_INCREASE_PERCENT_BLO1=ConvertAbilityRealLevelField('Blo1')
var ABILITY_RLF_MOVEMENT_SPEED_INCREASE_PERCENT_BLO2=ConvertAbilityRealLevelField('Blo2')
var ABILITY_RLF_SCALING_FACTOR=ConvertAbilityRealLevelField('Blo3')
var ABILITY_RLF_HIT_POINTS_PER_SECOND_CAN1=ConvertAbilityRealLevelField('Can1')
var ABILITY_RLF_MAX_HIT_POINTS=ConvertAbilityRealLevelField('Can2')
var ABILITY_RLF_DAMAGE_PER_SECOND_DEV2=ConvertAbilityRealLevelField('Dev2')
var ABILITY_RLF_MOVEMENT_UPDATE_FREQUENCY_CHD1=ConvertAbilityRealLevelField('Chd1')
var ABILITY_RLF_ATTACK_UPDATE_FREQUENCY_CHD2=ConvertAbilityRealLevelField('Chd2')
var ABILITY_RLF_SUMMONED_UNIT_DAMAGE_CHD3=ConvertAbilityRealLevelField('Chd3')
var ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_PERCENT_CRI1=ConvertAbilityRealLevelField('Cri1')
var ABILITY_RLF_ATTACK_SPEED_REDUCTION_PERCENT_CRI2=ConvertAbilityRealLevelField('Cri2')
var ABILITY_RLF_DAMAGE_REDUCTION_CRI3=ConvertAbilityRealLevelField('Cri3')
var ABILITY_RLF_CHANCE_TO_MISS_CRS=ConvertAbilityRealLevelField('Crs1')
var ABILITY_RLF_FULL_DAMAGE_RADIUS_DDA1=ConvertAbilityRealLevelField('Dda1')
var ABILITY_RLF_FULL_DAMAGE_AMOUNT_DDA2=ConvertAbilityRealLevelField('Dda2')
var ABILITY_RLF_PARTIAL_DAMAGE_RADIUS=ConvertAbilityRealLevelField('Dda3')
var ABILITY_RLF_PARTIAL_DAMAGE_AMOUNT=ConvertAbilityRealLevelField('Dda4')
var ABILITY_RLF_BUILDING_DAMAGE_FACTOR_SDS1=ConvertAbilityRealLevelField('Sds1')
var ABILITY_RLF_MAX_DAMAGE_UCO5=ConvertAbilityRealLevelField('Uco5')
var ABILITY_RLF_MOVE_SPEED_BONUS_UCO6=ConvertAbilityRealLevelField('Uco6')
var ABILITY_RLF_DAMAGE_TAKEN_PERCENT_DEF1=ConvertAbilityRealLevelField('Def1')
var ABILITY_RLF_DAMAGE_DEALT_PERCENT_DEF2=ConvertAbilityRealLevelField('Def2')
var ABILITY_RLF_MOVEMENT_SPEED_FACTOR_DEF3=ConvertAbilityRealLevelField('Def3')
var ABILITY_RLF_ATTACK_SPEED_FACTOR_DEF4=ConvertAbilityRealLevelField('Def4')
var ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_DEF5=ConvertAbilityRealLevelField('Def5')
var ABILITY_RLF_CHANCE_TO_DEFLECT=ConvertAbilityRealLevelField('Def6')
var ABILITY_RLF_DEFLECT_DAMAGE_TAKEN_PIERCING=ConvertAbilityRealLevelField('Def7')
var ABILITY_RLF_DEFLECT_DAMAGE_TAKEN_SPELLS=ConvertAbilityRealLevelField('Def8')
var ABILITY_RLF_RIP_DELAY=ConvertAbilityRealLevelField('Eat1')
var ABILITY_RLF_EAT_DELAY=ConvertAbilityRealLevelField('Eat2')
var ABILITY_RLF_HIT_POINTS_GAINED_EAT3=ConvertAbilityRealLevelField('Eat3')
var ABILITY_RLF_AIR_UNIT_LOWER_DURATION=ConvertAbilityRealLevelField('Ens1')
var ABILITY_RLF_AIR_UNIT_HEIGHT=ConvertAbilityRealLevelField('Ens2')
var ABILITY_RLF_MELEE_ATTACK_RANGE=ConvertAbilityRealLevelField('Ens3')
var ABILITY_RLF_INTERVAL_DURATION_EGM2=ConvertAbilityRealLevelField('Egm2')
var ABILITY_RLF_EFFECT_DELAY_FLA2=ConvertAbilityRealLevelField('Fla2')
var ABILITY_RLF_MINING_DURATION=ConvertAbilityRealLevelField('Gld2')
var ABILITY_RLF_RADIUS_OF_GRAVESTONES=ConvertAbilityRealLevelField('Gyd2')
var ABILITY_RLF_RADIUS_OF_CORPSES=ConvertAbilityRealLevelField('Gyd3')
var ABILITY_RLF_HIT_POINTS_GAINED_HEA1=ConvertAbilityRealLevelField('Hea1')
var ABILITY_RLF_DAMAGE_INCREASE_PERCENT_INF1=ConvertAbilityRealLevelField('Inf1')
var ABILITY_RLF_AUTOCAST_RANGE=ConvertAbilityRealLevelField('Inf3')
var ABILITY_RLF_LIFE_REGEN_RATE=ConvertAbilityRealLevelField('Inf4')
var ABILITY_RLF_GRAPHIC_DELAY_LIT1=ConvertAbilityRealLevelField('Lit1')
var ABILITY_RLF_GRAPHIC_DURATION_LIT2=ConvertAbilityRealLevelField('Lit2')
var ABILITY_RLF_DAMAGE_PER_SECOND_LSH1=ConvertAbilityRealLevelField('Lsh1')
var ABILITY_RLF_MANA_GAINED=ConvertAbilityRealLevelField('Mbt1')
var ABILITY_RLF_HIT_POINTS_GAINED_MBT2=ConvertAbilityRealLevelField('Mbt2')
var ABILITY_RLF_AUTOCAST_REQUIREMENT=ConvertAbilityRealLevelField('Mbt3')
var ABILITY_RLF_WATER_HEIGHT=ConvertAbilityRealLevelField('Mbt4')
var ABILITY_RLF_ACTIVATION_DELAY_MIN1=ConvertAbilityRealLevelField('Min1')
var ABILITY_RLF_INVISIBILITY_TRANSITION_TIME=ConvertAbilityRealLevelField('Min2')
var ABILITY_RLF_ACTIVATION_RADIUS=ConvertAbilityRealLevelField('Neu1')
var ABILITY_RLF_AMOUNT_REGENERATED=ConvertAbilityRealLevelField('Arm1')
var ABILITY_RLF_DAMAGE_PER_SECOND_POI1=ConvertAbilityRealLevelField('Poi1')
var ABILITY_RLF_ATTACK_SPEED_FACTOR_POI2=ConvertAbilityRealLevelField('Poi2')
var ABILITY_RLF_MOVEMENT_SPEED_FACTOR_POI3=ConvertAbilityRealLevelField('Poi3')
var ABILITY_RLF_EXTRA_DAMAGE_POA1=ConvertAbilityRealLevelField('Poa1')
var ABILITY_RLF_DAMAGE_PER_SECOND_POA2=ConvertAbilityRealLevelField('Poa2')
var ABILITY_RLF_ATTACK_SPEED_FACTOR_POA3=ConvertAbilityRealLevelField('Poa3')
var ABILITY_RLF_MOVEMENT_SPEED_FACTOR_POA4=ConvertAbilityRealLevelField('Poa4')
var ABILITY_RLF_DAMAGE_AMPLIFICATION=ConvertAbilityRealLevelField('Pos2')
var ABILITY_RLF_CHANCE_TO_STOMP_PERCENT=ConvertAbilityRealLevelField('War1')
var ABILITY_RLF_DAMAGE_DEALT_WAR2=ConvertAbilityRealLevelField('War2')
var ABILITY_RLF_FULL_DAMAGE_RADIUS_WAR3=ConvertAbilityRealLevelField('War3')
var ABILITY_RLF_HALF_DAMAGE_RADIUS_WAR4=ConvertAbilityRealLevelField('War4')
var ABILITY_RLF_SUMMONED_UNIT_DAMAGE_PRG3=ConvertAbilityRealLevelField('Prg3')
var ABILITY_RLF_UNIT_PAUSE_DURATION=ConvertAbilityRealLevelField('Prg4')
var ABILITY_RLF_HERO_PAUSE_DURATION=ConvertAbilityRealLevelField('Prg5')
var ABILITY_RLF_HIT_POINTS_GAINED_REJ1=ConvertAbilityRealLevelField('Rej1')
var ABILITY_RLF_MANA_POINTS_GAINED_REJ2=ConvertAbilityRealLevelField('Rej2')
var ABILITY_RLF_MINIMUM_LIFE_REQUIRED=ConvertAbilityRealLevelField('Rpb3')
var ABILITY_RLF_MINIMUM_MANA_REQUIRED=ConvertAbilityRealLevelField('Rpb4')
var ABILITY_RLF_REPAIR_COST_RATIO=ConvertAbilityRealLevelField('Rep1')
var ABILITY_RLF_REPAIR_TIME_RATIO=ConvertAbilityRealLevelField('Rep2')
var ABILITY_RLF_POWERBUILD_COST=ConvertAbilityRealLevelField('Rep3')
var ABILITY_RLF_POWERBUILD_RATE=ConvertAbilityRealLevelField('Rep4')
var ABILITY_RLF_NAVAL_RANGE_BONUS=ConvertAbilityRealLevelField('Rep5')
var ABILITY_RLF_DAMAGE_INCREASE_PERCENT_ROA1=ConvertAbilityRealLevelField('Roa1')
var ABILITY_RLF_LIFE_REGENERATION_RATE=ConvertAbilityRealLevelField('Roa3')
var ABILITY_RLF_MANA_REGEN=ConvertAbilityRealLevelField('Roa4')
var ABILITY_RLF_DAMAGE_INCREASE=ConvertAbilityRealLevelField('Nbr1')
var ABILITY_RLF_SALVAGE_COST_RATIO=ConvertAbilityRealLevelField('Sal1')
var ABILITY_RLF_IN_FLIGHT_SIGHT_RADIUS=ConvertAbilityRealLevelField('Esn1')
var ABILITY_RLF_HOVERING_SIGHT_RADIUS=ConvertAbilityRealLevelField('Esn2')
var ABILITY_RLF_HOVERING_HEIGHT=ConvertAbilityRealLevelField('Esn3')
var ABILITY_RLF_DURATION_OF_OWLS=ConvertAbilityRealLevelField('Esn5')
var ABILITY_RLF_FADE_DURATION=ConvertAbilityRealLevelField('Shm1')
var ABILITY_RLF_DAY_NIGHT_DURATION=ConvertAbilityRealLevelField('Shm2')
var ABILITY_RLF_ACTION_DURATION=ConvertAbilityRealLevelField('Shm3')
var ABILITY_RLF_MOVEMENT_SPEED_FACTOR_SLO1=ConvertAbilityRealLevelField('Slo1')
var ABILITY_RLF_ATTACK_SPEED_FACTOR_SLO2=ConvertAbilityRealLevelField('Slo2')
var ABILITY_RLF_DAMAGE_PER_SECOND_SPO1=ConvertAbilityRealLevelField('Spo1')
var ABILITY_RLF_MOVEMENT_SPEED_FACTOR_SPO2=ConvertAbilityRealLevelField('Spo2')
var ABILITY_RLF_ATTACK_SPEED_FACTOR_SPO3=ConvertAbilityRealLevelField('Spo3')
var ABILITY_RLF_ACTIVATION_DELAY_STA1=ConvertAbilityRealLevelField('Sta1')
var ABILITY_RLF_DETECTION_RADIUS_STA2=ConvertAbilityRealLevelField('Sta2')
var ABILITY_RLF_DETONATION_RADIUS=ConvertAbilityRealLevelField('Sta3')
var ABILITY_RLF_STUN_DURATION_STA4=ConvertAbilityRealLevelField('Sta4')
var ABILITY_RLF_ATTACK_SPEED_BONUS_PERCENT=ConvertAbilityRealLevelField('Uhf1')
var ABILITY_RLF_DAMAGE_PER_SECOND_UHF2=ConvertAbilityRealLevelField('Uhf2')
var ABILITY_RLF_LUMBER_PER_INTERVAL=ConvertAbilityRealLevelField('Wha1')
var ABILITY_RLF_ART_ATTACHMENT_HEIGHT=ConvertAbilityRealLevelField('Wha3')
var ABILITY_RLF_TELEPORT_AREA_WIDTH=ConvertAbilityRealLevelField('Wrp1')
var ABILITY_RLF_TELEPORT_AREA_HEIGHT=ConvertAbilityRealLevelField('Wrp2')
var ABILITY_RLF_LIFE_STOLEN_PER_ATTACK=ConvertAbilityRealLevelField('Ivam')
var ABILITY_RLF_DAMAGE_BONUS_IDAM=ConvertAbilityRealLevelField('Idam')
var ABILITY_RLF_CHANCE_TO_HIT_UNITS_PERCENT=ConvertAbilityRealLevelField('Iob2')
var ABILITY_RLF_CHANCE_TO_HIT_HEROS_PERCENT=ConvertAbilityRealLevelField('Iob3')
var ABILITY_RLF_CHANCE_TO_HIT_SUMMONS_PERCENT=ConvertAbilityRealLevelField('Iob4')
var ABILITY_RLF_DELAY_FOR_TARGET_EFFECT=ConvertAbilityRealLevelField('Idel')
var ABILITY_RLF_DAMAGE_DEALT_PERCENT_OF_NORMAL=ConvertAbilityRealLevelField('Iild')
var ABILITY_RLF_DAMAGE_RECEIVED_MULTIPLIER=ConvertAbilityRealLevelField('Iilw')
var ABILITY_RLF_MANA_REGENERATION_BONUS_AS_FRACTION_OF_NORMAL=ConvertAbilityRealLevelField('Imrp')
var ABILITY_RLF_MOVEMENT_SPEED_INCREASE_ISPI=ConvertAbilityRealLevelField('Ispi')
var ABILITY_RLF_DAMAGE_PER_SECOND_IDPS=ConvertAbilityRealLevelField('Idps')
var ABILITY_RLF_ATTACK_DAMAGE_INCREASE_CAC1=ConvertAbilityRealLevelField('Cac1')
var ABILITY_RLF_DAMAGE_PER_SECOND_COR1=ConvertAbilityRealLevelField('Cor1')
var ABILITY_RLF_ATTACK_SPEED_INCREASE_ISX1=ConvertAbilityRealLevelField('Isx1')
var ABILITY_RLF_DAMAGE_WRS1=ConvertAbilityRealLevelField('Wrs1')
var ABILITY_RLF_TERRAIN_DEFORMATION_AMPLITUDE=ConvertAbilityRealLevelField('Wrs2')
var ABILITY_RLF_DAMAGE_CTC1=ConvertAbilityRealLevelField('Ctc1')
var ABILITY_RLF_EXTRA_DAMAGE_TO_TARGET=ConvertAbilityRealLevelField('Ctc2')
var ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_CTC3=ConvertAbilityRealLevelField('Ctc3')
var ABILITY_RLF_ATTACK_SPEED_REDUCTION_CTC4=ConvertAbilityRealLevelField('Ctc4')
var ABILITY_RLF_DAMAGE_CTB1=ConvertAbilityRealLevelField('Ctb1')
var ABILITY_RLF_CASTING_DELAY_SECONDS=ConvertAbilityRealLevelField('Uds2')
var ABILITY_RLF_MANA_LOSS_PER_UNIT_DTN1=ConvertAbilityRealLevelField('Dtn1')
var ABILITY_RLF_DAMAGE_TO_SUMMONED_UNITS_DTN2=ConvertAbilityRealLevelField('Dtn2')
var ABILITY_RLF_TRANSITION_TIME_SECONDS=ConvertAbilityRealLevelField('Ivs1')
var ABILITY_RLF_MANA_DRAINED_PER_SECOND_NMR1=ConvertAbilityRealLevelField('Nmr1')
var ABILITY_RLF_CHANCE_TO_REDUCE_DAMAGE_PERCENT=ConvertAbilityRealLevelField('Ssk1')
var ABILITY_RLF_MINIMUM_DAMAGE=ConvertAbilityRealLevelField('Ssk2')
var ABILITY_RLF_IGNORED_DAMAGE=ConvertAbilityRealLevelField('Ssk3')
var ABILITY_RLF_FULL_DAMAGE_DEALT=ConvertAbilityRealLevelField('Hfs1')
var ABILITY_RLF_FULL_DAMAGE_INTERVAL=ConvertAbilityRealLevelField('Hfs2')
var ABILITY_RLF_HALF_DAMAGE_DEALT=ConvertAbilityRealLevelField('Hfs3')
var ABILITY_RLF_HALF_DAMAGE_INTERVAL=ConvertAbilityRealLevelField('Hfs4')
var ABILITY_RLF_BUILDING_REDUCTION_HFS5=ConvertAbilityRealLevelField('Hfs5')
var ABILITY_RLF_MAXIMUM_DAMAGE_HFS6=ConvertAbilityRealLevelField('Hfs6')
var ABILITY_RLF_MANA_PER_HIT_POINT=ConvertAbilityRealLevelField('Nms1')
var ABILITY_RLF_DAMAGE_ABSORBED_PERCENT=ConvertAbilityRealLevelField('Nms2')
var ABILITY_RLF_WAVE_DISTANCE=ConvertAbilityRealLevelField('Uim1')
var ABILITY_RLF_WAVE_TIME_SECONDS=ConvertAbilityRealLevelField('Uim2')
var ABILITY_RLF_DAMAGE_DEALT_UIM3=ConvertAbilityRealLevelField('Uim3')
var ABILITY_RLF_AIR_TIME_SECONDS_UIM4=ConvertAbilityRealLevelField('Uim4')
var ABILITY_RLF_UNIT_RELEASE_INTERVAL_SECONDS=ConvertAbilityRealLevelField('Uls2')
var ABILITY_RLF_DAMAGE_RETURN_FACTOR=ConvertAbilityRealLevelField('Uls4')
var ABILITY_RLF_DAMAGE_RETURN_THRESHOLD=ConvertAbilityRealLevelField('Uls5')
var ABILITY_RLF_RETURNED_DAMAGE_FACTOR=ConvertAbilityRealLevelField('Uts1')
var ABILITY_RLF_RECEIVED_DAMAGE_FACTOR=ConvertAbilityRealLevelField('Uts2')
var ABILITY_RLF_DEFENSE_BONUS_UTS3=ConvertAbilityRealLevelField('Uts3')
var ABILITY_RLF_DAMAGE_BONUS_NBA1=ConvertAbilityRealLevelField('Nba1')
var ABILITY_RLF_SUMMONED_UNIT_DURATION_SECONDS_NBA3=ConvertAbilityRealLevelField('Nba3')
var ABILITY_RLF_MANA_PER_SUMMONED_HITPOINT=ConvertAbilityRealLevelField('Cmg2')
var ABILITY_RLF_CHARGE_FOR_CURRENT_LIFE=ConvertAbilityRealLevelField('Cmg3')
var ABILITY_RLF_HIT_POINTS_DRAINED=ConvertAbilityRealLevelField('Ndr1')
var ABILITY_RLF_MANA_POINTS_DRAINED=ConvertAbilityRealLevelField('Ndr2')
var ABILITY_RLF_DRAIN_INTERVAL_SECONDS=ConvertAbilityRealLevelField('Ndr3')
var ABILITY_RLF_LIFE_TRANSFERRED_PER_SECOND=ConvertAbilityRealLevelField('Ndr4')
var ABILITY_RLF_MANA_TRANSFERRED_PER_SECOND=ConvertAbilityRealLevelField('Ndr5')
var ABILITY_RLF_BONUS_LIFE_FACTOR=ConvertAbilityRealLevelField('Ndr6')
var ABILITY_RLF_BONUS_LIFE_DECAY=ConvertAbilityRealLevelField('Ndr7')
var ABILITY_RLF_BONUS_MANA_FACTOR=ConvertAbilityRealLevelField('Ndr8')
var ABILITY_RLF_BONUS_MANA_DECAY=ConvertAbilityRealLevelField('Ndr9')
var ABILITY_RLF_CHANCE_TO_MISS_PERCENT=ConvertAbilityRealLevelField('Nsi2')
var ABILITY_RLF_MOVEMENT_SPEED_MODIFIER=ConvertAbilityRealLevelField('Nsi3')
var ABILITY_RLF_ATTACK_SPEED_MODIFIER=ConvertAbilityRealLevelField('Nsi4')
var ABILITY_RLF_DAMAGE_PER_SECOND_TDG1=ConvertAbilityRealLevelField('Tdg1')
var ABILITY_RLF_MEDIUM_DAMAGE_RADIUS_TDG2=ConvertAbilityRealLevelField('Tdg2')
var ABILITY_RLF_MEDIUM_DAMAGE_PER_SECOND=ConvertAbilityRealLevelField('Tdg3')
var ABILITY_RLF_SMALL_DAMAGE_RADIUS_TDG4=ConvertAbilityRealLevelField('Tdg4')
var ABILITY_RLF_SMALL_DAMAGE_PER_SECOND=ConvertAbilityRealLevelField('Tdg5')
var ABILITY_RLF_AIR_TIME_SECONDS_TSP1=ConvertAbilityRealLevelField('Tsp1')
var ABILITY_RLF_MINIMUM_HIT_INTERVAL_SECONDS=ConvertAbilityRealLevelField('Tsp2')
var ABILITY_RLF_DAMAGE_PER_SECOND_NBF5=ConvertAbilityRealLevelField('Nbf5')
var ABILITY_RLF_MAXIMUM_RANGE=ConvertAbilityRealLevelField('Ebl1')
var ABILITY_RLF_MINIMUM_RANGE=ConvertAbilityRealLevelField('Ebl2')
var ABILITY_RLF_DAMAGE_PER_TARGET_EFK1=ConvertAbilityRealLevelField('Efk1')
var ABILITY_RLF_MAXIMUM_TOTAL_DAMAGE=ConvertAbilityRealLevelField('Efk2')
var ABILITY_RLF_MAXIMUM_SPEED_ADJUSTMENT=ConvertAbilityRealLevelField('Efk4')
var ABILITY_RLF_DECAYING_DAMAGE=ConvertAbilityRealLevelField('Esh1')
var ABILITY_RLF_MOVEMENT_SPEED_FACTOR_ESH2=ConvertAbilityRealLevelField('Esh2')
var ABILITY_RLF_ATTACK_SPEED_FACTOR_ESH3=ConvertAbilityRealLevelField('Esh3')
var ABILITY_RLF_DECAY_POWER=ConvertAbilityRealLevelField('Esh4')
var ABILITY_RLF_INITIAL_DAMAGE_ESH5=ConvertAbilityRealLevelField('Esh5')
var ABILITY_RLF_MAXIMUM_LIFE_ABSORBED=ConvertAbilityRealLevelField('abs1')
var ABILITY_RLF_MAXIMUM_MANA_ABSORBED=ConvertAbilityRealLevelField('abs2')
var ABILITY_RLF_MOVEMENT_SPEED_INCREASE_BSK1=ConvertAbilityRealLevelField('bsk1')
var ABILITY_RLF_ATTACK_SPEED_INCREASE_BSK2=ConvertAbilityRealLevelField('bsk2')
var ABILITY_RLF_DAMAGE_TAKEN_INCREASE=ConvertAbilityRealLevelField('bsk3')
var ABILITY_RLF_LIFE_PER_UNIT=ConvertAbilityRealLevelField('dvm1')
var ABILITY_RLF_MANA_PER_UNIT=ConvertAbilityRealLevelField('dvm2')
var ABILITY_RLF_LIFE_PER_BUFF=ConvertAbilityRealLevelField('dvm3')
var ABILITY_RLF_MANA_PER_BUFF=ConvertAbilityRealLevelField('dvm4')
var ABILITY_RLF_SUMMONED_UNIT_DAMAGE_DVM5=ConvertAbilityRealLevelField('dvm5')
var ABILITY_RLF_DAMAGE_BONUS_FAK1=ConvertAbilityRealLevelField('fak1')
var ABILITY_RLF_MEDIUM_DAMAGE_FACTOR_FAK2=ConvertAbilityRealLevelField('fak2')
var ABILITY_RLF_SMALL_DAMAGE_FACTOR_FAK3=ConvertAbilityRealLevelField('fak3')
var ABILITY_RLF_FULL_DAMAGE_RADIUS_FAK4=ConvertAbilityRealLevelField('fak4')
var ABILITY_RLF_HALF_DAMAGE_RADIUS_FAK5=ConvertAbilityRealLevelField('fak5')
var ABILITY_RLF_EXTRA_DAMAGE_PER_SECOND=ConvertAbilityRealLevelField('liq1')
var ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_LIQ2=ConvertAbilityRealLevelField('liq2')
var ABILITY_RLF_ATTACK_SPEED_REDUCTION_LIQ3=ConvertAbilityRealLevelField('liq3')
var ABILITY_RLF_MAGIC_DAMAGE_FACTOR=ConvertAbilityRealLevelField('mim1')
var ABILITY_RLF_UNIT_DAMAGE_PER_MANA_POINT=ConvertAbilityRealLevelField('mfl1')
var ABILITY_RLF_HERO_DAMAGE_PER_MANA_POINT=ConvertAbilityRealLevelField('mfl2')
var ABILITY_RLF_UNIT_MAXIMUM_DAMAGE=ConvertAbilityRealLevelField('mfl3')
var ABILITY_RLF_HERO_MAXIMUM_DAMAGE=ConvertAbilityRealLevelField('mfl4')
var ABILITY_RLF_DAMAGE_COOLDOWN=ConvertAbilityRealLevelField('mfl5')
var ABILITY_RLF_DISTRIBUTED_DAMAGE_FACTOR_SPL1=ConvertAbilityRealLevelField('spl1')
var ABILITY_RLF_LIFE_REGENERATED=ConvertAbilityRealLevelField('irl1')
var ABILITY_RLF_MANA_REGENERATED=ConvertAbilityRealLevelField('irl2')
var ABILITY_RLF_MANA_LOSS_PER_UNIT_IDC1=ConvertAbilityRealLevelField('idc1')
var ABILITY_RLF_SUMMONED_UNIT_DAMAGE_IDC2=ConvertAbilityRealLevelField('idc2')
var ABILITY_RLF_ACTIVATION_DELAY_IMO2=ConvertAbilityRealLevelField('imo2')
var ABILITY_RLF_LURE_INTERVAL_SECONDS=ConvertAbilityRealLevelField('imo3')
var ABILITY_RLF_DAMAGE_BONUS_ISR1=ConvertAbilityRealLevelField('isr1')
var ABILITY_RLF_DAMAGE_REDUCTION_ISR2=ConvertAbilityRealLevelField('isr2')
var ABILITY_RLF_DAMAGE_BONUS_IPV1=ConvertAbilityRealLevelField('ipv1')
var ABILITY_RLF_LIFE_STEAL_AMOUNT=ConvertAbilityRealLevelField('ipv2')
var ABILITY_RLF_LIFE_RESTORED_FACTOR=ConvertAbilityRealLevelField('ast1')
var ABILITY_RLF_MANA_RESTORED_FACTOR=ConvertAbilityRealLevelField('ast2')
var ABILITY_RLF_ATTACH_DELAY=ConvertAbilityRealLevelField('gra1')
var ABILITY_RLF_REMOVE_DELAY=ConvertAbilityRealLevelField('gra2')
var ABILITY_RLF_HERO_REGENERATION_DELAY=ConvertAbilityRealLevelField('Nsa2')
var ABILITY_RLF_UNIT_REGENERATION_DELAY=ConvertAbilityRealLevelField('Nsa3')
var ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_NSA4=ConvertAbilityRealLevelField('Nsa4')
var ABILITY_RLF_HIT_POINTS_PER_SECOND_NSA5=ConvertAbilityRealLevelField('Nsa5')
var ABILITY_RLF_DAMAGE_TO_SUMMONED_UNITS_IXS1=ConvertAbilityRealLevelField('Ixs1')
var ABILITY_RLF_MAGIC_DAMAGE_REDUCTION_IXS2=ConvertAbilityRealLevelField('Ixs2')
var ABILITY_RLF_SUMMONED_UNIT_DURATION=ConvertAbilityRealLevelField('Npa6')
var ABILITY_RLF_SHIELD_COOLDOWN_TIME=ConvertAbilityRealLevelField('Nse1')
var ABILITY_RLF_DAMAGE_PER_SECOND_NDO1=ConvertAbilityRealLevelField('Ndo1')
var ABILITY_RLF_SUMMONED_UNIT_DURATION_SECONDS_NDO3=ConvertAbilityRealLevelField('Ndo3')
var ABILITY_RLF_MEDIUM_DAMAGE_RADIUS_FLK1=ConvertAbilityRealLevelField('flk1')
var ABILITY_RLF_SMALL_DAMAGE_RADIUS_FLK2=ConvertAbilityRealLevelField('flk2')
var ABILITY_RLF_FULL_DAMAGE_AMOUNT_FLK3=ConvertAbilityRealLevelField('flk3')
var ABILITY_RLF_MEDIUM_DAMAGE_AMOUNT=ConvertAbilityRealLevelField('flk4')
var ABILITY_RLF_SMALL_DAMAGE_AMOUNT=ConvertAbilityRealLevelField('flk5')
var ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_PERCENT_HBN1=ConvertAbilityRealLevelField('Hbn1')
var ABILITY_RLF_ATTACK_SPEED_REDUCTION_PERCENT_HBN2=ConvertAbilityRealLevelField('Hbn2')
var ABILITY_RLF_MAX_MANA_DRAINED_UNITS=ConvertAbilityRealLevelField('fbk1')
var ABILITY_RLF_DAMAGE_RATIO_UNITS_PERCENT=ConvertAbilityRealLevelField('fbk2')
var ABILITY_RLF_MAX_MANA_DRAINED_HEROS=ConvertAbilityRealLevelField('fbk3')
var ABILITY_RLF_DAMAGE_RATIO_HEROS_PERCENT=ConvertAbilityRealLevelField('fbk4')
var ABILITY_RLF_SUMMONED_DAMAGE=ConvertAbilityRealLevelField('fbk5')
var ABILITY_RLF_DISTRIBUTED_DAMAGE_FACTOR_NCA1=ConvertAbilityRealLevelField('nca1')
var ABILITY_RLF_INITIAL_DAMAGE_PXF1=ConvertAbilityRealLevelField('pxf1')
var ABILITY_RLF_DAMAGE_PER_SECOND_PXF2=ConvertAbilityRealLevelField('pxf2')
var ABILITY_RLF_DAMAGE_PER_SECOND_MLS1=ConvertAbilityRealLevelField('mls1')
var ABILITY_RLF_BEAST_COLLISION_RADIUS=ConvertAbilityRealLevelField('Nst2')
var ABILITY_RLF_DAMAGE_AMOUNT_NST3=ConvertAbilityRealLevelField('Nst3')
var ABILITY_RLF_DAMAGE_RADIUS=ConvertAbilityRealLevelField('Nst4')
var ABILITY_RLF_DAMAGE_DELAY=ConvertAbilityRealLevelField('Nst5')
var ABILITY_RLF_FOLLOW_THROUGH_TIME=ConvertAbilityRealLevelField('Ncl1')
var ABILITY_RLF_ART_DURATION=ConvertAbilityRealLevelField('Ncl4')
var ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_PERCENT_NAB1=ConvertAbilityRealLevelField('Nab1')
var ABILITY_RLF_ATTACK_SPEED_REDUCTION_PERCENT_NAB2=ConvertAbilityRealLevelField('Nab2')
var ABILITY_RLF_PRIMARY_DAMAGE=ConvertAbilityRealLevelField('Nab4')
var ABILITY_RLF_SECONDARY_DAMAGE=ConvertAbilityRealLevelField('Nab5')
var ABILITY_RLF_DAMAGE_INTERVAL_NAB6=ConvertAbilityRealLevelField('Nab6')
var ABILITY_RLF_GOLD_COST_FACTOR=ConvertAbilityRealLevelField('Ntm1')
var ABILITY_RLF_LUMBER_COST_FACTOR=ConvertAbilityRealLevelField('Ntm2')
var ABILITY_RLF_MOVE_SPEED_BONUS_NEG1=ConvertAbilityRealLevelField('Neg1')
var ABILITY_RLF_DAMAGE_BONUS_NEG2=ConvertAbilityRealLevelField('Neg2')
var ABILITY_RLF_DAMAGE_AMOUNT_NCS1=ConvertAbilityRealLevelField('Ncs1')
var ABILITY_RLF_DAMAGE_INTERVAL_NCS2=ConvertAbilityRealLevelField('Ncs2')
var ABILITY_RLF_MAX_DAMAGE_NCS4=ConvertAbilityRealLevelField('Ncs4')
var ABILITY_RLF_BUILDING_DAMAGE_FACTOR_NCS5=ConvertAbilityRealLevelField('Ncs5')
var ABILITY_RLF_EFFECT_DURATION=ConvertAbilityRealLevelField('Ncs6')
var ABILITY_RLF_SPAWN_INTERVAL_NSY1=ConvertAbilityRealLevelField('Nsy1')
var ABILITY_RLF_SPAWN_UNIT_DURATION=ConvertAbilityRealLevelField('Nsy3')
var ABILITY_RLF_SPAWN_UNIT_OFFSET=ConvertAbilityRealLevelField('Nsy4')
var ABILITY_RLF_LEASH_RANGE_NSY5=ConvertAbilityRealLevelField('Nsy5')
var ABILITY_RLF_SPAWN_INTERVAL_NFY1=ConvertAbilityRealLevelField('Nfy1')
var ABILITY_RLF_LEASH_RANGE_NFY2=ConvertAbilityRealLevelField('Nfy2')
var ABILITY_RLF_CHANCE_TO_DEMOLISH=ConvertAbilityRealLevelField('Nde1')
var ABILITY_RLF_DAMAGE_MULTIPLIER_BUILDINGS=ConvertAbilityRealLevelField('Nde2')
var ABILITY_RLF_DAMAGE_MULTIPLIER_UNITS=ConvertAbilityRealLevelField('Nde3')
var ABILITY_RLF_DAMAGE_MULTIPLIER_HEROES=ConvertAbilityRealLevelField('Nde4')
var ABILITY_RLF_BONUS_DAMAGE_MULTIPLIER=ConvertAbilityRealLevelField('Nic1')
var ABILITY_RLF_DEATH_DAMAGE_FULL_AMOUNT=ConvertAbilityRealLevelField('Nic2')
var ABILITY_RLF_DEATH_DAMAGE_FULL_AREA=ConvertAbilityRealLevelField('Nic3')
var ABILITY_RLF_DEATH_DAMAGE_HALF_AMOUNT=ConvertAbilityRealLevelField('Nic4')
var ABILITY_RLF_DEATH_DAMAGE_HALF_AREA=ConvertAbilityRealLevelField('Nic5')
var ABILITY_RLF_DEATH_DAMAGE_DELAY=ConvertAbilityRealLevelField('Nic6')
var ABILITY_RLF_DAMAGE_AMOUNT_NSO1=ConvertAbilityRealLevelField('Nso1')
var ABILITY_RLF_DAMAGE_PERIOD=ConvertAbilityRealLevelField('Nso2')
var ABILITY_RLF_DAMAGE_PENALTY=ConvertAbilityRealLevelField('Nso3')
var ABILITY_RLF_MOVEMENT_SPEED_REDUCTION_PERCENT_NSO4=ConvertAbilityRealLevelField('Nso4')
var ABILITY_RLF_ATTACK_SPEED_REDUCTION_PERCENT_NSO5=ConvertAbilityRealLevelField('Nso5')
var ABILITY_RLF_SPLIT_DELAY=ConvertAbilityRealLevelField('Nlm2')
var ABILITY_RLF_MAX_HITPOINT_FACTOR=ConvertAbilityRealLevelField('Nlm4')
var ABILITY_RLF_LIFE_DURATION_SPLIT_BONUS=ConvertAbilityRealLevelField('Nlm5')
var ABILITY_RLF_WAVE_INTERVAL=ConvertAbilityRealLevelField('Nvc3')
var ABILITY_RLF_BUILDING_DAMAGE_FACTOR_NVC4=ConvertAbilityRealLevelField('Nvc4')
var ABILITY_RLF_FULL_DAMAGE_AMOUNT_NVC5=ConvertAbilityRealLevelField('Nvc5')
var ABILITY_RLF_HALF_DAMAGE_FACTOR=ConvertAbilityRealLevelField('Nvc6')
var ABILITY_RLF_INTERVAL_BETWEEN_PULSES=ConvertAbilityRealLevelField('Tau5')
var ABILITY_BLF_PERCENT_BONUS_HAB2=ConvertAbilityBooleanLevelField('Hab2')
var ABILITY_BLF_USE_TELEPORT_CLUSTERING_HMT3=ConvertAbilityBooleanLevelField('Hmt3')
var ABILITY_BLF_NEVER_MISS_OCR5=ConvertAbilityBooleanLevelField('Ocr5')
var ABILITY_BLF_EXCLUDE_ITEM_DAMAGE=ConvertAbilityBooleanLevelField('Ocr6')
var ABILITY_BLF_BACKSTAB_DAMAGE=ConvertAbilityBooleanLevelField('Owk4')
var ABILITY_BLF_INHERIT_UPGRADES_UAN3=ConvertAbilityBooleanLevelField('Uan3')
var ABILITY_BLF_MANA_CONVERSION_AS_PERCENT=ConvertAbilityBooleanLevelField('Udp3')
var ABILITY_BLF_LIFE_CONVERSION_AS_PERCENT=ConvertAbilityBooleanLevelField('Udp4')
var ABILITY_BLF_LEAVE_TARGET_ALIVE=ConvertAbilityBooleanLevelField('Udp5')
var ABILITY_BLF_PERCENT_BONUS_UAU3=ConvertAbilityBooleanLevelField('Uau3')
var ABILITY_BLF_DAMAGE_IS_PERCENT_RECEIVED=ConvertAbilityBooleanLevelField('Eah2')
var ABILITY_BLF_MELEE_BONUS=ConvertAbilityBooleanLevelField('Ear2')
var ABILITY_BLF_RANGED_BONUS=ConvertAbilityBooleanLevelField('Ear3')
var ABILITY_BLF_FLAT_BONUS=ConvertAbilityBooleanLevelField('Ear4')
var ABILITY_BLF_NEVER_MISS_HBH5=ConvertAbilityBooleanLevelField('Hbh5')
var ABILITY_BLF_PERCENT_BONUS_HAD2=ConvertAbilityBooleanLevelField('Had2')
var ABILITY_BLF_CAN_DEACTIVATE=ConvertAbilityBooleanLevelField('Hds1')
var ABILITY_BLF_RAISED_UNITS_ARE_INVULNERABLE=ConvertAbilityBooleanLevelField('Hre2')
var ABILITY_BLF_PERCENTAGE_OAR2=ConvertAbilityBooleanLevelField('Oar2')
var ABILITY_BLF_SUMMON_BUSY_UNITS=ConvertAbilityBooleanLevelField('Btl2')
var ABILITY_BLF_CREATES_BLIGHT=ConvertAbilityBooleanLevelField('Bli2')
var ABILITY_BLF_EXPLODES_ON_DEATH=ConvertAbilityBooleanLevelField('Sds6')
var ABILITY_BLF_ALWAYS_AUTOCAST_FAE2=ConvertAbilityBooleanLevelField('Fae2')
var ABILITY_BLF_REGENERATE_ONLY_AT_NIGHT=ConvertAbilityBooleanLevelField('Mbt5')
var ABILITY_BLF_SHOW_SELECT_UNIT_BUTTON=ConvertAbilityBooleanLevelField('Neu3')
var ABILITY_BLF_SHOW_UNIT_INDICATOR=ConvertAbilityBooleanLevelField('Neu4')
var ABILITY_BLF_CHARGE_OWNING_PLAYER=ConvertAbilityBooleanLevelField('Ans6')
var ABILITY_BLF_PERCENTAGE_ARM2=ConvertAbilityBooleanLevelField('Arm2')
var ABILITY_BLF_TARGET_IS_INVULNERABLE=ConvertAbilityBooleanLevelField('Pos3')
var ABILITY_BLF_TARGET_IS_MAGIC_IMMUNE=ConvertAbilityBooleanLevelField('Pos4')
var ABILITY_BLF_KILL_ON_CASTER_DEATH=ConvertAbilityBooleanLevelField('Ucb6')
var ABILITY_BLF_NO_TARGET_REQUIRED_REJ4=ConvertAbilityBooleanLevelField('Rej4')
var ABILITY_BLF_ACCEPTS_GOLD=ConvertAbilityBooleanLevelField('Rtn1')
var ABILITY_BLF_ACCEPTS_LUMBER=ConvertAbilityBooleanLevelField('Rtn2')
var ABILITY_BLF_PREFER_HOSTILES_ROA5=ConvertAbilityBooleanLevelField('Roa5')
var ABILITY_BLF_PREFER_FRIENDLIES_ROA6=ConvertAbilityBooleanLevelField('Roa6')
var ABILITY_BLF_ROOTED_TURNING=ConvertAbilityBooleanLevelField('Roo3')
var ABILITY_BLF_ALWAYS_AUTOCAST_SLO3=ConvertAbilityBooleanLevelField('Slo3')
var ABILITY_BLF_HIDE_BUTTON=ConvertAbilityBooleanLevelField('Ihid')
var ABILITY_BLF_USE_TELEPORT_CLUSTERING_ITP2=ConvertAbilityBooleanLevelField('Itp2')
var ABILITY_BLF_IMMUNE_TO_MORPH_EFFECTS=ConvertAbilityBooleanLevelField('Eth1')
var ABILITY_BLF_DOES_NOT_BLOCK_BUILDINGS=ConvertAbilityBooleanLevelField('Eth2')
var ABILITY_BLF_AUTO_ACQUIRE_ATTACK_TARGETS=ConvertAbilityBooleanLevelField('Gho1')
var ABILITY_BLF_IMMUNE_TO_MORPH_EFFECTS_GHO2=ConvertAbilityBooleanLevelField('Gho2')
var ABILITY_BLF_DO_NOT_BLOCK_BUILDINGS=ConvertAbilityBooleanLevelField('Gho3')
var ABILITY_BLF_INCLUDE_RANGED_DAMAGE=ConvertAbilityBooleanLevelField('Ssk4')
var ABILITY_BLF_INCLUDE_MELEE_DAMAGE=ConvertAbilityBooleanLevelField('Ssk5')
var ABILITY_BLF_MOVE_TO_PARTNER=ConvertAbilityBooleanLevelField('coa2')
var ABILITY_BLF_CAN_BE_DISPELLED=ConvertAbilityBooleanLevelField('cyc1')
var ABILITY_BLF_IGNORE_FRIENDLY_BUFFS=ConvertAbilityBooleanLevelField('dvm6')
var ABILITY_BLF_DROP_ITEMS_ON_DEATH=ConvertAbilityBooleanLevelField('inv2')
var ABILITY_BLF_CAN_USE_ITEMS=ConvertAbilityBooleanLevelField('inv3')
var ABILITY_BLF_CAN_GET_ITEMS=ConvertAbilityBooleanLevelField('inv4')
var ABILITY_BLF_CAN_DROP_ITEMS=ConvertAbilityBooleanLevelField('inv5')
var ABILITY_BLF_REPAIRS_ALLOWED=ConvertAbilityBooleanLevelField('liq4')
var ABILITY_BLF_CASTER_ONLY_SPLASH=ConvertAbilityBooleanLevelField('mfl6')
var ABILITY_BLF_NO_TARGET_REQUIRED_IRL4=ConvertAbilityBooleanLevelField('irl4')
var ABILITY_BLF_DISPEL_ON_ATTACK=ConvertAbilityBooleanLevelField('irl5')
var ABILITY_BLF_AMOUNT_IS_RAW_VALUE=ConvertAbilityBooleanLevelField('ipv3')
var ABILITY_BLF_SHARED_SPELL_COOLDOWN=ConvertAbilityBooleanLevelField('spb2')
var ABILITY_BLF_SLEEP_ONCE=ConvertAbilityBooleanLevelField('sla1')
var ABILITY_BLF_ALLOW_ON_ANY_PLAYER_SLOT=ConvertAbilityBooleanLevelField('sla2')
var ABILITY_BLF_DISABLE_OTHER_ABILITIES=ConvertAbilityBooleanLevelField('Ncl5')
var ABILITY_BLF_ALLOW_BOUNTY=ConvertAbilityBooleanLevelField('Ntm4')
var ABILITY_SLF_ICON_NORMAL=ConvertAbilityStringLevelField('aart')
var ABILITY_SLF_CASTER=ConvertAbilityStringLevelField('acat')
var ABILITY_SLF_TARGET=ConvertAbilityStringLevelField('atat')
var ABILITY_SLF_SPECIAL=ConvertAbilityStringLevelField('asat')
var ABILITY_SLF_EFFECT=ConvertAbilityStringLevelField('aeat')
var ABILITY_SLF_AREA_EFFECT=ConvertAbilityStringLevelField('aaea')
var ABILITY_SLF_LIGHTNING_EFFECTS=ConvertAbilityStringLevelField('alig')
var ABILITY_SLF_MISSILE_ART=ConvertAbilityStringLevelField('amat')
var ABILITY_SLF_TOOLTIP_LEARN=ConvertAbilityStringLevelField('aret')
var ABILITY_SLF_TOOLTIP_LEARN_EXTENDED=ConvertAbilityStringLevelField('arut')
var ABILITY_SLF_TOOLTIP_NORMAL=ConvertAbilityStringLevelField('atp1')
var ABILITY_SLF_TOOLTIP_TURN_OFF=ConvertAbilityStringLevelField('aut1')
var ABILITY_SLF_TOOLTIP_NORMAL_EXTENDED=ConvertAbilityStringLevelField('aub1')
var ABILITY_SLF_TOOLTIP_TURN_OFF_EXTENDED=ConvertAbilityStringLevelField('auu1')
var ABILITY_SLF_NORMAL_FORM_UNIT_EME1=ConvertAbilityStringLevelField('Eme1')
var ABILITY_SLF_SPAWNED_UNITS=ConvertAbilityStringLevelField('Ndp1')
var ABILITY_SLF_ABILITY_FOR_UNIT_CREATION=ConvertAbilityStringLevelField('Nrc1')
var ABILITY_SLF_NORMAL_FORM_UNIT_MIL1=ConvertAbilityStringLevelField('Mil1')
var ABILITY_SLF_ALTERNATE_FORM_UNIT_MIL2=ConvertAbilityStringLevelField('Mil2')
var ABILITY_SLF_BASE_ORDER_ID_ANS5=ConvertAbilityStringLevelField('Ans5')
var ABILITY_SLF_MORPH_UNITS_GROUND=ConvertAbilityStringLevelField('Ply2')
var ABILITY_SLF_MORPH_UNITS_AIR=ConvertAbilityStringLevelField('Ply3')
var ABILITY_SLF_MORPH_UNITS_AMPHIBIOUS=ConvertAbilityStringLevelField('Ply4')
var ABILITY_SLF_MORPH_UNITS_WATER=ConvertAbilityStringLevelField('Ply5')
var ABILITY_SLF_UNIT_TYPE_ONE=ConvertAbilityStringLevelField('Rai3')
var ABILITY_SLF_UNIT_TYPE_TWO=ConvertAbilityStringLevelField('Rai4')
var ABILITY_SLF_UNIT_TYPE_SOD2=ConvertAbilityStringLevelField('Sod2')
var ABILITY_SLF_SUMMON_1_UNIT_TYPE=ConvertAbilityStringLevelField('Ist1')
var ABILITY_SLF_SUMMON_2_UNIT_TYPE=ConvertAbilityStringLevelField('Ist2')
var ABILITY_SLF_RACE_TO_CONVERT=ConvertAbilityStringLevelField('Ndc1')
var ABILITY_SLF_PARTNER_UNIT_TYPE=ConvertAbilityStringLevelField('coa1')
var ABILITY_SLF_PARTNER_UNIT_TYPE_ONE=ConvertAbilityStringLevelField('dcp1')
var ABILITY_SLF_PARTNER_UNIT_TYPE_TWO=ConvertAbilityStringLevelField('dcp2')
var ABILITY_SLF_REQUIRED_UNIT_TYPE=ConvertAbilityStringLevelField('tpi1')
var ABILITY_SLF_CONVERTED_UNIT_TYPE=ConvertAbilityStringLevelField('tpi2')
var ABILITY_SLF_SPELL_LIST=ConvertAbilityStringLevelField('spb1')
var ABILITY_SLF_BASE_ORDER_ID_SPB5=ConvertAbilityStringLevelField('spb5')
var ABILITY_SLF_BASE_ORDER_ID_NCL6=ConvertAbilityStringLevelField('Ncl6')
var ABILITY_SLF_ABILITY_UPGRADE_1=ConvertAbilityStringLevelField('Neg3')
var ABILITY_SLF_ABILITY_UPGRADE_2=ConvertAbilityStringLevelField('Neg4')
var ABILITY_SLF_ABILITY_UPGRADE_3=ConvertAbilityStringLevelField('Neg5')
var ABILITY_SLF_ABILITY_UPGRADE_4=ConvertAbilityStringLevelField('Neg6')
var ABILITY_SLF_SPAWN_UNIT_ID_NSY2=ConvertAbilityStringLevelField('Nsy2')
var ITEM_IF_LEVEL=ConvertItemIntegerField('ilev')
var ITEM_IF_NUMBER_OF_CHARGES=ConvertItemIntegerField('iuse')
var ITEM_IF_COOLDOWN_GROUP=ConvertItemIntegerField('icid')
var ITEM_IF_MAX_HIT_POINTS=ConvertItemIntegerField('ihtp')
var ITEM_IF_HIT_POINTS=ConvertItemIntegerField('ihpc')
var ITEM_IF_PRIORITY=ConvertItemIntegerField('ipri')
var ITEM_IF_ARMOR_TYPE=ConvertItemIntegerField('iarm')
var ITEM_IF_TINTING_COLOR_RED=ConvertItemIntegerField('iclr')
var ITEM_IF_TINTING_COLOR_GREEN=ConvertItemIntegerField('iclg')
var ITEM_IF_TINTING_COLOR_BLUE=ConvertItemIntegerField('iclb')
var ITEM_IF_TINTING_COLOR_ALPHA=ConvertItemIntegerField('ical')
var ITEM_RF_SCALING_VALUE=ConvertItemRealField('isca')
var ITEM_BF_DROPPED_WHEN_CARRIER_DIES=ConvertItemBooleanField('idrp')
var ITEM_BF_CAN_BE_DROPPED=ConvertItemBooleanField('idro')
var ITEM_BF_PERISHABLE=ConvertItemBooleanField('iper')
var ITEM_BF_INCLUDE_AS_RANDOM_CHOICE=ConvertItemBooleanField('iprn')
var ITEM_BF_USE_AUTOMATICALLY_WHEN_ACQUIRED=ConvertItemBooleanField('ipow')
var ITEM_BF_CAN_BE_SOLD_TO_MERCHANTS=ConvertItemBooleanField('ipaw')
var ITEM_BF_ACTIVELY_USED=ConvertItemBooleanField('iusa')
var ITEM_SF_MODEL_USED=ConvertItemStringField('ifil')
var UNIT_IF_DEFENSE_TYPE=ConvertUnitIntegerField('udty')
var UNIT_IF_ARMOR_TYPE=ConvertUnitIntegerField('uarm')
var UNIT_IF_LOOPING_FADE_IN_RATE=ConvertUnitIntegerField('ulfi')
var UNIT_IF_LOOPING_FADE_OUT_RATE=ConvertUnitIntegerField('ulfo')
var UNIT_IF_AGILITY=ConvertUnitIntegerField('uagc')
var UNIT_IF_INTELLIGENCE=ConvertUnitIntegerField('uinc')
var UNIT_IF_STRENGTH=ConvertUnitIntegerField('ustc')
var UNIT_IF_AGILITY_PERMANENT=ConvertUnitIntegerField('uagm')
var UNIT_IF_INTELLIGENCE_PERMANENT=ConvertUnitIntegerField('uinm')
var UNIT_IF_STRENGTH_PERMANENT=ConvertUnitIntegerField('ustm')
var UNIT_IF_AGILITY_WITH_BONUS=ConvertUnitIntegerField('uagb')
var UNIT_IF_INTELLIGENCE_WITH_BONUS=ConvertUnitIntegerField('uinb')
var UNIT_IF_STRENGTH_WITH_BONUS=ConvertUnitIntegerField('ustb')
var UNIT_IF_GOLD_BOUNTY_AWARDED_NUMBER_OF_DICE=ConvertUnitIntegerField('ubdi')
var UNIT_IF_GOLD_BOUNTY_AWARDED_BASE=ConvertUnitIntegerField('ubba')
var UNIT_IF_GOLD_BOUNTY_AWARDED_SIDES_PER_DIE=ConvertUnitIntegerField('ubsi')
var UNIT_IF_LUMBER_BOUNTY_AWARDED_NUMBER_OF_DICE=ConvertUnitIntegerField('ulbd')
var UNIT_IF_LUMBER_BOUNTY_AWARDED_BASE=ConvertUnitIntegerField('ulba')
var UNIT_IF_LUMBER_BOUNTY_AWARDED_SIDES_PER_DIE=ConvertUnitIntegerField('ulbs')
var UNIT_IF_LEVEL=ConvertUnitIntegerField('ulev')
var UNIT_IF_FORMATION_RANK=ConvertUnitIntegerField('ufor')
var UNIT_IF_ORIENTATION_INTERPOLATION=ConvertUnitIntegerField('uori')
var UNIT_IF_ELEVATION_SAMPLE_POINTS=ConvertUnitIntegerField('uept')
var UNIT_IF_TINTING_COLOR_RED=ConvertUnitIntegerField('uclr')
var UNIT_IF_TINTING_COLOR_GREEN=ConvertUnitIntegerField('uclg')
var UNIT_IF_TINTING_COLOR_BLUE=ConvertUnitIntegerField('uclb')
var UNIT_IF_TINTING_COLOR_ALPHA=ConvertUnitIntegerField('ucal')
var UNIT_IF_MOVE_TYPE=ConvertUnitIntegerField('umvt')
var UNIT_IF_TARGETED_AS=ConvertUnitIntegerField('utar')
var UNIT_IF_UNIT_CLASSIFICATION=ConvertUnitIntegerField('utyp')
var UNIT_IF_HIT_POINTS_REGENERATION_TYPE=ConvertUnitIntegerField('uhrt')
var UNIT_IF_PLACEMENT_PREVENTED_BY=ConvertUnitIntegerField('upar')
var UNIT_IF_PRIMARY_ATTRIBUTE=ConvertUnitIntegerField('upra')
var UNIT_RF_STRENGTH_PER_LEVEL=ConvertUnitRealField('ustp')
var UNIT_RF_AGILITY_PER_LEVEL=ConvertUnitRealField('uagp')
var UNIT_RF_INTELLIGENCE_PER_LEVEL=ConvertUnitRealField('uinp')
var UNIT_RF_HIT_POINTS_REGENERATION_RATE=ConvertUnitRealField('uhpr')
var UNIT_RF_MANA_REGENERATION=ConvertUnitRealField('umpr')
var UNIT_RF_DEATH_TIME=ConvertUnitRealField('udtm')
var UNIT_RF_FLY_HEIGHT=ConvertUnitRealField('ufyh')
var UNIT_RF_TURN_RATE=ConvertUnitRealField('umvr')
var UNIT_RF_ELEVATION_SAMPLE_RADIUS=ConvertUnitRealField('uerd')
var UNIT_RF_FOG_OF_WAR_SAMPLE_RADIUS=ConvertUnitRealField('ufrd')
var UNIT_RF_MAXIMUM_PITCH_ANGLE_DEGREES=ConvertUnitRealField('umxp')
var UNIT_RF_MAXIMUM_ROLL_ANGLE_DEGREES=ConvertUnitRealField('umxr')
var UNIT_RF_SCALING_VALUE=ConvertUnitRealField('usca')
var UNIT_RF_ANIMATION_RUN_SPEED=ConvertUnitRealField('urun')
var UNIT_RF_SELECTION_SCALE=ConvertUnitRealField('ussc')
var UNIT_RF_SELECTION_CIRCLE_HEIGHT=ConvertUnitRealField('uslz')
var UNIT_RF_SHADOW_IMAGE_HEIGHT=ConvertUnitRealField('ushh')
var UNIT_RF_SHADOW_IMAGE_WIDTH=ConvertUnitRealField('ushw')
var UNIT_RF_SHADOW_IMAGE_CENTER_X=ConvertUnitRealField('ushx')
var UNIT_RF_SHADOW_IMAGE_CENTER_Y=ConvertUnitRealField('ushy')
var UNIT_RF_ANIMATION_WALK_SPEED=ConvertUnitRealField('uwal')
var UNIT_RF_DEFENSE=ConvertUnitRealField('udfc')
var UNIT_RF_SIGHT_RADIUS=ConvertUnitRealField('usir')
var UNIT_RF_PRIORITY=ConvertUnitRealField('upri')
var UNIT_RF_SPEED=ConvertUnitRealField('umvc')
var UNIT_RF_OCCLUDER_HEIGHT=ConvertUnitRealField('uocc')
var UNIT_RF_HP=ConvertUnitRealField('uhpc')
var UNIT_RF_MANA=ConvertUnitRealField('umpc')
var UNIT_RF_ACQUISITION_RANGE=ConvertUnitRealField('uacq')
var UNIT_RF_CAST_BACK_SWING=ConvertUnitRealField('ucbs')
var UNIT_RF_CAST_POINT=ConvertUnitRealField('ucpt')
var UNIT_RF_MINIMUM_ATTACK_RANGE=ConvertUnitRealField('uamn')
var UNIT_BF_RAISABLE=ConvertUnitBooleanField('urai')
var UNIT_BF_DECAYABLE=ConvertUnitBooleanField('udec')
var UNIT_BF_IS_A_BUILDING=ConvertUnitBooleanField('ubdg')
var UNIT_BF_USE_EXTENDED_LINE_OF_SIGHT=ConvertUnitBooleanField('ulos')
var UNIT_BF_NEUTRAL_BUILDING_SHOWS_MINIMAP_ICON=ConvertUnitBooleanField('unbm')
var UNIT_BF_HERO_HIDE_HERO_INTERFACE_ICON=ConvertUnitBooleanField('uhhb')
var UNIT_BF_HERO_HIDE_HERO_MINIMAP_DISPLAY=ConvertUnitBooleanField('uhhm')
var UNIT_BF_HERO_HIDE_HERO_DEATH_MESSAGE=ConvertUnitBooleanField('uhhd')
var UNIT_BF_HIDE_MINIMAP_DISPLAY=ConvertUnitBooleanField('uhom')
var UNIT_BF_SCALE_PROJECTILES=ConvertUnitBooleanField('uscb')
var UNIT_BF_SELECTION_CIRCLE_ON_WATER=ConvertUnitBooleanField('usew')
var UNIT_BF_HAS_WATER_SHADOW=ConvertUnitBooleanField('ushr')
var UNIT_SF_NAME=ConvertUnitStringField('unam')
var UNIT_SF_PROPER_NAMES=ConvertUnitStringField('upro')
var UNIT_SF_GROUND_TEXTURE=ConvertUnitStringField('uubs')
var UNIT_SF_SHADOW_IMAGE_UNIT=ConvertUnitStringField('ushu')
var UNIT_WEAPON_IF_ATTACK_DAMAGE_NUMBER_OF_DICE=ConvertUnitWeaponIntegerField('ua1d')
var UNIT_WEAPON_IF_ATTACK_DAMAGE_BASE=ConvertUnitWeaponIntegerField('ua1b')
var UNIT_WEAPON_IF_ATTACK_DAMAGE_SIDES_PER_DIE=ConvertUnitWeaponIntegerField('ua1s')
var UNIT_WEAPON_IF_ATTACK_MAXIMUM_NUMBER_OF_TARGETS=ConvertUnitWeaponIntegerField('utc1')
var UNIT_WEAPON_IF_ATTACK_ATTACK_TYPE=ConvertUnitWeaponIntegerField('ua1t')
var UNIT_WEAPON_IF_ATTACK_WEAPON_SOUND=ConvertUnitWeaponIntegerField('ucs1')
var UNIT_WEAPON_IF_ATTACK_AREA_OF_EFFECT_TARGETS=ConvertUnitWeaponIntegerField('ua1p')
var UNIT_WEAPON_IF_ATTACK_TARGETS_ALLOWED=ConvertUnitWeaponIntegerField('ua1g')
var UNIT_WEAPON_RF_ATTACK_BACKSWING_POINT=ConvertUnitWeaponRealField('ubs1')
var UNIT_WEAPON_RF_ATTACK_DAMAGE_POINT=ConvertUnitWeaponRealField('udp1')
var UNIT_WEAPON_RF_ATTACK_BASE_COOLDOWN=ConvertUnitWeaponRealField('ua1c')
var UNIT_WEAPON_RF_ATTACK_DAMAGE_LOSS_FACTOR=ConvertUnitWeaponRealField('udl1')
var UNIT_WEAPON_RF_ATTACK_DAMAGE_FACTOR_MEDIUM=ConvertUnitWeaponRealField('uhd1')
var UNIT_WEAPON_RF_ATTACK_DAMAGE_FACTOR_SMALL=ConvertUnitWeaponRealField('uqd1')
var UNIT_WEAPON_RF_ATTACK_DAMAGE_SPILL_DISTANCE=ConvertUnitWeaponRealField('usd1')
var UNIT_WEAPON_RF_ATTACK_DAMAGE_SPILL_RADIUS=ConvertUnitWeaponRealField('usr1')
var UNIT_WEAPON_RF_ATTACK_PROJECTILE_SPEED=ConvertUnitWeaponRealField('ua1z')
var UNIT_WEAPON_RF_ATTACK_PROJECTILE_ARC=ConvertUnitWeaponRealField('uma1')
var UNIT_WEAPON_RF_ATTACK_AREA_OF_EFFECT_FULL_DAMAGE=ConvertUnitWeaponRealField('ua1f')
var UNIT_WEAPON_RF_ATTACK_AREA_OF_EFFECT_MEDIUM_DAMAGE=ConvertUnitWeaponRealField('ua1h')
var UNIT_WEAPON_RF_ATTACK_AREA_OF_EFFECT_SMALL_DAMAGE=ConvertUnitWeaponRealField('ua1q')
var UNIT_WEAPON_RF_ATTACK_RANGE=ConvertUnitWeaponRealField('ua1r')
var UNIT_WEAPON_BF_ATTACK_SHOW_UI=ConvertUnitWeaponBooleanField('uwu1')
var UNIT_WEAPON_BF_ATTACKS_ENABLED=ConvertUnitWeaponBooleanField('uaen')
var UNIT_WEAPON_BF_ATTACK_PROJECTILE_HOMING_ENABLED=ConvertUnitWeaponBooleanField('umh1')
var UNIT_WEAPON_SF_ATTACK_PROJECTILE_ART=ConvertUnitWeaponStringField('ua1m')
var MOVE_TYPE_UNKNOWN=ConvertMoveType(0)
var MOVE_TYPE_FOOT=ConvertMoveType(1)
var MOVE_TYPE_FLY=ConvertMoveType(2)
var MOVE_TYPE_HORSE=ConvertMoveType(4)
var MOVE_TYPE_HOVER=ConvertMoveType(8)
var MOVE_TYPE_FLOAT=ConvertMoveType(16)
var MOVE_TYPE_AMPHIBIOUS=ConvertMoveType(32)
var MOVE_TYPE_UNBUILDABLE=ConvertMoveType(64)
var TARGET_FLAG_NONE=ConvertTargetFlag(1)
var TARGET_FLAG_GROUND=ConvertTargetFlag(2)
var TARGET_FLAG_AIR=ConvertTargetFlag(4)
var TARGET_FLAG_STRUCTURE=ConvertTargetFlag(8)
var TARGET_FLAG_WARD=ConvertTargetFlag(16)
var TARGET_FLAG_ITEM=ConvertTargetFlag(32)
var TARGET_FLAG_TREE=ConvertTargetFlag(64)
var TARGET_FLAG_WALL=ConvertTargetFlag(128)
var TARGET_FLAG_DEBRIS=ConvertTargetFlag(256)
var TARGET_FLAG_DECORATION=ConvertTargetFlag(512)
var TARGET_FLAG_BRIDGE=ConvertTargetFlag(1024)
var DEFENSE_TYPE_LIGHT=ConvertDefenseType(0)
var DEFENSE_TYPE_MEDIUM=ConvertDefenseType(1)
var DEFENSE_TYPE_LARGE=ConvertDefenseType(2)
var DEFENSE_TYPE_FORT=ConvertDefenseType(3)
var DEFENSE_TYPE_NORMAL=ConvertDefenseType(4)
var DEFENSE_TYPE_HERO=ConvertDefenseType(5)
var DEFENSE_TYPE_DIVINE=ConvertDefenseType(6)
var DEFENSE_TYPE_NONE=ConvertDefenseType(7)
var HERO_ATTRIBUTE_STR=ConvertHeroAttribute(1)
var HERO_ATTRIBUTE_INT=ConvertHeroAttribute(2)
var HERO_ATTRIBUTE_AGI=ConvertHeroAttribute(3)
var ARMOR_TYPE_WHOKNOWS=ConvertArmorType(0)
var ARMOR_TYPE_FLESH=ConvertArmorType(1)
var ARMOR_TYPE_METAL=ConvertArmorType(2)
var ARMOR_TYPE_WOOD=ConvertArmorType(3)
var ARMOR_TYPE_ETHREAL=ConvertArmorType(4)
var ARMOR_TYPE_STONE=ConvertArmorType(5)
var REGENERATION_TYPE_NONE=ConvertRegenType(0)
var REGENERATION_TYPE_ALWAYS=ConvertRegenType(1)
var REGENERATION_TYPE_BLIGHT=ConvertRegenType(2)
var REGENERATION_TYPE_DAY=ConvertRegenType(3)
var REGENERATION_TYPE_NIGHT=ConvertRegenType(4)
var UNIT_CATEGORY_GIANT=ConvertUnitCategory(1)
var UNIT_CATEGORY_UNDEAD=ConvertUnitCategory(2)
var UNIT_CATEGORY_SUMMONED=ConvertUnitCategory(4)
var UNIT_CATEGORY_MECHANICAL=ConvertUnitCategory(8)
var UNIT_CATEGORY_PEON=ConvertUnitCategory(16)
var UNIT_CATEGORY_SAPPER=ConvertUnitCategory(32)
var UNIT_CATEGORY_TOWNHALL=ConvertUnitCategory(64)
var UNIT_CATEGORY_ANCIENT=ConvertUnitCategory(128)
var UNIT_CATEGORY_NEUTRAL=ConvertUnitCategory(256)
var UNIT_CATEGORY_WARD=ConvertUnitCategory(512)
var UNIT_CATEGORY_STANDON=ConvertUnitCategory(1024)
var UNIT_CATEGORY_TAUREN=ConvertUnitCategory(2048)
var PATHING_FLAG_UNWALKABLE=ConvertPathingFlag(2)
var PATHING_FLAG_UNFLYABLE=ConvertPathingFlag(4)
var PATHING_FLAG_UNBUILDABLE=ConvertPathingFlag(8)
var PATHING_FLAG_UNPEONHARVEST=ConvertPathingFlag(16)
var PATHING_FLAG_BLIGHTED=ConvertPathingFlag(32)
var PATHING_FLAG_UNFLOATABLE=ConvertPathingFlag(64)
var PATHING_FLAG_UNAMPHIBIOUS=ConvertPathingFlag(128)
var PATHING_FLAG_UNITEMPLACABLE=ConvertPathingFlag(256)

function Deg2Rad(degrees) {
}
function Rad2Deg(radians) {
}
function Sin(radians) {
}
function Cos(radians) {
}
function Tan(radians) {
}
function Asin(y) {
}
function Acos(x) {
}
function Atan(x) {
}
function Atan2(y,x) {
}
function SquareRoot(x) {
}
function Pow(x,power) {
}
function MathRound(r) {
}
function I2R(i) {
}
function R2I(r) {
}
function I2S(i) {
}
function R2S(r) {
}
function R2SW(r,width,precision) {
}
function S2I(s) {
}
function S2R(s) {
}
function GetHandleId(h) {
}
function SubString(source,start,end) {
}
function StringLength(s) {
}
function StringCase(source,upper) {
}
function StringHash(s) {
}
function GetLocalizedString(source) {
}
function GetLocalizedHotkey(source) {
}
function SetMapName(name) {
}
function SetMapDescription(description) {
}
function SetTeams(teamcount) {
}
function SetPlayers(playercount) {
}
function DefineStartLocation(whichStartLoc,x,y) {
}
function DefineStartLocationLoc(whichStartLoc,whichLocation) {
}
function SetStartLocPrioCount(whichStartLoc,prioSlotCount) {
}
function SetStartLocPrio(whichStartLoc,prioSlotIndex,otherStartLocIndex,priority) {
}
function GetStartLocPrioSlot(whichStartLoc,prioSlotIndex) {
}
function GetStartLocPrio(whichStartLoc,prioSlotIndex) {
}
function SetEnemyStartLocPrioCount(whichStartLoc,prioSlotCount) {
}
function SetEnemyStartLocPrio(whichStartLoc,prioSlotIndex,otherStartLocIndex,priority) {
}
function SetGameTypeSupported(whichGameType,value) {
}
function SetMapFlag(whichMapFlag,value) {
}
function SetGamePlacement(whichPlacementType) {
}
function SetGameSpeed(whichspeed) {
}
function SetGameDifficulty(whichdifficulty) {
}
function SetResourceDensity(whichdensity) {
}
function SetCreatureDensity(whichdensity) {
}
function GetTeams() {
}
function GetPlayers() {
}
function IsGameTypeSupported(whichGameType) {
}
function GetGameTypeSelected() {
}
function IsMapFlagSet(whichMapFlag) {
}
function GetGamePlacement() {
}
function GetGameSpeed() {
}
function GetGameDifficulty() {
}
function GetResourceDensity() {
}
function GetCreatureDensity() {
}
function GetStartLocationX(whichStartLocation) {
}
function GetStartLocationY(whichStartLocation) {
}
function GetStartLocationLoc(whichStartLocation) {
}
function SetPlayerTeam(whichPlayer,whichTeam) {
}
function SetPlayerStartLocation(whichPlayer,startLocIndex) {
}
function ForcePlayerStartLocation(whichPlayer,startLocIndex) {
}
function SetPlayerColor(whichPlayer,color) {
}
function SetPlayerAlliance(sourcePlayer,otherPlayer,whichAllianceSetting,value) {
}
function SetPlayerTaxRate(sourcePlayer,otherPlayer,whichResource,rate) {
}
function SetPlayerRacePreference(whichPlayer,whichRacePreference) {
}
function SetPlayerRaceSelectable(whichPlayer,value) {
}
function SetPlayerController(whichPlayer,controlType) {
}
function SetPlayerName(whichPlayer,name) {
}
function SetPlayerOnScoreScreen(whichPlayer,flag) {
}
function GetPlayerTeam(whichPlayer) {
}
function GetPlayerStartLocation(whichPlayer) {
}
function GetPlayerColor(whichPlayer) {
}
function GetPlayerSelectable(whichPlayer) {
}
function GetPlayerController(whichPlayer) {
}
function GetPlayerSlotState(whichPlayer) {
}
function GetPlayerTaxRate(sourcePlayer,otherPlayer,whichResource) {
}
function IsPlayerRacePrefSet(whichPlayer,pref) {
}
function GetPlayerName(whichPlayer) {
}
function CreateTimer() {
}
function DestroyTimer(whichTimer) {
}
function TimerStart(whichTimer,timeout,periodic,handlerFunc) {
}
function TimerGetElapsed(whichTimer) {
}
function TimerGetRemaining(whichTimer) {
}
function TimerGetTimeout(whichTimer) {
}
function PauseTimer(whichTimer) {
}
function ResumeTimer(whichTimer) {
}
function GetExpiredTimer() {
}
function CreateGroup() {
}
function DestroyGroup(whichGroup) {
}
function GroupAddUnit(whichGroup,whichUnit) {
}
function GroupRemoveUnit(whichGroup,whichUnit) {
}
function BlzGroupAddGroupFast(whichGroup,addGroup) {
}
function BlzGroupRemoveGroupFast(whichGroup,removeGroup) {
}
function GroupClear(whichGroup) {
}
function BlzGroupGetSize(whichGroup) {
}
function BlzGroupUnitAt(whichGroup,index) {
}
function GroupEnumUnitsOfType(whichGroup,unitname,filter) {
}
function GroupEnumUnitsOfPlayer(whichGroup,whichPlayer,filter) {
}
function GroupEnumUnitsOfTypeCounted(whichGroup,unitname,filter,countLimit) {
}
function GroupEnumUnitsInRect(whichGroup,r,filter) {
}
function GroupEnumUnitsInRectCounted(whichGroup,r,filter,countLimit) {
}
function GroupEnumUnitsInRange(whichGroup,x,y,radius,filter) {
}
function GroupEnumUnitsInRangeOfLoc(whichGroup,whichLocation,radius,filter) {
}
function GroupEnumUnitsInRangeCounted(whichGroup,x,y,radius,filter,countLimit) {
}
function GroupEnumUnitsInRangeOfLocCounted(whichGroup,whichLocation,radius,filter,countLimit) {
}
function GroupEnumUnitsSelected(whichGroup,whichPlayer,filter) {
}
function GroupImmediateOrder(whichGroup,order) {
}
function GroupImmediateOrderById(whichGroup,order) {
}
function GroupPointOrder(whichGroup,order,x,y) {
}
function GroupPointOrderLoc(whichGroup,order,whichLocation) {
}
function GroupPointOrderById(whichGroup,order,x,y) {
}
function GroupPointOrderByIdLoc(whichGroup,order,whichLocation) {
}
function GroupTargetOrder(whichGroup,order,targetWidget) {
}
function GroupTargetOrderById(whichGroup,order,targetWidget) {
}
function ForGroup(whichGroup,callback) {
}
function FirstOfGroup(whichGroup) {
}
function CreateForce() {
}
function DestroyForce(whichForce) {
}
function ForceAddPlayer(whichForce,whichPlayer) {
}
function ForceRemovePlayer(whichForce,whichPlayer) {
}
function BlzForceHasPlayer(whichForce,whichPlayer) {
}
function ForceClear(whichForce) {
}
function ForceEnumPlayers(whichForce,filter) {
}
function ForceEnumPlayersCounted(whichForce,filter,countLimit) {
}
function ForceEnumAllies(whichForce,whichPlayer,filter) {
}
function ForceEnumEnemies(whichForce,whichPlayer,filter) {
}
function ForForce(whichForce,callback) {
}
function Rect(minx,miny,maxx,maxy) {
}
function RectFromLoc(min,max) {
}
function RemoveRect(whichRect) {
}
function SetRect(whichRect,minx,miny,maxx,maxy) {
}
function SetRectFromLoc(whichRect,min,max) {
}
function MoveRectTo(whichRect,newCenterX,newCenterY) {
}
function MoveRectToLoc(whichRect,newCenterLoc) {
}
function GetRectCenterX(whichRect) {
}
function GetRectCenterY(whichRect) {
}
function GetRectMinX(whichRect) {
}
function GetRectMinY(whichRect) {
}
function GetRectMaxX(whichRect) {
}
function GetRectMaxY(whichRect) {
}
function CreateRegion() {
}
function RemoveRegion(whichRegion) {
}
function RegionAddRect(whichRegion,r) {
}
function RegionClearRect(whichRegion,r) {
}
function RegionAddCell(whichRegion,x,y) {
}
function RegionAddCellAtLoc(whichRegion,whichLocation) {
}
function RegionClearCell(whichRegion,x,y) {
}
function RegionClearCellAtLoc(whichRegion,whichLocation) {
}
function Location(x,y) {
}
function RemoveLocation(whichLocation) {
}
function MoveLocation(whichLocation,newX,newY) {
}
function GetLocationX(whichLocation) {
}
function GetLocationY(whichLocation) {
}
function GetLocationZ(whichLocation) {
}
function IsUnitInRegion(whichRegion,whichUnit) {
}
function IsPointInRegion(whichRegion,x,y) {
}
function IsLocationInRegion(whichRegion,whichLocation) {
}
function GetWorldBounds() {
}
function CreateTrigger() {
}
function DestroyTrigger(whichTrigger) {
}
function ResetTrigger(whichTrigger) {
}
function EnableTrigger(whichTrigger) {
}
function DisableTrigger(whichTrigger) {
}
function IsTriggerEnabled(whichTrigger) {
}
function TriggerWaitOnSleeps(whichTrigger,flag) {
}
function IsTriggerWaitOnSleeps(whichTrigger) {
}
function GetFilterUnit() {
}
function GetEnumUnit() {
}
function GetFilterDestructable() {
}
function GetEnumDestructable() {
}
function GetFilterItem() {
}
function GetEnumItem() {
}
function ParseTags(taggedString) {
}
function GetFilterPlayer() {
}
function GetEnumPlayer() {
}
function GetTriggeringTrigger() {
}
function GetTriggerEventId() {
}
function GetTriggerEvalCount(whichTrigger) {
}
function GetTriggerExecCount(whichTrigger) {
}
function ExecuteFunc(funcName) {
}
function And(operandA,operandB) {
}
function Or(operandA,operandB) {
}
function Not(operand) {
}
function Condition(func) {
}
function DestroyCondition(c) {
}
function Filter(func) {
}
function DestroyFilter(f) {
}
function DestroyBoolExpr(e) {
}
function TriggerRegisterVariableEvent(whichTrigger,varName,opcode,limitval) {
}
function TriggerRegisterTimerEvent(whichTrigger,timeout,periodic) {
}
function TriggerRegisterTimerExpireEvent(whichTrigger,t) {
}
function TriggerRegisterGameStateEvent(whichTrigger,whichState,opcode,limitval) {
}
function TriggerRegisterDialogEvent(whichTrigger,whichDialog) {
}
function TriggerRegisterDialogButtonEvent(whichTrigger,whichButton) {
}
function GetEventGameState() {
}
function TriggerRegisterGameEvent(whichTrigger,whichGameEvent) {
}
function GetWinningPlayer() {
}
function TriggerRegisterEnterRegion(whichTrigger,whichRegion,filter) {
}
function GetTriggeringRegion() {
}
function GetEnteringUnit() {
}
function TriggerRegisterLeaveRegion(whichTrigger,whichRegion,filter) {
}
function GetLeavingUnit() {
}
function TriggerRegisterTrackableHitEvent(whichTrigger,t) {
}
function TriggerRegisterTrackableTrackEvent(whichTrigger,t) {
}
function TriggerRegisterCommandEvent(whichTrigger,whichAbility,order) {
}
function TriggerRegisterUpgradeCommandEvent(whichTrigger,whichUpgrade) {
}
function GetTriggeringTrackable() {
}
function GetClickedButton() {
}
function GetClickedDialog() {
}
function GetTournamentFinishSoonTimeRemaining() {
}
function GetTournamentFinishNowRule() {
}
function GetTournamentFinishNowPlayer() {
}
function GetTournamentScore(whichPlayer) {
}
function GetSaveBasicFilename() {
}
function TriggerRegisterPlayerEvent(whichTrigger,whichPlayer,whichPlayerEvent) {
}
function GetTriggerPlayer() {
}
function TriggerRegisterPlayerUnitEvent(whichTrigger,whichPlayer,whichPlayerUnitEvent,filter) {
}
function GetLevelingUnit() {
}
function GetLearningUnit() {
}
function GetLearnedSkill() {
}
function GetLearnedSkillLevel() {
}
function GetRevivableUnit() {
}
function GetRevivingUnit() {
}
function GetAttacker() {
}
function GetRescuer() {
}
function GetDyingUnit() {
}
function GetKillingUnit() {
}
function GetDecayingUnit() {
}
function GetConstructingStructure() {
}
function GetCancelledStructure() {
}
function GetConstructedStructure() {
}
function GetResearchingUnit() {
}
function GetResearched() {
}
function GetTrainedUnitType() {
}
function GetTrainedUnit() {
}
function GetDetectedUnit() {
}
function GetSummoningUnit() {
}
function GetSummonedUnit() {
}
function GetTransportUnit() {
}
function GetLoadedUnit() {
}
function GetSellingUnit() {
}
function GetSoldUnit() {
}
function GetBuyingUnit() {
}
function GetSoldItem() {
}
function GetChangingUnit() {
}
function GetChangingUnitPrevOwner() {
}
function GetManipulatingUnit() {
}
function GetManipulatedItem() {
}
function GetOrderedUnit() {
}
function GetIssuedOrderId() {
}
function GetOrderPointX() {
}
function GetOrderPointY() {
}
function GetOrderPointLoc() {
}
function GetOrderTarget() {
}
function GetOrderTargetDestructable() {
}
function GetOrderTargetItem() {
}
function GetOrderTargetUnit() {
}
function GetSpellAbilityUnit() {
}
function GetSpellAbilityId() {
}
function GetSpellAbility() {
}
function GetSpellTargetLoc() {
}
function GetSpellTargetX() {
}
function GetSpellTargetY() {
}
function GetSpellTargetDestructable() {
}
function GetSpellTargetItem() {
}
function GetSpellTargetUnit() {
}
function TriggerRegisterPlayerAllianceChange(whichTrigger,whichPlayer,whichAlliance) {
}
function TriggerRegisterPlayerStateEvent(whichTrigger,whichPlayer,whichState,opcode,limitval) {
}
function GetEventPlayerState() {
}
function TriggerRegisterPlayerChatEvent(whichTrigger,whichPlayer,chatMessageToDetect,exactMatchOnly) {
}
function GetEventPlayerChatString() {
}
function GetEventPlayerChatStringMatched() {
}
function TriggerRegisterDeathEvent(whichTrigger,whichWidget) {
}
function GetTriggerUnit() {
}
function TriggerRegisterUnitStateEvent(whichTrigger,whichUnit,whichState,opcode,limitval) {
}
function GetEventUnitState() {
}
function TriggerRegisterUnitEvent(whichTrigger,whichUnit,whichEvent) {
}
function GetEventDamage() {
}
function GetEventDamageSource() {
}
function GetEventDetectingPlayer() {
}
function TriggerRegisterFilterUnitEvent(whichTrigger,whichUnit,whichEvent,filter) {
}
function GetEventTargetUnit() {
}
function TriggerRegisterUnitInRange(whichTrigger,whichUnit,range,filter) {
}
function TriggerAddCondition(whichTrigger,condition) {
}
function TriggerRemoveCondition(whichTrigger,whichCondition) {
}
function TriggerClearConditions(whichTrigger) {
}
function TriggerAddAction(whichTrigger,actionFunc) {
}
function TriggerRemoveAction(whichTrigger,whichAction) {
}
function TriggerClearActions(whichTrigger) {
}
function TriggerSleepAction(timeout) {
}
function TriggerWaitForSound(s,offset) {
}
function TriggerEvaluate(whichTrigger) {
}
function TriggerExecute(whichTrigger) {
}
function TriggerExecuteWait(whichTrigger) {
}
function TriggerSyncStart() {
}
function TriggerSyncReady() {
}
function GetWidgetLife(whichWidget) {
}
function SetWidgetLife(whichWidget,newLife) {
}
function GetWidgetX(whichWidget) {
}
function GetWidgetY(whichWidget) {
}
function GetTriggerWidget() {
}
function CreateDestructable(objectid,x,y,face,scale,variation) {
}
function CreateDestructableZ(objectid,x,y,z,face,scale,variation) {
}
function CreateDeadDestructable(objectid,x,y,face,scale,variation) {
}
function CreateDeadDestructableZ(objectid,x,y,z,face,scale,variation) {
}
function RemoveDestructable(d) {
}
function KillDestructable(d) {
}
function SetDestructableInvulnerable(d,flag) {
}
function IsDestructableInvulnerable(d) {
}
function EnumDestructablesInRect(r,filter,actionFunc) {
}
function GetDestructableTypeId(d) {
}
function GetDestructableX(d) {
}
function GetDestructableY(d) {
}
function SetDestructableLife(d,life) {
}
function GetDestructableLife(d) {
}
function SetDestructableMaxLife(d,max) {
}
function GetDestructableMaxLife(d) {
}
function DestructableRestoreLife(d,life,birth) {
}
function QueueDestructableAnimation(d,whichAnimation) {
}
function SetDestructableAnimation(d,whichAnimation) {
}
function SetDestructableAnimationSpeed(d,speedFactor) {
}
function ShowDestructable(d,flag) {
}
function GetDestructableOccluderHeight(d) {
}
function SetDestructableOccluderHeight(d,height) {
}
function GetDestructableName(d) {
}
function GetTriggerDestructable() {
}
function CreateItem(itemid,x,y) {
}
function RemoveItem(whichItem) {
}
function GetItemPlayer(whichItem) {
}
function GetItemTypeId(i) {
}
function GetItemX(i) {
}
function GetItemY(i) {
}
function SetItemPosition(i,x,y) {
}
function SetItemDropOnDeath(whichItem,flag) {
}
function SetItemDroppable(i,flag) {
}
function SetItemPawnable(i,flag) {
}
function SetItemPlayer(whichItem,whichPlayer,changeColor) {
}
function SetItemInvulnerable(whichItem,flag) {
}
function IsItemInvulnerable(whichItem) {
}
function SetItemVisible(whichItem,show) {
}
function IsItemVisible(whichItem) {
}
function IsItemOwned(whichItem) {
}
function IsItemPowerup(whichItem) {
}
function IsItemSellable(whichItem) {
}
function IsItemPawnable(whichItem) {
}
function IsItemIdPowerup(itemId) {
}
function IsItemIdSellable(itemId) {
}
function IsItemIdPawnable(itemId) {
}
function EnumItemsInRect(r,filter,actionFunc) {
}
function GetItemLevel(whichItem) {
}
function GetItemType(whichItem) {
}
function SetItemDropID(whichItem,unitId) {
}
function GetItemName(whichItem) {
}
function GetItemCharges(whichItem) {
}
function SetItemCharges(whichItem,charges) {
}
function GetItemUserData(whichItem) {
}
function SetItemUserData(whichItem,data) {
}
function CreateUnit(id,unitid,x,y,face) {
}
function CreateUnitByName(whichPlayer,unitname,x,y,face) {
}
function CreateUnitAtLoc(id,unitid,whichLocation,face) {
}
function CreateUnitAtLocByName(id,unitname,whichLocation,face) {
}
function CreateCorpse(whichPlayer,unitid,x,y,face) {
}
function KillUnit(whichUnit) {
}
function RemoveUnit(whichUnit) {
}
function ShowUnit(whichUnit,show) {
}
function SetUnitState(whichUnit,whichUnitState,newVal) {
}
function SetUnitX(whichUnit,newX) {
}
function SetUnitY(whichUnit,newY) {
}
function SetUnitPosition(whichUnit,newX,newY) {
}
function SetUnitPositionLoc(whichUnit,whichLocation) {
}
function SetUnitFacing(whichUnit,facingAngle) {
}
function SetUnitFacingTimed(whichUnit,facingAngle,duration) {
}
function SetUnitMoveSpeed(whichUnit,newSpeed) {
}
function SetUnitFlyHeight(whichUnit,newHeight,rate) {
}
function SetUnitTurnSpeed(whichUnit,newTurnSpeed) {
}
function SetUnitPropWindow(whichUnit,newPropWindowAngle) {
}
function SetUnitAcquireRange(whichUnit,newAcquireRange) {
}
function SetUnitCreepGuard(whichUnit,creepGuard) {
}
function GetUnitAcquireRange(whichUnit) {
}
function GetUnitTurnSpeed(whichUnit) {
}
function GetUnitPropWindow(whichUnit) {
}
function GetUnitFlyHeight(whichUnit) {
}
function GetUnitDefaultAcquireRange(whichUnit) {
}
function GetUnitDefaultTurnSpeed(whichUnit) {
}
function GetUnitDefaultPropWindow(whichUnit) {
}
function GetUnitDefaultFlyHeight(whichUnit) {
}
function SetUnitOwner(whichUnit,whichPlayer,changeColor) {
}
function SetUnitColor(whichUnit,whichColor) {
}
function SetUnitScale(whichUnit,scaleX,scaleY,scaleZ) {
}
function SetUnitTimeScale(whichUnit,timeScale) {
}
function SetUnitBlendTime(whichUnit,blendTime) {
}
function SetUnitVertexColor(whichUnit,red,green,blue,alpha) {
}
function QueueUnitAnimation(whichUnit,whichAnimation) {
}
function SetUnitAnimation(whichUnit,whichAnimation) {
}
function SetUnitAnimationByIndex(whichUnit,whichAnimation) {
}
function SetUnitAnimationWithRarity(whichUnit,whichAnimation,rarity) {
}
function AddUnitAnimationProperties(whichUnit,animProperties,add) {
}
function SetUnitLookAt(whichUnit,whichBone,lookAtTarget,offsetX,offsetY,offsetZ) {
}
function ResetUnitLookAt(whichUnit) {
}
function SetUnitRescuable(whichUnit,byWhichPlayer,flag) {
}
function SetUnitRescueRange(whichUnit,range) {
}
function SetHeroStr(whichHero,newStr,permanent) {
}
function SetHeroAgi(whichHero,newAgi,permanent) {
}
function SetHeroInt(whichHero,newInt,permanent) {
}
function GetHeroStr(whichHero,includeBonuses) {
}
function GetHeroAgi(whichHero,includeBonuses) {
}
function GetHeroInt(whichHero,includeBonuses) {
}
function UnitStripHeroLevel(whichHero,howManyLevels) {
}
function GetHeroXP(whichHero) {
}
function SetHeroXP(whichHero,newXpVal,showEyeCandy) {
}
function GetHeroSkillPoints(whichHero) {
}
function UnitModifySkillPoints(whichHero,skillPointDelta) {
}
function AddHeroXP(whichHero,xpToAdd,showEyeCandy) {
}
function SetHeroLevel(whichHero,level,showEyeCandy) {
}
function GetHeroLevel(whichHero) {
}
function GetUnitLevel(whichUnit) {
}
function GetHeroProperName(whichHero) {
}
function SuspendHeroXP(whichHero,flag) {
}
function IsSuspendedXP(whichHero) {
}
function SelectHeroSkill(whichHero,abilcode) {
}
function GetUnitAbilityLevel(whichUnit,abilcode) {
}
function DecUnitAbilityLevel(whichUnit,abilcode) {
}
function IncUnitAbilityLevel(whichUnit,abilcode) {
}
function SetUnitAbilityLevel(whichUnit,abilcode,level) {
}
function ReviveHero(whichHero,x,y,doEyecandy) {
}
function ReviveHeroLoc(whichHero,loc,doEyecandy) {
}
function SetUnitExploded(whichUnit,exploded) {
}
function SetUnitInvulnerable(whichUnit,flag) {
}
function PauseUnit(whichUnit,flag) {
}
function IsUnitPaused(whichHero) {
}
function SetUnitPathing(whichUnit,flag) {
}
function ClearSelection() {
}
function SelectUnit(whichUnit,flag) {
}
function GetUnitPointValue(whichUnit) {
}
function GetUnitPointValueByType(unitType) {
}
function UnitAddItem(whichUnit,whichItem) {
}
function UnitAddItemById(whichUnit,itemId) {
}
function UnitAddItemToSlotById(whichUnit,itemId,itemSlot) {
}
function UnitRemoveItem(whichUnit,whichItem) {
}
function UnitRemoveItemFromSlot(whichUnit,itemSlot) {
}
function UnitHasItem(whichUnit,whichItem) {
}
function UnitItemInSlot(whichUnit,itemSlot) {
}
function UnitInventorySize(whichUnit) {
}
function UnitDropItemPoint(whichUnit,whichItem,x,y) {
}
function UnitDropItemSlot(whichUnit,whichItem,slot) {
}
function UnitDropItemTarget(whichUnit,whichItem,target) {
}
function UnitUseItem(whichUnit,whichItem) {
}
function UnitUseItemPoint(whichUnit,whichItem,x,y) {
}
function UnitUseItemTarget(whichUnit,whichItem,target) {
}
function GetUnitX(whichUnit) {
}
function GetUnitY(whichUnit) {
}
function GetUnitLoc(whichUnit) {
}
function GetUnitFacing(whichUnit) {
}
function GetUnitMoveSpeed(whichUnit) {
}
function GetUnitDefaultMoveSpeed(whichUnit) {
}
function GetUnitState(whichUnit,whichUnitState) {
}
function GetOwningPlayer(whichUnit) {
}
function GetUnitTypeId(whichUnit) {
}
function GetUnitRace(whichUnit) {
}
function GetUnitName(whichUnit) {
}
function GetUnitFoodUsed(whichUnit) {
}
function GetUnitFoodMade(whichUnit) {
}
function GetFoodMade(unitId) {
}
function GetFoodUsed(unitId) {
}
function SetUnitUseFood(whichUnit,useFood) {
}
function GetUnitRallyPoint(whichUnit) {
}
function GetUnitRallyUnit(whichUnit) {
}
function GetUnitRallyDestructable(whichUnit) {
}
function IsUnitInGroup(whichUnit,whichGroup) {
}
function IsUnitInForce(whichUnit,whichForce) {
}
function IsUnitOwnedByPlayer(whichUnit,whichPlayer) {
}
function IsUnitAlly(whichUnit,whichPlayer) {
}
function IsUnitEnemy(whichUnit,whichPlayer) {
}
function IsUnitVisible(whichUnit,whichPlayer) {
}
function IsUnitDetected(whichUnit,whichPlayer) {
}
function IsUnitInvisible(whichUnit,whichPlayer) {
}
function IsUnitFogged(whichUnit,whichPlayer) {
}
function IsUnitMasked(whichUnit,whichPlayer) {
}
function IsUnitSelected(whichUnit,whichPlayer) {
}
function IsUnitRace(whichUnit,whichRace) {
}
function IsUnitType(whichUnit,whichUnitType) {
}
function IsUnit(whichUnit,whichSpecifiedUnit) {
}
function IsUnitInRange(whichUnit,otherUnit,distance) {
}
function IsUnitInRangeXY(whichUnit,x,y,distance) {
}
function IsUnitInRangeLoc(whichUnit,whichLocation,distance) {
}
function IsUnitHidden(whichUnit) {
}
function IsUnitIllusion(whichUnit) {
}
function IsUnitInTransport(whichUnit,whichTransport) {
}
function IsUnitLoaded(whichUnit) {
}
function IsHeroUnitId(unitId) {
}
function IsUnitIdType(unitId,whichUnitType) {
}
function UnitShareVision(whichUnit,whichPlayer,share) {
}
function UnitSuspendDecay(whichUnit,suspend) {
}
function UnitAddType(whichUnit,whichUnitType) {
}
function UnitRemoveType(whichUnit,whichUnitType) {
}
function UnitAddAbility(whichUnit,abilityId) {
}
function UnitRemoveAbility(whichUnit,abilityId) {
}
function UnitMakeAbilityPermanent(whichUnit,permanent,abilityId) {
}
function UnitRemoveBuffs(whichUnit,removePositive,removeNegative) {
}
function UnitRemoveBuffsEx(whichUnit,removePositive,removeNegative,magic,physical,timedLife,aura,autoDispel) {
}
function UnitHasBuffsEx(whichUnit,removePositive,removeNegative,magic,physical,timedLife,aura,autoDispel) {
}
function UnitCountBuffsEx(whichUnit,removePositive,removeNegative,magic,physical,timedLife,aura,autoDispel) {
}
function UnitAddSleep(whichUnit,add) {
}
function UnitCanSleep(whichUnit) {
}
function UnitAddSleepPerm(whichUnit,add) {
}
function UnitCanSleepPerm(whichUnit) {
}
function UnitIsSleeping(whichUnit) {
}
function UnitWakeUp(whichUnit) {
}
function UnitApplyTimedLife(whichUnit,buffId,duration) {
}
function UnitIgnoreAlarm(whichUnit,flag) {
}
function UnitIgnoreAlarmToggled(whichUnit) {
}
function UnitResetCooldown(whichUnit) {
}
function UnitSetConstructionProgress(whichUnit,constructionPercentage) {
}
function UnitSetUpgradeProgress(whichUnit,upgradePercentage) {
}
function UnitPauseTimedLife(whichUnit,flag) {
}
function UnitSetUsesAltIcon(whichUnit,flag) {
}
function UnitDamagePoint(whichUnit,delay,radius,x,y,amount,attack,ranged,attackType,damageType,weaponType) {
}
function UnitDamageTarget(whichUnit,target,amount,attack,ranged,attackType,damageType,weaponType) {
}
function IssueImmediateOrder(whichUnit,order) {
}
function IssueImmediateOrderById(whichUnit,order) {
}
function IssuePointOrder(whichUnit,order,x,y) {
}
function IssuePointOrderLoc(whichUnit,order,whichLocation) {
}
function IssuePointOrderById(whichUnit,order,x,y) {
}
function IssuePointOrderByIdLoc(whichUnit,order,whichLocation) {
}
function IssueTargetOrder(whichUnit,order,targetWidget) {
}
function IssueTargetOrderById(whichUnit,order,targetWidget) {
}
function IssueInstantPointOrder(whichUnit,order,x,y,instantTargetWidget) {
}
function IssueInstantPointOrderById(whichUnit,order,x,y,instantTargetWidget) {
}
function IssueInstantTargetOrder(whichUnit,order,targetWidget,instantTargetWidget) {
}
function IssueInstantTargetOrderById(whichUnit,order,targetWidget,instantTargetWidget) {
}
function IssueBuildOrder(whichPeon,unitToBuild,x,y) {
}
function IssueBuildOrderById(whichPeon,unitId,x,y) {
}
function IssueNeutralImmediateOrder(forWhichPlayer,neutralStructure,unitToBuild) {
}
function IssueNeutralImmediateOrderById(forWhichPlayer,neutralStructure,unitId) {
}
function IssueNeutralPointOrder(forWhichPlayer,neutralStructure,unitToBuild,x,y) {
}
function IssueNeutralPointOrderById(forWhichPlayer,neutralStructure,unitId,x,y) {
}
function IssueNeutralTargetOrder(forWhichPlayer,neutralStructure,unitToBuild,target) {
}
function IssueNeutralTargetOrderById(forWhichPlayer,neutralStructure,unitId,target) {
}
function GetUnitCurrentOrder(whichUnit) {
}
function SetResourceAmount(whichUnit,amount) {
}
function AddResourceAmount(whichUnit,amount) {
}
function GetResourceAmount(whichUnit) {
}
function WaygateGetDestinationX(waygate) {
}
function WaygateGetDestinationY(waygate) {
}
function WaygateSetDestination(waygate,x,y) {
}
function WaygateActivate(waygate,activate) {
}
function WaygateIsActive(waygate) {
}
function AddItemToAllStock(itemId,currentStock,stockMax) {
}
function AddItemToStock(whichUnit,itemId,currentStock,stockMax) {
}
function AddUnitToAllStock(unitId,currentStock,stockMax) {
}
function AddUnitToStock(whichUnit,unitId,currentStock,stockMax) {
}
function RemoveItemFromAllStock(itemId) {
}
function RemoveItemFromStock(whichUnit,itemId) {
}
function RemoveUnitFromAllStock(unitId) {
}
function RemoveUnitFromStock(whichUnit,unitId) {
}
function SetAllItemTypeSlots(slots) {
}
function SetAllUnitTypeSlots(slots) {
}
function SetItemTypeSlots(whichUnit,slots) {
}
function SetUnitTypeSlots(whichUnit,slots) {
}
function GetUnitUserData(whichUnit) {
}
function SetUnitUserData(whichUnit,data) {
}
function Player(number) {
}
function GetLocalPlayer() {
}
function IsPlayerAlly(whichPlayer,otherPlayer) {
}
function IsPlayerEnemy(whichPlayer,otherPlayer) {
}
function IsPlayerInForce(whichPlayer,whichForce) {
}
function IsPlayerObserver(whichPlayer) {
}
function IsVisibleToPlayer(x,y,whichPlayer) {
}
function IsLocationVisibleToPlayer(whichLocation,whichPlayer) {
}
function IsFoggedToPlayer(x,y,whichPlayer) {
}
function IsLocationFoggedToPlayer(whichLocation,whichPlayer) {
}
function IsMaskedToPlayer(x,y,whichPlayer) {
}
function IsLocationMaskedToPlayer(whichLocation,whichPlayer) {
}
function GetPlayerRace(whichPlayer) {
}
function GetPlayerId(whichPlayer) {
}
function GetPlayerUnitCount(whichPlayer,includeIncomplete) {
}
function GetPlayerTypedUnitCount(whichPlayer,unitName,includeIncomplete,includeUpgrades) {
}
function GetPlayerStructureCount(whichPlayer,includeIncomplete) {
}
function GetPlayerState(whichPlayer,whichPlayerState) {
}
function GetPlayerScore(whichPlayer,whichPlayerScore) {
}
function GetPlayerAlliance(sourcePlayer,otherPlayer,whichAllianceSetting) {
}
function GetPlayerHandicap(whichPlayer) {
}
function GetPlayerHandicapXP(whichPlayer) {
}
function GetPlayerHandicapReviveTime(whichPlayer) {
}
function GetPlayerHandicapDamage(whichPlayer) {
}
function SetPlayerHandicap(whichPlayer,handicap) {
}
function SetPlayerHandicapXP(whichPlayer,handicap) {
}
function SetPlayerHandicapReviveTime(whichPlayer,handicap) {
}
function SetPlayerHandicapDamage(whichPlayer,handicap) {
}
function SetPlayerTechMaxAllowed(whichPlayer,techid,maximum) {
}
function GetPlayerTechMaxAllowed(whichPlayer,techid) {
}
function AddPlayerTechResearched(whichPlayer,techid,levels) {
}
function SetPlayerTechResearched(whichPlayer,techid,setToLevel) {
}
function GetPlayerTechResearched(whichPlayer,techid,specificonly) {
}
function GetPlayerTechCount(whichPlayer,techid,specificonly) {
}
function SetPlayerUnitsOwner(whichPlayer,newOwner) {
}
function CripplePlayer(whichPlayer,toWhichPlayers,flag) {
}
function SetPlayerAbilityAvailable(whichPlayer,abilid,avail) {
}
function SetPlayerState(whichPlayer,whichPlayerState,value) {
}
function RemovePlayer(whichPlayer,gameResult) {
}
function CachePlayerHeroData(whichPlayer) {
}
function SetFogStateRect(forWhichPlayer,whichState,where,useSharedVision) {
}
function SetFogStateRadius(forWhichPlayer,whichState,centerx,centerY,radius,useSharedVision) {
}
function SetFogStateRadiusLoc(forWhichPlayer,whichState,center,radius,useSharedVision) {
}
function FogMaskEnable(enable) {
}
function IsFogMaskEnabled() {
}
function FogEnable(enable) {
}
function IsFogEnabled() {
}
function CreateFogModifierRect(forWhichPlayer,whichState,where,useSharedVision,afterUnits) {
}
function CreateFogModifierRadius(forWhichPlayer,whichState,centerx,centerY,radius,useSharedVision,afterUnits) {
}
function CreateFogModifierRadiusLoc(forWhichPlayer,whichState,center,radius,useSharedVision,afterUnits) {
}
function DestroyFogModifier(whichFogModifier) {
}
function FogModifierStart(whichFogModifier) {
}
function FogModifierStop(whichFogModifier) {
}
function VersionGet() {
}
function VersionCompatible(whichVersion) {
}
function VersionSupported(whichVersion) {
}
function EndGame(doScoreScreen) {
}
function ChangeLevel(newLevel,doScoreScreen) {
}
function RestartGame(doScoreScreen) {
}
function ReloadGame() {
}
function SetCampaignMenuRace(r) {
}
function SetCampaignMenuRaceEx(campaignIndex) {
}
function ForceCampaignSelectScreen() {
}
function LoadGame(saveFileName,doScoreScreen) {
}
function SaveGame(saveFileName) {
}
function RenameSaveDirectory(sourceDirName,destDirName) {
}
function RemoveSaveDirectory(sourceDirName) {
}
function CopySaveGame(sourceSaveName,destSaveName) {
}
function SaveGameExists(saveName) {
}
function SetMaxCheckpointSaves(maxCheckpointSaves) {
}
function SaveGameCheckpoint(saveFileName,showWindow) {
}
function SyncSelections() {
}
function SetFloatGameState(whichFloatGameState,value) {
}
function GetFloatGameState(whichFloatGameState) {
}
function SetIntegerGameState(whichIntegerGameState,value) {
}
function GetIntegerGameState(whichIntegerGameState) {
}
function SetTutorialCleared(cleared) {
}
function SetMissionAvailable(campaignNumber,missionNumber,available) {
}
function SetCampaignAvailable(campaignNumber,available) {
}
function SetOpCinematicAvailable(campaignNumber,available) {
}
function SetEdCinematicAvailable(campaignNumber,available) {
}
function GetDefaultDifficulty() {
}
function SetDefaultDifficulty(g) {
}
function SetCustomCampaignButtonVisible(whichButton,visible) {
}
function GetCustomCampaignButtonVisible(whichButton) {
}
function DoNotSaveReplay() {
}
function DialogCreate() {
}
function DialogDestroy(whichDialog) {
}
function DialogClear(whichDialog) {
}
function DialogSetMessage(whichDialog,messageText) {
}
function DialogAddButton(whichDialog,buttonText,hotkey) {
}
function DialogAddQuitButton(whichDialog,doScoreScreen,buttonText,hotkey) {
}
function DialogDisplay(whichPlayer,whichDialog,flag) {
}
function ReloadGameCachesFromDisk() {
}
function InitGameCache(campaignFile) {
}
function SaveGameCache(whichCache) {
}
function StoreInteger(cache,missionKey,key,value) {
}
function StoreReal(cache,missionKey,key,value) {
}
function StoreBoolean(cache,missionKey,key,value) {
}
function StoreUnit(cache,missionKey,key,whichUnit) {
}
function StoreString(cache,missionKey,key,value) {
}
function SyncStoredInteger(cache,missionKey,key) {
}
function SyncStoredReal(cache,missionKey,key) {
}
function SyncStoredBoolean(cache,missionKey,key) {
}
function SyncStoredUnit(cache,missionKey,key) {
}
function SyncStoredString(cache,missionKey,key) {
}
function HaveStoredInteger(cache,missionKey,key) {
}
function HaveStoredReal(cache,missionKey,key) {
}
function HaveStoredBoolean(cache,missionKey,key) {
}
function HaveStoredUnit(cache,missionKey,key) {
}
function HaveStoredString(cache,missionKey,key) {
}
function FlushGameCache(cache) {
}
function FlushStoredMission(cache,missionKey) {
}
function FlushStoredInteger(cache,missionKey,key) {
}
function FlushStoredReal(cache,missionKey,key) {
}
function FlushStoredBoolean(cache,missionKey,key) {
}
function FlushStoredUnit(cache,missionKey,key) {
}
function FlushStoredString(cache,missionKey,key) {
}
function GetStoredInteger(cache,missionKey,key) {
}
function GetStoredReal(cache,missionKey,key) {
}
function GetStoredBoolean(cache,missionKey,key) {
}
function GetStoredString(cache,missionKey,key) {
}
function RestoreUnit(cache,missionKey,key,forWhichPlayer,x,y,facing) {
}
function InitHashtable() {
}
function SaveInteger(table,parentKey,childKey,value) {
}
function SaveReal(table,parentKey,childKey,value) {
}
function SaveBoolean(table,parentKey,childKey,value) {
}
function SaveStr(table,parentKey,childKey,value) {
}
function SavePlayerHandle(table,parentKey,childKey,whichPlayer) {
}
function SaveWidgetHandle(table,parentKey,childKey,whichWidget) {
}
function SaveDestructableHandle(table,parentKey,childKey,whichDestructable) {
}
function SaveItemHandle(table,parentKey,childKey,whichItem) {
}
function SaveUnitHandle(table,parentKey,childKey,whichUnit) {
}
function SaveAbilityHandle(table,parentKey,childKey,whichAbility) {
}
function SaveTimerHandle(table,parentKey,childKey,whichTimer) {
}
function SaveTriggerHandle(table,parentKey,childKey,whichTrigger) {
}
function SaveTriggerConditionHandle(table,parentKey,childKey,whichTriggercondition) {
}
function SaveTriggerActionHandle(table,parentKey,childKey,whichTriggeraction) {
}
function SaveTriggerEventHandle(table,parentKey,childKey,whichEvent) {
}
function SaveForceHandle(table,parentKey,childKey,whichForce) {
}
function SaveGroupHandle(table,parentKey,childKey,whichGroup) {
}
function SaveLocationHandle(table,parentKey,childKey,whichLocation) {
}
function SaveRectHandle(table,parentKey,childKey,whichRect) {
}
function SaveBooleanExprHandle(table,parentKey,childKey,whichBoolexpr) {
}
function SaveSoundHandle(table,parentKey,childKey,whichSound) {
}
function SaveEffectHandle(table,parentKey,childKey,whichEffect) {
}
function SaveUnitPoolHandle(table,parentKey,childKey,whichUnitpool) {
}
function SaveItemPoolHandle(table,parentKey,childKey,whichItempool) {
}
function SaveQuestHandle(table,parentKey,childKey,whichQuest) {
}
function SaveQuestItemHandle(table,parentKey,childKey,whichQuestitem) {
}
function SaveDefeatConditionHandle(table,parentKey,childKey,whichDefeatcondition) {
}
function SaveTimerDialogHandle(table,parentKey,childKey,whichTimerdialog) {
}
function SaveLeaderboardHandle(table,parentKey,childKey,whichLeaderboard) {
}
function SaveMultiboardHandle(table,parentKey,childKey,whichMultiboard) {
}
function SaveMultiboardItemHandle(table,parentKey,childKey,whichMultiboarditem) {
}
function SaveTrackableHandle(table,parentKey,childKey,whichTrackable) {
}
function SaveDialogHandle(table,parentKey,childKey,whichDialog) {
}
function SaveButtonHandle(table,parentKey,childKey,whichButton) {
}
function SaveTextTagHandle(table,parentKey,childKey,whichTexttag) {
}
function SaveLightningHandle(table,parentKey,childKey,whichLightning) {
}
function SaveImageHandle(table,parentKey,childKey,whichImage) {
}
function SaveUbersplatHandle(table,parentKey,childKey,whichUbersplat) {
}
function SaveRegionHandle(table,parentKey,childKey,whichRegion) {
}
function SaveFogStateHandle(table,parentKey,childKey,whichFogState) {
}
function SaveFogModifierHandle(table,parentKey,childKey,whichFogModifier) {
}
function SaveAgentHandle(table,parentKey,childKey,whichAgent) {
}
function SaveHashtableHandle(table,parentKey,childKey,whichHashtable) {
}
function SaveFrameHandle(table,parentKey,childKey,whichFrameHandle) {
}
function LoadInteger(table,parentKey,childKey) {
}
function LoadReal(table,parentKey,childKey) {
}
function LoadBoolean(table,parentKey,childKey) {
}
function LoadStr(table,parentKey,childKey) {
}
function LoadPlayerHandle(table,parentKey,childKey) {
}
function LoadWidgetHandle(table,parentKey,childKey) {
}
function LoadDestructableHandle(table,parentKey,childKey) {
}
function LoadItemHandle(table,parentKey,childKey) {
}
function LoadUnitHandle(table,parentKey,childKey) {
}
function LoadAbilityHandle(table,parentKey,childKey) {
}
function LoadTimerHandle(table,parentKey,childKey) {
}
function LoadTriggerHandle(table,parentKey,childKey) {
}
function LoadTriggerConditionHandle(table,parentKey,childKey) {
}
function LoadTriggerActionHandle(table,parentKey,childKey) {
}
function LoadTriggerEventHandle(table,parentKey,childKey) {
}
function LoadForceHandle(table,parentKey,childKey) {
}
function LoadGroupHandle(table,parentKey,childKey) {
}
function LoadLocationHandle(table,parentKey,childKey) {
}
function LoadRectHandle(table,parentKey,childKey) {
}
function LoadBooleanExprHandle(table,parentKey,childKey) {
}
function LoadSoundHandle(table,parentKey,childKey) {
}
function LoadEffectHandle(table,parentKey,childKey) {
}
function LoadUnitPoolHandle(table,parentKey,childKey) {
}
function LoadItemPoolHandle(table,parentKey,childKey) {
}
function LoadQuestHandle(table,parentKey,childKey) {
}
function LoadQuestItemHandle(table,parentKey,childKey) {
}
function LoadDefeatConditionHandle(table,parentKey,childKey) {
}
function LoadTimerDialogHandle(table,parentKey,childKey) {
}
function LoadLeaderboardHandle(table,parentKey,childKey) {
}
function LoadMultiboardHandle(table,parentKey,childKey) {
}
function LoadMultiboardItemHandle(table,parentKey,childKey) {
}
function LoadTrackableHandle(table,parentKey,childKey) {
}
function LoadDialogHandle(table,parentKey,childKey) {
}
function LoadButtonHandle(table,parentKey,childKey) {
}
function LoadTextTagHandle(table,parentKey,childKey) {
}
function LoadLightningHandle(table,parentKey,childKey) {
}
function LoadImageHandle(table,parentKey,childKey) {
}
function LoadUbersplatHandle(table,parentKey,childKey) {
}
function LoadRegionHandle(table,parentKey,childKey) {
}
function LoadFogStateHandle(table,parentKey,childKey) {
}
function LoadFogModifierHandle(table,parentKey,childKey) {
}
function LoadHashtableHandle(table,parentKey,childKey) {
}
function LoadFrameHandle(table,parentKey,childKey) {
}
function HaveSavedInteger(table,parentKey,childKey) {
}
function HaveSavedReal(table,parentKey,childKey) {
}
function HaveSavedBoolean(table,parentKey,childKey) {
}
function HaveSavedString(table,parentKey,childKey) {
}
function HaveSavedHandle(table,parentKey,childKey) {
}
function RemoveSavedInteger(table,parentKey,childKey) {
}
function RemoveSavedReal(table,parentKey,childKey) {
}
function RemoveSavedBoolean(table,parentKey,childKey) {
}
function RemoveSavedString(table,parentKey,childKey) {
}
function RemoveSavedHandle(table,parentKey,childKey) {
}
function FlushParentHashtable(table) {
}
function FlushChildHashtable(table,parentKey) {
}
function GetRandomInt(lowBound,highBound) {
}
function GetRandomReal(lowBound,highBound) {
}
function CreateUnitPool() {
}
function DestroyUnitPool(whichPool) {
}
function UnitPoolAddUnitType(whichPool,unitId,weight) {
}
function UnitPoolRemoveUnitType(whichPool,unitId) {
}
function PlaceRandomUnit(whichPool,forWhichPlayer,x,y,facing) {
}
function CreateItemPool() {
}
function DestroyItemPool(whichItemPool) {
}
function ItemPoolAddItemType(whichItemPool,itemId,weight) {
}
function ItemPoolRemoveItemType(whichItemPool,itemId) {
}
function PlaceRandomItem(whichItemPool,x,y) {
}
function ChooseRandomCreep(level) {
}
function ChooseRandomNPBuilding() {
}
function ChooseRandomItem(level) {
}
function ChooseRandomItemEx(whichType,level) {
}
function SetRandomSeed(seed) {
}
function SetTerrainFog(a,b,c,d,e) {
}
function ResetTerrainFog() {
}
function SetUnitFog(a,b,c,d,e) {
}
function SetTerrainFogEx(style,zstart,zend,density,red,green,blue) {
}
function DisplayTextToPlayer(toPlayer,x,y,message) {
}
function DisplayTimedTextToPlayer(toPlayer,x,y,duration,message) {
}
function DisplayTimedTextFromPlayer(toPlayer,x,y,duration,message) {
}
function ClearTextMessages() {
}
function SetDayNightModels(terrainDNCFile,unitDNCFile) {
}
function SetPortraitLight(portraitDNCFile) {
}
function SetSkyModel(skyModelFile) {
}
function EnableUserControl(b) {
}
function EnableUserUI(b) {
}
function SuspendTimeOfDay(b) {
}
function SetTimeOfDayScale(r) {
}
function GetTimeOfDayScale() {
}
function ShowInterface(flag,fadeDuration) {
}
function PauseGame(flag) {
}
function UnitAddIndicator(whichUnit,red,green,blue,alpha) {
}
function AddIndicator(whichWidget,red,green,blue,alpha) {
}
function PingMinimap(x,y,duration) {
}
function PingMinimapEx(x,y,duration,red,green,blue,extraEffects) {
}
function CreateMinimapIconOnUnit(whichUnit,red,green,blue,pingPath,fogVisibility) {
}
function CreateMinimapIconAtLoc(where,red,green,blue,pingPath,fogVisibility) {
}
function CreateMinimapIcon(x,y,red,green,blue,pingPath,fogVisibility) {
}
function SkinManagerGetLocalPath(key) {
}
function DestroyMinimapIcon(pingId) {
}
function SetMinimapIconVisible(whichMinimapIcon,visible) {
}
function SetMinimapIconOrphanDestroy(whichMinimapIcon,doDestroy) {
}
function EnableOcclusion(flag) {
}
function SetIntroShotText(introText) {
}
function SetIntroShotModel(introModelPath) {
}
function EnableWorldFogBoundary(b) {
}
function PlayModelCinematic(modelName) {
}
function PlayCinematic(movieName) {
}
function ForceUIKey(key) {
}
function ForceUICancel() {
}
function DisplayLoadDialog() {
}
function SetAltMinimapIcon(iconPath) {
}
function DisableRestartMission(flag) {
}
function CreateTextTag() {
}
function DestroyTextTag(t) {
}
function SetTextTagText(t,s,height) {
}
function SetTextTagPos(t,x,y,heightOffset) {
}
function SetTextTagPosUnit(t,whichUnit,heightOffset) {
}
function SetTextTagColor(t,red,green,blue,alpha) {
}
function SetTextTagVelocity(t,xvel,yvel) {
}
function SetTextTagVisibility(t,flag) {
}
function SetTextTagSuspended(t,flag) {
}
function SetTextTagPermanent(t,flag) {
}
function SetTextTagAge(t,age) {
}
function SetTextTagLifespan(t,lifespan) {
}
function SetTextTagFadepoint(t,fadepoint) {
}
function SetReservedLocalHeroButtons(reserved) {
}
function GetAllyColorFilterState() {
}
function SetAllyColorFilterState(state) {
}
function GetCreepCampFilterState() {
}
function SetCreepCampFilterState(state) {
}
function EnableMinimapFilterButtons(enableAlly,enableCreep) {
}
function EnableDragSelect(state,ui) {
}
function EnablePreSelect(state,ui) {
}
function EnableSelect(state,ui) {
}
function CreateTrackable(trackableModelPath,x,y,facing) {
}
function CreateQuest() {
}
function DestroyQuest(whichQuest) {
}
function QuestSetTitle(whichQuest,title) {
}
function QuestSetDescription(whichQuest,description) {
}
function QuestSetIconPath(whichQuest,iconPath) {
}
function QuestSetRequired(whichQuest,required) {
}
function QuestSetCompleted(whichQuest,completed) {
}
function QuestSetDiscovered(whichQuest,discovered) {
}
function QuestSetFailed(whichQuest,failed) {
}
function QuestSetEnabled(whichQuest,enabled) {
}
function IsQuestRequired(whichQuest) {
}
function IsQuestCompleted(whichQuest) {
}
function IsQuestDiscovered(whichQuest) {
}
function IsQuestFailed(whichQuest) {
}
function IsQuestEnabled(whichQuest) {
}
function QuestCreateItem(whichQuest) {
}
function QuestItemSetDescription(whichQuestItem,description) {
}
function QuestItemSetCompleted(whichQuestItem,completed) {
}
function IsQuestItemCompleted(whichQuestItem) {
}
function CreateDefeatCondition() {
}
function DestroyDefeatCondition(whichCondition) {
}
function DefeatConditionSetDescription(whichCondition,description) {
}
function FlashQuestDialogButton() {
}
function ForceQuestDialogUpdate() {
}
function CreateTimerDialog(t) {
}
function DestroyTimerDialog(whichDialog) {
}
function TimerDialogSetTitle(whichDialog,title) {
}
function TimerDialogSetTitleColor(whichDialog,red,green,blue,alpha) {
}
function TimerDialogSetTimeColor(whichDialog,red,green,blue,alpha) {
}
function TimerDialogSetSpeed(whichDialog,speedMultFactor) {
}
function TimerDialogDisplay(whichDialog,display) {
}
function IsTimerDialogDisplayed(whichDialog) {
}
function TimerDialogSetRealTimeRemaining(whichDialog,timeRemaining) {
}
function CreateLeaderboard() {
}
function DestroyLeaderboard(lb) {
}
function LeaderboardDisplay(lb,show) {
}
function IsLeaderboardDisplayed(lb) {
}
function LeaderboardGetItemCount(lb) {
}
function LeaderboardSetSizeByItemCount(lb,count) {
}
function LeaderboardAddItem(lb,label,value,p) {
}
function LeaderboardRemoveItem(lb,index) {
}
function LeaderboardRemovePlayerItem(lb,p) {
}
function LeaderboardClear(lb) {
}
function LeaderboardSortItemsByValue(lb,ascending) {
}
function LeaderboardSortItemsByPlayer(lb,ascending) {
}
function LeaderboardSortItemsByLabel(lb,ascending) {
}
function LeaderboardHasPlayerItem(lb,p) {
}
function LeaderboardGetPlayerIndex(lb,p) {
}
function LeaderboardSetLabel(lb,label) {
}
function LeaderboardGetLabelText(lb) {
}
function PlayerSetLeaderboard(toPlayer,lb) {
}
function PlayerGetLeaderboard(toPlayer) {
}
function LeaderboardSetLabelColor(lb,red,green,blue,alpha) {
}
function LeaderboardSetValueColor(lb,red,green,blue,alpha) {
}
function LeaderboardSetStyle(lb,showLabel,showNames,showValues,showIcons) {
}
function LeaderboardSetItemValue(lb,whichItem,val) {
}
function LeaderboardSetItemLabel(lb,whichItem,val) {
}
function LeaderboardSetItemStyle(lb,whichItem,showLabel,showValue,showIcon) {
}
function LeaderboardSetItemLabelColor(lb,whichItem,red,green,blue,alpha) {
}
function LeaderboardSetItemValueColor(lb,whichItem,red,green,blue,alpha) {
}
function CreateMultiboard() {
}
function DestroyMultiboard(lb) {
}
function MultiboardDisplay(lb,show) {
}
function IsMultiboardDisplayed(lb) {
}
function MultiboardMinimize(lb,minimize) {
}
function IsMultiboardMinimized(lb) {
}
function MultiboardClear(lb) {
}
function MultiboardSetTitleText(lb,label) {
}
function MultiboardGetTitleText(lb) {
}
function MultiboardSetTitleTextColor(lb,red,green,blue,alpha) {
}
function MultiboardGetRowCount(lb) {
}
function MultiboardGetColumnCount(lb) {
}
function MultiboardSetColumnCount(lb,count) {
}
function MultiboardSetRowCount(lb,count) {
}
function MultiboardSetItemsStyle(lb,showValues,showIcons) {
}
function MultiboardSetItemsValue(lb,value) {
}
function MultiboardSetItemsValueColor(lb,red,green,blue,alpha) {
}
function MultiboardSetItemsWidth(lb,width) {
}
function MultiboardSetItemsIcon(lb,iconPath) {
}
function MultiboardGetItem(lb,row,column) {
}
function MultiboardReleaseItem(mbi) {
}
function MultiboardSetItemStyle(mbi,showValue,showIcon) {
}
function MultiboardSetItemValue(mbi,val) {
}
function MultiboardSetItemValueColor(mbi,red,green,blue,alpha) {
}
function MultiboardSetItemWidth(mbi,width) {
}
function MultiboardSetItemIcon(mbi,iconFileName) {
}
function MultiboardSuppressDisplay(flag) {
}
function SetCameraPosition(x,y) {
}
function SetCameraQuickPosition(x,y) {
}
function SetCameraBounds(x1,y1,x2,y2,x3,y3,x4,y4) {
}
function StopCamera() {
}
function ResetToGameCamera(duration) {
}
function PanCameraTo(x,y) {
}
function PanCameraToTimed(x,y,duration) {
}
function PanCameraToWithZ(x,y,zOffsetDest) {
}
function PanCameraToTimedWithZ(x,y,zOffsetDest,duration) {
}
function SetCinematicCamera(cameraModelFile) {
}
function SetCameraRotateMode(x,y,radiansToSweep,duration) {
}
function SetCameraField(whichField,value,duration) {
}
function AdjustCameraField(whichField,offset,duration) {
}
function SetCameraTargetController(whichUnit,xoffset,yoffset,inheritOrientation) {
}
function SetCameraOrientController(whichUnit,xoffset,yoffset) {
}
function CreateCameraSetup() {
}
function CameraSetupSetField(whichSetup,whichField,value,duration) {
}
function CameraSetupGetField(whichSetup,whichField) {
}
function CameraSetupSetDestPosition(whichSetup,x,y,duration) {
}
function CameraSetupGetDestPositionLoc(whichSetup) {
}
function CameraSetupGetDestPositionX(whichSetup) {
}
function CameraSetupGetDestPositionY(whichSetup) {
}
function CameraSetupApply(whichSetup,doPan,panTimed) {
}
function CameraSetupApplyWithZ(whichSetup,zDestOffset) {
}
function CameraSetupApplyForceDuration(whichSetup,doPan,forceDuration) {
}
function CameraSetupApplyForceDurationWithZ(whichSetup,zDestOffset,forceDuration) {
}
function BlzCameraSetupSetLabel(whichSetup,label) {
}
function BlzCameraSetupGetLabel(whichSetup) {
}
function CameraSetTargetNoise(mag,velocity) {
}
function CameraSetSourceNoise(mag,velocity) {
}
function CameraSetTargetNoiseEx(mag,velocity,vertOnly) {
}
function CameraSetSourceNoiseEx(mag,velocity,vertOnly) {
}
function CameraSetSmoothingFactor(factor) {
}
function CameraSetFocalDistance(distance) {
}
function CameraSetDepthOfFieldScale(scale) {
}
function SetCineFilterTexture(filename) {
}
function SetCineFilterBlendMode(whichMode) {
}
function SetCineFilterTexMapFlags(whichFlags) {
}
function SetCineFilterStartUV(minu,minv,maxu,maxv) {
}
function SetCineFilterEndUV(minu,minv,maxu,maxv) {
}
function SetCineFilterStartColor(red,green,blue,alpha) {
}
function SetCineFilterEndColor(red,green,blue,alpha) {
}
function SetCineFilterDuration(duration) {
}
function DisplayCineFilter(flag) {
}
function IsCineFilterDisplayed() {
}
function SetCinematicScene(portraitUnitId,color,speakerTitle,text,sceneDuration,voiceoverDuration) {
}
function EndCinematicScene() {
}
function ForceCinematicSubtitles(flag) {
}
function SetCinematicAudio(cinematicAudio) {
}
function GetCameraMargin(whichMargin) {
}
function GetCameraBoundMinX() {
}
function GetCameraBoundMinY() {
}
function GetCameraBoundMaxX() {
}
function GetCameraBoundMaxY() {
}
function GetCameraField(whichField) {
}
function GetCameraTargetPositionX() {
}
function GetCameraTargetPositionY() {
}
function GetCameraTargetPositionZ() {
}
function GetCameraTargetPositionLoc() {
}
function GetCameraEyePositionX() {
}
function GetCameraEyePositionY() {
}
function GetCameraEyePositionZ() {
}
function GetCameraEyePositionLoc() {
}
function NewSoundEnvironment(environmentName) {
}
function CreateSound(fileName,looping,is3D,stopwhenoutofrange,fadeInRate,fadeOutRate,eaxSetting) {
}
function CreateSoundFilenameWithLabel(fileName,looping,is3D,stopwhenoutofrange,fadeInRate,fadeOutRate,SLKEntryName) {
}
function CreateSoundFromLabel(soundLabel,looping,is3D,stopwhenoutofrange,fadeInRate,fadeOutRate) {
}
function CreateMIDISound(soundLabel,fadeInRate,fadeOutRate) {
}
function SetSoundParamsFromLabel(soundHandle,soundLabel) {
}
function SetSoundDistanceCutoff(soundHandle,cutoff) {
}
function SetSoundChannel(soundHandle,channel) {
}
function SetSoundVolume(soundHandle,volume) {
}
function SetSoundPitch(soundHandle,pitch) {
}
function SetSoundPlayPosition(soundHandle,millisecs) {
}
function SetSoundDistances(soundHandle,minDist,maxDist) {
}
function SetSoundConeAngles(soundHandle,inside,outside,outsideVolume) {
}
function SetSoundConeOrientation(soundHandle,x,y,z) {
}
function SetSoundPosition(soundHandle,x,y,z) {
}
function SetSoundVelocity(soundHandle,x,y,z) {
}
function AttachSoundToUnit(soundHandle,whichUnit) {
}
function StartSound(soundHandle) {
}
function StopSound(soundHandle,killWhenDone,fadeOut) {
}
function KillSoundWhenDone(soundHandle) {
}
function SetMapMusic(musicName,random,index) {
}
function ClearMapMusic() {
}
function PlayMusic(musicName) {
}
function PlayMusicEx(musicName,frommsecs,fadeinmsecs) {
}
function StopMusic(fadeOut) {
}
function ResumeMusic() {
}
function PlayThematicMusic(musicFileName) {
}
function PlayThematicMusicEx(musicFileName,frommsecs) {
}
function EndThematicMusic() {
}
function SetMusicVolume(volume) {
}
function SetMusicPlayPosition(millisecs) {
}
function SetThematicMusicVolume(volume) {
}
function SetThematicMusicPlayPosition(millisecs) {
}
function SetSoundDuration(soundHandle,duration) {
}
function GetSoundDuration(soundHandle) {
}
function GetSoundFileDuration(musicFileName) {
}
function VolumeGroupSetVolume(vgroup,scale) {
}
function VolumeGroupReset() {
}
function GetSoundIsPlaying(soundHandle) {
}
function GetSoundIsLoading(soundHandle) {
}
function RegisterStackedSound(soundHandle,byPosition,rectwidth,rectheight) {
}
function UnregisterStackedSound(soundHandle,byPosition,rectwidth,rectheight) {
}
function SetSoundFacialAnimationLabel(soundHandle,animationLabel) {
}
function SetSoundFacialAnimationGroupLabel(soundHandle,groupLabel) {
}
function SetSoundFacialAnimationSetFilepath(soundHandle,animationSetFilepath) {
}
function SetDialogueSpeakerNameKey(soundHandle,speakerName) {
}
function GetDialogueSpeakerNameKey(soundHandle) {
}
function SetDialogueTextKey(soundHandle,dialogueText) {
}
function GetDialogueTextKey(soundHandle) {
}
function AddWeatherEffect(where,effectID) {
}
function RemoveWeatherEffect(whichEffect) {
}
function EnableWeatherEffect(whichEffect,enable) {
}
function TerrainDeformCrater(x,y,radius,depth,duration,permanent) {
}
function TerrainDeformRipple(x,y,radius,depth,duration,count,spaceWaves,timeWaves,radiusStartPct,limitNeg) {
}
function TerrainDeformWave(x,y,dirX,dirY,distance,speed,radius,depth,trailTime,count) {
}
function TerrainDeformRandom(x,y,radius,minDelta,maxDelta,duration,updateInterval) {
}
function TerrainDeformStop(deformation,duration) {
}
function TerrainDeformStopAll() {
}
function AddSpecialEffect(modelName,x,y) {
}
function AddSpecialEffectLoc(modelName,where) {
}
function AddSpecialEffectTarget(modelName,targetWidget,attachPointName) {
}
function DestroyEffect(whichEffect) {
}
function AddSpellEffect(abilityString,t,x,y) {
}
function AddSpellEffectLoc(abilityString,t,where) {
}
function AddSpellEffectById(abilityId,t,x,y) {
}
function AddSpellEffectByIdLoc(abilityId,t,where) {
}
function AddSpellEffectTarget(modelName,t,targetWidget,attachPoint) {
}
function AddSpellEffectTargetById(abilityId,t,targetWidget,attachPoint) {
}
function AddLightning(codeName,checkVisibility,x1,y1,x2,y2) {
}
function AddLightningEx(codeName,checkVisibility,x1,y1,z1,x2,y2,z2) {
}
function DestroyLightning(whichBolt) {
}
function MoveLightning(whichBolt,checkVisibility,x1,y1,x2,y2) {
}
function MoveLightningEx(whichBolt,checkVisibility,x1,y1,z1,x2,y2,z2) {
}
function GetLightningColorA(whichBolt) {
}
function GetLightningColorR(whichBolt) {
}
function GetLightningColorG(whichBolt) {
}
function GetLightningColorB(whichBolt) {
}
function SetLightningColor(whichBolt,r,g,b,a) {
}
function GetAbilityEffect(abilityString,t,index) {
}
function GetAbilityEffectById(abilityId,t,index) {
}
function GetAbilitySound(abilityString,t) {
}
function GetAbilitySoundById(abilityId,t) {
}
function GetTerrainCliffLevel(x,y) {
}
function SetWaterBaseColor(red,green,blue,alpha) {
}
function SetWaterDeforms(val) {
}
function GetTerrainType(x,y) {
}
function GetTerrainVariance(x,y) {
}
function SetTerrainType(x,y,terrainType,variation,area,shape) {
}
function IsTerrainPathable(x,y,t) {
}
function SetTerrainPathable(x,y,t,flag) {
}
function CreateImage(file,sizeX,sizeY,sizeZ,posX,posY,posZ,originX,originY,originZ,imageType) {
}
function DestroyImage(whichImage) {
}
function ShowImage(whichImage,flag) {
}
function SetImageConstantHeight(whichImage,flag,height) {
}
function SetImagePosition(whichImage,x,y,z) {
}
function SetImageColor(whichImage,red,green,blue,alpha) {
}
function SetImageRender(whichImage,flag) {
}
function SetImageRenderAlways(whichImage,flag) {
}
function SetImageAboveWater(whichImage,flag,useWaterAlpha) {
}
function SetImageType(whichImage,imageType) {
}
function CreateUbersplat(x,y,name,red,green,blue,alpha,forcePaused,noBirthTime) {
}
function DestroyUbersplat(whichSplat) {
}
function ResetUbersplat(whichSplat) {
}
function FinishUbersplat(whichSplat) {
}
function ShowUbersplat(whichSplat,flag) {
}
function SetUbersplatRender(whichSplat,flag) {
}
function SetUbersplatRenderAlways(whichSplat,flag) {
}
function SetBlight(whichPlayer,x,y,radius,addBlight) {
}
function SetBlightRect(whichPlayer,r,addBlight) {
}
function SetBlightPoint(whichPlayer,x,y,addBlight) {
}
function SetBlightLoc(whichPlayer,whichLocation,radius,addBlight) {
}
function CreateBlightedGoldmine(id,x,y,face) {
}
function IsPointBlighted(x,y) {
}
function SetDoodadAnimation(x,y,radius,doodadID,nearestOnly,animName,animRandom) {
}
function SetDoodadAnimationRect(r,doodadID,animName,animRandom) {
}
function StartMeleeAI(num,script) {
}
function StartCampaignAI(num,script) {
}
function CommandAI(num,command,data) {
}
function PauseCompAI(p,pause) {
}
function GetAIDifficulty(num) {
}
function RemoveGuardPosition(hUnit) {
}
function RecycleGuardPosition(hUnit) {
}
function RemoveAllGuardPositions(num) {
}
function Cheat(cheatStr) {
}
function IsNoVictoryCheat() {
}
function IsNoDefeatCheat() {
}
function Preload(filename) {
}
function PreloadEnd(timeout) {
}
function PreloadStart() {
}
function PreloadRefresh() {
}
function PreloadEndEx() {
}
function PreloadGenClear() {
}
function PreloadGenStart() {
}
function PreloadGenEnd(filename) {
}
function Preloader(filename) {
}
function BlzHideCinematicPanels(enable) {
}
function AutomationSetTestType(testType) {
}
function AutomationTestStart(testName) {
}
function AutomationTestEnd() {
}
function AutomationTestingFinished() {
}
function BlzGetTriggerPlayerMouseX() {
}
function BlzGetTriggerPlayerMouseY() {
}
function BlzGetTriggerPlayerMousePosition() {
}
function BlzGetTriggerPlayerMouseButton() {
}
function BlzSetAbilityTooltip(abilCode,tooltip,level) {
}
function BlzSetAbilityActivatedTooltip(abilCode,tooltip,level) {
}
function BlzSetAbilityExtendedTooltip(abilCode,extendedTooltip,level) {
}
function BlzSetAbilityActivatedExtendedTooltip(abilCode,extendedTooltip,level) {
}
function BlzSetAbilityResearchTooltip(abilCode,researchTooltip,level) {
}
function BlzSetAbilityResearchExtendedTooltip(abilCode,researchExtendedTooltip,level) {
}
function BlzGetAbilityTooltip(abilCode,level) {
}
function BlzGetAbilityActivatedTooltip(abilCode,level) {
}
function BlzGetAbilityExtendedTooltip(abilCode,level) {
}
function BlzGetAbilityActivatedExtendedTooltip(abilCode,level) {
}
function BlzGetAbilityResearchTooltip(abilCode,level) {
}
function BlzGetAbilityResearchExtendedTooltip(abilCode,level) {
}
function BlzSetAbilityIcon(abilCode,iconPath) {
}
function BlzGetAbilityIcon(abilCode) {
}
function BlzSetAbilityActivatedIcon(abilCode,iconPath) {
}
function BlzGetAbilityActivatedIcon(abilCode) {
}
function BlzGetAbilityPosX(abilCode) {
}
function BlzGetAbilityPosY(abilCode) {
}
function BlzSetAbilityPosX(abilCode,x) {
}
function BlzSetAbilityPosY(abilCode,y) {
}
function BlzGetAbilityActivatedPosX(abilCode) {
}
function BlzGetAbilityActivatedPosY(abilCode) {
}
function BlzSetAbilityActivatedPosX(abilCode,x) {
}
function BlzSetAbilityActivatedPosY(abilCode,y) {
}
function BlzGetUnitMaxHP(whichUnit) {
}
function BlzSetUnitMaxHP(whichUnit,hp) {
}
function BlzGetUnitMaxMana(whichUnit) {
}
function BlzSetUnitMaxMana(whichUnit,mana) {
}
function BlzSetItemName(whichItem,name) {
}
function BlzSetItemDescription(whichItem,description) {
}
function BlzGetItemDescription(whichItem) {
}
function BlzSetItemTooltip(whichItem,tooltip) {
}
function BlzGetItemTooltip(whichItem) {
}
function BlzSetItemExtendedTooltip(whichItem,extendedTooltip) {
}
function BlzGetItemExtendedTooltip(whichItem) {
}
function BlzSetItemIconPath(whichItem,iconPath) {
}
function BlzGetItemIconPath(whichItem) {
}
function BlzSetUnitName(whichUnit,name) {
}
function BlzSetHeroProperName(whichUnit,heroProperName) {
}
function BlzGetUnitBaseDamage(whichUnit,weaponIndex) {
}
function BlzSetUnitBaseDamage(whichUnit,baseDamage,weaponIndex) {
}
function BlzGetUnitDiceNumber(whichUnit,weaponIndex) {
}
function BlzSetUnitDiceNumber(whichUnit,diceNumber,weaponIndex) {
}
function BlzGetUnitDiceSides(whichUnit,weaponIndex) {
}
function BlzSetUnitDiceSides(whichUnit,diceSides,weaponIndex) {
}
function BlzGetUnitAttackCooldown(whichUnit,weaponIndex) {
}
function BlzSetUnitAttackCooldown(whichUnit,cooldown,weaponIndex) {
}
function BlzSetSpecialEffectColorByPlayer(whichEffect,whichPlayer) {
}
function BlzSetSpecialEffectColor(whichEffect,r,g,b) {
}
function BlzSetSpecialEffectAlpha(whichEffect,alpha) {
}
function BlzSetSpecialEffectScale(whichEffect,scale) {
}
function BlzSetSpecialEffectPosition(whichEffect,x,y,z) {
}
function BlzSetSpecialEffectHeight(whichEffect,height) {
}
function BlzSetSpecialEffectTimeScale(whichEffect,timeScale) {
}
function BlzSetSpecialEffectTime(whichEffect,time) {
}
function BlzSetSpecialEffectOrientation(whichEffect,yaw,pitch,roll) {
}
function BlzSetSpecialEffectYaw(whichEffect,yaw) {
}
function BlzSetSpecialEffectPitch(whichEffect,pitch) {
}
function BlzSetSpecialEffectRoll(whichEffect,roll) {
}
function BlzSetSpecialEffectX(whichEffect,x) {
}
function BlzSetSpecialEffectY(whichEffect,y) {
}
function BlzSetSpecialEffectZ(whichEffect,z) {
}
function BlzSetSpecialEffectPositionLoc(whichEffect,loc) {
}
function BlzGetLocalSpecialEffectX(whichEffect) {
}
function BlzGetLocalSpecialEffectY(whichEffect) {
}
function BlzGetLocalSpecialEffectZ(whichEffect) {
}
function BlzSpecialEffectClearSubAnimations(whichEffect) {
}
function BlzSpecialEffectRemoveSubAnimation(whichEffect,whichSubAnim) {
}
function BlzSpecialEffectAddSubAnimation(whichEffect,whichSubAnim) {
}
function BlzPlaySpecialEffect(whichEffect,whichAnim) {
}
function BlzPlaySpecialEffectWithTimeScale(whichEffect,whichAnim,timeScale) {
}
function BlzGetAnimName(whichAnim) {
}
function BlzGetUnitArmor(whichUnit) {
}
function BlzSetUnitArmor(whichUnit,armorAmount) {
}
function BlzUnitHideAbility(whichUnit,abilId,flag) {
}
function BlzUnitDisableAbility(whichUnit,abilId,flag,hideUI) {
}
function BlzUnitCancelTimedLife(whichUnit) {
}
function BlzIsUnitSelectable(whichUnit) {
}
function BlzIsUnitInvulnerable(whichUnit) {
}
function BlzUnitInterruptAttack(whichUnit) {
}
function BlzGetUnitCollisionSize(whichUnit) {
}
function BlzGetAbilityManaCost(abilId,level) {
}
function BlzGetAbilityCooldown(abilId,level) {
}
function BlzSetUnitAbilityCooldown(whichUnit,abilId,level,cooldown) {
}
function BlzGetUnitAbilityCooldown(whichUnit,abilId,level) {
}
function BlzGetUnitAbilityCooldownRemaining(whichUnit,abilId) {
}
function BlzEndUnitAbilityCooldown(whichUnit,abilCode) {
}
function BlzStartUnitAbilityCooldown(whichUnit,abilCode,cooldown) {
}
function BlzGetUnitAbilityManaCost(whichUnit,abilId,level) {
}
function BlzSetUnitAbilityManaCost(whichUnit,abilId,level,manaCost) {
}
function BlzGetLocalUnitZ(whichUnit) {
}
function BlzDecPlayerTechResearched(whichPlayer,techid,levels) {
}
function BlzSetEventDamage(damage) {
}
function BlzGetEventDamageTarget() {
}
function BlzGetEventAttackType() {
}
function BlzGetEventDamageType() {
}
function BlzGetEventWeaponType() {
}
function BlzSetEventAttackType(attackType) {
}
function BlzSetEventDamageType(damageType) {
}
function BlzSetEventWeaponType(weaponType) {
}
function BlzGetEventIsAttack() {
}
function RequestExtraIntegerData(dataType,whichPlayer,param1,param2,param3,param4,param5,param6) {
}
function RequestExtraBooleanData(dataType,whichPlayer,param1,param2,param3,param4,param5,param6) {
}
function RequestExtraStringData(dataType,whichPlayer,param1,param2,param3,param4,param5,param6) {
}
function RequestExtraRealData(dataType,whichPlayer,param1,param2,param3,param4,param5,param6) {
}
function BlzGetUnitZ(whichUnit) {
}
function BlzEnableSelections(enableSelection,enableSelectionCircle) {
}
function BlzIsSelectionEnabled() {
}
function BlzIsSelectionCircleEnabled() {
}
function BlzCameraSetupApplyForceDurationSmooth(whichSetup,doPan,forcedDuration,easeInDuration,easeOutDuration,smoothFactor) {
}
function BlzEnableTargetIndicator(enable) {
}
function BlzIsTargetIndicatorEnabled() {
}
function BlzShowTerrain(show) {
}
function BlzShowSkyBox(show) {
}
function BlzStartRecording(fps) {
}
function BlzEndRecording() {
}
function BlzShowUnitTeamGlow(whichUnit,show) {
}
function BlzGetOriginFrame(frameType,index) {
}
function BlzEnableUIAutoPosition(enable) {
}
function BlzHideOriginFrames(enable) {
}
function BlzConvertColor(a,r,g,b) {
}
function BlzLoadTOCFile(TOCFile) {
}
function BlzCreateFrame(name,owner,priority,createContext) {
}
function BlzCreateSimpleFrame(name,owner,createContext) {
}
function BlzCreateFrameByType(typeName,name,owner,inherits,createContext) {
}
function BlzDestroyFrame(frame) {
}
function BlzFrameSetPoint(frame,point,relative,relativePoint,x,y) {
}
function BlzFrameSetAbsPoint(frame,point,x,y) {
}
function BlzFrameClearAllPoints(frame) {
}
function BlzFrameSetAllPoints(frame,relative) {
}
function BlzFrameSetVisible(frame,visible) {
}
function BlzFrameIsVisible(frame) {
}
function BlzGetFrameByName(name,createContext) {
}
function BlzFrameGetName(frame) {
}
function BlzFrameClick(frame) {
}
function BlzFrameSetText(frame,text) {
}
function BlzFrameGetText(frame) {
}
function BlzFrameAddText(frame,text) {
}
function BlzFrameSetTextSizeLimit(frame,size) {
}
function BlzFrameGetTextSizeLimit(frame) {
}
function BlzFrameSetTextColor(frame,color) {
}
function BlzFrameSetFocus(frame,flag) {
}
function BlzFrameSetModel(frame,modelFile,cameraIndex) {
}
function BlzFrameSetEnable(frame,enabled) {
}
function BlzFrameGetEnable(frame) {
}
function BlzFrameSetAlpha(frame,alpha) {
}
function BlzFrameGetAlpha(frame) {
}
function BlzFrameSetSpriteAnimate(frame,primaryProp,flags) {
}
function BlzFrameSetTexture(frame,texFile,flag,blend) {
}
function BlzFrameSetScale(frame,scale) {
}
function BlzFrameSetTooltip(frame,tooltip) {
}
function BlzFrameCageMouse(frame,enable) {
}
function BlzFrameSetValue(frame,value) {
}
function BlzFrameGetValue(frame) {
}
function BlzFrameSetMinMaxValue(frame,minValue,maxValue) {
}
function BlzFrameSetStepSize(frame,stepSize) {
}
function BlzFrameSetSize(frame,width,height) {
}
function BlzFrameSetVertexColor(frame,color) {
}
function BlzFrameSetLevel(frame,level) {
}
function BlzFrameSetParent(frame,parent) {
}
function BlzFrameGetParent(frame) {
}
function BlzFrameGetHeight(frame) {
}
function BlzFrameGetWidth(frame) {
}
function BlzFrameSetFont(frame,fileName,height,flags) {
}
function BlzFrameSetTextAlignment(frame,vert,horz) {
}
function BlzTriggerRegisterFrameEvent(whichTrigger,frame,eventId) {
}
function BlzGetTriggerFrame() {
}
function BlzGetTriggerFrameEvent() {
}
function BlzGetTriggerFrameValue() {
}
function BlzGetTriggerFrameText() {
}
function BlzTriggerRegisterPlayerSyncEvent(whichTrigger,whichPlayer,prefix,fromServer) {
}
function BlzSendSyncData(prefix,data) {
}
function BlzGetTriggerSyncPrefix() {
}
function BlzGetTriggerSyncData() {
}
function BlzTriggerRegisterPlayerKeyEvent(whichTrigger,whichPlayer,key,metaKey,keyDown) {
}
function BlzGetTriggerPlayerKey() {
}
function BlzGetTriggerPlayerMetaKey() {
}
function BlzGetTriggerPlayerIsKeyDown() {
}
function BlzEnableCursor(enable) {
}
function BlzSetMousePos(x,y) {
}
function BlzGetLocalClientWidth() {
}
function BlzGetLocalClientHeight() {
}
function BlzIsLocalClientActive() {
}
function BlzGetMouseFocusUnit() {
}
function BlzChangeMinimapTerrainTex(texFile) {
}
function BlzGetLocale() {
}
function BlzGetSpecialEffectScale(whichEffect) {
}
function BlzSetSpecialEffectMatrixScale(whichEffect,x,y,z) {
}
function BlzResetSpecialEffectMatrix(whichEffect) {
}
function BlzGetUnitAbility(whichUnit,abilId) {
}
function BlzGetUnitAbilityByIndex(whichUnit,index) {
}
function BlzDisplayChatMessage(whichPlayer,recipient,message) {
}
function BlzPauseUnitEx(whichUnit,flag) {
}
function BlzSetUnitFacingEx(whichUnit,facingAngle) {
}
function CreateCommandButtonEffect(abilityId,order) {
}
function CreateUpgradeCommandButtonEffect(whichUprgade) {
}
function CreateLearnCommandButtonEffect(abilityId) {
}
function DestroyCommandButtonEffect(whichEffect) {
}
function BlzBitOr(x,y) {
}
function BlzBitAnd(x,y) {
}
function BlzBitXor(x,y) {
}
function BlzGetAbilityBooleanField(whichAbility,whichField) {
}
function BlzGetAbilityIntegerField(whichAbility,whichField) {
}
function BlzGetAbilityRealField(whichAbility,whichField) {
}
function BlzGetAbilityStringField(whichAbility,whichField) {
}
function BlzGetAbilityBooleanLevelField(whichAbility,whichField,level) {
}
function BlzGetAbilityIntegerLevelField(whichAbility,whichField,level) {
}
function BlzGetAbilityRealLevelField(whichAbility,whichField,level) {
}
function BlzGetAbilityStringLevelField(whichAbility,whichField,level) {
}
function BlzGetAbilityBooleanLevelArrayField(whichAbility,whichField,level,index) {
}
function BlzGetAbilityIntegerLevelArrayField(whichAbility,whichField,level,index) {
}
function BlzGetAbilityRealLevelArrayField(whichAbility,whichField,level,index) {
}
function BlzGetAbilityStringLevelArrayField(whichAbility,whichField,level,index) {
}
function BlzSetAbilityBooleanField(whichAbility,whichField,value) {
}
function BlzSetAbilityIntegerField(whichAbility,whichField,value) {
}
function BlzSetAbilityRealField(whichAbility,whichField,value) {
}
function BlzSetAbilityStringField(whichAbility,whichField,value) {
}
function BlzSetAbilityBooleanLevelField(whichAbility,whichField,level,value) {
}
function BlzSetAbilityIntegerLevelField(whichAbility,whichField,level,value) {
}
function BlzSetAbilityRealLevelField(whichAbility,whichField,level,value) {
}
function BlzSetAbilityStringLevelField(whichAbility,whichField,level,value) {
}
function BlzSetAbilityBooleanLevelArrayField(whichAbility,whichField,level,index,value) {
}
function BlzSetAbilityIntegerLevelArrayField(whichAbility,whichField,level,index,value) {
}
function BlzSetAbilityRealLevelArrayField(whichAbility,whichField,level,index,value) {
}
function BlzSetAbilityStringLevelArrayField(whichAbility,whichField,level,index,value) {
}
function BlzAddAbilityBooleanLevelArrayField(whichAbility,whichField,level,value) {
}
function BlzAddAbilityIntegerLevelArrayField(whichAbility,whichField,level,value) {
}
function BlzAddAbilityRealLevelArrayField(whichAbility,whichField,level,value) {
}
function BlzAddAbilityStringLevelArrayField(whichAbility,whichField,level,value) {
}
function BlzRemoveAbilityBooleanLevelArrayField(whichAbility,whichField,level,value) {
}
function BlzRemoveAbilityIntegerLevelArrayField(whichAbility,whichField,level,value) {
}
function BlzRemoveAbilityRealLevelArrayField(whichAbility,whichField,level,value) {
}
function BlzRemoveAbilityStringLevelArrayField(whichAbility,whichField,level,value) {
}
function BlzGetItemAbilityByIndex(whichItem,index) {
}
function BlzGetItemAbility(whichItem,abilCode) {
}
function BlzItemAddAbility(whichItem,abilCode) {
}
function BlzGetItemBooleanField(whichItem,whichField) {
}
function BlzGetItemIntegerField(whichItem,whichField) {
}
function BlzGetItemRealField(whichItem,whichField) {
}
function BlzGetItemStringField(whichItem,whichField) {
}
function BlzSetItemBooleanField(whichItem,whichField,value) {
}
function BlzSetItemIntegerField(whichItem,whichField,value) {
}
function BlzSetItemRealField(whichItem,whichField,value) {
}
function BlzSetItemStringField(whichItem,whichField,value) {
}
function BlzItemRemoveAbility(whichItem,abilCode) {
}
function BlzGetUnitBooleanField(whichUnit,whichField) {
}
function BlzGetUnitIntegerField(whichUnit,whichField) {
}
function BlzGetUnitRealField(whichUnit,whichField) {
}
function BlzGetUnitStringField(whichUnit,whichField) {
}
function BlzSetUnitBooleanField(whichUnit,whichField,value) {
}
function BlzSetUnitIntegerField(whichUnit,whichField,value) {
}
function BlzSetUnitRealField(whichUnit,whichField,value) {
}
function BlzSetUnitStringField(whichUnit,whichField,value) {
}
function BlzGetUnitWeaponBooleanField(whichUnit,whichField,index) {
}
function BlzGetUnitWeaponIntegerField(whichUnit,whichField,index) {
}
function BlzGetUnitWeaponRealField(whichUnit,whichField,index) {
}
function BlzGetUnitWeaponStringField(whichUnit,whichField,index) {
}
function BlzSetUnitWeaponBooleanField(whichUnit,whichField,index,value) {
}
function BlzSetUnitWeaponIntegerField(whichUnit,whichField,index,value) {
}
function BlzSetUnitWeaponRealField(whichUnit,whichField,index,value) {
}
function BlzSetUnitWeaponStringField(whichUnit,whichField,index,value) {
}
function BlzGetUnitSkin(whichUnit) {
}
function BlzGetItemSkin(whichItem) {
}
function BlzSetUnitSkin(whichUnit,skinId) {
}
function BlzSetItemSkin(whichItem,skinId) {
}
function BlzCreateItemWithSkin(itemid,x,y,skinId) {
}
function BlzCreateUnitWithSkin(id,unitid,x,y,face,skinId) {
}
function BlzCreateDestructableWithSkin(objectid,x,y,face,scale,variation,skinId) {
}
function BlzCreateDestructableZWithSkin(objectid,x,y,z,face,scale,variation,skinId) {
}
function BlzCreateDeadDestructableWithSkin(objectid,x,y,face,scale,variation,skinId) {
}
function BlzCreateDeadDestructableZWithSkin(objectid,x,y,z,face,scale,variation,skinId) {
}
function BlzGetPlayerTownHallCount(whichPlayer) {
}
