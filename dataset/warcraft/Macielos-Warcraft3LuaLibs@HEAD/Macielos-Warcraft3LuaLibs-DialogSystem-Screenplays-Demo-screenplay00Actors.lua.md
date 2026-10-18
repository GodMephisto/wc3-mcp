---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "DialogSystem/Screenplays/Demo/screenplay00Actors.lua"
ref: HEAD
---

function createActors()
    actorFootman = ScreenplayFactory.createActor(udg_footman, 'Footman Valdeck')
    actorDwarf = ScreenplayFactory.createActor(udg_dwarf, 'Rifleman Isgrinn')
    actorElf = ScreenplayFactory.createActor(udg_elf, "Elven Priest with Stick in his Holy Arse")
    actorGrunt = ScreenplayFactory.createActor(udg_grunt, 'Bored Grunt')
    actorPeon = ScreenplayFactory.createActor(udg_peon, 'Fearful Peon')
end