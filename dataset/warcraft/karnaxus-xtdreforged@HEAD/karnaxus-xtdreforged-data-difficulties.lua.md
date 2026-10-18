---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "data/difficulties.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Difficulties                                        |
    | =================================================== |
    | All the game difficulties.                          |
    | =================================================== |
    +-----------------------------------------------------+
]]

--[[
    All the game difficulties.
]]
XTD.Data.Difficulties = {
    Easy = {
        name = "Easy",
        hpMultiplier = 1.25,
        goldMultiplier = 1.35,
        creepMultiplier = 1.10,
        damageMultiplier = 0.75,
        movementSpeedMultiplier = 1.00,
        initialDelay = 45.00,
        betweenWaveEnabled = true,
        betweenWaveDelay = 25.00,
        initialGold = 1000,
        intervalGoldSeconds = 10,
        intervalGoldAmount = 50,
        maxLeaks = 120
    },

    Medium = {
        name = "Medium",
        hpMultiplier = 1.45,
        goldMultiplier = 1.15,
        creepMultiplier = 1.20,
        damageMultiplier = 1.00,
        movementSpeedMultiplier = 1.10,
        initialDelay = 35.00,
        betweenWaveEnabled = true,
        betweenWaveDelay = 20.00,
        initialGold = 900,
        intervalGoldSeconds = 15,
        intervalGoldAmount = 45,
        maxLeaks = 100
    },

    Hard = {
        name = "Hard",
        hpMultiplier = 1.65,
        goldMultiplier = 0.95,
        creepMultiplier = 1.30,
        damageMultiplier = 1.25,
        movementSpeedMultiplier = 1.20,
        initialDelay = 25.00,
        betweenWaveEnabled = false,
        betweenWaveDelay = 0.00,
        initialGold = 800,
        intervalGoldSeconds = 20,
        intervalGoldAmount = 40,
        maxLeaks = 80
    },

    Insane = {
        name = "Insane",
        hpMultiplier = 1.85,
        goldMultiplier = 0.75,
        creepMultiplier = 1.40,
        damageMultiplier = 1.50,
        movementSpeedMultiplier = 1.30,
        initialDelay = 15.00,
        betweenWaveEnabled = false,
        betweenWaveDelay = 0.00,
        initialGold = 700,
        intervalGoldSeconds = 25,
        intervalGoldAmount = 35,
        maxLeaks = 60
    },

    Nightmare = {
        name = "Nightmare",
        hpMultiplier = 2.05,
        goldMultiplier = 0.55,
        creepMultiplier = 1.50,
        damageMultiplier = 1.75,
        movementSpeedMultiplier = 1.40,
        initialDelay = 10.00,
        betweenWaveEnabled = false,
        betweenWaveDelay = 0.00,
        initialGold = 600,
        intervalGoldSeconds = 30,
        intervalGoldAmount = 30,
        maxLeaks = 40
    }
}
