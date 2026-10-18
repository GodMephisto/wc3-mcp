---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/lane_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Lane Manager                                        |
    | =================================================== |
    | Manages lanes.                                      |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.LaneManager = XTD.Systems.LaneManager or {}

XTD.Systems.LaneManager.enabledLanes = {
    NW = false,
    NE = false,
    SE = false,
    SW = false
}

XTD.Systems.LaneManager.activePlayers = {
    [0] = false,
    [1] = false,
    [2] = false,
    [3] = false,
    [4] = false,
    [5] = false,
    [6] = false,
    [7] = false
}

--[[
    Initialize the Lane Manager.
]]
function XTD.Systems.LaneManager.Init()
    XTD.Core.Debug("LaneManager", "Initializing the Lane Manager...")

    XTD.Systems.LaneManager.DetermineEnabledLanes()

    XTD.Core.Debug("LaneManager", "Lane Manager initialized.")
end

--[[
    Determine which lanes to enable.
]]
function XTD.Systems.LaneManager.DetermineEnabledLanes()
    local self = XTD.Systems.LaneManager

    XTD.Core.PlayerManager.ForEach(function (playerData)
        XTD.Systems.LaneManager.activePlayers[playerData.id] = true
    end)

    if self.activePlayers[0] or self.activePlayers[1] then
        self.enabledLanes.NW = true
    end

    if self.activePlayers[2] or self.activePlayers[3] then
        self.enabledLanes.NE = true
    end

    if self.activePlayers[4] or self.activePlayers[5] then
        self.enabledLanes.SE = true
    end

    if self.activePlayers[6] or self.activePlayers[7] then
        self.enabledLanes.SW = true
    end
end

--[[
    Get the enabled lanes.
]]
function XTD.Systems.LaneManager.GetEnabledLanes()
    return XTD.Systems.LaneManager.enabledLanes
end