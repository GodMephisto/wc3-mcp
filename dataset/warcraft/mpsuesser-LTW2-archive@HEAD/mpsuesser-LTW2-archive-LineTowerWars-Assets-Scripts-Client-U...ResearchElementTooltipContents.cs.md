---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/HoverTooltip/Contents/ResearchElementTooltipContents.cs"
ref: HEAD
---

﻿using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class ResearchElementTooltipContents : MonoBehaviour {
    [SerializeField] private TMP_Text Title;
    [SerializeField] private TMP_Text Cost;
    [SerializeField] private TMP_Text AssociatedTowers;

    public void Load(ResearchElementTooltip tooltip) {
        Title.SetText(tooltip.Title);
        Cost.SetText(tooltip.GoldCost.ToString());

        List<string> towerDisplayNames = new List<string>();
        foreach (TowerType associatedTower in tooltip.AssociatedTowers) {
            towerDisplayNames.Add(TowerConstants.DisplayName[associatedTower]);
        }
        
        string listOfTowers = string.Join(", ", towerDisplayNames);
        AssociatedTowers.SetText(listOfTowers);
    }
}