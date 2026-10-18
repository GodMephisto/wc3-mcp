---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/boss_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | X TOWER DEFENSE: REFORGED                           |
    | Boss Manager                                        |
    +-----------------------------------------------------+
]]

XTD.Systems.BossManager = XTD.Systems.BossManager or {}

XTD.Systems.BossManager.checkInterval = 0.25
XTD.Systems.BossManager.activeBosses = {}

XTD.Systems.BossManager.bosses = {
    [20] = {
        warning = "An evil champion from the Burning Legion approaches",
        rewardGold = 600,
        scale = 1.5,
        bonusArmor = 10,
        bonusHPMultiplier = 1.25,
        regenPerSecond = 0.50,
        summonThresholds = { 80, 60, 40, 20 },
        summonUnitId = FourCC("n01P")
    },

    [30] = {
        warning = "Doom looms -- A Doom Guard Champion has appeared...",
        rewardGold = 900,
        scale = 1.5,
        bonusArmor = 10,
        bonusHPMultiplier = 1.25,
        regenPerSecond = 0.50,
        enrageThreshold = 50,
        enrageSpeedBonus = 150
    },

    [40] = {
        warning = "Kil'jaeden, the Deceiver, has entered the battlefield! The fate of the world now rests in your hands. Stand your ground!",
        rewardGold = 2000,
        scale = 1.5,
        bonusArmor = 10,
        bonusHPMultiplier = 1.25,
        regenPerSecond = 0.50,
        summonThresholds = { 80, 60, 40, 20 },
        summonUnitId = FourCC("n01P"),
        enrageThreshold = 50,
        enrageSpeedBonus = 40
    }
}

--[[
    Initialize the Boss Manager.
]]
function XTD.Systems.BossManager.Init()
    XTD.Core.Debug("BossManager", "Initializing the Boss Manager...")

    XTD.Systems.BossManager.InitBossCheckTimer()

    XTD.Core.Debug("BossManager", "Boss Manager initialized.")
end

--[[
    Get the wave number.
]]
function XTD.Systems.BossManager.GetWaveNumber(wave)
    if wave == nil then
        return nil
    end

    if type(wave) == "number" then
        return wave
    end

    if type(wave) == "table" then
        return wave.number
    end

    return nil
end

--[[
    Get the boss configurations.
]]
function XTD.Systems.BossManager.GetBossConfig(wave)
    local waveNumber = XTD.Systems.BossManager.GetWaveNumber(wave)

    if waveNumber == nil then
        return nil
    end

    return XTD.Systems.BossManager.bosses[waveNumber]
end

--[[
    Apply to boss units on spawn.
]]
function XTD.Systems.BossManager.OnBossSpawn(boss, wave)
    if boss == nil or wave == nil then
        return
    end

    local waveNumber = XTD.Systems.BossManager.GetWaveNumber(wave)
    local bossConfig = XTD.Systems.BossManager.GetBossConfig(wave)

    if waveNumber == nil or bossConfig == nil then
        return
    end

    XTD.Systems.BossManager.ApplyBossStats(boss, waveNumber)

    local bossId = GetHandleId(boss)

    XTD.Systems.BossManager.activeBosses[bossId] = {
        unit = boss,
        wave = waveNumber,
        config = bossConfig,
        nextSummonIndex = 1,
        enraged = false
    }

    local unitData = XTD.Systems.SpawnManager.creeps[bossId]

    if unitData ~= nil then
        unitData.isBoss = true
    end

    XTD.UI.MessageManager.Broadcast(
        XTD.Core.Color.Red(bossConfig.warning)
    )

    XTD.Core.Debug("BossManager", "Boss spawned for wave " .. tostring(waveNumber))
end

--[[
    Triggered when a unit dies.
]]
function XTD.Systems.BossManager.OnBossDeath(boss, wave, killer)
    if boss == nil then
        return
    end

    local bossId = GetHandleId(boss)
    local data = XTD.Systems.BossManager.activeBosses[bossId]

    if data ~= nil then
        wave = data.wave
    end

    XTD.Systems.BossManager.activeBosses[bossId] = nil

    local unitData = XTD.Systems.SpawnManager.creeps[bossId]

    if killer ~= nil and wave ~= nil and unitData ~= nil and unitData.isBoss then
        local player = GetOwningPlayer(killer)

        XTD.Systems.BossManager.GiveBossReward(player, wave)
    end
end

