---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "systems/unit_manager.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Unit Manager                                        |
    | =================================================== |
    | Unit Manager                                        |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Systems.UnitManager = XTD.Systems.UnitManager or {}

--[[
    Initialize the Unit Manager.
]]
function XTD.Systems.UnitManager.Init()
    XTD.Core.Debug("UnitManager", "Initializing the Unit Manager...")
    XTD.Core.Debug("UnitManager", "Unit Manager initialized.")
end

--[[
    Spawn a unit at a point.
]]
function XTD.Systems.UnitManager.SpawnUnitAtPoint(player, unitType, x, y, facing, teleport)
    if player == nil or unitType == nil or x == nil or y == nil then
        return
    end

    local unit = CreateUnit(
        player,
        unitType,
        x,
        y,
        facing or 270.00
    )

    if teleport ~= nil and teleport then
        XTD.Systems.UnitManager.TeleportEffect(unit)
    end

    return unit
end

--[[
    Get the max life for a unit.
]]
function XTD.Systems.UnitManager.GetMaxLife(unit)
    if unit == nil then
        XTD.Core.Debug("UnitManager", "unit is nil.", "GetMaxLife")
        return
    end

    return GetUnitState(unit, UNIT_STATE_MAX_LIFE)
end

--[[
    Set the max life for a unit.
]]
function XTD.Systems.UnitManager.SetMaxLife(unit, maxLife)
    if unit == nil then
        XTD.Core.Debug("UnitManager", "unit is nil.", "SetMaxLife")
        return
    end

    BlzSetUnitMaxHP(unit, maxLife)
    SetUnitState(unit, UNIT_STATE_LIFE, maxLife)
end

--[[
    Get the current HP/life for a unit.
]]
function XTD.Systems.UnitManager.GetCurrentLife(unit)
    if unit == nil then
        XTD.Core.Debug("UnitManager", "unit is nil.", "GetCurrentLife")
        return
    end

    return GetUnitState(unit, UNIT_STATE_LIFE)
end

--[[
    Heal a unit by either percent or flat HP.
]]
function XTD.Systems.UnitManager.HealUnit(unit, value, healType)
    if unit == nil then
        XTD.Core.Debug("UnitManager", "unit is nil.", "HealUnit")
        return
    end

    if value == nil then
        XTD.Core.Debug("UnitManager", "value is nil.", "HealUnit")
        return
    end

    if value <= 0 then
        return
    end

    local currentHP = XTD.Systems.UnitManager.GetCurrentLife(unit)
    local maxHP = XTD.Systems.UnitManager.GetMaxLife(unit)
    local hpToHeal = value

    if healType == "percent" then
        hpToHeal = maxHP * (value / 100.00)
    end

    SetUnitState(
        unit,
        UNIT_STATE_LIFE,
        math.min(currentHP + hpToHeal, maxHP)
    )
end

--[[
    Get the movement speed for a unit.
]]
function XTD.Systems.UnitManager.GetMovementSpeed(unit)
    if unit == nil then
        return 0
    end

    return GetUnitMoveSpeed(unit)
end

--[[
    Set the movement speed for a unit.
]]
function XTD.Systems.UnitManager.SetMovementSpeed(unit, speed)
    if unit == nil or speed == nil then
        return
    end

    SetUnitMoveSpeed(unit, speed)
end

--[[
    Disable collision for a unit.
]]
function XTD.Systems.UnitManager.DisableCollision(unit)
    if unit == nil then
        return
    end

    SetUnitPathing(unit, false)
end

--[[
    Apply the teleport effect for a unit.
]]
function XTD.Systems.UnitManager.TeleportEffect(unit)
    if unit == nil then
        return
    end

    local x = GetUnitX(unit)
    local y = GetUnitY(unit)

    DestroyEffect(AddSpecialEffect(
        "Abilities\\Spells\\Human\\MassTeleport\\MassTeleportTarget.mdl",
        x,
        y
    )) 
end

--[[
    Disable the unit from attacking (at least their acquire range).
]]
function XTD.Systems.UnitManager.DisableAttack(unit)
    if unit == nil then
        XTD.Core.Debug("UnitManager", "unit is nil.", "DisableAttack")
        return
    end

    SetUnitAcquireRange(unit, 0.00)
end

--[[
    Enable the unit to attack (via their acquire range).
]]
function XTD.Systems.UnitManager.EnableAttack(unit)
    if unit == nil then
        XTD.Core.Debug("UnitManager", "unit is nil.", "EnableAttack")
        return
    end

    SetUnitAcquireRange(unit, 500.00)
end

