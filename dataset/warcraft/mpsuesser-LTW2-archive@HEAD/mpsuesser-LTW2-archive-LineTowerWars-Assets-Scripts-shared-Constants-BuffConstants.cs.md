---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Constants/BuffConstants.cs"
ref: HEAD
---

﻿using System.Collections.Generic;

public static class BuffConstants {

    // The ordering within lists will be 0th index is lowest priority and later indexes overwrite
    public static readonly HashSet<BuffType[]> MutuallyExclusiveBuffTypes = new HashSet<BuffType[]>() {
        new[] { BuffType.Ancient1, BuffType.Ancient2 },
        new[] { BuffType.Devotion1, BuffType.Devotion2, BuffType.Devotion3, BuffType.Devotion4 },
        new[] { BuffType.Endurance1, BuffType.Endurance2, BuffType.Endurance3, BuffType.Endurance4 },
        new[] { BuffType.Geomancy1, BuffType.Geomancy2 },
        new[] { BuffType.TorrentSlow1, BuffType.TorrentSlow2, BuffType.CripplingDecaySlow },
        new[] { BuffType.TemporalShift1, BuffType.TemporalShift2, BuffType.TemporalImplosion },
        new[] { BuffType.VoidLashingDebuff1, BuffType.VoidLashingDebuff2 },
        new[] { BuffType.Corrupted1, BuffType.Corrupted2 },
        new[] { BuffType.FrostboltSlow, BuffType.IceblastSlow, BuffType.ChainsOfIceSlow },
        new[] { BuffType.TurbulentWeather1, BuffType.TurbulentWeather2, BuffType.TurbulentWeather3 },
        new[] { BuffType.FireboltStun, BuffType.FireblastStun, BuffType.PyroblastStun },
        new[] { BuffType.BlindedByTheLight1, BuffType.BlindedByTheLight2, BuffType.TitanicallyBlind },
        new[] { BuffType.FrostAttackSlow1, BuffType.FrostAttackSlow2, BuffType.FrostBlastSlow1, BuffType.FrostBlastSlow2, BuffType.ChillingDeathSlow },
        new[] { BuffType.EssenceOfNature1, BuffType.EssenceOfNature2 },
        new[] { BuffType.EssenceOfFrost1, BuffType.EssenceOfFrost2 },
        new[] { BuffType.EssenceOfPower1, BuffType.EssenceOfPower2 },
        new[] { BuffType.EssenceOfTheSea1, BuffType.EssenceOfTheSea2 },
        new[] { BuffType.EssenceOfLight1, BuffType.EssenceOfLight2 },
        new[] { BuffType.EssenceOfBlight1, BuffType.EssenceOfBlight2 },
        new[] { BuffType.EssenceOfDarkness1, BuffType.EssenceOfDarkness2 },
    };

    public static readonly HashSet<BuffType> HarmfulSpellEffectTypes = new HashSet<BuffType>() {
        BuffType.LingeringVoid,
        BuffType.StaticOverload,
        BuffType.TorrentSlow1,
        BuffType.TorrentSlow2,
        BuffType.TemporalShift1,
        BuffType.TemporalShift2,
        BuffType.TemporalImplosion,
        BuffType.VoidLashingDebuff1,
        BuffType.VoidLashingDebuff2,
        BuffType.HungeringVoidDebuff,
        BuffType.Corrupted1,
        BuffType.Corrupted2,
        BuffType.Plague,
        BuffType.UnendingPlague,
        BuffType.Ignite1,
        BuffType.Ignite2,
        BuffType.MeteoricVulnerability,
        BuffType.VolcanicExposure,
        BuffType.VolcanicOpportunity,
        BuffType.ShatteredArmor1,
        BuffType.ShatteredArmor2,
        BuffType.Devastation1,
        BuffType.Devastation2,
        BuffType.NaturesForce,
        BuffType.TurbulentWeather1,
        BuffType.TurbulentWeather2,
        BuffType.TurbulentWeather3,
        BuffType.FrostboltSlow,
        BuffType.IceblastSlow,
        BuffType.ChainsOfIceSlow,
        BuffType.FireboltStun,
        BuffType.FireblastStun,
        BuffType.PyroblastStun,
        BuffType.ArcaneExposure,
        BuffType.BlindedByTheLight1,
        BuffType.BlindedByTheLight2,
        BuffType.TitanicallyBlind,
        BuffType.FrostAttackSlow1,
        BuffType.FrostAttackSlow2,
        BuffType.FrostBlastSlow1,
        BuffType.FrostBlastSlow2,
        BuffType.ChillingDeathSlow,
        BuffType.ChillingDeathAttackSpeedSlow,
        BuffType.Frostbite,
        BuffType.IceLanced1,
        BuffType.IceLanced2,
        BuffType.CrystallizedPenetration,
        BuffType.EssenceOfPowerStun,
        BuffType.EssenceOfFrostSlow1,
        BuffType.EssenceOfFrostSlow2,
    };
}