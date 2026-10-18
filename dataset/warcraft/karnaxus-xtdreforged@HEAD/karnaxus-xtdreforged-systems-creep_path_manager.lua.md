---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/creep_path_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Creep Path Manager                                  |
    | =================================================== |
    | Manages the creep walking orders.                   |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.CreepPathManager = XTD.Systems.CreepPathManager or {}

XTD.Systems.CreepPathManager.triggers = {}
XTD.Systems.CreepPathManager.regions = {}
XTD.Systems.CreepPathManager.checkTimer = nil

--[[
    Initializing the Creep Path Manager.
]]
function XTD.Systems.CreepPathManager.Init()
    XTD.Core.Debug("CreepPathManager", "Initializing the Creep Path Manager...")

    XTD.Systems.CreepPathManager.triggers = {}
    XTD.Systems.CreepPathManager.regions = {}
    XTD.Systems.CreepPathManager.checkTimer = nil
    XTD.Systems.CreepPathManager.RegisterRegionTriggers()

    XTD.Systems.CreepBoxManager.checkTimer = CreateTimer()
    TimerStart(XTD.Systems.CreepPathManager.checkTimer, 1.00, true, XTD.Systems.CreepPathManager.CheckCreeps)

    XTD.Core.Debug("CreepPathManager", "Creep Path Manager initialized.")
end

--[[
    Register all the region triggers.
]]
function XTD.Systems.CreepPathManager.RegisterRegionTriggers()
    local self = XTD.Systems.CreepPathManager
    
    for _, regions in pairs(XTD.Data.Regions.Lanes) do
        self.RegisterRegionTrigger(regions.Spawn)
        self.RegisterRegionTrigger(regions.LaneEnd)
        self.RegisterRegionTrigger(regions.Split[1])
        self.RegisterRegionTrigger(regions.Split[2])
        self.RegisterRegionTrigger(regions.Clone[1])
        self.RegisterRegionTrigger(regions.Clone[2])
        self.RegisterRegionTrigger(regions.End[1])
        self.RegisterRegionTrigger(regions.End[2])
        self.RegisterRegionTrigger(regions.Right[1][1])
        self.RegisterRegionTrigger(regions.Right[1][2])
        self.RegisterRegionTrigger(regions.Right[2][1])
        self.RegisterRegionTrigger(regions.Right[2][2])
        self.RegisterRegionTrigger(regions.Left[1][1])
        self.RegisterRegionTrigger(regions.Left[1][2])
        self.RegisterRegionTrigger(regions.Left[2][1])
        self.RegisterRegionTrigger(regions.Left[2][2])
    end

    self.RegisterRegionTrigger(XTD.Data.Regions.Lanes.NW.EndZone)
end

--[[
    Register a region trigger.
]]
function XTD.Systems.CreepPathManager.RegisterRegionTrigger(regionName)
    if regionName == nil then
        XTD.Core.Debug("CreepPathManager", "regionName is nil.", "RegisterRegionTrigger")
        return
    end

    local self = XTD.Systems.CreepPathManager

    self.triggers[regionName] = CreateTrigger()
    self.regions[regionName] = CreateRegion()

    local rect = XTD.Systems.RegionManager.ResolveRegion(regionName)

    if rect == nil then
        XTD.Core.Debug("CreepPathManager", "rect is nil.", "RegisterRegionTrigger")
        return
    end

    RegionAddRect(self.regions[regionName], rect)
    TriggerRegisterEnterRegion(self.triggers[regionName], self.regions[regionName], nil)

    TriggerAddAction(self.triggers[regionName], function ()
        local unit = GetTriggerUnit()

        if not XTD.Systems.CreepManager.IsCreep(unit) then
            return
        end

        self.OnUnitEnterRegion(unit, regionName)
    end)
end

