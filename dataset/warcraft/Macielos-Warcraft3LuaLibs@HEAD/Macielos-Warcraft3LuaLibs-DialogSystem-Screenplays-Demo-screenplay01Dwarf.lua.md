---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "DialogSystem/Screenplays/Demo/screenplay01Dwarf.lua"
ref: HEAD
---

ScreenplayFactory:saveBuilderForMessageChain("dwarf", function()
    return {
        [1] = {
            text = "Hey, lad. Good time drinkin' with ya! Until the next time!",
            actor = actorDwarf,
        },
        [2] = {
            text = "Yeeeeah, s-s-s-sure thing, pal!",
            actor = actorFootman,
        },
    }
end)