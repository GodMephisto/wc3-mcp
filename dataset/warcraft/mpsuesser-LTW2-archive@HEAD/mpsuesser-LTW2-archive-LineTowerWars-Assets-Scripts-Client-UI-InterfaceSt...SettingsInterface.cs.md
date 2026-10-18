---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/common/SettingsMenu/SettingsInterface.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsInterface : InterfaceState, IHandleKeyDownInput {
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        true;
    
    [SerializeField] private Button BackButton;
    
    private HashSet<SettingsInterfaceContent> Contents { get; set; }

    private void Awake() {
        BackButton.onClick.AddListener(BackButtonPressed);
    }
    
    private static readonly HashSet<KeyCode> keyDownSubscriptions =
        new HashSet<KeyCode>() {
            KeyCode.Escape,
        };
    public HashSet<KeyCode> KeyDownSubscriptions => keyDownSubscriptions;
    public bool HandleInputKeyDown(KeyCode kc) {
        return kc switch {
            KeyCode.Escape => BackPressed(),
            _ => false
        };
    }

    private void Start() {
        PreheatAllContent();
    }

    private void BackButtonPressed() => BackPressed();
    private bool BackPressed() => Close();

    private void PreheatAllContent() {
        LTWLogger.Log("TODO: Maybe preheat if necessary");
        /* foreach (SettingsInterfaceContent content in Contents) {
            content.Preheat();
        } */
    }
}