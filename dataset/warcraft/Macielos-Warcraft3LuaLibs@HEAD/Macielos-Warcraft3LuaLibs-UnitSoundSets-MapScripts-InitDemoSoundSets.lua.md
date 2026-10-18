---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "UnitSoundSets/MapScripts/InitDemoSoundSets.lua"
ref: HEAD
---

if Debug then Debug.beginFile "InitDemoSoundSets" end
do
    --your unit codes from object editor
    local REYNART = FourCC('H000')
    local INAYLIA = FourCC('H001')

    --your ability codes from object editor
    local KNIVES = FourCC('AEfk')
    local SHIFT = FourCC('AEbl')
    local STEALTH = FourCC('AOwk')
    local LIQUIDATION = FourCC('ANfd')

    local PREFIX = 'war3mapImported\\'

    --just some utility functions so you don't have to repeat the full path for every unit
    local function addSoundSet(unitId, filePrefix)
        UnitSoundSets:addUnitSoundSet(unitId, PREFIX .. filePrefix)
    end

    local function addAbilitySoundSet(unitId, abilityId, filePrefix)
        UnitSoundSets:addUnitAbilitySingleSound(unitId, abilityId, PREFIX .. filePrefix)
    end

    --your function, it will be called on map initialization

    OnInit.final(function()
        print('InitDemoSoundSets START')
        addSoundSet(REYNART, 'Reynart')
        addSoundSet(INAYLIA, 'Inaylia')
        addAbilitySoundSet(INAYLIA, KNIVES, 'InalkaKnives')
        addAbilitySoundSet(INAYLIA, SHIFT, 'InalkaShift')
        addAbilitySoundSet(INAYLIA, STEALTH, 'InalkaStealth')
        addAbilitySoundSet(INAYLIA, LIQUIDATION, 'InalkaLiquidation')
        UnitSoundSets:addUnitSoundSet(FourCC('Hamg'), 'units\\human\\HeroArchMage\\HeroArchMage')
        print('InitDemoSoundSets DONE')
    end)
end

if Debug then Debug.endFile() end
