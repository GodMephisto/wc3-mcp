---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "Libs/src/SimpleUtils.lua"
ref: HEAD
---

if Debug then Debug.beginFile "SimpleUtils" end
SimpleUtils = {
    globalDebug = false,
    debug = false,
    debugTime = false,
    timedCalls = {}
}

function SimpleUtils.debugFunc(func, name)
    return func()
end

function SimpleUtils.printTime(since, name)
    if SimpleUtils.debugTime then
        local call = SimpleUtils.timedCalls[name] or 0
        call = call + 1
        SimpleUtils.timedCalls[name] = call
        local duration = os.time() - since
        --if duration > 0 then
        print(name .. ": " .. tostring(call) .. ". call took " .. tostring(duration) .. "ms")
        --end
    end
end

function SimpleUtils.debugFuncTimed(func, name)
    local start = os.time()
    local name = name or ""
    local result = func()
    SimpleUtils.printTime(start, name)
    return result
end

function SimpleUtils.newClass(t, constructor)
    local t = t
    t.__index = t
    t.lookupt = {}
    t.new = function()
        local o = {}
        setmetatable(o, t)
        if not (constructor == nil) then
            constructor(o)
        end
        return o
    end
    t.destroy = function()
        t.lookupt[t] = nil
    end
    if SimpleUtils.debug and SimpleUtils.globalDebug then
        print("made new class for " .. tostring(t))
    end
end

function SimpleUtils.timed(dur, func)
    local tmr = CreateTimer()
    TimerStart(tmr, dur, false, function()
        func()
        SimpleUtils.releaseTimer(tmr)
    end)
    return tmr;
end

function SimpleUtils.timedSkippable(dur, func)
    SkippableTimers:start(dur, func)
end

function SimpleUtils.timedRepeat(dur, count, func)
    local tmr = CreateTimer()
    local t, c = count, 0
    if t == nil then
        TimerStart(tmr, dur, true, function()
            func(tmr)
        end)
    else
        TimerStart(tmr, dur, true, function()
            func(tmr)
            c = c + 1
            if c >= t then
                SimpleUtils.releaseTimer(tmr)
            end
        end)
    end
    return tmr;
end

function SimpleUtils.timedPeriodic(interval, shouldContinueFunc)
    local timer = CreateTimer()
    TimerStart(timer, interval, true, function()
        if shouldContinueFunc() ~= true then
            SimpleUtils.releaseTimer(timer)
        end
    end)
    return timer
end

-- :: clones a table and any child tables (setting metatables)
-- @t = table to copy
function SimpleUtils.deepCopy(t)
    local t2 = {}
    if getmetatable(t) then
        setmetatable(t2, getmetatable(t))
    end
    for k, v in pairs(t) do
        if type(v) == "table" then
            local newt = {}
            if getmetatable(v) then
                setmetatable(newt, getmetatable(v))
            end
            for k2, v2 in pairs(v) do
                newt[k2] = v2
            end
            t2[k] = newt
        else
            t2[k] = v
        end
    end
    return t2
end

function SimpleUtils.destroyTable(t)
    for i, v in pairs(t) do
        if type(v) == "table" then
            for i2, v2 in pairs(v) do
                v2 = nil
                i2 = nil
            end
        else
            v = nil
            i = nil
        end
    end
end

function SimpleUtils.playSound(snd)
    StopSound(snd, false, false)
    StartSound(snd)
end

function SimpleUtils.tableLength(t)
    if not t then
        return 0
    end
    local idx = 0
    while true do
        assert(idx < 10000, 'ERROR: reached 10000 items in table, feel free to increase it in SimpleUnits script, I just gave it some limit to avoid potential infinite loop')
        if t[idx + 1] == nil then
            return idx
        end
        idx = idx + 1
    end
end

function SimpleUtils.tableRemove(t, itemToRemove)
    if not t then
        return false
    end
    for i, item in ipairs(t) do
        if item == itemToRemove then

        end
        count = count + 1
    end
    return count
end

function SimpleUtils.ifElse(condition, onTrue, onFalse)
    if condition == true then
        return onTrue
    else
        return onFalse
    end
end

function SimpleUtils.printDupa()
    print("dupa")
end

function SimpleUtils.fadeOut(duration)
    CinematicFadeBJ(bj_CINEFADETYPE_FADEOUT, duration, "ReplaceableTextures\\CameraMasks\\Black_mask.blp", 1, 1, 1, 0)
end

function SimpleUtils.fadeIn(duration)
    CinematicFadeBJ(bj_CINEFADETYPE_FADEIN, duration, "ReplaceableTextures\\CameraMasks\\Black_mask.blp", 1, 1, 1, 0)
end

function SimpleUtils.valueToString(v, prefix)
    if type(v) == "table" then
        return SimpleUtils.toString(v, prefix .. " ")
    else
        return tostring(v)
    end
end

function SimpleUtils.toString(o, prefix)
    if not prefix then prefix = "" end
    local s = "{\n"
    for key, value in pairs(o) do
        local keyString = SimpleUtils.valueToString(key, '')
        local valueString = SimpleUtils.valueToString(value, prefix)
        s = s .. prefix .. "\"" .. keyString .. "\": \"" .. valueString .. "\",\n"
    end
    s = s .. "\n}\n"
    return s
end

function SimpleUtils.printToString(label, o)
    print(label .. ": " .. SimpleUtils.valueToString(o, " "))
end

function SimpleUtils.getRandomNumbers(min, max, count)
    local rangeLength = max - min + 1
    --print("rangeLength: " .. rangeLength)
    if rangeLength < count then
        print("Invalid args for SimpleUtils.getRandomNumbers: " .. tostring(min) .. " > " .. tostring(max))
        return {}
    end
    local range = {}
    for i = min, max do
        table.insert(range, i)
        --print("range insert " .. tostring(i))
    end

    local output = {}
    for _ = 0, count - 1 do
        local nextNumberIndex = math.random(1, rangeLength)
        --print("nextNumberIndex " .. tostring(nextNumberIndex))
        local nextNumber = table.remove(range, nextNumberIndex)
        --print("removed nextNumber " .. tostring(nextNumber))
        table.insert(output, nextNumber)
        rangeLength = rangeLength - 1
    end
    return output
end

function SimpleUtils.releaseTimer(whichTimer)
    PauseTimer(whichTimer)
    DestroyTimer(whichTimer)
end

function SimpleUtils.printWarn(msg)
    print("[WARN] " .. msg)
end

function SimpleUtils.split(string, separator)
    if separator == nil then
        separator = "%%s"
    end
    local t = {}
    for str in string.gmatch(string, "([^".. separator .."]+)") do
        table.insert(t, str)
    end
    --SimpleUtils.printToString("split", t)
    return t
end

-- Returns the distance between 2 coordinates in Warcraft III units
function SimpleUtils.distanceBetweenCoordinates(x1, y1, x2, y2)
    local dx = (x2 - x1)
    local dy = (y2 - y1)

    return SquareRoot(dx*dx + dy*dy)
end

function SimpleUtils.angleBetweenCoordinates(x, y, x2, y2)
    return Atan2(y2 - y, x2 - x)
end
if Debug then Debug.endFile() end
