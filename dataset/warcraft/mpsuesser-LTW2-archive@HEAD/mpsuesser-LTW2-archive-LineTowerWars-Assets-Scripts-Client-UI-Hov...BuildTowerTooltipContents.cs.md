---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/HoverTooltip/Contents/BuildTowerTooltipContents.cs"
ref: HEAD
---

﻿using UnityEngine;
using TMPro;
using System;

public class BuildTowerTooltipContents : MonoBehaviour {
    [SerializeField] private TMP_Text Title;
    [SerializeField] private TMP_Text Cost;
    [SerializeField] private TMP_Text UpgradePaths;
    [SerializeField] private TMP_Text Range;
    [SerializeField] private TMP_Text AttackSpeed;
    [SerializeField] private TMP_Text DamageRange;
    [SerializeField] private TMP_Text DamageType;

    public void Load(BuildTowerTooltip tooltip) {
        Title.SetText(tooltip.Title);
        Cost.SetText(tooltip.GoldCost.ToString());
        UpgradePaths.SetText(tooltip.UpgradePaths.ToString()); // TODO: Make this more useful
        Range.SetText($"{tooltip.Range:0.#}");
        AttackSpeed.SetText($"{tooltip.AttackSpeed:0.#}");
        DamageRange.SetText($"{tooltip.MinDamage:0.#} - {tooltip.MaxDamage:0.#}");
        DamageType.SetText(tooltip.DamageType);
    }
}