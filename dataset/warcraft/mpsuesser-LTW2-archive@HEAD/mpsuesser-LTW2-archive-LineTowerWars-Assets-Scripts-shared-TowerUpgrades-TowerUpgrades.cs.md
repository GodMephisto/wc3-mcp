---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/TowerUpgrades/TowerUpgrades.cs"
ref: HEAD
---

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerUpgrades : SingletonBehaviour<TowerUpgrades>
{
    private Dictionary<TowerType, List<TowerUpgrade>> UpgradesAvailableToTower { get; set; }
    public HashSet<TowerUpgrade> AllUpgrades { get; private set; }
    
    private void Awake() {
        InitializeSingleton(this);

        LoadTowerUpgrades();
    }

    public List<TowerUpgrade> GetAvailableUpgradesForTowerType(TowerType towerType) {
        if (UpgradesAvailableToTower.ContainsKey(towerType)) {
            return UpgradesAvailableToTower[towerType];
        }

        return new List<TowerUpgrade>();
    }

    private void LoadTowerUpgrades() {
        UpgradesAvailableToTower = new Dictionary<TowerType, List<TowerUpgrade>>();
        TowerUpgrade[] upgrades = Resources.LoadAll<TowerUpgrade>("shared/TowerUpgrades");
        AllUpgrades = new HashSet<TowerUpgrade>(upgrades);
        foreach (TowerUpgrade upgrade in upgrades) {
            if (!UpgradesAvailableToTower.ContainsKey(upgrade.SourceTowerType)) {
                UpgradesAvailableToTower[upgrade.SourceTowerType] = new List<TowerUpgrade>();
            }

            UpgradesAvailableToTower[upgrade.SourceTowerType].Add(upgrade);
        }
    }

    public List<TowerType> GetAllTowersAssociatedWithElementalTechType(ElementalTechType elementalTechType) {
        List<TowerType> associatedTowers = new List<TowerType>();
        foreach (TowerUpgrade upgrade in AllUpgrades) {
            if (upgrade.RequiredTechSet.Contains(elementalTechType)) {
                associatedTowers.Add(upgrade.TargetTowerType);
            }
        }

        return associatedTowers;
    }

    public TowerUpgrade GetUpgradeBySourceAndTargetTowerType(TowerType sourceTowerType, TowerType targetTowerType) {
        foreach (TowerUpgrade upgrade in AllUpgrades) {
            if (upgrade.SourceTowerType == sourceTowerType && upgrade.TargetTowerType == targetTowerType) {
                return upgrade;
            }
        }

        throw new ResourceNotFoundException(
            $"There was no upgrade with source tower type {sourceTowerType} and target tower type {targetTowerType}");
    }
}
