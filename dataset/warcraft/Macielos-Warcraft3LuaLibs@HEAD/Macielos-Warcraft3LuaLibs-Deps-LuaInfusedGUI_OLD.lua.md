---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "Deps/LuaInfusedGUI_OLD.lua"
ref: HEAD
---

if Debug then Debug.beginFile "LuaInfusedGUI" end
--[[
    Lua-Infused GUI with automatic memory leak resolution: Modernizing the experience for a better future for users of the Trigger Editor.

    Credits:
        Bribe, Tasyen, Dr Super Good, HerlySQR, Antares, Marcielos

    Transforming rects, locations, groups, forces and BJ hashtable wrappers into Lua tables, which are automatically garbage collected.

    Provides RegisterAnyPlayerUnitEvent to cut down on handle count and simplify syntax for Lua users while benefitting GUI.

    Provides GUI.enumUnitsInRect/InRange/Selected/etc. which replaces the first parameter with a function (which takes a unit), for immediate action without needing a separate group variable.

    Provides GUI.loopArray for safe iteration over a __jarray

    Update: 16 Mar 2026 by InsanityAI
    Changes:
        - Fixed asserts early exiting functions in case _THROW_ERROR_ON_INVALID_ARG is set to true and the arg condition is valid

    Update: 15 Mar 2026 by InsanityAI
    Changes:
        - Added _THROW_ERROR_ON_INVALID_ARG & _PRINT_WARNING_ON_INVALID_ARG flags that modify how assert works within this system
        - Location and Rect overrides now return non-nil values even if error is disabled but no valid argument was provided

    Update: 01 Mar 2026 by Marcielos & InsanityAI
    Changes:
        - Fixed GroupClear, GroupAddUnit, GroupAddGroup and GroupRemoveGroup overrides
        - Fixed Hashtable API where argument order was wrong
        - Overridden CreateMinimapIconAtLoc and ExecuteFunc

    Update: 02 Feb 2026 by Insanity_AI
    Changes:
        - FakedType property is now a string
        - replaced _G with _ENV for a (negligible) speed boost
        - additional asserts for hashtable API
        - Hashtable API now replaces the natives instead of BJs
        - GroupRemoveUnit now no longer breaks the FakeGroup (thanks Antares & Macielos)
        - fixed SetHeroStat
        - added some String & Math API overrides (check the bottom of the script for the list)
        - modified GroupXOrder overrides to use group natives in order to retain speed and formation of units when ordered as a group (thanks Macielos)
        - swapped order of overrides: group <-> location, so that group overrides happen first

    Update: 30 Sep 2025 by Insanity_AI
    Changes:
        - asserts on arguments so DebugUtils can more effectively tell you what's wrong
        - StringHashBJ and GetHandleIdBJ returns 0 if the argument is falsy, otherwise returns the argument itself
        - fixed Hashtable API overrides to support niche Hashtable mechanic of being able to store integer, real, string, boolean and a handle simultaneously on same key pair
        - explicit boolean return for following natives: IsUnitInGroup, IsUnitGroupEmptyBJ, BlzForceHasPlayer, IsPlayerInForce, IsUnitInForce
        - GroupPickRandomUnit will no longer return 0 if group is empty
        - swapped FlushChildHashtableBJ arguments to match the Blizzard.j signature
        - type override to return 'userdata' for FakeLocation, FakeRect, FakeGroup, FakeForce and FakeHashtable
        - added Debug.beginFile/endFile
        - added EmmyLua annotations
        - stored the 4 timers defined in Lua root by Blizzard.j so that their references never get lost and the objects never get collected by GC which ultimately causes desyncs
        - WC3 Native Math API replaced with Lua's math API
        - SubStringBJ replaced with string.sub

    Uses optionally:
        https://github.com/BribeFromTheHive/Lua-Core/blob/main/Total_Initialization.lua
        https://github.com/BribeFromTheHive/Lua-Core/blob/main/Hook.lua
        https://github.com/BribeFromTheHive/Lua-Core/blob/main/Global_Variable_Remapper.lua
        https://github.com/BribeFromTheHive/Lua-Core/blob/main/UnitEvent.lua
--]]
GUI = {}
do
    --Configurables
    local _THROW_ERROR_ON_INVALID_ARG   = true -- set to true if you want LIGUI to throw errors when incorrect arguments are sent to overriden functions
    local _PRINT_WARNING_ON_INVALID_ARG = true  -- set to true if you want warnings by LIGUI when incorrect arguments are sent to overriden functions
    local _USE_GLOBAL_REMAP             = false -- set to true if you want GUI to have extended functionality such as "udg_HashTableArray" (which gives GUI an infinite supply of shared hashtables)
    local _USE_UNIT_EVENT               = false -- set to true if you have UnitEvent in your map and want to automatically remove units from their unit groups if they are removed from the game.

    --Define common variables to be utilized throughout the script.
    local unpack                        = table.unpack
    local assert                        = assert
    -- Used to check if function should exit early due to invalid arguments, instead of executing its internal logic
    local check                         = (function() ---@type fun(condition:boolean, msg: string): shouldEarlyExit: boolean 
        if _THROW_ERROR_ON_INVALID_ARG then
            return function(condition, msg)
                return not assert(condition, msg)
            end
        elseif _PRINT_WARNING_ON_INVALID_ARG then
            return function(condition, msg)
                if not condition then
                    if Debug then
                        Debug.errorHandler("LIGUI: " .. msg, 3)
                    else
                        print("|cFFFF0000LIGUI: " .. msg)
                    end
                end
                return not condition
            end
        else
            return function(condition)
                return not condition
            end
        end
    end)()

    ---@class FakedType
    ---@field __faketype string

    do
        local oldType = type
        --[[ Type extender - if object being checked is a table, check if it's one of the replacements for userdata --]]
        ---@param obj unknown
        ---@return string typeName
        function type(obj)
            local thisType = oldType(obj)
            if thisType == 'table' and obj.__faketype and oldType(obj.__faketype) == 'string' then
                return obj --[[@as FakedType]].__faketype
            end
            return thisType
        end
    end

    --[[-----------------------------------------------------------------------------------------
        __jarray expander by Bribe

        This snippet will ensure that objects used as indices in udg_ arrays will be automatically
        cleaned up when the garbage collector runs, and tries to re-use metatables whenever possible.
        -------------------------------------------------------------------------------------------]]
    do
        local mts = {}
        local weakKeys = { __mode = "k" } --ensures tables with non-nilled objects as keys will be garbage collected.

        ---Re-define __jarray.
        ---@param default? any
        ---@param tab? table
        ---@return table
        function __jarray(default, tab)
            local mt
            if default then
                mts[default] = mts[default] or {
                    __index = function()
                        return default
                    end,
                    __mode = "k"
                }
                mt = mts[default]
            else
                mt = weakKeys
            end
            return setmetatable(tab or {}, mt)
        end

        --have to do a wide search for all arrays in the variable editor. The WarCraft 3 _ENV table is HUGE,
        --and without editing the war3map.lua file manually, it is not possible to rewrite it in advance.
        for k, v in pairs(_ENV) do
            if type(v) == "table" and string.sub(k, 1, 4) == "udg_" then
                __jarray(v[0], v)
            end
        end
        ---Add this safe iterator function for jarrays.
        ---@param whichTable table
        ---@param func fun(index:integer, value:any)
        function GUI.loopArray(whichTable, func)
            for i = rawget(whichTable, 0) ~= nil and 0 or 1, #whichTable do
                func(i, rawget(whichTable, i))
            end
        end
    end
    --[=============[
      • HASHTABLES •
    --]=============]
    --[[ GUI hashtable converter by Tasyen and Bribe

        Converts GUI hashtables API into Lua Tables, overwrites StringHashBJ and GetHandleIdBJ to permit
        typecasting, bypasses the 256 hashtable limit by avoiding hashtables, provides the variable
        "HashTableArray", which automatically creates hashtables for you as needed (so you don't have to
        initialize them each time). ]]
    do
        ---@param s string
        ---@return string s
        function StringHashBJ(s)
            return s or 0
        end

        ---@generic T
        ---@param id T
        ---@return T id
        function GetHandleIdBJ(id)
            return id or 0
        end

        ---@alias FakeHashtableBucket<T> {[unknown]: {[unknown]: T}}
        ---@class FakeHashtable: FakedType
        ---@field boolean FakeHashtableBucket<boolean>
        ---@field integer FakeHashtableBucket<integer>
        ---@field real FakeHashtableBucket<real>
        ---@field string FakeHashtableBucket<string>
        ---@field handle FakeHashtableBucket<handle>

        ---@param whichHashTable FakeHashtable
        ---@param type 'boolean'|'integer'|'real'|'string'|'handle'
        ---@param parentKey unknown
        ---@return unknown
        local function load(whichHashTable, type, parentKey)
            local typedTable = whichHashTable[type]
            if not typedTable then
                typedTable = {}
                whichHashTable[type] = typedTable
            end
            local index = typedTable[parentKey]
            if not index then
                index = __jarray()
                typedTable[parentKey] = index
            end
            return index
        end
        if _USE_GLOBAL_REMAP then
            OnInit(function(import)
                local remap = import "GlobalRemapArray"
                local hashes = __jarray()
                remap("udg_HashTableArray", function(index)
                    return load(hashes, 'handle', index)
                end)
            end)
        end

        ---@param whichHashTable FakeHashtable
        ---@param parentKey unknown
        ---@param childKey unknown
        ---@return boolean shouldEarlyExit
        local function checkHashtableArgs(whichHashTable, parentKey, childKey)
            return check(whichHashTable ~= nil, 'whichHashTable cannot be nil') or
                check(parentKey ~= nil, 'parentKey cannot be nil') or
                check(childKey ~= nil, 'childKey cannot be nil')
        end

        ---@return FakeHashtable
        function InitHashtable()
            return { __faketype = "userdata" }
        end

        ---@param value unknown?
        ---@param childKey unknown
        ---@param parentKey unknown
        ---@param whichHashTable FakeHashtable
        ---@param type 'boolean'|'integer'|'real'|'string'|'handle'
        local function saveInto(whichHashTable, type, parentKey, childKey, value)
            if checkHashtableArgs(whichHashTable, parentKey, childKey) then return end
            load(whichHashTable, type, parentKey)[childKey] = value
        end

        ---@generic T
        ---@param type 'boolean'|'integer'|'real'|'string'|'handle'
        ---@return fun(whichHashTable: FakeHashtable, parentKey: unknown, childKey: unknown, value: T)
        local function createSaveIntoTyped(type)
            check(type ~= nil, 'type cannot be nil')
            ---@generic T
            ---@param whichHashTable FakeHashtable
            ---@param parentKey unknown
            ---@param childKey unknown
            ---@param value T
            return function(whichHashTable, parentKey, childKey, value)
                return saveInto(whichHashTable, type, parentKey, childKey, value)
            end
        end

        SaveInteger = createSaveIntoTyped('integer') ---@type fun(whichHashtable: FakeHashtable, parentKey: unknown, childKey: unknown, value: integer)
        SaveReal = createSaveIntoTyped('real') ---@type fun(whichHashtable: FakeHashtable, parentKey: unknown, childKey: unknown, value: number)
        SaveBoolean = createSaveIntoTyped('boolean') ---@type fun(whichHashtable: FakeHashtable, parentKey: unknown, childKey: unknown, value: boolean)
        SaveStr = createSaveIntoTyped('string') ---@type fun(whichHashtable: FakeHashtable, parentKey: unknown, childKey: unknown, value: string)
        local saveHandle = createSaveIntoTyped('handle')

        ---@param whichHashTable FakeHashtable
        ---@param type 'boolean'|'integer'|'real'|'string'|'handle'
        ---@param parentKey unknown
        ---@param childKey unknown
        ---@param default unknown|nil
        ---@return unknown|nil
        local function loadFrom(whichHashTable, type, parentKey, childKey, default)
            if checkHashtableArgs(whichHashTable, parentKey, childKey) then return default end
            local val = load(whichHashTable, type, parentKey)[childKey]
            return val ~= nil and val or default
        end

        ---@param type 'boolean'|'integer'|'real'|'string'|'handle'|nil
        ---@param default unknown
        ---@return fun(whichHashTable: FakeHashtable, parentKey: unknown, childKey: unknown): unknown|nil
        local function createDefault(type, default)
            return function(whichHashTable, parentKey, childKey)
                return loadFrom(whichHashTable, type or 'handle', parentKey, childKey, default)
            end
        end
        LoadInteger = createDefault('integer', 0) ---@type fun(whichHashTable: FakeHashtable, parentKey: unknown, childKey: unknown): integer
        LoadReal = createDefault('real', 0) ---@type fun(whichHashTable: FakeHashtable, parentKey: unknown, childKey: unknown): number
        LoadBoolean = createDefault('boolean', false) ---@type fun(whichHashTable: FakeHashtable, parentKey: unknown, childKey: unknown): boolean
        LoadStr = createDefault('string', '') ---@type fun(whichHashTable: FakeHashtable, parentKey: unknown, childKey: unknown): string
        local loadHandle = createDefault('handle', nil)

        do
            local sub = string.sub
            for key in pairs(_ENV) do
                if sub(key, -6) == "Handle" then
                    local str = sub(key, 1, 4)
                    if str == "Save" then
                        _ENV[key] = saveHandle
                    elseif str == "Load" then
                        _ENV[key] = loadHandle
                    end
                end
            end
        end

        ---@param whichHashTable FakeHashtable
        ---@param parentKey unknown
        ---@param childKey unknown
        ---@return boolean
        function HaveSavedBoolean(whichHashTable, parentKey, childKey)
            if checkHashtableArgs(whichHashTable, parentKey, childKey) then return false end
            return load(whichHashTable, parentKey, 'boolean')[childKey] ~= nil
        end

        ---@param whichHashTable FakeHashtable
        ---@param parentKey unknown
        ---@param childKey unknown
        ---@return boolean
        function HaveSavedInteger(whichHashTable, parentKey, childKey)
            if checkHashtableArgs(whichHashTable, parentKey, childKey) then return false end
            return load(whichHashTable, parentKey, 'integer')[childKey] ~= nil
        end

        ---@param whichHashTable FakeHashtable
        ---@param parentKey unknown
        ---@param childKey unknown
        ---@return boolean
        function HaveSavedReal(whichHashTable, parentKey, childKey)
            if checkHashtableArgs(whichHashTable, parentKey, childKey) then return false end
            return load(whichHashTable, parentKey, 'real')[childKey] ~= nil
        end

        ---@param whichHashTable FakeHashtable
        ---@param parentKey unknown
        ---@param childKey unknown
        ---@return boolean
        function HaveSavedString(whichHashTable, parentKey, childKey)
            if checkHashtableArgs(whichHashTable, parentKey, childKey) then return false end
            return load(whichHashTable, parentKey, 'string')[childKey] ~= nil
        end

        ---@param whichHashTable FakeHashtable
        ---@param parentKey unknown
        ---@param childKey unknown
        ---@return boolean
        function HaveSavedHandle(whichHashTable, parentKey, childKey)
            if checkHashtableArgs(whichHashTable, parentKey, childKey) then return false end
            return load(whichHashTable, parentKey, 'handle')[childKey] ~= nil
        end

        ---@param whichHashTable FakeHashtable
        function FlushParentHashtable(whichHashTable)
            if check(whichHashTable ~= nil, 'whichHashTable cannot be nil') then return end
            whichHashTable.boolean = nil
            whichHashTable.integer = nil
            whichHashTable.real = nil
            whichHashTable.string = nil
            whichHashTable.handle = nil
        end

        ---@param whichHashTable FakeHashtable
        ---@param parentKey unknown
        function FlushChildHashtable(whichHashTable, parentKey)
            if check(whichHashTable ~= nil, 'whichHashTable cannot be nil') then return end
            if check(parentKey ~= nil, 'parentKey cannot be nil') then return end
            if whichHashTable.boolean then whichHashTable.boolean[parentKey] = nil end
            if whichHashTable.integer then whichHashTable.integer[parentKey] = nil end
            if whichHashTable.real then whichHashTable.real[parentKey] = nil end
            if whichHashTable.string then whichHashTable.string[parentKey] = nil end
            if whichHashTable.handle then whichHashTable.handle[parentKey] = nil end
        end
    end
    --[=============================[
      • GROUPS (UNIT GROUPS IN GUI) •
    --]=============================]
    do
        local mainGroup = bj_lastCreatedGroup
        local issueGroup = CreateGroup() --[[@as group]]
        DestroyGroup(bj_suspendDecayFleshGroup --[[@as group]])
        DestroyGroup(bj_suspendDecayBoneGroup --[[@as group]])
        DestroyGroup = DoNothing

        ---@class FakeGroup: FakedType
        ---@field [integer] unit
        ---@field indexOf {[unit]: integer}

        ---@return FakeGroup
        function CreateGroup()
            return { indexOf = {}, __faketype = "userdata" }
        end

        bj_lastCreatedGroup = CreateGroup()
        bj_suspendDecayFleshGroup = CreateGroup()
        bj_suspendDecayBoneGroup = CreateGroup()

        local oldGroupClear = GroupClear --[[@as fun(group: group)]]
        local oldGroupAddUnit = GroupAddUnit --[[@as fun(group: group, unit: unit)]]

        local groups ---@type table<unit, table<FakeGroup, boolean>>
        if _USE_UNIT_EVENT then
            groups = {}

            ---@param group FakeGroup
            function GroupClear(group)
                if check(group ~= nil, 'group cannot be nil') then return end
                local u
                for i = 1, #group do
                    u = group[i]
                    groups[u] = nil
                    group.indexOf[u] = nil
                    group[i] = nil
                end
            end
        else
            ---@param group FakeGroup
            function GroupClear(group)
                if check(group ~= nil, 'group cannot be nil') then return end
                for i = 1, #group do
                    group.indexOf[group[i]] = nil
                    group[i] = nil
                end
            end
        end

        ---@param group FakeGroup
        ---@param unit unit
        function GroupAddUnit(group, unit)
            if check(group ~= nil, 'group cannot be nil') then return end
            if check(unit ~= nil, 'unit cannot be nil') then return end
            if group.indexOf[unit] then return end

            local pos = #group + 1
            group.indexOf[unit] = pos
            group[pos] = unit
            if groups then
                groups[unit] = groups[unit] or __jarray()
                groups[unit][group] = true
            end
        end

        ---@param group FakeGroup
        ---@param unit unit
        function GroupRemoveUnit(group, unit)
            if check(group ~= nil, 'group cannot be nil') then return end
            if check(unit ~= nil, 'unit cannot be nil') then return end
            local indexOf = group.indexOf
            if indexOf == nil then return end
            local pos = indexOf[unit]
            if pos == nil then return end

            local size = #group
            if pos ~= size then
                local replUnit = group[size]
                group[pos] = replUnit
                indexOf[replUnit] = pos
            end
            group[size] = nil
            indexOf[unit] = nil
            if groups then
                groups[unit][group] = nil
            end
        end

        ---@param unit unit
        ---@param group FakeGroup
        ---@return boolean
        function IsUnitInGroup(unit, group)
            if check(unit ~= nil, 'unit cannot be nil') then return false end
            if check(group ~= nil, 'group cannot be nil') then return false end
            return group.indexOf[unit] and true or false
        end

        ---@param group FakeGroup
        ---@return unit|nil
        function FirstOfGroup(group)
            if check(group ~= nil, 'group cannot be nil') then return end
            return group[1]
        end

        local enumUnit
        ---@return unit enumUnit
        function GetEnumUnit()
            return enumUnit
        end

        ---@param group FakeGroup
        ---@param code fun(u: unit)
        function GUI.forGroup(group, code)
            if check(group ~= nil, 'group cannot be nil') then return end
            if check(code ~= nil, 'code cannot be nil') then return end
            for i = 1, #group do
                code(group[i])
            end
        end

        ---@param group FakeGroup
        ---@param code fun(u)
        function ForGroup(group, code)
            if check(group ~= nil, 'group cannot be nil') then return end
            if check(code ~= nil, 'code cannot be nil') then return end
            local old = enumUnit
            GUI.forGroup(group, function(unit)
                enumUnit = unit
                code()
            end)
            enumUnit = old
        end

        do
            local oldUnitAt = BlzGroupUnitAt

            ---@param group FakeGroup
            ---@param index integer
            ---@return unit|nil
            function BlzGroupUnitAt(group, index)
                if check(group ~= nil, 'group cannot be nil') then return nil end
                if check(index ~= nil, 'index cannot be nil') then return nil end
                return group[index + 1]
            end

            local oldGetSize = BlzGroupGetSize

            ---@param code fun(u: unit)
            local function groupAction(code)
                for i = 0, oldGetSize(mainGroup) - 1 do
                    code(oldUnitAt(mainGroup, i) --[[@as unit should be fine]])
                end
            end
            for _, name in ipairs({
                "OfType",
                "OfPlayer",
                "OfTypeCounted",
                "InRect",
                "InRectCounted",
                "InRange",
                "InRangeOfLoc",
                "InRangeCounted",
                "InRangeOfLocCounted",
                "Selected"
            }) do
                local varStr = "GroupEnumUnits" .. name
                local old = _ENV[varStr]

                ---@param group FakeGroup
                ---@param ... unknown
                _ENV[varStr] = function(group, ...)
                    if group then
                        old(mainGroup, ...)
                        GroupClear(group)
                        groupAction(function(unit)
                            GroupAddUnit(group, unit)
                        end)
                    end
                end
                --Provide API for Lua users who just want to efficiently run code, without caring about the group itself.
                ---@param code fun(group: FakeGroup, ...: unknown)
                ---@param ... unknown
                GUI["enumUnits" .. name] = function(code, ...)
                    if check(code ~= nil, 'code cannot be nil') then return end
                    old(mainGroup, ...)
                    groupAction(code)
                end
            end
        end

        for _, name in ipairs {
            "GroupImmediateOrder",
            "GroupImmediateOrderById",
            "GroupPointOrder",
            "GroupPointOrderById",
            "GroupTargetOrder",
            "GroupTargetOrderById"
        } do
            local old = _ENV[name]
            ---@param group FakeGroup
            ---@param ... unknown
            ---@return boolean
            _ENV[name] = function(group, ...)
                if check(group ~= nil, ' group cannot be nil') then return false end
                oldGroupClear(issueGroup)
                for _, unit in ipairs(group) do
                    oldGroupAddUnit(issueGroup, unit)
                end
                return old(issueGroup, ...)
            end
        end

        ---@param group FakeGroup
        ---@return integer
        function BlzGroupGetSize(group)
            if check(group ~= nil, 'group cannot be nil') then return 0 end
            return #group
        end

        ---@param group FakeGroup
        ---@param add FakeGroup
        function GroupAddGroup(add, group)
            if check(group ~= nil, 'group cannot be nil') then return end
            if check(add ~= nil, 'add cannot be nil') then return end
            GUI.forGroup(add, function(unit)
                GroupAddUnit(group, unit)
            end)
        end

        ---@param group FakeGroup
        ---@param remove FakeGroup
        function GroupRemoveGroup(remove, group)
            if check(group ~= nil, 'group cannot be nil') then return end
            if check(remove ~= nil, 'remove cannot be nil') then return end
            GUI.forGroup(remove, function(unit)
                GroupRemoveUnit(group, unit)
            end)
        end

        ---@param group FakeGroup
        ---@return unit|nil
        function GroupPickRandomUnit(group)
            if check(group ~= nil, 'group cannot be nil') then return nil end
            return group[1] and group[GetRandomInt(1, #group)]
        end

        ---@param group FakeGroup
        ---@return boolean
        function IsUnitGroupEmptyBJ(group)
            if check(group ~= nil, 'group cannot be nil') then return true end -- if it's a nil group, I'm sure the appropriate logic is to say it's empty?
            return not group[1]
        end

        ForGroupBJ = ForGroup
        CountUnitsInGroup = BlzGroupGetSize
        BlzGroupAddGroupFast = GroupAddGroup
        BlzGroupRemoveGroupFast = GroupRemoveGroup
        GroupPickRandomUnitEnum = nil
        CountUnitsInGroupEnum = nil
        GroupAddGroupEnum = nil
        GroupRemoveGroupEnum = nil

        if groups then
            OnInit(function(import)
                import "UnitEvent"
                ---@param data {unit: unit}
                UnitEvent.onRemoval(function(data)
                    local u = data.unit
                    local g = groups[u]
                    if g then
                        for group, _ in pairs(g) do
                            GroupRemoveUnit(group, u)
                        end
                    end
                end)
            end)
        end
    end

    --[===========================[
      • LOCATIONS (POINTS IN GUI) •
    --]===========================]
    do
        ---@class FakeLocation: FakedType
        ---@field [1] number x
        ---@field [2] number y

        local oldLocation = Location
        local location

        ---@param x number
        ---@param y number
        ---@return FakeLocation
        function Location(x, y)
            if check(x ~= nil, 'x cannot be nil') then return { 0.00, 0.00, __faketype = 'userdata' } end
            if check(y ~= nil, 'y cannot be nil') then return { 0.00, 0.00, __faketype = 'userdata' } end
            return { x, y, __faketype = "userdata" }
        end

        do
            local oldRemove = RemoveLocation
            local oldGetX   = GetLocationX
            local oldGetY   = GetLocationY
            local oldRally  = GetUnitRallyPoint

            ---@param unit unit
            ---@return FakeLocation?
            function GetUnitRallyPoint(unit)
                if check(unit ~= nil, 'unit cannot be nil') then return nil end -- no unit, no rally
                local removeThis = oldRally(unit)                                    --Actually needs to create a location for a brief moment, as there is no GetUnitRallyX/Y
                if removeThis == nil then return nil end                             -- in case there's no rally
                local loc = Location(oldGetX(removeThis), oldGetY(removeThis))
                oldRemove(removeThis)
                return loc
            end
        end

        RemoveLocation = DoNothing ---@type fun(location: FakeLocation)

        do
            local oldMoveLoc = MoveLocation
            local oldGetZ = GetLocationZ

            ---@param x number
            ---@param y number
            ---@return number z
            function GUI.getCoordZ(x, y)
                function GUI.getCoordZ(x, y)
                    if check(x ~= nil, 'x cannot be nil') then return 0 end
                    if check(y ~= nil, 'y cannot be nil') then return 0 end
                    oldMoveLoc(location, x, y)
                    return oldGetZ(location)
                end

                location = oldLocation(x, y)
                return GUI.getCoordZ(x, y)
            end
        end

        ---@param loc FakeLocation
        ---@return number x
        function GetLocationX(loc)
            if check(loc ~= nil, 'loc cannot be nil') then return 0 end
            return loc[1]
        end

        ---@param loc FakeLocation
        ---@return number y
        function GetLocationY(loc)
            if check(loc ~= nil, 'loc cannot be nil') then return 0 end
            return loc[2]
        end

        ---@param loc FakeLocation
        ---@return number z
        function GetLocationZ(loc)
            if check(loc ~= nil, 'loc cannot be nil') then return 0 end
            return GUI.getCoordZ(loc[1], loc[2])
        end

        ---@param loc FakeLocation
        ---@param x number
        ---@param y number
        function MoveLocation(loc, x, y)
            if check(loc ~= nil, 'loc cannot be nil') then return end
            loc[1] = x
            loc[2] = y
        end

        ---@param varName string
        ---@param suffix string|nil
        local function fakeCreate(varName, suffix)
            local getX = _ENV[varName .. "X"]
            local getY = _ENV[varName .. "Y"]
            _ENV[varName .. (suffix or "Loc")] = function(obj) return Location(getX(obj), getY(obj)) end
        end
        fakeCreate("GetUnit")
        fakeCreate("GetOrderPoint")
        fakeCreate("GetSpellTarget")
        fakeCreate("CameraSetupGetDestPosition")
        fakeCreate("GetCameraTargetPosition")
        fakeCreate("GetCameraEyePosition")
        fakeCreate("BlzGetTriggerPlayerMouse", "Position")
        fakeCreate("GetStartLocation")

        ---@param effect effect
        ---@param loc FakeLocation
        function BlzSetSpecialEffectPositionLoc(effect, loc)
            if check(effect ~= nil, 'effect cannot be nil') then return end
            if check(loc ~= nil, 'loc cannot be nil') then return end
            local x, y = loc[1], loc[2]
            BlzSetSpecialEffectPosition(effect, x, y, GUI.getCoordZ(x, y))
        end

        ---@param oldVarName string
        ---@param newVarName string
        ---@param index integer needed to determine which of the parameters calls for a location.
        local function hook(oldVarName, newVarName, index)
            local new = _ENV[newVarName]
            local func

            local errorMsgIndex1 = 'Function ' .. oldVarName .. '\'s argument #1 - location cannot be nil!'
            local errorMsgIndex2 = 'Function ' .. oldVarName .. '\'s argument #2 - location cannot be nil!'
            local errorMsgIndex3 = 'Function ' .. oldVarName .. '\'s argument #3 - location cannot be nil!'

            if index == 1 then
                func = function(loc, ...)
                    if check(loc ~= nil, errorMsgIndex1) then return new(0, 0, ...) end
                    return new(loc[1], loc[2], ...)
                end
            elseif index == 2 then
                func = function(a, loc, ...)
                    if check(loc ~= nil, errorMsgIndex2) then return new(a, 0, 0, ...) end
                    return new(a, loc[1], loc[2], ...)
                end
            else --index==3
                func = function(a, b, loc, ...)
                    if check(loc ~= nil, errorMsgIndex3) then return new(a, b, 0, 0, ...) end
                    return new(a, b, loc[1], loc[2], ...)
                end
            end
            _ENV[oldVarName] = func
        end
        hook("IsLocationInRegion", "IsPointInRegion", 2)
        hook("IsUnitInRangeLoc", "IsUnitInRangeXY", 2)
        hook("IssuePointOrderLoc", "IssuePointOrder", 3)
        IssuePointOrderLocBJ = IssuePointOrderLoc
        hook("IssuePointOrderByIdLoc", "IssuePointOrderById", 3)
        hook("IsLocationVisibleToPlayer", "IsVisibleToPlayer", 1)
        hook("IsLocationFoggedToPlayer", "IsFoggedToPlayer", 1)
        hook("IsLocationMaskedToPlayer", "IsMaskedToPlayer", 1)
        hook("CreateFogModifierRadiusLoc", "CreateFogModifierRadius", 3)
        hook("AddSpecialEffectLoc", "AddSpecialEffect", 2)
        hook("AddSpellEffectLoc", "AddSpellEffect", 3)
        hook("AddSpellEffectByIdLoc", "AddSpellEffectById", 3)
        hook("SetBlightLoc", "SetBlight", 2)
        hook("DefineStartLocationLoc", "DefineStartLocation", 2)
        hook("GroupEnumUnitsInRangeOfLoc", "GroupEnumUnitsInRange", 2)
        hook("GroupEnumUnitsInRangeOfLocCounted", "GroupEnumUnitsInRangeCounted", 2)
        hook("GroupPointOrderLoc", "GroupPointOrder", 3)
        GroupPointOrderLocBJ = GroupPointOrderLoc
        hook("GroupPointOrderByIdLoc", "GroupPointOrderById", 3)
        hook("MoveRectToLoc", "MoveRectTo", 2)
        hook("RegionAddCellAtLoc", "RegionAddCell", 2)
        hook("RegionClearCellAtLoc", "RegionClearCell", 2)
        hook("CreateUnitAtLoc", "CreateUnit", 3)
        hook("CreateUnitAtLocByName", "CreateUnitByName", 3)
        hook("SetUnitPositionLoc", "SetUnitPosition", 2)
        hook("ReviveHeroLoc", "ReviveHero", 2)
        hook("SetFogStateRadiusLoc", "SetFogStateRadius", 3)
        hook('CreateMinimapIconAtLoc', 'CreateMinimapIcon', 1)

        ---@param min FakeLocation
        ---@param max FakeLocation
        ---@return FakeRect newRect
        function RectFromLoc(min, max)
            if check(min ~= nil, 'min cannot be nil') then return nil end
            if check(max ~= nil, 'max cannot be nil') then return nil end
            return Rect(min[1], min[2], max[1], max[2]) --[[@as FakeRect]]
        end

        ---@param whichRect FakeRect
        ---@param min FakeLocation
        ---@param max FakeLocation
        function SetRectFromLoc(whichRect, min, max)
            if check(min ~= nil, 'min cannot be nil') then return end
            if check(max ~= nil, 'max cannot be nil') then return end
            SetRect(whichRect, min[1], min[2], max[1], max[2])
        end
    end

    --[========================[
      • RECTS (REGIONS IN GUI) •
    --]========================]
    do
        ---@class FakeRect: FakedType
        ---@field [1] number minX
        ---@field [2] number minY
        ---@field [3] number maxX
        ---@field [4] number maxY

        local oldRect, rect = Rect, nil
        ---@param minX number
        ---@param minY number
        ---@param maxX number
        ---@param maxY number
        ---@return FakeRect
        function Rect(minX, minY, maxX, maxY)
            if check(minX ~= nil, 'minX cannot be nil') then return { 0, 0, 0, 0, __faketype = "userdata" } end
            if check(minY ~= nil, 'minY cannot be nil') then return { 0, 0, 0, 0, __faketype = "userdata" } end
            if check(maxX ~= nil, 'maxX cannot be nil') then return { 0, 0, 0, 0, __faketype = "userdata" } end
            if check(maxY ~= nil, 'maxY cannot be nil') then return { 0, 0, 0, 0, __faketype = "userdata" } end
            return { minX, minY, maxX, maxY, __faketype = "userdata" }
        end

        local oldSetRect = SetRect
        ---@param rect FakeRect
        ---@param minX number
        ---@param minY number
        ---@param maxX number
        ---@param maxY number
        function SetRect(rect, minX, minY, maxX, maxY)
            if check(rect ~= nil, 'rect cannot be nil') then return end
            if check(minX ~= nil, 'minX cannot be nil') then return end
            if check(minY ~= nil, 'minY cannot be nil') then return end
            if check(maxX ~= nil, 'maxX cannot be nil') then return end
            if check(maxY ~= nil, 'maxY cannot be nil') then return end
            rect[1] = minX
            rect[2] = minY
            rect[3] = maxX
            rect[4] = maxY
        end

        do
            local oldWorld = GetWorldBounds
            local getMinX = GetRectMinX
            local getMinY = GetRectMinY
            local getMaxX = GetRectMaxX
            local getMaxY = GetRectMaxY
            local remover = RemoveRect
            RemoveRect = DoNothing
            local newWorld

            ---@return FakeRect
            function GetWorldBounds()
                if not newWorld then
                    local w = oldWorld() --[[@as rect]]
                    newWorld = Rect(getMinX(w), getMinY(w), getMaxX(w), getMaxY(w))
                    remover(w)
                end
                return Rect(unpack(newWorld))
            end

            GetEntireMapRect = GetWorldBounds
        end

        ---@param rect FakeRect
        ---@return number
        function GetRectMinX(rect)
            if check(rect ~= nil, 'rect cannot be nil') then return 0 end
            return rect[1]
        end

        ---@param rect FakeRect
        ---@return number
        function GetRectMinY(rect)
            if check(rect ~= nil, 'rect cannot be nil') then return 0 end
            return rect[2]
        end

        ---@param rect FakeRect
        ---@return number
        function GetRectMaxX(rect)
            if check(rect ~= nil, 'rect cannot be nil') then return 0 end
            return rect[3]
        end

        ---@param rect FakeRect
        ---@return number
        function GetRectMaxY(rect)
            if check(rect ~= nil, 'rect cannot be nil') then return 0 end
            return rect[4]
        end

        ---@param rect FakeRect
        ---@return number
        function GetRectCenterX(rect)
            if check(rect ~= nil, 'rect cannot be nil') then return 0 end
            return (rect[1] + rect[3]) / 2
        end

        ---@param rect FakeRect
        ---@return number
        function GetRectCenterY(rect)
            if check(rect ~= nil, 'rect cannot be nil') then return 0 end
            return (rect[2] + rect[4]) / 2
        end

        ---@param rect FakeRect
        ---@param x number
        ---@param y number
        function MoveRectTo(rect, x, y)
            if check(rect ~= nil, 'rect cannot be nil') then return end
            if check(x ~= nil, 'x cannot be nil') then return end
            if check(y ~= nil, 'y cannot be nil') then return end
            x = x - GetRectCenterX(rect)
            y = y - GetRectCenterY(rect)
            SetRect(rect, rect[1] + x, rect[2] + y, rect[3] + x, rect[4] + y)
        end

        ---@param varName string
        ---@param index integer needed to determine which of the parameters calls for a rect.
        local function hook(varName, index)
            local old = _ENV[varName]
            local func

            local errorMsgIndex1 = 'Function ' .. varName .. '\'s argument #1 - rect cannot be nil!'
            local errorMsgIndex2 = 'Function ' .. varName .. '\'s argument #2 - rect cannot be nil!'
            local errorMsgIndex3 = 'Function ' .. varName .. '\'s argument #3 - rect cannot be nil!'
            if index == 1 then
                func = function(rct, ...)
                    if check(rct ~= nil, errorMsgIndex1) then
                        oldSetRect(rect --[[@as rect]], 0, 0, 0, 0)
                    else
                        oldSetRect(rect --[[@as rect]], unpack(rct))
                    end
                    return old(rect, ...)
                end
            elseif index == 2 then
                func = function(a, rct, ...)
                    if check(rct ~= nil, errorMsgIndex2) then
                        oldSetRect(rect --[[@as rect]], 0, 0, 0, 0)
                    else
                        oldSetRect(rect --[[@as rect]], unpack(rct))
                    end
                    return old(a, rect, ...)
                end
            else --index==3
                func = function(a, b, rct, ...)
                    if check(rct ~= nil, errorMsgIndex3) then
                        oldSetRect(rect --[[@as rect]], 0, 0, 0, 0)
                    else
                        oldSetRect(rect --[[@as rect]], unpack(rct))
                    end
                    return old(a, b, rect, ...)
                end
            end

            ---@param ... unknown
            _ENV[varName] = function(...)
                if not rect then rect = oldRect(0, 0, 32, 32) end
                _ENV[varName] = func
                return func(...)
            end
        end
        hook("EnumDestructablesInRect", 1)
        hook("EnumItemsInRect", 1)
        hook("AddWeatherEffect", 1)
        hook("SetDoodadAnimationRect", 1)
        hook("GroupEnumUnitsInRect", 2)
        hook("GroupEnumUnitsInRectCounted", 2)
        hook("RegionAddRect", 2)
        hook("RegionClearRect", 2)
        hook("SetBlightRect", 2)
        hook("SetFogStateRect", 3)
        hook("CreateFogModifierRect", 3)
    end
    --[===============================[
      • FORCES (PLAYER GROUPS IN GUI) •
    --]===============================]
    do
        ---@class FakeForce: FakedType
        ---@field [integer] player
        ---@field indexOf {[player]: integer}

        local oldForce, mainForce = CreateForce, nil
        local function initForce()
            initForce = DoNothing
            mainForce = oldForce()
        end

        ---@return FakeForce
        function CreateForce()
            return { indexOf = {}, __faketype = "userdata" }
        end

        DestroyForce = DoNothing ---@type fun(force: FakeForce)
        local oldClear = ForceClear

        ---@param force FakeForce
        function ForceClear(force)
            if check(force ~= nil, 'force cannot be nil') then return end
            for i, val in ipairs(force) do
                force.indexOf[val] = nil
                force[i] = nil
            end
        end

        do
            local oldCripple = CripplePlayer
            local oldAdd = ForceAddPlayer

            ---@param player player
            ---@param force FakeForce
            ---@param flag boolean
            function GUI.cripplePlayer(player, force, flag)
                function GUI.cripplePlayer(player, force, flag)
                    for _, val in ipairs(force) do
                        oldAdd(mainForce --[[@ as force]], val)
                    end
                    oldCripple(player, mainForce --[[@ as force]], flag)
                    oldClear(mainForce --[[@ as force]])
                end

                initForce()
                GUI.cripplePlayer(player, force, flag)
            end

            ---@param player player
            ---@param force FakeForce
            ---@param flag boolean
            function CripplePlayer(player, force, flag)
                if check(player ~= nil, 'player cannot be nil') then return end
                if check(force ~= nil, 'force cannot be nil') then return end
                GUI.cripplePlayer(player, force, flag)
            end
        end

        ---@param force FakeForce
        ---@param player player
        function ForceAddPlayer(force, player)
            if check(force ~= nil, 'force cannot be nil') then return end
            if check(player ~= nil, 'player cannot be nil') then return end
            if force.indexOf[player] then return end

            local pos = #force + 1
            force.indexOf[player] = pos
            force[pos] = player
        end

        ---@param force FakeForce
        ---@param player player
        function ForceRemovePlayer(force, player)
            if check(force ~= nil, 'force cannot be nil') then return end
            if check(player ~= nil, 'player cannot be nil') then return end
            local pos = force.indexOf[player]
            if pos == nil then return end

            force.indexOf[player] = nil
            local top = #force
            if pos ~= top then
                force[pos] = force[top]
                force.indexOf[force[top]] = pos
            end
            force[top] = nil
        end

        ---@param force FakeForce
        ---@param player player
        ---@return boolean
        function BlzForceHasPlayer(force, player)
            if check(force ~= nil, 'force cannot be nil') then return false end
            if check(player ~= nil, 'player cannot be nil') then return false end
            return force.indexOf[player] and true or false
        end

        ---@param player player
        ---@param force FakeForce
        ---@return boolean
        function IsPlayerInForce(player, force)
            if check(player ~= nil, 'player cannot be nil') then return false end
            if check(force ~= nil, 'force cannot be nil') then return false end
            return force.indexOf[player] and true or false
        end

        ---@param unit unit
        ---@param force FakeForce
        ---@return boolean
        function IsUnitInForce(unit, force)
            if check(unit ~= nil, 'unit cannot be nil') then return false end
            if check(force ~= nil, 'force cannot be nil') then return false end
            return force.indexOf[GetOwningPlayer(unit)] and true or false
        end

        local enumPlayer
        local oldForForce = ForForce
        local oldEnumPlayer = GetEnumPlayer

        ---@return player
        function GetEnumPlayer()
            return enumPlayer
        end

        ---@param force FakeForce
        ---@param code function
        function ForForce(force, code)
            if check(force ~= nil, 'force cannot be nil') then return end
            if check(code ~= nil, 'code cannot be nil') then return end
            local old = enumPlayer
            for _, player in ipairs(force) do
                enumPlayer = player
                code()
            end
            enumPlayer = old
        end

        ---@param force FakeForce
        local function funnelEnum(force)
            if check(force ~= nil, 'force cannot be nil') then return end
            ForceClear(force)
            oldForForce(mainForce, function()
                ForceAddPlayer(force, oldEnumPlayer())
            end)
            oldClear(mainForce --[[@as force]])
        end
        ---@param varStr string
        local function hookEnum(varStr)
            local old = _ENV[varStr]
            local deferred
            function deferred(force, ...)
                function deferred(force, ...)
                    old(mainForce, ...)
                    funnelEnum(force)
                end

                initForce()
                _ENV[varStr](force, ...)
            end

            _ENV[varStr] = function(force, ...)
                if check(force ~= nil, 'force cannot be nil') then return end
                deferred(force, ...)
            end
        end
        hookEnum("ForceEnumPlayers")
        hookEnum("ForceEnumPlayersCounted")
        hookEnum("ForceEnumAllies")
        hookEnum("ForceEnumEnemies")
        ---@param force FakeForce
        ---@return integer
        function CountPlayersInForceBJ(force)
            if check(force ~= nil, 'force cannot be nil') then return 0 end
            return #force
        end

        CountPlayersInForceEnum = nil

        ---@param player player
        ---@return FakeForce
        function GetForceOfPlayer(player)
            if check(player ~= nil, 'player cannot be nil') then return nil end
            --No longer leaks. There was no reason to dynamically create forces to begin with.
            return bj_FORCE_PLAYER[GetPlayerId(player)]
        end
    end

    -- section on Blizzard.j desyncable objects
    do
        local desyncCausingTimer1 = bj_queuedExecTimeoutTimer
        local desyncCausingTimer2 = bj_delayedSuspendDecayTimer
        local desyncCausingTimer3 = bj_volumeGroupsTimer
        local desyncCausingTimer4 = bj_lastStartedTimer
        function GUI.__constantly_loaded()
            -- some nonsense lines to make sure this function "always" needs the relevant upvalues
            if desyncCausingTimer1 then return true end
            if desyncCausingTimer2 then return true end
            if desyncCausingTimer3 then return true end
            if desyncCausingTimer4 then return true end
        end
    end

    --Blizzard forgot to add this, but still enabled it for GUI. Therefore, I've extracted and simplified the code from DebugIdInteger2IdString
    ---@param value integer
    ---@return string
    function BlzFourCC2S(value)
        if value == nil then return "" end
        local result = ""
        for _ = 1, 4 do
            result = string.char(value % 256) .. result
            value = value // 256
        end
        return result
    end

    ---@param trig trigger
    ---@param r FakeRect
    function TriggerRegisterDestDeathInRegionEvent(trig, r)
        if check(trig ~= nil, 'trigger cannot be nil') then return end
        if check(r ~= nil, 'rect cannot be nil') then return end
        --Removes the limit on the number of destructables that can be registered.
        EnumDestructablesInRect(r, nil, function() TriggerRegisterDeathEvent(trig, GetEnumDestructable()) end)
    end

    IsUnitAliveBJ = UnitAlive --use the reliable native instead of the life checks

    ---@param u unit
    ---@return boolean
    function IsUnitDeadBJ(u)
        return not UnitAlive(u)
    end

    ---@param whichUnit unit
    ---@param propWindow number
    function SetUnitPropWindowBJ(whichUnit, propWindow)
        --Allows the Prop Window to be set to zero to allow unit movement to be suspended.
        SetUnitPropWindow(whichUnit, math.rad(propWindow))
    end

    if _USE_GLOBAL_REMAP then
        OnInit(function(import)
            import "GlobalRemap"
            GlobalRemap("udg_INFINITE_LOOP", function() return -1 end) --a readonly variable for infinite looping in GUI.
        end)
    end

    do
        local cache = __jarray()

        ---@param whichTrig trigger
        ---@return function
        function GUI.wrapTrigger(whichTrig)
            if check(whichTrig ~= nil, 'whichTrig cannot be nil') then return nil end
            local func = cache[whichTrig]
            if not func then
                func = function()
                    if IsTriggerEnabled(whichTrig) and TriggerEvaluate(whichTrig) then
                        TriggerExecute(whichTrig)
                    end
                end
                cache[whichTrig] = func
            end
            return func
        end
    end
    do
        --[[---------------------------------------------------------------------------------------------
            RegisterAnyPlayerUnitEvent by Bribe

            RegisterAnyPlayerUnitEvent cuts down on handle count for alread-registered events, plus has
            the benefit for Lua users to just use function calls.

            Adds a third parameter to the RegisterAnyPlayerUnitEvent function: "skip". If true, disables
            the specified event, while allowing a single function to run discretely. It also allows (if
            Global Variable Remapper is included) GUI to un-register a playerunitevent by setting
            udg_RemoveAnyUnitEvent to the trigger they wish to remove.

            The "return" value of RegisterAnyPlayerUnitEvent calls the "remove" method. The API, therefore,
            has been reduced to just this one function (in addition to the bj override).
        -----------------------------------------------------------------------------------------------]]
        local fStack, tStack, oldBJ = {}, {},
            TriggerRegisterAnyUnitEventBJ ---@type {[eventid]: function[]}, {[eventid]: trigger[]}

        ---@param event playerunitevent
        ---@param userFunc function
        ---@param skip boolean?
        function RegisterAnyPlayerUnitEvent(event, userFunc, skip)
            if check(event ~= nil, 'event cannot be nil') then return end
            if check(userFunc ~= nil, 'userFunc cannot be nil') then return end
            if skip then
                local t = tStack[event]
                if t and IsTriggerEnabled(t) then
                    DisableTrigger(t)
                    userFunc()
                    EnableTrigger(t)
                else
                    userFunc()
                end
            else
                local funcs, insertAt = fStack[event], 1
                if funcs then
                    insertAt = #funcs + 1
                    if insertAt == 1 then EnableTrigger(tStack[event]) end
                else
                    local t = CreateTrigger()
                    oldBJ(t, event)
                    tStack[event], funcs = t, {}
                    fStack[event] = funcs
                    TriggerAddCondition(t, Filter(function()
                        for _, func in ipairs(funcs) do func() end
                    end))
                end
                funcs[insertAt] = userFunc
                return function()
                    local total = #funcs
                    for i = 1, total do
                        if funcs[i] == userFunc then
                            if total == 1 then
                                DisableTrigger(tStack[event]) --no more events are registered, disable the event (for now).
                            elseif total > i then
                                funcs[i] = funcs[total]
                            end                --pop just the top index down to this vacant slot so we don't have to down-shift the entire stack.
                            funcs[total] = nil --remove the top entry.
                            return true
                        end
                    end
                end
            end
        end

        local trigFuncs
        ---@param trig trigger
        ---@param event playerunitevent
        ---@return function|nil
        function TriggerRegisterAnyUnitEventBJ(trig, event)
            if check(trig ~= nil, 'trig cannot be nil') then return nil end
            if check(event ~= nil, 'event cannot be nil') then return nil end
            local removeFunc = RegisterAnyPlayerUnitEvent(event, GUI.wrapTrigger(trig))
            if _USE_GLOBAL_REMAP then
                if not trigFuncs then
                    trigFuncs = __jarray()
                    GlobalRemap("udg_RemoveAnyUnitEvent", nil, function(t)
                        if trigFuncs[t] then
                            trigFuncs[t]()
                            trigFuncs[t] = nil
                        end
                    end)
                end
                trigFuncs[trig] = removeFunc
            end
            return removeFunc
        end
    end

    -- Modify to allow requests for negative hero stats, as per request from Tasyen.
    ---@param whichHero unit
    ---@param whichStat integer
    ---@param value integer
    function SetHeroStat(whichHero, whichStat, value)
        if check(whichStat ~= nil, 'whichStat cannot be nil') then return end
        if (whichStat == bj_HEROSTAT_STR) then
            SetHeroStr(whichHero, value, true)
        elseif (whichStat == bj_HEROSTAT_AGI) then
            SetHeroAgi(whichHero, value, true)
        elseif (whichStat == bj_HEROSTAT_INT) then
            SetHeroInt(whichHero, value, true)
        end
    end

    -- ExecuteFunc native is useless in Lua, so let's replace it:
    ---@param funcName string
    function ExecuteFunc(funcName)
        local func = _ENV[funcName]
        if func == nil then
            check(false, 'Function by the name ' .. funcName .. ' is not found!')
        else
            func()
        end
    end

    --The next part of the code is purely optional, as it is intended to optimize rather than add new functionality
    CommentString                        = nil
    RegisterDestDeathInRegionEnum        = nil

    --This next list comes from HerlySQR, and its purpose is to eliminate useless wrapper functions (only where the parameters aligned):
    StringIdentity                       = GetLocalizedString
    TriggerRegisterTimerExpireEventBJ    = TriggerRegisterTimerExpireEvent
    TriggerRegisterDialogEventBJ         = TriggerRegisterDialogEvent
    TriggerRegisterUpgradeCommandEventBJ = TriggerRegisterUpgradeCommandEvent
    RemoveWeatherEffectBJ                = RemoveWeatherEffect
    DestroyLightningBJ                   = DestroyLightning
    GetLightningColorABJ                 = GetLightningColorA
    GetLightningColorRBJ                 = GetLightningColorR
    GetLightningColorGBJ                 = GetLightningColorG
    GetLightningColorBBJ                 = GetLightningColorB
    SetLightningColorBJ                  = SetLightningColor
    GetAbilityEffectBJ                   = GetAbilityEffectById
    GetAbilitySoundBJ                    = GetAbilitySoundById
    ResetTerrainFogBJ                    = ResetTerrainFog
    SetSoundDistanceCutoffBJ             = SetSoundDistanceCutoff
    SetSoundPitchBJ                      = SetSoundPitch
    AttachSoundToUnitBJ                  = AttachSoundToUnit
    KillSoundWhenDoneBJ                  = KillSoundWhenDone
    PlayThematicMusicBJ                  = PlayThematicMusic
    EndThematicMusicBJ                   = EndThematicMusic
    StopMusicBJ                          = StopMusic
    ResumeMusicBJ                        = ResumeMusic
    VolumeGroupResetImmediateBJ          = VolumeGroupReset
    WaitForSoundBJ                       = TriggerWaitForSound
    ClearMapMusicBJ                      = ClearMapMusic
    DestroyEffectBJ                      = DestroyEffect
    GetItemLifeBJ                        = GetWidgetLife     -- This was just to type casting
    SetItemLifeBJ                        = SetWidgetLife     -- This was just to type casting
    --UnitRemoveBuffBJ                     = UnitRemoveAbility -- The buffs are abilities
    GetLearnedSkillBJ                    = GetLearnedSkill
    UnitDropItemPointBJ                  = UnitDropItemPoint
    UnitDropItemTargetBJ                 = UnitDropItemTarget
    UnitUseItemDestructable              = UnitUseItemTarget -- This was just to type casting
    UnitInventorySizeBJ                  = UnitInventorySize
    SetItemInvulnerableBJ                = SetItemInvulnerable
    SetItemDropOnDeathBJ                 = SetItemDropOnDeath
    SetItemDroppableBJ                   = SetItemDroppable
    SetItemPlayerBJ                      = SetItemPlayer
    ChooseRandomItemBJ                   = ChooseRandomItem
    ChooseRandomNPBuildingBJ             = ChooseRandomNPBuilding
    ChooseRandomCreepBJ                  = ChooseRandomCreep
    String2UnitIdBJ                      = UnitId -- I think they just wanted a better name
    GetIssuedOrderIdBJ                   = GetIssuedOrderId
    GetKillingUnitBJ                     = GetKillingUnit
    IsUnitHiddenBJ                       = IsUnitHidden
    IssueTrainOrderByIdBJ                = IssueImmediateOrderById -- I think they just wanted a better name
    IssueUpgradeOrderByIdBJ              = IssueImmediateOrderById -- I think they just wanted a better name
    GetAttackedUnitBJ                    = GetTriggerUnit          -- I think they just wanted a better name
    SetUnitFlyHeightBJ                   = SetUnitFlyHeight
    SetUnitTurnSpeedBJ                   = SetUnitTurnSpeed
    GetUnitDefaultPropWindowBJ           = GetUnitDefaultPropWindow
    SetUnitBlendTimeBJ                   = SetUnitBlendTime
    SetUnitAcquireRangeBJ                = SetUnitAcquireRange
    UnitSetCanSleepBJ                    = UnitAddSleep
    UnitCanSleepBJ                       = UnitCanSleep
    UnitWakeUpBJ                         = UnitWakeUp
    UnitIsSleepingBJ                     = UnitIsSleeping
    IsUnitPausedBJ                       = IsUnitPaused
    SetUnitExplodedBJ                    = SetUnitExploded
    GetTransportUnitBJ                   = GetTransportUnit
    GetLoadedUnitBJ                      = GetLoadedUnit
    IsUnitInTransportBJ                  = IsUnitInTransport
    IsUnitLoadedBJ                       = IsUnitLoaded
    IsUnitIllusionBJ                     = IsUnitIllusion
    SetDestructableInvulnerableBJ        = SetDestructableInvulnerable
    IsDestructableInvulnerableBJ         = IsDestructableInvulnerable
    SetDestructableMaxLifeBJ             = SetDestructableMaxLife
    WaygateIsActiveBJ                    = WaygateIsActive
    QueueUnitAnimationBJ                 = QueueUnitAnimation
    SetDestructableAnimationBJ           = SetDestructableAnimation
    QueueDestructableAnimationBJ         = QueueDestructableAnimation
    DialogSetMessageBJ                   = DialogSetMessage
    DialogClearBJ                        = DialogClear
    GetClickedButtonBJ                   = GetClickedButton
    GetClickedDialogBJ                   = GetClickedDialog
    DestroyQuestBJ                       = DestroyQuest
    QuestSetTitleBJ                      = QuestSetTitle
    QuestSetDescriptionBJ                = QuestSetDescription
    QuestSetCompletedBJ                  = QuestSetCompleted
    QuestSetFailedBJ                     = QuestSetFailed
    QuestSetDiscoveredBJ                 = QuestSetDiscovered
    QuestItemSetDescriptionBJ            = QuestItemSetDescription
    QuestItemSetCompletedBJ              = QuestItemSetCompleted
    DestroyDefeatConditionBJ             = DestroyDefeatCondition
    DefeatConditionSetDescriptionBJ      = DefeatConditionSetDescription
    FlashQuestDialogButtonBJ             = FlashQuestDialogButton
    DestroyTimerBJ                       = DestroyTimer
    DestroyTimerDialogBJ                 = DestroyTimerDialog
    TimerDialogSetTitleBJ                = TimerDialogSetTitle
    TimerDialogSetSpeedBJ                = TimerDialogSetSpeed
    --TimerDialogDisplayBJ                 = TimerDialogDisplay
    LeaderboardSetStyleBJ                = LeaderboardSetStyle
    LeaderboardGetItemCountBJ            = LeaderboardGetItemCount
    LeaderboardHasPlayerItemBJ           = LeaderboardHasPlayerItem
    DestroyLeaderboardBJ                 = DestroyLeaderboard
    --LeaderboardDisplayBJ                 = LeaderboardDisplay
    LeaderboardSortItemsByPlayerBJ       = LeaderboardSortItemsByPlayer
    LeaderboardSortItemsByLabelBJ        = LeaderboardSortItemsByLabel
    PlayerGetLeaderboardBJ               = PlayerGetLeaderboard
    DestroyMultiboardBJ                  = DestroyMultiboard
    SetTextTagPosUnitBJ                  = SetTextTagPosUnit
    SetTextTagSuspendedBJ                = SetTextTagSuspended
    SetTextTagPermanentBJ                = SetTextTagPermanent
    SetTextTagAgeBJ                      = SetTextTagAge
    SetTextTagLifespanBJ                 = SetTextTagLifespan
    SetTextTagFadepointBJ                = SetTextTagFadepoint
    DestroyTextTagBJ                     = DestroyTextTag
    ForceCinematicSubtitlesBJ            = ForceCinematicSubtitles
    DisplayCineFilterBJ                  = DisplayCineFilter
    SaveGameCacheBJ                      = SaveGameCache
    FlushGameCacheBJ                     = FlushGameCache
    SaveGameCheckPointBJ                 = SaveGameCheckpoint
    LoadGameBJ                           = LoadGame
    RenameSaveDirectoryBJ                = RenameSaveDirectory
    RemoveSaveDirectoryBJ                = RemoveSaveDirectory
    CopySaveGameBJ                       = CopySaveGame
    IssueTargetOrderBJ                   = IssueTargetOrder
    IssueTargetDestructableOrder         = IssueTargetOrder -- This was just to type casting
    IssueTargetItemOrder                 = IssueTargetOrder -- This was just to type casting
    IssueImmediateOrderBJ                = IssueImmediateOrder
    GroupTargetOrderBJ                   = GroupTargetOrder
    GroupImmediateOrderBJ                = GroupImmediateOrder
    GroupTrainOrderByIdBJ                = GroupImmediateOrderById
    GroupTargetDestructableOrder         = GroupTargetOrder       -- This was just to type casting
    GroupTargetItemOrder                 = GroupTargetOrder       -- This was just to type casting
    GetDyingDestructable                 = GetTriggerDestructable -- I think they just wanted a better name
    GetAbilityName                       = GetObjectName          -- I think they just wanted a better name

    -- List of math overrides, provided by Antares & Insanity_AI
    CosBJ                                = function(degrees) return math.cos(degrees * bj_DEGTORAD) end ---@type fun(degrees: number): number
    SinBJ                                = function(degrees) return math.sin(degrees * bj_DEGTORAD) end ---@type fun(degrees: number): number
    TanBJ                                = function(degrees) return math.tan(degrees * bj_DEGTORAD) end ---@type fun(degrees: number): number
    AsinBJ                               = function(ratio) return math.asin(ratio) * bj_RADTODEG end ---@type fun(ratio: number): number
    AcosBJ                               = function(ratio) return math.acos(ratio) * bj_RADTODEG end ---@type fun(ratio: number): number

    -- Native Atans are faster than math.atan, surprisingly
    -- AtanBJ                               = function(ratio) return math.atan(ratio) * bj_RADTODEG end ---@type fun(ratio: number): number
    -- Atan2BJ                              = function(y, x) return math.atan(y, x) * bj_RADTODEG end ---@type fun(x: number, y: number): number

    Cos                                  = math.cos
    Sin                                  = math.sin
    Tan                                  = math.tan
    Acos                                 = math.acos
    Asin                                 = math.asin
    Pow                                  = function(base, exponent) return base ^ exponent end ---@type fun(base: number, exponent: number): number
    SquareRoot                           = math.sqrt
    Deg2Rad                              = function(degrees) return degrees * bj_DEGTORAD end ---@type fun(degrees: number): number
    Rad2Deg                              = function(radians) return radians * bj_RADTODEG end ---@type fun(radians: number): number

    SubStringBJ                          = string.sub
    SubString                            = function(source, start, _end) return string.sub(source, start + 1, _end) end ---@type fun(source: string, start: integer, _end: integer): string
    StringLength                         = string.len
    StringCase                           = function(source, upper) if upper then return string.upper(source) else return string.lower(source) end end ---@type fun(source: string, upper: boolean): string
end
if Debug then Debug.endFile() end
