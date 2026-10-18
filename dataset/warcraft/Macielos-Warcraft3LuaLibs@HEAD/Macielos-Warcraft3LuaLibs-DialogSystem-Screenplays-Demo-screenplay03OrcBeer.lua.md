---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "DialogSystem/Screenplays/Demo/screenplay03OrcBeer.lua"
ref: HEAD
---

ScreenplayFactory:saveBuilderForMessageChain("orcBeer", function()
    return {
        [1] = {
            text = "Ahh, there you have a camp, hummies. Gotta tell the warchief. Here's your beer, loser. Enjoy it... while you can, hehehe.",
            actor = actorGrunt,
        },
    }
end)