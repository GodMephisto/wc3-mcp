---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/command_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Command Manager                                     |
    | =================================================== |
    | Manages commands from players.                      |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.CommandManager = XTD.Systems.CommandManager or {}

XTD.Systems.CommandManager.commands = {}

--[[
    Initialize the Command Manager.
]]
function XTD.Systems.CommandManager.Init()
    XTD.Core.Debug("CommandManager", "Initializing the Command Manager...", "Init")

    XTD.Systems.CommandManager.RegisterCommands()
    XTD.Systems.CommandManager.RegisterChatTriggers()

    XTD.Core.Debug("CommandManager", "Command Manager initialized.", "Init")
end

--[[
    Register a new command.
]]
function XTD.Systems.CommandManager.Register(command, callback, developerOnly, description)
    XTD.Systems.CommandManager.commands[string.lower(command)] = {
        callback = callback,
        developerOnly = developerOnly or false,
        description = description or ""
    }
end

--[[
    Build the command list.
]]
function XTD.Systems.CommandManager.BuildCommandList(includeDeveloper)
    local lines = {}

    for command, data in pairs(XTD.Systems.CommandManager.commands) do
        if command ~= "/commands" then
            if not data.developerOnly or includeDeveloper then
                local line =
                    XTD.Core.Color.BrightGreen(command .. " ") ..
                    XTD.Core.Color.BrightBlue(data.description or "")

                table.insert(lines, line)
            end
        end
    end

    table.sort(lines)

    return lines
end

