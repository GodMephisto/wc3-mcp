---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/Targeting/Options/TowerBaseAbilities/TowerSellOption.cs"
ref: HEAD
---

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TowerSellOption : HoverTooltip
{
    private Button Btn { get; set; }

    protected override void Awake() {
        Btn = GetComponentInChildren<Button>(true);
        Btn.onClick.AddListener(SellClicked);

        base.Awake();
    }

    private static void SellClicked() {
        EventBus.SellPressed();
    }

    protected override Tooltip GetTooltipContent() {
        double returnValuePct = TowerConstants.SellReturnValue * 100;
        double duration = TowerConstants.SellDuration;
        return new StandardTooltip(
            $"Sell Tower",
            $"{returnValuePct}% gold return",
            "S",
            $"Sell this tower for {returnValuePct}% the accumulated gold it cost to build. Takes {duration} seconds and can be canceled before completion.",
            ""
        );
    }
}