--[[
    Handle when a unit enters a region.
]]
function XTD.Systems.CreepPathManager.OnUnitEnterRegion(unit, regionName)
    local self = XTD.Systems.CreepPathManager

    if unit == nil then
        XTD.Core.Debug("CreepPathManager", "unit is nil.", "OnUnitEnterRegion")
        return
    end

    if regionName == nil then
        XTD.Core.Debug("CreepPathManager", "regionName is nil.", "OnUnitEnterRegion")
        return
    end

    if regionName == "EndZone" then
        XTD.Systems.EndzoneManager.OnEnter(unit)
        return
    end

    if XTD.Core.Helpers.EndsWith(regionName, "Clone") and not self.CloneEnabled() then
        return
    end

    local creepData = XTD.Systems.SpawnManager.creeps[GetHandleId(unit)]

    if creepData == nil then
        XTD.Core.Debug("CreepPathManager", "creepData is nil.", "OnUnitEnterRegion")
        return
    end

    local lane = creepData.lane

    local nextRegion = self.GetNextRegion(regionName, lane, unit, creepData)

    if nextRegion == nil then
        XTD.Core.Debug("CreepPathManager", "nextRegion is nil.", "OnUnitEnterRegion")
        return
    end

    local rect = XTD.Systems.RegionManager.ResolveRegion(nextRegion)

    if rect == nil then
        XTD.Core.Debug("CreepPathManager", "rect is nil.", "OnUnitEnterRegion")
        return
    end

    creepData.nextRegion = nextRegion

    local x, y = XTD.Systems.RegionManager.GetRectCenter(rect)

    if x == nil or y == nil then
        XTD.Core.Debug("CreepPathManager", "x or y is nil.", "OnUnitEnterRegion")
        return
    end

    IssuePointOrder(unit, "move", x, y)
end

--[[
    Get the next region.
]]
function XTD.Systems.CreepPathManager.GetNextRegion(regionName, lane, unit, creepData)
    local self = XTD.Systems.CreepPathManager

    if regionName == nil then
        XTD.Core.Debug("CreepPathManager", "regionName is nil.", "GetNextRegion")
        return nil
    end

    if lane == nil then
        XTD.Core.Debug("CreepPathManager", "lane is nil.", "GetNextRegion")
        return nil
    end

    local regions = XTD.Data.Regions.Lanes[lane]

    if regions == nil then
        XTD.Core.Debug("CreepPathManager", "regions is nil.", "GetNextRegion")
        return nil
    end

    if creepData == nil then
        XTD.Core.Debug("CreepPathManager", "creepData is nil.", "GetNextRegion")
        return nil
    end

    if XTD.Core.Helpers.EndsWith(regionName, "Spawn") then
        return regions.Split[1]
    end

    if XTD.Core.Helpers.EndsWith(regionName, "Begin") then
        if self.CloneEnabled() then
            local cloneRegionName = self.GetCloneRegion(regionName)

            if cloneRegionName == nil then
                XTD.Core.Debug("CreepPathManager", "cloneRegionName is nil.", "GetNextRegion")
                return nil
            end

            self.CloneUnit(cloneRegionName, unit)

            local index = self.GetIndexFromRegionArr("Left", regionName, regions)

            if index == nil then
                XTD.Core.Debug("CreepPathManager", "index is nil.", "GetNextRegion")
                return nil
            end

            return regions.Left[index][1]
        else
            local direction = creepData.direction

            if direction == nil then
                XTD.Core.Debug("CreepPathManager", "direction is nil.", "GetNextRegion")
                return nil
            end

            local index = nil
            local dir = nil

            if direction == "left" then
                index = self.GetIndexFromRegionArr("Left", regionName, regions)
                dir = "Left"
            else
                index = self.GetIndexFromRegionArr("Right", regionName, regions)
                dir = "Right"
            end

            if index == nil then
                XTD.Core.Debug("CreepPathManager", "index is nil.", "GetNextRegion")
                return nil
            end

            return regions[dir][index][1]
        end
    end

    if XTD.Core.Helpers.EndsWith(regionName, "Clone") then
        if self.CloneEnabled() then
            local index = self.GetIndexFromRegionArr("Right", regionName, regions)

            if index == nil then
                XTD.Core.Debug("CreepPathManager", "index is nil.", "GetNextRegion")
                return nil
            end

            return regions.Right[index][1]
        end
    end

    if XTD.Core.Helpers.Contains(regionName, "Right") then
        local index = self.GetIndexFromRegionArr("Right", regionName, regions)

        if index == nil then
            XTD.Core.Debug("CreepPathManager", "index is nil.", "GetNextRegion")
            return nil
        end

        if XTD.Core.Helpers.EndsWith(regionName, "1") then
            return regions.Right[index][2]
        elseif XTD.Core.Helpers.EndsWith(regionName, "2") then
            return regions.End[index]
        end
    end

    if XTD.Core.Helpers.Contains(regionName, "Left") then
        local index = self.GetIndexFromRegionArr("Left", regionName, regions)

        if index == nil then
            XTD.Core.Debug("CreepPathManager", "index is nil.", "GetNextRegion")
            return nil
        end

        if XTD.Core.Helpers.EndsWith(regionName, "1") then
            return regions.Left[index][2]
        elseif XTD.Core.Helpers.EndsWith(regionName, "2") then
            return regions.End[index]
        end
    end

    if XTD.Core.Helpers.EndsWith(regionName, "End") then
        local index = self.GetIndexFromPlayerEnd(regionName, regions)

        if index == nil then
            XTD.Core.Debug("CreepPathManager", "index is nil.", "GetNextRegion")
            return nil
        end

        if index == 1 then
            return regions.Split[2]
        else
            return regions.LaneEnd
        end
    end

    if XTD.Core.Helpers.EndsWith(regionName, "Lane") then
        return regions.EndZone
    end

    return nil
