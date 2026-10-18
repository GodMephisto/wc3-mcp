---
type: vendor-doc
source_repo: "https://github.com/Macielos/Warcraft3LuaLibs"
source_path: "DialogSystem/Screenplays/Demo/screenplay03Orc.lua"
ref: HEAD
---

ScreenplayFactory:saveBuilderForMessageChain("orc", function()
    local sounds = {}
    sounds.grunt01 = gg_snd_GruntWhat2
    sounds.footman01 = gg_snd_FootmanWarcry1

    return {
        [1] = {
            text = "|cffff0000Hey, orc!|r",
            actor = actorFootman,
        },
        [2] = {
            text = "Whazzup, hummie?",
            actor = actorGrunt,
            anim = "stand two",
            sound = sounds.grunt01,
            delayNextItem = 4.0,
        },
        [3] = {
            actor = actorFootman,
            choices = {
                [1] = {
                    text = "I will fuck you up!",
                    onChoice = function()
                        ScreenplaySystem:currentItem().choices[1].visible = false
                    end,
                    onChoiceGoTo = 4
                },
                [2] = {
                    text = "Show me your wares.",
                    onChoiceGoTo = 7
                },
                [3] = {
                    text = "Where can I get drunk here?",
                    onChoice = function()
                        ScreenplaySystem:currentItem().choices[3].visible = false
                        ScreenplaySystem:currentItem().choices[4].visible = true
                    end,
                    onChoiceGoTo = 9
                },
                [4] = {
                    text = "You sure you have no alcohol left?",
                    visible = false,
                    onChoice = function()
                        ScreenplaySystem:currentItem().choices[4].visible = false
                    end,
                    onChoiceGoTo = 11
                },
                [5] = {
                    text = "I have to go.",
                    onChoiceGoTo = 19
                }
            }
        },
        [4] = {
            text = "I will fuck you up, for Lordaeron and the King!",
            actor = actorFootman,
            sound = sounds.footman01
        },
        [5] = {
            text = "Waaaaahaaaaa!",
            actor = actorPeon,
            trigger = gg_trg_Scene_Orc_Peon_Escape
        },
        [6] = {
            text = "Go home, hummie, you're drunk.",
            actor = actorGrunt,
            thenGoTo = 3
        },
        [7] = {
            text = "Show me your wares!",
            actor = actorFootman,
            thenGoToFunc = function()
                if sceneOrcAlreadyAskedAboutWares == true
                then
                    return 18
                else
                    sceneOrcAlreadyAskedAboutWares = true
                    return 8
                end
            end
        },
        [8] = {
            text = "Do I look like a merchant, hummie?!",
            actor = actorGrunt,
            thenGoTo = 3
        },
        [9] = {
            text = "Where can I get drunk here?",
            actor = actorFootman,
        },
        [10] = {
            text = "Sorry, drinking is Ur-Gora, we burned the last tavern in town yesterday. Besides, you're already drunk.",
            actor = actorGrunt,
            thenGoTo = 3,
        },
        [11] = {
            text = "You sure you have no alcohol left?",
            actor = actorFootman,
        },
        [12] = {
            text = "Ehhh, actually, I think I kept one last keg of beer. It will be yours in exchange for the location of your camp.",
            actor = actorGrunt,
        },
        [13] = {
            actor = actorFootman,
            choices = {
                [1] = {
                    text = "I'll take you there, just gimme my beer!",
                    onChoiceGoTo = 14
                },
                [2] = {
                    text = "Forget it, Horde filth!",
                    onChoiceGoTo = 16
                },
            }
        },
        [14] = {
            text = "I'll take you there, just gimme my beer!",
            actor = actorFootman,
        },
        [15] = {
            text = "Okay. Let's go.",
            actor = actorGrunt,
            thenEndScene = true,
            trigger = gg_trg_Scene_Orc_Follow,
        },
        [16] = {
            text = "Forget it, Horde filth!",
            actor = actorFootman,
        },
        [17] = {
            text = "Eh, it was worth a try",
            actor = actorGrunt,
            thenGoTo = 3
        },
        [18] = {
            text = "Told you, dumbass, I'm not a fucking merchant. Trading is Ur-Gora.",
            actor = actorGrunt,
            thenGoTo = 3,
        },
        [19] = {
            text = "I have to go.",
            actor = actorFootman,
        },
        [20] = {
            text = "Yeah, yeah, whatever.",
            actor = actorGrunt,
            thenEndScene = true
        },
        [21] = {
            text = "|cffff0000Hey, orc! I just visited that elven asshole, so I'm pissed as hell!|r",
            actor = actorFootman,
            thenGoTo = 2
        }
    }
end)