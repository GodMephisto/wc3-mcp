---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/HoverTooltip/Types/SendUnitTooltip.cs"
ref: HEAD
---

﻿public class SendUnitTooltip : Tooltip
{
    public string Name { get; set; }
    public int GoldCost { get; set; }

    public double MaxHP { get; set; }
    public double MoveSpeed { get; set; }
    public string ArmorType { get; set; }

    public int IncomeReward { get; set; }
    public int Bounty { get; set; }
    public string SpecialModifiers { get; set;  }

    public SendUnitTooltip(EnemyType enemyType) : base() {
        Name = $"Send: {EnemyConstants.DisplayName[enemyType]}";
        GoldCost = EnemyConstants.GoldCost[enemyType];

        MaxHP = EnemyConstants.HP[enemyType];
        MoveSpeed = EnemyConstants.MoveSpeed[enemyType];
        ArmorType = EnemyConstants.ArmorModifier[enemyType].ToString();

        IncomeReward = EnemyConstants.IncomeReward[enemyType];
        Bounty = EnemyConstants.KillReward[enemyType];
        SpecialModifiers = ""; // TODO
    }
}