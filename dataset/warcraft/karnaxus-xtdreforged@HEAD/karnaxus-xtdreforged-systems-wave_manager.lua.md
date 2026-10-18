---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/wave_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Wave Manager                                        |
    | =================================================== |
    | Manages the waves of creeps.                        |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.WaveManager = XTD.Systems.WaveManager or {}

XTD.Systems.WaveManager.currentWave = 0
XTD.Systems.WaveManager.spawnDisabled = false
XTD.Systems.WaveManager.customWave = false
XTD.Systems.WaveManager.betweenWaveEnabled = true
XTD.Systems.WaveManager.wavesPaused = false

--[[
    Initialize the Wave Manager.
]]
function XTD.Systems.WaveManager.Init()
    XTD.Core.Debug("WaveManager", "Initializing the Wave Manager...")

    XTD.Systems.WaveManager.betweenWaveEnabled = true
    XTD.Systems.WaveManager.currentWave = 0
    XTD.Systems.WaveManager.spawnDisabled = false
    XTD.Systems.WaveManager.wavesPaused = false

    XTD.Core.Debug("WaveManager", "Wave Manager initialized.")
end

--[[
    Begin the next wave of creeps.
]]
function XTD.Systems.WaveManager.BeginNextWave()
    local self = XTD.Systems.WaveManager

    if XTD.Systems.WaveManager.wavesPaused then
        return
    end

    if self.currentWave == 0 then
        local waveSettings = XTD.Data.Waves[self.currentWave + 1]
        local gold = waveSettings.goldForWave

        XTD.Core.PlayerManager.AddResourceToAllPlayers("gold", gold)

        self.GoldForNextWaveMessage(gold)
    end

    if XTD.Core.startAtWave and not self.customWave then
        self.currentWave = XTD.Core.startAtWave
        self.customWave = true
    else
        self.currentWave = self.currentWave + 1
    end

    XTD.Core.PlayerManager.UpdateNativeUIForAllPlayers()

    XTD.Systems.ResultsManager.StartKillRecordForWave(self.currentWave)
    XTD.Systems.ResultsManager.StartDamageRecordForWave(self.currentWave)
    XTD.Systems.ResultsManager.StartWaveTimer(self.currentWave)

    self.WaveNotifications()

    StartSound(gg_snd_Spawning)

    XTD.UI.MessageManager.Broadcast(
        XTD.Core.Helpers.CreateTextHeader(
            "Wave " .. tostring(self.currentWave) .. " Begins",
            "BrightBlue",
            "AncientGold"
        ) ..
        XTD.Core.Color.AncientGold("\n\n Creeps are spawning, get ready...\n\n")
    )

    local enabledLanes = XTD.Systems.LaneManager.GetEnabledLanes()

    for lane, enabled in pairs(enabledLanes) do
        if enabled then
            local spawn = XTD.Systems.RegionManager.ResolveRegion(XTD.Data.Regions.Lanes[lane].Spawn)

            if spawn ~= nil then
                local x, y = XTD.Systems.RegionManager.GetRectCenter(spawn)

                if x ~= nil and y ~= nil then
                    PingMinimap(x, y, 5.00)
                end
            end
        end
    end

    local gameSettings = XTD.Core.GameManager.GetGameSettings()
    local waveConfig = XTD.Data.Waves[self.currentWave]

    if gameSettings == nil then
        XTD.Core.Debug("WaveManager", "gameSettings is nil.", "BeginNextWave")
        return
    end

    if waveConfig == nil then
        XTD.Core.Debug("WaveManager", "waveConfig is nil.", "BeginNextWave")
        return
    end

    local totalCreeps = math.ceil(waveConfig.total * gameSettings.creepMultiplier)

    if waveConfig.category == XTD.Data.WaveCategories.BOSS then
        totalCreeps = 1
    end

    XTD.Systems.SpawnManager.SpawnWave(
        waveConfig.unitType,
        totalCreeps,
        waveConfig.spawnDelay,
        function (totalSpawned)
            self.OnSpawnComplete(totalSpawned)
        end
    )
end

--[[
    Handle when spawning has finished.
]]
function XTD.Systems.WaveManager.OnSpawnComplete(totalSpawned)
    if totalSpawned == nil then
        XTD.Core.Debug("WaveManager", "totalSpawned is nil.", "OnSpawnComplete")
        return
    end

    StartSound(gg_snd_Spawned)

    local waveSettings = XTD.Data.Waves[XTD.Systems.WaveManager.currentWave]
    local unitName = waveSettings.name

    XTD.UI.MessageManager.Broadcast(
        XTD.Core.Helpers.CreateTextHeader(
            "Wave " .. tostring(XTD.Systems.WaveManager.currentWave) .. " Spawning Complete!",
            "BrightBlue",
            "AncientGold"
        ) .. "\n\n" ..
        XTD.Core.Color.BrightGreen(tostring(totalSpawned)) .. " " ..
        XTD.Core.Color.BrightBlue(unitName) ..
        XTD.Core.Color.AncientGold(" spawned.\n") ..
        XTD.Core.Color.AncientGold("Armor: ") ..
        XTD.Core.Color.Ivory(waveSettings.armorType) ..
        XTD.Core.Color.AncientGold(" | Base: ") ..
        XTD.Core.Color.Ivory(tostring(waveSettings.armorBase)) ..
        XTD.Core.Color.AncientGold(" | Bonus: ") ..
        XTD.Core.Color.Ivory(tostring(waveSettings.armorBonus)) ..
        XTD.Core.Color.AncientGold(" | Type: ") ..
        XTD.Core.Color.Ivory(waveSettings.defenseType) .. "\n\n" ..
        XTD.Core.Color.Ivory(waveSettings.description) .. "\n\n"
    )

    local tag = XTD.Systems.CreepBoxManager.textTags[XTD.Systems.WaveManager.currentWave]

    if tag ~= nil then
        XTD.UI.WorldLabelManager.HideWorldLabel(tag)
        RemoveUnit(XTD.Systems.CreepBoxManager.boxCreeps[XTD.Systems.WaveManager.currentWave])
    end
