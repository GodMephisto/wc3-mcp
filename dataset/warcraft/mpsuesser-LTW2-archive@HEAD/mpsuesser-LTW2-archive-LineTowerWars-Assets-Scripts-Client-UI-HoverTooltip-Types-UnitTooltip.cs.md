---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/HoverTooltip/Types/UnitTooltip.cs"
ref: HEAD
---

public class UnitTooltip : Tooltip
{
    public string Name { get; set; }

    public double MaxHP { get; set; }
    public double MoveSpeed { get; set; }
    public string ArmorType { get; set; }

    public int GoldCost { get; set; }
    public int IncomeReward { get; set; }
    public int Bounty { get; set; }

    public UnitTooltip(EnemyType enemyType) : base() {
        Name = EnemyConstants.DisplayName[enemyType];

        MaxHP = EnemyConstants.HP[enemyType];
        MoveSpeed = EnemyConstants.MoveSpeed[enemyType];
        ArmorType = EnemyConstants.ArmorModifier[enemyType].ToString();

        GoldCost = EnemyConstants.GoldCost[enemyType];
        IncomeReward = EnemyConstants.IncomeReward[enemyType];
        Bounty = EnemyConstants.KillReward[enemyType];
    }
}