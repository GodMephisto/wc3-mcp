---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/shared/Data/TowerAttackScheme.cs"
ref: HEAD
---

﻿public class TowerAttackScheme {
    public AttackDeliveryModifiers Delivery { get; private set; }

    public double Range { get; private set; }
    public double Cooldown { get; private set; }
    public (double, double) DamageRange { get; private set; }
    public AttackType Modifier { get; private set; }

    public TowerAttackScheme(TowerType towerType) {
        Delivery = TowerConstants.AttackDelivery[towerType];
        Range = TowerConstants.Range[towerType];
        Cooldown = TowerConstants.Cooldown[towerType];
        DamageRange = TowerConstants.DamageRange[towerType];
        Modifier = TowerConstants.AttackModifier[towerType];
    }
}