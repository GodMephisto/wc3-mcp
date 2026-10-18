---
type: vendor-doc
source_repo: "https://github.com/jlfarris91/WurstMMD"
source_path: "wurst/MMD.wurst"
ref: HEAD
---

package MMD
import ClosureTimers
import ErrorHandling

@configurable public constant bool SHOW_DEBUG_MESSAGES = true

constant string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789_-+= \\!@#$%^&*()/?>.<,;:'\"{}[]|`~"
constant int num_chars = chars.length()
boolean initialized = false
trigger g_initializedTrigger = CreateTrigger()
        
gamecache gc = null
constant string ESCAPED_CHARS = " \\"

constant int CURRENT_VERSION = 1
constant int MINIMUM_PARSER_VERSION = 1
constant string FILENAME = "MMD.Dat"
constant string M_KEY_VAL = "val:"
constant string M_KEY_CHK = "chk:"
constant int NUM_SENDERS_NAIVE = 1
constant int NUM_SENDERS_SAFE = 3
int num_senders = NUM_SENDERS_NAIVE
int num_msg = 0        

timer clock = CreateTimer()
string array q_msg
real array q_time
int array q_index
QueueNode q_head = null
QueueNode q_tail = null

public enum Goal
  None
  High
  Low

function Goal.toString() returns string
  switch (this)
    case None
      return "none"
    case High
      return "high"
    case Low
      return "low"

public enum ValueType
  String
  Real
  Int

function ValueType.toString() returns string
  switch (this)
    case String
      return "string"
    case Real
      return "real"
    case Int
      return "int"

public enum Suggestion
  None
  Track
  Leaderboard

function Suggestion.toString() returns string
  switch (this)
    case None
      return "none"
    case Track
      return "track"
    case Leaderboard
      return "leaderboard"

public enum PlayerFlag
  Drawer
  Loser
  Winner
  Leaver
  Practicing

function PlayerFlag.toString() returns string
  switch (this)
    case Drawer
      return "drawer"
    case Loser
      return "loser"
    case Winner
      return "winner"
    case Leaver
      return "leaver"
    case Practicing
      return "practicing"

public interface IMMDValueString
  function set(player p, string value)

public interface IMMDValueReal
  function set(player p, real value)
  function add(player p, real value)
  function sub(player p, real value)

class ValueReal implements IMMDValueReal
  string packedName
  construct(string packedName)
    this.packedName = packedName
  override function set(player p, real value)
    Emitter.emitSetVariable(p, packedName, "=", value.toString())
  override function add(player p, real value)
    Emitter.emitSetVariable(p, packedName, "+=", value.toString())
  override function sub(player p, real value)
    Emitter.emitSetVariable(p, packedName, "-=", value.toString())

public interface IMMDValueInt
  function set(player p, int value)
  function add(player p, int value)
  function sub(player p, int value)

class ValueInt implements IMMDValueInt
  private string packedName
  construct(string packedName)
    this.packedName = packedName
  override function set(player p, int value)
    Emitter.emitSetVariable(p, packedName, "=", value.toString())
  override function add(player p, int value)
    Emitter.emitSetVariable(p, packedName, "+=", value.toString())
  override function sub(player p, int value)
    Emitter.emitSetVariable(p, packedName, "-=", value.toString())

public interface IMMDEvent
  function raise(vararg string args)

class Event implements IMMDEvent
  private string packedName
  private int numArgs
  construct(string packedName, int numArgs)
    this.packedName = packedName
    this.numArgs = numArgs
  override function raise(vararg string args)
    var argsData = ""
    var count = 0
    for arg in args
      count++
      argsData += " " + pack(arg)
    if (count != numArgs)
      error("MMD: Expected {0} args for event {1} but got {2}".format(numArgs.toString(), packedName, count.toString()))
    Emitter.emitEvent(packedName, numArgs == 0 ? "" : argsData)

/**Triggered when tampering is detected. Increases the number of safeguards against tampering.*/
public function raiseGuard(string reason)
  if (SHOW_DEBUG_MESSAGES)
    BJDebugMsg("MMD: Guard Raised! (" + reason + ")")
  num_senders = NUM_SENDERS_SAFE //increase number of players voting on each message

/**Returns seconds elapsed in game time*/
function time() returns real
  return clock.getElapsed()

/**Initializes the char-to-int conversion*/
function prepC2I()
  for i = 0 to num_chars-1
    var id = SubString(chars, i, i+1)
    if (id == StringCase(id, true))
      id += "U"
    gc.saveInt("c2i", id, i)