--[[
    Copy the unit state from one unit to another.
]]
function XTD.Systems.UnitManager.CopyUnitState(sourceUnit, targetUnit)
    if sourceUnit == nil then
        XTD.Core.Debug("UnitManager", "sourceUnit is nil.", "CopyUnitState")
        return
    end

    if targetUnit == nil then
        XTD.Core.Debug("UnitManager", "targetUnit is nil.", "CopyUnitState")
        return
    end

    SetUnitState(
        targetUnit,
        UNIT_STATE_LIFE,
        GetUnitState(sourceUnit, UNIT_STATE_LIFE)
    )

    SetUnitMoveSpeed(
        targetUnit,
        GetUnitMoveSpeed(sourceUnit)
    )
end

--[[
    Get the unit type id for a unit.
]]
function XTD.Systems.UnitManager.GetUnitTypeId(unit)
    if unit == nil then
        XTD.Core.Debug("UnitManager", "unit is nil.", "GetUnitTypeId")
        return nil
    end

    return GetUnitTypeId(unit)
end

--[[
    Remove a unit from the map on leak.
]]
function XTD.Systems.UnitManager.RemoveUnitOnLeak(unit)
    if unit == nil then
        XTD.Core.Debug("UnitManager", "unit is nil.", "RemoveUnitOnLeak")
        return
    end

    XTD.Systems.UnitManager.TeleportEffect(unit)

    ShowUnit(unit, false)
    KillUnit(unit)
end

--[[
    Set the armor for a unit.
]]
function XTD.Systems.UnitManager.SetUnitArmor(unit, value)
    if unit == nil then
        XTD.Core.Debug("UnitManager", "unit is nil.", "SetUnitArmor")
        return
    end

    if value == nil then
        XTD.Core.Debug("UnitManager", "value is nil.", "SetUnitArmor")
        return
    end

    BlzSetUnitArmor(unit, value)
end

--[[
    Set the scale for a unit.
]]
function XTD.Systems.UnitManager.SetUnitScale(unit, scale)
    if unit == nil then
        XTD.Core.Debug("UnitManager", "unit is nil.", "SetUnitScale")
        return
    end

    if scale == nil then
        XTD.Core.Debug("UnitManager", "scale is nil.", "SetUnitScale")
        return
    end

    BlzSetUnitRealField(unit, UNIT_RF_SCALING_VALUE, scale)
end

--[[
    Set the life for a unit.
]]
function XTD.Systems.UnitManager.SetUnitLife(unit, value)
    if unit == nil then
        XTD.Core.Debug("UnitManager", "unit is nil.", "SetUnitLife")
        return
    end

    if value == nil then
        XTD.Core.Debug("UnitManager", "value is nil.", "SetUnitLife")
        return
    end

    SetUnitState(
        unit,
        UNIT_STATE_LIFE,
        value
    )
end

--[[
    Get the health percentage for a unit.
]]
function XTD.Systems.UnitManager.GetHealthPercent(unit)
    if unit == nil then
        return 0
    end

    local life = GetUnitState(unit, UNIT_STATE_LIFE)
    local maxLife = GetUnitState(unit, UNIT_STATE_MAX_LIFE)

    if maxLife <= 0 then
        return 0
    end

    return (life / maxLife) * 100
end

--[[
    Transfer units from one player to another.
]]
function XTD.Systems.UnitManager.TransferUnits(fromPlayer, toPlayer)
    if fromPlayer == nil then
        XTD.Core.Debug("UnitManager", "fromPlayer is nil.", "TransferUnits")
        return
    end

    if toPlayer == nil then
        XTD.Core.Debug("UnitManager", "toPlayer is nil.", "TransferUnits")
        return
    end

    GroupEnumUnitsOfPlayer(group, fromPlayer, nil)

    ForGroup(group, function ()
        local unit = GetEnumUnit()

        if unit ~= nil and GetWidgetLife(unit) > 0.405 then
            SetUnitOwner(unit, toPlayer, true)
        end
    end)

    DestroyGroup(group)
end

--[[
    Blow up an unit.
]]
function XTD.Systems.UnitManager.BlowUpUnit(unit)
    DestroyEffect(
        AddSpecialEffect(
            "Abilities\\Weapons\\DemolisherMissile\\DemolisherMissile.mdl",
            GetUnitX(unit),
            GetUnitY(unit)
        )
    )

    XTD.Systems.SpawnManager.creeps[GetHandleId(unit)] = nil

    KillUnit(unit)
end

--[[
    Blow up all units in the game.
]]
function XTD.Systems.UnitManager.BlowUpAllUnits()
    local group = CreateGroup()

    for playerIndex = 0, bj_MAX_PLAYERS - 1 do
        GroupEnumUnitsOfPlayer(group, Player(playerIndex), nil)

        ForGroup(group, function ()
            local unit = GetEnumUnit()

            if unit ~= nil and GetUnitTypeId(unit) ~= 0 then
                XTD.Systems.UnitManager.BlowUpUnit(unit)
            end
        end)

        GroupClear(group)
    end

    DestroyGroup(group)
end