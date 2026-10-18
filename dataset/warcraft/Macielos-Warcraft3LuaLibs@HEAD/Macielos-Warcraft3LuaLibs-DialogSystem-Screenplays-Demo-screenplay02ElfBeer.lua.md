---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "DialogSystem/Screenplays/Demo/screenplay02ElfBeer.lua"
ref: HEAD
---

ScreenplayFactory:saveBuilderForMessageChain("elfBeer", function()
    return {
        [1] = {
            text = "Heh, I just knew this priest wouldn't speak this Holy Light bullcrap sober.",
            actor = actorFootman,
        },
    }
end)