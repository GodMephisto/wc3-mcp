---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "core/regions.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Regions                                             |
    | =================================================== |
    | All the regions for use in XTD: Reforged.           |
    | =================================================== |
    +-----------------------------------------------------+
]]

XTD.Data.Regions = XTD.Data.Regions or {}

--[[
    The creep lanes.
]]
XTD.Data.Regions.Lanes = {
    NW = {
        Spawn = "NWSpawn",

        Split = {
            [1] = "NWP1Begin",
            [2] = "NWP2Begin"
        },

        Clone = {
            [1] = "NWP1Clone",
            [2] = "NWP2Clone"
        },

        Right = {
            [1] = { "NWP1Right1", "NWP1Right2" },
            [2] = { "NWP2Right1", "NWP2Right2" }
        },

        Left = {
            [1] = { "NWP1Left1", "NWP1Left2" },
            [2] = { "NWP2Left1", "NWP2Left2" }
        },

        End = {
            [1] = "NWP1End",
            [2] = "NWP2End"
        },

        LaneEnd = "NWEndLane",
        EndZone = "EndZone"
    },

    NE = {
        Spawn = "NESpawn",

        Split = {
            [1] = "NEP3Begin",
            [2] = "NEP4Begin"
        },

        Clone = {
            [1] = "NEP3Clone",
            [2] = "NEP4Clone"
        },

        Right = {
            [1] = { "NEP3Right1", "NEP3Right2" },
            [2] = { "NEP4Right1", "NEP4Right2" }
        },

        Left = {
            [1] = { "NEP3Left1", "NEP3Left2" },
            [2] = { "NEP4Left1", "NEP4Left2" }
        },

        End = {
            [1] = "NEP3End",
            [2] = "NEP4End"
        },

        LaneEnd = "NEEndLane",
        EndZone = "EndZone"
    },

    SE = {
        Spawn = "SESpawn",

        Split = {
            [1] = "SEP5Begin",
            [2] = "SEP6Begin"
        },

        Clone = {
            [1] = "SEP5Clone",
            [2] = "SEP6Clone"
        },

        Right = {
            [1] = { "SEP5Right1", "SEP5Right2" },
            [2] = { "SEP6Right1", "SEP6Right2" }
        },

        Left = {
            [1] = { "SEP5Left1", "SEP5Left2" },
            [2] = { "SEP6Left1", "SEP6Left2" }
        },

        End = {
            [1] = "SEP5End",
            [2] = "SEP6End"
        },

        LaneEnd = "SEEndLane",
        EndZone = "EndZone"
    },

    SW = {
        Spawn = "SWSpawn",

        Split = {
            [1] = "SWP7Begin",
            [2] = "SWP8Begin"
        },

        Clone = {
            [1] = "SWP7Clone",
            [2] = "SWP8Clone"
        },

        Right = {
            [1] = { "SWP7Right1", "SWP7Right2" },
            [2] = { "SWP8Right1", "SWP8Right2" }
        },

        Left = {
            [1] = { "SWP7Left1", "SWP7Left2" },
            [2] = { "SWP8Left1", "SWP8Left2" }
        },

        End = {
            [1] = "SWP7End",
            [2] = "SWP8End"
        },

        LaneEnd = "SWEndLane",
        EndZone = "EndZone"
    }
}

