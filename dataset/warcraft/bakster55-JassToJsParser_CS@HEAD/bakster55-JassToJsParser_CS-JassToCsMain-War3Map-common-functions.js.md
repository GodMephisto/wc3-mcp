---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/War3Map/common-functions.js"
ref: HEAD
---

(function () {
    if (typeof Object.__id == "undefined") {
        var __id = 0;

        Object.__id = function (o) {
            if (typeof o.__uniqueid == "undefined") {
                Object.defineProperty(o, "__uniqueid", {
                    value: ++__id,
                    enumerable: false,
                    // This could go either way, depending on your 
                    // interpretation of what an "id" is
                    writable: false
                });
            }

            return o.__uniqueid;
        };
    }
})();

var needHeroXP = window["War3mapMisc"].NeedHeroXP || 200;
var xpTableInternal = needHeroXP.toString().split(",").map(e => Number(e));
xpTableInternal.unshift(0);
xpTableInternal.unshift(0);
(function () {
    var a = window["War3mapMisc"].NeedHeroXPFormulaA || 1;
    var b = window["War3mapMisc"].NeedHeroXPFormulaB || 100;
    var c = window["War3mapMisc"].NeedHeroXPFormulaC || 0;
    var maxHeroLevel = window["War3mapMisc"].MaxHeroLevel || 0;

    for (var i = 2; i <= maxHeroLevel; i++) {
        if (xpTableInternal[i] == undefined) {
            xpTableInternal[i] = a * xpTableInternal[i - 1] + b * i + c
        }
    }
})();

Array.prototype.getInt = function (i) {
    var element = this[i];

    if (element == undefined) { return 0; }

    return element;
};

var player = new PlayerInternal(1);
player.name = "Baks#2542";
player.nameHash = 1350;
player.loadCode = `-load unsbmWdrGF9bkmbkmbt4dEDDSwbkJbkNbkwbkubmE0bmEn8`;
player.state = {};
player.playerSlotState = ConvertPlayerSlotState(1);


var players = [
    player
]
var units = [];
var groups = [];
var currentEnumUnit;
var currentEnumPlayer;

function PlayerInternal(id) {
    this.id = id,
        this.units = [],
        this.state = {}
}

function Unit(player, unitId, location) {
    this.player = player;
    this.xp = 0;
    this.str = 0;
    this.agi = 0;
    this.int = 0;
    this.unitId = unitId || 0;
    this.lvl = 0;
    this.items = [];
    this.abilities = [];
    this.location = location;
}

function Group() {
    this.units = []
}

function LocationInternal(x, y) {
    this.x = x;
    this.y = y;
}

function AbilityInternal(code, level) {
    this.code = code;
    this.level = level;
}

function ItemInternal(itemId) {
    this.itemId = itemId;
}


function ConvertRace(i) {
    return i;
}
function ConvertAllianceType(i) {
    return i;
}
function ConvertRacePref(i) {
    return i;
}
function ConvertIGameState(i) {
    return i;
}
function ConvertFGameState(i) {
    return i;
}
function ConvertPlayerState(i) {
    return i;
}
function ConvertPlayerScore(i) {
    return i;
}
function ConvertPlayerGameResult(i) {
    return i;
}
function ConvertUnitState(i) {
    return i;
}
function ConvertAIDifficulty(i) {
    return i;
}
function ConvertGameEvent(i) {
    return i;
}
function ConvertPlayerEvent(i) {
    return i;
}
function ConvertPlayerUnitEvent(i) {
    return i;
}
function ConvertWidgetEvent(i) {
    return i;
}
function ConvertDialogEvent(i) {
    return i;
}
function ConvertUnitEvent(i) {
    return i;
}
function ConvertLimitOp(i) {
    return i;
}
function ConvertUnitType(i) {
    return i;
}
function ConvertGameSpeed(i) {
    return i;
}
function ConvertPlacement(i) {
    return i;
}
function ConvertStartLocPrio(i) {
    return i;
}
function ConvertGameDifficulty(i) {
    return i;
}
function ConvertGameType(i) {
    return i;
}
function ConvertMapFlag(i) {
    return i;
}
function ConvertMapVisibility(i) {
    return i;
}
function ConvertMapSetting(i) {
    return i;
}
function ConvertMapDensity(i) {
    return i;
}
function ConvertMapControl(i) {
    return i;
}
function ConvertPlayerColor(i) {
    return i;
}
function ConvertPlayerSlotState(i) {
    return i;
}
function ConvertVolumeGroup(i) {
    return i;
}
function ConvertCameraField(i) {
    return i;
}
function ConvertBlendMode(i) {
    return i;
}
function ConvertRarityControl(i) {
    return i;
}
function ConvertTexMapFlags(i) {
    return i;
}
function ConvertFogState(i) {
    return i;
}
function ConvertEffectType(i) {
    return i;
}
function ConvertVersion(i) {
    return i;
}
function ConvertItemType(i) {
    return i;
}
function ConvertAttackType(i) {
    return i;
}
function ConvertDamageType(i) {
    return i;
}
function ConvertWeaponType(i) {
    return i;
}
function ConvertSoundType(i) {
    return i;
}
function ConvertPathingType(i) {
    return i;
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
    return players.length;
}
function GetBJPlayerNeutralVictim() {
}
function GetBJPlayerNeutralExtra() {
}
function GetBJMaxPlayerSlots() {
    return players.length;
}
function GetPlayerNeutralPassive() {
}
function GetPlayerNeutralAggressive() {
}


var FALSE = false
var TRUE = true
var JASS_MAX_ARRAY_SIZE = 8192

var PLAYER_NEUTRAL_PASSIVE = 15
var PLAYER_NEUTRAL_AGGRESSIVE = 12

var PLAYER_COLOR_RED = ConvertPlayerColor(0)
var PLAYER_COLOR_BLUE = ConvertPlayerColor(1)
var PLAYER_COLOR_CYAN = ConvertPlayerColor(2)
var PLAYER_COLOR_PURPLE = ConvertPlayerColor(3)
var PLAYER_COLOR_YELLOW = ConvertPlayerColor(4)
var PLAYER_COLOR_ORANGE = ConvertPlayerColor(5)
var PLAYER_COLOR_GREEN = ConvertPlayerColor(6)
var PLAYER_COLOR_PINK = ConvertPlayerColor(7)
var PLAYER_COLOR_LIGHT_GRAY = ConvertPlayerColor(8)
var PLAYER_COLOR_LIGHT_BLUE = ConvertPlayerColor(9)
var PLAYER_COLOR_AQUA = ConvertPlayerColor(10)
var PLAYER_COLOR_BROWN = ConvertPlayerColor(11)

var RACE_HUMAN = ConvertRace(1)
var RACE_ORC = ConvertRace(2)
var RACE_UNDEAD = ConvertRace(3)
var RACE_NIGHTELF = ConvertRace(4)
var RACE_DEMON = ConvertRace(5)
var RACE_OTHER = ConvertRace(7)

var PLAYER_GAME_RESULT_VICTORY = ConvertPlayerGameResult(0)
var PLAYER_GAME_RESULT_DEFEAT = ConvertPlayerGameResult(1)
var PLAYER_GAME_RESULT_TIE = ConvertPlayerGameResult(2)
var PLAYER_GAME_RESULT_NEUTRAL = ConvertPlayerGameResult(3)

var ALLIANCE_PASSIVE = ConvertAllianceType(0)
var ALLIANCE_HELP_REQUEST = ConvertAllianceType(1)
var ALLIANCE_HELP_RESPONSE = ConvertAllianceType(2)
var ALLIANCE_SHARED_XP = ConvertAllianceType(3)
var ALLIANCE_SHARED_SPELLS = ConvertAllianceType(4)
var ALLIANCE_SHARED_VISION = ConvertAllianceType(5)
var ALLIANCE_SHARED_CONTROL = ConvertAllianceType(6)
var ALLIANCE_SHARED_ADVANCED_CONTROL = ConvertAllianceType(7)
var ALLIANCE_RESCUABLE = ConvertAllianceType(8)
var ALLIANCE_SHARED_VISION_FORCED = ConvertAllianceType(9)

var VERSION_REIGN_OF_CHAOS = ConvertVersion(0)
var VERSION_FROZEN_THRONE = ConvertVersion(1)

var ATTACK_TYPE_NORMAL = ConvertAttackType(0)
var ATTACK_TYPE_MELEE = ConvertAttackType(1)
var ATTACK_TYPE_PIERCE = ConvertAttackType(2)
var ATTACK_TYPE_SIEGE = ConvertAttackType(3)
var ATTACK_TYPE_MAGIC = ConvertAttackType(4)
var ATTACK_TYPE_CHAOS = ConvertAttackType(5)
var ATTACK_TYPE_HERO = ConvertAttackType(6)

var DAMAGE_TYPE_UNKNOWN = ConvertDamageType(0)
var DAMAGE_TYPE_NORMAL = ConvertDamageType(4)
var DAMAGE_TYPE_ENHANCED = ConvertDamageType(5)
var DAMAGE_TYPE_FIRE = ConvertDamageType(8)
var DAMAGE_TYPE_COLD = ConvertDamageType(9)
var DAMAGE_TYPE_LIGHTNING = ConvertDamageType(10)
var DAMAGE_TYPE_POISON = ConvertDamageType(11)
var DAMAGE_TYPE_DISEASE = ConvertDamageType(12)
var DAMAGE_TYPE_DIVINE = ConvertDamageType(13)
var DAMAGE_TYPE_MAGIC = ConvertDamageType(14)
var DAMAGE_TYPE_SONIC = ConvertDamageType(15)
var DAMAGE_TYPE_ACID = ConvertDamageType(16)
var DAMAGE_TYPE_FORCE = ConvertDamageType(17)
var DAMAGE_TYPE_DEATH = ConvertDamageType(18)
var DAMAGE_TYPE_MIND = ConvertDamageType(19)
var DAMAGE_TYPE_PLANT = ConvertDamageType(20)
var DAMAGE_TYPE_DEFENSIVE = ConvertDamageType(21)
var DAMAGE_TYPE_DEMOLITION = ConvertDamageType(22)
var DAMAGE_TYPE_SLOW_POISON = ConvertDamageType(23)
var DAMAGE_TYPE_SPIRIT_LINK = ConvertDamageType(24)
var DAMAGE_TYPE_SHADOW_STRIKE = ConvertDamageType(25)
var DAMAGE_TYPE_UNIVERSAL = ConvertDamageType(26)

var WEAPON_TYPE_WHOKNOWS = ConvertWeaponType(0)
var WEAPON_TYPE_METAL_LIGHT_CHOP = ConvertWeaponType(1)
var WEAPON_TYPE_METAL_MEDIUM_CHOP = ConvertWeaponType(2)
var WEAPON_TYPE_METAL_HEAVY_CHOP = ConvertWeaponType(3)
var WEAPON_TYPE_METAL_LIGHT_SLICE = ConvertWeaponType(4)
var WEAPON_TYPE_METAL_MEDIUM_SLICE = ConvertWeaponType(5)
var WEAPON_TYPE_METAL_HEAVY_SLICE = ConvertWeaponType(6)
var WEAPON_TYPE_METAL_MEDIUM_BASH = ConvertWeaponType(7)
var WEAPON_TYPE_METAL_HEAVY_BASH = ConvertWeaponType(8)
var WEAPON_TYPE_METAL_MEDIUM_STAB = ConvertWeaponType(9)
var WEAPON_TYPE_METAL_HEAVY_STAB = ConvertWeaponType(10)
var WEAPON_TYPE_WOOD_LIGHT_SLICE = ConvertWeaponType(11)
var WEAPON_TYPE_WOOD_MEDIUM_SLICE = ConvertWeaponType(12)
var WEAPON_TYPE_WOOD_HEAVY_SLICE = ConvertWeaponType(13)
var WEAPON_TYPE_WOOD_LIGHT_BASH = ConvertWeaponType(14)
var WEAPON_TYPE_WOOD_MEDIUM_BASH = ConvertWeaponType(15)
var WEAPON_TYPE_WOOD_HEAVY_BASH = ConvertWeaponType(16)
var WEAPON_TYPE_WOOD_LIGHT_STAB = ConvertWeaponType(17)
var WEAPON_TYPE_WOOD_MEDIUM_STAB = ConvertWeaponType(18)
var WEAPON_TYPE_CLAW_LIGHT_SLICE = ConvertWeaponType(19)
var WEAPON_TYPE_CLAW_MEDIUM_SLICE = ConvertWeaponType(20)
var WEAPON_TYPE_CLAW_HEAVY_SLICE = ConvertWeaponType(21)
var WEAPON_TYPE_AXE_MEDIUM_CHOP = ConvertWeaponType(22)
var WEAPON_TYPE_ROCK_HEAVY_BASH = ConvertWeaponType(23)

var PATHING_TYPE_ANY = ConvertPathingType(0)
var PATHING_TYPE_WALKABILITY = ConvertPathingType(1)
var PATHING_TYPE_FLYABILITY = ConvertPathingType(2)
var PATHING_TYPE_BUILDABILITY = ConvertPathingType(3)
var PATHING_TYPE_PEONHARVESTPATHING = ConvertPathingType(4)
var PATHING_TYPE_BLIGHTPATHING = ConvertPathingType(5)
var PATHING_TYPE_FLOATABILITY = ConvertPathingType(6)
var PATHING_TYPE_AMPHIBIOUSPATHING = ConvertPathingType(7)





var RACE_PREF_HUMAN = ConvertRacePref(1)
var RACE_PREF_ORC = ConvertRacePref(2)
var RACE_PREF_NIGHTELF = ConvertRacePref(4)
var RACE_PREF_UNDEAD = ConvertRacePref(8)
var RACE_PREF_DEMON = ConvertRacePref(16)
var RACE_PREF_RANDOM = ConvertRacePref(32)
var RACE_PREF_USER_SELECTABLE = ConvertRacePref(64)

var MAP_CONTROL_USER = ConvertMapControl(0)
var MAP_CONTROL_COMPUTER = ConvertMapControl(1)
var MAP_CONTROL_RESCUABLE = ConvertMapControl(2)
var MAP_CONTROL_NEUTRAL = ConvertMapControl(3)
var MAP_CONTROL_CREEP = ConvertMapControl(4)
var MAP_CONTROL_NONE = ConvertMapControl(5)

var GAME_TYPE_MELEE = ConvertGameType(1)
var GAME_TYPE_FFA = ConvertGameType(2)
var GAME_TYPE_USE_MAP_SETTINGS = ConvertGameType(4)
var GAME_TYPE_BLIZ = ConvertGameType(8)
var GAME_TYPE_ONE_ON_ONE = ConvertGameType(16)
var GAME_TYPE_TWO_TEAM_PLAY = ConvertGameType(32)
var GAME_TYPE_THREE_TEAM_PLAY = ConvertGameType(64)
var GAME_TYPE_FOUR_TEAM_PLAY = ConvertGameType(128)

