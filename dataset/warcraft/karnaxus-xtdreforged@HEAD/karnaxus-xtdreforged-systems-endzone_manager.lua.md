---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/endzone_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Endzone Manager                                     |
    | =================================================== |
    | Manages the endzone.                                |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.EndzoneManager = XTD.Systems.EndzoneManager or {}

XTD.Systems.EndzoneManager.leaksLeft = 0

--[[
    Initialize the End Zone Manager.
]]
function XTD.Systems.EndzoneManager.Init()
    XTD.Core.Debug("EndzoneManager", "Initializing the End Zone Manager...", "Init")

    XTD.Systems.EndzoneManager.leaksLeft = 0

    XTD.Core.Debug("EndzoneManager", "End Zone Manager initialized.", "Init")
end

--[[
    Handle when a unit enters the endzone.
]]
function XTD.Systems.EndzoneManager.OnEnter(unit)
    if unit == nil then
        XTD.Core.Debug("EndzoneManager", "unit is nil.", "OnEnter")
        return
    end

    if XTD.Core.GameManager.gameOver then
        return
    end

    if not XTD.Systems.CreepManager.IsCreep(unit) then
        return
    end

    if not XTD.Systems.WaveManager.IsBonusWave() then
        local waveSettings = XTD.Data.Waves[XTD.Systems.WaveManager.currentWave]

        if waveSettings == nil then
            XTD.Core.Debug("EndzoneManager", "waveSettings is nil.", "OnEnter")
            return
        end

        XTD.Systems.EndzoneManager.leaksLeft = XTD.Systems.EndzoneManager.leaksLeft - waveSettings.leakValue

        StartSound(gg_snd_Leak)

        XTD.UI.MessageManager.Broadcast(
            XTD.Core.Color.Red("Leak Detected! ") ..
            XTD.Core.Color.BrightGreen(tostring(XTD.Systems.EndzoneManager.leaksLeft)) ..
            XTD.Core.Color.AncientGold(" leaks left!")
        )

        local rect = XTD.Systems.RegionManager.ResolveRegion(XTD.Data.Regions.Lanes.NW.EndZone)

        if rect ~= nil then
            local x, y = XTD.Systems.RegionManager.GetRectCenter(rect)

            if x ~= nil and y ~= nil then
                PingMinimap(x, y, 5.00)
            end
        end
    end    

    XTD.Systems.UnitManager.RemoveUnitOnLeak(unit)

    if XTD.Systems.EndzoneManager.leaksLeft <= 0 then
        XTD.Systems.WaveManager.spawnDisabled = true
        XTD.Core.GameManager.DeclareDefeat()
        return
    end
end