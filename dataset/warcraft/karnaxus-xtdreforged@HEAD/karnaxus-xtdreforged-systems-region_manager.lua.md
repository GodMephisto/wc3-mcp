---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/region_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Region Manager                                      |
    | =================================================== |
    | Manages regions.                                    |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.RegionManager = XTD.Systems.RegionManager or {}

--[[
    Initialize the Region Manager.
]]
function XTD.Systems.RegionManager.Init()
    XTD.Core.Debug("RegionManager", "Initializing the Region Manager...")
    XTD.Core.Debug("RegionManager", "Region Manager initialized.")
end

--[[
    Resolve a region.
]]
function XTD.Systems.RegionManager.ResolveRegion(regionName)
    if regionName == nil then
        XTD.Core.Debug("RegionManager", "regionName is nil.", "ResolveRegion")
        return nil
    end

    return _G["gg_rct_" ..regionName]
end

--[[
    Get the center of a region rect.
]]
function XTD.Systems.RegionManager.GetRectCenter(rect)
    if rect == nil then
        XTD.Core.Debug("RegionManager", "rect is nil.", "GetRectCenter")
        return nil, nil
    end

    local x = GetRectCenterX(rect)
    local y = GetRectCenterY(rect)

    return x, y
end