var MAP_FOG_HIDE_TERRAIN = ConvertMapFlag(1)
var MAP_FOG_MAP_EXPLORED = ConvertMapFlag(2)
var MAP_FOG_ALWAYS_VISIBLE = ConvertMapFlag(4)

var MAP_USE_HANDICAPS = ConvertMapFlag(8)
var MAP_OBSERVERS = ConvertMapFlag(16)
var MAP_OBSERVERS_ON_DEATH = ConvertMapFlag(32)

var MAP_FIXED_COLORS = ConvertMapFlag(128)

var MAP_LOCK_RESOURCE_TRADING = ConvertMapFlag(256)
var MAP_RESOURCE_TRADING_ALLIES_ONLY = ConvertMapFlag(512)

var MAP_LOCK_ALLIANCE_CHANGES = ConvertMapFlag(1024)
var MAP_ALLIANCE_CHANGES_HIDDEN = ConvertMapFlag(2048)

var MAP_CHEATS = ConvertMapFlag(4096)
var MAP_CHEATS_HIDDEN = ConvertMapFlag(8192)

var MAP_LOCK_SPEED = ConvertMapFlag(8192 * 2)
var MAP_LOCK_RANDOM_SEED = ConvertMapFlag(8192 * 4)
var MAP_SHARED_ADVANCED_CONTROL = ConvertMapFlag(8192 * 8)
var MAP_RANDOM_HERO = ConvertMapFlag(8192 * 16)
var MAP_RANDOM_RACES = ConvertMapFlag(8192 * 32)
var MAP_RELOADED = ConvertMapFlag(8192 * 64)

var MAP_PLACEMENT_RANDOM = ConvertPlacement(0)
var MAP_PLACEMENT_FIXED = ConvertPlacement(1)
var MAP_PLACEMENT_USE_MAP_SETTINGS = ConvertPlacement(2)
var MAP_PLACEMENT_TEAMS_TOGETHER = ConvertPlacement(3)

var MAP_LOC_PRIO_LOW = ConvertStartLocPrio(0)
var MAP_LOC_PRIO_HIGH = ConvertStartLocPrio(1)
var MAP_LOC_PRIO_NOT = ConvertStartLocPrio(2)

var MAP_DENSITY_NONE = ConvertMapDensity(0)
var MAP_DENSITY_LIGHT = ConvertMapDensity(1)
var MAP_DENSITY_MEDIUM = ConvertMapDensity(2)
var MAP_DENSITY_HEAVY = ConvertMapDensity(3)

var MAP_DIFFICULTY_EASY = ConvertGameDifficulty(0)
var MAP_DIFFICULTY_NORMAL = ConvertGameDifficulty(1)
var MAP_DIFFICULTY_HARD = ConvertGameDifficulty(2)
var MAP_DIFFICULTY_INSANE = ConvertGameDifficulty(3)

var MAP_SPEED_SLOWEST = ConvertGameSpeed(0)
var MAP_SPEED_SLOW = ConvertGameSpeed(1)
var MAP_SPEED_NORMAL = ConvertGameSpeed(2)
var MAP_SPEED_FAST = ConvertGameSpeed(3)
var MAP_SPEED_FASTEST = ConvertGameSpeed(4)

var PLAYER_SLOT_STATE_EMPTY = ConvertPlayerSlotState(0)
var PLAYER_SLOT_STATE_PLAYING = ConvertPlayerSlotState(1)
var PLAYER_SLOT_STATE_LEFT = ConvertPlayerSlotState(2)




var SOUND_VOLUMEGROUP_UNITMOVEMENT = ConvertVolumeGroup(0)
var SOUND_VOLUMEGROUP_UNITSOUNDS = ConvertVolumeGroup(1)
var SOUND_VOLUMEGROUP_COMBAT = ConvertVolumeGroup(2)
var SOUND_VOLUMEGROUP_SPELLS = ConvertVolumeGroup(3)
var SOUND_VOLUMEGROUP_UI = ConvertVolumeGroup(4)
var SOUND_VOLUMEGROUP_MUSIC = ConvertVolumeGroup(5)
var SOUND_VOLUMEGROUP_AMBIENTSOUNDS = ConvertVolumeGroup(6)
var SOUND_VOLUMEGROUP_FIRE = ConvertVolumeGroup(7)









var GAME_STATE_DIVINE_INTERVENTION = ConvertIGameState(0)
var GAME_STATE_DISCONNECTED = ConvertIGameState(1)
var GAME_STATE_TIME_OF_DAY = ConvertFGameState(2)

var PLAYER_STATE_GAME_RESULT = ConvertPlayerState(0)



var PLAYER_STATE_RESOURCE_GOLD = ConvertPlayerState(1)
var PLAYER_STATE_RESOURCE_LUMBER = ConvertPlayerState(2)
var PLAYER_STATE_RESOURCE_HERO_TOKENS = ConvertPlayerState(3)
var PLAYER_STATE_RESOURCE_FOOD_CAP = ConvertPlayerState(4)
var PLAYER_STATE_RESOURCE_FOOD_USED = ConvertPlayerState(5)
var PLAYER_STATE_FOOD_CAP_CEILING = ConvertPlayerState(6)

var PLAYER_STATE_GIVES_BOUNTY = ConvertPlayerState(7)
var PLAYER_STATE_ALLIED_VICTORY = ConvertPlayerState(8)
var PLAYER_STATE_PLACED = ConvertPlayerState(9)
var PLAYER_STATE_OBSERVER_ON_DEATH = ConvertPlayerState(10)
var PLAYER_STATE_OBSERVER = ConvertPlayerState(11)
var PLAYER_STATE_UNFOLLOWABLE = ConvertPlayerState(12)



var PLAYER_STATE_GOLD_UPKEEP_RATE = ConvertPlayerState(13)
var PLAYER_STATE_LUMBER_UPKEEP_RATE = ConvertPlayerState(14)



var PLAYER_STATE_GOLD_GATHERED = ConvertPlayerState(15)
var PLAYER_STATE_LUMBER_GATHERED = ConvertPlayerState(16)

var PLAYER_STATE_NO_CREEP_SLEEP = ConvertPlayerState(25)

var UNIT_STATE_LIFE = ConvertUnitState(0)
var UNIT_STATE_MAX_LIFE = ConvertUnitState(1)
var UNIT_STATE_MANA = ConvertUnitState(2)
var UNIT_STATE_MAX_MANA = ConvertUnitState(3)

var AI_DIFFICULTY_NEWBIE = ConvertAIDifficulty(0)
var AI_DIFFICULTY_NORMAL = ConvertAIDifficulty(1)
var AI_DIFFICULTY_INSANE = ConvertAIDifficulty(2)


var PLAYER_SCORE_UNITS_TRAINED = ConvertPlayerScore(0)
var PLAYER_SCORE_UNITS_KILLED = ConvertPlayerScore(1)
var PLAYER_SCORE_STRUCT_BUILT = ConvertPlayerScore(2)
var PLAYER_SCORE_STRUCT_RAZED = ConvertPlayerScore(3)
var PLAYER_SCORE_TECH_PERCENT = ConvertPlayerScore(4)
var PLAYER_SCORE_FOOD_MAXPROD = ConvertPlayerScore(5)
var PLAYER_SCORE_FOOD_MAXUSED = ConvertPlayerScore(6)
var PLAYER_SCORE_HEROES_KILLED = ConvertPlayerScore(7)
var PLAYER_SCORE_ITEMS_GAINED = ConvertPlayerScore(8)
var PLAYER_SCORE_MERCS_HIRED = ConvertPlayerScore(9)
var PLAYER_SCORE_GOLD_MINED_TOTAL = ConvertPlayerScore(10)
var PLAYER_SCORE_GOLD_MINED_UPKEEP = ConvertPlayerScore(11)
var PLAYER_SCORE_GOLD_LOST_UPKEEP = ConvertPlayerScore(12)
var PLAYER_SCORE_GOLD_LOST_TAX = ConvertPlayerScore(13)
var PLAYER_SCORE_GOLD_GIVEN = ConvertPlayerScore(14)
var PLAYER_SCORE_GOLD_RECEIVED = ConvertPlayerScore(15)
var PLAYER_SCORE_LUMBER_TOTAL = ConvertPlayerScore(16)
var PLAYER_SCORE_LUMBER_LOST_UPKEEP = ConvertPlayerScore(17)
var PLAYER_SCORE_LUMBER_LOST_TAX = ConvertPlayerScore(18)
var PLAYER_SCORE_LUMBER_GIVEN = ConvertPlayerScore(19)
var PLAYER_SCORE_LUMBER_RECEIVED = ConvertPlayerScore(20)
var PLAYER_SCORE_UNIT_TOTAL = ConvertPlayerScore(21)
var PLAYER_SCORE_HERO_TOTAL = ConvertPlayerScore(22)
var PLAYER_SCORE_RESOURCE_TOTAL = ConvertPlayerScore(23)
var PLAYER_SCORE_TOTAL = ConvertPlayerScore(24)




















var EVENT_GAME_VICTORY = ConvertGameEvent(0)
var EVENT_GAME_END_LEVEL = ConvertGameEvent(1)

var EVENT_GAME_VARIABLE_LIMIT = ConvertGameEvent(2)
var EVENT_GAME_STATE_LIMIT = ConvertGameEvent(3)

var EVENT_GAME_TIMER_EXPIRED = ConvertGameEvent(4)

var EVENT_GAME_ENTER_REGION = ConvertGameEvent(5)
var EVENT_GAME_LEAVE_REGION = ConvertGameEvent(6)

var EVENT_GAME_TRACKABLE_HIT = ConvertGameEvent(7)
var EVENT_GAME_TRACKABLE_TRACK = ConvertGameEvent(8)

var EVENT_GAME_SHOW_SKILL = ConvertGameEvent(9)
var EVENT_GAME_BUILD_SUBMENU = ConvertGameEvent(10)




var EVENT_PLAYER_STATE_LIMIT = ConvertPlayerEvent(11)
var EVENT_PLAYER_ALLIANCE_CHANGED = ConvertPlayerEvent(12)

var EVENT_PLAYER_DEFEAT = ConvertPlayerEvent(13)
var EVENT_PLAYER_VICTORY = ConvertPlayerEvent(14)
var EVENT_PLAYER_LEAVE = ConvertPlayerEvent(15)
var EVENT_PLAYER_CHAT = ConvertPlayerEvent(16)
var EVENT_PLAYER_END_CINEMATIC = ConvertPlayerEvent(17)





var EVENT_PLAYER_UNIT_ATTACKED = ConvertPlayerUnitEvent(18)
var EVENT_PLAYER_UNIT_RESCUED = ConvertPlayerUnitEvent(19)

var EVENT_PLAYER_UNIT_DEATH = ConvertPlayerUnitEvent(20)
var EVENT_PLAYER_UNIT_DECAY = ConvertPlayerUnitEvent(21)

var EVENT_PLAYER_UNIT_DETECTED = ConvertPlayerUnitEvent(22)
var EVENT_PLAYER_UNIT_HIDDEN = ConvertPlayerUnitEvent(23)

var EVENT_PLAYER_UNIT_SELECTED = ConvertPlayerUnitEvent(24)
var EVENT_PLAYER_UNIT_DESELECTED = ConvertPlayerUnitEvent(25)

var EVENT_PLAYER_UNIT_CONSTRUCT_START = ConvertPlayerUnitEvent(26)
var EVENT_PLAYER_UNIT_CONSTRUCT_CANCEL = ConvertPlayerUnitEvent(27)
var EVENT_PLAYER_UNIT_CONSTRUCT_FINISH = ConvertPlayerUnitEvent(28)

var EVENT_PLAYER_UNIT_UPGRADE_START = ConvertPlayerUnitEvent(29)
var EVENT_PLAYER_UNIT_UPGRADE_CANCEL = ConvertPlayerUnitEvent(30)
var EVENT_PLAYER_UNIT_UPGRADE_FINISH = ConvertPlayerUnitEvent(31)

var EVENT_PLAYER_UNIT_TRAIN_START = ConvertPlayerUnitEvent(32)
var EVENT_PLAYER_UNIT_TRAIN_CANCEL = ConvertPlayerUnitEvent(33)
var EVENT_PLAYER_UNIT_TRAIN_FINISH = ConvertPlayerUnitEvent(34)

var EVENT_PLAYER_UNIT_RESEARCH_START = ConvertPlayerUnitEvent(35)
var EVENT_PLAYER_UNIT_RESEARCH_CANCEL = ConvertPlayerUnitEvent(36)
var EVENT_PLAYER_UNIT_RESEARCH_FINISH = ConvertPlayerUnitEvent(37)
var EVENT_PLAYER_UNIT_ISSUED_ORDER = ConvertPlayerUnitEvent(38)
var EVENT_PLAYER_UNIT_ISSUED_POINT_ORDER = ConvertPlayerUnitEvent(39)
var EVENT_PLAYER_UNIT_ISSUED_TARGET_ORDER = ConvertPlayerUnitEvent(40)
var EVENT_PLAYER_UNIT_ISSUED_UNIT_ORDER = ConvertPlayerUnitEvent(40)

var EVENT_PLAYER_HERO_LEVEL = ConvertPlayerUnitEvent(41)
var EVENT_PLAYER_HERO_SKILL = ConvertPlayerUnitEvent(42)

var EVENT_PLAYER_HERO_REVIVABLE = ConvertPlayerUnitEvent(43)

var EVENT_PLAYER_HERO_REVIVE_START = ConvertPlayerUnitEvent(44)
var EVENT_PLAYER_HERO_REVIVE_CANCEL = ConvertPlayerUnitEvent(45)
var EVENT_PLAYER_HERO_REVIVE_FINISH = ConvertPlayerUnitEvent(46)
var EVENT_PLAYER_UNIT_SUMMON = ConvertPlayerUnitEvent(47)
var EVENT_PLAYER_UNIT_DROP_ITEM = ConvertPlayerUnitEvent(48)
var EVENT_PLAYER_UNIT_PICKUP_ITEM = ConvertPlayerUnitEvent(49)
var EVENT_PLAYER_UNIT_USE_ITEM = ConvertPlayerUnitEvent(50)

var EVENT_PLAYER_UNIT_LOADED = ConvertPlayerUnitEvent(51)





var EVENT_UNIT_DAMAGED = ConvertUnitEvent(52)
var EVENT_UNIT_DEATH = ConvertUnitEvent(53)
var EVENT_UNIT_DECAY = ConvertUnitEvent(54)
var EVENT_UNIT_DETECTED = ConvertUnitEvent(55)
var EVENT_UNIT_HIDDEN = ConvertUnitEvent(56)
var EVENT_UNIT_SELECTED = ConvertUnitEvent(57)
var EVENT_UNIT_DESELECTED = ConvertUnitEvent(58)

var EVENT_UNIT_STATE_LIMIT = ConvertUnitEvent(59)



var EVENT_UNIT_ACQUIRED_TARGET = ConvertUnitEvent(60)
var EVENT_UNIT_TARGET_IN_RANGE = ConvertUnitEvent(61)
var EVENT_UNIT_ATTACKED = ConvertUnitEvent(62)
var EVENT_UNIT_RESCUED = ConvertUnitEvent(63)