--[[
    The player build zones.
]]
XTD.Data.Regions.BuildZones = {
    [0] = {
        "P1BuildZone1",
        "P1BuildZone2",
        "P1BuildZone3",
        "P1BuildZone4",
        "P1BuildZone5",
        "P1BuildZone6",
        "P1BuildZone7",
        "P1BuildZone8",
        "P1BuildZone9",
        "P1BuildZone10",
        "P1BuildZone11",
        "P1BuildZone12",
        "P1BuildZone13",
        "P1BuildZone14",
        "P1BuildZone15",
        "P1BuildZone16",
        "P1BuildZone17",
        "P1BuildZone18",
        "P1BuildZone19",
        "P1BuildZone20",
        "P1BuildZone21",
        "P1BuildZone22",
        "P1BuildZone23",
        "P1BuildZone24",
        "P1BuildZone25"
    },

    [1] = {
        "P2BuildZone1",
        "P2BuildZone2",
        "P2BuildZone3",
        "P2BuildZone4",
        "P2BuildZone5",
        "P2BuildZone6",
        "P2BuildZone7",
        "P2BuildZone8",
        "P2BuildZone9",
        "P2BuildZone10",
        "P2BuildZone11",
        "P2BuildZone12",
        "P2BuildZone13",
        "P2BuildZone14",
        "P2BuildZone15",
        "P2BuildZone16",
        "P2BuildZone17",
        "P2BuildZone18",
        "P2BuildZone19",
        "P2BuildZone20",
        "P2BuildZone21",
        "P2BuildZone22",
        "P2BuildZone23",
        "P2BuildZone24",
        "P2BuildZone25"
    },

    [2] = {
        "P3BuildZone1",
        "P3BuildZone2",
        "P3BuildZone3",
        "P3BuildZone4",
        "P3BuildZone5",
        "P3BuildZone6",
        "P3BuildZone7",
        "P3BuildZone8",
        "P3BuildZone9",
        "P3BuildZone10",
        "P3BuildZone11",
        "P3BuildZone12",
        "P3BuildZone13",
        "P3BuildZone14",
        "P3BuildZone15",
        "P3BuildZone16",
        "P3BuildZone17",
        "P3BuildZone18",
        "P3BuildZone19",
        "P3BuildZone20",
        "P3BuildZone21",
        "P3BuildZone22",
        "P3BuildZone23",
        "P3BuildZone24",
        "P3BuildZone25"
    },

    [3] = {
        "P4BuildZone1",
        "P4BuildZone2",
        "P4BuildZone3",
        "P4BuildZone4",
        "P4BuildZone5",
        "P4BuildZone6",
        "P4BuildZone7",
        "P4BuildZone8",
        "P4BuildZone9",
        "P4BuildZone10",
        "P4BuildZone11",
        "P4BuildZone12",
        "P4BuildZone13",
        "P4BuildZone14",
        "P4BuildZone15",
        "P4BuildZone16",
        "P4BuildZone17",
        "P4BuildZone18",
        "P4BuildZone19",
        "P4BuildZone20",
        "P4BuildZone21",
        "P4BuildZone22",
        "P4BuildZone23",
        "P4BuildZone24",
        "P4BuildZone25"
    },

    [4] = {
        "P5BuildZone1",
        "P5BuildZone2",
        "P5BuildZone3",
        "P5BuildZone4",
        "P5BuildZone5",
        "P5BuildZone6",
        "P5BuildZone7",
        "P5BuildZone8",
        "P5BuildZone9",
        "P5BuildZone10",
        "P5BuildZone11",
        "P5BuildZone12",
        "P5BuildZone13",
        "P5BuildZone14",
        "P5BuildZone15",
        "P5BuildZone16",
        "P5BuildZone17",
        "P5BuildZone18",
        "P5BuildZone19",
        "P5BuildZone20",
        "P5BuildZone21",
        "P5BuildZone22",
        "P5BuildZone23",
        "P5BuildZone24",
        "P5BuildZone25"
    },

    [5] = {
        "P6BuildZone1",
        "P6BuildZone2",
        "P6BuildZone3",
        "P6BuildZone4",
        "P6BuildZone5",
        "P6BuildZone6",
        "P6BuildZone7",
        "P6BuildZone8",
        "P6BuildZone9",
        "P6BuildZone10",
        "P6BuildZone11",
        "P6BuildZone12",
        "P6BuildZone13",
        "P6BuildZone14",
        "P6BuildZone15",
        "P6BuildZone16",
        "P6BuildZone17",
        "P6BuildZone18",
        "P6BuildZone19",
        "P6BuildZone20",
        "P6BuildZone21",
        "P6BuildZone22",
        "P6BuildZone23",
        "P6BuildZone24",
        "P6BuildZone25"
    },

    [6] = {
        "P7BuildZone1",
        "P7BuildZone2",
        "P7BuildZone3",
        "P7BuildZone4",
        "P7BuildZone5",
        "P7BuildZone6",
        "P7BuildZone7",
        "P7BuildZone8",
        "P7BuildZone9",
        "P7BuildZone10",
        "P7BuildZone11",
        "P7BuildZone12",
        "P7BuildZone13",
        "P7BuildZone14",
        "P7BuildZone15",
        "P7BuildZone16",
        "P7BuildZone17",
        "P7BuildZone18",
        "P7BuildZone19",
        "P7BuildZone20",
        "P7BuildZone21",
        "P7BuildZone22",
        "P7BuildZone23",
        "P7BuildZone24",
        "P7BuildZone25"
    },

    [7] = {
        "P8BuildZone1",
        "P8BuildZone2",
        "P8BuildZone3",
        "P8BuildZone4",
        "P8BuildZone5",
        "P8BuildZone6",
        "P8BuildZone7",
        "P8BuildZone8",
        "P8BuildZone9",
        "P8BuildZone10",
        "P8BuildZone11",
        "P8BuildZone12",
        "P8BuildZone13",
        "P8BuildZone14",
        "P8BuildZone15",
        "P8BuildZone16",
        "P8BuildZone17",
        "P8BuildZone18",
        "P8BuildZone19",
        "P8BuildZone20",
        "P8BuildZone21",
        "P8BuildZone22",
        "P8BuildZone23",
        "P8BuildZone24",
        "P8BuildZone25"
    }
}

--[[
    All the creep boxes.
]]
XTD.Data.Regions.CreepBoxes = {
    "CreepBoxWave1",
    "CreepBoxWave2",
    "CreepBoxWave3",
    "CreepBoxWave4",
    "CreepBoxWave5",
    "CreepBoxWave6",
    "CreepBoxWave7",
    "CreepBoxWave8",
    "CreepBoxWave9",
    "CreepBoxWave10",
    "CreepBoxWave11",
    "CreepBoxWave12",
    "CreepBoxWave13",
    "CreepBoxWave14",
    "CreepBoxWave15",
    "CreepBoxWave16",
    "CreepBoxWave17",
    "CreepBoxWave18",
    "CreepBoxWave19",
    "CreepBoxWave20",
    "CreepBoxWave21",
    "CreepBoxWave22",
    "CreepBoxWave23",
    "CreepBoxWave24",
    "CreepBoxWave25",
    "CreepBoxWave26",
    "CreepBoxWave27",
    "CreepBoxWave28",
    "CreepBoxWave29",
    "CreepBoxWave30",
    "CreepBoxWave31",
    "CreepBoxWave32",
    "CreepBoxWave33",
    "CreepBoxWave34",
    "CreepBoxWave35",
    "CreepBoxWave36",
    "CreepBoxWave37",
    "CreepBoxWave38",
    "CreepBoxWave39",
    "CreepBoxWave40"
}