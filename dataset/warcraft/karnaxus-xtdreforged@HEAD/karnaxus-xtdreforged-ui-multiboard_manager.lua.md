---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "ui/multiboard_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Multiboard Manager                                  |
    | =================================================== |
    | Manages the multiboard.                             |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.UI.MultiboardManager = XTD.UI.MultiboardManager or {}

XTD.UI.MultiboardManager.board = nil
XTD.UI.MultiboardManager.updateTimer = nil

XTD.UI.MultiboardManager.icons = {
    player = "war3mapImported\\player.blp",
    kills = "war3mapImported\\kills.blp",
    gold = "war3mapImported\\gold.blp",
    waves = "war3mapImported\\waves.blp",
    creepsAlive = "war3mapImported\\creepsalive.blp",
    difficulty = "war3mapImported\\difficulty.blp",
    goldInterval = "war3mapImported\\goldinterval.blp",
    airWave = "war3mapImported\\airwave.blp",
    bonusWave = "war3mapImported\\bonuswave.blp",
    bossWave = "war3mapImported\\bosswave.blp",
    leaksLeft = "war3mapImported\\leaksleft.blp"
}

XTD.UI.MultiboardManager.COLS = 8

XTD.UI.MultiboardManager.WIDTHS = {
    ICON = 0.018,
    PLAYER = 0.120,

    KILLS_ICON = 0.018,
    KILLS = 0.045,

    GOLD_ICON = 0.018,
    GOLD = 0.055,

    INFO_TEXT = 0.190,
    SPACER = 0.001
}

XTD.UI.MultiboardManager.infoRows = {
    { key = "waves", icon = "waves", label = "Waves", color = XTD.Core.Color.SkyBlue },
    { key = "leaksLeft", icon = "leaksLeft", label = "Leaks Left", color = XTD.Core.Color.StoneBlue },
    { key = "creepsAlive", icon = "creepsAlive", label = "Creeps Alive", color = XTD.Core.Color.CreepGreen },
    { key = "airWave", icon = "airWave", label = "Air Wave", color = XTD.Core.Color.SkyBlue },
    { key = "bonusWave", icon = "bonusWave", label = "Bonus Wave", color = XTD.Core.Color.Emerald },
    { key = "bossWave", icon = "bossWave", label = "Boss Wave", color = XTD.Core.Color.BloodRed },
    { key = "goldInterval", icon = "goldInterval", label = "Gold Interval", color = XTD.Core.Color.Gold },
    { key = "difficulty", icon = "difficulty", label = "Difficulty", color = XTD.Core.Color.ArcanePurple }
}

--[[
    Initialize the Multiboard Manager.
]]
function XTD.UI.MultiboardManager.Init()
    local self = XTD.UI.MultiboardManager

    XTD.Core.Debug("MultiboardManager", "Initializing the Multiboard Manager...", "Init")

    self.Rebuild()

    if self.updateTimer ~= nil then
        PauseTimer(self.updateTimer)
        DestroyTimer(self.updateTimer)
        self.updateTimer = nil
    end

    self.updateTimer = CreateTimer()

    TimerStart(self.updateTimer, 1.00, true, function()
        self.Update()
    end)

    XTD.Core.Debug("MultiboardManager", "Multiboard Manager initialized.", "Init")
end

--[[
    Set an item on the multiboard.
]]
function XTD.UI.MultiboardManager.SetItem(row, col, text, icon, width, showText, showIcon)
    local self = XTD.UI.MultiboardManager

    if self.board == nil then
        return
    end

    local item = MultiboardGetItem(self.board, row, col)

    if item == nil then
        return
    end

    MultiboardSetItemValue(item, text or "")
    MultiboardSetItemIcon(item, icon or "")
    MultiboardSetItemWidth(item, width or 0.03)
    MultiboardSetItemStyle(item, showText ~= false, showIcon == true)

    MultiboardReleaseItem(item)
end

--[[
    Clear a row on the multiboard.
]]
function XTD.UI.MultiboardManager.ClearRow(row)
    local self = XTD.UI.MultiboardManager

    for col = 0, self.COLS - 1 do
        self.SetItem(row, col, "", nil, self.WIDTHS.SPACER, false, false)
    end
end

--[[
    Get the total active player count.
]]
function XTD.UI.MultiboardManager.GetPlayerCount()
    local count = 0

    XTD.Core.PlayerManager.ForEach(function()
        count = count + 1
    end)

    return count
