---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Constants/EffectDescriptors.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class EffectDescriptor {
    // Indicators for formatting on the client side
    public const string BI = "%B"; // BI = buff indicator
    public const string NL = "%N"; // NL = new line
    
    public string Name { get; }
    public string Description { get; protected set; }
    public List<BuffType> AssociatedBuffs { get; }

    public EffectDescriptor(
        string name,
        string desc,
        params BuffType[] buffTypes
    ) {
        Name = name;
        Description = desc;
        AssociatedBuffs = new List<BuffType>(buffTypes);
    }
}

public class BuffEffectDescriptor : EffectDescriptor {
    public BuffEffectDescriptor(
        string name,
        string desc,
        float duration = -1f
    ) : base(name, desc) {
        if (duration > 0f) {
            // TODO: We can do anything here. Another option is to store this value as
            // a member and display it in the corner of the tooltip instead of appending
            // it to the description.
            AppendDurationToDescription(duration);
        }
    }

    private void AppendDurationToDescription(float duration) {
        Description +=
            NL
            + NL
            + $"Lasts for {EffectDescriptors.FormatNumber(duration)} seconds.";
    }
}

public static class EffectDescriptors {
    private static string FormatNumber(int num) => num.ToString();
    
    // TODO: If these numbers are within some interval of a whole number (epsilon),
    // round them and print the result of FormatNumber(int). Otherwise, print only exactly
    // as many sig figs as is necessary.
    private static string FormatNumber(double num) => num.ToString();
    public static string FormatNumber(float num) => num.ToString();
    
    private static string FormatNumberDirectlyToWholePercentage(double num) =>
        Mathf.Round((float) num * 100f).ToString(CultureInfo.InvariantCulture) + '%';
    
