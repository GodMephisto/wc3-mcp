---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Constants/TraitConstants.cs"
ref: HEAD
---

﻿using System.Collections.Generic;

public enum TraitType {
    // Creeps
    FastProducing = 0,
    DeathPact1,
    UnholySacrifice1,
    Armored1,
    DevotionAura1,
    FelBlood,
    EnduranceAura1,
    Flying,
    Attacker,
    LingeringVoid,
    Boss,
    BasicSpellResistance,
    BasicSpellResistanceWithNoMentionOfImmunity,
    Armored2,
    DeathPact2,
    AncientAura1,
    Dash1,
    ChaoticVoid,
    EnduranceAura2,
    Dash2,
    DevotionAura2,
    UnholySacrifice2,
    Bombardment,
    GeomancyAura1,
    NecroticTransfusion,
    EnduranceAura3,
    Dash3,
    Skittering,
    DeathPact3,
    Armored3,
    EtherealAura,
    MajorSpellResistance,
    UnholySacrifice3,
    HardenedSkin,
    AncientAura2,
    DevotionAura3,
    EarthShieldProvider,
    GeomancyAura2,
    Dash4,
    ChaosPortal,
    ChaosEmpowermentAura,
    StoneskinFortitude,
    GoblinEngineering,
    ReactiveArmor,
    EngineOverload,
    Hypothermia,
    WarStance,
    EnduranceAura4,
    DevotionAura4,
    UnholySacrifice4,
    VolatileDeath,
    LegendarySpellResistance,
    
    // Towers
    AirAttackOnly,
    GroundAttackOnly,
    
    // Arcane
    Arcanize1,
    Arcanize2,
    Spellcaster1,
    Spellcaster2,
    KirinTorMastery,
    VolatileArcane1,
    VolatileArcane2,
    OverwhelmingArcane,
    
    // Fire
    Ignite1,
    Ignite2,
    OverwhelmingImpact1,
    OverwhelmingImpact2,
    VoidFlare,
    RisingHeat1,
    RisingHeat2,
    VolcanicEruption,
    
    // Ice
    FrostAttack1,
    FrostAttack2,
    FrostBlast1,
    FrostBlast2,
    ChillingDeath,
    IceLance1,
    IceLance2,
    CrystallizedLight,
    
    // Earth
    ShatterArmor1,
    ShatterArmor2,
    DevastatingAttack1,
    DevastatingAttack2,
    NaturesGuidance,
    Germination1,
    Germination2,
    LethalPreparation,
    
    // Lightning
    Overcharge1,
    Overcharge2,
    FocusedLightning1,
    FocusedLightning2,
    Annihilation,
    TurbulentWeather1,
    TurbulentWeather2,
    TurbulentWeather3,
    StaticCharge1,
    StaticCharge2,
    Radiance,
    
    
    // Water
    CrushingWave1,
    CrushingWave2,
    PressuringWater1,
    PressuringWater2,
    HurricaneStorm,
    Torrent1,
    Torrent2,
    CripplingDecay,
    
    // Holy
    BurstingLight1,
    BurstingLight2,
    LightBurst1,
    LightBurst2,
    DivineSpores,
    BlindingLight1,
    BlindingLight2,
    TitanDefenseMechanism,
    
    // Unholy
    Corruption1,
    Corruption2,
    RapidInfection1,
    RapidInfection2,
    Pestilence,
    UnholyMiasma1,
    UnholyMiasma2,
    Hellfire,
    
    // Void
    VoidGrowth1,
    VoidGrowth2,
    TemporalRift1,
    TemporalRift2,
    ImplosionRift,
    VoidLashing1,
    VoidLashing2,
    HungeringVoid,
    
    // Discs
    Inactive,
    ArcaneReaction1, // arcane
    ExplosiveReaction1, // fire
    EssenceOfNatureAura1, // earth
    EssenceOfFrostAura1, // ice
    EssenceOfPowerAura1, // lightning
    EssenceOfTheSeaAura1, // water
    EssenceOfLightAura1, // holy
    EssenceOfBlightAura1, // unholy
    EssenceOfDarknessAura1, // void
    ArcaneReaction2, // arcane
    ExplosiveReaction2, // fire
    EssenceOfNatureAura2, // earth
    EssenceOfFrostAura2, // ice
    EssenceOfPowerAura2, // lightning
    EssenceOfTheSeaAura2, // water
    EssenceOfLightAura2, // holy
    EssenceOfBlightAura2, // unholy
    EssenceOfDarknessAura2, // void
    
    Placeholder, // temp
}

public static class TraitConstants {
    public const int FastProducingInitialStockAmount = 3;
    public const int FastProducingRestockInterval = 3;

    public const double DevotionAura1Range = 700;
    public const double DevotionAura2Range = 700;
    public const double DevotionAura3Range = 860;
    public const double DevotionAura4Range = 860;
    public const int DevotionAura1ArmorDiff = 1;
    public const int DevotionAura2ArmorDiff = 3;
    public const int DevotionAura3ArmorDiff = 5;
    public const int DevotionAura4ArmorDiff = 7;

    public const double EnduranceAura1Range = 700;
    public const double EnduranceAura2Range = 700;
    public const double EnduranceAura3Range = 860;
    public const double EnduranceAura4Range = 860;
    public const double EnduranceAura1SpeedMultiplier = 1.1;
    public const double EnduranceAura2SpeedMultiplier = 1.15;
    public const double EnduranceAura3SpeedMultiplier = 1.2;
    public const double EnduranceAura4SpeedMultiplier = 1.225;

    public const float Armored1PhysicalSplashDamageTakenMultiplier = 0.7f;
    public const float Armored2PhysicalSplashDamageTakenMultiplier = 0.6f;
    public const float Armored3PhysicalSplashDamageTakenMultiplier = 0.5f;

    public const float BasicSpellResistanceHarmfulSpellEffectsDurationMultiplier = 0.5f;
    public const float MajorSpellResistanceHarmfulSpellEffectsDurationMultiplier = 0.5f;

    public const float UnholySacrifice1Range = 155;
    public const float UnholySacrifice2Range = 155;
    public const float UnholySacrifice3Range = 155;
    public const float UnholySacrifice4Range = 390;
    public const int UnholySacrifice1HealAmount = 3;
    public const int UnholySacrifice2HealAmount = 330;
    public const int UnholySacrifice3HealAmount = 5250;
    public const int UnholySacrifice4HealAmount = 10385;

