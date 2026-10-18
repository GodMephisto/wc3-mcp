---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/spawn_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Spawn Manager                                       |
    | =================================================== |
    | Manages spawning creeps.                            |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.SpawnManager = XTD.Systems.SpawnManager or {}

XTD.Systems.SpawnManager.creeps = {}

--[[
    Initialize the Spawn Manager.
]]
function XTD.Systems.SpawnManager.Init()
    XTD.Core.Debug("SpawnManager", "Initializing the Spawn Manager...")

    XTD.Core.Debug("SpawnManager", "Spawn Manager initialized.")
end

--[[
    Spawn a wave of creeps.
]]
function XTD.Systems.SpawnManager.SpawnWave(unitType, total, interval, onComplete)
    if unitType == nil then
        XTD.Core.Debug("SpawnManager", "unitType is nil.", "SpawnWave")
        return
    end

    if XTD.Systems.WaveManager.spawnDisabled then
        XTD.Core.Debug("SpawnManager", "Spawning is disabled.", "SpawnWave")
        return
    end

    local enabledLanes = XTD.Systems.LaneManager.GetEnabledLanes()

    if enabledLanes == nil then
        XTD.Core.Debug("SpawnManager", "enabledLanes is nil.", "SpawnWave")
        return
    end

    local totalSpawned = 0
    local endingSpawnCount = 0

    for _, enabled in pairs(enabledLanes) do
        if enabled then
            endingSpawnCount = endingSpawnCount + total
        end
    end

    WaitForSpawnFinish = function (totalUnitsSpawned)
        totalSpawned = totalSpawned + totalUnitsSpawned

        if totalSpawned == endingSpawnCount then
            if onComplete then
                onComplete(totalSpawned)
            end
        end
    end

    for lane, enabled in pairs(enabledLanes) do
        if enabled then
            local spawn = XTD.Data.Regions.Lanes[lane].Spawn

            if spawn ~= nil then
                local currentDirection = "left"
                local rect = XTD.Systems.RegionManager.ResolveRegion(spawn)
                local spawned = 0
                local timer = CreateTimer()
                local x, y = XTD.Systems.RegionManager.GetRectCenter(rect)

                if x == nil or y == nil then
                    XTD.Core.Debug("SpawnManager", "x or y is nil.", "SpawnWave")
                    return
                end

                TimerStart(timer, interval, true, function ()
                    spawned = spawned + 1

                    local unit = XTD.Systems.UnitManager.SpawnUnitAtPoint(
                        XTD.Constants.CREEPS,
                        unitType,
                        x,
                        y,
                        270.00,
                        true
                    )

                    XTD.Systems.SpawnManager.creeps[GetHandleId(unit)] = {
                        unit = unit,
                        lane = lane,
                        direction = currentDirection,
                        lastX = GetUnitX(unit),
                        lastY = GetUnitY(unit),
                        nextRegion = nil,
                        isBoss = false
                    }

                    XTD.Systems.ResultsManager.RegisterDamageTracking(unit)

                    if currentDirection == "left" then
                        currentDirection = "right"
                    else
                        currentDirection = "left"
                    end

                    XTD.Systems.CreepManager.IncrementCreepsAlive()

                    local gameSettings = XTD.Core.GameManager.GetGameSettings()
                    local waveSettings = XTD.Data.Waves[XTD.Systems.WaveManager.currentWave]

                    local maxLife = XTD.Systems.UnitManager.GetMaxLife(unit)
                    local newMaxLife = math.floor(maxLife * gameSettings.hpMultiplier)

                    XTD.Systems.UnitManager.SetMaxLife(unit, newMaxLife)

                    local baseSpeed = XTD.Systems.UnitManager.GetMovementSpeed(unit)
                    local speed = math.floor(baseSpeed * gameSettings.movementSpeedMultiplier)

                    XTD.Systems.UnitManager.SetMovementSpeed(unit, speed)
                    XTD.Systems.UnitManager.DisableCollision(unit)

                    if waveSettings.category == XTD.Data.WaveCategories.BOSS then
                        XTD.Systems.BossManager.OnBossSpawn(unit, XTD.Systems.WaveManager.currentWave)
                    end

                    if spawned >= total then
                        PauseTimer(timer)
                        DestroyTimer(timer)

                        WaitForSpawnFinish(spawned)
                    end
                end)
            end
        end
    end
end