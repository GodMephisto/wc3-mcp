---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/HoverTooltip/Types/TowerTooltip.cs"
ref: HEAD
---

public class TowerTooltip : Tooltip {
    public string Name { get; }
    public int AccGoldCost { get; }
    public double Range { get; }
    public double AttackSpeed { get; }
    public int HP { get; }
    public double MinDamage { get; }
    public double MaxDamage { get; }
    public string DamageType { get; }

    public TowerTooltip(TowerType towerType) : base() {
        Name = TowerConstants.DisplayName[towerType];
        AccGoldCost = 1; // TODO
        Range = TowerConstants.Range[towerType];
        AttackSpeed = 1 / TowerConstants.Cooldown[towerType];
        HP = TowerConstants.HP[towerType];
        (MinDamage, MaxDamage) = TowerConstants.DamageRange[towerType];
        DamageType = TowerConstants.AttackModifier[towerType].ToString();
    }
}