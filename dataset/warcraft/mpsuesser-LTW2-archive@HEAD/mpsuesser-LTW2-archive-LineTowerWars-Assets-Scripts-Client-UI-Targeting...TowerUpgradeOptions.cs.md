---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Targeting/Options/TowerUpgrades/TowerUpgradeOptions.cs"
ref: HEAD
---

using System;
using System.Collections.Generic;
using UnityEngine;

public class TowerUpgradeOptions : MonoBehaviour
{
    public void LoadForEntity(ClientEntity e) {
        ClearExistingOptions();

        if (!(e is ClientTower t)) {
            return;
        }

        List<TowerUpgrade> availableUpgrades =
            TowerUpgrades.Singleton.GetAvailableUpgradesForTowerType(t.Type);
        foreach (TowerUpgrade availableUpgrade in availableUpgrades) {
            TowerUpgradeOption.CreateWithParent(availableUpgrade, transform);
        }
    }

    private void ClearExistingOptions() {
        foreach (Transform t in transform) {
            Destroy(t.gameObject);
        }
    }

    private void UpgradePressed(TowerUpgrade upgrade) {
        LTWLogger.Log("DEPRECATED!");
    }
}