    public static readonly Dictionary<BuffType, EffectDescriptor> Buff = new Dictionary<BuffType, EffectDescriptor>() {
        // Creeps
        {
            BuffType.Devotion1,
            new BuffEffectDescriptor(
              "Devotion (1)",
              $"Increases armor by {FormatNumber(TraitConstants.DevotionAura1ArmorDiff)}."
            )
        },
        {
            BuffType.Devotion2,
            new BuffEffectDescriptor(
              "Devotion (2)",
              $"Increases armor by {FormatNumber(TraitConstants.DevotionAura2ArmorDiff)}."
            )
        },
        {
            BuffType.Devotion3,
            new BuffEffectDescriptor(
              "Devotion (3)",
              $"Increases armor by {FormatNumber(TraitConstants.DevotionAura3ArmorDiff)}."
            )
        },
        {
            BuffType.Devotion4,
            new BuffEffectDescriptor(
              "Devotion (4)",
              $"Increases armor by {FormatNumber(TraitConstants.DevotionAura4ArmorDiff)}."
            )
        },
        {
            BuffType.Endurance1,
            new BuffEffectDescriptor(
              "Endurance (1)",
              $"Increases movement speed by {FormatNumberDirectlyToWholePercentage(TraitConstants.EnduranceAura1SpeedMultiplier)}."
            )
        },
        {
            BuffType.Endurance2,
            new BuffEffectDescriptor(
              "Endurance (2)",
              $"Increases movement speed by {FormatNumberDirectlyToWholePercentage(TraitConstants.EnduranceAura2SpeedMultiplier)}."
            )
        },
        {
            BuffType.Endurance3,
            new BuffEffectDescriptor(
              "Endurance (3)",
              $"Increases movement speed by {FormatNumberDirectlyToWholePercentage(TraitConstants.EnduranceAura3SpeedMultiplier)}."
            )
        },
        {
            BuffType.Endurance4,
            new BuffEffectDescriptor(
              "Endurance (4)",
              $"Increases movement speed by {FormatNumberDirectlyToWholePercentage(TraitConstants.EnduranceAura4SpeedMultiplier)}."
            )
        },
        {
            BuffType.LingeringVoid,
            new BuffEffectDescriptor(
              "Lingering Void",
              $"Slows attack speed by {FormatNumberDirectlyToWholePercentage(TraitConstants.LingeringVoidAttackSpeedReductionPerStack)} per stack. Each stack independently lasts for {FormatNumber(TraitConstants.LingeringVoidDuration)} seconds."
            )
        },
        {
            BuffType.Ancient1,
            new BuffEffectDescriptor(
              "Ancient (1)",
              $"Reduces the effectiveness of movement-slowing effects by {FormatNumberDirectlyToWholePercentage(TraitConstants.Ancient1NegativeMovementEffectivenessMultiplier)}."
            )
        },
        {
            BuffType.Ancient2,
            new BuffEffectDescriptor(
              "Ancient (2)",
              $"Reduces the effectiveness of movement-slowing effects by {FormatNumberDirectlyToWholePercentage(TraitConstants.Ancient2NegativeMovementEffectivenessMultiplier)}."
            )
        },
        {
            BuffType.Geomancy1,
            new BuffEffectDescriptor(
              "Geomancy (1)",
              $"Reduces the effectiveness of armor-reducing effects by {FormatNumberDirectlyToWholePercentage(TraitConstants.Geomancy1ArmorReductionEffectivenessMultiplier)}."
            )
        },
        {
            BuffType.Geomancy2,
            new BuffEffectDescriptor(
              "Geomancy (2)",
              $"Reduces the effectiveness of armor-reducing effects by {FormatNumberDirectlyToWholePercentage(TraitConstants.Geomancy2ArmorReductionEffectivenessMultiplier)}."
            )
        },
        {
            BuffType.NecroticTransfusion,
            new BuffEffectDescriptor(
              "Necrotic Transfusion",
              "Steals one additional life per stack."
            )
        },
        {
            BuffType.Ethereal,
            new BuffEffectDescriptor(
              "Ethereal",
              $"Increases armor by {FormatNumber(TraitConstants.EtherealArmorPerStack)} per stack."
            )
        },
        {
            BuffType.HardenedSkin,
            new BuffEffectDescriptor(
              "Hardened Skin",
              $"Increases armor by {FormatNumber(TraitConstants.HardenedSkinArmorPerStack)} per stack."
              + EffectDescriptor.NL
              + EffectDescriptor.NL
              + $"Any physical damage taken above {FormatNumber(TraitConstants.HardenedSkinMinimumDamageToRemoveStack)} removes one stack."
            )
        },
        {
            BuffType.EarthShield,
            new BuffEffectDescriptor(
              "Earth Shield",
              $"Healing for {FormatNumberDirectlyToWholePercentage(TraitConstants.EarthShieldHealPercentageOfMaxHealth)} every {FormatNumber(TraitConstants.EarthShieldHealInterval)} seconds."
              + EffectDescriptor.NL
              + EffectDescriptor.NL
              + $"Removes all movement-slowing effects upon application.",
              TraitConstants.EarthShieldDuration
            )
        },
        {
            BuffType.ChaosEmpowerment,
            new BuffEffectDescriptor(
              "Chaos Empowerment",
              $"Gaining {FormatNumber(TraitConstants.ChaosEmpowermentManaGainPerSecond)} mana per second."
              + EffectDescriptor.NL
              + EffectDescriptor.NL
              + $"Increases spell resist by {FormatNumber(TraitConstants.ChaosEmpowermentSpellResist)}."
            )
        },
        {
            BuffType.StaticOverload,
            new BuffEffectDescriptor(
              "Static Overload",
              $"Attack speed is reduced by {FormatNumberDirectlyToWholePercentage(1 - TraitConstants.EngineOverloadAttackSpeedMultiplier)}.",
              TraitConstants.EngineOverloadDuration
            )
        },
        {
            BuffType.WarStance,
            new BuffEffectDescriptor(
              "War Stance",
              $""
            )
        },
        {
            BuffType.WarCry,
            new BuffEffectDescriptor(
              "War Cry",
              $""
            )
        },
        // Towers
        {
            BuffType.TorrentSlow1,
            new BuffEffectDescriptor(
              "Torrent (1)",
              $"Reduces movement speed by {FormatNumberDirectlyToWholePercentage(TraitConstants.Torrent1MovementSpeedReductionPerStack)} per stack. "
            + $"Stacks are capped at {FormatNumber(TraitConstants.Torrent1MaxStacks)}."
            )
        },
        {
            BuffType.TorrentSlow2,
            new BuffEffectDescriptor(
              "Torrent (2)",
              $"Reduces movement speed by {FormatNumberDirectlyToWholePercentage(TraitConstants.Torrent2MovementSpeedReductionPerStack)} per stack. "
              + $"Stacks are capped at {FormatNumber(TraitConstants.Torrent2MaxStacks)}."
            )
        },
        {
            BuffType.CripplingDecaySlow,
            new BuffEffectDescriptor(
              "Crippling Decay",
              $"Reduces movement speed by {FormatNumberDirectlyToWholePercentage(TraitConstants.CripplingDecayMovementSpeedReductionPerStack)} per stack. "
              + $"Stacks are capped at {FormatNumber(TraitConstants.CripplingDecayMaxStacks)}."
            )
        },
        {
            BuffType.CripplingVulnerability,
            new BuffEffectDescriptor(
              "Crippling Vulnerability",
              $"Increases damage taken by {FormatNumberDirectlyToWholePercentage(TraitConstants.CripplingDecayDamageTakenMultiplier)}."
            )
        },
        {
            BuffType.HurricaneStormParalysis,
            new BuffEffectDescriptor(
              "Hurricane Lockdown",
              $"Paralyzed; unable to move and can be attacked by ground-only towers.",
              TraitConstants.HurricaneStormParalyzationDuration
            )
        },
        {
            BuffType.TemporalShift1,
            new BuffEffectDescriptor(
              "Temporal Shift (1)",
              $"Upon expiration, this unit will be shifted back to its location at the time of application and take damage equal to ({FormatNumberDirectlyToWholePercentage(TraitConstants.TemporalShift1ExpirationMaxHealthDamage)} of its maximum health + {FormatNumber(TraitConstants.TemporalShift1ExpirationBaseDamage)}.",
              TraitConstants.TemporalShift1Duration
            )
        },
        {
            BuffType.TemporalShift2,
            new BuffEffectDescriptor(
              "Temporal Shift (2)",
              $"Upon expiration, this unit will be shifted back to its location at the time of application and take damage equal to ({FormatNumberDirectlyToWholePercentage(TraitConstants.TemporalShift2ExpirationMaxHealthDamage)} of its maximum health + {FormatNumber(TraitConstants.TemporalShift2ExpirationBaseDamage)}.",
              TraitConstants.TemporalShift2Duration
            )
        },
        {
            BuffType.TemporalImplosion,
            new BuffEffectDescriptor(
              "Temporal Implosion",
              $"Dying with this effect will cause a creep within a {FormatNumber(TraitConstants.TemporalImplosionDetonationRadius)} radius to be sent back to spawn and take damage equal to ({FormatNumberDirectlyToWholePercentage(TraitConstants.TemporalImplosionExpirationMaxHealthDamage)} of its maximum health + {FormatNumber(TraitConstants.TemporalImplosionExpirationBaseDamage)}.",
              TraitConstants.TemporalImplosionDuration
            )
        },
        {
            BuffType.VoidLashingDebuff1,
            new BuffEffectDescriptor(
              "Lashed (1)",
              $"Reduces healing received by ({FormatNumberDirectlyToWholePercentage(TraitConstants.VoidLashing1HealingReceivedMultiplier)} + {FormatNumber(TraitConstants.VoidLashing1HealingReceivedFlatDeduction)}).",
              TraitConstants.VoidLashing1Duration
            )
        },
        {
            BuffType.VoidLashingDebuff2,
            new BuffEffectDescriptor(
              "Lashed (2)",
              $"Reduces healing received by ({FormatNumberDirectlyToWholePercentage(TraitConstants.VoidLashing2HealingReceivedMultiplier)} + {FormatNumber(TraitConstants.VoidLashing2HealingReceivedFlatDeduction)}).",
              TraitConstants.VoidLashing2Duration
            )
        },
        {
            BuffType.VoidLashingBuff1,
            new BuffEffectDescriptor(
              "Void Lashing (1)",
              $"Increases attack damage equal to {FormatNumberDirectlyToWholePercentage(TraitConstants.VoidLashing1DamageMultiplierIncreaseByPercentageOfPrimaryTargetCurrentHealth)} of the current %-health of the most recent primary target.",
              TraitConstants.VoidLashing1Duration
            )
        },
        {
            BuffType.VoidLashingBuff2,
            new BuffEffectDescriptor(
              "Void Lashing (1)",
              $"Increases attack damage equal to {FormatNumberDirectlyToWholePercentage(TraitConstants.VoidLashing2DamageMultiplierIncreaseByPercentageOfPrimaryTargetCurrentHealth)} of the current %-health of the most recent primary target.",
              TraitConstants.VoidLashing2Duration
            )
        },
        {
            BuffType.HungeringVoidDebuff,
            new BuffEffectDescriptor(
              "Hungry",
              $"Reduces healing received by ({FormatNumberDirectlyToWholePercentage(TraitConstants.HungeringVoidHealingReceivedMultiplier)} + {FormatNumber(TraitConstants.HungeringVoidHealingReceivedFlatDeduction)})."
            )
        },
        {
            BuffType.HungeringVoidBuff,
            new BuffEffectDescriptor(
              "Hungering Void",
              $"Increases attack damage and attack speed equal to {FormatNumberDirectlyToWholePercentage(TraitConstants.HungeringVoidDamageDealtAndAttackSpeedMultiplierIncreaseByPercentageOfPrimaryTargetCurrentHealth)} of the current %-health of the most recent primary target.",
              TraitConstants.HungeringVoidBuffDuration
            )
        },
        {
            BuffType.Corrupted1,
            new BuffEffectDescriptor(
              "Corrupted (1)",
              $""
            )
        },
        {
            BuffType.Corrupted2,
            new BuffEffectDescriptor(
              "Corrupted (2)",
              $""
            )
        },
        {
            BuffType.Plague,
            new BuffEffectDescriptor(
              "Plague",
              $""
            )
        },
        {
            BuffType.UnendingPlague,
            new BuffEffectDescriptor(
              "Unending Plague",
              $""
            )
        },
        {
            BuffType.Ignite1,
            new BuffEffectDescriptor(
              "Ignite (1)",
              $""
            )
        },
        {
            BuffType.Ignite2,
            new BuffEffectDescriptor(
              "Ignite (2)",
              $""
            )
        },
        {
            BuffType.MeteoricVulnerability,
            new BuffEffectDescriptor(
              "Meteoric Vulnerability",
              $""
            )
        },
        {
            BuffType.RisingHeat1,
            new BuffEffectDescriptor(
              "Rising Heat (1)",
              $""
            )
        },
        {
            BuffType.RisingHeat2,
            new BuffEffectDescriptor(
              "Rising Heat (2)",
              $""
            )
        },
        {
            BuffType.VolcanicExposure,
            new BuffEffectDescriptor(
              "Volcanic Exposure",
              $""
            )
        },
        {
            BuffType.VolcanicOpportunity,
            new BuffEffectDescriptor(
              "Volcanic Opportunity",
              $""
            )
        },
        {
            BuffType.ShatteredArmor1,
            new BuffEffectDescriptor(
              "Shattered Armor (1)",
              $""
            )
        },
        {
            BuffType.ShatteredArmor2,
            new BuffEffectDescriptor(
              "Shattered Armor (2)",
              $""
            )
        },
        {
            BuffType.Devastation1,
            new BuffEffectDescriptor(
              "Devastation (1)",
              $""
            )
        },
        {
            BuffType.Devastation2,
            new BuffEffectDescriptor(
              "Devastation (2)",
              $""
            )
        },
        {
            BuffType.NaturesForce,
            new BuffEffectDescriptor(
              "Nature's Force",
              $""
            )
        },
        {
            BuffType.Germination1,
            new BuffEffectDescriptor(
              "Germination (1)",
              $""
            )
        },
        {
            BuffType.Germination2,
            new BuffEffectDescriptor(
              "Germination (2)",
              $""
            )
        },
        {
            BuffType.LethalPreparation,
            new BuffEffectDescriptor(
              "Lethal Preparation",
              $""
            )
        },
        {
            BuffType.TurbulentWeather1,
            new BuffEffectDescriptor(
              "Turbulent Weather (1)",
              $""
            )
        },
        {
            BuffType.TurbulentWeather2,
            new BuffEffectDescriptor(
              "Turbulent Weather (2)",
              $""
            )
        },
        {
            BuffType.TurbulentWeather3,
            new BuffEffectDescriptor(
              "Turbulent Weather (3)",
              $""
            )
        },
        {
            BuffType.FrostboltSlow,
            new BuffEffectDescriptor(
              "Slow (Frostbolt)",
              $""
            )
        },
        {
            BuffType.IceblastSlow,
            new BuffEffectDescriptor(
              "Slow (Iceblast)",
              $""
            )
        },
        {
            BuffType.ChainsOfIceSlow,
            new BuffEffectDescriptor(
              "Slow (Chains of Ice)",
              $""
            )
        },
        {
            BuffType.FireboltStun,
            new BuffEffectDescriptor(
              "Stun (Firebolt)",
              $""
            )
        },
        {
            BuffType.FireblastStun,
            new BuffEffectDescriptor(
              "Stun (Fireblast)",
              $""
            )
        },
        {
            BuffType.PyroblastStun,
            new BuffEffectDescriptor(
              "Stun (Pyroblast)",
              $""
            )
        },
        {
            BuffType.ArcaneExposure,
            new BuffEffectDescriptor(
              "Arcane Exposure",
              $""
            )
        },
        {
            BuffType.BlindedByTheLight1,
            new BuffEffectDescriptor(
              "Blinded by the Light (1)",
              $""
            )
        },
        {
            BuffType.BlindedByTheLight2,
            new BuffEffectDescriptor(
              "Blinded by the Light (2)",
              $""
            )
        },
        {
            BuffType.TitanicallyBlind,
            new BuffEffectDescriptor(
              "Titanically Blind",
              $""
            )
        },
        {
            BuffType.FrostAttackSlow1,
            new BuffEffectDescriptor(
              "Slow (Frost Attack (1))",
              $""
            )
        },
        {
            BuffType.FrostAttackSlow2,
            new BuffEffectDescriptor(
              "Slow (Frost Attack (2))",
              $""
            )
        },
        {
            BuffType.FrostBlastSlow1,
            new BuffEffectDescriptor(
              "Slow (Frost Blast (1))",
              $""
            )
        },
        {
            BuffType.FrostBlastSlow2,
            new BuffEffectDescriptor(
              "Slow (Frost Blast (2))",
              $""
            )
        },
        {
            BuffType.ChillingDeathSlow,
            new BuffEffectDescriptor(
              "Slow (Chilling Death)",
              $""
            )
        },
        {
            BuffType.ChillingDeathAttackSpeedSlow,
            new BuffEffectDescriptor(
              "Chilled to the Bone",
              $""
            )
        },
        {
            BuffType.Frostbite,
            new BuffEffectDescriptor(
              "Frostbite",
              $""
            )
        },
        {
            BuffType.IceLanced1,
            new BuffEffectDescriptor(
              "Ice Lanced (1)",
              $""
            )
        },
        {
            BuffType.IceLanced2,
            new BuffEffectDescriptor(
              "Ice Lanced (2)",
              $""
            )
        },
        {
            BuffType.CrystallizedPenetration,
            new BuffEffectDescriptor(
              "Crystallized Penetration",
              $""
            )
        },
        // Discs
        {
            BuffType.EssenceOfNature1,
            new BuffEffectDescriptor(
              "Essence of Nature (1)",
              $""
            )
        },
        {
            BuffType.EssenceOfFrost1,
            new BuffEffectDescriptor(
              "Essence of Frost (1)",
              $""
            )
        },
        {
            BuffType.EssenceOfPower1,
            new BuffEffectDescriptor(
              "Essence of Power (1)",
              $""
            )
        },
        {
            BuffType.EssenceOfTheSea1,
            new BuffEffectDescriptor(
              "Essence of the Sea (1)",
              $""
            )
        },
        {
            BuffType.EssenceOfLight1,
            new BuffEffectDescriptor(
              "Essence of Light (1)",
              $""
            )
        },
        {
            BuffType.EssenceOfBlight1,
            new BuffEffectDescriptor(
              "Essence of Blight (1)",
              $""
            )
        },
        {
            BuffType.EssenceOfDarkness1,
            new BuffEffectDescriptor(
              "Essence of Darkness (1)",
              $""
            )
        },
        {
            BuffType.EssenceOfNature2,
            new BuffEffectDescriptor(
              "Essence of Nature (2)",
              $""
            )
        },
        {
            BuffType.EssenceOfFrost2,
            new BuffEffectDescriptor(
              "Essence of Frost (2)",
              $""
            )
        },
        {
            BuffType.EssenceOfPower2,
            new BuffEffectDescriptor(
              "Essence of Power (2)",
              $""
            )
        },
        {
            BuffType.EssenceOfTheSea2,
            new BuffEffectDescriptor(
              "Essence of the Sea (2)",
              $""
            )
        },
        {
            BuffType.EssenceOfLight2,
            new BuffEffectDescriptor(
              "Essence of Light (2)",
              $""
            )
        },
        {
            BuffType.EssenceOfBlight2,
            new BuffEffectDescriptor(
              "Essence of Blight (2)",
              $""
            )
        },
        {
            BuffType.EssenceOfDarkness2,
            new BuffEffectDescriptor(
              "Essence of Darkness (2)",
              $""
            )
        },
        {
            BuffType.EssenceOfPowerStun,
            new BuffEffectDescriptor(
              "Essence of Power (2)",
              $""
            )
        },
        {
            BuffType.EssenceOfFrostSlow1,
            new BuffEffectDescriptor(
              "Slow (Essence of Frost (1))",
              $""
            )
        },
        {
            BuffType.EssenceOfFrostSlow2,
            new BuffEffectDescriptor(
              "Slow (Essence of Frost (2))",
              $""
            )
        },
    };
    
