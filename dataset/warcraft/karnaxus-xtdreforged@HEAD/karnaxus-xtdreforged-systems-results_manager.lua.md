---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/results_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Results Manager                                     |
    | =================================================== |
    | Manage game statistics and results.                 |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.ResultsManager = XTD.Systems.ResultsManager or {}

XTD.Systems.ResultsManager.mostKills = XTD.Systems.ResultsManager.mostKills or {}
XTD.Systems.ResultsManager.currentKills = XTD.Systems.ResultsManager.currentKills or {}
XTD.Systems.ResultsManager.calculatedKills = XTD.Systems.ResultsManager.calculatedKills or {}
XTD.Systems.ResultsManager.currentDamage = XTD.Systems.ResultsManager.currentDamage or {}
XTD.Systems.ResultsManager.waveTimers = XTD.Systems.ResultsManager.waveTimers or {}
XTD.Systems.ResultsManager.waveTimes = XTD.Systems.ResultsManager.waveTimes or {}
XTD.Systems.ResultsManager.waveEndSummaryEnabled = XTD.Systems.ResultsManager.waveEndSummaryEnabled or {}

--[[
    Initialize the Results Manager.
]]
function XTD.Systems.ResultsManager.Init()
    XTD.Core.Debug("ResultsManager", "Initializing the Results Manager...", "Init")

    XTD.Systems.ResultsManager.mostKills = {}
    XTD.Systems.ResultsManager.currentKills = {}
    XTD.Systems.ResultsManager.calculatedKills = {}
    XTD.Systems.ResultsManager.currentDamage = {}
    XTD.Systems.ResultsManager.waveTimers = {}
    XTD.Systems.ResultsManager.waveTimes = {}

    XTD.Core.PlayerManager.ForEach(function (playerData)
        XTD.Systems.ResultsManager.waveEndSummaryEnabled[playerData.id] = true
    end)

    XTD.Core.Debug("ResultsManager", "Results Manager initialized.", "Init")
end

--[[
    Start recording the total kills for each player a wave.
]]
function XTD.Systems.ResultsManager.StartKillRecordForWave(wave)
    local self = XTD.Systems.ResultsManager

    self.currentKills[wave] = {}
    self.calculatedKills[wave] = {}

    XTD.Core.PlayerManager.ForEach(function (playerData)
        self.currentKills[wave][playerData.id] = playerData.kills or 0
    end)
end

--[[
    Stop recording total kills and determines the top killer for the wave.
]]
function XTD.Systems.ResultsManager.StopKillRecord(wave)
    local self = XTD.Systems.ResultsManager

    if self.currentKills[wave] == nil then
        XTD.Core.Debug("ResultsManager", "currentKills for wave " .. wave .. " is nil.", "StopKillRecord")
        return
    end

    self.calculatedKills[wave] = {}

    XTD.Core.PlayerManager.ForEach(function (playerData)
        self.calculatedKills[wave][playerData.id] = (playerData.kills or 0) - (self.currentKills[wave][playerData.id] or 0)
    end)

    local top = { total = 0, playerId = nil }

    XTD.Core.PlayerManager.ForEach(function (playerData)
        if self.calculatedKills[wave][playerData.id] > top.total then
            top.total = self.calculatedKills[wave][playerData.id]
            top.playerId = playerData.id
        end
    end)

    if top == nil or top.total == nil or top.playerId == nil then
        XTD.Core.Debug("ResultsManager", "top is nil.", "StopKillRecord")
        return
    end

    local player = Player(top.playerId)
    local playerInfo = XTD.Core.PlayerManager.GetPlayer(player)

    if playerInfo == nil then
        XTD.Core.Debug("ResultsManager", "playerInfo is nil.", "StopKillRecord")
        return
    end

    local topKiller = playerInfo
    local totalKills = top.total

    return topKiller, totalKills
end

--[[
    Start recording the total damage for all players for the wave.
]]
function XTD.Systems.ResultsManager.StartDamageRecordForWave(wave)
    local self = XTD.Systems.ResultsManager

    self.currentDamage[wave] = {}

    XTD.Core.PlayerManager.ForEach(function (playerData)
        self.currentDamage[wave][playerData.id] = 0
    end)
end

--[[
    Register damage tracking for determining the most damage done for a wave.
]]
function XTD.Systems.ResultsManager.RegisterDamageTracking(unit)
    local self = XTD.Systems.ResultsManager
    local trigger = CreateTrigger()

    TriggerRegisterUnitEvent(trigger, unit, EVENT_UNIT_DAMAGED)

    TriggerAddAction(trigger, function ()
        local damageSource = GetEventDamageSource()
        local damage = GetEventDamage()

        if damageSource == nil or damage <= 0 then
            return
        end

        local player = GetOwningPlayer(damageSource)
        local playerData = XTD.Core.PlayerManager.GetPlayer(player)

        if playerData == nil then
            XTD.Core.Debug("ResultsManager", "playerData is nil.", "RegisterDamageTracking")
            return
        end

        local wave = XTD.Systems.WaveManager.currentWave

        if self.currentDamage[wave] == nil then
            XTD.Core.Debug("ResultsManager", "currentDamage for wave " .. wave .. " is nil", "RegisterDamageTracking")
            return
        end

        self.currentDamage[wave][playerData.id] = (self.currentDamage[wave][playerData.id] or 0) + damage
    end)