/**Converts a character to an int*/
function c2i(string c) returns int
  var id = c
  if id == StringCase(id, true)
    id = id + "U"
  var i = gc.loadInt("c2i", id)
  if (i < 0 or i >= num_chars or SubString(chars, i, i+1) != c) and gc.hasInt("c2i", id)
    //A cheater sent a fake sync to screw with the cached values
    i = 0
    while (i < num_chars)
      if c == SubString(chars, i, i+1) //cheating!
        raiseGuard("c2i poisoned")
        gc.saveInt("c2i", id, i)
        break
      i++
  return i

/**Computes a weak hash value, hopefully secure enough for our purposes*/
function poorHash(string s, int seed) returns int
  let n = s.length()
  var m = n + seed
  for i = 0 to n-1
    m = m * 41 + c2i(SubString(s, i, i+1))
  return m

/**Stores previously sent messages for tamper detection purposes*/
class QueueNode
  real timeout = 0.0
  string msg = ""
  int checksum = 0
  string key = ""
  QueueNode next = null
  
  construct(int id, string msg)
    this.timeout = time() + 7.0 + GetRandomReal(0, 2 + 0.1 * localPlayer.getId())
    this.msg = msg
    this.checksum = poorHash(msg, id)
    this.key = id.toString()

  ondestroy
    gc.flushInt(M_KEY_VAL + key, msg)
    gc.flushInt(M_KEY_CHK + key, key)
    this.msg = null
    this.key = null
    this.next = null

  function send()
    gc.saveInt(M_KEY_VAL + key, msg, checksum)
    gc.saveInt(M_KEY_CHK + key, key, checksum)
    gc.syncInt(M_KEY_VAL + key, msg)
    gc.syncInt(M_KEY_CHK + key, key)

/**Returns true for a fixed size uniform random subof players in the game*/
function isEmitter() returns bool
  var n = 0
  int array picks
  bool array pick_flags
  for i = 0 to 23
    if GetPlayerController(players[i]) == MAP_CONTROL_USER and GetPlayerSlotState(players[i]) == PLAYER_SLOT_STATE_PLAYING
      if n < num_senders //initializing picks
        picks[n] = i
        pick_flags[i] = true
      else //maintain the invariant 'P(being picked) = c/n'
        let r = GetRandomInt(0, n)
        if r < num_senders
          pick_flags[picks[r]] = false
          picks[r] = i
          pick_flags[i] = true
      n++
  return pick_flags[localPlayer.getId()]

/**Places meta-data in the replay and in network traffic*/
function emit(string message)
  if (not initialized)
    BJDebugMsg("MMD Emit Error: Library not initialized yet.")
    return

  //remember sent messages for tamper check
  let q = new QueueNode(num_msg, message)
  if q_head == null
    q_head = q
  else
    q_tail.next = q
  q_tail = q
          
  //send new message
  num_msg = num_msg + 1
  if isEmitter()
    q.send()

/**Performs tamper checks*/
function tick()
    
  //check previously sent messages for tampering
  var q = q_head

  while (q != null and q.timeout < time())
    if not gc.hasInt(M_KEY_VAL+q.key, q.msg)
      raiseGuard("message skipping")
      q.send()
    else if not gc.hasInt(M_KEY_CHK+q.key, q.key)
      raiseGuard("checksum skipping")
      q.send()
    else if gc.loadInt(M_KEY_VAL+q.key, q.msg) != q.checksum
      raiseGuard("message tampering")
      q.send()
    else if gc.loadInt(M_KEY_CHK+q.key, q.key) != q.checksum
      raiseGuard("checksum tampering")
      q.send()
    q_head = q.next
    destroy q
    q = q_head

  if q_head == null
    q_tail = null
  
  //check for future message tampering
  for i = 0 to 9
    if (not gc.hasInt(M_KEY_CHK + num_msg.toString(), num_msg.toString()))
      break
    raiseGuard("message insertion")
    emit("Blank")
    
/**Replaces control characters with escape sequences*/
function pack(string value) returns string
  var result = ""
  let valueLen = value.length()
  let escapedCharsLen = ESCAPED_CHARS.length()  
  //for each character in argument string
  for i = 0 to valueLen-1
    var c = SubString(value, i, i+1)
    //for each character in escaped chars string
    for j = 0 to escapedCharsLen-1
      //escape control characters
      if (c == SubString(ESCAPED_CHARS, j, j+1))
        c = "\\" + c
        break
    result += c    
  return result

///////////////////////////////////////////////////////////////
/// Public functions
///////////////////////////////////////////////////////////////

