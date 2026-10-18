---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "core/waves.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Waves                                               |
    | =================================================== |
    | All the waves of creeps.                            |
    | =================================================== |
    +-----------------------------------------------------+
]]

--[[
    The wave categories.
]]
XTD.Data.WaveCategories = {
    NORMAL = "normal",
    AIR = "air",
    BONUS = "bonus",
    BOSS = "boss"
}

--[[
    The different armor types.
]]
XTD.Data.ArmorTypes = {
    FLESH = "Flesh",
    METAL = "Metal",
    STONE = "Stone",
    WOOD = "Wood",
    ETHEREAL = "Ethereal"
}

--[[
    The different defense types.
]]
XTD.Data.DefenseTypes = {
    NORMAL = "Normal",
    SMALL = "Small",
    MEDIUM = "Medium",
    LARGE = "Large",
    FORTIFIED = "Fortified",
    HERO = "Hero",
    DIVINE = "Divine",
    UNARMORED = "Unarmored"
}

--[[
    All waves of creeps.
]]
XTD.Data.Waves = {
    [1] = {
        name = "Kobold Taskmaster",
        description = "A gruff overseer who keeps the mining crews working with threats and brute force. He fiercely protects his clan's precious light, bellowing his infamous warning: \"You no take candle!\"",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n001"),
        total = 40,
        hp = 200,
        bounty = 5,
        goldForWave = 50,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.SMALL,
        armorBase = 0,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = -10000,
            y = -10760
        },
        boxPosition = {
            x = -9725,
            y = -10625
        },
        number = 1
    },

    [2] = {
        name = "Gnoll Brute",
        description = "Larger and more savage than their lesser kin, Gnoll Brutes delight in smashing anything that resists them. \"Only the strongest survive!\"",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n002"),
        total = 42,
        hp = 250,
        bounty = 6,
        goldForWave = 75,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.SMALL,
        armorBase = 1,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = -10000,
            y = -11520
        },
        boxPosition = {
            x = -9725,
            y = -11380
        },
        number = 2
    },

    [3] = {
        name = "Fallen Priest",
        description = "Once a faithful servant of the Light, the Fallen Priest now commands the powers of death and shadow. Its blasphemous incantations sap the strength of the living while bolstering the undead who march beside it.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n003"),
        total = 44,
        hp = 300,
        bounty = 8,
        goldForWave = 100,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.MEDIUM,
        armorBase = 0,
        armorBonus = 1,
        leakValue = 1,
        labelPosition = {
            x = -8985,
            y = -10760
        },
        boxPosition = {
            x = -8710,
            y = -10620
        },
        number = 3
    },

    [4] = {
        name = "Dark Troll Berserker",
        description = "Fueled by fury, these savage warriors charge without fear, cutting through anything that stands in their way.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n00P"),
        total = 45,
        hp = 350,
        bounty = 9,
        goldForWave = 125,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.MEDIUM,
        armorBase = 1,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = -8995,
            y = -11520
        },
        boxPosition = {
            x = -8705,
            y = -11380
        },
        number = 4
    },

    [5] = {
        name = "Black Dragon",
        description = "Ancient flying behemoths whose infernal flames and impenetrable scales make them among the most feared creatures in existence.",
        category = XTD.Data.WaveCategories.AIR,
        unitType = FourCC("n00T"),
        total = 46,
        hp = 400,
        bounty = 10,
        goldForWave = 150,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 0,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = -7975,
            y = -10760
        },
        boxPosition = {
            x = -7680,
            y = -10620
        },
        number = 5
    },

    [6] = {
        name = "Murloc Tiderunner",
        description = "Swift amphibious hunters that ride the tides, using their speed and overwhelming numbers to overrun unsuspecting foes.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n00U"),
        total = 40,
        hp = 450,
        bounty = 11,
        goldForWave = 175,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 1,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = -7975,
            y = -11520
        },
        boxPosition = {
            x = -7680,
            y = -11380
        },
        number = 6
    },

    [7] = {
        name = "Razormane Chieftain",
        description = "The battle-hardened leader of the Razormane tribe, whose immense strength and savage determination rally his warriors to victory.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n00V"),
        total = 41,
        hp = 500,
        bounty = 12,
        goldForWave = 200,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 2,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = -6950,
            y = -10760
        },
        boxPosition = {
            x = -6655,
            y = -10620
        },
        number = 7
    },

    [8] = {
        name = "Enraged Wildkin",
        description = "The battle-hardened leader of the Razormane tribe, whose immense strength and savage determination rally his warriors to victory.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n00W"),
        total = 42,
        hp = 550,
        bounty = 13,
        goldForWave = 225,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 0,
        armorBonus = 1,
        leakValue = 1,
        labelPosition = {
            x = -6950,
            y = -11520
        },
        boxPosition = {
            x = -6655,
            y = -11380
        },
        number = 8
    },

    [9] = {
        name = "Ice Revenant",
        description = "The battle-hardened leader of the Razormane tribe, whose immense strength and savage determination rally his warriors to victory.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n00X"),
        total = 43,
        hp = 600,
        bounty = 14,
        goldForWave = 250,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.ETHEREAL,
        defenseType = XTD.Data.DefenseTypes.SMALL,
        armorBase = 0,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = -5915,
            y = -10760
        },
        boxPosition = {
            x = -5635,
            y = -10620
        },
        number = 9
    },

    [10] = {
        name = "Flying Machine",
        description = "Swift mechanical aircraft that weave through the skies, testing any defense unprepared for threats from above.",
        category = XTD.Data.WaveCategories.AIR,
        unitType = FourCC("h004"),
        total = 44,
        hp = 650,
        bounty = 15,
        goldForWave = 275,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.METAL,
        defenseType = XTD.Data.DefenseTypes.SMALL,
        armorBase = 0,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = -5915,
            y = -11520
        },
        boxPosition = {
            x = -5635,
            y = -11380
        },
        number = 10
    },

    [11] = {
        name = "Voidwalker",
        description = "Manifestations of pure Void energy, these haunting entities drift relentlessly forward, shrugging off wounds that would fell ordinary creatures.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n00Y"),
        total = 40,
        hp = 700,
        bounty = 16,
        goldForWave = 300,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.ETHEREAL,
        defenseType = XTD.Data.DefenseTypes.MEDIUM,
        armorBase = 1,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = -4875,
            y = -10760
        },
        boxPosition = {
            x = -4610,
            y = -10620
        },
        number = 11
    },

    [12] = {
        name = "Ogre Lord",
        description = "Towering over friend and foe alike, these brutal warlords lead with overwhelming strength, crushing anything that dares oppose them.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n00Z"),
        total = 41,
        hp = 750,
        bounty = 17,
        goldForWave = 325,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 2,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = -4875,
            y = -11520
        },
        boxPosition = {
            x = -4610,
            y = -11380
        },
        number = 12
    },

    [13] = {
        name = "Dark Wizard",
        description = "Consumed by a thirst for forbidden knowledge, these sinister sorcerers wield shadow and arcane magic to devastate any who stand in their way.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n01O"),
        total = 41,
        hp = 800,
        bounty = 17,
        goldForWave = 340,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 2,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = -3870,
            y = -10760
        },
        boxPosition = {
            x = -3585,
            y = -10620
        },
        number = 13
    },

    [14] = {
        name = "Infernal Juggernaut",
        description = "Forged in the heart of the Twisting Nether, these colossal engines of destruction crush all resistance beneath their molten fury.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n010"),
        total = 42,
        hp = 850,
        bounty = 18,
        goldForWave = 350,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.WOOD,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 2,
        armorBonus = 1,
        leakValue = 1,
        labelPosition = {
            x = -3870,
            y = -11520
        },
        boxPosition = {
            x = -3585,
            y = -11380
        },
        number = 14
    },

    [15] = {
        name = "Harpy Windwitch",
        description = "Masters of wind and storm, these winged sorceresses descend from the skies to unleash nature's fury upon their enemies.",
        category = XTD.Data.WaveCategories.AIR,
        unitType = FourCC("n011"),
        total = 43,
        hp = 900,
        bounty = 19,
        goldForWave = 375,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.SMALL,
        armorBase = 2,
        armorBonus = 2,
        leakValue = 1,
        labelPosition = {
            x = -2835,
            y = -10760
        },
        boxPosition = {
            x = -2561,
            y = -10620
        },
        number = 15
    },

    [16] = {
        name = "Makrura Tidal Lord",
        description = "Ancient rulers of the deep, these heavily armored behemoths emerge with the tides to crush any who dare oppose them.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n012"),
        total = 40,
        hp = 950,
        bounty = 20,
        goldForWave = 400,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.WOOD,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 3,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = -2835,
            y = -11520
        },
        boxPosition = {
            x = -2561,
            y = -11380
        },
        number = 16
    },

    [17] = {
        name = "Nerubian Webspinner",
        description = "Masters of the web, these sinister Nerubians ensnare their prey before closing in with deadly precision.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n013"),
        total = 41,
        hp = 1000,
        bounty = 21,
        goldForWave = 425,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 2,
        armorBonus = 2,
        leakValue = 1,
        labelPosition = {
            x = -1795,
            y = -10760
        },
        boxPosition = {
            x = -1535,
            y = -10620
        },
        number = 17
    },

    [18] = {
        name = "Ancient Hydra",
        description = "An ancient apex predator whose many ravenous heads strike without mercy, leaving only devastation in its wake.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n014"),
        total = 42,
        hp = 1050,
        bounty = 21,
        goldForWave = 425,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.WOOD,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 2,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = -1795,
            y = -11520
        },
        boxPosition = {
            x = -1535,
            y = -11380
        },
        number = 18
    },

    [19] = {
        name = "Felguard",
        description = "Elite warriors of the Burning Legion, these towering demons march with unwavering discipline, crushing all who stand before them.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n015"),
        total = 43,
        hp = 1100,
        bounty = 22,
        goldForWave = 450,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.METAL,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 1,
        armorBonus = 2,
        leakValue = 1,
        labelPosition = {
            x = -795,
            y = -10760
        },
        boxPosition = {
            x = -510,
            y = -10620
        },
        number = 19
    },

    [20] = {
        name = "Infernal",
        description = "Descending from the sky as a blazing meteor, this colossal demon of living stone and fel fire exists for one purpose—to reduce everything in its path to ashes.",
        category = XTD.Data.WaveCategories.BOSS,
        unitType = FourCC("n016"),
        total = 1,
        hp = 55000,
        bounty = 150,
        goldForWave = 475,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.STONE,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 6,
        armorBonus = 3,
        leakValue = 10,
        labelPosition = {
            x = -795,
            y = -11520
        },
        boxPosition = {
            x = -510,
            y = -11380
        },
        number = 20
    },

    [21] = {
        name = "War Golem",
        description = "Forged from enchanted stone and ancient magic, these colossal constructs march forward with unstoppable determination, crushing all who stand before them.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n017"),
        total = 40,
        hp = 1150,
        bounty = 23,
        goldForWave = 500,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.METAL,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 2,
        armorBonus = 2,
        leakValue = 1,
        labelPosition = {
            x = 260,
            y = -10760
        },
        boxPosition = {
            x = 513,
            y = -10620
        },
        number = 21
    },

    [22] = {
        name = "Satyr Hellcaller",
        description = "Twisted by the Burning Legion's corruption, these sinister spellcasters rain hellfire upon any who dare challenge their dark masters.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n018"),
        total = 41,
        hp = 1200,
        bounty = 24,
        goldForWave = 525,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 3,
        armorBonus = 1,
        leakValue = 1,
        labelPosition = {
            x = 260,
            y = -11520
        },
        boxPosition = {
            x = 513,
            y = -11380
        },
        number = 22
    },

    [23] = {
        name = "Abomination",
        description = "Stitched together from countless fallen corpses, these grotesque monstrosities lumber forward with terrifying strength and an unending hunger for slaughter.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("u001"),
        total = 42,
        hp = 1250,
        bounty = 25,
        goldForWave = 550,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 4,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = 1265,
            y = -10760
        },
        boxPosition = {
            x = 1535,
            y = -10620
        },
        number = 23
    },

    [24] = {
        name = "Skeletal Orc Champion",
        description = "Raised from the bones of legendary orc warriors, these undead champions march with the same relentless fury that once made them heroes in life.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n019"),
        total = 43,
        hp = 1300,
        bounty = 26,
        goldForWave = 575,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.WOOD,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 2,
        armorBonus = 1,
        leakValue = 1,
        labelPosition = {
            x = 1265,
            y = -11520
        },
        boxPosition = {
            x = 1535,
            y = -11380
        },
        number = 24
    },

    [25] = {
        name = "Gargoyle",
        description = "Perched like lifeless statues until the moment they strike, these winged horrors descend from the skies with terrifying speed and deadly precision.",
        category = XTD.Data.WaveCategories.AIR,
        unitType = FourCC("u002"),
        total = 44,
        hp = 1350,
        bounty = 27,
        goldForWave = 600,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.STONE,
        defenseType = XTD.Data.DefenseTypes.UNARMORED,
        armorBase = 3,
        armorBonus = 2,
        leakValue = 1,
        labelPosition = {
            x = 2315,
            y = -10760
        },
        boxPosition = {
            x = 2558,
            y = -10620
        },
        number = 25
    },

    [26] = {
        name = "Unbroken Rager",
        description = "Forged by hardship and driven by unbreakable resolve, these fierce warriors charge into battle with unstoppable fury, crushing all who stand before them.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n01A"),
        total = 40,
        hp = 1400,
        bounty = 28,
        goldForWave = 625,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 3,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = 2315,
            y = -11520
        },
        boxPosition = {
            x = 2558,
            y = -11380
        },
        number = 26
    },

    [27] = {
        name = "Faceless One Deathbringer",
        description = "Ancient harbingers of the Old Gods, these eldritch horrors bring madness, despair, and inevitable death to all who stand before them.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n01B"),
        total = 41,
        hp = 1450,
        bounty = 29,
        goldForWave = 650,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 2,
        armorBonus = 1,
        leakValue = 1,
        labelPosition = {
            x = 3325,
            y = -10760
        },
        boxPosition = {
            x = 3582,
            y = -10620
        },
        number = 27
    },

    [28] = {
        name = "Queen of Suffering",
        description = "A sinister demoness who thrives on anguish, the Queen of Suffering turns every battle into a nightmare, feeding upon the pain of those who challenge her.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n01C"),
        total = 42,
        hp = 1500,
        bounty = 30,
        goldForWave = 675,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 4,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = 3325,
            y = -11520
        },
        boxPosition = {
            x = 3582,
            y = -11380
        },
        number = 28
    },

    [29] = {
        name = "Brood Mother",
        description = "The ancient matriarch of a monstrous brood, she fiercely defends her offspring while unleashing overwhelming fury upon any who dare invade her nest.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n01D"),
        total = 43,
        hp = 1550,
        bounty = 31,
        goldForWave = 700,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 4,
        armorBonus = 1,
        leakValue = 1,
        labelPosition = {
            x = 4405,
            y = -10760
        },
        boxPosition = {
            x = 4605,
            y = -10620
        },
        number = 29
    },

    [30] = {
        name = "Doom Guard",
        description = "A legendary champion of the Burning Legion, this towering demon has led the destruction of countless worlds, wielding immense fel power and crushing all who dare stand against him.",
        category = XTD.Data.WaveCategories.BOSS,
        unitType = FourCC("n01E"),
        total = 1,
        hp = 85000,
        bounty = 350,
        goldForWave = 725,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.STONE,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 8,
        armorBonus = 2,
        leakValue = 15,
        labelPosition = {
            x = 4405,
            y = -11520
        },
        boxPosition = {
            x = 4605,
            y = -11380
        },
        number = 30
    },

    [31] = {
        name = "Lazy Giant Peasant",
        description = "A legendary champion of the Burning Legion, this towering demon has led the destruction of countless worlds, wielding immense fel power and crushing all who dare stand against him.",
        category = XTD.Data.WaveCategories.BONUS,
        unitType = FourCC("h005"),
        total = 40,
        hp = 60000,
        bounty = 600,
        goldForWave = 750,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.FORTIFIED,
        armorBase = 10,
        armorBonus = 4,
        leakValue = 0,
        labelPosition = {
            x = 5395,
            y = -10760
        },
        boxPosition = {
            x = 5630,
            y = -10620
        },
        number = 31
    },

    [32] = {
        name = "Dire Mammoth",
        description = "Ancient titans of the frozen wilds, these colossal beasts crush all before them with unstoppable strength and thunderous resolve.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n01F"),
        total = 41,
        hp = 1600,
        bounty = 32,
        goldForWave = 725,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 5,
        armorBonus = 1,
        leakValue = 1,
        labelPosition = {
            x = 5395,
            y = -11520
        },
        boxPosition = {
            x = 5630,
            y = -11380
        },
        number = 32
    },

    [33] = {
        name = "Enraged Jungle Stalker",
        description = "Consumed by primal fury, these savage predators emerge from the jungle with lightning speed, relentlessly hunting anything that crosses their path.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n01G"),
        total = 42,
        hp = 1650,
        bounty = 33,
        goldForWave = 750,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 3,
        armorBonus = 3,
        leakValue = 1,
        labelPosition = {
            x = 6405,
            y = -10760
        },
        boxPosition = {
            x = 6655,
            y = -10620
        },
        number = 33
    },

    [34] = {
        name = "Sludge Monstrosity",
        description = "A towering mass of toxic sludge and dark corruption, this grotesque horror advances relentlessly, consuming all in its path.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n01H"),
        total = 43,
        hp = 1700,
        bounty = 34,
        goldForWave = 775,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 5,
        armorBonus = 0,
        leakValue = 1,
        labelPosition = {
            x = 6405,
            y = -11520
        },
        boxPosition = {
            x = 6655,
            y = -11380
        },
        number = 34
    },

    [35] = {
        name = "Red Dragon",
        description = "Ancient masters of the skies, these majestic dragons unleash torrents of searing flame upon any who dare challenge their dominion.",
        category = XTD.Data.WaveCategories.AIR,
        unitType = FourCC("n01I"),
        total = 44,
        hp = 1750,
        bounty = 35,
        goldForWave = 800,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 6,
        armorBonus = 1,
        leakValue = 1,
        labelPosition = {
            x = 7390,
            y = -10760
        },
        boxPosition = {
            x = 7680,
            y = -10620
        },
        number = 35
    },

    [36] = {
        name = "Penguin Parade",
        description = "A disciplined march of surprisingly determined penguins. They're adorable... right up until they waddle straight through your defenses.",
        category = XTD.Data.WaveCategories.BONUS,
        unitType = FourCC("n01J"),
        total = 40,
        hp = 75000,
        bounty = 800,
        goldForWave = 825,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.MEDIUM,
        armorBase = 15,
        armorBonus = 6,
        leakValue = 0,
        labelPosition = {
            x = 7390,
            y = -11520
        },
        boxPosition = {
            x = 7680,
            y = -11380
        },
        number = 36
    },

    [37] = {
        name = "Draenei Darkslayer",
        description = "Veterans of an endless war against the Burning Legion, these fearless champions wield unwavering resolve and deadly skill to cut down any foe before them.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n01K"),
        total = 41,
        hp = 1800,
        bounty = 36,
        goldForWave = 850,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.METAL,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 6,
        armorBonus = 1,
        leakValue = 1,
        labelPosition = {
            x = 8465,
            y = -10760
        },
        boxPosition = {
            x = 8705,
            y = -10620
        },
        number = 37
    },

    [38] = {
        name = "Naga Royal Guard",
        description = "Chosen to stand beside naga royalty, these elite warriors wield unmatched strength and unwavering discipline, cutting down any who threaten their kingdom.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n01L"),
        total = 42,
        hp = 1850,
        bounty = 37,
        goldForWave = 875,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 5,
        armorBonus = 3,
        leakValue = 1,
        labelPosition = {
            x = 8465,
            y = -11520
        },
        boxPosition = {
            x = 8705,
            y = -11380
        },
        number = 38
    },

    [39] = {
        name = "Magnataur Destroyer",
        description = "Colossal warriors of the frozen north, these unstoppable behemoths trample their foes beneath earth-shattering strength and relentless fury.",
        category = XTD.Data.WaveCategories.NORMAL,
        unitType = FourCC("n01M"),
        total = 43,
        hp = 1900,
        bounty = 38,
        goldForWave = 900,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.FLESH,
        defenseType = XTD.Data.DefenseTypes.LARGE,
        armorBase = 6,
        armorBonus = 4,
        leakValue = 1,
        labelPosition = {
            x = 9430,
            y = -10760
        },
        boxPosition = {
            x = 9725,
            y = -10620
        },
        number = 39
    },

    [40] = {
        name = "Kil'jaeden",
        description = "The Deceiver has come. As one of the Burning Legion's most powerful and cunning leaders, Kil'jaeden has orchestrated the fall of countless worlds. Should he prevail, Azeroth will become just another memory consumed by the Legion's endless crusade.",
        category = XTD.Data.WaveCategories.BOSS,
        unitType = FourCC("N01N"),
        total = 1,
        hp = 100000,
        bounty = 1000,
        goldForWave = 925,
        spawnDelay = 0.35,
        armorType = XTD.Data.ArmorTypes.ETHEREAL,
        defenseType = XTD.Data.DefenseTypes.HERO,
        armorBase = 12,
        armorBonus = 4,
        leakValue = 20,
        labelPosition = {
            x = 9430,
            y = -11520
        },
        boxPosition = {
            x = 9725,
            y = -11380
        },
        number = 40
    }
}