    public const float DeathPact1ReviveDelay = 1.5f;
    public const float DeathPact2ReviveDelay = 1.5f;
    public const float DeathPact3ReviveDelay = 1.5f;
    public const float DeathPact1ReviveHealthMultiplier = 0.25f;
    public const float DeathPact2ReviveHealthMultiplier = 0.50f;
    public const float DeathPact3ReviveHealthMultiplier = 0.75f;

    public const float LingeringVoidDuration = 8;
    public const float LingeringVoidAttackSpeedReductionPerStack = 0.15f;
    
    public const float AncientAura1Range = 390;
    public const float Ancient1NegativeMovementEffectivenessMultiplier = 0.5f;
    public const float AncientAura2Range = 550;
    public const float Ancient2NegativeMovementEffectivenessMultiplier = 0.5f;
    
    public const int ChaoticVoidManaPerDamageInstance = 1;
    public const float ChaoticVoidHealPercentageOfMaxHealth = 0.04f;

    public const float BombardmentRange = 313;
    public const float BombardmentRocketInterval = 4f;
    public const int BombardmentRocketDamage = 40;

    public const float GeomancyAura1Range = 390;
    public const float Geomancy1ArmorReductionEffectivenessMultiplier = 0.5f;
    public const float GeomancyAura2Range = 550;
    public const float Geomancy2ArmorReductionEffectivenessMultiplier = 0.5f;

    public const float NecroticTransfusionRange = 390;
    public const float NecroticTransfusionDuration = 15;

    public const float EtherealAuraRange = 390;
    public const float EtherealAuraStackApplicationInterval = 7;
    public const int EtherealArmorPerStack = 2;
    public const int EtherealMaxStacks = 10;

    public const int HardenedSkinStartingStacks = 75;
    public const int HardenedSkinArmorPerStack = 1;
    public const int HardenedSkinMinimumDamageToRemoveStack = 75;

    public const float EarthShieldProviderRange = 313;
    public const float EarthShieldProviderCooldown = 14;
    public const float EarthShieldDuration = 15;
    public const float EarthShieldHealInterval = 5;
    public const float EarthShieldHealPercentageOfMaxHealth = .015f;

    public const float ChaosPortalRange = 390;
    public static readonly float[] ChaosPortalCastIntervals = {
        3f,
        4f,
        15f
    };
    
    public const float ChaosPortalFirstCastTimer = 3f;
    public const float ChaosPortalSecondCastTimer = 7f;
    public const float ChaosPortalRecurringCastInterval = 15f;
    
    public const float ChaosEmpowermentAuraRange = 860;
    public const float ChaosEmpowermentManaGainPerSecond = 0.2f;
    public const int ChaosEmpowermentSpellResist = 3;
    
    public const float GoblinEngineeringMinBaseMovementSpeed = 0.75f;
    public const float GoblinEngineeringHarmfulEffectsDurationMultiplier = 0.5f;

    public const int ReactiveArmorFirstThreshold = 800;
    public const float ReactiveArmorFirstThresholdMultiplier = 0.25f;
    public const int ReactiveArmorSecondThreshold = 2500;
    public const float ReactiveArmorSecondThresholdMultiplier = 0.05f;

    public static readonly float[] EngineOverloadHealthPercentageTriggers = {0.2f, 0.6f};
    public const float EngineOverloadAttackSpeedMultiplier = 0.65f;
    public const float EngineOverloadDuration = 2f;
    public const float EngineOverloadRange = 235f;
    
    public const float HypothermiaSlowEffectEffectivenessMultiplier = 1.5f;
    public const float HypothermiaMovementSpeedBasedDamageTakenCap = 2f;

    public const float WarStanceActivationHealthRatio = .2f;
    public const int WarStanceArmorBonus = 15;
    public const float WarCryAuraRange = 1560;
    public const float WarCryDamageDealtMultiplier = 1.25f;

    public const int VolatileDeathMaxDamage = 1000;
    public const float VolatileDeathRange = 300f;

    // Arcane traits
    public const float Arcanize1MaximumManaDamageMultiplier = 1.75f;
    public const float Arcanize2MaximumManaDamageMultiplier = 3f;

    public const int VolatileArcane1MaxBounces = 3;
    public const float VolatileArcane1BounceRange = 100;
    public const float VolatileArcane1AttackSpeedDropoff = 0.0033f;
    public const float VolatileArcane1MaxAttackSpeedDecrease = 0.33f;
    public const float VolatileArcane1SpellDamagePercentage = .33f;
    public const float VolatileArcane1ManaLossPerSecond = 2.5f;
    public const int VolatileArcane2MaxBounces = 4;
    public const float VolatileArcane2BounceRange = 100;
    public const float VolatileArcane2AttackSpeedDropoff = 0.0033f;
    public const float VolatileArcane2MaxAttackSpeedDecrease = 0.33f;
    public const float VolatileArcane2SpellDamagePercentage = .33f;
    public const float VolatileArcane2ManaLossPerSecond = 2.5f;

    public const int OverwhelmingArcaneMaxBounces = 4;
    public const float OverwhelmingArcaneManaPerTargetHit = 2.5f;
    public const float OverwhelmingArcaneDamageModifierPerMana = .021f;
    public const float OverwhelmingArcaneAdditionalSpellDamageHealthMinimum = .70f;
    public const float OverwhelmingArcaneAdditionalSpellDamagePercentage = .35f;
    public const float OverwhelmingArcaneHighHealthAdditionalManaGain = .5f;
    public const float OverwhelmingArcaneManaLossPerSecond = 10f;
    public const float OverwhelmingArcaneBounceRange = 100;

    public const int Spellcaster1FrostboltDamage = 48;
    public const int Spellcaster1FrostboltTargetMaximum = 5;
    public const float Spellcaster1FrostboltMovementSpeedMultiplier = .7f;
    public const float Spellcaster1FrostboltDebuffDuration = 3f;
    public const int Spellcaster1FireboltDamage = 336;
    public const float Spellcaster1FireboltStunDuration = 0.7f;
    public const int Spellcaster1ManaRegenerationPerSecond = 10;
    public const int Spellcaster2IceblastDamage = 198;
    public const int Spellcaster2IceblastTargetMaximum = 5;
    public const float Spellcaster2IceblastMovementSpeedMultiplier = .6f;
    public const float Spellcaster2IceblastAttackSpeedMultiplier = 0.75f;
    public const float Spellcaster2IceblastDebuffDuration = 3f;
    public const int Spellcaster2FireblastDamage = 1372;
    public const float Spellcaster2FireblastStunDuration = 0.8f;
    public const int Spellcaster2ManaRegenerationPerSecond = 10;

