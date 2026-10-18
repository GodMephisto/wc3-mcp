---
type: vendor-doc
source_repo: "https://github.com/karnaxus/xtdreforged"
source_path: "constants/index.lua"
ref: HEAD
---

--[[
    +-----------------------------------------------------+
    | =================================================== |
    | X TOWER DEFENSE: REFORGED                           |
    | =================================================== |
    | @author Karnaxus#11289                              |
    | =================================================== |
    | Constants                                           |
    | =================================================== |
    | All constants for XTD.                              |
    | =================================================== |
    +-----------------------------------------------------+
]]


--[[
    All constants.
]]
XTD.Constants = {
    TOWER_BUILDER = FourCC("u000"),
    CREEPS = Player(8),
    MAX_WAVES = 40,

    SPELL_TOWERS = {
        COLD = {
            FROSTGUARD_TOWER = FourCC("n00I"),
            ICEWATCH_TOWER = FourCC("n00K"),
            WINTERHOLD_TOWER = FourCC("n00S"),
            BLIZZARDSPIRE_TOWER = FourCC("n00D")
        },

        INFERNO = {
            EMBER_TOWER = FourCC("n00G"),
            FLAMEGUARD_TOWER = FourCC("n00H"),
            INFERNO_TOWER = FourCC("n00L"),
            HELLSPIRE_TOWER = FourCC("n00J")
        },

        ARCANE = {
            RUNIC_TOWER = FourCC("n00A"),
            ARCANE_TOWER = FourCC("n008"),
            SPELLWEAVER_TOWER = FourCC("n00B"),
            ASTRAL_SPIRE = FourCC("n009")
        },

        STORM = {
            SPARK_TOWER = FourCC("n004"),
            STORMWATCH_TOWER = FourCC("n005"),
            TEMPEST_TOWER = FourCC("n006"),
            THUNDERSPIRE_TOWER = FourCC("n007")
        },

        SHADOW = {
            SHADE_TOWER = FourCC("n00N"),
            DREADWATCH_TOWER = FourCC("n00F"),
            SHADOWKEEP_TOWER = FourCC("n00P"),
            DOOMSPIRE_TOWER = FourCC("n00E")
        },

        CANNON = {
            BOMBARD_TOWER = FourCC("h000"),
            WAR_CANNON = FourCC("h003"),
            SIEGE_CANNON = FourCC("h002"),
            CATACLYSM_TOWER = FourCC("h001")
        }
    }
}
