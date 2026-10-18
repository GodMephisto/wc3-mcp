---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "ui/message_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Message Manager                                     |
    | =================================================== |
    | Manages displaying messages to players.             |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.UI.MessageManager = XTD.UI.MessageManager or {}

--[[
    Broadcast text to all players.
]]
function XTD.UI.MessageManager.Broadcast(message, duration)
    if message == nil then
        return
    end

    DisplayTimedTextToForce(
        GetPlayersAll(),
        duration or 20.00,
        message
    )
end

--[[
    Display text to a specific player.
]]
function XTD.UI.MessageManager.Player(player, message, duration)
    if player == nil or message == nil then
        return
    end

    DisplayTimedTextToPlayer(
        player,
        0,
        0,
        duration or 20.00,
        message
    )
end

--[[
    Broadcast multiple messages over time.
]]
function XTD.UI.MessageManager.BroadcastMultipleMessagesOverTime(messages, delay)
    if messages == nil then
        return
    end

    if type(messages) ~= "table" then
        return
    end

    delay = delay or 1.00

    local index = 1

    local timer = CreateTimer()

    TimerStart(timer, delay, true, function ()
        if index > #messages then
            PauseTimer(timer)
            DestroyTimer(timer)
            return
        end

        if messages[index] == nil or messages[index] == "" then
            XTD.UI.MessageManager.Broadcast("\n")
        else 
            XTD.UI.MessageManager.Broadcast(messages[index])
        end

        index = index + 1
    end)
end