    public const int KirinTorMasteryChainsOfIceDamage = 1620;
    public const int KirinTorMasteryChainsOfIceTargetMaximum = 5;
    public const float KirinTorMasteryChainsOfIceMovementSpeedMultiplier = 0.5f;
    public const float KirinTorMasteryChainsOfIceAttackSpeedMultiplier = 0.65f;
    public const float KirinTorMasteryChainsOfIceDebuffDuration = 3f;
    public const int KirinTorMasteryPyroblastDamage = 9780;
    public const float KirinTorMasteryPyroblastStunDuration = 1f;
    public const int KirinTorMasteryArcaneExplosionDamage = 1980;
    public const float KirinTorMasteryArcaneExplosionRadius = 155;
    public const float KirinTorMasteryArcaneExplosionAttackDamageMultiplier = 0.9f;
    public const float KirinTorMasteryArcaneExplosionDebuffDuration = 3f;
    public const int KirinTorMasteryManaRegenerationPerSecond = 10;
    
    // Fire traits
    public const float Ignite1Interval = 2.1f;
    public const float Ignite1Range = 313;
    public const int Ignite1DamagePerSecond = 5;
    public const float Ignite1Duration = 5f;
    public const float Ignite2Interval = 2.1f;
    public const float Ignite2Range = 313;
    public const int Ignite2DamagePerSecond = 13;
    public const float Ignite2Duration = 5f;

    public const int OverwhelmingImpact1MeteorBurnTicks = 2;
    public const int OverwhelmingImpact1BurnDamagePerSecond = 50;
    public const int OverwhelmingImpact2MeteorBurnTicks = 2;
    public const int OverwhelmingImpact2BurnDamagePerSecond = 130;

    public const int VoidFlareMeteorBurnTicks = 3;
    public const int VoidFlareMaxDamagePerSecond = 800;
    public const int VoidFlareMinDamagePerSecond = 400; // TODO: Check with lolreported
    public const float VoidFlareManaRegenPerSecondDiff = 5;
    public const float MeteoricGroundBurnRadius = 313;
    public const int MeteoricVulnerabilitySpellResistReduction = 3;
    public const float MeteoricVulnerabilityDuration = 10f;
    
    // Each attack applies RisingHeat debuff
    public const float RisingHeat1AttackSpeedIncreasePerStack = 0.125f;
    public const int RisingHeat1StackResetThreshold = 12;
    public const float RisingHeat2AttackSpeedIncreasePerStack = 0.125f;
    public const int RisingHeat2StackResetThreshold = 16;

    public const float VolcanicEruptionProcChance = 0.5f;
    public const float VolcanicEruptionDamageMultiplier = 2.8f;
    public const float VolcanicEruptionArmorReductionMultiplier = .25f;
    public const float VolcanicEruptionMaxArmorReduction = 1.5f;
    public const float VolcanicEruptionGuaranteedProcHealthThreshold = .25f;
    public const float VolcanicEruptionLowHealthAttackSpeedMultiplier = 1.5f;

    // Ice traits
    public const float FrostAttackChill1MovementSpeedReductionMultiplierPerStack = 0.0325f;
    public const int FrostAttackChill1MaxStacks = 9;
    public const float FrostAttackChill2MovementSpeedReductionMultiplierPerStack = 0.0375f;
    public const int FrostAttackChill2MaxStacks = 8;

    public const float FrostBlastChill1MovementSpeedReductionMultiplierPerStack = 0.05f;
    public const int FrostBlastChill1MaxStacks = 8;
    public const float FrostBlastChill2MovementSpeedReductionMultiplierPerStack = 0.055f;
    public const int FrostBlastChill2MaxStacks = 8;

    public const float ChillingDeathRadius = 313;
    public const float ChillingDeathAttackSpeedMultiplier = 0.7f;
    public const float ChillingDeathMovementSpeedReductionMultiplierPerStack = 0.07f;
    public const int ChillingDeathMaxStacks = 8;
    public const float FrostbiteSlowEffectDurationMultiplier = 3f;
    public const float FrostbiteHealingReceivedMultiplier = .66f;
    public const float FrostbiteDuration = 9f;

    public const float IceLance1Range = 545;
    public const float IceLance1ArmorReductionPerHit = 0.1f;
    public const float IceLance2Range = 545;
    public const float IceLance2ArmorReductionPerHit = 0.2f;

    public const float CrystallizedLightRange = 625;
    public const float CrystallizedLightArmorReductionPerHit = 0.33f;
    public const float CrystallizedLightFirstTargetDamageMultiplier = 1.9f;
    public const float CrystallizedLightPerTargetDamageMultiplierDropoff = 0.06f;
    public const float CrystallizedLightMinimumDamageMultiplier = 1f;
    
    // Earth traits
    public const int ShatterArmor1ArmorReduction = 1;
    public const float ShatterArmor1Duration = 5f;
    public const int ShatterArmor2ArmorReduction = 2;
    public const float ShatterArmor2Duration = 5f;

    public const float DevastatingAttack1ArmorReductionPerHit = 0.2f;
    public const float DevastatingAttack2ArmorReductionPerHit = 0.35f;

    public const float NaturesGuidanceSelfHealByDamageDealtMultiplier = 0.025f;
    public const float NaturesGuidanceArmorReductionPerHit = 0.75f;

    public const float Germinate1MinIdleTimeInSeconds = 1f;
    public const float Germination1IdleStackIntervalInSeconds = 0.5f;
    public const float Germination1AdditionalDamageDealtMultiplierPerStack = .15f;
    public const int Germination1MaxStacks = 5;
    public const int Germination1AttackCount = 5;
    public const float Germinate2MinIdleTimeInSeconds = 1f;
    public const float Germination2IdleStackIntervalInSeconds = 0.5f;
    public const float Germination2AdditionalDamageDealtMultiplierPerStack = .2f;
    public const int Germination2MaxStacks = 5;
    public const int Germination2AttackCount = 5;

    public const float OpportunisticCriticalStrikeDamageMultiplier = 2f;
    public const float OpportunisticMinIdleTimeInSeconds = 1f;
    public const float LethalPreparationIdleStackIntervalInSeconds = 0.5f;
    public const float LethalPreparationAdditionalDamageDealtMultiplierPerStack = .3f;
    public const int LethalPreparationMaxStacks = 5;
    public const int LethalPreparationAttackCount = 5;
    