end

--[[
    Get the info start row number.
]]
function XTD.UI.MultiboardManager.GetInfoStartRow()
    return 1 + XTD.UI.MultiboardManager.GetPlayerCount() + 1
end

--[[
    Get the total rows.
]]
function XTD.UI.MultiboardManager.GetTotalRows()
    return XTD.UI.MultiboardManager.GetInfoStartRow() + #XTD.UI.MultiboardManager.infoRows
end

--[[
    Set a safe cell.
]]
function XTD.UI.MultiboardManager.SafeCall(callback, fallback)
    local success, result = pcall(callback)

    if success then
        return result
    end

    return fallback
end

--[[
    Colored text helper.
]]
function XTD.UI.MultiboardManager.ColorText(colorFunc, text)
    if colorFunc ~= nil then
        return colorFunc(text)
    end

    return text
end

--[[
    Build the info text.
]]
function XTD.UI.MultiboardManager.BuildInfoText(data, value)
    local label = data.label .. ": "
    local coloredLabel = XTD.UI.MultiboardManager.ColorText(data.color, label)

    return coloredLabel .. tostring(value)
end

function XTD.UI.MultiboardManager.Rebuild()
    local self = XTD.UI.MultiboardManager

    if self.board ~= nil then
        DestroyMultiboard(self.board)
        self.board = nil
    end

    self.board = CreateMultiboard()

    MultiboardSetTitleText(
        self.board,
        XTD.Core.Color.BrightBlue("X Tower Defense: ") ..
        XTD.Core.Color.Gold("Reforged")
    )

    MultiboardSetRowCount(self.board, self.GetTotalRows())
    MultiboardSetColumnCount(self.board, self.COLS)

    self.SetItem(0, 0, "", self.icons.player, self.WIDTHS.ICON, false, true)
    self.SetItem(0, 1, XTD.Core.Color.SkyBlue("Player"), nil, self.WIDTHS.PLAYER, true, false)

    self.SetItem(0, 2, "", self.icons.kills, self.WIDTHS.KILLS_ICON, false, true)
    self.SetItem(0, 3, XTD.Core.Color.BloodRed("Kills"), nil, self.WIDTHS.KILLS, true, false)

    self.SetItem(0, 4, "", self.icons.gold, self.WIDTHS.GOLD_ICON, false, true)
    self.SetItem(0, 5, XTD.Core.Color.Gold("Gold"), nil, self.WIDTHS.GOLD, true, false)

    self.SetItem(0, 6, "", nil, self.WIDTHS.SPACER, false, false)
    self.SetItem(0, 7, "", nil, self.WIDTHS.SPACER, false, false)

    local row = 1

    XTD.Core.PlayerManager.ForEach(function(playerData)
        self.SetItem(row, 0, "", nil, self.WIDTHS.ICON, false, false)
        self.SetItem(row, 1, XTD.Core.Color.Wrap(playerData.color, playerData.name), nil, self.WIDTHS.PLAYER, true, false)

        self.SetItem(row, 2, "", nil, self.WIDTHS.KILLS_ICON, false, false)
        self.SetItem(row, 3, "0", nil, self.WIDTHS.KILLS, true, false)

        self.SetItem(row, 4, "", nil, self.WIDTHS.GOLD_ICON, false, false)
        self.SetItem(row, 5, "0", nil, self.WIDTHS.GOLD, true, false)

        self.SetItem(row, 6, "", nil, self.WIDTHS.SPACER, false, false)
        self.SetItem(row, 7, "", nil, self.WIDTHS.SPACER, false, false)

        row = row + 1
    end)

    self.ClearRow(row)

    local infoStart = self.GetInfoStartRow()

    for index, data in ipairs(self.infoRows) do
        local infoRow = infoStart + index - 1

        self.SetItem(infoRow, 0, "", self.icons[data.icon], self.WIDTHS.ICON, false, true)
        self.SetItem(infoRow, 1, self.BuildInfoText(data, ""), nil, self.WIDTHS.INFO_TEXT, true, false)

        self.SetItem(infoRow, 2, "", nil, self.WIDTHS.SPACER, false, false)
        self.SetItem(infoRow, 3, "", nil, self.WIDTHS.SPACER, false, false)
        self.SetItem(infoRow, 4, "", nil, self.WIDTHS.SPACER, false, false)
        self.SetItem(infoRow, 5, "", nil, self.WIDTHS.SPACER, false, false)
        self.SetItem(infoRow, 6, "", nil, self.WIDTHS.SPACER, false, false)
        self.SetItem(infoRow, 7, "", nil, self.WIDTHS.SPACER, false, false)
    end

    MultiboardDisplay(self.board, true)
    MultiboardMinimize(self.board, false)

    self.Update()
