---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/InGame/BaseMenu/Buttons/ButtonWithTooltip.cs"
ref: HEAD
---

﻿using UnityEngine.UI;

public abstract class ButtonWithTooltip : HoverTooltip {
    public Button Btn { get; private set; }

    protected override void Awake() {
        Btn = GetComponent<Button>();

        base.Awake();
    }
}