end

--[[
    Stop recording damage and then determines the top damager for the wave.
]]
function XTD.Systems.ResultsManager.StopDamageRecord(wave)
    local self = XTD.Systems.ResultsManager

    local damageTable = self.currentDamage[wave]

    if damageTable == nil then
        XTD.Core.Debug("ResultsManager", "damageTable is nil.", "StopDamageRecord")
        return
    end

    local top = { total = 0, playerId = nil }

    XTD.Core.PlayerManager.ForEach(function (playerData)
        local damage = damageTable[playerData.id] or 0

        if damage > top.total then
            top.total = damage
            top.playerId = playerData.id
        end
    end)

    if top.playerId == nil or top.total <= 0 then
        XTD.Core.Debug("ResultsManager", "top is nil.", "StopDamageRecord")
        return
    end

    local player = Player(top.playerId)
    local playerInfo = XTD.Core.PlayerManager.GetPlayer(player)

    if playerInfo == nil then
        XTD.Core.Debug("ResultsManager", "playerInfo is nil.", "StopDamageRecord")
        return
    end

    local topDamager = playerInfo
    local totalDamage = XTD.Core.Helpers.FormatNumber(top.total)

    return topDamager, totalDamage
end

--[[
    Start the timer for a wave.
]]
function XTD.Systems.ResultsManager.StartWaveTimer(wave)
    local timer = CreateTimer()

    XTD.Systems.ResultsManager.waveTimers[wave] = timer

    TimerStart(timer, 999999.00, false, function () end)
end

--[[
    Stop the wave timer.
]]
function XTD.Systems.ResultsManager.StopWaveTimer(wave)
    local self = XTD.Systems.ResultsManager

    local timer = self.waveTimers[wave]

    if timer == nil then
        XTD.Core.Debug("ResultsManager", "timer is nil.", "StopWaveTimer")
        return
    end

    local elapsed = TimerGetElapsed(timer)

    self.waveTimes[wave] = elapsed

    PauseTimer(timer)
    DestroyTimer(timer)

    self.waveTimers[wave] = nil

    local formattedTime = XTD.Core.Helpers.FormatTime(self.waveTimes[wave])

    if formattedTime == nil then
        XTD.Core.Debug("ResultsManager", "formattedTime is nil.", "StopWaveTimer")
        return
    end

    return formattedTime
end

--[[
    Build the summary for a wave at the end.
]]
function XTD.Systems.ResultsManager.WaveEndSummary(wave)
    local self = XTD.Systems.ResultsManager

    if XTD.Core.waveEndSummary == false then
        StartSound(gg_snd_WaveComplete)
        XTD.UI.MessageManager.Broadcast(XTD.Systems.WaveManager.BuildWaveCompleteText(wave))
        return
    end

    local topKiller, totalKills = self.StopKillRecord(wave)
    local topDamager, totalDamage = self.StopDamageRecord(wave)
    local formattedTime = self.StopWaveTimer(wave)

    if topKiller == nil or totalKills == nil or topDamager == nil or totalDamage == nil or formattedTime == nil then
        return
    end

    local summary =
        XTD.Core.Color.BrightBlue("===============================\n") ..
        XTD.Core.Color.Yellow("Wave ") ..
        XTD.Core.Color.BrightGreen(tostring(wave)) ..
        XTD.Core.Color.Yellow(" Complete!\n\n") ..
        XTD.Core.Color.Teal("Top Killer:\n") ..
        XTD.Core.Color.Wrap(topKiller.color, topKiller.name) ..
        XTD.Core.Color.Yellow(" - ") ..
        XTD.Core.Color.BrightGreen(tostring(totalKills)) ..
        XTD.Core.Color.Yellow(" " .. XTD.Core.Helpers.SingularOrPlural("kill", "kills", totalKills)) .. "\n\n" ..
        XTD.Core.Color.Teal("Top Damager:\n") ..
        XTD.Core.Color.Wrap(topDamager.color, topDamager.name) ..
        XTD.Core.Color.Yellow(" - ") ..
        XTD.Core.Color.BrightGreen(totalDamage) ..
        XTD.Core.Color.Yellow(" damage\n\n") ..
        XTD.Core.Color.Teal("Wave Time\n") ..
        XTD.Core.Color.BrightGreen(formattedTime) .. "\n" ..
        XTD.Core.Color.BrightBlue("===============================")

    XTD.Core.PlayerManager.ForEach(function (playerData)
        local playerId = playerData.id

        StartSound(gg_snd_WaveComplete)

        if self.waveEndSummaryEnabled[playerId] == true then
            XTD.UI.MessageManager.Player(
                playerData.player,
                summary
            )
        else
            XTD.UI.MessageManager.Player(
                playerData.player,
                XTD.Systems.WaveManager.BuildWaveCompleteText(wave)
            )
        end
    end)
end