end

--[[
    Get the index for a player end region.
]]
function XTD.Systems.CreepPathManager.GetIndexFromPlayerEnd(regionName, regions)
    if regionName == nil then
        XTD.Core.Debug("CreepPathManager", "regionName is nil.", "GetIndexFromPlayerEnd")
        return nil
    end

    if regions == nil then
        XTD.Core.Debug("CreepPathManager", "regions is nil.", "GetIndexFromPlayerEnd")
        return nil
    end

    local arr = regions.End

    for index, region in ipairs(arr) do
        if region == regionName then
            return index
        end
    end

    return nil
end

--[[
    Get the index from a region index.
]]
function XTD.Systems.CreepPathManager.GetIndexFromRegionArr(key, regionName, regions)
    if key == nil then
        XTD.Core.Debug("CreepPathManager", "key is nil.", "GetIndexFromRegionArr")
        return nil
    end

    if regionName == nil then
        XTD.Core.Debug("CreepPathManager", "regionName is nil.", "GetIndexFromRegionArr")
        return nil
    end

    if regions == nil then
        XTD.Core.Debug("CreepPathManager", "regions is nil.", "GetIndexFromRegionArr")
        return nil
    end

    local arr = regions[key]
    local prefix = XTD.Core.Helpers.BeforeString(regionName, "Begin")

    if prefix == nil then
        prefix = regionName:gsub("Right%d$", ""):gsub("Left%d$", ""):gsub("Clone$", "")
    end

    for index, path in ipairs(arr) do
        for _, reg in ipairs(path) do
            if XTD.Core.Helpers.Contains(reg, prefix) then
                return index
            end
        end
    end

    return nil
end

--[[
    Get the clone region.
]]
function XTD.Systems.CreepPathManager.GetCloneRegion(regionName)
    if regionName == nil then
        XTD.Core.Debug("CreepPathManager", "regionName is nil.", "GetCloneRegion")
        return nil
    end    

    local prefix = XTD.Core.Helpers.BeforeString(regionName, "Begin")

    if prefix == nil then
        XTD.Core.Debug("CreepPathManager", "prefix is nil.", "GetCloneRegion")
        return nil
    end

    local cloneRegionName = prefix .. "Clone"

    return cloneRegionName
end