var EVENT_UNIT_CONSTRUCT_CANCEL = ConvertUnitEvent(64)
var EVENT_UNIT_CONSTRUCT_FINISH = ConvertUnitEvent(65)

var EVENT_UNIT_UPGRADE_START = ConvertUnitEvent(66)
var EVENT_UNIT_UPGRADE_CANCEL = ConvertUnitEvent(67)
var EVENT_UNIT_UPGRADE_FINISH = ConvertUnitEvent(68)




var EVENT_UNIT_TRAIN_START = ConvertUnitEvent(69)
var EVENT_UNIT_TRAIN_CANCEL = ConvertUnitEvent(70)
var EVENT_UNIT_TRAIN_FINISH = ConvertUnitEvent(71)

var EVENT_UNIT_RESEARCH_START = ConvertUnitEvent(72)
var EVENT_UNIT_RESEARCH_CANCEL = ConvertUnitEvent(73)
var EVENT_UNIT_RESEARCH_FINISH = ConvertUnitEvent(74)

var EVENT_UNIT_ISSUED_ORDER = ConvertUnitEvent(75)
var EVENT_UNIT_ISSUED_POINT_ORDER = ConvertUnitEvent(76)
var EVENT_UNIT_ISSUED_TARGET_ORDER = ConvertUnitEvent(77)

var EVENT_UNIT_HERO_LEVEL = ConvertUnitEvent(78)
var EVENT_UNIT_HERO_SKILL = ConvertUnitEvent(79)

var EVENT_UNIT_HERO_REVIVABLE = ConvertUnitEvent(80)
var EVENT_UNIT_HERO_REVIVE_START = ConvertUnitEvent(81)
var EVENT_UNIT_HERO_REVIVE_CANCEL = ConvertUnitEvent(82)
var EVENT_UNIT_HERO_REVIVE_FINISH = ConvertUnitEvent(83)

var EVENT_UNIT_SUMMON = ConvertUnitEvent(84)

var EVENT_UNIT_DROP_ITEM = ConvertUnitEvent(85)
var EVENT_UNIT_PICKUP_ITEM = ConvertUnitEvent(86)
var EVENT_UNIT_USE_ITEM = ConvertUnitEvent(87)

var EVENT_UNIT_LOADED = ConvertUnitEvent(88)

var EVENT_WIDGET_DEATH = ConvertWidgetEvent(89)

var EVENT_DIALOG_BUTTON_CLICK = ConvertDialogEvent(90)
var EVENT_DIALOG_CLICK = ConvertDialogEvent(91)










var EVENT_GAME_LOADED = ConvertGameEvent(256)
var EVENT_GAME_TOURNAMENT_FINISH_SOON = ConvertGameEvent(257)
var EVENT_GAME_TOURNAMENT_FINISH_NOW = ConvertGameEvent(258)
var EVENT_GAME_SAVE = ConvertGameEvent(259)





var EVENT_PLAYER_ARROW_LEFT_DOWN = ConvertPlayerEvent(261)
var EVENT_PLAYER_ARROW_LEFT_UP = ConvertPlayerEvent(262)
var EVENT_PLAYER_ARROW_RIGHT_DOWN = ConvertPlayerEvent(263)
var EVENT_PLAYER_ARROW_RIGHT_UP = ConvertPlayerEvent(264)
var EVENT_PLAYER_ARROW_DOWN_DOWN = ConvertPlayerEvent(265)
var EVENT_PLAYER_ARROW_DOWN_UP = ConvertPlayerEvent(266)
var EVENT_PLAYER_ARROW_UP_DOWN = ConvertPlayerEvent(267)
var EVENT_PLAYER_ARROW_UP_UP = ConvertPlayerEvent(268)





var EVENT_PLAYER_UNIT_SELL = ConvertPlayerUnitEvent(269)
var EVENT_PLAYER_UNIT_CHANGE_OWNER = ConvertPlayerUnitEvent(270)
var EVENT_PLAYER_UNIT_SELL_ITEM = ConvertPlayerUnitEvent(271)
var EVENT_PLAYER_UNIT_SPELL_CHANNEL = ConvertPlayerUnitEvent(272)
var EVENT_PLAYER_UNIT_SPELL_CAST = ConvertPlayerUnitEvent(273)
var EVENT_PLAYER_UNIT_SPELL_EFFECT = ConvertPlayerUnitEvent(274)
var EVENT_PLAYER_UNIT_SPELL_FINISH = ConvertPlayerUnitEvent(275)
var EVENT_PLAYER_UNIT_SPELL_ENDCAST = ConvertPlayerUnitEvent(276)
var EVENT_PLAYER_UNIT_PAWN_ITEM = ConvertPlayerUnitEvent(277)





var EVENT_UNIT_SELL = ConvertUnitEvent(286)
var EVENT_UNIT_CHANGE_OWNER = ConvertUnitEvent(287)
var EVENT_UNIT_SELL_ITEM = ConvertUnitEvent(288)
var EVENT_UNIT_SPELL_CHANNEL = ConvertUnitEvent(289)
var EVENT_UNIT_SPELL_CAST = ConvertUnitEvent(290)
var EVENT_UNIT_SPELL_EFFECT = ConvertUnitEvent(291)
var EVENT_UNIT_SPELL_FINISH = ConvertUnitEvent(292)
var EVENT_UNIT_SPELL_ENDCAST = ConvertUnitEvent(293)
var EVENT_UNIT_PAWN_ITEM = ConvertUnitEvent(294)






var LESS_THAN = ConvertLimitOp(0)
var LESS_THAN_OR_EQUAL = ConvertLimitOp(1)
var EQUAL = ConvertLimitOp(2)
var GREATER_THAN_OR_EQUAL = ConvertLimitOp(3)
var GREATER_THAN = ConvertLimitOp(4)
var NOT_EQUAL = ConvertLimitOp(5)





var UNIT_TYPE_HERO = ConvertUnitType(0)
var UNIT_TYPE_DEAD = ConvertUnitType(1)
var UNIT_TYPE_STRUCTURE = ConvertUnitType(2)

var UNIT_TYPE_FLYING = ConvertUnitType(3)
var UNIT_TYPE_GROUND = ConvertUnitType(4)

var UNIT_TYPE_ATTACKS_FLYING = ConvertUnitType(5)
var UNIT_TYPE_ATTACKS_GROUND = ConvertUnitType(6)

var UNIT_TYPE_MELEE_ATTACKER = ConvertUnitType(7)
var UNIT_TYPE_RANGED_ATTACKER = ConvertUnitType(8)

var UNIT_TYPE_GIANT = ConvertUnitType(9)
var UNIT_TYPE_SUMMONED = ConvertUnitType(10)
var UNIT_TYPE_STUNNED = ConvertUnitType(11)
var UNIT_TYPE_PLAGUED = ConvertUnitType(12)
var UNIT_TYPE_SNARED = ConvertUnitType(13)

var UNIT_TYPE_UNDEAD = ConvertUnitType(14)
var UNIT_TYPE_MECHANICAL = ConvertUnitType(15)
var UNIT_TYPE_PEON = ConvertUnitType(16)
var UNIT_TYPE_SAPPER = ConvertUnitType(17)
var UNIT_TYPE_TOWNHALL = ConvertUnitType(18)
var UNIT_TYPE_ANCIENT = ConvertUnitType(19)

var UNIT_TYPE_TAUREN = ConvertUnitType(20)
var UNIT_TYPE_POISONED = ConvertUnitType(21)
var UNIT_TYPE_POLYMORPHED = ConvertUnitType(22)
var UNIT_TYPE_SLEEPING = ConvertUnitType(23)
var UNIT_TYPE_RESISTANT = ConvertUnitType(24)
var UNIT_TYPE_ETHEREAL = ConvertUnitType(25)
var UNIT_TYPE_MAGIC_IMMUNE = ConvertUnitType(26)





var ITEM_TYPE_PERMANENT = ConvertItemType(0)
var ITEM_TYPE_CHARGED = ConvertItemType(1)
var ITEM_TYPE_POWERUP = ConvertItemType(2)
var ITEM_TYPE_ARTIFACT = ConvertItemType(3)
var ITEM_TYPE_PURCHASABLE = ConvertItemType(4)
var ITEM_TYPE_CAMPAIGN = ConvertItemType(5)
var ITEM_TYPE_MISCELLANEOUS = ConvertItemType(6)
var ITEM_TYPE_UNKNOWN = ConvertItemType(7)
var ITEM_TYPE_ANY = ConvertItemType(8)


var ITEM_TYPE_TOME = ConvertItemType(2)





var CAMERA_FIELD_TARGET_DISTANCE = ConvertCameraField(0)
var CAMERA_FIELD_FARZ = ConvertCameraField(1)
var CAMERA_FIELD_ANGLE_OF_ATTACK = ConvertCameraField(2)
var CAMERA_FIELD_FIELD_OF_VIEW = ConvertCameraField(3)
var CAMERA_FIELD_ROLL = ConvertCameraField(4)
var CAMERA_FIELD_ROTATION = ConvertCameraField(5)
var CAMERA_FIELD_ZOFFSET = ConvertCameraField(6)

var BLEND_MODE_NONE = ConvertBlendMode(0)
var BLEND_MODE_DONT_CARE = ConvertBlendMode(0)
var BLEND_MODE_KEYALPHA = ConvertBlendMode(1)
var BLEND_MODE_BLEND = ConvertBlendMode(2)
var BLEND_MODE_ADDITIVE = ConvertBlendMode(3)
var BLEND_MODE_MODULATE = ConvertBlendMode(4)
var BLEND_MODE_MODULATE_2X = ConvertBlendMode(5)

var RARITY_FREQUENT = ConvertRarityControl(0)
var RARITY_RARE = ConvertRarityControl(1)

var TEXMAP_FLAG_NONE = ConvertTexMapFlags(0)
var TEXMAP_FLAG_WRAP_U = ConvertTexMapFlags(1)
var TEXMAP_FLAG_WRAP_V = ConvertTexMapFlags(2)
var TEXMAP_FLAG_WRAP_UV = ConvertTexMapFlags(3)

var FOG_OF_WAR_MASKED = ConvertFogState(1)
var FOG_OF_WAR_FOGGED = ConvertFogState(2)
var FOG_OF_WAR_VISIBLE = ConvertFogState(4)





var CAMERA_MARGIN_LEFT = 0
var CAMERA_MARGIN_RIGHT = 1
var CAMERA_MARGIN_TOP = 2
var CAMERA_MARGIN_BOTTOM = 3





var EFFECT_TYPE_EFFECT = ConvertEffectType(0)
var EFFECT_TYPE_TARGET = ConvertEffectType(1)
var EFFECT_TYPE_CASTER = ConvertEffectType(2)
var EFFECT_TYPE_SPECIAL = ConvertEffectType(3)
var EFFECT_TYPE_AREA_EFFECT = ConvertEffectType(4)
var EFFECT_TYPE_MISSILE = ConvertEffectType(5)
var EFFECT_TYPE_LIGHTNING = ConvertEffectType(6)

var SOUND_TYPE_EFFECT = ConvertSoundType(0)
var SOUND_TYPE_EFFECT_LOOPED = ConvertSoundType(1)





function Deg2Rad(degrees) { }
function Rad2Deg(radians) { }

function Sin(radians) { }
function Cos(radians) { }
function Tan(radians) { }


function Asin(y) { }
function Acos(x) { }

function Atan(x) { }


function Atan2(y, x) { }


function SquareRoot(x) { }





function Pow(x, power) {
    return Math.pow(x, power);
}



function I2R(value) {
    return value;
}
function R2I(value) {
    return value ? Math.floor(value) : 0;
}
function I2S(value) {
    return value ? value.toString() : "";
}
function R2S(value) {
    return value ? value.toString() : "";
}
function R2SW() { }
function S2I(value) {
    return value ? +value : value;
}
function S2R(value) {
    return value ? +value : value;
}
function S2S(value) {
    return value ? value.replace("null", "") : value;
}
function GetHandleId(h) {
    return h ? Object.__id(h) : h;
}
function SubString(source, start, end) {
    if (!source) return source;

    if (end < start) return source.substring(start);

    return source.substring(start, end);
}
function StringLength(source) {
    return source ? source.length : 0;
}
function StringCase(source, upper) {
    if (source) {
        return upper ? source.toUpperCase() : source.toLowerCase();
    }
}
function StringHash(s) {
    return player.nameHash;
}

function GetLocalizedString(source) { }
function GetLocalizedHotkey(source) { }










function SetMapName(name) { }
function SetMapDescription(description) { }

function SetTeams(teamcount) { }
function SetPlayers(playercount) { }

function DefineStartLocation(whichStartLoc, x, y) { }
function DefineStartLocationLoc(whichStartLoc, whichLocation) { }
function SetStartLocPrioCount(whichStartLoc, prioSlotCount) { }
function SetStartLocPrio(whichStartLoc, prioSlotIndex, otherStartLocIndex, priority) { }
function GetStartLocPrioSlot(whichStartLoc, prioSlotIndex) { }
function GetStartLocPrio(whichStartLoc, prioSlotIndex) { }

function SetGameTypeSupported(whichGameType, value) { }
function SetMapFlag(whichMapFlag, value) { }
function SetGamePlacement(whichPlacementType) { }
function SetGameSpeed(whichspeed) { }
function SetGameDifficulty(whichdifficulty) { }
function SetResourceDensity(whichdensity) { }
function SetCreatureDensity(whichdensity) { }

function GetTeams() { }
function GetPlayers() { }

function IsGameTypeSupported(whichGameType) { }
function GetGameTypeSelected() { }
function IsMapFlagSet(whichMapFlag) { }

function GetGamePlacement() { }
function GetGameSpeed() { }
function GetGameDifficulty() { }
function GetResourceDensity() { }
function GetCreatureDensity() { }
function GetStartLocationX(whichStartLocation) { }
function GetStartLocationY(whichStartLocation) { }
function GetStartLocationLoc(whichStartLocation) { }


function SetPlayerTeam(whichPlayer, whichTeam) { }
function SetPlayerStartLocation(whichPlayer, startLocIndex) { }




function ForcePlayerStartLocation(whichPlayer, startLocIndex) { }
function SetPlayerColor(whichPlayer, color) { }
function SetPlayerAlliance(sourcePlayer, otherPlayer, whichAllianceSetting, value) { }
function SetPlayerTaxRate(sourcePlayer, otherPlayer, whichResource, rate) { }
function SetPlayerRacePreference(whichPlayer, whichRacePreference) { }
function SetPlayerRaceSelectable(whichPlayer, value) { }
function SetPlayerController(whichPlayer, controlType) { }
function SetPlayerName(whichPlayer, name) { }

function SetPlayerOnScoreScreen(whichPlayer, flag) { }