end

--[[
    Update the multiboard.
]]
function XTD.UI.MultiboardManager.Update()
    local self = XTD.UI.MultiboardManager

    if self.board == nil then
        return
    end

    local row = 1

    XTD.Core.PlayerManager.ForEach(function(playerData)
        local player = playerData.player

        local kills = playerData.kills or 0

        local gold = self.SafeCall(function()
            return XTD.Core.PlayerManager.GetResource(player, "gold")
        end, 0) or 0

        self.SetItem(row, 3, XTD.Core.Color.CoolGreen(tostring(kills)), nil, self.WIDTHS.KILLS, true, false)
        self.SetItem(row, 5, XTD.Core.Color.CoolGreen(tostring(gold)), nil, self.WIDTHS.GOLD, true, false)

        row = row + 1
    end)

    local currentWave = 0
    local totalWaves = XTD.Constants.MAX_WAVES or 0
    local creepsAlive = 0
    local leaksLeft = 0
    local difficulty = "Unknown"
    local goldInterval = XTD.Systems.IntervalGoldManager.GetRemaining()

    if XTD.Systems.WaveManager ~= nil then
        currentWave = XTD.Systems.WaveManager.currentWave or 0
    end

    if XTD.Systems.CreepManager ~= nil then
        creepsAlive = XTD.Systems.CreepManager.creepsAlive or 0
    end

    if XTD.Systems.EndzoneManager ~= nil then
        leaksLeft = XTD.Systems.EndzoneManager.leaksLeft or 0
    end

    if XTD.Core.GameManager ~= nil and XTD.Core.GameManager.difficulty ~= nil then
        difficulty = XTD.Core.GameManager.difficulty
    end

    local noText = XTD.Core.Color.No("No")
    local yesText = XTD.Core.Color.Yes("Yes")

    local airWave = noText
    local bonusWave = noText
    local bossWave = noText

    if XTD.Systems.WaveManager ~= nil then
        if XTD.Systems.WaveManager.IsAirWave ~= nil then
            if self.SafeCall(function()
                return XTD.Systems.WaveManager.IsAirWave()
            end, false) then
                airWave = yesText
            end
        end

        if XTD.Systems.WaveManager.IsBonusWave ~= nil then
            if self.SafeCall(function()
                return XTD.Systems.WaveManager.IsBonusWave()
            end, false) then
                bonusWave = yesText
            end
        end

        if XTD.Systems.WaveManager.IsBossWave ~= nil then
            if self.SafeCall(function()
                return XTD.Systems.WaveManager.IsBossWave()
            end, false) then
                bossWave = yesText
            end
        end
    end

    if XTD.Core.GameManager ~= nil and XTD.Core.GameManager.GetDifficultyInColor ~= nil then
        difficulty = self.SafeCall(function()
            return XTD.Core.GameManager.GetDifficultyInColor(difficulty)
        end, difficulty)
    end

    local values = {
        waves = XTD.Core.Color.CoolGreen(tostring(currentWave)) ..
            XTD.Core.Color.AncientGold(" / ") ..
            XTD.Core.Color.CoolGreen(tostring(totalWaves)),

        leaksLeft = XTD.Core.Color.CoolGreen(tostring(leaksLeft)),
        creepsAlive = XTD.Core.Color.CoolGreen(tostring(creepsAlive)),
        airWave = airWave,
        bonusWave = bonusWave,
        bossWave = bossWave,
        goldInterval = XTD.Core.Color.CoolGreen(tostring(goldInterval)),
        difficulty = tostring(difficulty)
    }

    local infoStart = self.GetInfoStartRow()

    for index, data in ipairs(self.infoRows) do
        self.SetItem(
            infoStart + index - 1,
            1,
            self.BuildInfoText(data, values[data.key] or "N/A"),
            nil,
            self.WIDTHS.INFO_TEXT,
            true,
            false
        )
    end
end