--[[
    Clone a unit.
]]
function XTD.Systems.CreepPathManager.CloneUnit(cloneRegion, unit)
    if cloneRegion == nil then
        XTD.Core.Debug("CreepPathManager", "cloneRegion is nil.", "CloneUnit")
        return
    end

    if unit == nil then
        XTD.Core.Debug("CreepPathManager", "unit is nil.", "CloneUnit")
        return
    end

    if not XTD.Core.Helpers.EndsWith(cloneRegion, "Clone") then
        XTD.Core.Debug("CreepPathManager", "cloneRegion does not end with Clone.", "CloneUnit")
        return
    end

    local rect = XTD.Systems.RegionManager.ResolveRegion(cloneRegion)

    if rect == nil then
        XTD.Core.Debug("CreepPathManager", "clone rect is nil.", "CloneUnit")
        return
    end

    local x, y = XTD.Systems.RegionManager.GetRectCenter(rect)

    if x == nil or y == nil then
        XTD.Core.Debug("CreepPathManager", "x or y is nil.", "CloneUnit")
        return
    end

    local unitId = XTD.Systems.UnitManager.GetUnitTypeId(unit)

    if unitId == nil then
        XTD.Core.Debug("CreepPathManager", "unitId is nil.", "CloneUnit")
        return
    end

    local timer = CreateTimer()

    TimerStart(timer, 0.35, false, function ()
        local clonedUnit = XTD.Systems.UnitManager.SpawnUnitAtPoint(
            XTD.Constants.CREEPS,
            unitId,
            x,
            y,
            270.00,
            true
        )

        XTD.Systems.UnitManager.DisableCollision(clonedUnit)
        XTD.Systems.UnitManager.CopyUnitState(unit, clonedUnit)
        XTD.Systems.CreepManager.IncrementCreepsAlive()

        local originalData = XTD.Systems.SpawnManager.creeps[GetHandleId(unit)]
        local clonedData = XTD.Core.Helpers.CopyTable(originalData)

        clonedData.unit = clonedUnit
        clonedData.lastX = GetUnitX(clonedUnit)
        clonedData.lastY = GetUnitY(clonedUnit)
        clonedData.nextRegion = nil

        XTD.Systems.SpawnManager.creeps[GetHandleId(clonedUnit)] = clonedData

        XTD.Systems.CreepPathManager.OnUnitEnterRegion(clonedUnit, cloneRegion)

        PauseTimer(timer)
        DestroyTimer(timer)
    end)
end

--[[
    Get whether creep cloning is enabled.
]]
function XTD.Systems.CreepPathManager.CloneEnabled()
    local gameSettings = XTD.Core.GameManager.GetGameSettings()

    if gameSettings.name == "Easy" then
        return false
    elseif gameSettings.name == "Medium" then
        return false
    elseif gameSettings.name == "Hard" or gameSettings.name == "Insane" or gameSettings.name == "Nightmare" then
        return true
    end

    return false
end

--[[
    Check all current creeps intermittently.
    This helps stop creeps from getting stuck.
]]
function XTD.Systems.CreepPathManager.CheckCreeps()
    for handleId, creepData in pairs(XTD.Systems.SpawnManager.creeps) do
        local unit = creepData.unit
        
        local x = GetUnitX(unit)
        local y = GetUnitY(unit)

        if math.abs(x - creepData.lastX) < 5 and math.abs(y - creepData.lastY) < 5 then
            local nextRegion = XTD.Systems.RegionManager.ResolveRegion(creepData.nextRegion)

            if nextRegion ~= nil then
                local nextX, nextY = XTD.Systems.RegionManager.GetRectCenter(nextRegion)

                if nextX ~= nil and nextY ~= nil then
                    IssuePointOrder(unit, "move", nextX, nextY)
                end
            end
        end

        creepData.lastX = x
        creepData.lastY = y
    end
end

--[[
    Order a summoned unit to go to the next region.
]]
function XTD.Systems.CreepPathManager.OrderSummonedAddToNextRegion(unit, regionName)
    if unit == nil then
        XTD.Core.Debug("CreepPathManager", "unit is nil.", "OrderSummonedAddToNextRegion")
        return
    end

    if regionName == nil then
        XTD.Core.Debug("CreepPathManager", "regionName is nil.", "OrderSummonedAddToNextRegion")
        return
    end

    local rect = XTD.Systems.RegionManager.ResolveRegion(regionName)

    if rect == nil then
        XTD.Core.Debug("CreepPathManager", "rect is nil.", "OrderSummonedAddToNextRegion")
        return
    end

    local x, y = XTD.Systems.RegionManager.GetRectCenter(rect)

    if x == nil or y == nil then
        XTD.Core.Debug("CreepPathManager", "x or y is nil.", "OrderSummonedAddToNextRegion")
        return
    end

    IssuePointOrder(unit, "move", x, y)
end