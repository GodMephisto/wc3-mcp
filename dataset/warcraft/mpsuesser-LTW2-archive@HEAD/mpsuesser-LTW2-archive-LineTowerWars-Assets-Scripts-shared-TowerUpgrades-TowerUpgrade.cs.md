---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/TowerUpgrades/TowerUpgrade.cs"
ref: HEAD
---

﻿using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TowerUpgradeName", menuName = "ScriptableObjects/TowerUpgrade", order = 1)]
public class TowerUpgrade : ScriptableObject {
    [SerializeField] public TowerType SourceTowerType;
    [SerializeField] public TowerType TargetTowerType;

    [Space(5)]
    [SerializeField] public int Cost;

    [Space(5)]
    [SerializeField] public double Duration;

    [Space(5)]
    [SerializeField] public ElementalTechType[] RequiredTech;

    public HashSet<ElementalTechType> RequiredTechSet { get; private set; }
    
    private void OnValidate() {
        RequiredTechSet = new HashSet<ElementalTechType>(RequiredTech);
    }
}