    // Holy traits
    public const int BurstingLight1AdditionalTargets = 3;
    public const int BurstingLight2AdditionalTargets = 4;

    public const int BlindingLight1AdditionalTargets = 6;
    public const int BlindedByTheLight1SpellResistDiff = -3;
    public const float BlindedByTheLight1DamageDealtMultiplier = 0.9f;
    public const float BlindedByTheLight1Duration = 3f;
    public const int BlindingLight2AdditionalTargets = 8;
    public const int BlindedByTheLight2SpellResistDiff = -3;
    public const float BlindedByTheLight2DamageDealtMultiplier = 0.85f;
    public const float BlindedByTheLight2Duration = 3f;

    public const int TitanDefenseMechanismAdditionalTargets = 10;
    public const float TitanDefenseMechanismTitanicallyBlindAuraRadius = 390;
    public const float TitanicallyBlindDamageDealtMultiplier = 0.8f;
    public const int TitanicallyBlindSpellResistDiff = -5;
    public const float TitanicallyBlindMovementSpeedMultiplier = 0.85f;

    public const int LightBurst1ManaRegenPerSecond = 10;
    public const float LightBurst1DistancePerManaConsumed = 5;
    public const int LightBurst2ManaRegenPerSecond = 10;
    public const float LightBurst2DistancePerManaConsumed = 6.25f;

    public const int DivineSporesManaRegenPerSecond = 10;
    public const float DivineSporesDistancePerManaConsumed = 8.6f;
    public const float DivineSporesHealRadius = 235;
    public const float DivineSporesHealAmountByDamageDealtMultiplier = 0.1f;

    // Lightning traits
    public const int Overcharge1DamagePerTenPercentCurrentHealth = 1;
    public const int Overcharge2DamagePerTenPercentCurrentHealth = 5;

    public const double FocusedLightning1DamageIncreasePerHit = .5;
    public const float TurbulentWeatherAura1Radius = 390;
    public const float TurbulentWeather1MovementSpeedMultiplier = 0.88f;
    public const double FocusedLightning2DamageIncreasePerHit = .75;
    public const float TurbulentWeatherAura2Radius = 390;
    public const float TurbulentWeather2MovementSpeedMultiplier = 0.82f;

    public const double AnnihilationDamageIncreasePerHit = 1;
    public const int AnnihilationAdditionalTargetsCount = 2;
    public const float AnnihilationAdditionalTargetsRadius = 235;
    public const float AnnihilationChainedTargetDamageDropoffPerHit = 0.25f;
    public const float TurbulentWeatherAura3Radius = 390;
    public const float TurbulentWeather3MovementSpeedMultiplier = 0.75f;

    public const float StaticCharge1AdditionalDamageTargetHealthMultiplier = 0.01f;
    public const int StaticCharge1ManaPerAttack = 3;
    public const int StaticCharge1LightningStrikeManaThreshold = 30;
    public const float StaticCharge1LightningStrikeRadius = 235;
    public const int StaticCharge1LightningStrikeDamage = 1000;
    public const float StaticCharge2AdditionalDamageTargetHealthMultiplier = 0.0133f;
    public const int StaticCharge2ManaPerAttack = 3;
    public const int StaticCharge2LightningStrikeManaThreshold = 30;
    public const float StaticCharge2LightningStrikeRadius = 235;
    public const int StaticCharge2LightningStrikeDamage = 3333;

    public const float RadianceAdditionalDamageTargetHealthMultiplier = 0.0133f;
    public const int RadianceManaPerAttack = 5;
    public const int RadianceMinManaForAbilityThreshold = 50;
    public const float RadianceLightningStrikeRadius = 235;
    public const int RadianceLightningStrikeDamage = 12500;
    public const float RadianceAbilityCurrentHealthReduction = 0.005f;
    public const float RadianceAbilityRadius = 156;
    
    // Unholy traits
    public const float FadingCorruption1Duration = 3f;
    public const float FadingCorruption1Radius = 125;
    public const int FadingCorruption1Damage = 28;
    public const float FadingCorruption2Duration = 3f;
    public const float FadingCorruption2Radius = 125;
    public const int FadingCorruption2Damage = 76;

    public const int UnholyMiasma1ManaPerAttack = 13;
    public const float UnholyMiasma1ManaLossPerSecond = 5;
    public const int UnholyMiasma2ManaPerAttack = 20;
    public const float UnholyMiasma2ManaLossPerSecond = 7.5f;
    
    public const int HellfireManaPerAttack = 20;
    public const float HellfireManaLossPerSecond = 7.5f;
    public const float HellfireMaxSplashOfBaseMultiplier = 2f;
    public const float HellfireInstantKillCurrentHealthMaxThreshold = 0.05f;
    public const float HellfireInstantKillCurrentHealthMinThreshold = 0.005f;

    public const int RapidInfection1AdditionalTargets = 1;
    public const float RapidInfection1ManaRegenPerSecond = 2.5f;
    public const int RapidInfection2AdditionalTargets = 2;
    public const float RapidInfection2ManaRegenPerSecond = 5f;
    public const int PestilenceAdditionalTargets = 2;
    public const float PestilenceManaRegenPerSecond = 7.5f;
    public const float PlagueDuration = 3f;
    public const int PlagueMaxStacks = 100;
    public const float PlagueAdditionalDamageTakenMultiplierPerStack = 0.0033f;
    public const float AdvancedPlagueSpreadRadius = 156;

    // Water traits
    public const int CrushingWave1AttacksToTriggerWave = 5;
    public const int CrushingWave1Damage = 5;
    public const int CrushingWave2AttacksToTriggerWave = 5;
    public const int CrushingWave2Damage = 16;
    public const float CrushingWaveTravelDistance = 400;
    public const float CrushingWaveTravelDuration = 1.5f;
    public const float CrushingWaveGameRadius = 150;
    
    public const float TorrentAura1Radius = 313;
    public const float TorrentAura1StackApplicationIntervalInSeconds = 1.5f;
    public const int Torrent1MaxStacks = 8;
    public const float Torrent1MovementSpeedReductionPerStack = 0.03f;
    public const float TorrentAura2Radius = 313;
    public const float TorrentAura2StackApplicationIntervalInSeconds = 1.5f;
    public const int Torrent2MaxStacks = 8;
    public const float Torrent2MovementSpeedReductionPerStack = 0.0375f;

