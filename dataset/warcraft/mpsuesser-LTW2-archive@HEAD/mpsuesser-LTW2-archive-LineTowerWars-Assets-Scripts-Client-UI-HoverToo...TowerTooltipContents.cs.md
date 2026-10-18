---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/HoverTooltip/Contents/TowerTooltipContents.cs"
ref: HEAD
---

using UnityEngine;
using TMPro;
using System;

public class TowerTooltipContents : MonoBehaviour {
    [SerializeField] private TMP_Text Name;
    [SerializeField] private TMP_Text Cost;
    [SerializeField] private TMP_Text Range;
    [SerializeField] private TMP_Text AttackSpeed;
    [SerializeField] private TMP_Text HP;
    [SerializeField] private TMP_Text DamageRange;
    [SerializeField] private TMP_Text DamageType;

    public void Load(TowerTooltip tooltip) {
        Name.SetText(tooltip.Name);
        Cost.SetText(tooltip.AccGoldCost.ToString());
        Range.SetText(String.Format("{0:0.#}", tooltip.Range));
        AttackSpeed.SetText(String.Format("{0:0.#}", tooltip.AttackSpeed));
        HP.SetText(tooltip.HP.ToString());
        DamageRange.SetText($"{String.Format("{0:0.#}", tooltip.MinDamage)} - {String.Format("{0:0.#}", tooltip.MaxDamage)}");
        DamageType.SetText(tooltip.DamageType);
    }
}
