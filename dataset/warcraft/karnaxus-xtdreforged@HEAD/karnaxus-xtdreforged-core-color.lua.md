---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "core/color.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Color                                               |
    | =================================================== |
    | NodeJS Chalk-like API for WC3.                      |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Core.Color = XTD.Core.Color or {}

function XTD.Core.Color.Wrap(hex, text)
    return "|cff" .. hex .. text .. "|r"
end

function XTD.Core.Color.Green(text)
    return XTD.Core.Color.Wrap("80FF80", text)
end

function XTD.Core.Color.Red(text)
    return XTD.Core.Color.Wrap("FF4040", text)
end

function XTD.Core.Color.Blue(text)
    return XTD.Core.Color.Wrap("80C0FF", text)
end

function XTD.Core.Color.Yellow(text)
    return XTD.Core.Color.Wrap("FFCC00", text)
end

function XTD.Core.Color.Purple(text)
    return XTD.Core.Color.Wrap("C080FF", text)
end

function XTD.Core.Color.White(text)
    return XTD.Core.Color.Wrap("FFFFFF", text)
end

function XTD.Core.Color.Teal(text)
    return XTD.Core.Color.Wrap("00C0C0", text)
end

function XTD.Core.Color.Pink(text)
    return XTD.Core.Color.Wrap("FFC0FF", text)
end

function XTD.Core.Color.Orange(text)
    return XTD.Core.Color.Wrap("FF8E09", text)
end

function XTD.Core.Color.Silver(text)
    return XTD.Core.Color.Wrap("C0C0C0", text)
end

function XTD.Core.Color.BrightGreen(text)
    return XTD.Core.Color.Wrap("BFF600", text)
end

function XTD.Core.Color.BrightBlue(text)
    return XTD.Core.Color.Wrap("1985FF", text)
end

function XTD.Core.Color.CoolBlue(text)
    return XTD.Core.Color.Wrap("0296F2", text)
end

function XTD.Core.Color.CoolGreen(text)
    return XTD.Core.Color.Wrap("BEF500", text)
end

function XTD.Core.Color.AncientGold(text)
    return XTD.Core.Color.Wrap("FFD45A", text)
end

function XTD.Core.Color.Ivory(text)
    return XTD.Core.Color.Wrap("E8E3D2", text)
end

function XTD.Core.Color.SoftGray(text)
    return XTD.Core.Color.Wrap("BFBFBF", text)
end

function XTD.Core.Color.Gold(text)
    return XTD.Core.Color.Wrap("FFD700", text)
end

function XTD.Core.Color.SkyBlue(text)
    return XTD.Core.Color.Wrap("66CCFF", text)
end

function XTD.Core.Color.Emerald(text)
    return XTD.Core.Color.Wrap("00FF99", text)
end

function XTD.Core.Color.BloodRed(text)
    return XTD.Core.Color.Wrap("CC2222", text)
end

function XTD.Core.Color.ArcanePurple(text)
    return XTD.Core.Color.Wrap("AA66FF", text)
end

function XTD.Core.Color.StoneBlue(text)
    return XTD.Core.Color.Wrap("4AA3FF", text)
end

function XTD.Core.Color.CreepGreen(text)
    return XTD.Core.Color.Wrap("77CC33", text)
end

function XTD.Core.Color.No(text)
    return XTD.Core.Color.Wrap("BEF500", text)
end

function XTD.Core.Color.Yes(text)
    return XTD.Core.Color.Wrap("FF4040", text)
end