    public const float CripplingDecayAuraRadius = 313;
    public const float CripplingDecayAura2StackApplicationIntervalInSeconds = 1.5f;
    public const int CripplingDecayMaxStacks = 8;
    public const float CripplingDecayMovementSpeedReductionPerStack = .05f;
    // TODO: Split this damage taken effect out into its own aura trait
    public const float CripplingDecayDamageTakenMultiplier = 1.15f;

    // TODO: Include in trait...
    // multiplier = 1 + ((1 - (targetRange / maxRange)) * maxMultiplier)
    public const float PressuringWater1MaxDamageDealtMultiplier = 1.5f;
    public const float PressuringWater2MaxDamageDealtMultiplier = 2f;
    public const float HurricaneStormMaxDamageDealtMultiplier = 2.5f;

    public const float HurricaneStormParalyzationProcChance = 0.25f;
    public const float HurricaneStormParalyzationDuration = 3f;
    public const float HurricaneStormParalyzationImmunityDuration = 8f;

    // Void traits
    public const float VoidGrowth1Radius = 313;
    public const int VoidGrowth1ManaRegenPerSecond = 1;
    public static HashSet<TowerType> VoidGrowth1EligibleTowerTypes
        = new HashSet<TowerType> {
            TowerType.Archer,
            TowerType.Cutter,
            TowerType.Gunner,
            TowerType.Grinder,
        };
    public static HashSet<TowerType> VoidGrowth1PreferredTowerTypes
        = new HashSet<TowerType> {
            TowerType.Archer,
            TowerType.Cutter,
        };
    public const TowerType VoidGrowth1ResultingTowerType = TowerType.Voidling;
    
    public const float VoidGrowth2Radius = 313;
    public const int VoidGrowth2ManaRegenPerSecond = 1;
    public static HashSet<TowerType> VoidGrowth2EligibleTowerTypes
        = new HashSet<TowerType> {
            TowerType.Voidling
        };
    public const TowerType VoidGrowth2ResultingTowerType = TowerType.Voidalisk;

    public const int TemporalRift1ManaRegenPerSecond = 10;
    public const float TemporalShift1Duration = 3f;
    public const float TemporalShift1ExpirationMaxHealthDamage = 0.03f;
    public const int TemporalShift1ExpirationBaseDamage = 150;
    public const float TemporalShift1PerUnitCooldownPeriod = 12f;
    public const int TemporalRift2ManaRegenPerSecond = 10;
    public const float TemporalShift2Duration = 3f;
    public const float TemporalShift2ExpirationMaxHealthDamage = 0.05f;
    public const int TemporalShift2ExpirationBaseDamage = 400;
    public const float TemporalShift2PerUnitCooldownPeriod = 9f;

    public const int ImplosionRiftManaRegenPerSecond = 10;
    public const float TemporalImplosionDuration = 3f;
    public const float TemporalImplosionDetonationRadius = 156;
    public const float TemporalImplosionExpirationMaxHealthDamage = 0.10f;
    public const int TemporalImplosionExpirationBaseDamage = 2500;

    public const float VoidLashing1Duration = 5f;
    public const float VoidLashing1HealingReceivedMultiplier = 0.9f;
    public const int VoidLashing1HealingReceivedFlatDeduction = 25;
    public const float VoidLashing1DamageMultiplierIncreaseByPercentageOfPrimaryTargetCurrentHealth = 0.5f;
    public const float VoidLashing2Duration = 5f;
    public const float VoidLashing2HealingReceivedMultiplier = 0.86f;
    public const int VoidLashing2HealingReceivedFlatDeduction = 80;
    public const float VoidLashing2DamageMultiplierIncreaseByPercentageOfPrimaryTargetCurrentHealth = 1f;

    public const float HungeringVoidBuffDuration = 5f;
    public const float HungeringVoidAuraRadius = 313;
    public const float HungeringVoidHealingReceivedMultiplier = 0.8f;
    public const int HungeringVoidHealingReceivedFlatDeduction = 250;
    public const float
        HungeringVoidDamageDealtAndAttackSpeedMultiplierIncreaseByPercentageOfPrimaryTargetCurrentHealth = 1f;
    
    public const double EssenceOfNatureAura1Range = 235;
    public const double EssenceOfNatureAura2Range = 235;
    public const float EssenceOfNature1AttackSpeedMultiplier = 1.10f;
    public const float EssenceOfNature2AttackSpeedMultiplier = 1.15f;

    public const double EssenceOfFrostAura1Range = 235;
    public const double EssenceOfFrostAura2Range = 235;
    public const float EssenceOfFrostSlow1PerStackMultiplier = .01f;
    public const float EssenceOfFrostSlow2PerStackMultiplier = .015f;
    public const double EssenceOfFrostSlowDuration = 60;
    public const int EssenceOfFrostSlowMaxStacks = 20;

    public const double EssenceOfDarknessAura1Range = 235;
    public const double EssenceOfDarknessAura2Range = 235;
    public const float EssenceOfDarkness1AdditionalDamageDealtPerStack = .02f;
    public const float EssenceOfDarkness2AdditionalDamageDealtPerStack = .02f;
    public const int EssenceOfDarkness1MaxStacks = 4;
    public const int EssenceOfDarkness2MaxStacks = 8;

    public const double EssenceOfTheSeaAura1Range = 235;
    public const double EssenceOfTheSeaAura2Range = 235;
    public const float EssenceOfTheSea1ManaRegenPerSecondDiff = 2;
    public const float EssenceOfTheSea2ManaRegenPerSecondDiff = 5;

    public const double EssenceOfLightAura1Range = 390;
    public const double EssenceOfLightAura2Range = 390;
    public const int EssenceOfLight1ArmorDiff = 3;
    public const int EssenceOfLight2ArmorDiff = 6;
    public const float EssenceOfLight1HealingReceivedMultiplier = 1.65f;
    public const float EssenceOfLight2HealingReceivedMultiplier = 2.00f;
    
    public const double EssenceOfPowerAura1Range = 390;
    public const double EssenceOfPowerAura2Range = 390;
    public const float EssenceOfPower1DamageReturnMultiplier = 3f;
    public const double EssenceOfPower1StunChance = .15;
    public const float EssenceOfPower2DamageReturnMultiplier = 3.5f;
    public const double EssenceOfPower2StunChance = .2;
    public const double EssenceOfPowerStunDuration = 2;
    