end

--[[
    Notify the players on special waves of creeps.
]]
function XTD.Systems.WaveManager.WaveNotifications()
    local waveSettings = XTD.Data.Waves[XTD.Systems.WaveManager.currentWave]
    local waveCategories = XTD.Data.WaveCategories

    if waveSettings.category == waveCategories.AIR then
        StartSound(gg_snd_AirWave)

        XTD.UI.MessageManager.Broadcast(
            XTD.Core.Helpers.CreateTextHeader(
                "WARNING - AIR WAVE!",
                "Red",
                "AncientGold"
            ) ..
            XTD.Core.Color.BrightBlue("\nFlying creeps have been detected!\n") ..
            XTD.Core.Color.AncientGold("Ensure your defenses can attack air units.\n\n")
        )
    elseif waveSettings.category == waveCategories.BONUS then
        StartSound(gg_snd_BonusWave)

        XTD.UI.MessageManager.Broadcast(
            XTD.Core.Helpers.CreateTextHeader(
                "BONUS WAVE!",
                "Red",
                "AncientGold"
            ) ..
            XTD.Core.Color.BrightBlue("\nBonus creeps have been detected!\n") ..
            XTD.Core.Color.AncientGold("These creeps offer higher rewards, but harder to kill. Don't worry, these creeps don't count as leaks.\n\n")
        )
    elseif waveSettings.category == waveCategories.BOSS then
        StartSound(gg_snd_BossWave)

        XTD.UI.MessageManager.Broadcast(
            XTD.Core.Helpers.CreateTextHeader(
                "WARNING - BOSS WAVE!",
                "Red",
                "AncientGold"
            ) ..
            XTD.Core.Color.BrightBlue("\nA boss has emerged.\n") ..
            XTD.Core.Color.AncientGold("Bosses posses special abilities that normal creeps do not, but do offer greater rewards. Good luck!\n\n")
        )
    end
end

--[[
    Return whether current wave is an air wave.
]]
function XTD.Systems.WaveManager.IsAirWave()
    local waveSettings = XTD.Data.Waves[XTD.Systems.WaveManager.currentWave]
    return waveSettings.category == XTD.Data.WaveCategories.AIR
end

--[[
    Return whether current wave is an bonus wave.
]]
function XTD.Systems.WaveManager.IsBonusWave()
    local waveSettings = XTD.Data.Waves[XTD.Systems.WaveManager.currentWave]
    return waveSettings.category == XTD.Data.WaveCategories.BONUS
end

--[[
    Return whether current wave is an boss wave.
]]
function XTD.Systems.WaveManager.IsBossWave()
    local waveSettings = XTD.Data.Waves[XTD.Systems.WaveManager.currentWave]
    return waveSettings.category == XTD.Data.WaveCategories.BOSS
end

--[[
    Display the gold for the next wave message.
]]
function XTD.Systems.WaveManager.GoldForNextWaveMessage(total)
    XTD.UI.MessageManager.Broadcast(
            XTD.Core.Color.AncientGold("\nYou just received ") ..
            XTD.Core.Color.BrightGreen(tostring(total)) ..
            XTD.Core.Color.AncientGold(" gold for the incoming wave.\n")
        )
end

--[[
    Handle when the wave is complete.
]]
function XTD.Systems.WaveManager.OnWaveComplete()
    local completedWave = XTD.Systems.WaveManager.currentWave
    local nextWave = completedWave + 1

    if XTD.Systems.WaveManager.spawnDisabled == true then
        XTD.Systems.ResultsManager.WaveEndSummary(completedWave)
        return
    end

    local waveSettings = XTD.Data.Waves[nextWave]

    if waveSettings ~= nil then
        local totalGold = waveSettings.goldForWave

        XTD.Core.PlayerManager.AddResourceToAllPlayers("gold", totalGold)

        XTD.Systems.WaveManager.GoldForNextWaveMessage(totalGold)
    end

    if nextWave <= XTD.Constants.MAX_WAVES then
        local gameSettings = XTD.Core.GameManager.GetGameSettings()

        if gameSettings.betweenWaveEnabled == true and XTD.Systems.WaveManager.betweenWaveEnabled then
            XTD.Core.Timer.Window(
                gameSettings.betweenWaveDelay,
                XTD.Core.Color.CoolGreen("Wave ") ..
                XTD.Core.Color.AncientGold(tostring(nextWave)) ..
                XTD.Core.Color.CoolGreen(" in..."),
                function ()
                    XTD.Systems.WaveManager.BeginNextWave()
                end,
                "waves"
            )
        else    
            XTD.Systems.WaveManager.BeginNextWave()
        end
    end

    XTD.Systems.ResultsManager.WaveEndSummary(completedWave)

    if completedWave == XTD.Constants.MAX_WAVES then
        XTD.Core.GameManager.DeclareVictory()
        return
    end
end