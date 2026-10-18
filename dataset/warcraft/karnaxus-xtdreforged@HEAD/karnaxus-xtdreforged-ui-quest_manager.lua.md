---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "ui/quest_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Quest Manager                                       |
    | =================================================== |
    | Manages displaying map info data.                   |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.UI.QuestManager = XTD.UI.QuestManager or {}

--[[
    Initialize the Quest Manager.
]]
function XTD.UI.QuestManager.Init()
    XTD.Core.Debug("QuestManager", "Initializing the Quest Manager...")

    XTD.UI.QuestManager.CreateQuests()

    XTD.Core.Debug("QuestManager", "Quest Manager initialized.")
end

--[[
    Create all the quests.
]]
function XTD.UI.QuestManager.CreateQuests()
    XTD.UI.QuestManager.CreateMainQuest()
    XTD.UI.QuestManager.CreateVersionQuest()
    XTD.UI.QuestManager.CreateCommandsQuest()
    XTD.UI.QuestManager.CreateSpecialWavesQuest()
    XTD.UI.QuestManager.CreateTowersQuest()
    XTD.UI.QuestManager.CreateGameDifficultiesQuest()
    XTD.UI.QuestManager.CreateGitHubQuest()
    XTD.UI.QuestManager.CreateBugReportQuest()
    XTD.UI.QuestManager.CreateMusicQuest()
end

--[[
    Create the main quest.
]]
function XTD.UI.QuestManager.CreateMainQuest()
    local quest = CreateQuest()

    QuestSetTitle(
        quest,
        XTD.Core.Color.CoolBlue("XTD: ") ..
        XTD.Core.Color.CoolGreen("Reforged")
    )

    QuestSetDescription(
        quest,
        XTD.Core.Color.Gold("Welcome to ") .. XTD.Core.Color.CoolBlue("XTD: ") .. XTD.Core.Color.CoolGreen("Reforged") .. "\n\n" ..
        XTD.Core.Color.CoolGreen("Welcome to ") .. XTD.Core.Color.CoolBlue("XTD: ") .. XTD.Core.Color.CoolGreen("Reforged") ..
        XTD.Core.Color.Ivory(", a complete reimagining of the original X Tower Defense created by Karnaxus (originally hs_goldwarrior).\n\n") ..
        XTD.Core.Color.Ivory("Build an arsenal of powerful towers, including devastating Spell Towers, to defend against 40 increasingly challenging waves of enemies. Coordinate with your teammates, adapt your defenses, and survive relentless Air Waves, Bonus Waves, and powerful Boss encounters.\n\n") ..
        XTD.Core.Color.CoolBlue("Featuresn\n") ..
        XTD.Core.Color.Ivory("• 30+ unique towers, including Spell Towers\n") ..
        XTD.Core.Color.Ivory("• 40 handcrafted waves\n") ..
        XTD.Core.Color.Ivory("• Air, Bonus, and Boss waves\n") ..
        XTD.Core.Color.Ivory("• Multiple difficulty levels\n") ..
        XTD.Core.Color.Ivory("• Cooperative gameplay for up to 8 players\n") ..
        XTD.Core.Color.Ivory("• Modernized systems, visuals, and quality-of-life improvements (Now written in Lua, no more GUI triggers)\n\n") ..
        XTD.Core.Color.Ivory("For additional information, commands, and gameplay tips, see the other quest entries or type /help in-game.\n\n") ..
        XTD.Core.Color.Ivory("Thank you for playing, and enjoy the next evolution of X Tower Defense!\n\n") ..
        XTD.Core.Color.CoolBlue("Originally Created By\n") ..
        XTD.Core.Color.CoolGreen("• Karnaxus#11289 (originally hs_goldwarrior)\n\n") ..
        XTD.Core.Color.CoolBlue("XTD: Reforged\n") ..
        XTD.Core.Color.CoolGreen("• Karnaxus#11289\n") ..
        XTD.Core.Color.CoolGreen("• Darkdayze#1213")
    )

    QuestSetIconPath(quest, "war3mapImported\\xtd.blp")
    QuestSetRequired(quest, true)
    QuestSetDiscovered(quest, true)
end

--[[
    Creete the map version quest.
]]
function XTD.UI.QuestManager.CreateVersionQuest()
    local quest = CreateQuest()

    QuestSetTitle(
        quest,
        XTD.Core.Color.CoolBlue("Map Version Information")
    )

    QuestSetDescription(
        quest,
        XTD.Core.Color.Gold("Version ") ..
        XTD.Core.Color.CoolGreen(XTD.Data.Map.Version)
    )

    QuestSetIconPath(quest, "war3mapImported\\version.blp")
    QuestSetRequired(quest, true)
    QuestSetDiscovered(quest, true)
end