    public const double EssenceOfBlightAura1Range = 235;
    public const double EssenceOfBlightAura2Range = 235;
    public const float EssenceOfBlight1ExplosionRadius = 125;
    public const float EssenceOfBlight2ExplosionRadius = 125;
    public const float EssenceOfBlight1ExplosionMultiplierOfMaxHealth = 0.015f;
    public const float EssenceOfBlight2ExplosionMultiplierOfMaxHealth = 0.018f;

    public const int ArcaneReaction1MaxPurgeTargets = 3;
    public const int ArcaneReaction2MaxPurgeTargets = 3;
    public const float ArcaneReaction1Radius = 390;
    public const float ArcaneReaction2Radius = 390;
    public const int ArcaneReaction1Cooldown = 5;
    public const int ArcaneReaction2Cooldown = 5;
    
    public static readonly HashSet<TraitType> SpellResistanceTraitTypes =
        new HashSet<TraitType>() {
            TraitType.BasicSpellResistance,
            TraitType.BasicSpellResistanceWithNoMentionOfImmunity,
            TraitType.MajorSpellResistance,
            TraitType.LegendarySpellResistance,
        };

    public const int ExplosiveReaction1Cooldown = 30;
    public const int ExplosiveReaction2Cooldown = 25;
    public const int ExplosiveReaction1ImmunityDuration = 5;
    public const int ExplosiveReaction2ImmunityDuration = 5;
    public const float ExplosiveReaction1DamageAsRatioOfMaxHealth = 0.2f;
    public const float ExplosiveReaction2DamageAsRatioOfMaxHealth = 0.24f;

    public static readonly Dictionary<EnemyType, HashSet<TraitType>> EnemyTraitMap
        = new Dictionary<EnemyType, HashSet<TraitType>> {
            { EnemyType.Sheep, new HashSet<TraitType> { TraitType.FastProducing } },
            { EnemyType.Wolf, new HashSet<TraitType> { TraitType.FastProducing } },
            { EnemyType.Skeleton, new HashSet<TraitType> { TraitType.DeathPact1 } },
            { EnemyType.Acolyte, new HashSet<TraitType> { TraitType.UnholySacrifice1 } },
            { EnemyType.Spider, new HashSet<TraitType> { TraitType.Armored1 } },
            { EnemyType.Swordsman, new HashSet<TraitType> { TraitType.DevotionAura1 } },
            { EnemyType.Grunt, new HashSet<TraitType> { TraitType.FelBlood } },
            { EnemyType.Temptress, new HashSet<TraitType> { TraitType.EnduranceAura1 } },
            { EnemyType.Shade, new HashSet<TraitType> { TraitType.Flying } },
            { EnemyType.MudGolem, new HashSet<TraitType> { TraitType.BasicSpellResistance } },
            { EnemyType.Treant, new HashSet<TraitType> { TraitType.Attacker, TraitType.LingeringVoid } },
            { EnemyType.RotGolem, new HashSet<TraitType> { TraitType.Boss, TraitType.BasicSpellResistanceWithNoMentionOfImmunity } },
            { EnemyType.Knight, new HashSet<TraitType> { TraitType.Armored2 } },
            { EnemyType.VengefulSpirit, new HashSet<TraitType> { TraitType.DeathPact2, TraitType.AncientAura1 } },
            { EnemyType.ForestTroll, new HashSet<TraitType> { TraitType.Dash1 } },
            { EnemyType.Wyvern, new HashSet<TraitType> { TraitType.Flying } },
            { EnemyType.Voidwalker, new HashSet<TraitType> { TraitType.ChaoticVoid } },
            { EnemyType.FacelessOne, new HashSet<TraitType> { TraitType.EnduranceAura2 } },
            { EnemyType.Dragonspawn, new HashSet<TraitType> { TraitType.Dash2, TraitType.BasicSpellResistance } },
            { EnemyType.SeaTurtle, new HashSet<TraitType> { TraitType.DevotionAura2 } },
            { EnemyType.Banshee, new HashSet<TraitType> { TraitType.Flying, TraitType.UnholySacrifice2 } },
            { EnemyType.SiegeEngine, new HashSet<TraitType> { TraitType.Attacker, TraitType.Bombardment } },
            { EnemyType.KoboldGeomancer, new HashSet<TraitType> { TraitType.GeomancyAura1 } },
            { EnemyType.Infernal, new HashSet<TraitType> { TraitType.Boss, TraitType.BasicSpellResistanceWithNoMentionOfImmunity } },
            { EnemyType.DeathRevenant, new HashSet<TraitType> { TraitType.DeathPact3, TraitType.NecroticTransfusion } },
            { EnemyType.SatyrShadowdancer, new HashSet<TraitType> { TraitType.EnduranceAura3 } },
            { EnemyType.CryptFiend, new HashSet<TraitType> { TraitType.Dash3, TraitType.Skittering } },
            { EnemyType.SpiritWalker, new HashSet<TraitType> { TraitType.Armored3, TraitType.EtherealAura } },
            { EnemyType.Necromancer, new HashSet<TraitType> { TraitType.MajorSpellResistance, TraitType.UnholySacrifice3 } },
            { EnemyType.AncientWendigo, new HashSet<TraitType> { TraitType.HardenedSkin, TraitType.AncientAura2 } },
            { EnemyType.GryphonRider, new HashSet<TraitType> { TraitType.Flying, TraitType.DevotionAura3 } },
            { EnemyType.OgreMagi, new HashSet<TraitType> { TraitType.EarthShieldProvider, TraitType.GeomancyAura2 } },
            { EnemyType.Abomination, new HashSet<TraitType> { TraitType.Dash4 } },
            { EnemyType.ChaosWarden, new HashSet<TraitType> { TraitType.ChaosPortal, TraitType.ChaosEmpowermentAura } },
            { EnemyType.MountainGiant, new HashSet<TraitType> { TraitType.Attacker, TraitType.StoneskinFortitude } },
            { EnemyType.GoblinShredder, new HashSet<TraitType> { TraitType.GoblinEngineering, TraitType.ReactiveArmor, TraitType.EngineOverload } },
            { EnemyType.KodoRider, new HashSet<TraitType> { TraitType.Hypothermia, TraitType.WarStance, TraitType.EnduranceAura4, TraitType.DevotionAura4 } },
            { EnemyType.FrostWyrm, new HashSet<TraitType> { TraitType.Flying, TraitType.MajorSpellResistance, TraitType.UnholySacrifice4, TraitType.NecroticTransfusion } },
            { EnemyType.Phoenix, new HashSet<TraitType> { TraitType.Boss, TraitType.Flying, TraitType.Attacker, TraitType.VolatileDeath, TraitType.LegendarySpellResistance } },
        };