--[[
    Apply special boss stats.
]]
function XTD.Systems.BossManager.ApplyBossStats(boss, wave)
    if boss == nil or wave == nil then
        return
    end

    local waveNumber = XTD.Systems.BossManager.GetWaveNumber(wave)
    local bossConfig = XTD.Systems.BossManager.GetBossConfig(wave)

    if waveNumber == nil or bossConfig == nil then
        return
    end

    local maxLife = XTD.Systems.UnitManager.GetMaxLife(boss)
    local newMaxLife = math.floor(maxLife * bossConfig.bonusHPMultiplier)

    XTD.Systems.UnitManager.SetMaxLife(boss, newMaxLife)
    XTD.Systems.UnitManager.SetUnitArmor(boss, bossConfig.bonusArmor)
    XTD.Systems.UnitManager.SetUnitScale(boss, bossConfig.scale)

    XTD.Core.Debug("BossManager", "Applied boss stats for wave " .. tostring(waveNumber) .. ".")
end

--[[
    Give the killing player a nice reward.
]]
function XTD.Systems.BossManager.GiveBossReward(player, wave)
    if player == nil or wave == nil then
        return
    end

    local bossConfig = XTD.Systems.BossManager.GetBossConfig(wave)

    if bossConfig == nil or bossConfig.rewardGold == nil then
        return
    end

    XTD.Core.PlayerManager.AddResource(player, "gold", bossConfig.rewardGold)

    XTD.UI.MessageManager.Player(
        player,
        XTD.Core.Color.BrightBlue("Boss Defeated! ") ..
        XTD.Core.Color.Ivory("Bonus reward: ") ..
        XTD.Core.Color.BrightGreen(tostring(bossConfig.rewardGold)) ..
        XTD.Core.Color.Ivory(" gold.")
    )
end

--[[
    Initialize the boss check timer.
]]
function XTD.Systems.BossManager.InitBossCheckTimer()
    local timer = CreateTimer()

    TimerStart(timer, XTD.Systems.BossManager.checkInterval, true, function()
        XTD.Systems.BossManager.CheckActiveBosses()
    end)
end

--[[
    Check all active bosses.
]]
function XTD.Systems.BossManager.CheckActiveBosses()
    for bossId, data in pairs(XTD.Systems.BossManager.activeBosses) do
        local boss = data.unit

        if boss == nil or GetUnitTypeId(boss) == 0 or GetWidgetLife(boss) <= 0.405 then
            XTD.Systems.BossManager.activeBosses[bossId] = nil
        else
            XTD.Systems.BossManager.ApplyBossRegen(boss, data)
            XTD.Systems.BossManager.CheckInfernal(boss, data)
            XTD.Systems.BossManager.CheckDoomguard(boss, data)
            XTD.Systems.BossManager.CheckKiljaeden(boss, data)
        end
    end
end

--[[
    Apply special boss regen stats.
]]
function XTD.Systems.BossManager.ApplyBossRegen(boss, data)
    if boss == nil or data == nil or data.config == nil then
        return
    end

    local regen = data.config.regenPerSecond or 0

    if regen <= 0 then
        return
    end

    local life = XTD.Systems.UnitManager.GetCurrentLife(boss)
    local maxLife = XTD.Systems.UnitManager.GetMaxLife(boss)
    local amount = regen * XTD.Systems.BossManager.checkInterval

    XTD.Systems.UnitManager.SetUnitLife(boss, math.min(life + amount, maxLife))
end

--[[
    Check the Infernal boss.
]]
function XTD.Systems.BossManager.CheckInfernal(boss, data)
    if data == nil or data.wave ~= 20 or data.config.summonUnitId == nil then
        return
    end

    XTD.Systems.BossManager.CheckSummonThresholds(
        boss,
        data,
        "The Infernal summons reinforcements!"
    )
end

--[[
    Check the Doomguard boss.
]]
function XTD.Systems.BossManager.CheckDoomguard(boss, data)
    if data == nil or data.wave ~= 30 then
        return
    end

    if data.enraged == true then
        return
    end

    local hpPercent = XTD.Systems.UnitManager.GetHealthPercent(boss)
    local threshold = data.config.enrageThreshold or 50

    if hpPercent <= threshold then
        XTD.Systems.BossManager.EnrageBoss(
            boss,
            data.config.enrageSpeedBonus or 150,
            "The Doomguard"
        )

        data.enraged = true
    end
end

