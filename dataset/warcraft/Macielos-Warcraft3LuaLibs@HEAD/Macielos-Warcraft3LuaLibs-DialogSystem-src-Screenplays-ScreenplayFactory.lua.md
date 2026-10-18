---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "DialogSystem/src/Screenplays/ScreenplayFactory.lua"
ref: HEAD
---

if Debug then Debug.beginFile "ScreenplayFactory" end
ScreenplayFactory = {
    screenplayBuilders = {}
}

do

    --- Creates an actor to be used in screenplay dialog lines. You can either create your actors every time in the beginning
    --- of a screenplay or create them all on map init and store in some global variables
    ---@param unit unit
    ---@param customName string
    ---@return ScreenplaySystem.actor
    function ScreenplayFactory.createActor(unit, customName)
        assert(unit ~= nil, "Actor unit cannot be null")
        local actor = ScreenplaySystem.actor:new()
        actor.unit = unit
        if customName then
            actor.name = customName
        else
            actor.name = UnitUtils:GetUnitProperName(unit)
        end
        return actor
    end

    --- Alternatively if you don't want to create a unit, you can also assign unit type and player to an actor, optionally with custom name
    --- Of course, with such actors camera panning, unit flashes and animations are not available
    ---@param unitType number
    ---@param player player
    ---@param customName string
    ---@return ScreenplaySystem.actor
    function ScreenplayFactory.createActorFromType(unitType, player, customName)
        assert(unitType ~= nil, "Actor unit type cannot be null")
        assert(player ~= nil, "Actor player cannot be null")
        local actor = ScreenplaySystem.actor:new()
        actor.unitType = unitType
        actor.player = player
        if customName then
            actor.name = customName
        else
            actor.name = GetObjectName(unitType)
        end
        return actor
    end

    local function printDebug(msg)
        if ScreenplaySystem.debug and SimpleUtils.globalDebug then
            print("[SCREENPLAY FACTORY] " .. msg)
        end
    end

    --- Stores a screenplay builder function under a given name. The function will be called when starting a scene by name.
    ---@param name string
    ---@param screenplayBuilderFunction function
    function ScreenplayFactory:saveBuilder(name, screenplayBuilderFunction)
        if (ScreenplayFactory.screenplayBuilders[name]) then
            SimpleUtils.printWarn("Duplicate screenplay key " .. name .. ", previous one will be overriden")
        end
        printDebug("Saving screenplay builder " .. tostring(name))
        ScreenplayFactory.screenplayBuilders[name] = screenplayBuilderFunction
    end

    --- Stores a screenplay builder function under a given name. The function will be called when starting a scene by name. A simpler variant of function above that accepts a raw message chain
    ---@param name string
    ---@param screenplayBuilderFunction function
    function ScreenplayFactory:saveBuilderForMessageChain(name, messageChainFunction)
        return self:saveBuilder(name, function()
            return ScreenplaySystem.chain:buildFromObject(messageChainFunction())
        end)
    end
end
if Debug then Debug.endFile() end