public class MMD

  static function onInitialized(code func)
    if initialized
      let callbackTrigger = CreateTrigger()
      callbackTrigger.addAction(func)
      callbackTrigger.execute()
      callbackTrigger.destr()
    else
      g_initializedTrigger.addAction(func)

  /**Sets a player flag like "win_on_leave"*/
  static function flagPlayer(player p, PlayerFlag flag)
    let id = p.getId()
    if id < 0 or id >= 24
      BJDebugMsg("MMD Flag Error: Invalid player. Must be P1 to P24.")
    else if GetPlayerController(players[id]) == MAP_CONTROL_USER
      Emitter.emitFlagPlayer(p, flag)

  /**Defines a variable to store string values in*/
  static function defineStringValue(string name) returns IMMDValueString
    return defineStringValue(name, Suggestion.None)

  /**Defines a variable to store string values in*/
  static function defineStringValue(string name, Suggestion suggestion) returns IMMDValueString
    if (name == "" or name == null)
      error("MMD: value name is empty")
    if (name.length() > 32)
      error("MMD: value name '" + name + "' is too long")
    let packedName = pack(name)
    gc.saveInt("types", name, ValueType.String castTo int)
    Emitter.emitDefineVariable(packedName, ValueType.String, Goal.None, suggestion)
    return (player p, string value) -> Emitter.emitSetVariable(p, packedName, "=", "\"{0}\"".format(pack(value)))

  /**Defines a variable to store integer values in*/
  static function defineValueInt(string name) returns IMMDValueInt
    return defineValueInt(name, Goal.None, Suggestion.None)

  /**Defines a variable to store integer values in*/
  static function defineValueInt(string name, Goal goal) returns IMMDValueInt
    return defineValueInt(name, goal, Suggestion.None)

  /**Defines a variable to store integer values in*/
  static function defineValueInt(string name, Goal goal, Suggestion suggestion) returns IMMDValueInt
    if (name == "" or name == null)
      error("MMD: value name is empty")
    if (name.length() > 32)
      error("MMD: value name '" + name + "' is too long")
    let packedName = pack(name)
    gc.saveInt("types", name, ValueType.Int castTo int)
    Emitter.emitDefineVariable(packedName, ValueType.Int, goal, suggestion)
    return new ValueInt(packedName)

  /**Defines a variable to store real values in*/
  static function defineValueReal(string name) returns IMMDValueReal
    return defineValueReal(name, Goal.None, Suggestion.None)

  /**Defines a variable to store real values in*/
  static function defineValueReal(string name, Goal goal) returns IMMDValueReal
    return defineValueReal(name, goal, Suggestion.None)

  /**Defines a variable to store real values in*/
  static function defineValueReal(string name, Goal goal, Suggestion suggestion) returns IMMDValueReal
    if (name == "" or name == null)
      error("MMD: value name is empty")
    if (name.length() > 32)
      error("MMD: value name '" + name + "' is too long")
    let packedName = pack(name)
    gc.saveInt("types", name, ValueType.Real castTo int)
    Emitter.emitDefineVariable(packedName, ValueType.Real, goal, suggestion)
    return new ValueReal(packedName)

  static function setValue(player p, string name, string value)
    updateValue(name, p, "=", "\"{0}\"".format(pack(value)), ValueType.String)

  static function setValue(player p, string name, real value)
    updateValue(name, p, "=", value.toString(), ValueType.Real)

  static function setValue(player p, string name, int value)
    updateValue(name, p, "=", value.toString(), ValueType.Int)

  static function addValue(player p, string name, real value)
    updateValue(name, p, "+=", value.toString(), ValueType.Real)

  static function addValue(player p, string name, int value)
    updateValue(name, p, "+=", value.toString(), ValueType.Int)

  static function subValue(player p, string name, real value)
    updateValue(name, p, "-=", value.toString(), ValueType.Real)

  static function subValue(player p, string name, int value)
    updateValue(name, p, "-=", value.toString(), ValueType.Int)
    
  /**Updates the value of a defined variable for a given player*/
  private static function updateValue(string name, player p, string op, string value, ValueType _type)
    let id = p.getId()
    if p == null or id < 0 or id >= 24
      //BJDebugMsg("MMD Set Error: Invalid player. Must be P1 to P24.")
      skip
    else if (_type castTo int) != gc.loadInt("types", name)
      BJDebugMsg("MMD Set Error: Updated value of undefined variable or used value of incorrect type.")
    else if op.length() == 0
      BJDebugMsg("MMD Set Error: Unrecognized operation type.")        
    else if name.length() > 50
      BJDebugMsg("MMD Set Error: Variable name is too long.")
    else if name.length() == 0
      BJDebugMsg("MMD Set Error: Variable name is empty.")
    else
      //BJDebugMsg ("MMD Update: [name: " + name + "] [pid: " + I2S ((p.getId())) + "] [op: " + op + "] [value: " + value + "] [val_type: " + I2S (val_type) + "]")
      Emitter.emitSetVariable(p, pack(name), op, value)

  static function defineEvent(string name) returns IMMDEvent
    return defineEvent(name, 0, name, "")

  static function defineEvent(string name, string format) returns IMMDEvent
    return defineEvent(name, 0, format, "")

  static function defineEvent(string name, string format, string argName0) returns IMMDEvent
    return defineEvent(name, 1, format, pack(argName0))

  static function defineEvent(string name, string format, string argName0, string argName1) returns IMMDEvent
    return defineEvent(name, 2, format, pack(argName0) + " " + pack(argName1))

  static function defineEvent(string name, string format, string argName0, string argName1, string argName2) returns IMMDEvent
    return defineEvent(name, 3, format, pack(argName0) + " " + pack(argName1) + " " + pack(argName2))

  /**Defines an event's arguments and format*/
  private static function defineEvent(string name, int num_args, string format, string data) returns IMMDEvent
    if gc.loadInt("events", name) != 0
      BJDebugMsg("MMD DefEvent Error: Event redefined.")
    else
      gc.saveInt("events", name, num_args + 1)
      let finalArgs = num_args > 0 ? "{0} {1}".format(num_args.toString(), data) : "0"
      Emitter.emitDefineEvent(pack(name), finalArgs, pack(format))
    return new Event(pack(name), num_args)

  static function logEvent(string name)
    logEvent(name, 0, "")

  static function logEvent(string name, string arg0)
    logEvent(name, 1, " " + pack(arg0))

  static function logEvent(string name, string arg0, string arg1)
    logEvent(name, 2, " " + pack(arg0) + " " + pack(arg1))

  static function logEvent(string name, string arg0, string arg1, string arg2)
    logEvent(name, 3, " " + pack(arg0) + " " + pack(arg1) + " " + pack(arg2))

  /**Places an event in the meta-data*/
  private static function logEvent(string name, int num_args, string data)
    if gc.loadInt("events", name) != num_args+1
      BJDebugMsg("MMD logEvent Error: Event not defined or defined with different # of args.")
    else
      Emitter.emitEvent(pack(name), data)

  static function defineSetting(string name, string value)
    defineSetting(name, ValueType.String, value)

  static function defineSetting(string name, int value)
    defineSetting(name, ValueType.Int, value.toString())

  static function defineSetting(string name, real value)
    defineSetting(name, ValueType.Real, value.toString())

  static function defineSetting(string name, bool value)
    defineSetting(name, ValueType.Int, value ? "1" : "0")

  /**Defines an event's arguments and format*/
  private static function defineSetting(string name, ValueType _type, string value)
    if gc.loadInt("setting", name) != 0
      BJDebugMsg("MMD DefSetting Error: Setting redefined.")
    else
      gc.saveInt("setting", name, 1)
      Emitter.emitDefineSetting(pack(name), _type.toString(), value)

  /**Emits meta-data which parsers will ignore unless they are customized to understand it*/
  static function logCustom(string unique_identifier, string data)
    Emitter.emitCustom(pack(unique_identifier), pack(data))