--[[
    Check the Kil`jaeden boss.
]]
function XTD.Systems.BossManager.CheckKiljaeden(boss, data)
    if data == nil or data.wave ~= 40 then
        return
    end

    XTD.Systems.BossManager.CheckSummonThresholds(
        boss,
        data,
        "Kil'jaeden summons reinforcements!"
    )

    if data.enraged == true then
        return
    end

    local hpPercent = XTD.Systems.UnitManager.GetHealthPercent(boss)
    local threshold = data.config.enrageThreshold or 50

    if hpPercent <= threshold then
        XTD.Systems.BossManager.EnrageBoss(
            boss,
            data.config.enrageSpeedBonus or 40,
            "Kil'jaeden"
        )

        data.enraged = true
    end
end

--[[
    Check add summoning thresholds.
]]
function XTD.Systems.BossManager.CheckSummonThresholds(boss, data, message)
    if boss == nil or data == nil or data.config == nil then
        return
    end

    local thresholds = data.config.summonThresholds

    if thresholds == nil or data.config.summonUnitId == nil then
        return
    end

    local threshold = thresholds[data.nextSummonIndex]

    if threshold == nil then
        return
    end

    local hpPercent = XTD.Systems.UnitManager.GetHealthPercent(boss)

    if hpPercent <= threshold then
        XTD.Systems.BossManager.SummonAdds(boss, data)

        XTD.UI.MessageManager.Broadcast(
            XTD.Core.Color.Purple(message)
        )

        data.nextSummonIndex = data.nextSummonIndex + 1
    end
end

--[[
    Summon additional units for the boss.
]]
function XTD.Systems.BossManager.SummonAdds(boss, data)
    if boss == nil or data == nil or data.config == nil then
        return
    end

    local x = GetUnitX(boss)
    local y = GetUnitY(boss)
    local owner = GetOwningPlayer(boss)

    local bossUnitData = XTD.Systems.SpawnManager.creeps[GetHandleId(boss)]

    if bossUnitData == nil then
        return
    end

    for i = 1, 5 do
        local angle = math.rad(i * 72)

        local unit = XTD.Systems.UnitManager.SpawnUnitAtPoint(
            owner,
            data.config.summonUnitId,
            x + 150 * math.cos(angle),
            y + 150 * math.sin(angle),
            bj_UNIT_FACING,
            true
        )

        XTD.Systems.SpawnManager.creeps[GetHandleId(unit)] = {
            unit = unit,
            lane = bossUnitData.lane,
            direction = bossUnitData.direction,
            lastX = x,
            lastY = y,
            nextRegion = bossUnitData.nextRegion,
            isBoss = false
        }

        XTD.Systems.ResultsManager.RegisterDamageTracking(unit)
        XTD.Systems.CreepManager.IncrementCreepsAlive()

        local gameSettings = XTD.Core.GameManager.GetGameSettings()

        local maxLife = XTD.Systems.UnitManager.GetMaxLife(unit)
        local newMaxLife = math.floor(maxLife * gameSettings.hpMultiplier)

        XTD.Systems.UnitManager.SetMaxLife(unit, newMaxLife)

        local baseSpeed = XTD.Systems.UnitManager.GetMovementSpeed(unit)
        local speed = math.floor(baseSpeed * gameSettings.movementSpeedMultiplier)

        XTD.Systems.UnitManager.SetMovementSpeed(unit, speed)
        XTD.Systems.UnitManager.DisableCollision(unit)

        XTD.Systems.CreepPathManager.OrderSummonedAddToNextRegion(unit, bossUnitData.nextRegion)

        DestroyEffect(
            AddSpecialEffect(
                "Abilities\\Spells\\Undead\\AnimateDead\\AnimateDeadTarget.mdl",
                GetUnitX(unit),
                GetUnitY(unit)
            )
        )
    end

    DestroyEffect(
        AddSpecialEffect(
            "Abilities\\Spells\\Other\\Doom\\DoomDeath.mdl",
            x,
            y
        )
    )
end

--[[
    Enrage the boss - basically makes the boss move faster.
]]
function XTD.Systems.BossManager.EnrageBoss(boss, speedBonus, bossName)
    if boss == nil then
        return
    end

    local currentSpeed = XTD.Systems.UnitManager.GetMovementSpeed(boss)

    XTD.Systems.UnitManager.SetMovementSpeed(boss, math.min(currentSpeed + speedBonus, 522))

    DestroyEffect(
        AddSpecialEffectTarget(
            "Abilities\\Spells\\Orc\\Bloodlust\\BloodlustTarget.mdl",
            boss,
            "origin"
        )
    )

    XTD.UI.MessageManager.Broadcast(
        XTD.Core.Color.Red(bossName .. " has enraged!")
    )
end