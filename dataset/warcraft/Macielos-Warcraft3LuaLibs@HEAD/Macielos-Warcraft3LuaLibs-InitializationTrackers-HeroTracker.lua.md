---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "InitializationTrackers/HeroTracker.lua"
ref: HEAD
---

Debug.beginFile "HeroTracker"
--MUST BE PUT AFTER UNIT TRACKER
do
    UnitTracker:register('heroes', function(unit)
        return IsUnitHero(unit)
    end)
end
Debug.endFile()