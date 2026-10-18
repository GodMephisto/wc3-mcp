---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "Libs/src/UtilsLegacyInterface.lua"
ref: HEAD
---

if Debug then Debug.beginFile "UtilsLegacyInterface" end
utils = {}
function utils.debugfunc(func, name)
    return SimpleUtils.debugFunc(func, name)
end

function utils.debugfuncTimed(func, name)
    return SimpleUtils.debugFuncTimed(func, name)
end

function utils.newclass(t, constructor)
    return SimpleUtils.newClass(t, constructor)
end

function utils.timed(dur, func)
    return SimpleUtils.timed(dur, func)
end

function utils.timedSkippable(dur, func)
    SimpleUtils.timedSkippable(dur, func)
end

function utils.timedRepeat(dur, count, func)
    return SimpleUtils.timedRepeat(dur, count, func)
end

function utils.ifElse(condition, onTrue, onFalse)
    return SimpleUtils.ifElse(condition, onTrue, onFalse)
end

function utils.getRandomNumbers(min, max, count)
    return SimpleUtils.getRandomNumbers(min, max, count)
end

function utils.printDupa()
    print("dupa")
end
if Debug then Debug.endFile() end
