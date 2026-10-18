---
type: vendor-doc
source_repo: "https://github.com/FP-GAME-DEV-TEAM/LostWorldDotA"
source_path: "src/vscripts/Precache.ts"
ref: HEAD
---

//================================
// 资源预载
//================================

export function Precache(context: CScriptPrecacheContext) {
    PrecacheResource("particle", "particles/units/heroes/hero_meepo/meepo_earthbind_projectile_fx.vpcf", context);
    PrecacheResource("soundfile", "soundevents/game_sounds_heroes/game_sounds_meepo.vsndevts", context);

    // 寒冰法师
    PrecacheUnitByNameSync("npc_dota_hero_crystal_maiden", context);
    PrecacheResource("soundfile", "soundevents/game_sounds_heroes/game_sounds_crystalmaiden.vsndevts", context);
    PrecacheResource("soundfile", "soundevents/game_sounds_heroes/game_sounds_ancient_apparition.vsndevts", context);
    PrecacheResource("particle", "particles/units/heroes/hero_crystalmaiden_persona/cm_persona_ambient_crystals_coverup.vpcf", context);
    PrecacheResource("particle", "particles/units/heroes/hero_crystalmaiden_persona/cm_persona_nova_sphere_shake.vpcf", context);
    PrecacheResource("particle", "particles/generic_gameplay/generic_slowed_cold.vpcf", context);
    PrecacheResource("particle", "particles/units/heroes/hero_crystalmaiden/maiden_shard_frostbite.vpcf", context);

}