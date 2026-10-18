---
type: vendor-doc
source_repo: "https://github.com/bakster55/JassToJsParser_CS"
source_path: "JassToCsMain/War3Map/blizzard.js"
ref: HEAD
---

var bj_PI = 3.14159
var bj_E = 2.71828
var bj_CELLWIDTH = 128.0
var bj_CLIFFHEIGHT = 128.0
var bj_UNIT_FACING = 270.0
var bj_RADTODEG = 180.0 / bj_PI
var bj_DEGTORAD = bj_PI / 180.0
var bj_TEXT_DELAY_QUEST = 20.00
var bj_TEXT_DELAY_QUESTUPDATE = 20.00
var bj_TEXT_DELAY_QUESTDONE = 20.00
var bj_TEXT_DELAY_QUESTFAILED = 20.00
var bj_TEXT_DELAY_QUESTREQUIREMENT = 20.00
var bj_TEXT_DELAY_MISSIONFAILED = 20.00
var bj_TEXT_DELAY_ALWAYSHINT = 12.00
var bj_TEXT_DELAY_HINT = 12.00
var bj_TEXT_DELAY_SECRET = 10.00
var bj_TEXT_DELAY_UNITACQUIRED = 15.00
var bj_TEXT_DELAY_UNITAVAILABLE = 10.00
var bj_TEXT_DELAY_ITEMACQUIRED = 10.00
var bj_TEXT_DELAY_WARNING = 12.00
var bj_QUEUE_DELAY_QUEST = 5.00
var bj_QUEUE_DELAY_HINT = 5.00
var bj_QUEUE_DELAY_SECRET = 3.00
var bj_HANDICAP_EASY = 60.00
var bj_HANDICAP_NORMAL = 90.00
var bj_HANDICAPDAMAGE_EASY = 50.00
var bj_HANDICAPDAMAGE_NORMAL = 90.00
var bj_HANDICAPREVIVE_NOTHARD = 50.00
var bj_GAME_STARTED_THRESHOLD = 0.01
var bj_WAIT_FOR_COND_MIN_INTERVAL = 0.10
var bj_POLLED_WAIT_INTERVAL = 0.10
var bj_POLLED_WAIT_SKIP_THRESHOLD = 2.00
var bj_MAX_INVENTORY = 6
var bj_MAX_PLAYERS = GetBJMaxPlayers()
var bj_PLAYER_NEUTRAL_VICTIM = GetBJPlayerNeutralVictim()
var bj_PLAYER_NEUTRAL_EXTRA = GetBJPlayerNeutralExtra()
var bj_MAX_PLAYER_SLOTS = GetBJMaxPlayerSlots()
var bj_MAX_SKELETONS = 25
var bj_MAX_STOCK_ITEM_SLOTS = 11
var bj_MAX_STOCK_UNIT_SLOTS = 11
var bj_MAX_ITEM_LEVEL = 10
var bj_MAX_CHECKPOINTS = 5
var bj_TOD_DAWN = 6.00
var bj_TOD_DUSK = 18.00
var bj_MELEE_STARTING_TOD = 8.00
var bj_MELEE_STARTING_GOLD_V0 = 750
var bj_MELEE_STARTING_GOLD_V1 = 500
var bj_MELEE_STARTING_LUMBER_V0 = 200
var bj_MELEE_STARTING_LUMBER_V1 = 150
var bj_MELEE_STARTING_HERO_TOKENS = 1
var bj_MELEE_HERO_LIMIT = 3
var bj_MELEE_HERO_TYPE_LIMIT = 1
var bj_MELEE_MINE_SEARCH_RADIUS = 2000
var bj_MELEE_CLEAR_UNITS_RADIUS = 1500
var bj_MELEE_CRIPPLE_TIMEOUT = 120.00
var bj_MELEE_CRIPPLE_MSG_DURATION = 20.00
var bj_MELEE_MAX_TWINKED_HEROES_V0 = 3
var bj_MELEE_MAX_TWINKED_HEROES_V1 = 1
var bj_CREEP_ITEM_DELAY = 0.50
var bj_STOCK_RESTOCK_INITIAL_DELAY = 120
var bj_STOCK_RESTOCK_INTERVAL = 30
var bj_STOCK_MAX_ITERATIONS = 20
var bj_MAX_DEST_IN_REGION_EVENTS = 64
var bj_CAMERA_MIN_FARZ = 100
var bj_CAMERA_DEFAULT_DISTANCE = 1650
var bj_CAMERA_DEFAULT_FARZ = 5000
var bj_CAMERA_DEFAULT_AOA = 304
var bj_CAMERA_DEFAULT_FOV = 70
var bj_CAMERA_DEFAULT_ROLL = 0
var bj_CAMERA_DEFAULT_ROTATION = 90
var bj_RESCUE_PING_TIME = 2.00
var bj_NOTHING_SOUND_DURATION = 5.00
var bj_TRANSMISSION_PING_TIME = 1.00
var bj_TRANSMISSION_IND_RED = 255
var bj_TRANSMISSION_IND_BLUE = 255
var bj_TRANSMISSION_IND_GREEN = 255
var bj_TRANSMISSION_IND_ALPHA = 255
var bj_TRANSMISSION_PORT_HANGTIME = 1.50
var bj_CINEMODE_INTERFACEFADE = 0.50
var bj_CINEMODE_GAMESPEED = MAP_SPEED_NORMAL
var bj_CINEMODE_VOLUME_UNITMOVEMENT = 0.40
var bj_CINEMODE_VOLUME_UNITSOUNDS = 0.00
var bj_CINEMODE_VOLUME_COMBAT = 0.40
var bj_CINEMODE_VOLUME_SPELLS = 0.40
var bj_CINEMODE_VOLUME_UI = 0.00
var bj_CINEMODE_VOLUME_MUSIC = 0.55
var bj_CINEMODE_VOLUME_AMBIENTSOUNDS = 1.00
var bj_CINEMODE_VOLUME_FIRE = 0.60
var bj_SPEECH_VOLUME_UNITMOVEMENT = 0.25
var bj_SPEECH_VOLUME_UNITSOUNDS = 0.00
var bj_SPEECH_VOLUME_COMBAT = 0.25
var bj_SPEECH_VOLUME_SPELLS = 0.25
var bj_SPEECH_VOLUME_UI = 0.00
var bj_SPEECH_VOLUME_MUSIC = 0.55
var bj_SPEECH_VOLUME_AMBIENTSOUNDS = 1.00
var bj_SPEECH_VOLUME_FIRE = 0.60
var bj_SMARTPAN_TRESHOLD_PAN = 500
var bj_SMARTPAN_TRESHOLD_SNAP = 3500
var bj_MAX_QUEUED_TRIGGERS = 100
var bj_QUEUED_TRIGGER_TIMEOUT = 180.00
var bj_CAMPAIGN_INDEX_T = 0
var bj_CAMPAIGN_INDEX_H = 1
var bj_CAMPAIGN_INDEX_U = 2
var bj_CAMPAIGN_INDEX_O = 3
var bj_CAMPAIGN_INDEX_N = 4
var bj_CAMPAIGN_INDEX_XN = 5
var bj_CAMPAIGN_INDEX_XH = 6
var bj_CAMPAIGN_INDEX_XU = 7
var bj_CAMPAIGN_INDEX_XO = 8
var bj_CAMPAIGN_OFFSET_T = 0
var bj_CAMPAIGN_OFFSET_H = 1
var bj_CAMPAIGN_OFFSET_U = 2
var bj_CAMPAIGN_OFFSET_O = 3
var bj_CAMPAIGN_OFFSET_N = 4
var bj_CAMPAIGN_OFFSET_XN = 5
var bj_CAMPAIGN_OFFSET_XH = 6
var bj_CAMPAIGN_OFFSET_XU = 7
var bj_CAMPAIGN_OFFSET_XO = 8
var bj_MISSION_INDEX_T00 = bj_CAMPAIGN_OFFSET_T * 1000 + 0
var bj_MISSION_INDEX_T01 = bj_CAMPAIGN_OFFSET_T * 1000 + 1
var bj_MISSION_INDEX_T02 = bj_CAMPAIGN_OFFSET_T * 1000 + 2
var bj_MISSION_INDEX_T03 = bj_CAMPAIGN_OFFSET_T * 1000 + 3
var bj_MISSION_INDEX_T04 = bj_CAMPAIGN_OFFSET_T * 1000 + 4
var bj_MISSION_INDEX_H00 = bj_CAMPAIGN_OFFSET_H * 1000 + 0
var bj_MISSION_INDEX_H01 = bj_CAMPAIGN_OFFSET_H * 1000 + 1
var bj_MISSION_INDEX_H02 = bj_CAMPAIGN_OFFSET_H * 1000 + 2
var bj_MISSION_INDEX_H03 = bj_CAMPAIGN_OFFSET_H * 1000 + 3
var bj_MISSION_INDEX_H04 = bj_CAMPAIGN_OFFSET_H * 1000 + 4
var bj_MISSION_INDEX_H05 = bj_CAMPAIGN_OFFSET_H * 1000 + 5
var bj_MISSION_INDEX_H06 = bj_CAMPAIGN_OFFSET_H * 1000 + 6
var bj_MISSION_INDEX_H07 = bj_CAMPAIGN_OFFSET_H * 1000 + 7
var bj_MISSION_INDEX_H08 = bj_CAMPAIGN_OFFSET_H * 1000 + 8
var bj_MISSION_INDEX_H09 = bj_CAMPAIGN_OFFSET_H * 1000 + 9
var bj_MISSION_INDEX_H10 = bj_CAMPAIGN_OFFSET_H * 1000 + 10
var bj_MISSION_INDEX_H11 = bj_CAMPAIGN_OFFSET_H * 1000 + 11
var bj_MISSION_INDEX_U00 = bj_CAMPAIGN_OFFSET_U * 1000 + 0
var bj_MISSION_INDEX_U01 = bj_CAMPAIGN_OFFSET_U * 1000 + 1
var bj_MISSION_INDEX_U02 = bj_CAMPAIGN_OFFSET_U * 1000 + 2
var bj_MISSION_INDEX_U03 = bj_CAMPAIGN_OFFSET_U * 1000 + 3
var bj_MISSION_INDEX_U05 = bj_CAMPAIGN_OFFSET_U * 1000 + 4
var bj_MISSION_INDEX_U07 = bj_CAMPAIGN_OFFSET_U * 1000 + 5
var bj_MISSION_INDEX_U08 = bj_CAMPAIGN_OFFSET_U * 1000 + 6
var bj_MISSION_INDEX_U09 = bj_CAMPAIGN_OFFSET_U * 1000 + 7
var bj_MISSION_INDEX_U10 = bj_CAMPAIGN_OFFSET_U * 1000 + 8
var bj_MISSION_INDEX_U11 = bj_CAMPAIGN_OFFSET_U * 1000 + 9
var bj_MISSION_INDEX_O00 = bj_CAMPAIGN_OFFSET_O * 1000 + 0
var bj_MISSION_INDEX_O01 = bj_CAMPAIGN_OFFSET_O * 1000 + 1
var bj_MISSION_INDEX_O02 = bj_CAMPAIGN_OFFSET_O * 1000 + 2
var bj_MISSION_INDEX_O03 = bj_CAMPAIGN_OFFSET_O * 1000 + 3
var bj_MISSION_INDEX_O04 = bj_CAMPAIGN_OFFSET_O * 1000 + 4
var bj_MISSION_INDEX_O05 = bj_CAMPAIGN_OFFSET_O * 1000 + 5
var bj_MISSION_INDEX_O06 = bj_CAMPAIGN_OFFSET_O * 1000 + 6
var bj_MISSION_INDEX_O07 = bj_CAMPAIGN_OFFSET_O * 1000 + 7
var bj_MISSION_INDEX_O08 = bj_CAMPAIGN_OFFSET_O * 1000 + 8
var bj_MISSION_INDEX_O09 = bj_CAMPAIGN_OFFSET_O * 1000 + 9
var bj_MISSION_INDEX_O10 = bj_CAMPAIGN_OFFSET_O * 1000 + 10
var bj_MISSION_INDEX_N00 = bj_CAMPAIGN_OFFSET_N * 1000 + 0
var bj_MISSION_INDEX_N01 = bj_CAMPAIGN_OFFSET_N * 1000 + 1
var bj_MISSION_INDEX_N02 = bj_CAMPAIGN_OFFSET_N * 1000 + 2
var bj_MISSION_INDEX_N03 = bj_CAMPAIGN_OFFSET_N * 1000 + 3
var bj_MISSION_INDEX_N04 = bj_CAMPAIGN_OFFSET_N * 1000 + 4
var bj_MISSION_INDEX_N05 = bj_CAMPAIGN_OFFSET_N * 1000 + 5
var bj_MISSION_INDEX_N06 = bj_CAMPAIGN_OFFSET_N * 1000 + 6
var bj_MISSION_INDEX_N07 = bj_CAMPAIGN_OFFSET_N * 1000 + 7
var bj_MISSION_INDEX_N08 = bj_CAMPAIGN_OFFSET_N * 1000 + 8
var bj_MISSION_INDEX_N09 = bj_CAMPAIGN_OFFSET_N * 1000 + 9
var bj_MISSION_INDEX_XN00 = bj_CAMPAIGN_OFFSET_XN * 1000 + 0
var bj_MISSION_INDEX_XN01 = bj_CAMPAIGN_OFFSET_XN * 1000 + 1
var bj_MISSION_INDEX_XN02 = bj_CAMPAIGN_OFFSET_XN * 1000 + 2
var bj_MISSION_INDEX_XN03 = bj_CAMPAIGN_OFFSET_XN * 1000 + 3
var bj_MISSION_INDEX_XN04 = bj_CAMPAIGN_OFFSET_XN * 1000 + 4
var bj_MISSION_INDEX_XN05 = bj_CAMPAIGN_OFFSET_XN * 1000 + 5
var bj_MISSION_INDEX_XN06 = bj_CAMPAIGN_OFFSET_XN * 1000 + 6
var bj_MISSION_INDEX_XN07 = bj_CAMPAIGN_OFFSET_XN * 1000 + 7
var bj_MISSION_INDEX_XN08 = bj_CAMPAIGN_OFFSET_XN * 1000 + 8
var bj_MISSION_INDEX_XN09 = bj_CAMPAIGN_OFFSET_XN * 1000 + 9
var bj_MISSION_INDEX_XN10 = bj_CAMPAIGN_OFFSET_XN * 1000 + 10
var bj_MISSION_INDEX_XH00 = bj_CAMPAIGN_OFFSET_XH * 1000 + 0
var bj_MISSION_INDEX_XH01 = bj_CAMPAIGN_OFFSET_XH * 1000 + 1
var bj_MISSION_INDEX_XH02 = bj_CAMPAIGN_OFFSET_XH * 1000 + 2
var bj_MISSION_INDEX_XH03 = bj_CAMPAIGN_OFFSET_XH * 1000 + 3
var bj_MISSION_INDEX_XH04 = bj_CAMPAIGN_OFFSET_XH * 1000 + 4
var bj_MISSION_INDEX_XH05 = bj_CAMPAIGN_OFFSET_XH * 1000 + 5
var bj_MISSION_INDEX_XH06 = bj_CAMPAIGN_OFFSET_XH * 1000 + 6
var bj_MISSION_INDEX_XH07 = bj_CAMPAIGN_OFFSET_XH * 1000 + 7
var bj_MISSION_INDEX_XH08 = bj_CAMPAIGN_OFFSET_XH * 1000 + 8
var bj_MISSION_INDEX_XH09 = bj_CAMPAIGN_OFFSET_XH * 1000 + 9
var bj_MISSION_INDEX_XU00 = bj_CAMPAIGN_OFFSET_XU * 1000 + 0
var bj_MISSION_INDEX_XU01 = bj_CAMPAIGN_OFFSET_XU * 1000 + 1
var bj_MISSION_INDEX_XU02 = bj_CAMPAIGN_OFFSET_XU * 1000 + 2
var bj_MISSION_INDEX_XU03 = bj_CAMPAIGN_OFFSET_XU * 1000 + 3
var bj_MISSION_INDEX_XU04 = bj_CAMPAIGN_OFFSET_XU * 1000 + 4
var bj_MISSION_INDEX_XU05 = bj_CAMPAIGN_OFFSET_XU * 1000 + 5
var bj_MISSION_INDEX_XU06 = bj_CAMPAIGN_OFFSET_XU * 1000 + 6
var bj_MISSION_INDEX_XU07 = bj_CAMPAIGN_OFFSET_XU * 1000 + 7
var bj_MISSION_INDEX_XU08 = bj_CAMPAIGN_OFFSET_XU * 1000 + 8
var bj_MISSION_INDEX_XU09 = bj_CAMPAIGN_OFFSET_XU * 1000 + 9
var bj_MISSION_INDEX_XU10 = bj_CAMPAIGN_OFFSET_XU * 1000 + 10
var bj_MISSION_INDEX_XU11 = bj_CAMPAIGN_OFFSET_XU * 1000 + 11
var bj_MISSION_INDEX_XU12 = bj_CAMPAIGN_OFFSET_XU * 1000 + 12
var bj_MISSION_INDEX_XU13 = bj_CAMPAIGN_OFFSET_XU * 1000 + 13
var bj_MISSION_INDEX_XO00 = bj_CAMPAIGN_OFFSET_XO * 1000 + 0
var bj_MISSION_INDEX_XO01 = bj_CAMPAIGN_OFFSET_XO * 1000 + 1
var bj_MISSION_INDEX_XO02 = bj_CAMPAIGN_OFFSET_XO * 1000 + 2
var bj_MISSION_INDEX_XO03 = bj_CAMPAIGN_OFFSET_XO * 1000 + 3
var bj_CINEMATICINDEX_TOP = 0
var bj_CINEMATICINDEX_HOP = 1
var bj_CINEMATICINDEX_HED = 2
var bj_CINEMATICINDEX_OOP = 3
var bj_CINEMATICINDEX_OED = 4
var bj_CINEMATICINDEX_UOP = 5
var bj_CINEMATICINDEX_UED = 6
var bj_CINEMATICINDEX_NOP = 7
var bj_CINEMATICINDEX_NED = 8
var bj_CINEMATICINDEX_XOP = 9
var bj_CINEMATICINDEX_XED = 10
var bj_ALLIANCE_UNALLIED = 0
var bj_ALLIANCE_UNALLIED_VISION = 1
var bj_ALLIANCE_ALLIED = 2
var bj_ALLIANCE_ALLIED_VISION = 3
var bj_ALLIANCE_ALLIED_UNITS = 4
var bj_ALLIANCE_ALLIED_ADVUNITS = 5
var bj_ALLIANCE_NEUTRAL = 6
var bj_ALLIANCE_NEUTRAL_VISION = 7
var bj_KEYEVENTTYPE_DEPRESS = 0
var bj_KEYEVENTTYPE_RELEASE = 1
var bj_KEYEVENTKEY_LEFT = 0
var bj_KEYEVENTKEY_RIGHT = 1
var bj_KEYEVENTKEY_DOWN = 2
var bj_KEYEVENTKEY_UP = 3
var bj_MOUSEEVENTTYPE_DOWN = 0
var bj_MOUSEEVENTTYPE_UP = 1
var bj_MOUSEEVENTTYPE_MOVE = 2
var bj_TIMETYPE_ADD = 0
var bj_TIMETYPE_SET = 1
var bj_TIMETYPE_SUB = 2
var bj_CAMERABOUNDS_ADJUST_ADD = 0
var bj_CAMERABOUNDS_ADJUST_SUB = 1
var bj_QUESTTYPE_REQ_DISCOVERED = 0
var bj_QUESTTYPE_REQ_UNDISCOVERED = 1
var bj_QUESTTYPE_OPT_DISCOVERED = 2
var bj_QUESTTYPE_OPT_UNDISCOVERED = 3
var bj_QUESTMESSAGE_DISCOVERED = 0
var bj_QUESTMESSAGE_UPDATED = 1
var bj_QUESTMESSAGE_COMPLETED = 2
var bj_QUESTMESSAGE_FAILED = 3
var bj_QUESTMESSAGE_REQUIREMENT = 4
var bj_QUESTMESSAGE_MISSIONFAILED = 5
var bj_QUESTMESSAGE_ALWAYSHINT = 6
var bj_QUESTMESSAGE_HINT = 7
var bj_QUESTMESSAGE_SECRET = 8
var bj_QUESTMESSAGE_UNITACQUIRED = 9
var bj_QUESTMESSAGE_UNITAVAILABLE = 10
var bj_QUESTMESSAGE_ITEMACQUIRED = 11
var bj_QUESTMESSAGE_WARNING = 12
var bj_SORTTYPE_SORTBYVALUE = 0
var bj_SORTTYPE_SORTBYPLAYER = 1
var bj_SORTTYPE_SORTBYLABEL = 2
var bj_CINEFADETYPE_FADEIN = 0
var bj_CINEFADETYPE_FADEOUT = 1
var bj_CINEFADETYPE_FADEOUTIN = 2
var bj_REMOVEBUFFS_POSITIVE = 0
var bj_REMOVEBUFFS_NEGATIVE = 1
var bj_REMOVEBUFFS_ALL = 2
var bj_REMOVEBUFFS_NONTLIFE = 3
var bj_BUFF_POLARITY_POSITIVE = 0
var bj_BUFF_POLARITY_NEGATIVE = 1
var bj_BUFF_POLARITY_EITHER = 2
var bj_BUFF_RESIST_MAGIC = 0
var bj_BUFF_RESIST_PHYSICAL = 1
var bj_BUFF_RESIST_EITHER = 2
var bj_BUFF_RESIST_BOTH = 3
var bj_HEROSTAT_STR = 0
var bj_HEROSTAT_AGI = 1
var bj_HEROSTAT_INT = 2
var bj_MODIFYMETHOD_ADD = 0
var bj_MODIFYMETHOD_SUB = 1
var bj_MODIFYMETHOD_SET = 2
var bj_UNIT_STATE_METHOD_ABSOLUTE = 0
var bj_UNIT_STATE_METHOD_RELATIVE = 1
var bj_UNIT_STATE_METHOD_DEFAULTS = 2
var bj_UNIT_STATE_METHOD_MAXIMUM = 3
var bj_GATEOPERATION_CLOSE = 0
var bj_GATEOPERATION_OPEN = 1
var bj_GATEOPERATION_DESTROY = 2
var bj_GAMECACHE_BOOLEAN = 0
var bj_GAMECACHE_INTEGER = 1
var bj_GAMECACHE_REAL = 2
var bj_GAMECACHE_UNIT = 3
var bj_GAMECACHE_STRING = 4
var bj_HASHTABLE_BOOLEAN = 0
var bj_HASHTABLE_INTEGER = 1
var bj_HASHTABLE_REAL = 2
var bj_HASHTABLE_STRING = 3
var bj_HASHTABLE_HANDLE = 4
var bj_ITEM_STATUS_HIDDEN = 0
var bj_ITEM_STATUS_OWNED = 1
var bj_ITEM_STATUS_INVULNERABLE = 2
var bj_ITEM_STATUS_POWERUP = 3
var bj_ITEM_STATUS_SELLABLE = 4
var bj_ITEM_STATUS_PAWNABLE = 5
var bj_ITEMCODE_STATUS_POWERUP = 0
var bj_ITEMCODE_STATUS_SELLABLE = 1
var bj_ITEMCODE_STATUS_PAWNABLE = 2
var bj_MINIMAPPINGSTYLE_SIMPLE = 0
var bj_MINIMAPPINGSTYLE_FLASHY = 1
var bj_MINIMAPPINGSTYLE_ATTACK = 2
var bj_CAMPPINGSTYLE_PRIMARY = 0
var bj_CAMPPINGSTYLE_PRIMARY_GREEN = 1
var bj_CAMPPINGSTYLE_PRIMARY_RED = 2
var bj_CAMPPINGSTYLE_BONUS = 3
var bj_CAMPPINGSTYLE_TURNIN = 4
var bj_CAMPPINGSTYLE_BOSS = 5
var bj_CAMPPINGSTYLE_CONTROL_ALLY = 6
var bj_CAMPPINGSTYLE_CONTROL_NEUTRAL = 7
var bj_CAMPPINGSTYLE_CONTROL_ENEMY = 8
var bj_CORPSE_MAX_DEATH_TIME = 8.00
var bj_CORPSETYPE_FLESH = 0
var bj_CORPSETYPE_BONE = 1
var bj_ELEVATOR_BLOCKER_CODE = 'DTep'
var bj_ELEVATOR_CODE01 = 'DTrf'
var bj_ELEVATOR_CODE02 = 'DTrx'
var bj_ELEVATOR_WALL_TYPE_ALL = 0
var bj_ELEVATOR_WALL_TYPE_EAST = 1
var bj_ELEVATOR_WALL_TYPE_NORTH = 2
var bj_ELEVATOR_WALL_TYPE_SOUTH = 3
var bj_ELEVATOR_WALL_TYPE_WEST = 4
var bj_FORCE_ALL_PLAYERS = null;
var bj_FORCE_PLAYER = [];
var bj_MELEE_MAX_TWINKED_HEROES = 0;
var bj_mapInitialPlayableArea = null;
var bj_mapInitialCameraBounds = null;
var bj_forLoopAIndex = 0;
var bj_forLoopBIndex = 0;
var bj_forLoopAIndexEnd = 0;
var bj_forLoopBIndexEnd = 0;
var bj_slotControlReady = false;
var bj_slotControlUsed = [];
var bj_slotControl = [];
var bj_gameStartedTimer = null;
var bj_gameStarted = false;
var bj_volumeGroupsTimer = CreateTimer();
var bj_isSinglePlayer = false;
var bj_dncSoundsDay = null;
var bj_dncSoundsNight = null;
var bj_dayAmbientSound = null;
var bj_nightAmbientSound = null;
var bj_dncSoundsDawn = null;
var bj_dncSoundsDusk = null;
var bj_dawnSound = null;
var bj_duskSound = null;
var bj_useDawnDuskSounds = true;
var bj_dncIsDaytime = false;
var bj_rescueSound = null;
var bj_questDiscoveredSound = null;
var bj_questUpdatedSound = null;
var bj_questCompletedSound = null;
var bj_questFailedSound = null;
var bj_questHintSound = null;
var bj_questSecretSound = null;
var bj_questItemAcquiredSound = null;
var bj_questWarningSound = null;
var bj_victoryDialogSound = null;
var bj_defeatDialogSound = null;
var bj_stockItemPurchased = null;
var bj_stockUpdateTimer = null;
var bj_stockAllowedPermanent = [];
var bj_stockAllowedCharged = [];
var bj_stockAllowedArtifact = [];
var bj_stockPickedItemLevel = 0;
var bj_stockPickedItemType;
var bj_meleeVisibilityTrained = null;
var bj_meleeVisibilityIsDay = true;
var bj_meleeGrantHeroItems = false;
var bj_meleeNearestMineToLoc = null;
var bj_meleeNearestMine = null;
var bj_meleeNearestMineDist = 0.00;
var bj_meleeGameOver = false;
var bj_meleeDefeated = [];
var bj_meleeVictoried = [];
var bj_ghoul = [];
var bj_crippledTimer = [];
var bj_crippledTimerWindows = [];
var bj_playerIsCrippled = [];
var bj_playerIsExposed = [];
var bj_finishSoonAllExposed = false;
var bj_finishSoonTimerDialog = null;
var bj_meleeTwinkedHeroes = [];
var bj_rescueUnitBehavior = null;
var bj_rescueChangeColorUnit = true;
var bj_rescueChangeColorBldg = true;
var bj_cineSceneEndingTimer = null;
var bj_cineSceneLastSound = null;
var bj_cineSceneBeingSkipped = null;
var bj_cineModePriorSpeed = MAP_SPEED_NORMAL;
var bj_cineModePriorFogSetting = false;
var bj_cineModePriorMaskSetting = false;
var bj_cineModeAlreadyIn = false;
var bj_cineModePriorDawnDusk = false;
var bj_cineModeSavedSeed = 0;
var bj_cineFadeFinishTimer = null;
var bj_cineFadeContinueTimer = null;
var bj_cineFadeContinueRed = 0;
var bj_cineFadeContinueGreen = 0;
var bj_cineFadeContinueBlue = 0;
var bj_cineFadeContinueTrans = 0;
var bj_cineFadeContinueDuration = 0;
var bj_cineFadeContinueTex = "";
var bj_queuedExecTotal = 0;
var bj_queuedExecTriggers = [];
var bj_queuedExecUseConds = [];
var bj_queuedExecTimeoutTimer = CreateTimer();
var bj_queuedExecTimeout = null;
var bj_destInRegionDiesCount = 0;
var bj_destInRegionDiesTrig = null;
var bj_groupCountUnits = 0;
var bj_forceCountPlayers = 0;
var bj_groupEnumTypeId = 0;
var bj_groupEnumOwningPlayer = null;
var bj_groupAddGroupDest = null;
var bj_groupRemoveGroupDest = null;
var bj_groupRandomConsidered = 0;
var bj_groupRandomCurrentPick = null;
var bj_groupLastCreatedDest = null;
var bj_randomSubGroupGroup = null;
var bj_randomSubGroupWant = 0;
var bj_randomSubGroupTotal = 0;
var bj_randomSubGroupChance = 0;
var bj_destRandomConsidered = 0;
var bj_destRandomCurrentPick = null;
var bj_elevatorWallBlocker = null;
var bj_elevatorNeighbor = null;
var bj_itemRandomConsidered = 0;
var bj_itemRandomCurrentPick = null;
var bj_forceRandomConsidered = 0;
var bj_forceRandomCurrentPick = null;
var bj_makeUnitRescuableUnit = null;
var bj_makeUnitRescuableFlag = true;
var bj_pauseAllUnitsFlag = true;
var bj_enumDestructableCenter = null;
var bj_enumDestructableRadius = 0;
var bj_setPlayerTargetColor = null;
var bj_isUnitGroupDeadResult = true;
var bj_isUnitGroupEmptyResult = true;
var bj_isUnitGroupInRectResult = true;
var bj_isUnitGroupInRectRect = null;
var bj_changeLevelShowScores = false;
var bj_changeLevelMapName = null;
var bj_suspendDecayFleshGroup = CreateGroup();
var bj_suspendDecayBoneGroup = CreateGroup();
var bj_delayedSuspendDecayTimer = CreateTimer();
var bj_delayedSuspendDecayTrig = null;
var bj_livingPlayerUnitsTypeId = 0;
var bj_lastDyingWidget = null;
var bj_randDistCount = 0;
var bj_randDistID = [];
var bj_randDistChance = [];
var bj_lastCreatedUnit = null;
var bj_lastCreatedItem = null;
var bj_lastRemovedItem = null;
var bj_lastHauntedGoldMine = null;
var bj_lastCreatedDestructable = null;
var bj_lastCreatedGroup = CreateGroup();
var bj_lastCreatedFogModifier = null;
var bj_lastCreatedEffect = null;
var bj_lastCreatedWeatherEffect = null;
var bj_lastCreatedTerrainDeformation = null;
var bj_lastCreatedQuest = null;
var bj_lastCreatedQuestItem = null;
var bj_lastCreatedDefeatCondition = null;
var bj_lastStartedTimer = CreateTimer();
var bj_lastCreatedTimerDialog = null;
var bj_lastCreatedLeaderboard = null;
var bj_lastCreatedMultiboard = null;
var bj_lastPlayedSound = null;
var bj_lastPlayedMusic = "";
var bj_lastTransmissionDuration = 0;
var bj_lastCreatedGameCache = null;
var bj_lastCreatedHashtable = null;
var bj_lastLoadedUnit = null;
var bj_lastCreatedButton = null;
var bj_lastReplacedUnit = null;
var bj_lastCreatedTextTag = null;
var bj_lastCreatedLightning = null;
var bj_lastCreatedImage = null;
var bj_lastCreatedUbersplat = null;
var bj_lastCreatedMinimapIcon = null;
var bj_lastCreatedCommandButtonEffect = null;
var filterIssueHauntOrderAtLocBJ = null;
var filterEnumDestructablesInCircleBJ = null;
var filterGetUnitsInRectOfPlayer = null;
var filterGetUnitsOfTypeIdAll = null;
var filterGetUnitsOfPlayerAndTypeId = null;
var filterMeleeTrainedUnitIsHeroBJ = null;
var filterLivingPlayerUnitsOfTypeId = null;
var bj_wantDestroyGroup = false;
var bj_lastInstObjFuncSuccessful = true;

function BJDebugMsg(msg) {
    var i = 0;
    do {
        DisplayTimedTextToPlayer(Player(i), 0, 0, 60, msg)
        i = i + 1
        if (i == bj_MAX_PLAYERS) { break; }
    } while (true)
}
function RMinBJ(a, b) {
    if ((a < b)) {
        return a
    }
    else {
        return b
    }
}
function RMaxBJ(a, b) {
    if ((a < b)) {
        return b
    }
    else {
        return a
    }
}
function RAbsBJ(a) {
    if ((a >= 0)) {
        return a
    }
    else {
        return -a
    }
}
function RSignBJ(a) {
    if ((a >= 0.0)) {
        return 1.0
    }
    else {
        return -1.0
    }
}
function IMinBJ(a, b) {
    if ((a < b)) {
        return a
    }
    else {
        return b
    }
}
function IMaxBJ(a, b) {
    if ((a < b)) {
        return b
    }
    else {
        return a
    }
}
function IAbsBJ(a) {
    if ((a >= 0)) {
        return a
    }
    else {
        return -a
    }
}
function ISignBJ(a) {
    if ((a >= 0)) {
        return 1
    }
    else {
        return -1
    }
}
function SinBJ(degrees) {
    return Sin(degrees * bj_DEGTORAD)
}
function CosBJ(degrees) {
    return Cos(degrees * bj_DEGTORAD)
}
function TanBJ(degrees) {
    return Tan(degrees * bj_DEGTORAD)
}
function AsinBJ(degrees) {
    return Asin(degrees) * bj_RADTODEG
}
function AcosBJ(degrees) {
    return Acos(degrees) * bj_RADTODEG
}
function AtanBJ(degrees) {
    return Atan(degrees) * bj_RADTODEG
}
function Atan2BJ(y, x) {
    return Atan2(y, x) * bj_RADTODEG
}
function AngleBetweenPoints(locA, locB) {
    return bj_RADTODEG * Atan2(GetLocationY(locB) - GetLocationY(locA), GetLocationX(locB) - GetLocationX(locA))
}
function DistanceBetweenPoints(locA, locB) {
    var dx = GetLocationX(locB) - GetLocationX(locA);
    var dy = GetLocationY(locB) - GetLocationY(locA);
    return SquareRoot(dx * dx + dy * dy)
}
function PolarProjectionBJ(source, dist, angle) {
    var x = GetLocationX(source) + dist * Cos(angle * bj_DEGTORAD);
    var y = GetLocationY(source) + dist * Sin(angle * bj_DEGTORAD);
    return Location(x, y)
}
function GetRandomDirectionDeg() {
    return GetRandomReal(0, 360)
}
function GetRandomPercentageBJ() {
    return GetRandomReal(0, 100)
}
function GetRandomLocInRect(whichRect) {
    return Location(GetRandomReal(GetRectMinX(whichRect), GetRectMaxX(whichRect)), GetRandomReal(GetRectMinY(whichRect), GetRectMaxY(whichRect)))
}
function ModuloInteger(dividend, divisor) {
    var modulus = dividend - (R2I(dividend / divisor)) * divisor;
    if ((modulus < 0)) {
        modulus = modulus + divisor
    }
    return modulus
}
function ModuloReal(dividend, divisor) {
    var modulus = dividend - I2R(R2I(dividend / divisor)) * divisor;
    if ((modulus < 0)) {
        modulus = modulus + divisor
    }
    return modulus
}
function OffsetLocation(loc, dx, dy) {
    return Location(GetLocationX(loc) + dx, GetLocationY(loc) + dy)
}
function OffsetRectBJ(r, dx, dy) {
    return Rect(GetRectMinX(r) + dx, GetRectMinY(r) + dy, GetRectMaxX(r) + dx, GetRectMaxY(r) + dy)
}
function RectFromCenterSizeBJ(center, width, height) {
    var x = GetLocationX(center);
    var y = GetLocationY(center);
    return Rect(x - width * 0.5, y - height * 0.5, x + width * 0.5, y + height * 0.5)
}
function RectContainsCoords(r, x, y) {
    return (GetRectMinX(r) <= x) && (x <= GetRectMaxX(r)) && (GetRectMinY(r) <= y) && (y <= GetRectMaxY(r))
}
function RectContainsLoc(r, loc) {
    return RectContainsCoords(r, GetLocationX(loc), GetLocationY(loc))
}
function RectContainsUnit(r, whichUnit) {
    return RectContainsCoords(r, GetUnitX(whichUnit), GetUnitY(whichUnit))
}
function RectContainsItem(whichItem, r) {
    if ((whichItem == null)) {
        return false
    }
    if ((IsItemOwned(whichItem))) {
        return false
    }
    return RectContainsCoords(r, GetItemX(whichItem), GetItemY(whichItem))
}
function ConditionalTriggerExecute(trig) {
    if (TriggerEvaluate(trig)) {
        TriggerExecute(trig)
    }
}
function TriggerExecuteBJ(trig, checkConditions) {
    if (checkConditions) {
        if (!(TriggerEvaluate(trig))) {
            return false
        }
    }
    TriggerExecute(trig)
    return true
}
function PostTriggerExecuteBJ(trig, checkConditions) {
    if (checkConditions) {
        if (!(TriggerEvaluate(trig))) {
            return false
        }
    }
    TriggerRegisterTimerEvent(trig, 0, false)
    return true
}
function QueuedTriggerCheck() {
    var s = "TrigQueue Check ";
    var i;
    i = 0
    do {
        if (i >= bj_queuedExecTotal) { break; }
        s = S2S(s + "q[") + I2S(i) + "]="
        if ((bj_queuedExecTriggers[i] == null)) {
            s = S2S(s + "null ")
        }
        else {
            s = S2S(s + "x ")
        }
        i = i + 1
    } while (true)
    s = S2S(s + "(") + I2S(bj_queuedExecTotal) + " total)"
    DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, 600, s)
}
function QueuedTriggerGetIndex(trig) {
    var index = 0;
    do {
        if (index >= bj_queuedExecTotal) { break; }
        if ((bj_queuedExecTriggers[index] == trig)) {
            return index
        }
        index = index + 1
    } while (true)
    return -1
}
function QueuedTriggerRemoveByIndex(trigIndex) {
    var index;
    if ((trigIndex >= bj_queuedExecTotal)) {
        return false
    }
    bj_queuedExecTotal = bj_queuedExecTotal - 1
    index = trigIndex
    do {
        if (index >= bj_queuedExecTotal) { break; }
        bj_queuedExecTriggers[index] = bj_queuedExecTriggers[index + 1]
        bj_queuedExecUseConds[index] = bj_queuedExecUseConds[index + 1]
        index = index + 1
    } while (true)
    return true
}
function QueuedTriggerAttemptExec() {
    do {
        if (bj_queuedExecTotal == 0) { break; }
        if (TriggerExecuteBJ(bj_queuedExecTriggers[0], bj_queuedExecUseConds[0])) {
            TimerStart(bj_queuedExecTimeoutTimer, bj_QUEUED_TRIGGER_TIMEOUT, false, null)
            return true
        }
        QueuedTriggerRemoveByIndex(0)
    } while (true)
    return false
}
function QueuedTriggerAddBJ(trig, checkConditions) {
    if ((bj_queuedExecTotal >= bj_MAX_QUEUED_TRIGGERS)) {
        return false
    }
    bj_queuedExecTriggers[bj_queuedExecTotal] = trig
    bj_queuedExecUseConds[bj_queuedExecTotal] = checkConditions
    bj_queuedExecTotal = bj_queuedExecTotal + 1
    if ((bj_queuedExecTotal == 1)) {
        QueuedTriggerAttemptExec()
    }
    return true
}
function QueuedTriggerRemoveBJ(trig) {
    var index;
    var trigIndex;
    var trigExecuted;
    trigIndex = QueuedTriggerGetIndex(trig)
    if ((trigIndex == -1)) {
        return
    }
    QueuedTriggerRemoveByIndex(trigIndex)
    if ((trigIndex == 0)) {
        PauseTimer(bj_queuedExecTimeoutTimer)
        QueuedTriggerAttemptExec()
    }
}
function QueuedTriggerDoneBJ() {
    var index;
    if ((bj_queuedExecTotal <= 0)) {
        return
    }
    QueuedTriggerRemoveByIndex(0)
    PauseTimer(bj_queuedExecTimeoutTimer)
    QueuedTriggerAttemptExec()
}
function QueuedTriggerClearBJ() {
    PauseTimer(bj_queuedExecTimeoutTimer)
    bj_queuedExecTotal = 0
}
function QueuedTriggerClearInactiveBJ() {
    bj_queuedExecTotal = IMinBJ(bj_queuedExecTotal, 1)
}
function QueuedTriggerCountBJ() {
    return bj_queuedExecTotal
}
function IsTriggerQueueEmptyBJ() {
    return bj_queuedExecTotal <= 0
}
function IsTriggerQueuedBJ(trig) {
    return QueuedTriggerGetIndex(trig) != -1
}
function GetForLoopIndexA() {
    return bj_forLoopAIndex
}
function SetForLoopIndexA(newIndex) {
    bj_forLoopAIndex = newIndex
}
function GetForLoopIndexB() {
    return bj_forLoopBIndex
}
function SetForLoopIndexB(newIndex) {
    bj_forLoopBIndex = newIndex
}
function PolledWait(duration) {
    var t;
    var timeRemaining;
    if ((duration > 0)) {
        t = CreateTimer()
        TimerStart(t, duration, false, null)
        do {
            timeRemaining = TimerGetRemaining(t)
            if (timeRemaining <= 0) { break; }
            if ((timeRemaining > bj_POLLED_WAIT_SKIP_THRESHOLD)) {
                TriggerSleepAction(0.1 * timeRemaining)
            }
            else {
                TriggerSleepAction(bj_POLLED_WAIT_INTERVAL)
            }
        } while (true)
        DestroyTimer(t)
    }
}
function IntegerTertiaryOp(flag, valueA, valueB) {
    if (flag) {
        return valueA
    }
    else {
        return valueB
    }
}
function DoNothing() {
}
function CommentString(commentString) {
}
function StringIdentity(theString) {
    return GetLocalizedString(theString)
}
function GetBooleanAnd(valueA, valueB) {
    return valueA && valueB
}
function GetBooleanOr(valueA, valueB) {
    return valueA || valueB
}
function PercentToInt(percentage, max) {
    var realpercent = percentage * I2R(max) * 0.01;
    var result = MathRound(realpercent);
    if ((result < 0)) {
        result = 0
    }
    else {
        if ((result > max)) {
            result = max
        }
    }
    return result
}
function PercentTo255(percentage) {
    return PercentToInt(percentage, 255)
}
function GetTimeOfDay() {
    return GetFloatGameState(GAME_STATE_TIME_OF_DAY)
}
function SetTimeOfDay(whatTime) {
    SetFloatGameState(GAME_STATE_TIME_OF_DAY, whatTime)
}
function SetTimeOfDayScalePercentBJ(scalePercent) {
    SetTimeOfDayScale(scalePercent * 0.01)
}
function GetTimeOfDayScalePercentBJ() {
    return GetTimeOfDayScale() * 100
}
function PlaySound(soundName) {
    var soundHandle = CreateSound(soundName, false, false, true, 12700, 12700, "");
    StartSound(soundHandle)
    KillSoundWhenDone(soundHandle)
}
function CompareLocationsBJ(A, B) {
    return GetLocationX(A) == GetLocationX(B) && GetLocationY(A) == GetLocationY(B)
}
function CompareRectsBJ(A, B) {
    return GetRectMinX(A) == GetRectMinX(B) && GetRectMinY(A) == GetRectMinY(B) && GetRectMaxX(A) == GetRectMaxX(B) && GetRectMaxY(A) == GetRectMaxY(B)
}
function GetRectFromCircleBJ(center, radius) {
    var centerX = GetLocationX(center);
    var centerY = GetLocationY(center);
    return Rect(centerX - radius, centerY - radius, centerX + radius, centerY + radius)
}
function GetCurrentCameraSetup() {
    var theCam = CreateCameraSetup();
    var duration = 0;
    CameraSetupSetField(theCam, CAMERA_FIELD_TARGET_DISTANCE, GetCameraField(CAMERA_FIELD_TARGET_DISTANCE), duration)
    CameraSetupSetField(theCam, CAMERA_FIELD_FARZ, GetCameraField(CAMERA_FIELD_FARZ), duration)
    CameraSetupSetField(theCam, CAMERA_FIELD_ZOFFSET, GetCameraField(CAMERA_FIELD_ZOFFSET), duration)
    CameraSetupSetField(theCam, CAMERA_FIELD_ANGLE_OF_ATTACK, bj_RADTODEG * GetCameraField(CAMERA_FIELD_ANGLE_OF_ATTACK), duration)
    CameraSetupSetField(theCam, CAMERA_FIELD_FIELD_OF_VIEW, bj_RADTODEG * GetCameraField(CAMERA_FIELD_FIELD_OF_VIEW), duration)
    CameraSetupSetField(theCam, CAMERA_FIELD_ROLL, bj_RADTODEG * GetCameraField(CAMERA_FIELD_ROLL), duration)
    CameraSetupSetField(theCam, CAMERA_FIELD_ROTATION, bj_RADTODEG * GetCameraField(CAMERA_FIELD_ROTATION), duration)
    CameraSetupSetField(theCam, CAMERA_FIELD_LOCAL_PITCH, bj_RADTODEG * GetCameraField(CAMERA_FIELD_LOCAL_PITCH), duration)
    CameraSetupSetField(theCam, CAMERA_FIELD_LOCAL_YAW, bj_RADTODEG * GetCameraField(CAMERA_FIELD_LOCAL_YAW), duration)
    CameraSetupSetField(theCam, CAMERA_FIELD_LOCAL_ROLL, bj_RADTODEG * GetCameraField(CAMERA_FIELD_LOCAL_ROLL), duration)
    CameraSetupSetDestPosition(theCam, GetCameraTargetPositionX(), GetCameraTargetPositionY(), duration)
    return theCam
}
function CameraSetupApplyForPlayer(doPan, whichSetup, whichPlayer, duration) {
    if ((GetLocalPlayer() == whichPlayer)) {
        CameraSetupApplyForceDuration(whichSetup, doPan, duration)
    }
}
function CameraSetupApplyForPlayerSmooth(doPan, whichSetup, whichPlayer, forcedDuration, easeInDuration, easeOutDuration, smoothFactor) {
    if ((GetLocalPlayer() == whichPlayer)) {
        BlzCameraSetupApplyForceDurationSmooth(whichSetup, doPan, forcedDuration, easeInDuration, easeOutDuration, smoothFactor)
    }
}
function CameraSetupGetFieldSwap(whichField, whichSetup) {
    return CameraSetupGetField(whichSetup, whichField)
}
function SetCameraFieldForPlayer(whichPlayer, whichField, value, duration) {
    if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraField(whichField, value, duration)
    }
}
function SetCameraTargetControllerNoZForPlayer(whichPlayer, whichUnit, xoffset, yoffset, inheritOrientation) {
    if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraTargetController(whichUnit, xoffset, yoffset, inheritOrientation)
    }
}
function SetCameraPositionForPlayer(whichPlayer, x, y) {
    if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraPosition(x, y)
    }
}
function SetCameraPositionLocForPlayer(whichPlayer, loc) {
    if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraPosition(GetLocationX(loc), GetLocationY(loc))
    }
}
function RotateCameraAroundLocBJ(degrees, loc, whichPlayer, duration) {
    if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraRotateMode(GetLocationX(loc), GetLocationY(loc), bj_DEGTORAD * degrees, duration)
    }
}
function PanCameraToForPlayer(whichPlayer, x, y) {
    if ((GetLocalPlayer() == whichPlayer)) {
        PanCameraTo(x, y)
    }
}
function PanCameraToLocForPlayer(whichPlayer, loc) {
    if ((GetLocalPlayer() == whichPlayer)) {
        PanCameraTo(GetLocationX(loc), GetLocationY(loc))
    }
}
function PanCameraToTimedForPlayer(whichPlayer, x, y, duration) {
    if ((GetLocalPlayer() == whichPlayer)) {
        PanCameraToTimed(x, y, duration)
    }
}
function PanCameraToTimedLocForPlayer(whichPlayer, loc, duration) {
    if ((GetLocalPlayer() == whichPlayer)) {
        PanCameraToTimed(GetLocationX(loc), GetLocationY(loc), duration)
    }
}
function PanCameraToTimedLocWithZForPlayer(whichPlayer, loc, zOffset, duration) {
    if ((GetLocalPlayer() == whichPlayer)) {
        PanCameraToTimedWithZ(GetLocationX(loc), GetLocationY(loc), zOffset, duration)
    }
}
function SmartCameraPanBJ(whichPlayer, loc, duration) {
    var dist;
    var cameraLoc = GetCameraTargetPositionLoc();
    if ((GetLocalPlayer() == whichPlayer)) {
        dist = DistanceBetweenPoints(loc, cameraLoc)
        if ((dist >= bj_SMARTPAN_TRESHOLD_SNAP)) {
            PanCameraToTimed(GetLocationX(loc), GetLocationY(loc), 0)
        }
        else {
            if ((dist >= bj_SMARTPAN_TRESHOLD_PAN)) {
                PanCameraToTimed(GetLocationX(loc), GetLocationY(loc), duration)
            }
            else {
            }
        }
    }
    RemoveLocation(cameraLoc)
}
function SetCinematicCameraForPlayer(whichPlayer, cameraModelFile) {
    if ((GetLocalPlayer() == whichPlayer)) {
        SetCinematicCamera(cameraModelFile)
    }
}
function ResetToGameCameraForPlayer(whichPlayer, duration) {
    if ((GetLocalPlayer() == whichPlayer)) {
        ResetToGameCamera(duration)
    }
}
function CameraSetSourceNoiseForPlayer(whichPlayer, magnitude, velocity) {
    if ((GetLocalPlayer() == whichPlayer)) {
        CameraSetSourceNoise(magnitude, velocity)
    }
}
function CameraSetTargetNoiseForPlayer(whichPlayer, magnitude, velocity) {
    if ((GetLocalPlayer() == whichPlayer)) {
        CameraSetTargetNoise(magnitude, velocity)
    }
}
function CameraSetEQNoiseForPlayer(whichPlayer, magnitude) {
    var richter = magnitude;
    if ((richter > 5.0)) {
        richter = 5.0
    }
    if ((richter < 2.0)) {
        richter = 2.0
    }
    if ((GetLocalPlayer() == whichPlayer)) {
        CameraSetTargetNoiseEx(magnitude * 2.0, magnitude * Pow(10, richter), true)
        CameraSetSourceNoiseEx(magnitude * 2.0, magnitude * Pow(10, richter), true)
    }
}
function CameraClearNoiseForPlayer(whichPlayer) {
    if ((GetLocalPlayer() == whichPlayer)) {
        CameraSetSourceNoise(0, 0)
        CameraSetTargetNoise(0, 0)
    }
}
function GetCurrentCameraBoundsMapRectBJ() {
    return Rect(GetCameraBoundMinX(), GetCameraBoundMinY(), GetCameraBoundMaxX(), GetCameraBoundMaxY())
}
function GetCameraBoundsMapRect() {
    return bj_mapInitialCameraBounds
}
function GetPlayableMapRect() {
    return bj_mapInitialPlayableArea
}
function GetEntireMapRect() {
    return GetWorldBounds()
}
function SetCameraBoundsToRect(r) {
    var minX = GetRectMinX(r);
    var minY = GetRectMinY(r);
    var maxX = GetRectMaxX(r);
    var maxY = GetRectMaxY(r);
    SetCameraBounds(minX, minY, minX, maxY, maxX, maxY, maxX, minY)
}
function SetCameraBoundsToRectForPlayerBJ(whichPlayer, r) {
    if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraBoundsToRect(r)
    }
}
function AdjustCameraBoundsBJ(adjustMethod, dxWest, dxEast, dyNorth, dySouth) {
    var minX = 0;
    var minY = 0;
    var maxX = 0;
    var maxY = 0;
    var scale = 0;
    if ((adjustMethod == bj_CAMERABOUNDS_ADJUST_ADD)) {
        scale = 1
    }
    else {
        if ((adjustMethod == bj_CAMERABOUNDS_ADJUST_SUB)) {
            scale = -1
        }
        else {
            return
        }
    }
    minX = GetCameraBoundMinX() - scale * dxWest
    maxX = GetCameraBoundMaxX() + scale * dxEast
    minY = GetCameraBoundMinY() - scale * dySouth
    maxY = GetCameraBoundMaxY() + scale * dyNorth
    if ((maxX < minX)) {
        minX = (minX + maxX) * 0.5
        maxX = minX
    }
    if ((maxY < minY)) {
        minY = (minY + maxY) * 0.5
        maxY = minY
    }
    SetCameraBounds(minX, minY, minX, maxY, maxX, maxY, maxX, minY)
}
function AdjustCameraBoundsForPlayerBJ(adjustMethod, whichPlayer, dxWest, dxEast, dyNorth, dySouth) {
    if ((GetLocalPlayer() == whichPlayer)) {
        AdjustCameraBoundsBJ(adjustMethod, dxWest, dxEast, dyNorth, dySouth)
    }
}
function SetCameraQuickPositionForPlayer(whichPlayer, x, y) {
    if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraQuickPosition(x, y)
    }
}
function SetCameraQuickPositionLocForPlayer(whichPlayer, loc) {
    if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraQuickPosition(GetLocationX(loc), GetLocationY(loc))
    }
}
function SetCameraQuickPositionLoc(loc) {
    SetCameraQuickPosition(GetLocationX(loc), GetLocationY(loc))
}
function StopCameraForPlayerBJ(whichPlayer) {
    if ((GetLocalPlayer() == whichPlayer)) {
        StopCamera()
    }
}
function SetCameraOrientControllerForPlayerBJ(whichPlayer, whichUnit, xoffset, yoffset) {
    if ((GetLocalPlayer() == whichPlayer)) {
        SetCameraOrientController(whichUnit, xoffset, yoffset)
    }
}
function CameraSetSmoothingFactorBJ(factor) {
    CameraSetSmoothingFactor(factor)
}
function CameraResetSmoothingFactorBJ() {
    CameraSetSmoothingFactor(0)
}
function DisplayTextToForce(toForce, message) {
    if ((IsPlayerInForce(GetLocalPlayer(), toForce))) {
        DisplayTextToPlayer(GetLocalPlayer(), 0, 0, message)
    }
}
function DisplayTimedTextToForce(toForce, duration, message) {
    if ((IsPlayerInForce(GetLocalPlayer(), toForce))) {
        DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, duration, message)
    }
}
function ClearTextMessagesBJ(toForce) {
    if ((IsPlayerInForce(GetLocalPlayer(), toForce))) {
        ClearTextMessages()
    }
}
function SubStringBJ(source, start, end) {
    return SubString(source, start - 1, end)
}
function GetHandleIdBJ(h) {
    return GetHandleId(h)
}
function StringHashBJ(s) {
    return StringHash(s)
}
function TriggerRegisterTimerEventPeriodic(trig, timeout) {
    return TriggerRegisterTimerEvent(trig, timeout, true)
}
function TriggerRegisterTimerEventSingle(trig, timeout) {
    return TriggerRegisterTimerEvent(trig, timeout, false)
}
function TriggerRegisterTimerExpireEventBJ(trig, t) {
    return TriggerRegisterTimerExpireEvent(trig, t)
}
function TriggerRegisterPlayerUnitEventSimple(trig, whichPlayer, whichEvent) {
    return TriggerRegisterPlayerUnitEvent(trig, whichPlayer, whichEvent, null)
}
function TriggerRegisterAnyUnitEventBJ(trig, whichEvent) {
    var index;
    index = 0
    do {
        TriggerRegisterPlayerUnitEvent(trig, Player(index), whichEvent, null)
        index = index + 1
        if (index == bj_MAX_PLAYER_SLOTS) { break; }
    } while (true)
}
function TriggerRegisterPlayerSelectionEventBJ(trig, whichPlayer, selected) {
    if (selected) {
        return TriggerRegisterPlayerUnitEvent(trig, whichPlayer, EVENT_PLAYER_UNIT_SELECTED, null)
    }
    else {
        return TriggerRegisterPlayerUnitEvent(trig, whichPlayer, EVENT_PLAYER_UNIT_DESELECTED, null)
    }
}
function TriggerRegisterPlayerKeyEventBJ(trig, whichPlayer, keType, keKey) {
    if ((keType == bj_KEYEVENTTYPE_DEPRESS)) {
        if ((keKey == bj_KEYEVENTKEY_LEFT)) {
            return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ARROW_LEFT_DOWN)
        }
        else {
            if ((keKey == bj_KEYEVENTKEY_RIGHT)) {
                return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ARROW_RIGHT_DOWN)
            }
            else {
                if ((keKey == bj_KEYEVENTKEY_DOWN)) {
                    return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ARROW_DOWN_DOWN)
                }
                else {
                    if ((keKey == bj_KEYEVENTKEY_UP)) {
                        return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ARROW_UP_DOWN)
                    }
                    else {
                        return null
                    }
                }
            }
        }
    }
    else {
        if ((keType == bj_KEYEVENTTYPE_RELEASE)) {
            if ((keKey == bj_KEYEVENTKEY_LEFT)) {
                return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ARROW_LEFT_UP)
            }
            else {
                if ((keKey == bj_KEYEVENTKEY_RIGHT)) {
                    return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ARROW_RIGHT_UP)
                }
                else {
                    if ((keKey == bj_KEYEVENTKEY_DOWN)) {
                        return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ARROW_DOWN_UP)
                    }
                    else {
                        if ((keKey == bj_KEYEVENTKEY_UP)) {
                            return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ARROW_UP_UP)
                        }
                        else {
                            return null
                        }
                    }
                }
            }
        }
        else {
            return null
        }
    }
}
function TriggerRegisterPlayerMouseEventBJ(trig, whichPlayer, meType) {
    if ((meType == bj_MOUSEEVENTTYPE_DOWN)) {
        return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_MOUSE_DOWN)
    }
    else {
        if ((meType == bj_MOUSEEVENTTYPE_UP)) {
            return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_MOUSE_UP)
        }
        else {
            if ((meType == bj_MOUSEEVENTTYPE_MOVE)) {
                return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_MOUSE_MOVE)
            }
            else {
                return null
            }
        }
    }
}
function TriggerRegisterPlayerEventVictory(trig, whichPlayer) {
    return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_VICTORY)
}
function TriggerRegisterPlayerEventDefeat(trig, whichPlayer) {
    return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_DEFEAT)
}
function TriggerRegisterPlayerEventLeave(trig, whichPlayer) {
    return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_LEAVE)
}
function TriggerRegisterPlayerEventAllianceChanged(trig, whichPlayer) {
    return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_ALLIANCE_CHANGED)
}
function TriggerRegisterPlayerEventEndCinematic(trig, whichPlayer) {
    return TriggerRegisterPlayerEvent(trig, whichPlayer, EVENT_PLAYER_END_CINEMATIC)
}
function TriggerRegisterGameStateEventTimeOfDay(trig, opcode, limitval) {
    return TriggerRegisterGameStateEvent(trig, GAME_STATE_TIME_OF_DAY, opcode, limitval)
}
function TriggerRegisterEnterRegionSimple(trig, whichRegion) {
    return TriggerRegisterEnterRegion(trig, whichRegion, null)
}
function TriggerRegisterLeaveRegionSimple(trig, whichRegion) {
    return TriggerRegisterLeaveRegion(trig, whichRegion, null)
}
function TriggerRegisterEnterRectSimple(trig, r) {
    var rectRegion = CreateRegion();
    RegionAddRect(rectRegion, r)
    return TriggerRegisterEnterRegion(trig, rectRegion, null)
}
function TriggerRegisterLeaveRectSimple(trig, r) {
    var rectRegion = CreateRegion();
    RegionAddRect(rectRegion, r)
    return TriggerRegisterLeaveRegion(trig, rectRegion, null)
}
function TriggerRegisterDistanceBetweenUnits(trig, whichUnit, condition, range) {
    return TriggerRegisterUnitInRange(trig, whichUnit, range, condition)
}
function TriggerRegisterUnitInRangeSimple(trig, range, whichUnit) {
    return TriggerRegisterUnitInRange(trig, whichUnit, range, null)
}
function TriggerRegisterUnitLifeEvent(trig, whichUnit, opcode, limitval) {
    return TriggerRegisterUnitStateEvent(trig, whichUnit, UNIT_STATE_LIFE, opcode, limitval)
}
function TriggerRegisterUnitManaEvent(trig, whichUnit, opcode, limitval) {
    return TriggerRegisterUnitStateEvent(trig, whichUnit, UNIT_STATE_MANA, opcode, limitval)
}
function TriggerRegisterDialogEventBJ(trig, whichDialog) {
    return TriggerRegisterDialogEvent(trig, whichDialog)
}
function TriggerRegisterShowSkillEventBJ(trig) {
    return TriggerRegisterGameEvent(trig, EVENT_GAME_SHOW_SKILL)
}
function TriggerRegisterBuildSubmenuEventBJ(trig) {
    return TriggerRegisterGameEvent(trig, EVENT_GAME_BUILD_SUBMENU)
}
function TriggerRegisterBuildCommandEventBJ(trig, unitId) {
    TriggerRegisterCommandEvent(trig, 'ANbu', UnitId2String(unitId))
    TriggerRegisterCommandEvent(trig, 'AHbu', UnitId2String(unitId))
    TriggerRegisterCommandEvent(trig, 'AEbu', UnitId2String(unitId))
    TriggerRegisterCommandEvent(trig, 'AObu', UnitId2String(unitId))
    TriggerRegisterCommandEvent(trig, 'AUbu', UnitId2String(unitId))
    return TriggerRegisterCommandEvent(trig, 'AGbu', UnitId2String(unitId))
}
function TriggerRegisterTrainCommandEventBJ(trig, unitId) {
    return TriggerRegisterCommandEvent(trig, 'Aque', UnitId2String(unitId))
}
function TriggerRegisterUpgradeCommandEventBJ(trig, techId) {
    return TriggerRegisterUpgradeCommandEvent(trig, techId)
}
function TriggerRegisterCommonCommandEventBJ(trig, order) {
    return TriggerRegisterCommandEvent(trig, 0, order)
}
function TriggerRegisterGameLoadedEventBJ(trig) {
    return TriggerRegisterGameEvent(trig, EVENT_GAME_LOADED)
}
function TriggerRegisterGameSavedEventBJ(trig) {
    return TriggerRegisterGameEvent(trig, EVENT_GAME_SAVE)
}
function RegisterDestDeathInRegionEnum() {
    bj_destInRegionDiesCount = bj_destInRegionDiesCount + 1
    if ((bj_destInRegionDiesCount <= bj_MAX_DEST_IN_REGION_EVENTS)) {
        TriggerRegisterDeathEvent(bj_destInRegionDiesTrig, GetEnumDestructable())
    }
}
function TriggerRegisterDestDeathInRegionEvent(trig, r) {
    bj_destInRegionDiesTrig = trig
    bj_destInRegionDiesCount = 0
    EnumDestructablesInRect(r, null, RegisterDestDeathInRegionEnum)
}
function AddWeatherEffectSaveLast(where, effectID) {
    bj_lastCreatedWeatherEffect = AddWeatherEffect(where, effectID)
    return bj_lastCreatedWeatherEffect
}
function GetLastCreatedWeatherEffect() {
    return bj_lastCreatedWeatherEffect
}
function RemoveWeatherEffectBJ(whichWeatherEffect) {
    RemoveWeatherEffect(whichWeatherEffect)
}
function TerrainDeformationCraterBJ(duration, permanent, where, radius, depth) {
    bj_lastCreatedTerrainDeformation = TerrainDeformCrater(GetLocationX(where), GetLocationY(where), radius, depth, R2I(duration * 1000), permanent)
    return bj_lastCreatedTerrainDeformation
}
function TerrainDeformationRippleBJ(duration, limitNeg, where, startRadius, endRadius, depth, wavePeriod, waveWidth) {
    var spaceWave;
    var timeWave;
    var radiusRatio;
    if ((endRadius <= 0 || waveWidth <= 0 || wavePeriod <= 0)) {
        return null
    }
    timeWave = 2.0 * duration / wavePeriod
    spaceWave = 2.0 * endRadius / waveWidth
    radiusRatio = startRadius / endRadius
    bj_lastCreatedTerrainDeformation = TerrainDeformRipple(GetLocationX(where), GetLocationY(where), endRadius, depth, R2I(duration * 1000), 1, spaceWave, timeWave, radiusRatio, limitNeg)
    return bj_lastCreatedTerrainDeformation
}
function TerrainDeformationWaveBJ(duration, source, target, radius, depth, trailDelay) {
    var distance;
    var dirX;
    var dirY;
    var speed;
    distance = DistanceBetweenPoints(source, target)
    if ((distance == 0 || duration <= 0)) {
        return null
    }
    dirX = (GetLocationX(target) - GetLocationX(source)) / distance
    dirY = (GetLocationY(target) - GetLocationY(source)) / distance
    speed = distance / duration
    bj_lastCreatedTerrainDeformation = TerrainDeformWave(GetLocationX(source), GetLocationY(source), dirX, dirY, distance, speed, radius, depth, R2I(trailDelay * 1000), 1)
    return bj_lastCreatedTerrainDeformation
}
function TerrainDeformationRandomBJ(duration, where, radius, minDelta, maxDelta, updateInterval) {
    bj_lastCreatedTerrainDeformation = TerrainDeformRandom(GetLocationX(where), GetLocationY(where), radius, minDelta, maxDelta, R2I(duration * 1000), R2I(updateInterval * 1000))
    return bj_lastCreatedTerrainDeformation
}
function TerrainDeformationStopBJ(deformation, duration) {
    TerrainDeformStop(deformation, R2I(duration * 1000))
}
function GetLastCreatedTerrainDeformation() {
    return bj_lastCreatedTerrainDeformation
}
function AddLightningLoc(codeName, where1, where2) {
    bj_lastCreatedLightning = AddLightningEx(codeName, true, GetLocationX(where1), GetLocationY(where1), GetLocationZ(where1), GetLocationX(where2), GetLocationY(where2), GetLocationZ(where2))
    return bj_lastCreatedLightning
}
function DestroyLightningBJ(whichBolt) {
    return DestroyLightning(whichBolt)
}
function MoveLightningLoc(whichBolt, where1, where2) {
    return MoveLightningEx(whichBolt, true, GetLocationX(where1), GetLocationY(where1), GetLocationZ(where1), GetLocationX(where2), GetLocationY(where2), GetLocationZ(where2))
}
function GetLightningColorABJ(whichBolt) {
    return GetLightningColorA(whichBolt)
}
function GetLightningColorRBJ(whichBolt) {
    return GetLightningColorR(whichBolt)
}
function GetLightningColorGBJ(whichBolt) {
    return GetLightningColorG(whichBolt)
}
function GetLightningColorBBJ(whichBolt) {
    return GetLightningColorB(whichBolt)
}
function SetLightningColorBJ(whichBolt, r, g, b, a) {
    return SetLightningColor(whichBolt, r, g, b, a)
}
function GetLastCreatedLightningBJ() {
    return bj_lastCreatedLightning
}
function GetAbilityEffectBJ(abilcode, t, index) {
    return GetAbilityEffectById(abilcode, t, index)
}
function GetAbilitySoundBJ(abilcode, t) {
    return GetAbilitySoundById(abilcode, t)
}
function GetTerrainCliffLevelBJ(where) {
    return GetTerrainCliffLevel(GetLocationX(where), GetLocationY(where))
}
function GetTerrainTypeBJ(where) {
    return GetTerrainType(GetLocationX(where), GetLocationY(where))
}
function GetTerrainVarianceBJ(where) {
    return GetTerrainVariance(GetLocationX(where), GetLocationY(where))
}
function SetTerrainTypeBJ(where, terrainType, variation, area, shape) {
    SetTerrainType(GetLocationX(where), GetLocationY(where), terrainType, variation, area, shape)
}
function IsTerrainPathableBJ(where, t) {
    return IsTerrainPathable(GetLocationX(where), GetLocationY(where), t)
}
function SetTerrainPathableBJ(where, t, flag) {
    SetTerrainPathable(GetLocationX(where), GetLocationY(where), t, flag)
}
function SetWaterBaseColorBJ(red, green, blue, transparency) {
    SetWaterBaseColor(PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - transparency))
}
function CreateFogModifierRectSimple(whichPlayer, whichFogState, r, afterUnits) {
    bj_lastCreatedFogModifier = CreateFogModifierRect(whichPlayer, whichFogState, r, true, afterUnits)
    return bj_lastCreatedFogModifier
}
function CreateFogModifierRadiusLocSimple(whichPlayer, whichFogState, center, radius, afterUnits) {
    bj_lastCreatedFogModifier = CreateFogModifierRadiusLoc(whichPlayer, whichFogState, center, radius, true, afterUnits)
    return bj_lastCreatedFogModifier
}
function CreateFogModifierRectBJ(enabled, whichPlayer, whichFogState, r) {
    bj_lastCreatedFogModifier = CreateFogModifierRect(whichPlayer, whichFogState, r, true, false)
    if (enabled) {
        FogModifierStart(bj_lastCreatedFogModifier)
    }
    return bj_lastCreatedFogModifier
}
function CreateFogModifierRadiusLocBJ(enabled, whichPlayer, whichFogState, center, radius) {
    bj_lastCreatedFogModifier = CreateFogModifierRadiusLoc(whichPlayer, whichFogState, center, radius, true, false)
    if (enabled) {
        FogModifierStart(bj_lastCreatedFogModifier)
    }
    return bj_lastCreatedFogModifier
}
function GetLastCreatedFogModifier() {
    return bj_lastCreatedFogModifier
}
function FogEnableOn() {
    FogEnable(true)
}
function FogEnableOff() {
    FogEnable(false)
}
function FogMaskEnableOn() {
    FogMaskEnable(true)
}
function FogMaskEnableOff() {
    FogMaskEnable(false)
}
function UseTimeOfDayBJ(flag) {
    SuspendTimeOfDay(!flag)
}
function SetTerrainFogExBJ(style, zstart, zend, density, red, green, blue) {
    SetTerrainFogEx(style, zstart, zend, density, red * 0.01, green * 0.01, blue * 0.01)
}
function ResetTerrainFogBJ() {
    ResetTerrainFog()
}
function SetDoodadAnimationBJ(animName, doodadID, radius, center) {
    SetDoodadAnimation(GetLocationX(center), GetLocationY(center), radius, doodadID, false, animName, false)
}
function SetDoodadAnimationRectBJ(animName, doodadID, r) {
    SetDoodadAnimationRect(r, doodadID, animName, false)
}
function AddUnitAnimationPropertiesBJ(add, animProperties, whichUnit) {
    AddUnitAnimationProperties(whichUnit, animProperties, add)
}
function CreateImageBJ(file, size, where, zOffset, imageType) {
    bj_lastCreatedImage = CreateImage(file, size, size, size, GetLocationX(where), GetLocationY(where), zOffset, 0, 0, 0, imageType)
    return bj_lastCreatedImage
}
function ShowImageBJ(flag, whichImage) {
    ShowImage(whichImage, flag)
}
function SetImagePositionBJ(whichImage, where, zOffset) {
    SetImagePosition(whichImage, GetLocationX(where), GetLocationY(where), zOffset)
}
function SetImageColorBJ(whichImage, red, green, blue, alpha) {
    SetImageColor(whichImage, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - alpha))
}
function GetLastCreatedImage() {
    return bj_lastCreatedImage
}
function CreateUbersplatBJ(where, name, red, green, blue, alpha, forcePaused, noBirthTime) {
    bj_lastCreatedUbersplat = CreateUbersplat(GetLocationX(where), GetLocationY(where), name, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - alpha), forcePaused, noBirthTime)
    return bj_lastCreatedUbersplat
}
function ShowUbersplatBJ(flag, whichSplat) {
    ShowUbersplat(whichSplat, flag)
}
function GetLastCreatedUbersplat() {
    return bj_lastCreatedUbersplat
}
function GetLastCreatedMinimapIcon() {
    return bj_lastCreatedMinimapIcon
}
function CreateMinimapIconOnUnitBJ(whichUnit, red, green, blue, pingPath, fogVisibility) {
    bj_lastCreatedMinimapIcon = CreateMinimapIconOnUnit(whichUnit, red, green, blue, pingPath, fogVisibility)
    return bj_lastCreatedMinimapIcon
}
function CreateMinimapIconAtLocBJ(where, red, green, blue, pingPath, fogVisibility) {
    bj_lastCreatedMinimapIcon = CreateMinimapIconAtLoc(where, red, green, blue, pingPath, fogVisibility)
    return bj_lastCreatedMinimapIcon
}
function CreateMinimapIconBJ(x, y, red, green, blue, pingPath, fogVisibility) {
    bj_lastCreatedMinimapIcon = CreateMinimapIcon(x, y, red, green, blue, pingPath, fogVisibility)
    return bj_lastCreatedMinimapIcon
}
function CampaignMinimapIconUnitBJ(whichUnit, style) {
    var red;
    var green;
    var blue;
    var path;
    if ((style == bj_CAMPPINGSTYLE_PRIMARY)) {
        red = 255
        green = 0
        blue = 0
        path = SkinManagerGetLocalPath("MinimapQuestObjectivePrimary")
    }
    else {
        if ((style == bj_CAMPPINGSTYLE_PRIMARY_GREEN)) {
            red = 0
            green = 255
            blue = 0
            path = SkinManagerGetLocalPath("MinimapQuestObjectivePrimary")
        }
        else {
            if ((style == bj_CAMPPINGSTYLE_PRIMARY_RED)) {
                red = 255
                green = 0
                blue = 0
                path = SkinManagerGetLocalPath("MinimapQuestObjectivePrimary")
            }
            else {
                if ((style == bj_CAMPPINGSTYLE_BONUS)) {
                    red = 255
                    green = 255
                    blue = 0
                    path = SkinManagerGetLocalPath("MinimapQuestObjectiveBonus")
                }
                else {
                    if ((style == bj_CAMPPINGSTYLE_TURNIN)) {
                        red = 255
                        green = 255
                        blue = 0
                        path = SkinManagerGetLocalPath("MinimapQuestTurnIn")
                    }
                    else {
                        if ((style == bj_CAMPPINGSTYLE_BOSS)) {
                            red = 255
                            green = 0
                            blue = 0
                            path = SkinManagerGetLocalPath("MinimapQuestBoss")
                        }
                        else {
                            if ((style == bj_CAMPPINGSTYLE_CONTROL_ALLY)) {
                                red = 0
                                green = 255
                                blue = 0
                                path = SkinManagerGetLocalPath("MinimapQuestControlPoint")
                            }
                            else {
                                if ((style == bj_CAMPPINGSTYLE_CONTROL_NEUTRAL)) {
                                    red = 255
                                    green = 255
                                    blue = 255
                                    path = SkinManagerGetLocalPath("MinimapQuestControlPoint")
                                }
                                else {
                                    if ((style == bj_CAMPPINGSTYLE_CONTROL_ENEMY)) {
                                        red = 255
                                        green = 0
                                        blue = 0
                                        path = SkinManagerGetLocalPath("MinimapQuestControlPoint")
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    CreateMinimapIconOnUnitBJ(whichUnit, red, green, blue, path, FOG_OF_WAR_MASKED)
    SetMinimapIconOrphanDestroy(bj_lastCreatedMinimapIcon, true)
}
function CampaignMinimapIconLocBJ(where, style) {
    var red;
    var green;
    var blue;
    var path;
    if ((style == bj_CAMPPINGSTYLE_PRIMARY)) {
        red = 0
        green = 255
        blue = 0
        path = SkinManagerGetLocalPath("MinimapQuestObjectivePrimary")
    }
    else {
        if ((style == bj_CAMPPINGSTYLE_PRIMARY_GREEN)) {
            red = 0
            green = 255
            blue = 0
            path = SkinManagerGetLocalPath("MinimapQuestObjectivePrimary")
        }
        else {
            if ((style == bj_CAMPPINGSTYLE_PRIMARY_RED)) {
                red = 255
                green = 0
                blue = 0
                path = SkinManagerGetLocalPath("MinimapQuestObjectivePrimary")
            }
            else {
                if ((style == bj_CAMPPINGSTYLE_BONUS)) {
                    red = 255
                    green = 255
                    blue = 0
                    path = SkinManagerGetLocalPath("MinimapQuestObjectiveBonus")
                }
                else {
                    if ((style == bj_CAMPPINGSTYLE_TURNIN)) {
                        red = 255
                        green = 255
                        blue = 0
                        path = SkinManagerGetLocalPath("MinimapQuestTurnIn")
                    }
                    else {
                        if ((style == bj_CAMPPINGSTYLE_BOSS)) {
                            red = 255
                            green = 0
                            blue = 0
                            path = SkinManagerGetLocalPath("MinimapQuestBoss")
                        }
                        else {
                            if ((style == bj_CAMPPINGSTYLE_CONTROL_ALLY)) {
                                red = 0
                                green = 255
                                blue = 0
                                path = SkinManagerGetLocalPath("MinimapQuestControlPoint")
                            }
                            else {
                                if ((style == bj_CAMPPINGSTYLE_CONTROL_NEUTRAL)) {
                                    red = 255
                                    green = 255
                                    blue = 255
                                    path = SkinManagerGetLocalPath("MinimapQuestControlPoint")
                                }
                                else {
                                    if ((style == bj_CAMPPINGSTYLE_CONTROL_ENEMY)) {
                                        red = 255
                                        green = 0
                                        blue = 0
                                        path = SkinManagerGetLocalPath("MinimapQuestControlPoint")
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    CreateMinimapIconAtLocBJ(where, red, green, blue, path, FOG_OF_WAR_MASKED)
}
function PlaySoundBJ(soundHandle) {
    bj_lastPlayedSound = soundHandle
    if ((soundHandle != null)) {
        StartSound(soundHandle)
    }
}
function StopSoundBJ(soundHandle, fadeOut) {
    StopSound(soundHandle, false, fadeOut)
}
function SetSoundVolumeBJ(soundHandle, volumePercent) {
    SetSoundVolume(soundHandle, PercentToInt(volumePercent, 127))
}
function SetSoundOffsetBJ(newOffset, soundHandle) {
    SetSoundPlayPosition(soundHandle, R2I(newOffset * 1000))
}
function SetSoundDistanceCutoffBJ(soundHandle, cutoff) {
    SetSoundDistanceCutoff(soundHandle, cutoff)
}
function SetSoundPitchBJ(soundHandle, pitch) {
    SetSoundPitch(soundHandle, pitch)
}
function SetSoundPositionLocBJ(soundHandle, loc, z) {
    SetSoundPosition(soundHandle, GetLocationX(loc), GetLocationY(loc), z)
}
function AttachSoundToUnitBJ(soundHandle, whichUnit) {
    AttachSoundToUnit(soundHandle, whichUnit)
}
function SetSoundConeAnglesBJ(soundHandle, inside, outside, outsideVolumePercent) {
    SetSoundConeAngles(soundHandle, inside, outside, PercentToInt(outsideVolumePercent, 127))
}
function KillSoundWhenDoneBJ(soundHandle) {
    KillSoundWhenDone(soundHandle)
}
function PlaySoundAtPointBJ(soundHandle, volumePercent, loc, z) {
    SetSoundPositionLocBJ(soundHandle, loc, z)
    SetSoundVolumeBJ(soundHandle, volumePercent)
    PlaySoundBJ(soundHandle)
}
function PlaySoundOnUnitBJ(soundHandle, volumePercent, whichUnit) {
    AttachSoundToUnitBJ(soundHandle, whichUnit)
    SetSoundVolumeBJ(soundHandle, volumePercent)
    PlaySoundBJ(soundHandle)
}
function PlaySoundFromOffsetBJ(soundHandle, volumePercent, startingOffset) {
    SetSoundVolumeBJ(soundHandle, volumePercent)
    PlaySoundBJ(soundHandle)
    SetSoundOffsetBJ(startingOffset, soundHandle)
}
function PlayMusicBJ(musicFileName) {
    bj_lastPlayedMusic = musicFileName
    PlayMusic(musicFileName)
}
function PlayMusicExBJ(musicFileName, startingOffset, fadeInTime) {
    bj_lastPlayedMusic = musicFileName
    PlayMusicEx(musicFileName, R2I(startingOffset * 1000), R2I(fadeInTime * 1000))
}
function SetMusicOffsetBJ(newOffset) {
    SetMusicPlayPosition(R2I(newOffset * 1000))
}
function PlayThematicMusicBJ(musicName) {
    PlayThematicMusic(musicName)
}
function PlayThematicMusicExBJ(musicName, startingOffset) {
    PlayThematicMusicEx(musicName, R2I(startingOffset * 1000))
}
function SetThematicMusicOffsetBJ(newOffset) {
    SetThematicMusicPlayPosition(R2I(newOffset * 1000))
}
function EndThematicMusicBJ() {
    EndThematicMusic()
}
function StopMusicBJ(fadeOut) {
    StopMusic(fadeOut)
}
function ResumeMusicBJ() {
    ResumeMusic()
}
function SetMusicVolumeBJ(volumePercent) {
    SetMusicVolume(PercentToInt(volumePercent, 127))
}
function SetThematicMusicVolumeBJ(volumePercent) {
    SetThematicMusicVolume(PercentToInt(volumePercent, 127))
}
function GetSoundDurationBJ(soundHandle) {
    if ((soundHandle == null)) {
        return bj_NOTHING_SOUND_DURATION
    }
    else {
        return I2R(GetSoundDuration(soundHandle)) * 0.001
    }
}
function GetSoundFileDurationBJ(musicFileName) {
    return I2R(GetSoundFileDuration(musicFileName)) * 0.001
}
function GetLastPlayedSound() {
    return bj_lastPlayedSound
}
function GetLastPlayedMusic() {
    return bj_lastPlayedMusic
}
function VolumeGroupSetVolumeBJ(vgroup, percent) {
    VolumeGroupSetVolume(vgroup, percent * 0.01)
}
function SetCineModeVolumeGroupsImmediateBJ() {
    VolumeGroupSetVolume(SOUND_VOLUMEGROUP_UNITMOVEMENT, bj_CINEMODE_VOLUME_UNITMOVEMENT)
    VolumeGroupSetVolume(SOUND_VOLUMEGROUP_UNITSOUNDS, bj_CINEMODE_VOLUME_UNITSOUNDS)
    VolumeGroupSetVolume(SOUND_VOLUMEGROUP_COMBAT, bj_CINEMODE_VOLUME_COMBAT)
    VolumeGroupSetVolume(SOUND_VOLUMEGROUP_SPELLS, bj_CINEMODE_VOLUME_SPELLS)
    VolumeGroupSetVolume(SOUND_VOLUMEGROUP_UI, bj_CINEMODE_VOLUME_UI)
    VolumeGroupSetVolume(SOUND_VOLUMEGROUP_MUSIC, bj_CINEMODE_VOLUME_MUSIC)
    VolumeGroupSetVolume(SOUND_VOLUMEGROUP_AMBIENTSOUNDS, bj_CINEMODE_VOLUME_AMBIENTSOUNDS)
    VolumeGroupSetVolume(SOUND_VOLUMEGROUP_FIRE, bj_CINEMODE_VOLUME_FIRE)
}
function SetCineModeVolumeGroupsBJ() {
    if (bj_gameStarted) {
        SetCineModeVolumeGroupsImmediateBJ()
    }
    else {
        TimerStart(bj_volumeGroupsTimer, bj_GAME_STARTED_THRESHOLD, false, SetCineModeVolumeGroupsImmediateBJ)
    }
}
function SetSpeechVolumeGroupsImmediateBJ() {
    VolumeGroupSetVolume(SOUND_VOLUMEGROUP_UNITMOVEMENT, bj_SPEECH_VOLUME_UNITMOVEMENT)
    VolumeGroupSetVolume(SOUND_VOLUMEGROUP_UNITSOUNDS, bj_SPEECH_VOLUME_UNITSOUNDS)
    VolumeGroupSetVolume(SOUND_VOLUMEGROUP_COMBAT, bj_SPEECH_VOLUME_COMBAT)
    VolumeGroupSetVolume(SOUND_VOLUMEGROUP_SPELLS, bj_SPEECH_VOLUME_SPELLS)
    VolumeGroupSetVolume(SOUND_VOLUMEGROUP_UI, bj_SPEECH_VOLUME_UI)
    VolumeGroupSetVolume(SOUND_VOLUMEGROUP_MUSIC, bj_SPEECH_VOLUME_MUSIC)
    VolumeGroupSetVolume(SOUND_VOLUMEGROUP_AMBIENTSOUNDS, bj_SPEECH_VOLUME_AMBIENTSOUNDS)
    VolumeGroupSetVolume(SOUND_VOLUMEGROUP_FIRE, bj_SPEECH_VOLUME_FIRE)
}
function SetSpeechVolumeGroupsBJ() {
    if (bj_gameStarted) {
        SetSpeechVolumeGroupsImmediateBJ()
    }
    else {
        TimerStart(bj_volumeGroupsTimer, bj_GAME_STARTED_THRESHOLD, false, SetSpeechVolumeGroupsImmediateBJ)
    }
}
function VolumeGroupResetImmediateBJ() {
    VolumeGroupReset()
}
function VolumeGroupResetBJ() {
    if (bj_gameStarted) {
        VolumeGroupResetImmediateBJ()
    }
    else {
        TimerStart(bj_volumeGroupsTimer, bj_GAME_STARTED_THRESHOLD, false, VolumeGroupResetImmediateBJ)
    }
}
function GetSoundIsPlayingBJ(soundHandle) {
    return GetSoundIsLoading(soundHandle) || GetSoundIsPlaying(soundHandle)
}
function WaitForSoundBJ(soundHandle, offset) {
    TriggerWaitForSound(soundHandle, offset)
}
function SetMapMusicIndexedBJ(musicName, index) {
    SetMapMusic(musicName, false, index)
}
function SetMapMusicRandomBJ(musicName) {
    SetMapMusic(musicName, true, 0)
}
function ClearMapMusicBJ() {
    ClearMapMusic()
}
function SetStackedSoundBJ(add, soundHandle, r) {
    var width = GetRectMaxX(r) - GetRectMinX(r);
    var height = GetRectMaxY(r) - GetRectMinY(r);
    SetSoundPosition(soundHandle, GetRectCenterX(r), GetRectCenterY(r), 0)
    if (add) {
        RegisterStackedSound(soundHandle, true, width, height)
    }
    else {
        UnregisterStackedSound(soundHandle, true, width, height)
    }
}
function StartSoundForPlayerBJ(whichPlayer, soundHandle) {
    if ((whichPlayer == GetLocalPlayer())) {
        StartSound(soundHandle)
    }
}
function VolumeGroupSetVolumeForPlayerBJ(whichPlayer, vgroup, scale) {
    if ((GetLocalPlayer() == whichPlayer)) {
        VolumeGroupSetVolume(vgroup, scale)
    }
}
function EnableDawnDusk(flag) {
    bj_useDawnDuskSounds = flag
}
function IsDawnDuskEnabled() {
    return bj_useDawnDuskSounds
}
function SetAmbientDaySound(inLabel) {
    var ToD;
    if ((bj_dayAmbientSound != null)) {
        StopSound(bj_dayAmbientSound, true, true)
    }
    bj_dayAmbientSound = CreateMIDISound(inLabel, 20, 20)
    ToD = GetTimeOfDay()
    if ((ToD >= bj_TOD_DAWN && ToD < bj_TOD_DUSK)) {
        StartSound(bj_dayAmbientSound)
    }
}
function SetAmbientNightSound(inLabel) {
    var ToD;
    if ((bj_nightAmbientSound != null)) {
        StopSound(bj_nightAmbientSound, true, true)
    }
    bj_nightAmbientSound = CreateMIDISound(inLabel, 20, 20)
    ToD = GetTimeOfDay()
    if ((ToD < bj_TOD_DAWN || ToD >= bj_TOD_DUSK)) {
        StartSound(bj_nightAmbientSound)
    }
}
function AddSpecialEffectLocBJ(where, modelName) {
    bj_lastCreatedEffect = AddSpecialEffectLoc(modelName, where)
    return bj_lastCreatedEffect
}
function AddSpecialEffectTargetUnitBJ(attachPointName, targetWidget, modelName) {
    bj_lastCreatedEffect = AddSpecialEffectTarget(modelName, targetWidget, attachPointName)
    return bj_lastCreatedEffect
}
function DestroyEffectBJ(whichEffect) {
    DestroyEffect(whichEffect)
}
function GetLastCreatedEffectBJ() {
    return bj_lastCreatedEffect
}
function CreateCommandButtonEffectBJ(abilityId, order) {
    bj_lastCreatedCommandButtonEffect = CreateCommandButtonEffect(abilityId, order)
    return bj_lastCreatedCommandButtonEffect
}
function CreateTrainCommandButtonEffectBJ(unitId) {
    bj_lastCreatedCommandButtonEffect = CreateCommandButtonEffect('Aque', UnitId2String(unitId))
    return bj_lastCreatedCommandButtonEffect
}
function CreateUpgradeCommandButtonEffectBJ(techId) {
    bj_lastCreatedCommandButtonEffect = CreateUpgradeCommandButtonEffect(techId)
    return bj_lastCreatedCommandButtonEffect
}
function CreateCommonCommandButtonEffectBJ(order) {
    bj_lastCreatedCommandButtonEffect = CreateCommandButtonEffect(0, order)
    return bj_lastCreatedCommandButtonEffect
}
function CreateLearnCommandButtonEffectBJ(abilityId) {
    bj_lastCreatedCommandButtonEffect = CreateLearnCommandButtonEffect(abilityId)
    return bj_lastCreatedCommandButtonEffect
}
function CreateBuildCommandButtonEffectBJ(unitId) {
    var r = GetPlayerRace(GetLocalPlayer());
    var abilityId;
    if ((r == RACE_HUMAN)) {
        abilityId = 'AHbu'
    }
    else {
        if ((r == RACE_ORC)) {
            abilityId = 'AObu'
        }
        else {
            if ((r == RACE_UNDEAD)) {
                abilityId = 'AUbu'
            }
            else {
                if ((r == RACE_NIGHTELF)) {
                    abilityId = 'AEbu'
                }
                else {
                    abilityId = 'ANbu'
                }
            }
        }
    }
    bj_lastCreatedCommandButtonEffect = CreateCommandButtonEffect(abilityId, UnitId2String(unitId))
    return bj_lastCreatedCommandButtonEffect
}
function GetLastCreatedCommandButtonEffectBJ() {
    return bj_lastCreatedCommandButtonEffect
}
function GetItemLoc(whichItem) {
    return Location(GetItemX(whichItem), GetItemY(whichItem))
}
function GetItemLifeBJ(whichWidget) {
    return GetWidgetLife(whichWidget)
}
function SetItemLifeBJ(whichWidget, life) {
    SetWidgetLife(whichWidget, life)
}
function AddHeroXPSwapped(xpToAdd, whichHero, showEyeCandy) {
    AddHeroXP(whichHero, xpToAdd, showEyeCandy)
}
function SetHeroLevelBJ(whichHero, newLevel, showEyeCandy) {
    var oldLevel = GetHeroLevel(whichHero);
    if ((newLevel > oldLevel)) {
        SetHeroLevel(whichHero, newLevel, showEyeCandy)
    }
    else {
        if ((newLevel < oldLevel)) {
            UnitStripHeroLevel(whichHero, oldLevel - newLevel)
        }
        else {
        }
    }
}
function DecUnitAbilityLevelSwapped(abilcode, whichUnit) {
    return DecUnitAbilityLevel(whichUnit, abilcode)
}
function IncUnitAbilityLevelSwapped(abilcode, whichUnit) {
    return IncUnitAbilityLevel(whichUnit, abilcode)
}
function SetUnitAbilityLevelSwapped(abilcode, whichUnit, level) {
    return SetUnitAbilityLevel(whichUnit, abilcode, level)
}
function GetUnitAbilityLevelSwapped(abilcode, whichUnit) {
    return GetUnitAbilityLevel(whichUnit, abilcode)
}
function UnitHasBuffBJ(whichUnit, buffcode) {
    return (GetUnitAbilityLevel(whichUnit, buffcode) > 0)
}
function UnitRemoveBuffBJ(buffcode, whichUnit) {
    return UnitRemoveAbility(whichUnit, buffcode)
}
function UnitAddItemSwapped(whichItem, whichHero) {
    return UnitAddItem(whichHero, whichItem)
}
function UnitAddItemByIdSwapped(itemId, whichHero) {
    bj_lastCreatedItem = CreateItem(itemId, GetUnitX(whichHero), GetUnitY(whichHero))
    UnitAddItem(whichHero, bj_lastCreatedItem)
    return bj_lastCreatedItem
}
function UnitRemoveItemSwapped(whichItem, whichHero) {
    bj_lastRemovedItem = whichItem
    UnitRemoveItem(whichHero, whichItem)
}
function UnitRemoveItemFromSlotSwapped(itemSlot, whichHero) {
    bj_lastRemovedItem = UnitRemoveItemFromSlot(whichHero, itemSlot - 1)
    return bj_lastRemovedItem
}
function CreateItemLoc(itemId, loc) {
    bj_lastCreatedItem = CreateItem(itemId, GetLocationX(loc), GetLocationY(loc))
    return bj_lastCreatedItem
}
function GetLastCreatedItem() {
    return bj_lastCreatedItem
}
function GetLastRemovedItem() {
    return bj_lastRemovedItem
}
function SetItemPositionLoc(whichItem, loc) {
    SetItemPosition(whichItem, GetLocationX(loc), GetLocationY(loc))
}
function GetLearnedSkillBJ() {
    return GetLearnedSkill()
}
function SuspendHeroXPBJ(flag, whichHero) {
    SuspendHeroXP(whichHero, !flag)
}
function SetPlayerHandicapDamageBJ(whichPlayer, handicapPercent) {
    SetPlayerHandicapDamage(whichPlayer, handicapPercent * 0.01)
}
function GetPlayerHandicapDamageBJ(whichPlayer) {
    return GetPlayerHandicapDamage(whichPlayer) * 100
}
function SetPlayerHandicapReviveTimeBJ(whichPlayer, handicapPercent) {
    SetPlayerHandicapReviveTime(whichPlayer, handicapPercent * 0.01)
}
function GetPlayerHandicapReviveTimeBJ(whichPlayer) {
    return GetPlayerHandicapReviveTime(whichPlayer) * 100
}
function SetPlayerHandicapXPBJ(whichPlayer, handicapPercent) {
    SetPlayerHandicapXP(whichPlayer, handicapPercent * 0.01)
}
function GetPlayerHandicapXPBJ(whichPlayer) {
    return GetPlayerHandicapXP(whichPlayer) * 100
}
function SetPlayerHandicapBJ(whichPlayer, handicapPercent) {
    SetPlayerHandicap(whichPlayer, handicapPercent * 0.01)
}
function GetPlayerHandicapBJ(whichPlayer) {
    return GetPlayerHandicap(whichPlayer) * 100
}
function GetHeroStatBJ(whichStat, whichHero, includeBonuses) {
    if ((whichStat == bj_HEROSTAT_STR)) {
        return GetHeroStr(whichHero, includeBonuses)
    }
    else {
        if ((whichStat == bj_HEROSTAT_AGI)) {
            return GetHeroAgi(whichHero, includeBonuses)
        }
        else {
            if ((whichStat == bj_HEROSTAT_INT)) {
                return GetHeroInt(whichHero, includeBonuses)
            }
            else {
                return 0
            }
        }
    }
}
function SetHeroStat(whichHero, whichStat, value) {
    if ((value <= 0)) {
        return
    }
    if ((whichStat == bj_HEROSTAT_STR)) {
        SetHeroStr(whichHero, value, true)
    }
    else {
        if ((whichStat == bj_HEROSTAT_AGI)) {
            SetHeroAgi(whichHero, value, true)
        }
        else {
            if ((whichStat == bj_HEROSTAT_INT)) {
                SetHeroInt(whichHero, value, true)
            }
            else {
            }
        }
    }
}
function ModifyHeroStat(whichStat, whichHero, modifyMethod, value) {
    if ((modifyMethod == bj_MODIFYMETHOD_ADD)) {
        SetHeroStat(whichHero, whichStat, GetHeroStatBJ(whichStat, whichHero, false) + value)
    }
    else {
        if ((modifyMethod == bj_MODIFYMETHOD_SUB)) {
            SetHeroStat(whichHero, whichStat, GetHeroStatBJ(whichStat, whichHero, false) - value)
        }
        else {
            if ((modifyMethod == bj_MODIFYMETHOD_SET)) {
                SetHeroStat(whichHero, whichStat, value)
            }
            else {
            }
        }
    }
}
function ModifyHeroSkillPoints(whichHero, modifyMethod, value) {
    if ((modifyMethod == bj_MODIFYMETHOD_ADD)) {
        return UnitModifySkillPoints(whichHero, value)
    }
    else {
        if ((modifyMethod == bj_MODIFYMETHOD_SUB)) {
            return UnitModifySkillPoints(whichHero, -value)
        }
        else {
            if ((modifyMethod == bj_MODIFYMETHOD_SET)) {
                return UnitModifySkillPoints(whichHero, value - GetHeroSkillPoints(whichHero))
            }
            else {
                return false
            }
        }
    }
}
function UnitDropItemPointBJ(whichUnit, whichItem, x, y) {
    return UnitDropItemPoint(whichUnit, whichItem, x, y)
}
function UnitDropItemPointLoc(whichUnit, whichItem, loc) {
    return UnitDropItemPoint(whichUnit, whichItem, GetLocationX(loc), GetLocationY(loc))
}
function UnitDropItemSlotBJ(whichUnit, whichItem, slot) {
    return UnitDropItemSlot(whichUnit, whichItem, slot - 1)
}
function UnitDropItemTargetBJ(whichUnit, whichItem, target) {
    return UnitDropItemTarget(whichUnit, whichItem, target)
}
function UnitUseItemDestructable(whichUnit, whichItem, target) {
    return UnitUseItemTarget(whichUnit, whichItem, target)
}
function UnitUseItemPointLoc(whichUnit, whichItem, loc) {
    return UnitUseItemPoint(whichUnit, whichItem, GetLocationX(loc), GetLocationY(loc))
}
function UnitItemInSlotBJ(whichUnit, itemSlot) {
    return UnitItemInSlot(whichUnit, itemSlot - 1)
}
function GetInventoryIndexOfItemTypeBJ(whichUnit, itemId) {
    var index;
    var indexItem;
    index = 0
    do {
        indexItem = UnitItemInSlot(whichUnit, index)
        if ((indexItem != null) && (GetItemTypeId(indexItem) == itemId)) {
            return index + 1
        }
        index = index + 1
        if (index >= bj_MAX_INVENTORY) { break; }
    } while (true)
    return 0
}
function GetItemOfTypeFromUnitBJ(whichUnit, itemId) {
    var index = GetInventoryIndexOfItemTypeBJ(whichUnit, itemId);
    if ((index == 0)) {
        return null
    }
    else {
        return UnitItemInSlot(whichUnit, index - 1)
    }
}
function UnitHasItemOfTypeBJ(whichUnit, itemId) {
    return GetInventoryIndexOfItemTypeBJ(whichUnit, itemId) > 0
}
function UnitInventoryCount(whichUnit) {
    var index = 0;
    var count = 0;
    do {
        if ((UnitItemInSlot(whichUnit, index) != null)) {
            count = count + 1
        }
        index = index + 1
        if (index >= bj_MAX_INVENTORY) { break; }
    } while (true)
    return count
}
function UnitInventorySizeBJ(whichUnit) {
    return UnitInventorySize(whichUnit)
}
function SetItemInvulnerableBJ(whichItem, flag) {
    SetItemInvulnerable(whichItem, flag)
}
function SetItemDropOnDeathBJ(whichItem, flag) {
    SetItemDropOnDeath(whichItem, flag)
}
function SetItemDroppableBJ(whichItem, flag) {
    SetItemDroppable(whichItem, flag)
}
function SetItemPlayerBJ(whichItem, whichPlayer, changeColor) {
    SetItemPlayer(whichItem, whichPlayer, changeColor)
}
function SetItemVisibleBJ(show, whichItem) {
    SetItemVisible(whichItem, show)
}
function IsItemHiddenBJ(whichItem) {
    return !IsItemVisible(whichItem)
}
function ChooseRandomItemBJ(level) {
    return ChooseRandomItem(level)
}
function ChooseRandomItemExBJ(level, whichType) {
    return ChooseRandomItemEx(whichType, level)
}
function ChooseRandomNPBuildingBJ() {
    return ChooseRandomNPBuilding()
}
function ChooseRandomCreepBJ(level) {
    return ChooseRandomCreep(level)
}
function EnumItemsInRectBJ(r, actionFunc) {
    EnumItemsInRect(r, null, actionFunc)
}
function RandomItemInRectBJEnum() {
    bj_itemRandomConsidered = bj_itemRandomConsidered + 1
    if ((GetRandomInt(1, bj_itemRandomConsidered) == 1)) {
        bj_itemRandomCurrentPick = GetEnumItem()
    }
}
function RandomItemInRectBJ(r, filter) {
    bj_itemRandomConsidered = 0
    bj_itemRandomCurrentPick = null
    EnumItemsInRect(r, filter, RandomItemInRectBJEnum)
    DestroyBoolExpr(filter)
    return bj_itemRandomCurrentPick
}
function RandomItemInRectSimpleBJ(r) {
    return RandomItemInRectBJ(r, null)
}
function CheckItemStatus(whichItem, status) {
    if ((status == bj_ITEM_STATUS_HIDDEN)) {
        return !IsItemVisible(whichItem)
    }
    else {
        if ((status == bj_ITEM_STATUS_OWNED)) {
            return IsItemOwned(whichItem)
        }
        else {
            if ((status == bj_ITEM_STATUS_INVULNERABLE)) {
                return IsItemInvulnerable(whichItem)
            }
            else {
                if ((status == bj_ITEM_STATUS_POWERUP)) {
                    return IsItemPowerup(whichItem)
                }
                else {
                    if ((status == bj_ITEM_STATUS_SELLABLE)) {
                        return IsItemSellable(whichItem)
                    }
                    else {
                        if ((status == bj_ITEM_STATUS_PAWNABLE)) {
                            return IsItemPawnable(whichItem)
                        }
                        else {
                            return false
                        }
                    }
                }
            }
        }
    }
}
function CheckItemcodeStatus(itemId, status) {
    if ((status == bj_ITEMCODE_STATUS_POWERUP)) {
        return IsItemIdPowerup(itemId)
    }
    else {
        if ((status == bj_ITEMCODE_STATUS_SELLABLE)) {
            return IsItemIdSellable(itemId)
        }
        else {
            if ((status == bj_ITEMCODE_STATUS_PAWNABLE)) {
                return IsItemIdPawnable(itemId)
            }
            else {
                return false
            }
        }
    }
}
function UnitId2OrderIdBJ(unitId) {
    return unitId
}
function String2UnitIdBJ(unitIdString) {
    return UnitId(unitIdString)
}
function UnitId2StringBJ(unitId) {
    var unitString = UnitId2String(unitId);
    if ((unitString != null)) {
        return unitString
    }
    return ""
}
function String2OrderIdBJ(orderIdString) {
    var orderId;
    orderId = OrderId(orderIdString)
    if ((orderId != 0)) {
        return orderId
    }
    orderId = UnitId(orderIdString)
    if ((orderId != 0)) {
        return orderId
    }
    return 0
}
function OrderId2StringBJ(orderId) {
    var orderString;
    orderString = OrderId2String(orderId)
    if ((orderString != null)) {
        return orderString
    }
    orderString = UnitId2String(orderId)
    if ((orderString != null)) {
        return orderString
    }
    return ""
}
function GetIssuedOrderIdBJ() {
    return GetIssuedOrderId()
}
function GetKillingUnitBJ() {
    return GetKillingUnit()
}
function CreateUnitAtLocSaveLast(id, unitid, loc, face) {
    if ((unitid == 'ugol')) {
        bj_lastCreatedUnit = CreateBlightedGoldmine(id, GetLocationX(loc), GetLocationY(loc), face)
    }
    else {
        bj_lastCreatedUnit = CreateUnitAtLoc(id, unitid, loc, face)
    }
    return bj_lastCreatedUnit
}
function GetLastCreatedUnit() {
    return bj_lastCreatedUnit
}
function CreateNUnitsAtLoc(count, unitId, whichPlayer, loc, face) {
    GroupClear(bj_lastCreatedGroup)
    do {
        count = count - 1
        if (count < 0) { break; }
        CreateUnitAtLocSaveLast(whichPlayer, unitId, loc, face)
        GroupAddUnit(bj_lastCreatedGroup, bj_lastCreatedUnit)
    } while (true)
    return bj_lastCreatedGroup
}
function CreateNUnitsAtLocFacingLocBJ(count, unitId, whichPlayer, loc, lookAt) {
    return CreateNUnitsAtLoc(count, unitId, whichPlayer, loc, AngleBetweenPoints(loc, lookAt))
}
function GetLastCreatedGroupEnum() {
    GroupAddUnit(bj_groupLastCreatedDest, GetEnumUnit())
}
function GetLastCreatedGroup() {
    bj_groupLastCreatedDest = CreateGroup()
    ForGroup(bj_lastCreatedGroup, GetLastCreatedGroupEnum)
    return bj_groupLastCreatedDest
}
function CreateCorpseLocBJ(unitid, whichPlayer, loc) {
    bj_lastCreatedUnit = CreateCorpse(whichPlayer, unitid, GetLocationX(loc), GetLocationY(loc), GetRandomReal(0, 360))
    return bj_lastCreatedUnit
}
function UnitSuspendDecayBJ(suspend, whichUnit) {
    UnitSuspendDecay(whichUnit, suspend)
}
function DelayedSuspendDecayStopAnimEnum() {
    var enumUnit = GetEnumUnit();
    if ((GetUnitState(enumUnit, UNIT_STATE_LIFE) <= 0)) {
        SetUnitTimeScale(enumUnit, 0.0001)
    }
}
function DelayedSuspendDecayBoneEnum() {
    var enumUnit = GetEnumUnit();
    if ((GetUnitState(enumUnit, UNIT_STATE_LIFE) <= 0)) {
        UnitSuspendDecay(enumUnit, true)
        SetUnitTimeScale(enumUnit, 0.0001)
    }
}
function DelayedSuspendDecayFleshEnum() {
    var enumUnit = GetEnumUnit();
    if ((GetUnitState(enumUnit, UNIT_STATE_LIFE) <= 0)) {
        UnitSuspendDecay(enumUnit, true)
        SetUnitTimeScale(enumUnit, 10.0)
        SetUnitAnimation(enumUnit, "decay flesh")
    }
}
function DelayedSuspendDecay() {
    var boneGroup;
    var fleshGroup;
    boneGroup = bj_suspendDecayBoneGroup
    fleshGroup = bj_suspendDecayFleshGroup
    bj_suspendDecayBoneGroup = CreateGroup()
    bj_suspendDecayFleshGroup = CreateGroup()
    ForGroup(fleshGroup, DelayedSuspendDecayStopAnimEnum)
    ForGroup(boneGroup, DelayedSuspendDecayStopAnimEnum)
    TriggerSleepAction(bj_CORPSE_MAX_DEATH_TIME)
    ForGroup(fleshGroup, DelayedSuspendDecayFleshEnum)
    ForGroup(boneGroup, DelayedSuspendDecayBoneEnum)
    TriggerSleepAction(0.05)
    ForGroup(fleshGroup, DelayedSuspendDecayStopAnimEnum)
    DestroyGroup(boneGroup)
    DestroyGroup(fleshGroup)
}
function DelayedSuspendDecayCreate() {
    bj_delayedSuspendDecayTrig = CreateTrigger()
    TriggerRegisterTimerExpireEvent(bj_delayedSuspendDecayTrig, bj_delayedSuspendDecayTimer)
    TriggerAddAction(bj_delayedSuspendDecayTrig, DelayedSuspendDecay)
}
function CreatePermanentCorpseLocBJ(style, unitid, whichPlayer, loc, facing) {
    bj_lastCreatedUnit = CreateCorpse(whichPlayer, unitid, GetLocationX(loc), GetLocationY(loc), facing)
    SetUnitBlendTime(bj_lastCreatedUnit, 0)
    if ((style == bj_CORPSETYPE_FLESH)) {
        SetUnitAnimation(bj_lastCreatedUnit, "decay flesh")
        GroupAddUnit(bj_suspendDecayFleshGroup, bj_lastCreatedUnit)
    }
    else {
        if ((style == bj_CORPSETYPE_BONE)) {
            SetUnitAnimation(bj_lastCreatedUnit, "decay bone")
            GroupAddUnit(bj_suspendDecayBoneGroup, bj_lastCreatedUnit)
        }
        else {
            SetUnitAnimation(bj_lastCreatedUnit, "decay bone")
            GroupAddUnit(bj_suspendDecayBoneGroup, bj_lastCreatedUnit)
        }
    }
    TimerStart(bj_delayedSuspendDecayTimer, 0.05, false, null)
    return bj_lastCreatedUnit
}
function GetUnitStateSwap(whichState, whichUnit) {
    return GetUnitState(whichUnit, whichState)
}
function GetUnitStatePercent(whichUnit, whichState, whichMaxState) {
    var value = GetUnitState(whichUnit, whichState);
    var maxValue = GetUnitState(whichUnit, whichMaxState);
    if ((whichUnit == null) || (maxValue == 0)) {
        return 0.0
    }
    return value / maxValue * 100.0
}
function GetUnitLifePercent(whichUnit) {
    return GetUnitStatePercent(whichUnit, UNIT_STATE_LIFE, UNIT_STATE_MAX_LIFE)
}
function GetUnitManaPercent(whichUnit) {
    return GetUnitStatePercent(whichUnit, UNIT_STATE_MANA, UNIT_STATE_MAX_MANA)
}
function SelectUnitSingle(whichUnit) {
    ClearSelection()
    SelectUnit(whichUnit, true)
}
function SelectGroupBJEnum() {
    SelectUnit(GetEnumUnit(), true)
}
function SelectGroupBJ(g) {
    ClearSelection()
    ForGroup(g, SelectGroupBJEnum)
}
function SelectUnitAdd(whichUnit) {
    SelectUnit(whichUnit, true)
}
function SelectUnitRemove(whichUnit) {
    SelectUnit(whichUnit, false)
}
function ClearSelectionForPlayer(whichPlayer) {
    if ((GetLocalPlayer() == whichPlayer)) {
        ClearSelection()
    }
}
function SelectUnitForPlayerSingle(whichUnit, whichPlayer) {
    if ((GetLocalPlayer() == whichPlayer)) {
        ClearSelection()
        SelectUnit(whichUnit, true)
    }
}
function SelectGroupForPlayerBJ(g, whichPlayer) {
    if ((GetLocalPlayer() == whichPlayer)) {
        ClearSelection()
        ForGroup(g, SelectGroupBJEnum)
    }
}
function SelectUnitAddForPlayer(whichUnit, whichPlayer) {
    if ((GetLocalPlayer() == whichPlayer)) {
        SelectUnit(whichUnit, true)
    }
}
function SelectUnitRemoveForPlayer(whichUnit, whichPlayer) {
    if ((GetLocalPlayer() == whichPlayer)) {
        SelectUnit(whichUnit, false)
    }
}
function SetUnitLifeBJ(whichUnit, newValue) {
    SetUnitState(whichUnit, UNIT_STATE_LIFE, RMaxBJ(0, newValue))
}
function SetUnitManaBJ(whichUnit, newValue) {
    SetUnitState(whichUnit, UNIT_STATE_MANA, RMaxBJ(0, newValue))
}
function SetUnitLifePercentBJ(whichUnit, percent) {
    SetUnitState(whichUnit, UNIT_STATE_LIFE, GetUnitState(whichUnit, UNIT_STATE_MAX_LIFE) * RMaxBJ(0, percent) * 0.01)
}
function SetUnitManaPercentBJ(whichUnit, percent) {
    SetUnitState(whichUnit, UNIT_STATE_MANA, GetUnitState(whichUnit, UNIT_STATE_MAX_MANA) * RMaxBJ(0, percent) * 0.01)
}
function IsUnitDeadBJ(whichUnit) {
    return GetUnitState(whichUnit, UNIT_STATE_LIFE) <= 0
}
function IsUnitAliveBJ(whichUnit) {
    return !IsUnitDeadBJ(whichUnit)
}
function IsUnitGroupDeadBJEnum() {
    if (!IsUnitDeadBJ(GetEnumUnit())) {
        bj_isUnitGroupDeadResult = false
    }
}
function IsUnitGroupDeadBJ(g) {
    var wantDestroy = bj_wantDestroyGroup;
    bj_wantDestroyGroup = false
    bj_isUnitGroupDeadResult = true
    ForGroup(g, IsUnitGroupDeadBJEnum)
    if ((wantDestroy)) {
        DestroyGroup(g)
    }
    return bj_isUnitGroupDeadResult
}
function IsUnitGroupEmptyBJEnum() {
    bj_isUnitGroupEmptyResult = false
}
function IsUnitGroupEmptyBJ(g) {
    var wantDestroy = bj_wantDestroyGroup;
    bj_wantDestroyGroup = false
    bj_isUnitGroupEmptyResult = true
    ForGroup(g, IsUnitGroupEmptyBJEnum)
    if ((wantDestroy)) {
        DestroyGroup(g)
    }
    return bj_isUnitGroupEmptyResult
}
function IsUnitGroupInRectBJEnum() {
    if (!RectContainsUnit(bj_isUnitGroupInRectRect, GetEnumUnit())) {
        bj_isUnitGroupInRectResult = false
    }
}
function IsUnitGroupInRectBJ(g, r) {
    bj_isUnitGroupInRectResult = true
    bj_isUnitGroupInRectRect = r
    ForGroup(g, IsUnitGroupInRectBJEnum)
    return bj_isUnitGroupInRectResult
}
function IsUnitHiddenBJ(whichUnit) {
    return IsUnitHidden(whichUnit)
}
function ShowUnitHide(whichUnit) {
    ShowUnit(whichUnit, false)
}
function ShowUnitShow(whichUnit) {
    if ((IsUnitType(whichUnit, UNIT_TYPE_HERO) && IsUnitDeadBJ(whichUnit))) {
        return
    }
    ShowUnit(whichUnit, true)
}
function IssueHauntOrderAtLocBJFilter() {
    return GetUnitTypeId(GetFilterUnit()) == 'ngol'
}
function IssueHauntOrderAtLocBJ(whichPeon, loc) {
    var g = null;
    var goldMine = null;
    g = CreateGroup()
    GroupEnumUnitsInRangeOfLoc(g, loc, 2 * bj_CELLWIDTH, filterIssueHauntOrderAtLocBJ)
    goldMine = FirstOfGroup(g)
    DestroyGroup(g)
    if ((goldMine == null)) {
        return false
    }
    return IssueTargetOrderById(whichPeon, 'ugol', goldMine)
}
function IssueBuildOrderByIdLocBJ(whichPeon, unitId, loc) {
    if ((unitId == 'ugol')) {
        return IssueHauntOrderAtLocBJ(whichPeon, loc)
    }
    else {
        return IssueBuildOrderById(whichPeon, unitId, GetLocationX(loc), GetLocationY(loc))
    }
}
function IssueTrainOrderByIdBJ(whichUnit, unitId) {
    return IssueImmediateOrderById(whichUnit, unitId)
}
function GroupTrainOrderByIdBJ(g, unitId) {
    return GroupImmediateOrderById(g, unitId)
}
function IssueUpgradeOrderByIdBJ(whichUnit, techId) {
    return IssueImmediateOrderById(whichUnit, techId)
}
function GetAttackedUnitBJ() {
    return GetTriggerUnit()
}
function SetUnitFlyHeightBJ(whichUnit, newHeight, rate) {
    SetUnitFlyHeight(whichUnit, newHeight, rate)
}
function SetUnitTurnSpeedBJ(whichUnit, turnSpeed) {
    SetUnitTurnSpeed(whichUnit, turnSpeed)
}
function SetUnitPropWindowBJ(whichUnit, propWindow) {
    var angle = propWindow;
    if ((angle <= 0)) {
        angle = 1
    }
    else {
        if ((angle >= 360)) {
            angle = 359
        }
    }
    angle = angle * bj_DEGTORAD
    SetUnitPropWindow(whichUnit, angle)
}
function GetUnitPropWindowBJ(whichUnit) {
    return GetUnitPropWindow(whichUnit) * bj_RADTODEG
}
function GetUnitDefaultPropWindowBJ(whichUnit) {
    return GetUnitDefaultPropWindow(whichUnit)
}
function SetUnitBlendTimeBJ(whichUnit, blendTime) {
    SetUnitBlendTime(whichUnit, blendTime)
}
function SetUnitAcquireRangeBJ(whichUnit, acquireRange) {
    SetUnitAcquireRange(whichUnit, acquireRange)
}
function UnitSetCanSleepBJ(whichUnit, canSleep) {
    UnitAddSleep(whichUnit, canSleep)
}
function UnitCanSleepBJ(whichUnit) {
    return UnitCanSleep(whichUnit)
}
function UnitWakeUpBJ(whichUnit) {
    UnitWakeUp(whichUnit)
}
function UnitIsSleepingBJ(whichUnit) {
    return UnitIsSleeping(whichUnit)
}
function WakePlayerUnitsEnum() {
    UnitWakeUp(GetEnumUnit())
}
function WakePlayerUnits(whichPlayer) {
    var g = CreateGroup();
    GroupEnumUnitsOfPlayer(g, whichPlayer, null)
    ForGroup(g, WakePlayerUnitsEnum)
    DestroyGroup(g)
}
function EnableCreepSleepBJ(enable) {
    SetPlayerState(Player(PLAYER_NEUTRAL_AGGRESSIVE), PLAYER_STATE_NO_CREEP_SLEEP, IntegerTertiaryOp(enable, 0, 1))
    if ((!enable)) {
        WakePlayerUnits(Player(PLAYER_NEUTRAL_AGGRESSIVE))
    }
}
function UnitGenerateAlarms(whichUnit, generate) {
    return UnitIgnoreAlarm(whichUnit, !generate)
}
function DoesUnitGenerateAlarms(whichUnit) {
    return !UnitIgnoreAlarmToggled(whichUnit)
}
function PauseAllUnitsBJEnum() {
    PauseUnit(GetEnumUnit(), bj_pauseAllUnitsFlag)
}
function PauseAllUnitsBJ(pause) {
    var index;
    var indexPlayer;
    var g;
    bj_pauseAllUnitsFlag = pause
    g = CreateGroup()
    index = 0
    do {
        indexPlayer = Player(index)
        if ((GetPlayerController(indexPlayer) == MAP_CONTROL_COMPUTER)) {
            PauseCompAI(indexPlayer, pause)
        }
        GroupEnumUnitsOfPlayer(g, indexPlayer, null)
        ForGroup(g, PauseAllUnitsBJEnum)
        GroupClear(g)
        index = index + 1
        if (index == bj_MAX_PLAYER_SLOTS) { break; }
    } while (true)
    DestroyGroup(g)
}
function PauseUnitBJ(pause, whichUnit) {
    PauseUnit(whichUnit, pause)
}
function IsUnitPausedBJ(whichUnit) {
    return IsUnitPaused(whichUnit)
}
function UnitPauseTimedLifeBJ(flag, whichUnit) {
    UnitPauseTimedLife(whichUnit, flag)
}
function UnitApplyTimedLifeBJ(duration, buffId, whichUnit) {
    UnitApplyTimedLife(whichUnit, buffId, duration)
}
function UnitShareVisionBJ(share, whichUnit, whichPlayer) {
    UnitShareVision(whichUnit, whichPlayer, share)
}
function UnitRemoveBuffsBJ(buffType, whichUnit) {
    if ((buffType == bj_REMOVEBUFFS_POSITIVE)) {
        UnitRemoveBuffs(whichUnit, true, false)
    }
    else {
        if ((buffType == bj_REMOVEBUFFS_NEGATIVE)) {
            UnitRemoveBuffs(whichUnit, false, true)
        }
        else {
            if ((buffType == bj_REMOVEBUFFS_ALL)) {
                UnitRemoveBuffs(whichUnit, true, true)
            }
            else {
                if ((buffType == bj_REMOVEBUFFS_NONTLIFE)) {
                    UnitRemoveBuffsEx(whichUnit, true, true, false, false, false, true, false)
                }
                else {
                }
            }
        }
    }
}
function UnitRemoveBuffsExBJ(polarity, resist, whichUnit, bTLife, bAura) {
    var bPos = (polarity == bj_BUFF_POLARITY_EITHER) || (polarity == bj_BUFF_POLARITY_POSITIVE);
    var bNeg = (polarity == bj_BUFF_POLARITY_EITHER) || (polarity == bj_BUFF_POLARITY_NEGATIVE);
    var bMagic = (resist == bj_BUFF_RESIST_BOTH) || (resist == bj_BUFF_RESIST_MAGIC);
    var bPhys = (resist == bj_BUFF_RESIST_BOTH) || (resist == bj_BUFF_RESIST_PHYSICAL);
    UnitRemoveBuffsEx(whichUnit, bPos, bNeg, bMagic, bPhys, bTLife, bAura, false)
}
function UnitCountBuffsExBJ(polarity, resist, whichUnit, bTLife, bAura) {
    var bPos = (polarity == bj_BUFF_POLARITY_EITHER) || (polarity == bj_BUFF_POLARITY_POSITIVE);
    var bNeg = (polarity == bj_BUFF_POLARITY_EITHER) || (polarity == bj_BUFF_POLARITY_NEGATIVE);
    var bMagic = (resist == bj_BUFF_RESIST_BOTH) || (resist == bj_BUFF_RESIST_MAGIC);
    var bPhys = (resist == bj_BUFF_RESIST_BOTH) || (resist == bj_BUFF_RESIST_PHYSICAL);
    return UnitCountBuffsEx(whichUnit, bPos, bNeg, bMagic, bPhys, bTLife, bAura, false)
}
function UnitRemoveAbilityBJ(abilityId, whichUnit) {
    return UnitRemoveAbility(whichUnit, abilityId)
}
function UnitAddAbilityBJ(abilityId, whichUnit) {
    return UnitAddAbility(whichUnit, abilityId)
}
function UnitRemoveTypeBJ(whichType, whichUnit) {
    return UnitRemoveType(whichUnit, whichType)
}
function UnitAddTypeBJ(whichType, whichUnit) {
    return UnitAddType(whichUnit, whichType)
}
function UnitMakeAbilityPermanentBJ(permanent, abilityId, whichUnit) {
    return UnitMakeAbilityPermanent(whichUnit, permanent, abilityId)
}
function SetUnitExplodedBJ(whichUnit, exploded) {
    SetUnitExploded(whichUnit, exploded)
}
function ExplodeUnitBJ(whichUnit) {
    SetUnitExploded(whichUnit, true)
    KillUnit(whichUnit)
}
function GetTransportUnitBJ() {
    return GetTransportUnit()
}
function GetLoadedUnitBJ() {
    return GetLoadedUnit()
}
function IsUnitInTransportBJ(whichUnit, whichTransport) {
    return IsUnitInTransport(whichUnit, whichTransport)
}
function IsUnitLoadedBJ(whichUnit) {
    return IsUnitLoaded(whichUnit)
}
function IsUnitIllusionBJ(whichUnit) {
    return IsUnitIllusion(whichUnit)
}
function ReplaceUnitBJ(whichUnit, newUnitId, unitStateMethod) {
    var oldUnit = whichUnit;
    var newUnit;
    var wasHidden;
    var index;
    var indexItem;
    var oldRatio;
    if ((oldUnit == null)) {
        bj_lastReplacedUnit = oldUnit
        return oldUnit
    }
    wasHidden = IsUnitHidden(oldUnit)
    ShowUnit(oldUnit, false)
    if ((newUnitId == 'ugol')) {
        newUnit = CreateBlightedGoldmine(GetOwningPlayer(oldUnit), GetUnitX(oldUnit), GetUnitY(oldUnit), GetUnitFacing(oldUnit))
    }
    else {
        newUnit = CreateUnit(GetOwningPlayer(oldUnit), newUnitId, GetUnitX(oldUnit), GetUnitY(oldUnit), GetUnitFacing(oldUnit))
    }
    if ((unitStateMethod == bj_UNIT_STATE_METHOD_RELATIVE)) {
        if ((GetUnitState(oldUnit, UNIT_STATE_MAX_LIFE) > 0)) {
            oldRatio = GetUnitState(oldUnit, UNIT_STATE_LIFE) / GetUnitState(oldUnit, UNIT_STATE_MAX_LIFE)
            SetUnitState(newUnit, UNIT_STATE_LIFE, oldRatio * GetUnitState(newUnit, UNIT_STATE_MAX_LIFE))
        }
        if ((GetUnitState(oldUnit, UNIT_STATE_MAX_MANA) > 0) && (GetUnitState(newUnit, UNIT_STATE_MAX_MANA) > 0)) {
            oldRatio = GetUnitState(oldUnit, UNIT_STATE_MANA) / GetUnitState(oldUnit, UNIT_STATE_MAX_MANA)
            SetUnitState(newUnit, UNIT_STATE_MANA, oldRatio * GetUnitState(newUnit, UNIT_STATE_MAX_MANA))
        }
    }
    else {
        if ((unitStateMethod == bj_UNIT_STATE_METHOD_ABSOLUTE)) {
            SetUnitState(newUnit, UNIT_STATE_LIFE, GetUnitState(oldUnit, UNIT_STATE_LIFE))
            if ((GetUnitState(newUnit, UNIT_STATE_MAX_MANA) > 0)) {
                SetUnitState(newUnit, UNIT_STATE_MANA, GetUnitState(oldUnit, UNIT_STATE_MANA))
            }
        }
        else {
            if ((unitStateMethod == bj_UNIT_STATE_METHOD_DEFAULTS)) {
            }
            else {
                if ((unitStateMethod == bj_UNIT_STATE_METHOD_MAXIMUM)) {
                    SetUnitState(newUnit, UNIT_STATE_LIFE, GetUnitState(newUnit, UNIT_STATE_MAX_LIFE))
                    SetUnitState(newUnit, UNIT_STATE_MANA, GetUnitState(newUnit, UNIT_STATE_MAX_MANA))
                }
                else {
                }
            }
        }
    }
    SetResourceAmount(newUnit, GetResourceAmount(oldUnit))
    if ((IsUnitType(oldUnit, UNIT_TYPE_HERO) && IsUnitType(newUnit, UNIT_TYPE_HERO))) {
        SetHeroXP(newUnit, GetHeroXP(oldUnit), false)
        index = 0
        do {
            indexItem = UnitItemInSlot(oldUnit, index)
            if ((indexItem != null)) {
                UnitRemoveItem(oldUnit, indexItem)
                UnitAddItem(newUnit, indexItem)
            }
            index = index + 1
            if (index >= bj_MAX_INVENTORY) { break; }
        } while (true)
    }
    if (wasHidden) {
        KillUnit(oldUnit)
        RemoveUnit(oldUnit)
    }
    else {
        RemoveUnit(oldUnit)
    }
    bj_lastReplacedUnit = newUnit
    return newUnit
}
function GetLastReplacedUnitBJ() {
    return bj_lastReplacedUnit
}
function SetUnitPositionLocFacingBJ(whichUnit, loc, facing) {
    SetUnitPositionLoc(whichUnit, loc)
    SetUnitFacing(whichUnit, facing)
}
function SetUnitPositionLocFacingLocBJ(whichUnit, loc, lookAt) {
    SetUnitPositionLoc(whichUnit, loc)
    SetUnitFacing(whichUnit, AngleBetweenPoints(loc, lookAt))
}
function AddItemToStockBJ(itemId, whichUnit, currentStock, stockMax) {
    AddItemToStock(whichUnit, itemId, currentStock, stockMax)
}
function AddUnitToStockBJ(unitId, whichUnit, currentStock, stockMax) {
    AddUnitToStock(whichUnit, unitId, currentStock, stockMax)
}
function RemoveItemFromStockBJ(itemId, whichUnit) {
    RemoveItemFromStock(whichUnit, itemId)
}
function RemoveUnitFromStockBJ(unitId, whichUnit) {
    RemoveUnitFromStock(whichUnit, unitId)
}
function SetUnitUseFoodBJ(enable, whichUnit) {
    SetUnitUseFood(whichUnit, enable)
}
function UnitDamagePointLoc(whichUnit, delay, radius, loc, amount, whichAttack, whichDamage) {
    return UnitDamagePoint(whichUnit, delay, radius, GetLocationX(loc), GetLocationY(loc), amount, true, false, whichAttack, whichDamage, WEAPON_TYPE_WHOKNOWS)
}
function UnitDamageTargetBJ(whichUnit, target, amount, whichAttack, whichDamage) {
    return UnitDamageTarget(whichUnit, target, amount, true, false, whichAttack, whichDamage, WEAPON_TYPE_WHOKNOWS)
}
function CreateDestructableLoc(objectid, loc, facing, scale, variation) {
    bj_lastCreatedDestructable = CreateDestructable(objectid, GetLocationX(loc), GetLocationY(loc), facing, scale, variation)
    return bj_lastCreatedDestructable
}
function CreateDeadDestructableLocBJ(objectid, loc, facing, scale, variation) {
    bj_lastCreatedDestructable = CreateDeadDestructable(objectid, GetLocationX(loc), GetLocationY(loc), facing, scale, variation)
    return bj_lastCreatedDestructable
}
function GetLastCreatedDestructable() {
    return bj_lastCreatedDestructable
}
function ShowDestructableBJ(flag, d) {
    ShowDestructable(d, flag)
}
function SetDestructableInvulnerableBJ(d, flag) {
    SetDestructableInvulnerable(d, flag)
}
function IsDestructableInvulnerableBJ(d) {
    return IsDestructableInvulnerable(d)
}
function GetDestructableLoc(whichDestructable) {
    return Location(GetDestructableX(whichDestructable), GetDestructableY(whichDestructable))
}
function EnumDestructablesInRectAll(r, actionFunc) {
    EnumDestructablesInRect(r, null, actionFunc)
}
function EnumDestructablesInCircleBJFilter() {
    var destLoc = GetDestructableLoc(GetFilterDestructable());
    var result;
    result = DistanceBetweenPoints(destLoc, bj_enumDestructableCenter) <= bj_enumDestructableRadius
    RemoveLocation(destLoc)
    return result
}
function IsDestructableDeadBJ(d) {
    return GetDestructableLife(d) <= 0
}
function IsDestructableAliveBJ(d) {
    return !IsDestructableDeadBJ(d)
}
function RandomDestructableInRectBJEnum() {
    bj_destRandomConsidered = bj_destRandomConsidered + 1
    if ((GetRandomInt(1, bj_destRandomConsidered) == 1)) {
        bj_destRandomCurrentPick = GetEnumDestructable()
    }
}
function RandomDestructableInRectBJ(r, filter) {
    bj_destRandomConsidered = 0
    bj_destRandomCurrentPick = null
    EnumDestructablesInRect(r, filter, RandomDestructableInRectBJEnum)
    DestroyBoolExpr(filter)
    return bj_destRandomCurrentPick
}
function RandomDestructableInRectSimpleBJ(r) {
    return RandomDestructableInRectBJ(r, null)
}
function EnumDestructablesInCircleBJ(radius, loc, actionFunc) {
    var r;
    if ((radius >= 0)) {
        bj_enumDestructableCenter = loc
        bj_enumDestructableRadius = radius
        r = GetRectFromCircleBJ(loc, radius)
        EnumDestructablesInRect(r, filterEnumDestructablesInCircleBJ, actionFunc)
        RemoveRect(r)
    }
}
function SetDestructableLifePercentBJ(d, percent) {
    SetDestructableLife(d, GetDestructableMaxLife(d) * percent * 0.01)
}
function SetDestructableMaxLifeBJ(d, max) {
    SetDestructableMaxLife(d, max)
}
function ModifyGateBJ(gateOperation, d) {
    if ((gateOperation == bj_GATEOPERATION_CLOSE)) {
        if ((GetDestructableLife(d) <= 0)) {
            DestructableRestoreLife(d, GetDestructableMaxLife(d), true)
        }
        SetDestructableAnimation(d, "stand")
    }
    else {
        if ((gateOperation == bj_GATEOPERATION_OPEN)) {
            if ((GetDestructableLife(d) > 0)) {
                KillDestructable(d)
            }
            SetDestructableAnimation(d, "death alternate")
        }
        else {
            if ((gateOperation == bj_GATEOPERATION_DESTROY)) {
                if ((GetDestructableLife(d) > 0)) {
                    KillDestructable(d)
                }
                SetDestructableAnimation(d, "death")
            }
            else {
            }
        }
    }
}
function GetElevatorHeight(d) {
    var height;
    height = 1 + R2I(GetDestructableOccluderHeight(d) / bj_CLIFFHEIGHT)
    if ((height < 1) || (height > 3)) {
        height = 1
    }
    return height
}
function ChangeElevatorHeight(d, newHeight) {
    var oldHeight;
    newHeight = IMaxBJ(1, newHeight)
    newHeight = IMinBJ(3, newHeight)
    oldHeight = GetElevatorHeight(d)
    SetDestructableOccluderHeight(d, bj_CLIFFHEIGHT * (newHeight - 1))
    if ((newHeight == 1)) {
        if ((oldHeight == 2)) {
            SetDestructableAnimation(d, "birth")
            QueueDestructableAnimation(d, "stand")
        }
        else {
            if ((oldHeight == 3)) {
                SetDestructableAnimation(d, "birth third")
                QueueDestructableAnimation(d, "stand")
            }
            else {
                SetDestructableAnimation(d, "stand")
            }
        }
    }
    else {
        if ((newHeight == 2)) {
            if ((oldHeight == 1)) {
                SetDestructableAnimation(d, "death")
                QueueDestructableAnimation(d, "stand second")
            }
            else {
                if ((oldHeight == 3)) {
                    SetDestructableAnimation(d, "birth second")
                    QueueDestructableAnimation(d, "stand second")
                }
                else {
                    SetDestructableAnimation(d, "stand second")
                }
            }
        }
        else {
            if ((newHeight == 3)) {
                if ((oldHeight == 1)) {
                    SetDestructableAnimation(d, "death third")
                    QueueDestructableAnimation(d, "stand third")
                }
                else {
                    if ((oldHeight == 2)) {
                        SetDestructableAnimation(d, "death second")
                        QueueDestructableAnimation(d, "stand third")
                    }
                    else {
                        SetDestructableAnimation(d, "stand third")
                    }
                }
            }
            else {
            }
        }
    }
}
function NudgeUnitsInRectEnum() {
    var nudgee = GetEnumUnit();
    SetUnitPosition(nudgee, GetUnitX(nudgee), GetUnitY(nudgee))
}
function NudgeItemsInRectEnum() {
    var nudgee = GetEnumItem();
    SetItemPosition(nudgee, GetItemX(nudgee), GetItemY(nudgee))
}
function NudgeObjectsInRect(nudgeArea) {
    var g;
    g = CreateGroup()
    GroupEnumUnitsInRect(g, nudgeArea, null)
    ForGroup(g, NudgeUnitsInRectEnum)
    DestroyGroup(g)
    EnumItemsInRect(nudgeArea, null, NudgeItemsInRectEnum)
}
function NearbyElevatorExistsEnum() {
    var d = GetEnumDestructable();
    var dType = GetDestructableTypeId(d);
    if ((dType == bj_ELEVATOR_CODE01) || (dType == bj_ELEVATOR_CODE02)) {
        bj_elevatorNeighbor = d
    }
}
function NearbyElevatorExists(x, y) {
    var findThreshold = 32;
    var r;
    r = Rect(x - findThreshold, y - findThreshold, x + findThreshold, y + findThreshold)
    bj_elevatorNeighbor = null
    EnumDestructablesInRect(r, null, NearbyElevatorExistsEnum)
    RemoveRect(r)
    return bj_elevatorNeighbor != null
}
function FindElevatorWallBlockerEnum() {
    bj_elevatorWallBlocker = GetEnumDestructable()
}
function ChangeElevatorWallBlocker(x, y, facing, open) {
    var blocker = null;
    var findThreshold = 32;
    var nudgeLength = 4.25 * bj_CELLWIDTH;
    var nudgeWidth = 1.25 * bj_CELLWIDTH;
    var r;
    r = Rect(x - findThreshold, y - findThreshold, x + findThreshold, y + findThreshold)
    bj_elevatorWallBlocker = null
    EnumDestructablesInRect(r, null, FindElevatorWallBlockerEnum)
    RemoveRect(r)
    blocker = bj_elevatorWallBlocker
    if ((blocker == null)) {
        blocker = CreateDeadDestructable(bj_ELEVATOR_BLOCKER_CODE, x, y, facing, 1, 0)
    }
    else {
        if ((GetDestructableTypeId(blocker) != bj_ELEVATOR_BLOCKER_CODE)) {
            return
        }
    }
    if ((open)) {
        if ((GetDestructableLife(blocker) > 0)) {
            KillDestructable(blocker)
        }
    }
    else {
        if ((GetDestructableLife(blocker) <= 0)) {
            DestructableRestoreLife(blocker, GetDestructableMaxLife(blocker), false)
        }
        if ((facing == 0)) {
            r = Rect(x - nudgeWidth / 2, y - nudgeLength / 2, x + nudgeWidth / 2, y + nudgeLength / 2)
            NudgeObjectsInRect(r)
            RemoveRect(r)
        }
        else {
            if ((facing == 90)) {
                r = Rect(x - nudgeLength / 2, y - nudgeWidth / 2, x + nudgeLength / 2, y + nudgeWidth / 2)
                NudgeObjectsInRect(r)
                RemoveRect(r)
            }
            else {
            }
        }
    }
}
function ChangeElevatorWalls(open, walls, d) {
    var x = GetDestructableX(d);
    var y = GetDestructableY(d);
    var distToBlocker = 192;
    var distToNeighbor = 256;
    if ((walls == bj_ELEVATOR_WALL_TYPE_ALL) || (walls == bj_ELEVATOR_WALL_TYPE_EAST)) {
        if ((!NearbyElevatorExists(x + distToNeighbor, y))) {
            ChangeElevatorWallBlocker(x + distToBlocker, y, 0, open)
        }
    }
    if ((walls == bj_ELEVATOR_WALL_TYPE_ALL) || (walls == bj_ELEVATOR_WALL_TYPE_NORTH)) {
        if ((!NearbyElevatorExists(x, y + distToNeighbor))) {
            ChangeElevatorWallBlocker(x, y + distToBlocker, 90, open)
        }
    }
    if ((walls == bj_ELEVATOR_WALL_TYPE_ALL) || (walls == bj_ELEVATOR_WALL_TYPE_SOUTH)) {
        if ((!NearbyElevatorExists(x, y - distToNeighbor))) {
            ChangeElevatorWallBlocker(x, y - distToBlocker, 90, open)
        }
    }
    if ((walls == bj_ELEVATOR_WALL_TYPE_ALL) || (walls == bj_ELEVATOR_WALL_TYPE_WEST)) {
        if ((!NearbyElevatorExists(x - distToNeighbor, y))) {
            ChangeElevatorWallBlocker(x - distToBlocker, y, 0, open)
        }
    }
}
function WaygateActivateBJ(activate, waygate) {
    WaygateActivate(waygate, activate)
}
function WaygateIsActiveBJ(waygate) {
    return WaygateIsActive(waygate)
}
function WaygateSetDestinationLocBJ(waygate, loc) {
    WaygateSetDestination(waygate, GetLocationX(loc), GetLocationY(loc))
}
function WaygateGetDestinationLocBJ(waygate) {
    return Location(WaygateGetDestinationX(waygate), WaygateGetDestinationY(waygate))
}
function UnitSetUsesAltIconBJ(flag, whichUnit) {
    UnitSetUsesAltIcon(whichUnit, flag)
}
function ForceUIKeyBJ(whichPlayer, key) {
    if ((GetLocalPlayer() == whichPlayer)) {
        ForceUIKey(key)
    }
}
function ForceUICancelBJ(whichPlayer) {
    if ((GetLocalPlayer() == whichPlayer)) {
        ForceUICancel()
    }
}
function ForGroupBJ(whichGroup, callback) {
    var wantDestroy = bj_wantDestroyGroup;
    bj_wantDestroyGroup = false
    ForGroup(whichGroup, callback)
    if ((wantDestroy)) {
        DestroyGroup(whichGroup)
    }
}
function GroupAddUnitSimple(whichUnit, whichGroup) {
    GroupAddUnit(whichGroup, whichUnit)
}
function GroupRemoveUnitSimple(whichUnit, whichGroup) {
    GroupRemoveUnit(whichGroup, whichUnit)
}
function GroupAddGroupEnum() {
    GroupAddUnit(bj_groupAddGroupDest, GetEnumUnit())
}
function GroupAddGroup(sourceGroup, destGroup) {
    var wantDestroy = bj_wantDestroyGroup;
    bj_wantDestroyGroup = false
    bj_groupAddGroupDest = destGroup
    ForGroup(sourceGroup, GroupAddGroupEnum)
    if ((wantDestroy)) {
        DestroyGroup(sourceGroup)
    }
}
function GroupRemoveGroupEnum() {
    GroupRemoveUnit(bj_groupRemoveGroupDest, GetEnumUnit())
}
function GroupRemoveGroup(sourceGroup, destGroup) {
    var wantDestroy = bj_wantDestroyGroup;
    bj_wantDestroyGroup = false
    bj_groupRemoveGroupDest = destGroup
    ForGroup(sourceGroup, GroupRemoveGroupEnum)
    if ((wantDestroy)) {
        DestroyGroup(sourceGroup)
    }
}
function ForceAddPlayerSimple(whichPlayer, whichForce) {
    ForceAddPlayer(whichForce, whichPlayer)
}
function ForceRemovePlayerSimple(whichPlayer, whichForce) {
    ForceRemovePlayer(whichForce, whichPlayer)
}
function GroupPickRandomUnitEnum() {
    bj_groupRandomConsidered = bj_groupRandomConsidered + 1
    if ((GetRandomInt(1, bj_groupRandomConsidered) == 1)) {
        bj_groupRandomCurrentPick = GetEnumUnit()
    }
}
function GroupPickRandomUnit(whichGroup) {
    var wantDestroy = bj_wantDestroyGroup;
    bj_wantDestroyGroup = false
    bj_groupRandomConsidered = 0
    bj_groupRandomCurrentPick = null
    ForGroup(whichGroup, GroupPickRandomUnitEnum)
    if ((wantDestroy)) {
        DestroyGroup(whichGroup)
    }
    return bj_groupRandomCurrentPick
}
function ForcePickRandomPlayerEnum() {
    bj_forceRandomConsidered = bj_forceRandomConsidered + 1
    if ((GetRandomInt(1, bj_forceRandomConsidered) == 1)) {
        bj_forceRandomCurrentPick = GetEnumPlayer()
    }
}
function ForcePickRandomPlayer(whichForce) {
    bj_forceRandomConsidered = 0
    bj_forceRandomCurrentPick = null
    ForForce(whichForce, ForcePickRandomPlayerEnum)
    return bj_forceRandomCurrentPick
}
function EnumUnitsSelected(whichPlayer, enumFilter, enumAction) {
    var g = CreateGroup();
    SyncSelections()
    GroupEnumUnitsSelected(g, whichPlayer, enumFilter)
    DestroyBoolExpr(enumFilter)
    ForGroup(g, enumAction)
    DestroyGroup(g)
}
function GetUnitsInRectMatching(r, filter) {
    var g = CreateGroup();
    GroupEnumUnitsInRect(g, r, filter)
    DestroyBoolExpr(filter)
    return g
}
function GetUnitsInRectAll(r) {
    return GetUnitsInRectMatching(r, null)
}
function GetUnitsInRectOfPlayerFilter() {
    return GetOwningPlayer(GetFilterUnit()) == bj_groupEnumOwningPlayer
}
function GetUnitsInRectOfPlayer(r, whichPlayer) {
    var g = CreateGroup();
    bj_groupEnumOwningPlayer = whichPlayer
    GroupEnumUnitsInRect(g, r, filterGetUnitsInRectOfPlayer)
    return g
}
function GetUnitsInRangeOfLocMatching(radius, whichLocation, filter) {
    var g = CreateGroup();
    GroupEnumUnitsInRangeOfLoc(g, whichLocation, radius, filter)
    DestroyBoolExpr(filter)
    return g
}
function GetUnitsInRangeOfLocAll(radius, whichLocation) {
    return GetUnitsInRangeOfLocMatching(radius, whichLocation, null)
}
function GetUnitsOfTypeIdAllFilter() {
    return GetUnitTypeId(GetFilterUnit()) == bj_groupEnumTypeId
}
function GetUnitsOfTypeIdAll(unitid) {
    var result = CreateGroup();
    var g = CreateGroup();
    var index;
    index = 0
    do {
        bj_groupEnumTypeId = unitid
        GroupClear(g)
        GroupEnumUnitsOfPlayer(g, Player(index), filterGetUnitsOfTypeIdAll)
        GroupAddGroup(g, result)
        index = index + 1
        if (index == bj_MAX_PLAYER_SLOTS) { break; }
    } while (true)
    DestroyGroup(g)
    return result
}
function GetUnitsOfPlayerMatching(whichPlayer, filter) {
    var g = CreateGroup();
    GroupEnumUnitsOfPlayer(g, whichPlayer, filter)
    DestroyBoolExpr(filter)
    return g
}
function GetUnitsOfPlayerAll(whichPlayer) {
    return GetUnitsOfPlayerMatching(whichPlayer, null)
}
function GetUnitsOfPlayerAndTypeIdFilter() {
    return GetUnitTypeId(GetFilterUnit()) == bj_groupEnumTypeId
}
function GetUnitsOfPlayerAndTypeId(whichPlayer, unitid) {
    var g = CreateGroup();
    bj_groupEnumTypeId = unitid
    GroupEnumUnitsOfPlayer(g, whichPlayer, filterGetUnitsOfPlayerAndTypeId)
    return g
}
function GetUnitsSelectedAll(whichPlayer) {
    var g = CreateGroup();
    SyncSelections()
    GroupEnumUnitsSelected(g, whichPlayer, null)
    return g
}
function GetForceOfPlayer(whichPlayer) {
    var f = CreateForce();
    ForceAddPlayer(f, whichPlayer)
    return f
}
function GetPlayersAll() {
    return bj_FORCE_ALL_PLAYERS
}
function GetPlayersByMapControl(whichControl) {
    var f = CreateForce();
    var playerIndex;
    var indexPlayer;
    playerIndex = 0
    do {
        indexPlayer = Player(playerIndex)
        if (GetPlayerController(indexPlayer) == whichControl) {
            ForceAddPlayer(f, indexPlayer)
        }
        playerIndex = playerIndex + 1
        if (playerIndex == bj_MAX_PLAYER_SLOTS) { break; }
    } while (true)
    return f
}
function GetPlayersAllies(whichPlayer) {
    var f = CreateForce();
    ForceEnumAllies(f, whichPlayer, null)
    return f
}
function GetPlayersEnemies(whichPlayer) {
    var f = CreateForce();
    ForceEnumEnemies(f, whichPlayer, null)
    return f
}
function GetPlayersMatching(filter) {
    var f = CreateForce();
    ForceEnumPlayers(f, filter)
    DestroyBoolExpr(filter)
    return f
}
function CountUnitsInGroupEnum() {
    bj_groupCountUnits = bj_groupCountUnits + 1
}
function CountUnitsInGroup(g) {
    var wantDestroy = bj_wantDestroyGroup;
    bj_wantDestroyGroup = false
    bj_groupCountUnits = 0
    ForGroup(g, CountUnitsInGroupEnum)
    if ((wantDestroy)) {
        DestroyGroup(g)
    }
    return bj_groupCountUnits
}
function CountPlayersInForceEnum() {
    bj_forceCountPlayers = bj_forceCountPlayers + 1
}
function CountPlayersInForceBJ(f) {
    bj_forceCountPlayers = 0
    ForForce(f, CountPlayersInForceEnum)
    return bj_forceCountPlayers
}
function GetRandomSubGroupEnum() {
    if ((bj_randomSubGroupWant > 0)) {
        if ((bj_randomSubGroupWant >= bj_randomSubGroupTotal) || (GetRandomReal(0, 1) < bj_randomSubGroupChance)) {
            GroupAddUnit(bj_randomSubGroupGroup, GetEnumUnit())
            bj_randomSubGroupWant = bj_randomSubGroupWant - 1
        }
    }
    bj_randomSubGroupTotal = bj_randomSubGroupTotal - 1
}
function GetRandomSubGroup(count, sourceGroup) {
    var g = CreateGroup();
    bj_randomSubGroupGroup = g
    bj_randomSubGroupWant = count
    bj_randomSubGroupTotal = CountUnitsInGroup(sourceGroup)
    if ((bj_randomSubGroupWant <= 0 || bj_randomSubGroupTotal <= 0)) {
        return g
    }
    bj_randomSubGroupChance = I2R(bj_randomSubGroupWant) / I2R(bj_randomSubGroupTotal)
    ForGroup(sourceGroup, GetRandomSubGroupEnum)
    return g
}
function LivingPlayerUnitsOfTypeIdFilter() {
    var filterUnit = GetFilterUnit();
    return IsUnitAliveBJ(filterUnit) && GetUnitTypeId(filterUnit) == bj_livingPlayerUnitsTypeId
}
function CountLivingPlayerUnitsOfTypeId(unitId, whichPlayer) {
    var g;
    var matchedCount;
    g = CreateGroup()
    bj_livingPlayerUnitsTypeId = unitId
    GroupEnumUnitsOfPlayer(g, whichPlayer, filterLivingPlayerUnitsOfTypeId)
    matchedCount = CountUnitsInGroup(g)
    DestroyGroup(g)
    return matchedCount
}
function ResetUnitAnimation(whichUnit) {
    SetUnitAnimation(whichUnit, "stand")
}
function SetUnitTimeScalePercent(whichUnit, percentScale) {
    SetUnitTimeScale(whichUnit, percentScale * 0.01)
}
function SetUnitScalePercent(whichUnit, percentScaleX, percentScaleY, percentScaleZ) {
    SetUnitScale(whichUnit, percentScaleX * 0.01, percentScaleY * 0.01, percentScaleZ * 0.01)
}
function SetUnitVertexColorBJ(whichUnit, red, green, blue, transparency) {
    SetUnitVertexColor(whichUnit, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - transparency))
}
function UnitAddIndicatorBJ(whichUnit, red, green, blue, transparency) {
    AddIndicator(whichUnit, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - transparency))
}
function DestructableAddIndicatorBJ(whichDestructable, red, green, blue, transparency) {
    AddIndicator(whichDestructable, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - transparency))
}
function ItemAddIndicatorBJ(whichItem, red, green, blue, transparency) {
    AddIndicator(whichItem, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - transparency))
}
function SetUnitFacingToFaceLocTimed(whichUnit, target, duration) {
    var unitLoc = GetUnitLoc(whichUnit);
    SetUnitFacingTimed(whichUnit, AngleBetweenPoints(unitLoc, target), duration)
    RemoveLocation(unitLoc)
}
function SetUnitFacingToFaceUnitTimed(whichUnit, target, duration) {
    var unitLoc = GetUnitLoc(target);
    SetUnitFacingToFaceLocTimed(whichUnit, unitLoc, duration)
    RemoveLocation(unitLoc)
}
function QueueUnitAnimationBJ(whichUnit, whichAnimation) {
    QueueUnitAnimation(whichUnit, whichAnimation)
}
function SetDestructableAnimationBJ(d, whichAnimation) {
    SetDestructableAnimation(d, whichAnimation)
}
function QueueDestructableAnimationBJ(d, whichAnimation) {
    QueueDestructableAnimation(d, whichAnimation)
}
function SetDestAnimationSpeedPercent(d, percentScale) {
    SetDestructableAnimationSpeed(d, percentScale * 0.01)
}
function DialogDisplayBJ(flag, whichDialog, whichPlayer) {
    DialogDisplay(whichPlayer, whichDialog, flag)
}
function DialogSetMessageBJ(whichDialog, message) {
    DialogSetMessage(whichDialog, message)
}
function DialogAddButtonBJ(whichDialog, buttonText) {
    bj_lastCreatedButton = DialogAddButton(whichDialog, buttonText, 0)
    return bj_lastCreatedButton
}
function DialogAddButtonWithHotkeyBJ(whichDialog, buttonText, hotkey) {
    bj_lastCreatedButton = DialogAddButton(whichDialog, buttonText, hotkey)
    return bj_lastCreatedButton
}
function DialogClearBJ(whichDialog) {
    DialogClear(whichDialog)
}
function GetLastCreatedButtonBJ() {
    return bj_lastCreatedButton
}
function GetClickedButtonBJ() {
    return GetClickedButton()
}
function GetClickedDialogBJ() {
    return GetClickedDialog()
}
function SetPlayerAllianceBJ(sourcePlayer, whichAllianceSetting, value, otherPlayer) {
    if ((sourcePlayer == otherPlayer)) {
        return
    }
    SetPlayerAlliance(sourcePlayer, otherPlayer, whichAllianceSetting, value)
}
function SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, flag) {
    SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_PASSIVE, flag)
    SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_HELP_REQUEST, flag)
    SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_HELP_RESPONSE, flag)
    SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_SHARED_XP, flag)
    SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_SHARED_SPELLS, flag)
}
function SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, flag) {
    SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_SHARED_VISION, flag)
}
function SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, flag) {
    SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_SHARED_CONTROL, flag)
}
function SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, flag) {
    SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_SHARED_ADVANCED_CONTROL, flag)
}
function SetPlayerAllianceStateBJ(sourcePlayer, otherPlayer, allianceState) {
    if ((sourcePlayer == otherPlayer)) {
        return
    }
    if (allianceState == bj_ALLIANCE_UNALLIED) {
        SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, false)
        SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, false)
        SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, false)
        SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, false)
    }
    else {
        if (allianceState == bj_ALLIANCE_UNALLIED_VISION) {
            SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, false)
            SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, true)
            SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, false)
            SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, false)
        }
        else {
            if (allianceState == bj_ALLIANCE_ALLIED) {
                SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, true)
                SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, false)
                SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, false)
                SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, false)
            }
            else {
                if (allianceState == bj_ALLIANCE_ALLIED_VISION) {
                    SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, true)
                    SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, true)
                    SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, false)
                    SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, false)
                }
                else {
                    if (allianceState == bj_ALLIANCE_ALLIED_UNITS) {
                        SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, true)
                        SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, true)
                        SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, true)
                        SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, false)
                    }
                    else {
                        if (allianceState == bj_ALLIANCE_ALLIED_ADVUNITS) {
                            SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, true)
                            SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, true)
                            SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, true)
                            SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, true)
                        }
                        else {
                            if (allianceState == bj_ALLIANCE_NEUTRAL) {
                                SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, false)
                                SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, false)
                                SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, false)
                                SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, false)
                                SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_PASSIVE, true)
                            }
                            else {
                                if (allianceState == bj_ALLIANCE_NEUTRAL_VISION) {
                                    SetPlayerAllianceStateAllyBJ(sourcePlayer, otherPlayer, false)
                                    SetPlayerAllianceStateVisionBJ(sourcePlayer, otherPlayer, true)
                                    SetPlayerAllianceStateControlBJ(sourcePlayer, otherPlayer, false)
                                    SetPlayerAllianceStateFullControlBJ(sourcePlayer, otherPlayer, false)
                                    SetPlayerAlliance(sourcePlayer, otherPlayer, ALLIANCE_PASSIVE, true)
                                }
                                else {
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
function SetForceAllianceStateBJ(sourceForce, targetForce, allianceState) {
    var sourceIndex;
    var targetIndex;
    sourceIndex = 0
    do {
        if ((sourceForce == bj_FORCE_ALL_PLAYERS || IsPlayerInForce(Player(sourceIndex), sourceForce))) {
            targetIndex = 0
            do {
                if ((targetForce == bj_FORCE_ALL_PLAYERS || IsPlayerInForce(Player(targetIndex), targetForce))) {
                    SetPlayerAllianceStateBJ(Player(sourceIndex), Player(targetIndex), allianceState)
                }
                targetIndex = targetIndex + 1
                if (targetIndex == bj_MAX_PLAYER_SLOTS) { break; }
            } while (true)
        }
        sourceIndex = sourceIndex + 1
        if (sourceIndex == bj_MAX_PLAYER_SLOTS) { break; }
    } while (true)
}
function PlayersAreCoAllied(playerA, playerB) {
    if ((playerA == playerB)) {
        return true
    }
    if (GetPlayerAlliance(playerA, playerB, ALLIANCE_PASSIVE)) {
        if (GetPlayerAlliance(playerB, playerA, ALLIANCE_PASSIVE)) {
            return true
        }
    }
    return false
}
function ShareEverythingWithTeamAI(whichPlayer) {
    var playerIndex;
    var indexPlayer;
    playerIndex = 0
    do {
        indexPlayer = Player(playerIndex)
        if ((PlayersAreCoAllied(whichPlayer, indexPlayer) && whichPlayer != indexPlayer)) {
            if ((GetPlayerController(indexPlayer) == MAP_CONTROL_COMPUTER)) {
                SetPlayerAlliance(whichPlayer, indexPlayer, ALLIANCE_SHARED_VISION, true)
                SetPlayerAlliance(whichPlayer, indexPlayer, ALLIANCE_SHARED_CONTROL, true)
                SetPlayerAlliance(whichPlayer, indexPlayer, ALLIANCE_SHARED_ADVANCED_CONTROL, true)
            }
        }
        playerIndex = playerIndex + 1
        if (playerIndex == bj_MAX_PLAYERS) { break; }
    } while (true)
}
function ShareEverythingWithTeam(whichPlayer) {
    var playerIndex;
    var indexPlayer;
    playerIndex = 0
    do {
        indexPlayer = Player(playerIndex)
        if ((PlayersAreCoAllied(whichPlayer, indexPlayer) && whichPlayer != indexPlayer)) {
            SetPlayerAlliance(whichPlayer, indexPlayer, ALLIANCE_SHARED_VISION, true)
            SetPlayerAlliance(whichPlayer, indexPlayer, ALLIANCE_SHARED_CONTROL, true)
            SetPlayerAlliance(indexPlayer, whichPlayer, ALLIANCE_SHARED_CONTROL, true)
            SetPlayerAlliance(whichPlayer, indexPlayer, ALLIANCE_SHARED_ADVANCED_CONTROL, true)
        }
        playerIndex = playerIndex + 1
        if (playerIndex == bj_MAX_PLAYERS) { break; }
    } while (true)
}
function ConfigureNeutralVictim() {
    var index;
    var indexPlayer;
    var neutralVictim = Player(bj_PLAYER_NEUTRAL_VICTIM);
    index = 0
    do {
        indexPlayer = Player(index)
        SetPlayerAlliance(neutralVictim, indexPlayer, ALLIANCE_PASSIVE, true)
        SetPlayerAlliance(indexPlayer, neutralVictim, ALLIANCE_PASSIVE, false)
        index = index + 1
        if (index == bj_MAX_PLAYERS) { break; }
    } while (true)
    indexPlayer = Player(PLAYER_NEUTRAL_AGGRESSIVE)
    SetPlayerAlliance(neutralVictim, indexPlayer, ALLIANCE_PASSIVE, true)
    SetPlayerAlliance(indexPlayer, neutralVictim, ALLIANCE_PASSIVE, true)
    SetPlayerState(neutralVictim, PLAYER_STATE_GIVES_BOUNTY, 0)
}
function MakeUnitsPassiveForPlayerEnum() {
    SetUnitOwner(GetEnumUnit(), Player(bj_PLAYER_NEUTRAL_VICTIM), false)
}
function MakeUnitsPassiveForPlayer(whichPlayer) {
    var playerUnits = CreateGroup();
    CachePlayerHeroData(whichPlayer)
    GroupEnumUnitsOfPlayer(playerUnits, whichPlayer, null)
    ForGroup(playerUnits, MakeUnitsPassiveForPlayerEnum)
    DestroyGroup(playerUnits)
}
function MakeUnitsPassiveForTeam(whichPlayer) {
    var playerIndex;
    var indexPlayer;
    playerIndex = 0
    do {
        indexPlayer = Player(playerIndex)
        if (PlayersAreCoAllied(whichPlayer, indexPlayer)) {
            MakeUnitsPassiveForPlayer(indexPlayer)
        }
        playerIndex = playerIndex + 1
        if (playerIndex == bj_MAX_PLAYERS) { break; }
    } while (true)
}
function AllowVictoryDefeat(gameResult) {
    if ((gameResult == PLAYER_GAME_RESULT_VICTORY)) {
        return !IsNoVictoryCheat()
    }
    if ((gameResult == PLAYER_GAME_RESULT_DEFEAT)) {
        return !IsNoDefeatCheat()
    }
    if ((gameResult == PLAYER_GAME_RESULT_NEUTRAL)) {
        return (!IsNoVictoryCheat()) && (!IsNoDefeatCheat())
    }
    return true
}
function EndGameBJ() {
    EndGame(true)
}
function MeleeVictoryDialogBJ(whichPlayer, leftGame) {
    var t = CreateTrigger();
    var d = DialogCreate();
    var formatString;
    if ((leftGame)) {
        formatString = GetLocalizedString("PLAYER_LEFT_GAME")
    }
    else {
        formatString = GetLocalizedString("PLAYER_VICTORIOUS")
    }
    DisplayTimedTextFromPlayer(whichPlayer, 0, 0, 60, formatString)
    DialogSetMessage(d, GetLocalizedString("GAMEOVER_VICTORY_MSG"))
    DialogAddButton(d, GetLocalizedString("GAMEOVER_CONTINUE_GAME"), GetLocalizedHotkey("GAMEOVER_CONTINUE_GAME"))
    t = CreateTrigger()
    TriggerRegisterDialogButtonEvent(t, DialogAddQuitButton(d, true, GetLocalizedString("GAMEOVER_QUIT_GAME"), GetLocalizedHotkey("GAMEOVER_QUIT_GAME")))
    DialogDisplay(whichPlayer, d, true)
    StartSoundForPlayerBJ(whichPlayer, bj_victoryDialogSound)
}
function MeleeDefeatDialogBJ(whichPlayer, leftGame) {
    var t = CreateTrigger();
    var d = DialogCreate();
    var formatString;
    if ((leftGame)) {
        formatString = GetLocalizedString("PLAYER_LEFT_GAME")
    }
    else {
        formatString = GetLocalizedString("PLAYER_DEFEATED")
    }
    DisplayTimedTextFromPlayer(whichPlayer, 0, 0, 60, formatString)
    DialogSetMessage(d, GetLocalizedString("GAMEOVER_DEFEAT_MSG"))
    if ((!bj_meleeGameOver && IsMapFlagSet(MAP_OBSERVERS_ON_DEATH))) {
        DialogAddButton(d, GetLocalizedString("GAMEOVER_CONTINUE_OBSERVING"), GetLocalizedHotkey("GAMEOVER_CONTINUE_OBSERVING"))
    }
    t = CreateTrigger()
    TriggerRegisterDialogButtonEvent(t, DialogAddQuitButton(d, true, GetLocalizedString("GAMEOVER_QUIT_GAME"), GetLocalizedHotkey("GAMEOVER_QUIT_GAME")))
    DialogDisplay(whichPlayer, d, true)
    StartSoundForPlayerBJ(whichPlayer, bj_defeatDialogSound)
}
function GameOverDialogBJ(whichPlayer, leftGame) {
    var t = CreateTrigger();
    var d = DialogCreate();
    var s;
    DisplayTimedTextFromPlayer(whichPlayer, 0, 0, 60, GetLocalizedString("PLAYER_LEFT_GAME"))
    if ((GetIntegerGameState(GAME_STATE_DISCONNECTED) != 0)) {
        s = GetLocalizedString("GAMEOVER_DISCONNECTED")
    }
    else {
        s = GetLocalizedString("GAMEOVER_GAME_OVER")
    }
    DialogSetMessage(d, s)
    t = CreateTrigger()
    TriggerRegisterDialogButtonEvent(t, DialogAddQuitButton(d, true, GetLocalizedString("GAMEOVER_OK"), GetLocalizedHotkey("GAMEOVER_OK")))
    DialogDisplay(whichPlayer, d, true)
    StartSoundForPlayerBJ(whichPlayer, bj_defeatDialogSound)
}
function RemovePlayerPreserveUnitsBJ(whichPlayer, gameResult, leftGame) {
    if (AllowVictoryDefeat(gameResult)) {
        RemovePlayer(whichPlayer, gameResult)
        if ((gameResult == PLAYER_GAME_RESULT_VICTORY)) {
            MeleeVictoryDialogBJ(whichPlayer, leftGame)
            return
        }
        else {
            if ((gameResult == PLAYER_GAME_RESULT_DEFEAT)) {
                MeleeDefeatDialogBJ(whichPlayer, leftGame)
            }
            else {
                GameOverDialogBJ(whichPlayer, leftGame)
            }
        }
    }
}
function CustomVictoryOkBJ() {
    if (bj_isSinglePlayer) {
        PauseGame(false)
        SetGameDifficulty(GetDefaultDifficulty())
    }
    if ((bj_changeLevelMapName == null)) {
        EndGame(bj_changeLevelShowScores)
    }
    else {
        ChangeLevel(bj_changeLevelMapName, bj_changeLevelShowScores)
    }
}
function CustomVictoryQuitBJ() {
    if (bj_isSinglePlayer) {
        PauseGame(false)
        SetGameDifficulty(GetDefaultDifficulty())
    }
    EndGame(bj_changeLevelShowScores)
}
function CustomVictoryDialogBJ(whichPlayer) {
    var t = CreateTrigger();
    var d = DialogCreate();
    DialogSetMessage(d, GetLocalizedString("GAMEOVER_VICTORY_MSG"))
    t = CreateTrigger()
    TriggerRegisterDialogButtonEvent(t, DialogAddButton(d, GetLocalizedString("GAMEOVER_CONTINUE"), GetLocalizedHotkey("GAMEOVER_CONTINUE")))
    TriggerAddAction(t, CustomVictoryOkBJ)
    t = CreateTrigger()
    TriggerRegisterDialogButtonEvent(t, DialogAddButton(d, GetLocalizedString("GAMEOVER_QUIT_MISSION"), GetLocalizedHotkey("GAMEOVER_QUIT_MISSION")))
    TriggerAddAction(t, CustomVictoryQuitBJ)
    if ((GetLocalPlayer() == whichPlayer)) {
        EnableUserControl(true)
        if (bj_isSinglePlayer) {
            PauseGame(true)
        }
        EnableUserUI(false)
    }
    DialogDisplay(whichPlayer, d, true)
    VolumeGroupSetVolumeForPlayerBJ(whichPlayer, SOUND_VOLUMEGROUP_UI, 1.0)
    StartSoundForPlayerBJ(whichPlayer, bj_victoryDialogSound)
}
function CustomVictorySkipBJ(whichPlayer) {
    if ((GetLocalPlayer() == whichPlayer)) {
        if (bj_isSinglePlayer) {
            SetGameDifficulty(GetDefaultDifficulty())
        }
        if ((bj_changeLevelMapName == null)) {
            EndGame(bj_changeLevelShowScores)
        }
        else {
            ChangeLevel(bj_changeLevelMapName, bj_changeLevelShowScores)
        }
    }
}
function CustomVictoryBJ(whichPlayer, showDialog, showScores) {
    if (AllowVictoryDefeat(PLAYER_GAME_RESULT_VICTORY)) {
        RemovePlayer(whichPlayer, PLAYER_GAME_RESULT_VICTORY)
        if (!bj_isSinglePlayer) {
            DisplayTimedTextFromPlayer(whichPlayer, 0, 0, 60, GetLocalizedString("PLAYER_VICTORIOUS"))
        }
        if ((GetPlayerController(whichPlayer) == MAP_CONTROL_USER)) {
            bj_changeLevelShowScores = showScores
            if (showDialog) {
                CustomVictoryDialogBJ(whichPlayer)
            }
            else {
                CustomVictorySkipBJ(whichPlayer)
            }
        }
    }
}
function CustomDefeatRestartBJ() {
    PauseGame(false)
    RestartGame(true)
}
function CustomDefeatReduceDifficultyBJ() {
    var diff = GetGameDifficulty();
    PauseGame(false)
    if ((diff == MAP_DIFFICULTY_EASY)) {
    }
    else {
        if ((diff == MAP_DIFFICULTY_NORMAL)) {
            SetGameDifficulty(MAP_DIFFICULTY_EASY)
        }
        else {
            if ((diff == MAP_DIFFICULTY_HARD)) {
                SetGameDifficulty(MAP_DIFFICULTY_NORMAL)
            }
            else {
            }
        }
    }
    RestartGame(true)
}
function CustomDefeatLoadBJ() {
    PauseGame(false)
    DisplayLoadDialog()
}
function CustomDefeatQuitBJ() {
    if (bj_isSinglePlayer) {
        PauseGame(false)
    }
    SetGameDifficulty(GetDefaultDifficulty())
    EndGame(true)
}
function CustomDefeatDialogBJ(whichPlayer, message) {
    var t = CreateTrigger();
    var d = DialogCreate();
    DialogSetMessage(d, message)
    if (bj_isSinglePlayer) {
        t = CreateTrigger()
        TriggerRegisterDialogButtonEvent(t, DialogAddButton(d, GetLocalizedString("GAMEOVER_RESTART"), GetLocalizedHotkey("GAMEOVER_RESTART")))
        TriggerAddAction(t, CustomDefeatRestartBJ)
        if ((GetGameDifficulty() != MAP_DIFFICULTY_EASY)) {
            t = CreateTrigger()
            TriggerRegisterDialogButtonEvent(t, DialogAddButton(d, GetLocalizedString("GAMEOVER_REDUCE_DIFFICULTY"), GetLocalizedHotkey("GAMEOVER_REDUCE_DIFFICULTY")))
            TriggerAddAction(t, CustomDefeatReduceDifficultyBJ)
        }
        t = CreateTrigger()
        TriggerRegisterDialogButtonEvent(t, DialogAddButton(d, GetLocalizedString("GAMEOVER_LOAD"), GetLocalizedHotkey("GAMEOVER_LOAD")))
        TriggerAddAction(t, CustomDefeatLoadBJ)
    }
    t = CreateTrigger()
    TriggerRegisterDialogButtonEvent(t, DialogAddButton(d, GetLocalizedString("GAMEOVER_QUIT_MISSION"), GetLocalizedHotkey("GAMEOVER_QUIT_MISSION")))
    TriggerAddAction(t, CustomDefeatQuitBJ)
    if ((GetLocalPlayer() == whichPlayer)) {
        EnableUserControl(true)
        if (bj_isSinglePlayer) {
            PauseGame(true)
        }
        EnableUserUI(false)
    }
    DialogDisplay(whichPlayer, d, true)
    VolumeGroupSetVolumeForPlayerBJ(whichPlayer, SOUND_VOLUMEGROUP_UI, 1.0)
    StartSoundForPlayerBJ(whichPlayer, bj_defeatDialogSound)
}
function CustomDefeatBJ(whichPlayer, message) {
    if (AllowVictoryDefeat(PLAYER_GAME_RESULT_DEFEAT)) {
        RemovePlayer(whichPlayer, PLAYER_GAME_RESULT_DEFEAT)
        if (!bj_isSinglePlayer) {
            DisplayTimedTextFromPlayer(whichPlayer, 0, 0, 60, GetLocalizedString("PLAYER_DEFEATED"))
        }
        if ((GetPlayerController(whichPlayer) == MAP_CONTROL_USER)) {
            CustomDefeatDialogBJ(whichPlayer, message)
        }
    }
}
function SetNextLevelBJ(nextLevel) {
    if ((nextLevel == "")) {
        bj_changeLevelMapName = null
    }
    else {
        bj_changeLevelMapName = nextLevel
    }
}
function SetPlayerOnScoreScreenBJ(flag, whichPlayer) {
    SetPlayerOnScoreScreen(whichPlayer, flag)
}
function CreateQuestBJ(questType, title, description, iconPath) {
    var required = (questType == bj_QUESTTYPE_REQ_DISCOVERED) || (questType == bj_QUESTTYPE_REQ_UNDISCOVERED);
    var discovered = (questType == bj_QUESTTYPE_REQ_DISCOVERED) || (questType == bj_QUESTTYPE_OPT_DISCOVERED);
    bj_lastCreatedQuest = CreateQuest()
    QuestSetTitle(bj_lastCreatedQuest, title)
    QuestSetDescription(bj_lastCreatedQuest, description)
    QuestSetIconPath(bj_lastCreatedQuest, iconPath)
    QuestSetRequired(bj_lastCreatedQuest, required)
    QuestSetDiscovered(bj_lastCreatedQuest, discovered)
    QuestSetCompleted(bj_lastCreatedQuest, false)
    return bj_lastCreatedQuest
}
function DestroyQuestBJ(whichQuest) {
    DestroyQuest(whichQuest)
}
function QuestSetEnabledBJ(enabled, whichQuest) {
    QuestSetEnabled(whichQuest, enabled)
}
function QuestSetTitleBJ(whichQuest, title) {
    QuestSetTitle(whichQuest, title)
}
function QuestSetDescriptionBJ(whichQuest, description) {
    QuestSetDescription(whichQuest, description)
}
function QuestSetCompletedBJ(whichQuest, completed) {
    QuestSetCompleted(whichQuest, completed)
}
function QuestSetFailedBJ(whichQuest, failed) {
    QuestSetFailed(whichQuest, failed)
}
function QuestSetDiscoveredBJ(whichQuest, discovered) {
    QuestSetDiscovered(whichQuest, discovered)
}
function GetLastCreatedQuestBJ() {
    return bj_lastCreatedQuest
}
function CreateQuestItemBJ(whichQuest, description) {
    bj_lastCreatedQuestItem = QuestCreateItem(whichQuest)
    QuestItemSetDescription(bj_lastCreatedQuestItem, description)
    QuestItemSetCompleted(bj_lastCreatedQuestItem, false)
    return bj_lastCreatedQuestItem
}
function QuestItemSetDescriptionBJ(whichQuestItem, description) {
    QuestItemSetDescription(whichQuestItem, description)
}
function QuestItemSetCompletedBJ(whichQuestItem, completed) {
    QuestItemSetCompleted(whichQuestItem, completed)
}
function GetLastCreatedQuestItemBJ() {
    return bj_lastCreatedQuestItem
}
function CreateDefeatConditionBJ(description) {
    bj_lastCreatedDefeatCondition = CreateDefeatCondition()
    DefeatConditionSetDescription(bj_lastCreatedDefeatCondition, description)
    return bj_lastCreatedDefeatCondition
}
function DestroyDefeatConditionBJ(whichCondition) {
    DestroyDefeatCondition(whichCondition)
}
function DefeatConditionSetDescriptionBJ(whichCondition, description) {
    DefeatConditionSetDescription(whichCondition, description)
}
function GetLastCreatedDefeatConditionBJ() {
    return bj_lastCreatedDefeatCondition
}
function FlashQuestDialogButtonBJ() {
    FlashQuestDialogButton()
}
function QuestMessageBJ(f, messageType, message) {
    if ((IsPlayerInForce(GetLocalPlayer(), f))) {
        if ((messageType == bj_QUESTMESSAGE_DISCOVERED)) {
            DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUEST, " ")
            DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUEST, message)
            StartSound(bj_questDiscoveredSound)
            FlashQuestDialogButton()
        }
        else {
            if ((messageType == bj_QUESTMESSAGE_UPDATED)) {
                DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUESTUPDATE, " ")
                DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUESTUPDATE, message)
                StartSound(bj_questUpdatedSound)
                FlashQuestDialogButton()
            }
            else {
                if ((messageType == bj_QUESTMESSAGE_COMPLETED)) {
                    DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUESTDONE, " ")
                    DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUESTDONE, message)
                    StartSound(bj_questCompletedSound)
                    FlashQuestDialogButton()
                }
                else {
                    if ((messageType == bj_QUESTMESSAGE_FAILED)) {
                        DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUESTFAILED, " ")
                        DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUESTFAILED, message)
                        StartSound(bj_questFailedSound)
                        FlashQuestDialogButton()
                    }
                    else {
                        if ((messageType == bj_QUESTMESSAGE_REQUIREMENT)) {
                            DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_QUESTREQUIREMENT, message)
                        }
                        else {
                            if ((messageType == bj_QUESTMESSAGE_MISSIONFAILED)) {
                                DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_MISSIONFAILED, " ")
                                DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_MISSIONFAILED, message)
                                StartSound(bj_questFailedSound)
                            }
                            else {
                                if ((messageType == bj_QUESTMESSAGE_HINT)) {
                                    DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_HINT, " ")
                                    DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_HINT, message)
                                    StartSound(bj_questHintSound)
                                }
                                else {
                                    if ((messageType == bj_QUESTMESSAGE_ALWAYSHINT)) {
                                        DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_ALWAYSHINT, " ")
                                        DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_ALWAYSHINT, message)
                                        StartSound(bj_questHintSound)
                                    }
                                    else {
                                        if ((messageType == bj_QUESTMESSAGE_SECRET)) {
                                            DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_SECRET, " ")
                                            DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_SECRET, message)
                                            StartSound(bj_questSecretSound)
                                        }
                                        else {
                                            if ((messageType == bj_QUESTMESSAGE_UNITACQUIRED)) {
                                                DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_UNITACQUIRED, " ")
                                                DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_UNITACQUIRED, message)
                                                StartSound(bj_questHintSound)
                                            }
                                            else {
                                                if ((messageType == bj_QUESTMESSAGE_UNITAVAILABLE)) {
                                                    DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_UNITAVAILABLE, " ")
                                                    DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_UNITAVAILABLE, message)
                                                    StartSound(bj_questHintSound)
                                                }
                                                else {
                                                    if ((messageType == bj_QUESTMESSAGE_ITEMACQUIRED)) {
                                                        DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_ITEMACQUIRED, " ")
                                                        DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_ITEMACQUIRED, message)
                                                        StartSound(bj_questItemAcquiredSound)
                                                    }
                                                    else {
                                                        if ((messageType == bj_QUESTMESSAGE_WARNING)) {
                                                            DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_WARNING, " ")
                                                            DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_TEXT_DELAY_WARNING, message)
                                                            StartSound(bj_questWarningSound)
                                                        }
                                                        else {
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
function StartTimerBJ(t, periodic, timeout) {
    bj_lastStartedTimer = t
    TimerStart(t, timeout, periodic, null)
    return bj_lastStartedTimer
}
function CreateTimerBJ(periodic, timeout) {
    bj_lastStartedTimer = CreateTimer()
    TimerStart(bj_lastStartedTimer, timeout, periodic, null)
    return bj_lastStartedTimer
}
function DestroyTimerBJ(whichTimer) {
    DestroyTimer(whichTimer)
}
function PauseTimerBJ(pause, whichTimer) {
    if (pause) {
        PauseTimer(whichTimer)
    }
    else {
        ResumeTimer(whichTimer)
    }
}
function GetLastCreatedTimerBJ() {
    return bj_lastStartedTimer
}
function CreateTimerDialogBJ(t, title) {
    bj_lastCreatedTimerDialog = CreateTimerDialog(t)
    TimerDialogSetTitle(bj_lastCreatedTimerDialog, title)
    TimerDialogDisplay(bj_lastCreatedTimerDialog, true)
    return bj_lastCreatedTimerDialog
}
function DestroyTimerDialogBJ(td) {
    DestroyTimerDialog(td)
}
function TimerDialogSetTitleBJ(td, title) {
    TimerDialogSetTitle(td, title)
}
function TimerDialogSetTitleColorBJ(td, red, green, blue, transparency) {
    TimerDialogSetTitleColor(td, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - transparency))
}
function TimerDialogSetTimeColorBJ(td, red, green, blue, transparency) {
    TimerDialogSetTimeColor(td, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - transparency))
}
function TimerDialogSetSpeedBJ(td, speedMultFactor) {
    TimerDialogSetSpeed(td, speedMultFactor)
}
function TimerDialogDisplayForPlayerBJ(show, td, whichPlayer) {
    if ((GetLocalPlayer() == whichPlayer)) {
        TimerDialogDisplay(td, show)
    }
}
function TimerDialogDisplayBJ(show, td) {
    TimerDialogDisplay(td, show)
}
function GetLastCreatedTimerDialogBJ() {
    return bj_lastCreatedTimerDialog
}
function LeaderboardResizeBJ(lb) {
    var size = LeaderboardGetItemCount(lb);
    if ((LeaderboardGetLabelText(lb) == "")) {
        size = size - 1
    }
    LeaderboardSetSizeByItemCount(lb, size)
}
function LeaderboardSetPlayerItemValueBJ(whichPlayer, lb, val) {
    LeaderboardSetItemValue(lb, LeaderboardGetPlayerIndex(lb, whichPlayer), val)
}
function LeaderboardSetPlayerItemLabelBJ(whichPlayer, lb, val) {
    LeaderboardSetItemLabel(lb, LeaderboardGetPlayerIndex(lb, whichPlayer), val)
}
function LeaderboardSetPlayerItemStyleBJ(whichPlayer, lb, showLabel, showValue, showIcon) {
    LeaderboardSetItemStyle(lb, LeaderboardGetPlayerIndex(lb, whichPlayer), showLabel, showValue, showIcon)
}
function LeaderboardSetPlayerItemLabelColorBJ(whichPlayer, lb, red, green, blue, transparency) {
    LeaderboardSetItemLabelColor(lb, LeaderboardGetPlayerIndex(lb, whichPlayer), PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - transparency))
}
function LeaderboardSetPlayerItemValueColorBJ(whichPlayer, lb, red, green, blue, transparency) {
    LeaderboardSetItemValueColor(lb, LeaderboardGetPlayerIndex(lb, whichPlayer), PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - transparency))
}
function LeaderboardSetLabelColorBJ(lb, red, green, blue, transparency) {
    LeaderboardSetLabelColor(lb, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - transparency))
}
function LeaderboardSetValueColorBJ(lb, red, green, blue, transparency) {
    LeaderboardSetValueColor(lb, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - transparency))
}
function LeaderboardSetLabelBJ(lb, label) {
    LeaderboardSetLabel(lb, label)
    LeaderboardResizeBJ(lb)
}
function LeaderboardSetStyleBJ(lb, showLabel, showNames, showValues, showIcons) {
    LeaderboardSetStyle(lb, showLabel, showNames, showValues, showIcons)
}
function LeaderboardGetItemCountBJ(lb) {
    return LeaderboardGetItemCount(lb)
}
function LeaderboardHasPlayerItemBJ(lb, whichPlayer) {
    return LeaderboardHasPlayerItem(lb, whichPlayer)
}
function ForceSetLeaderboardBJ(lb, toForce) {
    var index;
    var indexPlayer;
    index = 0
    do {
        indexPlayer = Player(index)
        if (IsPlayerInForce(indexPlayer, toForce)) {
            PlayerSetLeaderboard(indexPlayer, lb)
        }
        index = index + 1
        if (index == bj_MAX_PLAYERS) { break; }
    } while (true)
}
function CreateLeaderboardBJ(toForce, label) {
    bj_lastCreatedLeaderboard = CreateLeaderboard()
    LeaderboardSetLabel(bj_lastCreatedLeaderboard, label)
    ForceSetLeaderboardBJ(bj_lastCreatedLeaderboard, toForce)
    LeaderboardDisplay(bj_lastCreatedLeaderboard, true)
    return bj_lastCreatedLeaderboard
}
function DestroyLeaderboardBJ(lb) {
    DestroyLeaderboard(lb)
}
function LeaderboardDisplayBJ(show, lb) {
    LeaderboardDisplay(lb, show)
}
function LeaderboardAddItemBJ(whichPlayer, lb, label, value) {
    if ((LeaderboardHasPlayerItem(lb, whichPlayer))) {
        LeaderboardRemovePlayerItem(lb, whichPlayer)
    }
    LeaderboardAddItem(lb, label, value, whichPlayer)
    LeaderboardResizeBJ(lb)
}
function LeaderboardRemovePlayerItemBJ(whichPlayer, lb) {
    LeaderboardRemovePlayerItem(lb, whichPlayer)
    LeaderboardResizeBJ(lb)
}
function LeaderboardSortItemsBJ(lb, sortType, ascending) {
    if ((sortType == bj_SORTTYPE_SORTBYVALUE)) {
        LeaderboardSortItemsByValue(lb, ascending)
    }
    else {
        if ((sortType == bj_SORTTYPE_SORTBYPLAYER)) {
            LeaderboardSortItemsByPlayer(lb, ascending)
        }
        else {
            if ((sortType == bj_SORTTYPE_SORTBYLABEL)) {
                LeaderboardSortItemsByLabel(lb, ascending)
            }
            else {
            }
        }
    }
}
function LeaderboardSortItemsByPlayerBJ(lb, ascending) {
    LeaderboardSortItemsByPlayer(lb, ascending)
}
function LeaderboardSortItemsByLabelBJ(lb, ascending) {
    LeaderboardSortItemsByLabel(lb, ascending)
}
function LeaderboardGetPlayerIndexBJ(whichPlayer, lb) {
    return LeaderboardGetPlayerIndex(lb, whichPlayer) + 1
}
function LeaderboardGetIndexedPlayerBJ(position, lb) {
    var index;
    var indexPlayer;
    index = 0
    do {
        indexPlayer = Player(index)
        if ((LeaderboardGetPlayerIndex(lb, indexPlayer) == position - 1)) {
            return indexPlayer
        }
        index = index + 1
        if (index == bj_MAX_PLAYERS) { break; }
    } while (true)
    return Player(PLAYER_NEUTRAL_PASSIVE)
}
function PlayerGetLeaderboardBJ(whichPlayer) {
    return PlayerGetLeaderboard(whichPlayer)
}
function GetLastCreatedLeaderboard() {
    return bj_lastCreatedLeaderboard
}
function CreateMultiboardBJ(cols, rows, title) {
    bj_lastCreatedMultiboard = CreateMultiboard()
    MultiboardSetRowCount(bj_lastCreatedMultiboard, rows)
    MultiboardSetColumnCount(bj_lastCreatedMultiboard, cols)
    MultiboardSetTitleText(bj_lastCreatedMultiboard, title)
    MultiboardDisplay(bj_lastCreatedMultiboard, true)
    return bj_lastCreatedMultiboard
}
function DestroyMultiboardBJ(mb) {
    DestroyMultiboard(mb)
}
function GetLastCreatedMultiboard() {
    return bj_lastCreatedMultiboard
}
function MultiboardDisplayBJ(show, mb) {
    MultiboardDisplay(mb, show)
}
function MultiboardMinimizeBJ(minimize, mb) {
    MultiboardMinimize(mb, minimize)
}
function MultiboardSetTitleTextColorBJ(mb, red, green, blue, transparency) {
    MultiboardSetTitleTextColor(mb, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - transparency))
}
function MultiboardAllowDisplayBJ(flag) {
    MultiboardSuppressDisplay(!flag)
}
function MultiboardSetItemStyleBJ(mb, col, row, showValue, showIcon) {
    var curRow = 0;
    var curCol = 0;
    var numRows = MultiboardGetRowCount(mb);
    var numCols = MultiboardGetColumnCount(mb);
    var mbitem = null;
    do {
        curRow = curRow + 1
        if (curRow > numRows) { break; }
        if ((row == 0 || row == curRow)) {
            curCol = 0
            do {
                curCol = curCol + 1
                if (curCol > numCols) { break; }
                if ((col == 0 || col == curCol)) {
                    mbitem = MultiboardGetItem(mb, curRow - 1, curCol - 1)
                    MultiboardSetItemStyle(mbitem, showValue, showIcon)
                    MultiboardReleaseItem(mbitem)
                }
            } while (true)
        }
    } while (true)
}
function MultiboardSetItemValueBJ(mb, col, row, val) {
    var curRow = 0;
    var curCol = 0;
    var numRows = MultiboardGetRowCount(mb);
    var numCols = MultiboardGetColumnCount(mb);
    var mbitem = null;
    do {
        curRow = curRow + 1
        if (curRow > numRows) { break; }
        if ((row == 0 || row == curRow)) {
            curCol = 0
            do {
                curCol = curCol + 1
                if (curCol > numCols) { break; }
                if ((col == 0 || col == curCol)) {
                    mbitem = MultiboardGetItem(mb, curRow - 1, curCol - 1)
                    MultiboardSetItemValue(mbitem, val)
                    MultiboardReleaseItem(mbitem)
                }
            } while (true)
        }
    } while (true)
}
function MultiboardSetItemColorBJ(mb, col, row, red, green, blue, transparency) {
    var curRow = 0;
    var curCol = 0;
    var numRows = MultiboardGetRowCount(mb);
    var numCols = MultiboardGetColumnCount(mb);
    var mbitem = null;
    do {
        curRow = curRow + 1
        if (curRow > numRows) { break; }
        if ((row == 0 || row == curRow)) {
            curCol = 0
            do {
                curCol = curCol + 1
                if (curCol > numCols) { break; }
                if ((col == 0 || col == curCol)) {
                    mbitem = MultiboardGetItem(mb, curRow - 1, curCol - 1)
                    MultiboardSetItemValueColor(mbitem, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - transparency))
                    MultiboardReleaseItem(mbitem)
                }
            } while (true)
        }
    } while (true)
}
function MultiboardSetItemWidthBJ(mb, col, row, width) {
    var curRow = 0;
    var curCol = 0;
    var numRows = MultiboardGetRowCount(mb);
    var numCols = MultiboardGetColumnCount(mb);
    var mbitem = null;
    do {
        curRow = curRow + 1
        if (curRow > numRows) { break; }
        if ((row == 0 || row == curRow)) {
            curCol = 0
            do {
                curCol = curCol + 1
                if (curCol > numCols) { break; }
                if ((col == 0 || col == curCol)) {
                    mbitem = MultiboardGetItem(mb, curRow - 1, curCol - 1)
                    MultiboardSetItemWidth(mbitem, width / 100.0)
                    MultiboardReleaseItem(mbitem)
                }
            } while (true)
        }
    } while (true)
}
function MultiboardSetItemIconBJ(mb, col, row, iconFileName) {
    var curRow = 0;
    var curCol = 0;
    var numRows = MultiboardGetRowCount(mb);
    var numCols = MultiboardGetColumnCount(mb);
    var mbitem = null;
    do {
        curRow = curRow + 1
        if (curRow > numRows) { break; }
        if ((row == 0 || row == curRow)) {
            curCol = 0
            do {
                curCol = curCol + 1
                if (curCol > numCols) { break; }
                if ((col == 0 || col == curCol)) {
                    mbitem = MultiboardGetItem(mb, curRow - 1, curCol - 1)
                    MultiboardSetItemIcon(mbitem, iconFileName)
                    MultiboardReleaseItem(mbitem)
                }
            } while (true)
        }
    } while (true)
}
function TextTagSize2Height(size) {
    return size * 0.023 / 10
}
function TextTagSpeed2Velocity(speed) {
    return speed * 0.071 / 128
}
function SetTextTagColorBJ(tt, red, green, blue, transparency) {
    SetTextTagColor(tt, PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - transparency))
}
function SetTextTagVelocityBJ(tt, speed, angle) {
    var vel = TextTagSpeed2Velocity(speed);
    var xvel = vel * Cos(angle * bj_DEGTORAD);
    var yvel = vel * Sin(angle * bj_DEGTORAD);
    SetTextTagVelocity(tt, xvel, yvel)
}
function SetTextTagTextBJ(tt, s, size) {
    var textHeight = TextTagSize2Height(size);
    SetTextTagText(tt, s, textHeight)
}
function SetTextTagPosBJ(tt, loc, zOffset) {
    SetTextTagPos(tt, GetLocationX(loc), GetLocationY(loc), zOffset)
}
function SetTextTagPosUnitBJ(tt, whichUnit, zOffset) {
    SetTextTagPosUnit(tt, whichUnit, zOffset)
}
function SetTextTagSuspendedBJ(tt, flag) {
    SetTextTagSuspended(tt, flag)
}
function SetTextTagPermanentBJ(tt, flag) {
    SetTextTagPermanent(tt, flag)
}
function SetTextTagAgeBJ(tt, age) {
    SetTextTagAge(tt, age)
}
function SetTextTagLifespanBJ(tt, lifespan) {
    SetTextTagLifespan(tt, lifespan)
}
function SetTextTagFadepointBJ(tt, fadepoint) {
    SetTextTagFadepoint(tt, fadepoint)
}
function CreateTextTagLocBJ(s, loc, zOffset, size, red, green, blue, transparency) {
    bj_lastCreatedTextTag = CreateTextTag()
    SetTextTagTextBJ(bj_lastCreatedTextTag, s, size)
    SetTextTagPosBJ(bj_lastCreatedTextTag, loc, zOffset)
    SetTextTagColorBJ(bj_lastCreatedTextTag, red, green, blue, transparency)
    return bj_lastCreatedTextTag
}
function CreateTextTagUnitBJ(s, whichUnit, zOffset, size, red, green, blue, transparency) {
    bj_lastCreatedTextTag = CreateTextTag()
    SetTextTagTextBJ(bj_lastCreatedTextTag, s, size)
    SetTextTagPosUnitBJ(bj_lastCreatedTextTag, whichUnit, zOffset)
    SetTextTagColorBJ(bj_lastCreatedTextTag, red, green, blue, transparency)
    return bj_lastCreatedTextTag
}
function DestroyTextTagBJ(tt) {
    DestroyTextTag(tt)
}
function ShowTextTagForceBJ(show, tt, whichForce) {
    if ((IsPlayerInForce(GetLocalPlayer(), whichForce))) {
        SetTextTagVisibility(tt, show)
    }
}
function GetLastCreatedTextTag() {
    return bj_lastCreatedTextTag
}
function PauseGameOn() {
    PauseGame(true)
}
function PauseGameOff() {
    PauseGame(false)
}
function SetUserControlForceOn(whichForce) {
    if ((IsPlayerInForce(GetLocalPlayer(), whichForce))) {
        EnableUserControl(true)
    }
}
function SetUserControlForceOff(whichForce) {
    if ((IsPlayerInForce(GetLocalPlayer(), whichForce))) {
        EnableUserControl(false)
    }
}
function ShowInterfaceForceOn(whichForce, fadeDuration) {
    if ((IsPlayerInForce(GetLocalPlayer(), whichForce))) {
        ShowInterface(true, fadeDuration)
    }
}
function ShowInterfaceForceOff(whichForce, fadeDuration) {
    if ((IsPlayerInForce(GetLocalPlayer(), whichForce))) {
        ShowInterface(false, fadeDuration)
    }
}
function PingMinimapForForce(whichForce, x, y, duration) {
    if ((IsPlayerInForce(GetLocalPlayer(), whichForce))) {
        PingMinimap(x, y, duration)
    }
}
function PingMinimapLocForForce(whichForce, loc, duration) {
    PingMinimapForForce(whichForce, GetLocationX(loc), GetLocationY(loc), duration)
}
function PingMinimapForPlayer(whichPlayer, x, y, duration) {
    if ((GetLocalPlayer() == whichPlayer)) {
        PingMinimap(x, y, duration)
    }
}
function PingMinimapLocForPlayer(whichPlayer, loc, duration) {
    PingMinimapForPlayer(whichPlayer, GetLocationX(loc), GetLocationY(loc), duration)
}
function PingMinimapForForceEx(whichForce, x, y, duration, style, red, green, blue) {
    var red255 = PercentTo255(red);
    var green255 = PercentTo255(green);
    var blue255 = PercentTo255(blue);
    if ((IsPlayerInForce(GetLocalPlayer(), whichForce))) {
        if ((red255 == 255) && (green255 == 0) && (blue255 == 0)) {
            red255 = 254
        }
        if ((style == bj_MINIMAPPINGSTYLE_SIMPLE)) {
            PingMinimapEx(x, y, duration, red255, green255, blue255, false)
        }
        else {
            if ((style == bj_MINIMAPPINGSTYLE_FLASHY)) {
                PingMinimapEx(x, y, duration, red255, green255, blue255, true)
            }
            else {
                if ((style == bj_MINIMAPPINGSTYLE_ATTACK)) {
                    PingMinimapEx(x, y, duration, 255, 0, 0, false)
                }
                else {
                }
            }
        }
    }
}
function PingMinimapLocForForceEx(whichForce, loc, duration, style, red, green, blue) {
    PingMinimapForForceEx(whichForce, GetLocationX(loc), GetLocationY(loc), duration, style, red, green, blue)
}
function EnableWorldFogBoundaryBJ(enable, f) {
    if ((IsPlayerInForce(GetLocalPlayer(), f))) {
        EnableWorldFogBoundary(enable)
    }
}
function EnableOcclusionBJ(enable, f) {
    if ((IsPlayerInForce(GetLocalPlayer(), f))) {
        EnableOcclusion(enable)
    }
}
function CancelCineSceneBJ() {
    StopSoundBJ(bj_cineSceneLastSound, true)
    EndCinematicScene()
}
function TryInitCinematicBehaviorBJ() {
    var index;
    if ((bj_cineSceneBeingSkipped == null)) {
        bj_cineSceneBeingSkipped = CreateTrigger()
        index = 0
        do {
            TriggerRegisterPlayerEvent(bj_cineSceneBeingSkipped, Player(index), EVENT_PLAYER_END_CINEMATIC)
            index = index + 1
            if (index == bj_MAX_PLAYERS) { break; }
        } while (true)
        TriggerAddAction(bj_cineSceneBeingSkipped, CancelCineSceneBJ)
    }
}
function SetCinematicSceneBJ(soundHandle, portraitUnitId, color, speakerTitle, text, sceneDuration, voiceoverDuration) {
    bj_cineSceneLastSound = soundHandle
    SetCinematicScene(portraitUnitId, color, speakerTitle, text, sceneDuration, voiceoverDuration)
    PlaySoundBJ(soundHandle)
}
function GetTransmissionDuration(soundHandle, timeType, timeVal) {
    var duration;
    if ((timeType == bj_TIMETYPE_ADD)) {
        duration = GetSoundDurationBJ(soundHandle) + timeVal
    }
    else {
        if ((timeType == bj_TIMETYPE_SET)) {
            duration = timeVal
        }
        else {
            if ((timeType == bj_TIMETYPE_SUB)) {
                duration = GetSoundDurationBJ(soundHandle) - timeVal
            }
            else {
                duration = GetSoundDurationBJ(soundHandle)
            }
        }
    }
    if ((duration < 0)) {
        duration = 0
    }
    return duration
}
function WaitTransmissionDuration(soundHandle, timeType, timeVal) {
    if ((timeType == bj_TIMETYPE_SET)) {
        TriggerSleepAction(timeVal)
    }
    else {
        if ((soundHandle == null)) {
            TriggerSleepAction(bj_NOTHING_SOUND_DURATION)
        }
        else {
            if ((timeType == bj_TIMETYPE_SUB)) {
                WaitForSoundBJ(soundHandle, timeVal)
            }
            else {
                if ((timeType == bj_TIMETYPE_ADD)) {
                    WaitForSoundBJ(soundHandle, 0)
                    TriggerSleepAction(timeVal)
                }
                else {
                }
            }
        }
    }
}
function DoTransmissionBasicsXYBJ(unitId, color, x, y, soundHandle, unitName, message, duration) {
    SetCinematicSceneBJ(soundHandle, unitId, color, unitName, message, duration + bj_TRANSMISSION_PORT_HANGTIME, duration)
    if ((unitId != 0)) {
        PingMinimap(x, y, bj_TRANSMISSION_PING_TIME)
    }
}
function TransmissionFromUnitWithNameBJ(toForce, whichUnit, unitName, soundHandle, message, timeType, timeVal, wait) {
    TryInitCinematicBehaviorBJ()
    AttachSoundToUnit(soundHandle, whichUnit)
    timeVal = RMaxBJ(timeVal, 0)
    bj_lastTransmissionDuration = GetTransmissionDuration(soundHandle, timeType, timeVal)
    bj_lastPlayedSound = soundHandle
    if ((IsPlayerInForce(GetLocalPlayer(), toForce))) {
        if ((whichUnit == null)) {
            DoTransmissionBasicsXYBJ(0, PLAYER_COLOR_RED, 0, 0, soundHandle, unitName, message, bj_lastTransmissionDuration)
        }
        else {
            DoTransmissionBasicsXYBJ(GetUnitTypeId(whichUnit), GetPlayerColor(GetOwningPlayer(whichUnit)), GetUnitX(whichUnit), GetUnitY(whichUnit), soundHandle, unitName, message, bj_lastTransmissionDuration)
            if ((!IsUnitHidden(whichUnit))) {
                UnitAddIndicator(whichUnit, bj_TRANSMISSION_IND_RED, bj_TRANSMISSION_IND_BLUE, bj_TRANSMISSION_IND_GREEN, bj_TRANSMISSION_IND_ALPHA)
            }
        }
    }
    if (wait && (bj_lastTransmissionDuration > 0)) {
        WaitTransmissionDuration(soundHandle, timeType, timeVal)
    }
}
function PlayDialogueFromSpeakerEx(toForce, speaker, speakerType, soundHandle, timeType, timeVal, wait) {
    if (GetUnitTypeId(speaker) != speakerType) {
        debugBJDebugMsg((S2S("Attempted to play FacialAnimation with the wrong speaker UnitType - Param: " + I2S(speakerType)) + " Runtime: " + I2S(GetUnitTypeId(speaker))))
    }
    TryInitCinematicBehaviorBJ()
    AttachSoundToUnit(soundHandle, speaker)
    timeVal = RMaxBJ(timeVal, 0)
    bj_lastTransmissionDuration = GetTransmissionDuration(soundHandle, timeType, timeVal)
    bj_lastPlayedSound = soundHandle
    if ((IsPlayerInForce(GetLocalPlayer(), toForce))) {
        SetCinematicSceneBJ(soundHandle, speakerType, GetPlayerColor(GetOwningPlayer(speaker)), GetLocalizedString(GetDialogueSpeakerNameKey(soundHandle)), GetLocalizedString(GetDialogueTextKey(soundHandle)), bj_lastTransmissionDuration + bj_TRANSMISSION_PORT_HANGTIME, bj_lastTransmissionDuration)
    }
    if (wait && (bj_lastTransmissionDuration > 0)) {
        WaitTransmissionDuration(soundHandle, timeType, timeVal)
    }
    return true
}
function PlayDialogueFromSpeakerTypeEx(toForce, fromPlayer, speakerType, loc, soundHandle, timeType, timeVal, wait) {
    TryInitCinematicBehaviorBJ()
    timeVal = RMaxBJ(timeVal, 0)
    bj_lastTransmissionDuration = GetTransmissionDuration(soundHandle, timeType, timeVal)
    bj_lastPlayedSound = soundHandle
    if ((IsPlayerInForce(GetLocalPlayer(), toForce))) {
        SetCinematicSceneBJ(soundHandle, speakerType, GetPlayerColor(fromPlayer), GetLocalizedString(GetDialogueSpeakerNameKey(soundHandle)), GetLocalizedString(GetDialogueTextKey(soundHandle)), bj_lastTransmissionDuration + bj_TRANSMISSION_PORT_HANGTIME, bj_lastTransmissionDuration)
        if ((speakerType != 0)) {
            PingMinimap(GetLocationX(loc), GetLocationY(loc), bj_TRANSMISSION_PING_TIME)
        }
    }
    if (wait && (bj_lastTransmissionDuration > 0)) {
        WaitTransmissionDuration(soundHandle, timeType, timeVal)
    }
    return true
}
function TransmissionFromUnitTypeWithNameBJ(toForce, fromPlayer, unitId, unitName, loc, soundHandle, message, timeType, timeVal, wait) {
    TryInitCinematicBehaviorBJ()
    timeVal = RMaxBJ(timeVal, 0)
    bj_lastTransmissionDuration = GetTransmissionDuration(soundHandle, timeType, timeVal)
    bj_lastPlayedSound = soundHandle
    if ((IsPlayerInForce(GetLocalPlayer(), toForce))) {
        DoTransmissionBasicsXYBJ(unitId, GetPlayerColor(fromPlayer), GetLocationX(loc), GetLocationY(loc), soundHandle, unitName, message, bj_lastTransmissionDuration)
    }
    if (wait && (bj_lastTransmissionDuration > 0)) {
        WaitTransmissionDuration(soundHandle, timeType, timeVal)
    }
}
function GetLastTransmissionDurationBJ() {
    return bj_lastTransmissionDuration
}
function ForceCinematicSubtitlesBJ(flag) {
    ForceCinematicSubtitles(flag)
}
function CinematicModeExBJ(cineMode, forForce, interfaceFadeTime) {
    if ((!bj_gameStarted)) {
        interfaceFadeTime = 0
    }
    if ((cineMode)) {
        if ((!bj_cineModeAlreadyIn)) {
            SetCinematicAudio(true)
            bj_cineModeAlreadyIn = true
            bj_cineModePriorSpeed = GetGameSpeed()
            bj_cineModePriorFogSetting = IsFogEnabled()
            bj_cineModePriorMaskSetting = IsFogMaskEnabled()
            bj_cineModePriorDawnDusk = IsDawnDuskEnabled()
            bj_cineModeSavedSeed = GetRandomInt(0, 1000000)
        }
        if ((IsPlayerInForce(GetLocalPlayer(), forForce))) {
            ClearTextMessages()
            ShowInterface(false, interfaceFadeTime)
            EnableUserControl(false)
            EnableOcclusion(false)
            SetCineModeVolumeGroupsBJ()
        }
        SetGameSpeed(bj_CINEMODE_GAMESPEED)
        SetMapFlag(MAP_LOCK_SPEED, true)
        FogMaskEnable(false)
        FogEnable(false)
        EnableWorldFogBoundary(false)
        EnableDawnDusk(false)
        SetRandomSeed(0)
    }
    else {
        bj_cineModeAlreadyIn = false
        SetCinematicAudio(false)
        if ((IsPlayerInForce(GetLocalPlayer(), forForce))) {
            ShowInterface(true, interfaceFadeTime)
            EnableUserControl(true)
            EnableOcclusion(true)
            VolumeGroupReset()
            EndThematicMusic()
            CameraResetSmoothingFactorBJ()
        }
        SetMapFlag(MAP_LOCK_SPEED, false)
        SetGameSpeed(bj_cineModePriorSpeed)
        FogMaskEnable(bj_cineModePriorMaskSetting)
        FogEnable(bj_cineModePriorFogSetting)
        EnableWorldFogBoundary(true)
        EnableDawnDusk(bj_cineModePriorDawnDusk)
        SetRandomSeed(bj_cineModeSavedSeed)
    }
}
function CinematicModeBJ(cineMode, forForce) {
    CinematicModeExBJ(cineMode, forForce, bj_CINEMODE_INTERFACEFADE)
}
function DisplayCineFilterBJ(flag) {
    DisplayCineFilter(flag)
}
function CinematicFadeCommonBJ(red, green, blue, duration, tex, startTrans, endTrans) {
    if ((duration == 0)) {
        startTrans = endTrans
    }
    EnableUserUI(false)
    SetCineFilterTexture(tex)
    SetCineFilterBlendMode(BLEND_MODE_BLEND)
    SetCineFilterTexMapFlags(TEXMAP_FLAG_NONE)
    SetCineFilterStartUV(0, 0, 1, 1)
    SetCineFilterEndUV(0, 0, 1, 1)
    SetCineFilterStartColor(PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - startTrans))
    SetCineFilterEndColor(PercentTo255(red), PercentTo255(green), PercentTo255(blue), PercentTo255(100.0 - endTrans))
    SetCineFilterDuration(duration)
    DisplayCineFilter(true)
}
function FinishCinematicFadeBJ() {
    DestroyTimer(bj_cineFadeFinishTimer)
    bj_cineFadeFinishTimer = null
    DisplayCineFilter(false)
    EnableUserUI(true)
}
function FinishCinematicFadeAfterBJ(duration) {
    bj_cineFadeFinishTimer = CreateTimer()
    TimerStart(bj_cineFadeFinishTimer, duration, false, FinishCinematicFadeBJ)
}
function ContinueCinematicFadeBJ() {
    DestroyTimer(bj_cineFadeContinueTimer)
    bj_cineFadeContinueTimer = null
    CinematicFadeCommonBJ(bj_cineFadeContinueRed, bj_cineFadeContinueGreen, bj_cineFadeContinueBlue, bj_cineFadeContinueDuration, bj_cineFadeContinueTex, bj_cineFadeContinueTrans, 100)
}
function ContinueCinematicFadeAfterBJ(duration, red, green, blue, trans, tex) {
    bj_cineFadeContinueRed = red
    bj_cineFadeContinueGreen = green
    bj_cineFadeContinueBlue = blue
    bj_cineFadeContinueTrans = trans
    bj_cineFadeContinueDuration = duration
    bj_cineFadeContinueTex = tex
    bj_cineFadeContinueTimer = CreateTimer()
    TimerStart(bj_cineFadeContinueTimer, duration, false, ContinueCinematicFadeBJ)
}
function AbortCinematicFadeBJ() {
    if ((bj_cineFadeContinueTimer != null)) {
        DestroyTimer(bj_cineFadeContinueTimer)
    }
    if ((bj_cineFadeFinishTimer != null)) {
        DestroyTimer(bj_cineFadeFinishTimer)
    }
}
function CinematicFadeBJ(fadetype, duration, tex, red, green, blue, trans) {
    if ((fadetype == bj_CINEFADETYPE_FADEOUT)) {
        AbortCinematicFadeBJ()
        CinematicFadeCommonBJ(red, green, blue, duration, tex, 100, trans)
    }
    else {
        if ((fadetype == bj_CINEFADETYPE_FADEIN)) {
            AbortCinematicFadeBJ()
            CinematicFadeCommonBJ(red, green, blue, duration, tex, trans, 100)
            FinishCinematicFadeAfterBJ(duration)
        }
        else {
            if ((fadetype == bj_CINEFADETYPE_FADEOUTIN)) {
                if ((duration > 0)) {
                    AbortCinematicFadeBJ()
                    CinematicFadeCommonBJ(red, green, blue, duration * 0.5, tex, 100, trans)
                    ContinueCinematicFadeAfterBJ(duration * 0.5, red, green, blue, trans, tex)
                    FinishCinematicFadeAfterBJ(duration)
                }
            }
            else {
            }
        }
    }
}
function CinematicFilterGenericBJ(duration, bmode, tex, red0, green0, blue0, trans0, red1, green1, blue1, trans1) {
    AbortCinematicFadeBJ()
    SetCineFilterTexture(tex)
    SetCineFilterBlendMode(bmode)
    SetCineFilterTexMapFlags(TEXMAP_FLAG_NONE)
    SetCineFilterStartUV(0, 0, 1, 1)
    SetCineFilterEndUV(0, 0, 1, 1)
    SetCineFilterStartColor(PercentTo255(red0), PercentTo255(green0), PercentTo255(blue0), PercentTo255(100.0 - trans0))
    SetCineFilterEndColor(PercentTo255(red1), PercentTo255(green1), PercentTo255(blue1), PercentTo255(100.0 - trans1))
    SetCineFilterDuration(duration)
    DisplayCineFilter(true)
}
function RescueUnitBJ(whichUnit, rescuer, changeColor) {
    if (IsUnitDeadBJ(whichUnit) || (GetOwningPlayer(whichUnit) == rescuer)) {
        return
    }
    StartSound(bj_rescueSound)
    SetUnitOwner(whichUnit, rescuer, changeColor)
    UnitAddIndicator(whichUnit, 0, 255, 0, 255)
    PingMinimapForPlayer(rescuer, GetUnitX(whichUnit), GetUnitY(whichUnit), bj_RESCUE_PING_TIME)
}
function TriggerActionUnitRescuedBJ() {
    var theUnit = GetTriggerUnit();
    if (IsUnitType(theUnit, UNIT_TYPE_STRUCTURE)) {
        RescueUnitBJ(theUnit, GetOwningPlayer(GetRescuer()), bj_rescueChangeColorBldg)
    }
    else {
        RescueUnitBJ(theUnit, GetOwningPlayer(GetRescuer()), bj_rescueChangeColorUnit)
    }
}
function TryInitRescuableTriggersBJ() {
    var index;
    if ((bj_rescueUnitBehavior == null)) {
        bj_rescueUnitBehavior = CreateTrigger()
        index = 0
        do {
            TriggerRegisterPlayerUnitEvent(bj_rescueUnitBehavior, Player(index), EVENT_PLAYER_UNIT_RESCUED, null)
            index = index + 1
            if (index == bj_MAX_PLAYER_SLOTS) { break; }
        } while (true)
        TriggerAddAction(bj_rescueUnitBehavior, TriggerActionUnitRescuedBJ)
    }
}
function SetRescueUnitColorChangeBJ(changeColor) {
    bj_rescueChangeColorUnit = changeColor
}
function SetRescueBuildingColorChangeBJ(changeColor) {
    bj_rescueChangeColorBldg = changeColor
}
function MakeUnitRescuableToForceBJEnum() {
    TryInitRescuableTriggersBJ()
    SetUnitRescuable(bj_makeUnitRescuableUnit, GetEnumPlayer(), bj_makeUnitRescuableFlag)
}
function MakeUnitRescuableToForceBJ(whichUnit, isRescuable, whichForce) {
    bj_makeUnitRescuableUnit = whichUnit
    bj_makeUnitRescuableFlag = isRescuable
    ForForce(whichForce, MakeUnitRescuableToForceBJEnum)
}
function InitRescuableBehaviorBJ() {
    var index;
    index = 0
    do {
        if ((GetPlayerController(Player(index)) == MAP_CONTROL_RESCUABLE)) {
            TryInitRescuableTriggersBJ()
            return
        }
        index = index + 1
        if (index == bj_MAX_PLAYERS) { break; }
    } while (true)
}
function SetPlayerTechResearchedSwap(techid, levels, whichPlayer) {
    SetPlayerTechResearched(whichPlayer, techid, levels)
}
function SetPlayerTechMaxAllowedSwap(techid, maximum, whichPlayer) {
    SetPlayerTechMaxAllowed(whichPlayer, techid, maximum)
}
function SetPlayerMaxHeroesAllowed(maximum, whichPlayer) {
    SetPlayerTechMaxAllowed(whichPlayer, 'HERO', maximum)
}
function GetPlayerTechCountSimple(techid, whichPlayer) {
    return GetPlayerTechCount(whichPlayer, techid, true)
}
function GetPlayerTechMaxAllowedSwap(techid, whichPlayer) {
    return GetPlayerTechMaxAllowed(whichPlayer, techid)
}
function SetPlayerAbilityAvailableBJ(avail, abilid, whichPlayer) {
    SetPlayerAbilityAvailable(whichPlayer, abilid, avail)
}
function SetCampaignMenuRaceBJ(campaignNumber) {
    if ((campaignNumber == bj_CAMPAIGN_INDEX_T)) {
        SetCampaignMenuRace(RACE_OTHER)
    }
    else {
        if ((campaignNumber == bj_CAMPAIGN_INDEX_H)) {
            SetCampaignMenuRace(RACE_HUMAN)
        }
        else {
            if ((campaignNumber == bj_CAMPAIGN_INDEX_U)) {
                SetCampaignMenuRace(RACE_UNDEAD)
            }
            else {
                if ((campaignNumber == bj_CAMPAIGN_INDEX_O)) {
                    SetCampaignMenuRace(RACE_ORC)
                }
                else {
                    if ((campaignNumber == bj_CAMPAIGN_INDEX_N)) {
                        SetCampaignMenuRace(RACE_NIGHTELF)
                    }
                    else {
                        if ((campaignNumber == bj_CAMPAIGN_INDEX_XN)) {
                            SetCampaignMenuRaceEx(bj_CAMPAIGN_OFFSET_XN)
                        }
                        else {
                            if ((campaignNumber == bj_CAMPAIGN_INDEX_XH)) {
                                SetCampaignMenuRaceEx(bj_CAMPAIGN_OFFSET_XH)
                            }
                            else {
                                if ((campaignNumber == bj_CAMPAIGN_INDEX_XU)) {
                                    SetCampaignMenuRaceEx(bj_CAMPAIGN_OFFSET_XU)
                                }
                                else {
                                    if ((campaignNumber == bj_CAMPAIGN_INDEX_XO)) {
                                        SetCampaignMenuRaceEx(bj_CAMPAIGN_OFFSET_XO)
                                    }
                                    else {
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
function SetMissionAvailableBJ(available, missionIndex) {
    var campaignNumber = R2I(missionIndex / 1000);
    var missionNumber = missionIndex - campaignNumber * 1000;
    SetMissionAvailable(campaignNumber, missionNumber, available)
}
function SetCampaignAvailableBJ(available, campaignNumber) {
    var campaignOffset;
    if ((campaignNumber == bj_CAMPAIGN_INDEX_H)) {
        SetTutorialCleared(true)
    }
    if ((campaignNumber == bj_CAMPAIGN_INDEX_XN)) {
        campaignOffset = bj_CAMPAIGN_OFFSET_XN
    }
    else {
        if ((campaignNumber == bj_CAMPAIGN_INDEX_XH)) {
            campaignOffset = bj_CAMPAIGN_OFFSET_XH
        }
        else {
            if ((campaignNumber == bj_CAMPAIGN_INDEX_XU)) {
                campaignOffset = bj_CAMPAIGN_OFFSET_XU
            }
            else {
                if ((campaignNumber == bj_CAMPAIGN_INDEX_XO)) {
                    campaignOffset = bj_CAMPAIGN_OFFSET_XO
                }
                else {
                    campaignOffset = campaignNumber
                }
            }
        }
    }
    SetCampaignAvailable(campaignOffset, available)
    SetCampaignMenuRaceBJ(campaignNumber)
    ForceCampaignSelectScreen()
}
function SetCinematicAvailableBJ(available, cinematicIndex) {
    if ((cinematicIndex == bj_CINEMATICINDEX_TOP)) {
        SetOpCinematicAvailable(bj_CAMPAIGN_INDEX_T, available)
        PlayCinematic("TutorialOp")
    }
    else {
        if ((cinematicIndex == bj_CINEMATICINDEX_HOP)) {
            SetOpCinematicAvailable(bj_CAMPAIGN_INDEX_H, available)
            PlayCinematic("HumanOp")
        }
        else {
            if ((cinematicIndex == bj_CINEMATICINDEX_HED)) {
                SetEdCinematicAvailable(bj_CAMPAIGN_INDEX_H, available)
                PlayCinematic("HumanEd")
            }
            else {
                if ((cinematicIndex == bj_CINEMATICINDEX_OOP)) {
                    SetOpCinematicAvailable(bj_CAMPAIGN_INDEX_O, available)
                    PlayCinematic("OrcOp")
                }
                else {
                    if ((cinematicIndex == bj_CINEMATICINDEX_OED)) {
                        SetEdCinematicAvailable(bj_CAMPAIGN_INDEX_O, available)
                        PlayCinematic("OrcEd")
                    }
                    else {
                        if ((cinematicIndex == bj_CINEMATICINDEX_UOP)) {
                            SetEdCinematicAvailable(bj_CAMPAIGN_INDEX_U, available)
                            PlayCinematic("UndeadOp")
                        }
                        else {
                            if ((cinematicIndex == bj_CINEMATICINDEX_UED)) {
                                SetEdCinematicAvailable(bj_CAMPAIGN_INDEX_U, available)
                                PlayCinematic("UndeadEd")
                            }
                            else {
                                if ((cinematicIndex == bj_CINEMATICINDEX_NOP)) {
                                    SetEdCinematicAvailable(bj_CAMPAIGN_INDEX_N, available)
                                    PlayCinematic("NightElfOp")
                                }
                                else {
                                    if ((cinematicIndex == bj_CINEMATICINDEX_NED)) {
                                        SetEdCinematicAvailable(bj_CAMPAIGN_INDEX_N, available)
                                        PlayCinematic("NightElfEd")
                                    }
                                    else {
                                        if ((cinematicIndex == bj_CINEMATICINDEX_XOP)) {
                                            SetOpCinematicAvailable(bj_CAMPAIGN_OFFSET_XN, available)
                                        }
                                        else {
                                            if ((cinematicIndex == bj_CINEMATICINDEX_XED)) {
                                                SetEdCinematicAvailable(bj_CAMPAIGN_OFFSET_XU, available)
                                                PlayCinematic("OutroX")
                                            }
                                            else {
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
function InitGameCacheBJ(campaignFile) {
    bj_lastCreatedGameCache = InitGameCache(campaignFile)
    return bj_lastCreatedGameCache
}
function SaveGameCacheBJ(cache) {
    return SaveGameCache(cache)
}
function GetLastCreatedGameCacheBJ() {
    return bj_lastCreatedGameCache
}
function InitHashtableBJ() {
    bj_lastCreatedHashtable = InitHashtable()
    return bj_lastCreatedHashtable
}
function GetLastCreatedHashtableBJ() {
    return bj_lastCreatedHashtable
}
function StoreRealBJ(value, key, missionKey, cache) {
    StoreReal(cache, missionKey, key, value)
}
function StoreIntegerBJ(value, key, missionKey, cache) {
    StoreInteger(cache, missionKey, key, value)
}
function StoreBooleanBJ(value, key, missionKey, cache) {
    StoreBoolean(cache, missionKey, key, value)
}
function StoreStringBJ(value, key, missionKey, cache) {
    return StoreString(cache, missionKey, key, value)
}
function StoreUnitBJ(whichUnit, key, missionKey, cache) {
    return StoreUnit(cache, missionKey, key, whichUnit)
}
function SaveRealBJ(value, key, missionKey, table) {
    SaveReal(table, missionKey, key, value)
}
function SaveIntegerBJ(value, key, missionKey, table) {
    SaveInteger(table, missionKey, key, value)
}
function SaveBooleanBJ(value, key, missionKey, table) {
    SaveBoolean(table, missionKey, key, value)
}
function SaveStringBJ(value, key, missionKey, table) {
    return SaveStr(table, missionKey, key, value)
}
function SavePlayerHandleBJ(whichPlayer, key, missionKey, table) {
    return SavePlayerHandle(table, missionKey, key, whichPlayer)
}
function SaveWidgetHandleBJ(whichWidget, key, missionKey, table) {
    return SaveWidgetHandle(table, missionKey, key, whichWidget)
}
function SaveDestructableHandleBJ(whichDestructable, key, missionKey, table) {
    return SaveDestructableHandle(table, missionKey, key, whichDestructable)
}
function SaveItemHandleBJ(whichItem, key, missionKey, table) {
    return SaveItemHandle(table, missionKey, key, whichItem)
}
function SaveUnitHandleBJ(whichUnit, key, missionKey, table) {
    return SaveUnitHandle(table, missionKey, key, whichUnit)
}
function SaveAbilityHandleBJ(whichAbility, key, missionKey, table) {
    return SaveAbilityHandle(table, missionKey, key, whichAbility)
}
function SaveTimerHandleBJ(whichTimer, key, missionKey, table) {
    return SaveTimerHandle(table, missionKey, key, whichTimer)
}
function SaveTriggerHandleBJ(whichTrigger, key, missionKey, table) {
    return SaveTriggerHandle(table, missionKey, key, whichTrigger)
}
function SaveTriggerConditionHandleBJ(whichTriggercondition, key, missionKey, table) {
    return SaveTriggerConditionHandle(table, missionKey, key, whichTriggercondition)
}
function SaveTriggerActionHandleBJ(whichTriggeraction, key, missionKey, table) {
    return SaveTriggerActionHandle(table, missionKey, key, whichTriggeraction)
}
function SaveTriggerEventHandleBJ(whichEvent, key, missionKey, table) {
    return SaveTriggerEventHandle(table, missionKey, key, whichEvent)
}
function SaveForceHandleBJ(whichForce, key, missionKey, table) {
    return SaveForceHandle(table, missionKey, key, whichForce)
}
function SaveGroupHandleBJ(whichGroup, key, missionKey, table) {
    return SaveGroupHandle(table, missionKey, key, whichGroup)
}
function SaveLocationHandleBJ(whichLocation, key, missionKey, table) {
    return SaveLocationHandle(table, missionKey, key, whichLocation)
}
function SaveRectHandleBJ(whichRect, key, missionKey, table) {
    return SaveRectHandle(table, missionKey, key, whichRect)
}
function SaveBooleanExprHandleBJ(whichBoolexpr, key, missionKey, table) {
    return SaveBooleanExprHandle(table, missionKey, key, whichBoolexpr)
}
function SaveSoundHandleBJ(whichSound, key, missionKey, table) {
    return SaveSoundHandle(table, missionKey, key, whichSound)
}
function SaveEffectHandleBJ(whichEffect, key, missionKey, table) {
    return SaveEffectHandle(table, missionKey, key, whichEffect)
}
function SaveUnitPoolHandleBJ(whichUnitpool, key, missionKey, table) {
    return SaveUnitPoolHandle(table, missionKey, key, whichUnitpool)
}
function SaveItemPoolHandleBJ(whichItempool, key, missionKey, table) {
    return SaveItemPoolHandle(table, missionKey, key, whichItempool)
}
function SaveQuestHandleBJ(whichQuest, key, missionKey, table) {
    return SaveQuestHandle(table, missionKey, key, whichQuest)
}
function SaveQuestItemHandleBJ(whichQuestitem, key, missionKey, table) {
    return SaveQuestItemHandle(table, missionKey, key, whichQuestitem)
}
function SaveDefeatConditionHandleBJ(whichDefeatcondition, key, missionKey, table) {
    return SaveDefeatConditionHandle(table, missionKey, key, whichDefeatcondition)
}
function SaveTimerDialogHandleBJ(whichTimerdialog, key, missionKey, table) {
    return SaveTimerDialogHandle(table, missionKey, key, whichTimerdialog)
}
function SaveLeaderboardHandleBJ(whichLeaderboard, key, missionKey, table) {
    return SaveLeaderboardHandle(table, missionKey, key, whichLeaderboard)
}
function SaveMultiboardHandleBJ(whichMultiboard, key, missionKey, table) {
    return SaveMultiboardHandle(table, missionKey, key, whichMultiboard)
}
function SaveMultiboardItemHandleBJ(whichMultiboarditem, key, missionKey, table) {
    return SaveMultiboardItemHandle(table, missionKey, key, whichMultiboarditem)
}
function SaveTrackableHandleBJ(whichTrackable, key, missionKey, table) {
    return SaveTrackableHandle(table, missionKey, key, whichTrackable)
}
function SaveDialogHandleBJ(whichDialog, key, missionKey, table) {
    return SaveDialogHandle(table, missionKey, key, whichDialog)
}
function SaveButtonHandleBJ(whichButton, key, missionKey, table) {
    return SaveButtonHandle(table, missionKey, key, whichButton)
}
function SaveTextTagHandleBJ(whichTexttag, key, missionKey, table) {
    return SaveTextTagHandle(table, missionKey, key, whichTexttag)
}
function SaveLightningHandleBJ(whichLightning, key, missionKey, table) {
    return SaveLightningHandle(table, missionKey, key, whichLightning)
}
function SaveImageHandleBJ(whichImage, key, missionKey, table) {
    return SaveImageHandle(table, missionKey, key, whichImage)
}
function SaveUbersplatHandleBJ(whichUbersplat, key, missionKey, table) {
    return SaveUbersplatHandle(table, missionKey, key, whichUbersplat)
}
function SaveRegionHandleBJ(whichRegion, key, missionKey, table) {
    return SaveRegionHandle(table, missionKey, key, whichRegion)
}
function SaveFogStateHandleBJ(whichFogState, key, missionKey, table) {
    return SaveFogStateHandle(table, missionKey, key, whichFogState)
}
function SaveFogModifierHandleBJ(whichFogModifier, key, missionKey, table) {
    return SaveFogModifierHandle(table, missionKey, key, whichFogModifier)
}
function SaveAgentHandleBJ(whichAgent, key, missionKey, table) {
    return SaveAgentHandle(table, missionKey, key, whichAgent)
}
function SaveHashtableHandleBJ(whichHashtable, key, missionKey, table) {
    return SaveHashtableHandle(table, missionKey, key, whichHashtable)
}
function GetStoredRealBJ(key, missionKey, cache) {
    return GetStoredReal(cache, missionKey, key)
}
function GetStoredIntegerBJ(key, missionKey, cache) {
    return GetStoredInteger(cache, missionKey, key)
}
function GetStoredBooleanBJ(key, missionKey, cache) {
    return GetStoredBoolean(cache, missionKey, key)
}
function GetStoredStringBJ(key, missionKey, cache) {
    var s;
    s = GetStoredString(cache, missionKey, key)
    if ((s == null)) {
        return ""
    }
    else {
        return s
    }
}
function LoadRealBJ(key, missionKey, table) {
    return LoadReal(table, missionKey, key)
}
function LoadIntegerBJ(key, missionKey, table) {
    return LoadInteger(table, missionKey, key)
}
function LoadBooleanBJ(key, missionKey, table) {
    return LoadBoolean(table, missionKey, key)
}
function LoadStringBJ(key, missionKey, table) {
    var s;
    s = LoadStr(table, missionKey, key)
    if ((s == null)) {
        return ""
    }
    else {
        return s
    }
}
function LoadPlayerHandleBJ(key, missionKey, table) {
    return LoadPlayerHandle(table, missionKey, key)
}
function LoadWidgetHandleBJ(key, missionKey, table) {
    return LoadWidgetHandle(table, missionKey, key)
}
function LoadDestructableHandleBJ(key, missionKey, table) {
    return LoadDestructableHandle(table, missionKey, key)
}
function LoadItemHandleBJ(key, missionKey, table) {
    return LoadItemHandle(table, missionKey, key)
}
function LoadUnitHandleBJ(key, missionKey, table) {
    return LoadUnitHandle(table, missionKey, key)
}
function LoadAbilityHandleBJ(key, missionKey, table) {
    return LoadAbilityHandle(table, missionKey, key)
}
function LoadTimerHandleBJ(key, missionKey, table) {
    return LoadTimerHandle(table, missionKey, key)
}
function LoadTriggerHandleBJ(key, missionKey, table) {
    return LoadTriggerHandle(table, missionKey, key)
}
function LoadTriggerConditionHandleBJ(key, missionKey, table) {
    return LoadTriggerConditionHandle(table, missionKey, key)
}
function LoadTriggerActionHandleBJ(key, missionKey, table) {
    return LoadTriggerActionHandle(table, missionKey, key)
}
function LoadTriggerEventHandleBJ(key, missionKey, table) {
    return LoadTriggerEventHandle(table, missionKey, key)
}
function LoadForceHandleBJ(key, missionKey, table) {
    return LoadForceHandle(table, missionKey, key)
}
function LoadGroupHandleBJ(key, missionKey, table) {
    return LoadGroupHandle(table, missionKey, key)
}
function LoadLocationHandleBJ(key, missionKey, table) {
    return LoadLocationHandle(table, missionKey, key)
}
function LoadRectHandleBJ(key, missionKey, table) {
    return LoadRectHandle(table, missionKey, key)
}
function LoadBooleanExprHandleBJ(key, missionKey, table) {
    return LoadBooleanExprHandle(table, missionKey, key)
}
function LoadSoundHandleBJ(key, missionKey, table) {
    return LoadSoundHandle(table, missionKey, key)
}
function LoadEffectHandleBJ(key, missionKey, table) {
    return LoadEffectHandle(table, missionKey, key)
}
function LoadUnitPoolHandleBJ(key, missionKey, table) {
    return LoadUnitPoolHandle(table, missionKey, key)
}
function LoadItemPoolHandleBJ(key, missionKey, table) {
    return LoadItemPoolHandle(table, missionKey, key)
}
function LoadQuestHandleBJ(key, missionKey, table) {
    return LoadQuestHandle(table, missionKey, key)
}
function LoadQuestItemHandleBJ(key, missionKey, table) {
    return LoadQuestItemHandle(table, missionKey, key)
}
function LoadDefeatConditionHandleBJ(key, missionKey, table) {
    return LoadDefeatConditionHandle(table, missionKey, key)
}
function LoadTimerDialogHandleBJ(key, missionKey, table) {
    return LoadTimerDialogHandle(table, missionKey, key)
}
function LoadLeaderboardHandleBJ(key, missionKey, table) {
    return LoadLeaderboardHandle(table, missionKey, key)
}
function LoadMultiboardHandleBJ(key, missionKey, table) {
    return LoadMultiboardHandle(table, missionKey, key)
}
function LoadMultiboardItemHandleBJ(key, missionKey, table) {
    return LoadMultiboardItemHandle(table, missionKey, key)
}
function LoadTrackableHandleBJ(key, missionKey, table) {
    return LoadTrackableHandle(table, missionKey, key)
}
function LoadDialogHandleBJ(key, missionKey, table) {
    return LoadDialogHandle(table, missionKey, key)
}
function LoadButtonHandleBJ(key, missionKey, table) {
    return LoadButtonHandle(table, missionKey, key)
}
function LoadTextTagHandleBJ(key, missionKey, table) {
    return LoadTextTagHandle(table, missionKey, key)
}
function LoadLightningHandleBJ(key, missionKey, table) {
    return LoadLightningHandle(table, missionKey, key)
}
function LoadImageHandleBJ(key, missionKey, table) {
    return LoadImageHandle(table, missionKey, key)
}
function LoadUbersplatHandleBJ(key, missionKey, table) {
    return LoadUbersplatHandle(table, missionKey, key)
}
function LoadRegionHandleBJ(key, missionKey, table) {
    return LoadRegionHandle(table, missionKey, key)
}
function LoadFogStateHandleBJ(key, missionKey, table) {
    return LoadFogStateHandle(table, missionKey, key)
}
function LoadFogModifierHandleBJ(key, missionKey, table) {
    return LoadFogModifierHandle(table, missionKey, key)
}
function LoadHashtableHandleBJ(key, missionKey, table) {
    return LoadHashtableHandle(table, missionKey, key)
}
function RestoreUnitLocFacingAngleBJ(key, missionKey, cache, forWhichPlayer, loc, facing) {
    bj_lastLoadedUnit = RestoreUnit(cache, missionKey, key, forWhichPlayer, GetLocationX(loc), GetLocationY(loc), facing)
    return bj_lastLoadedUnit
}
function RestoreUnitLocFacingPointBJ(key, missionKey, cache, forWhichPlayer, loc, lookAt) {
    return RestoreUnitLocFacingAngleBJ(key, missionKey, cache, forWhichPlayer, loc, AngleBetweenPoints(loc, lookAt))
}
function GetLastRestoredUnitBJ() {
    return bj_lastLoadedUnit
}
function FlushGameCacheBJ(cache) {
    FlushGameCache(cache)
}
function FlushStoredMissionBJ(missionKey, cache) {
    FlushStoredMission(cache, missionKey)
}
function FlushParentHashtableBJ(table) {
    FlushParentHashtable(table)
}
function FlushChildHashtableBJ(missionKey, table) {
    FlushChildHashtable(table, missionKey)
}
function HaveStoredValue(key, valueType, missionKey, cache) {
    if ((valueType == bj_GAMECACHE_BOOLEAN)) {
        return HaveStoredBoolean(cache, missionKey, key)
    }
    else {
        if ((valueType == bj_GAMECACHE_INTEGER)) {
            return HaveStoredInteger(cache, missionKey, key)
        }
        else {
            if ((valueType == bj_GAMECACHE_REAL)) {
                return HaveStoredReal(cache, missionKey, key)
            }
            else {
                if ((valueType == bj_GAMECACHE_UNIT)) {
                    return HaveStoredUnit(cache, missionKey, key)
                }
                else {
                    if ((valueType == bj_GAMECACHE_STRING)) {
                        return HaveStoredString(cache, missionKey, key)
                    }
                    else {
                        return false
                    }
                }
            }
        }
    }
}
function HaveSavedValue(key, valueType, missionKey, table) {
    if ((valueType == bj_HASHTABLE_BOOLEAN)) {
        return HaveSavedBoolean(table, missionKey, key)
    }
    else {
        if ((valueType == bj_HASHTABLE_INTEGER)) {
            return HaveSavedInteger(table, missionKey, key)
        }
        else {
            if ((valueType == bj_HASHTABLE_REAL)) {
                return HaveSavedReal(table, missionKey, key)
            }
            else {
                if ((valueType == bj_HASHTABLE_STRING)) {
                    return HaveSavedString(table, missionKey, key)
                }
                else {
                    if ((valueType == bj_HASHTABLE_HANDLE)) {
                        return HaveSavedHandle(table, missionKey, key)
                    }
                    else {
                        return false
                    }
                }
            }
        }
    }
}
function ShowCustomCampaignButton(show, whichButton) {
    SetCustomCampaignButtonVisible(whichButton - 1, show)
}
function IsCustomCampaignButtonVisibile(whichButton) {
    return GetCustomCampaignButtonVisible(whichButton - 1)
}
function SaveGameCheckPointBJ(mapSaveName, doCheckpointHint) {
    SaveGameCheckpoint(mapSaveName, doCheckpointHint)
}
function LoadGameBJ(loadFileName, doScoreScreen) {
    LoadGame(loadFileName, doScoreScreen)
}
function SaveAndChangeLevelBJ(saveFileName, newLevel, doScoreScreen) {
    SaveGame(saveFileName)
    ChangeLevel(newLevel, doScoreScreen)
}
function SaveAndLoadGameBJ(saveFileName, loadFileName, doScoreScreen) {
    SaveGame(saveFileName)
    LoadGame(loadFileName, doScoreScreen)
}
function RenameSaveDirectoryBJ(sourceDirName, destDirName) {
    return RenameSaveDirectory(sourceDirName, destDirName)
}
function RemoveSaveDirectoryBJ(sourceDirName) {
    return RemoveSaveDirectory(sourceDirName)
}
function CopySaveGameBJ(sourceSaveName, destSaveName) {
    return CopySaveGame(sourceSaveName, destSaveName)
}
function GetPlayerStartLocationX(whichPlayer) {
    return GetStartLocationX(GetPlayerStartLocation(whichPlayer))
}
function GetPlayerStartLocationY(whichPlayer) {
    return GetStartLocationY(GetPlayerStartLocation(whichPlayer))
}
function GetPlayerStartLocationLoc(whichPlayer) {
    return GetStartLocationLoc(GetPlayerStartLocation(whichPlayer))
}
function GetRectCenter(whichRect) {
    return Location(GetRectCenterX(whichRect), GetRectCenterY(whichRect))
}
function IsPlayerSlotState(whichPlayer, whichState) {
    return GetPlayerSlotState(whichPlayer) == whichState
}
function GetFadeFromSeconds(seconds) {
    if ((seconds != 0)) {
        return R2I(128 / R2I(seconds))
    }
    return 10000
}
function GetFadeFromSecondsAsReal(seconds) {
    if ((seconds != 0)) {
        return 128.00 / seconds
    }
    return 10000.00
}
function AdjustPlayerStateSimpleBJ(whichPlayer, whichPlayerState, delta) {
    SetPlayerState(whichPlayer, whichPlayerState, GetPlayerState(whichPlayer, whichPlayerState) + delta)
}
function AdjustPlayerStateBJ(delta, whichPlayer, whichPlayerState) {
    if ((delta > 0)) {
        if ((whichPlayerState == PLAYER_STATE_RESOURCE_GOLD)) {
            AdjustPlayerStateSimpleBJ(whichPlayer, PLAYER_STATE_GOLD_GATHERED, delta)
        }
        else {
            if ((whichPlayerState == PLAYER_STATE_RESOURCE_LUMBER)) {
                AdjustPlayerStateSimpleBJ(whichPlayer, PLAYER_STATE_LUMBER_GATHERED, delta)
            }
        }
    }
    AdjustPlayerStateSimpleBJ(whichPlayer, whichPlayerState, delta)
}
function SetPlayerStateBJ(whichPlayer, whichPlayerState, value) {
    var oldValue = GetPlayerState(whichPlayer, whichPlayerState);
    AdjustPlayerStateBJ(value - oldValue, whichPlayer, whichPlayerState)
}
function SetPlayerFlagBJ(whichPlayerFlag, flag, whichPlayer) {
    SetPlayerState(whichPlayer, whichPlayerFlag, IntegerTertiaryOp(flag, 1, 0))
}
function SetPlayerTaxRateBJ(rate, whichResource, sourcePlayer, otherPlayer) {
    SetPlayerTaxRate(sourcePlayer, otherPlayer, whichResource, rate)
}
function GetPlayerTaxRateBJ(whichResource, sourcePlayer, otherPlayer) {
    return GetPlayerTaxRate(sourcePlayer, otherPlayer, whichResource)
}
function IsPlayerFlagSetBJ(whichPlayerFlag, whichPlayer) {
    return GetPlayerState(whichPlayer, whichPlayerFlag) == 1
}
function AddResourceAmountBJ(delta, whichUnit) {
    AddResourceAmount(whichUnit, delta)
}
function GetConvertedPlayerId(whichPlayer) {
    return GetPlayerId(whichPlayer) + 1
}
function ConvertedPlayer(convertedPlayerId) {
    return Player(convertedPlayerId - 1)
}
function GetRectWidthBJ(r) {
    return GetRectMaxX(r) - GetRectMinX(r)
}
function GetRectHeightBJ(r) {
    return GetRectMaxY(r) - GetRectMinY(r)
}
function BlightGoldMineForPlayerBJ(goldMine, whichPlayer) {
    var mineX;
    var mineY;
    var mineGold;
    var newMine;
    if (GetUnitTypeId(goldMine) != 'ngol') {
        return null
    }
    mineX = GetUnitX(goldMine)
    mineY = GetUnitY(goldMine)
    mineGold = GetResourceAmount(goldMine)
    RemoveUnit(goldMine)
    newMine = CreateBlightedGoldmine(whichPlayer, mineX, mineY, bj_UNIT_FACING)
    SetResourceAmount(newMine, mineGold)
    return newMine
}
function BlightGoldMineForPlayer(goldMine, whichPlayer) {
    bj_lastHauntedGoldMine = BlightGoldMineForPlayerBJ(goldMine, whichPlayer)
    return bj_lastHauntedGoldMine
}
function GetLastHauntedGoldMine() {
    return bj_lastHauntedGoldMine
}
function IsPointBlightedBJ(where) {
    return IsPointBlighted(GetLocationX(where), GetLocationY(where))
}
function SetPlayerColorBJEnum() {
    SetUnitColor(GetEnumUnit(), bj_setPlayerTargetColor)
}
function SetPlayerColorBJ(whichPlayer, color, changeExisting) {
    var g;
    SetPlayerColor(whichPlayer, color)
    if (changeExisting) {
        bj_setPlayerTargetColor = color
        g = CreateGroup()
        GroupEnumUnitsOfPlayer(g, whichPlayer, null)
        ForGroup(g, SetPlayerColorBJEnum)
        DestroyGroup(g)
    }
}
function SetPlayerUnitAvailableBJ(unitId, allowed, whichPlayer) {
    if (allowed) {
        SetPlayerTechMaxAllowed(whichPlayer, unitId, -1)
    }
    else {
        SetPlayerTechMaxAllowed(whichPlayer, unitId, 0)
    }
}
function LockGameSpeedBJ() {
    SetMapFlag(MAP_LOCK_SPEED, true)
}
function UnlockGameSpeedBJ() {
    SetMapFlag(MAP_LOCK_SPEED, false)
}
function IssueTargetOrderBJ(whichUnit, order, targetWidget) {
    return IssueTargetOrder(whichUnit, order, targetWidget)
}
function IssuePointOrderLocBJ(whichUnit, order, whichLocation) {
    return IssuePointOrderLoc(whichUnit, order, whichLocation)
}
function IssueTargetDestructableOrder(whichUnit, order, targetWidget) {
    return IssueTargetOrder(whichUnit, order, targetWidget)
}
function IssueTargetItemOrder(whichUnit, order, targetWidget) {
    return IssueTargetOrder(whichUnit, order, targetWidget)
}
function IssueImmediateOrderBJ(whichUnit, order) {
    return IssueImmediateOrder(whichUnit, order)
}
function GroupTargetOrderBJ(whichGroup, order, targetWidget) {
    return GroupTargetOrder(whichGroup, order, targetWidget)
}
function GroupPointOrderLocBJ(whichGroup, order, whichLocation) {
    return GroupPointOrderLoc(whichGroup, order, whichLocation)
}
function GroupImmediateOrderBJ(whichGroup, order) {
    return GroupImmediateOrder(whichGroup, order)
}
function GroupTargetDestructableOrder(whichGroup, order, targetWidget) {
    return GroupTargetOrder(whichGroup, order, targetWidget)
}
function GroupTargetItemOrder(whichGroup, order, targetWidget) {
    return GroupTargetOrder(whichGroup, order, targetWidget)
}
function GetDyingDestructable() {
    return GetTriggerDestructable()
}
function SetUnitRallyPoint(whichUnit, targPos) {
    IssuePointOrderLocBJ(whichUnit, "setrally", targPos)
}
function SetUnitRallyUnit(whichUnit, targUnit) {
    IssueTargetOrder(whichUnit, "setrally", targUnit)
}
function SetUnitRallyDestructable(whichUnit, targDest) {
    IssueTargetOrder(whichUnit, "setrally", targDest)
}
function SaveDyingWidget() {
    bj_lastDyingWidget = GetTriggerWidget()
}
function SetBlightRectBJ(addBlight, whichPlayer, r) {
    SetBlightRect(whichPlayer, r, addBlight)
}
function SetBlightRadiusLocBJ(addBlight, whichPlayer, loc, radius) {
    SetBlightLoc(whichPlayer, loc, radius, addBlight)
}
function GetAbilityName(abilcode) {
    return GetObjectName(abilcode)
}
function MeleeStartingVisibility() {
    SetFloatGameState(GAME_STATE_TIME_OF_DAY, bj_MELEE_STARTING_TOD)
}
function MeleeStartingResources() {
    var index;
    var indexPlayer;
    var v;
    var startingGold;
    var startingLumber;
    v = VersionGet()
    if ((v == VERSION_REIGN_OF_CHAOS)) {
        startingGold = bj_MELEE_STARTING_GOLD_V0
        startingLumber = bj_MELEE_STARTING_LUMBER_V0
    }
    else {
        startingGold = bj_MELEE_STARTING_GOLD_V1
        startingLumber = bj_MELEE_STARTING_LUMBER_V1
    }
    index = 0
    do {
        indexPlayer = Player(index)
        if ((GetPlayerSlotState(indexPlayer) == PLAYER_SLOT_STATE_PLAYING)) {
            SetPlayerState(indexPlayer, PLAYER_STATE_RESOURCE_GOLD, startingGold)
            SetPlayerState(indexPlayer, PLAYER_STATE_RESOURCE_LUMBER, startingLumber)
        }
        index = index + 1
        if (index == bj_MAX_PLAYERS) { break; }
    } while (true)
}
function ReducePlayerTechMaxAllowed(whichPlayer, techId, limit) {
    var oldMax = GetPlayerTechMaxAllowed(whichPlayer, techId);
    if ((oldMax < 0 || oldMax > limit)) {
        SetPlayerTechMaxAllowed(whichPlayer, techId, limit)
    }
}
function MeleeStartingHeroLimit() {
    var index;
    index = 0
    do {
        SetPlayerMaxHeroesAllowed(bj_MELEE_HERO_LIMIT, Player(index))
        ReducePlayerTechMaxAllowed(Player(index), 'Hamg', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Hmkg', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Hpal', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Hblm', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Obla', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Ofar', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Otch', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Oshd', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Edem', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Ekee', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Emoo', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Ewar', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Udea', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Udre', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Ulic', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Ucrl', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Npbm', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Nbrn', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Nngs', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Nplh', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Nbst', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Nalc', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Ntin', bj_MELEE_HERO_TYPE_LIMIT)
        ReducePlayerTechMaxAllowed(Player(index), 'Nfir', bj_MELEE_HERO_TYPE_LIMIT)
        index = index + 1
        if (index == bj_MAX_PLAYERS) { break; }
    } while (true)
}
function MeleeTrainedUnitIsHeroBJFilter() {
    return IsUnitType(GetFilterUnit(), UNIT_TYPE_HERO)
}
function MeleeGrantItemsToHero(whichUnit) {
    var owner = GetPlayerId(GetOwningPlayer(whichUnit));
    if ((bj_meleeTwinkedHeroes.getInt(owner) < bj_MELEE_MAX_TWINKED_HEROES)) {
        UnitAddItemById(whichUnit, 'stwp')
        bj_meleeTwinkedHeroes[owner] = bj_meleeTwinkedHeroes.getInt(owner) + 1
    }
}
function MeleeGrantItemsToTrainedHero() {
    MeleeGrantItemsToHero(GetTrainedUnit())
}
function MeleeGrantItemsToHiredHero() {
    MeleeGrantItemsToHero(GetSoldUnit())
}
function MeleeGrantHeroItems() {
    var index;
    var trig;
    index = 0
    do {
        bj_meleeTwinkedHeroes[index] = 0
        index = index + 1
        if (index == bj_MAX_PLAYER_SLOTS) { break; }
    } while (true)
    index = 0
    do {
        trig = CreateTrigger()
        TriggerRegisterPlayerUnitEvent(trig, Player(index), EVENT_PLAYER_UNIT_TRAIN_FINISH, filterMeleeTrainedUnitIsHeroBJ)
        TriggerAddAction(trig, MeleeGrantItemsToTrainedHero)
        index = index + 1
        if (index == bj_MAX_PLAYERS) { break; }
    } while (true)
    trig = CreateTrigger()
    TriggerRegisterPlayerUnitEvent(trig, Player(PLAYER_NEUTRAL_PASSIVE), EVENT_PLAYER_UNIT_SELL, filterMeleeTrainedUnitIsHeroBJ)
    TriggerAddAction(trig, MeleeGrantItemsToHiredHero)
    bj_meleeGrantHeroItems = true
}
function MeleeClearExcessUnit() {
    var theUnit = GetEnumUnit();
    var owner = GetPlayerId(GetOwningPlayer(theUnit));
    if ((owner == PLAYER_NEUTRAL_AGGRESSIVE)) {
        RemoveUnit(GetEnumUnit())
    }
    else {
        if ((owner == PLAYER_NEUTRAL_PASSIVE)) {
            if (!IsUnitType(theUnit, UNIT_TYPE_STRUCTURE)) {
                RemoveUnit(GetEnumUnit())
            }
        }
    }
}
function MeleeClearNearbyUnits(x, y, range) {
    var nearbyUnits;
    nearbyUnits = CreateGroup()
    GroupEnumUnitsInRange(nearbyUnits, x, y, range, null)
    ForGroup(nearbyUnits, MeleeClearExcessUnit)
    DestroyGroup(nearbyUnits)
}
function MeleeClearExcessUnits() {
    var index;
    var locX;
    var locY;
    var indexPlayer;
    index = 0
    do {
        indexPlayer = Player(index)
        if ((GetPlayerSlotState(indexPlayer) == PLAYER_SLOT_STATE_PLAYING)) {
            locX = GetStartLocationX(GetPlayerStartLocation(indexPlayer))
            locY = GetStartLocationY(GetPlayerStartLocation(indexPlayer))
            MeleeClearNearbyUnits(locX, locY, bj_MELEE_CLEAR_UNITS_RADIUS)
        }
        index = index + 1
        if (index == bj_MAX_PLAYERS) { break; }
    } while (true)
}
function MeleeEnumFindNearestMine() {
    var enumUnit = GetEnumUnit();
    var dist;
    var unitLoc;
    if ((GetUnitTypeId(enumUnit) == 'ngol')) {
        unitLoc = GetUnitLoc(enumUnit)
        dist = DistanceBetweenPoints(unitLoc, bj_meleeNearestMineToLoc)
        RemoveLocation(unitLoc)
        if ((bj_meleeNearestMineDist < 0) || (dist < bj_meleeNearestMineDist)) {
            bj_meleeNearestMine = enumUnit
            bj_meleeNearestMineDist = dist
        }
    }
}
function MeleeFindNearestMine(src, range) {
    var nearbyMines;
    bj_meleeNearestMine = null
    bj_meleeNearestMineDist = -1
    bj_meleeNearestMineToLoc = src
    nearbyMines = CreateGroup()
    GroupEnumUnitsInRangeOfLoc(nearbyMines, src, range, null)
    ForGroup(nearbyMines, MeleeEnumFindNearestMine)
    DestroyGroup(nearbyMines)
    return bj_meleeNearestMine
}
function MeleeRandomHeroLoc(p, id1, id2, id3, id4, loc) {
    var hero = null;
    var roll;
    var pick;
    var v;
    v = VersionGet()
    if ((v == VERSION_REIGN_OF_CHAOS)) {
        roll = GetRandomInt(1, 3)
    }
    else {
        roll = GetRandomInt(1, 4)
    }
    if (roll == 1) {
        pick = id1
    }
    else {
        if (roll == 2) {
            pick = id2
        }
        else {
            if (roll == 3) {
                pick = id3
            }
            else {
                if (roll == 4) {
                    pick = id4
                }
                else {
                    pick = id1
                }
            }
        }
    }
    hero = CreateUnitAtLoc(p, pick, loc, bj_UNIT_FACING)
    if (bj_meleeGrantHeroItems) {
        MeleeGrantItemsToHero(hero)
    }
    return hero
}
function MeleeGetProjectedLoc(src, targ, distance, deltaAngle) {
    var srcX = GetLocationX(src);
    var srcY = GetLocationY(src);
    var direction = Atan2(GetLocationY(targ) - srcY, GetLocationX(targ) - srcX) + deltaAngle;
    return Location(srcX + distance * Cos(direction), srcY + distance * Sin(direction))
}
function MeleeGetNearestValueWithin(val, minVal, maxVal) {
    if ((val < minVal)) {
        return minVal
    }
    else {
        if ((val > maxVal)) {
            return maxVal
        }
        else {
            return val
        }
    }
}
function MeleeGetLocWithinRect(src, r) {
    var withinX = MeleeGetNearestValueWithin(GetLocationX(src), GetRectMinX(r), GetRectMaxX(r));
    var withinY = MeleeGetNearestValueWithin(GetLocationY(src), GetRectMinY(r), GetRectMaxY(r));
    return Location(withinX, withinY)
}
function MeleeStartingUnitsHuman(whichPlayer, startLoc, doHeroes, doCamera, doPreload) {
    var useRandomHero = IsMapFlagSet(MAP_RANDOM_HERO);
    var unitSpacing = 64.00;
    var nearestMine;
    var nearMineLoc;
    var heroLoc;
    var peonX;
    var peonY;
    var townHall = null;
    if ((doPreload)) {
        Preloader("scripts\\HumanMelee.pld")
    }
    nearestMine = MeleeFindNearestMine(startLoc, bj_MELEE_MINE_SEARCH_RADIUS)
    if ((nearestMine != null)) {
        townHall = CreateUnitAtLoc(whichPlayer, 'htow', startLoc, bj_UNIT_FACING)
        nearMineLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 320, 0)
        peonX = GetLocationX(nearMineLoc)
        peonY = GetLocationY(nearMineLoc)
        CreateUnit(whichPlayer, 'hpea', peonX + 0.00 * unitSpacing, peonY + 1.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'hpea', peonX + 1.00 * unitSpacing, peonY + 0.15 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'hpea', peonX - 1.00 * unitSpacing, peonY + 0.15 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'hpea', peonX + 0.60 * unitSpacing, peonY - 1.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'hpea', peonX - 0.60 * unitSpacing, peonY - 1.00 * unitSpacing, bj_UNIT_FACING)
        heroLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 384, 45)
    }
    else {
        townHall = CreateUnitAtLoc(whichPlayer, 'htow', startLoc, bj_UNIT_FACING)
        peonX = GetLocationX(startLoc)
        peonY = GetLocationY(startLoc) - 224.00
        CreateUnit(whichPlayer, 'hpea', peonX + 2.00 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'hpea', peonX + 1.00 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'hpea', peonX + 0.00 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'hpea', peonX - 1.00 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'hpea', peonX - 2.00 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        heroLoc = Location(peonX, peonY - 2.00 * unitSpacing)
    }
    if ((townHall != null)) {
        UnitAddAbilityBJ('Amic', townHall)
        UnitMakeAbilityPermanentBJ(true, 'Amic', townHall)
    }
    if ((doHeroes)) {
        if (useRandomHero) {
            MeleeRandomHeroLoc(whichPlayer, 'Hamg', 'Hmkg', 'Hpal', 'Hblm', heroLoc)
        }
        else {
            SetPlayerState(whichPlayer, PLAYER_STATE_RESOURCE_HERO_TOKENS, bj_MELEE_STARTING_HERO_TOKENS)
        }
    }
    if ((doCamera)) {
        SetCameraPositionForPlayer(whichPlayer, peonX, peonY)
        SetCameraQuickPositionForPlayer(whichPlayer, peonX, peonY)
    }
}
function MeleeStartingUnitsOrc(whichPlayer, startLoc, doHeroes, doCamera, doPreload) {
    var useRandomHero = IsMapFlagSet(MAP_RANDOM_HERO);
    var unitSpacing = 64.00;
    var nearestMine;
    var nearMineLoc;
    var heroLoc;
    var peonX;
    var peonY;
    if ((doPreload)) {
        Preloader("scripts\\OrcMelee.pld")
    }
    nearestMine = MeleeFindNearestMine(startLoc, bj_MELEE_MINE_SEARCH_RADIUS)
    if ((nearestMine != null)) {
        CreateUnitAtLoc(whichPlayer, 'ogre', startLoc, bj_UNIT_FACING)
        nearMineLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 320, 0)
        peonX = GetLocationX(nearMineLoc)
        peonY = GetLocationY(nearMineLoc)
        CreateUnit(whichPlayer, 'opeo', peonX + 0.00 * unitSpacing, peonY + 1.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'opeo', peonX + 1.00 * unitSpacing, peonY + 0.15 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'opeo', peonX - 1.00 * unitSpacing, peonY + 0.15 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'opeo', peonX + 0.60 * unitSpacing, peonY - 1.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'opeo', peonX - 0.60 * unitSpacing, peonY - 1.00 * unitSpacing, bj_UNIT_FACING)
        heroLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 384, 45)
    }
    else {
        CreateUnitAtLoc(whichPlayer, 'ogre', startLoc, bj_UNIT_FACING)
        peonX = GetLocationX(startLoc)
        peonY = GetLocationY(startLoc) - 224.00
        CreateUnit(whichPlayer, 'opeo', peonX + 2.00 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'opeo', peonX + 1.00 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'opeo', peonX + 0.00 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'opeo', peonX - 1.00 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'opeo', peonX - 2.00 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        heroLoc = Location(peonX, peonY - 2.00 * unitSpacing)
    }
    if ((doHeroes)) {
        if (useRandomHero) {
            MeleeRandomHeroLoc(whichPlayer, 'Obla', 'Ofar', 'Otch', 'Oshd', heroLoc)
        }
        else {
            SetPlayerState(whichPlayer, PLAYER_STATE_RESOURCE_HERO_TOKENS, bj_MELEE_STARTING_HERO_TOKENS)
        }
    }
    if ((doCamera)) {
        SetCameraPositionForPlayer(whichPlayer, peonX, peonY)
        SetCameraQuickPositionForPlayer(whichPlayer, peonX, peonY)
    }
}
function MeleeStartingUnitsUndead(whichPlayer, startLoc, doHeroes, doCamera, doPreload) {
    var useRandomHero = IsMapFlagSet(MAP_RANDOM_HERO);
    var unitSpacing = 64.00;
    var nearestMine;
    var nearMineLoc;
    var nearTownLoc;
    var heroLoc;
    var peonX;
    var peonY;
    var ghoulX;
    var ghoulY;
    if ((doPreload)) {
        Preloader("scripts\\UndeadMelee.pld")
    }
    nearestMine = MeleeFindNearestMine(startLoc, bj_MELEE_MINE_SEARCH_RADIUS)
    if ((nearestMine != null)) {
        CreateUnitAtLoc(whichPlayer, 'unpl', startLoc, bj_UNIT_FACING)
        nearestMine = BlightGoldMineForPlayerBJ(nearestMine, whichPlayer)
        nearTownLoc = MeleeGetProjectedLoc(startLoc, GetUnitLoc(nearestMine), 288, 0)
        ghoulX = GetLocationX(nearTownLoc)
        ghoulY = GetLocationY(nearTownLoc)
        bj_ghoul[GetPlayerId(whichPlayer)] = CreateUnit(whichPlayer, 'ugho', ghoulX + 0.00 * unitSpacing, ghoulY + 0.00 * unitSpacing, bj_UNIT_FACING)
        nearMineLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 320, 0)
        peonX = GetLocationX(nearMineLoc)
        peonY = GetLocationY(nearMineLoc)
        CreateUnit(whichPlayer, 'uaco', peonX + 0.00 * unitSpacing, peonY + 0.50 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'uaco', peonX + 0.65 * unitSpacing, peonY - 0.50 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'uaco', peonX - 0.65 * unitSpacing, peonY - 0.50 * unitSpacing, bj_UNIT_FACING)
        SetBlightLoc(whichPlayer, nearMineLoc, 768, true)
        heroLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 384, 45)
    }
    else {
        CreateUnitAtLoc(whichPlayer, 'unpl', startLoc, bj_UNIT_FACING)
        peonX = GetLocationX(startLoc)
        peonY = GetLocationY(startLoc) - 224.00
        CreateUnit(whichPlayer, 'uaco', peonX - 1.50 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'uaco', peonX - 0.50 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'uaco', peonX + 0.50 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'ugho', peonX + 1.50 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        SetBlightLoc(whichPlayer, startLoc, 768, true)
        heroLoc = Location(peonX, peonY - 2.00 * unitSpacing)
    }
    if ((doHeroes)) {
        if (useRandomHero) {
            MeleeRandomHeroLoc(whichPlayer, 'Udea', 'Udre', 'Ulic', 'Ucrl', heroLoc)
        }
        else {
            SetPlayerState(whichPlayer, PLAYER_STATE_RESOURCE_HERO_TOKENS, bj_MELEE_STARTING_HERO_TOKENS)
        }
    }
    if ((doCamera)) {
        SetCameraPositionForPlayer(whichPlayer, peonX, peonY)
        SetCameraQuickPositionForPlayer(whichPlayer, peonX, peonY)
    }
}
function MeleeStartingUnitsNightElf(whichPlayer, startLoc, doHeroes, doCamera, doPreload) {
    var useRandomHero = IsMapFlagSet(MAP_RANDOM_HERO);
    var unitSpacing = 64.00;
    var minTreeDist = 3.50 * bj_CELLWIDTH;
    var minWispDist = 1.75 * bj_CELLWIDTH;
    var nearestMine;
    var nearMineLoc;
    var wispLoc;
    var heroLoc;
    var peonX;
    var peonY;
    var tree;
    if ((doPreload)) {
        Preloader("scripts\\NightElfMelee.pld")
    }
    nearestMine = MeleeFindNearestMine(startLoc, bj_MELEE_MINE_SEARCH_RADIUS)
    if ((nearestMine != null)) {
        nearMineLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 650, 0)
        nearMineLoc = MeleeGetLocWithinRect(nearMineLoc, GetRectFromCircleBJ(GetUnitLoc(nearestMine), minTreeDist))
        tree = CreateUnitAtLoc(whichPlayer, 'etol', nearMineLoc, bj_UNIT_FACING)
        IssueTargetOrder(tree, "entangleinstant", nearestMine)
        wispLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 320, 0)
        wispLoc = MeleeGetLocWithinRect(wispLoc, GetRectFromCircleBJ(GetUnitLoc(nearestMine), minWispDist))
        peonX = GetLocationX(wispLoc)
        peonY = GetLocationY(wispLoc)
        CreateUnit(whichPlayer, 'ewsp', peonX + 0.00 * unitSpacing, peonY + 1.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'ewsp', peonX + 1.00 * unitSpacing, peonY + 0.15 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'ewsp', peonX - 1.00 * unitSpacing, peonY + 0.15 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'ewsp', peonX + 0.58 * unitSpacing, peonY - 1.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'ewsp', peonX - 0.58 * unitSpacing, peonY - 1.00 * unitSpacing, bj_UNIT_FACING)
        heroLoc = MeleeGetProjectedLoc(GetUnitLoc(nearestMine), startLoc, 384, 45)
    }
    else {
        CreateUnitAtLoc(whichPlayer, 'etol', startLoc, bj_UNIT_FACING)
        peonX = GetLocationX(startLoc)
        peonY = GetLocationY(startLoc) - 224.00
        CreateUnit(whichPlayer, 'ewsp', peonX - 2.00 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'ewsp', peonX - 1.00 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'ewsp', peonX + 0.00 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'ewsp', peonX + 1.00 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        CreateUnit(whichPlayer, 'ewsp', peonX + 2.00 * unitSpacing, peonY + 0.00 * unitSpacing, bj_UNIT_FACING)
        heroLoc = Location(peonX, peonY - 2.00 * unitSpacing)
    }
    if ((doHeroes)) {
        if (useRandomHero) {
            MeleeRandomHeroLoc(whichPlayer, 'Edem', 'Ekee', 'Emoo', 'Ewar', heroLoc)
        }
        else {
            SetPlayerState(whichPlayer, PLAYER_STATE_RESOURCE_HERO_TOKENS, bj_MELEE_STARTING_HERO_TOKENS)
        }
    }
    if ((doCamera)) {
        SetCameraPositionForPlayer(whichPlayer, peonX, peonY)
        SetCameraQuickPositionForPlayer(whichPlayer, peonX, peonY)
    }
}
function MeleeStartingUnitsUnknownRace(whichPlayer, startLoc, doHeroes, doCamera, doPreload) {
    var index;
    if ((doPreload)) {
    }
    index = 0
    do {
        CreateUnit(whichPlayer, 'nshe', GetLocationX(startLoc) + GetRandomReal(-256, 256), GetLocationY(startLoc) + GetRandomReal(-256, 256), GetRandomReal(0, 360))
        index = index + 1
        if (index == 12) { break; }
    } while (true)
    if ((doHeroes)) {
        SetPlayerState(whichPlayer, PLAYER_STATE_RESOURCE_HERO_TOKENS, bj_MELEE_STARTING_HERO_TOKENS)
    }
    if ((doCamera)) {
        SetCameraPositionLocForPlayer(whichPlayer, startLoc)
        SetCameraQuickPositionLocForPlayer(whichPlayer, startLoc)
    }
}
function MeleeStartingUnits() {
    var index;
    var indexPlayer;
    var indexStartLoc;
    var indexRace;
    Preloader("scripts\\SharedMelee.pld")
    index = 0
    do {
        indexPlayer = Player(index)
        if ((GetPlayerSlotState(indexPlayer) == PLAYER_SLOT_STATE_PLAYING)) {
            indexStartLoc = GetStartLocationLoc(GetPlayerStartLocation(indexPlayer))
            indexRace = GetPlayerRace(indexPlayer)
            if ((indexRace == RACE_HUMAN)) {
                MeleeStartingUnitsHuman(indexPlayer, indexStartLoc, true, true, true)
            }
            else {
                if ((indexRace == RACE_ORC)) {
                    MeleeStartingUnitsOrc(indexPlayer, indexStartLoc, true, true, true)
                }
                else {
                    if ((indexRace == RACE_UNDEAD)) {
                        MeleeStartingUnitsUndead(indexPlayer, indexStartLoc, true, true, true)
                    }
                    else {
                        if ((indexRace == RACE_NIGHTELF)) {
                            MeleeStartingUnitsNightElf(indexPlayer, indexStartLoc, true, true, true)
                        }
                        else {
                            MeleeStartingUnitsUnknownRace(indexPlayer, indexStartLoc, true, true, true)
                        }
                    }
                }
            }
        }
        index = index + 1
        if (index == bj_MAX_PLAYERS) { break; }
    } while (true)
}
function MeleeStartingUnitsForPlayer(whichRace, whichPlayer, loc, doHeroes) {
    if ((whichRace == RACE_HUMAN)) {
        MeleeStartingUnitsHuman(whichPlayer, loc, doHeroes, false, false)
    }
    else {
        if ((whichRace == RACE_ORC)) {
            MeleeStartingUnitsOrc(whichPlayer, loc, doHeroes, false, false)
        }
        else {
            if ((whichRace == RACE_UNDEAD)) {
                MeleeStartingUnitsUndead(whichPlayer, loc, doHeroes, false, false)
            }
            else {
                if ((whichRace == RACE_NIGHTELF)) {
                    MeleeStartingUnitsNightElf(whichPlayer, loc, doHeroes, false, false)
                }
                else {
                }
            }
        }
    }
}
function PickMeleeAI(num, s1, s2, s3) {
    var pick;
    if (GetAIDifficulty(num) == AI_DIFFICULTY_NEWBIE) {
        StartMeleeAI(num, s1)
        return
    }
    if (s2 == null) {
        pick = 1
    }
    else {
        if (s3 == null) {
            pick = GetRandomInt(1, 2)
        }
        else {
            pick = GetRandomInt(1, 3)
        }
    }
    if (pick == 1) {
        StartMeleeAI(num, s1)
    }
    else {
        if (pick == 2) {
            StartMeleeAI(num, s2)
        }
        else {
            StartMeleeAI(num, s3)
        }
    }
}
function MeleeStartingAI() {
    var index;
    var indexPlayer;
    var indexRace;
    index = 0
    do {
        indexPlayer = Player(index)
        if ((GetPlayerSlotState(indexPlayer) == PLAYER_SLOT_STATE_PLAYING)) {
            indexRace = GetPlayerRace(indexPlayer)
            if ((GetPlayerController(indexPlayer) == MAP_CONTROL_COMPUTER)) {
                if ((indexRace == RACE_HUMAN)) {
                    PickMeleeAI(indexPlayer, "human.ai", null, null)
                }
                else {
                    if ((indexRace == RACE_ORC)) {
                        PickMeleeAI(indexPlayer, "orc.ai", null, null)
                    }
                    else {
                        if ((indexRace == RACE_UNDEAD)) {
                            PickMeleeAI(indexPlayer, "undead.ai", null, null)
                            RecycleGuardPosition(bj_ghoul[index])
                        }
                        else {
                            if ((indexRace == RACE_NIGHTELF)) {
                                PickMeleeAI(indexPlayer, "elf.ai", null, null)
                            }
                            else {
                            }
                        }
                    }
                }
                ShareEverythingWithTeamAI(indexPlayer)
            }
        }
        index = index + 1
        if (index == bj_MAX_PLAYERS) { break; }
    } while (true)
}
function LockGuardPosition(targ) {
    SetUnitCreepGuard(targ, true)
}
function MeleePlayerIsOpponent(playerIndex, opponentIndex) {
    var thePlayer = Player(playerIndex);
    var theOpponent = Player(opponentIndex);
    if ((playerIndex == opponentIndex)) {
        return false
    }
    if ((GetPlayerSlotState(theOpponent) != PLAYER_SLOT_STATE_PLAYING)) {
        return false
    }
    if ((bj_meleeDefeated[opponentIndex])) {
        return false
    }
    if (GetPlayerAlliance(thePlayer, theOpponent, ALLIANCE_PASSIVE)) {
        if (GetPlayerAlliance(theOpponent, thePlayer, ALLIANCE_PASSIVE)) {
            if ((GetPlayerState(thePlayer, PLAYER_STATE_ALLIED_VICTORY) == 1)) {
                if ((GetPlayerState(theOpponent, PLAYER_STATE_ALLIED_VICTORY) == 1)) {
                    return false
                }
            }
        }
    }
    return true
}
function MeleeGetAllyStructureCount(whichPlayer) {
    var playerIndex;
    var buildingCount;
    var indexPlayer;
    buildingCount = 0
    playerIndex = 0
    do {
        indexPlayer = Player(playerIndex)
        if ((PlayersAreCoAllied(whichPlayer, indexPlayer))) {
            buildingCount = buildingCount + GetPlayerStructureCount(indexPlayer, true)
        }
        playerIndex = playerIndex + 1
        if (playerIndex == bj_MAX_PLAYERS) { break; }
    } while (true)
    return buildingCount
}
function MeleeGetAllyCount(whichPlayer) {
    var playerIndex;
    var playerCount;
    var indexPlayer;
    playerCount = 0
    playerIndex = 0
    do {
        indexPlayer = Player(playerIndex)
        if (PlayersAreCoAllied(whichPlayer, indexPlayer) && !bj_meleeDefeated[playerIndex] && (whichPlayer != indexPlayer)) {
            playerCount = playerCount + 1
        }
        playerIndex = playerIndex + 1
        if (playerIndex == bj_MAX_PLAYERS) { break; }
    } while (true)
    return playerCount
}
function MeleeGetAllyKeyStructureCount(whichPlayer) {
    var playerIndex;
    var indexPlayer;
    var keyStructs;
    keyStructs = 0
    playerIndex = 0
    do {
        indexPlayer = Player(playerIndex)
        if ((PlayersAreCoAllied(whichPlayer, indexPlayer))) {
            keyStructs = keyStructs + BlzGetPlayerTownHallCount(indexPlayer)
        }
        playerIndex = playerIndex + 1
        if (playerIndex == bj_MAX_PLAYERS) { break; }
    } while (true)
    return keyStructs
}
function MeleeDoDrawEnum() {
    var thePlayer = GetEnumPlayer();
    CachePlayerHeroData(thePlayer)
    RemovePlayerPreserveUnitsBJ(thePlayer, PLAYER_GAME_RESULT_TIE, false)
}
function MeleeDoVictoryEnum() {
    var thePlayer = GetEnumPlayer();
    var playerIndex = GetPlayerId(thePlayer);
    if ((!bj_meleeVictoried[playerIndex])) {
        bj_meleeVictoried[playerIndex] = true
        CachePlayerHeroData(thePlayer)
        RemovePlayerPreserveUnitsBJ(thePlayer, PLAYER_GAME_RESULT_VICTORY, false)
    }
}
function MeleeDoDefeat(whichPlayer) {
    bj_meleeDefeated[GetPlayerId(whichPlayer)] = true
    RemovePlayerPreserveUnitsBJ(whichPlayer, PLAYER_GAME_RESULT_DEFEAT, false)
}
function MeleeDoDefeatEnum() {
    var thePlayer = GetEnumPlayer();
    CachePlayerHeroData(thePlayer)
    MakeUnitsPassiveForTeam(thePlayer)
    MeleeDoDefeat(thePlayer)
}
function MeleeDoLeave(whichPlayer) {
    if ((GetIntegerGameState(GAME_STATE_DISCONNECTED) != 0)) {
        GameOverDialogBJ(whichPlayer, true)
    }
    else {
        bj_meleeDefeated[GetPlayerId(whichPlayer)] = true
        RemovePlayerPreserveUnitsBJ(whichPlayer, PLAYER_GAME_RESULT_DEFEAT, true)
    }
}
function MeleeRemoveObservers() {
    var playerIndex;
    var indexPlayer;
    playerIndex = 0
    do {
        indexPlayer = Player(playerIndex)
        if ((IsPlayerObserver(indexPlayer))) {
            RemovePlayerPreserveUnitsBJ(indexPlayer, PLAYER_GAME_RESULT_NEUTRAL, false)
        }
        playerIndex = playerIndex + 1
        if (playerIndex == bj_MAX_PLAYERS) { break; }
    } while (true)
}
function MeleeCheckForVictors() {
    var playerIndex;
    var opponentIndex;
    var opponentlessPlayers = CreateForce();
    var gameOver = false;
    playerIndex = 0
    do {
        if ((!bj_meleeDefeated[playerIndex])) {
            opponentIndex = 0
            do {
                if (MeleePlayerIsOpponent(playerIndex, opponentIndex)) {
                    return CreateForce()
                }
                opponentIndex = opponentIndex + 1
                if (opponentIndex == bj_MAX_PLAYERS) { break; }
            } while (true)
            ForceAddPlayer(opponentlessPlayers, Player(playerIndex))
            gameOver = true
        }
        playerIndex = playerIndex + 1
        if (playerIndex == bj_MAX_PLAYERS) { break; }
    } while (true)
    bj_meleeGameOver = gameOver
    return opponentlessPlayers
}
function MeleeCheckForLosersAndVictors() {
    var playerIndex;
    var indexPlayer;
    var defeatedPlayers = CreateForce();
    var victoriousPlayers;
    var gameOver = false;
    if ((bj_meleeGameOver)) {
        return
    }
    if ((GetIntegerGameState(GAME_STATE_DISCONNECTED) != 0)) {
        bj_meleeGameOver = true
        return
    }
    playerIndex = 0
    do {
        indexPlayer = Player(playerIndex)
        if ((!bj_meleeDefeated[playerIndex] && !bj_meleeVictoried[playerIndex])) {
            if ((MeleeGetAllyStructureCount(indexPlayer) <= 0)) {
                ForceAddPlayer(defeatedPlayers, Player(playerIndex))
                bj_meleeDefeated[playerIndex] = true
            }
        }
        playerIndex = playerIndex + 1
        if (playerIndex == bj_MAX_PLAYERS) { break; }
    } while (true)
    victoriousPlayers = MeleeCheckForVictors()
    ForForce(defeatedPlayers, MeleeDoDefeatEnum)
    ForForce(victoriousPlayers, MeleeDoVictoryEnum)
    if ((bj_meleeGameOver)) {
        MeleeRemoveObservers()
    }
}
function MeleeGetCrippledWarningMessage(whichPlayer) {
    var r = GetPlayerRace(whichPlayer);
    if ((r == RACE_HUMAN)) {
        return GetLocalizedString("CRIPPLE_WARNING_HUMAN")
    }
    else {
        if ((r == RACE_ORC)) {
            return GetLocalizedString("CRIPPLE_WARNING_ORC")
        }
        else {
            if ((r == RACE_NIGHTELF)) {
                return GetLocalizedString("CRIPPLE_WARNING_NIGHTELF")
            }
            else {
                if ((r == RACE_UNDEAD)) {
                    return GetLocalizedString("CRIPPLE_WARNING_UNDEAD")
                }
                else {
                    return ""
                }
            }
        }
    }
}
function MeleeGetCrippledTimerMessage(whichPlayer) {
    var r = GetPlayerRace(whichPlayer);
    if ((r == RACE_HUMAN)) {
        return GetLocalizedString("CRIPPLE_TIMER_HUMAN")
    }
    else {
        if ((r == RACE_ORC)) {
            return GetLocalizedString("CRIPPLE_TIMER_ORC")
        }
        else {
            if ((r == RACE_NIGHTELF)) {
                return GetLocalizedString("CRIPPLE_TIMER_NIGHTELF")
            }
            else {
                if ((r == RACE_UNDEAD)) {
                    return GetLocalizedString("CRIPPLE_TIMER_UNDEAD")
                }
                else {
                    return ""
                }
            }
        }
    }
}
function MeleeGetCrippledRevealedMessage(whichPlayer) {
    return S2S(GetLocalizedString("CRIPPLE_REVEALING_PREFIX") + GetPlayerName(whichPlayer)) + GetLocalizedString("CRIPPLE_REVEALING_POSTFIX")
}
function MeleeExposePlayer(whichPlayer, expose) {
    var playerIndex;
    var indexPlayer;
    var toExposeTo = CreateForce();
    CripplePlayer(whichPlayer, toExposeTo, false)
    bj_playerIsExposed[GetPlayerId(whichPlayer)] = expose
    playerIndex = 0
    do {
        indexPlayer = Player(playerIndex)
        if ((!PlayersAreCoAllied(whichPlayer, indexPlayer))) {
            ForceAddPlayer(toExposeTo, indexPlayer)
        }
        playerIndex = playerIndex + 1
        if (playerIndex == bj_MAX_PLAYERS) { break; }
    } while (true)
    CripplePlayer(whichPlayer, toExposeTo, expose)
    DestroyForce(toExposeTo)
}
function MeleeExposeAllPlayers() {
    var playerIndex;
    var indexPlayer;
    var playerIndex2;
    var indexPlayer2;
    var toExposeTo = CreateForce();
    playerIndex = 0
    do {
        indexPlayer = Player(playerIndex)
        ForceClear(toExposeTo)
        CripplePlayer(indexPlayer, toExposeTo, false)
        playerIndex2 = 0
        do {
            indexPlayer2 = Player(playerIndex2)
            if (playerIndex != playerIndex2) {
                if ((!PlayersAreCoAllied(indexPlayer, indexPlayer2))) {
                    ForceAddPlayer(toExposeTo, indexPlayer2)
                }
            }
            playerIndex2 = playerIndex2 + 1
            if (playerIndex2 == bj_MAX_PLAYERS) { break; }
        } while (true)
        CripplePlayer(indexPlayer, toExposeTo, true)
        playerIndex = playerIndex + 1
        if (playerIndex == bj_MAX_PLAYERS) { break; }
    } while (true)
    DestroyForce(toExposeTo)
}
function MeleeCrippledPlayerTimeout() {
    var expiredTimer = GetExpiredTimer();
    var playerIndex;
    var exposedPlayer;
    playerIndex = 0
    do {
        if ((bj_crippledTimer[playerIndex] == expiredTimer)) {
            if (true) { break; }
        }
        playerIndex = playerIndex + 1
        if (playerIndex == bj_MAX_PLAYERS) { break; }
    } while (true)
    if ((playerIndex == bj_MAX_PLAYERS)) {
        return
    }
    exposedPlayer = Player(playerIndex)
    if ((GetLocalPlayer() == exposedPlayer)) {
        TimerDialogDisplay(bj_crippledTimerWindows[playerIndex], false)
    }
    DisplayTimedTextToPlayer(GetLocalPlayer(), 0, 0, bj_MELEE_CRIPPLE_MSG_DURATION, MeleeGetCrippledRevealedMessage(exposedPlayer))
    MeleeExposePlayer(exposedPlayer, true)
}
function MeleePlayerIsCrippled(whichPlayer) {
    var playerStructures = GetPlayerStructureCount(whichPlayer, true);
    var playerKeyStructures = BlzGetPlayerTownHallCount(whichPlayer);
    return (playerStructures > 0) && (playerKeyStructures <= 0)
}
function MeleeCheckForCrippledPlayers() {
    var playerIndex;
    var indexPlayer;
    var crippledPlayers = CreateForce();
    var isNowCrippled;
    var indexRace;
    if (bj_finishSoonAllExposed) {
        return
    }
    playerIndex = 0
    do {
        indexPlayer = Player(playerIndex)
        isNowCrippled = MeleePlayerIsCrippled(indexPlayer)
        if ((!bj_playerIsCrippled[playerIndex] && isNowCrippled)) {
            bj_playerIsCrippled[playerIndex] = true
            TimerStart(bj_crippledTimer[playerIndex], bj_MELEE_CRIPPLE_TIMEOUT, false, MeleeCrippledPlayerTimeout)
            if ((GetLocalPlayer() == indexPlayer)) {
                TimerDialogDisplay(bj_crippledTimerWindows[playerIndex], true)
                DisplayTimedTextToPlayer(indexPlayer, 0, 0, bj_MELEE_CRIPPLE_MSG_DURATION, MeleeGetCrippledWarningMessage(indexPlayer))
            }
        }
        else {
            if ((bj_playerIsCrippled[playerIndex] && !isNowCrippled)) {
                bj_playerIsCrippled[playerIndex] = false
                PauseTimer(bj_crippledTimer[playerIndex])
                if ((GetLocalPlayer() == indexPlayer)) {
                    TimerDialogDisplay(bj_crippledTimerWindows[playerIndex], false)
                    if ((MeleeGetAllyStructureCount(indexPlayer) > 0)) {
                        if ((bj_playerIsExposed[playerIndex])) {
                            DisplayTimedTextToPlayer(indexPlayer, 0, 0, bj_MELEE_CRIPPLE_MSG_DURATION, GetLocalizedString("CRIPPLE_UNREVEALED"))
                        }
                        else {
                            DisplayTimedTextToPlayer(indexPlayer, 0, 0, bj_MELEE_CRIPPLE_MSG_DURATION, GetLocalizedString("CRIPPLE_UNCRIPPLED"))
                        }
                    }
                }
                MeleeExposePlayer(indexPlayer, false)
            }
        }
        playerIndex = playerIndex + 1
        if (playerIndex == bj_MAX_PLAYERS) { break; }
    } while (true)
}
function MeleeCheckLostUnit(lostUnit) {
    var lostUnitOwner = GetOwningPlayer(lostUnit);
    if ((GetPlayerStructureCount(lostUnitOwner, true) <= 0)) {
        MeleeCheckForLosersAndVictors()
    }
    MeleeCheckForCrippledPlayers()
}
function MeleeCheckAddedUnit(addedUnit) {
    var addedUnitOwner = GetOwningPlayer(addedUnit);
    if ((bj_playerIsCrippled[GetPlayerId(addedUnitOwner)])) {
        MeleeCheckForCrippledPlayers()
    }
}
function MeleeTriggerActionConstructCancel() {
    MeleeCheckLostUnit(GetCancelledStructure())
}
function MeleeTriggerActionUnitDeath() {
    if ((IsUnitType(GetDyingUnit(), UNIT_TYPE_STRUCTURE))) {
        MeleeCheckLostUnit(GetDyingUnit())
    }
}
function MeleeTriggerActionUnitConstructionStart() {
    MeleeCheckAddedUnit(GetConstructingStructure())
}
function MeleeTriggerActionPlayerDefeated() {
    var thePlayer = GetTriggerPlayer();
    CachePlayerHeroData(thePlayer)
    if ((MeleeGetAllyCount(thePlayer) > 0)) {
        ShareEverythingWithTeam(thePlayer)
        if ((!bj_meleeDefeated[GetPlayerId(thePlayer)])) {
            MeleeDoDefeat(thePlayer)
        }
    }
    else {
        MakeUnitsPassiveForTeam(thePlayer)
        if ((!bj_meleeDefeated[GetPlayerId(thePlayer)])) {
            MeleeDoDefeat(thePlayer)
        }
    }
    MeleeCheckForLosersAndVictors()
}
function MeleeTriggerActionPlayerLeft() {
    var thePlayer = GetTriggerPlayer();
    if ((IsPlayerObserver(thePlayer))) {
        RemovePlayerPreserveUnitsBJ(thePlayer, PLAYER_GAME_RESULT_NEUTRAL, false)
        return
    }
    CachePlayerHeroData(thePlayer)
    if ((MeleeGetAllyCount(thePlayer) > 0)) {
        ShareEverythingWithTeam(thePlayer)
        MeleeDoLeave(thePlayer)
    }
    else {
        MakeUnitsPassiveForTeam(thePlayer)
        MeleeDoLeave(thePlayer)
    }
    MeleeCheckForLosersAndVictors()
}
function MeleeTriggerActionAllianceChange() {
    MeleeCheckForLosersAndVictors()
    MeleeCheckForCrippledPlayers()
}
function MeleeTriggerTournamentFinishSoon() {
    var playerIndex;
    var indexPlayer;
    var timeRemaining = GetTournamentFinishSoonTimeRemaining();
    if (!bj_finishSoonAllExposed) {
        bj_finishSoonAllExposed = true
        playerIndex = 0
        do {
            indexPlayer = Player(playerIndex)
            if (bj_playerIsCrippled[playerIndex]) {
                bj_playerIsCrippled[playerIndex] = false
                PauseTimer(bj_crippledTimer[playerIndex])
                if ((GetLocalPlayer() == indexPlayer)) {
                    TimerDialogDisplay(bj_crippledTimerWindows[playerIndex], false)
                }
            }
            playerIndex = playerIndex + 1
            if (playerIndex == bj_MAX_PLAYERS) { break; }
        } while (true)
        MeleeExposeAllPlayers()
    }
    TimerDialogDisplay(bj_finishSoonTimerDialog, true)
    TimerDialogSetRealTimeRemaining(bj_finishSoonTimerDialog, timeRemaining)
}
function MeleeWasUserPlayer(whichPlayer) {
    var slotState;
    if ((GetPlayerController(whichPlayer) != MAP_CONTROL_USER)) {
        return false
    }
    slotState = GetPlayerSlotState(whichPlayer)
    return (slotState == PLAYER_SLOT_STATE_PLAYING || slotState == PLAYER_SLOT_STATE_LEFT)
}
function MeleeTournamentFinishNowRuleA(multiplier) {
    var playerScore = [];
    var teamScore = [];
    var teamForce = [];
    var teamCount;
    var index;
    var indexPlayer;
    var index2;
    var indexPlayer2;
    var bestTeam;
    var bestScore;
    var draw;
    index = 0
    do {
        indexPlayer = Player(index)
        if (MeleeWasUserPlayer(indexPlayer)) {
            playerScore[index] = GetTournamentScore(indexPlayer)
            if (playerScore.getInt(index) <= 0) {
                playerScore[index] = 1
            }
        }
        else {
            playerScore[index] = 0
        }
        index = index + 1
        if (index == bj_MAX_PLAYERS) { break; }
    } while (true)
    teamCount = 0
    index = 0
    do {
        if (playerScore.getInt(index) != 0) {
            indexPlayer = Player(index)
            teamScore[teamCount] = 0
            teamForce[teamCount] = CreateForce()
            index2 = index
            do {
                if (playerScore.getInt(index2) != 0) {
                    indexPlayer2 = Player(index2)
                    if (PlayersAreCoAllied(indexPlayer, indexPlayer2)) {
                        teamScore[teamCount] = teamScore.getInt(teamCount) + playerScore.getInt(index2)
                        ForceAddPlayer(teamForce[teamCount], indexPlayer2)
                        playerScore[index2] = 0
                    }
                }
                index2 = index2 + 1
                if (index2 == bj_MAX_PLAYERS) { break; }
            } while (true)
            teamCount = teamCount + 1
        }
        index = index + 1
        if (index == bj_MAX_PLAYERS) { break; }
    } while (true)
    bj_meleeGameOver = true
    if (teamCount != 0) {
        bestTeam = -1
        bestScore = -1
        index = 0
        do {
            if (teamScore.getInt(index) > bestScore) {
                bestTeam = index
                bestScore = teamScore.getInt(index)
            }
            index = index + 1
            if (index == teamCount) { break; }
        } while (true)
        draw = false
        index = 0
        do {
            if (index != bestTeam) {
                if (bestScore < (multiplier * teamScore.getInt(index))) {
                    draw = true
                }
            }
            index = index + 1
            if (index == teamCount) { break; }
        } while (true)
        if (draw) {
            index = 0
            do {
                ForForce(teamForce[index], MeleeDoDrawEnum)
                index = index + 1
                if (index == teamCount) { break; }
            } while (true)
        }
        else {
            index = 0
            do {
                if (index != bestTeam) {
                    ForForce(teamForce[index], MeleeDoDefeatEnum)
                }
                index = index + 1
                if (index == teamCount) { break; }
            } while (true)
            ForForce(teamForce[bestTeam], MeleeDoVictoryEnum)
        }
    }
}
function MeleeTriggerTournamentFinishNow() {
    var rule = GetTournamentFinishNowRule();
    if (bj_meleeGameOver) {
        return
    }
    if ((rule == 1)) {
        MeleeTournamentFinishNowRuleA(1)
    }
    else {
        MeleeTournamentFinishNowRuleA(3)
    }
    MeleeRemoveObservers()
}
function MeleeInitVictoryDefeat() {
    var trig;
    var index;
    var indexPlayer;
    bj_finishSoonTimerDialog = CreateTimerDialog(null)
    trig = CreateTrigger()
    TriggerRegisterGameEvent(trig, EVENT_GAME_TOURNAMENT_FINISH_SOON)
    TriggerAddAction(trig, MeleeTriggerTournamentFinishSoon)
    trig = CreateTrigger()
    TriggerRegisterGameEvent(trig, EVENT_GAME_TOURNAMENT_FINISH_NOW)
    TriggerAddAction(trig, MeleeTriggerTournamentFinishNow)
    index = 0
    do {
        indexPlayer = Player(index)
        if ((GetPlayerSlotState(indexPlayer) == PLAYER_SLOT_STATE_PLAYING)) {
            bj_meleeDefeated[index] = false
            bj_meleeVictoried[index] = false
            bj_playerIsCrippled[index] = false
            bj_playerIsExposed[index] = false
            bj_crippledTimer[index] = CreateTimer()
            bj_crippledTimerWindows[index] = CreateTimerDialog(bj_crippledTimer[index])
            TimerDialogSetTitle(bj_crippledTimerWindows[index], MeleeGetCrippledTimerMessage(indexPlayer))
            trig = CreateTrigger()
            TriggerRegisterPlayerUnitEvent(trig, indexPlayer, EVENT_PLAYER_UNIT_CONSTRUCT_CANCEL, null)
            TriggerAddAction(trig, MeleeTriggerActionConstructCancel)
            trig = CreateTrigger()
            TriggerRegisterPlayerUnitEvent(trig, indexPlayer, EVENT_PLAYER_UNIT_DEATH, null)
            TriggerAddAction(trig, MeleeTriggerActionUnitDeath)
            trig = CreateTrigger()
            TriggerRegisterPlayerUnitEvent(trig, indexPlayer, EVENT_PLAYER_UNIT_CONSTRUCT_START, null)
            TriggerAddAction(trig, MeleeTriggerActionUnitConstructionStart)
            trig = CreateTrigger()
            TriggerRegisterPlayerEvent(trig, indexPlayer, EVENT_PLAYER_DEFEAT)
            TriggerAddAction(trig, MeleeTriggerActionPlayerDefeated)
            trig = CreateTrigger()
            TriggerRegisterPlayerEvent(trig, indexPlayer, EVENT_PLAYER_LEAVE)
            TriggerAddAction(trig, MeleeTriggerActionPlayerLeft)
            trig = CreateTrigger()
            TriggerRegisterPlayerAllianceChange(trig, indexPlayer, ALLIANCE_PASSIVE)
            TriggerRegisterPlayerStateEvent(trig, indexPlayer, PLAYER_STATE_ALLIED_VICTORY, EQUAL, 1)
            TriggerAddAction(trig, MeleeTriggerActionAllianceChange)
        }
        else {
            bj_meleeDefeated[index] = true
            bj_meleeVictoried[index] = false
            if ((IsPlayerObserver(indexPlayer))) {
                trig = CreateTrigger()
                TriggerRegisterPlayerEvent(trig, indexPlayer, EVENT_PLAYER_LEAVE)
                TriggerAddAction(trig, MeleeTriggerActionPlayerLeft)
            }
        }
        index = index + 1
        if (index == bj_MAX_PLAYERS) { break; }
    } while (true)
    TimerStart(CreateTimer(), 2.0, false, MeleeTriggerActionAllianceChange)
}
function CheckInitPlayerSlotAvailability() {
    var index;
    if ((!bj_slotControlReady)) {
        index = 0
        do {
            bj_slotControlUsed[index] = false
            bj_slotControl[index] = MAP_CONTROL_USER
            index = index + 1
            if (index == bj_MAX_PLAYERS) { break; }
        } while (true)
        bj_slotControlReady = true
    }
}
function SetPlayerSlotAvailable(whichPlayer, control) {
    var playerIndex = GetPlayerId(whichPlayer);
    CheckInitPlayerSlotAvailability()
    bj_slotControlUsed[playerIndex] = true
    bj_slotControl[playerIndex] = control
}
function TeamInitPlayerSlots(teamCount) {
    var index;
    var indexPlayer;
    var team;
    SetTeams(teamCount)
    CheckInitPlayerSlotAvailability()
    index = 0
    team = 0
    do {
        if ((bj_slotControlUsed[index])) {
            indexPlayer = Player(index)
            SetPlayerTeam(indexPlayer, team)
            team = team + 1
            if ((team >= teamCount)) {
                team = 0
            }
        }
        index = index + 1
        if (index == bj_MAX_PLAYERS) { break; }
    } while (true)
}
function MeleeInitPlayerSlots() {
    TeamInitPlayerSlots(bj_MAX_PLAYERS)
}
function FFAInitPlayerSlots() {
    TeamInitPlayerSlots(bj_MAX_PLAYERS)
}
function OneOnOneInitPlayerSlots() {
    SetTeams(2)
    SetPlayers(2)
    TeamInitPlayerSlots(2)
}
function InitGenericPlayerSlots() {
    var gType = GetGameTypeSelected();
    if ((gType == GAME_TYPE_MELEE)) {
        MeleeInitPlayerSlots()
    }
    else {
        if ((gType == GAME_TYPE_FFA)) {
            FFAInitPlayerSlots()
        }
        else {
            if ((gType == GAME_TYPE_USE_MAP_SETTINGS)) {
            }
            else {
                if ((gType == GAME_TYPE_ONE_ON_ONE)) {
                    OneOnOneInitPlayerSlots()
                }
                else {
                    if ((gType == GAME_TYPE_TWO_TEAM_PLAY)) {
                        TeamInitPlayerSlots(2)
                    }
                    else {
                        if ((gType == GAME_TYPE_THREE_TEAM_PLAY)) {
                            TeamInitPlayerSlots(3)
                        }
                        else {
                            if ((gType == GAME_TYPE_FOUR_TEAM_PLAY)) {
                                TeamInitPlayerSlots(4)
                            }
                            else {
                            }
                        }
                    }
                }
            }
        }
    }
}
function SetDNCSoundsDawn() {
    if (bj_useDawnDuskSounds) {
        StartSound(bj_dawnSound)
    }
}
function SetDNCSoundsDusk() {
    if (bj_useDawnDuskSounds) {
        StartSound(bj_duskSound)
    }
}
function SetDNCSoundsDay() {
    var ToD = GetTimeOfDay();
    if ((ToD >= bj_TOD_DAWN && ToD < bj_TOD_DUSK) && !bj_dncIsDaytime) {
        bj_dncIsDaytime = true
        StopSound(bj_nightAmbientSound, false, true)
        StartSound(bj_dayAmbientSound)
    }
}
function SetDNCSoundsNight() {
    var ToD = GetTimeOfDay();
    if ((ToD < bj_TOD_DAWN || ToD >= bj_TOD_DUSK) && bj_dncIsDaytime) {
        bj_dncIsDaytime = false
        StopSound(bj_dayAmbientSound, false, true)
        StartSound(bj_nightAmbientSound)
    }
}
function InitDNCSounds() {
    bj_dawnSound = CreateSoundFromLabel("RoosterSound", false, false, false, 10000, 10000)
    bj_duskSound = CreateSoundFromLabel("WolfSound", false, false, false, 10000, 10000)
    bj_dncSoundsDawn = CreateTrigger()
    TriggerRegisterGameStateEvent(bj_dncSoundsDawn, GAME_STATE_TIME_OF_DAY, EQUAL, bj_TOD_DAWN)
    TriggerAddAction(bj_dncSoundsDawn, SetDNCSoundsDawn)
    bj_dncSoundsDusk = CreateTrigger()
    TriggerRegisterGameStateEvent(bj_dncSoundsDusk, GAME_STATE_TIME_OF_DAY, EQUAL, bj_TOD_DUSK)
    TriggerAddAction(bj_dncSoundsDusk, SetDNCSoundsDusk)
    bj_dncSoundsDay = CreateTrigger()
    TriggerRegisterGameStateEvent(bj_dncSoundsDay, GAME_STATE_TIME_OF_DAY, GREATER_THAN_OR_EQUAL, bj_TOD_DAWN)
    TriggerRegisterGameStateEvent(bj_dncSoundsDay, GAME_STATE_TIME_OF_DAY, LESS_THAN, bj_TOD_DUSK)
    TriggerAddAction(bj_dncSoundsDay, SetDNCSoundsDay)
    bj_dncSoundsNight = CreateTrigger()
    TriggerRegisterGameStateEvent(bj_dncSoundsNight, GAME_STATE_TIME_OF_DAY, LESS_THAN, bj_TOD_DAWN)
    TriggerRegisterGameStateEvent(bj_dncSoundsNight, GAME_STATE_TIME_OF_DAY, GREATER_THAN_OR_EQUAL, bj_TOD_DUSK)
    TriggerAddAction(bj_dncSoundsNight, SetDNCSoundsNight)
}
function InitBlizzardGlobals() {
    var index;
    var userControlledPlayers;
    var v;
    filterIssueHauntOrderAtLocBJ = Filter(IssueHauntOrderAtLocBJFilter)
    filterEnumDestructablesInCircleBJ = Filter(EnumDestructablesInCircleBJFilter)
    filterGetUnitsInRectOfPlayer = Filter(GetUnitsInRectOfPlayerFilter)
    filterGetUnitsOfTypeIdAll = Filter(GetUnitsOfTypeIdAllFilter)
    filterGetUnitsOfPlayerAndTypeId = Filter(GetUnitsOfPlayerAndTypeIdFilter)
    filterMeleeTrainedUnitIsHeroBJ = Filter(MeleeTrainedUnitIsHeroBJFilter)
    filterLivingPlayerUnitsOfTypeId = Filter(LivingPlayerUnitsOfTypeIdFilter)
    index = 0
    do {
        if (index == bj_MAX_PLAYER_SLOTS) { break; }
        bj_FORCE_PLAYER[index] = CreateForce()
        ForceAddPlayer(bj_FORCE_PLAYER[index], Player(index))
        index = index + 1
    } while (true)
    bj_FORCE_ALL_PLAYERS = CreateForce()
    ForceEnumPlayers(bj_FORCE_ALL_PLAYERS, null)
    bj_cineModePriorSpeed = GetGameSpeed()
    bj_cineModePriorFogSetting = IsFogEnabled()
    bj_cineModePriorMaskSetting = IsFogMaskEnabled()
    index = 0
    do {
        if (index >= bj_MAX_QUEUED_TRIGGERS) { break; }
        bj_queuedExecTriggers[index] = null
        bj_queuedExecUseConds[index] = false
        index = index + 1
    } while (true)
    bj_isSinglePlayer = false
    userControlledPlayers = 0
    index = 0
    do {
        if (index >= bj_MAX_PLAYERS) { break; }
        if ((GetPlayerController(Player(index)) == MAP_CONTROL_USER && GetPlayerSlotState(Player(index)) == PLAYER_SLOT_STATE_PLAYING)) {
            userControlledPlayers = userControlledPlayers + 1
        }
        index = index + 1
    } while (true)
    bj_isSinglePlayer = (userControlledPlayers == 1)
    bj_rescueSound = CreateSoundFromLabel("Rescue", false, false, false, 10000, 10000)
    bj_questDiscoveredSound = CreateSoundFromLabel("QuestNew", false, false, false, 10000, 10000)
    bj_questUpdatedSound = CreateSoundFromLabel("QuestUpdate", false, false, false, 10000, 10000)
    bj_questCompletedSound = CreateSoundFromLabel("QuestCompleted", false, false, false, 10000, 10000)
    bj_questFailedSound = CreateSoundFromLabel("QuestFailed", false, false, false, 10000, 10000)
    bj_questHintSound = CreateSoundFromLabel("Hint", false, false, false, 10000, 10000)
    bj_questSecretSound = CreateSoundFromLabel("SecretFound", false, false, false, 10000, 10000)
    bj_questItemAcquiredSound = CreateSoundFromLabel("ItemReward", false, false, false, 10000, 10000)
    bj_questWarningSound = CreateSoundFromLabel("Warning", false, false, false, 10000, 10000)
    bj_victoryDialogSound = CreateSoundFromLabel("QuestCompleted", false, false, false, 10000, 10000)
    bj_defeatDialogSound = CreateSoundFromLabel("QuestFailed", false, false, false, 10000, 10000)
    DelayedSuspendDecayCreate()
    v = VersionGet()
    if ((v == VERSION_REIGN_OF_CHAOS)) {
        bj_MELEE_MAX_TWINKED_HEROES = bj_MELEE_MAX_TWINKED_HEROES_V0
    }
    else {
        bj_MELEE_MAX_TWINKED_HEROES = bj_MELEE_MAX_TWINKED_HEROES_V1
    }
}
function InitQueuedTriggers() {
    bj_queuedExecTimeout = CreateTrigger()
    TriggerRegisterTimerExpireEvent(bj_queuedExecTimeout, bj_queuedExecTimeoutTimer)
    TriggerAddAction(bj_queuedExecTimeout, QueuedTriggerDoneBJ)
}
function InitMapRects() {
    bj_mapInitialPlayableArea = Rect(GetCameraBoundMinX() - GetCameraMargin(CAMERA_MARGIN_LEFT), GetCameraBoundMinY() - GetCameraMargin(CAMERA_MARGIN_BOTTOM), GetCameraBoundMaxX() + GetCameraMargin(CAMERA_MARGIN_RIGHT), GetCameraBoundMaxY() + GetCameraMargin(CAMERA_MARGIN_TOP))
    bj_mapInitialCameraBounds = GetCurrentCameraBoundsMapRectBJ()
}
function InitSummonableCaps() {
    var index;
    index = 0
    do {
        if ((!GetPlayerTechResearched(Player(index), 'Rhrt', true))) {
            SetPlayerTechMaxAllowed(Player(index), 'hrtt', 0)
        }
        if ((!GetPlayerTechResearched(Player(index), 'Robk', true))) {
            SetPlayerTechMaxAllowed(Player(index), 'otbk', 0)
        }
        SetPlayerTechMaxAllowed(Player(index), 'uske', bj_MAX_SKELETONS)
        index = index + 1
        if (index == bj_MAX_PLAYERS) { break; }
    } while (true)
}
function UpdateStockAvailability(whichItem) {
    var iType = GetItemType(whichItem);
    var iLevel = GetItemLevel(whichItem);
    if ((iType == ITEM_TYPE_PERMANENT)) {
        bj_stockAllowedPermanent[iLevel] = true
    }
    else {
        if ((iType == ITEM_TYPE_CHARGED)) {
            bj_stockAllowedCharged[iLevel] = true
        }
        else {
            if ((iType == ITEM_TYPE_ARTIFACT)) {
                bj_stockAllowedArtifact[iLevel] = true
            }
            else {
            }
        }
    }
}
function UpdateEachStockBuildingEnum() {
    var iteration = 0;
    var pickedItemId;
    do {
        pickedItemId = ChooseRandomItemEx(bj_stockPickedItemType, bj_stockPickedItemLevel)
        if (IsItemIdSellable(pickedItemId)) { break; }
        iteration = iteration + 1
        if ((iteration > bj_STOCK_MAX_ITERATIONS)) {
            return
        }
    } while (true)
    AddItemToStock(GetEnumUnit(), pickedItemId, 1, 1)
}
function UpdateEachStockBuilding(iType, iLevel) {
    var g;
    bj_stockPickedItemType = iType
    bj_stockPickedItemLevel = iLevel
    g = CreateGroup()
    GroupEnumUnitsOfType(g, "marketplace", null)
    ForGroup(g, UpdateEachStockBuildingEnum)
    DestroyGroup(g)
}
function PerformStockUpdates() {
    var pickedItemId;
    var pickedItemType;
    var pickedItemLevel = 0;
    var allowedCombinations = 0;
    var iLevel;
    iLevel = 1
    do {
        if ((bj_stockAllowedPermanent[iLevel])) {
            allowedCombinations = allowedCombinations + 1
            if ((GetRandomInt(1, allowedCombinations) == 1)) {
                pickedItemType = ITEM_TYPE_PERMANENT
                pickedItemLevel = iLevel
            }
        }
        if ((bj_stockAllowedCharged[iLevel])) {
            allowedCombinations = allowedCombinations + 1
            if ((GetRandomInt(1, allowedCombinations) == 1)) {
                pickedItemType = ITEM_TYPE_CHARGED
                pickedItemLevel = iLevel
            }
        }
        if ((bj_stockAllowedArtifact[iLevel])) {
            allowedCombinations = allowedCombinations + 1
            if ((GetRandomInt(1, allowedCombinations) == 1)) {
                pickedItemType = ITEM_TYPE_ARTIFACT
                pickedItemLevel = iLevel
            }
        }
        iLevel = iLevel + 1
        if (iLevel > bj_MAX_ITEM_LEVEL) { break; }
    } while (true)
    if ((allowedCombinations == 0)) {
        return
    }
    UpdateEachStockBuilding(pickedItemType, pickedItemLevel)
}
function StartStockUpdates() {
    PerformStockUpdates()
    TimerStart(bj_stockUpdateTimer, bj_STOCK_RESTOCK_INTERVAL, true, PerformStockUpdates)
}
function RemovePurchasedItem() {
    RemoveItemFromStock(GetSellingUnit(), GetItemTypeId(GetSoldItem()))
}
function InitNeutralBuildings() {
    var iLevel;
    iLevel = 0
    do {
        bj_stockAllowedPermanent[iLevel] = false
        bj_stockAllowedCharged[iLevel] = false
        bj_stockAllowedArtifact[iLevel] = false
        iLevel = iLevel + 1
        if (iLevel > bj_MAX_ITEM_LEVEL) { break; }
    } while (true)
    SetAllItemTypeSlots(bj_MAX_STOCK_ITEM_SLOTS)
    SetAllUnitTypeSlots(bj_MAX_STOCK_UNIT_SLOTS)
    bj_stockUpdateTimer = CreateTimer()
    TimerStart(bj_stockUpdateTimer, bj_STOCK_RESTOCK_INITIAL_DELAY, false, StartStockUpdates)
    bj_stockItemPurchased = CreateTrigger()
    TriggerRegisterPlayerUnitEvent(bj_stockItemPurchased, Player(PLAYER_NEUTRAL_PASSIVE), EVENT_PLAYER_UNIT_SELL_ITEM, null)
    TriggerAddAction(bj_stockItemPurchased, RemovePurchasedItem)
}
function MarkGameStarted() {
    bj_gameStarted = true
    DestroyTimer(bj_gameStartedTimer)
}
function DetectGameStarted() {
    bj_gameStartedTimer = CreateTimer()
    TimerStart(bj_gameStartedTimer, bj_GAME_STARTED_THRESHOLD, false, MarkGameStarted)
}
function InitBlizzard() {
    ConfigureNeutralVictim()
    InitBlizzardGlobals()
    InitQueuedTriggers()
    InitRescuableBehaviorBJ()
    InitDNCSounds()
    InitMapRects()
    InitSummonableCaps()
    InitNeutralBuildings()
    DetectGameStarted()
}
function RandomDistReset() {
    bj_randDistCount = 0
}
function RandomDistAddItem(inID, inChance) {
    bj_randDistID[bj_randDistCount] = inID
    bj_randDistChance[bj_randDistCount] = inChance
    bj_randDistCount = bj_randDistCount + 1
}
function RandomDistChoose() {
    var sum = 0;
    var chance = 0;
    var index;
    var foundID = -1;
    var done;
    if ((bj_randDistCount == 0)) {
        return -1
    }
    index = 0
    do {
        sum = sum + bj_randDistChance.getInt(index)
        index = index + 1
        if (index == bj_randDistCount) { break; }
    } while (true)
    chance = GetRandomInt(1, sum)
    index = 0
    sum = 0
    done = false
    do {
        sum = sum + bj_randDistChance.getInt(index)
        if ((chance <= sum)) {
            foundID = bj_randDistID.getInt(index)
            done = true
        }
        index = index + 1
        if ((index == bj_randDistCount)) {
            done = true
        }
        if (done == true) { break; }
    } while (true)
    return foundID
}
function UnitDropItem(inUnit, inItemID) {
    var x;
    var y;
    var radius = 32;
    var unitX;
    var unitY;
    var droppedItem;
    if ((inItemID == -1)) {
        return null
    }
    unitX = GetUnitX(inUnit)
    unitY = GetUnitY(inUnit)
    x = GetRandomReal(unitX - radius, unitX + radius)
    y = GetRandomReal(unitY - radius, unitY + radius)
    droppedItem = CreateItem(inItemID, x, y)
    SetItemDropID(droppedItem, GetUnitTypeId(inUnit))
    UpdateStockAvailability(droppedItem)
    return droppedItem
}
function WidgetDropItem(inWidget, inItemID) {
    var x;
    var y;
    var radius = 32;
    var widgetX;
    var widgetY;
    if ((inItemID == -1)) {
        return null
    }
    widgetX = GetWidgetX(inWidget)
    widgetY = GetWidgetY(inWidget)
    x = GetRandomReal(widgetX - radius, widgetX + radius)
    y = GetRandomReal(widgetY - radius, widgetY + radius)
    return CreateItem(inItemID, x, y)
}
function BlzIsLastInstanceObjectFunctionSuccessful() {
    return bj_lastInstObjFuncSuccessful
}
function BlzSetAbilityBooleanFieldBJ(whichAbility, whichField, value) {
    bj_lastInstObjFuncSuccessful = BlzSetAbilityBooleanField(whichAbility, whichField, value)
}
function BlzSetAbilityIntegerFieldBJ(whichAbility, whichField, value) {
    bj_lastInstObjFuncSuccessful = BlzSetAbilityIntegerField(whichAbility, whichField, value)
}
function BlzSetAbilityRealFieldBJ(whichAbility, whichField, value) {
    bj_lastInstObjFuncSuccessful = BlzSetAbilityRealField(whichAbility, whichField, value)
}
function BlzSetAbilityStringFieldBJ(whichAbility, whichField, value) {
    bj_lastInstObjFuncSuccessful = BlzSetAbilityStringField(whichAbility, whichField, value)
}
function BlzSetAbilityBooleanLevelFieldBJ(whichAbility, whichField, level, value) {
    bj_lastInstObjFuncSuccessful = BlzSetAbilityBooleanLevelField(whichAbility, whichField, level, value)
}
function BlzSetAbilityIntegerLevelFieldBJ(whichAbility, whichField, level, value) {
    bj_lastInstObjFuncSuccessful = BlzSetAbilityIntegerLevelField(whichAbility, whichField, level, value)
}
function BlzSetAbilityRealLevelFieldBJ(whichAbility, whichField, level, value) {
    bj_lastInstObjFuncSuccessful = BlzSetAbilityRealLevelField(whichAbility, whichField, level, value)
}
function BlzSetAbilityStringLevelFieldBJ(whichAbility, whichField, level, value) {
    bj_lastInstObjFuncSuccessful = BlzSetAbilityStringLevelField(whichAbility, whichField, level, value)
}
function BlzSetAbilityBooleanLevelArrayFieldBJ(whichAbility, whichField, level, index, value) {
    bj_lastInstObjFuncSuccessful = BlzSetAbilityBooleanLevelArrayField(whichAbility, whichField, level, index, value)
}
function BlzSetAbilityIntegerLevelArrayFieldBJ(whichAbility, whichField, level, index, value) {
    bj_lastInstObjFuncSuccessful = BlzSetAbilityIntegerLevelArrayField(whichAbility, whichField, level, index, value)
}
function BlzSetAbilityRealLevelArrayFieldBJ(whichAbility, whichField, level, index, value) {
    bj_lastInstObjFuncSuccessful = BlzSetAbilityRealLevelArrayField(whichAbility, whichField, level, index, value)
}
function BlzSetAbilityStringLevelArrayFieldBJ(whichAbility, whichField, level, index, value) {
    bj_lastInstObjFuncSuccessful = BlzSetAbilityStringLevelArrayField(whichAbility, whichField, level, index, value)
}
function BlzAddAbilityBooleanLevelArrayFieldBJ(whichAbility, whichField, level, value) {
    bj_lastInstObjFuncSuccessful = BlzAddAbilityBooleanLevelArrayField(whichAbility, whichField, level, value)
}
function BlzAddAbilityIntegerLevelArrayFieldBJ(whichAbility, whichField, level, value) {
    bj_lastInstObjFuncSuccessful = BlzAddAbilityIntegerLevelArrayField(whichAbility, whichField, level, value)
}
function BlzAddAbilityRealLevelArrayFieldBJ(whichAbility, whichField, level, value) {
    bj_lastInstObjFuncSuccessful = BlzAddAbilityRealLevelArrayField(whichAbility, whichField, level, value)
}
function BlzAddAbilityStringLevelArrayFieldBJ(whichAbility, whichField, level, value) {
    bj_lastInstObjFuncSuccessful = BlzAddAbilityStringLevelArrayField(whichAbility, whichField, level, value)
}
function BlzRemoveAbilityBooleanLevelArrayFieldBJ(whichAbility, whichField, level, value) {
    bj_lastInstObjFuncSuccessful = BlzRemoveAbilityBooleanLevelArrayField(whichAbility, whichField, level, value)
}
function BlzRemoveAbilityIntegerLevelArrayFieldBJ(whichAbility, whichField, level, value) {
    bj_lastInstObjFuncSuccessful = BlzRemoveAbilityIntegerLevelArrayField(whichAbility, whichField, level, value)
}
function BlzRemoveAbilityRealLevelArrayFieldBJ(whichAbility, whichField, level, value) {
    bj_lastInstObjFuncSuccessful = BlzRemoveAbilityRealLevelArrayField(whichAbility, whichField, level, value)
}
function BlzRemoveAbilityStringLevelArrayFieldBJ(whichAbility, whichField, level, value) {
    bj_lastInstObjFuncSuccessful = BlzRemoveAbilityStringLevelArrayField(whichAbility, whichField, level, value)
}
function BlzItemAddAbilityBJ(whichItem, abilCode) {
    bj_lastInstObjFuncSuccessful = BlzItemAddAbility(whichItem, abilCode)
}
function BlzItemRemoveAbilityBJ(whichItem, abilCode) {
    bj_lastInstObjFuncSuccessful = BlzItemRemoveAbility(whichItem, abilCode)
}
function BlzSetItemBooleanFieldBJ(whichItem, whichField, value) {
    bj_lastInstObjFuncSuccessful = BlzSetItemBooleanField(whichItem, whichField, value)
}
function BlzSetItemIntegerFieldBJ(whichItem, whichField, value) {
    bj_lastInstObjFuncSuccessful = BlzSetItemIntegerField(whichItem, whichField, value)
}
function BlzSetItemRealFieldBJ(whichItem, whichField, value) {
    bj_lastInstObjFuncSuccessful = BlzSetItemRealField(whichItem, whichField, value)
}
function BlzSetItemStringFieldBJ(whichItem, whichField, value) {
    bj_lastInstObjFuncSuccessful = BlzSetItemStringField(whichItem, whichField, value)
}
function BlzSetUnitBooleanFieldBJ(whichUnit, whichField, value) {
    bj_lastInstObjFuncSuccessful = BlzSetUnitBooleanField(whichUnit, whichField, value)
}
function BlzSetUnitIntegerFieldBJ(whichUnit, whichField, value) {
    bj_lastInstObjFuncSuccessful = BlzSetUnitIntegerField(whichUnit, whichField, value)
}
function BlzSetUnitRealFieldBJ(whichUnit, whichField, value) {
    bj_lastInstObjFuncSuccessful = BlzSetUnitRealField(whichUnit, whichField, value)
}
function BlzSetUnitStringFieldBJ(whichUnit, whichField, value) {
    bj_lastInstObjFuncSuccessful = BlzSetUnitStringField(whichUnit, whichField, value)
}
function BlzSetUnitWeaponBooleanFieldBJ(whichUnit, whichField, index, value) {
    bj_lastInstObjFuncSuccessful = BlzSetUnitWeaponBooleanField(whichUnit, whichField, index, value)
}
function BlzSetUnitWeaponIntegerFieldBJ(whichUnit, whichField, index, value) {
    bj_lastInstObjFuncSuccessful = BlzSetUnitWeaponIntegerField(whichUnit, whichField, index, value)
}
function BlzSetUnitWeaponRealFieldBJ(whichUnit, whichField, index, value) {
    bj_lastInstObjFuncSuccessful = BlzSetUnitWeaponRealField(whichUnit, whichField, index, value)
}
function BlzSetUnitWeaponStringFieldBJ(whichUnit, whichField, index, value) {
    bj_lastInstObjFuncSuccessful = BlzSetUnitWeaponStringField(whichUnit, whichField, index, value)
}
