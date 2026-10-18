---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "DialogSystem/Screenplays/Demo/screenplay02Elf.lua"
ref: HEAD
---

ScreenplayFactory:saveBuilderForMessageChain("elf", function()
    --an example of an actor without a unit
    local narratorUnitType = FourCC('nmed')
    local narratorPlayer = Player(1)
    local actorNarrator = ScreenplayFactory.createActorFromType(narratorUnitType, narratorPlayer, "Narrator")

    return {
        [1] = {
            text = "Soon, in his epic quest our mighty hero stood upon a High Elven sanctuary...",
            actor = actorNarrator,
            func = function()
                ScreenplayUtils.interpolateCamera(gg_cam_elf01a, gg_cam_elf01b, 90)
            end,
        },
        [2] = {
            text = "Greeting, noble warrior. I am a priest and can enlighten you so you can fully embrace the Holy Light on your path. Holy light blablala holy light blablala holy light blablala holy light blabla (you may wanna skip that or check if scrolling works) blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala holy light blablala ",
            actor = actorElf,
        },
        [3] = {
            text = "Ssssscrew this. Got anything to drink, longear?",
            actor = actorFootman,
        },
        [4] = {
            text = "I do not think you should drink anymore. Allow me to cast Dispel on you so that you are sober again.",
            actor = actorElf,
        },
        [5] = {
            text = "What!? No, no, don't you dare, inhuman beast!",
            actor = actorFootman,
            trigger = gg_trg_Scene_Elf_Cast_Dispel,
            fadeOutDuration = 1.0,
        },
        [6] = {
            text = "Was this the end of our hero's journey, you might ask? Well...",
            actor = actorNarrator,
        },
        [7] = {
            fadeInDuration = 1.0,
            delayText = 2.0,
            text = "Hahahahaha, your foul magic doesn't work on me, inhuman! I'm still drunk! And I wanna drink even more!",
            actor = actorFootman,
            fadeOutDuration = 2.0,
            func = function()
                ScreenplayUtils.interpolateCamera(gg_cam_elf02a, gg_cam_elf02b, 30)
            end
        },
    }
end)
