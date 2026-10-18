---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/DamageRelationships/DamageRelationships.cs"
ref: HEAD
---

using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageRelationships : SingletonBehaviour<DamageRelationships> {
    private static readonly HashSet<(AttackType, ArmorType, double)> Relationships =
        new HashSet<(AttackType, ArmorType, double)>() {
            (AttackType.Flux, ArmorType.Light, 1.25),
            (AttackType.Flux, ArmorType.Medium, .80),
            (AttackType.Flux, ArmorType.Heavy, 1.50),
            (AttackType.Flux, ArmorType.Fortified, .66),
            (AttackType.Flux, ArmorType.Hero, .66),
            (AttackType.Flux, ArmorType.Unarmored, 1.00),
            (AttackType.Chaos, ArmorType.Light, 1.00),
            (AttackType.Chaos, ArmorType.Medium, 1.00),
            (AttackType.Chaos, ArmorType.Heavy, 1.00),
            (AttackType.Chaos, ArmorType.Fortified, 1.00),
            (AttackType.Chaos, ArmorType.Hero, 1.00),
            (AttackType.Chaos, ArmorType.Unarmored, 1.00),
            (AttackType.Normal, ArmorType.Light, .80),
            (AttackType.Normal, ArmorType.Medium, 1.50),
            (AttackType.Normal, ArmorType.Heavy, 1.00),
            (AttackType.Normal, ArmorType.Fortified, .80),
            (AttackType.Normal, ArmorType.Hero, .80),
            (AttackType.Normal, ArmorType.Unarmored, 1.00),
            (AttackType.Piercing, ArmorType.Light, 1.50),
            (AttackType.Piercing, ArmorType.Medium, 1.00),
            (AttackType.Piercing, ArmorType.Heavy, .80),
            (AttackType.Piercing, ArmorType.Fortified, .66),
            (AttackType.Piercing, ArmorType.Hero, .66),
            (AttackType.Piercing, ArmorType.Unarmored, 1.50),
            (AttackType.Siege, ArmorType.Light, 1.00),
            (AttackType.Siege, ArmorType.Medium, .80),
            (AttackType.Siege, ArmorType.Heavy, .80),
            (AttackType.Siege, ArmorType.Fortified, 1.50),
            (AttackType.Siege, ArmorType.Hero, .80),
            (AttackType.Siege, ArmorType.Unarmored, 1.20),
        };
    
    private Dictionary<AttackType, Dictionary<ArmorType, double>> AttackToArmorRelationships { get; set; }
    private Dictionary<ArmorType, Dictionary<AttackType, double>> ArmorToAttackRelationships { get; set; }

    private void Awake() {
        InitializeSingleton(this);

        InitDicts();
    }

    private void InitDicts() {
        AttackToArmorRelationships = new Dictionary<AttackType, Dictionary<ArmorType, double>>();
        ArmorToAttackRelationships = new Dictionary<ArmorType, Dictionary<AttackType, double>>();
        
        foreach (AttackType attackType in Enum.GetValues(typeof(AttackType))) {
            AttackToArmorRelationships[attackType] = new Dictionary<ArmorType, double>();
        }
        foreach (ArmorType armorType in Enum.GetValues(typeof(ArmorType))) {
            ArmorToAttackRelationships[armorType] = new Dictionary<AttackType, double>();
        }
        
        foreach (
            (AttackType attackType, ArmorType armorType, double modifier)
            in Relationships
        ) {
            AttackToArmorRelationships[attackType][armorType] = modifier;
            ArmorToAttackRelationships[armorType][attackType] = modifier;
        }
    }

    public double GetModifierForRelationship(AttackType attackType, ArmorType armorType) {
        if (AttackToArmorRelationships[attackType].TryGetValue(armorType, out double modifier)) {
            return modifier;
        }

        return 1.0;
    }
}
