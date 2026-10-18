---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "Libs/src/SkippableTimers.lua"
ref: HEAD
---

if Debug then Debug.beginFile "SkippableTimers" end
SkippableTimers = {
    skippableTimers = {}
}

function SkippableTimers:start(dur, func)
    local timer = SimpleUtils.timed(dur, func)
    table.insert(self.skippableTimers, timer)
    return timer
end

function SkippableTimers:skip()
    for index, timer in ipairs(self.skippableTimers) do
        SimpleUtils.releaseTimer(timer)
    end
    self.skippableTimers = {}
end
if Debug then Debug.endFile() end
