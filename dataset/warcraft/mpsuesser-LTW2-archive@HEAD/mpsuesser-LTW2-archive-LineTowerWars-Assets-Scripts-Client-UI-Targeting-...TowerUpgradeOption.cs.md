---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Targeting/Options/TowerUpgrades/TowerUpgradeOption.cs"
ref: HEAD
---

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerUpgradeOption : HoverTooltip
{
    public static TowerUpgradeOption CreateWithParent(TowerUpgrade upgrade, Transform parent) {
        TowerUpgradeOption option = Instantiate(ClientPrefabs.Singleton.pfTowerUpgradeOption, parent);
        option.Load(upgrade);
        return option;
    }

    private TowerUpgrade UpgradeData { get; set; }

    private Image Icon { get; set; }
    private Button Btn { get; set; }

    private void Init() {
        Icon = GetComponentInChildren<Image>(true);
        Btn = GetComponentInChildren<Button>(true);

        Btn.onClick.AddListener(UpgradePressed);
    }

    private void Load(TowerUpgrade upgrade) {
        Init();

        UpgradeData = upgrade;

        Icon.sprite = ClientResources.Singleton.GetSpriteForTowerType(upgrade.TargetTowerType);
    }

    private void UpgradePressed() {
        LTWLogger.Log($"Upgrade pressed! source={UpgradeData.SourceTowerType}, target={UpgradeData.TargetTowerType}, cost={UpgradeData.Cost}, requirements={UpgradeData.RequiredTech.Length}");

        EventBus.UpgradePressed(UpgradeData);
    }

    private static readonly HashSet<char> Vowels = new HashSet<char>() { 'A', 'E', 'I', 'O', 'U', 'a', 'e', 'i', 'o', 'u' };
    protected override Tooltip GetTooltipContent() {
        string towerName = TowerConstants.DisplayName[UpgradeData.TargetTowerType];
        bool startsWithVowel = towerName.Length > 0 && Vowels.Contains(towerName[0]);

        string requirementText = "";
        if (UpgradeData.RequiredTech.Length > 0) {
            requirementText = "Requires: ";
            for (int i = 0; i < UpgradeData.RequiredTech.Length; i++) {
                if (i != 0) {
                    requirementText += ',';
                }

                requirementText += UpgradeData.RequiredTech[i];
            }
        }

        return new StandardTooltip(
            $"Upgrade: {towerName}",
            $"{UpgradeData.Cost} gold",
            $"-",
            $"Transform this tower into {(startsWithVowel ? "an" : "a")} {towerName} over {UpgradeData.Duration} seconds.",
            requirementText
        );
    }
}
