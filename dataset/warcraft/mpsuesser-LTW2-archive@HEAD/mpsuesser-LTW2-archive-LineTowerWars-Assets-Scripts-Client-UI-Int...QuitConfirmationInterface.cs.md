---
type: vendor-doc
source_repo: "https://github.com/mpsuesser/LTW2-archive"
source_path: "LineTowerWars/Assets/Scripts/Client/UI/InterfaceStates/InGame/QuitConfirmation/QuitConfirmationInterface.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class QuitConfirmationInterface : InterfaceState, IHandleKeyDownInput {
    public override bool ShouldShortCircuitBeforeCheckingBaseInterfaces =>
        true;
    
    [SerializeField] private Button ConfirmQuitButton;
    [SerializeField] private Button NevermindButton;

    private void Awake() {
        ConfirmQuitButton.onClick.AddListener(ConfirmQuitButtonPressed);
        NevermindButton.onClick.AddListener(NevermindButtonPressed);
    }
    
    private static readonly HashSet<KeyCode> keyDownSubscriptions =
        new HashSet<KeyCode>() {
            KeyCode.Return, KeyCode.Escape,
        };
    public HashSet<KeyCode> KeyDownSubscriptions => keyDownSubscriptions;
    public bool HandleInputKeyDown(KeyCode kc) {
        return kc switch {
            KeyCode.Return => ConfirmQuitPressed(),
            KeyCode.Escape => NevermindPressed(),
            _ => false
        };
    }

    private void NevermindButtonPressed() => NevermindPressed();
    private void ConfirmQuitButtonPressed() => ConfirmQuitPressed();
    
    private bool NevermindPressed() => Close();
    private static bool ConfirmQuitPressed() {
        ClientNetworkManager.Singleton.Disconnect();
        return true;
    }

}