---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "Libs/src/ConstructionTracker.lua"
ref: HEAD
---

Debug.beginFile "ConstructionTracker"
do
    local unitsUnderConstruction = CreateGroup()

    ConstructionTracker = {}

    function ConstructionTracker:isUnderConstruction(unit)
        return IsUnitInGroup(unit, unitsUnderConstruction)
    end

    OnInit.final(function()
        local beginTrigger = CreateTrigger()
        TriggerRegisterAnyUnitEventBJ(beginTrigger, EVENT_PLAYER_UNIT_CONSTRUCT_START)
        TriggerAddAction(beginTrigger, function()
            GroupAddUnit(unitsUnderConstruction, GetConstructingStructure())
        end)

        local cancelTrigger = CreateTrigger()
        TriggerRegisterAnyUnitEventBJ(cancelTrigger, EVENT_PLAYER_UNIT_CONSTRUCT_CANCEL)
        TriggerAddAction(cancelTrigger, function()
            GroupRemoveUnit(unitsUnderConstruction, GetCancelledStructure())
        end)

        local finishTrigger = CreateTrigger()
        TriggerRegisterAnyUnitEventBJ(finishTrigger, EVENT_PLAYER_UNIT_CONSTRUCT_FINISH)
        TriggerAddAction(finishTrigger, function()
            GroupRemoveUnit(unitsUnderConstruction, GetConstructedStructure())
        end)

        RegisterUnitDeathAction(nil, function(dyingUnit)
            if IsUnitInGroup(dyingUnit, unitsUnderConstruction) then
                GroupRemoveUnit(unitsUnderConstruction, dyingUnit)
            end
        end)
    end)
end
Debug.endFile()