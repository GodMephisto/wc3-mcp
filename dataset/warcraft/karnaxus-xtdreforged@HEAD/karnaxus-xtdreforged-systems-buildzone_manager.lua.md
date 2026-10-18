---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/buildzone_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Buildzone Manager                                   |
    | =================================================== |
    | Manages all the player build zones.                 |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.BuildZoneManager = XTD.Systems.BuildZoneManager or {}

XTD.Systems.BuildZoneManager.zones = {}
XTD.Systems.BuildZoneManager.permissions = {}

--[[
    Initialize the Build Zone Manager.
]]
function XTD.Systems.BuildZoneManager.Init()
    XTD.Core.Debug("BuildZoneManager", "Initializing the Build Zone Manager...", "Init")

    XTD.Systems.BuildZoneManager.SetupBuildZones()

    XTD.Core.Debug("BuildZoneManager", "Build Zone Manager initialized.", "Init")
end

--[[
    Setup the build zones.
]]
function XTD.Systems.BuildZoneManager.SetupBuildZones()
    local self = XTD.Systems.BuildZoneManager

    self.zones = {}
    self.permissions = {}

    for ownerId, zoneNames in pairs(XTD.Data.Regions.BuildZones) do
        self.zones[ownerId] = {}
        self.permissions[ownerId] = {}

        for _, zoneName in ipairs(zoneNames) do
            local rect = XTD.Systems.RegionManager.ResolveRegion(zoneName)

            if rect ~= nil then
                table.insert(self.zones[ownerId], rect)
            else
                XTD.Core.Debug("BuildZoneManager", "Missing region: " .. zoneName, "SetupBuildZones")
            end
        end

        for builderId = 0, 7 do
            self.permissions[ownerId][builderId] = ownerId == builderId
        end
    end

    self.RegisterBuildHandler()
end

--[[
    Get the zone owner for a given point.
]]
function XTD.Systems.BuildZoneManager.GetZoneOwnerAt(x, y)
    local self = XTD.Systems.BuildZoneManager

    for ownerId, rects in pairs(self.zones) do
        for _, rect in ipairs(rects) do
            if RectContainsCoords(rect, x, y) then
                return ownerId
            end
        end
    end

    return nil
end

--[[
    Check if a player is active.
]]
function XTD.Systems.BuildZoneManager.IsPlayerActive(playerId)
    local player = Player(playerId)

   return XTD.Core.PlayerManager.IsPlaying(player) 
end

--[[
    Check if a player can build in the given x, y point.
]]
function XTD.Systems.BuildZoneManager.CanBuild(player, x, y)
    local builderId = GetPlayerId(player)
    local zoneOwnerId = XTD.Systems.BuildZoneManager.GetZoneOwnerAt(x, y)

    if zoneOwnerId == nil then
        return true
    end

    if builderId == zoneOwnerId then
        return true
    end

    if not XTD.Systems.BuildZoneManager.IsPlayerActive(zoneOwnerId) then
        return true
    end

    return XTD.Systems.BuildZoneManager.permissions[zoneOwnerId][builderId] == true
end

--[[
    Set permissions for a build zone.
]]
function XTD.Systems.BuildZoneManager.SetPermission(zoneOwnerId, builderId, allowed)
    local self = XTD.Systems.BuildZoneManager

    if self.permissions[zoneOwnerId] == nil then
        return false
    end

    if builderId == "all" then
        for i = 0, 7 do
            self.permissions[zoneOwnerId][i] = allowed
        end

        return true
    end

    self.permissions[zoneOwnerId][builderId] = allowed

    return true
end

--[[
    Register the build handler.
]]
function XTD.Systems.BuildZoneManager.RegisterBuildHandler()
    local trigger = CreateTrigger()

    TriggerRegisterAnyUnitEventBJ(trigger, EVENT_PLAYER_UNIT_CONSTRUCT_START)

    TriggerAddAction(trigger, function ()
        local unit = GetTriggerUnit()

        if unit == nil then
            XTD.Core.Debug("BuildZoneManager", "unit is nil.", "RegisterBuildHandler")
            return
        end

        local player = GetOwningPlayer(unit)

        if player == nil then
            XTD.Core.Debug("BuildZoneManager", "player is nil.", "RegisterBuildHandler")
            return
        end

        if XTD.Systems.BuildZoneManager.CanBuild(player, GetUnitX(unit), GetUnitY(unit)) then
            return
        end

        local unitType = GetUnitTypeId(unit)
        local towerValue = XTD.Data.TowerValues[unitType]
        local refundGold = 0

        if towerValue ~= nil then
            refundGold = towerValue.gold or 0
        end

        XTD.Core.PlayerManager.AddResource(player, "gold", refundGold)
        RemoveUnit(unit)

        XTD.UI.MessageManager.Player(
            player,
            XTD.Core.Color.Red("You are not allowed to build in that player's zone without permission from that player.")
        )
    end)
end