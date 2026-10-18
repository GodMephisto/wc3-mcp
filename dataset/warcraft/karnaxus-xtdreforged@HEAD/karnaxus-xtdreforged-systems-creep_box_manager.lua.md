---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/creep_box_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Creep Box Manager                                   |
    | =================================================== |
    | Manages the creep boxes spawns and labels.          |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.CreepBoxManager = XTD.Systems.CreepBoxManager or {}

XTD.Systems.CreepBoxManager.textTags = {}
XTD.Systems.CreepBoxManager.boxCreeps = {}

--[[
    Initialize the Creep Box Manager.
]]
function XTD.Systems.CreepBoxManager.Init()
    XTD.Core.Debug("CreepBoxManager", "Initializing the Creep Box Manager...")

    XTD.Systems.CreepBoxManager.PopulateBoxes()

    XTD.Core.Debug("CreepBoxManager", "Creep Box Manager initialized.")
end

--[[
    Populate all the creep wave boxes.
]]
function XTD.Systems.CreepBoxManager.PopulateBoxes()
    XTD.Core.Debug("CreepBoxManager", "Populating the creep boxes...")

    local self = XTD.Systems.CreepBoxManager

    local creepWaves = XTD.Data.Waves

    if creepWaves == nil then
        XTD.Core.Debug("CreepBoxManager", "creepWaves is nil.", "PopulateBoxes")
        return
    end

    for wave, data in pairs(creepWaves) do
        local waveSettings = XTD.Data.Waves[wave]
        local special = ""

        if waveSettings.category == XTD.Data.WaveCategories.AIR then
            special = XTD.Core.Color.BloodRed(" (Air) ")
        end

        if waveSettings.category == XTD.Data.WaveCategories.BONUS then
            special = XTD.Core.Color.BloodRed(" (Bonus) ")
        end

        if waveSettings.category == XTD.Data.WaveCategories.BOSS then
            special = XTD.Core.Color.BloodRed(" (Boss) ")
        end

        self.boxCreeps[wave] = XTD.Systems.UnitManager.SpawnUnitAtPoint(
            XTD.Constants.CREEPS,
            data.unitType,
            data.boxPosition.x,
            data.boxPosition.y,
            270.00
        )

        self.textTags[wave] = XTD.UI.WorldLabelManager.CreateWorldLabel(
            XTD.Core.Color.BrightBlue(data.name) .. special .. "\n" ..
            XTD.Core.Color.AncientGold("Wave ") ..
            XTD.Core.Color.BrightGreen(tostring(wave)),
            data.labelPosition.x,
            data.labelPosition.y,
            50.00,
            0.026
        )
    end

    XTD.Core.Debug("CreepBoxManager", "Creep boxes populated.")
end