    public static Dictionary<TraitType, EffectDescriptor> Trait
        = new Dictionary<TraitType, EffectDescriptor> {
        // Creeps
        {
            TraitType.FastProducing,
            new EffectDescriptor(
                "Fast Producing",
                $"The initial stock of this unit after the delay is {TraitConstants.FastProducingInitialStockAmount} and stock replenishes more quickly, at a rate of once every {TraitConstants.FastProducingRestockInterval} seconds."
            )
        },
        {
            TraitType.DevotionAura1,
            new EffectDescriptor(
                "Devotion Aura (1)",
                $"Applies {EffectDescriptor.BI} to all creeps within a {TraitConstants.DevotionAura1Range} radius.",
                BuffType.Devotion1
            )
        },
        {
            TraitType.DevotionAura2,
            new EffectDescriptor(
                "Devotion Aura (2)",
                $"Applies {EffectDescriptor.BI} to all creeps within a {TraitConstants.DevotionAura2Range} radius.",
                BuffType.Devotion2
            )
        },
        {
            TraitType.DevotionAura3,
            new EffectDescriptor(
                "Devotion Aura (3)",
                $"Applies {EffectDescriptor.BI} to all creeps within a {TraitConstants.DevotionAura3Range} radius.",
                BuffType.Devotion3
            )
        },
        {
            TraitType.DevotionAura4,
            new EffectDescriptor(
                "Devotion Aura (4)",
                $"Applies {EffectDescriptor.BI} to all creeps within a {TraitConstants.DevotionAura4Range} radius.",
                BuffType.Devotion4
            )
        },
        {
            TraitType.EnduranceAura1,
            new EffectDescriptor(
                "Endurance Aura (1)",
                $"Applies {EffectDescriptor.BI} to all creeps within a {TraitConstants.EnduranceAura1Range} radius.",
                BuffType.Endurance1
            )
        },
        {
            TraitType.EnduranceAura2,
            new EffectDescriptor(
                "Endurance Aura (2)",
                $"Applies {EffectDescriptor.BI} to all creeps within a {TraitConstants.EnduranceAura2Range} radius.",
                BuffType.Endurance2
            )
        },
        {
            TraitType.EnduranceAura3,
            new EffectDescriptor(
                "Endurance Aura (3)",
                $"Applies {EffectDescriptor.BI} to all creeps within a {TraitConstants.EnduranceAura3Range} radius.",
                BuffType.Endurance3
            )
        },
        {
            TraitType.EnduranceAura4,
            new EffectDescriptor(
                "Endurance Aura (4)",
                $"Applies {EffectDescriptor.BI} to all creeps within a {TraitConstants.EnduranceAura4Range} radius.",
                BuffType.Endurance4
            )
        },
        {
            TraitType.DeathPact1,
            new EffectDescriptor(
                "Death Pact (1)",
                $"Returns to life with {TraitConstants.DeathPact1ReviveHealthMultiplier * 100}% health after {TraitConstants.DeathPact1ReviveDelay} seconds when killed. Only triggers once."
            )
        },
        {
            TraitType.DeathPact2,
            new EffectDescriptor(
                "Death Pact (2)",
                $"Returns to life with {TraitConstants.DeathPact2ReviveHealthMultiplier * 100}% health after {TraitConstants.DeathPact2ReviveDelay} seconds when killed. Only triggers once."
            )
        },
        {
            TraitType.DeathPact3,
            new EffectDescriptor(
                "Death Pact (3)",
                $"Returns to life with {TraitConstants.DeathPact3ReviveHealthMultiplier * 100}% health after {TraitConstants.DeathPact3ReviveDelay} seconds when killed. Only triggers once."
            )
        },
        {
            TraitType.UnholySacrifice1,
            new EffectDescriptor(
                "Unholy Sacrifice (1)",
                $"Heals all allied creeps within a {TraitConstants.UnholySacrifice1Range} radius for {TraitConstants.UnholySacrifice1HealAmount} when killed."
            )
        },
        {
            TraitType.UnholySacrifice2,
            new EffectDescriptor(
                "Unholy Sacrifice (2)",
                $"Heals all allied creeps within a {TraitConstants.UnholySacrifice2Range} radius for {TraitConstants.UnholySacrifice2HealAmount} when killed."
            )
        },
        {
            TraitType.UnholySacrifice3,
            new EffectDescriptor(
                "Unholy Sacrifice (3)",
                $"Heals all allied creeps within a {TraitConstants.UnholySacrifice3Range} radius for {TraitConstants.UnholySacrifice3HealAmount} when killed."
            )
        },
        {
            TraitType.UnholySacrifice4,
            new EffectDescriptor(
                "Unholy Sacrifice (4)",
                $"Heals all allied creeps within a {TraitConstants.UnholySacrifice4Range} radius for {TraitConstants.UnholySacrifice4HealAmount} when killed."
            )
        },
        {
            TraitType.Armored1,
            new EffectDescriptor(
                "Armored (1)",
                $"Damage taken from physical splash damage is reduced by {100 - (TraitConstants.Armored1PhysicalSplashDamageTakenMultiplier * 100)}%."
            )
        },
        {
            TraitType.Armored2,
            new EffectDescriptor(
                "Armored (2)",
                $"Damage taken from physical splash damage is reduced by {100 - (TraitConstants.Armored2PhysicalSplashDamageTakenMultiplier * 100)}%."
            )
        },
        {
            TraitType.Armored3,
            new EffectDescriptor(
                "Armored (3)",
                $"Damage taken from physical splash damage is reduced by {100 - (TraitConstants.Armored3PhysicalSplashDamageTakenMultiplier * 100)}%."
            )
        },
        {
            TraitType.FelBlood,
            new EffectDescriptor(
                "Fel Blood",
                $"This creep has an unusually high rate of health regeneration."
            )
        },
        {
            TraitType.Flying,
            new EffectDescriptor(
                "Flying",
                $"Flies over enemy towers and avoids ground attacks."
            )
        },
        {
            TraitType.Attacker,
            new EffectDescriptor(
                "Attacker",
                $"Attacks enemy towers and attempts to destroy them."
            )
        },
        {
            TraitType.LingeringVoid,
            new EffectDescriptor(
                "Lingering Void",
                $"Each attack applies one stack of {EffectDescriptor.BI} to the target tower.",
                BuffType.LingeringVoid
            )
        },
        {
            TraitType.Boss,
            new EffectDescriptor(
                "Boss",
                $"Sent as one strong creep with high base health, health regeneration, "+" and armor. Steals two lives."
            )
        },
        {
            TraitType.BasicSpellResistance,
            new EffectDescriptor(
                "Basic Spell Resistance",
                $"This creep has a naturally high spell resistance and immunity to all harmful movement-slowing effects. In addition, the durations of all harmful spell effects applied to this creep are reduced by {TraitConstants.BasicSpellResistanceHarmfulSpellEffectsDurationMultiplier * 100}%."
            )
        },
        {
            TraitType.BasicSpellResistanceWithNoMentionOfImmunity,
            new EffectDescriptor(
                "Basic Spell Resistance",
                "This creep has a naturally high spell resistance. In addition, the durations of all harmful spell effects applied to this creep are reduced by {BasicSpellResistanceHarmfulSpellEffectsDurationMultiplier * 100}%."
            )
        },
        {
            TraitType.MajorSpellResistance,
            new EffectDescriptor(
                "Major Spell Resistance",
                $"This creep has a naturally high spell resistance and immunity to all harmful movement-slowing effects. In addition, the durations of all harmful spell effects applied to this creep are reduced by {TraitConstants.MajorSpellResistanceHarmfulSpellEffectsDurationMultiplier * 100}%."
            )
        },
        {
            TraitType.LegendarySpellResistance,
            new EffectDescriptor(
                "Legendary Spell Resistance",
                $"This creep has a naturally high spell resistance and immunity to all harmful spell effects."
            )
        },
        {
            TraitType.AncientAura1,
            new EffectDescriptor(
                "Ancient Aura (1)",
                $"Applies {EffectDescriptor.BI} to all creeps within a {TraitConstants.AncientAura1Range} radius.",
                BuffType.Ancient1
            )
        },
        {
            TraitType.AncientAura2,
            new EffectDescriptor(
                "Ancient Aura (2)",
                $"Applies {EffectDescriptor.BI} to all creeps within a {TraitConstants.AncientAura2Range} radius.",
                BuffType.Ancient2
            )
        },
        {
            TraitType.Dash1,
            new EffectDescriptor(
                "Dash (1)",
                $"This creep is very fast."
            )
        },
        {
            TraitType.Dash2,
            new EffectDescriptor(
                "Dash (2)",
                $"This creep is very fast."
            )
        },
        {
            TraitType.Dash3,
            new EffectDescriptor(
                "Dash (3)",
                $"This creep is very fast."
            )
        },
        {
            TraitType.Dash4,
            new EffectDescriptor(
                "Dash (4)",
                $"This creep is very fast."
            )
        },
        {
            TraitType.ChaoticVoid,
            new EffectDescriptor(
                "Chaotic Void",
                $"Each time this creep takes damage, it gains {TraitConstants.ChaoticVoidManaPerDamageInstance} mana. At 100% mana, it heals for {TraitConstants.ChaoticVoidHealPercentageOfMaxHealth} of its maximum health, then its mana is reset to 0."
            )
        },
        {
            TraitType.Bombardment,
            new EffectDescriptor(
                "Bombardment",
                $"Sends a rocket toward a tower within a {TraitConstants.BombardmentRange} radius every {TraitConstants.BombardmentRocketInterval} seconds, dealing {TraitConstants.BombardmentRocketDamage} damage."
            )
        },
        {
            TraitType.GeomancyAura1,
            new EffectDescriptor(
                "Geomancy Aura (1)",
                $"Applies {EffectDescriptor.BI} to all creeps within a {TraitConstants.GeomancyAura1Range} radius.",
                BuffType.Geomancy1
            )
        },
        {
            TraitType.GeomancyAura2,
            new EffectDescriptor(
                "Geomancy Aura (2)",
                $"Applies {EffectDescriptor.BI} to all creeps within a {TraitConstants.GeomancyAura2Range} radius.",
                BuffType.Geomancy2
            )
        },
        {
            TraitType.NecroticTransfusion,
            new EffectDescriptor(
                "Necrotic Transfusion",
                $"When killed, targets a creep within {TraitConstants.NecroticTransfusionRange} radius to apply one stack of {EffectDescriptor.BI}.",
                BuffType.NecroticTransfusion
            )
        },
        {
            TraitType.Skittering,
            new EffectDescriptor(
                "Skittering",
                $"This creep won't draw attention from towers and will always be attacked as a last priority."
            )
        },
        {
            TraitType.EtherealAura,
            new EffectDescriptor(
                "Ethereal Aura",
                $"Applies one stack of {EffectDescriptor.BI} to all creeps within a {TraitConstants.EtherealAuraRange} radius every {TraitConstants.EtherealAuraStackApplicationInterval} seconds.",
                BuffType.Ethereal
            )
        },
        {
            TraitType.HardenedSkin,
            new EffectDescriptor(
                "Hardened Skin",
                $"Spawns with {TraitConstants.HardenedSkinStartingStacks} stacks of {EffectDescriptor.BI}.",
                BuffType.HardenedSkin
            )
        },
        {
            TraitType.EarthShieldProvider,
            new EffectDescriptor(
                "Earth Shield Provider",
                $"Applies {EffectDescriptor.BI} to one creep within a {TraitConstants.EarthShieldProviderRange} radius every {TraitConstants.EarthShieldProviderCooldown} seconds."
            )
        },
        {
            TraitType.ChaosPortal,
            new EffectDescriptor(
                "Chaos Portal",
                $"Channels energy towards another Chaos Warden with the highest current mana within a {TraitConstants.ChaosPortalRange} radius, instantly teleporting to its location.{EffectDescriptor.NL}{EffectDescriptor.NL}Casts once after {TraitConstants.ChaosPortalFirstCastTimer}, once after {TraitConstants.ChaosPortalSecondCastTimer}, then once every {TraitConstants.ChaosPortalRecurringCastInterval} seconds."
            )
        },
        {
            TraitType.ChaosEmpowermentAura,
            new EffectDescriptor(
                "Chaos Empowerment Aura",
                $"Applies {EffectDescriptor.BI} to all creeps within a {TraitConstants.ChaosEmpowermentAuraRange} radius."
            )
        },
        {
            TraitType.StoneskinFortitude,
            new EffectDescriptor(
                "Stoneskin Fortitude",
                $"Immune to all harmful movement-slowing effects."
            )
        },
        {
            TraitType.GoblinEngineering,
            new EffectDescriptor(
                "Goblin Engineering",
                $"Can't be beyond {TraitConstants.GoblinEngineeringMinBaseMovementSpeed * 100}% of its base movement speed and the durations of all harmful spell effects are reduced by {TraitConstants.GoblinEngineeringHarmfulEffectsDurationMultiplier * 100}%."
            )
        },
        {
            TraitType.ReactiveArmor,
            new EffectDescriptor(
                "Reactive Armor",
                $"Reduces excessive damage taken above {TraitConstants.ReactiveArmorFirstThreshold} by {(1 - TraitConstants.ReactiveArmorFirstThresholdMultiplier) * 100} and damage taken above {TraitConstants.ReactiveArmorSecondThreshold} by {(1 - TraitConstants.ReactiveArmorSecondThresholdMultiplier) * 100}%."
            )
        },
        {
            TraitType.EngineOverload,
            new EffectDescriptor(
                "Engine Overload",
                $"At {string.Join("% and ", TraitConstants.EngineOverloadHealthPercentageTriggers)}% health, a static burst triggers, reducing the attack speed of towers within a {TraitConstants.EngineOverloadRange} radius by {(1 - TraitConstants.EngineOverloadAttackSpeedMultiplier) * 100}% for {TraitConstants.EngineOverloadDuration} seconds."
            )
        },
        {
            TraitType.Hypothermia,
            new EffectDescriptor(
                "Hypothermia",
                $"Harmful movement-slowing effects applied to this creep are {TraitConstants.HypothermiaSlowEffectEffectivenessMultiplier * 100}% more effective and this creep takes 1% additional damage for every 1% its effective movement speed is below its base movement speed."
            )
        },
        {
            TraitType.WarStance,
            new EffectDescriptor(
                "War Stance",
                $"At {TraitConstants.WarStanceActivationHealthRatio * 100}% health, gains +{TraitConstants.WarStanceArmorBonus} and an aura applying {EffectDescriptor.BI} to all creeps within a {TraitConstants.WarCryAuraRange} range.",
                BuffType.WarCry
            )
        },
        {
            TraitType.VolatileDeath,
            new EffectDescriptor(
                "Volatile Death",
                $"Deals up to {TraitConstants.VolatileDeathMaxDamage} to towers within a {TraitConstants.VolatileDeathRange} when killed."
            )
        },
        
        // Towers
        {
            TraitType.Arcanize1,
            new EffectDescriptor(
                "Arcanize (1)",
                $"Gains 1 mana for each attack. At maximum mana capacity, damage dealt is increased by {(TraitConstants.Arcanize1MaximumManaDamageMultiplier - 1) * 100}%."
            )
        },
        {
            TraitType.Arcanize2,
            new EffectDescriptor(
                "Arcanize (2)",
                $"Gains 1 mana for each attack. At maximum mana capacity, damage dealt is increased by {(TraitConstants.Arcanize2MaximumManaDamageMultiplier - 1) * 100}%."
            )
        },
        {
            TraitType.Spellcaster1,
            new EffectDescriptor(
                "Spellcaster (1)",
                $""
            )
        },
        {
            TraitType.Spellcaster2,
            new EffectDescriptor(
                "Spellcaster (2)",
                $""
            )
        },
        {
            TraitType.KirinTorMastery,
            new EffectDescriptor(
                "Kirin Tor Mastery",
                $""
            )
        },
        {
            TraitType.VolatileArcane1,
            new EffectDescriptor(
                "Volatile Arcane (1)",
                $"Attacks bounce up to {TraitConstants.VolatileArcane1MaxBounces} times to creeps within a {TraitConstants.VolatileArcane1BounceRange} radius. Each bounce grants 1 mana. For every 1% mana, base attack speed is lowered by {TraitConstants.VolatileArcane1AttackSpeedDropoff * 100}%, up to {TraitConstants.VolatileArcane1MaxAttackSpeedDecrease * 100}.{EffectDescriptor.NL}{TraitConstants.VolatileArcane1SpellDamagePercentage * 100}% of the damage is dealt as spell damage.{EffectDescriptor.NL}Mana decreases by {TraitConstants.VolatileArcane1ManaLossPerSecond} every second."
            )
        },
        {
            TraitType.VolatileArcane2,
            new EffectDescriptor(
                "Volatile Arcane (2)",
                $"Attacks bounce up to {TraitConstants.VolatileArcane2MaxBounces} times to creeps within a {TraitConstants.VolatileArcane2BounceRange} radius. Each bounce grants 1 mana. For every 1% mana, base attack speed is lowered by {TraitConstants.VolatileArcane2AttackSpeedDropoff * 100}%, up to {TraitConstants.VolatileArcane2MaxAttackSpeedDecrease * 100}.{EffectDescriptor.NL}{TraitConstants.VolatileArcane2SpellDamagePercentage * 100}% of the damage is dealt as spell damage.{EffectDescriptor.NL}Mana decreases by {TraitConstants.VolatileArcane2ManaLossPerSecond} every second."
            )
        },
        {
            TraitType.OverwhelmingArcane,
            new EffectDescriptor(
                "Overwhelming Arcane",
                $""
            )
        },
        {
            TraitType.Ignite1,
            new EffectDescriptor(
                "Ignite (1)",
                $""
            )
        },
        {
            TraitType.Ignite2,
            new EffectDescriptor(
                "Ignite (2)",
                $""
            )
        },
        {
            TraitType.OverwhelmingImpact1,
            new EffectDescriptor(
                "Overwhelming Impact (1)",
                $""
            )
        },
        {
            TraitType.OverwhelmingImpact2,
            new EffectDescriptor(
                "Overwhelming Impact (2)",
                $""
            )
        },
        {
            TraitType.VoidFlare,
            new EffectDescriptor(
                "Void Flare",
                $""
            )
        },
        {
            TraitType.RisingHeat1,
            new EffectDescriptor(
                "Rising Heat (1)",
                $""
            )
        },
        {
            TraitType.RisingHeat2,
            new EffectDescriptor(
                "Rising Heat (2)",
                $""
            )
        },
        {
            TraitType.VolcanicEruption,
            new EffectDescriptor(
                "Volcanic Eruption",
                $""
            )
        },
        {
            TraitType.FrostAttack1,
            new EffectDescriptor(
                "Frost Attack (1)",
                $""
            )
        },
        {
            TraitType.FrostAttack2,
            new EffectDescriptor(
                "Frost Attack (2)",
                $""
            )
        },
        {
            TraitType.FrostBlast1,
            new EffectDescriptor(
                "Frost Blast (1)",
                $""
            )
        },
        {
            TraitType.FrostBlast2,
            new EffectDescriptor(
                "Frost Blast (2)",
                $""
            )
        },
        {
            TraitType.ChillingDeath,
            new EffectDescriptor(
                "Chilling Death",
                $""
            )
        },
        {
            TraitType.IceLance1,
            new EffectDescriptor(
                "Ice Lance (1)",
                $""
            )
        },
        {
            TraitType.IceLance2,
            new EffectDescriptor(
                "Ice Lance (2)",
                $""
            )
        },
        {
            TraitType.CrystallizedLight,
            new EffectDescriptor(
                "Crystallized Light",
                $""
            )
        },
        {
            TraitType.ShatterArmor1,
            new EffectDescriptor(
                "Shatter Armor (1)",
                $""
            )
        },
        {
            TraitType.ShatterArmor2,
            new EffectDescriptor(
                "Shatter Armor (2)",
                $""
            )
        },
        {
            TraitType.DevastatingAttack1,
            new EffectDescriptor(
                "Devastating Attack (1)",
                $""
            )
        },
        {
            TraitType.DevastatingAttack2,
            new EffectDescriptor(
                "Devastating Attack (2)",
                $""
            )
        },
        {
            TraitType.NaturesGuidance,
            new EffectDescriptor(
                "Nature's Guidance",
                $""
            )
        },
        {
            TraitType.Germination1,
            new EffectDescriptor(
                "Germinate (1)",
                $""
            )
        },
        {
            TraitType.Germination2,
            new EffectDescriptor(
                "Germinate (2)",
                $""
            )
        },
        {
            TraitType.LethalPreparation,
            new EffectDescriptor(
                "Opportunistic",
                $""
            )
        },
        {
            TraitType.Overcharge1,
            new EffectDescriptor(
                "Overcharge (1)",
                $""
            )
        },
        {
            TraitType.Overcharge2,
            new EffectDescriptor(
                "Overcharge (2)",
                $""
            )
        },
        {
            TraitType.FocusedLightning1,
            new EffectDescriptor(
                "Focused Lightning (1)",
                $""
            )
        },
        {
            TraitType.FocusedLightning2,
            new EffectDescriptor(
                "Focused Lightning (2)",
                $""
            )
        },
        {
            TraitType.Annihilation,
            new EffectDescriptor(
                "Annihilation",
                $""
            )
        },
        {
            TraitType.TurbulentWeather1,
            new EffectDescriptor(
                "Turbulent Weather (1)",
                $""
            )
        },
        {
            TraitType.TurbulentWeather2,
            new EffectDescriptor(
                "Turbulent Weather (2)",
                $""
            )
        },
        {
            TraitType.TurbulentWeather3,
            new EffectDescriptor(
                "Turbulent Weather (3)",
                $""
            )
        },
        {
            TraitType.StaticCharge1,
            new EffectDescriptor(
                "Static Charge (1)",
                $""
            )
        },
        {
            TraitType.StaticCharge2,
            new EffectDescriptor(
                "Static Charge (2)",
                $""
            )
        },
        {
            TraitType.Radiance,
            new EffectDescriptor(
                "Radiance",
                $""
            )
        },
        {
            TraitType.CrushingWave1,
            new EffectDescriptor(
                "Crushing Wave (1)",
                $""
            )
        },
        {
            TraitType.CrushingWave2,
            new EffectDescriptor(
                "Crushing Wave (2)",
                $""
            )
        },
        {
            TraitType.PressuringWater1,
            new EffectDescriptor(
                "Pressuring Water (1)",
                $""
            )
        },
        {
            TraitType.PressuringWater2,
            new EffectDescriptor(
                "Pressuring Water (2)",
                $""
            )
        },
        {
            TraitType.HurricaneStorm,
            new EffectDescriptor(
                "Hurricane Storm",
                $""
            )
        },
        {
            TraitType.Torrent1,
            new EffectDescriptor(
                "Torrent (1)",
                $""
            )
        },
        {
            TraitType.Torrent2,
            new EffectDescriptor(
                "Torrent (2)",
                $""
            )
        },
        {
            TraitType.CripplingDecay,
            new EffectDescriptor(
                "Crippling Decay",
                $""
            )
        },
        {
            TraitType.BurstingLight1,
            new EffectDescriptor(
                "Bursting Light (1)",
                $""
            )
        },
        {
            TraitType.BurstingLight2,
            new EffectDescriptor(
                "Bursting Light (2)",
                $""
            )
        },
        {
            TraitType.LightBurst1,
            new EffectDescriptor(
                "Light Burst (1)",
                $""
            )
        },
        {
            TraitType.LightBurst2,
            new EffectDescriptor(
                "Light Burst (2)",
                $""
            )
        },
        {
            TraitType.DivineSpores,
            new EffectDescriptor(
                "Divine Spores",
                $""
            )
        },
        {
            TraitType.BlindingLight1,
            new EffectDescriptor(
                "Blinding Light (1)",
                $""
            )
        },
        {
            TraitType.BlindingLight2,
            new EffectDescriptor(
                "Blinding Light (2)",
                $""
            )
        },
        {
            TraitType.TitanDefenseMechanism,
            new EffectDescriptor(
                "Titan Defense Mechanism",
                $""
            )
        },
        {
            TraitType.Corruption1,
            new EffectDescriptor(
                "Corruption (1)",
                $""
            )
        },
        {
            TraitType.Corruption2,
            new EffectDescriptor(
                "Corruption (2)",
                $""
            )
        },
        {
            TraitType.RapidInfection1,
            new EffectDescriptor(
                "Rapid Infection (1)",
                $""
            )
        },
        {
            TraitType.RapidInfection2,
            new EffectDescriptor(
                "Rapid Infection (2)",
                $""
            )
        },
        {
            TraitType.Pestilence,
            new EffectDescriptor(
                "Pestilence",
                $""
            )
        },
        {
            TraitType.UnholyMiasma1,
            new EffectDescriptor(
                "Unholy Miasma (1)",
                $""
            )
        },
        {
            TraitType.UnholyMiasma2,
            new EffectDescriptor(
                "Unholy Miasma (2)",
                $""
            )
        },
        {
            TraitType.Hellfire,
            new EffectDescriptor(
                "Hellfire",
                $""
            )
        },
        {
            TraitType.VoidGrowth1,
            new EffectDescriptor(
                "Void Growth (1)",
                $""
            )
        },
        {
            TraitType.VoidGrowth2,
            new EffectDescriptor(
                "Void Growth (2)",
                $""
            )
        },
        {
            TraitType.TemporalRift1,
            new EffectDescriptor(
                "Temporal Rift (1)",
                $""
            )
        },
        {
            TraitType.TemporalRift2,
            new EffectDescriptor(
                "Temporal Rift (2)",
                $""
            )
        },
        {
            TraitType.ImplosionRift,
            new EffectDescriptor(
                "Implosion Rift",
                $""
            )
        },
        {
            TraitType.VoidLashing1,
            new EffectDescriptor(
                "Void Lashing (1)",
                $""
            )
        },
        {
            TraitType.VoidLashing2,
            new EffectDescriptor(
                "Void Lashing (2)",
                $""
            )
        },
        {
            TraitType.HungeringVoid,
            new EffectDescriptor(
                "Hungering Void",
                $""
            )
        },
        
        // Discs
        
        {
            TraitType.Inactive,
            new EffectDescriptor(
                "Inactive",
                $""
            )
        },
        {
            TraitType.ArcaneReaction1,
            new EffectDescriptor(
                "Arcane Reaction",
                $""
            )
        },
        {
            TraitType.ArcaneReaction2,
            new EffectDescriptor(
                "Advanced Arcane Reaction",
                $""
            )
        },
        {
            TraitType.ExplosiveReaction1,
            new EffectDescriptor(
                "Explosive Reaction",
                $""
            )
        },
        {
            TraitType.ExplosiveReaction2,
            new EffectDescriptor(
                "Advanced Explosive Reaction",
                $""
            )
        },
        {
            TraitType.EssenceOfNatureAura1,
            new EffectDescriptor(
                "Essence of Nature Aura",
                $"Applies {EffectDescriptor.BI} to all towers within a {TraitConstants.EssenceOfNatureAura1Range} radius.",
                BuffType.EssenceOfNature1
            )
        },
        {
            TraitType.EssenceOfNatureAura2,
            new EffectDescriptor(
                "Advanced Essence of Nature Aura",
                $"Applies {EffectDescriptor.BI} to all towers within a {TraitConstants.EssenceOfNatureAura2Range} radius.",
                BuffType.EssenceOfNature2
            )
        },
        {
            TraitType.EssenceOfFrostAura1,
            new EffectDescriptor(
                "Essence of Frost Aura",
                $"Applies {EffectDescriptor.BI} to all towers within a {TraitConstants.EssenceOfFrostAura1Range} radius.",
                BuffType.EssenceOfFrost1
            )
        },
        {
            TraitType.EssenceOfFrostAura2,
            new EffectDescriptor(
                "Advanced Essence of Frost Aura",
                $"Applies {EffectDescriptor.BI} to all towers within a {TraitConstants.EssenceOfFrostAura2Range} radius.",
                BuffType.EssenceOfFrost2

            )
        },
        {
            TraitType.EssenceOfPowerAura1,
            new EffectDescriptor(
                "Essence of Power Aura",
                $""
            )
        },
        {
            TraitType.EssenceOfPowerAura2,
            new EffectDescriptor(
                "Advanced Essence of Power Aura",
                $""
            )
        },
        {
            TraitType.EssenceOfTheSeaAura1,
            new EffectDescriptor(
                "Essence of the Sea Aura",
                $""
            )
        },
        {
            TraitType.EssenceOfTheSeaAura2,
            new EffectDescriptor(
                "Advanced Essence of the Sea Aura",
                $""
            )
        },
        {
            TraitType.EssenceOfLightAura1,
            new EffectDescriptor(
                "Essence of Light Aura",
                $""
            )
        },
        {
            TraitType.EssenceOfLightAura2,
            new EffectDescriptor(
                "Advanced Essence of Light Aura",
                $""
            )
        },
        {
            TraitType.EssenceOfBlightAura1,
            new EffectDescriptor(
                "Essence of Blight Aura",
                $""
            )
        },
        {
            TraitType.EssenceOfBlightAura2,
            new EffectDescriptor(
                "Advanced Essence of Blight Aura",
                $""
            )
        },
        {
            TraitType.EssenceOfDarknessAura1,
            new EffectDescriptor(
                "Essence of Darkness Aura",
                $""
            )
        },
        {
            TraitType.EssenceOfDarknessAura2,
            new EffectDescriptor(
                "Advanced Essence of Darkness Aura",
                $""
            )
        },
    };
}