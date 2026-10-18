---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "DialogSystem/Screenplays/Demo/screenplay04Ghost.lua"
ref: HEAD
---

ScreenplayFactory:saveBuilderForMessageChain("ghost", function()
    --an example of an actor without a unit
    local ghostUnitType = FourCC('ngh1')
    local ghostPlayer = Player(2)
    local actorGhost = ScreenplayFactory.createActorFromType(ghostUnitType, ghostPlayer, "Forest Ghost")

    return {
        [1] = {
            text = "Who dares disrupt my slumber!?",
            actor = actorGhost,
        },
        [2] = {
            text = "What's that? There's nobody here.",
            actor = actorFootman,
        },
        [3] = {
            text = "I am the forest ghost! You can't see me! Tremble before me, mortal!",
            actor = actorGhost,
        },
        [4] = {
            text = "Screw this, I'm out.",
            actor = actorFootman,
        },
    }
end)