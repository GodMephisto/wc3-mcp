---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/death_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Death Manager                                       |
    | =================================================== |
    | Manages the death of creeps.                        |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.DeathManager = XTD.Systems.DeathManager or {}

--[[
    Initialize the Death Manager.
]]
function XTD.Systems.DeathManager.Init()
    XTD.Core.Debug("DeathManager", "Initializing the Death Manager...")

    XTD.Systems.DeathManager.RegisterTrigger()

    XTD.Core.Debug("DeathManager", "Death Manager initialized.")
end

--[[
    Register the death trigger.
]]
function XTD.Systems.DeathManager.RegisterTrigger()
    local trigger = CreateTrigger()

    TriggerRegisterAnyUnitEventBJ(trigger, EVENT_PLAYER_UNIT_DEATH)

    TriggerAddAction(trigger, function ()
        local dyingUnit = GetDyingUnit()

        if dyingUnit == nil then
            XTD.Core.Debug("DeathManager", "dyingUnit is nil.", "RegisterTrigger")
            return
        end

        if not XTD.Systems.CreepManager.IsCreep(dyingUnit) then
            return
        end

        XTD.Systems.DeathManager.OnDeath(dyingUnit)
    end)
end

--[[
    Handle when a creep dies.
]]
function XTD.Systems.DeathManager.OnDeath(unit)
    if unit == nil then
        XTD.Core.Debug("DeathManager", "unit is nil.", "OnDeath")
        return
    end

    local killer = GetKillingUnit()

    XTD.Systems.CreepManager.DecrementCreepsAlive()

    if killer ~= nil then
        local killingPlayer = GetOwningPlayer(killer)
        local waveSettings = XTD.Data.Waves[XTD.Systems.WaveManager.currentWave]
        local gameSettings = XTD.Core.GameManager.GetGameSettings()
        local totalGold = math.floor(waveSettings.bounty * gameSettings.goldMultiplier)
        local playerData = XTD.Core.PlayerManager.GetPlayer(killingPlayer)

        if playerData ~= nil then
            playerData.kills = (playerData.kills or 0) + 1

            XTD.Core.PlayerManager.UpdateNativeUI(playerData)
            XTD.Core.PlayerManager.AddResource(playerData.player, "gold", totalGold)

            XTD.Core.Debug("DeathManager", playerData.name .. " kills: " .. tostring(playerData.kills))
        end

        if waveSettings.category == XTD.Data.WaveCategories.BOSS then
            XTD.Systems.BossManager.OnBossDeath(unit, XTD.Systems.WaveManager.currentWave, killer)
        end
    end

    if XTD.Systems.CreepManager.creepsAlive <= 0 then
        XTD.Systems.CreepManager.creepsAlive = 0
        XTD.Systems.WaveManager.OnWaveComplete()
    end

    local timer = CreateTimer()

    TimerStart(timer, 1.00, false, function ()
        XTD.Systems.SpawnManager.creeps[GetHandleId(unit)] = nil

        RemoveUnit(unit)

        PauseTimer(timer)
        DestroyTimer(timer)
    end)
end