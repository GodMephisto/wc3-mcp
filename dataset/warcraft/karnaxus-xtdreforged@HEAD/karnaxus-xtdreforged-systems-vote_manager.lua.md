---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/vote_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Vote Manager                                        |
    | =================================================== |
    | Manages voting for players.                         |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.VoteManager = XTD.Systems.VoteManager or {}

XTD.Systems.VoteManager.activeVote = nil

--[[
    Initialize the Vote Manager.
]]
function XTD.Systems.VoteManager.Init()
    XTD.Core.Debug("VoteManager", "Initializing the Vote Manager...", "Init")
    XTD.Core.Debug("VoteManager", "Vote Manager initialized.", "Init")
end

--[[
    Start a new vote.
]]
function XTD.Systems.VoteManager.Start(config)
    if XTD.Systems.VoteManager.activeVote ~= nil then
        return
    end

    XTD.Systems.VoteManager.activeVote = {
        title = config.title,
        options = config.options,
        votes = {},
        duration = config.duration or 20.00,
        requiredVotes = XTD.Core.PlayerManager.TotalPlayers(),
        ended = false,
        onComplete = config.onComplete
    }

    local vote = XTD.Systems.VoteManager.activeVote

    local message =
        XTD.Core.Color.Yellow(vote.title) ..
        "\n\nType a number to vote:\n"

    for index, option in ipairs(vote.options) do
        message = message ..
            "\n" ..
            XTD.Core.Color.Blue(tostring(index)) ..
            " - " ..
            option.label
    end

    XTD.UI.MessageManager.Broadcast(message, vote.duration)

    XTD.Systems.VoteManager.RegisterChatVotes(vote)

    vote.timerData = XTD.Core.Timer.Window(vote.duration, XTD.Core.Color.BrightGreen(vote.title), function()
        XTD.Systems.VoteManager.Finish()
    end)
end

--[[
    Register chat votes.
]]
function XTD.Systems.VoteManager.RegisterChatVotes(vote)
    XTD.Core.PlayerManager.ForEach(function(playerData)
        for index, _ in ipairs(vote.options) do
            local optionIndex = index
            local trigger = CreateTrigger()

            TriggerRegisterPlayerChatEvent(
                trigger,
                playerData.player,
                tostring(optionIndex),
                true
            )

            TriggerAddAction(trigger, function()
                XTD.Core.Debug("VoteManager", "Vote chat detected")

                XTD.Systems.VoteManager.CastVote(
                    GetTriggerPlayer(),
                    optionIndex
                )
            end)
        end
    end)
end

--[[
    Cast a players vote.
]]
function XTD.Systems.VoteManager.CastVote(player, optionIndex)
    local vote = XTD.Systems.VoteManager.activeVote

    if vote == nil or vote.ended then
        return
    end

    local playerId = GetPlayerId(player)

    if vote.votes[playerId] ~= nil then
        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.Red("You have already voted."),
            5.00
        )
        return
    end

    local option = vote.options[optionIndex]

    if option == nil then
        return
    end

    vote.votes[playerId] = optionIndex

    local voteCount = XTD.Systems.VoteManager.GetVoteCount(vote)

    local playerData = XTD.Core.PlayerManager.GetPlayer(player)

    XTD.UI.MessageManager.Broadcast(
        XTD.Core.Color.Wrap(playerData.color, playerData.name) ..
        " voted for " ..
        option.label ..
        " " ..
        XTD.Core.Color.Blue("(" .. voteCount .. "/" .. vote.requiredVotes .. ")"),
        5.00
    )

    if voteCount >= vote.requiredVotes then
        XTD.Systems.VoteManager.Finish()
    end
end

--[[
    Finish a vote.
]]
function XTD.Systems.VoteManager.Finish()
    local vote = XTD.Systems.VoteManager.activeVote

    if vote == nil or vote.ended then
        return
    end

    vote.ended = true

    if vote.timerData then
        XTD.Core.Timer.DestroyWindow(vote.timerData)
        vote.timerData = nil
    end

    vote.ended = true

    XTD.Core.Timer.CancelWindow(vote.timerData);

    local counts = {}

    for index, _ in ipairs(vote.options) do
        counts[index] = 0
    end

    for _, optionIndex in pairs(vote.votes) do
        counts[optionIndex] = counts[optionIndex] + 1
    end

    local winningIndex = 1
    local winningVotes = counts[1]

    for index, count in ipairs(counts) do
        if count > winningVotes then
            winningIndex = index
            winningVotes = count
        end
    end

    local winner = vote.options[winningIndex];

    XTD.UI.MessageManager.Broadcast(
        XTD.Core.Color.Green("Vote complete: ") .. winner.label,
        10.00
    )

    XTD.Systems.VoteManager.activeVote = nil

    if vote.onComplete then
        vote.onComplete(winner)
    end
end

--[[
    Get the vote count.
]]
function XTD.Systems.VoteManager.GetVoteCount(vote)
    local count = 0

    for _, _ in pairs(vote.votes) do
        count = count + 1
    end

    return count
end

--[[
    Start a kick vote to kick a player.
]]
function XTD.Systems.VoteManager.StartKickVote(initiator, targetPlayer)
    local initiatorId = GetPlayerId(initiator)
    local targetId = GetPlayerId(targetPlayer)

    if initiatorId == targetId then
        XTD.UI.MessageManager.Player(initiator, XTD.Core.Color.Red("You cannot vote kick yourself."))
        return
    end

    local targetData = XTD.Core.PlayerManager.GetPlayer(targetPlayer)

    if targetData == nil then
        XTD.UI.MessageManager.Player(initiator, XTD.Core.Color.Red("Invalid player."))
        return
    end

    XTD.Systems.VoteManager.Start({
        title = "Vote Kick " .. XTD.Core.Color.Wrap(targetData.color, targetData.name) .. "?",
        duration = 20.00,

        options = {
            { label = XTD.Core.Color.CoolGreen("Yes"), value = true },
            { label = XTD.Core.Color.Red("No"), value = false }
        },

        onComplete = function (winner)
            if winner.value == true then
                XTD.Core.PlayerManager.Kick(targetPlayer)
            else    
                XTD.UI.MessageManager.Broadcast(
                    XTD.Core.Color.Red("Vote kick failed.")
                )
            end
        end
    })
end