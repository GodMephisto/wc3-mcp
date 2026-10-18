---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/creep_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Creep Manager                                       |
    | =================================================== |
    | Base creep manager.                                 |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.CreepManager = XTD.Systems.CreepManager or {}

XTD.Systems.CreepManager.creepsAlive = 0

--[[
    Initialize the Creep Manager.
]]
function XTD.Systems.CreepManager.Init()
    XTD.Core.Debug("CreepManager", "Initializing the Creep Manager...")

    XTD.Systems.CreepManager.creepsAlive = 0

    XTD.Core.Debug("CreepManager", "Creep Manager initialized.")
end

--[[
    Increment the total creeps alive by 1.
]]
function XTD.Systems.CreepManager.IncrementCreepsAlive()
    XTD.Systems.CreepManager.creepsAlive = XTD.Systems.CreepManager.creepsAlive + 1
end

--[[
    Check whether a unit is a creep.
]]
function XTD.Systems.CreepManager.IsCreep(unit)
    if unit == nil then
        return false
    end

    return GetOwningPlayer(unit) == XTD.Constants.CREEPS
end

--[[
    Decrement total creeps alive by 1.
]]
function XTD.Systems.CreepManager.DecrementCreepsAlive()
    XTD.Systems.CreepManager.creepsAlive = XTD.Systems.CreepManager.creepsAlive - 1
end