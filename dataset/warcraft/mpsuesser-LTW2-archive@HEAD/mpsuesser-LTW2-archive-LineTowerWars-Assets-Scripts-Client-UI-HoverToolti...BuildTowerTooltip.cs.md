---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/HoverTooltip/Types/BuildTowerTooltip.cs"
ref: HEAD
---

﻿public class BuildTowerTooltip : Tooltip {
    public string Title { get; }
    public int GoldCost { get; }
    public int UpgradePaths { get; }
    public double Range { get; }
    public double AttackSpeed { get; }
    public int HP { get; }
    public double MinDamage { get; }
    public double MaxDamage { get; }
    public string DamageType { get; }

    public BuildTowerTooltip(TowerType towerType) : base() {
        Title = $"Build: {TowerConstants.DisplayName[towerType]}";
        GoldCost = TowerConstants.BuildCost[towerType];
        UpgradePaths = TowerUpgrades.Singleton.GetAvailableUpgradesForTowerType(towerType).Count;
        Range = TowerConstants.Range[towerType];
        AttackSpeed = 1 / TowerConstants.Cooldown[towerType];
        HP = TowerConstants.HP[towerType];
        (MinDamage, MaxDamage) = TowerConstants.DamageRange[towerType];
        DamageType = TowerConstants.AttackModifier[towerType].ToString();
    }
}