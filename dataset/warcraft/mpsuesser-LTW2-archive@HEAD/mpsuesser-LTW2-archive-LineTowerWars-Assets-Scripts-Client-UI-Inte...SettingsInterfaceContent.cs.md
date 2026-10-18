---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/common/SettingsMenu/SettingsInterfaceContent.cs"
ref: HEAD
---

﻿using UnityEngine;

public abstract class SettingsInterfaceContent : MonoBehaviour {
    [SerializeField] private GameObject content;

    protected abstract void LoadSettings();

    private void Start() {
        Invoke(nameof(LoadSettings), 0.1f);
    }

    private void Enable() {
        AdjustContentPosition();
    }

    private void AdjustContentPosition() {
        // TODO: Would be nice to adjust so that windows with few options aren't centered but rather build downward from the top
    }
}