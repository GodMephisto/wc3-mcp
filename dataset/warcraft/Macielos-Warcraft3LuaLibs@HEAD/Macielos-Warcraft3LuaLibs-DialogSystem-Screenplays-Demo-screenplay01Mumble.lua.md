---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "DialogSystem/Screenplays/Demo/screenplay01Mumble.lua"
ref: HEAD
---

ScreenplayFactory:saveBuilderForMessageChain("mumble", function()
    return {
        [1] = {
            text = "<mumbling> Not soon enough, you inhuman beast.",
            actor = actorFootman,
        },
    }
end)