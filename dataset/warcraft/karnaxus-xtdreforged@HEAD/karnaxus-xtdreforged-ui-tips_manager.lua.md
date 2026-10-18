---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "ui/tips_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Tip Manager                                         |
    | =================================================== |
    | Manages displaying tips to players.                 |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.UI.TipsManager = XTD.UI.TipsManager or {}

XTD.UI.TipsManager.enabled = {}
XTD.UI.TipsManager.currentTip = 1
XTD.UI.TipsManager.interval = 45.00

XTD.UI.TipsManager.tips = {
    "Type /disable tips to turn off these tips. Type /enable tips to re-enable these tips.",
    "This map is currently in beta stage, so note that there will be some bugs and balance issues. Please type see the Codex (F9) to report bugs and balance issues. Thanks for your understanding!",
    "To allow other players to build in your zone: type /allowbuild <player|all>. To deny an allowed player or players: type /denybuild <player|all>.",
    "Rumor has it the Kobolds are still looking for their candle.",
    "Upgrade your towers instead of constantly building new ones. Higher tier towers are often more cost effective.",
    "Keep an eye on upcoming waves by viewing the creep preview boxes around the map.",
    "Air Waves require towers that can attack flying units. Don't get caught unprepared!",
    "Boss Waves feature powerful enemies with massive health pools. Plan your defenses accordingly.",
    "Bonus Waves are a chance to earn extra gold. Make the most of them!",
    "Balance single-target and area damage towers. You'll need both to survive later waves.",
    "Penguins are scientifically proven to waddle 17% more effectively in bonus waves.",
    "Different tower types excel in different situations. Experiment to find powerful combinations.",
    "Selling a Tier 1 tower refunds 100% of its cost. Higher tier towers refund less gold.",
    "Remember to spend your gold! Unused gold can't defeat creeps.",
    "If you're floating lots of gold, consider upgrading existing towers before building more.",
    "Building near the end of the path can help clean up creeps that survive your main defenses.",
    "Watch for Fast Waves! Speed can be just as dangerous as high health.",
    "That one peasant really is working. He's just taking his lunch break... since Wave 1.",
    "Some waves are tougher than others. Saving gold before a difficult wave can pay off.",
    "Working together is the key to victory. Coordinate with your teammates.",
    "If a teammate is struggling, consider helping defend their lane if game rules allow it.",
    "Hover over creep preview labels to learn what each upcoming wave contains.",
    "Leaking too many creeps brings the team closer to defeat. Every kill matters!",
    "Remember: Creeps don't pay taxes, but they do pay gold when defeated.",
    "Use your builder efficiently—planning ahead saves precious time between waves.",
    "Not every tower belongs everywhere. Positioning can be just as important as upgrades.",
    "No creeps were harmed in the making of this tip... yet.",
    "The strongest defense isn't always the most expensive one. Synergy beats raw cost.",
    "Need to experiment? Try different tower builds each game to discover new strategies.",
    "The deadliest wave is often the one you didn't prepare for."
}

--[[
    Initialize the Tips Manager.
]]
function XTD.UI.TipsManager.Init()
    XTD.Core.Debug("TipsManager", "Initializing the Tips Manager...", "Init")

    XTD.Core.PlayerManager.ForEach(function (playerData)
        XTD.UI.TipsManager.enabled[playerData.id] = true
    end)

    XTD.UI.TipsManager.Start()

    XTD.Core.Debug("TipsManager", "Tips Manager initialized.", "Init")
end

--[[
    Start the tips.
]]
function XTD.UI.TipsManager.Start()
    local timer = CreateTimer()

    TimerStart(timer, XTD.UI.TipsManager.interval, true, function ()
        XTD.UI.TipsManager.ShowNextTip()
    end)
end

--[[
    Display the next tip.
]]
function XTD.UI.TipsManager.ShowNextTip()
    local tip = XTD.UI.TipsManager.tips[XTD.UI.TipsManager.currentTip]

    if tip == nil then
        XTD.UI.TipsManager.currentTip = 1
        tip = XTD.UI.TipsManager.tips[1]
    end

    XTD.Core.PlayerManager.ForEach(function (playerData)
        local player = playerData.player
        local playerId = GetPlayerId(player)

        if XTD.UI.TipsManager.enabled[playerId] ~= false then
            XTD.UI.MessageManager.Player(
                player,
                XTD.Core.Color.Yellow("Tip: ") ..
                XTD.Core.Color.Teal(tip)
            )
        end
    end)

    XTD.UI.TipsManager.currentTip = XTD.UI.TipsManager.currentTip + 1

    if XTD.UI.TipsManager.currentTip > #XTD.UI.TipsManager.tips then
        XTD.UI.TipsManager.currentTip = 1
    end
end