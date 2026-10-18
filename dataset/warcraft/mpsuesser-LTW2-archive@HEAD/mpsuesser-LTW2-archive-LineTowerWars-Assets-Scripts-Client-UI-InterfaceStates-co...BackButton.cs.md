---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/common/BackButton.cs"
ref: HEAD
---

﻿public class BackButton : HoverTooltip {
    protected override Tooltip GetTooltipContent() {
        return new StandardTwoBodiesTooltip(
            "Back",
            "",
            "Go back to the base menu",
            ""
        );
    }
}