class Emitter

  static function emitFlagPlayer(player p, PlayerFlag flag)
    emit("FlagP {0} {1}".format(p.getId().toString(), flag.toString()))

  static function emitDefineVariable(string packedName, ValueType _type, Goal goal, Suggestion suggestion)
    emit("DefVarP {0} {1} {2} {3}".format(packedName, _type.toString(), goal.toString(), suggestion.toString()))

  static function emitSetVariable(player p, string packedName, string op, string packedValue)
    emit("VarP {0} {1} {2} {3}".format(p.getId().toString(), packedName, op, packedValue))

  static function emitDefineEvent(string packedName, string args, string packedFormat)
    emit("DefEvent {0} {1} {2}".format(packedName, args, packedFormat))

  /** args must either be empty or contain comma-separated strings starting with an empty space */
  static function emitEvent(string packedName, string args)
    emit("Event {0}{1}".format(packedName, args))
  
  static function emitDefineSetting(string packedName, string _type, string value)
    emit("setting {0} {1} {2}".format(packedName, _type, value))

  static function emitCustom(string packedIdentifier, string packedData)
    emit("custom {0} {1}".format(packedIdentifier, packedData))

///////////////////////////////////////////////////////////////
/// Initialization
///////////////////////////////////////////////////////////////

/**Emits initialization data*/
function init2()
  initialized = true
    
  emit("init version " + I2S(MINIMUM_PARSER_VERSION) + " " + I2S(CURRENT_VERSION))

  for i = 0 to 23
    let p = players[i]
    if GetPlayerController(p) == MAP_CONTROL_USER and GetPlayerSlotState(p) == PLAYER_SLOT_STATE_PLAYING
      emit("init pid " + I2S(i) + " " + pack(GetPlayerName(p)))

  doPeriodically(0.37, cb -> tick())

  g_initializedTrigger.execute()

///Places init2 on a timer, initializes game cache, and translates constants*/
init
  nullTimer(() -> init2())
  FlushGameCache(InitGameCache(FILENAME))
  gc = InitGameCache(FILENAME)
  TimerStart(clock, 999999999, false, null)
  prepC2I()