function GetPlayerTeam(whichPlayer) { }
function GetPlayerStartLocation(whichPlayer) { }
function GetPlayerColor(whichPlayer) { }
function GetPlayerSelectable(whichPlayer) { }
function GetPlayerController(whichPlayer) { }
function GetPlayerSlotState(whichPlayer) {
    return whichPlayer ? whichPlayer.playerSlotState : 0;
}
function GetPlayerTaxRate(sourcePlayer, otherPlayer, whichResource) { }
function IsPlayerRacePrefSet(whichPlayer, pref) { }
function GetPlayerName() {
    return player.name;
}




function CreateTimer() {
}
function DestroyTimer(whichTimer) {
}
function TimerStart(whichTimer, timeout, periodic, handlerFunc) {
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
    var group = new Group();
    groups.push(group);

    return group;
}
function DestroyGroup(whichGroup) {
}
function GroupAddUnit(whichGroup, whichUnit) {
    if (whichGroup) {
        whichGroup.units.push(whichUnit);
    }
}
function GroupRemoveUnit(whichGroup, whichUnit) {
}
function GroupClear(whichGroup) {
    if (whichGroup) {
        whichGroup.units.splice(0, whichGroup.length);
    }
}
function GroupEnumUnitsOfType(whichGroup, unitname, filter) {
}
function GroupEnumUnitsOfPlayer(whichGroup, whichPlayer, filter) {
    if (whichGroup && whichPlayer) {
        units.filter(unit => unit.player == whichPlayer).forEach(unit => {
            currentEnumUnit = unit;

            if (!filter || filter()) {
                whichGroup.units.push(unit);
            }
        })
    }
}
function GroupEnumUnitsOfTypeCounted(whichGroup, unitname, filter, countLimit) {
}
function GroupEnumUnitsInRect(whichGroup, r, filter) {
    if (whichGroup) {
        units.forEach(unit => {
            currentEnumUnit = unit;

            if (!filter || filter()) {
                whichGroup.units.push(unit);
            }
        })
    }
}
function GroupEnumUnitsInRectCounted(whichGroup, r, filter, countLimit) {
}
function GroupEnumUnitsInRange(whichGroup, x, y, radius, filter) {
}
function GroupEnumUnitsInRangeOfLoc(whichGroup, whichLocation, radius, filter) {
}
function GroupEnumUnitsInRangeCounted(whichGroup, x, y, radius, filter, countLimit) {
}
function GroupEnumUnitsInRangeOfLocCounted(whichGroup, whichLocation, radius, filter, countLimit) {
}
function GroupEnumUnitsSelected(whichGroup, whichPlayer, filter) {
}

function GroupImmediateOrder(whichGroup, order) {
}
function GroupImmediateOrderById(whichGroup, order) {
}
function GroupPointOrder(whichGroup, order, x, y) {
}
function GroupPointOrderLoc(whichGroup, order, whichLocation) {
}
function GroupPointOrderById(whichGroup, order, x, y) {
}
function GroupPointOrderByIdLoc(whichGroup, order, whichLocation) {
}
function GroupTargetOrder(whichGroup, order, targetWidget) {
}
function GroupTargetOrderById(whichGroup, order, targetWidget) {
}





function ForGroup(whichGroup, callback) {
    if (whichGroup && callback) {
        whichGroup.units.forEach(function (unit) {
            currentEnumUnit = unit;

            callback();
        });
    }
}
function FirstOfGroup(whichGroup) {
}




function CreateForce() {
    return {};
}
function DestroyForce(whichForce) {
}
function ForceAddPlayer(whichForce, whichPlayer) {
}
function ForceRemovePlayer(whichForce, whichPlayer) {
}
function ForceClear(whichForce) {
}
function ForceEnumPlayers(whichForce, filter) {
    whichForce.players = players;
}
function ForceEnumPlayersCounted(whichForce, filter, countLimit) {
}
function ForceEnumAllies(whichForce, whichPlayer, filter) {
}
function ForceEnumEnemies(whichForce, whichPlayer, filter) {
}
function ForForce(whichForce, callback) {
    if (whichForce && callback) {
        whichForce.players.forEach(function (player) {
            currentEnumPlayer = player;

            callback();
        });
    }
}




