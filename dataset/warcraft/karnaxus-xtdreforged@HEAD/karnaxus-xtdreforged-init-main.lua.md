---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "init/main.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Main                                                |
    | =================================================== |
    | The main bootstrap.                                 |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD = XTD or {}
XTD.Constants = XTD.Constants or {}
XTD.Data = XTD.Data or {}
XTD.Core = XTD.Core or {}
XTD.Systems = XTD.Systems or {}
XTD.UI = XTD.UI or {}

XTD.initialized = false

--[[
    Initialize X Tower Defense: Reforged
]]
function XTD.Init()
    if XTD.initialized then
        return
    end

    XTD.initialized = true

    XTD.Core.Debug("Main", "Initializing X Tower Defense: Reforged...")

    XTD.Core.MinimapManager.Init()
    XTD.Core.PlayerManager.Init()
    XTD.UI.CameraManager.Init()
    XTD.UI.CinematicManager.Init()
    XTD.Systems.LaneManager.Init()
    XTD.Systems.CreepBoxManager.Init()
    XTD.Systems.CreepManager.Init()
    XTD.Systems.VoteManager.Init()
    XTD.UI.WorldLabelManager.Init()
    XTD.Systems.UnitManager.Init()
    XTD.Core.GameManager.Init()
    XTD.Systems.RegionManager.Init()
    XTD.Systems.EndzoneManager.Init()
    XTD.Systems.DeathManager.Init()
    XTD.UI.QuestManager.Init()

    XTD.Core.Debug("Main", "X Tower Defense: Reforged initialized.")
end
