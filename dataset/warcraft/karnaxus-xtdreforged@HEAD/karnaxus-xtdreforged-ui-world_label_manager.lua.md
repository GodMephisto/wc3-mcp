---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "ui/world_label_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | World Label Manager                                 |
    | =================================================== |
    | Manages the world labels.                           |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.UI.WorldLabelManager = XTD.UI.WorldLabelManager or {}

--[[
    Initialize the World Label Manager.
]]
function XTD.UI.WorldLabelManager.Init()
    XTD.Core.Debug("WorldLabelManager", "Initializing the World Label Manager...")

    XTD.Core.Debug("WorldLabelManager", "World Label Manager initialized.")
end

--[[
    Create a new world text label.
]]
function XTD.UI.WorldLabelManager.CreateWorldLabel(text, x, y, z, size)
    local tag = CreateTextTag()

    SetTextTagText(tag, text, size or 0.024)
    SetTextTagPos(tag, x, y,z or 100.00)
    SetTextTagPermanent(tag, true)
    SetTextTagVisibility(tag, true)

    return tag
end

--[[
    Hide a world label.
]]
function XTD.UI.WorldLabelManager.HideWorldLabel(tag)
    if tag == nil then
        XTD.Core.Debug("WorldLabelManager", "label is nil.", "HideWorldLabel")
        return
    end

    SetTextTagVisibility(tag, false)
end