--[[
    Create the commands quest.
]]
function XTD.UI.QuestManager.CreateCommandsQuest()
    local quest = CreateQuest()

    QuestSetTitle(
        quest,
        XTD.Core.Color.CoolBlue("Available In-Game Commands")
    )

    local commands = {
        {
            command = "/commands",
            description = "Show available commands"
        },
        {
            command = "/enable tips",
            description = "Enable in-game tips"
        },
        {
            command = "/disable tips",
            description = "Disable in-game tips"
        },
        {
            command = "/air",
            description = "Show air waves"
        },
        {
            command = "/bonus",
            description = "Show bonus waves"
        },
        {
            command = "/boss",
            description = "Show boss waves"
        },
        {
            command = "/enable debug",
            description = "Enable debug mode"
        },
        {
            command = "/disable debug",
            description = "Disable debug mode"
        },
        {
            command = "/enable wavesummary",
            description = "Enable wave-end summary"
        },
        {
            command = "/disable wavesummary",
            description = "Disable wave-end summary"
        },
        {
            command = "/allowbuild <player|all>",
            description = "Allow players to build in your zone"
        },
        {
            command = "/denybuild <player|all>",
            description = "Deny players from building in your zone"
        },
        {
            command = "/givegold <player|all> <amount>",
            description = "Give gold to a player or all players"
        },
        {
            command = "/wave",
            description = "Show the current wave number"
        },
        {
            command = "/nextwave",
            description = "Show the next wave number"
        },
        {
            command = "/kickvote <player>",
            description = "Start a vote to kick a player"
        },
        {
            command = "/music next",
            description = "Play the next music track"
        },
        {
            command = "/music pause",
            description = "Pause the background music"
        },
        {
            command = "/music resume",
            description = "Resume the background music"
        },
        {
            command = "/music stop",
            description = "Stop the background music"
        },
        {
            command = "/music start",
            description = "Start background music"
        },
        {
            command = "/map",
            description = "View the map information"
        },
        {
            command = "/version",
            description = "Get the map version information"
        },
        {
            command = "/help",
            description = "Get help"
        }
    }

    local description = XTD.Core.Color.Ivory(
        "Below are commands you can run in-game to toggle features, get information, and customize your experience.\n\n"
    )

    for _, commandData in ipairs(commands) do
        description = description ..
            XTD.Core.Color.Gold(commandData.command) ..
            XTD.Core.Color.Ivory(" - ") ..
            XTD.Core.Color.CoolGreen(commandData.description) ..
            "\n"
    end

    QuestSetDescription(quest, description)

    QuestSetIconPath(quest, "war3mapImported\\commands.blp")
    QuestSetRequired(quest, true)
    QuestSetDiscovered(quest, true)
end

--[[
    Create the special waves quest.
]]
function XTD.UI.QuestManager.CreateSpecialWavesQuest()
    local quest = CreateQuest()

    QuestSetTitle(
        quest,
        XTD.Core.Color.CoolBlue("Special Waves (Air, Bonus and Boss)")
    )

    QuestSetDescription(
        quest,
        XTD.Core.Color.Gold("Air Waves: ") .. XTD.Core.Color.CoolGreen("5, 10, 15, 25, and 35\n") ..
        XTD.Core.Color.Gold("Bonus Waves: ") .. XTD.Core.Color.CoolGreen("31 and 36\n") ..
        XTD.Core.Color.Gold("Boss Waves: ") .. XTD.Core.Color.CoolGreen("20, 30, and 40")
    )

    QuestSetIconPath(quest, "war3mapImported\\specialwaves.blp")
    QuestSetRequired(quest, true)
    QuestSetDiscovered(quest, true)
end

--[[
    Create the towers quest.
]]
function XTD.UI.QuestManager.CreateTowersQuest()
    local quest = CreateQuest()

    QuestSetTitle(
        quest,
        XTD.Core.Color.CoolBlue("Towers")
    )

    QuestSetDescription(
        quest,
        XTD.Core.Color.Ivory("The key to victory is building an effective defense network.\n\n") ..
        XTD.Core.Color.CoolBlue("Each tower unique strengths and weaknesses:\n") ..
        XTD.Core.Color.Gold("• Damage towers excel at eliminating powerful creeps\n") ..
        XTD.Core.Color.Gold("• Area damage towers are effective against large groups (but have higher cooldowns)\n") ..
        XTD.Core.Color.Gold("• Utility towers can slow or weaken creeps\n") ..
        XTD.Core.Color.Gold("• Some towers can attack air units while others cannot\n\n") ..
        XTD.Core.Color.CoolBlue("Remember:\n") ..
        XTD.Core.Color.Gold("• Upgrade often\n") ..
        XTD.Core.Color.Gold("• Adapt your defenses as waves become stronger\n") ..
        XTD.Core.Color.Gold("• Prepare for air and boss waves\n") ..
        XTD.Core.Color.Gold("• Work together with your teammates to cover weak areas if needed\n") ..
        XTD.Core.Color.Gold("• A balanced defense is often strong than relying on a single tower type")
    )

    QuestSetIconPath(quest, "war3mapImported\\towers.blp")
    QuestSetRequired(quest, true)
    QuestSetDiscovered(quest, true)
end