--[[
    Register the commands.
]]
function XTD.Systems.CommandManager.RegisterCommands()
    XTD.Systems.CommandManager.Register("/enable debug", function (player)
        XTD.Core.debugEnabled = true
        XTD.UI.MessageManager.Player(player, XTD.Core.Color.BrightGreen("Debug enabled."))
    end, false, "Turn on debug mode")

    XTD.Systems.CommandManager.Register("/disable debug", function (player)
        XTD.Core.debugEnabled = true
        XTD.UI.MessageManager.Player(player, XTD.Core.Color.BrightGreen("Debug disabled."))
    end, false, "Turn off debug mode")

    XTD.Systems.CommandManager.Register("/disable tips", function (player)
        local playerId = GetPlayerId(player)

        XTD.UI.TipsManager.enabled[playerId] = false

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.Red("Tips disabled. ") ..
            XTD.Core.Color.Yellow("Type ") ..
            XTD.Core.Color.CoolGreen("/enable tips ") ..
            XTD.Core.Color.Yellow("to turn them back on.")
        )
    end, false, "Disable tips")

    XTD.Systems.CommandManager.Register("/enable tips", function (player)
        local playerId = GetPlayerId(player)

        XTD.UI.TipsManager.enabled[playerId] = true

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.CoolGreen("Tips enabled.")
        )
    end, false, "Enable tips")

    XTD.Systems.CommandManager.Register("/givegold", function(player, args)
        if #args < 3 then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("Usage: /givegold <player|all> <amount>")
            )
            return
        end

        local playerData = XTD.Core.PlayerManager.GetPlayer(player)

        if playerData == nil then
            return
        end

        local target = string.lower(args[2])
        local amount = tonumber(args[3])

        if amount == nil or amount <= 0 then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("Invalid gold amount.")
            )
            return
        end

        local currentGold = GetPlayerState(player, PLAYER_STATE_RESOURCE_GOLD)

        if currentGold < amount then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("You only have ") ..
                XTD.Core.Color.Yellow(tostring(currentGold)) ..
                XTD.Core.Color.Red(" gold.")
            )
            return
        end

        if target == "all" then
            local recipients = 0

            for i = 0, 7 do
                local p = Player(i)

                if p ~= player and XTD.Core.PlayerManager.IsPlaying(p) then
                    recipients = recipients + 1
                end
            end

            if recipients == 0 then
                XTD.UI.MessageManager.Player(
                    player,
                    XTD.Core.Color.Red("There are no other players.")
                )
                return
            end

            local totalCost = amount * recipients

            if currentGold < totalCost then
                XTD.UI.MessageManager.Player(
                    player,
                    XTD.Core.Color.Red("You need ") ..
                    XTD.Core.Color.Yellow(tostring(totalCost)) ..
                    XTD.Core.Color.Red(" gold to give ") ..
                    XTD.Core.Color.Yellow(tostring(amount)) ..
                    XTD.Core.Color.Red(" to everyone.")
                )
                return
            end

            AdjustPlayerStateBJ(-totalCost, player, PLAYER_STATE_RESOURCE_GOLD)

            for i = 0, 7 do
                local p = Player(i)

                if p ~= player and XTD.Core.PlayerManager.IsPlaying(p) then
                    AdjustPlayerStateBJ(amount, p, PLAYER_STATE_RESOURCE_GOLD)
                end
            end

            XTD.UI.MessageManager.Broadcast(
                XTD.Core.Color.Wrap(playerData.color, playerData.name) ..
                XTD.Core.Color.White(" gave ") ..
                XTD.Core.Color.Yellow(tostring(amount)) ..
                XTD.Core.Color.White(" gold to everyone.")
            )

            return
        end

        local playerId = tonumber(target)

        if playerId == nil or playerId < 1 or playerId > 8 then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("Player must be between 1 and 8.")
            )
            return
        end

        local targetPlayer = Player(playerId - 1)

        if targetPlayer == player then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("You cannot give gold to yourself.")
            )
            return
        end

        if not XTD.Core.PlayerManager.IsPlaying(targetPlayer) then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("That player is not currently playing.")
            )
            return
        end

        local targetPlayerData = XTD.Core.PlayerManager.GetPlayer(targetPlayer)

        if targetPlayerData == nil then
            return
        end

        AdjustPlayerStateBJ(-amount, player, PLAYER_STATE_RESOURCE_GOLD)
        AdjustPlayerStateBJ(amount, targetPlayer, PLAYER_STATE_RESOURCE_GOLD)

        XTD.UI.MessageManager.Broadcast(
            XTD.Core.Color.Wrap(playerData.color, playerData.name) ..
            XTD.Core.Color.White(" gave ") ..
            XTD.Core.Color.Yellow(tostring(amount)) ..
            XTD.Core.Color.White(" gold to ") ..
            XTD.Core.Color.Wrap(targetPlayerData.color, targetPlayerData.name) ..
            XTD.Core.Color.White(".")
        )
    end, false, "Give gold to another player or all players")

    XTD.Systems.CommandManager.Register("/wave", function (player)
        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.BrightBlue("Current Wave: ") .. 
            XTD.Core.Color.BrightGreen(tostring(XTD.Systems.WaveManager.currentWave))
        )
    end, false, "Show the current wave")

    XTD.Systems.CommandManager.Register("/nextwave", function (player)
        local nextWave = XTD.Systems.WaveManager.currentWave + 1

        if nextWave > XTD.Constants.MAX_WAVES then
            nextWave = 40
        end

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.BrightBlue("Next Wave: ") ..
            XTD.Core.Color.BrightGreen(tostring(nextWave))
        )
    end, false, "Show the next wave")

    XTD.Systems.CommandManager.Register("/air", function (player)
        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.BrightBlue("Air Waves: ") ..
            XTD.Core.Color.BrightGreen("5, 10, 15, 25, and 35")
        )
    end, false, "Show air waves")

    XTD.Systems.CommandManager.Register("/bonus", function (player)
        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.BrightBlue("Bonus Waves: ") ..
            XTD.Core.Color.BrightGreen("31 and 36")
        )
    end, false, "Show bonus waves")

    XTD.Systems.CommandManager.Register("/boss", function (player)
        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.BrightBlue("Boss Waves: ") ..
            XTD.Core.Color.BrightGreen("20, 30, and 40")
        )
    end, false, "Show boss waves")

    XTD.Systems.CommandManager.Register("/allowbuild", function(player, args)
        local targetArg = args[2]

        if targetArg == nil then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("Usage: ") ..
                XTD.Core.Color.Yellow("/allowbuild <player|all>")
            )
            return
        end

        local ownerId = GetPlayerId(player)

        if string.lower(targetArg) == "all" then
            XTD.Systems.BuildZoneManager.SetPermission(ownerId, "all", true)

            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.CoolGreen("Everyone may now build in your build area.")
            )

            return
        end

        local targetPlayer = XTD.Systems.CommandManager.GetPlayerFromArgument(targetArg)

        if targetPlayer == nil then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("Invalid player. Use a player number, color, or ") ..
                XTD.Core.Color.Yellow("all")
            )
            return
        end

        XTD.Systems.BuildZoneManager.SetPermission(
            ownerId,
            GetPlayerId(targetPlayer),
            true
        )

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.CoolGreen(GetPlayerName(targetPlayer)) ..
            XTD.Core.Color.Yellow(" may now build in your build area.")
        )
    end, false, "Allow players to build in your zone")

    XTD.Systems.CommandManager.Register("/denybuild", function(player, args)
        local targetArg = args[2]

        if targetArg == nil then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("Usage: ") ..
                XTD.Core.Color.Yellow("/denybuild <player|all>")
            )
            return
        end

        local ownerId = GetPlayerId(player)

        if string.lower(targetArg) == "all" then
            XTD.Systems.BuildZoneManager.SetPermission(ownerId, "all", false)

            -- Always allow yourself.
            XTD.Systems.BuildZoneManager.SetPermission(ownerId, ownerId, true)

            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("Only you may now build in your build area.")
            )

            return
        end

        local targetPlayer = XTD.Systems.CommandManager.GetPlayerFromArgument(targetArg)

        if targetPlayer == nil then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("Invalid player. Use a player number, color, or ") ..
                XTD.Core.Color.Yellow("all")
            )
            return
        end

        XTD.Systems.BuildZoneManager.SetPermission(
            ownerId,
            GetPlayerId(targetPlayer),
            false
        )

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.Red(GetPlayerName(targetPlayer)) ..
            XTD.Core.Color.Yellow(" may no longer build in your build area.")
        )
    end, false, "Deny players from building in your zone")

    XTD.Systems.CommandManager.Register("/disable wavesummary", function (player)
        local playerId = GetPlayerId(player)

        XTD.Systems.ResultsManager.waveEndSummaryEnabled[playerId] = false

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.Red("Wave end summary disabled. ") ..
            XTD.Core.Color.Yellow("Type ") ..
            XTD.Core.Color.CoolGreen("/enable wavesummary ") ..
            XTD.Core.Color.Yellow("to turn back on.")
        )
    end, false, "Disable wave end summary")

    XTD.Systems.CommandManager.Register("/enable wavesummary", function (player)
        local playerId = GetPlayerId(player)

        XTD.Systems.ResultsManager.waveEndSummaryEnabled[playerId] = true

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.CoolGreen("Wave end summary enabled.")
        )
    end, false, "Enable wave end summary")

    XTD.Systems.CommandManager.Register("/kickvote", function (player, args)
        local targetArg = args[2]

        if targetArg == nil then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("Usage: ") .. XTD.Core.Color.Yellow("/kickvote <player>")
            )

            return
        end

        local targetPlayer = XTD.Systems.CommandManager.GetPlayerFromArgument(targetArg)

        if targetPlayer == nil then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("Invalid player. Use color or number, like ") ..
                XTD.Core.Color.Yellow("/kickplayer red")
            )

            return
        end

        XTD.Systems.VoteManager.StartKickVote(player, targetPlayer)
    end, false, "Start a new kick vote to potentially kick a player")

    XTD.Systems.CommandManager.Register("/music next", function (player)
        XTD.Systems.MusicManager.SkipTrack()

        local playerData = XTD.Core.PlayerManager.GetPlayer(player)

        if playerData ~= nil then
            XTD.UI.MessageManager.Broadcast(
                XTD.Core.Color.Wrap(playerData.color, playerData.name) ..
                XTD.Core.Color.Ivory(" has moved background music to the next track.")
            )
        end
    end, false, "Move background music to the next track")

    XTD.Systems.CommandManager.Register("/music pause", function (player)
        XTD.Systems.MusicManager.PauseMusic()

        local playerData = XTD.Core.PlayerManager.GetPlayer(player)

        if playerData ~= nil then
            XTD.UI.MessageManager.Broadcast(
                XTD.Core.Color.Wrap(playerData.color, playerData.name) ..
                XTD.Core.Color.Ivory(" has paused the background music.")
            )
        end
    end, false, "Pause the background music")

    XTD.Systems.CommandManager.Register("/music resume", function (player)
        XTD.Systems.MusicManager.ResumeMusic()

        local playerData = XTD.Core.PlayerManager.GetPlayer(player)

        if playerData ~= nil then
            XTD.UI.MessageManager.Broadcast(
                XTD.Core.Color.Wrap(playerData.color, playerData.name) ..
                XTD.Core.Color.Ivory(" has resumed the background music.")
            )
        end
    end, false, "Resume paused background music")

    XTD.Systems.CommandManager.Register("/music stop", function (player)
        XTD.Systems.MusicManager.StopAllMusic()

        local playerData = XTD.Core.PlayerManager.GetPlayer(player)

        if playerData ~= nil then
            XTD.UI.MessageManager.Broadcast(
                XTD.Core.Color.Wrap(playerData.color, playerData.name) ..
                XTD.Core.Color.Ivory(" has stopped all background music.")
            )
        end
    end, false, "Stop the background music")

    XTD.Systems.CommandManager.Register("/music start", function (player)
        XTD.Systems.MusicManager.StartMusic()

        local playerData = XTD.Core.PlayerManager.GetPlayer(player)

        if playerData ~= nil then
            XTD.UI.MessageManager.Broadcast(
                XTD.Core.Color.Wrap(playerData.color, playerData.name) ..
                XTD.Core.Color.Ivory(" has started the background music.")
            )
        end
    end, false, "Start background music")

    XTD.Systems.CommandManager.Register("/map", function (player)
        local mapData = XTD.Data.Map
        local authors = ""
        local initial = true
        local separator = ""

        for _, author in ipairs(mapData.Authors) do
            if not initial then
                separator = ", "
            end

            initial = false

            authors = authors .. separator .. author
        end

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.CoolBlue("XTD: Reforged Map Information:\n") ..
            XTD.Core.Color.Gold("Authors: ") .. XTD.Core.Color.CoolGreen(authors) .. "\n" ..
            XTD.Core.Color.Gold("Version: ") .. XTD.Core.Color.CoolGreen(mapData.Version) .. "\n" ..
            XTD.Core.Color.Gold("Description:\n") ..
            XTD.Core.Color.CoolGreen(mapData.Description) .. "\n"
        )
    end, false, "View the map information")

    XTD.Systems.CommandManager.Register("/version", function (player)
        local mapData = XTD.Data.Map

        if mapData.Version ~= nil then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.CoolBlue("XTD: ") ..
                XTD.Core.Color.BrightGreen("Reforged ") ..
                XTD.Core.Color.Gold("v") ..
                XTD.Core.Color.Ivory(mapData.Version)
            )
        end
    end, false, "View the map version")

    XTD.Systems.CommandManager.Register("/help", function (player)
        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.CoolBlue("========== XTD: Reforged Help ==========\n") ..
            XTD.Core.Color.Ivory("• Press ") ..
            XTD.Core.Color.Gold("F9") ..
            XTD.Core.Color.Ivory(" to open the Quest Log.\n") ..
            XTD.Core.Color.Ivory("• Type ") ..
            XTD.Core.Color.Gold("/commands") ..
            XTD.Core.Color.Ivory(" to view all commands.\n") ..
            XTD.Core.Color.Ivory("• Type ") ..
            XTD.Core.Color.Gold("/map") ..
            XTD.Core.Color.Ivory(" to view map information.\n") ..
            XTD.Core.Color.Ivory("• Build and upgrade towers to survive ") ..
            XTD.Core.Color.BrightGreen("40 waves") ..
            XTD.Core.Color.Ivory(".\n") ..
            XTD.Core.Color.Ivory("• Watch for ") ..
            XTD.Core.Color.CoolBlue("Air") ..
            XTD.Core.Color.Ivory(", ") ..
            XTD.Core.Color.Yellow("Bonus") ..
            XTD.Core.Color.Ivory(", and ") ..
            XTD.Core.Color.Red("Boss") ..
            XTD.Core.Color.Ivory(" waves.\n\n") ..
            XTD.Core.Color.CoolGreen("For detailed information, press F9 to open the Quest Log.")
        )
    end, false, "Get help")

    XTD.Systems.CommandManager.Register("/gold", function (player, args)
        local amount = tonumber(args[2])

        if amount == nil or amount <= 0 then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("Usage: /gold <amount>")
            )
            return
        end

        XTD.Core.PlayerManager.AddResource(player, "gold", amount)

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.BrightGreen("You gave yourself ") ..
            XTD.Core.Color.Gold(tostring(amount)) ..
            XTD.Core.Color.BrightGreen(" gold.")
        )
    end, true, "Give yourself gold")

    XTD.Systems.CommandManager.Register("/leaks", function (player, args)
        local amount = tonumber(args[2])

        if amount == nil or amount <= 0 then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("Usage: /leaks <amount>")
            )
            return
        end

        XTD.Systems.EndzoneManager.leaksLeft = amount

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.BrightGreen("You set leaks left to ") ..
            XTD.Core.Color.Gold(tostring(amount)) ..
            XTD.Core.Color.BrightGreen(".")
        )
    end, true, "Set the leaks left")

    XTD.Systems.CommandManager.Register("/difficulty", function (player, args)
        local difficulty = args[2]

        if difficulty == nil or
            difficulty ~= "Easy" and
            difficulty ~= "Medium" and
            difficulty ~= "Hard" and
            difficulty ~= "Insane" and
            difficulty ~= "Nightmare" then
                XTD.UI.MessageManager.Player(
                    player,
                    XTD.Core.Color.Red("Invalid difficulty given.")
                )
                return
            end

        XTD.Core.GameManager.SetDifficulty(difficulty)

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.BrightGreen("You set game difficulty to ") ..
            XTD.Core.GameManager.GetDifficultyInColor(difficulty) ..
            XTD.Core.Color.BrightGreen(".")
        )
    end, true, "Change the game difficulty")

    XTD.Systems.CommandManager.Register("/skipwave", function (player)
        local creeps = XTD.Systems.SpawnManager.creeps

        for _, creepData in pairs(creeps) do
            XTD.Systems.UnitManager.BlowUpUnit(creepData.unit)
        end

        XTD.Systems.SpawnManager.creeps = {}

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.BrightGreen("All creeps have been killed and next wave should spawn.")
        )
    end, true, "Instantly kill all remaining creeps and complete the wave")

    XTD.Systems.CommandManager.Register("/disablespawndelay", function (player)
        XTD.Systems.WaveManager.betweenWaveEnabled = false

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.BrightGreen("Between wave delay disabled.")
        )
    end, true, "Disable the between wave delay period")

    XTD.Systems.CommandManager.Register("/enablespawndelay", function (player)
        XTD.Systems.WaveManager.betweenWaveEnabled = true

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.BrightGreen("Between wave delay enabled.")
        )
    end, true, "Enable the between wave delay period")

    XTD.Systems.CommandManager.Register("/pausewaves", function (player)
        XTD.Systems.WaveManager.wavesPaused = true
        XTD.Core.Timer.DestroyAllTimers("waves")
        XTD.Core.Timer.DestroyAllTimers("initial")

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.BrightGreen("Waves paused")
        )
    end, true, "Pause all waves")

    XTD.Systems.CommandManager.Register("/resumewaves", function (player)
        XTD.Systems.WaveManager.wavesPaused = false
        XTD.Systems.WaveManager.BeginNextWave()

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.BrightGreen("Waves resumed")
        )
    end, true, "Resume all waves")

    XTD.Systems.CommandManager.Register("/startwave", function (player, args)
        local wave = tonumber(args[2])

        if wave == nil or wave <= 0 or wave > XTD.Constants.MAX_WAVES then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Color.Red("Invalid wave given.")
            )
            return
        end

        XTD.Systems.WaveManager.wavesPaused = true
        XTD.Systems.WaveManager.currentWave = wave - 1
        XTD.Systems.WaveManager.wavesPaused = false
        XTD.Systems.WaveManager.BeginNextWave()

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.BrightGreen("Wave ") ..
            XTD.Core.Color.Gold(tostring(wave)) ..
            XTD.Core.Color.BrightGreen(" starting now.")
        )
    end, true, "Start a specific wave of creeps")

    XTD.Systems.CommandManager.Register("/commands", function(player)
        local lines = XTD.Systems.CommandManager.BuildCommandList(XTD.Core.devModeEnabled)
        local messages = {}
        local perPage = 7
        local page = 1

        for i = 1, #lines, perPage do
            local totalPages = math.ceil(#lines / perPage)
            local message =
                XTD.Core.Color.Gold("Available Commands (" .. tostring(page) .. "/" .. tostring(totalPages) .. "):\n")

            for j = i, math.min(i + perPage - 1, #lines) do
                message = message .. lines[j] .. "\n"
            end

            table.insert(messages, message)
            page = page + 1
        end

        if #messages == 0 then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("No commands available.")
            )
            return
        end

        XTD.UI.MessageManager.Player(player, messages[1])

        if #messages > 1 then
            local remaining = {}

            for i = 2, #messages do
                table.insert(remaining, messages[i])
            end

            XTD.UI.MessageManager.BroadcastMultipleMessagesOverTime(remaining, 2.00)
        end
    end, false, "Show available commands")
end

--[[
    Register all the chat triggers.
]]
function XTD.Systems.CommandManager.RegisterChatTriggers()
    local trigger = CreateTrigger()

    for i = 0, 7 do
        TriggerRegisterPlayerChatEvent(trigger, Player(i), "/", false)
    end

    TriggerAddAction(trigger, function()
        local player = GetTriggerPlayer()
        local message = string.lower(GetEventPlayerChatString())

        message = string.gsub(message, "^%s*(.-)%s*$", "%1")

        XTD.Systems.CommandManager.Handle(player, message)
    end)
end

--[[
    Handle the command.
]]
function XTD.Systems.CommandManager.Handle(player, message)
    local commandData = XTD.Systems.CommandManager.commands[message]

    if commandData ~= nil then
        if commandData.developerOnly and not XTD.Core.devModeEnabled then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Red("That command is only available in developer mode.")
            )
            return
        end

        commandData.callback(player, XTD.Systems.CommandManager.Split(message))
        return
    end

    local parts = XTD.Systems.CommandManager.Split(message)

    if #parts == 0 then
        return
    end

    local command = parts[1]

    commandData = XTD.Systems.CommandManager.commands[command]

    if commandData == nil then
        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.Red("Unknown command: ") ..
            XTD.Core.Color.Yellow(message)
        )
        return
    end

    if commandData.developerOnly and not XTD.Core.devModeEnabled then
        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.Red("That command is only available in developer mode.")
        )
        return
    end

    commandData.callback(player, parts)
end

--[[
    Helper to split a string by spaces.
]]
function XTD.Systems.CommandManager.Split(message)
    local parts = {}

    for part in string.gmatch(message, "%S+") do
        table.insert(parts, part)
    end

    return parts
end
