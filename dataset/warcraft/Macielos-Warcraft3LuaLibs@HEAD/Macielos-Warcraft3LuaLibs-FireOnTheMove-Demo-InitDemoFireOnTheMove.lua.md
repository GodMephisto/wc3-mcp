---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "FireOnTheMove/Demo/InitDemoFireOnTheMove.lua"
ref: HEAD
---

if Debug then Debug.beginFile "InitDemoFireOnTheMove" end
do
    local FAFNIR = FourCC('h000') --unit id from object editor
    local FENRIS = FourCC('h001') --unit id from object editor

    local ABILITY_FIRE_ON_THE_MOVE = FourCC('A000') --ability id from object editor

    local fireOnTheMoveUnitTypes = {}
    fireOnTheMoveUnitTypes[FAFNIR] = {
        bone = 'Turret_Base', --bone name that must be rotatable
        targetGround = true,
        targetStructures = true,
        targetAir = false,
        fireEffectPath = 'war3mapImported\\PartFafnirGun.mdl', --optional - effect to spawn when firing a missile
        projectileLaunchOffset = {
            x = 0,
            y = 90,
            z = 90
        }
    }
    fireOnTheMoveUnitTypes[FENRIS] = {
        bone = 'Turret_Base',
        targetGround = true,
        targetStructures = true,
        targetAir = true,
        fireEffectPath = 'war3campImported\\Particles_ScoutCar_Fenris.mdl',
        projectileLaunchOffset = {
            x = 0,
            y = 90,
            z = 90
        },
    }
    local abilitiesAllowingFireOnTheMove = {
        [1] = ABILITY_FIRE_ON_THE_MOVE
    }

    --if you dont want to make fire on the move require any ability, just replace line below with: FireOnTheMove:init(fireOnTheMoveUnitTypes, nil)
    FireOnTheMove:init(fireOnTheMoveUnitTypes, abilitiesAllowingFireOnTheMove)
end
if Debug then Debug.endFile() end