    public static readonly Dictionary<TowerType, HashSet<TraitType>> TowerTraitMap
        = new Dictionary<TowerType, HashSet<TraitType>> {
            // Ranged base
            {TowerType.Archer, new HashSet<TraitType> {} },
            {TowerType.Gunner, new HashSet<TraitType> {} },
            {TowerType.WatchTower, new HashSet<TraitType> {} },
            {TowerType.GuardTower, new HashSet<TraitType> {} },
            {TowerType.WardTower, new HashSet<TraitType> {} },
            {TowerType.UltimateWardTower, new HashSet<TraitType> {} },
            {TowerType.CannonTower, new HashSet<TraitType> { TraitType.GroundAttackOnly } },
            {TowerType.BombardTower, new HashSet<TraitType> { TraitType.GroundAttackOnly } },
            {TowerType.ArtilleryTower, new HashSet<TraitType> { TraitType.GroundAttackOnly } },
            {TowerType.UltimateArtilleryTower, new HashSet<TraitType> { TraitType.GroundAttackOnly } },

            // Melee base
            {TowerType.Cutter, new HashSet<TraitType> { TraitType.GroundAttackOnly } },
            {TowerType.Grinder, new HashSet<TraitType> { TraitType.GroundAttackOnly } },
            {TowerType.Carver, new HashSet<TraitType> { TraitType.GroundAttackOnly } },
            {TowerType.Executioner, new HashSet<TraitType> {} },
            {TowerType.Mauler, new HashSet<TraitType> {} },
            {TowerType.UltimateMauler, new HashSet<TraitType> {} },
            {TowerType.Crusher, new HashSet<TraitType> {} },
            {TowerType.Wrecker, new HashSet<TraitType> {} },
            {TowerType.Mangler, new HashSet<TraitType> {} },
            {TowerType.UltimateMangler, new HashSet<TraitType> {} },

            // Elemental core
            {TowerType.ElementalCore, new HashSet<TraitType> {} },

            // Arcane
            {TowerType.Spellslinger, new HashSet<TraitType> { TraitType.Arcanize1 } },
            {TowerType.SpellslingerMaster, new HashSet<TraitType> { TraitType.Arcanize2 } },
            {TowerType.ArcanePylon, new HashSet<TraitType> { TraitType.VolatileArcane1 } },
            {TowerType.ArcaneRepository, new HashSet<TraitType> { TraitType.VolatileArcane2 } },
            {TowerType.Archmage, new HashSet<TraitType> { TraitType.Spellcaster1 } },
            {TowerType.GrandArchmage, new HashSet<TraitType> { TraitType.Spellcaster2 } },
            {TowerType.UltimateKirinTorWizard, new HashSet<TraitType> { TraitType.KirinTorMastery } },
            {TowerType.UltimateArcaneOrb, new HashSet<TraitType> { TraitType.OverwhelmingArcane } },

            // Fire
            {TowerType.FirePit, new HashSet<TraitType> { TraitType.Ignite1 } },
            {TowerType.MagmaWell, new HashSet<TraitType> { TraitType.Ignite2 } },
            {TowerType.LivingFlame, new HashSet<TraitType> { TraitType.RisingHeat1 } },
            {TowerType.Hellfire, new HashSet<TraitType> { TraitType.RisingHeat2 } },
            {TowerType.UltimateFirelord, new HashSet<TraitType> { TraitType.VolcanicEruption } },
            {TowerType.MeteorAttractor, new HashSet<TraitType> { TraitType.OverwhelmingImpact1 } },
            {TowerType.Armageddon, new HashSet<TraitType> { TraitType.OverwhelmingImpact2 } },
            {TowerType.UltimateMoonbeamProjector, new HashSet<TraitType> { TraitType.VoidFlare } },

            // Ice
            {TowerType.FrozenObelisk, new HashSet<TraitType> { TraitType.FrostAttack1 } },
            {TowerType.RunicObelisk, new HashSet<TraitType> { TraitType.FrostAttack2 } },
            {TowerType.FrozenWatcher, new HashSet<TraitType> { TraitType.FrostBlast1 } },
            {TowerType.FrozenCore, new HashSet<TraitType> { TraitType.FrostBlast2 } },
            {TowerType.UltimateLich, new HashSet<TraitType> { TraitType.ChillingDeath } },
            {TowerType.Icicle, new HashSet<TraitType> { TraitType.IceLance1 } },
            {TowerType.Tricicle, new HashSet<TraitType> { TraitType.IceLance2 } },
            {TowerType.UltimateCrystal, new HashSet<TraitType> { TraitType.CrystallizedLight } },

            // Earth
            {TowerType.Rockfall, new HashSet<TraitType> { TraitType.ShatterArmor1, TraitType.GroundAttackOnly } },
            {TowerType.Avalanche, new HashSet<TraitType> { TraitType.ShatterArmor2, TraitType.GroundAttackOnly } },
            {TowerType.EarthGuardian, new HashSet<TraitType> { TraitType.DevastatingAttack1, TraitType.GroundAttackOnly } },
            {TowerType.AncientProtector, new HashSet<TraitType> { TraitType.DevastatingAttack2, TraitType.GroundAttackOnly } },
            {TowerType.UltimateAncientWarden, new HashSet<TraitType> { TraitType.NaturesGuidance, TraitType.GroundAttackOnly } },
            {TowerType.NoxiousWeed, new HashSet<TraitType> { TraitType.Germination1 } },
            {TowerType.NoxiousThorn, new HashSet<TraitType> { TraitType.Germination2 } },
            {TowerType.UltimateScorpion, new HashSet<TraitType> { TraitType.LethalPreparation } },

            // Lightning
            {TowerType.ShockParticle, new HashSet<TraitType> { TraitType.Overcharge1 } },
            {TowerType.ShockGenerator, new HashSet<TraitType> { TraitType.Overcharge2 } },
            {TowerType.Voltage, new HashSet<TraitType> { TraitType.StaticCharge1 } },
            {TowerType.HighVoltage, new HashSet<TraitType> { TraitType.StaticCharge2 } },
            {TowerType.UltimateOrbKeeper, new HashSet<TraitType> { TraitType.Radiance } },
            {TowerType.LightningBeacon, new HashSet<TraitType> { TraitType.FocusedLightning1, TraitType.TurbulentWeather1 } },
            {TowerType.LightningGenerator, new HashSet<TraitType> { TraitType.FocusedLightning2, TraitType.TurbulentWeather2 } },
            {TowerType.UltimateAnnihilationGlyph, new HashSet<TraitType> { TraitType.Annihilation, TraitType.TurbulentWeather3 } },

            // Holy
            {TowerType.LightFlies, new HashSet<TraitType> { TraitType.BurstingLight1 } },
            {TowerType.HolyLantern, new HashSet<TraitType> { TraitType.BurstingLight2 } },
            {TowerType.SunrayTower, new HashSet<TraitType> { TraitType.BlindingLight1 } },
            {TowerType.SunbeamTower, new HashSet<TraitType> { TraitType.BlindingLight2 } },
            {TowerType.UltimateTitanVault, new HashSet<TraitType> { TraitType.TitanDefenseMechanism } },
            {TowerType.Glowshroom, new HashSet<TraitType> { TraitType.LightBurst1, TraitType.AirAttackOnly } },
            {TowerType.Lightshroom, new HashSet<TraitType> { TraitType.LightBurst2, TraitType.AirAttackOnly } },
            {TowerType.UltimateDivineshroom, new HashSet<TraitType> { TraitType.DivineSpores, TraitType.AirAttackOnly } },

            // Water
            {TowerType.Splasher, new HashSet<TraitType> { TraitType.CrushingWave1 } },
            {TowerType.Tidecaller, new HashSet<TraitType> { TraitType.CrushingWave2 } },
            {TowerType.WaterElemental, new HashSet<TraitType> { TraitType.PressuringWater1 } },
            {TowerType.SeaElemental, new HashSet<TraitType> { TraitType.PressuringWater2 } },
            {TowerType.UltimateHurricaneElemental, new HashSet<TraitType> { TraitType.HurricaneStorm } },
            {TowerType.TideLurker, new HashSet<TraitType> { TraitType.Torrent1 } },
            {TowerType.AbyssStalker, new HashSet<TraitType> { TraitType.Torrent2 } },
            {TowerType.UltimateSludgeMonstrosity, new HashSet<TraitType> { TraitType.CripplingDecay } },

            // Unholy
            {TowerType.PlagueWell, new HashSet<TraitType> { TraitType.Corruption1 } },
            {TowerType.Catacomb, new HashSet<TraitType> { TraitType.Corruption2 } },
            {TowerType.SepticTank, new HashSet<TraitType> { TraitType.RapidInfection1 } },
            {TowerType.PlagueFanatic, new HashSet<TraitType> { TraitType.RapidInfection2 } },
            {TowerType.UltimateGravedigger, new HashSet<TraitType> { TraitType.Pestilence } },
            {TowerType.ObsidianDestroyer, new HashSet<TraitType> { TraitType.UnholyMiasma1 } },
            {TowerType.DecayedHorror, new HashSet<TraitType> { TraitType.UnholyMiasma2 } },
            {TowerType.UltimateDiabolist, new HashSet<TraitType> { TraitType.Hellfire } },

            // Void
            {TowerType.Voidling, new HashSet<TraitType> { TraitType.VoidGrowth1 } },
            {TowerType.Voidalisk, new HashSet<TraitType> { TraitType.VoidGrowth2 } },
            {TowerType.Riftweaver, new HashSet<TraitType> { TraitType.TemporalRift1 } },
            {TowerType.RiftLord, new HashSet<TraitType> { TraitType.TemporalRift2 } },
            {TowerType.UltimateDevourer, new HashSet<TraitType> { TraitType.ImplosionRift } },
            {TowerType.Lasher, new HashSet<TraitType> { TraitType.VoidLashing1 } },
            {TowerType.Ravager, new HashSet<TraitType> { TraitType.VoidLashing2 } },
            {TowerType.UltimateLeviathan, new HashSet<TraitType> { TraitType.HungeringVoid } },

            // Disc
            {TowerType.TechnologyDisc, new HashSet<TraitType> { TraitType.Inactive } },
            {TowerType.TechnologyDisc_Arcane, new HashSet<TraitType> { TraitType.ArcaneReaction1 } },
            {TowerType.TechnologyDisc_Fire, new HashSet<TraitType> { TraitType.ExplosiveReaction1 } },
            {TowerType.TechnologyDisc_Ice, new HashSet<TraitType> { TraitType.EssenceOfFrostAura1 } },
            {TowerType.TechnologyDisc_Earth, new HashSet<TraitType> { TraitType.EssenceOfNatureAura1 } },
            {TowerType.TechnologyDisc_Lightning, new HashSet<TraitType> { TraitType.EssenceOfPowerAura1 } },
            {TowerType.TechnologyDisc_Holy, new HashSet<TraitType> { TraitType.EssenceOfLightAura1 } },
            {TowerType.TechnologyDisc_Water, new HashSet<TraitType> { TraitType.EssenceOfTheSeaAura1 } },
            {TowerType.TechnologyDisc_Unholy, new HashSet<TraitType> { TraitType.EssenceOfBlightAura1 } },
            {TowerType.TechnologyDisc_Void, new HashSet<TraitType> { TraitType.EssenceOfDarknessAura1 } },
            {TowerType.TechnologyDisc_Arcane_Ultimate, new HashSet<TraitType> { TraitType.ArcaneReaction2 } },
            {TowerType.TechnologyDisc_Fire_Ultimate, new HashSet<TraitType> { TraitType.ExplosiveReaction2 } },
            {TowerType.TechnologyDisc_Ice_Ultimate, new HashSet<TraitType> { TraitType.EssenceOfFrostAura2 } },
            {TowerType.TechnologyDisc_Earth_Ultimate, new HashSet<TraitType> { TraitType.EssenceOfNatureAura2 } },
            {TowerType.TechnologyDisc_Lightning_Ultimate, new HashSet<TraitType> { TraitType.EssenceOfPowerAura2 } },
            {TowerType.TechnologyDisc_Holy_Ultimate, new HashSet<TraitType> { TraitType.EssenceOfLightAura2 } },
            {TowerType.TechnologyDisc_Water_Ultimate, new HashSet<TraitType> { TraitType.EssenceOfTheSeaAura2 } },
            {TowerType.TechnologyDisc_Unholy_Ultimate, new HashSet<TraitType> { TraitType.EssenceOfBlightAura2 } },
            {TowerType.TechnologyDisc_Void_Ultimate, new HashSet<TraitType> { TraitType.EssenceOfDarknessAura2 } },
        };
}