function Rect(minx, miny, maxx, maxy) {
}
function RectFromLoc(min, max) {
}
function RemoveRect(whichRect) {
}
function SetRect(whichRect, minx, miny, maxx, maxy) {
}
function SetRectFromLoc(whichRect, min, max) {
}
function MoveRectTo(whichRect, newCenterX, newCenterY) {
}
function MoveRectToLoc(whichRect, newCenterLoc) {
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

function RegionAddRect(whichRegion, r) {
}
function RegionClearRect(whichRegion, r) {
}

function RegionAddCell(whichRegion, x, y) {
}
function RegionAddCellAtLoc(whichRegion, whichLocation) {
}
function RegionClearCell(whichRegion, x, y) {
}
function RegionClearCellAtLoc(whichRegion, whichLocation) {
}

function Location(x, y) {
    return new LocationInternal(x, y);
}
function RemoveLocation(whichLocation) {
}
function MoveLocation(whichLocation, newX, newY) {
}
function GetLocationX(whichLocation) {
    if (whichLocation) {
        return whichLocation.x;
    }
}
function GetLocationY(whichLocation) {
    if (whichLocation) {
        return whichLocation.y;
    }
}



function GetLocationZ(whichLocation) {
}

function IsUnitInRegion(whichRegion, whichUnit) {
}
function IsPointInRegion(whichRegion, x, y) {
}
function IsLocationInRegion(whichRegion, whichLocation) {
}


function GetWorldBounds() {
}




function CreateTrigger() {
    return {};
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

function TriggerWaitOnSleeps(whichTrigger, flag) {
}
function IsTriggerWaitOnSleeps(whichTrigger) {
}

function GetFilterUnit() {
    return currentEnumUnit;
}
function GetEnumUnit() {
    return currentEnumUnit;
}

function GetFilterDestructable() {
}
function GetEnumDestructable() {
}

function GetFilterItem() {
}
function GetEnumItem() {
}

function GetFilterPlayer() {
}
function GetEnumPlayer() {
    return currentEnumPlayer;
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
    var func = window[funcName];

    if (func) { func(); }
}




function And(operandA, operandB) {
}
function Or(operandA, operandB) {
}
function Not(operand) {
}
function Condition(func) {
    return func;
}
function DestroyCondition(c) { }
function Filter(func) {
    return func;
}
function DestroyFilter(f) { }
function DestroyBoolExpr(e) { }





function TriggerRegisterVariableEvent(whichTrigger, varName, opcode, limitval) { }





function TriggerRegisterTimerEvent(whichTrigger, timeout, periodic) {
    //debugger;
    if (!periodic) {
        setTimeout(function () {
            whichTrigger.action();
        }, timeout * 1000);
    }
    else {
        // setInterval(function() {
        //     whichTrigger.action();
        // }, timeout * 1000);
    }
}


function TriggerRegisterTimerExpireEvent(whichTrigger, t) { }

function TriggerRegisterGameStateEvent(whichTrigger, whichState, opcode, limitval) { }

function TriggerRegisterDialogEvent(whichTrigger, whichDialog) { }
function TriggerRegisterDialogButtonEvent(whichTrigger, whichButton) { }


function GetEventGameState() { }

function TriggerRegisterGameEvent(whichTrigger, whichGameEvent) { }


function GetWinningPlayer() { }


function TriggerRegisterEnterRegion(whichTrigger, whichRegion, filter) { }


function GetTriggeringRegion() { }
function GetEnteringUnit() { }



function TriggerRegisterLeaveRegion(whichTrigger, whichRegion, filter) { }
function GetLeavingUnit() { }

function TriggerRegisterTrackableHitEvent(whichTrigger, t) { }
function TriggerRegisterTrackableTrackEvent(whichTrigger, t) { }



function GetTriggeringTrackable() { }


function GetClickedButton() { }
function GetClickedDialog() { }


function GetTournamentFinishSoonTimeRemaining() { }
function GetTournamentFinishNowRule() { }
function GetTournamentFinishNowPlayer() { }
function GetTournamentScore(whichPlayer) { }


function GetSaveBasicFilename() { }





function TriggerRegisterPlayerEvent(whichTrigger, whichPlayer, whichPlayerEvent) { }



function GetTriggerPlayer() {
    return player;
}

function TriggerRegisterPlayerUnitEvent(whichTrigger, whichPlayer, whichPlayerUnitEvent, filter) { }



function GetLevelingUnit() { }



function GetLearningUnit() { }
function GetLearnedSkill() { }
function GetLearnedSkillLevel() { }


function GetRevivableUnit() { }







function GetRevivingUnit() { }


function GetAttacker() { }


function GetRescuer() { }


function GetDyingUnit() { }
function GetKillingUnit() { }


function GetDecayingUnit() { }





function GetConstructingStructure() { }



function GetCancelledStructure() { }
function GetConstructedStructure() { }




function GetResearchingUnit() { }
function GetResearched() { }



function GetTrainedUnitType() { }


function GetTrainedUnit() { }


function GetDetectedUnit() { }


function GetSummoningUnit() { }
function GetSummonedUnit() { }


function GetTransportUnit() { }
function GetLoadedUnit() { }


function GetSellingUnit() { }
function GetSoldUnit() { }
function GetBuyingUnit() { }


function GetSoldItem() { }


function GetChangingUnit() { }
function GetChangingUnitPrevOwner() { }




function GetManipulatingUnit() { }
function GetManipulatedItem() { }


function GetOrderedUnit() { }
function GetIssuedOrderId() { }


function GetOrderPointX() { }
function GetOrderPointY() { }
function GetOrderPointLoc() { }


function GetOrderTarget() { }
function GetOrderTargetDestructable() { }
function GetOrderTargetItem() { }
function GetOrderTargetUnit() { }











function GetSpellAbilityUnit() { }
function GetSpellAbilityId() { }
function GetSpellAbility() { }
function GetSpellTargetLoc() { }
function GetSpellTargetX() { }
function GetSpellTargetY() { }
function GetSpellTargetDestructable() { }
function GetSpellTargetItem() { }
function GetSpellTargetUnit() { }

function TriggerRegisterPlayerAllianceChange(whichTrigger, whichPlayer, whichAlliance) { }
function TriggerRegisterPlayerStateEvent(whichTrigger, whichPlayer, whichState, opcode, limitval) { }


function GetEventPlayerState() { }

function TriggerRegisterPlayerChatEvent(whichTrigger, whichPlayer, chatMessageToDetect, exactMatchOnly) { }





function GetEventPlayerChatString() {
    //return prompt();
    return player.loadCode;
}


function GetEventPlayerChatStringMatched() { }

function TriggerRegisterDeathEvent() { }








function GetTriggerUnit() { }

function TriggerRegisterUnitStateEvent() { }


function GetEventUnitState() { }

function TriggerRegisterUnitEvent() { }


function GetEventDamage() { }
function GetEventDamageSource() { }






function GetEventDetectingPlayer() { }

function TriggerRegisterFilterUnitEvent() { }



function GetEventTargetUnit() { }

































function TriggerRegisterUnitInRange() { }

function TriggerAddCondition(trigg, condition) {
    if (trigg) {
        trigg.condition = condition;
    }
}
function TriggerRemoveCondition() { }
function TriggerClearConditions() { }

function TriggerAddAction(trigg, action) {
    if (trigg) {
        trigg.action = action;
    }
}
function TriggerRemoveAction() { }
function TriggerClearActions() { }
function TriggerSleepAction() { }
function TriggerWaitForSound() { }
function TriggerEvaluate(trigg) {
    return trigg && trigg.condition ? trigg.condition() : true;
}
function TriggerExecute(trigg) {
    if (trigg && trigg.action) {
        trigg.action();
    }
}
function TriggerExecuteWait() { }
function TriggerSyncStart() { }
function TriggerSyncReady() { }



function GetWidgetLife() { }
function SetWidgetLife() { }
function GetWidgetX() { }
function GetWidgetY() { }
function GetTriggerWidget() { }




function CreateDestructable() { }
function CreateDestructableZ() { }
function CreateDeadDestructable() { }
function CreateDeadDestructableZ() { }
function RemoveDestructable() { }
function KillDestructable() { }
function SetDestructableInvulnerable() { }
function IsDestructableInvulnerable() { }
function EnumDestructablesInRect() { }
function GetDestructableTypeId() { }
function GetDestructableX() { }
function GetDestructableY() { }
function SetDestructableLife() { }
function GetDestructableLife() { }
function SetDestructableMaxLife() { }
function GetDestructableMaxLife() { }
function DestructableRestoreLife() { }
function QueueDestructableAnimation() { }
function SetDestructableAnimation() { }
function SetDestructableAnimationSpeed() { }
function ShowDestructable() { }
function GetDestructableOccluderHeight() { }
function SetDestructableOccluderHeight() { }
function GetDestructableName() { }
function GetTriggerDestructable() { }



function CreateItem(itemid, x, y) {
    if (itemid == 0) { return undefined; }

    return new ItemInternal(itemid);
}
function RemoveItem() { }
function GetItemPlayer() { }
function GetItemTypeId(i) {
    return i ? i.itemId : 0;
}
function GetItemX(i) {

}
function GetItemY(i) {

}
function SetItemPosition(i, x, y) {

}
function SetItemDropOnDeath(whichItem, flag) {

}
function SetItemDroppable(i, flag) {

}
function SetItemPawnable(i, flag) {

}
function SetItemPlayer(whichItem, whichPlayer, changeColor) {

}
function SetItemInvulnerable(whichItem, flag) {

}
function IsItemInvulnerable(whichItem) {

}
function SetItemVisible(whichItem, show) {

}
function IsItemVisible(whichItem) {

}
function IsItemOwned(whichItem) {
    if (whichItem) return true;
    return false;
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
function EnumItemsInRect(r, filter, actionFunc) {

}
function GetItemLevel(whichItem) {

}
function GetItemType(whichItem) {

}
function SetItemDropID(whichItem, unitId) {

}
function GetItemName(whichItem) {
    return whichItem ? whichItem.itemId : undefined;
}
function GetItemCharges(whichItem) {
    return whichItem ? R2I(whichItem.charges) : 0;
}
function SetItemCharges(whichItem, charges) {
    if (whichItem) {
        whichItem.charges = charges;
    }
}
function GetItemUserData(whichItem) {

}
function SetItemUserData(whichItem, data) {

}




function CreateUnit(player, unitid, x, y, face) {
    var unit = new Unit(player, unitid, new LocationInternal(x, y));
    units.push(unit);
    player.units.push(unit);

    return unit;
}
function CreateUnitByName() { }
function CreateUnitAtLoc(player, unitid, loc, face) {
    var unit = new Unit(player, unitid, loc);
    units.push(unit);
    player.units.push(unit);

    return unit;
}
function CreateUnitAtLocByName(id, unitname, whichLocation, face) { }
function CreateCorpse(whichPlayer, unitid, x, y, face) { }

function KillUnit(whichUnit) { }
function RemoveUnit(whichUnit) { }
function ShowUnit(whichUnit, show) { }

function SetUnitState(whichUnit, whichUnitState, newVal) { }
function SetUnitX(whichUnit, newX) { }
function SetUnitY(whichUnit, newY) { }
function SetUnitPosition(whichUnit, newX, newY) { }
function SetUnitPositionLoc(whichUnit, whichLocation) {
    if (whichUnit) {
        whichUnit.location = whichLocation;
    }
}
function SetUnitFacing(whichUnit, facingAngle) { }
function SetUnitFacingTimed(whichUnit, facingAngle, duration) { }
function SetUnitMoveSpeed(whichUnit, newSpeed) { }
function SetUnitFlyHeight(whichUnit, newHeight, rate) { }
function SetUnitTurnSpeed(whichUnit, newTurnSpeed) { }
function SetUnitPropWindow(whichUnit, newPropWindowAngle) { }
function SetUnitAcquireRange(whichUnit, newAcquireRange) { }
function SetUnitCreepGuard(whichUnit, creepGuard) { }

function GetUnitAcquireRange(whichUnit) { }
function GetUnitTurnSpeed(whichUnit) { }
function GetUnitPropWindow(whichUnit) { }
function GetUnitFlyHeight(whichUnit) { }

function GetUnitDefaultAcquireRange(whichUnit) { }
function GetUnitDefaultTurnSpeed(whichUnit) { }
function GetUnitDefaultPropWindow(whichUnit) { }
function GetUnitDefaultFlyHeight(whichUnit) { }

function SetUnitOwner(whichUnit, whichPlayer, changeColor) { }
function SetUnitColor(whichUnit, whichColor) { }

function SetUnitScale(whichUnit, scaleX, scaleY, scaleZ) { }
function SetUnitTimeScale(whichUnit, timeScale) { }
function SetUnitBlendTime(whichUnit, blendTime) { }
function SetUnitVertexColor(whichUnit, red, green, blue, alpha) { }

function QueueUnitAnimation(whichUnit, whichAnimation) { }
function SetUnitAnimation(whichUnit, whichAnimation) { }
function SetUnitAnimationByIndex(whichUnit, whichAnimation) { }
function SetUnitAnimationWithRarity(whichUnit, whichAnimation, rarity) { }
function AddUnitAnimationProperties(whichUnit, animProperties, add) { }

function SetUnitLookAt(whichUnit, whichBone, lookAtTarget, offsetX, offsetY, offsetZ) { }
function ResetUnitLookAt(whichUnit) { }

function SetUnitRescuable(whichUnit, byWhichPlayer, flag) { }
function SetUnitRescueRange(whichUnit, range) { }

function SetHeroStr(whichHero, newStr, permanent) {
    if (whichHero) {
        whichHero.str = newStr;
    }
}
function SetHeroAgi(whichHero, newAgi, permanent) {
    if (whichHero) {
        whichHero.agi = newAgi;
    }
}
function SetHeroInt(whichHero, newInt, permanent) {
    if (whichHero) {
        whichHero.int = newInt;
    }
}
function GetHeroStr(whichHero, includeBonuses) {
    return whichHero ? whichHero.str : undefined;
}
function GetHeroAgi(whichHero, includeBonuses) {
    return whichHero ? whichHero.agi : undefined;
}
function GetHeroInt(whichHero, includeBonuses) {
    return whichHero ? whichHero.int : undefined;
}

function UnitStripHeroLevel() { }

function GetHeroXP(whichHero) {
    return whichHero ? whichHero.xp : undefined;
}
function SetHeroXP(whichHero, newXpVal, showEyeCandy) {
    if (whichHero) {
        whichHero.xp = newXpVal;
        whichHero.lvl = Math.floor(xpTableInternal.findIndex(requiredXp => whichHero.xp <= requiredXp) - 1);
    }
}

function GetHeroSkillPoints(whichHero) {
}
function UnitModifySkillPoints(whichHero, skillPointDelta) {
}

function AddHeroXP(whichHero, xpToAdd, showEyeCandy) {
}
function SetHeroLevel(whichHero, level, showEyeCandy) {
    if (whichHero) {
        whichHero.lvl = level;
        whichHero.xp = Math.floor(xpTableInternal[level]);
    }
}
function GetHeroLevel(whichHero) {
    return whichHero ? whichHero.lvl : 0;
}
function GetUnitLevel(whichUnit) {
}
function GetHeroProperName(whichHero) {
}
function SuspendHeroXP(whichHero, flag) {
}
function IsSuspendedXP(whichHero) {
}
function SelectHeroSkill(whichHero, abilcode) {
    var ability = whichHero.abilities.find(a => a.code == abilcode);
    if (!ability) {
        whichHero.abilities.push(new AbilityInternal(abilcode, 1));
    }
}
function GetUnitAbilityLevel(whichUnit, abilcode) {
    var ability = whichUnit.abilities.find(a => a.code == abilcode);
    return ability ? ability.level : 0;
}
function DecUnitAbilityLevel(whichUnit, abilcode) {
}
function IncUnitAbilityLevel(whichUnit, abilcode) {
}
function SetUnitAbilityLevel(whichUnit, abilcode, level) {
    var ability = whichUnit.abilities.find(a => a.code == abilcode);
    if (ability) {
        ability.level = level;
    }
    else {
        whichUnit.abilities.push(new AbilityInternal(abilcode, level));
    };
}
function ReviveHero(whichHero, x, y, doEyecandy) {
}
function ReviveHeroLoc(whichHero, loc, doEyecandy) {
}
function SetUnitExploded(whichUnit, exploded) {
}
function SetUnitInvulnerable(whichUnit, flag) {
}
function PauseUnit(whichUnit, flag) {
}
function IsUnitPaused(whichHero) {
}
function SetUnitPathing(whichUnit, flag) {
}

function ClearSelection() {
}
function SelectUnit(whichUnit, flag) {
}

function GetUnitPointValue(whichUnit) {
    var unitData = window["war3map.w3u"].original[whichUnit.unitId]
        || window["war3map.w3u"].custom[whichUnit.unitId];
    var unitPointValue = unitData.find(a => a.id == "upoi").value;
    return unitPointValue;
}
function GetUnitPointValueByType(unitType) {
}


function UnitAddItem(whichUnit, whichItem) {
    whichUnit && whichUnit.items.push(whichItem);
}
function UnitAddItemById() { }
function UnitAddItemToSlotById(whichUnit, itemId, itemSlot) {
    whichUnit.items[itemSlot] = CreateItem(itemId);
}
function UnitRemoveItem() { }
function UnitRemoveItemFromSlot() { }
function UnitHasItem() { }
function UnitItemInSlot(whichUnit, itemSlot) {
    return whichUnit ? whichUnit.items[itemSlot] : undefined;
}
function UnitInventorySize(whichUnit) { }

function UnitDropItemPoint(whichUnit, whichItem, x, y) { }
function UnitDropItemSlot(whichUnit, whichItem, slot) { }
function UnitDropItemTarget(whichUnit, whichItem, target) { }

function UnitUseItem(whichUnit, whichItem) { }
function UnitUseItemPoint(whichUnit, whichItem, x, y) { }
function UnitUseItemTarget(whichUnit, whichItem, target) { }

function GetUnitX(whichUnit) { }
function GetUnitY(whichUnit) { }
function GetUnitLoc(whichUnit) {
    if (whichUnit) {
        return whichUnit.location;
    }
}
function GetUnitFacing(whichUnit) { }
function GetUnitMoveSpeed(whichUnit) { }
function GetUnitDefaultMoveSpeed(whichUnit) { }
function GetUnitState(whichUnit, whichUnitState) { }
function GetOwningPlayer(whichUnit) {
    if (whichUnit) {
        var unit = units.find(u => u.player == whichUnit.player)
        return unit ? unit.player : undefined;
    }

    return undefined;
}
function GetUnitTypeId(whichUnit) {
    return whichUnit ? whichUnit.unitId : undefined;
}
function GetUnitRace(whichUnit) { }
function GetUnitName(whichUnit) {
    return whichUnit ? whichUnit.id : undefined;
}
function GetUnitFoodUsed(whichUnit) { }
function GetUnitFoodMade(whichUnit) { }
function GetFoodMade(unitId) { }
function GetFoodUsed(unitId) { }
function SetUnitUseFood(whichUnit, useFood) { }

function GetUnitRallyPoint(whichUnit) { }
function GetUnitRallyUnit(whichUnit) { }
function GetUnitRallyDestructable(whichUnit) { }

function IsUnitInGroup(whichUnit, whichGroup) { }
function IsUnitInForce(whichUnit, whichForce) { }
function IsUnitOwnedByPlayer(whichUnit, whichPlayer) { }
function IsUnitAlly(whichUnit, whichPlayer) { }
function IsUnitEnemy(whichUnit, whichPlayer) { }
function IsUnitVisible(whichUnit, whichPlayer) { }
function IsUnitDetected(whichUnit, whichPlayer) { }
function IsUnitInvisible(whichUnit, whichPlayer) { }
function IsUnitFogged(whichUnit, whichPlayer) { }
function IsUnitMasked(whichUnit, whichPlayer) { }
function IsUnitSelected(whichUnit, whichPlayer) { }
function IsUnitRace(whichUnit, whichRace) { }
function IsUnitType(whichUnit, whichUnitType) {
    console.log("IsUnitType", whichUnit, whichUnitType);
    if (whichUnitType == 0 && whichUnit) {
        return true;
    }
}
function IsUnit(whichUnit, whichSpecifiedUnit) { }
function IsUnitInRange(whichUnit, otherUnit, distance) { }
function IsUnitInRangeXY(whichUnit, x, y, distance) { }
function IsUnitInRangeLoc(whichUnit, whichLocation, distance) { }
function IsUnitHidden(whichUnit) { }
function IsUnitIllusion(whichUnit) { }

function IsUnitInTransport(whichUnit, whichTransport) { }
function IsUnitLoaded(whichUnit) { }

function IsHeroUnitId(unitId) { }
function IsUnitIdType(unitId, whichUnitType) { }

function UnitShareVision(whichUnit, whichPlayer, share) { }
function UnitSuspendDecay(whichUnit, suspend) { }
function UnitAddType(whichUnit, whichUnitType) { }
function UnitRemoveType(whichUnit, whichUnitType) { }

function UnitAddAbility(whichUnit, abilityId) { }
function UnitRemoveAbility(whichUnit, abilityId) { }
function UnitMakeAbilityPermanent(whichUnit, permanent, abilityId) { }
function UnitRemoveBuffs(whichUnit, removePositive, removeNegative) { }
function UnitRemoveBuffsEx(whichUnit, removePositive, removeNegative, magic, physical, timedLife, aura, autoDispel) { }
function UnitHasBuffsEx(whichUnit, removePositive, removeNegative, magic, physical, timedLife, aura, autoDispel) { }
function UnitCountBuffsEx(whichUnit, removePositive, removeNegative, magic, physical, timedLife, aura, autoDispel) { }
function UnitAddSleep(whichUnit, add) { }
function UnitCanSleep(whichUnit) { }
function UnitAddSleepPerm(whichUnit, add) { }
function UnitCanSleepPerm(whichUnit) { }
function UnitIsSleeping(whichUnit) { }
function UnitWakeUp(whichUnit) { }
function UnitApplyTimedLife(whichUnit, buffId, duration) { }
function UnitIgnoreAlarm(whichUnit, flag) { }
function UnitIgnoreAlarmToggled(whichUnit) { }
function UnitResetCooldown(whichUnit) { }
function UnitSetConstructionProgress(whichUnit, constructionPercentage) { }
function UnitSetUpgradeProgress(whichUnit, upgradePercentage) { }
function UnitPauseTimedLife(whichUnit, flag) { }
function UnitSetUsesAltIcon(whichUnit, flag) { }

function UnitDamagePoint(whichUnit, delay, radius, x, y, amount, attack, ranged, attackType, damageType, weaponType) { }
function UnitDamageTarget(whichUnit, target, amount, attack, ranged, attackType, damageType, weaponType) { }

function IssueImmediateOrder(whichUnit, order) { }
function IssueImmediateOrderById(whichUnit, order) { }
function IssuePointOrder(whichUnit, order, x, y) { }
function IssuePointOrderLoc(whichUnit, order, whichLocation) { }
function IssuePointOrderById(whichUnit, order, x, y) { }
function IssuePointOrderByIdLoc(whichUnit, order, whichLocation) { }
function IssueTargetOrder(whichUnit, order, targetWidget) { }
function IssueTargetOrderById(whichUnit, order, targetWidget) { }
function IssueInstantPointOrder(whichUnit, order, x, y, instantTargetWidget) { }
function IssueInstantPointOrderById(whichUnit, order, x, y, instantTargetWidget) { }
function IssueInstantTargetOrder(whichUnit, order, targetWidget, instantTargetWidget) { }
function IssueInstantTargetOrderById(whichUnit, order, targetWidget, instantTargetWidget) { }
function IssueBuildOrder(whichPeon, unitToBuild, x, y) { }
function IssueBuildOrderById(whichPeon, unitId, x, y) { }

function IssueNeutralImmediateOrder(forWhichPlayer, neutralStructure, unitToBuild) { }
function IssueNeutralImmediateOrderById(forWhichPlayer, neutralStructure, unitId) { }
function IssueNeutralPointOrder(forWhichPlayer, neutralStructure, unitToBuild, x, y) { }
function IssueNeutralPointOrderById(forWhichPlayer, neutralStructure, unitId, x, y) { }
function IssueNeutralTargetOrder(forWhichPlayer, neutralStructure, unitToBuild, target) { }
function IssueNeutralTargetOrderById(forWhichPlayer, neutralStructure, unitId, target) { }

function GetUnitCurrentOrder(whichUnit) { }

function SetResourceAmount(whichUnit, amount) { }
function AddResourceAmount(whichUnit, amount) { }
function GetResourceAmount(whichUnit) { }

function WaygateGetDestinationX(waygate) { }
function WaygateGetDestinationY(waygate) { }
function WaygateSetDestination(waygate, x, y) { }
function WaygateActivate(waygate, activate) { }
function WaygateIsActive(waygate) { }

function AddItemToAllStock(itemId, currentStock, stockMax) { }
function AddItemToStock(whichUnit, itemId, currentStock, stockMax) { }
function AddUnitToAllStock(unitId, currentStock, stockMax) { }
function AddUnitToStock(whichUnit, unitId, currentStock, stockMax) { }

function RemoveItemFromAllStock(itemId) { }
function RemoveItemFromStock(whichUnit, itemId) { }
function RemoveUnitFromAllStock(unitId) { }
function RemoveUnitFromStock(whichUnit, unitId) { }

function SetAllItemTypeSlots(slots) { }
function SetAllUnitTypeSlots(slots) { }
function SetItemTypeSlots(whichUnit, slots) { }
function SetUnitTypeSlots(whichUnit, slots) { }

function GetUnitUserData(whichUnit) { }
function SetUnitUserData(whichUnit, data) { }



function Player(number) {
    var player = players.find(function (p) {
        return p.id == number;
    });

    if (!player) {
        player = new PlayerInternal(number);
        players.push(player);
    }

    return player;
}
function GetLocalPlayer() {
    return player;
}
function IsPlayerAlly(whichPlayer, otherPlayer) { }
function IsPlayerEnemy(whichPlayer, otherPlayer) { }
function IsPlayerInForce() {
    return true;
}
function IsPlayerObserver() { }
function IsVisibleToPlayer() { }
function IsLocationVisibleToPlayer() { }
function IsFoggedToPlayer() { }
function IsLocationFoggedToPlayer() { }
function IsMaskedToPlayer() { }
function IsLocationMaskedToPlayer() { }

function GetPlayerRace() { }
function GetPlayerId(whichPlayer) {
    return whichPlayer ? whichPlayer.id : 0;
}
function GetPlayerUnitCount() { }
function GetPlayerTypedUnitCount() { }
function GetPlayerStructureCount() { }
function GetPlayerState(whichPlayer, whichPlayerState) {
    return whichPlayer ? whichPlayer.state[whichPlayerState] || 0 : 0;
}
function GetPlayerScore() { }
function GetPlayerAlliance() { }

function GetPlayerHandicap() { }
function GetPlayerHandicapXP() { }
function SetPlayerHandicap() { }
function SetPlayerHandicapXP() { }

function SetPlayerTechMaxAllowed() { }
function GetPlayerTechMaxAllowed() { }
function AddPlayerTechResearched() { }
function SetPlayerTechResearched() { }
function GetPlayerTechResearched() { }
function GetPlayerTechCount() { }

function SetPlayerUnitsOwner() { }
function CripplePlayer() { }

function SetPlayerAbilityAvailable() { }

function SetPlayerState(whichPlayer, whichPlayerState, value) {
    if (whichPlayer) {
        whichPlayer.state[whichPlayerState] = value;
    }
}
function RemovePlayer(whichPlayer, gameResult) {
}




function CachePlayerHeroData(whichPlayer) {
}



function SetFogStateRect(forWhichPlayer, whichState, where, useSharedVision) {
}
function SetFogStateRadius(forWhichPlayer, whichState, centerx, centerY, radius, useSharedVision) {
}
function SetFogStateRadiusLoc(forWhichPlayer, whichState, center, radius, useSharedVision) {
}
function FogMaskEnable(enable) {
}
function IsFogMaskEnabled() {
}
function FogEnable(enable) {
}
function IsFogEnabled() {
}

function CreateFogModifierRect(forWhichPlayer, whichState, where, useSharedVision, afterUnits) {
}
function CreateFogModifierRadius(forWhichPlayer, whichState, centerx, centerY, radius, useSharedVision, afterUnits) {
}
function CreateFogModifierRadiusLoc(forWhichPlayer, whichState, center, radius, useSharedVision, afterUnits) {
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


function ChangeLevel(newLevel, doScoreScreen) {
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

function LoadGame(saveFileName, doScoreScreen) {
}
function SaveGame(saveFileName) {
}
function RenameSaveDirectory(sourceDirName, destDirName) {
}
function RemoveSaveDirectory(sourceDirName) {
}
function CopySaveGame(sourceSaveName, destSaveName) {
}
function SaveGameExists(saveName) {
}
function SyncSelections() {
}
function SetFloatGameState(whichFloatGameState, value) {
}
function GetFloatGameState(whichFloatGameState) {
}
function SetIntegerGameState(whichIntegerGameState, value) {
}
function GetIntegerGameState(whichIntegerGameState) {
}




function SetTutorialCleared(cleared) {
}
function SetMissionAvailable(campaignNumber, missionNumber, available) {
}
function SetCampaignAvailable(campaignNumber, available) {
}
function SetOpCinematicAvailable(campaignNumber, available) {
}
function SetEdCinematicAvailable(campaignNumber, available) {
}
function GetDefaultDifficulty() {
}
function SetDefaultDifficulty(g) {
}
function SetCustomCampaignButtonVisible(whichButton, visible) {
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
function DialogSetMessage(whichDialog, messageText) {
}
function DialogAddButton(whichDialog, buttonText, hotkey) {
}
function DialogAddQuitButton(whichDialog, doScoreScreen, buttonText, hotkey) {
}
function DialogDisplay(whichPlayer, whichDialog, flag) {
}




function ReloadGameCachesFromDisk() {
}

function InitGameCache(campaignFile) {
    return {};
}
function SaveGameCache(whichCache) {
}

function StoreInteger(cache, missionKey, key, value) {
    if (cache) {
        cache[missionKey] = cache[missionKey] || {};
        cache[missionKey][key] = value;
    }
}
function StoreReal(cache, missionKey, key, value) {
    StoreInteger(cache, missionKey, key, value)
}
function StoreBoolean(cache, missionKey, key, value) {
    StoreInteger(cache, missionKey, key, value)
}
function StoreUnit(cache, missionKey, key, whichUnit) {
    StoreInteger(cache, missionKey, key, value)
}
function StoreString(cache, missionKey, key, value) {
    StoreInteger(cache, missionKey, key, value)
}

function SyncStoredInteger(cache, missionKey, key) {
}
function SyncStoredReal(cache, missionKey, key) {
}
function SyncStoredBoolean(cache, missionKey, key) {
}
function SyncStoredUnit(cache, missionKey, key) {
}
function SyncStoredString(cache, missionKey, key) {
}

function HaveStoredInteger(cache, missionKey, key) {
}
function HaveStoredReal(cache, missionKey, key) {
}
function HaveStoredBoolean(cache, missionKey, key) {
}
function HaveStoredUnit(cache, missionKey, key) {
}
function HaveStoredString(cache, missionKey, key) {
}

function FlushGameCache(cache) {
}
function FlushStoredMission(cache, missionKey) {
}
function FlushStoredInteger(cache, missionKey, key) {
}
function FlushStoredReal(cache, missionKey, key) {
}
function FlushStoredBoolean(cache, missionKey, key) {
}
function FlushStoredUnit(cache, missionKey, key) {
}
function FlushStoredString(cache, missionKey, key) {
}


function GetStoredInteger(cache, missionKey, key) {
    return GetStoredReal(cache, missionKey, key) || 0;
}
function GetStoredReal(cache, missionKey, key) {
    if (cache && cache[missionKey] && cache[missionKey][key]) {
        return cache[missionKey][key];
    }
}
function GetStoredBoolean(cache, missionKey, key) {
    return GetStoredReal(cache, missionKey, key) || false;
}
function GetStoredString(cache, missionKey, key) {
    if (cache && cache[missionKey] && cache[missionKey][key]) {
        return cache[missionKey][key];
    }
    return GetStoredReal(cache, missionKey, key) || "";
}
function RestoreUnit(cache, missionKey, key, forWhichPlayer, x, y, facing) {
}


function InitHashtable() {
    return {};
}

function SaveInteger(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveReal(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveBoolean(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveStr(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SavePlayerHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveWidgetHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveDestructableHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveItemHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveUnitHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveAbilityHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveTimerHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveTriggerHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveTriggerConditionHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveTriggerActionHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveTriggerEventHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveForceHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveGroupHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveLocationHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveRectHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveBooleanExprHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveSoundHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveEffectHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveUnitPoolHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveItemPoolHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveQuestHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveQuestItemHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveDefeatConditionHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveTimerDialogHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveLeaderboardHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveMultiboardHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveMultiboardItemHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveTrackableHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveDialogHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveButtonHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveTextTagHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveLightningHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveImageHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveUbersplatHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveRegionHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveFogStateHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveFogModifierHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveAgentHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}
function SaveHashtableHandle(table, parentKey, childKey, value) {
    StoreInteger(table, parentKey, childKey, value);
}


function LoadInteger(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadReal(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadBoolean(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadStr(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadPlayerHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadWidgetHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadDestructableHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadItemHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadUnitHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadAbilityHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadTimerHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadTriggerHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadTriggerConditionHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadTriggerActionHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadTriggerEventHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadForceHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadGroupHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadLocationHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadRectHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadBooleanExprHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadSoundHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadEffectHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadUnitPoolHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadItemPoolHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadQuestHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadQuestItemHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadDefeatConditionHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadTimerDialogHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadLeaderboardHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadMultiboardHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadMultiboardItemHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadTrackableHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadDialogHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadButtonHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadTextTagHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadLightningHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadImageHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadUbersplatHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadRegionHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadFogStateHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadFogModifierHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}
function LoadHashtableHandle(table, parentKey, childKey) {
    return GetStoredReal(table, parentKey, childKey);
}

function HaveSavedInteger(table, parentKey, childKey) {
}
function HaveSavedReal(table, parentKey, childKey) {
}
function HaveSavedBoolean(table, parentKey, childKey) {
}
function HaveSavedString(table, parentKey, childKey) {
}
function HaveSavedHandle(table, parentKey, childKey) {
}

function RemoveSavedInteger(table, parentKey, childKey) {
}
function RemoveSavedReal(table, parentKey, childKey) {
}
function RemoveSavedBoolean(table, parentKey, childKey) {
}
function RemoveSavedString(table, parentKey, childKey) {
}
function RemoveSavedHandle(table, parentKey, childKey) {
}

function FlushParentHashtable(table) {
}
function FlushChildHashtable(table, parentKey) {
}




function GetRandomInt(lowBound, highBound) {
    return Math.floor(Math.random() * (highBound - lowBound + 1)) + lowBound;
}
function GetRandomReal(lowBound, highBound) {
    return Math.random() * (highBound - lowBound) + lowBound;
}

function CreateUnitPool() {
}
function DestroyUnitPool(whichPool) {
}
function UnitPoolAddUnitType(whichPool, unitId, weight) {
}
function UnitPoolRemoveUnitType(whichPool, unitId) {
}
function PlaceRandomUnit(whichPool, forWhichPlayer, x, y, facing) {
}

function CreateItemPool() {
}
function DestroyItemPool(whichItemPool) {
}
function ItemPoolAddItemType(whichItemPool, itemId, weight) {
}
function ItemPoolRemoveItemType(whichItemPool, itemId) {
}
function PlaceRandomItem(whichItemPool, x, y) {
}


function ChooseRandomCreep(level) {
}
function ChooseRandomNPBuilding() {
}
function ChooseRandomItem(level) {
}
function ChooseRandomItemEx(whichType, level) {
}
function SetRandomSeed(seed) {
}



function SetTerrainFog(a, b, c, d, e) {
}
function ResetTerrainFog() {
}

function SetUnitFog(a, b, c, d, e) {
}
function SetTerrainFogEx(style, zstart, zend, density, red, green, blue) {
}
function DisplayTextToPlayer(toPlayer, x, y, message) {
    console.log(message);
}
function DisplayTimedTextToPlayer(toPlayer, x, y, duration, message) {
    console.log(message);
}
function DisplayTimedTextFromPlayer(toPlayer, x, y, duration, message) {
}
function ClearTextMessages() {
}
function SetDayNightModels(terrainDNCFile, unitDNCFile) {
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
function ShowInterface(flag, fadeDuration) {
}
function PauseGame(flag) {
}
function UnitAddIndicator(whichUnit, red, green, blue, alpha) {
}
function AddIndicator(whichWidget, red, green, blue, alpha) {
}
function PingMinimap(x, y, duration) {
}
function PingMinimapEx(x, y, duration, red, green, blue, extraEffects) {
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
function SetTextTagText(t, s, height) {
}
function SetTextTagPos(t, x, y, heightOffset) {
}
function SetTextTagPosUnit(t, whichUnit, heightOffset) {
}
function SetTextTagColor(t, red, green, blue, alpha) {
}
function SetTextTagVelocity(t, xvel, yvel) {
}
function SetTextTagVisibility(t, flag) {
}
function SetTextTagSuspended(t, flag) {
}
function SetTextTagPermanent(t, flag) {
}
function SetTextTagAge(t, age) {
}
function SetTextTagLifespan(t, lifespan) {
}
function SetTextTagFadepoint(t, fadepoint) {
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
function EnableMinimapFilterButtons(enableAlly, enableCreep) {
}
function EnableDragSelect(state, ui) {
}
function EnablePreSelect(state, ui) {
}
function EnableSelect(state, ui) {
}



function CreateTrackable(trackableModelPath, x, y, facing) {
}



function CreateQuest() {
}
function DestroyQuest(whichQuest) {
}
function QuestSetTitle(whichQuest, title) {
}
function QuestSetDescription(whichQuest, description) {
}
function QuestSetIconPath(whichQuest, iconPath) {
}

function QuestSetRequired(whichQuest, required) {
}
function QuestSetCompleted(whichQuest, completed) {
}
function QuestSetDiscovered(whichQuest, discovered) {
}
function QuestSetFailed(whichQuest, failed) {
}
function QuestSetEnabled(whichQuest, enabled) {
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
function QuestItemSetDescription(whichQuestItem, description) {
}
function QuestItemSetCompleted(whichQuestItem, completed) {
}

function IsQuestItemCompleted(whichQuestItem) {
}

function CreateDefeatCondition() {
}
function DestroyDefeatCondition(whichCondition) {
}
function DefeatConditionSetDescription(whichCondition, description) {
}

function FlashQuestDialogButton() {
}
function ForceQuestDialogUpdate() {
}



function CreateTimerDialog(t) {
}
function DestroyTimerDialog(whichDialog) {
}
function TimerDialogSetTitle(whichDialog, title) {
}
function TimerDialogSetTitleColor(whichDialog, red, green, blue, alpha) {
}
function TimerDialogSetTimeColor(whichDialog, red, green, blue, alpha) {
}
function TimerDialogSetSpeed(whichDialog, speedMultFactor) {
}
function TimerDialogDisplay(whichDialog, display) {
}
function IsTimerDialogDisplayed(whichDialog) {
}
function TimerDialogSetRealTimeRemaining(whichDialog, timeRemaining) {
}





function CreateLeaderboard() {
}
function DestroyLeaderboard(lb) {
}

function LeaderboardDisplay(lb, show) {
}
function IsLeaderboardDisplayed(lb) {
}

function LeaderboardGetItemCount(lb) {
}

function LeaderboardSetSizeByItemCount(lb, count) {
}
function LeaderboardAddItem(lb, label, value, p) {
}
function LeaderboardRemoveItem(lb, index) {
}
function LeaderboardRemovePlayerItem(lb, p) {
}
function LeaderboardClear(lb) {
}

function LeaderboardSortItemsByValue(lb, ascending) {
}
function LeaderboardSortItemsByPlayer(lb, ascending) {
}
function LeaderboardSortItemsByLabel(lb, ascending) {
}

function LeaderboardHasPlayerItem(lb, p) {
}
function LeaderboardGetPlayerIndex(lb, p) {
}
function LeaderboardSetLabel(lb, label) {
}
function LeaderboardGetLabelText(lb) {
}

function PlayerSetLeaderboard(toPlayer, lb) {
}
function PlayerGetLeaderboard(toPlayer) {
}

function LeaderboardSetLabelColor(lb, red, green, blue, alpha) {
}
function LeaderboardSetValueColor(lb, red, green, blue, alpha) {
}
function LeaderboardSetStyle(lb, showLabel, showNames, showValues, showIcons) {
}

function LeaderboardSetItemValue(lb, whichItem, val) {
}
function LeaderboardSetItemLabel(lb, whichItem, val) {
}
function LeaderboardSetItemStyle(lb, whichItem, showLabel, showValue, showIcon) {
}
function LeaderboardSetItemLabelColor(lb, whichItem, red, green, blue, alpha) {
}
function LeaderboardSetItemValueColor(lb, whichItem, red, green, blue, alpha) {
}






function CreateMultiboard() {
}
function DestroyMultiboard(lb) {
}

function MultiboardDisplay(lb, show) {
}
function IsMultiboardDisplayed(lb) {
}

function MultiboardMinimize(lb, minimize) {
}
function IsMultiboardMinimized(lb) {
}
function MultiboardClear(lb) {
}

function MultiboardSetTitleText(lb, label) {
}
function MultiboardGetTitleText(lb) {
}
function MultiboardSetTitleTextColor(lb, red, green, blue, alpha) {
}

function MultiboardGetRowCount(lb) {
    return 0;
}
function MultiboardGetColumnCount(lb) {
    return 0;
}

function MultiboardSetColumnCount(lb, count) {
}
function MultiboardSetRowCount(lb, count) {
}


function MultiboardSetItemsStyle(lb, showValues, showIcons) {
}
function MultiboardSetItemsValue(lb, value) {
}
function MultiboardSetItemsValueColor(lb, red, green, blue, alpha) {
}
function MultiboardSetItemsWidth(lb, width) {
}
function MultiboardSetItemsIcon(lb, iconPath) {
}



function MultiboardGetItem(lb, row, column) {
}
function MultiboardReleaseItem(mbi) {
}

function MultiboardSetItemStyle(mbi, showValue, showIcon) {
}
function MultiboardSetItemValue(mbi, val) {
}
function MultiboardSetItemValueColor(mbi, red, green, blue, alpha) {
}
function MultiboardSetItemWidth(mbi, width) {
}
function MultiboardSetItemIcon(mbi, iconFileName) {
}




function MultiboardSuppressDisplay(flag) {
}



function SetCameraPosition(x, y) {
}
function SetCameraQuickPosition(x, y) {
}
function SetCameraBounds(x1, y1, x2, y2, x3, y3, x4, y4) {
}
function StopCamera() {
}
function ResetToGameCamera(duration) {
}
function PanCameraTo(x, y) {
}
function PanCameraToTimed(x, y, duration) {
}
function PanCameraToWithZ(x, y, zOffsetDest) {
}
function PanCameraToTimedWithZ(x, y, zOffsetDest, duration) {
}
function SetCinematicCamera(cameraModelFile) {
}
function SetCameraRotateMode(x, y, radiansToSweep, duration) {
}
function SetCameraField(whichField, value, duration) {
}
function AdjustCameraField(whichField, offset, duration) {
}
function SetCameraTargetController(whichUnit, xoffset, yoffset, inheritOrientation) {
}
function SetCameraOrientController(whichUnit, xoffset, yoffset) {
}

function CreateCameraSetup() {
}
function CameraSetupSetField(whichSetup, whichField, value, duration) {
}
function CameraSetupGetField(whichSetup, whichField) {
}
function CameraSetupSetDestPosition(whichSetup, x, y, duration) {
}
function CameraSetupGetDestPositionLoc(whichSetup) {
}
function CameraSetupGetDestPositionX(whichSetup) {
}
function CameraSetupGetDestPositionY(whichSetup) {
}
function CameraSetupApply(whichSetup, doPan, panTimed) {
}
function CameraSetupApplyWithZ(whichSetup, zDestOffset) {
}
function CameraSetupApplyForceDuration(whichSetup, doPan, forceDuration) {
}
function CameraSetupApplyForceDurationWithZ(whichSetup, zDestOffset, forceDuration) {
}

function CameraSetTargetNoise(mag, velocity) {
}
function CameraSetSourceNoise(mag, velocity) {
}

function CameraSetTargetNoiseEx(mag, velocity, vertOnly) {
}
function CameraSetSourceNoiseEx(mag, velocity, vertOnly) {
}

function CameraSetSmoothingFactor(factor) {
}

function SetCineFilterTexture(filename) {
}
function SetCineFilterBlendMode(whichMode) {
}
function SetCineFilterTexMapFlags(whichFlags) {
}
function SetCineFilterStartUV(minu, minv, maxu, maxv) {
}
function SetCineFilterEndUV(minu, minv, maxu, maxv) {
}
function SetCineFilterStartColor(red, green, blue, alpha) {
}
function SetCineFilterEndColor(red, green, blue, alpha) {
}
function SetCineFilterDuration(duration) {
}
function DisplayCineFilter(flag) {
}
function IsCineFilterDisplayed() {
}

function SetCinematicScene(portraitUnitId, color, speakerTitle, text, sceneDuration, voiceoverDuration) {
}
function EndCinematicScene() {
}
function ForceCinematicSubtitles(flag) {
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

function CreateSound(fileName, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate, eaxSetting) {
}
function CreateSoundFilenameWithLabel(fileName, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate, SLKEntryName) {
}
function CreateSoundFromLabel(soundLabel, looping, is3D, stopwhenoutofrange, fadeInRate, fadeOutRate) {
}
function CreateMIDISound(soundLabel, fadeInRate, fadeOutRate) {
}

function SetSoundParamsFromLabel(soundHandle, soundLabel) {
}
function SetSoundDistanceCutoff(soundHandle, cutoff) {
}
function SetSoundChannel(soundHandle, channel) {
}
function SetSoundVolume(soundHandle, volume) {
}
function SetSoundPitch(soundHandle, pitch) {
}


function SetSoundPlayPosition(soundHandle, millisecs) {
}


function SetSoundDistances(soundHandle, minDist, maxDist) {
}
function SetSoundConeAngles(soundHandle, inside, outside, outsideVolume) {
}
function SetSoundConeOrientation(soundHandle, x, y, z) {
}
function SetSoundPosition(soundHandle, x, y, z) {
}
function SetSoundVelocity(soundHandle, x, y, z) {
}
function AttachSoundToUnit(soundHandle, whichUnit) {
}

function StartSound(soundHandle) {
}
function StopSound(soundHandle, killWhenDone, fadeOut) {
}
function KillSoundWhenDone(soundHandle) {
}


function SetMapMusic(musicName, random, index) {
}
function ClearMapMusic() {
}

function PlayMusic(musicName) {
}
function PlayMusicEx(musicName, frommsecs, fadeinmsecs) {
}
function StopMusic(fadeOut) {
}
function ResumeMusic() {
}

function PlayThematicMusic(musicFileName) {
}
function PlayThematicMusicEx(musicFileName, frommsecs) {
}
function EndThematicMusic() {
}

function SetMusicVolume(volume) {
}
function SetMusicPlayPosition(millisecs) {
}
function SetThematicMusicPlayPosition(millisecs) {
}


function SetSoundDuration(soundHandle, duration) {
}
function GetSoundDuration(soundHandle) {
}
function GetSoundFileDuration(musicFileName) {
}

function VolumeGroupSetVolume(vgroup, scale) {
}
function VolumeGroupReset() {
}

function GetSoundIsPlaying(soundHandle) {
}
function GetSoundIsLoading(soundHandle) {
}

function RegisterStackedSound(soundHandle, byPosition, rectwidth, rectheight) {
}
function UnregisterStackedSound(soundHandle, byPosition, rectwidth, rectheight) {
}




function AddWeatherEffect(where, effectID) {
}
function RemoveWeatherEffect(whichEffect) {
}
function EnableWeatherEffect(whichEffect, enable) {
}

function TerrainDeformCrater(x, y, radius, depth, duration, permanent) {
}
function TerrainDeformRipple(x, y, radius, depth, duration, count, spaceWaves, timeWaves, radiusStartPct, limitNeg) {
}
function TerrainDeformWave(x, y, dirX, dirY, distance, speed, radius, depth, trailTime, count) {
}
function TerrainDeformRandom(x, y, radius, minDelta, maxDelta, duration, updateInterval) {
}
function TerrainDeformStop(deformation, duration) {
}
function TerrainDeformStopAll() {
}

function AddSpecialEffect(modelName, x, y) {
}
function AddSpecialEffectLoc(modelName, where) {
}
function AddSpecialEffectTarget(modelName, targetWidget, attachPointName) {
}
function DestroyEffect(whichEffect) {
}

function AddSpellEffect(abilityString, t, x, y) {
}
function AddSpellEffectLoc(abilityString, t, where) {
}
function AddSpellEffectById(abilityId, t, x, y) {
}
function AddSpellEffectByIdLoc(abilityId, t, where) {
}
function AddSpellEffectTarget(modelName, t, targetWidget, attachPoint) {
}
function AddSpellEffectTargetById(abilityId, t, targetWidget, attachPoint) {
}

function AddLightning(codeName, checkVisibility, x1, y1, x2, y2) {
}
function AddLightningEx(codeName, checkVisibility, x1, y1, z1, x2, y2, z2) {
}
function DestroyLightning(whichBolt) {
}
function MoveLightning(whichBolt, checkVisibility, x1, y1, x2, y2) {
}
function MoveLightningEx(whichBolt, checkVisibility, x1, y1, z1, x2, y2, z2) {
}
function GetLightningColorA(whichBolt) {
}
function GetLightningColorR(whichBolt) {
}
function GetLightningColorG(whichBolt) {
}
function GetLightningColorB(whichBolt) {
}
function SetLightningColor(whichBolt, r, g, b, a) {
}

function GetAbilityEffect(abilityString, t, index) {
}
function GetAbilityEffectById(abilityId, t, index) {
}
function GetAbilitySound(abilityString, t) {
}
function GetAbilitySoundById(abilityId, t) {
}




function GetTerrainCliffLevel(x, y) {
}
function SetWaterBaseColor(red, green, blue, alpha) {
}
function SetWaterDeforms(val) {
}
function GetTerrainType(x, y) {
}
function GetTerrainVariance(x, y) {
}
function SetTerrainType(x, y, terrainType, variation, area, shape) {
}
function IsTerrainPathable(x, y, t) {
}
function SetTerrainPathable(x, y, t, flag) {
}




function CreateImage(file, sizeX, sizeY, sizeZ, posX, posY, posZ, originX, originY, originZ, imageType) {
}
function DestroyImage(whichImage) {
}
function ShowImage(whichImage, flag) {
}
function SetImageConstantHeight(whichImage, flag, height) {
}
function SetImagePosition(whichImage, x, y, z) {
}
function SetImageColor(whichImage, red, green, blue, alpha) {
}
function SetImageRender(whichImage, flag) {
}
function SetImageRenderAlways(whichImage, flag) {
}
function SetImageAboveWater(whichImage, flag, useWaterAlpha) {
}
function SetImageType(whichImage, imageType) {
}




function CreateUbersplat(x, y, name, red, green, blue, alpha, forcePaused, noBirthTime) {
}
function DestroyUbersplat(whichSplat) {
}
function ResetUbersplat(whichSplat) {
}
function FinishUbersplat(whichSplat) {
}
function ShowUbersplat(whichSplat, flag) {
}
function SetUbersplatRender(whichSplat, flag) {
}
function SetUbersplatRenderAlways(whichSplat, flag) {
}




function SetBlight(whichPlayer, x, y, radius, addBlight) {
}
function SetBlightRect(whichPlayer, r, addBlight) {
}
function SetBlightPoint(whichPlayer, x, y, addBlight) {
}
function SetBlightLoc(whichPlayer, whichLocation, radius, addBlight) {
}
function CreateBlightedGoldmine(id, x, y, face) {
}
function IsPointBlighted(x, y) {
}




function SetDoodadAnimation(x, y, radius, doodadID, nearestOnly, animName, animRandom) {
}
function SetDoodadAnimationRect(r, doodadID, animName, animRandom) {
}




function StartMeleeAI(num, script) {
}
function StartCampaignAI(num, script) {
}
function CommandAI(num, command, data) {
}
function PauseCompAI(p, pause) {
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
    console.log("Preload: " + filename);
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
function BlzSetAbilityTooltip(abilCode, tooltip, level) {
}
function BlzSetAbilityActivatedTooltip(abilCode, tooltip, level) {
}
function BlzSetAbilityExtendedTooltip(abilCode, extendedTooltip, level) {
}
function BlzSetAbilityActivatedExtendedTooltip(abilCode, extendedTooltip, level) {
}
function BlzSetAbilityResearchTooltip(abilCode, researchTooltip, level) {
}
function BlzSetAbilityResearchExtendedTooltip(abilCode, researchExtendedTooltip, level) {
}
function BlzGetAbilityTooltip(abilCode, level) {
    return abilCode;
}
function BlzGetAbilityActivatedTooltip(abilCode, level) {
}
function BlzGetAbilityExtendedTooltip(abilCode, level) {
}
function BlzGetAbilityActivatedExtendedTooltip(abilCode, level) {
}
function BlzGetAbilityResearchTooltip(abilCode, level) {
}
function BlzGetAbilityResearchExtendedTooltip(abilCode, level) {
}
function BlzSetAbilityIcon(abilCode, iconPath) {
}
function BlzGetAbilityIcon(abilCode) {
}
function BlzSetAbilityActivatedIcon(abilCode, iconPath) {
}
function BlzGetAbilityActivatedIcon(abilCode) {
}
function BlzGetAbilityPosX(abilCode) {
}
function BlzGetAbilityPosY(abilCode) {
}
function BlzSetAbilityPosX(abilCode, x) {
}
function BlzSetAbilityPosY(abilCode, y) {
}
function BlzGetAbilityActivatedPosX(abilCode) {
}
function BlzGetAbilityActivatedPosY(abilCode) {
}
function BlzSetAbilityActivatedPosX(abilCode, x) {
}
function BlzSetAbilityActivatedPosY(abilCode, y) {
}
function BlzGetUnitMaxHP(whichUnit) {
}
function BlzSetUnitMaxHP(whichUnit, hp) {
}
function BlzGetUnitMaxMana(whichUnit) {
}
function BlzSetUnitMaxMana(whichUnit, mana) {
}
function BlzSetItemName(whichItem, name) {
}
function BlzSetItemDescription(whichItem, description) {
}
function BlzGetItemDescription(whichItem) {
}
function BlzSetItemTooltip(whichItem, tooltip) {
}
function BlzGetItemTooltip(whichItem) {
}
function BlzSetItemExtendedTooltip(whichItem, extendedTooltip) {
}
function BlzGetItemExtendedTooltip(whichItem) {
}
function BlzSetItemIconPath(whichItem, iconPath) {
}
function BlzGetItemIconPath(whichItem) {
}
function BlzSetUnitName(whichUnit, name) {
}
function BlzSetHeroProperName(whichUnit, heroProperName) {
}
function BlzGetUnitBaseDamage(whichUnit, weaponIndex) {
}
function BlzSetUnitBaseDamage(whichUnit, baseDamage, weaponIndex) {
}
function BlzGetUnitDiceNumber(whichUnit, weaponIndex) {
}
function BlzSetUnitDiceNumber(whichUnit, diceNumber, weaponIndex) {
}
function BlzGetUnitDiceSides(whichUnit, weaponIndex) {
}
function BlzSetUnitDiceSides(whichUnit, diceSides, weaponIndex) {
}
function BlzGetUnitAttackCooldown(whichUnit, weaponIndex) {
}
function BlzSetUnitAttackCooldown(whichUnit, cooldown, weaponIndex) {
}
function BlzSetSpecialEffectColorByPlayer(whichEffect, whichPlayer) {
}
function BlzSetSpecialEffectColor(whichEffect, r, g, b) {
}
function BlzSetSpecialEffectAlpha(whichEffect, alpha) {
}
function BlzSetSpecialEffectScale(whichEffect, scale) {
}
function BlzSetSpecialEffectPosition(whichEffect, x, y, z) {
}
function BlzSetSpecialEffectHeight(whichEffect, height) {
}
function BlzSetSpecialEffectTimeScale(whichEffect, timeScale) {
}
function BlzSetSpecialEffectTime(whichEffect, time) {
}
function BlzSetSpecialEffectOrientation(whichEffect, yaw, pitch, roll) {
}
function BlzSetSpecialEffectYaw(whichEffect, yaw) {
}
function BlzSetSpecialEffectPitch(whichEffect, pitch) {
}
function BlzSetSpecialEffectRoll(whichEffect, roll) {
}
function BlzSetSpecialEffectX(whichEffect, x) {
}
function BlzSetSpecialEffectY(whichEffect, y) {
}
function BlzSetSpecialEffectZ(whichEffect, z) {
}
function BlzSetSpecialEffectPositionLoc(whichEffect, loc) {
}
function BlzGetLocalSpecialEffectX(whichEffect) {
}
function BlzGetLocalSpecialEffectY(whichEffect) {
}
function BlzGetLocalSpecialEffectZ(whichEffect) {
}
function BlzSpecialEffectClearSubAnimations(whichEffect) {
}
function BlzSpecialEffectRemoveSubAnimation(whichEffect, whichSubAnim) {
}
function BlzSpecialEffectAddSubAnimation(whichEffect, whichSubAnim) {
}
function BlzPlaySpecialEffect(whichEffect, whichAnim) {
}
function BlzPlaySpecialEffectWithTimeScale(whichEffect, whichAnim, timeScale) {
}
function BlzGetAnimName(whichAnim) {
}
function BlzGetUnitArmor(whichUnit) {
}
function BlzSetUnitArmor(whichUnit, armorAmount) {
}
function BlzUnitHideAbility(whichUnit, abilId, flag) {
}
function BlzUnitDisableAbility(whichUnit, abilId, flag, hideUI) {
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
function BlzGetAbilityManaCost(abilId, level) {
}
function BlzGetAbilityCooldown(abilId, level) {
}
function BlzSetUnitAbilityCooldown(whichUnit, abilId, level, cooldown) {
}
function BlzGetUnitAbilityCooldown(whichUnit, abilId, level) {
}
function BlzGetUnitAbilityCooldownRemaining(whichUnit, abilId) {
}
function BlzEndUnitAbilityCooldown(whichUnit, abilCode) {
}
function BlzStartUnitAbilityCooldown(whichUnit, abilCode, cooldown) {
}
function BlzGetUnitAbilityManaCost(whichUnit, abilId, level) {
}
function BlzSetUnitAbilityManaCost(whichUnit, abilId, level, manaCost) {
}
function BlzGetLocalUnitZ(whichUnit) {
}
function BlzDecPlayerTechResearched(whichPlayer, techid, levels) {
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
function RequestExtraIntegerData(dataType, whichPlayer, param1, param2, param3, param4, param5, param6) {
}
function RequestExtraBooleanData(dataType, whichPlayer, param1, param2, param3, param4, param5, param6) {
}
function RequestExtraStringData(dataType, whichPlayer, param1, param2, param3, param4, param5, param6) {
}
function RequestExtraRealData(dataType, whichPlayer, param1, param2, param3, param4, param5, param6) {
}
function BlzGetUnitZ(whichUnit) {
}
function BlzEnableSelections(enableSelection, enableSelectionCircle) {
}
function BlzIsSelectionEnabled() {
}
function BlzIsSelectionCircleEnabled() {
}
function BlzCameraSetupApplyForceDurationSmooth(whichSetup, doPan, forcedDuration, easeInDuration, easeOutDuration, smoothFactor) {
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
function BlzShowUnitTeamGlow(whichUnit, show) {
}
function BlzGetOriginFrame(frameType, index) {
}
function BlzEnableUIAutoPosition(enable) {
}
function BlzHideOriginFrames(enable) {
}
function BlzConvertColor(a, r, g, b) {
}
function BlzLoadTOCFile(TOCFile) {
}
function BlzCreateFrame(name, owner, priority, createContext) {
}
function BlzCreateSimpleFrame(name, owner, createContext) {
}
function BlzCreateFrameByType(typeName, name, owner, inherits, createContext) {
}
function BlzDestroyFrame(frame) {
}
function BlzFrameSetPoint(frame, point, relative, relativePoint, x, y) {
}
function BlzFrameSetAbsPoint(frame, point, x, y) {
}
function BlzFrameClearAllPoints(frame) {
}
function BlzFrameSetAllPoints(frame, relative) {
}
function BlzFrameSetVisible(frame, visible) {
}
function BlzFrameIsVisible(frame) {
}
function BlzGetFrameByName(name, createContext) {
}
function BlzFrameGetName(frame) {
}
function BlzFrameClick(frame) {
}
function BlzFrameSetText(frame, text) {
}
function BlzFrameGetText(frame) {
}
function BlzFrameAddText(frame, text) {
}
function BlzFrameSetTextSizeLimit(frame, size) {
}
function BlzFrameGetTextSizeLimit(frame) {
}
function BlzFrameSetTextColor(frame, color) {
}
function BlzFrameSetFocus(frame, flag) {
}
function BlzFrameSetModel(frame, modelFile, cameraIndex) {
}
function BlzFrameSetEnable(frame, enabled) {
}
function BlzFrameGetEnable(frame) {
}
function BlzFrameSetAlpha(frame, alpha) {
}
function BlzFrameGetAlpha(frame) {
}
function BlzFrameSetSpriteAnimate(frame, primaryProp, flags) {
}
function BlzFrameSetTexture(frame, texFile, flag, blend) {
}
function BlzFrameSetScale(frame, scale) {
}
function BlzFrameSetTooltip(frame, tooltip) {
}
function BlzFrameCageMouse(frame, enable) {
}
function BlzFrameSetValue(frame, value) {
}
function BlzFrameGetValue(frame) {
}
function BlzFrameSetMinMaxValue(frame, minValue, maxValue) {
}
function BlzFrameSetStepSize(frame, stepSize) {
}
function BlzFrameSetSize(frame, width, height) {
}
function BlzFrameSetVertexColor(frame, color) {
}
function BlzFrameSetLevel(frame, level) {
}
function BlzFrameSetParent(frame, parent) {
}
function BlzFrameGetParent(frame) {
}
function BlzFrameGetHeight(frame) {
}
function BlzFrameGetWidth(frame) {
}
function BlzFrameSetFont(frame, fileName, height, flags) {
}
function BlzFrameSetTextAlignment(frame, vert, horz) {
}
function BlzTriggerRegisterFrameEvent(whichTrigger, frame, eventId) {
}
function BlzGetTriggerFrame() {
}
function BlzGetTriggerFrameEvent() {
}
function BlzGetTriggerFrameValue() {
}
function BlzGetTriggerFrameText() {
}
function BlzTriggerRegisterPlayerSyncEvent(whichTrigger, whichPlayer, prefix, fromServer) {
}
function BlzSendSyncData(prefix, data) {
}
function BlzGetTriggerSyncPrefix() {
}
function BlzGetTriggerSyncData() {
}
function BlzTriggerRegisterPlayerKeyEvent(whichTrigger, whichPlayer, key, metaKey, keyDown) {
}
function BlzGetTriggerPlayerKey() {
}
function BlzGetTriggerPlayerMetaKey() {
}
function BlzGetTriggerPlayerIsKeyDown() {
}
function BlzEnableCursor(enable) {
}
function BlzSetMousePos(x, y) {
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
function BlzSetSpecialEffectMatrixScale(whichEffect, x, y, z) {
}
function BlzResetSpecialEffectMatrix(whichEffect) {
}
function BlzGetUnitAbility(whichUnit, abilId) {
}
function BlzGetUnitAbilityByIndex(whichUnit, index) {
}
function BlzDisplayChatMessage(whichPlayer, recipient, message) {
}
function BlzPauseUnitEx(whichUnit, flag) {
}
function BlzSetUnitFacingEx(whichUnit, facingAngle) {
}
function CreateCommandButtonEffect(abilityId, order) {
}
function CreateUpgradeCommandButtonEffect(whichUprgade) {
}
function CreateLearnCommandButtonEffect(abilityId) {
}
function DestroyCommandButtonEffect(whichEffect) {
}
function BlzBitOr(x, y) {
}
function BlzBitAnd(x, y) {
}
function BlzBitXor(x, y) {
}
function BlzGetAbilityBooleanField(whichAbility, whichField) {
}
function BlzGetAbilityIntegerField(whichAbility, whichField) {
}
function BlzGetAbilityRealField(whichAbility, whichField) {
}
function BlzGetAbilityStringField(whichAbility, whichField) {
}
function BlzGetAbilityBooleanLevelField(whichAbility, whichField, level) {
}
function BlzGetAbilityIntegerLevelField(whichAbility, whichField, level) {
}
function BlzGetAbilityRealLevelField(whichAbility, whichField, level) {
}
function BlzGetAbilityStringLevelField(whichAbility, whichField, level) {
}
function BlzGetAbilityBooleanLevelArrayField(whichAbility, whichField, level, index) {
}
function BlzGetAbilityIntegerLevelArrayField(whichAbility, whichField, level, index) {
}
function BlzGetAbilityRealLevelArrayField(whichAbility, whichField, level, index) {
}
function BlzGetAbilityStringLevelArrayField(whichAbility, whichField, level, index) {
}
function BlzSetAbilityBooleanField(whichAbility, whichField, value) {
}
function BlzSetAbilityIntegerField(whichAbility, whichField, value) {
}
function BlzSetAbilityRealField(whichAbility, whichField, value) {
}
function BlzSetAbilityStringField(whichAbility, whichField, value) {
}
function BlzSetAbilityBooleanLevelField(whichAbility, whichField, level, value) {
}
function BlzSetAbilityIntegerLevelField(whichAbility, whichField, level, value) {
}
function BlzSetAbilityRealLevelField(whichAbility, whichField, level, value) {
}
function BlzSetAbilityStringLevelField(whichAbility, whichField, level, value) {
}
function BlzSetAbilityBooleanLevelArrayField(whichAbility, whichField, level, index, value) {
}
function BlzSetAbilityIntegerLevelArrayField(whichAbility, whichField, level, index, value) {
}
function BlzSetAbilityRealLevelArrayField(whichAbility, whichField, level, index, value) {
}
function BlzSetAbilityStringLevelArrayField(whichAbility, whichField, level, index, value) {
}
function BlzAddAbilityBooleanLevelArrayField(whichAbility, whichField, level, value) {
}
function BlzAddAbilityIntegerLevelArrayField(whichAbility, whichField, level, value) {
}
function BlzAddAbilityRealLevelArrayField(whichAbility, whichField, level, value) {
}
function BlzAddAbilityStringLevelArrayField(whichAbility, whichField, level, value) {
}
function BlzRemoveAbilityBooleanLevelArrayField(whichAbility, whichField, level, value) {
}
function BlzRemoveAbilityIntegerLevelArrayField(whichAbility, whichField, level, value) {
}
function BlzRemoveAbilityRealLevelArrayField(whichAbility, whichField, level, value) {
}
function BlzRemoveAbilityStringLevelArrayField(whichAbility, whichField, level, value) {
}
function BlzGetItemAbilityByIndex(whichItem, index) {
}
function BlzGetItemAbility(whichItem, abilCode) {
}
function BlzItemAddAbility(whichItem, abilCode) {
}
function BlzGetItemBooleanField(whichItem, whichField) {
}
function BlzGetItemIntegerField(whichItem, whichField) {
}
function BlzGetItemRealField(whichItem, whichField) {
}
function BlzGetItemStringField(whichItem, whichField) {
}
function BlzSetItemBooleanField(whichItem, whichField, value) {
}
function BlzSetItemIntegerField(whichItem, whichField, value) {
}
function BlzSetItemRealField(whichItem, whichField, value) {
}
function BlzSetItemStringField(whichItem, whichField, value) {
}
function BlzItemRemoveAbility(whichItem, abilCode) {
}
function BlzGetUnitBooleanField(whichUnit, whichField) {
}
function BlzGetUnitIntegerField(whichUnit, whichField) {
}
function BlzGetUnitRealField(whichUnit, whichField) {
}
function BlzGetUnitStringField(whichUnit, whichField) {
}
function BlzSetUnitBooleanField(whichUnit, whichField, value) {
}
function BlzSetUnitIntegerField(whichUnit, whichField, value) {
}
function BlzSetUnitRealField(whichUnit, whichField, value) {
}
function BlzSetUnitStringField(whichUnit, whichField, value) {
}
function BlzGetUnitWeaponBooleanField(whichUnit, whichField, index) {
}
function BlzGetUnitWeaponIntegerField(whichUnit, whichField, index) {
}
function BlzGetUnitWeaponRealField(whichUnit, whichField, index) {
}
function BlzGetUnitWeaponStringField(whichUnit, whichField, index) {
}
function BlzSetUnitWeaponBooleanField(whichUnit, whichField, index, value) {
}
function BlzSetUnitWeaponIntegerField(whichUnit, whichField, index, value) {
}
function BlzSetUnitWeaponRealField(whichUnit, whichField, index, value) {
}
function BlzSetUnitWeaponStringField(whichUnit, whichField, index, value) {
}
function BlzGetUnitSkin(whichUnit) {
}
function BlzGetItemSkin(whichItem) {
}
function BlzSetUnitSkin(whichUnit, skinId) {
}
function BlzSetItemSkin(whichItem, skinId) {
}
function BlzCreateItemWithSkin(itemid, x, y, skinId) {
}
function BlzCreateUnitWithSkin(id, unitid, x, y, face, skinId) {
    return CreateUnit(id, unitid, x, y, face);
}
function BlzCreateDestructableWithSkin(objectid, x, y, face, scale, variation, skinId) {
}
function BlzCreateDestructableZWithSkin(objectid, x, y, z, face, scale, variation, skinId) {
}
function BlzCreateDeadDestructableWithSkin(objectid, x, y, face, scale, variation, skinId) {
}
function BlzCreateDeadDestructableZWithSkin(objectid, x, y, z, face, scale, variation, skinId) {
}
function BlzGetPlayerTownHallCount(whichPlayer) {
}
