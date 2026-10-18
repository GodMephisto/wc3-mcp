---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/interval_gold_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Gold Interval Manager                               |
    | =================================================== |
    | Manages giving players gold on an interval.         |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.IntervalGoldManager = XTD.Systems.IntervalGoldManager or {}

XTD.Systems.IntervalGoldManager.timer = nil
XTD.Systems.IntervalGoldManager.remaining = 0

--[[
    Initialize the Interval Gold Manager.
]]
function XTD.Systems.IntervalGoldManager.Init()
    XTD.Core.Debug("IntervalGoldManager", "Initializing the Interval Gold Manager...", "Init")

    local gameSettings = XTD.Core.GameManager.GetGameSettings()

    XTD.Systems.IntervalGoldManager.remaining = gameSettings.intervalGoldSeconds or 15

    XTD.Systems.IntervalGoldManager.timer = CreateTimer()

    TimerStart(XTD.Systems.IntervalGoldManager.timer, 1.00, true, function ()
        XTD.Systems.IntervalGoldManager.Tick()
    end)

    XTD.Core.Debug("IntervalGoldManager", "Interval Gold Manager initialized.", "Init")
end

--[[
    Process one interval gold timer tick.
]]
function XTD.Systems.IntervalGoldManager.Tick()
    local gameSettings = XTD.Core.GameManager.GetGameSettings()

    XTD.Systems.IntervalGoldManager.remaining = XTD.Systems.IntervalGoldManager.remaining - 1

    if XTD.Systems.IntervalGoldManager.remaining <= 0 then
        XTD.Systems.IntervalGoldManager.GiveGold()

        XTD.Systems.IntervalGoldManager.remaining = gameSettings.intervalGoldSeconds or 15
    end
end

--[[
    Give interval gold to every active player.
]]
function XTD.Systems.IntervalGoldManager.GiveGold()
    local gameSettings = XTD.Core.GameManager.GetGameSettings()
    local goldAmount = gameSettings.intervalGoldAmount or 0

    if goldAmount <= 0 then
        return
    end

    XTD.Core.PlayerManager.ForEach(function (playerData)
        if not XTD.Core.PlayerManager.hasLeft[playerData.id] then
            XTD.Core.PlayerManager.AddResource(
                playerData.player,
                "gold",
                goldAmount
            )
        end
    end)

    XTD.Core.Debug("IntervalGoldManager", "Gave " .. tostring(goldAmount) .. " gold to each active player.", "GiveGold")
end

--[[
    Get the current interval gold countdown.
]]
function XTD.Systems.IntervalGoldManager.GetRemaining()
    local remaining = XTD.Systems.IntervalGoldManager.remaining or 0

    local text =
        XTD.Core.Color.AncientGold(tostring(remaining)) .. " " ..
        XTD.Core.Color.BrightGreen(
            XTD.Core.Helpers.SingularOrPlural(
                "second",
                "seconds",
                remaining
            )
        )

    return text
end