--[[
    Create the game difficulties quest.
]]
function XTD.UI.QuestManager.CreateGameDifficultiesQuest()
    local quest = CreateQuest()

    QuestSetTitle(
        quest,
        XTD.Core.Color.CoolBlue("Game Difficulties")
    )

    QuestSetDescription(
        quest,
        XTD.Core.Color.CoolBlue("There are 5 different game difficulties to choose from:\n\n") ..
        XTD.Core.Color.Green("Easy:\n") ..
        XTD.Core.Color.Ivory("Creeps have reduced health, and are a bit slower. Extra initial time to build. Extra time between waves to build. Creeps DO NOT clone. Gold interval time is lowest and higher gold amount.\n\n") ..
        XTD.Core.Color.Yellow("Medium:\n") ..
        XTD.Core.Color.Ivory("Creeps have base health and movement speed. Slightly less initial time to build compared to Easy mode. Slightly less time between waves compared to Easy mode. Creeps DO NOT clone. Gold interval time is slightly higher than Easy mode and slightly less gold amount.\n\n") ..
        XTD.Core.Color.Orange("Hard:\n") ..
        XTD.Core.Color.Ivory("Creeps have extra health and movement speed. Slightly less initial time to build compared to Medium mode. No time between waves. Creeps do clone at the beginning of each players zone. Gold interval time is slightly higher than Medium Mode and slightly less gold amouht.\n\n") ..
        XTD.Core.Color.Red("Insane:\n") ..
        XTD.Core.Color.Ivory("Creeps have more health and speed as compared to Hard mode. Slightly less initial time to build compared to Hard mode. No time between waves. Creeps do clone at the beginning of each player zone. Gold interval time is slightly higher than Hard mode and slightly less gold amount.\n\n") ..
        XTD.Core.Color.SoftGray("Nightmare:\n") ..
        XTD.Core.Color.Ivory("The maximum and possibly impossible mode. Creeps have a good deal of increased health and movement speed. Very little time to build initially. No time between waves. Creeps do clone at the beginning of each player zone. Gold intervak timne is slightly higher than Insane mode and slightly less gold amount.\n\n") ..
        XTD.Core.Color.Ivory("Game difficulty is always voted on by all players at the beginning of the game. Good luck!")
    )

    QuestSetIconPath(quest, "war3mapImported\\difficulties.blp")
    QuestSetRequired(quest, true)
    QuestSetDiscovered(quest, true)
end

--[[
    Create the GitHub information quest.
]]
function XTD.UI.QuestManager.CreateGitHubQuest()
    local quest = CreateQuest()

    QuestSetTitle(
        quest,
        XTD.Core.Color.CoolBlue("GitHub (Source Code)")
    )

    QuestSetDescription(
        quest,
        XTD.Core.Color.CoolBlue("XTD: ") .. XTD.Core.Color.CoolGreen("Reforged ") ..
        XTD.Core.Color.Ivory("is written all in pure Lua code and zero GUI triggers. I do protect this map as I have put in a great deal of work creating it." ..
            "With that being said, I do keep the Lua code public for those that wish to learn.\n\n" ..
            "All Lua code for this map can be found on GitHub at:\n") ..
            XTD.Core.Color.CoolGreen("https://github.com/karnaxus/xtdreforged\n\n") ..
            XTD.Core.Color.Ivory("Be sure to read the license agreement for the Lua code.")
    )

    QuestSetIconPath(quest, "war3mapImported\\github.blp")
    QuestSetRequired(quest, true)
    QuestSetDiscovered(quest, true)
end

--[[
    Create the bugs report quest.
]]
function XTD.UI.QuestManager.CreateBugReportQuest()
    local quest = CreateQuest()

    QuestSetTitle(
        quest,
        XTD.Core.Color.CoolBlue("Bug Reports")
    )

    QuestSetDescription(
        quest,
        XTD.Core.Color.CoolBlue("Found a bug while playing this map?\n\n") ..
        XTD.Core.Color.Ivory("If you find a bug while playing this map, please report the bug at:\n") ..
        XTD.Core.Color.CoolGreen("https://github.com/karnaxus/xtdreforged/issues")
    )

    QuestSetIconPath(quest, "war3mapImported\\bugreport.blp")
    QuestSetRequired(quest, true)
    QuestSetDiscovered(quest, true)
end

--[[
    Create the music quest.
]]
function XTD.UI.QuestManager.CreateMusicQuest()
    local quest = CreateQuest()

    QuestSetTitle(
        quest,
        XTD.Core.Color.CoolBlue("Background Music")
    )

    local tracks = XTD.Data.Music

    local description = XTD.Core.Color.CoolBlue("XTD: ") .. XTD.Core.Color.CoolGreen("Reforged ") ..
        XTD.Core.Color.Ivory("uses custom background music in our map. Below are the current tracks we are using:\n\n")

    for _, trackData in ipairs(tracks) do
        description = description ..
            XTD.Core.Color.Gold(trackData.name) ..
            XTD.Core.Color.Ivory(" - ") ..
            XTD.Core.Color.CoolGreen(trackData.artist) ..
            "\n"
    end

    QuestSetDescription(quest, description)

    QuestSetIconPath(quest, "war3mapImported\\music.blp")
    QuestSetRequired(quest, true)
    QuestSetDiscovered(quest, true)
end