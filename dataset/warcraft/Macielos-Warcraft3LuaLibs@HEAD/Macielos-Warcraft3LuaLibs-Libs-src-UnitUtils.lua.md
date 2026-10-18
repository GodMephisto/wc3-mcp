---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "Libs/src/UnitUtils.lua"
ref: HEAD
---

if Debug then Debug.beginFile "UnitUtils" end

do
    UnitUtils = {}

    local location  = Location(0, 0)

    function UnitUtils:registerUnitDeathAction(onUnitDeath)
        RegisterPlayerUnitEvent(EVENT_PLAYER_UNIT_DEATH, function()
            local dyingUnit = GetDyingUnit()
            onUnitDeath(dyingUnit)
        end)
    end

    function UnitUtils:groupAllUnitsMatchingFilter(filter)
        local allUnits = CreateGroup()
        local playerUnits = CreateGroup()
        local index
        index = 0
        while true do
            GroupClear(playerUnits)
            GroupEnumUnitsOfPlayer(playerUnits, Player(index), filter)
            GroupAddGroup(playerUnits, allUnits)

            index = index + 1
            if index == bj_MAX_PLAYER_SLOTS then break end
        end
        DestroyGroup(playerUnits)

        return allUnits
    end

    function UnitUtils:forEachUnit(group, action)
        local groupSize = BlzGroupGetSize(group)
        for i = 0, groupSize - 1 do
            local unit = BlzGroupUnitAt(group, i)
            action(unit)
        end
    end

    function UnitUtils:forEachUnitReversed(group, action)
        local groupSize = BlzGroupGetSize(group)
        for i = groupSize - 1, 0, -1 do
            local unit = BlzGroupUnitAt(group, i)
            action(unit)
        end
    end

    function UnitUtils:forEachUnitUntilFalse(group, action)
        local groupSize = BlzGroupGetSize(group)
        for i = 0, groupSize - 1 do
            local unit = BlzGroupUnitAt(group, i)
            if action(unit) == false then
                return
            end
        end
    end

    function UnitUtils:GetLocZ(x, y)
        MoveLocation(location, x, y)
        return GetLocationZ(location)
    end

    function UnitUtils:getUnitZ(unit)
        return UnitUtils:GetLocZ(GetUnitX(unit), GetUnitY(unit)) + GetUnitFlyHeight(unit)
    end

    function UnitUtils:isEnemyUnit(player, unit)
        return UnitAlive(unit) and IsUnitEnemy(unit, player) and not IsUnitType(unit, UNIT_TYPE_STRUCTURE)
    end

    function UnitUtils:IsUnitHero(unit)
        return IsUnitType(unit, UNIT_TYPE_HERO)
    end

    function UnitUtils:GetUnitProperName(unit)
        if UnitUtils:IsUnitHero(unit) then
            return GetHeroName(unit)
        end
        return GetUnitName(unit)
    end

    function UnitUtils:GetHeroName(hero)
        return BlzGetUnitStringField(hero, ConvertUnitStringField(FourCC('upro')))
    end

end

if Debug then Debug.endFile() end
