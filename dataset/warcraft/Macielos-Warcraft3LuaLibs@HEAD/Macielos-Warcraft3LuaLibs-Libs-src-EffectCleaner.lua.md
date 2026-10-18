---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "Libs/src/EffectCleaner.lua"
ref: HEAD
---

Debug.beginFile "EffectCleaner"
do
    EffectCleaner = {
        unitsToEffects = {}
    }

    function EffectCleaner:register(unit, effect)
        if self.unitsToEffects[unit] == nil then
            self.unitsToEffects[unit] = {}
        end
        self.unitsToEffects[unit][effect] = true
    end

    function EffectCleaner:removeEffect(unit, effectToRemove)
        local effects = EffectCleaner.unitsToEffects[unit]
        if effects ~= nil then
            effects[effectToRemove] = nil
            DestroyEffect(effectToRemove)
        end
    end

    function EffectCleaner:remove(unit)
        self.unitsToEffects[unit] = nil
    end

    OnInit.final(function()
        RegisterUnitDeathAction(nil, function(dyingUnit)
            local effects = EffectCleaner.unitsToEffects[dyingUnit]
            if effects ~= nil then
                for effect, _ in pairs(effects) do
                    DestroyEffect(effect)
                end
                EffectCleaner:remove(dyingUnit)
            end
        end)
    end)
end
Debug.endFile()