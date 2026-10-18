---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Types/BuffType.cs"
ref: HEAD
---

﻿public enum BuffType {
    None = 0,
    // Creeps
    Devotion1,
    Devotion2,
    Devotion3,
    Devotion4,
    Endurance1,
    Endurance2,
    Endurance3,
    Endurance4,
    LingeringVoid,
    Ancient1,
    Ancient2,
    Geomancy1,
    Geomancy2,
    NecroticTransfusion,
    Ethereal,
    HardenedSkin,
    EarthShield,
    ChaosEmpowerment,
    StaticOverload,
    WarStance,
    WarCry,
    // Towers
    TorrentSlow1,
    TorrentSlow2,
    CripplingDecaySlow,
    CripplingVulnerability,
    HurricaneStormParalysis,
    TemporalShift1,
    TemporalShift2,
    TemporalImplosion,
    VoidLashingDebuff1,
    VoidLashingDebuff2,
    VoidLashingBuff1,
    VoidLashingBuff2,
    HungeringVoidDebuff,
    HungeringVoidBuff,
    Corrupted1,
    Corrupted2,
    Plague,
    UnendingPlague,
    Ignite1,
    Ignite2,
    MeteoricVulnerability,
    RisingHeat1,
    RisingHeat2,
    VolcanicExposure, // debuff on target, reducing armor
    VolcanicOpportunity, // target below 25% health, +attackspeed / eruption chance = 100%
    ShatteredArmor1,
    ShatteredArmor2,
    Devastation1, // armor reduction
    Devastation2,
    NaturesForce,
    Germination1,
    Germination2,
    LethalPreparation,
    TurbulentWeather1,
    TurbulentWeather2,
    TurbulentWeather3,
    FrostboltSlow,
    IceblastSlow,
    ChainsOfIceSlow,
    FireboltStun,
    FireblastStun,
    PyroblastStun,
    ArcaneExposure, // arcane explosion -atk damage debuff
    BlindedByTheLight1,
    BlindedByTheLight2,
    TitanicallyBlind,
    FrostAttackSlow1,
    FrostAttackSlow2,
    FrostBlastSlow1,
    FrostBlastSlow2,
    ChillingDeathSlow,
    ChillingDeathAttackSpeedSlow,
    Frostbite,
    IceLanced1,
    IceLanced2,
    CrystallizedPenetration,
    // Discs
    EssenceOfNature1, // earth
    EssenceOfFrost1, // ice
    EssenceOfPower1, // lightning
    EssenceOfTheSea1, // water
    EssenceOfLight1, // holy
    EssenceOfBlight1, // unholy
    EssenceOfDarkness1, // void
    EssenceOfNature2, // earth
    EssenceOfFrost2, // ice
    EssenceOfPower2, // lightning
    EssenceOfTheSea2, // water
    EssenceOfLight2, // holy
    EssenceOfBlight2, // unholy
    EssenceOfDarkness2, // void
    EssenceOfPowerStun,
    EssenceOfFrostSlow1,
    EssenceOfFrostSlow2,
}