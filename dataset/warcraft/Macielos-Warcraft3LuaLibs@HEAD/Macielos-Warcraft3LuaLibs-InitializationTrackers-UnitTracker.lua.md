---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "InitializationTrackers/UnitTracker.lua"
ref: HEAD
---

Debug.beginFile "UnitTracker"
do
    UnitTracker = {
        trackedGroupsAndConditions = {},
        trackedGroupsAndConditionsByName = {},
        calls = 0,
        debug = true,
    }

    local function printDebug(msg)
        if UnitTracker.debug and SimpleUtils.globalDebug then
            print("[UNIT TRACKER] " .. msg)
        end
    end

    function UnitTracker:register(name, condition)
        assert(self.trackedGroupsAndConditions[name] == nil, 'UnitTracker: group ' .. name .. ' already defined')
        assert(type(condition) == 'function', 'UnitTracker: condition must be a function')
        local group = CreateGroup()
        local object = {
            name = name,
            group = group,
            condition = condition
        }
        table.insert(self.trackedGroupsAndConditions, object)
        self.trackedGroupsAndConditionsByName[name] = object
        printDebug('Registered condition ' .. name)
    end

    function UnitTracker:getTrackedUnitGroup(name)
        local groupAndCondition = self.trackedGroupsAndConditionsByName[name]
        assert(groupAndCondition, 'TrackedUnitGroup ' .. name .. ' not found')
        return groupAndCondition.group
    end

    local function track(unit)
        if unit ~= nil then
            for i, trackedGroupAndCondition in ipairs(UnitTracker.trackedGroupsAndConditions) do
                if not IsUnitInGroup(unit, trackedGroupAndCondition.group) and trackedGroupAndCondition.condition(unit) == true then
                    GroupAddUnit(trackedGroupAndCondition.group, unit)
                end
            end
            UnitTracker.calls = UnitTracker.calls + 1
        end
        return unit
    end

    local originalCreateUnit = CreateUnit
    CreateUnit = function(...)
        return track(originalCreateUnit(...))
    end

    local originalCreateUnitByName = CreateUnitByName
    CreateUnitByName = function(...)
        return track(originalCreateUnitByName(...))
    end

    local originalCreateUnitAtLoc = CreateUnitAtLoc
    CreateUnitAtLoc = function(...)
        return track(originalCreateUnitAtLoc(...))
    end

    local originalCreateUnitAtLocByName = CreateUnitAtLocByName
    CreateUnitAtLocByName = function(...)
        return track(originalCreateUnitAtLocByName(...))
    end

    local originalBlzCreateUnitWithSkin = BlzCreateUnitWithSkin
    BlzCreateUnitWithSkin = function(...)
        return track(originalBlzCreateUnitWithSkin(...))
    end

    local originalRestoreUnit = RestoreUnit
    RestoreUnit = function(...)
        return track(originalRestoreUnit(...))
    end

    printSkillDebug("UnitTracker overrides DONE")
end
Debug.endFile()