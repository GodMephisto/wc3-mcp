---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "DialogSystem/Screenplays/Demo/screenplay00Intro.lua"
ref: HEAD
---

ScreenplayFactory:saveBuilderForMessageChain("intro", function()
    return {
        [1] = {
            text = ScreenplayMessages['intro001'],
            sound = ScreenplaySounds['intro001'],
            actor = actorFootman,
        },
    }
end)
